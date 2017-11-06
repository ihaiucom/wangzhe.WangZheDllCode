using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.GameSystem;
using CSProtocol;
using System;

namespace Assets.Scripts.GameLogic
{
	[FrameCommandClass(FRAMECMD_ID_DEF.FRAME_CMD_BUY_HORIZON_EQUIP)]
	public struct PlayerBuyHorizonEquipCommand : ICommandImplement
	{
		public ushort m_equipID;

		[FrameCommandCreator]
		public static IFrameCommand Creator(ref FRAME_CMD_PKG msg)
		{
			FrameCommand<PlayerBuyHorizonEquipCommand> frameCommand = FrameCommandFactory.CreateFrameCommand<PlayerBuyHorizonEquipCommand>();
			frameCommand.cmdData.m_equipID = msg.stCmdInfo.stCmdPlayerBuyHorizonEquip.wEquipID;
			return frameCommand;
		}

		public bool TransProtocol(FRAME_CMD_PKG msg)
		{
			msg.stCmdInfo.stCmdPlayerBuyHorizonEquip.wEquipID = this.m_equipID;
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
				PoolObjHandle<ActorRoot> orignalActor = player.Captain.handle.ActorControl.GetOrignalActor();
				if (orignalActor)
				{
					Singleton<CBattleSystem>.GetInstance().m_battleEquipSystem.ExecuteBuyHorizonEquipFrameCommand(this.m_equipID, ref orignalActor);
				}
			}
		}

		public void AwakeCommand(IFrameCommand cmd)
		{
		}
	}
}
