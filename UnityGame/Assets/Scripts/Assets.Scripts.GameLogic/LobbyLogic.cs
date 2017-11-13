using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.GameSystem;
using Assets.Scripts.Sound;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public class LobbyLogic : Singleton<LobbyLogic>, IUpdateLogic
	{
		protected delegate void SendMessageDeletageType(ref CSPkg msg);

		public delegate void SendMsgFailNetErrorCallback();

		public bool isLogin;

		public bool inMultiRoom;

		public bool inMultiGame;

		public uint uPlayerID;

		public ulong ulAccountUid;

		private bool m_bShouldNewbieEnterHall;

		private bool m_bShowNewbieEnterTaskForm;

		private bool m_bShowNewbieEnterExploreForm;

		private bool m_bShowNewbieEnterSymbolForm;

		public bool NeedUpdateClient;

		private int _settleMsgTimer;

		private int _gameEndTimer;

		private int _settlePanelTimer;

		public CSDT_RECONN_GAMEINGINFO reconnGameInfo;

		private string[] _lobbyOpenFormCmd;

		public bool CanShowRolling
		{
			get;
			private set;
		}

		public override void Init()
		{
			this.isLogin = false;
			this.inMultiRoom = false;
			this._settleMsgTimer = 0;
			this._gameEndTimer = 0;
			this._settlePanelTimer = 0;
			this.reconnGameInfo = null;
			CUIManager instance = Singleton<CUIManager>.GetInstance();
			instance.onFormSorted = (CUIManager.OnFormSorted)Delegate.Combine(instance.onFormSorted, new CUIManager.OnFormSorted(this.OnUiFormUpdate));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Net_SingleGameFinishError, new CUIEventManager.OnUIEventHandler(this.OnClickSingleGameFinishError));
		}

		public override void UnInit()
		{
			base.UnInit();
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Net_SingleGameFinishError, new CUIEventManager.OnUIEventHandler(this.OnClickSingleGameFinishError));
		}

		public bool ConnectServer()
		{
			if (Singleton<NetworkModule>.GetInstance().isOnlineMode && !this.isLogin)
			{
				Singleton<NetworkModule>.GetInstance().lobbySvr.ConnectedEvent -= new NetConnectedEvent(this.onLobbyConnected);
				Singleton<NetworkModule>.GetInstance().lobbySvr.DisconnectEvent -= new NetDisconnectEvent(this.onLobbyDisconnected);
				Singleton<NetworkModule>.GetInstance().lobbySvr.ConnectedEvent += new NetConnectedEvent(this.onLobbyConnected);
				Singleton<NetworkModule>.GetInstance().lobbySvr.DisconnectEvent += new NetDisconnectEvent(this.onLobbyDisconnected);
				ConnectorParam connectorParam = new ConnectorParam();
				connectorParam.url = ApolloConfig.loginUrl;
				connectorParam.ip = ApolloConfig.loginOnlyIpOrUrl;
				connectorParam.vPort = ApolloConfig.loginOnlyVPort;
				return Singleton<NetworkModule>.GetInstance().InitLobbyServerConnect(connectorParam);
			}
			return false;
		}

		public void OpenLobby()
		{
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Lobby_OpenLobbyForm);
			Singleton<GameInput>.GetInstance().ChangeLobbyMode();
			Singleton<CChatController>.GetInstance();
			Singleton<CChatController>.GetInstance().CreateForm();
			Singleton<CChatController>.GetInstance().ShowPanel(true, false);
			Singleton<CChatController>.instance.model.channelMgr.Clear(EChatChannel.Room, 0uL, 0u);
			Singleton<CChatController>.instance.model.channelMgr.SetChatTab(CChatChannelMgr.EChatTab.Normal);
			Singleton<CChatController>.GetInstance().bSendChat = true;
			Singleton<CLoudSpeakerSys>.instance.StartReqTimer();
			MonoSingleton<ShareSys>.GetInstance().SendShareDataMsg();
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Lobby_PrepareFight_Sub);
			this.ExecOpenForm(null);
		}

		private void OnUiFormUpdate(ListView<CUIFormScript> inForms)
		{
			if (inForms == null)
			{
				return;
			}
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CLobbySystem.LOBBY_FORM_PATH);
			CUIFormScript form2 = Singleton<CUIManager>.GetInstance().GetForm(CLobbySystem.SYSENTRY_FORM_PATH);
			CUIFormScript form3 = Singleton<CUIManager>.GetInstance().GetForm(CChatController.ChatFormPath);
			CUIFormScript form4 = Singleton<CUIManager>.GetInstance().GetForm(CChatController.ChatEntryPath);
			CUIFormScript form5 = Singleton<CUIManager>.GetInstance().GetForm(CUICommonSystem.FPS_FORM_PATH);
			CUIFormScript form6 = Singleton<CUIManager>.GetInstance().GetForm(CUIParticleSystem.s_qualifyingFormPath);
			CUIFormScript form7 = Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Dialog/Form_WeakGuide");
			CUIFormScript form8 = Singleton<CUIManager>.GetInstance().GetForm(CFunctionUnlockSys.FUC_UNLOCK_FORM_PATH);
			CUIFormScript form9 = Singleton<CUIManager>.GetInstance().GetForm(NewbieGuideScriptControl.FormGuideMaskPath);
			CUIFormScript form10 = Singleton<CUIManager>.GetInstance().GetForm(CUICommonSystem.FORM_MESSAGE_BOX);
			CUIFormScript form11 = Singleton<CUIManager>.GetInstance().GetForm(CUICommonSystem.FORM_SENDING_ALERT);
			CUIFormScript form12 = Singleton<CUIManager>.instance.GetForm(CTaskSys.TASK_FORM_PATH);
			CUIFormScript form13 = Singleton<CUIManager>.instance.GetForm(CAdventureSys.EXLPORE_FORM_PATH);
			CUIFormScript form14 = Singleton<CUIManager>.instance.GetForm(CSymbolSystem.s_symbolFormPath);
			CUIFormScript form15 = Singleton<CUIManager>.GetInstance().GetForm(CLobbySystem.RANKING_BTN_FORM_PATH);
			ListView<CUIFormScript> listView = new ListView<CUIFormScript>();
			if (form != null)
			{
				listView.Add(form);
			}
			if (form2 != null)
			{
				listView.Add(form2);
			}
			if (form3 != null)
			{
				listView.Add(form3);
			}
			if (form4 != null)
			{
				listView.Add(form4);
			}
			if (form5 != null)
			{
				listView.Add(form5);
			}
			if (form6 != null)
			{
				listView.Add(form6);
			}
			if (form7 != null)
			{
				listView.Add(form7);
			}
			if (form15 != null)
			{
				listView.Add(form15);
			}
			bool flag = true;
			ListView<CUIFormScript>.Enumerator enumerator = inForms.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (!listView.Contains(enumerator.Current))
				{
					flag = false;
					break;
				}
			}
			if (flag && inForms.Contains(form) && inForms.Contains(form2) && !inForms.Contains(form8) && Singleton<GameStateCtrl>.GetInstance().isLobbyState)
			{
				this.m_bShouldNewbieEnterHall = true;
			}
			else
			{
				this.m_bShouldNewbieEnterHall = false;
			}
			if (this.m_bShouldNewbieEnterHall)
			{
				CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
				if (masterRoleInfo != null && masterRoleInfo.IsOldPlayer() && !masterRoleInfo.IsOldPlayerGuided())
				{
					Singleton<CBattleGuideManager>.instance.OpenOldPlayerFirstForm();
				}
			}
			if (form9 != null)
			{
				listView.Add(form9);
			}
			flag = true;
			enumerator = inForms.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (!listView.Contains(enumerator.Current))
				{
					flag = false;
					break;
				}
			}
			if (flag && inForms.Contains(form) && inForms.Contains(form2) && !inForms.Contains(form8) && Singleton<GameStateCtrl>.GetInstance().isLobbyState && Singleton<LobbySvrMgr>.GetInstance().isLogin)
			{
				Singleton<EventRouter>.instance.BroadCastEvent("FucUnlockConditionChanged");
			}
			if (form10 != null)
			{
				listView.Add(form10);
			}
			if (form11 != null)
			{
				listView.Add(form11);
			}
			flag = true;
			enumerator = inForms.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (!listView.Contains(enumerator.Current))
				{
					flag = false;
					break;
				}
			}
			if (flag && inForms.Contains(form) && inForms.Contains(form2) && Singleton<GameStateCtrl>.GetInstance().isLobbyState)
			{
				if (!this.CanShowRolling)
				{
					this.CanShowRolling = true;
					Singleton<CRollingSystem>.GetInstance().StartRolling();
				}
				Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.LOBBY_PURE_LOBBY_SHOW);
			}
			else if (this.CanShowRolling)
			{
				this.CanShowRolling = false;
				Singleton<CRollingSystem>.GetInstance().StopRolling();
			}
			ListView<CUIFormScript> listView2 = new ListView<CUIFormScript>();
			listView2.Add(form6);
			listView2.Add(form5);
			listView2.Add(form7);
			listView2.Add(form2);
			this.m_bShowNewbieEnterTaskForm = LobbyLogic.TestTopFormCustom(form12, listView2);
			this.m_bShowNewbieEnterExploreForm = LobbyLogic.TestTopFormCustom(form13, listView2);
			this.m_bShowNewbieEnterSymbolForm = LobbyLogic.TestTopFormCustom(form14, listView2);
		}

		private static bool TestTopFormCustom(CUIFormScript inTesterForm, ListView<CUIFormScript> inExcludeForms)
		{
			if (inTesterForm == null)
			{
				return false;
			}
			bool result = false;
			ListView<CUIFormScript> listView = new ListView<CUIFormScript>();
			ListView<CUIFormScript> forms = Singleton<CUIManager>.instance.GetForms();
			if (forms != null)
			{
				listView.AddRange(forms);
			}
			if (listView.Count > 0)
			{
				if (inExcludeForms != null && inExcludeForms.Count > 0)
				{
					ListView<CUIFormScript>.Enumerator enumerator = inExcludeForms.GetEnumerator();
					while (enumerator.MoveNext())
					{
						listView.Remove(enumerator.Current);
					}
				}
				if (listView.Contains(inTesterForm))
				{
					CUIFormScript cUIFormScript = null;
					int count = listView.Count;
					for (int i = 0; i < count; i++)
					{
						if (!(listView[i] == null))
						{
							if (cUIFormScript == null)
							{
								cUIFormScript = listView[i];
							}
							else if (listView[i].GetSortingOrder() > cUIFormScript.GetSortingOrder())
							{
								cUIFormScript = listView[i];
							}
						}
					}
					if (cUIFormScript == inTesterForm)
					{
						result = true;
					}
				}
			}
			return result;
		}

		public void LateUpdate()
		{
			if (Singleton<BattleLogic>.GetInstance().isRuning)
			{
				return;
			}
			if (this.m_bShouldNewbieEnterHall)
			{
				MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.enterHall, new uint[0]);
				CLobbySystem.AutoPopAllow = !MonoSingleton<NewbieGuideManager>.GetInstance().isNewbieGuiding;
				this.m_bShouldNewbieEnterHall = false;
			}
			if (this.m_bShowNewbieEnterTaskForm)
			{
				MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.onEnterTaskForm, new uint[0]);
				this.m_bShowNewbieEnterTaskForm = false;
			}
			if (this.m_bShowNewbieEnterExploreForm)
			{
				MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.onEnterExploreForm, new uint[0]);
				this.m_bShowNewbieEnterExploreForm = false;
			}
			if (this.m_bShowNewbieEnterSymbolForm)
			{
				MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.onEnterSymbolForm, new uint[0]);
				this.m_bShowNewbieEnterSymbolForm = false;
			}
		}

		public void UpdateLogic(int delta)
		{
		}

		private void onLobbyConnected(object sender)
		{
			Debug.Log("onLobbyConnected");
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
		}

		private void onLobbyDisconnected(object sender)
		{
			Debug.Log("onLobbyDisconnected");
		}

		public void StartSettleTimers()
		{
			this.StopSettleMsgTimer();
			this._settleMsgTimer = Singleton<CTimerManager>.GetInstance().AddTimer(30000, 1, new CTimer.OnTimeUpHandler(this.onSettleMsgTimer));
			this.StopGameEndTimer();
			this._gameEndTimer = Singleton<CTimerManager>.GetInstance().AddTimer(25000, 1, new CTimer.OnTimeUpHandler(this.onGameEndTimer));
			this.StopSettlePanelTimer();
		}

		public void StopSettleMsgTimer()
		{
			Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref this._settleMsgTimer);
		}

		private void onSettleMsgTimer(int seq)
		{
			Singleton<GameBuilder>.GetInstance().EndGame();
			Singleton<CUIManager>.GetInstance().OpenTips("disconWithServer", true, 2f, null, new object[0]);
		}

		public void StopGameEndTimer()
		{
			Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref this._gameEndTimer);
		}

		private void onGameEndTimer(int seq)
		{
			DefaultGameEventParam defaultGameEventParam = default(DefaultGameEventParam);
			Singleton<BattleLogic>.GetInstance().onGameEnd(ref defaultGameEventParam);
		}

		public void StartSettlePanelTimer()
		{
			this.StopSettlePanelTimer();
			this._settlePanelTimer = Singleton<CTimerManager>.GetInstance().AddTimer(20000, 1, new CTimer.OnTimeUpHandler(this.onSettlePanelTimer));
		}

		public void StopSettlePanelTimer()
		{
			Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref this._settlePanelTimer);
		}

		private void onSettlePanelTimer(int seq)
		{
			Singleton<GameBuilder>.GetInstance().EndGame();
			Singleton<CUIManager>.GetInstance().OpenTips("cannotShowSettle", true, 2f, null, new object[0]);
		}

		public void CreateLocalPlayer(uint playerID, ulong ullUplayerID, int logicWorldID = 0)
		{
			Singleton<CRoleInfoManager>.GetInstance().SetMaterUUID(ullUplayerID);
			Singleton<CRoleInfoManager>.GetInstance().CreateRoleInfo(enROLEINFO_TYPE.PLAYER, ullUplayerID, logicWorldID);
		}

		public void LoginGame()
		{
			if (!this.isLogin)
			{
				Singleton<GameLogic>.GetInstance().ClearLogicData();
				string[] exceptFormNames = new string[]
				{
					CLoginSystem.s_splashFormPath,
					CLoginSystem.sLoginFormPath,
					CLobbySystem.LOBBY_FORM_PATH,
					CLobbySystem.SYSENTRY_FORM_PATH,
					CChatController.ChatFormPath,
					CLobbySystem.RANKING_BTN_FORM_PATH
				};
				Singleton<CUIManager>.GetInstance().CloseAllForm(exceptFormNames, true, true);
				CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1002u);
				uint versionNumber = CVersion.GetVersionNumber(GameFramework.AppVersion);
				cSPkg.stPkgData.stGameLoginReq.iCltAppVersion = (int)versionNumber;
				uint versionNumber2 = CVersion.GetVersionNumber(CVersion.GetUsedResourceVersion());
				cSPkg.stPkgData.stGameLoginReq.iCltResVersion = (int)versionNumber2;
				string buildNumber = CVersion.GetBuildNumber();
				Utility.StringToByteArray(buildNumber, ref cSPkg.stPkgData.stGameLoginReq.stClientInfo.szCltBuildNumber);
				string revisonNumber = CVersion.GetRevisonNumber();
				Utility.StringToByteArray(revisonNumber, ref cSPkg.stPkgData.stGameLoginReq.stClientInfo.szCltSvnVersion);
				Utility.StringToByteArray(SystemInfo.deviceName + " " + SystemInfo.deviceModel, ref cSPkg.stPkgData.stGameLoginReq.stClientInfo.szSystemHardware);
				Utility.StringToByteArray(SystemInfo.operatingSystem, ref cSPkg.stPkgData.stGameLoginReq.stClientInfo.szSystemSoftware);
				NetworkReachability internetReachability = Application.internetReachability;
				if (internetReachability != NetworkReachability.ReachableViaCarrierDataNetwork)
				{
					if (internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
					{
						Utility.StringToByteArray("Wi-Fi/LAN", ref cSPkg.stPkgData.stGameLoginReq.stClientInfo.szNetwork);
					}
				}
				else
				{
					Utility.StringToByteArray("2G/3G", ref cSPkg.stPkgData.stGameLoginReq.stClientInfo.szNetwork);
				}
				Utility.StringToByteArray(GameFramework.AppVersion, ref cSPkg.stPkgData.stGameLoginReq.stClientInfo.szClientVersion);
				ulong ullVisitorUid = 0uL;
				if (PlayerPrefs.HasKey("visitorUid"))
				{
					string @string = PlayerPrefs.GetString("visitorUid", "NotFound");
					if (@string != "NotFound")
					{
						ullVisitorUid = ulong.Parse(@string);
					}
				}
				cSPkg.stPkgData.stGameLoginReq.ullVisitorUid = ullVisitorUid;
				cSPkg.stPkgData.stGameLoginReq.stClientInfo.iLoginChannel = MonoSingleton<IDIPSys>.GetInstance().m_ChannelID;
				cSPkg.stPkgData.stGameLoginReq.stClientInfo.iMemorySize = SystemInfo.systemMemorySize;
				cSPkg.stPkgData.stGameLoginReq.iLogicWorldID = MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID;
				cSPkg.stPkgData.stGameLoginReq.dwTaccZoneID = (uint)MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.nodeID;
				cSPkg.stPkgData.stGameLoginReq.bPrivilege = (byte)Singleton<ApolloHelper>.GetInstance().GetCurrentLoginPrivilege();
				byte[] bytes = Encoding.get_ASCII().GetBytes(SystemInfo.deviceUniqueIdentifier);
				if (bytes.Length > 0)
				{
					Buffer.BlockCopy(bytes, 0, cSPkg.stPkgData.stGameLoginReq.szCltIMEI, 0, (bytes.Length > 64) ? 64 : bytes.Length);
				}
				Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
			}
		}

		public static void ReqStartGuideLevel11(bool bReEntry, uint heroType = 0u)
		{
			if (heroType == 0u)
			{
				heroType = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_NEWBIE_RECOMMEND_HEROTYPE1);
			}
			uint heroID = 0u;
			int inGuideLevelId = 0;
			if (CBattleGuideManager.GetGuide1v1HeroIDAndLevelID(heroType, out heroID, out inGuideLevelId))
			{
				Singleton<LobbyLogic>.GetInstance().ReqStartGuideLevel(inGuideLevelId, heroID);
				CLobbySystem.AutoPopAllow = false;
				Singleton<CBattleGuideManager>.instance.bReEntry = bReEntry;
			}
		}

		public static void ReqStartGuideLevel33(bool bReEntry)
		{
			uint dwConfValue = GameDataMgr.globalInfoDatabin.GetDataByKey(144u).dwConfValue;
			int dwConfValue2 = (int)GameDataMgr.globalInfoDatabin.GetDataByKey(117u).dwConfValue;
			Singleton<LobbyLogic>.GetInstance().ReqStartGuideLevel(dwConfValue2, dwConfValue);
			CLobbySystem.AutoPopAllow = false;
			Singleton<CBattleGuideManager>.instance.bReEntry = bReEntry;
			Singleton<CBattleGuideManager>.instance.bTrainingAdv = true;
		}

		public static void ReqStartGuideLevel55(bool bReEntry, uint heroType = 0u)
		{
			if (heroType == 0u)
			{
				heroType = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_NEWBIE_RECOMMEND_HEROTYPE1);
			}
			uint heroID = 0u;
			int inGuideLevelId = 0;
			if (CBattleGuideManager.GetGuide5v5HeroIDAndLevelID(heroType, out heroID, out inGuideLevelId))
			{
				Singleton<LobbyLogic>.GetInstance().ReqStartGuideLevel(inGuideLevelId, heroID);
				CLobbySystem.AutoPopAllow = false;
				Singleton<CBattleGuideManager>.instance.bReEntry = bReEntry;
				Singleton<CBattleGuideManager>.instance.bTrainingAdv = true;
			}
		}

		public static void ReqStartGuideLevelCasting(bool bReEntry)
		{
			uint dwConfValue = GameDataMgr.globalInfoDatabin.GetDataByKey(146u).dwConfValue;
			int dwConfValue2 = (int)GameDataMgr.globalInfoDatabin.GetDataByKey(119u).dwConfValue;
			Singleton<LobbyLogic>.GetInstance().ReqStartGuideLevel(dwConfValue2, dwConfValue);
			CLobbySystem.AutoPopAllow = false;
			Singleton<CBattleGuideManager>.instance.bReEntry = bReEntry;
			Singleton<CBattleGuideManager>.instance.bTrainingAdv = true;
		}

		public static void ReqStartGuideLevelJungle(bool bReEntry)
		{
			uint dwConfValue = GameDataMgr.globalInfoDatabin.GetDataByKey(115u).dwConfValue;
			int dwConfValue2 = (int)GameDataMgr.globalInfoDatabin.GetDataByKey(121u).dwConfValue;
			Singleton<LobbyLogic>.GetInstance().ReqStartGuideLevel(dwConfValue2, dwConfValue);
			CLobbySystem.AutoPopAllow = false;
			Singleton<CBattleGuideManager>.instance.bReEntry = bReEntry;
			Singleton<CBattleGuideManager>.instance.bTrainingAdv = true;
		}

		public static void ReqStartGuideLevelSelHero(bool bReEntry, int levelId)
		{
			ResLevelCfgInfo dataByKey = GameDataMgr.levelDatabin.GetDataByKey((long)levelId);
			DebugHelper.Assert(dataByKey != null, "Can't find level config -- ID: {0}", new object[]
			{
				levelId
			});
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			DebugHelper.Assert(masterRoleInfo != null, "Master role info is NULL!");
			if (Singleton<CMatchingSystem>.GetInstance().IsInMatching)
			{
				Singleton<CUIManager>.GetInstance().OpenTips("PVP_Matching", true, 1.5f, null, new object[0]);
				return;
			}
			Singleton<CBattleGuideManager>.instance.bTrainingAdv = true;
			Singleton<CBattleGuideManager>.instance.bReEntry = bReEntry;
			Singleton<CAdventureSys>.instance.currentLevelId = levelId;
			Singleton<CAdventureSys>.instance.currentDifficulty = 1;
			CSDT_SINGLE_GAME_OF_ADVENTURE cSDT_SINGLE_GAME_OF_ADVENTURE = new CSDT_SINGLE_GAME_OF_ADVENTURE();
			cSDT_SINGLE_GAME_OF_ADVENTURE.iLevelID = dataByKey.iCfgID;
			cSDT_SINGLE_GAME_OF_ADVENTURE.bChapterNo = (byte)dataByKey.iChapterId;
			cSDT_SINGLE_GAME_OF_ADVENTURE.bLevelNo = dataByKey.bLevelNo;
			cSDT_SINGLE_GAME_OF_ADVENTURE.bDifficultType = 1;
			Singleton<CHeroSelectBaseSystem>.instance.SetPVEDataWithAdventure(dataByKey.dwBattleListID, cSDT_SINGLE_GAME_OF_ADVENTURE, StringHelper.UTF8BytesToString(ref dataByKey.szName));
			Singleton<CHeroSelectBaseSystem>.instance.OpenForm(enSelectGameType.enGuide, (byte)dataByKey.iHeroNum, 0u, 0, 0);
		}

		public void ReqStartGuideLevel(int inGuideLevelId, uint heroID)
		{
			if (Singleton<CMatchingSystem>.GetInstance().IsInMatching)
			{
				Singleton<CUIManager>.GetInstance().OpenTips("PVP_Matching", true, 1.5f, null, new object[0]);
				return;
			}
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1050u);
			if (GameDataMgr.levelDatabin.GetDataByKey((long)inGuideLevelId) == null)
			{
				Debug.LogError("ResLevelCfgInfo can not find id = " + inGuideLevelId);
			}
			cSPkg.stPkgData.stStartSingleGameReq.stBattleParam.bGameType = 2;
			cSPkg.stPkgData.stStartSingleGameReq.stBattleParam.stGameDetail.construct(2L);
			cSPkg.stPkgData.stStartSingleGameReq.stBattleParam.stGameDetail.stGameOfGuide.iLevelID = inGuideLevelId;
			cSPkg.stPkgData.stStartSingleGameReq.stBattleList.stBattleList.wHeroCnt = 1;
			cSPkg.stPkgData.stStartSingleGameReq.stBattleList.stBattleList.BattleHeroList[0] = heroID;
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			masterRoleInfo.battleHeroList.Clear();
			masterRoleInfo.battleHeroList.Add(heroID);
			ResLevelCfgInfo dataByKey = GameDataMgr.levelDatabin.GetDataByKey((long)inGuideLevelId);
			DebugHelper.Assert(dataByKey != null, "Can't find level config with ID -- {0}", new object[]
			{
				inGuideLevelId
			});
			CSDT_BATTLE_PLAYER_BRIEF stBattlePlayer = cSPkg.stPkgData.stStartSingleGameReq.stBattlePlayer;
			stBattlePlayer.astFighter[0].bPosOfCamp = 0;
			stBattlePlayer.astFighter[0].bObjType = 1;
			stBattlePlayer.astFighter[0].bObjCamp = 1;
			stBattlePlayer.astFighter[0].astChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID = heroID;
			int num = 1;
			for (int i = 0; i < dataByKey.SelfCampAIHeroID.Length; i++)
			{
				if (dataByKey.SelfCampAIHeroID[i] != 0u)
				{
					stBattlePlayer.astFighter[num].bPosOfCamp = (byte)(i + 1);
					stBattlePlayer.astFighter[num].bObjType = 2;
					stBattlePlayer.astFighter[num].bObjCamp = 1;
					stBattlePlayer.astFighter[num].astChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID = dataByKey.SelfCampAIHeroID[i];
					num++;
				}
			}
			for (int j = 0; j < dataByKey.AIHeroID.Length; j++)
			{
				if (dataByKey.AIHeroID[j] != 0u)
				{
					stBattlePlayer.astFighter[num].bPosOfCamp = (byte)j;
					stBattlePlayer.astFighter[num].bObjType = 2;
					stBattlePlayer.astFighter[num].bObjCamp = 2;
					stBattlePlayer.astFighter[num].astChoiceHero[0].stBaseInfo.stCommonInfo.dwHeroID = dataByKey.AIHeroID[j];
					num++;
				}
			}
			stBattlePlayer.bNum = (byte)num;
			if (CBattleGuideManager.Is1v1GuideLevel(inGuideLevelId))
			{
				Singleton<CSoundManager>.instance.LoadBank("Newguide_Voice", CSoundManager.BankType.Battle);
			}
			else if ((long)inGuideLevelId == (long)((ulong)CBattleGuideManager.GuideLevelID3v3))
			{
				Singleton<CSoundManager>.instance.LoadBank("Newguide_3V3_Voice", CSoundManager.BankType.Battle);
			}
			else if (CBattleGuideManager.Is5v5GuideLevel(inGuideLevelId))
			{
				Singleton<CSoundManager>.instance.LoadBank("Newguide_5V5_Voice", CSoundManager.BankType.Battle);
			}
			else if ((long)inGuideLevelId == (long)((ulong)CBattleGuideManager.GuideLevelIDCasting))
			{
				Singleton<CSoundManager>.instance.LoadBank("Newguide_Voice_Rotate", CSoundManager.BankType.Battle);
			}
			else if ((long)inGuideLevelId == (long)((ulong)CBattleGuideManager.GuideLevelIDJungle))
			{
				Singleton<CSoundManager>.instance.LoadBank("Newguide_Voice_WildMonster", CSoundManager.BankType.Battle);
			}
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
			Singleton<WatchController>.GetInstance().Stop();
		}

		private COMDT_SINGLE_COMBAT_ROBOT_DETAIL CreateSingleGameRobotData(bool bGmWin, byte byResult)
		{
			return new COMDT_SINGLE_COMBAT_ROBOT_DETAIL();
		}

		public void ReqSingleGameFinish(bool ClickGameOver = false, bool bGmWin = false)
		{
			if (this.isLogin)
			{
				BattleStatistic battleStat = Singleton<BattleLogic>.GetInstance().battleStat;
				SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
				if (curLvelContext == null)
				{
					return;
				}
				byte b;
				if (bGmWin)
				{
					b = 1;
				}
				else if (curLvelContext.IsMobaMode() && !curLvelContext.IsFireHolePlayMode())
				{
					b = (Singleton<WinLose>.instance.LastSingleGameWin ? 1 : 2);
				}
				else if (Singleton<StarSystem>.instance.isFailure)
				{
					b = 2;
				}
				else
				{
					b = (Singleton<WinLose>.instance.LastSingleGameWin ? 1 : 2);
				}
				CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1052u);
				cSPkg.stPkgData.stFinSingleGameReq.stBattleParam.bGameType = (byte)curLvelContext.GetGameType();
				cSPkg.stPkgData.stFinSingleGameReq.stBattleParam.iLevelID = curLvelContext.m_mapID;
				cSPkg.stPkgData.stFinSingleGameReq.stBattleParam.dwUsedTime = (uint)Singleton<FrameSynchr>.GetInstance().LogicFrameTick / 1000u;
				cSPkg.stPkgData.stFinSingleGameReq.stBattleParam.bGameResult = b;
				cSPkg.stPkgData.stFinSingleGameReq.stCommonData = this.CreateReportData(Singleton<GamePlayerCenter>.instance.GetHostPlayer().PlayerId, bGmWin, b == 1);
				if (curLvelContext.IsGameTypeAdventure())
				{
					cSPkg.stPkgData.stFinSingleGameReq.stBattleParam.stGameDetail.stAdventure = new COMDT_SINGLE_GAME_PARAM_ADVENTURE();
					cSPkg.stPkgData.stFinSingleGameReq.stBattleParam.stGameDetail.stAdventure.bChapterNo = (byte)curLvelContext.m_chapterNo;
					cSPkg.stPkgData.stFinSingleGameReq.stBattleParam.stGameDetail.stAdventure.bLevelNo = curLvelContext.m_levelNo;
					cSPkg.stPkgData.stFinSingleGameReq.stBattleParam.stGameDetail.stAdventure.bDifficultType = (byte)curLvelContext.m_levelDifficulty;
				}
				else if (curLvelContext.IsGameTypeBurning())
				{
					BurnExpeditionUT.Build_Burn_BattleParam(cSPkg.stPkgData.stFinSingleGameReq.stBattleParam, ClickGameOver);
				}
				else if (curLvelContext.IsGameTypeComBat())
				{
					cSPkg.stPkgData.stFinSingleGameReq.stBattleParam.stGameDetail.stCombat = this.CreateSingleGameRobotData(bGmWin, b);
				}
				if (!bGmWin)
				{
					if (ClickGameOver)
					{
						cSPkg.stPkgData.stFinSingleGameReq.bPressExit = 1;
						battleStat.iBattleResult = 2;
					}
					else
					{
						cSPkg.stPkgData.stFinSingleGameReq.bPressExit = 0;
					}
				}
				Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().SyncNewbieAchieveToSvr(false);
				Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
			}
		}

		public void ReqMultiGameOver(bool ClickGameOver = false)
		{
			BattleStatistic battleStat = Singleton<BattleLogic>.GetInstance().battleStat;
			if (this.isLogin && battleStat != null)
			{
				if (ClickGameOver)
				{
					battleStat.iBattleResult = 2;
				}
				List<Player> list = new List<Player>(Singleton<GamePlayerCenter>.instance.GetAllPlayers());
				COM_PLAYERCAMP playerCamp = Singleton<GamePlayerCenter>.instance.GetHostPlayer().PlayerCamp;
				int iBattleResult = battleStat.iBattleResult;
				int num;
				int num2;
				if (iBattleResult != 1)
				{
					if (iBattleResult != 2)
					{
						num = 3;
						num2 = 3;
					}
					else if (playerCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
					{
						num = 2;
						num2 = 1;
					}
					else
					{
						num = 1;
						num2 = 2;
					}
				}
				else if (playerCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
				{
					num = 1;
					num2 = 2;
				}
				else
				{
					num = 2;
					num2 = 1;
				}
				this.GenerateStatData();
				int num3 = 0;
				CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1080u);
				for (int i = 0; i < list.get_Count(); i++)
				{
					if (!list.get_Item(i).Computer)
					{
						if (list.get_Item(i).PlayerCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
						{
							cSPkg.stPkgData.stMultGameOverReq.astAcntInfo[num3].stBattleParam.bGameResult = (byte)num;
						}
						else
						{
							cSPkg.stPkgData.stMultGameOverReq.astAcntInfo[num3].stBattleParam.bGameResult = (byte)num2;
						}
						if (list.get_Item(i) == Singleton<GamePlayerCenter>.instance.GetHostPlayer())
						{
							cSPkg.stPkgData.stMultGameOverReq.astAcntInfo[num3].stBattleParam.wNetworkMode = (ushort)this.CheckWifi();
							Singleton<DataReportSys>.GetInstance().Report(ref cSPkg.stPkgData.stMultGameOverReq.stClientRecordData, list.get_Item(i).PlayerId);
						}
						else
						{
							cSPkg.stPkgData.stMultGameOverReq.astAcntInfo[num3].stBattleParam.wNetworkMode = 0;
						}
						bool flag = cSPkg.stPkgData.stMultGameOverReq.astAcntInfo[num3].stBattleParam.bGameResult == 1;
						cSPkg.stPkgData.stMultGameOverReq.astAcntInfo[num3].dwAcntObjID = list.get_Item(i).PlayerId;
						cSPkg.stPkgData.stMultGameOverReq.astAcntInfo[num3].stCommonData = this.CreateReportData(list.get_Item(i).PlayerId, flag, flag);
						num3++;
						if (num3 > 4)
						{
							cSPkg.stPkgData.stMultGameOverReq.bAcntNum = (byte)num3;
							Singleton<NetworkModule>.GetInstance().SendGameMsg(ref cSPkg, 1081u);
							num3 = 0;
							cSPkg = NetworkModule.CreateDefaultCSPKG(1080u);
						}
					}
				}
				if (num3 > 0)
				{
					cSPkg.stPkgData.stMultGameOverReq.bAcntNum = (byte)num3;
					Singleton<NetworkModule>.GetInstance().SendGameMsg(ref cSPkg, 1081u);
				}
			}
		}

		private int CheckWifi()
		{
			if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
			{
				return 1;
			}
			if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
			{
				return 2;
			}
			return 0;
		}

		public void ReqQuitMultiGame()
		{
			if (this.isLogin && this.inMultiRoom)
			{
				CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1023u);
				Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
			}
		}

		public void ReqStartMultiGame()
		{
			if (this.isLogin && this.inMultiRoom)
			{
				if (!Singleton<BattleLogic>.instance.isWaitMultiStart)
				{
					CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1076u);
					Singleton<NetworkModule>.GetInstance().SendGameMsg(ref cSPkg, 0u);
				}
				else
				{
					Singleton<BattleLogic>.instance.StartFightMultiGame();
				}
			}
		}

		public void ReqMultiGameRunaway()
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1086u);
			cSPkg.stPkgData.stRunAwayReq = new CSPKG_MULTGAME_RUNAWAY_REQ();
			Singleton<NetworkModule>.instance.SendGameMsg(ref cSPkg, 0u);
		}

		public void ReqSelectHero(uint id)
		{
			if (this.isLogin)
			{
				CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1800u);
				cSPkg.stPkgData.stAchieveHeroReq.dwHeroId = id;
				Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
			}
		}

		private COMDT_SETTLE_COMMON_DATA CreateReportData(uint playerId, bool bGMWin = false, bool bWin = false)
		{
			COMDT_SETTLE_COMMON_DATA cOMDT_SETTLE_COMMON_DATA = new COMDT_SETTLE_COMMON_DATA();
			this.CreateHeroData(playerId, ref cOMDT_SETTLE_COMMON_DATA);
			this.CreateDestroyReportData(ref cOMDT_SETTLE_COMMON_DATA, bGMWin);
			this.CreateSettleReportData(playerId, ref cOMDT_SETTLE_COMMON_DATA, bGMWin, bWin);
			this.CreateAttrReportData(ref cOMDT_SETTLE_COMMON_DATA, bGMWin);
			this.CreateBattleNonHeroData(ref cOMDT_SETTLE_COMMON_DATA, bGMWin);
			this.CreateGeneralData(playerId, ref cOMDT_SETTLE_COMMON_DATA, bGMWin);
			cOMDT_SETTLE_COMMON_DATA.stOtherData.iLatitude = MonoSingleton<GPSSys>.GetInstance().iLatitude;
			cOMDT_SETTLE_COMMON_DATA.stOtherData.iLongitude = MonoSingleton<GPSSys>.GetInstance().iLongitude;
			return cOMDT_SETTLE_COMMON_DATA;
		}

		private void VerifyReportData(ref COMDT_SETTLE_COMMON_DATA CommonData)
		{
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			DebugHelper.Assert(curLvelContext != null);
			if (curLvelContext == null)
			{
				return;
			}
			for (int i = 0; i < curLvelContext.m_starDetail.Length; i++)
			{
				int iParam = curLvelContext.m_starDetail[i].iParam;
				if (iParam == 0)
				{
					return;
				}
				ResEvaluateStarInfo dataByKey = GameDataMgr.evaluateCondInfoDatabin.GetDataByKey((uint)iParam);
				if (dataByKey != null)
				{
					COMDT_STATISTIC_STRUCT_PILE cOMDT_STATISTIC_STRUCT_PILE = null;
					for (int j = 0; j < (int)CommonData.stStatisticData.bNum; j++)
					{
						if (CommonData.stStatisticData.astDetail[j].bReportType == 1)
						{
							cOMDT_STATISTIC_STRUCT_PILE = CommonData.stStatisticData.astDetail[j];
							break;
						}
					}
					if (cOMDT_STATISTIC_STRUCT_PILE == null)
					{
						int bNum = (int)CommonData.stStatisticData.bNum;
						cOMDT_STATISTIC_STRUCT_PILE = CommonData.stStatisticData.astDetail[bNum];
						cOMDT_STATISTIC_STRUCT_PILE.bReportType = 1;
						COMDT_STATISTIC_DATA stStatisticData = CommonData.stStatisticData;
						COMDT_STATISTIC_DATA expr_D9 = stStatisticData;
						expr_D9.bNum += 1;
					}
					COMDT_STATISTIC_BASE_STRUCT[] astDetail = cOMDT_STATISTIC_STRUCT_PILE.astDetail;
					IStarEvaluation evaluationAt = Singleton<StarSystem>.instance.GetEvaluationAt(i);
					if (evaluationAt != null)
					{
						for (int k = 0; k < dataByKey.astConditions.Length; k++)
						{
							if (dataByKey.astConditions[k].dwType == 1u)
							{
								IStarCondition conditionAt = evaluationAt.GetConditionAt(k);
								DebugHelper.Assert(conditionAt != null, "StarCondition==null");
								if (conditionAt != null)
								{
									this.VerifyCondition(conditionAt, ref dataByKey.astConditions[k], ref cOMDT_STATISTIC_STRUCT_PILE, ref astDetail);
								}
							}
						}
					}
				}
			}
		}

		private void VerifyCondition(IStarCondition InCondition, ref ResDT_ConditionInfo ConditionInfo, ref COMDT_STATISTIC_STRUCT_PILE StatRef, ref COMDT_STATISTIC_BASE_STRUCT[] DetailData)
		{
			int num = ConditionInfo.KeyDetail[0];
			int num2 = ConditionInfo.KeyDetail[1];
			int num3 = ConditionInfo.KeyDetail[2];
			int[] keys = InCondition.keys;
			DebugHelper.Assert(keys.Length >= 3 && keys[0] == num && keys[1] == num2 && keys[2] == num3, "VerifyCondition Error: {0}-{1}-{2}", new object[]
			{
				num,
				num2,
				num3
			});
			for (int i = 0; i < DetailData.Length; i++)
			{
				COMDT_STATISTIC_BASE_STRUCT cOMDT_STATISTIC_BASE_STRUCT = DetailData[i];
				if (cOMDT_STATISTIC_BASE_STRUCT.stKeyInfo.bKeyNum <= 0)
				{
					break;
				}
				if (cOMDT_STATISTIC_BASE_STRUCT.stKeyInfo.KeyDetail[0] == num && cOMDT_STATISTIC_BASE_STRUCT.stKeyInfo.KeyDetail[1] == num2 && cOMDT_STATISTIC_BASE_STRUCT.stKeyInfo.KeyDetail[2] == num3)
				{
					this.VerifyValues(InCondition, ref ConditionInfo, ref cOMDT_STATISTIC_BASE_STRUCT);
					return;
				}
			}
			DebugHelper.Assert(false, "can not visit this code!");
		}

		private void VerifyValues(IStarCondition InCondition, ref ResDT_ConditionInfo ConditionInfo, ref COMDT_STATISTIC_BASE_STRUCT Data)
		{
			Singleton<StarSystem>.instance.GetEnumerator();
			int[] values = InCondition.values;
			DebugHelper.Assert(values != null && values.Length == (int)Data.stValueInfo.bValueNum, "Values!=null&&Values.Length == Data.stValueInfo.bValueNum");
			int num = 0;
			while (num < ConditionInfo.ValueDetail.Length && num < (int)Data.stValueInfo.bValueNum)
			{
				if (ConditionInfo.ValueDetail[num] == 0)
				{
					break;
				}
				int num2 = Data.stValueInfo.ValueDetail[num];
				int num3 = values[num];
				int num4 = ConditionInfo.KeyDetail[0];
				int num5 = ConditionInfo.KeyDetail[1];
				int num6 = ConditionInfo.KeyDetail[2];
				if (num2 != num3)
				{
					DebugHelper.Assert(false, "({0})({1})({2}) Report Value Missmatch", new object[]
					{
						num4,
						num5,
						num6
					});
					DebugHelper.Assert(false, "Missvalue {0} {1}", new object[]
					{
						num2,
						num3
					});
					Data.stValueInfo.ValueDetail[num] = num3;
				}
				num++;
			}
		}

		private void GenerateStatData()
		{
			Singleton<BattleStatistic>.GetInstance().m_playerKDAStat.GenerateStatData();
		}

		private void CreateHeroBaseInfoData(HeroKDA HeroInfo, ref COMDT_HERO_BASE_INFO stHeroDetailInfo)
		{
			if (HeroInfo.actorHero.handle == null || HeroInfo.actorHero.handle.ValueComponent == null)
			{
				return;
			}
			ActorValueStatistic objValueStatistic = HeroInfo.actorHero.handle.ValueComponent.ObjValueStatistic;
			if (objValueStatistic != null)
			{
				stHeroDetailInfo.iActorLvl = objValueStatistic.iActorLvl;
				stHeroDetailInfo.iActorATT = objValueStatistic.iActorATT;
				stHeroDetailInfo.iActorINT = objValueStatistic.iActorINT;
				stHeroDetailInfo.iActorMaxHp = objValueStatistic.iActorMaxHp;
				stHeroDetailInfo.iDEFStrike = objValueStatistic.iDEFStrike;
				stHeroDetailInfo.iRESStrike = objValueStatistic.iRESStrike;
				stHeroDetailInfo.iFinalHurt = objValueStatistic.iFinalHurt;
				stHeroDetailInfo.iCritStrikeRate = objValueStatistic.iCritStrikeRate;
				stHeroDetailInfo.iCritStrikeValue = objValueStatistic.iCritStrikeValue;
				stHeroDetailInfo.iReduceCritStrikeRate = objValueStatistic.iReduceCritStrikeRate;
				stHeroDetailInfo.iReduceCritStrikeValue = objValueStatistic.iReduceCritStrikeValue;
				stHeroDetailInfo.iCritStrikeEff = objValueStatistic.iCritStrikeEff;
				stHeroDetailInfo.iPhysicsHemophagiaRate = objValueStatistic.iPhysicsHemophagiaRate;
				stHeroDetailInfo.iMagicHemophagiaRate = objValueStatistic.iMagicHemophagiaRate;
				stHeroDetailInfo.iPhysicsHemophagia = objValueStatistic.iPhysicsHemophagia;
				stHeroDetailInfo.iMagicHemophagia = objValueStatistic.iMagicHemophagia;
				stHeroDetailInfo.iHurtOutputRate = objValueStatistic.iHurtOutputRate;
				stHeroDetailInfo.iMaxMoveSpeed = objValueStatistic.iMoveSpeedMax;
				stHeroDetailInfo.iMaxSoulExp = objValueStatistic.iSoulExpMax;
				stHeroDetailInfo.iTotalSoulExp = HeroInfo.actorHero.handle.ValueComponent.actorSoulExp + HeroInfo.actorHero.handle.ValueComponent.actorSoulMaxExp;
			}
			ulong logicFrameTick = Singleton<FrameSynchr>.instance.LogicFrameTick;
			if (HeroInfo.actorHero.handle.MovementComponent != null)
			{
				PlayerMovement playerMovement = HeroInfo.actorHero.handle.MovementComponent as PlayerMovement;
				uint num = (uint)(logicFrameTick - playerMovement.m_ulLastMoveEndTime);
				playerMovement.m_uiMoveIntervalMax = ((playerMovement.m_uiMoveIntervalMax > num) ? playerMovement.m_uiMoveIntervalMax : num);
				playerMovement.m_uiNonMoveTotalTime += num;
			}
		}

		private void CreateBattleStatisticInfoData(HeroKDA HeroInfo, ref COMDT_HERO_BATTLE_STATISTIC_INFO stBattleStatisticInfo)
		{
			uint num = 1u;
			uint num2 = 1u;
			uint num3 = 1u;
			uint num4 = 1u;
			COM_PLAYERCAMP actorCamp = HeroInfo.actorHero.handle.TheActorMeta.ActorCamp;
			CCampKDAStat campKdaStat = Singleton<BattleStatistic>.GetInstance().m_playerKDAStat.m_CampKdaStat;
			if (campKdaStat != null)
			{
				num = campKdaStat.GetTeamTotalDamage(actorCamp);
				num2 = campKdaStat.GetTeamTotalToHeroDamage(actorCamp);
				num3 = campKdaStat.GetTeamTotalTakenDamage(actorCamp);
				num4 = campKdaStat.GetTeamTotalTakenHeroDamage(actorCamp);
			}
			stBattleStatisticInfo.dwKillCnt = (uint)HeroInfo.numKill;
			stBattleStatisticInfo.dwDeadCnt = (uint)HeroInfo.numDead;
			stBattleStatisticInfo.dwAssistCnt = (uint)HeroInfo.numAssist;
			stBattleStatisticInfo.bDestroyBaseCnt = (byte)HeroInfo.numDestroyBase;
			stBattleStatisticInfo.bDestroyTowerCnt = (byte)HeroInfo.numKillOrgan;
			stBattleStatisticInfo.wKillMonsterCnt = (ushort)HeroInfo.numKillMonster;
			stBattleStatisticInfo.wKillFakeMonsterCnt = (ushort)HeroInfo.numKillFakeMonster;
			stBattleStatisticInfo.wKillSoldierCnt = (ushort)HeroInfo.numKillSoldier;
			stBattleStatisticInfo.dwTotalHurtCnt = (uint)HeroInfo.hurtToEnemy;
			stBattleStatisticInfo.dwTotalHurtNum = HeroInfo.HurtToEnemyNum;
			stBattleStatisticInfo.dwTotalHurtHeroCnt = (uint)HeroInfo.hurtToHero;
			stBattleStatisticInfo.dwTotalHurtHeroNum = HeroInfo.HurtToHeroNum;
			stBattleStatisticInfo.dwTotalBeHurtCnt = (uint)HeroInfo.hurtTakenByEnemy;
			stBattleStatisticInfo.dwTotalBeHurtNum = HeroInfo.TotalBeAttackNum;
			stBattleStatisticInfo.dwTotalBeChosenAsAttackTargetNum = HeroInfo.TotalBeChosenAsAttackTargetNum;
			stBattleStatisticInfo.dwTotalBeHeroHurtCnt = (uint)HeroInfo.hurtTakenByHero;
			stBattleStatisticInfo.dwTotalHealCnt = (uint)HeroInfo.beHeal;
			stBattleStatisticInfo.dwTotalBeHealNum = HeroInfo.TotalBeHealNum;
			stBattleStatisticInfo.dwTotalBeChosenAsHealTargetNum = HeroInfo.TotalBeChosenAsHealTargetNum;
			stBattleStatisticInfo.bKillDragonCnt = (byte)HeroInfo.numKillDragon;
			stBattleStatisticInfo.wKillBlueBuffCnt = (ushort)HeroInfo.numKillBlueBa;
			stBattleStatisticInfo.wKillRedBuffCnt = (ushort)HeroInfo.numKillRedBa;
			stBattleStatisticInfo.bKillBigDragonCnt = (byte)HeroInfo.numKillBaron;
			stBattleStatisticInfo.dwDoubleKillCnt = (uint)HeroInfo.DoubleKillNum;
			stBattleStatisticInfo.dwTripleKillCnt = (uint)HeroInfo.TripleKillNum;
			stBattleStatisticInfo.dwUltraKillCnt = (uint)HeroInfo.QuataryKillNum;
			stBattleStatisticInfo.dwRampageCnt = (uint)HeroInfo.PentaKillNum;
			stBattleStatisticInfo.iPhysicalHurt = HeroInfo.physHurtToHero;
			stBattleStatisticInfo.iSpellHurt = HeroInfo.magicHurtToHero;
			stBattleStatisticInfo.iRealHurt = HeroInfo.realHurtToHero;
			stBattleStatisticInfo.iHealCnt = HeroInfo.heal;
			stBattleStatisticInfo.dwTotalHurtOrganCnt = (uint)HeroInfo.hurtToOrgan;
			float num5 = Singleton<FrameSynchr>.instance.LogicFrameTick / 60000f;
			if (num5 > 0f && HeroInfo.actorHero && HeroInfo.actorHero.handle.ValueComponent != null)
			{
				stBattleStatisticInfo.iGPM = (int)((float)HeroInfo.actorHero.handle.ValueComponent.GetGoldCoinIncomeInBattle() / num5);
				stBattleStatisticInfo.iXPM = (int)((float)(HeroInfo.actorHero.handle.ValueComponent.actorSoulExp + HeroInfo.actorHero.handle.ValueComponent.actorSoulMaxExp) / num5);
			}
			stBattleStatisticInfo.iHurtPM = (int)((float)HeroInfo.hurtToHero / num5);
			stBattleStatisticInfo.wNoAIKillCnt = HeroInfo.JudgeStat.KillNum;
			stBattleStatisticInfo.wNoAIDeadCnt = HeroInfo.JudgeStat.DeadNum;
			stBattleStatisticInfo.wNoAIAssistCnt = HeroInfo.JudgeStat.AssitNum;
			stBattleStatisticInfo.dwNoAICoinCnt = (uint)HeroInfo.JudgeStat.GainCoin;
			if (num > 0u)
			{
				stBattleStatisticInfo.wNoAITotalHurtRatio = (ushort)(100u * HeroInfo.JudgeStat.HurtToAll / num);
				stBattleStatisticInfo.bTotalHurtPercent = (byte)((long)(100 * HeroInfo.hurtToEnemy) / (long)((ulong)num));
			}
			if (num2 > 0u)
			{
				stBattleStatisticInfo.wNoAITotalHurtHeroRatio = (ushort)(100u * HeroInfo.JudgeStat.HurtToHero / num2);
				stBattleStatisticInfo.bTotalHurtHeroPercent = (byte)((long)(100 * HeroInfo.hurtToHero) / (long)((ulong)num2));
			}
			if (num3 > 0u)
			{
				stBattleStatisticInfo.bTotalBeHurtPercent = (byte)((long)(100 * HeroInfo.hurtTakenByEnemy) / (long)((ulong)num3));
			}
			if (num4 > 0u)
			{
				stBattleStatisticInfo.wNoAITotalBeHeroHurtRatio = (ushort)(100u * HeroInfo.JudgeStat.SufferHero / num4);
			}
			stBattleStatisticInfo.dwLevelThreeTime = Singleton<BattleStatistic>.instance.m_playerSoulLevelStat.GetPlayerLevelChangedTime(HeroInfo.actorHero.handle.TheActorMeta.PlayerId, 3u);
			stBattleStatisticInfo.dwLevelFourTime = Singleton<BattleStatistic>.instance.m_playerSoulLevelStat.GetPlayerLevelChangedTime(HeroInfo.actorHero.handle.TheActorMeta.PlayerId, 4u);
			if (HeroInfo.actorHero && HeroInfo.actorHero.handle.SkillControl != null && HeroInfo.actorHero.handle.SkillControl.stSkillStat != null)
			{
				stBattleStatisticInfo.bEyeNum = ((HeroInfo.actorHero.handle.SkillControl.stSkillStat.m_uiSpawnEyeTimes < 15u) ? ((byte)HeroInfo.actorHero.handle.SkillControl.stSkillStat.m_uiSpawnEyeTimes) : 15);
				for (int i = 0; i < (int)stBattleStatisticInfo.bEyeNum; i++)
				{
					stBattleStatisticInfo.astEyePos[i].iTime = HeroInfo.actorHero.handle.SkillControl.stSkillStat.stEyePostion[i].iTime;
					stBattleStatisticInfo.astEyePos[i].iXPos = HeroInfo.actorHero.handle.SkillControl.stSkillStat.stEyePostion[i].iXPos;
					stBattleStatisticInfo.astEyePos[i].iZPos = HeroInfo.actorHero.handle.SkillControl.stSkillStat.stEyePostion[i].iZPos;
				}
				stBattleStatisticInfo.bFakeWardNum = (byte)HeroInfo.actorHero.handle.SkillControl.stSkillStat.m_uiRealSpawnEyeTimes;
				if (HeroInfo.actorHero.handle.SkillControl.stSkillStat.SkillStatistictInfo != null)
				{
					if (HeroInfo.actorHero.handle.SkillControl.stSkillStat.SkillStatistictInfo[7].uiUsedTimes > (uint)stBattleStatisticInfo.bFakeWardNum)
					{
						stBattleStatisticInfo.bVisionWardNum = (byte)(HeroInfo.actorHero.handle.SkillControl.stSkillStat.SkillStatistictInfo[7].uiUsedTimes - (uint)stBattleStatisticInfo.bFakeWardNum);
					}
					stBattleStatisticInfo.dwActiveEquipUseNum = HeroInfo.actorHero.handle.SkillControl.stSkillStat.SkillStatistictInfo[8].uiUsedTimes;
				}
				stBattleStatisticInfo.dwSwitchWardNum = HeroInfo.actorHero.handle.SkillControl.stSkillStat.m_uiEyeSwitchTimes;
				stBattleStatisticInfo.dwTpSuccNum = HeroInfo.actorHero.handle.SkillControl.stSkillStat.m_uiMoveCitySucessTimes;
			}
			stBattleStatisticInfo.iHeroHurtCount = (int)HeroInfo.TotalBeAttackNum;
			stBattleStatisticInfo.iHeroHurtMax = HeroInfo.CountBeHurtMax;
			if (HeroInfo.CountBeHurtMin != -1)
			{
				stBattleStatisticInfo.iHeroHurtMin = HeroInfo.CountBeHurtMin;
			}
			stBattleStatisticInfo.iHeroHurtTotal = HeroInfo.hurtTakenByEnemy;
			stBattleStatisticInfo.iHeroHealCount = (int)HeroInfo.TotalBeHealNum;
			stBattleStatisticInfo.iHeroHealMax = HeroInfo.CountBeHealMax;
			if (HeroInfo.CountBeHealMin != -1)
			{
				stBattleStatisticInfo.iHeroHealMin = HeroInfo.CountBeHealMin;
			}
			stBattleStatisticInfo.iHeroHealTotal = HeroInfo.beHeal;
			stBattleStatisticInfo.iHeroCureMax = HeroInfo.CountSelfHealMax;
			if (HeroInfo.CountSelfHealMin != -1)
			{
				stBattleStatisticInfo.iHeroCureMin = HeroInfo.CountSelfHealMin;
			}
			stBattleStatisticInfo.iHeroCureTotal = HeroInfo.TotalCountSelfHeal;
			if (HeroInfo.actorHero && HeroInfo.actorHero.handle.SkillControl != null && HeroInfo.actorHero.handle.SkillControl.stSkillStat != null)
			{
				stBattleStatisticInfo.iHeroCtrlSkillNum = HeroInfo.actorHero.handle.SkillControl.stSkillStat.GetStunSkillNum();
				stBattleStatisticInfo.iHeroBeCtrledTime = (int)HeroInfo.actorHero.handle.SkillControl.stSkillStat.BeStunTime;
				stBattleStatisticInfo.iHeroCtrlOtherTime = (int)HeroInfo.actorHero.handle.SkillControl.stSkillStat.StunTime;
			}
			stBattleStatisticInfo.dwHpLQ10Kill = HeroInfo.NumKillUnderTenPercent;
			stBattleStatisticInfo.dwTotalEquipHurtCnt = HeroInfo.EquipeHurtValue;
			CBattleDeadStat battleDeadStat = Singleton<BattleStatistic>.instance.m_battleDeadStat;
			if (battleDeadStat != null)
			{
				stBattleStatisticInfo.dwAverageFightTime = (uint)battleDeadStat.GetActorAverageFightTime(HeroInfo.actorHero.handle.TheActorMeta.ActorCamp, ActorTypeDef.Actor_Type_Hero, HeroInfo.actorHero.handle.TheActorMeta.ConfigId);
			}
		}

		private void CreateSkillDetailInfoData(HeroKDA HeroInfo, ref COMDT_SKILL_STATISTIC_INFO[] stSkillDetailInfo, ref COMDT_HERO_BATTLE_STATISTIC_INFO stBattleStatisticInfo)
		{
			if (HeroInfo.actorHero && HeroInfo.actorHero.handle.SkillControl != null && HeroInfo.actorHero.handle.SkillControl.stSkillStat != null && HeroInfo.actorHero.handle.SkillControl.stSkillStat.SkillStatistictInfo != null)
			{
				SKILLSTATISTICTINFO[] skillStatistictInfo = HeroInfo.actorHero.handle.SkillControl.stSkillStat.SkillStatistictInfo;
				SkillSlot[] skillSlotArray = HeroInfo.actorHero.handle.SkillControl.SkillSlotArray;
				for (int i = 0; i < 5; i++)
				{
					if (skillSlotArray[i] != null)
					{
						stSkillDetailInfo[i].iSkillCfgID = skillStatistictInfo[i].iSkillCfgID;
						stSkillDetailInfo[i].dwUsedTimes = skillStatistictInfo[i].uiUsedTimes;
						stSkillDetailInfo[i].dwCDIntervalMin = skillStatistictInfo[i].uiCDIntervalMin;
						stSkillDetailInfo[i].dwAttackDistanceMax = (uint)skillStatistictInfo[i].iAttackDistanceMax;
						stSkillDetailInfo[i].dwHurtMax = (uint)skillStatistictInfo[i].iHurtMax;
						stSkillDetailInfo[i].iHurtValue = skillStatistictInfo[i].ihurtValue;
						stSkillDetailInfo[i].iAdValue = skillStatistictInfo[i].iadValue;
						stSkillDetailInfo[i].iApValue = skillStatistictInfo[i].iapValue;
						stSkillDetailInfo[i].iHpValue = skillStatistictInfo[i].ihpValue;
						stSkillDetailInfo[i].iLoseHpValue = skillStatistictInfo[i].iloseHpValue;
						stSkillDetailInfo[i].iHurtCount = skillStatistictInfo[i].ihurtCount;
						stSkillDetailInfo[i].iHemoFadeRate = skillStatistictInfo[i].ihemoFadeRate;
						stSkillDetailInfo[i].bSkillLevel = (byte)skillSlotArray[i].GetSkillLevel();
						stSkillDetailInfo[i].iTotalHurtValue = skillStatistictInfo[i].iHurtToHeroTotal;
						stSkillDetailInfo[i].iCDMax = (int)skillStatistictInfo[i].uiCDIntervalMax;
						if (skillStatistictInfo[i].uiCDIntervalMin != 4294967295u)
						{
							stSkillDetailInfo[i].iCDMin = (int)skillStatistictInfo[i].uiCDIntervalMin;
						}
						if (skillStatistictInfo[i].uiUsedTimes != 0u)
						{
							stSkillDetailInfo[i].iHitHeroRate = skillStatistictInfo[i].iUseSkillHitHeroTimes * 100 / (int)skillStatistictInfo[i].uiUsedTimes;
						}
						stSkillDetailInfo[i].iHitHeroCount = skillStatistictInfo[i].iHitHeroCount;
						ulong logicFrameTick = Singleton<FrameSynchr>.instance.LogicFrameTick;
						uint num = (uint)(logicFrameTick - (ulong)skillSlotArray[i].lLastUseTime);
						skillStatistictInfo[i].uiCDIntervalMax = ((skillStatistictInfo[i].uiCDIntervalMax > num) ? skillStatistictInfo[i].uiCDIntervalMax : num);
						if (i > 0)
						{
							stBattleStatisticInfo.iHeroSkillDamageTotal += skillStatistictInfo[i].iHurtTotal;
							stBattleStatisticInfo.iHeroSkillCount += (int)stSkillDetailInfo[i].dwUsedTimes;
							stBattleStatisticInfo.iHeroCDMax = Math.Max(stBattleStatisticInfo.iHeroCDMax, (int)skillStatistictInfo[i].uiCDIntervalMax);
							if (skillStatistictInfo[i].uiUsedTimes > 1u && skillStatistictInfo[i].uiCDIntervalMin != 4294967295u)
							{
								if (stBattleStatisticInfo.iHeroCDMin <= 0)
								{
									stBattleStatisticInfo.iHeroCDMin = (int)skillStatistictInfo[i].uiCDIntervalMin;
								}
								else
								{
									stBattleStatisticInfo.iHeroCDMin = Math.Min(stBattleStatisticInfo.iHeroCDMin, (int)skillStatistictInfo[i].uiCDIntervalMin);
								}
							}
							stBattleStatisticInfo.iHeroSkillDamageMax1 = Math.Max(stBattleStatisticInfo.iHeroSkillDamageMax1, skillStatistictInfo[i].iHitAllHurtTotalMax);
							if (skillStatistictInfo[i].iHitAllHurtTotalMin != -1)
							{
								if (stBattleStatisticInfo.iHeroSkillDamageMin1 <= 0)
								{
									stBattleStatisticInfo.iHeroSkillDamageMin1 = skillStatistictInfo[i].iHitAllHurtTotalMin;
								}
								else
								{
									stBattleStatisticInfo.iHeroSkillDamageMin1 = Math.Min(stBattleStatisticInfo.iHeroSkillDamageMin1, skillStatistictInfo[i].iHitAllHurtTotalMin);
								}
							}
							stBattleStatisticInfo.iHeroSkillDamageMax2 = Math.Max(stBattleStatisticInfo.iHeroSkillDamageMax2, skillStatistictInfo[i].iHurtMax);
							if (skillStatistictInfo[i].iHurtMin >= 0)
							{
								if (stBattleStatisticInfo.iHeroSkillDamageMin2 == 0)
								{
									stBattleStatisticInfo.iHeroSkillDamageMin2 = skillStatistictInfo[i].iHurtMin;
								}
								else
								{
									stBattleStatisticInfo.iHeroSkillDamageMin2 = Math.Min(stBattleStatisticInfo.iHeroSkillDamageMin2, skillStatistictInfo[i].iHurtMin);
								}
							}
							stBattleStatisticInfo.iHeroSkillDamageCountMax = Math.Max(stBattleStatisticInfo.iHeroSkillDamageCountMax, skillStatistictInfo[i].iHitCountMax);
							if (skillStatistictInfo[i].iHitCountMin >= 0)
							{
								if (stBattleStatisticInfo.iHeroSkillDamageCountMin == 0)
								{
									stBattleStatisticInfo.iHeroSkillDamageCountMin = skillStatistictInfo[i].iHitCountMin;
								}
								else
								{
									stBattleStatisticInfo.iHeroSkillDamageCountMin = Math.Min(stBattleStatisticInfo.iHeroSkillDamageCountMin, skillStatistictInfo[i].iHitCountMin);
								}
							}
							if (i == 4)
							{
								stBattleStatisticInfo.iHeroCureCount = (int)stSkillDetailInfo[i].dwUsedTimes;
							}
						}
						else
						{
							stBattleStatisticInfo.iHeroAttackCount = (int)stSkillDetailInfo[i].dwUsedTimes;
							stBattleStatisticInfo.iHeroDamageMax = (int)stSkillDetailInfo[i].dwHurtMax;
							if (skillStatistictInfo[i].iHurtMin != -1)
							{
								stBattleStatisticInfo.iHeroDamageMin = skillStatistictInfo[i].iHurtMin;
							}
							stBattleStatisticInfo.iHeroAttackCountTotal += skillStatistictInfo[i].iHurtTotal;
						}
					}
				}
			}
		}

		private void CreateHeroData(uint playerId, ref COMDT_SETTLE_COMMON_DATA CommonData)
		{
			PlayerKDA playerKDA = Singleton<BattleStatistic>.GetInstance().m_playerKDAStat.GetPlayerKDA(playerId);
			if (playerKDA == null)
			{
				DebugHelper.Assert(playerKDA != null, "Failed find player kda, id = {0}", new object[]
				{
					playerId
				});
				Singleton<BattleStatistic>.instance.m_playerKDAStat.DumpDebugInfo();
				return;
			}
			ListView<HeroKDA>.Enumerator enumerator = playerKDA.GetEnumerator();
			byte b = 0;
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.actorHero && enumerator.Current.actorHero.handle.ValueComponent != null)
				{
					bool flag = (int)b < CommonData.stHeroData.astHeroList.Length;
					DebugHelper.Assert(flag, "Invalid member count!");
					if (!flag)
					{
						break;
					}
					uint dwBloodTTH = BurnExpeditionUT.Get_BloodTH(enumerator.Current.actorHero.handle);
					CommonData.stHeroData.astHeroList[(int)b].dwHeroConfID = (uint)enumerator.Current.actorHero.handle.TheActorMeta.ConfigId;
					CommonData.stHeroData.astHeroList[(int)b].dwBloodTTH = dwBloodTTH;
					CommonData.stHeroData.astHeroList[(int)b].astTalentDetail = enumerator.Current.TalentArr;
					CommonData.stHeroData.astHeroList[(int)b].dwGhostLevel = (uint)enumerator.Current.actorHero.handle.ValueComponent.actorSoulLevel;
					this.CreateHeroBaseInfoData(enumerator.Current, ref CommonData.stHeroData.astHeroList[(int)b].stHeroDetailInfo);
					this.CreateBattleStatisticInfoData(enumerator.Current, ref CommonData.stHeroData.astHeroList[(int)b].stHeroBattleInfo);
					this.CreateSkillDetailInfoData(enumerator.Current, ref CommonData.stHeroData.astHeroList[(int)b].astSkillStatisticInfo, ref CommonData.stHeroData.astHeroList[(int)b].stHeroBattleInfo);
					PoolObjHandle<ActorRoot> captain = Singleton<GamePlayerCenter>.GetInstance().GetPlayer(playerId).Captain;
					if (captain.handle.EquipComponent != null)
					{
						Dictionary<ushort, uint> equipBoughtHistory = captain.handle.EquipComponent.GetEquipBoughtHistory();
						byte b2 = (byte)equipBoughtHistory.get_Count();
						b2 = ((b2 < 30) ? b2 : 30);
						CommonData.stHeroData.astHeroList[(int)b].bUsedEquipIndex = (byte)captain.handle.EquipComponent.GetUsedRecommendEquipGroup();
						CommonData.stHeroData.astHeroList[(int)b].bInBattleEquipNum = b2;
						Dictionary<ushort, uint>.Enumerator enumerator2 = equipBoughtHistory.GetEnumerator();
						int num = 0;
						while (enumerator2.MoveNext())
						{
							COMDT_SETTLE_INBATTLE_EQUIP_INFO cOMDT_SETTLE_INBATTLE_EQUIP_INFO = CommonData.stHeroData.astHeroList[(int)b].astInBattleEquipInfo[num];
							KeyValuePair<ushort, uint> current = enumerator2.get_Current();
							cOMDT_SETTLE_INBATTLE_EQUIP_INFO.dwEquipID = (uint)current.get_Key();
							COMDT_SETTLE_INBATTLE_EQUIP_INFO cOMDT_SETTLE_INBATTLE_EQUIP_INFO2 = CommonData.stHeroData.astHeroList[(int)b].astInBattleEquipInfo[num];
							KeyValuePair<ushort, uint> current2 = enumerator2.get_Current();
							cOMDT_SETTLE_INBATTLE_EQUIP_INFO2.dwFirstBuyTime = current2.get_Value();
							if (++num >= (int)b2)
							{
								break;
							}
						}
					}
					CommonData.stHeroData.astHeroList[(int)b].stHeroBattleInfo.dwFirstMoveTime = playerKDA.m_firstMoveTime;
					int num2 = playerKDA.m_nReviveCount;
					if (num2 > 20)
					{
						num2 = 20;
					}
					int num3 = 0;
					while (num3 < num2 && num3 < playerKDA.m_reviveMoveTime.Length && num3 < CommonData.stHeroData.astHeroList[(int)b].stHeroBattleInfo.ReviveDetail.Length)
					{
						CommonData.stHeroData.astHeroList[(int)b].stHeroBattleInfo.ReviveDetail[num3] = playerKDA.m_reviveMoveTime[num3];
						num3++;
					}
					CommonData.stHeroData.astHeroList[(int)b].stHeroBattleInfo.bReviveNum = (byte)num2;
					CommonData.stHeroData.astHeroList[(int)b].stHeroBattleInfo.dwSoliderKillCoin = (uint)enumerator.Current.CoinFromKillSolider;
					CommonData.stHeroData.astHeroList[(int)b].stHeroBattleInfo.dwMonsterKillCoin = (uint)enumerator.Current.CoinFromKillMonster;
					CommonData.stHeroData.astHeroList[(int)b].stHeroBattleInfo.dwTowerKillCoin = (uint)enumerator.Current.CoinFromKillTower;
					CommonData.stHeroData.astHeroList[(int)b].stHeroBattleInfo.dwDragonKillCoin = (uint)enumerator.Current.CoinFromKillDragon;
					CommonData.stHeroData.astHeroList[(int)b].stHeroBattleInfo.dwHeroKillCoin = (uint)enumerator.Current.CoinFromKillHero;
					CommonData.stHeroData.astHeroList[(int)b].stHeroBattleInfo.dwSystemAddCoin = (uint)enumerator.Current.CoinFromSystem;
					CommonData.stHeroData.astHeroList[(int)b].stHeroBattleInfo.bDeathNum = (byte)enumerator.Current.numDead;
					int num4 = 0;
					while (num4 < enumerator.Current.numDead && num4 < 50)
					{
						CommonData.stHeroData.astHeroList[(int)b].stHeroBattleInfo.astDeathPos[num4].iTime = enumerator.Current.m_arrDeadPosition[num4].iTime;
						CommonData.stHeroData.astHeroList[(int)b].stHeroBattleInfo.astDeathPos[num4].iXPos = enumerator.Current.m_arrDeadPosition[num4].iXPos;
						CommonData.stHeroData.astHeroList[(int)b].stHeroBattleInfo.astDeathPos[num4].iZPos = enumerator.Current.m_arrDeadPosition[num4].iZPos;
						num4++;
					}
					CommonData.stHeroData.astHeroList[(int)b].stHeroBattleInfo.dwScanNum = enumerator.Current.scanEyeCnt;
					CommonData.stHeroData.astHeroList[(int)b].stHeroBattleInfo.iHeroEndLive = ((enumerator.Current.actorHero.handle.ActorControl.myBehavior == ObjBehaviMode.State_Dead) ? 0 : 1);
					CommonData.stHeroData.astHeroList[(int)b].stHeroBattleInfo.iHeroInitHp = enumerator.Current.actorHero.handle.ValueComponent.actorHpTotal;
					CommonData.stHeroData.astHeroList[(int)b].stHeroBattleInfo.iHeroEndHp = enumerator.Current.actorHero.handle.ValueComponent.actorHp;
					b += 1;
				}
			}
			CommonData.stHeroData.bNum = b;
		}

		private void CreateDestroyReportData(ref COMDT_SETTLE_COMMON_DATA CommonData, bool bGMWin)
		{
			int num = 0;
			DictionaryView<uint, Dictionary<int, DestroyStat>> destroyStat = Singleton<BattleStatistic>.GetInstance().GetDestroyStat();
			COMDT_STATISTIC_STRUCT_PILE cOMDT_STATISTIC_STRUCT_PILE = CommonData.stStatisticData.astDetail[(int)CommonData.stStatisticData.bNum];
			COMDT_STATISTIC_BASE_STRUCT[] astDetail = cOMDT_STATISTIC_STRUCT_PILE.astDetail;
			if (destroyStat.Count != 0)
			{
				cOMDT_STATISTIC_STRUCT_PILE.bReportType = 1;
				DictionaryView<uint, Dictionary<int, DestroyStat>>.Enumerator enumerator = destroyStat.GetEnumerator();
				while (enumerator.MoveNext())
				{
					KeyValuePair<uint, Dictionary<int, DestroyStat>> current = enumerator.Current;
					Dictionary<int, DestroyStat> value = current.get_Value();
					Dictionary<int, DestroyStat>.Enumerator enumerator2 = value.GetEnumerator();
					int num2 = 0;
					int num3 = 0;
					while (enumerator2.MoveNext())
					{
						astDetail[num] = new COMDT_STATISTIC_BASE_STRUCT();
						astDetail[num].stKeyInfo.bKeyNum = 3;
						int[] keyDetail = astDetail[num].stKeyInfo.KeyDetail;
						int num4 = 0;
						KeyValuePair<uint, Dictionary<int, DestroyStat>> current2 = enumerator.Current;
						keyDetail[num4] = (int)current2.get_Key();
						int[] keyDetail2 = astDetail[num].stKeyInfo.KeyDetail;
						int num5 = 1;
						KeyValuePair<int, DestroyStat> current3 = enumerator2.get_Current();
						keyDetail2[num5] = current3.get_Key();
						astDetail[num].stKeyInfo.KeyDetail[2] = 1;
						astDetail[num].stValueInfo.bValueNum = 1;
						int[] valueDetail = astDetail[num].stValueInfo.ValueDetail;
						int num6 = 0;
						KeyValuePair<int, DestroyStat> current4 = enumerator2.get_Current();
						valueDetail[num6] = current4.get_Value().CampEnemyNum;
						num++;
						int num7 = num2;
						KeyValuePair<int, DestroyStat> current5 = enumerator2.get_Current();
						num2 = num7 + current5.get_Value().CampEnemyNum;
						astDetail[num].stKeyInfo.bKeyNum = 3;
						int[] keyDetail3 = astDetail[num].stKeyInfo.KeyDetail;
						int num8 = 0;
						KeyValuePair<uint, Dictionary<int, DestroyStat>> current6 = enumerator.Current;
						keyDetail3[num8] = (int)current6.get_Key();
						int[] keyDetail4 = astDetail[num].stKeyInfo.KeyDetail;
						int num9 = 1;
						KeyValuePair<int, DestroyStat> current7 = enumerator2.get_Current();
						keyDetail4[num9] = current7.get_Key();
						astDetail[num].stKeyInfo.KeyDetail[2] = 0;
						astDetail[num].stValueInfo.bValueNum = 1;
						int[] valueDetail2 = astDetail[num].stValueInfo.ValueDetail;
						int num10 = 0;
						KeyValuePair<int, DestroyStat> current8 = enumerator2.get_Current();
						valueDetail2[num10] = current8.get_Value().CampSelfNum;
						num++;
						int num11 = num3;
						KeyValuePair<int, DestroyStat> current9 = enumerator2.get_Current();
						num3 = num11 + current9.get_Value().CampSelfNum;
					}
					astDetail[num].stKeyInfo.bKeyNum = 3;
					int[] keyDetail5 = astDetail[num].stKeyInfo.KeyDetail;
					int num12 = 0;
					KeyValuePair<uint, Dictionary<int, DestroyStat>> current10 = enumerator.Current;
					keyDetail5[num12] = (int)current10.get_Key();
					astDetail[num].stKeyInfo.KeyDetail[1] = 0;
					astDetail[num].stKeyInfo.KeyDetail[2] = 1;
					astDetail[num].stValueInfo.bValueNum = 1;
					astDetail[num].stValueInfo.ValueDetail[0] = num2;
					num++;
					astDetail[num].stKeyInfo.bKeyNum = 3;
					int[] keyDetail6 = astDetail[num].stKeyInfo.KeyDetail;
					int num13 = 0;
					KeyValuePair<uint, Dictionary<int, DestroyStat>> current11 = enumerator.Current;
					keyDetail6[num13] = (int)current11.get_Key();
					astDetail[num].stKeyInfo.KeyDetail[1] = 0;
					astDetail[num].stKeyInfo.KeyDetail[2] = 0;
					astDetail[num].stValueInfo.bValueNum = 1;
					astDetail[num].stValueInfo.ValueDetail[0] = num3;
					num++;
				}
				CommonData.stStatisticData.astDetail[(int)CommonData.stStatisticData.bNum].wNum = (ushort)num;
				COMDT_STATISTIC_DATA stStatisticData = CommonData.stStatisticData;
				COMDT_STATISTIC_DATA expr_34E = stStatisticData;
				expr_34E.bNum += 1;
			}
			SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
			if (curLvelContext == null || !curLvelContext.IsMobaMode())
			{
				if (bGMWin)
				{
					this.FakeStarInfoForGMWin(ref CommonData);
				}
				else
				{
					this.VerifyReportData(ref CommonData);
				}
			}
		}

		private void FakeStarInfoForGMWin(ref COMDT_SETTLE_COMMON_DATA CommonData)
		{
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			DebugHelper.Assert(curLvelContext != null);
			if (curLvelContext == null)
			{
				return;
			}
			for (int i = 0; i < curLvelContext.m_starDetail.Length; i++)
			{
				int iParam = curLvelContext.m_starDetail[i].iParam;
				if (iParam == 0)
				{
					return;
				}
				ResEvaluateStarInfo dataByKey = GameDataMgr.evaluateCondInfoDatabin.GetDataByKey((uint)iParam);
				if (dataByKey != null)
				{
					COMDT_STATISTIC_STRUCT_PILE cOMDT_STATISTIC_STRUCT_PILE = null;
					for (int j = 0; j < (int)CommonData.stStatisticData.bNum; j++)
					{
						if (CommonData.stStatisticData.astDetail[j].bReportType == 1)
						{
							cOMDT_STATISTIC_STRUCT_PILE = CommonData.stStatisticData.astDetail[j];
							break;
						}
					}
					if (cOMDT_STATISTIC_STRUCT_PILE == null)
					{
						int bNum = (int)CommonData.stStatisticData.bNum;
						cOMDT_STATISTIC_STRUCT_PILE = CommonData.stStatisticData.astDetail[bNum];
						cOMDT_STATISTIC_STRUCT_PILE.bReportType = 1;
						COMDT_STATISTIC_DATA stStatisticData = CommonData.stStatisticData;
						COMDT_STATISTIC_DATA expr_D9 = stStatisticData;
						expr_D9.bNum += 1;
					}
					COMDT_STATISTIC_BASE_STRUCT[] astDetail = cOMDT_STATISTIC_STRUCT_PILE.astDetail;
					for (int k = 0; k < dataByKey.astConditions.Length; k++)
					{
						if (dataByKey.astConditions[k].dwType == 1u)
						{
							this.FakeWithCondition(ref dataByKey.astConditions[k], ref cOMDT_STATISTIC_STRUCT_PILE, ref astDetail);
						}
					}
				}
			}
		}

		private void FakeWithCondition(ref ResDT_ConditionInfo ConditionInfo, ref COMDT_STATISTIC_STRUCT_PILE StatRef, ref COMDT_STATISTIC_BASE_STRUCT[] DetailData)
		{
			int num = ConditionInfo.KeyDetail[0];
			int num2 = ConditionInfo.KeyDetail[1];
			int num3 = ConditionInfo.KeyDetail[2];
			for (int i = 0; i < DetailData.Length; i++)
			{
				COMDT_STATISTIC_BASE_STRUCT cOMDT_STATISTIC_BASE_STRUCT = DetailData[i];
				if (cOMDT_STATISTIC_BASE_STRUCT.stKeyInfo.bKeyNum <= 0)
				{
					break;
				}
				if (cOMDT_STATISTIC_BASE_STRUCT.stKeyInfo.KeyDetail[0] == num && cOMDT_STATISTIC_BASE_STRUCT.stKeyInfo.KeyDetail[1] == num2 && cOMDT_STATISTIC_BASE_STRUCT.stKeyInfo.KeyDetail[2] == num3)
				{
					this.FakeWithValues(ref ConditionInfo, ref cOMDT_STATISTIC_BASE_STRUCT);
					return;
				}
			}
			DetailData[(int)StatRef.wNum].stKeyInfo.bKeyNum = 3;
			DetailData[(int)StatRef.wNum].stKeyInfo.KeyDetail[0] = num;
			DetailData[(int)StatRef.wNum].stKeyInfo.KeyDetail[1] = num2;
			DetailData[(int)StatRef.wNum].stKeyInfo.KeyDetail[2] = num3;
			this.FakeWithValues(ref ConditionInfo, ref DetailData[(int)StatRef.wNum]);
			COMDT_STATISTIC_STRUCT_PILE cOMDT_STATISTIC_STRUCT_PILE = StatRef;
			COMDT_STATISTIC_STRUCT_PILE expr_10B = cOMDT_STATISTIC_STRUCT_PILE;
			expr_10B.wNum += 1;
		}

		private void FakeWithValues(ref ResDT_ConditionInfo ConditionInfo, ref COMDT_STATISTIC_BASE_STRUCT Data)
		{
			for (int i = 0; i < ConditionInfo.ValueDetail.Length; i++)
			{
				if (ConditionInfo.ValueDetail[i] == 0)
				{
					break;
				}
				Data.stValueInfo.bValueNum = (byte)(i + 1);
				Data.stValueInfo.ValueDetail[i] = this.FakeValue(ConditionInfo.ComparetorDetail[i], ConditionInfo.ValueDetail[i]);
			}
		}

		private int FakeValue(int Comparation, int CfgValue)
		{
			switch (Comparation)
			{
			case 1:
			case 2:
				return CfgValue - 1;
			case 3:
				return CfgValue;
			case 4:
			case 5:
				return CfgValue + 1;
			default:
				DebugHelper.Assert(false, "what the fuck?");
				return 0;
			}
		}

		private void CreateSettleReportData(uint playerId, ref COMDT_SETTLE_COMMON_DATA CommonData, bool bGMWin, bool bWin)
		{
			PlayerKDA playerKDA = Singleton<BattleStatistic>.GetInstance().m_playerKDAStat.GetPlayerKDA(playerId);
			CBattleDeadStat battleDeadStat = Singleton<BattleStatistic>.instance.m_battleDeadStat;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			int num7 = 0;
			ListView<HeroKDA>.Enumerator enumerator = playerKDA.GetEnumerator();
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			bool flag6 = false;
			while (enumerator.MoveNext())
			{
				num += enumerator.Current.LegendaryNum;
				num2 = enumerator.Current.PentaKillNum;
				num3 = enumerator.Current.QuataryKillNum;
				num4 += enumerator.Current.TripleKillNum;
				num5 += enumerator.Current.DoubleKillNum;
				if (enumerator.Current.bHurtTakenMost)
				{
					flag3 = true;
				}
				if (enumerator.Current.bGetCoinMost)
				{
					flag2 = true;
				}
				if (enumerator.Current.bHurtToHeroMost)
				{
					flag = true;
				}
				if (enumerator.Current.bAsssistMost)
				{
					flag5 = true;
				}
				if (enumerator.Current.bKillMost)
				{
					flag4 = true;
				}
				if (enumerator.Current.bKillOrganMost)
				{
					flag6 = true;
				}
			}
			if (bWin)
			{
				uint mvpPlayer = Singleton<BattleStatistic>.instance.GetMvpPlayer(playerKDA.PlayerCamp, bWin);
				if (mvpPlayer != 0u)
				{
					num6 = ((mvpPlayer == playerKDA.PlayerId) ? 1 : 0);
				}
			}
			else
			{
				uint mvpPlayer2 = Singleton<BattleStatistic>.instance.GetMvpPlayer(playerKDA.PlayerCamp, bWin);
				if (mvpPlayer2 != 0u)
				{
					num7 = ((mvpPlayer2 == playerKDA.PlayerId) ? 1 : 0);
				}
			}
			int num8 = 0;
			CommonData.stStatisticData.astDetail[(int)CommonData.stStatisticData.bNum].bReportType = 4;
			COMDT_STATISTIC_BASE_STRUCT[] astDetail = CommonData.stStatisticData.astDetail[(int)CommonData.stStatisticData.bNum].astDetail;
			astDetail[num8].stKeyInfo.bKeyNum = 1;
			astDetail[num8].stKeyInfo.KeyDetail[0] = 1;
			astDetail[num8].stValueInfo.bValueNum = 1;
			astDetail[num8].stValueInfo.ValueDetail[0] = playerKDA.numKill;
			num8++;
			astDetail[num8].stKeyInfo.bKeyNum = 1;
			astDetail[num8].stKeyInfo.KeyDetail[0] = 2;
			astDetail[num8].stValueInfo.bValueNum = 1;
			astDetail[num8].stValueInfo.ValueDetail[0] = playerKDA.numDead;
			num8++;
			astDetail[num8].stKeyInfo.bKeyNum = 1;
			astDetail[num8].stKeyInfo.KeyDetail[0] = 4;
			astDetail[num8].stValueInfo.bValueNum = 1;
			astDetail[num8].stValueInfo.ValueDetail[0] = playerKDA.numKillMonster;
			num8++;
			astDetail[num8].stKeyInfo.bKeyNum = 1;
			astDetail[num8].stKeyInfo.KeyDetail[0] = 3;
			astDetail[num8].stValueInfo.bValueNum = 1;
			astDetail[num8].stValueInfo.ValueDetail[0] = playerKDA.numAssist;
			num8++;
			astDetail[num8].stKeyInfo.bKeyNum = 1;
			astDetail[num8].stKeyInfo.KeyDetail[0] = 5;
			astDetail[num8].stValueInfo.bValueNum = 1;
			astDetail[num8].stValueInfo.ValueDetail[0] = (int)Singleton<StarSystem>.GetInstance().GetStarBits();
			if (bGMWin)
			{
				astDetail[num8].stValueInfo.ValueDetail[0] = 7;
			}
			num8++;
			astDetail[num8].stKeyInfo.bKeyNum = 1;
			astDetail[num8].stKeyInfo.KeyDetail[0] = 6;
			astDetail[num8].stValueInfo.bValueNum = 1;
			astDetail[num8].stValueInfo.ValueDetail[0] = battleDeadStat.GetDeadTime(COM_PLAYERCAMP.COM_PLAYERCAMP_1, ActorTypeDef.Actor_Type_Organ, 0);
			num8++;
			astDetail[num8].stKeyInfo.bKeyNum = 1;
			astDetail[num8].stKeyInfo.KeyDetail[0] = 7;
			astDetail[num8].stValueInfo.bValueNum = 1;
			astDetail[num8].stValueInfo.ValueDetail[0] = battleDeadStat.GetDeadTime(COM_PLAYERCAMP.COM_PLAYERCAMP_1, ActorTypeDef.Actor_Type_Organ, 1);
			num8++;
			astDetail[num8].stKeyInfo.bKeyNum = 1;
			astDetail[num8].stKeyInfo.KeyDetail[0] = 8;
			astDetail[num8].stValueInfo.bValueNum = 1;
			astDetail[num8].stValueInfo.ValueDetail[0] = battleDeadStat.GetDeadTime(COM_PLAYERCAMP.COM_PLAYERCAMP_2, ActorTypeDef.Actor_Type_Organ, 0);
			num8++;
			astDetail[num8].stKeyInfo.bKeyNum = 1;
			astDetail[num8].stKeyInfo.KeyDetail[0] = 9;
			astDetail[num8].stValueInfo.bValueNum = 1;
			astDetail[num8].stValueInfo.ValueDetail[0] = battleDeadStat.GetDeadTime(COM_PLAYERCAMP.COM_PLAYERCAMP_2, ActorTypeDef.Actor_Type_Organ, 1);
			num8++;
			astDetail[num8].stKeyInfo.bKeyNum = 1;
			astDetail[num8].stKeyInfo.KeyDetail[0] = 10;
			astDetail[num8].stValueInfo.bValueNum = 1;
			astDetail[num8].stValueInfo.ValueDetail[0] = battleDeadStat.GetKillDragonNum(COM_PLAYERCAMP.COM_PLAYERCAMP_1);
			num8++;
			astDetail[num8].stKeyInfo.bKeyNum = 1;
			astDetail[num8].stKeyInfo.KeyDetail[0] = 11;
			astDetail[num8].stValueInfo.bValueNum = 1;
			astDetail[num8].stValueInfo.ValueDetail[0] = battleDeadStat.GetKillDragonNum(COM_PLAYERCAMP.COM_PLAYERCAMP_2);
			num8++;
			astDetail[num8].stKeyInfo.bKeyNum = 1;
			astDetail[num8].stKeyInfo.KeyDetail[0] = 12;
			astDetail[num8].stValueInfo.bValueNum = 1;
			astDetail[num8].stValueInfo.ValueDetail[0] = battleDeadStat.GetAllMonsterDeadNum();
			num8++;
			astDetail[num8].stKeyInfo.bKeyNum = 1;
			astDetail[num8].stKeyInfo.KeyDetail[0] = 13;
			astDetail[num8].stValueInfo.bValueNum = 1;
			astDetail[num8].stValueInfo.ValueDetail[0] = num6;
			num8++;
			astDetail[num8].stKeyInfo.bKeyNum = 1;
			astDetail[num8].stKeyInfo.KeyDetail[0] = 14;
			astDetail[num8].stValueInfo.bValueNum = 1;
			astDetail[num8].stValueInfo.ValueDetail[0] = num7;
			num8++;
			astDetail[num8].stKeyInfo.bKeyNum = 1;
			astDetail[num8].stKeyInfo.KeyDetail[0] = 15;
			astDetail[num8].stValueInfo.bValueNum = 1;
			astDetail[num8].stValueInfo.ValueDetail[0] = num;
			num8++;
			astDetail[num8].stKeyInfo.bKeyNum = 1;
			astDetail[num8].stKeyInfo.KeyDetail[0] = 16;
			astDetail[num8].stValueInfo.bValueNum = 1;
			astDetail[num8].stValueInfo.ValueDetail[0] = num5;
			num8++;
			astDetail[num8].stKeyInfo.bKeyNum = 1;
			astDetail[num8].stKeyInfo.KeyDetail[0] = 17;
			astDetail[num8].stValueInfo.bValueNum = 1;
			astDetail[num8].stValueInfo.ValueDetail[0] = num4;
			num8++;
			astDetail[num8].stKeyInfo.bKeyNum = 1;
			astDetail[num8].stKeyInfo.KeyDetail[0] = 27;
			astDetail[num8].stValueInfo.bValueNum = 1;
			astDetail[num8].stValueInfo.ValueDetail[0] = num3;
			num8++;
			astDetail[num8].stKeyInfo.bKeyNum = 1;
			astDetail[num8].stKeyInfo.KeyDetail[0] = 28;
			astDetail[num8].stValueInfo.bValueNum = 1;
			astDetail[num8].stValueInfo.ValueDetail[0] = num2;
			num8++;
			if (Singleton<CBattleSystem>.GetInstance().FightForm != null)
			{
				astDetail[num8].stKeyInfo.bKeyNum = 1;
				astDetail[num8].stKeyInfo.KeyDetail[0] = 19;
				astDetail[num8].stValueInfo.bValueNum = 1;
				astDetail[num8].stValueInfo.ValueDetail[0] = Singleton<DataReportSys>.GetInstance().FPSMax;
				num8++;
				astDetail[num8].stKeyInfo.bKeyNum = 1;
				astDetail[num8].stKeyInfo.KeyDetail[0] = 20;
				astDetail[num8].stValueInfo.bValueNum = 1;
				if (Singleton<DataReportSys>.GetInstance().FPSCount == 0)
				{
					Singleton<DataReportSys>.GetInstance().FPSCount = 1;
				}
				astDetail[num8].stValueInfo.ValueDetail[0] = Singleton<DataReportSys>.GetInstance().FPSAVE / Singleton<DataReportSys>.GetInstance().FPSCount;
				num8++;
				astDetail[num8].stKeyInfo.bKeyNum = 1;
				astDetail[num8].stKeyInfo.KeyDetail[0] = 21;
				astDetail[num8].stValueInfo.bValueNum = 1;
				astDetail[num8].stValueInfo.ValueDetail[0] = Singleton<DataReportSys>.GetInstance().FPSMin;
				num8++;
			}
			astDetail[num8].stKeyInfo.bKeyNum = 1;
			astDetail[num8].stKeyInfo.KeyDetail[0] = 22;
			astDetail[num8].stValueInfo.bValueNum = 1;
			astDetail[num8].stValueInfo.ValueDetail[0] = Singleton<DataReportSys>.GetInstance().HeartPingMax;
			num8++;
			astDetail[num8].stKeyInfo.bKeyNum = 1;
			astDetail[num8].stKeyInfo.KeyDetail[0] = 23;
			astDetail[num8].stValueInfo.bValueNum = 1;
			astDetail[num8].stValueInfo.ValueDetail[0] = Singleton<DataReportSys>.GetInstance().HeartPingAve;
			num8++;
			astDetail[num8].stKeyInfo.bKeyNum = 1;
			astDetail[num8].stKeyInfo.KeyDetail[0] = 24;
			astDetail[num8].stValueInfo.bValueNum = 1;
			astDetail[num8].stValueInfo.ValueDetail[0] = Singleton<DataReportSys>.GetInstance().HeartPingMin;
			num8++;
			astDetail[num8].stKeyInfo.bKeyNum = 1;
			astDetail[num8].stKeyInfo.KeyDetail[0] = 25;
			astDetail[num8].stValueInfo.bValueNum = 1;
			astDetail[num8].stValueInfo.ValueDetail[0] = battleDeadStat.GetBaronDeadCount(COM_PLAYERCAMP.COM_PLAYERCAMP_1);
			num8++;
			astDetail[num8].stKeyInfo.bKeyNum = 1;
			astDetail[num8].stKeyInfo.KeyDetail[0] = 26;
			astDetail[num8].stValueInfo.bValueNum = 1;
			astDetail[num8].stValueInfo.ValueDetail[0] = battleDeadStat.GetBaronDeadCount(COM_PLAYERCAMP.COM_PLAYERCAMP_2);
			num8++;
			astDetail[num8].stKeyInfo.bKeyNum = 1;
			astDetail[num8].stKeyInfo.KeyDetail[0] = 29;
			astDetail[num8].stValueInfo.bValueNum = 1;
			astDetail[num8].stValueInfo.ValueDetail[0] = (flag ? 1 : 0);
			num8++;
			astDetail[num8].stKeyInfo.bKeyNum = 1;
			astDetail[num8].stKeyInfo.KeyDetail[0] = 30;
			astDetail[num8].stValueInfo.bValueNum = 1;
			astDetail[num8].stValueInfo.ValueDetail[0] = (flag2 ? 1 : 0);
			num8++;
			astDetail[num8].stKeyInfo.bKeyNum = 1;
			astDetail[num8].stKeyInfo.KeyDetail[0] = 31;
			astDetail[num8].stValueInfo.bValueNum = 1;
			astDetail[num8].stValueInfo.ValueDetail[0] = (flag3 ? 1 : 0);
			num8++;
			astDetail[num8].stKeyInfo.bKeyNum = 1;
			astDetail[num8].stKeyInfo.KeyDetail[0] = 32;
			astDetail[num8].stValueInfo.bValueNum = 1;
			astDetail[num8].stValueInfo.ValueDetail[0] = (flag5 ? 1 : 0);
			num8++;
			astDetail[num8].stKeyInfo.bKeyNum = 1;
			astDetail[num8].stKeyInfo.KeyDetail[0] = 33;
			astDetail[num8].stValueInfo.bValueNum = 1;
			astDetail[num8].stValueInfo.ValueDetail[0] = (flag4 ? 1 : 0);
			num8++;
			astDetail[num8].stKeyInfo.bKeyNum = 1;
			astDetail[num8].stKeyInfo.KeyDetail[0] = 34;
			astDetail[num8].stValueInfo.bValueNum = 1;
			astDetail[num8].stValueInfo.ValueDetail[0] = (flag6 ? 1 : 0);
			num8++;
			astDetail[num8].stKeyInfo.bKeyNum = 1;
			astDetail[num8].stKeyInfo.KeyDetail[0] = 35;
			astDetail[num8].stValueInfo.bValueNum = 1;
			astDetail[num8].stValueInfo.ValueDetail[0] = Singleton<BattleStatistic>.instance.GetCampScore(playerKDA.PlayerCamp);
			num8++;
			astDetail[num8].stKeyInfo.bKeyNum = 1;
			astDetail[num8].stKeyInfo.KeyDetail[0] = 36;
			astDetail[num8].stValueInfo.bValueNum = 1;
			astDetail[num8].stValueInfo.ValueDetail[0] = ((battleDeadStat.m_uiTakeFBPlayerId == playerId) ? 1 : 0);
			num8++;
			astDetail[num8].stKeyInfo.bKeyNum = 1;
			astDetail[num8].stKeyInfo.KeyDetail[0] = 37;
			astDetail[num8].stValueInfo.bValueNum = 1;
			astDetail[num8].stValueInfo.ValueDetail[0] = Singleton<BattleStatistic>.GetInstance().m_playerKDAStat.GetTeamAssistNum(playerKDA.PlayerCamp);
			num8++;
			astDetail[num8].stKeyInfo.bKeyNum = 1;
			astDetail[num8].stKeyInfo.KeyDetail[0] = 38;
			astDetail[num8].stValueInfo.bValueNum = 1;
			astDetail[num8].stValueInfo.ValueDetail[0] = Singleton<BattleStatistic>.GetInstance().m_playerKDAStat.GetTeamDeadNum(playerKDA.PlayerCamp);
			num8++;
			astDetail[num8].stKeyInfo.bKeyNum = 1;
			astDetail[num8].stKeyInfo.KeyDetail[0] = 39;
			astDetail[num8].stValueInfo.bValueNum = 1;
			astDetail[num8].stValueInfo.ValueDetail[0] = Singleton<BattleStatistic>.GetInstance().m_playerKDAStat.GetTeamCoin(playerKDA.PlayerCamp);
			num8++;
			CommonData.stStatisticData.astDetail[(int)CommonData.stStatisticData.bNum].wNum = (ushort)num8;
			COMDT_STATISTIC_DATA stStatisticData = CommonData.stStatisticData;
			COMDT_STATISTIC_DATA expr_EA0 = stStatisticData;
			expr_EA0.bNum += 1;
			CPlayerPvpHistoryController.SetFightAchive(ref CommonData.stGeneralData.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_GODLIKE, num > 0);
			CPlayerPvpHistoryController.SetFightAchive(ref CommonData.stGeneralData.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_PENTAKILL, num2 > 0);
			CPlayerPvpHistoryController.SetFightAchive(ref CommonData.stGeneralData.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_QUATARYKILL, num3 > 0);
			CPlayerPvpHistoryController.SetFightAchive(ref CommonData.stGeneralData.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_TRIPLEKILL, num4 > 0);
			CPlayerPvpHistoryController.SetFightAchive(ref CommonData.stGeneralData.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_DOUBLEKILL, num5 > 0);
			CPlayerPvpHistoryController.SetFightAchive(ref CommonData.stGeneralData.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_RECVDAMAGEMOST, flag3);
			CPlayerPvpHistoryController.SetFightAchive(ref CommonData.stGeneralData.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_GETMOENYMOST, flag2);
			CPlayerPvpHistoryController.SetFightAchive(ref CommonData.stGeneralData.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_HURTTOHEROMOST, flag);
			CPlayerPvpHistoryController.SetFightAchive(ref CommonData.stGeneralData.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_ASSISTMOST, flag5);
			CPlayerPvpHistoryController.SetFightAchive(ref CommonData.stGeneralData.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_KILLMOST, flag4);
			CPlayerPvpHistoryController.SetFightAchive(ref CommonData.stGeneralData.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_KILLORGANMOST, flag6);
			CPlayerPvpHistoryController.SetFightAchive(ref CommonData.stGeneralData.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_FIGHTWITHFRIEND, CPlayerPvpHistoryController.IsPlayWithFriend(playerKDA.PlayerCamp));
			CPlayerPvpHistoryController.SetFightAchive(ref CommonData.stGeneralData.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_RUNAWAY, playerKDA.bRunaway || playerKDA.bDisconnect || playerKDA.bHangup);
			CPlayerPvpHistoryController.SetFightAchive(ref CommonData.stGeneralData.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_ISCOMPUTER, playerKDA.IsComputer);
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			if (curLvelContext != null)
			{
				CPlayerPvpHistoryController.SetFightAchive(ref CommonData.stGeneralData.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_ISWARMBATTLE, curLvelContext.m_isWarmBattle);
				if (curLvelContext != null && curLvelContext.GetGameType() == COM_GAME_TYPE.COM_MULTI_GAME_OF_LADDER)
				{
					CPlayerPvpHistoryController.SetFightAchive(ref CommonData.stGeneralData.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_FIGHTWITHFRIEND, false);
					TeamInfo teamInfo = Singleton<CMatchingSystem>.instance.teamInfo;
					if (teamInfo != null)
					{
						switch (teamInfo.MemInfoList.Count)
						{
						case 2:
							CPlayerPvpHistoryController.SetFightAchive(ref CommonData.stGeneralData.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_LADDER2, true);
							break;
						case 3:
							CPlayerPvpHistoryController.SetFightAchive(ref CommonData.stGeneralData.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_LADDER3, true);
							break;
						case 4:
							CPlayerPvpHistoryController.SetFightAchive(ref CommonData.stGeneralData.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_LADDER4, true);
							break;
						case 5:
							CPlayerPvpHistoryController.SetFightAchive(ref CommonData.stGeneralData.dwRongyuFlag, COM_FIGHT_HISTORY_ACHIVE_BIT.FIGHT_HISTORY_ACHIVE_LADDER5V5, true);
							break;
						}
					}
				}
			}
		}

		private void CreateAttrReportData(ref COMDT_SETTLE_COMMON_DATA CommonData, bool bGMWin)
		{
			StarSystem instance = Singleton<StarSystem>.GetInstance();
			ListView<IStarEvaluation>.Enumerator enumerator = instance.GetEnumerator();
			int num = 0;
			CommonData.stStatisticData.astDetail[(int)CommonData.stStatisticData.bNum].bReportType = 2;
			COMDT_STATISTIC_BASE_STRUCT[] astDetail = CommonData.stStatisticData.astDetail[(int)CommonData.stStatisticData.bNum].astDetail;
			while (enumerator.MoveNext())
			{
				ListView<IStarCondition>.Enumerator enumerator2 = enumerator.Current.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					if (enumerator2.Current.type == 2)
					{
						int[] keys = enumerator2.Current.keys;
						int[] values = enumerator2.Current.values;
						DebugHelper.Assert(keys.Length == 4);
						astDetail[num].stKeyInfo.bKeyNum = (byte)keys.Length;
						for (int i = 0; i < keys.Length; i++)
						{
							astDetail[num].stKeyInfo.KeyDetail[i] = keys[i];
						}
						DebugHelper.Assert(values != null && values.Length >= 0);
						astDetail[num].stValueInfo.bValueNum = (byte)values.Length;
						for (int j = 0; j < values.Length; j++)
						{
							if (!bGMWin)
							{
								astDetail[num].stValueInfo.ValueDetail[j] = values[j];
							}
							else
							{
								astDetail[num].stValueInfo.ValueDetail[j] = this.FakeValue(enumerator2.Current.configInfo.ComparetorDetail[j], enumerator2.Current.configInfo.ValueDetail[j]);
							}
						}
						num++;
					}
				}
			}
			CommonData.stStatisticData.astDetail[(int)CommonData.stStatisticData.bNum].wNum = (ushort)num;
			COMDT_STATISTIC_DATA stStatisticData = CommonData.stStatisticData;
			COMDT_STATISTIC_DATA expr_1C8 = stStatisticData;
			expr_1C8.bNum += 1;
		}

		private void CreateBattleNonHeroData(ref COMDT_SETTLE_COMMON_DATA CommonData, bool bGMWin)
		{
		}

		private void CreateGeneralData(uint playerId, ref COMDT_SETTLE_COMMON_DATA CommonData, bool bGMWin)
		{
			CBattleDeadStat battleDeadStat = Singleton<BattleStatistic>.instance.m_battleDeadStat;
			COMDT_SETTLE_GAME_GENERAL_INFO stGeneralData = CommonData.stGeneralData;
			stGeneralData.dwFBTime = battleDeadStat.m_uiFBTime;
			stGeneralData.bKillDragonNum = (byte)battleDeadStat.GetKillDragonNum();
			uint data = CPlayerBehaviorStat.GetData(CPlayerBehaviorStat.BehaviorType.SortBYCoinBtnClick);
			stGeneralData.dwBoardSortNum = data;
			for (int i = 0; i < (int)stGeneralData.bKillDragonNum; i++)
			{
				if (i >= 10)
				{
					stGeneralData.bKillDragonNum = (byte)i;
					break;
				}
				stGeneralData.KillDragonTime[i] = (uint)battleDeadStat.GetDragonDeadTime(i);
			}
			int baronDeadCount = battleDeadStat.GetBaronDeadCount();
			stGeneralData.bKillBigDragonNum = (byte)baronDeadCount;
			for (int j = 0; j < baronDeadCount; j++)
			{
				if (j >= 10)
				{
					stGeneralData.bKillBigDragonNum = (byte)j;
					break;
				}
				stGeneralData.KillBigDragonTime[j] = (uint)battleDeadStat.GetBaronDeadTime(j);
			}
			List<CShenFuStat.ShenFuRecord> shenFuRecord = Singleton<BattleStatistic>.instance.m_shenFuStat.GetShenFuRecord(playerId);
			Dictionary<int, List<int>> dictionary = new Dictionary<int, List<int>>();
			for (int k = 0; k < shenFuRecord.get_Count(); k++)
			{
				CShenFuStat.ShenFuRecord shenFuRecord2 = shenFuRecord.get_Item(k);
				if (!dictionary.ContainsKey((int)shenFuRecord2.shenFuId))
				{
					List<int> list = new List<int>();
					list.Add((int)shenFuRecord2.shenFuId);
					dictionary.Add((int)shenFuRecord2.shenFuId, list);
				}
				else
				{
					List<int> list2 = dictionary.get_Item((int)shenFuRecord2.shenFuId);
					list2.Add((int)shenFuRecord2.shenFuId);
				}
			}
			int num = 0;
			using (Dictionary<int, List<int>>.KeyCollection.Enumerator enumerator = dictionary.get_Keys().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					int current = enumerator.get_Current();
					if (num >= 10)
					{
						break;
					}
					stGeneralData.astRuneTypePickUpNum[num].dwRuneID = (uint)current;
					stGeneralData.astRuneTypePickUpNum[num].dwPickUpNum = (uint)dictionary.get_Item(current).get_Count();
					num++;
				}
			}
			stGeneralData.bRuneTypeNum = (byte)num;
			PlayerKDA playerKDA = Singleton<BattleStatistic>.GetInstance().m_playerKDAStat.GetPlayerKDA(playerId);
			COM_PLAYERCAMP playerCamp = playerKDA.PlayerCamp;
			int num2 = (int)(Singleton<FrameSynchr>.GetInstance().LogicFrameTick * 0.001f);
			num2 -= 60;
			if (num2 > 0)
			{
				int num3 = num2 / 30;
				float num4 = (float)num2 * 1f / 30f;
				if (num4 - (float)num3 > 0f)
				{
					num3++;
				}
				if (num3 > 59)
				{
					num3 = 59;
				}
				stGeneralData.bGame30SecNum = (byte)num3;
				for (int l = 0; l < num3; l++)
				{
					if (l >= 60)
					{
						stGeneralData.bGame30SecNum = (byte)l;
						break;
					}
					int num5 = 60 + l * 30;
					COMDT_GAME_30SEC_INFO cOMDT_GAME_30SEC_INFO = stGeneralData.astGame30SecDetail[l];
					cOMDT_GAME_30SEC_INFO.dwGhostLevel = (uint)Singleton<BattleStatistic>.instance.m_playerSoulLevelStat.GetPlayerSoulLevelAtTime(playerId, num5 * 1000);
					cOMDT_GAME_30SEC_INFO.dwKillCnt = (uint)Singleton<BattleStatistic>.instance.m_battleDeadStat.GetHeroDeadAtTime(playerId, num5 * 1000);
					cOMDT_GAME_30SEC_INFO.dwKillDragonCnt = (uint)Singleton<BattleStatistic>.instance.m_battleDeadStat.GetKillDragonNumAtTime(playerId, num5 * 1000);
					cOMDT_GAME_30SEC_INFO.dwKillBigDragonCnt = (uint)Singleton<BattleStatistic>.instance.m_battleDeadStat.GetKillSpecialMonsterNumAtTime(playerId, num5 * 1000, 8);
					cOMDT_GAME_30SEC_INFO.dwDestroyTowerCnt = (uint)(Singleton<BattleStatistic>.instance.m_battleDeadStat.GetDeadNumAtTime(BattleLogic.GetOppositeCmp(playerKDA.PlayerCamp), ActorTypeDef.Actor_Type_Organ, 1, num5 * 1000) + Singleton<BattleStatistic>.instance.m_battleDeadStat.GetDeadNumAtTime(BattleLogic.GetOppositeCmp(playerKDA.PlayerCamp), ActorTypeDef.Actor_Type_Organ, 4, num5 * 1000));
					cOMDT_GAME_30SEC_INFO.dwDestroyOuterTowerCnt = (uint)Singleton<BattleStatistic>.instance.m_battleDeadStat.GetDeadNumAtTime(BattleLogic.GetOppositeCmp(playerKDA.PlayerCamp), ActorTypeDef.Actor_Type_Organ, 1, num5 * 1000);
					cOMDT_GAME_30SEC_INFO.dwDestroyBaseTowerCnt = (uint)Singleton<BattleStatistic>.instance.m_battleDeadStat.GetDeadNumAtTime(BattleLogic.GetOppositeCmp(playerKDA.PlayerCamp), ActorTypeDef.Actor_Type_Organ, 4, num5 * 1000);
					cOMDT_GAME_30SEC_INFO.dwBigDragonBuffCnt = (uint)Singleton<BattleStatistic>.instance.m_battleBuffStat.GetDataByIndex(playerKDA.PlayerCamp, l);
					cOMDT_GAME_30SEC_INFO.dwKillMonsterCnt = (uint)Singleton<BattleStatistic>.instance.m_battleDeadStat.GetMonsterDeadAtTime(playerId, num5 * 1000);
					cOMDT_GAME_30SEC_INFO.dwKillSoldierCnt = (uint)Singleton<BattleStatistic>.instance.m_battleDeadStat.GetSoldierDeadAtTime(playerId, num5 * 1000);
					cOMDT_GAME_30SEC_INFO.dwCoinCnt = playerKDA.GetPlayerCoinAtTimeWithType(l + 1, KDAStat.GET_COIN_CHANNEL_TYPE.GET_COIN_CHANNEL_TYPE_ALL);
					cOMDT_GAME_30SEC_INFO.dwSoliderKillCoin = playerKDA.GetPlayerCoinAtTimeWithType(l + 1, KDAStat.GET_COIN_CHANNEL_TYPE.GET_COIN_CHANNEL_TYPE_KILLSOLIDER);
					cOMDT_GAME_30SEC_INFO.dwMonsterKillCoin = playerKDA.GetPlayerCoinAtTimeWithType(l + 1, KDAStat.GET_COIN_CHANNEL_TYPE.GET_COIN_CHANNEL_TYPE_KILLMONSTER);
					cOMDT_GAME_30SEC_INFO.dwRedBuffCnt = (uint)Singleton<BattleStatistic>.instance.m_battleDeadStat.GetKillRedBaNumAtTime(playerId, num5 * 1000);
					cOMDT_GAME_30SEC_INFO.dwBlueBuffCnt = (uint)Singleton<BattleStatistic>.instance.m_battleDeadStat.GetKillBlueBaNumAtTime(playerId, num5 * 1000);
					JudgeStatc judgeStatc = default(JudgeStatc);
					PlayerKDA playerKDA2 = Singleton<BattleStatistic>.instance.m_playerKDAStat.GetPlayerKDA(playerId);
					ListView<HeroKDA>.Enumerator enumerator2 = playerKDA2.GetEnumerator();
					if (enumerator2.MoveNext() && enumerator2.Current.GetJudgeStatcByIndex(l, ref judgeStatc))
					{
						cOMDT_GAME_30SEC_INFO.wNoAIKillCnt = judgeStatc.KillNum;
						cOMDT_GAME_30SEC_INFO.wNoAIDeadCnt = judgeStatc.DeadNum;
						cOMDT_GAME_30SEC_INFO.wNoAIAssistCnt = judgeStatc.AssitNum;
						cOMDT_GAME_30SEC_INFO.wSelfBehaviorCoin = judgeStatc.GainCoin;
						cOMDT_GAME_30SEC_INFO.dwHurtHeroCnt = judgeStatc.HurtToHero;
						cOMDT_GAME_30SEC_INFO.dwBeHurtHeroCnt = judgeStatc.SufferHero;
					}
					CampJudgeRecord campJudgeRecord = default(CampJudgeRecord);
					if (Singleton<BattleStatistic>.instance.m_playerKDAStat.GetCampJudgeRecord(playerCamp, l, ref campJudgeRecord))
					{
						cOMDT_GAME_30SEC_INFO.wCampKillCnt = campJudgeRecord.killNum;
						cOMDT_GAME_30SEC_INFO.wCampDeadCnt = campJudgeRecord.deadNum;
						cOMDT_GAME_30SEC_INFO.dwTeamBehaviorCoin = campJudgeRecord.gainCoin;
						cOMDT_GAME_30SEC_INFO.dwTeamHurtHeroCnt = campJudgeRecord.hurtToHero;
						cOMDT_GAME_30SEC_INFO.dwTeamBeHurtHeroCnt = campJudgeRecord.sufferHero;
					}
					VInt2 timeLocation = Singleton<BattleStatistic>.instance.m_locStat.GetTimeLocation(playerId, l);
					cOMDT_GAME_30SEC_INFO.iXPos = timeLocation.x;
					cOMDT_GAME_30SEC_INFO.iZPos = timeLocation.y;
				}
			}
			stGeneralData.ullVisibleBits = Singleton<BattleStatistic>.GetInstance().u64EmenyIsVisibleToHostHero;
			stGeneralData.dwCamp1TowerFirstAttackTime = playerKDA.m_Camp1TowerFirstAttackTime;
			stGeneralData.dwCamp2TowerFirstAttackTime = playerKDA.m_Camp2TowerFirstAttackTime;
			stGeneralData.bCarrier = (byte)MonoSingleton<TdirMgr>.instance.GetISP();
			ListView<HeroKDA>.Enumerator enumerator3 = playerKDA.GetEnumerator();
			while (enumerator3.MoveNext())
			{
				stEquipInfo[] equips = enumerator3.Current.Equips;
				byte b = 0;
				int m = 0;
				int num6 = 0;
				while (m < equips.Length)
				{
					if (equips[m].m_equipID != 0)
					{
						stGeneralData.astEquipDetail[num6].bCnt = (byte)equips[m].m_amount;
						stGeneralData.astEquipDetail[num6].dwEquipID = (uint)equips[m].m_equipID;
						b += 1;
						num6++;
					}
					m++;
				}
				stGeneralData.bEquipNum = b;
			}
			PoolObjHandle<ActorRoot> captain = Singleton<GamePlayerCenter>.GetInstance().GetPlayer(playerId).Captain;
			if (captain && captain.handle.ValueComponent != null)
			{
				stGeneralData.dwTotalInGameCoin = (uint)captain.handle.ValueComponent.GetGoldCoinIncomeInBattle();
				stGeneralData.dwMaxInGameCoin = (uint)captain.handle.ValueComponent.GetMaxGoldCoinIncomeInBattle();
			}
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			if (curLvelContext.IsMobaModeWithOutGuide())
			{
				int num7 = 10;
				if (curLvelContext.m_mapType == 4 || curLvelContext.m_pvpPlayerNum < 10)
				{
					num7 = 3;
				}
				num7 *= captain.handle.TheActorMeta.ActorCamp - COM_PLAYERCAMP.COM_PLAYERCAMP_1;
				stGeneralData.dwUpRoadTower1DesTime = (uint)Singleton<BattleStatistic>.GetInstance().m_battleDeadStat.GetOrganTimeByOrder(num7 + 1);
				stGeneralData.dwUpRoadTower2DesTime = (uint)Singleton<BattleStatistic>.GetInstance().m_battleDeadStat.GetOrganTimeByOrder(num7 + 2);
				stGeneralData.dwUpRoadTower3DesTime = (uint)Singleton<BattleStatistic>.GetInstance().m_battleDeadStat.GetOrganTimeByOrder(num7 + 3);
				if (!curLvelContext.IsGameTypeEntertainment() && curLvelContext.m_pvpPlayerNum == 10)
				{
					stGeneralData.dwMidRoadTower1DesTime = (uint)Singleton<BattleStatistic>.GetInstance().m_battleDeadStat.GetOrganTimeByOrder(num7 + 4);
					stGeneralData.dwMidRoadTower2DesTime = (uint)Singleton<BattleStatistic>.GetInstance().m_battleDeadStat.GetOrganTimeByOrder(num7 + 5);
					stGeneralData.dwMidRoadTower3DesTime = (uint)Singleton<BattleStatistic>.GetInstance().m_battleDeadStat.GetOrganTimeByOrder(num7 + 6);
					stGeneralData.dwDownRoadTower1DesTime = (uint)Singleton<BattleStatistic>.GetInstance().m_battleDeadStat.GetOrganTimeByOrder(num7 + 7);
					stGeneralData.dwDownRoadTower2DesTime = (uint)Singleton<BattleStatistic>.GetInstance().m_battleDeadStat.GetOrganTimeByOrder(num7 + 8);
					stGeneralData.dwDownRoadTower3DesTime = (uint)Singleton<BattleStatistic>.GetInstance().m_battleDeadStat.GetOrganTimeByOrder(num7 + 9);
				}
			}
			uint num8 = (uint)Singleton<BattleStatistic>.instance.m_battleDeadStat.GetTotalNum(COM_PLAYERCAMP.COM_PLAYERCAMP_MID, ActorTypeDef.Actor_Type_Monster, 2, 8);
			if (num8 > 10u)
			{
				num8 = 10u;
			}
			stGeneralData.dwBigDragonBattleToDeadTimeNum = num8;
			for (uint num9 = 0u; num9 < stGeneralData.dwBigDragonBattleToDeadTimeNum; num9 += 1u)
			{
				stGeneralData.BigDragonBattleToDeadTimeDtail[(int)((uint)((UIntPtr)num9))] = (uint)Singleton<BattleStatistic>.instance.m_battleDeadStat.GetRecordAtIndex(COM_PLAYERCAMP.COM_PLAYERCAMP_MID, ActorTypeDef.Actor_Type_Monster, 2, 8, (int)num9).fightTime;
			}
			if (curLvelContext != null && curLvelContext.IsMobaModeWithOutGuide())
			{
				if (curLvelContext.m_pvpPlayerNum == 10)
				{
					uint num10 = (uint)Singleton<BattleStatistic>.instance.m_battleDeadStat.GetTotalNum(COM_PLAYERCAMP.COM_PLAYERCAMP_MID, ActorTypeDef.Actor_Type_Monster, 2, 9);
					if (num10 > 10u)
					{
						num10 = 10u;
					}
					stGeneralData.dwDragonBattleToDeadTimeNum = num10;
					for (uint num11 = 0u; num11 < stGeneralData.dwDragonBattleToDeadTimeNum; num11 += 1u)
					{
						stGeneralData.DragonBattleToDeadTimeDtail[(int)((uint)((UIntPtr)num11))] = (uint)Singleton<BattleStatistic>.instance.m_battleDeadStat.GetRecordAtIndex(COM_PLAYERCAMP.COM_PLAYERCAMP_MID, ActorTypeDef.Actor_Type_Monster, 2, 9, (int)num11).fightTime;
					}
				}
				else
				{
					uint num12 = (uint)Singleton<BattleStatistic>.instance.m_battleDeadStat.GetTotalNum(COM_PLAYERCAMP.COM_PLAYERCAMP_MID, ActorTypeDef.Actor_Type_Monster, 2, 7);
					if (num12 > 10u)
					{
						num12 = 10u;
					}
					stGeneralData.dwDragonBattleToDeadTimeNum = num12;
					for (uint num13 = 0u; num13 < stGeneralData.dwDragonBattleToDeadTimeNum; num13 += 1u)
					{
						stGeneralData.DragonBattleToDeadTimeDtail[(int)((uint)((UIntPtr)num13))] = (uint)Singleton<BattleStatistic>.instance.m_battleDeadStat.GetRecordAtIndex(COM_PLAYERCAMP.COM_PLAYERCAMP_MID, ActorTypeDef.Actor_Type_Monster, 2, 7, (int)num13).fightTime;
					}
				}
				if (curLvelContext.m_pvpPlayerNum == 10)
				{
					int[] array = new int[]
					{
						6006,
						6008,
						6007,
						6013,
						6011,
						6010
					};
					uint num14 = (uint)array.Length;
					if (num14 > 20u)
					{
						num14 = 20u;
					}
					stGeneralData.dwMonsterDeadNum = num14;
					for (uint num15 = 0u; num15 < stGeneralData.dwMonsterDeadNum; num15 += 1u)
					{
						stGeneralData.MonsterDeadDetail[(int)((uint)((UIntPtr)num15))] = (uint)Singleton<BattleStatistic>.instance.m_battleDeadStat.GetDeadNum(COM_PLAYERCAMP.COM_PLAYERCAMP_MID, ActorTypeDef.Actor_Type_Monster, 2, array[(int)((uint)((UIntPtr)num15))]);
					}
				}
				else
				{
					int[] array2 = new int[]
					{
						30,
						31,
						32,
						33,
						42,
						43,
						44,
						45,
						49,
						59
					};
					uint num16 = (uint)array2.Length;
					if (num16 > 20u)
					{
						num16 = 20u;
					}
					stGeneralData.dwMonsterDeadNum = num16;
					for (uint num17 = 0u; num17 < stGeneralData.dwMonsterDeadNum; num17 += 1u)
					{
						stGeneralData.MonsterDeadDetail[(int)((uint)((UIntPtr)num17))] = (uint)Singleton<BattleStatistic>.instance.m_battleDeadStat.GetDeadNum(COM_PLAYERCAMP.COM_PLAYERCAMP_MID, ActorTypeDef.Actor_Type_Monster, 2, array2[(int)((uint)((UIntPtr)num17))]);
					}
				}
			}
			stGeneralData.iTimeUse = (int)(Singleton<FrameSynchr>.instance.LogicFrameTick * 0.001f);
			stGeneralData.iPauseTimeTotal = (int)(Time.timeSinceLevelLoad - (float)stGeneralData.iTimeUse - 15f);
			if (stGeneralData.iPauseTimeTotal < 0)
			{
				stGeneralData.iPauseTimeTotal = 0;
			}
			COM_PLAYERCAMP playerCamp2 = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer().PlayerCamp;
			DictionaryView<uint, DictionaryView<uint, NONHERO_STATISTIC_INFO>> nonHeroInfo = Singleton<BattleStatistic>.instance.m_NonHeroInfo;
			DictionaryView<uint, NONHERO_STATISTIC_INFO> dictionaryView;
			if (nonHeroInfo.TryGetValue(2u, out dictionaryView))
			{
				NONHERO_STATISTIC_INFO nONHERO_STATISTIC_INFO;
				if (dictionaryView.TryGetValue((uint)playerCamp2, out nONHERO_STATISTIC_INFO))
				{
					stGeneralData.iBuildingAttackRange = (int)nONHERO_STATISTIC_INFO.uiAttackDistanceMax;
					stGeneralData.iBuildingAttackDamageMax = (int)nONHERO_STATISTIC_INFO.uiHurtMax;
					if (nONHERO_STATISTIC_INFO.uiHurtMin != 4294967295u)
					{
						stGeneralData.iBuildingAttackDamageMin = (int)nONHERO_STATISTIC_INFO.uiHurtMin;
					}
					stGeneralData.iBuildingHPMax = (int)nONHERO_STATISTIC_INFO.uiHpMax;
					if (nONHERO_STATISTIC_INFO.uiHpMin != 4294967295u)
					{
						stGeneralData.iBuildingHPMin = (int)nONHERO_STATISTIC_INFO.uiHpMin;
					}
					stGeneralData.iBuildingHurtCount = (int)nONHERO_STATISTIC_INFO.uiTotalBeAttackedNum;
					stGeneralData.iBuildingHurtMax = (int)nONHERO_STATISTIC_INFO.uiBeHurtMax;
					if (nONHERO_STATISTIC_INFO.uiBeHurtMin != 4294967295u)
					{
						stGeneralData.iBuildingHurtMin = (int)nONHERO_STATISTIC_INFO.uiBeHurtMin;
					}
					stGeneralData.iBuildingHurtTotal = (int)nONHERO_STATISTIC_INFO.uiTotalBeHurtCount;
				}
				else
				{
					stGeneralData.iBuildingHurtCount = 0;
					stGeneralData.iBuildingHurtMax = 0;
					stGeneralData.iBuildingHurtMin = 0;
					stGeneralData.iBuildingHurtTotal = 0;
					stGeneralData.iBuildingAttackDamageMax = 0;
					stGeneralData.iBuildingAttackDamageMin = 0;
					stGeneralData.iBuildingAttackRange = 0;
					List<PoolObjHandle<ActorRoot>> organActors = Singleton<GameObjMgr>.instance.OrganActors;
					for (int n = 0; n < organActors.get_Count(); n++)
					{
						PoolObjHandle<ActorRoot> poolObjHandle = organActors.get_Item(n);
						if (ActorHelper.IsHostEnemyActor(ref poolObjHandle))
						{
							int actorHp = poolObjHandle.handle.ValueComponent.actorHp;
							stGeneralData.iBuildingHPMax = Math.Max(actorHp, stGeneralData.iBuildingHPMax);
							if (stGeneralData.iBuildingHPMin == 0)
							{
								stGeneralData.iBuildingHPMin = actorHp;
							}
							else
							{
								stGeneralData.iBuildingHPMin = Math.Min(actorHp, stGeneralData.iBuildingHPMin);
							}
						}
					}
				}
			}
			if (nonHeroInfo.TryGetValue(2u, out dictionaryView))
			{
				COM_PLAYERCAMP key = (playerCamp2 == COM_PLAYERCAMP.COM_PLAYERCAMP_1) ? COM_PLAYERCAMP.COM_PLAYERCAMP_2 : COM_PLAYERCAMP.COM_PLAYERCAMP_1;
				NONHERO_STATISTIC_INFO nONHERO_STATISTIC_INFO2;
				if (dictionaryView.TryGetValue((uint)key, out nONHERO_STATISTIC_INFO2))
				{
					stGeneralData.iEnemyBuildingAttackRange = (int)nONHERO_STATISTIC_INFO2.uiAttackDistanceMax;
					stGeneralData.iEnemyBuildingDamageMax = (int)nONHERO_STATISTIC_INFO2.uiHurtMax;
					if (nONHERO_STATISTIC_INFO2.uiHurtMin != 4294967295u)
					{
						stGeneralData.iEnemyBuildingDamageMin = (int)nONHERO_STATISTIC_INFO2.uiHurtMin;
					}
					stGeneralData.iEnemyBuildingHPMax = (int)nONHERO_STATISTIC_INFO2.uiHpMax;
					if (nONHERO_STATISTIC_INFO2.uiHpMin != 4294967295u)
					{
						stGeneralData.iEnemyBuildingHPMin = (int)nONHERO_STATISTIC_INFO2.uiHpMin;
					}
					stGeneralData.iEnemyBuildingHurtMax = (int)nONHERO_STATISTIC_INFO2.uiBeHurtMax;
					if (nONHERO_STATISTIC_INFO2.uiBeHurtMin != 4294967295u)
					{
						stGeneralData.iEnemyBuildingHurtMin = (int)nONHERO_STATISTIC_INFO2.uiBeHurtMin;
					}
					stGeneralData.iEnemyBuildingHurtTotal = (int)nONHERO_STATISTIC_INFO2.uiTotalBeHurtCount;
				}
				else
				{
					stGeneralData.iEnemyBuildingHurtMax = 0;
					stGeneralData.iEnemyBuildingHurtMin = 0;
					stGeneralData.iEnemyBuildingHurtTotal = 0;
					stGeneralData.iEnemyBuildingDamageMax = 0;
					stGeneralData.iEnemyBuildingDamageMin = 0;
					stGeneralData.iEnemyBuildingAttackRange = 0;
					List<PoolObjHandle<ActorRoot>> organActors2 = Singleton<GameObjMgr>.instance.OrganActors;
					for (int num18 = 0; num18 < organActors2.get_Count(); num18++)
					{
						PoolObjHandle<ActorRoot> poolObjHandle2 = organActors2.get_Item(num18);
						if (!ActorHelper.IsHostEnemyActor(ref poolObjHandle2))
						{
							int actorHp2 = poolObjHandle2.handle.ValueComponent.actorHp;
							stGeneralData.iEnemyBuildingHPMax = Math.Max(actorHp2, stGeneralData.iEnemyBuildingHPMax);
							if (stGeneralData.iEnemyBuildingHPMin == 0)
							{
								stGeneralData.iEnemyBuildingHPMin = actorHp2;
							}
							else
							{
								stGeneralData.iEnemyBuildingHPMin = Math.Min(actorHp2, stGeneralData.iEnemyBuildingHPMin);
							}
						}
					}
				}
			}
			if (Singleton<BattleStatistic>.instance.m_stSoulStatisticInfo != null)
			{
				GET_SOUL_EXP_STATISTIC_INFO stSoulStatisticInfo = Singleton<BattleStatistic>.instance.m_stSoulStatisticInfo;
				stGeneralData.iExperienceHPAdd1 = stSoulStatisticInfo.iKillSoldierExpMax;
				stGeneralData.iExperienceHPAdd2 = stSoulStatisticInfo.iKillHeroExpMax;
				stGeneralData.iExperienceHPAdd3 = stSoulStatisticInfo.iKillOrganExpMax;
				stGeneralData.iExperienceHPAdd4 = stSoulStatisticInfo.iKillMonsterExpMax;
				stGeneralData.iExperienceHPAddTotal = stSoulStatisticInfo.iAddExpTotal;
			}
			if (nonHeroInfo.TryGetValue(1u, out dictionaryView))
			{
				COM_PLAYERCAMP key2 = (playerCamp2 == COM_PLAYERCAMP.COM_PLAYERCAMP_1) ? COM_PLAYERCAMP.COM_PLAYERCAMP_2 : COM_PLAYERCAMP.COM_PLAYERCAMP_1;
				NONHERO_STATISTIC_INFO nONHERO_STATISTIC_INFO3;
				if (dictionaryView.TryGetValue((uint)key2, out nONHERO_STATISTIC_INFO3))
				{
					stGeneralData.iEnemyAttackMax = (int)nONHERO_STATISTIC_INFO3.uiHurtMax;
					stGeneralData.iEnemyAttackMin = (int)nONHERO_STATISTIC_INFO3.uiHurtMin;
					stGeneralData.iEnemyHPMax = (int)nONHERO_STATISTIC_INFO3.uiHpMax;
					stGeneralData.iEnemyHPMin = (int)nONHERO_STATISTIC_INFO3.uiHpMin;
					if (stGeneralData.iEnemyHPMin == -1)
					{
						stGeneralData.iEnemyHPMin = 0;
					}
				}
			}
			List<PoolObjHandle<ActorRoot>> list3 = ActorHelper.FilterActors(Singleton<GameObjMgr>.instance.HeroActors, new ActorFilterDelegate(Singleton<BattleLogic>.instance.FilterEnemyActor));
			stGeneralData.iBossCount = list3.get_Count();
			for (int num19 = 0; num19 < list3.get_Count(); num19++)
			{
				stGeneralData.iBossHPMax = Math.Max(stGeneralData.iBossHPMax, list3.get_Item(num19).handle.ValueComponent.ObjValueStatistic.iActorMaxHp);
				if (list3.get_Item(num19).handle.ValueComponent.ObjValueStatistic.iActorMinHp != -1)
				{
					if (num19 == 0)
					{
						stGeneralData.iBossHPMin = list3.get_Item(num19).handle.ValueComponent.ObjValueStatistic.iActorMinHp;
					}
					else
					{
						stGeneralData.iBossHPMin = Math.Min(stGeneralData.iBossHPMin, list3.get_Item(num19).handle.ValueComponent.ObjValueStatistic.iActorMinHp);
					}
				}
				for (int num20 = 0; num20 < 10; num20++)
				{
					if (list3.get_Item(num19) && list3.get_Item(num19).handle.SkillControl != null && list3.get_Item(num19).handle.SkillControl.stSkillStat != null && list3.get_Item(num19).handle.SkillControl.stSkillStat.SkillStatistictInfo != null)
					{
						SKILLSTATISTICTINFO[] skillStatistictInfo = list3.get_Item(num19).handle.SkillControl.stSkillStat.SkillStatistictInfo;
						stGeneralData.iBossHurtMax = Math.Max(stGeneralData.iBossHurtMax, skillStatistictInfo[num20].iHurtMax);
						if ((num19 == 0 && num20 == 0) || stGeneralData.iBossHurtMin == -1)
						{
							stGeneralData.iBossHurtMin = skillStatistictInfo[num20].iHurtMin;
						}
						else if (skillStatistictInfo[num20].iHurtMin != -1)
						{
							stGeneralData.iBossHurtMin = Math.Min(stGeneralData.iBossHurtMin, skillStatistictInfo[num20].iHurtMin);
						}
						stGeneralData.iBossHurtTotal += skillStatistictInfo[num20].iHurtTotal;
						if (num20 == 0)
						{
							stGeneralData.iBossAttackCount += (int)skillStatistictInfo[num20].uiUsedTimes;
							stGeneralData.iBossAttackMax = Math.Max(stGeneralData.iBossAttackMax, skillStatistictInfo[num20].iHurtMax);
							if (skillStatistictInfo[num20].iHurtMin != -1)
							{
								if (num19 == 0)
								{
									stGeneralData.iBossAttackMin = skillStatistictInfo[num20].iHurtMin;
								}
								else
								{
									stGeneralData.iBossAttackMin = Math.Min(stGeneralData.iBossAttackMin, skillStatistictInfo[num20].iHurtMin);
								}
							}
						}
						else
						{
							stGeneralData.iBossUseSkillCount += (int)skillStatistictInfo[num20].uiUsedTimes;
							stGeneralData.iBossSkillDamageMax = Math.Max(stGeneralData.iBossSkillDamageMax, skillStatistictInfo[num20].iHurtMax);
							if (num19 == 0)
							{
								if (skillStatistictInfo[num20].iHurtMin != -1)
								{
									stGeneralData.iBossSkillDamageMin = skillStatistictInfo[num20].iHurtMin;
								}
							}
							else if (skillStatistictInfo[num20].iHurtMin != -1)
							{
								stGeneralData.iBossSkillDamageMin = Math.Min(stGeneralData.iBossSkillDamageMin, skillStatistictInfo[num20].iHurtMin);
							}
						}
						stGeneralData.iBossAttackTotal += skillStatistictInfo[num20].ihurtCount;
					}
				}
			}
			stGeneralData.iCommunicationCount2 = (int)Singleton<NetworkModule>.instance.RecvGameMsgCount;
			stGeneralData.iCommunicationCount1 = (int)Singleton<NetworkModule>.instance.RecvGameMsgCount;
			stGeneralData.bSelfCampKillTowerCnt = Singleton<BattleStatistic>.instance.m_battleDeadStat.GetDestroyTowerCount(playerKDA.PlayerCamp, 1);
			stGeneralData.bSelfCampKillBaseCnt = Singleton<BattleStatistic>.instance.m_battleDeadStat.GetDestroyTowerCount(playerKDA.PlayerCamp, 2);
			this.FillVDStat(ref stGeneralData, playerId);
			List<PoolObjHandle<ActorRoot>> organActors3 = Singleton<GameObjMgr>.instance.OrganActors;
			for (int num21 = 0; num21 < organActors3.get_Count(); num21++)
			{
				PoolObjHandle<ActorRoot> poolObjHandle3 = organActors3.get_Item(num21);
				if (poolObjHandle3.handle.TheActorMeta.ActorCamp == captain.handle.TheActorMeta.ActorCamp && poolObjHandle3.handle.TheStaticData.TheOrganOnlyInfo.OrganType == 2 && poolObjHandle3.handle.ValueComponent != null)
				{
					stGeneralData.dwSelfCampBaseBlood = (uint)poolObjHandle3.handle.ValueComponent.actorHp;
				}
			}
			if (captain && captain.handle.EquipComponent != null)
			{
				stGeneralData.bQuickBuyItemCnt = (byte)captain.handle.EquipComponent.m_iFastBuyEquipCount;
				stGeneralData.bPanelBuyItemCnt = (byte)captain.handle.EquipComponent.m_iBuyEquipCount;
			}
			int num22 = 0;
			stGeneralData.dwClickNum = 23u;
			for (int num23 = 1; num23 <= 23; num23++)
			{
				if (num22 >= 30)
				{
					stGeneralData.dwClickNum = (uint)num22;
					break;
				}
				stGeneralData.ClickDetail[num22] = CPlayerBehaviorStat.GetData((CPlayerBehaviorStat.BehaviorType)num23);
				num22++;
			}
			stGeneralData.dwOperateNum = 11u;
			stGeneralData.OperateType[0] = (uint)GameSettings.TheCommonAttackType;
			stGeneralData.OperateType[1] = (uint)GameSettings.TheCastType;
			stGeneralData.OperateType[2] = (uint)GameSettings.TheSelectType;
			stGeneralData.OperateType[3] = (uint)GameSettings.LunPanSensitivity;
			stGeneralData.OperateType[4] = (uint)GameSettings.JoyStickShowType;
			stGeneralData.OperateType[5] = (uint)GameSettings.TheSkillCancleType;
			stGeneralData.OperateType[6] = (GameSettings.ShowEnemyHeroHeadBtnMode ? 1u : 0u);
			stGeneralData.OperateType[7] = (GameSettings.LunPanLockEnemyHeroMode ? 1u : 0u);
			stGeneralData.OperateType[8] = (uint)GameSettings.TheLastHitMode;
			stGeneralData.OperateType[9] = (GameSettings.ShowEquipInfo ? 1u : 0u);
			stGeneralData.OperateType[10] = (uint)GameSettings.TheAttackOrganMode;
			stGeneralData.OperateType[11] = (uint)GameSettings.EquipPosMode;
			stGeneralData.iEnemyKIllHeroGap = Singleton<BattleStatistic>.instance.m_battleDeadStat.GetEnemyKillHeroMaxGap(playerKDA.PlayerCamp);
		}

		private void FillVDStat(ref COMDT_SETTLE_GAME_GENERAL_INFO stGeneralData, uint playerID)
		{
			if (Singleton<BattleStatistic>.instance.m_vdStat != null)
			{
				Player player = Singleton<GamePlayerCenter>.instance.GetPlayer(playerID);
				if (player != null)
				{
					COM_PLAYERCAMP inTo = COM_PLAYERCAMP.COM_PLAYERCAMP_MID;
					if (player.PlayerCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
					{
						inTo = COM_PLAYERCAMP.COM_PLAYERCAMP_2;
					}
					else if (player.PlayerCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_2)
					{
						inTo = COM_PLAYERCAMP.COM_PLAYERCAMP_1;
					}
					stGeneralData.bAdvantageNum = (((byte)Singleton<BattleStatistic>.instance.m_vdStat.count < 16) ? ((byte)Singleton<BattleStatistic>.instance.m_vdStat.count) : 16);
					for (int i = 0; i < (int)stGeneralData.bAdvantageNum; i++)
					{
						Singleton<BattleStatistic>.instance.m_vdStat.GetMaxCampStat(i, player.PlayerCamp, inTo, out stGeneralData.astAdvantageDetail[i].iMaxAdvantageValue, out stGeneralData.astAdvantageDetail[i].iMaxDisadvantageValue);
					}
					stGeneralData.iCurrentDisparity = Singleton<BattleStatistic>.instance.m_vdStat.CalcCampStat(player.PlayerCamp, inTo);
					stGeneralData.bSelfCampHaveWinningFlag = (Singleton<BattleStatistic>.instance.bSelfCampHaveWinningFlag ? 1 : 0);
				}
			}
		}

		public void GotoAccLoginPage()
		{
			Singleton<GameBuilder>.GetInstance().EndGame();
			Singleton<LobbySvrMgr>.GetInstance().isLogin = false;
			Singleton<LobbyLogic>.GetInstance().isLogin = false;
			Singleton<LobbySvrMgr>.GetInstance().isFirstLogin = false;
			if (!Singleton<ApolloHelper>.GetInstance().Logout())
			{
				Singleton<ApolloHelper>.GetInstance().Logout();
			}
			Singleton<GameStateCtrl>.GetInstance().GotoState("LoginState");
			Singleton<GameLogic>.GetInstance().OnPlayerLogout();
		}

		public void OnSendSingleGameFinishFail()
		{
			Singleton<CUIManager>.GetInstance().OpenMessageBox("网络异常，点击确定返回大厅。", enUIEventID.Net_SingleGameFinishError, false);
		}

		protected void OnClickSingleGameFinishError(CUIEvent e)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			Singleton<GameBuilder>.instance.EndGame();
		}

		public void SvrNtfUpdateClient()
		{
			if (Singleton<BattleLogic>.instance.isRuning)
			{
				this.NeedUpdateClient = true;
			}
			else
			{
				Singleton<CUIManager>.GetInstance().OpenMessageBox(Singleton<CTextManager>.GetInstance().GetText("VersionIsLow"), enUIEventID.None, false);
			}
		}

		public static bool IsLobbyFormPure()
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CLobbySystem.LOBBY_FORM_PATH);
			return null != form && !form.IsHided();
		}

		public bool ExecOpenForm(string[] prms = null)
		{
			if (prms != null)
			{
				this._lobbyOpenFormCmd = prms;
			}
			if (!Singleton<CLobbySystem>.GetInstance().IsInLobbyForm() || !this.isLogin)
			{
				return false;
			}
			if (this._lobbyOpenFormCmd == null || this._lobbyOpenFormCmd.Length < 2 || this._lobbyOpenFormCmd[0] != "OpenForm")
			{
				this._lobbyOpenFormCmd = null;
				return false;
			}
			int entranceType = 0;
			int targetId = 0;
			int targetId2 = 0;
			int.TryParse(this._lobbyOpenFormCmd[1], ref entranceType);
			if (this._lobbyOpenFormCmd.Length > 2)
			{
				int.TryParse(this._lobbyOpenFormCmd[2], ref targetId);
			}
			if (this._lobbyOpenFormCmd.Length > 3)
			{
				int.TryParse(this._lobbyOpenFormCmd[3], ref targetId2);
			}
			this._lobbyOpenFormCmd = null;
			return CUICommonSystem.JumpForm((RES_GAME_ENTRANCE_TYPE)entranceType, targetId, targetId2, null);
		}
	}
}
