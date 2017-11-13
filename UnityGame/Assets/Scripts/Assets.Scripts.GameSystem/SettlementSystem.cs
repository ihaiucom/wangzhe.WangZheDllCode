using Apollo;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.DataCenter;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.Sound;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	[MessageHandlerClass]
	public class SettlementSystem : Singleton<SettlementSystem>
	{
		protected enum ProfitWidgets
		{
			None = -1,
			WinLoseTitle,
			ExpInfo,
			CoinInfo,
			ProficiencyInfo,
			GuildInfo,
			LadderInfo,
			PvpMapInfo,
			GuildPointMaxTip,
			ExpThisWeek,
			GoldThisWeek,
			SymbolCoinInfo
		}

		public enum SettlementWidgets
		{
			None = -1,
			WinLoseTitle,
			ButtonGrid,
			Timer,
			Report,
			TotalScore,
			LeftPlayers,
			RightPlayers,
			LeftPlayersList,
			RightPlayersList,
			AddFriendBtn,
			ReportBtn,
			DetailBtn,
			DamageBtn,
			DetailCaption,
			DamageCaption,
			Duration,
			WaitNote,
			LeftOverViewTitle,
			RightOverViewTitle,
			LeftDamageTitle,
			RightDamageTitle,
			StartTime,
			CreditScore,
			ReplayKitRecord,
			BtnVictoryTips,
			DianZanLaHeiBtn,
			Recorder,
			TimeNode,
			MaxNum
		}

		private enum ShowBtnType
		{
			AddFriend,
			Report,
			LaHeiDianZan
		}

		private const float ExpBarWidth = 220.3f;

		private const float ProficientBarWidth = 159.45f;

		private const int StrHelperLength = 20;

		private const float TweenTime = 2f;

		private const int MaxAchievement = 8;

		private const string ColorStarGameObjectSubPath = "greyStar/colorStar";

		public const string SHARE_UPDATE_GRADE_FORM = "UGUI/Form/System/ShareUI/Form_SharePVPLadder.prefab";

		private readonly string _achievementsTips = string.Format("{0}{1}", "UGUI/Form/System/", "PvP/Settlement/Form_SettleAchievement");

		private SettlementSystem.ShowBtnType _curBtnType;

		private bool _win;

		private bool _neutral;

		public readonly string _profitFormName = string.Format("{0}{1}", "UGUI/Form/System/", "PvP/Settlement/Form_PvpNewProfit.prefab");

		private CUIFormScript _profitFormScript;

		private int playerNum;

		private static readonly string[] StrHelper = new string[20];

		private static readonly string[] StrHelper2 = new string[20];

		private bool _isLadderMatch;

		private LTDescr _coinLtd;

		private LTDescr _symbolCoinLtd;

		private LTDescr _expLtd;

		private float _expFrom;

		private float _expTo;

		private float _coinFrom;

		private float _coinTo;

		private float _symbolCoinFrom;

		private float _symbolCoinTo;

		private Text _coinTweenText;

		private Text _symbolCoinTweenText;

		private RectTransform _expTweenRect;

		private static uint _lvUpGrade = 0u;

		public static readonly string SettlementFormName = string.Format("{0}{1}", "UGUI/Form/System/", "PvP/Settlement/Form_PvpNewSettlement.prefab");

		private static string PlayerWinTimesStr = "PlayerWinTimes";

		private CUIFormScript _settleFormScript;

		private CUIListScript _leftListScript;

		private CUIListScript _rightListScript;

		private int _curLeftIndex;

		private int _curRightIndex;

		private GameObject m_ShareDataBtn;

		private int m_bMvp;

		private int m_bLegaendary;

		private int m_bPENTAKILL;

		private int m_bQUATARYKIL;

		private int m_bTRIPLEKILL;

		private int m_bWin;

		private bool m_bSendRedBag;

		private bool m_bSendPandoraMsg;

		private COM_GAME_TYPE m_GameMode = COM_GAME_TYPE.COM_SINGLE_GAME_OF_COMBAT;

		private int m_TotalPlayerNum;

		private bool m_bIsWarmBattle;

		private bool m_bHaveCpu;

		private enSelectType m_selectTypeHero = enSelectType.enNull;

		private int m_mapID;

		private bool m_bLuanDou;

		private bool m_bFireHole;

		private string _duration;

		private string _startTime;

		private uint _startTimeInt;

		private uint _camp1TotalDamage;

		private uint _camp1TotalTakenDamage;

		private uint _camp1TotalToHeroDamage;

		private uint _camp2TotalDamage;

		private uint _camp2TotalTakenDamage;

		private uint _camp2TotalToHeroDamage;

		private uint _camp1TotalKill;

		private uint _camp2TotalKill;

		private GameObject _cacheLastReportGo;

		private ulong _reportUid;

		private int _reportWordId;

		private COM_PLAYERCAMP _myCamp = COM_PLAYERCAMP.COM_PLAYERCAMP_1;

		private readonly string _ladderFormName = string.Format("{0}{1}", "UGUI/Form/System/", "PvP/Settlement/Form_LadderSettle");

		private bool _lastLadderWin;

		private bool _isSettle;

		private CUIFormScript _ladderForm;

		private GameObject _ladderRoot;

		private Animator _ladderAnimator;

		private bool _isUp;

		private bool _isDown;

		private bool _isBraveScoreIncreased;

		private bool m_isRisingStarAnimationStarted;

		private uint _oldGrade = 1u;

		private uint _oldLogicGrade = 1u;

		private uint _oldScore = 1u;

		private uint _oldMaxScore = 3u;

		private uint _newGrade = 1u;

		private uint _newLogicGrade = 1u;

		private uint _newScore = 1u;

		private uint _newMaxScore = 3u;

		private uint _curDian = 1u;

		private uint _curGrade = 1u;

		private uint _curMaxScore = 3u;

		private bool _changingGrage;

		private bool _doWangZheSpecial;

		private bool _seasonFirstBeWangZhe;

		private bool m_bGrade;

		private PvpAchievementForm m_sharePVPAchieventForm;

		private bool m_bLastAddFriendBtnState;

		private bool m_bLastReprotBtnState;

		private bool m_bLastOverViewBtnState;

		private bool m_bLastDataBtnState;

		private bool m_bShareDataSucc;

		private bool m_bShareOverView;

		private bool m_bIsDetail = true;

		private bool m_bBackShowTimeLine;

		private Transform m_PVPBtnGroup;

		private Transform m_PVPSwtichAddFriend;

		private Transform m_PVPSwitchStatistics;

		private Transform m_PVPSwitchOverview;

		private Transform m_PVPShareDataBtnGroup;

		private Transform m_PVPShareBtnClose;

		private Text m_timeLineText;

		private Transform m_BtnTimeLine;

		private Text m_TxtBtnShareCaption;

		private Transform m_btnVictotyTips;

		private Transform m_ChatNode;

		private Transform m_logIcon;

		private CUIFormScript m_UpdateGradeForm;

		public int HostPlayerHeroId
		{
			get;
			private set;
		}

		public override void Init()
		{
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_ProfitContinue, new CUIEventManager.OnUIEventHandler(this.OnClickProfitContinue));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_TimerEnd, new CUIEventManager.OnUIEventHandler(this.OnSettlementTimerEnd));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_ClickBack, new CUIEventManager.OnUIEventHandler(this.OnClickBack));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_ClickAgain, new CUIEventManager.OnUIEventHandler(this.OnClickBattleAgain));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_SaveReplay, new CUIEventManager.OnUIEventHandler(this.OnClickSaveReplay));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_ClickStatistics, new CUIEventManager.OnUIEventHandler(this.OnSwitchStatistics));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_ShowReport, new CUIEventManager.OnUIEventHandler(this.OnShowReport));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_CloseReport, new CUIEventManager.OnUIEventHandler(this.OnCloseReport));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_DoReport, new CUIEventManager.OnUIEventHandler(this.OnDoReport));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_ClickAddFriend, new CUIEventManager.OnUIEventHandler(this.OnAddFriend));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_ClickDianLa, new CUIEventManager.OnUIEventHandler(this.OnClickDianZanLaHei));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_OnCloseProfit, new CUIEventManager.OnUIEventHandler(this.OnCloseProfit));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_OnCloseSettlement, new CUIEventManager.OnUIEventHandler(this.OnCloseSettlement));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_SwitchAddFriendReport, new CUIEventManager.OnUIEventHandler(this.OnSwitchAddFriendReport));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_ClickLadderContinue, new CUIEventManager.OnUIEventHandler(this.OnLadderClickContinue));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_ClickShowAchievements, new CUIEventManager.OnUIEventHandler(this.OnShowAchievements));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_OpenSharePVPDefeat, new CUIEventManager.OnUIEventHandler(this.OnShowDefeatShare));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_ClickItemDisplay, new CUIEventManager.OnUIEventHandler(this.OnClickItemDisplay));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Share_ShowPVPSettleData, new CUIEventManager.OnUIEventHandler(this.OnShowPVPSettleData));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Share_ShowPVPSettleDataClose, new CUIEventManager.OnUIEventHandler(this.OnShowPVPSettleDataClose));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Share_ShowUpdateGradeShare, new CUIEventManager.OnUIEventHandler(this.OnShareUpdateGradShare));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Share_ShowUpdateGradeShareClose, new CUIEventManager.OnUIEventHandler(this.OnShareUpdateGradShareClose));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_Ladder_OnAnimatiorBravePanelShowInEnd, new CUIEventManager.OnUIEventHandler(this.OnAnimatiorBravePanelShowInEnd));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_Ladder_OnAnimatiorBraveProgressFillEnd, new CUIEventManager.OnUIEventHandler(this.OnAnimatiorBraveProgressFillEnd));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_Ladder_OnAnimatiorBraveDigitalJitterEnd, new CUIEventManager.OnUIEventHandler(this.OnAnimatiorBraveDigitalJitterEnd));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_Ladder_OnAnimatiorBraveDigitalReductionEnd, new CUIEventManager.OnUIEventHandler(this.OnAnimatiorBraveeDigitalReductionEnd));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_Ladder_OnAnimatiorBraveDigitalRisingStarEnd, new CUIEventManager.OnUIEventHandler(this.OnAnimatiorBraveDigitalRisingStarEnd));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_Ladder_OnAnimatiorBraveProgressFillFull, new CUIEventManager.OnUIEventHandler(this.OnAnimatiorBraveProgressFillFull));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_ShowAddFriendBtn, new CUIEventManager.OnUIEventHandler(this.OnShowFriendBtn));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_ShowReportBtn, new CUIEventManager.OnUIEventHandler(this.OnShowReportBtn));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_ShowDianLaBtn, new CUIEventManager.OnUIEventHandler(this.OnShowLaHeiDianZanBtn));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_OpenPlayerDetailInfo, new CUIEventManager.OnUIEventHandler(this.OnOpenPlayerDetailInfo));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.SettlementSys_GetMoreGold, new CUIEventManager.OnUIEventHandler(this.OnGetMoreGold));
			this.m_sharePVPAchieventForm = new PvpAchievementForm();
		}

		public override void UnInit()
		{
			base.UnInit();
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_ProfitContinue, new CUIEventManager.OnUIEventHandler(this.OnClickProfitContinue));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_TimerEnd, new CUIEventManager.OnUIEventHandler(this.OnSettlementTimerEnd));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_ClickBack, new CUIEventManager.OnUIEventHandler(this.OnClickBack));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_ClickAgain, new CUIEventManager.OnUIEventHandler(this.OnClickBattleAgain));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_SaveReplay, new CUIEventManager.OnUIEventHandler(this.OnClickSaveReplay));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_ClickStatistics, new CUIEventManager.OnUIEventHandler(this.OnSwitchStatistics));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_ShowReport, new CUIEventManager.OnUIEventHandler(this.OnShowReport));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_CloseReport, new CUIEventManager.OnUIEventHandler(this.OnCloseReport));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_DoReport, new CUIEventManager.OnUIEventHandler(this.OnDoReport));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_ClickAddFriend, new CUIEventManager.OnUIEventHandler(this.OnAddFriend));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_ClickDianLa, new CUIEventManager.OnUIEventHandler(this.OnClickDianZanLaHei));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_OnCloseProfit, new CUIEventManager.OnUIEventHandler(this.OnCloseProfit));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_OnCloseSettlement, new CUIEventManager.OnUIEventHandler(this.OnCloseSettlement));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_SwitchAddFriendReport, new CUIEventManager.OnUIEventHandler(this.OnSwitchAddFriendReport));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_ClickLadderContinue, new CUIEventManager.OnUIEventHandler(this.OnLadderClickContinue));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_ClickShowAchievements, new CUIEventManager.OnUIEventHandler(this.OnShowAchievements));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_OpenSharePVPDefeat, new CUIEventManager.OnUIEventHandler(this.OnShowDefeatShare));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_ClickItemDisplay, new CUIEventManager.OnUIEventHandler(this.OnClickItemDisplay));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Share_ShowPVPSettleData, new CUIEventManager.OnUIEventHandler(this.OnShowPVPSettleData));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Share_ShowPVPSettleDataClose, new CUIEventManager.OnUIEventHandler(this.OnShowPVPSettleDataClose));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Share_ShowUpdateGradeShare, new CUIEventManager.OnUIEventHandler(this.OnShareUpdateGradShare));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Share_ShowUpdateGradeShareClose, new CUIEventManager.OnUIEventHandler(this.OnShareUpdateGradShareClose));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_ShowAddFriendBtn, new CUIEventManager.OnUIEventHandler(this.OnShowFriendBtn));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_ShowReportBtn, new CUIEventManager.OnUIEventHandler(this.OnShowReportBtn));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.SettlementSys_ShowDianLaBtn, new CUIEventManager.OnUIEventHandler(this.OnShowLaHeiDianZanBtn));
		}

		private void OnClickProfitContinue(CUIEvent uiEvent)
		{
			this.ClosePersonalProfit();
			this.CheckPVPAchievement();
			MonoSingleton<ShareSys>.GetInstance().m_bShowTimeline = false;
		}

		private void OnSettlementTimerEnd(CUIEvent uiEvent)
		{
			this.ImpSettlementTimerEnd();
		}

		private void OnClickBack(CUIEvent uiEvent)
		{
			this.CloseSettlementPanel();
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			if (curLvelContext == null)
			{
				return;
			}
			if (!curLvelContext.IsGameTypeRewardMatch() && !curLvelContext.IsGameTypeLadder() && !curLvelContext.IsGameTypeGuildMatch())
			{
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Matching_OpenEntry);
			}
		}

		private void OnClickSaveReplay(CUIEvent uiEvent)
		{
			if (Singleton<GameReplayModule>.HasInstance())
			{
				if (Singleton<GameReplayModule>.GetInstance().FlushRecord())
				{
					Singleton<CUIManager>.GetInstance().OpenTips("replaySaved", true, 1.5f, null, new object[0]);
				}
				else
				{
					Singleton<CUIManager>.GetInstance().OpenTips("replaySaveFailed", true, 1.5f, null, new object[0]);
				}
			}
			if (this._settleFormScript)
			{
				CUICommonSystem.SetButtonEnable(Utility.GetComponetInChild<Button>(this._settleFormScript.gameObject, "Panel/ButtonGrid/BtnSaveReplay"), false, false, true);
			}
		}

		private void OnClickBattleAgain(CUIEvent uiEvent)
		{
			this.CloseSettlementPanel();
			if (this._isLadderMatch)
			{
				CUIEvent uiEvent2 = new CUIEvent
				{
					m_eventID = enUIEventID.Matching_OpenLadder
				};
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uiEvent2);
			}
			else
			{
				SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
				if (curLvelContext != null && curLvelContext.IsGameTypeGuildMatch())
				{
					Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Guild_Match_OpenMatchForm);
					return;
				}
				if (curLvelContext != null && !curLvelContext.IsGameTypeRewardMatch())
				{
					Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Matching_OpenEntry);
					return;
				}
				if (curLvelContext != null && curLvelContext.IsGameTypeRewardMatch())
				{
					CUIEvent cUIEvent = new CUIEvent();
					cUIEvent.m_eventParams.tag = 0;
					cUIEvent.m_eventID = enUIEventID.Union_Battle_BattleEntryGroup_Click;
					Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(cUIEvent);
					return;
				}
				if (Singleton<CMatchingSystem>.instance.cacheMathingInfo == null || Singleton<CMatchingSystem>.instance.cacheMathingInfo.uiEventId == enUIEventID.None)
				{
					return;
				}
				if (Singleton<CMatchingSystem>.instance.cacheMathingInfo.uiEventId == enUIEventID.Room_CreateRoom)
				{
					CRoomSystem.ReqCreateRoom(Singleton<CMatchingSystem>.instance.cacheMathingInfo.mapId, Singleton<CMatchingSystem>.instance.cacheMathingInfo.mapType, false);
				}
				else
				{
					CUIEvent cUIEvent2 = new CUIEvent();
					cUIEvent2.m_eventID = Singleton<CMatchingSystem>.instance.cacheMathingInfo.uiEventId;
					cUIEvent2.m_eventParams.tagUInt = Singleton<CMatchingSystem>.instance.cacheMathingInfo.mapId;
					cUIEvent2.m_eventParams.tag = (int)Singleton<CMatchingSystem>.instance.cacheMathingInfo.AILevel;
					CUIEvent uiEvent3 = cUIEvent2;
					Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uiEvent3);
				}
			}
		}

		private void OnShowAchievements(CUIEvent uiEvent)
		{
			Singleton<CUIManager>.instance.OpenForm(this._achievementsTips, false, true);
		}

		private void OnSwitchStatistics(CUIEvent uiEvent)
		{
			this.ImpSwitchStatistics();
		}

		private void OnShowFriendBtn(CUIEvent uiEvent)
		{
			this.ImpSwitchAddFriendReportLaHeiDianZan(SettlementSystem.ShowBtnType.AddFriend);
		}

		private void OnShowReportBtn(CUIEvent uiEvent)
		{
			this.ImpSwitchAddFriendReportLaHeiDianZan(SettlementSystem.ShowBtnType.Report);
		}

		private void OnShowLaHeiDianZanBtn(CUIEvent uiEvent)
		{
			this.ImpSwitchAddFriendReportLaHeiDianZan(SettlementSystem.ShowBtnType.LaHeiDianZan);
		}

		private void OnSwitchAddFriendReport(CUIEvent uiEvent)
		{
		}

		private void OnShowReport(CUIEvent uiEvent)
		{
			this.ImpShowReport(uiEvent);
		}

		private void OnCloseReport(CUIEvent uiEvent)
		{
			this.ImpCloseReport(uiEvent);
		}

		private void OnAddFriend(CUIEvent uiEvent)
		{
			this.ImpAddFriend(uiEvent);
		}

		private void OnClickDianZanLaHei(CUIEvent uiEvent)
		{
			uint commonUInt32Param = uiEvent.m_eventParams.commonUInt32Param1;
			uint commonUInt16Param = (uint)uiEvent.m_eventParams.commonUInt16Param1;
			ulong commonUInt64Param = uiEvent.m_eventParams.commonUInt64Param1;
			int num = (int)uiEvent.m_eventParams.commonUInt64Param2;
			string text = string.Empty;
			CPlayerKDAStat playerKDAStat = Singleton<BattleLogic>.GetInstance().battleStat.m_playerKDAStat;
			DictionaryView<uint, PlayerKDA>.Enumerator enumerator = playerKDAStat.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
				PlayerKDA value = current.get_Value();
				if (value != null && value.PlayerUid == commonUInt64Param && value.WorldId == num)
				{
					text = value.PlayerName;
					break;
				}
			}
			if (commonUInt32Param == commonUInt16Param)
			{
				Singleton<CUIManager>.instance.OpenTips(string.Format(Singleton<CTextManager>.GetInstance().GetText("ZanTeam"), text), false, 1.5f, null, new object[0]);
			}
			else
			{
				Singleton<CUIManager>.instance.OpenTips(string.Format(Singleton<CTextManager>.GetInstance().GetText("ZanEnemyTeam"), text), false, 1.5f, null, new object[0]);
			}
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1333u);
			cSPkg.stPkgData.stLikeReq.stAcntUin.ullUid = commonUInt64Param;
			cSPkg.stPkgData.stLikeReq.stAcntUin.dwLogicWorldId = (uint)num;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
			uiEvent.m_srcWidget.CustomSetActive(false);
		}

		[MessageHandler(1334)]
		public static void On_SCPKG_CMD_GIVE_THUMBS(CSPkg msg)
		{
			SCPKG_CMD_NTF_LIKE stLikeNtf = msg.stPkgData.stLikeNtf;
			string text = null;
			string text2 = null;
			bool flag = false;
			CPlayerKDAStat playerKDAStat = Singleton<BattleLogic>.GetInstance().battleStat.m_playerKDAStat;
			DictionaryView<uint, PlayerKDA>.Enumerator enumerator = playerKDAStat.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
				PlayerKDA value = current.get_Value();
				if (value != null)
				{
					if (text == null && value.PlayerUid == stLikeNtf.stLikeUin.ullUid && (long)value.WorldId == (long)((ulong)stLikeNtf.stLikeUin.dwLogicWorldId))
					{
						text = value.PlayerName;
						flag = value.IsHost;
					}
					else if (text2 == null && value.PlayerUid == stLikeNtf.stAcntUin.ullUid && (long)value.WorldId == (long)((ulong)stLikeNtf.stAcntUin.dwLogicWorldId))
					{
						text2 = value.PlayerName;
					}
				}
			}
			if (text == null || text2 == null)
			{
				return;
			}
			Singleton<CChatController>.GetInstance().BuildPlayerGiveThumbsSysTemMsg(text2, text);
			if (flag)
			{
				if (stLikeNtf.iLikeType == 0)
				{
					Singleton<CUIManager>.instance.OpenTips(string.Format(Singleton<CTextManager>.GetInstance().GetText("Teammate_Give_Me_A_Thumbs"), text2), false, 1.5f, null, new object[0]);
				}
				else
				{
					Singleton<CUIManager>.instance.OpenTips(string.Format(Singleton<CTextManager>.GetInstance().GetText("Opponent_Give_Me_A_Thumbs"), text2), false, 1.5f, null, new object[0]);
				}
			}
		}

		private void OnDoReport(CUIEvent uiEvent)
		{
			this.ImpDoReport(uiEvent);
		}

		private void OnCloseProfit(CUIEvent uiEvent)
		{
			this.DoCoinTweenEnd();
			this.DoExpTweenEnd();
			this._profitFormScript = null;
		}

		private void OnCloseSettlement(CUIEvent uiEvent)
		{
			Singleton<CChatController>.instance.model.channelMgr.Clear(EChatChannel.Settle, 0uL, 0u);
			Singleton<CChatController>.instance.model.channelMgr.SetChatTab(CChatChannelMgr.EChatTab.Normal);
			Singleton<CChatController>.instance.view.UpView(false);
			Singleton<CChatController>.instance.model.sysData.ClearEntryText();
			Singleton<CChatController>.instance.ShowPanel(false, false);
			CChatNetUT.Send_Leave_Settle();
			this._settleFormScript = null;
			this._leftListScript = null;
			this._rightListScript = null;
			this._cacheLastReportGo = null;
			this._curBtnType = SettlementSystem.ShowBtnType.AddFriend;
			this.ClearShareData();
			Singleton<GameReplayModule>.GetInstance().ClearRecord();
			Singleton<CRecordUseSDK>.instance.CallGameJoyGenerateWithNothing();
			Singleton<CFriendContoller>.instance.model.friendReserve.CheckShowAcceptReceiveReserve();
		}

		private void OnOpenPlayerDetailInfo(CUIEvent uiEvent)
		{
			ulong commonUInt64Param = uiEvent.m_eventParams.commonUInt64Param1;
			int tag = uiEvent.m_eventParams.tag;
			bool commonBool = uiEvent.m_eventParams.commonBool;
			if (Utility.IsValidPlayer(commonUInt64Param, tag) && !Utility.IsSelf(commonUInt64Param, tag) && !commonBool)
			{
				Singleton<CPlayerInfoSystem>.GetInstance().ShowPlayerDetailInfo(commonUInt64Param, tag, CPlayerInfoSystem.DetailPlayerInfoSource.DefaultOthers, true, CPlayerInfoSystem.Tab.Base_Info);
			}
		}

		private void OnGetMoreGold(CUIEvent uiEvent)
		{
			ushort txtKey = 17;
			Singleton<CUIManager>.GetInstance().OpenInfoForm((int)txtKey);
		}

		public void ShowPersonalProfit(bool win)
		{
			if (Singleton<CUIManager>.GetInstance().GetForm(this._profitFormName) != null)
			{
				return;
			}
			this._win = win;
			this._profitFormScript = Singleton<CUIManager>.GetInstance().OpenForm(this._profitFormName, false, true);
			if (this._profitFormScript == null)
			{
				return;
			}
			this.SetTitle();
			this.SetExpProfit();
			this.SetGoldCoinProfit();
			this.SetSymbolCoinProfit();
			this.SetMapInfo();
			this.SetProficiencyInfo();
			this.SetGuildInfo();
			this.SetLadderInfo();
			this.SetSpecialItem();
		}

		public void OnClickItemDisplay(CUIEvent uiEvent)
		{
			this.DoCoinAndExpTween();
		}

		public void ClosePersonalProfit()
		{
			this.DoCoinTweenEnd();
			this.DoExpTweenEnd();
			this._profitFormScript = null;
			Singleton<CUIManager>.GetInstance().CloseForm(this._profitFormName);
		}

		private void SetSpecialItem()
		{
			COMDT_PVPSPECITEM_OUTPUT specialItemInfo = Singleton<BattleStatistic>.GetInstance().SpecialItemInfo;
			COMDT_REWARD_DETAIL rewards = Singleton<BattleStatistic>.GetInstance().Rewards;
			CUseable[] array = new CUseable[(int)(specialItemInfo.bOutputCnt + rewards.bNum)];
			int num = 0;
			if (rewards.bNum > 0)
			{
				ListView<CUseable> useableListFromReward = CUseableManager.GetUseableListFromReward(rewards);
				for (int i = 0; i < useableListFromReward.Count; i++)
				{
					array[num] = useableListFromReward[i];
					num++;
				}
			}
			if (specialItemInfo.bOutputCnt > 0)
			{
				DictionaryView<uint, ResPVPSpecItem> pvpSpecialItemDict = GameDataMgr.pvpSpecialItemDict;
				if (pvpSpecialItemDict != null)
				{
					for (int j = 0; j < (int)specialItemInfo.bOutputCnt; j++)
					{
						ResPVPSpecItem resPVPSpecItem = null;
						if (pvpSpecialItemDict.TryGetValue(specialItemInfo.astOutputInfo[j].dwPVPSpecItemID, out resPVPSpecItem))
						{
							array[num] = CUseableManager.CreateUsableByServerType((RES_REWARDS_TYPE)resPVPSpecItem.wItemType, (int)specialItemInfo.astOutputInfo[j].dwPVPSpecItemCnt, resPVPSpecItem.dwItemID);
						}
						num++;
					}
				}
			}
			string text = Singleton<CTextManager>.GetInstance().GetText("gotAward");
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			if (curLvelContext != null && curLvelContext.IsGameTypeRewardMatch())
			{
				if (this._win)
				{
					text = Singleton<CTextManager>.GetInstance().GetText("Union_Battle_Tips8");
				}
				else
				{
					text = Singleton<CTextManager>.GetInstance().GetText("Union_Battle_Tips9");
				}
			}
			if (array.Length == 0)
			{
				this.DoCoinAndExpTween();
			}
			else if (curLvelContext != null && curLvelContext.IsGameTypeRewardMatch())
			{
				Singleton<CUIManager>.GetInstance().OpenAwardTip(array, text, true, enUIEventID.SettlementSys_ClickItemDisplay, false, true, "Form_AwardGold");
			}
			else
			{
				Singleton<CUIManager>.GetInstance().OpenAwardTip(array, text, true, enUIEventID.SettlementSys_ClickItemDisplay, false, true, "Form_Award");
			}
		}

		private void SetTitle()
		{
			GameObject gameObject = this._profitFormScript.m_formWidgets[0];
			gameObject.transform.FindChild("Win").gameObject.CustomSetActive(this._win);
			gameObject.transform.FindChild("Lose").gameObject.CustomSetActive(!this._win);
		}

		private void SetExpProfit()
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return;
			}
			ResAcntPvpExpInfo dataByKey = GameDataMgr.acntPvpExpDatabin.GetDataByKey((uint)((byte)masterRoleInfo.PvpLevel));
			if (dataByKey == null)
			{
				return;
			}
			COMDT_ACNT_INFO acntInfo = Singleton<BattleStatistic>.GetInstance().acntInfo;
			if (acntInfo == null)
			{
				return;
			}
			GameObject gameObject = this._profitFormScript.m_formWidgets[1];
			gameObject.transform.FindChild("PlayerName").gameObject.GetComponent<Text>().set_text(masterRoleInfo.Name);
			gameObject.transform.FindChild("PlayerLv").gameObject.GetComponent<Text>().set_text(string.Format("Lv.{0}", masterRoleInfo.PvpLevel));
			Text component = gameObject.transform.FindChild("ExpMaxTip").gameObject.GetComponent<Text>();
			component.set_text((acntInfo.bExpDailyLimit > 0) ? Singleton<CTextManager>.GetInstance().GetText("GetExp_Limit") : string.Empty);
			Text component2 = gameObject.transform.FindChild("PvpExpTxt").gameObject.GetComponent<Text>();
			component2.set_text(string.Format("{0}/{1}", acntInfo.dwPvpExp, dataByKey.dwNeedExp));
			Text component3 = gameObject.transform.FindChild("AddPvpExpTxt").gameObject.GetComponent<Text>();
			component3.set_text((acntInfo.dwPvpSettleExp >= 0u) ? string.Format("+{0}", acntInfo.dwPvpSettleExp) : acntInfo.dwPvpSettleExp.ToString());
			RectTransform component4 = gameObject.transform.FindChild("PvpExpSliderBg/BasePvpExpSlider").gameObject.GetComponent<RectTransform>();
			RectTransform component5 = gameObject.transform.FindChild("PvpExpSliderBg/AddPvpExpSlider").gameObject.GetComponent<RectTransform>();
			if (acntInfo.dwPvpSettleExp > 0u)
			{
				Singleton<CSoundManager>.GetInstance().PostEvent("UI_count_jingyan", null);
			}
			int num = (int)(acntInfo.dwPvpExp - acntInfo.dwPvpSettleExp);
			SettlementSystem._lvUpGrade = ((num < 0) ? acntInfo.dwPvpLv : 0u);
			float num2 = Mathf.Max(0f, (float)num / dataByKey.dwNeedExp);
			float num3 = Mathf.Max(0f, ((num < 0) ? acntInfo.dwPvpExp : acntInfo.dwPvpSettleExp) / dataByKey.dwNeedExp);
			component4.sizeDelta = new Vector2(num2 * 220.3f, component4.sizeDelta.y);
			component5.sizeDelta = new Vector2(num2 * 220.3f, component5.sizeDelta.y);
			this._expFrom = num2;
			this._expTo = num2 + num3;
			this._expTweenRect = component5;
			component4.gameObject.CustomSetActive(num >= 0);
			CUIHttpImageScript component6 = gameObject.transform.FindChild("HeadImage").GetComponent<CUIHttpImageScript>();
			Image component7 = gameObject.transform.FindChild("NobeIcon").GetComponent<Image>();
			Image component8 = gameObject.transform.FindChild("HeadFrame").GetComponent<Image>();
			if (!CSysDynamicBlock.bSocialBlocked)
			{
				string headUrl = masterRoleInfo.HeadUrl;
				component6.SetImageUrl(headUrl);
				MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(component7, (int)masterRoleInfo.GetNobeInfo().stGameVipClient.dwCurLevel, false, true, 0uL);
				MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(component8, (int)masterRoleInfo.GetNobeInfo().stGameVipClient.dwHeadIconId);
			}
			else
			{
				MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(component7, 0, false, true, 0uL);
			}
			GameObject gameObject2 = gameObject.transform.FindChild("ExpGroup/DoubleExp").gameObject;
			gameObject2.CustomSetActive(false);
			COMDT_REWARD_MULTIPLE_DETAIL multiDetail = Singleton<BattleStatistic>.GetInstance().multiDetail;
			for (int i = 0; i < SettlementSystem.StrHelper.Length; i++)
			{
				SettlementSystem.StrHelper[i] = null;
			}
			if (multiDetail == null)
			{
				return;
			}
			GameObject gameObject3 = this._profitFormScript.m_formWidgets[8];
			Text component9 = gameObject3.transform.GetComponent<Text>();
			string key = (masterRoleInfo.m_expWeekCur >= masterRoleInfo.m_expWeekLimit) ? "Settlement_WEEK_EXP_limited_full" : "Settlement_WEEK_EXP_limited";
			component9.set_text(Singleton<CTextManager>.GetInstance().GetText(key, new string[]
			{
				masterRoleInfo.m_expWeekCur.ToString(),
				masterRoleInfo.m_expWeekLimit.ToString()
			}));
			GameObject gameObject4 = this._profitFormScript.m_formWidgets[9];
			Text component10 = gameObject4.transform.GetComponent<Text>();
			key = ((masterRoleInfo.m_goldWeekCur >= masterRoleInfo.m_goldWeekLimit) ? "Settlement_WEEK_GOLD_limited_full" : "Settlement_WEEK_GOLD_limited");
			component10.set_text(Singleton<CTextManager>.GetInstance().GetText(key, new string[]
			{
				masterRoleInfo.m_goldWeekCur.ToString(),
				masterRoleInfo.m_goldWeekLimit.ToString()
			}));
			int multiple = CUseable.GetMultiple(acntInfo.dwPvpSettleBaseExp, ref multiDetail, 15, -1);
			COMDT_MULTIPLE_DATA[] array = null;
			uint multipleInfo = CUseable.GetMultipleInfo(out array, ref multiDetail, 15, -1);
			acntInfo = Singleton<BattleStatistic>.GetInstance().acntInfo;
			string text = string.Empty;
			if (multiple > 0)
			{
				text += "+";
			}
			text += multiple.ToString();
			SettlementSystem.StrHelper[0] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_1", new string[]
			{
				text
			});
			if (array != null)
			{
				int num4 = 0;
				while ((long)num4 < (long)((ulong)multipleInfo))
				{
					string text2 = string.Empty;
					if ((ulong)acntInfo.dwPvpSettleBaseExp * (ulong)((long)array[num4].iValue) > 0uL)
					{
						text2 = "+";
					}
					byte bOperator = array[num4].bOperator;
					if (bOperator != 0)
					{
						if (bOperator != 1)
						{
							text2 += "0";
						}
						else
						{
							double num5 = acntInfo.dwPvpSettleBaseExp * (double)array[num4].iValue / 10000.0;
							if (num5 > 0.0)
							{
								text2 += (int)(num5 + 0.9999);
							}
							else if (num5 < 0.0)
							{
								text2 += (int)(num5 - 0.9999);
							}
							else
							{
								text2 = "0";
							}
						}
					}
					else
					{
						text2 += array[num4].iValue;
					}
					SettlementSystem.StrHelper2[num4 + 1] = string.Empty;
					int num6 = array[num4].iValue / 100;
					if (num6 >= 0)
					{
						string[] strHelper = SettlementSystem.StrHelper2;
						int num7 = num4 + 1;
						string[] expr_671_cp_0 = strHelper;
						int expr_671_cp_1 = num7;
						expr_671_cp_0[expr_671_cp_1] += "+";
					}
					string[] strHelper2 = SettlementSystem.StrHelper2;
					int num8 = num4 + 1;
					string[] expr_694_cp_0 = strHelper2;
					int expr_694_cp_1 = num8;
					expr_694_cp_0[expr_694_cp_1] += num6;
					switch (array[num4].iType)
					{
					case 1:
						SettlementSystem.StrHelper[num4 + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_6", new string[]
						{
							text2
						});
						break;
					case 2:
						if (masterRoleInfo.HasVip(16))
						{
							SettlementSystem.StrHelper[num4 + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_9", new string[]
							{
								text2
							});
						}
						else
						{
							SettlementSystem.StrHelper[num4 + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_3", new string[]
							{
								text2
							});
						}
						break;
					case 3:
						SettlementSystem.StrHelper[num4 + 1] = string.Format(Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_4"), text2, masterRoleInfo.GetExpWinCount(), Math.Ceiling((double)((float)masterRoleInfo.GetExpExpireHours() / 24f)));
						break;
					case 4:
						SettlementSystem.StrHelper[num4 + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_2", new string[]
						{
							masterRoleInfo.dailyPvpCnt.ToString(),
							text2
						});
						break;
					case 5:
						SettlementSystem.StrHelper[num4 + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_13", new string[]
						{
							text2
						});
						break;
					case 6:
						SettlementSystem.StrHelper[num4 + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_15", new string[]
						{
							text2
						});
						break;
					case 7:
						SettlementSystem.StrHelper[num4 + 1] = Singleton<CTextManager>.instance.GetText("Daily_Quest_FirstVictoryName", new string[]
						{
							text2
						});
						break;
					case 8:
						SettlementSystem.StrHelper[num4 + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_16", new string[]
						{
							text2
						});
						break;
					case 9:
						SettlementSystem.StrHelper[num4 + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_14", new string[]
						{
							text2
						});
						break;
					case 10:
						SettlementSystem.StrHelper[num4 + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_21", new string[]
						{
							text2
						});
						break;
					case 11:
						SettlementSystem.StrHelper[num4 + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_17", new string[]
						{
							text2
						});
						break;
					case 12:
						SettlementSystem.StrHelper[num4 + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_18", new string[]
						{
							text2
						});
						break;
					case 13:
						SettlementSystem.StrHelper[num4 + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_20", new string[]
						{
							text2
						});
						break;
					case 14:
						SettlementSystem.StrHelper[num4 + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_19", new string[]
						{
							text2
						});
						break;
					}
					num4++;
				}
			}
			string text3 = SettlementSystem.StrHelper[0];
			for (int j = 1; j < SettlementSystem.StrHelper.Length; j++)
			{
				if (!string.IsNullOrEmpty(SettlementSystem.StrHelper[j]))
				{
					if (array[j].iType == 7 || string.IsNullOrEmpty(SettlementSystem.StrHelper2[j]))
					{
						text3 = string.Format("{0}\n{1}", text3, SettlementSystem.StrHelper[j]);
					}
					else
					{
						text3 = string.Format("{0}\n{1}({2}%)", text3, SettlementSystem.StrHelper[j], SettlementSystem.StrHelper2[j]);
					}
				}
			}
			gameObject2.CustomSetActive(true);
			if (multiple == 0)
			{
				gameObject2.CustomSetActive(false);
			}
			else
			{
				gameObject2.GetComponentInChildren<Text>().set_text(string.Format("{0}", text));
			}
			CUICommonSystem.SetCommonTipsEvent(this._profitFormScript, gameObject2, text3, enUseableTipsPos.enTop);
		}

		private void SetGoldCoinProfit()
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return;
			}
			COMDT_ACNT_INFO acntInfo = Singleton<BattleStatistic>.GetInstance().acntInfo;
			if (acntInfo == null)
			{
				return;
			}
			GameObject gameObject = this._profitFormScript.m_formWidgets[2];
			Text component = gameObject.transform.FindChild("goldTotalGroup/GoldNum").GetComponent<Text>();
			component.set_text("+0");
			this._coinFrom = 0f;
			this._coinTo = acntInfo.dwPvpSettleCoin;
			this._coinTweenText = component;
			GameObject gameObject2 = gameObject.transform.FindChild("GoldMax").gameObject;
			gameObject2.CustomSetActive(acntInfo.bReachDailyLimit > 0);
			GameObject gameObject3 = gameObject.transform.FindChild("goldTotalGroup/goldGroup/DoubleCoin").gameObject;
			gameObject3.CustomSetActive(false);
			Transform transform = gameObject.transform.FindChild("goldTotalGroup/goldGroup/QQVipIcon");
			transform.gameObject.CustomSetActive(false);
			for (int i = 0; i < SettlementSystem.StrHelper.Length; i++)
			{
				SettlementSystem.StrHelper[i] = null;
			}
			COMDT_REWARD_MULTIPLE_DETAIL multiDetail = Singleton<BattleStatistic>.GetInstance().multiDetail;
			if (multiDetail == null)
			{
				return;
			}
			int multiple = CUseable.GetMultiple(acntInfo.dwPvpSettleBaseCoin, ref multiDetail, 11, -1);
			COMDT_MULTIPLE_DATA[] array = null;
			uint multipleInfo = CUseable.GetMultipleInfo(out array, ref multiDetail, 11, -1);
			string text = string.Empty;
			if (multiple > 0)
			{
				text += "+";
			}
			text += multiple.ToString();
			SettlementSystem.StrHelper[0] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_7", new string[]
			{
				text
			});
			if (array != null)
			{
				gameObject3.CustomSetActive(true);
				bool flag = false;
				bool flag2 = false;
				int num = 0;
				while ((long)num < (long)((ulong)multipleInfo))
				{
					if (array[num].iType == 2)
					{
						flag = true;
					}
					if (array[num].iType == 9)
					{
						flag2 = true;
					}
					num++;
				}
				int num2 = 0;
				while ((long)num2 < (long)((ulong)multipleInfo))
				{
					string text2 = string.Empty;
					if ((ulong)acntInfo.dwPvpSettleBaseCoin * (ulong)((long)array[num2].iValue) > 0uL)
					{
						text2 = "+";
					}
					byte bOperator = array[num2].bOperator;
					if (bOperator != 0)
					{
						if (bOperator != 1)
						{
							text2 += "0";
						}
						else
						{
							double num3 = acntInfo.dwPvpSettleBaseCoin * (double)array[num2].iValue / 10000.0;
							if (num3 > 0.0)
							{
								text2 += (int)(num3 + 0.9999);
							}
							else if (num3 < 0.0)
							{
								text2 += (int)(num3 - 0.9999);
							}
							else
							{
								text2 = "0";
							}
						}
					}
					else
					{
						text2 += array[num2].iValue;
					}
					SettlementSystem.StrHelper2[num2 + 1] = string.Empty;
					int num4 = array[num2].iValue / 100;
					if (num4 >= 0)
					{
						string[] strHelper = SettlementSystem.StrHelper2;
						int num5 = num2 + 1;
						string[] expr_315_cp_0 = strHelper;
						int expr_315_cp_1 = num5;
						expr_315_cp_0[expr_315_cp_1] += "+";
					}
					string[] strHelper2 = SettlementSystem.StrHelper2;
					int num6 = num2 + 1;
					string[] expr_338_cp_0 = strHelper2;
					int expr_338_cp_1 = num6;
					expr_338_cp_0[expr_338_cp_1] += num4;
					switch (array[num2].iType)
					{
					case 1:
						SettlementSystem.StrHelper[num2 + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_6", new string[]
						{
							text2
						});
						break;
					case 2:
					{
						if (masterRoleInfo.HasVip(16))
						{
							SettlementSystem.StrHelper[num2 + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_9", new string[]
							{
								text2
							});
						}
						else
						{
							SettlementSystem.StrHelper[num2 + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_3", new string[]
							{
								text2
							});
						}
						Transform transform2 = transform.FindChild("Icon");
						if (transform2 != null)
						{
							MonoSingleton<NobeSys>.GetInstance().SetMyQQVipHead(transform2.GetComponent<Image>());
							transform.FindChild("Text").GetComponent<Text>().set_text(string.Format("{0}", text2));
							transform.gameObject.CustomSetActive(true);
						}
						break;
					}
					case 3:
						SettlementSystem.StrHelper[num2 + 1] = string.Format(Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_10"), text2, masterRoleInfo.GetCoinWinCount(), Math.Ceiling((double)((float)masterRoleInfo.GetCoinExpireHours() / 24f)));
						break;
					case 4:
						SettlementSystem.StrHelper[num2 + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_2", new string[]
						{
							masterRoleInfo.dailyPvpCnt.ToString(),
							text2
						});
						break;
					case 5:
					{
						SettlementSystem.StrHelper[num2 + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_13", new string[]
						{
							text2
						});
						Transform transform3 = gameObject.transform.FindChild("goldTotalGroup/goldGroup/WXIcon");
						MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(transform3.gameObject, masterRoleInfo.m_privilegeType, ApolloPlatform.Wechat, false, false, string.Empty, string.Empty);
						transform3.FindChild("Text").GetComponent<Text>().set_text(string.Format("{0}", text2));
						break;
					}
					case 6:
						SettlementSystem.StrHelper[num2 + 1] = Singleton<CTextManager>.instance.GetText("Guild_Settlement_Guild_Coin_Plus_Tip2", new string[]
						{
							text2
						});
						break;
					case 7:
						SettlementSystem.StrHelper[num2 + 1] = Singleton<CTextManager>.instance.GetText("Daily_Quest_FirstVictoryName", new string[]
						{
							text2
						});
						break;
					case 8:
						SettlementSystem.StrHelper[num2 + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_16", new string[]
						{
							text2
						});
						break;
					case 9:
					{
						SettlementSystem.StrHelper[num2 + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_14", new string[]
						{
							text2
						});
						Transform transform4 = gameObject.transform.FindChild("goldTotalGroup/goldGroup/QQGameCenterIcon");
						if (!flag)
						{
							MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(transform4.gameObject, masterRoleInfo.m_privilegeType, ApolloPlatform.QQ, false, false, string.Empty, string.Empty);
							transform4.FindChild("Text").GetComponent<Text>().set_text(string.Format("{0}", text2));
						}
						else
						{
							transform4.gameObject.CustomSetActive(false);
						}
						break;
					}
					case 10:
					{
						SettlementSystem.StrHelper[num2 + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_21", new string[]
						{
							text2
						});
						Transform transform5 = gameObject.transform.FindChild("goldTotalGroup/goldGroup/GuestGameCenterIcon");
						if (!flag && !flag2)
						{
							MonoSingleton<NobeSys>.GetInstance().SetGameCenterVisible(transform5.gameObject, masterRoleInfo.m_privilegeType, ApolloPlatform.Guest, false, false, string.Empty, string.Empty);
							transform5.FindChild("Text").GetComponent<Text>().set_text(string.Format("{0}", text2));
						}
						else
						{
							transform5.gameObject.CustomSetActive(false);
						}
						break;
					}
					case 11:
						SettlementSystem.StrHelper[num2 + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_17", new string[]
						{
							text2
						});
						break;
					case 12:
						SettlementSystem.StrHelper[num2 + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_18", new string[]
						{
							text2
						});
						break;
					case 13:
						SettlementSystem.StrHelper[num2 + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_20", new string[]
						{
							text2
						});
						break;
					case 14:
						SettlementSystem.StrHelper[num2 + 1] = Singleton<CTextManager>.instance.GetText("Pvp_settle_Common_Tips_19", new string[]
						{
							text2
						});
						break;
					}
					num2++;
				}
			}
			string text3 = SettlementSystem.StrHelper[0];
			for (int j = 1; j < SettlementSystem.StrHelper.Length; j++)
			{
				if (!string.IsNullOrEmpty(SettlementSystem.StrHelper[j]))
				{
					if (array[j].iType == 7 || string.IsNullOrEmpty(SettlementSystem.StrHelper2[j]))
					{
						text3 = string.Format("{0}\n{1}", text3, SettlementSystem.StrHelper[j]);
					}
					else
					{
						text3 = string.Format("{0}\n{1}({2}%)", text3, SettlementSystem.StrHelper[j], SettlementSystem.StrHelper2[j]);
					}
				}
			}
			if (CSysDynamicBlock.bLobbyEntryBlocked)
			{
				gameObject3.CustomSetActive(false);
			}
			else
			{
				gameObject3.CustomSetActive(true);
				if (multiple == 0)
				{
					gameObject3.CustomSetActive(false);
				}
				else
				{
					gameObject3.GetComponentInChildren<Text>().set_text(string.Format("{0}", text));
				}
				CUICommonSystem.SetCommonTipsEvent(this._profitFormScript, gameObject3, text3, enUseableTipsPos.enTop);
			}
		}

		private void SetSymbolCoinProfit()
		{
			if (Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo() == null)
			{
				return;
			}
			COMDT_ACNT_INFO acntInfo = Singleton<BattleStatistic>.GetInstance().acntInfo;
			if (acntInfo == null)
			{
				return;
			}
			GameObject gameObject = this._profitFormScript.m_formWidgets[10];
			Text component = gameObject.transform.FindChild("CoinNum").GetComponent<Text>();
			component.set_text("+0");
			this._symbolCoinFrom = 0f;
			this._symbolCoinTo = acntInfo.dwSymbolCoin;
			this._symbolCoinTweenText = component;
			GameObject gameObject2 = gameObject.transform.FindChild("CoinMax").gameObject;
			gameObject2.CustomSetActive(acntInfo.bReachDailyLimit > 0);
		}

		private void SetMapInfo()
		{
			GameObject gameObject = this._profitFormScript.m_formWidgets[6];
			gameObject.CustomSetActive(false);
			Text component = gameObject.transform.FindChild("GameType").GetComponent<Text>();
			Text component2 = gameObject.transform.FindChild("MapName").GetComponent<Text>();
			string text = Singleton<CTextManager>.instance.GetText("Battle_Settle_Game_Type_Single");
			string text2 = string.Empty;
			SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
			if (curLvelContext == null)
			{
				return;
			}
			uint mapID = (uint)curLvelContext.m_mapID;
			if (curLvelContext.IsMobaMode())
			{
				gameObject.CustomSetActive(true);
				text2 = curLvelContext.m_levelName;
				if (curLvelContext.IsGameTypeRewardMatch())
				{
					text = curLvelContext.m_SecondName;
				}
				else
				{
					text = Singleton<CTextManager>.instance.GetText(string.Format("Battle_Settle_Game_Type{0}", curLvelContext.m_pvpPlayerNum / 2));
				}
			}
			component.set_text(text);
			component2.set_text(text2);
		}

		private void SetProficiencyInfo()
		{
			GameObject gameObject = this._profitFormScript.m_formWidgets[3];
			gameObject.CustomSetActive(false);
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return;
			}
			PlayerKDA playerKDA = null;
			if (Singleton<BattleLogic>.GetInstance().battleStat != null && Singleton<BattleLogic>.GetInstance().battleStat.m_playerKDAStat != null)
			{
				playerKDA = Singleton<BattleLogic>.GetInstance().battleStat.m_playerKDAStat.GetHostKDA();
			}
			if (playerKDA == null)
			{
				return;
			}
			RectTransform component = gameObject.transform.FindChild("ProficiencySliderBg/BaseProficiencySlider").gameObject.GetComponent<RectTransform>();
			RectTransform component2 = gameObject.transform.FindChild("ProficiencySliderBg/AddProficiencySlider").gameObject.GetComponent<RectTransform>();
			Text component3 = gameObject.transform.FindChild("HeroName").GetComponent<Text>();
			Text component4 = gameObject.transform.FindChild("ProficiencyLv").GetComponent<Text>();
			Text component5 = gameObject.transform.FindChild("ProficiencyTxt").GetComponent<Text>();
			Text component6 = gameObject.transform.FindChild("AddProficiencyTxt").GetComponent<Text>();
			Image component7 = gameObject.transform.FindChild("HeroInfo/HeroHeadIcon").GetComponent<Image>();
			GameObject gameObject2 = gameObject.transform.FindChild("heroProficiencyImg").gameObject;
			component6.set_text(null);
			component3.set_text(string.Empty);
			ListView<HeroKDA>.Enumerator enumerator = playerKDA.GetEnumerator();
			uint num = 0u;
			while (enumerator.MoveNext())
			{
				HeroKDA current = enumerator.Current;
				if (current != null)
				{
					num = (uint)current.HeroId;
					uint skinId = current.SkinId;
					break;
				}
			}
			CHeroInfo cHeroInfo;
			masterRoleInfo.GetHeroInfo(num, out cHeroInfo, false);
			ActorMeta actorMeta = default(ActorMeta);
			ActorMeta actorMeta2 = actorMeta;
			actorMeta2.PlayerId = playerKDA.PlayerId;
			actorMeta2.ConfigId = (int)num;
			actorMeta = actorMeta2;
			ActorStaticData actorStaticData = default(ActorStaticData);
			IGameActorDataProvider actorDataProvider = Singleton<ActorDataCenter>.instance.GetActorDataProvider(GameActorDataProviderType.StaticLobbyDataProvider);
			actorDataProvider.GetActorStaticData(ref actorMeta, ref actorStaticData);
			COMDT_SETTLE_HERO_RESULT_INFO heroSettleInfo = SettlementSystem.GetHeroSettleInfo(num);
			if (heroSettleInfo == null)
			{
				return;
			}
			ResHeroProficiency heroProficiency = CHeroInfo.GetHeroProficiency(actorStaticData.TheHeroOnlyInfo.HeroCapability, (int)heroSettleInfo.dwProficiencyLv);
			if (heroProficiency == null)
			{
				return;
			}
			if (!Singleton<BattleLogic>.GetInstance().GetCurLvelContext().IsMobaMode() || cHeroInfo == null)
			{
				return;
			}
			gameObject.CustomSetActive(true);
			component3.set_text(actorStaticData.TheResInfo.Name);
			component6.set_text((heroSettleInfo.dwSettleProficiency > 0u) ? string.Format("+{0}", heroSettleInfo.dwSettleProficiency) : null);
			if (heroSettleInfo.dwSettleProficiency == 0u)
			{
			}
			component7.SetSprite(string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Icon_Dir, CSkinInfo.GetHeroSkinPic(num, 0u)), gameObject.GetComponent<CUIFormScript>(), true, false, false, false);
			int maxProficiency = CHeroInfo.GetMaxProficiency();
			float num2;
			float num3;
			if ((long)maxProficiency == (long)((ulong)heroSettleInfo.dwProficiencyLv))
			{
				num2 = 1f;
				num3 = 0f;
				component5.set_text("MAX");
			}
			else
			{
				int num4 = (int)(heroSettleInfo.dwProficiency - heroSettleInfo.dwSettleProficiency);
				num2 = Mathf.Max(0f, (float)num4 / heroProficiency.dwTopPoint);
				num3 = Mathf.Max(0f, ((num4 < 0) ? heroSettleInfo.dwProficiency : heroSettleInfo.dwSettleProficiency) / heroProficiency.dwTopPoint);
				component5.set_text(string.Format("{0} / {1}", heroSettleInfo.dwProficiency, heroProficiency.dwTopPoint));
			}
			component4.set_text(SettlementSystem.GetProficiencyLvTxt(actorStaticData.TheHeroOnlyInfo.HeroCapability, heroSettleInfo.dwProficiencyLv));
			CUICommonSystem.SetHeroProficiencyIconImage(this._profitFormScript, gameObject2, (int)heroSettleInfo.dwProficiencyLv);
			component2.sizeDelta = new Vector2((num2 + num3) * 159.45f, component2.sizeDelta.y);
			component.sizeDelta = new Vector2(num2 * 159.45f, component.sizeDelta.y);
			component2.gameObject.CustomSetActive(num3 > 0f);
		}

		private void SetVictoryTipsBtnInfo(CUIFormScript form)
		{
			PlayerKDA playerKDA = null;
			if (Singleton<BattleLogic>.GetInstance().battleStat != null && Singleton<BattleLogic>.GetInstance().battleStat.m_playerKDAStat != null)
			{
				playerKDA = Singleton<BattleLogic>.GetInstance().battleStat.m_playerKDAStat.GetHostKDA();
			}
			if (playerKDA == null)
			{
				return;
			}
			ListView<HeroKDA>.Enumerator enumerator = playerKDA.GetEnumerator();
			uint key = 0u;
			while (enumerator.MoveNext())
			{
				HeroKDA current = enumerator.Current;
				if (current != null)
				{
					key = (uint)current.HeroId;
					break;
				}
			}
			if (form != null)
			{
				GameObject widget = form.GetWidget(24);
				if (CBattleGuideManager.EnableHeroVictoryTips() && !CSysDynamicBlock.bLobbyEntryBlocked)
				{
					widget.CustomSetActive(true);
					Transform transform = widget.transform;
					CUIEventScript component = transform.FindChild("Btn").GetComponent<CUIEventScript>();
					ulong num = 0uL;
					CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
					if (masterRoleInfo != null)
					{
						num = masterRoleInfo.playerUllUID;
					}
					string platformArea = CUICommonSystem.GetPlatformArea();
					component.m_onClickEventParams.tagStr = Singleton<CTextManager>.GetInstance().GetText("WinTrick_Url_Result", new string[]
					{
						key.ToString(),
						key.ToString(),
						MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString(),
						num.ToString(),
						platformArea.ToString()
					});
					if (!this._win && !MonoSingleton<NewbieGuideManager>.instance.isNewbieGuiding)
					{
						int num2 = PlayerPrefs.GetInt(SettlementSystem.PlayerWinTimesStr, 0);
						num2++;
						uint globeValue = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_SHOW_WINTRICKTIPS_PVPLOSE_TIMES);
						if ((long)num2 >= (long)((ulong)globeValue))
						{
							transform.FindChild("Panel_Guide").gameObject.CustomSetActive(true);
							ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(key);
							string text;
							if (dataByKey != null)
							{
								text = dataByKey.szName;
							}
							else
							{
								text = Singleton<CTextManager>.GetInstance().GetText("WinTrick_Tips_DefaultHeroName");
							}
							transform.FindChild("Panel_Guide/Text").GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("WinTrick_Tips_text", new string[]
							{
								text
							}));
							PlayerPrefs.SetInt(SettlementSystem.PlayerWinTimesStr, 0);
						}
						else
						{
							PlayerPrefs.SetInt(SettlementSystem.PlayerWinTimesStr, num2);
							transform.FindChild("Panel_Guide").gameObject.CustomSetActive(false);
						}
					}
					else
					{
						PlayerPrefs.SetInt(SettlementSystem.PlayerWinTimesStr, 0);
						transform.FindChild("Panel_Guide").gameObject.CustomSetActive(false);
					}
				}
				else
				{
					widget.CustomSetActive(false);
				}
				if (this._neutral)
				{
					widget.CustomSetActive(false);
				}
			}
		}

		public static COMDT_SETTLE_HERO_RESULT_INFO GetHeroSettleInfo(uint heroId)
		{
			COMDT_SETTLE_HERO_RESULT_DETAIL heroSettleInfo = Singleton<BattleStatistic>.GetInstance().heroSettleInfo;
			if (heroSettleInfo == null)
			{
				return null;
			}
			for (int i = 0; i < (int)heroSettleInfo.bNum; i++)
			{
				if (heroSettleInfo.astHeroList[i] != null && heroSettleInfo.astHeroList[i].dwHeroConfID == heroId)
				{
					return heroSettleInfo.astHeroList[i];
				}
			}
			return null;
		}

		public static string GetProficiencyLvTxt(int heroType, uint level)
		{
			ResHeroProficiency heroProficiency = CHeroInfo.GetHeroProficiency(heroType, (int)level);
			return (heroProficiency != null) ? Utility.UTF8Convert(heroProficiency.szTitle) : string.Empty;
		}

		private void SetGuildInfo()
		{
			GameObject gameObject = this._profitFormScript.m_formWidgets[4];
			gameObject.CustomSetActive(false);
			if (Singleton<BattleStatistic>.GetInstance().acntInfo == null)
			{
				return;
			}
			if (!Singleton<CGuildSystem>.GetInstance().IsInNormalGuild())
			{
				return;
			}
			if (!this.IsGuildProfitGameType())
			{
				return;
			}
			GuildMemInfo playerGuildMemberInfo = CGuildHelper.GetPlayerGuildMemberInfo();
			if (playerGuildMemberInfo == null)
			{
				return;
			}
			GameObject obj = this._profitFormScript.m_formWidgets[7];
			Text component = gameObject.transform.FindChild("GuildPointTxt").GetComponent<Text>();
			Transform transform = gameObject.transform.FindChild("ImageIcon");
			Transform transform2 = gameObject.transform.FindChild("GuildText");
			uint num = playerGuildMemberInfo.RankInfo.byGameRankPoint - CGuildSystem.s_lastByGameRankpoint;
			DebugHelper.Assert(playerGuildMemberInfo.RankInfo.byGameRankPoint >= CGuildSystem.s_lastByGameRankpoint, "byGameRankPoint={0}, s_lastByGameRankpoint={1}", new object[]
			{
				playerGuildMemberInfo.RankInfo.byGameRankPoint,
				CGuildSystem.s_lastByGameRankpoint
			});
			if (transform != null)
			{
				transform.gameObject.CustomSetActive(num > 0u);
			}
			if (transform2 != null)
			{
				transform2.gameObject.CustomSetActive(num > 0u);
			}
			if (num > 0u)
			{
				obj.CustomSetActive(false);
				component.set_text(num.ToString(CultureInfo.get_InvariantCulture()));
			}
			else
			{
				obj.CustomSetActive(CGuildSystem.s_lastByGameRankpoint >= CGuildSystem.s_rankpointProfitMax);
				component.set_text(string.Empty);
			}
			CGuildSystem.s_lastByGameRankpoint = playerGuildMemberInfo.RankInfo.byGameRankPoint;
			CUICommonSystem.SetCommonTipsEvent(this._profitFormScript, gameObject, Singleton<CTextManager>.GetInstance().GetText("Guild_Settlement_Guild_Info_Tip"), enUseableTipsPos.enTop);
			gameObject.CustomSetActive(true);
		}

		private bool IsGuildProfitGameType()
		{
			SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
			return curLvelContext != null && curLvelContext.IsGuildProfitGameType();
		}

		private void SetLadderInfo()
		{
			GameObject gameObject = this._profitFormScript.m_formWidgets[5];
			gameObject.CustomSetActive(false);
			this._isLadderMatch = false;
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			if (curLvelContext == null)
			{
				return;
			}
			if (!curLvelContext.IsGameTypeLadder())
			{
				return;
			}
			COMDT_RANK_SETTLE_INFO rankInfo = Singleton<BattleStatistic>.GetInstance().rankInfo;
			if (rankInfo == null)
			{
				return;
			}
			gameObject.CustomSetActive(true);
			this._isLadderMatch = true;
			Transform transform = gameObject.transform.FindChild(string.Format("RankLevelName", new object[0]));
			if (transform != null)
			{
				Text component = transform.gameObject.GetComponent<Text>();
				component.set_text(CLadderView.GetRankName(rankInfo.bNowShowGrade, Singleton<CRoleInfoManager>.GetInstance().GetSelfRankClass()));
			}
			if (gameObject.transform.FindChild(string.Format("WangZheXingTxt", new object[0])) == null)
			{
				return;
			}
			Text component2 = gameObject.transform.FindChild(string.Format("WangZheXingTxt", new object[0])).gameObject.GetComponent<Text>();
			if ((int)CLadderSystem.GetGradeDataByShowGrade((int)rankInfo.bNowShowGrade).bLogicGrade == CLadderSystem.MAX_RANK_LEVEL)
			{
				Transform transform2 = gameObject.transform.FindChild(string.Format("XingGrid/ImgScore{0}", 1));
				if (transform2 != null)
				{
					transform2.gameObject.CustomSetActive(true);
				}
				component2.gameObject.CustomSetActive(true);
				component2.set_text(string.Format("X{0}", rankInfo.dwNowScore));
			}
			else
			{
				component2.gameObject.CustomSetActive(false);
				int num = 1;
				while ((long)num <= (long)((ulong)rankInfo.dwNowScore))
				{
					Transform transform3 = gameObject.transform.FindChild(string.Format("XingGrid/ImgScore{0}", num));
					if (transform3 != null)
					{
						transform3.gameObject.CustomSetActive(true);
					}
					num++;
				}
			}
			this._profitFormScript.m_formWidgets[6].gameObject.CustomSetActive(false);
		}

		private void DoCoinAndExpTween()
		{
			try
			{
				if (this._coinTweenText != null && this._coinTweenText.gameObject != null)
				{
					this._coinLtd = LeanTween.value(this._coinTweenText.gameObject, delegate(float value)
					{
						if (this._coinTweenText == null || this._coinTweenText.gameObject == null)
						{
							return;
						}
						this._coinTweenText.set_text(string.Format("+{0}", value.ToString("N0")));
						if (value >= this._coinTo)
						{
							this.DoCoinTweenEnd();
						}
					}, this._coinFrom, this._coinTo, 2f);
				}
				if (this._symbolCoinTweenText != null && this._symbolCoinTweenText.gameObject != null)
				{
					this._symbolCoinLtd = LeanTween.value(this._symbolCoinTweenText.gameObject, delegate(float value)
					{
						if (this._symbolCoinTweenText == null || this._symbolCoinTweenText.gameObject == null)
						{
							return;
						}
						this._symbolCoinTweenText.set_text(string.Format("+{0}", value.ToString("N0")));
						if (value >= this._symbolCoinTo)
						{
							this.DoSymbolCoinTweenEnd();
						}
					}, this._symbolCoinFrom, this._symbolCoinTo, 2f);
				}
				if (this._expTweenRect != null && this._expTweenRect.gameObject != null)
				{
					this._expLtd = LeanTween.value(this._expTweenRect.gameObject, delegate(float value)
					{
						if (this._expTweenRect == null || this._expTweenRect.gameObject == null)
						{
							return;
						}
						this._expTweenRect.sizeDelta = new Vector2(value * 220.3f, this._expTweenRect.sizeDelta.y);
						if (value >= this._expTo)
						{
							this.DoExpTweenEnd();
						}
					}, this._expFrom, this._expTo, 2f);
				}
			}
			catch (Exception ex)
			{
				DebugHelper.Assert(false, "Exceptin in DoCoinAndExpTween, {0}", new object[]
				{
					ex.get_Message()
				});
			}
		}

		public void DoCoinTweenEnd()
		{
			if (this._coinLtd == null || this._coinTweenText == null)
			{
				return;
			}
			this._coinTweenText.set_text(string.Format("+{0}", this._coinTo.ToString("N0")));
			COMDT_ACNT_INFO acntInfo = Singleton<BattleStatistic>.GetInstance().acntInfo;
			if (Singleton<BattleStatistic>.GetInstance().multiDetail != null)
			{
				CUICommonSystem.AppendMultipleText(this._coinTweenText, CUseable.GetMultiple(acntInfo.dwPvpSettleBaseCoin, ref Singleton<BattleStatistic>.GetInstance().multiDetail, 0, -1));
			}
			this._coinLtd.cancel();
			this._coinLtd = null;
			this._coinTweenText = null;
		}

		public void DoSymbolCoinTweenEnd()
		{
			if (this._symbolCoinLtd == null || this._symbolCoinTweenText == null)
			{
				return;
			}
			this._symbolCoinTweenText.set_text(string.Format("+{0}", this._symbolCoinTo.ToString("N0")));
			COMDT_ACNT_INFO acntInfo = Singleton<BattleStatistic>.GetInstance().acntInfo;
			if (Singleton<BattleStatistic>.GetInstance().multiDetail != null)
			{
				CUICommonSystem.AppendMultipleText(this._symbolCoinTweenText, CUseable.GetMultiple(acntInfo.dwPvpSettleBaseCoin, ref Singleton<BattleStatistic>.GetInstance().multiDetail, 14, -1));
			}
			this._symbolCoinLtd.cancel();
			this._symbolCoinLtd = null;
			this._symbolCoinTweenText = null;
		}

		private void DoExpTweenEnd()
		{
			if (this._expTweenRect != null && this._expLtd != null)
			{
				this._expTweenRect.sizeDelta = new Vector2(this._expTo * 220.3f, this._expTweenRect.sizeDelta.y);
				this._expLtd.cancel();
				this._expLtd = null;
				this._expTweenRect = null;
			}
			if (SettlementSystem._lvUpGrade > 1u)
			{
				CUIEvent cUIEvent = new CUIEvent();
				cUIEvent.m_eventID = enUIEventID.Settle_OpenLvlUp;
				cUIEvent.m_eventParams.tag = (int)(SettlementSystem._lvUpGrade - 1u);
				cUIEvent.m_eventParams.tag2 = (int)SettlementSystem._lvUpGrade;
				CUIEvent uiEvent = cUIEvent;
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uiEvent);
			}
			SettlementSystem._lvUpGrade = 0u;
		}

		public bool IsInSettlementState()
		{
			return Singleton<CUIManager>.GetInstance().GetForm(SettlementSystem.SettlementFormName) != null;
		}

		private void ClearSendRedBag()
		{
			this.m_bMvp = 0;
			this.m_bLegaendary = 0;
			this.m_bPENTAKILL = 0;
			this.m_bQUATARYKIL = 0;
			this.m_bTRIPLEKILL = 0;
			this.m_bWin = 0;
			this.m_bSendRedBag = false;
			this.m_bSendPandoraMsg = false;
			this.m_GameMode = COM_GAME_TYPE.COM_SINGLE_GAME_OF_COMBAT;
			this.m_TotalPlayerNum = 0;
			this.m_bIsWarmBattle = false;
			this.m_bHaveCpu = false;
			this.m_selectTypeHero = enSelectType.enNull;
			this.m_mapID = 0;
			this.m_bLuanDou = false;
			this.m_bFireHole = false;
		}

		public void ShowSettlementPanel(bool neutralShow = false)
		{
			if (Singleton<CUIManager>.GetInstance().GetForm(SettlementSystem.SettlementFormName) != null)
			{
				return;
			}
			this._neutral = neutralShow;
			if (this._neutral)
			{
				this._win = (Singleton<BattleStatistic>.GetInstance().iBattleResult == 1);
			}
			this.ClearSendRedBag();
			if (!neutralShow)
			{
				SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
				if (curLvelContext.IsRedBagMode())
				{
					PlayerKDA hostKDA = Singleton<BattleLogic>.GetInstance().battleStat.m_playerKDAStat.GetHostKDA();
					if (hostKDA != null)
					{
						if (this._win)
						{
							this.m_bWin = 1;
						}
						uint mvpPlayer = Singleton<BattleStatistic>.instance.GetMvpPlayer(hostKDA.PlayerCamp, this._win);
						if (mvpPlayer != 0u && mvpPlayer == hostKDA.PlayerId)
						{
							this.m_bMvp = 1;
						}
						ListView<HeroKDA>.Enumerator enumerator = hostKDA.GetEnumerator();
						while (enumerator.MoveNext())
						{
							HeroKDA current = enumerator.Current;
							if (current != null)
							{
								if (current.LegendaryNum > 0)
								{
									this.m_bLegaendary = 1;
								}
								if (current.PentaKillNum > 0)
								{
									this.m_bPENTAKILL = 1;
								}
								if (current.QuataryKillNum > 0)
								{
									this.m_bQUATARYKIL = 1;
								}
								if (current.TripleKillNum > 0)
								{
									this.m_bTRIPLEKILL = 1;
								}
							}
						}
						this.m_GameMode = curLvelContext.GetGameType();
						this.m_bSendRedBag = true;
					}
				}
				PlayerKDA hostKDA2 = Singleton<BattleLogic>.GetInstance().battleStat.m_playerKDAStat.GetHostKDA();
				if (hostKDA2 != null)
				{
					if (this._win)
					{
						this.m_bWin = 1;
					}
					uint mvpPlayer2 = Singleton<BattleStatistic>.instance.GetMvpPlayer(hostKDA2.PlayerCamp, this._win);
					if (mvpPlayer2 != 0u && mvpPlayer2 == hostKDA2.PlayerId)
					{
						this.m_bMvp = 1;
					}
					ListView<HeroKDA>.Enumerator enumerator2 = hostKDA2.GetEnumerator();
					while (enumerator2.MoveNext())
					{
						HeroKDA current2 = enumerator2.Current;
						if (current2 != null)
						{
							if (current2.LegendaryNum > 0)
							{
								this.m_bLegaendary = 1;
							}
							if (current2.PentaKillNum > 0)
							{
								this.m_bPENTAKILL = 1;
							}
							if (current2.QuataryKillNum > 0)
							{
								this.m_bQUATARYKIL = 1;
							}
							if (current2.TripleKillNum > 0)
							{
								this.m_bTRIPLEKILL = 1;
							}
						}
					}
					this.m_GameMode = curLvelContext.GetGameType();
					this.m_TotalPlayerNum = curLvelContext.m_pvpPlayerNum;
					this.m_bIsWarmBattle = curLvelContext.m_isWarmBattle;
					this.m_bHaveCpu = Singleton<GamePlayerCenter>.GetInstance().IsHostPlayerHasCpuEnemy();
					this.m_selectTypeHero = curLvelContext.GetSelectHeroType();
					this.m_mapID = curLvelContext.m_mapID;
					this.m_bLuanDou = curLvelContext.IsLuanDouPlayMode();
					this.m_bFireHole = curLvelContext.IsFireHolePlayMode();
					this.m_bSendPandoraMsg = true;
				}
			}
			this._settleFormScript = Singleton<CUIManager>.GetInstance().OpenForm(SettlementSystem.SettlementFormName, false, true);
			this._settleFormScript.m_formWidgets[2].CustomSetActive(true);
			this._settleFormScript.m_formWidgets[16].CustomSetActive(true);
			this._settleFormScript.m_formWidgets[1].CustomSetActive(false);
			this._settleFormScript.m_formWidgets[4].CustomSetActive(true);
			this._settleFormScript.m_formWidgets[5].CustomSetActive(true);
			this._settleFormScript.m_formWidgets[6].CustomSetActive(true);
			this._settleFormScript.m_formWidgets[15].GetComponent<Text>().set_text(this._duration);
			this._settleFormScript.m_formWidgets[15].CustomSetActive(true);
			this._settleFormScript.m_formWidgets[21].GetComponent<Text>().set_text(this._startTime);
			this._settleFormScript.m_formWidgets[21].CustomSetActive(true);
			this._settleFormScript.m_formWidgets[11].CustomSetActive(false);
			this._settleFormScript.m_formWidgets[12].CustomSetActive(false);
			this._settleFormScript.m_formWidgets[20].CustomSetActive(false);
			this._settleFormScript.m_formWidgets[19].CustomSetActive(false);
			this._settleFormScript.m_formWidgets[18].CustomSetActive(true);
			this._settleFormScript.m_formWidgets[17].CustomSetActive(true);
			this._settleFormScript.m_formWidgets[22].CustomSetActive(!this._neutral);
			this._settleFormScript.m_formWidgets[27].CustomSetActive(!this._neutral);
			this._settleFormScript.m_formWidgets[9].CustomSetActive(false);
			this._settleFormScript.m_formWidgets[10].CustomSetActive(false);
			this._settleFormScript.m_formWidgets[25].CustomSetActive(false);
			this._leftListScript = this._settleFormScript.m_formWidgets[7].GetComponent<CUIListScript>();
			this._rightListScript = this._settleFormScript.m_formWidgets[8].GetComponent<CUIListScript>();
			this.SetSettlementTitle();
			this.SetSettlementButton();
			this.SetPlayerSettlement();
			this.SetCreditSettlement();
			this.SetVictoryTipsBtnInfo(this._settleFormScript);
			GameObject gameObject = this._settleFormScript.m_formWidgets[23];
			if (gameObject != null)
			{
				Singleton<CReplayKitSys>.GetInstance().InitReplayKitRecordBtn(gameObject.transform);
			}
			if (this.m_ShareDataBtn && this.m_ShareDataBtn.activeSelf)
			{
				this.InitShareDataBtn(this._settleFormScript);
			}
			MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.pvpFin, new uint[]
			{
				this._win ? 1u : 2u,
				(uint)(this.playerNum / 2)
			});
			this._settleFormScript.m_formWidgets[26].CustomSetActive(false);
		}

		private void SetSettlementTitle()
		{
			if (this._settleFormScript == null)
			{
				return;
			}
			GameObject gameObject = this._settleFormScript.m_formWidgets[0];
			gameObject.transform.FindChild("Mid").gameObject.CustomSetActive(this._neutral);
			gameObject.transform.FindChild("Win").gameObject.CustomSetActive(this._win && !this._neutral);
			gameObject.transform.FindChild("Lose").gameObject.CustomSetActive(!this._win && !this._neutral);
			GameObject p = this._settleFormScript.m_formWidgets[4];
			Color color = (this._win && !this._neutral) ? Color.white : new Color(1f, 1f, 1f, 0.75f);
			Utility.GetComponetInChild<Image>(p, "BgRed").set_color(color);
			Utility.GetComponetInChild<Image>(p, "BgBlue").set_color(color);
			Utility.FindChild(p, "LightBlue1").CustomSetActive(this._win && !this._neutral);
			Utility.FindChild(p, "LightBlue2").CustomSetActive(this._win && !this._neutral);
			Utility.FindChild(p, "LightRed1").CustomSetActive(this._win && !this._neutral);
		}

		private void SetSettlementButton()
		{
			GameObject gameObject = this._settleFormScript.m_formWidgets[1];
			gameObject.transform.FindChild("BtnBack").gameObject.CustomSetActive(true);
			gameObject.transform.FindChild("BtnAgain").gameObject.CustomSetActive(!this._neutral);
			gameObject.transform.FindChild("BtnSaveReplay").gameObject.CustomSetActive(Singleton<GameReplayModule>.GetInstance().HasRecord && (Singleton<WatchController>.GetInstance().FightOverJust || !Singleton<WatchController>.GetInstance().IsWatching));
			gameObject.transform.FindChild("ButtonShare").gameObject.CustomSetActive(this._win && !this._neutral && !CSysDynamicBlock.bSocialBlocked);
			gameObject.transform.FindChild("ButtonShit").gameObject.CustomSetActive(!this._win && !this._neutral && !CSysDynamicBlock.bSocialBlocked);
			this.m_ShareDataBtn = gameObject.transform.FindChild("ButtonShareData").gameObject;
			this.m_ShareDataBtn.CustomSetActive(!this._neutral && !CSysDynamicBlock.bSocialBlocked);
		}

		public void SetLastMatchDuration(string duration, string startTime, uint startTimeInt)
		{
			this._duration = duration;
			this._startTime = startTime;
			this._startTimeInt = startTimeInt;
		}

		private void SetPlayerSettlement()
		{
			CUIListScript component = this._settleFormScript.m_formWidgets[7].GetComponent<CUIListScript>();
			CUIListScript component2 = this._settleFormScript.m_formWidgets[8].GetComponent<CUIListScript>();
			int count = Singleton<GamePlayerCenter>.GetInstance().GetAllCampPlayers(COM_PLAYERCAMP.COM_PLAYERCAMP_1).get_Count();
			int count2 = Singleton<GamePlayerCenter>.GetInstance().GetAllCampPlayers(COM_PLAYERCAMP.COM_PLAYERCAMP_2).get_Count();
			this.playerNum = count + count2;
			component.SetElementAmount(count);
			component2.SetElementAmount(count2);
			this._curLeftIndex = 0;
			this._curRightIndex = 0;
			CPlayerKDAStat playerKDAStat = Singleton<BattleLogic>.GetInstance().battleStat.m_playerKDAStat;
			if (playerKDAStat == null)
			{
				return;
			}
			DictionaryView<uint, PlayerKDA>.Enumerator enumerator = playerKDAStat.GetEnumerator();
			this._camp1TotalDamage = 0u;
			this._camp1TotalTakenDamage = 0u;
			this._camp1TotalToHeroDamage = 0u;
			this._camp2TotalDamage = 0u;
			this._camp2TotalTakenDamage = 0u;
			this._camp2TotalToHeroDamage = 0u;
			this._camp1TotalKill = 0u;
			this._camp2TotalKill = 0u;
			this._myCamp = Singleton<GamePlayerCenter>.GetInstance().hostPlayerCamp;
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
				PlayerKDA value = current.get_Value();
				this.CollectPlayerKda(value);
			}
			GameObject gameObject = this._settleFormScript.m_formWidgets[4];
			gameObject.transform.FindChild("LeftTotalKill").gameObject.GetComponent<Text>().set_text(this._camp1TotalKill.ToString(CultureInfo.get_InvariantCulture()));
			gameObject.transform.FindChild("RightTotalKill").gameObject.GetComponent<Text>().set_text(this._camp2TotalKill.ToString(CultureInfo.get_InvariantCulture()));
			DictionaryView<uint, PlayerKDA>.Enumerator enumerator2 = playerKDAStat.GetEnumerator();
			this.HostPlayerHeroId = this.GetHostPlayerHeroId(enumerator2);
			enumerator2.Reset();
			while (enumerator2.MoveNext())
			{
				KeyValuePair<uint, PlayerKDA> current2 = enumerator2.Current;
				PlayerKDA value2 = current2.get_Value();
				this.UpdatePlayerKda(value2);
			}
		}

		private void SetCreditSettlement()
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return;
			}
			COMDT_ACNT_INFO acntInfo = Singleton<BattleStatistic>.GetInstance().acntInfo;
			if (acntInfo == null)
			{
				return;
			}
			Text componetInChild = Utility.GetComponetInChild<Text>(this._settleFormScript.m_formWidgets[22], "Text");
			if (acntInfo.iSettleCreditValue > 0)
			{
				componetInChild.set_text(Singleton<CTextManager>.instance.GetText("Credit_Change_Tips_1", new string[]
				{
					masterRoleInfo.creditScore.ToString(),
					acntInfo.iSettleCreditValue.ToString()
				}));
			}
			else if (acntInfo.iSettleCreditValue < 0)
			{
				componetInChild.set_text(Singleton<CTextManager>.instance.GetText("Credit_Change_Tips_2", new string[]
				{
					masterRoleInfo.creditScore.ToString(),
					acntInfo.iSettleCreditValue.ToString()
				}));
			}
			else
			{
				componetInChild.set_text(masterRoleInfo.creditScore.ToString());
			}
		}

		private void CollectPlayerKda(PlayerKDA kda)
		{
			ListView<HeroKDA>.Enumerator enumerator = kda.GetEnumerator();
			while (enumerator.MoveNext())
			{
				HeroKDA current = enumerator.Current;
				if (current != null)
				{
					COM_PLAYERCAMP playerCamp = kda.PlayerCamp;
					if (playerCamp != COM_PLAYERCAMP.COM_PLAYERCAMP_1)
					{
						if (playerCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_2)
						{
							this._camp2TotalKill += (uint)current.numKill;
							this._camp2TotalDamage += (uint)current.hurtToEnemy;
							this._camp2TotalTakenDamage += (uint)current.hurtTakenByEnemy;
							this._camp2TotalToHeroDamage += (uint)current.hurtToHero;
						}
					}
					else
					{
						this._camp1TotalKill += (uint)current.numKill;
						this._camp1TotalDamage += (uint)current.hurtToEnemy;
						this._camp1TotalTakenDamage += (uint)current.hurtTakenByEnemy;
						this._camp1TotalToHeroDamage += (uint)current.hurtToHero;
					}
					break;
				}
			}
		}

		private void ImpSwitchAddFriendReportLaHeiDianZan(SettlementSystem.ShowBtnType btnType)
		{
			if (this._settleFormScript == null)
			{
				return;
			}
			Transform transform = this._settleFormScript.m_formWidgets[9].transform.FindChild("light");
			if (transform != null)
			{
				transform.gameObject.CustomSetActive(btnType == SettlementSystem.ShowBtnType.AddFriend && !this._neutral);
			}
			transform = this._settleFormScript.m_formWidgets[10].transform.FindChild("light");
			if (transform != null)
			{
				transform.gameObject.CustomSetActive(btnType == SettlementSystem.ShowBtnType.Report && !this._neutral);
			}
			transform = this._settleFormScript.m_formWidgets[25].transform.FindChild("light");
			if (transform != null)
			{
				transform.gameObject.CustomSetActive(btnType == SettlementSystem.ShowBtnType.LaHeiDianZan && !this._neutral);
			}
			if (this._leftListScript != null)
			{
				int elementAmount = this._leftListScript.GetElementAmount();
				for (int i = 0; i < elementAmount; i++)
				{
					CUIListElementScript elemenet = this._leftListScript.GetElemenet(i);
					SettlementHelper component = elemenet.gameObject.GetComponent<SettlementHelper>();
					component.AddFriendRoot.CustomSetActive(btnType == SettlementSystem.ShowBtnType.AddFriend && !this._neutral);
					component.ReportRoot.CustomSetActive(btnType == SettlementSystem.ShowBtnType.Report && !this._neutral);
					component.DianZanLaHeiRoot.CustomSetActive(btnType == SettlementSystem.ShowBtnType.LaHeiDianZan && !this._neutral);
				}
			}
			if (this._rightListScript != null)
			{
				int elementAmount2 = this._rightListScript.GetElementAmount();
				for (int j = 0; j < elementAmount2; j++)
				{
					CUIListElementScript elemenet2 = this._rightListScript.GetElemenet(j);
					SettlementHelper component2 = elemenet2.gameObject.GetComponent<SettlementHelper>();
					component2.AddFriendRoot.CustomSetActive(btnType == SettlementSystem.ShowBtnType.AddFriend && !this._neutral);
					component2.ReportRoot.CustomSetActive(btnType == SettlementSystem.ShowBtnType.Report && !this._neutral);
					component2.DianZanLaHeiRoot.CustomSetActive(btnType == SettlementSystem.ShowBtnType.LaHeiDianZan && !this._neutral);
				}
			}
			this._curBtnType = btnType;
		}

		private void ImpSwitchStatistics()
		{
			if (this._settleFormScript == null)
			{
				return;
			}
			bool flag = true;
			if (this._leftListScript != null)
			{
				int elementAmount = this._leftListScript.GetElementAmount();
				for (int i = 0; i < elementAmount; i++)
				{
					CUIListElementScript elemenet = this._leftListScript.GetElemenet(i);
					SettlementHelper component = elemenet.gameObject.GetComponent<SettlementHelper>();
					if (component.Detail.activeSelf)
					{
						component.Detail.CustomSetActive(false);
						component.Damage.CustomSetActive(true);
						flag = false;
					}
					else
					{
						component.Detail.CustomSetActive(true);
						component.Damage.CustomSetActive(false);
						flag = true;
					}
				}
			}
			if (this._rightListScript != null)
			{
				int elementAmount2 = this._rightListScript.GetElementAmount();
				for (int j = 0; j < elementAmount2; j++)
				{
					CUIListElementScript elemenet2 = this._rightListScript.GetElemenet(j);
					SettlementHelper component2 = elemenet2.gameObject.GetComponent<SettlementHelper>();
					if (component2.Detail.activeSelf)
					{
						component2.Detail.CustomSetActive(false);
						component2.Damage.CustomSetActive(true);
					}
					else
					{
						component2.Detail.CustomSetActive(true);
						component2.Damage.CustomSetActive(false);
					}
				}
			}
			this._settleFormScript.m_formWidgets[17].CustomSetActive(flag);
			this._settleFormScript.m_formWidgets[18].CustomSetActive(flag);
			this._settleFormScript.m_formWidgets[19].CustomSetActive(!flag);
			this._settleFormScript.m_formWidgets[20].CustomSetActive(!flag);
			this._settleFormScript.m_formWidgets[11].CustomSetActive(!flag);
			this._settleFormScript.m_formWidgets[12].CustomSetActive(flag);
			this.UpdateSharePVPDataCaption(flag);
		}

		private void UpdatePlayerKda(PlayerKDA kda)
		{
			if (kda == null)
			{
				return;
			}
			CUIListScript cUIListScript = null;
			int index = 0;
			SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
			if (curLvelContext == null)
			{
				return;
			}
			COM_PLAYERCAMP playerCamp = kda.PlayerCamp;
			if (playerCamp != COM_PLAYERCAMP.COM_PLAYERCAMP_1)
			{
				if (playerCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_2)
				{
					cUIListScript = this._rightListScript;
					index = this._curRightIndex++;
				}
			}
			else
			{
				cUIListScript = this._leftListScript;
				index = this._curLeftIndex++;
			}
			if (cUIListScript == null)
			{
				return;
			}
			CUIListElementScript elemenet = cUIListScript.GetElemenet(index);
			if (elemenet == null)
			{
				return;
			}
			SettlementHelper component = elemenet.gameObject.GetComponent<SettlementHelper>();
			this.UpdateEquip(component.Tianfu, kda);
			this.UpdateAchievements(component.Achievements, kda);
			bool flag = kda.PlayerCamp == Singleton<GamePlayerCenter>.instance.GetHostPlayer().PlayerCamp;
			bool flag2 = (this._win && flag) || (!this._win && !flag);
			float num = 0f;
			if (!Singleton<BattleStatistic>.instance.GetServerMvpScore(kda.PlayerId, out num))
			{
				num = (flag2 ? kda.MvpValue : (kda.MvpValue * (GameDataMgr.globalInfoDatabin.GetDataByKey(296u).dwConfValue / 100f)));
			}
			if (flag2)
			{
				ResGlobalInfo resGlobalInfo = null;
				int num2 = 1;
				int num3 = 0;
				if (GameDataMgr.svr2CltCfgDict.TryGetValue(25u, out resGlobalInfo))
				{
					num2 = (int)resGlobalInfo.dwConfValue;
				}
				if (GameDataMgr.svr2CltCfgDict.TryGetValue(26u, out resGlobalInfo))
				{
					num3 = (int)resGlobalInfo.dwConfValue;
				}
				num = num * (float)num2 + (float)num3;
			}
			else
			{
				ResGlobalInfo resGlobalInfo2 = null;
				int num4 = 1;
				int num5 = 0;
				if (GameDataMgr.svr2CltCfgDict.TryGetValue(27u, out resGlobalInfo2))
				{
					num4 = (int)resGlobalInfo2.dwConfValue;
				}
				if (GameDataMgr.svr2CltCfgDict.TryGetValue(28u, out resGlobalInfo2))
				{
					num5 = (int)resGlobalInfo2.dwConfValue;
				}
				num = num * (float)num4 + (float)num5;
			}
			if (this.playerNum > 2)
			{
				if (kda.PlayerId == Singleton<BattleStatistic>.instance.GetMvpPlayer(kda.PlayerCamp, flag2))
				{
					if (flag2)
					{
						component.WinMvpIcon.SetActive(true);
						component.WinMvpTxt.gameObject.SetActive(true);
						component.LoseMvpIcon.SetActive(false);
						component.LoseMvpTxt.gameObject.SetActive(false);
						component.NormalMvpIcon.SetActive(false);
						component.NormalMvpTxt.gameObject.SetActive(false);
						component.WinMvpTxt.set_text(Math.Max(num, 0f).ToString("F1"));
					}
					else
					{
						component.WinMvpIcon.SetActive(false);
						component.WinMvpTxt.gameObject.SetActive(false);
						component.LoseMvpIcon.SetActive(true);
						component.LoseMvpTxt.gameObject.SetActive(true);
						component.NormalMvpIcon.SetActive(false);
						component.NormalMvpTxt.gameObject.SetActive(false);
						component.LoseMvpTxt.set_text(Math.Max(num, 0f).ToString("F1"));
					}
				}
				else
				{
					component.WinMvpIcon.SetActive(false);
					component.WinMvpTxt.gameObject.SetActive(false);
					component.LoseMvpIcon.SetActive(false);
					component.LoseMvpTxt.gameObject.SetActive(false);
					component.NormalMvpIcon.SetActive(true);
					component.NormalMvpTxt.gameObject.SetActive(true);
					component.NormalMvpTxt.set_text(Math.Max(num, 0f).ToString("F1"));
				}
			}
			else
			{
				component.WinMvpIcon.SetActive(false);
				component.WinMvpTxt.gameObject.SetActive(false);
				component.LoseMvpIcon.SetActive(false);
				component.LoseMvpTxt.gameObject.SetActive(false);
				component.NormalMvpIcon.SetActive(false);
				component.NormalMvpTxt.gameObject.SetActive(false);
			}
			component.PlayerName.GetComponent<Text>().set_text(kda.PlayerName);
			component.PlayerLv.CustomSetActive(false);
			if (kda.PlayerId == Singleton<GamePlayerCenter>.GetInstance().HostPlayerId)
			{
				component.PlayerName.GetComponent<Text>().set_color(CUIUtility.s_Text_Color_Self);
				component.PlayerLv.GetComponent<Text>().set_color(CUIUtility.s_Text_Color_Self);
				component.ItsMe.CustomSetActive(true);
				MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(component.HeroNobe.GetComponent<Image>(), (int)Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().GetNobeInfo().stGameVipClient.dwCurLevel, false, true, kda.PrivacyBits);
			}
			else
			{
				component.ItsMe.CustomSetActive(false);
				MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(component.HeroNobe.GetComponent<Image>(), (int)kda.PlayerVipLv, false, false, kda.PrivacyBits);
			}
			if (kda.PlayerId == Singleton<GamePlayerCenter>.GetInstance().HostPlayerId || Singleton<CFriendContoller>.instance.model.IsGameFriend(kda.PlayerUid, (uint)kda.WorldId) || (kda.IsComputer && !curLvelContext.m_isWarmBattle))
			{
				component.AddFriend.CustomSetActive(false);
				component.Report.CustomSetActive(false);
				component.m_AddfriendBtnShow = false;
				component.m_ReportRootBtnShow = false;
			}
			else
			{
				component.AddFriend.CustomSetActive(true);
				component.Report.CustomSetActive(true);
				component.m_AddfriendBtnShow = true;
				component.m_ReportRootBtnShow = true;
				component.AddFriend.GetComponent<CUIEventScript>().m_onClickEventParams.commonUInt64Param1 = kda.PlayerUid;
				component.AddFriend.GetComponent<CUIEventScript>().m_onClickEventParams.commonUInt64Param2 = (ulong)((long)kda.WorldId);
				component.Report.GetComponent<CUIEventScript>().m_onClickEventParams.commonUInt64Param1 = kda.PlayerUid;
				component.Report.GetComponent<CUIEventScript>().m_onClickEventParams.commonUInt64Param2 = (ulong)((long)kda.WorldId);
			}
			if (kda.PlayerId != Singleton<GamePlayerCenter>.GetInstance().HostPlayerId && (!kda.IsComputer || curLvelContext.m_isWarmBattle) && this.playerNum >= 6 && !curLvelContext.IsGameTypePvpRoom())
			{
				component.DianZanLaHei.CustomSetActive(true);
				component.DianZanLaHei.GetComponent<CUIEventScript>().m_onClickEventParams.commonUInt64Param1 = kda.PlayerUid;
				component.DianZanLaHei.GetComponent<CUIEventScript>().m_onClickEventParams.commonUInt64Param2 = (ulong)((long)kda.WorldId);
				component.DianZanLaHei.GetComponent<CUIEventScript>().m_onClickEventParams.commonUInt32Param1 = (uint)kda.PlayerCamp;
				component.DianZanLaHei.GetComponent<CUIEventScript>().m_onClickEventParams.commonUInt16Param1 = (ushort)Singleton<GamePlayerCenter>.GetInstance().hostPlayerCamp;
			}
			else
			{
				component.DianZanLaHei.CustomSetActive(false);
			}
			component.AddFriendRoot.CustomSetActive(!this._neutral);
			component.ReportRoot.CustomSetActive(false);
			component.DianZanLaHeiRoot.CustomSetActive(false);
			component.m_AddfriendBtnShow = true;
			component.m_ReportRootBtnShow = false;
			ListView<HeroKDA>.Enumerator enumerator = kda.GetEnumerator();
			while (enumerator.MoveNext())
			{
				HeroKDA current = enumerator.Current;
				if (current != null)
				{
					Image component2 = component.HeroIcon.GetComponent<Image>();
					component2.SetSprite(string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Icon_Dir, CSkinInfo.GetHeroSkinPic((uint)current.HeroId, 0u)), this._settleFormScript, true, false, false, false);
					CUIEventScript component3 = component.HeroIcon.GetComponent<CUIEventScript>();
					CUIEventScript component4 = component.HeroIconBg.GetComponent<CUIEventScript>();
					if (Utility.IsSelf(kda.PlayerUid, kda.WorldId))
					{
						if (component3 != null)
						{
							component3.m_onClickEventParams.commonUInt64Param1 = 0uL;
							component3.m_onClickEventParams.tag = 0;
						}
						if (component4 != null)
						{
							component4.m_onClickEventParams.commonUInt64Param1 = 0uL;
							component4.m_onClickEventParams.tag = 0;
						}
					}
					else
					{
						if (component3 != null)
						{
							component3.m_onClickEventParams.commonUInt64Param1 = kda.PlayerUid;
							component3.m_onClickEventParams.tag = kda.WorldId;
							component3.m_onClickEventParams.commonBool = kda.IsComputer;
						}
						if (component4 != null)
						{
							component4.m_onClickEventParams.commonUInt64Param1 = kda.PlayerUid;
							component4.m_onClickEventParams.tag = kda.WorldId;
							component4.m_onClickEventParams.commonBool = kda.IsComputer;
						}
					}
					component.HeroLv.GetComponent<Text>().set_text(string.Format("{0}", current.SoulLevel));
					component.Kill.GetComponent<Text>().set_text(current.numKill.ToString(CultureInfo.get_InvariantCulture()));
					component.Death.GetComponent<Text>().set_text(current.numDead.ToString(CultureInfo.get_InvariantCulture()));
					component.Assist.GetComponent<Text>().set_text(current.numAssist.ToString(CultureInfo.get_InvariantCulture()));
					component.Coin.GetComponent<Text>().set_text(current.TotalCoin.ToString(CultureInfo.get_InvariantCulture()));
					uint num6;
					uint num7;
					uint num8;
					if (kda.PlayerCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
					{
						num6 = this._camp1TotalDamage;
						num7 = this._camp1TotalTakenDamage;
						num8 = this._camp1TotalToHeroDamage;
					}
					else
					{
						num6 = this._camp2TotalDamage;
						num7 = this._camp2TotalTakenDamage;
						num8 = this._camp2TotalToHeroDamage;
					}
					num6 = Math.Max(1u, num6);
					num7 = Math.Max(1u, num7);
					num8 = Math.Max(1u, num8);
					component.Damage.transform.FindChild("TotalDamageBg/TotalDamage").gameObject.GetComponent<Text>().set_text(current.hurtToEnemy.ToString(CultureInfo.get_InvariantCulture()));
					component.Damage.transform.FindChild("TotalDamageBg/TotalDamageBar").gameObject.GetComponent<Image>().set_fillAmount((float)current.hurtToEnemy / num6);
					component.Damage.transform.FindChild("TotalDamageBg/Percent").gameObject.GetComponent<Text>().set_text(string.Format("{0:P1}", (float)current.hurtToEnemy / num6));
					component.Damage.transform.FindChild("TotalTakenDamageBg/TotalTakenDamage").gameObject.GetComponent<Text>().set_text(current.hurtTakenByEnemy.ToString(CultureInfo.get_InvariantCulture()));
					component.Damage.transform.FindChild("TotalTakenDamageBg/TotalTakenDamageBar").gameObject.GetComponent<Image>().set_fillAmount((float)current.hurtTakenByEnemy / num7);
					component.Damage.transform.FindChild("TotalTakenDamageBg/Percent").gameObject.GetComponent<Text>().set_text(string.Format("{0:P1}", (float)current.hurtTakenByEnemy / num7));
					component.Damage.transform.FindChild("TotalDamageHeroBg/TotalDamageHero").gameObject.GetComponent<Text>().set_text(current.hurtToHero.ToString(CultureInfo.get_InvariantCulture()));
					component.Damage.transform.FindChild("TotalDamageHeroBg/TotalDamageHeroBar").gameObject.GetComponent<Image>().set_fillAmount((float)current.hurtToHero / num8);
					component.Damage.transform.FindChild("TotalDamageHeroBg/Percent").gameObject.GetComponent<Text>().set_text(string.Format("{0:P1}", (float)current.hurtToHero / num8));
					component.AddFriend.GetComponent<CUIEventScript>().m_onClickEventParams.tag = this.HostPlayerHeroId;
					break;
				}
			}
			component.Detail.CustomSetActive(true);
			component.Damage.CustomSetActive(false);
		}

		public int GetMvpScoreRankInCamp()
		{
			int num = 0;
			int num2 = 1;
			CPlayerKDAStat playerKDAStat = Singleton<BattleStatistic>.instance.m_playerKDAStat;
			if (playerKDAStat == null)
			{
				return num2;
			}
			DictionaryView<uint, PlayerKDA>.Enumerator enumerator = playerKDAStat.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
				PlayerKDA value = current.get_Value();
				if (value != null && value.PlayerId == Singleton<GamePlayerCenter>.instance.HostPlayerId)
				{
					int serverRawMvpScore = Singleton<BattleStatistic>.instance.GetServerRawMvpScore(value.PlayerId);
					num = serverRawMvpScore;
					break;
				}
			}
			enumerator.Reset();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, PlayerKDA> current2 = enumerator.Current;
				PlayerKDA value2 = current2.get_Value();
				if (value2 != null)
				{
					int serverRawMvpScore2 = Singleton<BattleStatistic>.instance.GetServerRawMvpScore(value2.PlayerId);
					if (value2.PlayerCamp == Singleton<GamePlayerCenter>.instance.hostPlayerCamp && value2.PlayerId != Singleton<GamePlayerCenter>.instance.HostPlayerId && serverRawMvpScore2 > num)
					{
						num2++;
					}
				}
			}
			return num2;
		}

		private int GetHostPlayerHeroId(DictionaryView<uint, PlayerKDA>.Enumerator playerKdaEmr)
		{
			while (playerKdaEmr.MoveNext())
			{
				KeyValuePair<uint, PlayerKDA> current = playerKdaEmr.Current;
				ulong playerUid = current.get_Value().PlayerUid;
				KeyValuePair<uint, PlayerKDA> current2 = playerKdaEmr.Current;
				if (Utility.IsSelf(playerUid, current2.get_Value().WorldId))
				{
					KeyValuePair<uint, PlayerKDA> current3 = playerKdaEmr.Current;
					ListView<HeroKDA>.Enumerator enumerator = current3.get_Value().GetEnumerator();
					if (enumerator.MoveNext() && enumerator.Current != null)
					{
						return enumerator.Current.HeroId;
					}
				}
			}
			return -1;
		}

		private void UpdateEquip(GameObject equip, PlayerKDA kda)
		{
			int num = 1;
			ListView<HeroKDA>.Enumerator enumerator = kda.GetEnumerator();
			while (enumerator.MoveNext())
			{
				HeroKDA current = enumerator.Current;
				if (current != null)
				{
					for (int i = 0; i < 6; i++)
					{
						ushort equipID = current.Equips[i].m_equipID;
						Transform transform = equip.transform.FindChild(string.Format("TianFu{0}", num));
						if (equipID != 0 && !(transform == null))
						{
							num++;
							CUICommonSystem.SetEquipIcon(equipID, transform.gameObject, this._settleFormScript);
						}
					}
					for (int j = num; j <= 6; j++)
					{
						Transform transform2 = equip.transform.FindChild(string.Format("TianFu{0}", j));
						if (!(transform2 == null))
						{
							transform2.gameObject.GetComponent<Image>().SetSprite(string.Format("{0}BattleSettle_EquipmentSpaceNew", CUIUtility.s_Sprite_Dynamic_Talent_Dir), this._settleFormScript, true, false, false, false);
						}
					}
					break;
				}
			}
		}

		private void SetAchievementIcon(GameObject achievements, PvpAchievement type, int index)
		{
			if (index > 8 || achievements == null)
			{
				return;
			}
			Transform transform = achievements.transform.FindChild(string.Format("Achievement{0}", index));
			if (transform == null)
			{
				return;
			}
			if (type == PvpAchievement.NULL)
			{
				transform.gameObject.CustomSetActive(false);
			}
			else
			{
				string prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Pvp_Settle_Dir, type.ToString());
				transform.gameObject.CustomSetActive(true);
				transform.GetComponent<Image>().SetSprite(prefabPath, this._settleFormScript, true, false, false, false);
			}
		}

		private void UpdateAchievements(GameObject achievements, PlayerKDA kda)
		{
			int num = 1;
			ListView<HeroKDA>.Enumerator enumerator = kda.GetEnumerator();
			while (enumerator.MoveNext())
			{
				HeroKDA current = enumerator.Current;
				if (current != null)
				{
					bool flag = false;
					for (int i = 1; i < 13; i++)
					{
						switch (i)
						{
						case 1:
							if (current.LegendaryNum > 0)
							{
								this.SetAchievementIcon(achievements, PvpAchievement.Legendary, num);
								num++;
							}
							break;
						case 2:
							if (current.PentaKillNum > 0 && !flag)
							{
								this.SetAchievementIcon(achievements, PvpAchievement.PentaKill, num);
								num++;
								flag = true;
							}
							break;
						case 3:
							if (current.QuataryKillNum > 0 && !flag)
							{
								this.SetAchievementIcon(achievements, PvpAchievement.QuataryKill, num);
								num++;
								flag = true;
							}
							break;
						case 4:
							if (current.TripleKillNum > 0 && !flag)
							{
								this.SetAchievementIcon(achievements, PvpAchievement.TripleKill, num);
								num++;
								flag = true;
							}
							break;
						case 5:
							if (current.DoubleKillNum > 0)
							{
							}
							break;
						case 6:
							if (current.bKillMost)
							{
								this.SetAchievementIcon(achievements, PvpAchievement.KillMost, num);
								num++;
							}
							break;
						case 7:
							if (current.bHurtMost)
							{
								this.SetAchievementIcon(achievements, PvpAchievement.HurtMost, num);
								num++;
							}
							break;
						case 8:
							if (current.bHurtTakenMost)
							{
								this.SetAchievementIcon(achievements, PvpAchievement.HurtTakenMost, num);
								num++;
							}
							break;
						case 9:
							if (current.bAsssistMost)
							{
								this.SetAchievementIcon(achievements, PvpAchievement.AsssistMost, num);
								num++;
							}
							break;
						case 10:
							if (current.bGetCoinMost)
							{
								this.SetAchievementIcon(achievements, PvpAchievement.GetCoinMost, num);
								num++;
							}
							break;
						case 11:
							if (current.bKillOrganMost)
							{
								this.SetAchievementIcon(achievements, PvpAchievement.KillOrganMost, num);
								num++;
							}
							break;
						case 12:
							if (kda.bRunaway || kda.bDisconnect || kda.bHangup)
							{
								this.SetAchievementIcon(achievements, PvpAchievement.RunAway, num);
								num++;
							}
							break;
						}
					}
					for (int j = num; j <= 8; j++)
					{
						this.SetAchievementIcon(achievements, PvpAchievement.NULL, j);
					}
					break;
				}
			}
		}

		private void CloseSettlementPanel()
		{
			this._settleFormScript = null;
			Singleton<CUIManager>.GetInstance().CloseForm(SettlementSystem.SettlementFormName);
			Singleton<CResourceManager>.GetInstance().UnloadUnusedAssets();
			NetworkAccelerator.TryToOpenTips();
			MonoSingleton<ShareSys>.instance.m_bShowTimeline = false;
			MonoSingleton<PandroaSys>.GetInstance().StopRedBox();
			MonoSingleton<PandroaSys>.GetInstance().StopinSettlement();
		}

		public bool IsExistSettleForm()
		{
			bool result = false;
			if (Singleton<CUIManager>.instance.GetForm(FightForm.s_battleUIForm) != null || Singleton<CUIManager>.instance.GetForm(WinLose.m_FormPath) != null || Singleton<CUIManager>.instance.GetForm(SettlementSystem.SettlementFormName) != null || Singleton<CUIManager>.instance.GetForm(this._profitFormName) != null || Singleton<CUIManager>.instance.GetForm(SingleGameSettleMgr.PATH_BURNING_WINLOSE) != null || Singleton<CUIManager>.instance.GetForm(SingleGameSettleMgr.PATH_BURNING_SETTLE) != null || Singleton<CUIManager>.instance.GetForm(PVESettleSys.PATH_STAR) != null || Singleton<CUIManager>.instance.GetForm(PVESettleSys.PATH_EXP) != null || Singleton<CUIManager>.instance.GetForm(PVESettleSys.PATH_ITEM) != null || Singleton<CUIManager>.instance.GetForm(PVESettleSys.PATH_LOSE) != null || Singleton<CUIManager>.instance.GetForm(PVESettleSys.PATH_LEVELUP) != null)
			{
				result = true;
			}
			return result;
		}

		private void ImpSettlementTimerEnd()
		{
			Singleton<GameBuilder>.instance.EndGame();
			Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharShow");
			Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharIcon");
			if (this._settleFormScript == null)
			{
				return;
			}
			this._settleFormScript.m_formWidgets[2].CustomSetActive(false);
			this._settleFormScript.m_formWidgets[16].CustomSetActive(false);
			this._settleFormScript.m_formWidgets[1].CustomSetActive(true);
			this._settleFormScript.m_formWidgets[4].CustomSetActive(true);
			this._settleFormScript.m_formWidgets[5].CustomSetActive(true);
			this._settleFormScript.m_formWidgets[6].CustomSetActive(true);
			this._settleFormScript.m_formWidgets[15].CustomSetActive(true);
			this._settleFormScript.m_formWidgets[11].CustomSetActive(false);
			this._settleFormScript.m_formWidgets[12].CustomSetActive(true);
			this._settleFormScript.m_formWidgets[19].CustomSetActive(false);
			this._settleFormScript.m_formWidgets[20].CustomSetActive(false);
			this._settleFormScript.m_formWidgets[17].CustomSetActive(true);
			this._settleFormScript.m_formWidgets[18].CustomSetActive(true);
			this._settleFormScript.m_formWidgets[22].CustomSetActive(!this._neutral);
			this._settleFormScript.m_formWidgets[27].CustomSetActive(!this._neutral);
			this._settleFormScript.m_formWidgets[9].CustomSetActive(!this._neutral);
			this._settleFormScript.m_formWidgets[10].CustomSetActive(!this._neutral);
			this._settleFormScript.m_formWidgets[25].CustomSetActive(!this._neutral);
			MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.PvPShowKDA, new uint[]
			{
				this._win ? 1u : 2u,
				(uint)(this.playerNum / 2)
			});
			SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
			if (curLvelContext != null && curLvelContext.IsMultilModeWithWarmBattle() && !this._neutral)
			{
				Singleton<CChatController>.instance.model.channelMgr.Clear(EChatChannel.Settle, 0uL, 0u);
				Singleton<CChatController>.instance.model.channelMgr.SetChatTab(CChatChannelMgr.EChatTab.Settle);
				Singleton<CChatController>.instance.ShowPanel(true, false);
				Singleton<CChatController>.instance.view.UpView(true);
				Singleton<CChatController>.instance.model.sysData.ClearEntryText();
				Singleton<CChatController>.GetInstance().BuildWarmBattlePlayerLeaveRoomSystemMsg();
				Singleton<CChatController>.GetInstance().BuildCachedPlayerLeaveRoomSystemMsg();
				Singleton<CChatController>.GetInstance().ClearCachedPlayerLeaveRoomSystemMsg();
				Singleton<CChatController>.GetInstance().SetEntryVisible(true);
			}
			else
			{
				Singleton<CChatController>.GetInstance().SetEntryVisible(false);
			}
			GameObject gameObject = this._settleFormScript.m_formWidgets[26];
			if (gameObject != null)
			{
				Singleton<CRecordUseSDK>.GetInstance().OpenMsgBoxForMomentRecorder(gameObject.transform);
			}
			if (this.m_bSendRedBag)
			{
				MonoSingleton<PandroaSys>.GetInstance().StartOpenRedBox(this.m_bWin, this.m_bMvp, this.m_bLegaendary, this.m_bPENTAKILL, this.m_bQUATARYKIL, this.m_bTRIPLEKILL, this.m_GameMode.ToString());
			}
			if (this.m_bSendPandoraMsg)
			{
				MonoSingleton<PandroaSys>.GetInstance().StartOpeninSettlement(this.m_TotalPlayerNum, this.m_GameMode, this.m_bIsWarmBattle ? 1 : 0, this.m_bHaveCpu ? 1 : 0, this.m_selectTypeHero, this.m_mapID, this.m_bLuanDou ? 1 : 0, this.m_bFireHole ? 1 : 0, this.m_bWin, this.m_bMvp, this.m_bLegaendary, this.m_bPENTAKILL, this.m_bQUATARYKIL, this.m_bTRIPLEKILL);
			}
			this.ClearSendRedBag();
		}

		protected void ImpShowReport(CUIEvent uiEvent)
		{
			if (this._settleFormScript == null || this._settleFormScript.gameObject == null)
			{
				return;
			}
			GameObject gameObject = this._settleFormScript.m_formWidgets[3];
			gameObject.CustomSetActive(true);
			this._cacheLastReportGo = uiEvent.m_srcWidget;
			this._reportUid = uiEvent.m_eventParams.commonUInt64Param1;
			this._reportWordId = (int)uiEvent.m_eventParams.commonUInt64Param2;
			CPlayerKDAStat playerKDAStat = Singleton<BattleLogic>.GetInstance().battleStat.m_playerKDAStat;
			string text = null;
			if (playerKDAStat != null)
			{
				DictionaryView<uint, PlayerKDA>.Enumerator enumerator = playerKDAStat.GetEnumerator();
				while (enumerator.MoveNext())
				{
					KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
					if (current.get_Value().PlayerUid == this._reportUid)
					{
						KeyValuePair<uint, PlayerKDA> current2 = enumerator.Current;
						if (current2.get_Value().WorldId == this._reportWordId)
						{
							KeyValuePair<uint, PlayerKDA> current3 = enumerator.Current;
							text = current3.get_Value().PlayerName;
							break;
						}
					}
				}
			}
			gameObject.transform.FindChild("ReportToggle/ReportName").gameObject.GetComponent<Text>().set_text(string.Format(Singleton<CTextManager>.GetInstance().GetText("Report_PlayerName"), text));
		}

		protected void ImpCloseReport(CUIEvent uiEvent)
		{
			if (this._settleFormScript == null || this._settleFormScript.gameObject == null)
			{
				return;
			}
			this._cacheLastReportGo = null;
			this._reportUid = 0uL;
			this._reportWordId = 0;
			GameObject obj = this._settleFormScript.m_formWidgets[3];
			obj.CustomSetActive(false);
		}

		protected void ImpDoReport(CUIEvent uiEvent)
		{
			if (this._settleFormScript == null || this._settleFormScript.gameObject == null)
			{
				return;
			}
			GameObject gameObject = this._settleFormScript.m_formWidgets[3];
			gameObject.CustomSetActive(false);
			Singleton<CUIManager>.instance.OpenTips(Singleton<CTextManager>.GetInstance().GetText("Report_Report"), false, 1.5f, null, new object[0]);
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(4305u);
			cSPkg.stPkgData.stUserComplaintReq.dwComplaintReason = 1u;
			cSPkg.stPkgData.stUserComplaintReq.ullComplaintUserUid = this._reportUid;
			cSPkg.stPkgData.stUserComplaintReq.iComplaintLogicWorldID = this._reportWordId;
			CPlayerKDAStat playerKDAStat = Singleton<BattleLogic>.GetInstance().battleStat.m_playerKDAStat;
			DictionaryView<uint, PlayerKDA>.Enumerator enumerator = playerKDAStat.GetEnumerator();
			int num = 0;
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
				PlayerKDA value = current.get_Value();
				if (value != null && value.PlayerUid == this._reportUid && value.WorldId == this._reportWordId)
				{
					if (!string.IsNullOrEmpty(value.PlayerOpenId))
					{
						Utility.StringToByteArray(value.PlayerOpenId, ref cSPkg.stPkgData.stUserComplaintReq.szComplaintUserOpenId);
					}
					byte[] array = Utility.BytesConvert(value.PlayerName);
					byte[] szComplaintPlayerName = cSPkg.stPkgData.stUserComplaintReq.szComplaintPlayerName;
					Array.Copy(array, szComplaintPlayerName, Math.Min(array.Length, szComplaintPlayerName.Length));
					szComplaintPlayerName[szComplaintPlayerName.Length - 1] = 0;
					cSPkg.stPkgData.stUserComplaintReq.iComplaintPlayerCamp = ((value.PlayerCamp == this._myCamp) ? 1 : 2);
				}
				num++;
			}
			GameObject gameObject2 = gameObject.transform.FindChild("ReportToggle").gameObject;
			if (gameObject2.transform.FindChild("ReportGuaJi").gameObject.GetComponent<Toggle>().get_isOn())
			{
				cSPkg.stPkgData.stUserComplaintReq.dwComplaintReason = 1u;
			}
			else if (gameObject2.transform.FindChild("ReportSong").gameObject.GetComponent<Toggle>().get_isOn())
			{
				cSPkg.stPkgData.stUserComplaintReq.dwComplaintReason = 2u;
			}
			else if (gameObject2.transform.FindChild("ReportXiaoJi").gameObject.GetComponent<Toggle>().get_isOn())
			{
				cSPkg.stPkgData.stUserComplaintReq.dwComplaintReason = 3u;
			}
			else if (gameObject2.transform.FindChild("ReportMaRen").gameObject.GetComponent<Toggle>().get_isOn())
			{
				cSPkg.stPkgData.stUserComplaintReq.dwComplaintReason = 4u;
			}
			else if (gameObject2.transform.FindChild("ReportYanYuan").gameObject.GetComponent<Toggle>().get_isOn())
			{
				cSPkg.stPkgData.stUserComplaintReq.dwComplaintReason = 5u;
			}
			else if (gameObject2.transform.FindChild("ReportGua").gameObject.GetComponent<Toggle>().get_isOn())
			{
				cSPkg.stPkgData.stUserComplaintReq.dwComplaintReason = 6u;
			}
			string str = CUIUtility.RemoveEmoji(gameObject2.transform.FindChild("InputField").gameObject.GetComponent<InputField>().get_text());
			Utility.StringToByteArray(str, ref cSPkg.stPkgData.stUserComplaintReq.szComplaintRemark);
			cSPkg.stPkgData.stUserComplaintReq.dwClientStartTime = this._startTimeInt;
			cSPkg.stPkgData.stUserComplaintReq.iBattlePlayerNumber = num;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
			this._reportUid = 0uL;
			this._reportWordId = 0;
			if (this._cacheLastReportGo == null)
			{
				return;
			}
			this._cacheLastReportGo.CustomSetActive(false);
			this._cacheLastReportGo = null;
		}

		private void ImpAddFriend(CUIEvent uiEvent)
		{
			Singleton<CFriendContoller>.instance.Open_Friend_Verify(uiEvent.m_eventParams.commonUInt64Param1, (uint)uiEvent.m_eventParams.commonUInt64Param2, false, COM_ADD_FRIEND_TYPE.COM_ADD_FRIEND_PVP, uiEvent.m_eventParams.tag, true);
			uiEvent.m_srcWidget.CustomSetActive(false);
		}

		public void ShowLadderSettleForm(bool win)
		{
			if (Singleton<CUIManager>.GetInstance().GetForm(this._ladderFormName) != null)
			{
				return;
			}
			this._ladderForm = Singleton<CUIManager>.GetInstance().OpenForm(this._ladderFormName, false, true);
			this._ladderRoot = this._ladderForm.gameObject.transform.FindChild("Ladder").gameObject;
			this._ladderAnimator = this._ladderRoot.GetComponent<Animator>();
			this._lastLadderWin = win;
			Transform transform = this._ladderForm.gameObject.transform.FindChild("ShareGroup/Btn_Continue");
			if (transform != null && transform.gameObject != null)
			{
				transform.gameObject.CustomSetActive(false);
			}
			Transform transform2 = this._ladderForm.gameObject.transform.FindChild("ShareGroup/Btn_Share");
			if (transform2 != null)
			{
				transform2.gameObject.CustomSetActive(false);
			}
			COMDT_RANK_SETTLE_INFO rankInfo = Singleton<BattleStatistic>.GetInstance().rankInfo;
			if (rankInfo != null)
			{
				this.SetLadderDisplayOldAndNewGrade((uint)rankInfo.bOldShowGrade, rankInfo.dwOldScore, (uint)rankInfo.bNowShowGrade, rankInfo.dwNowScore);
				this.SetBpModeOpenTip(rankInfo.bOldShowGrade, rankInfo.bNowShowGrade);
			}
			this._isSettle = true;
			this.LadderDisplayProcess();
		}

		public void ShowLadderSettleFormWithoutSettle()
		{
			if (Singleton<CUIManager>.GetInstance().GetForm(this._ladderFormName) != null)
			{
				return;
			}
			this._ladderForm = Singleton<CUIManager>.GetInstance().OpenForm(this._ladderFormName, false, true);
			this._ladderRoot = this._ladderForm.gameObject.transform.FindChild("Ladder").gameObject;
			this._ladderAnimator = this._ladderRoot.GetComponent<Animator>();
			this._isSettle = false;
			this.LadderDisplayProcess();
		}

		private void SetBpModeOpenTip(byte oldGrade, byte newGrade)
		{
			GameObject widget = this._ladderForm.GetWidget(6);
			if (widget != null)
			{
				widget.CustomSetActive(!Singleton<CLadderSystem>.GetInstance().IsUseBpMode(oldGrade) && Singleton<CLadderSystem>.GetInstance().IsUseBpMode(newGrade));
			}
		}

		private void ShowBraveScorePanel(bool isSettle)
		{
			if (this._ladderForm == null)
			{
				DebugHelper.Assert(false, "_ladderForm is null!!!");
				return;
			}
			GameObject widget = this._ladderForm.GetWidget(4);
			if (isSettle)
			{
				widget.CustomSetActive(true);
				this.ShowBraveScoreProcessPanel();
				this.ShowBraveScoreDetailPanel();
			}
			else
			{
				widget.CustomSetActive(false);
			}
		}

		private void ShowBraveScoreDetailPanel()
		{
			if (this._ladderForm == null)
			{
				DebugHelper.Assert(false, "_ladderForm is null!!!");
				return;
			}
			COMDT_RANK_SETTLE_INFO rankInfo = Singleton<BattleStatistic>.instance.rankInfo;
			if (rankInfo == null)
			{
				DebugHelper.Assert(false, "BattleStatistic.instance.rankInfo is null!!!");
				return;
			}
			uint dwGameScore = rankInfo.dwGameScore;
			uint dwConWinScore = rankInfo.dwConWinScore;
			uint dwMVPScore = rankInfo.dwMVPScore;
			uint dwMMRScore = rankInfo.dwMMRScore;
			uint num = dwGameScore + dwConWinScore + dwMVPScore + dwMMRScore;
			GameObject widget = this._ladderForm.GetWidget(0);
			GameObject widget2 = this._ladderForm.GetWidget(1);
			Text component = this._ladderForm.GetWidget(2).GetComponent<Text>();
			Text component2 = this._ladderForm.GetWidget(3).GetComponent<Text>();
			GameObject widget3 = this._ladderForm.GetWidget(26);
			if (num > 0u)
			{
				if (this.IsBraveScoreDeduction())
				{
					this._isBraveScoreIncreased = false;
					widget.CustomSetActive(false);
					widget3.CustomSetActive(true);
					component2.set_text("0");
				}
				else
				{
					this._isBraveScoreIncreased = true;
					widget.CustomSetActive(true);
					widget3.CustomSetActive(false);
					Text component3 = this._ladderForm.GetWidget(7).GetComponent<Text>();
					Text component4 = this._ladderForm.GetWidget(8).GetComponent<Text>();
					Text component5 = this._ladderForm.GetWidget(9).GetComponent<Text>();
					Text component6 = this._ladderForm.GetWidget(10).GetComponent<Text>();
					Text component7 = this._ladderForm.GetWidget(3).GetComponent<Text>();
					Text component8 = this._ladderForm.GetWidget(27).GetComponent<Text>();
					GameObject widget4 = this._ladderForm.GetWidget(18);
					GameObject widget5 = this._ladderForm.GetWidget(19);
					GameObject widget6 = this._ladderForm.GetWidget(20);
					GameObject widget7 = this._ladderForm.GetWidget(21);
					if (dwGameScore > 0u)
					{
						widget4.CustomSetActive(true);
						component3.gameObject.CustomSetActive(true);
						component3.set_text("+" + dwGameScore);
					}
					else
					{
						widget4.CustomSetActive(false);
						component3.gameObject.CustomSetActive(false);
					}
					if (dwConWinScore > 0u)
					{
						widget5.CustomSetActive(true);
						component4.gameObject.CustomSetActive(true);
						component4.set_text("+" + dwConWinScore);
						COMDT_RANKDETAIL currentRankDetail = Singleton<CLadderSystem>.GetInstance().GetCurrentRankDetail();
						if (currentRankDetail != null)
						{
							widget5.GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("Ladder_Brave_Liansheng", new string[]
							{
								currentRankDetail.dwContinuousWin.ToString()
							}));
						}
					}
					else
					{
						widget5.CustomSetActive(false);
						component4.gameObject.CustomSetActive(false);
					}
					if (dwMVPScore > 0u)
					{
						widget6.CustomSetActive(true);
						component5.gameObject.CustomSetActive(true);
						component5.set_text("+" + dwMVPScore);
						widget6.GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("Ladder_Brave_Xianfeng", new string[]
						{
							this.GetMvpScoreRankInCamp().ToString()
						}));
					}
					else
					{
						widget6.CustomSetActive(false);
						component5.gameObject.CustomSetActive(false);
					}
					if (dwMMRScore > 0u)
					{
						widget7.CustomSetActive(true);
						component6.gameObject.CustomSetActive(true);
						component6.set_text("+" + dwMMRScore);
					}
					else
					{
						widget7.CustomSetActive(false);
						component6.gameObject.CustomSetActive(false);
					}
					widget2.CustomSetActive(false);
					component2.set_text("+" + num.ToString());
				}
			}
			else
			{
				this._isBraveScoreIncreased = false;
				widget.CustomSetActive(false);
				widget3.CustomSetActive(false);
				widget2.CustomSetActive(true);
				component.set_text(Singleton<CTextManager>.GetInstance().GetText("Ladder_Brave_Guaji_Tip"));
				component2.set_text(string.Empty);
			}
		}

		private void ShowBraveScoreProcessPanel()
		{
			if (this._ladderForm == null)
			{
				DebugHelper.Assert(false, "_ladderForm is null!!!");
				return;
			}
			Image component = this._ladderForm.GetWidget(11).GetComponent<Image>();
			GameObject widget = this._ladderForm.GetWidget(28);
			Text component2 = this._ladderForm.GetWidget(12).GetComponent<Text>();
			Text component3 = this._ladderForm.GetWidget(13).GetComponent<Text>();
			COMDT_RANK_SETTLE_INFO rankInfo = Singleton<BattleStatistic>.GetInstance().rankInfo;
			if (rankInfo == null)
			{
				DebugHelper.Assert(false, "BattleStatistic.instance.rankInfo is null!!!");
				return;
			}
			uint dwOldAddStarScore = rankInfo.dwOldAddStarScore;
			uint braveScoreMax = Singleton<CLadderSystem>.GetInstance().GetBraveScoreMax(this._oldGrade);
			component.set_fillAmount(CLadderView.GetProcessCircleFillAmount((int)dwOldAddStarScore, (int)braveScoreMax));
			component2.set_text(dwOldAddStarScore + "/" + braveScoreMax);
			uint dwProtectGradeScore = CLadderSystem.GetGradeDataByShowGrade((int)this._oldGrade).dwProtectGradeScore;
			widget.transform.rotation = CLadderView.GetImgKeDuRotation(dwProtectGradeScore, braveScoreMax);
			if (dwOldAddStarScore < dwProtectGradeScore)
			{
				component.set_color(CUIUtility.s_Color_BraveScore_BaojiKedu_Off);
			}
			else
			{
				component.set_color(CUIUtility.s_Color_BraveScore_BaojiKedu_On);
			}
			component3.set_text(Singleton<CTextManager>.GetInstance().GetText("Ladder_Brave_Exchange_Tip", new string[]
			{
				braveScoreMax.ToString()
			}));
		}

		private void ShowLadderResultPanel(bool isSettle)
		{
			if (this._ladderForm == null)
			{
				DebugHelper.Assert(false, "_ladderForm is null!!!");
				return;
			}
			GameObject widget = this._ladderForm.GetWidget(14);
			GameObject widget2 = this._ladderForm.GetWidget(27);
			if (isSettle)
			{
				widget.CustomSetActive(true);
				uint selfRankClass = Singleton<CRoleInfoManager>.GetInstance().GetSelfRankClass();
				GameObject widget3 = this._ladderForm.GetWidget(15);
				CLadderView.ShowRankDetail(widget3, (byte)this._oldGrade, selfRankClass, this._oldScore, true, false, false, false, true);
				GameObject widget4 = this._ladderForm.GetWidget(16);
				CLadderView.ShowRankDetail(widget4, (byte)this._newGrade, selfRankClass, this._newScore, true, false, false, false, true);
				Text component = this._ladderForm.GetWidget(17).GetComponent<Text>();
				component.set_text(this.GetLadderResultDesc());
				COMDT_RANK_SETTLE_INFO rankInfo = Singleton<BattleStatistic>.GetInstance().rankInfo;
				Transform transform = widget.transform.FindChild("KingMarkPanel");
				if (rankInfo != null && transform != null)
				{
					transform.gameObject.CustomSetActive(rankInfo.bIsFirstWangzhe != 0);
				}
				Text component2 = widget2.GetComponent<Text>();
				if (this._oldLogicGrade > this._newLogicGrade)
				{
					COMDT_RANK_SETTLE_INFO rankInfo2 = Singleton<BattleStatistic>.GetInstance().rankInfo;
					widget2.CustomSetActive(true);
					uint num = CLadderSystem.GetGradeDataByShowGrade((int)rankInfo2.bOldShowGrade).dwProtectGradeScore - rankInfo2.dwOldAddStarScore;
					component2.set_text(Singleton<CTextManager>.GetInstance().GetText("Ladder_Brave_KeepGrade_Txt3", new string[]
					{
						num.ToString()
					}));
				}
				else
				{
					widget2.CustomSetActive(false);
				}
			}
			else
			{
				widget.CustomSetActive(false);
			}
		}

		private bool IsBraveScoreDeduction()
		{
			COMDT_RANK_SETTLE_INFO rankInfo = Singleton<BattleStatistic>.GetInstance().rankInfo;
			if (rankInfo == null)
			{
				DebugHelper.Assert(false, "BattleStatistic.instance.rankInfo is null!!!");
				return false;
			}
			return this._oldLogicGrade == this._newLogicGrade && this._oldScore == this._newScore && rankInfo.dwOldAddStarScore != 0u && rankInfo.dwNowAddStarScore == 0u;
		}

		private bool IsBraveScoreExchange()
		{
			COMDT_RANK_SETTLE_INFO rankInfo = Singleton<BattleStatistic>.GetInstance().rankInfo;
			if (rankInfo == null)
			{
				DebugHelper.Assert(false, "BattleStatistic.instance.rankInfo is null!!!");
				return false;
			}
			return this._isBraveScoreIncreased && rankInfo.dwOldAddStarScore >= rankInfo.dwNowAddStarScore;
		}

		private string GetLadderResultDesc()
		{
			string result = string.Empty;
			uint selfRankClass = Singleton<CRoleInfoManager>.GetInstance().GetSelfRankClass();
			string rankName = CLadderView.GetRankName((byte)this._oldGrade, selfRankClass);
			string rankName2 = CLadderView.GetRankName((byte)this._newGrade, selfRankClass);
			if (this._newLogicGrade > this._oldLogicGrade)
			{
				result = Singleton<CTextManager>.GetInstance().GetText("Ladder_Settle_Result_Up_Grade", new string[]
				{
					rankName,
					rankName2
				});
			}
			else if (this._newLogicGrade < this._oldLogicGrade)
			{
				result = Singleton<CTextManager>.GetInstance().GetText("Ladder_Settle_Result_Down_Grade", new string[]
				{
					rankName,
					rankName2
				});
			}
			else if (this._newScore > this._oldScore)
			{
				uint num = this._newScore - this._oldScore;
				result = Singleton<CTextManager>.GetInstance().GetText("Ladder_Settle_Result_Up_Star", new string[]
				{
					rankName2,
					num.ToString()
				});
			}
			else if (this._newScore < this._oldScore)
			{
				uint num2 = this._oldScore - this._newScore;
				result = Singleton<CTextManager>.GetInstance().GetText("Ladder_Settle_Result_Down_Star", new string[]
				{
					rankName2,
					num2.ToString()
				});
			}
			else
			{
				result = Singleton<CTextManager>.GetInstance().GetText("Ladder_Settle_Result_No_Grade_Change", new string[]
				{
					rankName2
				});
			}
			return result;
		}

		private void PlayBraveScoreAnimation()
		{
			if (this._ladderForm == null)
			{
				DebugHelper.Assert(false, "_ladderForm is null!!!");
				return;
			}
			CUIAnimatorScript component = this._ladderForm.GetWidget(4).GetComponent<CUIAnimatorScript>();
			component.PlayAnimator("BravePanel_ShowIn");
		}

		private void OnAnimatiorBravePanelShowInEnd(CUIEvent uiEvent)
		{
			if (this._ladderForm == null)
			{
				DebugHelper.Assert(false, "_ladderForm is null!!!");
				return;
			}
			COMDT_RANK_SETTLE_INFO rankInfo = Singleton<BattleStatistic>.GetInstance().rankInfo;
			if (rankInfo == null)
			{
				DebugHelper.Assert(false, "BattleStatistic.GetInstance().rankInfo is null!!!");
				return;
			}
			uint braveScoreMax = Singleton<CLadderSystem>.GetInstance().GetBraveScoreMax(this._oldGrade);
			uint dwNowAddStarScore = rankInfo.dwNowAddStarScore;
			uint selfBraveScoreMax = Singleton<CLadderSystem>.GetInstance().GetSelfBraveScoreMax();
			ResRankGradeConf gradeDataByShowGrade = CLadderSystem.GetGradeDataByShowGrade((int)rankInfo.bOldShowGrade);
			ResRankGradeConf gradeDataByShowGrade2 = CLadderSystem.GetGradeDataByShowGrade((int)rankInfo.bNowShowGrade);
			GameObject widget = this._ladderForm.GetWidget(11);
			CUIProgressUpdaterScript component = widget.GetComponent<CUIProgressUpdaterScript>();
			if (this.IsBraveScoreExchange())
			{
				float processCircleFillAmount = CLadderView.GetProcessCircleFillAmount((int)braveScoreMax, (int)braveScoreMax);
				float processCircleFillAmount2 = CLadderView.GetProcessCircleFillAmount((int)gradeDataByShowGrade.dwProtectGradeScore, (int)braveScoreMax);
				component.m_fillEndEventID = enUIEventID.SettlementSys_Ladder_OnAnimatiorBraveProgressFillFull;
				component.StartFill(processCircleFillAmount, processCircleFillAmount2, CUIProgressUpdaterScript.enFillDirection.Clockwise, -1f);
			}
			else
			{
				float processCircleFillAmount3 = CLadderView.GetProcessCircleFillAmount((int)dwNowAddStarScore, (int)selfBraveScoreMax);
				float processCircleFillAmount4 = CLadderView.GetProcessCircleFillAmount((int)gradeDataByShowGrade2.dwProtectGradeScore, (int)selfBraveScoreMax);
				bool flag = this.IsBraveScoreDeduction();
				component.m_fillEndEventID = enUIEventID.SettlementSys_Ladder_OnAnimatiorBraveProgressFillEnd;
				component.StartFill(processCircleFillAmount3, processCircleFillAmount4, flag ? CUIProgressUpdaterScript.enFillDirection.CounterClockwise : CUIProgressUpdaterScript.enFillDirection.Clockwise, -1f);
			}
			this.m_isRisingStarAnimationStarted = false;
		}

		private void OnAnimatiorBraveProgressFillFull(CUIEvent uiEvent)
		{
			if (this.IsBraveScoreExchange())
			{
				if (this._ladderForm == null)
				{
					DebugHelper.Assert(false, "_ladderForm is null!!!");
					return;
				}
				COMDT_RANK_SETTLE_INFO rankInfo = Singleton<BattleStatistic>.GetInstance().rankInfo;
				if (rankInfo == null)
				{
					DebugHelper.Assert(false, "BattleStatistic.GetInstance().rankInfo is null!!!");
					return;
				}
				uint braveScoreMax = Singleton<CLadderSystem>.GetInstance().GetBraveScoreMax(this._oldGrade);
				uint selfBraveScoreMax = Singleton<CLadderSystem>.GetInstance().GetSelfBraveScoreMax();
				uint dwProtectGradeScore = CLadderSystem.GetGradeDataByShowGrade((int)rankInfo.bNowShowGrade).dwProtectGradeScore;
				CUIAnimatorScript component = this._ladderForm.GetWidget(4).GetComponent<CUIAnimatorScript>();
				if (selfBraveScoreMax != braveScoreMax)
				{
					component.SetUIEvent(enAnimatorEventType.AnimatorEnd, enUIEventID.SettlementSys_Ladder_OnAnimatiorBraveDigitalJitterEnd, default(stUIEventParams));
					component.PlayAnimator("Digital_Jitter");
				}
				else
				{
					component.SetUIEvent(enAnimatorEventType.AnimatorEnd, enUIEventID.SettlementSys_Ladder_OnAnimatiorBraveDigitalRisingStarEnd, default(stUIEventParams));
					component.PlayAnimator("Rising_Star");
					this.m_isRisingStarAnimationStarted = true;
				}
				GameObject widget = this._ladderForm.GetWidget(11);
				CUIProgressUpdaterScript component2 = widget.GetComponent<CUIProgressUpdaterScript>();
				uint dwNowAddStarScore = rankInfo.dwNowAddStarScore;
				if (dwNowAddStarScore > 0u)
				{
					float processCircleFillAmount = CLadderView.GetProcessCircleFillAmount((int)dwNowAddStarScore, (int)selfBraveScoreMax);
					float processCircleFillAmount2 = CLadderView.GetProcessCircleFillAmount((int)dwProtectGradeScore, (int)selfBraveScoreMax);
					float processCircleFillAmount3 = CLadderView.GetProcessCircleFillAmount(0, (int)selfBraveScoreMax);
					component2.m_fillEndEventID = enUIEventID.SettlementSys_Ladder_OnAnimatiorBraveProgressFillEnd;
					component2.StartFill(processCircleFillAmount, processCircleFillAmount, CUIProgressUpdaterScript.enFillDirection.Clockwise, processCircleFillAmount3);
				}
				else
				{
					Text component3 = this._ladderForm.GetWidget(13).GetComponent<Text>();
					component3.set_text(Singleton<CTextManager>.GetInstance().GetText("Ladder_Brave_Exchange_Tip", new string[]
					{
						selfBraveScoreMax.ToString()
					}));
					Text component4 = this._ladderForm.GetWidget(12).GetComponent<Text>();
					component4.set_text(dwNowAddStarScore + "/" + selfBraveScoreMax);
					GameObject widget2 = this._ladderForm.GetWidget(28);
					widget2.transform.rotation = CLadderView.GetImgKeDuRotation(dwProtectGradeScore, selfBraveScoreMax);
					component2.ResetFillAmount();
				}
			}
		}

		private void OnAnimatiorBraveProgressFillEnd(CUIEvent uiEvent)
		{
			if (this._ladderForm == null)
			{
				DebugHelper.Assert(false, "_ladderForm is null!!!");
				return;
			}
			COMDT_RANK_SETTLE_INFO rankInfo = Singleton<BattleStatistic>.GetInstance().rankInfo;
			if (rankInfo == null)
			{
				DebugHelper.Assert(false, "BattleStatistic.instance.rankInfo is null!!!");
				return;
			}
			uint braveScoreMax = Singleton<CLadderSystem>.GetInstance().GetBraveScoreMax(this._oldGrade);
			uint dwNowAddStarScore = rankInfo.dwNowAddStarScore;
			uint selfBraveScoreMax = Singleton<CLadderSystem>.GetInstance().GetSelfBraveScoreMax();
			uint dwProtectGradeScore = CLadderSystem.GetGradeDataByShowGrade((int)rankInfo.bNowShowGrade).dwProtectGradeScore;
			Text component = this._ladderForm.GetWidget(13).GetComponent<Text>();
			component.set_text(Singleton<CTextManager>.GetInstance().GetText("Ladder_Brave_Exchange_Tip", new string[]
			{
				selfBraveScoreMax.ToString()
			}));
			Text component2 = this._ladderForm.GetWidget(12).GetComponent<Text>();
			component2.set_text(dwNowAddStarScore + "/" + selfBraveScoreMax);
			GameObject widget = this._ladderForm.GetWidget(28);
			widget.transform.rotation = CLadderView.GetImgKeDuRotation(dwProtectGradeScore, selfBraveScoreMax);
			if (this.IsBraveScoreExchange() && this.m_isRisingStarAnimationStarted)
			{
				DebugHelper.Assert(this.m_isRisingStarAnimationStarted, "发生了积分兑换缺没有播飞星动画，请检查！！！");
				return;
			}
			CUIAnimatorScript component3 = this._ladderForm.GetWidget(4).GetComponent<CUIAnimatorScript>();
			if (selfBraveScoreMax != braveScoreMax)
			{
				component3.SetUIEvent(enAnimatorEventType.AnimatorEnd, enUIEventID.SettlementSys_Ladder_OnAnimatiorBraveDigitalJitterEnd, default(stUIEventParams));
				component3.PlayAnimator("Digital_Jitter");
			}
			else if (this.IsBraveScoreDeduction())
			{
				component3.SetUIEvent(enAnimatorEventType.AnimatorEnd, enUIEventID.SettlementSys_Ladder_OnAnimatiorBraveDigitalReductionEnd, default(stUIEventParams));
				component3.PlayAnimator("Digital_reduction");
			}
			else
			{
				this.DianXing();
			}
		}

		private void OnAnimatiorBraveDigitalJitterEnd(CUIEvent uiEvent)
		{
			if (Singleton<BattleStatistic>.GetInstance().rankInfo == null)
			{
				DebugHelper.Assert(false, "BattleStatistic.instance.rankInfo is null!!!");
				return;
			}
			CUIAnimatorScript cUIAnimatorScript = (this._ladderForm != null) ? this._ladderForm.GetWidget(4).GetComponent<CUIAnimatorScript>() : null;
			if (this.IsBraveScoreExchange())
			{
				if (cUIAnimatorScript)
				{
					cUIAnimatorScript.SetUIEvent(enAnimatorEventType.AnimatorEnd, enUIEventID.SettlementSys_Ladder_OnAnimatiorBraveDigitalRisingStarEnd, default(stUIEventParams));
					cUIAnimatorScript.PlayAnimator("Rising_Star");
				}
				this.m_isRisingStarAnimationStarted = true;
			}
			else
			{
				this.DianXing();
			}
		}

		private void OnAnimatiorBraveDigitalRisingStarEnd(CUIEvent uiEvent)
		{
			this.DianXing();
		}

		private void OnAnimatiorBraveeDigitalReductionEnd(CUIEvent uiEvent)
		{
			this.DianXing();
		}

		private void PlayResultAnimation()
		{
			if (this._ladderForm == null)
			{
				DebugHelper.Assert(false, "_ladderForm is null!!!");
				return;
			}
			CUIAnimatorScript component = this._ladderForm.GetWidget(14).GetComponent<CUIAnimatorScript>();
			component.PlayAnimator("ResulltPanel_ShowIn");
		}

		public void DianXing()
		{
			if (this.NeedDianXing())
			{
				uint num = this.NeedChangeGrade();
				if (num > 0u && !this._changingGrage)
				{
					this._changingGrage = true;
					this._curMaxScore = num;
					if (this._isUp)
					{
						this._curGrade = this.OpRankShowGrade(this._curGrade, 1);
						this._curDian = 0u;
					}
					else
					{
						this._curGrade = this.OpRankShowGrade(this._curGrade, -1);
						this._curDian = this._curMaxScore;
					}
					if (this._isUp)
					{
						this.Ladder_PlayLevelUpStart();
					}
					else
					{
						this.Ladder_PlayLevelDownStart();
					}
				}
				else if (!this._changingGrage)
				{
					if (this._isUp)
					{
						this._curDian += 1u;
						this.PlayXingAnim(this._curDian, this._curMaxScore, false);
					}
					else
					{
						this.PlayXingAnim(this._curDian, this._curMaxScore, this._isDown);
					}
				}
			}
			else if (!this._doWangZheSpecial && this._oldLogicGrade == this._newLogicGrade && (ulong)this._newLogicGrade == (ulong)((long)CLadderSystem.MAX_RANK_LEVEL) && this._oldScore == this._newScore && this._newScore == 0u)
			{
				this._doWangZheSpecial = true;
				this.PlayXingAnim(this._curDian, this._curMaxScore, this._isDown);
			}
			else
			{
				this._doWangZheSpecial = false;
				this.LadderAllDisplayEnd();
			}
		}

		private void LadderAllDisplayEnd()
		{
			if (this._ladderForm == null || this._ladderForm.gameObject == null)
			{
				return;
			}
			Text component = this._ladderForm.GetWidget(5).GetComponent<Text>();
			component.set_text(Singleton<CTextManager>.GetInstance().GetText("Ladder_Inherit_Last_Season_Grade_Finish"));
			Transform transform = this._ladderForm.gameObject.transform.FindChild("ShareGroup/Btn_Continue");
			if (transform != null && transform.gameObject != null)
			{
				transform.gameObject.CustomSetActive(true);
			}
			Transform transform2 = this._ladderForm.gameObject.transform.FindChild("ShareGroup/Btn_Share");
			if (transform2 != null)
			{
				if (CSysDynamicBlock.bSocialBlocked)
				{
					this.m_bGrade = false;
				}
				if (this.m_bGrade)
				{
					transform2.gameObject.CustomSetActive(true);
				}
				else
				{
					transform2.gameObject.CustomSetActive(false);
				}
			}
			if (this._isSettle)
			{
				this.PlayResultAnimation();
			}
		}

		private void ResetAllXing(uint targetScore, uint targetMax, bool inverseShow = false)
		{
			if (this._ladderRoot == null)
			{
				return;
			}
			GameObject gameObject = this._ladderRoot.transform.FindChild(string.Format("RankConNow/ScoreCon/Con{0}Star", 3)).gameObject;
			GameObject gameObject2 = this._ladderRoot.transform.FindChild(string.Format("RankConNow/ScoreCon/Con{0}Star", 4)).gameObject;
			GameObject gameObject3 = this._ladderRoot.transform.FindChild(string.Format("RankConNow/ScoreCon/Con{0}Star", 5)).gameObject;
			gameObject.CustomSetActive(false);
			gameObject2.CustomSetActive(false);
			gameObject3.CustomSetActive(false);
			GameObject gameObject4 = this._ladderRoot.transform.FindChild("RankConNow/WangZheXing").gameObject;
			if (targetMax > 5u)
			{
				Text component = gameObject4.transform.FindChild("XingNumTxt").gameObject.GetComponent<Text>();
				component.set_text(string.Format("X{0}", this._curDian));
			}
			else
			{
				gameObject4.CustomSetActive(false);
			}
			Transform transform = this._ladderRoot.transform.FindChild(string.Format("RankConNow/ScoreCon/Con{0}Star", targetMax));
			if (transform == null)
			{
				return;
			}
			GameObject gameObject5 = transform.gameObject;
			gameObject5.CustomSetActive(true);
			for (int i = 1; i <= 5; i++)
			{
				Transform transform2 = gameObject5.transform.FindChild(string.Format("Xing{0}", i));
				Transform transform3 = gameObject5.transform.FindChild(string.Format("Xing{0}/LiangXing", i));
				if (!(transform2 == null) && !(transform3 == null))
				{
					transform2.gameObject.GetComponent<Animator>().enabled = inverseShow;
					transform3.gameObject.CustomSetActive((long)i <= (long)((ulong)targetScore));
				}
			}
		}

		private void PlayXingAnim(uint targetScore, uint targetMax, bool disappear = false)
		{
			if (this._ladderRoot == null)
			{
				return;
			}
			GameObject xing = this.GetXing(targetScore, targetMax);
			if (xing == null && targetMax > 5u)
			{
				if (disappear && this._curDian > 0u)
				{
					this._curDian -= 1u;
				}
				GameObject gameObject = this._ladderRoot.transform.FindChild("RankConNow/WangZheXing").gameObject;
				gameObject.CustomSetActive(true);
				Text component = gameObject.transform.FindChild("XingNumTxt").gameObject.GetComponent<Text>();
				component.set_text(string.Format("X{0}", this._curDian));
				Animator component2 = gameObject.GetComponent<Animator>();
				component2.Play("Base Layer.wangzhe_starend");
				Singleton<CSoundManager>.GetInstance().PostEvent(disappear ? "UI_paiwei_diuxing" : "UI_paiwei_dexing", null);
				return;
			}
			if (xing != null)
			{
				xing.CustomSetActive(true);
				Animator component3 = xing.GetComponent<Animator>();
				if (component3 == null)
				{
					return;
				}
				component3.enabled = true;
				xing.transform.FindChild("LiangXing").gameObject.CustomSetActive(true);
				component3.Play(disappear ? "Base Layer.Start_Disappear" : "Base Layer.Start_ShowIn");
				Singleton<CSoundManager>.GetInstance().PostEvent(disappear ? "UI_paiwei_diuxing" : "UI_paiwei_dexing", null);
			}
		}

		private GameObject GetXing(uint targetScore, uint targetMax)
		{
			if (this._ladderRoot == null)
			{
				return null;
			}
			Transform transform = this._ladderRoot.transform.FindChild(string.Format("RankConNow/ScoreCon/Con{0}Star", targetMax));
			if (transform == null)
			{
				return null;
			}
			GameObject gameObject = transform.gameObject;
			if (gameObject == null)
			{
				return null;
			}
			Transform transform2 = gameObject.transform.FindChild(string.Format("Xing{0}", targetScore));
			return (transform2 != null) ? transform2.gameObject : null;
		}

		public uint NeedChangeGrade()
		{
			uint result = 0u;
			ResRankGradeConf gradeDataByShowGrade = CLadderSystem.GetGradeDataByShowGrade((int)this._curGrade);
			if (this._isUp && this.NeedDianXing())
			{
				if (this._curDian == gradeDataByShowGrade.dwGradeUpNeedScore)
				{
					result = CLadderSystem.GetGradeDataByLogicGrade((int)(gradeDataByShowGrade.bLogicGrade + 1)).dwGradeUpNeedScore;
				}
			}
			else if (this._isDown && this.NeedDianXing() && this._curDian == 0u)
			{
				result = CLadderSystem.GetGradeDataByLogicGrade((int)(gradeDataByShowGrade.bLogicGrade - 1)).dwGradeUpNeedScore;
			}
			return result;
		}

		public bool NeedDianXing()
		{
			byte bLogicGrade = CLadderSystem.GetGradeDataByShowGrade((int)this._curGrade).bLogicGrade;
			if (this._isUp)
			{
				return (uint)bLogicGrade < this._newLogicGrade || ((uint)bLogicGrade == this._newLogicGrade && this._curDian < this._newScore);
			}
			return (uint)bLogicGrade > this._newLogicGrade || ((uint)bLogicGrade == this._newLogicGrade && this._curDian > this._newScore);
		}

		protected void LadderDisplayProcess()
		{
			CLadderView.ShowRankDetail(this._ladderRoot.transform.FindChild("RankConNow").gameObject, (byte)this._oldGrade, Singleton<CRoleInfoManager>.GetInstance().GetSelfRankClass(), false, false);
			GameObject widget = this._ladderForm.GetWidget(5);
			widget.CustomSetActive(!this._isSettle);
			this.ShowBraveScorePanel(this._isSettle);
			this.ShowLadderResultPanel(this._isSettle);
			this.ResetAllXing(this._curDian, this._curMaxScore, false);
			this.Ladder_PlayShowIn();
		}

		public void OnLadderShowInOver()
		{
			if (this._isSettle)
			{
				this.PlayBraveScoreAnimation();
			}
			else
			{
				this.DianXing();
			}
		}

		public void OnLadderLevelUpStartOver()
		{
			if (this._ladderRoot == null)
			{
				return;
			}
			this.ResetAllXing(0u, this._curMaxScore, false);
			CLadderView.ShowRankDetail(this._ladderRoot.transform.FindChild("RankConNow").gameObject, (byte)this._curGrade, Singleton<CRoleInfoManager>.GetInstance().GetSelfRankClass(), false, false);
			this.Ladder_PlayLevelUpEnd();
		}

		public void OnLadderLevelUpEndOver()
		{
			this._changingGrage = false;
			this.DianXing();
		}

		public void OnLadderLevelDownStartOver()
		{
			if (this._ladderRoot == null)
			{
				return;
			}
			this.ResetAllXing(this._curMaxScore, this._curMaxScore, true);
			CLadderView.ShowRankDetail(this._ladderRoot.transform.FindChild("RankConNow").gameObject, (byte)this._curGrade, Singleton<CRoleInfoManager>.GetInstance().GetSelfRankClass(), false, false);
			this.Ladder_PlayLevelDownEnd();
		}

		public void OnLadderLevelDownEndOver()
		{
			if (this._ladderRoot == null)
			{
				return;
			}
			this._changingGrage = false;
			this.DianXing();
		}

		public void OnLadderXingDownOver()
		{
			GameObject xing = this.GetXing(this._curDian, this._curMaxScore);
			if (xing != null)
			{
				xing.CustomSetActive(true);
				Animator component = xing.GetComponent<Animator>();
				if (component == null)
				{
					return;
				}
				component.enabled = false;
			}
			this._curDian -= 1u;
			this.DianXing();
		}

		public void OnLadderXingUpOver()
		{
			GameObject xing = this.GetXing(this._curDian, this._curMaxScore);
			if (xing != null)
			{
				xing.CustomSetActive(true);
				Animator component = xing.GetComponent<Animator>();
				if (component == null)
				{
					return;
				}
				component.enabled = false;
			}
			this.DianXing();
		}

		public void OnLadderWangZheXingStartOver()
		{
		}

		public void OnLadderWangZheXingEndOver()
		{
			if (this._ladderRoot == null)
			{
				return;
			}
			this.DianXing();
		}

		public void Ladder_PlayLevelUpStart()
		{
			if (this._ladderAnimator != null)
			{
				this._ladderAnimator.Play("Base Layer.RankConNow_LevelUpStart");
			}
		}

		public void Ladder_PlayLevelUpEnd()
		{
			if (this._ladderAnimator == null)
			{
				return;
			}
			this._ladderAnimator.Play("Base Layer.RankConNow_LevelUpEnd");
			Singleton<CSoundManager>.GetInstance().PostEvent("UI_paiwei_shengji", null);
			if (CLadderSystem.IsMaxRankGrade((byte)this._curGrade))
			{
				Transform transform = this._ladderForm.transform.FindChild("Ladder/RankConNow/WangZheEffect");
				if (transform != null)
				{
					transform.gameObject.CustomSetActive(true);
				}
			}
		}

		public void Ladder_PlayLevelDownStart()
		{
			if (this._ladderAnimator != null)
			{
				this._ladderAnimator.Play("Base Layer.RankConNow_LevelDownStart");
			}
		}

		public void Ladder_PlayLevelDownEnd()
		{
			if (this._ladderAnimator == null)
			{
				return;
			}
			this._ladderAnimator.Play("Base Layer.RankConNow_LevelDownEnd");
			Singleton<CSoundManager>.GetInstance().PostEvent("UI_paiwei_jiangji", null);
		}

		public void Ladder_PlayShowIn()
		{
			if (this._ladderAnimator != null)
			{
				this._ladderAnimator.Play("Base Layer.RankConNow_ShowIn");
			}
		}

		public void SetLadderDisplayOldAndNewGrade(uint oldGrade, uint oldScore, uint newGrade, uint newScore)
		{
			this._oldGrade = oldGrade;
			this._oldScore = oldScore;
			ResRankGradeConf gradeDataByShowGrade = CLadderSystem.GetGradeDataByShowGrade((int)this._oldGrade);
			this._oldMaxScore = gradeDataByShowGrade.dwGradeUpNeedScore;
			this._oldLogicGrade = (uint)gradeDataByShowGrade.bLogicGrade;
			this._newGrade = newGrade;
			this._newScore = newScore;
			ResRankGradeConf gradeDataByShowGrade2 = CLadderSystem.GetGradeDataByShowGrade((int)this._newGrade);
			this._newMaxScore = gradeDataByShowGrade2.dwGradeUpNeedScore;
			this._newLogicGrade = (uint)gradeDataByShowGrade2.bLogicGrade;
			this._isUp = false;
			this._isDown = false;
			if (this._oldLogicGrade < this._newLogicGrade)
			{
				this.m_bGrade = true;
			}
			else
			{
				this.m_bGrade = false;
			}
			if (this._oldLogicGrade < this._newLogicGrade || (this._oldLogicGrade == this._newLogicGrade && this._oldScore < this._newScore))
			{
				this._isUp = true;
				this._isDown = false;
			}
			else
			{
				this._isDown = true;
				this._isUp = false;
			}
			this._curDian = this._oldScore;
			this._curGrade = this._oldGrade;
			this._curMaxScore = this._oldMaxScore;
		}

		protected void OnLadderClickContinue(CUIEvent uiEvent)
		{
			Singleton<CUIManager>.GetInstance().CloseForm(this._ladderFormName);
			this._ladderForm = null;
			this._ladderAnimator = null;
			this._ladderRoot = null;
			if (this._isSettle)
			{
				this.ShowPersonalProfit(this._lastLadderWin);
				this._lastLadderWin = false;
			}
			Singleton<CLadderSystem>.GetInstance().PromptSkinRewardIfNeed();
		}

		private void CheckPVPAchievement()
		{
			this.m_sharePVPAchieventForm.Init(this._win);
			if (this._win && this.m_sharePVPAchieventForm.CheckAchievement())
			{
				this.m_sharePVPAchieventForm.ShowVictory();
			}
			else
			{
				this.ShowSettlementPanel(false);
			}
		}

		private void OnShowDefeatShare(CUIEvent uiEvent)
		{
			if (!this._win)
			{
				this.m_sharePVPAchieventForm.ShowDefeat();
			}
			Singleton<CChatController>.instance.ShowPanel(false, false);
		}

		private uint OpRankShowGrade(uint rankWhowGrade, int opNum)
		{
			byte bLogicGrade = CLadderSystem.GetGradeDataByShowGrade((int)rankWhowGrade).bLogicGrade;
			int num = (int)bLogicGrade + opNum;
			if (num <= 0 || num > CLadderSystem.MAX_RANK_LEVEL)
			{
				return 0u;
			}
			return (uint)CLadderSystem.GetGradeDataByLogicGrade(num).bGrade;
		}

		private void ShowElementAddFriendBtn(bool bShow)
		{
			if (!bShow)
			{
				if (this._leftListScript != null)
				{
					int elementAmount = this._leftListScript.GetElementAmount();
					for (int i = 0; i < elementAmount; i++)
					{
						CUIListElementScript elemenet = this._leftListScript.GetElemenet(i);
						SettlementHelper component = elemenet.gameObject.GetComponent<SettlementHelper>();
						component.AddFriendRoot.CustomSetActive(false);
						component.ReportRoot.CustomSetActive(false);
						component.DianZanLaHeiRoot.CustomSetActive(false);
					}
				}
				if (this._rightListScript != null)
				{
					int elementAmount2 = this._rightListScript.GetElementAmount();
					for (int j = 0; j < elementAmount2; j++)
					{
						CUIListElementScript elemenet2 = this._rightListScript.GetElemenet(j);
						SettlementHelper component2 = elemenet2.gameObject.GetComponent<SettlementHelper>();
						component2.AddFriendRoot.CustomSetActive(false);
						component2.ReportRoot.CustomSetActive(false);
						component2.DianZanLaHeiRoot.CustomSetActive(false);
					}
				}
			}
			else
			{
				if (this._leftListScript != null)
				{
					int elementAmount3 = this._leftListScript.GetElementAmount();
					for (int k = 0; k < elementAmount3; k++)
					{
						CUIListElementScript elemenet3 = this._leftListScript.GetElemenet(k);
						SettlementHelper component3 = elemenet3.gameObject.GetComponent<SettlementHelper>();
						component3.AddFriendRoot.CustomSetActive(this._curBtnType == SettlementSystem.ShowBtnType.AddFriend && !this._neutral);
						component3.ReportRoot.CustomSetActive(this._curBtnType == SettlementSystem.ShowBtnType.Report && !this._neutral);
						component3.DianZanLaHeiRoot.CustomSetActive(this._curBtnType == SettlementSystem.ShowBtnType.LaHeiDianZan && !this._neutral);
					}
				}
				if (this._rightListScript != null)
				{
					int elementAmount4 = this._rightListScript.GetElementAmount();
					for (int l = 0; l < elementAmount4; l++)
					{
						CUIListElementScript elemenet4 = this._rightListScript.GetElemenet(l);
						SettlementHelper component4 = elemenet4.gameObject.GetComponent<SettlementHelper>();
						component4.AddFriendRoot.CustomSetActive(this._curBtnType == SettlementSystem.ShowBtnType.AddFriend && !this._neutral);
						component4.ReportRoot.CustomSetActive(this._curBtnType == SettlementSystem.ShowBtnType.Report && !this._neutral);
						component4.DianZanLaHeiRoot.CustomSetActive(this._curBtnType == SettlementSystem.ShowBtnType.LaHeiDianZan && !this._neutral);
					}
				}
			}
		}

		private void OnShowPVPSettleDataClose(CUIEvent uiEvnet)
		{
			Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.SHARE_PVP_SETTLEDATA_CLOSE);
			this.ChangeSharePVPDataBtnState(false);
			MonoSingleton<ShareSys>.GetInstance().m_bShowTimeline = this.m_bBackShowTimeLine;
			this.ShowElementAddFriendBtn(true);
			SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
			if (curLvelContext != null && curLvelContext.IsMultilModeWithWarmBattle())
			{
				Singleton<CChatController>.instance.ShowPanel(true, false);
				MonoSingleton<ShareSys>.GetInstance().m_ShareActivityParam.clear();
			}
		}

		private void OnShowPVPSettleData(CUIEvent uiEvnet)
		{
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			uint num = 0u;
			uint num2 = 0u;
			if (curLvelContext != null)
			{
				num = (uint)(curLvelContext.GetGameType() + 1);
				if (curLvelContext.IsMobaModeWithOutGuide())
				{
					num2 = (uint)curLvelContext.m_pvpPlayerNum;
				}
			}
			MonoSingleton<ShareSys>.GetInstance().m_ShareActivityParam.set(1, 2, new uint[]
			{
				num,
				num2
			});
			this.m_bBackShowTimeLine = MonoSingleton<ShareSys>.GetInstance().m_bShowTimeline;
			this.ChangeSharePVPDataBtnState(true);
			Singleton<EventRouter>.instance.AddEventHandler<Transform>(EventID.SHARE_TIMELINE_SUCC, new Action<Transform>(this.OnShareTimeLineSucc));
			this.ShowElementAddFriendBtn(false);
			Singleton<CChatController>.instance.ShowPanel(false, false);
		}

		private void ClearShareData()
		{
			this.m_bLastAddFriendBtnState = false;
			this.m_bLastReprotBtnState = false;
			this.m_bLastOverViewBtnState = false;
			this.m_bLastDataBtnState = false;
			this.m_bShareDataSucc = false;
			this.m_bShareOverView = false;
			this.m_bIsDetail = true;
			this.m_bBackShowTimeLine = false;
			this.m_PVPBtnGroup = null;
			this.m_PVPSwtichAddFriend = null;
			this.m_PVPSwitchStatistics = null;
			this.m_PVPSwitchOverview = null;
			this.m_PVPShareDataBtnGroup = null;
			this.m_PVPShareBtnClose = null;
			this.m_timeLineText = null;
			this.m_BtnTimeLine = null;
			this.m_TxtBtnShareCaption = null;
			this.m_ShareDataBtn = null;
			this.m_btnVictotyTips = null;
			this.m_ChatNode = null;
			this.m_logIcon = null;
		}

		private void ChangeSharePVPDataBtnState(bool bShowShare)
		{
			if (this.m_logIcon)
			{
				this.m_logIcon.gameObject.CustomSetActive(!bShowShare);
			}
			if (this.m_ChatNode)
			{
				this.m_ChatNode.gameObject.CustomSetActive(!bShowShare);
			}
			if (this.m_btnVictotyTips)
			{
				this.m_btnVictotyTips.gameObject.CustomSetActive(!bShowShare);
			}
			if (this.m_PVPBtnGroup)
			{
				this.m_PVPBtnGroup.gameObject.SetActive(!bShowShare);
			}
			if (this.m_PVPSwtichAddFriend)
			{
				if (bShowShare)
				{
					this.m_bLastAddFriendBtnState = this.m_PVPSwtichAddFriend.gameObject.activeSelf;
					this.m_PVPSwtichAddFriend.gameObject.SetActive(false);
				}
				else
				{
					this.m_PVPSwtichAddFriend.gameObject.SetActive(this.m_bLastAddFriendBtnState);
				}
			}
			if (this.m_PVPSwitchStatistics)
			{
				if (bShowShare)
				{
					this.m_bLastDataBtnState = this.m_PVPSwitchStatistics.gameObject.activeSelf;
					this.m_PVPSwitchStatistics.gameObject.SetActive(false);
				}
				else
				{
					this.m_PVPSwitchStatistics.gameObject.SetActive(this.m_bLastDataBtnState);
				}
			}
			if (this._settleFormScript != null)
			{
				this._settleFormScript.m_formWidgets[9].CustomSetActive(!bShowShare);
				this._settleFormScript.m_formWidgets[10].CustomSetActive(!bShowShare);
				this._settleFormScript.m_formWidgets[25].CustomSetActive(!bShowShare);
			}
			if (this.m_PVPSwitchOverview)
			{
				if (bShowShare)
				{
					this.m_bLastOverViewBtnState = this.m_PVPSwitchOverview.gameObject.activeSelf;
					this.m_PVPSwitchOverview.gameObject.SetActive(false);
				}
				else
				{
					this.m_PVPSwitchOverview.gameObject.SetActive(this.m_bLastOverViewBtnState);
				}
			}
			if (this.m_bIsDetail)
			{
				if (this.m_bShareDataSucc)
				{
					this.UpdateTimeBtnState(false);
				}
				else
				{
					this.UpdateTimeBtnState(true);
				}
			}
			else if (this.m_bShareOverView)
			{
				this.UpdateTimeBtnState(false);
			}
			else
			{
				this.UpdateTimeBtnState(true);
			}
			if (this.m_PVPShareDataBtnGroup)
			{
				this.m_PVPShareDataBtnGroup.gameObject.SetActive(bShowShare);
			}
			if (this.m_PVPShareBtnClose)
			{
				this.m_PVPShareBtnClose.gameObject.SetActive(bShowShare);
			}
		}

		private void UpdateTimeBtnState(bool bShow)
		{
			if (this.m_BtnTimeLine)
			{
				CUIEventScript component = this.m_BtnTimeLine.GetComponent<CUIEventScript>();
				component.enabled = bShow;
				this.m_BtnTimeLine.GetComponent<Button>().set_interactable(bShow);
				float a = 0.37f;
				if (bShow)
				{
					a = 1f;
				}
				this.m_BtnTimeLine.GetComponent<Image>().set_color(new Color(this.m_BtnTimeLine.GetComponent<Image>().get_color().r, this.m_BtnTimeLine.GetComponent<Image>().get_color().g, this.m_BtnTimeLine.GetComponent<Image>().get_color().b, a));
				if (this.m_timeLineText)
				{
					this.m_timeLineText.set_color(new Color(this.m_timeLineText.get_color().r, this.m_timeLineText.get_color().g, this.m_timeLineText.get_color().b, a));
				}
			}
		}

		public void SnapScreenShotShowBtn(bool bClose)
		{
			if (this.m_PVPShareDataBtnGroup)
			{
				this.m_PVPShareDataBtnGroup.gameObject.SetActive(bClose);
			}
			if (this.m_PVPShareBtnClose)
			{
				this.m_PVPShareBtnClose.gameObject.SetActive(bClose);
			}
		}

		private void InitShareDataBtn(CUIFormScript form)
		{
			this.m_logIcon = form.gameObject.transform.FindChild("Panel/Logo");
			this.m_ChatNode = form.gameObject.transform.FindChild("Panel/entry_node");
			this.m_btnVictotyTips = form.gameObject.transform.FindChild("Panel/ButtonVictoryTips");
			this.m_PVPBtnGroup = form.gameObject.transform.FindChild("Panel/ButtonGrid");
			this.m_PVPSwtichAddFriend = form.gameObject.transform.FindChild("Panel/SwtichAddFriend");
			this.m_PVPSwitchStatistics = form.gameObject.transform.FindChild("Panel/SwitchStatistics");
			this.m_TxtBtnShareCaption = form.gameObject.transform.FindChild("Panel/ButtonGrid/ButtonShareData/Text").GetComponent<Text>();
			this.m_PVPSwitchOverview = form.gameObject.transform.FindChild("Panel/SwitchOverview");
			this.m_PVPShareDataBtnGroup = form.gameObject.transform.FindChild("Panel/ShareGroup");
			this.m_BtnTimeLine = form.gameObject.transform.FindChild("Panel/ShareGroup/Button_TimeLine");
			this.m_PVPShareBtnClose = form.gameObject.transform.FindChild("Panel/Btn_Share_PVP_DATA_CLOSE");
			this.m_timeLineText = form.gameObject.transform.FindChild("Panel/ShareGroup/Button_TimeLine/ClickText").GetComponent<Text>();
			ShareSys.SetSharePlatfText(this.m_timeLineText);
			this.UpdateSharePVPDataCaption(this.m_bIsDetail);
		}

		private void UpdateSharePVPDataCaption(bool isDetail)
		{
			if (!CSysDynamicBlock.bSocialBlocked && !this._neutral)
			{
				if (!isDetail)
				{
					if (this.m_TxtBtnShareCaption)
					{
						this.m_TxtBtnShareCaption.set_text("分享数据");
					}
					if (this.m_ShareDataBtn)
					{
						this.m_ShareDataBtn.CustomSetActive(true);
					}
				}
				else
				{
					if (this.m_TxtBtnShareCaption)
					{
						this.m_TxtBtnShareCaption.set_text("分享战绩");
					}
					if (this.m_ShareDataBtn)
					{
						this.m_ShareDataBtn.CustomSetActive(true);
					}
				}
				this.m_bIsDetail = !isDetail;
			}
		}

		private void OnShareTimeLineSucc(Transform btn)
		{
			if (this.m_BtnTimeLine != null && this.m_BtnTimeLine == btn)
			{
				if (this.m_bIsDetail)
				{
					this.m_bShareDataSucc = true;
				}
				else
				{
					this.m_bShareOverView = true;
				}
			}
		}

		private void OnShareUpdateGradShare(CUIEvent uiEvent)
		{
			CUIFormScript cUIFormScript = Singleton<CUIManager>.instance.OpenForm("UGUI/Form/System/ShareUI/Form_SharePVPLadder.prefab", false, true);
			this.m_UpdateGradeForm = cUIFormScript;
			CLadderView.ShowRankDetail(cUIFormScript.transform.FindChild("ShareFrame/Ladder/RankConNow").gameObject, (byte)this._curGrade, Singleton<CRoleInfoManager>.GetInstance().GetSelfRankClass(), false, false);
			MonoSingleton<ShareSys>.GetInstance().UpdateShareGradeForm(cUIFormScript);
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo != null)
			{
				CUIHttpImageScript componetInChild = Utility.GetComponetInChild<CUIHttpImageScript>(cUIFormScript.gameObject, "ShareFrame/Ladder/RankConNow/PlayerHead");
				if (componetInChild)
				{
					componetInChild.SetImageUrl(masterRoleInfo.HeadUrl);
				}
				Text componetInChild2 = Utility.GetComponetInChild<Text>(cUIFormScript.gameObject, "ShareFrame/Ladder/RankConNow/PlayerName");
				if (componetInChild2)
				{
					componetInChild2.set_text(masterRoleInfo.Name);
				}
				Transform transform = cUIFormScript.transform.FindChild("ShareFrame/Ladder/Ladder/KingMarkPanel");
				if (transform != null)
				{
					if (masterRoleInfo.m_WangZheCnt > 0u)
					{
						transform.gameObject.CustomSetActive(true);
						Image componetInChild3 = Utility.GetComponetInChild<Image>(transform.gameObject, "KingMarkIcon");
						if (componetInChild3 != null)
						{
							componetInChild3.SetSprite(string.Format(CLadderSystem.ICON_KING_MARK_PATH, masterRoleInfo.m_WangZheCnt), cUIFormScript, true, false, false, false);
						}
					}
					else
					{
						transform.gameObject.CustomSetActive(false);
					}
				}
			}
		}

		private void OnShareUpdateGradShareClose(CUIEvent uiEvent)
		{
			if (this.m_UpdateGradeForm)
			{
				Singleton<CUIManager>.instance.CloseForm(this.m_UpdateGradeForm);
			}
			this.m_UpdateGradeForm = null;
		}
	}
}
