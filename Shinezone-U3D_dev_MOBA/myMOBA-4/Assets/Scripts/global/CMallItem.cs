using Assets.Scripts.Framework;
using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;

public class CMallItem
{
	public enum ItemType
	{
		Hero,
		Skin,
		Item
	}

	public enum IconType
	{
		Normal,
		Small
	}

	public enum PayBy
	{
		None,
		OnlyOne,
		Both
	}

	public enum OldPriceType
	{
		None,
		FirstOne,
		SecondOne,
		Both
	}

	private CMallItem.ItemType m_type;

	private CMallItem.IconType m_iconType;

	private IHeroData m_heroData;

	private CMallFactoryShopController.ShopProduct m_product;

	private CUseable m_useable;

	private ResHeroSkin m_skinData;

	private string m_firstName;

	private string m_secondName;

	private string m_iconPath;

	private stPayInfoSet m_payInfoSet;

	public CMallItem(uint heroID, CMallItem.IconType iconType = CMallItem.IconType.Normal)
	{
		this.m_type = CMallItem.ItemType.Hero;
		this.m_iconType = iconType;
		this.m_heroData = CHeroDataFactory.CreateHeroData(heroID);
		if (this.m_heroData != null)
		{
			this.m_firstName = this.m_heroData.heroName;
			this.m_secondName = null;
			string s_Sprite_Dynamic_Icon_Dir = CUIUtility.s_Sprite_Dynamic_Icon_Dir;
			if (iconType == CMallItem.IconType.Small)
			{
				this.m_useable = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_HERO, heroID, 1);
				if (this.m_useable != null)
				{
					this.m_iconPath = this.m_useable.GetIconPath();
				}
				else
				{
					this.m_iconPath = null;
				}
			}
			else if (this.m_heroData.heroCfgInfo != null)
			{
				this.m_iconPath = CUIUtility.s_Sprite_Dynamic_BustHero_Dir + this.m_heroData.heroCfgInfo.szImagePath;
			}
			else
			{
				this.m_iconPath = null;
			}
			ResHeroPromotion resPromotion = this.m_heroData.promotion();
			this.m_payInfoSet = CMallSystem.GetPayInfoSetOfGood(this.m_heroData.heroCfgInfo, resPromotion);
		}
		else
		{
			this.m_useable = null;
			this.m_firstName = null;
			this.m_secondName = null;
			this.m_iconPath = null;
			this.m_payInfoSet = default(stPayInfoSet);
		}
	}

	public CMallItem(uint heroID, uint skinID, CMallItem.IconType iconType = CMallItem.IconType.Normal)
	{
		this.m_type = CMallItem.ItemType.Skin;
		this.m_iconType = iconType;
		this.m_heroData = CHeroDataFactory.CreateHeroData(heroID);
		this.m_skinData = CSkinInfo.GetHeroSkin(heroID, skinID);
		if (this.m_heroData != null && this.m_skinData != null)
		{
			this.m_firstName = this.m_heroData.heroName;
			this.m_secondName = this.m_skinData.szSkinName;
			if (iconType == CMallItem.IconType.Small)
			{
				this.m_useable = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN, this.m_skinData.dwID, 1);
				if (this.m_useable != null)
				{
					this.m_iconPath = this.m_useable.GetIconPath();
				}
				else
				{
					this.m_iconPath = null;
				}
			}
			else
			{
				this.m_iconPath = CUIUtility.s_Sprite_Dynamic_BustHero_Dir + this.m_skinData.szSkinPicID;
			}
			ResSkinPromotion skinPromotion = CSkinInfo.GetSkinPromotion(heroID, skinID);
			this.m_payInfoSet = CMallSystem.GetPayInfoSetOfGood(this.m_skinData, skinPromotion);
		}
		else
		{
			this.m_useable = null;
			this.m_firstName = null;
			this.m_secondName = null;
			this.m_iconPath = null;
			this.m_payInfoSet = default(stPayInfoSet);
		}
	}

	public CMallItem(CMallFactoryShopController.ShopProduct product, CMallItem.IconType iconType = CMallItem.IconType.Small)
	{
		this.m_type = CMallItem.ItemType.Item;
		this.m_iconType = iconType;
		if (product != null)
		{
			this.m_useable = CUseableManager.CreateUseable(product.Type, 0uL, product.ID, (int)product.LimitCount, 0);
			this.m_product = product;
			this.m_firstName = this.m_useable.m_name;
			this.m_secondName = null;
			this.m_iconPath = this.m_useable.GetIconPath();
			RES_SHOPBUY_COINTYPE coinType = product.CoinType;
			enPayType payType = CMallSystem.ResBuyTypeToPayType((int)coinType);
			uint buyPrice = this.m_useable.GetBuyPrice(coinType);
			uint payValue = product.ConvertWithRealDiscount(buyPrice);
			this.m_payInfoSet = new stPayInfoSet(1);
			this.m_payInfoSet.m_payInfoCount = 1;
			this.m_payInfoSet.m_payInfos[0].m_discountForDisplay = product.DiscountForShow;
			this.m_payInfoSet.m_payInfos[0].m_oriValue = buyPrice;
			this.m_payInfoSet.m_payInfos[0].m_payType = payType;
			this.m_payInfoSet.m_payInfos[0].m_payValue = payValue;
		}
		else
		{
			this.m_useable = null;
			this.m_firstName = null;
			this.m_secondName = null;
			this.m_useable = null;
			this.m_iconPath = null;
			this.m_payInfoSet = default(stPayInfoSet);
		}
	}

	public string Icon()
	{
		return this.m_iconPath;
	}

	public CMallItem.ItemType Type()
	{
		return this.m_type;
	}

	public CMallItem.IconType GetIconType()
	{
		return this.m_iconType;
	}

	public int Grade()
	{
		if (this.m_iconType == CMallItem.IconType.Normal)
		{
			return -1;
		}
		if (this.m_useable != null)
		{
			return (int)(this.m_useable.m_grade + 1);
		}
		return -1;
	}

	public enHeroJobType Job()
	{
		if (this.m_heroData != null && this.m_type == CMallItem.ItemType.Hero)
		{
			return (enHeroJobType)this.m_heroData.heroCfgInfo.bMainJob;
		}
		return enHeroJobType.All;
	}

	public bool Owned(bool isIncludeValidExperience = false)
	{
		CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
		DebugHelper.Assert(masterRoleInfo != null, "Owned::Master Role Info Is Null");
		if (masterRoleInfo == null)
		{
			return false;
		}
		CMallItem.ItemType type = this.m_type;
		if (type != CMallItem.ItemType.Hero)
		{
			return type == CMallItem.ItemType.Skin && this.m_skinData != null && masterRoleInfo.IsHaveHeroSkin(this.m_skinData.dwHeroID, this.m_skinData.dwSkinID, isIncludeValidExperience);
		}
		return this.m_heroData != null && masterRoleInfo.IsHaveHero(this.m_heroData.cfgID, isIncludeValidExperience);
	}

	public bool CanBeAskFor()
	{
		CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
		DebugHelper.Assert(masterRoleInfo != null, "Owned::Master Role Info Is Null");
		if (masterRoleInfo == null)
		{
			return false;
		}
		if (this.Owned(false))
		{
			return false;
		}
		CMallItem.ItemType type = this.m_type;
		if (type != CMallItem.ItemType.Hero)
		{
			return type == CMallItem.ItemType.Skin && this.m_skinData != null && GameDataMgr.IsSkinCanBeAskFor(this.m_skinData.dwID);
		}
		return this.m_heroData != null && GameDataMgr.IsHeroCanBeAskFor(this.m_heroData.cfgID);
	}

	public bool CanSendFriend()
	{
		CMallItem.ItemType type = this.m_type;
		if (type != CMallItem.ItemType.Hero)
		{
			return type == CMallItem.ItemType.Skin && this.m_skinData != null && CHeroSkinBuyManager.ShouldShowBuyForFriend(true, this.m_skinData.dwHeroID, this.m_skinData.dwSkinID, false);
		}
		return this.m_heroData != null && CHeroSkinBuyManager.ShouldShowBuyForFriend(false, this.m_heroData.cfgID, 0u, false);
	}

	public uint HeroID()
	{
		if (this.m_heroData != null)
		{
			return this.m_heroData.cfgID;
		}
		return 166u;
	}

	public uint SkinID()
	{
		if (this.m_skinData != null)
		{
			return this.m_skinData.dwSkinID;
		}
		return 0u;
	}

	public uint SkinUniqID()
	{
		if (this.m_skinData != null)
		{
			return this.m_skinData.dwID;
		}
		return 0u;
	}

	public int ProductIdx()
	{
		if (this.m_product == null)
		{
			return -1;
		}
		return Singleton<CMallFactoryShopController>.GetInstance().GetProductIndex(this.m_product);
	}

	public string FirstName()
	{
		return this.m_firstName;
	}

	public string SecondName()
	{
		return this.m_secondName;
	}

	public stPayInfoSet PayInfoSet()
	{
		return this.m_payInfoSet;
	}

	public bool IsValidExperience()
	{
		CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
		DebugHelper.Assert(masterRoleInfo != null, "IsValidExperience::Master Role Info Is Null");
		if (masterRoleInfo == null)
		{
			return false;
		}
		CMallItem.ItemType type = this.m_type;
		if (type != CMallItem.ItemType.Hero)
		{
			return type == CMallItem.ItemType.Skin && this.m_skinData != null && masterRoleInfo.IsValidExperienceSkin(this.m_skinData.dwHeroID, this.m_skinData.dwSkinID);
		}
		return this.m_heroData != null && masterRoleInfo.IsValidExperienceHero(this.m_heroData.cfgID);
	}

	public CMallItem.OldPriceType GetOldPriceType()
	{
		switch (this.m_payInfoSet.m_payInfoCount)
		{
		case 0:
			return CMallItem.OldPriceType.None;
		case 1:
			if (this.m_payInfoSet.m_payInfos[0].m_oriValue != this.m_payInfoSet.m_payInfos[0].m_payValue)
			{
				return CMallItem.OldPriceType.FirstOne;
			}
			return CMallItem.OldPriceType.None;
		case 2:
			if (this.m_payInfoSet.m_payInfos[0].m_oriValue != this.m_payInfoSet.m_payInfos[0].m_payValue && this.m_payInfoSet.m_payInfos[1].m_oriValue != this.m_payInfoSet.m_payInfos[1].m_payValue)
			{
				return CMallItem.OldPriceType.Both;
			}
			if (this.m_payInfoSet.m_payInfos[0].m_oriValue != this.m_payInfoSet.m_payInfos[0].m_payValue)
			{
				return CMallItem.OldPriceType.FirstOne;
			}
			if (this.m_payInfoSet.m_payInfos[1].m_oriValue != this.m_payInfoSet.m_payInfos[1].m_payValue)
			{
				return CMallItem.OldPriceType.SecondOne;
			}
			return CMallItem.OldPriceType.None;
		default:
			return CMallItem.OldPriceType.None;
		}
	}

	public RES_LUCKYDRAW_ITEMTAG TagType(ref ResHeroPromotion heroPromotion, ref ResSkinPromotion skinPromotion)
	{
		CMallItem.ItemType type = this.m_type;
		if (type != CMallItem.ItemType.Hero)
		{
			if (type == CMallItem.ItemType.Skin)
			{
				if (this.m_skinData != null)
				{
					skinPromotion = CSkinInfo.GetSkinPromotion(this.m_skinData.dwID);
					if (skinPromotion == null)
					{
						return RES_LUCKYDRAW_ITEMTAG.RES_LUCKYDRAW_ITEMTAG_NONE;
					}
					return (RES_LUCKYDRAW_ITEMTAG)skinPromotion.bTag;
				}
			}
		}
		else if (this.m_heroData != null)
		{
			heroPromotion = this.m_heroData.promotion();
			if (heroPromotion == null)
			{
				return RES_LUCKYDRAW_ITEMTAG.RES_LUCKYDRAW_ITEMTAG_NONE;
			}
			return (RES_LUCKYDRAW_ITEMTAG)heroPromotion.bTag;
		}
		return RES_LUCKYDRAW_ITEMTAG.RES_LUCKYDRAW_ITEMTAG_NONE;
	}

	public bool TagInfo(ref string iconPath, ref string text, bool owned = false)
	{
		CTextManager instance = Singleton<CTextManager>.GetInstance();
		if (owned)
		{
			iconPath = "UGUI/Sprite/Common/Product_New.prefab";
			text = instance.GetText("Mall_Hero_State_Own");
			return true;
		}
		if (this.Owned(false))
		{
			iconPath = null;
			text = null;
			return false;
		}
		ResHeroPromotion resHeroPromotion = null;
		ResSkinPromotion resSkinPromotion = null;
		RES_LUCKYDRAW_ITEMTAG rES_LUCKYDRAW_ITEMTAG = this.TagType(ref resHeroPromotion, ref resSkinPromotion);
		string text2 = null;
		if (this.m_type == CMallItem.ItemType.Hero)
		{
			ResHeroShop heroShop = null;
			GameDataMgr.heroShopInfoDict.TryGetValue(this.m_heroData.heroCfgInfo.dwCfgID, out heroShop);
			text2 = CMallSystem.GetRegisterSalesHeroDay(ref resHeroPromotion, heroShop);
		}
		else if (this.m_type == CMallItem.ItemType.Skin)
		{
			ResHeroSkinShop heroShop2 = null;
			GameDataMgr.skinShopInfoDict.TryGetValue(this.m_skinData.dwID, out heroShop2);
			text2 = CMallSystem.GetRegisterSalesSkinDay(ref resSkinPromotion, heroShop2);
		}
		if (text2 != null)
		{
			iconPath = "UGUI/Sprite/Common/Product_Unusual.prefab";
			text = text2;
			return true;
		}
		switch (rES_LUCKYDRAW_ITEMTAG)
		{
		case RES_LUCKYDRAW_ITEMTAG.RES_LUCKYDRAW_ITEMTAG_NONE:
			if (this.m_type == CMallItem.ItemType.Hero && this.m_heroData.heroCfgInfo.bTag == 1)
			{
				CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
				if (masterRoleInfo == null)
				{
					iconPath = null;
					text = null;
					return false;
				}
				if (GameDataMgr.svr2CltCfgDict.ContainsKey(24u))
				{
					ResGlobalInfo resGlobalInfo = new ResGlobalInfo();
					if (GameDataMgr.svr2CltCfgDict.TryGetValue(24u, out resGlobalInfo))
					{
						uint dwConfValue = resGlobalInfo.dwConfValue;
						if (masterRoleInfo.PvpLevel < dwConfValue)
						{
							iconPath = "UGUI/Sprite/Common/Hero_Newbie.prefab";
							text = Singleton<CTextManager>.GetInstance().GetText("Hero_Tag_Newbie");
							return true;
						}
					}
				}
			}
			iconPath = null;
			text = null;
			return false;
		case RES_LUCKYDRAW_ITEMTAG.RES_LUCKYDRAW_ITEMTAG_UNUSUAL:
		{
			int num = 0;
			uint currentUTCTime = (uint)CRoleInfo.GetCurrentUTCTime();
			CMallItem.ItemType type = this.m_type;
			if (type != CMallItem.ItemType.Hero)
			{
				if (type == CMallItem.ItemType.Skin)
				{
					if (resSkinPromotion == null)
					{
						iconPath = null;
						text = null;
						return false;
					}
					if (resSkinPromotion.dwOnTimeGen > currentUTCTime)
					{
						num = (int)(resSkinPromotion.dwOffTimeGen - resSkinPromotion.dwOnTimeGen);
					}
					else
					{
						num = (int)(resSkinPromotion.dwOffTimeGen - currentUTCTime);
					}
				}
			}
			else
			{
				if (resHeroPromotion == null)
				{
					iconPath = null;
					text = null;
					return false;
				}
				if (resHeroPromotion.dwOnTimeGen > currentUTCTime)
				{
					num = (int)(resHeroPromotion.dwOffTimeGen - resHeroPromotion.dwOnTimeGen);
				}
				else
				{
					num = (int)(resHeroPromotion.dwOffTimeGen - currentUTCTime);
				}
			}
			if (num <= 0)
			{
				iconPath = null;
				text = null;
				return false;
			}
			int num2 = (int)Math.Ceiling((double)num / 86400.0);
			if (num2 > 0)
			{
				iconPath = "UGUI/Sprite/Common/Product_Unusual.prefab";
				text = Singleton<CTextManager>.GetInstance().GetText("Mall_Promotion_Tag_1", new string[]
				{
					num2.ToString()
				});
				return true;
			}
			iconPath = null;
			text = null;
			return false;
		}
		case RES_LUCKYDRAW_ITEMTAG.RES_LUCKYDRAW_ITEMTAG_NEW:
			iconPath = "UGUI/Sprite/Common/Product_New.prefab";
			text = Singleton<CTextManager>.GetInstance().GetText("Common_Tag_New");
			return true;
		case RES_LUCKYDRAW_ITEMTAG.RES_LUCKYDRAW_ITEMTAG_HOT:
			iconPath = "UGUI/Sprite/Common/Product_Hot.prefab";
			text = Singleton<CTextManager>.GetInstance().GetText("Common_Tag_Hot");
			return true;
		case RES_LUCKYDRAW_ITEMTAG.RES_LUCKYDRAW_ITEMTAG_DISCOUNT:
		{
			float num3 = 100f;
			switch (this.m_type)
			{
			case CMallItem.ItemType.Hero:
				if (resHeroPromotion == null)
				{
					iconPath = null;
					text = null;
					return false;
				}
				num3 = resHeroPromotion.dwDiscount / 10f;
				break;
			case CMallItem.ItemType.Skin:
				if (resSkinPromotion == null)
				{
					iconPath = null;
					text = null;
					return false;
				}
				num3 = resSkinPromotion.dwDiscount / 10f;
				break;
			}
			iconPath = "UGUI/Sprite/Common/Product_Discount.prefab";
			if (Math.Abs(num3 % 1f) < 1.401298E-45f)
			{
				text = string.Format("{0}折", ((int)num3).ToString("D"));
			}
			else
			{
				text = string.Format("{0}折", num3.ToString("0.0"));
			}
			return true;
		}
		default:
			iconPath = null;
			text = null;
			return false;
		}
	}

	public byte ObtWayType()
	{
		CMallItem.ItemType type = this.m_type;
		if (type != CMallItem.ItemType.Hero)
		{
			if (type != CMallItem.ItemType.Skin)
			{
				return 0;
			}
			if (this.m_skinData == null)
			{
				return 0;
			}
			ResHeroSkinShop resHeroSkinShop = null;
			GameDataMgr.skinShopInfoDict.TryGetValue(this.m_skinData.dwID, out resHeroSkinShop);
            return (resHeroSkinShop == null) ? (byte)0 : resHeroSkinShop.bGetPathType;
		}
		else
		{
			if (this.m_heroData == null)
			{
				return 0;
			}
			ResHeroShop resHeroShop = null;
			GameDataMgr.heroShopInfoDict.TryGetValue(this.m_heroData.heroCfgInfo.dwCfgID, out resHeroShop);
            return (resHeroShop == null) ? (byte)0 : resHeroShop.bObtWayType;
		}
	}

	public string ObtWay()
	{
		CMallItem.ItemType type = this.m_type;
		if (type != CMallItem.ItemType.Hero)
		{
			if (type != CMallItem.ItemType.Skin)
			{
				return null;
			}
			if (this.m_skinData == null)
			{
				return null;
			}
			ResHeroSkinShop resHeroSkinShop = null;
			GameDataMgr.skinShopInfoDict.TryGetValue(this.m_skinData.dwID, out resHeroSkinShop);
			return (resHeroSkinShop == null) ? null : resHeroSkinShop.szGetPath;
		}
		else
		{
			if (this.m_heroData == null)
			{
				return null;
			}
			ResHeroShop resHeroShop = null;
			GameDataMgr.heroShopInfoDict.TryGetValue(this.m_heroData.heroCfgInfo.dwCfgID, out resHeroShop);
			return (resHeroShop == null) ? null : resHeroShop.szObtWay;
		}
	}

	public CMallItem.PayBy PayWay()
	{
		switch (this.m_payInfoSet.m_payInfoCount)
		{
		case 0:
			return CMallItem.PayBy.None;
		case 1:
			return CMallItem.PayBy.OnlyOne;
		case 2:
			return CMallItem.PayBy.Both;
		default:
			return CMallItem.PayBy.None;
		}
	}
}
