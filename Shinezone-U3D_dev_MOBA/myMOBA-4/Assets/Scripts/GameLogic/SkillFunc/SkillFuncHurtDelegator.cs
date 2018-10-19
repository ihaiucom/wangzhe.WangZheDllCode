using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using ResData;
using System;

namespace Assets.Scripts.GameLogic.SkillFunc
{
	[SkillFuncHandlerClass]
	public class SkillFuncHurtDelegator
	{
		private static PoolObjHandle<ActorRoot> _originateActor;

		private static PoolObjHandle<ActorRoot> _targetActor;

		private static int GetEffectFadeRate(ref SSkillFuncContext inContext)
		{
			int iNextDeltaFadeRate = inContext.inBuffSkill.handle.cfgData.iNextDeltaFadeRate;
			int iNextLowFadeRate = inContext.inBuffSkill.handle.cfgData.iNextLowFadeRate;
			int num = 10000 - (inContext.inEffectCount - 1) * iNextDeltaFadeRate;
			return (num >= iNextLowFadeRate) ? num : iNextLowFadeRate;
		}

		private static int GetOverlayFadeRate(ref SSkillFuncContext inContext)
		{
			PoolObjHandle<ActorRoot> inTargetObj = inContext.inTargetObj;
			int num = 10000 - inContext.inBuffSkill.handle.cfgData.iOverlayFadeRate;
			if (num == 10000)
			{
				return 10000;
			}
			if (num < 10000 && num >= 0 && inTargetObj && inTargetObj.handle.BuffHolderComp != null)
			{
				int skillID = inContext.inBuffSkill.handle.SkillID;
				if (inContext.inBuffSkill && inContext.inBuffSkill.handle.bFirstEffect)
				{
					inContext.inBuffSkill.handle.bFirstEffect = false;
					return 10000;
				}
				int num2 = inTargetObj.handle.BuffHolderComp.FindBuffCount(skillID);
				if (num2 > 1)
				{
					return num;
				}
			}
			return 10000;
		}

		private static bool HandleSkillFuncHurt(ref SSkillFuncContext inContext, HurtTypeDef hurtType)
		{
			int num = 0;
			if (inContext.inStage == ESkillFuncStage.Enter)
			{
				PoolObjHandle<ActorRoot> inTargetObj = inContext.inTargetObj;
				PoolObjHandle<ActorRoot> inOriginator = inContext.inOriginator;
				if (inTargetObj && !inTargetObj.handle.ActorControl.IsDeadState)
				{
					inContext.inCustomData = default(HurtAttackerInfo);
					inContext.inCustomData.Init(inOriginator, inTargetObj);
					HurtDataInfo hurtDataInfo;
					hurtDataInfo.atker = inOriginator;
					hurtDataInfo.target = inTargetObj;
					hurtDataInfo.attackInfo = inContext.inCustomData;
					hurtDataInfo.atkSlot = inContext.inUseContext.SlotType;
					hurtDataInfo.hurtType = hurtType;
					hurtDataInfo.extraHurtType = (ExtraHurtTypeDef)inContext.GetSkillFuncParam(0, false);
					hurtDataInfo.hurtValue = inContext.GetSkillFuncParam(1, true);
					hurtDataInfo.adValue = inContext.GetSkillFuncParam(2, true);
					hurtDataInfo.apValue = inContext.GetSkillFuncParam(3, true);
					hurtDataInfo.hpValue = inContext.GetSkillFuncParam(4, true);
					hurtDataInfo.loseHpValue = inContext.GetSkillFuncParam(5, true);
					hurtDataInfo.iConditionType = inContext.GetSkillFuncParam(6, false);
					hurtDataInfo.iConditionParam = inContext.GetSkillFuncParam(7, true);
					hurtDataInfo.hurtCount = inContext.inDoCount;
					hurtDataInfo.firstHemoFadeRate = inContext.inBuffSkill.handle.cfgData.iFirstLifeStealAttenuation;
					hurtDataInfo.followUpHemoFadeRate = inContext.inBuffSkill.handle.cfgData.iFollowUpLifeStealAttenuation;
					hurtDataInfo.iEffectCountInSingleTrigger = inContext.inEffectCountInSingleTrigger;
					hurtDataInfo.bExtraBuff = inContext.inBuffSkill.handle.bExtraBuff;
					hurtDataInfo.gatherTime = inContext.inUseContext.GatherTime;
					bool bBounceHurt = inContext.inBuffSkill.handle.cfgData.bEffectSubType == 9;
					hurtDataInfo.bBounceHurt = bBounceHurt;
					hurtDataInfo.bLastHurt = inContext.inLastEffect;
					hurtDataInfo.iAddTotalHurtValueRate = 0;
					hurtDataInfo.iAddTotalHurtValue = 0;
					hurtDataInfo.iCanSkillCrit = (int)inContext.inBuffSkill.handle.cfgData.bCanSkillCrit;
					hurtDataInfo.iDamageLimit = inContext.inBuffSkill.handle.cfgData.iDamageLimit;
					hurtDataInfo.iMonsterDamageLimit = inContext.inBuffSkill.handle.cfgData.iMonsterDamageLimit;
					hurtDataInfo.iLongRangeReduction = inContext.inBuffSkill.handle.cfgData.iLongRangeReduction;
					hurtDataInfo.iEffectiveTargetType = (int)inContext.inBuffSkill.handle.cfgData.bEffectiveTargetType;
					hurtDataInfo.iOverlayFadeRate = SkillFuncHurtDelegator.GetOverlayFadeRate(ref inContext);
					hurtDataInfo.iEffectFadeRate = SkillFuncHurtDelegator.GetEffectFadeRate(ref inContext);
					hurtDataInfo.iReduceDamage = 0;
					hurtDataInfo.SkillUseFrom = inContext.inUseContext.skillUseFrom;
					hurtDataInfo.uiFromId = inContext.inUseContext.uiFromId;
					hurtDataInfo.ExtraEffectSlotType = (SkillSlotType)((inContext.inBuffSkill.handle.cfgData.iExtraEffectSlotType != -1) ? inContext.inBuffSkill.handle.cfgData.iExtraEffectSlotType : ((int)hurtDataInfo.atkSlot));
					num = inTargetObj.handle.ActorControl.TakeDamage(ref hurtDataInfo);
					inContext.inAction.handle.refParams.AddRefParam("HurtValue", -num);
				}
			}
			else if (inContext.inStage == ESkillFuncStage.Update)
			{
				PoolObjHandle<ActorRoot> inTargetObj2 = inContext.inTargetObj;
				PoolObjHandle<ActorRoot> inOriginator2 = inContext.inOriginator;
				if (inTargetObj2 && !inTargetObj2.handle.ActorControl.IsDeadState)
				{
					HurtDataInfo hurtDataInfo2;
					hurtDataInfo2.atker = inOriginator2;
					hurtDataInfo2.target = inTargetObj2;
					hurtDataInfo2.attackInfo = inContext.inCustomData;
					hurtDataInfo2.atkSlot = inContext.inUseContext.SlotType;
					hurtDataInfo2.hurtType = hurtType;
					hurtDataInfo2.extraHurtType = (ExtraHurtTypeDef)inContext.GetSkillFuncParam(0, false);
					hurtDataInfo2.hurtValue = inContext.GetSkillFuncParam(1, true);
					hurtDataInfo2.adValue = inContext.GetSkillFuncParam(2, true);
					hurtDataInfo2.apValue = inContext.GetSkillFuncParam(3, true);
					hurtDataInfo2.hpValue = inContext.GetSkillFuncParam(4, true);
					hurtDataInfo2.loseHpValue = inContext.GetSkillFuncParam(5, true);
					hurtDataInfo2.iConditionType = inContext.GetSkillFuncParam(6, false);
					hurtDataInfo2.iConditionParam = inContext.GetSkillFuncParam(7, true);
					hurtDataInfo2.hurtCount = inContext.inDoCount;
					hurtDataInfo2.firstHemoFadeRate = inContext.inBuffSkill.handle.cfgData.iFirstLifeStealAttenuation;
					hurtDataInfo2.followUpHemoFadeRate = inContext.inBuffSkill.handle.cfgData.iFollowUpLifeStealAttenuation;
					hurtDataInfo2.iEffectCountInSingleTrigger = inContext.inEffectCountInSingleTrigger;
					hurtDataInfo2.bExtraBuff = inContext.inBuffSkill.handle.bExtraBuff;
					hurtDataInfo2.gatherTime = inContext.inUseContext.GatherTime;
					bool bBounceHurt2 = inContext.inBuffSkill.handle.cfgData.bEffectSubType == 9;
					hurtDataInfo2.bBounceHurt = bBounceHurt2;
					hurtDataInfo2.bLastHurt = inContext.inLastEffect;
					hurtDataInfo2.iAddTotalHurtValueRate = 0;
					hurtDataInfo2.iAddTotalHurtValue = 0;
					hurtDataInfo2.iCanSkillCrit = (int)inContext.inBuffSkill.handle.cfgData.bCanSkillCrit;
					hurtDataInfo2.iDamageLimit = inContext.inBuffSkill.handle.cfgData.iDamageLimit;
					hurtDataInfo2.iMonsterDamageLimit = inContext.inBuffSkill.handle.cfgData.iMonsterDamageLimit;
					hurtDataInfo2.iLongRangeReduction = inContext.inBuffSkill.handle.cfgData.iLongRangeReduction;
					hurtDataInfo2.iEffectiveTargetType = (int)inContext.inBuffSkill.handle.cfgData.bEffectiveTargetType;
					hurtDataInfo2.iOverlayFadeRate = SkillFuncHurtDelegator.GetOverlayFadeRate(ref inContext);
					hurtDataInfo2.iEffectFadeRate = SkillFuncHurtDelegator.GetEffectFadeRate(ref inContext);
					hurtDataInfo2.iReduceDamage = 0;
					hurtDataInfo2.SkillUseFrom = inContext.inUseContext.skillUseFrom;
					hurtDataInfo2.uiFromId = inContext.inUseContext.uiFromId;
					hurtDataInfo2.ExtraEffectSlotType = (SkillSlotType)((inContext.inBuffSkill.handle.cfgData.iExtraEffectSlotType != -1) ? inContext.inBuffSkill.handle.cfgData.iExtraEffectSlotType : ((int)hurtDataInfo2.atkSlot));
					num = inTargetObj2.handle.ActorControl.TakeDamage(ref hurtDataInfo2);
					inContext.inAction.handle.refParams.AddRefParam("HurtValue", -num);
				}
			}
			return num != 0;
		}

		[SkillFuncHandler(0, new int[]
		{

		})]
		public static bool OnSkillFuncPhysHurt(ref SSkillFuncContext inContext)
		{
			return inContext.inStage != ESkillFuncStage.Leave && SkillFuncHurtDelegator.HandleSkillFuncHurt(ref inContext, HurtTypeDef.PhysHurt);
		}

		[SkillFuncHandler(1, new int[]
		{

		})]
		public static bool OnSkillFuncMagicHurt(ref SSkillFuncContext inContext)
		{
			return inContext.inStage != ESkillFuncStage.Leave && SkillFuncHurtDelegator.HandleSkillFuncHurt(ref inContext, HurtTypeDef.MagicHurt);
		}

		[SkillFuncHandler(2, new int[]
		{

		})]
		public static bool OnSkillFuncRealHurt(ref SSkillFuncContext inContext)
		{
			return inContext.inStage != ESkillFuncStage.Leave && SkillFuncHurtDelegator.HandleSkillFuncHurt(ref inContext, HurtTypeDef.RealHurt);
		}

		[SkillFuncHandler(3, new int[]
		{

		})]
		public static bool OnSkillFuncAddHp(ref SSkillFuncContext inContext)
		{
			return inContext.inStage != ESkillFuncStage.Leave && SkillFuncHurtDelegator.HandleSkillFuncHurt(ref inContext, HurtTypeDef.Therapic);
		}

		private static int GetSkillFuncProtectValue(ref SSkillFuncContext inContext)
		{
			int result = 0;
			if (inContext.inStage == ESkillFuncStage.Enter)
			{
				PoolObjHandle<ActorRoot> inTargetObj = inContext.inTargetObj;
				PoolObjHandle<ActorRoot> inOriginator = inContext.inOriginator;
				if (inTargetObj && inOriginator)
				{
					inContext.inCustomData = default(HurtAttackerInfo);
					inContext.inCustomData.Init(inOriginator, inTargetObj);
					HurtDataInfo hurtDataInfo;
					hurtDataInfo.atker = inOriginator;
					hurtDataInfo.target = inTargetObj;
					hurtDataInfo.attackInfo = inContext.inCustomData;
					hurtDataInfo.atkSlot = inContext.inUseContext.SlotType;
					hurtDataInfo.hurtType = HurtTypeDef.PhysHurt;
					hurtDataInfo.extraHurtType = ExtraHurtTypeDef.ExtraHurt_Value;
					hurtDataInfo.hurtValue = inContext.GetSkillFuncParam(1, true);
					hurtDataInfo.adValue = inContext.GetSkillFuncParam(2, true);
					hurtDataInfo.apValue = inContext.GetSkillFuncParam(3, true);
					hurtDataInfo.hpValue = inContext.GetSkillFuncParam(4, true);
					hurtDataInfo.iConditionType = inContext.GetSkillFuncParam(6, false);
					hurtDataInfo.iConditionParam = inContext.GetSkillFuncParam(7, true);
					hurtDataInfo.loseHpValue = 0;
					hurtDataInfo.hurtCount = inContext.inDoCount;
					hurtDataInfo.firstHemoFadeRate = 10000;
					hurtDataInfo.followUpHemoFadeRate = 10000;
					hurtDataInfo.iEffectCountInSingleTrigger = 1;
					hurtDataInfo.bExtraBuff = false;
					hurtDataInfo.gatherTime = inContext.inUseContext.GatherTime;
					hurtDataInfo.bBounceHurt = false;
					hurtDataInfo.bLastHurt = inContext.inLastEffect;
					hurtDataInfo.iAddTotalHurtValueRate = 0;
					hurtDataInfo.iAddTotalHurtValue = 0;
					hurtDataInfo.iCanSkillCrit = (int)inContext.inBuffSkill.handle.cfgData.bCanSkillCrit;
					hurtDataInfo.iDamageLimit = inContext.inBuffSkill.handle.cfgData.iDamageLimit;
					hurtDataInfo.iMonsterDamageLimit = inContext.inBuffSkill.handle.cfgData.iMonsterDamageLimit;
					hurtDataInfo.iLongRangeReduction = inContext.inBuffSkill.handle.cfgData.iLongRangeReduction;
					hurtDataInfo.iEffectiveTargetType = (int)inContext.inBuffSkill.handle.cfgData.bEffectiveTargetType;
					hurtDataInfo.iOverlayFadeRate = 10000;
					hurtDataInfo.iEffectFadeRate = 10000;
					hurtDataInfo.iReduceDamage = 0;
					hurtDataInfo.SkillUseFrom = inContext.inUseContext.skillUseFrom;
					hurtDataInfo.uiFromId = inContext.inUseContext.uiFromId;
					hurtDataInfo.ExtraEffectSlotType = (SkillSlotType)((inContext.inBuffSkill.handle.cfgData.iExtraEffectSlotType != -1) ? inContext.inBuffSkill.handle.cfgData.iExtraEffectSlotType : ((int)hurtDataInfo.atkSlot));
					result = inTargetObj.handle.ActorControl.actor.HurtControl.CommonDamagePart(ref hurtDataInfo);
				}
			}
			return result;
		}

		private static void SendProtectEvent(ref SSkillFuncContext inContext, int type, int changeValue)
		{
			if (changeValue != 0 && inContext.inTargetObj && inContext.inTargetObj.handle.BuffHolderComp != null)
			{
				BuffProtectRule protectRule = inContext.inTargetObj.handle.BuffHolderComp.protectRule;
				protectRule.SendProtectEvent(type, changeValue);
			}
		}

		[SkillFuncHandler(27, new int[]
		{

		})]
		public static bool OnSkillFuncProtect(ref SSkillFuncContext inContext)
		{
			if (inContext.inStage == ESkillFuncStage.Enter)
			{
				int skillFuncProtectValue = SkillFuncHurtDelegator.GetSkillFuncProtectValue(ref inContext);
				int skillFuncParam = inContext.GetSkillFuncParam(0, false);
				int skillFuncParam2 = inContext.GetSkillFuncParam(5, false);
				int skillFuncParam3 = inContext.GetSkillFuncParam(6, false);
				if (skillFuncParam == 1)
				{
					inContext.inBuffSkill.handle.CustomParams[0] += skillFuncProtectValue;
					SkillFuncHurtDelegator.SendProtectEvent(ref inContext, 0, skillFuncProtectValue);
				}
				else if (skillFuncParam == 2)
				{
					inContext.inBuffSkill.handle.CustomParams[1] += skillFuncProtectValue;
					SkillFuncHurtDelegator.SendProtectEvent(ref inContext, 1, skillFuncProtectValue);
				}
				else if (skillFuncParam == 3)
				{
					inContext.inBuffSkill.handle.CustomParams[2] += skillFuncProtectValue;
					SkillFuncHurtDelegator.SendProtectEvent(ref inContext, 2, skillFuncProtectValue);
				}
				else if (skillFuncParam == 4)
				{
					inContext.inBuffSkill.handle.CustomParams[3] += skillFuncProtectValue;
					SkillFuncHurtDelegator.SendProtectEvent(ref inContext, 3, skillFuncProtectValue);
				}
				inContext.inBuffSkill.handle.CustomParams[4] = skillFuncParam2;
				inContext.inBuffSkill.handle.CustomParams[5] = skillFuncParam3;
				inContext.inBuffSkill.handle.SlotType = inContext.inUseContext.SlotType;
				return true;
			}
			return false;
		}

		[SkillFuncHandler(33, new int[]
		{

		})]
		public static bool OnSkillFuncExtraEffect(ref SSkillFuncContext inContext)
		{
			return true;
		}

		[SkillFuncHandler(10, new int[]
		{

		})]
		public static bool OnSkillFuncSuckBlood(ref SSkillFuncContext inContext)
		{
			PoolObjHandle<ActorRoot> inOriginator = inContext.inOriginator;
			if (!inOriginator)
			{
				return false;
			}
			ResDT_SkillFunc skillFunc = null;
			if (inContext.inStage != ESkillFuncStage.Leave)
			{
				int num = 0;
				int iParam = inContext.inSkillFunc.astSkillFuncParam[0].iParam;
				int num2 = 0;
				if (inContext.inAction.handle.refParams.GetRefParam("HurtValue", ref num))
				{
					for (int i = 0; i < inOriginator.handle.BuffHolderComp.SpawnedBuffList.Count; i++)
					{
						BuffSkill buffSkill = inOriginator.handle.BuffHolderComp.SpawnedBuffList[i];
						if (buffSkill != null && buffSkill.FindSkillFunc(64, out skillFunc))
						{
							int skillFuncParam = buffSkill.GetSkillFuncParam(skillFunc, 0, false);
							if (skillFuncParam == 1)
							{
								num2 += buffSkill.GetSkillFuncParam(skillFunc, 4, false);
							}
						}
					}
					int num3 = (int)((long)num * (long)iParam / 10000L * (long)(10000 + num2) / 10000L);
					if (num3 > 0)
					{
						int num4 = inOriginator.handle.ValueComponent.actorHp;
						inOriginator.handle.ActorControl.ReviveHp(num3);
						num4 = inOriginator.handle.ValueComponent.actorHp - num4;
						HemophagiaEventResultInfo hemophagiaEventResultInfo = new HemophagiaEventResultInfo(inOriginator, num4);
						Singleton<GameEventSys>.instance.SendEvent<HemophagiaEventResultInfo>(GameEventDef.Event_Hemophagia, ref hemophagiaEventResultInfo);
					}
				}
			}
			return true;
		}

		[SkillFuncHandler(44, new int[]
		{

		})]
		public static bool OnSkillFuncConditionHurtOut(ref SSkillFuncContext inContext)
		{
			return true;
		}

		[SkillFuncHandler(48, new int[]
		{

		})]
		public static bool OnSkillFuncTargetExtraHurt(ref SSkillFuncContext inContext)
		{
			return true;
		}

		[SkillFuncHandler(49, new int[]
		{

		})]
		public static bool OnSkillFuncTargetExtraExp(ref SSkillFuncContext inContext)
		{
			return true;
		}

		[SkillFuncHandler(50, new int[]
		{

		})]
		public static bool OnSkillFuncAddExp(ref SSkillFuncContext inContext)
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
					inTargetObj.handle.ValueComponent.AddSoulExp(num, true, AddSoulType.SkillFunc);
				}
			}
			return true;
		}

		[SkillFuncHandler(52, new int[]
		{

		})]
		public static bool OnSkillFuncImmuneCrit(ref SSkillFuncContext inContext)
		{
			PoolObjHandle<ActorRoot> inTargetObj = inContext.inTargetObj;
			if (inTargetObj)
			{
				if (inContext.inStage == ESkillFuncStage.Enter)
				{
					inTargetObj.handle.ActorControl.AddNoAbilityFlag(ObjAbilityType.ObjAbility_ImmuneCrit);
				}
				else if (inContext.inStage == ESkillFuncStage.Leave)
				{
					inTargetObj.handle.ActorControl.RmvNoAbilityFlag(ObjAbilityType.ObjAbility_ImmuneCrit);
				}
			}
			return true;
		}

		[SkillFuncHandler(53, new int[]
		{

		})]
		public static bool OnSkillFuncLimiteMaxHurt(ref SSkillFuncContext inContext)
		{
			PoolObjHandle<ActorRoot> inTargetObj = inContext.inTargetObj;
			if (inTargetObj)
			{
				if (inContext.inStage == ESkillFuncStage.Enter)
				{
					int skillFuncParam = inContext.GetSkillFuncParam(0, false);
					inTargetObj.handle.BuffHolderComp.protectRule.SetLimiteMaxHurt(true, skillFuncParam);
				}
				else if (inContext.inStage == ESkillFuncStage.Leave)
				{
					inTargetObj.handle.BuffHolderComp.protectRule.SetLimiteMaxHurt(false, 0);
				}
			}
			return true;
		}

		[SkillFuncHandler(61, new int[]
		{

		})]
		public static bool OnSkillFuncCommonAtkWithMagicHurt(ref SSkillFuncContext inContext)
		{
			return true;
		}

		[SkillFuncHandler(67, new int[]
		{

		})]
		public static bool OnSkillFuncDecHurtRate(ref SSkillFuncContext inContext)
		{
			return true;
		}

		[SkillFuncHandler(68, new int[]
		{

		})]
		public static bool OnSkillFuncExtraHurtWithLowHp(ref SSkillFuncContext inContext)
		{
			return true;
		}

		[SkillFuncHandler(69, new int[]
		{

		})]
		public static bool OnSkillFuncBlindess(ref SSkillFuncContext inContext)
		{
			PoolObjHandle<ActorRoot> inTargetObj = inContext.inTargetObj;
			if (inTargetObj)
			{
				if (inContext.inStage == ESkillFuncStage.Enter)
				{
					inTargetObj.handle.ActorControl.AddNoAbilityFlag(ObjAbilityType.ObjAbility_Blindness);
				}
				else if (inContext.inStage == ESkillFuncStage.Leave)
				{
					inTargetObj.handle.ActorControl.RmvNoAbilityFlag(ObjAbilityType.ObjAbility_Blindness);
				}
			}
			return true;
		}

		[SkillFuncHandler(71, new int[]
		{

		})]
		public static bool OnSkillFuncTargetExtraCoin(ref SSkillFuncContext inContext)
		{
			return true;
		}

		[SkillFuncHandler(80, new int[]
		{

		})]
		public static bool OnSkillFuncBlockPhysHurt(ref SSkillFuncContext inContext)
		{
			return true;
		}

		[SkillFuncHandler(82, new int[]
		{

		})]
		public static bool OnSkillFuncSuckBloodSpecialSkill(ref SSkillFuncContext inContext)
		{
			PoolObjHandle<ActorRoot> inOriginator = inContext.inOriginator;
			if (!inOriginator)
			{
				return false;
			}
			if (inContext.inStage != ESkillFuncStage.Leave)
			{
				int num = 0;
				if ((inContext.inSkillFunc.astSkillFuncParam[1].iParam & 1 << (int)inContext.inUseContext.SlotType) == 0)
				{
					return false;
				}
				if (inContext.inAction.handle.refParams.GetRefParam("HurtValue", ref num))
				{
					int nAddHp = num * inContext.inSkillFunc.astSkillFuncParam[0].iParam / 10000;
					inOriginator.handle.ActorControl.ReviveHp(nAddHp);
				}
			}
			return true;
		}

		[SkillFuncHandler(83, new int[]
		{

		})]
		public static bool OnSkillFuncBounceHurt(ref SSkillFuncContext inContext)
		{
			return true;
		}

		[SkillFuncHandler(88, new int[]
		{

		})]
		public static bool OnSkillFuncImmuneControlAndShareHurt(ref SSkillFuncContext inContext)
		{
			SkillFuncHurtDelegator._originateActor = inContext.inOriginator;
			SkillFuncHurtDelegator._targetActor = inContext.inTargetObj;
			if (SkillFuncHurtDelegator._originateActor && SkillFuncHurtDelegator._targetActor)
			{
				if (inContext.inStage == ESkillFuncStage.Enter)
				{
					SkillFuncHurtDelegator._originateActor.handle.ActorControl.AddNoAbilityFlag(ObjAbilityType.ObjAbility_ImmuneControl);
					SkillFuncHurtDelegator._targetActor.handle.ActorControl.AddNoAbilityFlag(ObjAbilityType.ObjAbility_ImmuneControl);
					Singleton<GameEventSys>.GetInstance().AddEventHandler<HurtEventResultInfo>(GameEventDef.Event_ActorDamage, new RefAction<HurtEventResultInfo>(SkillFuncHurtDelegator.OnActorDamage));
				}
				else if (inContext.inStage == ESkillFuncStage.Leave)
				{
					SkillFuncHurtDelegator._originateActor.handle.ActorControl.RmvNoAbilityFlag(ObjAbilityType.ObjAbility_ImmuneControl);
					SkillFuncHurtDelegator._targetActor.handle.ActorControl.RmvNoAbilityFlag(ObjAbilityType.ObjAbility_ImmuneControl);
					Singleton<GameEventSys>.GetInstance().RmvEventHandler<HurtEventResultInfo>(GameEventDef.Event_ActorDamage, new RefAction<HurtEventResultInfo>(SkillFuncHurtDelegator.OnActorDamage));
				}
				return true;
			}
			return false;
		}

		private static void OnActorDamage(ref HurtEventResultInfo info)
		{
			if (!SkillFuncHurtDelegator._originateActor || !SkillFuncHurtDelegator._targetActor || !info.src || info.src.handle.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Hero || info.hpChanged >= 0)
			{
				return;
			}
			if (info.src == SkillFuncHurtDelegator._originateActor)
			{
				if (!SkillFuncHurtDelegator._targetActor.handle.ActorControl.IsDeadState)
				{
					SkillFuncHurtDelegator._targetActor.handle.ValueComponent.actorHp += info.hpChanged;
				}
			}
			else if (info.src == SkillFuncHurtDelegator._targetActor && !SkillFuncHurtDelegator._originateActor.handle.ActorControl.IsDeadState)
			{
				SkillFuncHurtDelegator._originateActor.handle.ValueComponent.actorHp += info.hpChanged;
			}
		}
	}
}
