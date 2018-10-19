using Assets.Scripts.Framework;
using Assets.Scripts.GameSystem;
using CSProtocol;
using System;

namespace Assets.Scripts.GameLogic
{
	[FrameCommandClass(FRAMECMD_ID_DEF.FRAME_CMD_Signal_Btn_Position)]
	public struct SignalBtnPosition : ICommandImplement
	{
		public byte m_signalID;

		public VInt3 m_worldPos;

		[FrameCommandCreator]
		public static IFrameCommand Creator(ref FRAME_CMD_PKG msg)
		{
			FrameCommand<SignalBtnPosition> frameCommand = FrameCommandFactory.CreateFrameCommand<SignalBtnPosition>();
			frameCommand.cmdData.m_signalID = msg.stCmdInfo.stCmdSignalBtnPosition.bSignalID;
			frameCommand.cmdData.m_worldPos = CommonTools.ToVector3(msg.stCmdInfo.stCmdSignalBtnPosition.stWorldPos);
			return frameCommand;
		}

		public bool TransProtocol(FRAME_CMD_PKG msg)
		{
			msg.stCmdInfo.stCmdSignalBtnPosition.bSignalID = this.m_signalID;
			CommonTools.FromVector3(this.m_worldPos, ref msg.stCmdInfo.stCmdSignalBtnPosition.stWorldPos);
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
				signalPanel.ExecCommand_SignalBtn_Position(cmd.playerID, this.m_signalID, ref this.m_worldPos);
			}
		}

		public void AwakeCommand(IFrameCommand cmd)
		{
		}
	}
}
