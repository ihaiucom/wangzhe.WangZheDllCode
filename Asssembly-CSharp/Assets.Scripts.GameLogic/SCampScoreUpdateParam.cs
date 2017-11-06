using Assets.Scripts.Common;
using CSProtocol;
using System;

namespace Assets.Scripts.GameLogic
{
	public struct SCampScoreUpdateParam
	{
		public int HeadCount;

		public int HeadPoints;

		public PoolObjHandle<ActorRoot> src;

		public PoolObjHandle<ActorRoot> atker;

		public COM_PLAYERCAMP CampType;

		public SCampScoreUpdateParam(int inHeadCount, int inHeadPoints, PoolObjHandle<ActorRoot> inSrc, PoolObjHandle<ActorRoot> inAtker, COM_PLAYERCAMP inCampType)
		{
			this.HeadCount = inHeadCount;
			this.HeadPoints = inHeadPoints;
			this.src = inSrc;
			this.atker = inAtker;
			this.CampType = inCampType;
		}
	}
}
