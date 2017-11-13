using System;

namespace Assets.Scripts.GameLogic
{
	public class CoutofControlInfo
	{
		public int combId;

		public int totalTime;

		public int leftTime;

		public CoutofControlInfo(int _combineID, int _totalTime, int _leftTime)
		{
			this.combId = _combineID;
			this.totalTime = _totalTime;
			this.leftTime = _leftTime;
		}
	}
}
