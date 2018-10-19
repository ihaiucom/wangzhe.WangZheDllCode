using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	[MessageHandlerClass]
	public class CQualifyingSystem : Singleton<CQualifyingSystem>
	{
		public static string s_qualifyingFormPath = "UGUI/Form/System/Qualifying/Form_Qualifying.prefab";

		public int m_menuIndex;

		public int m_areaIndex;

		public SCPKG_CLASSOFRANKDETAIL_NTF m_rankBaseInfo;

		public ListView<CSDT_CLASSOFRANKDETAIL> m_rankList = new ListView<CSDT_CLASSOFRANKDETAIL>();

		public override void Init()
		{
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Qualifying_OpenForm, new CUIEventManager.OnUIEventHandler(this.Qualifying_OpenForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Qualifying_MenuSelect, new CUIEventManager.OnUIEventHandler(this.Qualifying_MenuSelect));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Qualifying_BattleAreaBtnDown, new CUIEventManager.OnUIEventHandler(this.Qualifying_BattleAreaBtnDown));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Qualifying_BattleAreaBtnUp, new CUIEventManager.OnUIEventHandler(this.Qualifying_BattleAreaBtnUp));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Qualifying_RankListSelect, new CUIEventManager.OnUIEventHandler(this.Qualifying_RankListSelect));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Qualifying_RankListElementInit, new CUIEventManager.OnUIEventHandler(this.Qualifying_RankListElementInit));
		}

		private void Qualifying_OpenForm(CUIEvent uiEvent)
		{
			Singleton<CUIManager>.GetInstance().OpenForm(CQualifyingSystem.s_qualifyingFormPath, false, true);
			this.InitMenu();
			this.RefreshForm();
		}

		private void Qualifying_MenuSelect(CUIEvent uiEvent)
		{
			this.m_menuIndex = uiEvent.m_srcWidget.GetComponent<CUIListScript>().GetSelectedIndex();
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CQualifyingSystem.s_qualifyingFormPath);
			GameObject gameObject = form.gameObject;
			GameObject gameObject2 = gameObject.transform.Find("Panel/Panel_HeroInfo").gameObject;
			GameObject gameObject3 = gameObject.transform.Find("Panel/Panel_RankInfo").gameObject;
			gameObject2.CustomSetActive(false);
			gameObject3.CustomSetActive(false);
			if (this.m_menuIndex == 0)
			{
				gameObject2.CustomSetActive(true);
				this.RefreshHeroInfo();
			}
			else if (this.m_menuIndex == 1)
			{
				CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
				if (masterRoleInfo.m_rankGrade == 0)
				{
					Singleton<CUIManager>.GetInstance().OpenTips("You have not rank info", false, 1.5f, null, new object[0]);
					return;
				}
				gameObject3.CustomSetActive(true);
				this.RefreshRankInfo();
			}
		}

		private void Qualifying_BattleAreaBtnDown(CUIEvent uiEvent)
		{
		}

		private void Qualifying_BattleAreaBtnUp(CUIEvent uiEvent)
		{
		}

		private void Qualifying_RankListElementInit(CUIEvent uiEvent)
		{
			int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
			GameObject srcWidget = uiEvent.m_srcWidget;
			Text component = srcWidget.transform.Find("Cell/lblContent1").GetComponent<Text>();
			Text component2 = srcWidget.transform.Find("Cell/lblContent2").GetComponent<Text>();
			Text component3 = srcWidget.transform.Find("Cell/lblContent3").GetComponent<Text>();
			Text component4 = srcWidget.transform.Find("Cell/lblContent4").GetComponent<Text>();
			CSDT_CLASSOFRANKDETAIL cSDT_CLASSOFRANKDETAIL = this.m_rankList[srcWidgetIndexInBelongedList];
			component.text = (srcWidgetIndexInBelongedList + 1).ToString();
			component2.text = StringHelper.UTF8BytesToString(ref cSDT_CLASSOFRANKDETAIL.stDetail.szAcntName);
			component3.text = cSDT_CLASSOFRANKDETAIL.stDetail.dwWinCnt.ToString();
			component4.text = cSDT_CLASSOFRANKDETAIL.stDetail.bScore.ToString();
		}

		private void Qualifying_RankListSelect(CUIEvent uiEvent)
		{
			int selectedIndex = uiEvent.m_srcWidget.gameObject.GetComponent<CUIListScript>().GetSelectedIndex();
			GameObject gameObject = uiEvent.m_srcFormScript.gameObject;
			GameObject gameObject2 = gameObject.transform.Find("Panel/Panel_RankInfo").gameObject;
			CUIListScript component = gameObject2.transform.Find("Panel_Left/ListHeroIno").GetComponent<CUIListScript>();
			CSDT_CLASSOFRANKDETAIL cSDT_CLASSOFRANKDETAIL = this.m_rankList[selectedIndex];
			component.SetElementAmount(cSDT_CLASSOFRANKDETAIL.stDetail.astCommonUseHero.Length);
			for (int i = 0; i < cSDT_CLASSOFRANKDETAIL.stDetail.astCommonUseHero.Length; i++)
			{
				GameObject gameObject3 = component.GetElemenet(i).gameObject;
				Image component2 = gameObject3.transform.Find("heroInfo/imgRank").GetComponent<Image>();
				Text component3 = gameObject3.transform.Find("heroInfo/lblRank").GetComponent<Text>();
				if (cSDT_CLASSOFRANKDETAIL.stDetail.astCommonUseHero[i].dwHeroId == 0u)
				{
					gameObject3.CustomSetActive(false);
				}
				else
				{
					component2.SetSprite(CUIUtility.s_Sprite_System_Qualifying_Dir + "ranking_icon" + cSDT_CLASSOFRANKDETAIL.stDetail.astCommonUseHero[i].bHeroProficiencyLv, uiEvent.m_srcFormScript, true, false, false, false);
					component3.text = cSDT_CLASSOFRANKDETAIL.stDetail.astCommonUseHero[i].dwHeroProficiency.ToString();
					gameObject3.CustomSetActive(true);
				}
			}
		}

		private void InitMenu()
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CQualifyingSystem.s_qualifyingFormPath);
			if (form == null)
			{
				return;
			}
			GameObject gameObject = form.gameObject.transform.Find("Panel/ListMenu").gameObject;
			CUIListScript component = gameObject.GetComponent<CUIListScript>();
			string[] array = new string[]
			{
				Singleton<CTextManager>.GetInstance().GetText("Qualifying_Menu1"),
				Singleton<CTextManager>.GetInstance().GetText("Qualifying_Menu2")
			};
			component.SetElementAmount(array.Length);
			for (int i = 0; i < component.m_elementAmount; i++)
			{
				CUIListElementScript elemenet = component.GetElemenet(i);
				Text component2 = elemenet.gameObject.transform.Find("Text").GetComponent<Text>();
				component2.text = array[i];
			}
			this.m_menuIndex = 0;
		}

		public void RefreshForm()
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CQualifyingSystem.s_qualifyingFormPath);
			if (form == null)
			{
				return;
			}
			GameObject gameObject = form.gameObject.transform.Find("Panel/ListMenu").gameObject;
			CUIListScript component = gameObject.GetComponent<CUIListScript>();
			component.m_alwaysDispatchSelectedChangeEvent = true;
			component.SelectElement(this.m_menuIndex, true);
			component.m_alwaysDispatchSelectedChangeEvent = false;
		}

		public void RefreshHeroInfo()
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CQualifyingSystem.s_qualifyingFormPath);
			if (form == null)
			{
				return;
			}
			ListView<IHeroData> hostHeroList = CHeroDataFactory.GetHostHeroList(true, CMallSortHelper.HeroViewSortType.Name);
			GameObject gameObject = form.gameObject.transform.Find("Panel/Panel_HeroInfo").gameObject;
			Text component = gameObject.transform.Find("lblProficiency").GetComponent<Text>();
			CUIListScript component2 = gameObject.transform.Find("ListHeroIno").GetComponent<CUIListScript>();
			component2.SetElementAmount(hostHeroList.Count);
			for (int i = 0; i < hostHeroList.Count; i++)
			{
				GameObject gameObject2 = component2.GetElemenet(i).gameObject;
				Image component3 = gameObject2.transform.Find("heroInfo/imgRank").GetComponent<Image>();
				Text component4 = gameObject2.transform.Find("heroInfo/lblRank").GetComponent<Text>();
				component3.SetSprite(CUIUtility.s_Sprite_System_Qualifying_Dir + "ranking_icon" + hostHeroList[i].proficiencyLV, form, true, false, false, false);
				component4.text = hostHeroList[i].proficiency.ToString();
			}
			uint num = 0u;
			for (int j = 0; j < hostHeroList.Count; j++)
			{
				num += hostHeroList[j].proficiency;
			}
			component.text = CUIUtility.StringReplace(Singleton<CTextManager>.GetInstance().GetText("Qualifying_Title0"), new string[]
			{
				num.ToString()
			});
		}

		public void RefreshRankInfo()
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CQualifyingSystem.s_qualifyingFormPath);
			if (form == null)
			{
				return;
			}
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			GameObject gameObject = form.gameObject.transform.Find("Panel/Panel_RankInfo").gameObject;
			Text component = gameObject.transform.Find("Panel_Top/lblName").GetComponent<Text>();
			Text component2 = gameObject.transform.Find("Panel_Top/lblContent1").GetComponent<Text>();
			Text component3 = gameObject.transform.Find("Panel_Top/lblContent2").GetComponent<Text>();
			Text component4 = gameObject.transform.Find("Panel_Top/lblContent3").GetComponent<Text>();
			Text component5 = gameObject.transform.Find("Panel_Top/lblContent4").GetComponent<Text>();
			Text component6 = gameObject.transform.Find("Panel_Center/lblName").GetComponent<Text>();
			CUIListScript component7 = gameObject.transform.Find("Panel_Center/List").GetComponent<CUIListScript>();
			component.text = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer().Name;
			ResRankGradeConf gradeDataByShowGrade = CLadderSystem.GetGradeDataByShowGrade((int)masterRoleInfo.m_rankGrade);
			component2.text = StringHelper.UTF8BytesToString(ref gradeDataByShowGrade.szGradeDesc);
			component3.text = this.m_rankBaseInfo.dwSelfTotalFightCnt.ToString();
			component4.text = this.m_rankBaseInfo.dwSelfFightWinCnt.ToString();
			component5.text = this.m_rankBaseInfo.dwSelfScore.ToString();
			component6.text = this.m_rankBaseInfo.dwClass.ToString() + " Area";
			this.m_rankList.Sort(new Comparison<CSDT_CLASSOFRANKDETAIL>(this.SortCompare));
			component7.SetElementAmount(this.m_rankList.Count);
			int index = 0;
			for (int i = 0; i < this.m_rankList.Count; i++)
			{
				if (this.m_rankList[i].stAcntUin.ullUid == masterRoleInfo.playerUllUID)
				{
					index = i;
					break;
				}
			}
			if (this.m_rankList.Count > 0)
			{
				component7.SelectElement(index, true);
				component7.MoveElementInScrollArea(index, true);
			}
		}

		private int SortCompare(CSDT_CLASSOFRANKDETAIL info1, CSDT_CLASSOFRANKDETAIL info2)
		{
			return (info2.stDetail.bScore <= info1.stDetail.bScore) ? 0 : 1;
		}

		[MessageHandler(2600)]
		public static void ReciveRankAreaInfo(CSPkg msg)
		{
			SCPKG_CLASSOFRANKDETAIL_NTF stClassOfRankDetailNtf = msg.stPkgData.stClassOfRankDetailNtf;
			if (Singleton<CQualifyingSystem>.GetInstance().m_rankBaseInfo != null && Singleton<CQualifyingSystem>.GetInstance().m_rankBaseInfo.bGrade != stClassOfRankDetailNtf.bGrade)
			{
				Singleton<CQualifyingSystem>.GetInstance().m_rankList.Clear();
			}
			Singleton<CQualifyingSystem>.GetInstance().m_rankBaseInfo = stClassOfRankDetailNtf;
			Singleton<CQualifyingSystem>.GetInstance().m_rankList.AddRange(stClassOfRankDetailNtf.astRecoed);
		}
	}
}
