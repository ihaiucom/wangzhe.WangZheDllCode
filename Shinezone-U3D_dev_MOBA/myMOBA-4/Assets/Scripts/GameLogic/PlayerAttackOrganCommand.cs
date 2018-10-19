using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic.GameKernal;
using CSProtocol;
using System;

namespace Assets.Scripts.GameLogic
{
	[FrameCommandClass(FRAMECMD_ID_DEF.FRAME_CMD_PLAYERATTACKORGANMODE)]
	public struct PlayerAttackOrganCommand : ICommandImplement
	{
		public byte attackOrganMode;

		[FrameCommandCreator]
		public static IFrameCommand Creator(ref FRAME_CMD_PKG msg)
		{
			FrameCommand<PlayerAttackOrganCommand> frameCommand = FrameCommandFactory.CreateFrameCommand<PlayerAttackOrganCommand>();
			frameCommand.cmdData.attackOrganMode = msg.stCmdInfo.stCmdPlayAttackOrganMode.bAttackOrganMode;
			return frameCommand;
		}

		public bool TransProtocol(FRAME_CMD_PKG msg)
		{
			msg.stCmdInfo.stCmdPlayAttackOrganMode.bAttackOrganMode = this.attackOrganMode;
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
			Player player = Singleton<GamePlayerCenter>.GetInstance().GetPlayer(cmd.playerID);
			if (player != null)
			{
				player.SetAttackOrganMode((AttackOrganMode)this.attackOrganMode);
			}
		}

		public void AwakeCommand(IFrameCommand cmd)
		{
		}
	}
}
