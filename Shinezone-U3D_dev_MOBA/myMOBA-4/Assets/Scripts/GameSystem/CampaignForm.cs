using Assets.Scripts.UI;
using ResData;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class CampaignForm : ActivityForm
	{
		public enum DynamicAssets
		{
			ButtonBlueImage,
			ButtonYellowImage
		}

		public class ActivityMenuItem
		{
			public GameObject root;

			public Activity activity;

			public Text name;

			public Image flag;

			public Text flagText;

			public Image hotspot;

			public ActivityMenuItem(GameObject node, Activity actv)
			{
				this.root = node;
				this.activity = actv;
				this.name = Utility.GetComponetInChild<Text>(node, "Name");
				this.flag = Utility.GetComponetInChild<Image>(node, "Flag");
				this.flagText = Utility.GetComponetInChild<Text>(node, "Flag/Text");
				this.hotspot = Utility.GetComponetInChild<Image>(node, "Hotspot");
				this.activity.OnTimeStateChange += new Activity.ActivityEvent(this.OnStateChange);
				this.activity.OnMaskStateChange += new Activity.ActivityEvent(this.OnStateChange);
				this.Validate();
			}

			public void Clear()
			{
				this.activity.OnTimeStateChange -= new Activity.ActivityEvent(this.OnStateChange);
				this.activity.OnMaskStateChange -= new Activity.ActivityEvent(this.OnStateChange);
			}

			public void Validate()
			{
				this.name.text = this.activity.Name;
				RES_WEAL_COLORBAR_TYPE flagType = this.activity.FlagType;
				if (flagType != RES_WEAL_COLORBAR_TYPE.RES_WEAL_COLORBAR_TYPE_NULL)
				{
					this.flag.gameObject.CustomSetActive(true);
					string text = flagType.ToString();
					this.flag.SetSprite(CUIUtility.GetSpritePrefeb("UGUI/Sprite/Dynamic/Activity/" + text, false, false), false);
					this.flagText.text = Singleton<CTextManager>.GetInstance().GetText(text);
				}
				else
				{
					this.flag.gameObject.CustomSetActive(false);
				}
				this.hotspot.gameObject.CustomSetActive(this.activity.ReadyForDot);
			}

			private void OnStateChange(Activity actv)
			{
				DebugHelper.Assert(this.hotspot != null, "hotspot != null");
				if (this.hotspot != null)
				{
					this.hotspot.gameObject.SetActive(this.activity.ReadyForDot);
				}
			}
		}

		private class ActivityTabIndex
		{
			public int idx;

			public Activity m_Activity;
		}

		public static string s_formPath = CUIUtility.s_Form_Activity_Dir + "Form_Activity.prefab";

		private CUIFormScript _uiForm;

		private string[] m_strTitleList = new string[]
		{
			"精彩活动",
			"游戏公告"
		};

		private GameObject[] m_TitleListObj = new GameObject[2];

		private CUIListScript m_TitleMenuList;

		private CUIListScript _uiListMenu;

		private ListView<CampaignForm.ActivityMenuItem> _actvMenuList;

		private int _selectedIndex;

		private int _initTimer;

		private int _initStep;

		private GameObject _title;

		private Text _titleText;

		private Image _titleImage;

		private ScrollRect _viewScroll;

		private CampaignFormView _view;

		private static int G_DISPLAYCOLS = 5;

		private ListView<CampaignForm.ActivityTabIndex>[] m_AllSelectActivityMenuList = new ListView<CampaignForm.ActivityTabIndex>[CampaignForm.G_DISPLAYCOLS];

		private int m_nSelectActivityIndex;

		private int m_nUseActivityTabCount;

		private int[] m_ActivtyTypeToTabIdx = new int[CampaignForm.G_DISPLAYCOLS];

		public override CUIFormScript formScript
		{
			get
			{
				return this._uiForm;
			}
		}

		public CampaignForm(ActivitySys sys) : base(sys)
		{
			this._uiForm = null;
		}

		public Image GetDynamicImage(CampaignForm.DynamicAssets index)
		{
			return this._uiForm.GetWidget((int)index).GetComponent<Image>();
		}

		public GameObject GetIDIPRedObj()
		{
			if (this._uiForm == null)
			{
				return null;
			}
			if (this.m_TitleListObj == null)
			{
				return null;
			}
			int num = this.m_TitleListObj.Length;
			return this.m_TitleListObj[num - 1];
		}

		public void UpdatePandoraRedPoint()
		{
			if (this.m_TitleListObj == null)
			{
				return;
			}
			int num = this.m_TitleListObj.Length;
			for (int i = this.m_nUseActivityTabCount; i < num - 1; i++)
			{
				int idxTab = i - this.m_nUseActivityTabCount;
				int num2 = MonoSingleton<PandroaSys>.GetInstance().IsShowPandoraTabRedPointByTabIdx(PandroaSys.PandoraModuleType.action, idxTab);
				if (num2 > 0)
				{
					CUICommonSystem.AddRedDot(this.m_TitleListObj[i], enRedDotPos.enTopRight, num2, 0, 0);
				}
				else
				{
					CUICommonSystem.DelRedDot(this.m_TitleListObj[i]);
				}
			}
		}

		public override void Open()
		{
			if (null != this._uiForm)
			{
				return;
			}
			this.ClearActiveData();
			this._uiForm = Singleton<CUIManager>.GetInstance().OpenForm(CampaignForm.s_formPath, false, true);
			if (null == this._uiForm)
			{
				return;
			}
			Transform trParent = this._uiForm.gameObject.transform.Find("Panel/Panle_Pandora");
			MonoSingleton<PandroaSys>.GetInstance().InitPandoraTab(PandroaSys.PandoraModuleType.action, trParent);
			this._initTimer = Singleton<CTimerManager>.GetInstance().AddTimer(80, 4, new CTimer.OnTimeUpHandler(this.onInitTimer));
			this._initStep = 0;
			this.PandroaUpdateBtn();
		}

		public void PandroaUpdateBtn()
		{
			if (this._uiForm)
			{
				MonoSingleton<PandroaSys>.GetInstance().ShowActiveActBoxBtn(this._uiForm);
			}
		}

		private void onInitTimer(int seq)
		{
			switch (++this._initStep)
			{
			case 1:
				this.InitSelectActivtyMenuData();
				this._title = Utility.FindChild(this._uiForm.gameObject, "Panel/Title");
				this._titleText = Utility.GetComponetInChild<Text>(this._title, "Text");
				this._titleImage = Utility.GetComponetInChild<Image>(this._title, "Image");
				this._uiListMenu = Utility.GetComponetInChild<CUIListScript>(this._uiForm.gameObject, "Panel/Panle_Activity/Menu/List");
				this._viewScroll = Utility.GetComponetInChild<ScrollRect>(this._uiForm.gameObject, "Panel/Panle_Activity/ScrollRect");
				this._view = new CampaignFormView(Utility.FindChild(this._uiForm.gameObject, "Panel/Panle_Activity/ScrollRect/Content"), this, null);
				break;
			case 2:
			{
				this.m_TitleMenuList = Utility.GetComponetInChild<CUIListScript>(this._uiForm.gameObject, "Panel/TitleMenu/List");
				this.m_strTitleList = new string[this.m_nUseActivityTabCount];
				for (int i = 0; i < this.m_nUseActivityTabCount; i++)
				{
					if (i < 3)
					{
						this.m_strTitleList[i] = Singleton<ActivitySys>.GetInstance().m_ActivtyTabName[i];
					}
					else
					{
						this.m_strTitleList[i] = Singleton<ActivitySys>.GetInstance().m_ActivtyTabName[0];
					}
				}
				int num = MonoSingleton<PandroaSys>.GetInstance().PandoraTabCount(PandroaSys.PandoraModuleType.action);
				this.m_TitleMenuList.SetElementAmount(this.m_strTitleList.Length + num + 1);
				this.m_TitleListObj = new GameObject[this.m_strTitleList.Length + num + 1];
				for (int j = 0; j < this.m_strTitleList.Length; j++)
				{
					CUIListElementScript elemenet = this.m_TitleMenuList.GetElemenet(j);
					Text componetInChild = Utility.GetComponetInChild<Text>(elemenet.gameObject, "Text");
					if (componetInChild)
					{
						componetInChild.text = this.m_strTitleList[j];
					}
					this.m_TitleListObj[j] = elemenet.gameObject;
				}
				for (int k = 0; k < num; k++)
				{
					CUIListElementScript elemenet2 = this.m_TitleMenuList.GetElemenet(k + this.m_strTitleList.Length);
					Text componetInChild2 = Utility.GetComponetInChild<Text>(elemenet2.gameObject, "Text");
					if (componetInChild2)
					{
						componetInChild2.text = MonoSingleton<PandroaSys>.GetInstance().GetPandoraTabName(PandroaSys.PandoraModuleType.action, k);
					}
					this.m_TitleListObj[k + this.m_strTitleList.Length] = elemenet2.gameObject;
				}
				CUIListElementScript elemenet3 = this.m_TitleMenuList.GetElemenet(this.m_strTitleList.Length + num);
				Text componetInChild3 = Utility.GetComponetInChild<Text>(elemenet3.gameObject, "Text");
				if (componetInChild3)
				{
					componetInChild3.text = "游戏公告";
				}
				this.m_TitleListObj[this.m_strTitleList.Length + num] = elemenet3.gameObject;
				this.m_TitleMenuList.SelectElement(0, true);
				break;
			}
			case 3:
				this.UpdateBuildMenulistTabIdx(0);
				break;
			case 4:
			{
				int num2 = -1;
				bool flag = true;
				while (++num2 < this._actvMenuList.Count)
				{
					if ((flag && this._actvMenuList[num2].activity.ReadyForGet) || (!flag && this._actvMenuList[num2].activity.ReadyForDot))
					{
						break;
					}
					if (flag && num2 + 1 == this._actvMenuList.Count)
					{
						num2 = -1;
						flag = false;
					}
				}
				if (num2 >= this._actvMenuList.Count)
				{
					num2 = 0;
				}
				this._uiListMenu.SelectElement(num2, true);
				this.SelectMenuItem(num2);
				Singleton<ActivitySys>.GetInstance().OnStateChange += new ActivitySys.StateChangeDelegate(this.OnValidateActivityRedSpot);
				Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Activity_Select, new CUIEventManager.OnUIEventHandler(this.OnSelectActivity));
				Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Activity_Select_TitleMenu, new CUIEventManager.OnUIEventHandler(this.OnSelectTitleMenu));
				this.UpdateTitelRedDot();
				Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref this._initTimer);
				this._initStep = 0;
				Singleton<ActivitySys>.GetInstance().OnCampaignFormOpened();
				this.UpdatePandoraRedPoint();
				break;
			}
			}
		}

		private void OnValidateActivityRedSpot()
		{
			this.UpdateTitelRedDot();
		}

		private void UpdateTitelRedDot()
		{
			if (this._uiForm == null || this.m_TitleMenuList == null)
			{
				return;
			}
			for (int i = 0; i < this.m_nUseActivityTabCount; i++)
			{
				CUIListElementScript elemenet = this.m_TitleMenuList.GetElemenet(i);
				if (elemenet != null)
				{
					int num = 0;
					for (int j = 0; j < this.m_AllSelectActivityMenuList[i].Count; j++)
					{
						Activity activity = this.m_AllSelectActivityMenuList[i][j].m_Activity;
						if (activity != null && activity.ReadyForGet)
						{
							num++;
						}
					}
					if (num > 0)
					{
						CUICommonSystem.AddRedDot(elemenet.gameObject, enRedDotPos.enTopRight, num, 0, 0);
					}
					else
					{
						CUICommonSystem.DelRedDot(elemenet.gameObject);
					}
				}
			}
		}

		private void OnSelectTitleMenu(CUIEvent uiEvent)
		{
			this.SelectTitleMenu(uiEvent.m_srcWidgetIndexInBelongedList);
		}

		private void SelectTitleMenu(int tabIdx)
		{
			if (this._uiForm == null)
			{
				return;
			}
			if (tabIdx < this.m_nUseActivityTabCount)
			{
				this.UpdateBuildMenulistTabIdx(tabIdx);
				Transform transform = this._uiForm.gameObject.transform.Find("Panel/Panle_Activity");
				if (transform)
				{
					transform.gameObject.CustomSetActive(true);
				}
				Transform transform2 = this._uiForm.gameObject.transform.Find("Panel/Panle_IDIP");
				if (transform2)
				{
					transform2.gameObject.CustomSetActive(false);
				}
				Transform transform3 = this._uiForm.gameObject.transform.Find("Panel/Panle_Pandora");
				if (transform3)
				{
					transform3.gameObject.CustomSetActive(false);
				}
			}
			else if (tabIdx >= this.m_nUseActivityTabCount)
			{
				int num = MonoSingleton<PandroaSys>.GetInstance().PandoraTabCount(PandroaSys.PandoraModuleType.action);
				if (num > 0)
				{
					if (tabIdx >= this.m_nUseActivityTabCount && tabIdx < this.m_nUseActivityTabCount + num)
					{
						Transform transform4 = this._uiForm.gameObject.transform.Find("Panel/Panle_Activity");
						if (transform4)
						{
							transform4.gameObject.CustomSetActive(false);
						}
						Transform transform5 = this._uiForm.gameObject.transform.Find("Panel/Panle_IDIP");
						if (transform5)
						{
							transform5.gameObject.CustomSetActive(false);
						}
						Transform transform6 = this._uiForm.gameObject.transform.Find("Panel/Panle_Pandora");
						MonoSingleton<PandroaSys>.GetInstance().OnPandoraTabClick(PandroaSys.PandoraModuleType.action, tabIdx - this.m_nUseActivityTabCount);
						if (transform6)
						{
							transform6.gameObject.CustomSetActive(true);
						}
					}
					else if (tabIdx == this.m_nUseActivityTabCount + num)
					{
						this.ProcessIDIPTable();
					}
				}
				else
				{
					this.ProcessIDIPTable();
				}
			}
		}

		private void ProcessIDIPTable()
		{
			if (this._uiForm == null)
			{
				return;
			}
			Transform transform = this._uiForm.gameObject.transform.Find("Panel/Panle_Activity");
			if (transform)
			{
				transform.gameObject.CustomSetActive(false);
			}
			Transform transform2 = this._uiForm.gameObject.transform.Find("Panel/Panle_Pandora");
			if (transform2)
			{
				transform2.gameObject.CustomSetActive(false);
			}
			Transform transform3 = this._uiForm.gameObject.transform.Find("Panel/Panle_IDIP");
			if (transform3)
			{
				transform3.gameObject.CustomSetActive(true);
			}
			MonoSingleton<IDIPSys>.GetInstance().OnOpenIDIPForm(this._uiForm);
			CMiShuSystem.SendUIClickToServer(enUIClickReprotID.rp_OpenGongGao);
		}

		private void ClearActiveData()
		{
			for (int i = 0; i < this.m_AllSelectActivityMenuList.Length; i++)
			{
				this.m_AllSelectActivityMenuList[i] = new ListView<CampaignForm.ActivityTabIndex>();
			}
			this.m_nUseActivityTabCount = 0;
			this.m_nSelectActivityIndex = 0;
		}

		private void InitSelectActivtyMenuData()
		{
			for (int i = 0; i < this.m_AllSelectActivityMenuList.Length; i++)
			{
				this.m_AllSelectActivityMenuList[i] = new ListView<CampaignForm.ActivityTabIndex>();
			}
			this.m_nUseActivityTabCount = 0;
			this.m_nSelectActivityIndex = 0;
			ListView<Activity> activityList = base.Sys.GetActivityList((Activity actv) => actv.Entrance == RES_WEAL_ENTRANCE_TYPE.RES_WEAL_ENTRANCE_TYPE_ACTIVITY);
			activityList.Sort(delegate(Activity l, Activity r)
			{
				bool readyForGet = l.ReadyForGet;
				bool readyForGet2 = r.ReadyForGet;
				if (readyForGet != readyForGet2)
				{
					return (!readyForGet) ? 1 : -1;
				}
				bool completed = l.Completed;
				bool completed2 = r.Completed;
				if (completed != completed2)
				{
					return (!completed) ? -1 : 1;
				}
				if (l.FlagType != r.FlagType)
				{
					return r.FlagType - l.FlagType;
				}
				return (int)(l.Sequence - r.Sequence);
			});
			for (int j = 0; j < activityList.Count; j++)
			{
				Activity activity = activityList[j];
				int num = activity.GetTabID();
				if (num < 0 || num >= CampaignForm.G_DISPLAYCOLS)
				{
					num = 0;
				}
				CampaignForm.ActivityTabIndex activityTabIndex = new CampaignForm.ActivityTabIndex();
				activityTabIndex.idx = num;
				activityTabIndex.m_Activity = activity;
				this.m_AllSelectActivityMenuList[num].Add(activityTabIndex);
			}
			int num2 = 0;
			for (int k = 0; k < this.m_AllSelectActivityMenuList.Length; k++)
			{
				if (this.m_AllSelectActivityMenuList[k].Count > 0)
				{
					this.m_nUseActivityTabCount++;
					this.m_ActivtyTypeToTabIdx[num2] = this.m_AllSelectActivityMenuList[k][0].idx;
					num2++;
				}
			}
		}

		private void UpdateBuildMenulistTabIdx(int idx)
		{
			if (this._actvMenuList != null)
			{
				for (int i = 0; i < this._actvMenuList.Count; i++)
				{
					this._actvMenuList[i].Clear();
				}
				this._actvMenuList = null;
			}
			this._actvMenuList = new ListView<CampaignForm.ActivityMenuItem>();
			this._selectedIndex = -1;
			if (idx >= this.m_ActivtyTypeToTabIdx.Length)
			{
				return;
			}
			this.m_nSelectActivityIndex = this.m_ActivtyTypeToTabIdx[idx];
			if (this.m_nSelectActivityIndex >= this.m_AllSelectActivityMenuList.Length)
			{
				return;
			}
			this._uiListMenu.SetElementAmount(this.m_AllSelectActivityMenuList[this.m_nSelectActivityIndex].Count);
			for (int j = 0; j < this.m_AllSelectActivityMenuList[this.m_nSelectActivityIndex].Count; j++)
			{
				Activity activity = this.m_AllSelectActivityMenuList[this.m_nSelectActivityIndex][j].m_Activity;
				CUIListElementScript elemenet = this._uiListMenu.GetElemenet(j);
				CampaignForm.ActivityMenuItem item = new CampaignForm.ActivityMenuItem(elemenet.gameObject, activity);
				this._actvMenuList.Add(item);
			}
			this.SelectMenuItem(0);
			this._uiListMenu.SelectElement(0, true);
		}

		public override void Update()
		{
			if (this._view != null && this._uiForm && this._uiForm.gameObject && this._initTimer == 0)
			{
				this._view.Update();
			}
		}

		private void SelectMenuItem(int index)
		{
			if (index < 0 || index >= this._actvMenuList.Count)
			{
				this._titleImage.gameObject.CustomSetActive(false);
				this._titleText.gameObject.CustomSetActive(true);
				this._titleText.text = Singleton<CTextManager>.GetInstance().GetText("activityEmptyTitle");
				this._view.SetActivity(null);
				return;
			}
			if (index != this._selectedIndex)
			{
				this._selectedIndex = index;
				CampaignForm.ActivityMenuItem activityMenuItem = this._actvMenuList[this._selectedIndex];
				string title = activityMenuItem.activity.Title;
				if (string.IsNullOrEmpty(title))
				{
					this._title.CustomSetActive(false);
				}
				else
				{
					this._title.CustomSetActive(true);
					if (activityMenuItem.activity.IsImageTitle)
					{
						this._titleText.gameObject.CustomSetActive(false);
						this._titleImage.gameObject.CustomSetActive(true);
						this._titleImage.SetSprite(CUIUtility.GetSpritePrefeb(ActivitySys.SpriteRootDir + title, false, false), false);
						this._titleImage.SetNativeSize();
					}
					else
					{
						this._titleImage.gameObject.CustomSetActive(false);
						this._titleText.gameObject.CustomSetActive(true);
						this._titleText.text = title;
					}
				}
				this._view.SetActivity(activityMenuItem.activity);
				this._viewScroll.verticalNormalizedPosition = 1f;
				this.Update();
				activityMenuItem.activity.Visited = true;
			}
		}

		public void SelectActivity(Activity target)
		{
			if (target == null)
			{
				return;
			}
			if (null != this.m_TitleMenuList)
			{
				for (int i = 0; i < this.m_ActivtyTypeToTabIdx.Length; i++)
				{
					if (this.m_ActivtyTypeToTabIdx[i] == target.GetTabID())
					{
						this.SelectTitleMenu(i);
						this.m_TitleMenuList.SelectElement(i, false);
						break;
					}
				}
			}
			if (this._actvMenuList != null)
			{
				for (int j = 0; j < this._actvMenuList.Count; j++)
				{
					if (this._actvMenuList[j].activity == target)
					{
						this.SelectMenuItem(j);
						this._uiListMenu.SelectElement(j, true);
						break;
					}
				}
			}
		}

		private void OnSelectActivity(CUIEvent uiEvent)
		{
			this.SelectMenuItem(uiEvent.m_srcWidgetIndexInBelongedList);
			CUICommonSystem.CloseUseableTips();
		}

		public override void Close()
		{
			if (this._actvMenuList != null)
			{
				for (int i = 0; i < this._actvMenuList.Count; i++)
				{
					this._actvMenuList[i].Clear();
				}
				this._actvMenuList = null;
			}
			if (this._view != null)
			{
				this._view.Clear();
				this._view = null;
			}
			if (null != this._uiForm)
			{
				Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Activity_Select, new CUIEventManager.OnUIEventHandler(this.OnSelectActivity));
				Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Activity_Select_TitleMenu, new CUIEventManager.OnUIEventHandler(this.OnSelectTitleMenu));
				CUIFormScript uiForm = this._uiForm;
				this._uiForm = null;
				this._uiListMenu = null;
				this.m_TitleMenuList = null;
				this.m_TitleListObj = null;
				this.ClearActiveData();
				Singleton<CUIManager>.GetInstance().CloseForm(uiForm);
				MonoSingleton<NobeSys>.GetInstance().ShowDelayNobeLoseTipsInfo();
				MonoSingleton<PandroaSys>.GetInstance().ShowPopNews();
				MonoSingleton<PandroaSys>.GetInstance().ClosePandoraTabWindow(PandroaSys.PandoraModuleType.action);
			}
			MonoSingleton<IDIPSys>.GetInstance().OnCloseIDIPForm(null);
			Singleton<ActivitySys>.GetInstance().OnStateChange -= new ActivitySys.StateChangeDelegate(this.OnValidateActivityRedSpot);
			Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref this._initTimer);
		}
	}
}
