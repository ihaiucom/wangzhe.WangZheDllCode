using System;

namespace com.tencent.pandora
{
	public class LuaFunction : LuaBase
	{
		internal LuaCSFunction function;

		private IntPtr L;

		private int beginPos = -1;

		public LuaFunction(int reference, LuaState interpreter)
		{
			this._Reference = reference;
			this.function = null;
			this._Interpreter = interpreter;
			this.L = this._Interpreter.L;
			this.translator = this._Interpreter.translator;
		}

		public LuaFunction(LuaCSFunction function, LuaState interpreter)
		{
			this._Reference = 0;
			this.function = function;
			this._Interpreter = interpreter;
			this.L = this._Interpreter.L;
			this.translator = this._Interpreter.translator;
		}

		public LuaFunction(int reference, IntPtr l)
		{
			this._Reference = reference;
			this.function = null;
			this.L = l;
			this.translator = ObjectTranslator.FromState(this.L);
			this._Interpreter = this.translator.interpreter;
		}

		internal object[] call(object[] args, Type[] returnTypes)
		{
			int num = 0;
			LuaScriptMgr.PushTraceBack(this.L);
			int num2 = LuaDLL.lua_gettop(this.L);
			if (!LuaDLL.lua_checkstack(this.L, args.Length + 6))
			{
				LuaDLL.lua_pop(this.L, 1);
				throw new LuaException("Lua stack overflow");
			}
			this.push(this.L);
			if (args != null)
			{
				num = args.Length;
				for (int i = 0; i < args.Length; i++)
				{
					base.PushArgs(this.L, args[i]);
				}
			}
			int num3 = LuaDLL.lua_pcall(this.L, num, -1, -num - 2);
			if (num3 != 0)
			{
				string text = LuaDLL.lua_tostring(this.L, -1);
				LuaDLL.lua_settop(this.L, num2 - 1);
				if (text == null)
				{
					text = "Unknown Lua Error";
				}
				throw new LuaScriptException(text, string.Empty);
			}
			object[] result = (returnTypes != null) ? this.translator.popValues(this.L, num2, returnTypes) : this.translator.popValues(this.L, num2);
			LuaDLL.lua_settop(this.L, num2 - 1);
			return result;
		}

		public object[] Call(params object[] args)
		{
			return this.call(args, null);
		}

		public object[] Call()
		{
			int num = this.BeginPCall();
			if (this.PCall(num, 0))
			{
				object[] result = this.PopValues(num);
				this.EndPCall(num);
				return result;
			}
			LuaDLL.lua_settop(this.L, num - 1);
			return null;
		}

		public object[] Call(double arg1)
		{
			int num = this.BeginPCall();
			LuaScriptMgr.Push(this.L, arg1);
			if (this.PCall(num, 1))
			{
				object[] result = this.PopValues(num);
				this.EndPCall(num);
				return result;
			}
			LuaDLL.lua_settop(this.L, num - 1);
			return null;
		}

		public int BeginPCall()
		{
			LuaScriptMgr.PushTraceBack(this.L);
			this.beginPos = LuaDLL.lua_gettop(this.L);
			this.push(this.L);
			return this.beginPos;
		}

		public bool PCall(int oldTop, int args)
		{
			if (LuaDLL.lua_pcall(this.L, args, -1, -args - 2) != 0)
			{
				string text = LuaDLL.lua_tostring(this.L, -1);
				LuaDLL.lua_settop(this.L, oldTop - 1);
				if (text == null)
				{
					text = "Unknown Lua Error";
				}
				throw new LuaScriptException(text, string.Empty);
			}
			return true;
		}

		public object[] PopValues(int oldTop)
		{
			return this.translator.popValues(this.L, oldTop);
		}

		public void EndPCall(int oldTop)
		{
			LuaDLL.lua_settop(this.L, oldTop - 1);
		}

		public IntPtr GetLuaState()
		{
			return this.L;
		}

		internal void push(IntPtr luaState)
		{
			if (this._Reference != 0)
			{
				LuaDLL.lua_getref(luaState, this._Reference);
			}
			else
			{
				this._Interpreter.pushCSFunction(this.function);
			}
		}

		internal void push()
		{
			if (this._Reference != 0)
			{
				LuaDLL.lua_getref(this.L, this._Reference);
			}
			else
			{
				this._Interpreter.pushCSFunction(this.function);
			}
		}

		public override string ToString()
		{
			return "function";
		}

		public override bool Equals(object o)
		{
			if (!(o is LuaFunction))
			{
				return false;
			}
			LuaFunction luaFunction = (LuaFunction)o;
			if (this._Reference != 0 && luaFunction._Reference != 0)
			{
				return this._Interpreter.compareRef(luaFunction._Reference, this._Reference);
			}
			return this.function == luaFunction.function;
		}

		public override int GetHashCode()
		{
			if (this._Reference != 0)
			{
				return this._Reference;
			}
			return this.function.GetHashCode();
		}

		public int GetReference()
		{
			return this._Reference;
		}
	}
}
