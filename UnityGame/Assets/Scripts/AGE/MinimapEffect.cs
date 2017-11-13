using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameSystem;
using System;

namespace AGE
{
	[EventCategory("Effect")]
	public class MinimapEffect : DurationEvent
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int selfId;

		[ObjectTemplate(new Type[]
		{

		})]
		public int targetId;

		[ObjectTemplate(new Type[]
		{

		})]
		public string effect = string.Empty;

		public override BaseEvent Clone()
		{
			MinimapEffect minimapEffect = ClassObjPool<MinimapEffect>.Get();
			minimapEffect.CopyData(this);
			return minimapEffect;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			MinimapEffect minimapEffect = src as MinimapEffect;
			this.targetId = minimapEffect.targetId;
			this.selfId = minimapEffect.selfId;
			this.effect = minimapEffect.effect;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.targetId = 0;
			this.selfId = 0;
			this.effect = string.Empty;
		}

		public override void Enter(Action _action, Track _track)
		{
			MinimapSys theMinimapSys = Singleton<CBattleSystem>.instance.TheMinimapSys;
			if (theMinimapSys == null || theMinimapSys.miniMapEffectModule == null)
			{
				return;
			}
			PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.targetId);
			if (actorHandle)
			{
				theMinimapSys.miniMapEffectModule.PlayFollowActorEffect(this.effect, -1f, actorHandle);
			}
		}

		public override void Leave(Action _action, Track _track)
		{
			MinimapSys theMinimapSys = Singleton<CBattleSystem>.instance.TheMinimapSys;
			if (theMinimapSys == null || theMinimapSys.miniMapEffectModule == null)
			{
				return;
			}
			PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.targetId);
			if (actorHandle)
			{
				theMinimapSys.miniMapEffectModule.StopFollowActorEffect(actorHandle);
			}
		}
	}
}
