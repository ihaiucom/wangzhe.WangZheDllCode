using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.UI;
using ResData;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class CRecordUseSDK : Singleton<CRecordUseSDK>
	{
		public enum RECORD_EVENT_PRIORITY
		{
			RECORD_EVENT_TYPE_INVALID,
			RECORD_EVENT_TYPE_ASSIST,
			RECORD_EVENT_TYPE_ONCEKILL,
			RECORD_EVENT_TYPE_DOUBLEKILL,
			RECORD_EVENT_TYPE_TRIPLEKILL,
			RECORD_EVENT_TYPE_QUATARYKILL,
			RECORD_EVENT_TYPE_PENTAKILL
		}

		public enum CHECK_PERMISSION_STUTAS
		{
			CHECK_PERMISSION_STUTAS_TYPE_WITHOUTRESULT = -1,
			CHECK_PERMISSION_STUTAS_TYPE_NOPERMISSION,
			CHECK_PERMISSION_STUTAS_TYPE_PERMISSIONOK
		}

		public enum CHECK_WHITELIST_STATUS
		{
			CHECK_WHITELIST_STATUS_TYPE_INVALID,
			CHECK_WHITELIST_STATUS_TYPE_AUTOCHECK,
			CHECK_WHITELIST_STATUS_TYPE_TIMEUP,
			CHECK_WHITELIST_STATUS_TYPE_RESULTOK,
			CHECK_WHITELIST_STATUS_TYPE_RESULTFAILED,
			CHECK_WHITELIST_STATUS_TYPE_VIDEOMGRCLICK,
			CHECK_WHITELIST_STATUS_TYPE_LOBBYRECORDER
		}

		public enum CHECK_FROM_TYPE
		{
			CHECK_FROM_TYPE_KINGTIME,
			CHECK_FROM_TYPE_RECORDER
		}

		public struct RECORD_INFO
		{
			public CRecordUseSDK.RECORD_EVENT_PRIORITY eventPriority;

			public long lEndTime;

			public RECORD_INFO(CRecordUseSDK.RECORD_EVENT_PRIORITY _eventPriority, long _lEndTime)
			{
				this.eventPriority = _eventPriority;
				this.lEndTime = _lEndTime;
			}
		}

		private const string MODENAME = "MN";

		private const string GRADENAME = "GN";

		private const string HERONAME = "HN";

		private const string KILLNUM = "KN";

		private const string DEADNUM = "DN";

		private const string ASSISTNUM = "AN";

		private const string PLAYERNAME = "PN";

		private const string WORLDID = "WID";

		private const string PLATFORM = "PF";

		private const string VIPLEVEL = "VL";

		private const string PLAYERLEVEL = "PL";

		private const string HURTVALUE = "HV";

		private const string BEHURTVALUE = "BHV";

		private const string EQUIPNAME = "EN";

		private const string SKILLNAME = "SN";

		private const string ISMVP = "MVP";

		private const string ISTRIPLEKILL = "3k";

		private const string ISQUATARYKILL = "4k";

		private const string ISPENTAKILL = "5k";

		private const string ISLEGENDARY = "CS";

		private const string WINORLOSE = "WL";

		private const string GAMEID = "GID";

		private CRecordUseSDK.CHECK_PERMISSION_STUTAS m_enmCheckPermissionRes = CRecordUseSDK.CHECK_PERMISSION_STUTAS.CHECK_PERMISSION_STUTAS_TYPE_WITHOUTRESULT;

		private CRecordUseSDK.RECORD_EVENT_PRIORITY m_enLastEventPriority;

		private long m_lLastEventStartTime;

		private long m_lLastEventEndTime;

		private uint m_uiMinSpaceLimit = 200u;

		private uint m_uiWarningSpaceLimit = 500u;

		private bool m_bIsStartRecordOk;

		private Transform m_RecorderPanel;

		private uint m_uiEventStartTimeInterval = 5000u;

		private uint m_uiEventEndTimeInterval = 10000u;

		private uint m_uiEventNumMax = 5u;

		private uint m_ui543KillEventTotalTime = 90000u;

		private uint m_ui210KillEventTotalTime = 60000u;

		private long m_lGameEndTime;

		private long m_lGameStartTime;

		private int m_iContinuKillMaxNum = -1;

		private PoolObjHandle<ActorRoot> m_hostActor;

		private string m_strHostPlayerName;

		private string m_strHostHeroName;

		private bool m_bIsMvp;

		private bool m_bIsRecordMomentsEnable;

		private GameObject m_objKingBar;

		private long m_lVideoTimeLen;

		private uint m_uiOnceDoubleEventTimeIntervalReduce = 5000u;

		private Dictionary<CRecordUseSDK.RECORD_EVENT_PRIORITY, SortedList<long, long>> m_stRecordInfo = new Dictionary<CRecordUseSDK.RECORD_EVENT_PRIORITY, SortedList<long, long>>();

		private bool m_bIsCallGameJoyGenerate;

		private bool m_bIsCallStopGameJoyRecord;

		private CRecordUseSDK.CHECK_WHITELIST_STATUS m_enmCheckWhiteListStatus;

		private bool m_bIsRecorderShowing;

		private CRecordUseSDK.CHECK_FROM_TYPE m_enmCheckFromType;

		private uint m_uiFreeRecorderMinSpaceLimit = 400u;

		private bool m_bIsFirstOpenLobby = true;

		private Dictionary<string, string> m_extraInfos = new Dictionary<string, string>();

		private Vector2 m_Vec2FreeRecordWindowPosInLobby = Vector2.zero;

		private Vector2 m_Vec2FreeRecordWindowPosInBattle = Vector2.zero;

		private bool m_bIsShowingFreeRecorder;

		private string m_strGameId = string.Empty;

		private void Reset()
		{
			this.m_enLastEventPriority = CRecordUseSDK.RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_INVALID;
			this.m_lLastEventStartTime = 0L;
			this.m_lLastEventEndTime = 0L;
			this.m_lGameStartTime = 0L;
			this.m_lGameEndTime = 0L;
			this.m_iContinuKillMaxNum = -1;
			this.m_bIsStartRecordOk = false;
			this.m_bIsMvp = false;
			this.m_bIsCallGameJoyGenerate = false;
			this.m_bIsCallStopGameJoyRecord = false;
			this.m_lVideoTimeLen = 0L;
			if (this.m_hostActor)
			{
				this.m_hostActor.Release();
			}
			if (this.m_stRecordInfo != null)
			{
				this.m_stRecordInfo.Clear();
			}
			this.m_strHostPlayerName = string.Empty;
			this.m_strHostHeroName = string.Empty;
		}

		public override void Init()
		{
			base.Init();
			this.m_uiEventStartTimeInterval = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_RECORDER_KINGTIME_EVENTSTARTTIMEINTERVAL) * 1000u;
			this.m_uiEventEndTimeInterval = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_RECORDER_KINGTIME_EVENTENDTIMEINTERVAL) * 1000u;
			this.m_uiEventNumMax = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_RECORDER_KINGTIME_EVENTNUMMAX);
			this.m_ui543KillEventTotalTime = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_RECORDER_KINGTIME_VIDEO543KILLTOTALTIME) * 1000u;
			this.m_ui210KillEventTotalTime = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_RECORDER_KINGTIME_VIDEO210KILLTOTALTIME) * 1000u;
			this.m_uiMinSpaceLimit = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_RECORDER_KINGTIME_ANDROIDMINSPACELIMIT);
			this.m_uiFreeRecorderMinSpaceLimit = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_RECORDER_FREERECORDER_ANDROIDMINSPACELIMIT);
			this.m_uiOnceDoubleEventTimeIntervalReduce = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_RECORDER_KINGTIME_ONCEDOUBLEEVENTTIMEINTERVAL) * 1000u;
			this.Reset();
			Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_FightPrepare, new RefAction<DefaultGameEventParam>(this.OnFightPrepare));
			Singleton<GameEventSys>.instance.AddEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.OnActorDead));
			Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_DoubleKill, new RefAction<DefaultGameEventParam>(this.OnActorDoubleKill));
			Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_TripleKill, new RefAction<DefaultGameEventParam>(this.OnActorTripleKill));
			Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_QuataryKill, new RefAction<DefaultGameEventParam>(this.OnActorQuataryKill));
			Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_PentaKill, new RefAction<DefaultGameEventParam>(this.OnActorPentaKill));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Record_Save_Moment_Video, new CUIEventManager.OnUIEventHandler(this.OnSaveMomentVideo));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Record_Save_Moment_Video_Cancel, new CUIEventManager.OnUIEventHandler(this.OnSaveMomentVideoCancel));
			Singleton<EventRouter>.GetInstance().AddEventHandler<bool>(EventID.GAMEJOY_STARTRECORDING_RESULT, new Action<bool>(this.OnGameJoyStartRecordResult));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.OB_Video_Btn_VideoMgr_Click, new CUIEventManager.OnUIEventHandler(this.OnBtnVideoMgrClick));
			Singleton<EventRouter>.GetInstance().AddEventHandler<bool>(EventID.GAMEJOY_SDK_PERMISSION_CHECK_RESULT, new Action<bool>(this.OnGameJoyCheckPermissionResult));
			Singleton<EventRouter>.GetInstance().AddEventHandler<GameJoy.SDKFeature>(EventID.GAMEJOY_SDK_FEATURE_CHECK_RESULT, new Action<GameJoy.SDKFeature>(this.OnGameJoyCheckAvailabilityResult));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Record_Check_WhiteList_TimeUp, new CUIEventManager.OnUIEventHandler(this.OnCheckWhiteListTimeUp));
			Singleton<EventRouter>.GetInstance().AddEventHandler<long>(EventID.GAMEJOY_STOPRECORDING_RESULT, new Action<long>(this.OnGameJoyStopRecordResult));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Lobby_OpenLobbyForm, new CUIEventManager.OnUIEventHandler(this.OnLobbyFormOpen));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Loading_LoadingFromOpen, new CUIEventManager.OnUIEventHandler(this.OnLoadingFormOpen));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Loading_LoadingFromClose, new CUIEventManager.OnUIEventHandler(this.OnLoadingFormClose));
			this.m_bIsRecorderShowing = false;
			this.m_bIsFirstOpenLobby = true;
			this.m_Vec2FreeRecordWindowPosInLobby = Vector2.zero;
			this.m_Vec2FreeRecordWindowPosInBattle = Vector2.zero;
		}

		public override void UnInit()
		{
			base.UnInit();
			if (this.m_RecorderPanel)
			{
				this.m_stRecordInfo.Clear();
			}
			Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_FightPrepare, new RefAction<DefaultGameEventParam>(this.OnFightPrepare));
			Singleton<GameEventSys>.instance.RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.OnActorDead));
			Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_DoubleKill, new RefAction<DefaultGameEventParam>(this.OnActorDoubleKill));
			Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_TripleKill, new RefAction<DefaultGameEventParam>(this.OnActorTripleKill));
			Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_QuataryKill, new RefAction<DefaultGameEventParam>(this.OnActorQuataryKill));
			Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_PentaKill, new RefAction<DefaultGameEventParam>(this.OnActorPentaKill));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Record_Save_Moment_Video, new CUIEventManager.OnUIEventHandler(this.OnSaveMomentVideo));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Record_Save_Moment_Video_Cancel, new CUIEventManager.OnUIEventHandler(this.OnSaveMomentVideoCancel));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<bool>(EventID.GAMEJOY_STARTRECORDING_RESULT, new Action<bool>(this.OnGameJoyStartRecordResult));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.OB_Video_Btn_VideoMgr_Click, new CUIEventManager.OnUIEventHandler(this.OnBtnVideoMgrClick));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<bool>(EventID.GAMEJOY_SDK_PERMISSION_CHECK_RESULT, new Action<bool>(this.OnGameJoyCheckPermissionResult));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<GameJoy.SDKFeature>(EventID.GAMEJOY_SDK_FEATURE_CHECK_RESULT, new Action<GameJoy.SDKFeature>(this.OnGameJoyCheckAvailabilityResult));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Record_Check_WhiteList_TimeUp, new CUIEventManager.OnUIEventHandler(this.OnCheckWhiteListTimeUp));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<long>(EventID.GAMEJOY_STOPRECORDING_RESULT, new Action<long>(this.OnGameJoyStopRecordResult));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Lobby_OpenLobbyForm, new CUIEventManager.OnUIEventHandler(this.OnLobbyFormOpen));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Loading_LoadingFromOpen, new CUIEventManager.OnUIEventHandler(this.OnLoadingFormOpen));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Loading_LoadingFromClose, new CUIEventManager.OnUIEventHandler(this.OnLoadingFormClose));
		}

		private void OnFightPrepare(ref DefaultGameEventParam prm)
		{
			SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
			if (curLvelContext == null)
			{
				this.m_bIsRecordMomentsEnable = false;
				return;
			}
			this.m_bIsRecordMomentsEnable = (GameSettings.EnableKingTimeMode && this.GetRecorderGlobalCfgEnableFlag() && !Singleton<WatchController>.GetInstance().IsWatching && curLvelContext.IsMobaModeWithOutGuide());
			if (this.m_bIsRecordMomentsEnable)
			{
				if (Singleton<LobbyLogic>.instance.reconnGameInfo != null)
				{
					this.m_bIsRecordMomentsEnable = false;
					Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("RecordMomentSuspendRecord"), false, 1.5f, null, new object[0]);
					return;
				}
				this.Reset();
				this.m_hostActor = Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain;
				Singleton<GameJoy>.instance.StartMomentsRecording();
			}
		}

		private void AddRecordEvent(CRecordUseSDK.RECORD_EVENT_PRIORITY eventPriority, long lStartTime, long lEndTime)
		{
			if (this.m_stRecordInfo == null)
			{
				return;
			}
			SortedList<long, long> sortedList = null;
			if (!this.m_stRecordInfo.TryGetValue(eventPriority, out sortedList))
			{
				sortedList = new SortedList<long, long>();
				this.m_stRecordInfo.Add(eventPriority, sortedList);
			}
			if (sortedList != null && !sortedList.ContainsKey(lStartTime))
			{
				sortedList.Add(lStartTime, lEndTime);
			}
			this.m_enLastEventPriority = CRecordUseSDK.RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_INVALID;
		}

		private void UpdateRecordEvent(PoolObjHandle<ActorRoot> eventActor, CRecordUseSDK.RECORD_EVENT_PRIORITY eventPriority)
		{
			if (!this.m_bIsRecordMomentsEnable || !this.m_bIsStartRecordOk)
			{
				return;
			}
			if (eventPriority != CRecordUseSDK.RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_INVALID && Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain != eventActor)
			{
				return;
			}
			long num = GameJoy.getSystemCurrentTimeMillis - this.m_lGameStartTime;
			bool flag = false;
			if (eventActor && eventActor.handle.ActorControl != null)
			{
				HeroWrapper heroWrapper = eventActor.handle.ActorControl as HeroWrapper;
				if (heroWrapper != null)
				{
					flag = heroWrapper.IsInMultiKill();
				}
			}
			if (!flag || eventPriority == CRecordUseSDK.RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_INVALID)
			{
				if (this.m_enLastEventPriority > CRecordUseSDK.RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_INVALID)
				{
					this.AddRecordEvent(this.m_enLastEventPriority, this.m_lLastEventStartTime, this.m_lLastEventEndTime);
				}
				this.m_enLastEventPriority = eventPriority;
				this.m_lLastEventStartTime = num;
				this.m_lLastEventEndTime = num;
			}
			else
			{
				if (this.m_enLastEventPriority != CRecordUseSDK.RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_INVALID && eventPriority <= this.m_enLastEventPriority)
				{
					return;
				}
				if (this.m_enLastEventPriority <= CRecordUseSDK.RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_ASSIST)
				{
					this.m_lLastEventStartTime = num;
				}
				this.m_enLastEventPriority = eventPriority;
				this.m_lLastEventEndTime = num;
			}
		}

		private void OnActorDead(ref GameDeadEventParam prm)
		{
			if (!this.m_bIsRecordMomentsEnable || !this.m_bIsStartRecordOk)
			{
				return;
			}
			if (!prm.src || prm.src.handle.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Hero)
			{
				return;
			}
			this.UpdateRecordEvent(prm.atker, CRecordUseSDK.RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_ONCEKILL);
			if (Singleton<GamePlayerCenter>.instance != null && Singleton<GamePlayerCenter>.instance.GetHostPlayer() != null && prm.atker != Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain)
			{
				if (prm.src && prm.src.handle.ActorControl != null)
				{
					List<KeyValuePair<uint, ulong>>.Enumerator enumerator = prm.src.handle.ActorControl.hurtSelfActorList.GetEnumerator();
					while (enumerator.MoveNext())
					{
						KeyValuePair<uint, ulong> current = enumerator.Current;
						if (current.Key == Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain.handle.ObjID)
						{
							this.UpdateRecordEvent(Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain, CRecordUseSDK.RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_ASSIST);
							return;
						}
					}
				}
				if (prm.atker && prm.atker.handle.ActorControl != null)
				{
					List<KeyValuePair<uint, ulong>>.Enumerator enumerator2 = prm.atker.handle.ActorControl.helpSelfActorList.GetEnumerator();
					while (enumerator2.MoveNext())
					{
						KeyValuePair<uint, ulong> current2 = enumerator2.Current;
						if (current2.Key == Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain.handle.ObjID)
						{
							this.UpdateRecordEvent(Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain, CRecordUseSDK.RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_ASSIST);
							return;
						}
					}
				}
			}
			else if (Singleton<GamePlayerCenter>.instance != null && Singleton<GamePlayerCenter>.instance.GetHostPlayer() != null && prm.atker == Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain)
			{
				HeroWrapper heroWrapper = prm.orignalAtker.handle.ActorControl as HeroWrapper;
				if (heroWrapper != null && heroWrapper.ContiKillNum > this.m_iContinuKillMaxNum)
				{
					this.m_iContinuKillMaxNum = heroWrapper.ContiKillNum;
				}
			}
		}

		private void OnActorDoubleKill(ref DefaultGameEventParam prm)
		{
			this.UpdateRecordEvent(prm.atker, CRecordUseSDK.RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_DOUBLEKILL);
		}

		private void OnActorTripleKill(ref DefaultGameEventParam prm)
		{
			this.UpdateRecordEvent(prm.atker, CRecordUseSDK.RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_TRIPLEKILL);
		}

		private void OnActorQuataryKill(ref DefaultGameEventParam prm)
		{
			this.UpdateRecordEvent(prm.atker, CRecordUseSDK.RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_QUATARYKILL);
		}

		private void OnActorPentaKill(ref DefaultGameEventParam prm)
		{
			this.UpdateRecordEvent(prm.atker, CRecordUseSDK.RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_PENTAKILL);
		}

		public void DoFightOver()
		{
			if (!this.m_bIsRecordMomentsEnable || !this.m_bIsStartRecordOk)
			{
				return;
			}
			this.UpdateRecordEvent(default(PoolObjHandle<ActorRoot>), CRecordUseSDK.RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_INVALID);
			this.m_lGameEndTime = GameJoy.getSystemCurrentTimeMillis - this.m_lGameStartTime;
			this.StopMomentsRecording();
			this.SetExtraInfos();
		}

		private int GetAssistCountWithTime(float fStartTime, float fEndTime)
		{
			int num = 0;
			SortedList<long, long> sortedList = null;
			if (this.m_stRecordInfo != null && this.m_stRecordInfo.TryGetValue(CRecordUseSDK.RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_ASSIST, out sortedList) && sortedList != null)
			{
				IEnumerator<KeyValuePair<long, long>> enumerator = sortedList.GetEnumerator();
				while (enumerator.MoveNext())
				{
					KeyValuePair<long, long> current = enumerator.Current;
					if (fStartTime <= (float)current.Key)
					{
						KeyValuePair<long, long> current2 = enumerator.Current;
						if ((float)current2.Value <= fEndTime)
						{
							num++;
							continue;
						}
					}
					KeyValuePair<long, long> current3 = enumerator.Current;
					if ((float)current3.Value > fEndTime)
					{
						break;
					}
				}
			}
			return num;
		}

		private void InsertAssistInfo(ref SortedList<int, SortedList<long, long>> assistInfo, int iAssistCount, long lStartTime, long lEndTime)
		{
			if (assistInfo == null)
			{
				assistInfo = new SortedList<int, SortedList<long, long>>();
			}
			SortedList<long, long> sortedList = null;
			if (!assistInfo.TryGetValue(iAssistCount, out sortedList))
			{
				sortedList = new SortedList<long, long>();
				assistInfo.Add(iAssistCount, sortedList);
			}
			if (sortedList != null && !sortedList.ContainsKey(lStartTime))
			{
				sortedList.Add(lStartTime, lEndTime);
			}
		}

		private void ChooseTopEvent()
		{
			int num = 0;
			int num2 = 0;
			bool flag = false;
			SortedList<long, CRecordUseSDK.RECORD_INFO> sortedList = new SortedList<long, CRecordUseSDK.RECORD_INFO>();
			SortedList<long, long> sortedList2 = null;
			for (CRecordUseSDK.RECORD_EVENT_PRIORITY rECORD_EVENT_PRIORITY = CRecordUseSDK.RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_PENTAKILL; rECORD_EVENT_PRIORITY > CRecordUseSDK.RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_DOUBLEKILL; rECORD_EVENT_PRIORITY--)
			{
				if (this.m_stRecordInfo != null && this.m_stRecordInfo.TryGetValue(rECORD_EVENT_PRIORITY, out sortedList2) && sortedList2 != null)
				{
					IEnumerator<KeyValuePair<long, long>> enumerator = sortedList2.GetEnumerator();
					while (enumerator.MoveNext())
					{
						num2++;
						if ((long)num2 > (long)((ulong)this.m_uiEventNumMax))
						{
							flag = true;
							break;
						}
						KeyValuePair<long, long> current = enumerator.Current;
						long arg_9F_0;
						if (current.Key < (long)((ulong)this.m_uiEventStartTimeInterval))
						{
							arg_9F_0 = 0L;
						}
						else
						{
							KeyValuePair<long, long> current2 = enumerator.Current;
							arg_9F_0 = current2.Key - (long)((ulong)this.m_uiEventStartTimeInterval);
						}
						long num3 = arg_9F_0;
						KeyValuePair<long, long> current3 = enumerator.Current;
						long num4 = current3.Value + (long)((ulong)this.m_uiEventEndTimeInterval);
						num4 = ((num4 <= this.m_lGameEndTime) ? num4 : this.m_lGameEndTime);
						num += (int)(num4 - num3);
						if ((long)num > (long)((ulong)this.m_ui543KillEventTotalTime))
						{
							flag = true;
							break;
						}
						if (!sortedList.ContainsKey(num3))
						{
							sortedList.Add(num3, new CRecordUseSDK.RECORD_INFO(rECORD_EVENT_PRIORITY, num4));
						}
					}
				}
				if (flag)
				{
					break;
				}
			}
			if (!flag && (long)num < (long)((ulong)this.m_ui210KillEventTotalTime))
			{
				bool flag2 = false;
				SortedList<int, SortedList<long, long>> sortedList3 = null;
				SortedList<long, long> sortedList4 = null;
				if (this.m_stRecordInfo != null && this.m_stRecordInfo.TryGetValue(CRecordUseSDK.RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_ASSIST, out sortedList4))
				{
					flag2 = true;
				}
				for (CRecordUseSDK.RECORD_EVENT_PRIORITY rECORD_EVENT_PRIORITY2 = CRecordUseSDK.RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_DOUBLEKILL; rECORD_EVENT_PRIORITY2 > CRecordUseSDK.RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_ASSIST; rECORD_EVENT_PRIORITY2--)
				{
					if (this.m_stRecordInfo != null && this.m_stRecordInfo.TryGetValue(rECORD_EVENT_PRIORITY2, out sortedList2) && sortedList2 != null)
					{
						IEnumerator<KeyValuePair<long, long>> enumerator2 = sortedList2.GetEnumerator();
						while (enumerator2.MoveNext())
						{
							KeyValuePair<long, long> current4 = enumerator2.Current;
							long arg_1ED_0;
							if (current4.Key < (long)((ulong)this.m_uiEventStartTimeInterval))
							{
								arg_1ED_0 = 0L;
							}
							else
							{
								KeyValuePair<long, long> current5 = enumerator2.Current;
								arg_1ED_0 = current5.Key - (long)((ulong)this.m_uiEventStartTimeInterval);
							}
							long num5 = arg_1ED_0;
							KeyValuePair<long, long> current6 = enumerator2.Current;
							long num6 = current6.Value + (long)((ulong)this.m_uiEventEndTimeInterval) - (long)((ulong)this.m_uiOnceDoubleEventTimeIntervalReduce);
							num6 = ((num6 <= this.m_lGameEndTime) ? num6 : this.m_lGameEndTime);
							if (!flag2)
							{
								num2++;
								if ((long)num2 > (long)((ulong)this.m_uiEventNumMax))
								{
									flag = true;
									break;
								}
								num += (int)(num6 - num5);
								if ((long)num > (long)((ulong)this.m_ui210KillEventTotalTime))
								{
									flag = true;
									break;
								}
								if (!sortedList.ContainsKey(num5))
								{
									sortedList.Add(num5, new CRecordUseSDK.RECORD_INFO(rECORD_EVENT_PRIORITY2, num6));
								}
							}
							else
							{
								int assistCountWithTime = this.GetAssistCountWithTime((float)num5, (float)num6);
								this.InsertAssistInfo(ref sortedList3, assistCountWithTime, num5, num6);
							}
						}
						if (flag2 && sortedList3 != null && sortedList3.Count > 0)
						{
							int count = sortedList3.Count;
							for (int i = count - 1; i >= 0; i--)
							{
								sortedList2 = sortedList3.Values[i];
								if (sortedList2 != null)
								{
									IEnumerator<KeyValuePair<long, long>> enumerator3 = sortedList2.GetEnumerator();
									while (enumerator3.MoveNext())
									{
										KeyValuePair<long, long> current7 = enumerator3.Current;
										long key = current7.Key;
										KeyValuePair<long, long> current8 = enumerator3.Current;
										long value = current8.Value;
										num2++;
										if ((long)num2 > (long)((ulong)this.m_uiEventNumMax))
										{
											flag = true;
											break;
										}
										num += (int)(value - key);
										if ((long)num > (long)((ulong)this.m_ui210KillEventTotalTime))
										{
											flag = true;
											break;
										}
										if (!sortedList.ContainsKey(key))
										{
											sortedList.Add(key, new CRecordUseSDK.RECORD_INFO(rECORD_EVENT_PRIORITY2, value));
										}
									}
								}
							}
							sortedList3.Clear();
						}
					}
					if (flag)
					{
						break;
					}
				}
				if (!flag && this.m_stRecordInfo != null && this.m_stRecordInfo.TryGetValue(CRecordUseSDK.RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_ASSIST, out sortedList2) && sortedList2 != null)
				{
					IEnumerator<KeyValuePair<long, long>> enumerator4 = sortedList2.GetEnumerator();
					while (enumerator4.MoveNext())
					{
						KeyValuePair<long, long> current9 = enumerator4.Current;
						long arg_431_0;
						if (current9.Key < (long)((ulong)this.m_uiEventStartTimeInterval))
						{
							arg_431_0 = 0L;
						}
						else
						{
							KeyValuePair<long, long> current10 = enumerator4.Current;
							arg_431_0 = current10.Key - (long)((ulong)this.m_uiEventStartTimeInterval);
						}
						long num7 = arg_431_0;
						KeyValuePair<long, long> current11 = enumerator4.Current;
						long num8 = current11.Value + (long)((ulong)this.m_uiEventEndTimeInterval) - (long)((ulong)this.m_uiOnceDoubleEventTimeIntervalReduce);
						num8 = ((num8 <= this.m_lGameEndTime) ? num8 : this.m_lGameEndTime);
						num2++;
						if ((long)num2 > (long)((ulong)this.m_uiEventNumMax))
						{
							break;
						}
						num += (int)(num8 - num7);
						if ((long)num > (long)((ulong)this.m_ui210KillEventTotalTime))
						{
							break;
						}
						if (!sortedList.ContainsKey(num7))
						{
							sortedList.Add(num7, new CRecordUseSDK.RECORD_INFO(CRecordUseSDK.RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_ASSIST, num8));
						}
					}
				}
			}
			List<TimeStamp> list = new List<TimeStamp>();
			long num9 = 0L;
			long num10 = 0L;
			IEnumerator<KeyValuePair<long, CRecordUseSDK.RECORD_INFO>> enumerator5 = sortedList.GetEnumerator();
			while (enumerator5.MoveNext())
			{
				KeyValuePair<long, CRecordUseSDK.RECORD_INFO> current12 = enumerator5.Current;
				long num11 = current12.Key;
				KeyValuePair<long, CRecordUseSDK.RECORD_INFO> current13 = enumerator5.Current;
				long lEndTime = current13.Value.lEndTime;
				if (list.Count > 0 && num9 > num11)
				{
					list.RemoveAt(list.Count - 1);
					num -= (int)(num9 - num10);
					num9 = (num9 + num11) / 2L;
					num11 = num9;
					num += (int)(num9 - num10);
					list.Add(new TimeStamp(num10, num9));
				}
				num10 = num11;
				num9 = lEndTime;
				list.Add(new TimeStamp(num11, lEndTime));
			}
			this.m_bIsCallGameJoyGenerate = true;
			Singleton<GameJoy>.instance.GenerateMomentsVideo(list, this.GetVideoName(), this.m_extraInfos);
		}

		public bool OpenRecorderCheck(GameObject KingBar, CRecordUseSDK.CHECK_FROM_TYPE fromType)
		{
			this.m_objKingBar = KingBar;
			this.m_enmCheckFromType = fromType;
			this.m_enmCheckWhiteListStatus = CRecordUseSDK.CHECK_WHITELIST_STATUS.CHECK_WHITELIST_STATUS_TYPE_INVALID;
			Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(null, 3, enUIEventID.Record_Check_WhiteList_TimeUp);
			this.CheckWhiteList();
			return false;
		}

		private void CheckPermission()
		{
			GameJoy.CheckSDKPermission();
		}

		private void CheckWhiteList()
		{
			GameJoy.CheckSDKFeature();
		}

		private bool CheckStorage()
		{
			bool result = true;
			long num = 0L;
			using (AndroidJavaObject androidJavaObject = new AndroidJavaObject("android.os.StatFs", new object[]
			{
				Application.persistentDataPath
			}))
			{
				num = (long)androidJavaObject.Call<int>("getBlockSize", new object[0]) * (long)androidJavaObject.Call<int>("getAvailableBlocks", new object[0]) / 1024L / 1024L;
			}
			uint num2 = this.m_uiMinSpaceLimit;
			if (this.m_enmCheckFromType == CRecordUseSDK.CHECK_FROM_TYPE.CHECK_FROM_TYPE_RECORDER)
			{
				num2 = this.m_uiFreeRecorderMinSpaceLimit;
			}
			if (num < (long)((ulong)num2))
			{
				this.SetKingBarSliderState(false);
				Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("ReplayKit_Disk_Space_Limit"), false, 1.5f, null, new object[0]);
				result = false;
			}
			return result;
		}

		public void OpenMsgBoxForMomentRecorder(Transform container)
		{
			if (!this.m_bIsRecordMomentsEnable)
			{
				return;
			}
			if (container == null)
			{
				return;
			}
			if (this.m_stRecordInfo == null || this.m_stRecordInfo.Count == 0)
			{
				return;
			}
			if (this.m_lVideoTimeLen <= 0L)
			{
				Singleton<CUIManager>.instance.OpenTips("RecordMoment_EndGame_Tips_NoRecorderExist", true, 1.5f, null, new object[0]);
				return;
			}
			this.m_RecorderPanel = container;
			if (GameSettings.EnableKingTimeMode && this.m_bIsStartRecordOk)
			{
				Transform transform = container.FindChild("Extra/Image/Image/Text");
				if (transform && transform.gameObject)
				{
					Text component = transform.gameObject.GetComponent<Text>();
					if (component)
					{
						component.text = Singleton<CTextManager>.GetInstance().GetText("RecordSaveMomentVideo");
					}
				}
				container.gameObject.CustomSetActive(true);
			}
		}

		private void CloseRecorderPanel()
		{
			if (this.m_RecorderPanel != null)
			{
				Transform transform = this.m_RecorderPanel.FindChild("Extra");
				if (transform)
				{
					transform.gameObject.CustomSetActive(false);
				}
			}
		}

		private void OnSaveMomentVideo(CUIEvent uiEvent)
		{
			Vector3 recorderPosition = this.GetRecorderPosition();
			Singleton<GameJoy>.instance.SetDefaultUploadShareDialogPosition(recorderPosition.x, recorderPosition.y);
			this.CloseRecorderPanel();
			this.ChooseTopEvent();
		}

		private void OnSaveMomentVideoCancel(CUIEvent uiEvent)
		{
			this.CloseRecorderPanel();
			this.CallGameJoyGenerateWithNothing();
		}

		public void CallGameJoyGenerateWithNothing()
		{
			if (this.m_bIsRecordMomentsEnable && this.m_bIsStartRecordOk && !this.m_bIsCallGameJoyGenerate)
			{
				this.m_bIsCallGameJoyGenerate = true;
				Singleton<GameJoy>.instance.GenerateMomentsVideo(null, null, null);
			}
		}

		public void StopMomentsRecording()
		{
			if (this.m_bIsRecordMomentsEnable && this.m_bIsStartRecordOk && !this.m_bIsCallStopGameJoyRecord)
			{
				this.m_bIsCallStopGameJoyRecord = true;
				Singleton<GameJoy>.instance.EndMomentsRecording();
			}
		}

		private int ConvertMaxMultiKillPriorityToResDef()
		{
			int result = -1;
			if (this.m_stRecordInfo.Count > 0)
			{
				if (this.m_stRecordInfo.ContainsKey(CRecordUseSDK.RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_PENTAKILL))
				{
					result = 6;
				}
				else if (this.m_stRecordInfo.ContainsKey(CRecordUseSDK.RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_QUATARYKILL))
				{
					result = 5;
				}
				else if (this.m_stRecordInfo.ContainsKey(CRecordUseSDK.RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_TRIPLEKILL))
				{
					result = 4;
				}
				else if (this.m_stRecordInfo.ContainsKey(CRecordUseSDK.RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_DOUBLEKILL))
				{
					result = 3;
				}
				else if (this.m_stRecordInfo.ContainsKey(CRecordUseSDK.RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_ONCEKILL))
				{
					result = 2;
				}
				else if (this.m_stRecordInfo.ContainsKey(CRecordUseSDK.RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_ASSIST))
				{
					result = 0;
				}
			}
			return result;
		}

		private void SetExtraInfos()
		{
			this.m_extraInfos.Clear();
			if (Singleton<GamePlayerCenter>.instance.HostPlayerId != 0u)
			{
				Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
				if (hostPlayer != null)
				{
					this.m_strHostPlayerName = hostPlayer.Name;
					this.m_extraInfos.Add("PN", hostPlayer.Name);
					this.m_extraInfos.Add("VL", hostPlayer.VipLv.ToString());
					this.m_extraInfos.Add("PL", hostPlayer.Level.ToString());
				}
			}
			this.m_extraInfos.Add("WID", MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString());
			this.m_extraInfos.Add("PF", Singleton<ApolloHelper>.GetInstance().CurPlatform.ToString());
			if (this.m_hostActor)
			{
				this.m_strHostHeroName = this.m_hostActor.handle.name;
				if (!string.IsNullOrEmpty(this.m_strHostHeroName))
				{
					int num = this.m_strHostHeroName.IndexOf('(');
					string value = this.m_strHostHeroName.Substring(num + 1, this.m_strHostHeroName.Length - num - 2);
					this.m_extraInfos.Add("HN", value);
				}
				uint mvpPlayer = Singleton<BattleStatistic>.instance.GetMvpPlayer(this.m_hostActor.handle.TheActorMeta.ActorCamp, true);
				if (mvpPlayer != 0u && mvpPlayer == this.m_hostActor.handle.TheActorMeta.PlayerId)
				{
					this.m_bIsMvp = true;
				}
				this.m_extraInfos.Add("MVP", ((!this.m_bIsMvp) ? 0 : 1).ToString());
			}
			if (Singleton<BattleLogic>.GetInstance().battleStat != null && Singleton<BattleLogic>.GetInstance().battleStat.m_playerKDAStat != null)
			{
				PlayerKDA hostKDA = Singleton<BattleLogic>.GetInstance().battleStat.m_playerKDAStat.GetHostKDA();
				if (hostKDA != null)
				{
					this.m_extraInfos.Add("HV", hostKDA.TotalHurt.ToString());
					this.m_extraInfos.Add("BHV", hostKDA.TotalBeHurt.ToString());
					this.m_extraInfos.Add("KN", hostKDA.numKill.ToString());
					this.m_extraInfos.Add("DN", hostKDA.numDead.ToString());
					this.m_extraInfos.Add("AN", hostKDA.numAssist.ToString());
					this.m_extraInfos.Add("CS", hostKDA.LegendaryNum.ToString());
				}
			}
			if (this.m_hostActor.handle.EquipComponent != null)
			{
				stEquipInfo[] equips = this.m_hostActor.handle.EquipComponent.GetEquips();
				if (equips != null)
				{
					for (int i = 0; i < 6; i++)
					{
						if (equips[i].m_equipID != 0)
						{
							ResEquipInBattle dataByKey = GameDataMgr.m_equipInBattleDatabin.GetDataByKey((uint)equips[i].m_equipID);
							if (dataByKey != null)
							{
								this.m_extraInfos.Add("EN" + i.ToString(), equips[i].m_equipID.ToString() + dataByKey.szName);
							}
							else
							{
								this.m_extraInfos.Add("EN" + i.ToString(), equips[i].m_equipID.ToString());
							}
						}
						else
						{
							this.m_extraInfos.Add("EN" + i.ToString(), "0");
						}
					}
				}
			}
			if (this.m_hostActor.handle.SkillControl != null)
			{
				for (SkillSlotType skillSlotType = SkillSlotType.SLOT_SKILL_1; skillSlotType <= SkillSlotType.SLOT_SKILL_2; skillSlotType++)
				{
					SkillSlot skillSlot;
					if (this.m_hostActor.handle.SkillControl.TryGetSkillSlot(skillSlotType, out skillSlot) && skillSlot.SkillObj != null && skillSlot.SkillObj.cfgData != null)
					{
						this.m_extraInfos.Add("SN" + (int)skillSlotType, skillSlot.SkillObj.SkillID + skillSlot.SkillObj.cfgData.szSkillName);
					}
				}
			}
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			if (curLvelContext != null)
			{
				this.m_extraInfos.Add("MN", ((int)curLvelContext.GetGameType()).ToString());
				if (curLvelContext.IsMultilModeWithoutWarmBattle())
				{
					this.m_extraInfos.Add("GID", this.m_strGameId);
				}
			}
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo != null)
			{
				ResRankGradeConf gradeDataByShowGrade = CLadderSystem.GetGradeDataByShowGrade((int)masterRoleInfo.m_rankGrade);
				if (gradeDataByShowGrade != null)
				{
					this.m_extraInfos.Add("GN", gradeDataByShowGrade.szGradeDesc);
				}
			}
			if (this.m_stRecordInfo != null && this.m_stRecordInfo.Count > 0)
			{
				byte b = 0;
				if (this.m_stRecordInfo.ContainsKey(CRecordUseSDK.RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_TRIPLEKILL))
				{
					b = 1;
				}
				this.m_extraInfos.Add("3k", b.ToString());
				b = 0;
				if (this.m_stRecordInfo.ContainsKey(CRecordUseSDK.RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_QUATARYKILL))
				{
					b = 1;
				}
				this.m_extraInfos.Add("4k", b.ToString());
				b = 0;
				if (this.m_stRecordInfo.ContainsKey(CRecordUseSDK.RECORD_EVENT_PRIORITY.RECORD_EVENT_TYPE_PENTAKILL))
				{
					b = 1;
				}
				this.m_extraInfos.Add("5k", b.ToString());
			}
			if (Singleton<BattleLogic>.GetInstance().battleStat != null)
			{
				this.m_extraInfos.Add("WL", Singleton<BattleLogic>.GetInstance().battleStat.iBattleResult.ToString());
			}
		}

		private string GetVideoName()
		{
			string str = Singleton<CTextManager>.GetInstance().GetText("RecordMomentVideoNameHeader");
			str += this.m_strHostPlayerName;
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			if (curLvelContext != null)
			{
				str += curLvelContext.m_levelName;
				if (curLvelContext.IsGameTypeLadder())
				{
					str += curLvelContext.m_gameMatchName;
				}
			}
			if (this.m_bIsMvp)
			{
				str += "MVP";
			}
			if (!string.IsNullOrEmpty(this.m_strHostHeroName))
			{
				int num = this.m_strHostHeroName.IndexOf('(');
				string str2 = this.m_strHostHeroName.Substring(num + 1, this.m_strHostHeroName.Length - num - 2);
				str += str2;
			}
			int num2 = this.ConvertMaxMultiKillPriorityToResDef();
			if (num2 > 2)
			{
				ResMultiKill dataByKey = GameDataMgr.multiKillDatabin.GetDataByKey((long)num2);
				if (dataByKey != null)
				{
					str += dataByKey.szAchievementName;
				}
			}
			return str + Singleton<CTextManager>.GetInstance().GetText("RecordMomentVideoNameTail");
		}

		private Vector3 GetRecorderPosition()
		{
			Vector3 result = Vector3.zero;
			if (this.m_RecorderPanel)
			{
				Transform transform = this.m_RecorderPanel.FindChild("Record");
				if (transform)
				{
					Camera camera = Camera.current;
					if (camera == null)
					{
						camera = Camera.allCameras[0];
					}
					if (camera)
					{
						result = camera.WorldToViewportPoint(transform.transform.position);
					}
				}
			}
			return result;
		}

		private void OnGameJoyStartRecordResult(bool bRes)
		{
			this.m_bIsStartRecordOk = bRes;
			if (bRes)
			{
				this.m_lGameStartTime = GameJoy.getSystemCurrentTimeMillis;
			}
		}

		private void OnGameJoyStopRecordResult(long lDuration)
		{
			this.m_lVideoTimeLen = lDuration;
		}

		private void OnBtnVideoMgrClick(CUIEvent cuiEvent)
		{
			this.m_enmCheckWhiteListStatus = CRecordUseSDK.CHECK_WHITELIST_STATUS.CHECK_WHITELIST_STATUS_TYPE_VIDEOMGRCLICK;
			Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(null, 3, enUIEventID.Record_Check_WhiteList_TimeUp);
			this.CheckWhiteList();
		}

		public bool GetRecorderGlobalCfgEnableFlag()
		{
			bool result = false;
			if (GameDataMgr.svr2CltCfgDict != null && GameDataMgr.svr2CltCfgDict.ContainsKey(12u))
			{
				ResGlobalInfo resGlobalInfo = new ResGlobalInfo();
				if (GameDataMgr.svr2CltCfgDict.TryGetValue(12u, out resGlobalInfo))
				{
					result = (resGlobalInfo.dwConfValue > 0u);
				}
			}
			return result;
		}

		private void OnGameJoyCheckPermissionResult(bool bRes)
		{
			this.m_enmCheckPermissionRes = ((!bRes) ? CRecordUseSDK.CHECK_PERMISSION_STUTAS.CHECK_PERMISSION_STUTAS_TYPE_NOPERMISSION : CRecordUseSDK.CHECK_PERMISSION_STUTAS.CHECK_PERMISSION_STUTAS_TYPE_PERMISSIONOK);
			if (this.m_objKingBar != null && !bRes)
			{
				this.SetKingBarSliderState(false);
				Singleton<CUIManager>.instance.OpenTips("GameJoyCheckPermissionFailed", true, 1.5f, null, new object[0]);
			}
			if (bRes && this.m_enmCheckFromType == CRecordUseSDK.CHECK_FROM_TYPE.CHECK_FROM_TYPE_RECORDER)
			{
				this.SetRecorderState(true, true);
			}
		}

		private bool CheckStorageAndPermission()
		{
			if (!this.CheckStorage())
			{
				return false;
			}
			this.CheckPermission();
			return true;
		}

		private void OnGameJoyCheckAvailabilityResult(GameJoy.SDKFeature enmFeature)
		{
			if (this.m_enmCheckWhiteListStatus != CRecordUseSDK.CHECK_WHITELIST_STATUS.CHECK_WHITELIST_STATUS_TYPE_TIMEUP && this.m_enmCheckWhiteListStatus != CRecordUseSDK.CHECK_WHITELIST_STATUS.CHECK_WHITELIST_STATUS_TYPE_AUTOCHECK)
			{
				if (this.m_enmCheckWhiteListStatus != CRecordUseSDK.CHECK_WHITELIST_STATUS.CHECK_WHITELIST_STATUS_TYPE_LOBBYRECORDER)
				{
					Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
				}
				if ((this.m_enmCheckFromType == CRecordUseSDK.CHECK_FROM_TYPE.CHECK_FROM_TYPE_KINGTIME && (enmFeature & GameJoy.SDKFeature.Moment) == (GameJoy.SDKFeature)0) || (this.m_enmCheckFromType == CRecordUseSDK.CHECK_FROM_TYPE.CHECK_FROM_TYPE_RECORDER && (enmFeature & GameJoy.SDKFeature.Manual) == (GameJoy.SDKFeature)0))
				{
					Singleton<CUIManager>.instance.OpenTips("GamejoyCheckAvailabilityFailed", true, 1.5f, null, new object[0]);
					this.SetKingBarSliderState(false);
					if ((enmFeature & GameJoy.SDKFeature.Moment) == (GameJoy.SDKFeature)0)
					{
						GameSettings.EnableKingTimeMode = false;
					}
					if ((enmFeature & GameJoy.SDKFeature.Moment) == (GameJoy.SDKFeature)0)
					{
						GameSettings.EnableRecorderMode = false;
					}
				}
				else
				{
					this.CheckStorageAndPermission();
				}
				if (this.m_enmCheckWhiteListStatus == CRecordUseSDK.CHECK_WHITELIST_STATUS.CHECK_WHITELIST_STATUS_TYPE_VIDEOMGRCLICK)
				{
					if ((enmFeature & GameJoy.SDKFeature.Moment) != (GameJoy.SDKFeature)0 || (enmFeature & GameJoy.SDKFeature.Manual) != (GameJoy.SDKFeature)0)
					{
						Singleton<GameJoy>.instance.ShowVideoListDialog();
					}
					this.m_enmCheckWhiteListStatus = CRecordUseSDK.CHECK_WHITELIST_STATUS.CHECK_WHITELIST_STATUS_TYPE_INVALID;
					return;
				}
			}
			else if (this.m_enmCheckWhiteListStatus == CRecordUseSDK.CHECK_WHITELIST_STATUS.CHECK_WHITELIST_STATUS_TYPE_AUTOCHECK)
			{
				if ((enmFeature & GameJoy.SDKFeature.Moment) == (GameJoy.SDKFeature)0)
				{
					GameSettings.EnableKingTimeMode = false;
				}
				if ((enmFeature & GameJoy.SDKFeature.Moment) == (GameJoy.SDKFeature)0)
				{
					GameSettings.EnableRecorderMode = false;
				}
			}
		}

		private void SetGameSettingVariableValue(bool bIsOpen)
		{
			if (this.m_enmCheckFromType == CRecordUseSDK.CHECK_FROM_TYPE.CHECK_FROM_TYPE_KINGTIME)
			{
				if (GameSettings.EnableKingTimeMode != bIsOpen)
				{
					GameSettings.EnableKingTimeMode = bIsOpen;
				}
			}
			else if (this.m_enmCheckFromType == CRecordUseSDK.CHECK_FROM_TYPE.CHECK_FROM_TYPE_RECORDER && GameSettings.EnableRecorderMode != bIsOpen)
			{
				GameSettings.EnableRecorderMode = bIsOpen;
			}
		}

		public void SetKingBarSliderState(bool bIsOpen)
		{
			if (this.m_objKingBar != null)
			{
				Transform transform = this.m_objKingBar.transform.FindChild("Slider");
				if (transform)
				{
					CUISliderEventScript component = transform.GetComponent<CUISliderEventScript>();
					int num = (!bIsOpen) ? 0 : 1;
					if ((int)component.value != num)
					{
						component.value = (float)num;
					}
				}
				else
				{
					this.SetGameSettingVariableValue(bIsOpen);
				}
			}
			else
			{
				this.SetGameSettingVariableValue(bIsOpen);
			}
		}

		private void OnCheckWhiteListTimeUp(CUIEvent uiEvent)
		{
			if (this.m_enmCheckWhiteListStatus == CRecordUseSDK.CHECK_WHITELIST_STATUS.CHECK_WHITELIST_STATUS_TYPE_INVALID)
			{
				this.SetKingBarSliderState(false);
			}
			this.m_enmCheckWhiteListStatus = CRecordUseSDK.CHECK_WHITELIST_STATUS.CHECK_WHITELIST_STATUS_TYPE_TIMEUP;
		}

		public void OnBadGameEnd()
		{
			if (this.m_bIsStartRecordOk && this.m_bIsRecordMomentsEnable)
			{
				CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(SettlementSystem.SettlementFormName);
				if (form && form.gameObject.activeSelf)
				{
					return;
				}
				this.StopMomentsRecording();
				this.CallGameJoyGenerateWithNothing();
			}
		}

		public void SetRecorderState(bool bIsShow, bool bIsLobby = true)
		{
			if (!this.GetRecorderGlobalCfgEnableFlag() || !GameSettings.EnableRecorderMode || Singleton<WatchController>.GetInstance().IsWatching)
			{
				return;
			}
			if (bIsShow)
			{
				if (this.m_enmCheckWhiteListStatus == CRecordUseSDK.CHECK_WHITELIST_STATUS.CHECK_WHITELIST_STATUS_TYPE_RESULTFAILED || this.m_enmCheckPermissionRes == CRecordUseSDK.CHECK_PERMISSION_STUTAS.CHECK_PERMISSION_STUTAS_TYPE_NOPERMISSION || !this.CheckStorage())
				{
					GameSettings.EnableRecorderMode = false;
					return;
				}
				if (!this.m_bIsRecorderShowing)
				{
					this.m_bIsRecorderShowing = true;
					if (!this.m_bIsShowingFreeRecorder)
					{
						this.m_bIsShowingFreeRecorder = true;
						Singleton<GameJoy>.instance.StartRecorder();
					}
					if (!bIsLobby)
					{
						if (this.m_Vec2FreeRecordWindowPosInBattle != Vector2.zero)
						{
							Singleton<GameJoy>.instance.SetCurrentRecorderPosition(this.m_Vec2FreeRecordWindowPosInBattle.x, this.m_Vec2FreeRecordWindowPosInBattle.y);
						}
						else
						{
							Singleton<GameJoy>.instance.SetCurrentRecorderPosition(1f - 90f / (float)Screen.width, 1f - 132f / (float)Screen.height);
						}
					}
					else if (this.m_Vec2FreeRecordWindowPosInLobby != Vector2.zero)
					{
						Singleton<GameJoy>.instance.SetCurrentRecorderPosition(this.m_Vec2FreeRecordWindowPosInLobby.x, this.m_Vec2FreeRecordWindowPosInLobby.y);
					}
					else
					{
						Singleton<GameJoy>.instance.SetCurrentRecorderPosition(1f - 30f / (float)Screen.width, 1f - 61f / (float)Screen.height);
					}
				}
			}
			else if (this.m_bIsRecorderShowing)
			{
				this.m_bIsRecorderShowing = false;
				if (bIsLobby)
				{
					this.m_Vec2FreeRecordWindowPosInLobby = Singleton<GameJoy>.instance.currentRecorderPosition;
				}
				else
				{
					this.m_Vec2FreeRecordWindowPosInBattle = Singleton<GameJoy>.instance.currentRecorderPosition;
				}
				Singleton<GameJoy>.instance.SetCurrentRecorderPosition(-1f, -1f);
			}
		}

		public void StopFreeRecorder()
		{
			this.m_bIsRecorderShowing = false;
			this.m_bIsShowingFreeRecorder = false;
			Singleton<GameJoy>.instance.StopRecorder();
		}

		private void OnLobbyFormOpen(CUIEvent uiEvent)
		{
			if (this.GetRecorderGlobalCfgEnableFlag())
			{
				if (this.m_bIsFirstOpenLobby)
				{
					this.m_bIsFirstOpenLobby = false;
					if (GameSettings.EnableRecorderMode)
					{
						this.m_enmCheckWhiteListStatus = CRecordUseSDK.CHECK_WHITELIST_STATUS.CHECK_WHITELIST_STATUS_TYPE_LOBBYRECORDER;
						this.m_enmCheckFromType = CRecordUseSDK.CHECK_FROM_TYPE.CHECK_FROM_TYPE_RECORDER;
					}
					else
					{
						this.m_enmCheckWhiteListStatus = CRecordUseSDK.CHECK_WHITELIST_STATUS.CHECK_WHITELIST_STATUS_TYPE_AUTOCHECK;
					}
					this.CheckWhiteList();
				}
				else
				{
					this.SetRecorderState(true, true);
				}
			}
		}

		private void OnLoadingFormOpen(CUIEvent uiEvent)
		{
			this.SetRecorderState(false, true);
		}

		private void OnLoadingFormClose(CUIEvent uiEvent)
		{
			this.SetRecorderState(true, false);
		}

		public void OnGameSettingSetVideoHighQuality(bool bIsHigh)
		{
			if (!this.GetRecorderGlobalCfgEnableFlag())
			{
				return;
			}
			GameJoy.VideoQuality videoQuality = GameJoy.VideoQuality.Low;
			if (bIsHigh)
			{
				videoQuality = GameJoy.VideoQuality.High;
			}
			Singleton<GameJoy>.instance.SetVideoQuality(videoQuality);
		}

		public void SetGameId(uint deskID, uint seq, uint entity)
		{
			this.m_strGameId = string.Concat(new object[]
			{
				deskID,
				"|",
				seq,
				"|",
				entity
			});
		}
	}
}
