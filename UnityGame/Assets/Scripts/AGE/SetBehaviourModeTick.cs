using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;

namespace AGE
{
	[EventCategory("MMGame/Skill")]
	public class SetBehaviourModeTick : TickEvent
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int targetId = -1;

		public bool stopMove = true;

		public bool clearMove;

		public bool stopCurSkill;

		public bool delayStopCurSkill;

		public bool deadControl;

		public override void OnUse()
		{
			base.OnUse();
			this.targetId = -1;
			this.stopMove = true;
			this.clearMove = false;
			this.stopCurSkill = false;
			this.delayStopCurSkill = false;
			this.deadControl = false;
		}

		public override BaseEvent Clone()
		{
			SetBehaviourModeTick setBehaviourModeTick = ClassObjPool<SetBehaviourModeTick>.Get();
			setBehaviourModeTick.CopyData(this);
			return setBehaviourModeTick;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			SetBehaviourModeTick setBehaviourModeTick = src as SetBehaviourModeTick;
			this.targetId = setBehaviourModeTick.targetId;
			this.stopMove = setBehaviourModeTick.stopMove;
			this.clearMove = setBehaviourModeTick.clearMove;
			this.stopCurSkill = setBehaviourModeTick.stopCurSkill;
			this.delayStopCurSkill = setBehaviourModeTick.delayStopCurSkill;
			this.deadControl = setBehaviourModeTick.deadControl;
		}

		public override void Process(Action _action, Track _track)
		{
			PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.targetId);
			if (!actorHandle)
			{
				if (ActionManager.Instance.isPrintLog)
				{
				}
				return;
			}
			ObjWrapper actorControl = actorHandle.handle.ActorControl;
			if (actorControl == null)
			{
				if (ActionManager.Instance.isPrintLog)
				{
				}
				return;
			}
			if (this.stopMove)
			{
				actorControl.TerminateMove();
			}
			if (this.clearMove)
			{
				actorControl.ClearMoveCommand();
			}
			if (this.stopCurSkill)
			{
				actorControl.ForceAbortCurUseSkill();
			}
			if (this.delayStopCurSkill && !this.stopCurSkill)
			{
				actorControl.DelayAbortCurUseSkill();
			}
			if (this.deadControl && actorControl.actor.TheStaticData.TheBaseAttribute.DeadControl && actorControl.IsDeadState)
			{
				actorControl.AddNoAbilityFlag(ObjAbilityType.ObjAbility_DeadControl);
				actorControl.SetDeadMode(ObjDeadMode.DeadState_Idle);
			}
		}
	}
}
