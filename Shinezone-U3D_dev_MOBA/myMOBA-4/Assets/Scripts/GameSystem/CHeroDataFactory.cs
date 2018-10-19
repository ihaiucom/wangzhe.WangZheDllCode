using Assets.Scripts.Framework;
using ResData;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameSystem
{
	internal class CHeroDataFactory
	{
		private static ListView<ResHeroCfgInfo> m_CfgHeroList;

		private static ListView<ResHeroCfgInfo> m_CfgHeroShopList;

		private static ListView<IHeroData> m_banHeroList;

		public static IHeroData CreateHeroData(uint id)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			DebugHelper.Assert(masterRoleInfo != null, "CreateHeroData ---- Master role is null");
			CHeroInfo info;
			if (masterRoleInfo != null && masterRoleInfo.GetHeroInfo(id, out info, true))
			{
				return new CHeroInfoData(info);
			}
			return new CHeroCfgData(id);
		}

		public static IHeroData CreateCustomHeroData(uint id)
		{
			return new CCustomHeroData(id);
		}

		public static bool IsHeroCanUse(uint heroID)
		{
			bool result = false;
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return result;
			}
			if (masterRoleInfo.GetHeroInfoDic().ContainsKey(heroID))
			{
				result = true;
			}
			else if (masterRoleInfo.IsFreeHero(heroID))
			{
				result = true;
			}
			return result;
		}

		public static ListView<IHeroData> GetHostHeroList(bool isIncludeValidExperienceHero, CMallSortHelper.HeroViewSortType sortType = CMallSortHelper.HeroViewSortType.Name)
		{
			ListView<IHeroData> listView = new ListView<IHeroData>();
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return listView;
			}
			DictionaryView<uint, CHeroInfo>.Enumerator enumerator = masterRoleInfo.GetHeroInfoDic().GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (isIncludeValidExperienceHero)
				{
					CRoleInfo arg_41_0 = masterRoleInfo;
					KeyValuePair<uint, CHeroInfo> current = enumerator.Current;
					if (arg_41_0.IsOwnHero(current.Key))
					{
						goto IL_66;
					}
					CRoleInfo arg_5C_0 = masterRoleInfo;
					KeyValuePair<uint, CHeroInfo> current2 = enumerator.Current;
					if (arg_5C_0.IsValidExperienceHero(current2.Key))
					{
						goto IL_66;
					}
					continue;
					IL_66:
					ListView<IHeroData> arg_7C_0 = listView;
					KeyValuePair<uint, CHeroInfo> current3 = enumerator.Current;
					arg_7C_0.Add(CHeroDataFactory.CreateHeroData(current3.Key));
				}
				else
				{
					CRoleInfo arg_97_0 = masterRoleInfo;
					KeyValuePair<uint, CHeroInfo> current4 = enumerator.Current;
					if (arg_97_0.IsOwnHero(current4.Key))
					{
						ListView<IHeroData> arg_B7_0 = listView;
						KeyValuePair<uint, CHeroInfo> current5 = enumerator.Current;
						arg_B7_0.Add(CHeroDataFactory.CreateHeroData(current5.Key));
					}
				}
			}
			if (CSysDynamicBlock.bLobbyEntryBlocked)
			{
				for (int i = listView.Count - 1; i >= 0; i--)
				{
					IHeroData heroData = listView[i];
					if (heroData.heroCfgInfo.bIOSHide > 0)
					{
						listView.Remove(heroData);
					}
				}
			}
			CHeroOverviewSystem.SortHeroList(ref listView, sortType, false);
			return listView;
		}

		public static ListView<IHeroData> GetPvPHeroList(CMallSortHelper.HeroViewSortType sortType = CMallSortHelper.HeroViewSortType.Name)
		{
			ListView<IHeroData> listView = new ListView<IHeroData>();
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return listView;
			}
			DictionaryView<uint, CHeroInfo>.Enumerator enumerator = masterRoleInfo.GetHeroInfoDic().GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, CHeroInfo> current = enumerator.Current;
				if ((current.Value.MaskBits & 2u) > 0u)
				{
					ListView<IHeroData> arg_5D_0 = listView;
					KeyValuePair<uint, CHeroInfo> current2 = enumerator.Current;
					arg_5D_0.Add(CHeroDataFactory.CreateHeroData(current2.Key));
				}
			}
			for (int i = 0; i < masterRoleInfo.freeHeroList.Count; i++)
			{
				if (!masterRoleInfo.GetHeroInfoDic().ContainsKey(masterRoleInfo.freeHeroList[i].dwFreeHeroID))
				{
					listView.Add(CHeroDataFactory.CreateHeroData(masterRoleInfo.freeHeroList[i].dwFreeHeroID));
				}
			}
			if (CSysDynamicBlock.bLobbyEntryBlocked)
			{
				for (int j = listView.Count - 1; j >= 0; j--)
				{
					IHeroData heroData = listView[j];
					if (heroData.heroCfgInfo.bIOSHide > 0)
					{
						listView.Remove(heroData);
					}
				}
			}
			CHeroOverviewSystem.SortHeroList(ref listView, sortType, false);
			return listView;
		}

		public static ListView<IHeroData> GetTrainingHeroList(CMallSortHelper.HeroViewSortType sortType = CMallSortHelper.HeroViewSortType.Name)
		{
			ListView<IHeroData> listView = new ListView<IHeroData>();
			List<uint> list = new List<uint>();
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return listView;
			}
			DictionaryView<uint, CHeroInfo>.Enumerator enumerator = masterRoleInfo.GetHeroInfoDic().GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, CHeroInfo> current = enumerator.Current;
				if ((current.Value.MaskBits & 2u) > 0u)
				{
					ListView<IHeroData> arg_63_0 = listView;
					KeyValuePair<uint, CHeroInfo> current2 = enumerator.Current;
					arg_63_0.Add(CHeroDataFactory.CreateHeroData(current2.Key));
					List<uint> arg_79_0 = list;
					KeyValuePair<uint, CHeroInfo> current3 = enumerator.Current;
					arg_79_0.Add(current3.Key);
				}
			}
			for (int i = 0; i < masterRoleInfo.freeHeroList.Count; i++)
			{
				if (!masterRoleInfo.GetHeroInfoDic().ContainsKey(masterRoleInfo.freeHeroList[i].dwFreeHeroID))
				{
					listView.Add(CHeroDataFactory.CreateHeroData(masterRoleInfo.freeHeroList[i].dwFreeHeroID));
					list.Add(masterRoleInfo.freeHeroList[i].dwFreeHeroID);
				}
			}
			ListView<ResHeroCfgInfo> allHeroList = CHeroDataFactory.GetAllHeroList();
			for (int j = 0; j < allHeroList.Count; j++)
			{
				if (allHeroList[j].bIsTrainUse == 1 && !list.Contains(allHeroList[j].dwCfgID))
				{
					listView.Add(CHeroDataFactory.CreateHeroData(allHeroList[j].dwCfgID));
				}
			}
			if (CSysDynamicBlock.bLobbyEntryBlocked)
			{
				for (int k = listView.Count - 1; k >= 0; k--)
				{
					IHeroData heroData = listView[k];
					if (heroData.heroCfgInfo.bIOSHide > 0)
					{
						listView.Remove(heroData);
					}
				}
			}
			CHeroOverviewSystem.SortHeroList(ref listView, sortType, false);
			return listView;
		}

		public static ListView<ResHeroCfgInfo> GetAllHeroList()
		{
			if (CHeroDataFactory.m_CfgHeroList == null)
			{
				CHeroDataFactory.m_CfgHeroList = new ListView<ResHeroCfgInfo>();
			}
			if (CHeroDataFactory.m_CfgHeroList.Count > 0)
			{
				CHeroDataFactory.m_CfgHeroList.Clear();
			}
			GameDataMgr.heroDatabin.Accept(delegate(ResHeroCfgInfo x)
			{
				if (GameDataMgr.IsHeroAvailable(x.dwCfgID) && (!CSysDynamicBlock.bLobbyEntryBlocked || x.bIOSHide == 0))
				{
					CHeroDataFactory.m_CfgHeroList.Add(x);
				}
			});
			return CHeroDataFactory.m_CfgHeroList;
		}

		public static ListView<IHeroData> GetBanHeroList()
		{
			if (CHeroDataFactory.m_banHeroList == null)
			{
				CHeroDataFactory.m_banHeroList = new ListView<IHeroData>();
			}
			if (CHeroDataFactory.m_banHeroList.Count > 0)
			{
				CHeroDataFactory.m_banHeroList.Clear();
			}
			GameDataMgr.heroDatabin.Accept(delegate(ResHeroCfgInfo x)
			{
				if (GameDataMgr.IsHeroAvailable(x.dwCfgID) && (!CSysDynamicBlock.bLobbyEntryBlocked || x.bIOSHide == 0))
				{
					CHeroDataFactory.m_banHeroList.Add(CHeroDataFactory.CreateHeroData(x.dwCfgID));
				}
			});
			return CHeroDataFactory.m_banHeroList;
		}

		public static ListView<ResHeroCfgInfo> GetAllHeroListAtShop()
		{
			if (CHeroDataFactory.m_CfgHeroShopList == null)
			{
				CHeroDataFactory.m_CfgHeroShopList = new ListView<ResHeroCfgInfo>();
			}
			if (CHeroDataFactory.m_CfgHeroShopList.Count > 0)
			{
				CHeroDataFactory.m_CfgHeroShopList.Clear();
			}
			GameDataMgr.heroDatabin.Accept(delegate(ResHeroCfgInfo x)
			{
				if (GameDataMgr.IsHeroAvailableAtShop(x.dwCfgID) && (!CSysDynamicBlock.bLobbyEntryBlocked || x.bIOSHide == 0))
				{
					CHeroDataFactory.m_CfgHeroShopList.Add(x);
				}
			});
			return CHeroDataFactory.m_CfgHeroShopList;
		}

		public static void ResetBufferList()
		{
			if (CHeroDataFactory.m_CfgHeroList != null && CHeroDataFactory.m_CfgHeroList.Count > 0)
			{
				CHeroDataFactory.m_CfgHeroList.Clear();
			}
			if (CHeroDataFactory.m_CfgHeroShopList != null && CHeroDataFactory.m_CfgHeroShopList.Count > 0)
			{
				CHeroDataFactory.m_CfgHeroShopList.Clear();
			}
		}
	}
}
