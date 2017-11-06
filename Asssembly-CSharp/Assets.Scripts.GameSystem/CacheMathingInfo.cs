using Assets.Scripts.UI;
using CSProtocol;
using System;

namespace Assets.Scripts.GameSystem
{
	public class CacheMathingInfo
	{
		public enUIEventID uiEventId;

		public byte mapType;

		public uint mapId;

		public bool CanGameAgain;

		public COM_AI_LEVEL AILevel = COM_AI_LEVEL.COM_AI_LEVEL_MIDDLE_OF_NORMALBATTLE;
	}
}
