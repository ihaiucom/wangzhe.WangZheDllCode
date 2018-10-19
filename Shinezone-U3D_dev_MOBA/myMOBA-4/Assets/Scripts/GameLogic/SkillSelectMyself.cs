using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	[SkillBaseSelectTarget(SkillTargetRule.Myself)]
	public class SkillSelectMyself : SkillBaseSelectTarget
	{
		public override ActorRoot SelectTarget(SkillSlot UseSlot)
		{
			if (UseSlot.SlotType == SkillSlotType.SLOT_SKILL_4)
			{
				SkillChooseTargetEventParam skillChooseTargetEventParam = new SkillChooseTargetEventParam(UseSlot.Actor, UseSlot.Actor, 1);
				Singleton<GameEventSys>.instance.SendEvent<SkillChooseTargetEventParam>(GameEventDef.Event_ActorBeChosenAsTarget, ref skillChooseTargetEventParam);
			}
			return UseSlot.Actor.handle;
		}

		public override VInt3 SelectTargetDir(SkillSlot UseSlot)
		{
			return UseSlot.Actor.handle.forward;
		}
	}
}
