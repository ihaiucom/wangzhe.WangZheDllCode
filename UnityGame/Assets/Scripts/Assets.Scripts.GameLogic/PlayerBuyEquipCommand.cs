using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.GameSystem;
using CSProtocol;
using System;

namespace Assets.Scripts.GameLogic
{
	[FrameCommandClass(FRAMECMD_ID_DEF.FRAME_CMD_PLAYER_BUY_EQUIP)]
	public struct PlayerBuyEquipCommand : ICommandImplement
	{
		public ushort m_equipID;

		[FrameCommandCreator]
		public static IFrameCommand Creator(ref FRAME_CMD_PKG msg)
		{
			FrameCommand<PlayerBuyEquipCommand> frameCommand = FrameCommandFactory.CreateFrameCommand<PlayerBuyEquipCommand>();
			frameCommand.cmdData.m_equipID = msg.stCmdInfo.stCmdPlayerBuyEquip.wEquipID;
			return frameCommand;
		}

		public bool TransProtocol(FRAME_CMD_PKG msg)
		{
			msg.stCmdInfo.stCmdPlayerBuyEquip.wEquipID = this.m_equipID;
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
				if (player.Captain.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Call)
				{
					CallActorWrapper callActorWrapper = player.Captain.handle.ActorControl as CallActorWrapper;
					if (callActorWrapper != null && callActorWrapper.hostActor)
					{
						Singleton<CBattleSystem>.GetInstance().m_battleEquipSystem.ExecuteBuyEquipFrameCommand(this.m_equipID, ref callActorWrapper.hostActor);
					}
				}
				else
				{
					Singleton<CBattleSystem>.GetInstance().m_battleEquipSystem.ExecuteBuyEquipFrameCommand(this.m_equipID, ref player.Captain);
				}
			}
		}

		public void AwakeCommand(IFrameCommand cmd)
		{
		}
	}
}
