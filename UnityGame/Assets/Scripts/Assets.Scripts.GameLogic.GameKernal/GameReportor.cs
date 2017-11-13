using Apollo;
using Assets.Scripts.Framework;
using Assets.Scripts.GameSystem;
using CSProtocol;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameLogic.GameKernal
{
	public class GameReportor
	{
		private readonly List<KeyValuePair<string, string>> _eventsLoadingTime = new List<KeyValuePair<string, string>>();

		public void PrepareReport()
		{
			this._eventsLoadingTime.Clear();
			ApolloAccountInfo accountInfo = Singleton<ApolloHelper>.GetInstance().GetAccountInfo(false);
			DebugHelper.Assert(accountInfo != null, "account info is null");
			this._eventsLoadingTime.Add(new KeyValuePair<string, string>("OpenID", (accountInfo != null) ? accountInfo.OpenId : "0"));
			this._eventsLoadingTime.Add(new KeyValuePair<string, string>("LevelID", Singleton<GameContextEx>.GetInstance().GameContextCommonInfo.MapId.ToString()));
			this._eventsLoadingTime.Add(new KeyValuePair<string, string>("isPVPLevel", Singleton<GameContextEx>.GetInstance().IsMobaMode().ToString()));
			this._eventsLoadingTime.Add(new KeyValuePair<string, string>("isPVPMode", Singleton<GameContextEx>.GetInstance().IsMobaMode().ToString()));
			this._eventsLoadingTime.Add(new KeyValuePair<string, string>("bLevelNo", Singleton<GameContextEx>.GetInstance().GameContextSoloInfo.LevelNo.ToString()));
		}

		public void DoApolloReport()
		{
			string openID = Singleton<ApolloHelper>.GetInstance().GetOpenID();
			int mapId = Singleton<GameContextEx>.GetInstance().GameContextCommonInfo.MapId;
			COM_GAME_TYPE gameType = Singleton<GameContextEx>.GetInstance().GameContextCommonInfo.GameType;
			List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
			list.Add(new KeyValuePair<string, string>("g_version", CVersion.GetAppVersion()));
			list.Add(new KeyValuePair<string, string>("WorldID", MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString()));
			list.Add(new KeyValuePair<string, string>("platform", Singleton<ApolloHelper>.GetInstance().CurPlatform.ToString()));
			list.Add(new KeyValuePair<string, string>("openid", openID));
			list.Add(new KeyValuePair<string, string>("GameType", gameType.ToString()));
			list.Add(new KeyValuePair<string, string>("MapID", mapId.ToString()));
			list.Add(new KeyValuePair<string, string>("LoadingTime", Singleton<GameBuilderEx>.GetInstance().LastLoadingTime.ToString()));
			Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("Service_LoadingBattle", list, true);
			List<KeyValuePair<string, string>> list2 = new List<KeyValuePair<string, string>>();
			list2.Add(new KeyValuePair<string, string>("g_version", CVersion.GetAppVersion()));
			list2.Add(new KeyValuePair<string, string>("WorldID", MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString()));
			list2.Add(new KeyValuePair<string, string>("platform", Singleton<ApolloHelper>.GetInstance().CurPlatform.ToString()));
			list2.Add(new KeyValuePair<string, string>("openid", openID));
			list2.Add(new KeyValuePair<string, string>("totaltime", Singleton<CHeroSelectBaseSystem>.instance.m_fOpenHeroSelectForm.ToString()));
			list2.Add(new KeyValuePair<string, string>("gameType", gameType.ToString()));
			list2.Add(new KeyValuePair<string, string>("role_list", string.Empty));
			list2.Add(new KeyValuePair<string, string>("errorCode", string.Empty));
			list2.Add(new KeyValuePair<string, string>("error_msg", string.Empty));
			Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("Service_Login_EnterGame", list2, true);
			float num = Singleton<FrameSynchr>.GetInstance().LogicFrameTick * 0.001f;
			List<KeyValuePair<string, string>> list3 = new List<KeyValuePair<string, string>>();
			list3.Add(new KeyValuePair<string, string>("g_version", CVersion.GetAppVersion()));
			list3.Add(new KeyValuePair<string, string>("WorldID", MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString()));
			list3.Add(new KeyValuePair<string, string>("platform", Singleton<ApolloHelper>.GetInstance().CurPlatform.ToString()));
			list3.Add(new KeyValuePair<string, string>("openid", openID));
			list3.Add(new KeyValuePair<string, string>("GameType", gameType.ToString()));
			list3.Add(new KeyValuePair<string, string>("MapID", mapId.ToString()));
			list3.Add(new KeyValuePair<string, string>("Battle_Time", num.ToString()));
			list3.Add(new KeyValuePair<string, string>("music", GameSettings.EnableMusic.ToString()));
			list3.Add(new KeyValuePair<string, string>("quality", GameSettings.RenderQuality.ToString()));
			list3.Add(new KeyValuePair<string, string>("status", "1"));
			list3.Add(new KeyValuePair<string, string>("Quality_Mode", GameSettings.ModelLOD.ToString()));
			list3.Add(new KeyValuePair<string, string>("Quality_Particle", GameSettings.ParticleLOD.ToString()));
			list3.Add(new KeyValuePair<string, string>("receiveMoveCmdAverage", Singleton<FrameSynchr>.instance.m_receiveMoveCmdAverage.ToString()));
			list3.Add(new KeyValuePair<string, string>("receiveMoveCmdMax", Singleton<FrameSynchr>.instance.m_receiveMoveCmdMax.ToString()));
			list3.Add(new KeyValuePair<string, string>("execMoveCmdAverage", Singleton<FrameSynchr>.instance.m_execMoveCmdAverage.ToString()));
			list3.Add(new KeyValuePair<string, string>("execMoveCmdMax", Singleton<FrameSynchr>.instance.m_execMoveCmdMax.ToString()));
			list3.Add(new KeyValuePair<string, string>("MaxEndBlockWaitNum", Singleton<FrameSynchr>.instance.m_maxEndBlockWaitNum.ToString()));
			list3.Add(new KeyValuePair<string, string>("MaxExecuteFrameOnce", Singleton<FrameSynchr>.instance.m_maxExcuteFrameOnce.ToString()));
			List<KeyValuePair<string, string>> list4 = Singleton<DataReportSys>.GetInstance().ReportFPSToBeacon();
			for (int i = 0; i < list4.get_Count(); i++)
			{
				list3.Add(list4.get_Item(i));
			}
			list3.Add(new KeyValuePair<string, string>("LOD_Down", Singleton<BattleLogic>.GetInstance().m_iAutoLODState.ToString()));
			if (NetworkAccelerator.started)
			{
				if (NetworkAccelerator.isAccerating())
				{
					list3.Add(new KeyValuePair<string, string>("AccState", "Acc"));
				}
				else
				{
					list3.Add(new KeyValuePair<string, string>("AccState", "Direct"));
				}
			}
			else
			{
				list3.Add(new KeyValuePair<string, string>("AccState", "Off"));
			}
			int num2 = 0;
			if (MonoSingleton<VoiceSys>.GetInstance().UseSpeak && MonoSingleton<VoiceSys>.GetInstance().UseMic)
			{
				num2 = 2;
			}
			else if (MonoSingleton<VoiceSys>.GetInstance().UseSpeak)
			{
				num2 = 1;
			}
			list3.Add(new KeyValuePair<string, string>("Mic", num2.ToString()));
			Singleton<ApolloHelper>.GetInstance().ApolloRepoertEvent("Service_PVPBattle_Summary", list3, true);
			this._eventsLoadingTime.Clear();
			try
			{
				float num3 = (float)Singleton<DataReportSys>.GetInstance().FPS10Count / (float)Singleton<DataReportSys>.GetInstance().FPSCount;
				int iFps10PercentNum = Mathf.CeilToInt(num3 * 100f / 10f) * 10;
				float num4 = (float)(Singleton<DataReportSys>.GetInstance().FPS10Count + Singleton<DataReportSys>.GetInstance().FPS18Count) / (float)Singleton<DataReportSys>.GetInstance().FPSCount;
				int iFps18PercentNum = Mathf.CeilToInt(num4 * 100f / 10f) * 10;
				CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(5000u);
				cSPkg.stPkgData.stCltPerformance.iMapID = mapId;
				cSPkg.stPkgData.stCltPerformance.iPlayerCnt = Singleton<GamePlayerCenter>.instance.GetAllPlayers().get_Count();
				cSPkg.stPkgData.stCltPerformance.chModelLOD = (sbyte)GameSettings.ModelLOD;
				cSPkg.stPkgData.stCltPerformance.chParticleLOD = (sbyte)GameSettings.ParticleLOD;
				cSPkg.stPkgData.stCltPerformance.chCameraHeight = (sbyte)GameSettings.CameraHeight;
				cSPkg.stPkgData.stCltPerformance.chEnableOutline = (GameSettings.EnableOutline ? 1 : 0);
				cSPkg.stPkgData.stCltPerformance.iFps10PercentNum = iFps10PercentNum;
				cSPkg.stPkgData.stCltPerformance.iFps18PercentNum = iFps18PercentNum;
				cSPkg.stPkgData.stCltPerformance.iAveFps = Singleton<DataReportSys>.GetInstance().FPSAVE;
				cSPkg.stPkgData.stCltPerformance.iPingAverage = Singleton<DataReportSys>.GetInstance().HeartPingAve;
				cSPkg.stPkgData.stCltPerformance.iPingVariance = Singleton<DataReportSys>.GetInstance().HeartPingVar;
				Utility.StringToByteArray(SystemInfo.deviceModel, ref cSPkg.stPkgData.stCltPerformance.szDeviceModel);
				Utility.StringToByteArray(SystemInfo.graphicsDeviceName, ref cSPkg.stPkgData.stCltPerformance.szGPUName);
				cSPkg.stPkgData.stCltPerformance.iCpuCoreNum = SystemInfo.processorCount;
				cSPkg.stPkgData.stCltPerformance.iSysMemorySize = SystemInfo.systemMemorySize;
				cSPkg.stPkgData.stCltPerformance.iAvailMemory = DeviceCheckSys.GetAvailMemory();
				Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
			}
			catch (Exception ex)
			{
				Debug.Log(ex.get_Message());
			}
		}
	}
}
