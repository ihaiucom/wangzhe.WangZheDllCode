using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.GameSystem;
using CSProtocol;
using System;

namespace Assets.Scripts.GameLogic
{
	[FrameCSSYNCCommandClass(CSSYNC_TYPE_DEF.CSSYNC_CMD_BASEATTACK)]
	public struct UseCommonAttackCommand : ICommandImplement
	{
		public sbyte Start;

		public uint uiRealObjID;

		[FrameCommandCreator]
		public static IFrameCommand Creator(ref CSDT_GAMING_CSSYNCINFO msg)
		{
			FrameCommand<UseCommonAttackCommand> frameCommand = FrameCommandFactory.CreateCSSyncFrameCommand<UseCommonAttackCommand>();
			frameCommand.cmdData.Start = (sbyte)msg.stCSSyncDt.stBaseAttack.bStart;
			frameCommand.cmdData.uiRealObjID = msg.stCSSyncDt.stBaseAttack.dwObjectID;
			return frameCommand;
		}

		public bool TransProtocol(FRAME_CMD_PKG msg)
		{
			return true;
		}

		public bool TransProtocol(CSDT_GAMING_CSSYNCINFO msg)
		{
			msg.stCSSyncDt.stBaseAttack.bStart = (byte)this.Start;
			msg.stCSSyncDt.stBaseAttack.dwObjectID = this.uiRealObjID;
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
				player.Captain.handle.ActorControl.CmdCommonAttackMode(cmd, this.Start, this.uiRealObjID);
				Singleton<CBattleSystem>.GetInstance().m_battleEquipSystem.ExecuteInOutEquipShopFrameCommand(0, ref player.Captain);
			}
		}

		public void AwakeCommand(IFrameCommand cmd)
		{
		}
	}
}
