using com.tencent.pandora;
using System;
using System.Globalization;
using System.Reflection;

public class TypeWrap
{
	private static Type classType = typeof(Type);

	public static void Register(IntPtr L)
	{
		LuaMethod[] regs = new LuaMethod[]
		{
			new LuaMethod("Equals", new LuaCSFunction(TypeWrap.Equals)),
			new LuaMethod("GetType", new LuaCSFunction(TypeWrap.GetType)),
			new LuaMethod("GetTypeArray", new LuaCSFunction(TypeWrap.GetTypeArray)),
			new LuaMethod("GetTypeCode", new LuaCSFunction(TypeWrap.GetTypeCode)),
			new LuaMethod("GetTypeFromCLSID", new LuaCSFunction(TypeWrap.GetTypeFromCLSID)),
			new LuaMethod("GetTypeFromHandle", new LuaCSFunction(TypeWrap.GetTypeFromHandle)),
			new LuaMethod("GetTypeFromProgID", new LuaCSFunction(TypeWrap.GetTypeFromProgID)),
			new LuaMethod("GetTypeHandle", new LuaCSFunction(TypeWrap.GetTypeHandle)),
			new LuaMethod("IsSubclassOf", new LuaCSFunction(TypeWrap.IsSubclassOf)),
			new LuaMethod("FindInterfaces", new LuaCSFunction(TypeWrap.FindInterfaces)),
			new LuaMethod("GetInterface", new LuaCSFunction(TypeWrap.GetInterface)),
			new LuaMethod("GetInterfaceMap", new LuaCSFunction(TypeWrap.GetInterfaceMap)),
			new LuaMethod("GetInterfaces", new LuaCSFunction(TypeWrap.GetInterfaces)),
			new LuaMethod("IsAssignableFrom", new LuaCSFunction(TypeWrap.IsAssignableFrom)),
			new LuaMethod("IsInstanceOfType", new LuaCSFunction(TypeWrap.IsInstanceOfType)),
			new LuaMethod("GetArrayRank", new LuaCSFunction(TypeWrap.GetArrayRank)),
			new LuaMethod("GetElementType", new LuaCSFunction(TypeWrap.GetElementType)),
			new LuaMethod("GetEvent", new LuaCSFunction(TypeWrap.GetEvent)),
			new LuaMethod("GetEvents", new LuaCSFunction(TypeWrap.GetEvents)),
			new LuaMethod("GetField", new LuaCSFunction(TypeWrap.GetField)),
			new LuaMethod("GetFields", new LuaCSFunction(TypeWrap.GetFields)),
			new LuaMethod("GetHashCode", new LuaCSFunction(TypeWrap.GetHashCode)),
			new LuaMethod("GetMember", new LuaCSFunction(TypeWrap.GetMember)),
			new LuaMethod("GetMembers", new LuaCSFunction(TypeWrap.GetMembers)),
			new LuaMethod("GetMethod", new LuaCSFunction(TypeWrap.GetMethod)),
			new LuaMethod("GetMethods", new LuaCSFunction(TypeWrap.GetMethods)),
			new LuaMethod("GetNestedType", new LuaCSFunction(TypeWrap.GetNestedType)),
			new LuaMethod("GetNestedTypes", new LuaCSFunction(TypeWrap.GetNestedTypes)),
			new LuaMethod("GetProperties", new LuaCSFunction(TypeWrap.GetProperties)),
			new LuaMethod("GetProperty", new LuaCSFunction(TypeWrap.GetProperty)),
			new LuaMethod("GetConstructor", new LuaCSFunction(TypeWrap.GetConstructor)),
			new LuaMethod("GetConstructors", new LuaCSFunction(TypeWrap.GetConstructors)),
			new LuaMethod("GetDefaultMembers", new LuaCSFunction(TypeWrap.GetDefaultMembers)),
			new LuaMethod("FindMembers", new LuaCSFunction(TypeWrap.FindMembers)),
			new LuaMethod("InvokeMember", new LuaCSFunction(TypeWrap.InvokeMember)),
			new LuaMethod("ToString", new LuaCSFunction(TypeWrap.ToString)),
			new LuaMethod("GetGenericArguments", new LuaCSFunction(TypeWrap.GetGenericArguments)),
			new LuaMethod("GetGenericTypeDefinition", new LuaCSFunction(TypeWrap.GetGenericTypeDefinition)),
			new LuaMethod("MakeGenericType", new LuaCSFunction(TypeWrap.MakeGenericType)),
			new LuaMethod("GetGenericParameterConstraints", new LuaCSFunction(TypeWrap.GetGenericParameterConstraints)),
			new LuaMethod("MakeArrayType", new LuaCSFunction(TypeWrap.MakeArrayType)),
			new LuaMethod("MakeByRefType", new LuaCSFunction(TypeWrap.MakeByRefType)),
			new LuaMethod("MakePointerType", new LuaCSFunction(TypeWrap.MakePointerType)),
			new LuaMethod("ReflectionOnlyGetType", new LuaCSFunction(TypeWrap.ReflectionOnlyGetType)),
			new LuaMethod("New", new LuaCSFunction(TypeWrap._CreateType)),
			new LuaMethod("GetClassType", new LuaCSFunction(TypeWrap.GetClassType)),
			new LuaMethod("__tostring", new LuaCSFunction(TypeWrap.Lua_ToString))
		};
		LuaField[] fields = new LuaField[]
		{
			new LuaField("Delimiter", new LuaCSFunction(TypeWrap.get_Delimiter), null),
			new LuaField("EmptyTypes", new LuaCSFunction(TypeWrap.get_EmptyTypes), null),
			new LuaField("FilterAttribute", new LuaCSFunction(TypeWrap.get_FilterAttribute), null),
			new LuaField("FilterName", new LuaCSFunction(TypeWrap.get_FilterName), null),
			new LuaField("FilterNameIgnoreCase", new LuaCSFunction(TypeWrap.get_FilterNameIgnoreCase), null),
			new LuaField("Missing", new LuaCSFunction(TypeWrap.get_Missing), null),
			new LuaField("Assembly", new LuaCSFunction(TypeWrap.get_Assembly), null),
			new LuaField("AssemblyQualifiedName", new LuaCSFunction(TypeWrap.get_AssemblyQualifiedName), null),
			new LuaField("Attributes", new LuaCSFunction(TypeWrap.get_Attributes), null),
			new LuaField("BaseType", new LuaCSFunction(TypeWrap.get_BaseType), null),
			new LuaField("DeclaringType", new LuaCSFunction(TypeWrap.get_DeclaringType), null),
			new LuaField("DefaultBinder", new LuaCSFunction(TypeWrap.get_DefaultBinder), null),
			new LuaField("FullName", new LuaCSFunction(TypeWrap.get_FullName), null),
			new LuaField("GUID", new LuaCSFunction(TypeWrap.get_GUID), null),
			new LuaField("HasElementType", new LuaCSFunction(TypeWrap.get_HasElementType), null),
			new LuaField("IsAbstract", new LuaCSFunction(TypeWrap.get_IsAbstract), null),
			new LuaField("IsAnsiClass", new LuaCSFunction(TypeWrap.get_IsAnsiClass), null),
			new LuaField("IsArray", new LuaCSFunction(TypeWrap.get_IsArray), null),
			new LuaField("IsAutoClass", new LuaCSFunction(TypeWrap.get_IsAutoClass), null),
			new LuaField("IsAutoLayout", new LuaCSFunction(TypeWrap.get_IsAutoLayout), null),
			new LuaField("IsByRef", new LuaCSFunction(TypeWrap.get_IsByRef), null),
			new LuaField("IsClass", new LuaCSFunction(TypeWrap.get_IsClass), null),
			new LuaField("IsCOMObject", new LuaCSFunction(TypeWrap.get_IsCOMObject), null),
			new LuaField("IsContextful", new LuaCSFunction(TypeWrap.get_IsContextful), null),
			new LuaField("IsEnum", new LuaCSFunction(TypeWrap.get_IsEnum), null),
			new LuaField("IsExplicitLayout", new LuaCSFunction(TypeWrap.get_IsExplicitLayout), null),
			new LuaField("IsImport", new LuaCSFunction(TypeWrap.get_IsImport), null),
			new LuaField("IsInterface", new LuaCSFunction(TypeWrap.get_IsInterface), null),
			new LuaField("IsLayoutSequential", new LuaCSFunction(TypeWrap.get_IsLayoutSequential), null),
			new LuaField("IsMarshalByRef", new LuaCSFunction(TypeWrap.get_IsMarshalByRef), null),
			new LuaField("IsNestedAssembly", new LuaCSFunction(TypeWrap.get_IsNestedAssembly), null),
			new LuaField("IsNestedFamANDAssem", new LuaCSFunction(TypeWrap.get_IsNestedFamANDAssem), null),
			new LuaField("IsNestedFamily", new LuaCSFunction(TypeWrap.get_IsNestedFamily), null),
			new LuaField("IsNestedFamORAssem", new LuaCSFunction(TypeWrap.get_IsNestedFamORAssem), null),
			new LuaField("IsNestedPrivate", new LuaCSFunction(TypeWrap.get_IsNestedPrivate), null),
			new LuaField("IsNestedPublic", new LuaCSFunction(TypeWrap.get_IsNestedPublic), null),
			new LuaField("IsNotPublic", new LuaCSFunction(TypeWrap.get_IsNotPublic), null),
			new LuaField("IsPointer", new LuaCSFunction(TypeWrap.get_IsPointer), null),
			new LuaField("IsPrimitive", new LuaCSFunction(TypeWrap.get_IsPrimitive), null),
			new LuaField("IsPublic", new LuaCSFunction(TypeWrap.get_IsPublic), null),
			new LuaField("IsSealed", new LuaCSFunction(TypeWrap.get_IsSealed), null),
			new LuaField("IsSerializable", new LuaCSFunction(TypeWrap.get_IsSerializable), null),
			new LuaField("IsSpecialName", new LuaCSFunction(TypeWrap.get_IsSpecialName), null),
			new LuaField("IsUnicodeClass", new LuaCSFunction(TypeWrap.get_IsUnicodeClass), null),
			new LuaField("IsValueType", new LuaCSFunction(TypeWrap.get_IsValueType), null),
			new LuaField("MemberType", new LuaCSFunction(TypeWrap.get_MemberType), null),
			new LuaField("Module", new LuaCSFunction(TypeWrap.get_Module), null),
			new LuaField("Namespace", new LuaCSFunction(TypeWrap.get_Namespace), null),
			new LuaField("ReflectedType", new LuaCSFunction(TypeWrap.get_ReflectedType), null),
			new LuaField("TypeHandle", new LuaCSFunction(TypeWrap.get_TypeHandle), null),
			new LuaField("TypeInitializer", new LuaCSFunction(TypeWrap.get_TypeInitializer), null),
			new LuaField("UnderlyingSystemType", new LuaCSFunction(TypeWrap.get_UnderlyingSystemType), null),
			new LuaField("ContainsGenericParameters", new LuaCSFunction(TypeWrap.get_ContainsGenericParameters), null),
			new LuaField("IsGenericTypeDefinition", new LuaCSFunction(TypeWrap.get_IsGenericTypeDefinition), null),
			new LuaField("IsGenericType", new LuaCSFunction(TypeWrap.get_IsGenericType), null),
			new LuaField("IsGenericParameter", new LuaCSFunction(TypeWrap.get_IsGenericParameter), null),
			new LuaField("IsNested", new LuaCSFunction(TypeWrap.get_IsNested), null),
			new LuaField("IsVisible", new LuaCSFunction(TypeWrap.get_IsVisible), null),
			new LuaField("GenericParameterPosition", new LuaCSFunction(TypeWrap.get_GenericParameterPosition), null),
			new LuaField("GenericParameterAttributes", new LuaCSFunction(TypeWrap.get_GenericParameterAttributes), null),
			new LuaField("DeclaringMethod", new LuaCSFunction(TypeWrap.get_DeclaringMethod), null),
			new LuaField("StructLayoutAttribute", new LuaCSFunction(TypeWrap.get_StructLayoutAttribute), null)
		};
		LuaScriptMgr.RegisterLib(L, "System.Type", typeof(Type), regs, fields, typeof(object));
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int _CreateType(IntPtr L)
	{
		LuaDLL.luaL_error(L, "Type class does not have a constructor function");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, TypeWrap.classType);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_Delimiter(IntPtr L)
	{
		LuaScriptMgr.Push(L, Type.Delimiter);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_EmptyTypes(IntPtr L)
	{
		LuaScriptMgr.PushArray(L, Type.EmptyTypes);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_FilterAttribute(IntPtr L)
	{
		LuaScriptMgr.Push(L, Type.FilterAttribute);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_FilterName(IntPtr L)
	{
		LuaScriptMgr.Push(L, Type.FilterName);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_FilterNameIgnoreCase(IntPtr L)
	{
		LuaScriptMgr.Push(L, Type.FilterNameIgnoreCase);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_Missing(IntPtr L)
	{
		LuaScriptMgr.PushVarObject(L, Type.Missing);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_Assembly(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Type type = (Type)luaObject;
		if (type == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name Assembly");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index Assembly on a nil value");
			}
		}
		LuaScriptMgr.PushObject(L, type.get_Assembly());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_AssemblyQualifiedName(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Type type = (Type)luaObject;
		if (type == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name AssemblyQualifiedName");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index AssemblyQualifiedName on a nil value");
			}
		}
		LuaScriptMgr.Push(L, type.get_AssemblyQualifiedName());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_Attributes(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Type type = (Type)luaObject;
		if (type == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name Attributes");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index Attributes on a nil value");
			}
		}
		LuaScriptMgr.Push(L, type.get_Attributes());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_BaseType(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Type type = (Type)luaObject;
		if (type == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name BaseType");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index BaseType on a nil value");
			}
		}
		LuaScriptMgr.Push(L, type.get_BaseType());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_DeclaringType(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Type type = (Type)luaObject;
		if (type == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name DeclaringType");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index DeclaringType on a nil value");
			}
		}
		LuaScriptMgr.Push(L, type.get_DeclaringType());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_DefaultBinder(IntPtr L)
	{
		LuaScriptMgr.PushObject(L, Type.get_DefaultBinder());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_FullName(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Type type = (Type)luaObject;
		if (type == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name FullName");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index FullName on a nil value");
			}
		}
		LuaScriptMgr.Push(L, type.get_FullName());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_GUID(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Type type = (Type)luaObject;
		if (type == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name GUID");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index GUID on a nil value");
			}
		}
		LuaScriptMgr.PushValue(L, type.get_GUID());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_HasElementType(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Type type = (Type)luaObject;
		if (type == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name HasElementType");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index HasElementType on a nil value");
			}
		}
		LuaScriptMgr.Push(L, type.get_HasElementType());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_IsAbstract(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Type type = (Type)luaObject;
		if (type == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name IsAbstract");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index IsAbstract on a nil value");
			}
		}
		LuaScriptMgr.Push(L, type.get_IsAbstract());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_IsAnsiClass(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Type type = (Type)luaObject;
		if (type == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name IsAnsiClass");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index IsAnsiClass on a nil value");
			}
		}
		LuaScriptMgr.Push(L, type.get_IsAnsiClass());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_IsArray(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Type type = (Type)luaObject;
		if (type == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name IsArray");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index IsArray on a nil value");
			}
		}
		LuaScriptMgr.Push(L, type.get_IsArray());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_IsAutoClass(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Type type = (Type)luaObject;
		if (type == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name IsAutoClass");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index IsAutoClass on a nil value");
			}
		}
		LuaScriptMgr.Push(L, type.get_IsAutoClass());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_IsAutoLayout(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Type type = (Type)luaObject;
		if (type == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name IsAutoLayout");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index IsAutoLayout on a nil value");
			}
		}
		LuaScriptMgr.Push(L, type.get_IsAutoLayout());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_IsByRef(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Type type = (Type)luaObject;
		if (type == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name IsByRef");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index IsByRef on a nil value");
			}
		}
		LuaScriptMgr.Push(L, type.get_IsByRef());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_IsClass(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Type type = (Type)luaObject;
		if (type == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name IsClass");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index IsClass on a nil value");
			}
		}
		LuaScriptMgr.Push(L, type.get_IsClass());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_IsCOMObject(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Type type = (Type)luaObject;
		if (type == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name IsCOMObject");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index IsCOMObject on a nil value");
			}
		}
		LuaScriptMgr.Push(L, type.get_IsCOMObject());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_IsContextful(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Type type = (Type)luaObject;
		if (type == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name IsContextful");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index IsContextful on a nil value");
			}
		}
		LuaScriptMgr.Push(L, type.get_IsContextful());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_IsEnum(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Type type = (Type)luaObject;
		if (type == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name IsEnum");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index IsEnum on a nil value");
			}
		}
		LuaScriptMgr.Push(L, type.get_IsEnum());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_IsExplicitLayout(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Type type = (Type)luaObject;
		if (type == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name IsExplicitLayout");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index IsExplicitLayout on a nil value");
			}
		}
		LuaScriptMgr.Push(L, type.get_IsExplicitLayout());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_IsImport(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Type type = (Type)luaObject;
		if (type == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name IsImport");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index IsImport on a nil value");
			}
		}
		LuaScriptMgr.Push(L, type.get_IsImport());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_IsInterface(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Type type = (Type)luaObject;
		if (type == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name IsInterface");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index IsInterface on a nil value");
			}
		}
		LuaScriptMgr.Push(L, type.get_IsInterface());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_IsLayoutSequential(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Type type = (Type)luaObject;
		if (type == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name IsLayoutSequential");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index IsLayoutSequential on a nil value");
			}
		}
		LuaScriptMgr.Push(L, type.get_IsLayoutSequential());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_IsMarshalByRef(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Type type = (Type)luaObject;
		if (type == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name IsMarshalByRef");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index IsMarshalByRef on a nil value");
			}
		}
		LuaScriptMgr.Push(L, type.get_IsMarshalByRef());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_IsNestedAssembly(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Type type = (Type)luaObject;
		if (type == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name IsNestedAssembly");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index IsNestedAssembly on a nil value");
			}
		}
		LuaScriptMgr.Push(L, type.get_IsNestedAssembly());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_IsNestedFamANDAssem(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Type type = (Type)luaObject;
		if (type == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name IsNestedFamANDAssem");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index IsNestedFamANDAssem on a nil value");
			}
		}
		LuaScriptMgr.Push(L, type.get_IsNestedFamANDAssem());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_IsNestedFamily(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Type type = (Type)luaObject;
		if (type == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name IsNestedFamily");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index IsNestedFamily on a nil value");
			}
		}
		LuaScriptMgr.Push(L, type.get_IsNestedFamily());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_IsNestedFamORAssem(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Type type = (Type)luaObject;
		if (type == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name IsNestedFamORAssem");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index IsNestedFamORAssem on a nil value");
			}
		}
		LuaScriptMgr.Push(L, type.get_IsNestedFamORAssem());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_IsNestedPrivate(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Type type = (Type)luaObject;
		if (type == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name IsNestedPrivate");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index IsNestedPrivate on a nil value");
			}
		}
		LuaScriptMgr.Push(L, type.get_IsNestedPrivate());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_IsNestedPublic(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Type type = (Type)luaObject;
		if (type == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name IsNestedPublic");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index IsNestedPublic on a nil value");
			}
		}
		LuaScriptMgr.Push(L, type.get_IsNestedPublic());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_IsNotPublic(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Type type = (Type)luaObject;
		if (type == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name IsNotPublic");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index IsNotPublic on a nil value");
			}
		}
		LuaScriptMgr.Push(L, type.get_IsNotPublic());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_IsPointer(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Type type = (Type)luaObject;
		if (type == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name IsPointer");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index IsPointer on a nil value");
			}
		}
		LuaScriptMgr.Push(L, type.get_IsPointer());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_IsPrimitive(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Type type = (Type)luaObject;
		if (type == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name IsPrimitive");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index IsPrimitive on a nil value");
			}
		}
		LuaScriptMgr.Push(L, type.get_IsPrimitive());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_IsPublic(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Type type = (Type)luaObject;
		if (type == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name IsPublic");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index IsPublic on a nil value");
			}
		}
		LuaScriptMgr.Push(L, type.get_IsPublic());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_IsSealed(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Type type = (Type)luaObject;
		if (type == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name IsSealed");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index IsSealed on a nil value");
			}
		}
		LuaScriptMgr.Push(L, type.get_IsSealed());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_IsSerializable(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Type type = (Type)luaObject;
		if (type == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name IsSerializable");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index IsSerializable on a nil value");
			}
		}
		LuaScriptMgr.Push(L, type.get_IsSerializable());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_IsSpecialName(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Type type = (Type)luaObject;
		if (type == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name IsSpecialName");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index IsSpecialName on a nil value");
			}
		}
		LuaScriptMgr.Push(L, type.get_IsSpecialName());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_IsUnicodeClass(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Type type = (Type)luaObject;
		if (type == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name IsUnicodeClass");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index IsUnicodeClass on a nil value");
			}
		}
		LuaScriptMgr.Push(L, type.get_IsUnicodeClass());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_IsValueType(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Type type = (Type)luaObject;
		if (type == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name IsValueType");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index IsValueType on a nil value");
			}
		}
		LuaScriptMgr.Push(L, type.get_IsValueType());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_MemberType(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Type type = (Type)luaObject;
		if (type == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name MemberType");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index MemberType on a nil value");
			}
		}
		LuaScriptMgr.Push(L, type.get_MemberType());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_Module(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Type type = (Type)luaObject;
		if (type == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name Module");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index Module on a nil value");
			}
		}
		LuaScriptMgr.PushObject(L, type.get_Module());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_Namespace(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Type type = (Type)luaObject;
		if (type == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name Namespace");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index Namespace on a nil value");
			}
		}
		LuaScriptMgr.Push(L, type.get_Namespace());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_ReflectedType(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Type type = (Type)luaObject;
		if (type == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name ReflectedType");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index ReflectedType on a nil value");
			}
		}
		LuaScriptMgr.Push(L, type.get_ReflectedType());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_TypeHandle(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Type type = (Type)luaObject;
		if (type == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name TypeHandle");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index TypeHandle on a nil value");
			}
		}
		LuaScriptMgr.PushValue(L, type.get_TypeHandle());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_TypeInitializer(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Type type = (Type)luaObject;
		if (type == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name TypeInitializer");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index TypeInitializer on a nil value");
			}
		}
		LuaScriptMgr.PushObject(L, type.get_TypeInitializer());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_UnderlyingSystemType(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Type type = (Type)luaObject;
		if (type == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name UnderlyingSystemType");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index UnderlyingSystemType on a nil value");
			}
		}
		LuaScriptMgr.Push(L, type.get_UnderlyingSystemType());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_ContainsGenericParameters(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Type type = (Type)luaObject;
		if (type == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name ContainsGenericParameters");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index ContainsGenericParameters on a nil value");
			}
		}
		LuaScriptMgr.Push(L, type.get_ContainsGenericParameters());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_IsGenericTypeDefinition(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Type type = (Type)luaObject;
		if (type == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name IsGenericTypeDefinition");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index IsGenericTypeDefinition on a nil value");
			}
		}
		LuaScriptMgr.Push(L, type.get_IsGenericTypeDefinition());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_IsGenericType(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Type type = (Type)luaObject;
		if (type == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name IsGenericType");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index IsGenericType on a nil value");
			}
		}
		LuaScriptMgr.Push(L, type.get_IsGenericType());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_IsGenericParameter(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Type type = (Type)luaObject;
		if (type == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name IsGenericParameter");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index IsGenericParameter on a nil value");
			}
		}
		LuaScriptMgr.Push(L, type.get_IsGenericParameter());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_IsNested(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Type type = (Type)luaObject;
		if (type == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name IsNested");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index IsNested on a nil value");
			}
		}
		LuaScriptMgr.Push(L, type.get_IsNested());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_IsVisible(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Type type = (Type)luaObject;
		if (type == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name IsVisible");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index IsVisible on a nil value");
			}
		}
		LuaScriptMgr.Push(L, type.get_IsVisible());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_GenericParameterPosition(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Type type = (Type)luaObject;
		if (type == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name GenericParameterPosition");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index GenericParameterPosition on a nil value");
			}
		}
		LuaScriptMgr.Push(L, type.get_GenericParameterPosition());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_GenericParameterAttributes(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Type type = (Type)luaObject;
		if (type == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name GenericParameterAttributes");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index GenericParameterAttributes on a nil value");
			}
		}
		LuaScriptMgr.Push(L, type.get_GenericParameterAttributes());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_DeclaringMethod(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Type type = (Type)luaObject;
		if (type == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name DeclaringMethod");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index DeclaringMethod on a nil value");
			}
		}
		LuaScriptMgr.PushObject(L, type.get_DeclaringMethod());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_StructLayoutAttribute(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Type type = (Type)luaObject;
		if (type == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name StructLayoutAttribute");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index StructLayoutAttribute on a nil value");
			}
		}
		LuaScriptMgr.PushObject(L, type.get_StructLayoutAttribute());
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Lua_ToString(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		if (luaObject != null)
		{
			LuaScriptMgr.Push(L, luaObject.ToString());
		}
		else
		{
			LuaScriptMgr.Push(L, "Table: System.Type");
		}
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Equals(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 2 && LuaScriptMgr.CheckTypes(L, 1, typeof(Type), typeof(Type)))
		{
			Type type = LuaScriptMgr.GetVarObject(L, 1) as Type;
			Type typeObject = LuaScriptMgr.GetTypeObject(L, 2);
			bool b = (type != null) ? type.Equals(typeObject) : (typeObject == null);
			LuaScriptMgr.Push(L, b);
			return 1;
		}
		if (num == 2 && LuaScriptMgr.CheckTypes(L, 1, typeof(Type), typeof(object)))
		{
			Type type2 = LuaScriptMgr.GetVarObject(L, 1) as Type;
			object varObject = LuaScriptMgr.GetVarObject(L, 2);
			bool b2 = (type2 != null) ? type2.Equals(varObject) : (varObject == null);
			LuaScriptMgr.Push(L, b2);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: Type.Equals");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetType(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 1)
		{
			Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
			Type type = typeObject.GetType();
			LuaScriptMgr.Push(L, type);
			return 1;
		}
		if (num == 1 && LuaScriptMgr.CheckTypes(L, 1, typeof(string)))
		{
			string @string = LuaScriptMgr.GetString(L, 1);
			Type type2 = Type.GetType(@string);
			LuaScriptMgr.Push(L, type2);
			return 1;
		}
		if (num == 2)
		{
			string luaString = LuaScriptMgr.GetLuaString(L, 1);
			bool boolean = LuaScriptMgr.GetBoolean(L, 2);
			Type type3 = Type.GetType(luaString, boolean);
			LuaScriptMgr.Push(L, type3);
			return 1;
		}
		if (num == 3)
		{
			string luaString2 = LuaScriptMgr.GetLuaString(L, 1);
			bool boolean2 = LuaScriptMgr.GetBoolean(L, 2);
			bool boolean3 = LuaScriptMgr.GetBoolean(L, 3);
			Type type4 = Type.GetType(luaString2, boolean2, boolean3);
			LuaScriptMgr.Push(L, type4);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: Type.GetType");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetTypeArray(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		object[] arrayObject = LuaScriptMgr.GetArrayObject<object>(L, 1);
		Type[] typeArray = Type.GetTypeArray(arrayObject);
		LuaScriptMgr.PushArray(L, typeArray);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetTypeCode(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
		TypeCode typeCode = Type.GetTypeCode(typeObject);
		LuaScriptMgr.Push(L, typeCode);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetTypeFromCLSID(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 1)
		{
			Guid guid = (Guid)LuaScriptMgr.GetNetObject(L, 1, typeof(Guid));
			Type typeFromCLSID = Type.GetTypeFromCLSID(guid);
			LuaScriptMgr.Push(L, typeFromCLSID);
			return 1;
		}
		if (num == 2 && LuaScriptMgr.CheckTypes(L, 1, typeof(Guid), typeof(string)))
		{
			Guid guid2 = (Guid)LuaScriptMgr.GetLuaObject(L, 1);
			string @string = LuaScriptMgr.GetString(L, 2);
			Type typeFromCLSID2 = Type.GetTypeFromCLSID(guid2, @string);
			LuaScriptMgr.Push(L, typeFromCLSID2);
			return 1;
		}
		if (num == 2 && LuaScriptMgr.CheckTypes(L, 1, typeof(Guid), typeof(bool)))
		{
			Guid guid3 = (Guid)LuaScriptMgr.GetLuaObject(L, 1);
			bool flag = LuaDLL.lua_toboolean(L, 2);
			Type typeFromCLSID3 = Type.GetTypeFromCLSID(guid3, flag);
			LuaScriptMgr.Push(L, typeFromCLSID3);
			return 1;
		}
		if (num == 3)
		{
			Guid guid4 = (Guid)LuaScriptMgr.GetNetObject(L, 1, typeof(Guid));
			string luaString = LuaScriptMgr.GetLuaString(L, 2);
			bool boolean = LuaScriptMgr.GetBoolean(L, 3);
			Type typeFromCLSID4 = Type.GetTypeFromCLSID(guid4, luaString, boolean);
			LuaScriptMgr.Push(L, typeFromCLSID4);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: Type.GetTypeFromCLSID");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetTypeFromHandle(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		RuntimeTypeHandle runtimeTypeHandle = (RuntimeTypeHandle)LuaScriptMgr.GetNetObject(L, 1, typeof(RuntimeTypeHandle));
		Type typeFromHandle = Type.GetTypeFromHandle(runtimeTypeHandle);
		LuaScriptMgr.Push(L, typeFromHandle);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetTypeFromProgID(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 1)
		{
			string luaString = LuaScriptMgr.GetLuaString(L, 1);
			Type typeFromProgID = Type.GetTypeFromProgID(luaString);
			LuaScriptMgr.Push(L, typeFromProgID);
			return 1;
		}
		if (num == 2 && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(string)))
		{
			string @string = LuaScriptMgr.GetString(L, 1);
			string string2 = LuaScriptMgr.GetString(L, 2);
			Type typeFromProgID2 = Type.GetTypeFromProgID(@string, string2);
			LuaScriptMgr.Push(L, typeFromProgID2);
			return 1;
		}
		if (num == 2 && LuaScriptMgr.CheckTypes(L, 1, typeof(string), typeof(bool)))
		{
			string string3 = LuaScriptMgr.GetString(L, 1);
			bool flag = LuaDLL.lua_toboolean(L, 2);
			Type typeFromProgID3 = Type.GetTypeFromProgID(string3, flag);
			LuaScriptMgr.Push(L, typeFromProgID3);
			return 1;
		}
		if (num == 3)
		{
			string luaString2 = LuaScriptMgr.GetLuaString(L, 1);
			string luaString3 = LuaScriptMgr.GetLuaString(L, 2);
			bool boolean = LuaScriptMgr.GetBoolean(L, 3);
			Type typeFromProgID4 = Type.GetTypeFromProgID(luaString2, luaString3, boolean);
			LuaScriptMgr.Push(L, typeFromProgID4);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: Type.GetTypeFromProgID");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetTypeHandle(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		object varObject = LuaScriptMgr.GetVarObject(L, 1);
		RuntimeTypeHandle typeHandle = Type.GetTypeHandle(varObject);
		LuaScriptMgr.PushValue(L, typeHandle);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int IsSubclassOf(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
		Type typeObject2 = LuaScriptMgr.GetTypeObject(L, 2);
		bool b = typeObject.IsSubclassOf(typeObject2);
		LuaScriptMgr.Push(L, b);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int FindInterfaces(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
		LuaTypes luaTypes = LuaDLL.lua_type(L, 2);
		TypeFilter typeFilter;
		if (luaTypes != LuaTypes.LUA_TFUNCTION)
		{
			typeFilter = (TypeFilter)LuaScriptMgr.GetNetObject(L, 2, typeof(TypeFilter));
		}
		else
		{
			LuaFunction func = LuaScriptMgr.GetLuaFunction(L, 2);
			typeFilter = delegate(Type param0, object param1)
			{
				int oldTop = func.BeginPCall();
				LuaScriptMgr.Push(L, param0);
				LuaScriptMgr.PushVarObject(L, param1);
				func.PCall(oldTop, 2);
				object[] array = func.PopValues(oldTop);
				func.EndPCall(oldTop);
				return (bool)array[0];
			};
		}
		object varObject = LuaScriptMgr.GetVarObject(L, 3);
		Type[] o = typeObject.FindInterfaces(typeFilter, varObject);
		LuaScriptMgr.PushArray(L, o);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetInterface(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 2)
		{
			Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
			string luaString = LuaScriptMgr.GetLuaString(L, 2);
			Type @interface = typeObject.GetInterface(luaString);
			LuaScriptMgr.Push(L, @interface);
			return 1;
		}
		if (num == 3)
		{
			Type typeObject2 = LuaScriptMgr.GetTypeObject(L, 1);
			string luaString2 = LuaScriptMgr.GetLuaString(L, 2);
			bool boolean = LuaScriptMgr.GetBoolean(L, 3);
			Type interface2 = typeObject2.GetInterface(luaString2, boolean);
			LuaScriptMgr.Push(L, interface2);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: Type.GetInterface");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetInterfaceMap(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
		Type typeObject2 = LuaScriptMgr.GetTypeObject(L, 2);
		InterfaceMapping interfaceMap = typeObject.GetInterfaceMap(typeObject2);
		LuaScriptMgr.PushValue(L, interfaceMap);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetInterfaces(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
		Type[] interfaces = typeObject.GetInterfaces();
		LuaScriptMgr.PushArray(L, interfaces);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int IsAssignableFrom(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
		Type typeObject2 = LuaScriptMgr.GetTypeObject(L, 2);
		bool b = typeObject.IsAssignableFrom(typeObject2);
		LuaScriptMgr.Push(L, b);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int IsInstanceOfType(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
		object varObject = LuaScriptMgr.GetVarObject(L, 2);
		bool b = typeObject.IsInstanceOfType(varObject);
		LuaScriptMgr.Push(L, b);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetArrayRank(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
		int arrayRank = typeObject.GetArrayRank();
		LuaScriptMgr.Push(L, arrayRank);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetElementType(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
		Type elementType = typeObject.GetElementType();
		LuaScriptMgr.Push(L, elementType);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetEvent(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 2)
		{
			Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
			string luaString = LuaScriptMgr.GetLuaString(L, 2);
			EventInfo @event = typeObject.GetEvent(luaString);
			LuaScriptMgr.PushObject(L, @event);
			return 1;
		}
		if (num == 3)
		{
			Type typeObject2 = LuaScriptMgr.GetTypeObject(L, 1);
			string luaString2 = LuaScriptMgr.GetLuaString(L, 2);
			BindingFlags bindingFlags = (int)LuaScriptMgr.GetNetObject(L, 3, typeof(BindingFlags));
			EventInfo event2 = typeObject2.GetEvent(luaString2, bindingFlags);
			LuaScriptMgr.PushObject(L, event2);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: Type.GetEvent");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetEvents(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 1)
		{
			Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
			EventInfo[] events = typeObject.GetEvents();
			LuaScriptMgr.PushArray(L, events);
			return 1;
		}
		if (num == 2)
		{
			Type typeObject2 = LuaScriptMgr.GetTypeObject(L, 1);
			BindingFlags bindingFlags = (int)LuaScriptMgr.GetNetObject(L, 2, typeof(BindingFlags));
			EventInfo[] events2 = typeObject2.GetEvents(bindingFlags);
			LuaScriptMgr.PushArray(L, events2);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: Type.GetEvents");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetField(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 2)
		{
			Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
			string luaString = LuaScriptMgr.GetLuaString(L, 2);
			FieldInfo field = typeObject.GetField(luaString);
			LuaScriptMgr.PushObject(L, field);
			return 1;
		}
		if (num == 3)
		{
			Type typeObject2 = LuaScriptMgr.GetTypeObject(L, 1);
			string luaString2 = LuaScriptMgr.GetLuaString(L, 2);
			BindingFlags bindingFlags = (int)LuaScriptMgr.GetNetObject(L, 3, typeof(BindingFlags));
			FieldInfo field2 = typeObject2.GetField(luaString2, bindingFlags);
			LuaScriptMgr.PushObject(L, field2);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: Type.GetField");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetFields(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 1)
		{
			Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
			FieldInfo[] fields = typeObject.GetFields();
			LuaScriptMgr.PushArray(L, fields);
			return 1;
		}
		if (num == 2)
		{
			Type typeObject2 = LuaScriptMgr.GetTypeObject(L, 1);
			BindingFlags bindingFlags = (int)LuaScriptMgr.GetNetObject(L, 2, typeof(BindingFlags));
			FieldInfo[] fields2 = typeObject2.GetFields(bindingFlags);
			LuaScriptMgr.PushArray(L, fields2);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: Type.GetFields");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetHashCode(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
		int hashCode = typeObject.GetHashCode();
		LuaScriptMgr.Push(L, hashCode);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetMember(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 2)
		{
			Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
			string luaString = LuaScriptMgr.GetLuaString(L, 2);
			MemberInfo[] member = typeObject.GetMember(luaString);
			LuaScriptMgr.PushArray(L, member);
			return 1;
		}
		if (num == 3)
		{
			Type typeObject2 = LuaScriptMgr.GetTypeObject(L, 1);
			string luaString2 = LuaScriptMgr.GetLuaString(L, 2);
			BindingFlags bindingFlags = (int)LuaScriptMgr.GetNetObject(L, 3, typeof(BindingFlags));
			MemberInfo[] member2 = typeObject2.GetMember(luaString2, bindingFlags);
			LuaScriptMgr.PushArray(L, member2);
			return 1;
		}
		if (num == 4)
		{
			Type typeObject3 = LuaScriptMgr.GetTypeObject(L, 1);
			string luaString3 = LuaScriptMgr.GetLuaString(L, 2);
			MemberTypes memberTypes = (int)LuaScriptMgr.GetNetObject(L, 3, typeof(MemberTypes));
			BindingFlags bindingFlags2 = (int)LuaScriptMgr.GetNetObject(L, 4, typeof(BindingFlags));
			MemberInfo[] member3 = typeObject3.GetMember(luaString3, memberTypes, bindingFlags2);
			LuaScriptMgr.PushArray(L, member3);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: Type.GetMember");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetMembers(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 1)
		{
			Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
			MemberInfo[] members = typeObject.GetMembers();
			LuaScriptMgr.PushArray(L, members);
			return 1;
		}
		if (num == 2)
		{
			Type typeObject2 = LuaScriptMgr.GetTypeObject(L, 1);
			BindingFlags bindingFlags = (int)LuaScriptMgr.GetNetObject(L, 2, typeof(BindingFlags));
			MemberInfo[] members2 = typeObject2.GetMembers(bindingFlags);
			LuaScriptMgr.PushArray(L, members2);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: Type.GetMembers");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetMethod(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 2)
		{
			Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
			string luaString = LuaScriptMgr.GetLuaString(L, 2);
			MethodInfo method = typeObject.GetMethod(luaString);
			LuaScriptMgr.PushObject(L, method);
			return 1;
		}
		if (num == 3 && LuaScriptMgr.CheckTypes(L, 1, typeof(Type), typeof(string), typeof(Type[])))
		{
			Type typeObject2 = LuaScriptMgr.GetTypeObject(L, 1);
			string @string = LuaScriptMgr.GetString(L, 2);
			Type[] arrayObject = LuaScriptMgr.GetArrayObject<Type>(L, 3);
			MethodInfo method2 = typeObject2.GetMethod(@string, arrayObject);
			LuaScriptMgr.PushObject(L, method2);
			return 1;
		}
		if (num == 3 && LuaScriptMgr.CheckTypes(L, 1, typeof(Type), typeof(string), typeof(BindingFlags)))
		{
			Type typeObject3 = LuaScriptMgr.GetTypeObject(L, 1);
			string string2 = LuaScriptMgr.GetString(L, 2);
			BindingFlags bindingFlags = (int)LuaScriptMgr.GetLuaObject(L, 3);
			MethodInfo method3 = typeObject3.GetMethod(string2, bindingFlags);
			LuaScriptMgr.PushObject(L, method3);
			return 1;
		}
		if (num == 4)
		{
			Type typeObject4 = LuaScriptMgr.GetTypeObject(L, 1);
			string luaString2 = LuaScriptMgr.GetLuaString(L, 2);
			Type[] arrayObject2 = LuaScriptMgr.GetArrayObject<Type>(L, 3);
			ParameterModifier[] arrayObject3 = LuaScriptMgr.GetArrayObject<ParameterModifier>(L, 4);
			MethodInfo method4 = typeObject4.GetMethod(luaString2, arrayObject2, arrayObject3);
			LuaScriptMgr.PushObject(L, method4);
			return 1;
		}
		if (num == 6)
		{
			Type typeObject5 = LuaScriptMgr.GetTypeObject(L, 1);
			string luaString3 = LuaScriptMgr.GetLuaString(L, 2);
			BindingFlags bindingFlags2 = (int)LuaScriptMgr.GetNetObject(L, 3, typeof(BindingFlags));
			Binder binder = (Binder)LuaScriptMgr.GetNetObject(L, 4, typeof(Binder));
			Type[] arrayObject4 = LuaScriptMgr.GetArrayObject<Type>(L, 5);
			ParameterModifier[] arrayObject5 = LuaScriptMgr.GetArrayObject<ParameterModifier>(L, 6);
			MethodInfo method5 = typeObject5.GetMethod(luaString3, bindingFlags2, binder, arrayObject4, arrayObject5);
			LuaScriptMgr.PushObject(L, method5);
			return 1;
		}
		if (num == 7)
		{
			Type typeObject6 = LuaScriptMgr.GetTypeObject(L, 1);
			string luaString4 = LuaScriptMgr.GetLuaString(L, 2);
			BindingFlags bindingFlags3 = (int)LuaScriptMgr.GetNetObject(L, 3, typeof(BindingFlags));
			Binder binder2 = (Binder)LuaScriptMgr.GetNetObject(L, 4, typeof(Binder));
			CallingConventions callingConventions = (int)LuaScriptMgr.GetNetObject(L, 5, typeof(CallingConventions));
			Type[] arrayObject6 = LuaScriptMgr.GetArrayObject<Type>(L, 6);
			ParameterModifier[] arrayObject7 = LuaScriptMgr.GetArrayObject<ParameterModifier>(L, 7);
			MethodInfo method6 = typeObject6.GetMethod(luaString4, bindingFlags3, binder2, callingConventions, arrayObject6, arrayObject7);
			LuaScriptMgr.PushObject(L, method6);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: Type.GetMethod");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetMethods(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 1)
		{
			Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
			MethodInfo[] methods = typeObject.GetMethods();
			LuaScriptMgr.PushArray(L, methods);
			return 1;
		}
		if (num == 2)
		{
			Type typeObject2 = LuaScriptMgr.GetTypeObject(L, 1);
			BindingFlags bindingFlags = (int)LuaScriptMgr.GetNetObject(L, 2, typeof(BindingFlags));
			MethodInfo[] methods2 = typeObject2.GetMethods(bindingFlags);
			LuaScriptMgr.PushArray(L, methods2);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: Type.GetMethods");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetNestedType(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 2)
		{
			Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
			string luaString = LuaScriptMgr.GetLuaString(L, 2);
			Type nestedType = typeObject.GetNestedType(luaString);
			LuaScriptMgr.Push(L, nestedType);
			return 1;
		}
		if (num == 3)
		{
			Type typeObject2 = LuaScriptMgr.GetTypeObject(L, 1);
			string luaString2 = LuaScriptMgr.GetLuaString(L, 2);
			BindingFlags bindingFlags = (int)LuaScriptMgr.GetNetObject(L, 3, typeof(BindingFlags));
			Type nestedType2 = typeObject2.GetNestedType(luaString2, bindingFlags);
			LuaScriptMgr.Push(L, nestedType2);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: Type.GetNestedType");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetNestedTypes(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 1)
		{
			Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
			Type[] nestedTypes = typeObject.GetNestedTypes();
			LuaScriptMgr.PushArray(L, nestedTypes);
			return 1;
		}
		if (num == 2)
		{
			Type typeObject2 = LuaScriptMgr.GetTypeObject(L, 1);
			BindingFlags bindingFlags = (int)LuaScriptMgr.GetNetObject(L, 2, typeof(BindingFlags));
			Type[] nestedTypes2 = typeObject2.GetNestedTypes(bindingFlags);
			LuaScriptMgr.PushArray(L, nestedTypes2);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: Type.GetNestedTypes");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetProperties(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 1)
		{
			Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
			PropertyInfo[] properties = typeObject.GetProperties();
			LuaScriptMgr.PushArray(L, properties);
			return 1;
		}
		if (num == 2)
		{
			Type typeObject2 = LuaScriptMgr.GetTypeObject(L, 1);
			BindingFlags bindingFlags = (int)LuaScriptMgr.GetNetObject(L, 2, typeof(BindingFlags));
			PropertyInfo[] properties2 = typeObject2.GetProperties(bindingFlags);
			LuaScriptMgr.PushArray(L, properties2);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: Type.GetProperties");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetProperty(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 2)
		{
			Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
			string luaString = LuaScriptMgr.GetLuaString(L, 2);
			PropertyInfo property = typeObject.GetProperty(luaString);
			LuaScriptMgr.PushObject(L, property);
			return 1;
		}
		if (num == 3 && LuaScriptMgr.CheckTypes(L, 1, typeof(Type), typeof(string), typeof(Type[])))
		{
			Type typeObject2 = LuaScriptMgr.GetTypeObject(L, 1);
			string @string = LuaScriptMgr.GetString(L, 2);
			Type[] arrayObject = LuaScriptMgr.GetArrayObject<Type>(L, 3);
			PropertyInfo property2 = typeObject2.GetProperty(@string, arrayObject);
			LuaScriptMgr.PushObject(L, property2);
			return 1;
		}
		if (num == 3 && LuaScriptMgr.CheckTypes(L, 1, typeof(Type), typeof(string), typeof(Type)))
		{
			Type typeObject3 = LuaScriptMgr.GetTypeObject(L, 1);
			string string2 = LuaScriptMgr.GetString(L, 2);
			Type typeObject4 = LuaScriptMgr.GetTypeObject(L, 3);
			PropertyInfo property3 = typeObject3.GetProperty(string2, typeObject4);
			LuaScriptMgr.PushObject(L, property3);
			return 1;
		}
		if (num == 3 && LuaScriptMgr.CheckTypes(L, 1, typeof(Type), typeof(string), typeof(BindingFlags)))
		{
			Type typeObject5 = LuaScriptMgr.GetTypeObject(L, 1);
			string string3 = LuaScriptMgr.GetString(L, 2);
			BindingFlags bindingFlags = (int)LuaScriptMgr.GetLuaObject(L, 3);
			PropertyInfo property4 = typeObject5.GetProperty(string3, bindingFlags);
			LuaScriptMgr.PushObject(L, property4);
			return 1;
		}
		if (num == 4)
		{
			Type typeObject6 = LuaScriptMgr.GetTypeObject(L, 1);
			string luaString2 = LuaScriptMgr.GetLuaString(L, 2);
			Type typeObject7 = LuaScriptMgr.GetTypeObject(L, 3);
			Type[] arrayObject2 = LuaScriptMgr.GetArrayObject<Type>(L, 4);
			PropertyInfo property5 = typeObject6.GetProperty(luaString2, typeObject7, arrayObject2);
			LuaScriptMgr.PushObject(L, property5);
			return 1;
		}
		if (num == 5)
		{
			Type typeObject8 = LuaScriptMgr.GetTypeObject(L, 1);
			string luaString3 = LuaScriptMgr.GetLuaString(L, 2);
			Type typeObject9 = LuaScriptMgr.GetTypeObject(L, 3);
			Type[] arrayObject3 = LuaScriptMgr.GetArrayObject<Type>(L, 4);
			ParameterModifier[] arrayObject4 = LuaScriptMgr.GetArrayObject<ParameterModifier>(L, 5);
			PropertyInfo property6 = typeObject8.GetProperty(luaString3, typeObject9, arrayObject3, arrayObject4);
			LuaScriptMgr.PushObject(L, property6);
			return 1;
		}
		if (num == 7)
		{
			Type typeObject10 = LuaScriptMgr.GetTypeObject(L, 1);
			string luaString4 = LuaScriptMgr.GetLuaString(L, 2);
			BindingFlags bindingFlags2 = (int)LuaScriptMgr.GetNetObject(L, 3, typeof(BindingFlags));
			Binder binder = (Binder)LuaScriptMgr.GetNetObject(L, 4, typeof(Binder));
			Type typeObject11 = LuaScriptMgr.GetTypeObject(L, 5);
			Type[] arrayObject5 = LuaScriptMgr.GetArrayObject<Type>(L, 6);
			ParameterModifier[] arrayObject6 = LuaScriptMgr.GetArrayObject<ParameterModifier>(L, 7);
			PropertyInfo property7 = typeObject10.GetProperty(luaString4, bindingFlags2, binder, typeObject11, arrayObject5, arrayObject6);
			LuaScriptMgr.PushObject(L, property7);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: Type.GetProperty");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetConstructor(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 2)
		{
			Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
			Type[] arrayObject = LuaScriptMgr.GetArrayObject<Type>(L, 2);
			ConstructorInfo constructor = typeObject.GetConstructor(arrayObject);
			LuaScriptMgr.PushObject(L, constructor);
			return 1;
		}
		if (num == 5)
		{
			Type typeObject2 = LuaScriptMgr.GetTypeObject(L, 1);
			BindingFlags bindingFlags = (int)LuaScriptMgr.GetNetObject(L, 2, typeof(BindingFlags));
			Binder binder = (Binder)LuaScriptMgr.GetNetObject(L, 3, typeof(Binder));
			Type[] arrayObject2 = LuaScriptMgr.GetArrayObject<Type>(L, 4);
			ParameterModifier[] arrayObject3 = LuaScriptMgr.GetArrayObject<ParameterModifier>(L, 5);
			ConstructorInfo constructor2 = typeObject2.GetConstructor(bindingFlags, binder, arrayObject2, arrayObject3);
			LuaScriptMgr.PushObject(L, constructor2);
			return 1;
		}
		if (num == 6)
		{
			Type typeObject3 = LuaScriptMgr.GetTypeObject(L, 1);
			BindingFlags bindingFlags2 = (int)LuaScriptMgr.GetNetObject(L, 2, typeof(BindingFlags));
			Binder binder2 = (Binder)LuaScriptMgr.GetNetObject(L, 3, typeof(Binder));
			CallingConventions callingConventions = (int)LuaScriptMgr.GetNetObject(L, 4, typeof(CallingConventions));
			Type[] arrayObject4 = LuaScriptMgr.GetArrayObject<Type>(L, 5);
			ParameterModifier[] arrayObject5 = LuaScriptMgr.GetArrayObject<ParameterModifier>(L, 6);
			ConstructorInfo constructor3 = typeObject3.GetConstructor(bindingFlags2, binder2, callingConventions, arrayObject4, arrayObject5);
			LuaScriptMgr.PushObject(L, constructor3);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: Type.GetConstructor");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetConstructors(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 1)
		{
			Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
			ConstructorInfo[] constructors = typeObject.GetConstructors();
			LuaScriptMgr.PushArray(L, constructors);
			return 1;
		}
		if (num == 2)
		{
			Type typeObject2 = LuaScriptMgr.GetTypeObject(L, 1);
			BindingFlags bindingFlags = (int)LuaScriptMgr.GetNetObject(L, 2, typeof(BindingFlags));
			ConstructorInfo[] constructors2 = typeObject2.GetConstructors(bindingFlags);
			LuaScriptMgr.PushArray(L, constructors2);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: Type.GetConstructors");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetDefaultMembers(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
		MemberInfo[] defaultMembers = typeObject.GetDefaultMembers();
		LuaScriptMgr.PushArray(L, defaultMembers);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int FindMembers(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 5);
		Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
		MemberTypes memberTypes = (int)LuaScriptMgr.GetNetObject(L, 2, typeof(MemberTypes));
		BindingFlags bindingFlags = (int)LuaScriptMgr.GetNetObject(L, 3, typeof(BindingFlags));
		LuaTypes luaTypes = LuaDLL.lua_type(L, 4);
		MemberFilter memberFilter;
		if (luaTypes != LuaTypes.LUA_TFUNCTION)
		{
			memberFilter = (MemberFilter)LuaScriptMgr.GetNetObject(L, 4, typeof(MemberFilter));
		}
		else
		{
			LuaFunction func = LuaScriptMgr.GetLuaFunction(L, 4);
			memberFilter = delegate(MemberInfo param0, object param1)
			{
				int oldTop = func.BeginPCall();
				LuaScriptMgr.PushObject(L, param0);
				LuaScriptMgr.PushVarObject(L, param1);
				func.PCall(oldTop, 2);
				object[] array = func.PopValues(oldTop);
				func.EndPCall(oldTop);
				return (bool)array[0];
			};
		}
		object varObject = LuaScriptMgr.GetVarObject(L, 5);
		MemberInfo[] o = typeObject.FindMembers(memberTypes, bindingFlags, memberFilter, varObject);
		LuaScriptMgr.PushArray(L, o);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int InvokeMember(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 6)
		{
			Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
			string luaString = LuaScriptMgr.GetLuaString(L, 2);
			BindingFlags bindingFlags = (int)LuaScriptMgr.GetNetObject(L, 3, typeof(BindingFlags));
			Binder binder = (Binder)LuaScriptMgr.GetNetObject(L, 4, typeof(Binder));
			object varObject = LuaScriptMgr.GetVarObject(L, 5);
			object[] arrayObject = LuaScriptMgr.GetArrayObject<object>(L, 6);
			object o = typeObject.InvokeMember(luaString, bindingFlags, binder, varObject, arrayObject);
			LuaScriptMgr.PushVarObject(L, o);
			return 1;
		}
		if (num == 7)
		{
			Type typeObject2 = LuaScriptMgr.GetTypeObject(L, 1);
			string luaString2 = LuaScriptMgr.GetLuaString(L, 2);
			BindingFlags bindingFlags2 = (int)LuaScriptMgr.GetNetObject(L, 3, typeof(BindingFlags));
			Binder binder2 = (Binder)LuaScriptMgr.GetNetObject(L, 4, typeof(Binder));
			object varObject2 = LuaScriptMgr.GetVarObject(L, 5);
			object[] arrayObject2 = LuaScriptMgr.GetArrayObject<object>(L, 6);
			CultureInfo cultureInfo = (CultureInfo)LuaScriptMgr.GetNetObject(L, 7, typeof(CultureInfo));
			object o2 = typeObject2.InvokeMember(luaString2, bindingFlags2, binder2, varObject2, arrayObject2, cultureInfo);
			LuaScriptMgr.PushVarObject(L, o2);
			return 1;
		}
		if (num == 9)
		{
			Type typeObject3 = LuaScriptMgr.GetTypeObject(L, 1);
			string luaString3 = LuaScriptMgr.GetLuaString(L, 2);
			BindingFlags bindingFlags3 = (int)LuaScriptMgr.GetNetObject(L, 3, typeof(BindingFlags));
			Binder binder3 = (Binder)LuaScriptMgr.GetNetObject(L, 4, typeof(Binder));
			object varObject3 = LuaScriptMgr.GetVarObject(L, 5);
			object[] arrayObject3 = LuaScriptMgr.GetArrayObject<object>(L, 6);
			ParameterModifier[] arrayObject4 = LuaScriptMgr.GetArrayObject<ParameterModifier>(L, 7);
			CultureInfo cultureInfo2 = (CultureInfo)LuaScriptMgr.GetNetObject(L, 8, typeof(CultureInfo));
			string[] arrayString = LuaScriptMgr.GetArrayString(L, 9);
			object o3 = typeObject3.InvokeMember(luaString3, bindingFlags3, binder3, varObject3, arrayObject3, arrayObject4, cultureInfo2, arrayString);
			LuaScriptMgr.PushVarObject(L, o3);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: Type.InvokeMember");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int ToString(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
		string str = typeObject.ToString();
		LuaScriptMgr.Push(L, str);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetGenericArguments(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
		Type[] genericArguments = typeObject.GetGenericArguments();
		LuaScriptMgr.PushArray(L, genericArguments);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetGenericTypeDefinition(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
		Type genericTypeDefinition = typeObject.GetGenericTypeDefinition();
		LuaScriptMgr.Push(L, genericTypeDefinition);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int MakeGenericType(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
		Type[] paramsObject = LuaScriptMgr.GetParamsObject<Type>(L, 2, num - 1);
		Type o = typeObject.MakeGenericType(paramsObject);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetGenericParameterConstraints(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
		Type[] genericParameterConstraints = typeObject.GetGenericParameterConstraints();
		LuaScriptMgr.PushArray(L, genericParameterConstraints);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int MakeArrayType(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 1)
		{
			Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
			Type o = typeObject.MakeArrayType();
			LuaScriptMgr.Push(L, o);
			return 1;
		}
		if (num == 2)
		{
			Type typeObject2 = LuaScriptMgr.GetTypeObject(L, 1);
			int num2 = (int)LuaScriptMgr.GetNumber(L, 2);
			Type o2 = typeObject2.MakeArrayType(num2);
			LuaScriptMgr.Push(L, o2);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: Type.MakeArrayType");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int MakeByRefType(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
		Type o = typeObject.MakeByRefType();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int MakePointerType(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		Type typeObject = LuaScriptMgr.GetTypeObject(L, 1);
		Type o = typeObject.MakePointerType();
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int ReflectionOnlyGetType(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		string luaString = LuaScriptMgr.GetLuaString(L, 1);
		bool boolean = LuaScriptMgr.GetBoolean(L, 2);
		bool boolean2 = LuaScriptMgr.GetBoolean(L, 3);
		Type o = Type.ReflectionOnlyGetType(luaString, boolean, boolean2);
		LuaScriptMgr.Push(L, o);
		return 1;
	}
}
