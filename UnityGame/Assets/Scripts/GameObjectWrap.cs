using com.tencent.pandora;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectWrap
{
	private static Type classType = typeof(GameObject);

	public static void Register(IntPtr L)
	{
		LuaMethod[] regs = new LuaMethod[]
		{
			new LuaMethod("SampleAnimation", new LuaCSFunction(GameObjectWrap.SampleAnimation)),
			new LuaMethod("CreatePrimitive", new LuaCSFunction(GameObjectWrap.CreatePrimitive)),
			new LuaMethod("GetComponent", new LuaCSFunction(GameObjectWrap.GetComponent)),
			new LuaMethod("GetComponentInChildren", new LuaCSFunction(GameObjectWrap.GetComponentInChildren)),
			new LuaMethod("GetComponentInParent", new LuaCSFunction(GameObjectWrap.GetComponentInParent)),
			new LuaMethod("GetComponents", new LuaCSFunction(GameObjectWrap.GetComponents)),
			new LuaMethod("GetComponentsInChildren", new LuaCSFunction(GameObjectWrap.GetComponentsInChildren)),
			new LuaMethod("GetComponentsInParent", new LuaCSFunction(GameObjectWrap.GetComponentsInParent)),
			new LuaMethod("SetActive", new LuaCSFunction(GameObjectWrap.SetActive)),
			new LuaMethod("CompareTag", new LuaCSFunction(GameObjectWrap.CompareTag)),
			new LuaMethod("FindGameObjectWithTag", new LuaCSFunction(GameObjectWrap.FindGameObjectWithTag)),
			new LuaMethod("FindWithTag", new LuaCSFunction(GameObjectWrap.FindWithTag)),
			new LuaMethod("FindGameObjectsWithTag", new LuaCSFunction(GameObjectWrap.FindGameObjectsWithTag)),
			new LuaMethod("SendMessageUpwards", new LuaCSFunction(GameObjectWrap.SendMessageUpwards)),
			new LuaMethod("SendMessage", new LuaCSFunction(GameObjectWrap.SendMessage)),
			new LuaMethod("BroadcastMessage", new LuaCSFunction(GameObjectWrap.BroadcastMessage)),
			new LuaMethod("AddComponent", new LuaCSFunction(GameObjectWrap.AddComponent)),
			new LuaMethod("Find", new LuaCSFunction(GameObjectWrap.Find)),
			new LuaMethod("New", new LuaCSFunction(GameObjectWrap._CreateGameObject)),
			new LuaMethod("GetClassType", new LuaCSFunction(GameObjectWrap.GetClassType)),
			new LuaMethod("__eq", new LuaCSFunction(GameObjectWrap.Lua_Eq))
		};
		LuaField[] fields = new LuaField[]
		{
			new LuaField("isStatic", new LuaCSFunction(GameObjectWrap.get_isStatic), new LuaCSFunction(GameObjectWrap.set_isStatic)),
			new LuaField("transform", new LuaCSFunction(GameObjectWrap.get_transform), null),
			new LuaField("rigidbody", new LuaCSFunction(GameObjectWrap.get_rigidbody), null),
			new LuaField("rigidbody2D", new LuaCSFunction(GameObjectWrap.get_rigidbody2D), null),
			new LuaField("camera", new LuaCSFunction(GameObjectWrap.get_camera), null),
			new LuaField("light", new LuaCSFunction(GameObjectWrap.get_light), null),
			new LuaField("animation", new LuaCSFunction(GameObjectWrap.get_animation), null),
			new LuaField("constantForce", new LuaCSFunction(GameObjectWrap.get_constantForce), null),
			new LuaField("renderer", new LuaCSFunction(GameObjectWrap.get_renderer), null),
			new LuaField("audio", new LuaCSFunction(GameObjectWrap.get_audio), null),
			new LuaField("guiText", new LuaCSFunction(GameObjectWrap.get_guiText), null),
			new LuaField("networkView", new LuaCSFunction(GameObjectWrap.get_networkView), null),
			new LuaField("guiTexture", new LuaCSFunction(GameObjectWrap.get_guiTexture), null),
			new LuaField("collider", new LuaCSFunction(GameObjectWrap.get_collider), null),
			new LuaField("collider2D", new LuaCSFunction(GameObjectWrap.get_collider2D), null),
			new LuaField("hingeJoint", new LuaCSFunction(GameObjectWrap.get_hingeJoint), null),
			new LuaField("particleEmitter", new LuaCSFunction(GameObjectWrap.get_particleEmitter), null),
			new LuaField("particleSystem", new LuaCSFunction(GameObjectWrap.get_particleSystem), null),
			new LuaField("layer", new LuaCSFunction(GameObjectWrap.get_layer), new LuaCSFunction(GameObjectWrap.set_layer)),
			new LuaField("activeSelf", new LuaCSFunction(GameObjectWrap.get_activeSelf), null),
			new LuaField("activeInHierarchy", new LuaCSFunction(GameObjectWrap.get_activeInHierarchy), null),
			new LuaField("tag", new LuaCSFunction(GameObjectWrap.get_tag), new LuaCSFunction(GameObjectWrap.set_tag)),
			new LuaField("gameObject", new LuaCSFunction(GameObjectWrap.get_gameObject), null)
		};
		LuaScriptMgr.RegisterLib(L, "UnityEngine.GameObject", typeof(GameObject), regs, fields, typeof(Object));
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int _CreateGameObject(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 0)
		{
			GameObject obj = new GameObject();
			LuaScriptMgr.Push(L, obj);
			return 1;
		}
		if (num == 1)
		{
			string @string = LuaScriptMgr.GetString(L, 1);
			GameObject obj2 = new GameObject(@string);
			LuaScriptMgr.Push(L, obj2);
			return 1;
		}
		if (LuaScriptMgr.CheckTypes(L, 1, typeof(string)) && LuaScriptMgr.CheckParamsType(L, typeof(Type), 2, num - 1))
		{
			string string2 = LuaScriptMgr.GetString(L, 1);
			Type[] paramsObject = LuaScriptMgr.GetParamsObject<Type>(L, 2, num - 1);
			GameObject obj3 = new GameObject(string2, paramsObject);
			LuaScriptMgr.Push(L, obj3);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: GameObject.New");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, GameObjectWrap.classType);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_isStatic(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		GameObject gameObject = (GameObject)luaObject;
		if (gameObject == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name isStatic");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index isStatic on a nil value");
			}
		}
		LuaScriptMgr.Push(L, gameObject.isStatic);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_transform(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		GameObject gameObject = (GameObject)luaObject;
		if (gameObject == null)
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
		LuaScriptMgr.Push(L, gameObject.transform);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_rigidbody(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		GameObject gameObject = (GameObject)luaObject;
		if (gameObject == null)
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
		LuaScriptMgr.Push(L, gameObject.rigidbody);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_rigidbody2D(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		GameObject gameObject = (GameObject)luaObject;
		if (gameObject == null)
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
		LuaScriptMgr.Push(L, gameObject.rigidbody2D);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_camera(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		GameObject gameObject = (GameObject)luaObject;
		if (gameObject == null)
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
		LuaScriptMgr.Push(L, gameObject.camera);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_light(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		GameObject gameObject = (GameObject)luaObject;
		if (gameObject == null)
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
		LuaScriptMgr.Push(L, gameObject.light);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_animation(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		GameObject gameObject = (GameObject)luaObject;
		if (gameObject == null)
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
		LuaScriptMgr.Push(L, gameObject.animation);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_constantForce(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		GameObject gameObject = (GameObject)luaObject;
		if (gameObject == null)
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
		LuaScriptMgr.Push(L, gameObject.constantForce);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_renderer(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		GameObject gameObject = (GameObject)luaObject;
		if (gameObject == null)
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
		LuaScriptMgr.Push(L, gameObject.renderer);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_audio(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		GameObject gameObject = (GameObject)luaObject;
		if (gameObject == null)
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
		LuaScriptMgr.Push(L, gameObject.audio);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_guiText(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		GameObject gameObject = (GameObject)luaObject;
		if (gameObject == null)
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
		LuaScriptMgr.Push(L, gameObject.guiText);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_networkView(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		GameObject gameObject = (GameObject)luaObject;
		if (gameObject == null)
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
		LuaScriptMgr.Push(L, gameObject.networkView);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_guiTexture(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		GameObject gameObject = (GameObject)luaObject;
		if (gameObject == null)
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
		LuaScriptMgr.Push(L, gameObject.guiTexture);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_collider(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		GameObject gameObject = (GameObject)luaObject;
		if (gameObject == null)
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
		LuaScriptMgr.Push(L, gameObject.collider);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_collider2D(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		GameObject gameObject = (GameObject)luaObject;
		if (gameObject == null)
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
		LuaScriptMgr.Push(L, gameObject.collider2D);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_hingeJoint(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		GameObject gameObject = (GameObject)luaObject;
		if (gameObject == null)
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
		LuaScriptMgr.Push(L, gameObject.hingeJoint);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_particleEmitter(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		GameObject gameObject = (GameObject)luaObject;
		if (gameObject == null)
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
		LuaScriptMgr.Push(L, gameObject.particleEmitter);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_particleSystem(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		GameObject gameObject = (GameObject)luaObject;
		if (gameObject == null)
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
		LuaScriptMgr.Push(L, gameObject.particleSystem);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_layer(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		GameObject gameObject = (GameObject)luaObject;
		if (gameObject == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name layer");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index layer on a nil value");
			}
		}
		LuaScriptMgr.Push(L, gameObject.layer);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_activeSelf(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		GameObject gameObject = (GameObject)luaObject;
		if (gameObject == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name activeSelf");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index activeSelf on a nil value");
			}
		}
		LuaScriptMgr.Push(L, gameObject.activeSelf);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_activeInHierarchy(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		GameObject gameObject = (GameObject)luaObject;
		if (gameObject == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name activeInHierarchy");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index activeInHierarchy on a nil value");
			}
		}
		LuaScriptMgr.Push(L, gameObject.activeInHierarchy);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_tag(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		GameObject gameObject = (GameObject)luaObject;
		if (gameObject == null)
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
		LuaScriptMgr.Push(L, gameObject.tag);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int get_gameObject(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		GameObject gameObject = (GameObject)luaObject;
		if (gameObject == null)
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
		LuaScriptMgr.Push(L, gameObject.gameObject);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int set_isStatic(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		GameObject gameObject = (GameObject)luaObject;
		if (gameObject == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name isStatic");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index isStatic on a nil value");
			}
		}
		gameObject.isStatic = LuaScriptMgr.GetBoolean(L, 3);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int set_layer(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		GameObject gameObject = (GameObject)luaObject;
		if (gameObject == null)
		{
			LuaTypes luaTypes = LuaDLL.lua_type(L, 1);
			if (luaTypes == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name layer");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index layer on a nil value");
			}
		}
		gameObject.layer = (int)LuaScriptMgr.GetNumber(L, 3);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int set_tag(IntPtr L)
	{
		object luaObject = LuaScriptMgr.GetLuaObject(L, 1);
		GameObject gameObject = (GameObject)luaObject;
		if (gameObject == null)
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
		gameObject.tag = LuaScriptMgr.GetString(L, 3);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int SampleAnimation(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 3);
		GameObject gameObject = (GameObject)LuaScriptMgr.GetUnityObjectSelf(L, 1, "GameObject");
		AnimationClip animation = (AnimationClip)LuaScriptMgr.GetUnityObject(L, 2, typeof(AnimationClip));
		float time = (float)LuaScriptMgr.GetNumber(L, 3);
		gameObject.SampleAnimation(animation, time);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int CreatePrimitive(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		PrimitiveType type = (PrimitiveType)((int)LuaScriptMgr.GetNetObject(L, 1, typeof(PrimitiveType)));
		GameObject obj = GameObject.CreatePrimitive(type);
		LuaScriptMgr.Push(L, obj);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetComponent(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 2 && LuaScriptMgr.CheckTypes(L, 1, typeof(GameObject), typeof(string)))
		{
			GameObject gameObject = (GameObject)LuaScriptMgr.GetUnityObjectSelf(L, 1, "GameObject");
			string @string = LuaScriptMgr.GetString(L, 2);
			Component component = gameObject.GetComponent(@string);
			LuaScriptMgr.Push(L, component);
			return 1;
		}
		if (num == 2 && LuaScriptMgr.CheckTypes(L, 1, typeof(GameObject), typeof(Type)))
		{
			GameObject gameObject2 = (GameObject)LuaScriptMgr.GetUnityObjectSelf(L, 1, "GameObject");
			Type typeObject = LuaScriptMgr.GetTypeObject(L, 2);
			Component component2 = gameObject2.GetComponent(typeObject);
			LuaScriptMgr.Push(L, component2);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: GameObject.GetComponent");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetComponentInChildren(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		GameObject gameObject = (GameObject)LuaScriptMgr.GetUnityObjectSelf(L, 1, "GameObject");
		Type typeObject = LuaScriptMgr.GetTypeObject(L, 2);
		Component componentInChildren = gameObject.GetComponentInChildren(typeObject);
		LuaScriptMgr.Push(L, componentInChildren);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetComponentInParent(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		GameObject gameObject = (GameObject)LuaScriptMgr.GetUnityObjectSelf(L, 1, "GameObject");
		Type typeObject = LuaScriptMgr.GetTypeObject(L, 2);
		Component componentInParent = gameObject.GetComponentInParent(typeObject);
		LuaScriptMgr.Push(L, componentInParent);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetComponents(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 2)
		{
			GameObject gameObject = (GameObject)LuaScriptMgr.GetUnityObjectSelf(L, 1, "GameObject");
			Type typeObject = LuaScriptMgr.GetTypeObject(L, 2);
			Component[] components = gameObject.GetComponents(typeObject);
			LuaScriptMgr.PushArray(L, components);
			return 1;
		}
		if (num == 3)
		{
			GameObject gameObject2 = (GameObject)LuaScriptMgr.GetUnityObjectSelf(L, 1, "GameObject");
			Type typeObject2 = LuaScriptMgr.GetTypeObject(L, 2);
			List<Component> results = (List<Component>)LuaScriptMgr.GetNetObject(L, 3, typeof(List<Component>));
			gameObject2.GetComponents(typeObject2, results);
			return 0;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: GameObject.GetComponents");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetComponentsInChildren(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 2)
		{
			GameObject gameObject = (GameObject)LuaScriptMgr.GetUnityObjectSelf(L, 1, "GameObject");
			Type typeObject = LuaScriptMgr.GetTypeObject(L, 2);
			Component[] componentsInChildren = gameObject.GetComponentsInChildren(typeObject);
			LuaScriptMgr.PushArray(L, componentsInChildren);
			return 1;
		}
		if (num == 3)
		{
			GameObject gameObject2 = (GameObject)LuaScriptMgr.GetUnityObjectSelf(L, 1, "GameObject");
			Type typeObject2 = LuaScriptMgr.GetTypeObject(L, 2);
			bool boolean = LuaScriptMgr.GetBoolean(L, 3);
			Component[] componentsInChildren2 = gameObject2.GetComponentsInChildren(typeObject2, boolean);
			LuaScriptMgr.PushArray(L, componentsInChildren2);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: GameObject.GetComponentsInChildren");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int GetComponentsInParent(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 2)
		{
			GameObject gameObject = (GameObject)LuaScriptMgr.GetUnityObjectSelf(L, 1, "GameObject");
			Type typeObject = LuaScriptMgr.GetTypeObject(L, 2);
			Component[] componentsInParent = gameObject.GetComponentsInParent(typeObject);
			LuaScriptMgr.PushArray(L, componentsInParent);
			return 1;
		}
		if (num == 3)
		{
			GameObject gameObject2 = (GameObject)LuaScriptMgr.GetUnityObjectSelf(L, 1, "GameObject");
			Type typeObject2 = LuaScriptMgr.GetTypeObject(L, 2);
			bool boolean = LuaScriptMgr.GetBoolean(L, 3);
			Component[] componentsInParent2 = gameObject2.GetComponentsInParent(typeObject2, boolean);
			LuaScriptMgr.PushArray(L, componentsInParent2);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: GameObject.GetComponentsInParent");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int SetActive(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		GameObject gameObject = (GameObject)LuaScriptMgr.GetUnityObjectSelf(L, 1, "GameObject");
		bool boolean = LuaScriptMgr.GetBoolean(L, 2);
		gameObject.SetActive(boolean);
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int CompareTag(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		GameObject gameObject = (GameObject)LuaScriptMgr.GetUnityObjectSelf(L, 1, "GameObject");
		string luaString = LuaScriptMgr.GetLuaString(L, 2);
		bool b = gameObject.CompareTag(luaString);
		LuaScriptMgr.Push(L, b);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int FindGameObjectWithTag(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string luaString = LuaScriptMgr.GetLuaString(L, 1);
		GameObject obj = GameObject.FindGameObjectWithTag(luaString);
		LuaScriptMgr.Push(L, obj);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int FindWithTag(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string luaString = LuaScriptMgr.GetLuaString(L, 1);
		GameObject obj = GameObject.FindWithTag(luaString);
		LuaScriptMgr.Push(L, obj);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int FindGameObjectsWithTag(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string luaString = LuaScriptMgr.GetLuaString(L, 1);
		GameObject[] o = GameObject.FindGameObjectsWithTag(luaString);
		LuaScriptMgr.PushArray(L, o);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int SendMessageUpwards(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 2)
		{
			GameObject gameObject = (GameObject)LuaScriptMgr.GetUnityObjectSelf(L, 1, "GameObject");
			string luaString = LuaScriptMgr.GetLuaString(L, 2);
			gameObject.SendMessageUpwards(luaString);
			return 0;
		}
		if (num == 3 && LuaScriptMgr.CheckTypes(L, 1, typeof(GameObject), typeof(string), typeof(SendMessageOptions)))
		{
			GameObject gameObject2 = (GameObject)LuaScriptMgr.GetUnityObjectSelf(L, 1, "GameObject");
			string @string = LuaScriptMgr.GetString(L, 2);
			SendMessageOptions options = (SendMessageOptions)((int)LuaScriptMgr.GetLuaObject(L, 3));
			gameObject2.SendMessageUpwards(@string, options);
			return 0;
		}
		if (num == 3 && LuaScriptMgr.CheckTypes(L, 1, typeof(GameObject), typeof(string), typeof(object)))
		{
			GameObject gameObject3 = (GameObject)LuaScriptMgr.GetUnityObjectSelf(L, 1, "GameObject");
			string string2 = LuaScriptMgr.GetString(L, 2);
			object varObject = LuaScriptMgr.GetVarObject(L, 3);
			gameObject3.SendMessageUpwards(string2, varObject);
			return 0;
		}
		if (num == 4)
		{
			GameObject gameObject4 = (GameObject)LuaScriptMgr.GetUnityObjectSelf(L, 1, "GameObject");
			string luaString2 = LuaScriptMgr.GetLuaString(L, 2);
			object varObject2 = LuaScriptMgr.GetVarObject(L, 3);
			SendMessageOptions options2 = (SendMessageOptions)((int)LuaScriptMgr.GetNetObject(L, 4, typeof(SendMessageOptions)));
			gameObject4.SendMessageUpwards(luaString2, varObject2, options2);
			return 0;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: GameObject.SendMessageUpwards");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int SendMessage(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 2)
		{
			GameObject gameObject = (GameObject)LuaScriptMgr.GetUnityObjectSelf(L, 1, "GameObject");
			string luaString = LuaScriptMgr.GetLuaString(L, 2);
			gameObject.SendMessage(luaString);
			return 0;
		}
		if (num == 3 && LuaScriptMgr.CheckTypes(L, 1, typeof(GameObject), typeof(string), typeof(SendMessageOptions)))
		{
			GameObject gameObject2 = (GameObject)LuaScriptMgr.GetUnityObjectSelf(L, 1, "GameObject");
			string @string = LuaScriptMgr.GetString(L, 2);
			SendMessageOptions options = (SendMessageOptions)((int)LuaScriptMgr.GetLuaObject(L, 3));
			gameObject2.SendMessage(@string, options);
			return 0;
		}
		if (num == 3 && LuaScriptMgr.CheckTypes(L, 1, typeof(GameObject), typeof(string), typeof(object)))
		{
			GameObject gameObject3 = (GameObject)LuaScriptMgr.GetUnityObjectSelf(L, 1, "GameObject");
			string string2 = LuaScriptMgr.GetString(L, 2);
			object varObject = LuaScriptMgr.GetVarObject(L, 3);
			gameObject3.SendMessage(string2, varObject);
			return 0;
		}
		if (num == 4)
		{
			GameObject gameObject4 = (GameObject)LuaScriptMgr.GetUnityObjectSelf(L, 1, "GameObject");
			string luaString2 = LuaScriptMgr.GetLuaString(L, 2);
			object varObject2 = LuaScriptMgr.GetVarObject(L, 3);
			SendMessageOptions options2 = (SendMessageOptions)((int)LuaScriptMgr.GetNetObject(L, 4, typeof(SendMessageOptions)));
			gameObject4.SendMessage(luaString2, varObject2, options2);
			return 0;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: GameObject.SendMessage");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int BroadcastMessage(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 2)
		{
			GameObject gameObject = (GameObject)LuaScriptMgr.GetUnityObjectSelf(L, 1, "GameObject");
			string luaString = LuaScriptMgr.GetLuaString(L, 2);
			gameObject.BroadcastMessage(luaString);
			return 0;
		}
		if (num == 3 && LuaScriptMgr.CheckTypes(L, 1, typeof(GameObject), typeof(string), typeof(SendMessageOptions)))
		{
			GameObject gameObject2 = (GameObject)LuaScriptMgr.GetUnityObjectSelf(L, 1, "GameObject");
			string @string = LuaScriptMgr.GetString(L, 2);
			SendMessageOptions options = (SendMessageOptions)((int)LuaScriptMgr.GetLuaObject(L, 3));
			gameObject2.BroadcastMessage(@string, options);
			return 0;
		}
		if (num == 3 && LuaScriptMgr.CheckTypes(L, 1, typeof(GameObject), typeof(string), typeof(object)))
		{
			GameObject gameObject3 = (GameObject)LuaScriptMgr.GetUnityObjectSelf(L, 1, "GameObject");
			string string2 = LuaScriptMgr.GetString(L, 2);
			object varObject = LuaScriptMgr.GetVarObject(L, 3);
			gameObject3.BroadcastMessage(string2, varObject);
			return 0;
		}
		if (num == 4)
		{
			GameObject gameObject4 = (GameObject)LuaScriptMgr.GetUnityObjectSelf(L, 1, "GameObject");
			string luaString2 = LuaScriptMgr.GetLuaString(L, 2);
			object varObject2 = LuaScriptMgr.GetVarObject(L, 3);
			SendMessageOptions options2 = (SendMessageOptions)((int)LuaScriptMgr.GetNetObject(L, 4, typeof(SendMessageOptions)));
			gameObject4.BroadcastMessage(luaString2, varObject2, options2);
			return 0;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: GameObject.BroadcastMessage");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int AddComponent(IntPtr L)
	{
		int num = LuaDLL.lua_gettop(L);
		if (num == 2 && LuaScriptMgr.CheckTypes(L, 1, typeof(GameObject), typeof(Type)))
		{
			GameObject gameObject = (GameObject)LuaScriptMgr.GetUnityObjectSelf(L, 1, "GameObject");
			Type typeObject = LuaScriptMgr.GetTypeObject(L, 2);
			Component obj = gameObject.AddComponent(typeObject);
			LuaScriptMgr.Push(L, obj);
			return 1;
		}
		if (num == 2 && LuaScriptMgr.CheckTypes(L, 1, typeof(GameObject), typeof(string)))
		{
			GameObject gameObject2 = (GameObject)LuaScriptMgr.GetUnityObjectSelf(L, 1, "GameObject");
			string @string = LuaScriptMgr.GetString(L, 2);
			Component obj2 = gameObject2.AddComponent(@string);
			LuaScriptMgr.Push(L, obj2);
			return 1;
		}
		LuaDLL.luaL_error(L, "invalid arguments to method: GameObject.AddComponent");
		return 0;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Find(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		string luaString = LuaScriptMgr.GetLuaString(L, 1);
		GameObject obj = GameObject.Find(luaString);
		LuaScriptMgr.Push(L, obj);
		return 1;
	}

	[MonoPInvokeCallback(typeof(LuaCSFunction))]
	private static int Lua_Eq(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Object x = LuaScriptMgr.GetLuaObject(L, 1) as Object;
		Object y = LuaScriptMgr.GetLuaObject(L, 2) as Object;
		bool b = x == y;
		LuaScriptMgr.Push(L, b);
		return 1;
	}
}
