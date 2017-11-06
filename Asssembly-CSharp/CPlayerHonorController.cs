using Assets.Scripts.Framework;
using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[MessageHandlerClass]
public class CPlayerHonorController : Singleton<CPlayerHonorController>
{
	private List<COMDT_HONORINFO> m_honorInfoList;

	public override void Init()
	{
		base.Init();
		this.m_honorInfoList = new List<COMDT_HONORINFO>();
	}

	public override void UnInit()
	{
		base.UnInit();
		this.m_honorInfoList = null;
		Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Player_Info_Honor_Item_Enable, new CUIEventManager.OnUIEventHandler(this.OnHonorItemEnable));
		Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Player_Info_Honor_Select_Change, new CUIEventManager.OnUIEventHandler(this.OnHonorSelectChange));
		Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Player_Info_Honor_Chosen, new CUIEventManager.OnUIEventHandler(this.OnHonorChosen));
		Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.PlayerInfoSystem_Tab_Change, new Action(this.OnPlayerInfoTabChange));
	}

	public bool Loaded(CUIFormScript form)
	{
		if (form == null)
		{
			return false;
		}
		GameObject widget = form.GetWidget(9);
		if (widget == null)
		{
			return false;
		}
		GameObject x = Utility.FindChild(widget, "pnlHonorInfo");
		return !(x == null);
	}

	public void Load(CUIFormScript form)
	{
		if (form == null)
		{
			return;
		}
		CUICommonSystem.LoadUIPrefab("UGUI/Form/System/Player/HonorInfo", "pnlHonorInfo", form.GetWidget(9), form);
	}

	public void Draw(CUIFormScript form)
	{
		if (form == null)
		{
			return;
		}
		GameObject widget = form.GetWidget(9);
		if (widget == null)
		{
			return;
		}
		Utility.FindChild(widget, "pnlHonorInfo").CustomSetActive(true);
		GameObject gameObject = Utility.FindChild(widget, "pnlHonorInfo/pnlContainer/HonorList");
		if (gameObject == null)
		{
			return;
		}
		CUIListScript component = gameObject.GetComponent<CUIListScript>();
		if (component == null)
		{
			return;
		}
		Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Player_Info_Honor_Item_Enable, new CUIEventManager.OnUIEventHandler(this.OnHonorItemEnable));
		Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Player_Info_Honor_Select_Change, new CUIEventManager.OnUIEventHandler(this.OnHonorSelectChange));
		Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Player_Info_Honor_Chosen, new CUIEventManager.OnUIEventHandler(this.OnHonorChosen));
		Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Player_Info_Honor_Item_Enable, new CUIEventManager.OnUIEventHandler(this.OnHonorItemEnable));
		Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Player_Info_Honor_Select_Change, new CUIEventManager.OnUIEventHandler(this.OnHonorSelectChange));
		Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Player_Info_Honor_Chosen, new CUIEventManager.OnUIEventHandler(this.OnHonorChosen));
		this.m_honorInfoList.Clear();
		CPlayerProfile profile = Singleton<CPlayerInfoSystem>.GetInstance().GetProfile();
		Dictionary<int, COMDT_HONORINFO> honorDic = profile.GetHonorDic();
		int selectedHonorId = profile.GetSelectedHonorId();
		Dictionary<int, COMDT_HONORINFO>.Enumerator enumerator = honorDic.GetEnumerator();
		while (enumerator.MoveNext())
		{
			KeyValuePair<int, COMDT_HONORINFO> current = enumerator.get_Current();
			COMDT_HONORINFO value = current.get_Value();
			if (value != null)
			{
				this.m_honorInfoList.Add(value);
			}
		}
		if (this.m_honorInfoList != null)
		{
			this.m_honorInfoList.Sort(delegate(COMDT_HONORINFO l, COMDT_HONORINFO r)
			{
				if (l == null)
				{
					return 1;
				}
				if (r == null)
				{
					return -1;
				}
				return l.iHonorID.CompareTo(r.iHonorID);
			});
		}
		component.SetElementAmount(this.m_honorInfoList.get_Count());
		component.SelectElement(-1, false);
		COMDT_HONORINFO honorInfo = null;
		honorDic.TryGetValue(selectedHonorId, ref honorInfo);
		this.UpdateSelectedHonor(form, honorInfo);
	}

	private void SetHonorImage(Transform imgTransform, COMDT_HONORINFO honorInfo)
	{
		string honorImagePath = this.GetHonorImagePath(honorInfo.iHonorID, honorInfo.iHonorLevel);
		Image component = imgTransform.GetComponent<Image>();
		component.SetSprite(CUIUtility.GetSpritePrefeb(honorImagePath, false, false), false);
	}

	private void SetHonorAssitImage(Transform imgTransform, COMDT_HONORINFO honorInfo, CUIFormScript form)
	{
		if (imgTransform != null)
		{
			Image component = imgTransform.GetComponent<Image>();
			if (component == null)
			{
				return;
			}
			switch (honorInfo.iHonorID)
			{
			case 1:
			{
				string prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Pvp_Settle_Large_Dir, "Img_Icon_Red_Mvp");
				component.SetSprite(prefabPath, form, true, false, false, false);
				break;
			}
			case 2:
			{
				string prefabPath2 = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Pvp_Settle_Large_Dir, "HurtMost");
				component.SetSprite(prefabPath2, form, true, false, false, false);
				break;
			}
			case 3:
			{
				string prefabPath3 = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Pvp_Settle_Large_Dir, "KillMost");
				component.SetSprite(prefabPath3, form, true, false, false, false);
				break;
			}
			case 4:
			{
				string prefabPath4 = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Pvp_Settle_Large_Dir, "MostMoney");
				component.SetSprite(prefabPath4, form, true, false, false, false);
				break;
			}
			case 5:
			{
				string prefabPath5 = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Pvp_Settle_Large_Dir, "AsssistMost");
				component.SetSprite(prefabPath5, form, true, false, false, false);
				break;
			}
			case 6:
			{
				string prefabPath6 = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Pvp_Settle_Large_Dir, "HurtTakenMost");
				component.SetSprite(prefabPath6, form, true, false, false, false);
				break;
			}
			}
		}
	}

	private void SetHonorName(Transform labelTransform, COMDT_HONORINFO honorInfo)
	{
		if (labelTransform != null)
		{
			CTextManager instance = Singleton<CTextManager>.GetInstance();
			Text component = labelTransform.GetComponent<Text>();
			if (component != null)
			{
				switch (honorInfo.iHonorID)
				{
				case 1:
					component.set_text(instance.GetText("Player_Info_Honor_Name_MVP"));
					break;
				case 2:
					component.set_text(instance.GetText("Player_Info_Honor_Name_MAXDAMAGETOHERO"));
					break;
				case 3:
					component.set_text(instance.GetText("Player_Info_Honor_Name_MAXKILL"));
					break;
				case 4:
					component.set_text(instance.GetText("Player_Info_Honor_Name_MAXMONEY"));
					break;
				case 5:
					component.set_text(instance.GetText("Player_Info_Honor_Name_MAXASSIS"));
					break;
				case 6:
					component.set_text(instance.GetText("Player_Info_Honor_Name_MAXRECVDAMAGE"));
					break;
				default:
					component.set_text(string.Empty);
					break;
				}
			}
		}
	}

	private void SetHonorPoint(Transform cntTransform, COMDT_HONORINFO honorInfo)
	{
		if (cntTransform != null)
		{
			Text component = cntTransform.GetComponent<Text>();
			if (component != null)
			{
				ResHonor dataByKey = GameDataMgr.resHonor.GetDataByKey((long)honorInfo.iHonorID);
				if (dataByKey == null)
				{
					component.set_text(string.Empty);
				}
				else
				{
					component.set_text(this.GetHonorProgress(honorInfo.iHonorPoint, dataByKey));
				}
			}
		}
	}

	private void SetHonorDesc(Transform descTransform, COMDT_HONORINFO honorInfo)
	{
		if (descTransform != null)
		{
			Text component = descTransform.GetComponent<Text>();
			if (component != null)
			{
				ResHonor dataByKey = GameDataMgr.resHonor.GetDataByKey((long)honorInfo.iHonorID);
				if (dataByKey == null)
				{
					component.set_text(string.Empty);
				}
				else
				{
					component.set_text(dataByKey.szDesc);
				}
			}
		}
	}

	private void SetHonorStatus(GameObject chosenGo, COMDT_HONORINFO honorInfo)
	{
		if (honorInfo == null)
		{
			chosenGo.CustomSetActive(false);
			return;
		}
		CPlayerProfile profile = Singleton<CPlayerInfoSystem>.GetInstance().GetProfile();
		int selectedHonorId = profile.GetSelectedHonorId();
		if (honorInfo.iHonorID == selectedHonorId)
		{
			chosenGo.CustomSetActive(true);
		}
		else
		{
			chosenGo.CustomSetActive(false);
		}
	}

	private string GetHonorProgress(int point, ResHonor honorCfg)
	{
		string result = string.Empty;
		if (honorCfg == null)
		{
			return result;
		}
		for (int i = honorCfg.astHonorLevel.Length - 1; i >= 0; i--)
		{
			if (point < honorCfg.astHonorLevel[i].iMaxPoint)
			{
				result = string.Format("{0}/{1}", point, honorCfg.astHonorLevel[i].iMaxPoint);
			}
			else
			{
				if (i != honorCfg.astHonorLevel.Length - 1)
				{
					return result;
				}
				result = string.Format("{0}", point);
			}
		}
		return result;
	}

	public string GetHonorImagePath(int id, int level)
	{
		ResHonor dataByKey = GameDataMgr.resHonor.GetDataByKey((long)id);
		if (dataByKey == null)
		{
			return string.Format("{0}{1}{2}", CUIUtility.s_Sprite_System_Honor_Dir, "Img_Honor_", 0);
		}
		if (level < 0 || level > dataByKey.astHonorLevel.Length)
		{
			return null;
		}
		if (level == 0)
		{
			return string.Format("{0}{1}{2}", CUIUtility.s_Sprite_System_Honor_Dir, "Img_Honor_", 0);
		}
		return string.Format("{0}{1}{2}{3}{4}", new object[]
		{
			CUIUtility.s_Sprite_System_Honor_Dir,
			"Img_Honor_",
			id,
			"_",
			level
		});
	}

	public void UpdateSelectedHonor(CUIFormScript form, COMDT_HONORINFO honorInfo)
	{
		CTextManager instance = Singleton<CTextManager>.GetInstance();
		string text = instance.GetText("Player_Info_Honor_Btn_Using");
		string text2 = instance.GetText("Player_Info_Honor_Btn_Use");
		string text3 = instance.GetText("Player_Info_Honor_Btn_Browse");
		GameObject widget = form.GetWidget(9);
		if (widget == null)
		{
			return;
		}
		Transform transform = form.transform.Find("pnlBg/pnlBody/pnlHonorInfo/pnlContainer/SelectedHonor/Button");
		Button button = (transform == null) ? null : transform.GetComponent<Button>();
		GameObject gameObject = Utility.FindChild(widget, "pnlHonorInfo/pnlContainer/SelectedHonor/CurrentHonor/normal");
		GameObject gameObject2 = Utility.FindChild(widget, "pnlHonorInfo/pnlContainer/SelectedHonor/CurrentHonor/label");
		GameObject gameObject3 = Utility.FindChild(widget, "pnlHonorInfo/pnlContainer/SelectedHonor/CurrentHonor/cnt");
		GameObject gameObject4 = Utility.FindChild(widget, "pnlHonorInfo/pnlContainer/SelectedHonor/Text");
		CPlayerProfile profile = Singleton<CPlayerInfoSystem>.GetInstance().GetProfile();
		int selectedHonorId = profile.GetSelectedHonorId();
		CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
		bool flag = true;
		if (masterRoleInfo != null && masterRoleInfo.playerUllUID != profile.m_uuid)
		{
			flag = false;
		}
		if (honorInfo == null)
		{
			honorInfo = new COMDT_HONORINFO();
			if (button != null)
			{
				CUICommonSystem.SetButtonEnableWithShader(button, false, true);
				CUICommonSystem.SetButtonName(transform.gameObject, text2);
			}
			if (gameObject != null)
			{
				this.SetHonorImage(gameObject.transform, honorInfo);
			}
			if (gameObject2 != null)
			{
				this.SetHonorName(gameObject2.transform, honorInfo);
			}
			if (gameObject3 != null)
			{
				this.SetHonorPoint(gameObject3.transform, honorInfo);
			}
			if (gameObject4 != null)
			{
				this.SetHonorDesc(gameObject4.transform, honorInfo);
			}
			if (!flag && button != null)
			{
				CUICommonSystem.SetButtonEnableWithShader(button, false, true);
				CUICommonSystem.SetButtonName(transform.gameObject, text3);
				return;
			}
			return;
		}
		else
		{
			if (gameObject != null)
			{
				this.SetHonorImage(gameObject.transform, honorInfo);
			}
			if (gameObject2 != null)
			{
				this.SetHonorName(gameObject2.transform, honorInfo);
			}
			if (gameObject3 != null)
			{
				this.SetHonorPoint(gameObject3.transform, honorInfo);
			}
			if (gameObject4 != null)
			{
				this.SetHonorDesc(gameObject4.transform, honorInfo);
			}
			if (!flag && button != null)
			{
				CUICommonSystem.SetButtonEnableWithShader(button, false, true);
				CUICommonSystem.SetButtonName(transform.gameObject, text3);
				return;
			}
			if (honorInfo.iHonorID == selectedHonorId && honorInfo.iHonorID != 0)
			{
				if (button != null)
				{
					CUICommonSystem.SetButtonEnableWithShader(button, false, true);
					CUICommonSystem.SetButtonName(transform.gameObject, text);
				}
			}
			else if (honorInfo.iHonorLevel <= 0)
			{
				if (button != null)
				{
					CUICommonSystem.SetButtonEnableWithShader(button, false, true);
					CUICommonSystem.SetButtonName(transform.gameObject, text2);
				}
			}
			else if (button != null)
			{
				CUICommonSystem.SetButtonEnableWithShader(button, true, true);
				CUICommonSystem.SetButtonName(transform.gameObject, text2);
			}
			return;
		}
	}

	private void OnHonorItemEnable(CUIEvent uiEvent)
	{
		int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
		if (srcWidgetIndexInBelongedList < 0 || srcWidgetIndexInBelongedList >= this.m_honorInfoList.get_Count())
		{
			return;
		}
		COMDT_HONORINFO cOMDT_HONORINFO = this.m_honorInfoList.get_Item(srcWidgetIndexInBelongedList);
		if (cOMDT_HONORINFO == null)
		{
			return;
		}
		ResHonor dataByKey = GameDataMgr.resHonor.GetDataByKey((long)cOMDT_HONORINFO.iHonorID);
		if (dataByKey == null)
		{
			return;
		}
		if (cOMDT_HONORINFO.iHonorLevel < 0 || cOMDT_HONORINFO.iHonorLevel > dataByKey.astHonorLevel.Length)
		{
			return;
		}
		CUIListElementScript cUIListElementScript = uiEvent.m_srcWidgetScript as CUIListElementScript;
		if (cUIListElementScript == null)
		{
			return;
		}
		GameObject widget = cUIListElementScript.GetWidget(0);
		GameObject widget2 = cUIListElementScript.GetWidget(1);
		GameObject widget3 = cUIListElementScript.GetWidget(2);
		GameObject widget4 = cUIListElementScript.GetWidget(3);
		GameObject widget5 = cUIListElementScript.GetWidget(4);
		if (widget != null)
		{
			this.SetHonorImage(widget.transform, cOMDT_HONORINFO);
		}
		if (widget2 != null)
		{
			this.SetHonorAssitImage(widget2.transform, cOMDT_HONORINFO, uiEvent.m_srcFormScript);
		}
		if (widget3 != null)
		{
			this.SetHonorPoint(widget3.transform, cOMDT_HONORINFO);
		}
		this.SetHonorStatus(widget5, cOMDT_HONORINFO);
		widget4.CustomSetActive(false);
	}

	private void OnHonorSelectChange(CUIEvent uiEvent)
	{
		CUIListScript cUIListScript = uiEvent.m_srcWidgetScript as CUIListScript;
		if (cUIListScript == null)
		{
			return;
		}
		int selectedIndex = cUIListScript.GetSelectedIndex();
		if (uiEvent.m_srcFormScript == null || selectedIndex < 0 || selectedIndex >= this.m_honorInfoList.get_Count())
		{
			return;
		}
		COMDT_HONORINFO cOMDT_HONORINFO = this.m_honorInfoList.get_Item(selectedIndex);
		if (cOMDT_HONORINFO == null)
		{
			return;
		}
		this.UpdateSelectedHonor(uiEvent.m_srcFormScript, cOMDT_HONORINFO);
	}

	private void OnHonorChosen(CUIEvent uiEvent)
	{
		GameObject widget = uiEvent.m_srcFormScript.GetWidget(9);
		if (widget == null)
		{
			return;
		}
		GameObject gameObject = Utility.FindChild(widget, "pnlHonorInfo/pnlContainer/HonorList");
		if (gameObject == null)
		{
			return;
		}
		CUIListScript component = gameObject.GetComponent<CUIListScript>();
		if (component == null)
		{
			return;
		}
		int selectedIndex = component.GetSelectedIndex();
		if (uiEvent.m_srcFormScript == null || selectedIndex < 0 || selectedIndex >= this.m_honorInfoList.get_Count())
		{
			return;
		}
		COMDT_HONORINFO cOMDT_HONORINFO = this.m_honorInfoList.get_Item(selectedIndex);
		if (cOMDT_HONORINFO == null)
		{
			return;
		}
		if (cOMDT_HONORINFO.iHonorLevel <= 0)
		{
			return;
		}
		CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1416u);
		CSPKG_USEHONOR_REQ stUseHonorReq = cSPkg.stPkgData.stUseHonorReq;
		stUseHonorReq.iHonorID = cOMDT_HONORINFO.iHonorID;
		Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
	}

	private void OnPlayerInfoTabChange()
	{
		Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Player_Info_Honor_Item_Enable, new CUIEventManager.OnUIEventHandler(this.OnHonorItemEnable));
		Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Player_Info_Honor_Select_Change, new CUIEventManager.OnUIEventHandler(this.OnHonorSelectChange));
		Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Player_Info_Honor_Chosen, new CUIEventManager.OnUIEventHandler(this.OnHonorChosen));
		Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.PlayerInfoSystem_Tab_Change, new Action(this.OnPlayerInfoTabChange));
	}

	[MessageHandler(1417)]
	public static void ReceiveHonorChosenRsp(CSPkg msg)
	{
		Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
		CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CPlayerInfoSystem.sPlayerInfoFormPath);
		SCPKG_USEHONOR_RSP stUseHonorRsp = msg.stPkgData.stUseHonorRsp;
		if (stUseHonorRsp.iErrorCode != 0)
		{
			Singleton<CUIManager>.GetInstance().OpenTips(Utility.ProtErrCodeToStr(1417, msg.stPkgData.stUseHonorRsp.iErrorCode), false, 1.5f, null, new object[0]);
			if (form != null)
			{
				COMDT_HONORINFO cOMDT_HONORINFO = new COMDT_HONORINFO();
				cOMDT_HONORINFO.iHonorID = msg.stPkgData.stUseHonorRsp.iHonorID;
				cOMDT_HONORINFO.iHonorLevel = 0;
				Singleton<CPlayerHonorController>.GetInstance().UpdateSelectedHonor(form, cOMDT_HONORINFO);
			}
			return;
		}
		CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
		if (masterRoleInfo != null)
		{
			masterRoleInfo.selectedHonorID = msg.stPkgData.stUseHonorRsp.iHonorID;
			CPlayerProfile profile = Singleton<CPlayerInfoSystem>.GetInstance().GetProfile();
			profile.ConvertRoleInfoData(masterRoleInfo);
			if (form != null)
			{
				Singleton<CPlayerHonorController>.GetInstance().Draw(form);
			}
		}
	}
}
