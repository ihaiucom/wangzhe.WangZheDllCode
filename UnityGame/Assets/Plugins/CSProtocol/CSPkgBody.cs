using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class CSPkgBody : ProtocolObject
	{
		public ProtocolObject dataObject;

		public byte[] szData;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 242u;

		public static readonly int CLASS_ID = 1645;

		public CSPKG_CMD_HEARTBEAT stHeartBeat
		{
			get
			{
				return this.dataObject as CSPKG_CMD_HEARTBEAT;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_GAMELOGINDISPATCH stGameLoginDispatch
		{
			get
			{
				return this.dataObject as SCPKG_GAMELOGINDISPATCH;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_GAMELOGINREQ stGameLoginReq
		{
			get
			{
				return this.dataObject as CSPKG_CMD_GAMELOGINREQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_GAMELOGINRSP stGameLoginRsp
		{
			get
			{
				return this.dataObject as SCPKG_CMD_GAMELOGINRSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_GAMING_UPERMSG stGamingUperMsg
		{
			get
			{
				return this.dataObject as CSPKG_GAMING_UPERMSG;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_NTF_ACNT_REGISTER stNtfAcntRegiter
		{
			get
			{
				return this.dataObject as SCPKG_NTF_ACNT_REGISTER;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_ACNT_REGISTER_REQ stAcntRegisterReq
		{
			get
			{
				return this.dataObject as CSPKG_ACNT_REGISTER_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_ACNT_REGISTER_RES stAcntRegisterRes
		{
			get
			{
				return this.dataObject as CSPKG_ACNT_REGISTER_RES;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_NTF_ACNT_INFO_UPD stNtfAcntInfoUpd
		{
			get
			{
				return this.dataObject as SCPKG_NTF_ACNT_INFO_UPD;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_NTF_ACNT_LEVELUP stNtfAcntLevelUp
		{
			get
			{
				return this.dataObject as SCPKG_NTF_ACNT_LEVELUP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_CHEATCMD stCheatCmd
		{
			get
			{
				return this.dataObject as CSPKG_CMD_CHEATCMD;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_LOGINFINISHNTF stGameLoginFinishNtf
		{
			get
			{
				return this.dataObject as SCPKG_CMD_LOGINFINISHNTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_RELOGINNOW stReloginNow
		{
			get
			{
				return this.dataObject as SCPKG_CMD_RELOGINNOW;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_NTF_ACNT_PVPLEVELUP stNtfAcntPvpLevelUp
		{
			get
			{
				return this.dataObject as SCPKG_NTF_ACNT_PVPLEVELUP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_GAMELOGOUTREQ stGameLogoutReq
		{
			get
			{
				return this.dataObject as CSPKG_CMD_GAMELOGOUTREQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_GAMELOGOUTRSP stGameLogoutRsp
		{
			get
			{
				return this.dataObject as SCPKG_CMD_GAMELOGOUTRSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_LOGINSYN_REQ stLoginSynReq
		{
			get
			{
				return this.dataObject as SCPKG_CMD_LOGINSYN_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_LOGINSYN_RSP stLoginSynRsp
		{
			get
			{
				return this.dataObject as CSPKG_CMD_LOGINSYN_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CREATEULTIGAMEREQ stCreateMultGameReq
		{
			get
			{
				return this.dataObject as CSPKG_CREATEULTIGAMEREQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_JOINMULTIGAMERSP stJoinMultGameRsp
		{
			get
			{
				return this.dataObject as SCPKG_JOINMULTIGAMERSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_QUITMULTIGAMEREQ stQuitMultGameReq
		{
			get
			{
				return this.dataObject as CSPKG_QUITMULTIGAMEREQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_QUITMULTIGAMERSP stQuitMultGameRsp
		{
			get
			{
				return this.dataObject as SCPKG_QUITMULTIGAMERSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_ROOMCHGNTF stRoomChgNtf
		{
			get
			{
				return this.dataObject as SCPKG_ROOMCHGNTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_ASK_ACNT_TRANS_VISITORSVRDATA stAskAcntTransVisitorSvrData
		{
			get
			{
				return this.dataObject as SCPKG_ASK_ACNT_TRANS_VISITORSVRDATA;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_RSP_ACNT_TRANS_VISITORSVRDATA stRspAcntTransVisitorSvrData
		{
			get
			{
				return this.dataObject as CSPKG_RSP_ACNT_TRANS_VISITORSVRDATA;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_GAMECONN_REDIRECT stGameConnRedirect
		{
			get
			{
				return this.dataObject as SCPKG_GAMECONN_REDIRECT;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_FRAPBOOT_SINGLE stFrapBootSingle
		{
			get
			{
				return this.dataObject as SCPKG_FRAPBOOT_SINGLE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_FRAPBOOTINFO stFrapBootInfo
		{
			get
			{
				return this.dataObject as SCPKG_FRAPBOOTINFO;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_REQUESTFRAPBOOTSINGLE stReqFrapBootSingle
		{
			get
			{
				return this.dataObject as CSPKG_REQUESTFRAPBOOTSINGLE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_REQUESTFRAPBOOTTIMEOUT stReqFrapBootTimeout
		{
			get
			{
				return this.dataObject as CSPKG_REQUESTFRAPBOOTTIMEOUT;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_OFFINGRESTART_REQ stOffingRestartReq
		{
			get
			{
				return this.dataObject as SCPKG_OFFINGRESTART_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_OFFINGRESTART_RSP stOffingRestartRsp
		{
			get
			{
				return this.dataObject as CSPKG_OFFINGRESTART_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_GAMELOGINLIMIT stLoginLimitRsp
		{
			get
			{
				return this.dataObject as SCPKG_CMD_GAMELOGINLIMIT;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_BANTIME_CHG stBanTimeChg
		{
			get
			{
				return this.dataObject as SCPKG_CMD_BANTIME_CHG;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_ISACCEPT_AIPLAYER_REQ stIsAcceptAiPlayerReq
		{
			get
			{
				return this.dataObject as SCPKG_ISACCEPT_AIPLAYER_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_ISACCEPT_AIPLAYER_RSP stIsAcceptAiPlayerRsp
		{
			get
			{
				return this.dataObject as CSPKG_ISACCEPT_AIPLAYER_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_NOTICE_HANGUP stNoticeHangup
		{
			get
			{
				return this.dataObject as SCPKG_NOTICE_HANGUP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_STARTSINGLEGAMEREQ stStartSingleGameReq
		{
			get
			{
				return this.dataObject as CSPKG_STARTSINGLEGAMEREQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_STARTSINGLEGAMERSP stStartSingleGameRsp
		{
			get
			{
				return this.dataObject as SCPKG_STARTSINGLEGAMERSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_SINGLEGAMEFINREQ stFinSingleGameReq
		{
			get
			{
				return this.dataObject as CSPKG_SINGLEGAMEFINREQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_SINGLEGAMEFINRSP stFinSingleGameRsp
		{
			get
			{
				return this.dataObject as SCPKG_SINGLEGAMEFINRSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_SINGLEGAMESWEEPREQ stSweepSingleGameReq
		{
			get
			{
				return this.dataObject as CSPKG_SINGLEGAMESWEEPREQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_SINGLEGAMESWEEPRSP stSweepSingleGameRsp
		{
			get
			{
				return this.dataObject as SCPKG_SINGLEGAMESWEEPRSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_GET_CHAPTER_REWARD_REQ stGetChapterRewardReq
		{
			get
			{
				return this.dataObject as CSPKG_GET_CHAPTER_REWARD_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_GET_CHAPTER_REWARD_RSP stGetChapterRewardRsp
		{
			get
			{
				return this.dataObject as SCPKG_GET_CHAPTER_REWARD_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_QUITSINGLEGAMEREQ stQuitSingleGameReq
		{
			get
			{
				return this.dataObject as CSPKG_QUITSINGLEGAMEREQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_QUITSINGLEGAMERSP stQuitSingleGameRsp
		{
			get
			{
				return this.dataObject as SCPKG_QUITSINGLEGAMERSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_ASKINMULTGAME_REQ stAskInMultGameReq
		{
			get
			{
				return this.dataObject as CSPKG_ASKINMULTGAME_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_ASKINMULTGAME_RSP stAskInMultGameRsp
		{
			get
			{
				return this.dataObject as SCPKG_ASKINMULTGAME_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_SECURE_INFO_START_REQ stSecureInfoStartReq
		{
			get
			{
				return this.dataObject as CSPKG_SECURE_INFO_START_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_MULTGAME_BEGINBAN stMultGameBeginBan
		{
			get
			{
				return this.dataObject as SCPKG_MULTGAME_BEGINBAN;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_MULTGAME_BEGINPICK stMultGameBeginPick
		{
			get
			{
				return this.dataObject as SCPKG_MULTGAME_BEGINPICK;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_MULTGAME_BEGINADJUST stMultGameBeginAdjust
		{
			get
			{
				return this.dataObject as SCPKG_MULTGAME_BEGINADJUST;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_MULTGAME_BEGINLOAD stMultGameBeginLoad
		{
			get
			{
				return this.dataObject as SCPKG_MULTGAME_BEGINLOAD;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_MULTGAME_LOADFIN stMultGameLoadFin
		{
			get
			{
				return this.dataObject as CSPKG_MULTGAME_LOADFIN;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_MULTGAME_BEGINFIGHT stMultGameBeginFight
		{
			get
			{
				return this.dataObject as SCPKG_MULTGAME_BEGINFIGHT;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_MULTGAMEREADYNTF stMultGameReadyNtf
		{
			get
			{
				return this.dataObject as SCPKG_MULTGAMEREADYNTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_MULTGAMEABORTNTF stMultGameAbortNtf
		{
			get
			{
				return this.dataObject as SCPKG_MULTGAMEABORTNTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_MULTGAME_GAMEOVER stMultGameOverReq
		{
			get
			{
				return this.dataObject as CSPKG_MULTGAME_GAMEOVER;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_MULTGAME_GAMEOVER stMultGameOverRsp
		{
			get
			{
				return this.dataObject as SCPKG_MULTGAME_GAMEOVER;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_MULTGAME_SETTLEGAIN stMultGameSettleGain
		{
			get
			{
				return this.dataObject as SCPKG_MULTGAME_SETTLEGAIN;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_MULTGAME_LOADPROCESS stMultGameLoadProcessReq
		{
			get
			{
				return this.dataObject as CSPKG_MULTGAME_LOADPROCESS;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_MULTGAME_LOADPROCESS stMultGameLoadProcessRsp
		{
			get
			{
				return this.dataObject as SCPKG_MULTGAME_LOADPROCESS;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_MULTGAME_NTF_CLT_GAMEOVER stNtfCltGameOver
		{
			get
			{
				return this.dataObject as SCPKG_MULTGAME_NTF_CLT_GAMEOVER;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_MULTGAME_RUNAWAY_REQ stRunAwayReq
		{
			get
			{
				return this.dataObject as CSPKG_MULTGAME_RUNAWAY_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_MULTGAME_RUNAWAY_RSP stRunAwayRsp
		{
			get
			{
				return this.dataObject as SCPKG_MULTGAME_RUNAWAY_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_MULTGAME_RUNAWAY_NTF stRunAwayNtf
		{
			get
			{
				return this.dataObject as SCPKG_MULTGAME_RUNAWAY_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_MULTGAMERECOVERNTF stMultGameRecoverNtf
		{
			get
			{
				return this.dataObject as SCPKG_MULTGAMERECOVERNTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_RECOVERGAMEFRAP_REQ stRecoverFrapReq
		{
			get
			{
				return this.dataObject as CSPKG_RECOVERGAMEFRAP_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_RECOVERGAMEFRAP_RSP stRecoverFrapRsp
		{
			get
			{
				return this.dataObject as SCPKG_RECOVERGAMEFRAP_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_RECONNGAME_NTF stReconnGameNtf
		{
			get
			{
				return this.dataObject as SCPKG_RECONNGAME_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_RECOVERGAMESUCC stRecoverGameSuccReq
		{
			get
			{
				return this.dataObject as CSPKG_RECOVERGAMESUCC;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_MULTGAME_DISCONN_NTF stMultGameDisconnNtf
		{
			get
			{
				return this.dataObject as SCPKG_MULTGAME_DISCONN_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_MULTGAME_RECONN_NTF stMultGameReconnNtf
		{
			get
			{
				return this.dataObject as SCPKG_MULTGAME_RECONN_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_KFRAPLATERCHG_REQ stKFrapsLaterChgReq
		{
			get
			{
				return this.dataObject as CSPKG_KFRAPLATERCHG_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_KFRAPLATERCHG_NTF stKFrapsLaterChgNtf
		{
			get
			{
				return this.dataObject as SCPKG_KFRAPLATERCHG_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_MULTGAME_DIE_REQ stMultGameDieReq
		{
			get
			{
				return this.dataObject as CSPKG_MULTGAME_DIE_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_HANGUP_NTF stHangUpNtf
		{
			get
			{
				return this.dataObject as SCPKG_HANGUP_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_ITEMSALE stItemSale
		{
			get
			{
				return this.dataObject as CSPKG_CMD_ITEMSALE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_ITEMADD stItemAdd
		{
			get
			{
				return this.dataObject as SCPKG_CMD_ITEMADD;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_ITEMDEL stItemDel
		{
			get
			{
				return this.dataObject as SCPKG_CMD_ITEMDEL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_EQUIPWEAR stEquipWear
		{
			get
			{
				return this.dataObject as CSPKG_CMD_EQUIPWEAR;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_EQUIPCHG stEquipChg
		{
			get
			{
				return this.dataObject as SCPKG_CMD_EQUIPCHG;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_PROPUSE stPropUse
		{
			get
			{
				return this.dataObject as CSPKG_CMD_PROPUSE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_PKGQUERY stPkgQuery
		{
			get
			{
				return this.dataObject as CSPKG_CMD_PKGQUERY;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_PKGDETAIL stPkgDetail
		{
			get
			{
				return this.dataObject as SCPKG_CMD_PKGDETAIL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_ITEMCOMP stItemComp
		{
			get
			{
				return this.dataObject as CSPKG_CMD_ITEMCOMP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_HEROADVANCE stHeroAdvanceReq
		{
			get
			{
				return this.dataObject as CSPKG_CMD_HEROADVANCE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_HEROADVANCE stHeroAdvanceRsp
		{
			get
			{
				return this.dataObject as SCPKG_CMD_HEROADVANCE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_SHOPBUY stShopBuyReq
		{
			get
			{
				return this.dataObject as CSPKG_CMD_SHOPBUY;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_SHOPBUY stShopBuyRsp
		{
			get
			{
				return this.dataObject as SCPKG_CMD_SHOPBUY;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_COINBUY stCoinBuyReq
		{
			get
			{
				return this.dataObject as CSPKG_CMD_COINBUY;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_COINBUY stCoinBuyRsp
		{
			get
			{
				return this.dataObject as SCPKG_CMD_COINBUY;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_NTF_CLRSHOPBUYLIMIT stClrShopBuyLimit
		{
			get
			{
				return this.dataObject as SCPKG_NTF_CLRSHOPBUYLIMIT;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_AUTOREFRESH stAutoRefresh
		{
			get
			{
				return this.dataObject as CSPKG_CMD_AUTOREFRESH;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_MANUALREFRESH stManualRefresh
		{
			get
			{
				return this.dataObject as CSPKG_CMD_MANUALREFRESH;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_SHOPDETAIL stShopDetail
		{
			get
			{
				return this.dataObject as SCPKG_CMD_SHOPDETAIL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_SYMBOLNAMECHG stSymbolNameChgReq
		{
			get
			{
				return this.dataObject as CSPKG_CMD_SYMBOLNAMECHG;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_SYMBOLNAMECHG stSymbolNameChgRsp
		{
			get
			{
				return this.dataObject as SCPKG_CMD_SYMBOLNAMECHG;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_HORNUSE stHornUseReq
		{
			get
			{
				return this.dataObject as CSPKG_CMD_HORNUSE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_HORNUSE stHornUseRsp
		{
			get
			{
				return this.dataObject as SCPKG_CMD_HORNUSE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_ITEMBUY stItemBuyReq
		{
			get
			{
				return this.dataObject as CSPKG_CMD_ITEMBUY;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_ITEMBUY stItemBuyRsp
		{
			get
			{
				return this.dataObject as SCPKG_CMD_ITEMBUY;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_NTF_SHOPTIMEOUT stShopTimeOut
		{
			get
			{
				return this.dataObject as SCPKG_NTF_SHOPTIMEOUT;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_CLRSHOPREFRESH stClrShopRefresh
		{
			get
			{
				return this.dataObject as SCPKG_CMD_CLRSHOPREFRESH;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_SYMBOLCOMP stSymbolCompReq
		{
			get
			{
				return this.dataObject as CSPKG_CMD_SYMBOLCOMP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_SYMBOLCOMP stSymbolCompRsp
		{
			get
			{
				return this.dataObject as SCPKG_CMD_SYMBOLCOMP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_SYMBOLQUERY stSymbolQuery
		{
			get
			{
				return this.dataObject as CSPKG_CMD_SYMBOLQUERY;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_SYMBOLDETAIL stSymbolDetail
		{
			get
			{
				return this.dataObject as SCPKG_CMD_SYMBOLDETAIL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_SYMBOLWEAR stSymbolWear
		{
			get
			{
				return this.dataObject as CSPKG_CMD_SYMBOLWEAR;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_SYMBOLOFF stSymbolOff
		{
			get
			{
				return this.dataObject as CSPKG_CMD_SYMBOLOFF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_SYMBOLCHG stSymbolChg
		{
			get
			{
				return this.dataObject as SCPKG_CMD_SYMBOLCHG;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_SYMBOLPAGESEL stSymbolPageChgReq
		{
			get
			{
				return this.dataObject as CSPKG_CMD_SYMBOLPAGESEL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_SYMBOLPAGESEL stSymbolPageChgRsp
		{
			get
			{
				return this.dataObject as SCPKG_CMD_SYMBOLPAGESEL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_SYMBOLCHG_LIST stSymbolChgList
		{
			get
			{
				return this.dataObject as SCPKG_CMD_SYMBOLCHG_LIST;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_EQUIPSMELT stEquipSmelt
		{
			get
			{
				return this.dataObject as CSPKG_CMD_EQUIPSMELT;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_EQUIPENCHANT stEquipEnchantReq
		{
			get
			{
				return this.dataObject as CSPKG_CMD_EQUIPENCHANT;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_EQUIPENCHANT stEquipEnchantRsp
		{
			get
			{
				return this.dataObject as SCPKG_CMD_EQUIPENCHANT;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_COINDRAW_RESULT stCoinDrawResult
		{
			get
			{
				return this.dataObject as SCPKG_COINDRAW_RESULT;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_GEAR_LVLUP stGearLevelUp
		{
			get
			{
				return this.dataObject as CSPKG_CMD_GEAR_LVLUP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_GEAR_LVLUPALL stGearLvlUpAll
		{
			get
			{
				return this.dataObject as CSPKG_CMD_GEAR_LVLUPALL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_GEAR_LEVELINFO stGearLevelInfo
		{
			get
			{
				return this.dataObject as SCPKG_CMD_GEAR_LEVELINFO;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_GEAR_ADVANCE stGearAdvanceReq
		{
			get
			{
				return this.dataObject as CSPKG_CMD_GEAR_ADVANCE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_GEAR_ADVANCE stGearAdvanceRsp
		{
			get
			{
				return this.dataObject as SCPKG_CMD_GEAR_ADVANCE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_PROPUSE_GIFTGET stGiftUseGet
		{
			get
			{
				return this.dataObject as SCPKG_CMD_PROPUSE_GIFTGET;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_ACNTCOUPONS stAcntCouponsReq
		{
			get
			{
				return this.dataObject as CSPKG_CMD_ACNTCOUPONS;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_ACNTCOUPONS stAcntCouponsRsp
		{
			get
			{
				return this.dataObject as SCPKG_CMD_ACNTCOUPONS;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_SPECIAL_SALEINFO stSPecialSaleDetail
		{
			get
			{
				return this.dataObject as SCPKG_CMD_SPECIAL_SALEINFO;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_SPECSALEBUY stSpecSaleBuyReq
		{
			get
			{
				return this.dataObject as CSPKG_CMD_SPECSALEBUY;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_SPECSALEBUY stSpecSaleBuyRsp
		{
			get
			{
				return this.dataObject as SCPKG_CMD_SPECSALEBUY;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_SYMBOL_MAKE stSymbolMake
		{
			get
			{
				return this.dataObject as CSPKG_CMD_SYMBOL_MAKE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_SYMBOL_BREAK stSymbolBreak
		{
			get
			{
				return this.dataObject as CSPKG_CMD_SYMBOL_BREAK;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_SYMBOL_MAKE stSymbolMakeRsp
		{
			get
			{
				return this.dataObject as SCPKG_CMD_SYMBOL_MAKE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_SYMBOL_BREAK stSymbolBreakRsp
		{
			get
			{
				return this.dataObject as SCPKG_CMD_SYMBOL_BREAK;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_COUPONS_REWARDGET stCouponsRewardReq
		{
			get
			{
				return this.dataObject as CSPKG_CMD_COUPONS_REWARDGET;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_COUPONS_REWARDINFO stCouponsRewardRsp
		{
			get
			{
				return this.dataObject as SCPKG_CMD_COUPONS_REWARDINFO;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_SYMBOLPAGE_CLR stSymbolPageClrReq
		{
			get
			{
				return this.dataObject as CSPKG_CMD_SYMBOLPAGE_CLR;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_SYMBOLPAGE_CLR stSymbolPageClrRsp
		{
			get
			{
				return this.dataObject as SCPKG_CMD_SYMBOLPAGE_CLR;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_TALENT_BUY stTalentBuyReq
		{
			get
			{
				return this.dataObject as CSPKG_CMD_TALENT_BUY;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_TALENT_BUY stTalentBuyRsp
		{
			get
			{
				return this.dataObject as SCPKG_CMD_TALENT_BUY;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_SKILLUNLOCK_SEL stUnlockSkillSelReq
		{
			get
			{
				return this.dataObject as CSPKG_CMD_SKILLUNLOCK_SEL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_SKILLUNLOCK_SEL stUnlockSkillSelRsp
		{
			get
			{
				return this.dataObject as SCPKG_CMD_SKILLUNLOCK_SEL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_HERO_WAKECHG stHeroWakeChg
		{
			get
			{
				return this.dataObject as SCPKG_CMD_HERO_WAKECHG;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_HERO_WAKEOPT stHeroWakeOpt
		{
			get
			{
				return this.dataObject as CSPKG_CMD_HERO_WAKEOPT;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_HERO_WAKESTEP stHeroWakeStep
		{
			get
			{
				return this.dataObject as SCPKG_CMD_HERO_WAKESTEP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_HEROWAKE_REWARD stHeroWakeReward
		{
			get
			{
				return this.dataObject as SCPKG_CMD_HEROWAKE_REWARD;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_PROPUSE stPropUseRsp
		{
			get
			{
				return this.dataObject as SCPKG_CMD_PROPUSE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_SALERECMD_BUY stSaleRecmdBuyReq
		{
			get
			{
				return this.dataObject as CSPKG_CMD_SALERECMD_BUY;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_SALERECMD_BUY stSaleRecmdBuyRsp
		{
			get
			{
				return this.dataObject as SCPKG_CMD_SALERECMD_BUY;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_RANDDRAW_REQ stRandDrawReq
		{
			get
			{
				return this.dataObject as CSPKG_CMD_RANDDRAW_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_RANDDRAW_RSP stRandDrawRsp
		{
			get
			{
				return this.dataObject as SCPKG_CMD_RANDDRAW_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_NTF_RANDDRAW_SYNID stSyncRandDraw
		{
			get
			{
				return this.dataObject as SCPKG_NTF_RANDDRAW_SYNID;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_SYMBOLRCMD_WEAR stSymbolRcmdWearReq
		{
			get
			{
				return this.dataObject as CSPKG_CMD_SYMBOLRCMD_WEAR;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_SYMBOLRCMD_WEAR stSymbolRcmdWearRsp
		{
			get
			{
				return this.dataObject as SCPKG_CMD_SYMBOLRCMD_WEAR;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_SYMBOLRCMD_SEL stSymbolRcmdSelReq
		{
			get
			{
				return this.dataObject as CSPKG_CMD_SYMBOLRCMD_SEL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_SYMBOLRCMD_SEL stSymbolRcmdSelRsp
		{
			get
			{
				return this.dataObject as SCPKG_CMD_SYMBOLRCMD_SEL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_RAREEXCHANGE_REQ stRareExchangeReq
		{
			get
			{
				return this.dataObject as CSPKG_CMD_RAREEXCHANGE_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_RAREEXCHANGE_RSP stRareExchangeRsp
		{
			get
			{
				return this.dataObject as SCPKG_CMD_RAREEXCHANGE_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_NTF_SYMBOLPAGESYN stSymbolPageSyn
		{
			get
			{
				return this.dataObject as SCPKG_NTF_SYMBOLPAGESYN;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_NTF_ERRCODE stNtfErr
		{
			get
			{
				return this.dataObject as SCPKG_NTF_ERRCODE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_NTF_NEWIEBITSYN stNewieBitSyn
		{
			get
			{
				return this.dataObject as SCPKG_NTF_NEWIEBITSYN;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_NTF_NEWIEALLBITSYN stNewieAllBitSyn
		{
			get
			{
				return this.dataObject as SCPKG_NTF_NEWIEALLBITSYN;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_CHG_SIGNATURE stSignatureReq
		{
			get
			{
				return this.dataObject as CSPKG_CMD_CHG_SIGNATURE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_CHG_SIGNATURE stSignatureRsp
		{
			get
			{
				return this.dataObject as SCPKG_CMD_CHG_SIGNATURE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_LIST_FRIEND stFriendListReq
		{
			get
			{
				return this.dataObject as CSPKG_CMD_LIST_FRIEND;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_LIST_FRIEND stFriendListRsp
		{
			get
			{
				return this.dataObject as SCPKG_CMD_LIST_FRIEND;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_LIST_FRIENDREQ stFriendReqListReq
		{
			get
			{
				return this.dataObject as CSPKG_CMD_LIST_FRIENDREQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_LIST_FRIENDREQ stFriendReqListRsp
		{
			get
			{
				return this.dataObject as SCPKG_CMD_LIST_FRIENDREQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_SEARCH_PLAYER stFriendSearchPlayerReq
		{
			get
			{
				return this.dataObject as CSPKG_CMD_SEARCH_PLAYER;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_SEARCH_PLAYER stFriendSearchPlayerRsp
		{
			get
			{
				return this.dataObject as SCPKG_CMD_SEARCH_PLAYER;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_ADD_FRIEND stFriendAddReq
		{
			get
			{
				return this.dataObject as CSPKG_CMD_ADD_FRIEND;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_ADD_FRIEND stFriendAddRsp
		{
			get
			{
				return this.dataObject as SCPKG_CMD_ADD_FRIEND;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_DEL_FRIEND stFriendDelReq
		{
			get
			{
				return this.dataObject as CSPKG_CMD_DEL_FRIEND;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_DEL_FRIEND stFriendDelRsp
		{
			get
			{
				return this.dataObject as SCPKG_CMD_DEL_FRIEND;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_ADD_FRIEND_CONFIRM stFriendAddConfirmReq
		{
			get
			{
				return this.dataObject as CSPKG_CMD_ADD_FRIEND_CONFIRM;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_ADD_FRIEND_CONFIRM stFriendAddConfirmRsp
		{
			get
			{
				return this.dataObject as SCPKG_CMD_ADD_FRIEND_CONFIRM;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_ADD_FRIEND_DENY stFriendAddDenyReq
		{
			get
			{
				return this.dataObject as CSPKG_CMD_ADD_FRIEND_DENY;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_ADD_FRIEND_DENY stFriendAddDenyRsp
		{
			get
			{
				return this.dataObject as SCPKG_CMD_ADD_FRIEND_DENY;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_INVITE_GAME stFriendInviteGameReq
		{
			get
			{
				return this.dataObject as CSPKG_CMD_INVITE_GAME;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_INVITE_GAME stFriendInviteGameRsp
		{
			get
			{
				return this.dataObject as SCPKG_CMD_INVITE_GAME;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_INVITE_RECEIVE_ACHIEVE stFriendReceiveAchieveReq
		{
			get
			{
				return this.dataObject as CSPKG_CMD_INVITE_RECEIVE_ACHIEVE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_INVITE_RECEIVE_ACHIEVE stFriendReceiveAchieveRsp
		{
			get
			{
				return this.dataObject as SCPKG_CMD_INVITE_RECEIVE_ACHIEVE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_DONATE_FRIEND_POINT stFriendDonatePointReq
		{
			get
			{
				return this.dataObject as CSPKG_CMD_DONATE_FRIEND_POINT;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_DONATE_FRIEND_POINT stFriendDonatePointRsp
		{
			get
			{
				return this.dataObject as SCPKG_CMD_DONATE_FRIEND_POINT;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_DONATE_FRIEND_POINT_ALL stFriendDonatePointAllReq
		{
			get
			{
				return this.dataObject as CSPKG_CMD_DONATE_FRIEND_POINT_ALL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_DONATE_FRIEND_POINT_ALL stFriendDonatePointAllRsp
		{
			get
			{
				return this.dataObject as SCPKG_CMD_DONATE_FRIEND_POINT_ALL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_NTF_CHG_INTIMACY stNtfChgIntimacy
		{
			get
			{
				return this.dataObject as SCPKG_CMD_NTF_CHG_INTIMACY;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_NOASKFORFLAG_CHG stAcntAskforFlagReq
		{
			get
			{
				return this.dataObject as CSPKG_CMD_NOASKFORFLAG_CHG;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_NOASKFORFLAG_CHG stAcntAskforflagRsp
		{
			get
			{
				return this.dataObject as SCPKG_CMD_NOASKFORFLAG_CHG;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_NTF_FRIEND_NOASKFOR_FLAGCHG stNtfFriendNoAskforFlag
		{
			get
			{
				return this.dataObject as SCPKG_NTF_FRIEND_NOASKFOR_FLAGCHG;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_GET_INVITE_INFO stFriendInviteInfoReq
		{
			get
			{
				return this.dataObject as CSPKG_CMD_GET_INVITE_INFO;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_GET_INVITE_INFO stFriendInviteInfoRSP
		{
			get
			{
				return this.dataObject as SCPKG_CMD_GET_INVITE_INFO;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_NTF_FRIEND_REQUEST stNtfFriendRequest
		{
			get
			{
				return this.dataObject as SCPKG_CMD_NTF_FRIEND_REQUEST;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_NTF_FRIEND_ADD stNtfFriendAdd
		{
			get
			{
				return this.dataObject as SCPKG_CMD_NTF_FRIEND_ADD;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_NTF_FRIEND_DEL stNtfFriendDel
		{
			get
			{
				return this.dataObject as SCPKG_CMD_NTF_FRIEND_DEL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_NTF_FRIEND_LOGIN_STATUS stNtfFriendLoginStatus
		{
			get
			{
				return this.dataObject as SCPKG_CMD_NTF_FRIEND_LOGIN_STATUS;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_LIST_FREC stFRecReq
		{
			get
			{
				return this.dataObject as CSPKG_CMD_LIST_FREC;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_LIST_FREC stFRecRsp
		{
			get
			{
				return this.dataObject as SCPKG_CMD_LIST_FREC;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_RECALL_FRIEND stFriendRecallReq
		{
			get
			{
				return this.dataObject as CSPKG_CMD_RECALL_FRIEND;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_RECALL_FRIEND stFriendRecallRsp
		{
			get
			{
				return this.dataObject as SCPKG_CMD_RECALL_FRIEND;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_NTF_RECALL_FRIEND stNtfRecallFirend
		{
			get
			{
				return this.dataObject as SCPKG_CMD_NTF_RECALL_FRIEND;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_BLACKLIST stBlackListProfile
		{
			get
			{
				return this.dataObject as SCPKG_CMD_BLACKLIST;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_OPER_HERO_REQ stOperHeroReq
		{
			get
			{
				return this.dataObject as CSPKG_OPER_HERO_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_OPER_HERO_NTF stOperHeroRsp
		{
			get
			{
				return this.dataObject as SCPKG_OPER_HERO_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CONFIRM_HERO stConfirmHero
		{
			get
			{
				return this.dataObject as CSPKG_CONFIRM_HERO;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CONFIRM_HERO_NTF stConfirmHeroNtf
		{
			get
			{
				return this.dataObject as SCPKG_CONFIRM_HERO_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_DEFAULT_HERO_NTF stDefaultHeroNtf
		{
			get
			{
				return this.dataObject as SCPKG_DEFAULT_HERO_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_HERO_CANCEL_CONFIRM stHeroCancelConfirm
		{
			get
			{
				return this.dataObject as CSPKG_HERO_CANCEL_CONFIRM;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_HERO_CANCEL_CONFIRM_NTF stHeroCancelConfirmNtf
		{
			get
			{
				return this.dataObject as SCPKG_HERO_CANCEL_CONFIRM_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_SHOW_HERO_WIN_RATIO stShowHeroWinRatio
		{
			get
			{
				return this.dataObject as CSPKG_SHOW_HERO_WIN_RATIO;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_SHOW_HERO_WIN_RATIO_NTF stShowHeroWinRatioNtf
		{
			get
			{
				return this.dataObject as SCPKG_SHOW_HERO_WIN_RATIO_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_RELAYSVRPING stRelaySvrPing
		{
			get
			{
				return this.dataObject as CSPKG_RELAYSVRPING;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_GAMESVRPING stGameSvrPing
		{
			get
			{
				return this.dataObject as CSPKG_GAMESVRPING;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CLRCDLIMIT_REQ stClrCdLimitReq
		{
			get
			{
				return this.dataObject as CSPKG_CLRCDLIMIT_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CLRCDLIMIT_RSP stClrCdLimitRsp
		{
			get
			{
				return this.dataObject as SCPKG_CLRCDLIMIT_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_RELAYHASHCHECK stRelayHashChk
		{
			get
			{
				return this.dataObject as CSPKG_RELAYHASHCHECK;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_NEXTFIRSTWINSEC_NTF stNextFirstWinSecNtf
		{
			get
			{
				return this.dataObject as CSPKG_NEXTFIRSTWINSEC_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_COINGETPATH_REQ stCoinGetPathReq
		{
			get
			{
				return this.dataObject as CSPKG_COINGETPATH_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_COINGETPATH_RSP stCoinGetPathRsp
		{
			get
			{
				return this.dataObject as SCPKG_COINGETPATH_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_RELAYHASHCHECK stRelayHashChkRsp
		{
			get
			{
				return this.dataObject as SCPKG_RELAYHASHCHECK;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_CHAT_REQ stChatReq
		{
			get
			{
				return this.dataObject as CSPKG_CMD_CHAT_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_GET_CHAT_MSG_REQ stGetChatMsgReq
		{
			get
			{
				return this.dataObject as CSPKG_CMD_GET_CHAT_MSG_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_CHAT_NTF stChatNtf
		{
			get
			{
				return this.dataObject as SCPKG_CMD_CHAT_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CHAT_COMPLAINT_REQ stComplaintChatReq
		{
			get
			{
				return this.dataObject as CSPKG_CHAT_COMPLAINT_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CHAT_COMPLAINT_RSP stComplaintChatRsp
		{
			get
			{
				return this.dataObject as SCPKG_CHAT_COMPLAINT_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_GET_HORNMSG stGetHornMsgReq
		{
			get
			{
				return this.dataObject as CSPKG_CMD_GET_HORNMSG;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_GET_HORNMSG stGetHornMsgRsp
		{
			get
			{
				return this.dataObject as SCPKG_CMD_GET_HORNMSG;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_OFFLINE_CHAT_NTF stOfflineChatNtf
		{
			get
			{
				return this.dataObject as SCPKG_OFFLINE_CHAT_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CLEAN_OFFLINE_CHAT_REQ stCleanOfflineChatReq
		{
			get
			{
				return this.dataObject as CSPKG_CLEAN_OFFLINE_CHAT_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_LEAVE_SETTLEUI_NTF stLeaveSettleUiNtf
		{
			get
			{
				return this.dataObject as SCPKG_LEAVE_SETTLEUI_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_GAIN_CHEST stGainChestReq
		{
			get
			{
				return this.dataObject as CSPKG_CMD_GAIN_CHEST;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_GAIN_CHEST stGainChestRsp
		{
			get
			{
				return this.dataObject as SCPKG_CMD_GAIN_CHEST;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_FRIEND_REFUSE_RECALL stRefuseRecallReq
		{
			get
			{
				return this.dataObject as CSPKG_CMD_FRIEND_REFUSE_RECALL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_NTF_REFUSE_RECALL stNtfRefuseRecall
		{
			get
			{
				return this.dataObject as SCPKG_CMD_NTF_REFUSE_RECALL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_FRIEND_REFUSE_RECALL stRefuseRecallRsp
		{
			get
			{
				return this.dataObject as SCPKG_CMD_FRIEND_REFUSE_RECALL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_DEFRIEND stDeFriendReq
		{
			get
			{
				return this.dataObject as CSPKG_CMD_DEFRIEND;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_DEFRIEND stDeFriendRsp
		{
			get
			{
				return this.dataObject as SCPKG_CMD_DEFRIEND;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_CANCEL_DEFRIEND stCancelDeFriendReq
		{
			get
			{
				return this.dataObject as CSPKG_CMD_CANCEL_DEFRIEND;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_CANCEL_DEFRIEND stCancelDeFriendRsp
		{
			get
			{
				return this.dataObject as SCPKG_CMD_CANCEL_DEFRIEND;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_LBS_REPORT stLbsReportReq
		{
			get
			{
				return this.dataObject as CSPKG_CMD_LBS_REPORT;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_LBS_SEARCH stLbsSearchReq
		{
			get
			{
				return this.dataObject as CSPKG_CMD_LBS_SEARCH;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_LBS_SEARCH stLbsSearchRsq
		{
			get
			{
				return this.dataObject as SCPKG_CMD_LBS_SEARCH;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_LBS_REMOVE stLbsRemoveReq
		{
			get
			{
				return this.dataObject as CSPKG_CMD_LBS_REMOVE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_LIKE_REQ stLikeReq
		{
			get
			{
				return this.dataObject as CSPKG_CMD_LIKE_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_NTF_LIKE stLikeNtf
		{
			get
			{
				return this.dataObject as SCPKG_CMD_NTF_LIKE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_LICENSE_REQ stLicenseGetReq
		{
			get
			{
				return this.dataObject as CSPKG_CMD_LICENSE_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_LICENSE_RSP stLicenseGetRsp
		{
			get
			{
				return this.dataObject as SCPKG_CMD_LICENSE_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_INTIMACY_RELATION_REQUEST stIntimacyRelationRequestReq
		{
			get
			{
				return this.dataObject as CSPKG_CMD_INTIMACY_RELATION_REQUEST;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_INTIMACY_RELATION_REQUEST stIntimacyRelationRequestRsp
		{
			get
			{
				return this.dataObject as SCPKG_CMD_INTIMACY_RELATION_REQUEST;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_NTF_INTIMACY_RELATION_REQUEST stNtfIntimacyRelationRequest
		{
			get
			{
				return this.dataObject as SCPKG_CMD_NTF_INTIMACY_RELATION_REQUEST;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_CHG_INTIMACY_CONFIRM stChgIntimacyConfirmReq
		{
			get
			{
				return this.dataObject as CSPKG_CMD_CHG_INTIMACY_CONFIRM;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_CHG_INTIMACY_CONFIRM stChgIntimacyConfirmRsp
		{
			get
			{
				return this.dataObject as SCPKG_CMD_CHG_INTIMACY_CONFIRM;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_NTF_CHG_INTIMACY_CONFIRM stNtfChgIntimacyConfirm
		{
			get
			{
				return this.dataObject as SCPKG_CMD_NTF_CHG_INTIMACY_CONFIRM;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_CHG_INTIMACY_DENY stChgIntimacyDenyReq
		{
			get
			{
				return this.dataObject as CSPKG_CMD_CHG_INTIMACY_DENY;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_CHG_INTIMACY_DENY stChgIntimacyDenyRsp
		{
			get
			{
				return this.dataObject as SCPKG_CMD_CHG_INTIMACY_DENY;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_NTF_CHG_INTIMACY_DENY stNtfChgIntimacyDeny
		{
			get
			{
				return this.dataObject as SCPKG_CMD_NTF_CHG_INTIMACY_DENY;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_CHG_INTIMACYPRIORITY stChgIntimacyPriorityReq
		{
			get
			{
				return this.dataObject as CSPKG_CMD_CHG_INTIMACYPRIORITY;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_RECRUITMENT_REWARD_REQ stRecruitmentRewardReq
		{
			get
			{
				return this.dataObject as CSPKG_RECRUITMENT_REWARD_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_RECRUITMENT_REWARD_RSP stRecruitmentRewardRsp
		{
			get
			{
				return this.dataObject as SCPKG_RECRUITMENT_REWARD_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_RECRUITMENT_REWARD_ERR_NTF stRecruitmentRewardErrNtf
		{
			get
			{
				return this.dataObject as SCPKG_RECRUITMENT_REWARD_ERR_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_RECRUITMENT_ERR_NTF stRecruitmentErrNtf
		{
			get
			{
				return this.dataObject as SCPKG_RECRUITMENT_ERR_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CHG_RECRUITMENT_NTF stChgRecruitmentNtf
		{
			get
			{
				return this.dataObject as SCPKG_CHG_RECRUITMENT_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_ASKFORREQ_GET stAskforReqGetReq
		{
			get
			{
				return this.dataObject as CSPKG_CMD_ASKFORREQ_GET;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_ASKFORREQ_GET stAskforReqGetRsp
		{
			get
			{
				return this.dataObject as SCPKG_CMD_ASKFORREQ_GET;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_ASKFORREQ_GETFAIL stAskforReqGetFail
		{
			get
			{
				return this.dataObject as SCPKG_CMD_ASKFORREQ_GETFAIL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_ASKFORREQ_DEL stAskforReqDelReq
		{
			get
			{
				return this.dataObject as CSPKG_CMD_ASKFORREQ_DEL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_ASKFORREQ_DEL stAskforReqDelRsp
		{
			get
			{
				return this.dataObject as SCPKG_CMD_ASKFORREQ_DEL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_ASKFORREQ_SEND stAskforReqSendReq
		{
			get
			{
				return this.dataObject as CSPKG_CMD_ASKFORREQ_SEND;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_ASKFORREQ_SEND stAskforReqSendRsp
		{
			get
			{
				return this.dataObject as SCPKG_CMD_ASKFORREQ_SEND;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_ASKFORREQ_READ stAskforReqReadReq
		{
			get
			{
				return this.dataObject as CSPKG_CMD_ASKFORREQ_READ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_ASKFORREQ_READ stAskforReqReadRsp
		{
			get
			{
				return this.dataObject as SCPKG_CMD_ASKFORREQ_READ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_ASKFORREQ_REFUSE stAskforRefuseReq
		{
			get
			{
				return this.dataObject as CSPKG_CMD_ASKFORREQ_REFUSE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_ASKFORREQ_REFUSE stAskforRefuseRsp
		{
			get
			{
				return this.dataObject as SCPKG_CMD_ASKFORREQ_REFUSE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CMD_ASKFORREQ_CONFIRM stAskforConfirmReq
		{
			get
			{
				return this.dataObject as CSPKG_CMD_ASKFORREQ_CONFIRM;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_ASKFORREQ_CONFIRM stAskforConfirmRsp
		{
			get
			{
				return this.dataObject as SCPKG_CMD_ASKFORREQ_CONFIRM;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_MAILOPT_REQ stMailOptReq
		{
			get
			{
				return this.dataObject as CSPKG_MAILOPT_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_MAILOPT_RES stMailOptRes
		{
			get
			{
				return this.dataObject as SCPKG_MAILOPT_RES;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_FUNCUNLOCK_REQ stFuncUnlockReq
		{
			get
			{
				return this.dataObject as CSPKG_FUNCUNLOCK_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_ACNTDETAILINFO_RSP stAcntDetailInfoRsp
		{
			get
			{
				return this.dataObject as SCPKG_ACNTDETAILINFO_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_ACNT_HEAD_URL_CHG_NTF stAcntHeadUrlChgNtf
		{
			get
			{
				return this.dataObject as SCPKG_ACNT_HEAD_URL_CHG_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_ACNTSELFMSGINFO stAcntSelfMsgInfo
		{
			get
			{
				return this.dataObject as SCPKG_ACNTSELFMSGINFO;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_AKALISHOP_INFO stAkaliShopInfo
		{
			get
			{
				return this.dataObject as SCPKG_AKALISHOP_INFO;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_AKALISHOP_BUYREQ stAkaliShopBuyReq
		{
			get
			{
				return this.dataObject as CSPKG_AKALISHOP_BUYREQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_AKALISHOP_BUYRSP stAkaliShopBuyRsp
		{
			get
			{
				return this.dataObject as SCPKG_AKALISHOP_BUYRSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_AKALISHOP_FLAGREQ stAkaliShopFlagReq
		{
			get
			{
				return this.dataObject as CSPKG_AKALISHOP_FLAGREQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_AKALISHOP_FLAGRSP stAkaliShopFlagRsp
		{
			get
			{
				return this.dataObject as SCPKG_AKALISHOP_FLAGRSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_HONORINFOCHG_RSP stHonorInfoChgRsp
		{
			get
			{
				return this.dataObject as SCPKG_HONORINFOCHG_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_HONORINFO_RSP stHonorInfoRsp
		{
			get
			{
				return this.dataObject as SCPKG_HONORINFO_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_USEHONOR_REQ stUseHonorReq
		{
			get
			{
				return this.dataObject as CSPKG_USEHONOR_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_USEHONOR_RSP stUseHonorRsp
		{
			get
			{
				return this.dataObject as SCPKG_USEHONOR_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_NOTICENEW_REQ stNoticeNewReq
		{
			get
			{
				return this.dataObject as CSPKG_NOTICENEW_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_NOTICENEW_RSP stNoticeNewRsp
		{
			get
			{
				return this.dataObject as SCPKG_NOTICENEW_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_NOTICELIST_REQ stNoticeListReq
		{
			get
			{
				return this.dataObject as CSPKG_NOTICELIST_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_NOTICELIST_RSP stNoticeListRsp
		{
			get
			{
				return this.dataObject as SCPKG_NOTICELIST_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_NOTICEINFO_REQ stNoticeInfoReq
		{
			get
			{
				return this.dataObject as CSPKG_NOTICEINFO_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_NOTICEINFO_RSP stNoticeInfoRsp
		{
			get
			{
				return this.dataObject as SCPKG_NOTICEINFO_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_SEND_GUILD_MAIL_REQ stSendGuildMailReq
		{
			get
			{
				return this.dataObject as CSPKG_SEND_GUILD_MAIL_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_SEND_GUILD_MAIL_RSP stSendGuildMailRsp
		{
			get
			{
				return this.dataObject as SCPKG_SEND_GUILD_MAIL_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_FIGHTHISTORY_REQ stFightHistoryReq
		{
			get
			{
				return this.dataObject as CSPKG_FIGHTHISTORY_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_FIGHTHISTORYLIST_REQ stFightHistoryListReq
		{
			get
			{
				return this.dataObject as CSPKG_FIGHTHISTORYLIST_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_FIGHTHISTORYLIST_RSP stFightHistoryListRsp
		{
			get
			{
				return this.dataObject as SCPKG_FIGHTHISTORYLIST_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_ROLLINGMSG_NTF stRollingMsgNtf
		{
			get
			{
				return this.dataObject as SCPKG_ROLLINGMSG_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_REDDOTLIST_REQ stRedDotListReq
		{
			get
			{
				return this.dataObject as CSPKG_REDDOTLIST_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_REDDOTLIST_RSP stRedDotListRsp
		{
			get
			{
				return this.dataObject as SCPKG_REDDOTLIST_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CHKXUNYOUSERV_REQ stChkXunyouServ
		{
			get
			{
				return this.dataObject as CSPKG_CHKXUNYOUSERV_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_USUALTASK_REQ stUsualTaskReq
		{
			get
			{
				return this.dataObject as CSPKG_USUALTASK_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_USUALTASK_RES stUsualTaskRes
		{
			get
			{
				return this.dataObject as SCPKG_USUALTASK_RES;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_TASKSUBMIT_REQ stSubmitTaskReq
		{
			get
			{
				return this.dataObject as CSPKG_TASKSUBMIT_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_TASKSUBMIT_RES stSubmitTaskRes
		{
			get
			{
				return this.dataObject as SCPKG_TASKSUBMIT_RES;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_TASKUPD_NTF stTaskUdpNtf
		{
			get
			{
				return this.dataObject as SCPKG_TASKUPD_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_TASKDONE_CLIENTREPORT stClientReportTaskDone
		{
			get
			{
				return this.dataObject as CSPKG_TASKDONE_CLIENTREPORT;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_USUALTASKDISCARD_RES stUsualDiscardTaskRes
		{
			get
			{
				return this.dataObject as SCPKG_USUALTASKDISCARD_RES;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_NEWTASKGET_NTF stNewTaskGet
		{
			get
			{
				return this.dataObject as SCPKG_NEWTASKGET_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_DELTASK_NTF stDelTask
		{
			get
			{
				return this.dataObject as SCPKG_DELTASK_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_NTF_HUOYUEDUINFO stNtfHuoYueDuInfo
		{
			get
			{
				return this.dataObject as SCPKG_NTF_HUOYUEDUINFO;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_GETHUOYUEDUREWARD_REQ stHuoYueDuRewardReq
		{
			get
			{
				return this.dataObject as CSPKG_GETHUOYUEDUREWARD_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_GETHUOYUEDUREWARD_RSP stHuoYueDuRewardRsp
		{
			get
			{
				return this.dataObject as SCPKG_GETHUOYUEDUREWARD_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_HUOYUEDUREWARDERR_NTF stHuoYueDuRewardErr
		{
			get
			{
				return this.dataObject as SCPKG_HUOYUEDUREWARDERR_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_GETPVPLEVELREWARD_REQ stLevelRewardReq
		{
			get
			{
				return this.dataObject as CSPKG_GETPVPLEVELREWARD_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_GETPVPLEVELREWARD_RSP stLevelRewardRsp
		{
			get
			{
				return this.dataObject as SCPKG_GETPVPLEVELREWARD_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_ACHIEVEHERO_REQ stAchieveHeroReq
		{
			get
			{
				return this.dataObject as CSPKG_ACHIEVEHERO_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_ACHIEVEHERO_RSP stAchieveHeroRsp
		{
			get
			{
				return this.dataObject as SCPKG_ACHIEVEHERO_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_ACNTHEROINFO_NTY stAcntHeroInfoNty
		{
			get
			{
				return this.dataObject as SCPKG_ACNTHEROINFO_NTY;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_HEROEXP_ADD stHeroExpAdd
		{
			get
			{
				return this.dataObject as SCPKG_CMD_HEROEXP_ADD;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_ADDHERO_NTY stAddHeroNty
		{
			get
			{
				return this.dataObject as SCPKG_ADDHERO_NTY;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_BATTLELIST_REQ stBattleListReq
		{
			get
			{
				return this.dataObject as CSPKG_BATTLELIST_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_BATTLELIST_RSP stBattleListRsp
		{
			get
			{
				return this.dataObject as SCPKG_BATTLELIST_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_BATTLELIST_NTY stBattleListNtf
		{
			get
			{
				return this.dataObject as SCPKG_BATTLELIST_NTY;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_UPGRADESTAR_REQ stUpgradeStarReq
		{
			get
			{
				return this.dataObject as CSPKG_UPGRADESTAR_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_UPGRADESTAR_RSP stUpgradeStarRsp
		{
			get
			{
				return this.dataObject as SCPKG_UPGRADESTAR_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_NTF_HERO_INFO_UPD stHeroInfoUpdNtf
		{
			get
			{
				return this.dataObject as SCPKG_NTF_HERO_INFO_UPD;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_SKILLUPDATE_REQ stSkillUpdateReq
		{
			get
			{
				return this.dataObject as CSPKG_SKILLUPDATE_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_SKILLUPDATE_RSP stSkillUpdateRsp
		{
			get
			{
				return this.dataObject as SCPKG_SKILLUPDATE_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_ACHIEVEHERO_RSP stGMAddHeroRsp
		{
			get
			{
				return this.dataObject as SCPKG_ACHIEVEHERO_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_FREEHERO_REQ stFreeHeroReq
		{
			get
			{
				return this.dataObject as CSPKG_FREEHERO_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_FREEHERO_RSP stFreeHeroRsp
		{
			get
			{
				return this.dataObject as SCPKG_FREEHERO_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_GMUNLOGCKHEROPVP_RSP stGMUnlockHeroPVPRsp
		{
			get
			{
				return this.dataObject as SCPKG_GMUNLOGCKHEROPVP_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_BUYHERO_REQ stBuyHeroReq
		{
			get
			{
				return this.dataObject as CSPKG_BUYHERO_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_BUYHERO_RSP stBuyHeroRsp
		{
			get
			{
				return this.dataObject as SCPKG_BUYHERO_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_BUYHEROSKIN_REQ stBuyHeroSkinReq
		{
			get
			{
				return this.dataObject as CSPKG_BUYHEROSKIN_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_BUYHEROSKIN_RSP stBuyHeroSkinRsp
		{
			get
			{
				return this.dataObject as SCPKG_BUYHEROSKIN_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_WEARHEROSKIN_REQ stWearHeroSkinReq
		{
			get
			{
				return this.dataObject as CSPKG_WEARHEROSKIN_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_WEARHEROSKIN_RSP stWearHeroSkinRsp
		{
			get
			{
				return this.dataObject as SCPKG_WEARHEROSKIN_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_HEROSKIN_ADD stHeroSkinAdd
		{
			get
			{
				return this.dataObject as SCPKG_HEROSKIN_ADD;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_UPHEROLVL_REQ stUpHeroLvlReq
		{
			get
			{
				return this.dataObject as CSPKG_UPHEROLVL_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_UPHEROLVL_RSP stUpHeroLvlRsp
		{
			get
			{
				return this.dataObject as SCPKG_UPHEROLVL_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_LIMITSKIN_ADD stLimitSkinAdd
		{
			get
			{
				return this.dataObject as SCPKG_LIMITSKIN_ADD;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_LIMITSKIN_DEL stLimitSkinDel
		{
			get
			{
				return this.dataObject as SCPKG_LIMITSKIN_DEL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_USEEXPCARD_NTF stUseExpCardNtf
		{
			get
			{
				return this.dataObject as SCPKG_USEEXPCARD_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_GM_ADDALLSKIN_RSP stGMAddAllSkillRsp
		{
			get
			{
				return this.dataObject as SCPKG_GM_ADDALLSKIN_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_PRESENTHERO_REQ stPresentHeroReq
		{
			get
			{
				return this.dataObject as CSPKG_PRESENTHERO_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_PRESENTHERO_RSP stPresentHeroRsp
		{
			get
			{
				return this.dataObject as SCPKG_PRESENTHERO_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_PRESENTSKIN_REQ stPresentSkinReq
		{
			get
			{
				return this.dataObject as CSPKG_PRESENTSKIN_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_PERSENTSKIN_RSP stPresentSkinRsp
		{
			get
			{
				return this.dataObject as SCPKG_PERSENTSKIN_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_ACNT_PSWDINFO_OPEN stAcntPswdOpenReq
		{
			get
			{
				return this.dataObject as CSPKG_ACNT_PSWDINFO_OPEN;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_ACNT_PSWDINFO_OPEN stAcntPswdOpenRsp
		{
			get
			{
				return this.dataObject as SCPKG_ACNT_PSWDINFO_OPEN;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_ACNT_PSWDINFO_CLOSE stAcntPswdCloseReq
		{
			get
			{
				return this.dataObject as CSPKG_ACNT_PSWDINFO_CLOSE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_ACNT_PSWDINFO_CLOSE stAcntPswdCloseRsp
		{
			get
			{
				return this.dataObject as SCPKG_ACNT_PSWDINFO_CLOSE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_ACNT_PSWDINFO_CHG stAcntPswdChgReq
		{
			get
			{
				return this.dataObject as CSPKG_ACNT_PSWDINFO_CHG;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_ACNT_PSWDINFO_CHG stAcntPswdChgRsp
		{
			get
			{
				return this.dataObject as SCPKG_ACNT_PSWDINFO_CHG;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_ACNT_PSWDINFO_FORCE stAcntPswdForceReq
		{
			get
			{
				return this.dataObject as CSPKG_ACNT_PSWDINFO_FORCE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_ACNT_PSWDINFO_FORCE stAcntPswdForceRsp
		{
			get
			{
				return this.dataObject as SCPKG_ACNT_PSWDINFO_FORCE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_ACNT_PSWDINFO_FORCECAL stAcntPswdFroceCalReq
		{
			get
			{
				return this.dataObject as CSPKG_ACNT_PSWDINFO_FORCECAL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_ACNT_PSWDINFO_FORCECAL stAcntPswdFroceCalRsp
		{
			get
			{
				return this.dataObject as SCPKG_ACNT_PSWDINFO_FORCECAL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_MATCH_REQ stMatchReq
		{
			get
			{
				return this.dataObject as CSPKG_MATCH_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_MATCH_RSP stMatchRsp
		{
			get
			{
				return this.dataObject as SCPKG_MATCH_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_ROOM_STARTSINGLEGAME_NTF stRoomStateSingleGameNtf
		{
			get
			{
				return this.dataObject as SCPKG_ROOM_STARTSINGLEGAME_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_START_MULTI_GAME_REQ stStartMultiGameReq
		{
			get
			{
				return this.dataObject as CSPKG_START_MULTI_GAME_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_START_MULTI_GAME_RSP stStartMultiGameRsp
		{
			get
			{
				return this.dataObject as SCPKG_START_MULTI_GAME_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_ADD_NPC_REQ stAddNpcReq
		{
			get
			{
				return this.dataObject as CSPKG_ADD_NPC_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_INVITE_FRIEND_JOIN_ROOM_REQ stInviteFriendJoinRoomReq
		{
			get
			{
				return this.dataObject as CSPKG_INVITE_FRIEND_JOIN_ROOM_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_INVITE_FRIEND_JOIN_ROOM_RSP stInviteFriendJoinRoomRsp
		{
			get
			{
				return this.dataObject as SCPKG_INVITE_FRIEND_JOIN_ROOM_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_KICKOUT_ROOMMEMBER_REQ stKickoutRoomMemberReq
		{
			get
			{
				return this.dataObject as CSPKG_KICKOUT_ROOMMEMBER_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_INVITE_JOIN_GAME_REQ stInviteJoinGameReq
		{
			get
			{
				return this.dataObject as SCPKG_INVITE_JOIN_GAME_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_INVITE_JOIN_GAME_RSP stInviteJoinGameRsp
		{
			get
			{
				return this.dataObject as CSPKG_INVITE_JOIN_GAME_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CREATE_TEAM_REQ stCreateTeamReq
		{
			get
			{
				return this.dataObject as CSPKG_CREATE_TEAM_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_JOIN_TEAM_RSP stJoinTeamRsp
		{
			get
			{
				return this.dataObject as SCPKG_JOIN_TEAM_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_INVITE_FRIEND_JOIN_TEAM_REQ stInviteFriendJoinTeamReq
		{
			get
			{
				return this.dataObject as CSPKG_INVITE_FRIEND_JOIN_TEAM_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_INVITE_FRIEND_JOIN_TEAM_RSP stInviteFriendJoinTeamRsp
		{
			get
			{
				return this.dataObject as SCPKG_INVITE_FRIEND_JOIN_TEAM_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_TEAM_CHG stTeamChgNtf
		{
			get
			{
				return this.dataObject as SCPKG_TEAM_CHG;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_LEAVL_TEAM stLeaveTeam
		{
			get
			{
				return this.dataObject as CSPKG_LEAVL_TEAM;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_OPER_TEAM_REQ stOperTeamReq
		{
			get
			{
				return this.dataObject as CSPKG_OPER_TEAM_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_SELF_BEKICK_FROM_TEAM stSelfBeKickFromTeam
		{
			get
			{
				return this.dataObject as SCPKG_SELF_BEKICK_FROM_TEAM;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_ACNT_LEAVE_TEAM_RSP stAcntLeaveRsp
		{
			get
			{
				return this.dataObject as SCPKG_ACNT_LEAVE_TEAM_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_ROOM_CONFIRM_REQ stRoomConfirmReq
		{
			get
			{
				return this.dataObject as CSPKG_ROOM_CONFIRM_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_ROOM_CONFIRM_RSP stRoomConfirmRsp
		{
			get
			{
				return this.dataObject as SCPKG_ROOM_CONFIRM_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_ROOM_CHGMEMBERPOS_REQ stChgMemberPosReq
		{
			get
			{
				return this.dataObject as CSPKG_ROOM_CHGMEMBERPOS_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_INVITE_GUILD_MEMBER_JOIN_REQ stInviteGuildMemberJoinReq
		{
			get
			{
				return this.dataObject as CSPKG_INVITE_GUILD_MEMBER_JOIN_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_GET_GUILD_MEMBER_GAME_STATE_REQ stGetGuildMemberGameStateReq
		{
			get
			{
				return this.dataObject as CSPKG_GET_GUILD_MEMBER_GAME_STATE_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_GET_GUILD_MEMBER_GAME_STATE_RSP stGetGuildMemberGameStateRsp
		{
			get
			{
				return this.dataObject as SCPKG_GET_GUILD_MEMBER_GAME_STATE_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_ROOM_CHGPOS_CONFIRM_REQ stChgRoomPosConfirmReq
		{
			get
			{
				return this.dataObject as CSPKG_ROOM_CHGPOS_CONFIRM_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_ROOM_CHGPOS_NTF stChgRoomPosNtf
		{
			get
			{
				return this.dataObject as SCPKG_ROOM_CHGPOS_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_GET_PREPARE_GUILD_LIST_REQ stGetPrepareGuildListReq
		{
			get
			{
				return this.dataObject as CSPKG_GET_PREPARE_GUILD_LIST_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_GET_PREPARE_GUILD_LIST_RSP stGetPrepareGuildListRsp
		{
			get
			{
				return this.dataObject as SCPKG_GET_PREPARE_GUILD_LIST_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_GET_GUILD_INFO_REQ stGetGuildInfoReq
		{
			get
			{
				return this.dataObject as CSPKG_GET_GUILD_INFO_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_GET_GUILD_INFO_RSP stGetGuildInfoRsp
		{
			get
			{
				return this.dataObject as SCPKG_GET_GUILD_INFO_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_GET_PREPARE_GUILD_INFO_REQ stGetPrepareGuildInfoReq
		{
			get
			{
				return this.dataObject as CSPKG_GET_PREPARE_GUILD_INFO_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_GET_PREPARE_GUILD_INFO_RSP stGetPrepareGuildInfoRsp
		{
			get
			{
				return this.dataObject as SCPKG_GET_PREPARE_GUILD_INFO_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CREATE_GUILD_REQ stCreateGuildReq
		{
			get
			{
				return this.dataObject as CSPKG_CREATE_GUILD_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CREATE_GUILD_RSP stCreateGuildRsp
		{
			get
			{
				return this.dataObject as SCPKG_CREATE_GUILD_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_JOIN_PREPARE_GUILD_REQ stJoinPrepareGuildReq
		{
			get
			{
				return this.dataObject as CSPKG_JOIN_PREPARE_GUILD_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_JOIN_PREPARE_GUILD_RSP stJoinPrepareGuildRsp
		{
			get
			{
				return this.dataObject as SCPKG_JOIN_PREPARE_GUILD_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_JOIN_PREPARE_GUILD_NTF stJoinPrepareGuildNtf
		{
			get
			{
				return this.dataObject as SCPKG_JOIN_PREPARE_GUILD_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_ADD_GUILD_NTF stAddGuildNtf
		{
			get
			{
				return this.dataObject as SCPKG_ADD_GUILD_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_MEMBER_ONLINE_NTF stMemberOnlineNtf
		{
			get
			{
				return this.dataObject as SCPKG_MEMBER_ONLINE_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_PREPARE_GUILD_BREAK_NTF stPrepareGuildBreakNtf
		{
			get
			{
				return this.dataObject as SCPKG_PREPARE_GUILD_BREAK_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_MODIFY_GUILD_SETTING_REQ stModifyGuildSettingReq
		{
			get
			{
				return this.dataObject as CSPKG_MODIFY_GUILD_SETTING_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_MODIFY_GUILD_SETTING_RSP stModifyGuildSettingRsp
		{
			get
			{
				return this.dataObject as SCPKG_MODIFY_GUILD_SETTING_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_GET_GUILD_APPLY_LIST_REQ stGetGuildApplyListReq
		{
			get
			{
				return this.dataObject as CSPKG_GET_GUILD_APPLY_LIST_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_GET_GUILD_APPLY_LIST_RSP stGetGuildApplyListRsp
		{
			get
			{
				return this.dataObject as SCPKG_GET_GUILD_APPLY_LIST_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_APPLY_JOIN_GUILD_REQ stApplyJoinGuildReq
		{
			get
			{
				return this.dataObject as CSPKG_APPLY_JOIN_GUILD_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_APPLY_JOIN_GUILD_RSP stApplyJoinGuildRsp
		{
			get
			{
				return this.dataObject as SCPKG_APPLY_JOIN_GUILD_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_JOIN_GUILD_APPLY_NTF stJoinGuildApplyNtf
		{
			get
			{
				return this.dataObject as SCPKG_JOIN_GUILD_APPLY_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_NEW_MEMBER_JOIN_GULD_NTF stNewMemberJoinGuildNtf
		{
			get
			{
				return this.dataObject as SCPKG_NEW_MEMBER_JOIN_GULD_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_APPROVE_JOIN_GUILD_APPLY stApproveJoinGuildApply
		{
			get
			{
				return this.dataObject as CSPKG_APPROVE_JOIN_GUILD_APPLY;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_QUIT_GUILD_REQ stQuitGuildReq
		{
			get
			{
				return this.dataObject as CSPKG_QUIT_GUILD_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_QUIT_GUILD_RSP stQuitGuildRsp
		{
			get
			{
				return this.dataObject as SCPKG_QUIT_GUILD_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_QUIT_GUILD_NTF stQuitGuildNtf
		{
			get
			{
				return this.dataObject as SCPKG_QUIT_GUILD_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_GUILD_INVITE_REQ stGuildInviteReq
		{
			get
			{
				return this.dataObject as CSPKG_GUILD_INVITE_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_GUILD_INVITE_RSP stGuildInviteRsp
		{
			get
			{
				return this.dataObject as SCPKG_GUILD_INVITE_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_SEARCH_GUILD_REQ stSearchGuildReq
		{
			get
			{
				return this.dataObject as CSPKG_SEARCH_GUILD_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_SEARCH_GUILD_RSP stSearchGuildRsp
		{
			get
			{
				return this.dataObject as SCPKG_SEARCH_GUILD_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_DEAL_GUILD_INVITE stDealGuildInvite
		{
			get
			{
				return this.dataObject as CSPKG_DEAL_GUILD_INVITE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_GUILD_RECOMMEND_REQ stGuildRecommendReq
		{
			get
			{
				return this.dataObject as CSPKG_GUILD_RECOMMEND_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_GUILD_RECOMMEND_RSP stGuildRecommendRsp
		{
			get
			{
				return this.dataObject as SCPKG_GUILD_RECOMMEND_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_GUILD_RECOMMEND_NTF stGuildRecommendNtf
		{
			get
			{
				return this.dataObject as SCPKG_GUILD_RECOMMEND_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_GET_GUILD_RECOMMEND_LIST_REQ stGetGuildRecommendListReq
		{
			get
			{
				return this.dataObject as CSPKG_GET_GUILD_RECOMMEND_LIST_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_GET_GUILD_RECOMMEND_LIST_RSP stGetGuildRecommendListRsp
		{
			get
			{
				return this.dataObject as SCPKG_GET_GUILD_RECOMMEND_LIST_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_REJECT_GUILD_RECOMMEND stRejectGuildRecommend
		{
			get
			{
				return this.dataObject as CSPKG_REJECT_GUILD_RECOMMEND;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_DEAL_GUILD_INVITE_RSP stDealGuildInviteRsp
		{
			get
			{
				return this.dataObject as SCPKG_DEAL_GUILD_INVITE_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_SEARCH_PREGUILD_REQ stSearchPreGuildReq
		{
			get
			{
				return this.dataObject as CSPKG_SEARCH_PREGUILD_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_SEARCH_PREGUILD_RSP stSearchPreGuildRsp
		{
			get
			{
				return this.dataObject as SCPKG_SEARCH_PREGUILD_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_GUILD_LEVEL_CHANGE_NTF stGuildLvChgNtf
		{
			get
			{
				return this.dataObject as SCPKG_GUILD_LEVEL_CHANGE_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_APPOINT_POSITION_REQ stAppointPositionReq
		{
			get
			{
				return this.dataObject as CSPKG_APPOINT_POSITION_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_APPOINT_POSITION_RSP stAppointPositionRsp
		{
			get
			{
				return this.dataObject as SCPKG_APPOINT_POSITION_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_GUILD_POSITION_CHG_NTF stGuildPositionChgNtf
		{
			get
			{
				return this.dataObject as SCPKG_GUILD_POSITION_CHG_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_FIRE_GUILD_MEMBER_REQ stFireGuildMemberReq
		{
			get
			{
				return this.dataObject as CSPKG_FIRE_GUILD_MEMBER_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_FIRE_GUILD_MEMBER_RSP stFireGuildMemberRsp
		{
			get
			{
				return this.dataObject as SCPKG_FIRE_GUILD_MEMBER_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_FIRE_GUILD_MEMBER_NTF stFireGuildMemberNtf
		{
			get
			{
				return this.dataObject as SCPKG_FIRE_GUILD_MEMBER_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_GUILD_CROSS_DAY_NTF stGuildCrossDayNtf
		{
			get
			{
				return this.dataObject as SCPKG_GUILD_CROSS_DAY_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_MEMBER_RANK_POINT_NTF stMemberRankPointNtf
		{
			get
			{
				return this.dataObject as SCPKG_MEMBER_RANK_POINT_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_GUILD_RANK_RESET_NTF stGuildRankResetNtf
		{
			get
			{
				return this.dataObject as SCPKG_GUILD_RANK_RESET_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_GUILD_CONSTRUCT_CHG stGuildConstructChg
		{
			get
			{
				return this.dataObject as SCPKG_GUILD_CONSTRUCT_CHG;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CHG_GUILD_HEADID_REQ stChgGuildHeadIDReq
		{
			get
			{
				return this.dataObject as CSPKG_CHG_GUILD_HEADID_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CHG_GUILD_HEADID_RSP stChgGuildHeadIDRsp
		{
			get
			{
				return this.dataObject as SCPKG_CHG_GUILD_HEADID_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CHG_GUILD_NOTICE_REQ stChgGuildNoticeReq
		{
			get
			{
				return this.dataObject as CSPKG_CHG_GUILD_NOTICE_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CHG_GUILD_NOTICE_RSP stChgGuildNoticeRsp
		{
			get
			{
				return this.dataObject as SCPKG_CHG_GUILD_NOTICE_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_UPGRADE_GUILD_BY_COUPONS_REQ stUpgradeGuildByCouponsReq
		{
			get
			{
				return this.dataObject as CSPKG_UPGRADE_GUILD_BY_COUPONS_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_UPGRADE_GUILD_BY_COUPONS_RSP stUpgradeGuildByCouponsRsp
		{
			get
			{
				return this.dataObject as SCPKG_UPGRADE_GUILD_BY_COUPONS_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_GUILD_SIGNIN_REQ stGuildSignInReq
		{
			get
			{
				return this.dataObject as CSPKG_GUILD_SIGNIN_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_GUILD_SIGNIN_RSP stGuildSignInRsp
		{
			get
			{
				return this.dataObject as SCPKG_GUILD_SIGNIN_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_GUILD_SEASON_RESET_NTF stGuildSeasonResetNtf
		{
			get
			{
				return this.dataObject as SCPKG_GUILD_SEASON_RESET_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_GET_GROUP_GUILD_ID_REQ stGetGroupGuildIDReq
		{
			get
			{
				return this.dataObject as CSPKG_GET_GROUP_GUILD_ID_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_GET_GROUP_GUILD_ID_NTF stGetGroupGuildIDNtf
		{
			get
			{
				return this.dataObject as SCPKG_GET_GROUP_GUILD_ID_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_SET_GUILD_GROUP_OPENID_REQ stSetGuildGroupOpenIDReq
		{
			get
			{
				return this.dataObject as CSPKG_SET_GUILD_GROUP_OPENID_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_SET_GUILD_GROUP_OPENID_NTF stSetGuildGroupOpenIDNtf
		{
			get
			{
				return this.dataObject as SCPKG_SET_GUILD_GROUP_OPENID_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_GUILD_EVENT_REQ stGuildEventReq
		{
			get
			{
				return this.dataObject as CSPKG_GUILD_EVENT_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_GUILD_EVENT_RSP stGuildEventRsp
		{
			get
			{
				return this.dataObject as SCPKG_GUILD_EVENT_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_SEND_GUILD_RECRUIT_REQ stSendGuildRecruitReq
		{
			get
			{
				return this.dataObject as CSPKG_SEND_GUILD_RECRUIT_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_SEND_GUILD_RECRUIT_RSP stSendGuildRecruitRsp
		{
			get
			{
				return this.dataObject as SCPKG_SEND_GUILD_RECRUIT_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_GET_GUILD_RECRUIT_REQ stGetGuildRecruitReq
		{
			get
			{
				return this.dataObject as CSPKG_GET_GUILD_RECRUIT_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_GET_GUILD_RECRUIT_RSP stGetGuildRecruitRsp
		{
			get
			{
				return this.dataObject as SCPKG_GET_GUILD_RECRUIT_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_GUILD_BINDQUNLOG_REQ stGuildBindQunReq
		{
			get
			{
				return this.dataObject as CSPKG_GUILD_BINDQUNLOG_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_GUILD_UNBINDQUNLOG_REQ stGuildUnBindQunReq
		{
			get
			{
				return this.dataObject as CSPKG_GUILD_UNBINDQUNLOG_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_ACNTACTIVITYINFO_REQ stAcntActivityInfoReq
		{
			get
			{
				return this.dataObject as CSPKG_ACNTACTIVITYINFO_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_ACNTACTIVITYINFO_RSP stAcntActivityInfoRsp
		{
			get
			{
				return this.dataObject as SCPKG_ACNTACTIVITYINFO_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_ACTIVITYENDDEPLETION_NTF stActivityEndDepletionNtf
		{
			get
			{
				return this.dataObject as SCPKG_ACTIVITYENDDEPLETION_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_DRAWWEAL_REQ stDrawWealReq
		{
			get
			{
				return this.dataObject as CSPKG_DRAWWEAL_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_DRAWWEAL_RSP stDrawWealRsp
		{
			get
			{
				return this.dataObject as SCPKG_DRAWWEAL_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_WEALDETAIL_NTF stWealDetailNtf
		{
			get
			{
				return this.dataObject as SCPKG_WEALDETAIL_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_WEAL_CON_DATA_NTF stWealConDataNtf
		{
			get
			{
				return this.dataObject as SCPKG_WEAL_CON_DATA_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_WEAL_DATA_REQ stWealDataReq
		{
			get
			{
				return this.dataObject as CSPKG_WEAL_DATA_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_WEAL_DATA_NTF stWealDataNtf
		{
			get
			{
				return this.dataObject as SCPKG_WEAL_DATA_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_PROP_MULTIPLE_NTF stPropMultipleNtf
		{
			get
			{
				return this.dataObject as SCPKG_PROP_MULTIPLE_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_RES_DATA_NTF stResDataNtf
		{
			get
			{
				return this.dataObject as SCPKG_RES_DATA_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_WEAL_EXCHANGE_RES stWealExchangeRes
		{
			get
			{
				return this.dataObject as SCPKG_WEAL_EXCHANGE_RES;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_WEAL_POINTDATA_NTF stWealPointDataNtf
		{
			get
			{
				return this.dataObject as SCPKG_WEAL_POINTDATA_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CLASSOFRANKDETAIL_NTF stClassOfRankDetailNtf
		{
			get
			{
				return this.dataObject as SCPKG_CLASSOFRANKDETAIL_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_UPDRANKINFO_NTF stUpdateRankInfo
		{
			get
			{
				return this.dataObject as SCPKG_UPDRANKINFO_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_GET_RANKING_LIST_REQ stGetRankingListReq
		{
			get
			{
				return this.dataObject as CSPKG_GET_RANKING_LIST_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_GET_RANKING_LIST_RSP stGetRankingListRsp
		{
			get
			{
				return this.dataObject as SCPKG_GET_RANKING_LIST_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_GET_RANKING_ACNT_INFO_REQ stGetRankingAcntInfoReq
		{
			get
			{
				return this.dataObject as CSPKG_GET_RANKING_ACNT_INFO_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_GET_RANKING_ACNT_INFO_RSP stGetRankingAcntInfoRsp
		{
			get
			{
				return this.dataObject as SCPKG_GET_RANKING_ACNT_INFO_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_GET_ACNT_DETAIL_INFO_REQ stGetAcntDetailInfoReq
		{
			get
			{
				return this.dataObject as CSPKG_GET_ACNT_DETAIL_INFO_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_GET_ACNT_DETAIL_INFO_RSP stGetAcntDetailInfoRsp
		{
			get
			{
				return this.dataObject as SCPKG_GET_ACNT_DETAIL_INFO_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_SET_ACNT_NEWBIE_TYPE_REQ stSetAcntNewbieTypeReq
		{
			get
			{
				return this.dataObject as CSPKG_SET_ACNT_NEWBIE_TYPE_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_SET_ACNT_NEWBIE_TYPE_RSP stSetAcntNewbieTypeRsp
		{
			get
			{
				return this.dataObject as SCPKG_SET_ACNT_NEWBIE_TYPE_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_GET_SELFRANKINFO stGetSelfRankInfo
		{
			get
			{
				return this.dataObject as CSPKG_GET_SELFRANKINFO;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_ACNT_RANKINFO_RSP stNtfRankInfoRsp
		{
			get
			{
				return this.dataObject as SCPKG_ACNT_RANKINFO_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_MONTH_WEEK_CARD_USE_RSP stMonthWeekCardUseRsp
		{
			get
			{
				return this.dataObject as SCPKG_MONTH_WEEK_CARD_USE_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_GET_RANKLIST_BY_SPECIAL_SCORE_REQ stGetRankListBySpecialScoreReq
		{
			get
			{
				return this.dataObject as CSPKG_GET_RANKLIST_BY_SPECIAL_SCORE_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_GET_RANKLIST_BY_SPECIAL_SCORE_RSP stGetRankListBySpecialScoreRsp
		{
			get
			{
				return this.dataObject as SCPKG_GET_RANKLIST_BY_SPECIAL_SCORE_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_GET_SPECIAL_GUILD_RANK_INFO_REQ stGetSpecialGuildRankInfoReq
		{
			get
			{
				return this.dataObject as CSPKG_GET_SPECIAL_GUILD_RANK_INFO_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_GET_SPECIAL_GUILD_RANK_INFO_RSP stGetSpecialGuildRankInfoRsp
		{
			get
			{
				return this.dataObject as SCPKG_GET_SPECIAL_GUILD_RANK_INFO_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_GET_INTIMACY_RELATION_REQ stGetIntimacyRelationReq
		{
			get
			{
				return this.dataObject as CSPKG_GET_INTIMACY_RELATION_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_GET_INTIMACY_RELATION_RSP stGetIntimacyRelationRsp
		{
			get
			{
				return this.dataObject as SCPKG_GET_INTIMACY_RELATION_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_GET_BURNING_PROGRESS_REQ stGetBurningProgressReq
		{
			get
			{
				return this.dataObject as CSPKG_GET_BURNING_PROGRESS_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_GET_BURNING_PROGRESS_RSP stGetBurningProgressRsp
		{
			get
			{
				return this.dataObject as SCPKG_GET_BURNING_PROGRESS_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_GET_BURNING_REWARD_REQ stGetBurningRewardReq
		{
			get
			{
				return this.dataObject as CSPKG_GET_BURNING_REWARD_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_GET_BURNING_REWARD_RSP stGetBurningRewardRsp
		{
			get
			{
				return this.dataObject as SCPKG_GET_BURNING_REWARD_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_RESET_BURNING_PROGRESS_REQ stResetBurningProgressReq
		{
			get
			{
				return this.dataObject as CSPKG_RESET_BURNING_PROGRESS_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_RESET_BURNING_PROGRESS_RSP stResetBurningProgressRsp
		{
			get
			{
				return this.dataObject as SCPKG_RESET_BURNING_PROGRESS_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_PVP_COMMINFO_REPORT stPVPCommonInfoReport
		{
			get
			{
				return this.dataObject as CSPKG_PVP_COMMINFO_REPORT;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_PVP_GAMEDATA_REPORT stPVPGameDataReport
		{
			get
			{
				return this.dataObject as CSPKG_PVP_GAMEDATA_REPORT;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_PVP_GAMELOG_REPORT stPVPGameLogReport
		{
			get
			{
				return this.dataObject as CSPKG_PVP_GAMELOG_REPORT;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_PVP_NTF_CLIENT stNtfClient
		{
			get
			{
				return this.dataObject as SCPKG_PVP_NTF_CLIENT;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_PVP_GAMEDATA_REPORTOVER stPVPGameDataReportOver
		{
			get
			{
				return this.dataObject as CSPKG_PVP_GAMEDATA_REPORTOVER;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_PVP_GAMELOG_REPORTOVER stPVPGameLogReportOver
		{
			get
			{
				return this.dataObject as CSPKG_PVP_GAMELOG_REPORTOVER;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_SETBATTLELISTOFARENA_REQ stSetBattleListOfArenaReq
		{
			get
			{
				return this.dataObject as CSPKG_SETBATTLELISTOFARENA_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_SETBATTLELISTOFARENA_RSP stSetBattleListOfArenaRsp
		{
			get
			{
				return this.dataObject as SCPKG_SETBATTLELISTOFARENA_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_JOINARENA_REQ stJoinArenaReq
		{
			get
			{
				return this.dataObject as CSPKG_JOINARENA_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_JOINARENA_RSP stJoinArenaRsp
		{
			get
			{
				return this.dataObject as SCPKG_JOINARENA_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_GETARENADATA_REQ stGetArenaDataReq
		{
			get
			{
				return this.dataObject as CSPKG_GETARENADATA_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_GETARENADATA_RSP stGetArenaDataRsp
		{
			get
			{
				return this.dataObject as SCPKG_GETARENADATA_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CHGARENAFIGHTERREQ stChgArenaFighterReq
		{
			get
			{
				return this.dataObject as CSPKG_CHGARENAFIGHTERREQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CHGARENAFIGHTERRSP stChgArenaFighterRsp
		{
			get
			{
				return this.dataObject as SCPKG_CHGARENAFIGHTERRSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_GETTOPFIGHTEROFARENA_REQ stGetTopFighterOfArenaReq
		{
			get
			{
				return this.dataObject as CSPKG_GETTOPFIGHTEROFARENA_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_GETTOPFIGHTEROFARENA_RSP stGetTopFighterOfArenaRsp
		{
			get
			{
				return this.dataObject as SCPKG_GETTOPFIGHTEROFARENA_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_GETARENAFIGHTHISTORY_REQ stGetArenaFightHistoryReq
		{
			get
			{
				return this.dataObject as CSPKG_GETARENAFIGHTHISTORY_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_GETARENAFIGHTHISTORY_RSP stGetArenaFightHistoryRsp
		{
			get
			{
				return this.dataObject as SCPKG_GETARENAFIGHTHISTORY_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_RANKCURSEASONHISTORY_NTF stRankCurSeasonHistory
		{
			get
			{
				return this.dataObject as SCPKG_RANKCURSEASONHISTORY_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_RANKPASTSEASONHISTORY_NTF stRankPastSeasonHistory
		{
			get
			{
				return this.dataObject as SCPKG_RANKPASTSEASONHISTORY_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_GETRANKREWARD_REQ stGetRankRewardReq
		{
			get
			{
				return this.dataObject as CSPKG_GETRANKREWARD_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_GETRANKREWARD_RSP stGetRankRewardRsp
		{
			get
			{
				return this.dataObject as SCPKG_GETRANKREWARD_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_NTF_ADDCURSEASONRECORD stNtfAddCurSeasonRecord
		{
			get
			{
				return this.dataObject as SCPKG_NTF_ADDCURSEASONRECORD;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_NTF_ADDPASTSEASONRECORD stNtfAddPastSeasonRecord
		{
			get
			{
				return this.dataObject as SCPKG_NTF_ADDPASTSEASONRECORD;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_ANTIDATA_REQ stAntiDataReq
		{
			get
			{
				return this.dataObject as CSPKG_ANTIDATA_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_ANTIDATA_SYN stAntiDataSyn
		{
			get
			{
				return this.dataObject as SCPKG_ANTIDATA_SYN;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CREATE_TVOIP_ROOM_NTF stCreateTvoipRoomNtf
		{
			get
			{
				return this.dataObject as SCPKG_CREATE_TVOIP_ROOM_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_JOIN_TVOIP_ROOM_NTF stJoinTvoipRoomNtf
		{
			get
			{
				return this.dataObject as SCPKG_JOIN_TVOIP_ROOM_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CHANGE_NAME_REQ stChangeNameReq
		{
			get
			{
				return this.dataObject as CSPKG_CHANGE_NAME_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CHANGE_NAME_RSP stChangeNameRsp
		{
			get
			{
				return this.dataObject as SCPKG_CHANGE_NAME_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_GUILD_NAME_CHG_NTF stGuildNameChgNtf
		{
			get
			{
				return this.dataObject as SCPKG_GUILD_NAME_CHG_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_UPDATECLIENTBITS_NTF stUpdateClientBitsNtf
		{
			get
			{
				return this.dataObject as CSPKG_UPDATECLIENTBITS_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_SERVERTIME_REQ stServerTimeReq
		{
			get
			{
				return this.dataObject as CSPKG_SERVERTIME_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_SERVERTIME_RSP stServerTimeRsp
		{
			get
			{
				return this.dataObject as SCPKG_SERVERTIME_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_UPDNEWCLIENTBITS_NTF stUpdNewClientBits
		{
			get
			{
				return this.dataObject as CSPKG_UPDNEWCLIENTBITS_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_NTF_FRIEND_GAME_STATE stNtfFriendGameState
		{
			get
			{
				return this.dataObject as SCPKG_CMD_NTF_FRIEND_GAME_STATE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_NTF_SNS_FRIEND stNtfSnsFriend
		{
			get
			{
				return this.dataObject as SCPKG_NTF_SNS_FRIEND;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CHG_SNS_FRIEND_PROFILE stSnsFriendChgProfile
		{
			get
			{
				return this.dataObject as SCPKG_CHG_SNS_FRIEND_PROFILE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_NTF_ACNT_SNSNAME stNtfAcntSnsName
		{
			get
			{
				return this.dataObject as SCPKG_CMD_NTF_ACNT_SNSNAME;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_NTF_SNS_NICKNAME stNtfSnsNickName
		{
			get
			{
				return this.dataObject as SCPKG_NTF_SNS_NICKNAME;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_FUNCTION_SWITCH_NTF stFunctionSwitchNtf
		{
			get
			{
				return this.dataObject as SCPKG_FUNCTION_SWITCH_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_QQVIPINFO_REQ stQQVIPInfoReq
		{
			get
			{
				return this.dataObject as CSPKG_QQVIPINFO_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_QQVIPINFO_RSP stQQVIPInfoRsp
		{
			get
			{
				return this.dataObject as SCPKG_QQVIPINFO_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_DIRECT_BUY_ITEM_REQ stDirectBuyItemReq
		{
			get
			{
				return this.dataObject as CSPKG_DIRECT_BUY_ITEM_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_DIRECT_BUY_ITEM_RSP stDirectBuyItemRsp
		{
			get
			{
				return this.dataObject as SCPKG_DIRECT_BUY_ITEM_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_PVE_REVIVE_REQ stPveReviveReq
		{
			get
			{
				return this.dataObject as CSPKG_PVE_REVIVE_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_PVE_REVIVE_RSP stPveReviveRsp
		{
			get
			{
				return this.dataObject as SCPKG_PVE_REVIVE_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_USER_COMPLAINT_REQ stUserComplaintReq
		{
			get
			{
				return this.dataObject as CSPKG_USER_COMPLAINT_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_USER_COMPLAINT_RSP stUserComplaintRsp
		{
			get
			{
				return this.dataObject as SCPKG_USER_COMPLAINT_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_SHARE_TLOG_REQ stShareTLogReq
		{
			get
			{
				return this.dataObject as CSPKG_SHARE_TLOG_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_SHARE_TLOG_RSP stShareTLogRsp
		{
			get
			{
				return this.dataObject as SCPKG_SHARE_TLOG_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_DYE_INBATTLE_NEWBIEBIT_REQ stDyeInBattleNewbieBitReq
		{
			get
			{
				return this.dataObject as CSPKG_DYE_INBATTLE_NEWBIEBIT_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_DYE_INBATTLE_NEWBIEBIT_RSP stDyeInBattleNewbieBitRsp
		{
			get
			{
				return this.dataObject as SCPKG_DYE_INBATTLE_NEWBIEBIT_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_ACHIEVEMENT_INFO_NTF stGetAchievememtInfoNtf
		{
			get
			{
				return this.dataObject as SCPKG_ACHIEVEMENT_INFO_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_ACHIEVEMENT_STATE_CHG_NTF stAchievementStateChgNtf
		{
			get
			{
				return this.dataObject as SCPKG_ACHIEVEMENT_STATE_CHG_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_ACHIEVEMENT_DONE_DATA_CHG_NTF stAchievementDoneDataChgNtf
		{
			get
			{
				return this.dataObject as SCPKG_ACHIEVEMENT_DONE_DATA_CHG_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_GET_ACHIEVEMENT_REWARD_REQ stGetAchievementRewardReq
		{
			get
			{
				return this.dataObject as CSPKG_GET_ACHIEVEMENT_REWARD_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_GET_ACHIEVEMENT_REWARD_RSP stGetAchievementRewardRsp
		{
			get
			{
				return this.dataObject as SCPKG_GET_ACHIEVEMENT_REWARD_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_GET_TROPHYLVL_REWARD_REQ stGetTrophyLvlRewardReq
		{
			get
			{
				return this.dataObject as CSPKG_GET_TROPHYLVL_REWARD_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_GET_TROPHYLVL_REWARD_RSP stGetTrophyLvlmentRewardRsp
		{
			get
			{
				return this.dataObject as SCPKG_GET_TROPHYLVL_REWARD_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_NTF_TROPHYLVLUP stNtfTrophyLvlUp
		{
			get
			{
				return this.dataObject as SCPKG_NTF_TROPHYLVLUP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_REQ_ACHIEVE stReqAchieveShow
		{
			get
			{
				return this.dataObject as CSPKG_REQ_ACHIEVE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_RSP_ACHIEVE stRspAchieveShow
		{
			get
			{
				return this.dataObject as SCPKG_RSP_ACHIEVE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_DAILY_CHECK_DATA_NTF stDailyCheckDataNtf
		{
			get
			{
				return this.dataObject as SCPKG_DAILY_CHECK_DATA_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_GAME_VIP_NTF stGameVipNtf
		{
			get
			{
				return this.dataObject as SCPKG_GAME_VIP_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_HEADIMG_CHG_REQ stHeadImgChgReq
		{
			get
			{
				return this.dataObject as CSPKG_HEADIMG_CHG_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_HEADIMG_CHG_RSP stHeadImgChgRsp
		{
			get
			{
				return this.dataObject as SCPKG_HEADIMG_CHG_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_HEADIMG_FLAGCLR_REQ stHeadImgFlagClrReq
		{
			get
			{
				return this.dataObject as CSPKG_HEADIMG_FLAGCLR_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_HEADIMG_FLAGCLR_RSP stHeadImgFlagClrRsp
		{
			get
			{
				return this.dataObject as SCPKG_HEADIMG_FLAGCLR_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_NTF_HEADIMG_CHG stHeadImgChgNtf
		{
			get
			{
				return this.dataObject as SCPKG_NTF_HEADIMG_CHG;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_HEADIMG_LIST_SYNC stHeadImgListSync
		{
			get
			{
				return this.dataObject as SCPKG_HEADIMG_LIST_SYNC;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_NTF_HEADIMG_ADD stHeadImgAddNtf
		{
			get
			{
				return this.dataObject as SCPKG_NTF_HEADIMG_ADD;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_NTF_HEADIMG_DEL stHeadImgDelNtf
		{
			get
			{
				return this.dataObject as SCPKG_NTF_HEADIMG_DEL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_LUCKYDRAW_DATA_NTF stLuckyDrawDataNtf
		{
			get
			{
				return this.dataObject as SCPKG_LUCKYDRAW_DATA_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_LUCKYDRAW_REQ stLuckyDrawReq
		{
			get
			{
				return this.dataObject as CSPKG_LUCKYDRAW_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_LUCKYDRAW_RSP stLuckyDrawRsp
		{
			get
			{
				return this.dataObject as SCPKG_LUCKYDRAW_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_LUCKYDRAW_EXTERN_REQ stLuckyDrawExternReq
		{
			get
			{
				return this.dataObject as CSPKG_LUCKYDRAW_EXTERN_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_LUCKYDRAW_EXTERN_RSP stLuckyDrawExternRsp
		{
			get
			{
				return this.dataObject as SCPKG_LUCKYDRAW_EXTERN_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_ACNT_VOICESTATE stAcntVoiceState
		{
			get
			{
				return this.dataObject as CSPKG_ACNT_VOICESTATE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_NTF_VOICESTATE stNtfVoiceState
		{
			get
			{
				return this.dataObject as SCPKG_NTF_VOICESTATE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_SURRENDER_REQ stSurrenderReq
		{
			get
			{
				return this.dataObject as CSPKG_SURRENDER_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_SURRENDER_RSP stSurrenderRsp
		{
			get
			{
				return this.dataObject as SCPKG_SURRENDER_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_SURRENDER_NTF stSurrenderNtf
		{
			get
			{
				return this.dataObject as SCPKG_SURRENDER_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CLT_PERFORMANCE stCltPerformance
		{
			get
			{
				return this.dataObject as CSPKG_CLT_PERFORMANCE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CLT_ACTION_STATISTICS stCltActionStatistics
		{
			get
			{
				return this.dataObject as CSPKG_CLT_ACTION_STATISTICS;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_ENTERTAINMENT_SYN_RAND_HERO_CNT stEntertainmentRandHeroCnt
		{
			get
			{
				return this.dataObject as SCPKG_ENTERTAINMENT_SYN_RAND_HERO_CNT;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_GETAWARDPOOL_REQ stGetAwardPoolReq
		{
			get
			{
				return this.dataObject as CSPKG_GETAWARDPOOL_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_GETAWARDPOOL_RSP stGetAwardPoolRsp
		{
			get
			{
				return this.dataObject as SCPKG_GETAWARDPOOL_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_MATCHPOINT_NTF stMatchPointNtf
		{
			get
			{
				return this.dataObject as SCPKG_MATCHPOINT_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_BUY_MATCHTICKET_REQ stBuyMatchTicketReq
		{
			get
			{
				return this.dataObject as CSPKG_BUY_MATCHTICKET_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_BUY_MATCHTICKET_RSP stBuyMatchTicketRsp
		{
			get
			{
				return this.dataObject as SCPKG_BUY_MATCHTICKET_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_GET_MATCHINFO_REQ stGetMatchInfoReq
		{
			get
			{
				return this.dataObject as CSPKG_GET_MATCHINFO_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_GET_MATCHINFO_RSP stGetMatchInfoRsp
		{
			get
			{
				return this.dataObject as SCPKG_GET_MATCHINFO_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_GET_REWARDMATCH_INFO_REQ stGetRewardMatchInfoReq
		{
			get
			{
				return this.dataObject as CSPKG_GET_REWARDMATCH_INFO_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_GET_REWARDMATCH_INFO_RSP stGetRewardMatchInfoRsp
		{
			get
			{
				return this.dataObject as SCPKG_GET_REWARDMATCH_INFO_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_REWARDMATCH_STATE_CHG_REQ stRewardMatchStateChgReq
		{
			get
			{
				return this.dataObject as CSPKG_REWARDMATCH_STATE_CHG_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_REWARDMATCH_INFO_CHG_NTF stRewardMatchInfoChgNtf
		{
			get
			{
				return this.dataObject as SCPKG_REWARDMATCH_INFO_CHG_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_SELFDEFINE_HEROEQUIP_CHG_REQ stSelfDefineHeroEquipChgReq
		{
			get
			{
				return this.dataObject as CSPKG_SELFDEFINE_HEROEQUIP_CHG_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_SELFDEFINE_HEROEQUIP_CHG_RSP stSelfDefineHeroEquipChgRsp
		{
			get
			{
				return this.dataObject as SCPKG_SELFDEFINE_HEROEQUIP_CHG_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_RECOVER_SYSTEMEQUIP_REQ stRecoverSystemEquipChgReq
		{
			get
			{
				return this.dataObject as CSPKG_RECOVER_SYSTEMEQUIP_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_RECOVER_SYSTEMEQUIP_RSP stRecoverSystemEquipChgRsp
		{
			get
			{
				return this.dataObject as SCPKG_RECOVER_SYSTEMEQUIP_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_MATCHTEAM_DESTROY_NTF stMatchTeamDestroyNtf
		{
			get
			{
				return this.dataObject as SCPKG_MATCHTEAM_DESTROY_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_GET_ACNT_CREDIT_VALUE stGetAcntCreditValue
		{
			get
			{
				return this.dataObject as CSPKG_GET_ACNT_CREDIT_VALUE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_NTF_ACNT_CREDIT_VALUE stNtfAcntCreditValue
		{
			get
			{
				return this.dataObject as SCPKG_NTF_ACNT_CREDIT_VALUE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_JOINMULTIGAMEREQ stJoinMultiGameReq
		{
			get
			{
				return this.dataObject as CSPKG_JOINMULTIGAMEREQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_JOIN_TEAM_REQ stJoinTeamReq
		{
			get
			{
				return this.dataObject as CSPKG_JOIN_TEAM_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_NTF_CUR_BAN_PICK_INFO stNtfCurBanPickInfo
		{
			get
			{
				return this.dataObject as SCPKG_NTF_CUR_BAN_PICK_INFO;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_BAN_HERO_REQ stBanHeroReq
		{
			get
			{
				return this.dataObject as CSPKG_BAN_HERO_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_BAN_HERO_RSP stBanHeroRsp
		{
			get
			{
				return this.dataObject as SCPKG_BAN_HERO_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_SWAP_HERO_REQ stSwapHeroReq
		{
			get
			{
				return this.dataObject as CSPKG_SWAP_HERO_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_NTF_SWAP_HERO stNtfSwapHero
		{
			get
			{
				return this.dataObject as SCPKG_NTF_SWAP_HERO;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CONFIRM_SWAP_HERO_REQ stConfirmSwapHeroReq
		{
			get
			{
				return this.dataObject as CSPKG_CONFIRM_SWAP_HERO_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_NTF_CONFIRM_SWAP_HERO stNtfConfirmSwapHero
		{
			get
			{
				return this.dataObject as SCPKG_NTF_CONFIRM_SWAP_HERO;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_OBSERVE_FRIEND_REQ stObserveFriendReq
		{
			get
			{
				return this.dataObject as CSPKG_OBSERVE_FRIEND_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_OBSERVE_FRIEND_RSP stObserveFriendRsp
		{
			get
			{
				return this.dataObject as SCPKG_OBSERVE_FRIEND_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_OBSERVE_GREAT_REQ stObserveGreatReq
		{
			get
			{
				return this.dataObject as CSPKG_OBSERVE_GREAT_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_OBSERVE_GREAT_RSP stObserveGreatRsp
		{
			get
			{
				return this.dataObject as SCPKG_OBSERVE_GREAT_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_GET_GREATMATCH_REQ stGetGreatMatchReq
		{
			get
			{
				return this.dataObject as CSPKG_GET_GREATMATCH_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_GET_GREATMATCH_RSP stGetGreatMatchRsp
		{
			get
			{
				return this.dataObject as SCPKG_GET_GREATMATCH_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_SELFDEFINE_CHATINFO_CHG_REQ stSelfDefineChatInfoChgReq
		{
			get
			{
				return this.dataObject as CSPKG_SELFDEFINE_CHATINFO_CHG_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_SELFDEFINE_CHATINFO_CHG_RSP stSelfDefineChatInfoChgRsp
		{
			get
			{
				return this.dataObject as SCPKG_SELFDEFINE_CHATINFO_CHG_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CANCEL_SWAP_HERO_REQ stCancelSwapHeroReq
		{
			get
			{
				return this.dataObject as CSPKG_CANCEL_SWAP_HERO_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CANCEL_SWAP_HERO_RSP stCancelSwapHeroRsp
		{
			get
			{
				return this.dataObject as SCPKG_CANCEL_SWAP_HERO_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_GET_VIDEOFRAPS_REQ stGetVideoFrapsReq
		{
			get
			{
				return this.dataObject as CSPKG_GET_VIDEOFRAPS_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_GET_VIDEOFRAPS_RSP stGetVideoFrapsRsp
		{
			get
			{
				return this.dataObject as SCPKG_GET_VIDEOFRAPS_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_QUITOBSERVE_REQ stQuitObserveReq
		{
			get
			{
				return this.dataObject as CSPKG_QUITOBSERVE_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_QUITOBSERVE_RSP stQuitObserveRsp
		{
			get
			{
				return this.dataObject as SCPKG_QUITOBSERVE_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_ACNT_QUITSETTLEUI_REQ stQuitSettleUIReq
		{
			get
			{
				return this.dataObject as CSPKG_ACNT_QUITSETTLEUI_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_GETFRIEND_GAMESTATE_REQ stGetFriendGameState
		{
			get
			{
				return this.dataObject as CSPKG_GETFRIEND_GAMESTATE_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_WEAL_CONTENT_SHARE_DONE stWealContentShareDone
		{
			get
			{
				return this.dataObject as CSPKG_WEAL_CONTENT_SHARE_DONE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_TRANSDATA_RENAME_NTF stTransDataRenameNft
		{
			get
			{
				return this.dataObject as SCPKG_TRANSDATA_RENAME_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_TRANSDATA_RENAME_REQ stTransDataRenameReq
		{
			get
			{
				return this.dataObject as CSPKG_TRANSDATA_RENAME_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_TRANSDATA_RENAME_RES stTransDataRenameRes
		{
			get
			{
				return this.dataObject as SCPKG_TRANSDATA_RENAME_RES;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_UPLOADCLTLOG_REQ stUploadCltlogReq
		{
			get
			{
				return this.dataObject as SCPKG_UPLOADCLTLOG_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_UPLOADCLTLOG_NTF stUploadCltlogNtf
		{
			get
			{
				return this.dataObject as CSPKG_UPLOADCLTLOG_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_OBTIPS_NTF stOBTipsNtf
		{
			get
			{
				return this.dataObject as SCPKG_OBTIPS_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_ACNT_OLD_TYPE_NTF stAcntOldTypeNtf
		{
			get
			{
				return this.dataObject as SCPKG_ACNT_OLD_TYPE_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_ACNT_SET_OLD_TYPE stAcntSetOldType
		{
			get
			{
				return this.dataObject as CSPKG_ACNT_SET_OLD_TYPE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_PAUSE_REQ stPauseReq
		{
			get
			{
				return this.dataObject as CSPKG_PAUSE_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_PAUSE_RSP stPauseRsp
		{
			get
			{
				return this.dataObject as SCPKG_PAUSE_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_SELECT_NEWBIE_HERO_REQ stSelectNewbieHeroReq
		{
			get
			{
				return this.dataObject as CSPKG_SELECT_NEWBIE_HERO_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_ACNT_MOBA_INFO stAcntMobaInfoNtf
		{
			get
			{
				return this.dataObject as SCPKG_ACNT_MOBA_INFO;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_GUILD_MATCH_SELFSIGNUPINFO_RSP stGuildMatchSelfSignUpInfoRsp
		{
			get
			{
				return this.dataObject as SCPKG_GUILD_MATCH_SELFSIGNUPINFO_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_GUILD_MATCH_SIGNUPLIST_REQ stGuildMatchSignUpListReq
		{
			get
			{
				return this.dataObject as CSPKG_GUILD_MATCH_SIGNUPLIST_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_GUILD_MATCH_SIGNUPLIST_RSP stGuildMatchSignUpListRsp
		{
			get
			{
				return this.dataObject as SCPKG_GUILD_MATCH_SIGNUPLIST_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_GUILD_MATCH_SIGNUP_REQ stGuildMatchSignUpReq
		{
			get
			{
				return this.dataObject as CSPKG_GUILD_MATCH_SIGNUP_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_GUILD_MATCH_SIGNUP_RSP stGuildMatchSignUpRsp
		{
			get
			{
				return this.dataObject as SCPKG_GUILD_MATCH_SIGNUP_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_MOD_GUILD_MATCH_SIGNUP_REQ stModGuildMatchSignUpReq
		{
			get
			{
				return this.dataObject as CSPKG_MOD_GUILD_MATCH_SIGNUP_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_MOD_GUILD_MATCH_SIGNUP_RSP stModGuildMatchSignUpRsp
		{
			get
			{
				return this.dataObject as SCPKG_MOD_GUILD_MATCH_SIGNUP_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_GUILD_MATCH_GETINVITE_REQ stGuildMatchGetInviteReq
		{
			get
			{
				return this.dataObject as CSPKG_GUILD_MATCH_GETINVITE_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_GUILD_MATCH_GETINVITE_RSP stGuildMatchGetInviteRsp
		{
			get
			{
				return this.dataObject as SCPKG_GUILD_MATCH_GETINVITE_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CHG_GUILD_MATCH_LEADER_REQ stChgGuildMatchLeaderReq
		{
			get
			{
				return this.dataObject as CSPKG_CHG_GUILD_MATCH_LEADER_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CHG_GUILD_MATCH_LEADER_NTF stChgGuildMatchLeaderNtf
		{
			get
			{
				return this.dataObject as SCPKG_CHG_GUILD_MATCH_LEADER_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_INVITE_GUILD_MATCH_MEMBER_REQ stInviteGuildMatchMemberReq
		{
			get
			{
				return this.dataObject as CSPKG_INVITE_GUILD_MATCH_MEMBER_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_INVITE_GUILD_MATCH_MEMBER_NTF stInviteGuildMatchMemberNtf
		{
			get
			{
				return this.dataObject as SCPKG_INVITE_GUILD_MATCH_MEMBER_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_DEAL_GUILD_MATCH_MEMBER_INVITE stDealGuildMatchMemberInvite
		{
			get
			{
				return this.dataObject as CSPKG_DEAL_GUILD_MATCH_MEMBER_INVITE;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_GUILD_MATCH_MEMBER_INVITE_RSP stGuildMatchMemberInviteRsp
		{
			get
			{
				return this.dataObject as SCPKG_GUILD_MATCH_MEMBER_INVITE_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_KICK_GUILD_MATCH_MEMBER_REQ stKickGuildMatchMemberReq
		{
			get
			{
				return this.dataObject as CSPKG_KICK_GUILD_MATCH_MEMBER_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_LEAVE_GUILD_MATCH_TEAM_REQ stLeaveGuildMatchTeamReq
		{
			get
			{
				return this.dataObject as CSPKG_LEAVE_GUILD_MATCH_TEAM_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_GUILD_MATCH_MEMBER_CHG_NTF stGuildMatchMemberChgNtf
		{
			get
			{
				return this.dataObject as SCPKG_GUILD_MATCH_MEMBER_CHG_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_START_GUILD_MATCH_REQ stStartGuildMatch
		{
			get
			{
				return this.dataObject as CSPKG_START_GUILD_MATCH_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_SET_GUILD_MATCH_READY_REQ stSetGuildMatchReadyReq
		{
			get
			{
				return this.dataObject as CSPKG_SET_GUILD_MATCH_READY_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_SET_GUILD_MATCH_READY_RSP stSetGuildMatchReadyRsp
		{
			get
			{
				return this.dataObject as SCPKG_SET_GUILD_MATCH_READY_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_SET_GUILD_MATCH_READY_NTF stSetGuildMatchReadyNtf
		{
			get
			{
				return this.dataObject as SCPKG_SET_GUILD_MATCH_READY_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_GUILD_MATCH_SCORE_CHG_NTF stGuildMatchScoreChgNtf
		{
			get
			{
				return this.dataObject as SCPKG_GUILD_MATCH_SCORE_CHG_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_START_GUILD_MATCH_RSP stStartGuildMatchRsp
		{
			get
			{
				return this.dataObject as SCPKG_START_GUILD_MATCH_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_GUILD_MATCH_OB_INFO_CHG stGuildMatchOBInfoChg
		{
			get
			{
				return this.dataObject as SCPKG_GUILD_MATCH_OB_INFO_CHG;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_OB_GUILD_MATCH_REQ stOBGuildMatchReq
		{
			get
			{
				return this.dataObject as CSPKG_OB_GUILD_MATCH_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_OB_GUILD_MATCH_RSP stOBGuildMatchRsp
		{
			get
			{
				return this.dataObject as SCPKG_OB_GUILD_MATCH_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_GET_GUILD_MATCH_HISTORY_REQ stGetGuildMatchHistoryReq
		{
			get
			{
				return this.dataObject as CSPKG_GET_GUILD_MATCH_HISTORY_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_GET_GUILD_MATCH_HISTORY_RSP stGetGuildMatchHistoryRsp
		{
			get
			{
				return this.dataObject as SCPKG_GET_GUILD_MATCH_HISTORY_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CHG_GUILD_MATCH_LEADER_RSP stChgGuildMatchLeaderRsp
		{
			get
			{
				return this.dataObject as SCPKG_CHG_GUILD_MATCH_LEADER_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_GUILD_MATCH_REMIND_REQ stGuildMatchRemindReq
		{
			get
			{
				return this.dataObject as CSPKG_GUILD_MATCH_REMIND_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_GUILD_MATCH_REMIND_NTF stGuildMatchRemindNtf
		{
			get
			{
				return this.dataObject as SCPKG_GUILD_MATCH_REMIND_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_GET_GUILD_MATCH_OB_CNT_REQ stGetGuildMatchOBCntReq
		{
			get
			{
				return this.dataObject as CSPKG_GET_GUILD_MATCH_OB_CNT_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_GET_GUILD_MATCH_OB_CNT_RSP stGetGuildMatchOBCntRsp
		{
			get
			{
				return this.dataObject as SCPKG_GET_GUILD_MATCH_OB_CNT_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_NTF_SWITCHOFF stSwtichOffNtf
		{
			get
			{
				return this.dataObject as SCPKG_NTF_SWITCHOFF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_EQUIPEVAL_REQ stEquipEvalReq
		{
			get
			{
				return this.dataObject as CSPKG_EQUIPEVAL_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_EQUIPEVAL_RSP stEquipEvalRsp
		{
			get
			{
				return this.dataObject as SCPKG_EQUIPEVAL_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_GET_EQUIPEVAL_REQ stGetEquipEvalReq
		{
			get
			{
				return this.dataObject as CSPKG_GET_EQUIPEVAL_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_GET_EQUIPEVAL_RSP stGetEquipEvalRsp
		{
			get
			{
				return this.dataObject as CSPKG_GET_EQUIPEVAL_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_GUILD_MATCH_WEEK_RESET_NTF stGuildMatchWeekResetNtf
		{
			get
			{
				return this.dataObject as SCPKG_GUILD_MATCH_WEEK_RESET_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_GET_LADDER_REWARD_SKIN_REQ stGetLadderRewardSkinReq
		{
			get
			{
				return this.dataObject as CSPKG_GET_LADDER_REWARD_SKIN_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_SERVER_PING_REQ stServerPingReq
		{
			get
			{
				return this.dataObject as SCPKG_SERVER_PING_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_SERVER_PING_RSP stServerPingRsp
		{
			get
			{
				return this.dataObject as CSPKG_SERVER_PING_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_APPLY_MASTER_REQ stApplyMasterReq
		{
			get
			{
				return this.dataObject as CSPKG_APPLY_MASTER_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_APPLY_MASTER_RSP stApplyMasterRsp
		{
			get
			{
				return this.dataObject as SCPKG_APPLY_MASTER_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CONFIRM_MASTER_REQ stConfirmMasterReq
		{
			get
			{
				return this.dataObject as CSPKG_CONFIRM_MASTER_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CONFIRM_MASTER_RSP stConfirmMasterRsp
		{
			get
			{
				return this.dataObject as SCPKG_CONFIRM_MASTER_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_REMOVE_MASTER_REQ stRemoveMasterReq
		{
			get
			{
				return this.dataObject as CSPKG_REMOVE_MASTER_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_REMOVE_MASTER_RSP stRemoveMasterRsp
		{
			get
			{
				return this.dataObject as SCPKG_REMOVE_MASTER_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_MASTERREQ_LIST stMasterReqList
		{
			get
			{
				return this.dataObject as SCPKG_MASTERREQ_LIST;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_MASTERREQ_NTF stMasterReqNtf
		{
			get
			{
				return this.dataObject as SCPKG_MASTERREQ_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_MASTERSTUDENT_INFO stMasterStudentInfo
		{
			get
			{
				return this.dataObject as SCPKG_MASTERSTUDENT_INFO;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_MASTERSTUDENT_ADD stMasterStudentAdd
		{
			get
			{
				return this.dataObject as SCPKG_MASTERSTUDENT_ADD;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_MASTERSTUDENT_DEL stMasterStudentDel
		{
			get
			{
				return this.dataObject as SCPKG_MASTERSTUDENT_DEL;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_GRADUATE_NTF stGraduateNtf
		{
			get
			{
				return this.dataObject as SCPKG_GRADUATE_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_MASTERACNTDATA_NTF stMasterAcntDataNtf
		{
			get
			{
				return this.dataObject as SCPKG_MASTERACNTDATA_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_GETSTUDENTLIST_REQ stGetStudentListReq
		{
			get
			{
				return this.dataObject as CSPKG_GETSTUDENTLIST_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_GETSTUDENTLIST_RSP stGetStudentListRsp
		{
			get
			{
				return this.dataObject as SCPKG_GETSTUDENTLIST_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CMD_NTF_MASTERSTUDENT_LOGIN_STATUS stMasterStudentLoginNtf
		{
			get
			{
				return this.dataObject as SCPKG_CMD_NTF_MASTERSTUDENT_LOGIN_STATUS;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_MASTERSTUDENT_NOASKFOR_NTF stMasterStudentNoAskforNtf
		{
			get
			{
				return this.dataObject as SCPKG_MASTERSTUDENT_NOASKFOR_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_SHOWRECENTUSEDHERO_REQ stShowRecentUsedHeroReq
		{
			get
			{
				return this.dataObject as CSPKG_SHOWRECENTUSEDHERO_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_PROFITLIMIT_NTF stProfitLimitNtf
		{
			get
			{
				return this.dataObject as SCPKG_PROFITLIMIT_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CHGNAMECD_NTF stChgNameCDNtf
		{
			get
			{
				return this.dataObject as SCPKG_CHGNAMECD_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_PVPBAN_NTF stPvPBanNtf
		{
			get
			{
				return this.dataObject as SCPKG_PVPBAN_NTF;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CHG_USEDHEROEQUIP_REQ stChgUsedHeroEquipReq
		{
			get
			{
				return this.dataObject as CSPKG_CHG_USEDHEROEQUIP_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CHG_USEDHEROEQUIP_RSP stChgUsedHeroEquipRsp
		{
			get
			{
				return this.dataObject as SCPKG_CHG_USEDHEROEQUIP_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CHG_HEROEQUIPNAME_REQ stChgHeroEquipNameReq
		{
			get
			{
				return this.dataObject as CSPKG_CHG_HEROEQUIPNAME_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CHG_HEROEQUIPNAME_RSP stChgHeroEquipNameRsp
		{
			get
			{
				return this.dataObject as SCPKG_CHG_HEROEQUIPNAME_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_OPERATE_USER_PRIVACY_REQ stOperateUserPrivacyReq
		{
			get
			{
				return this.dataObject as CSPKG_OPERATE_USER_PRIVACY_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_OPERATE_USER_PRIVACY_RSP stOperateUserPrivacyRsp
		{
			get
			{
				return this.dataObject as SCPKG_OPERATE_USER_PRIVACY_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CLTUPLOADDATA_REQ stCltUploadDataReq
		{
			get
			{
				return this.dataObject as SCPKG_CLTUPLOADDATA_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CLTUPLOADDATA_RSP stCltUploadDataRsp
		{
			get
			{
				return this.dataObject as CSPKG_CLTUPLOADDATA_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_NTF_SVRRESTART stSvrRestartNtf
		{
			get
			{
				return this.dataObject as SCPKG_NTF_SVRRESTART;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CHG_FRIEND_CARD_REQ stChgFriendCardReq
		{
			get
			{
				return this.dataObject as CSPKG_CHG_FRIEND_CARD_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CHG_FRIEND_CARD_RSP stChgFriendCardRsp
		{
			get
			{
				return this.dataObject as SCPKG_CHG_FRIEND_CARD_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_CHG_OTHERSTATE_BIT_REQ stChgOtherStatebBitReq
		{
			get
			{
				return this.dataObject as CSPKG_CHG_OTHERSTATE_BIT_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_CHG_OTHERSTATE_BIT_RSP stChgOtherStatebBitRsp
		{
			get
			{
				return this.dataObject as SCPKG_CHG_OTHERSTATE_BIT_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_RESERVE_MSG_REQ stCltReserveMsgReq
		{
			get
			{
				return this.dataObject as CSPKG_RESERVE_MSG_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_RESERVE_MSG_RSP stSvrReserveMsgRsp
		{
			get
			{
				return this.dataObject as SCPKG_RESERVE_MSG_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public SCPKG_RESERVE_MSG_REQ stSvrReserveMsgReq
		{
			get
			{
				return this.dataObject as SCPKG_RESERVE_MSG_REQ;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public CSPKG_RESERVE_MSG_RSP stCltReserveMsgRsp
		{
			get
			{
				return this.dataObject as CSPKG_RESERVE_MSG_RSP;
			}
			set
			{
				this.dataObject = value;
			}
		}

		public ProtocolObject select(long selector)
		{
			if (selector <= 1081L)
			{
				this.select_1000_1081(selector);
			}
			else if (selector <= 1144L)
			{
				this.select_1082_1144(selector);
			}
			else if (selector <= 1215L)
			{
				this.select_1145_1215(selector);
			}
			else if (selector <= 1329L)
			{
				this.select_1216_1329(selector);
			}
			else if (selector <= 1441L)
			{
				this.select_1330_1441(selector);
			}
			else if (selector <= 1906L)
			{
				this.select_1442_1906(selector);
			}
			else if (selector <= 2230L)
			{
				this.select_1907_2230(selector);
			}
			else if (selector <= 2602L)
			{
				this.select_2231_2602(selector);
			}
			else if (selector <= 4102L)
			{
				this.select_2603_4102(selector);
			}
			else if (selector <= 5201L)
			{
				this.select_4103_5201(selector);
			}
			else if (selector <= 5307L)
			{
				this.select_5202_5307(selector);
			}
			else if (selector <= 5611L)
			{
				this.select_5308_5611(selector);
			}
			else if (selector <= 5616L)
			{
				this.select_5612_5616(selector);
			}
			else if (this.dataObject != null)
			{
				this.dataObject.Release();
				this.dataObject = null;
			}
			return this.dataObject;
		}

		public TdrError.ErrorType construct(long selector)
		{
			TdrError.ErrorType result = TdrError.ErrorType.TDR_NO_ERROR;
			ProtocolObject protocolObject = this.select(selector);
			if (protocolObject != null)
			{
				return protocolObject.construct();
			}
			if (this.szData == null)
			{
				this.szData = new byte[256000];
			}
			return result;
		}

		public TdrError.ErrorType pack(long selector, ref byte[] buffer, int size, ref int usedSize, uint cutVer)
		{
			if (buffer.GetLength(0) == 0 || size > buffer.GetLength(0))
			{
				return TdrError.ErrorType.TDR_ERR_INVALID_BUFFER_PARAMETER;
			}
			TdrWriteBuf tdrWriteBuf = ClassObjPool<TdrWriteBuf>.Get();
			tdrWriteBuf.set(ref buffer, size);
			TdrError.ErrorType errorType = this.pack(selector, ref tdrWriteBuf, cutVer);
			if (errorType == TdrError.ErrorType.TDR_NO_ERROR)
			{
				buffer = tdrWriteBuf.getBeginPtr();
				usedSize = tdrWriteBuf.getUsedSize();
			}
			tdrWriteBuf.Release();
			return errorType;
		}

		public TdrError.ErrorType pack(long selector, ref TdrWriteBuf destBuf, uint cutVer)
		{
			if (cutVer == 0u || CSPkgBody.CURRVERSION < cutVer)
			{
				cutVer = CSPkgBody.CURRVERSION;
			}
			if (CSPkgBody.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = TdrError.ErrorType.TDR_NO_ERROR;
			ProtocolObject protocolObject = this.select(selector);
			if (protocolObject != null)
			{
				return protocolObject.pack(ref destBuf, cutVer);
			}
			for (int i = 0; i < 256000; i++)
			{
				errorType = destBuf.writeUInt8(this.szData[i]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public TdrError.ErrorType unpack(long selector, ref byte[] buffer, int size, ref int usedSize, uint cutVer)
		{
			if (buffer.GetLength(0) == 0 || size > buffer.GetLength(0))
			{
				return TdrError.ErrorType.TDR_ERR_INVALID_BUFFER_PARAMETER;
			}
			TdrReadBuf tdrReadBuf = ClassObjPool<TdrReadBuf>.Get();
			tdrReadBuf.set(ref buffer, size);
			TdrError.ErrorType result = this.unpack(selector, ref tdrReadBuf, cutVer);
			usedSize = tdrReadBuf.getUsedSize();
			tdrReadBuf.Release();
			return result;
		}

		public TdrError.ErrorType unpack(long selector, ref TdrReadBuf srcBuf, uint cutVer)
		{
			if (cutVer == 0u || CSPkgBody.CURRVERSION < cutVer)
			{
				cutVer = CSPkgBody.CURRVERSION;
			}
			if (CSPkgBody.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = TdrError.ErrorType.TDR_NO_ERROR;
			ProtocolObject protocolObject = this.select(selector);
			if (protocolObject != null)
			{
				return protocolObject.unpack(ref srcBuf, cutVer);
			}
			for (int i = 0; i < 256000; i++)
			{
				errorType = srcBuf.readUInt8(ref this.szData[i]);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		private void select_1000_1081(long selector)
		{
			if (selector >= 1000L && selector <= 1081L)
			{
				switch ((int)(selector - 1000L))
				{
				case 0:
					if (!(this.dataObject is CSPKG_CMD_HEARTBEAT))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_HEARTBEAT)ProtocolObjectPool.Get(CSPKG_CMD_HEARTBEAT.CLASS_ID);
					}
					return;
				case 1:
					if (!(this.dataObject is SCPKG_GAMELOGINDISPATCH))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_GAMELOGINDISPATCH)ProtocolObjectPool.Get(SCPKG_GAMELOGINDISPATCH.CLASS_ID);
					}
					return;
				case 2:
					if (!(this.dataObject is CSPKG_CMD_GAMELOGINREQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_GAMELOGINREQ)ProtocolObjectPool.Get(CSPKG_CMD_GAMELOGINREQ.CLASS_ID);
					}
					return;
				case 3:
					if (!(this.dataObject is SCPKG_CMD_GAMELOGINRSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_GAMELOGINRSP)ProtocolObjectPool.Get(SCPKG_CMD_GAMELOGINRSP.CLASS_ID);
					}
					return;
				case 4:
					if (!(this.dataObject is CSPKG_GAMING_UPERMSG))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_GAMING_UPERMSG)ProtocolObjectPool.Get(CSPKG_GAMING_UPERMSG.CLASS_ID);
					}
					return;
				case 7:
					if (!(this.dataObject is SCPKG_NTF_ACNT_REGISTER))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_NTF_ACNT_REGISTER)ProtocolObjectPool.Get(SCPKG_NTF_ACNT_REGISTER.CLASS_ID);
					}
					return;
				case 8:
					if (!(this.dataObject is CSPKG_ACNT_REGISTER_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_ACNT_REGISTER_REQ)ProtocolObjectPool.Get(CSPKG_ACNT_REGISTER_REQ.CLASS_ID);
					}
					return;
				case 9:
					if (!(this.dataObject is CSPKG_ACNT_REGISTER_RES))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_ACNT_REGISTER_RES)ProtocolObjectPool.Get(CSPKG_ACNT_REGISTER_RES.CLASS_ID);
					}
					return;
				case 10:
					if (!(this.dataObject is SCPKG_NTF_ACNT_INFO_UPD))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_NTF_ACNT_INFO_UPD)ProtocolObjectPool.Get(SCPKG_NTF_ACNT_INFO_UPD.CLASS_ID);
					}
					return;
				case 11:
					if (!(this.dataObject is SCPKG_NTF_ACNT_LEVELUP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_NTF_ACNT_LEVELUP)ProtocolObjectPool.Get(SCPKG_NTF_ACNT_LEVELUP.CLASS_ID);
					}
					return;
				case 12:
					if (!(this.dataObject is CSPKG_CMD_CHEATCMD))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_CHEATCMD)ProtocolObjectPool.Get(CSPKG_CMD_CHEATCMD.CLASS_ID);
					}
					return;
				case 13:
					if (!(this.dataObject is SCPKG_CMD_LOGINFINISHNTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_LOGINFINISHNTF)ProtocolObjectPool.Get(SCPKG_CMD_LOGINFINISHNTF.CLASS_ID);
					}
					return;
				case 14:
					if (!(this.dataObject is SCPKG_CMD_RELOGINNOW))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_RELOGINNOW)ProtocolObjectPool.Get(SCPKG_CMD_RELOGINNOW.CLASS_ID);
					}
					return;
				case 15:
					if (!(this.dataObject is SCPKG_NTF_ACNT_PVPLEVELUP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_NTF_ACNT_PVPLEVELUP)ProtocolObjectPool.Get(SCPKG_NTF_ACNT_PVPLEVELUP.CLASS_ID);
					}
					return;
				case 16:
					if (!(this.dataObject is CSPKG_CMD_GAMELOGOUTREQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_GAMELOGOUTREQ)ProtocolObjectPool.Get(CSPKG_CMD_GAMELOGOUTREQ.CLASS_ID);
					}
					return;
				case 17:
					if (!(this.dataObject is SCPKG_CMD_GAMELOGOUTRSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_GAMELOGOUTRSP)ProtocolObjectPool.Get(SCPKG_CMD_GAMELOGOUTRSP.CLASS_ID);
					}
					return;
				case 18:
					if (!(this.dataObject is SCPKG_CMD_LOGINSYN_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_LOGINSYN_REQ)ProtocolObjectPool.Get(SCPKG_CMD_LOGINSYN_REQ.CLASS_ID);
					}
					return;
				case 19:
					if (!(this.dataObject is CSPKG_CMD_LOGINSYN_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_LOGINSYN_RSP)ProtocolObjectPool.Get(CSPKG_CMD_LOGINSYN_RSP.CLASS_ID);
					}
					return;
				case 20:
					if (!(this.dataObject is CSPKG_CREATEULTIGAMEREQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CREATEULTIGAMEREQ)ProtocolObjectPool.Get(CSPKG_CREATEULTIGAMEREQ.CLASS_ID);
					}
					return;
				case 22:
					if (!(this.dataObject is SCPKG_JOINMULTIGAMERSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_JOINMULTIGAMERSP)ProtocolObjectPool.Get(SCPKG_JOINMULTIGAMERSP.CLASS_ID);
					}
					return;
				case 23:
					if (!(this.dataObject is CSPKG_QUITMULTIGAMEREQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_QUITMULTIGAMEREQ)ProtocolObjectPool.Get(CSPKG_QUITMULTIGAMEREQ.CLASS_ID);
					}
					return;
				case 24:
					if (!(this.dataObject is SCPKG_QUITMULTIGAMERSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_QUITMULTIGAMERSP)ProtocolObjectPool.Get(SCPKG_QUITMULTIGAMERSP.CLASS_ID);
					}
					return;
				case 25:
					if (!(this.dataObject is SCPKG_ROOMCHGNTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_ROOMCHGNTF)ProtocolObjectPool.Get(SCPKG_ROOMCHGNTF.CLASS_ID);
					}
					return;
				case 26:
					if (!(this.dataObject is SCPKG_ASK_ACNT_TRANS_VISITORSVRDATA))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_ASK_ACNT_TRANS_VISITORSVRDATA)ProtocolObjectPool.Get(SCPKG_ASK_ACNT_TRANS_VISITORSVRDATA.CLASS_ID);
					}
					return;
				case 27:
					if (!(this.dataObject is CSPKG_RSP_ACNT_TRANS_VISITORSVRDATA))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_RSP_ACNT_TRANS_VISITORSVRDATA)ProtocolObjectPool.Get(CSPKG_RSP_ACNT_TRANS_VISITORSVRDATA.CLASS_ID);
					}
					return;
				case 30:
					if (!(this.dataObject is SCPKG_GAMECONN_REDIRECT))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_GAMECONN_REDIRECT)ProtocolObjectPool.Get(SCPKG_GAMECONN_REDIRECT.CLASS_ID);
					}
					return;
				case 34:
					if (!(this.dataObject is SCPKG_FRAPBOOT_SINGLE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_FRAPBOOT_SINGLE)ProtocolObjectPool.Get(SCPKG_FRAPBOOT_SINGLE.CLASS_ID);
					}
					return;
				case 35:
					if (!(this.dataObject is SCPKG_FRAPBOOTINFO))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_FRAPBOOTINFO)ProtocolObjectPool.Get(SCPKG_FRAPBOOTINFO.CLASS_ID);
					}
					return;
				case 36:
					if (!(this.dataObject is CSPKG_REQUESTFRAPBOOTSINGLE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_REQUESTFRAPBOOTSINGLE)ProtocolObjectPool.Get(CSPKG_REQUESTFRAPBOOTSINGLE.CLASS_ID);
					}
					return;
				case 37:
					if (!(this.dataObject is CSPKG_REQUESTFRAPBOOTTIMEOUT))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_REQUESTFRAPBOOTTIMEOUT)ProtocolObjectPool.Get(CSPKG_REQUESTFRAPBOOTTIMEOUT.CLASS_ID);
					}
					return;
				case 40:
					if (!(this.dataObject is SCPKG_OFFINGRESTART_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_OFFINGRESTART_REQ)ProtocolObjectPool.Get(SCPKG_OFFINGRESTART_REQ.CLASS_ID);
					}
					return;
				case 41:
					if (!(this.dataObject is CSPKG_OFFINGRESTART_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_OFFINGRESTART_RSP)ProtocolObjectPool.Get(CSPKG_OFFINGRESTART_RSP.CLASS_ID);
					}
					return;
				case 42:
					if (!(this.dataObject is SCPKG_CMD_GAMELOGINLIMIT))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_GAMELOGINLIMIT)ProtocolObjectPool.Get(SCPKG_CMD_GAMELOGINLIMIT.CLASS_ID);
					}
					return;
				case 43:
					if (!(this.dataObject is SCPKG_CMD_BANTIME_CHG))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_BANTIME_CHG)ProtocolObjectPool.Get(SCPKG_CMD_BANTIME_CHG.CLASS_ID);
					}
					return;
				case 44:
					if (!(this.dataObject is SCPKG_ISACCEPT_AIPLAYER_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_ISACCEPT_AIPLAYER_REQ)ProtocolObjectPool.Get(SCPKG_ISACCEPT_AIPLAYER_REQ.CLASS_ID);
					}
					return;
				case 45:
					if (!(this.dataObject is CSPKG_ISACCEPT_AIPLAYER_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_ISACCEPT_AIPLAYER_RSP)ProtocolObjectPool.Get(CSPKG_ISACCEPT_AIPLAYER_RSP.CLASS_ID);
					}
					return;
				case 46:
					if (!(this.dataObject is SCPKG_NOTICE_HANGUP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_NOTICE_HANGUP)ProtocolObjectPool.Get(SCPKG_NOTICE_HANGUP.CLASS_ID);
					}
					return;
				case 50:
					if (!(this.dataObject is CSPKG_STARTSINGLEGAMEREQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_STARTSINGLEGAMEREQ)ProtocolObjectPool.Get(CSPKG_STARTSINGLEGAMEREQ.CLASS_ID);
					}
					return;
				case 51:
					if (!(this.dataObject is SCPKG_STARTSINGLEGAMERSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_STARTSINGLEGAMERSP)ProtocolObjectPool.Get(SCPKG_STARTSINGLEGAMERSP.CLASS_ID);
					}
					return;
				case 52:
					if (!(this.dataObject is CSPKG_SINGLEGAMEFINREQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_SINGLEGAMEFINREQ)ProtocolObjectPool.Get(CSPKG_SINGLEGAMEFINREQ.CLASS_ID);
					}
					return;
				case 53:
					if (!(this.dataObject is SCPKG_SINGLEGAMEFINRSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_SINGLEGAMEFINRSP)ProtocolObjectPool.Get(SCPKG_SINGLEGAMEFINRSP.CLASS_ID);
					}
					return;
				case 54:
					if (!(this.dataObject is CSPKG_SINGLEGAMESWEEPREQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_SINGLEGAMESWEEPREQ)ProtocolObjectPool.Get(CSPKG_SINGLEGAMESWEEPREQ.CLASS_ID);
					}
					return;
				case 55:
					if (!(this.dataObject is SCPKG_SINGLEGAMESWEEPRSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_SINGLEGAMESWEEPRSP)ProtocolObjectPool.Get(SCPKG_SINGLEGAMESWEEPRSP.CLASS_ID);
					}
					return;
				case 56:
					if (!(this.dataObject is CSPKG_GET_CHAPTER_REWARD_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_GET_CHAPTER_REWARD_REQ)ProtocolObjectPool.Get(CSPKG_GET_CHAPTER_REWARD_REQ.CLASS_ID);
					}
					return;
				case 57:
					if (!(this.dataObject is SCPKG_GET_CHAPTER_REWARD_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_GET_CHAPTER_REWARD_RSP)ProtocolObjectPool.Get(SCPKG_GET_CHAPTER_REWARD_RSP.CLASS_ID);
					}
					return;
				case 58:
					if (!(this.dataObject is CSPKG_QUITSINGLEGAMEREQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_QUITSINGLEGAMEREQ)ProtocolObjectPool.Get(CSPKG_QUITSINGLEGAMEREQ.CLASS_ID);
					}
					return;
				case 59:
					if (!(this.dataObject is SCPKG_QUITSINGLEGAMERSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_QUITSINGLEGAMERSP)ProtocolObjectPool.Get(SCPKG_QUITSINGLEGAMERSP.CLASS_ID);
					}
					return;
				case 60:
					if (!(this.dataObject is CSPKG_ASKINMULTGAME_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_ASKINMULTGAME_REQ)ProtocolObjectPool.Get(CSPKG_ASKINMULTGAME_REQ.CLASS_ID);
					}
					return;
				case 61:
					if (!(this.dataObject is SCPKG_ASKINMULTGAME_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_ASKINMULTGAME_RSP)ProtocolObjectPool.Get(SCPKG_ASKINMULTGAME_RSP.CLASS_ID);
					}
					return;
				case 62:
					if (!(this.dataObject is CSPKG_SECURE_INFO_START_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_SECURE_INFO_START_REQ)ProtocolObjectPool.Get(CSPKG_SECURE_INFO_START_REQ.CLASS_ID);
					}
					return;
				case 69:
					if (!(this.dataObject is SCPKG_MULTGAME_BEGINBAN))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_MULTGAME_BEGINBAN)ProtocolObjectPool.Get(SCPKG_MULTGAME_BEGINBAN.CLASS_ID);
					}
					return;
				case 70:
					if (!(this.dataObject is SCPKG_MULTGAME_BEGINPICK))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_MULTGAME_BEGINPICK)ProtocolObjectPool.Get(SCPKG_MULTGAME_BEGINPICK.CLASS_ID);
					}
					return;
				case 71:
					if (!(this.dataObject is SCPKG_MULTGAME_BEGINADJUST))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_MULTGAME_BEGINADJUST)ProtocolObjectPool.Get(SCPKG_MULTGAME_BEGINADJUST.CLASS_ID);
					}
					return;
				case 75:
					if (!(this.dataObject is SCPKG_MULTGAME_BEGINLOAD))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_MULTGAME_BEGINLOAD)ProtocolObjectPool.Get(SCPKG_MULTGAME_BEGINLOAD.CLASS_ID);
					}
					return;
				case 76:
					if (!(this.dataObject is CSPKG_MULTGAME_LOADFIN))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_MULTGAME_LOADFIN)ProtocolObjectPool.Get(CSPKG_MULTGAME_LOADFIN.CLASS_ID);
					}
					return;
				case 77:
					if (!(this.dataObject is SCPKG_MULTGAME_BEGINFIGHT))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_MULTGAME_BEGINFIGHT)ProtocolObjectPool.Get(SCPKG_MULTGAME_BEGINFIGHT.CLASS_ID);
					}
					return;
				case 78:
					if (!(this.dataObject is SCPKG_MULTGAMEREADYNTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_MULTGAMEREADYNTF)ProtocolObjectPool.Get(SCPKG_MULTGAMEREADYNTF.CLASS_ID);
					}
					return;
				case 79:
					if (!(this.dataObject is SCPKG_MULTGAMEABORTNTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_MULTGAMEABORTNTF)ProtocolObjectPool.Get(SCPKG_MULTGAMEABORTNTF.CLASS_ID);
					}
					return;
				case 80:
					if (!(this.dataObject is CSPKG_MULTGAME_GAMEOVER))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_MULTGAME_GAMEOVER)ProtocolObjectPool.Get(CSPKG_MULTGAME_GAMEOVER.CLASS_ID);
					}
					return;
				case 81:
					if (!(this.dataObject is SCPKG_MULTGAME_GAMEOVER))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_MULTGAME_GAMEOVER)ProtocolObjectPool.Get(SCPKG_MULTGAME_GAMEOVER.CLASS_ID);
					}
					return;
				}
			}
			if (this.dataObject != null)
			{
				this.dataObject.Release();
				this.dataObject = null;
			}
		}

		private void select_1082_1144(long selector)
		{
			if (selector >= 1082L && selector <= 1144L)
			{
				switch ((int)(selector - 1082L))
				{
				case 0:
					if (!(this.dataObject is SCPKG_MULTGAME_SETTLEGAIN))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_MULTGAME_SETTLEGAIN)ProtocolObjectPool.Get(SCPKG_MULTGAME_SETTLEGAIN.CLASS_ID);
					}
					return;
				case 1:
					if (!(this.dataObject is CSPKG_MULTGAME_LOADPROCESS))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_MULTGAME_LOADPROCESS)ProtocolObjectPool.Get(CSPKG_MULTGAME_LOADPROCESS.CLASS_ID);
					}
					return;
				case 2:
					if (!(this.dataObject is SCPKG_MULTGAME_LOADPROCESS))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_MULTGAME_LOADPROCESS)ProtocolObjectPool.Get(SCPKG_MULTGAME_LOADPROCESS.CLASS_ID);
					}
					return;
				case 3:
					if (!(this.dataObject is SCPKG_MULTGAME_NTF_CLT_GAMEOVER))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_MULTGAME_NTF_CLT_GAMEOVER)ProtocolObjectPool.Get(SCPKG_MULTGAME_NTF_CLT_GAMEOVER.CLASS_ID);
					}
					return;
				case 4:
					if (!(this.dataObject is CSPKG_MULTGAME_RUNAWAY_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_MULTGAME_RUNAWAY_REQ)ProtocolObjectPool.Get(CSPKG_MULTGAME_RUNAWAY_REQ.CLASS_ID);
					}
					return;
				case 5:
					if (!(this.dataObject is SCPKG_MULTGAME_RUNAWAY_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_MULTGAME_RUNAWAY_RSP)ProtocolObjectPool.Get(SCPKG_MULTGAME_RUNAWAY_RSP.CLASS_ID);
					}
					return;
				case 6:
					if (!(this.dataObject is SCPKG_MULTGAME_RUNAWAY_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_MULTGAME_RUNAWAY_NTF)ProtocolObjectPool.Get(SCPKG_MULTGAME_RUNAWAY_NTF.CLASS_ID);
					}
					return;
				case 7:
					if (!(this.dataObject is SCPKG_MULTGAMERECOVERNTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_MULTGAMERECOVERNTF)ProtocolObjectPool.Get(SCPKG_MULTGAMERECOVERNTF.CLASS_ID);
					}
					return;
				case 8:
					if (!(this.dataObject is CSPKG_RECOVERGAMEFRAP_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_RECOVERGAMEFRAP_REQ)ProtocolObjectPool.Get(CSPKG_RECOVERGAMEFRAP_REQ.CLASS_ID);
					}
					return;
				case 9:
					if (!(this.dataObject is SCPKG_RECOVERGAMEFRAP_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_RECOVERGAMEFRAP_RSP)ProtocolObjectPool.Get(SCPKG_RECOVERGAMEFRAP_RSP.CLASS_ID);
					}
					return;
				case 10:
					if (!(this.dataObject is SCPKG_RECONNGAME_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_RECONNGAME_NTF)ProtocolObjectPool.Get(SCPKG_RECONNGAME_NTF.CLASS_ID);
					}
					return;
				case 11:
					if (!(this.dataObject is CSPKG_RECOVERGAMESUCC))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_RECOVERGAMESUCC)ProtocolObjectPool.Get(CSPKG_RECOVERGAMESUCC.CLASS_ID);
					}
					return;
				case 12:
					if (!(this.dataObject is SCPKG_MULTGAME_DISCONN_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_MULTGAME_DISCONN_NTF)ProtocolObjectPool.Get(SCPKG_MULTGAME_DISCONN_NTF.CLASS_ID);
					}
					return;
				case 13:
					if (!(this.dataObject is SCPKG_MULTGAME_RECONN_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_MULTGAME_RECONN_NTF)ProtocolObjectPool.Get(SCPKG_MULTGAME_RECONN_NTF.CLASS_ID);
					}
					return;
				case 14:
					if (!(this.dataObject is CSPKG_KFRAPLATERCHG_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_KFRAPLATERCHG_REQ)ProtocolObjectPool.Get(CSPKG_KFRAPLATERCHG_REQ.CLASS_ID);
					}
					return;
				case 15:
					if (!(this.dataObject is SCPKG_KFRAPLATERCHG_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_KFRAPLATERCHG_NTF)ProtocolObjectPool.Get(SCPKG_KFRAPLATERCHG_NTF.CLASS_ID);
					}
					return;
				case 16:
					if (!(this.dataObject is CSPKG_MULTGAME_DIE_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_MULTGAME_DIE_REQ)ProtocolObjectPool.Get(CSPKG_MULTGAME_DIE_REQ.CLASS_ID);
					}
					return;
				case 17:
					if (!(this.dataObject is SCPKG_HANGUP_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_HANGUP_NTF)ProtocolObjectPool.Get(SCPKG_HANGUP_NTF.CLASS_ID);
					}
					return;
				case 19:
					if (!(this.dataObject is CSPKG_CMD_ITEMSALE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_ITEMSALE)ProtocolObjectPool.Get(CSPKG_CMD_ITEMSALE.CLASS_ID);
					}
					return;
				case 20:
					if (!(this.dataObject is SCPKG_CMD_ITEMADD))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_ITEMADD)ProtocolObjectPool.Get(SCPKG_CMD_ITEMADD.CLASS_ID);
					}
					return;
				case 21:
					if (!(this.dataObject is SCPKG_CMD_ITEMDEL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_ITEMDEL)ProtocolObjectPool.Get(SCPKG_CMD_ITEMDEL.CLASS_ID);
					}
					return;
				case 22:
					if (!(this.dataObject is CSPKG_CMD_EQUIPWEAR))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_EQUIPWEAR)ProtocolObjectPool.Get(CSPKG_CMD_EQUIPWEAR.CLASS_ID);
					}
					return;
				case 24:
					if (!(this.dataObject is SCPKG_CMD_EQUIPCHG))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_EQUIPCHG)ProtocolObjectPool.Get(SCPKG_CMD_EQUIPCHG.CLASS_ID);
					}
					return;
				case 25:
					if (!(this.dataObject is CSPKG_CMD_PROPUSE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_PROPUSE)ProtocolObjectPool.Get(CSPKG_CMD_PROPUSE.CLASS_ID);
					}
					return;
				case 26:
					if (!(this.dataObject is CSPKG_CMD_PKGQUERY))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_PKGQUERY)ProtocolObjectPool.Get(CSPKG_CMD_PKGQUERY.CLASS_ID);
					}
					return;
				case 27:
					if (!(this.dataObject is SCPKG_CMD_PKGDETAIL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_PKGDETAIL)ProtocolObjectPool.Get(SCPKG_CMD_PKGDETAIL.CLASS_ID);
					}
					return;
				case 28:
					if (!(this.dataObject is CSPKG_CMD_ITEMCOMP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_ITEMCOMP)ProtocolObjectPool.Get(CSPKG_CMD_ITEMCOMP.CLASS_ID);
					}
					return;
				case 29:
					if (!(this.dataObject is CSPKG_CMD_HEROADVANCE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_HEROADVANCE)ProtocolObjectPool.Get(CSPKG_CMD_HEROADVANCE.CLASS_ID);
					}
					return;
				case 30:
					if (!(this.dataObject is SCPKG_CMD_HEROADVANCE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_HEROADVANCE)ProtocolObjectPool.Get(SCPKG_CMD_HEROADVANCE.CLASS_ID);
					}
					return;
				case 31:
					if (!(this.dataObject is CSPKG_CMD_SHOPBUY))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_SHOPBUY)ProtocolObjectPool.Get(CSPKG_CMD_SHOPBUY.CLASS_ID);
					}
					return;
				case 32:
					if (!(this.dataObject is SCPKG_CMD_SHOPBUY))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_SHOPBUY)ProtocolObjectPool.Get(SCPKG_CMD_SHOPBUY.CLASS_ID);
					}
					return;
				case 33:
					if (!(this.dataObject is CSPKG_CMD_COINBUY))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_COINBUY)ProtocolObjectPool.Get(CSPKG_CMD_COINBUY.CLASS_ID);
					}
					return;
				case 34:
					if (!(this.dataObject is SCPKG_CMD_COINBUY))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_COINBUY)ProtocolObjectPool.Get(SCPKG_CMD_COINBUY.CLASS_ID);
					}
					return;
				case 35:
					if (!(this.dataObject is SCPKG_NTF_CLRSHOPBUYLIMIT))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_NTF_CLRSHOPBUYLIMIT)ProtocolObjectPool.Get(SCPKG_NTF_CLRSHOPBUYLIMIT.CLASS_ID);
					}
					return;
				case 36:
					if (!(this.dataObject is CSPKG_CMD_AUTOREFRESH))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_AUTOREFRESH)ProtocolObjectPool.Get(CSPKG_CMD_AUTOREFRESH.CLASS_ID);
					}
					return;
				case 37:
					if (!(this.dataObject is CSPKG_CMD_MANUALREFRESH))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_MANUALREFRESH)ProtocolObjectPool.Get(CSPKG_CMD_MANUALREFRESH.CLASS_ID);
					}
					return;
				case 38:
					if (!(this.dataObject is SCPKG_CMD_SHOPDETAIL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_SHOPDETAIL)ProtocolObjectPool.Get(SCPKG_CMD_SHOPDETAIL.CLASS_ID);
					}
					return;
				case 39:
					if (!(this.dataObject is CSPKG_CMD_SYMBOLNAMECHG))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_SYMBOLNAMECHG)ProtocolObjectPool.Get(CSPKG_CMD_SYMBOLNAMECHG.CLASS_ID);
					}
					return;
				case 40:
					if (!(this.dataObject is SCPKG_CMD_SYMBOLNAMECHG))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_SYMBOLNAMECHG)ProtocolObjectPool.Get(SCPKG_CMD_SYMBOLNAMECHG.CLASS_ID);
					}
					return;
				case 41:
					if (!(this.dataObject is CSPKG_CMD_HORNUSE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_HORNUSE)ProtocolObjectPool.Get(CSPKG_CMD_HORNUSE.CLASS_ID);
					}
					return;
				case 42:
					if (!(this.dataObject is SCPKG_CMD_HORNUSE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_HORNUSE)ProtocolObjectPool.Get(SCPKG_CMD_HORNUSE.CLASS_ID);
					}
					return;
				case 43:
					if (!(this.dataObject is CSPKG_CMD_ITEMBUY))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_ITEMBUY)ProtocolObjectPool.Get(CSPKG_CMD_ITEMBUY.CLASS_ID);
					}
					return;
				case 44:
					if (!(this.dataObject is SCPKG_CMD_ITEMBUY))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_ITEMBUY)ProtocolObjectPool.Get(SCPKG_CMD_ITEMBUY.CLASS_ID);
					}
					return;
				case 45:
					if (!(this.dataObject is SCPKG_NTF_SHOPTIMEOUT))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_NTF_SHOPTIMEOUT)ProtocolObjectPool.Get(SCPKG_NTF_SHOPTIMEOUT.CLASS_ID);
					}
					return;
				case 46:
					if (!(this.dataObject is SCPKG_CMD_CLRSHOPREFRESH))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_CLRSHOPREFRESH)ProtocolObjectPool.Get(SCPKG_CMD_CLRSHOPREFRESH.CLASS_ID);
					}
					return;
				case 48:
					if (!(this.dataObject is CSPKG_CMD_SYMBOLCOMP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_SYMBOLCOMP)ProtocolObjectPool.Get(CSPKG_CMD_SYMBOLCOMP.CLASS_ID);
					}
					return;
				case 49:
					if (!(this.dataObject is SCPKG_CMD_SYMBOLCOMP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_SYMBOLCOMP)ProtocolObjectPool.Get(SCPKG_CMD_SYMBOLCOMP.CLASS_ID);
					}
					return;
				case 50:
					if (!(this.dataObject is CSPKG_CMD_SYMBOLQUERY))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_SYMBOLQUERY)ProtocolObjectPool.Get(CSPKG_CMD_SYMBOLQUERY.CLASS_ID);
					}
					return;
				case 51:
					if (!(this.dataObject is SCPKG_CMD_SYMBOLDETAIL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_SYMBOLDETAIL)ProtocolObjectPool.Get(SCPKG_CMD_SYMBOLDETAIL.CLASS_ID);
					}
					return;
				case 52:
					if (!(this.dataObject is CSPKG_CMD_SYMBOLWEAR))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_SYMBOLWEAR)ProtocolObjectPool.Get(CSPKG_CMD_SYMBOLWEAR.CLASS_ID);
					}
					return;
				case 53:
					if (!(this.dataObject is CSPKG_CMD_SYMBOLOFF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_SYMBOLOFF)ProtocolObjectPool.Get(CSPKG_CMD_SYMBOLOFF.CLASS_ID);
					}
					return;
				case 54:
					if (!(this.dataObject is SCPKG_CMD_SYMBOLCHG))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_SYMBOLCHG)ProtocolObjectPool.Get(SCPKG_CMD_SYMBOLCHG.CLASS_ID);
					}
					return;
				case 55:
					if (!(this.dataObject is CSPKG_CMD_SYMBOLPAGESEL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_SYMBOLPAGESEL)ProtocolObjectPool.Get(CSPKG_CMD_SYMBOLPAGESEL.CLASS_ID);
					}
					return;
				case 56:
					if (!(this.dataObject is SCPKG_CMD_SYMBOLPAGESEL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_SYMBOLPAGESEL)ProtocolObjectPool.Get(SCPKG_CMD_SYMBOLPAGESEL.CLASS_ID);
					}
					return;
				case 57:
					if (!(this.dataObject is SCPKG_CMD_SYMBOLCHG_LIST))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_SYMBOLCHG_LIST)ProtocolObjectPool.Get(SCPKG_CMD_SYMBOLCHG_LIST.CLASS_ID);
					}
					return;
				case 58:
					if (!(this.dataObject is CSPKG_CMD_EQUIPSMELT))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_EQUIPSMELT)ProtocolObjectPool.Get(CSPKG_CMD_EQUIPSMELT.CLASS_ID);
					}
					return;
				case 59:
					if (!(this.dataObject is CSPKG_CMD_EQUIPENCHANT))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_EQUIPENCHANT)ProtocolObjectPool.Get(CSPKG_CMD_EQUIPENCHANT.CLASS_ID);
					}
					return;
				case 60:
					if (!(this.dataObject is SCPKG_CMD_EQUIPENCHANT))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_EQUIPENCHANT)ProtocolObjectPool.Get(SCPKG_CMD_EQUIPENCHANT.CLASS_ID);
					}
					return;
				case 61:
					if (!(this.dataObject is SCPKG_COINDRAW_RESULT))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_COINDRAW_RESULT)ProtocolObjectPool.Get(SCPKG_COINDRAW_RESULT.CLASS_ID);
					}
					return;
				case 62:
					if (!(this.dataObject is CSPKG_CMD_GEAR_LVLUP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_GEAR_LVLUP)ProtocolObjectPool.Get(CSPKG_CMD_GEAR_LVLUP.CLASS_ID);
					}
					return;
				}
			}
			if (this.dataObject != null)
			{
				this.dataObject.Release();
				this.dataObject = null;
			}
		}

		private void select_1145_1215(long selector)
		{
			if (selector >= 1145L && selector <= 1215L)
			{
				switch ((int)(selector - 1145L))
				{
				case 0:
					if (!(this.dataObject is CSPKG_CMD_GEAR_LVLUPALL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_GEAR_LVLUPALL)ProtocolObjectPool.Get(CSPKG_CMD_GEAR_LVLUPALL.CLASS_ID);
					}
					return;
				case 1:
					if (!(this.dataObject is SCPKG_CMD_GEAR_LEVELINFO))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_GEAR_LEVELINFO)ProtocolObjectPool.Get(SCPKG_CMD_GEAR_LEVELINFO.CLASS_ID);
					}
					return;
				case 2:
					if (!(this.dataObject is CSPKG_CMD_GEAR_ADVANCE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_GEAR_ADVANCE)ProtocolObjectPool.Get(CSPKG_CMD_GEAR_ADVANCE.CLASS_ID);
					}
					return;
				case 3:
					if (!(this.dataObject is SCPKG_CMD_GEAR_ADVANCE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_GEAR_ADVANCE)ProtocolObjectPool.Get(SCPKG_CMD_GEAR_ADVANCE.CLASS_ID);
					}
					return;
				case 4:
					if (!(this.dataObject is SCPKG_CMD_PROPUSE_GIFTGET))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_PROPUSE_GIFTGET)ProtocolObjectPool.Get(SCPKG_CMD_PROPUSE_GIFTGET.CLASS_ID);
					}
					return;
				case 5:
					if (!(this.dataObject is CSPKG_CMD_ACNTCOUPONS))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_ACNTCOUPONS)ProtocolObjectPool.Get(CSPKG_CMD_ACNTCOUPONS.CLASS_ID);
					}
					return;
				case 6:
					if (!(this.dataObject is SCPKG_CMD_ACNTCOUPONS))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_ACNTCOUPONS)ProtocolObjectPool.Get(SCPKG_CMD_ACNTCOUPONS.CLASS_ID);
					}
					return;
				case 7:
					if (!(this.dataObject is SCPKG_CMD_SPECIAL_SALEINFO))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_SPECIAL_SALEINFO)ProtocolObjectPool.Get(SCPKG_CMD_SPECIAL_SALEINFO.CLASS_ID);
					}
					return;
				case 8:
					if (!(this.dataObject is CSPKG_CMD_SPECSALEBUY))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_SPECSALEBUY)ProtocolObjectPool.Get(CSPKG_CMD_SPECSALEBUY.CLASS_ID);
					}
					return;
				case 9:
					if (!(this.dataObject is SCPKG_CMD_SPECSALEBUY))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_SPECSALEBUY)ProtocolObjectPool.Get(SCPKG_CMD_SPECSALEBUY.CLASS_ID);
					}
					return;
				case 10:
					if (!(this.dataObject is CSPKG_CMD_SYMBOL_MAKE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_SYMBOL_MAKE)ProtocolObjectPool.Get(CSPKG_CMD_SYMBOL_MAKE.CLASS_ID);
					}
					return;
				case 11:
					if (!(this.dataObject is CSPKG_CMD_SYMBOL_BREAK))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_SYMBOL_BREAK)ProtocolObjectPool.Get(CSPKG_CMD_SYMBOL_BREAK.CLASS_ID);
					}
					return;
				case 12:
					if (!(this.dataObject is SCPKG_CMD_SYMBOL_MAKE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_SYMBOL_MAKE)ProtocolObjectPool.Get(SCPKG_CMD_SYMBOL_MAKE.CLASS_ID);
					}
					return;
				case 13:
					if (!(this.dataObject is SCPKG_CMD_SYMBOL_BREAK))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_SYMBOL_BREAK)ProtocolObjectPool.Get(SCPKG_CMD_SYMBOL_BREAK.CLASS_ID);
					}
					return;
				case 15:
					if (!(this.dataObject is CSPKG_CMD_COUPONS_REWARDGET))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_COUPONS_REWARDGET)ProtocolObjectPool.Get(CSPKG_CMD_COUPONS_REWARDGET.CLASS_ID);
					}
					return;
				case 16:
					if (!(this.dataObject is SCPKG_CMD_COUPONS_REWARDINFO))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_COUPONS_REWARDINFO)ProtocolObjectPool.Get(SCPKG_CMD_COUPONS_REWARDINFO.CLASS_ID);
					}
					return;
				case 17:
					if (!(this.dataObject is CSPKG_CMD_SYMBOLPAGE_CLR))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_SYMBOLPAGE_CLR)ProtocolObjectPool.Get(CSPKG_CMD_SYMBOLPAGE_CLR.CLASS_ID);
					}
					return;
				case 18:
					if (!(this.dataObject is SCPKG_CMD_SYMBOLPAGE_CLR))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_SYMBOLPAGE_CLR)ProtocolObjectPool.Get(SCPKG_CMD_SYMBOLPAGE_CLR.CLASS_ID);
					}
					return;
				case 19:
					if (!(this.dataObject is CSPKG_CMD_TALENT_BUY))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_TALENT_BUY)ProtocolObjectPool.Get(CSPKG_CMD_TALENT_BUY.CLASS_ID);
					}
					return;
				case 20:
					if (!(this.dataObject is SCPKG_CMD_TALENT_BUY))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_TALENT_BUY)ProtocolObjectPool.Get(SCPKG_CMD_TALENT_BUY.CLASS_ID);
					}
					return;
				case 21:
					if (!(this.dataObject is CSPKG_CMD_SKILLUNLOCK_SEL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_SKILLUNLOCK_SEL)ProtocolObjectPool.Get(CSPKG_CMD_SKILLUNLOCK_SEL.CLASS_ID);
					}
					return;
				case 22:
					if (!(this.dataObject is SCPKG_CMD_SKILLUNLOCK_SEL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_SKILLUNLOCK_SEL)ProtocolObjectPool.Get(SCPKG_CMD_SKILLUNLOCK_SEL.CLASS_ID);
					}
					return;
				case 23:
					if (!(this.dataObject is SCPKG_CMD_HERO_WAKECHG))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_HERO_WAKECHG)ProtocolObjectPool.Get(SCPKG_CMD_HERO_WAKECHG.CLASS_ID);
					}
					return;
				case 24:
					if (!(this.dataObject is CSPKG_CMD_HERO_WAKEOPT))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_HERO_WAKEOPT)ProtocolObjectPool.Get(CSPKG_CMD_HERO_WAKEOPT.CLASS_ID);
					}
					return;
				case 25:
					if (!(this.dataObject is SCPKG_CMD_HERO_WAKESTEP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_HERO_WAKESTEP)ProtocolObjectPool.Get(SCPKG_CMD_HERO_WAKESTEP.CLASS_ID);
					}
					return;
				case 26:
					if (!(this.dataObject is SCPKG_CMD_HEROWAKE_REWARD))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_HEROWAKE_REWARD)ProtocolObjectPool.Get(SCPKG_CMD_HEROWAKE_REWARD.CLASS_ID);
					}
					return;
				case 27:
					if (!(this.dataObject is SCPKG_CMD_PROPUSE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_PROPUSE)ProtocolObjectPool.Get(SCPKG_CMD_PROPUSE.CLASS_ID);
					}
					return;
				case 30:
					if (!(this.dataObject is CSPKG_CMD_SALERECMD_BUY))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_SALERECMD_BUY)ProtocolObjectPool.Get(CSPKG_CMD_SALERECMD_BUY.CLASS_ID);
					}
					return;
				case 31:
					if (!(this.dataObject is SCPKG_CMD_SALERECMD_BUY))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_SALERECMD_BUY)ProtocolObjectPool.Get(SCPKG_CMD_SALERECMD_BUY.CLASS_ID);
					}
					return;
				case 32:
					if (!(this.dataObject is CSPKG_CMD_RANDDRAW_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_RANDDRAW_REQ)ProtocolObjectPool.Get(CSPKG_CMD_RANDDRAW_REQ.CLASS_ID);
					}
					return;
				case 33:
					if (!(this.dataObject is SCPKG_CMD_RANDDRAW_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_RANDDRAW_RSP)ProtocolObjectPool.Get(SCPKG_CMD_RANDDRAW_RSP.CLASS_ID);
					}
					return;
				case 34:
					if (!(this.dataObject is SCPKG_NTF_RANDDRAW_SYNID))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_NTF_RANDDRAW_SYNID)ProtocolObjectPool.Get(SCPKG_NTF_RANDDRAW_SYNID.CLASS_ID);
					}
					return;
				case 35:
					if (!(this.dataObject is CSPKG_CMD_SYMBOLRCMD_WEAR))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_SYMBOLRCMD_WEAR)ProtocolObjectPool.Get(CSPKG_CMD_SYMBOLRCMD_WEAR.CLASS_ID);
					}
					return;
				case 36:
					if (!(this.dataObject is SCPKG_CMD_SYMBOLRCMD_WEAR))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_SYMBOLRCMD_WEAR)ProtocolObjectPool.Get(SCPKG_CMD_SYMBOLRCMD_WEAR.CLASS_ID);
					}
					return;
				case 37:
					if (!(this.dataObject is CSPKG_CMD_SYMBOLRCMD_SEL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_SYMBOLRCMD_SEL)ProtocolObjectPool.Get(CSPKG_CMD_SYMBOLRCMD_SEL.CLASS_ID);
					}
					return;
				case 38:
					if (!(this.dataObject is SCPKG_CMD_SYMBOLRCMD_SEL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_SYMBOLRCMD_SEL)ProtocolObjectPool.Get(SCPKG_CMD_SYMBOLRCMD_SEL.CLASS_ID);
					}
					return;
				case 39:
					if (!(this.dataObject is CSPKG_CMD_RAREEXCHANGE_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_RAREEXCHANGE_REQ)ProtocolObjectPool.Get(CSPKG_CMD_RAREEXCHANGE_REQ.CLASS_ID);
					}
					return;
				case 40:
					if (!(this.dataObject is SCPKG_CMD_RAREEXCHANGE_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_RAREEXCHANGE_RSP)ProtocolObjectPool.Get(SCPKG_CMD_RAREEXCHANGE_RSP.CLASS_ID);
					}
					return;
				case 41:
					if (!(this.dataObject is SCPKG_NTF_SYMBOLPAGESYN))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_NTF_SYMBOLPAGESYN)ProtocolObjectPool.Get(SCPKG_NTF_SYMBOLPAGESYN.CLASS_ID);
					}
					return;
				case 45:
					if (!(this.dataObject is SCPKG_NTF_ERRCODE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_NTF_ERRCODE)ProtocolObjectPool.Get(SCPKG_NTF_ERRCODE.CLASS_ID);
					}
					return;
				case 46:
					if (!(this.dataObject is SCPKG_NTF_NEWIEBITSYN))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_NTF_NEWIEBITSYN)ProtocolObjectPool.Get(SCPKG_NTF_NEWIEBITSYN.CLASS_ID);
					}
					return;
				case 47:
					if (!(this.dataObject is SCPKG_NTF_NEWIEALLBITSYN))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_NTF_NEWIEALLBITSYN)ProtocolObjectPool.Get(SCPKG_NTF_NEWIEALLBITSYN.CLASS_ID);
					}
					return;
				case 48:
					if (!(this.dataObject is CSPKG_CMD_CHG_SIGNATURE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_CHG_SIGNATURE)ProtocolObjectPool.Get(CSPKG_CMD_CHG_SIGNATURE.CLASS_ID);
					}
					return;
				case 49:
					if (!(this.dataObject is SCPKG_CMD_CHG_SIGNATURE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_CHG_SIGNATURE)ProtocolObjectPool.Get(SCPKG_CMD_CHG_SIGNATURE.CLASS_ID);
					}
					return;
				case 55:
					if (!(this.dataObject is CSPKG_CMD_LIST_FRIEND))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_LIST_FRIEND)ProtocolObjectPool.Get(CSPKG_CMD_LIST_FRIEND.CLASS_ID);
					}
					return;
				case 56:
					if (!(this.dataObject is SCPKG_CMD_LIST_FRIEND))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_LIST_FRIEND)ProtocolObjectPool.Get(SCPKG_CMD_LIST_FRIEND.CLASS_ID);
					}
					return;
				case 57:
					if (!(this.dataObject is CSPKG_CMD_LIST_FRIENDREQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_LIST_FRIENDREQ)ProtocolObjectPool.Get(CSPKG_CMD_LIST_FRIENDREQ.CLASS_ID);
					}
					return;
				case 58:
					if (!(this.dataObject is SCPKG_CMD_LIST_FRIENDREQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_LIST_FRIENDREQ)ProtocolObjectPool.Get(SCPKG_CMD_LIST_FRIENDREQ.CLASS_ID);
					}
					return;
				case 59:
					if (!(this.dataObject is CSPKG_CMD_SEARCH_PLAYER))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_SEARCH_PLAYER)ProtocolObjectPool.Get(CSPKG_CMD_SEARCH_PLAYER.CLASS_ID);
					}
					return;
				case 60:
					if (!(this.dataObject is SCPKG_CMD_SEARCH_PLAYER))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_SEARCH_PLAYER)ProtocolObjectPool.Get(SCPKG_CMD_SEARCH_PLAYER.CLASS_ID);
					}
					return;
				case 61:
					if (!(this.dataObject is CSPKG_CMD_ADD_FRIEND))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_ADD_FRIEND)ProtocolObjectPool.Get(CSPKG_CMD_ADD_FRIEND.CLASS_ID);
					}
					return;
				case 62:
					if (!(this.dataObject is SCPKG_CMD_ADD_FRIEND))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_ADD_FRIEND)ProtocolObjectPool.Get(SCPKG_CMD_ADD_FRIEND.CLASS_ID);
					}
					return;
				case 63:
					if (!(this.dataObject is CSPKG_CMD_DEL_FRIEND))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_DEL_FRIEND)ProtocolObjectPool.Get(CSPKG_CMD_DEL_FRIEND.CLASS_ID);
					}
					return;
				case 64:
					if (!(this.dataObject is SCPKG_CMD_DEL_FRIEND))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_DEL_FRIEND)ProtocolObjectPool.Get(SCPKG_CMD_DEL_FRIEND.CLASS_ID);
					}
					return;
				case 65:
					if (!(this.dataObject is CSPKG_CMD_ADD_FRIEND_CONFIRM))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_ADD_FRIEND_CONFIRM)ProtocolObjectPool.Get(CSPKG_CMD_ADD_FRIEND_CONFIRM.CLASS_ID);
					}
					return;
				case 66:
					if (!(this.dataObject is SCPKG_CMD_ADD_FRIEND_CONFIRM))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_ADD_FRIEND_CONFIRM)ProtocolObjectPool.Get(SCPKG_CMD_ADD_FRIEND_CONFIRM.CLASS_ID);
					}
					return;
				case 67:
					if (!(this.dataObject is CSPKG_CMD_ADD_FRIEND_DENY))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_ADD_FRIEND_DENY)ProtocolObjectPool.Get(CSPKG_CMD_ADD_FRIEND_DENY.CLASS_ID);
					}
					return;
				case 68:
					if (!(this.dataObject is SCPKG_CMD_ADD_FRIEND_DENY))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_ADD_FRIEND_DENY)ProtocolObjectPool.Get(SCPKG_CMD_ADD_FRIEND_DENY.CLASS_ID);
					}
					return;
				case 69:
					if (!(this.dataObject is CSPKG_CMD_INVITE_GAME))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_INVITE_GAME)ProtocolObjectPool.Get(CSPKG_CMD_INVITE_GAME.CLASS_ID);
					}
					return;
				case 70:
					if (!(this.dataObject is SCPKG_CMD_INVITE_GAME))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_INVITE_GAME)ProtocolObjectPool.Get(SCPKG_CMD_INVITE_GAME.CLASS_ID);
					}
					return;
				}
			}
			if (this.dataObject != null)
			{
				this.dataObject.Release();
				this.dataObject = null;
			}
		}

		private void select_1216_1329(long selector)
		{
			if (selector >= 1216L && selector <= 1329L)
			{
				switch ((int)(selector - 1216L))
				{
				case 0:
					if (!(this.dataObject is CSPKG_CMD_INVITE_RECEIVE_ACHIEVE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_INVITE_RECEIVE_ACHIEVE)ProtocolObjectPool.Get(CSPKG_CMD_INVITE_RECEIVE_ACHIEVE.CLASS_ID);
					}
					return;
				case 1:
					if (!(this.dataObject is SCPKG_CMD_INVITE_RECEIVE_ACHIEVE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_INVITE_RECEIVE_ACHIEVE)ProtocolObjectPool.Get(SCPKG_CMD_INVITE_RECEIVE_ACHIEVE.CLASS_ID);
					}
					return;
				case 2:
					if (!(this.dataObject is CSPKG_CMD_DONATE_FRIEND_POINT))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_DONATE_FRIEND_POINT)ProtocolObjectPool.Get(CSPKG_CMD_DONATE_FRIEND_POINT.CLASS_ID);
					}
					return;
				case 3:
					if (!(this.dataObject is SCPKG_CMD_DONATE_FRIEND_POINT))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_DONATE_FRIEND_POINT)ProtocolObjectPool.Get(SCPKG_CMD_DONATE_FRIEND_POINT.CLASS_ID);
					}
					return;
				case 6:
					if (!(this.dataObject is CSPKG_CMD_DONATE_FRIEND_POINT_ALL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_DONATE_FRIEND_POINT_ALL)ProtocolObjectPool.Get(CSPKG_CMD_DONATE_FRIEND_POINT_ALL.CLASS_ID);
					}
					return;
				case 7:
					if (!(this.dataObject is SCPKG_CMD_DONATE_FRIEND_POINT_ALL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_DONATE_FRIEND_POINT_ALL)ProtocolObjectPool.Get(SCPKG_CMD_DONATE_FRIEND_POINT_ALL.CLASS_ID);
					}
					return;
				case 8:
					if (!(this.dataObject is SCPKG_CMD_NTF_CHG_INTIMACY))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_NTF_CHG_INTIMACY)ProtocolObjectPool.Get(SCPKG_CMD_NTF_CHG_INTIMACY.CLASS_ID);
					}
					return;
				case 9:
					if (!(this.dataObject is CSPKG_CMD_NOASKFORFLAG_CHG))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_NOASKFORFLAG_CHG)ProtocolObjectPool.Get(CSPKG_CMD_NOASKFORFLAG_CHG.CLASS_ID);
					}
					return;
				case 10:
					if (!(this.dataObject is SCPKG_CMD_NOASKFORFLAG_CHG))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_NOASKFORFLAG_CHG)ProtocolObjectPool.Get(SCPKG_CMD_NOASKFORFLAG_CHG.CLASS_ID);
					}
					return;
				case 11:
					if (!(this.dataObject is SCPKG_NTF_FRIEND_NOASKFOR_FLAGCHG))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_NTF_FRIEND_NOASKFOR_FLAGCHG)ProtocolObjectPool.Get(SCPKG_NTF_FRIEND_NOASKFOR_FLAGCHG.CLASS_ID);
					}
					return;
				case 12:
					if (!(this.dataObject is CSPKG_CMD_GET_INVITE_INFO))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_GET_INVITE_INFO)ProtocolObjectPool.Get(CSPKG_CMD_GET_INVITE_INFO.CLASS_ID);
					}
					return;
				case 13:
					if (!(this.dataObject is SCPKG_CMD_GET_INVITE_INFO))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_GET_INVITE_INFO)ProtocolObjectPool.Get(SCPKG_CMD_GET_INVITE_INFO.CLASS_ID);
					}
					return;
				case 14:
					if (!(this.dataObject is SCPKG_CMD_NTF_FRIEND_REQUEST))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_NTF_FRIEND_REQUEST)ProtocolObjectPool.Get(SCPKG_CMD_NTF_FRIEND_REQUEST.CLASS_ID);
					}
					return;
				case 15:
					if (!(this.dataObject is SCPKG_CMD_NTF_FRIEND_ADD))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_NTF_FRIEND_ADD)ProtocolObjectPool.Get(SCPKG_CMD_NTF_FRIEND_ADD.CLASS_ID);
					}
					return;
				case 16:
					if (!(this.dataObject is SCPKG_CMD_NTF_FRIEND_DEL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_NTF_FRIEND_DEL)ProtocolObjectPool.Get(SCPKG_CMD_NTF_FRIEND_DEL.CLASS_ID);
					}
					return;
				case 17:
					if (!(this.dataObject is SCPKG_CMD_NTF_FRIEND_LOGIN_STATUS))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_NTF_FRIEND_LOGIN_STATUS)ProtocolObjectPool.Get(SCPKG_CMD_NTF_FRIEND_LOGIN_STATUS.CLASS_ID);
					}
					return;
				case 18:
					if (!(this.dataObject is CSPKG_CMD_LIST_FREC))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_LIST_FREC)ProtocolObjectPool.Get(CSPKG_CMD_LIST_FREC.CLASS_ID);
					}
					return;
				case 19:
					if (!(this.dataObject is SCPKG_CMD_LIST_FREC))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_LIST_FREC)ProtocolObjectPool.Get(SCPKG_CMD_LIST_FREC.CLASS_ID);
					}
					return;
				case 20:
					if (!(this.dataObject is CSPKG_CMD_RECALL_FRIEND))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_RECALL_FRIEND)ProtocolObjectPool.Get(CSPKG_CMD_RECALL_FRIEND.CLASS_ID);
					}
					return;
				case 21:
					if (!(this.dataObject is SCPKG_CMD_RECALL_FRIEND))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_RECALL_FRIEND)ProtocolObjectPool.Get(SCPKG_CMD_RECALL_FRIEND.CLASS_ID);
					}
					return;
				case 22:
					if (!(this.dataObject is SCPKG_CMD_NTF_RECALL_FRIEND))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_NTF_RECALL_FRIEND)ProtocolObjectPool.Get(SCPKG_CMD_NTF_RECALL_FRIEND.CLASS_ID);
					}
					return;
				case 23:
					if (!(this.dataObject is SCPKG_CMD_BLACKLIST))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_BLACKLIST)ProtocolObjectPool.Get(SCPKG_CMD_BLACKLIST.CLASS_ID);
					}
					return;
				case 24:
					if (!(this.dataObject is CSPKG_OPER_HERO_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_OPER_HERO_REQ)ProtocolObjectPool.Get(CSPKG_OPER_HERO_REQ.CLASS_ID);
					}
					return;
				case 25:
					if (!(this.dataObject is SCPKG_OPER_HERO_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_OPER_HERO_NTF)ProtocolObjectPool.Get(SCPKG_OPER_HERO_NTF.CLASS_ID);
					}
					return;
				case 26:
					if (!(this.dataObject is CSPKG_CONFIRM_HERO))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CONFIRM_HERO)ProtocolObjectPool.Get(CSPKG_CONFIRM_HERO.CLASS_ID);
					}
					return;
				case 27:
					if (!(this.dataObject is SCPKG_CONFIRM_HERO_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CONFIRM_HERO_NTF)ProtocolObjectPool.Get(SCPKG_CONFIRM_HERO_NTF.CLASS_ID);
					}
					return;
				case 28:
					if (!(this.dataObject is SCPKG_DEFAULT_HERO_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_DEFAULT_HERO_NTF)ProtocolObjectPool.Get(SCPKG_DEFAULT_HERO_NTF.CLASS_ID);
					}
					return;
				case 29:
					if (!(this.dataObject is CSPKG_HERO_CANCEL_CONFIRM))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_HERO_CANCEL_CONFIRM)ProtocolObjectPool.Get(CSPKG_HERO_CANCEL_CONFIRM.CLASS_ID);
					}
					return;
				case 30:
					if (!(this.dataObject is SCPKG_HERO_CANCEL_CONFIRM_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_HERO_CANCEL_CONFIRM_NTF)ProtocolObjectPool.Get(SCPKG_HERO_CANCEL_CONFIRM_NTF.CLASS_ID);
					}
					return;
				case 31:
					if (!(this.dataObject is CSPKG_SHOW_HERO_WIN_RATIO))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_SHOW_HERO_WIN_RATIO)ProtocolObjectPool.Get(CSPKG_SHOW_HERO_WIN_RATIO.CLASS_ID);
					}
					return;
				case 32:
					if (!(this.dataObject is SCPKG_SHOW_HERO_WIN_RATIO_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_SHOW_HERO_WIN_RATIO_NTF)ProtocolObjectPool.Get(SCPKG_SHOW_HERO_WIN_RATIO_NTF.CLASS_ID);
					}
					return;
				case 44:
					if (!(this.dataObject is CSPKG_RELAYSVRPING))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_RELAYSVRPING)ProtocolObjectPool.Get(CSPKG_RELAYSVRPING.CLASS_ID);
					}
					return;
				case 45:
					if (!(this.dataObject is CSPKG_GAMESVRPING))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_GAMESVRPING)ProtocolObjectPool.Get(CSPKG_GAMESVRPING.CLASS_ID);
					}
					return;
				case 46:
					if (!(this.dataObject is CSPKG_CLRCDLIMIT_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CLRCDLIMIT_REQ)ProtocolObjectPool.Get(CSPKG_CLRCDLIMIT_REQ.CLASS_ID);
					}
					return;
				case 47:
					if (!(this.dataObject is SCPKG_CLRCDLIMIT_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CLRCDLIMIT_RSP)ProtocolObjectPool.Get(SCPKG_CLRCDLIMIT_RSP.CLASS_ID);
					}
					return;
				case 64:
					if (!(this.dataObject is CSPKG_RELAYHASHCHECK))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_RELAYHASHCHECK)ProtocolObjectPool.Get(CSPKG_RELAYHASHCHECK.CLASS_ID);
					}
					return;
				case 65:
					if (!(this.dataObject is CSPKG_NEXTFIRSTWINSEC_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_NEXTFIRSTWINSEC_NTF)ProtocolObjectPool.Get(CSPKG_NEXTFIRSTWINSEC_NTF.CLASS_ID);
					}
					return;
				case 66:
					if (!(this.dataObject is CSPKG_COINGETPATH_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_COINGETPATH_REQ)ProtocolObjectPool.Get(CSPKG_COINGETPATH_REQ.CLASS_ID);
					}
					return;
				case 67:
					if (!(this.dataObject is SCPKG_COINGETPATH_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_COINGETPATH_RSP)ProtocolObjectPool.Get(SCPKG_COINGETPATH_RSP.CLASS_ID);
					}
					return;
				case 68:
					if (!(this.dataObject is SCPKG_RELAYHASHCHECK))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_RELAYHASHCHECK)ProtocolObjectPool.Get(SCPKG_RELAYHASHCHECK.CLASS_ID);
					}
					return;
				case 84:
					if (!(this.dataObject is CSPKG_CMD_CHAT_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_CHAT_REQ)ProtocolObjectPool.Get(CSPKG_CMD_CHAT_REQ.CLASS_ID);
					}
					return;
				case 85:
					if (!(this.dataObject is CSPKG_CMD_GET_CHAT_MSG_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_GET_CHAT_MSG_REQ)ProtocolObjectPool.Get(CSPKG_CMD_GET_CHAT_MSG_REQ.CLASS_ID);
					}
					return;
				case 86:
					if (!(this.dataObject is SCPKG_CMD_CHAT_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_CHAT_NTF)ProtocolObjectPool.Get(SCPKG_CMD_CHAT_NTF.CLASS_ID);
					}
					return;
				case 87:
					if (!(this.dataObject is CSPKG_CHAT_COMPLAINT_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CHAT_COMPLAINT_REQ)ProtocolObjectPool.Get(CSPKG_CHAT_COMPLAINT_REQ.CLASS_ID);
					}
					return;
				case 88:
					if (!(this.dataObject is SCPKG_CHAT_COMPLAINT_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CHAT_COMPLAINT_RSP)ProtocolObjectPool.Get(SCPKG_CHAT_COMPLAINT_RSP.CLASS_ID);
					}
					return;
				case 89:
					if (!(this.dataObject is CSPKG_CMD_GET_HORNMSG))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_GET_HORNMSG)ProtocolObjectPool.Get(CSPKG_CMD_GET_HORNMSG.CLASS_ID);
					}
					return;
				case 90:
					if (!(this.dataObject is SCPKG_CMD_GET_HORNMSG))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_GET_HORNMSG)ProtocolObjectPool.Get(SCPKG_CMD_GET_HORNMSG.CLASS_ID);
					}
					return;
				case 91:
					if (!(this.dataObject is SCPKG_OFFLINE_CHAT_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_OFFLINE_CHAT_NTF)ProtocolObjectPool.Get(SCPKG_OFFLINE_CHAT_NTF.CLASS_ID);
					}
					return;
				case 92:
					if (!(this.dataObject is CSPKG_CLEAN_OFFLINE_CHAT_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CLEAN_OFFLINE_CHAT_REQ)ProtocolObjectPool.Get(CSPKG_CLEAN_OFFLINE_CHAT_REQ.CLASS_ID);
					}
					return;
				case 93:
					if (!(this.dataObject is SCPKG_LEAVE_SETTLEUI_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_LEAVE_SETTLEUI_NTF)ProtocolObjectPool.Get(SCPKG_LEAVE_SETTLEUI_NTF.CLASS_ID);
					}
					return;
				case 104:
					if (!(this.dataObject is CSPKG_CMD_GAIN_CHEST))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_GAIN_CHEST)ProtocolObjectPool.Get(CSPKG_CMD_GAIN_CHEST.CLASS_ID);
					}
					return;
				case 105:
					if (!(this.dataObject is SCPKG_CMD_GAIN_CHEST))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_GAIN_CHEST)ProtocolObjectPool.Get(SCPKG_CMD_GAIN_CHEST.CLASS_ID);
					}
					return;
				case 106:
					if (!(this.dataObject is CSPKG_CMD_FRIEND_REFUSE_RECALL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_FRIEND_REFUSE_RECALL)ProtocolObjectPool.Get(CSPKG_CMD_FRIEND_REFUSE_RECALL.CLASS_ID);
					}
					return;
				case 107:
					if (!(this.dataObject is SCPKG_CMD_NTF_REFUSE_RECALL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_NTF_REFUSE_RECALL)ProtocolObjectPool.Get(SCPKG_CMD_NTF_REFUSE_RECALL.CLASS_ID);
					}
					return;
				case 108:
					if (!(this.dataObject is SCPKG_CMD_FRIEND_REFUSE_RECALL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_FRIEND_REFUSE_RECALL)ProtocolObjectPool.Get(SCPKG_CMD_FRIEND_REFUSE_RECALL.CLASS_ID);
					}
					return;
				case 109:
					if (!(this.dataObject is CSPKG_CMD_DEFRIEND))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_DEFRIEND)ProtocolObjectPool.Get(CSPKG_CMD_DEFRIEND.CLASS_ID);
					}
					return;
				case 110:
					if (!(this.dataObject is SCPKG_CMD_DEFRIEND))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_DEFRIEND)ProtocolObjectPool.Get(SCPKG_CMD_DEFRIEND.CLASS_ID);
					}
					return;
				case 111:
					if (!(this.dataObject is CSPKG_CMD_CANCEL_DEFRIEND))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_CANCEL_DEFRIEND)ProtocolObjectPool.Get(CSPKG_CMD_CANCEL_DEFRIEND.CLASS_ID);
					}
					return;
				case 112:
					if (!(this.dataObject is SCPKG_CMD_CANCEL_DEFRIEND))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_CANCEL_DEFRIEND)ProtocolObjectPool.Get(SCPKG_CMD_CANCEL_DEFRIEND.CLASS_ID);
					}
					return;
				case 113:
					if (!(this.dataObject is CSPKG_CMD_LBS_REPORT))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_LBS_REPORT)ProtocolObjectPool.Get(CSPKG_CMD_LBS_REPORT.CLASS_ID);
					}
					return;
				}
			}
			if (this.dataObject != null)
			{
				this.dataObject.Release();
				this.dataObject = null;
			}
		}

		private void select_1330_1441(long selector)
		{
			if (selector >= 1330L && selector <= 1441L)
			{
				switch ((int)(selector - 1330L))
				{
				case 0:
					if (!(this.dataObject is CSPKG_CMD_LBS_SEARCH))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_LBS_SEARCH)ProtocolObjectPool.Get(CSPKG_CMD_LBS_SEARCH.CLASS_ID);
					}
					return;
				case 1:
					if (!(this.dataObject is SCPKG_CMD_LBS_SEARCH))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_LBS_SEARCH)ProtocolObjectPool.Get(SCPKG_CMD_LBS_SEARCH.CLASS_ID);
					}
					return;
				case 2:
					if (!(this.dataObject is CSPKG_CMD_LBS_REMOVE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_LBS_REMOVE)ProtocolObjectPool.Get(CSPKG_CMD_LBS_REMOVE.CLASS_ID);
					}
					return;
				case 3:
					if (!(this.dataObject is CSPKG_CMD_LIKE_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_LIKE_REQ)ProtocolObjectPool.Get(CSPKG_CMD_LIKE_REQ.CLASS_ID);
					}
					return;
				case 4:
					if (!(this.dataObject is SCPKG_CMD_NTF_LIKE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_NTF_LIKE)ProtocolObjectPool.Get(SCPKG_CMD_NTF_LIKE.CLASS_ID);
					}
					return;
				case 20:
					if (!(this.dataObject is CSPKG_CMD_LICENSE_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_LICENSE_REQ)ProtocolObjectPool.Get(CSPKG_CMD_LICENSE_REQ.CLASS_ID);
					}
					return;
				case 21:
					if (!(this.dataObject is SCPKG_CMD_LICENSE_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_LICENSE_RSP)ProtocolObjectPool.Get(SCPKG_CMD_LICENSE_RSP.CLASS_ID);
					}
					return;
				case 30:
					if (!(this.dataObject is CSPKG_CMD_INTIMACY_RELATION_REQUEST))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_INTIMACY_RELATION_REQUEST)ProtocolObjectPool.Get(CSPKG_CMD_INTIMACY_RELATION_REQUEST.CLASS_ID);
					}
					return;
				case 31:
					if (!(this.dataObject is SCPKG_CMD_INTIMACY_RELATION_REQUEST))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_INTIMACY_RELATION_REQUEST)ProtocolObjectPool.Get(SCPKG_CMD_INTIMACY_RELATION_REQUEST.CLASS_ID);
					}
					return;
				case 32:
					if (!(this.dataObject is SCPKG_CMD_NTF_INTIMACY_RELATION_REQUEST))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_NTF_INTIMACY_RELATION_REQUEST)ProtocolObjectPool.Get(SCPKG_CMD_NTF_INTIMACY_RELATION_REQUEST.CLASS_ID);
					}
					return;
				case 33:
					if (!(this.dataObject is CSPKG_CMD_CHG_INTIMACY_CONFIRM))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_CHG_INTIMACY_CONFIRM)ProtocolObjectPool.Get(CSPKG_CMD_CHG_INTIMACY_CONFIRM.CLASS_ID);
					}
					return;
				case 34:
					if (!(this.dataObject is SCPKG_CMD_CHG_INTIMACY_CONFIRM))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_CHG_INTIMACY_CONFIRM)ProtocolObjectPool.Get(SCPKG_CMD_CHG_INTIMACY_CONFIRM.CLASS_ID);
					}
					return;
				case 35:
					if (!(this.dataObject is SCPKG_CMD_NTF_CHG_INTIMACY_CONFIRM))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_NTF_CHG_INTIMACY_CONFIRM)ProtocolObjectPool.Get(SCPKG_CMD_NTF_CHG_INTIMACY_CONFIRM.CLASS_ID);
					}
					return;
				case 36:
					if (!(this.dataObject is CSPKG_CMD_CHG_INTIMACY_DENY))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_CHG_INTIMACY_DENY)ProtocolObjectPool.Get(CSPKG_CMD_CHG_INTIMACY_DENY.CLASS_ID);
					}
					return;
				case 37:
					if (!(this.dataObject is SCPKG_CMD_CHG_INTIMACY_DENY))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_CHG_INTIMACY_DENY)ProtocolObjectPool.Get(SCPKG_CMD_CHG_INTIMACY_DENY.CLASS_ID);
					}
					return;
				case 38:
					if (!(this.dataObject is SCPKG_CMD_NTF_CHG_INTIMACY_DENY))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_NTF_CHG_INTIMACY_DENY)ProtocolObjectPool.Get(SCPKG_CMD_NTF_CHG_INTIMACY_DENY.CLASS_ID);
					}
					return;
				case 39:
					if (!(this.dataObject is CSPKG_CMD_CHG_INTIMACYPRIORITY))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_CHG_INTIMACYPRIORITY)ProtocolObjectPool.Get(CSPKG_CMD_CHG_INTIMACYPRIORITY.CLASS_ID);
					}
					return;
				case 40:
					if (!(this.dataObject is CSPKG_RECRUITMENT_REWARD_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_RECRUITMENT_REWARD_REQ)ProtocolObjectPool.Get(CSPKG_RECRUITMENT_REWARD_REQ.CLASS_ID);
					}
					return;
				case 41:
					if (!(this.dataObject is SCPKG_RECRUITMENT_REWARD_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_RECRUITMENT_REWARD_RSP)ProtocolObjectPool.Get(SCPKG_RECRUITMENT_REWARD_RSP.CLASS_ID);
					}
					return;
				case 42:
					if (!(this.dataObject is SCPKG_RECRUITMENT_REWARD_ERR_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_RECRUITMENT_REWARD_ERR_NTF)ProtocolObjectPool.Get(SCPKG_RECRUITMENT_REWARD_ERR_NTF.CLASS_ID);
					}
					return;
				case 43:
					if (!(this.dataObject is SCPKG_RECRUITMENT_ERR_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_RECRUITMENT_ERR_NTF)ProtocolObjectPool.Get(SCPKG_RECRUITMENT_ERR_NTF.CLASS_ID);
					}
					return;
				case 44:
					if (!(this.dataObject is SCPKG_CHG_RECRUITMENT_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CHG_RECRUITMENT_NTF)ProtocolObjectPool.Get(SCPKG_CHG_RECRUITMENT_NTF.CLASS_ID);
					}
					return;
				case 49:
					if (!(this.dataObject is CSPKG_CMD_ASKFORREQ_GET))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_ASKFORREQ_GET)ProtocolObjectPool.Get(CSPKG_CMD_ASKFORREQ_GET.CLASS_ID);
					}
					return;
				case 50:
					if (!(this.dataObject is SCPKG_CMD_ASKFORREQ_GET))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_ASKFORREQ_GET)ProtocolObjectPool.Get(SCPKG_CMD_ASKFORREQ_GET.CLASS_ID);
					}
					return;
				case 51:
					if (!(this.dataObject is SCPKG_CMD_ASKFORREQ_GETFAIL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_ASKFORREQ_GETFAIL)ProtocolObjectPool.Get(SCPKG_CMD_ASKFORREQ_GETFAIL.CLASS_ID);
					}
					return;
				case 52:
					if (!(this.dataObject is CSPKG_CMD_ASKFORREQ_DEL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_ASKFORREQ_DEL)ProtocolObjectPool.Get(CSPKG_CMD_ASKFORREQ_DEL.CLASS_ID);
					}
					return;
				case 53:
					if (!(this.dataObject is SCPKG_CMD_ASKFORREQ_DEL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_ASKFORREQ_DEL)ProtocolObjectPool.Get(SCPKG_CMD_ASKFORREQ_DEL.CLASS_ID);
					}
					return;
				case 54:
					if (!(this.dataObject is CSPKG_CMD_ASKFORREQ_SEND))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_ASKFORREQ_SEND)ProtocolObjectPool.Get(CSPKG_CMD_ASKFORREQ_SEND.CLASS_ID);
					}
					return;
				case 55:
					if (!(this.dataObject is SCPKG_CMD_ASKFORREQ_SEND))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_ASKFORREQ_SEND)ProtocolObjectPool.Get(SCPKG_CMD_ASKFORREQ_SEND.CLASS_ID);
					}
					return;
				case 56:
					if (!(this.dataObject is CSPKG_CMD_ASKFORREQ_READ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_ASKFORREQ_READ)ProtocolObjectPool.Get(CSPKG_CMD_ASKFORREQ_READ.CLASS_ID);
					}
					return;
				case 57:
					if (!(this.dataObject is SCPKG_CMD_ASKFORREQ_READ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_ASKFORREQ_READ)ProtocolObjectPool.Get(SCPKG_CMD_ASKFORREQ_READ.CLASS_ID);
					}
					return;
				case 58:
					if (!(this.dataObject is CSPKG_CMD_ASKFORREQ_REFUSE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_ASKFORREQ_REFUSE)ProtocolObjectPool.Get(CSPKG_CMD_ASKFORREQ_REFUSE.CLASS_ID);
					}
					return;
				case 59:
					if (!(this.dataObject is SCPKG_CMD_ASKFORREQ_REFUSE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_ASKFORREQ_REFUSE)ProtocolObjectPool.Get(SCPKG_CMD_ASKFORREQ_REFUSE.CLASS_ID);
					}
					return;
				case 60:
					if (!(this.dataObject is CSPKG_CMD_ASKFORREQ_CONFIRM))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CMD_ASKFORREQ_CONFIRM)ProtocolObjectPool.Get(CSPKG_CMD_ASKFORREQ_CONFIRM.CLASS_ID);
					}
					return;
				case 61:
					if (!(this.dataObject is SCPKG_CMD_ASKFORREQ_CONFIRM))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_ASKFORREQ_CONFIRM)ProtocolObjectPool.Get(SCPKG_CMD_ASKFORREQ_CONFIRM.CLASS_ID);
					}
					return;
				case 70:
					if (!(this.dataObject is CSPKG_MAILOPT_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_MAILOPT_REQ)ProtocolObjectPool.Get(CSPKG_MAILOPT_REQ.CLASS_ID);
					}
					return;
				case 71:
					if (!(this.dataObject is SCPKG_MAILOPT_RES))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_MAILOPT_RES)ProtocolObjectPool.Get(SCPKG_MAILOPT_RES.CLASS_ID);
					}
					return;
				case 72:
					if (!(this.dataObject is CSPKG_FUNCUNLOCK_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_FUNCUNLOCK_REQ)ProtocolObjectPool.Get(CSPKG_FUNCUNLOCK_REQ.CLASS_ID);
					}
					return;
				case 73:
					if (!(this.dataObject is SCPKG_ACNTDETAILINFO_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_ACNTDETAILINFO_RSP)ProtocolObjectPool.Get(SCPKG_ACNTDETAILINFO_RSP.CLASS_ID);
					}
					return;
				case 74:
					if (!(this.dataObject is SCPKG_ACNT_HEAD_URL_CHG_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_ACNT_HEAD_URL_CHG_NTF)ProtocolObjectPool.Get(SCPKG_ACNT_HEAD_URL_CHG_NTF.CLASS_ID);
					}
					return;
				case 75:
					if (!(this.dataObject is SCPKG_ACNTSELFMSGINFO))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_ACNTSELFMSGINFO)ProtocolObjectPool.Get(SCPKG_ACNTSELFMSGINFO.CLASS_ID);
					}
					return;
				case 76:
					if (!(this.dataObject is SCPKG_AKALISHOP_INFO))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_AKALISHOP_INFO)ProtocolObjectPool.Get(SCPKG_AKALISHOP_INFO.CLASS_ID);
					}
					return;
				case 77:
					if (!(this.dataObject is CSPKG_AKALISHOP_BUYREQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_AKALISHOP_BUYREQ)ProtocolObjectPool.Get(CSPKG_AKALISHOP_BUYREQ.CLASS_ID);
					}
					return;
				case 78:
					if (!(this.dataObject is SCPKG_AKALISHOP_BUYRSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_AKALISHOP_BUYRSP)ProtocolObjectPool.Get(SCPKG_AKALISHOP_BUYRSP.CLASS_ID);
					}
					return;
				case 79:
					if (!(this.dataObject is CSPKG_AKALISHOP_FLAGREQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_AKALISHOP_FLAGREQ)ProtocolObjectPool.Get(CSPKG_AKALISHOP_FLAGREQ.CLASS_ID);
					}
					return;
				case 80:
					if (!(this.dataObject is SCPKG_AKALISHOP_FLAGRSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_AKALISHOP_FLAGRSP)ProtocolObjectPool.Get(SCPKG_AKALISHOP_FLAGRSP.CLASS_ID);
					}
					return;
				case 84:
					if (!(this.dataObject is SCPKG_HONORINFOCHG_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_HONORINFOCHG_RSP)ProtocolObjectPool.Get(SCPKG_HONORINFOCHG_RSP.CLASS_ID);
					}
					return;
				case 85:
					if (!(this.dataObject is SCPKG_HONORINFO_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_HONORINFO_RSP)ProtocolObjectPool.Get(SCPKG_HONORINFO_RSP.CLASS_ID);
					}
					return;
				case 86:
					if (!(this.dataObject is CSPKG_USEHONOR_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_USEHONOR_REQ)ProtocolObjectPool.Get(CSPKG_USEHONOR_REQ.CLASS_ID);
					}
					return;
				case 87:
					if (!(this.dataObject is SCPKG_USEHONOR_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_USEHONOR_RSP)ProtocolObjectPool.Get(SCPKG_USEHONOR_RSP.CLASS_ID);
					}
					return;
				case 90:
					if (!(this.dataObject is CSPKG_NOTICENEW_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_NOTICENEW_REQ)ProtocolObjectPool.Get(CSPKG_NOTICENEW_REQ.CLASS_ID);
					}
					return;
				case 91:
					if (!(this.dataObject is SCPKG_NOTICENEW_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_NOTICENEW_RSP)ProtocolObjectPool.Get(SCPKG_NOTICENEW_RSP.CLASS_ID);
					}
					return;
				case 92:
					if (!(this.dataObject is CSPKG_NOTICELIST_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_NOTICELIST_REQ)ProtocolObjectPool.Get(CSPKG_NOTICELIST_REQ.CLASS_ID);
					}
					return;
				case 93:
					if (!(this.dataObject is SCPKG_NOTICELIST_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_NOTICELIST_RSP)ProtocolObjectPool.Get(SCPKG_NOTICELIST_RSP.CLASS_ID);
					}
					return;
				case 94:
					if (!(this.dataObject is CSPKG_NOTICEINFO_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_NOTICEINFO_REQ)ProtocolObjectPool.Get(CSPKG_NOTICEINFO_REQ.CLASS_ID);
					}
					return;
				case 95:
					if (!(this.dataObject is SCPKG_NOTICEINFO_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_NOTICEINFO_RSP)ProtocolObjectPool.Get(SCPKG_NOTICEINFO_RSP.CLASS_ID);
					}
					return;
				case 100:
					if (!(this.dataObject is CSPKG_SEND_GUILD_MAIL_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_SEND_GUILD_MAIL_REQ)ProtocolObjectPool.Get(CSPKG_SEND_GUILD_MAIL_REQ.CLASS_ID);
					}
					return;
				case 101:
					if (!(this.dataObject is SCPKG_SEND_GUILD_MAIL_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_SEND_GUILD_MAIL_RSP)ProtocolObjectPool.Get(SCPKG_SEND_GUILD_MAIL_RSP.CLASS_ID);
					}
					return;
				case 110:
					if (!(this.dataObject is CSPKG_FIGHTHISTORY_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_FIGHTHISTORY_REQ)ProtocolObjectPool.Get(CSPKG_FIGHTHISTORY_REQ.CLASS_ID);
					}
					return;
				case 111:
					if (!(this.dataObject is CSPKG_FIGHTHISTORYLIST_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_FIGHTHISTORYLIST_REQ)ProtocolObjectPool.Get(CSPKG_FIGHTHISTORYLIST_REQ.CLASS_ID);
					}
					return;
				}
			}
			if (this.dataObject != null)
			{
				this.dataObject.Release();
				this.dataObject = null;
			}
		}

		private void select_1442_1906(long selector)
		{
			if (selector >= 1800L && selector <= 1833L)
			{
				switch ((int)(selector - 1800L))
				{
				case 0:
					if (!(this.dataObject is CSPKG_ACHIEVEHERO_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_ACHIEVEHERO_REQ)ProtocolObjectPool.Get(CSPKG_ACHIEVEHERO_REQ.CLASS_ID);
					}
					return;
				case 1:
					if (!(this.dataObject is SCPKG_ACHIEVEHERO_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_ACHIEVEHERO_RSP)ProtocolObjectPool.Get(SCPKG_ACHIEVEHERO_RSP.CLASS_ID);
					}
					return;
				case 2:
					if (!(this.dataObject is SCPKG_ACNTHEROINFO_NTY))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_ACNTHEROINFO_NTY)ProtocolObjectPool.Get(SCPKG_ACNTHEROINFO_NTY.CLASS_ID);
					}
					return;
				case 3:
					if (!(this.dataObject is SCPKG_CMD_HEROEXP_ADD))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_HEROEXP_ADD)ProtocolObjectPool.Get(SCPKG_CMD_HEROEXP_ADD.CLASS_ID);
					}
					return;
				case 4:
					if (!(this.dataObject is SCPKG_ADDHERO_NTY))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_ADDHERO_NTY)ProtocolObjectPool.Get(SCPKG_ADDHERO_NTY.CLASS_ID);
					}
					return;
				case 5:
					if (!(this.dataObject is CSPKG_BATTLELIST_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_BATTLELIST_REQ)ProtocolObjectPool.Get(CSPKG_BATTLELIST_REQ.CLASS_ID);
					}
					return;
				case 6:
					if (!(this.dataObject is SCPKG_BATTLELIST_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_BATTLELIST_RSP)ProtocolObjectPool.Get(SCPKG_BATTLELIST_RSP.CLASS_ID);
					}
					return;
				case 7:
					if (!(this.dataObject is SCPKG_BATTLELIST_NTY))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_BATTLELIST_NTY)ProtocolObjectPool.Get(SCPKG_BATTLELIST_NTY.CLASS_ID);
					}
					return;
				case 8:
					if (!(this.dataObject is CSPKG_UPGRADESTAR_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_UPGRADESTAR_REQ)ProtocolObjectPool.Get(CSPKG_UPGRADESTAR_REQ.CLASS_ID);
					}
					return;
				case 9:
					if (!(this.dataObject is SCPKG_UPGRADESTAR_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_UPGRADESTAR_RSP)ProtocolObjectPool.Get(SCPKG_UPGRADESTAR_RSP.CLASS_ID);
					}
					return;
				case 10:
					if (!(this.dataObject is SCPKG_NTF_HERO_INFO_UPD))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_NTF_HERO_INFO_UPD)ProtocolObjectPool.Get(SCPKG_NTF_HERO_INFO_UPD.CLASS_ID);
					}
					return;
				case 11:
					if (!(this.dataObject is CSPKG_SKILLUPDATE_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_SKILLUPDATE_REQ)ProtocolObjectPool.Get(CSPKG_SKILLUPDATE_REQ.CLASS_ID);
					}
					return;
				case 12:
					if (!(this.dataObject is SCPKG_SKILLUPDATE_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_SKILLUPDATE_RSP)ProtocolObjectPool.Get(SCPKG_SKILLUPDATE_RSP.CLASS_ID);
					}
					return;
				case 13:
					if (!(this.dataObject is SCPKG_ACHIEVEHERO_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_ACHIEVEHERO_RSP)ProtocolObjectPool.Get(SCPKG_ACHIEVEHERO_RSP.CLASS_ID);
					}
					return;
				case 14:
					if (!(this.dataObject is CSPKG_FREEHERO_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_FREEHERO_REQ)ProtocolObjectPool.Get(CSPKG_FREEHERO_REQ.CLASS_ID);
					}
					return;
				case 15:
					if (!(this.dataObject is SCPKG_FREEHERO_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_FREEHERO_RSP)ProtocolObjectPool.Get(SCPKG_FREEHERO_RSP.CLASS_ID);
					}
					return;
				case 16:
					if (!(this.dataObject is SCPKG_GMUNLOGCKHEROPVP_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_GMUNLOGCKHEROPVP_RSP)ProtocolObjectPool.Get(SCPKG_GMUNLOGCKHEROPVP_RSP.CLASS_ID);
					}
					return;
				case 17:
					if (!(this.dataObject is CSPKG_BUYHERO_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_BUYHERO_REQ)ProtocolObjectPool.Get(CSPKG_BUYHERO_REQ.CLASS_ID);
					}
					return;
				case 18:
					if (!(this.dataObject is SCPKG_BUYHERO_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_BUYHERO_RSP)ProtocolObjectPool.Get(SCPKG_BUYHERO_RSP.CLASS_ID);
					}
					return;
				case 19:
					if (!(this.dataObject is CSPKG_BUYHEROSKIN_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_BUYHEROSKIN_REQ)ProtocolObjectPool.Get(CSPKG_BUYHEROSKIN_REQ.CLASS_ID);
					}
					return;
				case 20:
					if (!(this.dataObject is SCPKG_BUYHEROSKIN_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_BUYHEROSKIN_RSP)ProtocolObjectPool.Get(SCPKG_BUYHEROSKIN_RSP.CLASS_ID);
					}
					return;
				case 21:
					if (!(this.dataObject is CSPKG_WEARHEROSKIN_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_WEARHEROSKIN_REQ)ProtocolObjectPool.Get(CSPKG_WEARHEROSKIN_REQ.CLASS_ID);
					}
					return;
				case 22:
					if (!(this.dataObject is SCPKG_WEARHEROSKIN_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_WEARHEROSKIN_RSP)ProtocolObjectPool.Get(SCPKG_WEARHEROSKIN_RSP.CLASS_ID);
					}
					return;
				case 23:
					if (!(this.dataObject is SCPKG_HEROSKIN_ADD))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_HEROSKIN_ADD)ProtocolObjectPool.Get(SCPKG_HEROSKIN_ADD.CLASS_ID);
					}
					return;
				case 24:
					if (!(this.dataObject is CSPKG_UPHEROLVL_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_UPHEROLVL_REQ)ProtocolObjectPool.Get(CSPKG_UPHEROLVL_REQ.CLASS_ID);
					}
					return;
				case 25:
					if (!(this.dataObject is SCPKG_UPHEROLVL_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_UPHEROLVL_RSP)ProtocolObjectPool.Get(SCPKG_UPHEROLVL_RSP.CLASS_ID);
					}
					return;
				case 26:
					if (!(this.dataObject is SCPKG_LIMITSKIN_ADD))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_LIMITSKIN_ADD)ProtocolObjectPool.Get(SCPKG_LIMITSKIN_ADD.CLASS_ID);
					}
					return;
				case 27:
					if (!(this.dataObject is SCPKG_LIMITSKIN_DEL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_LIMITSKIN_DEL)ProtocolObjectPool.Get(SCPKG_LIMITSKIN_DEL.CLASS_ID);
					}
					return;
				case 28:
					if (!(this.dataObject is SCPKG_USEEXPCARD_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_USEEXPCARD_NTF)ProtocolObjectPool.Get(SCPKG_USEEXPCARD_NTF.CLASS_ID);
					}
					return;
				case 29:
					if (!(this.dataObject is SCPKG_GM_ADDALLSKIN_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_GM_ADDALLSKIN_RSP)ProtocolObjectPool.Get(SCPKG_GM_ADDALLSKIN_RSP.CLASS_ID);
					}
					return;
				case 30:
					if (!(this.dataObject is CSPKG_PRESENTHERO_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_PRESENTHERO_REQ)ProtocolObjectPool.Get(CSPKG_PRESENTHERO_REQ.CLASS_ID);
					}
					return;
				case 31:
					if (!(this.dataObject is SCPKG_PRESENTHERO_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_PRESENTHERO_RSP)ProtocolObjectPool.Get(SCPKG_PRESENTHERO_RSP.CLASS_ID);
					}
					return;
				case 32:
					if (!(this.dataObject is CSPKG_PRESENTSKIN_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_PRESENTSKIN_REQ)ProtocolObjectPool.Get(CSPKG_PRESENTSKIN_REQ.CLASS_ID);
					}
					return;
				case 33:
					if (!(this.dataObject is SCPKG_PERSENTSKIN_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_PERSENTSKIN_RSP)ProtocolObjectPool.Get(SCPKG_PERSENTSKIN_RSP.CLASS_ID);
					}
					return;
				}
			}
			if (selector >= 1500L && selector <= 1508L)
			{
				switch ((int)(selector - 1500L))
				{
				case 0:
					if (!(this.dataObject is CSPKG_USUALTASK_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_USUALTASK_REQ)ProtocolObjectPool.Get(CSPKG_USUALTASK_REQ.CLASS_ID);
					}
					return;
				case 1:
					if (!(this.dataObject is SCPKG_USUALTASK_RES))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_USUALTASK_RES)ProtocolObjectPool.Get(SCPKG_USUALTASK_RES.CLASS_ID);
					}
					return;
				case 2:
					if (!(this.dataObject is CSPKG_TASKSUBMIT_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_TASKSUBMIT_REQ)ProtocolObjectPool.Get(CSPKG_TASKSUBMIT_REQ.CLASS_ID);
					}
					return;
				case 3:
					if (!(this.dataObject is SCPKG_TASKSUBMIT_RES))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_TASKSUBMIT_RES)ProtocolObjectPool.Get(SCPKG_TASKSUBMIT_RES.CLASS_ID);
					}
					return;
				case 4:
					if (!(this.dataObject is SCPKG_TASKUPD_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_TASKUPD_NTF)ProtocolObjectPool.Get(SCPKG_TASKUPD_NTF.CLASS_ID);
					}
					return;
				case 5:
					if (!(this.dataObject is CSPKG_TASKDONE_CLIENTREPORT))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_TASKDONE_CLIENTREPORT)ProtocolObjectPool.Get(CSPKG_TASKDONE_CLIENTREPORT.CLASS_ID);
					}
					return;
				case 6:
					if (!(this.dataObject is SCPKG_USUALTASKDISCARD_RES))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_USUALTASKDISCARD_RES)ProtocolObjectPool.Get(SCPKG_USUALTASKDISCARD_RES.CLASS_ID);
					}
					return;
				case 7:
					if (!(this.dataObject is SCPKG_NEWTASKGET_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_NEWTASKGET_NTF)ProtocolObjectPool.Get(SCPKG_NEWTASKGET_NTF.CLASS_ID);
					}
					return;
				case 8:
					if (!(this.dataObject is SCPKG_DELTASK_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_DELTASK_NTF)ProtocolObjectPool.Get(SCPKG_DELTASK_NTF.CLASS_ID);
					}
					return;
				}
			}
			if (selector >= 1901L && selector <= 1906L)
			{
				switch ((int)(selector - 1901L))
				{
				case 0:
					if (!(this.dataObject is CSPKG_ACNT_PSWDINFO_OPEN))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_ACNT_PSWDINFO_OPEN)ProtocolObjectPool.Get(CSPKG_ACNT_PSWDINFO_OPEN.CLASS_ID);
					}
					return;
				case 1:
					if (!(this.dataObject is SCPKG_ACNT_PSWDINFO_OPEN))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_ACNT_PSWDINFO_OPEN)ProtocolObjectPool.Get(SCPKG_ACNT_PSWDINFO_OPEN.CLASS_ID);
					}
					return;
				case 2:
					if (!(this.dataObject is CSPKG_ACNT_PSWDINFO_CLOSE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_ACNT_PSWDINFO_CLOSE)ProtocolObjectPool.Get(CSPKG_ACNT_PSWDINFO_CLOSE.CLASS_ID);
					}
					return;
				case 3:
					if (!(this.dataObject is SCPKG_ACNT_PSWDINFO_CLOSE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_ACNT_PSWDINFO_CLOSE)ProtocolObjectPool.Get(SCPKG_ACNT_PSWDINFO_CLOSE.CLASS_ID);
					}
					return;
				case 4:
					if (!(this.dataObject is CSPKG_ACNT_PSWDINFO_CHG))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_ACNT_PSWDINFO_CHG)ProtocolObjectPool.Get(CSPKG_ACNT_PSWDINFO_CHG.CLASS_ID);
					}
					return;
				case 5:
					if (!(this.dataObject is SCPKG_ACNT_PSWDINFO_CHG))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_ACNT_PSWDINFO_CHG)ProtocolObjectPool.Get(SCPKG_ACNT_PSWDINFO_CHG.CLASS_ID);
					}
					return;
				}
			}
			if (selector >= 1600L && selector <= 1603L)
			{
				switch ((int)(selector - 1600L))
				{
				case 0:
					if (!(this.dataObject is SCPKG_NTF_HUOYUEDUINFO))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_NTF_HUOYUEDUINFO)ProtocolObjectPool.Get(SCPKG_NTF_HUOYUEDUINFO.CLASS_ID);
					}
					return;
				case 1:
					if (!(this.dataObject is CSPKG_GETHUOYUEDUREWARD_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_GETHUOYUEDUREWARD_REQ)ProtocolObjectPool.Get(CSPKG_GETHUOYUEDUREWARD_REQ.CLASS_ID);
					}
					return;
				case 2:
					if (!(this.dataObject is SCPKG_GETHUOYUEDUREWARD_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_GETHUOYUEDUREWARD_RSP)ProtocolObjectPool.Get(SCPKG_GETHUOYUEDUREWARD_RSP.CLASS_ID);
					}
					return;
				case 3:
					if (!(this.dataObject is SCPKG_HUOYUEDUREWARDERR_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_HUOYUEDUREWARDERR_NTF)ProtocolObjectPool.Get(SCPKG_HUOYUEDUREWARDERR_NTF.CLASS_ID);
					}
					return;
				}
			}
			if (selector != 1470L)
			{
				if (selector != 1471L)
				{
					if (selector != 1614L)
					{
						if (selector != 1615L)
						{
							if (selector != 1442L)
							{
								if (selector != 1450L)
								{
									if (selector != 1480L)
									{
										if (this.dataObject != null)
										{
											this.dataObject.Release();
											this.dataObject = null;
										}
									}
									else if (!(this.dataObject is CSPKG_CHKXUNYOUSERV_REQ))
									{
										if (this.dataObject != null)
										{
											this.dataObject.Release();
										}
										this.dataObject = (CSPKG_CHKXUNYOUSERV_REQ)ProtocolObjectPool.Get(CSPKG_CHKXUNYOUSERV_REQ.CLASS_ID);
									}
								}
								else if (!(this.dataObject is SCPKG_ROLLINGMSG_NTF))
								{
									if (this.dataObject != null)
									{
										this.dataObject.Release();
									}
									this.dataObject = (SCPKG_ROLLINGMSG_NTF)ProtocolObjectPool.Get(SCPKG_ROLLINGMSG_NTF.CLASS_ID);
								}
							}
							else if (!(this.dataObject is SCPKG_FIGHTHISTORYLIST_RSP))
							{
								if (this.dataObject != null)
								{
									this.dataObject.Release();
								}
								this.dataObject = (SCPKG_FIGHTHISTORYLIST_RSP)ProtocolObjectPool.Get(SCPKG_FIGHTHISTORYLIST_RSP.CLASS_ID);
							}
						}
						else if (!(this.dataObject is SCPKG_GETPVPLEVELREWARD_RSP))
						{
							if (this.dataObject != null)
							{
								this.dataObject.Release();
							}
							this.dataObject = (SCPKG_GETPVPLEVELREWARD_RSP)ProtocolObjectPool.Get(SCPKG_GETPVPLEVELREWARD_RSP.CLASS_ID);
						}
					}
					else if (!(this.dataObject is CSPKG_GETPVPLEVELREWARD_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_GETPVPLEVELREWARD_REQ)ProtocolObjectPool.Get(CSPKG_GETPVPLEVELREWARD_REQ.CLASS_ID);
					}
				}
				else if (!(this.dataObject is SCPKG_REDDOTLIST_RSP))
				{
					if (this.dataObject != null)
					{
						this.dataObject.Release();
					}
					this.dataObject = (SCPKG_REDDOTLIST_RSP)ProtocolObjectPool.Get(SCPKG_REDDOTLIST_RSP.CLASS_ID);
				}
			}
			else if (!(this.dataObject is CSPKG_REDDOTLIST_REQ))
			{
				if (this.dataObject != null)
				{
					this.dataObject.Release();
				}
				this.dataObject = (CSPKG_REDDOTLIST_REQ)ProtocolObjectPool.Get(CSPKG_REDDOTLIST_REQ.CLASS_ID);
			}
		}

		private void select_1907_2230(long selector)
		{
			if (selector >= 2010L && selector <= 2038L)
			{
				switch ((int)(selector - 2010L))
				{
				case 0:
					if (!(this.dataObject is CSPKG_MATCH_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_MATCH_REQ)ProtocolObjectPool.Get(CSPKG_MATCH_REQ.CLASS_ID);
					}
					return;
				case 1:
					if (!(this.dataObject is SCPKG_MATCH_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_MATCH_RSP)ProtocolObjectPool.Get(SCPKG_MATCH_RSP.CLASS_ID);
					}
					return;
				case 2:
					if (!(this.dataObject is SCPKG_ROOM_STARTSINGLEGAME_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_ROOM_STARTSINGLEGAME_NTF)ProtocolObjectPool.Get(SCPKG_ROOM_STARTSINGLEGAME_NTF.CLASS_ID);
					}
					return;
				case 3:
					if (!(this.dataObject is CSPKG_START_MULTI_GAME_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_START_MULTI_GAME_REQ)ProtocolObjectPool.Get(CSPKG_START_MULTI_GAME_REQ.CLASS_ID);
					}
					return;
				case 4:
					if (!(this.dataObject is SCPKG_START_MULTI_GAME_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_START_MULTI_GAME_RSP)ProtocolObjectPool.Get(SCPKG_START_MULTI_GAME_RSP.CLASS_ID);
					}
					return;
				case 5:
					if (!(this.dataObject is CSPKG_ADD_NPC_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_ADD_NPC_REQ)ProtocolObjectPool.Get(CSPKG_ADD_NPC_REQ.CLASS_ID);
					}
					return;
				case 7:
					if (!(this.dataObject is CSPKG_INVITE_FRIEND_JOIN_ROOM_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_INVITE_FRIEND_JOIN_ROOM_REQ)ProtocolObjectPool.Get(CSPKG_INVITE_FRIEND_JOIN_ROOM_REQ.CLASS_ID);
					}
					return;
				case 8:
					if (!(this.dataObject is SCPKG_INVITE_FRIEND_JOIN_ROOM_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_INVITE_FRIEND_JOIN_ROOM_RSP)ProtocolObjectPool.Get(SCPKG_INVITE_FRIEND_JOIN_ROOM_RSP.CLASS_ID);
					}
					return;
				case 9:
					if (!(this.dataObject is CSPKG_KICKOUT_ROOMMEMBER_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_KICKOUT_ROOMMEMBER_REQ)ProtocolObjectPool.Get(CSPKG_KICKOUT_ROOMMEMBER_REQ.CLASS_ID);
					}
					return;
				case 10:
					if (!(this.dataObject is SCPKG_INVITE_JOIN_GAME_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_INVITE_JOIN_GAME_REQ)ProtocolObjectPool.Get(SCPKG_INVITE_JOIN_GAME_REQ.CLASS_ID);
					}
					return;
				case 11:
					if (!(this.dataObject is CSPKG_INVITE_JOIN_GAME_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_INVITE_JOIN_GAME_RSP)ProtocolObjectPool.Get(CSPKG_INVITE_JOIN_GAME_RSP.CLASS_ID);
					}
					return;
				case 12:
					if (!(this.dataObject is CSPKG_CREATE_TEAM_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CREATE_TEAM_REQ)ProtocolObjectPool.Get(CSPKG_CREATE_TEAM_REQ.CLASS_ID);
					}
					return;
				case 13:
					if (!(this.dataObject is SCPKG_JOIN_TEAM_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_JOIN_TEAM_RSP)ProtocolObjectPool.Get(SCPKG_JOIN_TEAM_RSP.CLASS_ID);
					}
					return;
				case 14:
					if (!(this.dataObject is CSPKG_INVITE_FRIEND_JOIN_TEAM_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_INVITE_FRIEND_JOIN_TEAM_REQ)ProtocolObjectPool.Get(CSPKG_INVITE_FRIEND_JOIN_TEAM_REQ.CLASS_ID);
					}
					return;
				case 15:
					if (!(this.dataObject is SCPKG_INVITE_FRIEND_JOIN_TEAM_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_INVITE_FRIEND_JOIN_TEAM_RSP)ProtocolObjectPool.Get(SCPKG_INVITE_FRIEND_JOIN_TEAM_RSP.CLASS_ID);
					}
					return;
				case 16:
					if (!(this.dataObject is SCPKG_TEAM_CHG))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_TEAM_CHG)ProtocolObjectPool.Get(SCPKG_TEAM_CHG.CLASS_ID);
					}
					return;
				case 17:
					if (!(this.dataObject is CSPKG_LEAVL_TEAM))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_LEAVL_TEAM)ProtocolObjectPool.Get(CSPKG_LEAVL_TEAM.CLASS_ID);
					}
					return;
				case 18:
					if (!(this.dataObject is CSPKG_OPER_TEAM_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_OPER_TEAM_REQ)ProtocolObjectPool.Get(CSPKG_OPER_TEAM_REQ.CLASS_ID);
					}
					return;
				case 19:
					if (!(this.dataObject is SCPKG_SELF_BEKICK_FROM_TEAM))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_SELF_BEKICK_FROM_TEAM)ProtocolObjectPool.Get(SCPKG_SELF_BEKICK_FROM_TEAM.CLASS_ID);
					}
					return;
				case 20:
					if (!(this.dataObject is SCPKG_ACNT_LEAVE_TEAM_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_ACNT_LEAVE_TEAM_RSP)ProtocolObjectPool.Get(SCPKG_ACNT_LEAVE_TEAM_RSP.CLASS_ID);
					}
					return;
				case 21:
					if (!(this.dataObject is CSPKG_ROOM_CONFIRM_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_ROOM_CONFIRM_REQ)ProtocolObjectPool.Get(CSPKG_ROOM_CONFIRM_REQ.CLASS_ID);
					}
					return;
				case 22:
					if (!(this.dataObject is SCPKG_ROOM_CONFIRM_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_ROOM_CONFIRM_RSP)ProtocolObjectPool.Get(SCPKG_ROOM_CONFIRM_RSP.CLASS_ID);
					}
					return;
				case 23:
					if (!(this.dataObject is CSPKG_ROOM_CHGMEMBERPOS_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_ROOM_CHGMEMBERPOS_REQ)ProtocolObjectPool.Get(CSPKG_ROOM_CHGMEMBERPOS_REQ.CLASS_ID);
					}
					return;
				case 24:
					if (!(this.dataObject is CSPKG_INVITE_GUILD_MEMBER_JOIN_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_INVITE_GUILD_MEMBER_JOIN_REQ)ProtocolObjectPool.Get(CSPKG_INVITE_GUILD_MEMBER_JOIN_REQ.CLASS_ID);
					}
					return;
				case 25:
					if (!(this.dataObject is CSPKG_GET_GUILD_MEMBER_GAME_STATE_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_GET_GUILD_MEMBER_GAME_STATE_REQ)ProtocolObjectPool.Get(CSPKG_GET_GUILD_MEMBER_GAME_STATE_REQ.CLASS_ID);
					}
					return;
				case 26:
					if (!(this.dataObject is SCPKG_GET_GUILD_MEMBER_GAME_STATE_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_GET_GUILD_MEMBER_GAME_STATE_RSP)ProtocolObjectPool.Get(SCPKG_GET_GUILD_MEMBER_GAME_STATE_RSP.CLASS_ID);
					}
					return;
				case 27:
					if (!(this.dataObject is CSPKG_ROOM_CHGPOS_CONFIRM_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_ROOM_CHGPOS_CONFIRM_REQ)ProtocolObjectPool.Get(CSPKG_ROOM_CHGPOS_CONFIRM_REQ.CLASS_ID);
					}
					return;
				case 28:
					if (!(this.dataObject is SCPKG_ROOM_CHGPOS_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_ROOM_CHGPOS_NTF)ProtocolObjectPool.Get(SCPKG_ROOM_CHGPOS_NTF.CLASS_ID);
					}
					return;
				}
			}
			if (selector >= 2203L && selector <= 2230L)
			{
				switch ((int)(selector - 2203L))
				{
				case 0:
					if (!(this.dataObject is CSPKG_GET_PREPARE_GUILD_LIST_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_GET_PREPARE_GUILD_LIST_REQ)ProtocolObjectPool.Get(CSPKG_GET_PREPARE_GUILD_LIST_REQ.CLASS_ID);
					}
					return;
				case 1:
					if (!(this.dataObject is SCPKG_GET_PREPARE_GUILD_LIST_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_GET_PREPARE_GUILD_LIST_RSP)ProtocolObjectPool.Get(SCPKG_GET_PREPARE_GUILD_LIST_RSP.CLASS_ID);
					}
					return;
				case 2:
					if (!(this.dataObject is CSPKG_GET_GUILD_INFO_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_GET_GUILD_INFO_REQ)ProtocolObjectPool.Get(CSPKG_GET_GUILD_INFO_REQ.CLASS_ID);
					}
					return;
				case 3:
					if (!(this.dataObject is SCPKG_GET_GUILD_INFO_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_GET_GUILD_INFO_RSP)ProtocolObjectPool.Get(SCPKG_GET_GUILD_INFO_RSP.CLASS_ID);
					}
					return;
				case 4:
					if (!(this.dataObject is CSPKG_GET_PREPARE_GUILD_INFO_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_GET_PREPARE_GUILD_INFO_REQ)ProtocolObjectPool.Get(CSPKG_GET_PREPARE_GUILD_INFO_REQ.CLASS_ID);
					}
					return;
				case 5:
					if (!(this.dataObject is SCPKG_GET_PREPARE_GUILD_INFO_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_GET_PREPARE_GUILD_INFO_RSP)ProtocolObjectPool.Get(SCPKG_GET_PREPARE_GUILD_INFO_RSP.CLASS_ID);
					}
					return;
				case 6:
					if (!(this.dataObject is CSPKG_CREATE_GUILD_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CREATE_GUILD_REQ)ProtocolObjectPool.Get(CSPKG_CREATE_GUILD_REQ.CLASS_ID);
					}
					return;
				case 7:
					if (!(this.dataObject is SCPKG_CREATE_GUILD_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CREATE_GUILD_RSP)ProtocolObjectPool.Get(SCPKG_CREATE_GUILD_RSP.CLASS_ID);
					}
					return;
				case 8:
					if (!(this.dataObject is CSPKG_JOIN_PREPARE_GUILD_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_JOIN_PREPARE_GUILD_REQ)ProtocolObjectPool.Get(CSPKG_JOIN_PREPARE_GUILD_REQ.CLASS_ID);
					}
					return;
				case 9:
					if (!(this.dataObject is SCPKG_JOIN_PREPARE_GUILD_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_JOIN_PREPARE_GUILD_RSP)ProtocolObjectPool.Get(SCPKG_JOIN_PREPARE_GUILD_RSP.CLASS_ID);
					}
					return;
				case 10:
					if (!(this.dataObject is SCPKG_JOIN_PREPARE_GUILD_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_JOIN_PREPARE_GUILD_NTF)ProtocolObjectPool.Get(SCPKG_JOIN_PREPARE_GUILD_NTF.CLASS_ID);
					}
					return;
				case 11:
					if (!(this.dataObject is SCPKG_ADD_GUILD_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_ADD_GUILD_NTF)ProtocolObjectPool.Get(SCPKG_ADD_GUILD_NTF.CLASS_ID);
					}
					return;
				case 12:
					if (!(this.dataObject is SCPKG_MEMBER_ONLINE_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_MEMBER_ONLINE_NTF)ProtocolObjectPool.Get(SCPKG_MEMBER_ONLINE_NTF.CLASS_ID);
					}
					return;
				case 13:
					if (!(this.dataObject is SCPKG_PREPARE_GUILD_BREAK_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_PREPARE_GUILD_BREAK_NTF)ProtocolObjectPool.Get(SCPKG_PREPARE_GUILD_BREAK_NTF.CLASS_ID);
					}
					return;
				case 14:
					if (!(this.dataObject is CSPKG_MODIFY_GUILD_SETTING_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_MODIFY_GUILD_SETTING_REQ)ProtocolObjectPool.Get(CSPKG_MODIFY_GUILD_SETTING_REQ.CLASS_ID);
					}
					return;
				case 15:
					if (!(this.dataObject is SCPKG_MODIFY_GUILD_SETTING_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_MODIFY_GUILD_SETTING_RSP)ProtocolObjectPool.Get(SCPKG_MODIFY_GUILD_SETTING_RSP.CLASS_ID);
					}
					return;
				case 16:
					if (!(this.dataObject is CSPKG_GET_GUILD_APPLY_LIST_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_GET_GUILD_APPLY_LIST_REQ)ProtocolObjectPool.Get(CSPKG_GET_GUILD_APPLY_LIST_REQ.CLASS_ID);
					}
					return;
				case 17:
					if (!(this.dataObject is SCPKG_GET_GUILD_APPLY_LIST_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_GET_GUILD_APPLY_LIST_RSP)ProtocolObjectPool.Get(SCPKG_GET_GUILD_APPLY_LIST_RSP.CLASS_ID);
					}
					return;
				case 18:
					if (!(this.dataObject is CSPKG_APPLY_JOIN_GUILD_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_APPLY_JOIN_GUILD_REQ)ProtocolObjectPool.Get(CSPKG_APPLY_JOIN_GUILD_REQ.CLASS_ID);
					}
					return;
				case 19:
					if (!(this.dataObject is SCPKG_APPLY_JOIN_GUILD_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_APPLY_JOIN_GUILD_RSP)ProtocolObjectPool.Get(SCPKG_APPLY_JOIN_GUILD_RSP.CLASS_ID);
					}
					return;
				case 20:
					if (!(this.dataObject is SCPKG_JOIN_GUILD_APPLY_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_JOIN_GUILD_APPLY_NTF)ProtocolObjectPool.Get(SCPKG_JOIN_GUILD_APPLY_NTF.CLASS_ID);
					}
					return;
				case 21:
					if (!(this.dataObject is SCPKG_NEW_MEMBER_JOIN_GULD_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_NEW_MEMBER_JOIN_GULD_NTF)ProtocolObjectPool.Get(SCPKG_NEW_MEMBER_JOIN_GULD_NTF.CLASS_ID);
					}
					return;
				case 22:
					if (!(this.dataObject is CSPKG_APPROVE_JOIN_GUILD_APPLY))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_APPROVE_JOIN_GUILD_APPLY)ProtocolObjectPool.Get(CSPKG_APPROVE_JOIN_GUILD_APPLY.CLASS_ID);
					}
					return;
				case 23:
					if (!(this.dataObject is CSPKG_QUIT_GUILD_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_QUIT_GUILD_REQ)ProtocolObjectPool.Get(CSPKG_QUIT_GUILD_REQ.CLASS_ID);
					}
					return;
				case 24:
					if (!(this.dataObject is SCPKG_QUIT_GUILD_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_QUIT_GUILD_RSP)ProtocolObjectPool.Get(SCPKG_QUIT_GUILD_RSP.CLASS_ID);
					}
					return;
				case 25:
					if (!(this.dataObject is SCPKG_QUIT_GUILD_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_QUIT_GUILD_NTF)ProtocolObjectPool.Get(SCPKG_QUIT_GUILD_NTF.CLASS_ID);
					}
					return;
				case 26:
					if (!(this.dataObject is CSPKG_GUILD_INVITE_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_GUILD_INVITE_REQ)ProtocolObjectPool.Get(CSPKG_GUILD_INVITE_REQ.CLASS_ID);
					}
					return;
				case 27:
					if (!(this.dataObject is SCPKG_GUILD_INVITE_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_GUILD_INVITE_RSP)ProtocolObjectPool.Get(SCPKG_GUILD_INVITE_RSP.CLASS_ID);
					}
					return;
				}
			}
			if (selector >= 1907L && selector <= 1910L)
			{
				switch ((int)(selector - 1907L))
				{
				case 0:
					if (!(this.dataObject is CSPKG_ACNT_PSWDINFO_FORCE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_ACNT_PSWDINFO_FORCE)ProtocolObjectPool.Get(CSPKG_ACNT_PSWDINFO_FORCE.CLASS_ID);
					}
					return;
				case 1:
					if (!(this.dataObject is SCPKG_ACNT_PSWDINFO_FORCE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_ACNT_PSWDINFO_FORCE)ProtocolObjectPool.Get(SCPKG_ACNT_PSWDINFO_FORCE.CLASS_ID);
					}
					return;
				case 2:
					if (!(this.dataObject is CSPKG_ACNT_PSWDINFO_FORCECAL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_ACNT_PSWDINFO_FORCECAL)ProtocolObjectPool.Get(CSPKG_ACNT_PSWDINFO_FORCECAL.CLASS_ID);
					}
					return;
				case 3:
					if (!(this.dataObject is SCPKG_ACNT_PSWDINFO_FORCECAL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_ACNT_PSWDINFO_FORCECAL)ProtocolObjectPool.Get(SCPKG_ACNT_PSWDINFO_FORCECAL.CLASS_ID);
					}
					return;
				}
			}
			if (this.dataObject != null)
			{
				this.dataObject.Release();
				this.dataObject = null;
			}
		}

		private void select_2231_2602(long selector)
		{
			if (selector >= 2231L && selector <= 2292L)
			{
				switch ((int)(selector - 2231L))
				{
				case 0:
					if (!(this.dataObject is CSPKG_SEARCH_GUILD_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_SEARCH_GUILD_REQ)ProtocolObjectPool.Get(CSPKG_SEARCH_GUILD_REQ.CLASS_ID);
					}
					return;
				case 1:
					if (!(this.dataObject is SCPKG_SEARCH_GUILD_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_SEARCH_GUILD_RSP)ProtocolObjectPool.Get(SCPKG_SEARCH_GUILD_RSP.CLASS_ID);
					}
					return;
				case 2:
					if (!(this.dataObject is CSPKG_DEAL_GUILD_INVITE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_DEAL_GUILD_INVITE)ProtocolObjectPool.Get(CSPKG_DEAL_GUILD_INVITE.CLASS_ID);
					}
					return;
				case 3:
					if (!(this.dataObject is CSPKG_GUILD_RECOMMEND_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_GUILD_RECOMMEND_REQ)ProtocolObjectPool.Get(CSPKG_GUILD_RECOMMEND_REQ.CLASS_ID);
					}
					return;
				case 4:
					if (!(this.dataObject is SCPKG_GUILD_RECOMMEND_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_GUILD_RECOMMEND_RSP)ProtocolObjectPool.Get(SCPKG_GUILD_RECOMMEND_RSP.CLASS_ID);
					}
					return;
				case 5:
					if (!(this.dataObject is SCPKG_GUILD_RECOMMEND_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_GUILD_RECOMMEND_NTF)ProtocolObjectPool.Get(SCPKG_GUILD_RECOMMEND_NTF.CLASS_ID);
					}
					return;
				case 6:
					if (!(this.dataObject is CSPKG_GET_GUILD_RECOMMEND_LIST_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_GET_GUILD_RECOMMEND_LIST_REQ)ProtocolObjectPool.Get(CSPKG_GET_GUILD_RECOMMEND_LIST_REQ.CLASS_ID);
					}
					return;
				case 7:
					if (!(this.dataObject is SCPKG_GET_GUILD_RECOMMEND_LIST_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_GET_GUILD_RECOMMEND_LIST_RSP)ProtocolObjectPool.Get(SCPKG_GET_GUILD_RECOMMEND_LIST_RSP.CLASS_ID);
					}
					return;
				case 8:
					if (!(this.dataObject is CSPKG_REJECT_GUILD_RECOMMEND))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_REJECT_GUILD_RECOMMEND)ProtocolObjectPool.Get(CSPKG_REJECT_GUILD_RECOMMEND.CLASS_ID);
					}
					return;
				case 9:
					if (!(this.dataObject is SCPKG_DEAL_GUILD_INVITE_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_DEAL_GUILD_INVITE_RSP)ProtocolObjectPool.Get(SCPKG_DEAL_GUILD_INVITE_RSP.CLASS_ID);
					}
					return;
				case 10:
					if (!(this.dataObject is CSPKG_SEARCH_PREGUILD_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_SEARCH_PREGUILD_REQ)ProtocolObjectPool.Get(CSPKG_SEARCH_PREGUILD_REQ.CLASS_ID);
					}
					return;
				case 11:
					if (!(this.dataObject is SCPKG_SEARCH_PREGUILD_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_SEARCH_PREGUILD_RSP)ProtocolObjectPool.Get(SCPKG_SEARCH_PREGUILD_RSP.CLASS_ID);
					}
					return;
				case 14:
					if (!(this.dataObject is SCPKG_GUILD_LEVEL_CHANGE_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_GUILD_LEVEL_CHANGE_NTF)ProtocolObjectPool.Get(SCPKG_GUILD_LEVEL_CHANGE_NTF.CLASS_ID);
					}
					return;
				case 18:
					if (!(this.dataObject is CSPKG_APPOINT_POSITION_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_APPOINT_POSITION_REQ)ProtocolObjectPool.Get(CSPKG_APPOINT_POSITION_REQ.CLASS_ID);
					}
					return;
				case 19:
					if (!(this.dataObject is SCPKG_APPOINT_POSITION_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_APPOINT_POSITION_RSP)ProtocolObjectPool.Get(SCPKG_APPOINT_POSITION_RSP.CLASS_ID);
					}
					return;
				case 20:
					if (!(this.dataObject is SCPKG_GUILD_POSITION_CHG_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_GUILD_POSITION_CHG_NTF)ProtocolObjectPool.Get(SCPKG_GUILD_POSITION_CHG_NTF.CLASS_ID);
					}
					return;
				case 21:
					if (!(this.dataObject is CSPKG_FIRE_GUILD_MEMBER_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_FIRE_GUILD_MEMBER_REQ)ProtocolObjectPool.Get(CSPKG_FIRE_GUILD_MEMBER_REQ.CLASS_ID);
					}
					return;
				case 22:
					if (!(this.dataObject is SCPKG_FIRE_GUILD_MEMBER_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_FIRE_GUILD_MEMBER_RSP)ProtocolObjectPool.Get(SCPKG_FIRE_GUILD_MEMBER_RSP.CLASS_ID);
					}
					return;
				case 23:
					if (!(this.dataObject is SCPKG_FIRE_GUILD_MEMBER_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_FIRE_GUILD_MEMBER_NTF)ProtocolObjectPool.Get(SCPKG_FIRE_GUILD_MEMBER_NTF.CLASS_ID);
					}
					return;
				case 33:
					if (!(this.dataObject is SCPKG_GUILD_CROSS_DAY_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_GUILD_CROSS_DAY_NTF)ProtocolObjectPool.Get(SCPKG_GUILD_CROSS_DAY_NTF.CLASS_ID);
					}
					return;
				case 36:
					if (!(this.dataObject is SCPKG_MEMBER_RANK_POINT_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_MEMBER_RANK_POINT_NTF)ProtocolObjectPool.Get(SCPKG_MEMBER_RANK_POINT_NTF.CLASS_ID);
					}
					return;
				case 37:
					if (!(this.dataObject is SCPKG_GUILD_RANK_RESET_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_GUILD_RANK_RESET_NTF)ProtocolObjectPool.Get(SCPKG_GUILD_RANK_RESET_NTF.CLASS_ID);
					}
					return;
				case 40:
					if (!(this.dataObject is SCPKG_GUILD_CONSTRUCT_CHG))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_GUILD_CONSTRUCT_CHG)ProtocolObjectPool.Get(SCPKG_GUILD_CONSTRUCT_CHG.CLASS_ID);
					}
					return;
				case 41:
					if (!(this.dataObject is CSPKG_CHG_GUILD_HEADID_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CHG_GUILD_HEADID_REQ)ProtocolObjectPool.Get(CSPKG_CHG_GUILD_HEADID_REQ.CLASS_ID);
					}
					return;
				case 42:
					if (!(this.dataObject is SCPKG_CHG_GUILD_HEADID_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CHG_GUILD_HEADID_RSP)ProtocolObjectPool.Get(SCPKG_CHG_GUILD_HEADID_RSP.CLASS_ID);
					}
					return;
				case 43:
					if (!(this.dataObject is CSPKG_CHG_GUILD_NOTICE_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CHG_GUILD_NOTICE_REQ)ProtocolObjectPool.Get(CSPKG_CHG_GUILD_NOTICE_REQ.CLASS_ID);
					}
					return;
				case 44:
					if (!(this.dataObject is SCPKG_CHG_GUILD_NOTICE_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CHG_GUILD_NOTICE_RSP)ProtocolObjectPool.Get(SCPKG_CHG_GUILD_NOTICE_RSP.CLASS_ID);
					}
					return;
				case 45:
					if (!(this.dataObject is CSPKG_UPGRADE_GUILD_BY_COUPONS_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_UPGRADE_GUILD_BY_COUPONS_REQ)ProtocolObjectPool.Get(CSPKG_UPGRADE_GUILD_BY_COUPONS_REQ.CLASS_ID);
					}
					return;
				case 46:
					if (!(this.dataObject is SCPKG_UPGRADE_GUILD_BY_COUPONS_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_UPGRADE_GUILD_BY_COUPONS_RSP)ProtocolObjectPool.Get(SCPKG_UPGRADE_GUILD_BY_COUPONS_RSP.CLASS_ID);
					}
					return;
				case 47:
					if (!(this.dataObject is CSPKG_GUILD_SIGNIN_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_GUILD_SIGNIN_REQ)ProtocolObjectPool.Get(CSPKG_GUILD_SIGNIN_REQ.CLASS_ID);
					}
					return;
				case 48:
					if (!(this.dataObject is SCPKG_GUILD_SIGNIN_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_GUILD_SIGNIN_RSP)ProtocolObjectPool.Get(SCPKG_GUILD_SIGNIN_RSP.CLASS_ID);
					}
					return;
				case 49:
					if (!(this.dataObject is SCPKG_GUILD_SEASON_RESET_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_GUILD_SEASON_RESET_NTF)ProtocolObjectPool.Get(SCPKG_GUILD_SEASON_RESET_NTF.CLASS_ID);
					}
					return;
				case 50:
					if (!(this.dataObject is CSPKG_GET_GROUP_GUILD_ID_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_GET_GROUP_GUILD_ID_REQ)ProtocolObjectPool.Get(CSPKG_GET_GROUP_GUILD_ID_REQ.CLASS_ID);
					}
					return;
				case 51:
					if (!(this.dataObject is SCPKG_GET_GROUP_GUILD_ID_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_GET_GROUP_GUILD_ID_NTF)ProtocolObjectPool.Get(SCPKG_GET_GROUP_GUILD_ID_NTF.CLASS_ID);
					}
					return;
				case 52:
					if (!(this.dataObject is CSPKG_SET_GUILD_GROUP_OPENID_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_SET_GUILD_GROUP_OPENID_REQ)ProtocolObjectPool.Get(CSPKG_SET_GUILD_GROUP_OPENID_REQ.CLASS_ID);
					}
					return;
				case 53:
					if (!(this.dataObject is SCPKG_SET_GUILD_GROUP_OPENID_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_SET_GUILD_GROUP_OPENID_NTF)ProtocolObjectPool.Get(SCPKG_SET_GUILD_GROUP_OPENID_NTF.CLASS_ID);
					}
					return;
				case 54:
					if (!(this.dataObject is CSPKG_GUILD_EVENT_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_GUILD_EVENT_REQ)ProtocolObjectPool.Get(CSPKG_GUILD_EVENT_REQ.CLASS_ID);
					}
					return;
				case 55:
					if (!(this.dataObject is SCPKG_GUILD_EVENT_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_GUILD_EVENT_RSP)ProtocolObjectPool.Get(SCPKG_GUILD_EVENT_RSP.CLASS_ID);
					}
					return;
				case 56:
					if (!(this.dataObject is CSPKG_SEND_GUILD_RECRUIT_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_SEND_GUILD_RECRUIT_REQ)ProtocolObjectPool.Get(CSPKG_SEND_GUILD_RECRUIT_REQ.CLASS_ID);
					}
					return;
				case 57:
					if (!(this.dataObject is SCPKG_SEND_GUILD_RECRUIT_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_SEND_GUILD_RECRUIT_RSP)ProtocolObjectPool.Get(SCPKG_SEND_GUILD_RECRUIT_RSP.CLASS_ID);
					}
					return;
				case 58:
					if (!(this.dataObject is CSPKG_GET_GUILD_RECRUIT_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_GET_GUILD_RECRUIT_REQ)ProtocolObjectPool.Get(CSPKG_GET_GUILD_RECRUIT_REQ.CLASS_ID);
					}
					return;
				case 59:
					if (!(this.dataObject is SCPKG_GET_GUILD_RECRUIT_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_GET_GUILD_RECRUIT_RSP)ProtocolObjectPool.Get(SCPKG_GET_GUILD_RECRUIT_RSP.CLASS_ID);
					}
					return;
				case 60:
					if (!(this.dataObject is CSPKG_GUILD_BINDQUNLOG_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_GUILD_BINDQUNLOG_REQ)ProtocolObjectPool.Get(CSPKG_GUILD_BINDQUNLOG_REQ.CLASS_ID);
					}
					return;
				case 61:
					if (!(this.dataObject is CSPKG_GUILD_UNBINDQUNLOG_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_GUILD_UNBINDQUNLOG_REQ)ProtocolObjectPool.Get(CSPKG_GUILD_UNBINDQUNLOG_REQ.CLASS_ID);
					}
					return;
				}
			}
			if (selector >= 2500L && selector <= 2512L)
			{
				switch ((int)(selector - 2500L))
				{
				case 0:
					if (!(this.dataObject is CSPKG_ACNTACTIVITYINFO_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_ACNTACTIVITYINFO_REQ)ProtocolObjectPool.Get(CSPKG_ACNTACTIVITYINFO_REQ.CLASS_ID);
					}
					return;
				case 1:
					if (!(this.dataObject is SCPKG_ACNTACTIVITYINFO_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_ACNTACTIVITYINFO_RSP)ProtocolObjectPool.Get(SCPKG_ACNTACTIVITYINFO_RSP.CLASS_ID);
					}
					return;
				case 2:
					if (!(this.dataObject is SCPKG_ACTIVITYENDDEPLETION_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_ACTIVITYENDDEPLETION_NTF)ProtocolObjectPool.Get(SCPKG_ACTIVITYENDDEPLETION_NTF.CLASS_ID);
					}
					return;
				case 3:
					if (!(this.dataObject is CSPKG_DRAWWEAL_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_DRAWWEAL_REQ)ProtocolObjectPool.Get(CSPKG_DRAWWEAL_REQ.CLASS_ID);
					}
					return;
				case 4:
					if (!(this.dataObject is SCPKG_DRAWWEAL_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_DRAWWEAL_RSP)ProtocolObjectPool.Get(SCPKG_DRAWWEAL_RSP.CLASS_ID);
					}
					return;
				case 5:
					if (!(this.dataObject is SCPKG_WEALDETAIL_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_WEALDETAIL_NTF)ProtocolObjectPool.Get(SCPKG_WEALDETAIL_NTF.CLASS_ID);
					}
					return;
				case 6:
					if (!(this.dataObject is SCPKG_WEAL_CON_DATA_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_WEAL_CON_DATA_NTF)ProtocolObjectPool.Get(SCPKG_WEAL_CON_DATA_NTF.CLASS_ID);
					}
					return;
				case 7:
					if (!(this.dataObject is CSPKG_WEAL_DATA_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_WEAL_DATA_REQ)ProtocolObjectPool.Get(CSPKG_WEAL_DATA_REQ.CLASS_ID);
					}
					return;
				case 8:
					if (!(this.dataObject is SCPKG_WEAL_DATA_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_WEAL_DATA_NTF)ProtocolObjectPool.Get(SCPKG_WEAL_DATA_NTF.CLASS_ID);
					}
					return;
				case 9:
					if (!(this.dataObject is SCPKG_PROP_MULTIPLE_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_PROP_MULTIPLE_NTF)ProtocolObjectPool.Get(SCPKG_PROP_MULTIPLE_NTF.CLASS_ID);
					}
					return;
				case 10:
					if (!(this.dataObject is SCPKG_RES_DATA_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_RES_DATA_NTF)ProtocolObjectPool.Get(SCPKG_RES_DATA_NTF.CLASS_ID);
					}
					return;
				case 11:
					if (!(this.dataObject is SCPKG_WEAL_EXCHANGE_RES))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_WEAL_EXCHANGE_RES)ProtocolObjectPool.Get(SCPKG_WEAL_EXCHANGE_RES.CLASS_ID);
					}
					return;
				case 12:
					if (!(this.dataObject is SCPKG_WEAL_POINTDATA_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_WEAL_POINTDATA_NTF)ProtocolObjectPool.Get(SCPKG_WEAL_POINTDATA_NTF.CLASS_ID);
					}
					return;
				}
			}
			if (selector >= 2600L && selector <= 2602L)
			{
				switch ((int)(selector - 2600L))
				{
				case 0:
					if (!(this.dataObject is SCPKG_CLASSOFRANKDETAIL_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CLASSOFRANKDETAIL_NTF)ProtocolObjectPool.Get(SCPKG_CLASSOFRANKDETAIL_NTF.CLASS_ID);
					}
					return;
				case 1:
					if (!(this.dataObject is SCPKG_UPDRANKINFO_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_UPDRANKINFO_NTF)ProtocolObjectPool.Get(SCPKG_UPDRANKINFO_NTF.CLASS_ID);
					}
					return;
				case 2:
					if (!(this.dataObject is CSPKG_GET_RANKING_LIST_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_GET_RANKING_LIST_REQ)ProtocolObjectPool.Get(CSPKG_GET_RANKING_LIST_REQ.CLASS_ID);
					}
					return;
				}
			}
			if (this.dataObject != null)
			{
				this.dataObject.Release();
				this.dataObject = null;
			}
		}

		private void select_2603_4102(long selector)
		{
			if (selector >= 2900L && selector <= 2918L)
			{
				switch ((int)(selector - 2900L))
				{
				case 0:
					if (!(this.dataObject is CSPKG_SETBATTLELISTOFARENA_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_SETBATTLELISTOFARENA_REQ)ProtocolObjectPool.Get(CSPKG_SETBATTLELISTOFARENA_REQ.CLASS_ID);
					}
					return;
				case 1:
					if (!(this.dataObject is SCPKG_SETBATTLELISTOFARENA_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_SETBATTLELISTOFARENA_RSP)ProtocolObjectPool.Get(SCPKG_SETBATTLELISTOFARENA_RSP.CLASS_ID);
					}
					return;
				case 3:
					if (!(this.dataObject is CSPKG_JOINARENA_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_JOINARENA_REQ)ProtocolObjectPool.Get(CSPKG_JOINARENA_REQ.CLASS_ID);
					}
					return;
				case 4:
					if (!(this.dataObject is SCPKG_JOINARENA_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_JOINARENA_RSP)ProtocolObjectPool.Get(SCPKG_JOINARENA_RSP.CLASS_ID);
					}
					return;
				case 5:
					if (!(this.dataObject is CSPKG_GETARENADATA_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_GETARENADATA_REQ)ProtocolObjectPool.Get(CSPKG_GETARENADATA_REQ.CLASS_ID);
					}
					return;
				case 6:
					if (!(this.dataObject is SCPKG_GETARENADATA_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_GETARENADATA_RSP)ProtocolObjectPool.Get(SCPKG_GETARENADATA_RSP.CLASS_ID);
					}
					return;
				case 7:
					if (!(this.dataObject is CSPKG_CHGARENAFIGHTERREQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CHGARENAFIGHTERREQ)ProtocolObjectPool.Get(CSPKG_CHGARENAFIGHTERREQ.CLASS_ID);
					}
					return;
				case 8:
					if (!(this.dataObject is SCPKG_CHGARENAFIGHTERRSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CHGARENAFIGHTERRSP)ProtocolObjectPool.Get(SCPKG_CHGARENAFIGHTERRSP.CLASS_ID);
					}
					return;
				case 9:
					if (!(this.dataObject is CSPKG_GETTOPFIGHTEROFARENA_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_GETTOPFIGHTEROFARENA_REQ)ProtocolObjectPool.Get(CSPKG_GETTOPFIGHTEROFARENA_REQ.CLASS_ID);
					}
					return;
				case 10:
					if (!(this.dataObject is SCPKG_GETTOPFIGHTEROFARENA_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_GETTOPFIGHTEROFARENA_RSP)ProtocolObjectPool.Get(SCPKG_GETTOPFIGHTEROFARENA_RSP.CLASS_ID);
					}
					return;
				case 11:
					if (!(this.dataObject is CSPKG_GETARENAFIGHTHISTORY_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_GETARENAFIGHTHISTORY_REQ)ProtocolObjectPool.Get(CSPKG_GETARENAFIGHTHISTORY_REQ.CLASS_ID);
					}
					return;
				case 12:
					if (!(this.dataObject is SCPKG_GETARENAFIGHTHISTORY_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_GETARENAFIGHTHISTORY_RSP)ProtocolObjectPool.Get(SCPKG_GETARENAFIGHTHISTORY_RSP.CLASS_ID);
					}
					return;
				case 13:
					if (!(this.dataObject is SCPKG_RANKCURSEASONHISTORY_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_RANKCURSEASONHISTORY_NTF)ProtocolObjectPool.Get(SCPKG_RANKCURSEASONHISTORY_NTF.CLASS_ID);
					}
					return;
				case 14:
					if (!(this.dataObject is SCPKG_RANKPASTSEASONHISTORY_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_RANKPASTSEASONHISTORY_NTF)ProtocolObjectPool.Get(SCPKG_RANKPASTSEASONHISTORY_NTF.CLASS_ID);
					}
					return;
				case 15:
					if (!(this.dataObject is CSPKG_GETRANKREWARD_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_GETRANKREWARD_REQ)ProtocolObjectPool.Get(CSPKG_GETRANKREWARD_REQ.CLASS_ID);
					}
					return;
				case 16:
					if (!(this.dataObject is SCPKG_GETRANKREWARD_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_GETRANKREWARD_RSP)ProtocolObjectPool.Get(SCPKG_GETRANKREWARD_RSP.CLASS_ID);
					}
					return;
				case 17:
					if (!(this.dataObject is SCPKG_NTF_ADDCURSEASONRECORD))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_NTF_ADDCURSEASONRECORD)ProtocolObjectPool.Get(SCPKG_NTF_ADDCURSEASONRECORD.CLASS_ID);
					}
					return;
				case 18:
					if (!(this.dataObject is SCPKG_NTF_ADDPASTSEASONRECORD))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_NTF_ADDPASTSEASONRECORD)ProtocolObjectPool.Get(SCPKG_NTF_ADDPASTSEASONRECORD.CLASS_ID);
					}
					return;
				}
			}
			if (selector >= 2603L && selector <= 2618L)
			{
				switch ((int)(selector - 2603L))
				{
				case 0:
					if (!(this.dataObject is SCPKG_GET_RANKING_LIST_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_GET_RANKING_LIST_RSP)ProtocolObjectPool.Get(SCPKG_GET_RANKING_LIST_RSP.CLASS_ID);
					}
					return;
				case 1:
					if (!(this.dataObject is CSPKG_GET_RANKING_ACNT_INFO_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_GET_RANKING_ACNT_INFO_REQ)ProtocolObjectPool.Get(CSPKG_GET_RANKING_ACNT_INFO_REQ.CLASS_ID);
					}
					return;
				case 2:
					if (!(this.dataObject is SCPKG_GET_RANKING_ACNT_INFO_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_GET_RANKING_ACNT_INFO_RSP)ProtocolObjectPool.Get(SCPKG_GET_RANKING_ACNT_INFO_RSP.CLASS_ID);
					}
					return;
				case 3:
					if (!(this.dataObject is CSPKG_GET_ACNT_DETAIL_INFO_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_GET_ACNT_DETAIL_INFO_REQ)ProtocolObjectPool.Get(CSPKG_GET_ACNT_DETAIL_INFO_REQ.CLASS_ID);
					}
					return;
				case 4:
					if (!(this.dataObject is SCPKG_GET_ACNT_DETAIL_INFO_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_GET_ACNT_DETAIL_INFO_RSP)ProtocolObjectPool.Get(SCPKG_GET_ACNT_DETAIL_INFO_RSP.CLASS_ID);
					}
					return;
				case 5:
					if (!(this.dataObject is CSPKG_SET_ACNT_NEWBIE_TYPE_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_SET_ACNT_NEWBIE_TYPE_REQ)ProtocolObjectPool.Get(CSPKG_SET_ACNT_NEWBIE_TYPE_REQ.CLASS_ID);
					}
					return;
				case 6:
					if (!(this.dataObject is SCPKG_SET_ACNT_NEWBIE_TYPE_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_SET_ACNT_NEWBIE_TYPE_RSP)ProtocolObjectPool.Get(SCPKG_SET_ACNT_NEWBIE_TYPE_RSP.CLASS_ID);
					}
					return;
				case 7:
					if (!(this.dataObject is CSPKG_GET_SELFRANKINFO))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_GET_SELFRANKINFO)ProtocolObjectPool.Get(CSPKG_GET_SELFRANKINFO.CLASS_ID);
					}
					return;
				case 8:
					if (!(this.dataObject is SCPKG_ACNT_RANKINFO_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_ACNT_RANKINFO_RSP)ProtocolObjectPool.Get(SCPKG_ACNT_RANKINFO_RSP.CLASS_ID);
					}
					return;
				case 9:
					if (!(this.dataObject is SCPKG_MONTH_WEEK_CARD_USE_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_MONTH_WEEK_CARD_USE_RSP)ProtocolObjectPool.Get(SCPKG_MONTH_WEEK_CARD_USE_RSP.CLASS_ID);
					}
					return;
				case 10:
					if (!(this.dataObject is CSPKG_GET_RANKLIST_BY_SPECIAL_SCORE_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_GET_RANKLIST_BY_SPECIAL_SCORE_REQ)ProtocolObjectPool.Get(CSPKG_GET_RANKLIST_BY_SPECIAL_SCORE_REQ.CLASS_ID);
					}
					return;
				case 11:
					if (!(this.dataObject is SCPKG_GET_RANKLIST_BY_SPECIAL_SCORE_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_GET_RANKLIST_BY_SPECIAL_SCORE_RSP)ProtocolObjectPool.Get(SCPKG_GET_RANKLIST_BY_SPECIAL_SCORE_RSP.CLASS_ID);
					}
					return;
				case 12:
					if (!(this.dataObject is CSPKG_GET_SPECIAL_GUILD_RANK_INFO_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_GET_SPECIAL_GUILD_RANK_INFO_REQ)ProtocolObjectPool.Get(CSPKG_GET_SPECIAL_GUILD_RANK_INFO_REQ.CLASS_ID);
					}
					return;
				case 13:
					if (!(this.dataObject is SCPKG_GET_SPECIAL_GUILD_RANK_INFO_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_GET_SPECIAL_GUILD_RANK_INFO_RSP)ProtocolObjectPool.Get(SCPKG_GET_SPECIAL_GUILD_RANK_INFO_RSP.CLASS_ID);
					}
					return;
				case 14:
					if (!(this.dataObject is CSPKG_GET_INTIMACY_RELATION_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_GET_INTIMACY_RELATION_REQ)ProtocolObjectPool.Get(CSPKG_GET_INTIMACY_RELATION_REQ.CLASS_ID);
					}
					return;
				case 15:
					if (!(this.dataObject is SCPKG_GET_INTIMACY_RELATION_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_GET_INTIMACY_RELATION_RSP)ProtocolObjectPool.Get(SCPKG_GET_INTIMACY_RELATION_RSP.CLASS_ID);
					}
					return;
				}
			}
			if (selector >= 2700L && selector <= 2705L)
			{
				switch ((int)(selector - 2700L))
				{
				case 0:
					if (!(this.dataObject is CSPKG_GET_BURNING_PROGRESS_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_GET_BURNING_PROGRESS_REQ)ProtocolObjectPool.Get(CSPKG_GET_BURNING_PROGRESS_REQ.CLASS_ID);
					}
					return;
				case 1:
					if (!(this.dataObject is SCPKG_GET_BURNING_PROGRESS_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_GET_BURNING_PROGRESS_RSP)ProtocolObjectPool.Get(SCPKG_GET_BURNING_PROGRESS_RSP.CLASS_ID);
					}
					return;
				case 2:
					if (!(this.dataObject is CSPKG_GET_BURNING_REWARD_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_GET_BURNING_REWARD_REQ)ProtocolObjectPool.Get(CSPKG_GET_BURNING_REWARD_REQ.CLASS_ID);
					}
					return;
				case 3:
					if (!(this.dataObject is SCPKG_GET_BURNING_REWARD_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_GET_BURNING_REWARD_RSP)ProtocolObjectPool.Get(SCPKG_GET_BURNING_REWARD_RSP.CLASS_ID);
					}
					return;
				case 4:
					if (!(this.dataObject is CSPKG_RESET_BURNING_PROGRESS_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_RESET_BURNING_PROGRESS_REQ)ProtocolObjectPool.Get(CSPKG_RESET_BURNING_PROGRESS_REQ.CLASS_ID);
					}
					return;
				case 5:
					if (!(this.dataObject is SCPKG_RESET_BURNING_PROGRESS_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_RESET_BURNING_PROGRESS_RSP)ProtocolObjectPool.Get(SCPKG_RESET_BURNING_PROGRESS_RSP.CLASS_ID);
					}
					return;
				}
			}
			if (selector >= 2800L && selector <= 2805L)
			{
				switch ((int)(selector - 2800L))
				{
				case 0:
					if (!(this.dataObject is CSPKG_PVP_COMMINFO_REPORT))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_PVP_COMMINFO_REPORT)ProtocolObjectPool.Get(CSPKG_PVP_COMMINFO_REPORT.CLASS_ID);
					}
					return;
				case 1:
					if (!(this.dataObject is CSPKG_PVP_GAMEDATA_REPORT))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_PVP_GAMEDATA_REPORT)ProtocolObjectPool.Get(CSPKG_PVP_GAMEDATA_REPORT.CLASS_ID);
					}
					return;
				case 2:
					if (!(this.dataObject is CSPKG_PVP_GAMELOG_REPORT))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_PVP_GAMELOG_REPORT)ProtocolObjectPool.Get(CSPKG_PVP_GAMELOG_REPORT.CLASS_ID);
					}
					return;
				case 3:
					if (!(this.dataObject is SCPKG_PVP_NTF_CLIENT))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_PVP_NTF_CLIENT)ProtocolObjectPool.Get(SCPKG_PVP_NTF_CLIENT.CLASS_ID);
					}
					return;
				case 4:
					if (!(this.dataObject is CSPKG_PVP_GAMEDATA_REPORTOVER))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_PVP_GAMEDATA_REPORTOVER)ProtocolObjectPool.Get(CSPKG_PVP_GAMEDATA_REPORTOVER.CLASS_ID);
					}
					return;
				case 5:
					if (!(this.dataObject is CSPKG_PVP_GAMELOG_REPORTOVER))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_PVP_GAMELOG_REPORTOVER)ProtocolObjectPool.Get(CSPKG_PVP_GAMELOG_REPORTOVER.CLASS_ID);
					}
					return;
				}
			}
			if (selector >= 4000L && selector <= 4003L)
			{
				switch ((int)(selector - 4000L))
				{
				case 0:
					if (!(this.dataObject is CSPKG_UPDATECLIENTBITS_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_UPDATECLIENTBITS_NTF)ProtocolObjectPool.Get(CSPKG_UPDATECLIENTBITS_NTF.CLASS_ID);
					}
					return;
				case 1:
					if (!(this.dataObject is CSPKG_SERVERTIME_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_SERVERTIME_REQ)ProtocolObjectPool.Get(CSPKG_SERVERTIME_REQ.CLASS_ID);
					}
					return;
				case 2:
					if (!(this.dataObject is SCPKG_SERVERTIME_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_SERVERTIME_RSP)ProtocolObjectPool.Get(SCPKG_SERVERTIME_RSP.CLASS_ID);
					}
					return;
				case 3:
					if (!(this.dataObject is CSPKG_UPDNEWCLIENTBITS_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_UPDNEWCLIENTBITS_NTF)ProtocolObjectPool.Get(CSPKG_UPDNEWCLIENTBITS_NTF.CLASS_ID);
					}
					return;
				}
			}
			if (selector >= 3200L && selector <= 3202L)
			{
				switch ((int)(selector - 3200L))
				{
				case 0:
					if (!(this.dataObject is CSPKG_CHANGE_NAME_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CHANGE_NAME_REQ)ProtocolObjectPool.Get(CSPKG_CHANGE_NAME_REQ.CLASS_ID);
					}
					return;
				case 1:
					if (!(this.dataObject is SCPKG_CHANGE_NAME_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CHANGE_NAME_RSP)ProtocolObjectPool.Get(SCPKG_CHANGE_NAME_RSP.CLASS_ID);
					}
					return;
				case 2:
					if (!(this.dataObject is SCPKG_GUILD_NAME_CHG_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_GUILD_NAME_CHG_NTF)ProtocolObjectPool.Get(SCPKG_GUILD_NAME_CHG_NTF.CLASS_ID);
					}
					return;
				}
			}
			if (selector >= 4100L && selector <= 4102L)
			{
				switch ((int)(selector - 4100L))
				{
				case 0:
					if (!(this.dataObject is SCPKG_CMD_NTF_FRIEND_GAME_STATE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_NTF_FRIEND_GAME_STATE)ProtocolObjectPool.Get(SCPKG_CMD_NTF_FRIEND_GAME_STATE.CLASS_ID);
					}
					return;
				case 1:
					if (!(this.dataObject is SCPKG_NTF_SNS_FRIEND))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_NTF_SNS_FRIEND)ProtocolObjectPool.Get(SCPKG_NTF_SNS_FRIEND.CLASS_ID);
					}
					return;
				case 2:
					if (!(this.dataObject is SCPKG_CHG_SNS_FRIEND_PROFILE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CHG_SNS_FRIEND_PROFILE)ProtocolObjectPool.Get(SCPKG_CHG_SNS_FRIEND_PROFILE.CLASS_ID);
					}
					return;
				}
			}
			if (selector != 3000L)
			{
				if (selector != 3001L)
				{
					if (selector != 3100L)
					{
						if (selector != 3101L)
						{
							if (this.dataObject != null)
							{
								this.dataObject.Release();
								this.dataObject = null;
							}
						}
						else if (!(this.dataObject is SCPKG_JOIN_TVOIP_ROOM_NTF))
						{
							if (this.dataObject != null)
							{
								this.dataObject.Release();
							}
							this.dataObject = (SCPKG_JOIN_TVOIP_ROOM_NTF)ProtocolObjectPool.Get(SCPKG_JOIN_TVOIP_ROOM_NTF.CLASS_ID);
						}
					}
					else if (!(this.dataObject is SCPKG_CREATE_TVOIP_ROOM_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CREATE_TVOIP_ROOM_NTF)ProtocolObjectPool.Get(SCPKG_CREATE_TVOIP_ROOM_NTF.CLASS_ID);
					}
				}
				else if (!(this.dataObject is SCPKG_ANTIDATA_SYN))
				{
					if (this.dataObject != null)
					{
						this.dataObject.Release();
					}
					this.dataObject = (SCPKG_ANTIDATA_SYN)ProtocolObjectPool.Get(SCPKG_ANTIDATA_SYN.CLASS_ID);
				}
			}
			else if (!(this.dataObject is CSPKG_ANTIDATA_REQ))
			{
				if (this.dataObject != null)
				{
					this.dataObject.Release();
				}
				this.dataObject = (CSPKG_ANTIDATA_REQ)ProtocolObjectPool.Get(CSPKG_ANTIDATA_REQ.CLASS_ID);
			}
		}

		private void select_4103_5201(long selector)
		{
			if (selector >= 4401L && selector <= 4414L)
			{
				switch ((int)(selector - 4401L))
				{
				case 0:
					if (!(this.dataObject is SCPKG_ACHIEVEMENT_INFO_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_ACHIEVEMENT_INFO_NTF)ProtocolObjectPool.Get(SCPKG_ACHIEVEMENT_INFO_NTF.CLASS_ID);
					}
					return;
				case 1:
					if (!(this.dataObject is SCPKG_ACHIEVEMENT_STATE_CHG_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_ACHIEVEMENT_STATE_CHG_NTF)ProtocolObjectPool.Get(SCPKG_ACHIEVEMENT_STATE_CHG_NTF.CLASS_ID);
					}
					return;
				case 2:
					if (!(this.dataObject is SCPKG_ACHIEVEMENT_DONE_DATA_CHG_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_ACHIEVEMENT_DONE_DATA_CHG_NTF)ProtocolObjectPool.Get(SCPKG_ACHIEVEMENT_DONE_DATA_CHG_NTF.CLASS_ID);
					}
					return;
				case 3:
					if (!(this.dataObject is CSPKG_GET_ACHIEVEMENT_REWARD_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_GET_ACHIEVEMENT_REWARD_REQ)ProtocolObjectPool.Get(CSPKG_GET_ACHIEVEMENT_REWARD_REQ.CLASS_ID);
					}
					return;
				case 4:
					if (!(this.dataObject is SCPKG_GET_ACHIEVEMENT_REWARD_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_GET_ACHIEVEMENT_REWARD_RSP)ProtocolObjectPool.Get(SCPKG_GET_ACHIEVEMENT_REWARD_RSP.CLASS_ID);
					}
					return;
				case 9:
					if (!(this.dataObject is CSPKG_GET_TROPHYLVL_REWARD_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_GET_TROPHYLVL_REWARD_REQ)ProtocolObjectPool.Get(CSPKG_GET_TROPHYLVL_REWARD_REQ.CLASS_ID);
					}
					return;
				case 10:
					if (!(this.dataObject is SCPKG_GET_TROPHYLVL_REWARD_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_GET_TROPHYLVL_REWARD_RSP)ProtocolObjectPool.Get(SCPKG_GET_TROPHYLVL_REWARD_RSP.CLASS_ID);
					}
					return;
				case 11:
					if (!(this.dataObject is SCPKG_NTF_TROPHYLVLUP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_NTF_TROPHYLVLUP)ProtocolObjectPool.Get(SCPKG_NTF_TROPHYLVLUP.CLASS_ID);
					}
					return;
				case 12:
					if (!(this.dataObject is CSPKG_REQ_ACHIEVE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_REQ_ACHIEVE)ProtocolObjectPool.Get(CSPKG_REQ_ACHIEVE.CLASS_ID);
					}
					return;
				case 13:
					if (!(this.dataObject is SCPKG_RSP_ACHIEVE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_RSP_ACHIEVE)ProtocolObjectPool.Get(SCPKG_RSP_ACHIEVE.CLASS_ID);
					}
					return;
				}
			}
			if (selector >= 5101L && selector <= 5111L)
			{
				switch ((int)(selector - 5101L))
				{
				case 0:
					if (!(this.dataObject is CSPKG_GETAWARDPOOL_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_GETAWARDPOOL_REQ)ProtocolObjectPool.Get(CSPKG_GETAWARDPOOL_REQ.CLASS_ID);
					}
					return;
				case 1:
					if (!(this.dataObject is SCPKG_GETAWARDPOOL_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_GETAWARDPOOL_RSP)ProtocolObjectPool.Get(SCPKG_GETAWARDPOOL_RSP.CLASS_ID);
					}
					return;
				case 2:
					if (!(this.dataObject is SCPKG_MATCHPOINT_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_MATCHPOINT_NTF)ProtocolObjectPool.Get(SCPKG_MATCHPOINT_NTF.CLASS_ID);
					}
					return;
				case 3:
					if (!(this.dataObject is CSPKG_BUY_MATCHTICKET_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_BUY_MATCHTICKET_REQ)ProtocolObjectPool.Get(CSPKG_BUY_MATCHTICKET_REQ.CLASS_ID);
					}
					return;
				case 4:
					if (!(this.dataObject is SCPKG_BUY_MATCHTICKET_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_BUY_MATCHTICKET_RSP)ProtocolObjectPool.Get(SCPKG_BUY_MATCHTICKET_RSP.CLASS_ID);
					}
					return;
				case 5:
					if (!(this.dataObject is CSPKG_GET_MATCHINFO_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_GET_MATCHINFO_REQ)ProtocolObjectPool.Get(CSPKG_GET_MATCHINFO_REQ.CLASS_ID);
					}
					return;
				case 6:
					if (!(this.dataObject is SCPKG_GET_MATCHINFO_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_GET_MATCHINFO_RSP)ProtocolObjectPool.Get(SCPKG_GET_MATCHINFO_RSP.CLASS_ID);
					}
					return;
				case 7:
					if (!(this.dataObject is CSPKG_GET_REWARDMATCH_INFO_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_GET_REWARDMATCH_INFO_REQ)ProtocolObjectPool.Get(CSPKG_GET_REWARDMATCH_INFO_REQ.CLASS_ID);
					}
					return;
				case 8:
					if (!(this.dataObject is SCPKG_GET_REWARDMATCH_INFO_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_GET_REWARDMATCH_INFO_RSP)ProtocolObjectPool.Get(SCPKG_GET_REWARDMATCH_INFO_RSP.CLASS_ID);
					}
					return;
				case 9:
					if (!(this.dataObject is CSPKG_REWARDMATCH_STATE_CHG_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_REWARDMATCH_STATE_CHG_REQ)ProtocolObjectPool.Get(CSPKG_REWARDMATCH_STATE_CHG_REQ.CLASS_ID);
					}
					return;
				case 10:
					if (!(this.dataObject is SCPKG_REWARDMATCH_INFO_CHG_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_REWARDMATCH_INFO_CHG_NTF)ProtocolObjectPool.Get(SCPKG_REWARDMATCH_INFO_CHG_NTF.CLASS_ID);
					}
					return;
				}
			}
			if (selector >= 4301L && selector <= 4310L)
			{
				switch ((int)(selector - 4301L))
				{
				case 0:
					if (!(this.dataObject is CSPKG_DIRECT_BUY_ITEM_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_DIRECT_BUY_ITEM_REQ)ProtocolObjectPool.Get(CSPKG_DIRECT_BUY_ITEM_REQ.CLASS_ID);
					}
					return;
				case 1:
					if (!(this.dataObject is SCPKG_DIRECT_BUY_ITEM_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_DIRECT_BUY_ITEM_RSP)ProtocolObjectPool.Get(SCPKG_DIRECT_BUY_ITEM_RSP.CLASS_ID);
					}
					return;
				case 2:
					if (!(this.dataObject is CSPKG_PVE_REVIVE_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_PVE_REVIVE_REQ)ProtocolObjectPool.Get(CSPKG_PVE_REVIVE_REQ.CLASS_ID);
					}
					return;
				case 3:
					if (!(this.dataObject is SCPKG_PVE_REVIVE_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_PVE_REVIVE_RSP)ProtocolObjectPool.Get(SCPKG_PVE_REVIVE_RSP.CLASS_ID);
					}
					return;
				case 4:
					if (!(this.dataObject is CSPKG_USER_COMPLAINT_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_USER_COMPLAINT_REQ)ProtocolObjectPool.Get(CSPKG_USER_COMPLAINT_REQ.CLASS_ID);
					}
					return;
				case 5:
					if (!(this.dataObject is SCPKG_USER_COMPLAINT_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_USER_COMPLAINT_RSP)ProtocolObjectPool.Get(SCPKG_USER_COMPLAINT_RSP.CLASS_ID);
					}
					return;
				case 6:
					if (!(this.dataObject is CSPKG_SHARE_TLOG_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_SHARE_TLOG_REQ)ProtocolObjectPool.Get(CSPKG_SHARE_TLOG_REQ.CLASS_ID);
					}
					return;
				case 7:
					if (!(this.dataObject is SCPKG_SHARE_TLOG_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_SHARE_TLOG_RSP)ProtocolObjectPool.Get(SCPKG_SHARE_TLOG_RSP.CLASS_ID);
					}
					return;
				case 8:
					if (!(this.dataObject is CSPKG_DYE_INBATTLE_NEWBIEBIT_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_DYE_INBATTLE_NEWBIEBIT_REQ)ProtocolObjectPool.Get(CSPKG_DYE_INBATTLE_NEWBIEBIT_REQ.CLASS_ID);
					}
					return;
				case 9:
					if (!(this.dataObject is SCPKG_DYE_INBATTLE_NEWBIEBIT_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_DYE_INBATTLE_NEWBIEBIT_RSP)ProtocolObjectPool.Get(SCPKG_DYE_INBATTLE_NEWBIEBIT_RSP.CLASS_ID);
					}
					return;
				}
			}
			if (selector >= 4600L && selector <= 4608L)
			{
				switch ((int)(selector - 4600L))
				{
				case 0:
					if (!(this.dataObject is SCPKG_GAME_VIP_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_GAME_VIP_NTF)ProtocolObjectPool.Get(SCPKG_GAME_VIP_NTF.CLASS_ID);
					}
					return;
				case 1:
					if (!(this.dataObject is CSPKG_HEADIMG_CHG_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_HEADIMG_CHG_REQ)ProtocolObjectPool.Get(CSPKG_HEADIMG_CHG_REQ.CLASS_ID);
					}
					return;
				case 2:
					if (!(this.dataObject is SCPKG_HEADIMG_CHG_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_HEADIMG_CHG_RSP)ProtocolObjectPool.Get(SCPKG_HEADIMG_CHG_RSP.CLASS_ID);
					}
					return;
				case 3:
					if (!(this.dataObject is CSPKG_HEADIMG_FLAGCLR_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_HEADIMG_FLAGCLR_REQ)ProtocolObjectPool.Get(CSPKG_HEADIMG_FLAGCLR_REQ.CLASS_ID);
					}
					return;
				case 4:
					if (!(this.dataObject is SCPKG_HEADIMG_FLAGCLR_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_HEADIMG_FLAGCLR_RSP)ProtocolObjectPool.Get(SCPKG_HEADIMG_FLAGCLR_RSP.CLASS_ID);
					}
					return;
				case 5:
					if (!(this.dataObject is SCPKG_NTF_HEADIMG_CHG))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_NTF_HEADIMG_CHG)ProtocolObjectPool.Get(SCPKG_NTF_HEADIMG_CHG.CLASS_ID);
					}
					return;
				case 6:
					if (!(this.dataObject is SCPKG_HEADIMG_LIST_SYNC))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_HEADIMG_LIST_SYNC)ProtocolObjectPool.Get(SCPKG_HEADIMG_LIST_SYNC.CLASS_ID);
					}
					return;
				case 7:
					if (!(this.dataObject is SCPKG_NTF_HEADIMG_ADD))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_NTF_HEADIMG_ADD)ProtocolObjectPool.Get(SCPKG_NTF_HEADIMG_ADD.CLASS_ID);
					}
					return;
				case 8:
					if (!(this.dataObject is SCPKG_NTF_HEADIMG_DEL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_NTF_HEADIMG_DEL)ProtocolObjectPool.Get(SCPKG_NTF_HEADIMG_DEL.CLASS_ID);
					}
					return;
				}
			}
			if (selector >= 4800L && selector <= 4804L)
			{
				switch ((int)(selector - 4800L))
				{
				case 0:
					if (!(this.dataObject is SCPKG_LUCKYDRAW_DATA_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_LUCKYDRAW_DATA_NTF)ProtocolObjectPool.Get(SCPKG_LUCKYDRAW_DATA_NTF.CLASS_ID);
					}
					return;
				case 1:
					if (!(this.dataObject is CSPKG_LUCKYDRAW_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_LUCKYDRAW_REQ)ProtocolObjectPool.Get(CSPKG_LUCKYDRAW_REQ.CLASS_ID);
					}
					return;
				case 2:
					if (!(this.dataObject is SCPKG_LUCKYDRAW_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_LUCKYDRAW_RSP)ProtocolObjectPool.Get(SCPKG_LUCKYDRAW_RSP.CLASS_ID);
					}
					return;
				case 3:
					if (!(this.dataObject is CSPKG_LUCKYDRAW_EXTERN_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_LUCKYDRAW_EXTERN_REQ)ProtocolObjectPool.Get(CSPKG_LUCKYDRAW_EXTERN_REQ.CLASS_ID);
					}
					return;
				case 4:
					if (!(this.dataObject is SCPKG_LUCKYDRAW_EXTERN_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_LUCKYDRAW_EXTERN_RSP)ProtocolObjectPool.Get(SCPKG_LUCKYDRAW_EXTERN_RSP.CLASS_ID);
					}
					return;
				}
			}
			if (selector >= 5000L && selector <= 5003L)
			{
				switch ((int)(selector - 5000L))
				{
				case 0:
					if (!(this.dataObject is CSPKG_CLT_PERFORMANCE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CLT_PERFORMANCE)ProtocolObjectPool.Get(CSPKG_CLT_PERFORMANCE.CLASS_ID);
					}
					return;
				case 2:
					if (!(this.dataObject is CSPKG_CLT_ACTION_STATISTICS))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CLT_ACTION_STATISTICS)ProtocolObjectPool.Get(CSPKG_CLT_ACTION_STATISTICS.CLASS_ID);
					}
					return;
				case 3:
					if (!(this.dataObject is SCPKG_ENTERTAINMENT_SYN_RAND_HERO_CNT))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_ENTERTAINMENT_SYN_RAND_HERO_CNT)ProtocolObjectPool.Get(SCPKG_ENTERTAINMENT_SYN_RAND_HERO_CNT.CLASS_ID);
					}
					return;
				}
			}
			if (selector >= 4900L && selector <= 4902L)
			{
				switch ((int)(selector - 4900L))
				{
				case 0:
					if (!(this.dataObject is CSPKG_SURRENDER_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_SURRENDER_REQ)ProtocolObjectPool.Get(CSPKG_SURRENDER_REQ.CLASS_ID);
					}
					return;
				case 1:
					if (!(this.dataObject is SCPKG_SURRENDER_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_SURRENDER_RSP)ProtocolObjectPool.Get(SCPKG_SURRENDER_RSP.CLASS_ID);
					}
					return;
				case 2:
					if (!(this.dataObject is SCPKG_SURRENDER_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_SURRENDER_NTF)ProtocolObjectPool.Get(SCPKG_SURRENDER_NTF.CLASS_ID);
					}
					return;
				}
			}
			if (selector != 4103L)
			{
				if (selector != 4104L)
				{
					if (selector != 4200L)
					{
						if (selector != 4201L)
						{
							if (selector != 4850L)
							{
								if (selector != 4851L)
								{
									if (selector != 4110L)
									{
										if (selector != 4500L)
										{
											if (selector != 5201L)
											{
												if (this.dataObject != null)
												{
													this.dataObject.Release();
													this.dataObject = null;
												}
											}
											else if (!(this.dataObject is CSPKG_SELFDEFINE_HEROEQUIP_CHG_REQ))
											{
												if (this.dataObject != null)
												{
													this.dataObject.Release();
												}
												this.dataObject = (CSPKG_SELFDEFINE_HEROEQUIP_CHG_REQ)ProtocolObjectPool.Get(CSPKG_SELFDEFINE_HEROEQUIP_CHG_REQ.CLASS_ID);
											}
										}
										else if (!(this.dataObject is SCPKG_DAILY_CHECK_DATA_NTF))
										{
											if (this.dataObject != null)
											{
												this.dataObject.Release();
											}
											this.dataObject = (SCPKG_DAILY_CHECK_DATA_NTF)ProtocolObjectPool.Get(SCPKG_DAILY_CHECK_DATA_NTF.CLASS_ID);
										}
									}
									else if (!(this.dataObject is SCPKG_FUNCTION_SWITCH_NTF))
									{
										if (this.dataObject != null)
										{
											this.dataObject.Release();
										}
										this.dataObject = (SCPKG_FUNCTION_SWITCH_NTF)ProtocolObjectPool.Get(SCPKG_FUNCTION_SWITCH_NTF.CLASS_ID);
									}
								}
								else if (!(this.dataObject is SCPKG_NTF_VOICESTATE))
								{
									if (this.dataObject != null)
									{
										this.dataObject.Release();
									}
									this.dataObject = (SCPKG_NTF_VOICESTATE)ProtocolObjectPool.Get(SCPKG_NTF_VOICESTATE.CLASS_ID);
								}
							}
							else if (!(this.dataObject is CSPKG_ACNT_VOICESTATE))
							{
								if (this.dataObject != null)
								{
									this.dataObject.Release();
								}
								this.dataObject = (CSPKG_ACNT_VOICESTATE)ProtocolObjectPool.Get(CSPKG_ACNT_VOICESTATE.CLASS_ID);
							}
						}
						else if (!(this.dataObject is SCPKG_QQVIPINFO_RSP))
						{
							if (this.dataObject != null)
							{
								this.dataObject.Release();
							}
							this.dataObject = (SCPKG_QQVIPINFO_RSP)ProtocolObjectPool.Get(SCPKG_QQVIPINFO_RSP.CLASS_ID);
						}
					}
					else if (!(this.dataObject is CSPKG_QQVIPINFO_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_QQVIPINFO_REQ)ProtocolObjectPool.Get(CSPKG_QQVIPINFO_REQ.CLASS_ID);
					}
				}
				else if (!(this.dataObject is SCPKG_NTF_SNS_NICKNAME))
				{
					if (this.dataObject != null)
					{
						this.dataObject.Release();
					}
					this.dataObject = (SCPKG_NTF_SNS_NICKNAME)ProtocolObjectPool.Get(SCPKG_NTF_SNS_NICKNAME.CLASS_ID);
				}
			}
			else if (!(this.dataObject is SCPKG_CMD_NTF_ACNT_SNSNAME))
			{
				if (this.dataObject != null)
				{
					this.dataObject.Release();
				}
				this.dataObject = (SCPKG_CMD_NTF_ACNT_SNSNAME)ProtocolObjectPool.Get(SCPKG_CMD_NTF_ACNT_SNSNAME.CLASS_ID);
			}
		}

		private void select_5202_5307(long selector)
		{
			if (selector >= 5202L && selector <= 5307L)
			{
				switch ((int)(selector - 5202L))
				{
				case 0:
					if (!(this.dataObject is SCPKG_SELFDEFINE_HEROEQUIP_CHG_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_SELFDEFINE_HEROEQUIP_CHG_RSP)ProtocolObjectPool.Get(SCPKG_SELFDEFINE_HEROEQUIP_CHG_RSP.CLASS_ID);
					}
					return;
				case 1:
					if (!(this.dataObject is CSPKG_RECOVER_SYSTEMEQUIP_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_RECOVER_SYSTEMEQUIP_REQ)ProtocolObjectPool.Get(CSPKG_RECOVER_SYSTEMEQUIP_REQ.CLASS_ID);
					}
					return;
				case 2:
					if (!(this.dataObject is SCPKG_RECOVER_SYSTEMEQUIP_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_RECOVER_SYSTEMEQUIP_RSP)ProtocolObjectPool.Get(SCPKG_RECOVER_SYSTEMEQUIP_RSP.CLASS_ID);
					}
					return;
				case 3:
					if (!(this.dataObject is SCPKG_MATCHTEAM_DESTROY_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_MATCHTEAM_DESTROY_NTF)ProtocolObjectPool.Get(SCPKG_MATCHTEAM_DESTROY_NTF.CLASS_ID);
					}
					return;
				case 4:
					if (!(this.dataObject is CSPKG_GET_ACNT_CREDIT_VALUE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_GET_ACNT_CREDIT_VALUE)ProtocolObjectPool.Get(CSPKG_GET_ACNT_CREDIT_VALUE.CLASS_ID);
					}
					return;
				case 5:
					if (!(this.dataObject is SCPKG_NTF_ACNT_CREDIT_VALUE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_NTF_ACNT_CREDIT_VALUE)ProtocolObjectPool.Get(SCPKG_NTF_ACNT_CREDIT_VALUE.CLASS_ID);
					}
					return;
				case 6:
					if (!(this.dataObject is CSPKG_JOINMULTIGAMEREQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_JOINMULTIGAMEREQ)ProtocolObjectPool.Get(CSPKG_JOINMULTIGAMEREQ.CLASS_ID);
					}
					return;
				case 8:
					if (!(this.dataObject is CSPKG_JOIN_TEAM_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_JOIN_TEAM_REQ)ProtocolObjectPool.Get(CSPKG_JOIN_TEAM_REQ.CLASS_ID);
					}
					return;
				case 9:
					if (!(this.dataObject is SCPKG_NTF_CUR_BAN_PICK_INFO))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_NTF_CUR_BAN_PICK_INFO)ProtocolObjectPool.Get(SCPKG_NTF_CUR_BAN_PICK_INFO.CLASS_ID);
					}
					return;
				case 10:
					if (!(this.dataObject is CSPKG_BAN_HERO_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_BAN_HERO_REQ)ProtocolObjectPool.Get(CSPKG_BAN_HERO_REQ.CLASS_ID);
					}
					return;
				case 11:
					if (!(this.dataObject is SCPKG_BAN_HERO_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_BAN_HERO_RSP)ProtocolObjectPool.Get(SCPKG_BAN_HERO_RSP.CLASS_ID);
					}
					return;
				case 12:
					if (!(this.dataObject is CSPKG_SWAP_HERO_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_SWAP_HERO_REQ)ProtocolObjectPool.Get(CSPKG_SWAP_HERO_REQ.CLASS_ID);
					}
					return;
				case 13:
					if (!(this.dataObject is SCPKG_NTF_SWAP_HERO))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_NTF_SWAP_HERO)ProtocolObjectPool.Get(SCPKG_NTF_SWAP_HERO.CLASS_ID);
					}
					return;
				case 14:
					if (!(this.dataObject is CSPKG_CONFIRM_SWAP_HERO_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CONFIRM_SWAP_HERO_REQ)ProtocolObjectPool.Get(CSPKG_CONFIRM_SWAP_HERO_REQ.CLASS_ID);
					}
					return;
				case 15:
					if (!(this.dataObject is SCPKG_NTF_CONFIRM_SWAP_HERO))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_NTF_CONFIRM_SWAP_HERO)ProtocolObjectPool.Get(SCPKG_NTF_CONFIRM_SWAP_HERO.CLASS_ID);
					}
					return;
				case 16:
					if (!(this.dataObject is CSPKG_OBSERVE_FRIEND_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_OBSERVE_FRIEND_REQ)ProtocolObjectPool.Get(CSPKG_OBSERVE_FRIEND_REQ.CLASS_ID);
					}
					return;
				case 17:
					if (!(this.dataObject is SCPKG_OBSERVE_FRIEND_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_OBSERVE_FRIEND_RSP)ProtocolObjectPool.Get(SCPKG_OBSERVE_FRIEND_RSP.CLASS_ID);
					}
					return;
				case 18:
					if (!(this.dataObject is CSPKG_OBSERVE_GREAT_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_OBSERVE_GREAT_REQ)ProtocolObjectPool.Get(CSPKG_OBSERVE_GREAT_REQ.CLASS_ID);
					}
					return;
				case 19:
					if (!(this.dataObject is SCPKG_OBSERVE_GREAT_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_OBSERVE_GREAT_RSP)ProtocolObjectPool.Get(SCPKG_OBSERVE_GREAT_RSP.CLASS_ID);
					}
					return;
				case 20:
					if (!(this.dataObject is CSPKG_GET_GREATMATCH_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_GET_GREATMATCH_REQ)ProtocolObjectPool.Get(CSPKG_GET_GREATMATCH_REQ.CLASS_ID);
					}
					return;
				case 21:
					if (!(this.dataObject is SCPKG_GET_GREATMATCH_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_GET_GREATMATCH_RSP)ProtocolObjectPool.Get(SCPKG_GET_GREATMATCH_RSP.CLASS_ID);
					}
					return;
				case 22:
					if (!(this.dataObject is CSPKG_SELFDEFINE_CHATINFO_CHG_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_SELFDEFINE_CHATINFO_CHG_REQ)ProtocolObjectPool.Get(CSPKG_SELFDEFINE_CHATINFO_CHG_REQ.CLASS_ID);
					}
					return;
				case 23:
					if (!(this.dataObject is SCPKG_SELFDEFINE_CHATINFO_CHG_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_SELFDEFINE_CHATINFO_CHG_RSP)ProtocolObjectPool.Get(SCPKG_SELFDEFINE_CHATINFO_CHG_RSP.CLASS_ID);
					}
					return;
				case 24:
					if (!(this.dataObject is CSPKG_CANCEL_SWAP_HERO_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CANCEL_SWAP_HERO_REQ)ProtocolObjectPool.Get(CSPKG_CANCEL_SWAP_HERO_REQ.CLASS_ID);
					}
					return;
				case 25:
					if (!(this.dataObject is SCPKG_CANCEL_SWAP_HERO_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CANCEL_SWAP_HERO_RSP)ProtocolObjectPool.Get(SCPKG_CANCEL_SWAP_HERO_RSP.CLASS_ID);
					}
					return;
				case 26:
					if (!(this.dataObject is CSPKG_GET_VIDEOFRAPS_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_GET_VIDEOFRAPS_REQ)ProtocolObjectPool.Get(CSPKG_GET_VIDEOFRAPS_REQ.CLASS_ID);
					}
					return;
				case 27:
					if (!(this.dataObject is SCPKG_GET_VIDEOFRAPS_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_GET_VIDEOFRAPS_RSP)ProtocolObjectPool.Get(SCPKG_GET_VIDEOFRAPS_RSP.CLASS_ID);
					}
					return;
				case 28:
					if (!(this.dataObject is CSPKG_QUITOBSERVE_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_QUITOBSERVE_REQ)ProtocolObjectPool.Get(CSPKG_QUITOBSERVE_REQ.CLASS_ID);
					}
					return;
				case 29:
					if (!(this.dataObject is SCPKG_QUITOBSERVE_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_QUITOBSERVE_RSP)ProtocolObjectPool.Get(SCPKG_QUITOBSERVE_RSP.CLASS_ID);
					}
					return;
				case 30:
					if (!(this.dataObject is CSPKG_ACNT_QUITSETTLEUI_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_ACNT_QUITSETTLEUI_REQ)ProtocolObjectPool.Get(CSPKG_ACNT_QUITSETTLEUI_REQ.CLASS_ID);
					}
					return;
				case 31:
					if (!(this.dataObject is CSPKG_GETFRIEND_GAMESTATE_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_GETFRIEND_GAMESTATE_REQ)ProtocolObjectPool.Get(CSPKG_GETFRIEND_GAMESTATE_REQ.CLASS_ID);
					}
					return;
				case 32:
					if (!(this.dataObject is CSPKG_WEAL_CONTENT_SHARE_DONE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_WEAL_CONTENT_SHARE_DONE)ProtocolObjectPool.Get(CSPKG_WEAL_CONTENT_SHARE_DONE.CLASS_ID);
					}
					return;
				case 33:
					if (!(this.dataObject is SCPKG_TRANSDATA_RENAME_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_TRANSDATA_RENAME_NTF)ProtocolObjectPool.Get(SCPKG_TRANSDATA_RENAME_NTF.CLASS_ID);
					}
					return;
				case 34:
					if (!(this.dataObject is CSPKG_TRANSDATA_RENAME_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_TRANSDATA_RENAME_REQ)ProtocolObjectPool.Get(CSPKG_TRANSDATA_RENAME_REQ.CLASS_ID);
					}
					return;
				case 35:
					if (!(this.dataObject is SCPKG_TRANSDATA_RENAME_RES))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_TRANSDATA_RENAME_RES)ProtocolObjectPool.Get(SCPKG_TRANSDATA_RENAME_RES.CLASS_ID);
					}
					return;
				case 36:
					if (!(this.dataObject is SCPKG_UPLOADCLTLOG_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_UPLOADCLTLOG_REQ)ProtocolObjectPool.Get(SCPKG_UPLOADCLTLOG_REQ.CLASS_ID);
					}
					return;
				case 37:
					if (!(this.dataObject is CSPKG_UPLOADCLTLOG_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_UPLOADCLTLOG_NTF)ProtocolObjectPool.Get(CSPKG_UPLOADCLTLOG_NTF.CLASS_ID);
					}
					return;
				case 38:
					if (!(this.dataObject is SCPKG_OBTIPS_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_OBTIPS_NTF)ProtocolObjectPool.Get(SCPKG_OBTIPS_NTF.CLASS_ID);
					}
					return;
				case 39:
					if (!(this.dataObject is SCPKG_ACNT_OLD_TYPE_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_ACNT_OLD_TYPE_NTF)ProtocolObjectPool.Get(SCPKG_ACNT_OLD_TYPE_NTF.CLASS_ID);
					}
					return;
				case 40:
					if (!(this.dataObject is CSPKG_ACNT_SET_OLD_TYPE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_ACNT_SET_OLD_TYPE)ProtocolObjectPool.Get(CSPKG_ACNT_SET_OLD_TYPE.CLASS_ID);
					}
					return;
				case 41:
					if (!(this.dataObject is CSPKG_PAUSE_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_PAUSE_REQ)ProtocolObjectPool.Get(CSPKG_PAUSE_REQ.CLASS_ID);
					}
					return;
				case 42:
					if (!(this.dataObject is SCPKG_PAUSE_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_PAUSE_RSP)ProtocolObjectPool.Get(SCPKG_PAUSE_RSP.CLASS_ID);
					}
					return;
				case 43:
					if (!(this.dataObject is CSPKG_SELECT_NEWBIE_HERO_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_SELECT_NEWBIE_HERO_REQ)ProtocolObjectPool.Get(CSPKG_SELECT_NEWBIE_HERO_REQ.CLASS_ID);
					}
					return;
				case 44:
					if (!(this.dataObject is SCPKG_ACNT_MOBA_INFO))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_ACNT_MOBA_INFO)ProtocolObjectPool.Get(SCPKG_ACNT_MOBA_INFO.CLASS_ID);
					}
					return;
				case 90:
					if (!(this.dataObject is SCPKG_GUILD_MATCH_SELFSIGNUPINFO_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_GUILD_MATCH_SELFSIGNUPINFO_RSP)ProtocolObjectPool.Get(SCPKG_GUILD_MATCH_SELFSIGNUPINFO_RSP.CLASS_ID);
					}
					return;
				case 91:
					if (!(this.dataObject is CSPKG_GUILD_MATCH_SIGNUPLIST_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_GUILD_MATCH_SIGNUPLIST_REQ)ProtocolObjectPool.Get(CSPKG_GUILD_MATCH_SIGNUPLIST_REQ.CLASS_ID);
					}
					return;
				case 92:
					if (!(this.dataObject is SCPKG_GUILD_MATCH_SIGNUPLIST_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_GUILD_MATCH_SIGNUPLIST_RSP)ProtocolObjectPool.Get(SCPKG_GUILD_MATCH_SIGNUPLIST_RSP.CLASS_ID);
					}
					return;
				case 93:
					if (!(this.dataObject is CSPKG_GUILD_MATCH_SIGNUP_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_GUILD_MATCH_SIGNUP_REQ)ProtocolObjectPool.Get(CSPKG_GUILD_MATCH_SIGNUP_REQ.CLASS_ID);
					}
					return;
				case 94:
					if (!(this.dataObject is SCPKG_GUILD_MATCH_SIGNUP_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_GUILD_MATCH_SIGNUP_RSP)ProtocolObjectPool.Get(SCPKG_GUILD_MATCH_SIGNUP_RSP.CLASS_ID);
					}
					return;
				case 95:
					if (!(this.dataObject is CSPKG_MOD_GUILD_MATCH_SIGNUP_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_MOD_GUILD_MATCH_SIGNUP_REQ)ProtocolObjectPool.Get(CSPKG_MOD_GUILD_MATCH_SIGNUP_REQ.CLASS_ID);
					}
					return;
				case 96:
					if (!(this.dataObject is SCPKG_MOD_GUILD_MATCH_SIGNUP_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_MOD_GUILD_MATCH_SIGNUP_RSP)ProtocolObjectPool.Get(SCPKG_MOD_GUILD_MATCH_SIGNUP_RSP.CLASS_ID);
					}
					return;
				case 97:
					if (!(this.dataObject is CSPKG_GUILD_MATCH_GETINVITE_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_GUILD_MATCH_GETINVITE_REQ)ProtocolObjectPool.Get(CSPKG_GUILD_MATCH_GETINVITE_REQ.CLASS_ID);
					}
					return;
				case 98:
					if (!(this.dataObject is SCPKG_GUILD_MATCH_GETINVITE_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_GUILD_MATCH_GETINVITE_RSP)ProtocolObjectPool.Get(SCPKG_GUILD_MATCH_GETINVITE_RSP.CLASS_ID);
					}
					return;
				case 99:
					if (!(this.dataObject is CSPKG_CHG_GUILD_MATCH_LEADER_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CHG_GUILD_MATCH_LEADER_REQ)ProtocolObjectPool.Get(CSPKG_CHG_GUILD_MATCH_LEADER_REQ.CLASS_ID);
					}
					return;
				case 100:
					if (!(this.dataObject is SCPKG_CHG_GUILD_MATCH_LEADER_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CHG_GUILD_MATCH_LEADER_NTF)ProtocolObjectPool.Get(SCPKG_CHG_GUILD_MATCH_LEADER_NTF.CLASS_ID);
					}
					return;
				case 101:
					if (!(this.dataObject is CSPKG_INVITE_GUILD_MATCH_MEMBER_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_INVITE_GUILD_MATCH_MEMBER_REQ)ProtocolObjectPool.Get(CSPKG_INVITE_GUILD_MATCH_MEMBER_REQ.CLASS_ID);
					}
					return;
				case 102:
					if (!(this.dataObject is SCPKG_INVITE_GUILD_MATCH_MEMBER_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_INVITE_GUILD_MATCH_MEMBER_NTF)ProtocolObjectPool.Get(SCPKG_INVITE_GUILD_MATCH_MEMBER_NTF.CLASS_ID);
					}
					return;
				case 103:
					if (!(this.dataObject is CSPKG_DEAL_GUILD_MATCH_MEMBER_INVITE))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_DEAL_GUILD_MATCH_MEMBER_INVITE)ProtocolObjectPool.Get(CSPKG_DEAL_GUILD_MATCH_MEMBER_INVITE.CLASS_ID);
					}
					return;
				case 104:
					if (!(this.dataObject is SCPKG_GUILD_MATCH_MEMBER_INVITE_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_GUILD_MATCH_MEMBER_INVITE_RSP)ProtocolObjectPool.Get(SCPKG_GUILD_MATCH_MEMBER_INVITE_RSP.CLASS_ID);
					}
					return;
				case 105:
					if (!(this.dataObject is CSPKG_KICK_GUILD_MATCH_MEMBER_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_KICK_GUILD_MATCH_MEMBER_REQ)ProtocolObjectPool.Get(CSPKG_KICK_GUILD_MATCH_MEMBER_REQ.CLASS_ID);
					}
					return;
				}
			}
			if (this.dataObject != null)
			{
				this.dataObject.Release();
				this.dataObject = null;
			}
		}

		private void select_5308_5611(long selector)
		{
			if (selector >= 5308L && selector <= 5334L)
			{
				switch ((int)(selector - 5308L))
				{
				case 0:
					if (!(this.dataObject is CSPKG_LEAVE_GUILD_MATCH_TEAM_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_LEAVE_GUILD_MATCH_TEAM_REQ)ProtocolObjectPool.Get(CSPKG_LEAVE_GUILD_MATCH_TEAM_REQ.CLASS_ID);
					}
					return;
				case 1:
					if (!(this.dataObject is SCPKG_GUILD_MATCH_MEMBER_CHG_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_GUILD_MATCH_MEMBER_CHG_NTF)ProtocolObjectPool.Get(SCPKG_GUILD_MATCH_MEMBER_CHG_NTF.CLASS_ID);
					}
					return;
				case 2:
					if (!(this.dataObject is CSPKG_START_GUILD_MATCH_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_START_GUILD_MATCH_REQ)ProtocolObjectPool.Get(CSPKG_START_GUILD_MATCH_REQ.CLASS_ID);
					}
					return;
				case 3:
					if (!(this.dataObject is CSPKG_SET_GUILD_MATCH_READY_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_SET_GUILD_MATCH_READY_REQ)ProtocolObjectPool.Get(CSPKG_SET_GUILD_MATCH_READY_REQ.CLASS_ID);
					}
					return;
				case 4:
					if (!(this.dataObject is SCPKG_SET_GUILD_MATCH_READY_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_SET_GUILD_MATCH_READY_RSP)ProtocolObjectPool.Get(SCPKG_SET_GUILD_MATCH_READY_RSP.CLASS_ID);
					}
					return;
				case 5:
					if (!(this.dataObject is SCPKG_SET_GUILD_MATCH_READY_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_SET_GUILD_MATCH_READY_NTF)ProtocolObjectPool.Get(SCPKG_SET_GUILD_MATCH_READY_NTF.CLASS_ID);
					}
					return;
				case 6:
					if (!(this.dataObject is SCPKG_GUILD_MATCH_SCORE_CHG_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_GUILD_MATCH_SCORE_CHG_NTF)ProtocolObjectPool.Get(SCPKG_GUILD_MATCH_SCORE_CHG_NTF.CLASS_ID);
					}
					return;
				case 7:
					if (!(this.dataObject is SCPKG_START_GUILD_MATCH_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_START_GUILD_MATCH_RSP)ProtocolObjectPool.Get(SCPKG_START_GUILD_MATCH_RSP.CLASS_ID);
					}
					return;
				case 8:
					if (!(this.dataObject is SCPKG_GUILD_MATCH_OB_INFO_CHG))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_GUILD_MATCH_OB_INFO_CHG)ProtocolObjectPool.Get(SCPKG_GUILD_MATCH_OB_INFO_CHG.CLASS_ID);
					}
					return;
				case 9:
					if (!(this.dataObject is CSPKG_OB_GUILD_MATCH_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_OB_GUILD_MATCH_REQ)ProtocolObjectPool.Get(CSPKG_OB_GUILD_MATCH_REQ.CLASS_ID);
					}
					return;
				case 10:
					if (!(this.dataObject is SCPKG_OB_GUILD_MATCH_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_OB_GUILD_MATCH_RSP)ProtocolObjectPool.Get(SCPKG_OB_GUILD_MATCH_RSP.CLASS_ID);
					}
					return;
				case 11:
					if (!(this.dataObject is CSPKG_GET_GUILD_MATCH_HISTORY_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_GET_GUILD_MATCH_HISTORY_REQ)ProtocolObjectPool.Get(CSPKG_GET_GUILD_MATCH_HISTORY_REQ.CLASS_ID);
					}
					return;
				case 12:
					if (!(this.dataObject is SCPKG_GET_GUILD_MATCH_HISTORY_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_GET_GUILD_MATCH_HISTORY_RSP)ProtocolObjectPool.Get(SCPKG_GET_GUILD_MATCH_HISTORY_RSP.CLASS_ID);
					}
					return;
				case 13:
					if (!(this.dataObject is SCPKG_CHG_GUILD_MATCH_LEADER_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CHG_GUILD_MATCH_LEADER_RSP)ProtocolObjectPool.Get(SCPKG_CHG_GUILD_MATCH_LEADER_RSP.CLASS_ID);
					}
					return;
				case 14:
					if (!(this.dataObject is CSPKG_GUILD_MATCH_REMIND_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_GUILD_MATCH_REMIND_REQ)ProtocolObjectPool.Get(CSPKG_GUILD_MATCH_REMIND_REQ.CLASS_ID);
					}
					return;
				case 15:
					if (!(this.dataObject is SCPKG_GUILD_MATCH_REMIND_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_GUILD_MATCH_REMIND_NTF)ProtocolObjectPool.Get(SCPKG_GUILD_MATCH_REMIND_NTF.CLASS_ID);
					}
					return;
				case 16:
					if (!(this.dataObject is CSPKG_GET_GUILD_MATCH_OB_CNT_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_GET_GUILD_MATCH_OB_CNT_REQ)ProtocolObjectPool.Get(CSPKG_GET_GUILD_MATCH_OB_CNT_REQ.CLASS_ID);
					}
					return;
				case 17:
					if (!(this.dataObject is SCPKG_GET_GUILD_MATCH_OB_CNT_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_GET_GUILD_MATCH_OB_CNT_RSP)ProtocolObjectPool.Get(SCPKG_GET_GUILD_MATCH_OB_CNT_RSP.CLASS_ID);
					}
					return;
				case 18:
					if (!(this.dataObject is SCPKG_NTF_SWITCHOFF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_NTF_SWITCHOFF)ProtocolObjectPool.Get(SCPKG_NTF_SWITCHOFF.CLASS_ID);
					}
					return;
				case 19:
					if (!(this.dataObject is CSPKG_EQUIPEVAL_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_EQUIPEVAL_REQ)ProtocolObjectPool.Get(CSPKG_EQUIPEVAL_REQ.CLASS_ID);
					}
					return;
				case 20:
					if (!(this.dataObject is SCPKG_EQUIPEVAL_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_EQUIPEVAL_RSP)ProtocolObjectPool.Get(SCPKG_EQUIPEVAL_RSP.CLASS_ID);
					}
					return;
				case 21:
					if (!(this.dataObject is CSPKG_GET_EQUIPEVAL_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_GET_EQUIPEVAL_REQ)ProtocolObjectPool.Get(CSPKG_GET_EQUIPEVAL_REQ.CLASS_ID);
					}
					return;
				case 22:
					if (!(this.dataObject is CSPKG_GET_EQUIPEVAL_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_GET_EQUIPEVAL_RSP)ProtocolObjectPool.Get(CSPKG_GET_EQUIPEVAL_RSP.CLASS_ID);
					}
					return;
				case 23:
					if (!(this.dataObject is SCPKG_GUILD_MATCH_WEEK_RESET_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_GUILD_MATCH_WEEK_RESET_NTF)ProtocolObjectPool.Get(SCPKG_GUILD_MATCH_WEEK_RESET_NTF.CLASS_ID);
					}
					return;
				case 24:
					if (!(this.dataObject is CSPKG_GET_LADDER_REWARD_SKIN_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_GET_LADDER_REWARD_SKIN_REQ)ProtocolObjectPool.Get(CSPKG_GET_LADDER_REWARD_SKIN_REQ.CLASS_ID);
					}
					return;
				case 25:
					if (!(this.dataObject is SCPKG_SERVER_PING_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_SERVER_PING_REQ)ProtocolObjectPool.Get(SCPKG_SERVER_PING_REQ.CLASS_ID);
					}
					return;
				case 26:
					if (!(this.dataObject is CSPKG_SERVER_PING_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_SERVER_PING_RSP)ProtocolObjectPool.Get(CSPKG_SERVER_PING_RSP.CLASS_ID);
					}
					return;
				}
			}
			if (selector >= 5402L && selector <= 5418L)
			{
				switch ((int)(selector - 5402L))
				{
				case 0:
					if (!(this.dataObject is CSPKG_APPLY_MASTER_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_APPLY_MASTER_REQ)ProtocolObjectPool.Get(CSPKG_APPLY_MASTER_REQ.CLASS_ID);
					}
					return;
				case 1:
					if (!(this.dataObject is SCPKG_APPLY_MASTER_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_APPLY_MASTER_RSP)ProtocolObjectPool.Get(SCPKG_APPLY_MASTER_RSP.CLASS_ID);
					}
					return;
				case 2:
					if (!(this.dataObject is CSPKG_CONFIRM_MASTER_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CONFIRM_MASTER_REQ)ProtocolObjectPool.Get(CSPKG_CONFIRM_MASTER_REQ.CLASS_ID);
					}
					return;
				case 3:
					if (!(this.dataObject is SCPKG_CONFIRM_MASTER_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CONFIRM_MASTER_RSP)ProtocolObjectPool.Get(SCPKG_CONFIRM_MASTER_RSP.CLASS_ID);
					}
					return;
				case 4:
					if (!(this.dataObject is CSPKG_REMOVE_MASTER_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_REMOVE_MASTER_REQ)ProtocolObjectPool.Get(CSPKG_REMOVE_MASTER_REQ.CLASS_ID);
					}
					return;
				case 5:
					if (!(this.dataObject is SCPKG_REMOVE_MASTER_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_REMOVE_MASTER_RSP)ProtocolObjectPool.Get(SCPKG_REMOVE_MASTER_RSP.CLASS_ID);
					}
					return;
				case 6:
					if (!(this.dataObject is SCPKG_MASTERREQ_LIST))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_MASTERREQ_LIST)ProtocolObjectPool.Get(SCPKG_MASTERREQ_LIST.CLASS_ID);
					}
					return;
				case 7:
					if (!(this.dataObject is SCPKG_MASTERREQ_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_MASTERREQ_NTF)ProtocolObjectPool.Get(SCPKG_MASTERREQ_NTF.CLASS_ID);
					}
					return;
				case 8:
					if (!(this.dataObject is SCPKG_MASTERSTUDENT_INFO))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_MASTERSTUDENT_INFO)ProtocolObjectPool.Get(SCPKG_MASTERSTUDENT_INFO.CLASS_ID);
					}
					return;
				case 9:
					if (!(this.dataObject is SCPKG_MASTERSTUDENT_ADD))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_MASTERSTUDENT_ADD)ProtocolObjectPool.Get(SCPKG_MASTERSTUDENT_ADD.CLASS_ID);
					}
					return;
				case 10:
					if (!(this.dataObject is SCPKG_MASTERSTUDENT_DEL))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_MASTERSTUDENT_DEL)ProtocolObjectPool.Get(SCPKG_MASTERSTUDENT_DEL.CLASS_ID);
					}
					return;
				case 11:
					if (!(this.dataObject is SCPKG_GRADUATE_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_GRADUATE_NTF)ProtocolObjectPool.Get(SCPKG_GRADUATE_NTF.CLASS_ID);
					}
					return;
				case 12:
					if (!(this.dataObject is SCPKG_MASTERACNTDATA_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_MASTERACNTDATA_NTF)ProtocolObjectPool.Get(SCPKG_MASTERACNTDATA_NTF.CLASS_ID);
					}
					return;
				case 13:
					if (!(this.dataObject is CSPKG_GETSTUDENTLIST_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_GETSTUDENTLIST_REQ)ProtocolObjectPool.Get(CSPKG_GETSTUDENTLIST_REQ.CLASS_ID);
					}
					return;
				case 14:
					if (!(this.dataObject is SCPKG_GETSTUDENTLIST_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_GETSTUDENTLIST_RSP)ProtocolObjectPool.Get(SCPKG_GETSTUDENTLIST_RSP.CLASS_ID);
					}
					return;
				case 15:
					if (!(this.dataObject is SCPKG_CMD_NTF_MASTERSTUDENT_LOGIN_STATUS))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CMD_NTF_MASTERSTUDENT_LOGIN_STATUS)ProtocolObjectPool.Get(SCPKG_CMD_NTF_MASTERSTUDENT_LOGIN_STATUS.CLASS_ID);
					}
					return;
				case 16:
					if (!(this.dataObject is SCPKG_MASTERSTUDENT_NOASKFOR_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_MASTERSTUDENT_NOASKFOR_NTF)ProtocolObjectPool.Get(SCPKG_MASTERSTUDENT_NOASKFOR_NTF.CLASS_ID);
					}
					return;
				}
			}
			if (selector >= 5600L && selector <= 5611L)
			{
				switch ((int)(selector - 5600L))
				{
				case 0:
					if (!(this.dataObject is CSPKG_CHG_USEDHEROEQUIP_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CHG_USEDHEROEQUIP_REQ)ProtocolObjectPool.Get(CSPKG_CHG_USEDHEROEQUIP_REQ.CLASS_ID);
					}
					return;
				case 1:
					if (!(this.dataObject is SCPKG_CHG_USEDHEROEQUIP_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CHG_USEDHEROEQUIP_RSP)ProtocolObjectPool.Get(SCPKG_CHG_USEDHEROEQUIP_RSP.CLASS_ID);
					}
					return;
				case 2:
					if (!(this.dataObject is CSPKG_CHG_HEROEQUIPNAME_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CHG_HEROEQUIPNAME_REQ)ProtocolObjectPool.Get(CSPKG_CHG_HEROEQUIPNAME_REQ.CLASS_ID);
					}
					return;
				case 3:
					if (!(this.dataObject is SCPKG_CHG_HEROEQUIPNAME_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CHG_HEROEQUIPNAME_RSP)ProtocolObjectPool.Get(SCPKG_CHG_HEROEQUIPNAME_RSP.CLASS_ID);
					}
					return;
				case 4:
					if (!(this.dataObject is CSPKG_OPERATE_USER_PRIVACY_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_OPERATE_USER_PRIVACY_REQ)ProtocolObjectPool.Get(CSPKG_OPERATE_USER_PRIVACY_REQ.CLASS_ID);
					}
					return;
				case 5:
					if (!(this.dataObject is SCPKG_OPERATE_USER_PRIVACY_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_OPERATE_USER_PRIVACY_RSP)ProtocolObjectPool.Get(SCPKG_OPERATE_USER_PRIVACY_RSP.CLASS_ID);
					}
					return;
				case 6:
					if (!(this.dataObject is SCPKG_CLTUPLOADDATA_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CLTUPLOADDATA_REQ)ProtocolObjectPool.Get(SCPKG_CLTUPLOADDATA_REQ.CLASS_ID);
					}
					return;
				case 7:
					if (!(this.dataObject is CSPKG_CLTUPLOADDATA_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CLTUPLOADDATA_RSP)ProtocolObjectPool.Get(CSPKG_CLTUPLOADDATA_RSP.CLASS_ID);
					}
					return;
				case 8:
					if (!(this.dataObject is SCPKG_NTF_SVRRESTART))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_NTF_SVRRESTART)ProtocolObjectPool.Get(SCPKG_NTF_SVRRESTART.CLASS_ID);
					}
					return;
				case 9:
					if (!(this.dataObject is CSPKG_CHG_FRIEND_CARD_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CHG_FRIEND_CARD_REQ)ProtocolObjectPool.Get(CSPKG_CHG_FRIEND_CARD_REQ.CLASS_ID);
					}
					return;
				case 10:
					if (!(this.dataObject is SCPKG_CHG_FRIEND_CARD_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CHG_FRIEND_CARD_RSP)ProtocolObjectPool.Get(SCPKG_CHG_FRIEND_CARD_RSP.CLASS_ID);
					}
					return;
				case 11:
					if (!(this.dataObject is CSPKG_CHG_OTHERSTATE_BIT_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_CHG_OTHERSTATE_BIT_REQ)ProtocolObjectPool.Get(CSPKG_CHG_OTHERSTATE_BIT_REQ.CLASS_ID);
					}
					return;
				}
			}
			if (selector >= 5500L && selector <= 5503L)
			{
				switch ((int)(selector - 5500L))
				{
				case 0:
					if (!(this.dataObject is CSPKG_SHOWRECENTUSEDHERO_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_SHOWRECENTUSEDHERO_REQ)ProtocolObjectPool.Get(CSPKG_SHOWRECENTUSEDHERO_REQ.CLASS_ID);
					}
					return;
				case 1:
					if (!(this.dataObject is SCPKG_PROFITLIMIT_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_PROFITLIMIT_NTF)ProtocolObjectPool.Get(SCPKG_PROFITLIMIT_NTF.CLASS_ID);
					}
					return;
				case 2:
					if (!(this.dataObject is SCPKG_CHGNAMECD_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CHGNAMECD_NTF)ProtocolObjectPool.Get(SCPKG_CHGNAMECD_NTF.CLASS_ID);
					}
					return;
				case 3:
					if (!(this.dataObject is SCPKG_PVPBAN_NTF))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_PVPBAN_NTF)ProtocolObjectPool.Get(SCPKG_PVPBAN_NTF.CLASS_ID);
					}
					return;
				}
			}
			if (this.dataObject != null)
			{
				this.dataObject.Release();
				this.dataObject = null;
			}
		}

		private void select_5612_5616(long selector)
		{
			if (selector >= 5612L && selector <= 5616L)
			{
				switch ((int)(selector - 5612L))
				{
				case 0:
					if (!(this.dataObject is SCPKG_CHG_OTHERSTATE_BIT_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_CHG_OTHERSTATE_BIT_RSP)ProtocolObjectPool.Get(SCPKG_CHG_OTHERSTATE_BIT_RSP.CLASS_ID);
					}
					return;
				case 1:
					if (!(this.dataObject is CSPKG_RESERVE_MSG_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_RESERVE_MSG_REQ)ProtocolObjectPool.Get(CSPKG_RESERVE_MSG_REQ.CLASS_ID);
					}
					return;
				case 2:
					if (!(this.dataObject is SCPKG_RESERVE_MSG_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_RESERVE_MSG_RSP)ProtocolObjectPool.Get(SCPKG_RESERVE_MSG_RSP.CLASS_ID);
					}
					return;
				case 3:
					if (!(this.dataObject is SCPKG_RESERVE_MSG_REQ))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (SCPKG_RESERVE_MSG_REQ)ProtocolObjectPool.Get(SCPKG_RESERVE_MSG_REQ.CLASS_ID);
					}
					return;
				case 4:
					if (!(this.dataObject is CSPKG_RESERVE_MSG_RSP))
					{
						if (this.dataObject != null)
						{
							this.dataObject.Release();
						}
						this.dataObject = (CSPKG_RESERVE_MSG_RSP)ProtocolObjectPool.Get(CSPKG_RESERVE_MSG_RSP.CLASS_ID);
					}
					return;
				}
			}
			if (this.dataObject != null)
			{
				this.dataObject.Release();
				this.dataObject = null;
			}
		}

		public override int GetClassID()
		{
			return CSPkgBody.CLASS_ID;
		}

		public override void OnRelease()
		{
			if (this.dataObject != null)
			{
				this.dataObject.Release();
				this.dataObject = null;
			}
		}
	}
}
