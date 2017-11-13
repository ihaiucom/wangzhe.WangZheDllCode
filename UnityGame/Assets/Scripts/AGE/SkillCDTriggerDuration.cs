using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;

namespace AGE
{
	[EventCategory("MMGame/Skill")]
	public class SkillCDTriggerDuration : DurationCondition
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int targetId = -1;

		public bool useSlotType;

		public SkillSlotType slotType;

		public bool bAbortReduce;

		public int abortReduceTime;

		private PoolObjHandle<ActorRoot> targetActor;

		private SkillComponent skillControl;

		public override BaseEvent Clone()
		{
			SkillCDTriggerDuration skillCDTriggerDuration = ClassObjPool<SkillCDTriggerDuration>.Get();
			skillCDTriggerDuration.CopyData(this);
			return skillCDTriggerDuration;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			SkillCDTriggerDuration skillCDTriggerDuration = src as SkillCDTriggerDuration;
			this.targetId = skillCDTriggerDuration.targetId;
			this.useSlotType = skillCDTriggerDuration.useSlotType;
			this.slotType = skillCDTriggerDuration.slotType;
			this.abortReduceTime = skillCDTriggerDuration.abortReduceTime;
			this.bAbortReduce = skillCDTriggerDuration.bAbortReduce;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.targetId = -1;
			this.targetActor.Release();
			this.skillControl = null;
		}

		public override void Leave(Action _action, Track _track)
		{
			base.Leave(_action, _track);
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
			SkillSlot skillSlot = null;
			if (!this.useSlotType)
			{
				this.StartSkillContextCD(_action, ref skillSlot);
			}
			else
			{
				this.StartSkillSlotCD(ref skillSlot);
			}
			if (skillSlot != null && this.bAbortReduce && _track.curTime <= base.End && !_track.Loop)
			{
				skillSlot.ChangeSkillCD(this.abortReduceTime);
			}
		}

		private void StartSkillContextCD(Action _action, ref SkillSlot slot)
		{
			SkillUseContext refParamObject = _action.refParams.GetRefParamObject<SkillUseContext>("SkillContext");
			if (refParamObject != null && this.skillControl.TryGetSkillSlot(refParamObject.SlotType, out slot) && slot != null)
			{
				slot.StartSkillCD();
			}
		}

		private void StartSkillSlotCD(ref SkillSlot slot)
		{
			if (this.skillControl.TryGetSkillSlot(this.slotType, out slot) && slot != null)
			{
				slot.StartSkillCD();
			}
		}
	}
}
