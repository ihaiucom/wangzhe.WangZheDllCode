using System;

namespace Assets.Scripts.GameLogic
{
	public class xAxisAccelerateMotionControler : SpecialMotionControler
	{
		private int curTime;

		private int lerpCurTime;

		private int lastDistance;

		private int lastLerpDistance;

		protected int[] acceleration = new int[2];

		private int[] accelarationTime = new int[2];

		private int xPos;

		private int desPos;

		private int curAccelaration;

		private int curLerpAccelaration;

		private int lerpMotionSpeed;

		private bool accelerationSwitch;

		private bool lerpAccelerationSwitch;

		public void InitMotionControler(int _time0, int _time1, int _time2, int _distanceZ0, int _distanceZ1, int _distanceX)
		{
			this.curTime = 0;
			this.lerpCurTime = 0;
			this.lastDistance = 0;
			this.lastLerpDistance = 0;
			this.xPos = _distanceX;
			this.accelerationSwitch = false;
			this.lerpAccelerationSwitch = false;
			long num = (long)_time0;
			long a = -(long)_distanceX * 2L * 1000L * 1000L;
			long b = num * num;
			this.acceleration[0] = (int)IntMath.Divide(a, b);
			num = (long)(_time1 - _time0);
			b = num * num;
			this.acceleration[1] = (int)IntMath.Divide(a, b);
			num = (long)(_time2 - _time0);
			this.desPos = (int)((long)this.acceleration[1] * num * num / 1000L / 1000L / 2L + (long)_distanceX);
			this.accelarationTime[0] = _time0;
			this.accelarationTime[1] = _time1;
			this.motionSpeed = (int)((long)(-this.acceleration[0] * this.accelarationTime[0]) / 1000L);
			this.curAccelaration = this.acceleration[0];
			this.lerpMotionSpeed = this.motionSpeed;
			this.curLerpAccelaration = this.acceleration[0];
		}

		public int getDesPostion()
		{
			return this.desPos;
		}

		public override int GetMotionDistance(int _allTime)
		{
			long a = ((long)this.motionSpeed * (long)_allTime << 1) + (long)this.curAccelaration * ((long)_allTime * (long)_allTime) / 1000L;
			return (int)IntMath.Divide(a, 2000L);
		}

		public int GetLerpMotionDistance(int _allTime)
		{
			long a = ((long)this.lerpMotionSpeed * (long)_allTime << 1) + (long)this.curLerpAccelaration * ((long)_allTime * (long)_allTime) / 1000L;
			return (int)IntMath.Divide(a, 2000L);
		}

		public override int GetMotionLerpDistance(int _deltaTime)
		{
			int lerpMotionDistance;
			int result;
			if (!this.lerpAccelerationSwitch && this.lerpCurTime < this.accelarationTime[0] && this.lerpCurTime + _deltaTime >= this.accelarationTime[0])
			{
				this.lerpMotionSpeed = 0;
				this.curLerpAccelaration = this.acceleration[1];
				_deltaTime = this.lerpCurTime + _deltaTime - this.accelarationTime[0];
				lerpMotionDistance = this.GetLerpMotionDistance(_deltaTime);
				result = this.xPos + lerpMotionDistance - this.lastLerpDistance;
				this.lastLerpDistance = lerpMotionDistance;
				this.lerpCurTime = _deltaTime;
				this.lerpAccelerationSwitch = true;
				return result;
			}
			this.lerpCurTime += _deltaTime;
			lerpMotionDistance = this.GetLerpMotionDistance(this.lerpCurTime);
			result = lerpMotionDistance - this.lastLerpDistance;
			this.lastLerpDistance = lerpMotionDistance;
			return result;
		}

		public override int GetMotionDeltaDistance(int _deltaTime)
		{
			int motionDistance;
			int result;
			if (!this.accelerationSwitch && this.curTime < this.accelarationTime[0] && this.curTime + _deltaTime >= this.accelarationTime[0])
			{
				this.motionSpeed = 0;
				this.curAccelaration = this.acceleration[1];
				_deltaTime = this.curTime + _deltaTime - this.accelarationTime[0];
				motionDistance = this.GetMotionDistance(_deltaTime);
				result = this.xPos + motionDistance - this.lastDistance;
				this.lastDistance = motionDistance;
				this.curTime = _deltaTime;
				this.accelerationSwitch = true;
				return result;
			}
			this.curTime += _deltaTime;
			motionDistance = this.GetMotionDistance(this.curTime);
			result = motionDistance - this.lastDistance;
			this.lastDistance = motionDistance;
			return result;
		}
	}
}
