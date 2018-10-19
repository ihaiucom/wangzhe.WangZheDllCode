using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;

namespace AGE
{
	[EventCategory("MMGame/Skill")]
	public class CaptainSwitchTick : TickEvent
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int TargetId = -1;

		public bool isHostAcotr = true;

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			CaptainSwitchTick captainSwitchTick = src as CaptainSwitchTick;
			this.TargetId = captainSwitchTick.TargetId;
			this.isHostAcotr = captainSwitchTick.isHostAcotr;
		}

		public override BaseEvent Clone()
		{
			CaptainSwitchTick captainSwitchTick = ClassObjPool<CaptainSwitchTick>.Get();
			captainSwitchTick.CopyData(this);
			return captainSwitchTick;
		}

		public override void OnUse()
		{
			base.OnUse();
		}

		private void CaptainCallActorSwitch(ref PoolObjHandle<ActorRoot> tarActor)
		{
			HeroWrapper heroWrapper = tarActor.handle.ActorControl as HeroWrapper;
			if (heroWrapper != null)
			{
				CallActorWrapper callActorWrapper = heroWrapper.GetCallActor().handle.ActorControl as CallActorWrapper;
				if (callActorWrapper != null)
				{
					callActorWrapper.CaptainCallActorSwitch();
				}
			}
		}

		private void CaptainHostSwitch(ref PoolObjHandle<ActorRoot> tarActor)
		{
			CallActorWrapper callActorWrapper = tarActor.handle.ActorControl as CallActorWrapper;
			if (callActorWrapper != null)
			{
				callActorWrapper.CaptainHostSwitch();
			}
		}

		public override void Process(Action _action, Track _track)
		{
			PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.TargetId);
			if (!actorHandle)
			{
				if (ActionManager.Instance.isPrintLog)
				{
				}
				return;
			}
			if (this.isHostAcotr)
			{
				this.CaptainCallActorSwitch(ref actorHandle);
			}
			else
			{
				this.CaptainHostSwitch(ref actorHandle);
			}
		}
	}
}
