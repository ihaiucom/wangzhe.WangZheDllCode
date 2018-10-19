using Assets.Scripts.Common;
using System;

namespace Assets.Scripts.GameLogic.SkillFunc
{
	[SkillFuncHandlerClass]
	internal class SkillFuncDefendDelegator
	{
		[SkillFuncHandler(30, new int[]
		{

		})]
		public static bool OnSkillFuncImmuneHurt(ref SSkillFuncContext inContext)
		{
			return true;
		}

		[SkillFuncHandler(31, new int[]
		{

		})]
		public static bool OnSkillFuncImmuneControlSkillEffect(ref SSkillFuncContext inContext)
		{
			if (inContext.inStage == ESkillFuncStage.Enter)
			{
				PoolObjHandle<ActorRoot> inTargetObj = inContext.inTargetObj;
				if (inTargetObj)
				{
					inTargetObj.handle.ActorControl.AddNoAbilityFlag(ObjAbilityType.ObjAbility_ImmuneControl);
					if (inContext.GetSkillFuncParam(0, false) == 1)
					{
						int typeMask = 4;
						inTargetObj.handle.BuffHolderComp.ClearEffectTypeBuff(typeMask);
					}
				}
			}
			else if (inContext.inStage == ESkillFuncStage.Leave)
			{
				PoolObjHandle<ActorRoot> inTargetObj2 = inContext.inTargetObj;
				if (inTargetObj2)
				{
					inTargetObj2.handle.ActorControl.RmvNoAbilityFlag(ObjAbilityType.ObjAbility_ImmuneControl);
				}
			}
			return true;
		}

		[SkillFuncHandler(15, new int[]
		{

		})]
		public static bool OnSkillFuncImmuneNegativeSkillEffect(ref SSkillFuncContext inContext)
		{
			if (inContext.inStage == ESkillFuncStage.Enter)
			{
				PoolObjHandle<ActorRoot> inTargetObj = inContext.inTargetObj;
				if (inTargetObj)
				{
					inTargetObj.handle.ActorControl.AddNoAbilityFlag(ObjAbilityType.ObjAbility_ImmuneNegative);
					if (inContext.GetSkillFuncParam(0, false) == 1)
					{
						int num = 2;
						num += 4;
						inTargetObj.handle.BuffHolderComp.ClearEffectTypeBuff(num);
					}
				}
			}
			else if (inContext.inStage == ESkillFuncStage.Leave)
			{
				PoolObjHandle<ActorRoot> inTargetObj2 = inContext.inTargetObj;
				if (inTargetObj2)
				{
					inTargetObj2.handle.ActorControl.RmvNoAbilityFlag(ObjAbilityType.ObjAbility_ImmuneNegative);
				}
			}
			return true;
		}

		[SkillFuncHandler(34, new int[]
		{

		})]
		public static bool OnSkillFuncClearSkillEffect(ref SSkillFuncContext inContext)
		{
			if (inContext.inStage == ESkillFuncStage.Enter)
			{
				PoolObjHandle<ActorRoot> inTargetObj = inContext.inTargetObj;
				if (inTargetObj)
				{
					int skillFuncParam = inContext.GetSkillFuncParam(0, false);
					inTargetObj.handle.BuffHolderComp.ClearEffectTypeBuff(skillFuncParam);
				}
			}
			return true;
		}

		[SkillFuncHandler(54, new int[]
		{

		})]
		public static bool OnSkillFuncImmuneDeadHurt(ref SSkillFuncContext inContext)
		{
			return true;
		}
	}
}
