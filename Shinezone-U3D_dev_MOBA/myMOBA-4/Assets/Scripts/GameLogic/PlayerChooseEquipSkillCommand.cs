using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.GameSystem;
using CSProtocol;
using System;

namespace Assets.Scripts.GameLogic
{
	[FrameCommandClass(FRAMECMD_ID_DEF.FRAME_CMD_PLAYER_CHOOSE_EQUIPSKILL)]
	public struct PlayerChooseEquipSkillCommand : ICommandImplement
	{
		public int m_iEquipSlotIndex;

		[FrameCommandCreator]
		public static IFrameCommand Creator(ref FRAME_CMD_PKG msg)
		{
			FrameCommand<PlayerChooseEquipSkillCommand> frameCommand = FrameCommandFactory.CreateFrameCommand<PlayerChooseEquipSkillCommand>();
			frameCommand.cmdData.m_iEquipSlotIndex = (int)msg.stCmdInfo.stCmdPlayerChooseEquipSkill.bEquipSlotIndex;
			return frameCommand;
		}

		public bool TransProtocol(FRAME_CMD_PKG msg)
		{
			msg.stCmdInfo.stCmdPlayerChooseEquipSkill.bEquipSlotIndex = (byte)this.m_iEquipSlotIndex;
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
					Singleton<CBattleSystem>.GetInstance().m_battleEquipSystem.ExecChooseEquipSkillCmd(this.m_iEquipSlotIndex, ref orignalActor);
				}
			}
		}

		public void AwakeCommand(IFrameCommand cmd)
		{
		}
	}
}
