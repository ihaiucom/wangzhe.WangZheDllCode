using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using System.Collections.Generic;
using System.Text;

public class InBattleInputChat
{
	public enum EChatCamp
	{
		None = -1,
		Alice = 1,
		All = 0
	}

	public class InBatChatEntity
	{
		public enum EType
		{
			None = -1,
			Normal,
			System,
			ParseAllColor
		}

		public static readonly string COLOR_ENEMY = "<color=#ff463c>";

		public static readonly string COLOR_SYSTEM = "<color=#ffbe00>";

		public static readonly string COLOR_OTHERS = "<color=#18c8ff>";

		public static readonly string COLOR_WHITE = "<color=#ffffff>";

		public ulong ullUid;

		public byte camp;

		public bool bAlice;

		public InBattleInputChat.InBatChatEntity.EType type;

		public byte startIndex;

		public string line1;

		public string line2;

		public string rawText;

		public bool bShow;

		private uint m_maxCooldownTime;

		private ulong m_startCooldownTimestamp;

		private bool bInCD;

		public byte colorEnd;

		private bool bLine1Colored;

		private bool bLine2Colored;

		public void StartCooldown(uint maxCooldownTime = 0u)
		{
			if (maxCooldownTime == 0u)
			{
				maxCooldownTime = InBattleInputChat.Show_Max_Time;
			}
			this.m_maxCooldownTime = maxCooldownTime;
			if (maxCooldownTime > 0u)
			{
				this.m_startCooldownTimestamp = Singleton<FrameSynchr>.GetInstance().LogicFrameTick;
				this.bInCD = true;
			}
		}

		public void Shade(StringBuilder builder)
		{
			if (builder == null || this.colorEnd <= 0 || string.IsNullOrEmpty(this.line1))
			{
				return;
			}
			if ((int)this.colorEnd > this.line1.get_Length())
			{
				this.colorEnd = (byte)this.line1.get_Length();
			}
			if (this.type == InBattleInputChat.InBatChatEntity.EType.System)
			{
				if (!this.bLine1Colored && !string.IsNullOrEmpty(this.line1))
				{
					this.bLine1Colored = this._shade(builder, ref this.line1, this.line1.get_Length(), InBattleInputChat.InBatChatEntity.COLOR_SYSTEM);
				}
				if (!this.bLine2Colored && !string.IsNullOrEmpty(this.line2))
				{
					this.bLine2Colored = this._shade(builder, ref this.line2, this.line2.get_Length(), InBattleInputChat.InBatChatEntity.COLOR_SYSTEM);
				}
			}
			else
			{
				if (!this.bLine1Colored && !string.IsNullOrEmpty(this.line1))
				{
					this.bLine1Colored = this._shade(builder, ref this.line1, (int)this.colorEnd, (!this.bAlice) ? InBattleInputChat.InBatChatEntity.COLOR_ENEMY : InBattleInputChat.InBatChatEntity.COLOR_OTHERS);
				}
				if (!this.bLine2Colored && !string.IsNullOrEmpty(this.line2))
				{
					this.bLine2Colored = this._shade(builder, ref this.line2, this.line2.get_Length(), InBattleInputChat.InBatChatEntity.COLOR_WHITE);
				}
			}
		}

		private bool _shade(StringBuilder builder, ref string content, int endIndex, string color1)
		{
			if (builder == null || string.IsNullOrEmpty(content))
			{
				return false;
			}
			builder.Remove(0, builder.get_Length());
			builder.Append(content);
			if (endIndex != content.get_Length())
			{
				builder.Insert(0, color1);
				builder.Insert((int)this.colorEnd + color1.get_Length(), "</color><color=#ffffff>");
				builder.Insert(builder.get_Length(), "</color>");
			}
			else
			{
				builder.Insert(0, color1);
				builder.Insert(builder.get_Length(), "</color>");
			}
			content = builder.ToString();
			return true;
		}

		public bool IsInCoolDown()
		{
			return this.m_maxCooldownTime > 0u && this.bInCD;
		}

		public void UpdateCooldown()
		{
			if (!this.IsInCoolDown())
			{
				return;
			}
			uint num = (uint)(Singleton<FrameSynchr>.GetInstance().LogicFrameTick - this.m_startCooldownTimestamp);
			if (num >= this.m_maxCooldownTime)
			{
				this.bInCD = false;
				this.m_startCooldownTimestamp = 0uL;
				InBattleInputChat inputChat = Singleton<InBattleMsgMgr>.instance.m_InputChat;
				if (inputChat != null)
				{
					this.Clear();
					inputChat.ReclyChatEntity(this);
				}
			}
		}

		public void Clear()
		{
			this.colorEnd = 0;
			this.ullUid = 0uL;
			this.camp = 0;
			this.type = InBattleInputChat.InBatChatEntity.EType.Normal;
			this.startIndex = 0;
			this.line1 = (this.line2 = string.Empty);
			this.rawText = string.Empty;
			this.bShow = false;
			this.m_maxCooldownTime = 0u;
			this.m_startCooldownTimestamp = 0uL;
			this.bLine1Colored = (this.bLine2Colored = false);
			this.bInCD = false;
		}
	}

	public static int CHAT_LINE_COUNT = 4;

	public static int ChatEntityDisapearCDTime;

	public static uint Show_Max_Time = 10000u;

	private ListView<InBattleInputChat.InBatChatEntity> m_chatEntityList = new ListView<InBattleInputChat.InBatChatEntity>();

	private ListView<InBattleInputChat.InBatChatEntity> m_caches = new ListView<InBattleInputChat.InBatChatEntity>();

	private InBattleMsgInputView m_view;

	public InBattleInputChat.EChatCamp m_curChatCamp = InBattleInputChat.EChatCamp.None;

	public InBattleInputChat()
	{
		this.m_view = new InBattleMsgInputView();
		Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.InBattle_InputChat_SwitchCamp, new CUIEventManager.OnUIEventHandler(this.On_InputChat_SwitchCamp));
		Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.InBattle_InputChat_InputClick, new CUIEventManager.OnUIEventHandler(this.On_InputChat_InputClick));
	}

	public void Init(CUIFormScript battleFormScript)
	{
		if (battleFormScript == null)
		{
			return;
		}
		if (this.m_view != null)
		{
			this.m_view.Init(battleFormScript);
		}
		for (int i = 0; i < InBattleInputChat.CHAT_LINE_COUNT + 2; i++)
		{
			this.m_caches.Add(new InBattleInputChat.InBatChatEntity());
		}
		this.SetCurChatCamp(InBattleInputChat.EChatCamp.Alice);
	}

	public void Clear()
	{
		if (this.m_view != null)
		{
			this.m_view.Clear();
		}
		this.m_view = null;
		this.m_chatEntityList.Clear();
		this.m_caches.Clear();
		Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.InBattle_InputChat_SwitchCamp, new CUIEventManager.OnUIEventHandler(this.On_InputChat_SwitchCamp));
		Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.InBattle_InputChat_InputClick, new CUIEventManager.OnUIEventHandler(this.On_InputChat_InputClick));
	}

	public void SetInputChatEnable(int v)
	{
		if (this.m_view == null)
		{
			return;
		}
		bool flag = v == 1;
		this.m_view.SetChatLineNodeShowable(flag);
		this.m_view.SetChatButtonNodeShowable(flag);
	}

	private void On_InputChat_InputClick(CUIEvent uievent)
	{
		if (this.m_view != null)
		{
			this.m_view.EnableInputFiled();
		}
	}

	private void On_InputChat_SwitchCamp(CUIEvent uievent)
	{
		if (this.m_curChatCamp == InBattleInputChat.EChatCamp.Alice)
		{
			this.SetCurChatCamp(InBattleInputChat.EChatCamp.All);
		}
		else if (this.m_curChatCamp == InBattleInputChat.EChatCamp.All)
		{
			this.SetCurChatCamp(InBattleInputChat.EChatCamp.Alice);
		}
		else
		{
			this.SetCurChatCamp(InBattleInputChat.EChatCamp.Alice);
		}
	}

	private void SetCurChatCamp(InBattleInputChat.EChatCamp v)
	{
		if (this.m_curChatCamp == v)
		{
			return;
		}
		this.m_curChatCamp = v;
		if (this.m_view != null)
		{
			this.m_view.ShowCamp(this.m_curChatCamp);
		}
	}

	public void ServerDisableInputChat()
	{
		if (this.m_view != null)
		{
			this.m_view.SetChatButtonNodeCheckable(false);
			this.m_view.SetChatLineNodeShowable(false);
		}
	}

	public void Add(InBattleInputChat.InBatChatEntity ent)
	{
		if (ent == null)
		{
			return;
		}
		if (ent.type == InBattleInputChat.InBatChatEntity.EType.ParseAllColor)
		{
			List<string> list = Singleton<CChatController>.instance.ColorParser.Parse(this.m_view.lineWidth - 10, ent.rawText, this.m_view.fontSize);
			if (list.get_Count() == 1)
			{
				ent.line1 = list.get_Item(0);
			}
			else if (list.get_Count() >= 2)
			{
				ent.line1 = list.get_Item(0);
				ent.line2 = list.get_Item(1);
			}
		}
		else
		{
			bool flag = false;
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < ent.rawText.get_Length(); i++)
			{
				int characterWidth = CChatParser.GetCharacterWidth(ent.rawText.get_Chars(i), this.m_view.fontSize);
				if (num + characterWidth > this.m_view.lineWidth - 10)
				{
					flag = true;
					ent.line1 = ent.rawText.Substring(num2, i);
					ent.line2 = ent.rawText.Substring(i);
					break;
				}
				num += characterWidth;
			}
			if (!flag)
			{
				ent.line1 = ent.rawText;
			}
		}
		if (this.m_view != null && this.m_view.IsAllTextLineShowed())
		{
			this.m_chatEntityList.RemoveAt(0);
		}
		if (this.m_chatEntityList.Count == InBattleInputChat.CHAT_LINE_COUNT)
		{
			this.m_chatEntityList.RemoveAt(0);
		}
		this.m_chatEntityList.Add(ent);
		if (this.m_view != null)
		{
			this.m_view.Refresh(this.m_chatEntityList);
		}
	}

	public InBattleInputChat.InBatChatEntity ConstructEnt(ulong ullUid, string playerName, string content, byte camp)
	{
		InBattleInputChat.InBatChatEntity validChatEntity = this.GetValidChatEntity();
		string text = string.Empty;
		Player playerByUid = Singleton<GamePlayerCenter>.instance.GetPlayerByUid(ullUid);
		if (playerByUid != null && playerByUid.Captain)
		{
			text = playerByUid.Captain.handle.TheStaticData.TheResInfo.Name;
			validChatEntity.bAlice = playerByUid.Captain.handle.IsHostCamp();
		}
		validChatEntity.ullUid = ullUid;
		string text2 = string.Empty;
		if (camp == 0)
		{
			text2 = "[全部]";
		}
		string text3 = string.Format("{0}{1}({2}):", text2, playerName, text);
		validChatEntity.colorEnd = (byte)text3.get_Length();
		validChatEntity.rawText = string.Format("{0}{1}", text3, content);
		validChatEntity.camp = camp;
		validChatEntity.type = InBattleInputChat.InBatChatEntity.EType.Normal;
		return validChatEntity;
	}

	public InBattleInputChat.InBatChatEntity ConstructEnt(string content, InBattleInputChat.InBatChatEntity.EType type = InBattleInputChat.InBatChatEntity.EType.System)
	{
		InBattleInputChat.InBatChatEntity validChatEntity = this.GetValidChatEntity();
		validChatEntity.ullUid = 0uL;
		string text = string.Empty;
		if (type == InBattleInputChat.InBatChatEntity.EType.System)
		{
			text = "[系统]";
		}
		validChatEntity.type = type;
		validChatEntity.rawText = string.Format("{0} {1}", text, content);
		validChatEntity.colorEnd = (byte)validChatEntity.rawText.get_Length();
		return validChatEntity;
	}

	public InBattleInputChat.InBatChatEntity ConstructColorFlagEnt(string content)
	{
		InBattleInputChat.InBatChatEntity validChatEntity = this.GetValidChatEntity();
		validChatEntity.ullUid = 0uL;
		string text = "[系统]";
		validChatEntity.type = InBattleInputChat.InBatChatEntity.EType.ParseAllColor;
		validChatEntity.rawText = string.Format("{0}{1}{2} {3}", new object[]
		{
			InBattleInputChat.InBatChatEntity.COLOR_SYSTEM,
			text,
			"</color>",
			content
		});
		return validChatEntity;
	}

	private InBattleInputChat.InBatChatEntity GetValidChatEntity()
	{
		InBattleInputChat.InBatChatEntity result;
		if (this.m_caches.Count > 0)
		{
			result = this.m_caches[0];
			this.m_caches.RemoveAt(0);
		}
		else
		{
			result = new InBattleInputChat.InBatChatEntity();
		}
		return result;
	}

	private void ReclyChatEntity(InBattleInputChat.InBatChatEntity ent)
	{
		if (ent == null)
		{
			return;
		}
		int num = -1;
		for (int i = 0; i < this.m_chatEntityList.Count; i++)
		{
			InBattleInputChat.InBatChatEntity inBatChatEntity = this.m_chatEntityList[i];
			if (inBatChatEntity != null && ent.ullUid == inBatChatEntity.ullUid)
			{
				num = i;
				break;
			}
		}
		if (num != -1)
		{
			this.m_caches.Add(ent);
			this.m_chatEntityList.RemoveAt(num);
			if (this.m_view != null)
			{
				this.m_view.Refresh(this.m_chatEntityList);
			}
		}
	}

	public void Update()
	{
		for (int i = 0; i < this.m_chatEntityList.Count; i++)
		{
			InBattleInputChat.InBatChatEntity inBatChatEntity = this.m_chatEntityList[i];
			if (inBatChatEntity != null && inBatChatEntity.bShow)
			{
				inBatChatEntity.UpdateCooldown();
			}
		}
		if (this.m_view != null)
		{
			this.m_view.Update();
		}
	}
}
