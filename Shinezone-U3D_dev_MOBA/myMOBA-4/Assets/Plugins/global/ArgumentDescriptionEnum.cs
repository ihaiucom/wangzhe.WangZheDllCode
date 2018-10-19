using System;
using System.Collections.Generic;

[Argument(100)]
public class ArgumentDescriptionEnum : IArgumentDescription
{
	public bool Accept(Type InType)
	{
		return InType != null && InType.IsEnum;
	}

	public string GetValue(Type InType, string InArgument)
	{
		DebugHelper.Assert(InArgument != null);
		string[] names = Enum.GetNames(InType);
		for (int i = 0; i < names.Length; i++)
		{
			if (names[i].Equals(InArgument, StringComparison.CurrentCultureIgnoreCase))
			{
				return names[i];
			}
		}
		string text;
		if (ArgumentDescriptionDefault.CheckConvertUtil(InArgument, typeof(int), out text))
		{
			int num = System.Convert.ToInt32(InArgument);
			return Enum.GetName(InType, num);
		}
		return string.Empty;
	}

	public bool CheckConvert(string InArgument, Type InType, out string OutErrorMessage)
	{
		DebugHelper.Assert(InArgument != null && InType.IsEnum);
		OutErrorMessage = string.Empty;
		string[] names = Enum.GetNames(InType);
		for (int i = 0; i < names.Length; i++)
		{
			if (names[i].Equals(InArgument, StringComparison.CurrentCultureIgnoreCase))
			{
				return true;
			}
		}
		string text;
		if (ArgumentDescriptionDefault.CheckConvertUtil(InArgument, typeof(int), out text))
		{
			int num = System.Convert.ToInt32(InArgument);
			string name = Enum.GetName(InType, num);
			if (string.IsNullOrEmpty(name))
			{
				OutErrorMessage = string.Format("不能将\"{0}\"转换到{1}的任何值.", InArgument, InType.Name);
			}
			return false;
		}
		OutErrorMessage = string.Format("不能将\"{0}\"转换为任何有效属性.", InArgument);
		return false;
	}

	public static int StringToEnum(Type InType, string InText)
	{
		string text;
		if (ArgumentDescriptionDefault.CheckConvertUtil(InText, typeof(int), out text))
		{
			return System.Convert.ToInt32(InText);
		}
		return System.Convert.ToInt32(Enum.Parse(InType, InText, true));
	}

	public List<string> GetCandinates(Type InType)
	{
		string[] names = Enum.GetNames(InType);
		return (names == null) ? null : LinqS.ToStringList(names);
	}

	public List<string> FilteredCandinates(Type InType, string InArgument)
	{
		string text;
		if (ArgumentDescriptionDefault.CheckConvertUtil(InArgument, typeof(int), out text))
		{
			int num = System.Convert.ToInt32(InArgument);
			string name = Enum.GetName(InType, num);
			return this.FilteredCandinatesInner(InType, name);
		}
		return this.FilteredCandinatesInner(InType, InArgument);
	}

	protected List<string> FilteredCandinatesInner(Type InType, string InArgument)
	{
		List<string> candinates = this.GetCandinates(InType);
		if (candinates != null && InArgument != null)
		{
			candinates.RemoveAll((string x) => !x.StartsWith(InArgument, StringComparison.CurrentCultureIgnoreCase));
		}
		return candinates;
	}

	public bool AcceptAsMethodParameter(Type InType)
	{
		return InType.IsEnum;
	}

	public object Convert(string InArgument, Type InType)
	{
		string text;
		if (ArgumentDescriptionDefault.CheckConvertUtil(InArgument, typeof(int), out text))
		{
			int value = System.Convert.ToInt32(InArgument);
			return Enum.ToObject(InType, value);
		}
		return Enum.Parse(InType, InArgument, true);
	}
}
