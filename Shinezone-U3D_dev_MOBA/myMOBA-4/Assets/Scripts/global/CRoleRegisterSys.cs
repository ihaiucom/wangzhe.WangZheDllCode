using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using System.Text;
using UnityEngine;

[MessageHandlerClass]
public class CRoleRegisterSys : Singleton<CRoleRegisterSys>
{
	public static readonly string s_videoBgFormPath = "UGUI/Form/System/RoleCreate/Form_VideoBg.prefab";

	public static readonly string s_roleCreateFormPath = "UGUI/Form/System/RoleCreate/Form_RoleCreate.prefab";

	public static readonly string s_gameDifficultSelectFormPath = "UGUI/Form/System/RoleCreate/Form_GameDifficultSelect.prefab";

	public static readonly string s_heroTypeSelectFormPath = "UGUI/Form/System/RoleCreate/Form_HeroTypeSelect.prefab";

	private System.Random m_random = new System.Random();

	public override void Init()
	{
		Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.ROLE_CREATE, new CUIEventManager.OnUIEventHandler(this.OnRoleCreate));
		Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.ROLE_CREATE_RANDOM, new CUIEventManager.OnUIEventHandler(this.OnRandomName));
		Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.GAME_DIFF_SELECT, new CUIEventManager.OnUIEventHandler(this.OnGameDifSelect));
		Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.GAME_DIFF_CONFIRM, new CUIEventManager.OnUIEventHandler(this.OnGameDifConfirm));
		Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.ROLE_CREATE_TIMER_CHANGE, new CUIEventManager.OnUIEventHandler(this.OnRoleCreateTimerChange));
		Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.ROLE_HeroType_Select, new CUIEventManager.OnUIEventHandler(this.OnHeroTypeSelect));
		Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.ROLE_HeroType_Confirm, new CUIEventManager.OnUIEventHandler(this.OnHeroTypeConfirm));
		base.Init();
	}

	public override void UnInit()
	{
		Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.ROLE_CREATE, new CUIEventManager.OnUIEventHandler(this.OnRoleCreate));
		Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.ROLE_CREATE_RANDOM, new CUIEventManager.OnUIEventHandler(this.OnRandomName));
		Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.GAME_DIFF_SELECT, new CUIEventManager.OnUIEventHandler(this.OnGameDifSelect));
		Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.GAME_DIFF_CONFIRM, new CUIEventManager.OnUIEventHandler(this.OnGameDifConfirm));
		Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.ROLE_CREATE_TIMER_CHANGE, new CUIEventManager.OnUIEventHandler(this.OnRoleCreateTimerChange));
		Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.ROLE_HeroType_Select, new CUIEventManager.OnUIEventHandler(this.OnHeroTypeSelect));
		Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.ROLE_HeroType_Confirm, new CUIEventManager.OnUIEventHandler(this.OnHeroTypeConfirm));
		base.UnInit();
	}

	public void OpenRoleCreateForm()
	{
		Debug.Log("OpenRoleCreateForm...");
		Singleton<CUIManager>.GetInstance().OpenForm(CRoleRegisterSys.s_videoBgFormPath, false, true);
		Singleton<CUIManager>.GetInstance().OpenForm(CRoleRegisterSys.s_roleCreateFormPath, false, true);
	}

	public void CloseRoleCreateForm()
	{
		Singleton<CUIManager>.GetInstance().CloseForm(CRoleRegisterSys.s_videoBgFormPath);
		Singleton<CUIManager>.GetInstance().CloseForm(CRoleRegisterSys.s_roleCreateFormPath);
	}

	public void SetRoleCreateFormVisible(bool bVisible)
	{
		CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CRoleRegisterSys.s_roleCreateFormPath);
		if (form != null)
		{
			form.gameObject.CustomSetActive(bVisible);
		}
	}

	private void OnRoleCreate(CUIEvent cuiEvent)
	{
		string roleName = CRoleRegisterView.RoleName;
		switch (Utility.CheckRoleName(roleName))
		{
		case Utility.NameResult.Vaild:
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1008u);
			cSPkg.stPkgData.stAcntRegisterReq.dwHeadId = 2u;
			cSPkg.stPkgData.stAcntRegisterReq.szUserName = Encoding.UTF8.GetBytes(roleName);
			cSPkg.stPkgData.stAcntRegisterReq.iRegChannel = Singleton<ApolloHelper>.GetInstance().GetRegisterChannelId();
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
			break;
		}
		case Utility.NameResult.Null:
			CRoleRegisterView.RoleName = string.Empty;
			Singleton<CUIManager>.instance.OpenTips("RoleRegister_InputName", true, 1.5f, null, new object[0]);
			break;
		case Utility.NameResult.OutOfLength:
			Singleton<CUIManager>.instance.OpenTips("RoleRegister_NameOutOfLength", true, 1.5f, null, new object[0]);
			break;
		case Utility.NameResult.InVaildChar:
			Singleton<CUIManager>.instance.OpenTips("RoleRegister_NameInvaildChar", true, 1.5f, null, new object[0]);
			break;
		}
	}

	public void ShowErrorCode(int errorCode)
	{
		if (errorCode == 6)
		{
			Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("RoleRegister_NameExists"), false, 1.5f, null, new object[0]);
		}
	}

	public void ShowErrorCode(byte[] szName)
	{
		string text = Utility.UTF8Convert(szName);
		string roleName = CRoleRegisterView.RoleName;
		string arg = string.Empty;
		int num = Math.Max(text.Length, roleName.Length);
		for (int i = 0; i < num; i++)
		{
			if (!text[i].Equals(roleName[i]))
			{
				arg = string.Format("{0}{1}", arg, roleName[i]);
			}
		}
		Singleton<CUIManager>.GetInstance().OpenMessageBox(string.Format(Singleton<CTextManager>.GetInstance().GetText("Register_Name_Invalid_Words_2"), arg), false);
	}

	public void OnRandomName(CUIEvent cuiEvent)
	{
		string randomName = this.GetRandomName();
		CRoleRegisterView.RoleName = randomName;
	}

	public void OpenGameDifSelectForm()
	{
		CRoleRegisterView.OpenGameDifSelectForm();
	}

	public void CloseGameDifSelectForm()
	{
		CRoleRegisterView.CloseGameDifSelectForm();
	}

	public void OnGameDifSelect(CUIEvent cuiEvent)
	{
		CRoleRegisterView.SetGameDifficult(cuiEvent.m_eventParams.tag);
	}

	public void OnGameDifConfirm(CUIEvent cuiEvent)
	{
		CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(2608u);
		cSPkg.stPkgData.stSetAcntNewbieTypeReq.bAcntNewbieType = (byte)cuiEvent.m_eventParams.tag;
		Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		LobbyLogic.ReqStartGuideLevel11(false, 0u);
	}

	public void OpenHeroTypeSelectForm()
	{
		CRoleRegisterView.OpenHeroTypeSelectForm();
		CRoleRegisterView.RefreshRecommendTips();
	}

	public void OnHeroTypeSelect(CUIEvent uiEvent)
	{
		CRoleRegisterView.SetHeroType(uiEvent.m_eventParams.tagUInt);
	}

	public void OnHeroTypeConfirm(CUIEvent uiEvent)
	{
		CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
		if (masterRoleInfo != null && masterRoleInfo.acntMobaInfo.iSelectedHeroType == 0)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(5245u);
			cSPkg.stPkgData.stSelectNewbieHeroReq.iHeroType = (int)uiEvent.m_eventParams.tagUInt;
			if (Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false))
			{
				masterRoleInfo.acntMobaInfo.iSelectedHeroType = (int)uiEvent.m_eventParams.tagUInt;
			}
		}
		LobbyLogic.ReqStartGuideLevel11(false, uiEvent.m_eventParams.tagUInt);
	}

	public void RefreshRecommendTips()
	{
		CRoleRegisterView.RefreshRecommendTips();
	}

	private void OnRoleCreateTimerChange(CUIEvent cuiEvent)
	{
	}

	public string GetRandomName()
	{
		ResRobotName resRobotName = null;
		ResRobotSubNameC resRobotSubNameC = null;
		ResRobotSubNameA resRobotSubNameA = null;
		ResRobotSubNameB resRobotSubNameB = null;
		while (resRobotName == null)
		{
			uint num = (uint)this.m_random.Next(0, GameDataMgr.robotName.count);
			resRobotName = GameDataMgr.robotName.GetDataByKey(num + 1u);
		}
		while (resRobotSubNameC == null)
		{
			uint num2 = (uint)this.m_random.Next(0, GameDataMgr.robotSubNameC.count);
			resRobotSubNameC = GameDataMgr.robotSubNameC.GetDataByKey(num2 + 1u);
		}
		while (resRobotSubNameA == null)
		{
			uint num3 = (uint)this.m_random.Next(0, GameDataMgr.robotSubNameA.count);
			resRobotSubNameA = GameDataMgr.robotSubNameA.GetDataByKey(num3 + 1u);
		}
		while (resRobotSubNameB == null)
		{
			uint num4 = (uint)this.m_random.Next(0, GameDataMgr.robotSubNameB.count);
			resRobotSubNameB = GameDataMgr.robotSubNameB.GetDataByKey(num4 + 1u);
		}
		return string.Format("{0}{1}{2}{3}", new object[]
		{
			Utility.UTF8Convert(resRobotName.szName),
			Utility.UTF8Convert(resRobotSubNameC.szName),
			Utility.UTF8Convert(resRobotSubNameA.szName),
			Utility.UTF8Convert(resRobotSubNameB.szName)
		});
	}

	[MessageHandler(1007)]
	public static void OnNtfRegister(CSPkg msg)
	{
		Singleton<CUIManager>.instance.CloseSendMsgAlert();
		Debug.Log("receive message , id = CSProtocolMacros.SCID_NTF_ACNT_REGISTER ...");
		Singleton<GameStateCtrl>.GetInstance().GotoState("CreateRoleState");
	}

	[MessageHandler(1009)]
	public static void OnRegisterRes(CSPkg msg)
	{
		Singleton<CUIManager>.instance.CloseSendMsgAlert();
		if (msg.stPkgData.stAcntRegisterRes.bErrCode != 0)
		{
			Singleton<CRoleRegisterSys>.instance.ShowErrorCode((int)msg.stPkgData.stAcntRegisterRes.bErrCode);
		}
		Debug.Log("receive message , id = CSProtocolMacros.SCID_ACNT_REGISTER_RES , err code = " + msg.stPkgData.stAcntRegisterRes.bErrCode.ToString());
	}

	[MessageHandler(2609)]
	public static void OnGameDifSelect(CSPkg msg)
	{
	}

	[MessageHandler(4103)]
	public static void OnAcntSnsNameNtf(CSPkg msg)
	{
		if (string.IsNullOrEmpty(CRoleRegisterView.RoleName))
		{
			CRoleRegisterView.RoleName = Utility.UTF8Convert(msg.stPkgData.stNtfAcntSnsName.szNickName);
		}
	}
}
