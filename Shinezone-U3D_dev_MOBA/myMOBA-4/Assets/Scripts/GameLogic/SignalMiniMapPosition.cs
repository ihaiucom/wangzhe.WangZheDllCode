using Assets.Scripts.Framework;
using Assets.Scripts.GameSystem;
using CSProtocol;
using System;

namespace Assets.Scripts.GameLogic
{
	[FrameCommandClass(FRAMECMD_ID_DEF.FRAME_CMD_Signal_MiniMap_Position)]
	public struct SignalMiniMapPosition : ICommandImplement
	{
		public byte m_signalID;

		public VInt3 m_worldPos;

		public byte m_elementType;

		[FrameCommandCreator]
		public static IFrameCommand Creator(ref FRAME_CMD_PKG msg)
		{
			FrameCommand<SignalMiniMapPosition> frameCommand = FrameCommandFactory.CreateFrameCommand<SignalMiniMapPosition>();
			frameCommand.cmdData.m_signalID = msg.stCmdInfo.stCmdSignalMiniMapPosition.bSignalID;
			frameCommand.cmdData.m_worldPos = CommonTools.ToVector3(msg.stCmdInfo.stCmdSignalMiniMapPosition.stWorldPos);
			frameCommand.cmdData.m_elementType = msg.stCmdInfo.stCmdSignalMiniMapPosition.bElementType;
			return frameCommand;
		}

		public bool TransProtocol(FRAME_CMD_PKG msg)
		{
			msg.stCmdInfo.stCmdSignalMiniMapPosition.bSignalID = this.m_signalID;
			CommonTools.FromVector3(this.m_worldPos, ref msg.stCmdInfo.stCmdSignalMiniMapPosition.stWorldPos);
			msg.stCmdInfo.stCmdSignalMiniMapPosition.bElementType = this.m_elementType;
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
			SignalPanel signalPanel = (Singleton<CBattleSystem>.GetInstance().FightForm == null) ? null : Singleton<CBattleSystem>.GetInstance().FightForm.GetSignalPanel();
			if (signalPanel != null)
			{
				signalPanel.ExecCommand_SignalMiniMap_Position(cmd.playerID, this.m_signalID, ref this.m_worldPos, this.m_elementType);
			}
		}

		public void AwakeCommand(IFrameCommand cmd)
		{
		}
	}
}
