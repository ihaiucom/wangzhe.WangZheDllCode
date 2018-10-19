using com.tencent.pandora;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ComponentWrap
{
	private static Type classType = typeof(Component);

	public static void Register(IntPtr L)
	{
		LuaMethod[] regs = new LuaMethod[]
		{
			new LuaMethod("GetComponent", new LuaCSFunction(ComponentWrap.GetComponent)),
			new LuaMethod("GetComponentInChildren", new LuaCSFunction(ComponentWrap.GetComponentInChildren)),
			new LuaMethod("GetComponentsInChildren", new LuaCSFunction(ComponentWrap.GetComponentsInChildren)),
			new LuaMethod("GetComponentInParent", new LuaCSFunction(ComponentWrap.GetComponentInParent)),
			new LuaMethod("GetComponentsInParent", new LuaCSFunction(ComponentWrap.GetComponentsInParent)),
			new LuaMethod("GetComponents", new LuaCSFunction(ComponentWrap.GetComponents)),
			new LuaMethod("CompareTag", new LuaCSFunction(ComponentWrap.CompareTag)),
			new LuaMethod("SendMessageUpwards", new LuaCSFunction(ComponentWrap.SendMessageUpwards)),
			new LuaMethod("SendMessage", new LuaCSFunction(ComponentWrap.SendMessage)),
			new LuaMethod("BroadcastMessage", new LuaCSFunction(ComponentWrap.BroadcastMessage)),
			new LuaMethod("New", new LuaCSFunction(ComponentWrap._CreateComponent)),
			new LuaMethod("GetClassType", new LuaCSFunction(ComponentWrap.GetClassType)),
			new LuaMethod("__eq", new LuaCSFunction(ComponentWrap.Lua_Eq))
		};
		LuaField[] fields = new LuaField[]
		{
			new LuaField("transform", new LuaCSFunction(ComponentWrap.get_transform), null),
			new LuaField("rigidbody", new LuaCSFunction(ComponentWrap.get_rigidbody), null),
			new LuaField("rigidbody2D", new LuaCSFunction(ComponentWrap.get_rigidbody2D), null),
			new LuaField("camera", new LuaCSFunction(ComponentWrap.get_camera), null),
			new LuaField("light", new LuaCSFunction(ComponentWrap.get_light), null),
			new LuaField("animation", new LuaCSFunction(ComponentWrap.get_animation), null),
			new LuaField("constantForce", new LuaCSFunction(ComponentWrap.get_constantForce), null),
			new LuaField("renderer", new LuaCSFunction(ComponentWrap.get_renderer), null),
			new LuaField("audio", new LuaCSFunction(ComponentWrap.get_audio), null),
			new LuaField("guiText", new LuaCSFunction(ComponentWrap.get_guiText), null),
			new LuaField("networkView", new LuaCSFunction(ComponentWrap.get_networkView), null),
			new LuaField("guiTexture", new LuaCSFunction(ComponentWrap.get_guiTexture), null),
			new LuaField("collider", new LuaCSFunction(ComponentWrap.get_collider), null),
			new LuaField("collider2D", new LuaCSFunction(ComponentWrap.get_collider2D), null),
			new LuaField("hingeJoint", new LuaCSFunction(ComponentWrap.get_hingeJoint), null),
			new LuaField("particleEmitter", new LuaCSFunction(ComponentWrap.get_particleEmitter), null),
			new LuaField("particleSystem", new LuaCSFunction(ComponentWrap.get_particleSystem), null),
			new LuaField("gameObject", new LuaCSFunction(ComponentWrap.get_gameObject), null),
			new LuaField("tag", new LuaCSFunction(ComponentWrap.get_tag), new LuaCSFunction(ComponentWrap.set_tag))
		};
		LuaScriptMgr.RegisterLib(L, "UnityEngine.Component", typeof(Component), regs, fields, typeof(UnityEngine.Object));
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int _CreateComponent(IntPtr L)
	{
		if (LuaDLL.lua_gettop(L) == 0)
		{
			Component obj = new Component();
			LuaScriptMgr.Push(L, obj);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: Component.New");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, ComponentWrap.classType);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_transform(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Component component = (Component)luaObject;
		if (component == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name transform");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index transform on a nil value");
			}
		}
		LuaScriptMgr.Push(L, component.transform);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_rigidbody(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Component component = (Component)luaObject;
		if (component == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name rigidbody");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index rigidbody on a nil value");
			}
		}
		LuaScriptMgr.Push(L, component.rigidbody);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_rigidbody2D(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Component component = (Component)luaObject;
		if (component == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name rigidbody2D");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index rigidbody2D on a nil value");
			}
		}
		LuaScriptMgr.Push(L, component.rigidbody2D);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_camera(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Component component = (Component)luaObject;
		if (component == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name camera");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index camera on a nil value");
			}
		}
		LuaScriptMgr.Push(L, component.camera);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_light(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Component component = (Component)luaObject;
		if (component == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name light");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index light on a nil value");
			}
		}
		LuaScriptMgr.Push(L, component.light);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_animation(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Component component = (Component)luaObject;
		if (component == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name animation");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index animation on a nil value");
			}
		}
		LuaScriptMgr.Push(L, component.animation);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_constantForce(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Component component = (Component)luaObject;
		if (component == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name constantForce");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index constantForce on a nil value");
			}
		}
		LuaScriptMgr.Push(L, component.constantForce);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_renderer(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Component component = (Component)luaObject;
		if (component == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name renderer");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index renderer on a nil value");
			}
		}
		LuaScriptMgr.Push(L, component.renderer);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_audio(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Component component = (Component)luaObject;
		if (component == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name audio");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index audio on a nil value");
			}
		}
		LuaScriptMgr.Push(L, component.audio);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_guiText(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Component component = (Component)luaObject;
		if (component == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name guiText");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index guiText on a nil value");
			}
		}
		LuaScriptMgr.Push(L, component.guiText);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_networkView(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Component component = (Component)luaObject;
		if (component == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name networkView");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index networkView on a nil value");
			}
		}
		LuaScriptMgr.Push(L, component.networkView);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_guiTexture(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Component component = (Component)luaObject;
		if (component == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name guiTexture");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index guiTexture on a nil value");
			}
		}
		LuaScriptMgr.Push(L, component.guiTexture);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_collider(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Component component = (Component)luaObject;
		if (component == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name collider");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index collider on a nil value");
			}
		}
		LuaScriptMgr.Push(L, component.collider);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_collider2D(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Component component = (Component)luaObject;
		if (component == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name collider2D");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index collider2D on a nil value");
			}
		}
		LuaScriptMgr.Push(L, component.collider2D);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_hingeJoint(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Component component = (Component)luaObject;
		if (component == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name hingeJoint");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index hingeJoint on a nil value");
			}
		}
		LuaScriptMgr.Push(L, component.hingeJoint);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_particleEmitter(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Component component = (Component)luaObject;
		if (component == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name particleEmitter");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index particleEmitter on a nil value");
			}
		}
		LuaScriptMgr.Push(L, component.particleEmitter);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_particleSystem(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Component component = (Component)luaObject;
		if (component == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name particleSystem");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index particleSystem on a nil value");
			}
		}
		LuaScriptMgr.Push(L, component.particleSystem);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_gameObject(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Component component = (Component)luaObject;
		if (component == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name gameObject");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index gameObject on a nil value");
			}
		}
		LuaScriptMgr.Push(L, component.gameObject);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_tag(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Component component = (Component)luaObject;
		if (component == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name tag");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index tag on a nil value");
			}
		}
		LuaScriptMgr.Push(L, component.tag);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int set_tag(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		Component component = (Component)luaObject;
		if (component == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name tag");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index tag on a nil value");
			}
		}
		component.tag = LuaScriptMgr.GetString(L, 3);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetComponent(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 2 && LuaScriptMgr.CheckTypes(L, 1, typeof(Component), typeof(string)))
		{
			Component component = (Component)LuaScriptMgr.GetUnityObjectSelf(L, 1, "Component");
			string @string = LuaScriptMgr.GetString(L, 2);
			Component component2 = component.GetComponent(@string);
			LuaScriptMgr.Push(L, component2);
			return 1;
		}
		if (num == 2 && LuaScriptMgr.CheckTypes(L, 1, typeof(Component), typeof(Type)))
		{
			Component component3 = (Component)LuaScriptMgr.GetUnityObjectSelf(L, 1, "Component");
			Type typeObject = LuaScriptMgr.GetTypeObject(L, 2);
			Component component4 = component3.GetComponent(typeObject);
			LuaScriptMgr.Push(L, component4);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: Component.GetComponent");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetComponentInChildren(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Component component = (Component)LuaScriptMgr.GetUnityObjectSelf(L, 1, "Component");
		Type typeObject = LuaScriptMgr.GetTypeObject(L, 2);
		Component componentInChildren = component.GetComponentInChildren(typeObject);
		LuaScriptMgr.Push(L, componentInChildren);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetComponentsInChildren(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 2)
		{
			Component component = (Component)LuaScriptMgr.GetUnityObjectSelf(L, 1, "Component");
			Type typeObject = LuaScriptMgr.GetTypeObject(L, 2);
			Component[] componentsInChildren = component.GetComponentsInChildren(typeObject);
			LuaScriptMgr.PushArray(L, componentsInChildren);
			return 1;
		}
		if (num == 3)
		{
			Component component2 = (Component)LuaScriptMgr.GetUnityObjectSelf(L, 1, "Component");
			Type typeObject2 = LuaScriptMgr.GetTypeObject(L, 2);
			bool boolean = LuaScriptMgr.GetBoolean(L, 3);
			Component[] componentsInChildren2 = component2.GetComponentsInChildren(typeObject2, boolean);
			LuaScriptMgr.PushArray(L, componentsInChildren2);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: Component.GetComponentsInChildren");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetComponentInParent(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Component component = (Component)LuaScriptMgr.GetUnityObjectSelf(L, 1, "Component");
		Type typeObject = LuaScriptMgr.GetTypeObject(L, 2);
		Component componentInParent = component.GetComponentInParent(typeObject);
		LuaScriptMgr.Push(L, componentInParent);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetComponentsInParent(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 2)
		{
			Component component = (Component)LuaScriptMgr.GetUnityObjectSelf(L, 1, "Component");
			Type typeObject = LuaScriptMgr.GetTypeObject(L, 2);
			Component[] componentsInParent = component.GetComponentsInParent(typeObject);
			LuaScriptMgr.PushArray(L, componentsInParent);
			return 1;
		}
		if (num == 3)
		{
			Component component2 = (Component)LuaScriptMgr.GetUnityObjectSelf(L, 1, "Component");
			Type typeObject2 = LuaScriptMgr.GetTypeObject(L, 2);
			bool boolean = LuaScriptMgr.GetBoolean(L, 3);
			Component[] componentsInParent2 = component2.GetComponentsInParent(typeObject2, boolean);
			LuaScriptMgr.PushArray(L, componentsInParent2);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: Component.GetComponentsInParent");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetComponents(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 2)
		{
			Component component = (Component)LuaScriptMgr.GetUnityObjectSelf(L, 1, "Component");
			Type typeObject = LuaScriptMgr.GetTypeObject(L, 2);
			Component[] components = component.GetComponents(typeObject);
			LuaScriptMgr.PushArray(L, components);
			return 1;
		}
		if (num == 3)
		{
			Component component2 = (Component)LuaScriptMgr.GetUnityObjectSelf(L, 1, "Component");
			Type typeObject2 = LuaScriptMgr.GetTypeObject(L, 2);
			List<Component> results = (List<Component>)LuaScriptMgr.GetNetObject(L, 3, typeof(List<Component>));
			component2.GetComponents(typeObject2, results);
			return 0;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: Component.GetComponents");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int CompareTag(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Component component = (Component)LuaScriptMgr.GetUnityObjectSelf(L, 1, "Component");
		string luaString = LuaScriptMgr.GetLuaString(L, 2);
		bool b = component.CompareTag(luaString);
		LuaScriptMgr.Push(L, b);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int SendMessageUpwards(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 2)
		{
			Component component = (Component)LuaScriptMgr.GetUnityObjectSelf(L, 1, "Component");
			string luaString = LuaScriptMgr.GetLuaString(L, 2);
			component.SendMessageUpwards(luaString);
			return 0;
		}
		if (num == 3 && LuaScriptMgr.CheckTypes(L, 1, typeof(Component), typeof(string), typeof(SendMessageOptions)))
		{
			Component component2 = (Component)LuaScriptMgr.GetUnityObjectSelf(L, 1, "Component");
			string @string = LuaScriptMgr.GetString(L, 2);
			SendMessageOptions options = (SendMessageOptions)((int)LuaScriptMgr.GetLuaObject(L, 3));
			component2.SendMessageUpwards(@string, options);
			return 0;
		}
		if (num == 3 && LuaScriptMgr.CheckTypes(L, 1, typeof(Component), typeof(string), typeof(object)))
		{
			Component component3 = (Component)LuaScriptMgr.GetUnityObjectSelf(L, 1, "Component");
			string string2 = LuaScriptMgr.GetString(L, 2);
			object varObject = LuaScriptMgr.GetVarObject(L, 3);
			component3.SendMessageUpwards(string2, varObject);
			return 0;
		}
		if (num == 4)
		{
			Component component4 = (Component)LuaScriptMgr.GetUnityObjectSelf(L, 1, "Component");
			string luaString2 = LuaScriptMgr.GetLuaString(L, 2);
			object varObject2 = LuaScriptMgr.GetVarObject(L, 3);
			SendMessageOptions options2 = (SendMessageOptions)((int)LuaScriptMgr.GetNetObject(L, 4, typeof(SendMessageOptions)));
			component4.SendMessageUpwards(luaString2, varObject2, options2);
			return 0;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: Component.SendMessageUpwards");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int SendMessage(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 2)
		{
			Component component = (Component)LuaScriptMgr.GetUnityObjectSelf(L, 1, "Component");
			string luaString = LuaScriptMgr.GetLuaString(L, 2);
			component.SendMessage(luaString);
			return 0;
		}
		if (num == 3 && LuaScriptMgr.CheckTypes(L, 1, typeof(Component), typeof(string), typeof(SendMessageOptions)))
		{
			Component component2 = (Component)LuaScriptMgr.GetUnityObjectSelf(L, 1, "Component");
			string @string = LuaScriptMgr.GetString(L, 2);
			SendMessageOptions options = (SendMessageOptions)((int)LuaScriptMgr.GetLuaObject(L, 3));
			component2.SendMessage(@string, options);
			return 0;
		}
		if (num == 3 && LuaScriptMgr.CheckTypes(L, 1, typeof(Component), typeof(string), typeof(object)))
		{
			Component component3 = (Component)LuaScriptMgr.GetUnityObjectSelf(L, 1, "Component");
			string string2 = LuaScriptMgr.GetString(L, 2);
			object varObject = LuaScriptMgr.GetVarObject(L, 3);
			component3.SendMessage(string2, varObject);
			return 0;
		}
		if (num == 4)
		{
			Component component4 = (Component)LuaScriptMgr.GetUnityObjectSelf(L, 1, "Component");
			string luaString2 = LuaScriptMgr.GetLuaString(L, 2);
			object varObject2 = LuaScriptMgr.GetVarObject(L, 3);
			SendMessageOptions options2 = (SendMessageOptions)((int)LuaScriptMgr.GetNetObject(L, 4, typeof(SendMessageOptions)));
			component4.SendMessage(luaString2, varObject2, options2);
			return 0;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: Component.SendMessage");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int BroadcastMessage(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 2)
		{
			Component component = (Component)LuaScriptMgr.GetUnityObjectSelf(L, 1, "Component");
			string luaString = LuaScriptMgr.GetLuaString(L, 2);
			component.BroadcastMessage(luaString);
			return 0;
		}
		if (num == 3 && LuaScriptMgr.CheckTypes(L, 1, typeof(Component), typeof(string), typeof(SendMessageOptions)))
		{
			Component component2 = (Component)LuaScriptMgr.GetUnityObjectSelf(L, 1, "Component");
			string @string = LuaScriptMgr.GetString(L, 2);
			SendMessageOptions options = (SendMessageOptions)((int)LuaScriptMgr.GetLuaObject(L, 3));
			component2.BroadcastMessage(@string, options);
			return 0;
		}
		if (num == 3 && LuaScriptMgr.CheckTypes(L, 1, typeof(Component), typeof(string), typeof(object)))
		{
			Component component3 = (Component)LuaScriptMgr.GetUnityObjectSelf(L, 1, "Component");
			string string2 = LuaScriptMgr.GetString(L, 2);
			object varObject = LuaScriptMgr.GetVarObject(L, 3);
			component3.BroadcastMessage(string2, varObject);
			return 0;
		}
		if (num == 4)
		{
			Component component4 = (Component)LuaScriptMgr.GetUnityObjectSelf(L, 1, "Component");
			string luaString2 = LuaScriptMgr.GetLuaString(L, 2);
			object varObject2 = LuaScriptMgr.GetVarObject(L, 3);
			SendMessageOptions options2 = (SendMessageOptions)((int)LuaScriptMgr.GetNetObject(L, 4, typeof(SendMessageOptions)));
			component4.BroadcastMessage(luaString2, varObject2, options2);
			return 0;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: Component.BroadcastMessage");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Lua_Eq(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		UnityEngine.Object x = LuaScriptMgr.GetLuaObject(L, 1) as UnityEngine.Object;
		UnityEngine.Object y = LuaScriptMgr.GetLuaObject(L, 2) as UnityEngine.Object;
		bool b = x == y;
		LuaScriptMgr.Push(L, b);
		return 1;
	}
}
