using System;

namespace Assets.Scripts.GameSystem
{
	public class CMallSortHelper
	{
		public enum HeroSortType
		{
			Default,
			Name,
			Coupons,
			Coin,
			ReleaseTime,
			TypeCount
		}

		public enum SkinSortType
		{
			Default,
			Name,
			Coupons,
			ReleaseTime,
			Quality,
			TypeCount
		}

		public enum HeroViewSortType
		{
			Default,
			Name,
			Proficiency,
			ReleaseTime,
			TypeCount
		}

		public static string[] heroSortTypeNameKeys = new string[]
		{
			"Mall_Hero_Sort_Type_Default",
			"Mall_Hero_Sort_Type_Name",
			"Mall_Hero_Sort_Type_Coupons",
			"Mall_Hero_Sort_Type_Coin",
			"Mall_Hero_Sort_Type_ReleaseTime"
		};

		public static string[] skinSortTypeNameKeys = new string[]
		{
			"Mall_Skin_Sort_Type_Default",
			"Mall_Skin_Sort_Type_Name",
			"Mall_Skin_Sort_Type_Coupons",
			"Mall_Skin_Sort_Type_ReleaseTime",
			"Mall_Skin_Sort_Type_Quality"
		};

		public static string[] heroViewSortTypeNameKeys = new string[]
		{
			"Hero_View_Sort_Type_Default",
			"Hero_View_Sort_Type_Name",
			"Hero_View_Sort_Type_Proficiency",
			"Hero_View_Sort_Type_ReleaseTime"
		};

		public static HeroSortImp CreateHeroSorter()
		{
			return Singleton<HeroSortImp>.GetInstance();
		}

		public static HeroViewSortImp CreateHeroViewSorter()
		{
			return Singleton<HeroViewSortImp>.GetInstance();
		}

		public static SkinSortImp CreateSkinSorter()
		{
			return Singleton<SkinSortImp>.GetInstance();
		}
	}
}
