using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using System.Text.RegularExpressions;
using UnityEngine;

public class CChatParser
{
	public class LabelType
	{
		public string info;

		public CChatParser.InfoType type;

		public LabelType(string text, CChatParser.InfoType tp)
		{
			this.info = text;
			this.type = tp;
		}
	}

	public enum InfoType
	{
		Text,
		Face,
		Button
	}

	private const string SPEAKER_STR = "        ";

	private const string SPEAKER_REP_STR = "{0}{1}";

	public static int chat_entry_lineHeight = 39;

	public static int chat_list_max_width = 355;

	public static int chat_guild_list_max_width = 308;

	public static int chat_entry_channel_img_width = 47;

	public static int start_x = 53;

	public static int chatFaceWidth = 26;

	public static int chatFaceHeight = 34;

	public static int lineHeight = 34;

	public int maxWidth;

	public int viewFontSize = 18;

	public static float element_width = 547.4f;

	public static float element_height = 86f;

	public static float element_half_height = 43f;

	private int mPositionX;

	private int mPositionY;

	private int mOriginalPositionX = 5;

	private int mWidth;

	private ListView<CChatParser.LabelType> mList = new ListView<CChatParser.LabelType>();

	private CChatEntity curEntNode;

	public int chatCell_InitHeight = 80;

	public int chatCell_patchHeight = 10;

	public int chatCell_DeltaHeight = 30;

	public bool bProc_ChatEntry;

	public static int[] ascii_width = new int[]
	{
		7,
		6,
		7,
		15,
		12,
		20,
		15,
		4,
		9,
		9,
		10,
		17,
		6,
		6,
		6,
		6,
		12,
		12,
		12,
		12,
		12,
		12,
		12,
		12,
		12,
		12,
		6,
		6,
		17,
		17,
		17,
		11,
		20,
		15,
		13,
		15,
		16,
		12,
		11,
		18,
		16,
		7,
		10,
		14,
		10,
		20,
		17,
		18,
		13,
		18,
		14,
		13,
		11,
		16,
		14,
		20,
		14,
		13,
		14,
		8,
		6,
		8,
		20,
		10,
		10,
		14,
		14,
		11,
		14,
		13,
		7,
		14,
		13,
		6,
		7,
		13,
		6,
		19,
		13,
		13,
		14,
		14,
		9,
		10,
		8,
		13,
		11,
		18,
		11,
		12,
		11,
		10,
		10,
		10,
		17
	};

	public void Parse(string value, int startX, CChatEntity entNode)
	{
		if (string.IsNullOrEmpty(value))
		{
			return;
		}
		this.curEntNode = entNode;
		this.mList.Clear();
		this.mWidth = 0;
		this.mPositionY = -CChatParser.chatFaceHeight;
		this.mPositionX = (this.mOriginalPositionX = 8);
		this.ParseText(value);
		this.ShowText();
	}

	private void ParseText(string value)
	{
		if (this.curEntNode != null && (this.curEntNode.type == EChaterType.Speaker || this.curEntNode.type == EChaterType.LoudSpeaker))
		{
			value = string.Format("{0}{1}", "        ", value);
		}
		string text = value;
		string pattern = "(%\\d+)|(\\[(\\d+)\\|(\\d+)\\|([a-z0-9]{32})\\])";
		MatchCollection matchCollection = null;
		if (!string.IsNullOrEmpty(value))
		{
			try
			{
				matchCollection = Regex.Matches(value, pattern);
			}
			catch (Exception)
			{
			}
		}
		if (matchCollection != null && matchCollection.Count > 0)
		{
			int count = matchCollection.Count;
			for (int i = 0; i < count; i++)
			{
				Match match = matchCollection[i];
				string value2 = match.Value;
				if (!string.IsNullOrEmpty(value2))
				{
					int num = text.IndexOf(value2);
					int startIndex = num + value2.Length;
					if (num > -1)
					{
						string text2 = text.Substring(0, num);
						if (!string.IsNullOrEmpty(text2))
						{
							this.mList.Add(new CChatParser.LabelType(text2, CChatParser.InfoType.Text));
						}
						if (!string.IsNullOrEmpty(value2) && value2.IndexOf("%") != -1)
						{
							this.mList.Add(new CChatParser.LabelType(value2, CChatParser.InfoType.Face));
						}
						if (!string.IsNullOrEmpty(value2) && value2.IndexOf("[") != -1 && match.Groups.Count == 6)
						{
							int num2 = Convert.ToInt32(match.Groups[3].ToString());
							int num3 = Convert.ToInt32(match.Groups[4].ToString());
							string a = Convert.ToString(match.Groups[5]);
							string b = Utility.CreateMD5Hash(string.Format("{0}{1}{2}", num2, num3, "j9cWiS6lPjw4g"), Utility.MD5_STRING_CASE.LOWER);
							if (a == b)
							{
								this.mList.Add(new CChatParser.LabelType(value2, CChatParser.InfoType.Button));
							}
						}
						text = text.Substring(startIndex);
					}
				}
			}
			if (!string.IsNullOrEmpty(text))
			{
				this.mList.Add(new CChatParser.LabelType(text, CChatParser.InfoType.Text));
			}
		}
		else
		{
			this.mList.Add(new CChatParser.LabelType(text, CChatParser.InfoType.Text));
		}
	}

	public static bool ParseActionBtnContent(string value, out string btnName, out string desc, out enUIEventID uiEventID)
	{
		string pattern = "\\[(\\d+)\\|(\\d+)\\|([a-z0-9]{32})\\]";
		MatchCollection matchCollection = null;
		btnName = null;
		uiEventID = enUIEventID.None;
		desc = null;
		if (!string.IsNullOrEmpty(value))
		{
			try
			{
				matchCollection = Regex.Matches(value, pattern);
			}
			catch (Exception)
			{
				return false;
			}
		}
		if (matchCollection != null && matchCollection.Count > 0)
		{
			Match match = matchCollection[0];
			if (match.Groups.Count == 4)
			{
				uiEventID = (enUIEventID)Convert.ToInt32(match.Groups[2].ToString());
				CChatParser.GetActionBtnNameAndDesc(uiEventID, out btnName, out desc);
				return true;
			}
		}
		return false;
	}

	private static void GetActionBtnNameAndDesc(enUIEventID uiEventID, out string btnName, out string btnDesc)
	{
		btnName = null;
		btnDesc = null;
		if (uiEventID == enUIEventID.Guild_JoinPlatformGroup)
		{
			btnName = Singleton<CTextManager>.GetInstance().GetText("Guild_Platform_Remind_Join_Chat_Btn_Text");
			btnDesc = Singleton<CTextManager>.GetInstance().GetText("Guild_Platform_Remind_Join_Chat_Text");
		}
	}

	private void ShowText()
	{
		int count = this.mList.Count;
		for (int i = 0; i < count; i++)
		{
			CChatParser.LabelType labelType = this.mList[i];
			switch (labelType.type)
			{
			case CChatParser.InfoType.Text:
				this.CreateText(labelType.info);
				break;
			case CChatParser.InfoType.Face:
				this.CreateTextFace(labelType.info);
				break;
			case CChatParser.InfoType.Button:
				this.CreateButton(labelType.info);
				break;
			}
		}
		if (this.curEntNode != null)
		{
			this.curEntNode.final_width += 8f;
		}
	}

	private void CreateText(string value)
	{
		string value2 = string.Empty;
		string value3 = string.Empty;
		int num = value.IndexOf("\\n");
		if (num != -1)
		{
			value2 = value.Substring(0, num);
			value3 = value.Substring(num + 2);
			if (!string.IsNullOrEmpty(value2))
			{
				this.CreateText(value2);
			}
			if (!this.bProc_ChatEntry)
			{
				return;
			}
			this.mPositionX = this.mOriginalPositionX;
			this.mPositionY -= CChatParser.lineHeight;
			if (!string.IsNullOrEmpty(value2))
			{
				this.CreateText(value3);
			}
		}
		else
		{
			string empty = string.Empty;
			float num2 = 0f;
			bool flag = this.WrapText(value, (float)this.mPositionX, out empty, out num2);
			if (flag)
			{
				int num3 = empty.IndexOf("\\n");
				value2 = empty.Substring(0, num3);
				value3 = empty.Substring(num3 + 2);
				if (!string.IsNullOrEmpty(value2))
				{
					this.CreateText(value2);
				}
				if (this.bProc_ChatEntry)
				{
					return;
				}
				this.mPositionX = this.mOriginalPositionX;
				this.mPositionY -= CChatParser.lineHeight;
				if (!string.IsNullOrEmpty(value3))
				{
					this.CreateText(value3);
				}
			}
			else
			{
				num2 += 2.5f;
				if (this.curEntNode == null)
				{
					return;
				}
				this.curEntNode.TextObjList.Add(new CTextImageNode(value, CChatParser.InfoType.Text, num2, 0f, (float)this.mPositionX, (float)this.mPositionY));
				if ((float)this.mPositionY < this.curEntNode.final_height)
				{
					this.curEntNode.final_height = (float)this.mPositionY;
				}
				if (this.curEntNode.final_height < (float)(-(float)CChatParser.lineHeight * 2))
				{
					this.curEntNode.numLine = 3u;
				}
				else if (this.curEntNode.final_height < (float)(-(float)CChatParser.lineHeight))
				{
					this.curEntNode.numLine = 2u;
				}
				else
				{
					this.curEntNode.numLine = 1u;
				}
				this.mPositionX += (int)num2;
				if ((float)this.mPositionX > this.curEntNode.final_width)
				{
					this.curEntNode.final_width = (float)this.mPositionX;
				}
			}
		}
	}

	private void CreateTextFace(string value)
	{
		if (this.curEntNode == null)
		{
			return;
		}
		bool flag = false;
		if (CChatParser.chatFaceWidth + this.mPositionX > this.maxWidth)
		{
			if (this.bProc_ChatEntry)
			{
				return;
			}
			this.mPositionX = this.mOriginalPositionX;
			this.mPositionY -= CChatParser.chatFaceHeight;
			this.curEntNode.numLine = 2u;
			flag = true;
		}
		this.curEntNode.TextObjList.Add(new CTextImageNode(value.Substring(1), CChatParser.InfoType.Face, (float)CChatParser.chatFaceWidth, 0f, (float)this.mPositionX, (float)(-(float)CChatParser.chatFaceHeight)));
		this.mPositionX += CChatParser.chatFaceWidth;
		this.mWidth = Mathf.Max(this.mPositionX, this.mWidth);
		if (!flag)
		{
			this.curEntNode.final_width += (float)CChatParser.chatFaceWidth;
		}
	}

	private void CreateButton(string value)
	{
		string value2 = string.Empty;
		string value3 = string.Empty;
		string text = null;
		string text2 = null;
		enUIEventID enUIEventID = enUIEventID.None;
		if (!CChatParser.ParseActionBtnContent(value, out text, out text2, out enUIEventID))
		{
			return;
		}
		string empty = string.Empty;
		float num = 0f;
		bool flag = this.WrapText(text2, (float)this.mPositionX, out empty, out num);
		if (flag)
		{
			int num2 = empty.IndexOf("\\n");
			value2 = empty.Substring(0, num2);
			value3 = empty.Substring(num2 + 2);
			if (!string.IsNullOrEmpty(value2))
			{
				this.CreateText(value2);
			}
			if (this.bProc_ChatEntry)
			{
				return;
			}
			this.mPositionX = this.mOriginalPositionX;
			this.mPositionY -= CChatParser.lineHeight;
			if (!string.IsNullOrEmpty(value3))
			{
				this.CreateText(value3);
			}
			num += 2.5f;
			if (this.curEntNode == null)
			{
				return;
			}
			this.curEntNode.TextObjList.Add(new CTextImageNode(value, CChatParser.InfoType.Button, num, 0f, (float)this.mPositionX, (float)this.mPositionY));
		}
		else
		{
			if (!string.IsNullOrEmpty(empty))
			{
				this.CreateText(empty);
			}
			num += 2.5f;
			if (this.curEntNode == null)
			{
				return;
			}
			this.curEntNode.TextObjList.Add(new CTextImageNode(value, CChatParser.InfoType.Button, num, 0f, (float)this.mPositionX, (float)this.mPositionY));
			if ((float)this.mPositionY < this.curEntNode.final_height)
			{
				this.curEntNode.final_height = (float)this.mPositionY;
			}
			if (this.curEntNode.final_height < (float)(-(float)CChatParser.lineHeight * 2))
			{
				this.curEntNode.numLine = 3u;
			}
			else if (this.curEntNode.final_height < (float)(-(float)CChatParser.lineHeight))
			{
				this.curEntNode.numLine = 2u;
			}
			else
			{
				this.curEntNode.numLine = 1u;
			}
			this.mPositionX += (int)num;
			if ((float)this.mPositionX > this.curEntNode.final_width)
			{
				this.curEntNode.final_width = (float)this.mPositionX;
			}
		}
	}

	private bool WrapText(string text, float curPosX, out string finalText, out float length)
	{
		float num = curPosX;
		float num2 = 0f;
		int num3 = -1;
		for (int i = 0; i < text.Length; i++)
		{
			char ch = text[i];
			float num4 = (float)CChatParser.GetCharacterWidth(ch, this.viewFontSize);
			num += num4;
			num2 += num4;
			if (num > (float)this.maxWidth)
			{
				num3 = i;
				break;
			}
		}
		length = num2;
		if (num3 != -1)
		{
			text = text.Insert(num3, "\\n");
		}
		finalText = text;
		return num3 != -1;
	}

	public static int GetCharacterWidth(char ch, int fontSize)
	{
		if (ch >= ' ' && ch <= '~')
		{
			return CChatParser.ascii_width[(int)(ch - ' ')];
		}
		return fontSize;
	}
}
