using Assets.Scripts.Framework;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using UnityEngine;

namespace Assets.Scripts.GameSystem
{
	public class CEquip : CUseable
	{
		public ResEquipInfo m_equipData;

		public override COM_REWARDS_TYPE MapRewardType
		{
			get
			{
				return COM_REWARDS_TYPE.COM_REWARDS_TYPE_EQUIP;
			}
		}

		public CEquip(ulong objID, uint baseID, int stackCount = 0, int addTime = 0)
		{
			this.m_equipData = GameDataMgr.equipInfoDatabin.GetDataByKey(baseID);
			if (this.m_equipData == null)
			{
				Debug.Log("not equip id" + baseID);
				return;
			}
			this.m_type = COM_ITEM_TYPE.COM_OBJTYPE_ITEMEQUIP;
			this.m_objID = objID;
			this.m_baseID = baseID;
			this.m_name = StringHelper.UTF8BytesToString(ref this.m_equipData.szName);
			this.m_description = StringHelper.UTF8BytesToString(ref this.m_equipData.szDesc);
			this.m_iconID = this.m_equipData.dwIcon;
			this.m_stackCount = stackCount;
			this.m_stackMax = this.m_equipData.iOverLimit;
			this.m_goldCoinBuy = this.m_equipData.dwPVPCoinBuy;
			this.m_dianQuanBuy = this.m_equipData.dwCouponsBuy;
			this.m_diamondBuy = 0u;
			this.m_arenaCoinBuy = this.m_equipData.dwArenaCoinBuy;
			this.m_burningCoinBuy = this.m_equipData.dwBurningCoinBuy;
			this.m_guildCoinBuy = this.m_equipData.dwGuildCoinBuy;
			this.m_dianQuanDirectBuy = 0u;
			this.m_coinSale = this.m_equipData.dwCoinSale;
			this.m_grade = this.m_equipData.bGrade;
			this.m_isSale = this.m_equipData.bIsSale;
			this.m_addTime = addTime;
			base.ResetTime();
		}

		public override string GetIconPath()
		{
			return CUIUtility.s_Sprite_Dynamic_Icon_Dir + this.m_iconID;
		}
	}
}
