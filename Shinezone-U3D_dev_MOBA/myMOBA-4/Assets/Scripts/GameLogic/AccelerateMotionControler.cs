using System;

namespace Assets.Scripts.GameLogic
{
	public class AccelerateMotionControler : SpecialMotionControler
	{
		private int curTime;

		private int lerpCurTime;

		private int lastDistance;

		private int lastLerpDistance;

		private int accelerateSpeed;

		public void Reset()
		{
			this.curTime = 0;
			this.lerpCurTime = 0;
			this.lastDistance = 0;
			this.lastLerpDistance = 0;
		}

		public void ResetTime()
		{
			this.curTime = 0;
			this.lastDistance = 0;
		}

		public void ResetLerpTime()
		{
			this.lerpCurTime = 0;
			this.lastLerpDistance = 0;
		}

		public void InitMotionControler(int _motionSpeed, int _accelerateSpeed)
		{
			this.curTime = 0;
			this.lerpCurTime = 0;
			this.lastDistance = 0;
			this.lastLerpDistance = 0;
			this.motionSpeed = _motionSpeed;
			this.accelerateSpeed = _accelerateSpeed;
		}

		public void InitMotionControler(int _time, int _distance, int _accelerateSpeed)
		{
			this.curTime = 0;
			this.lerpCurTime = 0;
			this.lastDistance = 0;
			this.lastLerpDistance = 0;
			this.accelerateSpeed = _accelerateSpeed;
			long a = (long)_distance * 2000L - (long)this.accelerateSpeed * ((long)_time * (long)_time);
			this.motionSpeed = (int)IntMath.Divide(a, (long)((long)_time << 1));
		}

		public override int GetMotionDistance(int _allTime)
		{
			long a = ((long)this.motionSpeed * (long)_allTime << 1) + (long)this.accelerateSpeed * ((long)_allTime * (long)_allTime);
			return (int)IntMath.Divide(a, 2000L);
		}

		public override int GetMotionLerpDistance(int _deltaTime)
		{
			this.lerpCurTime += _deltaTime;
			int motionDistance = this.GetMotionDistance(this.lerpCurTime);
			int result = motionDistance - this.lastLerpDistance;
			this.lastLerpDistance = motionDistance;
			return result;
		}

		public override int GetMotionDeltaDistance(int _deltaTime)
		{
			this.curTime += _deltaTime;
			int motionDistance = this.GetMotionDistance(this.curTime);
			int result = motionDistance - this.lastDistance;
			this.lastDistance = motionDistance;
			return result;
		}
	}
}
