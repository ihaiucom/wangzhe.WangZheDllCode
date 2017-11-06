using System;

namespace Apollo
{
	internal class ApolloHttpVersion
	{
		public const string HTTP_V10 = "HTTP/1.0";

		public const string HTTP_V11 = "HTTP/1.1";

		public const string HTTP_V20 = "HTTP/2.0";

		public static bool Valied(string version)
		{
			return version == "HTTP/1.0" || version == "HTTP/1.1" || version == "HTTP/2.0";
		}
	}
}
