using Apollo;
using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class CUICommonSystem : Singleton<CUICommonSystem>
	{
		public static string FPS_FORM_PATH = "UGUI/Form/System/FPS/FPSForm.prefab";

		public static string FORM_TEXT_TIPS = "UGUI/Form/Common/Form_Text_Tips.prefab";

		public static string FORM_SENDING_ALERT = "UGUI/Form/Common/Form_SendMsgAlert.prefab";

		public static string FORM_MESSAGE_BOX = "UGUI/Form/Common/Form_MessageBox.prefab";

		public static string FORM_COMMON_TIPS = "UGUI/Form/Common/Form_CommonInfo.prefab";

		public static string FORM_ITEM_TIPS = "UGUI/Form/Common/Form_ItemInfo.prefab";

		public static string GIFT_BAG_DETAIL_PATH = "UGUI/Form/Common/Form_Gift_Bag_Detail.prefab";

		public static string s_manPath = "Prefab_Characters/Prefab_Image/Man/Handsome_1";

		public static string s_womanPath = "Prefab_Characters/Prefab_Image/Girl/BeautyGirl_1";

		public static string[] s_attNameList = new string[37];

		private static string s_last3DModelPath = null;

		public static string s_newHeroOrSkinPath = "UGUI/Form/Common/Form_NewHeroOrSkin.prefab";

		public static string s_newSymbolFormPath = string.Format("{0}{1}", "UGUI/Form/System/", "Symbol/Form_NewSymbol.prefab");

		public static string s_FormFadeRecoverAniName = "FadeRecover";

		public static string s_FormFadeOutAniName = "FadeOut";

		public static bool s_IsOpenFormFade = true;

		public static ValueDataInfo[] s_heroValArr = new ValueDataInfo[37];

		public static uint s_heroId = 0u;

		public static List<uint> s_pctFuncEftList = new List<uint>();

		public static List<NewHeroOrSkinParams> newHeroOrSkinList = new List<NewHeroOrSkinParams>();

		private static bool s_isNewHeroOrSkinShowing = false;

		public override void Init()
		{
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Tips_Close, new CUIEventManager.OnUIEventHandler(this.OnTips_Close));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Common_SendMsgAlertOpen, new CUIEventManager.OnUIEventHandler(this.OnSendMsgAlertOpen));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Common_SendMsgAlertClose, new CUIEventManager.OnUIEventHandler(this.OnSendMsgAlertClose));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Tips_ItemInfoOpen, new CUIEventManager.OnUIEventHandler(this.OnTips_ItemInfoOpen));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Tips_ItemInfoClose, new CUIEventManager.OnUIEventHandler(this.OnTips_ItemInfoClose));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Tips_CommonInfoOpen, new CUIEventManager.OnUIEventHandler(this.OnTips_CommonInfoOpen));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Tips_CommonInfoClose, new CUIEventManager.OnUIEventHandler(this.OnTips_CommonInfoClose));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSkin_LoadNewHeroOrSkin3DModel, new CUIEventManager.OnUIEventHandler(CUICommonSystem.OnLoadNewHeroOrSkin3DModel));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.GiftBag_OnShowDetail, new CUIEventManager.OnUIEventHandler(this.OnGiftBag_ShowDetail));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.UIFade_CloseFormWithAniPopUp, new CUIEventManager.OnUIEventHandler(this.OnCloseFormWithPopUp));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.UIFade_CloseFormWithAni, new CUIEventManager.OnUIEventHandler(this.OnCloseFormWithFirsEntry));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.UI_OnFormCloseWithAnimation, new CUIEventManager.OnUIEventHandler(this.OnCloseFormWithAnimation));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.UIFade_ResetLobbyFormFadeRecover, new CUIEventManager.OnUIEventHandler(this.OnResetLobbyFormFadeRecover));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.UIFade_OpenFormWitAni_Hero, new CUIEventManager.OnUIEventHandler(this.OnOpenHeroWithAni));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.UIFade_OpenFormWitAni_Symbol, new CUIEventManager.OnUIEventHandler(this.OnOpenSymbolWithAni));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.UIFade_OpenFormWitAni_Skill, new CUIEventManager.OnUIEventHandler(this.OnOpenSkillWithAni));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.UIFade_OpenFormWitAni_Equip, new CUIEventManager.OnUIEventHandler(this.OnOpenEquipWithAni));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.UIFade_OpenFormWitAni_ChatSetting, new CUIEventManager.OnUIEventHandler(this.OnOpenChatSettingWithAni));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.UIFade_OpenFormWitAni_Chengjiu, new CUIEventManager.OnUIEventHandler(this.OnOpenChengjiuWithAni));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.UIFade_OpenFormWitAni_BattleTeam, new CUIEventManager.OnUIEventHandler(this.OnOpenBattleTeamWithAni));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.UIFade_OpenFormWitAni_Bag, new CUIEventManager.OnUIEventHandler(this.OnOpenBagWithAni));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.UIFade_OpenFormWitAni_Mishu, new CUIEventManager.OnUIEventHandler(this.OnOpenMishuWithAni));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.UIFade_OpenFormWitAni_Shop, new CUIEventManager.OnUIEventHandler(this.OnOpenShopWithAni));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.UIFade_OpenFormWitAni_OB, new CUIEventManager.OnUIEventHandler(this.OnOpenOBWithAni));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.UIFade_OpenFormWitAni_Charge, new CUIEventManager.OnUIEventHandler(this.OnOpenChargeWithAni));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.UIFade_OpenFormWitAni_Friend, new CUIEventManager.OnUIEventHandler(this.OnOpenFriendWithAni));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.UIFade_OpenFormWitAni_Mail, new CUIEventManager.OnUIEventHandler(this.OnOpenMailWithAni));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.UIFade_OpenFormWitAni_Setting, new CUIEventManager.OnUIEventHandler(this.OnOpenSettingWithAni));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.UIFade_OpenFormWitAni_PvpEntry, new CUIEventManager.OnUIEventHandler(this.OnOpenPvpEntryWithAni));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.UIFade_OpenFormWitAni_LadderEntry, new CUIEventManager.OnUIEventHandler(this.OnOpenLadderEntryWithAni));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.UIFade_OpenFormWitAni_PveEntry, new CUIEventManager.OnUIEventHandler(this.OnOpenPveEntryWithAni));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.UIFade_OpenFormWitAni_UnionBattleEntry, new CUIEventManager.OnUIEventHandler(this.OnOpenUnionBattleWithAni));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Common_OpenUrl, new CUIEventManager.OnUIEventHandler(this.OnCommonOpenUrl));
			int num = 0;
			int.TryParse(Singleton<CTextManager>.instance.GetText("LobbyFadeAniDisableFlag"), out num);
			CUICommonSystem.s_IsOpenFormFade = (num == 0);
			for (int i = 0; i < CUICommonSystem.s_heroValArr.Length; i++)
			{
				CUICommonSystem.s_heroValArr[i] = new ValueDataInfo((RES_FUNCEFT_TYPE)i, 0, 0, null, 0, 0, 0);
			}
			CUICommonSystem.InitPctFuncEftList();
			CUICommonSystem.InitAttNameList();
			CFileManager.s_delegateOnOperateFileFail = (CFileManager.DelegateOnOperateFileFail)Delegate.Combine(CFileManager.s_delegateOnOperateFileFail, new CFileManager.DelegateOnOperateFileFail(this.OnOperateFileFail));
		}

		private static void InitPctFuncEftList()
		{
			CUICommonSystem.s_pctFuncEftList.Clear();
			CUICommonSystem.s_pctFuncEftList.Add(18u);
			CUICommonSystem.s_pctFuncEftList.Add(6u);
			CUICommonSystem.s_pctFuncEftList.Add(12u);
			CUICommonSystem.s_pctFuncEftList.Add(9u);
			CUICommonSystem.s_pctFuncEftList.Add(10u);
			CUICommonSystem.s_pctFuncEftList.Add(15u);
			CUICommonSystem.s_pctFuncEftList.Add(20u);
		}

		private static void InitAttNameList()
		{
			CTextManager instance = Singleton<CTextManager>.GetInstance();
			for (int i = 0; i < 37; i++)
			{
				switch (i)
				{
				case 1:
					CUICommonSystem.s_attNameList[i] = instance.GetText("Hero_Prop_PhyAtkPt");
					break;
				case 2:
					CUICommonSystem.s_attNameList[i] = instance.GetText("Hero_Prop_MgcAtkPt");
					break;
				case 3:
					CUICommonSystem.s_attNameList[i] = instance.GetText("Hero_Prop_PhyDefPt");
					break;
				case 4:
					CUICommonSystem.s_attNameList[i] = instance.GetText("Hero_Prop_MgcDefPt");
					break;
				case 5:
					CUICommonSystem.s_attNameList[i] = instance.GetText("Hero_Prop_MaxHp");
					break;
				case 6:
					CUICommonSystem.s_attNameList[i] = instance.GetText("Hero_Prop_CritRate");
					break;
				case 7:
					CUICommonSystem.s_attNameList[i] = instance.GetText("Hero_Prop_PhyArmorHurt");
					break;
				case 8:
					CUICommonSystem.s_attNameList[i] = instance.GetText("Hero_Prop_MgcArmorHurt");
					break;
				case 9:
					CUICommonSystem.s_attNameList[i] = instance.GetText("Hero_Prop_PhyVamp");
					break;
				case 10:
					CUICommonSystem.s_attNameList[i] = instance.GetText("Hero_Prop_MgcVamp");
					break;
				case 11:
					CUICommonSystem.s_attNameList[i] = instance.GetText("Hero_Prop_AntiCrit");
					break;
				case 12:
					CUICommonSystem.s_attNameList[i] = instance.GetText("Hero_Prop_CritEft");
					break;
				case 13:
					CUICommonSystem.s_attNameList[i] = instance.GetText("Hero_Prop_RealHurt");
					break;
				case 14:
					CUICommonSystem.s_attNameList[i] = instance.GetText("Hero_Prop_RealHurtLess");
					break;
				case 15:
					CUICommonSystem.s_attNameList[i] = instance.GetText("Hero_Prop_MoveSpd");
					break;
				case 16:
					CUICommonSystem.s_attNameList[i] = instance.GetText("Hero_Prop_HpRecover");
					break;
				case 17:
					CUICommonSystem.s_attNameList[i] = instance.GetText("Hero_Prop_CtrlReduce");
					break;
				case 18:
					CUICommonSystem.s_attNameList[i] = instance.GetText("Hero_Prop_AtkSpd");
					break;
				case 19:
					CUICommonSystem.s_attNameList[i] = instance.GetText("Hero_Prop_BaseHurtAdd");
					break;
				case 20:
					CUICommonSystem.s_attNameList[i] = instance.GetText("Hero_Prop_CdReduce");
					break;
				case 21:
					CUICommonSystem.s_attNameList[i] = instance.GetText("Hero_Prop_SightArea");
					break;
				case 22:
					CUICommonSystem.s_attNameList[i] = instance.GetText("Hero_Prop_HitRate");
					break;
				case 23:
					CUICommonSystem.s_attNameList[i] = instance.GetText("Hero_Prop_HitRateAvoid");
					break;
				case 24:
					CUICommonSystem.s_attNameList[i] = instance.GetText("Hero_Prop_CritLvl");
					break;
				case 25:
					CUICommonSystem.s_attNameList[i] = instance.GetText("Hero_Prop_ReduceCritLvl");
					break;
				case 26:
					CUICommonSystem.s_attNameList[i] = instance.GetText("Hero_Prop_PhyVampLvl");
					break;
				case 27:
					CUICommonSystem.s_attNameList[i] = instance.GetText("Hero_Prop_MgcVampLvl");
					break;
				case 28:
					CUICommonSystem.s_attNameList[i] = instance.GetText("Hero_Prop_AtkSpdLvl");
					break;
				case 29:
					CUICommonSystem.s_attNameList[i] = instance.GetText("Hero_Prop_CtrlReduceLvl");
					break;
				case 30:
					CUICommonSystem.s_attNameList[i] = instance.GetText("Hero_Prop_HurtLessRate");
					break;
				default:
					CUICommonSystem.s_attNameList[i] = string.Empty;
					break;
				}
			}
		}

		private void OnOperateFileFail(string fullPath, enFileOperation fileOperation, Exception ex)
		{
			string text = null;
			Exception e = null;
			switch (fileOperation)
			{
			case enFileOperation.ReadFile:
				text = "ReadFileFail";
				e = new ReadFileException(text, ex);
				break;
			case enFileOperation.WriteFile:
				text = "WriteFileFail";
				e = new WriteFileException(text, ex);
				break;
			case enFileOperation.DeleteFile:
				text = "DeleteFileFail";
				e = new DeleteFileException(text, ex);
				break;
			case enFileOperation.CreateDirectory:
				text = "CreateDirectoryFail";
				e = new CreateDirectoryException(text, ex);
				break;
			case enFileOperation.DeleteDirectory:
				text = "DeleteDirectoryFail";
				e = new DeleteDirectoryException(text, ex);
				break;
			}
			if (!string.IsNullOrEmpty(text))
			{
				Singleton<CUIManager>.GetInstance().OpenTips(text, true, 1.5f, null, new object[0]);
				if (ex != null)
				{
					fullPath += ex.ToString();
				}
				BuglyAgent.ReportException(e, fullPath);
			}
		}

		private void OnTips_Close(CUIEvent uiEvent)
		{
			Singleton<CUIManager>.GetInstance().CloseForm(uiEvent.m_srcFormScript);
		}

		private void OnSendMsgAlertOpen(CUIEvent uiEvent)
		{
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CUICommonSystem.FORM_SENDING_ALERT, false, false);
			if (cUIFormScript == null)
			{
				return;
			}
			Transform transform = cUIFormScript.transform.Find("Timer");
			if (transform == null)
			{
				return;
			}
			CUITimerScript component = transform.GetComponent<CUITimerScript>();
			if (component != null)
			{
				component.EndTimer();
			}
			if (!string.IsNullOrEmpty(uiEvent.m_eventParams.tagStr))
			{
				cUIFormScript.transform.Find("Panel/Panel").gameObject.CustomSetActive(true);
				cUIFormScript.transform.Find("Panel/Image").gameObject.CustomSetActive(false);
				Text component2 = cUIFormScript.transform.Find("Panel/Panel/Text").GetComponent<Text>();
				component2.text = uiEvent.m_eventParams.tagStr;
				if (uiEvent.m_eventParams.tag != 0 && component != null)
				{
					component.SetTotalTime((float)uiEvent.m_eventParams.tag);
					if (uiEvent.m_eventParams.tag2 != 0)
					{
						component.SetTimerEventId(enTimerEventType.TimeUp, (enUIEventID)uiEvent.m_eventParams.tag2);
					}
				}
			}
			else if (uiEvent.m_eventParams.tag != 0 && component != null)
			{
				component.SetTotalTime((float)uiEvent.m_eventParams.tag);
				if (uiEvent.m_eventParams.tag2 != 0)
				{
					component.SetTimerEventId(enTimerEventType.TimeUp, (enUIEventID)uiEvent.m_eventParams.tag2);
				}
			}
			component.StartTimer();
		}

		private void OnSendMsgAlertClose(CUIEvent uiEvent)
		{
			Singleton<CUIManager>.GetInstance().CloseForm("UGUI/Form/Common/Form_SendMsgAlert.prefab");
		}

		private void OnTips_ItemInfoOpen(CUIEvent uiEvent)
		{
			this.OpenUseableTips(uiEvent.m_eventParams.iconUseable, uiEvent.m_pointerEventData.pressPosition.x, uiEvent.m_pointerEventData.pressPosition.y, (enUseableTipsPos)uiEvent.m_eventParams.tag);
		}

		private void OnTips_ItemInfoClose(CUIEvent uiEvent)
		{
			CUICommonSystem.CloseUseableTips();
		}

		public static void CloseUseableTips()
		{
			Singleton<CUIManager>.GetInstance().CloseForm(CUICommonSystem.FORM_ITEM_TIPS);
		}

		public static void CloseCommonTips()
		{
			Singleton<CUIManager>.GetInstance().CloseForm(CUICommonSystem.FORM_COMMON_TIPS);
		}

		public void OpenUseableTips(CUseable iconUseable, float srcX, float srcY, enUseableTipsPos pos = enUseableTipsPos.enTop)
		{
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CUICommonSystem.FORM_ITEM_TIPS, false, false);
			if (cUIFormScript == null)
			{
				return;
			}
			GameObject gameObject = cUIFormScript.gameObject.transform.Find("Panel").gameObject;
			GameObject gameObject2 = cUIFormScript.gameObject.transform.Find("Panel/PanelTop").gameObject;
			GameObject gameObject3 = cUIFormScript.gameObject.transform.Find("Panel/itemCell").gameObject;
			Text component = cUIFormScript.gameObject.transform.Find("Panel/titleContainer/lblName").GetComponent<Text>();
			Text component2 = cUIFormScript.gameObject.transform.Find("Panel/lblHave").GetComponent<Text>();
			Text component3 = cUIFormScript.gameObject.transform.Find("Panel/titleContainer/lblCount").GetComponent<Text>();
			Text component4 = cUIFormScript.gameObject.transform.Find("Panel/panelPrice/lblPrice").GetComponent<Text>();
			Text component5 = cUIFormScript.gameObject.transform.Find("Panel/lblDesc").GetComponent<Text>();
			GameObject gameObject4 = cUIFormScript.gameObject.transform.Find("Panel/panelPrice").gameObject;
			CUICommonSystem.SetItemCell(cUIFormScript, gameObject3, iconUseable, false, false, false, false);
			component.text = iconUseable.m_name;
			component5.text = iconUseable.m_description;
			component2.gameObject.CustomSetActive(false);
			component3.gameObject.CustomSetActive(false);
			gameObject4.gameObject.CustomSetActive(false);
			CUseableContainer useableContainer = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GetUseableContainer(enCONTAINER_TYPE.ITEM);
			if (iconUseable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP)
			{
				CItem cItem = iconUseable as CItem;
				int useableStackCount = useableContainer.GetUseableStackCount(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, cItem.m_baseID);
				if (cItem.m_itemData.bClass != 2 && cItem.m_itemData.bClass != 3)
				{
					component3.text = "当前拥有 <color=#a52a2aff>" + useableStackCount + "</color>";
					component3.gameObject.CustomSetActive(true);
				}
				component4.text = cItem.m_coinSale.ToString();
				component5.text = CUIUtility.StringReplace(component5.text, new string[]
				{
					useableStackCount.ToString()
				});
			}
			else if (iconUseable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMEQUIP)
			{
				int useableStackCount2 = useableContainer.GetUseableStackCount(COM_ITEM_TYPE.COM_OBJTYPE_ITEMEQUIP, iconUseable.m_baseID);
				component3.text = "当前拥有 <color=#a52a2aff>" + useableStackCount2 + "</color>";
				component3.gameObject.CustomSetActive(true);
				component4.text = iconUseable.m_coinSale.ToString();
				component5.text = CUIUtility.StringReplace(component5.text, new string[]
				{
					useableStackCount2.ToString()
				});
			}
			else if (iconUseable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL)
			{
				int useableStackCount3 = useableContainer.GetUseableStackCount(COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL, iconUseable.m_baseID);
				component3.text = "当前拥有 <color=#a52a2aff>" + useableStackCount3 + "</color>";
				component3.gameObject.CustomSetActive(true);
				component4.text = iconUseable.m_coinSale.ToString();
				component5.text = CUIUtility.StringReplace(component5.text, new string[]
				{
					useableStackCount3.ToString()
				});
			}
			CVirtualItem cVirtualItem = iconUseable as CVirtualItem;
			if (cVirtualItem != null)
			{
				enVirtualItemType virtualType = cVirtualItem.m_virtualType;
				if (virtualType == enVirtualItemType.enExp)
				{
					CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
					component3.text = string.Concat(new object[]
					{
						"当前经验",
						masterRoleInfo.PvpExp,
						"/",
						masterRoleInfo.PvpNeedExp
					});
					component3.gameObject.CustomSetActive(true);
				}
			}
			RectTransform rectTransform = (RectTransform)gameObject.transform;
			float height = ((RectTransform)gameObject2.transform).rect.height;
			rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, height + component5.preferredHeight);
			float num = srcX;
			float num2 = srcY;
			float num3 = 60f;
			float num4 = cUIFormScript.ChangeFormValueToScreen(rectTransform.rect.width);
			float num5 = cUIFormScript.ChangeFormValueToScreen(rectTransform.rect.height);
			switch (pos)
			{
			case enUseableTipsPos.enTop:
				num -= num4 / 2f;
				num2 += num3;
				break;
			case enUseableTipsPos.enLeft:
				num -= num4 + num3;
				num2 -= num5 / 2f;
				break;
			case enUseableTipsPos.enRight:
				num += num3;
				num2 -= num5 / 2f;
				break;
			case enUseableTipsPos.enBottom:
				num -= num4 / 2f;
				num2 -= num5 + num3;
				break;
			}
			if (num < 0f)
			{
				num = 0f;
			}
			else if (num + num4 > (float)Screen.width)
			{
				num = (float)Screen.width - num4;
			}
			if (num2 < 0f)
			{
				num2 = 0f;
			}
			else if (num2 + num5 > (float)Screen.height)
			{
				num2 = (float)Screen.height - num5;
			}
			rectTransform.position = CUIUtility.ScreenToWorldPoint(null, new Vector2(num, num2), 0f);
		}

		private void OnTips_CommonInfoOpen(CUIEvent uiEvent)
		{
			CUICommonSystem.OpenCommonTips(uiEvent.m_pointerEventData.pressPosition.x, uiEvent.m_pointerEventData.pressPosition.y, uiEvent.m_eventParams.tagStr, (enUseableTipsPos)uiEvent.m_eventParams.tag);
		}

		private void OnTips_CommonInfoClose(CUIEvent uiEvent)
		{
			CUICommonSystem.CloseCommonTips();
		}

		private void OnGiftBag_ShowDetail(CUIEvent uiEvent)
		{
			CUseable iconUseable = uiEvent.m_eventParams.iconUseable;
			if (iconUseable == null)
			{
				return;
			}
			if (iconUseable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP)
			{
				CItem cItem = iconUseable as CItem;
				if (cItem == null || cItem.m_itemData.bIsView <= 0)
				{
					return;
				}
				uint key = (uint)cItem.m_itemData.EftParam[0];
				ResRandomRewardStore dataByKey = GameDataMgr.randomRewardDB.GetDataByKey(key);
				ListView<CUseable> listView = new ListView<CUseable>();
				for (int i = 0; i < dataByKey.astRewardDetail.Length; i++)
				{
					if (dataByKey.astRewardDetail[i].bItemType != 0)
					{
						CUseable cUseable;
						if (dataByKey.astRewardDetail[i].dwLowCnt != dataByKey.astRewardDetail[i].dwHighCnt)
						{
							cUseable = CUseableManager.CreateUsableByRandowReward((RES_RANDOM_REWARD_TYPE)dataByKey.astRewardDetail[i].bItemType, 1, dataByKey.astRewardDetail[i].dwItemID);
						}
						else
						{
							cUseable = CUseableManager.CreateUsableByRandowReward((RES_RANDOM_REWARD_TYPE)dataByKey.astRewardDetail[i].bItemType, (int)dataByKey.astRewardDetail[i].dwLowCnt, dataByKey.astRewardDetail[i].dwItemID);
						}
						if (cUseable != null)
						{
							listView.Add(cUseable);
						}
					}
				}
				if (listView.Count == 0)
				{
					return;
				}
				CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CUICommonSystem.GIFT_BAG_DETAIL_PATH, false, true);
				if (cUIFormScript == null)
				{
					return;
				}
				Text componetInChild = Utility.GetComponetInChild<Text>(cUIFormScript.gameObject, "Panel/lblDesc");
				if (componetInChild != null)
				{
					componetInChild.text = cItem.m_description;
				}
				CUIListScript componetInChild2 = Utility.GetComponetInChild<CUIListScript>(cUIFormScript.gameObject, "Panel/itemGroup");
				componetInChild2.SetElementAmount(listView.Count);
				for (int j = 0; j < listView.Count; j++)
				{
					CUIListElementScript elemenet = componetInChild2.GetElemenet(j);
					CUICommonSystem.SetItemCell(elemenet.m_belongedFormScript, elemenet.GetWidget(0), listView[j], true, false, true, false);
					if (listView[j].m_stackCount == 1)
					{
						Utility.FindChild(elemenet.GetWidget(0), "lblIconCount").CustomSetActive(false);
					}
					else
					{
						Utility.FindChild(elemenet.GetWidget(0), "lblIconCount").CustomSetActive(true);
					}
					if (listView[j] != null)
					{
						CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
						if (masterRoleInfo != null)
						{
							if (listView[j].m_type == COM_ITEM_TYPE.COM_OBJTYPE_HERO)
							{
								CHeroItem cHeroItem = listView[j] as CHeroItem;
								if (masterRoleInfo.IsOwnHero(cHeroItem.m_heroData.dwCfgID))
								{
									Utility.FindChild(elemenet.GetWidget(0), "HaveItemFlag").CustomSetActive(true);
								}
								else
								{
									Utility.FindChild(elemenet.GetWidget(0), "HaveItemFlag").CustomSetActive(false);
								}
							}
							else if (listView[j].m_type == COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN)
							{
								CHeroSkin cHeroSkin = listView[j] as CHeroSkin;
								if (masterRoleInfo.IsHaveHeroSkin(cHeroSkin.m_heroId, cHeroSkin.m_skinId, false))
								{
									Utility.FindChild(elemenet.GetWidget(0), "HaveItemFlag").CustomSetActive(true);
								}
								else
								{
									Utility.FindChild(elemenet.GetWidget(0), "HaveItemFlag").CustomSetActive(false);
								}
							}
						}
					}
				}
			}
		}

		private static void OpenCommonTips(float srcX, float srcY, string strContent, enUseableTipsPos pos = enUseableTipsPos.enTop)
		{
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CUICommonSystem.FORM_COMMON_TIPS, false, false);
			if (cUIFormScript == null)
			{
				return;
			}
			GameObject gameObject = cUIFormScript.gameObject.transform.Find("Panel").gameObject;
			GameObject gameObject2 = cUIFormScript.gameObject.transform.Find("Panel/PanelTop").gameObject;
			Text component = cUIFormScript.gameObject.transform.Find("Panel/lblDesc").GetComponent<Text>();
			component.text = strContent;
			RectTransform rectTransform = (RectTransform)gameObject.transform;
			float height = ((RectTransform)gameObject2.transform).rect.height;
			rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, height + component.preferredHeight);
			float num = srcX;
			float num2 = srcY;
			float num3 = 60f;
			float num4 = cUIFormScript.ChangeFormValueToScreen(rectTransform.rect.width);
			float num5 = cUIFormScript.ChangeFormValueToScreen(rectTransform.rect.height);
			switch (pos)
			{
			case enUseableTipsPos.enTop:
				num -= num4 / 2f;
				num2 += num3;
				break;
			case enUseableTipsPos.enLeft:
				num -= num4 + num3;
				num2 -= num5 / 2f;
				break;
			case enUseableTipsPos.enRight:
				num += num3;
				num2 -= num5 / 2f;
				break;
			case enUseableTipsPos.enBottom:
				num -= num4 / 2f;
				num2 -= num5 + num3;
				break;
			}
			if (num < 0f)
			{
				num = 0f;
			}
			else if (num + num4 > (float)Screen.width)
			{
				num = (float)Screen.width - num4;
			}
			if (num2 < 0f)
			{
				num2 = 0f;
			}
			else if (num2 + num5 > (float)Screen.height)
			{
				num2 = (float)Screen.height - num5;
			}
			rectTransform.position = CUIUtility.ScreenToWorldPoint(null, new Vector2(num, num2), 0f);
		}

		public static void SetCommonTipsEvent(CUIFormScript formScript, GameObject targetObj, string strContent, enUseableTipsPos tipPos = enUseableTipsPos.enTop)
		{
			CUIEventScript cUIEventScript = targetObj.GetComponent<CUIEventScript>();
			if (cUIEventScript == null)
			{
				cUIEventScript = targetObj.AddComponent<CUIEventScript>();
				cUIEventScript.Initialize(formScript);
			}
			stUIEventParams eventParams = default(stUIEventParams);
			eventParams.tag = (int)tipPos;
			eventParams.tagStr = strContent;
			cUIEventScript.SetUIEvent(enUIEventType.Down, enUIEventID.Tips_CommonInfoOpen, eventParams);
			cUIEventScript.SetUIEvent(enUIEventType.Up, enUIEventID.Tips_CommonInfoClose, eventParams);
		}

		public static void SetItemCell(CUIFormScript formScript, GameObject itemCell, CUseable itemUseable, bool isHaveClickEvent = true, bool displayAll = false, bool cntForceDisplay = false, bool isClickToShowDetail = false)
		{
			if (itemUseable == null || itemCell == null)
			{
				return;
			}
			Image component = itemCell.transform.Find("imgIcon").GetComponent<Image>();
			CUIUtility.SetImageSprite(component, itemUseable.GetIconPath(), formScript, false, false, false, false);
			string prefabPath = string.Format("{0}Common_slotBg{1}", "UGUI/Sprite/Common/", (int)(itemUseable.m_grade + 1));
			CUIUtility.SetImageSprite(itemCell.GetComponent<Image>(), prefabPath, formScript, true, false, false, false);
			Transform transform = itemCell.transform.Find("lblIconCount");
			if (transform != null)
			{
				if (cntForceDisplay)
				{
					transform.gameObject.CustomSetActive(true);
				}
				Text component2 = transform.GetComponent<Text>();
				if (itemUseable.m_stackCount < 10000 || displayAll)
				{
					component2.text = itemUseable.m_stackCount.ToString();
				}
				else
				{
					component2.text = itemUseable.m_stackCount / 10000 + "万";
				}
				CUICommonSystem.AppendMultipleText(component2, itemUseable.m_stackMulti);
				if (itemUseable.m_stackCount <= 0)
				{
					component2.gameObject.CustomSetActive(false);
				}
				if (itemUseable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL)
				{
					if (((CSymbolItem)itemUseable).IsGuildSymbol())
					{
						component2.text = string.Empty;
					}
					else
					{
						component2.text = itemUseable.GetSalableCount().ToString();
					}
				}
			}
			Transform transform2 = itemCell.transform.Find("imgSuipian");
			if (transform2 != null)
			{
				Image component3 = transform2.GetComponent<Image>();
				component3.gameObject.CustomSetActive(false);
				if (itemUseable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP)
				{
					CItem cItem = itemUseable as CItem;
					if (cItem.m_itemData.bClass == 2 || cItem.m_itemData.bClass == 3)
					{
						component3.gameObject.CustomSetActive(true);
					}
				}
			}
			Transform transform3 = itemCell.transform.Find("ItemName");
			if (transform3 != null)
			{
				Text component4 = transform3.gameObject.GetComponent<Text>();
				if (component4 != null)
				{
					component4.text = itemUseable.m_name;
				}
			}
			Transform transform4 = itemCell.transform.Find("imgExperienceMark");
			if (transform4 != null)
			{
				if (itemUseable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP && CItem.IsHeroExperienceCard(itemUseable.m_baseID))
				{
					transform4.gameObject.CustomSetActive(true);
					transform4.GetComponent<Image>().SetSprite(CUIUtility.GetSpritePrefeb(CExperienceCardSystem.HeroExperienceCardMarkPath, false, false), false);
				}
				else if (itemUseable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP && CItem.IsSkinExperienceCard(itemUseable.m_baseID))
				{
					transform4.gameObject.CustomSetActive(true);
					transform4.GetComponent<Image>().SetSprite(CUIUtility.GetSpritePrefeb(CExperienceCardSystem.SkinExperienceCardMarkPath, false, false), false);
				}
				else
				{
					transform4.gameObject.CustomSetActive(false);
				}
			}
			if (isHaveClickEvent)
			{
				CUIEventScript cUIEventScript = itemCell.GetComponent<CUIEventScript>();
				if (cUIEventScript == null)
				{
					cUIEventScript = itemCell.AddComponent<CUIEventScript>();
					cUIEventScript.Initialize(formScript);
				}
				stUIEventParams eventParams = default(stUIEventParams);
				eventParams.iconUseable = itemUseable;
				cUIEventScript.SetUIEvent(enUIEventType.Down, enUIEventID.Tips_ItemInfoOpen, eventParams);
				cUIEventScript.SetUIEvent(enUIEventType.Up, enUIEventID.Tips_ItemInfoClose, eventParams);
			}
			if (isClickToShowDetail)
			{
				CItem cItem2 = itemUseable as CItem;
				if (cItem2 == null || cItem2.m_itemData.bIsView <= 0)
				{
					return;
				}
				CUIEventScript cUIEventScript2 = itemCell.GetComponent<CUIEventScript>();
				if (cUIEventScript2 == null)
				{
					cUIEventScript2 = itemCell.AddComponent<CUIEventScript>();
					cUIEventScript2.Initialize(formScript);
				}
				cUIEventScript2.SetUIEvent(enUIEventType.Click, enUIEventID.GiftBag_OnShowDetail, new stUIEventParams
				{
					iconUseable = itemUseable
				});
			}
		}

		public static stSkillPropertyPrams[] ParseSkillLevelUpProperty(ref ResDT_SkillDescription[] skillPropertyDesc, uint heroID)
		{
			stSkillPropertyPrams[] array = new stSkillPropertyPrams[skillPropertyDesc.Length];
			int num = 0;
			for (int i = 0; i < skillPropertyDesc.Length; i++)
			{
				if (skillPropertyDesc[i].szSkillDescType != string.Empty)
				{
					array[num].name = skillPropertyDesc[i].szSkillDescType;
					array[num].baseValue = CUICommonSystem.ParseValue(skillPropertyDesc[i].szSkillDescBaseValue, heroID);
					array[num].growthValue = CUICommonSystem.ParseValue(skillPropertyDesc[i].szSkillDescGrowth, heroID);
					array[num].valueType = skillPropertyDesc[i].dwSkillDescValueType;
					num++;
				}
			}
			return array;
		}

		private static float ParseValue(string str, uint heroID)
		{
			float result;
			int num;
			if (str == string.Empty)
			{
				result = 0f;
			}
			else if (int.TryParse(str, out num))
			{
				result = (float)num / 100f;
			}
			else
			{
				result = (float)CUICommonSystem.GetSkillPropertyValue(str, heroID);
			}
			return result;
		}

		public static void RefreshSkillLevelUpProperty(GameObject skillPropertyInfoPanel, ref stSkillPropertyPrams[] skillPropertyParams, int skillSlotID)
		{
			if (skillPropertyInfoPanel == null)
			{
				return;
			}
			if (skillSlotID < 1 || skillSlotID > 3)
			{
				skillPropertyInfoPanel.CustomSetActive(false);
			}
			else
			{
				int num;
				if (skillSlotID == 3)
				{
					num = 3;
				}
				else
				{
					num = 6;
				}
				for (int i = 4; i <= 6; i++)
				{
					GameObject obj = Utility.FindChild(skillPropertyInfoPanel, "Header/level" + i);
					obj.CustomSetActive(num == 6);
				}
				int num2 = 0;
				for (int j = 0; j < 5; j++)
				{
					GameObject gameObject = Utility.FindChild(skillPropertyInfoPanel, "InfoList/PropertyInfo_" + j);
					if (gameObject == null)
					{
						return;
					}
					if (skillPropertyParams[j].name != null)
					{
						Text componetInChild = Utility.GetComponetInChild<Text>(gameObject, "PropertyName");
						componetInChild.text = skillPropertyParams[j].name;
						for (int k = 1; k <= 6; k++)
						{
							Text componetInChild2 = Utility.GetComponetInChild<Text>(gameObject, "PropertyValue/level" + k);
							if (k <= num)
							{
								float num3 = skillPropertyParams[j].growthValue * (float)(k - 1);
								float num4 = skillPropertyParams[j].baseValue + num3;
								int num5 = (int)num4;
								string text;
								if (skillPropertyParams[j].valueType == 1u)
								{
									text = num5.ToString() + "%";
								}
								else if (num4 > (float)num5)
								{
									text = num4.ToString("0.0");
								}
								else
								{
									text = num5.ToString();
								}
								componetInChild2.text = text;
							}
							else
							{
								componetInChild2.text = string.Empty;
							}
						}
						num2++;
						gameObject.CustomSetActive(true);
					}
					else
					{
						gameObject.CustomSetActive(false);
					}
				}
				skillPropertyInfoPanel.CustomSetActive(num2 > 0);
			}
		}

		public static string ConvertMillisecondToSecondWithOneDecimal(int millsecond)
		{
			float num = (float)millsecond / 1000f;
			string result;
			if (millsecond % 1000 > 0)
			{
				result = ((float)millsecond / 1000f).ToString("0.0");
			}
			else
			{
				result = (millsecond / 1000).ToString();
			}
			return result;
		}

		public static void AppendMultipleText(Text target, int multiple)
		{
			if (multiple > 0)
			{
				target.color = ((multiple <= 1) ? Color.yellow : Color.green);
			}
		}

		public static void SetProgressBarData(GameObject progressBar, int curVal, int totalVal, bool bShowTxt = false)
		{
			Image component = progressBar.transform.Find("Image").GetComponent<Image>();
			component.CustomFillAmount((float)curVal / (float)totalVal);
			Text component2 = progressBar.transform.Find("Text").GetComponent<Text>();
			if (bShowTxt)
			{
				component2.text = curVal.ToString() + "/" + totalVal.ToString();
			}
			else
			{
				component2.text = string.Empty;
			}
		}

		public static void SetButtonName(GameObject btn, string strName)
		{
			Transform transform = btn.transform.Find("Text");
			if (transform != null)
			{
				Text component = transform.GetComponent<Text>();
				if (component != null)
				{
					component.text = strName;
				}
			}
		}

		public static void SetHeroItemData(CUIFormScript formScript, GameObject item, IHeroData data, enHeroHeadType headType = enHeroHeadType.enIcon, bool bIconGray = false, bool isShowSpecMatrial = true)
		{
			if (item == null || data == null)
			{
				return;
			}
			Transform transform = item.transform;
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo != null)
			{
				CUICommonSystem.SetHeroItemImage(formScript, item, CSkinInfo.GetHeroSkinPic(data.cfgID, 0u), headType, bIconGray, isShowSpecMatrial);
			}
			Transform transform2 = transform.Find("profession");
			Transform transform3 = transform.Find("heroNameText");
			if (transform2 != null)
			{
				CUICommonSystem.SetHeroJob(formScript, transform2.gameObject, (enHeroJobType)data.heroType);
			}
			if (transform3 != null)
			{
				Text component = transform3.GetComponent<Text>();
				component.text = data.heroName;
			}
		}

		public static void SetHeroJob(CUIFormScript formScript, GameObject root, enHeroJobType jobType)
		{
			root.GetComponent<Image>().SetSprite(CUIUtility.s_Sprite_Dynamic_Profession_Dir + jobType.ToString(), formScript, true, false, false, false);
		}

		public static void SetHeroItemImage(CUIFormScript formScript, GameObject item, string imgPath, enHeroHeadType headType = enHeroHeadType.enBust, bool bGray = false, bool isShowSpecMatrial = false)
		{
			if (null == item)
			{
				return;
			}
			if (bGray)
			{
				isShowSpecMatrial = false;
			}
			Transform transform = item.transform.Find("imageIcon");
			if (transform != null)
			{
				Image component = transform.GetComponent<Image>();
				if (component != null)
				{
					component.color = ((!bGray) ? CUIUtility.s_Color_White : CUIUtility.s_Color_GrayShader);
					string str = CUIUtility.s_Sprite_Dynamic_Icon_Dir;
					if (headType == enHeroHeadType.enBust)
					{
						str = CUIUtility.s_Sprite_Dynamic_BustHero_Dir;
					}
					else if (headType == enHeroHeadType.enBustCircle)
					{
						str = CUIUtility.s_Sprite_Dynamic_BustCircle_Dir;
					}
					if (component != null)
					{
						if (headType == enHeroHeadType.enBust)
						{
							if (bGray)
							{
								CUIUtility.SetImageGrayMatrial(component);
							}
							component.SetSprite(str + imgPath, formScript, false, true, true, isShowSpecMatrial);
						}
						else
						{
							component.SetSprite(str + imgPath, formScript, false, true, true, false);
						}
					}
				}
			}
		}

		public static void SetHeroProficiencyIconImage(CUIFormScript formScript, GameObject proficiencyIcon, int proficiencyLV)
		{
			if (null == formScript || null == proficiencyIcon)
			{
				return;
			}
			int val = 1;
			int maxProficiency = CHeroInfo.GetMaxProficiency();
			proficiencyLV = Math.Max(val, Math.Min(maxProficiency, proficiencyLV));
			Image component = proficiencyIcon.GetComponent<Image>();
			if (component != null)
			{
				component.SetSprite(string.Format("{0}{1}{2}", CUIUtility.s_Sprite_Dynamic_Proficiency_Dir, "HeroProficiency_Level_", proficiencyLV), formScript, true, false, false, false);
			}
		}

		public static void SetHeroProficiencyBgImage(CUIFormScript formScript, GameObject proficiencyBg, int proficiencyLV, bool bLoading)
		{
			if (null == formScript || null == proficiencyBg)
			{
				return;
			}
			int val = 1;
			int maxProficiency = CHeroInfo.GetMaxProficiency();
			proficiencyLV = Math.Max(val, Math.Min(maxProficiency, proficiencyLV));
			Image component = proficiencyBg.GetComponent<Image>();
			if (component != null)
			{
				string arg = string.Empty;
				if (proficiencyLV != maxProficiency)
				{
					arg = (bLoading ? string.Format("{0}{1}", "HeroProficiency_Bg_Level_1", "_loading") : "HeroProficiency_Bg_Level_1");
					component.SetSprite(string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Proficiency_Dir, arg), formScript, true, false, false, false);
				}
				else
				{
					arg = (bLoading ? string.Format("{0}{1}", "HeroProficiency_Bg_Level_Max", "_loading") : "HeroProficiency_Bg_Level_Max");
					component.SetSprite(string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Proficiency_Dir, arg), formScript, true, false, false, false);
				}
			}
		}

		private static GameObject InstantiateArtPrefabObj(CActorInfo actorInfoRes, bool bLobbyShow)
		{
			CActorInfo cActorInfo = (CActorInfo)UnityEngine.Object.Instantiate(actorInfoRes);
			string text = null;
			if (bLobbyShow)
			{
				text = cActorInfo.GetArtPrefabNameLobby(0);
			}
			if (string.IsNullOrEmpty(text))
			{
				text = cActorInfo.GetArtPrefabName(0, -1);
			}
			GameObject result = null;
			if (!string.IsNullOrEmpty(text))
			{
				result = MonoSingleton<SceneMgr>.GetInstance().InstantiateLOD(text, false, SceneObjType.ActionRes);
			}
			return result;
		}

		public static ObjData GetOrgan3DObj(int inOrganId, bool bLobbyShow)
		{
			ResOrganCfgInfo dataCfgInfoByCurLevelDiff = OrganDataHelper.GetDataCfgInfoByCurLevelDiff(inOrganId);
			if (dataCfgInfoByCurLevelDiff == null)
			{
				return default(ObjData);
			}
			CActorInfo cActorInfo = Singleton<CResourceManager>.GetInstance().GetResource(StringHelper.UTF8BytesToString(ref dataCfgInfoByCurLevelDiff.szCharacterInfo), typeof(CActorInfo), enResourceType.UI3DImage, false, false).m_content as CActorInfo;
			if (cActorInfo == null)
			{
				return default(ObjData);
			}
			return new ObjData
			{
				Object = CUICommonSystem.InstantiateArtPrefabObj(cActorInfo, bLobbyShow),
				ActorInfo = cActorInfo
			};
		}

		public static ObjNameData GetOrgan3DObjPath(int inOrganId, bool bLobbyShow)
		{
			ResOrganCfgInfo dataCfgInfoByCurLevelDiff = OrganDataHelper.GetDataCfgInfoByCurLevelDiff(inOrganId);
			if (dataCfgInfoByCurLevelDiff == null)
			{
				return default(ObjNameData);
			}
			CActorInfo actorInfoRes = Singleton<CResourceManager>.GetInstance().GetResource(StringHelper.UTF8BytesToString(ref dataCfgInfoByCurLevelDiff.szCharacterInfo), typeof(CActorInfo), enResourceType.UI3DImage, false, false).m_content as CActorInfo;
			return CUICommonSystem.Get3DObjPath(actorInfoRes, bLobbyShow);
		}

		private static ObjNameData Get3DObjPath(CActorInfo actorInfoRes, bool bLobbyShow)
		{
			if (actorInfoRes == null)
			{
				return default(ObjNameData);
			}
			CActorInfo cActorInfo = (CActorInfo)UnityEngine.Object.Instantiate(actorInfoRes);
			string text = null;
			if (bLobbyShow)
			{
				text = cActorInfo.GetArtPrefabNameLobby(0);
			}
			if (string.IsNullOrEmpty(text))
			{
				text = cActorInfo.GetArtPrefabName(0, -1);
			}
			return new ObjNameData
			{
				ObjectName = text,
				ActorInfo = cActorInfo
			};
		}

		public static ObjNameData GetMonster3DObjPath(int inMonId, bool bLobbyShow)
		{
			ResMonsterCfgInfo resMonsterCfgInfo = MonsterDataHelper.GetDataCfgInfoByCurLevelDiff(inMonId);
			if (resMonsterCfgInfo == null)
			{
				resMonsterCfgInfo = MonsterDataHelper.GetDataCfgInfo(inMonId, 1);
			}
			if (resMonsterCfgInfo == null)
			{
				return default(ObjNameData);
			}
			CActorInfo actorInfoRes = Singleton<CResourceManager>.GetInstance().GetResource(StringHelper.UTF8BytesToString(ref resMonsterCfgInfo.szCharacterInfo), typeof(CActorInfo), enResourceType.UI3DImage, false, false).m_content as CActorInfo;
			return CUICommonSystem.Get3DObjPath(actorInfoRes, bLobbyShow);
		}

		public static ObjData GetMonster3DObj(int inMonId, bool bLobbyShow)
		{
			ResMonsterCfgInfo dataCfgInfoByCurLevelDiff = MonsterDataHelper.GetDataCfgInfoByCurLevelDiff(inMonId);
			if (dataCfgInfoByCurLevelDiff == null)
			{
				return default(ObjData);
			}
			CActorInfo cActorInfo = Singleton<CResourceManager>.GetInstance().GetResource(StringHelper.UTF8BytesToString(ref dataCfgInfoByCurLevelDiff.szCharacterInfo), typeof(CActorInfo), enResourceType.UI3DImage, false, false).m_content as CActorInfo;
			if (cActorInfo == null)
			{
				return default(ObjData);
			}
			return new ObjData
			{
				Object = CUICommonSystem.InstantiateArtPrefabObj(cActorInfo, bLobbyShow),
				ActorInfo = cActorInfo
			};
		}

		public static ObjNameData GetHero3DObjPath(uint heroID, bool bLobbyShow)
		{
			ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(heroID);
			if (dataByKey == null)
			{
				return default(ObjNameData);
			}
			CActorInfo actorInfoRes = Singleton<CResourceManager>.GetInstance().GetResource(StringHelper.UTF8BytesToString(ref dataByKey.szCharacterInfo), typeof(CActorInfo), enResourceType.UI3DImage, false, false).m_content as CActorInfo;
			return CUICommonSystem.Get3DObjPath(actorInfoRes, bLobbyShow);
		}

		public static ObjData GetHero3DObj(uint heroID, bool bLobbyShow)
		{
			ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(heroID);
			if (dataByKey == null)
			{
				return default(ObjData);
			}
			CActorInfo cActorInfo = Singleton<CResourceManager>.GetInstance().GetResource(StringHelper.UTF8BytesToString(ref dataByKey.szCharacterInfo), typeof(CActorInfo), enResourceType.UI3DImage, false, false).m_content as CActorInfo;
			if (cActorInfo == null)
			{
				return default(ObjData);
			}
			return new ObjData
			{
				Object = CUICommonSystem.InstantiateArtPrefabObj(cActorInfo, bLobbyShow),
				ActorInfo = cActorInfo
			};
		}

		public static string GetRole3DObjPath(int roleID)
		{
			return (roleID != 1) ? CUICommonSystem.s_manPath : CUICommonSystem.s_womanPath;
		}

		public static ObjNameData GetHeroPrefabPath(uint heroID, int skidId, bool bLobbyShow)
		{
			ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(heroID);
			if (dataByKey == null)
			{
				return default(ObjNameData);
			}
			CActorInfo cActorInfo = Singleton<CResourceManager>.GetInstance().GetResource(StringHelper.UTF8BytesToString(ref dataByKey.szCharacterInfo), typeof(CActorInfo), enResourceType.UI3DImage, false, false).m_content as CActorInfo;
			if (cActorInfo == null)
			{
				return default(ObjNameData);
			}
			CActorInfo cActorInfo2 = (CActorInfo)UnityEngine.Object.Instantiate(cActorInfo);
			if (cActorInfo2 == null)
			{
				return default(ObjNameData);
			}
			string objectName = cActorInfo2.GetArtPrefabName(skidId, -1);
			if (bLobbyShow)
			{
				objectName = cActorInfo2.GetArtPrefabNameLobby(skidId);
			}
			return new ObjNameData
			{
				ObjectName = objectName,
				ActorInfo = cActorInfo2
			};
		}

		public static GameObject GetAnimation3DOjb(string animationName)
		{
			GameObject original = (GameObject)Singleton<CResourceManager>.GetInstance().GetResource(CUIUtility.s_Animation3D_Dir + animationName, typeof(GameObject), enResourceType.UI3DImage, false, false).m_content;
			return (GameObject)UnityEngine.Object.Instantiate(original);
		}

		public static GameObject GetUIPrefab(string prefabPath)
		{
			GameObject original = (GameObject)Singleton<CResourceManager>.GetInstance().GetResource(prefabPath, typeof(GameObject), enResourceType.UIPrefab, false, false).m_content;
			return (GameObject)UnityEngine.Object.Instantiate(original);
		}

		public static void AddRedDot(GameObject target, enRedDotPos dotPos = enRedDotPos.enTopRight, int alertNum = 0, int offsetX = 0, int offsetY = 0)
		{
			CUIRedDotSystem.AddRedDot(target, dotPos, alertNum, offsetX, offsetY);
		}

		public static void DelRedDot(GameObject target)
		{
			CUIRedDotSystem.DelRedDot(target);
		}

		public static bool IsHaveRedDot(GameObject target)
		{
			return CUIRedDotSystem.IsHaveRedDot(target);
		}

		public static void SetButtonEnable(Button btn, bool isEnable, bool isEventEnable, bool bChangeTextColor = true)
		{
			CUIEventScript component = btn.gameObject.GetComponent<CUIEventScript>();
			component.enabled = isEventEnable;
			btn.interactable = isEnable;
			if (bChangeTextColor)
			{
				Text componentInChildren = btn.gameObject.GetComponentInChildren<Text>();
				if (componentInChildren != null)
				{
					componentInChildren.color = ((!isEnable) ? CUIUtility.s_Text_Color_Disable : CUIUtility.s_Text_Color_White);
				}
			}
		}

		public static void SetButtonEnableWithShader(Button btn, bool isEnable, bool bChangeTextColor = true)
		{
			CUIEventScript component = btn.gameObject.GetComponent<CUIEventScript>();
			component.enabled = isEnable;
			if (isEnable)
			{
				btn.gameObject.CustomSetActive(true);
			}
			Image component2 = btn.gameObject.GetComponent<Image>();
			if (component2 != null)
			{
				if (isEnable)
				{
					component2.color = CUIUtility.s_Color_White;
				}
				else
				{
					component2.color = CUIUtility.s_Color_GrayShader;
				}
			}
			if (bChangeTextColor)
			{
				Text componentInChildren = btn.gameObject.GetComponentInChildren<Text>();
				if (componentInChildren != null)
				{
					componentInChildren.color = ((!isEnable) ? CUIUtility.s_Text_Color_Disable : CUIUtility.s_Text_Color_White);
				}
			}
		}

		public static void SetToggleEnable(Toggle tgl, bool enable)
		{
			CUIUtility.SetToggleEnable(tgl, enable, CUIUtility.s_Text_Color_White);
		}

		public static void OpenFps()
		{
		}

		public static void SetTextContent(GameObject textObj, string strContent)
		{
			if (textObj == null || strContent == null)
			{
				return;
			}
			Text component = textObj.transform.GetComponent<Text>();
			if (component != null)
			{
				component.text = strContent;
			}
		}

		public static void SetTextContent(Transform textObjTrans, string strContent)
		{
			if (textObjTrans == null || strContent == null)
			{
				return;
			}
			Text component = textObjTrans.GetComponent<Text>();
			if (component != null)
			{
				component.text = strContent;
			}
		}

		public static void SetObjActive(GameObject targetObj, bool isActive)
		{
			if (targetObj != null)
			{
				targetObj.CustomSetActive(isActive);
			}
		}

		public static void SetObjActive(Transform targetTrans, bool isActive)
		{
			if (targetTrans != null)
			{
				targetTrans.gameObject.CustomSetActive(isActive);
			}
		}

		public static void DestoryObj(GameObject target, float delay = 0.1f)
		{
			if (target != null)
			{
				target.transform.SetParent(null);
				target.CustomSetActive(false);
				UnityEngine.Object.Destroy(target, delay);
			}
		}

		public static CUIEventScript GetUIEventScript(Transform trans)
		{
			if (trans != null)
			{
				return trans.GetComponent<CUIEventScript>();
			}
			return null;
		}

		public static CUIListScript GetUIListScript(Transform trans)
		{
			if (trans != null)
			{
				return trans.GetComponent<CUIListScript>();
			}
			return null;
		}

		public static CUITimerScript GetUITimerScript(Transform trans)
		{
			if (trans != null)
			{
				return trans.GetComponent<CUITimerScript>();
			}
			return null;
		}

		public static void SetTextColorSize(GameObject textObj, Color color, enFontSize size)
		{
			if (textObj == null)
			{
				return;
			}
			Text component = textObj.transform.GetComponent<Text>();
			if (component != null)
			{
				component.color = color;
				component.fontSize = (int)size;
			}
		}

		public static void PlayAnimation(Transform trans, string aniName = null)
		{
			if (trans == null)
			{
				return;
			}
			Animation component = trans.GetComponent<Animation>();
			if (component != null)
			{
				if (string.IsNullOrEmpty(aniName))
				{
					component.Play();
				}
				else
				{
					component.Play(aniName);
				}
			}
		}

		public static void InitMenuPanel(GameObject listObj, string[] titleList, int selectIndex, bool isDispatchEvent = true)
		{
			if (listObj != null)
			{
				CUIListScript component = listObj.GetComponent<CUIListScript>();
				component.SetElementAmount(titleList.Length);
				for (int i = 0; i < component.m_elementAmount; i++)
				{
					CUIListElementScript elemenet = component.GetElemenet(i);
					Text component2 = elemenet.gameObject.transform.Find("Text").GetComponent<Text>();
					component2.text = titleList[i];
				}
				component.SelectElement(-1, false);
				component.SelectElement(selectIndex, isDispatchEvent);
			}
		}

		public static void PlayAnimation(GameObject target, string aniName, bool forceRewind = false)
		{
			if (target == null)
			{
				return;
			}
			CUIAnimationScript component = target.GetComponent<CUIAnimationScript>();
			if (component == null)
			{
				return;
			}
			component.PlayAnimation(aniName, forceRewind);
		}

		public static void PlayAnimator(GameObject target, string stateName)
		{
			CUIAnimatorScript cUIAnimatorScript = (!(target != null)) ? null : target.GetComponent<CUIAnimatorScript>();
			if (cUIAnimatorScript == null)
			{
				return;
			}
			cUIAnimatorScript.PlayAnimator(stateName);
		}

		public static string GetSkillDesc(int skillId, CHeroInfo heroInfo, int skillLevel, uint heroId)
		{
			ResSkillCfgInfo skillCfgInfo = CSkillData.GetSkillCfgInfo(skillId);
			if (skillCfgInfo == null)
			{
				return string.Empty;
			}
			string text = StringHelper.UTF8BytesToString(ref skillCfgInfo.szSkillDesc);
			string[] escapeString = CSkillData.GetEscapeString(text);
			if (escapeString != null)
			{
				for (int i = 0; i < escapeString.Length; i++)
				{
					int num = CSkillData.CalcEscapeValue(escapeString[i], heroInfo, skillLevel, 1, heroId);
					string newValue = string.Empty;
					if (num != 0)
					{
						newValue = num.ToString();
					}
					text = text.Replace("[" + escapeString[i] + "]", newValue);
				}
			}
			return text;
		}

		public static string GetSkillDesc(int skillId, ValueDataInfo[] valueData, int skillLevel, uint heroId)
		{
			ResSkillCfgInfo skillCfgInfo = CSkillData.GetSkillCfgInfo(skillId);
			if (skillCfgInfo == null)
			{
				return string.Empty;
			}
			string text = StringHelper.UTF8BytesToString(ref skillCfgInfo.szSkillDesc);
			string[] escapeString = CSkillData.GetEscapeString(text);
			if (escapeString != null)
			{
				for (int i = 0; i < escapeString.Length; i++)
				{
					int num = CSkillData.CalcEscapeValue(escapeString[i], valueData, skillLevel, 1, heroId);
					string newValue = string.Empty;
					if (num != 0)
					{
						newValue = num.ToString();
					}
					text = text.Replace("[" + escapeString[i] + "]", newValue);
				}
			}
			return text;
		}

		public static string GetSkillDesc(string skillDesc, ValueDataInfo[] valueData, int skillLevel, int heroSoulLevel, uint heroId)
		{
			string text = skillDesc;
			string[] escapeString = CSkillData.GetEscapeString(text);
			skillLevel = ((skillLevel >= 1) ? skillLevel : 1);
			if (escapeString != null)
			{
				for (int i = 0; i < escapeString.Length; i++)
				{
					text = text.Replace("[" + escapeString[i] + "]", CSkillData.CalcEscapeValue(escapeString[i], valueData, skillLevel, heroSoulLevel, heroId).ToString());
				}
			}
			return text;
		}

		private static void SetHeroValueArr(uint heroId, ref ValueDataInfo[] dataArr)
		{
			if (CUICommonSystem.s_heroId == heroId)
			{
				return;
			}
			CUICommonSystem.s_heroId = heroId;
			ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(heroId);
			if (dataByKey == null)
			{
				return;
			}
			CUICommonSystem.SetBaseValue(dataArr[1], dataByKey.iBaseATT);
			CUICommonSystem.SetBaseValue(dataArr[2], dataByKey.iBaseINT);
			CUICommonSystem.SetBaseValue(dataArr[3], dataByKey.iBaseDEF);
			CUICommonSystem.SetBaseValue(dataArr[4], dataByKey.iBaseRES);
			CUICommonSystem.SetBaseValue(dataArr[5], dataByKey.iBaseHP);
			CUICommonSystem.SetBaseValue(dataArr[6], dataByKey.iCritRate);
			CUICommonSystem.SetBaseValue(dataArr[7], 0);
			CUICommonSystem.SetBaseValue(dataArr[8], 0);
			CUICommonSystem.SetBaseValue(dataArr[9], 0);
			CUICommonSystem.SetBaseValue(dataArr[10], 0);
			CUICommonSystem.SetBaseValue(dataArr[11], 0);
			CUICommonSystem.SetBaseValue(dataArr[12], dataByKey.iCritEft);
			CUICommonSystem.SetBaseValue(dataArr[13], 0);
			CUICommonSystem.SetBaseValue(dataArr[14], 0);
			CUICommonSystem.SetBaseValue(dataArr[15], dataByKey.iBaseSpeed);
			CUICommonSystem.SetBaseValue(dataArr[16], dataByKey.iBaseHPAdd);
			CUICommonSystem.SetBaseValue(dataArr[17], 0);
			CUICommonSystem.SetBaseValue(dataArr[18], dataByKey.iBaseAtkSpd);
			CUICommonSystem.SetBaseValue(dataArr[19], 0);
			CUICommonSystem.SetBaseValue(dataArr[20], 0);
			CUICommonSystem.SetBaseValue(dataArr[21], dataByKey.iSightR);
			CUICommonSystem.SetBaseValue(dataArr[22], 0);
			CUICommonSystem.SetBaseValue(dataArr[23], 0);
		}

		public static void SetBaseValue(ValueDataInfo info, int val)
		{
			info.baseValue = val;
			info.growValue = 0;
			info.addValue = 0;
			info.decValue = 0;
			info.addRatio = 0;
			info.decRatio = 0;
		}

		public static string GetSkillDescLobby(string skillDesc, uint heroId)
		{
			CUICommonSystem.SetHeroValueArr(heroId, ref CUICommonSystem.s_heroValArr);
			return CUICommonSystem.GetSkillDesc(skillDesc, CUICommonSystem.s_heroValArr, 1, 1, heroId);
		}

		public static int GetSkillPropertyValue(string skillDesc, uint heroId)
		{
			CUICommonSystem.SetHeroValueArr(heroId, ref CUICommonSystem.s_heroValArr);
			return CSkillData.CalcEscapeValue(skillDesc, CUICommonSystem.s_heroValArr, 1, 1, heroId);
		}

		public static string GetFuncEftDesc(ref ResDT_FuncEft_Obj[] funcEftArr, bool bValueExpand = false)
		{
			string text = string.Empty;
			if (funcEftArr == null || funcEftArr.Length == 0)
			{
				return string.Empty;
			}
			for (int i = 0; i < funcEftArr.Length; i++)
			{
				if (funcEftArr[i].iValue != 0 && (int)funcEftArr[i].wType < CUICommonSystem.s_attNameList.Length)
				{
					if (funcEftArr[i].bValType == 0)
					{
						if (CUICommonSystem.s_pctFuncEftList.IndexOf((uint)funcEftArr[i].wType) != -1)
						{
							if (bValueExpand)
							{
								text += string.Format("{0}<color=#31d840ff>+{1}</color>\n", CUICommonSystem.s_attNameList[(int)funcEftArr[i].wType], CUICommonSystem.GetValuePercent(funcEftArr[i].iValue / 100));
							}
							else
							{
								text += string.Format("{0}<color=#31d840ff>+{1}</color>\n", CUICommonSystem.s_attNameList[(int)funcEftArr[i].wType], CUICommonSystem.GetValuePercent(funcEftArr[i].iValue));
							}
						}
						else if (bValueExpand)
						{
							text += string.Format("{0}<color=#31d840ff>+{1}</color>\n", CUICommonSystem.s_attNameList[(int)funcEftArr[i].wType], (float)funcEftArr[i].iValue / 100f);
						}
						else
						{
							text += string.Format("{0}<color=#31d840ff>+{1}</color>\n", CUICommonSystem.s_attNameList[(int)funcEftArr[i].wType], funcEftArr[i].iValue);
						}
					}
					else if (funcEftArr[i].bValType == 1)
					{
						text += string.Format("{0}<color=#31d840ff>+{1}</color>\n", CUICommonSystem.s_attNameList[(int)funcEftArr[i].wType], CUICommonSystem.GetValuePercent(funcEftArr[i].iValue));
					}
				}
			}
			return text;
		}

		public static void SetListProp(GameObject listObj, ref int[] propArr, ref int[] propPctArr)
		{
			int num = 37;
			if (listObj == null || propArr == null || propPctArr == null || propArr.Length != num || propPctArr.Length != num)
			{
				return;
			}
			int num2 = 0;
			for (int i = 0; i < num; i++)
			{
				if (propArr[i] > 0 || propPctArr[i] > 0)
				{
					num2++;
				}
			}
			CUIListScript component = listObj.GetComponent<CUIListScript>();
			component.SetElementAmount(num2);
			num2 = 0;
			for (int i = 0; i < num; i++)
			{
				if (propArr[i] > 0 || propPctArr[i] > 0)
				{
					CUIListElementScript elemenet = component.GetElemenet(num2);
					Text text = (!(elemenet != null)) ? null : elemenet.gameObject.transform.Find("titleText").GetComponent<Text>();
					Text text2 = (!(elemenet != null)) ? null : elemenet.gameObject.transform.Find("valueText").GetComponent<Text>();
					if (text != null)
					{
						text.text = CUICommonSystem.s_attNameList[i];
					}
					if (text2 != null)
					{
						if (propArr[i] > 0)
						{
							text2.text = string.Format("+{0}", propArr[i]);
						}
						else if (propPctArr[i] > 0)
						{
							text2.text = string.Format("+{0}", CUICommonSystem.GetValuePercent(propPctArr[i]));
						}
					}
					num2++;
				}
			}
		}

		public static void SetListProp(GameObject listObj, ref int[] propArr, ref int[] propPctArr, ref int[] propValAddArr, ref int[] propPctAddArr, bool bShowAdd = true)
		{
			int num = 37;
			if (listObj == null || propArr == null || propPctArr == null || propValAddArr == null || propPctAddArr == null || propArr.Length != num || propPctArr.Length != num || propValAddArr.Length != num || propPctAddArr.Length != num)
			{
				return;
			}
			int num2 = 0;
			for (int i = 0; i < num; i++)
			{
				if (propArr[i] > 0 || propPctArr[i] > 0 || propValAddArr[i] / 10000 > 0 || propPctAddArr[i] > 0)
				{
					num2++;
				}
			}
			CUIListScript component = listObj.GetComponent<CUIListScript>();
			component.SetElementAmount(num2);
			num2 = 0;
			for (int i = 0; i < num; i++)
			{
				if (propArr[i] > 0 || propPctArr[i] > 0 || propValAddArr[i] / 10000 > 0 || propPctAddArr[i] > 0)
				{
					CUIListElementScript elemenet = component.GetElemenet(num2);
					if (elemenet != null)
					{
						Text component2 = elemenet.gameObject.transform.Find("titleText").GetComponent<Text>();
						Text component3 = elemenet.gameObject.transform.Find("valueText").GetComponent<Text>();
						Text component4 = elemenet.gameObject.transform.Find("addValText").GetComponent<Text>();
						GameObject gameObject = elemenet.gameObject.transform.Find("GreenPointer").gameObject;
						DebugHelper.Assert(component4 != null);
						if (component4 != null)
						{
							component4.gameObject.CustomSetActive(bShowAdd);
						}
						gameObject.CustomSetActive(bShowAdd);
						if (component2 != null)
						{
							component2.text = CUICommonSystem.s_attNameList[i];
						}
						if (component3 != null && component4 != null)
						{
							if (propArr[i] > 0 || propValAddArr[i] > 0)
							{
								component3.text = string.Format("{0}", propArr[i]);
								component4.text = string.Format("{0}", propValAddArr[i]);
							}
							else if (propPctArr[i] > 0 || propPctAddArr[i] > 0)
							{
								component3.text = CUICommonSystem.GetValuePercent(propPctArr[i]);
								component4.text = string.Format("{0}", CUICommonSystem.GetValuePercent(propPctAddArr[i]));
							}
						}
					}
					num2++;
				}
			}
		}

		public static void SetHeroSkinQualityPic(CUIFormScript formScript, GameObject qualityObj, uint heroId, uint skinId)
		{
			if (null == qualityObj)
			{
				return;
			}
			ResHeroSkinShop resHeroSkinShop = null;
			GameDataMgr.skinShopInfoDict.TryGetValue(heroId, out resHeroSkinShop);
			if (resHeroSkinShop == null)
			{
				return;
			}
			if (resHeroSkinShop.bIsLimitSkin > 0)
			{
				string prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_SkinQuality_Dir, StringHelper.UTF8BytesToString(ref resHeroSkinShop.szLimitQualityPic));
				CUIUtility.SetImageSprite(qualityObj.GetComponent<Image>(), prefabPath, formScript, true, false, false, false);
			}
			else
			{
				ResSkinQualityPicInfo dataByKey = GameDataMgr.skinQualityPicDatabin.GetDataByKey((uint)resHeroSkinShop.bSkinQuality);
				if (dataByKey == null)
				{
					return;
				}
				string prefabPath2 = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_SkinQuality_Dir, StringHelper.UTF8BytesToString(ref dataByKey.szQualityPicPath));
				CUIUtility.SetImageSprite(qualityObj.GetComponent<Image>(), prefabPath2, formScript, true, false, false, false);
			}
		}

		public static void SetHeroSkinLabelPic(CUIFormScript formScript, GameObject labelObj, uint heroId, uint skinId)
		{
			if (null == labelObj)
			{
				return;
			}
			ResHeroSkinShop resHeroSkinShop = null;
			GameDataMgr.skinShopInfoDict.TryGetValue(CSkinInfo.GetSkinCfgId(heroId, skinId), out resHeroSkinShop);
			if (resHeroSkinShop == null)
			{
				labelObj.CustomSetActive(false);
				return;
			}
			RectTransform rectTransform = labelObj.transform as RectTransform;
			string text = string.Empty;
			if (resHeroSkinShop.bIsLimitSkin > 0)
			{
				if (!string.IsNullOrEmpty(resHeroSkinShop.szLimitLabelPic))
				{
					text = resHeroSkinShop.szLimitLabelPic;
				}
				rectTransform.sizeDelta = new Vector2(120f, 38f);
			}
			else
			{
				ResSkinQualityPicInfo dataByKey = GameDataMgr.skinQualityPicDatabin.GetDataByKey((uint)resHeroSkinShop.bSkinQuality);
				if (dataByKey != null && !string.IsNullOrEmpty(dataByKey.szLabelPicPath))
				{
					text = dataByKey.szLabelPicPath;
				}
				rectTransform.sizeDelta = new Vector2(95f, 46f);
			}
			if (string.IsNullOrEmpty(text))
			{
				labelObj.CustomSetActive(false);
			}
			else
			{
				labelObj.CustomSetActive(true);
				text = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_SkinQuality_Dir, text);
				CUIUtility.SetImageSprite(labelObj.GetComponent<Image>(), text, formScript, true, false, false, false);
			}
		}

		public static void SetEquipIcon(ushort equipId, GameObject equipIcon, CUIFormScript formScript)
		{
			if (null == equipIcon)
			{
				return;
			}
			ResEquipInBattle dataByKey = GameDataMgr.m_equipInBattleDatabin.GetDataByKey((uint)equipId);
			if (dataByKey != null)
			{
				string prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_System_BattleEquip_Dir, dataByKey.szIcon);
				CUIUtility.SetImageSprite(equipIcon.GetComponent<Image>(), prefabPath, formScript, true, false, false, false);
			}
		}

		public static string GetValuePercent(int value)
		{
			if (value % 100 != 0)
			{
				value = value / 10 * 10;
			}
			return string.Format("{0}%", (float)value / 100f);
		}

		public static void OpenCoinNotEnoughTip()
		{
			string text = Singleton<CTextManager>.GetInstance().GetText("Common_GoldCoin_Not_Enough");
			Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(text, enUIEventID.Purchase_OpenBuyCoin, enUIEventID.None, false);
		}

		public static void OpenDianQuanNotEnoughTip()
		{
			string text = Singleton<CTextManager>.GetInstance().GetText("Common_DianQuan_Not_Enough");
			Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(text, enUIEventID.Pay_OpenBuyDianQuanPanel, enUIEventID.None, false);
		}

		public static void OpenApNotEnoughTip()
		{
			string text = Singleton<CTextManager>.GetInstance().GetText("Common_Ap_Not_Enough");
			Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(text, enUIEventID.Purchase_OpenBuyActionPoint, enUIEventID.None, false);
		}

		public static Color GetTextQualityColor(int quality)
		{
			quality = ((quality >= 1) ? quality : 1);
			quality = ((quality <= 5) ? quality : 5);
			return CUIUtility.s_Text_Color_Hero_Advance[quality - 1];
		}

		public static void ShowNewHeroOrSkin(uint heroId, uint skinId, enUIEventID eventID, bool bInitAnima = true, COM_REWARDS_TYPE rewardType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_HERO, bool interactableTransition = false, string prefabPath = null, enFormPriority priority = enFormPriority.Priority1, uint convertCoin = 0u, int experienceDays = 0)
		{
			NewHeroOrSkinParams item = default(NewHeroOrSkinParams);
			if (heroId == 0u && skinId > 0u)
			{
				CSkinInfo.ResolveHeroSkin(skinId, out heroId, out skinId);
			}
			item.heroId = heroId;
			item.skinId = skinId;
			item.eventID = eventID;
			item.bInitAnima = bInitAnima;
			item.rewardType = rewardType;
			item.interactableTransition = interactableTransition;
			item.prefabPath = prefabPath;
			item.priority = priority;
			item.convertCoin = convertCoin;
			item.experienceDays = experienceDays;
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Common_NewHeroOrSkinFormClose, new CUIEventManager.OnUIEventHandler(CUICommonSystem.NewHeroOrSkinFormClose));
			CUICommonSystem.newHeroOrSkinList.Add(item);
			CUICommonSystem.ProcessShowNewHeroOrSkin();
		}

		private static void OnLoadNewHeroOrSkin3DModel(CUIEvent uiEvent)
		{
			CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
			if (!srcFormScript.gameObject.activeSelf)
			{
				return;
			}
			GameObject widget = srcFormScript.GetWidget(0);
			CUI3DImageScript cUI3DImageScript = (!(widget != null)) ? null : widget.GetComponent<CUI3DImageScript>();
			if (CUICommonSystem.s_last3DModelPath != null && cUI3DImageScript != null)
			{
				cUI3DImageScript.RemoveGameObject(CUICommonSystem.s_last3DModelPath);
			}
			uint heroId = uiEvent.m_eventParams.heroId;
			uint tag = (uint)uiEvent.m_eventParams.tag;
			bool flag = Convert.ToBoolean(uiEvent.m_eventParams.tag2);
			ObjNameData heroPrefabPath = CUICommonSystem.GetHeroPrefabPath(heroId, (int)tag, true);
			CUICommonSystem.s_last3DModelPath = heroPrefabPath.ObjectName;
			GameObject gameObject = (!(cUI3DImageScript != null)) ? null : cUI3DImageScript.AddGameObject(CUICommonSystem.s_last3DModelPath, false, false);
			CHeroAnimaSystem instance = Singleton<CHeroAnimaSystem>.GetInstance();
			instance.Set3DModel(gameObject);
			if (gameObject == null)
			{
				CUICommonSystem.s_last3DModelPath = null;
				return;
			}
			if (heroPrefabPath.ActorInfo != null)
			{
				gameObject.transform.localScale = new Vector3(heroPrefabPath.ActorInfo.LobbyScale, heroPrefabPath.ActorInfo.LobbyScale, heroPrefabPath.ActorInfo.LobbyScale);
			}
			if (flag)
			{
				instance.InitAnimatList();
				instance.InitAnimatSoundList(heroId, tag);
				instance.OnModePlayAnima("Come");
			}
		}

		private static void NewHeroOrSkinFormClose(CUIEvent uiEventID)
		{
			CUICommonSystem.s_isNewHeroOrSkinShowing = false;
			if (CUICommonSystem.newHeroOrSkinList.Count == 0)
			{
				Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Common_NewHeroOrSkinFormClose, new CUIEventManager.OnUIEventHandler(CUICommonSystem.NewHeroOrSkinFormClose));
				return;
			}
			NewHeroOrSkinParams newHeroOrSkinParams = CUICommonSystem.newHeroOrSkinList[0];
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(newHeroOrSkinParams.eventID);
			CUICommonSystem.newHeroOrSkinList.RemoveAt(0);
			CUICommonSystem.ProcessShowNewHeroOrSkin();
		}

		private static void ProcessShowNewHeroOrSkin()
		{
			if (CUICommonSystem.s_isNewHeroOrSkinShowing)
			{
				return;
			}
			if (CUICommonSystem.newHeroOrSkinList.Count == 0)
			{
				Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Common_NewHeroOrSkinFormClose, new CUIEventManager.OnUIEventHandler(CUICommonSystem.NewHeroOrSkinFormClose));
				return;
			}
			CUICommonSystem.s_isNewHeroOrSkinShowing = true;
			NewHeroOrSkinParams newHeroOrSkinParams = CUICommonSystem.newHeroOrSkinList[0];
			Singleton<CTopLobbyEntry>.GetInstance().CloseForm();
			CUIFormScript cUIFormScript;
			if (!string.IsNullOrEmpty(newHeroOrSkinParams.prefabPath))
			{
				cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(newHeroOrSkinParams.prefabPath, true, true);
			}
			else
			{
				cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CUICommonSystem.s_newHeroOrSkinPath, false, true);
			}
			if (cUIFormScript == null)
			{
				return;
			}
			MonoSingleton<ShareSys>.GetInstance().m_bShowTimeline = false;
			DynamicShadow.EnableDynamicShow(cUIFormScript.gameObject, true);
			cUIFormScript.SetPriority(newHeroOrSkinParams.priority);
			Transform transform = cUIFormScript.transform.Find("btnGroup/Button_Share");
			if (transform != null)
			{
				if (CSysDynamicBlock.bSocialBlocked || MonoSingleton<ShareSys>.GetInstance().m_bHide)
				{
					transform.gameObject.CustomSetActive(false);
				}
				else
				{
					transform.gameObject.CustomSetActive(true);
				}
			}
			GameObject widget = cUIFormScript.GetWidget(2);
			GameObject widget2 = cUIFormScript.GetWidget(3);
			GameObject widget3 = cUIFormScript.GetWidget(6);
			GameObject widget4 = cUIFormScript.GetWidget(7);
			GameObject widget5 = cUIFormScript.GetWidget(8);
			GameObject widget6 = cUIFormScript.GetWidget(1);
			GameObject widget7 = cUIFormScript.GetWidget(5);
			COM_REWARDS_TYPE rewardType = newHeroOrSkinParams.rewardType;
			if (rewardType != COM_REWARDS_TYPE.COM_REWARDS_TYPE_HERO)
			{
				if (rewardType == COM_REWARDS_TYPE.COM_REWARDS_TYPE_SKIN)
				{
					if (widget != null)
					{
						widget.CustomSetActive(false);
					}
					if (widget2 != null)
					{
						if (newHeroOrSkinParams.experienceDays > 0)
						{
							widget2.CustomSetActive(false);
							widget4.CustomSetActive(true);
							widget5.CustomSetActive(true);
							widget5.GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("ExpCard_NewHeroOrSkin_ExpTime", new string[]
							{
								newHeroOrSkinParams.experienceDays.ToString()
							});
						}
						else
						{
							widget2.CustomSetActive(true);
							widget4.CustomSetActive(false);
							widget5.CustomSetActive(false);
						}
					}
					if (newHeroOrSkinParams.convertCoin > 0u && widget2 != null)
					{
						widget2.CustomSetActive(false);
						widget7.CustomSetActive(true);
						GameObject gameObject = widget7.transform.FindChild("ConvertCoinText").gameObject;
						gameObject.CustomSetActive(true);
						Text component = gameObject.GetComponent<Text>();
						if (newHeroOrSkinParams.rewardType == COM_REWARDS_TYPE.COM_REWARDS_TYPE_SKIN)
						{
							component.text = string.Format(Singleton<CTextManager>.GetInstance().GetText("SkinConvertItem"), newHeroOrSkinParams.convertCoin);
						}
					}
					else
					{
						if (widget4 != null && !widget4.activeSelf)
						{
							widget2.CustomSetActive(true);
						}
						widget7.CustomSetActive(false);
					}
					if (widget6 != null)
					{
						widget6.CustomSetActive(true);
						ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(newHeroOrSkinParams.heroId, newHeroOrSkinParams.skinId);
						if (heroSkin != null)
						{
							Text component2 = widget6.GetComponent<Text>();
							if (component2 != null)
							{
								component2.text = StringHelper.UTF8BytesToString(ref heroSkin.szSkinName);
							}
							MonoSingleton<ShareSys>.instance.m_ShareInfo.heroId = newHeroOrSkinParams.heroId;
							MonoSingleton<ShareSys>.instance.m_ShareInfo.skinId = newHeroOrSkinParams.skinId;
							MonoSingleton<ShareSys>.instance.m_ShareInfo.rewardType = newHeroOrSkinParams.rewardType;
							MonoSingleton<ShareSys>.instance.m_ShareInfo.beginDownloadTime = Time.time;
							MonoSingleton<ShareSys>.instance.m_ShareInfo.shareSkinUrl = heroSkin.szShareSkinUrl;
							ResHeroSkinShop resHeroSkinShop = null;
							GameDataMgr.skinShopInfoDict.TryGetValue(heroSkin.dwID, out resHeroSkinShop);
							if (!string.IsNullOrEmpty(MonoSingleton<ShareSys>.instance.m_ShareInfo.shareSkinUrl))
							{
								if (transform != null)
								{
									transform.gameObject.CustomSetActive(true);
								}
								CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
								if (masterRoleInfo != null)
								{
									masterRoleInfo.m_nHeroSkinCount++;
								}
								ShareSys.CLoadReq loadReq = default(ShareSys.CLoadReq);
								loadReq.m_Url = MonoSingleton<ShareSys>.instance.m_ShareInfo.shareSkinUrl;
								loadReq.m_CachePath = MonoSingleton<ShareSys>.instance.m_SharePicCDNCachePath;
								loadReq.m_LoadError = ShareSys.ELoadError.None;
								loadReq.m_Type = 2;
								MonoSingleton<ShareSys>.GetInstance().PreLoadShareSkinImage(loadReq);
							}
							else if (transform != null)
							{
								transform.gameObject.CustomSetActive(false);
							}
						}
						else if (transform != null)
						{
							transform.gameObject.CustomSetActive(false);
						}
						if (transform != null && CSysDynamicBlock.bSocialBlocked)
						{
							transform.gameObject.CustomSetActive(false);
						}
					}
				}
			}
			else
			{
				if (transform != null)
				{
					if (CSysDynamicBlock.bSocialBlocked || MonoSingleton<ShareSys>.GetInstance().m_bHide || newHeroOrSkinParams.experienceDays > 0)
					{
						transform.gameObject.CustomSetActive(false);
					}
					else
					{
						transform.gameObject.CustomSetActive(true);
					}
				}
				if (widget != null)
				{
					if (newHeroOrSkinParams.experienceDays > 0)
					{
						widget.CustomSetActive(false);
						widget3.CustomSetActive(true);
						widget5.CustomSetActive(true);
						widget5.GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("ExpCard_NewHeroOrSkin_ExpTime", new string[]
						{
							newHeroOrSkinParams.experienceDays.ToString()
						});
					}
					else
					{
						widget.CustomSetActive(true);
						widget3.CustomSetActive(false);
						widget5.CustomSetActive(false);
					}
				}
				if (widget2 != null)
				{
					widget2.CustomSetActive(false);
				}
				if (newHeroOrSkinParams.convertCoin > 0u && widget != null)
				{
					widget.CustomSetActive(false);
					widget7.CustomSetActive(true);
					GameObject gameObject2 = widget7.transform.FindChild("ConvertCoinText").gameObject;
					gameObject2.CustomSetActive(true);
					Text component3 = gameObject2.GetComponent<Text>();
					if (newHeroOrSkinParams.rewardType == COM_REWARDS_TYPE.COM_REWARDS_TYPE_HERO)
					{
						component3.text = string.Format(Singleton<CTextManager>.GetInstance().GetText("HeroConvertItem"), newHeroOrSkinParams.convertCoin);
					}
				}
				else
				{
					if (widget3 != null && !widget3.activeSelf)
					{
						widget.CustomSetActive(true);
					}
					widget7.CustomSetActive(false);
				}
				if (widget6 != null)
				{
					widget6.CustomSetActive(true);
					IHeroData heroData = CHeroDataFactory.CreateHeroData(newHeroOrSkinParams.heroId);
					Text component4 = widget6.GetComponent<Text>();
					component4.text = heroData.heroName;
				}
				MonoSingleton<ShareSys>.instance.m_ShareInfo.heroId = newHeroOrSkinParams.heroId;
				MonoSingleton<ShareSys>.instance.m_ShareInfo.skinId = newHeroOrSkinParams.skinId;
				MonoSingleton<ShareSys>.instance.m_ShareInfo.rewardType = newHeroOrSkinParams.rewardType;
				MonoSingleton<ShareSys>.instance.m_ShareInfo.shareSkinUrl = MonoSingleton<ShareSys>.instance.m_ShareInfo.heroId.ToString();
				ShareSys.CLoadReq loadReq2 = default(ShareSys.CLoadReq);
				loadReq2.m_Url = MonoSingleton<ShareSys>.instance.m_ShareInfo.shareSkinUrl;
				loadReq2.m_CachePath = MonoSingleton<ShareSys>.instance.m_SharePicCDNCachePath;
				loadReq2.m_LoadError = ShareSys.ELoadError.None;
				loadReq2.m_Type = 1;
				MonoSingleton<ShareSys>.GetInstance().PreLoadShareSkinImage(loadReq2);
			}
			cUIFormScript.m_eventIDs[1] = enUIEventID.Common_NewHeroOrSkinFormClose;
			GameObject widget8 = cUIFormScript.GetWidget(4);
			if (newHeroOrSkinParams.interactableTransition && widget8 != null)
			{
				CUIAnimatorScript cUIAnimatorScript = widget8.GetComponent<CUIAnimatorScript>();
				if (cUIAnimatorScript == null)
				{
					cUIAnimatorScript = widget8.AddComponent<CUIAnimatorScript>();
					if (cUIAnimatorScript != null)
					{
						cUIAnimatorScript.Initialize(cUIFormScript);
					}
				}
				if (cUIAnimatorScript != null)
				{
					cUIAnimatorScript.PlayAnimator("Interactable_Enabled");
				}
			}
			CUITimerScript component5 = cUIFormScript.GetComponent<CUITimerScript>();
			if (component5 != null)
			{
				component5.SetTotalTime(0.38f);
				component5.m_eventIDs[1] = enUIEventID.HeroSkin_LoadNewHeroOrSkin3DModel;
				stUIEventParams stUIEventParams = default(stUIEventParams);
				stUIEventParams.heroId = newHeroOrSkinParams.heroId;
				stUIEventParams.tag = (int)newHeroOrSkinParams.skinId;
				stUIEventParams.tag2 = Convert.ToInt32(newHeroOrSkinParams.bInitAnima);
				component5.m_eventParams[1] = stUIEventParams;
				component5.StartTimer();
			}
		}

		public static bool ShowSymbol(CUseableContainer container, enUIEventID eventID = enUIEventID.None)
		{
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CUICommonSystem.s_newSymbolFormPath, false, true);
			if (!(cUIFormScript != null) || !(cUIFormScript.gameObject != null) || container == null)
			{
				Singleton<CUIManager>.GetInstance().CloseForm(CUICommonSystem.s_newSymbolFormPath);
				return false;
			}
			int curUseableCount = container.GetCurUseableCount();
			GameObject gameObject = Utility.FindChild(cUIFormScript.gameObject, "Panel_NewSymbol/ContentOne");
			GameObject gameObject2 = Utility.FindChild(cUIFormScript.gameObject, "Panel_NewSymbol/ContentMulti");
			cUIFormScript.m_eventIDs[1] = eventID;
			if (curUseableCount == 0)
			{
				Singleton<CUIManager>.GetInstance().CloseForm(CUICommonSystem.s_newSymbolFormPath);
				return false;
			}
			GameObject gameObject3;
			if (curUseableCount == 1)
			{
				gameObject.CustomSetActive(true);
				gameObject2.CustomSetActive(false);
				gameObject3 = gameObject;
			}
			else
			{
				gameObject.CustomSetActive(false);
				gameObject2.CustomSetActive(true);
				gameObject3 = Utility.FindChild(gameObject2, "ScrollRect/Container/Content");
				if (gameObject3 != null)
				{
					Utility.SetChildrenActive(gameObject3, false);
				}
			}
			if (gameObject3 == null)
			{
				Singleton<CUIManager>.GetInstance().CloseForm(CUICommonSystem.s_newSymbolFormPath);
				return false;
			}
			for (int i = 0; i < curUseableCount; i++)
			{
				GameObject gameObject4 = Utility.FindChild(gameObject3, string.Format("NewSymbol{0}", i));
				if (gameObject4 != null)
				{
					gameObject4.CustomSetActive(true);
					Image componetInChild = Utility.GetComponetInChild<Image>(gameObject4, "Icon_Img");
					Text componetInChild2 = Utility.GetComponetInChild<Text>(gameObject4, "Symbol_Name");
					Text componetInChild3 = Utility.GetComponetInChild<Text>(gameObject4, "SymbolDesc");
					if (componetInChild == null || componetInChild2 == null || componetInChild3 == null)
					{
						return false;
					}
					CSymbolItem cSymbolItem = container.GetUseableByIndex(i) as CSymbolItem;
					if (cSymbolItem != null)
					{
						CUIUtility.SetImageSprite(componetInChild, cSymbolItem.GetIconPath(), cUIFormScript, true, false, false, false);
						componetInChild2.text = cSymbolItem.m_name;
						componetInChild3.text = CSymbolSystem.GetSymbolAttString(cSymbolItem, true);
					}
				}
			}
			return true;
		}

		public static void OpenGoldCoinNotEnoughTip()
		{
			string text = Singleton<CTextManager>.GetInstance().GetText("Common_GoldCoin_Not_Enough_2");
			Singleton<CUIManager>.GetInstance().OpenMessageBox(text, false);
		}

		public static void OpenBurningCoinNotEnoughTip()
		{
			string text = Singleton<CTextManager>.GetInstance().GetText("Common_Burning_Coin_Not_Enough");
			Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(text, enUIEventID.Shop_GetBurningCoinConfirm, enUIEventID.None, false);
		}

		public static void OpenArenaCoinNotEnoughTip()
		{
			string text = Singleton<CTextManager>.GetInstance().GetText("Common_Arena_Coin_Not_Enough");
			Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(text, enUIEventID.Shop_GetArenaCoinConfirm, enUIEventID.None, false);
		}

		public static void OpenGuildCoinNotEnoughTip()
		{
			string text = Singleton<CTextManager>.GetInstance().GetText("Common_Guild_Coin_Not_Enough");
			Singleton<CUIManager>.GetInstance().OpenMessageBox(text, false);
		}

		public static string GetCountFormatString(int curCnt, int totalCnt, bool bShowTotalCnt = true)
		{
			string result = string.Empty;
			if (curCnt < totalCnt)
			{
				if (bShowTotalCnt)
				{
					result = string.Format("<color=#c60e00ff>{0}</color><color=#cecfe1ff>/{1}</color>", curCnt, totalCnt);
				}
				else
				{
					result = string.Format("<color=#c60e00ff>{0}</color>", curCnt);
				}
			}
			else if (bShowTotalCnt)
			{
				result = string.Format("<color=#cecfe1ff>{0}/{1}</color>", curCnt, totalCnt);
			}
			else
			{
				result = string.Format("<color=#cecfe1ff>{0}</color>", curCnt);
			}
			return result;
		}

		public static void SetGetInfoToList(CUIFormScript formScript, CUIListScript list, CUseable itemUseable)
		{
			ResDT_ItemSrc_Info[] astSrcInfo;
			if (itemUseable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP)
			{
				astSrcInfo = ((CItem)itemUseable).m_itemData.astSrcInfo;
			}
			else if (itemUseable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMEQUIP)
			{
				astSrcInfo = ((CEquip)itemUseable).m_equipData.astSrcInfo;
			}
			else
			{
				if (itemUseable.m_type != COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL)
				{
					return;
				}
				astSrcInfo = ((CSymbolItem)itemUseable).m_SymbolData.astSrcInfo;
			}
			ResDT_ItemSrc_Info[] array;
			if (CSysDynamicBlock.bLobbyEntryBlocked)
			{
				ListView<ResDT_ItemSrc_Info> listView = new ListView<ResDT_ItemSrc_Info>();
				for (int i = 0; i < astSrcInfo.Length; i++)
				{
					ResDT_ItemSrc_Info resDT_ItemSrc_Info = astSrcInfo[i];
					listView.Add((resDT_ItemSrc_Info.bType != 2 && resDT_ItemSrc_Info.bType != 3) ? resDT_ItemSrc_Info : null);
				}
				array = LinqS.ToArray<ResDT_ItemSrc_Info>(listView);
			}
			else
			{
				array = astSrcInfo;
			}
			int num = 0;
			for (int j = 0; j < array.Length; j++)
			{
				if (array[j] == null || array[j].bType == 0)
				{
					break;
				}
				num++;
			}
			if (list != null)
			{
				list.SetElementAmount(num);
				for (int k = 0; k < num; k++)
				{
					GameObject gameObject = list.GetElemenet(k).gameObject;
					GameObject obj = Utility.FindChild(gameObject, "Item_Property");
					GameObject gameObject2 = Utility.FindChild(gameObject, "Item_Shop");
					byte bType = array[k].bType;
					int dwID = (int)array[k].dwID;
					bool flag = false;
					string errorStr = string.Empty;
					stUIEventParams eventParams = default(stUIEventParams);
					eventParams.itemGetInfoParams = default(stItemGetInfoParams);
					eventParams.itemGetInfoParams.getType = bType;
					if (bType == 1)
					{
						obj.CustomSetActive(true);
						gameObject2.CustomSetActive(false);
						Text component = Utility.FindChild(gameObject, "Item_Property/Chapter").GetComponent<Text>();
						Text component2 = Utility.FindChild(gameObject, "Item_Property/Game_Name").GetComponent<Text>();
						Text component3 = Utility.FindChild(gameObject, "Item_Property/Game_Difficulty").GetComponent<Text>();
						ResLevelCfgInfo dataByKey = GameDataMgr.levelDatabin.GetDataByKey((long)dwID);
						DebugHelper.Assert(dataByKey != null, "ResLevelCfgInfo[{0}] can not be find!", new object[]
						{
							dwID
						});
						flag = Singleton<CAdventureSys>.GetInstance().IsLevelOpen(dataByKey.iCfgID);
						eventParams.itemGetInfoParams.levelInfo = dataByKey;
						eventParams.itemGetInfoParams.isCanDo = flag;
						if (!flag)
						{
							eventParams.itemGetInfoParams.errorStr = Singleton<CTextManager>.instance.GetText("Hero_Level_Not_Open");
						}
						component.text = Singleton<CTextManager>.instance.GetText("PVE_Level_Chapter_Number", new string[]
						{
							dataByKey.iChapterId.ToString()
						});
						component2.text = Utility.UTF8Convert(dataByKey.szName);
						component3.gameObject.CustomSetActive(false);
						uint num2 = (!flag) ? 0u : Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().pveLevelDetail[(int)(dataByKey.bLevelDifficulty - 1)].ChapterDetailList[dataByKey.iChapterId - 1].LevelDetailList[(int)(dataByKey.bLevelNo - 1)].PlayNum;
						uint dwChallengeNum = dataByKey.dwChallengeNum;
						if (dataByKey.bLevelDifficulty == 2)
						{
							component3.text = Singleton<CTextManager>.instance.GetText("Hero_Info_Text4", new string[]
							{
								(dwChallengeNum - num2).ToString(),
								dwChallengeNum.ToString()
							});
							component3.gameObject.CustomSetActive(true);
						}
					}
					else if (bType == 2)
					{
						RES_SHOP_TYPE rES_SHOP_TYPE = (RES_SHOP_TYPE)dwID;
						RES_SPECIALFUNCUNLOCK_TYPE rES_SPECIALFUNCUNLOCK_TYPE = RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_NONE;
						obj.CustomSetActive(false);
						gameObject2.CustomSetActive(true);
						gameObject2.GetComponentInChildren<Text>().text = Singleton<CTextManager>.instance.GetText(string.Format("Shop_Type_{0}", dwID));
						if (rES_SHOP_TYPE == RES_SHOP_TYPE.RES_SHOPTYPE_ARENA)
						{
							rES_SPECIALFUNCUNLOCK_TYPE = RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_ARENA;
						}
						else if (rES_SHOP_TYPE == RES_SHOP_TYPE.RES_SHOPTYPE_BURNING)
						{
							rES_SPECIALFUNCUNLOCK_TYPE = RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_LIUGUOYUANZHENG;
						}
						else if (rES_SHOP_TYPE == RES_SHOP_TYPE.RES_SHOPTYPE_PVPSYMBOL)
						{
							rES_SPECIALFUNCUNLOCK_TYPE = RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_SYMBOLSHOP;
						}
						else if (rES_SHOP_TYPE == RES_SHOP_TYPE.RES_SHOPTYPE_GUILD)
						{
							rES_SPECIALFUNCUNLOCK_TYPE = RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_GUILDSHOP;
						}
						else if (rES_SHOP_TYPE == RES_SHOP_TYPE.RES_SHOPTYPE_FIXED)
						{
							rES_SPECIALFUNCUNLOCK_TYPE = RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_SHOP;
						}
						if (rES_SPECIALFUNCUNLOCK_TYPE == RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_NONE || Singleton<CFunctionUnlockSys>.instance.FucIsUnlock(rES_SPECIALFUNCUNLOCK_TYPE))
						{
							flag = true;
						}
						else
						{
							ResSpecialFucUnlock dataByKey2 = GameDataMgr.specialFunUnlockDatabin.GetDataByKey((uint)rES_SPECIALFUNCUNLOCK_TYPE);
							errorStr = Utility.UTF8Convert(dataByKey2.szLockedTip);
						}
						eventParams.itemGetInfoParams.isCanDo = flag;
						eventParams.itemGetInfoParams.errorStr = errorStr;
						eventParams.tag = dwID;
					}
					else if (bType == 3)
					{
						flag = true;
						eventParams.itemGetInfoParams.isCanDo = flag;
						eventParams.itemGetInfoParams.errorStr = string.Empty;
						obj.CustomSetActive(false);
						gameObject2.CustomSetActive(true);
						gameObject2.GetComponentInChildren<Text>().text = Singleton<CTextManager>.instance.GetText("Mall");
					}
					gameObject.GetComponent<CUIEventScript>().SetUIEvent(enUIEventType.Click, enUIEventID.Tips_ItemSourceElementClick, eventParams);
				}
				list.gameObject.CustomSetActive(true);
			}
		}

		public static void SetMaterialDirectBuy(CUIFormScript form, GameObject target, CUseable useable, bool autoHide = true)
		{
			if (target == null)
			{
				return;
			}
			if ((useable == null || useable.m_stackCount == 0) && autoHide && target.activeSelf)
			{
				target.CustomSetActive(false);
			}
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null || useable == null)
			{
				return;
			}
			if (useable.m_dianQuanDirectBuy == 0u && autoHide && target.activeSelf)
			{
				target.CustomSetActive(false);
			}
			if (useable.m_stackCount > (int)masterRoleInfo.MaterialDirectBuyLimit)
			{
				useable.m_stackCount = (int)masterRoleInfo.MaterialDirectBuyLimit;
			}
			CUIEventScript cUIEventScript = target.GetComponent<CUIEventScript>();
			if (cUIEventScript == null)
			{
				cUIEventScript = target.AddComponent<CUIEventScript>();
				cUIEventScript.Initialize(form);
			}
			target.CustomSetActive(true);
			cUIEventScript.SetUIEvent(enUIEventType.Click, enUIEventID.HeroInfo_Material_Direct_Buy, new stUIEventParams
			{
				iconUseable = useable
			});
		}

		public static void OpenUrl(string strUrl, bool isOpenInGame = false, int dir = 0)
		{
			if (isOpenInGame)
			{
				IApolloCommonService apolloCommonService = IApollo.Instance.GetService(8) as IApolloCommonService;
				if (apolloCommonService != null)
				{
					apolloCommonService.OpenUrl(strUrl, (APOLLO_SCREENDIR)dir);
				}
			}
			else
			{
				Application.OpenURL(strUrl);
			}
		}

		public static void RefreshExperienceHeroLeftTime(GameObject txtHeroLeftTimeGo, GameObject timerGo, uint heroId)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			txtHeroLeftTimeGo.CustomSetActive(true);
			int experienceHeroLeftTime = masterRoleInfo.GetExperienceHeroLeftTime(heroId);
			string timeSpanFormatString = Utility.GetTimeSpanFormatString(experienceHeroLeftTime);
			if (Utility.IsOverOneDay(experienceHeroLeftTime))
			{
				txtHeroLeftTimeGo.GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("ExpCard_Hero_ExpTime") + "<color=#ffb900>" + timeSpanFormatString + "</color>";
			}
			else
			{
				timerGo.CustomSetActive(true);
				txtHeroLeftTimeGo.GetComponent<Text>().text = Singleton<CTextManager>.GetInstance().GetText("ExpCard_Hero_ExpTime");
				CUITimerScript component = timerGo.GetComponent<CUITimerScript>();
				component.SetTotalTime((float)experienceHeroLeftTime);
				component.StartTimer();
			}
		}

		public static void RefreshExperienceSkinLeftTime(GameObject txtSkinLeftTimeGo, GameObject timerGo, uint heroId, uint skinId, string tipText = null, bool isTipTextDisplayedWithTwoLines = false)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo.IsValidExperienceSkin(heroId, skinId))
			{
				txtSkinLeftTimeGo.CustomSetActive(true);
				int experienceSkinLeftTime = masterRoleInfo.GetExperienceSkinLeftTime(heroId, skinId);
				string timeSpanFormatString = Utility.GetTimeSpanFormatString(experienceSkinLeftTime);
				if (string.IsNullOrEmpty(tipText))
				{
					tipText = Singleton<CTextManager>.GetInstance().GetText("ExpCard_Skin_ExpTime");
				}
				if (Utility.IsOverOneDay(experienceSkinLeftTime))
				{
					timerGo.CustomSetActive(false);
					txtSkinLeftTimeGo.GetComponent<Text>().text = string.Concat(new string[]
					{
						tipText,
						(!isTipTextDisplayedWithTwoLines) ? string.Empty : "\n",
						"<color=#ffb900>",
						timeSpanFormatString,
						"</color>"
					});
				}
				else
				{
					timerGo.CustomSetActive(true);
					txtSkinLeftTimeGo.GetComponent<Text>().text = tipText;
					CUITimerScript component = timerGo.GetComponent<CUITimerScript>();
					component.SetTotalTime((float)experienceSkinLeftTime);
					component.StartTimer();
				}
			}
			else
			{
				txtSkinLeftTimeGo.CustomSetActive(false);
			}
		}

		public static RES_SPECIALFUNCUNLOCK_TYPE EntryTypeToUnlockType(RES_GAME_ENTRANCE_TYPE entranceType)
		{
			RES_SPECIALFUNCUNLOCK_TYPE result = RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_MAX;
			switch (entranceType)
			{
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_SHOP:
				result = RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_SHOP;
				break;
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_PVP:
				result = RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_PVPMODE;
				break;
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_LADDER:
				result = RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_PVPRANK;
				break;
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_ANRENA:
				result = RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_ARENA;
				break;
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_BURNING:
				result = RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_LIUGUOYUANZHENG;
				break;
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_HERO:
				result = RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_HERO;
				break;
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_SYMBOL:
				result = RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_SYMBOL;
				break;
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_PACKAGE:
				result = RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_BAG;
				break;
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_TASK:
				result = RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_TASK;
				break;
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_GUILD:
				result = RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_UNION;
				break;
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_FRIEND:
				result = RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_FRIEND;
				break;
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_SHOP_HERO:
				result = RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_SHOP;
				break;
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_SHOP_SKIN:
				result = RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_SHOP;
				break;
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_SHOP_SYMBOL:
				result = RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_SHOP;
				break;
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_SHOP_DISCOUNT:
				result = RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_SHOP;
				break;
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_SHOP_TREASURE:
				result = RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_SHOP;
				break;
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_SHOP_MISTERY:
				result = RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_BLACKSHOP;
				break;
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_SHOP_DIAMOND_TREASURE:
				result = RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_SHOP;
				break;
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_SYMBOL_MAKE:
				result = RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_SYMBOL;
				break;
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_UNION_BATTLE_ENTRY:
				result = RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_REWARDMATCH;
				break;
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_ADDSKILL:
				result = RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_ADDEDSKILL;
				break;
			}
			return result;
		}

		public static bool JumpForm(RES_GAME_ENTRANCE_TYPE entranceType, int targetId = 0, int targetId2 = 0, List<uint> tagList = null)
		{
			CUIEvent cUIEvent = new CUIEvent();
			switch (entranceType)
			{
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_SHOP:
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Mall_OpenForm);
				break;
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_RECHARGE:
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Pay_OpenBuyDianQuanPanel);
				break;
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_COUPONSSHOP:
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Pay_OpenBuyDianQuanPanel);
				break;
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_PVP:
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Matching_OpenEntry);
				break;
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_LADDER:
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Matching_OpenLadder);
				break;
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_PVE:
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Explore_OpenForm);
				break;
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_ANRENA:
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Arena_OpenForm);
				break;
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_BURNING:
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Burn_OpenForm);
				break;
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_HERO:
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.HeroView_OpenForm);
				break;
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_NOTICE:
				cUIEvent.m_eventID = enUIEventID.Activity_OpenForm;
				cUIEvent.m_eventParams.tag = targetId;
				cUIEvent.m_eventParams.tag2 = targetId2;
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(cUIEvent);
				break;
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_SYMBOL:
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Symbol_OpenForm);
				break;
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_PACKAGE:
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Bag_OpenForm);
				break;
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_TASK:
				cUIEvent.m_eventID = enUIEventID.Task_OpenForm;
				cUIEvent.m_eventParams.tag = 0;
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(cUIEvent);
				break;
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_GUILD:
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Guild_OpenForm);
				break;
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_FRIEND:
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Friend_OpenForm);
				break;
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_SHOP_HERO:
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.HeroInfo_GotoMall);
				break;
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_SHOP_SKIN:
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Mall_GoToSkinTab);
				break;
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_SHOP_SYMBOL:
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Mall_GoToSymbolTab);
				break;
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_SHOP_DISCOUNT:
				cUIEvent.m_eventID = enUIEventID.Mall_Open_Factory_Shop_Tab;
				cUIEvent.m_eventParams.tag2 = targetId;
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(cUIEvent);
				break;
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_SHOP_TREASURE:
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Mall_GotoCouponsTreasureTab);
				break;
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_SHOP_MISTERY:
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Mall_GoToMysteryTab);
				break;
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_SHOP_DIAMOND_TREASURE:
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Mall_GotoDianmondTreasureTab);
				break;
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_NOBE:
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.NOBE_OPEN_Form);
				break;
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_QQVIP:
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.IDIP_QQVIP_OpenWealForm);
				break;
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_SYMBOL_MAKE:
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Symbol_OpenForm_ToMakeTab);
				break;
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_TASK_DAY:
				cUIEvent.m_eventID = enUIEventID.Task_OpenForm;
				cUIEvent.m_eventParams.tag = 1;
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(cUIEvent);
				break;
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_PVE_ADV:
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Adv_OpenChapterForm);
				break;
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_SHOP_NEWPROD:
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Mall_GoToRecommendHeroTab);
				break;
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_PVP_PLAYER:
			{
				CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CMatchingSystem.PATH_MATCHING_ENTRY);
				if (form == null)
				{
					Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Matching_OpenEntry);
					form = Singleton<CUIManager>.GetInstance().GetForm(CMatchingSystem.PATH_MATCHING_ENTRY);
				}
				cUIEvent.m_srcFormScript = form;
				cUIEvent.m_eventID = enUIEventID.Matching_BtnGroup_Click;
				cUIEvent.m_eventParams.tag = 1;
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(cUIEvent);
				break;
			}
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_PVP_COMPUTER:
			{
				CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CMatchingSystem.PATH_MATCHING_ENTRY);
				if (form == null)
				{
					Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Matching_OpenEntry);
					form = Singleton<CUIManager>.GetInstance().GetForm(CMatchingSystem.PATH_MATCHING_ENTRY);
				}
				cUIEvent.m_srcFormScript = form;
				cUIEvent.m_eventID = enUIEventID.Matching_BtnGroup_Click;
				cUIEvent.m_eventParams.tag = 2;
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(cUIEvent);
				break;
			}
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_PVP_TRAIN:
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Matching_GuidePanel);
				break;
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_UNION_BATTLE_ENTRY:
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Union_Battle_ClickEntry);
				break;
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_SEVENCHECKIN:
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.SevenCheck_OpenForm);
				break;
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_ADDSKILL:
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.AddedSkill_OpenForm);
				break;
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_MAIL:
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Mail_OpenForm);
				break;
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_MAILSystem:
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Mail_OpenForm);
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Mail_TabSystem);
				break;
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_HERO_PREVIEW:
				cUIEvent.m_eventID = enUIEventID.HeroInfo_OpenForm;
				cUIEvent.m_eventParams.openHeroFormPar.heroId = (uint)targetId;
				cUIEvent.m_eventParams.openHeroFormPar.openSrc = enHeroFormOpenSrc.HeroBuyClick;
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(cUIEvent);
				break;
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_SKIN_PREVIEW:
				cUIEvent.m_eventID = enUIEventID.HeroInfo_OpenForm;
				cUIEvent.m_eventParams.openHeroFormPar.heroId = (uint)targetId;
				cUIEvent.m_eventParams.openHeroFormPar.skinId = (uint)targetId2;
				cUIEvent.m_eventParams.openHeroFormPar.openSrc = enHeroFormOpenSrc.SkinBuyClick;
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(cUIEvent);
				break;
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_14DayCheck:
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Day14Check_OpenForm);
				break;
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_DRAW_SYMBOL:
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Lottery_Open_Form);
				break;
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_PVP_HISTORY:
				Singleton<CPlayerPvpHistoryController>.instance.OpenDetailPvpHistory(tagList[0], tagList[1]);
				break;
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_XUN_YOU_PAY_ENTRANCE:
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Partner_OpenXunYou_Buy);
				break;
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_BattleTeamMatch:
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Guild_Match_OpenMatchForm);
				break;
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_GiftCenter:
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Mall_GiftCenter_Open);
				break;
			case RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_LOBBY_TV:
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Lobby_GotoBattleWebHome);
				break;
			default:
				return false;
			}
			return true;
		}

		public static void SetHeadItemCell(GameObject headItemCell, string headUrl, int vipLevel, int headBkId)
		{
			if (CSysDynamicBlock.bSocialBlocked)
			{
				return;
			}
			if (headItemCell == null)
			{
				return;
			}
			Transform transform = headItemCell.transform;
			Transform transform2 = transform.FindChild("HeadIcon");
			Transform transform3 = transform.FindChild("HeadFrame");
			Transform transform4 = transform.FindChild("NobeIcon");
			if (transform2 != null)
			{
				transform2.GetComponent<CUIHttpImageScript>().SetImageUrl(headUrl);
			}
			if (transform3 != null)
			{
				MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(transform3.GetComponent<Image>(), headBkId);
			}
			if (transform4 != null)
			{
				MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(transform4.GetComponent<Image>(), vipLevel, false, true, 0uL);
			}
		}

		public static void SetHostHeadItemCell(GameObject headItemCell)
		{
			if (CSysDynamicBlock.bSocialBlocked)
			{
				return;
			}
			if (headItemCell == null)
			{
				return;
			}
			Transform transform = headItemCell.transform;
			Transform transform2 = transform.FindChild("HeadIcon");
			Transform transform3 = transform.FindChild("HeadFrame");
			Transform transform4 = transform.FindChild("NobeIcon");
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return;
			}
			if (transform2 != null)
			{
				transform2.GetComponent<CUIHttpImageScript>().SetImageUrl(masterRoleInfo.HeadUrl);
			}
			if (transform3 != null)
			{
				MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(transform3.GetComponent<Image>(), (int)masterRoleInfo.GetNobeInfo().stGameVipClient.dwHeadIconId);
			}
			if (transform4 != null)
			{
				MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(transform4.GetComponent<Image>(), (int)masterRoleInfo.GetNobeInfo().stGameVipClient.dwCurLevel, false, true, 0uL);
			}
		}

		private static int GetSecUtilOpenOfDay(DateTime dateTimeNow, ResActTime[] ActTime)
		{
			int result = -1;
			if (ActTime != null)
			{
				DateTime dateTime = new DateTime(1970, 1, 1, dateTimeNow.Hour, dateTimeNow.Minute, dateTimeNow.Second);
				for (int i = 0; i < ActTime.Length; i++)
				{
					if (ActTime[i].dwStartTime == 0u && ActTime[i].dwEndTime == 0u)
					{
						break;
					}
					ulong dtVal = (ulong)ActTime[i].dwStartTime + 19700101000000uL;
					ulong num = (ulong)ActTime[i].dwEndTime + 19700101000000uL;
					num = ((num != 19700101240000uL) ? num : 19700101235959uL);
					DateTime dateTime2 = Utility.ULongToDateTime(dtVal);
					DateTime t = Utility.ULongToDateTime(num);
					if (DateTime.Compare(dateTime, dateTime2) < 0)
					{
						result = (int)(dateTime2 - dateTime).TotalSeconds;
						break;
					}
					if (DateTime.Compare(dateTime, dateTime2) >= 0 && DateTime.Compare(dateTime, t) <= 0)
					{
						result = 0;
						break;
					}
				}
			}
			return result;
		}

		public static bool IsMatchOpened(RES_BATTLE_MAP_TYPE mapType, uint mapId)
		{
			ResRewardMatchTimeInfo inMatchTimeInfo = null;
			GameDataMgr.matchTimeInfoDict.TryGetValue(GameDataMgr.GetDoubleKey((uint)mapType, mapId), out inMatchTimeInfo);
			return CUICommonSystem.IsMatchOpened(inMatchTimeInfo);
		}

		public static uint GetMatchOpenTime(RES_BATTLE_MAP_TYPE mapType, uint mapId)
		{
			uint num;
			int num2;
			uint timeUtilOpen = CUICommonSystem.GetTimeUtilOpen(mapType, mapId, out num, out num2);
			return (uint)(CRoleInfo.GetCurrentUTCTime() + (int)timeUtilOpen);
		}

		public static uint GetTimeUtilOpen(RES_BATTLE_MAP_TYPE mapType, uint mapId, out uint utilOpenSec, out int utilOpenDay)
		{
			uint num = 0u;
			int num2 = 0;
			ResRewardMatchTimeInfo inMatchTimeInfo = null;
			GameDataMgr.matchTimeInfoDict.TryGetValue(GameDataMgr.GetDoubleKey((uint)mapType, mapId), out inMatchTimeInfo);
			CUICommonSystem.GetTimeUtilOpen(inMatchTimeInfo, out num, out num2);
			utilOpenSec = num;
			utilOpenDay = num2;
			return num;
		}

		public static bool IsMatchOpened(ResRewardMatchTimeInfo inMatchTimeInfo)
		{
			bool result = false;
			if (inMatchTimeInfo != null)
			{
				ulong ullStartDate = inMatchTimeInfo.ullStartDate;
				ulong ullEndDate = inMatchTimeInfo.ullEndDate;
				DateTime t = Utility.ULongToDateTime(ullStartDate);
				DateTime t2 = Utility.ULongToDateTime(ullEndDate);
				DateTime t3 = Utility.ToUtcTime2Local((long)CRoleInfo.GetCurrentUTCTime());
				if (DateTime.Compare(t3, t) >= 0 && DateTime.Compare(t3, t2) <= 0 && inMatchTimeInfo.bIsOpen != 0)
				{
					result = true;
				}
			}
			return result;
		}

		public static void GetTimeUtilOpen(ResRewardMatchTimeInfo inMatchTimeInfo, out uint utilOpenSec, out int utilOpenDay)
		{
			uint num = 0u;
			int num2 = 0;
			DateTime dateTime = Utility.ToUtcTime2Local((long)CRoleInfo.GetCurrentUTCTime());
			if (inMatchTimeInfo != null)
			{
				RES_CYCLETIME_TYPE bCycleType = (RES_CYCLETIME_TYPE)inMatchTimeInfo.bCycleType;
				if (bCycleType != RES_CYCLETIME_TYPE.RES_CYCLETIME_DAY)
				{
					if (bCycleType == RES_CYCLETIME_TYPE.RES_CYCLETIME_WEEK)
					{
						int bCycleParmNum = (int)inMatchTimeInfo.bCycleParmNum;
						int i = -1;
						while (i < 0)
						{
							for (int j = 0; j < bCycleParmNum; j++)
							{
								if ((ulong)CUICommonSystem.GetDayOfWeek(inMatchTimeInfo.CycleParm[j]) == (ulong)((long)dateTime.DayOfWeek))
								{
									i = CUICommonSystem.GetSecUtilOpenOfDay(dateTime, inMatchTimeInfo.astActTime);
								}
							}
							if (i < 0)
							{
								int newDayDeltaSec = (int)Utility.GetNewDayDeltaSec((int)Utility.ToUtcSeconds(dateTime));
								dateTime = dateTime.AddSeconds((double)newDayDeltaSec);
								num += (uint)newDayDeltaSec;
								num2++;
							}
							else
							{
								num += (uint)i;
							}
						}
					}
				}
				else
				{
					int i = -1;
					while (i < 0)
					{
						i = CUICommonSystem.GetSecUtilOpenOfDay(dateTime, inMatchTimeInfo.astActTime);
						if (i < 0)
						{
							int newDayDeltaSec2 = (int)Utility.GetNewDayDeltaSec((int)Utility.ToUtcSeconds(dateTime));
							dateTime = dateTime.AddSeconds((double)newDayDeltaSec2);
							num += (uint)newDayDeltaSec2;
							num2++;
						}
						else
						{
							num += (uint)i;
						}
					}
				}
			}
			utilOpenDay = num2;
			utilOpenSec = num;
		}

		private static uint GetDayOfWeek(uint weekEnumInMatchTimeInfo)
		{
			if (weekEnumInMatchTimeInfo < 1u)
			{
				DebugHelper.Assert(false, "比赛星期几的枚举值范围是1到7，不能小于1: {0}", new object[]
				{
					weekEnumInMatchTimeInfo
				});
				return 0u;
			}
			return weekEnumInMatchTimeInfo - 1u;
		}

		public static stMatchOpenInfo GetMatchOpenState(RES_BATTLE_MAP_TYPE mapType, uint mapId)
		{
			stMatchOpenInfo result;
			result.descStr = string.Empty;
			result.leftDay = 0;
			result.leftSec = 0u;
			ResRewardMatchTimeInfo resRewardMatchTimeInfo = null;
			GameDataMgr.matchTimeInfoDict.TryGetValue(GameDataMgr.GetDoubleKey((uint)mapType, mapId), out resRewardMatchTimeInfo);
			uint num = 0u;
			int leftDay = 0;
			if (resRewardMatchTimeInfo == null)
			{
				result.matchState = enMatchOpenState.enMatchClose;
				return result;
			}
			if (!CUICommonSystem.IsMatchOpened(resRewardMatchTimeInfo))
			{
				result.matchState = enMatchOpenState.enMatchClose;
			}
			else
			{
				CUICommonSystem.GetTimeUtilOpen(resRewardMatchTimeInfo, out num, out leftDay);
				if (num == 0u)
				{
					result.matchState = enMatchOpenState.enMatchOpen_InActiveTime;
				}
				else
				{
					result.matchState = enMatchOpenState.enMatchOpen_NotInActiveTime;
				}
			}
			result.descStr = resRewardMatchTimeInfo.szTimeTips;
			result.leftSec = num;
			result.leftDay = leftDay;
			return result;
		}

		public static bool IsInSignUpTime(RES_BATTLE_MAP_TYPE mapType, uint mapId)
		{
			ResRewardMatchTimeInfo resRewardMatchTimeInfo = null;
			GameDataMgr.matchTimeInfoDict.TryGetValue(GameDataMgr.GetDoubleKey((uint)mapType, mapId), out resRewardMatchTimeInfo);
			if (!CUICommonSystem.IsMatchOpened(resRewardMatchTimeInfo))
			{
				return false;
			}
			if (resRewardMatchTimeInfo.bCycleType == 2)
			{
				int currentUTCTime = CRoleInfo.GetCurrentUTCTime();
				DateTime dateTime = Utility.ToUtcTime2Local((long)currentUTCTime);
				for (int i = 0; i < (int)resRewardMatchTimeInfo.bSignUpCycleParmNum; i++)
				{
					if ((ulong)CUICommonSystem.GetDayOfWeek(resRewardMatchTimeInfo.SignUpCycleParm[i]) == (ulong)((long)dateTime.DayOfWeek))
					{
						DateTime dateTime2 = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);
						uint num = Utility.ToUtcSeconds(dateTime2);
						uint num2 = num + Utility.GetSecondsFromHourMinuteSecond(resRewardMatchTimeInfo.astSignUpActTime[i].dwStartTime);
						uint num3 = num + Utility.GetSecondsFromHourMinuteSecond(resRewardMatchTimeInfo.astSignUpActTime[i].dwEndTime);
						if ((ulong)num2 <= (ulong)((long)currentUTCTime) && (long)currentUTCTime <= (long)((ulong)num3))
						{
							return true;
						}
					}
				}
				return false;
			}
			DebugHelper.Assert(false, "战队赛时间的周期目前仅支持‘每周’！！！");
			return false;
		}

		public static uint GetGuildMatchNextEndTime()
		{
			uint guildMatchMapId = Singleton<CGuildMatchSystem>.GetInstance().GetGuildMatchMapId();
			ResRewardMatchTimeInfo resRewardMatchTimeInfo;
			GameDataMgr.matchTimeInfoDict.TryGetValue(GameDataMgr.GetDoubleKey(6u, guildMatchMapId), out resRewardMatchTimeInfo);
			if (!CUICommonSystem.IsMatchOpened(resRewardMatchTimeInfo))
			{
				return 0u;
			}
			if (resRewardMatchTimeInfo.bCycleType == 2)
			{
				if (resRewardMatchTimeInfo.bCycleParmNum > 0)
				{
					int[] array = new int[7];
					for (int i = 0; i < array.Length; i++)
					{
						array[i] = -1;
						for (int j = 0; j < (int)resRewardMatchTimeInfo.bCycleParmNum; j++)
						{
							if ((long)i == (long)((ulong)CUICommonSystem.GetDayOfWeek(resRewardMatchTimeInfo.CycleParm[j])))
							{
								array[i] = j;
								break;
							}
						}
					}
					DateTime baseDate = Utility.ToUtcTime2Local((long)CRoleInfo.GetCurrentUTCTime());
					int dayOfWeek = (int)baseDate.DayOfWeek;
					for (int k = dayOfWeek; k <= dayOfWeek + array.Length; k++)
					{
						int num = array[k % array.Length];
						if (num >= 0 && num < resRewardMatchTimeInfo.astActTime.Length)
						{
							uint secondsFromHourMinuteSecond = Utility.GetSecondsFromHourMinuteSecond(resRewardMatchTimeInfo.astActTime[num].dwEndTime);
							if (k != dayOfWeek)
							{
								return CUICommonSystem.GetUtcTime(baseDate.AddDays((double)(k - dayOfWeek)), secondsFromHourMinuteSecond);
							}
							if ((long)(baseDate.Hour * 3600 + baseDate.Minute * 60 + baseDate.Second) < (long)((ulong)secondsFromHourMinuteSecond))
							{
								return CUICommonSystem.GetUtcTime(baseDate, secondsFromHourMinuteSecond);
							}
						}
					}
				}
			}
			else
			{
				DebugHelper.Assert(false, "战队赛时间的周期目前仅支持‘每周’！！！");
			}
			return 0u;
		}

		private static uint GetUtcTime(DateTime baseDate, uint addSeconds)
		{
			DateTime dateTime = new DateTime(baseDate.Year, baseDate.Month, baseDate.Day, 0, 0, 0);
			dateTime = dateTime.AddSeconds(addSeconds);
			return Utility.ToUtcSeconds(dateTime);
		}

		public static void SetRankDisplay(uint rankNumber, Transform rankTransform)
		{
			GameObject gameObject = rankTransform.Find("imgRank1st").gameObject;
			GameObject gameObject2 = rankTransform.Find("imgRank2nd").gameObject;
			GameObject gameObject3 = rankTransform.Find("imgRank3rd").gameObject;
			GameObject gameObject4 = rankTransform.Find("txtRank").gameObject;
			GameObject gameObject5 = rankTransform.Find("txtNotInRank").gameObject;
			CUICommonSystem.SetRankDisplay(rankNumber, gameObject, gameObject2, gameObject3, gameObject4, gameObject5);
		}

		private static void SetRankDisplay(uint rankNumber, GameObject imgRank1st, GameObject imgRank2nd, GameObject imgRank3rd, GameObject txtRank, GameObject txtNotInRank)
		{
			if (rankNumber == 0u)
			{
				imgRank1st.CustomSetActive(false);
				imgRank2nd.CustomSetActive(false);
				imgRank3rd.CustomSetActive(false);
				txtRank.CustomSetActive(false);
				txtNotInRank.CustomSetActive(true);
			}
			else if (rankNumber == 1u)
			{
				imgRank1st.CustomSetActive(true);
				imgRank2nd.CustomSetActive(false);
				imgRank3rd.CustomSetActive(false);
				txtRank.CustomSetActive(false);
				txtNotInRank.CustomSetActive(false);
			}
			else if (rankNumber == 2u)
			{
				imgRank1st.CustomSetActive(false);
				imgRank2nd.CustomSetActive(true);
				imgRank3rd.CustomSetActive(false);
				txtRank.CustomSetActive(false);
				txtNotInRank.CustomSetActive(false);
			}
			else if (rankNumber == 3u)
			{
				imgRank1st.CustomSetActive(false);
				imgRank2nd.CustomSetActive(false);
				imgRank3rd.CustomSetActive(true);
				txtRank.CustomSetActive(false);
				txtNotInRank.CustomSetActive(false);
			}
			else
			{
				imgRank1st.CustomSetActive(false);
				imgRank2nd.CustomSetActive(false);
				imgRank3rd.CustomSetActive(false);
				txtRank.CustomSetActive(true);
				txtNotInRank.CustomSetActive(false);
				txtRank.GetComponent<Text>().text = rankNumber.ToString();
			}
		}

		public static void LoadUIPrefab(string prefabPath, string prefabName, GameObject container, CUIFormScript formScript)
		{
			if (formScript == null || container == null)
			{
				return;
			}
			if (container.transform.Find(prefabName) != null)
			{
				return;
			}
			GameObject uIPrefab = CUICommonSystem.GetUIPrefab(prefabPath);
			uIPrefab.name = prefabName;
			RectTransform component = uIPrefab.GetComponent<RectTransform>();
			component.SetParent(container.transform);
			component.localPosition = Vector3.zero;
			component.localRotation = Quaternion.identity;
			component.localScale = Vector3.one;
			component.anchorMin = new Vector2(0f, 0f);
			component.anchorMax = new Vector2(1f, 1f);
			component.offsetMin = Vector2.zero;
			component.offsetMax = Vector2.zero;
			formScript.InitializeComponent(container);
		}

		public static string GetPlatformArea()
		{
			string result = "1";
			if (ApolloConfig.platform == ApolloPlatform.QQ)
			{
				if (Application.platform == RuntimePlatform.Android)
				{
					result = "1";
				}
				else if (Application.platform == RuntimePlatform.IPhonePlayer)
				{
					result = "2";
				}
			}
			else if (ApolloConfig.platform == ApolloPlatform.Wechat)
			{
				if (Application.platform == RuntimePlatform.Android)
				{
					result = "3";
				}
				else if (Application.platform == RuntimePlatform.IPhonePlayer)
				{
					result = "4";
				}
			}
			return result;
		}

		public static bool IsOpenFormFadeFunc()
		{
			return false;
		}

		public static void OpenFormWithAnimation(string animationName, enUIEventID openFormEvent, stUIEventParams par)
		{
			Singleton<CUIEventManager>.instance.DispatchUIEvent(openFormEvent, par);
		}

		public static void CloseFormWithAnimation(CUIFormScript targetForm, string animationName, bool isPlaySysEntryFadeOut = true)
		{
			if (CUICommonSystem.IsOpenFormFadeFunc())
			{
				CUIAnimationScript component = targetForm.GetComponent<CUIAnimationScript>();
				if (component != null && component.enabled && component.IsHaveAnimation(animationName))
				{
					component.SetAnimationEvent(enAnimationEventType.AnimationEnd, enUIEventID.UI_OnFormCloseWithAnimation, default(stUIEventParams));
					component.PlayAnimation(animationName, false);
					if (isPlaySysEntryFadeOut)
					{
						CUICommonSystem.PlayLobbySysEntryFormFadeOut();
					}
					targetForm.TryEnableInput(false);
				}
				else
				{
					targetForm.Close();
				}
			}
			else
			{
				targetForm.Close();
			}
		}

		protected void OnCloseFormWithFirsEntry(CUIEvent uiEvent)
		{
			CUICommonSystem.CloseFormWithAnimation(uiEvent.m_srcFormScript, CUICommonSystem.s_FormFadeOutAniName, true);
		}

		protected void OnCloseFormWithPopUp(CUIEvent uiEvent)
		{
			CUICommonSystem.CloseFormWithAnimation(uiEvent.m_srcFormScript, CUICommonSystem.s_FormFadeOutAniName, false);
		}

		protected void OnCloseFormWithAnimation(CUIEvent uiEvent)
		{
			if (uiEvent.m_srcWidgetScript != null && uiEvent.m_srcWidgetScript is CUIAnimationScript)
			{
				((CUIAnimationScript)uiEvent.m_srcWidgetScript).StopAnimation();
			}
			if (uiEvent.m_srcFormScript != null)
			{
				uiEvent.m_srcFormScript.ClearFadeAnimationEndEvent();
				uiEvent.m_srcFormScript.Close();
			}
			CUICommonSystem.PlayLobbySysEntryFormFadeIn();
		}

		public static void PlayLobbyRankFormFadeOut()
		{
			if (!CUICommonSystem.IsOpenFormFadeFunc())
			{
				return;
			}
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CLobbySystem.RANKING_BTN_FORM_PATH);
			if (form != null)
			{
				form.ClearFadeAnimationEndEvent();
				CUICommonSystem.PlayAnimation(form.gameObject, CUICommonSystem.s_FormFadeOutAniName, false);
			}
		}

		public static void PlayLobbySysEntryFormFadeOut()
		{
			if (!CUICommonSystem.IsOpenFormFadeFunc())
			{
				return;
			}
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CLobbySystem.SYSENTRY_FORM_PATH);
			if (form != null)
			{
				form.ClearFadeAnimationEndEvent();
				CUICommonSystem.PlayAnimation(form.gameObject, CUICommonSystem.s_FormFadeOutAniName, false);
			}
		}

		public static void PlayLobbySysEntryFormFadeIn()
		{
			if (!CUICommonSystem.IsOpenFormFadeFunc())
			{
				return;
			}
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CLobbySystem.SYSENTRY_FORM_PATH);
			if (form != null)
			{
				form.RePlayFadIn();
			}
		}

		protected void OnResetLobbyFormFadeRecover(CUIEvent evt)
		{
			CUICommonSystem.ResetLobbyFormFadeRecover();
		}

		public static void ResetLobbyFormFadeRecover()
		{
		}

		protected void OnOpenBagWithAni(CUIEvent uiEvent)
		{
			CUICommonSystem.OpenFormWithAnimation(CUICommonSystem.s_FormFadeOutAniName, enUIEventID.Bag_OpenForm, uiEvent.m_eventParams);
		}

		protected void OnOpenSymbolWithAni(CUIEvent uiEvent)
		{
			CUICommonSystem.OpenFormWithAnimation(CUICommonSystem.s_FormFadeOutAniName, enUIEventID.Symbol_OpenForm, uiEvent.m_eventParams);
		}

		protected void OnOpenEquipWithAni(CUIEvent uiEvent)
		{
			CUICommonSystem.OpenFormWithAnimation(CUICommonSystem.s_FormFadeOutAniName, enUIEventID.CustomEquip_OpenForm, uiEvent.m_eventParams);
		}

		protected void OnOpenChengjiuWithAni(CUIEvent uiEvent)
		{
			CUICommonSystem.OpenFormWithAnimation(CUICommonSystem.s_FormFadeOutAniName, enUIEventID.Achievement_Open_Overview_Form, uiEvent.m_eventParams);
		}

		protected void OnOpenBattleTeamWithAni(CUIEvent uiEvent)
		{
			if (Singleton<CGuildSystem>.instance.IsOpenGuildSystem())
			{
				CUICommonSystem.OpenFormWithAnimation(CUICommonSystem.s_FormFadeOutAniName, enUIEventID.Guild_OpenForm, uiEvent.m_eventParams);
			}
		}

		protected void OnOpenShopWithAni(CUIEvent uiEvent)
		{
			if (Singleton<CMallSystem>.instance.IsOpenMall())
			{
				CUICommonSystem.OpenFormWithAnimation(CUICommonSystem.s_FormFadeOutAniName, enUIEventID.Mall_OpenForm, uiEvent.m_eventParams);
			}
		}

		protected void OnOpenSettingWithAni(CUIEvent uiEvent)
		{
			CUICommonSystem.OpenFormWithAnimation(CUICommonSystem.s_FormFadeOutAniName, enUIEventID.Settings_OpenForm, uiEvent.m_eventParams);
		}

		protected void OnOpenHeroWithAni(CUIEvent uiEvent)
		{
			CUICommonSystem.OpenFormWithAnimation(CUICommonSystem.s_FormFadeOutAniName, enUIEventID.HeroView_OpenForm, uiEvent.m_eventParams);
		}

		protected void OnOpenSkillWithAni(CUIEvent uiEvent)
		{
			if (Singleton<CAddSkillSys>.instance.IsOpenAddSkillSys())
			{
				CUICommonSystem.OpenFormWithAnimation(CUICommonSystem.s_FormFadeOutAniName, enUIEventID.AddedSkill_OpenForm, uiEvent.m_eventParams);
			}
		}

		protected void OnOpenChatSettingWithAni(CUIEvent uiEvent)
		{
			CUICommonSystem.OpenFormWithAnimation(CUICommonSystem.s_FormFadeOutAniName, enUIEventID.InBatShortcut_OpenForm, uiEvent.m_eventParams);
		}

		protected void OnOpenMishuWithAni(CUIEvent uiEvent)
		{
			if (Singleton<CTaskSys>.instance.IsOpenTaskSys())
			{
				CUICommonSystem.OpenFormWithAnimation(CUICommonSystem.s_FormFadeOutAniName, enUIEventID.MiShu_OnClickMiShu, uiEvent.m_eventParams);
			}
		}

		protected void OnOpenOBWithAni(CUIEvent uiEvent)
		{
			if (CUICommonSystem.IsInMatchingWithAlert())
			{
				return;
			}
			CUICommonSystem.OpenFormWithAnimation(CUICommonSystem.s_FormFadeOutAniName, enUIEventID.OB_Form_Open, uiEvent.m_eventParams);
		}

		protected void OnOpenChargeWithAni(CUIEvent uiEvent)
		{
			if (Singleton<CPaySystem>.instance.IsOpenPaySys())
			{
				CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CLobbySystem.LOBBY_FORM_PATH);
				if (form != null)
				{
					if (form.IsHided())
					{
						CUICommonSystem.OpenFormWithAnimation(CUICommonSystem.s_FormFadeOutAniName, enUIEventID.Pay_OpenBuyDianQuanPanel, uiEvent.m_eventParams);
					}
					else
					{
						CUICommonSystem.OpenFormWithAnimation(CUICommonSystem.s_FormFadeOutAniName, enUIEventID.Pay_OpenBuyDianQuanPanelWithLobby, uiEvent.m_eventParams);
					}
				}
				else
				{
					CUICommonSystem.OpenFormWithAnimation(CUICommonSystem.s_FormFadeOutAniName, enUIEventID.Pay_OpenBuyDianQuanPanel, uiEvent.m_eventParams);
				}
			}
		}

		protected void OnOpenFriendWithAni(CUIEvent uiEvent)
		{
			CUICommonSystem.OpenFormWithAnimation(CUICommonSystem.s_FormFadeOutAniName, enUIEventID.Friend_OpenForm, uiEvent.m_eventParams);
		}

		protected void OnOpenMailWithAni(CUIEvent uiEvent)
		{
			CUICommonSystem.OpenFormWithAnimation(CUICommonSystem.s_FormFadeOutAniName, enUIEventID.Mail_OpenForm, uiEvent.m_eventParams);
		}

		protected void OnOpenPvpEntryWithAni(CUIEvent uiEvent)
		{
			if (Singleton<CMatchingSystem>.instance.IsOpenMatchingSystem())
			{
				CUICommonSystem.OpenFormWithAnimation(CUICommonSystem.s_FormFadeOutAniName, enUIEventID.Matching_OpenEntry, uiEvent.m_eventParams);
			}
		}

		protected void OnOpenLadderEntryWithAni(CUIEvent uiEvent)
		{
			if (Singleton<CLadderSystem>.instance.IsLadderOpen(uiEvent))
			{
				CUICommonSystem.OpenFormWithAnimation(CUICommonSystem.s_FormFadeOutAniName, enUIEventID.Matching_OpenLadder, uiEvent.m_eventParams);
			}
		}

		protected void OnOpenPveEntryWithAni(CUIEvent uiEvent)
		{
			if (CUICommonSystem.IsInMatchingWithAlert())
			{
				return;
			}
			CUICommonSystem.OpenFormWithAnimation(CUICommonSystem.s_FormFadeOutAniName, enUIEventID.Explore_OpenForm, uiEvent.m_eventParams);
		}

		protected void OnOpenUnionBattleWithAni(CUIEvent uiEvent)
		{
			if (Singleton<CUnionBattleEntrySystem>.instance.IsUnionBattleOpen(uiEvent))
			{
				CUICommonSystem.OpenFormWithAnimation(CUICommonSystem.s_FormFadeOutAniName, enUIEventID.Union_Battle_ClickEntry, uiEvent.m_eventParams);
			}
		}

		public static bool IsInMatchingWithAlert()
		{
			if (Singleton<CMatchingSystem>.GetInstance().IsInMatching)
			{
				Singleton<CUIManager>.GetInstance().OpenTips("PVP_Matching", true, 1.5f, null, new object[0]);
				return true;
			}
			return false;
		}

		private void OnCommonOpenUrl(CUIEvent uiEvent)
		{
			string tagStr = uiEvent.m_eventParams.tagStr;
			if (!string.IsNullOrEmpty(tagStr))
			{
				CUICommonSystem.OpenUrl(tagStr, true, 0);
			}
		}

		public static enNetWorkType GetNetWorkType()
		{
			int num = CVersionUpdateSystem.Android_GetNetworkType();
			if (num == 0)
			{
				return enNetWorkType.enNone;
			}
			if (num == 1)
			{
				return enNetWorkType.enWifi;
			}
			return enNetWorkType.enNet;
		}
	}
}
