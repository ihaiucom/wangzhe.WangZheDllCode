using Assets.Scripts.Common;
using System;

namespace Assets.Scripts.GameLogic.SkillFunc
{
	[SkillFuncHandlerClass]
	internal class SkillFuncBuffDelegator
	{
		[SkillFuncHandler(28, new int[]
		{

		})]
		public static bool OnSkillFuncAddMark(ref SSkillFuncContext inContext)
		{
			if (inContext.inStage == ESkillFuncStage.Enter)
			{
				PoolObjHandle<ActorRoot> inTargetObj = inContext.inTargetObj;
				if (inTargetObj)
				{
					uint markType = 1u;
					if (inContext.inBuffSkill && inContext.inBuffSkill.handle.cfgData != null)
					{
						markType = (uint)inContext.inBuffSkill.handle.cfgData.bEffectType;
					}
					int skillFuncParam = inContext.GetSkillFuncParam(0, false);
					if (inContext.GetSkillFuncParam(1, false) == 0)
					{
						inTargetObj.handle.BuffHolderComp.markRule.AddBufferMark(inContext.inOriginator, skillFuncParam, markType, inContext.inUseContext);
					}
					else
					{
						inTargetObj.handle.BuffHolderComp.markRule.ClearBufferMark(inContext.inOriginator, skillFuncParam);
					}
				}
				return true;
			}
			return false;
		}

		[SkillFuncHandler(29, new int[]
		{

		})]
		public static bool OnSkillFuncTriggerMark(ref SSkillFuncContext inContext)
		{
			if (inContext.inStage == ESkillFuncStage.Enter)
			{
				PoolObjHandle<ActorRoot> inTargetObj = inContext.inTargetObj;
				if (inTargetObj)
				{
					int skillFuncParam = inContext.GetSkillFuncParam(0, false);
					inTargetObj.handle.BuffHolderComp.markRule.TriggerBufferMark(inContext.inOriginator, skillFuncParam, inContext.inUseContext);
					int skillFuncParam2 = inContext.GetSkillFuncParam(1, false);
					if (skillFuncParam2 != 0)
					{
						inTargetObj.handle.BuffHolderComp.RemoveBuff(skillFuncParam2);
					}
				}
				return true;
			}
			return false;
		}

		[SkillFuncHandler(51, new int[]
		{

		})]
		public static bool OnSkillFuncControlExtraEffect(ref SSkillFuncContext inContext)
		{
			return true;
		}
	}
}
