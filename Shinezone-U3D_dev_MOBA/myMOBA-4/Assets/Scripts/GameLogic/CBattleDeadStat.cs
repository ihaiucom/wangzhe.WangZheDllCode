using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using CSProtocol;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameLogic
{
	public class CBattleDeadStat
	{
		private List<DeadRecord> m_deadRecordList = new List<DeadRecord>(96);

		private int m_deadMonsterNum;

		public uint m_uiFBTime;

		public uint m_uiTakeFBPlayerId;

		private int m_baojunEnterCombatTime;

		private int m_baronEnterCombatTime;

		private int m_bigDragonEnterCombatTime;

		private int[] m_arrHeroEnterCombatTime = new int[10];

		public int enemyKillHeroMaxGap;

		public void StartRecord()
		{
			this.Clear();
			this.AddEventHandler();
		}

		public void Clear()
		{
			this.m_deadMonsterNum = 0;
			this.m_deadRecordList.Clear();
			this.RemoveEventHandler();
			this.enemyKillHeroMaxGap = 0;
			for (int i = 0; i < 10; i++)
			{
				this.m_arrHeroEnterCombatTime[0] = 0;
			}
			this.m_uiFBTime = 0u;
			this.m_uiTakeFBPlayerId = 0u;
		}

		public void AddEventHandler()
		{
			Singleton<GameEventSys>.instance.AddEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.OnDeadRecord));
			Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorEnterCombat, new RefAction<DefaultGameEventParam>(this.OnEnterCombat));
		}

		public void RemoveEventHandler()
		{
			Singleton<GameEventSys>.instance.RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.OnDeadRecord));
			Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorEnterCombat, new RefAction<DefaultGameEventParam>(this.OnEnterCombat));
		}

		private void OnEnterCombat(ref DefaultGameEventParam prm)
		{
			if (prm.src && prm.src.handle.ActorControl != null && prm.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster && prm.src.handle.ActorControl.GetActorSubType() == 2)
			{
				if (prm.src.handle.ActorControl.GetActorSubSoliderType() == 7)
				{
					this.m_baojunEnterCombatTime = (int)Singleton<FrameSynchr>.instance.LogicFrameTick;
				}
				else if (prm.src.handle.ActorControl.GetActorSubSoliderType() == 8)
				{
					this.m_baronEnterCombatTime = (int)Singleton<FrameSynchr>.instance.LogicFrameTick;
				}
				else if (prm.src.handle.ActorControl.GetActorSubSoliderType() == 9)
				{
					this.m_bigDragonEnterCombatTime = (int)Singleton<FrameSynchr>.instance.LogicFrameTick;
				}
			}
			else if (prm.src && prm.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
			{
				List<PoolObjHandle<ActorRoot>> heroActors = Singleton<GameObjMgr>.instance.HeroActors;
				int count = heroActors.Count;
				for (int i = 0; i < count; i++)
				{
					if (heroActors[i].handle.ObjID == prm.src.handle.ObjID && i < 10)
					{
						this.m_arrHeroEnterCombatTime[i] = (int)Singleton<FrameSynchr>.instance.LogicFrameTick;
						break;
					}
				}
			}
		}

		private void OnDeadRecord(ref GameDeadEventParam prm)
		{
			if (prm.bImmediateRevive)
			{
				return;
			}
			PoolObjHandle<ActorRoot> src = prm.src;
			PoolObjHandle<ActorRoot> logicAtker = prm.logicAtker;
			if (!src || !logicAtker)
			{
				return;
			}
			if (src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
			{
				DeadRecord item = new DeadRecord(src.handle.TheActorMeta.ActorCamp, src.handle.TheActorMeta.ActorType, src.handle.TheActorMeta.ConfigId, (int)Singleton<FrameSynchr>.instance.LogicFrameTick, logicAtker.handle.TheActorMeta.ActorCamp, logicAtker.handle.TheActorMeta.PlayerId, logicAtker.handle.TheActorMeta.ActorType);
				List<PoolObjHandle<ActorRoot>> heroActors = Singleton<GameObjMgr>.instance.HeroActors;
				int count = heroActors.Count;
				for (int i = 0; i < count; i++)
				{
					if (heroActors[i].handle.ObjID == prm.src.handle.ObjID && i < 10)
					{
						item.fightTime = (int)Singleton<FrameSynchr>.instance.LogicFrameTick - this.m_arrHeroEnterCombatTime[i];
						break;
					}
				}
				this.m_deadRecordList.Add(item);
				if (this.m_uiFBTime == 0u)
				{
					this.m_uiFBTime = (uint)Singleton<FrameSynchr>.instance.LogicFrameTick;
					if (prm.orignalAtker)
					{
						this.m_uiTakeFBPlayerId = prm.orignalAtker.handle.TheActorMeta.PlayerId;
					}
				}
			}
			else if (src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster)
			{
				DeadRecord item2 = new DeadRecord(src.handle.TheActorMeta.ActorCamp, src.handle.TheActorMeta.ActorType, src.handle.TheActorMeta.ConfigId, (int)Singleton<FrameSynchr>.instance.LogicFrameTick, logicAtker.handle.TheActorMeta.ActorCamp, logicAtker.handle.TheActorMeta.PlayerId, logicAtker.handle.TheActorMeta.ActorType);
				if (src.handle.ActorControl != null)
				{
					item2.actorSubType = src.handle.ActorControl.GetActorSubType();
					item2.actorSubSoliderType = src.handle.ActorControl.GetActorSubSoliderType();
					if (item2.actorSubType == 2)
					{
						if (item2.actorSubSoliderType == 7)
						{
							item2.fightTime = (int)Singleton<FrameSynchr>.instance.LogicFrameTick - this.m_baojunEnterCombatTime;
						}
						else if (item2.actorSubSoliderType == 8)
						{
							item2.fightTime = (int)Singleton<FrameSynchr>.instance.LogicFrameTick - this.m_baronEnterCombatTime;
						}
						else if (item2.actorSubSoliderType == 9)
						{
							item2.fightTime = (int)Singleton<FrameSynchr>.instance.LogicFrameTick - this.m_bigDragonEnterCombatTime;
						}
					}
				}
				this.m_deadRecordList.Add(item2);
				this.m_deadMonsterNum++;
			}
			else if (src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ && (src.handle.TheStaticData.TheOrganOnlyInfo.OrganType == 1 || src.handle.TheStaticData.TheOrganOnlyInfo.OrganType == 4 || src.handle.TheStaticData.TheOrganOnlyInfo.OrganType == 2))
			{
				DeadRecord item3 = new DeadRecord(src.handle.TheActorMeta.ActorCamp, src.handle.TheActorMeta.ActorType, src.handle.TheActorMeta.ConfigId, (int)Singleton<FrameSynchr>.instance.LogicFrameTick, logicAtker.handle.TheActorMeta.ActorCamp, logicAtker.handle.TheActorMeta.PlayerId, logicAtker.handle.TheActorMeta.ActorType);
				if (src.handle.ObjLinker != null)
				{
					item3.iOrder = src.handle.ObjLinker.BattleOrder;
					item3.actorSubType = (byte)src.handle.TheStaticData.TheOrganOnlyInfo.OrganType;
				}
				this.m_deadRecordList.Add(item3);
			}
		}

		public int GetOrganTimeByOrder(int iOrder)
		{
			for (int i = 0; i < this.m_deadRecordList.Count; i++)
			{
				if (this.m_deadRecordList[i].actorType == ActorTypeDef.Actor_Type_Organ && this.m_deadRecordList[i].iOrder == iOrder)
				{
					return this.m_deadRecordList[i].deadTime;
				}
			}
			return 0;
		}

		public int GetDeadTime(COM_PLAYERCAMP camp, ActorTypeDef actorType, int index)
		{
			int num = 0;
			for (int i = 0; i < this.m_deadRecordList.Count; i++)
			{
				if (actorType == this.m_deadRecordList[i].actorType && camp == this.m_deadRecordList[i].camp)
				{
					if (num == index)
					{
						return this.m_deadRecordList[i].deadTime;
					}
					num++;
				}
			}
			return 0;
		}

		public int GetDeadNum(COM_PLAYERCAMP camp, ActorTypeDef actorType, int subType, int cfgId)
		{
			int num = 0;
			for (int i = 0; i < this.m_deadRecordList.Count; i++)
			{
				if (camp == this.m_deadRecordList[i].camp && actorType == this.m_deadRecordList[i].actorType && subType == (int)this.m_deadRecordList[i].actorSubType && cfgId == this.m_deadRecordList[i].cfgId)
				{
					num++;
				}
			}
			return num;
		}

		public int GetDragonDeadTime(int index)
		{
			int num = 0;
			for (int i = 0; i < this.m_deadRecordList.Count; i++)
			{
				if (this.m_deadRecordList[i].actorType == ActorTypeDef.Actor_Type_Monster && this.m_deadRecordList[i].actorSubType == 2 && (this.m_deadRecordList[i].actorSubSoliderType == 9 || this.m_deadRecordList[i].actorSubSoliderType == 7))
				{
					if (num == index)
					{
						return this.m_deadRecordList[i].deadTime;
					}
					num++;
				}
			}
			return 0;
		}

		public int GetKillDragonNum(COM_PLAYERCAMP killerCamp)
		{
			int num = 0;
			for (int i = 0; i < this.m_deadRecordList.Count; i++)
			{
				if (killerCamp == this.m_deadRecordList[i].killerCamp && this.m_deadRecordList[i].actorType == ActorTypeDef.Actor_Type_Monster && this.m_deadRecordList[i].actorSubType == 2 && (this.m_deadRecordList[i].actorSubSoliderType == 9 || this.m_deadRecordList[i].actorSubSoliderType == 7))
				{
					num++;
				}
			}
			return num;
		}

		public int GetKillDragonNum()
		{
			int num = 0;
			for (int i = 0; i < this.m_deadRecordList.Count; i++)
			{
				if (this.m_deadRecordList[i].actorType == ActorTypeDef.Actor_Type_Monster && this.m_deadRecordList[i].actorSubType == 2 && (this.m_deadRecordList[i].actorSubSoliderType == 9 || this.m_deadRecordList[i].actorSubSoliderType == 7))
				{
					num++;
				}
			}
			return num;
		}

		public int GetAllMonsterDeadNum()
		{
			return this.m_deadMonsterNum;
		}

		public int GetHeroDeadAtTime(uint playerID, int deadTime)
		{
			int num = 0;
			List<DeadRecord> list = new List<DeadRecord>();
			for (int i = 0; i < this.m_deadRecordList.Count; i++)
			{
				DeadRecord deadRecord = this.m_deadRecordList[i];
				if (deadRecord.AttackPlayerID == playerID && deadRecord.actorType == ActorTypeDef.Actor_Type_Hero && deadRecord.deadTime < deadTime)
				{
					num++;
				}
			}
			return num;
		}

		public int GetMonsterDeadAtTime(uint playerID, int deadTime)
		{
			int num = 0;
			List<DeadRecord> list = new List<DeadRecord>();
			for (int i = 0; i < this.m_deadRecordList.Count; i++)
			{
				DeadRecord deadRecord = this.m_deadRecordList[i];
				if (deadRecord.AttackPlayerID == playerID && deadRecord.actorType == ActorTypeDef.Actor_Type_Monster && deadRecord.actorSubType == 2 && deadRecord.deadTime < deadTime)
				{
					num++;
				}
			}
			return num;
		}

		public int GetSoldierDeadAtTime(uint playerID, int deadTime)
		{
			int num = 0;
			List<DeadRecord> list = new List<DeadRecord>();
			for (int i = 0; i < this.m_deadRecordList.Count; i++)
			{
				DeadRecord deadRecord = this.m_deadRecordList[i];
				if (deadRecord.AttackPlayerID == playerID && deadRecord.actorType == ActorTypeDef.Actor_Type_Monster && deadRecord.actorSubType == 1 && deadRecord.deadTime < deadTime)
				{
					num++;
				}
			}
			return num;
		}

		public int GetKillDragonNumAtTime(uint playerID, int deadTime)
		{
			int num = 0;
			List<DeadRecord> list = new List<DeadRecord>();
			for (int i = 0; i < this.m_deadRecordList.Count; i++)
			{
				DeadRecord deadRecord = this.m_deadRecordList[i];
				if (deadRecord.AttackPlayerID == playerID && deadRecord.actorType == ActorTypeDef.Actor_Type_Monster && deadRecord.actorSubType == 2 && (deadRecord.actorSubSoliderType == 9 || deadRecord.actorSubSoliderType == 7) && deadRecord.deadTime < deadTime)
				{
					num++;
				}
			}
			return num;
		}

		public int GetKillSpecialMonsterNumAtTime(uint playerID, int deadTime, byte bySoldierType)
		{
			int num = 0;
			for (int i = 0; i < this.m_deadRecordList.Count; i++)
			{
				DeadRecord deadRecord = this.m_deadRecordList[i];
				if (deadRecord.AttackPlayerID == playerID && deadRecord.actorType == ActorTypeDef.Actor_Type_Monster && deadRecord.actorSubType == 2 && deadRecord.actorSubSoliderType == bySoldierType && deadRecord.deadTime < deadTime)
				{
					num++;
				}
			}
			return num;
		}

		public int GetDeadNumAtTime(COM_PLAYERCAMP camp, ActorTypeDef actorType, int subType, int deadTime)
		{
			int num = 0;
			for (int i = 0; i < this.m_deadRecordList.Count; i++)
			{
				DeadRecord deadRecord = this.m_deadRecordList[i];
				if (camp == deadRecord.camp && deadRecord.actorType == actorType && (int)deadRecord.actorSubType == subType && deadRecord.deadTime < deadTime)
				{
					num++;
				}
			}
			return num;
		}

		public int GetKillBlueBaNumAtTime(uint playerID, int deadTime)
		{
			return this.GetKillSpecialMonsterNumAtTime(playerID, deadTime, 10);
		}

		public int GetKillRedBaNumAtTime(uint playerID, int deadTime)
		{
			return this.GetKillSpecialMonsterNumAtTime(playerID, deadTime, 11);
		}

		public int GetBaronDeadCount()
		{
			int num = 0;
			for (int i = 0; i < this.m_deadRecordList.Count; i++)
			{
				DeadRecord deadRecord = this.m_deadRecordList[i];
				if (deadRecord.actorType == ActorTypeDef.Actor_Type_Monster && deadRecord.actorSubType == 2 && deadRecord.actorSubSoliderType == 8)
				{
					num++;
				}
			}
			return num;
		}

		public int GetBaronDeadCount(COM_PLAYERCAMP killerCamp)
		{
			int num = 0;
			for (int i = 0; i < this.m_deadRecordList.Count; i++)
			{
				DeadRecord deadRecord = this.m_deadRecordList[i];
				if (killerCamp == deadRecord.killerCamp && deadRecord.actorType == ActorTypeDef.Actor_Type_Monster && deadRecord.actorSubType == 2 && deadRecord.actorSubSoliderType == 8)
				{
					num++;
				}
			}
			return num;
		}

		public int GetBaronDeadTime(int index)
		{
			int num = 0;
			for (int i = 0; i < this.m_deadRecordList.Count; i++)
			{
				DeadRecord deadRecord = this.m_deadRecordList[i];
				if (deadRecord.actorType == ActorTypeDef.Actor_Type_Monster && deadRecord.actorSubType == 2 && deadRecord.actorSubSoliderType == 8)
				{
					if (num == index)
					{
						return this.m_deadRecordList[i].deadTime;
					}
					num++;
				}
			}
			return 0;
		}

		public byte GetDestroyTowerCount(COM_PLAYERCAMP killerCamp, int TowerType)
		{
			byte b = 0;
			for (int i = 0; i < this.m_deadRecordList.Count; i++)
			{
				DeadRecord deadRecord = this.m_deadRecordList[i];
				if (killerCamp == deadRecord.killerCamp && deadRecord.actorType == ActorTypeDef.Actor_Type_Organ && (int)deadRecord.actorSubType == TowerType)
				{
					b += 1;
				}
			}
			return b;
		}

		public int GetTotalNum(COM_PLAYERCAMP camp, ActorTypeDef actorType, byte actorSubType, byte actorSubSoliderType)
		{
			int num = 0;
			for (int i = 0; i < this.m_deadRecordList.Count; i++)
			{
				if (this.m_deadRecordList[i].camp == camp && this.m_deadRecordList[i].actorType == actorType && this.m_deadRecordList[i].actorSubType == actorSubType && this.m_deadRecordList[i].actorSubSoliderType == actorSubSoliderType)
				{
					num++;
				}
			}
			return num;
		}

		public DeadRecord GetRecordAtIndex(COM_PLAYERCAMP camp, ActorTypeDef actorType, byte actorSubType, byte actorSubSoliderType, int index)
		{
			int num = 0;
			for (int i = 0; i < this.m_deadRecordList.Count; i++)
			{
				if (this.m_deadRecordList[i].camp == camp && this.m_deadRecordList[i].actorType == actorType && this.m_deadRecordList[i].actorSubType == actorSubType && this.m_deadRecordList[i].actorSubSoliderType == actorSubSoliderType)
				{
					if (num == index)
					{
						return this.m_deadRecordList[i];
					}
					num++;
				}
			}
			return default(DeadRecord);
		}

		public int GetEnemyKillHeroMaxGap(COM_PLAYERCAMP camp)
		{
			int num = 0;
			int num2 = 0;
			this.enemyKillHeroMaxGap = 0;
			COM_PLAYERCAMP cOM_PLAYERCAMP = COM_PLAYERCAMP.COM_PLAYERCAMP_1;
			if (camp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
			{
				cOM_PLAYERCAMP = COM_PLAYERCAMP.COM_PLAYERCAMP_2;
			}
			for (int i = 0; i < this.m_deadRecordList.Count; i++)
			{
				if (this.m_deadRecordList[i].camp == camp && this.m_deadRecordList[i].actorType == ActorTypeDef.Actor_Type_Hero && this.m_deadRecordList[i].killerActorType == ActorTypeDef.Actor_Type_Hero)
				{
					num2++;
				}
				else if (this.m_deadRecordList[i].camp == cOM_PLAYERCAMP && this.m_deadRecordList[i].actorType == ActorTypeDef.Actor_Type_Hero && this.m_deadRecordList[i].killerActorType == ActorTypeDef.Actor_Type_Hero)
				{
					num++;
				}
				int num3 = num2 - num;
				if (num3 > this.enemyKillHeroMaxGap)
				{
					this.enemyKillHeroMaxGap = num3;
				}
			}
			return this.enemyKillHeroMaxGap;
		}

		public int GetActorAverageFightTime(COM_PLAYERCAMP camp, ActorTypeDef actorType, int cfgId)
		{
			int num = 0;
			int count = this.m_deadRecordList.Count;
			int num2 = 0;
			for (int i = 0; i < count; i++)
			{
				if (camp == this.m_deadRecordList[i].camp && actorType == this.m_deadRecordList[i].actorType && cfgId == this.m_deadRecordList[i].cfgId)
				{
					num++;
					num2 += this.m_deadRecordList[i].fightTime;
				}
			}
			return (num != 0) ? (num2 / num) : 0;
		}
	}
}
