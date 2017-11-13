using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.GameSystem;
using CSProtocol;
using System;

namespace Assets.Scripts.GameLogic
{
	[FrameSCSYNCCommandClass(SC_FRAME_CMD_ID_DEF.SC_FRAME_CMD_PLAYERDISCONNECT)]
	public struct SvrDisconnectCommand : ICommandImplement
	{
		public bool TransProtocol(FRAME_CMD_PKG msg)
		{
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
			Player player = Singleton<GamePlayerCenter>.instance.GetPlayer(cmd.playerID);
			if (player != null && player.Captain)
			{
				PoolObjHandle<ActorRoot> orignalActor = player.Captain.handle.ActorControl.GetOrignalActor();
				if (orignalActor)
				{
					if (ActorHelper.IsHostCampActor(ref orignalActor))
					{
						KillDetailInfo killDetailInfo = new KillDetailInfo();
						killDetailInfo.Killer = orignalActor;
						killDetailInfo.bSelfCamp = true;
						killDetailInfo.Type = KillDetailInfoType.Info_Type_Disconnect;
						Singleton<EventRouter>.instance.BroadCastEvent<KillDetailInfo>(EventID.AchievementRecorderEvent, killDetailInfo);
						Singleton<EventRouter>.instance.BroadCastEvent<bool, uint>(EventID.DisConnectNtf, true, cmd.playerID);
					}
					orignalActor.handle.ActorControl.SetOffline(true);
				}
				Singleton<CBattleSystem>.GetInstance().m_battleEquipSystem.ExecuteInOutEquipShopFrameCommand(0, ref player.Captain);
			}
		}

		public void AwakeCommand(IFrameCommand cmd)
		{
		}
	}
}
