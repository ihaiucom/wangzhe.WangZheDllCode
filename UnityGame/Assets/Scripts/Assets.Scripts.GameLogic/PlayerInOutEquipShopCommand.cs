using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.GameSystem;
using CSProtocol;
using System;

namespace Assets.Scripts.GameLogic
{
	[FrameCommandClass(FRAMECMD_ID_DEF.FRAME_CMD_PLAYER_IN_OUT_EQUIPSHOP)]
	public struct PlayerInOutEquipShopCommand : ICommandImplement
	{
		public byte m_inOut;

		[FrameCommandCreator]
		public static IFrameCommand Creator(ref FRAME_CMD_PKG msg)
		{
			FrameCommand<PlayerInOutEquipShopCommand> frameCommand = FrameCommandFactory.CreateFrameCommand<PlayerInOutEquipShopCommand>();
			frameCommand.cmdData.m_inOut = msg.stCmdInfo.stCmdPlayerInOutEquipShop.bInOut;
			return frameCommand;
		}

		public bool TransProtocol(FRAME_CMD_PKG msg)
		{
			msg.stCmdInfo.stCmdPlayerInOutEquipShop.bInOut = this.m_inOut;
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
				Singleton<CBattleSystem>.GetInstance().m_battleEquipSystem.ExecuteInOutEquipShopFrameCommand(this.m_inOut, ref player.Captain);
			}
		}

		public void AwakeCommand(IFrameCommand cmd)
		{
		}
	}
}
