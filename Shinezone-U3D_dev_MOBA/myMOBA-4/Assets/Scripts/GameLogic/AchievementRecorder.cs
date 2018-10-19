using Assets.Scripts.Common;
using Assets.Scripts.GameLogic.GameKernal;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameLogic
{
	public class AchievementRecorder
	{
		private bool bFristBlood;

		public void StartRecord()
		{
			this.Clear();
			Singleton<GameEventSys>.instance.AddEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.OnActorDeath));
		}

		public void Clear()
		{
			this.bFristBlood = false;
			Singleton<GameEventSys>.instance.RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.OnActorDeath));
		}

		private void OnActorDeath(ref GameDeadEventParam prm)
		{
			if (!prm.src || !prm.orignalAtker || prm.bImmediateRevive || prm.bSuicide)
			{
				return;
			}
			bool flag = false;
			byte actorSubSoliderType = prm.src.handle.ActorControl.GetActorSubSoliderType();
			if (actorSubSoliderType == 8 || actorSubSoliderType == 9)
			{
				flag = true;
			}
			if (prm.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster && prm.src.handle.TheActorMeta.ConfigId != Singleton<BattleLogic>.instance.DragonId && !flag)
			{
				return;
			}
			KillDetailInfo killDetailInfo = this.OnActorDeathd(ref prm);
			if (killDetailInfo != null)
			{
				Singleton<EventRouter>.instance.BroadCastEvent<KillDetailInfo>(EventID.AchievementRecorderEvent, killDetailInfo);
			}
		}

		private KillDetailInfo OnActorDeathd(ref GameDeadEventParam param)
		{
			KillDetailInfo killDetailInfo = null;
			KillDetailInfoType killDetailInfoType = KillDetailInfoType.Info_Type_None;
			KillDetailInfoType heroMultiKillType = KillDetailInfoType.Info_Type_None;
			KillDetailInfoType heroContiKillType = KillDetailInfoType.Info_Type_None;
			PoolObjHandle<ActorRoot> poolObjHandle = default(PoolObjHandle<ActorRoot>);
			PoolObjHandle<ActorRoot> poolObjHandle2 = default(PoolObjHandle<ActorRoot>);
			List<uint> list = new List<uint>();
			DefaultGameEventParam defaultGameEventParam = new DefaultGameEventParam(param.src, param.atker, ref param.orignalAtker, ref param.logicAtker);
			HeroWrapper heroWrapper = null;
			HeroWrapper heroWrapper2 = null;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
			DebugHelper.Assert(hostPlayer != null, "Fatal error in OnActorDeadthd, HostPlayer is null!");
			if (hostPlayer != null)
			{
				DebugHelper.Assert(hostPlayer.Captain, "Fatal error in OnActorDeadthd, Captain is null!");
			}
			bool bSelfCamp = hostPlayer.PlayerCamp == defaultGameEventParam.orignalAtker.handle.TheActorMeta.ActorCamp;
			uint objID = hostPlayer.Captain.handle.ObjID;
			bool bPlayerSelf_KillOrKilled = objID == defaultGameEventParam.src.handle.ObjID || objID == defaultGameEventParam.orignalAtker.handle.ObjID;
			bool flag4 = defaultGameEventParam.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero;
			bool flag5 = defaultGameEventParam.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ;
			byte actorSubSoliderType = defaultGameEventParam.src.handle.ActorControl.GetActorSubSoliderType();
			bool flag6 = false;
			bool flag7 = false;
			bool flag8 = false;
			if (defaultGameEventParam.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster && actorSubSoliderType == 8)
			{
				flag6 = true;
			}
			else if (defaultGameEventParam.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster && actorSubSoliderType == 9)
			{
				flag7 = true;
			}
			else if (defaultGameEventParam.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster && actorSubSoliderType == 7)
			{
				flag8 = true;
			}
			poolObjHandle = defaultGameEventParam.src;
			poolObjHandle2 = defaultGameEventParam.orignalAtker;
			if (flag4)
			{
				heroWrapper = (defaultGameEventParam.src.handle.ActorControl as HeroWrapper);
				heroWrapper2 = (defaultGameEventParam.orignalAtker.handle.ActorControl as HeroWrapper);
				if (defaultGameEventParam.orignalAtker.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
				{
					if (defaultGameEventParam.orignalAtker.handle.ObjID == objID)
					{
						bPlayerSelf_KillOrKilled = true;
					}
					flag = true;
					bSelfCamp = (hostPlayer.PlayerCamp == defaultGameEventParam.orignalAtker.handle.TheActorMeta.ActorCamp);
				}
				else if (heroWrapper.IsKilledByHero())
				{
					flag = true;
					poolObjHandle2 = heroWrapper.LastHeroAtker;
					heroWrapper2 = (heroWrapper.LastHeroAtker.handle.ActorControl as HeroWrapper);
					if (poolObjHandle2.handle.ObjID == objID)
					{
						bPlayerSelf_KillOrKilled = true;
					}
					bSelfCamp = (hostPlayer.PlayerCamp == poolObjHandle2.handle.TheActorMeta.ActorCamp);
				}
				else if (defaultGameEventParam.orignalAtker.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ)
				{
					flag2 = true;
					flag = false;
					bSelfCamp = (hostPlayer.PlayerCamp == defaultGameEventParam.orignalAtker.handle.TheActorMeta.ActorCamp);
				}
				else if (defaultGameEventParam.orignalAtker.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster)
				{
					flag3 = true;
					flag = false;
					bSelfCamp = (hostPlayer.PlayerCamp == defaultGameEventParam.orignalAtker.handle.TheActorMeta.ActorCamp);
				}
				if (flag4 && flag)
				{
					heroWrapper2.ContiDeadNum = 0;
					heroWrapper2.ContiKillNum++;
					if (heroWrapper2.IsInMultiKill())
					{
						heroWrapper2.MultiKillNum++;
					}
					else
					{
						heroWrapper2.MultiKillNum = 1;
					}
					heroWrapper2.UpdateLastKillTime();
				}
			}
			if (flag4 && flag)
			{
				if (poolObjHandle && poolObjHandle.handle.ActorControl != null)
				{
					List<KeyValuePair<uint, ulong>>.Enumerator enumerator = poolObjHandle.handle.ActorControl.hurtSelfActorList.GetEnumerator();
					while (enumerator.MoveNext())
					{
						KeyValuePair<uint, ulong> current = enumerator.Current;
						if (current.Key != poolObjHandle2.handle.ObjID)
						{
							List<uint> arg_43B_0 = list;
							KeyValuePair<uint, ulong> current2 = enumerator.Current;
							arg_43B_0.Add(current2.Key);
						}
					}
				}
				if (poolObjHandle2 && poolObjHandle2.handle.ActorControl != null)
				{
					List<KeyValuePair<uint, ulong>>.Enumerator enumerator2 = poolObjHandle2.handle.ActorControl.helpSelfActorList.GetEnumerator();
					while (enumerator2.MoveNext())
					{
						KeyValuePair<uint, ulong> current3 = enumerator2.Current;
						if (current3.Key != poolObjHandle2.handle.ObjID)
						{
							List<uint> arg_4B9_0 = list;
							KeyValuePair<uint, ulong> current4 = enumerator2.Current;
							arg_4B9_0.Add(current4.Key);
						}
					}
				}
				for (int i = 0; i < list.Count - 1; i++)
				{
					for (int j = i + 1; j < list.Count; j++)
					{
						if (list[i] == list[j])
						{
							list.RemoveAt(j);
							j--;
						}
					}
				}
				bool flag9 = false;
				if (heroWrapper2.MultiKillNum == 2)
				{
					flag9 = true;
					heroMultiKillType = KillDetailInfoType.Info_Type_DoubleKill;
					Singleton<GameEventSys>.GetInstance().SendEvent<DefaultGameEventParam>(GameEventDef.Event_DoubleKill, ref defaultGameEventParam);
				}
				else if (heroWrapper2.MultiKillNum == 3)
				{
					flag9 = true;
					heroMultiKillType = KillDetailInfoType.Info_Type_TripleKill;
					Singleton<GameEventSys>.GetInstance().SendEvent<DefaultGameEventParam>(GameEventDef.Event_TripleKill, ref defaultGameEventParam);
				}
				else if (heroWrapper2.MultiKillNum == 4)
				{
					flag9 = true;
					heroMultiKillType = KillDetailInfoType.Info_Type_QuataryKill;
					Singleton<GameEventSys>.GetInstance().SendEvent<DefaultGameEventParam>(GameEventDef.Event_QuataryKill, ref defaultGameEventParam);
				}
				else if (heroWrapper2.MultiKillNum >= 5)
				{
					flag9 = true;
					heroMultiKillType = KillDetailInfoType.Info_Type_PentaKill;
					Singleton<GameEventSys>.GetInstance().SendEvent<DefaultGameEventParam>(GameEventDef.Event_PentaKill, ref defaultGameEventParam);
				}
				if (flag9 && killDetailInfo == null)
				{
					killDetailInfo = this._create(poolObjHandle2, poolObjHandle, list, killDetailInfoType, heroMultiKillType, killDetailInfoType, bSelfCamp, false, bPlayerSelf_KillOrKilled, false);
				}
			}
			if (flag4 && flag && heroWrapper.ContiKillNum >= 3)
			{
				if (heroWrapper.ContiKillNum >= 7)
				{
					Singleton<GameEventSys>.GetInstance().SendEvent<DefaultGameEventParam>(GameEventDef.Event_OdysseyBeStopped, ref defaultGameEventParam);
				}
				if (killDetailInfo == null)
				{
					killDetailInfo = this._create(poolObjHandle2, poolObjHandle, list, KillDetailInfoType.Info_Type_StopMultiKill, heroMultiKillType, killDetailInfoType, bSelfCamp, false, bPlayerSelf_KillOrKilled, false);
				}
			}
			if (flag4 && flag && !this.bFristBlood)
			{
				this.bFristBlood = true;
				return this._create(poolObjHandle2, poolObjHandle, list, KillDetailInfoType.Info_Type_First_Kill, heroMultiKillType, killDetailInfoType, bSelfCamp, false, bPlayerSelf_KillOrKilled, false);
			}
			if (flag5 && (defaultGameEventParam.src.handle.TheStaticData.TheOrganOnlyInfo.OrganType == 1 || defaultGameEventParam.src.handle.TheStaticData.TheOrganOnlyInfo.OrganType == 4))
			{
				KillDetailInfo killDetailInfo2 = this._create(poolObjHandle2, poolObjHandle, list, KillDetailInfoType.Info_Type_DestroyTower, killDetailInfoType, killDetailInfoType, bSelfCamp, false, bPlayerSelf_KillOrKilled, false);
				if (defaultGameEventParam.src.handle.TheStaticData.TheOrganOnlyInfo.DeadEnemySoldier > 0)
				{
					Singleton<EventRouter>.instance.BroadCastEvent<KillDetailInfo>(EventID.AchievementRecorderEvent, killDetailInfo2);
					return this._create(poolObjHandle2, poolObjHandle, list, KillDetailInfoType.Info_Type_Soldier_Boosted, killDetailInfoType, killDetailInfoType, bSelfCamp, false, bPlayerSelf_KillOrKilled, false);
				}
				return killDetailInfo2;
			}
			else
			{
				if (flag4 && flag)
				{
					bool flag10 = false;
					if (heroWrapper2.ContiKillNum == 3)
					{
						flag10 = true;
						heroContiKillType = KillDetailInfoType.Info_Type_MonsterKill;
					}
					else if (heroWrapper2.ContiKillNum == 4)
					{
						flag10 = true;
						heroContiKillType = KillDetailInfoType.Info_Type_DominateBattle;
					}
					else if (heroWrapper2.ContiKillNum == 5)
					{
						flag10 = true;
						heroContiKillType = KillDetailInfoType.Info_Type_Legendary;
					}
					else if (heroWrapper2.ContiKillNum == 6)
					{
						flag10 = true;
						heroContiKillType = KillDetailInfoType.Info_Type_TotalAnnihilat;
					}
					else if (heroWrapper2.ContiKillNum >= 7)
					{
						flag10 = true;
						heroContiKillType = KillDetailInfoType.Info_Type_Odyssey;
						Singleton<GameEventSys>.GetInstance().SendEvent<DefaultGameEventParam>(GameEventDef.Event_Odyssey, ref defaultGameEventParam);
					}
					if (flag10 && killDetailInfo == null)
					{
						killDetailInfo = this._create(poolObjHandle2, poolObjHandle, list, killDetailInfoType, killDetailInfoType, heroContiKillType, bSelfCamp, false, bPlayerSelf_KillOrKilled, false);
					}
				}
				if (flag4 && this.IsAllDead(ref defaultGameEventParam.src))
				{
					KillDetailInfo arg = this._create(poolObjHandle2, poolObjHandle, list, killDetailInfoType, killDetailInfoType, killDetailInfoType, bSelfCamp, true, bPlayerSelf_KillOrKilled, false);
					Singleton<EventRouter>.instance.BroadCastEvent<KillDetailInfo>(EventID.AchievementRecorderEvent, arg);
				}
				if (killDetailInfo != null)
				{
					return killDetailInfo;
				}
				if (flag4 && (flag || flag2 || flag3))
				{
					return this._create(poolObjHandle2, poolObjHandle, list, KillDetailInfoType.Info_Type_Kill, killDetailInfoType, killDetailInfoType, bSelfCamp, false, bPlayerSelf_KillOrKilled, false);
				}
				if (flag8)
				{
					return this._create(poolObjHandle2, poolObjHandle, list, KillDetailInfoType.Info_Type_Kill_3V3_Dragon, killDetailInfoType, killDetailInfoType, bSelfCamp, false, bPlayerSelf_KillOrKilled, param.bSuicide);
				}
				if (flag7)
				{
					Singleton<EventRouter>.GetInstance().BroadCastEvent<PoolObjHandle<ActorRoot>, int>(EventID.BATTLE_DATAANL_DRAGON_KILLED, poolObjHandle2, 0);
					return this._create(poolObjHandle2, poolObjHandle, list, KillDetailInfoType.Info_Type_Kill_5V5_SmallDragon, killDetailInfoType, killDetailInfoType, bSelfCamp, false, bPlayerSelf_KillOrKilled, param.bSuicide);
				}
				if (flag6)
				{
					Singleton<EventRouter>.GetInstance().BroadCastEvent<PoolObjHandle<ActorRoot>, int>(EventID.BATTLE_DATAANL_DRAGON_KILLED, poolObjHandle2, 1);
					return this._create(poolObjHandle2, poolObjHandle, list, KillDetailInfoType.Info_Type_Kill_5V5_BigDragon, killDetailInfoType, killDetailInfoType, bSelfCamp, false, bPlayerSelf_KillOrKilled, param.bSuicide);
				}
				return null;
			}
		}

		private bool IsAllDead(ref PoolObjHandle<ActorRoot> actorHandle)
		{
			if (!actorHandle)
			{
				return false;
			}
			List<Player> allCampPlayers = Singleton<GamePlayerCenter>.instance.GetAllCampPlayers(actorHandle.handle.TheActorMeta.ActorCamp);
			if (allCampPlayers.Count <= 1)
			{
				return false;
			}
			if (allCampPlayers != null)
			{
				for (int i = 0; i < allCampPlayers.Count; i++)
				{
					Player player = allCampPlayers[i];
					if (player != null && !player.IsAllHeroesDead())
					{
						return false;
					}
				}
			}
			return true;
		}

		private KillDetailInfo _create(PoolObjHandle<ActorRoot> killer, PoolObjHandle<ActorRoot> victim, List<uint> assistList, KillDetailInfoType type, KillDetailInfoType HeroMultiKillType, KillDetailInfoType HeroContiKillType, bool bSelfCamp, bool bAllDead, bool bPlayerSelf_KillOrKilled, bool bSuicide = false)
		{
			return new KillDetailInfo
			{
				Killer = killer,
				Victim = victim,
				assistList = assistList,
				Type = type,
				HeroMultiKillType = HeroMultiKillType,
				HeroContiKillType = HeroContiKillType,
				bSelfCamp = bSelfCamp,
				bAllDead = bAllDead,
				bPlayerSelf_KillOrKilled = bPlayerSelf_KillOrKilled,
				bSuicide = bSuicide
			};
		}
	}
}
