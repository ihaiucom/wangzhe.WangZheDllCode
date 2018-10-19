using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic.GameKernal;
using CSProtocol;
using System;

namespace Assets.Scripts.GameLogic
{
	[FrameCommandClass(FRAMECMD_ID_DEF.FRAME_CMD_SET_SKILL_LEVEL)]
	public struct SetSkillLevelInBattleCommand : ICommandImplement
	{
		public byte SkillSlot;

		public byte SkillLevel;

		[FrameCommandCreator]
		public static IFrameCommand Creator(ref FRAME_CMD_PKG msg)
		{
			FrameCommand<SetSkillLevelInBattleCommand> frameCommand = FrameCommandFactory.CreateFrameCommand<SetSkillLevelInBattleCommand>();
			frameCommand.cmdData.SkillSlot = msg.stCmdInfo.stCmdSetSkillLevel.bSkillSlot;
			frameCommand.cmdData.SkillLevel = msg.stCmdInfo.stCmdSetSkillLevel.bSkillLevel;
			return frameCommand;
		}

		public bool TransProtocol(FRAME_CMD_PKG msg)
		{
			msg.stCmdInfo.stCmdSetSkillLevel.bSkillSlot = this.SkillSlot;
			msg.stCmdInfo.stCmdSetSkillLevel.bSkillLevel = this.SkillLevel;
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
			if (player != null && player.isGM && player.Captain && player.Captain.handle.SkillControl != null && this.SkillSlot < 10)
			{
				if (player.Captain.handle.ActorControl != null)
				{
					player.Captain.handle.ActorControl.GetOrignalActor().handle.SkillControl.SkillSlotArray[(int)this.SkillSlot].SetSkillLevel((int)this.SkillLevel);
				}
				else
				{
					player.Captain.handle.SkillControl.SkillSlotArray[(int)this.SkillSlot].SetSkillLevel((int)this.SkillLevel);
				}
			}
		}

		public void AwakeCommand(IFrameCommand cmd)
		{
		}
	}
}
