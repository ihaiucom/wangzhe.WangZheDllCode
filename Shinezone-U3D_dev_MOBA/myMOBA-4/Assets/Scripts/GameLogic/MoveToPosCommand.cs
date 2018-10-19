using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic.GameKernal;
using CSProtocol;
using System;

namespace Assets.Scripts.GameLogic
{
	[FrameCommandClass(FRAMECMD_ID_DEF.FRAME_CMD_PLAYERMOVE)]
	public struct MoveToPosCommand : ICommandImplement
	{
		public VInt3 destPosition;

		[FrameCommandCreator]
		public static IFrameCommand Creator(ref FRAME_CMD_PKG msg)
		{
			FrameCommand<MoveToPosCommand> frameCommand = FrameCommandFactory.CreateFrameCommand<MoveToPosCommand>();
			frameCommand.cmdData.destPosition = CommonTools.ToVector3(msg.stCmdInfo.stCmdPlayerMove.stWorldPos);
			return frameCommand;
		}

		public bool TransProtocol(FRAME_CMD_PKG msg)
		{
			CommonTools.FromVector3(this.destPosition, ref msg.stCmdInfo.stCmdPlayerMove.stWorldPos);
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
			if (player != null && player.Captain)
			{
				player.Captain.handle.ActorControl.CmdMovePosition(cmd, this.destPosition);
				if (!player.m_bMoved)
				{
					player.m_bMoved = true;
					Singleton<EventRouter>.instance.BroadCastEvent<Player>(EventID.FirstMoved, player);
				}
			}
		}

		public void AwakeCommand(IFrameCommand cmd)
		{
			Player player = Singleton<GamePlayerCenter>.GetInstance().GetPlayer(cmd.playerID);
			if (player != null && player.Captain)
			{
				player.Captain.handle.ActorControl.CmdMovePosition(cmd, this.destPosition);
				if (!player.m_bMoved)
				{
					player.m_bMoved = true;
					Singleton<EventRouter>.instance.BroadCastEvent<Player>(EventID.FirstMoved, player);
				}
			}
		}
	}
}
