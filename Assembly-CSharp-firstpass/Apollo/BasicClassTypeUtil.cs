using System;

namespace Apollo
{
	public class BasicClassTypeUtil
	{
		public static object CreateObject<T>()
		{
			return BasicClassTypeUtil.CreateObject(typeof(T));
		}

		public static object CreateObject(Type type)
		{
			object result;
			try
			{
				if (type.ToString() == "System.String")
				{
					result = string.Empty;
				}
				else
				{
					result = Activator.CreateInstance(type);
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
			return result;
		}

		public static object CreateListItem(Type typeList)
		{
			Type[] genericArguments = typeList.GetGenericArguments();
			if (genericArguments == null || genericArguments.Length == 0)
			{
				return null;
			}
			return BasicClassTypeUtil.CreateObject(genericArguments[0]);
		}
	}
}
