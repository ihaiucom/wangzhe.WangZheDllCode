using Assets.Scripts.UI;
using CSProtocol;
using System;
using UnityEngine;

namespace Assets.Scripts.GameSystem
{
	internal abstract class CBuyPanelBase
	{
		protected string FormPath = string.Empty;

		protected bool m_bShowCheckRight;

		protected CUIFormScript m_FormScript;

		protected int m_CurBuyTime;

		protected int m_MaxBuyTime;

		protected GameObject[] m_Buttons;

		protected GameObject m_CheckRightButton;

		protected bool bOpen;

		protected bool bShopping;

		public abstract int CurBuyTime
		{
			get;
			set;
		}

		public abstract bool bCanBuy
		{
			get;
		}

		public bool bTimesOut
		{
			get
			{
				return this.m_CurBuyTime >= this.m_MaxBuyTime;
			}
		}

		public virtual void initPanel(CUIFormScript form)
		{
			this.bShopping = false;
		}

		public virtual void open()
		{
			if (this.bOpen)
			{
				return;
			}
			this.m_FormScript = Singleton<CUIManager>.GetInstance().OpenForm(this.FormPath, false, true);
			this.bOpen = true;
			this.initPanel(this.m_FormScript);
		}

		public virtual void close()
		{
			if (!this.bOpen)
			{
				return;
			}
			this.bOpen = false;
			Singleton<CUIManager>.GetInstance().CloseForm(this.FormPath);
		}

		protected void showVipButton(bool bShowCheckRight)
		{
			this.m_bShowCheckRight = bShowCheckRight;
			if (this.m_bShowCheckRight)
			{
				if (this.m_Buttons != null)
				{
					for (int i = 0; i < this.m_Buttons.Length; i++)
					{
						this.m_Buttons[i].CustomSetActive(false);
					}
				}
				if (this.m_CheckRightButton)
				{
					this.m_CheckRightButton.CustomSetActive(true);
				}
			}
			else
			{
				if (this.m_Buttons != null)
				{
					for (int j = 0; j < this.m_Buttons.Length; j++)
					{
						this.m_Buttons[j].CustomSetActive(true);
					}
				}
				if (this.m_CheckRightButton)
				{
					this.m_CheckRightButton.CustomSetActive(false);
				}
			}
		}

		protected bool IsHaveEnoughDianQuan(int times)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			return masterRoleInfo.DianQuan >= (ulong)this.GetRequireDianquan(times);
		}

		protected bool IsHaveEnoughTimes(int TimesToBuy)
		{
			return this.m_MaxBuyTime - this.CurBuyTime >= TimesToBuy;
		}

		protected abstract uint GetRequireDianquan(int times);

		public virtual void BuyRsp(CSPkg msg)
		{
			this.close();
			this.bShopping = true;
			this.CurBuyTime = msg.stPkgData.stShopBuyRsp.iBuySubType;
			int iBuyType = msg.stPkgData.stShopBuyRsp.iBuyType;
			if (iBuyType != 4)
			{
				if (iBuyType == 5)
				{
					Singleton<CUIManager>.GetInstance().OpenTips("BuyAct_BuySpSuccess", true, 1f, null, new object[]
					{
						msg.stPkgData.stShopBuyRsp.iChgValue.ToString()
					});
				}
			}
			else
			{
				Singleton<CUIManager>.GetInstance().OpenTips("BuyAct_BuyApSuccess", true, 1f, null, new object[]
				{
					msg.stPkgData.stShopBuyRsp.iChgValue.ToString()
				});
			}
		}
	}
}
