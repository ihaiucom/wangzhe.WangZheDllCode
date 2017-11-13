using Assets.Scripts.Framework;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	[MessageHandlerClass]
	public class CPlayerPvpInfoController : Singleton<CPlayerPvpInfoController>
	{
		private static string sCacheSubTabStr = "sgame_{0}_playerinfo_pvpinfo_detailmode";

		private static string[] sMainTitles = new string[2];

		private static string[] sSubTitles = new string[7];

		private static Vector3[] sTemVector;

		private bool m_initedBaseValue;

		private uint m_baseValueKDA;

		private uint m_baseValueGold;

		private uint m_baseValueBattle;

		private uint m_baseValueAlive;

		private uint m_baseValueHurtHero;

		private float[] m_ability = new float[7];

		private static int CacheSubTab
		{
			get
			{
				return PlayerPrefs.GetInt(string.Format(CPlayerPvpInfoController.sCacheSubTabStr, Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().playerUllUID.ToString()));
			}
			set
			{
				PlayerPrefs.SetInt(string.Format(CPlayerPvpInfoController.sCacheSubTabStr, Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().playerUllUID.ToString()), value);
			}
		}

		public override void Init()
		{
			CPlayerPvpInfoController.sMainTitles = new string[]
			{
				Singleton<CTextManager>.instance.GetText("Player_Info_Title_1"),
				Singleton<CTextManager>.instance.GetText("Player_Info_Title_2")
			};
			CPlayerPvpInfoController.sSubTitles = new string[]
			{
				Singleton<CTextManager>.instance.GetText("Player_Info_Title_3"),
				Singleton<CTextManager>.instance.GetText("Player_Info_Title_4"),
				Singleton<CTextManager>.instance.GetText("Player_Info_Title_5"),
				Singleton<CTextManager>.instance.GetText("Player_Info_Title_6"),
				Singleton<CTextManager>.instance.GetText("Player_Info_Title_7"),
				Singleton<CTextManager>.instance.GetText("Player_Info_Title_8"),
				Singleton<CTextManager>.instance.GetText("Player_Info_Title_9")
			};
			Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.Player_Info_Pvp_MainList_Click, new CUIEventManager.OnUIEventHandler(this.OnMainListClick));
			Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.Player_Info_Pvp_SubList_Click, new CUIEventManager.OnUIEventHandler(this.OnSubListClick));
			Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.Player_Info_Pvp_SubList_Show, new CUIEventManager.OnUIEventHandler(this.OnSubListShow));
			Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.Player_Info_Pvp_Graph_Show, new CUIEventManager.OnUIEventHandler(this.OnGraphShow));
			Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.Player_Info_Pvp_Detail_Show, new CUIEventManager.OnUIEventHandler(this.OnDetailShow));
		}

		public override void UnInit()
		{
			Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.Player_Info_Pvp_MainList_Click, new CUIEventManager.OnUIEventHandler(this.OnMainListClick));
			Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.Player_Info_Pvp_SubList_Click, new CUIEventManager.OnUIEventHandler(this.OnSubListClick));
			Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.Player_Info_Pvp_SubList_Show, new CUIEventManager.OnUIEventHandler(this.OnSubListShow));
			Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.Player_Info_Pvp_Graph_Show, new CUIEventManager.OnUIEventHandler(this.OnGraphShow));
			Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.Player_Info_Pvp_Detail_Show, new CUIEventManager.OnUIEventHandler(this.OnDetailShow));
		}

		public void OnMainListClick(CUIEvent uiEvent)
		{
			this.UpdateUI();
		}

		public void OnSubListClick(CUIEvent uiEvent)
		{
			this.UpdateUI();
		}

		public void OnSubListShow(CUIEvent uiEvent)
		{
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CPlayerInfoSystem.sPlayerInfoFormPath);
			if (form == null)
			{
				return;
			}
			GameObject widget = form.GetWidget(2);
			CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(widget, "panelTop/DropList/List");
			componetInChild.gameObject.CustomSetActive(!componetInChild.gameObject.activeSelf);
		}

		public void OnGraphShow(CUIEvent uiEvent)
		{
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CPlayerInfoSystem.sPlayerInfoFormPath);
			if (form == null)
			{
				return;
			}
			GameObject widget = form.GetWidget(2);
			Utility.FindChild(widget, "btnGraph").CustomSetActive(false);
			Utility.FindChild(widget, "btnDetail").CustomSetActive(true);
			this.UpdateUI();
		}

		public void OnDetailShow(CUIEvent uiEvent)
		{
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CPlayerInfoSystem.sPlayerInfoFormPath);
			if (form == null)
			{
				return;
			}
			GameObject widget = form.GetWidget(2);
			Utility.FindChild(widget, "btnGraph").CustomSetActive(true);
			Utility.FindChild(widget, "btnDetail").CustomSetActive(false);
			this.UpdateUI();
		}

		public void InitUI()
		{
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CPlayerInfoSystem.sPlayerInfoFormPath);
			if (form == null)
			{
				return;
			}
			GameObject widget = form.GetWidget(2);
			CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(widget, "panelTop/MenuList");
			CUIListScript componetInChild2 = Utility.GetComponetInChild<CUIListScript>(widget, "panelTop/DropList/List");
			CUICommonSystem.InitMenuPanel(componetInChild.gameObject, CPlayerPvpInfoController.sMainTitles, 0, true);
			CUICommonSystem.InitMenuPanel(componetInChild2.gameObject, CPlayerPvpInfoController.sSubTitles, CPlayerPvpInfoController.CacheSubTab, true);
			Utility.FindChild(widget, "btnGraph").CustomSetActive(false);
			Utility.FindChild(widget, "btnDetail").CustomSetActive(true);
		}

		public void UpdateUI()
		{
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CPlayerInfoSystem.sPlayerInfoFormPath);
			if (form == null)
			{
				return;
			}
			GameObject widget = form.GetWidget(2);
			CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(widget, "panelTop/MenuList");
			CUIListScript componetInChild2 = Utility.GetComponetInChild<CUIListScript>(widget, "panelTop/DropList/List");
			GameObject gameObject = Utility.FindChild(widget, "panelTop");
			GameObject gameObject2 = Utility.FindChild(widget, "panelLeft");
			GameObject gameObject3 = Utility.FindChild(widget, "panelLeftGraph");
			GameObject p = Utility.FindChild(widget, "panelRight");
			int selectedIndex = componetInChild.GetSelectedIndex();
			int selectedIndex2 = componetInChild2.GetSelectedIndex();
			if (selectedIndex < 0 || selectedIndex2 < 0 || selectedIndex >= Enum.GetValues(typeof(MainTab)).get_Length() || selectedIndex2 >= Enum.GetValues(typeof(SubTab)).get_Length())
			{
				return;
			}
			CPlayerPvpInfoController.CacheSubTab = selectedIndex2;
			this.InitBaseValue();
			MainTab mainTab = (MainTab)selectedIndex;
			SubTab subTab = (SubTab)selectedIndex2;
			Utility.GetComponetInChild<Text>(widget, "panelTop/DropList/Button_Down/Text").set_text(CPlayerPvpInfoController.sSubTitles[(int)subTab]);
			float num = 0f;
			float num2 = 0f;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			float num7 = 0f;
			uint num8 = 0u;
			uint num9 = 0u;
			uint num10 = 0u;
			uint num11 = 0u;
			uint num12 = 0u;
			uint num13 = 0u;
			int num14 = 0;
			float num15 = 0f;
			CPlayerProfile profile = Singleton<CPlayerInfoSystem>.instance.GetProfile();
			CPlayerPvpInfoController.ConvertAverageData(profile, mainTab, subTab, out num, out num2, out num3, out num4, out num5, out num6, out num7);
			CPlayerPvpInfoController.ConvertBaseData(profile, mainTab, subTab, out num8, out num9, out num10, out num11, out num12, out num13);
			CPlayerPvpInfoController.ConvertComplexData(profile, mainTab, subTab, out num14, out num15);
			CPlayerPvpInfoController.ConvertAbilityData(profile, mainTab, subTab, this.m_ability);
			for (int i = 0; i < this.m_ability.Length; i++)
			{
				this.m_ability[i] = Mathf.Clamp(this.m_ability[i], 0f, 1f);
			}
			if (subTab == SubTab.MatchAll || subTab == SubTab.Match5V5 || subTab == SubTab.MatchRank || subTab == SubTab.MatchGuild)
			{
				Utility.GetComponetInChild<Text>(gameObject2, "Content/txt1").set_text(num.ToString("F1"));
				Utility.GetComponetInChild<Text>(gameObject2, "Content/txt2").set_text(num2.ToString("P0"));
				Utility.GetComponetInChild<Text>(gameObject2, "Content/txt3").set_text(num3.ToString());
				Utility.GetComponetInChild<Text>(gameObject2, "Content/txt4").set_text(num4.ToString());
				Utility.GetComponetInChild<Text>(gameObject2, "Content/txt5").set_text(num5.ToString());
				Utility.GetComponetInChild<Text>(gameObject2, "Content/txt6").set_text(num6.ToString());
				Utility.GetComponetInChild<Text>(p, "DateBar3/txt9").set_text((this.m_ability[6] * 100f).ToString("F1"));
			}
			else
			{
				Utility.GetComponetInChild<Text>(gameObject2, "Content/txt1").set_text("-");
				Utility.GetComponetInChild<Text>(gameObject2, "Content/txt2").set_text("-");
				Utility.GetComponetInChild<Text>(gameObject2, "Content/txt3").set_text("-");
				Utility.GetComponetInChild<Text>(gameObject2, "Content/txt4").set_text("-");
				Utility.GetComponetInChild<Text>(gameObject2, "Content/txt5").set_text("-");
				Utility.GetComponetInChild<Text>(gameObject2, "Content/txt6").set_text("-");
				Utility.GetComponetInChild<Text>(p, "DateBar3/txt9").set_text("-");
			}
			if (subTab != SubTab.Match1V1)
			{
				Utility.GetComponetInChild<Text>(p, "txt1").set_text(num8.ToString());
				Utility.GetComponetInChild<Text>(p, "txt2").set_text(num9.ToString());
				Utility.GetComponetInChild<Text>(p, "txt3").set_text(num10.ToString());
				Utility.GetComponetInChild<Text>(p, "txt4").set_text(num11.ToString());
				Utility.GetComponetInChild<Text>(p, "txt5").set_text(num12.ToString());
				Utility.GetComponetInChild<Text>(p, "txt6").set_text(num13.ToString());
			}
			else
			{
				Utility.GetComponetInChild<Text>(p, "txt1").set_text("-");
				Utility.GetComponetInChild<Text>(p, "txt2").set_text("-");
				Utility.GetComponetInChild<Text>(p, "txt3").set_text("-");
				Utility.GetComponetInChild<Text>(p, "txt4").set_text("-");
				Utility.GetComponetInChild<Text>(p, "txt5").set_text("-");
				Utility.GetComponetInChild<Text>(p, "txt6").set_text("-");
			}
			Utility.GetComponetInChild<Text>(p, "DateBar1/txt7").set_text(num14.ToString());
			Utility.GetComponetInChild<Text>(p, "DateBar2/txt8").set_text((num15 == 1f) ? num15.ToString("P0") : num15.ToString("P1"));
			Utility.GetComponetInChild<Image>(p, "DateBar2/Bar").set_fillAmount(num15);
			Utility.GetComponetInChild<Image>(p, "DateBar3/Bar").set_fillAmount(CPlayerProfile.Divide(this.m_ability[6], 1f));
			this.InitTemVector();
			Vector3 a = Vector3.zero;
			Vector3[] array = new Vector3[6];
			for (int j = 1; j <= 6; j++)
			{
				GameObject gameObject4 = Utility.FindChild(widget, string.Format("panelLeftGraph/Content/p{0}", j));
				a = CPlayerPvpInfoController.sTemVector[j] - CPlayerPvpInfoController.sTemVector[0];
				array[j - 1] = CPlayerPvpInfoController.sTemVector[0] + a * this.m_ability[j - 1];
				gameObject4.transform.localPosition = array[j - 1];
			}
			CUIPolygon componetInChild3 = Utility.GetComponetInChild<CUIPolygon>(gameObject3, "Content/Polygon");
			componetInChild3.vertexs = array;
			componetInChild3.SetAllDirty();
			GameObject gameObject5 = Utility.FindChild(widget, "btnGraph");
			GameObject gameObject6 = Utility.FindChild(widget, "btnDetail");
			GameObject obj = Utility.FindChild(widget, "btnShare");
			gameObject2.CustomSetActive(gameObject5.activeSelf);
			gameObject3.CustomSetActive(gameObject6.activeSelf);
			obj.CustomSetActive(CPlayerInfoSystem.isSelf(Singleton<CPlayerInfoSystem>.instance.GetProfile().m_uuid));
			componetInChild2.gameObject.CustomSetActive(false);
			if (CSysDynamicBlock.bLobbyEntryBlocked)
			{
				CUICommonSystem.SetObjActive(form.transform.FindChild("pnlBg/pnlBody/pnlPvpInfo/btnShare"), false);
			}
		}

		private void InitTemVector()
		{
			if (CPlayerPvpInfoController.sTemVector != null)
			{
				return;
			}
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CPlayerInfoSystem.sPlayerInfoFormPath);
			if (form == null)
			{
				return;
			}
			GameObject widget = form.GetWidget(2);
			CPlayerPvpInfoController.sTemVector = new Vector3[7];
			for (int i = 0; i <= 6; i++)
			{
				GameObject gameObject = Utility.FindChild(widget, string.Format("panelLeftGraph/Content/t{0}", i));
				CPlayerPvpInfoController.sTemVector[i] = gameObject.transform.localPosition;
			}
		}

		private void InitBaseValue()
		{
			if (this.m_initedBaseValue)
			{
				return;
			}
			ResGlobalInfo resGlobalInfo = null;
			GameDataMgr.svr2CltCfgDict.TryGetValue(16u, out resGlobalInfo);
			if (resGlobalInfo == null)
			{
				return;
			}
			this.m_baseValueKDA = resGlobalInfo.dwConfValue;
			GameDataMgr.svr2CltCfgDict.TryGetValue(17u, out resGlobalInfo);
			if (resGlobalInfo == null)
			{
				return;
			}
			this.m_baseValueGold = resGlobalInfo.dwConfValue;
			GameDataMgr.svr2CltCfgDict.TryGetValue(18u, out resGlobalInfo);
			if (resGlobalInfo == null)
			{
				return;
			}
			this.m_baseValueBattle = resGlobalInfo.dwConfValue;
			GameDataMgr.svr2CltCfgDict.TryGetValue(19u, out resGlobalInfo);
			if (resGlobalInfo == null)
			{
				return;
			}
			this.m_baseValueAlive = resGlobalInfo.dwConfValue;
			GameDataMgr.svr2CltCfgDict.TryGetValue(20u, out resGlobalInfo);
			if (resGlobalInfo == null)
			{
				return;
			}
			this.m_baseValueHurtHero = resGlobalInfo.dwConfValue;
			this.m_initedBaseValue = true;
		}

		private static void ConvertBaseData(CPlayerProfile profile, MainTab mainTab, SubTab subTab, out uint winMvp, out uint loseMvp, out uint godLike, out uint tripleKill, out uint quataryKill, out uint pentaKill)
		{
			winMvp = 0u;
			loseMvp = 0u;
			godLike = 0u;
			tripleKill = 0u;
			quataryKill = 0u;
			pentaKill = 0u;
			if (mainTab == MainTab.MatchAll)
			{
				if (subTab == SubTab.MatchAll)
				{
					winMvp = profile.MVPCnt();
					loseMvp = profile.LoseSoulCnt();
					godLike = profile.HolyShit();
					tripleKill = profile.TripleKill();
					quataryKill = profile.QuataryKill();
					pentaKill = profile.PentaKill();
				}
				else
				{
					COMDT_HERO_STATISTIC_INFO cOMDT_HERO_STATISTIC_INFO = null;
					if (subTab == SubTab.Match5V5)
					{
						cOMDT_HERO_STATISTIC_INFO = profile.pvpExtraDetail.st5v5;
					}
					else if (subTab == SubTab.MatchRank)
					{
						cOMDT_HERO_STATISTIC_INFO = profile.pvpExtraDetail.stLadder;
					}
					else if (subTab == SubTab.MatchGuild)
					{
						cOMDT_HERO_STATISTIC_INFO = profile.pvpExtraDetail.stGuildMatch;
					}
					else if (subTab == SubTab.Match3V3)
					{
						cOMDT_HERO_STATISTIC_INFO = profile.pvpExtraDetail.st3v3;
					}
					else if (subTab == SubTab.Match1V1)
					{
						cOMDT_HERO_STATISTIC_INFO = profile.pvpExtraDetail.st1v1;
					}
					else if (subTab == SubTab.MatchEntertainment)
					{
						cOMDT_HERO_STATISTIC_INFO = profile.pvpExtraDetail.stEntertainment;
					}
					winMvp = cOMDT_HERO_STATISTIC_INFO.dwMvp;
					loseMvp = cOMDT_HERO_STATISTIC_INFO.dwLoseSoul;
					godLike = cOMDT_HERO_STATISTIC_INFO.dwGodLike;
					tripleKill = cOMDT_HERO_STATISTIC_INFO.dwTripleKill;
					quataryKill = cOMDT_HERO_STATISTIC_INFO.dwUltraKill;
					pentaKill = cOMDT_HERO_STATISTIC_INFO.dwRampage;
				}
			}
			else if (mainTab == MainTab.Match100)
			{
				byte b = 0;
				byte b2 = 0;
				CPlayerPvpInfoController.GetGameTypeAcntNum(subTab, out b, out b2);
				int num = 0;
				while ((long)num < (long)((ulong)profile.pvpExtraDetail.dwRecentNum))
				{
					if (b == 0 || (profile.pvpExtraDetail.astRecentDetail[num].bGameType == b && profile.pvpExtraDetail.astRecentDetail[num].bMapAcntNum == b2))
					{
						winMvp += profile.pvpExtraDetail.astRecentDetail[num].dwMvp;
						loseMvp += profile.pvpExtraDetail.astRecentDetail[num].dwLoseSoul;
						godLike += profile.pvpExtraDetail.astRecentDetail[num].dwGodLike;
						tripleKill += profile.pvpExtraDetail.astRecentDetail[num].dwTripleKill;
						quataryKill += profile.pvpExtraDetail.astRecentDetail[num].dwUltraKill;
						pentaKill += profile.pvpExtraDetail.astRecentDetail[num].dwRampage;
					}
					num++;
				}
			}
		}

		private static void ConvertAverageData(CPlayerProfile profile, MainTab mainTab, SubTab subTab, out float kda, out float joinFight, out int totalCoinGet, out int totalHurtToEnemy, out int totalBeHurt, out int totalHurtToOrgan, out float totalDead)
		{
			kda = 0f;
			joinFight = 0f;
			totalCoinGet = 0;
			totalHurtToEnemy = 0;
			totalBeHurt = 0;
			totalHurtToOrgan = 0;
			totalDead = 0f;
			if (mainTab == MainTab.MatchAll)
			{
				if (subTab == SubTab.Match5V5 || subTab == SubTab.MatchRank || subTab == SubTab.MatchGuild)
				{
					COMDT_HERO_STATISTIC_INFO cOMDT_HERO_STATISTIC_INFO = null;
					if (subTab == SubTab.Match5V5)
					{
						cOMDT_HERO_STATISTIC_INFO = profile.pvpExtraDetail.st5v5;
					}
					else if (subTab == SubTab.MatchRank)
					{
						cOMDT_HERO_STATISTIC_INFO = profile.pvpExtraDetail.stLadder;
					}
					else if (subTab == SubTab.MatchGuild)
					{
						cOMDT_HERO_STATISTIC_INFO = profile.pvpExtraDetail.stGuildMatch;
					}
					uint b = cOMDT_HERO_STATISTIC_INFO.dwWinNum + cOMDT_HERO_STATISTIC_INFO.dwLoseNum;
					kda = CPlayerProfile.Divide(cOMDT_HERO_STATISTIC_INFO.ullKDAPct, b) / 100f;
					joinFight = CPlayerProfile.Divide(cOMDT_HERO_STATISTIC_INFO.dwBattleRatioPct, b) / 100f;
					totalCoinGet = (int)CPlayerProfile.Divide(cOMDT_HERO_STATISTIC_INFO.dwGPM, b);
					totalHurtToEnemy = (int)CPlayerProfile.Divide(cOMDT_HERO_STATISTIC_INFO.dwHurtPM, b);
					totalBeHurt = (int)CPlayerProfile.Divide(cOMDT_HERO_STATISTIC_INFO.ullTotalBeHurt, b);
					totalHurtToOrgan = (int)CPlayerProfile.Divide(cOMDT_HERO_STATISTIC_INFO.ullTotalHurtOrgan, b);
					totalDead = CPlayerProfile.Divide(cOMDT_HERO_STATISTIC_INFO.dwDead, b);
				}
				else if (subTab == SubTab.MatchAll)
				{
					uint num = profile.pvpExtraDetail.st5v5.dwWinNum + profile.pvpExtraDetail.st5v5.dwLoseNum;
					ulong ullKDAPct = profile.pvpExtraDetail.st5v5.ullKDAPct;
					uint dwBattleRatioPct = profile.pvpExtraDetail.st5v5.dwBattleRatioPct;
					uint dwGPM = profile.pvpExtraDetail.st5v5.dwGPM;
					uint dwHurtPM = profile.pvpExtraDetail.st5v5.dwHurtPM;
					ulong ullTotalBeHurt = profile.pvpExtraDetail.st5v5.ullTotalBeHurt;
					ulong ullTotalHurtOrgan = profile.pvpExtraDetail.st5v5.ullTotalHurtOrgan;
					uint dwDead = profile.pvpExtraDetail.st5v5.dwDead;
					uint num2 = profile.pvpExtraDetail.stLadder.dwWinNum + profile.pvpExtraDetail.stLadder.dwLoseNum;
					ulong ullKDAPct2 = profile.pvpExtraDetail.stLadder.ullKDAPct;
					uint dwBattleRatioPct2 = profile.pvpExtraDetail.stLadder.dwBattleRatioPct;
					uint dwGPM2 = profile.pvpExtraDetail.stLadder.dwGPM;
					uint dwHurtPM2 = profile.pvpExtraDetail.stLadder.dwHurtPM;
					ulong ullTotalBeHurt2 = profile.pvpExtraDetail.stLadder.ullTotalBeHurt;
					ulong ullTotalHurtOrgan2 = profile.pvpExtraDetail.stLadder.ullTotalHurtOrgan;
					uint dwDead2 = profile.pvpExtraDetail.stLadder.dwDead;
					uint num3 = profile.pvpExtraDetail.stGuildMatch.dwWinNum + profile.pvpExtraDetail.stGuildMatch.dwLoseNum;
					ulong ullKDAPct3 = profile.pvpExtraDetail.stGuildMatch.ullKDAPct;
					uint dwBattleRatioPct3 = profile.pvpExtraDetail.stGuildMatch.dwBattleRatioPct;
					uint dwGPM3 = profile.pvpExtraDetail.stGuildMatch.dwGPM;
					uint dwHurtPM3 = profile.pvpExtraDetail.stGuildMatch.dwHurtPM;
					ulong ullTotalBeHurt3 = profile.pvpExtraDetail.stGuildMatch.ullTotalBeHurt;
					ulong ullTotalHurtOrgan3 = profile.pvpExtraDetail.stGuildMatch.ullTotalHurtOrgan;
					uint dwDead3 = profile.pvpExtraDetail.stGuildMatch.dwDead;
					uint num4 = num + num2 + num3;
					if (num4 == 0u)
					{
						num4 = 1u;
					}
					kda = CPlayerProfile.Divide(ullKDAPct + ullKDAPct2 + ullKDAPct3, num4) / 100f;
					joinFight = CPlayerProfile.Divide(dwBattleRatioPct + dwBattleRatioPct2 + dwBattleRatioPct3, num4) / 100f;
					totalCoinGet = (int)((dwGPM + dwGPM2 + dwGPM3) / num4);
					totalHurtToEnemy = (int)((dwHurtPM + dwHurtPM2 + dwHurtPM3) / num4);
					totalBeHurt = (int)((ullTotalBeHurt + ullTotalBeHurt2 + ullTotalBeHurt3) / (ulong)num4);
					totalHurtToOrgan = (int)((ullTotalHurtOrgan + ullTotalHurtOrgan2 + ullTotalHurtOrgan3) / (ulong)num4);
					totalDead = CPlayerProfile.Divide(dwDead + dwDead2 + dwDead3, num4);
				}
			}
			else if (mainTab == MainTab.Match100)
			{
				uint num5 = 0u;
				uint num6 = 0u;
				byte b2 = 0;
				byte b3 = 0;
				CPlayerPvpInfoController.GetGameTypeAcntNum(subTab, out b2, out b3);
				ulong num7 = 0uL;
				ulong num8 = 0uL;
				ulong num9 = 0uL;
				ulong num10 = 0uL;
				ulong num11 = 0uL;
				ulong num12 = 0uL;
				ulong num13 = 0uL;
				int num14 = 0;
				while ((long)num14 < (long)((ulong)profile.pvpExtraDetail.dwRecentNum))
				{
					if ((b2 == 0 || (profile.pvpExtraDetail.astRecentDetail[num14].bGameType == b2 && b3 == profile.pvpExtraDetail.astRecentDetail[num14].bMapAcntNum)) && (profile.pvpExtraDetail.astRecentDetail[num14].bGameType != 5 || profile.pvpExtraDetail.astRecentDetail[num14].bMapAcntNum >= 10) && profile.pvpExtraDetail.astRecentDetail[num14].bGameType != 9)
					{
						num7 += profile.pvpExtraDetail.astRecentDetail[num14].ullKDAPct;
						num8 += (ulong)profile.pvpExtraDetail.astRecentDetail[num14].dwBattleRatioPct;
						num6 += profile.pvpExtraDetail.astRecentDetail[num14].dwCampKill;
						num9 += (ulong)profile.pvpExtraDetail.astRecentDetail[num14].dwGPM;
						num10 += (ulong)profile.pvpExtraDetail.astRecentDetail[num14].dwHurtPM;
						num11 += profile.pvpExtraDetail.astRecentDetail[num14].ullTotalBeHurt;
						num12 += profile.pvpExtraDetail.astRecentDetail[num14].ullTotalHurtOrgan;
						num13 += (ulong)profile.pvpExtraDetail.astRecentDetail[num14].dwDead;
						num5 += 1u;
					}
					num14++;
				}
				kda = CPlayerProfile.Divide(num7, num5) / 100f;
				joinFight = CPlayerProfile.Divide(num8, num5) / 100f;
				totalCoinGet = (int)CPlayerProfile.Divide(num9, num5);
				totalHurtToEnemy = (int)CPlayerProfile.Divide(num10, num5);
				totalBeHurt = (int)CPlayerProfile.Divide(num11, num5);
				totalHurtToOrgan = (int)CPlayerProfile.Divide(num12, num5);
				totalDead = CPlayerProfile.Divide(num13, num5);
			}
		}

		private static void ConvertComplexData(CPlayerProfile profile, MainTab mainTab, SubTab subTab, out int gameCnt, out float gameWins)
		{
			int num = 0;
			gameCnt = 0;
			gameWins = 0f;
			if (mainTab == MainTab.MatchAll)
			{
				if (subTab == SubTab.MatchAll)
				{
					gameCnt = profile.Pvp1V1TotalGameCnt() + profile.Pvp3V3TotalGameCnt() + profile.Pvp5V5TotalGameCnt() + profile.EntertainmentTotalGameCnt() + profile.PvpGuildTotalGameCnt() + profile.RankTotalGameCnt();
					num = profile.Pvp1V1WinGameCnt() + profile.Pvp3V3WinGameCnt() + profile.Pvp5V5WinGameCnt() + profile.EntertainmentWinGameCnt() + profile.PvpGuildWinGameCnt() + profile.RankWinGameCnt();
					gameWins = CPlayerProfile.Divide((uint)num, (uint)gameCnt);
				}
				else if (subTab == SubTab.Match1V1)
				{
					gameCnt = profile.Pvp1V1TotalGameCnt();
					gameWins = profile.Pvp1V1Wins();
				}
				else if (subTab == SubTab.Match3V3)
				{
					gameCnt = profile.Pvp3V3TotalGameCnt();
					gameWins = profile.Pvp3V3Wins();
				}
				else if (subTab == SubTab.Match5V5)
				{
					gameCnt = profile.Pvp5V5TotalGameCnt();
					gameWins = profile.Pvp5V5Wins();
				}
				else if (subTab == SubTab.MatchEntertainment)
				{
					gameCnt = profile.EntertainmentTotalGameCnt();
					gameWins = profile.EntertainmentWins();
				}
				else if (subTab == SubTab.MatchGuild)
				{
					gameCnt = profile.PvpGuildTotalGameCnt();
					gameWins = profile.PvpGuildWins();
				}
				else if (subTab == SubTab.MatchRank)
				{
					gameCnt = profile.RankTotalGameCnt();
					gameWins = profile.RankWins();
				}
			}
			else if (mainTab == MainTab.Match100)
			{
				byte b = 0;
				byte b2 = 0;
				CPlayerPvpInfoController.GetGameTypeAcntNum(subTab, out b, out b2);
				int num2 = 0;
				while ((long)num2 < (long)((ulong)profile.pvpExtraDetail.dwRecentNum))
				{
					if (b == 0 || (profile.pvpExtraDetail.astRecentDetail[num2].bGameType == b && profile.pvpExtraDetail.astRecentDetail[num2].bMapAcntNum == b2))
					{
						num += (int)profile.pvpExtraDetail.astRecentDetail[num2].dwWinNum;
						gameCnt++;
					}
					num2++;
				}
				gameWins = CPlayerProfile.Divide((uint)num, (uint)gameCnt);
			}
		}

		private static void ConvertAbilityData(CPlayerProfile profile, MainTab mainTab, SubTab subTab, float[] ability)
		{
			for (int i = 0; i < ability.Length; i++)
			{
				ability[i] = 0f;
			}
			CSDT_STATISTIC_DATA_EXTRA_RADAR_INFO cSDT_STATISTIC_DATA_EXTRA_RADAR_INFO = null;
			if (profile.pvpAbilityDetail == null)
			{
				return;
			}
			if (mainTab == MainTab.MatchAll)
			{
				if (subTab == SubTab.MatchAll)
				{
					cSDT_STATISTIC_DATA_EXTRA_RADAR_INFO = profile.pvpAbilityDetail.stTotal;
				}
				else if (subTab == SubTab.Match5V5)
				{
					cSDT_STATISTIC_DATA_EXTRA_RADAR_INFO = profile.pvpAbilityDetail.stTotal5v5;
				}
				else if (subTab == SubTab.MatchRank)
				{
					cSDT_STATISTIC_DATA_EXTRA_RADAR_INFO = profile.pvpAbilityDetail.stTotalLadder;
				}
				else if (subTab == SubTab.MatchGuild)
				{
					cSDT_STATISTIC_DATA_EXTRA_RADAR_INFO = profile.pvpAbilityDetail.stTotalGuild;
				}
			}
			else if (mainTab == MainTab.Match100)
			{
				if (subTab == SubTab.MatchAll)
				{
					cSDT_STATISTIC_DATA_EXTRA_RADAR_INFO = profile.pvpAbilityDetail.stRecentTotal;
				}
				else if (subTab == SubTab.Match5V5)
				{
					cSDT_STATISTIC_DATA_EXTRA_RADAR_INFO = profile.pvpAbilityDetail.stRecent5v5;
				}
				else if (subTab == SubTab.MatchRank)
				{
					cSDT_STATISTIC_DATA_EXTRA_RADAR_INFO = profile.pvpAbilityDetail.stRecentLadder;
				}
				else if (subTab == SubTab.MatchGuild)
				{
					cSDT_STATISTIC_DATA_EXTRA_RADAR_INFO = profile.pvpAbilityDetail.stRecentGuild;
				}
			}
			if (cSDT_STATISTIC_DATA_EXTRA_RADAR_INFO == null)
			{
				return;
			}
			ability[0] = cSDT_STATISTIC_DATA_EXTRA_RADAR_INFO.dwKDA / 100f;
			ability[1] = cSDT_STATISTIC_DATA_EXTRA_RADAR_INFO.dwPush / 100f;
			ability[2] = cSDT_STATISTIC_DATA_EXTRA_RADAR_INFO.dwHurtHero / 100f;
			ability[3] = cSDT_STATISTIC_DATA_EXTRA_RADAR_INFO.dwGrow / 100f;
			ability[4] = cSDT_STATISTIC_DATA_EXTRA_RADAR_INFO.dwBattle / 100f;
			ability[5] = cSDT_STATISTIC_DATA_EXTRA_RADAR_INFO.dwSurvive / 100f;
			ability[6] = cSDT_STATISTIC_DATA_EXTRA_RADAR_INFO.dwTotal / 100f;
		}

		private static void GetGameTypeAcntNum(SubTab subTab, out byte gameType, out byte acntNum)
		{
			gameType = 255;
			acntNum = 0;
			if (subTab == SubTab.MatchAll)
			{
				gameType = 0;
				acntNum = 0;
			}
			else if (subTab == SubTab.Match5V5)
			{
				gameType = 5;
				acntNum = 10;
			}
			else if (subTab == SubTab.MatchRank)
			{
				gameType = 4;
				acntNum = 10;
			}
			else if (subTab == SubTab.MatchGuild)
			{
				gameType = 11;
				acntNum = 10;
			}
			else if (subTab == SubTab.Match3V3)
			{
				gameType = 5;
				acntNum = 6;
			}
			else if (subTab == SubTab.Match1V1)
			{
				gameType = 5;
				acntNum = 2;
			}
			else if (subTab == SubTab.MatchEntertainment)
			{
				gameType = 9;
				acntNum = 10;
			}
		}
	}
}
