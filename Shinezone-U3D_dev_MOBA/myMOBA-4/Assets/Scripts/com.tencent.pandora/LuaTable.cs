using System;
using System.Collections;

namespace com.tencent.pandora
{
	public class LuaTable : LuaBase
	{
		public object this[string field]
		{
			get
			{
				return this._Interpreter.getObject(this._Reference, field);
			}
			set
			{
				this._Interpreter.setObject(this._Reference, field, value);
			}
		}

		public object this[object field]
		{
			get
			{
				return this._Interpreter.getObject(this._Reference, field);
			}
			set
			{
				this._Interpreter.setObject(this._Reference, field, value);
			}
		}

		public int Count
		{
			get
			{
				return this._Interpreter.GetTableDict(this).Count;
			}
		}

		public ICollection Keys
		{
			get
			{
				return this._Interpreter.GetTableDict(this).Keys;
			}
		}

		public ICollection Values
		{
			get
			{
				return this._Interpreter.GetTableDict(this).Values;
			}
		}

		public LuaTable(int reference, LuaState interpreter)
		{
			this._Reference = reference;
			this._Interpreter = interpreter;
			this.translator = interpreter.translator;
		}

		public LuaTable(int reference, IntPtr L)
		{
			this._Reference = reference;
			this.translator = ObjectTranslator.FromState(L);
			this._Interpreter = this.translator.interpreter;
		}

		public IDictionaryEnumerator GetEnumerator()
		{
			return this._Interpreter.GetTableDict(this).GetEnumerator();
		}

		public void SetMetaTable(LuaTable metaTable)
		{
			this.push(this._Interpreter.L);
			metaTable.push(this._Interpreter.L);
			LuaDLL.lua_setmetatable(this._Interpreter.L, -2);
			LuaDLL.lua_pop(this._Interpreter.L, 1);
		}

		public T[] ToArray<T>()
		{
			IntPtr l = this._Interpreter.L;
			this.push(l);
			return LuaScriptMgr.GetArrayObject<T>(l, -1);
		}

		public void Set(string key, object o)
		{
			IntPtr l = this._Interpreter.L;
			this.push(l);
			LuaDLL.lua_pushstring(l, key);
			base.PushArgs(l, o);
			LuaDLL.lua_rawset(l, -3);
			LuaDLL.lua_settop(l, 0);
		}

		internal object rawget(string field)
		{
			return this._Interpreter.rawGetObject(this._Reference, field);
		}

		internal object rawgetFunction(string field)
		{
			object obj = this._Interpreter.rawGetObject(this._Reference, field);
			if (obj is LuaCSFunction)
			{
				return new LuaFunction((LuaCSFunction)obj, this._Interpreter);
			}
			return obj;
		}

		public LuaFunction RawGetFunc(string field)
		{
			IntPtr l = this._Interpreter.L;
			LuaFunction result = null;
			int newTop = LuaDLL.lua_gettop(l);
			LuaDLL.lua_getref(l, this._Reference);
			LuaDLL.lua_pushstring(l, field);
			LuaDLL.lua_gettable(l, -2);
			LuaTypes luaTypes = LuaDLL.lua_type(l, -1);
			if (luaTypes == LuaTypes.LUA_TFUNCTION)
			{
				result = new LuaFunction(LuaDLL.luaL_ref(l, LuaIndexes.LUA_REGISTRYINDEX), l);
			}
			LuaDLL.lua_settop(l, newTop);
			return result;
		}

		internal void push(IntPtr luaState)
		{
			LuaDLL.lua_getref(luaState, this._Reference);
		}

		public override string ToString()
		{
			return "table";
		}
	}
}
