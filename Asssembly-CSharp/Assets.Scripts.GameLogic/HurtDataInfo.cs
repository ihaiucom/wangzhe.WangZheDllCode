using Assets.Scripts.Common;
using System;

namespace Assets.Scripts.GameLogic
{
	public struct HurtDataInfo
	{
		public PoolObjHandle<ActorRoot> atker;

		public PoolObjHandle<ActorRoot> target;

		public HurtAttackerInfo attackInfo;

		public SkillSlotType atkSlot;

		public HurtTypeDef hurtType;

		public ExtraHurtTypeDef extraHurtType;

		public int hurtValue;

		public int adValue;

		public int apValue;

		public int hpValue;

		public int loseHpValue;

		public int hurtCount;

		public int firstHemoFadeRate;

		public int followUpHemoFadeRate;

		public int iEffectCountInSingleTrigger;

		public bool bExtraBuff;

		public int gatherTime;

		public bool bBounceHurt;

		public bool bLastHurt;

		public int iAddTotalHurtValue;

		public int iAddTotalHurtValueRate;

		public int iCanSkillCrit;

		public int iDamageLimit;

		public int iMonsterDamageLimit;

		public int iLongRangeReduction;

		public int iEffectiveTargetType;

		public int iOverlayFadeRate;

		public int iEffectFadeRate;

		public int iReduceDamage;

		public int iConditionType;

		public int iConditionParam;

		public SkillSlotType ExtraEffectSlotType;

		public SKILL_USE_FROM_TYPE SkillUseFrom;

		public uint uiFromId;
	}
}
