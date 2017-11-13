using Apollo;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class CFriendView
	{
		public enum Tab
		{
			Friend_SNS,
			Friend,
			Mentor,
			Friend_LBS = 5,
			Friend_Recruit,
			Find_SocialFriend,
			None
		}

		public class AddFriendView
		{
			private CUIFormScript form;

			private Text input;

			private GameObject info_text;

			private FriendShower searchFriendShower;

			public CUIListScript recommandFriendListCom;

			private static readonly Vector2 recommandFriendListPos1 = new Vector2(40f, 190f);

			private static readonly Vector2 recommandFriendListSize1 = new Vector2(-80f, 180f);

			private static readonly Vector2 recommandFriendListPos2 = new Vector2(40f, 340f);

			private static readonly Vector2 recommandFriendListSize2 = new Vector2(-80f, 320f);

			public int m_addFriendTypeSrv = 1;

			public bool bShow;

			public COMDT_FRIEND_INFO search_info_Game;

			public GameObject buttons_node;

			public void Record_SearchFriend(COMDT_FRIEND_INFO info)
			{
				this.search_info_Game = info;
			}

			public void Clear_SearchFriend()
			{
				this.search_info_Game = null;
			}

			public void Init(int searchType)
			{
				Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_SerchFriend, new CUIEventManager.OnUIEventHandler(this.On_SearchFriend));
				Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_Close_AddForm, new CUIEventManager.OnUIEventHandler(this.On_Friend_Close_AddForm));
				this.form = Singleton<CUIManager>.GetInstance().OpenForm(CFriendContoller.AddFriendFormPath, false, true);
				this.input = this.form.transform.FindChild("GameObject/SearchFriend/InputField/Text").GetComponent<Text>();
				this.searchFriendShower = this.form.transform.FindChild("GameObject/SearchFriend/Result/Friend").GetComponent<FriendShower>();
				this.searchFriendShower.gameObject.CustomSetActive(false);
				this.recommandFriendListCom = Utility.GetComponetInChild<CUIListScript>(this.form.gameObject, "GameObject/RecommandList");
				this.buttons_node = this.form.transform.FindChild("GameObject/Buttons").gameObject;
				this.info_text = this.form.transform.Find("GameObject/SearchFriend/Result/info").gameObject;
				if (this.info_text != null)
				{
					this.info_text.CustomSetActive(false);
				}
				CFriendContoller.s_addViewtype = searchType;
				FriendSysNetCore.Send_Request_RecommandFriend_List(searchType);
				this.Refresh(-1);
				this.bShow = true;
				this.m_addFriendTypeSrv = searchType;
				GameObject widget = this.form.GetWidget(0);
				GameObject widget2 = this.form.GetWidget(1);
				GameObject widget3 = this.form.GetWidget(2);
				GameObject widget4 = this.form.GetWidget(3);
				string[] array = new string[]
				{
					string.Empty,
					Singleton<CTextManager>.GetInstance().GetText("Friend_AddTitle"),
					Singleton<CTextManager>.GetInstance().GetText("Mentor_IWant", new string[]
					{
						Singleton<CTextManager>.GetInstance().GetText("Mentor_GetMentor")
					}),
					Singleton<CTextManager>.GetInstance().GetText("Mentor_IWant", new string[]
					{
						Singleton<CTextManager>.GetInstance().GetText("Mentor_GetApprentice")
					})
				};
				string[] array2 = new string[]
				{
					string.Empty,
					Singleton<CTextManager>.GetInstance().GetText("Mentor_InputReplacer", new string[]
					{
						Singleton<CTextManager>.GetInstance().GetText("chat_title_friend")
					}),
					Singleton<CTextManager>.GetInstance().GetText("Mentor_IWant", new string[]
					{
						Singleton<CTextManager>.GetInstance().GetText("Mentor_mentor")
					}),
					Singleton<CTextManager>.GetInstance().GetText("Mentor_IWant", new string[]
					{
						Singleton<CTextManager>.GetInstance().GetText("Mentor_apprentice")
					})
				};
				string[] array3 = new string[]
				{
					string.Empty,
					Singleton<CTextManager>.GetInstance().GetText("Mentor_recommend", new string[]
					{
						Singleton<CTextManager>.GetInstance().GetText("chat_title_friend")
					}),
					Singleton<CTextManager>.GetInstance().GetText("Mentor_recommend", new string[]
					{
						Singleton<CTextManager>.GetInstance().GetText("Mentor_mentor")
					}),
					Singleton<CTextManager>.GetInstance().GetText("Mentor_recommend", new string[]
					{
						Singleton<CTextManager>.GetInstance().GetText("Mentor_apprentice")
					})
				};
				string[] array4 = new string[]
				{
					string.Empty,
					Singleton<CTextManager>.GetInstance().GetText("Mentor_recommend", new string[]
					{
						Singleton<CTextManager>.GetInstance().GetText("chat_title_friend")
					}),
					Singleton<CTextManager>.GetInstance().GetText("Mentor_recommend", new string[]
					{
						Singleton<CTextManager>.GetInstance().GetText("Mentor_mentor")
					}),
					Singleton<CTextManager>.GetInstance().GetText("Mentor_recommend", new string[]
					{
						Singleton<CTextManager>.GetInstance().GetText("Mentor_apprentice")
					})
				};
				widget.GetComponent<Text>().set_text(array[searchType]);
				widget2.GetComponent<Text>().set_text(array2[searchType]);
				widget3.GetComponent<Text>().set_text(array3[searchType]);
				widget4.GetComponent<Text>().set_text(array4[searchType]);
			}

			public void Clear()
			{
				Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Friend_SerchFriend, new CUIEventManager.OnUIEventHandler(this.On_SearchFriend));
				Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Friend_Close_AddForm, new CUIEventManager.OnUIEventHandler(this.On_Friend_Close_AddForm));
				this.input = null;
				this.searchFriendShower = null;
				this.recommandFriendListCom = null;
				this.form = null;
				this.search_info_Game = null;
				this.buttons_node = null;
				this.bShow = false;
				Singleton<CFriendContoller>.GetInstance().search_info = null;
			}

			public void Refresh(int type = -1)
			{
				if (type == -1)
				{
					type = (int)CFriendContoller.GetFriendType(CFriendContoller.s_addViewtype);
				}
				this.buttons_node.CustomSetActive(false);
				this.Show_Search_Game((CFriendModel.FriendType)type);
				this.Show_Search_Result(null);
			}

			private void Show_Search_Game(CFriendModel.FriendType type)
			{
				this.Refresh_Friend_Recommand_List(type);
				this.Refresh_Friend_Recommand_List_Pos();
			}

			public void On_SearchFriend(CUIEvent uiEvent)
			{
				this.Clear_SearchFriend();
				this.searchFriendShower.gameObject.CustomSetActive(false);
				if (string.IsNullOrEmpty(this.input.get_text()))
				{
					Singleton<CUIManager>.GetInstance().OpenMessageBox(UT.GetText("Friend_Input_Tips"), false);
				}
				else
				{
					FriendSysNetCore.Send_Serch_Player(this.input.get_text(), enFriendSearchSource.FriendSystem);
				}
				this.Refresh_Friend_Recommand_List_Pos();
			}

			public void On_Friend_Close_AddForm(CUIEvent uiEvent)
			{
				this.Clear();
			}

			public void Show_Search_Result(COMDT_FRIEND_INFO info)
			{
				COMDT_FRIEND_INFO cOMDT_FRIEND_INFO = this.search_info_Game;
				this.recommandFriendListCom.gameObject.CustomSetActive(true);
				if (cOMDT_FRIEND_INFO == null)
				{
					if (this.searchFriendShower != null)
					{
						this.searchFriendShower.gameObject.CustomSetActive(false);
					}
				}
				else if (this.searchFriendShower != null)
				{
					this.searchFriendShower.gameObject.CustomSetActive(true);
					UT.ShowFriendData(cOMDT_FRIEND_INFO, this.searchFriendShower, this.GetItemTypeBySearchType(this.m_addFriendTypeSrv), false, CFriendModel.FriendType.RequestFriend, this.form, false);
					if (this.recommandFriendListCom.GetElementAmount() == 0)
					{
						this.recommandFriendListCom.gameObject.CustomSetActive(false);
					}
				}
			}

			public void Refresh_Friend_Recommand_List(CFriendModel.FriendType type)
			{
				if (this.recommandFriendListCom == null)
				{
					return;
				}
				Singleton<CFriendContoller>.GetInstance().model.FilterRecommendFriends();
				int dataCount = Singleton<CFriendContoller>.GetInstance().model.GetDataCount(type);
				this.recommandFriendListCom.SetElementAmount(dataCount);
				for (int i = 0; i < dataCount; i++)
				{
					COMDT_FRIEND_INFO infoAtIndex = Singleton<CFriendContoller>.GetInstance().model.GetInfoAtIndex(type, i);
					if (infoAtIndex != null)
					{
						this.Refresh_Recomand_Friend(i, infoAtIndex, (int)type);
					}
				}
			}

			public void Refresh_Friend_Recommand_List_Pos()
			{
				if (this.recommandFriendListCom == null)
				{
					return;
				}
				if (this.searchFriendShower == null)
				{
					return;
				}
				RectTransform rectTransform = this.recommandFriendListCom.transform as RectTransform;
				if (this.searchFriendShower.gameObject.activeSelf)
				{
					rectTransform.anchoredPosition = CFriendView.AddFriendView.recommandFriendListPos1;
					rectTransform.sizeDelta = CFriendView.AddFriendView.recommandFriendListSize1;
					this.recommandFriendListCom.m_scrollAreaSize = new Vector2(rectTransform.rect.width, rectTransform.rect.height);
				}
				else
				{
					rectTransform.anchoredPosition = CFriendView.AddFriendView.recommandFriendListPos2;
					rectTransform.sizeDelta = CFriendView.AddFriendView.recommandFriendListSize2;
					this.recommandFriendListCom.m_scrollAreaSize = new Vector2(rectTransform.rect.width, rectTransform.rect.height);
				}
			}

			public void Refresh_Recomand_Friend(int index, COMDT_FRIEND_INFO info, int type)
			{
				CUIListElementScript elemenet = this.recommandFriendListCom.GetElemenet(index);
				if (elemenet == null)
				{
					return;
				}
				elemenet.SetDataTag(type.ToString());
				FriendShower component = elemenet.GetComponent<FriendShower>();
				if (component != null)
				{
					UT.ShowFriendData(info, component, this.GetItemTypeBySearchType(this.m_addFriendTypeSrv), false, (CFriendModel.FriendType)type, this.form, true);
				}
			}

			private FriendShower.ItemType GetItemTypeBySearchType(int searchType)
			{
				switch (searchType)
				{
				case 1:
					return FriendShower.ItemType.Add;
				case 2:
					return FriendShower.ItemType.AddMentor;
				case 3:
					return FriendShower.ItemType.AddApprentice;
				default:
					return FriendShower.ItemType.Add;
				}
			}
		}

		public class Verfication
		{
			public enum eVerficationFormWidget
			{
				None = -1,
				NameInputField,
				DescText
			}

			private static int Verfication_ChatMaxLength = 15;

			private ulong ullUid;

			private uint dwLogicWorldId;

			private bool m_bAddSearchFirend;

			private InputField _inputName;

			private COM_ADD_FRIEND_TYPE m_addFriendSourceType;

			private int m_addFriendUseHeroId;

			public Verfication()
			{
				Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_Verify_Close, new CUIEventManager.OnUIEventHandler(this.On_Friend_Verify_Close));
				Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Friend_Verify_Send, new CUIEventManager.OnUIEventHandler(this.On_Friend_Verify_Send));
			}

			public void Update()
			{
				if (this._inputName == null)
				{
					return;
				}
				string text = this._inputName.get_text();
				if (text != null && text.get_Length() > CFriendView.Verfication.Verfication_ChatMaxLength)
				{
					this._inputName.DeactivateInputField();
					this._inputName.set_text(text.Substring(0, CFriendView.Verfication.Verfication_ChatMaxLength));
					Singleton<CUIManager>.GetInstance().OpenMessageBox(string.Format(Singleton<CTextManager>.instance.GetText("chat_input_max"), CFriendView.Verfication.Verfication_ChatMaxLength), false);
				}
			}

			public void Open(ulong ullUid, uint dwLogicWorldId, bool bAddSearchFirend, COM_ADD_FRIEND_TYPE addFriendType = COM_ADD_FRIEND_TYPE.COM_ADD_FRIEND_NULL, int useHeroId = -1, bool onlyAddFriend = true)
			{
				if (CFriendContoller.s_addViewtype == 1 || onlyAddFriend)
				{
					CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CFriendContoller.VerifyFriendFormPath, true, true);
					if (cUIFormScript == null)
					{
						return;
					}
					GameObject gameObject = cUIFormScript.GetWidget(0).gameObject;
					if (gameObject == null)
					{
						return;
					}
					this._inputName = gameObject.GetComponent<InputField>();
					if (this._inputName == null)
					{
						return;
					}
					if (this._inputName.get_placeholder() != null)
					{
						this._inputName.get_placeholder().gameObject.CustomSetActive(false);
					}
					string randomVerifyContent = Singleton<CFriendContoller>.instance.model.GetRandomVerifyContent();
					if (!string.IsNullOrEmpty(randomVerifyContent))
					{
						this._inputName.set_text(randomVerifyContent);
					}
					this.ullUid = ullUid;
					this.dwLogicWorldId = dwLogicWorldId;
					this.m_bAddSearchFirend = bAddSearchFirend;
					this.m_addFriendSourceType = addFriendType;
					this.m_addFriendUseHeroId = useHeroId;
				}
				else
				{
					int s_addViewtype = CFriendContoller.s_addViewtype;
					if (s_addViewtype == 2 || s_addViewtype == 3)
					{
						string title = null;
						string stringPlacer = null;
						string mentorStateString = Singleton<CFriendContoller>.GetInstance().GetMentorStateString();
						if (mentorStateString != null)
						{
							title = Singleton<CTextManager>.GetInstance().GetText("Mentor_VerifyReqTitle", new string[]
							{
								mentorStateString
							});
							stringPlacer = Singleton<CTextManager>.GetInstance().GetText("Mentor_VerifyReqReplacer", new string[]
							{
								mentorStateString
							});
						}
						if (Singleton<CFriendContoller>.instance.m_mentorSelectedUin == null)
						{
							Singleton<CFriendContoller>.instance.m_mentorSelectedUin = new COMDT_ACNT_UNIQ();
						}
						Singleton<CFriendContoller>.instance.m_mentorSelectedUin.dwLogicWorldId = dwLogicWorldId;
						Singleton<CFriendContoller>.instance.m_mentorSelectedUin.ullUid = ullUid;
						COMDT_FRIEND_INFO info = Singleton<CFriendContoller>.GetInstance().model.GetInfo(CFriendModel.FriendType.Apprentice, ullUid, dwLogicWorldId);
						if (info == null)
						{
							info = Singleton<CFriendContoller>.GetInstance().model.GetInfo(CFriendModel.FriendType.Mentor, ullUid, dwLogicWorldId);
						}
						if (info == null)
						{
							info = Singleton<CFriendContoller>.GetInstance().model.GetInfo(CFriendModel.FriendType.Apprentice, ullUid, dwLogicWorldId);
						}
						if (info == null)
						{
							Singleton<CUIManager>.GetInstance().OpenStringSenderBox(title, Singleton<CTextManager>.GetInstance().GetText("Mentor_VerifyReqDesc"), stringPlacer, new CUIManager.StringSendboxOnSend(Singleton<CFriendContoller>.GetInstance().OnMentorApplyVerifyBoxRetrun), CFriendView.Verfication.GetRandomMentorReuqestStr(CFriendContoller.s_addViewtype));
						}
						else
						{
							Singleton<CUIManager>.GetInstance().OpenTips(Singleton<CTextManager>.GetInstance().GetText("Mentor_Err_AlreadyMentor"), false, 1.5f, null, new object[0]);
						}
					}
				}
			}

			public static string GetRandomReuqestStr(string titlePrefix, int low, int high)
			{
				int num = Random.Range(low, high);
				return Singleton<CTextManager>.GetInstance().GetText(titlePrefix + num);
			}

			public static string GetRandomMentorReuqestStr(int type = -1)
			{
				int num = Random.Range(0, 3);
				if (type == -1)
				{
					switch (CFriendContoller.GetMentorState(-1, null))
					{
					case enMentorState.IWantApprentice:
					case enMentorState.IHasApprentice:
						type = 3;
						goto IL_3B;
					}
					type = 2;
				}
				IL_3B:
				int num2 = type;
				if (num2 == 2)
				{
					return Singleton<CTextManager>.GetInstance().GetText("Mentor_requestMentor_" + num);
				}
				if (num2 != 3)
				{
					return string.Empty;
				}
				return Singleton<CTextManager>.GetInstance().GetText("Mentor_requestApprentice_" + num);
			}

			private void On_Friend_Verify_Close(CUIEvent uievent)
			{
				this.Clear();
			}

			public void Clear()
			{
				this.ullUid = 0uL;
				this.dwLogicWorldId = 0u;
				this.m_bAddSearchFirend = false;
				this._inputName = null;
				this.m_addFriendSourceType = COM_ADD_FRIEND_TYPE.COM_ADD_FRIEND_NULL;
				this.m_addFriendUseHeroId = -1;
			}

			private void On_Friend_Verify_Send(CUIEvent uievent)
			{
				if (uievent == null)
				{
					Singleton<CUIManager>.instance.CloseForm(CFriendContoller.VerifyFriendFormPath);
					return;
				}
				CUIFormScript srcFormScript = uievent.m_srcFormScript;
				if (srcFormScript == null)
				{
					Singleton<CUIManager>.instance.CloseForm(CFriendContoller.VerifyFriendFormPath);
					return;
				}
				InputField component = srcFormScript.GetWidget(0).GetComponent<InputField>();
				if (component == null)
				{
					Singleton<CUIManager>.instance.CloseForm(CFriendContoller.VerifyFriendFormPath);
					return;
				}
				string veriyText = CUIUtility.RemoveEmoji(component.get_text()).Trim();
				if (this.ullUid == 0uL)
				{
					Singleton<CUIManager>.instance.CloseForm(CFriendContoller.VerifyFriendFormPath);
					return;
				}
				if (this.m_bAddSearchFirend)
				{
					FriendSysNetCore.Send_Request_BeFriend(this.ullUid, this.dwLogicWorldId, veriyText, this.m_addFriendSourceType, this.m_addFriendUseHeroId);
				}
				else
				{
					FriendSysNetCore.Send_Request_BeFriend(this.ullUid, this.dwLogicWorldId, veriyText, this.m_addFriendSourceType, this.m_addFriendUseHeroId);
					Singleton<CFriendContoller>.instance.model.Remove(CFriendModel.FriendType.Recommend, this.ullUid, this.dwLogicWorldId);
				}
				this.ullUid = 0uL;
				this.dwLogicWorldId = 0u;
				this.m_bAddSearchFirend = false;
				this._inputName = null;
				this.m_addFriendSourceType = COM_ADD_FRIEND_TYPE.COM_ADD_FRIEND_NULL;
				this.m_addFriendUseHeroId = -1;
				Singleton<CUIManager>.instance.CloseForm(CFriendContoller.VerifyFriendFormPath);
			}
		}

		public class TabMgr
		{
			public class TabElement
			{
				public CFriendView.Tab tab;

				public string content;

				public TabElement(CFriendView.Tab tab, string content)
				{
					this.tab = tab;
					this.content = content;
				}
			}

			private ListView<CFriendView.TabMgr.TabElement> tabElements = new ListView<CFriendView.TabMgr.TabElement>();

			private List<string> tabTextList = new List<string>();

			public void Clear()
			{
				this.tabElements.Clear();
				this.tabTextList.Clear();
			}

			public void Add(CFriendView.Tab tab, string text)
			{
				this.tabElements.Add(new CFriendView.TabMgr.TabElement(tab, text));
			}

			public int GetIndex(CFriendView.Tab tabValue)
			{
				for (int i = 0; i < this.tabElements.Count; i++)
				{
					CFriendView.TabMgr.TabElement tabElement = this.tabElements[i];
					if (tabElement != null && tabElement.tab == tabValue)
					{
						return i;
					}
				}
				return -1;
			}

			public CFriendView.TabMgr.TabElement GetTabElement(int index)
			{
				if (index >= 0 && index < this.tabElements.Count)
				{
					return this.tabElements[index];
				}
				return null;
			}

			public List<string> GetTabTextList()
			{
				this.tabTextList.Clear();
				for (int i = 0; i < this.tabElements.Count; i++)
				{
					CFriendView.TabMgr.TabElement tabElement = this.tabElements[i];
					if (tabElement != null && !this.tabTextList.Contains(tabElement.content))
					{
						this.tabTextList.Add(tabElement.content);
					}
				}
				return this.tabTextList;
			}
		}

		private CUIFormScript friendform;

		public GameObject imgNode;

		public GameObject bgLineNode;

		public GameObject friendListNode;

		public CUIListScript friendListCom;

		private GameObject addFriendBtnGameObject;

		private Vector2 friendListSizeDeltaOrg;

		private Vector2 friendListPosOrg;

		private Rect friendListRectOrg;

		private GameObject info_node;

		private CUIListScript tablistScript;

		public CFriendView.AddFriendView addFriendView;

		public CFriendView.Verfication verficationView;

		public IntimacyRelationView intimacyRelationView;

		public CFriendMentorTaskView mentorTaskView;

		private GameObject pandoraSocial_node;

		public Text ifnoText;

		private Text btnText;

		private GameObject sns_invite_btn;

		private GameObject sns_share_switch;

		private GameObject sns_add_switch;

		private GameObject rule_btn_node;

		private GameObject mycard_btn_node;

		private GameObject lbs_node;

		private GameObject m_QQboxBtn;

		private GameObject m_IntimacyRelationBtn;

		private GameObject m_blackListBtn;

		private GameObject m_requestListBtn;

		private Toggle localtionToggle;

		private Toggle nanToggle;

		private Toggle nvToggle;

		private Button lbsRefreshBtn;

		private FriendRecruitView friendRecruitView = new FriendRecruitView();

		private CFriendView.TabMgr tabMgr = new CFriendView.TabMgr();

		private CFriendModel.FriendType m_listFriendType;

		private FriendShower.ItemType m_listItemType;

		private int m_lastMCLevel = -1;

		private bool m_isPandoraActived;

		private CFriendView.Tab _tab;

		public CFriendView.Tab CurTab
		{
			get
			{
				return this._tab;
			}
			set
			{
				if (this._tab == value)
				{
					return;
				}
				this._tab = value;
				this.bgLineNode.CustomSetActive(true);
				this.imgNode.CustomSetActive(true);
				this.sns_invite_btn.CustomSetActive(false);
				this.lbs_node.CustomSetActive(false);
				this.addFriendBtnGameObject.CustomSetActive(!this.m_isPandoraActived);
				this.m_IntimacyRelationBtn.CustomSetActive(false);
				this.friendRecruitView.Hide();
				this.Refresh_List(this.CurTab);
				this.Refresh_SnsSwitch();
				this.pandoraSocial_node.CustomSetActive(false);
				this.mycard_btn_node.CustomSetActive(false);
				if (this._tab == CFriendView.Tab.Friend)
				{
					this.addFriendBtnGameObject.CustomSetActive(!CSysDynamicBlock.bFriendBlocked && !this.m_isPandoraActived);
				}
				if (this.m_QQboxBtn != null)
				{
					bool bActive = false;
					ulong curTime = (ulong)((long)CRoleInfo.GetCurrentUTCTime());
					if (MonoSingleton<BannerImageSys>.GetInstance().QQBOXInfo.isTimeValid(curTime))
					{
						bActive = true;
					}
					if (ApolloConfig.platform == ApolloPlatform.QQ || ApolloConfig.platform == ApolloPlatform.WTLogin)
					{
						if (this._tab == CFriendView.Tab.Friend_SNS)
						{
							this.m_QQboxBtn.CustomSetActive(bActive);
						}
						else
						{
							this.m_QQboxBtn.CustomSetActive(false);
						}
					}
					else
					{
						this.m_QQboxBtn.CustomSetActive(false);
					}
					if (CSysDynamicBlock.bLobbyEntryBlocked)
					{
						this.m_QQboxBtn.CustomSetActive(false);
					}
				}
				CFriendModel model = Singleton<CFriendContoller>.GetInstance().model;
				this.m_blackListBtn.CustomSetActive(this._tab == CFriendView.Tab.Friend);
				this.m_requestListBtn.CustomSetActive(this._tab == CFriendView.Tab.Friend);
				this.m_IntimacyRelationBtn.CustomSetActive(this._tab == CFriendView.Tab.Friend);
				switch (this._tab)
				{
				case CFriendView.Tab.Friend_SNS:
				{
					if (this.ifnoText != null)
					{
						this.ifnoText.set_text(Singleton<CTextManager>.instance.GetText("Friend_NoFriend_Tip"));
					}
					int dataCount = model.GetDataCount(CFriendModel.FriendType.SNS);
					if (this.info_node != null)
					{
						this.info_node.CustomSetActive(dataCount == 0);
					}
					if (this.sns_invite_btn != null)
					{
						this.sns_invite_btn.CustomSetActive(!CSysDynamicBlock.bSocialBlocked);
					}
					if (this.rule_btn_node != null)
					{
						this.rule_btn_node.CustomSetActive(false);
					}
					return;
				}
				case CFriendView.Tab.Friend:
				{
					if (this.ifnoText != null)
					{
						this.ifnoText.set_text(Singleton<CTextManager>.instance.GetText("Friend_NoFriend_Tip"));
					}
					int dataCount2 = model.GetDataCount(CFriendModel.FriendType.GameFriend);
					this.info_node.CustomSetActive(dataCount2 == 0);
					this.rule_btn_node.CustomSetActive(true);
					this.m_IntimacyRelationBtn.CustomSetActive(true);
					if (CSysDynamicBlock.bLobbyEntryBlocked)
					{
						CUICommonSystem.SetObjActive(this.rule_btn_node, false);
						CUICommonSystem.SetObjActive(this.m_IntimacyRelationBtn, false);
					}
					return;
				}
				case CFriendView.Tab.Mentor:
					if (this.rule_btn_node != null)
					{
						this.rule_btn_node.CustomSetActive(true);
					}
					if (CSysDynamicBlock.bLobbyEntryBlocked)
					{
						CUICommonSystem.SetObjActive(this.rule_btn_node, false);
					}
					return;
				case CFriendView.Tab.Friend_LBS:
				{
					string text = string.Empty;
					if (!string.IsNullOrEmpty(model.searchLBSZero))
					{
						text = model.searchLBSZero;
					}
					else
					{
						text = Singleton<CTextManager>.instance.GetText("Friend_NoLBSList_Tip");
					}
					if (this.ifnoText != null)
					{
						this.ifnoText.set_text(text);
					}
					this.rule_btn_node.CustomSetActive(false);
					this.sns_invite_btn.CustomSetActive(false);
					this.lbs_node.CustomSetActive(true);
					this.addFriendBtnGameObject.CustomSetActive(false);
					this.m_QQboxBtn.CustomSetActive(false);
					this.SyncGenderToggleState();
					this.SyncLBSShareBtnState();
					int num = 0;
					ListView<CSDT_LBS_USER_INFO> currentLBSList = Singleton<CFriendContoller>.instance.model.GetCurrentLBSList();
					if (currentLBSList != null)
					{
						num = currentLBSList.Count;
					}
					this.info_node.CustomSetActive(num == 0);
					return;
				}
				case CFriendView.Tab.Friend_Recruit:
					this.bgLineNode.CustomSetActive(false);
					this.imgNode.CustomSetActive(false);
					this.rule_btn_node.CustomSetActive(false);
					this.sns_invite_btn.CustomSetActive(false);
					this.lbs_node.CustomSetActive(false);
					this.addFriendBtnGameObject.CustomSetActive(false);
					this.m_QQboxBtn.CustomSetActive(false);
					this.info_node.CustomSetActive(false);
					this.friendRecruitView.Show();
					return;
				case CFriendView.Tab.Find_SocialFriend:
					this.info_node.CustomSetActive(false);
					this.imgNode.CustomSetActive(false);
					this.rule_btn_node.CustomSetActive(false);
					this.pandoraSocial_node.CustomSetActive(true);
					this.mycard_btn_node.CustomSetActive(true);
					MonoSingleton<PandroaSys>.GetInstance().OnPandoraTabClick(PandroaSys.PandoraModuleType.friend, 0);
					return;
				}
				this.info_node.CustomSetActive(false);
				this.rule_btn_node.CustomSetActive(false);
			}
		}

		public CFriendView()
		{
			this.addFriendView = new CFriendView.AddFriendView();
			this.verficationView = new CFriendView.Verfication();
			this.intimacyRelationView = new IntimacyRelationView();
			this.mentorTaskView = new CFriendMentorTaskView();
		}

		public CFriendView.TabMgr GetTabMgr()
		{
			return this.tabMgr;
		}

		public void OpenForm(CUIEvent uiEvent)
		{
			this.friendform = Singleton<CUIManager>.GetInstance().OpenForm(CFriendContoller.FriendFormPath, false, true);
			GameObject gameObject = this.friendform.gameObject;
			GameObject widget = this.friendform.GetWidget(11);
			this.pandoraSocial_node = this.friendform.GetWidget(15);
			if (widget != null)
			{
				RectTransform rectTransform = widget.transform as RectTransform;
				this.friendListSizeDeltaOrg = rectTransform.sizeDelta;
				this.friendListPosOrg = rectTransform.anchoredPosition;
				this.friendListRectOrg = rectTransform.rect;
			}
			this.imgNode = gameObject.transform.Find("node/Image").gameObject;
			this.bgLineNode = gameObject.transform.Find("node/Bg").gameObject;
			this.friendListNode = gameObject.transform.Find("node/Image/FriendList").gameObject;
			this.friendListNode.CustomSetActive(true);
			this.friendListCom = this.friendListNode.GetComponent<CUIListScript>();
			this.addFriendBtnGameObject = Utility.FindChild(gameObject, "node/Buttons/ButtonsOther/Add");
			this.info_node = gameObject.transform.Find("node/Image/info_node").gameObject;
			this.info_node.CustomSetActive(false);
			this.ifnoText = Utility.GetComponetInChild<Text>(gameObject, "node/Image/info_node/Text");
			this.ifnoText.set_text(Singleton<CTextManager>.instance.GetText("Friend_NoFriend_Tip"));
			string text = Singleton<CTextManager>.instance.GetText("FriendAdd_Tab_QQ");
			if (Singleton<ApolloHelper>.GetInstance().CurPlatform == ApolloPlatform.Wechat)
			{
				text = Singleton<CTextManager>.instance.GetText("FriendAdd_Tab_Weixin");
			}
			GameObject gameObject2 = gameObject.transform.Find("TopCommon/Panel_Menu/List").gameObject;
			this.tablistScript = gameObject2.GetComponent<CUIListScript>();
			CFriendView.Tab curTab = CSysDynamicBlock.bSocialBlocked ? CFriendView.Tab.Friend : CFriendView.Tab.Friend_SNS;
			MonoSingleton<PandroaSys>.GetInstance().InitPandoraTab(PandroaSys.PandoraModuleType.friend, this.pandoraSocial_node.transform);
			if (!CSysDynamicBlock.bSocialBlocked)
			{
				this.tabMgr.Add(CFriendView.Tab.Friend_SNS, text);
				this.tabMgr.Add(CFriendView.Tab.Friend, UT.GetText("Friend_Title_List"));
				string pandoraTabName = MonoSingleton<PandroaSys>.GetInstance().GetPandoraTabName(PandroaSys.PandoraModuleType.friend, 0);
				this.m_isPandoraActived = !string.IsNullOrEmpty(pandoraTabName);
				if (!string.IsNullOrEmpty(pandoraTabName))
				{
					this.tabMgr.Add(CFriendView.Tab.Find_SocialFriend, pandoraTabName);
				}
				this.tabMgr.Add(CFriendView.Tab.Friend_LBS, "附近的人");
				this.tabMgr.Add(CFriendView.Tab.Mentor, UT.GetText("Mentor_Title"));
			}
			else
			{
				this.tabMgr.Add(CFriendView.Tab.Friend, UT.GetText("Friend_Title_List"));
				this.tabMgr.Add(CFriendView.Tab.Mentor, UT.GetText("Mentor_Title"));
			}
			List<string> tabTextList = this.tabMgr.GetTabTextList();
			UT.SetTabList(tabTextList, 0, this.tablistScript);
			this.btnText = Utility.GetComponetInChild<Text>(gameObject, "node/Buttons/ButtonsOther/Invite/Text");
			this.sns_invite_btn = gameObject.transform.Find("node/Buttons/ButtonsOther/Invite").gameObject;
			string text2 = Singleton<CTextManager>.instance.GetText("FriendAdd_Invite_Btn_QQ");
			if (Singleton<ApolloHelper>.GetInstance().CurPlatform == ApolloPlatform.Wechat)
			{
				text2 = Singleton<CTextManager>.instance.GetText("FriendAdd_Invite_Btn_Weixin");
			}
			this.btnText.set_text(text2);
			this.sns_invite_btn.CustomSetActive(false);
			this.rule_btn_node = gameObject.transform.Find("btnGroup/btnRule").gameObject;
			this.mycard_btn_node = gameObject.transform.Find("btnGroup/btnMyCard").gameObject;
			this.m_blackListBtn = gameObject.transform.Find("node/Buttons/BtnBlackList").gameObject;
			this.m_requestListBtn = gameObject.transform.Find("node/Buttons/ButtonsOther/BtnRequestList").gameObject;
			this.lbs_node = gameObject.transform.Find("node/LBSNode").gameObject;
			this.m_QQboxBtn = Utility.FindChild(gameObject, "node/Buttons/ButtonsOther/QQBoxBtn");
			this.m_IntimacyRelationBtn = Utility.FindChild(gameObject, "node/Buttons/ButtonsOther/Relation");
			this.sns_share_switch = Utility.FindChild(gameObject, "node/SnsNtfNode/SnsToggle");
			this.sns_add_switch = Utility.FindChild(gameObject, "node/SnsNtfNode/AddToggle");
			this.sns_share_switch.CustomSetActive(false);
			this.localtionToggle = Utility.FindChild(gameObject, "node/LBSNode/location").GetComponent<Toggle>();
			this.nanToggle = Utility.FindChild(gameObject, "node/LBSNode/nan").GetComponent<Toggle>();
			this.nvToggle = Utility.FindChild(gameObject, "node/LBSNode/nv").GetComponent<Toggle>();
			this.lbsRefreshBtn = Utility.FindChild(gameObject, "node/LBSNode/Add").GetComponent<Button>();
			this.friendRecruitView.Init(Utility.FindChild(gameObject, "node/zhaomu_node").gameObject);
			Text componetInChild = Utility.GetComponetInChild<Text>(this.sns_share_switch, "Label");
			if (Singleton<ApolloHelper>.GetInstance().CurPlatform == ApolloPlatform.QQ)
			{
				if (componetInChild != null)
				{
					componetInChild.set_text(Singleton<CTextManager>.instance.GetText("Friend_SNS_NTF_Switch_Tips_1"));
				}
			}
			else if (componetInChild != null)
			{
				componetInChild.set_text(Singleton<CTextManager>.instance.GetText("Friend_SNS_NTF_Switch_Tips_2"));
			}
			this.sns_add_switch.CustomSetActive(false);
			this.tablistScript.m_alwaysDispatchSelectedChangeEvent = true;
			this.tablistScript.SelectElement(0, true);
			this.tablistScript.m_alwaysDispatchSelectedChangeEvent = false;
			this._tab = CFriendView.Tab.None;
			this.Refresh_Tab();
			this.CurTab = curTab;
			CUIListElementScript elemenet = this.tablistScript.GetElemenet(this.tabMgr.GetIndex(CFriendView.Tab.Friend_Recruit));
			if (elemenet != null)
			{
				Singleton<CUINewFlagSystem>.GetInstance().AddNewFlag(elemenet.gameObject, enNewFlagKey.New_Friend_Recruit_V15, enNewFlagPos.enTopRight, 1f, 0f, 0f, enNewFlagType.enNewFlag);
			}
			elemenet = this.tablistScript.GetElemenet(this.tabMgr.GetIndex(CFriendView.Tab.Find_SocialFriend));
			if (elemenet != null)
			{
				Singleton<CUINewFlagSystem>.GetInstance().AddNewFlag(elemenet.gameObject, enNewFlagKey.New_Friend_FindSocialFriend_V15, enNewFlagPos.enTopRight, 1f, 0f, 0f, enNewFlagType.enNewFlag);
			}
		}

		public void OpenSearchForm(int searchType)
		{
			this.addFriendView.Init(searchType);
		}

		private void On_SearchFriend(CUIEvent uiEvent)
		{
			this.addFriendView.On_SearchFriend(uiEvent);
		}

		public void OpenMentorRequestForm()
		{
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CFriendContoller.MentorRequestListFormPath, false, true);
		}

		public void Show_Search_Result(COMDT_FRIEND_INFO info)
		{
			if (this.addFriendView == null || !this.addFriendView.bShow)
			{
				return;
			}
			this.addFriendView.Record_SearchFriend(info);
			this.addFriendView.Show_Search_Result(info);
		}

		public void OpenRelationForm()
		{
			this.intimacyRelationView.Open();
		}

		public bool IsActive()
		{
			return this.friendform != null;
		}

		public void Update()
		{
			if (this.verficationView != null)
			{
				this.verficationView.Update();
			}
			if (this.lbsRefreshBtn != null && Singleton<CFriendContoller>.instance.startCooldownTimestamp > 0uL)
			{
				uint num = (uint)(Singleton<FrameSynchr>.GetInstance().LogicFrameTick - Singleton<CFriendContoller>.instance.startCooldownTimestamp);
				Transform transform = this.lbsRefreshBtn.transform.Find("Text");
				if (transform == null)
				{
					return;
				}
				Text component = transform.GetComponent<Text>();
				if (component == null)
				{
					return;
				}
				int num2 = (int)(10000u - num);
				if (num2 > 0)
				{
					CUICommonSystem.SetButtonEnableWithShader(this.lbsRefreshBtn, false, true);
					this.lbsRefreshBtn.enabled = false;
					component.set_text(string.Format(Singleton<CTextManager>.instance.GetText("LBS_Refresh_CDInfo"), (num2 / 1000).ToString()));
				}
				else
				{
					Singleton<CFriendContoller>.instance.startCooldownTimestamp = 0uL;
					component.set_text(Singleton<CTextManager>.instance.GetText("LBS_Refresh_CDInfoNormal"));
					CUICommonSystem.SetButtonEnableWithShader(this.lbsRefreshBtn, Singleton<CFriendContoller>.instance.model.EnableShareLocation, true);
				}
			}
		}

		public void CloseForm()
		{
			this.Clear();
			MonoSingleton<PandroaSys>.GetInstance().ClosePandoraTabWindow(PandroaSys.PandoraModuleType.friend);
		}

		public void Clear()
		{
			if (this.tablistScript != null)
			{
				CUICommonSystem.DelRedDot(this.tablistScript.GetElemenet(0).gameObject);
				CUICommonSystem.DelRedDot(this.tablistScript.GetElemenet(1).gameObject);
				if (!CSysDynamicBlock.bSocialBlocked)
				{
					CUICommonSystem.DelRedDot(this.tablistScript.GetElemenet(2).gameObject);
				}
			}
			this.tablistScript = null;
			this.bgLineNode = null;
			this.friendListNode = null;
			this.friendListCom = null;
			this.imgNode = null;
			this.bgLineNode = null;
			this.addFriendBtnGameObject = null;
			this.info_node = null;
			this.btnText = null;
			this.ifnoText = null;
			this.friendform = null;
			this.lbs_node = null;
			this.m_QQboxBtn = null;
			this.sns_invite_btn = null;
			this.sns_share_switch = null;
			this.sns_add_switch = null;
			this.rule_btn_node = null;
			this.m_IntimacyRelationBtn = null;
			this.localtionToggle = (this.nanToggle = (this.nvToggle = null));
			this.lbsRefreshBtn = null;
			if (this.tabMgr != null)
			{
				this.tabMgr.Clear();
			}
			if (this.friendRecruitView != null)
			{
				this.friendRecruitView.Clear();
			}
			if (this.intimacyRelationView != null)
			{
				this.intimacyRelationView.Clear();
			}
			if (this.verficationView != null)
			{
				this.verficationView.Clear();
			}
			if (this.addFriendView != null)
			{
				this.addFriendView.Clear();
			}
			if (this.mentorTaskView != null)
			{
				this.mentorTaskView.Clear();
			}
		}

		public void SyncGenderToggleState()
		{
			if (this.nanToggle != null)
			{
				this.nanToggle.set_isOn((Singleton<CFriendContoller>.instance.model.fileter & 1u) != 0u);
			}
			if (this.nvToggle != null)
			{
				this.nvToggle.set_isOn((Singleton<CFriendContoller>.instance.model.fileter & 2u) != 0u);
			}
		}

		public void SyncLBSShareBtnState()
		{
			bool enableShareLocation = Singleton<CFriendContoller>.instance.model.EnableShareLocation;
			if (this.lbsRefreshBtn != null)
			{
				CUICommonSystem.SetButtonEnableWithShader(this.lbsRefreshBtn, enableShareLocation, true);
			}
			if (this.localtionToggle != null)
			{
				this.localtionToggle.set_isOn(enableShareLocation);
			}
		}

		public void On_Tab_Change(int index)
		{
			CFriendView.TabMgr.TabElement tabElement = this.tabMgr.GetTabElement(index);
			if (tabElement == null)
			{
				return;
			}
			this.CurTab = tabElement.tab;
			if (this.tablistScript == null)
			{
				return;
			}
			switch (this.CurTab)
			{
			case CFriendView.Tab.Friend_LBS:
			{
				CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
				if (masterRoleInfo != null && !masterRoleInfo.IsClientBitsSet(4))
				{
					Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("Friend_LBSFristTimeOpen_Tip"), enUIEventID.Friend_LBS_NoShare, enUIEventID.None, false);
					masterRoleInfo.SetClientBits(4, true, true);
				}
				break;
			}
			case CFriendView.Tab.Friend_Recruit:
				Singleton<CUINewFlagSystem>.GetInstance().DelNewFlag(this.tablistScript.GetElemenet(this.tabMgr.GetIndex(this.CurTab)).gameObject, enNewFlagKey.New_Friend_Recruit_V15, true);
				Singleton<CBattleGuideManager>.GetInstance().OpenBannerDlgByBannerGuideId(29u, null, false);
				break;
			case CFriendView.Tab.Find_SocialFriend:
				Singleton<CUINewFlagSystem>.GetInstance().SetNewFlagForSocialFriend(true);
				Singleton<CUINewFlagSystem>.GetInstance().DelNewFlag(this.tablistScript.GetElemenet(this.tabMgr.GetIndex(this.CurTab)).gameObject, enNewFlagKey.New_Friend_FindSocialFriend_V15, true);
				Singleton<CBattleGuideManager>.GetInstance().OpenBannerDlgByBannerGuideId(31u, null, false);
				break;
			}
		}

		public void Refresh()
		{
			this.Refresh_Tab();
			this.Refresh_SnsSwitch();
			this.Refresh_List(this.CurTab);
			if (this.addFriendView != null && this.addFriendView.bShow)
			{
				this.addFriendView.Refresh(-1);
			}
		}

		public void Refresh_SnsSwitch()
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return;
			}
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CFriendContoller.MentorRequestListFormPath);
			if (form != null)
			{
				GameObject widget = form.GetWidget(1);
				bool isOn = CFriendModel.IsOnSnsSwitch(masterRoleInfo.snsSwitchBits, COM_REFUSE_TYPE.COM_REFUSE_TYPE_MASTERREQ);
				widget.GetComponent<Toggle>().set_isOn(isOn);
				this.RefreshMentorReqList();
			}
			if (this.sns_share_switch == null)
			{
				return;
			}
			if (this.sns_add_switch == null)
			{
				return;
			}
			this.sns_share_switch.CustomSetActive(false);
			this.sns_add_switch.CustomSetActive(false);
			CFriendContoller instance = Singleton<CFriendContoller>.GetInstance();
			CUIFormScript singleListForm = instance.GetSingleListForm();
			if (singleListForm)
			{
				GameObject widget2 = singleListForm.GetWidget(1);
				enFriendSingleListType singleListType = instance.singleListType;
				if (singleListType != enFriendSingleListType.requestList)
				{
					if (singleListType == enFriendSingleListType.blackList)
					{
						widget2.CustomSetActive(false);
					}
				}
				else
				{
					widget2.CustomSetActive(true);
					bool isOn2 = CFriendModel.IsOnSnsSwitch(masterRoleInfo.snsSwitchBits, COM_REFUSE_TYPE.COM_REFUSE_TYPE_ADDFRIEND);
					widget2.transform.Find("AddToggle").GetComponent<Toggle>().set_isOn(isOn2);
				}
			}
			if (CSysDynamicBlock.bSocialBlocked)
			{
				return;
			}
			if (this.CurTab == CFriendView.Tab.Friend_SNS)
			{
				this.sns_share_switch.CustomSetActive(true);
				bool isOn3 = CFriendModel.IsOnSnsSwitch(masterRoleInfo.snsSwitchBits, COM_REFUSE_TYPE.COM_REFUSE_TYPE_DONOTE_AND_REC);
				this.sns_share_switch.GetComponent<Toggle>().set_isOn(isOn3);
			}
		}

		public void Refresh_Tab()
		{
			if (this.friendform == null)
			{
				return;
			}
			CFriendModel model = Singleton<CFriendContoller>.GetInstance().model;
			int dataCount = model.GetDataCount(CFriendModel.FriendType.RequestFriend);
			int dataCount2 = model.GetDataCount(CFriendModel.FriendType.MentorRequestList);
			bool flag = model.FRData.HasRedDot();
			GameObject gameObject = Utility.FindChild(this.friendform.gameObject, "node/Buttons/ButtonsOther/Relation");
			if (gameObject != null)
			{
				int index = this.tabMgr.GetIndex(CFriendView.Tab.Friend);
				if (flag)
				{
					CUICommonSystem.AddRedDot(gameObject, enRedDotPos.enTopRight, 0, 0, 0);
					CUICommonSystem.AddRedDot(this.tablistScript.GetElemenet(index).gameObject, enRedDotPos.enTopRight, 0, 0, 0);
				}
				else
				{
					CUICommonSystem.DelRedDot(gameObject);
					CUICommonSystem.DelRedDot(this.tablistScript.GetElemenet(index).gameObject);
				}
			}
			bool flag2 = Singleton<CTaskSys>.instance.IsMentorTaskRedDot();
			int index2 = this.tabMgr.GetIndex(CFriendView.Tab.Mentor);
			GameObject gameObject2 = Utility.FindChild(this.friendform.gameObject, "node/mentorButtons/BtnMentorQuest");
			if (gameObject2 != null)
			{
				if (flag2)
				{
					CUICommonSystem.AddRedDot(gameObject2, enRedDotPos.enTopRight, 0, 0, 0);
				}
				else
				{
					CUICommonSystem.DelRedDot(gameObject2);
				}
			}
			GameObject widget = this.friendform.GetWidget(16);
			int index3 = this.tabMgr.GetIndex(CFriendView.Tab.Friend);
			if (dataCount > 0)
			{
				CUICommonSystem.AddRedDot(widget, enRedDotPos.enTopRight, dataCount, 0, 0);
				if (index3 != -1)
				{
					CUICommonSystem.AddRedDot(this.tablistScript.GetElemenet(index3).gameObject, enRedDotPos.enTopRight, dataCount, 0, 0);
				}
			}
			else
			{
				CUICommonSystem.DelRedDot(widget);
				if (index3 != -1)
				{
					CUICommonSystem.DelRedDot(this.tablistScript.GetElemenet(index3).gameObject);
				}
			}
			if (this.friendform != null)
			{
				Transform transform = this.friendform.GetWidget(3).transform.Find("BtnRequestList");
				int index4 = this.tabMgr.GetIndex(CFriendView.Tab.Mentor);
				if (dataCount2 > 0)
				{
					CUICommonSystem.AddRedDot(transform.gameObject, enRedDotPos.enTopRight, dataCount2, 0, 0);
					if (index4 != -1)
					{
						CUICommonSystem.AddRedDot(this.tablistScript.GetElemenet(index4).gameObject, enRedDotPos.enTopRight, dataCount2, 0, 0);
					}
				}
				else
				{
					CUICommonSystem.DelRedDot(transform.gameObject);
					if (index4 != -1)
					{
						CUICommonSystem.DelRedDot(this.tablistScript.GetElemenet(index4).gameObject);
					}
				}
				if (flag2)
				{
					CUICommonSystem.AddRedDot(this.tablistScript.GetElemenet(index2).gameObject, enRedDotPos.enTopRight, 0, 0, 0);
				}
			}
		}

		public void Refresh_SingleList()
		{
			CFriendContoller instance = Singleton<CFriendContoller>.GetInstance();
			CUIFormScript singleListForm = instance.GetSingleListForm();
			if (singleListForm)
			{
				CUIListScript component = singleListForm.GetWidget(11).GetComponent<CUIListScript>();
				GameObject widget = singleListForm.GetWidget(3);
				enFriendSingleListType singleListType = instance.singleListType;
				if (singleListType != enFriendSingleListType.requestList)
				{
					if (singleListType == enFriendSingleListType.blackList)
					{
						List<CFriendModel.stBlackName> blackList = Singleton<CFriendContoller>.instance.model.GetBlackList();
						this._refresh_black_list(component, blackList);
						component.gameObject.CustomSetActive(true);
						if (widget != null && blackList != null)
						{
							widget.CustomSetActive(blackList.get_Count() == 0);
						}
					}
				}
				else
				{
					ListView<COMDT_FRIEND_INFO> list = Singleton<CFriendContoller>.instance.model.GetList(CFriendModel.FriendType.RequestFriend);
					this._refresh_list(component, list, FriendShower.ItemType.Request, false, CFriendModel.FriendType.RequestFriend);
					component.gameObject.CustomSetActive(true);
					if (widget != null && list != null)
					{
						widget.CustomSetActive(list.Count == 0);
					}
				}
			}
		}

		public void Refresh_List(CFriendView.Tab type)
		{
			if (this.friendListCom == null)
			{
				return;
			}
			GameObject widget = this.friendform.GetWidget(6);
			GameObject widget2 = this.friendform.GetWidget(10);
			widget2.CustomSetActive(false);
			GameObject widget3 = this.friendform.GetWidget(1);
			GameObject widget4 = this.friendform.GetWidget(3);
			GameObject widget5 = this.friendform.GetWidget(5);
			GameObject widget6 = this.friendform.GetWidget(14);
			GameObject widget7 = this.friendform.GetWidget(13);
			widget3.CustomSetActive(type != CFriendView.Tab.Mentor && type != CFriendView.Tab.Find_SocialFriend);
			widget4.CustomSetActive(type == CFriendView.Tab.Mentor);
			widget5.CustomSetActive(false);
			GameObject widget8 = this.friendform.GetWidget(11);
			this.ifnoText = Utility.GetComponetInChild<Text>(widget, "info_node/Text");
			this.ifnoText.set_text(Singleton<CTextManager>.instance.GetText("Friend_NoFriend_Tip"));
			if (widget8 != null)
			{
				RectTransform rectTransform = widget8.transform as RectTransform;
				rectTransform.sizeDelta = this.friendListSizeDeltaOrg;
				rectTransform.anchoredPosition = this.friendListPosOrg;
				CUIListScript component = widget8.GetComponent<CUIListScript>();
				if (component != null)
				{
					component.EnableExtraContent(false);
				}
			}
			widget.CustomSetActive(true);
			this.Refresh_SingleList();
			switch (type)
			{
			case CFriendView.Tab.Friend_SNS:
			{
				ListView<COMDT_FRIEND_INFO> list = Singleton<CFriendContoller>.instance.model.GetList(CFriendModel.FriendType.SNS);
				this._refresh_list(this.friendListCom, list, FriendShower.ItemType.Normal, true, CFriendModel.FriendType.SNS);
				this.friendListCom.gameObject.CustomSetActive(true);
				if (this.info_node != null && list != null)
				{
					this.info_node.CustomSetActive(list.Count == 0);
				}
				break;
			}
			case CFriendView.Tab.Friend:
			{
				ListView<COMDT_FRIEND_INFO> list2 = Singleton<CFriendContoller>.instance.model.GetList(CFriendModel.FriendType.GameFriend);
				this._refresh_list(this.friendListCom, list2, FriendShower.ItemType.Normal, false, CFriendModel.FriendType.GameFriend);
				this.friendListCom.gameObject.CustomSetActive(true);
				if (this.info_node != null && list2 != null)
				{
					this.info_node.CustomSetActive(list2.Count == 0);
				}
				break;
			}
			case CFriendView.Tab.Mentor:
			{
				this.ifnoText.set_text(Singleton<CTextManager>.instance.GetText("Mentor_NoInfoTips"));
				enMentorState mentorState = CFriendContoller.GetMentorState(-1, null);
				if (mentorState == enMentorState.None)
				{
					widget5.CustomSetActive(false);
					widget.CustomSetActive(true);
					if (this.info_node != null)
					{
						this.info_node.CustomSetActive(true);
					}
					return;
				}
				bool flag = mentorState == enMentorState.IWantMentor || mentorState == enMentorState.IWantApprentice;
				bool flag2 = mentorState == enMentorState.IHasMentor || mentorState == enMentorState.IHasApprentice;
				bool flag3 = Singleton<CFriendContoller>.GetInstance().HasMentor(null);
				widget5.CustomSetActive(flag && !flag3);
				widget.CustomSetActive(flag2 || flag3);
				if (flag && !flag3)
				{
					uint dwConfValue = GameDataMgr.globalInfoDatabin.GetDataByKey(270u).dwConfValue;
					uint dwConfValue2 = GameDataMgr.globalInfoDatabin.GetDataByKey(269u).dwConfValue;
					uint bLogicGrade = (uint)CLadderSystem.GetGradeDataByShowGrade((int)CFriendContoller.GetMentorGradeLimit()).bLogicGrade;
					string text = string.Empty;
					ResRankGradeConf gradeDataByLogicGrade = CLadderSystem.GetGradeDataByLogicGrade((int)bLogicGrade);
					if (gradeDataByLogicGrade != null)
					{
						text = gradeDataByLogicGrade.szGradeDesc;
					}
					GameObject widget9 = this.friendform.GetWidget(8);
					GameObject widget10 = this.friendform.GetWidget(9);
					widget9.GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("ApprenticeIntroRequire", new string[]
					{
						dwConfValue.ToString()
					}));
					widget10.GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("MentorIntroRequire", new string[]
					{
						dwConfValue2.ToString(),
						text
					}));
					widget6.GetComponent<Text>().set_text(Singleton<CUIManager>.GetInstance().GetRuleTextContent(19));
					widget7.GetComponent<Text>().set_text(Singleton<CUIManager>.GetInstance().GetRuleTextContent(18));
				}
				else
				{
					RectTransform rectTransform2 = widget.transform.Find("MentorSubTab") as RectTransform;
					int elementAmount = Singleton<CFriendContoller>.GetInstance().RefreshMentorTabData();
					CUIListScript component2 = rectTransform2.gameObject.GetComponent<CUIListScript>();
					component2.gameObject.CustomSetActive(true);
					component2.SetElementAmount(elementAmount);
					int num = 0;
					for (int i = 0; i < CFriendContoller.s_mentorTabStr.Length; i++)
					{
						if ((CFriendContoller.MentorTabMask & 1 << i) != 0)
						{
							CUIListElementScript elemenet = component2.GetElemenet(num);
							Transform transform = elemenet.transform.Find("Text");
							if (transform != null)
							{
								Text component3 = transform.GetComponent<Text>();
								component3.set_text(CFriendContoller.s_mentorTabStr[i]);
							}
							elemenet.SetDataTag(i.ToString());
							num++;
						}
					}
					if (num != 0)
					{
						component2.SelectElement(0, true);
					}
					RectTransform rectTransform3 = widget2.transform as RectTransform;
					RectTransform rectTransform4 = widget8.transform as RectTransform;
					rectTransform4.sizeDelta = new Vector2(this.friendListSizeDeltaOrg.x, this.friendListSizeDeltaOrg.y - rectTransform3.rect.height);
					rectTransform4.anchoredPosition = new Vector2(this.friendListPosOrg.x, this.friendListPosOrg.y - rectTransform3.rect.height / 2f + 20f);
					widget8.GetComponent<CUIListScript>().m_scrollAreaSize = new Vector2(this.friendListRectOrg.width, this.friendListRectOrg.height - rectTransform3.rect.height);
				}
				GameObject gameObject = widget4.transform.Find("BtnIWantMentor").gameObject;
				GameObject gameObject2 = widget4.transform.Find("BtnIWantApprentice").gameObject;
				gameObject.CustomSetActive(true);
				gameObject2.CustomSetActive(mentorState == enMentorState.IWantApprentice || mentorState == enMentorState.IHasApprentice);
				break;
			}
			case CFriendView.Tab.Friend_LBS:
			{
				CFriendModel model = Singleton<CFriendContoller>.instance.model;
				ListView<CSDT_LBS_USER_INFO> currentLBSList = model.GetCurrentLBSList();
				this._refresh_LBS_list(this.friendListCom, currentLBSList);
				this.friendListCom.gameObject.CustomSetActive(Singleton<CFriendContoller>.instance.model.EnableShareLocation);
				if (this.info_node != null)
				{
					if (currentLBSList == null)
					{
						this.info_node.CustomSetActive(true);
					}
					else
					{
						this.info_node.CustomSetActive(currentLBSList.Count == 0);
					}
				}
				break;
			}
			case CFriendView.Tab.Friend_Recruit:
				widget.CustomSetActive(false);
				break;
			case CFriendView.Tab.Find_SocialFriend:
				this.info_node.CustomSetActive(false);
				widget.CustomSetActive(false);
				this.rule_btn_node.CustomSetActive(false);
				this.pandoraSocial_node.CustomSetActive(true);
				this.mycard_btn_node.CustomSetActive(true);
				MonoSingleton<PandroaSys>.GetInstance().OnPandoraTabClick(PandroaSys.PandoraModuleType.friend, 0);
				break;
			}
		}

		private void RefuseAllInvalidMentorRequest()
		{
			ListView<COMDT_FRIEND_INFO> list = Singleton<CFriendContoller>.GetInstance().model.GetList(CFriendModel.FriendType.MentorRequestList);
			if (list == null)
			{
				return;
			}
			for (int i = 0; i < list.Count; i++)
			{
				stFriendVerifyContent friendVerifyData = Singleton<CFriendContoller>.GetInstance().model.GetFriendVerifyData(list[i].stUin.ullUid, list[i].stUin.dwLogicWorldId, CFriendModel.enVerifyDataSet.Mentor);
				if (friendVerifyData != null && friendVerifyData.mentorType == 3)
				{
					CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(5404u);
					cSPkg.stPkgData.stConfirmMasterReq.bConfirmType = 2;
					cSPkg.stPkgData.stConfirmMasterReq.stUin.ullUid = list[i].stUin.ullUid;
					cSPkg.stPkgData.stConfirmMasterReq.stUin.dwLogicWorldId = list[i].stUin.dwLogicWorldId;
					cSPkg.stPkgData.stConfirmMasterReq.bReqType = (byte)friendVerifyData.mentorType;
					Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
				}
			}
		}

		public void OnMentorTabChange(CUIEvent evt)
		{
			try
			{
				CUIListScript cUIListScript = (CUIListScript)evt.m_srcWidgetScript;
				CUIListElementScript selectedElement = cUIListScript.GetSelectedElement();
				CFriendContoller.enMentorTab enMentorTab = (CFriendContoller.enMentorTab)Convert.ToInt32(selectedElement.GetDataTag());
				CFriendModel.FriendType friendType = CFriendModel.FriendType.Mentor;
				FriendShower.ItemType type = FriendShower.ItemType.Normal;
				GameObject widget = evt.m_srcFormScript.GetWidget(12);
				GameObject widget2 = evt.m_srcFormScript.GetWidget(11);
				CUIListScript cUIListScript2 = null;
				if (widget2 != null)
				{
					cUIListScript2 = widget2.GetComponent<CUIListScript>();
				}
				bool flag = false;
				CS_STUDENTLIST_TYPE tag = CS_STUDENTLIST_TYPE.CS_STUDENTLIST_BROTHER;
				CFriendContoller.enMentorTab enMentorTab2 = enMentorTab;
				if (enMentorTab2 != CFriendContoller.enMentorTab.MentorAndClassmate)
				{
					if (enMentorTab2 == CFriendContoller.enMentorTab.Apprentice)
					{
						friendType = CFriendModel.FriendType.Apprentice;
						type = FriendShower.ItemType.Apprentice;
						flag = Singleton<CFriendContoller>.GetInstance().m_mentorListOff[1].needShowMore();
						if (cUIListScript2 != null)
						{
							cUIListScript2.EnableExtraContent(flag);
						}
						tag = CS_STUDENTLIST_TYPE.CS_STUDENTLIST_MINE;
					}
				}
				else
				{
					friendType = CFriendModel.FriendType.Mentor;
					type = FriendShower.ItemType.Mentor;
					flag = Singleton<CFriendContoller>.GetInstance().m_mentorListOff[2].needShowMore();
					if (cUIListScript2 != null)
					{
						cUIListScript2.EnableExtraContent(flag);
					}
					tag = CS_STUDENTLIST_TYPE.CS_STUDENTLIST_BROTHER;
				}
				if (flag)
				{
					CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
					uIEvent.m_eventID = enUIEventID.Mentor_GetMoreMentor;
					uIEvent.m_eventParams.tag = (int)tag;
					widget.GetComponent<CUIEventScript>().SetUIEvent(enUIEventType.Click, uIEvent.m_eventID, uIEvent.m_eventParams);
				}
				ListView<COMDT_FRIEND_INFO> list = Singleton<CFriendContoller>.instance.model.GetList(friendType);
				if (list != null)
				{
					this._refresh_list(this.friendListCom, list, type, true, friendType);
					this.friendListCom.gameObject.CustomSetActive(true);
					if (this.info_node != null)
					{
						this.info_node.CustomSetActive(list.Count == 0);
					}
				}
			}
			catch (Exception var_13_18A)
			{
				Debug.LogError("CFriendView + OnMentorTabChange()");
			}
		}

		public COMDT_FRIEND_INFO _get_current_info(int index)
		{
			return Singleton<CFriendContoller>.GetInstance().model.GetInfoAtIndex(this.m_listFriendType, index);
		}

		public void On_List_ElementEnable(CUIEvent uievent)
		{
			int srcWidgetIndexInBelongedList = uievent.m_srcWidgetIndexInBelongedList;
			FriendShower component = uievent.m_srcWidget.GetComponent<FriendShower>();
			if (component != null)
			{
				if (component.mentor_relationship != null)
				{
					component.mentor_relationship.CustomSetActive(false);
				}
				if (component.mentor_graduation != null)
				{
					component.mentor_graduation.CustomSetActive(false);
				}
				CFriendView.Tab curTab = this.CurTab;
				if (curTab != CFriendView.Tab.Friend_LBS)
				{
					COMDT_FRIEND_INFO cOMDT_FRIEND_INFO = this._get_current_info(srcWidgetIndexInBelongedList);
					if (cOMDT_FRIEND_INFO != null)
					{
						UT.ShowFriendData(cOMDT_FRIEND_INFO, component, this.m_listItemType, this.CurTab == CFriendView.Tab.Friend_SNS, this.m_listFriendType, uievent.m_srcFormScript, true);
						int srvFriendTypeFromFriendType = CFriendContoller.GetSrvFriendTypeFromFriendType(this.m_listFriendType);
						if (srvFriendTypeFromFriendType >= 0)
						{
							component.sendHeartBtn_eventScript.m_onClickEventID = enUIEventID.Friend_SendCoin;
							component.sendHeartBtn_eventScript.m_onClickEventParams.tag = srvFriendTypeFromFriendType;
							component.sendHeartBtn_eventScript.m_onClickEventParams.commonUInt64Param1 = cOMDT_FRIEND_INFO.stUin.ullUid;
							component.sendHeartBtn_eventScript.m_onClickEventParams.commonUInt64Param2 = (ulong)cOMDT_FRIEND_INFO.stUin.dwLogicWorldId;
						}
					}
				}
				else
				{
					ListView<CSDT_LBS_USER_INFO> currentLBSList = Singleton<CFriendContoller>.instance.model.GetCurrentLBSList();
					if (currentLBSList != null && srcWidgetIndexInBelongedList < currentLBSList.Count)
					{
						CSDT_LBS_USER_INFO info = currentLBSList[srcWidgetIndexInBelongedList];
						if (component != null)
						{
							UT.ShowLBSUserData(info, component);
						}
					}
				}
			}
		}

		public void RefreshMentorReqList()
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CFriendContoller.MentorRequestListFormPath);
			if (form == null)
			{
				return;
			}
			GameObject widget = form.GetWidget(0);
			if (widget == null)
			{
				return;
			}
			CUIListScript component = widget.GetComponent<CUIListScript>();
			ListView<COMDT_FRIEND_INFO> list = Singleton<CFriendContoller>.instance.model.GetList(CFriendModel.FriendType.MentorRequestList);
			if (list != null)
			{
				this._refresh_list(component, list, FriendShower.ItemType.MentorRequest, false, CFriendModel.FriendType.MentorRequestList);
			}
			component.gameObject.CustomSetActive(true);
		}

		public static void MentorPrivilegeMainList_OnEnable(CUIEvent uievt)
		{
			ResFamousMentor dataByKey = GameDataMgr.famousMentorDatabin.GetDataByKey((long)(uievt.m_srcWidgetIndexInBelongedList + 1));
			if (dataByKey == null)
			{
				Debug.Log("MentorPrivilege_Refresh dont get famose mentor data!");
				return;
			}
			CUIListElementScript component = uievt.m_srcWidget.GetComponent<CUIListElementScript>();
			if (component == null)
			{
				return;
			}
			GameObject gameObject = component.transform.Find("Item/GiftObj/GiftsContainer").gameObject;
			ResPropInfo dataByKey2 = GameDataMgr.itemDatabin.GetDataByKey(dataByKey.dwLvUpBonusPackage);
			Image component2 = component.transform.Find("Item/NobeObj/imgMyPointBg/tipsRight/privilegeLvCount").GetComponent<Image>();
			Image component3 = component.transform.Find("Item/GiftObj/imgMyPointBg/tipsRight/bonusLvCount").GetComponent<Image>();
			string prefabPath = string.Format("{0}{1}.prefab", CUIUtility.s_Sprite_System_ShareUI_Dir, (uievt.m_srcWidgetIndexInBelongedList + 1).ToString());
			CUIUtility.SetImageSprite(component2, prefabPath, null, true, false, false, false);
			CUIUtility.SetImageSprite(component3, prefabPath, null, true, false, false, false);
			if (dataByKey2 != null)
			{
				ResRandomRewardStore dataByKey3 = GameDataMgr.randomRewardDB.GetDataByKey((long)((int)dataByKey2.EftParam[0]));
				if (dataByKey3 != null)
				{
					int i;
					for (i = 0; i < dataByKey3.astRewardDetail.Length; i++)
					{
						if (i >= 6)
						{
							break;
						}
						Transform transform = gameObject.transform.Find("Gift" + i);
						if (dataByKey3.astRewardDetail[i].dwItemID == 0u)
						{
							transform.gameObject.CustomSetActive(false);
						}
						else
						{
							transform.gameObject.CustomSetActive(true);
							CUseable cUseable = CUseableManager.CreateUsableByRandowReward((RES_RANDOM_REWARD_TYPE)dataByKey3.astRewardDetail[i].bItemType, (int)dataByKey3.astRewardDetail[i].dwLowCnt, dataByKey3.astRewardDetail[i].dwItemID);
							transform.gameObject.CustomSetActive(true);
							Transform transform2 = transform.FindChild(string.Format("Icon", new object[0]));
							Transform transform3 = transform.FindChild(string.Format("Text", new object[0]));
							Transform transform4 = transform.FindChild(string.Format("ExperienceCard", new object[0]));
							Transform transform5 = transform.FindChild(string.Format("TextNum", new object[0]));
							transform5.gameObject.GetComponent<Text>().set_text(dataByKey3.astRewardDetail[i].dwLowCnt.ToString());
							if (transform4 != null)
							{
								transform4.gameObject.CustomSetActive(CItem.IsSkinExperienceCard(dataByKey3.astRewardDetail[i].dwItemID) || CItem.IsHeroExperienceCard(dataByKey3.astRewardDetail[i].dwItemID));
							}
							if (transform2 != null && transform3 != null)
							{
								transform2.gameObject.GetComponent<Image>().SetSprite(cUseable.GetIconPath(), component.m_belongedFormScript, true, false, false, false);
								transform3.gameObject.GetComponent<Text>().set_text(cUseable.m_name);
								while (transform2.transform.childCount > 0)
								{
									Transform child = transform2.transform.GetChild(0);
									child.parent = null;
									CUIParticleScript component4 = child.GetComponent<CUIParticleScript>();
									if (component4 != null && component4.m_belongedFormScript != null)
									{
										component4.m_belongedFormScript.RemoveUIComponent(component4);
									}
									CUICommonSystem.DestoryObj(child.gameObject, 0.1f);
								}
								if (cUseable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_HEADIMG)
								{
									CHeadImg cHeadImg = (CHeadImg)cUseable;
									MonoSingleton<NobeSys>.instance.SetHeadIconBkEffect(transform2.GetComponent<Image>(), (int)cHeadImg.m_baseID, component.m_belongedFormScript, 1f, true);
								}
							}
						}
					}
					while (i < 6)
					{
						Transform transform6 = gameObject.transform.Find("Gift" + i);
						transform6.gameObject.CustomSetActive(false);
						i++;
					}
				}
			}
			GameObject gameObject2 = component.transform.Find("Item/NobeObj/privilegeContainer").gameObject;
			for (int j = 0; j < 6; j++)
			{
				Transform transform7 = gameObject2.transform.Find("itemCell" + j);
				if (string.IsNullOrEmpty(dataByKey.astPrivilegeIcon[j].szPrivilegeIcon))
				{
					transform7.gameObject.CustomSetActive(false);
				}
				else
				{
					transform7.gameObject.CustomSetActive(true);
					Transform transform8 = transform7.FindChild("imgback");
					Transform transform9 = transform7.FindChild("Text");
					if (transform8 != null && transform9 != null)
					{
						transform8.gameObject.GetComponent<Image>().SetSprite("UGUI/Sprite/Dynamic/mentor/" + dataByKey.astPrivilegeIcon[j].szPrivilegeIcon, component.m_belongedFormScript, true, false, false, false);
						transform9.GetComponent<Text>().set_text(dataByKey.astPrivilegeIcon[j].szPrivilegeDesc);
					}
				}
			}
		}

		public static void MentorPrivilege_SetMainList(CUIFormScript form)
		{
			CUIListScript component = form.GetWidget(8).GetComponent<CUIListScript>();
			component.SetElementAmount(GameDataMgr.famousMentorDatabin.count);
		}

		public static void MentorPrivilege_Refresh(CUIFormScript form, int currDisLv)
		{
			string prefabPath = string.Format("{0}{1}.prefab", CUIUtility.s_Sprite_System_ShareUI_Dir, currDisLv.ToString());
			Image component = form.GetWidget(0).GetComponent<Image>();
			Image component2 = form.GetWidget(7).GetComponent<Image>();
			CUIUtility.SetImageSprite(component, prefabPath, null, true, false, false, false);
			CUIUtility.SetImageSprite(component2, prefabPath, null, true, false, false, false);
			Text component3 = form.GetWidget(4).GetComponent<Text>();
			ResFamousMentor dataByKey = GameDataMgr.famousMentorDatabin.GetDataByKey((long)currDisLv);
			component3.set_text(Singleton<CTextManager>.GetInstance().GetText("Mentor_Requirement", new string[]
			{
				dataByKey.dwPoint.ToString()
			}));
			Text component4 = form.GetWidget(3).GetComponent<Text>();
			component4.set_text(dataByKey.szTitle);
		}

		private void _refresh_list(CUIListScript listScript, ListView<COMDT_FRIEND_INFO> data_list, FriendShower.ItemType type, bool bShowNickName, CFriendModel.FriendType friend)
		{
			if (listScript == null)
			{
				return;
			}
			this.m_listItemType = type;
			this.m_listFriendType = friend;
			int count = data_list.Count;
			if (friend == CFriendModel.FriendType.Mentor || friend == CFriendModel.FriendType.Apprentice)
			{
				Singleton<CFriendContoller>.GetInstance().model.SortShower(friend);
			}
			listScript.SetElementAmount(count);
		}

		private void _refresh_black_list(CUIListScript listScript, List<CFriendModel.stBlackName> blackList)
		{
			if (listScript == null)
			{
				return;
			}
			int count = blackList.get_Count();
			listScript.SetElementAmount(count);
			for (int i = 0; i < count; i++)
			{
				CUIListElementScript elemenet = listScript.GetElemenet(i);
				if (elemenet != null && listScript.IsElementInScrollArea(i))
				{
					FriendShower component = elemenet.GetComponent<FriendShower>();
					CFriendModel.stBlackName stBlackName = blackList.get_Item(i);
					if (component != null)
					{
						UT.ShowBlackListData(ref stBlackName, component);
					}
				}
			}
		}

		private void _refresh_LBS_list(CUIListScript listScript, ListView<CSDT_LBS_USER_INFO> LBSList)
		{
			if (listScript == null)
			{
				return;
			}
			if (LBSList == null)
			{
				listScript.SetElementAmount(0);
				return;
			}
			int count = LBSList.Count;
			listScript.SetElementAmount(count);
		}

		public void On_Friend_Invite_SNS_Friend(CUIEvent uiEvent)
		{
			if (!MonoSingleton<ShareSys>.instance.IsInstallPlatform())
			{
				return;
			}
			string text = UT.GetText("Friend_Invite_SNS_Title");
			string text2 = UT.GetText("Friend_Invite_SNS_Desc");
			Singleton<ApolloHelper>.GetInstance().ShareToFriend(text, text2);
		}

		public CFriendView.Tab GetSelectedTab()
		{
			if (this.tablistScript != null)
			{
				int selectedIndex = this.tablistScript.GetSelectedIndex();
				CFriendView.TabMgr.TabElement tabElement = Singleton<CFriendContoller>.GetInstance().view.tabMgr.GetTabElement(selectedIndex);
				if (tabElement != null)
				{
					return tabElement.tab;
				}
			}
			return CFriendView.Tab.None;
		}

		public static void OpenFriendTab(CFriendView.Tab tab)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CFriendContoller.FriendFormPath);
			if (form == null)
			{
				Singleton<CFriendContoller>.GetInstance().view.OpenForm(null);
				form = Singleton<CUIManager>.GetInstance().GetForm(CFriendContoller.FriendFormPath);
			}
			if (form == null)
			{
				return;
			}
			GameObject gameObject = form.transform.Find("TopCommon/Panel_Menu/List").gameObject;
			CUIListScript component = gameObject.GetComponent<CUIListScript>();
			component.SelectElement(Singleton<CFriendContoller>.GetInstance().view.tabMgr.GetIndex(tab), true);
		}
	}
}
