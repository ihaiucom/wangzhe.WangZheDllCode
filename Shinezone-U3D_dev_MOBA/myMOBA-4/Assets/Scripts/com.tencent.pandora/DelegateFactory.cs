using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace com.tencent.pandora
{
	public static class DelegateFactory
	{
		private delegate Delegate DelegateValue(LuaFunction func);

		private static Dictionary<Type, DelegateFactory.DelegateValue> dict = new Dictionary<Type, DelegateFactory.DelegateValue>();

		[NoToLua]
		public static void Register(IntPtr L)
		{
			DelegateFactory.dict.Add(typeof(Action<GameObject>), new DelegateFactory.DelegateValue(DelegateFactory.Action_GameObject));
			DelegateFactory.dict.Add(typeof(Action), new DelegateFactory.DelegateValue(DelegateFactory.Action));
			DelegateFactory.dict.Add(typeof(UnityAction), new DelegateFactory.DelegateValue(DelegateFactory.UnityEngine_Events_UnityAction));
			DelegateFactory.dict.Add(typeof(MemberFilter), new DelegateFactory.DelegateValue(DelegateFactory.System_Reflection_MemberFilter));
			DelegateFactory.dict.Add(typeof(TypeFilter), new DelegateFactory.DelegateValue(DelegateFactory.System_Reflection_TypeFilter));
		}

		[NoToLua]
		public static Delegate CreateDelegate(Type t, LuaFunction func)
		{
			DelegateFactory.DelegateValue delegateValue = null;
			if (!DelegateFactory.dict.TryGetValue(t, out delegateValue))
			{
				Debug.LogError("Delegate " + t.FullName + " not register");
				return null;
			}
			return delegateValue(func);
		}

		public static Delegate Action_GameObject(LuaFunction func)
		{
			return new Action<GameObject>(delegate(GameObject param0)
			{
				int oldTop = func.BeginPCall();
				IntPtr luaState = func.GetLuaState();
				LuaScriptMgr.Push(luaState, param0);
				func.PCall(oldTop, 1);
				func.EndPCall(oldTop);
			});
		}

		public static Delegate Action(LuaFunction func)
		{
			return new Action(delegate
			{
				func.Call();
			});
		}

		public static Delegate UnityEngine_Events_UnityAction(LuaFunction func)
		{
			return new UnityAction(delegate
			{
				func.Call();
			});
		}

		public static Delegate System_Reflection_MemberFilter(LuaFunction func)
		{
			return new MemberFilter(delegate(MemberInfo param0, object param1)
			{
				int oldTop = func.BeginPCall();
				IntPtr luaState = func.GetLuaState();
				LuaScriptMgr.PushObject(luaState, param0);
				LuaScriptMgr.PushVarObject(luaState, param1);
				func.PCall(oldTop, 2);
				object[] array = func.PopValues(oldTop);
				func.EndPCall(oldTop);
				return (bool)array[0];
			});
		}

		public static Delegate System_Reflection_TypeFilter(LuaFunction func)
		{
			return new TypeFilter(delegate(Type param0, object param1)
			{
				int oldTop = func.BeginPCall();
				IntPtr luaState = func.GetLuaState();
				LuaScriptMgr.Push(luaState, param0);
				LuaScriptMgr.PushVarObject(luaState, param1);
				func.PCall(oldTop, 2);
				object[] array = func.PopValues(oldTop);
				func.EndPCall(oldTop);
				return (bool)array[0];
			});
		}

		public static void Clear()
		{
			DelegateFactory.dict.Clear();
		}
	}
}
