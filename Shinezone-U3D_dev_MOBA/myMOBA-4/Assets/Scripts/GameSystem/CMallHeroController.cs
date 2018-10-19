using Assets.Scripts.UI;
using ResData;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class CMallHeroController
	{
		public const string MALL_HERO_OWN_FLAG_KEY = "Mall_Hero_Own_Flag_Key";

		private ListView<ResHeroCfgInfo> m_heroList = new ListView<ResHeroCfgInfo>();

		private enHeroJobType m_heroJobType;

		private bool m_notOwnFlag;

		public void Init()
		{
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_HeroItem_Enable, new CUIEventManager.OnUIEventHandler(this.OnHeroItemEnable));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_Hero_JobSelect, new CUIEventManager.OnUIEventHandler(this.OnHeroJobSelect));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_Appear, new CUIEventManager.OnUIEventHandler(this.OnMallAppear));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_Jump_Form, new CUIEventManager.OnUIEventHandler(this.OnMallJumpForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_Hero_Own_Flag_Change, new CUIEventManager.OnUIEventHandler(this.OnHeroOwnFlagChange));
			Singleton<EventRouter>.instance.AddEventHandler<uint>("HeroAdd", new Action<uint>(this.OnNtyAddHero));
			Singleton<EventRouter>.instance.AddEventHandler(EventID.Mall_Sort_Type_Changed, new Action(this.OnSortTypeChanged));
			this.m_notOwnFlag = (PlayerPrefs.GetInt("Mall_Hero_Own_Flag_Key", 0) != 0);
		}

		public void UnInit()
		{
		}

		public void Load(CUIFormScript form)
		{
			CUICommonSystem.LoadUIPrefab("UGUI/Form/System/Mall/BuyHero", "pnlBuyHero", form.GetWidget(3), form);
		}

		public bool Loaded(CUIFormScript form)
		{
			GameObject x = Utility.FindChild(form.GetWidget(3), "pnlBuyHero");
			return !(x == null);
		}

		public void Draw(CUIFormScript form)
		{
			if (form == null)
			{
				return;
			}
			GameObject gameObject = form.transform.Find("pnlBodyBg/pnlBuyHero").gameObject;
			if (gameObject != null)
			{
				gameObject.CustomSetActive(true);
				this.m_heroJobType = enHeroJobType.All;
				string text = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_All");
				string text2 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Tank");
				string text3 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Soldier");
				string text4 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Assassin");
				string text5 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Master");
				string text6 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Archer");
				string text7 = Singleton<CTextManager>.GetInstance().GetText("Hero_Job_Aid");
				string[] titleList = new string[]
				{
					text,
					text2,
					text3,
					text4,
					text5,
					text6,
					text7
				};
				Transform transform = gameObject.transform.Find("MenuList");
				this.RefreshHeroOwnFlag();
				CUICommonSystem.InitMenuPanel(transform.gameObject, titleList, (int)this.m_heroJobType, true);
			}
		}

		private void OnHeroItemEnable(CUIEvent uiEvent)
		{
			int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
			if (srcWidgetIndexInBelongedList < 0 || srcWidgetIndexInBelongedList >= this.m_heroList.Count)
			{
				return;
			}
			if (uiEvent.m_srcWidget == null)
			{
				return;
			}
			CMallItemWidget component = uiEvent.m_srcWidget.GetComponent<CMallItemWidget>();
			if (component == null)
			{
				return;
			}
			if (uiEvent.m_srcWidget != null)
			{
				CMallItem item = new CMallItem(this.m_heroList[srcWidgetIndexInBelongedList].dwCfgID, CMallItem.IconType.Normal);
				Singleton<CMallSystem>.GetInstance().SetMallItem(component, item);
			}
		}

		private void OnSortTypeChanged()
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(Singleton<CMallSystem>.GetInstance().sMallFormPath);
			if (form == null)
			{
				return;
			}
			if (Singleton<CMallSystem>.GetInstance().CurTab != CMallSystem.Tab.Hero)
			{
				return;
			}
			this.RefreshHeroListObject(form);
		}

		private void OnNtyAddHero(uint heroId)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(Singleton<CMallSystem>.GetInstance().sMallFormPath);
			if (form == null)
			{
				return;
			}
			this.RefreshHeroListObject(form);
		}

		private void OnMallJumpForm(CUIEvent uiEvent)
		{
			CUICommonSystem.JumpForm((RES_GAME_ENTRANCE_TYPE)uiEvent.m_eventParams.tag, 0, 0, null);
		}

		private void OnHeroJobSelect(CUIEvent uiEvent)
		{
			CUIListScript component = uiEvent.m_srcWidget.GetComponent<CUIListScript>();
			int selectedIndex = component.GetSelectedIndex();
			this.m_heroJobType = (enHeroJobType)selectedIndex;
			this.RefreshHeroListObject(uiEvent.m_srcFormScript);
		}

		private void OnHeroOwnFlagChange(CUIEvent uiEvent)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(Singleton<CMallSystem>.GetInstance().sMallFormPath);
			if (form == null)
			{
				return;
			}
			GameObject gameObject = Utility.FindChild(form.GetWidget(3), "pnlBuyHero/ownFlag");
			if (gameObject != null)
			{
				this.m_notOwnFlag = gameObject.GetComponent<Toggle>().isOn;
				PlayerPrefs.SetInt("Mall_Hero_Own_Flag_Key", (!this.m_notOwnFlag) ? 0 : 1);
				PlayerPrefs.Save();
			}
			this.RefreshHeroOwnFlag();
			this.RefreshHeroListObject(form);
		}

		private void OnMallAppear(CUIEvent uiEvent)
		{
			this.RefreshHeroListObject(uiEvent.m_srcFormScript);
		}

		private void RefreshHeroOwnFlag()
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(Singleton<CMallSystem>.GetInstance().sMallFormPath);
			if (form == null)
			{
				return;
			}
			GameObject gameObject = Utility.FindChild(form.GetWidget(3), "pnlBuyHero/ownFlag");
			if (gameObject != null)
			{
				gameObject.GetComponent<Toggle>().isOn = this.m_notOwnFlag;
			}
		}

		private void RefreshHeroListObject(CUIFormScript form)
		{
			if (form == null || Singleton<CMallSystem>.GetInstance().CurTab != CMallSystem.Tab.Hero)
			{
				return;
			}
			this.ResetHeroList();
			this.SortHeroList();
			CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(form.gameObject, "pnlBodyBg/pnlBuyHero/List");
			if (componetInChild == null)
			{
				return;
			}
			componetInChild.SetElementAmount(this.m_heroList.Count);
		}

		private void ResetHeroList()
		{
			this.m_heroList.Clear();
			ListView<ResHeroCfgInfo> allHeroListAtShop = CHeroDataFactory.GetAllHeroListAtShop();
			for (int i = 0; i < allHeroListAtShop.Count; i++)
			{
				CMallItem cMallItem = new CMallItem(allHeroListAtShop[i].dwCfgID, CMallItem.IconType.Normal);
				if (this.m_heroJobType == enHeroJobType.All || allHeroListAtShop[i].bMainJob == (byte)this.m_heroJobType || allHeroListAtShop[i].bMinorJob == (byte)this.m_heroJobType)
				{
					if (this.m_notOwnFlag)
					{
						if (!cMallItem.Owned(false))
						{
							this.m_heroList.Add(allHeroListAtShop[i]);
						}
					}
					else
					{
						this.m_heroList.Add(allHeroListAtShop[i]);
					}
				}
			}
		}

		private void SortHeroList()
		{
			if (this.m_heroList == null)
			{
				return;
			}
			this.m_heroList.Sort(CMallSortHelper.CreateHeroSorter());
			if (CMallSortHelper.CreateHeroSorter().IsDesc())
			{
				this.m_heroList.Reverse();
			}
		}

		public int GetHeroIndexByConfigId(uint heroID = 0u)
		{
			if (heroID == 0u)
			{
				return 0;
			}
			for (int i = 0; i < this.m_heroList.Count; i++)
			{
				if (this.m_heroList[i] != null && this.m_heroList[i].dwCfgID == heroID)
				{
					return i;
				}
			}
			return 0;
		}

		public IHeroData GetHeroDataByIndex(int index)
		{
			if (index >= 0 && this.m_heroList != null && this.m_heroList[index] != null && index < this.m_heroList.Count)
			{
				return CHeroDataFactory.CreateHeroData(this.m_heroList[index].dwCfgID);
			}
			return null;
		}

		public int GetHeroListCount()
		{
			if (this.m_heroList == null)
			{
				return 0;
			}
			return this.m_heroList.Count;
		}
	}
}
