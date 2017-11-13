using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic.GameKernal;
using CSProtocol;
using System;

namespace Assets.Scripts.GameLogic
{
	[FrameSCSYNCCommandClass(SC_FRAME_CMD_ID_DEF.SC_FRAME_CMD_CHGAUTOAI)]
	public struct AutoAIChgCommand : ICommandImplement
	{
		public byte m_autoType;

		public uint m_playerID;

		public bool TransProtocol(FRAME_CMD_PKG msg)
		{
			return true;
		}

		public bool TransProtocol(CSDT_GAMING_CSSYNCINFO msg)
		{
			return true;
		}

		public void OnReceive(IFrameCommand cmd)
		{
		}

		public void Preprocess(IFrameCommand cmd)
		{
		}

		public void ExecCommand(IFrameCommand cmd)
		{
			Player player = Singleton<GamePlayerCenter>.instance.GetPlayer(this.m_playerID);
			if (player != null && player.Captain)
			{
				PoolObjHandle<ActorRoot> orignalActor = player.Captain.handle.ActorControl.GetOrignalActor();
				if (orignalActor)
				{
					if (this.m_autoType == 1)
					{
						player.Captain.handle.ActorControl.SetAutoAI(true);
					}
					else if (this.m_autoType == 2)
					{
						player.Captain.handle.ActorControl.SetAutoAI(false);
					}
					else if (this.m_autoType == 3)
					{
						player.Captain.handle.ActorControl.SetOffline(true);
					}
				}
			}
		}

		public void AwakeCommand(IFrameCommand cmd)
		{
		}
	}
}
