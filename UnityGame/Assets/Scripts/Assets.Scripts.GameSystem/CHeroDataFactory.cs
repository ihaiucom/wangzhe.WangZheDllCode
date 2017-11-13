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
					CRoleInfo cRoleInfo = masterRoleInfo;
					KeyValuePair<uint, CHeroInfo> current = enumerator.Current;
					if (!cRoleInfo.IsOwnHero(current.get_Key()))
					{
						CRoleInfo cRoleInfo2 = masterRoleInfo;
						KeyValuePair<uint, CHeroInfo> current2 = enumerator.Current;
						if (!cRoleInfo2.IsValidExperienceHero(current2.get_Key()))
						{
							continue;
						}
					}
					ListView<IHeroData> listView2 = listView;
					KeyValuePair<uint, CHeroInfo> current3 = enumerator.Current;
					listView2.Add(CHeroDataFactory.CreateHeroData(current3.get_Key()));
				}
				else
				{
					CRoleInfo cRoleInfo3 = masterRoleInfo;
					KeyValuePair<uint, CHeroInfo> current4 = enumerator.Current;
					if (cRoleInfo3.IsOwnHero(current4.get_Key()))
					{
						ListView<IHeroData> listView3 = listView;
						KeyValuePair<uint, CHeroInfo> current5 = enumerator.Current;
						listView3.Add(CHeroDataFactory.CreateHeroData(current5.get_Key()));
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
				if ((current.get_Value().MaskBits & 2u) > 0u)
				{
					ListView<IHeroData> listView2 = listView;
					KeyValuePair<uint, CHeroInfo> current2 = enumerator.Current;
					listView2.Add(CHeroDataFactory.CreateHeroData(current2.get_Key()));
				}
			}
			for (int i = 0; i < masterRoleInfo.freeHeroList.get_Count(); i++)
			{
				if (!masterRoleInfo.GetHeroInfoDic().ContainsKey(masterRoleInfo.freeHeroList.get_Item(i).dwFreeHeroID))
				{
					listView.Add(CHeroDataFactory.CreateHeroData(masterRoleInfo.freeHeroList.get_Item(i).dwFreeHeroID));
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
				if ((current.get_Value().MaskBits & 2u) > 0u)
				{
					ListView<IHeroData> listView2 = listView;
					KeyValuePair<uint, CHeroInfo> current2 = enumerator.Current;
					listView2.Add(CHeroDataFactory.CreateHeroData(current2.get_Key()));
					List<uint> list2 = list;
					KeyValuePair<uint, CHeroInfo> current3 = enumerator.Current;
					list2.Add(current3.get_Key());
				}
			}
			for (int i = 0; i < masterRoleInfo.freeHeroList.get_Count(); i++)
			{
				if (!masterRoleInfo.GetHeroInfoDic().ContainsKey(masterRoleInfo.freeHeroList.get_Item(i).dwFreeHeroID))
				{
					listView.Add(CHeroDataFactory.CreateHeroData(masterRoleInfo.freeHeroList.get_Item(i).dwFreeHeroID));
					list.Add(masterRoleInfo.freeHeroList.get_Item(i).dwFreeHeroID);
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
