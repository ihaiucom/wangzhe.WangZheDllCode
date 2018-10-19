using Assets.Scripts.Common;
using ResData;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameLogic
{
	public class AttackModeTargetSearcher : Singleton<AttackModeTargetSearcher>
	{
		private PoolObjHandle<ActorRoot> curActorPtr;

		private List<ActorRoot> heroList = new List<ActorRoot>();

		private List<ActorRoot> bossList = new List<ActorRoot>();

		private List<ActorRoot> monsterList = new List<ActorRoot>();

		private List<ActorRoot> monsterNotInBattleList = new List<ActorRoot>();

		private List<ActorRoot> organList = new List<ActorRoot>();

		private TargetPropertyLessEqualFilter heroFilter = default(TargetPropertyLessEqualFilter);

		private TargetPropertyLessEqualFilter bossFiler = default(TargetPropertyLessEqualFilter);

		private TargetPropertyLessEqualFilter monsterFiler = default(TargetPropertyLessEqualFilter);

		private TargetPropertyLessEqualFilter monsterNotInBattleFiler = default(TargetPropertyLessEqualFilter);

		private TargetPropertyLessEqualFilter organFiler = default(TargetPropertyLessEqualFilter);

		private SceneManagement.Process LowestHpHandler;

		private SceneManagement.Process NearestHandler;

		private int searchRadius;

		private uint searchTypeMask;

		private VInt3 searchPosition;

		private bool MonsterNotInBattle(ref PoolObjHandle<ActorRoot> monster)
		{
			if (monster)
			{
				MonsterWrapper monsterWrapper = monster.handle.ActorControl as MonsterWrapper;
				if (monsterWrapper != null)
				{
					ResMonsterCfgInfo cfgInfo = monsterWrapper.cfgInfo;
					if (cfgInfo != null && cfgInfo.bMonsterType == 2)
					{
						ObjAgent actorAgent = monster.handle.ActorAgent;
						ObjBehaviMode curBehavior = actorAgent.GetCurBehavior();
						if (curBehavior == ObjBehaviMode.State_Idle || curBehavior == ObjBehaviMode.State_Dead || curBehavior == ObjBehaviMode.State_Null)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		private void FilterLowestHpActor(ref PoolObjHandle<ActorRoot> _actorPtr)
		{
			if (!_actorPtr.handle.HorizonMarker.IsVisibleFor(this.curActorPtr.handle.TheActorMeta.ActorCamp) || !this.curActorPtr.handle.CanAttack(_actorPtr) || this.curActorPtr == _actorPtr || ((ulong)this.searchTypeMask & (ulong)(1L << (int)(this.curActorPtr.handle.TheActorMeta.ActorType & (ActorTypeDef)31))) > 0uL)
			{
				return;
			}
			if (TypeSearchCondition.Fit(_actorPtr, ActorTypeDef.Actor_Type_Hero))
			{
				if (DistanceSearchCondition.Fit(this.curActorPtr, _actorPtr, this.searchRadius))
				{
					this.heroFilter.Searcher(_actorPtr, RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP, new PropertyDelegate(TargetProperty.GetPropertyHpRate));
				}
			}
			else if (TypeSearchCondition.Fit(_actorPtr, ActorTypeDef.Actor_Type_Organ))
			{
				if (DistanceSearchCondition.Fit(this.curActorPtr, _actorPtr, this.searchRadius))
				{
					this.organFiler.Searcher(_actorPtr, RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP, new PropertyDelegate(TargetProperty.GetPropertyHpRate));
				}
			}
			else if (TypeSearchCondition.Fit(_actorPtr, ActorTypeDef.Actor_Type_Monster, false))
			{
				if (DistanceSearchCondition.Fit(this.curActorPtr, _actorPtr, this.searchRadius))
				{
					if (this.MonsterNotInBattle(ref _actorPtr))
					{
						this.monsterNotInBattleFiler.Searcher(_actorPtr, RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP, new PropertyDelegate(TargetProperty.GetPropertyHpRate));
					}
					else
					{
						this.monsterFiler.Searcher(_actorPtr, RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP, new PropertyDelegate(TargetProperty.GetPropertyHpRate));
					}
				}
			}
			else if (TypeSearchCondition.Fit(_actorPtr, ActorTypeDef.Actor_Type_Monster, true) && DistanceSearchCondition.Fit(this.curActorPtr, _actorPtr, this.searchRadius))
			{
				this.bossFiler.Searcher(_actorPtr, RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP, new PropertyDelegate(TargetProperty.GetPropertyHpRate));
			}
		}

		private void FilterNearestActor(ref PoolObjHandle<ActorRoot> _actorPtr)
		{
			if (!_actorPtr.handle.HorizonMarker.IsVisibleFor(this.curActorPtr.handle.TheActorMeta.ActorCamp) || !this.curActorPtr.handle.CanAttack(_actorPtr) || this.curActorPtr == _actorPtr || ((ulong)this.searchTypeMask & (ulong)(1L << (int)(this.curActorPtr.handle.TheActorMeta.ActorType & (ActorTypeDef)31))) > 0uL)
			{
				return;
			}
			if (TypeSearchCondition.Fit(_actorPtr, ActorTypeDef.Actor_Type_Hero))
			{
				if (DistanceSearchCondition.Fit(this.curActorPtr, _actorPtr, this.searchRadius))
				{
					this.heroFilter.Searcher(this.curActorPtr, _actorPtr, new DistanceDelegate(TargetDistance.GetDistance));
				}
			}
			else if (TypeSearchCondition.Fit(_actorPtr, ActorTypeDef.Actor_Type_Organ))
			{
				if (DistanceSearchCondition.Fit(this.curActorPtr, _actorPtr, this.searchRadius))
				{
					this.organFiler.Searcher(this.curActorPtr, _actorPtr, new DistanceDelegate(TargetDistance.GetDistance));
				}
			}
			else if (TypeSearchCondition.Fit(_actorPtr, ActorTypeDef.Actor_Type_Monster, false))
			{
				if (DistanceSearchCondition.Fit(this.curActorPtr, _actorPtr, this.searchRadius))
				{
					if (this.MonsterNotInBattle(ref _actorPtr))
					{
						this.monsterNotInBattleFiler.Searcher(this.curActorPtr, _actorPtr, new DistanceDelegate(TargetDistance.GetDistance));
					}
					else
					{
						this.monsterFiler.Searcher(this.curActorPtr, _actorPtr, new DistanceDelegate(TargetDistance.GetDistance));
					}
				}
			}
			else if (TypeSearchCondition.Fit(_actorPtr, ActorTypeDef.Actor_Type_Monster, true) && DistanceSearchCondition.Fit(this.curActorPtr, _actorPtr, this.searchRadius))
			{
				this.bossFiler.Searcher(this.curActorPtr, _actorPtr, new DistanceDelegate(TargetDistance.GetDistance));
			}
		}

		private void FilterNearestActorByPosition(ref PoolObjHandle<ActorRoot> _actorPtr)
		{
			if (!_actorPtr.handle.HorizonMarker.IsVisibleFor(this.curActorPtr.handle.TheActorMeta.ActorCamp) || !this.curActorPtr.handle.CanAttack(_actorPtr) || this.curActorPtr == _actorPtr)
			{
				return;
			}
			if (TypeSearchCondition.Fit(_actorPtr, ActorTypeDef.Actor_Type_Hero))
			{
				if (DistanceSearchCondition.Fit(this.searchPosition, _actorPtr, this.searchRadius))
				{
					this.heroFilter.Searcher(this.searchPosition, _actorPtr, new DistanceDelegate(TargetDistance.GetDistance));
				}
			}
			else if (TypeSearchCondition.Fit(_actorPtr, ActorTypeDef.Actor_Type_Organ))
			{
				if (DistanceSearchCondition.Fit(this.searchPosition, _actorPtr, this.searchRadius))
				{
					this.organFiler.Searcher(this.searchPosition, _actorPtr, new DistanceDelegate(TargetDistance.GetDistance));
				}
			}
			else if (TypeSearchCondition.Fit(_actorPtr, ActorTypeDef.Actor_Type_Monster, false))
			{
				if (DistanceSearchCondition.Fit(this.searchPosition, _actorPtr, this.searchRadius))
				{
					this.monsterFiler.Searcher(this.searchPosition, _actorPtr, new DistanceDelegate(TargetDistance.GetDistance));
				}
			}
			else if (TypeSearchCondition.Fit(_actorPtr, ActorTypeDef.Actor_Type_Monster, true) && DistanceSearchCondition.Fit(this.searchPosition, _actorPtr, this.searchRadius))
			{
				this.bossFiler.Searcher(this.searchPosition, _actorPtr, new DistanceDelegate(TargetDistance.GetDistance));
			}
		}

		private void Clear()
		{
			this.curActorPtr.Release();
			this.heroList.Clear();
			this.bossList.Clear();
			this.monsterList.Clear();
			this.monsterNotInBattleList.Clear();
			this.organList.Clear();
			this.heroFilter.Initial(this.heroList, 18446744073709551615uL);
			this.bossFiler.Initial(this.bossList, 18446744073709551615uL);
			this.monsterFiler.Initial(this.monsterList, 18446744073709551615uL);
			this.monsterNotInBattleFiler.Initial(this.monsterNotInBattleList, 18446744073709551615uL);
			this.organFiler.Initial(this.organList, 18446744073709551615uL);
		}

		private uint GetSearchPriorityTarget(bool bIncludeMonsterNotInBattle = true)
		{
			if (this.heroList.Count >= 1)
			{
				return this.heroList[0].ObjID;
			}
			if (this.bossList.Count >= 1)
			{
				return this.bossList[0].ObjID;
			}
			if (this.monsterList.Count >= 1)
			{
				return this.monsterList[0].ObjID;
			}
			if (this.organList.Count >= 1)
			{
				return this.organList[0].ObjID;
			}
			if (this.monsterNotInBattleList.Count >= 1 && bIncludeMonsterNotInBattle)
			{
				return this.monsterNotInBattleList[0].ObjID;
			}
			return 0u;
		}

		private uint GetSearchPriorityTargetInLastHitMode(bool bIncludeMonsterNotInBattle = true)
		{
			if (this.bossList.Count >= 1)
			{
				return this.bossList[0].ObjID;
			}
			if (this.monsterList.Count >= 1)
			{
				return this.monsterList[0].ObjID;
			}
			if (this.organList.Count >= 1)
			{
				return this.organList[0].ObjID;
			}
			if (this.heroList.Count >= 1)
			{
				return this.heroList[0].ObjID;
			}
			if (this.monsterNotInBattleList.Count >= 1 && bIncludeMonsterNotInBattle)
			{
				return this.monsterNotInBattleList[0].ObjID;
			}
			return 0u;
		}

		private uint GetSearchPriorityTargetInAttackOrganMode(bool bIncludeMonsterNotInBattle = true)
		{
			if (this.organList.Count >= 1)
			{
				return this.organList[0].ObjID;
			}
			if (this.bossList.Count >= 1)
			{
				return this.bossList[0].ObjID;
			}
			if (this.monsterList.Count >= 1)
			{
				return this.monsterList[0].ObjID;
			}
			if (this.heroList.Count >= 1)
			{
				return this.heroList[0].ObjID;
			}
			if (this.monsterNotInBattleList.Count >= 1 && bIncludeMonsterNotInBattle)
			{
				return this.monsterNotInBattleList[0].ObjID;
			}
			return 0u;
		}

		public uint SearchLowestHpTarget(ref PoolObjHandle<ActorRoot> _actorPtr, int _srchR, uint _typeMask, bool bIncludeMonsterNotInBattle = true, SearchTargetPriority priority = SearchTargetPriority.CommonAttack)
		{
			this.Clear();
			this.curActorPtr = _actorPtr;
			this.searchRadius = _srchR;
			this.searchTypeMask = _typeMask;
			this.LowestHpHandler = new SceneManagement.Process(this.FilterLowestHpActor);
			SceneManagement instance = Singleton<SceneManagement>.GetInstance();
			SceneManagement.Coordinate coord = default(SceneManagement.Coordinate);
			instance.GetCoord_Center(ref coord, _actorPtr.handle.location.xz, _srchR);
			instance.UpdateDirtyNodes();
			instance.ForeachActors(coord, this.LowestHpHandler);
			if (priority == SearchTargetPriority.CommonAttack)
			{
				return this.GetSearchPriorityTarget(bIncludeMonsterNotInBattle);
			}
			if (priority == SearchTargetPriority.LastHit)
			{
				return this.GetSearchPriorityTargetInLastHitMode(bIncludeMonsterNotInBattle);
			}
			return this.GetSearchPriorityTargetInAttackOrganMode(bIncludeMonsterNotInBattle);
		}

		public uint SearchNearestTarget(ref PoolObjHandle<ActorRoot> _actorPtr, int _srchR, uint _typeMask, bool bIncludeMonsterNotInBattle = true, SearchTargetPriority priority = SearchTargetPriority.CommonAttack)
		{
			this.Clear();
			this.curActorPtr = _actorPtr;
			this.searchRadius = _srchR;
			this.searchTypeMask = _typeMask;
			this.NearestHandler = new SceneManagement.Process(this.FilterNearestActor);
			SceneManagement instance = Singleton<SceneManagement>.GetInstance();
			SceneManagement.Coordinate coord = default(SceneManagement.Coordinate);
			instance.GetCoord_Center(ref coord, _actorPtr.handle.location.xz, _srchR);
			instance.UpdateDirtyNodes();
			instance.ForeachActors(coord, this.NearestHandler);
			if (priority == SearchTargetPriority.CommonAttack)
			{
				return this.GetSearchPriorityTarget(bIncludeMonsterNotInBattle);
			}
			if (priority == SearchTargetPriority.LastHit)
			{
				return this.GetSearchPriorityTargetInLastHitMode(bIncludeMonsterNotInBattle);
			}
			return this.GetSearchPriorityTargetInAttackOrganMode(bIncludeMonsterNotInBattle);
		}

		public uint SearchNearestTarget(ref PoolObjHandle<ActorRoot> _actorPtr, VInt3 _position, int _srchR)
		{
			this.Clear();
			this.curActorPtr = _actorPtr;
			this.searchRadius = _srchR;
			this.searchPosition = _position;
			this.NearestHandler = new SceneManagement.Process(this.FilterNearestActorByPosition);
			SceneManagement instance = Singleton<SceneManagement>.GetInstance();
			SceneManagement.Coordinate coord = default(SceneManagement.Coordinate);
			instance.GetCoord_Center(ref coord, _position.xz, _srchR);
			instance.UpdateDirtyNodes();
			instance.ForeachActors(coord, this.NearestHandler);
			return this.GetSearchPriorityTarget(true);
		}
	}
}
