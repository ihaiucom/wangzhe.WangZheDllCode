using Assets.Scripts.GameLogic;
using System;

namespace Assets.Scripts.Framework
{
	public class GameDataValidator : Singleton<GameDataValidator>
	{
		public bool bDebugLossPickHeroMessage;

		public bool IsLoseImportantGameData()
		{
			return !Singleton<LobbyLogic>.instance.inMultiRoom;
		}

		public bool ValidateGameData()
		{
			if (!Singleton<GameReplayModule>.instance.IsReplaying && this.IsLoseImportantGameData())
			{
				DebugHelper.Assert(false, "Warning, Lose HostPlayer, try to reconnect to gameserver. stacktrace:{0}", new object[]
				{
					Environment.get_StackTrace()
				});
				DebugHelper.Assert(Singleton<NetworkModule>.instance.gameSvr != null, "invalid gameserver");
				Singleton<NetworkModule>.instance.gameSvr.RestartConnector();
				return false;
			}
			return true;
		}
	}
}
