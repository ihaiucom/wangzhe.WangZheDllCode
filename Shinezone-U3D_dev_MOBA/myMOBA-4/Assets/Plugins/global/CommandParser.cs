using System;
using System.Text.RegularExpressions;

public class CommandParser
{
	public static readonly string RegexPattern = " (?=(?:[^\\\"]*\\\"[^\\\"]*\\\")*[^\\\"]*$)";

	public string text
	{
		get;
		protected set;
	}

	public string[] sections
	{
		get;
		protected set;
	}

	public string baseCommand
	{
		get;
		protected set;
	}

	public string[] arguments
	{
		get;
		protected set;
	}

	public CommandParser()
	{
		string empty = string.Empty;
		this.baseCommand = empty;
		this.text = empty;
	}

	public void Parse(string InText)
	{
		this.text = InText;
		this.sections = Regex.Split(this.text, CommandParser.RegexPattern);
		for (int i = 0; i < this.sections.Length; i++)
		{
			this.sections[i] = this.sections[i].Trim(new char[]
			{
				'"'
			});
		}
		if (this.sections.Length > 0)
		{
			this.baseCommand = this.sections[0];
		}
		else
		{
			this.baseCommand = string.Empty;
		}
		if (this.sections.Length > 1)
		{
			this.arguments = LinqS.Skip(this.sections, 1);
		}
		else
		{
			this.arguments = null;
		}
	}
}
