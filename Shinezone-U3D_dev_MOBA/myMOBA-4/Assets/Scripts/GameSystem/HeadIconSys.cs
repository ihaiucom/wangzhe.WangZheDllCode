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
	[MessageHandlerClass]
	public class HeadIconSys : Singleton<HeadIconSys>
	{
		private enum enHeadImgTab
		{
			All,
			Nobe,
			Activity,
			Skin,
			Team
		}

		public class HeadImgInfo
		{
			public uint dwID;

			public uint dwGetTime;

			public byte bNtfFlag;
		}

		public static readonly string s_headImgChgForm = CUIUtility.s_IDIP_Form_Dir + "Form_HeadChangeIcon";

		private DictionaryView<HeadIconSys.enHeadImgTab, ListView<ResHeadImage>> headImageDic = new DictionaryView<HeadIconSys.enHeadImgTab, ListView<ResHeadImage>>();

		private ListView<HeadIconSys.HeadImgInfo> m_headImgInfo = new ListView<HeadIconSys.HeadImgInfo>();

		public uint UnReadFlagNum
		{
			get
			{
				uint num = 0u;
				for (int i = 0; i < this.m_headImgInfo.Count; i++)
				{
					if (this.m_headImgInfo[i].bNtfFlag == 1)
					{
						num += 1u;
					}
				}
				return num;
			}
		}

		public override void Init()
		{
			base.Init();
			Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.HeadIcon_Change_Form_Open, new CUIEventManager.OnUIEventHandler(this.On_HeadIcon_Form_Open));
			Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.HeadIcon_Change_Form_Close, new CUIEventManager.OnUIEventHandler(this.On_HeadIcon_Change_Form_Close));
			Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.HeadIcon_Change_Tab_Click, new CUIEventManager.OnUIEventHandler(this.On_HeadIcon_Change_Tab_Click));
			Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.HeadIcon_Change_Icon_Click, new CUIEventManager.OnUIEventHandler(this.On_HeadIcon_Change_Icon_Click));
			Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.HeadIcon_Change_Confirm, new CUIEventManager.OnUIEventHandler(this.On_HeadIcon_Change_Confirm));
			Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.HeadIcon_Change_Item_Enable, new CUIEventManager.OnUIEventHandler(this.On_HeadIcon_Change_Item_Enable));
			Singleton<EventRouter>.instance.AddEventHandler(EventID.NOBE_STATE_CHANGE, new Action(this.OnHeadInfoRefresh));
		}

		public override void UnInit()
		{
			base.UnInit();
			Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.HeadIcon_Change_Form_Open, new CUIEventManager.OnUIEventHandler(this.On_HeadIcon_Form_Open));
			Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.HeadIcon_Change_Form_Close, new CUIEventManager.OnUIEventHandler(this.On_HeadIcon_Change_Form_Close));
			Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.HeadIcon_Change_Tab_Click, new CUIEventManager.OnUIEventHandler(this.On_HeadIcon_Change_Tab_Click));
			Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.HeadIcon_Change_Icon_Click, new CUIEventManager.OnUIEventHandler(this.On_HeadIcon_Change_Icon_Click));
			Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.HeadIcon_Change_Confirm, new CUIEventManager.OnUIEventHandler(this.On_HeadIcon_Change_Confirm));
			Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.HeadIcon_Change_Item_Enable, new CUIEventManager.OnUIEventHandler(this.On_HeadIcon_Change_Item_Enable));
			Singleton<EventRouter>.instance.RemoveEventHandler(EventID.NOBE_STATE_CHANGE, new Action(this.OnHeadInfoRefresh));
		}

		public void SortResDicAll()
		{
			ListView<ResHeadImage> curHeadImgList = this.GetCurHeadImgList(HeadIconSys.enHeadImgTab.All);
			curHeadImgList.Sort(new Comparison<ResHeadImage>(this.ComparisonByTab));
			curHeadImgList = this.GetCurHeadImgList(HeadIconSys.enHeadImgTab.Nobe);
			curHeadImgList.Sort(new Comparison<ResHeadImage>(this.ComparisonByWeight));
			curHeadImgList = this.GetCurHeadImgList(HeadIconSys.enHeadImgTab.Activity);
			curHeadImgList.Sort(new Comparison<ResHeadImage>(this.ComparisonByTime));
			curHeadImgList = this.GetCurHeadImgList(HeadIconSys.enHeadImgTab.Skin);
			curHeadImgList.Sort(new Comparison<ResHeadImage>(this.ComparisonByTime));
			curHeadImgList = this.GetCurHeadImgList(HeadIconSys.enHeadImgTab.Team);
			curHeadImgList.Sort(new Comparison<ResHeadImage>(this.ComparisonByWeight));
		}

		private void SortResDic(HeadIconSys.enHeadImgTab tab)
		{
			ListView<ResHeadImage> curHeadImgList = this.GetCurHeadImgList(tab);
			if (tab == HeadIconSys.enHeadImgTab.All)
			{
				curHeadImgList.Sort(new Comparison<ResHeadImage>(this.ComparisonByTab));
			}
			else if (tab == HeadIconSys.enHeadImgTab.Nobe || tab == HeadIconSys.enHeadImgTab.Team)
			{
				curHeadImgList.Sort(new Comparison<ResHeadImage>(this.ComparisonByWeight));
			}
			else if (tab == HeadIconSys.enHeadImgTab.Activity || tab == HeadIconSys.enHeadImgTab.Skin)
			{
				curHeadImgList.Sort(new Comparison<ResHeadImage>(this.ComparisonByTime));
			}
		}

		private int ComparisonByTab(ResHeadImage left, ResHeadImage right)
		{
			HeadIconSys.HeadImgInfo info = this.GetInfo(left.dwID);
			HeadIconSys.HeadImgInfo info2 = this.GetInfo(right.dwID);
			if (info != null && info2 == null)
			{
				return -10000;
			}
			if (info == null && info2 != null)
			{
				return 10000;
			}
			if (info != null || info2 != null)
			{
				if (info != null && info2 != null)
				{
					if (info.bNtfFlag == 1 && info2.bNtfFlag == 0)
					{
						return -100;
					}
					if (info.bNtfFlag == 0 && info2.bNtfFlag == 1)
					{
						return 100;
					}
					if (info.bNtfFlag == 0 && info2.bNtfFlag == 0)
					{
						if (left.bHeadType < right.bHeadType)
						{
							return -1;
						}
						if (left.bHeadType > right.bHeadType)
						{
							return 1;
						}
						if (left.dwID < right.dwID)
						{
							return -1;
						}
						return 1;
					}
				}
				return 0;
			}
			if (left.bHeadType < right.bHeadType)
			{
				return -1;
			}
			if (left.bHeadType > right.bHeadType)
			{
				return 1;
			}
			if (left.dwID < right.dwID)
			{
				return -1;
			}
			return 1;
		}

		private int ComparisonByWeight(ResHeadImage left, ResHeadImage right)
		{
			HeadIconSys.HeadImgInfo info = this.GetInfo(left.dwID);
			HeadIconSys.HeadImgInfo info2 = this.GetInfo(right.dwID);
			if (info != null && info2 == null)
			{
				return -10000;
			}
			if (info == null && info2 != null)
			{
				return 10000;
			}
			if (info == null && info2 == null)
			{
				if (left.bSortWeight > right.bSortWeight)
				{
					return -10;
				}
				if (left.bSortWeight < right.bSortWeight)
				{
					return 10;
				}
				if (left.dwID < right.dwID)
				{
					return -1;
				}
				return 1;
			}
			else
			{
				if (info == null || info2 == null)
				{
					return 0;
				}
				if (info.bNtfFlag == 1 && info2.bNtfFlag == 0)
				{
					return -1000;
				}
				if (info.bNtfFlag == 0 && info2.bNtfFlag == 1)
				{
					return 1000;
				}
				if (left.bSortWeight > right.bSortWeight)
				{
					return -10;
				}
				if (left.bSortWeight < right.bSortWeight)
				{
					return 10;
				}
				if (left.dwID < right.dwID)
				{
					return -1;
				}
				return 1;
			}
		}

		private int ComparisonByTime(ResHeadImage left, ResHeadImage right)
		{
			HeadIconSys.HeadImgInfo info = this.GetInfo(left.dwID);
			HeadIconSys.HeadImgInfo info2 = this.GetInfo(right.dwID);
			if (info != null && info2 == null)
			{
				return -10000;
			}
			if (info == null && info2 != null)
			{
				return 10000;
			}
			if (info == null && info2 == null)
			{
				if (left.bSortWeight > right.bSortWeight)
				{
					return -10;
				}
				if (left.bSortWeight < right.bSortWeight)
				{
					return 10;
				}
				if (left.dwID < right.dwID)
				{
					return -1;
				}
				return 1;
			}
			else
			{
				if (info == null || info2 == null)
				{
					return 0;
				}
				if (info.bNtfFlag == 1 && info2.bNtfFlag == 0)
				{
					return -1000;
				}
				if (info.bNtfFlag == 0 && info2.bNtfFlag == 1)
				{
					return 1000;
				}
				if (info.dwGetTime > info2.dwGetTime)
				{
					return -100;
				}
				if (info.dwGetTime < info2.dwGetTime)
				{
					return 100;
				}
				if (left.bSortWeight > right.bSortWeight)
				{
					return -10;
				}
				if (left.bSortWeight < right.bSortWeight)
				{
					return 10;
				}
				if (left.dwID < right.dwID)
				{
					return -1;
				}
				return 1;
			}
		}

		public ResHeadImage GetResHeadImage(int id)
		{
			ListView<ResHeadImage> curHeadImgList = this.GetCurHeadImgList(HeadIconSys.enHeadImgTab.All);
			for (int i = 0; i < curHeadImgList.Count; i++)
			{
				ResHeadImage resHeadImage = curHeadImgList[i];
				if (resHeadImage != null && (ulong)resHeadImage.dwID == (ulong)((long)id))
				{
					return resHeadImage;
				}
			}
			return null;
		}

		private ListView<ResHeadImage> GetCurHeadImgList(HeadIconSys.enHeadImgTab curTab)
		{
			ListView<ResHeadImage> listView = null;
			if (!this.headImageDic.TryGetValue(curTab, out listView))
			{
				DictionaryView<uint, ResHeadImage>.Enumerator enumerator = GameDataMgr.headImageDict.GetEnumerator();
				RES_HEADIMG_SOURCE_TYPE headType = this.GetHeadType(curTab);
				listView = new ListView<ResHeadImage>();
				if (GameDataMgr.headImageDict.Count == 0)
				{
					return listView;
				}
				while (enumerator.MoveNext())
				{
					KeyValuePair<uint, ResHeadImage> current = enumerator.Current;
					ResHeadImage value = current.Value;
					if (headType == RES_HEADIMG_SOURCE_TYPE.RES_HEADIMG_SOURCE_MAX || headType == (RES_HEADIMG_SOURCE_TYPE)value.bHeadType)
					{
						listView.Add(value);
					}
				}
				this.headImageDic.Add(curTab, listView);
			}
			return listView;
		}

		private void On_HeadIcon_Form_Open(CUIEvent uiEvent)
		{
			CUIFormScript cUIFormScript = Singleton<CUIManager>.instance.OpenForm(HeadIconSys.s_headImgChgForm, false, true);
			if (cUIFormScript != null)
			{
				CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(cUIFormScript.gameObject, "pnlBg/Panel_Menu/List");
				componetInChild.SetElementAmount(5);
				for (int i = 0; i < 5; i++)
				{
					componetInChild.GetElemenet(i).GetComponentInChildren<Text>().text = Singleton<CTextManager>.instance.GetText(string.Format("HeadImg_Tab_Txt_{0}", i + 1));
				}
				componetInChild.SelectElement(0, true);
			}
		}

		private void On_HeadIcon_Change_Tab_Click(CUIEvent uiEvent)
		{
			HeadIconSys.enHeadImgTab curTab = this.GetCurTab();
			this.SortResDic(curTab);
			this.OnShowMainPanel(curTab);
		}

		private HeadIconSys.enHeadImgTab GetCurTab()
		{
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(HeadIconSys.s_headImgChgForm);
			if (form != null)
			{
				CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(form.gameObject, "pnlBg/Panel_Menu/List");
				return (HeadIconSys.enHeadImgTab)componetInChild.GetSelectedIndex();
			}
			return HeadIconSys.enHeadImgTab.All;
		}

		private ResHeadImage GetSelectedHeadImg()
		{
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(HeadIconSys.s_headImgChgForm);
			if (form != null)
			{
				CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(form.gameObject, "pnlBg/Panel_Menu/List");
				CUIListScript componetInChild2 = Utility.GetComponetInChild<CUIListScript>(form.gameObject, "pnlBg/Panel_Main/List");
				int selectedIndex = componetInChild.GetSelectedIndex();
				int selectedIndex2 = componetInChild2.GetSelectedIndex();
				ListView<ResHeadImage> curHeadImgList = this.GetCurHeadImgList((HeadIconSys.enHeadImgTab)selectedIndex);
				if (selectedIndex2 > -1 && selectedIndex2 < curHeadImgList.Count)
				{
					return curHeadImgList[selectedIndex2];
				}
			}
			return null;
		}

		private RES_HEADIMG_SOURCE_TYPE GetHeadType(HeadIconSys.enHeadImgTab type)
		{
			if (type == HeadIconSys.enHeadImgTab.All)
			{
				return RES_HEADIMG_SOURCE_TYPE.RES_HEADIMG_SOURCE_MAX;
			}
			if (type == HeadIconSys.enHeadImgTab.Nobe)
			{
				return RES_HEADIMG_SOURCE_TYPE.RES_HEADIMG_SOURCE_NOBE;
			}
			if (type == HeadIconSys.enHeadImgTab.Activity)
			{
				return RES_HEADIMG_SOURCE_TYPE.RES_HEADIMG_SOURCE_ACTIVITY;
			}
			if (type == HeadIconSys.enHeadImgTab.Skin)
			{
				return RES_HEADIMG_SOURCE_TYPE.RES_HEADIMG_SOURCE_SKIN;
			}
			if (type == HeadIconSys.enHeadImgTab.Team)
			{
				return RES_HEADIMG_SOURCE_TYPE.RES_HEADIMG_SOURCE_BATTLE;
			}
			return RES_HEADIMG_SOURCE_TYPE.RES_HEADIMG_SOURCE_MAX;
		}

		public HeadIconSys.HeadImgInfo GetInfo(uint headImgId)
		{
			for (int i = 0; i < this.m_headImgInfo.Count; i++)
			{
				if (this.m_headImgInfo[i].dwID == headImgId)
				{
					return this.m_headImgInfo[i];
				}
			}
			return null;
		}

		public ResHeadImage GetResInfo(uint headImgId)
		{
			ListView<ResHeadImage> curHeadImgList = this.GetCurHeadImgList(HeadIconSys.enHeadImgTab.All);
			for (int i = 0; i < curHeadImgList.Count; i++)
			{
				if (curHeadImgList[i].dwID == headImgId)
				{
					return curHeadImgList[i];
				}
			}
			return null;
		}

		private void OnShowMainPanel(HeadIconSys.enHeadImgTab curTab)
		{
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(HeadIconSys.s_headImgChgForm);
			if (form != null)
			{
				ListView<ResHeadImage> curHeadImgList = this.GetCurHeadImgList(curTab);
				int count = curHeadImgList.Count;
				CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(form.gameObject, "pnlBg/Panel_Main/List");
				componetInChild.SetElementAmount(count);
				int index = 0;
				for (int i = 0; i < count; i++)
				{
					if (this.IsHeadIconInUse(curHeadImgList[i].dwID))
					{
						index = i;
						break;
					}
				}
				if (count > 0)
				{
					componetInChild.SelectElement(index, true);
				}
				else
				{
					this.OnShowDetailPanel(-1);
				}
			}
		}

		private void OnHeadInfoRefresh()
		{
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(HeadIconSys.s_headImgChgForm);
			if (form != null)
			{
				HeadIconSys.enHeadImgTab curTab = this.GetCurTab();
				ListView<ResHeadImage> curHeadImgList = this.GetCurHeadImgList(curTab);
				int count = curHeadImgList.Count;
				CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(form.gameObject, "pnlBg/Panel_Main/List");
				for (int i = 0; i < count; i++)
				{
					CUIListElementScript elemenet = componetInChild.GetElemenet(i);
					if (elemenet != null && elemenet.gameObject != null)
					{
						this.OnUpdateElement(elemenet.gameObject, i, curTab);
					}
				}
			}
		}

		private void OnUpdateElement(GameObject element, int index, HeadIconSys.enHeadImgTab curTab)
		{
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(HeadIconSys.s_headImgChgForm);
			if (form == null)
			{
				return;
			}
			ListView<ResHeadImage> curHeadImgList = this.GetCurHeadImgList(curTab);
			if (index < 0 || index >= curHeadImgList.Count)
			{
				return;
			}
			ResHeadImage resHeadImage = curHeadImgList[index];
			if (resHeadImage == null)
			{
				return;
			}
			HeadIconSys.HeadImgInfo info = this.GetInfo(resHeadImage.dwID);
			Image componetInChild = Utility.GetComponetInChild<Image>(element, "HeadImg");
			MonoSingleton<NobeSys>.instance.SetHeadIconBk(componetInChild, (int)resHeadImage.dwID);
			MonoSingleton<NobeSys>.instance.SetHeadIconBkEffect(componetInChild, (int)resHeadImage.dwID, form, 0.8f, true);
			if (info != null)
			{
				Utility.FindChild(element, "Flag").CustomSetActive(info.bNtfFlag == 1);
				Utility.FindChild(element, "Lock").CustomSetActive(false);
				Utility.FindChild(element, "Text").CustomSetActive(this.IsHeadIconInUse(info.dwID));
			}
			else
			{
				Utility.FindChild(element, "Flag").CustomSetActive(false);
				Utility.FindChild(element, "Lock").CustomSetActive(true);
				Utility.FindChild(element, "Text").CustomSetActive(false);
			}
		}

		private bool IsHeadIconInUse(uint headIconId)
		{
			return MonoSingleton<NobeSys>.GetInstance().m_vipInfo.stGameVipClient.dwHeadIconId == headIconId;
		}

		private void On_HeadIcon_Change_Item_Enable(CUIEvent uiEvent)
		{
			HeadIconSys.enHeadImgTab curTab = this.GetCurTab();
			int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
			ListView<ResHeadImage> curHeadImgList = this.GetCurHeadImgList(curTab);
			this.OnUpdateElement(uiEvent.m_srcWidget, srcWidgetIndexInBelongedList, curTab);
		}

		private void On_HeadIcon_Change_Icon_Click(CUIEvent uiEvent)
		{
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(HeadIconSys.s_headImgChgForm);
			if (form != null)
			{
				int selectedIndex = uiEvent.m_srcWidget.GetComponent<CUIListScript>().GetSelectedIndex();
				this.OnShowDetailPanel(selectedIndex);
				ResHeadImage selectedHeadImg = this.GetSelectedHeadImg();
				if (selectedHeadImg != null)
				{
					HeadIconSys.HeadImgInfo info = this.GetInfo(selectedHeadImg.dwID);
					if (info != null && info.bNtfFlag == 1)
					{
						HeadIconSys.OnHeadIconFlagClearReq(info.dwID);
					}
				}
			}
		}

		private void OnShowDetailPanel(int index)
		{
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(HeadIconSys.s_headImgChgForm);
			if (form != null)
			{
				HeadIconSys.enHeadImgTab curTab = this.GetCurTab();
				ListView<ResHeadImage> curHeadImgList = this.GetCurHeadImgList(curTab);
				if (index == -1)
				{
					Utility.FindChild(form.gameObject, "pnlBg/Panel_Detail/Node").SetActive(false);
					return;
				}
				if (index < curHeadImgList.Count)
				{
					Utility.FindChild(form.gameObject, "pnlBg/Panel_Detail/Node").SetActive(true);
					ResHeadImage resHeadImage = curHeadImgList[index];
					HeadIconSys.HeadImgInfo info = this.GetInfo(resHeadImage.dwID);
					Text componetInChild = Utility.GetComponetInChild<Text>(form.gameObject, "pnlBg/Panel_Detail/Node/DescTxt");
					Text componetInChild2 = Utility.GetComponetInChild<Text>(form.gameObject, "pnlBg/Panel_Detail/Node/AvaildTimeTxt");
					Image componetInChild3 = Utility.GetComponetInChild<Image>(form.gameObject, "pnlBg/Panel_Detail/Node/HeadImg");
					Button componetInChild4 = Utility.GetComponetInChild<Button>(form.gameObject, "pnlBg/Panel_Detail/Node/Button");
					Text componetInChild5 = Utility.GetComponetInChild<Text>(form.gameObject, "pnlBg/Panel_Detail/Node/SrcTxt");
					componetInChild.text = resHeadImage.szHeadDesc;
					MonoSingleton<NobeSys>.instance.SetHeadIconBk(componetInChild3, (int)resHeadImage.dwID);
					MonoSingleton<NobeSys>.instance.SetHeadIconBkEffect(componetInChild3, (int)resHeadImage.dwID, form, 1f, false);
					if (resHeadImage.dwValidSecond == 0u)
					{
						componetInChild2.text = Singleton<CTextManager>.instance.GetText("HeadImg_Tips_1");
					}
					else if (info != null)
					{
						DateTime dateTime = Utility.ToUtcTime2Local((long)CRoleInfo.GetCurrentUTCTime() + (long)((ulong)resHeadImage.dwValidSecond));
						componetInChild2.text = Singleton<CTextManager>.instance.GetText("HeadImg_Tips_2", new string[]
						{
							dateTime.Year.ToString(),
							dateTime.Month.ToString(),
							dateTime.Day.ToString()
						});
					}
					else
					{
						componetInChild2.text = Singleton<CTextManager>.instance.GetText("HeadImg_Tips_3", new string[]
						{
							Math.Ceiling((double)(resHeadImage.dwValidSecond / 86400f)).ToString()
						});
					}
					if (info != null)
					{
						componetInChild4.gameObject.CustomSetActive(!this.IsHeadIconInUse(info.dwID));
						componetInChild5.gameObject.SetActive(false);
					}
					else
					{
						componetInChild4.gameObject.CustomSetActive(false);
						componetInChild5.gameObject.SetActive(true);
						componetInChild5.text = resHeadImage.szHeadAccess;
					}
				}
			}
		}

		private void On_HeadIcon_Change_Confirm(CUIEvent uiEvent)
		{
			ResHeadImage selectedHeadImg = this.GetSelectedHeadImg();
			if (selectedHeadImg == null)
			{
				return;
			}
			if (MonoSingleton<NobeSys>.GetInstance().m_vipInfo.stGameVipClient.dwHeadIconId == selectedHeadImg.dwID)
			{
				return;
			}
			HeadIconSys.OnHeadIconChangeReq(selectedHeadImg.dwID);
		}

		private void On_HeadIcon_Change_Form_Close(CUIEvent uiEvent)
		{
		}

		public void OnHeadIconSyncList(ushort count, COMDT_ACNT_HEADIMG_INFO[] astHeadImgInfo)
		{
			this.m_headImgInfo.Clear();
			for (int i = 0; i < (int)count; i++)
			{
				HeadIconSys.HeadImgInfo headImgInfo = new HeadIconSys.HeadImgInfo();
				headImgInfo.dwID = astHeadImgInfo[i].dwID;
				headImgInfo.dwGetTime = astHeadImgInfo[i].dwGetTime;
				headImgInfo.bNtfFlag = astHeadImgInfo[i].bNtfFlag;
				this.m_headImgInfo.Add(headImgInfo);
			}
		}

		public void SetMasterHeadImg(uint headImg)
		{
			MonoSingleton<NobeSys>.GetInstance().m_vipInfo.stGameVipClient.dwHeadIconId = headImg;
		}

		public void ClearHeadImgFlag(uint headImg)
		{
			for (int i = 0; i < this.m_headImgInfo.Count; i++)
			{
				if (this.m_headImgInfo[i].dwID == headImg)
				{
					this.m_headImgInfo[i].bNtfFlag = 0;
				}
			}
		}

		public void AddHeadImgInfo(uint dwHeadImgID, uint dwGetTime)
		{
			HeadIconSys.HeadImgInfo headImgInfo = new HeadIconSys.HeadImgInfo();
			headImgInfo.dwID = dwHeadImgID;
			headImgInfo.dwGetTime = dwGetTime;
			headImgInfo.bNtfFlag = 1;
			for (int i = 0; i < this.m_headImgInfo.Count; i++)
			{
				if (this.m_headImgInfo[i].dwID == dwHeadImgID)
				{
					this.m_headImgInfo.RemoveAt(i);
					i--;
				}
			}
			this.m_headImgInfo.Add(headImgInfo);
		}

		public void DelHeadImgInfo(uint dwHeadImgID)
		{
			for (int i = 0; i < this.m_headImgInfo.Count; i++)
			{
				if (this.m_headImgInfo[i].dwID == dwHeadImgID)
				{
					this.m_headImgInfo.RemoveAt(i);
					i--;
				}
			}
		}

		public void Clear()
		{
			this.headImageDic.Clear();
			this.m_headImgInfo.Clear();
		}

		private void RefreshUseButton(bool isShowUseButton)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(HeadIconSys.s_headImgChgForm);
			if (form != null)
			{
				GameObject gameObject = Utility.FindChild(form.gameObject, "pnlBg/Panel_Detail/Node/Button");
				if (gameObject != null)
				{
					gameObject.CustomSetActive(isShowUseButton);
				}
			}
		}

		private static void OnHeadIconChangeReq(uint headImgId)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(4601u);
			cSPkg.stPkgData.stHeadImgChgReq.dwHeadImgID = headImgId;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		[MessageHandler(4602)]
		public static void OnHeadIconChangeRsp(CSPkg msg)
		{
			if (msg.stPkgData.stHeadImgChgRsp.iResult == 0)
			{
				Singleton<HeadIconSys>.instance.SetMasterHeadImg(msg.stPkgData.stHeadImgChgRsp.dwHeadImgID);
				Singleton<EventRouter>.instance.BroadCastEvent(EventID.NOBE_STATE_CHANGE);
			}
		}

		private static void OnHeadIconFlagClearReq(uint headImgId)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(4603u);
			cSPkg.stPkgData.stHeadImgFlagClrReq.dwHeadImgID = headImgId;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		[MessageHandler(4604)]
		public static void OnHeadIconFlagClearRsp(CSPkg msg)
		{
			Singleton<HeadIconSys>.instance.ClearHeadImgFlag(msg.stPkgData.stHeadImgFlagClrRsp.dwHeadImgID);
			Singleton<EventRouter>.instance.BroadCastEvent(EventID.NOBE_STATE_CHANGE);
			Singleton<EventRouter>.instance.BroadCastEvent(EventID.HEAD_IMAGE_FLAG_CHANGE);
		}

		[MessageHandler(4605)]
		public static void OnHeadIconChangeNtf(CSPkg msg)
		{
			Singleton<HeadIconSys>.instance.SetMasterHeadImg(msg.stPkgData.stHeadImgChgNtf.dwHeadImgID);
			Singleton<HeadIconSys>.instance.RefreshUseButton(false);
			Singleton<EventRouter>.instance.BroadCastEvent(EventID.NOBE_STATE_CHANGE);
		}

		[MessageHandler(4606)]
		public static void OnHeadIconSyncNtf(CSPkg msg)
		{
			Singleton<HeadIconSys>.instance.OnHeadIconSyncList(msg.stPkgData.stHeadImgListSync.stHeadImgList.wHeadImgCnt, msg.stPkgData.stHeadImgListSync.stHeadImgList.astHeadImgInfo);
			Singleton<EventRouter>.instance.BroadCastEvent(EventID.HEAD_IMAGE_FLAG_CHANGE);
		}

		[MessageHandler(4607)]
		public static void OnHeadIconAddNtf(CSPkg msg)
		{
			Singleton<HeadIconSys>.instance.AddHeadImgInfo(msg.stPkgData.stHeadImgAddNtf.dwHeadImgID, msg.stPkgData.stHeadImgAddNtf.dwGetTime);
			Singleton<EventRouter>.instance.BroadCastEvent(EventID.HEAD_IMAGE_FLAG_CHANGE);
		}

		[MessageHandler(4608)]
		public static void OnHeadIconDelNtf(CSPkg msg)
		{
			Singleton<HeadIconSys>.instance.DelHeadImgInfo(msg.stPkgData.stHeadImgDelNtf.dwHeadImgID);
			Singleton<EventRouter>.instance.BroadCastEvent(EventID.HEAD_IMAGE_FLAG_CHANGE);
		}
	}
}
