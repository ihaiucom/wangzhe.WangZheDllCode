using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace com.tencent.pandora
{
	internal class CheckType
	{
		private ObjectTranslator translator;

		private ExtractValue extractNetObject;

		private Dictionary<long, ExtractValue> extractValues = new Dictionary<long, ExtractValue>();

		public CheckType(ObjectTranslator translator)
		{
			this.translator = translator;
			this.extractValues.Add(typeof(object).TypeHandle.Value.ToInt64(), new ExtractValue(this.getAsObject));
			this.extractValues.Add(typeof(sbyte).TypeHandle.Value.ToInt64(), new ExtractValue(this.getAsSbyte));
			this.extractValues.Add(typeof(byte).TypeHandle.Value.ToInt64(), new ExtractValue(this.getAsByte));
			this.extractValues.Add(typeof(short).TypeHandle.Value.ToInt64(), new ExtractValue(this.getAsShort));
			this.extractValues.Add(typeof(ushort).TypeHandle.Value.ToInt64(), new ExtractValue(this.getAsUshort));
			this.extractValues.Add(typeof(int).TypeHandle.Value.ToInt64(), new ExtractValue(this.getAsInt));
			this.extractValues.Add(typeof(uint).TypeHandle.Value.ToInt64(), new ExtractValue(this.getAsUint));
			this.extractValues.Add(typeof(long).TypeHandle.Value.ToInt64(), new ExtractValue(this.getAsLong));
			this.extractValues.Add(typeof(ulong).TypeHandle.Value.ToInt64(), new ExtractValue(this.getAsUlong));
			this.extractValues.Add(typeof(double).TypeHandle.Value.ToInt64(), new ExtractValue(this.getAsDouble));
			this.extractValues.Add(typeof(char).TypeHandle.Value.ToInt64(), new ExtractValue(this.getAsChar));
			this.extractValues.Add(typeof(float).TypeHandle.Value.ToInt64(), new ExtractValue(this.getAsFloat));
			this.extractValues.Add(typeof(decimal).TypeHandle.Value.ToInt64(), new ExtractValue(this.getAsDecimal));
			this.extractValues.Add(typeof(bool).TypeHandle.Value.ToInt64(), new ExtractValue(this.getAsBoolean));
			this.extractValues.Add(typeof(string).TypeHandle.Value.ToInt64(), new ExtractValue(this.getAsString));
			this.extractValues.Add(typeof(LuaFunction).TypeHandle.Value.ToInt64(), new ExtractValue(this.getAsFunction));
			this.extractValues.Add(typeof(LuaTable).TypeHandle.Value.ToInt64(), new ExtractValue(this.getAsTable));
			this.extractValues.Add(typeof(Vector3).TypeHandle.Value.ToInt64(), new ExtractValue(this.getAsVector3));
			this.extractValues.Add(typeof(Vector2).TypeHandle.Value.ToInt64(), new ExtractValue(this.getAsVector2));
			this.extractValues.Add(typeof(Quaternion).TypeHandle.Value.ToInt64(), new ExtractValue(this.getAsQuaternion));
			this.extractValues.Add(typeof(Color).TypeHandle.Value.ToInt64(), new ExtractValue(this.getAsColor));
			this.extractValues.Add(typeof(Vector4).TypeHandle.Value.ToInt64(), new ExtractValue(this.getAsVector4));
			this.extractValues.Add(typeof(Ray).TypeHandle.Value.ToInt64(), new ExtractValue(this.getAsRay));
			this.extractValues.Add(typeof(Bounds).TypeHandle.Value.ToInt64(), new ExtractValue(this.getAsBounds));
			this.extractNetObject = new ExtractValue(this.getAsNetObject);
		}

		internal ExtractValue getExtractor(IReflect paramType)
		{
			return this.getExtractor(paramType.UnderlyingSystemType);
		}

		internal ExtractValue getExtractor(Type paramType)
		{
			if (paramType.IsByRef)
			{
				paramType = paramType.GetElementType();
			}
			long key = paramType.TypeHandle.Value.ToInt64();
			if (this.extractValues.ContainsKey(key))
			{
				return this.extractValues[key];
			}
			return this.extractNetObject;
		}

		internal ExtractValue checkType(IntPtr luaState, int stackPos, Type paramType)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(luaState, stackPos);
			if (paramType.IsByRef)
			{
				paramType = paramType.GetElementType();
			}
			Type underlyingType = Nullable.GetUnderlyingType(paramType);
			if (underlyingType != null)
			{
				paramType = underlyingType;
			}
			long key = paramType.TypeHandle.Value.ToInt64();
			if (paramType.Equals(typeof(object)))
			{
				return this.extractValues[key];
			}
			if (paramType.IsGenericParameter)
			{
				if (luaTypes == LuaTypes.LUA_TBOOLEAN)
				{
					return this.extractValues[typeof(bool).TypeHandle.Value.ToInt64()];
				}
				if (luaTypes == LuaTypes.LUA_TSTRING)
				{
					return this.extractValues[typeof(string).TypeHandle.Value.ToInt64()];
				}
				if (luaTypes == LuaTypes.LUA_TTABLE)
				{
					return this.extractValues[typeof(LuaTable).TypeHandle.Value.ToInt64()];
				}
				if (luaTypes == LuaTypes.LUA_TUSERDATA)
				{
					return this.extractValues[typeof(object).TypeHandle.Value.ToInt64()];
				}
				if (luaTypes == LuaTypes.LUA_TFUNCTION)
				{
					return this.extractValues[typeof(LuaFunction).TypeHandle.Value.ToInt64()];
				}
				if (luaTypes == LuaTypes.LUA_TNUMBER)
				{
					return this.extractValues[typeof(double).TypeHandle.Value.ToInt64()];
				}
			}
			if (paramType.IsValueType && luaTypes == LuaTypes.LUA_TTABLE)
			{
				int newTop = LuaDLL.lua_gettop(luaState);
				ExtractValue extractValue = null;
				LuaDLL.lua_pushvalue(luaState, stackPos);
				LuaDLL.lua_pushstring(luaState, "class");
				LuaDLL.lua_gettable(luaState, -2);
				if (!LuaDLL.lua_isnil(luaState, -1))
				{
					string a = LuaDLL.lua_tostring(luaState, -1);
					if (a == "Vector3" && paramType == typeof(Vector3))
					{
						extractValue = this.extractValues[typeof(Vector3).TypeHandle.Value.ToInt64()];
					}
					else if (a == "Vector2" && paramType == typeof(Vector2))
					{
						extractValue = this.extractValues[typeof(Vector2).TypeHandle.Value.ToInt64()];
					}
					else if (a == "Quaternion" && paramType == typeof(Quaternion))
					{
						extractValue = this.extractValues[typeof(Quaternion).TypeHandle.Value.ToInt64()];
					}
					else if (a == "Color" && paramType == typeof(Color))
					{
						extractValue = this.extractValues[typeof(Color).TypeHandle.Value.ToInt64()];
					}
					else if (a == "Vector4" && paramType == typeof(Vector4))
					{
						extractValue = this.extractValues[typeof(Vector4).TypeHandle.Value.ToInt64()];
					}
					else if (a == "Ray" && paramType == typeof(Ray))
					{
						extractValue = this.extractValues[typeof(Ray).TypeHandle.Value.ToInt64()];
					}
					else
					{
						extractValue = null;
					}
				}
				LuaDLL.lua_settop(luaState, newTop);
				if (extractValue != null)
				{
					return extractValue;
				}
			}
			if (LuaDLL.lua_isnumber(luaState, stackPos))
			{
				return this.extractValues[key];
			}
			if (paramType == typeof(bool))
			{
				if (LuaDLL.lua_isboolean(luaState, stackPos))
				{
					return this.extractValues[key];
				}
			}
			else if (paramType == typeof(string))
			{
				if (LuaDLL.lua_isstring(luaState, stackPos))
				{
					return this.extractValues[key];
				}
				if (luaTypes == LuaTypes.LUA_TNIL)
				{
					return this.extractNetObject;
				}
			}
			else if (paramType == typeof(LuaTable))
			{
				if (luaTypes == LuaTypes.LUA_TTABLE)
				{
					return this.extractValues[key];
				}
			}
			else if (paramType == typeof(LuaFunction))
			{
				if (luaTypes == LuaTypes.LUA_TFUNCTION)
				{
					return this.extractValues[key];
				}
			}
			else if (typeof(Delegate).IsAssignableFrom(paramType) && luaTypes == LuaTypes.LUA_TFUNCTION)
			{
				this.translator.throwError(luaState, "Delegates not implemnented");
			}
			else if (paramType.IsInterface && luaTypes == LuaTypes.LUA_TTABLE)
			{
				this.translator.throwError(luaState, "Interfaces not implemnented");
			}
			else
			{
				if ((paramType.IsInterface || paramType.IsClass) && luaTypes == LuaTypes.LUA_TNIL)
				{
					return this.extractNetObject;
				}
				if (LuaDLL.lua_type(luaState, stackPos) == LuaTypes.LUA_TTABLE)
				{
					if (LuaDLL.luaL_getmetafield(luaState, stackPos, "__index") != LuaTypes.LUA_TNIL)
					{
						object netObject = this.translator.getNetObject(luaState, -1);
						LuaDLL.lua_settop(luaState, -2);
						if (netObject != null && paramType.IsAssignableFrom(netObject.GetType()))
						{
							return this.extractNetObject;
						}
					}
				}
				else
				{
					object rawNetObject = this.translator.getRawNetObject(luaState, stackPos);
					if (rawNetObject != null && paramType.IsAssignableFrom(rawNetObject.GetType()))
					{
						return this.extractNetObject;
					}
				}
			}
			return null;
		}

		private object getAsSbyte(IntPtr luaState, int stackPos)
		{
			sbyte b = (sbyte)LuaDLL.lua_tonumber(luaState, stackPos);
			if ((int)b == 0 && !LuaDLL.lua_isnumber(luaState, stackPos))
			{
				return null;
			}
			return b;
		}

		private object getAsByte(IntPtr luaState, int stackPos)
		{
			byte b = (byte)LuaDLL.lua_tonumber(luaState, stackPos);
			if (b == 0 && !LuaDLL.lua_isnumber(luaState, stackPos))
			{
				return null;
			}
			return b;
		}

		private object getAsShort(IntPtr luaState, int stackPos)
		{
			short num = (short)LuaDLL.lua_tonumber(luaState, stackPos);
			if (num == 0 && !LuaDLL.lua_isnumber(luaState, stackPos))
			{
				return null;
			}
			return num;
		}

		private object getAsUshort(IntPtr luaState, int stackPos)
		{
			ushort num = (ushort)LuaDLL.lua_tonumber(luaState, stackPos);
			if (num == 0 && !LuaDLL.lua_isnumber(luaState, stackPos))
			{
				return null;
			}
			return num;
		}

		private object getAsInt(IntPtr luaState, int stackPos)
		{
			int num = (int)LuaDLL.lua_tonumber(luaState, stackPos);
			if (num == 0 && !LuaDLL.lua_isnumber(luaState, stackPos))
			{
				return null;
			}
			return num;
		}

		private object getAsUint(IntPtr luaState, int stackPos)
		{
			uint num = (uint)LuaDLL.lua_tonumber(luaState, stackPos);
			if (num == 0u && !LuaDLL.lua_isnumber(luaState, stackPos))
			{
				return null;
			}
			return num;
		}

		private object getAsLong(IntPtr luaState, int stackPos)
		{
			long num = (long)LuaDLL.lua_tonumber(luaState, stackPos);
			if (num == 0L && !LuaDLL.lua_isnumber(luaState, stackPos))
			{
				return null;
			}
			return num;
		}

		private object getAsUlong(IntPtr luaState, int stackPos)
		{
			ulong num = (ulong)LuaDLL.lua_tonumber(luaState, stackPos);
			if (num == 0uL && !LuaDLL.lua_isnumber(luaState, stackPos))
			{
				return null;
			}
			return num;
		}

		private object getAsDouble(IntPtr luaState, int stackPos)
		{
			double num = LuaDLL.lua_tonumber(luaState, stackPos);
			if (num == 0.0 && !LuaDLL.lua_isnumber(luaState, stackPos))
			{
				return null;
			}
			return num;
		}

		private object getAsChar(IntPtr luaState, int stackPos)
		{
			char c = (char)LuaDLL.lua_tonumber(luaState, stackPos);
			if (c == '\0' && !LuaDLL.lua_isnumber(luaState, stackPos))
			{
				return null;
			}
			return c;
		}

		private object getAsFloat(IntPtr luaState, int stackPos)
		{
			float num = (float)LuaDLL.lua_tonumber(luaState, stackPos);
			if (num == 0f && !LuaDLL.lua_isnumber(luaState, stackPos))
			{
				return null;
			}
			return num;
		}

		private object getAsDecimal(IntPtr luaState, int stackPos)
		{
			decimal num = (decimal)LuaDLL.lua_tonumber(luaState, stackPos);
			if (num == 0m && !LuaDLL.lua_isnumber(luaState, stackPos))
			{
				return null;
			}
			return num;
		}

		private object getAsBoolean(IntPtr luaState, int stackPos)
		{
			return LuaDLL.lua_toboolean(luaState, stackPos);
		}

		private object getAsString(IntPtr luaState, int stackPos)
		{
			string text = LuaDLL.lua_tostring(luaState, stackPos);
			if (text == string.Empty && !LuaDLL.lua_isstring(luaState, stackPos))
			{
				return null;
			}
			return text;
		}

		private object getAsTable(IntPtr luaState, int stackPos)
		{
			return this.translator.getTable(luaState, stackPos);
		}

		private object getAsFunction(IntPtr luaState, int stackPos)
		{
			return this.translator.getFunction(luaState, stackPos);
		}

		public object getAsObject(IntPtr luaState, int stackPos)
		{
			if (LuaDLL.lua_type(luaState, stackPos) == LuaTypes.LUA_TTABLE && LuaDLL.luaL_getmetafield(luaState, stackPos, "__index") != LuaTypes.LUA_TNIL)
			{
				if (LuaDLL.luaL_checkmetatable(luaState, -1))
				{
					LuaDLL.lua_insert(luaState, stackPos);
					LuaDLL.lua_remove(luaState, stackPos + 1);
				}
				else
				{
					LuaDLL.lua_settop(luaState, -2);
				}
			}
			return this.translator.getObject(luaState, stackPos);
		}

		public object getAsNetObject(IntPtr luaState, int stackPos)
		{
			object obj = this.translator.getRawNetObject(luaState, stackPos);
			if (obj == null && LuaDLL.lua_type(luaState, stackPos) == LuaTypes.LUA_TTABLE && LuaDLL.luaL_getmetafield(luaState, stackPos, "__index") != LuaTypes.LUA_TNIL)
			{
				if (LuaDLL.luaL_checkmetatable(luaState, -1))
				{
					LuaDLL.lua_insert(luaState, stackPos);
					LuaDLL.lua_remove(luaState, stackPos + 1);
					obj = this.translator.getNetObject(luaState, stackPos);
				}
				else
				{
					LuaDLL.lua_settop(luaState, -2);
				}
			}
			return obj;
		}

		public object getAsVector3(IntPtr L, int stackPos)
		{
			return LuaScriptMgr.GetVector3(L, stackPos);
		}

		public object getAsVector2(IntPtr L, int stackPos)
		{
			return LuaScriptMgr.GetVector2(L, stackPos);
		}

		public object getAsQuaternion(IntPtr L, int stackPos)
		{
			return LuaScriptMgr.GetQuaternion(L, stackPos);
		}

		public object getAsColor(IntPtr L, int stackPos)
		{
			return LuaScriptMgr.GetColor(L, stackPos);
		}

		public object getAsVector4(IntPtr L, int stackPos)
		{
			return LuaScriptMgr.GetVector4(L, stackPos);
		}

		public object getAsRay(IntPtr L, int stackPos)
		{
			return LuaScriptMgr.GetRay(L, stackPos);
		}

		public object getAsBounds(IntPtr L, int stackPos)
		{
			return LuaScriptMgr.GetBounds(L, stackPos);
		}
	}
}
