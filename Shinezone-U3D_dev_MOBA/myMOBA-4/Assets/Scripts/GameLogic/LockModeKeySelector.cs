using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.GameSystem;
using ResData;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameLogic
{
	internal class LockModeKeySelector : Singleton<LockModeKeySelector>
	{
		private AttackTargetType m_TargetType;

		private PoolObjHandle<ActorRoot> m_CurSelectedActor;

		public override void Init()
		{
			this.m_TargetType = AttackTargetType.ATTACK_TARGET_HERO;
		}

		public void SelectAttackTarget(AttackTargetType _targetType)
		{
		}

		private void SortActorListByTag(PoolObjHandle<ActorRoot> InActor, ref List<ActorRoot> actorList, SelectEnemyType type)
		{
			if (actorList.Count <= 1)
			{
				return;
			}
			int count = actorList.Count;
			ulong[] array = new ulong[count];
			if (type == SelectEnemyType.SelectLowHp)
			{
				array[0] = TargetProperty.GetPropertyHpRate(actorList[0], RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP);
			}
			else
			{
				array[0] = (ulong)(InActor.handle.location - actorList[0].location).sqrMagnitudeLong2D;
			}
			for (int i = 1; i < count; i++)
			{
				ActorRoot actorRoot = actorList[i];
				ulong num;
				if (type == SelectEnemyType.SelectLowHp)
				{
					num = TargetProperty.GetPropertyHpRate(actorRoot, RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP);
				}
				else
				{
					num = (ulong)(InActor.handle.location - actorList[0].location).sqrMagnitudeLong2D;
				}
				array[i] = num;
				int j;
				for (j = i; j >= 1; j--)
				{
					if (num >= array[j - 1])
					{
						break;
					}
					array[j] = array[j - 1];
					actorList[j] = actorList[j - 1];
				}
				array[j] = num;
				actorList[j] = actorRoot;
			}
		}

		private ActorRoot GetLowerValueTargetByTag(PoolObjHandle<ActorRoot> InActor, List<ActorRoot> actorList, SelectEnemyType type)
		{
			int count = actorList.Count;
			if (count <= 0)
			{
				return null;
			}
			if (count <= 1)
			{
				return actorList[0];
			}
			int i;
			for (i = 0; i < count; i++)
			{
				if (actorList[i].SelfPtr == InActor)
				{
					break;
				}
			}
			ActorRoot result;
			if (i < count - 1)
			{
				result = actorList[i + 1];
			}
			else
			{
				result = actorList[0];
			}
			return result;
		}

		private uint GetLowerValueTargetIdByTag(PoolObjHandle<ActorRoot> InActor, List<ActorRoot> actorList, SelectEnemyType type)
		{
			ActorRoot actorRoot = null;
			int count = actorList.Count;
			if (count > 0)
			{
				if (count == 1)
				{
					actorRoot = actorList[0];
				}
				else if (this.m_CurSelectedActor)
				{
					actorRoot = this.GetLowerValueTargetByTag(this.m_CurSelectedActor, actorList, type);
				}
				else
				{
					actorRoot = actorList[0];
				}
			}
			if (actorRoot != null)
			{
				this.m_CurSelectedActor = actorRoot.SelfPtr;
				return actorRoot.ObjID;
			}
			return 0u;
		}

		private uint GetSelectTargetByTag(AttackTargetType targetType, SelectEnemyType selectType)
		{
			List<ActorRoot> list = new List<ActorRoot>();
			List<ActorRoot> list2 = new List<ActorRoot>();
			List<ActorRoot> list3 = new List<ActorRoot>();
			List<ActorRoot> list4 = new List<ActorRoot>();
			Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
			if (hostPlayer == null || !hostPlayer.Captain)
			{
				return 0u;
			}
			PoolObjHandle<ActorRoot> captain = hostPlayer.Captain;
			List<PoolObjHandle<ActorRoot>> gameActors = Singleton<GameObjMgr>.GetInstance().GameActors;
			int count = gameActors.Count;
			for (int i = 0; i < count; i++)
			{
				ActorRoot handle = gameActors[i].handle;
				if (handle.HorizonMarker.IsVisibleFor(captain.handle.TheActorMeta.ActorCamp))
				{
					if (captain.handle.CanAttack(handle))
					{
						if (targetType == AttackTargetType.ATTACK_TARGET_HERO)
						{
							if (TypeSearchCondition.Fit(handle, ActorTypeDef.Actor_Type_Hero) && DistanceSearchCondition.Fit(handle, captain, captain.handle.ActorControl.SearchRange))
							{
								list.Add(handle);
							}
						}
						else if (TypeSearchCondition.Fit(handle, ActorTypeDef.Actor_Type_Organ))
						{
							if (DistanceSearchCondition.Fit(handle, captain, captain.handle.ActorControl.SearchRange))
							{
								list4.Add(handle);
							}
						}
						else if (TypeSearchCondition.Fit(handle, ActorTypeDef.Actor_Type_Monster) && DistanceSearchCondition.Fit(handle, captain, captain.handle.ActorControl.SearchRange))
						{
							MonsterWrapper monsterWrapper = handle.AsMonster();
							if (monsterWrapper.cfgInfo.bSoldierType == 7 || monsterWrapper.cfgInfo.bSoldierType == 8 || monsterWrapper.cfgInfo.bSoldierType == 9)
							{
								list3.Add(handle);
							}
							else
							{
								list2.Add(handle);
							}
						}
					}
				}
			}
			uint lowerValueTargetIdByTag;
			if (targetType == AttackTargetType.ATTACK_TARGET_HERO)
			{
				this.SortActorListByTag(captain, ref list, selectType);
				lowerValueTargetIdByTag = this.GetLowerValueTargetIdByTag(captain, list, selectType);
			}
			else
			{
				this.SortActorListByTag(captain, ref list3, selectType);
				this.SortActorListByTag(captain, ref list2, selectType);
				this.SortActorListByTag(captain, ref list4, selectType);
				List<ActorRoot> list5 = new List<ActorRoot>();
				for (int j = 0; j < list3.Count; j++)
				{
					list5.Add(list3[j]);
				}
				for (int j = 0; j < list2.Count; j++)
				{
					list5.Add(list2[j]);
				}
				for (int j = 0; j < list4.Count; j++)
				{
					list5.Add(list4[j]);
				}
				lowerValueTargetIdByTag = this.GetLowerValueTargetIdByTag(captain, list5, selectType);
			}
			return lowerValueTargetIdByTag;
		}

		public void OnHandleClickSelectTargetBtn(AttackTargetType _targetType)
		{
			OperateMode operateMode = OperateMode.DefaultMode;
			Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
			if (hostPlayer != null)
			{
				operateMode = hostPlayer.GetOperateMode();
			}
			if (operateMode == OperateMode.DefaultMode)
			{
				return;
			}
			if (_targetType != this.m_TargetType)
			{
				this.m_CurSelectedActor.Release();
			}
			SelectEnemyType selectType = SelectEnemyType.SelectLowHp;
			if (hostPlayer != null)
			{
				selectType = hostPlayer.AttackTargetMode;
			}
			uint selectTargetByTag = this.GetSelectTargetByTag(_targetType, selectType);
			Singleton<NetLockAttackTarget>.GetInstance().SendLockAttackTarget(selectTargetByTag);
			this.m_TargetType = _targetType;
		}
	}
}
