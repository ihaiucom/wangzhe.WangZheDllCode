using Apollo;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	[MessageHandlerClass]
	public class CPaySystem : Singleton<CPaySystem>
	{
		public static string s_firstPayFormPath = "UGUI/Form/System/Pay/Form_FirstPayDiamond.prefab";

		public static string s_renewalDianQuanFormPath = "UGUI/Form/System/Pay/Form_RenewalDiamond.prefab";

		public static string s_buyDianQuanFormPath = "UGUI/Form/System/Pay/Form_BuyDiamond.prefab";

		private int m_dianQuanGiftId;

		private int m_dianQuanCnt;

		private int m_pandoraDianQuanReqSeq = -1;

		private ulong m_lastDianQuan;

		private ListView<ResCouponsBuyInfo> dianQuanBuyInfoList = new ListView<ResCouponsBuyInfo>();

		private ListView<CUseable> rewardItems = new ListView<CUseable>();

		public override void Init()
		{
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Pay_OpenFirstPayPanel, new CUIEventManager.OnUIEventHandler(this.OnOpenFirstPayPanel));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Pay_OpenRenewalPanel, new CUIEventManager.OnUIEventHandler(this.OnOpenRenewalPanel));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Pay_GetFirstPayReward, new CUIEventManager.OnUIEventHandler(this.OnGetFirstPayReward));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Pay_GetRenewalReward, new CUIEventManager.OnUIEventHandler(this.OnGetRenewalReward));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Pay_RevertToVisiable, new CUIEventManager.OnUIEventHandler(this.OnRevertoVisiable));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Pay_OpenBuyDianQuanPanel, new CUIEventManager.OnUIEventHandler(this.OnOpenBuyDianQuanPanel));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Pay_OpenBuyDianQuanPanelWithLobby, new CUIEventManager.OnUIEventHandler(this.OnOpenBuyDianQuanPanel));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Pay_FirstPayDianQuan, new CUIEventManager.OnUIEventHandler(this.OnFirstPayDianQuan));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Pay_RenewalDianQuan, new CUIEventManager.OnUIEventHandler(this.OnRenewalDianQuan));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Pay_ClickDianQuanGift, new CUIEventManager.OnUIEventHandler(this.OnClickDianQuanGift));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Pay_BuyDianQuanPanelClose, new CUIEventManager.OnUIEventHandler(this.OnBuyDianQuanPanelClose));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Pay_ClickGetNewHeroPanel, new CUIEventManager.OnUIEventHandler(this.OnClickGetNewHeroPanel));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Pay_PlayHeroVideo, new CUIEventManager.OnUIEventHandler(this.OnPlayHeroVideo));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Pay_TehuiShop, new CUIEventManager.OnUIEventHandler(this.OnDisplayChaoZhiGift));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Pay_GotoTehuiShop, new CUIEventManager.OnUIEventHandler(this.OnGotoChaoZhiGift));
			Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.ApolloHelper_Pay_Success, new Action(this.OnPaySuccess));
			Singleton<EventRouter>.GetInstance().AddEventHandler<int>(EventID.ApolloHelper_Pay_Risk_Hit, new Action<int>(this.OnPayRiskHit));
			Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.ApolloHelper_Pay_Failed, new Action(this.OnPayFailed));
			Singleton<EventRouter>.GetInstance().AddEventHandler(EventID.ApolloHelper_Need_Login, new Action(this.OnNeedLogin));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Pay_OpenBuyDianQuanPanelinLobby, new CUIEventManager.OnUIEventHandler(this.OnOpenBuyDianQuanPanelInLobby));
			Singleton<EventRouter>.GetInstance().AddEventHandler<CMallFactoryShopController.ShopProduct>(EventID.Mall_Factory_Shop_Product_Off_Sale, new Action<CMallFactoryShopController.ShopProduct>(this.NeedUpdateChaoZhieGift));
		}

		public override void UnInit()
		{
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Pay_OpenFirstPayPanel, new CUIEventManager.OnUIEventHandler(this.OnOpenFirstPayPanel));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Pay_OpenRenewalPanel, new CUIEventManager.OnUIEventHandler(this.OnOpenRenewalPanel));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Pay_GetFirstPayReward, new CUIEventManager.OnUIEventHandler(this.OnGetFirstPayReward));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Pay_GetRenewalReward, new CUIEventManager.OnUIEventHandler(this.OnGetRenewalReward));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Pay_RevertToVisiable, new CUIEventManager.OnUIEventHandler(this.OnRevertoVisiable));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Pay_OpenBuyDianQuanPanel, new CUIEventManager.OnUIEventHandler(this.OnOpenBuyDianQuanPanel));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Pay_FirstPayDianQuan, new CUIEventManager.OnUIEventHandler(this.OnFirstPayDianQuan));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Pay_RenewalDianQuan, new CUIEventManager.OnUIEventHandler(this.OnRenewalDianQuan));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Pay_ClickDianQuanGift, new CUIEventManager.OnUIEventHandler(this.OnClickDianQuanGift));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Pay_BuyDianQuanPanelClose, new CUIEventManager.OnUIEventHandler(this.OnBuyDianQuanPanelClose));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Pay_ClickGetNewHeroPanel, new CUIEventManager.OnUIEventHandler(this.OnClickGetNewHeroPanel));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Pay_PlayHeroVideo, new CUIEventManager.OnUIEventHandler(this.OnPlayHeroVideo));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Pay_TehuiShop, new CUIEventManager.OnUIEventHandler(this.OnDisplayChaoZhiGift));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Pay_GotoTehuiShop, new CUIEventManager.OnUIEventHandler(this.OnGotoChaoZhiGift));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.ApolloHelper_Pay_Success, new Action(this.OnPaySuccess));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<int>(EventID.ApolloHelper_Pay_Risk_Hit, new Action<int>(this.OnPayRiskHit));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.ApolloHelper_Pay_Failed, new Action(this.OnPayFailed));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler(EventID.ApolloHelper_Need_Login, new Action(this.OnNeedLogin));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Pay_OpenBuyDianQuanPanelinLobby, new CUIEventManager.OnUIEventHandler(this.OnOpenBuyDianQuanPanelInLobby));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<CMallFactoryShopController.ShopProduct>(EventID.Mall_Factory_Shop_Product_Off_Sale, new Action<CMallFactoryShopController.ShopProduct>(this.NeedUpdateChaoZhieGift));
		}

		private void OnOpenFirstPayPanel(CUIEvent uiEvent)
		{
			Singleton<CChatController>.GetInstance().ShowPanel(true, false);
			CUIFormScript x = Singleton<CUIManager>.GetInstance().OpenForm(CPaySystem.s_firstPayFormPath, false, true);
			if (x == null)
			{
				return;
			}
			Singleton<BeaconHelper>.GetInstance().m_curBuyDianInfo.buy_dia_channel = "first";
			Singleton<BeaconHelper>.GetInstance().m_curBuyDianInfo.call_back_time = Time.time;
			this.RefreshFirstPayPanel();
		}

		private void NeedUpdateChaoZhieGift(CMallFactoryShopController.ShopProduct product)
		{
			int dwConfValue = (int)GameDataMgr.globalInfoDatabin.GetDataByKey(172u).dwConfValue;
			int dwConfValue2 = (int)GameDataMgr.globalInfoDatabin.GetDataByKey(173u).dwConfValue;
			CMallFactoryShopController.ShopProduct product2 = Singleton<CMallFactoryShopController>.GetInstance().GetProduct((uint)dwConfValue);
			CMallFactoryShopController.ShopProduct product3 = Singleton<CMallFactoryShopController>.GetInstance().GetProduct((uint)dwConfValue2);
			if ((product2 == null || product2.IsOnSale != 1) && (product3 == null || product3.IsOnSale != 1))
			{
				CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
				CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CLobbySystem.LOBBY_FORM_PATH);
				if (form == null)
				{
					return;
				}
				Transform x = form.transform.Find("DiamondPayBtn");
				if (x != null)
				{
					GameObject gameObject = form.transform.Find("DiamondPayBtn").gameObject;
					if (gameObject != null && masterRoleInfo.IsClientBitsSet(0))
					{
						gameObject.CustomSetActive(false);
						masterRoleInfo.SetClientBits(0, false, true);
					}
				}
			}
		}

		private void OnGotoChaoZhiGift(CUIEvent uiEvent)
		{
			string formPath = string.Format("{0}{1}", "UGUI/Form/System/", "SevenDayCheck/DisplayTeHuiShop.prefab");
			Singleton<CUIManager>.GetInstance().CloseForm(formPath);
			CUICommonSystem.JumpForm(RES_GAME_ENTRANCE_TYPE.RES_GAME_ENTRANCE_SHOP_DISCOUNT, 0, 0, null);
		}

		private void OnDisplayChaoZhiGift(CUIEvent uiEvent)
		{
			string formPath = string.Format("{0}{1}", "UGUI/Form/System/", "SevenDayCheck/DisplayTeHuiShop.prefab");
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(formPath, false, true);
			Transform transform = cUIFormScript.gameObject.transform.FindChild("Panel/Title/Text");
			transform.gameObject.GetComponent<Text>().text = Singleton<CTextManager>.instance.GetText("GotoTehuiShopTitle");
		}

		private void OnOpenRenewalPanel(CUIEvent uiEvent)
		{
			Singleton<CChatController>.GetInstance().ShowPanel(true, false);
			CUIFormScript x = Singleton<CUIManager>.GetInstance().OpenForm(CPaySystem.s_renewalDianQuanFormPath, false, true);
			if (x == null)
			{
				return;
			}
			this.RefreshRenewalPanel();
		}

		private void OnGetFirstPayReward(CUIEvent uiEvent)
		{
			CPaySystem.SendReqGetDianQuanReward(CS_COUPONS_PAYTYPE.CS_COUPONS_PAYTYPE_FIRST);
		}

		private void OnGetRenewalReward(CUIEvent uiEvent)
		{
			CPaySystem.SendReqGetDianQuanReward(CS_COUPONS_PAYTYPE.CS_COUPONS_PAYTYPE_RENEW);
		}

		private void OnOpenBuyDianQuanPanelInLobby(CUIEvent uiEvent)
		{
			CUICommonSystem.ResetLobbyFormFadeRecover();
			Singleton<BeaconHelper>.GetInstance().m_curBuyDianInfo.buy_dia_channel = "formal";
			Singleton<BeaconHelper>.GetInstance().m_curBuyDianInfo.call_back_time = Time.time;
			this.OnOpenBuyDianQuanPanel(uiEvent);
		}

		private void OnRevertoVisiable(CUIEvent uiEvent)
		{
			CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
			if (srcFormScript == null)
			{
				return;
			}
			GameObject target = srcFormScript.m_formWidgets[1];
			if (NetworkAccelerator.IsCommercialized() && !CUIRedDotSystem.IsShowRedDotByVersion(enRedID.Lobby_PayEntry))
			{
				CUIRedDotSystem.DelRedDot(target);
			}
		}

		private void OnOpenBuyDianQuanPanel(CUIEvent uiEvent)
		{
			CPartnerSystem.HideSysEntryChargeRedDot();
			this.OpenBuyDianQuanPanel(uiEvent);
		}

		private void OnBuyDianQuanPanelClose(CUIEvent uiEvent)
		{
			if (Singleton<CUIManager>.instance.GetForm(CRoomSystem.PATH_ROOM) == null && Singleton<CUIManager>.instance.GetForm(CMatchingSystem.PATH_MATCHING_MULTI) == null)
			{
				Singleton<CTopLobbyEntry>.GetInstance().CloseForm();
			}
			if (!CSysDynamicBlock.bLobbyEntryBlocked)
			{
				this.AutoOpenRewardPanel(false);
			}
		}

		private void OnFirstPayDianQuan(CUIEvent uiEvent)
		{
			this.OpenBuyDianQuanPanel(uiEvent);
		}

		private void OnRenewalDianQuan(CUIEvent uiEvent)
		{
			this.OpenBuyDianQuanPanel(uiEvent);
		}

		private void OnClickDianQuanGift(CUIEvent uiEvent)
		{
			this.m_dianQuanGiftId = uiEvent.m_eventParams.dianQuanBuyPar.giftId;
			this.m_dianQuanCnt = uiEvent.m_eventParams.dianQuanBuyPar.dianQuanCnt;
			uint dianQuanGiftCnt = this.GetDianQuanGiftCnt(this.m_dianQuanGiftId);
			Singleton<BeaconHelper>.GetInstance().m_curBuyDianInfo.buy_dia_id = this.m_dianQuanCnt.ToString();
			DateTime curTime = DateTime.Now;
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo != null)
			{
				curTime = Utility.ToUtcTime2Local((long)CRoleInfo.GetCurrentUTCTime());
			}
			Singleton<BeaconHelper>.GetInstance().ReportOpenBuyDianEvent(curTime);
			DebugHelper.Assert(masterRoleInfo != null, "pay master role = null");
			if (masterRoleInfo == null)
			{
				return;
			}
			if (masterRoleInfo.DianQuan + (ulong)dianQuanGiftCnt > 2147483647uL)
			{
				Singleton<CUIManager>.GetInstance().OpenTips("充值超过钻石上限", false, 1.5f, null, new object[0]);
				return;
			}
			Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
			Singleton<ApolloHelper>.GetInstance().Pay(this.m_dianQuanCnt.ToString(), string.Empty);
		}

		private uint GetDianQuanGiftCnt(int giftId)
		{
			uint result = 0u;
			giftId--;
			if (giftId >= 0 && giftId < this.dianQuanBuyInfoList.Count)
			{
				ResCouponsBuyInfo resCouponsBuyInfo = this.dianQuanBuyInfoList[giftId];
				if (resCouponsBuyInfo == null)
				{
					return 0u;
				}
				if (resCouponsBuyInfo.bFirstGift > 0 && !this.IsDianQuanHaveFirstPay(resCouponsBuyInfo.dwID))
				{
					result = resCouponsBuyInfo.dwBuyCount;
				}
				else
				{
					result = resCouponsBuyInfo.dwExtraGiftCnt;
				}
			}
			return result;
		}

		public void RefreshBuyDianQuanPanel()
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CPaySystem.s_buyDianQuanFormPath);
			if (form == null)
			{
				return;
			}
			DatabinTable<ResCouponsBuyInfo, uint> androidDianQuanBuyInfo = GameDataMgr.androidDianQuanBuyInfo;
			this.dianQuanBuyInfoList.Clear();
			androidDianQuanBuyInfo.Accept(delegate(ResCouponsBuyInfo x)
			{
				this.dianQuanBuyInfoList.Add(x);
			});
			this.SortDianQuanInfoList();
			CUIListScript component = form.transform.Find("pnlBg/pnlBody/List").GetComponent<CUIListScript>();
			component.SetElementAmount(this.dianQuanBuyInfoList.Count);
			for (int i = 0; i < this.dianQuanBuyInfoList.Count; i++)
			{
				ResCouponsBuyInfo resCouponsBuyInfo = this.dianQuanBuyInfoList[i];
				CUIListElementScript elemenet = component.GetElemenet(i);
				Image component2 = elemenet.transform.Find("imgIcon").GetComponent<Image>();
				string prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Purchase_Dir, StringHelper.UTF8BytesToString(ref resCouponsBuyInfo.szImgPath));
				component2.SetSprite(prefabPath, form, true, false, false, false);
				Text component3 = elemenet.transform.Find("diamondCntText").GetComponent<Text>();
				component3.text = string.Format(Singleton<CTextManager>.GetInstance().GetText("Pay_DianQuan_Cnt"), resCouponsBuyInfo.dwBuyCount);
				GameObject gameObject = elemenet.transform.Find("buyPanel/buyBtn").gameObject;
				Text component4 = gameObject.transform.Find("Text").GetComponent<Text>();
				component4.text = string.Format(Singleton<CTextManager>.GetInstance().GetText("Pay_DianQuan_Price"), resCouponsBuyInfo.dwBuyPrice);
				GameObject gameObject2 = elemenet.transform.Find("additionPanel").gameObject;
				gameObject2.CustomSetActive(false);
				GameObject gameObject3 = elemenet.transform.Find("pnlRecommend").gameObject;
				if (resCouponsBuyInfo.bFirstGift > 0 && !this.IsDianQuanHaveFirstPay(resCouponsBuyInfo.dwID))
				{
					gameObject3.CustomSetActive(true);
					Text component5 = gameObject3.transform.Find("txtDiscount").GetComponent<Text>();
					component5.text = Singleton<CTextManager>.GetInstance().GetText("Pay_First_Pay_Double");
				}
				else
				{
					if (resCouponsBuyInfo.dwExtraGiftCnt > 0u)
					{
						gameObject2.CustomSetActive(true);
						Text component6 = gameObject2.transform.Find("Text").GetComponent<Text>();
						component6.text = string.Format(Singleton<CTextManager>.GetInstance().GetText("Pay_Gift_Diamond_Cnt"), resCouponsBuyInfo.dwExtraGiftCnt);
					}
					gameObject3.CustomSetActive(false);
				}
				CUIEventScript component7 = gameObject.GetComponent<CUIEventScript>();
				stUIEventParams eventParams = default(stUIEventParams);
				eventParams.dianQuanBuyPar.giftId = (int)resCouponsBuyInfo.dwID;
				eventParams.dianQuanBuyPar.dianQuanCnt = (int)resCouponsBuyInfo.dwBuyCount;
				component7.SetUIEvent(enUIEventType.Click, enUIEventID.Pay_ClickDianQuanGift, eventParams);
			}
			if (CSysDynamicBlock.bLobbyEntryBlocked)
			{
				Transform transform = form.transform.FindChild("Button_OpenNobe");
				Transform transform2 = form.transform.FindChild("Button_HelpMe");
				Transform transform3 = form.transform.FindChild("Button_TongCai");
				Transform transform4 = form.transform.FindChild("Button_Partner");
				if (transform)
				{
					transform.gameObject.CustomSetActive(false);
				}
				if (transform2)
				{
					transform2.gameObject.CustomSetActive(false);
				}
				if (transform3)
				{
					transform3.gameObject.CustomSetActive(false);
				}
				if (transform4)
				{
					transform4.gameObject.CustomSetActive(false);
				}
			}
		}

		public void OpenBuyDianQuanPanel(CUIEvent uiEvent)
		{
			if (this.IsOpenPaySys())
			{
				Singleton<CChatController>.GetInstance().ShowPanel(true, false);
				CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CPaySystem.s_buyDianQuanFormPath, false, true);
				if (cUIFormScript == null)
				{
					return;
				}
				GameObject obj = cUIFormScript.m_formWidgets[0];
				GameObject gameObject = cUIFormScript.m_formWidgets[1];
				if (NetworkAccelerator.IsCommercialized())
				{
					obj.CustomSetActive(false);
					gameObject.CustomSetActive(true);
					if (CUIRedDotSystem.IsShowRedDotByVersion(enRedID.Lobby_PayEntry))
					{
						CUIRedDotSystem.AddRedDot(gameObject, enRedDotPos.enTopRight, 0, 0, 0);
					}
				}
				else
				{
					obj.CustomSetActive(CTongCaiSys.IsShowBuyTongCaiBtn());
					gameObject.CustomSetActive(false);
				}
				Singleton<CTopLobbyEntry>.GetInstance().OpenForm();
				this.RefreshBuyDianQuanPanel();
				MonoSingleton<NobeSys>.GetInstance().ShowNobeTipsInDiamond();
				if (uiEvent.m_eventID != enUIEventID.Pay_OpenBuyDianQuanPanelWithLobby)
				{
					Transform transform = cUIFormScript.transform.Find("bg/btnClose");
					if (transform)
					{
						CUIEventScript component = transform.GetComponent<CUIEventScript>();
						if (component != null)
						{
							component.SetUIEvent(enUIEventType.Click, enUIEventID.None);
							component.m_closeFormWhenClicked = true;
						}
					}
				}
			}
		}

		public bool IsOpenPaySys()
		{
			return ApolloConfig.payEnabled;
		}

		private bool IsDianQuanHaveFirstPay(uint id)
		{
			id -= 1u;
			enNewbieAchieve inIndex = enNewbieAchieve.Androiod_Diamond_0 + (int)id;
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			return masterRoleInfo.IsNewbieAchieveSet((int)inIndex);
		}

		private void SetDianQuanFirstPay(int id)
		{
			id--;
			enNewbieAchieve inIndex = enNewbieAchieve.Androiod_Diamond_0 + id;
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			masterRoleInfo.SetNewbieAchieve((int)inIndex, true, true);
		}

		private void SortDianQuanInfoList()
		{
			int count = this.dianQuanBuyInfoList.Count;
			for (int i = 0; i < count - 1; i++)
			{
				for (int j = 0; j < count - 1 - i; j++)
				{
					bool flag = false;
					bool flag2 = this.dianQuanBuyInfoList[j].bFirstGift > 0 && !this.IsDianQuanHaveFirstPay(this.dianQuanBuyInfoList[j].dwID);
					bool flag3 = this.dianQuanBuyInfoList[j + 1].bFirstGift > 0 && !this.IsDianQuanHaveFirstPay(this.dianQuanBuyInfoList[j + 1].dwID);
					if (flag2 == flag3)
					{
						if (this.dianQuanBuyInfoList[j].dwBuyCount > this.dianQuanBuyInfoList[j + 1].dwBuyCount)
						{
							flag = true;
						}
					}
					else if (!flag2 && flag3)
					{
						flag = true;
					}
					if (flag)
					{
						ResCouponsBuyInfo value = this.dianQuanBuyInfoList[j];
						this.dianQuanBuyInfoList[j] = this.dianQuanBuyInfoList[j + 1];
						this.dianQuanBuyInfoList[j + 1] = value;
					}
				}
			}
		}

		private void RefreshFirstPayPanel()
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CPaySystem.s_firstPayFormPath);
			if (form == null)
			{
				return;
			}
			uint dwConfValue = GameDataMgr.globalInfoDatabin.GetDataByKey(60u).dwConfValue;
			ResRandomRewardStore dataByKey = GameDataMgr.randomRewardDB.GetDataByKey(dwConfValue);
			Transform transform = form.transform.Find("Panel_FirstPay/bodyPanel");
			CUIListScript component = transform.Find("rewardList").GetComponent<CUIListScript>();
			this.RefreshRewardList(form, component, ref dataByKey.astRewardDetail, 0);
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			bool flag = masterRoleInfo.IsGuidedStateSet(22);
			bool flag2 = masterRoleInfo.IsGuidedStateSet(23);
			GameObject gameObject = transform.Find("payButton").gameObject;
			GameObject gameObject2 = transform.Find("getButton").gameObject;
			CUICommonSystem.SetButtonName(gameObject, Singleton<CTextManager>.GetInstance().GetText("Pay_Btn_Top_Up"));
			gameObject.CustomSetActive(!flag);
			CUICommonSystem.SetButtonName(gameObject2, Singleton<CTextManager>.GetInstance().GetText("Pay_Btn_Get_Reward"));
			gameObject2.CustomSetActive(flag && !flag2);
		}

		private void RefreshRenewalPanel()
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CPaySystem.s_renewalDianQuanFormPath);
			if (form == null)
			{
				return;
			}
			uint dwConfValue = GameDataMgr.globalInfoDatabin.GetDataByKey(61u).dwConfValue;
			ResRandomRewardStore dataByKey = GameDataMgr.randomRewardDB.GetDataByKey(dwConfValue);
			Transform transform = form.transform.Find("Panel_Renewal/bodyPanel");
			Text component = transform.Find("rewardDescText").GetComponent<Text>();
			component.text = StringHelper.UTF8BytesToString(ref dataByKey.szRewardDesc);
			GameObject gameObject = transform.Find("itemCellFirst").gameObject;
			CUseable itemUseable = CUseableManager.CreateUsableByRandowReward((RES_RANDOM_REWARD_TYPE)dataByKey.astRewardDetail[0].bItemType, (int)dataByKey.astRewardDetail[0].dwLowCnt, dataByKey.astRewardDetail[0].dwItemID);
			CUICommonSystem.SetItemCell(form, gameObject, itemUseable, true, false, false, false);
			CUIListScript component2 = transform.Find("List").GetComponent<CUIListScript>();
			this.RefreshRewardList(form, component2, ref dataByKey.astRewardDetail, 1);
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			bool flag = masterRoleInfo.IsGuidedStateSet(24);
			bool flag2 = masterRoleInfo.IsGuidedStateSet(25);
			GameObject gameObject2 = transform.Find("payButton").gameObject;
			GameObject gameObject3 = transform.Find("getButton").gameObject;
			CUICommonSystem.SetButtonName(gameObject2, Singleton<CTextManager>.GetInstance().GetText("Pay_Btn_Top_Up"));
			gameObject2.CustomSetActive(!flag);
			CUICommonSystem.SetButtonName(gameObject3, Singleton<CTextManager>.GetInstance().GetText("Pay_Btn_Get_Reward"));
			gameObject3.CustomSetActive(flag && !flag2);
		}

		private void RefreshRewardList(CUIFormScript form, CUIListScript listScript, ref ResDT_RandomRewardInfo[] rewardInfoArr, int index = 0)
		{
			if (form == null || listScript == null)
			{
				return;
			}
			int num = 0;
			for (int i = index; i < rewardInfoArr.Length; i++)
			{
				if (rewardInfoArr[i].bItemType == 0)
				{
					break;
				}
				num++;
			}
			listScript.SetElementAmount(num);
			for (int j = 0; j < num; j++)
			{
				CUIListElementScript elemenet = listScript.GetElemenet(j);
				CUseable itemUseable = CUseableManager.CreateUsableByRandowReward((RES_RANDOM_REWARD_TYPE)rewardInfoArr[j + index].bItemType, (int)rewardInfoArr[j + index].dwLowCnt, rewardInfoArr[j + index].dwItemID);
				GameObject gameObject = elemenet.transform.Find("itemCell").gameObject;
				CUICommonSystem.SetItemCell(form, gameObject, itemUseable, true, false, false, false);
			}
		}

		public void OnPandroiaPaySuccss()
		{
			if (this.m_pandoraDianQuanReqSeq == -1)
			{
				CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
				if (masterRoleInfo != null)
				{
					this.m_lastDianQuan = masterRoleInfo.DianQuan;
				}
				this.m_pandoraDianQuanReqSeq = Singleton<CTimerManager>.GetInstance().AddTimer(5000, 3, new CTimer.OnTimeUpHandler(this.ReqAcntDianQuanHanlder));
			}
		}

		private void ReqAcntDianQuanHanlder(int seq)
		{
			CS_COUPONS_PAYTYPE payType = CS_COUPONS_PAYTYPE.CS_COUPONS_PAYTYPE_QUERY;
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				CPaySystem.SendReqQueryAcntDianQuan(payType, false);
			}
			else
			{
				ulong dianQuan = masterRoleInfo.DianQuan;
				if (this.m_lastDianQuan != dianQuan)
				{
					Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref seq);
					this.m_pandoraDianQuanReqSeq = -1;
					this.m_lastDianQuan = dianQuan;
				}
				else
				{
					CPaySystem.SendReqQueryAcntDianQuan(payType, false);
				}
			}
		}

		private void OnPaySuccess()
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			CS_COUPONS_PAYTYPE payType = CS_COUPONS_PAYTYPE.CS_COUPONS_PAYTYPE_NULL;
			if (!masterRoleInfo.IsGuidedStateSet(22))
			{
				payType = CS_COUPONS_PAYTYPE.CS_COUPONS_PAYTYPE_FIRST;
			}
			else if (!masterRoleInfo.IsGuidedStateSet(24))
			{
				payType = CS_COUPONS_PAYTYPE.CS_COUPONS_PAYTYPE_RENEW;
			}
			CPaySystem.SendReqQueryAcntDianQuan(payType, true);
			this.SetDianQuanFirstPay(this.m_dianQuanGiftId);
			this.RefreshBuyDianQuanPanel();
			this.RefreshFirstPayPanel();
			this.RefreshRenewalPanel();
		}

		private void OnPayRiskHit(int errCode)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			Singleton<CUIManager>.GetInstance().OpenMessageBox(Singleton<CTextManager>.GetInstance().GetText(string.Format("Err_Common_Pay_Risk_Hit_{0}", errCode)), false);
		}

		private void OnPayFailed()
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			Singleton<CUIManager>.GetInstance().OpenTips("Err_Common_Pay_Failed", true, 1.5f, null, new object[0]);
		}

		private void OnNeedLogin()
		{
			Singleton<CUIManager>.GetInstance().OpenTips("Common_Need_Login", true, 1.5f, null, new object[0]);
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<ApolloAccountInfo>(EventID.ApolloHelper_Login_Success, new Action<ApolloAccountInfo>(LobbyMsgHandler.SendMidasToken));
			Singleton<EventRouter>.GetInstance().AddEventHandler<ApolloAccountInfo>(EventID.ApolloHelper_Login_Success, new Action<ApolloAccountInfo>(LobbyMsgHandler.SendMidasToken));
			Singleton<CTimerManager>.GetInstance().AddTimer(500, 1, new CTimer.OnTimeUpHandler(this.SwitchToLogin));
			Singleton<CUIManager>.GetInstance().OpenSendMsgAlert(5, enUIEventID.None);
		}

		private void SwitchToLogin(int seq)
		{
			Singleton<ApolloHelper>.GetInstance().Login(Singleton<ApolloHelper>.GetInstance().CurPlatform, true, 0uL, null);
		}

		private void OnClickGetNewHeroPanel(CUIEvent uiEvent)
		{
			if (this.rewardItems != null && this.rewardItems.Count > 0)
			{
				Singleton<CUIManager>.GetInstance().OpenAwardTip(LinqS.ToArray<CUseable>(this.rewardItems), null, false, enUIEventID.None, false, false, "Form_Award");
			}
		}

		private void OnPlayHeroVideo(CUIEvent uiEvent)
		{
			Handheld.PlayFullScreenMovie("Video/HeroVideo.mp4", Color.black, FullScreenMovieControlMode.CancelOnInput, FullScreenMovieScalingMode.AspectFit);
		}

		public static void SendReqQueryAcntDianQuan(CS_COUPONS_PAYTYPE payType = CS_COUPONS_PAYTYPE.CS_COUPONS_PAYTYPE_QUERY, bool isAlert = true)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1150u);
			cSPkg.stPkgData.stAcntCouponsReq.bPayType = (byte)payType;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, isAlert);
		}

		public static void SendReqGetDianQuanReward(CS_COUPONS_PAYTYPE payType)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1160u);
			cSPkg.stPkgData.stCouponsRewardReq.bPayType = (byte)payType;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		[MessageHandler(1161)]
		public static void OnReceiveDianQuanReward(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			SCPKG_CMD_COUPONS_REWARDINFO stCouponsRewardRsp = msg.stPkgData.stCouponsRewardRsp;
			bool flag = true;
			CPaySystem instance = Singleton<CPaySystem>.GetInstance();
			instance.rewardItems.Clear();
			int num = Mathf.Min((int)stCouponsRewardRsp.stRewardInfo.bNum, stCouponsRewardRsp.stRewardInfo.astRewardDetail.Length);
			for (int i = 0; i < num; i++)
			{
				if (stCouponsRewardRsp.stRewardInfo.astRewardDetail[i].bType == 5)
				{
					CUICommonSystem.ShowNewHeroOrSkin(stCouponsRewardRsp.stRewardInfo.astRewardDetail[i].stRewardInfo.stHero.dwHeroID, 0u, enUIEventID.Pay_ClickGetNewHeroPanel, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_HERO, false, null, enFormPriority.Priority1, 0u, 0);
					flag = false;
					break;
				}
				if (stCouponsRewardRsp.stRewardInfo.astRewardDetail[i].bType == 10)
				{
					uint heroId;
					uint skinId;
					CSkinInfo.ResolveHeroSkin(stCouponsRewardRsp.stRewardInfo.astRewardDetail[i].stRewardInfo.stSkin.dwSkinID, out heroId, out skinId);
					CUICommonSystem.ShowNewHeroOrSkin(heroId, skinId, enUIEventID.Pay_ClickGetNewHeroPanel, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_SKIN, false, null, enFormPriority.Priority1, 0u, 0);
					flag = false;
					break;
				}
				if (stCouponsRewardRsp.stRewardInfo.astRewardDetail[i].bType == 1)
				{
					if (stCouponsRewardRsp.stRewardInfo.astRewardDetail[i].bFromType == 1)
					{
						uint dwHeroID = stCouponsRewardRsp.stRewardInfo.astRewardDetail[i].stFromInfo.stHeroInfo.dwHeroID;
						ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(dwHeroID);
						if (dataByKey != null)
						{
							ResHeroShop resHeroShop = null;
							GameDataMgr.heroShopInfoDict.TryGetValue(dataByKey.dwCfgID, out resHeroShop);
							CUICommonSystem.ShowNewHeroOrSkin(dwHeroID, 0u, enUIEventID.Pay_ClickGetNewHeroPanel, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_HERO, true, null, enFormPriority.Priority1, (resHeroShop == null) ? 1u : resHeroShop.dwChgItemCnt, 0);
						}
					}
					else if (stCouponsRewardRsp.stRewardInfo.astRewardDetail[i].bFromType == 2)
					{
						uint dwSkinID = stCouponsRewardRsp.stRewardInfo.astRewardDetail[i].stFromInfo.stSkinInfo.dwSkinID;
						uint heroId2 = 0u;
						uint skinId2 = 0u;
						CSkinInfo.ResolveHeroSkin(dwSkinID, out heroId2, out skinId2);
						ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(heroId2, skinId2);
						if (heroSkin != null)
						{
							ResHeroSkinShop resHeroSkinShop = null;
							GameDataMgr.skinShopInfoDict.TryGetValue(heroSkin.dwID, out resHeroSkinShop);
							CUICommonSystem.ShowNewHeroOrSkin(heroId2, skinId2, enUIEventID.Pay_ClickGetNewHeroPanel, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_HERO, true, null, enFormPriority.Priority1, (resHeroSkinShop == null) ? 1u : resHeroSkinShop.dwChgItemCnt, 0);
						}
					}
				}
			}
			instance.rewardItems = CUseableManager.GetUseableListFromReward(stCouponsRewardRsp.stRewardInfo);
			if (flag)
			{
				Singleton<CUIManager>.GetInstance().OpenAwardTip(LinqS.ToArray<CUseable>(instance.rewardItems), null, false, enUIEventID.None, false, false, "Form_Award");
			}
		}

		[MessageHandler(1191)]
		public static void OnReceiveNewbitSyn(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			SCPKG_NTF_NEWIEBITSYN stNewieBitSyn = msg.stPkgData.stNewieBitSyn;
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			masterRoleInfo.SetGuidedStateSet((int)stNewieBitSyn.dwBitType, true);
			if (stNewieBitSyn.dwBitType == 22u || stNewieBitSyn.dwBitType == 23u || stNewieBitSyn.dwBitType == 24u || stNewieBitSyn.dwBitType == 25u)
			{
				CLobbySystem.RefreshDianQuanPayButton(true);
				Singleton<CPaySystem>.GetInstance().AutoOpenRewardPanel(true);
			}
		}

		public void AutoOpenRewardPanel(bool checkLobby)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (!checkLobby || LobbyLogic.IsLobbyFormPure())
			{
				if (masterRoleInfo.IsGuidedStateSet(22) && !masterRoleInfo.IsGuidedStateSet(23))
				{
					Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Pay_OpenFirstPayPanel);
				}
				else if (masterRoleInfo.IsGuidedStateSet(24) && !masterRoleInfo.IsGuidedStateSet(25))
				{
					Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Pay_OpenRenewalPanel);
				}
			}
		}
	}
}
