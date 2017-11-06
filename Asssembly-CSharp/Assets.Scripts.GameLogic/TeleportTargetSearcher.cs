using Assets.Scripts.Common;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameLogic
{
	public class TeleportTargetSearcher : Singleton<TeleportTargetSearcher>
	{
		private PoolObjHandle<ActorRoot> curActorPtr;

		private List<ActorRoot> heroList = new List<ActorRoot>();

		private List<ActorRoot> organList = new List<ActorRoot>();

		private List<ActorRoot> eyeList = new List<ActorRoot>();

		private List<ActorRoot> soldierList = new List<ActorRoot>();

		private TargetPropertyLessEqualFilter heroFilter = default(TargetPropertyLessEqualFilter);

		private TargetPropertyLessEqualFilter organFiler = default(TargetPropertyLessEqualFilter);

		private TargetPropertyLessEqualFilter eyeFiler = default(TargetPropertyLessEqualFilter);

		private TargetPropertyLessEqualFilter soldierFilter = default(TargetPropertyLessEqualFilter);

		private SceneManagement.Process NearestHandler;

		private int searchRadius;

		private uint searchTypeMask;

		private VInt3 searchPosition;

		private void Clear()
		{
			this.curActorPtr.Release();
			this.heroList.Clear();
			this.organList.Clear();
			this.eyeList.Clear();
			this.soldierList.Clear();
			this.heroFilter.Initial(this.heroList, 18446744073709551615uL);
			this.organFiler.Initial(this.organList, 18446744073709551615uL);
			this.eyeFiler.Initial(this.eyeList, 18446744073709551615uL);
			this.soldierFilter.Initial(this.soldierList, 18446744073709551615uL);
		}

		private uint GetSearchPriorityTarget()
		{
			if (this.heroList.get_Count() >= 1)
			{
				return this.heroList.get_Item(0).ObjID;
			}
			if (this.organList.get_Count() >= 1)
			{
				return this.organList.get_Item(0).ObjID;
			}
			if (this.eyeList.get_Count() >= 1)
			{
				return this.eyeList.get_Item(0).ObjID;
			}
			if (this.soldierList.get_Count() >= 1)
			{
				return this.soldierList.get_Item(0).ObjID;
			}
			return 0u;
		}

		private void FilterNearestCanTeleportActorByPosition(ref PoolObjHandle<ActorRoot> _actorPtr)
		{
			if (!this.curActorPtr.handle.CanTeleport(_actorPtr) || this.curActorPtr == _actorPtr)
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
			else if (TypeSearchCondition.Fit(_actorPtr, ActorTypeDef.Actor_Type_EYE, false))
			{
				if (DistanceSearchCondition.Fit(this.searchPosition, _actorPtr, this.searchRadius))
				{
					this.eyeFiler.Searcher(this.searchPosition, _actorPtr, new DistanceDelegate(TargetDistance.GetDistance));
				}
			}
			else if (TypeSearchCondition.FitSoldier(_actorPtr) && DistanceSearchCondition.Fit(this.searchPosition, _actorPtr, this.searchRadius))
			{
				this.soldierFilter.Searcher(this.searchPosition, _actorPtr, new DistanceDelegate(TargetDistance.GetDistance));
			}
		}

		public uint SearchNearestCanTeleportTarget(ref PoolObjHandle<ActorRoot> _actorPtr, VInt3 _position, int _srchR)
		{
			this.Clear();
			this.curActorPtr = _actorPtr;
			this.searchRadius = _srchR;
			this.searchPosition = _position;
			this.NearestHandler = new SceneManagement.Process(this.FilterNearestCanTeleportActorByPosition);
			SceneManagement instance = Singleton<SceneManagement>.GetInstance();
			SceneManagement.Coordinate coord = default(SceneManagement.Coordinate);
			instance.GetCoord_Center(ref coord, _position.xz, _srchR);
			instance.UpdateDirtyNodes();
			instance.ForeachActors(coord, this.NearestHandler);
			return this.GetSearchPriorityTarget();
		}
	}
}
