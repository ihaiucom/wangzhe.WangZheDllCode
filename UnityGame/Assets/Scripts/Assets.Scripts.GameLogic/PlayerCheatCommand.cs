using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic.GameKernal;
using CSProtocol;
using System;

namespace Assets.Scripts.GameLogic
{
	[FrameCommandClass(FRAMECMD_ID_DEF.FRAME_CMD_PLAYER_CHEAT)]
	public struct PlayerCheatCommand : ICommandImplement
	{
		public byte CheatType;

		[FrameCommandCreator]
		public static IFrameCommand Creator(ref FRAME_CMD_PKG msg)
		{
			FrameCommand<PlayerCheatCommand> frameCommand = FrameCommandFactory.CreateFrameCommand<PlayerCheatCommand>();
			frameCommand.cmdData.CheatType = msg.stCmdInfo.stCmdPlayerCheat.bCheatType;
			return frameCommand;
		}

		public bool TransProtocol(FRAME_CMD_PKG msg)
		{
			msg.stCmdInfo.stCmdPlayerCheat.bCheatType = this.CheatType;
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
			if (player != null && ((flag && player.isGM) || (!flag && LobbyMsgHandler.isHostGMAcnt)))
			{
				PoolObjHandle<ActorRoot> orignalActor = player.Captain.handle.ActorControl.GetOrignalActor();
				if (orignalActor)
				{
					CheatCommandBattleEntry.ProcessCheat(this.CheatType, ref orignalActor);
				}
			}
		}

		public void AwakeCommand(IFrameCommand cmd)
		{
		}
	}
}
