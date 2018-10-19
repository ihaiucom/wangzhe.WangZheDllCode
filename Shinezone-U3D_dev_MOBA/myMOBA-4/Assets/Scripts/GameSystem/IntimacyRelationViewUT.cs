using Assets.Scripts.Framework;
using Assets.Scripts.UI;
using CSProtocol;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class IntimacyRelationViewUT
	{
		public static Color blueColor = new Color(0.266666681f, 0.6901961f, 0.933333337f, 1f);

		public static Color redColor = new Color(0.921568632f, 0.396078438f, 0.482352942f, 1f);

		private static int _level1_MaxValue;

		private static int _level2_MaxValue;

		private static int _level3_MaxValue;

		private static int _level4_MaxValue;

		private static int[] priority = new int[]
		{
			1,
			4,
			2,
			3
		};

		public static void Show_Item(CUIComponent com, CFR frData, CUIFormScript uiFrom)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			if (masterRoleInfo != null && masterRoleInfo.playerUllUID == frData.ulluid && (long)masterRoleInfo.logicWorldID == (long)((ulong)frData.worldID))
			{
				return;
			}
			IntimacyRelationViewUT.Show_Item_Top(com, frData, uiFrom);
			IntimacyRelationViewUT.Show_Item_Middle(com, frData, uiFrom);
			IntimacyRelationViewUT.Show_Item_Bottom(com, frData);
			frData.bRedDot = false;
			GameObject obj = Utility.FindChild(com.m_widgets[1], "NobeImag");
			obj.CustomSetActive(!frData.bInShowChoiseRelaList);
		}

		public static void SetRelationBGImg(Image img, byte state)
		{
			IntimacyRelationViewUT.SetRelationBGImg(img, (COM_INTIMACY_STATE)state);
		}

		public static void SetRelationBGImg(Image img, COM_INTIMACY_STATE state)
		{
			if (img != null)
			{
				if (state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY || state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY_CONFIRM || state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY_DENY || state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_SIDEKICK || state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_SIDEKICK_CONFIRM || state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_SIDEKICK_DENY)
				{
					img.color = IntimacyRelationViewUT.blueColor;
					img.gameObject.CustomSetActive(true);
				}
				else if (state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_LOVER || state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_LOVER_CONFIRM || state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_LOVER_DENY || state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_BESTIE || state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_BESTIE_CONFIRM || state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_BESTIE_DENY)
				{
					img.color = IntimacyRelationViewUT.redColor;
					img.gameObject.CustomSetActive(true);
				}
				else
				{
					img.gameObject.CustomSetActive(false);
				}
			}
		}

		public static void Show_Item_Top(CUIComponent com, CFR frData, CUIFormScript uiFrom)
		{
			Image componetInChild = Utility.GetComponetInChild<Image>(com.gameObject, "Image");
			IntimacyRelationViewUT.SetRelationBGImg(componetInChild, frData.state);
			IntimacyRelationViewUT.SetButtonParam(com.m_widgets[19], frData);
			if (IntimacyRelationViewUT.IsRelaState(frData.state))
			{
				com.m_widgets[19].CustomSetActive(true);
			}
			else
			{
				com.m_widgets[19].CustomSetActive(false);
			}
			COMDT_FRIEND_INFO friendInfo = frData.friendInfo;
			if (friendInfo == null)
			{
				return;
			}
			CUIHttpImageScript component = com.m_widgets[1].GetComponent<CUIHttpImageScript>();
			UT.SetHttpImage(component, friendInfo.szHeadUrl);
			Image componetInChild2 = Utility.GetComponetInChild<Image>(component.gameObject, "NobeImag");
			if (componetInChild2 && uiFrom != null)
			{
				MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(componetInChild2, (int)friendInfo.stGameVip.dwHeadIconId);
				MonoSingleton<NobeSys>.GetInstance().SetHeadIconBkEffect(componetInChild2, (int)friendInfo.stGameVip.dwHeadIconId, uiFrom, 1f, true);
			}
			GameObject gameObject = com.m_widgets[2];
			if (gameObject)
			{
				MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(gameObject.GetComponent<Image>(), (int)friendInfo.stGameVip.dwCurLevel, false, false, friendInfo.ullUserPrivacyBits);
			}
			Text component2 = com.m_widgets[3].GetComponent<Text>();
			string text = UT.Bytes2String(friendInfo.szUserName);
			if (component2 != null)
			{
				component2.text = text;
			}
			GameObject genderImage = com.m_widgets[4];
			FriendShower.ShowGender(genderImage, (COM_SNSGENDER)friendInfo.bGender);
		}

		public static void Show_Item_Middle(CUIComponent com, CFR frData, CUIFormScript uiFrom)
		{
			IntimacyRelationViewUT.SetButtonParam(com.m_widgets[16], frData);
			IntimacyRelationViewUT.SetButtonParam(com.m_widgets[14], frData);
			IntimacyRelationViewUT.SetButtonParam(com.m_widgets[12], frData);
			IntimacyRelationViewUT.SetButtonParam(com.m_widgets[7], frData);
			int cDDays = frData.CDDays;
			GameObject obj = Utility.FindChild(com.gameObject, "mengban");
			obj.CustomSetActive(cDDays != -1);
			if (cDDays != -1)
			{
				com.m_widgets[5].CustomSetActive(true);
				com.m_widgets[6].CustomSetActive(false);
				IntimacyRelationViewUT.Set_Middle_Text(com, true, string.Format(UT.FRData().IntimRela_CD_CountDown, cDDays), false);
				return;
			}
			if (frData.state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_VALUE_FULL && cDDays == -1)
			{
				com.m_widgets[5].CustomSetActive(true);
				com.m_widgets[6].CustomSetActive(true);
				com.m_widgets[10].CustomSetActive(false);
				IntimacyRelationViewUT.Set_Drop_Text(com, !frData.bInShowChoiseRelaList, frData);
				IntimacyRelationViewUT.Set_Drop_List(com, frData.bInShowChoiseRelaList);
				IntimacyRelationViewUT.Set_Middle_Text(com, false, string.Empty, false);
			}
			if (IntimacyRelationViewUT.IsRelaState(frData.state))
			{
				com.m_widgets[5].CustomSetActive(true);
				com.m_widgets[6].CustomSetActive(false);
				IntimacyRelationViewUT.Set_Middle_Text(com, true, IntimacyRelationViewUT.GetMiddleTextStr(frData.state), true);
				ushort num;
				CFriendModel.EIntimacyType eIntimacyType;
				bool flag;
				Singleton<CFriendContoller>.instance.model.GetFriendIntimacy(frData.ulluid, frData.worldID, out num, out eIntimacyType, out flag);
				if (num > 0)
				{
					int relaLevel = IntimacyRelationViewUT.CalcRelaLevel((int)num);
					string relaIconByRelaLevel = IntimacyRelationViewUT.GetRelaIconByRelaLevel(relaLevel, frData.state);
					if (!string.IsNullOrEmpty(relaIconByRelaLevel))
					{
						GameObject p = com.m_widgets[10];
						GameObject gameObject = Utility.FindChild(p, "Icon");
						if (gameObject != null)
						{
							Image component = gameObject.GetComponent<Image>();
							if (component != null)
							{
								component.gameObject.CustomSetActive(true);
								component.SetSprite(relaIconByRelaLevel, uiFrom, true, false, false, false);
								component.SetNativeSize();
							}
						}
					}
				}
			}
			if (IntimacyRelationViewUT.IsRelaStateConfirm(frData.state))
			{
				if (frData.bReciveOthersRequest)
				{
					com.m_widgets[5].CustomSetActive(true);
					com.m_widgets[6].CustomSetActive(false);
					COM_INTIMACY_STATE stateByConfirmState = IntimacyRelationViewUT.GetStateByConfirmState(frData.state);
					string relationText = IntimacyRelationViewUT.GetRelationText(stateByConfirmState);
					string txt = string.Format(UT.FRData().IntimRela_Tips_ReceiveOtherReqRela, UT.Bytes2String(frData.friendInfo.szUserName), relationText);
					IntimacyRelationViewUT.Set_Middle_Text(com, true, txt, false);
				}
				else
				{
					com.m_widgets[5].CustomSetActive(true);
					com.m_widgets[6].CustomSetActive(false);
					IntimacyRelationViewUT.Set_Middle_Text(com, true, UT.FRData().IntimRela_Tips_Wait4TargetRspReqRela, false);
				}
			}
			if (IntimacyRelationViewUT.IsRelaStateDeny(frData.state))
			{
				if (frData.bReciveOthersRequest)
				{
					com.m_widgets[5].CustomSetActive(true);
					com.m_widgets[6].CustomSetActive(false);
					COM_INTIMACY_STATE stateByDenyState = IntimacyRelationViewUT.GetStateByDenyState(frData.state);
					string relationText2 = IntimacyRelationViewUT.GetRelationText(stateByDenyState);
					string txt2 = string.Format(UT.FRData().IntimRela_Tips_ReceiveOtherDelRela, UT.Bytes2String(frData.friendInfo.szUserName), relationText2);
					IntimacyRelationViewUT.Set_Middle_Text(com, true, txt2, false);
				}
				else
				{
					com.m_widgets[5].CustomSetActive(true);
					com.m_widgets[6].CustomSetActive(false);
					IntimacyRelationViewUT.Set_Middle_Text(com, true, UT.FRData().IntimRela_Tips_Wait4TargetRspDelRela, false);
				}
			}
		}

		public static void Set_Drop_Text(CUIComponent com, bool bShow, CFR frData)
		{
			com.m_widgets[9].CustomSetActive(bShow);
			string text = string.Empty;
			CFriendRelationship.FRConfig cFGByIndex = Singleton<CFriendContoller>.instance.model.FRData.GetCFGByIndex(frData.choiseRelation);
			if (cFGByIndex != null)
			{
				text = cFGByIndex.cfgRelaStr;
			}
			else
			{
				text = UT.FRData().IntimRela_Tips_SelectRelation;
			}
			com.m_widgets[9].GetComponent<Text>().text = text;
		}

		public static void Set_Drop_List(CUIComponent com, bool bShow)
		{
			com.m_widgets[8].CustomSetActive(bShow);
		}

		public static void Set_Middle_Text(CUIComponent com, bool bShow, string txt = "", bool bShowIcon = false)
		{
			GameObject gameObject = com.m_widgets[10];
			gameObject.CustomSetActive(bShow);
			GameObject gameObject2 = Utility.FindChild(gameObject, "text");
			gameObject2.CustomSetActive(bShow);
			gameObject2.GetComponent<Text>().text = txt;
			GameObject obj = Utility.FindChild(gameObject, "Icon");
			obj.CustomSetActive(bShowIcon);
		}

		public static string GetRelationInLoadingMenu(byte state, string name)
		{
			if (IntimacyRelationViewUT.IsRelaState(state))
			{
				RelationConfig relaTextCfg = Singleton<CFriendContoller>.instance.model.FRData.GetRelaTextCfg(state);
				if (relaTextCfg != null)
				{
					return Singleton<CTextManager>.instance.GetText(relaTextCfg.IntimacyShowLoad, new string[]
					{
						name
					});
				}
			}
			return string.Empty;
		}

		public static string GetRelationText(COM_INTIMACY_STATE state)
		{
			if (IntimacyRelationViewUT.IsRelaState(state))
			{
				RelationConfig relaTextCfg = Singleton<CFriendContoller>.instance.model.FRData.GetRelaTextCfg(state);
				if (relaTextCfg != null)
				{
					return relaTextCfg.IntimRela_Type;
				}
			}
			return string.Empty;
		}

		public static string GetMiddleTextStr(COM_INTIMACY_STATE state)
		{
			return IntimacyRelationViewUT.GetRelationText(state);
		}

		public static void Show_Item_Bottom(CUIComponent com, CFR frData)
		{
			IntimacyRelationViewUT.SetButtonParam(com.m_widgets[12], frData);
			IntimacyRelationViewUT.SetButtonParam(com.m_widgets[14], frData);
			IntimacyRelationViewUT.SetButtonParam(com.m_widgets[16], frData);
			IntimacyRelationViewUT.SetButtonParam(com.m_widgets[20], frData);
			IntimacyRelationViewUT.SetButtonParam(com.m_widgets[22], frData);
			int cDDays = frData.CDDays;
			if (cDDays != -1)
			{
				com.m_widgets[11].CustomSetActive(true);
				IntimacyRelationViewUT.Set_2_Button(com, false, false, string.Empty, string.Empty);
				IntimacyRelationViewUT.Set_1_Button(com, false, string.Empty);
				IntimacyRelationViewUT.Set_ReSelect_Button(com, false, string.Empty);
				IntimacyRelationViewUT.Set_DoSelect_Button(com, false, string.Empty);
				IntimacyRelationViewUT.Set_Bottom_Text(com, true, UT.FRData().IntimRela_Tips_RelaHasDel, false);
				return;
			}
			if (frData.state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_VALUE_FULL && cDDays == -1)
			{
				com.m_widgets[11].CustomSetActive(true);
				string intimRela_Tips_OK = UT.FRData().IntimRela_Tips_OK;
				string intimRela_Tips_Cancle = UT.FRData().IntimRela_Tips_Cancle;
				IntimacyRelationViewUT.Set_DoSelect_Button(com, frData.choiseRelation != -1, intimRela_Tips_OK);
				IntimacyRelationViewUT.Set_2_Button(com, false, false, intimRela_Tips_Cancle, intimRela_Tips_OK);
				IntimacyRelationViewUT.Set_1_Button(com, false, string.Empty);
				IntimacyRelationViewUT.Set_ReSelect_Button(com, false, string.Empty);
				IntimacyRelationViewUT.Set_Bottom_Text(com, false, string.Empty, false);
			}
			if (IntimacyRelationViewUT.IsRelaState(frData.state))
			{
				com.m_widgets[11].CustomSetActive(true);
				IntimacyRelationViewUT.Set_2_Button(com, false, false, string.Empty, string.Empty);
				IntimacyRelationViewUT.Set_1_Button(com, false, string.Empty);
				IntimacyRelationViewUT.Set_ReSelect_Button(com, false, string.Empty);
				IntimacyRelationViewUT.Set_DoSelect_Button(com, false, string.Empty);
				COM_INTIMACY_STATE firstChoiseState = Singleton<CFriendContoller>.instance.model.FRData.GetFirstChoiseState();
				if (firstChoiseState == frData.state)
				{
					IntimacyRelationViewUT.Set_Bottom_Text(com, true, UT.FRData().IntimRela_AleadyFristChoise, true);
				}
				else
				{
					IntimacyRelationViewUT.Set_Bottom_Text(com, false, string.Empty, false);
					IntimacyRelationViewUT.Set_1_Button(com, true, UT.FRData().IntimRela_DoFristChoise);
				}
			}
			if (IntimacyRelationViewUT.IsRelaStateConfirm(frData.state))
			{
				if (frData.bReciveOthersRequest)
				{
					com.m_widgets[11].CustomSetActive(true);
					IntimacyRelationViewUT.Set_2_Button(com, true, true, UT.FRData().IntimRela_Tips_Cancle, UT.FRData().IntimRela_Tips_OK);
					IntimacyRelationViewUT.Set_1_Button(com, false, string.Empty);
					IntimacyRelationViewUT.Set_Bottom_Text(com, false, string.Empty, false);
					IntimacyRelationViewUT.Set_ReSelect_Button(com, false, string.Empty);
					IntimacyRelationViewUT.Set_DoSelect_Button(com, false, string.Empty);
				}
				else
				{
					com.m_widgets[11].CustomSetActive(true);
					IntimacyRelationViewUT.Set_2_Button(com, false, false, string.Empty, string.Empty);
					IntimacyRelationViewUT.Set_1_Button(com, false, string.Empty);
					IntimacyRelationViewUT.Set_Bottom_Text(com, false, UT.FRData().IntimRela_Tips_Wait4TargetRspReqRela, false);
					IntimacyRelationViewUT.Set_ReSelect_Button(com, true, UT.FRData().IntimRela_ReselectRelation);
					IntimacyRelationViewUT.Set_DoSelect_Button(com, false, string.Empty);
				}
			}
			if (IntimacyRelationViewUT.IsRelaStateDeny(frData.state))
			{
				if (frData.bReciveOthersRequest)
				{
					com.m_widgets[11].CustomSetActive(true);
					IntimacyRelationViewUT.Set_2_Button(com, true, true, UT.FRData().IntimRela_Tips_Cancle, UT.FRData().IntimRela_Tips_OK);
					IntimacyRelationViewUT.Set_1_Button(com, false, string.Empty);
					IntimacyRelationViewUT.Set_Bottom_Text(com, false, string.Empty, false);
					IntimacyRelationViewUT.Set_ReSelect_Button(com, false, string.Empty);
					IntimacyRelationViewUT.Set_DoSelect_Button(com, false, string.Empty);
				}
				else
				{
					com.m_widgets[11].CustomSetActive(true);
					IntimacyRelationViewUT.Set_2_Button(com, false, false, string.Empty, string.Empty);
					IntimacyRelationViewUT.Set_1_Button(com, false, string.Empty);
					IntimacyRelationViewUT.Set_Bottom_Text(com, false, UT.FRData().IntimRela_Tips_Wait4TargetRspDelRela, false);
					IntimacyRelationViewUT.Set_ReSelect_Button(com, true, UT.FRData().IntimRela_ReDelRelation);
					IntimacyRelationViewUT.Set_DoSelect_Button(com, false, string.Empty);
				}
			}
		}

		public static void Set_2_Button(CUIComponent com, bool bLeftShow, bool bRightShow, string left = "", string right = "")
		{
			com.m_widgets[16].CustomSetActive(bLeftShow);
			com.m_widgets[14].CustomSetActive(bRightShow);
			com.m_widgets[17].GetComponent<Text>().text = left;
			com.m_widgets[15].GetComponent<Text>().text = right;
		}

		public static void Set_1_Button(CUIComponent com, bool bShow, string txt = "")
		{
			com.m_widgets[12].CustomSetActive(bShow);
			com.m_widgets[13].GetComponent<Text>().text = txt;
		}

		public static void Set_ReSelect_Button(CUIComponent com, bool bShow, string txt = "")
		{
			com.m_widgets[20].CustomSetActive(bShow);
			com.m_widgets[21].GetComponent<Text>().text = txt;
		}

		public static void Set_DoSelect_Button(CUIComponent com, bool bShow, string txt = "")
		{
			com.m_widgets[22].CustomSetActive(bShow);
			com.m_widgets[23].GetComponent<Text>().text = txt;
		}

		public static void Set_Bottom_Text(CUIComponent com, bool bShow, string txt = "", bool bShowIcon = false)
		{
			GameObject p = com.m_widgets[18];
			GameObject gameObject = Utility.FindChild(p, "text");
			gameObject.CustomSetActive(bShow);
			gameObject.GetComponent<Text>().text = txt;
			GameObject obj = Utility.FindChild(p, "Icon");
			obj.CustomSetActive(bShowIcon);
		}

		private static void SetButtonParam(GameObject obj, CFR frData)
		{
			CUIEventScript component = obj.GetComponent<CUIEventScript>();
			if (component != null)
			{
				component.m_onClickEventParams.commonUInt64Param1 = frData.ulluid;
				component.m_onClickEventParams.tagUInt = frData.worldID;
				component.m_onClickEventParams.tag = (int)frData.state;
				component.m_onClickEventParams.tag2 = frData.choiseRelation;
			}
		}

		private static void Process_Bottm_Btns(bool bShow, GameObject node, ulong ullUid, uint dwLogicWorldId)
		{
			GameObject gameObject = node.transform.FindChild("Button_Send").gameObject;
			GameObject gameObject2 = node.transform.FindChild("Button_Cancel").gameObject;
			IntimacyRelationViewUT.SetEvtParam(gameObject, ullUid, dwLogicWorldId);
			IntimacyRelationViewUT.SetEvtParam(gameObject2, ullUid, dwLogicWorldId);
			gameObject.CustomSetActive(bShow);
			gameObject.CustomSetActive(bShow);
		}

		private static void SetEvtParam(GameObject obj, ulong ullUid, uint dwLogicWorldId)
		{
			CUIEventScript component = obj.GetComponent<CUIEventScript>();
			component.m_onClickEventParams.commonUInt64Param2 = ullUid;
			component.m_onClickEventParams.taskId = dwLogicWorldId;
		}

		public static COM_INTIMACY_STATE GetConfirmState(byte state)
		{
			return IntimacyRelationViewUT.GetConfirmState((COM_INTIMACY_STATE)state);
		}

		public static COM_INTIMACY_STATE GetConfirmState(COM_INTIMACY_STATE state)
		{
			if (state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY)
			{
				return COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY_CONFIRM;
			}
			if (state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_LOVER)
			{
				return COM_INTIMACY_STATE.COM_INTIMACY_STATE_LOVER_CONFIRM;
			}
			if (state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_SIDEKICK)
			{
				return COM_INTIMACY_STATE.COM_INTIMACY_STATE_SIDEKICK_CONFIRM;
			}
			if (state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_BESTIE)
			{
				return COM_INTIMACY_STATE.COM_INTIMACY_STATE_BESTIE_CONFIRM;
			}
			return COM_INTIMACY_STATE.COM_INTIMACY_STATE_NULL;
		}

		public static COM_INTIMACY_STATE GetStateByConfirmState(COM_INTIMACY_STATE state)
		{
			if (state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY_CONFIRM)
			{
				return COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY;
			}
			if (state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_LOVER_CONFIRM)
			{
				return COM_INTIMACY_STATE.COM_INTIMACY_STATE_LOVER;
			}
			if (state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_SIDEKICK_CONFIRM)
			{
				return COM_INTIMACY_STATE.COM_INTIMACY_STATE_SIDEKICK;
			}
			if (state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_BESTIE_CONFIRM)
			{
				return COM_INTIMACY_STATE.COM_INTIMACY_STATE_BESTIE;
			}
			return COM_INTIMACY_STATE.COM_INTIMACY_STATE_NULL;
		}

		public static COM_INTIMACY_STATE GetDenyState(byte state)
		{
			return IntimacyRelationViewUT.GetDenyState((COM_INTIMACY_STATE)state);
		}

		public static COM_INTIMACY_STATE GetDenyState(COM_INTIMACY_STATE state)
		{
			if (state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY)
			{
				return COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY_DENY;
			}
			if (state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_LOVER)
			{
				return COM_INTIMACY_STATE.COM_INTIMACY_STATE_LOVER_DENY;
			}
			if (state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_SIDEKICK)
			{
				return COM_INTIMACY_STATE.COM_INTIMACY_STATE_SIDEKICK_DENY;
			}
			if (state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_BESTIE)
			{
				return COM_INTIMACY_STATE.COM_INTIMACY_STATE_BESTIE_DENY;
			}
			return COM_INTIMACY_STATE.COM_INTIMACY_STATE_NULL;
		}

		public static COM_INTIMACY_STATE GetStateByDenyState(COM_INTIMACY_STATE state)
		{
			if (state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY_DENY)
			{
				return COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY;
			}
			if (state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_LOVER_DENY)
			{
				return COM_INTIMACY_STATE.COM_INTIMACY_STATE_LOVER;
			}
			if (state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_SIDEKICK_DENY)
			{
				return COM_INTIMACY_STATE.COM_INTIMACY_STATE_SIDEKICK;
			}
			if (state == COM_INTIMACY_STATE.COM_INTIMACY_STATE_BESTIE_DENY)
			{
				return COM_INTIMACY_STATE.COM_INTIMACY_STATE_BESTIE;
			}
			return COM_INTIMACY_STATE.COM_INTIMACY_STATE_NULL;
		}

		public static bool IsRelaState(COM_INTIMACY_STATE state)
		{
			return IntimacyRelationViewUT.IsRelaState((byte)state);
		}

		public static bool IsRelaState(byte state)
		{
			return state == 1 || state == 2 || state == 3 || state == 4;
		}

		public static bool IsRelaStateConfirm(COM_INTIMACY_STATE state)
		{
			return IntimacyRelationViewUT.IsRelaStateConfirm((byte)state);
		}

		public static bool IsRelaStateConfirm(byte state)
		{
			return state == 20 || state == 22 || state == 24 || state == 26;
		}

		public static bool IsRelaStateDeny(COM_INTIMACY_STATE state)
		{
			return IntimacyRelationViewUT.IsRelaStateDeny((byte)state);
		}

		public static bool IsRelaStateDeny(byte state)
		{
			return state == 21 || state == 23 || state == 25 || state == 27;
		}

		public static int Getlevel1_maxValue()
		{
			if (IntimacyRelationViewUT._level1_MaxValue == 0)
			{
				IntimacyRelationViewUT._level1_MaxValue = (int)GameDataMgr.globalInfoDatabin.GetDataByKey(331u).dwConfValue;
			}
			return IntimacyRelationViewUT._level1_MaxValue;
		}

		public static int Getlevel2_maxValue()
		{
			if (IntimacyRelationViewUT._level2_MaxValue == 0)
			{
				IntimacyRelationViewUT._level2_MaxValue = (int)GameDataMgr.globalInfoDatabin.GetDataByKey(332u).dwConfValue;
			}
			return IntimacyRelationViewUT._level2_MaxValue;
		}

		public static int Getlevel3_maxValue()
		{
			if (IntimacyRelationViewUT._level3_MaxValue == 0)
			{
				IntimacyRelationViewUT._level3_MaxValue = (int)GameDataMgr.globalInfoDatabin.GetDataByKey(354u).dwConfValue;
			}
			return IntimacyRelationViewUT._level3_MaxValue;
		}

		public static int Getlevel4_maxValue()
		{
			if (IntimacyRelationViewUT._level4_MaxValue == 0)
			{
				IntimacyRelationViewUT._level4_MaxValue = (int)GameDataMgr.globalInfoDatabin.GetDataByKey(355u).dwConfValue;
			}
			return IntimacyRelationViewUT._level4_MaxValue;
		}

		public static int CalcRelaLevel(int intimacyValue)
		{
			if (intimacyValue >= 0 && intimacyValue < IntimacyRelationViewUT.Getlevel1_maxValue())
			{
				return 0;
			}
			if (intimacyValue >= IntimacyRelationViewUT.Getlevel1_maxValue() && intimacyValue < IntimacyRelationViewUT.Getlevel2_maxValue())
			{
				return 1;
			}
			if (intimacyValue >= IntimacyRelationViewUT.Getlevel2_maxValue() && intimacyValue < IntimacyRelationViewUT.Getlevel3_maxValue())
			{
				return 2;
			}
			if (intimacyValue >= IntimacyRelationViewUT.Getlevel3_maxValue() && intimacyValue < IntimacyRelationViewUT.Getlevel4_maxValue())
			{
				return 3;
			}
			if (intimacyValue >= IntimacyRelationViewUT.Getlevel4_maxValue())
			{
				return 4;
			}
			return 0;
		}

		public static string GetRelaIcon(int intimacyValue, byte state)
		{
			return IntimacyRelationViewUT.GetRelaIcon(intimacyValue, (COM_INTIMACY_STATE)state);
		}

		public static string GetRelaIcon(int intimacyValue, COM_INTIMACY_STATE state)
		{
			int relaLevel = IntimacyRelationViewUT.CalcRelaLevel(intimacyValue);
			return IntimacyRelationViewUT.GetRelaIconByRelaLevel(relaLevel, state);
		}

		public static string GetRelaIconByRelaLevel(int relaLevel, COM_INTIMACY_STATE state)
		{
			RelationConfig relaTextCfg = Singleton<CFriendContoller>.instance.model.FRData.GetRelaTextCfg(state);
			if (relaTextCfg == null)
			{
				return string.Empty;
			}
			if (relaLevel == 1 || relaLevel == 2)
			{
				return relaTextCfg.iconLevel_1;
			}
			if (relaLevel == 3)
			{
				return relaTextCfg.iconLevel_2;
			}
			if (relaLevel == 4)
			{
				return relaTextCfg.iconLevel_3;
			}
			return string.Empty;
		}

		public static string GetLevelDescStr(int relaLevel)
		{
			if (relaLevel == 1)
			{
				return Singleton<CTextManager>.instance.GetText("RelaLevel_Desc_1");
			}
			if (relaLevel == 2)
			{
				return Singleton<CTextManager>.instance.GetText("RelaLevel_Desc_2");
			}
			if (relaLevel == 3)
			{
				return Singleton<CTextManager>.instance.GetText("RelaLevel_Desc_3");
			}
			if (relaLevel == 4)
			{
				return Singleton<CTextManager>.instance.GetText("RelaLevel_Desc_4");
			}
			return string.Empty;
		}

		public static int GetNxtLevelValue(int intimacyValue)
		{
			if (intimacyValue >= 0 && intimacyValue < IntimacyRelationViewUT.Getlevel1_maxValue())
			{
				return IntimacyRelationViewUT.Getlevel1_maxValue();
			}
			if (intimacyValue >= IntimacyRelationViewUT.Getlevel1_maxValue() && intimacyValue < IntimacyRelationViewUT.Getlevel2_maxValue())
			{
				return IntimacyRelationViewUT.Getlevel2_maxValue();
			}
			if (intimacyValue >= IntimacyRelationViewUT.Getlevel2_maxValue() && intimacyValue < IntimacyRelationViewUT.Getlevel3_maxValue())
			{
				return IntimacyRelationViewUT.Getlevel3_maxValue();
			}
			if (intimacyValue >= IntimacyRelationViewUT.Getlevel3_maxValue() && intimacyValue < IntimacyRelationViewUT.Getlevel4_maxValue())
			{
				return IntimacyRelationViewUT.Getlevel4_maxValue();
			}
			if (intimacyValue >= IntimacyRelationViewUT.Getlevel4_maxValue())
			{
				return IntimacyRelationViewUT.Getlevel4_maxValue();
			}
			return -1;
		}

		public static int GetCurLevelDoorValue(int intimacyValue)
		{
			if (intimacyValue >= 0 && intimacyValue < IntimacyRelationViewUT.Getlevel1_maxValue())
			{
				return 0;
			}
			if (intimacyValue >= IntimacyRelationViewUT.Getlevel1_maxValue() && intimacyValue < IntimacyRelationViewUT.Getlevel2_maxValue())
			{
				return IntimacyRelationViewUT.Getlevel1_maxValue();
			}
			if (intimacyValue >= IntimacyRelationViewUT.Getlevel2_maxValue() && intimacyValue < IntimacyRelationViewUT.Getlevel3_maxValue())
			{
				return IntimacyRelationViewUT.Getlevel2_maxValue();
			}
			if (intimacyValue >= IntimacyRelationViewUT.Getlevel3_maxValue() && intimacyValue < IntimacyRelationViewUT.Getlevel4_maxValue())
			{
				return IntimacyRelationViewUT.Getlevel3_maxValue();
			}
			if (intimacyValue >= IntimacyRelationViewUT.Getlevel4_maxValue())
			{
				return IntimacyRelationViewUT.Getlevel4_maxValue();
			}
			return -1;
		}

		public static bool IsRelationPriorityHigher(byte State1, byte State2, byte firstChoise)
		{
			return IntimacyRelationViewUT.IsRelationPriorityHigher((COM_INTIMACY_STATE)State1, (COM_INTIMACY_STATE)State2, (COM_INTIMACY_STATE)firstChoise);
		}

		public static bool IsRelationPriorityHigher(byte State1, COM_INTIMACY_STATE State2, byte firstChoise)
		{
			return IntimacyRelationViewUT.IsRelationPriorityHigher((COM_INTIMACY_STATE)State1, State2, (COM_INTIMACY_STATE)firstChoise);
		}

		public static bool IsRelationPriorityHigher(COM_INTIMACY_STATE State1, COM_INTIMACY_STATE State2, COM_INTIMACY_STATE firstChoise)
		{
			return State2 == COM_INTIMACY_STATE.COM_INTIMACY_STATE_NULL || (State1 != State2 && (State1 == firstChoise || (State2 != firstChoise && IntimacyRelationViewUT.priority[State1 - COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY] > IntimacyRelationViewUT.priority[State2 - COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY])));
		}
	}
}
