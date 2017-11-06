using System;

namespace Assets.Scripts.GameSystem
{
	public struct stVipInfo
	{
		public uint score;

		public uint level;

		public uint headIconId;

		public void Reset()
		{
			this.score = 0u;
			this.level = 0u;
			this.headIconId = 0u;
		}
	}
}
