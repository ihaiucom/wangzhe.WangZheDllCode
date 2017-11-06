using Assets.Scripts.Common;
using CSProtocol;
using ResData;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameLogic
{
	public class TargetSearcher : Singleton<TargetSearcher>
	{
		private List<PoolObjHandle<ActorRoot>> collidedActors = new List<PoolObjHandle<ActorRoot>>();

		private PoolObjHandle<ActorRoot> _coordActor;

		private VCollisionShape _coordShape;

		private bool _coordFilterEnemy;

		private bool _coordFilterAlly;

		private bool _coordCheckSight;

		private AreaEventTrigger _coordTriggerRef;

		private SceneManagement.Process _coordHandler;

		private List<ActorRoot> tempActors = new List<ActorRoot>(15);

		public override void Init()
		{
			base.Init();
			this._coordHandler = new SceneManagement.Process(this.FilterCoordActor);
		}

		public ActorRoot GetNearestFriendly(ActorRoot InActor, int srchR, uint filter)
		{
			ActorRoot result = null;
			ulong num = (ulong)((long)srchR * (long)srchR);
			List<PoolObjHandle<ActorRoot>> campActors = Singleton<GameObjMgr>.GetInstance().GetCampActors(InActor.TheActorMeta.ActorCamp);
			for (int i = 0; i < campActors.get_Count(); i++)
			{
				ActorRoot handle = campActors.get_Item(i).handle;
				if (((ulong)filter & 1uL << (int)(handle.TheActorMeta.ActorType & (ActorTypeDef)31)) <= 0uL && !handle.ActorControl.IsDeadState)
				{
					ulong sqrMagnitudeLong2D = (ulong)(handle.location - InActor.location).sqrMagnitudeLong2D;
					if (sqrMagnitudeLong2D < num)
					{
						result = handle;
						num = sqrMagnitudeLong2D;
					}
				}
			}
			return result;
		}

		public ActorRoot GetNearestEnemy(ActorRoot InActor, int srchR, uint filter, bool bWithMonsterNotInBattle = true)
		{
			ActorRoot result = null;
			ulong num = (ulong)((long)srchR * (long)srchR);
			for (int i = 0; i < 3; i++)
			{
				if (i != (int)InActor.TheActorMeta.ActorCamp)
				{
					List<PoolObjHandle<ActorRoot>> campActors = Singleton<GameObjMgr>.GetInstance().GetCampActors((COM_PLAYERCAMP)i);
					int count = campActors.get_Count();
					for (int j = 0; j < count; j++)
					{
						ActorRoot handle = campActors.get_Item(j).handle;
						ActorTypeDef actorType = handle.TheActorMeta.ActorType;
						if (actorType == ActorTypeDef.Actor_Type_Monster)
						{
							MonsterWrapper monsterWrapper = handle.ActorControl as MonsterWrapper;
							if (monsterWrapper != null && monsterWrapper.hostActor)
							{
								actorType = monsterWrapper.hostActor.handle.TheActorMeta.ActorType;
							}
						}
						if (((ulong)filter & 1uL << (int)(actorType & (ActorTypeDef)31)) <= 0uL && handle.HorizonMarker.IsVisibleFor(InActor.TheActorMeta.ActorCamp))
						{
							if (!bWithMonsterNotInBattle)
							{
								MonsterWrapper monsterWrapper2 = handle.ActorControl as MonsterWrapper;
								if (monsterWrapper2 != null)
								{
									ResMonsterCfgInfo cfgInfo = monsterWrapper2.cfgInfo;
									if (cfgInfo != null && cfgInfo.bMonsterType == 2)
									{
										ObjAgent actorAgent = handle.ActorAgent;
										ObjBehaviMode curBehavior = actorAgent.GetCurBehavior();
										if (curBehavior == ObjBehaviMode.State_Idle || curBehavior == ObjBehaviMode.State_Dead || curBehavior == ObjBehaviMode.State_Null)
										{
											goto IL_178;
										}
									}
								}
							}
							ulong sqrMagnitudeLong2D = (ulong)(handle.location - InActor.location).sqrMagnitudeLong2D;
							if (sqrMagnitudeLong2D < num && InActor.CanAttack(handle))
							{
								result = handle;
								num = sqrMagnitudeLong2D;
							}
						}
						IL_178:;
					}
				}
			}
			return result;
		}

		public ActorRoot GetNearRandomEnemy(ActorRoot InActor, int srchR)
		{
			ActorRoot result = null;
			ulong num = (ulong)((long)srchR * (long)srchR);
			for (int i = 0; i < 3; i++)
			{
				if (i != (int)InActor.TheActorMeta.ActorCamp)
				{
					List<PoolObjHandle<ActorRoot>> campActors = Singleton<GameObjMgr>.GetInstance().GetCampActors((COM_PLAYERCAMP)i);
					int count = campActors.get_Count();
					for (int j = 0; j < count; j++)
					{
						ActorRoot handle = campActors.get_Item(j).handle;
						if (handle.HorizonMarker.IsVisibleFor(InActor.TheActorMeta.ActorCamp))
						{
							ulong sqrMagnitudeLong2D = (ulong)(handle.location - InActor.location).sqrMagnitudeLong2D;
							if (sqrMagnitudeLong2D < num && InActor.CanAttack(handle))
							{
								return handle;
							}
						}
					}
				}
			}
			return result;
		}

		public ActorRoot GetNearestEnemyIgnoreVisible(ActorRoot InActor, int srchR, uint filter)
		{
			ActorRoot result = null;
			ulong num = (ulong)((long)srchR * (long)srchR);
			for (int i = 0; i < 3; i++)
			{
				if (i != (int)InActor.TheActorMeta.ActorCamp)
				{
					List<PoolObjHandle<ActorRoot>> campActors = Singleton<GameObjMgr>.GetInstance().GetCampActors((COM_PLAYERCAMP)i);
					int count = campActors.get_Count();
					for (int j = 0; j < count; j++)
					{
						ActorRoot handle = campActors.get_Item(j).handle;
						if (((ulong)filter & 1uL << (int)(handle.TheActorMeta.ActorType & (ActorTypeDef)31)) <= 0uL)
						{
							ulong sqrMagnitudeLong2D = (ulong)(handle.location - InActor.location).sqrMagnitudeLong2D;
							if (sqrMagnitudeLong2D < num && InActor.CanAttack(handle))
							{
								result = handle;
								num = sqrMagnitudeLong2D;
							}
						}
					}
				}
			}
			return result;
		}

		public ActorRoot GetLowestHpTarget(ActorRoot InActor, int srchR, TargetPriority priotity, uint filter, bool bEnemy = true, bool bWithMonsterNotInBattle = true)
		{
			List<ActorRoot> list = new List<ActorRoot>();
			List<ActorRoot> list2 = new List<ActorRoot>();
			List<ActorRoot> list3 = new List<ActorRoot>();
			TargetPropertyLessEqualFilter targetPropertyLessEqualFilter = new TargetPropertyLessEqualFilter(list, 18446744073709551615uL);
			TargetPropertyLessEqualFilter targetPropertyLessEqualFilter2 = new TargetPropertyLessEqualFilter(list2, 18446744073709551615uL);
			TargetDistanceNearFilter targetDistanceNearFilter = new TargetDistanceNearFilter(18446744073709551615uL);
			if (bEnemy)
			{
				for (int i = 0; i < 3; i++)
				{
					if (i != (int)InActor.TheActorMeta.ActorCamp)
					{
						List<PoolObjHandle<ActorRoot>> campActors = Singleton<GameObjMgr>.GetInstance().GetCampActors((COM_PLAYERCAMP)i);
						int count = campActors.get_Count();
						for (int j = 0; j < count; j++)
						{
							ActorRoot handle = campActors.get_Item(j).handle;
							if (((ulong)filter & 1uL << (int)(handle.TheActorMeta.ActorType & (ActorTypeDef)31)) <= 0uL && handle.HorizonMarker.IsVisibleFor(InActor.TheActorMeta.ActorCamp) && InActor.CanAttack(handle))
							{
								if (TypeSearchCondition.Fit(handle, ActorTypeDef.Actor_Type_Hero))
								{
									if (DistanceSearchCondition.Fit(handle, InActor, srchR))
									{
										targetPropertyLessEqualFilter.Searcher(handle, RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP, new PropertyDelegate(TargetProperty.GetPropertyHpRate));
									}
								}
								else if (TypeSearchCondition.Fit(handle, ActorTypeDef.Actor_Type_Organ))
								{
									if (DistanceSearchCondition.Fit(handle, InActor, srchR))
									{
										list3.Add(handle);
									}
								}
								else if (TypeSearchCondition.Fit(handle, ActorTypeDef.Actor_Type_Monster) && TypeSearchCondition.FitWithJungleMonsterNotInBattle(handle, bWithMonsterNotInBattle) && DistanceSearchCondition.Fit(handle, InActor, srchR))
								{
									targetPropertyLessEqualFilter2.Searcher(handle, RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP, new PropertyDelegate(TargetProperty.GetPropertyHpRate));
								}
							}
						}
					}
				}
			}
			else
			{
				List<PoolObjHandle<ActorRoot>> campActors2 = Singleton<GameObjMgr>.GetInstance().GetCampActors(InActor.TheActorMeta.ActorCamp);
				int count2 = campActors2.get_Count();
				for (int k = 0; k < count2; k++)
				{
					ActorRoot handle2 = campActors2.get_Item(k).handle;
					if (((ulong)filter & 1uL << (int)(handle2.TheActorMeta.ActorType & (ActorTypeDef)31)) <= 0uL && !handle2.ActorControl.IsDeadState && handle2.HorizonMarker.IsVisibleFor(InActor.TheActorMeta.ActorCamp))
					{
						if (TypeSearchCondition.Fit(handle2, ActorTypeDef.Actor_Type_Hero))
						{
							if (DistanceSearchCondition.Fit(handle2, InActor, srchR))
							{
								targetPropertyLessEqualFilter.Searcher(handle2, RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP, new PropertyDelegate(TargetProperty.GetPropertyHpRate));
							}
						}
						else if (TypeSearchCondition.Fit(handle2, ActorTypeDef.Actor_Type_Organ))
						{
							if (DistanceSearchCondition.Fit(handle2, InActor, srchR))
							{
								list3.Add(handle2);
							}
						}
						else if (TypeSearchCondition.Fit(handle2, ActorTypeDef.Actor_Type_Monster) && DistanceSearchCondition.Fit(handle2, InActor, srchR))
						{
							targetPropertyLessEqualFilter2.Searcher(handle2, RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP, new PropertyDelegate(TargetProperty.GetPropertyHpRate));
						}
					}
				}
			}
			int iTargetCount = list.get_Count() + list3.get_Count() + list2.get_Count();
			int count3 = list.get_Count();
			if (count3 > 0)
			{
				ActorRoot actorRoot;
				if (count3 == 1)
				{
					actorRoot = list.get_Item(0);
				}
				else
				{
					actorRoot = targetDistanceNearFilter.Searcher(list.GetEnumerator(), InActor);
				}
				PoolObjHandle<ActorRoot> src = default(PoolObjHandle<ActorRoot>);
				if (actorRoot != null)
				{
					src = actorRoot.SelfPtr;
				}
				SkillChooseTargetEventParam skillChooseTargetEventParam = new SkillChooseTargetEventParam(src, InActor.SelfPtr, iTargetCount);
				Singleton<GameEventSys>.instance.SendEvent<SkillChooseTargetEventParam>(GameEventDef.Event_ActorBeChosenAsTarget, ref skillChooseTargetEventParam);
				return actorRoot;
			}
			count3 = list3.get_Count();
			if (count3 > 0)
			{
				if (count3 == 1)
				{
					return list3.get_Item(0);
				}
				return targetDistanceNearFilter.Searcher(list3.GetEnumerator(), InActor);
			}
			else
			{
				count3 = list2.get_Count();
				if (count3 <= 0)
				{
					return null;
				}
				if (count3 == 1)
				{
					return list2.get_Item(0);
				}
				return targetDistanceNearFilter.Searcher(list2.GetEnumerator(), InActor);
			}
		}

		public ActorRoot GetNearestEnemy(ActorRoot InActor, int srchR, TargetPriority priotity, uint filter, bool bWithMonsterNotInBattle = true)
		{
			ActorRoot actorRoot = null;
			ActorRoot actorRoot2 = null;
			ulong num = (ulong)((long)srchR * (long)srchR);
			ulong num2 = num;
			for (int i = 0; i < 3; i++)
			{
				if (i != (int)InActor.TheActorMeta.ActorCamp)
				{
					List<PoolObjHandle<ActorRoot>> campActors = Singleton<GameObjMgr>.GetInstance().GetCampActors((COM_PLAYERCAMP)i);
					int count = campActors.get_Count();
					for (int j = 0; j < count; j++)
					{
						ActorRoot handle = campActors.get_Item(j).handle;
						if (((ulong)filter & 1uL << (int)(handle.TheActorMeta.ActorType & (ActorTypeDef)31)) <= 0uL && handle.HorizonMarker.IsVisibleFor(InActor.TheActorMeta.ActorCamp))
						{
							if (!bWithMonsterNotInBattle)
							{
								MonsterWrapper monsterWrapper = handle.ActorControl as MonsterWrapper;
								if (monsterWrapper != null)
								{
									ResMonsterCfgInfo cfgInfo = monsterWrapper.cfgInfo;
									if (cfgInfo != null && cfgInfo.bMonsterType == 2)
									{
										ObjAgent actorAgent = handle.ActorAgent;
										ObjBehaviMode curBehavior = actorAgent.GetCurBehavior();
										if (curBehavior == ObjBehaviMode.State_Idle || curBehavior == ObjBehaviMode.State_Dead || curBehavior == ObjBehaviMode.State_Null)
										{
											goto IL_1BB;
										}
									}
								}
							}
							if ((priotity == TargetPriority.TargetPriority_Hero && handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero) || (priotity == TargetPriority.TargetPriority_Monster && handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster) || (priotity == TargetPriority.TargetPriority_Organ && handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ))
							{
								ulong sqrMagnitudeLong2D = (ulong)(handle.location - InActor.location).sqrMagnitudeLong2D;
								if (sqrMagnitudeLong2D < num && InActor.CanAttack(handle))
								{
									actorRoot = handle;
									num = sqrMagnitudeLong2D;
								}
							}
							else
							{
								ulong sqrMagnitudeLong2D2 = (ulong)(handle.location - InActor.location).sqrMagnitudeLong2D;
								if (sqrMagnitudeLong2D2 < num2 && InActor.CanAttack(handle))
								{
									actorRoot2 = handle;
									num2 = sqrMagnitudeLong2D2;
								}
							}
						}
						IL_1BB:;
					}
				}
			}
			return (actorRoot != null) ? actorRoot : actorRoot2;
		}

		public ActorRoot GetNearestEnemyDogfaceFirst(ActorRoot InActor, int srchR)
		{
			ActorRoot actorRoot = null;
			ActorRoot actorRoot2 = null;
			ulong num = (ulong)((long)srchR * (long)srchR);
			ulong num2 = (ulong)((long)srchR * (long)srchR);
			for (int i = 0; i < 3; i++)
			{
				if (i != (int)InActor.TheActorMeta.ActorCamp)
				{
					List<PoolObjHandle<ActorRoot>> campActors = Singleton<GameObjMgr>.GetInstance().GetCampActors((COM_PLAYERCAMP)i);
					int count = campActors.get_Count();
					for (int j = 0; j < count; j++)
					{
						ActorRoot handle = campActors.get_Item(j).handle;
						if (handle.HorizonMarker.IsVisibleFor(InActor.TheActorMeta.ActorCamp))
						{
							if (handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
							{
								ulong sqrMagnitudeLong2D = (ulong)(handle.location - InActor.location).sqrMagnitudeLong2D;
								if (sqrMagnitudeLong2D < num && InActor.CanAttack(handle))
								{
									actorRoot = handle;
									num = sqrMagnitudeLong2D;
								}
							}
							else
							{
								ulong sqrMagnitudeLong2D2 = (ulong)(handle.location - InActor.location).sqrMagnitudeLong2D;
								if (sqrMagnitudeLong2D2 < num2 && InActor.CanAttack(handle))
								{
									actorRoot2 = handle;
									num2 = sqrMagnitudeLong2D2;
								}
							}
						}
					}
				}
			}
			return (actorRoot2 != null) ? actorRoot2 : actorRoot;
		}

		public ActorRoot GetNearestEnemyDogfaceFirstAndDogfaceHasPriority(ActorRoot InActor, int srchR)
		{
			ActorRoot actorRoot = null;
			ActorRoot actorRoot2 = null;
			ActorRoot actorRoot3 = null;
			ActorRoot actorRoot4 = null;
			ActorRoot actorRoot5 = null;
			ActorRoot actorRoot6 = null;
			ulong num = (ulong)((long)srchR * (long)srchR);
			ulong num2 = num;
			ulong num3 = num;
			ulong num4 = num;
			ulong num5 = num;
			ulong num6 = num;
			for (int i = 0; i < 3; i++)
			{
				if (i != (int)InActor.TheActorMeta.ActorCamp)
				{
					List<PoolObjHandle<ActorRoot>> campActors = Singleton<GameObjMgr>.GetInstance().GetCampActors((COM_PLAYERCAMP)i);
					int count = campActors.get_Count();
					for (int j = 0; j < count; j++)
					{
						ActorRoot handle = campActors.get_Item(j).handle;
						if (handle.HorizonMarker.IsVisibleFor(InActor.TheActorMeta.ActorCamp))
						{
							if (handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
							{
								ulong sqrMagnitudeLong2D = (ulong)(handle.location - InActor.location).sqrMagnitudeLong2D;
								if (sqrMagnitudeLong2D < num5 && InActor.CanAttack(handle))
								{
									actorRoot5 = handle;
									num5 = sqrMagnitudeLong2D;
								}
							}
							else if (handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Call)
							{
								ulong sqrMagnitudeLong2D2 = (ulong)(handle.location - InActor.location).sqrMagnitudeLong2D;
								if (sqrMagnitudeLong2D2 < num6 && InActor.CanAttack(handle))
								{
									actorRoot6 = handle;
									num6 = sqrMagnitudeLong2D2;
								}
							}
							else
							{
								MonsterWrapper monsterWrapper = handle.AsMonster();
								if (monsterWrapper != null)
								{
									if (monsterWrapper.cfgInfo.bSoldierType == 3)
									{
										ulong sqrMagnitudeLong2D3 = (ulong)(handle.location - InActor.location).sqrMagnitudeLong2D;
										if (sqrMagnitudeLong2D3 < num && InActor.CanAttack(handle))
										{
											actorRoot = handle;
											num = sqrMagnitudeLong2D3;
										}
									}
									else if (monsterWrapper.cfgInfo.bSoldierType == 1)
									{
										ulong sqrMagnitudeLong2D4 = (ulong)(handle.location - InActor.location).sqrMagnitudeLong2D;
										if (sqrMagnitudeLong2D4 < num2 && InActor.CanAttack(handle))
										{
											actorRoot2 = handle;
											num2 = sqrMagnitudeLong2D4;
										}
									}
									else if (monsterWrapper.cfgInfo.bSoldierType == 2)
									{
										ulong sqrMagnitudeLong2D5 = (ulong)(handle.location - InActor.location).sqrMagnitudeLong2D;
										if (sqrMagnitudeLong2D5 < num3 && InActor.CanAttack(handle))
										{
											actorRoot3 = handle;
											num3 = sqrMagnitudeLong2D5;
										}
									}
									else if (monsterWrapper.cfgInfo.bMonsterType != 2)
									{
										ulong sqrMagnitudeLong2D6 = (ulong)(handle.location - InActor.location).sqrMagnitudeLong2D;
										if (sqrMagnitudeLong2D6 < num4 && InActor.CanAttack(handle))
										{
											actorRoot4 = handle;
											num4 = sqrMagnitudeLong2D6;
										}
									}
								}
							}
						}
					}
				}
			}
			ActorRoot result = null;
			if (actorRoot != null)
			{
				result = actorRoot;
			}
			else if (actorRoot2 != null)
			{
				result = actorRoot2;
			}
			else if (actorRoot3 != null)
			{
				result = actorRoot3;
			}
			else if (actorRoot4 != null)
			{
				result = actorRoot4;
			}
			else if (actorRoot5 != null)
			{
				result = actorRoot5;
			}
			else if (actorRoot6 != null)
			{
				result = actorRoot6;
			}
			return result;
		}

		public ActorRoot GetNearestEnemyWithoutNotInBattleJungleMonster(ActorRoot InActor, int srchR)
		{
			ActorRoot result = null;
			ulong num = (ulong)((long)srchR * (long)srchR);
			for (int i = 0; i < 3; i++)
			{
				if (i != (int)InActor.TheActorMeta.ActorCamp)
				{
					List<PoolObjHandle<ActorRoot>> campActors = Singleton<GameObjMgr>.GetInstance().GetCampActors((COM_PLAYERCAMP)i);
					int count = campActors.get_Count();
					for (int j = 0; j < count; j++)
					{
						ActorRoot handle = campActors.get_Item(j).handle;
						if (handle.HorizonMarker.IsVisibleFor(InActor.TheActorMeta.ActorCamp))
						{
							MonsterWrapper monsterWrapper = handle.AsMonster();
							if (monsterWrapper != null)
							{
								ResMonsterCfgInfo cfgInfo = monsterWrapper.cfgInfo;
								if (cfgInfo != null && cfgInfo.bMonsterType == 2)
								{
									ObjAgent actorAgent = handle.ActorAgent;
									ObjBehaviMode curBehavior = actorAgent.GetCurBehavior();
									if (curBehavior == ObjBehaviMode.State_Idle || curBehavior == ObjBehaviMode.State_Dead || curBehavior == ObjBehaviMode.State_Null)
									{
										goto IL_FF;
									}
								}
							}
							ulong sqrMagnitudeLong2D = (ulong)(handle.location - InActor.location).sqrMagnitudeLong2D;
							if (sqrMagnitudeLong2D < num && InActor.CanAttack(handle))
							{
								result = handle;
								num = sqrMagnitudeLong2D;
							}
						}
						IL_FF:;
					}
				}
			}
			return result;
		}

		public ActorRoot GetNearestEnemyWithTwoPriorityWithoutJungleMonster(ActorRoot InActor, int srchR, TargetPriority priotity_1, TargetPriority priotity_2)
		{
			ActorRoot actorRoot = null;
			ActorRoot actorRoot2 = null;
			ActorRoot actorRoot3 = null;
			ulong num = (ulong)((long)srchR * (long)srchR);
			ulong num2 = num;
			ulong num3 = num;
			for (int i = 0; i < 3; i++)
			{
				if (i != (int)InActor.TheActorMeta.ActorCamp)
				{
					List<PoolObjHandle<ActorRoot>> campActors = Singleton<GameObjMgr>.GetInstance().GetCampActors((COM_PLAYERCAMP)i);
					int count = campActors.get_Count();
					for (int j = 0; j < count; j++)
					{
						ActorRoot handle = campActors.get_Item(j).handle;
						if (handle.HorizonMarker.IsVisibleFor(InActor.TheActorMeta.ActorCamp))
						{
							MonsterWrapper monsterWrapper = handle.AsMonster();
							if (monsterWrapper != null)
							{
								ResMonsterCfgInfo cfgInfo = monsterWrapper.cfgInfo;
								if (cfgInfo != null && cfgInfo.bMonsterType == 2)
								{
									goto IL_1F8;
								}
							}
							if ((priotity_1 == TargetPriority.TargetPriority_Hero && handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero) || (priotity_1 == TargetPriority.TargetPriority_Monster && handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster) || (priotity_1 == TargetPriority.TargetPriority_Organ && handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ))
							{
								ulong sqrMagnitudeLong2D = (ulong)(handle.location - InActor.location).sqrMagnitudeLong2D;
								if (sqrMagnitudeLong2D < num && InActor.CanAttack(handle))
								{
									actorRoot = handle;
									num = sqrMagnitudeLong2D;
								}
							}
							else if ((priotity_2 == TargetPriority.TargetPriority_Hero && handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero) || (priotity_2 == TargetPriority.TargetPriority_Monster && handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster) || (priotity_2 == TargetPriority.TargetPriority_Organ && handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ))
							{
								ulong sqrMagnitudeLong2D2 = (ulong)(handle.location - InActor.location).sqrMagnitudeLong2D;
								if (sqrMagnitudeLong2D2 < num2 && InActor.CanAttack(handle))
								{
									actorRoot2 = handle;
									num2 = sqrMagnitudeLong2D2;
								}
							}
							else
							{
								ulong sqrMagnitudeLong2D3 = (ulong)(handle.location - InActor.location).sqrMagnitudeLong2D;
								if (sqrMagnitudeLong2D3 < num3 && InActor.CanAttack(handle))
								{
									actorRoot3 = handle;
									num3 = sqrMagnitudeLong2D3;
								}
							}
						}
						IL_1F8:;
					}
				}
			}
			return (actorRoot != null) ? actorRoot : ((actorRoot2 != null) ? actorRoot2 : actorRoot3);
		}

		public ActorRoot GetNearestEnemyWithPriorityWithoutJungleMonster(ActorRoot InActor, int srchR, TargetPriority priotity)
		{
			ActorRoot actorRoot = null;
			ActorRoot actorRoot2 = null;
			ulong num = (ulong)((long)srchR * (long)srchR);
			ulong num2 = num;
			for (int i = 0; i < 3; i++)
			{
				if (i != (int)InActor.TheActorMeta.ActorCamp)
				{
					List<PoolObjHandle<ActorRoot>> campActors = Singleton<GameObjMgr>.GetInstance().GetCampActors((COM_PLAYERCAMP)i);
					int count = campActors.get_Count();
					for (int j = 0; j < count; j++)
					{
						ActorRoot handle = campActors.get_Item(j).handle;
						if (handle.HorizonMarker.IsVisibleFor(InActor.TheActorMeta.ActorCamp))
						{
							MonsterWrapper monsterWrapper = handle.AsMonster();
							if (monsterWrapper != null)
							{
								ResMonsterCfgInfo cfgInfo = monsterWrapper.cfgInfo;
								if (cfgInfo != null && cfgInfo.bMonsterType == 2)
								{
									goto IL_17E;
								}
							}
							if ((priotity == TargetPriority.TargetPriority_Hero && handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero) || (priotity == TargetPriority.TargetPriority_Monster && handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster) || (priotity == TargetPriority.TargetPriority_Organ && handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ) || (priotity == TargetPriority.TargetPriority_CallActor && handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Call))
							{
								ulong sqrMagnitudeLong2D = (ulong)(handle.location - InActor.location).sqrMagnitudeLong2D;
								if (sqrMagnitudeLong2D < num && InActor.CanAttack(handle))
								{
									actorRoot = handle;
									num = sqrMagnitudeLong2D;
								}
							}
							else
							{
								ulong sqrMagnitudeLong2D2 = (ulong)(handle.location - InActor.location).sqrMagnitudeLong2D;
								if (sqrMagnitudeLong2D2 < num2 && InActor.CanAttack(handle))
								{
									actorRoot2 = handle;
									num2 = sqrMagnitudeLong2D2;
								}
							}
						}
						IL_17E:;
					}
				}
			}
			return (actorRoot != null) ? actorRoot : actorRoot2;
		}

		public ActorRoot GetNearestEnemyWithPriorityWithoutNotInBattleJungleMonster(ActorRoot InActor, int srchR, TargetPriority priotity)
		{
			ActorRoot actorRoot = null;
			ActorRoot actorRoot2 = null;
			ulong num = (ulong)((long)srchR * (long)srchR);
			for (int i = 0; i < 3; i++)
			{
				if (i != (int)InActor.TheActorMeta.ActorCamp)
				{
					List<PoolObjHandle<ActorRoot>> campActors = Singleton<GameObjMgr>.GetInstance().GetCampActors((COM_PLAYERCAMP)i);
					int count = campActors.get_Count();
					for (int j = 0; j < count; j++)
					{
						ActorRoot handle = campActors.get_Item(j).handle;
						if (handle.HorizonMarker.IsVisibleFor(InActor.TheActorMeta.ActorCamp))
						{
							MonsterWrapper monsterWrapper = handle.AsMonster();
							if (monsterWrapper != null)
							{
								ResMonsterCfgInfo cfgInfo = monsterWrapper.cfgInfo;
								if (cfgInfo != null && cfgInfo.bMonsterType == 2)
								{
									ObjAgent actorAgent = handle.ActorAgent;
									ObjBehaviMode curBehavior = actorAgent.GetCurBehavior();
									if (curBehavior == ObjBehaviMode.State_Idle || curBehavior == ObjBehaviMode.State_Dead || curBehavior == ObjBehaviMode.State_Null)
									{
										goto IL_193;
									}
								}
							}
							if ((priotity == TargetPriority.TargetPriority_Hero && handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero) || (priotity == TargetPriority.TargetPriority_Monster && handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster) || (priotity == TargetPriority.TargetPriority_Organ && handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ))
							{
								ulong sqrMagnitudeLong2D = (ulong)(handle.location - InActor.location).sqrMagnitudeLong2D;
								if (sqrMagnitudeLong2D < num && InActor.CanAttack(handle))
								{
									actorRoot = handle;
									num = sqrMagnitudeLong2D;
									if (priotity == TargetPriority.TargetPriority_Organ)
									{
										break;
									}
								}
							}
							else
							{
								ulong sqrMagnitudeLong2D2 = (ulong)(handle.location - InActor.location).sqrMagnitudeLong2D;
								if (sqrMagnitudeLong2D2 < num && InActor.CanAttack(handle))
								{
									actorRoot2 = handle;
								}
							}
						}
						IL_193:;
					}
				}
			}
			return (actorRoot != null) ? actorRoot : actorRoot2;
		}

		public ActorRoot GetNearestEnemyWithPriorityWithoutNotInBattleJungleMonsterWithoutActor(ActorRoot InActor, int srchR, TargetPriority priotity, uint withOutActor)
		{
			ActorRoot actorRoot = null;
			ActorRoot actorRoot2 = null;
			ulong num = (ulong)((long)srchR * (long)srchR);
			for (int i = 0; i < 3; i++)
			{
				if (i != (int)InActor.TheActorMeta.ActorCamp)
				{
					List<PoolObjHandle<ActorRoot>> campActors = Singleton<GameObjMgr>.GetInstance().GetCampActors((COM_PLAYERCAMP)i);
					int count = campActors.get_Count();
					for (int j = 0; j < count; j++)
					{
						ActorRoot handle = campActors.get_Item(j).handle;
						if (handle != null && handle.ObjID != withOutActor && handle.HorizonMarker.IsVisibleFor(InActor.TheActorMeta.ActorCamp))
						{
							MonsterWrapper monsterWrapper = handle.AsMonster();
							if (monsterWrapper != null)
							{
								ResMonsterCfgInfo cfgInfo = monsterWrapper.cfgInfo;
								if (cfgInfo != null && cfgInfo.bMonsterType == 2)
								{
									ObjAgent actorAgent = handle.ActorAgent;
									ObjBehaviMode curBehavior = actorAgent.GetCurBehavior();
									if (curBehavior == ObjBehaviMode.State_Idle || curBehavior == ObjBehaviMode.State_Dead || curBehavior == ObjBehaviMode.State_Null)
									{
										goto IL_1A8;
									}
								}
							}
							if ((priotity == TargetPriority.TargetPriority_Hero && handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero) || (priotity == TargetPriority.TargetPriority_Monster && handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster) || (priotity == TargetPriority.TargetPriority_Organ && handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ))
							{
								ulong sqrMagnitudeLong2D = (ulong)(handle.location - InActor.location).sqrMagnitudeLong2D;
								if (sqrMagnitudeLong2D < num && InActor.CanAttack(handle))
								{
									actorRoot = handle;
									num = sqrMagnitudeLong2D;
									if (priotity == TargetPriority.TargetPriority_Organ)
									{
										break;
									}
								}
							}
							else
							{
								ulong sqrMagnitudeLong2D2 = (ulong)(handle.location - InActor.location).sqrMagnitudeLong2D;
								if (sqrMagnitudeLong2D2 < num && InActor.CanAttack(handle))
								{
									actorRoot2 = handle;
								}
							}
						}
						IL_1A8:;
					}
				}
			}
			return (actorRoot != null) ? actorRoot : actorRoot2;
		}

		public ActorRoot GetNearestEnemyWithoutNotInBattleJungleMonsterWithoutActor(ActorRoot InActor, int srchR, uint withOutActor)
		{
			if (InActor == null)
			{
				return null;
			}
			ActorRoot result = null;
			ulong num = (ulong)((long)srchR * (long)srchR);
			for (int i = 0; i < 3; i++)
			{
				if (i != (int)InActor.TheActorMeta.ActorCamp)
				{
					List<PoolObjHandle<ActorRoot>> campActors = Singleton<GameObjMgr>.GetInstance().GetCampActors((COM_PLAYERCAMP)i);
					int count = campActors.get_Count();
					for (int j = 0; j < count; j++)
					{
						ActorRoot handle = campActors.get_Item(j).handle;
						if (handle != null && handle.ObjID != withOutActor && handle.HorizonMarker.IsVisibleFor(InActor.TheActorMeta.ActorCamp))
						{
							MonsterWrapper monsterWrapper = handle.AsMonster();
							if (monsterWrapper != null)
							{
								ResMonsterCfgInfo cfgInfo = monsterWrapper.cfgInfo;
								if (cfgInfo != null && cfgInfo.bMonsterType == 2)
								{
									ObjAgent actorAgent = handle.ActorAgent;
									ObjBehaviMode curBehavior = actorAgent.GetCurBehavior();
									if (curBehavior == ObjBehaviMode.State_Idle || curBehavior == ObjBehaviMode.State_Dead || curBehavior == ObjBehaviMode.State_Null)
									{
										goto IL_11B;
									}
								}
							}
							ulong sqrMagnitudeLong2D = (ulong)(handle.location - InActor.location).sqrMagnitudeLong2D;
							if (sqrMagnitudeLong2D < num && InActor.CanAttack(handle))
							{
								result = handle;
								num = sqrMagnitudeLong2D;
							}
						}
						IL_11B:;
					}
				}
			}
			return result;
		}

		public ActorRoot GetNearestEnemyWithoutJungleMonsterWithoutActor(ActorRoot InActor, int srchR, uint withOutActor)
		{
			if (InActor == null)
			{
				return null;
			}
			ActorRoot result = null;
			ulong num = (ulong)((long)srchR * (long)srchR);
			for (int i = 0; i < 3; i++)
			{
				if (i != (int)InActor.TheActorMeta.ActorCamp)
				{
					List<PoolObjHandle<ActorRoot>> campActors = Singleton<GameObjMgr>.GetInstance().GetCampActors((COM_PLAYERCAMP)i);
					int count = campActors.get_Count();
					for (int j = 0; j < count; j++)
					{
						ActorRoot handle = campActors.get_Item(j).handle;
						if (handle != null && handle.ObjID != withOutActor && handle.HorizonMarker.IsVisibleFor(InActor.TheActorMeta.ActorCamp))
						{
							MonsterWrapper monsterWrapper = handle.AsMonster();
							if (monsterWrapper == null || monsterWrapper.cfgInfo.bMonsterType != 2)
							{
								ulong sqrMagnitudeLong2D = (ulong)(handle.location - InActor.location).sqrMagnitudeLong2D;
								if (sqrMagnitudeLong2D < num && InActor.CanAttack(handle))
								{
									result = handle;
									num = sqrMagnitudeLong2D;
								}
							}
						}
					}
				}
			}
			return result;
		}

		public ActorRoot GetNearestEnemyWithoutJungleMonsterAndCallActorWithoutActor(ActorRoot InActor, int srchR, uint withOutActor)
		{
			if (InActor == null)
			{
				return null;
			}
			ActorRoot result = null;
			ulong num = (ulong)((long)srchR * (long)srchR);
			for (int i = 0; i < 3; i++)
			{
				if (i != (int)InActor.TheActorMeta.ActorCamp)
				{
					List<PoolObjHandle<ActorRoot>> campActors = Singleton<GameObjMgr>.GetInstance().GetCampActors((COM_PLAYERCAMP)i);
					int count = campActors.get_Count();
					for (int j = 0; j < count; j++)
					{
						ActorRoot handle = campActors.get_Item(j).handle;
						if (handle != null && handle.ObjID != withOutActor && handle.HorizonMarker.IsVisibleFor(InActor.TheActorMeta.ActorCamp))
						{
							MonsterWrapper monsterWrapper = handle.AsMonster();
							if (monsterWrapper == null || monsterWrapper.cfgInfo.bMonsterType != 2)
							{
								CallActorWrapper callActorWrapper = handle.AsCallActor();
								if (callActorWrapper == null || callActorWrapper.IsTrueType)
								{
									ulong sqrMagnitudeLong2D = (ulong)(handle.location - InActor.location).sqrMagnitudeLong2D;
									if (sqrMagnitudeLong2D < num && InActor.CanAttack(handle))
									{
										result = handle;
										num = sqrMagnitudeLong2D;
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		public ActorRoot GetLowHpTeamMember(ActorRoot InActor, int srchR, int HPRate, uint filter)
		{
			ActorRoot result = null;
			ulong num = (ulong)((long)srchR * (long)srchR);
			List<PoolObjHandle<ActorRoot>> campActors = Singleton<GameObjMgr>.GetInstance().GetCampActors(InActor.TheActorMeta.ActorCamp);
			int count = campActors.get_Count();
			for (int i = 0; i < count; i++)
			{
				ActorRoot handle = campActors.get_Item(i).handle;
				if (((ulong)filter & 1uL << (int)(handle.TheActorMeta.ActorType & (ActorTypeDef)31)) <= 0uL && !handle.ActorControl.IsDeadState)
				{
					ulong sqrMagnitudeLong2D = (ulong)(handle.location - InActor.location).sqrMagnitudeLong2D;
					if (sqrMagnitudeLong2D < num && handle.ValueComponent.actorHp * 10000 < handle.ValueComponent.actorHpTotal * HPRate)
					{
						return handle;
					}
				}
			}
			return result;
		}

		public int GetActorsInCircle(VInt3 center, int radius, PoolObjHandle<ActorRoot>[] outResults, AreaEventTrigger InTriggerRef)
		{
			int num = 0;
			ulong num2 = (ulong)((long)radius * (long)radius);
			List<PoolObjHandle<ActorRoot>> gameActors = Singleton<GameObjMgr>.GetInstance().GameActors;
			int count = gameActors.get_Count();
			for (int i = 0; i < count; i++)
			{
				if (num >= outResults.Length)
				{
					break;
				}
				PoolObjHandle<ActorRoot> poolObjHandle = gameActors.get_Item(i);
				if (!(null != InTriggerRef) || InTriggerRef.ActorFilter(ref poolObjHandle))
				{
					ulong sqrMagnitudeLong2D = (ulong)(poolObjHandle.handle.location - center).sqrMagnitudeLong2D;
					if (sqrMagnitudeLong2D < num2)
					{
						outResults[num++] = poolObjHandle;
					}
				}
			}
			return num;
		}

		public List<PoolObjHandle<ActorRoot>> GetCollidedActors()
		{
			return this.collidedActors;
		}

		public void EndCollidedActorList()
		{
			this.collidedActors.Clear();
		}

		private void FilterCoordActor(ref PoolObjHandle<ActorRoot> actorPtr)
		{
			ActorRoot handle = actorPtr.handle;
			if (handle.ActorControl.IsDeadState || handle.shape == null || actorPtr == this._coordActor)
			{
				return;
			}
			if (handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_EYE)
			{
				return;
			}
			if ((this._coordActor && ((this._coordFilterAlly && handle.IsSelfCamp(this._coordActor.handle)) || ((this._coordFilterEnemy || (handle.ObjLinker.Invincible && handle.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_EYE)) && handle.IsEnemyCamp(this._coordActor.handle)))) || (handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ && !handle.AttackOrderReady) || (this._coordCheckSight && !handle.HorizonMarker.IsVisibleFor(this._coordActor.handle.TheActorMeta.ActorCamp)))
			{
				return;
			}
			if (null != this._coordTriggerRef && !this._coordTriggerRef.ActorFilter(ref actorPtr))
			{
				return;
			}
			if (handle.shape.Intersects(this._coordShape))
			{
				this.collidedActors.Add(actorPtr);
			}
		}

		public void BeginCollidedActorList(PoolObjHandle<ActorRoot> InActor, VCollisionShape collisionShape, bool bFilterEnemy, bool bFilterAlly, AreaEventTrigger InTriggerRef, bool bCheckSight)
		{
			this.collidedActors.Clear();
			if (collisionShape == null)
			{
				return;
			}
			this._coordActor = InActor;
			this._coordShape = collisionShape;
			this._coordFilterEnemy = bFilterEnemy;
			this._coordFilterAlly = bFilterAlly;
			this._coordTriggerRef = InTriggerRef;
			this._coordCheckSight = bCheckSight;
			SceneManagement instance = Singleton<SceneManagement>.GetInstance();
			SceneManagement.Coordinate coord = default(SceneManagement.Coordinate);
			instance.GetCoord(ref coord, collisionShape);
			instance.UpdateDirtyNodes();
			instance.ForeachActors(coord, this._coordHandler);
			this._coordShape = null;
			this._coordTriggerRef = null;
		}

		public List<PoolObjHandle<ActorRoot>> GetActorsInPolygon(GeoPolygon collisionPolygon, AreaEventTrigger InTriggerRef)
		{
			if (collisionPolygon == null)
			{
				return null;
			}
			List<PoolObjHandle<ActorRoot>> list = null;
			List<PoolObjHandle<ActorRoot>> gameActors = Singleton<GameObjMgr>.GetInstance().GameActors;
			int count = gameActors.get_Count();
			for (int i = 0; i < count; i++)
			{
				PoolObjHandle<ActorRoot> poolObjHandle = gameActors.get_Item(i);
				if (poolObjHandle && (!(null != InTriggerRef) || InTriggerRef.ActorFilter(ref poolObjHandle)) && collisionPolygon.IsPointIn(poolObjHandle.handle.location))
				{
					if (list == null)
					{
						list = new List<PoolObjHandle<ActorRoot>>();
					}
					list.Add(poolObjHandle);
				}
			}
			return list;
		}

		public ActorRoot GetAnyEnemyInCircle(ActorRoot InActor, VInt3 Pos, int srchR)
		{
			long num = (long)srchR * (long)srchR;
			for (int i = 0; i < 3; i++)
			{
				if (i != (int)InActor.TheActorMeta.ActorCamp)
				{
					List<PoolObjHandle<ActorRoot>> campActors = Singleton<GameObjMgr>.GetInstance().GetCampActors((COM_PLAYERCAMP)i);
					int count = campActors.get_Count();
					for (int j = 0; j < count; j++)
					{
						ActorRoot handle = campActors.get_Item(j).handle;
						if (InActor.CanAttack(handle))
						{
							long sqrMagnitudeLong2D = (handle.location - Pos).sqrMagnitudeLong2D;
							if (sqrMagnitudeLong2D < num)
							{
								return handle;
							}
						}
					}
				}
			}
			return null;
		}

		public List<ActorRoot> GetOurCampActors(ActorRoot InActor, int srchR)
		{
			this.tempActors.Clear();
			long num = (long)srchR * (long)srchR;
			List<PoolObjHandle<ActorRoot>> campActors = Singleton<GameObjMgr>.GetInstance().GetCampActors(InActor.TheActorMeta.ActorCamp);
			int count = campActors.get_Count();
			for (int i = 0; i < count; i++)
			{
				ActorRoot handle = campActors.get_Item(i).handle;
				if (handle.ObjID != InActor.ObjID && !handle.ActorControl.IsDeadState)
				{
					long sqrMagnitudeLong2D = (handle.location - InActor.location).sqrMagnitudeLong2D;
					if (sqrMagnitudeLong2D < num)
					{
						this.tempActors.Add(handle);
					}
				}
			}
			return this.tempActors;
		}

		public int GetEnemyCountInRange(ActorRoot InActor, int srchR)
		{
			long num = (long)srchR * (long)srchR;
			int num2 = 0;
			List<PoolObjHandle<ActorRoot>> campActors;
			if (InActor.TheActorMeta.ActorCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
			{
				campActors = Singleton<GameObjMgr>.GetInstance().GetCampActors(COM_PLAYERCAMP.COM_PLAYERCAMP_2);
			}
			else
			{
				if (InActor.TheActorMeta.ActorCamp != COM_PLAYERCAMP.COM_PLAYERCAMP_2)
				{
					return 0;
				}
				campActors = Singleton<GameObjMgr>.GetInstance().GetCampActors(COM_PLAYERCAMP.COM_PLAYERCAMP_1);
			}
			int count = campActors.get_Count();
			for (int i = 0; i < count; i++)
			{
				ActorRoot handle = campActors.get_Item(i).handle;
				if (!handle.ActorControl.IsDeadState)
				{
					long sqrMagnitudeLong2D = (handle.location - InActor.location).sqrMagnitudeLong2D;
					if (sqrMagnitudeLong2D < num)
					{
						num2++;
					}
				}
			}
			return num2;
		}

		public ActorRoot GetNearestSelfCampHero(ActorRoot InActor, int range)
		{
			long num = (long)range * (long)range;
			ActorRoot result = null;
			List<PoolObjHandle<ActorRoot>> campActors = Singleton<GameObjMgr>.GetInstance().GetCampActors(InActor.TheActorMeta.ActorCamp);
			int count = campActors.get_Count();
			for (int i = 0; i < count; i++)
			{
				ActorRoot handle = campActors.get_Item(i).handle;
				if (handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero && !handle.ActorControl.IsDeadState && handle.ObjID != InActor.ObjID)
				{
					long sqrMagnitudeLong2D = (handle.location - InActor.location).sqrMagnitudeLong2D;
					if (sqrMagnitudeLong2D < num)
					{
						result = handle;
						num = sqrMagnitudeLong2D;
					}
				}
			}
			return result;
		}

		public int GetEnemyHeroCountInRange(ActorRoot InActor, int srchR)
		{
			long num = (long)srchR * (long)srchR;
			int num2 = 0;
			List<PoolObjHandle<ActorRoot>> campActors;
			if (InActor.TheActorMeta.ActorCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
			{
				campActors = Singleton<GameObjMgr>.GetInstance().GetCampActors(COM_PLAYERCAMP.COM_PLAYERCAMP_2);
			}
			else
			{
				if (InActor.TheActorMeta.ActorCamp != COM_PLAYERCAMP.COM_PLAYERCAMP_2)
				{
					return 0;
				}
				campActors = Singleton<GameObjMgr>.GetInstance().GetCampActors(COM_PLAYERCAMP.COM_PLAYERCAMP_1);
			}
			int count = campActors.get_Count();
			for (int i = 0; i < count; i++)
			{
				ActorRoot handle = campActors.get_Item(i).handle;
				if (handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero && !handle.ActorControl.IsDeadState)
				{
					long sqrMagnitudeLong2D = (handle.location - InActor.location).sqrMagnitudeLong2D;
					if (sqrMagnitudeLong2D < num)
					{
						num2++;
					}
				}
			}
			return num2;
		}

		public void NotifySelfCampToAttack(PoolObjHandle<ActorRoot> InActor, int srchR, PoolObjHandle<ActorRoot> target)
		{
			long num = (long)srchR;
			num *= num;
			List<PoolObjHandle<ActorRoot>> campActors = Singleton<GameObjMgr>.GetInstance().GetCampActors(InActor.handle.TheActorMeta.ActorCamp);
			int count = campActors.get_Count();
			for (int i = 0; i < count; i++)
			{
				ActorRoot handle = campActors.get_Item(i).handle;
				if (handle.ObjID != InActor.handle.ObjID)
				{
					long sqrMagnitudeLong2D = (handle.location - InActor.handle.location).sqrMagnitudeLong2D;
					if (sqrMagnitudeLong2D < num)
					{
						handle.ActorControl.SetHelpToAttackTarget(InActor, target);
					}
				}
			}
		}

		public void CommanderNotifyToAttack(COM_PLAYERCAMP camp, int srchR, PoolObjHandle<ActorRoot> target, bool ignoreSearchRange)
		{
			long num = (long)srchR;
			num *= num;
			List<PoolObjHandle<ActorRoot>> campActors = Singleton<GameObjMgr>.GetInstance().GetCampActors(camp);
			int count = campActors.get_Count();
			for (int i = 0; i < count; i++)
			{
				ActorRoot handle = campActors.get_Item(i).handle;
				if (handle.ObjID != target.handle.ObjID && handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
				{
					long sqrMagnitudeLong2D = (handle.location - target.handle.location).sqrMagnitudeLong2D;
					if (sqrMagnitudeLong2D < num)
					{
						handle.ActorControl.SetToAttackTarget(target, ignoreSearchRange);
					}
				}
			}
		}

		public PoolObjHandle<ActorRoot> GetCommanderHeroTarget(COM_PLAYERCAMP camp, int range)
		{
			List<PoolObjHandle<ActorRoot>> heroActors = Singleton<GameObjMgr>.GetInstance().HeroActors;
			int count = heroActors.get_Count();
			for (int i = 0; i < count; i++)
			{
				ActorRoot handle = heroActors.get_Item(i).handle;
				if (handle.TheActorMeta.ActorCamp != camp && handle.ValueComponent.actorHp * 100 <= handle.ValueComponent.actorHpTotal * 70 && !handle.ActorControl.IsDeadState)
				{
					int heroNumInRange = this.GetHeroNumInRange(handle, range, handle.TheActorMeta.ActorCamp);
					int heroNumInRange2 = this.GetHeroNumInRange(handle, range, camp);
					if (heroNumInRange <= 2 && heroNumInRange2 > heroNumInRange)
					{
						return heroActors.get_Item(i);
					}
				}
			}
			return default(PoolObjHandle<ActorRoot>);
		}

		public PoolObjHandle<ActorRoot> GetCommanderTowerTarget(COM_PLAYERCAMP camp, int friendRange, int enemyRange)
		{
			List<PoolObjHandle<ActorRoot>> towerActors = Singleton<GameObjMgr>.GetInstance().TowerActors;
			int count = towerActors.get_Count();
			OrganPos organPos = OrganPos.Base;
			for (int i = 0; i < count; i++)
			{
				ActorRoot handle = towerActors.get_Item(i).handle;
				if (handle.TheActorMeta.ActorCamp != camp && !handle.ActorControl.IsDeadState && handle.ObjLinker.theOrganPos < organPos)
				{
					organPos = handle.ObjLinker.theOrganPos;
				}
			}
			for (int j = 0; j < count; j++)
			{
				ActorRoot handle2 = towerActors.get_Item(j).handle;
				if (handle2.TheActorMeta.ActorCamp != camp && !handle2.ActorControl.IsDeadState && handle2.ObjLinker.theOrganPos == organPos)
				{
					int heroNumInRange = this.GetHeroNumInRange(handle2, enemyRange, handle2.TheActorMeta.ActorCamp);
					int heroNumInRange2 = this.GetHeroNumInRange(handle2, friendRange, camp);
					if (heroNumInRange2 >= heroNumInRange)
					{
						return towerActors.get_Item(j);
					}
				}
			}
			return default(PoolObjHandle<ActorRoot>);
		}

		public int GetHeroNumInRange(ActorRoot InActor, int range, Relationship rship)
		{
			long num = (long)range * (long)range;
			int num2 = 0;
			List<PoolObjHandle<ActorRoot>> heroActors = Singleton<GameObjMgr>.GetInstance().HeroActors;
			int count = heroActors.get_Count();
			for (int i = 0; i < count; i++)
			{
				ActorRoot handle = heroActors.get_Item(i).handle;
				if ((rship != Relationship.Enemy || InActor.TheActorMeta.ActorCamp != handle.TheActorMeta.ActorCamp) && (rship != Relationship.FriendAndMe || InActor.TheActorMeta.ActorCamp == handle.TheActorMeta.ActorCamp) && !handle.ActorControl.IsDeadState)
				{
					long sqrMagnitudeLong2D = (handle.location - InActor.location).sqrMagnitudeLong2D;
					if (sqrMagnitudeLong2D < num)
					{
						num2++;
					}
				}
			}
			return num2;
		}

		public int GetHeroNumInRange(ActorRoot InActor, int range, COM_PLAYERCAMP camp)
		{
			long num = (long)range * (long)range;
			int num2 = 0;
			List<PoolObjHandle<ActorRoot>> heroActors = Singleton<GameObjMgr>.GetInstance().HeroActors;
			int count = heroActors.get_Count();
			for (int i = 0; i < count; i++)
			{
				ActorRoot handle = heroActors.get_Item(i).handle;
				if (handle.TheActorMeta.ActorCamp == camp && !handle.ActorControl.IsDeadState)
				{
					long sqrMagnitudeLong2D = (handle.location - InActor.location).sqrMagnitudeLong2D;
					if (sqrMagnitudeLong2D < num)
					{
						num2++;
					}
				}
			}
			return num2;
		}

		public int GetAliveHeroNum(COM_PLAYERCAMP camp)
		{
			int num = 0;
			List<PoolObjHandle<ActorRoot>> heroActors = Singleton<GameObjMgr>.GetInstance().HeroActors;
			int count = heroActors.get_Count();
			for (int i = 0; i < count; i++)
			{
				ActorRoot handle = heroActors.get_Item(i).handle;
				if (handle.TheActorMeta.ActorCamp == camp && !handle.ActorControl.IsDeadState)
				{
					num++;
				}
			}
			return num;
		}

		public bool HasCantAttackEnemyBuilding(ActorRoot InActor, int srchR)
		{
			bool result = false;
			ulong num = (ulong)((long)srchR * (long)srchR);
			List<PoolObjHandle<ActorRoot>> organActors = Singleton<GameObjMgr>.GetInstance().OrganActors;
			int count = organActors.get_Count();
			for (int i = 0; i < count; i++)
			{
				ActorRoot handle = organActors.get_Item(i).handle;
				if (handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ)
				{
					ulong sqrMagnitudeLong2D = (ulong)(handle.location - InActor.location).sqrMagnitudeLong2D;
					if (sqrMagnitudeLong2D < num && !handle.ActorControl.IsDeadState && !InActor.IsSelfCamp(handle) && !handle.AttackOrderReady)
					{
						return true;
					}
				}
			}
			return result;
		}

		public uint GetEnemyBuilding(ActorRoot InActor, int srchR)
		{
			uint result = 0u;
			ulong num = (ulong)((long)srchR * (long)srchR);
			List<PoolObjHandle<ActorRoot>> organActors = Singleton<GameObjMgr>.GetInstance().OrganActors;
			int count = organActors.get_Count();
			for (int i = 0; i < count; i++)
			{
				ActorRoot handle = organActors.get_Item(i).handle;
				if (handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ)
				{
					ulong sqrMagnitudeLong2D = (ulong)(handle.location - InActor.location).sqrMagnitudeLong2D;
					if (sqrMagnitudeLong2D < num && !handle.ActorControl.IsDeadState && !InActor.IsSelfCamp(handle))
					{
						result = handle.ObjID;
						break;
					}
				}
			}
			return result;
		}

		public bool HasSelfSoldierCountRoundEnemyBuilding(ActorRoot InActor, uint buildId)
		{
			bool result = false;
			List<PoolObjHandle<ActorRoot>> organActors = Singleton<GameObjMgr>.GetInstance().OrganActors;
			int count = organActors.get_Count();
			for (int i = 0; i < count; i++)
			{
				ActorRoot handle = organActors.get_Item(i).handle;
				if (handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ || handle.ObjID == buildId)
				{
					long num = (long)handle.ActorControl.AttackRange * (long)handle.ActorControl.AttackRange;
					List<PoolObjHandle<ActorRoot>> soldierActors = Singleton<GameObjMgr>.GetInstance().SoldierActors;
					int count2 = soldierActors.get_Count();
					for (int j = 0; j < count2; j++)
					{
						ActorRoot handle2 = soldierActors.get_Item(j).handle;
						if (handle2.IsSelfCamp(InActor))
						{
							long sqrMagnitudeLong2D = (handle2.location - handle.location).sqrMagnitudeLong2D;
							if (sqrMagnitudeLong2D < num)
							{
								result = true;
								break;
							}
						}
					}
					break;
				}
			}
			return result;
		}

		public bool HasEnemyBuildingAndEnemyBuildingWillAttackSelf(ActorRoot InActor, int srchR)
		{
			bool result = false;
			ulong num = (ulong)((long)srchR * (long)srchR);
			List<PoolObjHandle<ActorRoot>> organActors = Singleton<GameObjMgr>.GetInstance().OrganActors;
			int count = organActors.get_Count();
			for (int i = 0; i < count; i++)
			{
				ActorRoot handle = organActors.get_Item(i).handle;
				if (handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ)
				{
					ulong sqrMagnitudeLong2D = (ulong)(handle.location - InActor.location).sqrMagnitudeLong2D;
					if (sqrMagnitudeLong2D < num && !handle.ActorControl.IsDeadState && !InActor.IsSelfCamp(handle))
					{
						result = true;
						long num2 = (long)handle.ActorControl.AttackRange * (long)handle.ActorControl.AttackRange;
						List<PoolObjHandle<ActorRoot>> soldierActors = Singleton<GameObjMgr>.GetInstance().SoldierActors;
						int count2 = soldierActors.get_Count();
						for (int j = 0; j < count2; j++)
						{
							ActorRoot handle2 = soldierActors.get_Item(j).handle;
							if (handle2.IsSelfCamp(InActor))
							{
								long sqrMagnitudeLong2D2 = (handle2.location - handle.location).sqrMagnitudeLong2D;
								if (sqrMagnitudeLong2D2 < num2)
								{
									return false;
								}
							}
						}
					}
				}
			}
			return result;
		}
	}
}
