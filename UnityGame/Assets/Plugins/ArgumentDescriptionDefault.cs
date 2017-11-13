using System;
using System.Collections.Generic;

[Argument(-1)]
public class ArgumentDescriptionDefault : IArgumentDescription
{
	public bool Accept(Type InType)
	{
		return true;
	}

	public string GetValue(Type InType, string InArgument)
	{
		DebugHelper.Assert(InArgument != null);
		return InArgument;
	}

	public bool CheckConvert(string InArgument, Type InType, out string OutErrorMessage)
	{
		return ArgumentDescriptionDefault.CheckConvertUtil(InArgument, InType, out OutErrorMessage);
	}

	public static bool CheckConvertUtil(string InArgument, Type InType, out string OutErrorMessage)
	{
		bool result;
		try
		{
			System.Convert.ChangeType(InArgument, InType);
			OutErrorMessage = string.Empty;
			result = true;
		}
		catch (Exception ex)
		{
			OutErrorMessage = ex.get_Message();
			result = false;
		}
		return result;
	}

	public object Convert(string InArgument, Type InType)
	{
		object result;
		try
		{
			result = System.Convert.ChangeType(InArgument, InType);
		}
		catch (Exception)
		{
			result = null;
		}
		return result;
	}

	public List<string> GetCandinates(Type InType)
	{
		return null;
	}

	public List<string> FilteredCandinates(Type InType, string InArgument)
	{
		return null;
	}

	public bool AcceptAsMethodParameter(Type InType)
	{
		return InType == typeof(string) || InType.get_IsValueType();
	}
}
