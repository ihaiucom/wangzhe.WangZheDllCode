using Assets.Scripts.Framework;
using ResData;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Assets.Scripts.GameSystem
{
	public class HeroSortImp : Singleton<HeroSortImp>, IMallSort<CMallSortHelper.HeroSortType>, IComparer<ResHeroCfgInfo>
	{
		private CMallSortHelper.HeroSortType m_sortType;

		private bool m_desc;

		private CRoleInfo m_roleInfo;

		private CultureInfo m_culture;

		public override void Init()
		{
			base.Init();
			this.m_sortType = CMallSortHelper.HeroSortType.Default;
			this.m_desc = false;
			this.m_roleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			this.m_culture = new CultureInfo("zh-CN");
		}

		public override void UnInit()
		{
			base.UnInit();
			this.m_sortType = CMallSortHelper.HeroSortType.Default;
			this.m_desc = false;
			this.m_roleInfo = null;
			this.m_culture = null;
		}

		public string GetSortTypeName(CMallSortHelper.HeroSortType sortType = CMallSortHelper.HeroSortType.Default)
		{
			if (sortType < CMallSortHelper.HeroSortType.Default || sortType > (CMallSortHelper.HeroSortType)CMallSortHelper.heroSortTypeNameKeys.Length)
			{
				return null;
			}
			return Singleton<CTextManager>.GetInstance().GetText(CMallSortHelper.heroSortTypeNameKeys[(int)sortType]);
		}

		public CMallSortHelper.HeroSortType GetCurSortType()
		{
			return this.m_sortType;
		}

		public bool IsDesc()
		{
			return this.m_desc;
		}

		public void SetSortType(CMallSortHelper.HeroSortType sortType = CMallSortHelper.HeroSortType.Default)
		{
			if (sortType != CMallSortHelper.HeroSortType.Default)
			{
				if (this.m_sortType != sortType)
				{
					this.m_desc = false;
				}
				else
				{
					this.m_desc = !this.m_desc;
				}
			}
			else
			{
				this.m_desc = false;
			}
			this.m_sortType = sortType;
		}

		private int CompareDefault(ResHeroCfgInfo l, ResHeroCfgInfo r)
		{
			bool flag = this.m_roleInfo.IsHaveHero(l.dwCfgID, false);
			bool flag2 = this.m_roleInfo.IsHaveHero(r.dwCfgID, false);
			if (flag && !flag2)
			{
				return 1;
			}
			if (!flag && flag2)
			{
				return -1;
			}
			ResHeroPromotion resHeroPromotion = CHeroDataFactory.CreateHeroData(l.dwCfgID).promotion();
			ResHeroPromotion resHeroPromotion2 = CHeroDataFactory.CreateHeroData(r.dwCfgID).promotion();
			ResHeroShop resHeroShop = null;
			ResHeroShop resHeroShop2 = null;
			GameDataMgr.heroShopInfoDict.TryGetValue(l.dwCfgID, out resHeroShop);
			GameDataMgr.heroShopInfoDict.TryGetValue(r.dwCfgID, out resHeroShop2);
			uint num = (resHeroShop != null) ? resHeroShop.dwSortId : 4294967295u;
			uint num2 = (resHeroShop2 != null) ? resHeroShop2.dwSortId : 4294967295u;
			if (resHeroPromotion != null)
			{
				num = resHeroPromotion.dwSortIndex;
			}
			if (resHeroPromotion2 != null)
			{
				num2 = resHeroPromotion2.dwSortIndex;
			}
			if (resHeroShop != null)
			{
				ResDT_RegisterSale_Info stRegisterSale = resHeroShop.stRegisterSale;
				bool flag3 = CMallSystem.IsinRegisterSales(stRegisterSale);
				if (flag3)
				{
					num = stRegisterSale.dwSortID;
				}
			}
			if (resHeroShop2 != null)
			{
				ResDT_RegisterSale_Info stRegisterSale2 = resHeroShop2.stRegisterSale;
				bool flag4 = CMallSystem.IsinRegisterSales(stRegisterSale2);
				if (flag4)
				{
					num2 = stRegisterSale2.dwSortID;
				}
			}
			if (num < num2)
			{
				return 1;
			}
			if (num > num2)
			{
				return -1;
			}
			if (l.dwCfgID < r.dwCfgID)
			{
				return -1;
			}
			if (l.dwCfgID > r.dwCfgID)
			{
				return 1;
			}
			return 0;
		}

		private int CompareCoin(ResHeroCfgInfo l, ResHeroCfgInfo r)
		{
			ResHeroPromotion resPromotion = CHeroDataFactory.CreateHeroData(l.dwCfgID).promotion();
			ResHeroPromotion resPromotion2 = CHeroDataFactory.CreateHeroData(r.dwCfgID).promotion();
			ResHeroShop resHeroShop = null;
			ResHeroShop resHeroShop2 = null;
			GameDataMgr.heroShopInfoDict.TryGetValue(l.dwCfgID, out resHeroShop);
			GameDataMgr.heroShopInfoDict.TryGetValue(r.dwCfgID, out resHeroShop2);
			stPayInfoSet payInfoSetOfGood = CMallSystem.GetPayInfoSetOfGood(l, resPromotion);
			stPayInfoSet payInfoSetOfGood2 = CMallSystem.GetPayInfoSetOfGood(r, resPromotion2);
			uint num = 4294967295u;
			uint num2 = 4294967295u;
			for (int i = 0; i < payInfoSetOfGood.m_payInfoCount; i++)
			{
				if (payInfoSetOfGood.m_payInfos[i].m_payType == enPayType.GoldCoin && payInfoSetOfGood.m_payInfos[i].m_payValue <= num)
				{
					num = payInfoSetOfGood.m_payInfos[i].m_payValue;
				}
			}
			for (int j = 0; j < payInfoSetOfGood2.m_payInfoCount; j++)
			{
				if (payInfoSetOfGood2.m_payInfos[j].m_payType == enPayType.GoldCoin && payInfoSetOfGood2.m_payInfos[j].m_payValue <= num2)
				{
					num2 = payInfoSetOfGood2.m_payInfos[j].m_payValue;
				}
			}
			if (num == 4294967295u && this.IsDesc())
			{
				num = 0u;
			}
			if (num2 == 4294967295u && this.IsDesc())
			{
				num2 = 0u;
			}
			return num.CompareTo(num2);
		}

		private int CompareCoupons(ResHeroCfgInfo l, ResHeroCfgInfo r)
		{
			ResHeroPromotion resPromotion = CHeroDataFactory.CreateHeroData(l.dwCfgID).promotion();
			ResHeroPromotion resPromotion2 = CHeroDataFactory.CreateHeroData(r.dwCfgID).promotion();
			stPayInfoSet payInfoSetOfGood = CMallSystem.GetPayInfoSetOfGood(l, resPromotion);
			stPayInfoSet payInfoSetOfGood2 = CMallSystem.GetPayInfoSetOfGood(r, resPromotion2);
			uint num = 4294967295u;
			uint num2 = 4294967295u;
			for (int i = 0; i < payInfoSetOfGood.m_payInfoCount; i++)
			{
				if ((payInfoSetOfGood.m_payInfos[i].m_payType == enPayType.Diamond || payInfoSetOfGood.m_payInfos[i].m_payType == enPayType.DianQuan || payInfoSetOfGood.m_payInfos[i].m_payType == enPayType.DiamondAndDianQuan) && payInfoSetOfGood.m_payInfos[i].m_payValue < num)
				{
					num = payInfoSetOfGood.m_payInfos[i].m_payValue;
				}
			}
			for (int j = 0; j < payInfoSetOfGood2.m_payInfoCount; j++)
			{
				if ((payInfoSetOfGood2.m_payInfos[j].m_payType == enPayType.Diamond || payInfoSetOfGood2.m_payInfos[j].m_payType == enPayType.DianQuan || payInfoSetOfGood2.m_payInfos[j].m_payType == enPayType.DiamondAndDianQuan) && payInfoSetOfGood2.m_payInfos[j].m_payValue < num2)
				{
					num2 = payInfoSetOfGood2.m_payInfos[j].m_payValue;
				}
			}
			if (num == 4294967295u && this.IsDesc())
			{
				num = 0u;
			}
			if (num2 == 4294967295u && this.IsDesc())
			{
				num2 = 0u;
			}
			return num.CompareTo(num2);
		}

		private int CompareReleaseTime(ResHeroCfgInfo l, ResHeroCfgInfo r)
		{
			ResHeroShop resHeroShop = null;
			ResHeroShop resHeroShop2 = null;
			GameDataMgr.heroShopInfoDict.TryGetValue(l.dwCfgID, out resHeroShop);
			GameDataMgr.heroShopInfoDict.TryGetValue(r.dwCfgID, out resHeroShop2);
			if (resHeroShop == null)
			{
				return 1;
			}
			if (resHeroShop2 == null)
			{
				return -1;
			}
			return resHeroShop.dwReleaseId.CompareTo(resHeroShop2.dwReleaseId);
		}

		private int CompareName(ResHeroCfgInfo l, ResHeroCfgInfo r)
		{
			return string.Compare(l.szName, r.szName, this.m_culture, 0);
		}

		public int Compare(ResHeroCfgInfo l, ResHeroCfgInfo r)
		{
			if (l == null)
			{
				return 1;
			}
			if (r == null)
			{
				return -1;
			}
			if (this.m_roleInfo == null)
			{
				return -1;
			}
			switch (this.m_sortType)
			{
			case CMallSortHelper.HeroSortType.Name:
				return this.CompareName(l, r);
			case CMallSortHelper.HeroSortType.Coupons:
				return this.CompareCoupons(l, r);
			case CMallSortHelper.HeroSortType.Coin:
				return this.CompareCoin(l, r);
			case CMallSortHelper.HeroSortType.ReleaseTime:
				return this.CompareReleaseTime(l, r);
			default:
				return this.CompareDefault(l, r);
			}
		}
	}
}
