using System;

namespace Assets.Scripts.GameLogic
{
	public class SpecialMotionControler
	{
		protected int motionSpeed;

		public virtual int GetMotionDistance(int _allTime)
		{
			return 0;
		}

		public virtual int GetMotionDeltaDistance(int _deltaTime)
		{
			return 0;
		}

		public virtual int GetMotionLerpDistance(int _deltaTime)
		{
			return 0;
		}
	}
}
