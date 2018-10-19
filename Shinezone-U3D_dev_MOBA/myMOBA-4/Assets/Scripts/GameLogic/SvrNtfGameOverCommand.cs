using Assets.Scripts.Framework;
using CSProtocol;
using System;

namespace Assets.Scripts.GameLogic
{
	[FrameSCSYNCCommandClass(SC_FRAME_CMD_ID_DEF.SC_FRAME_CMD_SVRNTF_GAMEOVER)]
	public struct SvrNtfGameOverCommand : ICommandImplement
	{
		public byte m_bWinCamp;

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
			Singleton<BattleLogic>.instance.DealGameSurrender(this.m_bWinCamp);
		}

		public void AwakeCommand(IFrameCommand cmd)
		{
		}
	}
}
