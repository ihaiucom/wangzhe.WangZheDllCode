using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Apollo
{
	internal sealed class ApolloObjectManager : MonoBehaviour
	{
		private DictionaryView<ulong, ApolloObject> dictObjectCollection = new DictionaryView<ulong, ApolloObject>();

		private static GameObject container;

		private static ApolloObjectManager instance;

		private static bool init;

		private ListView<ApolloObject> removedUpdatableList = new ListView<ApolloObject>();

		private ListView<ApolloObject> removedReflectibleList = new ListView<ApolloObject>();

		private ListView<ApolloObject> acceptUpdatedObjectList = new ListView<ApolloObject>();

		public static ApolloObjectManager Instance
		{
			get
			{
				if (!ApolloObjectManager.init)
				{
					ApolloObjectManager.init = true;
					if (ApolloObjectManager.container == null)
					{
						ApolloObjectManager.container = new GameObject();
						UnityEngine.Object.DontDestroyOnLoad(ApolloObjectManager.container);
					}
					ApolloObjectManager.instance = (ApolloObjectManager.container.AddComponent(typeof(ApolloObjectManager)) as ApolloObjectManager);
				}
				return ApolloObjectManager.instance;
			}
		}

		private ApolloObjectManager()
		{
			ApolloObjectManager.setApolloSendMessageCallback(new ApolloSendMessageDelegate(ApolloObjectManager.onSendMessage));
			ApolloObjectManager.setApolloSendStructCallback(new ApolloSendStructDelegate(ApolloObjectManager.onSendStruct));
			ApolloObjectManager.setApolloSendResultCallback(new ApolloSendResultDelegate(ApolloObjectManager.onSendResult));
			ApolloObjectManager.setApolloSendBufferCallback(new ApolloSendBufferDelegate(ApolloObjectManager.onSendBuffer));
			ApolloObjectManager.setApolloSendResultBufferCallback(new ApolloSendResultBufferDelegate(ApolloObjectManager.onSendResultBuffer));
		}

		public void AddObject(ApolloObject obj)
		{
			if (obj == null)
			{
				return;
			}
			if (!this.dictObjectCollection.ContainsKey(obj.ObjectId))
			{
				this.dictObjectCollection.Add(obj.ObjectId, obj);
				ApolloObjectManager.addApolloObject(obj.ObjectId, obj.GetType().FullName);
			}
		}

		public void RemoveObject(ApolloObject obj)
		{
			if (obj == null)
			{
				return;
			}
			if (this.dictObjectCollection.ContainsKey(obj.ObjectId))
			{
				this.dictObjectCollection.Remove(obj.ObjectId);
				ApolloObjectManager.removeApolloObject(obj.ObjectId);
			}
		}

		public void ClearObjects()
		{
			foreach (ulong current in this.dictObjectCollection.Keys)
			{
				ApolloObject apolloObject = this.dictObjectCollection[current];
				ApolloObjectManager.removeApolloObject(apolloObject.ObjectId);
			}
			this.dictObjectCollection.Clear();
		}

		[MonoPInvokeCallback(typeof(ApolloSendMessageDelegate))]
		private static void onSendMessage(ulong objectId, [MarshalAs(UnmanagedType.LPStr)] string function, [MarshalAs(UnmanagedType.LPStr)] string param)
		{
			if (!ApolloObjectManager.Instance.dictObjectCollection.ContainsKey(objectId))
			{
				ADebug.LogError(string.Concat(new object[]
				{
					"onSendMessage not exist: ",
					objectId,
					" function:",
					function,
					" param:",
					param
				}));
				return;
			}
			ApolloObject apolloObject = ApolloObjectManager.Instance.dictObjectCollection[objectId];
			if (apolloObject != null && function != null)
			{
				Type type = apolloObject.GetType();
				MethodInfo method = type.GetMethod(function, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreReturn, null, new Type[]
				{
					typeof(string)
				}, null);
				if (method != null)
				{
					method.Invoke(apolloObject, new object[]
					{
						param
					});
				}
				else
				{
					ADebug.LogError("onSendMessage not exist method:" + function);
				}
			}
			else
			{
				ADebug.Log("onSendMessage:" + objectId + " do not exist");
			}
		}

		[MonoPInvokeCallback(typeof(ApolloSendResultDelegate))]
		private static void onSendResult(ulong objectId, [MarshalAs(UnmanagedType.LPStr)] string function, int result)
		{
			ApolloObject apolloObject = ApolloObjectManager.Instance.dictObjectCollection[objectId];
			if (apolloObject != null && function != null)
			{
				Type type = apolloObject.GetType();
				MethodInfo method = type.GetMethod(function, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreReturn, null, new Type[]
				{
					typeof(int)
				}, null);
				if (method != null)
				{
					method.Invoke(apolloObject, new object[]
					{
						result
					});
				}
				else
				{
					ADebug.LogError("onSendResult not exist method:" + function + " " + type.FullName);
				}
			}
			else
			{
				ADebug.LogError("onSendResult:" + objectId + " do not exist");
			}
		}

		[MonoPInvokeCallback(typeof(ApolloSendStructDelegate))]
		private static void onSendStruct(ulong objectId, [MarshalAs(UnmanagedType.LPStr)] string function, IntPtr param)
		{
			ApolloObject apolloObject = ApolloObjectManager.Instance.dictObjectCollection[objectId];
			if (apolloObject != null && function != null)
			{
				Type type = apolloObject.GetType();
				MethodInfo method = type.GetMethod(function, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreReturn, null, new Type[]
				{
					typeof(IntPtr)
				}, null);
				if (method != null)
				{
					method.Invoke(apolloObject, new object[]
					{
						param
					});
				}
				else
				{
					ADebug.LogError("onSendStruct not exist method:" + function + " " + type.FullName);
				}
			}
			else
			{
				ADebug.LogError("onSendStruct:" + objectId + " do not exist");
			}
		}

		[MonoPInvokeCallback(typeof(ApolloSendBufferDelegate))]
		private static void onSendBuffer(ulong objectId, [MarshalAs(UnmanagedType.LPStr)] string function, IntPtr buffer, int len)
		{
			ApolloObject apolloObject = ApolloObjectManager.Instance.dictObjectCollection[objectId];
			if (apolloObject != null && function != null)
			{
				Type type = apolloObject.GetType();
				MethodInfo method = type.GetMethod(function, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreReturn, null, new Type[]
				{
					typeof(byte[])
				}, null);
				if (method != null)
				{
					byte[] array = new byte[len];
					Marshal.Copy(buffer, array, 0, len);
					method.Invoke(apolloObject, new object[]
					{
						array
					});
				}
				else
				{
					ADebug.LogError("onSendBuffer not exist method:" + function + " " + type.FullName);
				}
			}
			else
			{
				ADebug.LogError("onSendBuffer:" + objectId + " do not exist");
			}
		}

		[MonoPInvokeCallback(typeof(ApolloSendResultBufferDelegate))]
		private static void onSendResultBuffer(ulong objectId, [MarshalAs(UnmanagedType.LPStr)] string function, int result, IntPtr buffer, int len)
		{
			ApolloObject apolloObject = ApolloObjectManager.Instance.dictObjectCollection[objectId];
			if (apolloObject != null && function != null)
			{
				Type type = apolloObject.GetType();
				MethodInfo method = type.GetMethod(function, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreReturn, null, new Type[]
				{
					typeof(int),
					typeof(byte[])
				}, null);
				if (method != null)
				{
					byte[] array = new byte[len];
					if (buffer != IntPtr.Zero && len > 0)
					{
						Marshal.Copy(buffer, array, 0, len);
					}
					method.Invoke(apolloObject, new object[]
					{
						result,
						array
					});
				}
				else
				{
					ADebug.LogError("onSendResultBuffer not exist method:" + function + " " + type.FullName);
				}
			}
			else
			{
				ADebug.LogError("onSendResultBuffer:" + objectId + " do not exist");
			}
		}

		[DllImport("apollo", CallingConvention = CallingConvention.Cdecl)]
		private static extern void addApolloObject(ulong objectId, [MarshalAs(UnmanagedType.LPStr)] string objName);

		[DllImport("apollo", CallingConvention = CallingConvention.Cdecl)]
		private static extern void removeApolloObject(ulong objectId);

		[DllImport("apollo", CallingConvention = CallingConvention.Cdecl)]
		private static extern void setApolloSendMessageCallback([MarshalAs(UnmanagedType.FunctionPtr)] ApolloSendMessageDelegate callback);

		[DllImport("apollo", CallingConvention = CallingConvention.Cdecl)]
		private static extern void setApolloSendStructCallback([MarshalAs(UnmanagedType.FunctionPtr)] ApolloSendStructDelegate callback);

		[DllImport("apollo", CallingConvention = CallingConvention.Cdecl)]
		private static extern void setApolloSendResultCallback([MarshalAs(UnmanagedType.FunctionPtr)] ApolloSendResultDelegate callback);

		[DllImport("apollo", CallingConvention = CallingConvention.Cdecl)]
		private static extern void setApolloSendBufferCallback([MarshalAs(UnmanagedType.FunctionPtr)] ApolloSendBufferDelegate callback);

		[DllImport("apollo", CallingConvention = CallingConvention.Cdecl)]
		private static extern void setApolloSendResultBufferCallback([MarshalAs(UnmanagedType.FunctionPtr)] ApolloSendResultBufferDelegate callback);

		private void Awake()
		{
		}

		public void Update()
		{
			for (int i = 0; i < this.acceptUpdatedObjectList.Count; i++)
			{
				ApolloObject apolloObject = this.acceptUpdatedObjectList[i];
				if (apolloObject.Removable)
				{
					this.removedUpdatableList.Add(apolloObject);
				}
				else
				{
					apolloObject.Update();
				}
			}
			for (int j = 0; j < this.removedUpdatableList.Count; j++)
			{
				ApolloObject apolloObject2 = this.removedUpdatableList[j];
				if (apolloObject2 != null)
				{
					this.RemoveAcceptUpdatedObject(apolloObject2);
				}
			}
			this.removedUpdatableList.Clear();
			DictionaryView<ulong, ApolloObject>.Enumerator enumerator = this.dictObjectCollection.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<ulong, ApolloObject> current = enumerator.Current;
				ApolloObject value = current.Value;
				if (value.Removable)
				{
					this.removedReflectibleList.Add(value);
				}
			}
			for (int k = 0; k < this.removedReflectibleList.Count; k++)
			{
				ApolloObject apolloObject3 = this.removedReflectibleList[k];
				if (apolloObject3 != null)
				{
					this.RemoveObject(apolloObject3);
				}
			}
			this.removedReflectibleList.Clear();
		}

		public void AddAcceptUpdatedObject(ApolloObject obj)
		{
			if (obj != null && !this.acceptUpdatedObjectList.Contains(obj))
			{
				this.acceptUpdatedObjectList.Add(obj);
			}
		}

		public void RemoveAcceptUpdatedObject(ApolloObject obj)
		{
			if (obj != null && this.acceptUpdatedObjectList.Contains(obj))
			{
				this.acceptUpdatedObjectList.Remove(obj);
			}
		}

		public void OnApplicationQuit()
		{
			ADebug.Log("ObjectManager OnApplicationQuit");
			for (int i = 0; i < this.acceptUpdatedObjectList.Count; i++)
			{
				ApolloObject apolloObject = this.acceptUpdatedObjectList[i];
				apolloObject.OnApplicationQuit();
			}
			this.acceptUpdatedObjectList.Clear();
			this.ClearObjects();
			ApolloObjectManager.apollo_quit();
		}

		public void OnApplicationPause(bool pauseStatus)
		{
			ADebug.Log("ObjectManager OnApplicationPause:" + pauseStatus);
			for (int i = 0; i < this.acceptUpdatedObjectList.Count; i++)
			{
				ApolloObject apolloObject = this.acceptUpdatedObjectList[i];
				apolloObject.OnApplicationPause(pauseStatus);
			}
		}

		public void OnDisable()
		{
			ADebug.Log("ObjectManager OnDisable");
			for (int i = 0; i < this.acceptUpdatedObjectList.Count; i++)
			{
				ApolloObject apolloObject = this.acceptUpdatedObjectList[i];
				apolloObject.OnDisable();
			}
			this.acceptUpdatedObjectList.Clear();
		}

		[DllImport("apollo", CallingConvention = CallingConvention.Cdecl)]
		private static extern void apollo_quit();
	}
}
