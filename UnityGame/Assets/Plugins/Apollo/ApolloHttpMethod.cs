using System;

namespace Apollo
{
	internal class ApolloHttpMethod
	{
		public const string GET = "GET";

		public const string POST = "POST";

		public const string HEAD = "HEAD";

		public static bool Valied(string method)
		{
			return method == "GET" || method == "POST" || method == "HEAD";
		}
	}
}
