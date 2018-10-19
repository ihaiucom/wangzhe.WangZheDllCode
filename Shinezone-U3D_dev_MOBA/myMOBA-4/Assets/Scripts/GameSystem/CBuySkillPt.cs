using Assets.Scripts.Framework;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	internal class CBuySkillPt : CBuyPanelBase
	{
		private int m_CostDiamond;

		private int m_Gain;

		private Text m_content;

		public override int CurBuyTime
		{
			get
			{
				return this.m_CurBuyTime;
			}
			set
			{
				this.m_CurBuyTime = value;
				this.calcComsume();
				this.RefreshUI();
			}
		}

		public override bool bCanBuy
		{
			get
			{
				return base.IsHaveEnoughDianQuan(1) && base.IsHaveEnoughTimes(1) && this.bSkillPointZero;
			}
		}

		private bool bSkillPointZero
		{
			get
			{
				return Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_skillPoint <= 0u;
			}
		}

		public void init()
		{
			this.FormPath = "UGUI/Form/System/Purchase/Form_BuySkillPoint.prefab";
			this.m_MaxBuyTime = (int)GameDataMgr.globalInfoDatabin.GetDataByKey(27u).dwConfValue;
		}

		public void unInit()
		{
		}

		private void onAtrrChange()
		{
			this.RefreshUI();
		}

		public override void initPanel(CUIFormScript form)
		{
			base.initPanel(form);
			this.m_content = this.m_FormScript.transform.FindChild("Content").GetComponent<Text>();
			this.m_Buttons = new GameObject[2];
			this.m_Buttons[0] = this.m_FormScript.transform.Find("ButtonGroup/Button_Confirm").gameObject;
			this.m_Buttons[1] = this.m_FormScript.transform.Find("ButtonGroup/Button_Cancel").gameObject;
			this.m_CheckRightButton = this.m_FormScript.transform.Find("ButtonGroup/Button_CheckRight").gameObject;
			this.RefreshUI();
			Singleton<EventRouter>.GetInstance().AddEventHandler("MasterAttributesChanged", new Action(this.onAtrrChange));
		}

		private void calcComsume()
		{
			ResShopInfo dataByKey = GameDataMgr.resShopInfoDatabin.GetDataByKey((uint)((ushort)(500 + this.m_CurBuyTime + 1)));
			if (dataByKey != null)
			{
				this.m_CostDiamond = (int)dataByKey.dwCoinPrice;
				this.m_Gain = (int)dataByKey.dwValue;
			}
			else
			{
				this.m_CostDiamond = 0;
				this.m_Gain = 0;
			}
		}

		private void RefreshUI()
		{
			if (!this.bOpen)
			{
				return;
			}
			if (!base.bTimesOut)
			{
				this.m_content.text = Singleton<CTextManager>.GetInstance().GetText("BuyAct_NoSkillPoint", new string[]
				{
					this.m_Gain.ToString(),
					this.m_CostDiamond.ToString(),
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

		protected override uint GetRequireDianquan(int times)
		{
			uint num = 0u;
			for (int i = 0; i < times; i++)
			{
				uint dwCoinPrice = GameDataMgr.resShopInfoDatabin.GetDataByKey((uint)((ushort)(500 + this.m_CurBuyTime + 1 + i))).dwCoinPrice;
				if ((ulong)(dwCoinPrice + num) > 0xffffffffL)
				{
					return 4294967295u;
				}
				num += dwCoinPrice;
			}
			return num;
		}

		public void Buy()
		{
			if (this.bCanBuy)
			{
				CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1113u);
				cSPkg.stPkgData.stShopBuyReq.iBuyType = 5;
				cSPkg.stPkgData.stShopBuyReq.iBuySubType = this.CurBuyTime + 1;
				Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
			}
			else if (!base.IsHaveEnoughDianQuan(1))
			{
				Singleton<CUIManager>.GetInstance().OpenTips("没有足够的钻石！", false, 1.5f, null, new object[0]);
			}
			else if (!this.bSkillPointZero)
			{
				Singleton<CUIManager>.GetInstance().OpenTips("当前技能点不为零，不能继续购买！", false, 1.5f, null, new object[0]);
			}
		}

		public override void BuyRsp(CSPkg msg)
		{
			base.BuyRsp(msg);
		}

		public void SetSvrData(ref CSDT_ACNT_SHOPBUY_INFO ShopBuyInfo)
		{
			this.CurBuyTime = this.m_MaxBuyTime - ShopBuyInfo.LeftShopBuyCnt[2];
		}
	}
}
