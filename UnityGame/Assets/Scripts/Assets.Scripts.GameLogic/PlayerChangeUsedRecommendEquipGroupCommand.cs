using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.GameSystem;
using CSProtocol;
using System;

namespace Assets.Scripts.GameLogic
{
	[FrameCommandClass(FRAMECMD_ID_DEF.FRAME_CMD_CHANGE_USED_RECOMMEND_EQUIP_GROUP)]
	public struct PlayerChangeUsedRecommendEquipGroupCommand : ICommandImplement
	{
		public byte m_group;

		[FrameCommandCreator]
		public static IFrameCommand Creator(ref FRAME_CMD_PKG msg)
		{
			FrameCommand<PlayerChangeUsedRecommendEquipGroupCommand> frameCommand = FrameCommandFactory.CreateFrameCommand<PlayerChangeUsedRecommendEquipGroupCommand>();
			frameCommand.cmdData.m_group = msg.stCmdInfo.stCmdPlayerChangeUsedRecommendEquipGroup.bGroup;
			return frameCommand;
		}

		public bool TransProtocol(FRAME_CMD_PKG msg)
		{
			msg.stCmdInfo.stCmdPlayerChangeUsedRecommendEquipGroup.bGroup = this.m_group;
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
					Singleton<CBattleSystem>.GetInstance().m_battleEquipSystem.ExecuteChangeUsedRecommendEquipGroup(this.m_group, ref orignalActor);
				}
			}
		}

		public void AwakeCommand(IFrameCommand cmd)
		{
		}
	}
}
