using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class CGameObjectPool : Singleton<CGameObjectPool>
{
	private class stDelayRecycle
	{
		public GameObject recycleObj;

		public int recycleTime;

		public float[] objSize;

		public Vector3[] objScale;

		public CGameObjectPool.OnDelayRecycleDelegate callback;
	}

	public class ParticleSystemCache
	{
		public ParticleSystem par;

		public bool emmitState;
	}

	public delegate void OnDelayRecycleDelegate(GameObject recycleObj, float[] objSize, Vector3[] scale);

	private DictionaryView<int, Queue<CPooledGameObjectScript>> m_pooledGameObjectMap = new DictionaryView<int, Queue<CPooledGameObjectScript>>();

	private DictionaryView<int, Component> m_componentMap = new DictionaryView<int, Component>();

	private LinkedList<CGameObjectPool.stDelayRecycle> m_delayRecycle = new LinkedList<CGameObjectPool.stDelayRecycle>();

	private GameObject m_poolRoot;

	private bool m_clearPooledObjects;

	private int m_clearPooledObjectsExecuteFrame;

	private static int s_frameCounter;

	public override void Init()
	{
		this.m_poolRoot = new GameObject("CGameObjectPool");
		GameObject gameObject = GameObject.Find("BootObj");
		if (gameObject != null)
		{
			this.m_poolRoot.transform.SetParent(gameObject.transform);
		}
	}

	public override void UnInit()
	{
	}

	public void Update()
	{
		CGameObjectPool.s_frameCounter++;
		this.UpdateDelayRecycle();
		if (this.m_clearPooledObjects && this.m_clearPooledObjectsExecuteFrame == CGameObjectPool.s_frameCounter)
		{
			this.ExecuteClearPooledObjects();
			this.m_clearPooledObjects = false;
		}
	}

	public void ClearPooledObjects()
	{
		this.m_clearPooledObjects = true;
		this.m_clearPooledObjectsExecuteFrame = CGameObjectPool.s_frameCounter + 1;
	}

	public void ExecuteClearPooledObjects()
	{
		for (LinkedListNode<CGameObjectPool.stDelayRecycle> linkedListNode = this.m_delayRecycle.get_First(); linkedListNode != null; linkedListNode = linkedListNode.get_Next())
		{
			if (null != linkedListNode.get_Value().recycleObj)
			{
				this.RecycleGameObject(linkedListNode.get_Value().recycleObj);
			}
		}
		this.m_delayRecycle.Clear();
		this.m_componentMap.Clear();
		DictionaryView<int, Queue<CPooledGameObjectScript>>.Enumerator enumerator = this.m_pooledGameObjectMap.GetEnumerator();
		while (enumerator.MoveNext())
		{
			KeyValuePair<int, Queue<CPooledGameObjectScript>> current = enumerator.Current;
			Queue<CPooledGameObjectScript> value = current.get_Value();
			while (value.get_Count() > 0)
			{
				CPooledGameObjectScript cPooledGameObjectScript = value.Dequeue();
				if (cPooledGameObjectScript != null && cPooledGameObjectScript.gameObject != null)
				{
					Object.Destroy(cPooledGameObjectScript.gameObject);
				}
			}
		}
		this.m_pooledGameObjectMap.Clear();
	}

	public void UpdateParticleChecker(int maxNum)
	{
	}

	private void UpdateDelayRecycle()
	{
		LinkedListNode<CGameObjectPool.stDelayRecycle> linkedListNode = this.m_delayRecycle.get_First();
		int num = (int)(Time.time * 1000f);
		while (linkedListNode != null)
		{
			LinkedListNode<CGameObjectPool.stDelayRecycle> linkedListNode2 = linkedListNode;
			linkedListNode = linkedListNode.get_Next();
			if (null == linkedListNode2.get_Value().recycleObj)
			{
				this.m_delayRecycle.Remove(linkedListNode2);
			}
			else
			{
				if (linkedListNode2.get_Value().recycleTime > num)
				{
					break;
				}
				if (linkedListNode2.get_Value().callback != null)
				{
					linkedListNode2.get_Value().callback(linkedListNode2.get_Value().recycleObj, linkedListNode2.get_Value().objSize, linkedListNode2.get_Value().objScale);
				}
				this.RecycleGameObject(linkedListNode2.get_Value().recycleObj);
				this.m_delayRecycle.Remove(linkedListNode2);
			}
		}
	}

	public GameObject GetGameObject(string prefabFullPath, Vector3 pos, Quaternion rot, enResourceType resourceType)
	{
		bool flag = false;
		return this.GetGameObject(prefabFullPath, pos, rot, true, resourceType, out flag);
	}

	public GameObject GetGameObject(string prefabFullPath, Vector3 pos, Quaternion rot, enResourceType resourceType, out bool isInit)
	{
		return this.GetGameObject(prefabFullPath, pos, rot, true, resourceType, out isInit);
	}

	public GameObject GetGameObject(string prefabFullPath, Vector3 pos, enResourceType resourceType)
	{
		bool flag = false;
		return this.GetGameObject(prefabFullPath, pos, Quaternion.identity, false, resourceType, out flag);
	}

	public GameObject GetGameObject(string prefabFullPath, Vector3 pos, enResourceType resourceType, out bool isInit)
	{
		return this.GetGameObject(prefabFullPath, pos, Quaternion.identity, false, resourceType, out isInit);
	}

	public GameObject GetGameObject(string prefabFullPath, enResourceType resourceType)
	{
		bool flag = false;
		return this.GetGameObject(prefabFullPath, Vector3.zero, Quaternion.identity, false, resourceType, out flag);
	}

	public GameObject GetGameObject(string prefabFullPath, enResourceType resourceType, out bool isInit)
	{
		return this.GetGameObject(prefabFullPath, Vector3.zero, Quaternion.identity, false, resourceType, out isInit);
	}

	public T GetCachedComponent<T>(GameObject go, bool autoAdd = false) where T : Component
	{
		if (null == go)
		{
			return (T)((object)null);
		}
		Component component = null;
		if (this.m_componentMap.TryGetValue(go.GetInstanceID(), out component) && (!autoAdd || component != null))
		{
			return component as T;
		}
		component = go.GetComponent<T>();
		if (autoAdd && component == null)
		{
			component = go.AddComponent<T>();
		}
		this.m_componentMap[go.GetInstanceID()] = component;
		if (null == component)
		{
			return (T)((object)null);
		}
		return component as T;
	}

	private GameObject GetGameObject(string prefabFullPath, Vector3 pos, Quaternion rot, bool useRotation, enResourceType resourceType, out bool isInit)
	{
		string text = CFileManager.EraseExtension(prefabFullPath);
		Queue<CPooledGameObjectScript> queue = null;
		if (!this.m_pooledGameObjectMap.TryGetValue(text.JavaHashCodeIgnoreCase(), out queue))
		{
			queue = new Queue<CPooledGameObjectScript>();
			this.m_pooledGameObjectMap.Add(text.JavaHashCodeIgnoreCase(), queue);
		}
		CPooledGameObjectScript cPooledGameObjectScript = null;
		while (queue.get_Count() > 0)
		{
			cPooledGameObjectScript = queue.Dequeue();
			if (cPooledGameObjectScript != null && cPooledGameObjectScript.gameObject != null)
			{
				cPooledGameObjectScript.gameObject.transform.SetParent(null, true);
				cPooledGameObjectScript.gameObject.transform.position = pos;
				cPooledGameObjectScript.gameObject.transform.rotation = rot;
				cPooledGameObjectScript.gameObject.transform.localScale = cPooledGameObjectScript.m_defaultScale;
				break;
			}
			cPooledGameObjectScript = null;
		}
		if (cPooledGameObjectScript == null)
		{
			cPooledGameObjectScript = this.CreateGameObject(prefabFullPath, pos, rot, useRotation, resourceType, text);
		}
		if (cPooledGameObjectScript == null)
		{
			isInit = false;
			return null;
		}
		isInit = cPooledGameObjectScript.m_isInit;
		cPooledGameObjectScript.OnGet();
		return cPooledGameObjectScript.gameObject;
	}

	public void RecycleGameObjectDelay(GameObject pooledGameObject, int delayMillSeconds, CGameObjectPool.OnDelayRecycleDelegate callback = null, float[] objSize = null, Vector3[] objScale = null)
	{
		CGameObjectPool.stDelayRecycle stDelayRecycle = new CGameObjectPool.stDelayRecycle();
		stDelayRecycle.recycleObj = pooledGameObject;
		stDelayRecycle.recycleTime = (int)(Time.time * 1000f) + delayMillSeconds;
		stDelayRecycle.objSize = objSize;
		stDelayRecycle.objScale = objScale;
		stDelayRecycle.callback = callback;
		if (this.m_delayRecycle.get_Count() == 0)
		{
			this.m_delayRecycle.AddLast(stDelayRecycle);
			return;
		}
		for (LinkedListNode<CGameObjectPool.stDelayRecycle> linkedListNode = this.m_delayRecycle.get_Last(); linkedListNode != null; linkedListNode = linkedListNode.get_Previous())
		{
			if (linkedListNode.get_Value().recycleTime < stDelayRecycle.recycleTime)
			{
				this.m_delayRecycle.AddAfter(linkedListNode, stDelayRecycle);
				return;
			}
		}
		this.m_delayRecycle.AddFirst(stDelayRecycle);
	}

	public void RecycleGameObject(GameObject pooledGameObject)
	{
		this._RecycleGameObject(pooledGameObject, false);
	}

	public void RecyclePreparedGameObject(GameObject pooledGameObject)
	{
		this._RecycleGameObject(pooledGameObject, true);
	}

	private void _RecycleGameObject(GameObject pooledGameObject, bool setIsInit)
	{
		if (pooledGameObject == null)
		{
			return;
		}
		CPooledGameObjectScript component = pooledGameObject.GetComponent<CPooledGameObjectScript>();
		if (component != null)
		{
			Queue<CPooledGameObjectScript> queue = null;
			if (this.m_pooledGameObjectMap.TryGetValue(component.m_prefabKey.JavaHashCodeIgnoreCase(), out queue))
			{
				queue.Enqueue(component);
				component.OnRecycle();
				component.transform.SetParent(this.m_poolRoot.transform, true);
				component.m_isInit = setIsInit;
				return;
			}
		}
		Object.Destroy(pooledGameObject);
	}

	public void PrepareGameObject(string prefabFullPath, enResourceType resourceType, int amount, bool assertNull = true)
	{
		string text = CFileManager.EraseExtension(prefabFullPath);
		Queue<CPooledGameObjectScript> queue = null;
		if (!this.m_pooledGameObjectMap.TryGetValue(text.JavaHashCodeIgnoreCase(), out queue))
		{
			queue = new Queue<CPooledGameObjectScript>();
			this.m_pooledGameObjectMap.Add(text.JavaHashCodeIgnoreCase(), queue);
		}
		if (queue.get_Count() >= amount)
		{
			return;
		}
		amount -= queue.get_Count();
		for (int i = 0; i < amount; i++)
		{
			CPooledGameObjectScript cPooledGameObjectScript = this.CreateGameObject(prefabFullPath, Vector3.zero, Quaternion.identity, false, resourceType, text);
			if (assertNull)
			{
				DebugHelper.Assert(cPooledGameObjectScript != null, "Failed Create Game object from \"{0}\"", new object[]
				{
					prefabFullPath
				});
			}
			if (cPooledGameObjectScript != null)
			{
				queue.Enqueue(cPooledGameObjectScript);
				cPooledGameObjectScript.gameObject.transform.SetParent(this.m_poolRoot.transform, true);
				cPooledGameObjectScript.OnPrepare();
			}
		}
	}

	private CPooledGameObjectScript CreateGameObject(string prefabFullPath, Vector3 pos, Quaternion rot, bool useRotation, enResourceType resourceType, string prefabKey)
	{
		bool needCached = resourceType == enResourceType.BattleScene;
		GameObject gameObject = Singleton<CResourceManager>.GetInstance().GetResource(prefabFullPath, typeof(GameObject), resourceType, needCached, false).m_content as GameObject;
		if (gameObject == null)
		{
			return null;
		}
		GameObject gameObject2;
		if (useRotation)
		{
			gameObject2 = (Object.Instantiate(gameObject, pos, rot) as GameObject);
		}
		else
		{
			gameObject2 = (Object.Instantiate(gameObject) as GameObject);
			gameObject2.transform.position = pos;
		}
		DebugHelper.Assert(gameObject2 != null);
		CPooledGameObjectScript cPooledGameObjectScript = gameObject2.GetComponent<CPooledGameObjectScript>();
		if (cPooledGameObjectScript == null)
		{
			cPooledGameObjectScript = gameObject2.AddComponent<CPooledGameObjectScript>();
		}
		cPooledGameObjectScript.Initialize(prefabKey);
		cPooledGameObjectScript.OnCreate();
		return cPooledGameObjectScript;
	}
}
