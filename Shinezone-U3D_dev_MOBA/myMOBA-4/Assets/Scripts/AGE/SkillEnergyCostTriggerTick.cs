using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;

namespace AGE
{
	[EventCategory("MMGame/Skill")]
	public class SkillEnergyCostTriggerTick : TickEvent
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int targetId = -1;

		public override BaseEvent Clone()
		{
			SkillEnergyCostTriggerTick skillEnergyCostTriggerTick = ClassObjPool<SkillEnergyCostTriggerTick>.Get();
			skillEnergyCostTriggerTick.CopyData(this);
			return skillEnergyCostTriggerTick;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			SkillEnergyCostTriggerTick skillEnergyCostTriggerTick = src as SkillEnergyCostTriggerTick;
			this.targetId = skillEnergyCostTriggerTick.targetId;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.targetId = -1;
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
			SkillComponent skillControl = actorHandle.handle.SkillControl;
			if (skillControl == null)
			{
				if (ActionManager.Instance.isPrintLog)
				{
				}
				return;
			}
			SkillUseContext refParamObject = _action.refParams.GetRefParamObject<SkillUseContext>("SkillContext");
			SkillSlot skillSlot;
			if (refParamObject != null && skillControl.TryGetSkillSlot(refParamObject.SlotType, out skillSlot) && skillSlot != null)
			{
				skillSlot.CurSkillEnergyCostTick();
			}
		}
	}
}
