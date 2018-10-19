using Assets.Scripts.Framework;
using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	public class HurtComponent : LogicComponent
	{
		private ResBattleParam battleParam;

		public override void OnUse()
		{
			base.OnUse();
			this.battleParam = null;
		}

		public override void Init()
		{
			base.Init();
			this.battleParam = GameDataMgr.battleParam.GetAnyData();
		}

		private int GetExtraHurtValue(ref HurtDataInfo hurt)
		{
			int totalValue = hurt.target.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP].totalValue;
			int actorHp = hurt.target.handle.ValueComponent.actorHp;
			return hurt.loseHpValue * (totalValue - actorHp) / 10000;
		}

		private int GetBaseHurtValue(ref HurtDataInfo hurt)
		{
			int result = 1;
			int totalValue = hurt.target.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP].totalValue;
			int actorHp = hurt.target.handle.ValueComponent.actorHp;
			switch (hurt.extraHurtType)
			{
			case ExtraHurtTypeDef.ExtraHurt_Value:
				result = 1;
				break;
			case ExtraHurtTypeDef.ExtraHurt_MaxHp:
				result = totalValue;
				break;
			case ExtraHurtTypeDef.ExtraHurt_CurHp:
				result = totalValue - actorHp;
				break;
			case ExtraHurtTypeDef.ExtraHurt_LoseHp:
				result = actorHp;
				break;
			}
			return result;
		}

		private int GetBaseHurtRate(ref HurtDataInfo hurt)
		{
			int result = 1;
			if (hurt.extraHurtType != ExtraHurtTypeDef.ExtraHurt_Value)
			{
				result = 100;
			}
			return result;
		}

		private int GetAttackerATT(ref HurtDataInfo hurt)
		{
			int result = hurt.attackInfo.iActorATT;
			if (hurt.target && hurt.atker && hurt.target.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ && hurt.atker.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero && hurt.atker.handle.ValueComponent != null)
			{
				int basePropertyValue = hurt.atker.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYATKPT].basePropertyValue;
				int dwConfValue = (int)GameDataMgr.globalInfoDatabin.GetDataByKey(348u).dwConfValue;
				result = Math.Max(hurt.attackInfo.iActorATT, basePropertyValue + hurt.attackInfo.iActorINT * dwConfValue / 10000);
			}
			return result;
		}

		public int CommonDamagePart(ref HurtDataInfo hurt)
		{
			int hurtValue = hurt.hurtValue;
			int num = (hurtValue + hurt.adValue * this.GetAttackerATT(ref hurt) / 10000 + hurt.apValue * hurt.attackInfo.iActorINT / 10000 + hurt.hpValue * hurt.attackInfo.iActorMaxHp / 10000 + this.GetExtraHurtValue(ref hurt)) * this.GetBaseHurtValue(ref hurt) / this.GetBaseHurtRate(ref hurt);
			if (hurt.atkSlot == SkillSlotType.SLOT_SKILL_0 && hurt.atker)
			{
				num += hurt.atker.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_BASEHURTADD].totalValue;
			}
			return num;
		}

		private int ReduceDamagePart(ref HurtDataInfo hurt, HurtTypeDef hurtType)
		{
			int result = 0;
			if (hurtType == HurtTypeDef.PhysHurt)
			{
				int num = this.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_PHYDEFPT].totalValue - hurt.attackInfo.iDEFStrike;
				num = num * (10000 - hurt.attackInfo.iDEFStrikeRate) / 10000;
				num = ((num <= 0) ? 0 : num);
				int num2 = num + (int)(this.battleParam.dwM_PhysicsDefend * (uint)this.actor.ValueComponent.mActorValue.actorLvl) + (int)this.battleParam.dwN_PhysicsDefend;
				if (num2 != 0)
				{
					result = num * 10000 / num2;
				}
			}
			else if (hurtType == HurtTypeDef.MagicHurt)
			{
				int num3 = this.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MGCDEFPT].totalValue - hurt.attackInfo.iRESStrike;
				num3 = num3 * (10000 - hurt.attackInfo.iRESStrikeRate) / 10000;
				num3 = ((num3 <= 0) ? 0 : num3);
				int num4 = num3 + (int)(this.battleParam.dwM_MagicDefend * (uint)this.actor.ValueComponent.mActorValue.actorLvl) + (int)this.battleParam.dwN_MagicDefend;
				if (num4 != 0)
				{
					result = num3 * 10000 / num4;
				}
			}
			return result;
		}

		private int CriticalDamagePart(ref HurtDataInfo hurt)
		{
			bool flag = false;
			int result = 0;
			if (hurt.iCanSkillCrit == 0)
			{
				return result;
			}
			int num = hurt.attackInfo.iCritStrikeValue + hurt.attackInfo.iActorLvl * (int)this.battleParam.dwM_Critical + (int)this.battleParam.dwN_Critical;
			int num2 = 0;
			int num3 = 0;
			if (num > 0)
			{
				num2 = hurt.attackInfo.iCritStrikeValue * 10000 / num + hurt.attackInfo.iCritStrikeRate;
			}
			int num4 = hurt.attackInfo.iReduceCritStrikeValue + this.actor.ValueComponent.mActorValue.actorLvl * (int)this.battleParam.dwM_ReduceCritical + (int)this.battleParam.dwN_ReduceCritical;
			if (num4 > 0)
			{
				num3 = hurt.attackInfo.iReduceCritStrikeValue * 10000 / num4;
				num3 += hurt.attackInfo.iReduceCritStrikeRate;
			}
			if (!this.actor.ActorControl.GetNoAbilityFlag(ObjAbilityType.ObjAbility_ImmuneCrit))
			{
				flag = ((int)FrameRandom.Random(10000u) < num2 - num3);
			}
			result = ((!flag) ? 0 : hurt.attackInfo.iCritStrikeEff);
			if (flag)
			{
				DefaultGameEventParam defaultGameEventParam = new DefaultGameEventParam(hurt.atker, hurt.target);
				Singleton<GameEventSys>.instance.SendEvent<DefaultGameEventParam>(GameEventDef.Event_ActorCrit, ref defaultGameEventParam);
			}
			return result;
		}

		private int Hemophagia(ref HurtDataInfo hurt, int hurtValue)
		{
			int num = 0;
			if (hurt.atker)
			{
				if (hurt.hurtType == HurtTypeDef.PhysHurt)
				{
					int num2 = 0;
					int num3 = hurt.attackInfo.iPhysicsHemophagia + hurt.attackInfo.iActorLvl * (int)this.battleParam.dwM_PhysicsHemophagia + (int)this.battleParam.dwN_PhysicsHemophagia;
					if (num3 > 0)
					{
						num2 = hurt.attackInfo.iPhysicsHemophagia * 10000 / num3;
					}
					num = hurtValue * (num2 + hurt.attackInfo.iPhysicsHemophagiaRate) / 10000;
				}
				else if (hurt.hurtType == HurtTypeDef.MagicHurt)
				{
					int num4 = 0;
					int num5 = hurt.attackInfo.iMagicHemophagia + hurt.attackInfo.iActorLvl * (int)this.battleParam.dwM_MagicHemophagia + (int)this.battleParam.dwN_MagicHemophagia;
					if (num5 > 0)
					{
						num4 = hurt.attackInfo.iMagicHemophagia * 10000 / num5;
					}
					num = hurtValue * (num4 + hurt.attackInfo.iMagicHemophagiaRate) / 10000;
				}
				if (hurt.iEffectCountInSingleTrigger > 1)
				{
					num = num * hurt.followUpHemoFadeRate / 10000;
				}
				else
				{
					num = num * hurt.firstHemoFadeRate / 10000;
				}
				if (num > 0)
				{
					int num6 = hurt.atker.handle.ValueComponent.actorHp;
					hurt.atker.handle.ActorControl.ReviveHp(num);
					num6 = hurt.atker.handle.ValueComponent.actorHp - num6;
					HemophagiaEventResultInfo hemophagiaEventResultInfo = new HemophagiaEventResultInfo(hurt.atker, num6);
					Singleton<GameEventSys>.instance.SendEvent<HemophagiaEventResultInfo>(GameEventDef.Event_Hemophagia, ref hemophagiaEventResultInfo);
				}
				return num;
			}
			return num;
		}

		public bool ImmuneDamage(ref HurtDataInfo hurt)
		{
			if (hurt.atkSlot == SkillSlotType.SLOT_SKILL_0 && hurt.atker && hurt.atker.handle.ActorControl != null && hurt.atker.handle.ActorControl.GetNoAbilityFlag(ObjAbilityType.ObjAbility_Blindness))
			{
				ActorSkillEventParam actorSkillEventParam = new ActorSkillEventParam(hurt.target, SkillSlotType.SLOT_SKILL_0);
				Singleton<GameSkillEventSys>.GetInstance().SendEvent<ActorSkillEventParam>(GameSkillEventDef.AllEvent_Blindess, hurt.target, ref actorSkillEventParam, GameSkillEventChannel.Channel_AllActor);
				return true;
			}
			return hurt.target && hurt.target.handle.ActorControl.GetNoAbilityFlag(ObjAbilityType.ObjAbility_ImmuneNegative);
		}

		public int TakeDamage(ref HurtDataInfo hurt)
		{
			int num = this.CriticalDamagePart(ref hurt);
			int num2 = (int)(9900 + FrameRandom.Random(200u));
			this.actor.BuffHolderComp.OnDamageExtraValueEffect(ref hurt, hurt.atker, hurt.atkSlot);
			int num3;
			int num4;
			if (hurt.hurtType == HurtTypeDef.Therapic)
			{
				num3 = this.CommonDamagePart(ref hurt) * num2 / 10000;
				num3 = num3 * hurt.iOverlayFadeRate / 10000;
				if (hurt.iAddTotalHurtValueRate != 0)
				{
					num3 += num3 * hurt.iAddTotalHurtValueRate / 10000;
				}
				num3 += hurt.iAddTotalHurtValue;
				if (hurt.atker && hurt.atker != this.actorPtr && Singleton<BattleLogic>.GetInstance().GetCurLvelContext().IsLuanDouPlayMode())
				{
					num3 >>= 1;
				}
				num4 = this.actor.ValueComponent.actorHp;
				this.actor.ActorControl.ReviveHp(num3);
				num4 = this.actor.ValueComponent.actorHp - num4;
				if (hurt.atker && this.actor.TheActorMeta.ActorCamp == hurt.atker.handle.TheActorMeta.ActorCamp && this.actor.ValueComponent.actorHp > 0)
				{
					this.actor.ActorControl.AddHelpSelfActor(hurt.atker);
				}
			}
			else
			{
				if (this.ImmuneDamage(ref hurt) || this.actor.BuffHolderComp.BuffImmuneDamage(ref hurt))
				{
					return 0;
				}
				bool flag = hurt.atker && hurt.atker.handle.bOneKiller;
				int actorHp = this.actor.ValueComponent.actorHp;
				num3 = this.CommonDamagePart(ref hurt) * num2 / 10000;
				num3 = this.actor.BuffHolderComp.OnHurtBounceDamage(ref hurt, num3);
				num3 = num3 * (10000 - this.ReduceDamagePart(ref hurt, hurt.hurtType)) / 10000 * (10000 + num) / 10000 + hurt.attackInfo.iFinalHurt - this.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_REALHURTLESS].totalValue;
				num3 = num3 * (10000 - this.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_PROPERTY_HURTREDUCERATE].totalValue) / 10000;
				int extraHurtOutputRate = this.actor.BuffHolderComp.GetExtraHurtOutputRate(hurt.atker);
				num3 = num3 * (10000 + hurt.attackInfo.iHurtOutputRate + extraHurtOutputRate) / 10000 * hurt.gatherTime;
				if (num3 < 0)
				{
					num3 = 0;
				}
				if (hurt.atker)
				{
					num3 = num3 * Singleton<BattleLogic>.GetInstance().clashAddition.CalcDamageAddition(hurt.atker.handle.TheStaticData.TheBaseAttribute.ClashMark, this.actor.TheStaticData.TheBaseAttribute.ClashMark) / 10000;
				}
				num3 = this.actor.BuffHolderComp.OnDamage(ref hurt, num3);
				if (hurt.atker && hurt.atker.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero && hurt.atker.handle.TheStaticData.TheHeroOnlyInfo.AttackDistanceType == 2 && hurt.iLongRangeReduction > 0)
				{
					num3 = num3 * hurt.iLongRangeReduction / 10000;
				}
				int num5 = hurt.iDamageLimit;
				if (hurt.target && hurt.target.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster)
				{
					if (num5 > 0 && hurt.iMonsterDamageLimit > 0)
					{
						num5 = ((num5 >= hurt.iMonsterDamageLimit) ? hurt.iMonsterDamageLimit : num5);
					}
					else if (hurt.iMonsterDamageLimit > 0)
					{
						num5 = hurt.iMonsterDamageLimit;
					}
				}
				if (hurt.iReduceDamage > 0)
				{
					num3 -= hurt.iReduceDamage;
					num3 = ((num3 >= 0) ? num3 : 0);
				}
				if (num5 > 0)
				{
					num3 = ((num3 >= num5) ? num5 : num3);
				}
				if (hurt.target && hurt.target.handle.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Organ)
				{
					this.Hemophagia(ref hurt, num3);
				}
				num4 = this.actor.ValueComponent.actorHp;
				this.actor.ValueComponent.actorHp -= num3;
				num4 = this.actor.ValueComponent.actorHp - num4;
				if (flag)
				{
					this.actor.ValueComponent.actorHp = 0;
					num4 = actorHp * -1;
				}
				if (hurt.atker && this.actor.TheActorMeta.ActorCamp != hurt.atker.handle.TheActorMeta.ActorCamp && this.actor.ValueComponent.actorHp > 0)
				{
					this.actor.ActorControl.AddHurtSelfActor(hurt.atker);
				}
			}
			HurtEventResultInfo hurtEventResultInfo = new HurtEventResultInfo(base.GetActor(), hurt.atker, hurt, num3, num4, num);
			Singleton<GameEventSys>.instance.SendEvent<HurtEventResultInfo>(GameEventDef.Event_ActorDamage, ref hurtEventResultInfo);
			return num4;
		}

		public int TakeBouncesDamage(ref HurtDataInfo hurt)
		{
			int num = this.CriticalDamagePart(ref hurt);
			int num2 = (int)(9900 + FrameRandom.Random(200u));
			this.actor.BuffHolderComp.OnDamageExtraValueEffect(ref hurt, hurt.atker, hurt.atkSlot);
			if (this.ImmuneDamage(ref hurt) || this.actor.BuffHolderComp.BuffImmuneDamage(ref hurt))
			{
				return 0;
			}
			bool flag = hurt.atker && hurt.atker.handle.bOneKiller;
			int actorHp = this.actor.ValueComponent.actorHp;
			int num3 = this.CommonDamagePart(ref hurt) * (10000 - this.ReduceDamagePart(ref hurt, hurt.hurtType)) / 10000 * (10000 + num) / 10000 - this.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_REALHURTLESS].totalValue;
			num3 = num3 * (10000 - this.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_PROPERTY_HURTREDUCERATE].totalValue) / 10000;
			if (num3 < 0)
			{
				num3 = 0;
			}
			if (hurt.atker)
			{
				num3 = num3 * Singleton<BattleLogic>.GetInstance().clashAddition.CalcDamageAddition(hurt.atker.handle.TheStaticData.TheBaseAttribute.ClashMark, this.actor.TheStaticData.TheBaseAttribute.ClashMark) / 10000;
			}
			num3 = this.actor.BuffHolderComp.OnDamage(ref hurt, num3);
			int num4 = this.actor.ValueComponent.actorHp;
			this.actor.ValueComponent.actorHp -= num3;
			num4 = this.actor.ValueComponent.actorHp - num4;
			if (flag)
			{
				this.actor.ValueComponent.actorHp = 0;
				num4 = actorHp * -1;
			}
			if (hurt.atker && this.actor.TheActorMeta.ActorCamp != hurt.atker.handle.TheActorMeta.ActorCamp && this.actor.ValueComponent.actorHp > 0)
			{
				this.actor.ActorControl.AddHurtSelfActor(hurt.atker);
			}
			HurtEventResultInfo hurtEventResultInfo = new HurtEventResultInfo(base.GetActor(), hurt.atker, hurt, num3, num4, num);
			Singleton<GameEventSys>.instance.SendEvent<HurtEventResultInfo>(GameEventDef.Event_ActorDamage, ref hurtEventResultInfo);
			return num4;
		}
	}
}
