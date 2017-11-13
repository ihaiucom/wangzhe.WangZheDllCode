using System;
using System.Collections.Generic;

public interface IArgumentDescription
{
	bool Accept(Type InType);

	bool CheckConvert(string InArgument, Type InType, out string OutErrorMessage);

	string GetValue(Type InType, string InArgument);

	List<string> GetCandinates(Type InType);

	List<string> FilteredCandinates(Type InType, string InArgument);

	bool AcceptAsMethodParameter(Type InType);

	object Convert(string InArgument, Type InType);
}
