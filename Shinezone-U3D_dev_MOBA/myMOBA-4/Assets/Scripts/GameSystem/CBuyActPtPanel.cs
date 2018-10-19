using Assets.Scripts.Framework;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	internal class CBuyActPtPanel : CBuyPanelBase
	{
		private int m_CostDiamond;

		private int m_GainActPt;

		private Text m_content;

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
				return base.IsHaveEnoughTimes(1) && base.IsHaveEnoughDianQuan(1) && !this.willOverMaxAP(1);
			}
		}

		public void init()
		{
			this.FormPath = "UGUI/Form/System/Purchase/Form_BuyActionPoint.prefab";
			this.m_MaxBuyTime = (int)GameDataMgr.globalInfoDatabin.GetDataByKey(23u).dwConfValue;
			Singleton<EventRouter>.GetInstance().AddEventHandler("MasterAttributesChanged", new Action(this.onAtrrChange));
		}

		public void unInit()
		{
			Singleton<EventRouter>.GetInstance().RemoveEventHandler("MasterAttributesChanged", new Action(this.onAtrrChange));
		}

		private void onAtrrChange()
		{
			this.RefreshUI();
		}

		public override void initPanel(CUIFormScript form)
		{
			base.initPanel(form);
			this.m_content = this.m_FormScript.transform.Find("Content").GetComponent<Text>();
			this.m_Buttons = new GameObject[2];
			this.m_Buttons[0] = this.m_FormScript.transform.Find("ButtonGroup/Button_Confirm").gameObject;
			this.m_Buttons[1] = this.m_FormScript.transform.Find("ButtonGroup/Button_Cancel").gameObject;
			this.m_CheckRightButton = this.m_FormScript.transform.Find("ButtonGroup/Button_CheckRight").gameObject;
			this.RefreshUI();
		}

		private void updateButtonState()
		{
			if (!base.bTimesOut)
			{
				base.showVipButton(false);
			}
			else
			{
				base.showVipButton(true);
			}
		}

		private void RefreshUI()
		{
			if (this.bOpen && !this.bShopping)
			{
				if (!base.bTimesOut)
				{
					this.m_content.text = Singleton<CTextManager>.GetInstance().GetText("BuyAct_Confirm", new string[]
					{
						this.m_CostDiamond.ToString(),
						this.m_GainActPt.ToString(),
						this.CurBuyTime.ToString()
					});
				}
				else
				{
					this.m_content.text = Singleton<CTextManager>.GetInstance().GetText("BuyAct_NoMore", new string[]
					{
						this.CurBuyTime.ToString(),
						this.m_MaxBuyTime.ToString()
					});
				}
				this.updateButtonState();
			}
		}

		private void calcComsume()
		{
			ResShopInfo dataByKey = GameDataMgr.resShopInfoDatabin.GetDataByKey((uint)((ushort)(400 + this.m_CurBuyTime + 1)));
			if (dataByKey != null)
			{
				this.m_CostDiamond = (int)dataByKey.dwCoinPrice;
				this.m_GainActPt = (int)dataByKey.dwValue;
			}
			else
			{
				this.m_CostDiamond = 0;
				this.m_GainActPt = 0;
			}
		}

		protected override uint GetRequireDianquan(int times)
		{
			uint num = 0u;
			for (int i = 0; i < times; i++)
			{
				uint dwCoinPrice = GameDataMgr.resShopInfoDatabin.GetDataByKey((uint)((ushort)(400 + this.m_CurBuyTime + 1 + i))).dwCoinPrice;
				if ((ulong)(dwCoinPrice + num) > 0xffffffffL)
				{
					return 4294967295u;
				}
				num += dwCoinPrice;
			}
			return num;
		}

		protected bool willOverMaxAP(int times)
		{
			int num = 0;
			for (int i = 0; i < times; i++)
			{
				num += (int)GameDataMgr.resShopInfoDatabin.GetDataByKey((uint)((ushort)(400 + this.m_CurBuyTime + 1))).dwValue;
			}
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			int dwConfValue = (int)GameDataMgr.globalInfoDatabin.GetDataByKey(3u).dwConfValue;
			return (ulong)masterRoleInfo.CurActionPoint + (ulong)((long)num) > (ulong)((long)dwConfValue);
		}

		public void Buy()
		{
			if (this.bCanBuy)
			{
				CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1113u);
				cSPkg.stPkgData.stShopBuyReq = new CSPKG_CMD_SHOPBUY();
				cSPkg.stPkgData.stShopBuyReq.iBuyType = 4;
				cSPkg.stPkgData.stShopBuyReq.iBuySubType = this.CurBuyTime + 1;
				Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
			}
			else if (!base.IsHaveEnoughDianQuan(1))
			{
				CUICommonSystem.OpenDianQuanNotEnoughTip();
			}
			else if (this.willOverMaxAP(1))
			{
				Singleton<CUIManager>.GetInstance().OpenTips("将超过体力最大上限值！", false, 1.5f, null, new object[0]);
			}
		}

		public override void BuyRsp(CSPkg msg)
		{
			base.BuyRsp(msg);
		}

		public void SetSvrData(ref CSDT_ACNT_SHOPBUY_INFO ShopBuyInfo)
		{
			this.CurBuyTime = this.m_MaxBuyTime - ShopBuyInfo.LeftShopBuyCnt[1];
		}
	}
}
