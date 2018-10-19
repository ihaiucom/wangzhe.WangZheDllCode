using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic.GameKernal;
using CSProtocol;
using System;

namespace Assets.Scripts.GameLogic
{
	[FrameCommandClass(FRAMECMD_ID_DEF.FRAME_CMD_SWITCHSUPERKILLER)]
	public struct SwitchActorSuperKillerCommand : ICommandImplement
	{
		public sbyte IsSuperKiller;

		[FrameCommandCreator]
		public static IFrameCommand Creator(ref FRAME_CMD_PKG msg)
		{
			FrameCommand<SwitchActorSuperKillerCommand> frameCommand = FrameCommandFactory.CreateFrameCommand<SwitchActorSuperKillerCommand>();
			frameCommand.cmdData.IsSuperKiller = msg.stCmdInfo.stCmdPlayerSwitchSuperKiller.chIsSuperKiller;
			return frameCommand;
		}

		public bool TransProtocol(FRAME_CMD_PKG msg)
		{
			msg.stCmdInfo.stCmdPlayerSwitchSuperKiller.chIsSuperKiller = this.IsSuperKiller;
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
			SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
			bool flag = curLvelContext != null && curLvelContext.IsMobaMode();
			Player player = Singleton<GamePlayerCenter>.GetInstance().GetPlayer(cmd.playerID);
			if (player != null && player.Captain && ((flag && player.isGM) || (!flag && LobbyMsgHandler.isHostGMAcnt)))
			{
				player.Captain.handle.bOneKiller = ((int)this.IsSuperKiller != 0);
			}
		}

		public void AwakeCommand(IFrameCommand cmd)
		{
		}
	}
}
