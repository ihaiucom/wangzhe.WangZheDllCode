using Assets.Scripts.Framework;
using Assets.Scripts.UI;
using CSProtocol;
using System;

namespace Assets.Scripts.GameSystem
{
	[MessageHandlerClass]
	public class FriendRelationNetCore
	{
		public static void Send_INTIMACY_RELATION_REQUEST(ulong ulluid, uint worldID, COM_INTIMACY_STATE state, COM_INTIMACY_RELATION_CHG_TYPE chgType)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1360u);
			cSPkg.stPkgData.stIntimacyRelationRequestReq.stUin.ullUid = ulluid;
			cSPkg.stPkgData.stIntimacyRelationRequestReq.stUin.dwLogicWorldId = worldID;
			cSPkg.stPkgData.stIntimacyRelationRequestReq.bIntimacyState = (byte)state;
			cSPkg.stPkgData.stIntimacyRelationRequestReq.bRelationChgType = (byte)chgType;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		[MessageHandler(1361)]
		public static void On_Send_INTIMACY_RELATION_REQUEST(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			SCPKG_CMD_INTIMACY_RELATION_REQUEST stIntimacyRelationRequestRsp = msg.stPkgData.stIntimacyRelationRequestRsp;
			if (stIntimacyRelationRequestRsp.dwResult == 0u)
			{
				string strContent = string.Empty;
				COM_INTIMACY_STATE state = COM_INTIMACY_STATE.COM_INTIMACY_STATE_NULL;
				if (stIntimacyRelationRequestRsp.bRelationChgType == 1)
				{
					if (IntimacyRelationViewUT.IsRelaState(stIntimacyRelationRequestRsp.bIntimacyState))
					{
						state = IntimacyRelationViewUT.GetConfirmState(stIntimacyRelationRequestRsp.bIntimacyState);
						RelationConfig relaTextCfg = Singleton<CFriendContoller>.instance.model.FRData.GetRelaTextCfg(stIntimacyRelationRequestRsp.bIntimacyState);
						strContent = relaTextCfg.IntimRela_Tips_SendRequestSuccess;
					}
				}
				else if (stIntimacyRelationRequestRsp.bRelationChgType == 2 && IntimacyRelationViewUT.IsRelaState(stIntimacyRelationRequestRsp.bIntimacyState))
				{
					state = IntimacyRelationViewUT.GetDenyState(stIntimacyRelationRequestRsp.bIntimacyState);
					RelationConfig relaTextCfg2 = Singleton<CFriendContoller>.instance.model.FRData.GetRelaTextCfg(stIntimacyRelationRequestRsp.bIntimacyState);
					strContent = relaTextCfg2.IntimRela_Tips_SendDelSuccess;
				}
				Singleton<CUIManager>.GetInstance().OpenTips(strContent, false, 1.5f, null, new object[0]);
				CFriendRelationship.FRData.Add(stIntimacyRelationRequestRsp.stUin.ullUid, stIntimacyRelationRequestRsp.stUin.dwLogicWorldId, state, (COM_INTIMACY_RELATION_CHG_TYPE)stIntimacyRelationRequestRsp.bRelationChgType, 0u, false);
			}
			else
			{
				Singleton<CUIManager>.GetInstance().OpenTips(UT.ErrorCode_String(stIntimacyRelationRequestRsp.dwResult), false, 1.5f, null, new object[0]);
			}
		}

		[MessageHandler(1362)]
		public static void On_NTF_INTIMACY_RELATION_REQUEST(CSPkg msg)
		{
			SCPKG_CMD_NTF_INTIMACY_RELATION_REQUEST stNtfIntimacyRelationRequest = msg.stPkgData.stNtfIntimacyRelationRequest;
			if (stNtfIntimacyRelationRequest == null)
			{
				return;
			}
			COM_INTIMACY_STATE cOM_INTIMACY_STATE = COM_INTIMACY_STATE.COM_INTIMACY_STATE_NULL;
			if (stNtfIntimacyRelationRequest.bRelationChgType == 1)
			{
				if (IntimacyRelationViewUT.IsRelaState(stNtfIntimacyRelationRequest.bIntimacyState))
				{
					cOM_INTIMACY_STATE = IntimacyRelationViewUT.GetConfirmState(stNtfIntimacyRelationRequest.bIntimacyState);
				}
			}
			else if (stNtfIntimacyRelationRequest.bRelationChgType == 2 && IntimacyRelationViewUT.IsRelaState(stNtfIntimacyRelationRequest.bIntimacyState))
			{
				cOM_INTIMACY_STATE = IntimacyRelationViewUT.GetDenyState(stNtfIntimacyRelationRequest.bIntimacyState);
			}
			if (cOM_INTIMACY_STATE != COM_INTIMACY_STATE.COM_INTIMACY_STATE_NULL)
			{
				CFriendRelationship.FRData.Add(stNtfIntimacyRelationRequest.stUin.ullUid, stNtfIntimacyRelationRequest.stUin.dwLogicWorldId, cOM_INTIMACY_STATE, (COM_INTIMACY_RELATION_CHG_TYPE)stNtfIntimacyRelationRequest.bRelationChgType, 0u, true);
			}
		}

		public static void Send_CHG_INTIMACY_CONFIRM(ulong ulluid, uint worldID, COM_INTIMACY_STATE value, COM_INTIMACY_RELATION_CHG_TYPE type)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1363u);
			cSPkg.stPkgData.stChgIntimacyConfirmReq.stUin.ullUid = ulluid;
			cSPkg.stPkgData.stChgIntimacyConfirmReq.stUin.dwLogicWorldId = worldID;
			cSPkg.stPkgData.stChgIntimacyConfirmReq.bRelationChgType = (byte)type;
			cSPkg.stPkgData.stChgIntimacyConfirmReq.bIntimacyState = (byte)value;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		[MessageHandler(1364)]
		public static void On_Send_CHG_INTIMACY_CONFIRM(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			SCPKG_CMD_CHG_INTIMACY_CONFIRM stChgIntimacyConfirmRsp = msg.stPkgData.stChgIntimacyConfirmRsp;
			if (stChgIntimacyConfirmRsp.dwResult == 0u)
			{
				if (stChgIntimacyConfirmRsp.bRelationChgType == 1 && IntimacyRelationViewUT.IsRelaState(stChgIntimacyConfirmRsp.bIntimacyState))
				{
					CFriendRelationship.FRData.Add(stChgIntimacyConfirmRsp.stUin.ullUid, stChgIntimacyConfirmRsp.stUin.dwLogicWorldId, (COM_INTIMACY_STATE)stChgIntimacyConfirmRsp.bIntimacyState, (COM_INTIMACY_RELATION_CHG_TYPE)stChgIntimacyConfirmRsp.bRelationChgType, 0u, false);
				}
				if (stChgIntimacyConfirmRsp.bRelationChgType == 2 && IntimacyRelationViewUT.IsRelaState(stChgIntimacyConfirmRsp.bIntimacyState))
				{
					CFriendRelationship.FRData.Add(stChgIntimacyConfirmRsp.stUin.ullUid, stChgIntimacyConfirmRsp.stUin.dwLogicWorldId, COM_INTIMACY_STATE.COM_INTIMACY_STATE_VALUE_FULL, (COM_INTIMACY_RELATION_CHG_TYPE)stChgIntimacyConfirmRsp.bRelationChgType, stChgIntimacyConfirmRsp.dwTerminateTime, false);
				}
			}
			else
			{
				Singleton<CUIManager>.GetInstance().OpenTips(UT.ErrorCode_String(stChgIntimacyConfirmRsp.dwResult), false, 1.5f, null, new object[0]);
				COMDT_ACNT_UNIQ stUin = stChgIntimacyConfirmRsp.stUin;
				if (stUin != null)
				{
					FriendRelationNetCore.ProcessByErrorCode(stChgIntimacyConfirmRsp.dwResult, stUin.ullUid, stUin.dwLogicWorldId);
				}
			}
		}

		public static void ProcessByErrorCode(uint errorCode, ulong ulluid, uint worldID)
		{
			if (errorCode == 0u)
			{
				return;
			}
			if (errorCode == 191u || errorCode == 193u || errorCode == 188u || errorCode == 189u || errorCode == 189u || errorCode == 101u || errorCode == 189u)
			{
				CFR cfr = CFriendRelationship.FRData.GetCfr(ulluid, worldID);
				if (cfr != null)
				{
					cfr.state = COM_INTIMACY_STATE.COM_INTIMACY_STATE_VALUE_FULL;
				}
			}
		}

		[MessageHandler(1365)]
		public static void On_NTF_CHG_INTIMACY_CONFIRM(CSPkg msg)
		{
			SCPKG_CMD_NTF_CHG_INTIMACY_CONFIRM stNtfChgIntimacyConfirm = msg.stPkgData.stNtfChgIntimacyConfirm;
			if (stNtfChgIntimacyConfirm == null)
			{
				return;
			}
			if (stNtfChgIntimacyConfirm.bRelationChgType == 1 && IntimacyRelationViewUT.IsRelaState(stNtfChgIntimacyConfirm.bIntimacyState))
			{
				CFriendRelationship.FRData.Add(stNtfChgIntimacyConfirm.stUin.ullUid, stNtfChgIntimacyConfirm.stUin.dwLogicWorldId, (COM_INTIMACY_STATE)stNtfChgIntimacyConfirm.bIntimacyState, (COM_INTIMACY_RELATION_CHG_TYPE)stNtfChgIntimacyConfirm.bRelationChgType, 0u, false);
			}
			if (stNtfChgIntimacyConfirm.bRelationChgType == 2 && IntimacyRelationViewUT.IsRelaState(stNtfChgIntimacyConfirm.bIntimacyState))
			{
				CFriendRelationship.FRData.Add(stNtfChgIntimacyConfirm.stUin.ullUid, stNtfChgIntimacyConfirm.stUin.dwLogicWorldId, COM_INTIMACY_STATE.COM_INTIMACY_STATE_VALUE_FULL, (COM_INTIMACY_RELATION_CHG_TYPE)stNtfChgIntimacyConfirm.bRelationChgType, stNtfChgIntimacyConfirm.dwTerminateTime, false);
			}
		}

		public static void Send_CHG_INTIMACY_DENY(ulong ulluid, uint worldID, COM_INTIMACY_STATE state, COM_INTIMACY_RELATION_CHG_TYPE type)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1366u);
			cSPkg.stPkgData.stChgIntimacyDenyReq.stUin.ullUid = ulluid;
			cSPkg.stPkgData.stChgIntimacyDenyReq.stUin.dwLogicWorldId = worldID;
			cSPkg.stPkgData.stChgIntimacyDenyReq.bRelationChgType = (byte)type;
			cSPkg.stPkgData.stChgIntimacyDenyReq.bIntimacyState = (byte)state;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		[MessageHandler(1367)]
		public static void On_Send_CHG_INTIMACY_DENY(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			SCPKG_CMD_CHG_INTIMACY_DENY stChgIntimacyDenyRsp = msg.stPkgData.stChgIntimacyDenyRsp;
			if (stChgIntimacyDenyRsp.dwResult != 0u)
			{
				Singleton<CUIManager>.GetInstance().OpenTips(UT.ErrorCode_String(stChgIntimacyDenyRsp.dwResult), false, 1.5f, null, new object[0]);
			}
		}

		[MessageHandler(1368)]
		public static void On_NTF_CHG_INTIMACY_DENY(CSPkg msg)
		{
			SCPKG_CMD_NTF_CHG_INTIMACY_DENY stNtfChgIntimacyDeny = msg.stPkgData.stNtfChgIntimacyDeny;
			if (stNtfChgIntimacyDeny == null)
			{
				return;
			}
			COMDT_FRIEND_INFO gameOrSnsFriend = Singleton<CFriendContoller>.instance.model.GetGameOrSnsFriend(stNtfChgIntimacyDeny.stUin.ullUid, stNtfChgIntimacyDeny.stUin.dwLogicWorldId);
			string text = string.Empty;
			if (stNtfChgIntimacyDeny.bRelationChgType == 1)
			{
				string text2 = string.Empty;
				RelationConfig relaTextCfg = CFriendRelationship.FRData.GetRelaTextCfg(stNtfChgIntimacyDeny.bIntimacyState);
				if (relaTextCfg != null)
				{
					text2 = relaTextCfg.IntimRela_Tips_DenyYourRequest;
				}
				if (!string.IsNullOrEmpty(text2) && gameOrSnsFriend != null)
				{
					text = string.Format(text2, UT.Bytes2String(gameOrSnsFriend.szUserName));
					CFriendRelationship.FRData.Add(stNtfChgIntimacyDeny.stUin.ullUid, stNtfChgIntimacyDeny.stUin.dwLogicWorldId, COM_INTIMACY_STATE.COM_INTIMACY_STATE_VALUE_FULL, COM_INTIMACY_RELATION_CHG_TYPE.COM_INTIMACY_RELATION_NULL, 0u, false);
				}
			}
			if (stNtfChgIntimacyDeny.bRelationChgType == 2)
			{
				if (!IntimacyRelationViewUT.IsRelaState(stNtfChgIntimacyDeny.bIntimacyState))
				{
					return;
				}
				RelationConfig relaTextCfg2 = CFriendRelationship.FRData.GetRelaTextCfg(stNtfChgIntimacyDeny.bIntimacyState);
				if (relaTextCfg2 == null)
				{
					return;
				}
				if (gameOrSnsFriend != null)
				{
					text = string.Format(relaTextCfg2.IntimRela_Tips_DenyYourDel, UT.Bytes2String(gameOrSnsFriend.szUserName));
				}
				CFriendRelationship.FRData.Add(stNtfChgIntimacyDeny.stUin.ullUid, stNtfChgIntimacyDeny.stUin.dwLogicWorldId, (COM_INTIMACY_STATE)stNtfChgIntimacyDeny.bIntimacyState, COM_INTIMACY_RELATION_CHG_TYPE.COM_INTIMACY_RELATION_NULL, 0u, false);
			}
			if (string.IsNullOrEmpty(text))
			{
				Singleton<CUIManager>.GetInstance().OpenTips(text, false, 1.5f, null, new object[0]);
			}
		}

		public static void Send_CHG_INTIMACYPRIORITY(COM_INTIMACY_STATE type)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1369u);
			cSPkg.stPkgData.stChgIntimacyPriorityReq.bIntimacyState = (byte)type;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
			Singleton<CFriendContoller>.GetInstance().model.FRData.SetFirstChoiseState(type);
		}
	}
}
