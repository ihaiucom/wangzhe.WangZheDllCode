using com.tencent.pandora;
using System;

public class com_tencent_pandora_DelegateFactoryWrap
{
	private static Type classType = typeof(DelegateFactory);

	public static void Register(IntPtr L)
	{
		LuaMethod[] regs = new LuaMethod[]
		{
			new LuaMethod("Action_GameObject", new LuaCSFunction(com_tencent_pandora_DelegateFactoryWrap.Action_GameObject)),
			new LuaMethod("Action", new LuaCSFunction(com_tencent_pandora_DelegateFactoryWrap.Action)),
			new LuaMethod("UnityEngine_Events_UnityAction", new LuaCSFunction(com_tencent_pandora_DelegateFactoryWrap.UnityEngine_Events_UnityAction)),
			new LuaMethod("System_Reflection_MemberFilter", new LuaCSFunction(com_tencent_pandora_DelegateFactoryWrap.System_Reflection_MemberFilter)),
			new LuaMethod("System_Reflection_TypeFilter", new LuaCSFunction(com_tencent_pandora_DelegateFactoryWrap.System_Reflection_TypeFilter)),
			new LuaMethod("Clear", new LuaCSFunction(com_tencent_pandora_DelegateFactoryWrap.Clear)),
			new LuaMethod("New", new LuaCSFunction(com_tencent_pandora_DelegateFactoryWrap._Createcom_tencent_pandora_DelegateFactory)),
			new LuaMethod("GetClassType", new LuaCSFunction(com_tencent_pandora_DelegateFactoryWrap.GetClassType))
		};
		LuaScriptMgr.RegisterLib(L, "com.tencent.pandora.DelegateFactory", regs);
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int _Createcom_tencent_pandora_DelegateFactory(IntPtr L)
	{
		LuaDLL.luaL_error(L, "com.tencent.pandora.DelegateFactory class does not have a constructor function");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, com_tencent_pandora_DelegateFactoryWrap.classType);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Action_GameObject(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		LuaFunction luaFunction = LuaScriptMgr.GetLuaFunction(L, 1);
		Delegate o = DelegateFactory.Action_GameObject(luaFunction);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Action(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		LuaFunction luaFunction = LuaScriptMgr.GetLuaFunction(L, 1);
		Delegate o = DelegateFactory.Action(luaFunction);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int UnityEngine_Events_UnityAction(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		LuaFunction luaFunction = LuaScriptMgr.GetLuaFunction(L, 1);
		Delegate o = DelegateFactory.UnityEngine_Events_UnityAction(luaFunction);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int System_Reflection_MemberFilter(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		LuaFunction luaFunction = LuaScriptMgr.GetLuaFunction(L, 1);
		Delegate o = DelegateFactory.System_Reflection_MemberFilter(luaFunction);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int System_Reflection_TypeFilter(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		LuaFunction luaFunction = LuaScriptMgr.GetLuaFunction(L, 1);
		Delegate o = DelegateFactory.System_Reflection_TypeFilter(luaFunction);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Clear(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 0);
		DelegateFactory.Clear();
		return 0;
	}
}
