using Apollo;
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
	public class CGuildHelper
	{
		public const string DynamicPrefabDiamondIconName = "90005";

		public const string DynamicPrefabCoinIconName = "90001";

		public const int MoonStarConversionRatio = 6;

		public const int SunStarConversionRatio = 36;

		public const int CrownStarConversionRatio = 216;

		public static readonly string DynamicPrefabPathCrown = CUIUtility.s_Sprite_Dynamic_Guild_Dir + "Guild_Icon_crown";

		public static readonly string DynamicPrefabPathSun = CUIUtility.s_Sprite_Dynamic_Guild_Dir + "Guild_Icon_sun";

		public static readonly string DynamicPrefabPathMoon = CUIUtility.s_Sprite_Dynamic_Guild_Dir + "Guild_Icon_moon";

		public static readonly string DynamicPrefabPathStar = CUIUtility.s_Sprite_Dynamic_Guild_Dir + "Guild_Icon_star";

		public static string GetBuildingName(int buildingType)
		{
			string text;
			switch (buildingType)
			{
			case 1:
				text = Singleton<CTextManager>.GetInstance().GetText("Guild_Guild_Building_Type_Hall");
				break;
			case 2:
				text = Singleton<CTextManager>.GetInstance().GetText("Guild_Guild_Building_Type_Barrack");
				break;
			case 3:
				text = Singleton<CTextManager>.GetInstance().GetText("Guild_Guild_Building_Type_Factory");
				break;
			case 4:
				text = Singleton<CTextManager>.GetInstance().GetText("Guild_Guild_Building_Type_Statue");
				break;
			case 5:
				text = Singleton<CTextManager>.GetInstance().GetText("Guild_Guild_Building_Type_Shop");
				break;
			default:
				text = Singleton<CTextManager>.GetInstance().GetText("Guild_Guild_Building_Type_Unknown");
				break;
			}
			return text;
		}

		public static bool IsViceChairmanFull()
		{
			ListView<GuildMemInfo> listMemInfo = Singleton<CGuildModel>.GetInstance().CurrentGuildInfo.listMemInfo;
			int num = 0;
			for (int i = 0; i < listMemInfo.Count; i++)
			{
				if (listMemInfo[i].enPosition == COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_VICE_CHAIRMAN)
				{
					num++;
				}
			}
			int viceChairmanMaxCount = CGuildHelper.GetViceChairmanMaxCount();
			return viceChairmanMaxCount > 0 && num >= viceChairmanMaxCount;
		}

		public static int GetViceChairmanMaxCount()
		{
			int guildLevel = CGuildHelper.GetGuildLevel();
			if (guildLevel > 0)
			{
				return (int)GameDataMgr.guildLevelDatabin.GetDataByKey((uint)((byte)guildLevel)).bViceChairManCnt;
			}
			return -1;
		}

		public static void GetViceChairmanUidAndName(out List<ulong> uids, out List<string> names)
		{
			uids = new List<ulong>();
			names = new List<string>();
			ListView<GuildMemInfo> listMemInfo = Singleton<CGuildModel>.GetInstance().CurrentGuildInfo.listMemInfo;
			for (int i = 0; i < listMemInfo.Count; i++)
			{
				if (listMemInfo[i].enPosition == COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_VICE_CHAIRMAN)
				{
					uids.Add(listMemInfo[i].stBriefInfo.uulUid);
					names.Add(listMemInfo[i].stBriefInfo.sName);
				}
			}
		}

		public static ulong GetChairmanUid()
		{
			ListView<GuildMemInfo> listMemInfo = Singleton<CGuildModel>.GetInstance().CurrentGuildInfo.listMemInfo;
			for (int i = 0; i < listMemInfo.Count; i++)
			{
				if (listMemInfo[i].enPosition == COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_CHAIRMAN)
				{
					return listMemInfo[i].stBriefInfo.uulUid;
				}
			}
			return 0uL;
		}

		public static string GetPositionName(COM_PLAYER_GUILD_STATE position)
		{
			string result;
			switch (position)
			{
			case COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_CHAIRMAN:
				result = Singleton<CTextManager>.GetInstance().GetText("Guild_ChairMan");
				break;
			case COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_VICE_CHAIRMAN:
				result = Singleton<CTextManager>.GetInstance().GetText("Guild_Vice_Chairman_Short");
				break;
			case COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_ELDER:
				result = Singleton<CTextManager>.GetInstance().GetText("Guild_Elder");
				break;
			case COM_PLAYER_GUILD_STATE.COM_PLAYER_GUILD_STATE_MEMBER:
				result = Singleton<CTextManager>.GetInstance().GetText("Guild_Normal_Member");
				break;
			default:
				result = string.Empty;
				break;
			}
			return result;
		}

		public static double GetSelfRecommendTimeout()
		{
			uint dwConfValue = GameDataMgr.guildMiscDatabin.GetDataByKey(17u).dwConfValue;
			TimeSpan timeSpan = new TimeSpan(0, 0, 0, (int)dwConfValue);
			return timeSpan.get_TotalHours();
		}

		public static string GetDonateDescription(RES_GUILD_DONATE_TYPE donateType)
		{
			ResGuildDonate dataByKey = GameDataMgr.guildDonateDatabin.GetDataByKey((uint)((byte)donateType));
			uint dwCostGold = dataByKey.dwCostGold;
			uint dwCostCoupons = dataByKey.dwCostCoupons;
			uint dwGetConstruct = dataByKey.dwGetConstruct;
			uint dwGetGuildMoney = dataByKey.dwGetGuildMoney;
			uint dwGetCoinPool = dataByKey.dwGetCoinPool;
			string text = (dwCostGold == 0u) ? dwCostCoupons.ToString() : dwCostGold.ToString();
			string text2 = Singleton<CTextManager>.GetInstance().GetText((dwCostGold == 0u) ? "Money_Type_DianQuan" : "Money_Type_GoldCoin");
			return Singleton<CTextManager>.GetInstance().GetText("Guild_Donate_Description", new string[]
			{
				text,
				text2,
				dwGetConstruct.ToString(),
				dwGetGuildMoney.ToString(),
				dwGetCoinPool.ToString()
			});
		}

		public static string GetDonateSuccessTip(RES_GUILD_DONATE_TYPE donateType)
		{
			ResGuildDonate dataByKey = GameDataMgr.guildDonateDatabin.GetDataByKey((uint)((byte)donateType));
			uint dwGetConstruct = dataByKey.dwGetConstruct;
			uint dwGetGuildMoney = dataByKey.dwGetGuildMoney;
			uint dwGetCoinPool = dataByKey.dwGetCoinPool;
			return Singleton<CTextManager>.GetInstance().GetText("Guild_Donate_Success", new string[]
			{
				dwGetConstruct.ToString(),
				dwGetGuildMoney.ToString(),
				dwGetCoinPool.ToString()
			});
		}

		public static uint GetDonateCostCoin(RES_GUILD_DONATE_TYPE donateType)
		{
			ResGuildDonate dataByKey = GameDataMgr.guildDonateDatabin.GetDataByKey((uint)((byte)donateType));
			return dataByKey.dwCostGold;
		}

		public static uint GetDonateCostDianQuan(RES_GUILD_DONATE_TYPE donateType)
		{
			ResGuildDonate dataByKey = GameDataMgr.guildDonateDatabin.GetDataByKey((uint)((byte)donateType));
			return dataByKey.dwCostCoupons;
		}

		public static bool IsDonateUseCoin(RES_GUILD_DONATE_TYPE donateType)
		{
			ResGuildDonate dataByKey = GameDataMgr.guildDonateDatabin.GetDataByKey((uint)((byte)donateType));
			return dataByKey.dwCostGold != 0u;
		}

		public static int GetMaxGuildMemberCountByLevel(int guildLevel)
		{
			ResGuildLevel dataByKey = GameDataMgr.guildLevelDatabin.GetDataByKey((uint)((byte)guildLevel));
			if (dataByKey != null)
			{
				return (int)dataByKey.bMaxMemberCnt;
			}
			DebugHelper.Assert(false, "CGuildHelper.GetMaxGuildMemberCountByLevel(): resGuildLevel is null, guildLevel={0}", new object[]
			{
				guildLevel
			});
			return -1;
		}

		public static int GetUpgradeCostDianQuanByLevel(int guildLevel)
		{
			ResGuildLevel dataByKey = GameDataMgr.guildLevelDatabin.GetDataByKey((uint)((byte)guildLevel));
			if (dataByKey != null)
			{
				return dataByKey.iUpgradeCostCoupons;
			}
			DebugHelper.Assert(false, "CGuildHelper.GetUpgradeCostDianQuanByLevel(): resGuildLevel is null, guildLevel={0}", new object[]
			{
				guildLevel
			});
			return -1;
		}

		public static RankpointRankInfo GetPlayerGuildRankpointRankInfo(enGuildRankpointRankListType rankListType)
		{
			ListView<RankpointRankInfo> listView = Singleton<CGuildModel>.GetInstance().RankpointRankInfoLists[(int)rankListType];
			for (int i = 0; i < listView.Count; i++)
			{
				if (Singleton<CGuildModel>.GetInstance().CurrentGuildInfo.briefInfo.uulUid == listView[i].guildId)
				{
					return listView[i];
				}
			}
			return CGuildHelper.CreatePlayerGuildRankpointRankInfo(rankListType);
		}

		public static bool IsWeekRankpointRank(enGuildRankpointRankListType rankListType)
		{
			return rankListType == enGuildRankpointRankListType.CurrentWeek || rankListType == enGuildRankpointRankListType.LastWeek;
		}

		public static RankpointRankInfo CreatePlayerGuildRankpointRankInfo(enGuildRankpointRankListType rankListType)
		{
			return new RankpointRankInfo
			{
				guildId = Singleton<CGuildModel>.GetInstance().CurrentGuildInfo.briefInfo.uulUid,
				rankScore = (CGuildHelper.IsWeekRankListType(rankListType) ? Singleton<CGuildModel>.GetInstance().CurrentGuildInfo.RankInfo.weekRankPoint : Singleton<CGuildModel>.GetInstance().CurrentGuildInfo.RankInfo.totalRankPoint),
				guildHeadId = Singleton<CGuildModel>.GetInstance().CurrentGuildInfo.briefInfo.dwHeadId,
				guildName = Singleton<CGuildModel>.GetInstance().CurrentGuildInfo.briefInfo.sName,
				guildLevel = Singleton<CGuildModel>.GetInstance().CurrentGuildInfo.briefInfo.bLevel,
				memberNum = (byte)CGuildHelper.GetGuildMemberCount(),
				star = Singleton<CGuildModel>.GetInstance().CurrentGuildInfo.star
			};
		}

		private static bool IsWeekRankListType(enGuildRankpointRankListType rankListType)
		{
			return rankListType == enGuildRankpointRankListType.CurrentWeek || rankListType == enGuildRankpointRankListType.LastWeek;
		}

		public static string GetRankpointClearTimeFormatString()
		{
			uint dwConfValue = GameDataMgr.guildMiscDatabin.GetDataByKey(38u).dwConfValue;
			uint num = Singleton<CGuildModel>.GetInstance().CurrentGuildInfo.RankInfo.seasonStartTime + dwConfValue;
			return Utility.ToUtcTime2Local((long)((ulong)num)).ToString(Singleton<CTextManager>.GetInstance().GetText("Guild_Rankpoint_Clear_Time_Format"));
		}

		public static uint GetRankpointWeekAwardCoin(uint rank)
		{
			ResGuildRankReward resGuildRankReward = GameDataMgr.guildRankRewardDatabin.FindIf((ResGuildRankReward x) => (long)x.iStartRankNo <= (long)((ulong)rank) && (ulong)rank <= (ulong)((long)x.iEndRankNo));
			return (resGuildRankReward != null) ? resGuildRankReward.dwGold : GameDataMgr.guildRankRewardDatabin.GetDataByKey(-1L).dwGold;
		}

		public static uint GetRankpointWeekAwardDiamond(uint rank)
		{
			ResGuildRankReward resGuildRankReward = GameDataMgr.guildRankRewardDatabin.FindIf((ResGuildRankReward x) => (long)x.iStartRankNo <= (long)((ulong)rank) && (ulong)rank <= (ulong)((long)x.iEndRankNo));
			return (resGuildRankReward != null) ? resGuildRankReward.dwDiamond : GameDataMgr.guildRankRewardDatabin.GetDataByKey(-1L).dwDiamond;
		}

		public static uint GetRankpointSeasonAwardCoin(uint grade)
		{
			ResGuildGradeConf dataByKey = GameDataMgr.guildGradeDatabin.GetDataByKey(grade);
			if (dataByKey != null)
			{
				return dataByKey.dwGold;
			}
			return 0u;
		}

		public static uint GetRankpointSeasonAwardDiamond(uint grade)
		{
			ResGuildGradeConf dataByKey = GameDataMgr.guildGradeDatabin.GetDataByKey(grade);
			if (dataByKey != null)
			{
				return dataByKey.dwDiamond;
			}
			return 0u;
		}

		public static string GetHeadUrl(string serverUrl)
		{
			return Singleton<ApolloHelper>.GetInstance().ToSnsHeadUrl(serverUrl);
		}

		public static bool IsFirstGuildListPage(SCPKG_GET_RANKING_LIST_RSP rsp)
		{
			return rsp.stRankingListDetail.stOfSucc.iStart == 1;
		}

		public static GuildMemInfo GetPlayerGuildMemberInfo()
		{
			return Singleton<CGuildModel>.GetInstance().GetPlayerGuildMemberInfo();
		}

		public static GuildMemInfo GetGuildMemberInfoByUid(ulong uid)
		{
			return Singleton<CGuildModel>.GetInstance().GetGuildMemberInfoByUid(uid);
		}

		public static GuildMemInfo GetGuildMemberInfoByName(string name)
		{
			return Singleton<CGuildModel>.GetInstance().GetGuildMemberInfoByName(name);
		}

		public static uint GetPlayerGuildConstruct()
		{
			GuildMemInfo playerGuildMemberInfo = Singleton<CGuildModel>.GetInstance().GetPlayerGuildMemberInfo();
			if (playerGuildMemberInfo != null)
			{
				return playerGuildMemberInfo.dwConstruct;
			}
			DebugHelper.Assert(false, "CGuildHelper.GetPlayerGuildConstruct() playerMemInfo == null!!! Maybe server not send GuildInfo at login time!!!");
			return 0u;
		}

		public static bool IsLastPage(int curPageId, uint totalCnt, int maxCntPerPage)
		{
			int num = (int)Math.Ceiling(totalCnt / (double)maxCntPerPage) - 1;
			return curPageId >= num;
		}

		public static bool IsNeedRequestNewRankpoinRank(enGuildRankpointRankListType rankListType)
		{
			int currentUTCTime = CRoleInfo.GetCurrentUTCTime();
			return currentUTCTime - Singleton<CGuildModel>.GetInstance().RankpointRankLastGottenTimes[(int)rankListType] > 300;
		}

		public static bool IsSelf(ulong playerUid)
		{
			return playerUid == Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID;
		}

		public static int GetNobeLevel(ulong playerUid, uint nobeLevelFromGuild)
		{
			return CGuildHelper.IsSelf(playerUid) ? MonoSingleton<NobeSys>.GetInstance().GetSelfNobeLevel() : ((int)nobeLevelFromGuild);
		}

		public static int GetNobeHeadIconId(ulong playerUid, uint nobeHeadIconIdFromGuild)
		{
			return CGuildHelper.IsSelf(playerUid) ? MonoSingleton<NobeSys>.GetInstance().GetSelfNobeHeadIdx() : ((int)nobeHeadIconIdFromGuild);
		}

		public static bool IsGuildMaxLevel(int curLevel)
		{
			return CGuildHelper.GetUpgradeCostDianQuanByLevel(curLevel) == -1;
		}

		public static bool IsMemberOnline(GuildMemInfo guildMemInfo)
		{
			return guildMemInfo.stBriefInfo.dwGameEntity != 0u;
		}

		public static string GetGuildName()
		{
			if (Singleton<CGuildSystem>.GetInstance().IsInNormalGuild())
			{
				return Singleton<CGuildModel>.GetInstance().CurrentGuildInfo.briefInfo.sName;
			}
			return string.Empty;
		}

		public static ulong GetGuildUid()
		{
			if (Singleton<CGuildSystem>.GetInstance().IsInNormalGuild())
			{
				return Singleton<CGuildModel>.GetInstance().CurrentGuildInfo.briefInfo.uulUid;
			}
			return 0uL;
		}

		public static int GetGuildLevel()
		{
			if (Singleton<CGuildSystem>.GetInstance().IsInNormalGuild())
			{
				return (int)Singleton<CGuildModel>.GetInstance().CurrentGuildInfo.briefInfo.bLevel;
			}
			return -1;
		}

		public static uint GetGuildStarLevel()
		{
			if (Singleton<CGuildSystem>.GetInstance().IsInNormalGuild())
			{
				return Singleton<CGuildModel>.GetInstance().CurrentGuildInfo.star;
			}
			return 0u;
		}

		public static uint GetGuildGrade()
		{
			if (Singleton<CGuildSystem>.GetInstance().IsInNormalGuild())
			{
				return CGuildHelper.GetGradeByRankpointScore(Singleton<CGuildModel>.GetInstance().CurrentGuildInfo.RankInfo.totalRankPoint);
			}
			return 0u;
		}

		public static string GetGuildHeadPath()
		{
			if (Singleton<CGuildSystem>.GetInstance().IsInNormalGuild())
			{
				return CUIUtility.s_Sprite_Dynamic_GuildHead_Dir + Singleton<CGuildModel>.GetInstance().CurrentGuildInfo.briefInfo.dwHeadId;
			}
			return string.Empty;
		}

		public static uint GetGuildMatchSeasonScore()
		{
			if (Singleton<CGuildSystem>.GetInstance().IsInNormalGuild())
			{
				return Singleton<CGuildModel>.GetInstance().CurrentGuildInfo.GuildMatchInfo.dwScore;
			}
			return 0u;
		}

		public static uint GetGuildMatchWeekScore()
		{
			if (Singleton<CGuildSystem>.GetInstance().IsInNormalGuild())
			{
				return Singleton<CGuildModel>.GetInstance().CurrentGuildInfo.GuildMatchInfo.dwWeekScore;
			}
			return 0u;
		}

		public static uint GetGuildMatchLastWeekRankNo()
		{
			if (Singleton<CGuildSystem>.GetInstance().IsInNormalGuild())
			{
				return Singleton<CGuildModel>.GetInstance().CurrentGuildInfo.GuildMatchInfo.dwLastRankNo;
			}
			return 0u;
		}

		public static uint GetCoinProfitPercentage(int guildLevel)
		{
			int num = guildLevel - 1;
			if (CGuildSystem.s_coinProfitPercentage != null && 0 <= num && num < CGuildSystem.s_coinProfitPercentage.Length)
			{
				return CGuildSystem.s_coinProfitPercentage[num];
			}
			return 0u;
		}

		public static uint GetGuildItemShopOpenSlotCount()
		{
			uint guildStarLevel = CGuildHelper.GetGuildStarLevel();
			if (guildStarLevel == 0u)
			{
				DebugHelper.Assert(false, "error guildStarLevel: {0}!!!", new object[]
				{
					guildStarLevel
				});
				return 0u;
			}
			ResGuildShopStarIndexConf resGuildShopStarIndexConf = GameDataMgr.guildStarLevel.FindIf((ResGuildShopStarIndexConf x) => x.dwBeginStar <= guildStarLevel && guildStarLevel <= x.dwEndStar);
			if (resGuildShopStarIndexConf != null)
			{
				return resGuildShopStarIndexConf.dwGuildItemShopOpenSlotCnt;
			}
			return 0u;
		}

		public static uint GetGuildHeadImageShopOpenSlotCount()
		{
			uint guildGrade = CGuildHelper.GetGuildGrade();
			if (guildGrade > 0u)
			{
				return GameDataMgr.guildGradeDatabin.GetDataByKey(guildGrade).dwGuildHeadImageShopOpenSlotCnt;
			}
			DebugHelper.Assert(false, "error guild grade: {0}!!!", new object[]
			{
				guildGrade
			});
			return 0u;
		}

		public static uint GetStarLevelForOpenGuildItemShopSlot(int slotOffset)
		{
			uint num = 0u;
			int count = GameDataMgr.guildStarLevel.count;
			for (int i = 0; i < count; i++)
			{
				ResGuildShopStarIndexConf dataByIndex = GameDataMgr.guildStarLevel.GetDataByIndex(i);
				if ((ulong)num <= (ulong)((long)slotOffset) && (long)slotOffset < (long)((ulong)dataByIndex.dwGuildItemShopOpenSlotCnt))
				{
					return dataByIndex.dwBeginStar;
				}
				num = dataByIndex.dwGuildItemShopOpenSlotCnt;
			}
			DebugHelper.Assert(false, "error slotOffset{0}: check shop and guildStarLevel res!!!", new object[]
			{
				slotOffset
			});
			return 0u;
		}

		public static string GetGradeNameForOpenGuildHeadImageShopSlot(int slotOffset)
		{
			uint num = 0u;
			int count = GameDataMgr.guildGradeDatabin.count;
			for (int i = 0; i < count; i++)
			{
				ResGuildGradeConf dataByIndex = GameDataMgr.guildGradeDatabin.GetDataByIndex(i);
				if ((ulong)num <= (ulong)((long)slotOffset) && (long)slotOffset < (long)((ulong)dataByIndex.dwGuildHeadImageShopOpenSlotCnt))
				{
					return StringHelper.UTF8BytesToString(ref dataByIndex.szGradeDesc);
				}
				num = dataByIndex.dwGuildHeadImageShopOpenSlotCnt;
			}
			DebugHelper.Assert(false, "error slotOffset{0}: check shop and guildGrade res!!!", new object[]
			{
				slotOffset
			});
			return string.Empty;
		}

		public static ListView<GuildMemInfo> GetGuildMemberInfos()
		{
			return Singleton<CGuildModel>.GetInstance().CurrentGuildInfo.listMemInfo;
		}

		public static int GetRankpointMemberListPlayerIndex()
		{
			List<KeyValuePair<ulong, MemberRankInfo>> rankpointMemberInfoList = Singleton<CGuildModel>.GetInstance().RankpointMemberInfoList;
			for (int i = 0; i < rankpointMemberInfoList.get_Count(); i++)
			{
				if (rankpointMemberInfoList.get_Item(i).get_Key() == Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID)
				{
					return i;
				}
			}
			return -1;
		}

		public static void SetStarLevelPanel(uint starLevel, Transform panelTransform, CUIFormScript form)
		{
			if (panelTransform == null)
			{
				return;
			}
			int num = (int)(starLevel / 216u);
			int num2 = (int)(starLevel % 216u / 36u);
			int num3 = (int)(starLevel % 216u % 36u / 6u);
			int num4 = (int)(starLevel % 216u % 36u % 6u);
			int childCount = panelTransform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				Transform child = panelTransform.GetChild(i);
				if (child == null)
				{
					return;
				}
				Image component = child.GetComponent<Image>();
				if (component == null)
				{
					return;
				}
				child.gameObject.CustomSetActive(true);
				if (i < num)
				{
					component.SetSprite(CGuildHelper.DynamicPrefabPathCrown, form, true, false, false, false);
				}
				else if (i < num + num2)
				{
					component.SetSprite(CGuildHelper.DynamicPrefabPathSun, form, true, false, false, false);
				}
				else if (i < num + num2 + num3)
				{
					component.SetSprite(CGuildHelper.DynamicPrefabPathMoon, form, true, false, false, false);
				}
				else if (i < num + num2 + num3 + num4)
				{
					component.SetSprite(CGuildHelper.DynamicPrefabPathStar, form, true, false, false, false);
				}
				else
				{
					child.gameObject.CustomSetActive(false);
				}
			}
			CUICommonSystem.SetCommonTipsEvent(form, panelTransform.gameObject, CGuildHelper.GetStarLevelTipString(starLevel), enUseableTipsPos.enTop);
		}

		private static string GetStarLevelTipString(uint starLevel)
		{
			return Singleton<CTextManager>.GetInstance().GetText("Guild_StarLevel_Current", new string[]
			{
				starLevel.ToString()
			});
		}

		public static string GetGradeName(uint rankpointScore)
		{
			ResGuildGradeConf gradeResByRankpointScore = CGuildHelper.GetGradeResByRankpointScore(rankpointScore);
			return (gradeResByRankpointScore != null) ? StringHelper.BytesToString(gradeResByRankpointScore.szGradeDesc) : string.Empty;
		}

		public static string GetGradeIconPathByRankpointScore(uint rankpointScore)
		{
			ResGuildGradeConf gradeResByRankpointScore = CGuildHelper.GetGradeResByRankpointScore(rankpointScore);
			return (gradeResByRankpointScore != null) ? (CUIUtility.s_Sprite_Dynamic_Guild_Dir + gradeResByRankpointScore.szIcon) : string.Empty;
		}

		public static uint GetGradeByRankpointScore(uint rankpointScore)
		{
			ResGuildGradeConf gradeResByRankpointScore = CGuildHelper.GetGradeResByRankpointScore(rankpointScore);
			return (uint)((gradeResByRankpointScore != null) ? gradeResByRankpointScore.bIndex : 0);
		}

		private static ResGuildGradeConf GetGradeResByRankpointScore(uint rankpointScore)
		{
			for (int i = 0; i < GameDataMgr.guildGradeDatabin.count; i++)
			{
				ResGuildGradeConf dataByIndex = GameDataMgr.guildGradeDatabin.GetDataByIndex(i);
				if ((ulong)rankpointScore <= (ulong)((long)dataByIndex.iScore))
				{
					return dataByIndex;
				}
			}
			return GameDataMgr.guildGradeDatabin.GetDataByIndex(GameDataMgr.guildGradeDatabin.count - 1);
		}

		public static string GetGradeIconPathByGrade(int grade)
		{
			ResGuildGradeConf dataByKey = GameDataMgr.guildGradeDatabin.GetDataByKey((uint)((byte)grade));
			if (dataByKey != null)
			{
				return CUIUtility.s_Sprite_Dynamic_Guild_Dir + dataByKey.szIcon;
			}
			return string.Empty;
		}

		public static bool IsPlayerSigned()
		{
			GuildMemInfo playerGuildMemberInfo = CGuildHelper.GetPlayerGuildMemberInfo();
			return playerGuildMemberInfo != null && playerGuildMemberInfo.RankInfo.isSigned;
		}

		public static void SetPlayerSigned(bool isSigned)
		{
			GuildMemInfo playerGuildMemberInfo = CGuildHelper.GetPlayerGuildMemberInfo();
			if (playerGuildMemberInfo != null)
			{
				playerGuildMemberInfo.RankInfo.isSigned = isSigned;
			}
		}

		public static int GetGuildMemberCount()
		{
			return Singleton<CGuildModel>.GetInstance().CurrentGuildInfo.listMemInfo.Count;
		}

		public static bool IsGuildMemberFull()
		{
			return CGuildHelper.GetGuildMemberCount() >= CGuildHelper.GetMaxGuildMemberCountByLevel(CGuildHelper.GetGuildLevel());
		}

		public static bool IsGuildMaxGrade()
		{
			ResGuildGradeConf dataByIndex = GameDataMgr.guildGradeDatabin.GetDataByIndex(GameDataMgr.guildGradeDatabin.count - 1);
			return dataByIndex != null && CGuildHelper.GetGuildGrade() == (uint)dataByIndex.bIndex;
		}

		public static bool IsGuildHighestMatchScore()
		{
			ResGuildMisc dataByKey = GameDataMgr.guildMiscDatabin.GetDataByKey(50u);
			if (dataByKey != null)
			{
				uint guildMatchLastWeekRankNo = CGuildHelper.GetGuildMatchLastWeekRankNo();
				if (guildMatchLastWeekRankNo != 0u)
				{
					return guildMatchLastWeekRankNo <= dataByKey.dwConfValue;
				}
			}
			return false;
		}

		public static int GetGuildLogicWorldId()
		{
			GuildBaseInfo baseGuildInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_baseGuildInfo;
			if (baseGuildInfo != null)
			{
				return baseGuildInfo.logicWorldId;
			}
			return 0;
		}

		public static int GetMemberLogicWorldId(ulong memberUid)
		{
			GuildMemInfo guildMemberInfoByUid = CGuildHelper.GetGuildMemberInfoByUid(memberUid);
			if (guildMemberInfoByUid != null)
			{
				return guildMemberInfoByUid.stBriefInfo.dwLogicWorldId;
			}
			return 0;
		}

		public static byte GetSendGuildMailCnt()
		{
			GuildExtInfo extGuildInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_extGuildInfo;
			if (extGuildInfo != null)
			{
				return extGuildInfo.bSendGuildMailCnt;
			}
			return 0;
		}

		public static void SetSendGuildMailCnt(byte sendGuildMailCnt)
		{
			GuildExtInfo extGuildInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_extGuildInfo;
			if (extGuildInfo != null)
			{
				extGuildInfo.bSendGuildMailCnt = sendGuildMailCnt;
			}
		}

		public static uint GetSendGuildMailLimit()
		{
			ResGuildMisc dataByKey = GameDataMgr.guildMiscDatabin.GetDataByKey(46u);
			if (dataByKey != null)
			{
				return dataByKey.dwConfValue;
			}
			return 0u;
		}

		public static bool IsSelfInGuildMemberList()
		{
			ulong playerUllUID = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID;
			ListView<GuildMemInfo> listMemInfo = Singleton<CGuildModel>.GetInstance().CurrentGuildInfo.listMemInfo;
			if (listMemInfo != null)
			{
				for (int i = 0; i < listMemInfo.Count; i++)
				{
					if (listMemInfo[i].stBriefInfo.uulUid == playerUllUID)
					{
						return true;
					}
				}
			}
			return false;
		}

		public static bool IsInLastQuitGuildCd()
		{
			if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_extGuildInfo.dwLastQuitGuildTime != 0u)
			{
				int currentUTCTime = CRoleInfo.GetCurrentUTCTime();
				uint dwConfValue = GameDataMgr.guildMiscDatabin.GetDataByKey(7u).dwConfValue;
				int num = (int)((ulong)(Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().m_extGuildInfo.dwLastQuitGuildTime + dwConfValue) - (ulong)((long)currentUTCTime));
				TimeSpan timeSpan = new TimeSpan(0, 0, 0, num);
				if (num > 0)
				{
					Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Guild_Cannot_Apply_Tip", new string[]
					{
						((int)timeSpan.get_TotalMinutes()).ToString(),
						timeSpan.get_Seconds().ToString()
					}), false, 1.5f, null, new object[0]);
					return true;
				}
			}
			return false;
		}

		public static bool IsGuildMatchLeaderPosition(GuildMemInfo guildMemInfo)
		{
			return guildMemInfo != null && Convert.ToBoolean(guildMemInfo.GuildMatchInfo.bIsLeader);
		}

		public static bool IsGuildMatchLeaderPosition(ulong memberUid)
		{
			GuildMemInfo guildMemberInfoByUid = CGuildHelper.GetGuildMemberInfoByUid(memberUid);
			return CGuildHelper.IsGuildMatchLeaderPosition(guildMemberInfoByUid);
		}

		public static int GetGuildMatchLeftCntInCurRound(int curMatchCnt)
		{
			int dwConfValue = (int)GameDataMgr.guildMiscDatabin.GetDataByKey(48u).dwConfValue;
			return dwConfValue - curMatchCnt;
		}

		public static bool IsGuildMatchReachMatchCntLimit(GuildMemInfo guildMemInfo)
		{
			return guildMemInfo != null && CGuildHelper.GetGuildMatchLeftCntInCurRound((int)guildMemInfo.GuildMatchInfo.bWeekMatchCnt) <= 0;
		}

		public static int GuildMemberComparisonForInvite(GuildMemInfo a, GuildMemInfo b)
		{
			if (CGuildHelper.IsMemberOnline(a) && !CGuildHelper.IsMemberOnline(b))
			{
				return -1;
			}
			if (!CGuildHelper.IsMemberOnline(a) && CGuildHelper.IsMemberOnline(b))
			{
				return 1;
			}
			if (a.isGuildMatchSignedUp && !b.isGuildMatchSignedUp)
			{
				return -1;
			}
			if (!a.isGuildMatchSignedUp && b.isGuildMatchSignedUp)
			{
				return 1;
			}
			return (a.stBriefInfo.uulUid < b.stBriefInfo.uulUid) ? -1 : 1;
		}

		public static bool IsInSameGuild(ulong playerUid)
		{
			return Singleton<CGuildSystem>.GetInstance().IsInNormalGuild() && CGuildHelper.GetGuildMemberInfoByUid(playerUid) != null;
		}

		public static uint GetGuildMemberMinPvpLevel()
		{
			return GameDataMgr.guildMiscDatabin.GetDataByKey(10u).dwConfValue;
		}

		public static bool IsGuildNeedApproval(uint guildSettingMask)
		{
			return Convert.ToBoolean(guildSettingMask & 1u);
		}

		public static string GetLadderGradeLimitText(int gradeLimit)
		{
			string result = string.Empty;
			int bLogicGrade = (int)CLadderSystem.GetGradeDataByShowGrade(gradeLimit).bLogicGrade;
			if (bLogicGrade > 1)
			{
				result = CLadderView.GetRankName((byte)gradeLimit, 0u);
			}
			else if (bLogicGrade == 1)
			{
				result = Singleton<CTextManager>.GetInstance().GetText("Guild_No_Grade_Limit_Tip");
			}
			return result;
		}

		public static string GetGuildJoinLimitText(int levelLimit, int gradeLimit, uint settingMask)
		{
			string text = Singleton<CTextManager>.GetInstance().GetText("Guild_List_Colunm_Limit_Level", new string[]
			{
				levelLimit.ToString()
			});
			string text2 = Singleton<CTextManager>.GetInstance().GetText("Guild_List_Colunm_Limit_Ladder_Grade", new string[]
			{
				CGuildHelper.GetLadderGradeLimitText(gradeLimit)
			});
			string text3 = Singleton<CTextManager>.GetInstance().GetText(CGuildHelper.IsGuildNeedApproval(settingMask) ? "Guild_List_Colunm_Limit_Need_Apply" : "Guild_List_Colunm_Limit_No_Need_Apply");
			return string.Concat(new string[]
			{
				text,
				"、",
				text2,
				"\n",
				text3
			});
		}

		public static bool IsReachGuildJoinLimit(int playerPvpLevel, int playerRankGrade)
		{
			GuildInfo currentGuildInfo = Singleton<CGuildModel>.GetInstance().CurrentGuildInfo;
			return playerPvpLevel >= (int)currentGuildInfo.briefInfo.LevelLimit && CLadderSystem.GetGradeDataByShowGrade(CGuildHelper.GetFixedPlayerRankGrade(playerRankGrade)).bLogicGrade >= CLadderSystem.GetGradeDataByShowGrade((int)currentGuildInfo.briefInfo.GradeLimit).bLogicGrade;
		}

		public static int GetFixedPlayerRankGrade(int playerRankGrade)
		{
			if (playerRankGrade == 0)
			{
				return (int)CLadderSystem.GetGradeDataByLogicGrade(1).bGrade;
			}
			return playerRankGrade;
		}

		public static byte GetFixedGuildLevelLimit(byte originalLevelLimit)
		{
			uint guildMemberMinPvpLevel = CGuildHelper.GetGuildMemberMinPvpLevel();
			if ((uint)originalLevelLimit < guildMemberMinPvpLevel)
			{
				return (byte)guildMemberMinPvpLevel;
			}
			return originalLevelLimit;
		}

		public static byte GetFixedGuildGradeLimit(byte originalGradeLimit)
		{
			if (originalGradeLimit < 1)
			{
				return 1;
			}
			return originalGradeLimit;
		}

		public static bool IsLobbyFormGuildBtnShowRedDot()
		{
			return CGuildHelper.IsGuildInfoFormGuildMatchBtnShowRedDot() || (Singleton<CGuildSystem>.GetInstance().IsInNormalGuild() && !CGuildHelper.IsPlayerSigned());
		}

		public static bool IsGuildInfoFormGuildMatchBtnShowRedDot()
		{
			if (!Singleton<CGuildSystem>.GetInstance().IsInNormalGuild())
			{
				return false;
			}
			bool flag = Singleton<CGuildMatchSystem>.GetInstance().IsSelfBelongedTeamLeader();
			bool flag2 = Singleton<CGuildMatchSystem>.GetInstance().IsHaveInvitationUnhandled();
			if (Singleton<CGuildMatchSystem>.GetInstance().IsInGuildMatchTime() || PlayerPrefs.GetInt("GuildMatch_GuildMatchBtnClicked") == 0)
			{
				return true;
			}
			if (flag)
			{
				if (Singleton<CGuildMatchSystem>.GetInstance().m_isHaveNewSignUpInfo || CGuildHelper.IsCanShowRedDotForTeamNotFullReason())
				{
					return true;
				}
			}
			else if (PlayerPrefs.GetInt("GuildMatch_InvitationTabClicked") == 0 && flag2)
			{
				return true;
			}
			return false;
		}

		public static bool IsGuildMatchFormSignUpListBtnShowRedDot()
		{
			if (!Singleton<CGuildSystem>.GetInstance().IsInNormalGuild())
			{
				return false;
			}
			bool flag = Singleton<CGuildMatchSystem>.GetInstance().IsSelfBelongedTeamLeader();
			return flag && Singleton<CGuildMatchSystem>.GetInstance().m_isHaveNewSignUpInfo;
		}

		public static bool IsGuildMatchFormInvitationTabShowRedDot()
		{
			if (!Singleton<CGuildSystem>.GetInstance().IsInNormalGuild())
			{
				return false;
			}
			bool flag = Singleton<CGuildMatchSystem>.GetInstance().IsSelfBelongedTeamLeader();
			bool flag2 = Singleton<CGuildMatchSystem>.GetInstance().IsHaveInvitationUnhandled();
			return !flag && PlayerPrefs.GetInt("GuildMatch_InvitationTabClicked") == 0 && flag2;
		}

		public static bool IsGuildMatchFormInviteTabShowRedDot()
		{
			if (!Singleton<CGuildSystem>.GetInstance().IsInNormalGuild())
			{
				return false;
			}
			bool flag = Singleton<CGuildMatchSystem>.GetInstance().IsSelfBelongedTeamLeader();
			return flag && CGuildHelper.IsCanShowRedDotForTeamNotFullReason();
		}

		private static bool IsCanShowRedDotForTeamNotFullReason()
		{
			bool flag = Singleton<CGuildMatchSystem>.GetInstance().IsTeamFull(Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().playerUllUID);
			bool flag2 = true;
			int @int = PlayerPrefs.GetInt("GuildMatch_GuildMatchEndTimeRecrod");
			if (CRoleInfo.GetCurrentUTCTime() <= @int)
			{
				flag2 = false;
			}
			return !flag && flag2;
		}

		public static uint GetGroupGuildId()
		{
			return Singleton<CGuildModel>.GetInstance().CurrentGuildInfo.groupGuildId;
		}

		public static string GetBindQQGroupSignature()
		{
			ApolloAccountInfo accountInfo = Singleton<ApolloHelper>.GetInstance().GetAccountInfo(false);
			if (accountInfo != null)
			{
				string text = string.Concat(new object[]
				{
					accountInfo.OpenId,
					"_",
					Singleton<ApolloHelper>.GetInstance().GetAppId(),
					"_",
					Singleton<ApolloHelper>.GetInstance().GetAppKey(),
					"_",
					CGuildHelper.GetGroupGuildId(),
					"_",
					CGuildHelper.GetGuildLogicWorldId()
				});
				Debug.Log("signature=" + text);
				return Utility.CreateMD5Hash(text, Utility.MD5_STRING_CASE.UPPER);
			}
			return string.Empty;
		}
	}
}
