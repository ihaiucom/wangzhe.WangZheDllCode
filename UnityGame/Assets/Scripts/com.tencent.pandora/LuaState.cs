using System;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace com.tencent.pandora
{
	public class LuaState : IDisposable
	{
		public IntPtr L;

		internal LuaCSFunction tracebackFunction;

		internal ObjectTranslator translator;

		internal LuaCSFunction panicCallback;

		internal LuaCSFunction printFunction;

		internal LuaCSFunction loadfileFunction;

		internal LuaCSFunction loaderFunction;

		internal LuaCSFunction dofileFunction;

		internal LuaCSFunction import_wrapFunction;

		public object this[string fullPath]
		{
			get
			{
				int newTop = LuaDLL.lua_gettop(this.L);
				string[] array = fullPath.Split(new char[]
				{
					'.'
				});
				LuaDLL.lua_getglobal(this.L, array[0]);
				object @object = this.translator.getObject(this.L, -1);
				if (array.Length > 1)
				{
					string[] array2 = new string[array.Length - 1];
					Array.Copy(array, 1, array2, 0, array.Length - 1);
					@object = this.getObject(array2);
				}
				LuaDLL.lua_settop(this.L, newTop);
				return @object;
			}
			set
			{
				int newTop = LuaDLL.lua_gettop(this.L);
				string[] array = fullPath.Split(new char[]
				{
					'.'
				});
				if (array.Length == 1)
				{
					this.translator.push(this.L, value);
					LuaDLL.lua_setglobal(this.L, fullPath);
				}
				else
				{
					LuaDLL.lua_rawglobal(this.L, array[0]);
					if (LuaDLL.lua_type(this.L, -1) == LuaTypes.LUA_TNIL)
					{
						Debug.LogError("Table " + array[0] + " not exists");
						LuaDLL.lua_settop(this.L, newTop);
						return;
					}
					string[] array2 = new string[array.Length - 1];
					Array.Copy(array, 1, array2, 0, array.Length - 1);
					this.setObject(array2, value);
				}
				LuaDLL.lua_settop(this.L, newTop);
			}
		}

		public LuaState()
		{
			this.L = LuaDLL.luaL_newstate();
			LuaDLL.luaL_openlibs(this.L);
			LuaDLL.lua_pushstring(this.L, "LUAINTERFACE LOADED");
			LuaDLL.lua_pushboolean(this.L, true);
			LuaDLL.lua_settable(this.L, LuaIndexes.LUA_REGISTRYINDEX);
			LuaDLL.lua_newtable(this.L);
			LuaDLL.lua_setglobal(this.L, "luanet");
			LuaDLL.lua_pushvalue(this.L, LuaIndexes.LUA_GLOBALSINDEX);
			LuaDLL.lua_getglobal(this.L, "luanet");
			LuaDLL.lua_pushstring(this.L, "getmetatable");
			LuaDLL.lua_getglobal(this.L, "getmetatable");
			LuaDLL.lua_settable(this.L, -3);
			LuaDLL.lua_pushstring(this.L, "rawget");
			LuaDLL.lua_getglobal(this.L, "rawget");
			LuaDLL.lua_settable(this.L, -3);
			LuaDLL.lua_pushstring(this.L, "rawset");
			LuaDLL.lua_getglobal(this.L, "rawset");
			LuaDLL.lua_settable(this.L, -3);
			LuaDLL.lua_replace(this.L, LuaIndexes.LUA_GLOBALSINDEX);
			this.translator = new ObjectTranslator(this, this.L);
			LuaDLL.lua_replace(this.L, LuaIndexes.LUA_GLOBALSINDEX);
			this.translator.PushTranslator(this.L);
			this.panicCallback = new LuaCSFunction(LuaStatic.panic);
			LuaDLL.lua_atpanic(this.L, this.panicCallback);
			this.printFunction = new LuaCSFunction(LuaStatic.print);
			LuaDLL.lua_pushstdcallcfunction(this.L, this.printFunction, 0);
			LuaDLL.lua_setfield(this.L, LuaIndexes.LUA_GLOBALSINDEX, "print");
			this.loadfileFunction = new LuaCSFunction(LuaStatic.loadfile);
			LuaDLL.lua_pushstdcallcfunction(this.L, this.loadfileFunction, 0);
			LuaDLL.lua_setfield(this.L, LuaIndexes.LUA_GLOBALSINDEX, "loadfile");
			this.dofileFunction = new LuaCSFunction(LuaStatic.dofile);
			LuaDLL.lua_pushstdcallcfunction(this.L, this.dofileFunction, 0);
			LuaDLL.lua_setfield(this.L, LuaIndexes.LUA_GLOBALSINDEX, "dofile");
			this.import_wrapFunction = new LuaCSFunction(LuaStatic.importWrap);
			LuaDLL.lua_pushstdcallcfunction(this.L, this.import_wrapFunction, 0);
			LuaDLL.lua_setfield(this.L, LuaIndexes.LUA_GLOBALSINDEX, "import");
			this.loaderFunction = new LuaCSFunction(LuaStatic.loader);
			LuaDLL.lua_pushstdcallcfunction(this.L, this.loaderFunction, 0);
			int index = LuaDLL.lua_gettop(this.L);
			LuaDLL.lua_getfield(this.L, LuaIndexes.LUA_GLOBALSINDEX, "package");
			LuaDLL.lua_getfield(this.L, -1, "loaders");
			int num = LuaDLL.lua_gettop(this.L);
			for (int i = LuaDLL.luaL_getn(this.L, num) + 1; i > 1; i--)
			{
				LuaDLL.lua_rawgeti(this.L, num, i - 1);
				LuaDLL.lua_rawseti(this.L, num, i);
			}
			LuaDLL.lua_pushvalue(this.L, index);
			LuaDLL.lua_rawseti(this.L, num, 1);
			LuaDLL.lua_settop(this.L, 0);
			this.DoString(LuaStatic.init_luanet);
			this.tracebackFunction = new LuaCSFunction(LuaStatic.traceback);
		}

		public void Close()
		{
			if (this.L != IntPtr.Zero)
			{
				this.translator.Destroy();
				LuaDLL.lua_close(this.L);
			}
		}

		public ObjectTranslator GetTranslator()
		{
			return this.translator;
		}

		internal void ThrowExceptionFromError(int oldTop)
		{
			string text = LuaDLL.lua_tostring(this.L, -1);
			LuaDLL.lua_settop(this.L, oldTop);
			if (text == null)
			{
				text = "Unknown Lua Error";
			}
			throw new LuaScriptException(text, string.Empty);
		}

		internal int SetPendingException(Exception e)
		{
			if (e != null)
			{
				this.translator.throwError(this.L, e.ToString());
				LuaDLL.lua_pushnil(this.L);
				return 1;
			}
			return 0;
		}

		public LuaFunction LoadString(string chunk, string name, LuaTable env)
		{
			int oldTop = LuaDLL.lua_gettop(this.L);
			byte[] bytes = Encoding.get_UTF8().GetBytes(chunk);
			if (LuaDLL.luaL_loadbuffer(this.L, bytes, bytes.Length, name) != 0)
			{
				this.ThrowExceptionFromError(oldTop);
			}
			if (env != null)
			{
				env.push(this.L);
				LuaDLL.lua_setfenv(this.L, -2);
			}
			LuaFunction function = this.translator.getFunction(this.L, -1);
			this.translator.popValues(this.L, oldTop);
			return function;
		}

		public LuaFunction LoadString(string chunk, string name)
		{
			return this.LoadString(chunk, name, null);
		}

		public LuaFunction LoadFile(string fileName)
		{
			int oldTop = LuaDLL.lua_gettop(this.L);
			byte[] array = null;
			using (FileStream fileStream = new FileStream(fileName, 3))
			{
				BinaryReader binaryReader = new BinaryReader(fileStream);
				array = binaryReader.ReadBytes((int)fileStream.get_Length());
				fileStream.Close();
			}
			if (LuaDLL.luaL_loadbuffer(this.L, array, array.Length, fileName) != 0)
			{
				this.ThrowExceptionFromError(oldTop);
			}
			LuaFunction function = this.translator.getFunction(this.L, -1);
			this.translator.popValues(this.L, oldTop);
			return function;
		}

		public object[] DoString(string chunk)
		{
			return this.DoString(chunk, "chunk", null);
		}

		public object[] DoString(string chunk, string chunkName, LuaTable env)
		{
			int oldTop = LuaDLL.lua_gettop(this.L);
			byte[] bytes = Encoding.get_UTF8().GetBytes(chunk);
			if (LuaDLL.luaL_loadbuffer(this.L, bytes, bytes.Length, chunkName) == 0)
			{
				if (env != null)
				{
					env.push(this.L);
					LuaDLL.lua_setfenv(this.L, -2);
				}
				if (LuaDLL.lua_pcall(this.L, 0, -1, 0) == 0)
				{
					return this.translator.popValues(this.L, oldTop);
				}
				this.ThrowExceptionFromError(oldTop);
			}
			else
			{
				this.ThrowExceptionFromError(oldTop);
			}
			return null;
		}

		public object[] DoFile(string fileName)
		{
			return this.DoFile(fileName, null);
		}

		public object[] DoFile(string fileName, LuaTable env)
		{
			LuaDLL.lua_pushstdcallcfunction(this.L, this.tracebackFunction, 0);
			int oldTop = LuaDLL.lua_gettop(this.L);
			byte[] array = LuaStatic.Load(fileName);
			if (array == null)
			{
				if (!fileName.Contains("mobdebug"))
				{
					Debug.LogError("Loader lua file failed: " + fileName);
				}
				LuaDLL.lua_pop(this.L, 1);
				return null;
			}
			if (LuaDLL.luaL_loadbuffer(this.L, array, array.Length, fileName) == 0)
			{
				if (env != null)
				{
					env.push(this.L);
					LuaDLL.lua_setfenv(this.L, -2);
				}
				if (LuaDLL.lua_pcall(this.L, 0, -1, -2) == 0)
				{
					object[] result = this.translator.popValues(this.L, oldTop);
					LuaDLL.lua_pop(this.L, 1);
					return result;
				}
				this.ThrowExceptionFromError(oldTop);
				LuaDLL.lua_pop(this.L, 1);
			}
			else
			{
				this.ThrowExceptionFromError(oldTop);
				LuaDLL.lua_pop(this.L, 1);
			}
			return null;
		}

		internal object getObject(string[] remainingPath)
		{
			object obj = null;
			for (int i = 0; i < remainingPath.Length; i++)
			{
				LuaDLL.lua_pushstring(this.L, remainingPath[i]);
				LuaDLL.lua_gettable(this.L, -2);
				obj = this.translator.getObject(this.L, -1);
				if (obj == null)
				{
					break;
				}
			}
			return obj;
		}

		public double GetNumber(string fullPath)
		{
			return (double)this[fullPath];
		}

		public string GetString(string fullPath)
		{
			return (string)this[fullPath];
		}

		public LuaTable GetTable(string fullPath)
		{
			return (LuaTable)this[fullPath];
		}

		public LuaFunction GetFunction(string fullPath)
		{
			object obj = this[fullPath];
			return (obj is LuaCSFunction) ? new LuaFunction((LuaCSFunction)obj, this) : ((LuaFunction)obj);
		}

		public Delegate GetFunction(Type delegateType, string fullPath)
		{
			this.translator.throwError(this.L, "function delegates not implemnented");
			return null;
		}

		internal void setObject(string[] remainingPath, object val)
		{
			for (int i = 0; i < remainingPath.Length - 1; i++)
			{
				LuaDLL.lua_pushstring(this.L, remainingPath[i]);
				LuaDLL.lua_gettable(this.L, -2);
			}
			LuaDLL.lua_pushstring(this.L, remainingPath[remainingPath.Length - 1]);
			this.translator.push(this.L, val);
			LuaDLL.lua_settable(this.L, -3);
		}

		public void NewTable(string fullPath)
		{
			string[] array = fullPath.Split(new char[]
			{
				'.'
			});
			int newTop = LuaDLL.lua_gettop(this.L);
			if (array.Length == 1)
			{
				LuaDLL.lua_newtable(this.L);
				LuaDLL.lua_setglobal(this.L, fullPath);
			}
			else
			{
				LuaDLL.lua_getglobal(this.L, array[0]);
				for (int i = 1; i < array.Length - 1; i++)
				{
					LuaDLL.lua_pushstring(this.L, array[i]);
					LuaDLL.lua_gettable(this.L, -2);
				}
				LuaDLL.lua_pushstring(this.L, array[array.Length - 1]);
				LuaDLL.lua_newtable(this.L);
				LuaDLL.lua_settable(this.L, -3);
			}
			LuaDLL.lua_settop(this.L, newTop);
		}

		public LuaTable NewTable()
		{
			int newTop = LuaDLL.lua_gettop(this.L);
			LuaDLL.lua_newtable(this.L);
			LuaTable result = (LuaTable)this.translator.getObject(this.L, -1);
			LuaDLL.lua_settop(this.L, newTop);
			return result;
		}

		public ListDictionary GetTableDict(LuaTable table)
		{
			ListDictionary listDictionary = new ListDictionary();
			int newTop = LuaDLL.lua_gettop(this.L);
			this.translator.push(this.L, table);
			LuaDLL.lua_pushnil(this.L);
			while (LuaDLL.lua_next(this.L, -2) != 0)
			{
				listDictionary.set_Item(this.translator.getObject(this.L, -2), this.translator.getObject(this.L, -1));
				LuaDLL.lua_settop(this.L, -2);
			}
			LuaDLL.lua_settop(this.L, newTop);
			return listDictionary;
		}

		internal void dispose(int reference)
		{
			if (this.L != IntPtr.Zero)
			{
				LuaDLL.lua_unref(this.L, reference);
			}
		}

		internal object rawGetObject(int reference, string field)
		{
			int newTop = LuaDLL.lua_gettop(this.L);
			LuaDLL.lua_getref(this.L, reference);
			LuaDLL.lua_pushstring(this.L, field);
			LuaDLL.lua_rawget(this.L, -2);
			object @object = this.translator.getObject(this.L, -1);
			LuaDLL.lua_settop(this.L, newTop);
			return @object;
		}

		internal object getObject(int reference, string field)
		{
			int newTop = LuaDLL.lua_gettop(this.L);
			LuaDLL.lua_getref(this.L, reference);
			object @object = this.getObject(field.Split(new char[]
			{
				'.'
			}));
			LuaDLL.lua_settop(this.L, newTop);
			return @object;
		}

		internal object getObject(int reference, object field)
		{
			int newTop = LuaDLL.lua_gettop(this.L);
			LuaDLL.lua_getref(this.L, reference);
			this.translator.push(this.L, field);
			LuaDLL.lua_gettable(this.L, -2);
			object @object = this.translator.getObject(this.L, -1);
			LuaDLL.lua_settop(this.L, newTop);
			return @object;
		}

		internal void setObject(int reference, string field, object val)
		{
			int newTop = LuaDLL.lua_gettop(this.L);
			LuaDLL.lua_getref(this.L, reference);
			this.setObject(field.Split(new char[]
			{
				'.'
			}), val);
			LuaDLL.lua_settop(this.L, newTop);
		}

		internal void setObject(int reference, object field, object val)
		{
			int newTop = LuaDLL.lua_gettop(this.L);
			LuaDLL.lua_getref(this.L, reference);
			this.translator.push(this.L, field);
			this.translator.push(this.L, val);
			LuaDLL.lua_settable(this.L, -3);
			LuaDLL.lua_settop(this.L, newTop);
		}

		public LuaFunction RegisterFunction(string path, object target, MethodBase function)
		{
			int newTop = LuaDLL.lua_gettop(this.L);
			LuaMethodWrapper @object = new LuaMethodWrapper(this.translator, target, function.get_DeclaringType(), function);
			this.translator.push(this.L, new LuaCSFunction(@object.call));
			this[path] = this.translator.getObject(this.L, -1);
			LuaFunction function2 = this.GetFunction(path);
			LuaDLL.lua_settop(this.L, newTop);
			return function2;
		}

		public LuaFunction CreateFunction(object target, MethodBase function)
		{
			int newTop = LuaDLL.lua_gettop(this.L);
			LuaMethodWrapper @object = new LuaMethodWrapper(this.translator, target, function.get_DeclaringType(), function);
			this.translator.push(this.L, new LuaCSFunction(@object.call));
			object object2 = this.translator.getObject(this.L, -1);
			LuaFunction result = (object2 is LuaCSFunction) ? new LuaFunction((LuaCSFunction)object2, this) : ((LuaFunction)object2);
			LuaDLL.lua_settop(this.L, newTop);
			return result;
		}

		internal bool compareRef(int ref1, int ref2)
		{
			if (ref1 == ref2)
			{
				return true;
			}
			int newTop = LuaDLL.lua_gettop(this.L);
			LuaDLL.lua_getref(this.L, ref1);
			LuaDLL.lua_getref(this.L, ref2);
			int num = LuaDLL.lua_equal(this.L, -1, -2);
			LuaDLL.lua_settop(this.L, newTop);
			return num != 0;
		}

		internal void pushCSFunction(LuaCSFunction function)
		{
			this.translator.pushFunction(this.L, function);
		}

		public void Dispose()
		{
			this.Dispose(true);
			this.L = IntPtr.Zero;
			GC.SuppressFinalize(this);
			GC.Collect();
			GC.WaitForPendingFinalizers();
		}

		public virtual void Dispose(bool dispose)
		{
			if (dispose && this.translator != null)
			{
				this.translator.pendingEvents.Dispose();
				this.translator = null;
			}
		}
	}
}
