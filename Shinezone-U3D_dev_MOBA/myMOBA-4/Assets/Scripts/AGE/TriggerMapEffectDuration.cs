using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.GameSystem;
using System;

namespace AGE
{
	[EventCategory("Effect")]
	public class TriggerMapEffectDuration : DurationEvent
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int selfId;

		[ObjectTemplate(new Type[]
		{

		})]
		public int targetId;

		public override BaseEvent Clone()
		{
			TriggerMapEffectDuration triggerMapEffectDuration = ClassObjPool<TriggerMapEffectDuration>.Get();
			triggerMapEffectDuration.CopyData(this);
			return triggerMapEffectDuration;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			TriggerMapEffectDuration triggerMapEffectDuration = src as TriggerMapEffectDuration;
			this.targetId = triggerMapEffectDuration.targetId;
			this.selfId = triggerMapEffectDuration.selfId;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.targetId = 0;
			this.selfId = 0;
		}

		public override void Enter(Action _action, Track _track)
		{
			PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.selfId);
			PoolObjHandle<ActorRoot> actorHandle2 = _action.GetActorHandle(this.targetId);
			if (!actorHandle || !actorHandle2)
			{
				return;
			}
			MinimapSys theMinimapSys = Singleton<CBattleSystem>.instance.TheMinimapSys;
			if (theMinimapSys == null || theMinimapSys.miniMapEffectModule == null)
			{
				return;
			}
			Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
			if (hostPlayer == null || !hostPlayer.Captain)
			{
				return;
			}
			if (actorHandle.handle.Visible)
			{
				string eft = (!actorHandle.handle.IsHostCamp()) ? MiniMapEffectModule.teleportUIEft_src_enemy : MiniMapEffectModule.teleportUIEft_src;
				theMinimapSys.miniMapEffectModule.PlayFollowActorEffect(eft, -1f, actorHandle);
			}
			int num = Horizon.QueryGlobalSight();
			VInt3 worldLoc = new VInt3(actorHandle2.handle.location.x, actorHandle2.handle.location.z, 0);
			if (Singleton<GameFowManager>.instance.IsVisible(worldLoc, hostPlayer.PlayerCamp) || (actorHandle2.handle.location - hostPlayer.Captain.handle.location).sqrMagnitudeLong2D < (long)(num * num))
			{
				string eft2 = (!actorHandle2.handle.IsHostCamp()) ? MiniMapEffectModule.teleportUIEft_dst_enemy : MiniMapEffectModule.teleportUIEft_dst;
				theMinimapSys.miniMapEffectModule.PlayFollowActorEffect(eft2, -1f, actorHandle2);
			}
		}

		public override void Leave(Action _action, Track _track)
		{
			PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.selfId);
			PoolObjHandle<ActorRoot> actorHandle2 = _action.GetActorHandle(this.targetId);
			if (!actorHandle || !actorHandle2)
			{
				return;
			}
			MinimapSys theMinimapSys = Singleton<CBattleSystem>.instance.TheMinimapSys;
			if (theMinimapSys == null || theMinimapSys.miniMapEffectModule == null)
			{
				return;
			}
			theMinimapSys.miniMapEffectModule.StopFollowActorEffect(actorHandle);
			theMinimapSys.miniMapEffectModule.StopFollowActorEffect(actorHandle2);
		}
	}
}
