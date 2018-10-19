using Apollo;
using Assets.Scripts.Framework;
using Assets.Scripts.UI;
using ResData;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class CPartnerSystem : Singleton<CPartnerSystem>
	{
		public const string s_partnerFormPath = "UGUI/Form/System/Pay/Form_Partner.prefab";

		public override void Init()
		{
			base.Init();
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Partner_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnOpenForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Partner_OpenXunYou_Buy, new CUIEventManager.OnUIEventHandler(this.OnOpenXunYouBuy));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Partner_Refresh_Entry, new CUIEventManager.OnUIEventHandler(this.OnRefreshEntry));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.OPEN_TongCai, new CUIEventManager.OnUIEventHandler(this.OnOpenTongCaiBuy));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Partner_OpenTongcai_Buy_Confirm, new CUIEventManager.OnUIEventHandler(this.OnOpenTongCaiConfirm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Partner_OpenXunYou_Buy_Confirm, new CUIEventManager.OnUIEventHandler(this.OnOpenXunYouBuyConfirm));
		}

		public override void UnInit()
		{
			base.UnInit();
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Partner_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnOpenForm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Partner_OpenXunYou_Buy, new CUIEventManager.OnUIEventHandler(this.OnOpenXunYouBuy));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Partner_Refresh_Entry, new CUIEventManager.OnUIEventHandler(this.OnRefreshEntry));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.OPEN_TongCai, new CUIEventManager.OnUIEventHandler(this.OnOpenTongCaiBuy));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Partner_OpenTongcai_Buy_Confirm, new CUIEventManager.OnUIEventHandler(this.OnOpenTongCaiConfirm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Partner_OpenXunYou_Buy_Confirm, new CUIEventManager.OnUIEventHandler(this.OnOpenXunYouBuyConfirm));
		}

		private void OnOpenForm(CUIEvent uiEvent)
		{
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/System/Pay/Form_Partner.prefab", false, true);
			if (cUIFormScript != null && NetworkAccelerator.IsCommercialized())
			{
				CUIRedDotSystem.SetRedDotViewByVersion(enRedID.Lobby_PayEntry);
				Singleton<CUINewFlagSystem>.GetInstance().SetNewFlagForXunYouBuy(true);
			}
			this.UpdateEntryStatus(cUIFormScript);
		}

		private void OnOpenXunYouBuy(CUIEvent uiEvent)
		{
			uint srv2CltGlobalValue = GameDataMgr.GetSrv2CltGlobalValue(RES_SRV2CLT_GLOBAL_CONF_TYPE.RES_SRV2CLT_GLOBAL_CONF_TYPE_TONGCAI_XUNYOU_MUTEX_TIPS_2);
			bool flag = srv2CltGlobalValue == 1u;
			bool flag2 = MonoSingleton<CTongCaiSys>.GetInstance().IsTongCaiUserAndCanUse();
			flag = (flag && flag2);
			if (flag)
			{
				Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("Partner_Mutext_Tips_2"), enUIEventID.Partner_OpenXunYou_Buy_Confirm, enUIEventID.None, false);
			}
			else
			{
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Partner_OpenXunYou_Buy_Confirm);
			}
		}

		private void OnOpenXunYouBuyConfirm(CUIEvent uiEvent)
		{
			Singleton<CUINewFlagSystem>.GetInstance().SetNewFlagForXunYouBuy(false);
			string webUIUrl = NetworkAccelerator.getWebUIUrl();
			if (!string.IsNullOrEmpty(webUIUrl))
			{
				CUICommonSystem.OpenUrl(webUIUrl, true, 2);
			}
		}

		public void OnOpenTongCaiBuy(CUIEvent uiEvent)
		{
			uint srv2CltGlobalValue = GameDataMgr.GetSrv2CltGlobalValue(RES_SRV2CLT_GLOBAL_CONF_TYPE.RES_SRV2CLT_GLOBAL_CONF_TYPE_TONGCAI_XUNYOU_MUTEX_TIPS_1);
			bool flag = srv2CltGlobalValue == 1u;
			NetworkAccelerator.UserStatus userStatus = NetworkAccelerator.GetUserStatus();
			flag = (flag && (userStatus == NetworkAccelerator.UserStatus.InUse || userStatus == NetworkAccelerator.UserStatus.FreeTrial));
			if (flag)
			{
				Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("Partner_Mutext_Tips_1"), enUIEventID.Partner_OpenTongcai_Buy_Confirm, enUIEventID.None, false);
			}
			else
			{
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Partner_OpenTongcai_Buy_Confirm);
			}
		}

		public void OnOpenTongCaiConfirm(CUIEvent uiEvent)
		{
			ApolloAccountInfo accountInfo = Singleton<ApolloHelper>.GetInstance().GetAccountInfo(false);
			MonoSingleton<CTongCaiSys>.instance.OpenTongCaiH5(accountInfo);
		}

		private void OnRefreshEntry(CUIEvent uiEvent)
		{
			CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
			if (srcFormScript != null)
			{
				this.UpdateEntryStatus(srcFormScript);
			}
		}

		public static void RefreshSysEntryChargeRedDot()
		{
			if (!NetworkAccelerator.IsCommercialized())
			{
				return;
			}
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CLobbySystem.SYSENTRY_FORM_PATH);
			if (form != null)
			{
				GameObject gameObject = Utility.FindChild(form.gameObject, "PlayerBtn/Dianquan/Button");
				if (gameObject != null && CUIRedDotSystem.IsShowRedDotByVersion(enRedID.Lobby_PayEntry))
				{
					CUIRedDotSystem.AddRedDot(gameObject, enRedDotPos.enTopRight, 0, 0, 0);
				}
			}
		}

		public static void HideSysEntryChargeRedDot()
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CLobbySystem.SYSENTRY_FORM_PATH);
			if (form != null)
			{
				GameObject gameObject = Utility.FindChild(form.gameObject, "PlayerBtn/Dianquan/Button");
				if (gameObject != null)
				{
					CUIRedDotSystem.DelRedDot(gameObject);
				}
			}
		}

		private void UpdateEntryStatus(CUIFormScript form)
		{
			if (form == null)
			{
				return;
			}
			GameObject widget = form.GetWidget(0);
			GameObject widget2 = form.GetWidget(1);
			this.UpdateTongCaiStatus(widget);
			this.UpdateXunYouStatus(widget2);
		}

		private void UpdateTongCaiStatus(GameObject container)
		{
			if (container == null)
			{
				DebugHelper.Assert(false, "TongCai container is null");
				return;
			}
			if (CSysDynamicBlock.bLobbyEntryBlocked)
			{
				container.CustomSetActive(false);
				return;
			}
			container.CustomSetActive(CTongCaiSys.IsShowBuyTongCaiBtn());
		}

		private void UpdateXunYouStatus(GameObject container)
		{
			if (container == null)
			{
				DebugHelper.Assert(false, "XunYou container is null");
				return;
			}
			if (CSysDynamicBlock.bLobbyEntryBlocked)
			{
				container.CustomSetActive(false);
				return;
			}
			NetworkAccelerator.UserStatus userStatus = NetworkAccelerator.GetUserStatus();
			string vIPValidTime = NetworkAccelerator.getVIPValidTime();
			if (NetworkAccelerator.IsCommercialized() && userStatus != NetworkAccelerator.UserStatus.NotQualified)
			{
				container.CustomSetActive(true);
				GameObject gameObject = Utility.FindChild(container, "txtContainer/desc");
				GameObject gameObject2 = Utility.FindChild(container, "txtContainer/time");
				gameObject.CustomSetActive(true);
				gameObject2.CustomSetActive(true);
				if (gameObject != null && gameObject2 != null)
				{
					Text component = gameObject.GetComponent<Text>();
					Text component2 = gameObject2.GetComponent<Text>();
					if (component != null && component2 != null)
					{
						switch (userStatus)
						{
						case NetworkAccelerator.UserStatus.Qualified:
							component.text = "迅游加速，助你超神";
							component2.text = string.Empty;
							gameObject2.CustomSetActive(false);
							break;
						case NetworkAccelerator.UserStatus.FreeTrial:
							component.text = "免费试用中";
							component2.text = "试用期至 " + vIPValidTime;
							break;
						case NetworkAccelerator.UserStatus.TrialExpired:
							component.text = "试用结束，付费开启加速";
							component2.text = vIPValidTime + "试用已过期";
							break;
						case NetworkAccelerator.UserStatus.InUse:
							component.text = "加速中";
							component2.text = "有效期至 " + vIPValidTime;
							break;
						case NetworkAccelerator.UserStatus.Expired:
							component.text = "续费加速服务";
							component2.text = vIPValidTime + " 已过期";
							break;
						}
					}
				}
			}
			else
			{
				container.CustomSetActive(false);
			}
		}
	}
}
