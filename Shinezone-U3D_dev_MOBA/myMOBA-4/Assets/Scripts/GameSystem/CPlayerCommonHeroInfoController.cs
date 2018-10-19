using Assets.Scripts.Framework;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class CPlayerCommonHeroInfoController : Singleton<CPlayerCommonHeroInfoController>
	{
		public static string sPlayerInfoCommonHeroFormPath = "UGUI/Form/System/Player/Form_Player_Info_CommonHeroInfo.prefab";

		private static string[] sMainTitles = new string[2];

		private static string[] sSubTitles = new string[4];

		private static string[] sSortTitles = new string[7];

		private ListView<COMDT_MOST_USED_HERO_INFO> m_commonHeroList = new ListView<COMDT_MOST_USED_HERO_INFO>();

		private int m_lastSortIndex = -1;

		private byte m_lastIsUp;

		public override void Init()
		{
			CPlayerCommonHeroInfoController.sMainTitles = new string[]
			{
				Singleton<CTextManager>.instance.GetText("Player_Info_Title_36"),
				Singleton<CTextManager>.instance.GetText("Player_Info_Title_37")
			};
			CPlayerCommonHeroInfoController.sSubTitles = new string[]
			{
				Singleton<CTextManager>.instance.GetText("Player_Info_Title_3"),
				Singleton<CTextManager>.instance.GetText("Player_Info_Title_4"),
				Singleton<CTextManager>.instance.GetText("Player_Info_Title_5"),
				Singleton<CTextManager>.instance.GetText("Player_Info_Title_9")
			};
			CPlayerCommonHeroInfoController.sSortTitles = new string[]
			{
				Singleton<CTextManager>.instance.GetText("Player_Info_Title_38"),
				Singleton<CTextManager>.instance.GetText("Player_Info_Title_39"),
				Singleton<CTextManager>.instance.GetText("Player_Info_Title_40"),
				Singleton<CTextManager>.instance.GetText("Player_Info_Title_42"),
				Singleton<CTextManager>.instance.GetText("Player_Info_Title_43"),
				Singleton<CTextManager>.instance.GetText("Player_Info_Title_44"),
				Singleton<CTextManager>.instance.GetText("Player_Info_Title_41")
			};
			Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.Player_Info_Common_Hero_Enable, new CUIEventManager.OnUIEventHandler(this.OnCommonHeroItemEnable));
			Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.Player_Info_Common_Hero_Main_List_Click, new CUIEventManager.OnUIEventHandler(this.OnCommonHeroMainListClick));
			Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.Player_Info_Common_Hero_Sub_List_Click, new CUIEventManager.OnUIEventHandler(this.OnCommonHeroSubListClick));
			Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.Player_Info_Common_Hero_Sort_List_Click, new CUIEventManager.OnUIEventHandler(this.OnCommonHeroSortListClick));
			Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.Player_Info_Common_Hero_Sub_List_Show, new CUIEventManager.OnUIEventHandler(this.OnCommonHeroSubListShow));
			Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.Player_Info_Common_Hero_Detail_List_Enable, new CUIEventManager.OnUIEventHandler(this.OnCommonHeroListEnable));
		}

		public override void UnInit()
		{
			Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.Player_Info_Common_Hero_Enable, new CUIEventManager.OnUIEventHandler(this.OnCommonHeroItemEnable));
			Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.Player_Info_Common_Hero_Main_List_Click, new CUIEventManager.OnUIEventHandler(this.OnCommonHeroMainListClick));
			Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.Player_Info_Common_Hero_Sub_List_Click, new CUIEventManager.OnUIEventHandler(this.OnCommonHeroSubListClick));
			Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.Player_Info_Common_Hero_Sort_List_Click, new CUIEventManager.OnUIEventHandler(this.OnCommonHeroSortListClick));
			Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.Player_Info_Common_Hero_Sub_List_Show, new CUIEventManager.OnUIEventHandler(this.OnCommonHeroSubListShow));
			Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.Player_Info_Common_Hero_Detail_List_Enable, new CUIEventManager.OnUIEventHandler(this.OnCommonHeroListEnable));
		}

		public void OpenForm()
		{
			CUIFormScript cUIFormScript = Singleton<CUIManager>.instance.OpenForm(CPlayerCommonHeroInfoController.sPlayerInfoCommonHeroFormPath, false, true);
			if (cUIFormScript == null)
			{
				return;
			}
			CPlayerProfile profile = Singleton<CPlayerInfoSystem>.instance.GetProfile();
			GameObject widget = cUIFormScript.GetWidget(0);
			if (widget == null)
			{
				return;
			}
			this.m_lastSortIndex = -1;
			this.m_lastIsUp = 0;
			CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(widget, "SubMenuList/List");
			CUIListScript componetInChild2 = Utility.GetComponetInChild<CUIListScript>(widget, "DetailList/MenuList");
			CUICommonSystem.InitMenuPanel(componetInChild.gameObject, CPlayerCommonHeroInfoController.sSubTitles, 0, true);
			CUICommonSystem.InitMenuPanel(componetInChild2.gameObject, CPlayerCommonHeroInfoController.sSortTitles, 0, true);
		}

		public void InitCommonHeroUI()
		{
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CPlayerInfoSystem.sPlayerInfoFormPath);
			if (form == null)
			{
				return;
			}
			CPlayerProfile profile = Singleton<CPlayerInfoSystem>.instance.GetProfile();
			GameObject widget = form.GetWidget(7);
			if (widget == null)
			{
				return;
			}
			CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(widget, "BaseList");
			if (componetInChild == null)
			{
				return;
			}
			int num = profile.MostUsedHeroList().Count;
			uint dwConfValue = GameDataMgr.globalInfoDatabin.GetDataByKey(178u).dwConfValue;
			if ((long)num > (long)((ulong)dwConfValue))
			{
				num = (int)dwConfValue;
			}
			componetInChild.SetElementAmount(num);
		}

		public void UpdateUI()
		{
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CPlayerCommonHeroInfoController.sPlayerInfoCommonHeroFormPath);
			if (form == null)
			{
				return;
			}
			GameObject widget = form.GetWidget(0);
			CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(widget, "SubMenuList/List");
			CUIListScript componetInChild2 = Utility.GetComponetInChild<CUIListScript>(widget, "DetailList/MenuList");
			CUIListScript componetInChild3 = Utility.GetComponetInChild<CUIListScript>(widget, "DetailList/List");
			Text componetInChild4 = Utility.GetComponetInChild<Text>(widget, "SubMenuList/Button_Down/Text");
			int selectedIndex = componetInChild.GetSelectedIndex();
			int selectedIndex2 = componetInChild2.GetSelectedIndex();
			componetInChild.gameObject.CustomSetActive(false);
			componetInChild4.text = CPlayerCommonHeroInfoController.sSubTitles[selectedIndex];
			componetInChild3.SetElementAmount(this.m_commonHeroList.Count);
		}

		private void UpdateSortMenu()
		{
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CPlayerCommonHeroInfoController.sPlayerInfoCommonHeroFormPath);
			if (form == null)
			{
				return;
			}
			GameObject widget = form.GetWidget(0);
			CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(widget, "DetailList/MenuList");
			int selectedIndex = componetInChild.GetSelectedIndex();
			if (this.m_lastSortIndex != selectedIndex)
			{
				this.m_lastIsUp = 0;
			}
			else
			{
                this.m_lastIsUp = ((this.m_lastIsUp != 1) ? (byte)1 : (byte)0);
			}
			this.m_lastSortIndex = selectedIndex;
			int elementAmount = componetInChild.m_elementAmount;
			for (int i = 0; i < elementAmount; i++)
			{
				GameObject gameObject = componetInChild.GetElemenet(i).gameObject;
				GameObject obj = Utility.FindChild(gameObject, "Text/Up");
				GameObject obj2 = Utility.FindChild(gameObject, "Text/Down");
				if (i == selectedIndex)
				{
					obj.CustomSetActive(this.m_lastIsUp == 1);
					obj2.CustomSetActive(this.m_lastIsUp != 1);
				}
				else
				{
					obj.CustomSetActive(false);
					obj2.CustomSetActive(false);
				}
			}
		}

		private void OnCommonHeroItemEnable(CUIEvent uiEvent)
		{
			CPlayerProfile profile = Singleton<CPlayerInfoSystem>.instance.GetProfile();
			int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
			GameObject srcWidget = uiEvent.m_srcWidget;
			GameObject gameObject = Utility.FindChild(srcWidget, "heroItem");
			ListView<COMDT_MOST_USED_HERO_INFO> listView = profile.MostUsedHeroList();
			if (listView == null || srcWidgetIndexInBelongedList >= listView.Count)
			{
				return;
			}
			COMDT_MOST_USED_HERO_INFO cOMDT_MOST_USED_HERO_INFO = listView[srcWidgetIndexInBelongedList];
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return;
			}
			IHeroData heroData = CHeroDataFactory.CreateHeroData(cOMDT_MOST_USED_HERO_INFO.dwHeroID);
			GameObject proficiencyIcon = Utility.FindChild(gameObject, "heroProficiencyImg");
			GameObject proficiencyBg = Utility.FindChild(gameObject, "heroProficiencyBgImg");
			CUICommonSystem.SetHeroProficiencyIconImage(uiEvent.m_srcFormScript, proficiencyIcon, (int)cOMDT_MOST_USED_HERO_INFO.dwProficiencyLv);
			CUICommonSystem.SetHeroProficiencyBgImage(uiEvent.m_srcFormScript, proficiencyBg, (int)cOMDT_MOST_USED_HERO_INFO.dwProficiencyLv, false);
			if (!CPlayerInfoSystem.isSelf(profile.m_uuid))
			{
				CUICommonSystem.SetHeroItemImage(uiEvent.m_srcFormScript, gameObject, CSkinInfo.GetHeroSkinPic(cOMDT_MOST_USED_HERO_INFO.dwHeroID, cOMDT_MOST_USED_HERO_INFO.dwSkinID), enHeroHeadType.enBust, false, true);
			}
			else
			{
				CUICommonSystem.SetHeroItemImage(uiEvent.m_srcFormScript, gameObject, masterRoleInfo.GetHeroSkinPic(cOMDT_MOST_USED_HERO_INFO.dwHeroID), enHeroHeadType.enBust, false, true);
			}
			GameObject root = Utility.FindChild(gameObject, "profession");
			CUICommonSystem.SetHeroJob(uiEvent.m_srcFormScript, root, (enHeroJobType)heroData.heroType);
			Text componetInChild = Utility.GetComponetInChild<Text>(gameObject, "heroNameText");
			componetInChild.text = heroData.heroName;
			Text componetInChild2 = Utility.GetComponetInChild<Text>(gameObject, "TotalCount");
			componetInChild2.text = Singleton<CTextManager>.instance.GetText("Player_Info_PVP_Total_Count", new string[]
			{
				(cOMDT_MOST_USED_HERO_INFO.dwGameWinNum + cOMDT_MOST_USED_HERO_INFO.dwGameLoseNum).ToString()
			});
			Text componetInChild3 = Utility.GetComponetInChild<Text>(gameObject, "WinRate");
			componetInChild3.text = Singleton<CTextManager>.instance.GetText("Player_Info_PVP_Win_Rate", new string[]
			{
				CPlayerProfile.Round(CPlayerProfile.Divide(cOMDT_MOST_USED_HERO_INFO.dwGameWinNum, cOMDT_MOST_USED_HERO_INFO.dwGameWinNum + cOMDT_MOST_USED_HERO_INFO.dwGameLoseNum) * 100f)
			});
			ulong num = 0uL;
			ulong num2 = 0uL;
			ulong num3 = 0uL;
			uint num4 = 0u;
			COMDT_HERO_STATISTIC_DETAIL stStatisticDetail = cOMDT_MOST_USED_HERO_INFO.stStatisticDetail;
			uint dwNum = stStatisticDetail.dwNum;
			int num5 = 0;
			while ((long)num5 < (long)((ulong)dwNum))
			{
				COMDT_HERO_STATISTIC_INFO cOMDT_HERO_STATISTIC_INFO = stStatisticDetail.astTypeDetail[num5];
				num += cOMDT_HERO_STATISTIC_INFO.ullKDAPct;
				num2 += cOMDT_HERO_STATISTIC_INFO.ullTotalHurtHero;
				num3 += cOMDT_HERO_STATISTIC_INFO.ullTotalBeHurt;
				num4 = num4 + cOMDT_HERO_STATISTIC_INFO.dwWinNum + cOMDT_HERO_STATISTIC_INFO.dwLoseNum;
				num5++;
			}
			num4 = ((num4 != 0u) ? num4 : 1u);
			Utility.GetComponetInChild<Text>(gameObject, "AverKDA").text = Singleton<CTextManager>.instance.GetText("Player_Info_PVP_AverKDA", new string[]
			{
				(num / (ulong)num4 / 100uL).ToString("0.0")
			});
			Utility.GetComponetInChild<Text>(gameObject, "AverHurt").text = Singleton<CTextManager>.instance.GetText("Player_Info_PVP_AverHurt", new string[]
			{
				(num2 / (ulong)num4).ToString("d")
			});
			Utility.GetComponetInChild<Text>(gameObject, "AverTakenHurt").text = Singleton<CTextManager>.instance.GetText("Player_Info_PVP_AverTakenHurt", new string[]
			{
				(num3 / (ulong)num4).ToString("d")
			});
		}

		public void OnCommonHeroMainListClick(CUIEvent uiEvent)
		{
			this.OpenForm();
		}

		public void OnCommonHeroSubListClick(CUIEvent uiEvent)
		{
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CPlayerCommonHeroInfoController.sPlayerInfoCommonHeroFormPath);
			if (form == null)
			{
				return;
			}
			GameObject widget = form.GetWidget(0);
			CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(widget, "SubMenuList/List");
			int selectedIndex = componetInChild.GetSelectedIndex();
			byte gameType = CPlayerCommonHeroInfoController.GetGameType(selectedIndex);
			this.m_commonHeroList.Clear();
			ListView<COMDT_MOST_USED_HERO_INFO> listView = Singleton<CPlayerInfoSystem>.instance.GetProfile().MostUsedHeroList();
			for (int i = 0; i < listView.Count; i++)
			{
				int num = 0;
				while ((long)num < (long)((ulong)listView[i].stStatisticDetail.dwNum))
				{
					if ((gameType == 0 || listView[i].stStatisticDetail.astTypeDetail[num].bGameType == gameType) && listView[i].stStatisticDetail.astTypeDetail[num].dwWinNum + listView[i].stStatisticDetail.astTypeDetail[num].dwLoseNum > 0u)
					{
						this.m_commonHeroList.Add(listView[i]);
						break;
					}
					num++;
				}
			}
			CUIListScript componetInChild2 = Utility.GetComponetInChild<CUIListScript>(widget, "DetailList/MenuList");
			int selectedIndex2 = componetInChild2.GetSelectedIndex();
			this.SrotCommonHeroList(selectedIndex2);
			this.UpdateUI();
		}

		public void OnCommonHeroSortListClick(CUIEvent uiEvent)
		{
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CPlayerCommonHeroInfoController.sPlayerInfoCommonHeroFormPath);
			if (form == null)
			{
				return;
			}
			GameObject widget = form.GetWidget(0);
			CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(widget, "SubMenuList/List");
			CUIListScript componetInChild2 = Utility.GetComponetInChild<CUIListScript>(widget, "DetailList/MenuList");
			int selectedIndex = componetInChild2.GetSelectedIndex();
			this.UpdateSortMenu();
			this.SrotCommonHeroList(selectedIndex);
			this.UpdateUI();
		}

		public void OnCommonHeroSubListShow(CUIEvent uiEvent)
		{
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CPlayerCommonHeroInfoController.sPlayerInfoCommonHeroFormPath);
			if (form == null)
			{
				return;
			}
			GameObject widget = form.GetWidget(0);
			GameObject gameObject = Utility.FindChild(widget, "SubMenuList/List");
			gameObject.CustomSetActive(!gameObject.activeSelf);
		}

		public void OnCommonHeroListEnable(CUIEvent uiEvent)
		{
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CPlayerCommonHeroInfoController.sPlayerInfoCommonHeroFormPath);
			if (form == null)
			{
				return;
			}
			int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
			GameObject srcWidget = uiEvent.m_srcWidget;
			if (srcWidgetIndexInBelongedList >= 0 && srcWidgetIndexInBelongedList < this.m_commonHeroList.Count)
			{
				Image componetInChild = Utility.GetComponetInChild<Image>(srcWidget, "HeadImg");
				Text componetInChild2 = Utility.GetComponetInChild<Text>(srcWidget, "NameTxt");
				Text componetInChild3 = Utility.GetComponetInChild<Text>(srcWidget, "UsedCntTxt");
				Text componetInChild4 = Utility.GetComponetInChild<Text>(srcWidget, "WinsTxt");
				Text componetInChild5 = Utility.GetComponetInChild<Text>(srcWidget, "KdaTxt");
				Text componetInChild6 = Utility.GetComponetInChild<Text>(srcWidget, "GoldTxt");
				Text componetInChild7 = Utility.GetComponetInChild<Text>(srcWidget, "HurtTxt");
				Text componetInChild8 = Utility.GetComponetInChild<Text>(srcWidget, "BeHurtTxt");
				COMDT_MOST_USED_HERO_INFO cOMDT_MOST_USED_HERO_INFO = this.m_commonHeroList[srcWidgetIndexInBelongedList];
				ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(cOMDT_MOST_USED_HERO_INFO.dwHeroID);
				string prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Icon_Dir, CSkinInfo.GetHeroSkinPic(this.m_commonHeroList[srcWidgetIndexInBelongedList].dwHeroID, 0u));
				componetInChild.SetSprite(prefabPath, form, true, false, false, false);
				componetInChild2.text = ((dataByKey != null) ? dataByKey.szName : string.Empty);
				GameObject widget = form.GetWidget(0);
				CUIListScript componetInChild9 = Utility.GetComponetInChild<CUIListScript>(widget, "SubMenuList/List");
				int selectedIndex = componetInChild9.GetSelectedIndex();
				byte gameType = CPlayerCommonHeroInfoController.GetGameType(selectedIndex);
				uint num = 0u;
				uint num2 = 0u;
				uint num3 = 0u;
				uint num4 = 0u;
				uint num5 = 0u;
				uint num6 = 0u;
				uint num7 = 0u;
				ulong num8 = 0uL;
				uint dwGameWinNum = cOMDT_MOST_USED_HERO_INFO.dwGameWinNum;
				uint dwGameLoseNum = cOMDT_MOST_USED_HERO_INFO.dwGameLoseNum;
				uint b = dwGameWinNum + dwGameLoseNum;
				if (cOMDT_MOST_USED_HERO_INFO.stStatisticDetail.dwNum > 0u)
				{
					int num9 = 0;
					while ((long)num9 < (long)((ulong)cOMDT_MOST_USED_HERO_INFO.stStatisticDetail.dwNum))
					{
						if (gameType == 0 || cOMDT_MOST_USED_HERO_INFO.stStatisticDetail.astTypeDetail[num9].bGameType == gameType)
						{
							num += cOMDT_MOST_USED_HERO_INFO.stStatisticDetail.astTypeDetail[num9].dwWinNum;
							num2 += cOMDT_MOST_USED_HERO_INFO.stStatisticDetail.astTypeDetail[num9].dwLoseNum;
							num3 += cOMDT_MOST_USED_HERO_INFO.stStatisticDetail.astTypeDetail[num9].dwKill;
							num4 += cOMDT_MOST_USED_HERO_INFO.stStatisticDetail.astTypeDetail[num9].dwDead;
							num5 += cOMDT_MOST_USED_HERO_INFO.stStatisticDetail.astTypeDetail[num9].dwAssist;
							num6 += cOMDT_MOST_USED_HERO_INFO.stStatisticDetail.astTypeDetail[num9].dwGPM;
							num7 += cOMDT_MOST_USED_HERO_INFO.stStatisticDetail.astTypeDetail[num9].dwHurtPM;
							num8 += cOMDT_MOST_USED_HERO_INFO.stStatisticDetail.astTypeDetail[num9].ullTotalBeHurt;
						}
						num9++;
					}
					uint b2 = num + num2;
					if (gameType == 0)
					{
						componetInChild3.text = b.ToString();
						componetInChild4.text = CPlayerProfile.Divide(dwGameWinNum, b).ToString("P0");
					}
					else
					{
						componetInChild3.text = b2.ToString();
						componetInChild4.text = CPlayerProfile.Divide(num, b2).ToString("P0");
					}
					componetInChild5.text = string.Format("{0}/{1}/{2}", CPlayerProfile.Divide(num3, b2).ToString("F1"), CPlayerProfile.Divide(num4, b2).ToString("F1"), CPlayerProfile.Divide(num5, b2).ToString("F1"));
					componetInChild6.text = CPlayerProfile.Divide(num6, b2).ToString("F0");
					componetInChild7.text = CPlayerProfile.Divide(num7, b2).ToString("F0");
					componetInChild8.text = CPlayerProfile.Divide(num8, b2).ToString("F0");
				}
				else
				{
					componetInChild3.text = "0";
					componetInChild4.text = "0";
					componetInChild5.text = "0.0/0.0/0.0";
					componetInChild6.text = "0";
					componetInChild7.text = "0";
					componetInChild8.text = "0";
				}
			}
		}

		private void SrotCommonHeroList(int sortIndex = 0)
		{
			if (sortIndex == 0)
			{
				this.m_commonHeroList.Sort(new Comparison<COMDT_MOST_USED_HERO_INFO>(this.CommonHeroSortOrder));
			}
			else if (sortIndex == 1)
			{
				this.m_commonHeroList.Sort(new Comparison<COMDT_MOST_USED_HERO_INFO>(this.CommonHeroSortUsedCnt));
			}
			else if (sortIndex == 2)
			{
				this.m_commonHeroList.Sort(new Comparison<COMDT_MOST_USED_HERO_INFO>(this.CommonHeroSortWins));
			}
			else if (sortIndex == 3)
			{
				this.m_commonHeroList.Sort(new Comparison<COMDT_MOST_USED_HERO_INFO>(this.CommonHeroSortUsedGold));
			}
			else if (sortIndex == 4)
			{
				this.m_commonHeroList.Sort(new Comparison<COMDT_MOST_USED_HERO_INFO>(this.CommonHeroSortUsedHurt));
			}
			else if (sortIndex == 5)
			{
				this.m_commonHeroList.Sort(new Comparison<COMDT_MOST_USED_HERO_INFO>(this.CommonHeroSortUsedBeHurt));
			}
			else if (sortIndex == 6)
			{
				this.m_commonHeroList.Sort(new Comparison<COMDT_MOST_USED_HERO_INFO>(this.CommonHeroSortKda));
			}
		}

		private int CommonHeroSortOrder(COMDT_MOST_USED_HERO_INFO left, COMDT_MOST_USED_HERO_INFO right)
		{
			ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(left.dwHeroID);
			ResHeroCfgInfo dataByKey2 = GameDataMgr.heroDatabin.GetDataByKey(right.dwHeroID);
			if (dataByKey == null || dataByKey2 == null)
			{
				return 0;
			}
			return (int)(((this.m_lastIsUp != 1) ? 1u : 4294967295u) * (dataByKey2.dwShowSortId - dataByKey.dwShowSortId));
		}

		private int CommonHeroSortUsedCnt(COMDT_MOST_USED_HERO_INFO left, COMDT_MOST_USED_HERO_INFO right)
		{
			return (int)(((this.m_lastIsUp != 1) ? 1u : 4294967295u) * (right.dwGameWinNum + right.dwGameLoseNum - left.dwGameWinNum - left.dwGameLoseNum));
		}

		private int CommonHeroSortWins(COMDT_MOST_USED_HERO_INFO left, COMDT_MOST_USED_HERO_INFO right)
		{
			return (int)(((this.m_lastIsUp != 1) ? 1u : 4294967295u) * (1000u * right.dwGameWinNum / (right.dwGameWinNum + right.dwGameLoseNum) - 1000u * left.dwGameWinNum / (left.dwGameWinNum + left.dwGameLoseNum)));
		}

		private int CommonHeroSortKda(COMDT_MOST_USED_HERO_INFO left, COMDT_MOST_USED_HERO_INFO right)
		{
			float num = (left.stStatisticDetail.dwNum <= 0u) ? 0f : (CPlayerProfile.Divide(left.stStatisticDetail.astTypeDetail[0].ullKDAPct, left.stStatisticDetail.astTypeDetail[0].dwWinNum + left.stStatisticDetail.astTypeDetail[0].dwLoseNum) * 100f);
			float num2 = (right.stStatisticDetail.dwNum <= 0u) ? 0f : (CPlayerProfile.Divide(right.stStatisticDetail.astTypeDetail[0].ullKDAPct, right.stStatisticDetail.astTypeDetail[0].dwWinNum + right.stStatisticDetail.astTypeDetail[0].dwLoseNum) * 100f);
			return ((this.m_lastIsUp != 1) ? 1 : -1) * ((int)num2 - (int)num);
		}

		private int CommonHeroSortUsedGold(COMDT_MOST_USED_HERO_INFO left, COMDT_MOST_USED_HERO_INFO right)
		{
			float num = (left.stStatisticDetail.dwNum <= 0u) ? 0f : (CPlayerProfile.Divide(left.stStatisticDetail.astTypeDetail[0].dwGPM, left.stStatisticDetail.astTypeDetail[0].dwWinNum + left.stStatisticDetail.astTypeDetail[0].dwLoseNum) * 100f);
			float num2 = (right.stStatisticDetail.dwNum <= 0u) ? 0f : (CPlayerProfile.Divide(right.stStatisticDetail.astTypeDetail[0].dwGPM, right.stStatisticDetail.astTypeDetail[0].dwWinNum + right.stStatisticDetail.astTypeDetail[0].dwLoseNum) * 100f);
			return ((this.m_lastIsUp != 1) ? 1 : -1) * ((int)num2 - (int)num);
		}

		private int CommonHeroSortUsedHurt(COMDT_MOST_USED_HERO_INFO left, COMDT_MOST_USED_HERO_INFO right)
		{
			float num = (left.stStatisticDetail.dwNum <= 0u) ? 0f : (CPlayerProfile.Divide(left.stStatisticDetail.astTypeDetail[0].dwHurtPM, left.stStatisticDetail.astTypeDetail[0].dwWinNum + left.stStatisticDetail.astTypeDetail[0].dwLoseNum) * 100f);
			float num2 = (right.stStatisticDetail.dwNum <= 0u) ? 0f : (CPlayerProfile.Divide(right.stStatisticDetail.astTypeDetail[0].dwHurtPM, right.stStatisticDetail.astTypeDetail[0].dwWinNum + right.stStatisticDetail.astTypeDetail[0].dwLoseNum) * 100f);
			return ((this.m_lastIsUp != 1) ? 1 : -1) * ((int)num2 - (int)num);
		}

		private int CommonHeroSortUsedBeHurt(COMDT_MOST_USED_HERO_INFO left, COMDT_MOST_USED_HERO_INFO right)
		{
			float num = (left.stStatisticDetail.dwNum <= 0u) ? 0f : (CPlayerProfile.Divide(left.stStatisticDetail.astTypeDetail[0].ullTotalBeHurt, left.stStatisticDetail.astTypeDetail[0].dwWinNum + left.stStatisticDetail.astTypeDetail[0].dwLoseNum) * 100f);
			float num2 = (right.stStatisticDetail.dwNum <= 0u) ? 0f : (CPlayerProfile.Divide(right.stStatisticDetail.astTypeDetail[0].ullTotalBeHurt, right.stStatisticDetail.astTypeDetail[0].dwWinNum + right.stStatisticDetail.astTypeDetail[0].dwLoseNum) * 100f);
			return ((this.m_lastIsUp != 1) ? 1 : -1) * ((int)num2 - (int)num);
		}

		private static byte GetGameType(int subIndex)
		{
			if (subIndex == 0)
			{
				return 0;
			}
			if (subIndex == 1)
			{
				return 4;
			}
			if (subIndex == 2)
			{
				return 5;
			}
			if (subIndex == 3)
			{
				return 11;
			}
			return 0;
		}
	}
}
