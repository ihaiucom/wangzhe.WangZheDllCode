using Assets.Scripts.Framework;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using UnityEngine;

namespace Assets.Scripts.GameSystem
{
	public class CUseableManager
	{
		public static CUseable CreateUseable(COM_ITEM_TYPE useableType, ulong objID, uint baseID, int bCount = 0, int addTime = 0)
		{
			CUseable result = null;
			if (useableType == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP)
			{
				result = new CItem(objID, baseID, bCount, addTime);
			}
			else if (useableType == COM_ITEM_TYPE.COM_OBJTYPE_ITEMEQUIP)
			{
				result = new CEquip(objID, baseID, bCount, addTime);
			}
			else if (useableType == COM_ITEM_TYPE.COM_OBJTYPE_HERO)
			{
				result = new CHeroItem(objID, baseID, bCount, addTime);
			}
			else if (useableType == COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL)
			{
				result = new CSymbolItem(objID, baseID, bCount, addTime);
			}
			else if (useableType == COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN)
			{
				result = new CHeroSkin(objID, baseID, bCount, addTime);
			}
			else if (useableType == COM_ITEM_TYPE.COM_OBJTYPE_HEADIMG)
			{
				result = new CHeadImg(objID, baseID, 0);
			}
			return result;
		}

		public static CUseable CreateUseable(COM_ITEM_TYPE useableType, uint baseID, int bCount = 0)
		{
			CUseable result = null;
			if (useableType == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP)
			{
				result = new CItem(0uL, baseID, bCount, 0);
			}
			else if (useableType == COM_ITEM_TYPE.COM_OBJTYPE_ITEMEQUIP)
			{
				result = new CEquip(0uL, baseID, bCount, 0);
			}
			else if (useableType == COM_ITEM_TYPE.COM_OBJTYPE_HERO)
			{
				result = new CHeroItem(0uL, baseID, bCount, 0);
			}
			else if (useableType == COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL)
			{
				result = new CSymbolItem(0uL, baseID, bCount, 0);
			}
			else if (useableType == COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN)
			{
				result = new CHeroSkin(0uL, baseID, bCount, 0);
			}
			else if (useableType == COM_ITEM_TYPE.COM_OBJTYPE_HEADIMG)
			{
				result = new CHeadImg(0uL, baseID, 0);
			}
			return result;
		}

		public static CUseable CreateVirtualUseable(enVirtualItemType vType, int bCount)
		{
			return new CVirtualItem(vType, bCount);
		}

		public static CUseable CreateExpUseable(COM_ITEM_TYPE useableType, ulong objID, uint expDays, uint baseID, int bCount = 0, int addTime = 0)
		{
			CUseable result = null;
			if (useableType == COM_ITEM_TYPE.COM_OBJTYPE_HERO)
			{
				result = new CExpHeroItem(0uL, baseID, expDays, bCount, 0);
			}
			else if (useableType == COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN)
			{
				result = new CExpHeroSkin(0uL, baseID, expDays, bCount, 0);
			}
			return result;
		}

		public static CUseable CreateCoinUseable(RES_SHOPBUY_COINTYPE coinType, int count)
		{
			enVirtualItemType bType = enVirtualItemType.enNull;
			switch (coinType)
			{
			case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_COUPONS:
				bType = enVirtualItemType.enDianQuan;
				goto IL_8C;
			case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_PVPCOIN:
				bType = enVirtualItemType.enGoldCoin;
				goto IL_8C;
			case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_BURNINGCOIN:
				bType = enVirtualItemType.enHeart;
				goto IL_8C;
			case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_ARENACOIN:
				bType = enVirtualItemType.enArenaCoin;
				goto IL_8C;
			case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_SKINCOIN:
				bType = enVirtualItemType.enSkinCoin;
				goto IL_8C;
			case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_SYMBOLCOIN:
				bType = enVirtualItemType.enSymbolCoin;
				goto IL_8C;
			case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_DIAMOND:
			case RES_SHOPBUY_COINTYPE.RES_SHOPBUY_TYPE_MIXPAY:
				bType = enVirtualItemType.enDiamond;
				goto IL_8C;
			}
			Debug.LogError("CoinType:" + coinType.ToString() + " is not supported!");
			IL_8C:
			return new CVirtualItem(bType, count);
		}

		public static CUseable CreateUsableByServerType(COM_REWARDS_TYPE type, int cnt, uint baseId)
		{
			CUseable result = null;
			switch (type)
			{
			case COM_REWARDS_TYPE.COM_REWARDS_TYPE_COIN:
				result = CUseableManager.CreateVirtualUseable(enVirtualItemType.enGoldCoin, cnt);
				break;
			case COM_REWARDS_TYPE.COM_REWARDS_TYPE_ITEM:
				result = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP, 0uL, baseId, cnt, 0);
				break;
			case COM_REWARDS_TYPE.COM_REWARDS_TYPE_EXP:
				result = CUseableManager.CreateVirtualUseable(enVirtualItemType.enExp, cnt);
				break;
			case COM_REWARDS_TYPE.COM_REWARDS_TYPE_COUPONS:
				result = CUseableManager.CreateVirtualUseable(enVirtualItemType.enDianQuan, cnt);
				break;
			case COM_REWARDS_TYPE.COM_REWARDS_TYPE_EQUIP:
				result = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMEQUIP, 0uL, baseId, cnt, 0);
				break;
			case COM_REWARDS_TYPE.COM_REWARDS_TYPE_HERO:
				result = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_HERO, baseId, cnt);
				break;
			case COM_REWARDS_TYPE.COM_REWARDS_TYPE_SYMBOL:
				result = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMSYMBOL, 0uL, baseId, cnt, 0);
				break;
			case COM_REWARDS_TYPE.COM_REWARDS_TYPE_BURNING_COIN:
				result = CUseableManager.CreateVirtualUseable(enVirtualItemType.enBurningCoin, cnt);
				break;
			case COM_REWARDS_TYPE.COM_REWARDS_TYPE_ARENA_COIN:
				result = CUseableManager.CreateVirtualUseable(enVirtualItemType.enArenaCoin, cnt);
				break;
			case COM_REWARDS_TYPE.COM_REWARDS_TYPE_AP:
				result = CUseableManager.CreateVirtualUseable(enVirtualItemType.enHeart, cnt);
				break;
			case COM_REWARDS_TYPE.COM_REWARDS_TYPE_SKIN:
				result = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN, 0uL, baseId, cnt, 0);
				break;
			case COM_REWARDS_TYPE.COM_REWARDS_TYPE_HONOUR:
				result = CUseableManager.CreateVirtualUseable(enVirtualItemType.enGoldCoin, cnt);
				break;
			case COM_REWARDS_TYPE.COM_REWARDS_TYPE_HEROPOOLEXP:
				result = CUseableManager.CreateVirtualUseable(enVirtualItemType.enExpPool, cnt);
				break;
			case COM_REWARDS_TYPE.COM_REWARDS_TYPE_SKINCOIN:
				result = CUseableManager.CreateVirtualUseable(enVirtualItemType.enSkinCoin, cnt);
				break;
			case COM_REWARDS_TYPE.COM_REWARDS_TYPE_SYMBOLCOIN:
				result = CUseableManager.CreateVirtualUseable(enVirtualItemType.enSymbolCoin, cnt);
				break;
			case COM_REWARDS_TYPE.COM_REWARDS_TYPE_DIAMOND:
				result = CUseableManager.CreateVirtualUseable(enVirtualItemType.enDiamond, cnt);
				break;
			case COM_REWARDS_TYPE.COM_REWARDS_TYPE_HUOYUEDU:
				result = CUseableManager.CreateVirtualUseable(enVirtualItemType.enHuoyueDu, cnt);
				break;
			case COM_REWARDS_TYPE.COM_REWARDS_TYPE_MATCH_POINT_PERSON:
				result = CUseableManager.CreateVirtualUseable(enVirtualItemType.enMatchPersonalPoint, cnt);
				break;
			case COM_REWARDS_TYPE.COM_REWARDS_TYPE_MATCH_POINT_GUILD:
				result = CUseableManager.CreateVirtualUseable(enVirtualItemType.enMatchTeamPoint, cnt);
				break;
			case COM_REWARDS_TYPE.COM_REWARDS_TYPE_HEADIMAGE:
				result = CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_HEADIMG, 0uL, baseId, cnt, 0);
				break;
			case COM_REWARDS_TYPE.COM_REWARDS_TYPE_ACHIEVE:
				CUseableManager.CreateVirtualUseable(enVirtualItemType.enAchievementPoint, cnt);
				break;
			case COM_REWARDS_TYPE.COM_REWARDS_TYPE_MASTERPOINT:
				result = CUseableManager.CreateVirtualUseable(enVirtualItemType.enMentorPoint, cnt);
				break;
			}
			return result;
		}

		public static CUseable CreateUsableByServerType(RES_REWARDS_TYPE type, int cnt, uint baseId)
		{
			COM_REWARDS_TYPE type2;
			CUseableManager.ResRewardTypeToComRewardType(type, out type2);
			return CUseableManager.CreateUsableByServerType(type2, cnt, baseId);
		}

		public static CUseable CreateUsableByRandowReward(RES_RANDOM_REWARD_TYPE type, int cnt, uint baseId)
		{
			COM_REWARDS_TYPE type2;
			CUseableManager.RandomRewardTypeToComRewardType(type, out type2);
			return CUseableManager.CreateUsableByServerType(type2, cnt, baseId);
		}

		public static ListView<CUseable> CreateUsableListByRandowReward(RES_RANDOM_REWARD_TYPE type, int cnt, uint baseId)
		{
			ListView<CUseable> listView = new ListView<CUseable>();
			ResRandomRewardStore dataByKey;
			if (type != RES_RANDOM_REWARD_TYPE.RES_RANDOM_REWARD_TYPE_NEST)
			{
				CUseable cUseable = CUseableManager.CreateUsableByRandowReward(type, cnt, baseId);
				if (cUseable != null)
				{
					listView.Add(cUseable);
				}
			}
			else if ((dataByKey = GameDataMgr.randomRewardDB.GetDataByKey(baseId)) != null)
			{
				for (int i = 0; i < dataByKey.astRewardDetail.Length; i++)
				{
					if (dataByKey.astRewardDetail[i].bItemType == 0 || dataByKey.astRewardDetail[i].bItemType >= 18)
					{
						break;
					}
					listView.AddRange(CUseableManager.CreateUsableListByRandowReward((RES_RANDOM_REWARD_TYPE)dataByKey.astRewardDetail[i].bItemType, (int)dataByKey.astRewardDetail[i].dwLowCnt, dataByKey.astRewardDetail[i].dwItemID));
				}
			}
			return listView;
		}

		public static void ResRewardTypeToComRewardType(RES_REWARDS_TYPE rType, out COM_REWARDS_TYPE cType)
		{
			switch (rType)
			{
			case RES_REWARDS_TYPE.RES_REWARDS_TYPE_ITEM:
				cType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_ITEM;
				return;
			case RES_REWARDS_TYPE.RES_REWARDS_TYPE_EXP:
				cType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_EXP;
				return;
			case RES_REWARDS_TYPE.RES_REWARDS_TYPE_COUPONS:
				cType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_COUPONS;
				return;
			case RES_REWARDS_TYPE.RES_REWARDS_TYPE_EQUIP:
				cType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_EQUIP;
				return;
			case RES_REWARDS_TYPE.RES_REWARDS_TYPE_SYMBOL:
				cType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_SYMBOL;
				return;
			case RES_REWARDS_TYPE.RES_REWARDS_TYPE_ARENACOIN:
				cType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_ARENA_COIN;
				return;
			case RES_REWARDS_TYPE.RES_REWARDS_TYPE_HONOUR:
				cType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_HONOUR;
				return;
			case RES_REWARDS_TYPE.RES_REWARDS_TYPE_SKINCOIN:
				cType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_SKINCOIN;
				return;
			case RES_REWARDS_TYPE.RES_REWARDS_TYPE_HEROPOOLEXP:
				cType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_HEROPOOLEXP;
				return;
			case RES_REWARDS_TYPE.RES_REWARDS_TYPE_SKIN:
				cType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_SKIN;
				return;
			case RES_REWARDS_TYPE.RES_REWARDS_TYPE_SYMBOLCOIN:
				cType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_SYMBOLCOIN;
				return;
			case RES_REWARDS_TYPE.RES_REWARDS_TYPE_DIAMOND:
				cType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_DIAMOND;
				return;
			case RES_REWARDS_TYPE.RES_REWARDS_TYPE_HUOYUEDU:
				cType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_HUOYUEDU;
				return;
			case RES_REWARDS_TYPE.RES_REWARDS_TYPE_HEADIMAGE:
				cType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_HEADIMAGE;
				return;
			case RES_REWARDS_TYPE.RES_REWARDS_TYPE_HERO:
				cType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_HERO;
				return;
			case RES_REWARDS_TYPE.RES_REWARDS_TYPE_MINGSHIPOINT:
				cType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_MASTERPOINT;
				return;
			}
			cType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_MAX;
		}

		public static void RandomRewardTypeToComRewardType(RES_RANDOM_REWARD_TYPE rType, out COM_REWARDS_TYPE cType)
		{
			switch (rType)
			{
			case RES_RANDOM_REWARD_TYPE.RES_RANDOM_REWARD_TYPE_ITEM:
				cType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_ITEM;
				return;
			case RES_RANDOM_REWARD_TYPE.RES_RANDOM_REWARD_TYPE_EQUIP:
				cType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_EQUIP;
				return;
			case RES_RANDOM_REWARD_TYPE.RES_RANDOM_REWARD_TYPE_HERO:
				cType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_HERO;
				return;
			case RES_RANDOM_REWARD_TYPE.RES_RANDOM_REWARD_TYPE_SYMBOL:
				cType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_SYMBOL;
				return;
			case RES_RANDOM_REWARD_TYPE.RES_RANDOM_REWARD_TYPE_AP:
				cType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_AP;
				return;
			case RES_RANDOM_REWARD_TYPE.RES_RANDOM_REWARD_TYPE_COIN:
				cType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_COIN;
				return;
			case RES_RANDOM_REWARD_TYPE.RES_RANDOM_REWARD_TYPE_COUPONS:
				cType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_COUPONS;
				return;
			case RES_RANDOM_REWARD_TYPE.RES_RANDOM_REWARD_TYPE_BURNINGCOIN:
				cType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_BURNING_COIN;
				return;
			case RES_RANDOM_REWARD_TYPE.RES_RANDOM_REWARD_TYPE_ARENACOIN:
				cType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_ARENA_COIN;
				return;
			case RES_RANDOM_REWARD_TYPE.RES_RANDOM_REWARD_TYPE_SKIN:
				cType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_SKIN;
				return;
			case RES_RANDOM_REWARD_TYPE.RES_RANDOM_REWARD_TYPE_SKINCOIN:
				cType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_SKINCOIN;
				return;
			case RES_RANDOM_REWARD_TYPE.RES_RANDOM_REWARD_TYPE_HEROPOOLEXP:
				cType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_HEROPOOLEXP;
				return;
			case RES_RANDOM_REWARD_TYPE.RES_RANDOM_REWARD_TYPE_SYMBOLCOIN:
				cType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_SYMBOLCOIN;
				return;
			case RES_RANDOM_REWARD_TYPE.RES_RANDOM_REWARD_TYPE_DIAMOND:
				cType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_DIAMOND;
				return;
			case RES_RANDOM_REWARD_TYPE.RES_RANDOM_REWARD_TYPE_HEADIMG:
				cType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_HEADIMAGE;
				return;
			}
			cType = COM_REWARDS_TYPE.COM_REWARDS_TYPE_MAX;
		}

		public static CUseable GetUseableByRewardInfo(ResRandomRewardStore inRewardInfo)
		{
			if (inRewardInfo != null)
			{
				COM_REWARDS_TYPE type;
				CUseableManager.RandomRewardTypeToComRewardType((RES_RANDOM_REWARD_TYPE)inRewardInfo.astRewardDetail[0].bItemType, out type);
				int dwLowCnt = (int)inRewardInfo.astRewardDetail[0].dwLowCnt;
				uint dwItemID = inRewardInfo.astRewardDetail[0].dwItemID;
				return CUseableManager.CreateUsableByServerType(type, dwLowCnt, dwItemID);
			}
			return null;
		}

		public static ListView<CUseable> GetUseableListFromReward(COMDT_REWARD_DETAIL reward)
		{
			ListView<CUseable> listView = new ListView<CUseable>();
			for (int i = 0; i < (int)reward.bNum; i++)
			{
				switch (reward.astRewardDetail[i].bType)
				{
				case 0:
				{
					CUseable item = CUseableManager.CreateUsableByServerType((COM_REWARDS_TYPE)reward.astRewardDetail[i].bType, (int)reward.astRewardDetail[i].stRewardInfo.dwCoin, 0u);
					listView.Add(item);
					break;
				}
				case 1:
				{
					CUseable cUseable = CUseableManager.CreateUsableByServerType((COM_REWARDS_TYPE)reward.astRewardDetail[i].bType, (int)reward.astRewardDetail[i].stRewardInfo.stItem.dwCnt, reward.astRewardDetail[i].stRewardInfo.stItem.dwItemID);
					if (cUseable != null)
					{
						if (reward.astRewardDetail[i].bFromType == 1)
						{
							cUseable.ExtraFromType = (int)reward.astRewardDetail[i].bFromType;
							cUseable.ExtraFromData = (int)reward.astRewardDetail[i].stFromInfo.stHeroInfo.dwHeroID;
						}
						else if (reward.astRewardDetail[i].bFromType == 2)
						{
							cUseable.ExtraFromType = (int)reward.astRewardDetail[i].bFromType;
							cUseable.ExtraFromData = (int)reward.astRewardDetail[i].stFromInfo.stSkinInfo.dwSkinID;
						}
						listView.Add(cUseable);
					}
					break;
				}
				case 3:
				{
					CUseable item2 = CUseableManager.CreateUsableByServerType((COM_REWARDS_TYPE)reward.astRewardDetail[i].bType, (int)reward.astRewardDetail[i].stRewardInfo.dwCoupons, 0u);
					listView.Add(item2);
					break;
				}
				case 4:
				{
					CUseable item3 = CUseableManager.CreateUsableByServerType((COM_REWARDS_TYPE)reward.astRewardDetail[i].bType, (int)reward.astRewardDetail[i].stRewardInfo.stEquip.dwCnt, reward.astRewardDetail[i].stRewardInfo.stEquip.dwEquipID);
					listView.Add(item3);
					break;
				}
				case 5:
				{
					CUseable item4 = CUseableManager.CreateUsableByServerType((COM_REWARDS_TYPE)reward.astRewardDetail[i].bType, (int)reward.astRewardDetail[i].stRewardInfo.stHero.dwCnt, reward.astRewardDetail[i].stRewardInfo.stHero.dwHeroID);
					listView.Add(item4);
					break;
				}
				case 6:
				{
					CUseable item5 = CUseableManager.CreateUsableByServerType((COM_REWARDS_TYPE)reward.astRewardDetail[i].bType, (int)reward.astRewardDetail[i].stRewardInfo.stSymbol.dwCnt, reward.astRewardDetail[i].stRewardInfo.stSymbol.dwSymbolID);
					listView.Add(item5);
					break;
				}
				case 7:
				{
					CUseable item6 = CUseableManager.CreateUsableByServerType((COM_REWARDS_TYPE)reward.astRewardDetail[i].bType, (int)reward.astRewardDetail[i].stRewardInfo.dwBurningCoin, 0u);
					listView.Add(item6);
					break;
				}
				case 8:
				{
					CUseable item7 = CUseableManager.CreateUsableByServerType((COM_REWARDS_TYPE)reward.astRewardDetail[i].bType, (int)reward.astRewardDetail[i].stRewardInfo.dwArenaCoin, 0u);
					listView.Add(item7);
					break;
				}
				case 9:
				{
					CUseable item8 = CUseableManager.CreateUsableByServerType((COM_REWARDS_TYPE)reward.astRewardDetail[i].bType, (int)reward.astRewardDetail[i].stRewardInfo.dwAP, 0u);
					listView.Add(item8);
					break;
				}
				case 10:
				{
					CUseable item9 = CUseableManager.CreateUsableByServerType((COM_REWARDS_TYPE)reward.astRewardDetail[i].bType, (int)reward.astRewardDetail[i].stRewardInfo.stSkin.dwCnt, reward.astRewardDetail[i].stRewardInfo.stSkin.dwSkinID);
					listView.Add(item9);
					break;
				}
				case 11:
				{
					CUseable item10 = CUseableManager.CreateUsableByServerType((COM_REWARDS_TYPE)reward.astRewardDetail[i].bType, (int)reward.astRewardDetail[i].stRewardInfo.dwPvpCoin, 0u);
					listView.Add(item10);
					break;
				}
				case 12:
				{
					CUseable item11 = CUseableManager.CreateUsableByServerType((COM_REWARDS_TYPE)reward.astRewardDetail[i].bType, (int)reward.astRewardDetail[i].stRewardInfo.dwHeroPoolExp, 0u);
					listView.Add(item11);
					break;
				}
				case 13:
				{
					CUseable item12 = CUseableManager.CreateUsableByServerType((COM_REWARDS_TYPE)reward.astRewardDetail[i].bType, (int)reward.astRewardDetail[i].stRewardInfo.dwSkinCoin, 0u);
					listView.Add(item12);
					break;
				}
				case 14:
				{
					CUseable cUseable2 = CUseableManager.CreateUsableByServerType((COM_REWARDS_TYPE)reward.astRewardDetail[i].bType, (int)reward.astRewardDetail[i].stRewardInfo.dwSymbolCoin, 0u);
					if (cUseable2 != null)
					{
						listView.Add(cUseable2);
					}
					break;
				}
				case 16:
				{
					CUseable item13 = CUseableManager.CreateUsableByServerType((COM_REWARDS_TYPE)reward.astRewardDetail[i].bType, (int)reward.astRewardDetail[i].stRewardInfo.dwDiamond, 0u);
					listView.Add(item13);
					break;
				}
				case 17:
				{
					CUseable cUseable3 = CUseableManager.CreateUsableByServerType((COM_REWARDS_TYPE)reward.astRewardDetail[i].bType, (int)reward.astRewardDetail[i].stRewardInfo.dwHuoYueDu, 0u);
					if (cUseable3 != null)
					{
						listView.Add(cUseable3);
					}
					break;
				}
				case 18:
				{
					CUseable item14 = CUseableManager.CreateUsableByServerType((COM_REWARDS_TYPE)reward.astRewardDetail[i].bType, (int)reward.astRewardDetail[i].stRewardInfo.dwMatchPointPer, 0u);
					listView.Add(item14);
					break;
				}
				case 19:
				{
					CUseable item15 = CUseableManager.CreateUsableByServerType((COM_REWARDS_TYPE)reward.astRewardDetail[i].bType, (int)reward.astRewardDetail[i].stRewardInfo.dwMatchPointGuild, 0u);
					listView.Add(item15);
					break;
				}
				case 20:
				{
					CUseable cUseable4 = CUseableManager.CreateUsableByServerType((COM_REWARDS_TYPE)reward.astRewardDetail[i].bType, 1, reward.astRewardDetail[i].stRewardInfo.stHeadImage.dwHeadImgID);
					if (cUseable4 != null)
					{
						listView.Add(cUseable4);
					}
					break;
				}
				case 21:
				{
					CUseable cUseable5 = CUseableManager.CreateUsableByServerType((COM_REWARDS_TYPE)reward.astRewardDetail[i].bType, (int)reward.astRewardDetail[i].stRewardInfo.dwAchieve, 0u);
					if (cUseable5 != null)
					{
						listView.Add(cUseable5);
					}
					break;
				}
				case 22:
				{
					CUseable cUseable6 = CUseableManager.CreateUsableByServerType((COM_REWARDS_TYPE)reward.astRewardDetail[i].bType, (int)reward.astRewardDetail[i].stRewardInfo.dwMasterPoint, 0u);
					if (cUseable6 != null)
					{
						listView.Add(cUseable6);
					}
					break;
				}
				}
			}
			return listView;
		}

		public static ListView<CUseable> GetUseableListFromItemList(COMDT_REWARD_ITEMLIST itemList)
		{
			ListView<CUseable> listView = new ListView<CUseable>();
			for (int i = 0; i < (int)itemList.wRewardCnt; i++)
			{
				ushort wItemType = itemList.astRewardList[i].wItemType;
				ushort wItemCnt = itemList.astRewardList[i].wItemCnt;
				uint dwItemID = itemList.astRewardList[i].dwItemID;
				CUseable cUseable = CUseableManager.CreateUseable((COM_ITEM_TYPE)wItemType, 0uL, dwItemID, (int)wItemCnt, 0);
				if (cUseable != null)
				{
					byte bFromType = itemList.astRewardList[i].bFromType;
					if (bFromType != 1)
					{
						if (bFromType == 2)
						{
							cUseable.ExtraFromType = (int)itemList.astRewardList[i].bFromType;
							cUseable.ExtraFromData = (int)itemList.astRewardList[i].stFromInfo.stSkinInfo.dwSkinID;
						}
					}
					else
					{
						cUseable.ExtraFromType = (int)itemList.astRewardList[i].bFromType;
						cUseable.ExtraFromData = (int)itemList.astRewardList[i].stFromInfo.stHeroInfo.dwHeroID;
					}
					listView.Add(cUseable);
				}
			}
			return listView;
		}

		public static void ShowUseableItem(CUseable item)
		{
			if (item == null)
			{
				return;
			}
			if (item.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP || item.MapRewardType == COM_REWARDS_TYPE.COM_REWARDS_TYPE_ITEM)
			{
				if (item.ExtraFromType == 1)
				{
					int extraFromData = item.ExtraFromData;
					CUICommonSystem.ShowNewHeroOrSkin((uint)extraFromData, 0u, enUIEventID.Activity_HeroShow_Back, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_HERO, true, null, enFormPriority.Priority1, (uint)item.m_stackCount, 0);
				}
				else if (item.ExtraFromType == 2)
				{
					int extraFromData2 = item.ExtraFromData;
					CUICommonSystem.ShowNewHeroOrSkin(0u, (uint)extraFromData2, enUIEventID.Activity_HeroShow_Back, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_SKIN, true, null, enFormPriority.Priority1, (uint)item.m_stackCount, 0);
				}
				else
				{
					CUseable[] items = new CUseable[]
					{
						item
					};
					Singleton<CUIManager>.GetInstance().OpenAwardTip(items, Singleton<CTextManager>.GetInstance().GetText("gotAward"), true, enUIEventID.None, false, true, "Form_Award");
				}
			}
			else if (item is CHeroSkin)
			{
				CHeroSkin cHeroSkin = item as CHeroSkin;
				CUICommonSystem.ShowNewHeroOrSkin(cHeroSkin.m_heroId, cHeroSkin.m_skinId, enUIEventID.Activity_HeroShow_Back, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_SKIN, true, null, enFormPriority.Priority1, 0u, 0);
			}
			else if (item is CHeroItem)
			{
				CUICommonSystem.ShowNewHeroOrSkin(item.m_baseID, 0u, enUIEventID.Activity_HeroShow_Back, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_HERO, true, null, enFormPriority.Priority1, 0u, 0);
			}
			else
			{
				CUseable[] items2 = new CUseable[]
				{
					item
				};
				Singleton<CUIManager>.GetInstance().OpenAwardTip(items2, Singleton<CTextManager>.GetInstance().GetText("gotAward"), true, enUIEventID.None, false, true, "Form_Award");
			}
		}
	}
}
