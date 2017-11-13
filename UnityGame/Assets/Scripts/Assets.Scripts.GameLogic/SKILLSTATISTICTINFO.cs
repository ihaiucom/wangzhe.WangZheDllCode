using System;

namespace Assets.Scripts.GameLogic
{
	public class SKILLSTATISTICTINFO
	{
		public int iSkillCfgID;

		public uint uiUsedTimes;

		public uint uiCDIntervalMin;

		public uint uiCDIntervalMax;

		public float fLastUseTime;

		public int iAttackDistanceMax;

		public int iHurtMax;

		public int iHurtMin;

		public int iHurtTotal;

		public int iHurtToHeroTotal;

		public int ihurtValue;

		public int iadValue;

		public int iapValue;

		public int ihpValue;

		public int iloseHpValue;

		public int ihurtCount;

		public int ihemoFadeRate;

		public int iHitCountMax;

		public int iHitCountMin;

		public int iHitCount;

		public int iHitAllHurtTotalMax;

		public int iHitAllHurtTotalMin;

		public int iTmpHitAllHurtTotal;

		public int iTmpHitAllHurtCountIndex;

		public int iUseSkillHitHeroTimes;

		public bool bIsCurUseSkillHitHero;

		public int iHitHeroCount;

		public SKILLSTATISTICTINFO(int i)
		{
			this.iSkillCfgID = 0;
			this.uiUsedTimes = 0u;
			this.uiCDIntervalMin = 4294967295u;
			this.uiCDIntervalMax = 0u;
			this.fLastUseTime = 0f;
			this.iAttackDistanceMax = 0;
			this.iHurtMax = 0;
			this.iHurtMin = -1;
			this.iHurtTotal = 0;
			this.iHurtToHeroTotal = 0;
			this.ihurtValue = 0;
			this.iadValue = 0;
			this.iapValue = 0;
			this.ihpValue = 0;
			this.iloseHpValue = 0;
			this.ihurtCount = 0;
			this.ihemoFadeRate = 0;
			this.iHitCountMax = 0;
			this.iHitCountMin = -1;
			this.iHitCount = 0;
			this.iHitAllHurtTotalMax = 0;
			this.iHitAllHurtTotalMin = -1;
			this.iTmpHitAllHurtTotal = 0;
			this.iTmpHitAllHurtCountIndex = 0;
			this.iUseSkillHitHeroTimes = 0;
			this.bIsCurUseSkillHitHero = false;
		}
	}
}
