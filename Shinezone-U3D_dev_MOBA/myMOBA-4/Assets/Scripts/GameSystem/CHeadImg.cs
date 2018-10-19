using Assets.Scripts.Framework;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using UnityEngine;

namespace Assets.Scripts.GameSystem
{
	public class CHeadImg : CUseable
	{
		public ResHeadImage m_headImgData;

		public CHeadImg(ulong objID, uint baseID, int addTime = 0)
		{
			GameDataMgr.headImageDict.TryGetValue(baseID, out this.m_headImgData);
			if (this.m_headImgData == null)
			{
				Debug.Log("not HeadImg id" + baseID);
				return;
			}
			this.m_type = COM_ITEM_TYPE.COM_OBJTYPE_HEADIMG;
			this.m_objID = objID;
			this.m_baseID = baseID;
			this.m_name = StringHelper.UTF8BytesToString(ref this.m_headImgData.szHeadDesc);
			this.m_description = StringHelper.UTF8BytesToString(ref this.m_headImgData.szHeadDesc);
			this.m_goldCoinBuy = this.m_headImgData.dwPVPCoinBuy;
			this.m_dianQuanBuy = this.m_headImgData.dwCouponsBuy;
			this.m_diamondBuy = this.m_headImgData.dwDiamondBuy;
			this.m_guildCoinBuy = this.m_headImgData.dwGuildCoinBuy;
			this.m_dianQuanDirectBuy = 0u;
			this.m_stackMax = 1;
			this.m_addTime = addTime;
			base.ResetTime();
		}

		public override string GetIconPath()
		{
			return string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Nobe_Dir, this.m_headImgData.szHeadIcon);
		}
	}
}
