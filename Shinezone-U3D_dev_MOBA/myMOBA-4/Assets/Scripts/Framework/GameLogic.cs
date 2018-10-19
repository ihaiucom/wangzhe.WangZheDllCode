using AGE;
using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.GameSystem;
using CSProtocol;
using Pathfinding.RVO;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Assets.Scripts.Framework
{
	public class GameLogic : Singleton<GameLogic>, IGameModule
	{
		private GameObjMgr optObjMgr;

		private LobbyLogic optLobby;

		private BattleLogic optBattle;

		private DropItemMgr optDropMgr;

		private CRoleInfoManager optRoleMgr;

		public uint GameRunningTick;

		public bool bInLogicTick;

		private bool bHasTailPart;

		private int nTailPartDelta;

		public uint HashCheckFreq = 500u;

		public uint SnakeTraceMasks;

		public uint SnakeTraceSize;

		public override void Init()
		{
			this.optObjMgr = Singleton<GameObjMgr>.GetInstance();
			this.optLobby = Singleton<LobbyLogic>.GetInstance();
			this.optBattle = Singleton<BattleLogic>.GetInstance();
			Singleton<GameInput>.GetInstance();
			this.optDropMgr = Singleton<DropItemMgr>.GetInstance();
			this.optRoleMgr = Singleton<CRoleInfoManager>.GetInstance();
		}

		public void UpdateFrame()
		{
		}

		private void UpdateLogicPartA(int nDelta)
		{
			this.bInLogicTick = true;
			uint curFrameNum = Singleton<FrameSynchr>.GetInstance().CurFrameNum;
			Singleton<GameEventSys>.instance.UpdateEvent();
			ActionManager.Instance.UpdateLogic(nDelta);
			if (MTileHandlerHelper.Instance != null)
			{
				MTileHandlerHelper.Instance.UpdateLogic();
			}
			if (RVOSimulator.Instance != null)
			{
				RVOSimulator.Instance.UpdateLogic(nDelta);
			}
			this.optLobby.UpdateLogic(nDelta);
			this.optBattle.UpdateLogic(nDelta);
			this.bInLogicTick = false;
		}

		private void UpdateLogicPartB(int nDelta)
		{
			this.bInLogicTick = true;
			this.optObjMgr.UpdateLogic(nDelta);
			this.optDropMgr.UpdateLogic(nDelta);
			this.optRoleMgr.UpdateLogic(nDelta);
			if (Singleton<ShenFuSystem>.instance != null)
			{
				Singleton<ShenFuSystem>.instance.UpdateLogic(nDelta);
			}
			Singleton<CTimerManager>.instance.UpdateLogic(nDelta);
			this.SampleFrameSyncData();
			this.bInLogicTick = false;
		}

		public void UpdateLogic(int nDelta, bool bPart)
		{
			this.GameRunningTick += (uint)nDelta;
			DebugHelper.Assert(!this.bHasTailPart);
			this.UpdateLogicPartA(nDelta);
			this.bHasTailPart = true;
			this.nTailPartDelta = nDelta;
			if (!bPart)
			{
				this.UpdateTails();
			}
		}

		public bool UpdateTails()
		{
			if (this.bHasTailPart)
			{
				try
				{
					this.UpdateLogicPartB(this.nTailPartDelta);
					this.bHasTailPart = false;
				}
				catch (Exception ex)
				{
					string text = ex.StackTrace;
					if (text.Length > 200)
					{
						text = text.Substring(0, 200);
					}
					DebugHelper.Assert(false, "exception in GameLogic.UpdateTails:{0}, \n {1}", new object[]
					{
						ex.Message,
						text
					});
					this.bHasTailPart = false;
				}
				return true;
			}
			return false;
		}

		public void LateUpdate()
		{
			this.optObjMgr.LateUpdate();
		}

		public void OpenLobby()
		{
			Singleton<LobbyLogic>.GetInstance().OpenLobby();
		}

		private void SampleFrameSyncData()
		{
			if (Singleton<FrameSynchr>.instance.bActive && 
                Singleton<FrameSynchr>.instance.CurFrameNum % this.HashCheckFreq == 0u && 
                Singleton<BattleLogic>.instance.isFighting && 
                !Singleton<WatchController>.GetInstance().IsWatching)
			{
				List<PoolObjHandle<ActorRoot>> heroActors = Singleton<GameObjMgr>.instance.HeroActors;
				int num = 1 + heroActors.Count * 7 + 1;
				int[] array = new int[num];
				int num2 = 0;
				array[num2++] = (int)Singleton<FrameSynchr>.instance.CurFrameNum;
				for (int i = 0; i < heroActors.Count; i++)
				{
					ActorRoot handle = heroActors[i].handle;
					array[num2++] = (int)handle.ObjID;
					array[num2++] = handle.location.x;
					array[num2++] = handle.location.y;
					array[num2++] = handle.location.z;
					array[num2++] = handle.ValueComponent.actorHp;
					array[num2++] = handle.ValueComponent.actorHpTotal;
					array[num2++] = (int)(handle.ActorControl.myBehavior | (ObjBehaviMode)(((!handle.HorizonMarker.IsVisibleFor(COM_PLAYERCAMP.COM_PLAYERCAMP_1)) ? 0 : 1) << 5) | (ObjBehaviMode)(((!handle.HorizonMarker.IsVisibleFor(COM_PLAYERCAMP.COM_PLAYERCAMP_2)) ? 0 : 1) << 6));
				}
				array[num2++] = Singleton<GameObjMgr>.GetInstance().GameActors.Count;
				byte[] array2 = new byte[num * 4];
				Buffer.BlockCopy(array, 0, array2, 0, array2.Length);
				MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
				mD5CryptoServiceProvider.Initialize();
				mD5CryptoServiceProvider.TransformFinalBlock(array2, 0, array2.Length);
				ulong num3 = (ulong)BitConverter.ToInt64(mD5CryptoServiceProvider.Hash, 0);
				ulong num4 = (ulong)BitConverter.ToInt64(mD5CryptoServiceProvider.Hash, 8);
				ulong ullHashToChk = num3 ^ num4;
				CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1280u);
				cSPkg.stPkgData.stRelayHashChk.dwKFrapsNo = Singleton<FrameSynchr>.instance.CurFrameNum;
				cSPkg.stPkgData.stRelayHashChk.ullHashToChk = ullHashToChk;
				if (Singleton<GamePlayerCenter>.instance.GetHostPlayer() != null)
				{
					CampInfo campInfoByCamp = Singleton<BattleStatistic>.GetInstance().GetCampInfoByCamp(Singleton<GamePlayerCenter>.instance.GetHostPlayer().PlayerCamp);
					int headPoints = campInfoByCamp.HeadPoints;
					COM_PLAYERCAMP campType;
					if (campInfoByCamp.campType == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
					{
						campType = COM_PLAYERCAMP.COM_PLAYERCAMP_2;
					}
					else
					{
						campType = COM_PLAYERCAMP.COM_PLAYERCAMP_1;
					}
					CampInfo campInfoByCamp2 = Singleton<BattleStatistic>.GetInstance().GetCampInfoByCamp(campType);
					int headPoints2 = campInfoByCamp2.HeadPoints;
					int iCampKillCntDiff = headPoints - headPoints2;
					cSPkg.stPkgData.stRelayHashChk.iCampKillCntDiff = iCampKillCntDiff;
				}
				Singleton<NetworkModule>.instance.SendGameMsg(ref cSPkg, 0u);
			}
		}

		public void OnPlayerLogout()
		{
			Singleton<NetworkModule>.GetInstance().CloseAllServerConnect();
			this.ClearLogicData();
		}

		public void ClearLogicData()
		{
			Singleton<CRoleInfoManager>.instance.ClearMasterRoleInfo();
			Singleton<CAdventureSys>.GetInstance().Clear();
			Singleton<CMatchingSystem>.GetInstance().Clear();
			Singleton<CRoomSystem>.GetInstance().Clear();
			Singleton<CSymbolSystem>.GetInstance().Clear();
			Singleton<ActivitySys>.GetInstance().Clear();
			Singleton<CFriendContoller>.instance.ClearAll();
			Singleton<CChatController>.instance.ClearAll();
			Singleton<BurnExpeditionController>.instance.ClearAll();
			Singleton<InBattleMsgMgr>.instance.ClearData();
			if (MonoSingleton<NewbieGuideManager>.HasInstance())
			{
				MonoSingleton<NewbieGuideManager>.instance.StopCurrentGuide();
				MonoSingleton<NewbieGuideManager>.ClearDestroy();
			}
			Singleton<CMailSys>.instance.Clear();
			Singleton<CTaskSys>.instance.Clear();
			Singleton<CGuildSystem>.GetInstance().Clear();
			Singleton<CGuildMatchSystem>.GetInstance().Clear();
			GameDataMgr.ClearServerResData();
			Singleton<CMallFactoryShopController>.GetInstance().Clear();
			Singleton<RankingSystem>.GetInstance().ClearAll();
			Singleton<CLobbySystem>.GetInstance().Clear();
			Singleton<CUnionBattleRankSystem>.GetInstance().Clear();
			Singleton<HeadIconSys>.instance.Clear();
			Singleton<CLoudSpeakerSys>.instance.Clear();
			Singleton<COBSystem>.instance.Clear();
			Singleton<CInviteSystem>.instance.Clear();
			Singleton<CArenaSystem>.instance.Clear();
			Singleton<SCModuleControl>.instance.Clear();
			MonoSingleton<IDIPSys>.GetInstance().ClearIDIPData();
			MonoSingleton<TGASys>.GetInstance().UnInitSys();
		}
	}
}
