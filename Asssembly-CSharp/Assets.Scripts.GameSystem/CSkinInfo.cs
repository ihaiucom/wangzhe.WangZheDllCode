using Assets.Scripts.Framework;
using Assets.Scripts.UI;
using ResData;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameSystem
{
	public class CSkinInfo
	{
		private uint m_wearId;

		private static DictionaryView<uint, ListView<ResHeroSkin>> s_heroSkinDic = new DictionaryView<uint, ListView<ResHeroSkin>>();

		public static ListView<ResHeroSkin> s_availableSkin = new ListView<ResHeroSkin>();

		public void Init(ushort skinId)
		{
			this.m_wearId = (uint)skinId;
		}

		public static void InitHeroSkinDicData()
		{
			CSkinInfo.s_heroSkinDic.Clear();
			Dictionary<long, object>.Enumerator enumerator = GameDataMgr.heroSkinDatabin.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<long, object> current = enumerator.get_Current();
				ResHeroSkin resHeroSkin = current.get_Value() as ResHeroSkin;
				if (resHeroSkin != null && GameDataMgr.heroDatabin.GetDataByKey(resHeroSkin.dwHeroID) != null)
				{
					if (!CSkinInfo.s_heroSkinDic.ContainsKey(resHeroSkin.dwHeroID))
					{
						ListView<ResHeroSkin> value = new ListView<ResHeroSkin>();
						CSkinInfo.s_heroSkinDic.Add(resHeroSkin.dwHeroID, value);
					}
					CSkinInfo.s_heroSkinDic[resHeroSkin.dwHeroID].Add(resHeroSkin);
				}
			}
		}

		public static bool IsCanBuy(uint heroId, uint skinId)
		{
			ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(heroId, skinId);
			if (heroSkin == null)
			{
				return false;
			}
			if (!GameDataMgr.IsSkinAvailableAtShop(heroSkin.dwID))
			{
				return false;
			}
			ResSkinPromotion resSkinPromotion = new ResSkinPromotion();
			stPayInfoSet stPayInfoSet = default(stPayInfoSet);
			resSkinPromotion = CSkinInfo.GetSkinPromotion(heroSkin.dwID);
			stPayInfoSet payInfoSetOfGood;
			if (resSkinPromotion != null)
			{
				payInfoSetOfGood = CMallSystem.GetPayInfoSetOfGood(false, 0u, resSkinPromotion.bIsBuyCoupons > 0, resSkinPromotion.dwBuyCoupons, resSkinPromotion.bIsBuyDiamond > 0, resSkinPromotion.dwBuyDiamond, 10000u);
			}
			else
			{
				payInfoSetOfGood = CMallSystem.GetPayInfoSetOfGood(heroSkin);
			}
			return payInfoSetOfGood.m_payInfoCount > 0;
		}

		public static stPayInfoSet GetSkinPayInfoSet(uint heroId, uint skinId)
		{
			ResSkinPromotion resPromotion = new ResSkinPromotion();
			stPayInfoSet stPayInfoSet = default(stPayInfoSet);
			resPromotion = CSkinInfo.GetSkinPromotion(heroId, skinId);
			ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(heroId, skinId);
			return CMallSystem.GetPayInfoSetOfGood(heroSkin, resPromotion);
		}

		public static ResHeroSkin GetHeroSkin(uint heroId, uint skinId)
		{
			ListView<ResHeroSkin> listView = null;
			CSkinInfo.s_heroSkinDic.TryGetValue(heroId, out listView);
			if (listView != null)
			{
				for (int i = 0; i < listView.Count; i++)
				{
					if (listView[i] != null && listView[i].dwHeroID == heroId && listView[i].dwSkinID == skinId)
					{
						return listView[i];
					}
				}
			}
			return CSkinInfo.GetHeroSkin(heroId * 100u + skinId);
		}

		public static ResHeroSkin GetHeroSkin(uint uniSkinId)
		{
			return GameDataMgr.heroSkinDatabin.GetDataByKey(uniSkinId);
		}

		public static string GetHeroSkinPic(uint heroId, uint skinId)
		{
			string result = string.Empty;
			ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(heroId, skinId);
			if (heroSkin != null)
			{
				result = StringHelper.UTF8BytesToString(ref heroSkin.szSkinPicID);
			}
			return result;
		}

		public static string GetSkinIconPath(uint skinCfgId)
		{
			ResHeroSkin dataByKey = GameDataMgr.heroSkinDatabin.GetDataByKey(skinCfgId);
			if (dataByKey != null)
			{
				return CUIUtility.s_Sprite_Dynamic_Icon_Dir + dataByKey.szSkinPicID;
			}
			return string.Empty;
		}

		public static void ResolveHeroSkin(uint skinCfgId, out uint heroId, out uint skinId)
		{
			heroId = 0u;
			skinId = 0u;
			ResHeroSkin dataByKey = GameDataMgr.heroSkinDatabin.GetDataByKey(skinCfgId);
			if (dataByKey != null)
			{
				heroId = dataByKey.dwHeroID;
				skinId = dataByKey.dwSkinID;
			}
		}

		public static uint GetSkinCfgId(uint heroId, uint skinId)
		{
			ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(heroId, skinId);
			if (heroSkin != null)
			{
				return heroSkin.dwID;
			}
			return 0u;
		}

		public uint GetWearSkinId()
		{
			return this.m_wearId;
		}

		public void SetWearSkinId(uint skinId)
		{
			this.m_wearId = skinId;
		}

		public static int GetHeroSkinCnt(uint heroId)
		{
			return CSkinInfo.GetAvailableSkinByHeroId(heroId).Count;
		}

		public static ListView<ResHeroSkin> GetAvailableSkinByHeroId(uint heroId)
		{
			CSkinInfo.s_availableSkin.Clear();
			if (CSkinInfo.s_heroSkinDic.ContainsKey(heroId))
			{
				ListView<ResHeroSkin> listView = CSkinInfo.s_heroSkinDic[heroId];
				for (int i = 0; i < listView.Count; i++)
				{
					if (GameDataMgr.IsSkinAvailable(listView[i].dwID))
					{
						CSkinInfo.s_availableSkin.Add(listView[i]);
					}
				}
			}
			return CSkinInfo.s_availableSkin;
		}

		public static uint GetSkinIdByIndex(uint heroId, int index)
		{
			ListView<ResHeroSkin> availableSkinByHeroId = CSkinInfo.GetAvailableSkinByHeroId(heroId);
			if (availableSkinByHeroId != null && index >= 0 && index < availableSkinByHeroId.Count)
			{
				return availableSkinByHeroId[index].dwSkinID;
			}
			return 0u;
		}

		public static int GetIndexBySkinId(uint heroId, uint skinId)
		{
			ListView<ResHeroSkin> availableSkinByHeroId = CSkinInfo.GetAvailableSkinByHeroId(heroId);
			if (availableSkinByHeroId != null)
			{
				for (int i = 0; i < availableSkinByHeroId.Count; i++)
				{
					if (availableSkinByHeroId[i].dwSkinID == skinId)
					{
						return i;
					}
				}
			}
			return -1;
		}

		public static void GetHeroSkinProp(uint heroId, uint skinId, ref int[] propArr, ref int[] propPctArr, ref string[] propIconArr)
		{
			int num = 37;
			for (int i = 0; i < num; i++)
			{
				propArr[i] = 0;
				propPctArr[i] = 0;
				propIconArr[i] = string.Empty;
			}
			ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(heroId, skinId);
			if (heroSkin != null)
			{
				for (int j = 0; j < heroSkin.astAttr.Length; j++)
				{
					if (heroSkin.astAttr[j].wType == 0)
					{
						break;
					}
					if (heroSkin.astAttr[j].bValType == 0)
					{
						propArr[(int)heroSkin.astAttr[j].wType] += heroSkin.astAttr[j].iValue;
					}
					else if (heroSkin.astAttr[j].bValType == 1)
					{
						propPctArr[(int)heroSkin.astAttr[j].wType] += heroSkin.astAttr[j].iValue;
					}
				}
			}
		}

		public static int GetCombatEft(uint heroId, uint skinId)
		{
			ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(heroId, skinId);
			if (heroSkin != null)
			{
				return (int)heroSkin.dwCombatAbility;
			}
			return 0;
		}

		public static ResSkinPromotion GetSkinPromotion(uint uniSkinId)
		{
			ResHeroSkinShop resHeroSkinShop = null;
			GameDataMgr.skinShopInfoDict.TryGetValue(uniSkinId, out resHeroSkinShop);
			if (resHeroSkinShop == null)
			{
				return null;
			}
			for (int i = 0; i < 5; i++)
			{
				uint num = resHeroSkinShop.PromotionID[i];
				if (num != 0u && GameDataMgr.skinPromotionDict.ContainsKey(num))
				{
					ResSkinPromotion resSkinPromotion = new ResSkinPromotion();
					if (GameDataMgr.skinPromotionDict.TryGetValue(num, out resSkinPromotion) && (ulong)resSkinPromotion.dwOnTimeGen <= (ulong)((long)CRoleInfo.GetCurrentUTCTime()) && (ulong)resSkinPromotion.dwOffTimeGen >= (ulong)((long)CRoleInfo.GetCurrentUTCTime()))
					{
						return resSkinPromotion;
					}
				}
			}
			return null;
		}

		public static ResSkinPromotion GetSkinPromotion(uint heroId, uint skinId)
		{
			uint skinCfgId = CSkinInfo.GetSkinCfgId(heroId, skinId);
			return CSkinInfo.GetSkinPromotion(skinCfgId);
		}

		public static string GetSkinName(uint skinUniId)
		{
			ResHeroSkin dataByKey = GameDataMgr.heroSkinDatabin.GetDataByKey(skinUniId);
			if (dataByKey != null)
			{
				return StringHelper.UTF8BytesToString(ref dataByKey.szSkinName);
			}
			return string.Empty;
		}

		public static uint GetHeroSkinCost(uint heroId, uint skinId, RES_SHOPBUY_COINTYPE costType)
		{
			ResHeroSkinShop resHeroSkinShop = null;
			uint skinCfgId = CSkinInfo.GetSkinCfgId(heroId, skinId);
			GameDataMgr.skinShopInfoDict.TryGetValue(skinCfgId, out resHeroSkinShop);
			uint result = 0u;
			if (resHeroSkinShop == null)
			{
				return result;
			}
			switch (costType)
			{
			case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_SKINCOIN:
				return resHeroSkinShop.dwBuySkinCoin;
			case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_DIAMOND:
				return resHeroSkinShop.dwBuyDiamond;
			}
			if (costType != RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_COUPONS)
			{
				return result;
			}
			return resHeroSkinShop.dwBuyCoupons;
		}

		public static bool IsBuyForbiddenForRankBigGrade(uint heroId, uint skinId, out RES_RANK_LIMIT_TYPE rankLimitType, out byte rankLimitBigGrade, out ulong rankLimitParam, out bool isHaveRankBigGradeLimit)
		{
			uint skinCfgId = CSkinInfo.GetSkinCfgId(heroId, skinId);
			ResHeroSkinShop resHeroSkinShop;
			if (GameDataMgr.skinShopInfoDict.TryGetValue(skinCfgId, out resHeroSkinShop))
			{
				rankLimitType = (RES_RANK_LIMIT_TYPE)resHeroSkinShop.bRankLimitType;
				rankLimitBigGrade = resHeroSkinShop.bRankLimitGrade;
				rankLimitParam = resHeroSkinShop.ullRankLimitParam;
				switch (rankLimitType)
				{
				case RES_RANK_LIMIT_TYPE.RES_RANK_LIMIT_CURGRADE:
					isHaveRankBigGradeLimit = true;
					return CLadderSystem.GetRankBigGrade(Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_rankGrade) < rankLimitBigGrade;
				case RES_RANK_LIMIT_TYPE.RES_RANK_LIMIT_SEASONGRADE:
					isHaveRankBigGradeLimit = true;
					return CLadderSystem.GetRankBigGrade(Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_rankSeasonHighestGrade) < rankLimitBigGrade;
				case RES_RANK_LIMIT_TYPE.RES_RANK_LIMIT_MAXGRADE:
					isHaveRankBigGradeLimit = true;
					return CLadderSystem.GetRankBigGrade(Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_rankHistoryHighestGrade) < rankLimitBigGrade;
				case RES_RANK_LIMIT_TYPE.RES_RANK_LIMIT_HISTORYGRADE:
					isHaveRankBigGradeLimit = true;
					if (Singleton<CLadderSystem>.GetInstance().IsCurSeason(rankLimitParam))
					{
						return CLadderSystem.GetRankBigGrade(Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_rankGrade) < rankLimitBigGrade;
					}
					return CLadderSystem.GetRankBigGrade(Singleton<CLadderSystem>.GetInstance().GetHistorySeasonGrade(rankLimitParam)) < rankLimitBigGrade;
				}
			}
			rankLimitType = RES_RANK_LIMIT_TYPE.RES_RANK_LIMIT_NULL;
			rankLimitBigGrade = 0;
			rankLimitParam = 0uL;
			isHaveRankBigGradeLimit = false;
			return false;
		}

		public static bool GetSkinFeatureCnt(uint heroId, uint skinId, out int cnt)
		{
			cnt = 0;
			if (skinId == 0u)
			{
				return false;
			}
			ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(heroId, skinId);
			if (heroSkin != null)
			{
				for (int i = 0; i < 10; i++)
				{
					if (string.IsNullOrEmpty(heroSkin.astFeature[i].szDesc))
					{
						break;
					}
					cnt++;
				}
			}
			return cnt > 0;
		}
	}
}
