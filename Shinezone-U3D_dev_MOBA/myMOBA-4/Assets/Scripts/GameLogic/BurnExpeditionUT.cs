using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic.DataCenter;
using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameLogic
{
	public class BurnExpeditionUT
	{
		public static int GetIndex(string s)
		{
			return int.Parse(s.Substring(s.IndexOf("_") + 1));
		}

		public static int GetLevelCFGID(int level_index)
		{
			return 30501 + level_index;
		}

		public static CSDT_SINGLE_GAME_OF_BURNING Create_CSDT_SINGLE_GAME_OF_BURNING(int levelIndex)
		{
			return new CSDT_SINGLE_GAME_OF_BURNING
			{
				bLevelNo = Singleton<BurnExpeditionController>.GetInstance().model.Get_LevelNo(levelIndex),
				iLevelID = Singleton<BurnExpeditionController>.GetInstance().model.Get_LevelID(levelIndex)
			};
		}

		public static ResLevelCfgInfo Get_LevelConfigInfo(int levelIndex)
		{
			return GameDataMgr.burnMap.GetDataByKey((long)Singleton<BurnExpeditionController>.GetInstance().model.Get_LevelID(levelIndex));
		}

		public static void ApplyHP2Game(List<PoolObjHandle<ActorRoot>> actorList)
		{
			List<PoolObjHandle<ActorRoot>>.Enumerator enumerator = actorList.GetEnumerator();
			while (enumerator.MoveNext())
			{
				PoolObjHandle<ActorRoot> current = enumerator.Current;
				current.handle.ActorControl.bForceNotRevive = true;
				IGameActorDataProvider actorDataProvider = Singleton<ActorDataCenter>.instance.GetActorDataProvider(GameActorDataProviderType.ServerDataProvider);
				ActorServerData actorServerData = default(ActorServerData);
				IGameActorDataProvider arg_53_0 = actorDataProvider;
				PoolObjHandle<ActorRoot> current2 = enumerator.Current;
				arg_53_0.GetActorServerData(ref current2.handle.TheActorMeta, ref actorServerData);
				PoolObjHandle<ActorRoot> current3 = enumerator.Current;
				int actorHpTotal = current3.handle.ValueComponent.actorHpTotal;
				int actorHp = (int)BurnExpeditionUT.Convert_FactHP(actorServerData.TheBurnInfo.HeroRemainingHp, (uint)actorHpTotal);
				PoolObjHandle<ActorRoot> current4 = enumerator.Current;
				current4.handle.ValueComponent.actorHp = actorHp;
			}
		}

		public static void ApplyBuff(List<PoolObjHandle<ActorRoot>> actorList, int buffid)
		{
			if (buffid == 0)
			{
				return;
			}
			List<PoolObjHandle<ActorRoot>>.Enumerator enumerator = actorList.GetEnumerator();
			while (enumerator.MoveNext())
			{
				PoolObjHandle<ActorRoot> current = enumerator.Current;
				if (current.handle.IsHostCamp())
				{
					BufConsumer bufConsumer = new BufConsumer(buffid, enumerator.Current, enumerator.Current);
					bufConsumer.Use();
				}
			}
		}

		public static void Handle_Burn_Settle(ref SCPKG_SINGLEGAMEFINRSP rsp)
		{
			if (rsp.iErrCode == 0)
			{
				bool flag = rsp.stDetail.stGameInfo.bGameResult == 1;
				if (flag)
				{
					BurnExpeditionUT.Finish_Level(Singleton<BurnExpeditionController>.GetInstance().model.curSelect_LevelIndex);
				}
			}
			else
			{
				Singleton<CUIManager>.GetInstance().OpenMessageBox(string.Format(UT.GetText("Burn_Error_Settle_Failed"), rsp.iErrCode), false);
			}
		}

		public static uint Get_BloodTH(ActorRoot actor)
		{
			if (actor == null || actor.ValueComponent == null)
			{
				return 0u;
			}
			return BurnExpeditionUT.Get_BloodTH((uint)actor.ValueComponent.actorHp, (uint)actor.ValueComponent.actorHpTotal);
		}

		public static uint Get_BloodTH(uint curHP, uint max)
		{
			return BurnExpeditionUT.Convert_BloodTH(curHP, max);
		}

		public static uint Convert_BloodTH(uint curHp, uint max)
		{
			return curHp * 10000u / max;
		}

		public static uint Convert_FactHP(uint bloodTH, uint max)
		{
			return Math.Max(bloodTH * max / 10000u, 1u);
		}

		public static void Build_Burn_BattleParam(COMDT_SINGLE_GAME_PARAM param, bool bClickGameOver)
		{
			param.bGameType = (byte)COM_GAME_TYPE.COM_SINGLE_GAME_OF_BURNING;
			PlayerKDA hostKDA = Singleton<BattleStatistic>.GetInstance().m_playerKDAStat.GetHostKDA();
			if (hostKDA == null)
			{
				DebugHelper.Assert(hostKDA != null, "Failed find player kda");
				Singleton<BattleStatistic>.instance.m_playerKDAStat.DumpDebugInfo();
				return;
			}
			ListView<HeroKDA>.Enumerator enumerator = hostKDA.GetEnumerator();
			if (!bClickGameOver)
			{
				while (enumerator.MoveNext())
				{
					int configId = enumerator.Current.actorHero.handle.TheActorMeta.ConfigId;
					Singleton<BurnExpeditionController>.GetInstance().model.SetHero_Hp((uint)configId, (int)BurnExpeditionUT.Get_BloodTH(enumerator.Current.actorHero.handle));
				}
			}
			param.stGameDetail.stBurning = new COMDT_BURNING_ENEMY_HERO_DETAIL();
			COMDT_BURNING_HERO_INFO[] astHeroList = param.stGameDetail.stBurning.astHeroList;
			List<PoolObjHandle<ActorRoot>> heroActors = Singleton<GameObjMgr>.GetInstance().HeroActors;
			int num = 0;
			for (int i = 0; i < heroActors.Count; i++)
			{
				ActorRoot handle = heroActors[i].handle;
				if (!handle.IsHostCamp())
				{
					astHeroList[num].dwHeroID = (uint)handle.TheActorMeta.ConfigId;
					bool flag = handle.ValueComponent.actorHp == 0;
					uint num2;
					byte bIsDead;
					if (flag)
					{
						num2 = 0u;
						bIsDead = 1;
					}
					else
					{
						num2 = BurnExpeditionUT.Get_BloodTH(handle);
						bIsDead = 0;
					}
					astHeroList[num].dwBloodTTH = num2;
					astHeroList[num].bIsDead = bIsDead;
					if (!bClickGameOver)
					{
						BurnExpeditionUT.Record_EnemyHero_HPInfo(handle.TheActorMeta.ConfigId, num2);
					}
					num++;
				}
			}
			param.stGameDetail.stBurning.bHeroNum = (byte)num;
		}

		public static void Finish_Level(int level_index)
		{
			BurnExpeditionModel model = Singleton<BurnExpeditionController>.GetInstance().model;
			model.FinishLevel(level_index);
			model.UnLockBox(level_index);
			model.CalcProgress();
		}

		public static void Record_EnemyHero_HPInfo(int heroID, uint blood)
		{
			BurnExpeditionModel model = Singleton<BurnExpeditionController>.GetInstance().model;
			COMDT_PLAYERINFO current_Enemy_PlayerInfo = model.Get_Current_Enemy_PlayerInfo();
			for (int i = 0; i < current_Enemy_PlayerInfo.astChoiceHero.Length; i++)
			{
				COMDT_CHOICEHERO cOMDT_CHOICEHERO = current_Enemy_PlayerInfo.astChoiceHero[i];
				if (cOMDT_CHOICEHERO.stBaseInfo.stCommonInfo.dwHeroID != 0u && (ulong)cOMDT_CHOICEHERO.stBaseInfo.stCommonInfo.dwHeroID == (ulong)((long)heroID))
				{
					cOMDT_CHOICEHERO.stBurningInfo.dwBloodTTH = blood;
					if (blood == 0u)
					{
						cOMDT_CHOICEHERO.stBurningInfo.bIsDead = 1;
					}
					else
					{
						cOMDT_CHOICEHERO.stBurningInfo.bIsDead = 0;
					}
				}
			}
		}

		public static int Get_Buff_CDTime(int buffid)
		{
			ResSkillCombineCfgInfo dataByKey = GameDataMgr.skillCombineDatabin.GetDataByKey((long)buffid);
			if (dataByKey == null)
			{
				return -1;
			}
			return dataByKey.iDuration;
		}
	}
}
