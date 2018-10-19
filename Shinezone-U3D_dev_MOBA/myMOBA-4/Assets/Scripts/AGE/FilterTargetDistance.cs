using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.GameSystem;
using System;

namespace AGE
{
	[EventCategory("MMGame/Skill")]
	public class FilterTargetDistance : TickCondition
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int sourceId = -1;

		public int targetId = -1;

		private bool bCheckFilter;

		public override BaseEvent Clone()
		{
			FilterTargetDistance filterTargetDistance = ClassObjPool<FilterTargetDistance>.Get();
			filterTargetDistance.CopyData(this);
			return filterTargetDistance;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			FilterTargetDistance filterTargetDistance = src as FilterTargetDistance;
			if (filterTargetDistance != null)
			{
				this.sourceId = filterTargetDistance.sourceId;
				this.targetId = filterTargetDistance.targetId;
				this.bCheckFilter = filterTargetDistance.bCheckFilter;
			}
		}

		public override void OnUse()
		{
			base.OnUse();
			this.sourceId = -1;
			this.targetId = -1;
			this.bCheckFilter = false;
		}

		private void CheckTransmitShowParticalDistance(ref PoolObjHandle<ActorRoot> srcObj, ref PoolObjHandle<ActorRoot> targetObj)
		{
			if (!srcObj && !targetObj)
			{
				return;
			}
			if (targetObj.handle.Visible)
			{
				this.bCheckFilter = true;
				return;
			}
			Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
			if (hostPlayer == null || !hostPlayer.Captain)
			{
				return;
			}
			int num = Horizon.QueryGlobalSight();
			VInt3 worldLoc = new VInt3(targetObj.handle.location.x, targetObj.handle.location.z, 0);
			bool flag = Singleton<GameFowManager>.instance.IsVisible(worldLoc, hostPlayer.PlayerCamp);
			if (flag)
			{
				this.bCheckFilter = true;
				return;
			}
			int count = Singleton<GameObjMgr>.instance.HeroActors.Count;
			for (int i = 0; i < count; i++)
			{
				PoolObjHandle<ActorRoot> ptr = Singleton<GameObjMgr>.instance.HeroActors[i];
				if (ptr && !ptr.handle.ActorControl.IsDeadState && !ActorHelper.IsHostEnemyActor(ref ptr) && (targetObj.handle.location - hostPlayer.Captain.handle.location).sqrMagnitudeLong2D < (long)(num * num))
				{
					this.bCheckFilter = true;
					break;
				}
			}
		}

		public override void Process(Action _action, Track _track)
		{
			PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.sourceId);
			if (!actorHandle)
			{
				if (ActionManager.Instance.isPrintLog)
				{
				}
				return;
			}
			PoolObjHandle<ActorRoot> actorHandle2 = _action.GetActorHandle(this.targetId);
			if (!actorHandle2)
			{
				if (ActionManager.Instance.isPrintLog)
				{
				}
				return;
			}
			this.CheckTransmitShowParticalDistance(ref actorHandle, ref actorHandle2);
			base.Process(_action, _track);
		}

		public override bool Check(Action _action, Track _track)
		{
			return this.bCheckFilter;
		}
	}
}
