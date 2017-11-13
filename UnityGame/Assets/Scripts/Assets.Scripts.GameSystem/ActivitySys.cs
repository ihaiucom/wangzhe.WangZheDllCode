using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Assets.Scripts.GameSystem
{
	[MessageHandlerClass]
	public class ActivitySys : Singleton<ActivitySys>
	{
		private class RewardList
		{
			public ListView<CUseable> usabList;

			public int flags;

			public RewardList(ListView<CUseable> usabList, int flags)
			{
				this.usabList = usabList;
				this.flags = flags;
			}
		}

		public delegate void StateChangeDelegate();

		public const long GAME_OVER_TIME = 4099680000L;

		public const int CHECK_TIMER_CYCLE = 1000;

		public const string TODAY_LOGIN_SHOW_ACTIVITY_CNT = "TODAY_LOGIN_SHOW_ACTIVITY_CNT";

		public static string SpriteRootDir = "UGUI/Sprite/Dynamic/Activity/";

		private DictionaryView<uint, Activity> _actvDict;

		private int _checkTimer;

		private int _refreshTimer;

		private CampaignForm _campaignForm;

		private uint _openActivityKey;

		private bool m_bShareTask;

		public string[] m_ActivtyTabName = new string[3];

		private bool _firstLoad = true;

		private ListView<ActivitySys.RewardList> _rewardListQueue = new ListView<ActivitySys.RewardList>();

		private int _rewardQueueIndex = -1;

		private ActivitySys.RewardList _rewardShowList;

		private int _rewardShowIndex = -1;

		private bool _rewardHasSpecial;

		public event ActivitySys.StateChangeDelegate OnStateChange
		{
			[MethodImpl(32)]
			add
			{
				this.OnStateChange = (ActivitySys.StateChangeDelegate)Delegate.Combine(this.OnStateChange, value);
			}
			[MethodImpl(32)]
			remove
			{
				this.OnStateChange = (ActivitySys.StateChangeDelegate)Delegate.Remove(this.OnStateChange, value);
			}
		}

		public bool IsShareTask
		{
			get
			{
				return this.m_bShareTask;
			}
			set
			{
				this.m_bShareTask = value;
			}
		}

		public override void Init()
		{
			this.m_bShareTask = false;
			this._actvDict = null;
			this._checkTimer = 0;
			this._refreshTimer = 0;
			this._campaignForm = new CampaignForm(this);
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Activity_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnOpenCampaignForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Activity_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnCloseCampaignForm));
			Singleton<EventRouter>.instance.AddEventHandler("IDIPNOTICE_UNREAD_NUM_UPDATE", new Action(this.OnIDIPNoticeUpdate));
			Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.GLOBAL_REFRESH_TIME, new Action(ActivitySys.OnResetAllExchangeCount));
			for (int i = 0; i < 3; i++)
			{
				this.m_ActivtyTabName[i] = Singleton<CTextManager>.GetInstance().GetText("Activty_Tab_Index_" + i);
			}
		}

		public override void UnInit()
		{
			Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref this._checkTimer);
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Activity_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnOpenCampaignForm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Activity_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnCloseCampaignForm));
			Singleton<EventRouter>.instance.RemoveEventHandler("IDIPNOTICE_UNREAD_NUM_UPDATE", new Action(this.OnIDIPNoticeUpdate));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.GLOBAL_REFRESH_TIME, new Action(ActivitySys.OnResetAllExchangeCount));
		}

		public static bool NeedShowWhenLogin()
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return true;
			}
			string key = string.Format("{0}_{1}", "TODAY_LOGIN_SHOW_ACTIVITY_CNT", masterRoleInfo.playerUllUID);
			if (masterRoleInfo.bFirstLoginToday)
			{
				PlayerPrefs.SetInt(key, 0);
			}
			uint @int = (uint)PlayerPrefs.GetInt(key, 0);
			if (GameDataMgr.svr2CltCfgDict != null && GameDataMgr.svr2CltCfgDict.ContainsKey(0u))
			{
				uint num = 0u;
				ResGlobalInfo resGlobalInfo = new ResGlobalInfo();
				if (GameDataMgr.svr2CltCfgDict.TryGetValue(0u, out resGlobalInfo))
				{
					num = resGlobalInfo.dwConfValue;
				}
				if (@int >= num)
				{
					return false;
				}
			}
			return true;
		}

		public static void UpdateLoginShowCnt()
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return;
			}
			string key = string.Format("{0}_{1}", "TODAY_LOGIN_SHOW_ACTIVITY_CNT", masterRoleInfo.playerUllUID);
			uint @int = (uint)PlayerPrefs.GetInt(key, 0);
			PlayerPrefs.SetInt(key, (int)(@int + 1u));
			PlayerPrefs.Save();
		}

		private void OnIDIPNoticeUpdate()
		{
			if (this._campaignForm != null)
			{
				GameObject iDIPRedObj = this._campaignForm.GetIDIPRedObj();
				if (iDIPRedObj)
				{
					if (MonoSingleton<IDIPSys>.GetInstance().HaveUpdateList)
					{
						CUICommonSystem.AddRedDot(iDIPRedObj, enRedDotPos.enTopRight, 0, 0, 0);
					}
					else if (iDIPRedObj != null)
					{
						CUICommonSystem.DelRedDot(iDIPRedObj);
					}
				}
			}
		}

		public void PandroaUpdateBtn()
		{
			if (this._campaignForm != null)
			{
				this._campaignForm.PandroaUpdateBtn();
			}
		}

		public void UpdatePandoraRedPoint()
		{
			if (this._campaignForm != null)
			{
				this._campaignForm.UpdatePandoraRedPoint();
			}
		}

		public void Clear()
		{
			this.m_bShareTask = false;
			this._openActivityKey = 0u;
			this.OnCloseCampaignForm(null);
			if (this._actvDict != null)
			{
				DictionaryView<uint, Activity>.Enumerator enumerator = this._actvDict.GetEnumerator();
				while (enumerator.MoveNext())
				{
					KeyValuePair<uint, Activity> current = enumerator.Current;
					current.get_Value().Clear();
				}
				this._actvDict = null;
			}
			Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref this._checkTimer);
			Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref this._refreshTimer);
		}

		private void TryAdd(Activity actv)
		{
			if (this._actvDict.ContainsKey(actv.Key))
			{
				return;
			}
			actv.CheckTimeState();
			if (actv.timeState == Activity.TimeState.ForeShow || actv.timeState == Activity.TimeState.Going)
			{
				actv.Start();
				this._actvDict.Add(actv.Key, actv);
			}
		}

		public void LoadInfo(ref COMDT_WEAL_LIST actvList)
		{
			if (this._actvDict == null)
			{
				return;
			}
			for (ushort num = 0; num < actvList.wCnt; num += 1)
			{
				COMDT_WEAL_DETAIL cOMDT_WEAL_DETAIL = actvList.astWealDetail[(int)num];
				Activity activity = this.GetActivity((COM_WEAL_TYPE)cOMDT_WEAL_DETAIL.bWealType, cOMDT_WEAL_DETAIL.dwWealID);
				if (activity != null)
				{
					activity.UpdateInfo(ref cOMDT_WEAL_DETAIL.stWealDetail);
				}
			}
		}

		public void LoadPointsData(COMDT_WEAL_POINT_DATA pointsInfo)
		{
			if (pointsInfo != null)
			{
				for (ushort num = 0; num < (ushort)pointsInfo.bWealCnt; num += 1)
				{
					COMDT_WEAL_POINT_DETAIL info = pointsInfo.astWealList[(int)num];
					this.UpdateOnePointData(info);
				}
			}
		}

		public void UpdateOnePointData(COMDT_WEAL_POINT_DETAIL info)
		{
			PointsExchangeActivity pointsExchangeActivity = this.GetActivity(COM_WEAL_TYPE.COM_WEAL_PTEXCHANGE, info.dwWealID) as PointsExchangeActivity;
			if (pointsExchangeActivity != null)
			{
				pointsExchangeActivity.UpdatePointsInfo(info.dwPointCnt, info.dwWealValue);
			}
		}

		public void LoadStatistic(ref COMDT_WEAL_CON_DATA dat)
		{
			for (ushort num = 0; num < dat.wWealNum; num += 1)
			{
				COMDT_WEAL_CON_DATA_DETAIL cOMDT_WEAL_CON_DATA_DETAIL = dat.astWealDetail[(int)num];
				ExeTaskActivity exeTaskActivity = this.GetActivity(COM_WEAL_TYPE.COM_WEAL_CONDITION, cOMDT_WEAL_CON_DATA_DETAIL.dwWealID) as ExeTaskActivity;
				if (exeTaskActivity != null)
				{
					exeTaskActivity.LoadInfo(cOMDT_WEAL_CON_DATA_DETAIL);
				}
			}
		}

		public void StartTimer()
		{
			if (this._checkTimer == 0)
			{
				this._checkTimer = Singleton<CTimerManager>.GetInstance().AddTimer(1000, -1, new CTimer.OnTimeUpHandler(this.OnTimerCheck), false);
			}
			if (this._refreshTimer == 0)
			{
				DateTime dateTime = Utility.ToUtcTime2Local((long)CRoleInfo.GetCurrentUTCTime());
				DateTime dateTime2 = dateTime.AddDays(1.0);
				DateTime dateTime3 = new DateTime(dateTime2.get_Year(), dateTime2.get_Month(), dateTime2.get_Day(), 0, Random.Range(5, 60), Random.Range(0, 60));
				TimeSpan timeSpan = dateTime3 - dateTime;
				this._refreshTimer = Singleton<CTimerManager>.GetInstance().AddTimer((int)(timeSpan.get_TotalSeconds() * 1000.0), 1, new CTimer.OnTimeUpHandler(this.RequestRefresh));
			}
		}

		public void RequestRefresh(int seq = 0)
		{
			if (seq > 0)
			{
				Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref this._refreshTimer);
			}
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2507u);
			cSPkg.stPkgData.stWealDataReq.bReserved = 1;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		public Activity GetActivity(COM_WEAL_TYPE type, uint id)
		{
			if (this._actvDict != null)
			{
				uint key = Activity.GenKey(type, id);
				if (this._actvDict.ContainsKey(key))
				{
					return this._actvDict[key];
				}
			}
			return null;
		}

		private void RemoveActivity(uint key)
		{
			if (this._actvDict != null && this._actvDict.ContainsKey(key))
			{
				this._actvDict[key].Clear();
				this._actvDict.Remove(key);
			}
		}

		public ListView<Activity> GetActivityList(Func<Activity, bool> filter)
		{
			ListView<Activity> listView = new ListView<Activity>();
			if (this._actvDict != null)
			{
				DictionaryView<uint, Activity>.Enumerator enumerator = this._actvDict.GetEnumerator();
				while (enumerator.MoveNext())
				{
					KeyValuePair<uint, Activity> current = enumerator.Current;
					if (filter.Invoke(current.get_Value()))
					{
						ListView<Activity> listView2 = listView;
						KeyValuePair<uint, Activity> current2 = enumerator.Current;
						listView2.Add(current2.get_Value());
					}
				}
			}
			return listView;
		}

		private void OnOpenCampaignForm(CUIEvent uiEvent)
		{
			if (CSysDynamicBlock.bLobbyEntryBlocked || CSysDynamicBlock.bOperationBlock)
			{
				return;
			}
			if (uiEvent != null)
			{
				this._openActivityKey = Activity.GenKey((COM_WEAL_TYPE)uiEvent.m_eventParams.tag, (uint)uiEvent.m_eventParams.tag2);
			}
			if (this._campaignForm != null && this._actvDict != null)
			{
				this.RequestRefresh(0);
				this._campaignForm.Open();
				CMiShuSystem.SendUIClickToServer(enUIClickReprotID.rp_OpenHuodong);
			}
		}

		internal void OnCampaignFormOpened()
		{
			if (this._openActivityKey > 0u)
			{
				if (this._actvDict.ContainsKey(this._openActivityKey))
				{
					this._campaignForm.SelectActivity(this._actvDict[this._openActivityKey]);
				}
				this._openActivityKey = 0u;
			}
			MonoSingleton<IDIPSys>.GetInstance().UpdateGlobalPoint();
		}

		private void OnCloseCampaignForm(CUIEvent uiEvent)
		{
			if (this._campaignForm != null)
			{
				this._campaignForm.Close();
				CUICommonSystem.CloseUseableTips();
			}
		}

		private void OnTimerCheck(int timeSeq)
		{
			if (this._actvDict == null || Singleton<BattleLogic>.GetInstance().isRuning)
			{
				return;
			}
			DictionaryView<uint, Activity>.Enumerator enumerator = this._actvDict.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, Activity> current = enumerator.Current;
				current.get_Value().CheckTimeState();
			}
			this._campaignForm.Update();
		}

		public bool CheckReadyForDot(RES_WEAL_ENTRANCE_TYPE entry)
		{
			if (this._actvDict != null)
			{
				DictionaryView<uint, Activity>.Enumerator enumerator = this._actvDict.GetEnumerator();
				while (enumerator.MoveNext())
				{
					KeyValuePair<uint, Activity> current = enumerator.Current;
					if (current.get_Value().Entrance == entry)
					{
						KeyValuePair<uint, Activity> current2 = enumerator.Current;
						if (current2.get_Value().ReadyForDot)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		public uint GetReveivableRedDot(RES_WEAL_ENTRANCE_TYPE entry)
		{
			uint num = 0u;
			if (this._actvDict != null)
			{
				DictionaryView<uint, Activity>.Enumerator enumerator = this._actvDict.GetEnumerator();
				while (enumerator.MoveNext())
				{
					KeyValuePair<uint, Activity> current = enumerator.Current;
					if (current.get_Value().Entrance == entry)
					{
						KeyValuePair<uint, Activity> current2 = enumerator.Current;
						if (current2.get_Value().ReadyForGet)
						{
							num += 1u;
						}
					}
				}
			}
			return num;
		}

		internal void _NotifyStateChanged()
		{
			if (this.OnStateChange != null)
			{
				this.OnStateChange();
			}
		}

		private void ShowRewards(SCPKG_DRAWWEAL_RSP rspPkg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			Activity activity = Singleton<ActivitySys>.GetInstance().GetActivity((COM_WEAL_TYPE)rspPkg.bWealType, rspPkg.dwWealID);
			if (activity != null)
			{
				if (rspPkg.iResult == 0)
				{
					activity.SetPhaseMarked(rspPkg.dwPeriodID);
					if (rspPkg.stReward.bNum > 0)
					{
						ListView<CUseable> useableListFromReward = CUseableManager.GetUseableListFromReward(rspPkg.stReward);
						for (int i = 0; i < useableListFromReward.Count; i++)
						{
							useableListFromReward[i].m_stackMulti = CUseable.GetMultipleValue(rspPkg.stMultipleInfo, 1) / 10000;
						}
						this._rewardListQueue.Add(new ActivitySys.RewardList(useableListFromReward, rspPkg.iExtraCode));
						Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Common_NewHeroOrSkinFormClose, new CUIEventManager.OnUIEventHandler(this.ShowNextReward));
						Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_Get_AWARD_CLOSE_FORM, new CUIEventManager.OnUIEventHandler(this.ShowNextReward));
						this.ShowNextReward(null);
					}
				}
				else
				{
					string text = "[";
					if (rspPkg.iResult == 8 || rspPkg.iResult == 9)
					{
						text += Singleton<CTextManager>.GetInstance().GetText("payError");
					}
					else
					{
						text += rspPkg.iResult;
					}
					text += "]";
					Singleton<CUIManager>.GetInstance().OpenMessageBox(Singleton<CTextManager>.GetInstance().GetText("drawRewardFailed") + text, false);
				}
			}
		}

		private void ShowNextReward(CUIEvent firstIfNull)
		{
			if (firstIfNull == null && (this._rewardShowList != null || this._rewardQueueIndex > -1))
			{
				return;
			}
			if (this._rewardShowList == null)
			{
				this._rewardQueueIndex++;
				if (this._rewardQueueIndex >= this._rewardListQueue.Count)
				{
					Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Common_NewHeroOrSkinFormClose, new CUIEventManager.OnUIEventHandler(this.ShowNextReward));
					Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Mall_Get_AWARD_CLOSE_FORM, new CUIEventManager.OnUIEventHandler(this.ShowNextReward));
					this._rewardListQueue.Clear();
					this._rewardQueueIndex = -1;
					this._rewardShowIndex = -1;
					return;
				}
				this._rewardShowList = this._rewardListQueue[this._rewardQueueIndex];
				this._rewardShowIndex = -1;
				this._rewardHasSpecial = false;
			}
			while (++this._rewardShowIndex < this._rewardShowList.usabList.Count && this._rewardShowList.usabList[this._rewardShowIndex].MapRewardType != COM_REWARDS_TYPE.COM_REWARDS_TYPE_HERO && this._rewardShowList.usabList[this._rewardShowIndex].MapRewardType != COM_REWARDS_TYPE.COM_REWARDS_TYPE_SKIN && (this._rewardShowList.usabList[this._rewardShowIndex].MapRewardType != COM_REWARDS_TYPE.COM_REWARDS_TYPE_ITEM || (this._rewardShowList.usabList[this._rewardShowIndex].ExtraFromType != 1 && this._rewardShowList.usabList[this._rewardShowIndex].ExtraFromType != 2)))
			{
			}
			if (this._rewardShowIndex < this._rewardShowList.usabList.Count)
			{
				CUseable cUseable = this._rewardShowList.usabList[this._rewardShowIndex];
				if (cUseable.MapRewardType == COM_REWARDS_TYPE.COM_REWARDS_TYPE_ITEM)
				{
					if (cUseable.ExtraFromType == 1)
					{
						int extraFromData = cUseable.ExtraFromData;
						CUICommonSystem.ShowNewHeroOrSkin((uint)extraFromData, 0u, enUIEventID.None, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_HERO, true, null, enFormPriority.Priority1, (uint)cUseable.m_stackCount, 0);
					}
					else if (cUseable.ExtraFromType == 2)
					{
						int extraFromData2 = cUseable.ExtraFromData;
						CUICommonSystem.ShowNewHeroOrSkin(0u, (uint)extraFromData2, enUIEventID.None, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_SKIN, true, null, enFormPriority.Priority1, (uint)cUseable.m_stackCount, 0);
					}
				}
				else if (cUseable is CHeroSkin)
				{
					CHeroSkin cHeroSkin = cUseable as CHeroSkin;
					CUICommonSystem.ShowNewHeroOrSkin(cHeroSkin.m_heroId, cHeroSkin.m_skinId, enUIEventID.None, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_SKIN, true, null, enFormPriority.Priority1, 0u, 0);
				}
				else
				{
					CUICommonSystem.ShowNewHeroOrSkin(cUseable.m_baseID, 0u, enUIEventID.None, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_HERO, true, null, enFormPriority.Priority1, 0u, 0);
				}
				this._rewardHasSpecial = true;
			}
			else if (this._rewardShowList.usabList.Count > 1 || !this._rewardHasSpecial)
			{
				bool flag = (this._rewardShowList.flags & 2) > 0;
				Singleton<CUIManager>.GetInstance().OpenAwardTip(LinqS.ToArray<CUseable>(this._rewardShowList.usabList), Singleton<CTextManager>.GetInstance().GetText(flag ? "gotExtraAward" : "gotAward"), true, enUIEventID.None, false, false, "Form_Award");
				this._rewardShowList = null;
			}
			else
			{
				this._rewardShowList = null;
				this.ShowNextReward(new CUIEvent());
			}
		}

		[MessageHandler(2504)]
		public static void OnActivityDrawRsp(CSPkg msg)
		{
			Singleton<ActivitySys>.GetInstance().ShowRewards(msg.stPkgData.stDrawWealRsp);
		}

		[MessageHandler(2511)]
		public static void OnResExchange(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			if (msg.stPkgData.stWealExchangeRes.dwWealID != 0u)
			{
				ListView<CUseable> useableListFromItemList = CUseableManager.GetUseableListFromItemList(msg.stPkgData.stWealExchangeRes.stExchangeRes);
				if (useableListFromItemList.Count > 0)
				{
					CUseableManager.ShowUseableItem(useableListFromItemList[0]);
				}
				if (msg.stPkgData.stWealExchangeRes.bWealType == 4)
				{
					ExchangeActivity exchangeActivity = (ExchangeActivity)Singleton<ActivitySys>.GetInstance().GetActivity(COM_WEAL_TYPE.COM_WEAL_EXCHANGE, msg.stPkgData.stWealExchangeRes.dwWealID);
					if (exchangeActivity != null)
					{
						exchangeActivity.IncreaseExchangeCount((int)msg.stPkgData.stWealExchangeRes.bWealIdx, msg.stPkgData.stWealExchangeRes.dwDrawCnt);
						exchangeActivity.UpdateView();
					}
				}
				else if (msg.stPkgData.stWealExchangeRes.bWealType == 5)
				{
					PointsExchangeActivity pointsExchangeActivity = (PointsExchangeActivity)Singleton<ActivitySys>.GetInstance().GetActivity(COM_WEAL_TYPE.COM_WEAL_PTEXCHANGE, msg.stPkgData.stWealExchangeRes.dwWealID);
					if (pointsExchangeActivity != null)
					{
						pointsExchangeActivity.IncreaseExchangeCount((int)msg.stPkgData.stWealExchangeRes.bWealIdx, msg.stPkgData.stWealExchangeRes.dwDrawCnt);
						pointsExchangeActivity.UpdateView();
					}
				}
				return;
			}
		}

		public static void OnResetAllExchangeCount()
		{
			ListView<Activity> activityList = Singleton<ActivitySys>.GetInstance().GetActivityList((Activity actv) => actv.Type == COM_WEAL_TYPE.COM_WEAL_EXCHANGE);
			for (int i = 0; i < activityList.Count; i++)
			{
				ExchangeActivity exchangeActivity = activityList[i] as ExchangeActivity;
				if (exchangeActivity != null)
				{
					exchangeActivity.ResetExchangeCount();
					exchangeActivity.UpdateView();
				}
			}
			ListView<Activity> activityList2 = Singleton<ActivitySys>.GetInstance().GetActivityList((Activity actv) => actv.Type == COM_WEAL_TYPE.COM_WEAL_PTEXCHANGE);
			for (int j = 0; j < activityList2.Count; j++)
			{
				PointsExchangeActivity pointsExchangeActivity = activityList2[j] as PointsExchangeActivity;
				if (pointsExchangeActivity != null)
				{
					pointsExchangeActivity.ResetExchangeCount();
					pointsExchangeActivity.UpdateView();
				}
			}
		}

		private void CreateStatic()
		{
			if (this._actvDict == null)
			{
				this._actvDict = new DictionaryView<uint, Activity>();
			}
			DictionaryView<uint, ResWealCheckIn>.Enumerator enumerator = GameDataMgr.wealCheckInDict.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, ResWealCheckIn> current = enumerator.Current;
				this.TryAdd(new CheckInActivity(this, current.get_Value()));
			}
			DictionaryView<uint, ResWealFixedTime>.Enumerator enumerator2 = GameDataMgr.wealFixtimeDict.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				KeyValuePair<uint, ResWealFixedTime> current2 = enumerator2.Current;
				this.TryAdd(new FixTimeActivity(this, current2.get_Value()));
			}
			DictionaryView<uint, ResWealMultiple>.Enumerator enumerator3 = GameDataMgr.wealMultipleDict.GetEnumerator();
			while (enumerator3.MoveNext())
			{
				KeyValuePair<uint, ResWealMultiple> current3 = enumerator3.Current;
				this.TryAdd(new MultiGainActivity(this, current3.get_Value()));
			}
			DictionaryView<uint, ResCltWealExchange>.Enumerator enumerator4 = GameDataMgr.wealExchangeDict.GetEnumerator();
			while (enumerator4.MoveNext())
			{
				KeyValuePair<uint, ResCltWealExchange> current4 = enumerator4.Current;
				this.TryAdd(new ExchangeActivity(this, current4.get_Value()));
			}
			DictionaryView<uint, ResWealPointExchange>.Enumerator enumerator5 = GameDataMgr.wealPointExchangeDict.GetEnumerator();
			while (enumerator5.MoveNext())
			{
				KeyValuePair<uint, ResWealPointExchange> current5 = enumerator5.Current;
				this.TryAdd(new PointsExchangeActivity(this, current5.get_Value()));
			}
			DictionaryView<uint, ResWealCondition>.Enumerator enumerator6 = GameDataMgr.wealConditionDict.GetEnumerator();
			while (enumerator6.MoveNext())
			{
				KeyValuePair<uint, ResWealCondition> current6 = enumerator6.Current;
				this.TryAdd(new ExeTaskActivity(this, current6.get_Value()));
			}
			DictionaryView<uint, ResWealText>.Enumerator enumerator7 = GameDataMgr.wealNoticeDict.GetEnumerator();
			while (enumerator7.MoveNext())
			{
				KeyValuePair<uint, ResWealText> current7 = enumerator7.Current;
				this.TryAdd(new NoticeActivity(this, current7.get_Value()));
			}
		}

		[MessageHandler(2508)]
		public static void OnActivityDataNtf(CSPkg msg)
		{
			Singleton<ActivitySys>.GetInstance().CreateStatic();
			Singleton<ActivitySys>.GetInstance().LoadDynamicData(msg);
			Singleton<ActivitySys>.GetInstance().OnActivityDataReady();
		}

		private void LoadDynamicData(CSPkg msg)
		{
			this.LoadInfo(ref msg.stPkgData.stWealDataNtf.stWealList);
			this.LoadStatistic(ref msg.stPkgData.stWealDataNtf.stWealConData);
			this.LoadPointsData(msg.stPkgData.stWealDataNtf.stWealPointData);
			if (this._actvDict != null)
			{
				uint[] array = new uint[this._actvDict.Count];
				int num = 0;
				DictionaryView<uint, Activity>.Enumerator enumerator = this._actvDict.GetEnumerator();
				while (enumerator.MoveNext())
				{
					KeyValuePair<uint, Activity> current = enumerator.Current;
					Activity value = current.get_Value();
					value.CheckTimeState();
					if (value.timeState == Activity.TimeState.InHiding || value.timeState == Activity.TimeState.Close || (value.Completed && (value.TimeType == RES_WEAL_TIME_TYPE.RES_WEAL_TIME_TYPE_FOREVER || value.TimeType == RES_WEAL_TIME_TYPE.RES_WEAL_TIME_TYPE_REGISTER_LIMIT)))
					{
						array[num++] = value.Key;
					}
				}
				for (int i = 0; i < num; i++)
				{
					this.RemoveActivity(array[i]);
				}
				this._NotifyStateChanged();
				this.StartTimer();
			}
		}

		private void OnActivityDataReady()
		{
			if (!Singleton<BattleLogic>.instance.isRuning && !MonoSingleton<NewbieGuideManager>.GetInstance().isNewbieGuiding && Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().PvpLevel >= 5u && !CSysDynamicBlock.bLobbyEntryBlocked)
			{
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.SevenCheck_LoginOpen);
			}
		}

		[MessageHandler(2505)]
		public static void OnActivityInfoNtf(CSPkg msg)
		{
			Singleton<ActivitySys>.GetInstance().LoadInfo(ref msg.stPkgData.stWealDetailNtf.stWealList);
		}

		[MessageHandler(2512)]
		public static void OnPointsInfoNtf(CSPkg msg)
		{
			if (msg.stPkgData.stWealPointDataNtf != null)
			{
				Singleton<ActivitySys>.GetInstance().UpdateOnePointData(msg.stPkgData.stWealPointDataNtf.stWealData);
			}
		}

		[MessageHandler(2506)]
		public static void OnActivityStatisticNtf(CSPkg msg)
		{
			Singleton<ActivitySys>.GetInstance().LoadStatistic(ref msg.stPkgData.stWealConDataNtf.stWealConData);
		}
	}
}
