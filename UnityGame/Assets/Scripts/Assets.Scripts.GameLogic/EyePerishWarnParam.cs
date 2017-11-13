using Assets.Scripts.Common;
using System;

namespace Assets.Scripts.GameLogic
{
	public struct EyePerishWarnParam
	{
		public PoolObjHandle<ActorRoot> SrcEye;

		public int LeftTime;

		public EyePerishWarnParam(PoolObjHandle<ActorRoot> inEye, int inLeftTime)
		{
			this.SrcEye = inEye;
			this.LeftTime = inLeftTime;
		}
	}
}
