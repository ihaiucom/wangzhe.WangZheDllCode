using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic.GameKernal;
using CSProtocol;
using System;

namespace Assets.Scripts.GameLogic
{
	[FrameSCSYNCCommandClass(SC_FRAME_CMD_ID_DEF.SC_FRAME_CMD_ASSISTSTATECHG)]
	public struct AssistStateChgCommand : ICommandImplement
	{
		public byte m_chgType;

		public uint m_aiPlayerID;

		public uint m_masterPlayerID;

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
			Player player = Singleton<GamePlayerCenter>.instance.GetPlayer(this.m_aiPlayerID);
			Player player2 = Singleton<GamePlayerCenter>.instance.GetPlayer(this.m_masterPlayerID);
			if (player != null && player.Captain && player2 != null && player2.Captain)
			{
				PoolObjHandle<ActorRoot> orignalActor = player.Captain.handle.ActorControl.GetOrignalActor();
				if (orignalActor)
				{
					if (this.m_chgType == 1)
					{
						orignalActor.handle.ActorControl.SetFollowOther(true, player2.Captain.handle.ObjID);
					}
					else
					{
						orignalActor.handle.ActorControl.SetFollowOther(false, 0u);
					}
				}
			}
		}

		public void AwakeCommand(IFrameCommand cmd)
		{
		}
	}
}
