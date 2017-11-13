using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using CSProtocol;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CChatView
{
	private enum enChatFormWidgets
	{
		GuildRecruitList
	}

	public class CChatViewEntryNode
	{
		private Text txt_down;

		private Image channel_friend;

		private Image channel_guild;

		private Image channel_guildMatchTeam;

		private Image channel_lobby;

		private Image channel_room;

		private Image channel_team;

		private Image channel_speaker;

		private GameObject channel_default;

		private GameObject entry_node;

		private GameObject entry_node_lobby_bg;

		private GameObject entry_node_speaker;

		public GameObject text_template;

		public GameObject image_template;

		private GameObject action_btn;

		private GameObject entry_bubble;

		private Text entry_bubble_CountText;

		public void SetBubbleCount(int count)
		{
			if (count > 99)
			{
				count = 99;
			}
			this.entry_bubble.CustomSetActive(count > 0);
			if (this.entry_bubble_CountText != null)
			{
				this.entry_bubble_CountText.set_text(count.ToString());
			}
		}

		private void InitInsideNode(GameObject entryNode)
		{
			this.entry_node = entryNode;
			this.entry_node.CustomSetActive(true);
			this.entry_node_speaker = Utility.FindChild(this.entry_node, "entry_node_speaker");
			this.entry_node_speaker.CustomSetActive(false);
			this.entry_node_lobby_bg = Utility.FindChild(this.entry_node, "normal_node/LobbyBg");
			this.txt_down = Utility.GetComponetInChild<Text>(this.entry_node, "normal_node/txt_down");
			this.txt_down.set_text(string.Empty);
			this.channel_friend = this.entry_node.transform.FindChild("normal_node/channel_img/friend").GetComponent<Image>();
			this.channel_guild = this.entry_node.transform.FindChild("normal_node/channel_img/gulid").GetComponent<Image>();
			this.channel_lobby = this.entry_node.transform.FindChild("normal_node/channel_img/lobby").GetComponent<Image>();
			this.channel_room = this.entry_node.transform.FindChild("normal_node/channel_img/room").GetComponent<Image>();
			this.channel_team = this.entry_node.transform.FindChild("normal_node/channel_img/team").GetComponent<Image>();
			this.channel_speaker = this.entry_node.transform.FindChild("normal_node/channel_img/speaker").GetComponent<Image>();
			this.channel_default = Utility.FindChild(this.entry_node, "normal_node/channel_img/default");
			Transform transform = this.entry_node.transform.FindChild("normal_node/channel_img/guildMatchTeam");
			if (transform != null)
			{
				this.channel_guildMatchTeam = transform.GetComponent<Image>();
			}
			this.entry_bubble = Utility.FindChild(this.entry_node, "bubble");
			if (this.entry_bubble != null)
			{
				this.entry_bubble_CountText = this.entry_bubble.transform.FindChild("Text").GetComponent<Text>();
			}
			this.action_btn = Utility.FindChild(this.entry_node, "normal_node/actionBtn");
			this.action_btn.CustomSetActive(false);
		}

		public void Clear()
		{
			this.txt_down = null;
			this.channel_friend = null;
			this.channel_guild = null;
			this.channel_guildMatchTeam = null;
			this.channel_lobby = null;
			this.channel_room = null;
			this.channel_team = null;
			this.channel_speaker = null;
			this.channel_default = null;
			this.entry_node = null;
			this.entry_node_lobby_bg = null;
			this.entry_node_speaker = null;
			this.text_template = null;
			this.image_template = null;
			this.entry_bubble = null;
			this.entry_bubble_CountText = null;
			this.action_btn = null;
		}

		public void SetVisivble(bool bShow)
		{
			this.entry_node.CustomSetActive(bShow);
		}

		public void Refresh_EntryForm(GameObject entryGameObject, CUIFormScript form)
		{
			this.Clear();
			if (entryGameObject == null)
			{
				return;
			}
			this.text_template = entryGameObject.transform.Find("template/Text_template").gameObject;
			this.image_template = entryGameObject.transform.Find("template/Image_template").gameObject;
			this.InitInsideNode(entryGameObject);
			CChatEntity entryEntity = Singleton<CChatController>.GetInstance().model.sysData.entryEntity;
			if (this.txt_down == null || this.txt_down.gameObject == null)
			{
				return;
			}
			GameObject gameObject = this.txt_down.gameObject;
			this.SetEntryChannelImage(Singleton<CChatController>.GetInstance().model.sysData.CurChannel);
			this.Clear_EntryForm_Node();
			bool flag = Singleton<CChatController>.GetInstance().model.sysData.CurChannel == EChatChannel.Speaker;
			this.entry_node_lobby_bg.CustomSetActive(!flag);
			this.entry_node_speaker.CustomSetActive(flag);
			if (flag)
			{
				CUIAutoScroller component = this.entry_node_speaker.GetComponent<CUIAutoScroller>();
				component.SetText(CUIUtility.RemoveEmoji(entryEntity.text));
				component.StopAutoScroll();
				component.StartAutoScroll(true);
			}
			else if (entryEntity.TextObjList.Count > 0)
			{
				for (int i = 0; i < entryEntity.TextObjList.Count; i++)
				{
					CTextImageNode cTextImageNode = entryEntity.TextObjList[i];
					this.Create_Content(gameObject, cTextImageNode.content, cTextImageNode.type, cTextImageNode.posX, cTextImageNode.posY, cTextImageNode.width, entryEntity.type == EChaterType.Self, false, true, form);
				}
			}
			else
			{
				this.Create_Content(gameObject, Singleton<CTextManager>.instance.GetText("ChatEntry_Default_Text"), CChatParser.InfoType.Text, 0f, 0f, 200f, true, false, true, form);
				this.SetEntryChannelImage(EChatChannel.Default);
			}
			this.SetBubbleCount(Singleton<CChatController>.instance.model.channelMgr.GetAllFriendUnreadCount());
		}

		public void Clear_EntryForm_Node()
		{
			if (this.txt_down != null)
			{
				CChatUT.DestoryAllChild(this.txt_down.gameObject);
			}
		}

		public void SetEntryChannelImage(EChatChannel v)
		{
			if (this.channel_friend != null)
			{
				this.channel_friend.gameObject.CustomSetActive(false);
			}
			if (this.channel_guild != null)
			{
				this.channel_guild.gameObject.CustomSetActive(false);
			}
			if (this.channel_guildMatchTeam != null)
			{
				this.channel_guildMatchTeam.gameObject.CustomSetActive(false);
			}
			if (this.channel_lobby != null)
			{
				this.channel_lobby.gameObject.CustomSetActive(false);
			}
			if (this.channel_room != null)
			{
				this.channel_room.gameObject.CustomSetActive(false);
			}
			if (this.channel_team != null)
			{
				this.channel_team.gameObject.CustomSetActive(false);
			}
			if (this.channel_speaker != null)
			{
				this.channel_speaker.gameObject.CustomSetActive(false);
			}
			if (this.channel_default != null)
			{
				this.channel_default.gameObject.CustomSetActive(false);
			}
			switch (v)
			{
			case EChatChannel.Team:
				if (this.channel_team != null)
				{
					this.channel_team.gameObject.CustomSetActive(true);
				}
				return;
			case EChatChannel.Room:
			case EChatChannel.Settle:
				if (this.channel_room != null)
				{
					this.channel_room.gameObject.CustomSetActive(true);
				}
				return;
			case EChatChannel.Lobby:
				if (this.channel_lobby != null)
				{
					this.channel_lobby.gameObject.CustomSetActive(true);
				}
				return;
			case EChatChannel.GuildMatchTeam:
				if (this.channel_guildMatchTeam != null)
				{
					this.channel_guildMatchTeam.gameObject.CustomSetActive(true);
				}
				return;
			case EChatChannel.Friend:
				if (this.channel_friend != null)
				{
					this.channel_friend.gameObject.CustomSetActive(true);
				}
				return;
			case EChatChannel.Guild:
				if (this.channel_guild != null)
				{
					this.channel_guild.gameObject.CustomSetActive(true);
				}
				return;
			case EChatChannel.Default:
				if (this.channel_default != null)
				{
					this.channel_default.CustomSetActive(true);
				}
				return;
			}
		}

		private void Create_Content(GameObject pNode, string content, CChatParser.InfoType type, float x, float y, float width, bool bSelf, bool bVip = false, bool bUse_Entry = false, CUIFormScript form = null)
		{
			this.action_btn.CustomSetActive(false);
			if (type == CChatParser.InfoType.Text)
			{
				if (this.text_template == null || pNode == null)
				{
					return;
				}
				Text text = this.GetText("text", this.text_template, pNode);
				text.set_text(CUIUtility.RemoveEmoji(content));
				if (!bVip)
				{
					if (bSelf)
					{
						text.set_color(new Color(0f, 0f, 0f));
					}
					else
					{
						text.set_color(CUIUtility.s_Color_White);
					}
				}
				else if (bSelf)
				{
					text.set_color(CUIUtility.s_Text_Color_Vip_Chat_Self);
				}
				else
				{
					text.set_color(CUIUtility.s_Text_Color_Vip_Chat_Other);
				}
				if (bUse_Entry)
				{
					text.set_color(CUIUtility.s_Color_White);
				}
				if (bUse_Entry)
				{
					text.get_rectTransform().anchoredPosition = new Vector2(x, -(float)CChatParser.chat_entry_lineHeight);
				}
				else
				{
					text.get_rectTransform().anchoredPosition = new Vector2(x, y);
				}
				text.get_rectTransform().sizeDelta = new Vector2(width, text.get_rectTransform().sizeDelta.y);
			}
			else if (type == CChatParser.InfoType.Face)
			{
				int index;
				int.TryParse(content, ref index);
				Image image = this.GetImage(index, this.image_template, pNode);
				image.get_rectTransform().anchoredPosition = new Vector2(x, y);
			}
			else
			{
				if (this.action_btn == null || form == null)
				{
					return;
				}
				string btnName = null;
				string text2 = null;
				enUIEventID uiEventID = enUIEventID.None;
				if (!CChatParser.ParseActionBtnContent(content, out btnName, out text2, out uiEventID))
				{
					return;
				}
				if (this.action_btn != null)
				{
					Singleton<CChatController>.GetInstance().FilterActionBtn(uiEventID, this.action_btn, btnName, form);
				}
			}
		}

		private Text GetText(string name, GameObject template, GameObject pNode)
		{
			GameObject gameObject = (GameObject)Object.Instantiate(template);
			gameObject.name = "text";
			gameObject.CustomSetActive(true);
			gameObject.transform.SetParent(pNode.transform);
			Text component = gameObject.GetComponent<Text>();
			component.get_rectTransform().localPosition = new Vector3(0f, 0f, 0f);
			component.get_rectTransform().pivot = new Vector2(0f, 0f);
			component.get_rectTransform().localScale = new Vector3(1f, 1f, 1f);
			return component;
		}

		private Image GetImage(int index, GameObject template, GameObject pNode)
		{
			GameObject gameObject = (GameObject)Object.Instantiate(template);
			gameObject.name = "image";
			gameObject.CustomSetActive(true);
			gameObject.transform.SetParent(pNode.transform);
			Image component = gameObject.GetComponent<Image>();
			component.get_rectTransform().localPosition = new Vector3(0f, 0f, 0f);
			component.get_rectTransform().pivot = new Vector2(0f, 0f);
			component.get_rectTransform().localScale = new Vector3(1f, 1f, 1f);
			if (index > 77)
			{
				index = 77;
			}
			if (index < 1)
			{
				index = 1;
			}
			component.SetSprite(string.Format("UGUI/Sprite/Dynamic/ChatFace/{0}", index), null, true, false, false, false);
			component.SetNativeSize();
			return component;
		}
	}

	public static int ChatFaceCount = 77;

	public static int ChatMaxLength = 20;

	public static int max_bubble_num = 99;

	public bool bShow;

	public bool bRefreshNew = true;

	public CUIFormScript chatForm;

	private CUIListScript listScript;

	public CUIListScript LobbyScript;

	public CUIListScript FriendTabListScript;

	public CanvasGroup LobbyScript_cg;

	public CanvasGroup FriendTabListScript_cg;

	public CUIListScript ChatFaceListScript;

	private InputField inputField;

	private GameObject sendBtn;

	private GameObject toolBarNode;

	private GameObject screenBtn;

	private bool bInited;

	private GameObject nodeGameObject;

	private GameObject deleteGameObject;

	private bool m_inputTextChanged;

	private GameObject loudSpeakerNode;

	private GameObject sendSpeaker;

	private GameObject sendLoudSpeaker;

	public static Vector2 entrySizeLobby = new Vector2(355f, 42f);

	public static Vector2 entrySizeRoom = new Vector2(350f, 42f);

	public static Vector2 entrySizeTeam = new Vector2(350f, 42f);

	public GameObject bubbleNode;

	private CChatView.CChatViewEntryNode m_viewEntryNode = new CChatView.CChatViewEntryNode();

	private ListView<COMDT_FRIEND_INFO> friendTablist;

	private bool lastB;

	private int checkTimer = -1;

	private List<uint> curChannels;

	public CChatParser ChatParser = new CChatParser();

	public GameObject text_template;

	public GameObject image_template;

	public GameObject info_node_obj;

	public Text info_text;

	public Animator Anim;

	private EChatChannel _tab;

	private static float single_line_height = 38f;

	private static float double_line_height = 76f;

	private static float trible_line_height = 100f;

	public EChatChannel CurTab
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
			this.SetChatFaceShow(false);
			this.SetShow(this.LobbyScript_cg, false);
			this.SetShow(this.FriendTabListScript_cg, false);
			if (this.bubbleNode != null)
			{
				this.bubbleNode.CustomSetActive(false);
			}
			this.toolBarNode.CustomSetActive(true);
			this.info_node_obj.CustomSetActive(false);
			if (Singleton<CChatController>.instance.view != null)
			{
				Singleton<CChatController>.instance.view.ShowLoudSpeaker(false, null);
			}
			this.Flag_Readed(this.CurTab);
			GameObject widget = this.chatForm.GetWidget(0);
			widget.CustomSetActive(false);
			switch (this._tab)
			{
			case EChatChannel.Team:
				this.SetShow(this.LobbyScript_cg, true);
				this.Refresh_ChatEntity_List(true, EChatChannel.None);
				break;
			case EChatChannel.Room:
				this.SetShow(this.LobbyScript_cg, true);
				this.Refresh_ChatEntity_List(true, EChatChannel.None);
				break;
			case EChatChannel.Lobby:
				if (Singleton<CChatController>.instance.view != null && Singleton<CLoudSpeakerSys>.instance.CurLoudSpeaker != null)
				{
					Singleton<CChatController>.instance.view.ShowLoudSpeaker(true, Singleton<CLoudSpeakerSys>.instance.CurLoudSpeaker);
				}
				this.SetShow(this.LobbyScript_cg, true);
				this.Refresh_ChatEntity_List(true, EChatChannel.None);
				break;
			case EChatChannel.GuildMatchTeam:
				this.SetShow(this.LobbyScript_cg, true);
				this.Refresh_ChatEntity_List(true, EChatChannel.None);
				break;
			case EChatChannel.Friend:
				this.SetShow(this.FriendTabListScript_cg, true);
				this.Refresh_ChatEntity_List(true, EChatChannel.None);
				this.toolBarNode.CustomSetActive(false);
				this.Process_Friend_Tip();
				break;
			case EChatChannel.Guild:
				this.SetShow(this.LobbyScript_cg, true);
				this.Refresh_ChatEntity_List(true, EChatChannel.None);
				break;
			case EChatChannel.GuildRecruit:
				widget.CustomSetActive(true);
				this.RefreshGuildRecruitList();
				this.RefreshGuildRecruitInfoNode();
				break;
			case EChatChannel.Friend_Chat:
				this.SetShow(this.LobbyScript_cg, true);
				this.Refresh_ChatEntity_List(true, EChatChannel.None);
				break;
			case EChatChannel.Settle:
				this.SetShow(this.LobbyScript_cg, true);
				this.Refresh_ChatEntity_List(true, EChatChannel.None);
				break;
			}
			this.Refresh_All_RedPoint();
		}
	}

	public void Update()
	{
		if (!this.m_inputTextChanged || this.inputField == null)
		{
			return;
		}
		this.m_inputTextChanged = false;
		string text = this.inputField.get_text();
		if (text != null && text.get_Length() > CChatView.ChatMaxLength)
		{
			this.inputField.DeactivateInputField();
			this.inputField.set_text(text.Substring(0, CChatView.ChatMaxLength));
			Singleton<CUIManager>.GetInstance().OpenMessageBox(string.Format(Singleton<CTextManager>.instance.GetText("chat_input_max"), CChatView.ChatMaxLength), false);
		}
	}

	public void CreateDetailChatForm()
	{
		if (this.chatForm == null)
		{
			this.chatForm = Singleton<CUIManager>.GetInstance().OpenForm(CChatController.ChatFormPath, false, true);
			this.Init();
		}
	}

	private void Init()
	{
		Singleton<CChatController>.instance.model.channelMgr.GetChannel(EChatChannel.Lobby).Init_Timer();
		this.nodeGameObject = Utility.FindChild(this.chatForm.gameObject, "node");
		this.nodeGameObject.CustomSetActive(false);
		this.Anim = this.nodeGameObject.GetComponent<Animator>();
		this.info_node_obj = this.chatForm.transform.FindChild("node/info_node").gameObject;
		this.info_text = this.chatForm.transform.FindChild("node/info_node/Text").gameObject.GetComponent<Text>();
		this.inputField = this.chatForm.transform.FindChild("node/ToolBar/InputField").gameObject.GetComponent<InputField>();
		this.inputField.get_onValueChange().AddListener(new UnityAction<string>(this.On_InputFiled_ValueChange));
		this.toolBarNode = this.chatForm.transform.FindChild("node/ToolBar").gameObject;
		this.sendBtn = this.chatForm.transform.FindChild("node/ToolBar/SendBtn").gameObject;
		this.bubbleNode = this.chatForm.transform.FindChild("node/bubble").gameObject;
		this.listScript = this.chatForm.transform.FindChild("node/Tab/List").gameObject.GetComponent<CUIListScript>();
		this.curChannels = Singleton<CChatController>.instance.model.channelMgr.CurActiveChannels;
		this.BuildTabList(this.curChannels, 0);
		this.sendSpeaker = Utility.FindChild(this.chatForm.gameObject, "node/SendSpeaker");
		this.sendLoudSpeaker = Utility.FindChild(this.chatForm.gameObject, "node/SendLoudSpeaker");
		this.loudSpeakerNode = Utility.FindChild(this.chatForm.gameObject, "node/ListView/LobbyChatList/LoudSpeakerNode");
		this.LobbyScript = this.chatForm.transform.FindChild("node/ListView/LobbyChatList").gameObject.GetComponent<CUIListScript>();
		this.FriendTabListScript = this.chatForm.transform.FindChild("node/ListView/FriendItemList").gameObject.GetComponent<CUIListScript>();
		this.FriendTabListScript.m_alwaysDispatchSelectedChangeEvent = true;
		this.ChatFaceListScript = this.chatForm.transform.FindChild("node/ListView/ChatFaceList").gameObject.GetComponent<CUIListScript>();
		this.screenBtn = Utility.FindChild(this.chatForm.gameObject, "node/ListView/Button");
		this.LobbyScript_cg = this.LobbyScript.GetComponent<CanvasGroup>();
		this.FriendTabListScript_cg = this.FriendTabListScript.GetComponent<CanvasGroup>();
		if (this.FriendTabListScript != null)
		{
			this.FriendTabListScript.gameObject.CustomSetActive(true);
		}
		if (this.LobbyScript != null)
		{
			this.LobbyScript.gameObject.CustomSetActive(true);
		}
		this.deleteGameObject = Utility.FindChild(this.chatForm.gameObject, "node/ToolBar/delete");
		this.deleteGameObject.CustomSetActive(false);
		this.SetInputFiledEnable(false);
		this.InitCheckTimer();
		this.text_template = Utility.FindChild(this.chatForm.gameObject, "Text_template");
		this.image_template = Utility.FindChild(this.chatForm.gameObject, "Image_template");
		this.Refresh_All_RedPoint();
		this._tab = EChatChannel.None;
		this.bInited = true;
		CUIEventScript componetInChild = Utility.GetComponetInChild<CUIEventScript>(this.chatForm.gameObject, "node/SendSpeaker");
		CUIEventScript componetInChild2 = Utility.GetComponetInChild<CUIEventScript>(this.chatForm.gameObject, "node/SendLoudSpeaker");
		componetInChild.m_onClickEventParams.commonUInt32Param1 = 10041u;
		componetInChild2.m_onClickEventParams.commonUInt32Param1 = 10042u;
		this.Refresh_EntryForm();
	}

	public void BuildTabList(List<uint> list, int index = 0)
	{
		this.listScript.SetElementAmount(list.get_Count());
		for (int i = 0; i < this.listScript.m_elementAmount; i++)
		{
			CUIListElementScript elemenet = this.listScript.GetElemenet(i);
			CUIEventScript component = elemenet.GetComponent<CUIEventScript>();
			component.m_onClickEventParams.tag = i;
			Text component2 = elemenet.gameObject.transform.FindChild("Text").GetComponent<Text>();
			component2.set_text(Singleton<CTextManager>.instance.GetText(CChatUT.GetEChatChannel_Text((EChatChannel)list.get_Item(i))));
		}
		this.listScript.SelectElement(index, true);
	}

	public void ReBuildTabText()
	{
		if (this.listScript == null)
		{
			return;
		}
		for (int i = 0; i < this.curChannels.get_Count(); i++)
		{
			CUIListElementScript elemenet = this.listScript.GetElemenet(i);
			if (elemenet == null)
			{
				return;
			}
			Text component = elemenet.gameObject.transform.FindChild("Text").GetComponent<Text>();
			component.set_text(Singleton<CTextManager>.instance.GetText(CChatUT.GetEChatChannel_Text((EChatChannel)this.curChannels.get_Item(i))));
		}
	}

	public void InitCheckTimer()
	{
		if (this.checkTimer != -1)
		{
			return;
		}
		this.checkTimer = Singleton<CTimerManager>.GetInstance().AddTimer(40, 0, new CTimer.OnTimeUpHandler(this.On_CheckInputField_Focus));
		UT.ResetTimer(this.checkTimer, true);
	}

	private void On_InputFiled_ValueChange(string arg0)
	{
		this.m_inputTextChanged = true;
	}

	public void SetCheckTimerEnable(bool b)
	{
		if (this.checkTimer != -1)
		{
			UT.ResetTimer(this.checkTimer, !b);
		}
	}

	private void On_CheckInputField_Focus(int timer)
	{
		if (this.lastB != this.inputField.get_isFocused())
		{
			this.lastB = this.inputField.get_isFocused();
			this.SetInputFiledEnable(this.lastB);
		}
	}

	public string GetInputText()
	{
		if (this.inputField != null)
		{
			return this.inputField.get_text();
		}
		return null;
	}

	public void ClearInputText()
	{
		if (this.inputField != null)
		{
			this.inputField.set_text(string.Empty);
		}
	}

	public void ClearChatForm()
	{
		this._tab = EChatChannel.None;
		this.info_node_obj = null;
		this.info_text = null;
		this.bShow = false;
		this.bRefreshNew = true;
		this.bInited = false;
		if (this.friendTablist != null)
		{
			this.friendTablist.Clear();
		}
		this.friendTablist = null;
		this.bubbleNode = null;
		this.lastB = false;
		if (this.checkTimer != -1)
		{
			Singleton<CTimerManager>.GetInstance().RemoveTimer(this.checkTimer);
		}
		this.checkTimer = -1;
		this.listScript = null;
		this.LobbyScript = (this.FriendTabListScript = null);
		this.LobbyScript_cg = (this.FriendTabListScript_cg = null);
		this.ChatFaceListScript = null;
		this.inputField = null;
		this.toolBarNode = null;
		this.screenBtn = null;
		this.nodeGameObject = null;
		this.deleteGameObject = null;
		this.m_inputTextChanged = false;
		this.sendSpeaker = null;
		this.sendLoudSpeaker = null;
		this.loudSpeakerNode = null;
		this.friendTablist = null;
		this.chatForm = null;
	}

	public void UpView(bool bup)
	{
		if (this.chatForm == null)
		{
			return;
		}
		if (bup)
		{
			this.chatForm.SetPriority(enFormPriority.Priority5);
		}
		else
		{
			this.chatForm.RestorePriority();
		}
	}

	public void HideDetailChatForm()
	{
		this.bShow = false;
		if (this.chatForm != null)
		{
			this.nodeGameObject.CustomSetActive(false);
		}
		this.SetCheckTimerEnable(false);
		Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Chat_CloseForm);
	}

	public void SetGuildRecruitListElement(CUIEvent uiEvent)
	{
		CChatModel model = Singleton<CChatController>.GetInstance().model;
		if (model == null || model.sysData == null)
		{
			return;
		}
		if (uiEvent.m_srcWidgetIndexInBelongedList >= 0 && uiEvent.m_srcWidgetIndexInBelongedList < model.sysData.m_guildRecruitInfos.get_Count())
		{
			GuildRecruitInfo guildRecruitInfo = model.sysData.m_guildRecruitInfos.get_Item(uiEvent.m_srcWidgetIndexInBelongedList);
			Transform transform = uiEvent.m_srcWidget.transform;
			if (transform != null)
			{
				CUIHttpImageScript component = transform.Find("head/pnlSnsHead/HttpImage").GetComponent<CUIHttpImageScript>();
				Text component2 = transform.Find("head/LevelBg/Level").GetComponent<Text>();
				Text component3 = transform.Find("name").GetComponent<Text>();
				Text component4 = transform.Find("content").GetComponent<Text>();
				CUIEventScript component5 = transform.Find("btnApplyJoin").GetComponent<CUIEventScript>();
				component.SetImageUrl(Singleton<ApolloHelper>.GetInstance().ToSnsHeadUrl(guildRecruitInfo.senderHeadUrl));
				component2.set_text(guildRecruitInfo.senderLevel.ToString());
				component3.set_text(guildRecruitInfo.senderName);
				component4.set_text(Singleton<CTextManager>.GetInstance().GetText("Guild_Recruit_Info_Format", new string[]
				{
					guildRecruitInfo.guildName,
					guildRecruitInfo.limitLevel.ToString(),
					CGuildHelper.GetLadderGradeLimitText((int)guildRecruitInfo.limitGrade)
				}));
				component5.m_onClickEventParams.commonUInt64Param1 = guildRecruitInfo.guildId;
				component5.m_onClickEventParams.tag = guildRecruitInfo.guildLogicWorldId;
			}
		}
	}

	public void ShowDetailChatForm()
	{
		CChatChannelMgr.EChatTab chatTab = Singleton<CChatController>.GetInstance().model.channelMgr.ChatTab;
		if (chatTab == CChatChannelMgr.EChatTab.Normal)
		{
			this.chatForm.SetPriority(enFormPriority.Priority2);
			this.sendSpeaker.CustomSetActive(true);
			this.sendLoudSpeaker.CustomSetActive(true);
		}
		else if (chatTab == CChatChannelMgr.EChatTab.Room || chatTab == CChatChannelMgr.EChatTab.Team)
		{
			this.sendSpeaker.CustomSetActive(false);
			this.sendLoudSpeaker.CustomSetActive(false);
		}
		else if (chatTab == CChatChannelMgr.EChatTab.Settle)
		{
			this.sendSpeaker.CustomSetActive(false);
			this.sendLoudSpeaker.CustomSetActive(false);
		}
		if (!this.bInited)
		{
			return;
		}
		if (this.chatForm == null)
		{
			return;
		}
		this._tab = EChatChannel.None;
		this.bShow = true;
		this.nodeGameObject.CustomSetActive(true);
		this.curChannels = Singleton<CChatController>.GetInstance().model.channelMgr.CurActiveChannels;
		this.curChannels.Sort();
		if (chatTab == CChatChannelMgr.EChatTab.Normal)
		{
			this.SortChannels();
		}
		this.BuildTabList(this.curChannels, 0);
		this.CurTab = Singleton<CChatController>.instance.model.sysData.LastChannel;
		int index = 0;
		for (int i = 0; i < this.curChannels.get_Count(); i++)
		{
			if (this.curChannels.get_Item(i) == (uint)this.CurTab)
			{
				index = i;
			}
		}
		UT.SetListIndex(this.listScript, index);
		this.SetChatFaceShow(false);
		this.SetCheckTimerEnable(true);
		if (CSysDynamicBlock.bLobbyEntryBlocked)
		{
			Transform transform = this.chatForm.transform.FindChild("node/SendSpeaker");
			Transform transform2 = this.chatForm.transform.FindChild("node/SendLoudSpeaker");
			if (transform)
			{
				transform.gameObject.CustomSetActive(false);
			}
			if (transform2)
			{
				transform2.gameObject.CustomSetActive(false);
			}
		}
	}

	private void SortChannels()
	{
		for (int i = 0; i < this.curChannels.get_Count(); i++)
		{
			EChatChannel eChatChannel = (EChatChannel)this.curChannels.get_Item(i);
			if (eChatChannel == EChatChannel.Friend && this.GetUnReadCount(eChatChannel) > 0)
			{
				this.curChannels.RemoveAt(i);
				this.curChannels.Insert(0, (uint)eChatChannel);
				return;
			}
			if (eChatChannel == EChatChannel.Guild && this.GetUnReadCount(eChatChannel) > 0)
			{
				this.curChannels.RemoveAt(i);
				this.curChannels.Insert(0, (uint)eChatChannel);
				return;
			}
		}
	}

	public void ShowEntryForm()
	{
		if (this.chatForm != null)
		{
			CChatChannelMgr.EChatTab chatTab = Singleton<CChatController>.GetInstance().model.channelMgr.ChatTab;
			if (chatTab == CChatChannelMgr.EChatTab.Normal)
			{
				this.chatForm.SetPriority(enFormPriority.Priority0);
			}
			else if (chatTab == CChatChannelMgr.EChatTab.Room || chatTab == CChatChannelMgr.EChatTab.Team || chatTab == CChatChannelMgr.EChatTab.Settle)
			{
				this.chatForm.SetPriority(enFormPriority.Priority5);
			}
		}
		this.Refresh_EntryForm();
	}

	public void SetEntryVisible(bool bShow)
	{
		if (this.m_viewEntryNode != null)
		{
			this.m_viewEntryNode.SetVisivble(bShow);
		}
	}

	public void Refresh_EntryForm()
	{
		if (this.m_viewEntryNode != null)
		{
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CLobbySystem.LOBBY_FORM_PATH);
			if (form != null && !form.IsHided())
			{
				GameObject gameObject = form.transform.Find("entry_node").gameObject;
				this.m_viewEntryNode.Refresh_EntryForm(gameObject, form);
			}
			CUIFormScript form2 = Singleton<CUIManager>.instance.GetForm(CRoomSystem.PATH_ROOM);
			if (form2 != null && !form2.IsHided())
			{
				GameObject gameObject2 = form2.transform.Find("entry_node").gameObject;
				this.m_viewEntryNode.Refresh_EntryForm(gameObject2, form2);
			}
			CUIFormScript form3 = Singleton<CUIManager>.instance.GetForm(CMatchingSystem.PATH_MATCHING_MULTI);
			if (form3 != null && !form3.IsHided())
			{
				GameObject gameObject3 = form3.transform.Find("entry_node").gameObject;
				this.m_viewEntryNode.Refresh_EntryForm(gameObject3, form3);
			}
			CUIFormScript form4 = Singleton<CUIManager>.instance.GetForm(SettlementSystem.SettlementFormName);
			if (form4 != null && !form4.IsHided())
			{
				GameObject gameObject4 = form4.transform.Find("Panel/entry_node").gameObject;
				this.m_viewEntryNode.Refresh_EntryForm(gameObject4, form4);
			}
			CUIFormScript form5 = Singleton<CUIManager>.instance.GetForm(CGuildMatchSystem.GuildMatchFormPath);
			if (form5 != null && !form5.IsHided())
			{
				GameObject gameObject5 = form5.transform.Find("entry_node").gameObject;
				this.m_viewEntryNode.Refresh_EntryForm(gameObject5, form5);
			}
		}
	}

	public void Clear_EntryForm_Node()
	{
		if (this.m_viewEntryNode != null)
		{
			this.m_viewEntryNode.Clear_EntryForm_Node();
		}
	}

	public void OpenSpeakerEntryNode(string constent)
	{
	}

	public void CloseSpeakerEntryNode()
	{
	}

	public void SetEntryChannelImage(EChatChannel v)
	{
		if (this.m_viewEntryNode != null)
		{
			this.m_viewEntryNode.SetEntryChannelImage(v);
		}
	}

	public void On_Chat_FaceList_Selected(CUIEvent uiEvent)
	{
		if (!this.bInited)
		{
			return;
		}
		int num = uiEvent.m_srcWidget.GetComponent<CUIListScript>().GetSelectedIndex() + 1;
		this.inputField.set_text(string.Format("{0}%{1}", this.inputField.get_text(), num));
	}

	public void SetChatFaceShow(bool bShow)
	{
		if (!this.bInited)
		{
			return;
		}
		this.ChatFaceListScript.gameObject.CustomSetActive(bShow);
		this.screenBtn.CustomSetActive(bShow);
		if (bShow && this.ChatFaceListScript.GetElementAmount() < CChatView.ChatFaceCount)
		{
			this.Rebuild_ChatFace_List();
		}
	}

	public void SetInputFiledEnable(bool bEnable)
	{
		if (bEnable)
		{
			this.deleteGameObject.CustomSetActive(false);
			this.inputField.MoveTextEnd(false);
		}
		else
		{
			this.deleteGameObject.CustomSetActive(false);
		}
	}

	public void On_Chat_ScreenButton_Click()
	{
		this.SetChatFaceShow(false);
	}

	public void RefreshGuildRecruitList()
	{
		CChatModel model = Singleton<CChatController>.GetInstance().model;
		if (model == null || model.sysData == null)
		{
			return;
		}
		CUIListScript component = this.chatForm.GetWidget(0).GetComponent<CUIListScript>();
		component.SetElementAmount(model.sysData.m_guildRecruitInfos.get_Count());
	}

	public void RefreshGuildRecruitInfoNode()
	{
		this.info_node_obj.CustomSetActive(true);
		this.info_text.set_text(Singleton<CTextManager>.GetInstance().GetText("Guild_Recruit_Chat_Tip"));
	}

	public void Process_Friend_Tip()
	{
		this.info_node_obj.CustomSetActive(true);
		bool flag = Singleton<CFriendContoller>.instance.model.IsAnyFriendExist(true);
		if (!Singleton<CFriendContoller>.instance.model.IsAnyFriendExist(false))
		{
			this.info_text.set_text(UT.GetText("Chat_NoFriend_Tip"));
		}
		else if (!flag)
		{
			this.info_text.set_text(UT.GetText("Chat_NoOnlineFriend_Tip"));
		}
		else
		{
			this.info_text.set_text(UT.GetText("Chat_HasFriend_Tip"));
		}
	}

	public void SetShow(CanvasGroup cg, bool bShow)
	{
		if (cg == null)
		{
			return;
		}
		if (bShow)
		{
			cg.alpha = 1f;
			cg.blocksRaycasts = true;
		}
		else
		{
			cg.alpha = 0f;
			cg.blocksRaycasts = false;
		}
	}

	public void On_Tab_Change(int index)
	{
		if (!this.bInited)
		{
			return;
		}
		if (index >= 0 && index < this.curChannels.get_Count())
		{
			this.CurTab = (EChatChannel)this.curChannels.get_Item(index);
		}
	}

	public void On_Friend_TabList_Selected(CUIEvent uiEvent)
	{
		if (!this.bInited)
		{
			return;
		}
		DebugHelper.Assert(uiEvent.m_srcWidgetIndexInBelongedList <= this.friendTablist.Count - 1, "---Chat, On_Friend_TabList_Selected");
		if (uiEvent.m_srcWidgetIndexInBelongedList > this.friendTablist.Count - 1)
		{
			return;
		}
		int selectedIndex = (uiEvent.m_srcWidgetScript as CUIListScript).GetSelectedIndex();
		COMDT_FRIEND_INFO cOMDT_FRIEND_INFO = this.friendTablist[selectedIndex];
		Singleton<CChatController>.GetInstance().model.sysData.ullUid = cOMDT_FRIEND_INFO.stUin.ullUid;
		Singleton<CChatController>.GetInstance().model.sysData.dwLogicWorldId = cOMDT_FRIEND_INFO.stUin.dwLogicWorldId;
		COfflineChatIndex cOfflineChatIndex = Singleton<CChatController>.instance.model.GetCOfflineChatIndex(cOMDT_FRIEND_INFO.stUin.ullUid, cOMDT_FRIEND_INFO.stUin.dwLogicWorldId);
		if (cOfflineChatIndex != null)
		{
			CChatNetUT.Send_Clear_Offline(cOfflineChatIndex.indexList);
			Singleton<CChatController>.instance.model.ClearCOfflineChatIndex(cOfflineChatIndex);
		}
		GameObject gameObject = uiEvent.m_srcWidget.GetComponent<CUIListScript>().GetElemenet(selectedIndex).gameObject;
		this._setRedPoint(gameObject.transform.FindChild("head/redPoint/Text").GetComponent<Text>(), 0);
		CUICommonSystem.DelRedDot(this.listScript.GetElemenet(0).gameObject);
		this.CurTab = EChatChannel.Friend_Chat;
		CUIListElementScript selectedElement = this.listScript.GetSelectedElement();
		Text component = selectedElement.gameObject.transform.FindChild("Text").GetComponent<Text>();
		component.set_text(Utility.UTF8Convert(cOMDT_FRIEND_INFO.szUserName));
	}

	public void Jump2FriendChat(COMDT_FRIEND_INFO info)
	{
		if (info == null)
		{
			return;
		}
		Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Chat_EntryPanel_Click);
		CChatModel model = Singleton<CChatController>.GetInstance().model;
		model.sysData.ullUid = info.stUin.ullUid;
		model.sysData.dwLogicWorldId = info.stUin.dwLogicWorldId;
		if (model.channelMgr._getChannel(EChatChannel.Friend, info.stUin.ullUid, info.stUin.dwLogicWorldId) == null)
		{
			CChatChannel cChatChannel = model.channelMgr.CreateChannel(EChatChannel.Friend, info.stUin.ullUid, info.stUin.dwLogicWorldId);
		}
		this.curChannels = Singleton<CChatController>.instance.model.channelMgr.CurActiveChannels;
		int num = this.curChannels.IndexOf(4u);
		if (num == -1)
		{
			this.curChannels.Add(4u);
			num = this.curChannels.get_Count() - 1;
		}
		this.BuildTabList(this.curChannels, num);
		this.CurTab = EChatChannel.Friend_Chat;
		CUIListElementScript selectedElement = this.listScript.GetSelectedElement();
		Text component = selectedElement.gameObject.transform.FindChild("Text").GetComponent<Text>();
		component.set_text(Utility.UTF8Convert(info.szUserName));
	}

	public void Refresh_All_RedPoint()
	{
		CChatModel model = Singleton<CChatController>.instance.model;
		if (this.curChannels == null || model == null || this.listScript == null)
		{
			return;
		}
		for (int i = 0; i < this.curChannels.get_Count(); i++)
		{
			uint num = this.curChannels.get_Item(i);
			int num2;
			if (num == 4u)
			{
				num2 = model.channelMgr.GetFriendTotal_UnreadCount();
			}
			else
			{
				CChatChannel channel = model.channelMgr.GetChannel((EChatChannel)num);
				if (channel != null)
				{
					num2 = channel.GetUnreadCount();
				}
				else
				{
					num2 = 0;
				}
			}
			CUIListElementScript elemenet = this.listScript.GetElemenet(i);
			if (!(elemenet == null))
			{
				if (num2 > 0)
				{
					CUICommonSystem.AddRedDot(elemenet.gameObject, enRedDotPos.enTopRight, num2, 0, 0);
				}
				else
				{
					CUICommonSystem.DelRedDot(elemenet.gameObject);
				}
			}
		}
	}

	private int GetUnReadCount(EChatChannel channelType)
	{
		int result;
		if (channelType == EChatChannel.Friend)
		{
			result = Singleton<CChatController>.instance.model.channelMgr.GetFriendTotal_UnreadCount();
		}
		else
		{
			CChatChannel channel = Singleton<CChatController>.instance.model.channelMgr.GetChannel(channelType);
			if (channel != null)
			{
				result = channel.GetUnreadCount();
			}
			else
			{
				result = 0;
			}
		}
		return result;
	}

	private void _setRedPoint(Text redText, int count)
	{
		if (redText == null)
		{
			return;
		}
		redText.gameObject.CustomSetActive(false);
		redText.transform.parent.gameObject.CustomSetActive(false);
		if (count > 0)
		{
			redText.transform.parent.gameObject.CustomSetActive(true);
			redText.set_text(count.ToString());
			if (count <= 9 && count >= 1)
			{
				redText.gameObject.CustomSetActive(true);
			}
		}
	}

	public void Refresh_ChatEntity_List(bool bForce = true, EChatChannel tab = EChatChannel.None)
	{
		if (this.nodeGameObject == null)
		{
			return;
		}
		if (!bForce && this.CurTab != tab)
		{
			return;
		}
		if (!this.nodeGameObject.activeSelf || !this.nodeGameObject.activeInHierarchy)
		{
			return;
		}
		ListView<CChatEntity> listView = null;
		CChatChannel cChatChannel = null;
		if (this.CurTab == EChatChannel.Lobby)
		{
			cChatChannel = Singleton<CChatController>.GetInstance().model.channelMgr.GetChannel(EChatChannel.Lobby);
			listView = cChatChannel.list;
			cChatChannel.ReadAll();
		}
		else if (this.CurTab == EChatChannel.Room)
		{
			cChatChannel = Singleton<CChatController>.GetInstance().model.channelMgr.GetChannel(EChatChannel.Room);
			listView = cChatChannel.list;
			cChatChannel.ReadAll();
		}
		else if (this.CurTab == EChatChannel.Guild)
		{
			cChatChannel = Singleton<CChatController>.GetInstance().model.channelMgr.GetChannel(EChatChannel.Guild);
			listView = cChatChannel.list;
			cChatChannel.ReadAll();
		}
		else if (this.CurTab == EChatChannel.GuildMatchTeam)
		{
			cChatChannel = Singleton<CChatController>.GetInstance().model.channelMgr.GetChannel(EChatChannel.GuildMatchTeam);
			listView = cChatChannel.list;
			cChatChannel.ReadAll();
		}
		else if (this.CurTab == EChatChannel.Friend_Chat)
		{
			CChatSysData sysData = Singleton<CChatController>.instance.model.sysData;
			cChatChannel = Singleton<CChatController>.GetInstance().model.channelMgr.GetFriendChannel(sysData.ullUid, sysData.dwLogicWorldId);
			listView = cChatChannel.list;
			this.Flag_Readed(EChatChannel.Friend_Chat);
		}
		else
		{
			if (this.CurTab == EChatChannel.Friend)
			{
				CFriendModel model = Singleton<CFriendContoller>.GetInstance().model;
				this.friendTablist = model.GetValidChatFriendList();
				this.friendTablist.Sort(new Comparison<COMDT_FRIEND_INFO>(CFriendModel.FriendDataSortForChatFriendList));
				this._refresh_friends_list(this.FriendTabListScript, this.friendTablist);
				return;
			}
			if (this.CurTab == EChatChannel.Team)
			{
				cChatChannel = Singleton<CChatController>.GetInstance().model.channelMgr.GetChannel(EChatChannel.Team);
				listView = cChatChannel.list;
				cChatChannel.ReadAll();
			}
			else if (this.CurTab == EChatChannel.Settle)
			{
				cChatChannel = Singleton<CChatController>.GetInstance().model.channelMgr.GetChannel(EChatChannel.Settle);
				listView = cChatChannel.list;
				cChatChannel.ReadAll();
			}
		}
		if (listView != null)
		{
			this._refresh_list(this.LobbyScript, listView, true, cChatChannel.sizeVec, cChatChannel);
		}
		this.Refresh_ChatInputView();
	}

	public void Refresh_ChatInputView()
	{
		if (this.inputField == null || this.sendBtn == null)
		{
			return;
		}
		if (this.CurTab == EChatChannel.Lobby)
		{
			CChatChannel channel = Singleton<CChatController>.instance.model.channelMgr.GetChannel(EChatChannel.Lobby);
			CChatController.enCheckChatResult enCheckChatResult = Singleton<CChatController>.instance.CheckSend(EChatChannel.Lobby, string.Empty, false);
			if (enCheckChatResult == CChatController.enCheckChatResult.CdLimit)
			{
				this.inputField.get_placeholder().GetComponent<Text>().set_text(string.Format(Singleton<CTextManager>.instance.GetText("Chat_Common_Tips_2"), channel.Get_Left_CDTime()));
			}
			else if (!CSysDynamicBlock.bChatPayBlock)
			{
				this.inputField.get_placeholder().GetComponent<Text>().set_text(Singleton<CTextManager>.instance.GetText("Chat_Common_Tips_1", new string[]
				{
					Singleton<CChatController>.GetInstance().model.sysData.restChatFreeCnt.ToString()
				}));
				bool flag = Singleton<CChatController>.GetInstance().model.sysData.restChatFreeCnt > 0u;
				this.sendBtn.transform.GetChild(0).gameObject.CustomSetActive(!flag);
				this.sendBtn.transform.GetChild(1).gameObject.CustomSetActive(flag);
				if (!flag)
				{
					GameObject gameObject = this.sendBtn.transform.Find("CostObj").gameObject;
					enPayType payType = CMallSystem.ResBuyTypeToPayType(Singleton<CChatController>.GetInstance().model.sysData.chatCostType);
					if (this.chatForm != null)
					{
						CHeroSkinBuyManager.SetPayCostIcon(this.chatForm, gameObject.transform, payType);
					}
					gameObject.transform.Find("priceTxt").GetComponent<Text>().set_text(string.Format("x{0}", Singleton<CChatController>.GetInstance().model.sysData.chatCostNum));
				}
			}
			else
			{
				this.inputField.get_placeholder().GetComponent<Text>().set_text(Singleton<CTextManager>.instance.GetText("Chat_Common_Tips_5"));
				this.sendBtn.transform.GetChild(0).gameObject.CustomSetActive(false);
				this.sendBtn.transform.GetChild(1).gameObject.CustomSetActive(true);
			}
		}
		else if (this.CurTab == EChatChannel.Team)
		{
			this.inputField.get_placeholder().GetComponent<Text>().set_text(Singleton<CTextManager>.instance.GetText("Chat_Common_Tips_11"));
			this.sendBtn.transform.GetChild(0).gameObject.CustomSetActive(false);
			this.sendBtn.transform.GetChild(1).gameObject.CustomSetActive(true);
		}
		else
		{
			this.inputField.get_placeholder().GetComponent<Text>().set_text(Singleton<CTextManager>.instance.GetText("Chat_Common_Tips_5"));
			this.sendBtn.transform.GetChild(0).gameObject.CustomSetActive(false);
			this.sendBtn.transform.GetChild(1).gameObject.CustomSetActive(true);
		}
	}

	public bool IsCheckHistory()
	{
		ListView<CChatEntity> listView = this._getList();
		if (listView == null || this.LobbyScript == null)
		{
			return false;
		}
		int count = listView.Count;
		return count != 0 && !this.LobbyScript.IsElementInScrollArea(count - 1);
	}

	private ListView<CChatEntity> _getList()
	{
		ListView<CChatEntity> result = null;
		if (this.CurTab == EChatChannel.Lobby)
		{
			CChatChannel channel = Singleton<CChatController>.GetInstance().model.channelMgr.GetChannel(EChatChannel.Lobby);
			if (channel == null)
			{
				return null;
			}
			result = channel.list;
		}
		else if (this.CurTab == EChatChannel.Room)
		{
			CChatChannel channel2 = Singleton<CChatController>.GetInstance().model.channelMgr.GetChannel(EChatChannel.Room);
			if (channel2 == null)
			{
				return null;
			}
			result = channel2.list;
		}
		else if (this.CurTab == EChatChannel.Guild)
		{
			CChatChannel channel3 = Singleton<CChatController>.GetInstance().model.channelMgr.GetChannel(EChatChannel.Guild);
			if (channel3 == null)
			{
				return null;
			}
			result = channel3.list;
		}
		else if (this.CurTab == EChatChannel.GuildMatchTeam)
		{
			CChatChannel channel4 = Singleton<CChatController>.GetInstance().model.channelMgr.GetChannel(EChatChannel.GuildMatchTeam);
			if (channel4 == null)
			{
				return null;
			}
			result = channel4.list;
		}
		else if (this.CurTab == EChatChannel.Friend_Chat)
		{
			CChatSysData sysData = Singleton<CChatController>.instance.model.sysData;
			if (sysData == null)
			{
				return null;
			}
			CChatChannel friendChannel = Singleton<CChatController>.GetInstance().model.channelMgr.GetFriendChannel(sysData.ullUid, sysData.dwLogicWorldId);
			if (friendChannel == null)
			{
				return null;
			}
			result = friendChannel.list;
		}
		else if (this.CurTab == EChatChannel.Team)
		{
			CChatChannel channel5 = Singleton<CChatController>.GetInstance().model.channelMgr.GetChannel(EChatChannel.Team);
			if (channel5 == null)
			{
				return null;
			}
			result = channel5.list;
		}
		else if (this.CurTab == EChatChannel.Settle)
		{
			CChatChannel channel6 = Singleton<CChatController>.GetInstance().model.channelMgr.GetChannel(EChatChannel.Settle);
			if (channel6 == null)
			{
				return null;
			}
			result = channel6.list;
		}
		return result;
	}

	public void On_List_ElementEnable(CUIEvent uievent)
	{
		if (uievent == null)
		{
			return;
		}
		int srcWidgetIndexInBelongedList = uievent.m_srcWidgetIndexInBelongedList;
		CChatEntity cChatEntity = null;
		ListView<CChatEntity> listView = this._getList();
		if (listView == null)
		{
			return;
		}
		if (srcWidgetIndexInBelongedList >= 0 && srcWidgetIndexInBelongedList < listView.Count)
		{
			cChatEntity = listView[srcWidgetIndexInBelongedList];
		}
		if (cChatEntity != null && uievent.m_srcWidget != null)
		{
			this.Show_ChatItem(uievent.m_srcWidget, cChatEntity, uievent.m_srcFormScript);
		}
		if (srcWidgetIndexInBelongedList == listView.Count - 1 && this.bubbleNode != null)
		{
			this.bubbleNode.CustomSetActive(false);
		}
	}

	public void ShowLoudSpeaker(bool bShow, COMDT_CHAT_MSG_HORN data = null)
	{
		if (this.loudSpeakerNode == null)
		{
			return;
		}
		if (data != null)
		{
			CUIHttpImageScript componetInChild = Utility.GetComponetInChild<CUIHttpImageScript>(this.loudSpeakerNode, "pnlSnsHead/HttpImage");
			Text componetInChild2 = Utility.GetComponetInChild<Text>(this.loudSpeakerNode, "name");
			Text componetInChild3 = Utility.GetComponetInChild<Text>(this.loudSpeakerNode, "content");
			Image componetInChild4 = Utility.GetComponetInChild<Image>(this.loudSpeakerNode, "pnlSnsHead/HttpImage/NobeIcon");
			Image componetInChild5 = Utility.GetComponetInChild<Image>(this.loudSpeakerNode, "pnlSnsHead/HttpImage/NobeImag");
			Text componetInChild6 = Utility.GetComponetInChild<Text>(this.loudSpeakerNode, "pnlSnsHead/HttpImage/bg/level");
			CUIEventScript componetInChild7 = Utility.GetComponetInChild<CUIEventScript>(this.loudSpeakerNode, "pnlSnsHead");
			componetInChild7.m_onClickEventID = enUIEventID.Chat_Form_Open_Mini_Player_Info_Form;
			componetInChild7.m_onClickEventParams.commonUInt64Param1 = data.stFrom.ullUid;
			componetInChild7.m_onClickEventParams.tag2 = data.stFrom.iLogicWorldID;
			string text;
			string text2;
			string text3;
			string url;
			COMDT_GAME_VIP_CLIENT cOMDT_GAME_VIP_CLIENT;
			CChatUT.GetUser(EChaterType.LoudSpeaker, data.stFrom.ullUid, (uint)data.stFrom.iLogicWorldID, out text, out text2, out text3, out url, out cOMDT_GAME_VIP_CLIENT);
			componetInChild2.set_text(text);
			componetInChild3.set_text(Utility.UTF8Convert(data.szContent));
			componetInChild6.set_text(text3);
			UT.SetHttpImage(componetInChild, url);
			if (cOMDT_GAME_VIP_CLIENT != null)
			{
				MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(componetInChild4, (int)cOMDT_GAME_VIP_CLIENT.dwCurLevel, false, true, 0uL);
				MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(componetInChild5, (int)cOMDT_GAME_VIP_CLIENT.dwHeadIconId);
			}
			if (componetInChild3.get_preferredHeight() > 60f)
			{
				this.loudSpeakerNode.GetComponent<RectTransform>().sizeDelta = new Vector2(544f, 106f);
			}
			else
			{
				this.loudSpeakerNode.GetComponent<RectTransform>().sizeDelta = new Vector2(544f, 86f);
			}
		}
		this.loudSpeakerNode.CustomSetActive(bShow && data != null);
	}

	private void _refresh_list(CUIListScript listScript, ListView<CChatEntity> data_list, bool bShow_Last, List<Vector2> sizeVec, CChatChannel channel)
	{
		if (listScript == null || channel == null)
		{
			return;
		}
		this.calc_size(data_list, sizeVec);
		int count = data_list.Count;
		listScript.SetElementAmount(count, sizeVec);
		if (this.bRefreshNew)
		{
			listScript.MoveElementInScrollArea(count - 1, true);
		}
		int start_index = 0;
		for (int i = 0; i < count; i++)
		{
			CUIListElementScript elemenet = listScript.GetElemenet(i);
			if (elemenet != null && listScript.IsElementInScrollArea(i))
			{
				this.Show_ChatItem(elemenet.gameObject, data_list[i], listScript.m_belongedFormScript);
				start_index = i;
			}
		}
		if (!this.bRefreshNew)
		{
			int unreadMeanbleChatEntCount = channel.GetUnreadMeanbleChatEntCount(start_index);
			if (unreadMeanbleChatEntCount > 0)
			{
				if (this.bubbleNode != null)
				{
					this.bubbleNode.CustomSetActive(true);
				}
				Text component = this.bubbleNode.transform.Find("Text").GetComponent<Text>();
				if (component != null)
				{
					if (unreadMeanbleChatEntCount > CChatView.max_bubble_num)
					{
						component.set_text(string.Format("{0}+", CChatView.max_bubble_num));
					}
					else
					{
						component.set_text(unreadMeanbleChatEntCount.ToString());
					}
				}
			}
			else if (this.bubbleNode != null)
			{
				this.bubbleNode.CustomSetActive(false);
			}
		}
		else if (this.bubbleNode != null)
		{
			this.bubbleNode.CustomSetActive(false);
		}
		this.bRefreshNew = true;
	}

	private void calc_size(ListView<CChatEntity> data_list, List<Vector2> sizeVec)
	{
		if (data_list == null || sizeVec == null)
		{
			return;
		}
		sizeVec.Clear();
		for (int i = 0; i < data_list.Count; i++)
		{
			CChatEntity cChatEntity = data_list[i];
			if (cChatEntity.type == EChaterType.LeaveRoom)
			{
				sizeVec.Add(new Vector2(CChatParser.element_width, CChatParser.element_half_height));
			}
			else
			{
				float y = (cChatEntity.numLine > 1u) ? (CChatParser.element_height + (float)CChatParser.lineHeight) : CChatParser.element_height;
				sizeVec.Add(new Vector2(CChatParser.element_width, y));
			}
		}
	}

	private void _refresh_friends_list(CUIListScript listScript, ListView<COMDT_FRIEND_INFO> data_list)
	{
		if (listScript == null)
		{
			return;
		}
		int count = data_list.Count;
		listScript.SetElementAmount(count);
		for (int i = 0; i < count; i++)
		{
			CUIListElementScript elemenet = listScript.GetElemenet(i);
			if (elemenet != null && listScript.IsElementInScrollArea(i))
			{
				this.Show_FriendTabItem(elemenet.gameObject, data_list[i]);
			}
		}
	}

	public void On_FriendsList_ElementEnable(CUIEvent uievent)
	{
		int srcWidgetIndexInBelongedList = uievent.m_srcWidgetIndexInBelongedList;
		COMDT_FRIEND_INFO cOMDT_FRIEND_INFO = null;
		ListView<COMDT_FRIEND_INFO> listView = this.friendTablist;
		if (srcWidgetIndexInBelongedList >= 0 && srcWidgetIndexInBelongedList < listView.Count)
		{
			cOMDT_FRIEND_INFO = listView[srcWidgetIndexInBelongedList];
		}
		if (cOMDT_FRIEND_INFO != null && uievent.m_srcWidget != null)
		{
			this.Show_FriendTabItem(uievent.m_srcWidget, cOMDT_FRIEND_INFO);
		}
	}

	public void Rebuild_ChatFace_List()
	{
		this.ChatFaceListScript.SetElementAmount(CChatView.ChatFaceCount);
		for (int i = 0; i < CChatView.ChatFaceCount; i++)
		{
			Image component = this.ChatFaceListScript.GetElemenet(i).GetComponent<Image>();
			UT.SetChatFace(this.ChatFaceListScript.m_belongedFormScript, component, i + 1);
		}
	}

	public void Show_FriendTabItem(GameObject node, COMDT_FRIEND_INFO info)
	{
		node.transform.FindChild("name").GetComponent<Text>().set_text(UT.Bytes2String(info.szUserName));
		node.transform.FindChild("head/LevelBg/Level").GetComponent<Text>().set_text(info.dwPvpLvl.ToString());
		Text component = node.transform.FindChild("head/redPoint/Text").GetComponent<Text>();
		GameObject gameObject = node.transform.FindChild("head/pnlSnsHead/HttpImage").gameObject;
		string url = UT.Bytes2String(info.szHeadUrl);
		if (!CSysDynamicBlock.bSocialBlocked)
		{
			gameObject.GetComponent<CUIHttpImageScript>().SetImageUrl(Singleton<ApolloHelper>.GetInstance().ToSnsHeadUrl(url));
		}
		Image component2 = gameObject.GetComponent<Image>();
		if (component2 != null)
		{
			bool flag = Singleton<CFriendContoller>.instance.model.IsFriendOfflineOnline(info.stUin.ullUid, info.stUin.dwLogicWorldId);
			UT.SetImage(component2, !flag);
		}
		if (info.stGameVip != null)
		{
			GameObject gameObject2 = node.transform.Find("head/pnlSnsHead/NobeIcon").gameObject;
			MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(gameObject2.GetComponent<Image>(), (int)info.stGameVip.dwCurLevel, false, false, info.ullUserPrivacyBits);
			GameObject gameObject3 = node.transform.Find("head/pnlSnsHead/NobeImag").gameObject;
			MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(gameObject3.GetComponent<Image>(), (int)info.stGameVip.dwHeadIconId);
		}
		CChatChannel friendChannel = Singleton<CChatController>.GetInstance().model.channelMgr.GetFriendChannel(info.stUin.ullUid, info.stUin.dwLogicWorldId);
		this._setRedPoint(component, friendChannel.GetUnreadCount());
		CChatEntity last = friendChannel.GetLast();
		Text component3 = node.transform.FindChild("Text").GetComponent<Text>();
		if (last != null)
		{
			component3.set_text(last.text);
		}
		else
		{
			component3.set_text(Singleton<CTextManager>.instance.GetText("Chat_Common_Tips_3"));
		}
		if (info.stGameVip != null && info.stGameVip.dwCurLevel > 0u)
		{
			component3.set_color(CUIUtility.s_Text_Color_Vip_Chat_Other);
		}
	}

	public void Show_ChatItem(GameObject node, CChatEntity ent, CUIFormScript form)
	{
		if (node == null || ent == null)
		{
			return;
		}
		GameObject gameObject = node.transform.Find("self").gameObject;
		GameObject gameObject2 = node.transform.Find("other").gameObject;
		GameObject gameObject3 = node.transform.Find("system").gameObject;
		GameObject gameObject4 = node.transform.Find("offline").gameObject;
		GameObject gameObject5 = node.transform.Find("time").gameObject;
		GameObject gameObject6 = node.transform.Find("speaker_1").gameObject;
		GameObject gameObject7 = node.transform.Find("speaker_2").gameObject;
		gameObject.CustomSetActive(false);
		gameObject2.CustomSetActive(false);
		gameObject3.CustomSetActive(false);
		gameObject4.CustomSetActive(false);
		gameObject5.CustomSetActive(false);
		gameObject6.CustomSetActive(false);
		gameObject7.CustomSetActive(false);
		GameObject gameObject8;
		if (ent.type == EChaterType.System || ent.type == EChaterType.LeaveRoom)
		{
			gameObject8 = gameObject3;
		}
		else if (ent.type == EChaterType.OfflineInfo)
		{
			gameObject8 = gameObject4;
		}
		else if (ent.type == EChaterType.Self)
		{
			gameObject8 = gameObject;
		}
		else if (ent.type == EChaterType.Time)
		{
			gameObject8 = gameObject5;
		}
		else if (ent.type == EChaterType.Speaker)
		{
			gameObject8 = gameObject6;
		}
		else if (ent.type == EChaterType.LoudSpeaker)
		{
			gameObject8 = gameObject7;
		}
		else
		{
			gameObject8 = gameObject2;
			CUIEventScript componetInChild = Utility.GetComponetInChild<CUIEventScript>(gameObject8, "pnlSnsHead");
			if (componetInChild != null)
			{
				componetInChild.m_onClickEventParams.tagStr = ent.text;
				componetInChild.m_onClickEventParams.tagStr1 = ent.name;
				componetInChild.m_onClickEventParams.pwd = ent.openId;
			}
		}
		gameObject8.CustomSetActive(true);
		CUIEventScript componetInChild2 = Utility.GetComponetInChild<CUIEventScript>(gameObject8, "pnlSnsHead");
		if (componetInChild2 != null)
		{
			componetInChild2.m_onClickEventParams.commonUInt64Param1 = ent.ullUid;
			componetInChild2.m_onClickEventParams.tag2 = (int)ent.iLogicWorldID;
		}
		if (ent.type == EChaterType.System || ent.type == EChaterType.LeaveRoom)
		{
			this.ShowRawText(gameObject8, ent);
		}
		else if (ent.type == EChaterType.OfflineInfo)
		{
			this.ShowRawText(gameObject8, ent);
		}
		else if (ent.type == EChaterType.Time)
		{
			this.ShowRawText(gameObject8, ent);
		}
		else if (ent.type == EChaterType.Speaker)
		{
			this.ShowRich(gameObject8, ent, form);
		}
		else if (ent.type == EChaterType.LoudSpeaker)
		{
			this.ShowRich(gameObject8, ent, form);
		}
		else
		{
			this.ShowRich(gameObject8, ent, form);
		}
		ent.bHasReaded = true;
	}

	private void ShowRich(GameObject playerNode, CChatEntity ent, CUIFormScript form)
	{
		if (ent.TextObjList == null || ent.TextObjList.Count == 0)
		{
			return;
		}
		playerNode.transform.Find("name").gameObject.GetComponent<Text>().set_text(ent.name);
		GameObject gameObject = playerNode.transform.Find("pnlSnsHead/HttpImage").gameObject;
		CUIHttpImageScript component = gameObject.GetComponent<CUIHttpImageScript>();
		UT.SetHttpImage(component, ent.head_url);
		Image component2 = gameObject.GetComponent<Image>();
		if (component2 != null && ent.type == EChaterType.Friend && ent.type == EChaterType.Friend)
		{
			bool flag = Singleton<CFriendContoller>.instance.model.IsFriendOfflineOnline(ent.ullUid, ent.iLogicWorldID);
			UT.SetImage(component2, !flag);
		}
		if (ent.stGameVip != null)
		{
			GameObject gameObject2 = playerNode.transform.Find("pnlSnsHead/HttpImage/NobeIcon").gameObject;
			if (gameObject2 != null)
			{
				MonoSingleton<NobeSys>.GetInstance().SetNobeIcon(gameObject2.GetComponent<Image>(), (int)ent.stGameVip.dwCurLevel, false, true, 0uL);
			}
			GameObject gameObject3 = playerNode.transform.Find("pnlSnsHead/HttpImage/NobeImag").gameObject;
			if (gameObject3 != null)
			{
				MonoSingleton<NobeSys>.GetInstance().SetHeadIconBk(gameObject3.GetComponent<Image>(), (int)ent.stGameVip.dwHeadIconId);
			}
		}
		playerNode.transform.Find("pnlSnsHead/HttpImage/bg/level").gameObject.GetComponent<Text>().set_text(ent.level);
		GameObject gameObject4 = playerNode.transform.Find("textImgNode").gameObject;
		GameObject gameObject5 = Utility.FindChild(playerNode, "actionBtn");
		gameObject5.CustomSetActive(false);
		CChatUT.DestoryAllChild(gameObject4);
		bool flag2 = false;
		for (int i = 0; i < ent.TextObjList.Count; i++)
		{
			CTextImageNode cTextImageNode = ent.TextObjList[i];
			if (cTextImageNode.type == CChatParser.InfoType.Text && !flag2)
			{
				flag2 = (cTextImageNode.posY <= -52f);
			}
			if (ent.stGameVip != null)
			{
				this.Create_Content(gameObject4, cTextImageNode.content, cTextImageNode.type, cTextImageNode.posX, cTextImageNode.posY, cTextImageNode.width, ent.type == EChaterType.Self, ent.stGameVip.dwCurLevel > 0u, false, form, gameObject5);
			}
		}
		RectTransform rectTransform = playerNode.transform.Find("textImgNode").transform as RectTransform;
		if (ent.numLine == 1u)
		{
			rectTransform.sizeDelta = new Vector2(ent.final_width, CChatView.single_line_height);
		}
		else if (ent.numLine == 2u)
		{
			rectTransform.sizeDelta = new Vector2(ent.final_width, CChatView.double_line_height);
		}
		else
		{
			rectTransform.sizeDelta = new Vector2(ent.final_width, CChatView.trible_line_height);
		}
	}

	private void ShowRawText(GameObject playerNode, CChatEntity ent)
	{
		Text component = playerNode.transform.Find("Text").GetComponent<Text>();
		if (component == null)
		{
			return;
		}
		component.set_text(ent.text);
		if (ent.type == EChaterType.LeaveRoom)
		{
			component.set_alignment(TextAnchor.MiddleLeft);
		}
		else
		{
			component.set_alignment(TextAnchor.UpperLeft);
		}
		RectTransform rectTransform = component.transform as RectTransform;
		if (rectTransform != null)
		{
			rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, CChatView.double_line_height);
		}
	}

	private void Create_Content(GameObject pNode, string content, CChatParser.InfoType type, float x, float y, float width, bool bSelf, bool bVip = false, bool bUse_Entry = false, CUIFormScript form = null, GameObject actionBtnNode = null)
	{
		actionBtnNode.CustomSetActive(false);
		if (type == CChatParser.InfoType.Text)
		{
			if (this.text_template == null || pNode == null)
			{
				return;
			}
			Text text = this.GetText("text", this.text_template, pNode);
			text.set_text(CUIUtility.RemoveEmoji(content));
			if (!bVip)
			{
				if (bSelf)
				{
					text.set_color(new Color(0f, 0f, 0f));
				}
				else
				{
					text.set_color(CUIUtility.s_Color_White);
				}
			}
			else if (bSelf)
			{
				text.set_color(CUIUtility.s_Text_Color_Vip_Chat_Self);
			}
			else
			{
				text.set_color(CUIUtility.s_Text_Color_Vip_Chat_Other);
			}
			if (bUse_Entry)
			{
				text.set_color(CUIUtility.s_Color_White);
			}
			if (bUse_Entry)
			{
				text.get_rectTransform().anchoredPosition = new Vector2(x, -(float)CChatParser.chat_entry_lineHeight);
			}
			else
			{
				text.get_rectTransform().anchoredPosition = new Vector2(x, y);
			}
			text.get_rectTransform().sizeDelta = new Vector2(width, text.get_rectTransform().sizeDelta.y);
		}
		else if (type == CChatParser.InfoType.Face)
		{
			int index;
			int.TryParse(content, ref index);
			Image image = this.GetImage(index, this.image_template, pNode);
			image.get_rectTransform().anchoredPosition = new Vector2(x, y);
		}
		else
		{
			string btnName = null;
			string text2 = null;
			enUIEventID uiEventID = enUIEventID.None;
			if (!CChatParser.ParseActionBtnContent(content, out btnName, out text2, out uiEventID))
			{
				return;
			}
			if (actionBtnNode != null)
			{
				Singleton<CChatController>.GetInstance().FilterActionBtn(uiEventID, actionBtnNode, btnName, form);
			}
		}
	}

	private Text GetText(string name, GameObject template, GameObject pNode)
	{
		GameObject gameObject = (GameObject)Object.Instantiate(template);
		gameObject.name = "text";
		gameObject.CustomSetActive(true);
		gameObject.transform.SetParent(pNode.transform);
		Text component = gameObject.GetComponent<Text>();
		component.get_rectTransform().localPosition = new Vector3(0f, 0f, 0f);
		component.get_rectTransform().pivot = new Vector2(0f, 0f);
		component.get_rectTransform().localScale = new Vector3(1f, 1f, 1f);
		return component;
	}

	private Image GetImage(int index, GameObject template, GameObject pNode)
	{
		GameObject gameObject = (GameObject)Object.Instantiate(template);
		gameObject.name = "image";
		gameObject.CustomSetActive(true);
		gameObject.transform.SetParent(pNode.transform);
		Image component = gameObject.GetComponent<Image>();
		component.get_rectTransform().localPosition = new Vector3(0f, 0f, 0f);
		component.get_rectTransform().pivot = new Vector2(0f, 0f);
		component.get_rectTransform().localScale = new Vector3(1f, 1f, 1f);
		if (index > 77)
		{
			index = 77;
		}
		if (index < 1)
		{
			index = 1;
		}
		component.SetSprite(string.Format("UGUI/Sprite/Dynamic/ChatFace/{0}", index), null, true, false, false, false);
		component.SetNativeSize();
		return component;
	}

	public void Flag_Readed(EChatChannel e)
	{
		if (e == EChatChannel.Friend_Chat)
		{
			CChatSysData sysData = Singleton<CChatController>.instance.model.sysData;
			Singleton<CChatController>.GetInstance().model.channelMgr.GetFriendChannel(sysData.ullUid, sysData.dwLogicWorldId).ReadAll();
		}
		else
		{
			Singleton<CChatController>.GetInstance().model.channelMgr.GetChannel(e).ReadAll();
		}
	}
}
