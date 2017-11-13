using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InBattleMsgInputView
{
	private GameObject m_inputText;

	private Text m_campTxt;

	public InputField m_inputField;

	private GameObject m_lineNode;

	private GameObject m_buttonNode;

	private GameObject[] m_cacheNode = new GameObject[InBattleInputChat.CHAT_LINE_COUNT];

	private bool[] m_nodeUseage = new bool[InBattleInputChat.CHAT_LINE_COUNT];

	private int intimeSendCount;

	public int intimeMaxCount = 5;

	public int intimeMSecond = 10000;

	public ulong intimeMSecond_StartTime;

	public uint intime_cd = 7000u;

	private CDButton m_inputButton;

	private string m_inCDTxt;

	private string m_inWramBattle;

	private StringBuilder lineBuilder = new StringBuilder();

	public int lineWidth
	{
		get;
		private set;
	}

	public int fontSize
	{
		get;
		private set;
	}

	public void Clear()
	{
		for (int i = 0; i < this.m_cacheNode.Length; i++)
		{
			this.m_cacheNode[i] = null;
		}
		this.m_cacheNode = null;
		this.m_nodeUseage = null;
		this.m_lineNode = null;
		this.m_buttonNode = null;
		this.m_campTxt = null;
		this.m_inputText = null;
		this.m_inputField.get_onEndEdit().RemoveAllListeners();
		this.m_inputField = null;
		this.m_inCDTxt = (this.m_inWramBattle = null);
		if (this.m_inputButton != null)
		{
			this.m_inputButton.Clear();
		}
		this.m_inputButton = null;
		this.intimeSendCount = 0;
		this.intimeMSecond_StartTime = 0uL;
	}

	public void Init(CUIFormScript battleFormScript)
	{
		if (battleFormScript == null)
		{
			return;
		}
		this.m_lineNode = Utility.FindChild(battleFormScript.gameObject, "InputChat_Lines");
		CUIFormScript form = Singleton<CUIManager>.instance.GetForm(InBattleShortcut.InBattleMsgView_FORM_PATH);
		this.m_campTxt = Utility.FindChild(form.gameObject, "chatTools/node/InputChat_Buttons/Button_Camp/Text").GetComponent<Text>();
		this.m_buttonNode = Utility.FindChild(form.gameObject, "chatTools/node/InputChat_Buttons");
		this.m_inputField = Utility.FindChild(form.gameObject, "chatTools/node/InputChat_Buttons/Button_Input/InputField").GetComponent<InputField>();
		this.m_inputText = Utility.FindChild(this.m_inputField.gameObject, "Text");
		GameObject gameObject = Utility.FindChild(form.gameObject, "chatTools/node/InputChat_Buttons/Button_Input/CDButton");
		if (gameObject != null)
		{
			this.m_inputButton = new CDButton(gameObject);
		}
		DebugHelper.Assert(this.m_campTxt != null, "---InBattleMsgInputView m_campTxt == null, check out...");
		DebugHelper.Assert(this.m_lineNode != null, "---InBattleMsgInputView m_lineNode == null, check out...");
		DebugHelper.Assert(this.m_buttonNode != null, "---InBattleMsgInputView m_buttonNode == null, check out...");
		DebugHelper.Assert(this.m_inputField != null, "---InBattleMsgInputView m_inputField == null, check out...");
		DebugHelper.Assert(this.m_inputText != null, "---InBattleMsgInputView m_inputText == null, check out...");
		bool bActive = GameSettings.InBattleInputChatEnable == 1;
		if (this.m_lineNode != null)
		{
			this.m_lineNode.CustomSetActive(bActive);
		}
		if (this.m_buttonNode != null)
		{
			this.m_buttonNode.CustomSetActive(bActive);
		}
		for (int i = 0; i < InBattleInputChat.CHAT_LINE_COUNT; i++)
		{
			this.m_cacheNode[i] = Utility.FindChild(battleFormScript.gameObject, string.Format("InputChat_Lines/line{0}", i));
			DebugHelper.Assert(this.m_cacheNode[i] != null, "---InBattleMsgInputView m_cacheNode == null, index:" + i);
			if (this.m_cacheNode[i] != null)
			{
				this.m_cacheNode[i].CustomSetActive(false);
			}
		}
		if (this.m_inputField != null)
		{
			this.m_inputField.get_onEndEdit().AddListener(new UnityAction<string>(this.On_EndEdit));
		}
		int lineWidth;
		int fontSize;
		this.CalcLineWidth(out lineWidth, out fontSize);
		this.lineWidth = lineWidth;
		this.fontSize = fontSize;
		if (!int.TryParse(Singleton<CTextManager>.instance.GetText("InBat_IntimeMaxCount"), ref this.intimeMaxCount))
		{
			DebugHelper.Assert(false, "---InBatMsg jason 你配置的 InBat_IntimeMaxCount 好像不是整数哦， check out");
		}
		if (!int.TryParse(Singleton<CTextManager>.instance.GetText("InBat_IntimeMSecond"), ref this.intimeMSecond))
		{
			DebugHelper.Assert(false, "---InBatMsg jason 你配置的 InBat_IntimeMSecond 好像不是整数哦， check out");
		}
		if (!uint.TryParse(Singleton<CTextManager>.instance.GetText("InBat_IntimeCD"), ref this.intime_cd))
		{
			DebugHelper.Assert(false, "---InBatMsg jason 你配置的 InBat_IntimeCD 好像不是整数哦， check out");
		}
		this.m_inCDTxt = Singleton<CTextManager>.instance.GetText("InBatInput_InCD");
		this.m_inWramBattle = Singleton<CTextManager>.instance.GetText("InBatInput_InWarm");
	}

	private void CalcLineWidth(out int lineWidth, out int fontSize)
	{
		GameObject gameObject = this.m_cacheNode[0];
		if (gameObject == null)
		{
			lineWidth = 200;
			fontSize = 18;
			return;
		}
		RectTransform rectTransform = gameObject.transform as RectTransform;
		if (rectTransform == null)
		{
			lineWidth = 200;
			fontSize = 18;
			return;
		}
		lineWidth = (int)rectTransform.sizeDelta.x;
		Text component = gameObject.transform.Find("Text").GetComponent<Text>();
		if (component == null)
		{
			lineWidth = 200;
			fontSize = 18;
			return;
		}
		fontSize = component.get_fontSize();
	}

	public void Update()
	{
		if (this.m_inputButton != null)
		{
			this.m_inputButton.Update();
		}
	}

	private void On_EndEdit(string content)
	{
		Singleton<InBattleMsgMgr>.instance.HideView();
		if (this.m_inputField != null)
		{
			this.m_inputField.set_text(string.Empty);
		}
		if (this.intimeMSecond_StartTime != 0uL)
		{
			uint num = (uint)(Singleton<FrameSynchr>.GetInstance().LogicFrameTick - this.intimeMSecond_StartTime);
			if ((ulong)num < (ulong)((long)this.intimeMSecond))
			{
				if (this.intimeSendCount + 1 > this.intimeMaxCount)
				{
					if (this.m_inputButton != null)
					{
						this.m_inputButton.StartCooldown(this.intime_cd, new Action(this.OnBlockCDEnd));
					}
					InBattleInputChat inputChat = Singleton<InBattleMsgMgr>.instance.m_InputChat;
					if (inputChat != null)
					{
						InBattleInputChat.InBatChatEntity ent = inputChat.ConstructEnt(this.m_inCDTxt, InBattleInputChat.InBatChatEntity.EType.System);
						inputChat.Add(ent);
					}
					this.intimeSendCount = 0;
					return;
				}
			}
			else
			{
				this.intimeSendCount = 0;
			}
		}
		InBattleInputChat inputChat2 = Singleton<InBattleMsgMgr>.instance.m_InputChat;
		if (inputChat2 == null)
		{
			return;
		}
		if (string.IsNullOrEmpty(content))
		{
			return;
		}
		content = CUIUtility.RemoveEmoji(content);
		if (this.m_inputText != null)
		{
			this.m_inputText.CustomSetActive(false);
		}
		SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
		if (curLvelContext != null && !Singleton<InBattleMsgMgr>.instance.ShouldBeThroughNet(curLvelContext))
		{
			InBattleInputChat inputChat3 = Singleton<InBattleMsgMgr>.instance.m_InputChat;
			if (inputChat3 != null)
			{
				InBattleInputChat.InBatChatEntity ent2 = inputChat3.ConstructEnt(this.m_inWramBattle, InBattleInputChat.InBatChatEntity.EType.System);
				inputChat3.Add(ent2);
			}
		}
		else if (!string.IsNullOrEmpty(content))
		{
			InBattleMsgNetCore.SendInBattleMsg_InputChat(content, (byte)inputChat2.m_curChatCamp);
		}
		if (this.intimeSendCount == 0)
		{
			this.intimeMSecond_StartTime = Singleton<FrameSynchr>.GetInstance().LogicFrameTick;
		}
		this.intimeSendCount++;
	}

	private void OnBlockCDEnd()
	{
		this.intimeSendCount = 0;
	}

	public void EnableInputFiled()
	{
		if (this.m_inputField != null)
		{
			this.m_inputField.ActivateInputField();
		}
	}

	public void ShowCamp(InBattleInputChat.EChatCamp v)
	{
		if (v == InBattleInputChat.EChatCamp.None)
		{
			return;
		}
		if (v == InBattleInputChat.EChatCamp.Alice)
		{
			this.m_campTxt.set_text("我方");
		}
		else
		{
			this.m_campTxt.set_text("全部");
		}
	}

	public void Refresh(ListView<InBattleInputChat.InBatChatEntity> chatEntityList)
	{
		if (chatEntityList == null)
		{
			return;
		}
		int i = 0;
		int num = 0;
		while (num < chatEntityList.Count && i < InBattleInputChat.CHAT_LINE_COUNT)
		{
			InBattleInputChat.InBatChatEntity inBatChatEntity = chatEntityList[num];
			if (inBatChatEntity != null)
			{
				if (inBatChatEntity.type != InBattleInputChat.InBatChatEntity.EType.ParseAllColor)
				{
					inBatChatEntity.Shade(this.lineBuilder);
				}
				if (!string.IsNullOrEmpty(inBatChatEntity.line1) && this.ShowTextInLine(i, inBatChatEntity.line1, inBatChatEntity.camp, inBatChatEntity.type, inBatChatEntity.ullUid))
				{
					inBatChatEntity.bShow = true;
					if (this.m_cacheNode[i] != null)
					{
						this.m_cacheNode[i].CustomSetActive(true);
					}
					if (!inBatChatEntity.IsInCoolDown())
					{
						inBatChatEntity.StartCooldown(0u);
					}
					i++;
				}
				if (!string.IsNullOrEmpty(inBatChatEntity.line2) && this.ShowTextInLine(i, inBatChatEntity.line2, inBatChatEntity.camp, inBatChatEntity.type, inBatChatEntity.ullUid))
				{
					inBatChatEntity.bShow = true;
					if (this.m_cacheNode[i] != null)
					{
						this.m_cacheNode[i].CustomSetActive(true);
					}
					if (!inBatChatEntity.IsInCoolDown())
					{
						inBatChatEntity.StartCooldown(0u);
					}
					i++;
				}
			}
			num++;
		}
		while (i < InBattleInputChat.CHAT_LINE_COUNT)
		{
			if (this.m_cacheNode[i] != null)
			{
				this.m_cacheNode[i].CustomSetActive(false);
			}
			i++;
		}
	}

	public bool IsAllTextLineShowed()
	{
		for (int i = 0; i < this.m_cacheNode.Length; i++)
		{
			GameObject gameObject = this.m_cacheNode[i];
			if (gameObject != null && !gameObject.activeSelf)
			{
				return false;
			}
		}
		return true;
	}

	public void SetChatLineNodeShowable(bool bShow)
	{
		if (this.m_lineNode != null)
		{
			this.m_lineNode.gameObject.CustomSetActive(bShow);
		}
	}

	public void SetChatButtonNodeShowable(bool bShow)
	{
		if (this.m_buttonNode != null)
		{
			this.m_buttonNode.gameObject.CustomSetActive(bShow);
		}
	}

	public void SetChatButtonNodeCheckable(bool bEnable)
	{
		if (this.m_buttonNode != null)
		{
			GameObject gameObject = this.m_buttonNode.transform.Find("Button_Camp").gameObject;
			GameObject gameObject2 = this.m_buttonNode.transform.Find("Button_Input").gameObject;
			DebugHelper.Assert(gameObject != null, "---InBattleMsgInputView.SetChatButtonNodeCheckable campObj  != null");
			DebugHelper.Assert(gameObject2 != null, "---InBattleMsgInputView.SetChatButtonNodeCheckable inputObj != null");
			if (gameObject != null)
			{
				gameObject.GetComponent<CUIEventScript>().enabled = bEnable;
			}
			if (gameObject2 != null)
			{
				gameObject2.GetComponent<CUIEventScript>().enabled = bEnable;
			}
		}
	}

	private bool ShowTextInLine(int index, string content, byte camp, InBattleInputChat.InBatChatEntity.EType type, ulong uid)
	{
		if (index >= InBattleInputChat.CHAT_LINE_COUNT)
		{
			return false;
		}
		Text text = null;
		this.GetCom(index, out text);
		if (text != null)
		{
			text.set_text(content);
			return true;
		}
		return false;
	}

	private bool IsSameCamp(ulong uid)
	{
		Player playerByUid = Singleton<GamePlayerCenter>.instance.GetPlayerByUid(uid);
		return playerByUid != null && Singleton<GamePlayerCenter>.instance.IsAtSameCamp(Singleton<GamePlayerCenter>.instance.HostPlayerId, playerByUid.PlayerId);
	}

	private void GetCom(int index, out Text txtCom)
	{
		DebugHelper.Assert(index < InBattleInputChat.CHAT_LINE_COUNT, string.Format("---InBattleMsgInputView.GetCom index < CHAT_LINE_COUNT, index:{0},CHAT_LINE_COUNT:{1}", index, InBattleInputChat.CHAT_LINE_COUNT));
		txtCom = null;
		if (index >= InBattleInputChat.CHAT_LINE_COUNT)
		{
			return;
		}
		GameObject gameObject = this.m_cacheNode[index];
		DebugHelper.Assert(gameObject != null, string.Format("---InBattleMsgInputView.GetCom node != null, index:{0}", index));
		if (gameObject != null)
		{
			GameObject gameObject2 = gameObject.transform.Find("Text").gameObject;
			DebugHelper.Assert(gameObject2 != null, string.Format("---InBattleMsgInputView.GetCom txtObj != null, index:{0}", index));
			txtCom = gameObject2.GetComponent<Text>();
		}
	}
}
