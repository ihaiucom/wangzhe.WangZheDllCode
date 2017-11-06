using System;
using System.Collections.Generic;
using UnityEngine;

public class NcEffectBehaviour : MonoBehaviour
{
	public class _RuntimeIntance
	{
		public GameObject m_ParentGameObject;

		public GameObject m_ChildGameObject;

		public _RuntimeIntance(GameObject parentGameObject, GameObject childGameObject)
		{
			this.m_ParentGameObject = parentGameObject;
			this.m_ChildGameObject = childGameObject;
		}
	}

	private static bool m_bShuttingDown;

	private static GameObject m_RootInstance;

	public float m_fUserTag;

	protected MeshFilter m_MeshFilter;

	protected ListView<Material> m_RuntimeMaterials;

	protected Renderer m_renderer;

	public NcEffectBehaviour()
	{
		this.m_MeshFilter = null;
	}

	protected Renderer GetRenderer()
	{
		if (null == this.m_renderer)
		{
			this.m_renderer = base.GetComponent<Renderer>();
		}
		return this.m_renderer;
	}

	public static float GetEngineTime()
	{
		if (Time.time == 0f)
		{
			return 1E-06f;
		}
		return Time.time;
	}

	public static float GetEngineDeltaTime()
	{
		return Time.deltaTime;
	}

	public virtual int GetAnimationState()
	{
		return -1;
	}

	public static GameObject GetRootInstanceEffect()
	{
		if (!NcEffectBehaviour.IsSafe())
		{
			return null;
		}
		if (NcEffectBehaviour.m_RootInstance == null)
		{
			NcEffectBehaviour.m_RootInstance = GameObject.Find("_InstanceObject");
			if (NcEffectBehaviour.m_RootInstance == null)
			{
				NcEffectBehaviour.m_RootInstance = new GameObject("_InstanceObject");
			}
		}
		return NcEffectBehaviour.m_RootInstance;
	}

	protected static void SetActive(GameObject target, bool bActive)
	{
		target.SetActive(bActive);
	}

	protected static void SetActiveRecursively(GameObject target, bool bActive)
	{
		target.SetActive(bActive);
	}

	protected static bool IsActive(GameObject target)
	{
		return target.activeSelf;
	}

	public static void HideNcDelayActive(GameObject tarObj)
	{
		NcEffectBehaviour.SetActiveRecursively(tarObj, false);
	}

	public static Texture[] PreloadTexture(GameObject tarObj)
	{
		if (tarObj == null)
		{
			return new Texture[0];
		}
		List<GameObject> list = new List<GameObject>();
		list.Add(tarObj);
		return NcEffectBehaviour.PreloadTexture(tarObj, list);
	}

	private static Texture[] PreloadTexture(GameObject tarObj, List<GameObject> parentPrefabList)
	{
		if (!NcEffectBehaviour.IsSafe())
		{
			return null;
		}
		Renderer[] componentsInChildren = tarObj.GetComponentsInChildren<Renderer>(true);
		ListView<Texture> listView = new ListView<Texture>();
		Renderer[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			Renderer renderer = array[i];
			if (renderer.sharedMaterials != null && renderer.sharedMaterials.Length > 0)
			{
				Material[] sharedMaterials = renderer.sharedMaterials;
				for (int j = 0; j < sharedMaterials.Length; j++)
				{
					Material material = sharedMaterials[j];
					if (material != null && material.mainTexture != null)
					{
						listView.Add(material.mainTexture);
					}
				}
			}
		}
		NcSpriteTexture[] componentsInChildren2 = tarObj.GetComponentsInChildren<NcSpriteTexture>(true);
		NcSpriteTexture[] array2 = componentsInChildren2;
		for (int k = 0; k < array2.Length; k++)
		{
			NcSpriteTexture ncSpriteTexture = array2[k];
			if (ncSpriteTexture.m_NcSpriteFactoryPrefab != null)
			{
				Texture[] array3 = NcEffectBehaviour.PreloadPrefab(ncSpriteTexture.m_NcSpriteFactoryPrefab, parentPrefabList, false);
				if (array3 != null)
				{
					listView.AddRange(array3);
				}
			}
		}
		NcSpriteFactory[] componentsInChildren3 = tarObj.GetComponentsInChildren<NcSpriteFactory>(true);
		NcSpriteFactory[] array4 = componentsInChildren3;
		for (int l = 0; l < array4.Length; l++)
		{
			NcSpriteFactory ncSpriteFactory = array4[l];
			if (ncSpriteFactory.m_SpriteList != null)
			{
				for (int m = 0; m < ncSpriteFactory.m_SpriteList.get_Count(); m++)
				{
					if (ncSpriteFactory.m_SpriteList.get_Item(m).m_EffectPrefab != null)
					{
						Texture[] array5 = NcEffectBehaviour.PreloadPrefab(ncSpriteFactory.m_SpriteList.get_Item(m).m_EffectPrefab, parentPrefabList, true);
						if (array5 == null)
						{
							ncSpriteFactory.m_SpriteList.get_Item(m).m_EffectPrefab = null;
						}
						else
						{
							listView.AddRange(array5);
						}
						if (ncSpriteFactory.m_SpriteList.get_Item(m).m_AudioClip != null)
						{
						}
					}
				}
			}
		}
		return LinqS.ToArray<Texture>(listView);
	}

	private static Texture[] PreloadPrefab(GameObject tarObj, List<GameObject> parentPrefabList, bool bCheckDup)
	{
		if (!parentPrefabList.Contains(tarObj))
		{
			parentPrefabList.Add(tarObj);
			Texture[] result = NcEffectBehaviour.PreloadTexture(tarObj, parentPrefabList);
			parentPrefabList.Remove(tarObj);
			return result;
		}
		if (bCheckDup)
		{
			string text = string.Empty;
			for (int i = 0; i < parentPrefabList.get_Count(); i++)
			{
				text = text + parentPrefabList.get_Item(i).name + "/";
			}
			Debug.LogWarning("LoadError : Recursive Prefab - " + text + tarObj.name);
			return null;
		}
		return null;
	}

	protected void AddRuntimeMaterial(Material addMaterial)
	{
		if (this.m_RuntimeMaterials == null)
		{
			this.m_RuntimeMaterials = new ListView<Material>();
		}
		if (!this.m_RuntimeMaterials.Contains(addMaterial))
		{
			this.m_RuntimeMaterials.Add(addMaterial);
		}
	}

	public static void AdjustSpeedRuntime(GameObject target, float fSpeedRate)
	{
		NcEffectBehaviour[] componentsInChildren = target.GetComponentsInChildren<NcEffectBehaviour>(true);
		NcEffectBehaviour[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			NcEffectBehaviour ncEffectBehaviour = array[i];
			ncEffectBehaviour.OnUpdateEffectSpeed(fSpeedRate, true);
		}
	}

	public static string GetMaterialColorName(Material mat)
	{
		string[] array = new string[]
		{
			"_Color",
			"_TintColor",
			"_EmisColor"
		};
		if (mat != null)
		{
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string text = array2[i];
				if (mat.HasProperty(text))
				{
					return text;
				}
			}
		}
		return null;
	}

	protected void DisableEmit()
	{
		ParticleSystem[] componentsInChildren = base.gameObject.GetComponentsInChildren<ParticleSystem>(true);
		ParticleSystem[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			ParticleSystem particleSystem = array[i];
			if (particleSystem != null)
			{
				particleSystem.enableEmission = false;
			}
		}
		ParticleEmitter[] componentsInChildren2 = base.gameObject.GetComponentsInChildren<ParticleEmitter>(true);
		ParticleEmitter[] array2 = componentsInChildren2;
		for (int j = 0; j < array2.Length; j++)
		{
			ParticleEmitter particleEmitter = array2[j];
			if (particleEmitter != null)
			{
				particleEmitter.emit = false;
			}
		}
	}

	public static bool IsSafe()
	{
		return !NcEffectBehaviour.m_bShuttingDown;
	}

	protected GameObject CreateEditorGameObject(GameObject srcGameObj)
	{
		return srcGameObj;
	}

	public GameObject CreateGameObject(string name)
	{
		if (!NcEffectBehaviour.IsSafe())
		{
			return null;
		}
		return this.CreateEditorGameObject(new GameObject(name));
	}

	public GameObject CreateGameObject(GameObject original)
	{
		if (!NcEffectBehaviour.IsSafe())
		{
			return null;
		}
		return this.CreateEditorGameObject((GameObject)Object.Instantiate(original));
	}

	public GameObject CreateGameObject(GameObject prefabObj, Vector3 position, Quaternion rotation)
	{
		if (!NcEffectBehaviour.IsSafe())
		{
			return null;
		}
		return this.CreateEditorGameObject((GameObject)Object.Instantiate(prefabObj, position, rotation));
	}

	public GameObject CreateGameObject(GameObject parentObj, GameObject prefabObj)
	{
		if (!NcEffectBehaviour.IsSafe())
		{
			return null;
		}
		GameObject gameObject = this.CreateGameObject(prefabObj);
		if (parentObj != null && gameObject != null)
		{
			this.ChangeParent(parentObj.transform, gameObject.transform, true, null);
		}
		return gameObject;
	}

	public GameObject CreateGameObject(GameObject parentObj, Transform parentTrans, GameObject prefabObj)
	{
		if (!NcEffectBehaviour.IsSafe())
		{
			return null;
		}
		GameObject gameObject = this.CreateGameObject(prefabObj);
		if (parentObj != null && gameObject != null)
		{
			this.ChangeParent(parentObj.transform, gameObject.transform, true, parentTrans);
		}
		return gameObject;
	}

	protected void ChangeParent(Transform newParent, Transform child, bool bKeepingLocalTransform, Transform addTransform)
	{
		NcTransformTool ncTransformTool = null;
		if (bKeepingLocalTransform)
		{
			ncTransformTool = new NcTransformTool(child.transform);
			if (addTransform != null)
			{
				ncTransformTool.AddTransform(addTransform);
			}
		}
		child.parent = newParent;
		if (bKeepingLocalTransform)
		{
			ncTransformTool.CopyToLocalTransform(child.transform);
		}
		if (bKeepingLocalTransform)
		{
		}
	}

	protected void UpdateMeshColors(Color color)
	{
		if (this.m_MeshFilter == null)
		{
			this.m_MeshFilter = (MeshFilter)base.gameObject.GetComponent(typeof(MeshFilter));
		}
		if (this.m_MeshFilter == null || this.m_MeshFilter.sharedMesh == null || this.m_MeshFilter.mesh == null)
		{
			return;
		}
		Color[] array = new Color[this.m_MeshFilter.mesh.vertexCount];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = color;
		}
		this.m_MeshFilter.mesh.colors = array;
	}

	protected virtual void OnDestroy()
	{
		if (this.m_RuntimeMaterials != null)
		{
			foreach (Material current in this.m_RuntimeMaterials)
			{
				Object.Destroy(current);
			}
			this.m_RuntimeMaterials = null;
		}
	}

	public void OnApplicationQuit()
	{
		NcEffectBehaviour.m_bShuttingDown = true;
	}

	public virtual void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
	{
	}

	public virtual void OnUpdateToolData()
	{
	}
}
