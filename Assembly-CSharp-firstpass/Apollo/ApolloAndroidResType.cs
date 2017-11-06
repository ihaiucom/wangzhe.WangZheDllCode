using System;

namespace Apollo
{
	internal class ApolloAndroidResType
	{
		public const string ID = "id";

		public const string Layout = "layout";

		public const string Drawable = "drawable";

		public const string Menu = "menu";

		public const string Str = "string";

		public const string Style = "style";

		public static bool Valied(string typeName)
		{
			return typeName == "id" || typeName == "layout" || typeName == "drawable" || typeName == "menu" || typeName == "string" || typeName == "style";
		}
	}
}
