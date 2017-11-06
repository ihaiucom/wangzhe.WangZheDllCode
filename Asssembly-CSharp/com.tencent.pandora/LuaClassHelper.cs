using System;

namespace com.tencent.pandora
{
	public class LuaClassHelper
	{
		public static LuaFunction getTableFunction(LuaTable luaTable, string name)
		{
			object obj = luaTable.rawget(name);
			if (obj is LuaFunction)
			{
				return (LuaFunction)obj;
			}
			return null;
		}

		public static object callFunction(LuaFunction function, object[] args, Type[] returnTypes, object[] inArgs, int[] outArgs)
		{
			object[] array = function.call(inArgs, returnTypes);
			object result;
			int num;
			if (returnTypes[0] == typeof(void))
			{
				result = null;
				num = 0;
			}
			else
			{
				result = array[0];
				num = 1;
			}
			for (int i = 0; i < outArgs.Length; i++)
			{
				args[outArgs[i]] = array[num];
				num++;
			}
			return result;
		}
	}
}
