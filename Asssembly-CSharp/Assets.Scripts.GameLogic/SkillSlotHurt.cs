using System;

namespace Assets.Scripts.GameLogic
{
	public struct SkillSlotHurt
	{
		public int curTotalHurt;

		public int nextTotalHurt;

		public uint skillUseCount;

		public int cdTime;

		public ulong recordTime;
	}
}
