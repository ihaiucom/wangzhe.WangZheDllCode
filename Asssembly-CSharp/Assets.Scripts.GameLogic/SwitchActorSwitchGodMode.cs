using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic.GameKernal;
using CSProtocol;
using System;

namespace Assets.Scripts.GameLogic
{
	[FrameCommandClass(FRAMECMD_ID_DEF.FRAME_CMD_SWITCHGODMODE)]
	public struct SwitchActorSwitchGodMode : ICommandImplement
	{
		public sbyte IsGodMode;

		[FrameCommandCreator]
		public static IFrameCommand Creator(ref FRAME_CMD_PKG msg)
		{
			FrameCommand<SwitchActorSwitchGodMode> frameCommand = FrameCommandFactory.CreateFrameCommand<SwitchActorSwitchGodMode>();
			frameCommand.cmdData.IsGodMode = msg.stCmdInfo.stCmdPlayerSwitchGodMode.chIsGodMode;
			return frameCommand;
		}

		public bool TransProtocol(FRAME_CMD_PKG msg)
		{
			msg.stCmdInfo.stCmdPlayerSwitchGodMode.chIsGodMode = this.IsGodMode;
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
			if (player != null && ((flag && player.isGM) || (!flag && LobbyMsgHandler.isHostGMAcnt)) && player.Captain && player.Captain.handle.ActorControl is HeroWrapper)
			{
				HeroWrapper heroWrapper = (HeroWrapper)player.Captain.handle.ActorControl;
				heroWrapper.bGodMode = ((int)this.IsGodMode != 0);
			}
		}

		public void AwakeCommand(IFrameCommand cmd)
		{
		}
	}
}
