using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using CSProtocol;
using Pathfinding.RVO;
using ResData;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameLogic.SkillFunc
{
	[SkillFuncHandlerClass]
	internal class SkillFuncSpecialDelegator
	{
		[SkillFuncHandler(38, new int[]
		{

		})]
		public static bool OnSkillFuncSightArea(ref SSkillFuncContext inContext)
		{
			if (inContext.inStage == ESkillFuncStage.Enter)
			{
				PoolObjHandle<ActorRoot> inTargetObj = inContext.inTargetObj;
				if (inTargetObj)
				{
					List<PoolObjHandle<ActorRoot>> heroActors = Singleton<GameObjMgr>.GetInstance().HeroActors;
					for (int i = 0; i < heroActors.get_Count(); i++)
					{
						ActorRoot handle = heroActors.get_Item(i).handle;
						if (inTargetObj.handle.TheActorMeta.ActorCamp != handle.TheActorMeta.ActorCamp)
						{
							handle.HorizonMarker.SetEnabled(false);
						}
					}
				}
			}
			else if (inContext.inStage == ESkillFuncStage.Leave)
			{
				PoolObjHandle<ActorRoot> inTargetObj2 = inContext.inTargetObj;
				if (inTargetObj2)
				{
					List<PoolObjHandle<ActorRoot>> heroActors2 = Singleton<GameObjMgr>.GetInstance().HeroActors;
					for (int j = 0; j < heroActors2.get_Count(); j++)
					{
						ActorRoot handle2 = heroActors2.get_Item(j).handle;
						if (inTargetObj2.handle.TheActorMeta.ActorCamp != handle2.TheActorMeta.ActorCamp)
						{
							handle2.HorizonMarker.SetEnabled(true);
						}
					}
				}
			}
			return true;
		}

		[SkillFuncHandler(32, new int[]
		{

		})]
		public static bool OnSkillFuncReviveSoon(ref SSkillFuncContext inContext)
		{
			return true;
		}

		private static void SkillFuncChangeSkillCDImpl(ref SSkillFuncContext inContext, int changeType, int slotMask, int value)
		{
			PoolObjHandle<ActorRoot> inTargetObj = inContext.inTargetObj;
			if (inTargetObj)
			{
				SkillComponent skillControl = inTargetObj.handle.SkillControl;
				if (skillControl != null)
				{
					SkillSlot skillSlot = null;
					for (int i = 0; i < 10; i++)
					{
						if (((slotMask == 0 && i != 0 && i != 4 && i != 5 && i != 7) || (slotMask & 1 << i) > 0) && skillControl.TryGetSkillSlot((SkillSlotType)i, out skillSlot) && skillSlot != null)
						{
							if (changeType == 0)
							{
								skillSlot.ChangeSkillCD(value);
							}
							else
							{
								skillSlot.ChangeMaxCDRate(value);
							}
						}
					}
				}
			}
		}

		[SkillFuncHandler(16, new int[]
		{

		})]
		public static bool OnSkillFuncChangeSkillCD(ref SSkillFuncContext inContext)
		{
			if (inContext.inStage == ESkillFuncStage.Enter)
			{
				int skillFuncParam = inContext.GetSkillFuncParam(0, false);
				int skillFuncParam2 = inContext.GetSkillFuncParam(1, false);
				int skillFuncParam3 = inContext.GetSkillFuncParam(2, false);
				inContext.LocalParams[0].iParam = skillFuncParam;
				inContext.LocalParams[1].iParam = skillFuncParam2;
				inContext.LocalParams[2].iParam = skillFuncParam3;
				SkillFuncSpecialDelegator.SkillFuncChangeSkillCDImpl(ref inContext, skillFuncParam, skillFuncParam2, skillFuncParam3);
			}
			else if (inContext.inStage == ESkillFuncStage.Leave)
			{
				int iParam = inContext.LocalParams[0].iParam;
				int iParam2 = inContext.LocalParams[1].iParam;
				int value = -inContext.LocalParams[2].iParam;
				if (iParam != 0)
				{
					SkillFuncSpecialDelegator.SkillFuncChangeSkillCDImpl(ref inContext, iParam, iParam2, value);
				}
			}
			return true;
		}

		[SkillFuncHandler(39, new int[]
		{

		})]
		public static bool OnSkillFuncInvisible(ref SSkillFuncContext inContext)
		{
			PoolObjHandle<ActorRoot> inTargetObj = inContext.inTargetObj;
			if (inTargetObj)
			{
				if (inContext.inStage == ESkillFuncStage.Enter)
				{
					inTargetObj.handle.HorizonMarker.AddHideMark(COM_PLAYERCAMP.COM_PLAYERCAMP_COUNT, HorizonConfig.HideMark.Skill, 1, false);
					inTargetObj.handle.HorizonMarker.SetTranslucentMark(HorizonConfig.HideMark.Skill, true, false);
				}
				else if (inContext.inStage == ESkillFuncStage.Leave)
				{
					COM_PLAYERCAMP[] othersCmp = BattleLogic.GetOthersCmp(inTargetObj.handle.TheActorMeta.ActorCamp);
					for (int i = 0; i < othersCmp.Length; i++)
					{
						if (inTargetObj.handle.HorizonMarker.HasHideMark(othersCmp[i], HorizonConfig.HideMark.Skill))
						{
							inTargetObj.handle.HorizonMarker.AddHideMark(othersCmp[i], HorizonConfig.HideMark.Skill, -1, false);
						}
					}
					inTargetObj.handle.HorizonMarker.SetTranslucentMark(HorizonConfig.HideMark.Skill, false, false);
				}
			}
			return true;
		}

		[SkillFuncHandler(75, new int[]
		{

		})]
		public static bool OnSkillFuncShowMark(ref SSkillFuncContext inContext)
		{
			PoolObjHandle<ActorRoot> inTargetObj = inContext.inTargetObj;
			int skillFuncParam = inContext.GetSkillFuncParam(0, false);
			if (inTargetObj)
			{
				if (inContext.inStage == ESkillFuncStage.Enter)
				{
					if (skillFuncParam == 1)
					{
						inTargetObj.handle.HorizonMarker.AddShowMark(inContext.inOriginator.handle.TheActorMeta.ActorCamp, HorizonConfig.ShowMark.Skill, 1);
					}
					else
					{
						inTargetObj.handle.HorizonMarker.AddShowMark(COM_PLAYERCAMP.COM_PLAYERCAMP_COUNT, HorizonConfig.ShowMark.Skill, 1);
					}
				}
				else if (inContext.inStage == ESkillFuncStage.Leave)
				{
					if (skillFuncParam == 1)
					{
						if (inTargetObj.handle.HorizonMarker.HasShowMark(inContext.inOriginator.handle.TheActorMeta.ActorCamp, HorizonConfig.ShowMark.Skill))
						{
							inTargetObj.handle.HorizonMarker.AddShowMark(inContext.inOriginator.handle.TheActorMeta.ActorCamp, HorizonConfig.ShowMark.Skill, -1);
						}
					}
					else
					{
						COM_PLAYERCAMP[] othersCmp = BattleLogic.GetOthersCmp(inTargetObj.handle.TheActorMeta.ActorCamp);
						for (int i = 0; i < othersCmp.Length; i++)
						{
							if (inTargetObj.handle.HorizonMarker.HasShowMark(othersCmp[i], HorizonConfig.ShowMark.Skill))
							{
								inTargetObj.handle.HorizonMarker.AddShowMark(othersCmp[i], HorizonConfig.ShowMark.Skill, -1);
							}
						}
					}
				}
			}
			return true;
		}

		[SkillFuncHandler(45, new int[]
		{

		})]
		public static bool OnSkillFuncIgnoreRVO(ref SSkillFuncContext inContext)
		{
			PoolObjHandle<ActorRoot> inTargetObj = inContext.inTargetObj;
			if (inTargetObj)
			{
				if (inContext.inStage == ESkillFuncStage.Enter)
				{
					RVOController component = inTargetObj.handle.gameObject.GetComponent<RVOController>();
					if (component != null)
					{
						component.enabled = false;
					}
				}
				else if (inContext.inStage == ESkillFuncStage.Leave)
				{
					RVOController component2 = inTargetObj.handle.gameObject.GetComponent<RVOController>();
					if (component2 != null)
					{
						component2.enabled = true;
					}
				}
			}
			return true;
		}

		[SkillFuncHandler(46, new int[]
		{

		})]
		public static bool OnSkillFuncHpCondition(ref SSkillFuncContext inContext)
		{
			int skillFuncParam = inContext.GetSkillFuncParam(1, false);
			if (skillFuncParam < 0 || skillFuncParam >= 37)
			{
				return false;
			}
			RES_FUNCEFT_TYPE key = (RES_FUNCEFT_TYPE)skillFuncParam;
			PoolObjHandle<ActorRoot> inTargetObj = inContext.inTargetObj;
			if (!inTargetObj)
			{
				return false;
			}
			int skillFuncParam2 = inContext.GetSkillFuncParam(2, false);
			if (inContext.inStage != ESkillFuncStage.Leave)
			{
				int actorHp = inTargetObj.handle.ValueComponent.actorHp;
				int totalValue = inTargetObj.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP].totalValue;
				int num = 10000 - actorHp * 10000 / totalValue;
				int skillFuncParam3 = inContext.GetSkillFuncParam(3, true);
				int skillFuncParam4 = inContext.GetSkillFuncParam(0, false);
				if (skillFuncParam4 == 0)
				{
					return false;
				}
				int num2 = num * skillFuncParam3 / skillFuncParam4;
				int iParam = inContext.LocalParams[0].iParam;
				if (skillFuncParam2 == 1)
				{
					ValueDataInfo valueDataInfo = inTargetObj.handle.ValueComponent.mActorValue[key] >> iParam;
					valueDataInfo = inTargetObj.handle.ValueComponent.mActorValue[key] << num2;
				}
				else
				{
					ValueDataInfo valueDataInfo2 = inTargetObj.handle.ValueComponent.mActorValue[key] - iParam;
					valueDataInfo2 = inTargetObj.handle.ValueComponent.mActorValue[key] + num2;
				}
				inContext.LocalParams[0].iParam = num2;
			}
			else if (inContext.inStage == ESkillFuncStage.Leave)
			{
				int iParam2 = inContext.LocalParams[0].iParam;
				if (skillFuncParam2 == 1)
				{
					ValueDataInfo valueDataInfo3 = inTargetObj.handle.ValueComponent.mActorValue[key] >> iParam2;
				}
				else
				{
					ValueDataInfo valueDataInfo4 = inTargetObj.handle.ValueComponent.mActorValue[key] - iParam2;
				}
			}
			return true;
		}

		[SkillFuncHandler(47, new int[]
		{

		})]
		public static bool OnSkillFuncChangeHudStyle(ref SSkillFuncContext inContext)
		{
			PoolObjHandle<ActorRoot> inTargetObj = inContext.inTargetObj;
			if (inTargetObj)
			{
				if (inContext.inStage == ESkillFuncStage.Enter)
				{
					inTargetObj.handle.HudControl.bBossHpBar = true;
				}
				else if (inContext.inStage == ESkillFuncStage.Leave)
				{
					inTargetObj.handle.HudControl.bBossHpBar = false;
				}
			}
			return true;
		}

		[SkillFuncHandler(55, new int[]
		{

		})]
		public static bool OnSkillFuncChangeSkill(ref SSkillFuncContext inContext)
		{
			PoolObjHandle<ActorRoot> inTargetObj = inContext.inTargetObj;
			if (inTargetObj)
			{
				if (inContext.inStage == ESkillFuncStage.Enter)
				{
					int skillFuncParam = inContext.GetSkillFuncParam(0, false);
					int skillFuncParam2 = inContext.GetSkillFuncParam(1, false);
					int skillFuncParam3 = inContext.GetSkillFuncParam(2, false);
					bool flag = inContext.GetSkillFuncParam(3, false) == 1;
					inContext.LocalParams[0].iParam = skillFuncParam;
					if (inTargetObj.handle.BuffHolderComp != null)
					{
						BuffChangeSkillRule changeSkillRule = inTargetObj.handle.BuffHolderComp.changeSkillRule;
						if (changeSkillRule != null)
						{
							changeSkillRule.ChangeSkillSlot((SkillSlotType)skillFuncParam, skillFuncParam2, skillFuncParam3);
						}
					}
					if (flag && skillFuncParam == 0)
					{
						inTargetObj.handle.SkillControl.bImmediateAttack = flag;
					}
				}
				else if (inContext.inStage == ESkillFuncStage.Leave)
				{
					int iParam = inContext.LocalParams[0].iParam;
					bool flag2 = inContext.GetSkillFuncParam(3, false) == 1;
					if (inTargetObj.handle.BuffHolderComp != null)
					{
						BuffChangeSkillRule changeSkillRule2 = inTargetObj.handle.BuffHolderComp.changeSkillRule;
						if (changeSkillRule2 != null)
						{
							changeSkillRule2.RecoverSkillSlot((SkillSlotType)iParam);
						}
						if (flag2 && iParam == 0)
						{
							inTargetObj.handle.SkillControl.bImmediateAttack = false;
							inTargetObj.handle.ActorControl.CancelCommonAttackMode();
						}
					}
				}
			}
			return true;
		}

		[SkillFuncHandler(56, new int[]
		{

		})]
		public static bool OnSkillFuncDisableSkill(ref SSkillFuncContext inContext)
		{
			PoolObjHandle<ActorRoot> inTargetObj = inContext.inTargetObj;
			if (inTargetObj)
			{
				if (inContext.inStage == ESkillFuncStage.Enter)
				{
					for (int i = 0; i < 8; i++)
					{
						int skillFuncParam = inContext.GetSkillFuncParam(i, false);
						if (skillFuncParam == 1)
						{
							inTargetObj.handle.ActorControl.AddDisableSkillFlag((SkillSlotType)i, false);
						}
					}
				}
				else if (inContext.inStage == ESkillFuncStage.Leave)
				{
					for (int j = 0; j < 8; j++)
					{
						int skillFuncParam2 = inContext.GetSkillFuncParam(j, false);
						if (skillFuncParam2 == 1)
						{
							inTargetObj.handle.ActorControl.RmvDisableSkillFlag((SkillSlotType)j, false);
						}
					}
				}
			}
			return true;
		}

		[SkillFuncHandler(70, new int[]
		{

		})]
		public static bool OnSkillFuncRemoveSkillBuff(ref SSkillFuncContext inContext)
		{
			if (inContext.inStage == ESkillFuncStage.Enter)
			{
				PoolObjHandle<ActorRoot> inTargetObj = inContext.inTargetObj;
				if (inTargetObj)
				{
					int skillFuncParam = inContext.GetSkillFuncParam(0, false);
					inTargetObj.handle.BuffHolderComp.RemoveBuff(skillFuncParam);
				}
			}
			return true;
		}

		[SkillFuncHandler(72, new int[]
		{

		})]
		public static bool OnSkillFuncSkillExtraHurt(ref SSkillFuncContext inContext)
		{
			return true;
		}

		[SkillFuncHandler(73, new int[]
		{

		})]
		public static bool OnSkillFuncSkillChangeParam(ref SSkillFuncContext inContext)
		{
			PoolObjHandle<ActorRoot> inTargetObj = inContext.inTargetObj;
			if (inTargetObj)
			{
				if (inContext.inStage == ESkillFuncStage.Enter)
				{
					int skillFuncParam = inContext.GetSkillFuncParam(0, false);
					int skillFuncParam2 = inContext.GetSkillFuncParam(1, false);
					int skillFuncParam3 = inContext.GetSkillFuncParam(2, false);
					inTargetObj.handle.SkillControl.ChangePassiveParam(skillFuncParam, skillFuncParam2, skillFuncParam3);
				}
				else if (inContext.inStage == ESkillFuncStage.Leave)
				{
					int skillFuncParam4 = inContext.GetSkillFuncParam(0, false);
					int skillFuncParam5 = inContext.GetSkillFuncParam(1, false);
					int skillFuncParam6 = inContext.GetSkillFuncParam(3, false);
					inTargetObj.handle.SkillControl.ChangePassiveParam(skillFuncParam4, skillFuncParam5, skillFuncParam6);
				}
			}
			return true;
		}

		[SkillFuncHandler(84, new int[]
		{

		})]
		public static bool OnSkillFuncBounceSkillEffect(ref SSkillFuncContext inContext)
		{
			return true;
		}

		[SkillFuncHandler(85, new int[]
		{

		})]
		public static bool OnSkillFuncDecreaseReviveTime(ref SSkillFuncContext inContext)
		{
			PoolObjHandle<ActorRoot> inTargetObj = inContext.inTargetObj;
			if (inTargetObj && inContext.inStage == ESkillFuncStage.Enter)
			{
				int skillFuncParam = inContext.GetSkillFuncParam(0, false);
				int skillFuncParam2 = inContext.GetSkillFuncParam(1, false);
				if (skillFuncParam == 0)
				{
					inTargetObj.handle.ActorControl.ReviveCooldown -= skillFuncParam2;
				}
				else if (skillFuncParam == 1)
				{
					inTargetObj.handle.ActorControl.ReviveCooldown -= (int)((long)(skillFuncParam2 * inTargetObj.handle.ActorControl.CfgReviveCD) / 10000L);
				}
			}
			return true;
		}

		[SkillFuncHandler(86, new int[]
		{

		})]
		public static bool OnSkillFuncAddGold(ref SSkillFuncContext inContext)
		{
			if (inContext.inStage != ESkillFuncStage.Leave)
			{
				PoolObjHandle<ActorRoot> inTargetObj = inContext.inTargetObj;
				if (inTargetObj)
				{
					int num = inContext.GetSkillFuncParam(0, false);
					int skillFuncParam = inContext.GetSkillFuncParam(1, false);
					int skillFuncParam2 = inContext.GetSkillFuncParam(2, false);
					if (skillFuncParam2 > 0 && skillFuncParam > 0)
					{
						ulong num2 = Singleton<FrameSynchr>.GetInstance().LogicFrameTick / 1000uL;
						num += skillFuncParam * (int)(num2 / (ulong)((long)skillFuncParam2));
					}
					inTargetObj.handle.ValueComponent.ChangeGoldCoinInBattle(num, true, false, default(Vector3), false, default(PoolObjHandle<ActorRoot>));
				}
			}
			return true;
		}
	}
}
