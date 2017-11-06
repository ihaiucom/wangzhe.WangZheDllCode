using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;

namespace AGE
{
	[EventCategory("MMGame/Skill")]
	internal class PickFlyDuration : DurationCondition
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int targetId;

		public int height;

		public int gravity = -10;

		private PoolObjHandle<ActorRoot> actor_;

		private bool done_;

		private int lastTime_;

		private int totalTime;

		private bool bMotionControl;

		private PlayerMovement movement;

		private AccelerateMotionControler motionControler;

		public override void OnUse()
		{
			base.OnUse();
			this.targetId = 0;
			this.height = 0;
			this.gravity = -10;
			this.actor_.Release();
			this.done_ = false;
			this.lastTime_ = 0;
			this.totalTime = 0;
			this.bMotionControl = false;
			this.movement = null;
			this.motionControler = null;
		}

		public override BaseEvent Clone()
		{
			PickFlyDuration pickFlyDuration = ClassObjPool<PickFlyDuration>.Get();
			pickFlyDuration.CopyData(this);
			return pickFlyDuration;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			PickFlyDuration pickFlyDuration = src as PickFlyDuration;
			this.targetId = pickFlyDuration.targetId;
			this.height = pickFlyDuration.height;
			this.gravity = pickFlyDuration.gravity;
			this.actor_ = pickFlyDuration.actor_;
			this.done_ = pickFlyDuration.done_;
			this.lastTime_ = pickFlyDuration.lastTime_;
			this.totalTime = pickFlyDuration.totalTime;
			this.bMotionControl = pickFlyDuration.bMotionControl;
			this.movement = pickFlyDuration.movement;
			this.motionControler = pickFlyDuration.motionControler;
		}

		public override void Enter(Action _action, Track _track)
		{
			this.done_ = false;
			base.Enter(_action, _track);
			this.actor_ = _action.GetActorHandle(this.targetId);
			if (!this.actor_)
			{
				return;
			}
			if (!this.actor_.handle.isMovable)
			{
				this.actor_.Release();
				this.done_ = true;
				return;
			}
			this.actor_.handle.ActorControl.TerminateMove();
			this.actor_.handle.ActorControl.AddNoAbilityFlag(ObjAbilityType.ObjAbility_Move);
			this.lastTime_ = 0;
			this.movement = (this.actor_.handle.MovementComponent as PlayerMovement);
			this.motionControler = new AccelerateMotionControler();
			if (this.movement != null)
			{
				int motionControlerCount = this.movement.GravityMode.GetMotionControlerCount();
				if (motionControlerCount < 3)
				{
					if (motionControlerCount == 0)
					{
						this.motionControler.InitMotionControler(this.length, 0, this.gravity);
					}
					else if (motionControlerCount == 1)
					{
						this.motionControler.InitMotionControler(this.length, 0, IntMath.Divide(this.gravity * 6, 10));
					}
					else if (motionControlerCount == 2)
					{
						this.motionControler.InitMotionControler(this.length, 0, IntMath.Divide(this.gravity * 4, 10));
					}
					this.movement.GravityMode.AddMotionControler(this.motionControler);
					this.movement.isFlying = true;
					this.bMotionControl = true;
				}
			}
		}

		public override void Process(Action _action, Track _track, int _localTime)
		{
			base.Process(_action, _track, _localTime);
		}

		public override void Leave(Action _action, Track _track)
		{
			if (!this.actor_)
			{
				return;
			}
			this.actor_.handle.ActorControl.RmvNoAbilityFlag(ObjAbilityType.ObjAbility_Move);
			this.done_ = true;
			if (this.movement != null && this.bMotionControl)
			{
				this.movement.GravityMode.RemoveMotionControler(this.motionControler);
			}
		}

		public override bool Check(Action _action, Track _track)
		{
			return this.done_;
		}
	}
}
