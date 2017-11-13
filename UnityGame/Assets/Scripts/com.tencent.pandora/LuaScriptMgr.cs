using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace com.tencent.pandora
{
	public class LuaScriptMgr
	{
		public delegate object[] FileExecutor(string name);

		public LuaState lua;

		public LuaScriptMgr.FileExecutor DoFile;

		public HashSet<string> fileList;

		private Dictionary<string, LuaBase> dict;

		private LuaFunction updateFunc;

		private LuaFunction lateUpdateFunc;

		private LuaFunction fixedUpdateFunc;

		private LuaFunction levelLoaded;

		private int unpackVec3;

		private int unpackVec2;

		private int unpackVec4;

		private int unpackQuat;

		private int unpackColor;

		private int unpackRay;

		private int unpackBounds;

		private int packVec3;

		private int packVec2;

		private int packVec4;

		private int packQuat;

		private LuaFunction packTouch;

		private int packRay;

		private LuaFunction packRaycastHit;

		private int packColor;

		private int packBounds;

		private int enumMetaRef;

		private int typeMetaRef;

		private int delegateMetaRef;

		private int iterMetaRef;

		private int arrayMetaRef;

		public static LockFreeQueue<LuaRef> refGCList = new LockFreeQueue<LuaRef>(1024);

		private static ObjectTranslator _translator = null;

		private static HashSet<Type> checkBaseType = new HashSet<Type>();

		private static LuaFunction traceback = null;

		private string luaIndex = "        \n        local rawget = rawget\n        local rawset = rawset\n        local getmetatable = getmetatable      \n        local type = type  \n        local function index(obj,name)  \n            local o = obj            \n            local meta = getmetatable(o)            \n            local parent = meta\n            local v = nil\n            \n            while meta~= nil do\n                v = rawget(meta, name)\n                \n                if v~= nil then\n                    if parent ~= meta then rawset(parent, name, v) end\n\n                    local t = type(v)\n\n                    if t == 'function' then                    \n                        return v\n                    else\n                        local func = v[1]\n                \n                        if func ~= nil then\n                            return func(obj)                         \n                        end\n                    end\n                    break\n                end\n                \n                meta = getmetatable(meta)\n            end\n\n           error('unknown member name '..name, 2)\n           return nil\t        \n        end\n        return index";

		private string luaNewIndex = "\n        local rawget = rawget\n        local getmetatable = getmetatable   \n        local rawset = rawset     \n        local function newindex(obj, name, val)            \n            local meta = getmetatable(obj)            \n            local parent = meta\n            local v = nil\n            \n            while meta~= nil do\n                v = rawget(meta, name)\n                \n                if v~= nil then\n                    if parent ~= meta then rawset(parent, name, v) end\n                    local func = v[2]\n                    if func ~= nil then                        \n                        return func(obj, nil, val)                        \n                    end\n                    break\n                end\n                \n                meta = getmetatable(meta)\n            end  \n       \n            error('field or property '..name..' does not exist', 2)\n            return nil\t\t\n        end\n        return newindex";

		private string luaTableCall = "\n        local rawget = rawget\n        local getmetatable = getmetatable     \n\n        local function call(obj, ...)\n            local meta = getmetatable(obj)\n            local fun = rawget(meta, 'New')\n            \n            if fun ~= nil then\n                return fun(...)\n            else\n                error('unknow function __call',2)\n            end\n        end\n\n        return call\n    ";

		private string luaEnumIndex = "\n        local rawget = rawget                \n        local getmetatable = getmetatable         \n\n        local function indexEnum(obj,name)\n            local v = rawget(obj, name)\n            \n            if v ~= nil then\n                return v\n            end\n\n            local meta = getmetatable(obj)  \n            local func = rawget(meta, name)            \n            \n            if func ~= nil then\n                v = func()\n                rawset(obj, name, v)\n                return v\n            else\n                error('field '..name..' does not exist', 2)\n            end\n        end\n\n        return indexEnum\n    ";

		private static Type monoType = typeof(Type).GetType();

		private static Dictionary<Enum, object> enumMap = new Dictionary<Enum, object>();

		public static LuaScriptMgr Instance
		{
			get;
			private set;
		}

		public LuaScriptMgr()
		{
			Debug.Log("LuaScriptMgr ctor");
			LuaScriptMgr.Instance = this;
			this.lua = new LuaState();
			LuaScriptMgr._translator = this.lua.GetTranslator();
			this.DoFile = new LuaScriptMgr.FileExecutor(this.DefaultFileExecutor);
			LuaDLL.luaopen_cjson(this.lua.L);
			LuaDLL.luaopen_cjson_safe(this.lua.L);
			if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXEditor)
			{
				LuaDLL.luaopen_bit(this.lua.L);
			}
			LuaDLL.tolua_openlibs(this.lua.L);
			this.fileList = new HashSet<string>();
			this.dict = new Dictionary<string, LuaBase>();
			LuaDLL.lua_pushstring(this.lua.L, "ToLua_Index");
			LuaDLL.luaL_dostring(this.lua.L, this.luaIndex);
			LuaDLL.lua_rawset(this.lua.L, LuaIndexes.LUA_REGISTRYINDEX);
			LuaDLL.lua_pushstring(this.lua.L, "ToLua_NewIndex");
			LuaDLL.luaL_dostring(this.lua.L, this.luaNewIndex);
			LuaDLL.lua_rawset(this.lua.L, LuaIndexes.LUA_REGISTRYINDEX);
			LuaDLL.lua_pushstring(this.lua.L, "ToLua_TableCall");
			LuaDLL.luaL_dostring(this.lua.L, this.luaTableCall);
			LuaDLL.lua_rawset(this.lua.L, LuaIndexes.LUA_REGISTRYINDEX);
			LuaDLL.lua_pushstring(this.lua.L, "ToLua_EnumIndex");
			LuaDLL.luaL_dostring(this.lua.L, this.luaEnumIndex);
			LuaDLL.lua_rawset(this.lua.L, LuaIndexes.LUA_REGISTRYINDEX);
			this.Bind();
			LuaDLL.lua_pushnumber(this.lua.L, 0.0);
			LuaDLL.lua_setglobal(this.lua.L, "_LuaScriptMgr");
		}

		private void SendGMmsg(params string[] param)
		{
			Debug.Log("SendGMmsg");
			string text = string.Empty;
			int num = 0;
			for (int i = 0; i < param.Length; i++)
			{
				string text2 = param[i];
				if (num > 0)
				{
					text = text + " " + text2;
					Debug.Log(text2);
				}
				num++;
			}
			this.CallLuaFunction("GMMsg", new object[]
			{
				text
			});
		}

		private void InitLayers(IntPtr L)
		{
			LuaTable luaTable = this.GetLuaTable("Layer");
			luaTable.push(L);
			for (int i = 0; i < 32; i++)
			{
				string text = LayerMask.LayerToName(i);
				if (text != string.Empty)
				{
					LuaDLL.lua_pushstring(L, text);
					LuaScriptMgr.Push(L, i);
					LuaDLL.lua_rawset(L, -3);
				}
			}
			LuaDLL.lua_settop(L, 0);
		}

		private void Bind()
		{
			IntPtr l = this.lua.L;
			this.BindArray(l);
			DelegateFactory.Register(l);
			LuaBinder.Bind(l, null);
		}

		private void BindArray(IntPtr L)
		{
			LuaDLL.luaL_newmetatable(L, "luaNet_array");
			LuaDLL.lua_pushstring(L, "__index");
			LuaDLL.lua_pushstdcallcfunction(L, new LuaCSFunction(LuaScriptMgr.IndexArray), 0);
			LuaDLL.lua_rawset(L, -3);
			LuaDLL.lua_pushstring(L, "__gc");
			LuaDLL.lua_pushstdcallcfunction(L, new LuaCSFunction(LuaScriptMgr.__gc), 0);
			LuaDLL.lua_rawset(L, -3);
			LuaDLL.lua_pushstring(L, "__newindex");
			LuaDLL.lua_pushstdcallcfunction(L, new LuaCSFunction(LuaScriptMgr.NewIndexArray), 0);
			LuaDLL.lua_rawset(L, -3);
			this.arrayMetaRef = LuaDLL.luaL_ref(this.lua.L, LuaIndexes.LUA_REGISTRYINDEX);
			LuaDLL.lua_settop(L, 0);
		}

		public IntPtr GetL()
		{
			return this.lua.L;
		}

		private void PrintLua(params string[] param)
		{
			if (param.Length != 2)
			{
				Debug.Log("PrintLua [ModuleName]");
				return;
			}
			this.CallLuaFunction("PrintLua", new object[]
			{
				param[1]
			});
		}

		public void LuaGC(params string[] param)
		{
			LuaDLL.lua_gc(this.lua.L, LuaGCOptions.LUA_GCCOLLECT, 0);
		}

		private void LuaMem(params string[] param)
		{
			this.CallLuaFunction("mem_report", new object[0]);
		}

		public void Start()
		{
			this.OnBundleLoaded();
			this.enumMetaRef = this.GetTypeMetaRef(typeof(Enum));
			this.typeMetaRef = this.GetTypeMetaRef(typeof(Type));
			this.delegateMetaRef = this.GetTypeMetaRef(typeof(Delegate));
			this.iterMetaRef = this.GetTypeMetaRef(typeof(IEnumerator));
			using (HashSet<Type>.Enumerator enumerator = LuaScriptMgr.checkBaseType.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Type current = enumerator.get_Current();
					Debug.LogWarning("BaseType " + current.get_FullName() + " not register to lua");
				}
			}
			LuaScriptMgr.checkBaseType.Clear();
		}

		private int GetLuaReference(string str)
		{
			LuaFunction luaFunction = this.GetLuaFunction(str);
			if (luaFunction != null)
			{
				return luaFunction.GetReference();
			}
			return -1;
		}

		private int GetTypeMetaRef(Type t)
		{
			string assemblyQualifiedName = t.get_AssemblyQualifiedName();
			LuaDLL.luaL_getmetatable(this.lua.L, assemblyQualifiedName);
			return LuaDLL.luaL_ref(this.lua.L, LuaIndexes.LUA_REGISTRYINDEX);
		}

		private void OnBundleLoaded()
		{
			this.DoFile("Global");
			this.InitLayers(this.lua.L);
			this.unpackVec3 = this.GetLuaReference("Vector3.Get");
			this.unpackVec2 = this.GetLuaReference("Vector2.Get");
			this.unpackVec4 = this.GetLuaReference("Vector4.Get");
			this.unpackQuat = this.GetLuaReference("Quaternion.Get");
			this.unpackColor = this.GetLuaReference("Color.Get");
			this.unpackRay = this.GetLuaReference("Ray.Get");
			this.unpackBounds = this.GetLuaReference("Bounds.Get");
			this.packVec3 = this.GetLuaReference("Vector3.New");
			this.packVec2 = this.GetLuaReference("Vector2.New");
			this.packVec4 = this.GetLuaReference("Vector4.New");
			this.packQuat = this.GetLuaReference("Quaternion.New");
			this.packRaycastHit = this.GetLuaFunction("Raycast.New");
			this.packColor = this.GetLuaReference("Color.New");
			this.packRay = this.GetLuaReference("Ray.New");
			this.packTouch = this.GetLuaFunction("Touch.New");
			this.packBounds = this.GetLuaReference("Bounds.New");
			LuaScriptMgr.traceback = this.GetLuaFunction("traceback");
			this.DoFile("Main");
			this.updateFunc = this.GetLuaFunction("Update");
			this.lateUpdateFunc = this.GetLuaFunction("LateUpdate");
			this.fixedUpdateFunc = this.GetLuaFunction("FixedUpdate");
			this.levelLoaded = this.GetLuaFunction("OnLevelWasLoaded");
			this.CallLuaFunction("Main", new object[0]);
		}

		public void OnLevelLoaded(int level)
		{
			this.levelLoaded.Call((double)level);
		}

		public void Update()
		{
			if (this.updateFunc != null)
			{
				int oldTop = this.updateFunc.BeginPCall();
				IntPtr luaState = this.updateFunc.GetLuaState();
				LuaScriptMgr.Push(luaState, Time.deltaTime);
				LuaScriptMgr.Push(luaState, Time.unscaledDeltaTime);
				this.updateFunc.PCall(oldTop, 2);
				this.updateFunc.EndPCall(oldTop);
			}
			while (!LuaScriptMgr.refGCList.IsEmpty())
			{
				LuaRef luaRef = LuaScriptMgr.refGCList.Dequeue();
				LuaDLL.lua_unref(luaRef.L, luaRef.reference);
			}
		}

		public void LateUpate()
		{
			if (this.lateUpdateFunc != null)
			{
				this.lateUpdateFunc.Call();
			}
		}

		public void FixedUpdate()
		{
			if (this.fixedUpdateFunc != null)
			{
				this.fixedUpdateFunc.Call((double)Time.fixedDeltaTime);
			}
		}

		private void SafeRelease(ref LuaFunction func)
		{
			if (func != null)
			{
				func.Release();
				func = null;
			}
		}

		private void SafeUnRef(ref int reference)
		{
			if (reference > 0)
			{
				LuaDLL.lua_unref(this.lua.L, reference);
				reference = -1;
			}
		}

		public static void ResetStaticVars()
		{
			LuaScriptMgr.refGCList = new LockFreeQueue<LuaRef>(1024);
			LuaScriptMgr._translator = null;
			LuaScriptMgr.checkBaseType = new HashSet<Type>();
			LuaScriptMgr.traceback = null;
		}

		public void Destroy()
		{
			LuaScriptMgr.Instance = null;
			this.DoFile = null;
			this.SafeUnRef(ref this.enumMetaRef);
			this.SafeUnRef(ref this.typeMetaRef);
			this.SafeUnRef(ref this.delegateMetaRef);
			this.SafeUnRef(ref this.iterMetaRef);
			this.SafeUnRef(ref this.arrayMetaRef);
			this.SafeRelease(ref this.packRaycastHit);
			this.SafeRelease(ref this.packTouch);
			this.SafeRelease(ref this.updateFunc);
			this.SafeRelease(ref this.lateUpdateFunc);
			this.SafeRelease(ref this.fixedUpdateFunc);
			LuaDLL.lua_gc(this.lua.L, LuaGCOptions.LUA_GCCOLLECT, 0);
			using (Dictionary<string, LuaBase>.Enumerator enumerator = this.dict.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, LuaBase> current = enumerator.get_Current();
					current.get_Value().Dispose();
				}
			}
			this.dict.Clear();
			this.fileList.Clear();
			this.lua.Close();
			this.lua.Dispose();
			this.lua = null;
			DelegateFactory.Clear();
			LuaBinder.wrapList.Clear();
			Debug.Log("Lua module destroy");
		}

		public object[] DoString(string str)
		{
			return this.lua.DoString(str);
		}

		public object[] DefaultFileExecutor(string fileName)
		{
			if (!this.fileList.Contains(fileName))
			{
				return this.lua.DoFile(fileName, null);
			}
			return null;
		}

		public object[] CallLuaFunction(string name, params object[] args)
		{
			LuaBase luaBase = null;
			if (this.dict.TryGetValue(name, ref luaBase))
			{
				LuaFunction luaFunction = luaBase as LuaFunction;
				return luaFunction.Call(args);
			}
			IntPtr l = this.lua.L;
			int newTop = LuaDLL.lua_gettop(l);
			if (LuaScriptMgr.PushLuaFunction(l, name))
			{
				int reference = LuaDLL.luaL_ref(l, LuaIndexes.LUA_REGISTRYINDEX);
				LuaFunction luaFunction2 = new LuaFunction(reference, this.lua);
				LuaDLL.lua_settop(l, newTop);
				object[] result = luaFunction2.Call(args);
				luaFunction2.Dispose();
				return result;
			}
			return null;
		}

		public LuaFunction GetLuaFunction(string name)
		{
			LuaBase luaBase = null;
			if (!this.dict.TryGetValue(name, ref luaBase))
			{
				IntPtr l = this.lua.L;
				int newTop = LuaDLL.lua_gettop(l);
				if (LuaScriptMgr.PushLuaFunction(l, name))
				{
					int reference = LuaDLL.luaL_ref(l, LuaIndexes.LUA_REGISTRYINDEX);
					luaBase = new LuaFunction(reference, this.lua);
					luaBase.name = name;
					this.dict.Add(name, luaBase);
				}
				else
				{
					Debug.LogError("Lua function " + name + " not exists");
				}
				LuaDLL.lua_settop(l, newTop);
			}
			else
			{
				luaBase.AddRef();
			}
			return luaBase as LuaFunction;
		}

		public int GetFunctionRef(string name)
		{
			IntPtr l = this.lua.L;
			int newTop = LuaDLL.lua_gettop(l);
			int result = -1;
			if (LuaScriptMgr.PushLuaFunction(l, name))
			{
				result = LuaDLL.luaL_ref(l, LuaIndexes.LUA_REGISTRYINDEX);
			}
			else
			{
				Debug.LogWarning("Lua function " + name + " not exists");
			}
			LuaDLL.lua_settop(l, newTop);
			return result;
		}

		public bool IsFuncExists(string name)
		{
			IntPtr l = this.lua.L;
			int newTop = LuaDLL.lua_gettop(l);
			if (LuaScriptMgr.PushLuaFunction(l, name))
			{
				LuaDLL.lua_settop(l, newTop);
				return true;
			}
			return false;
		}

		private static bool PushLuaTable(IntPtr L, string fullPath)
		{
			string[] array = fullPath.Split(new char[]
			{
				'.'
			});
			int num = LuaDLL.lua_gettop(L);
			LuaDLL.lua_pushstring(L, array[0]);
			LuaDLL.lua_rawget(L, LuaIndexes.LUA_GLOBALSINDEX);
			LuaTypes luaTypes = LuaDLL.lua_type(L, -1);
			if (luaTypes != LuaTypes.LUA_TTABLE)
			{
				LuaDLL.lua_settop(L, num);
				LuaDLL.lua_pushnil(L);
				Debug.LogError("Push lua table " + array[0] + " failed");
				return false;
			}
			for (int i = 1; i < array.Length; i++)
			{
				LuaDLL.lua_pushstring(L, array[i]);
				LuaDLL.lua_rawget(L, -2);
				luaTypes = LuaDLL.lua_type(L, -1);
				if (luaTypes != LuaTypes.LUA_TTABLE)
				{
					LuaDLL.lua_settop(L, num);
					Debug.LogError("Push lua table " + fullPath + " failed");
					return false;
				}
			}
			if (array.Length > 1)
			{
				LuaDLL.lua_insert(L, num + 1);
				LuaDLL.lua_settop(L, num + 1);
			}
			return true;
		}

		private static bool PushLuaFunction(IntPtr L, string fullPath)
		{
			int num = LuaDLL.lua_gettop(L);
			int num2 = fullPath.LastIndexOf('.');
			if (num2 > 0)
			{
				string fullPath2 = fullPath.Substring(0, num2);
				if (LuaScriptMgr.PushLuaTable(L, fullPath2))
				{
					string str = fullPath.Substring(num2 + 1);
					LuaDLL.lua_pushstring(L, str);
					LuaDLL.lua_rawget(L, -2);
				}
				LuaTypes luaTypes = LuaDLL.lua_type(L, -1);
				if (luaTypes != LuaTypes.LUA_TFUNCTION)
				{
					LuaDLL.lua_settop(L, num);
					return false;
				}
				LuaDLL.lua_insert(L, num + 1);
				LuaDLL.lua_settop(L, num + 1);
			}
			else
			{
				LuaDLL.lua_getglobal(L, fullPath);
				LuaTypes luaTypes2 = LuaDLL.lua_type(L, -1);
				if (luaTypes2 != LuaTypes.LUA_TFUNCTION)
				{
					LuaDLL.lua_settop(L, num);
					return false;
				}
			}
			return true;
		}

		public LuaTable GetLuaTable(string tableName)
		{
			LuaBase luaBase = null;
			if (!this.dict.TryGetValue(tableName, ref luaBase))
			{
				IntPtr l = this.lua.L;
				int newTop = LuaDLL.lua_gettop(l);
				if (LuaScriptMgr.PushLuaTable(l, tableName))
				{
					int reference = LuaDLL.luaL_ref(l, LuaIndexes.LUA_REGISTRYINDEX);
					luaBase = new LuaTable(reference, this.lua);
					luaBase.name = tableName;
					this.dict.Add(tableName, luaBase);
				}
				LuaDLL.lua_settop(l, newTop);
			}
			else
			{
				luaBase.AddRef();
			}
			return luaBase as LuaTable;
		}

		public void RemoveLuaRes(string name)
		{
			this.dict.Remove(name);
		}

		private static void CreateTable(IntPtr L, string fullPath)
		{
			string[] array = fullPath.Split(new char[]
			{
				'.'
			});
			int num = LuaDLL.lua_gettop(L);
			if (array.Length > 1)
			{
				LuaDLL.lua_getglobal(L, array[0]);
				if (LuaDLL.lua_type(L, -1) == LuaTypes.LUA_TNIL)
				{
					LuaDLL.lua_pop(L, 1);
					LuaDLL.lua_createtable(L, 0, 0);
					LuaDLL.lua_pushstring(L, array[0]);
					LuaDLL.lua_pushvalue(L, -2);
					LuaDLL.lua_settable(L, LuaIndexes.LUA_GLOBALSINDEX);
				}
				for (int i = 1; i < array.Length - 1; i++)
				{
					LuaDLL.lua_pushstring(L, array[i]);
					LuaDLL.lua_rawget(L, -2);
					if (LuaDLL.lua_type(L, -1) == LuaTypes.LUA_TNIL)
					{
						LuaDLL.lua_pop(L, 1);
						LuaDLL.lua_createtable(L, 0, 0);
						LuaDLL.lua_pushstring(L, array[i]);
						LuaDLL.lua_pushvalue(L, -2);
						LuaDLL.lua_rawset(L, -4);
					}
				}
				LuaDLL.lua_pushstring(L, array[array.Length - 1]);
				LuaDLL.lua_rawget(L, -2);
				if (LuaDLL.lua_type(L, -1) == LuaTypes.LUA_TNIL)
				{
					LuaDLL.lua_pop(L, 1);
					LuaDLL.lua_createtable(L, 0, 0);
					LuaDLL.lua_pushstring(L, array[array.Length - 1]);
					LuaDLL.lua_pushvalue(L, -2);
					LuaDLL.lua_rawset(L, -4);
				}
			}
			else
			{
				LuaDLL.lua_getglobal(L, array[0]);
				if (LuaDLL.lua_type(L, -1) == LuaTypes.LUA_TNIL)
				{
					LuaDLL.lua_pop(L, 1);
					LuaDLL.lua_createtable(L, 0, 0);
					LuaDLL.lua_pushstring(L, array[0]);
					LuaDLL.lua_pushvalue(L, -2);
					LuaDLL.lua_settable(L, LuaIndexes.LUA_GLOBALSINDEX);
				}
			}
			LuaDLL.lua_insert(L, num + 1);
			LuaDLL.lua_settop(L, num + 1);
		}

		public static void RegisterLib(IntPtr L, string libName, Type t, LuaMethod[] regs)
		{
			LuaScriptMgr.CreateTable(L, libName);
			LuaDLL.luaL_getmetatable(L, t.get_AssemblyQualifiedName());
			if (LuaDLL.lua_isnil(L, -1))
			{
				LuaDLL.lua_pop(L, 1);
				LuaDLL.luaL_newmetatable(L, t.get_AssemblyQualifiedName());
			}
			LuaDLL.lua_pushstring(L, "ToLua_EnumIndex");
			LuaDLL.lua_rawget(L, LuaIndexes.LUA_REGISTRYINDEX);
			LuaDLL.lua_setfield(L, -2, "__index");
			LuaDLL.lua_pushstring(L, "__gc");
			LuaDLL.lua_pushstdcallcfunction(L, new LuaCSFunction(LuaScriptMgr.__gc), 0);
			LuaDLL.lua_rawset(L, -3);
			for (int i = 0; i < regs.Length - 1; i++)
			{
				LuaDLL.lua_pushstring(L, regs[i].name);
				LuaDLL.lua_pushstdcallcfunction(L, regs[i].func, 0);
				LuaDLL.lua_rawset(L, -3);
			}
			int num = regs.Length - 1;
			LuaDLL.lua_pushstring(L, regs[num].name);
			LuaDLL.lua_pushstdcallcfunction(L, regs[num].func, 0);
			LuaDLL.lua_rawset(L, -4);
			LuaDLL.lua_setmetatable(L, -2);
			LuaDLL.lua_settop(L, 0);
		}

		public static void RegisterLib(IntPtr L, string libName, LuaMethod[] regs)
		{
			LuaScriptMgr.CreateTable(L, libName);
			for (int i = 0; i < regs.Length; i++)
			{
				LuaDLL.lua_pushstring(L, regs[i].name);
				LuaDLL.lua_pushstdcallcfunction(L, regs[i].func, 0);
				LuaDLL.lua_rawset(L, -3);
			}
			LuaDLL.lua_settop(L, 0);
		}

		[MonoPInvokeCallback(typeof(LuaCSFunction))]
		public static int __gc(IntPtr luaState)
		{
			int num = LuaDLL.luanet_rawnetobj(luaState, 1);
			if (num != -1)
			{
				ObjectTranslator objectTranslator = ObjectTranslator.FromState(luaState);
				objectTranslator.collectObject(num);
			}
			return 0;
		}

		public static void RegisterLib(IntPtr L, string libName, Type t, LuaMethod[] regs, LuaField[] fields, Type baseType)
		{
			LuaScriptMgr.CreateTable(L, libName);
			LuaDLL.luaL_getmetatable(L, t.get_AssemblyQualifiedName());
			if (LuaDLL.lua_isnil(L, -1))
			{
				LuaDLL.lua_pop(L, 1);
				LuaDLL.luaL_newmetatable(L, t.get_AssemblyQualifiedName());
			}
			if (baseType != null)
			{
				LuaDLL.luaL_getmetatable(L, baseType.get_AssemblyQualifiedName());
				if (LuaDLL.lua_isnil(L, -1))
				{
					LuaDLL.lua_pop(L, 1);
					LuaDLL.luaL_newmetatable(L, baseType.get_AssemblyQualifiedName());
					LuaScriptMgr.checkBaseType.Add(baseType);
				}
				else
				{
					LuaScriptMgr.checkBaseType.Remove(baseType);
				}
				LuaDLL.lua_setmetatable(L, -2);
			}
			LuaDLL.tolua_setindex(L);
			LuaDLL.tolua_setnewindex(L);
			LuaDLL.lua_pushstring(L, "__call");
			LuaDLL.lua_pushstring(L, "ToLua_TableCall");
			LuaDLL.lua_rawget(L, LuaIndexes.LUA_REGISTRYINDEX);
			LuaDLL.lua_rawset(L, -3);
			LuaDLL.lua_pushstring(L, "__gc");
			LuaDLL.lua_pushstdcallcfunction(L, new LuaCSFunction(LuaScriptMgr.__gc), 0);
			LuaDLL.lua_rawset(L, -3);
			for (int i = 0; i < regs.Length; i++)
			{
				LuaDLL.lua_pushstring(L, regs[i].name);
				LuaDLL.lua_pushstdcallcfunction(L, regs[i].func, 0);
				LuaDLL.lua_rawset(L, -3);
			}
			for (int j = 0; j < fields.Length; j++)
			{
				LuaDLL.lua_pushstring(L, fields[j].name);
				LuaDLL.lua_createtable(L, 2, 0);
				if (fields[j].getter != null)
				{
					LuaDLL.lua_pushstdcallcfunction(L, fields[j].getter, 0);
					LuaDLL.lua_rawseti(L, -2, 1);
				}
				if (fields[j].setter != null)
				{
					LuaDLL.lua_pushstdcallcfunction(L, fields[j].setter, 0);
					LuaDLL.lua_rawseti(L, -2, 2);
				}
				LuaDLL.lua_rawset(L, -3);
			}
			LuaDLL.lua_setmetatable(L, -2);
			LuaDLL.lua_settop(L, 0);
			LuaScriptMgr.checkBaseType.Remove(t);
		}

		public static double GetNumber(IntPtr L, int stackPos)
		{
			if (LuaDLL.lua_isnumber(L, stackPos))
			{
				return LuaDLL.lua_tonumber(L, stackPos);
			}
			LuaDLL.luaL_typerror(L, stackPos, "number");
			return 0.0;
		}

		public static bool GetBoolean(IntPtr L, int stackPos)
		{
			if (LuaDLL.lua_isboolean(L, stackPos))
			{
				return LuaDLL.lua_toboolean(L, stackPos);
			}
			LuaDLL.luaL_typerror(L, stackPos, "boolean");
			return false;
		}

		public static string GetString(IntPtr L, int stackPos)
		{
			string luaString = LuaScriptMgr.GetLuaString(L, stackPos);
			if (luaString == null)
			{
				LuaDLL.luaL_typerror(L, stackPos, "string");
			}
			return luaString;
		}

		public static LuaFunction GetFunction(IntPtr L, int stackPos)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, stackPos);
			if (luaTypes != LuaTypes.LUA_TFUNCTION)
			{
				return null;
			}
			LuaDLL.lua_pushvalue(L, stackPos);
			return new LuaFunction(LuaDLL.luaL_ref(L, LuaIndexes.LUA_REGISTRYINDEX), LuaScriptMgr.GetTranslator(L).interpreter);
		}

		public static LuaFunction ToLuaFunction(IntPtr L, int stackPos)
		{
			LuaDLL.lua_pushvalue(L, stackPos);
			return new LuaFunction(LuaDLL.luaL_ref(L, LuaIndexes.LUA_REGISTRYINDEX), LuaScriptMgr.GetTranslator(L).interpreter);
		}

		public static LuaFunction GetLuaFunction(IntPtr L, int stackPos)
		{
			LuaFunction function = LuaScriptMgr.GetFunction(L, stackPos);
			if (function == null)
			{
				LuaDLL.luaL_typerror(L, stackPos, "function");
				return null;
			}
			return function;
		}

		private static LuaTable ToLuaTable(IntPtr L, int stackPos)
		{
			LuaDLL.lua_pushvalue(L, stackPos);
			return new LuaTable(LuaDLL.luaL_ref(L, LuaIndexes.LUA_REGISTRYINDEX), LuaScriptMgr.GetTranslator(L).interpreter);
		}

		private static LuaTable GetTable(IntPtr L, int stackPos)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, stackPos);
			if (luaTypes != LuaTypes.LUA_TTABLE)
			{
				return null;
			}
			LuaDLL.lua_pushvalue(L, stackPos);
			return new LuaTable(LuaDLL.luaL_ref(L, LuaIndexes.LUA_REGISTRYINDEX), LuaScriptMgr.GetTranslator(L).interpreter);
		}

		public static LuaTable GetLuaTable(IntPtr L, int stackPos)
		{
			LuaTable table = LuaScriptMgr.GetTable(L, stackPos);
			if (table == null)
			{
				LuaDLL.luaL_typerror(L, stackPos, "table");
				return null;
			}
			return table;
		}

		public static object GetLuaObject(IntPtr L, int stackPos)
		{
			return LuaScriptMgr.GetTranslator(L).getRawNetObject(L, stackPos);
		}

		public static object GetNetObjectSelf(IntPtr L, int stackPos, string type)
		{
			object rawNetObject = LuaScriptMgr.GetTranslator(L).getRawNetObject(L, stackPos);
			if (rawNetObject == null)
			{
				LuaDLL.luaL_argerror(L, stackPos, string.Format("{0} expected, got nil", type));
				return null;
			}
			return rawNetObject;
		}

		public static object GetUnityObjectSelf(IntPtr L, int stackPos, string type)
		{
			object rawNetObject = LuaScriptMgr.GetTranslator(L).getRawNetObject(L, stackPos);
			Object x = (Object)rawNetObject;
			if (x == null)
			{
				LuaDLL.luaL_argerror(L, stackPos, string.Format("{0} expected, got nil", type));
				return null;
			}
			return rawNetObject;
		}

		public static object GetTrackedObjectSelf(IntPtr L, int stackPos, string type)
		{
			object rawNetObject = LuaScriptMgr.GetTranslator(L).getRawNetObject(L, stackPos);
			TrackedReference x = (TrackedReference)rawNetObject;
			if (x == null)
			{
				LuaDLL.luaL_argerror(L, stackPos, string.Format("{0} expected, got nil", type));
				return null;
			}
			return rawNetObject;
		}

		public static T GetNetObject<T>(IntPtr L, int stackPos)
		{
			return (T)((object)LuaScriptMgr.GetNetObject(L, stackPos, typeof(T)));
		}

		public static object GetNetObject(IntPtr L, int stackPos, Type type)
		{
			if (LuaDLL.lua_isnil(L, stackPos))
			{
				return null;
			}
			object luaObject = LuaScriptMgr.GetLuaObject(L, stackPos);
			if (luaObject == null)
			{
				LuaDLL.luaL_argerror(L, stackPos, string.Format("{0} expected, got nil", type.get_Name()));
				return null;
			}
			Type type2 = luaObject.GetType();
			if (type == type2 || type.IsAssignableFrom(type2))
			{
				return luaObject;
			}
			LuaDLL.luaL_argerror(L, stackPos, string.Format("{0} expected, got {1}", type.get_Name(), type2.get_Name()));
			return null;
		}

		public static T GetUnityObject<T>(IntPtr L, int stackPos) where T : Object
		{
			return (T)((object)LuaScriptMgr.GetUnityObject(L, stackPos, typeof(T)));
		}

		public static Object GetUnityObject(IntPtr L, int stackPos, Type type)
		{
			if (LuaDLL.lua_isnil(L, stackPos))
			{
				return null;
			}
			object luaObject = LuaScriptMgr.GetLuaObject(L, stackPos);
			if (luaObject == null)
			{
				LuaDLL.luaL_argerror(L, stackPos, string.Format("{0} expected, got nil", type.get_Name()));
				return null;
			}
			Object @object = (Object)luaObject;
			if (@object == null)
			{
				LuaDLL.luaL_argerror(L, stackPos, string.Format("{0} expected, got nil", type.get_Name()));
				return null;
			}
			Type type2 = @object.GetType();
			if (type == type2 || type2.IsSubclassOf(type))
			{
				return @object;
			}
			LuaDLL.luaL_argerror(L, stackPos, string.Format("{0} expected, got {1}", type.get_Name(), type2.get_Name()));
			return null;
		}

		public static T GetTrackedObject<T>(IntPtr L, int stackPos) where T : TrackedReference
		{
			return (T)((object)LuaScriptMgr.GetTrackedObject(L, stackPos, typeof(T)));
		}

		public static TrackedReference GetTrackedObject(IntPtr L, int stackPos, Type type)
		{
			if (LuaDLL.lua_isnil(L, stackPos))
			{
				return null;
			}
			object luaObject = LuaScriptMgr.GetLuaObject(L, stackPos);
			if (luaObject == null)
			{
				LuaDLL.luaL_argerror(L, stackPos, string.Format("{0} expected, got nil", type.get_Name()));
				return null;
			}
			TrackedReference trackedReference = luaObject as TrackedReference;
			if (trackedReference == null)
			{
				LuaDLL.luaL_argerror(L, stackPos, string.Format("{0} expected, got nil", type.get_Name()));
				return null;
			}
			Type type2 = luaObject.GetType();
			if (type == type2 || type2.IsSubclassOf(type))
			{
				return trackedReference;
			}
			LuaDLL.luaL_argerror(L, stackPos, string.Format("{0} expected, got {1}", type.get_Name(), type2.get_Name()));
			return null;
		}

		public static Type GetTypeObject(IntPtr L, int stackPos)
		{
			object luaObject = LuaScriptMgr.GetLuaObject(L, stackPos);
			if (luaObject == null || luaObject.GetType() != LuaScriptMgr.monoType)
			{
				LuaDLL.luaL_argerror(L, stackPos, string.Format("Type expected, got {0}", (luaObject == null) ? "nil" : luaObject.GetType().get_Name()));
			}
			return (Type)luaObject;
		}

		public static bool IsClassOf(Type child, Type parent)
		{
			return child == parent || parent.IsAssignableFrom(child);
		}

		private static ObjectTranslator GetTranslator(IntPtr L)
		{
			if (LuaScriptMgr._translator == null)
			{
				return ObjectTranslator.FromState(L);
			}
			return LuaScriptMgr._translator;
		}

		public static void PushVarObject(IntPtr L, object o)
		{
			if (o == null)
			{
				LuaDLL.lua_pushnil(L);
				return;
			}
			Type type = o.GetType();
			if (type.get_IsValueType())
			{
				if (type == typeof(bool))
				{
					bool value = (bool)o;
					LuaDLL.lua_pushboolean(L, value);
				}
				else if (type.get_IsEnum())
				{
					LuaScriptMgr.Push(L, (Enum)o);
				}
				else if (type.get_IsPrimitive())
				{
					double number = Convert.ToDouble(o);
					LuaDLL.lua_pushnumber(L, number);
				}
				else if (type == typeof(Vector3))
				{
					LuaScriptMgr.Push(L, (Vector3)o);
				}
				else if (type == typeof(Vector2))
				{
					LuaScriptMgr.Push(L, (Vector2)o);
				}
				else if (type == typeof(Vector4))
				{
					LuaScriptMgr.Push(L, (Vector4)o);
				}
				else if (type == typeof(Quaternion))
				{
					LuaScriptMgr.Push(L, (Quaternion)o);
				}
				else if (type == typeof(Color))
				{
					LuaScriptMgr.Push(L, (Color)o);
				}
				else if (type == typeof(RaycastHit))
				{
					LuaScriptMgr.Push(L, (RaycastHit)o);
				}
				else if (type == typeof(Touch))
				{
					LuaScriptMgr.Push(L, (Touch)o);
				}
				else if (type == typeof(Ray))
				{
					LuaScriptMgr.Push(L, (Ray)o);
				}
				else
				{
					LuaScriptMgr.PushValue(L, o);
				}
			}
			else if (type.get_IsArray())
			{
				LuaScriptMgr.PushArray(L, (Array)o);
			}
			else if (type == typeof(LuaCSFunction))
			{
				LuaScriptMgr.GetTranslator(L).pushFunction(L, (LuaCSFunction)o);
			}
			else if (type.IsSubclassOf(typeof(Delegate)))
			{
				LuaScriptMgr.Push(L, (Delegate)o);
			}
			else if (LuaScriptMgr.IsClassOf(type, typeof(IEnumerator)))
			{
				LuaScriptMgr.Push(L, (IEnumerator)o);
			}
			else if (type == typeof(string))
			{
				string str = (string)o;
				LuaDLL.lua_pushstring(L, str);
			}
			else if (type == typeof(LuaStringBuffer))
			{
				LuaStringBuffer luaStringBuffer = (LuaStringBuffer)o;
				LuaDLL.lua_pushlstring(L, luaStringBuffer.buffer, luaStringBuffer.buffer.Length);
			}
			else if (type.IsSubclassOf(typeof(Object)))
			{
				Object x = (Object)o;
				if (x == null)
				{
					LuaDLL.lua_pushnil(L);
				}
				else
				{
					LuaScriptMgr.PushObject(L, o);
				}
			}
			else if (type == typeof(LuaTable))
			{
				((LuaTable)o).push(L);
			}
			else if (type == typeof(LuaFunction))
			{
				((LuaFunction)o).push(L);
			}
			else if (type == LuaScriptMgr.monoType)
			{
				LuaScriptMgr.Push(L, (Type)o);
			}
			else if (type.IsSubclassOf(typeof(TrackedReference)))
			{
				TrackedReference x2 = (TrackedReference)o;
				if (x2 == null)
				{
					LuaDLL.lua_pushnil(L);
				}
				else
				{
					LuaScriptMgr.PushObject(L, o);
				}
			}
			else
			{
				LuaScriptMgr.PushObject(L, o);
			}
		}

		public static void PushObject(IntPtr L, object o)
		{
			LuaScriptMgr.GetTranslator(L).pushObject(L, o, "luaNet_metatable");
		}

		public static void Push(IntPtr L, Object obj)
		{
			LuaScriptMgr.PushObject(L, (obj == null) ? null : obj);
		}

		public static void Push(IntPtr L, TrackedReference obj)
		{
			LuaScriptMgr.PushObject(L, (obj == null) ? null : obj);
		}

		private static void PushMetaObject(IntPtr L, ObjectTranslator translator, object o, int metaRef)
		{
			if (o == null)
			{
				LuaDLL.lua_pushnil(L);
				return;
			}
			int weakTableRef = translator.weakTableRef;
			int num = -1;
			bool flag = translator.objectsBackMap.TryGetValue(o, ref num);
			if (flag)
			{
				if (LuaDLL.tolua_pushudata(L, weakTableRef, num))
				{
					return;
				}
				translator.collectObject(num);
			}
			num = translator.addObject(o, false);
			LuaDLL.tolua_pushnewudata(L, metaRef, weakTableRef, num);
		}

		public static void Push(IntPtr L, Type o)
		{
			LuaScriptMgr mgrFromLuaState = LuaScriptMgr.GetMgrFromLuaState(L);
			ObjectTranslator translator = mgrFromLuaState.lua.translator;
			LuaScriptMgr.PushMetaObject(L, translator, o, mgrFromLuaState.typeMetaRef);
		}

		public static void Push(IntPtr L, Delegate o)
		{
			LuaScriptMgr mgrFromLuaState = LuaScriptMgr.GetMgrFromLuaState(L);
			ObjectTranslator translator = mgrFromLuaState.lua.translator;
			LuaScriptMgr.PushMetaObject(L, translator, o, mgrFromLuaState.delegateMetaRef);
		}

		public static void Push(IntPtr L, IEnumerator o)
		{
			LuaScriptMgr mgrFromLuaState = LuaScriptMgr.GetMgrFromLuaState(L);
			ObjectTranslator translator = mgrFromLuaState.lua.translator;
			LuaScriptMgr.PushMetaObject(L, translator, o, mgrFromLuaState.iterMetaRef);
		}

		public static void PushArray(IntPtr L, Array o)
		{
			LuaScriptMgr mgrFromLuaState = LuaScriptMgr.GetMgrFromLuaState(L);
			ObjectTranslator translator = mgrFromLuaState.lua.translator;
			LuaScriptMgr.PushMetaObject(L, translator, o, mgrFromLuaState.arrayMetaRef);
		}

		public static void PushValue(IntPtr L, object obj)
		{
			LuaScriptMgr.GetTranslator(L).PushValueResult(L, obj);
		}

		public static void Push(IntPtr L, bool b)
		{
			LuaDLL.lua_pushboolean(L, b);
		}

		public static void Push(IntPtr L, string str)
		{
			LuaDLL.lua_pushstring(L, str);
		}

		public static void Push(IntPtr L, char d)
		{
			LuaDLL.lua_pushinteger(L, (int)d);
		}

		public static void Push(IntPtr L, sbyte d)
		{
			LuaDLL.lua_pushinteger(L, (int)d);
		}

		public static void Push(IntPtr L, byte d)
		{
			LuaDLL.lua_pushinteger(L, (int)d);
		}

		public static void Push(IntPtr L, short d)
		{
			LuaDLL.lua_pushinteger(L, (int)d);
		}

		public static void Push(IntPtr L, ushort d)
		{
			LuaDLL.lua_pushinteger(L, (int)d);
		}

		public static void Push(IntPtr L, int d)
		{
			LuaDLL.lua_pushinteger(L, d);
		}

		public static void Push(IntPtr L, uint d)
		{
			LuaDLL.lua_pushnumber(L, d);
		}

		public static void Push(IntPtr L, long d)
		{
			LuaDLL.lua_pushnumber(L, (double)d);
		}

		public static void Push(IntPtr L, ulong d)
		{
			LuaDLL.lua_pushnumber(L, d);
		}

		public static void Push(IntPtr L, float d)
		{
			LuaDLL.lua_pushnumber(L, (double)d);
		}

		public static void Push(IntPtr L, decimal d)
		{
			LuaDLL.lua_pushnumber(L, (double)d);
		}

		public static void Push(IntPtr L, double d)
		{
			LuaDLL.lua_pushnumber(L, d);
		}

		public static void Push(IntPtr L, IntPtr p)
		{
			LuaDLL.lua_pushlightuserdata(L, p);
		}

		public static void Push(IntPtr L, ILuaGeneratedType o)
		{
			if (o == null)
			{
				LuaDLL.lua_pushnil(L);
			}
			else
			{
				LuaTable luaTable = o.__luaInterface_getLuaTable();
				luaTable.push(L);
			}
		}

		public static void Push(IntPtr L, LuaTable table)
		{
			if (table == null)
			{
				LuaDLL.lua_pushnil(L);
			}
			else
			{
				table.push(L);
			}
		}

		public static void Push(IntPtr L, LuaFunction func)
		{
			if (func == null)
			{
				LuaDLL.lua_pushnil(L);
			}
			else
			{
				func.push();
			}
		}

		public static void Push(IntPtr L, LuaCSFunction func)
		{
			if (func == null)
			{
				LuaDLL.lua_pushnil(L);
				return;
			}
			LuaScriptMgr.GetTranslator(L).pushFunction(L, func);
		}

		public static bool CheckTypes(IntPtr L, int begin, Type type0)
		{
			return LuaScriptMgr.CheckType(L, type0, begin);
		}

		public static bool CheckTypes(IntPtr L, int begin, Type type0, Type type1)
		{
			return LuaScriptMgr.CheckType(L, type0, begin) && LuaScriptMgr.CheckType(L, type1, begin + 1);
		}

		public static bool CheckTypes(IntPtr L, int begin, Type type0, Type type1, Type type2)
		{
			return LuaScriptMgr.CheckType(L, type0, begin) && LuaScriptMgr.CheckType(L, type1, begin + 1) && LuaScriptMgr.CheckType(L, type2, begin + 2);
		}

		public static bool CheckTypes(IntPtr L, int begin, Type type0, Type type1, Type type2, Type type3)
		{
			return LuaScriptMgr.CheckType(L, type0, begin) && LuaScriptMgr.CheckType(L, type1, begin + 1) && LuaScriptMgr.CheckType(L, type2, begin + 2) && LuaScriptMgr.CheckType(L, type3, begin + 3);
		}

		public static bool CheckTypes(IntPtr L, int begin, Type type0, Type type1, Type type2, Type type3, Type type4)
		{
			return LuaScriptMgr.CheckType(L, type0, begin) && LuaScriptMgr.CheckType(L, type1, begin + 1) && LuaScriptMgr.CheckType(L, type2, begin + 2) && LuaScriptMgr.CheckType(L, type3, begin + 3) && LuaScriptMgr.CheckType(L, type4, begin + 4);
		}

		public static bool CheckTypes(IntPtr L, int begin, Type type0, Type type1, Type type2, Type type3, Type type4, Type type5)
		{
			return LuaScriptMgr.CheckType(L, type0, begin) && LuaScriptMgr.CheckType(L, type1, begin + 1) && LuaScriptMgr.CheckType(L, type2, begin + 2) && LuaScriptMgr.CheckType(L, type3, begin + 3) && LuaScriptMgr.CheckType(L, type4, begin + 4) && LuaScriptMgr.CheckType(L, type5, begin + 5);
		}

		public static bool CheckTypes(IntPtr L, int begin, Type type0, Type type1, Type type2, Type type3, Type type4, Type type5, Type type6)
		{
			return LuaScriptMgr.CheckType(L, type0, begin) && LuaScriptMgr.CheckType(L, type1, begin + 1) && LuaScriptMgr.CheckType(L, type2, begin + 2) && LuaScriptMgr.CheckType(L, type3, begin + 3) && LuaScriptMgr.CheckType(L, type4, begin + 4) && LuaScriptMgr.CheckType(L, type5, begin + 5) && LuaScriptMgr.CheckType(L, type6, begin + 6);
		}

		public static bool CheckTypes(IntPtr L, int begin, Type type0, Type type1, Type type2, Type type3, Type type4, Type type5, Type type6, Type type7)
		{
			return LuaScriptMgr.CheckType(L, type0, begin) && LuaScriptMgr.CheckType(L, type1, begin + 1) && LuaScriptMgr.CheckType(L, type2, begin + 2) && LuaScriptMgr.CheckType(L, type3, begin + 3) && LuaScriptMgr.CheckType(L, type4, begin + 4) && LuaScriptMgr.CheckType(L, type5, begin + 5) && LuaScriptMgr.CheckType(L, type6, begin + 6) && LuaScriptMgr.CheckType(L, type7, begin + 7);
		}

		public static bool CheckTypes(IntPtr L, int begin, Type type0, Type type1, Type type2, Type type3, Type type4, Type type5, Type type6, Type type7, Type type8)
		{
			return LuaScriptMgr.CheckType(L, type0, begin) && LuaScriptMgr.CheckType(L, type1, begin + 1) && LuaScriptMgr.CheckType(L, type2, begin + 2) && LuaScriptMgr.CheckType(L, type3, begin + 3) && LuaScriptMgr.CheckType(L, type4, begin + 4) && LuaScriptMgr.CheckType(L, type5, begin + 5) && LuaScriptMgr.CheckType(L, type6, begin + 6) && LuaScriptMgr.CheckType(L, type7, begin + 7) && LuaScriptMgr.CheckType(L, type8, begin + 8);
		}

		public static bool CheckTypes(IntPtr L, int begin, Type type0, Type type1, Type type2, Type type3, Type type4, Type type5, Type type6, Type type7, Type type8, Type type9)
		{
			return LuaScriptMgr.CheckType(L, type0, begin) && LuaScriptMgr.CheckType(L, type1, begin + 1) && LuaScriptMgr.CheckType(L, type2, begin + 2) && LuaScriptMgr.CheckType(L, type3, begin + 3) && LuaScriptMgr.CheckType(L, type4, begin + 4) && LuaScriptMgr.CheckType(L, type5, begin + 5) && LuaScriptMgr.CheckType(L, type6, begin + 6) && LuaScriptMgr.CheckType(L, type7, begin + 7) && LuaScriptMgr.CheckType(L, type8, begin + 8) && LuaScriptMgr.CheckType(L, type9, begin + 9);
		}

		public static bool CheckTypes(IntPtr L, int begin, params Type[] types)
		{
			for (int i = 0; i < types.Length; i++)
			{
				if (!LuaScriptMgr.CheckType(L, types[i], i + begin))
				{
					return false;
				}
			}
			return true;
		}

		public static bool CheckParamsType(IntPtr L, Type t, int begin, int count)
		{
			if (t == typeof(object))
			{
				return true;
			}
			for (int i = 0; i < count; i++)
			{
				if (!LuaScriptMgr.CheckType(L, t, i + begin))
				{
					return false;
				}
			}
			return true;
		}

		private static bool CheckTableType(IntPtr L, Type t, int stackPos)
		{
			if (t.get_IsArray())
			{
				return true;
			}
			if (t == typeof(LuaTable))
			{
				return true;
			}
			if (t.get_IsValueType())
			{
				int newTop = LuaDLL.lua_gettop(L);
				LuaDLL.lua_pushvalue(L, stackPos);
				LuaDLL.lua_pushstring(L, "class");
				LuaDLL.lua_gettable(L, -2);
				string text = LuaDLL.lua_tostring(L, -1);
				LuaDLL.lua_settop(L, newTop);
				if (text == "Vector3")
				{
					return t == typeof(Vector3);
				}
				if (text == "Vector2")
				{
					return t == typeof(Vector2);
				}
				if (text == "Quaternion")
				{
					return t == typeof(Quaternion);
				}
				if (text == "Color")
				{
					return t == typeof(Color);
				}
				if (text == "Vector4")
				{
					return t == typeof(Vector4);
				}
				if (text == "Ray")
				{
					return t == typeof(Ray);
				}
			}
			return false;
		}

		public static bool CheckType(IntPtr L, Type t, int pos)
		{
			if (t == typeof(object))
			{
				return true;
			}
			LuaTypes luaType = LuaDLL.lua_type(L, pos);
			switch (luaType)
			{
			case LuaTypes.LUA_TNIL:
				return t == null;
			case LuaTypes.LUA_TBOOLEAN:
				return t == typeof(bool);
			case LuaTypes.LUA_TNUMBER:
				return t.get_IsPrimitive();
			case LuaTypes.LUA_TSTRING:
				return t == typeof(string);
			case LuaTypes.LUA_TTABLE:
				return LuaScriptMgr.CheckTableType(L, t, pos);
			case LuaTypes.LUA_TFUNCTION:
				return t == typeof(LuaFunction);
			case LuaTypes.LUA_TUSERDATA:
				return LuaScriptMgr.CheckUserData(L, luaType, t, pos);
			}
			return false;
		}

		private static bool CheckUserData(IntPtr L, LuaTypes luaType, Type t, int pos)
		{
			object luaObject = LuaScriptMgr.GetLuaObject(L, pos);
			Type type = luaObject.GetType();
			if (t == type)
			{
				return true;
			}
			if (t == typeof(Type))
			{
				return type == LuaScriptMgr.monoType;
			}
			return t.IsAssignableFrom(type);
		}

		public static object[] GetParamsObject(IntPtr L, int stackPos, int count)
		{
			List<object> list = new List<object>();
			while (count > 0)
			{
				object varObject = LuaScriptMgr.GetVarObject(L, stackPos);
				stackPos++;
				count--;
				if (varObject == null)
				{
					LuaDLL.luaL_argerror(L, stackPos, "object expected, got nil");
					break;
				}
				list.Add(varObject);
			}
			return list.ToArray();
		}

		public static T[] GetParamsObject<T>(IntPtr L, int stackPos, int count)
		{
			List<T> list = new List<T>();
			T t = default(T);
			while (count > 0)
			{
				object luaObject = LuaScriptMgr.GetLuaObject(L, stackPos);
				stackPos++;
				count--;
				if (luaObject == null || luaObject.GetType() != typeof(T))
				{
					LuaDLL.luaL_argerror(L, stackPos, string.Format("{0} expected, got nil", typeof(T).get_Name()));
					break;
				}
				T t2 = (T)((object)luaObject);
				list.Add(t2);
			}
			return list.ToArray();
		}

		public static T[] GetArrayObject<T>(IntPtr L, int stackPos)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, stackPos);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				int num = 1;
				T t = default(T);
				List<T> list = new List<T>();
				LuaDLL.lua_pushvalue(L, stackPos);
				Type typeFromHandle = typeof(T);
				while (true)
				{
					LuaDLL.lua_rawgeti(L, -1, num);
					if (LuaDLL.lua_type(L, -1) == LuaTypes.LUA_TNIL)
					{
						break;
					}
					if (!LuaScriptMgr.CheckType(L, typeFromHandle, -1))
					{
						goto Block_3;
					}
					T t2 = (T)((object)LuaScriptMgr.GetVarObject(L, -1));
					list.Add(t2);
					LuaDLL.lua_pop(L, 1);
					num++;
				}
				LuaDLL.lua_pop(L, 1);
				return list.ToArray();
				Block_3:
				LuaDLL.lua_pop(L, 1);
			}
			else if (luaTypes == LuaTypes.LUA_TUSERDATA)
			{
				T[] netObject = LuaScriptMgr.GetNetObject<T[]>(L, stackPos);
				if (netObject != null)
				{
					return netObject;
				}
			}
			else if (luaTypes == LuaTypes.LUA_TNIL)
			{
				return null;
			}
			LuaDLL.luaL_error(L, string.Format("invalid arguments to method: {0}, pos {1}", LuaScriptMgr.GetErrorFunc(1), stackPos));
			return null;
		}

		private static string GetErrorFunc(int skip)
		{
			string text = string.Empty;
			StackTrace stackTrace = new StackTrace(skip, true);
			int num = 0;
			StackFrame frame;
			do
			{
				frame = stackTrace.GetFrame(num++);
				text = frame.GetFileName();
				text = Path.GetFileName(text);
			}
			while (!text.Contains("Wrap."));
			int num2 = text.LastIndexOf('\\');
			int num3 = text.LastIndexOf("Wrap.");
			string text2 = text.Substring(num2 + 1, num3 - num2 - 1);
			return string.Format("{0}.{1}", text2, frame.GetMethod().get_Name());
		}

		public static string GetLuaString(IntPtr L, int stackPos)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, stackPos);
			string result = null;
			if (luaTypes == LuaTypes.LUA_TSTRING)
			{
				result = LuaDLL.lua_tostring(L, stackPos);
			}
			else if (luaTypes == LuaTypes.LUA_TUSERDATA)
			{
				object luaObject = LuaScriptMgr.GetLuaObject(L, stackPos);
				if (luaObject == null)
				{
					LuaDLL.luaL_argerror(L, stackPos, "string expected, got nil");
					return string.Empty;
				}
				if (luaObject.GetType() == typeof(string))
				{
					result = (string)luaObject;
				}
				else
				{
					result = luaObject.ToString();
				}
			}
			else if (luaTypes == LuaTypes.LUA_TNUMBER)
			{
				result = LuaDLL.lua_tonumber(L, stackPos).ToString();
			}
			else if (luaTypes == LuaTypes.LUA_TBOOLEAN)
			{
				result = LuaDLL.lua_toboolean(L, stackPos).ToString();
			}
			else
			{
				if (luaTypes == LuaTypes.LUA_TNIL)
				{
					return result;
				}
				LuaDLL.lua_getglobal(L, "tostring");
				LuaDLL.lua_pushvalue(L, stackPos);
				LuaDLL.lua_call(L, 1, 1);
				result = LuaDLL.lua_tostring(L, -1);
				LuaDLL.lua_pop(L, 1);
			}
			return result;
		}

		public static string[] GetParamsString(IntPtr L, int stackPos, int count)
		{
			List<string> list = new List<string>();
			while (count > 0)
			{
				string luaString = LuaScriptMgr.GetLuaString(L, stackPos);
				stackPos++;
				count--;
				if (luaString == null)
				{
					LuaDLL.luaL_argerror(L, stackPos, "string expected, got nil");
					break;
				}
				list.Add(luaString);
			}
			return list.ToArray();
		}

		public static string[] GetArrayString(IntPtr L, int stackPos)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, stackPos);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				int num = 1;
				List<string> list = new List<string>();
				LuaDLL.lua_pushvalue(L, stackPos);
				while (true)
				{
					LuaDLL.lua_rawgeti(L, -1, num);
					if (LuaDLL.lua_type(L, -1) == LuaTypes.LUA_TNIL)
					{
						break;
					}
					string luaString = LuaScriptMgr.GetLuaString(L, -1);
					list.Add(luaString);
					LuaDLL.lua_pop(L, 1);
					num++;
				}
				LuaDLL.lua_pop(L, 1);
				return list.ToArray();
			}
			if (luaTypes == LuaTypes.LUA_TUSERDATA)
			{
				string[] netObject = LuaScriptMgr.GetNetObject<string[]>(L, stackPos);
				if (netObject != null)
				{
					return netObject;
				}
			}
			else if (luaTypes == LuaTypes.LUA_TNIL)
			{
				return null;
			}
			LuaDLL.luaL_error(L, string.Format("invalid arguments to method: {0}, pos {1}", LuaScriptMgr.GetErrorFunc(1), stackPos));
			return null;
		}

		public static T[] GetArrayNumber<T>(IntPtr L, int stackPos)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, stackPos);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				int num = 1;
				T t = default(T);
				List<T> list = new List<T>();
				LuaDLL.lua_pushvalue(L, stackPos);
				while (true)
				{
					LuaDLL.lua_rawgeti(L, -1, num);
					luaTypes = LuaDLL.lua_type(L, -1);
					if (luaTypes == LuaTypes.LUA_TNIL)
					{
						break;
					}
					if (luaTypes != LuaTypes.LUA_TNUMBER)
					{
						goto Block_3;
					}
					T t2 = (T)((object)Convert.ChangeType(LuaDLL.lua_tonumber(L, -1), typeof(T)));
					list.Add(t2);
					LuaDLL.lua_pop(L, 1);
					num++;
				}
				LuaDLL.lua_pop(L, 1);
				return list.ToArray();
				Block_3:;
			}
			else if (luaTypes == LuaTypes.LUA_TUSERDATA)
			{
				T[] netObject = LuaScriptMgr.GetNetObject<T[]>(L, stackPos);
				if (netObject != null)
				{
					return netObject;
				}
			}
			else if (luaTypes == LuaTypes.LUA_TNIL)
			{
				return null;
			}
			LuaDLL.luaL_error(L, string.Format("invalid arguments to method: {0}, pos {1}", LuaScriptMgr.GetErrorFunc(1), stackPos));
			return null;
		}

		public static bool[] GetArrayBool(IntPtr L, int stackPos)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, stackPos);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				int num = 1;
				List<bool> list = new List<bool>();
				LuaDLL.lua_pushvalue(L, stackPos);
				while (true)
				{
					LuaDLL.lua_rawgeti(L, -1, num);
					luaTypes = LuaDLL.lua_type(L, -1);
					if (luaTypes == LuaTypes.LUA_TNIL)
					{
						break;
					}
					if (luaTypes != LuaTypes.LUA_TNUMBER)
					{
						goto Block_3;
					}
					bool flag = LuaDLL.lua_toboolean(L, -1);
					list.Add(flag);
					LuaDLL.lua_pop(L, 1);
					num++;
				}
				LuaDLL.lua_pop(L, 1);
				return list.ToArray();
				Block_3:;
			}
			else if (luaTypes == LuaTypes.LUA_TUSERDATA)
			{
				bool[] netObject = LuaScriptMgr.GetNetObject<bool[]>(L, stackPos);
				if (netObject != null)
				{
					return netObject;
				}
			}
			else if (luaTypes == LuaTypes.LUA_TNIL)
			{
				return null;
			}
			LuaDLL.luaL_error(L, string.Format("invalid arguments to method: {0}, pos {1}", LuaScriptMgr.GetErrorFunc(1), stackPos));
			return null;
		}

		public static LuaStringBuffer GetStringBuffer(IntPtr L, int stackPos)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, stackPos);
			if (luaTypes == LuaTypes.LUA_TNIL)
			{
				return null;
			}
			if (luaTypes != LuaTypes.LUA_TSTRING)
			{
				LuaDLL.luaL_typerror(L, stackPos, "string");
				return null;
			}
			int len = 0;
			IntPtr source = LuaDLL.lua_tolstring(L, stackPos, out len);
			return new LuaStringBuffer(source, len);
		}

		public static void SetValueObject(IntPtr L, int pos, object obj)
		{
			LuaScriptMgr.GetTranslator(L).SetValueObject(L, pos, obj);
		}

		public static void CheckArgsCount(IntPtr L, int count)
		{
			int num = LuaDLL.lua_gettop(L);
			if (num != count)
			{
				string message = string.Format("no overload for method '{0}' takes '{1}' arguments", LuaScriptMgr.GetErrorFunc(1), num);
				LuaDLL.luaL_error(L, message);
			}
		}

		public static object GetVarTable(IntPtr L, int stackPos)
		{
			int num = LuaDLL.lua_gettop(L);
			LuaDLL.lua_pushvalue(L, stackPos);
			LuaDLL.lua_pushstring(L, "class");
			LuaDLL.lua_gettable(L, -2);
			object result;
			if (LuaDLL.lua_isnil(L, -1))
			{
				LuaDLL.lua_settop(L, num);
				LuaDLL.lua_pushvalue(L, stackPos);
				result = new LuaTable(LuaDLL.luaL_ref(L, LuaIndexes.LUA_REGISTRYINDEX), LuaScriptMgr.GetTranslator(L).interpreter);
			}
			else
			{
				string text = LuaDLL.lua_tostring(L, -1);
				LuaDLL.lua_settop(L, num);
				stackPos = ((stackPos > 0) ? stackPos : (stackPos + num + 1));
				if (text == "Vector3")
				{
					result = LuaScriptMgr.GetVector3(L, stackPos);
				}
				else if (text == "Vector2")
				{
					result = LuaScriptMgr.GetVector2(L, stackPos);
				}
				else if (text == "Quaternion")
				{
					result = LuaScriptMgr.GetQuaternion(L, stackPos);
				}
				else if (text == "Color")
				{
					result = LuaScriptMgr.GetColor(L, stackPos);
				}
				else if (text == "Vector4")
				{
					result = LuaScriptMgr.GetVector4(L, stackPos);
				}
				else if (text == "Ray")
				{
					result = LuaScriptMgr.GetRay(L, stackPos);
				}
				else if (text == "Bounds")
				{
					result = LuaScriptMgr.GetBounds(L, stackPos);
				}
				else
				{
					LuaDLL.lua_pushvalue(L, stackPos);
					result = new LuaTable(LuaDLL.luaL_ref(L, LuaIndexes.LUA_REGISTRYINDEX), LuaScriptMgr.GetTranslator(L).interpreter);
				}
			}
			return result;
		}

		public static object GetVarObject(IntPtr L, int stackPos)
		{
			switch (LuaDLL.lua_type(L, stackPos))
			{
			case LuaTypes.LUA_TBOOLEAN:
				return LuaDLL.lua_toboolean(L, stackPos);
			case LuaTypes.LUA_TNUMBER:
				return LuaDLL.lua_tonumber(L, stackPos);
			case LuaTypes.LUA_TSTRING:
				return LuaDLL.lua_tostring(L, stackPos);
			case LuaTypes.LUA_TTABLE:
				return LuaScriptMgr.GetVarTable(L, stackPos);
			case LuaTypes.LUA_TFUNCTION:
				LuaDLL.lua_pushvalue(L, stackPos);
				return new LuaFunction(LuaDLL.luaL_ref(L, LuaIndexes.LUA_REGISTRYINDEX), LuaScriptMgr.GetTranslator(L).interpreter);
			case LuaTypes.LUA_TUSERDATA:
			{
				int num = LuaDLL.luanet_rawnetobj(L, stackPos);
				if (num != -1)
				{
					object result = null;
					LuaScriptMgr.GetTranslator(L).objects.TryGetValue(num, ref result);
					return result;
				}
				return null;
			}
			}
			return null;
		}

		[MonoPInvokeCallback(typeof(LuaCSFunction))]
		public static int IndexArray(IntPtr L)
		{
			Array array = LuaScriptMgr.GetLuaObject(L, 1) as Array;
			if (array == null)
			{
				LuaDLL.luaL_error(L, "trying to index an invalid Array reference");
				LuaDLL.lua_pushnil(L);
				return 1;
			}
			LuaTypes luaTypes = LuaDLL.lua_type(L, 2);
			if (luaTypes == LuaTypes.LUA_TNUMBER)
			{
				int num = (int)LuaDLL.lua_tonumber(L, 2);
				if (num >= array.get_Length())
				{
					LuaDLL.luaL_error(L, string.Concat(new object[]
					{
						"array index out of bounds: ",
						num,
						" ",
						array.get_Length()
					}));
					return 0;
				}
				object value = array.GetValue(num);
				if (value == null)
				{
					LuaDLL.luaL_error(L, string.Format("array index {0} is null", num));
					return 0;
				}
				LuaScriptMgr.PushVarObject(L, value);
			}
			else if (luaTypes == LuaTypes.LUA_TSTRING)
			{
				string luaString = LuaScriptMgr.GetLuaString(L, 2);
				if (luaString == "Length")
				{
					LuaScriptMgr.Push(L, array.get_Length());
				}
			}
			return 1;
		}

		[MonoPInvokeCallback(typeof(LuaCSFunction))]
		public static int NewIndexArray(IntPtr L)
		{
			Array array = LuaScriptMgr.GetLuaObject(L, 1) as Array;
			if (array == null)
			{
				LuaDLL.luaL_error(L, "trying to index and invalid object reference");
				return 0;
			}
			int num = (int)LuaScriptMgr.GetNumber(L, 2);
			object obj = LuaScriptMgr.GetVarObject(L, 3);
			Type elementType = array.GetType().GetElementType();
			if (!LuaScriptMgr.CheckType(L, elementType, 3))
			{
				LuaDLL.luaL_error(L, "trying to set object type is not correct");
				return 0;
			}
			obj = Convert.ChangeType(obj, elementType);
			array.SetValue(obj, num);
			return 0;
		}

		public static void DumpStack(IntPtr L)
		{
			int num = LuaDLL.lua_gettop(L);
			int i = 1;
			while (i <= num)
			{
				LuaTypes luaTypes = LuaDLL.lua_type(L, i);
				switch (luaTypes)
				{
				case LuaTypes.LUA_TBOOLEAN:
					Debug.Log(LuaDLL.lua_toboolean(L, i).ToString());
					break;
				case LuaTypes.LUA_TLIGHTUSERDATA:
					goto IL_8D;
				case LuaTypes.LUA_TNUMBER:
					Debug.Log(LuaDLL.lua_tonumber(L, i).ToString());
					break;
				case LuaTypes.LUA_TSTRING:
					Debug.Log(LuaDLL.lua_tostring(L, i));
					break;
				default:
					goto IL_8D;
				}
				IL_84:
				i++;
				continue;
				IL_8D:
				Debug.Log(luaTypes.ToString());
				goto IL_84;
			}
		}

		private static object GetEnumObj(Enum e)
		{
			object obj = null;
			if (!LuaScriptMgr.enumMap.TryGetValue(e, ref obj))
			{
				obj = e;
				LuaScriptMgr.enumMap.Add(e, obj);
			}
			return obj;
		}

		public static void Push(IntPtr L, Enum e)
		{
			LuaScriptMgr mgrFromLuaState = LuaScriptMgr.GetMgrFromLuaState(L);
			ObjectTranslator translator = mgrFromLuaState.lua.translator;
			int weakTableRef = translator.weakTableRef;
			object enumObj = LuaScriptMgr.GetEnumObj(e);
			int num = -1;
			bool flag = translator.objectsBackMap.TryGetValue(enumObj, ref num);
			if (flag)
			{
				if (LuaDLL.tolua_pushudata(L, weakTableRef, num))
				{
					return;
				}
				translator.collectObject(num);
			}
			num = translator.addObject(enumObj, false);
			LuaDLL.tolua_pushnewudata(L, mgrFromLuaState.enumMetaRef, weakTableRef, num);
		}

		public static void Push(IntPtr L, LuaStringBuffer lsb)
		{
			if (lsb != null && lsb.buffer != null)
			{
				LuaDLL.lua_pushlstring(L, lsb.buffer, lsb.buffer.Length);
			}
			else
			{
				LuaDLL.lua_pushnil(L);
			}
		}

		public static LuaScriptMgr GetMgrFromLuaState(IntPtr L)
		{
			return LuaScriptMgr.Instance;
		}

		public static void ThrowLuaException(IntPtr L)
		{
			string text = LuaDLL.lua_tostring(L, -1);
			if (text == null)
			{
				text = "Unknown Lua Error";
			}
			throw new LuaScriptException(text.ToString(), string.Empty);
		}

		public static Vector3 GetVector3(IntPtr L, int stackPos)
		{
			LuaScriptMgr mgrFromLuaState = LuaScriptMgr.GetMgrFromLuaState(L);
			float x = 0f;
			float y = 0f;
			float z = 0f;
			LuaDLL.tolua_getfloat3(L, mgrFromLuaState.unpackVec3, stackPos, ref x, ref y, ref z);
			return new Vector3(x, y, z);
		}

		public static void Push(IntPtr L, Vector3 v3)
		{
			LuaScriptMgr mgrFromLuaState = LuaScriptMgr.GetMgrFromLuaState(L);
			LuaDLL.tolua_pushfloat3(L, mgrFromLuaState.packVec3, v3.x, v3.y, v3.z);
		}

		public static void Push(IntPtr L, Quaternion q)
		{
			LuaScriptMgr mgrFromLuaState = LuaScriptMgr.GetMgrFromLuaState(L);
			LuaDLL.tolua_pushfloat4(L, mgrFromLuaState.packQuat, q.x, q.y, q.z, q.w);
		}

		public static Quaternion GetQuaternion(IntPtr L, int stackPos)
		{
			LuaScriptMgr mgrFromLuaState = LuaScriptMgr.GetMgrFromLuaState(L);
			float x = 0f;
			float y = 0f;
			float z = 0f;
			float w = 1f;
			LuaDLL.tolua_getfloat4(L, mgrFromLuaState.unpackQuat, stackPos, ref x, ref y, ref z, ref w);
			return new Quaternion(x, y, z, w);
		}

		public static Vector2 GetVector2(IntPtr L, int stackPos)
		{
			LuaScriptMgr mgrFromLuaState = LuaScriptMgr.GetMgrFromLuaState(L);
			float x = 0f;
			float y = 0f;
			LuaDLL.tolua_getfloat2(L, mgrFromLuaState.unpackVec2, stackPos, ref x, ref y);
			return new Vector2(x, y);
		}

		public static void Push(IntPtr L, Vector2 v2)
		{
			LuaScriptMgr mgrFromLuaState = LuaScriptMgr.GetMgrFromLuaState(L);
			LuaDLL.tolua_pushfloat2(L, mgrFromLuaState.packVec2, v2.x, v2.y);
		}

		public static Vector4 GetVector4(IntPtr L, int stackPos)
		{
			LuaScriptMgr mgrFromLuaState = LuaScriptMgr.GetMgrFromLuaState(L);
			float x = 0f;
			float y = 0f;
			float z = 0f;
			float w = 0f;
			LuaDLL.tolua_getfloat4(L, mgrFromLuaState.unpackVec4, stackPos, ref x, ref y, ref z, ref w);
			return new Vector4(x, y, z, w);
		}

		public static void Push(IntPtr L, Vector4 v4)
		{
			LuaScriptMgr mgrFromLuaState = LuaScriptMgr.GetMgrFromLuaState(L);
			LuaDLL.tolua_pushfloat4(L, mgrFromLuaState.packVec4, v4.x, v4.y, v4.z, v4.w);
		}

		public static void Push(IntPtr L, RaycastHit hit)
		{
			LuaScriptMgr mgrFromLuaState = LuaScriptMgr.GetMgrFromLuaState(L);
			mgrFromLuaState.packRaycastHit.push(L);
			LuaScriptMgr.Push(L, hit.collider);
			LuaScriptMgr.Push(L, hit.distance);
			LuaScriptMgr.Push(L, hit.normal);
			LuaScriptMgr.Push(L, hit.point);
			LuaScriptMgr.Push(L, hit.rigidbody);
			LuaScriptMgr.Push(L, hit.transform);
			LuaDLL.lua_call(L, 6, -1);
		}

		public static void Push(IntPtr L, Ray ray)
		{
			LuaScriptMgr mgrFromLuaState = LuaScriptMgr.GetMgrFromLuaState(L);
			LuaDLL.lua_getref(L, mgrFromLuaState.packRay);
			LuaDLL.tolua_pushfloat3(L, mgrFromLuaState.packVec3, ray.direction.x, ray.direction.y, ray.direction.z);
			LuaDLL.tolua_pushfloat3(L, mgrFromLuaState.packVec3, ray.origin.x, ray.origin.y, ray.origin.z);
			LuaDLL.lua_call(L, 2, -1);
		}

		public static Ray GetRay(IntPtr L, int stackPos)
		{
			LuaScriptMgr mgrFromLuaState = LuaScriptMgr.GetMgrFromLuaState(L);
			float x = 0f;
			float y = 0f;
			float z = 0f;
			float x2 = 0f;
			float y2 = 0f;
			float z2 = 0f;
			LuaDLL.tolua_getfloat6(L, mgrFromLuaState.unpackRay, stackPos, ref x, ref y, ref z, ref x2, ref y2, ref z2);
			Vector3 origin = new Vector3(x, y, z);
			Vector3 direction = new Vector3(x2, y2, z2);
			return new Ray(origin, direction);
		}

		public static Bounds GetBounds(IntPtr L, int stackPos)
		{
			LuaScriptMgr mgrFromLuaState = LuaScriptMgr.GetMgrFromLuaState(L);
			float x = 0f;
			float y = 0f;
			float z = 0f;
			float x2 = 0f;
			float y2 = 0f;
			float z2 = 0f;
			LuaDLL.tolua_getfloat6(L, mgrFromLuaState.unpackBounds, stackPos, ref x, ref y, ref z, ref x2, ref y2, ref z2);
			Vector3 center = new Vector3(x, y, z);
			Vector3 size = new Vector3(x2, y2, z2);
			return new Bounds(center, size);
		}

		public static Color GetColor(IntPtr L, int stackPos)
		{
			LuaScriptMgr mgrFromLuaState = LuaScriptMgr.GetMgrFromLuaState(L);
			float r = 0f;
			float g = 0f;
			float b = 0f;
			float a = 0f;
			LuaDLL.tolua_getfloat4(L, mgrFromLuaState.unpackColor, stackPos, ref r, ref g, ref b, ref a);
			return new Color(r, g, b, a);
		}

		public static void Push(IntPtr L, Color clr)
		{
			LuaScriptMgr mgrFromLuaState = LuaScriptMgr.GetMgrFromLuaState(L);
			LuaDLL.tolua_pushfloat4(L, mgrFromLuaState.packColor, clr.r, clr.g, clr.b, clr.a);
		}

		public static void Push(IntPtr L, Touch touch)
		{
			LuaScriptMgr mgrFromLuaState = LuaScriptMgr.GetMgrFromLuaState(L);
			mgrFromLuaState.packTouch.push(L);
			LuaDLL.lua_pushinteger(L, touch.fingerId);
			LuaDLL.tolua_pushfloat2(L, mgrFromLuaState.packVec2, touch.position.x, touch.position.y);
			LuaDLL.tolua_pushfloat2(L, mgrFromLuaState.packVec2, touch.rawPosition.x, touch.rawPosition.y);
			LuaDLL.tolua_pushfloat2(L, mgrFromLuaState.packVec2, touch.deltaPosition.x, touch.deltaPosition.y);
			LuaDLL.lua_pushnumber(L, (double)touch.deltaTime);
			LuaDLL.lua_pushinteger(L, touch.tapCount);
			LuaDLL.lua_pushinteger(L, (int)touch.phase);
			LuaDLL.lua_call(L, 7, -1);
		}

		public static void Push(IntPtr L, Bounds bound)
		{
			LuaScriptMgr mgrFromLuaState = LuaScriptMgr.GetMgrFromLuaState(L);
			LuaDLL.lua_getref(L, mgrFromLuaState.packBounds);
			LuaScriptMgr.Push(L, bound.center);
			LuaScriptMgr.Push(L, bound.size);
			LuaDLL.lua_call(L, 2, -1);
		}

		public static void PushTraceBack(IntPtr L)
		{
			if (LuaScriptMgr.traceback == null)
			{
				LuaDLL.lua_getglobal(L, "traceback");
				return;
			}
			LuaScriptMgr.traceback.push();
		}
	}
}
