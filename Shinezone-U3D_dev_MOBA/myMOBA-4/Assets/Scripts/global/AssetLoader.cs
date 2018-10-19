using System;
using UnityEngine;

public class AssetLoader
{
	public bool preloading = true;

	public virtual UnityEngine.Object LoadAge(string path)
	{
		return Singleton<CResourceManager>.GetInstance().GetResource(path + ".xml", typeof(TextAsset), enResourceType.BattleScene, false, false).m_content;
	}

	public virtual UnityEngine.Object Load(string path)
	{
		return Resources.Load(path);
	}

	public virtual UnityEngine.Object Instantiate(UnityEngine.Object original)
	{
		return UnityEngine.Object.Instantiate(original);
	}

	public virtual UnityEngine.Object Instantiate(UnityEngine.Object original, Vector3 position, Quaternion rotation)
	{
		return UnityEngine.Object.Instantiate(original, position, rotation);
	}

	public virtual void DestroyObject(UnityEngine.Object obj)
	{
		if (obj is GameObject)
		{
			Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(obj as GameObject);
			return;
		}
		UnityEngine.Object.Destroy(obj);
	}
}
