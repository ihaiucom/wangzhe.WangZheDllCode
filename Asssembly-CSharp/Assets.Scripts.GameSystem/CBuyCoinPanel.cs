using Assets.Scripts.Framework;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	internal class CBuyCoinPanel : CBuyPanelBase
	{
		public enum enBuyCoinPanelWidget
		{
			Action_Panel,
			Notice_Panel
		}

		private int m_CostDiamond;

		private int m_GainCoin;

		private Text m_TitleText;

		private Text m_CostDiamondText;

		private Text m_GainCoinText;

		private CBuyCoinInfoPanel m_InfoPanel;

		public string TitleDescribe
		{
			get
			{
				return string.Format("购买金币({0}/{1})", this.m_CurBuyTime, this.m_MaxBuyTime);
			}
		}

		public override int CurBuyTime
		{
			get
			{
				return this.m_CurBuyTime;
			}
			set
			{
				if (value > this.m_MaxBuyTime)
				{
					return;
				}
				this.m_CurBuyTime = value;
				this.calcComsume();
				this.RefreshUI();
			}
		}

		public override bool bCanBuy
		{
			get
			{
				return base.IsHaveEnoughTimes(1) && base.IsHaveEnoughDianQuan(1);
			}
		}

		public bool bCanBuyTen
		{
			get
			{
				return base.IsHaveEnoughTimes(10) && base.IsHaveEnoughDianQuan(10);
			}
		}

		public void init()
		{
			this.FormPath = "UGUI/Form/System/Purchase/Form_BuyCoin.prefab";
			this.m_InfoPanel = new CBuyCoinInfoPanel();
			this.m_MaxBuyTime = (int)GameDataMgr.globalInfoDatabin.GetDataByKey(22u).dwConfValue;
		}

		public void unInit()
		{
			this.m_InfoPanel = null;
		}

		public override void initPanel(CUIFormScript form)
		{
			base.initPanel(form);
			this.m_TitleText = this.m_FormScript.transform.Find("Bg/Bg1/Title").GetComponent<Text>();
			this.m_Buttons = new GameObject[2];
			this.m_Buttons[0] = this.m_FormScript.transform.Find("Bg/Bg1/ButtonGroup/Button_BuyOne").gameObject;
			this.m_Buttons[1] = this.m_FormScript.transform.Find("Bg/Bg1/ButtonGroup/Button_BuyTen").gameObject;
			this.m_CheckRightButton = this.m_FormScript.transform.Find("Bg/Bg1/ButtonGroup/Button_CheckRight").gameObject;
			this.m_CostDiamondText = this.m_FormScript.transform.Find("Bg/Bg1/pnlAction/DiamondNum").GetComponent<Text>();
			this.m_GainCoinText = this.m_FormScript.transform.Find("Bg/Bg1/pnlAction/CoinNum").GetComponent<Text>();
			this.m_InfoPanel.initInfoPanel(this.m_FormScript.transform.Find("Bg/Bg2").gameObject);
			this.RefreshUI();
			Singleton<EventRouter>.GetInstance().AddEventHandler("MasterAttributesChanged", new Action(this.onAtrrChange));
		}

		public void Buy(int Param)
		{
			if (Param == 1)
			{
				this.BuyOne();
			}
			else if (Param == 10)
			{
				this.BuyTen();
			}
		}

		private void BuyOne()
		{
			if (this.bCanBuy)
			{
				this.sendBuyCoinMsg(0);
			}
			else if (!base.IsHaveEnoughDianQuan(1))
			{
				Singleton<CUIManager>.GetInstance().OpenTips("没有足够的钻石！", false, 1.5f, null, new object[0]);
			}
		}

		private void BuyTen()
		{
			if (this.bCanBuyTen)
			{
				this.sendBuyCoinMsg(1);
			}
			else if (!base.IsHaveEnoughDianQuan(10))
			{
				Singleton<CUIManager>.GetInstance().OpenTips("没有足够的钻石！", false, 1.5f, null, new object[0]);
			}
		}

		private uint calcGainCoin(uint coinBase)
		{
			uint dwConfValue = GameDataMgr.globalInfoDatabin.GetDataByKey(28u).dwConfValue;
			return coinBase + Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().Level * dwConfValue;
		}

		private void calcComsume()
		{
			ResCoinBuyInfo dataByKey = GameDataMgr.coninBuyDatabin.GetDataByKey((uint)((ushort)(this.m_CurBuyTime + 1)));
			if (dataByKey != null)
			{
				this.m_CostDiamond = (int)dataByKey.wCouponsCost;
				this.m_GainCoin = (int)this.calcGainCoin(dataByKey.dwCoinBase);
			}
			else
			{
				this.m_CostDiamond = 0;
				this.m_GainCoin = 0;
			}
		}

		private void RefreshUI()
		{
			if (!this.bOpen)
			{
				return;
			}
			if (this.m_TitleText)
			{
				this.m_TitleText.set_text(this.TitleDescribe);
			}
			this.m_CostDiamondText.set_text(this.m_CostDiamond.ToString());
			this.m_GainCoinText.set_text(this.m_GainCoin.ToString());
			this.updateButtonState();
		}

		private void onAtrrChange()
		{
			this.RefreshUI();
		}

		private void updateButtonState()
		{
			if (!base.bTimesOut)
			{
				if (this.m_FormScript != null)
				{
					GameObject widget = this.m_FormScript.GetWidget(0);
					if (widget != null && !widget.activeSelf)
					{
						widget.CustomSetActive(true);
					}
					GameObject widget2 = this.m_FormScript.GetWidget(1);
					if (widget2 != null && widget2.activeSelf)
					{
						widget2.CustomSetActive(false);
					}
				}
				base.showVipButton(false);
			}
			else
			{
				if (this.m_FormScript != null)
				{
					GameObject widget3 = this.m_FormScript.GetWidget(0);
					if (widget3 != null && widget3.activeSelf)
					{
						widget3.CustomSetActive(false);
					}
					GameObject widget4 = this.m_FormScript.GetWidget(1);
					if (widget4 != null && !widget4.activeSelf)
					{
						widget4.CustomSetActive(true);
					}
				}
				base.showVipButton(true);
			}
			this.refreshButtonState(10);
		}

		private void refreshButtonState(int timeToBuy)
		{
			if (this.m_FormScript == null)
			{
				return;
			}
			GameObject gameObject = this.m_FormScript.transform.Find("Bg/Bg1/ButtonGroup/Button_BuyTen").gameObject;
			CUIEventScript cUIEventScript = gameObject.GetComponent<CUIEventScript>();
			stUIEventParams eventParams = default(stUIEventParams);
			if (cUIEventScript == null)
			{
				cUIEventScript = gameObject.AddComponent<CUIEventScript>();
				cUIEventScript.Initialize(this.m_FormScript);
			}
			if (base.IsHaveEnoughTimes(timeToBuy))
			{
				gameObject.GetComponent<Image>().set_color(new Color(1f, 1f, 1f, 1f));
				eventParams.tag = 1;
			}
			else
			{
				gameObject.GetComponent<Image>().set_color(new Color(0f, 1f, 1f, 1f));
				eventParams.tag = 0;
			}
			cUIEventScript.SetUIEvent(enUIEventType.Click, enUIEventID.Purchase_BuyCoinTen, eventParams);
		}

		protected override uint GetRequireDianquan(int times)
		{
			uint num = 0u;
			for (int i = 0; i < times; i++)
			{
				uint wCouponsCost = (uint)GameDataMgr.coninBuyDatabin.GetDataByKey((uint)((ushort)(this.m_CurBuyTime + 1 + i))).wCouponsCost;
				if ((ulong)(wCouponsCost + num) > (ulong)-1)
				{
					return 4294967295u;
				}
				num += wCouponsCost;
			}
			return num;
		}

		private void sendBuyCoinMsg(int BuyCoinType)
		{
			CSPkg cSPkg = null;
			cSPkg = NetworkModule.CreateDefaultCSPKG(1115u);
			cSPkg.stPkgData.stCoinBuyReq = new CSPKG_CMD_COINBUY();
			cSPkg.stPkgData.stCoinBuyReq.bBuyType = (byte)BuyCoinType;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		public void BuyCoinRsp(CSPkg msg)
		{
			this.CurBuyTime = (int)(msg.stPkgData.stCoinBuyRsp.wBuyStartFreq + (ushort)msg.stPkgData.stCoinBuyRsp.stBuyList.bCoinGetCnt - 1);
			this.m_InfoPanel.BuyCoinRsp(msg);
		}

		public void SetSvrData(ref CSDT_ACNT_SHOPBUY_INFO ShopBuyInfo)
		{
			this.CurBuyTime = this.m_MaxBuyTime - ShopBuyInfo.LeftShopBuyCnt[0];
		}
	}
}
