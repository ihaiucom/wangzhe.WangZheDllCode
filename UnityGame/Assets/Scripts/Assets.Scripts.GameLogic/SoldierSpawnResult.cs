using System;

namespace Assets.Scripts.GameLogic
{
	public enum SoldierSpawnResult
	{
		ShouldWaitStart,
		ShouldWaitInterval,
		ShouldWaitSoldierInterval,
		ThresholdShouldWait,
		Finish,
		Error,
		UnStarted,
		Completed
	}
}
