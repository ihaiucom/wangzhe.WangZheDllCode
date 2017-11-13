using Assets.Scripts.GameLogic.DataCenter;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.GameSystem;
using CSProtocol;
using ResData;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public class SingleGameContext : GameContextBase
	{
		private struct Calc9SlotHeroData
		{
			public uint ConfigId;

			public int RecommendPos;

			public uint Ability;

			public uint Level;

			public int Quality;

			public int BornIndex;

			public bool selected;
		}

		public SingleGameContext(SCPKG_STARTSINGLEGAMERSP InMessage)
		{
			DebugHelper.Assert(InMessage != null, "输入不应该为null");
			this.RewardCount = (int)((InMessage != null) ? InMessage.dwRewardNum : 0u);
			Singleton<ActorDataCenter>.instance.ClearHeroServerData();
			if (Singleton<GamePlayerCenter>.instance.GetAllPlayers().get_Count() > 0)
			{
			}
			Singleton<GamePlayerCenter>.instance.ClearAllPlayers();
			if (InMessage == null || InMessage.stDetail.stSingleGameSucc == null || InMessage.stDetail.stSingleGameSucc.bNum < 1)
			{
				return;
			}
			this.DoNew9SlotCalc(InMessage);
			int num = Mathf.Min((int)InMessage.stDetail.stSingleGameSucc.bNum, InMessage.stDetail.stSingleGameSucc.astFighter.Length);
			for (int i = 0; i < num; i++)
			{
				COMDT_PLAYERINFO cOMDT_PLAYERINFO = InMessage.stDetail.stSingleGameSucc.astFighter[i];
				if (CheatCommandReplayEntry.heroPerformanceTest)
				{
					cOMDT_PLAYERINFO.astChoiceHero = InMessage.stDetail.stSingleGameSucc.astFighter[0].astChoiceHero;
				}
				if (cOMDT_PLAYERINFO.bObjType != 0)
				{
					string empty = string.Empty;
					int honorId = 0;
					int honorLevel = 0;
					uint gradeOfRank = 0u;
					uint wangZheCnt = 0u;
					ulong uid;
					uint logicWrold;
					uint level;
					if (cOMDT_PLAYERINFO.bObjType == 2)
					{
						if (InMessage.bGameType == 1 && Convert.ToBoolean(InMessage.stGameParam.stSingleGameRspOfCombat.bIsWarmBattle))
						{
							uid = cOMDT_PLAYERINFO.stDetail.stPlayerOfNpc.ullFakeUid;
							logicWrold = cOMDT_PLAYERINFO.stDetail.stPlayerOfNpc.dwFakeLogicWorldID;
							level = cOMDT_PLAYERINFO.stDetail.stPlayerOfNpc.dwFakePvpLevel;
						}
						else
						{
							uid = 0uL;
							logicWrold = 0u;
							level = cOMDT_PLAYERINFO.dwLevel;
						}
					}
					else
					{
						uid = ((cOMDT_PLAYERINFO.bObjType == 1) ? cOMDT_PLAYERINFO.stDetail.stPlayerOfAcnt.ullUid : 0uL);
						logicWrold = (uint)((cOMDT_PLAYERINFO.bObjType == 1) ? cOMDT_PLAYERINFO.stDetail.stPlayerOfAcnt.iLogicWorldID : 0);
						level = cOMDT_PLAYERINFO.dwLevel;
						honorId = cOMDT_PLAYERINFO.stDetail.stPlayerOfAcnt.iHonorID;
						honorLevel = cOMDT_PLAYERINFO.stDetail.stPlayerOfAcnt.iHonorLevel;
						gradeOfRank = (uint)((InMessage.stGameParam.stSingleGameRspOfCombat == null) ? 0 : InMessage.stGameParam.stSingleGameRspOfCombat.bGradeOfRank);
						wangZheCnt = cOMDT_PLAYERINFO.stDetail.stPlayerOfAcnt.dwWangZheCnt;
					}
					uint vipLv = 0u;
					if (cOMDT_PLAYERINFO.stDetail.stPlayerOfAcnt != null)
					{
						vipLv = cOMDT_PLAYERINFO.stDetail.stPlayerOfAcnt.stGameVip.dwCurLevel;
					}
					Player player = Singleton<GamePlayerCenter>.GetInstance().AddPlayer(cOMDT_PLAYERINFO.dwObjId, (COM_PLAYERCAMP)cOMDT_PLAYERINFO.bObjCamp, (int)cOMDT_PLAYERINFO.bPosOfCamp, level, cOMDT_PLAYERINFO.bObjType != 1, StringHelper.UTF8BytesToString(ref cOMDT_PLAYERINFO.szName), 0, (int)logicWrold, uid, vipLv, null, gradeOfRank, 0u, wangZheCnt, honorId, honorLevel, null, 0uL);
					if (player != null)
					{
						for (int j = 0; j < cOMDT_PLAYERINFO.astChoiceHero.Length; j++)
						{
							uint dwHeroID = cOMDT_PLAYERINFO.astChoiceHero[j].stBaseInfo.stCommonInfo.dwHeroID;
							player.AddHero(dwHeroID);
						}
						player.isGM = LobbyMsgHandler.isHostGMAcnt;
					}
					Singleton<ActorDataCenter>.instance.AddHeroesServerData(cOMDT_PLAYERINFO.dwObjId, cOMDT_PLAYERINFO.astChoiceHero);
				}
				if (cOMDT_PLAYERINFO.bObjType == 1)
				{
					Singleton<GamePlayerCenter>.GetInstance().SetHostPlayer(cOMDT_PLAYERINFO.dwObjId);
				}
			}
			if (InMessage.bGameType == 2)
			{
				CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
				if (masterRoleInfo != null)
				{
					GuideInfo guideInfo = default(GuideInfo);
					guideInfo.m_PlayerMobaLevel = masterRoleInfo.acntMobaInfo.iMobaLevel;
					Singleton<CBattleGuideManager>.instance.SetPlayerGuideInfo(ref guideInfo);
				}
			}
			this.LevelContext = CLevelCfgLogicManager.CreatePveLevelContext(InMessage);
		}

		public override GameInfoBase CreateGameInfo()
		{
			SingleGameInfo singleGameInfo = new SingleGameInfo();
			singleGameInfo.Initialize(this);
			return singleGameInfo;
		}

		protected void DoNew9SlotCalc(SCPKG_STARTSINGLEGAMERSP inMessage)
		{
			if (inMessage.bGameType == 8 || inMessage.bGameType == 7)
			{
				this.Calc9SlotHeroStandingPosition(inMessage.stDetail.stSingleGameSucc);
			}
		}

		private void Calc9SlotHeroStandingPosition(CSDT_BATTLE_PLAYER_BRIEF stBattlePlayer)
		{
			SingleGameContext.Calc9SlotHeroData[] array = new SingleGameContext.Calc9SlotHeroData[3];
			IGameActorDataProvider actorDataProvider = Singleton<ActorDataCenter>.instance.GetActorDataProvider(GameActorDataProviderType.StaticLobbyDataProvider);
			ActorStaticData actorStaticData = default(ActorStaticData);
			ActorMeta actorMeta = default(ActorMeta);
			if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo() == null)
			{
				return;
			}
			int num = 0;
			while (num < stBattlePlayer.astFighter[0].astChoiceHero.Length && num < 3)
			{
				uint dwHeroID = stBattlePlayer.astFighter[0].astChoiceHero[num].stBaseInfo.stCommonInfo.dwHeroID;
				if (dwHeroID != 0u)
				{
					actorMeta.ConfigId = (int)dwHeroID;
					actorDataProvider.GetActorStaticData(ref actorMeta, ref actorStaticData);
					array[num].Level = 1u;
					array[num].Quality = 1;
					array[num].RecommendPos = actorStaticData.TheHeroOnlyInfo.RecommendStandPos;
					array[num].Ability = (uint)CHeroDataFactory.CreateHeroData(dwHeroID).combatEft;
					array[num].ConfigId = dwHeroID;
					array[num].selected = false;
					array[num].BornIndex = -1;
				}
				num++;
			}
			this.ImpCalc9SlotHeroStandingPosition(ref array);
			int num2 = 0;
			while (num2 < stBattlePlayer.astFighter[0].astChoiceHero.Length && num2 < 3)
			{
				stBattlePlayer.astFighter[0].astChoiceHero[num2].stHeroExtral.iHeroPos = array[num2].BornIndex;
				num2++;
			}
			int num3 = 0;
			while (num3 < stBattlePlayer.astFighter[1].astChoiceHero.Length && num3 < 3)
			{
				uint dwHeroID2 = stBattlePlayer.astFighter[1].astChoiceHero[num3].stBaseInfo.stCommonInfo.dwHeroID;
				if (dwHeroID2 != 0u)
				{
					actorMeta.ConfigId = (int)dwHeroID2;
					actorDataProvider.GetActorStaticData(ref actorMeta, ref actorStaticData);
					array[num3].Level = (uint)stBattlePlayer.astFighter[1].astChoiceHero[num3].stBaseInfo.stCommonInfo.wLevel;
					array[num3].Quality = (int)stBattlePlayer.astFighter[1].astChoiceHero[num3].stBaseInfo.stCommonInfo.stQuality.wQuality;
					array[num3].RecommendPos = actorStaticData.TheHeroOnlyInfo.RecommendStandPos;
					array[num3].Ability = (uint)CHeroDataFactory.CreateHeroData(dwHeroID2).combatEft;
					array[num3].ConfigId = dwHeroID2;
					array[num3].selected = false;
					array[num3].BornIndex = -1;
				}
				num3++;
			}
			this.ImpCalc9SlotHeroStandingPosition(ref array);
			int num4 = 0;
			while (num4 < stBattlePlayer.astFighter[1].astChoiceHero.Length && num4 < 3)
			{
				stBattlePlayer.astFighter[1].astChoiceHero[num4].stHeroExtral.iHeroPos = array[num4].BornIndex;
				num4++;
			}
		}

		private void ImpCalc9SlotHeroStandingPosition(ref SingleGameContext.Calc9SlotHeroData[] heroes)
		{
			List<int> list = this.HasPositionHero(ref heroes, RES_HERO_RECOMMEND_POSITION.RES_HERO_RECOMMEND_POSITION_T_FRONT);
			switch (list.get_Count())
			{
			case 1:
			{
				for (int i = 0; i < 3; i++)
				{
					if (heroes[i].RecommendPos == 0)
					{
						heroes[i].selected = true;
						heroes[i].BornIndex = 1;
						break;
					}
				}
				int num = this.WhoIsBestHero(ref heroes);
				heroes[num].selected = true;
				if (heroes[num].RecommendPos == 1)
				{
					heroes[num].BornIndex = 3;
					num = this.WhoIsBestHero(ref heroes);
					heroes[num].selected = true;
					heroes[num].BornIndex = ((heroes[num].RecommendPos == 1) ? 5 : 8);
				}
				else
				{
					heroes[num].BornIndex = 8;
					num = this.WhoIsBestHero(ref heroes);
					heroes[num].selected = true;
					heroes[num].BornIndex = ((heroes[num].RecommendPos == 1) ? 3 : 6);
				}
				break;
			}
			case 2:
			{
				for (int j = 0; j < 3; j++)
				{
					if (heroes[j].RecommendPos == 1)
					{
						heroes[j].selected = true;
						heroes[j].BornIndex = 3;
						break;
					}
					if (heroes[j].RecommendPos == 2)
					{
						heroes[j].selected = true;
						heroes[j].BornIndex = 6;
						break;
					}
				}
				int num2 = this.WhoIsBestHero(ref heroes);
				heroes[num2].selected = true;
				heroes[num2].BornIndex = 1;
				num2 = this.WhoIsBestHero(ref heroes);
				heroes[num2].selected = true;
				heroes[num2].BornIndex = 0;
				break;
			}
			case 3:
			{
				int num3 = this.WhoIsBestHero(ref heroes);
				heroes[num3].selected = true;
				heroes[num3].BornIndex = 1;
				num3 = this.WhoIsBestHero(ref heroes);
				heroes[num3].selected = true;
				heroes[num3].BornIndex = 0;
				num3 = this.WhoIsBestHero(ref heroes);
				heroes[num3].selected = true;
				heroes[num3].BornIndex = 2;
				break;
			}
			default:
				list = this.HasPositionHero(ref heroes, RES_HERO_RECOMMEND_POSITION.RES_HERO_RECOMMEND_POSITION_T_CENTER);
				switch (list.get_Count())
				{
				case 1:
				{
					for (int k = 0; k < 3; k++)
					{
						if (heroes[k].RecommendPos == 1)
						{
							heroes[k].selected = true;
							heroes[k].BornIndex = 1;
							break;
						}
					}
					int num4 = this.WhoIsBestHero(ref heroes);
					heroes[num4].selected = true;
					heroes[num4].BornIndex = 8;
					num4 = this.WhoIsBestHero(ref heroes);
					heroes[num4].selected = true;
					heroes[num4].BornIndex = 6;
					break;
				}
				case 2:
				{
					for (int l = 0; l < 3; l++)
					{
						if (heroes[l].RecommendPos == 2)
						{
							heroes[l].selected = true;
							heroes[l].BornIndex = 3;
							break;
						}
					}
					int num5 = this.WhoIsBestHero(ref heroes);
					heroes[num5].selected = true;
					heroes[num5].BornIndex = 1;
					num5 = this.WhoIsBestHero(ref heroes);
					heroes[num5].selected = true;
					heroes[num5].BornIndex = 0;
					break;
				}
				case 3:
				{
					int num6 = this.WhoIsBestHero(ref heroes);
					heroes[num6].selected = true;
					heroes[num6].BornIndex = 1;
					num6 = this.WhoIsBestHero(ref heroes);
					heroes[num6].selected = true;
					heroes[num6].BornIndex = 0;
					num6 = this.WhoIsBestHero(ref heroes);
					heroes[num6].selected = true;
					heroes[num6].BornIndex = 2;
					break;
				}
				default:
				{
					int num7 = this.WhoIsBestHero(ref heroes);
					heroes[num7].selected = true;
					heroes[num7].BornIndex = 4;
					num7 = this.WhoIsBestHero(ref heroes);
					heroes[num7].selected = true;
					heroes[num7].BornIndex = 3;
					num7 = this.WhoIsBestHero(ref heroes);
					heroes[num7].selected = true;
					heroes[num7].BornIndex = 5;
					break;
				}
				}
				break;
			}
		}

		private List<int> HasPositionHero(ref SingleGameContext.Calc9SlotHeroData[] heroes, RES_HERO_RECOMMEND_POSITION pos)
		{
			List<int> list = new List<int>();
			for (int i = 0; i < 3; i++)
			{
				if (heroes[i].RecommendPos == (int)pos)
				{
					list.Add(i);
				}
			}
			return list;
		}

		private int WhoIsBestHero(ref SingleGameContext.Calc9SlotHeroData[] heroes)
		{
			if (this.IsBetterHero(ref heroes[0], ref heroes[1]) && this.IsBetterHero(ref heroes[0], ref heroes[2]))
			{
				return 0;
			}
			if (this.IsBetterHero(ref heroes[1], ref heroes[0]) && this.IsBetterHero(ref heroes[1], ref heroes[2]))
			{
				return 1;
			}
			return 2;
		}

		private bool IsBetterHero(ref SingleGameContext.Calc9SlotHeroData heroe1, ref SingleGameContext.Calc9SlotHeroData heroe2)
		{
			return heroe1.ConfigId > 0u && !heroe1.selected && (heroe2.ConfigId == 0u || heroe2.selected || heroe1.Ability > heroe2.Ability || (heroe1.Ability == heroe2.Ability && heroe1.Level > heroe2.Level) || (heroe1.Ability == heroe2.Ability && heroe1.Level == heroe2.Level && heroe1.Quality >= heroe2.Quality));
		}
	}
}
