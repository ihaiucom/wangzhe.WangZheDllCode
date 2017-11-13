using CSProtocol;
using System;

namespace Assets.Scripts.GameLogic
{
	public class SingleGameContextFactory
	{
		public static GameContextBase CreateSingleGameContext(SCPKG_STARTSINGLEGAMERSP InMessage)
		{
			return new SingleGameContext(InMessage);
		}
	}
}
