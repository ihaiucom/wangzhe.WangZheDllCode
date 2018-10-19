using Assets.Scripts.Framework;
using ResData;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Assets.Scripts.GameSystem
{
	public class SkinSortImp : Singleton<SkinSortImp>, IMallSort<CMallSortHelper.SkinSortType>, IComparer<ResHeroSkin>
	{
		private CMallSortHelper.SkinSortType m_sortType;

		private bool m_desc;

		private CRoleInfo m_roleInfo;

		private CultureInfo m_culture;

		public override void Init()
		{
			base.Init();
			this.m_sortType = CMallSortHelper.SkinSortType.Default;
			this.m_desc = false;
			this.m_roleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			this.m_culture = new CultureInfo("zh-CN");
		}

		public override void UnInit()
		{
			base.UnInit();
			this.m_roleInfo = null;
			this.m_culture = null;
		}

		public string GetSortTypeName(CMallSortHelper.SkinSortType sortType = CMallSortHelper.SkinSortType.Default)
		{
			if (sortType < CMallSortHelper.SkinSortType.Default || sortType > (CMallSortHelper.SkinSortType)CMallSortHelper.skinSortTypeNameKeys.Length)
			{
				return null;
			}
			return Singleton<CTextManager>.GetInstance().GetText(CMallSortHelper.skinSortTypeNameKeys[(int)sortType]);
		}

		public CMallSortHelper.SkinSortType GetCurSortType()
		{
			return this.m_sortType;
		}

		public bool IsDesc()
		{
			return this.m_desc;
		}

		public void SetSortType(CMallSortHelper.SkinSortType sortType = CMallSortHelper.SkinSortType.Default)
		{
			if (sortType != CMallSortHelper.SkinSortType.Default)
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

		private int CompareDefault(ResHeroSkin l, ResHeroSkin r)
		{
			int result = 0;
			ResSkinPromotion skinPromotion = CSkinInfo.GetSkinPromotion(l.dwHeroID, l.dwSkinID);
			ResSkinPromotion skinPromotion2 = CSkinInfo.GetSkinPromotion(r.dwHeroID, r.dwSkinID);
			ResHeroSkinShop resHeroSkinShop = null;
			GameDataMgr.skinShopInfoDict.TryGetValue(l.dwID, out resHeroSkinShop);
			ResHeroSkinShop resHeroSkinShop2 = null;
			GameDataMgr.skinShopInfoDict.TryGetValue(r.dwID, out resHeroSkinShop2);
			uint num = (resHeroSkinShop == null) ? 4294967295u : resHeroSkinShop.dwSortId;
			uint num2 = (resHeroSkinShop2 == null) ? 4294967295u : resHeroSkinShop2.dwSortId;
			if (skinPromotion != null)
			{
				num = skinPromotion.dwSortIndex;
			}
			if (skinPromotion2 != null)
			{
				num2 = skinPromotion2.dwSortIndex;
			}
			if (resHeroSkinShop != null)
			{
				ResDT_RegisterSale_Info stRegisterSale = resHeroSkinShop.stRegisterSale;
				bool flag = CMallSystem.IsinRegisterSales(stRegisterSale);
				if (flag)
				{
					num = stRegisterSale.dwSortID;
				}
			}
			if (resHeroSkinShop2 != null)
			{
				ResDT_RegisterSale_Info stRegisterSale2 = resHeroSkinShop2.stRegisterSale;
				bool flag2 = CMallSystem.IsinRegisterSales(stRegisterSale2);
				if (flag2)
				{
					num2 = stRegisterSale2.dwSortID;
				}
			}
			if (num < num2)
			{
				result = 1;
			}
			if (num > num2)
			{
				result = -1;
			}
			bool flag3 = this.m_roleInfo.IsHaveHeroSkin(l.dwHeroID, l.dwSkinID, false);
			bool flag4 = this.m_roleInfo.IsHaveHeroSkin(r.dwHeroID, r.dwSkinID, false);
			if (flag3 && !flag4)
			{
				return 1;
			}
			if (!flag3 && flag4)
			{
				return -1;
			}
			if ((skinPromotion != null && skinPromotion.bSortIndexOnly > 0) || (skinPromotion2 != null && skinPromotion2.bSortIndexOnly > 0))
			{
				return result;
			}
			if (CSkinInfo.IsCanBuy(l.dwHeroID, l.dwSkinID) && !CSkinInfo.IsCanBuy(r.dwHeroID, r.dwSkinID))
			{
				return -1;
			}
			if (!CSkinInfo.IsCanBuy(l.dwHeroID, l.dwSkinID) && CSkinInfo.IsCanBuy(r.dwHeroID, r.dwSkinID))
			{
				return 1;
			}
			if (this.m_roleInfo.IsHaveHero(l.dwHeroID, false) && !this.m_roleInfo.IsHaveHero(r.dwHeroID, false))
			{
				return -1;
			}
			if (!this.m_roleInfo.IsHaveHero(l.dwHeroID, false) && this.m_roleInfo.IsHaveHero(r.dwHeroID, false))
			{
				return 1;
			}
			return result;
		}

		private int CompareCoupons(ResHeroSkin l, ResHeroSkin r)
		{
			ResSkinPromotion skinPromotion = CSkinInfo.GetSkinPromotion(l.dwHeroID, l.dwSkinID);
			ResSkinPromotion skinPromotion2 = CSkinInfo.GetSkinPromotion(r.dwHeroID, r.dwSkinID);
			stPayInfoSet payInfoSetOfGood = CMallSystem.GetPayInfoSetOfGood(l, skinPromotion);
			stPayInfoSet payInfoSetOfGood2 = CMallSystem.GetPayInfoSetOfGood(r, skinPromotion2);
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

		private int CompareReleaseTime(ResHeroSkin l, ResHeroSkin r)
		{
			ResHeroSkinShop resHeroSkinShop = null;
			ResHeroSkinShop resHeroSkinShop2 = null;
			GameDataMgr.skinShopInfoDict.TryGetValue(l.dwID, out resHeroSkinShop);
			GameDataMgr.skinShopInfoDict.TryGetValue(r.dwID, out resHeroSkinShop2);
			if (resHeroSkinShop == null)
			{
				return 1;
			}
			if (resHeroSkinShop2 == null)
			{
				return -1;
			}
			return resHeroSkinShop.dwReleaseId.CompareTo(resHeroSkinShop2.dwReleaseId);
		}

		private int CompareName(ResHeroSkin l, ResHeroSkin r)
		{
			string strA = string.Format("{0}{1}", l.szHeroName, l.szSkinName);
			string strB = string.Format("{0}{1}", r.szHeroName, r.szSkinName);
			return string.Compare(strA, strB, this.m_culture, CompareOptions.None);
		}

		public int CompareQuality(ResHeroSkin l, ResHeroSkin r)
		{
			ResHeroSkinShop resHeroSkinShop = null;
			ResHeroSkinShop resHeroSkinShop2 = null;
			GameDataMgr.skinShopInfoDict.TryGetValue(l.dwID, out resHeroSkinShop);
			GameDataMgr.skinShopInfoDict.TryGetValue(r.dwID, out resHeroSkinShop2);
			if (resHeroSkinShop == null)
			{
				return 1;
			}
			if (resHeroSkinShop2 == null)
			{
				return -1;
			}
			int num = resHeroSkinShop2.bSkinQuality.CompareTo(resHeroSkinShop.bSkinQuality);
			if (num == 0)
			{
				return this.CompareReleaseTime(l, r);
			}
			return num;
		}

		public int Compare(ResHeroSkin l, ResHeroSkin r)
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
			case CMallSortHelper.SkinSortType.Name:
				return this.CompareName(l, r);
			case CMallSortHelper.SkinSortType.Coupons:
				return this.CompareCoupons(l, r);
			case CMallSortHelper.SkinSortType.ReleaseTime:
				return this.CompareReleaseTime(l, r);
			case CMallSortHelper.SkinSortType.Quality:
				return this.CompareQuality(l, r);
			default:
				return this.CompareDefault(l, r);
			}
		}
	}
}
