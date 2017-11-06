using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace com.tencent.pandora
{
	public class ObjectTranslator
	{
		private class CompareObject : IEqualityComparer<object>
		{
			public bool Equals(object x, object y)
			{
				return x == y;
			}

			public int GetHashCode(object obj)
			{
				if (obj != null)
				{
					return obj.GetHashCode();
				}
				return 0;
			}
		}

		internal CheckType typeChecker;

		public readonly Dictionary<int, object> objects = new Dictionary<int, object>();

		public readonly Dictionary<object, int> objectsBackMap = new Dictionary<object, int>(new ObjectTranslator.CompareObject());

		private static Dictionary<Type, int> typeMetaMap = new Dictionary<Type, int>();

		internal LuaState interpreter;

		public MetaFunctions metaFunctions;

		public List<Assembly> assemblies;

		private LuaCSFunction registerTableFunction;

		private LuaCSFunction unregisterTableFunction;

		private LuaCSFunction getMethodSigFunction;

		private LuaCSFunction getConstructorSigFunction;

		private LuaCSFunction importTypeFunction;

		private LuaCSFunction loadAssemblyFunction;

		private LuaCSFunction ctypeFunction;

		private LuaCSFunction enumFromIntFunction;

		internal EventHandlerContainer pendingEvents = new EventHandlerContainer();

		private static List<ObjectTranslator> list = new List<ObjectTranslator>();

		private static int indexTranslator = 0;

		private static Dictionary<Type, bool> valueTypeMap = new Dictionary<Type, bool>();

		private int nextObj;

		public int weakTableRef
		{
			get;
			private set;
		}

		public ObjectTranslator(LuaState interpreter, IntPtr luaState)
		{
			this.interpreter = interpreter;
			this.weakTableRef = -1;
			this.typeChecker = new CheckType(this);
			this.metaFunctions = new MetaFunctions(this);
			this.assemblies = new List<Assembly>();
			this.assemblies.Add(Assembly.GetExecutingAssembly());
			this.importTypeFunction = new LuaCSFunction(ObjectTranslator.importType);
			this.loadAssemblyFunction = new LuaCSFunction(ObjectTranslator.loadAssembly);
			this.registerTableFunction = new LuaCSFunction(ObjectTranslator.registerTable);
			this.unregisterTableFunction = new LuaCSFunction(ObjectTranslator.unregisterTable);
			this.getMethodSigFunction = new LuaCSFunction(ObjectTranslator.getMethodSignature);
			this.getConstructorSigFunction = new LuaCSFunction(ObjectTranslator.getConstructorSignature);
			this.ctypeFunction = new LuaCSFunction(ObjectTranslator.ctype);
			this.enumFromIntFunction = new LuaCSFunction(ObjectTranslator.enumFromInt);
			this.createLuaObjectList(luaState);
			this.createIndexingMetaFunction(luaState);
			this.createBaseClassMetatable(luaState);
			this.createClassMetatable(luaState);
			this.createFunctionMetatable(luaState);
			this.setGlobalFunctions(luaState);
		}

		public static ObjectTranslator FromState(IntPtr luaState)
		{
			LuaDLL.lua_getglobal(luaState, "_translator");
			int num = (int)LuaDLL.lua_tonumber(luaState, -1);
			LuaDLL.lua_pop(luaState, 1);
			return ObjectTranslator.list.get_Item(num);
		}

		public void PushTranslator(IntPtr L)
		{
			ObjectTranslator.list.Add(this);
			LuaDLL.lua_pushnumber(L, (double)ObjectTranslator.indexTranslator);
			LuaDLL.lua_setglobal(L, "_translator");
			ObjectTranslator.indexTranslator++;
		}

		public void Destroy()
		{
			IntPtr l = this.interpreter.L;
			using (Dictionary<Type, int>.Enumerator enumerator = ObjectTranslator.typeMetaMap.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<Type, int> current = enumerator.get_Current();
					int value = current.get_Value();
					LuaDLL.lua_unref(l, value);
				}
			}
			LuaDLL.lua_unref(l, this.weakTableRef);
			ObjectTranslator.typeMetaMap.Clear();
		}

		private void createLuaObjectList(IntPtr luaState)
		{
			LuaDLL.lua_pushstring(luaState, "luaNet_objects");
			LuaDLL.lua_newtable(luaState);
			LuaDLL.lua_pushvalue(luaState, -1);
			this.weakTableRef = LuaDLL.luaL_ref(luaState, LuaIndexes.LUA_REGISTRYINDEX);
			LuaDLL.lua_pushvalue(luaState, -1);
			LuaDLL.lua_setmetatable(luaState, -2);
			LuaDLL.lua_pushstring(luaState, "__mode");
			LuaDLL.lua_pushstring(luaState, "v");
			LuaDLL.lua_settable(luaState, -3);
			LuaDLL.lua_settable(luaState, LuaIndexes.LUA_REGISTRYINDEX);
		}

		private void createIndexingMetaFunction(IntPtr luaState)
		{
			LuaDLL.lua_pushstring(luaState, "luaNet_indexfunction");
			LuaDLL.luaL_dostring(luaState, MetaFunctions.luaIndexFunction);
			LuaDLL.lua_rawset(luaState, LuaIndexes.LUA_REGISTRYINDEX);
		}

		private void createBaseClassMetatable(IntPtr luaState)
		{
			LuaDLL.luaL_newmetatable(luaState, "luaNet_searchbase");
			LuaDLL.lua_pushstring(luaState, "__gc");
			LuaDLL.lua_pushstdcallcfunction(luaState, this.metaFunctions.gcFunction, 0);
			LuaDLL.lua_settable(luaState, -3);
			LuaDLL.lua_pushstring(luaState, "__tostring");
			LuaDLL.lua_pushstdcallcfunction(luaState, this.metaFunctions.toStringFunction, 0);
			LuaDLL.lua_settable(luaState, -3);
			LuaDLL.lua_pushstring(luaState, "__index");
			LuaDLL.lua_pushstdcallcfunction(luaState, this.metaFunctions.baseIndexFunction, 0);
			LuaDLL.lua_settable(luaState, -3);
			LuaDLL.lua_pushstring(luaState, "__newindex");
			LuaDLL.lua_pushstdcallcfunction(luaState, this.metaFunctions.newindexFunction, 0);
			LuaDLL.lua_settable(luaState, -3);
			LuaDLL.lua_settop(luaState, -2);
		}

		private void createClassMetatable(IntPtr luaState)
		{
			LuaDLL.luaL_newmetatable(luaState, "luaNet_class");
			LuaDLL.lua_pushstring(luaState, "__gc");
			LuaDLL.lua_pushstdcallcfunction(luaState, this.metaFunctions.gcFunction, 0);
			LuaDLL.lua_settable(luaState, -3);
			LuaDLL.lua_pushstring(luaState, "__tostring");
			LuaDLL.lua_pushstdcallcfunction(luaState, this.metaFunctions.toStringFunction, 0);
			LuaDLL.lua_settable(luaState, -3);
			LuaDLL.lua_pushstring(luaState, "__index");
			LuaDLL.lua_pushstdcallcfunction(luaState, this.metaFunctions.classIndexFunction, 0);
			LuaDLL.lua_settable(luaState, -3);
			LuaDLL.lua_pushstring(luaState, "__newindex");
			LuaDLL.lua_pushstdcallcfunction(luaState, this.metaFunctions.classNewindexFunction, 0);
			LuaDLL.lua_settable(luaState, -3);
			LuaDLL.lua_pushstring(luaState, "__call");
			LuaDLL.lua_pushstdcallcfunction(luaState, this.metaFunctions.callConstructorFunction, 0);
			LuaDLL.lua_settable(luaState, -3);
			LuaDLL.lua_settop(luaState, -2);
		}

		private void setGlobalFunctions(IntPtr luaState)
		{
			LuaDLL.lua_pushstdcallcfunction(luaState, this.metaFunctions.indexFunction, 0);
			LuaDLL.lua_setglobal(luaState, "get_object_member");
			LuaDLL.lua_pushstdcallcfunction(luaState, this.importTypeFunction, 0);
			LuaDLL.lua_setglobal(luaState, "import_type");
			LuaDLL.lua_pushstdcallcfunction(luaState, this.loadAssemblyFunction, 0);
			LuaDLL.lua_setglobal(luaState, "load_assembly");
			LuaDLL.lua_pushstdcallcfunction(luaState, this.registerTableFunction, 0);
			LuaDLL.lua_setglobal(luaState, "make_object");
			LuaDLL.lua_pushstdcallcfunction(luaState, this.unregisterTableFunction, 0);
			LuaDLL.lua_setglobal(luaState, "free_object");
			LuaDLL.lua_pushstdcallcfunction(luaState, this.getMethodSigFunction, 0);
			LuaDLL.lua_setglobal(luaState, "get_method_bysig");
			LuaDLL.lua_pushstdcallcfunction(luaState, this.getConstructorSigFunction, 0);
			LuaDLL.lua_setglobal(luaState, "get_constructor_bysig");
			LuaDLL.lua_pushstdcallcfunction(luaState, this.ctypeFunction, 0);
			LuaDLL.lua_setglobal(luaState, "ctype");
			LuaDLL.lua_pushstdcallcfunction(luaState, this.enumFromIntFunction, 0);
			LuaDLL.lua_setglobal(luaState, "enum");
		}

		private void createFunctionMetatable(IntPtr luaState)
		{
			LuaDLL.luaL_newmetatable(luaState, "luaNet_function");
			LuaDLL.lua_pushstring(luaState, "__gc");
			LuaDLL.lua_pushstdcallcfunction(luaState, this.metaFunctions.gcFunction, 0);
			LuaDLL.lua_settable(luaState, -3);
			LuaDLL.lua_pushstring(luaState, "__call");
			LuaDLL.lua_pushstdcallcfunction(luaState, this.metaFunctions.execDelegateFunction, 0);
			LuaDLL.lua_settable(luaState, -3);
			LuaDLL.lua_settop(luaState, -2);
		}

		internal void throwError(IntPtr luaState, string message)
		{
			LuaDLL.luaL_error(luaState, message);
		}

		[MonoPInvokeCallback(typeof(LuaCSFunction))]
		public static int loadAssembly(IntPtr luaState)
		{
			ObjectTranslator objectTranslator = ObjectTranslator.FromState(luaState);
			try
			{
				string text = LuaDLL.lua_tostring(luaState, 1);
				Assembly assembly = null;
				try
				{
					assembly = Assembly.Load(text);
				}
				catch (BadImageFormatException)
				{
				}
				if (assembly == null)
				{
					assembly = Assembly.Load(AssemblyName.GetAssemblyName(text));
				}
				if (assembly != null && !objectTranslator.assemblies.Contains(assembly))
				{
					objectTranslator.assemblies.Add(assembly);
				}
			}
			catch (Exception ex)
			{
				objectTranslator.throwError(luaState, ex.get_Message());
			}
			return 0;
		}

		internal Type FindType(string className)
		{
			using (List<Assembly>.Enumerator enumerator = this.assemblies.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Assembly current = enumerator.get_Current();
					Type type = current.GetType(className);
					if (type != null)
					{
						return type;
					}
				}
			}
			return null;
		}

		[MonoPInvokeCallback(typeof(LuaCSFunction))]
		public static int importType(IntPtr luaState)
		{
			ObjectTranslator objectTranslator = ObjectTranslator.FromState(luaState);
			string className = LuaDLL.lua_tostring(luaState, 1);
			Type type = objectTranslator.FindType(className);
			if (type != null)
			{
				objectTranslator.pushType(luaState, type);
			}
			else
			{
				LuaDLL.lua_pushnil(luaState);
			}
			return 1;
		}

		[MonoPInvokeCallback(typeof(LuaCSFunction))]
		public static int registerTable(IntPtr luaState)
		{
			ObjectTranslator objectTranslator = ObjectTranslator.FromState(luaState);
			if (LuaDLL.lua_type(luaState, 1) == LuaTypes.LUA_TTABLE)
			{
				LuaTable table = objectTranslator.getTable(luaState, 1);
				string text = LuaDLL.lua_tostring(luaState, 2);
				if (text != null)
				{
					Type type = objectTranslator.FindType(text);
					if (type != null)
					{
						object classInstance = CodeGeneration.Instance.GetClassInstance(type, table);
						objectTranslator.pushObject(luaState, classInstance, "luaNet_metatable");
						LuaDLL.lua_newtable(luaState);
						LuaDLL.lua_pushstring(luaState, "__index");
						LuaDLL.lua_pushvalue(luaState, -3);
						LuaDLL.lua_settable(luaState, -3);
						LuaDLL.lua_pushstring(luaState, "__newindex");
						LuaDLL.lua_pushvalue(luaState, -3);
						LuaDLL.lua_settable(luaState, -3);
						LuaDLL.lua_setmetatable(luaState, 1);
						LuaDLL.lua_pushstring(luaState, "base");
						int index = objectTranslator.addObject(classInstance);
						objectTranslator.pushNewObject(luaState, classInstance, index, "luaNet_searchbase");
						LuaDLL.lua_rawset(luaState, 1);
					}
					else
					{
						objectTranslator.throwError(luaState, "register_table: can not find superclass '" + text + "'");
					}
				}
				else
				{
					objectTranslator.throwError(luaState, "register_table: superclass name can not be null");
				}
			}
			else
			{
				objectTranslator.throwError(luaState, "register_table: first arg is not a table");
			}
			return 0;
		}

		[MonoPInvokeCallback(typeof(LuaCSFunction))]
		public static int unregisterTable(IntPtr luaState)
		{
			ObjectTranslator objectTranslator = ObjectTranslator.FromState(luaState);
			try
			{
				if (LuaDLL.lua_getmetatable(luaState, 1) != 0)
				{
					LuaDLL.lua_pushstring(luaState, "__index");
					LuaDLL.lua_gettable(luaState, -2);
					object rawNetObject = objectTranslator.getRawNetObject(luaState, -1);
					if (rawNetObject == null)
					{
						objectTranslator.throwError(luaState, "unregister_table: arg is not valid table");
					}
					FieldInfo field = rawNetObject.GetType().GetField("__luaInterface_luaTable");
					if (field == null)
					{
						objectTranslator.throwError(luaState, "unregister_table: arg is not valid table");
					}
					field.SetValue(rawNetObject, null);
					LuaDLL.lua_pushnil(luaState);
					LuaDLL.lua_setmetatable(luaState, 1);
					LuaDLL.lua_pushstring(luaState, "base");
					LuaDLL.lua_pushnil(luaState);
					LuaDLL.lua_settable(luaState, 1);
				}
				else
				{
					objectTranslator.throwError(luaState, "unregister_table: arg is not valid table");
				}
			}
			catch (Exception ex)
			{
				objectTranslator.throwError(luaState, ex.get_Message());
			}
			return 0;
		}

		[MonoPInvokeCallback(typeof(LuaCSFunction))]
		public static int getMethodSignature(IntPtr luaState)
		{
			ObjectTranslator objectTranslator = ObjectTranslator.FromState(luaState);
			int num = LuaDLL.luanet_checkudata(luaState, 1, "luaNet_class");
			IReflect reflect;
			object obj;
			if (num != -1)
			{
				reflect = (IReflect)objectTranslator.objects.get_Item(num);
				obj = null;
			}
			else
			{
				obj = objectTranslator.getRawNetObject(luaState, 1);
				if (obj == null)
				{
					objectTranslator.throwError(luaState, "get_method_bysig: first arg is not type or object reference");
					LuaDLL.lua_pushnil(luaState);
					return 1;
				}
				reflect = obj.GetType();
			}
			string text = LuaDLL.lua_tostring(luaState, 2);
			Type[] array = new Type[LuaDLL.lua_gettop(luaState) - 2];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = objectTranslator.FindType(LuaDLL.lua_tostring(luaState, i + 3));
			}
			try
			{
				MethodInfo method = reflect.GetMethod(text, 93, null, array, null);
				objectTranslator.pushFunction(luaState, new LuaCSFunction(new LuaMethodWrapper(objectTranslator, obj, reflect, method).call));
			}
			catch (Exception ex)
			{
				objectTranslator.throwError(luaState, ex.get_Message());
				LuaDLL.lua_pushnil(luaState);
			}
			return 1;
		}

		[MonoPInvokeCallback(typeof(LuaCSFunction))]
		public static int getConstructorSignature(IntPtr luaState)
		{
			ObjectTranslator objectTranslator = ObjectTranslator.FromState(luaState);
			IReflect reflect = null;
			int num = LuaDLL.luanet_checkudata(luaState, 1, "luaNet_class");
			if (num != -1)
			{
				reflect = (IReflect)objectTranslator.objects.get_Item(num);
			}
			if (reflect == null)
			{
				objectTranslator.throwError(luaState, "get_constructor_bysig: first arg is invalid type reference");
			}
			Type[] array = new Type[LuaDLL.lua_gettop(luaState) - 1];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = objectTranslator.FindType(LuaDLL.lua_tostring(luaState, i + 2));
			}
			try
			{
				ConstructorInfo constructor = reflect.get_UnderlyingSystemType().GetConstructor(array);
				objectTranslator.pushFunction(luaState, new LuaCSFunction(new LuaMethodWrapper(objectTranslator, null, reflect, constructor).call));
			}
			catch (Exception ex)
			{
				objectTranslator.throwError(luaState, ex.get_Message());
				LuaDLL.lua_pushnil(luaState);
			}
			return 1;
		}

		private Type typeOf(IntPtr luaState, int idx)
		{
			int num = LuaDLL.luanet_checkudata(luaState, 1, "luaNet_class");
			if (num == -1)
			{
				return null;
			}
			ProxyType proxyType = (ProxyType)this.objects.get_Item(num);
			return proxyType.UnderlyingSystemType;
		}

		public int pushError(IntPtr luaState, string msg)
		{
			LuaDLL.lua_pushnil(luaState);
			LuaDLL.lua_pushstring(luaState, msg);
			return 2;
		}

		[MonoPInvokeCallback(typeof(LuaCSFunction))]
		public static int ctype(IntPtr luaState)
		{
			ObjectTranslator objectTranslator = ObjectTranslator.FromState(luaState);
			Type type = objectTranslator.typeOf(luaState, 1);
			if (type == null)
			{
				return objectTranslator.pushError(luaState, "not a CLR class");
			}
			objectTranslator.pushObject(luaState, type, "luaNet_metatable");
			return 1;
		}

		[MonoPInvokeCallback(typeof(LuaCSFunction))]
		public static int enumFromInt(IntPtr luaState)
		{
			ObjectTranslator objectTranslator = ObjectTranslator.FromState(luaState);
			Type type = objectTranslator.typeOf(luaState, 1);
			if (type == null || !type.get_IsEnum())
			{
				return objectTranslator.pushError(luaState, "not an enum");
			}
			object o = null;
			LuaTypes luaTypes = LuaDLL.lua_type(luaState, 2);
			if (luaTypes == LuaTypes.LUA_TNUMBER)
			{
				int num = (int)LuaDLL.lua_tonumber(luaState, 2);
				o = Enum.ToObject(type, num);
			}
			else
			{
				if (luaTypes != LuaTypes.LUA_TSTRING)
				{
					return objectTranslator.pushError(luaState, "second argument must be a integer or a string");
				}
				string text = LuaDLL.lua_tostring(luaState, 2);
				string text2 = null;
				try
				{
					o = Enum.Parse(type, text);
				}
				catch (ArgumentException ex)
				{
					text2 = ex.get_Message();
				}
				if (text2 != null)
				{
					return objectTranslator.pushError(luaState, text2);
				}
			}
			objectTranslator.pushObject(luaState, o, "luaNet_metatable");
			return 1;
		}

		internal void pushType(IntPtr luaState, Type t)
		{
			this.pushObject(luaState, new ProxyType(t), "luaNet_class");
		}

		internal void pushFunction(IntPtr luaState, LuaCSFunction func)
		{
			this.pushObject(luaState, func, "luaNet_function");
		}

		private bool IsValueType(Type t)
		{
			bool flag = false;
			if (!ObjectTranslator.valueTypeMap.TryGetValue(t, ref flag))
			{
				flag = t.get_IsValueType();
				ObjectTranslator.valueTypeMap.Add(t, flag);
			}
			return flag;
		}

		public void pushObject(IntPtr luaState, object o, string metatable)
		{
			if (o == null)
			{
				LuaDLL.lua_pushnil(luaState);
				return;
			}
			int num = -1;
			bool isValueType = o.GetType().get_IsValueType();
			if (!isValueType && this.objectsBackMap.TryGetValue(o, ref num))
			{
				if (LuaDLL.tolua_pushudata(luaState, this.weakTableRef, num))
				{
					return;
				}
				this.collectObject(o, num);
			}
			num = this.addObject(o, isValueType);
			this.pushNewObject(luaState, o, num, metatable);
		}

		private static void PushMetaTable(IntPtr L, Type t)
		{
			int num = -1;
			if (!ObjectTranslator.typeMetaMap.TryGetValue(t, ref num))
			{
				LuaDLL.luaL_getmetatable(L, t.get_AssemblyQualifiedName());
				if (!LuaDLL.lua_isnil(L, -1))
				{
					LuaDLL.lua_pushvalue(L, -1);
					num = LuaDLL.luaL_ref(L, LuaIndexes.LUA_REGISTRYINDEX);
					ObjectTranslator.typeMetaMap.Add(t, num);
				}
			}
			else
			{
				LuaDLL.lua_getref(L, num);
			}
		}

		private void pushNewObject(IntPtr luaState, object o, int index, string metatable)
		{
			LuaDLL.lua_getref(luaState, this.weakTableRef);
			LuaDLL.luanet_newudata(luaState, index);
			if (metatable == "luaNet_metatable")
			{
				Type type = o.GetType();
				ObjectTranslator.PushMetaTable(luaState, o.GetType());
				if (LuaDLL.lua_isnil(luaState, -1))
				{
					string assemblyQualifiedName = type.get_AssemblyQualifiedName();
					Debug.LogWarning("Create not wrap ulua type:" + assemblyQualifiedName);
					LuaDLL.lua_settop(luaState, -2);
					LuaDLL.luaL_newmetatable(luaState, assemblyQualifiedName);
					LuaDLL.lua_pushstring(luaState, "cache");
					LuaDLL.lua_newtable(luaState);
					LuaDLL.lua_rawset(luaState, -3);
					LuaDLL.lua_pushlightuserdata(luaState, LuaDLL.luanet_gettag());
					LuaDLL.lua_pushnumber(luaState, 1.0);
					LuaDLL.lua_rawset(luaState, -3);
					LuaDLL.lua_pushstring(luaState, "__index");
					LuaDLL.lua_pushstring(luaState, "luaNet_indexfunction");
					LuaDLL.lua_rawget(luaState, LuaIndexes.LUA_REGISTRYINDEX);
					LuaDLL.lua_rawset(luaState, -3);
					LuaDLL.lua_pushstring(luaState, "__gc");
					LuaDLL.lua_pushstdcallcfunction(luaState, this.metaFunctions.gcFunction, 0);
					LuaDLL.lua_rawset(luaState, -3);
					LuaDLL.lua_pushstring(luaState, "__tostring");
					LuaDLL.lua_pushstdcallcfunction(luaState, this.metaFunctions.toStringFunction, 0);
					LuaDLL.lua_rawset(luaState, -3);
					LuaDLL.lua_pushstring(luaState, "__newindex");
					LuaDLL.lua_pushstdcallcfunction(luaState, this.metaFunctions.newindexFunction, 0);
					LuaDLL.lua_rawset(luaState, -3);
				}
			}
			else
			{
				LuaDLL.luaL_getmetatable(luaState, metatable);
			}
			LuaDLL.lua_setmetatable(luaState, -2);
			LuaDLL.lua_pushvalue(luaState, -1);
			LuaDLL.lua_rawseti(luaState, -3, index);
			LuaDLL.lua_remove(luaState, -2);
		}

		public void PushNewValueObject(IntPtr luaState, object o, int index)
		{
			LuaDLL.luanet_newudata(luaState, index);
			Type type = o.GetType();
			ObjectTranslator.PushMetaTable(luaState, o.GetType());
			if (LuaDLL.lua_isnil(luaState, -1))
			{
				string assemblyQualifiedName = type.get_AssemblyQualifiedName();
				Debug.LogWarning("Create not wrap ulua type:" + assemblyQualifiedName);
				LuaDLL.lua_settop(luaState, -2);
				LuaDLL.luaL_newmetatable(luaState, assemblyQualifiedName);
				LuaDLL.lua_pushstring(luaState, "cache");
				LuaDLL.lua_newtable(luaState);
				LuaDLL.lua_rawset(luaState, -3);
				LuaDLL.lua_pushlightuserdata(luaState, LuaDLL.luanet_gettag());
				LuaDLL.lua_pushnumber(luaState, 1.0);
				LuaDLL.lua_rawset(luaState, -3);
				LuaDLL.lua_pushstring(luaState, "__index");
				LuaDLL.lua_pushstring(luaState, "luaNet_indexfunction");
				LuaDLL.lua_rawget(luaState, LuaIndexes.LUA_REGISTRYINDEX);
				LuaDLL.lua_rawset(luaState, -3);
				LuaDLL.lua_pushstring(luaState, "__gc");
				LuaDLL.lua_pushstdcallcfunction(luaState, this.metaFunctions.gcFunction, 0);
				LuaDLL.lua_rawset(luaState, -3);
				LuaDLL.lua_pushstring(luaState, "__tostring");
				LuaDLL.lua_pushstdcallcfunction(luaState, this.metaFunctions.toStringFunction, 0);
				LuaDLL.lua_rawset(luaState, -3);
				LuaDLL.lua_pushstring(luaState, "__newindex");
				LuaDLL.lua_pushstdcallcfunction(luaState, this.metaFunctions.newindexFunction, 0);
				LuaDLL.lua_rawset(luaState, -3);
			}
			LuaDLL.lua_setmetatable(luaState, -2);
		}

		internal object getAsType(IntPtr luaState, int stackPos, Type paramType)
		{
			ExtractValue extractValue = this.typeChecker.checkType(luaState, stackPos, paramType);
			if (extractValue != null)
			{
				return extractValue(luaState, stackPos);
			}
			return null;
		}

		internal void collectObject(int udata)
		{
			object obj;
			bool flag = this.objects.TryGetValue(udata, ref obj);
			if (flag)
			{
				this.objects.Remove(udata);
				if (obj != null && !obj.GetType().get_IsValueType())
				{
					this.objectsBackMap.Remove(obj);
				}
			}
		}

		private void collectObject(object o, int udata)
		{
			this.objectsBackMap.Remove(o);
			this.objects.Remove(udata);
		}

		public int addObject(object obj)
		{
			int num = this.nextObj++;
			this.objects.set_Item(num, obj);
			if (!obj.GetType().get_IsValueType())
			{
				this.objectsBackMap.set_Item(obj, num);
			}
			return num;
		}

		public int addObject(object obj, bool isValueType)
		{
			int num = this.nextObj++;
			this.objects.set_Item(num, obj);
			if (!isValueType)
			{
				this.objectsBackMap.set_Item(obj, num);
			}
			return num;
		}

		public object getObject(IntPtr luaState, int index)
		{
			return LuaScriptMgr.GetVarObject(luaState, index);
		}

		internal LuaTable getTable(IntPtr luaState, int index)
		{
			LuaDLL.lua_pushvalue(luaState, index);
			return new LuaTable(LuaDLL.luaL_ref(luaState, LuaIndexes.LUA_REGISTRYINDEX), this.interpreter);
		}

		internal LuaFunction getFunction(IntPtr luaState, int index)
		{
			LuaDLL.lua_pushvalue(luaState, index);
			return new LuaFunction(LuaDLL.luaL_ref(luaState, LuaIndexes.LUA_REGISTRYINDEX), this.interpreter);
		}

		internal object getNetObject(IntPtr luaState, int index)
		{
			int num = LuaDLL.luanet_tonetobject(luaState, index);
			if (num != -1)
			{
				return this.objects.get_Item(num);
			}
			return null;
		}

		internal object getRawNetObject(IntPtr luaState, int index)
		{
			int num = LuaDLL.luanet_rawnetobj(luaState, index);
			object result = null;
			this.objects.TryGetValue(num, ref result);
			return result;
		}

		public void SetValueObject(IntPtr luaState, int stackPos, object obj)
		{
			int num = LuaDLL.luanet_rawnetobj(luaState, stackPos);
			if (num != -1)
			{
				this.objects.set_Item(num, obj);
			}
		}

		internal int returnValues(IntPtr luaState, object[] returnValues)
		{
			if (LuaDLL.lua_checkstack(luaState, returnValues.Length + 5))
			{
				for (int i = 0; i < returnValues.Length; i++)
				{
					this.push(luaState, returnValues[i]);
				}
				return returnValues.Length;
			}
			return 0;
		}

		internal object[] popValues(IntPtr luaState, int oldTop)
		{
			int num = LuaDLL.lua_gettop(luaState);
			if (oldTop == num)
			{
				return null;
			}
			List<object> list = new List<object>();
			for (int i = oldTop + 1; i <= num; i++)
			{
				list.Add(this.getObject(luaState, i));
			}
			LuaDLL.lua_settop(luaState, oldTop);
			return list.ToArray();
		}

		internal object[] popValues(IntPtr luaState, int oldTop, Type[] popTypes)
		{
			int num = LuaDLL.lua_gettop(luaState);
			if (oldTop == num)
			{
				return null;
			}
			List<object> list = new List<object>();
			int num2;
			if (popTypes[0] == typeof(void))
			{
				num2 = 1;
			}
			else
			{
				num2 = 0;
			}
			for (int i = oldTop + 1; i <= num; i++)
			{
				list.Add(this.getAsType(luaState, i, popTypes[num2]));
				num2++;
			}
			LuaDLL.lua_settop(luaState, oldTop);
			return list.ToArray();
		}

		private static bool IsILua(object o)
		{
			if (o is ILuaGeneratedType)
			{
				Type type = o.GetType();
				return type.GetInterface("ILuaGeneratedType") != null;
			}
			return false;
		}

		internal void push(IntPtr luaState, object o)
		{
			LuaScriptMgr.PushVarObject(luaState, o);
		}

		internal void PushValueResult(IntPtr lua, object o)
		{
			int index = this.addObject(o, true);
			this.PushNewValueObject(lua, o, index);
		}

		internal bool matchParameters(IntPtr luaState, MethodBase method, ref MethodCache methodCache)
		{
			return this.metaFunctions.matchParameters(luaState, method, ref methodCache);
		}

		internal Array tableToArray(object luaParamValue, Type paramArrayType)
		{
			return this.metaFunctions.TableToArray(luaParamValue, paramArrayType);
		}
	}
}
