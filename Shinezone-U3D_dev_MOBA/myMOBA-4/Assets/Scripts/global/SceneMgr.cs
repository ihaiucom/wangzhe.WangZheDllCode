using AGE;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class SceneMgr : MonoSingleton<SceneMgr>
{
	private const string LIMIT_CONFIG_FILE = "Config/ParticleLimit";

	private GameObject[] rootObjs;

	private DictionaryView<string, GameObject> cachedPrefabs = new DictionaryView<string, GameObject>();

	private Dictionary<string, bool> m_resourcesNotExist = new Dictionary<string, bool>();

	public bool m_dynamicLOD;

	private int[] LIMIT_CONFIG = new int[]
	{
		40,
		40
	};

	private string[] emptyActorPrefabs;

	private object[] commonEffects;

	public static string[] lod_postfix = new string[]
	{
		string.Empty,
		"_mid",
		"_low"
	};

	public SceneMgr()
	{
		string[] expr_33 = new string[8];
		expr_33[0] = "Prefab_Characters/EmptyHero";
		expr_33[1] = "Prefab_Characters/EmptyMonster";
		expr_33[3] = "Prefab_Characters/EmptyEye";
		expr_33[4] = "Prefab_Characters/EmptyCall";
		expr_33[5] = "Prefab_Characters/EmptyBullet";
		this.emptyActorPrefabs = expr_33;
		this.commonEffects = new object[]
		{
			"Prefab_Skill_Effects/tongyong_effects/Indicator/lockt_01",
			3,
			"Prefab_Skill_Effects/tongyong_effects/Siwang_tongyong/siwang_tongyong_01",
			5,
			"Prefab_Skill_Effects/tongyong_effects/Sence_Effeft/Chuansong_tishi",
			10,
			"prefab_skill_effects/tongyong_effects/tongyong_hurt/born_back_reborn/huicheng_tongyong_01",
			5,
			"prefab_skill_effects/common_effects/jiasu_tongyong_01",
			8
		};
	}

	public void PrepareGameObjectLOD(string path, bool isParticle, enResourceType type, int count)
	{
		string empty = string.Empty;
		this.GetPrefabLOD(path, isParticle, out empty);
		Singleton<CGameObjectPool>.instance.PrepareGameObject(empty, type, count, true);
	}



	private void InitObjs()
	{
		if (this.rootObjs == null)
		{
			string[] names = Enum.GetNames(typeof(SceneObjType));
			this.rootObjs = new GameObject[names.Length];
			for (int i = 0; i < names.Length; i++)
			{
				GameObject gameObject = new GameObject();
				gameObject.transform.parent = base.gameObject.transform;
				gameObject.name = names[i];
				this.rootObjs[i] = gameObject;
			}
		}
	}

	private void InitConfig()
	{
		CResource resource = Singleton<CResourceManager>.GetInstance().GetResource("Config/ParticleLimit", typeof(TextAsset), enResourceType.Numeric, false, true);
		if (resource == null)
		{
			return;
		}
		CBinaryObject cBinaryObject = resource.m_content as CBinaryObject;
		if (null == cBinaryObject)
		{
			return;
		}
		string text = StringHelper.ASCIIBytesToString(cBinaryObject.m_data);
		string[] array = text.Split(new char[]
		{
			'\r',
			'\n'
		});
		array = Array.FindAll<string>(array, (string x) => !string.IsNullOrEmpty(x));
		for (int i = 0; i < array.Length; i++)
		{
			string text2 = array[i];
			if (!string.IsNullOrEmpty(text2))
			{
				text2 = text2.Trim();
				if (text2.Contains("//"))
				{
					text2 = text2.Substring(0, text2.IndexOf("//"));
				}
				text2 = text2.Trim();
				if (!string.IsNullOrEmpty(text2))
				{
					string[] array2 = text2.Split(new char[]
					{
						':'
					});
					if (array2 == null || array2.Length != 2)
					{
						return;
					}
					int[] array3 = new int[2];
					for (int j = 0; j < array2.Length; j++)
					{
						array3[j] = Mathf.Abs(int.Parse(array2[j]));
					}
					if (array3[0] != 0 && array3[0] != 1)
					{
						return;
					}
					this.LIMIT_CONFIG[i] = array3[1];
				}
			}
		}
	}

	protected override void Init()
	{
		this.InitObjs();
		this.InitConfig();
	}

	public GameObject GetRoot(SceneObjType sceneObjType)
	{
		return this.rootObjs[(int)sceneObjType];
	}

	public void AddToRoot(GameObject obj, SceneObjType sceneObjType)
	{
		if (obj)
		{
			obj.transform.parent = this.rootObjs[(int)sceneObjType].transform;
		}
	}

	public void AddToRoot(GameObject obj, SceneObjType sceneObjType, Vector3 pos, Quaternion rot)
	{
		if (obj)
		{
			obj.transform.parent = this.rootObjs[(int)sceneObjType].transform;
			obj.transform.position = pos;
			obj.transform.rotation = rot;
		}
	}

	public GameObject Spawn(string name, SceneObjType sceneObjType)
	{
		GameObject gameObject = new GameObject();
		gameObject.name = string.Format("{0}({1})", name, gameObject.GetInstanceID());
		gameObject.transform.parent = this.rootObjs[(int)sceneObjType].transform;
		return gameObject;
	}

	public GameObject Spawn(string name, SceneObjType sceneObjType, VInt3 position, VInt3 forward)
	{
		string text = this.emptyActorPrefabs[(int)sceneObjType];
		GameObject gameObject2;
		if (text != null)
		{
			GameObject gameObject = Singleton<CResourceManager>.GetInstance().GetResource(text, typeof(GameObject), enResourceType.BattleScene, true, false).m_content as GameObject;
			gameObject2 = Singleton<CGameObjectPool>.GetInstance().GetGameObject(text, enResourceType.BattleScene);
			if (gameObject2 != null && gameObject != null)
			{
				gameObject2.layer = gameObject.layer;
				gameObject2.tag = gameObject.tag;
			}
		}
		else
		{
			gameObject2 = new GameObject();
		}
		gameObject2.name = name;
		gameObject2.transform.position = (Vector3)position;
		gameObject2.transform.rotation = Quaternion.LookRotation((Vector3)forward);
		gameObject2.transform.parent = this.rootObjs[(int)sceneObjType].transform;
		return gameObject2;
	}

	public GameObject Spawn(string name, SceneObjType sceneObjType, Vector3 position, Vector3 forward)
	{
		return new GameObject
		{
			name = name,
			transform = 
			{
				position = position,
				rotation = Quaternion.LookRotation(forward),
				parent = this.rootObjs[(int)sceneObjType].transform
			}
		};
	}

	public GameObject Spawn(string name, SceneObjType sceneObjType, Vector3 position, Quaternion rotation)
	{
		return new GameObject
		{
			name = name,
			transform = 
			{
				position = position,
				rotation = rotation,
				parent = this.rootObjs[(int)sceneObjType].transform
			}
		};
	}

	private T LoadResource<T>(string path) where T : UnityEngine.Object
	{
		path = CFileManager.EraseExtension(path);
		return Singleton<CResourceManager>.GetInstance().GetResource(path, typeof(T), enResourceType.BattleScene, true, false).m_content as T;
	}

	private int GetDynamicLod(int lod, bool isParticle)
	{
		if (!this.m_dynamicLOD || !isParticle || lod == 2 || this.LIMIT_CONFIG == null || lod >= this.LIMIT_CONFIG.Length || lod < 0)
		{
			return lod;
		}
		int particleActiveNumber = ParticleHelper.GetParticleActiveNumber();
		if (particleActiveNumber >= this.LIMIT_CONFIG[lod])
		{
			return 2;
		}
		return lod;
	}

	public T GetPrefabLOD<T>(string path, bool isParticle, out string realPath) where T : UnityEngine.Object
	{
		int i = (!isParticle) ? GameSettings.ModelLOD : GameSettings.ParticleLOD;
		if (Singleton<BattleLogic>.GetInstance().GetCurLvelContext() != null && !Singleton<BattleLogic>.GetInstance().GetCurLvelContext().IsMobaMode())
		{
			i--;
		}
		i = Mathf.Clamp(i, 0, 2);
		if (GameSettings.DynamicParticleLOD)
		{
			i = this.GetDynamicLod(i, isParticle);
		}
		while (i >= 0)
		{
			string text = path;
			string text2 = SceneMgr.lod_postfix[i];
			if (!string.IsNullOrEmpty(text2))
			{
				text += text2;
			}
			T t = this.LoadResource<T>(text);
			if (t)
			{
				realPath = text;
				return t;
			}
			i--;
		}
		realPath = path;
		this.m_resourcesNotExist.Add(path, true);
		return (T)((object)null);
	}

	public UnityEngine.Object GetPrefabLOD(string path, bool isParticle, out string realPath)
	{
		return this.GetPrefabLOD<UnityEngine.Object>(path, isParticle, out realPath);
	}

	public GameObject InstantiateLOD(string prefabName, bool isParticle, SceneObjType sceneObjType)
	{
		string text = null;
		GameObject prefabLOD = this.GetPrefabLOD<GameObject>(prefabName, isParticle, out text);
		if (prefabLOD == null)
		{
			return null;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(prefabLOD) as GameObject;
		if (gameObject)
		{
			gameObject.transform.parent = this.rootObjs[(int)sceneObjType].transform;
		}
		return gameObject;
	}

	public GameObject AddCulling(GameObject obj, string name = "CullingParent")
	{
		if (null == obj)
		{
			return null;
		}
		GameObject gameObject = new GameObject(name);
		if (null == obj.transform.parent)
		{
			gameObject.transform.position = obj.transform.position;
			gameObject.transform.rotation = obj.transform.rotation;
		}
		else
		{
			gameObject.transform.parent = obj.transform.parent;
			gameObject.transform.localPosition = obj.transform.localPosition;
			gameObject.transform.localRotation = obj.transform.localRotation;
		}
		obj.transform.parent = gameObject.transform;
		ObjectCulling objectCulling = gameObject.GetComponent<ObjectCulling>();
		if (null == objectCulling)
		{
			objectCulling = gameObject.AddComponent<ObjectCulling>();
		}
		objectCulling.Init(obj);
		return gameObject;
	}

	public GameObject InstantiateLOD(string prefabName, bool isParticle, SceneObjType sceneObjType, Vector3 pos)
	{
		GameObject gameObject = this.InstantiateLOD(prefabName, isParticle, sceneObjType);
		if (gameObject != null)
		{
			gameObject.transform.position = pos;
		}
		return gameObject;
	}

	public GameObject InstantiateLOD(string prefabName, bool isParticle, SceneObjType sceneObjType, Vector3 pos, Quaternion rot)
	{
		GameObject gameObject = this.InstantiateLOD(prefabName, isParticle, sceneObjType);
		if (gameObject != null)
		{
			gameObject.transform.position = pos;
			gameObject.transform.rotation = rot;
		}
		return gameObject;
	}

	public GameObject GetPooledGameObjLOD(string prefabName, bool isParticle, SceneObjType sceneObjType, Vector3 pos, Quaternion rot)
	{
		bool flag = false;
		return this.GetPooledGameObjLOD(prefabName, isParticle, sceneObjType, pos, rot, out flag);
	}

	public GameObject GetPooledGameObjLOD(string prefabName, bool isParticle, SceneObjType sceneObjType, Vector3 pos, Quaternion rot, out bool isInit)
	{
		return this._GetPooledGameObjLOD(prefabName, isParticle, sceneObjType, pos, rot, true, out isInit);
	}

	public GameObject GetPooledGameObjLOD(string prefabName, bool isParticle, SceneObjType sceneObjType, Vector3 pos)
	{
		bool flag = false;
		return this.GetPooledGameObjLOD(prefabName, isParticle, sceneObjType, pos, out flag);
	}

	public GameObject GetPooledGameObjLOD(string prefabName, bool isParticle, SceneObjType sceneObjType, Vector3 pos, out bool isInit)
	{
		return this._GetPooledGameObjLOD(prefabName, isParticle, sceneObjType, pos, Quaternion.identity, false, out isInit);
	}

	private GameObject _GetPooledGameObjLOD(string prefabName, bool isParticle, SceneObjType sceneObjType, Vector3 pos, Quaternion rot, bool useRotation, out bool isInit)
	{
		string prefabFullPath = null;
		isInit = false;
		if (this.m_resourcesNotExist.ContainsKey(prefabName))
		{
			return null;
		}
		this.GetPrefabLOD<GameObject>(prefabName, isParticle, out prefabFullPath);
		GameObject gameObject;
		if (useRotation)
		{
			gameObject = Singleton<CGameObjectPool>.GetInstance().GetGameObject(prefabFullPath, pos, rot, enResourceType.BattleScene, out isInit);
		}
		else
		{
			gameObject = Singleton<CGameObjectPool>.GetInstance().GetGameObject(prefabFullPath, pos, enResourceType.BattleScene, out isInit);
		}
		if (gameObject != null)
		{
			gameObject.transform.SetParent(this.rootObjs[(int)sceneObjType].transform);
			gameObject.transform.position = pos;
		}
		return gameObject;
	}

	public void ClearAll()
	{
		if (ActionManager.Instance != null)
		{
			ActionManager.Instance.ForceStop();
		}
		if (this.rootObjs != null)
		{
			for (int i = 0; i < this.rootObjs.Length; i++)
			{
				this.ClearObjs((SceneObjType)i);
			}
		}
		this.cachedPrefabs.Clear();
		this.m_resourcesNotExist.Clear();
		UpdateShadowPlane.ClearCache();
		base.StartCoroutine(this.UnloadAssets_Coroutine());
	}
 

    public IEnumerator PreloadCommonAssets()
    {
        if (!string.IsNullOrEmpty(emptyActorPrefabs[0]))
        {
            PrepareGameObjectLOD(emptyActorPrefabs[0], false, enResourceType.BattleScene, 6);
        }
        yield return new CWaitForNextFrame();

        if (!string.IsNullOrEmpty(emptyActorPrefabs[1]))
        {
            PrepareGameObjectLOD(emptyActorPrefabs[1], false, enResourceType.BattleScene, 30);
        }
        yield return new CWaitForNextFrame();

        if (!string.IsNullOrEmpty(emptyActorPrefabs[5]))
        {
            PrepareGameObjectLOD(emptyActorPrefabs[5], false, enResourceType.BattleScene, 50);
        }
        yield return new CWaitForNextFrame();

        if (!string.IsNullOrEmpty(emptyActorPrefabs[3]))
        {
            PrepareGameObjectLOD(emptyActorPrefabs[3], false, enResourceType.BattleScene, 10);
        }
        yield return new CWaitForNextFrame();
    }

    public IEnumerator PreloadCommonEffects()
    {
        var i = 0;
        while (i < commonEffects.Length)
        {
            PrepareGameObjectLOD((string)commonEffects[i], true, enResourceType.BattleScene, (int)commonEffects[i + 1]);
            yield return new CHoldForSecond(0f);
            i += 2;
        }
    }

    private IEnumerator UnloadAssets_Coroutine()
    {
        yield return null;
        yield return Resources.UnloadUnusedAssets();
        GC.Collect();
    }

	private void ClearObjs(SceneObjType type)
	{
		GameObject gameObject = this.rootObjs[(int)type];
		while (gameObject.transform.childCount > 0)
		{
			GameObject gameObject2 = gameObject.transform.GetChild(gameObject.transform.childCount - 1).gameObject;
			gameObject2.transform.parent = null;
			UnityEngine.Object.Destroy(gameObject2);
		}
	}
}
