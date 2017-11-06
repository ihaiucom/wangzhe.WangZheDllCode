using System;
using System.Collections.Generic;

public class AllColorParser
{
	private List<string> m_result = new List<string>();

	private int m_viewFontSize;

	private string Header_Half_Str = "<color=";

	private string tail_Str = "</color>";

	public List<string> Parse(int lingWidth, string content, int viewFontSize)
	{
		this.m_result.Clear();
		this.m_viewFontSize = viewFontSize;
		int num = 0;
		string text = this._parse(lingWidth, content, this.m_result, out num);
		if (!string.IsNullOrEmpty(text))
		{
			this.m_result.Add(text);
		}
		return this.m_result;
	}

	public List<string> Parse(int lingWidth, string content, int viewFontSize, out int actualWidth)
	{
		this.m_result.Clear();
		this.m_viewFontSize = viewFontSize;
		string text = this._parse(lingWidth, content, this.m_result, out actualWidth);
		if (!string.IsNullOrEmpty(text))
		{
			this.m_result.Add(text);
		}
		return this.m_result;
	}

	public string ParseSingleLine(int lingWidth, string content, int viewFontSize, out int actualWidth)
	{
		List<string> list = this.Parse(lingWidth, content, viewFontSize, out actualWidth);
		if (list.get_Count() > 0)
		{
			return list.get_Item(0);
		}
		return string.Empty;
	}

	private string _parse(int lingWidth, string content, List<string> result, out int actualWidth)
	{
		int i = 0;
		bool flag = true;
		bool flag2 = false;
		bool flag3 = false;
		int num = 0;
		int num2 = 0;
		string text = string.Empty;
		while (i < content.get_Length())
		{
			char c = content.get_Chars(i);
			if (c == '<')
			{
				flag2 = this.bHeader_Half_Match(i, content);
				if (flag2)
				{
					num2 = i;
					i += this.Header_Half_Str.get_Length();
					flag = false;
				}
				if (flag3 && this.bColorTail_Match(i, content))
				{
					i += this.tail_Str.get_Length();
					flag3 = false;
					continue;
				}
			}
			else if (c == '>' && flag2)
			{
				flag = true;
				flag3 = true;
				text = content.Substring(num2, i - num2 + 1);
				i++;
				continue;
			}
			if (flag)
			{
				int characterWidth = CChatParser.GetCharacterWidth(c, this.m_viewFontSize);
				if (num + characterWidth > lingWidth)
				{
					if (flag3)
					{
						string text2 = content.Substring(0, i);
						text2 += "</color>";
						actualWidth = num;
						result.Add(text2);
						string text3 = content.Substring(i);
						text3 = text + text3;
						int num3 = 0;
						string text4 = this._parse(lingWidth, text3, result, out num3);
						if (!string.IsNullOrEmpty(text4))
						{
							result.Add(text4);
						}
						return string.Empty;
					}
					actualWidth = num;
					string text5 = content.Substring(0, i);
					result.Add(text5);
					string content2 = content.Substring(i);
					int num4 = 0;
					string text6 = this._parse(lingWidth, content2, result, out num4);
					if (!string.IsNullOrEmpty(text6))
					{
						result.Add(text6);
					}
					return string.Empty;
				}
				else
				{
					num += characterWidth;
				}
			}
			i++;
		}
		actualWidth = num;
		return content;
	}

	private bool bHeader_Half_Match(int startIndex, string content)
	{
		return this.bMatch(startIndex, content, this.Header_Half_Str);
	}

	private bool bColorTail_Match(int startIndex, string content)
	{
		return this.bMatch(startIndex, content, this.tail_Str);
	}

	private bool bMatch(int startIndex, string content, string destStr)
	{
		int i = 0;
		while (i < destStr.get_Length())
		{
			if (content.get_Chars(startIndex) != destStr.get_Chars(i))
			{
				return false;
			}
			i++;
			startIndex++;
		}
		return true;
	}
}
