using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic.GameKernal;
using CSProtocol;
using System;

namespace Assets.Scripts.GameLogic
{
	[FrameCommandClass(FRAMECMD_ID_DEF.FRAME_CMD_SWITCHAOUTAI)]
	public struct SwitchActorAutoAICommand : ICommandImplement
	{
		public sbyte IsAutoAI;

		[FrameCommandCreator]
		public static IFrameCommand Creator(ref FRAME_CMD_PKG msg)
		{
			FrameCommand<SwitchActorAutoAICommand> frameCommand = FrameCommandFactory.CreateFrameCommand<SwitchActorAutoAICommand>();
			frameCommand.cmdData.IsAutoAI = msg.stCmdInfo.stCmdPlayerSwithAutoAI.chIsAutoAI;
			return frameCommand;
		}

		public bool TransProtocol(FRAME_CMD_PKG msg)
		{
			msg.stCmdInfo.stCmdPlayerSwithAutoAI.chIsAutoAI = this.IsAutoAI;
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
				player.Captain.handle.ActorControl.SetAutoAI((int)this.IsAutoAI != 0);
			}
		}

		public void AwakeCommand(IFrameCommand cmd)
		{
		}
	}
}
