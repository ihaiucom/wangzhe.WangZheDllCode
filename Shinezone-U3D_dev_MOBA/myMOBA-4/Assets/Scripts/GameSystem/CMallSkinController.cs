using Assets.Scripts.Framework;
using Assets.Scripts.UI;
using ResData;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class CMallSkinController
	{
		public const string MALL_SKIN_OWN_FLAG_KEY = "Mall_Skin_Own_Flag_Key";

		private ListView<ResHeroSkin> m_skinList = new ListView<ResHeroSkin>();

		private enHeroJobType m_heroJobType;

		private bool m_notOwnFlag;

		public void Init()
		{
			CSkinInfo.InitHeroSkinDicData();
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_SkinItem_Enable, new CUIEventManager.OnUIEventHandler(this.OnSkinItemEnable));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_Skin_JobSelect, new CUIEventManager.OnUIEventHandler(this.OnSkinJobSelect));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_Appear, new CUIEventManager.OnUIEventHandler(this.OnMallAppear));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Mall_Skin_Own_Flag_Change, new CUIEventManager.OnUIEventHandler(this.OnSkinOwnFlagChange));
			Singleton<EventRouter>.instance.AddEventHandler<uint>("HeroAdd", new Action<uint>(this.OnNtyHeroInfoChange));
			Singleton<EventRouter>.instance.AddEventHandler<uint, uint, uint>("HeroSkinAdd", new Action<uint, uint, uint>(this.OnNtyHeroInfoChangeBySkinAdd));
			Singleton<EventRouter>.instance.AddEventHandler(EventID.Mall_Sort_Type_Changed, new Action(this.OnSortTypeChanged));
			Singleton<EventRouter>.instance.AddEventHandler(EventID.SERVER_SKIN_DATABIN_READY, new Action(this.OnServerSkinDatabinReady));
			this.m_notOwnFlag = (PlayerPrefs.GetInt("Mall_Skin_Own_Flag_Key", 0) != 0);
		}

		public void UnInit()
		{
		}

		private void OnServerSkinDatabinReady()
		{
			CSkinInfo.InitHeroSkinDicData();
		}

		public void Load(CUIFormScript form)
		{
			CUICommonSystem.LoadUIPrefab("UGUI/Form/System/Mall/BuySkin", "pnlBuySkin", form.GetWidget(3), form);
		}

		public bool Loaded(CUIFormScript form)
		{
			GameObject x = Utility.FindChild(form.GetWidget(3), "pnlBuySkin");
			return !(x == null);
		}

		public void Draw(CUIFormScript form)
		{
			if (form == null)
			{
				return;
			}
			GameObject gameObject = form.transform.Find("pnlBodyBg/pnlBuySkin").gameObject;
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
				this.RefreshSkinOwnFlag();
				CUICommonSystem.InitMenuPanel(transform.gameObject, titleList, (int)this.m_heroJobType, true);
			}
		}

		private void OnNtyHeroInfoChange(uint heroId)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(Singleton<CMallSystem>.GetInstance().sMallFormPath);
			if (form == null)
			{
				return;
			}
			this.RefreshSkinListObject(form);
		}

		private void OnNtyHeroInfoChangeBySkinAdd(uint heroId, uint skinId, uint addReason)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(Singleton<CMallSystem>.GetInstance().sMallFormPath);
			if (form == null)
			{
				return;
			}
			this.OnNtyHeroInfoChange(heroId);
		}

		private void OnSortTypeChanged()
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(Singleton<CMallSystem>.GetInstance().sMallFormPath);
			if (form == null)
			{
				return;
			}
			if (Singleton<CMallSystem>.GetInstance().CurTab != CMallSystem.Tab.Skin)
			{
				return;
			}
			this.RefreshSkinListObject(form);
		}

		private void OnSkinItemEnable(CUIEvent uiEvent)
		{
			int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
			if (srcWidgetIndexInBelongedList < 0 || srcWidgetIndexInBelongedList >= this.m_skinList.Count)
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
				CMallItem item = new CMallItem(this.m_skinList[srcWidgetIndexInBelongedList].dwHeroID, this.m_skinList[srcWidgetIndexInBelongedList].dwSkinID, CMallItem.IconType.Normal);
				Singleton<CMallSystem>.GetInstance().SetMallItem(component, item);
			}
		}

		private void OnSkinJobSelect(CUIEvent uiEvent)
		{
			CUIListScript component = uiEvent.m_srcWidget.GetComponent<CUIListScript>();
			int selectedIndex = component.GetSelectedIndex();
			this.m_heroJobType = (enHeroJobType)selectedIndex;
			this.RefreshSkinListObject(uiEvent.m_srcFormScript);
		}

		private void OnSkinOwnFlagChange(CUIEvent uiEvent)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(Singleton<CMallSystem>.GetInstance().sMallFormPath);
			if (form == null)
			{
				return;
			}
			GameObject gameObject = Utility.FindChild(form.GetWidget(3), "pnlBuySkin/ownFlag");
			if (gameObject != null)
			{
				this.m_notOwnFlag = gameObject.GetComponent<Toggle>().isOn;
				PlayerPrefs.SetInt("Mall_Skin_Own_Flag_Key", (!this.m_notOwnFlag) ? 0 : 1);
				PlayerPrefs.Save();
			}
			this.RefreshSkinOwnFlag();
			this.RefreshSkinListObject(form);
		}

		private void OnMallAppear(CUIEvent uiEvent)
		{
			this.RefreshSkinListObject(uiEvent.m_srcFormScript);
		}

		private void RefreshSkinOwnFlag()
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(Singleton<CMallSystem>.GetInstance().sMallFormPath);
			if (form == null)
			{
				return;
			}
			GameObject gameObject = Utility.FindChild(form.GetWidget(3), "pnlBuySkin/ownFlag");
			if (gameObject != null)
			{
				gameObject.GetComponent<Toggle>().isOn = this.m_notOwnFlag;
			}
		}

		private void RefreshSkinListObject(CUIFormScript form)
		{
			if (form == null || !Singleton<CMallSystem>.GetInstance().m_IsMallFormOpen || Singleton<CMallSystem>.GetInstance().CurTab != CMallSystem.Tab.Skin)
			{
				return;
			}
			Transform transform = form.transform.Find("pnlBodyBg/pnlBuySkin");
			if (transform == null)
			{
				return;
			}
			this.ResetSkinList();
			this.SortSkinList();
			GameObject gameObject = transform.Find("List").gameObject;
			CUIListScript component = gameObject.GetComponent<CUIListScript>();
			component.SetElementAmount(this.m_skinList.Count);
		}

		private void ResetSkinList()
		{
			this.m_skinList.Clear();
			Dictionary<long, object>.Enumerator enumerator = GameDataMgr.heroSkinDatabin.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<long, object> current = enumerator.Current;
				ResHeroSkin resHeroSkin = current.Value as ResHeroSkin;
				if (resHeroSkin != null && resHeroSkin.dwSkinID != 0u && GameDataMgr.IsSkinAvailableAtShop(resHeroSkin.dwID))
				{
					ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(resHeroSkin.dwHeroID);
					if (dataByKey != null && GameDataMgr.IsHeroAvailable(dataByKey.dwCfgID))
					{
						CMallItem cMallItem = new CMallItem(resHeroSkin.dwHeroID, resHeroSkin.dwSkinID, CMallItem.IconType.Normal);
						if (this.m_heroJobType == enHeroJobType.All || dataByKey.bMainJob == (byte)this.m_heroJobType || dataByKey.bMinorJob == (byte)this.m_heroJobType)
						{
							if (this.m_notOwnFlag)
							{
								if (!cMallItem.Owned(false))
								{
									this.m_skinList.Add(resHeroSkin);
								}
							}
							else
							{
								this.m_skinList.Add(resHeroSkin);
							}
						}
					}
				}
			}
		}

		private void SortSkinList()
		{
			if (this.m_skinList == null)
			{
				return;
			}
			this.m_skinList.Sort(CMallSortHelper.CreateSkinSorter());
			if (CMallSortHelper.CreateSkinSorter().IsDesc())
			{
				this.m_skinList.Reverse();
			}
		}

		public int GetSkinIndexByConfigId(uint uniSkinID = 0u)
		{
			if (uniSkinID == 0u)
			{
				return -1;
			}
			for (int i = 0; i < this.m_skinList.Count; i++)
			{
				if (this.m_skinList[i] != null && this.m_skinList[i].dwID == uniSkinID)
				{
					return i;
				}
			}
			return -1;
		}

		public ResHeroSkin GetSkinDataByIndex(int index)
		{
			if (index >= 0 && this.m_skinList != null && this.m_skinList[index] != null && index < this.m_skinList.Count)
			{
				return this.m_skinList[index];
			}
			return null;
		}

		public int GetSkinListCount()
		{
			if (this.m_skinList == null)
			{
				return 0;
			}
			return this.m_skinList.Count;
		}
	}
}
