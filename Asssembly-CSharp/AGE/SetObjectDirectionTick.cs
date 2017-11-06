using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;

namespace AGE
{
	[EventCategory("MMGame/Skill")]
	public class SetObjectDirectionTick : TickEvent
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int targetId = -1;

		public VInt3 targetDir = VInt3.forward;

		public override void OnUse()
		{
			base.OnUse();
			this.targetId = -1;
			this.targetDir = VInt3.forward;
		}

		public override BaseEvent Clone()
		{
			SetObjectDirectionTick setObjectDirectionTick = ClassObjPool<SetObjectDirectionTick>.Get();
			setObjectDirectionTick.CopyData(this);
			return setObjectDirectionTick;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			SetObjectDirectionTick setObjectDirectionTick = src as SetObjectDirectionTick;
			this.targetId = setObjectDirectionTick.targetId;
			this.targetDir = setObjectDirectionTick.targetDir;
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
			if (this.targetDir.sqrMagnitudeLong < 1L)
			{
				if (ActionManager.Instance.isPrintLog)
				{
				}
				return;
			}
			actorHandle.handle.MovementComponent.SetRotate(this.targetDir, true);
		}
	}
}
