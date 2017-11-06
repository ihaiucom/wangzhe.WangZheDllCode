using System;

namespace Assets.Scripts.GameLogic
{
	public struct SoldierWaveParam
	{
		public int WaveIndex;

		public int RepeatCount;

		public int NextDuration;

		public SoldierWave Wave;

		public SoldierWaveParam(int inWaveIndex, int inRepeatCount, int inNextDuration, SoldierWave inWave)
		{
			this.WaveIndex = inWaveIndex;
			this.RepeatCount = inRepeatCount;
			this.NextDuration = inNextDuration;
			this.Wave = inWave;
		}
	}
}
