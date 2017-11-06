using System;

namespace Assets.Scripts.GameLogic
{
	public class ConstantMotionControler : SpecialMotionControler
	{
		public void InitMotionControler(int _motionSpeed)
		{
			this.motionSpeed = _motionSpeed;
		}

		public override int GetMotionDistance(int _allTime)
		{
			return this.motionSpeed * _allTime / 1000;
		}

		public override int GetMotionDeltaDistance(int _deltaTime)
		{
			return this.motionSpeed * _deltaTime / 1000;
		}
	}
}
