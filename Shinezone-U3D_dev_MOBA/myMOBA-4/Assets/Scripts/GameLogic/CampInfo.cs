using Assets.Scripts.Common;
using CSProtocol;
using ResData;
using System;
using System.Runtime.CompilerServices;

namespace Assets.Scripts.GameLogic
{
	public class CampInfo
	{
		public delegate void CampInfoValueChanged(COM_PLAYERCAMP campType, int inCampScore, int inHeadPts);

		private int _campScore;

		private int m_headPoints;

		public int destoryTowers;

		public int allHurtHp;

		public int numDeadSoldier;

		public int soulExpTotal;

		public int coinTotal;

		public COM_PLAYERCAMP campType;

		public event CampInfo.CampInfoValueChanged onCampScoreChanged;

		public int campScore
		{
			get
			{
				return this._campScore;
			}
		}

		public int HeadPoints
		{
			get
			{
				return this.m_headPoints;
			}
		}

		public CampInfo(COM_PLAYERCAMP CmpType)
		{
			this.campType = CmpType;
		}

		public void IncCampScore(PoolObjHandle<ActorRoot> inSrc, PoolObjHandle<ActorRoot> inAtker)
		{
			this._campScore++;
			this.OnUpdateCampPts(true, false, inSrc, inAtker);
		}

		public void IncHeadPoints(int deltaVal, PoolObjHandle<ActorRoot> inSrc, PoolObjHandle<ActorRoot> inAtker)
		{
			this.m_headPoints += deltaVal;
			this.OnUpdateCampPts(false, true, inSrc, inAtker);
		}

		private void OnUpdateCampPts(bool bUpdateScore, bool bUpdateHeadPts, PoolObjHandle<ActorRoot> inSrc, PoolObjHandle<ActorRoot> inAtker)
		{
			int num = this._campScore;
			if (!bUpdateScore)
			{
				num = -1;
			}
			int num2 = this.m_headPoints;
			if (!bUpdateHeadPts)
			{
				num2 = -1;
			}
			if (this.onCampScoreChanged != null)
			{
				this.onCampScoreChanged(this.campType, num, num2);
			}
			SCampScoreUpdateParam sCampScoreUpdateParam = new SCampScoreUpdateParam(num, num2, inSrc, inAtker, this.campType);
			Singleton<GameEventSys>.instance.SendEvent<SCampScoreUpdateParam>(GameEventDef.Event_CampScoreUpdated, ref sCampScoreUpdateParam);
		}

		public int GetScore(RES_STAR_CONDITION_DATA_SUB_TYPE inDataSubType)
		{
			if (inDataSubType == RES_STAR_CONDITION_DATA_SUB_TYPE.RES_STAR_CONDITION_DATA_HEAD_POINTS)
			{
				return this.HeadPoints;
			}
			if (inDataSubType == RES_STAR_CONDITION_DATA_SUB_TYPE.RES_STAR_CONDITION_DATA_HEADS)
			{
				return this.campScore;
			}
			return -1;
		}
	}
}
