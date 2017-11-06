using Apollo;
using Assets.Scripts.Framework;
using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using CSProtocol;
using MiniJSON;
using ResData;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TGASys : MonoSingleton<TGASys>
{
	private bool m_bInstall;

	private bool m_bStart;

	protected override void Init()
	{
		base.Init();
		Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.GLOBAL_SERVER_TO_CLIENT_CFG_READY, new Action(this.InitTGAConfig));
	}

	public void InitTGAConfig()
	{
		this.m_bInstall = false;
		if (GameDataMgr.svr2CltCfgDict != null && GameDataMgr.svr2CltCfgDict.ContainsKey(42u))
		{
			this.InitSys();
		}
	}

	public void UnInitSys()
	{
		this.m_bInstall = false;
	}

	private void InitSys()
	{
		try
		{
			ApolloAccountInfo accountInfo = Singleton<ApolloHelper>.GetInstance().GetAccountInfo(false);
			string text = string.Empty;
			string text2 = string.Empty;
			if (accountInfo != null)
			{
				ApolloToken token = accountInfo.GetToken(ApolloTokenType.Access);
				if (token != null)
				{
					text2 = token.Value;
				}
				text = accountInfo.OpenId;
			}
			int num = 1;
			if (ApolloConfig.platform == ApolloPlatform.QQ)
			{
				num = 1;
			}
			else if (ApolloConfig.platform == ApolloPlatform.Wechat)
			{
				num = 2;
			}
			else if (ApolloConfig.platform == ApolloPlatform.Guest)
			{
				num = 3;
			}
			string text3 = string.Empty;
			string text4 = string.Empty;
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			if (masterRoleInfo != null)
			{
				text3 = masterRoleInfo.Name;
				text4 = masterRoleInfo.HeadUrl;
			}
			string jsonString = string.Format("{{\"appid\" : \"{0}\",\"token\" : \"{1}\", \"accountType\":\"{2}\",\"areaid\":\"{3}\",\"openid\":\"{4}\",\"nikeName\":\"{5}\",\"avatarUrl\":\"{6}\",\"gameVersion\":\"{7}\" ,\"gameUid\":\"{8}\" ,\"gameCallObjName\":\"{9}\",\"gameCallObjMethd\":\"{10}\"}}", new object[]
			{
				ApolloConfig.GetAppID(),
				text2,
				num,
				MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID,
				text,
				text3,
				text4,
				CVersion.GetAppVersion(),
				masterRoleInfo.playerUllUID,
				"BootObj/TGASys",
				"OnCallBack"
			});
			TGALive.init(jsonString);
			this.m_bInstall = true;
		}
		catch (Exception ex)
		{
			Debug.Log("TGA" + ex.ToString());
		}
	}

	public bool Start()
	{
		bool flag = false;
		try
		{
			if (this.m_bInstall)
			{
				flag = TGALive.available();
				if (flag)
				{
					string token = string.Empty;
					ApolloAccountInfo accountInfo = Singleton<ApolloHelper>.GetInstance().GetAccountInfo(false);
					if (accountInfo != null)
					{
						ApolloToken token2 = accountInfo.GetToken(ApolloTokenType.Access);
						if (token2 != null)
						{
							token = token2.Value;
						}
					}
					TGALive.start(token, 1);
					this.m_bStart = true;
				}
			}
			else
			{
				flag = false;
			}
		}
		catch (Exception ex)
		{
			Debug.Log("TGA" + ex.ToString());
			flag = false;
		}
		return flag;
	}

	private void SendMsgChangeState(byte state)
	{
		CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(5611u);
		cSPkg.stPkgData.stChgOtherStatebBitReq.bChgType = state;
		cSPkg.stPkgData.stChgOtherStatebBitReq.dwOtherStateBits = 1u;
		Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
	}

	private void OnCallBack(string json)
	{
		Debug.Log("TGA call back " + json);
		Dictionary<string, object> dictionary = Json.Deserialize(json) as Dictionary<string, object>;
		if (dictionary != null && dictionary.ContainsKey("type"))
		{
			string text = dictionary.get_Item("type").ToString();
			if (text == "2")
			{
				if (this.m_bStart)
				{
					Debug.Log("TGA  update net");
				}
			}
			else
			{
				this.m_bStart = false;
				Debug.Log("TGA  update sendmsg");
			}
		}
	}

	public void battleInvitation(SCPKG_INVITE_JOIN_GAME_REQ info)
	{
		if (!this.m_bInstall)
		{
			return;
		}
		if (!this.m_bStart)
		{
			return;
		}
		try
		{
			string text = CUIUtility.RemoveEmoji(StringHelper.UTF8BytesToString(ref info.stInviterInfo.szName));
			ulong ullUid = info.stInviterInfo.ullUid;
			string text2 = string.Empty;
			ulong num = 0uL;
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			if (masterRoleInfo != null)
			{
				text2 = masterRoleInfo.Name;
				num = masterRoleInfo.playerUllUID;
			}
			string text3 = string.Empty;
			if (info.bInviteType == 1)
			{
				ResDT_LevelCommonInfo pvpMapCommonInfo = CLevelCfgLogicManager.GetPvpMapCommonInfo(info.stInviteDetail.stRoomDetail.bMapType, info.stInviteDetail.stRoomDetail.dwMapId);
				if (pvpMapCommonInfo != null)
				{
					text3 = Singleton<CTextManager>.instance.GetText("Invite_Map_Desc", new string[]
					{
						((int)(pvpMapCommonInfo.bMaxAcntNum / 2)).ToString(),
						((int)(pvpMapCommonInfo.bMaxAcntNum / 2)).ToString(),
						Utility.UTF8Convert(pvpMapCommonInfo.szName)
					});
				}
			}
			else if (info.bInviteType == 2)
			{
				ResDT_LevelCommonInfo pvpMapCommonInfo2 = CLevelCfgLogicManager.GetPvpMapCommonInfo(info.stInviteDetail.stTeamDetail.bMapType, info.stInviteDetail.stTeamDetail.dwMapId);
				if (pvpMapCommonInfo2 != null)
				{
					text3 = Singleton<CTextManager>.instance.GetText("Invite_Map_Desc", new string[]
					{
						((int)(pvpMapCommonInfo2.bMaxAcntNum / 2)).ToString(),
						((int)(pvpMapCommonInfo2.bMaxAcntNum / 2)).ToString(),
						Utility.UTF8Convert(pvpMapCommonInfo2.szName)
					});
				}
			}
			string jsonString = string.Format("{{ \"user_id\" : \"{0}\",\"user_name\" : \" {1} \", \"friend_id\":\" {2} \",\"friend_name\":\" {3} \",\"battle_mode\":\" {4} \"}}", new object[]
			{
				num,
				text2,
				ullUid,
				text,
				text3
			});
			TGALive.battleInvitation(jsonString);
		}
		catch (Exception ex)
		{
			Debug.Log("TGA" + ex.ToString());
		}
	}
}
