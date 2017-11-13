using Assets.Scripts.Framework;
using System;

namespace Assets.Scripts.GameLogic
{
	public class ActorValueStatistic
	{
		public int iActorLvl;

		public int iActorATT;

		public int iActorINT;

		public int iActorMaxHp;

		public int iActorMinHp = -1;

		public int iDEFStrike;

		public int iRESStrike;

		public int iFinalHurt;

		public int iCritStrikeRate;

		public int iCritStrikeValue;

		public int iReduceCritStrikeRate;

		public int iReduceCritStrikeValue;

		public int iCritStrikeEff;

		public int iPhysicsHemophagiaRate;

		public int iMagicHemophagiaRate;

		public int iPhysicsHemophagia;

		public int iMagicHemophagia;

		public int iHurtOutputRate;

		public int iMoveSpeedMax;

		public int iSoulExpMax;

		public uint uiAddSoulExpIntervalMax;

		public ulong ulLastAddSoulExpTime;

		public uint uiTeamSoulExpTotal;

		public ActorValueStatistic()
		{
			this.ulLastAddSoulExpTime = Singleton<FrameSynchr>.instance.LogicFrameTick;
		}
	}
}
