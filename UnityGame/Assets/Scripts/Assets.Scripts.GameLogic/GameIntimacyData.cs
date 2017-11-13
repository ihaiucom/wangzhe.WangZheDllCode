using CSProtocol;
using System;

namespace Assets.Scripts.GameLogic
{
	public class GameIntimacyData
	{
		public int intimacyValue;

		public COM_INTIMACY_STATE state;

		public ulong ulluid;

		public int worldId;

		public string title;

		public GameIntimacyData(int intimacyValue, COM_INTIMACY_STATE state, ulong ulluid, int worldId, string title)
		{
			this.intimacyValue = intimacyValue;
			this.state = state;
			this.ulluid = ulluid;
			this.worldId = worldId;
			this.title = title;
		}
	}
}
