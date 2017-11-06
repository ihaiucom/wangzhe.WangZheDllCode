using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;

namespace AGE
{
	[EventCategory("MMGame/Skill")]
	public class SkillCDTriggerTick : TickEvent
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int targetId = -1;

		public bool useSlotType;

		public SkillSlotType slotType;

		private PoolObjHandle<ActorRoot> targetActor;

		private SkillComponent skillControl;

		public override BaseEvent Clone()
		{
			SkillCDTriggerTick skillCDTriggerTick = ClassObjPool<SkillCDTriggerTick>.Get();
			skillCDTriggerTick.CopyData(this);
			return skillCDTriggerTick;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			SkillCDTriggerTick skillCDTriggerTick = src as SkillCDTriggerTick;
			this.targetId = skillCDTriggerTick.targetId;
			this.useSlotType = skillCDTriggerTick.useSlotType;
			this.slotType = skillCDTriggerTick.slotType;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.targetId = -1;
		}

		private void StartSkillContextCD(Action _action)
		{
			SkillUseContext refParamObject = _action.refParams.GetRefParamObject<SkillUseContext>("SkillContext");
			SkillSlot skillSlot;
			if (refParamObject != null && this.skillControl.TryGetSkillSlot(refParamObject.SlotType, out skillSlot) && skillSlot != null)
			{
				skillSlot.StartSkillCD();
			}
		}

		private void StartSkillSlotCD()
		{
			SkillSlot skillSlot;
			if (this.skillControl.TryGetSkillSlot(this.slotType, out skillSlot) && skillSlot != null)
			{
				skillSlot.StartSkillCD();
			}
		}

		public override void Process(Action _action, Track _track)
		{
			this.targetActor = _action.GetActorHandle(this.targetId);
			if (!this.targetActor)
			{
				if (ActionManager.Instance.isPrintLog)
				{
				}
				return;
			}
			this.skillControl = this.targetActor.handle.SkillControl;
			if (this.skillControl == null)
			{
				if (ActionManager.Instance.isPrintLog)
				{
				}
				return;
			}
			if (!this.useSlotType)
			{
				this.StartSkillContextCD(_action);
			}
			else
			{
				this.StartSkillSlotCD();
			}
		}
	}
}
