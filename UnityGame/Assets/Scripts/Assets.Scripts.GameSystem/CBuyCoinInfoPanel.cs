using Assets.Scripts.Framework;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	internal class CBuyCoinInfoPanel
	{
		private int m_InfoNum;

		private int m_LimitInfoNum = 100;

		private int m_CostDiamond;

		private int m_GainCoin;

		private GameObject m_InfoPanel;

		private List<BuyCoinInfo> m_stInfos = new List<BuyCoinInfo>();

		private int InfoNum
		{
			get
			{
				return this.m_InfoNum;
			}
			set
			{
				this.m_InfoNum = value;
			}
		}

		public void initInfoPanel(GameObject infoPanel)
		{
			this.m_InfoPanel = infoPanel;
			this.refreshPanel();
		}

		public void addInfo(BuyCoinInfo info)
		{
			if (this.InfoNum < this.m_LimitInfoNum)
			{
				this.m_stInfos.Add(info);
				this.InfoNum++;
			}
			else
			{
				this.m_stInfos.RemoveAt(0);
				this.m_stInfos.Add(info);
			}
			this.refreshPanel();
		}

		private string GetDescribeStr(BuyCoinInfo info)
		{
			string result = null;
			if (info.m_CritTime == 1)
			{
				result = string.Format("使用{0}钻石获得{1}金币", info.m_CostDiamond, info.m_GainCoin);
			}
			if (info.m_CritTime >= 2)
			{
				result = string.Format("使用{0}钻石获得{1}金币 暴击*{2}", info.m_CostDiamond, info.m_GainCoin, info.m_CritTime);
			}
			return result;
		}

		private void refreshPanel()
		{
			Transform transform = this.m_InfoPanel.transform.Find("List");
			DebugHelper.Assert(transform != null);
			CUIListScript cUIListScript = (transform != null) ? transform.GetComponent<CUIListScript>() : null;
			DebugHelper.Assert(cUIListScript != null);
			if (cUIListScript != null)
			{
				DebugHelper.Assert(this.m_stInfos != null);
				cUIListScript.SetElementAmount(this.m_stInfos.get_Count());
				bool flag = cUIListScript.IsElementInScrollArea(cUIListScript.m_elementAmount - 1);
				for (int i = 0; i < this.m_stInfos.get_Count(); i++)
				{
					Text component = cUIListScript.GetElemenet(i).transform.Find("Describe").GetComponent<Text>();
					component.set_text(this.GetDescribeStr(this.m_stInfos.get_Item(i)));
				}
				if (flag)
				{
					cUIListScript.MoveElementInScrollArea(cUIListScript.m_elementAmount - 1, false);
				}
			}
		}

		private void calcComsume(int BuyTime)
		{
			ResCoinBuyInfo dataByKey = GameDataMgr.coninBuyDatabin.GetDataByKey((uint)((ushort)BuyTime));
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

		private uint calcGainCoin(uint coinBase)
		{
			uint dwConfValue = GameDataMgr.globalInfoDatabin.GetDataByKey(28u).dwConfValue;
			return coinBase + Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().Level * dwConfValue;
		}

		public void BuyCoinRsp(CSPkg msg)
		{
			for (int i = 0; i < (int)msg.stPkgData.stCoinBuyRsp.stBuyList.bCoinGetCnt; i++)
			{
				BuyCoinInfo info = default(BuyCoinInfo);
				this.calcComsume((int)msg.stPkgData.stCoinBuyRsp.wBuyStartFreq + i);
				info.m_CostDiamond = this.m_CostDiamond;
				info.m_GainCoin = (int)msg.stPkgData.stCoinBuyRsp.stBuyList.CoinGetVal[i];
				info.m_CritTime = (int)(msg.stPkgData.stCoinBuyRsp.stBuyList.CoinGetVal[i] / (uint)((this.m_GainCoin == 0) ? 1 : this.m_GainCoin));
				this.addInfo(info);
			}
		}
	}
}
