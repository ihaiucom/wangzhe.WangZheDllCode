using AGE;
using ResData;
using Assets.Scripts.UI;
using Assets.Scripts.GameLogic.DataCenter;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.Common;
using Assets.Scripts.GameSystem;
using Assets.Scripts.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Assets.Scripts.Sound;
using behaviac;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace Assets.Scripts.GameLogic
{
	public class GameLoader : MonoSingleton<GameLoader>
	{
		private struct LoaderHelperWrapper
		{
			public LoaderHelper loadHelper;

			public int duty;
		}

		private struct LHCWrapper
		{
			public LoaderHelperCamera lhc;

			public LoaderHelper loadHelper;

			public int duty;
		}

		public delegate void LoadProgressDelegate(float progress);

		public delegate void LoadCompleteDelegate();

		private static GameSerializer s_serializer = new GameSerializer();

		private ArrayList levelList = new ArrayList();

		private ArrayList levelDesignList = new ArrayList();

		private ArrayList levelArtistList = new ArrayList();

		private List<string> soundBankList = new List<string>();

		public List<ActorMeta> actorList = new List<ActorMeta>();

		public ListView<ActorConfig> staticActors = new ListView<ActorConfig>();

		private int _nProgress;

		public bool isLoadStart;

		private GameLoader.LoadProgressDelegate LoadProgressEvent;

		private GameLoader.LoadCompleteDelegate LoadCompleteEvent;

		private float coroutineTime;

		private CCoroutine m_handle_PreSpawnSoldiers;

		private CCoroutine m_handle_SpawnDynamicActor;

		private CCoroutine m_handle_SpawnStaticActor;

		private CCoroutine m_handle_LoadAgeRecursiveAssets;

		private CCoroutine m_handle_LoadNoActorAssets;

		private CCoroutine m_handle_LoadActorAssets;

		private CCoroutine m_handle_LoadCommonAssets;

		private CCoroutine m_handle_LoadDesignLevel;

		private CCoroutine m_handle_LoadArtistLevel;

		private CCoroutine m_handle_LoadCommonAssetBundle;

		private CCoroutine m_handle_LoadCommonEffect;

		private CCoroutine m_handle_CoroutineLoad;

		private CCoroutine m_handle_AnalyseResPreload;

		private static Dictionary<string, string> s_vertexShaderMap = new Dictionary<string, string>
		{
			{
				"S_Game_Scene/Cloth_Lightmap_Wind",
				"S_Game_Scene/Light_VertexLit/Cloth_Lightmap_Wind"
			},
			{
				"S_Game_Scene/Cloth_Wind",
				"S_Game_Scene/Light_VertexLit/Cloth_Wind"
			},
			{
				"S_Game_Effects/Scroll2TexBend_LightMap",
				"S_Game_Effects/Light_VertexLit/Scroll2TexBend_LightMap"
			},
			{
				"S_Game_Effects/Scroll2TexBend",
				"S_Game_Effects/Light_VertexLit/Scroll2TexBend"
			},
			{
				"S_Game_Scene/Diffuse_NotFog",
				"S_Game_Scene/Light_VertexLit/Diffuse_NotFog"
			}
		};

		private List<ActorPreloadTab> actorPreload;

		private ActorPreloadTab noActorPreLoad;

		private List<ActorPreloadTab> ageRefAssetsList;

		public int nProgress
		{
			get
			{
				return this._nProgress;
			}
			set
			{
				if (value >= this._nProgress)
				{
					this._nProgress = value;
				}
			}
		}

		public void ResetLoader()
		{
			this.levelList.Clear();
			this.actorList.Clear();
			this.levelDesignList.Clear();
			this.levelArtistList.Clear();
			this.soundBankList.Clear();
			this.staticActors.Clear();
			this._nProgress = 0;
			if (this.isLoadStart)
			{
				Singleton<CCoroutineManager>.GetInstance().StopCoroutine(this.m_handle_PreSpawnSoldiers, true);
				Singleton<CCoroutineManager>.GetInstance().StopCoroutine(this.m_handle_SpawnDynamicActor, true);
				Singleton<CCoroutineManager>.GetInstance().StopCoroutine(this.m_handle_SpawnStaticActor, true);
				Singleton<CCoroutineManager>.GetInstance().StopCoroutine(this.m_handle_LoadAgeRecursiveAssets, true);
				Singleton<CCoroutineManager>.GetInstance().StopCoroutine(this.m_handle_LoadNoActorAssets, true);
				Singleton<CCoroutineManager>.GetInstance().StopCoroutine(this.m_handle_LoadActorAssets, true);
				Singleton<CCoroutineManager>.GetInstance().StopCoroutine(this.m_handle_LoadCommonAssets, true);
				Singleton<CCoroutineManager>.GetInstance().StopCoroutine(this.m_handle_LoadDesignLevel, true);
				Singleton<CCoroutineManager>.GetInstance().StopCoroutine(this.m_handle_LoadArtistLevel, true);
				Singleton<CCoroutineManager>.GetInstance().StopCoroutine(this.m_handle_LoadCommonAssetBundle, true);
				Singleton<CCoroutineManager>.GetInstance().StopCoroutine(this.m_handle_LoadCommonEffect, true);
				Singleton<CCoroutineManager>.GetInstance().StopCoroutine(this.m_handle_CoroutineLoad, true);
				Singleton<CCoroutineManager>.GetInstance().StopCoroutine(this.m_handle_AnalyseResPreload, true);
				this.isLoadStart = false;
			}
		}

		public void AddLevel(string name)
		{
			this.levelList.Add(name);
		}

		public void AddDesignSerializedLevel(string name)
		{
			this.levelDesignList.Add(name);
		}

		public void AddArtistSerializedLevel(string name)
		{
			this.levelArtistList.Add(name);
		}

		public void AddSoundBank(string name)
		{
			this.soundBankList.Add(name);
		}

		public void AddActor(ref ActorMeta actorMeta)
		{
			this.actorList.Add(actorMeta);
		}

		public void AddStaticActor(ActorConfig actor)
		{
			this.staticActors.Add(actor);
		}

		public void Load(GameLoader.LoadProgressDelegate progress, GameLoader.LoadCompleteDelegate finish)
		{
			if (this.isLoadStart)
			{
				return;
			}
			Debug.Log("GameLoader Start Load");
			this.LoadProgressEvent = progress;
			this.LoadCompleteEvent = finish;
			this._nProgress = 0;
			this.isLoadStart = true;
			this.m_handle_CoroutineLoad = Singleton<CCoroutineManager>.GetInstance().StartCoroutine(this.CoroutineLoad());
		}

		private bool ShouldYieldReturn()
		{
			return Time.realtimeSinceStartup - this.coroutineTime > 0.08f;
		}

		private void UpdateProgress(LoaderHelperCamera lhc, int oldProgress, int duty, int index, int count)
		{
			this.coroutineTime = Time.realtimeSinceStartup;
			this.nProgress = oldProgress + duty * index / count;
			this.LoadProgressEvent((float)this.nProgress * 0.0001f);
			if (lhc != null)
			{
				lhc.Update();
			}
		}

	

		private static void ChangeToVertexLit()
		{
			GameObject gameObject = GameObject.Find("Artist");
			Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				Renderer renderer = componentsInChildren[i];
				if (!(null == renderer) && renderer.sharedMaterials != null)
				{
					for (int j = 0; j < renderer.sharedMaterials.Length; j++)
					{
						if (!(null == renderer.sharedMaterials[j]) && !(null == renderer.sharedMaterials[j].shader))
						{
							string text = renderer.sharedMaterials[j].shader.name;
							text = GameLoader.ChangeVertexShader(text);
							renderer.sharedMaterials[j].shader = Shader.Find(text);
						}
					}
				}
			}
		}

		private static string ChangeVertexShader(string oldShader)
		{
			if (GameLoader.s_vertexShaderMap.ContainsKey(oldShader))
			{
				return GameLoader.s_vertexShaderMap[oldShader];
			}
			if (oldShader.Contains("S_Game_Scene/Light/"))
			{
				return oldShader.Replace("S_Game_Scene/Light/", "S_Game_Scene/Light_VertexLit/");
			}
			return oldShader;
		}

        private IEnumerator AnalyseAgeRecursiveAssets(LoaderHelper loadHelper)
        {
            if (ageRefAssetsList == null)
            {
                ageRefAssetsList = new List<ActorPreloadTab>();
            }
            ageRefAssetsList.Clear();
            var i = 0;
            while (i < actorPreload.Count)
            {
                var loadInfo = actorPreload[i];
                var markID = loadInfo.MarkID;
                var configID = loadInfo.theActor.ConfigId;
                var refAssets = loadHelper.GetRefAssets(markID, configID);
                var j = 0;
                while (j < loadInfo.ageActions.Count)
                {
                    AssetLoadBase base2 = loadInfo.ageActions[j];
                    var action = MonoSingleton<ActionManager>.instance.LoadActionResource(base2.assetPath);
                    yield return  new CHoldForSecond(0f);

                    if (action != null)
                    {
                        action.GetAssociatedResources(refAssets, markID);
                    }
                    j++;
                }
                i++;
            }

            i = 0;
            while (i < noActorPreLoad.ageActions.Count)
            {
                AssetLoadBase base3 = noActorPreLoad.ageActions[i];
                var action = MonoSingleton<ActionManager>.instance.LoadActionResource(base3.assetPath);
                yield return  new CHoldForSecond(0f);
                if (action == null)
                {
                    i++;
                    continue;
                }
                var refAssets = loadHelper.GetRefAssets(0, 0);
                action.GetAssociatedResources(refAssets, 0);
                yield  return new CHoldForSecond(0f);

                i++;
            }
            var numPasses = 10;
            i = 0;
            while (i < numPasses)
            {
                var restAssetsList = loadHelper.AnalyseAgeRefAssets(loadHelper.ageRefAssets2);
                ageRefAssetsList.AddRange(restAssetsList);
                loadHelper.ageRefAssets2.Clear();
                var idx = 0;
                while (idx < restAssetsList.Count)
                {
                    var restAssets = restAssetsList[idx];
                    var markID = restAssets.MarkID;
                    var configID = restAssets.theActor.ConfigId;
                    var j = 0;
                    while (j < restAssets.ageActions.Count)
                    {
                        AssetLoadBase base4 = restAssets.ageActions[j];
                        var action = MonoSingleton<ActionManager>.instance.LoadActionResource(base4.assetPath);
                        yield return new CHoldForSecond(0f);

                        if (action != null)
                        {
                            var refAssets = loadHelper.GetRefAssets(markID, configID);
                            action.GetAssociatedResources(refAssets, markID);
                        }
                        j++;
                    }
                    idx++;
                }
                i++;
            }

            yield return new CHoldForSecond(0f);
        }

        private IEnumerator CoroutineLoad()
        {
            yield return new CWaitForNextFrame();
            Singleton<CPlayerSocialInfoController>.GetInstance().ClearAddressBook();
            Singleton<GameDataMgr>.GetInstance().UnloadAllDataBin();
            GC.Collect();
            DynamicShadow.DisableAllDynamicShadows();
            DynamicShadow.InitDefaultGlobalVariables();
            Singleton<CUIManager>.GetInstance().ClearFormPool();
            Singleton<CGameObjectPool>.GetInstance().ClearPooledObjects();
            enResourceType[] resourceTypes = new enResourceType[5];
            resourceTypes[1] = enResourceType.UI3DImage;
            resourceTypes[2] = enResourceType.UIForm;
            resourceTypes[3] = enResourceType.UIPrefab;
            resourceTypes[4] = enResourceType.UISprite;
            Singleton<CResourceManager>.GetInstance().RemoveCachedResources(resourceTypes);
            nProgress = 200;
            LoadProgressEvent(nProgress * 0.0001f);
            yield return new CWaitForNextFrame();
            nProgress = 300;
            LoadProgressEvent(nProgress * 0.0001f);
            yield return  new CWaitForNextFrame();
            nProgress = 400;
            LoadProgressEvent(nProgress * 0.0001f);
            yield return new CWaitForNextFrame();
            if (levelList.Count == 0)
            {
                levelList.Add("EmptyScene");
            }
            PlaneShadowSettings.SetDefault();
            FogOfWarSettings.SetDefault();
            nProgress = 500;
            LoadProgressEvent(nProgress * 0.0001f);
            Singleton<CResourceManager>.GetInstance().UnloadUnusedAssets();
            yield return new CWaitForNextFrame();

            var idx = 0;
            while (idx < levelList.Count)
            {
                var op = Application.LoadLevelAsync((string)levelList[idx]);
                while (!op.isDone)
                {
                    yield return new CHoldForSecond(0f);
                }
                idx++;
            }
            nProgress = 600;
            LoadProgressEvent(nProgress * 0.0001f);
            yield return  new CWaitForNextFrame();
            if (((levelArtistList.Count > 0) || (levelDesignList.Count > 0)) && (Camera.allCameras != null))
            {
                var j = 0;
                while (j < Camera.allCameras.Length)
                {
                    if (Camera.main != null)
                    {
                        Object.Destroy(Camera.allCameras[j].gameObject);
                    }
                    j++;
                }
            }
            nProgress = 0x3e8;
            LoadProgressEvent(nProgress * 0.0001f);
            yield return new CWaitForNextFrame();
            var loadHelper = new LoaderHelper();
            var lhc = new LoaderHelperCamera();
            GameLoader.LoaderHelperWrapper inWrapper = new GameLoader.LoaderHelperWrapper();
            inWrapper.loadHelper = loadHelper;
            inWrapper.duty = 350;
            m_handle_LoadCommonAssetBundle = Singleton<CCoroutineManager>.GetInstance().StartCoroutine(LoadCommonAssetBundle(inWrapper));
            yield return m_handle_LoadCommonAssetBundle;
            
            GameLoader.LoaderHelperWrapper wrapper2 = new GameLoader.LoaderHelperWrapper();
            wrapper2.loadHelper = loadHelper;
            wrapper2.duty = 150;
            m_handle_LoadCommonEffect = Singleton<CCoroutineManager>.GetInstance().StartCoroutine(LoadCommonEffect(wrapper2));
            yield return m_handle_LoadCommonEffect;

            GameLoader.LoaderHelperWrapper wrapper3 = new GameLoader.LoaderHelperWrapper();
            wrapper3.loadHelper = loadHelper;
            wrapper3.duty = 0x3e8;
            m_handle_LoadArtistLevel = Singleton<CCoroutineManager>.GetInstance().StartCoroutine(LoadArtistLevel(wrapper3));
            yield return m_handle_LoadArtistLevel;

            yield return new CWaitForNextFrame();
            ReleaseMemoryIfNeed();
            GameLoader.LoaderHelperWrapper wrapper4 = new GameLoader.LoaderHelperWrapper();
            wrapper4.loadHelper = loadHelper;
            wrapper4.duty = 500;
            m_handle_LoadDesignLevel = Singleton<CCoroutineManager>.GetInstance().StartCoroutine(LoadDesignLevel(wrapper4));
            yield return m_handle_LoadDesignLevel;

            yield return new CWaitForNextFrame();
            ReleaseMemoryIfNeed();
            FogOfWar.PreBeginLevel();
            GameLoader.LoaderHelperWrapper wrapper5 = new GameLoader.LoaderHelperWrapper();
            wrapper5.loadHelper = loadHelper;
            wrapper5.duty = 500;
            m_handle_LoadCommonAssets = Singleton<CCoroutineManager>.GetInstance().StartCoroutine(LoadCommonAssets(wrapper5));
            yield return m_handle_LoadCommonAssets;

            yield return new CWaitForNextFrame();
            ReleaseMemoryIfNeed();
            m_handle_AnalyseResPreload = Singleton<CCoroutineManager>.GetInstance().StartCoroutine(AnalyseResourcePreload(loadHelper));
            yield return m_handle_AnalyseResPreload;

            Singleton<CCoroutineManager>.GetInstance().StartCoroutine(loadHelper.ReduceSkillRefDatabin());
            GameLoader.LHCWrapper wrapper6 = new GameLoader.LHCWrapper();
            wrapper6.lhc = lhc;
            wrapper6.loadHelper = loadHelper;
            wrapper6.duty = 0x9c4;
            m_handle_LoadActorAssets = Singleton<CCoroutineManager>.GetInstance().StartCoroutine(LoadActorAssets(wrapper6));
            yield return m_handle_LoadActorAssets;

            yield return new CWaitForNextFrame();
            ReleaseMemoryIfNeed();
            GameLoader.LHCWrapper wrapper7 = new GameLoader.LHCWrapper();
            wrapper7.lhc = lhc;
            wrapper7.loadHelper = loadHelper;
            wrapper7.duty = 0x3e8;
            m_handle_LoadNoActorAssets = Singleton<CCoroutineManager>.GetInstance().StartCoroutine(LoadNoActorAssets(wrapper7));
            yield return m_handle_LoadNoActorAssets;

            yield return new CWaitForNextFrame();
            ReleaseMemoryIfNeed();
            GameLoader.LHCWrapper wrapper8 = new GameLoader.LHCWrapper();
            wrapper8.lhc = lhc;
            wrapper8.loadHelper = loadHelper;
            wrapper8.duty = 0x76c;
            m_handle_LoadAgeRecursiveAssets = Singleton<CCoroutineManager>.GetInstance().StartCoroutine(LoadAgeRecursiveAssets(wrapper8));
            yield return m_handle_LoadAgeRecursiveAssets;

            yield return new CWaitForNextFrame();
            ReleaseMemoryIfNeed();
            while (!lhc.Update())
            {
                yield return new CWaitForNextFrame();
            }

            lhc.Destroy();
            lhc = null;
            GameLoader.LoaderHelperWrapper wrapper9 = new GameLoader.LoaderHelperWrapper();
            wrapper9.loadHelper = loadHelper;
            wrapper9.duty = 200;
            m_handle_SpawnStaticActor = Singleton<CCoroutineManager>.GetInstance().StartCoroutine(SpawnStaticActor(wrapper9));
            yield return m_handle_SpawnStaticActor;
            yield return new CWaitForNextFrame();

            ReleaseMemoryIfNeed();
            GameLoader.LoaderHelperWrapper wrapper10 = new GameLoader.LoaderHelperWrapper();
            wrapper10.loadHelper = loadHelper;
            wrapper10.duty = 200;
            m_handle_SpawnDynamicActor = Singleton<CCoroutineManager>.GetInstance().StartCoroutine(SpawnDynamicActor(wrapper10));
            yield return m_handle_SpawnDynamicActor;

            yield return new CWaitForNextFrame();
            ReleaseMemoryIfNeed();
            FogOfWar.BeginLevel();
            GC.Collect();
            yield return  new CWaitForNextFrame();

            yield return new CWaitForNextFrame();
            GameLoader.LoaderHelperWrapper wrapper11 = new GameLoader.LoaderHelperWrapper();
            wrapper11.loadHelper = loadHelper;
            wrapper11.duty = 100;
            m_handle_PreSpawnSoldiers = Singleton<CCoroutineManager>.GetInstance().StartCoroutine(PreSpawnSoldiers(wrapper11));
            yield return m_handle_PreSpawnSoldiers;

            yield return new CWaitForNextFrame();
            ReleaseMemoryIfNeed();
            nProgress = 0x2648;
            LoadProgressEvent(nProgress * 0.0001f);
            yield return new CWaitForNextFrame();

            if (GameSettings.AllowOutlineFilter)
            {
                OutlineFilter.EnableOutlineFilter();
            }
            yield return new CWaitForNextFrame();

            Shader.WarmupAllShaders();
            yield return new CWaitForNextFrame();

            Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("ActorInfo");
            Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharIcon");
            Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharBattle");
            Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharShow");
            Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharLoading");
            Singleton<CResourceManager>.GetInstance().UnloadUnusedAssets();
            nProgress = 0x26ac;
            LoadProgressEvent(nProgress * 0.0001f);
            yield return new CWaitForNextFrame();

            yield return new CWaitForNextFrame();
            Singleton<CBattleSystem>.GetInstance().LoadForm(!Singleton<WatchController>.GetInstance().IsWatching ? CBattleSystem.FormType.Fight : CBattleSystem.FormType.Watch);
            if (!Singleton<WatchController>.GetInstance().IsWatching)
            {
                SignalPanel.GetSignalTipsListScript();
            }
            var uiFormKillNotify = KillNotify.GetKillNotifyFormScript();
            if (uiFormKillNotify != null)
            {
                var go = uiFormKillNotify.gameObject.FindChildBFS("KillNotify_New");
                var go2 = go.FindChildBFS("KillNotify_Sub");
                go.CustomSetActive(true);
                go2.CustomSetActive(true);
                var animator = go2.GetComponent<Animator>();
                animator.enabled = true;
                var anims = KillNotifyUT.GetAllAnimations();
                var i = 0;
                while (i < anims.Count)
                {
                    animator.Play(anims[i]);
                    var j = 0;
                    while (j < 6)
                    {
                        animator.Update(0.5f);
                        j++;
                    }
                    yield return new CHoldForSecond(0f);

                    i++;
                }
                animator.enabled = false;
            }
            Singleton<BattleSkillHudControl>.CreateInstance();
            nProgress = 0x2710;
            LoadProgressEvent(nProgress * 0.0001f);
            yield return new CWaitForNextFrame();

            Singleton<GameDataMgr>.GetInstance().ReloadDataBinOnFighting();
            GC.Collect();
            actorPreload = null;
            noActorPreLoad = null;
            ageRefAssetsList.Clear();
            ageRefAssetsList = null;
            isLoadStart = false;
            LoadCompleteEvent();
            Debug.Log("GameLoader Finish Load");
        }

        private IEnumerator LoadActorAssets(GameLoader.LHCWrapper InWrapper)
        {
            var lhc = InWrapper.lhc;
            var duty = InWrapper.duty;
            var _loadHelper___2 = InWrapper.loadHelper;
            var _oldProgress___3 = nProgress;
            var _count___4 = actorPreload.Count;
            var _index___5 = 0;
            var i = 0;
            while (i < actorPreload.Count)
            {
                var _actorMeta___7 = actorPreload[i].theActor;
                var _hostPlayer___8 = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
                if (_hostPlayer___8.PlayerId == _actorMeta___7.PlayerId)
                {
                    var _heroCfgInfo___9 = GameDataMgr.heroDatabin.GetDataByKey((long)_actorMeta___7.ConfigId);
                    var j = 0;
                    while (j < _heroCfgInfo___9.astSkill.Length)
                    {
                        var _skillCfgInfo___11 = GameDataMgr.skillDatabin.GetDataByKey((long)_heroCfgInfo___9.astSkill[j].iSkillID);
                        object[] inParameters = new object[] { _heroCfgInfo___9.astSkill[j].iSkillID };
                        DebugHelper.Assert(_skillCfgInfo___11 != null, "Failed Found skill config id = {0}", inParameters);
                        var iconPath = string.Empty;
                        if (_skillCfgInfo___11 != null)
                        {
                            iconPath = StringHelper.UTF8BytesToString(ref _skillCfgInfo___11.szIconPath);
                        }
                        if (string.IsNullOrEmpty(iconPath))
                        {
                            j++;
                            continue;
                        }
                        iconPath = CUIUtility.s_Sprite_Dynamic_Skill_Dir + iconPath;
                        yield return new CHoldForSecond(0f);

                        Singleton<CGameObjectPool>.GetInstance().PrepareGameObject(iconPath, enResourceType.BattleScene, 1, true);
                        if (iconPath.EndsWith("0"))
                        {
                            iconPath = iconPath.Substring(0, iconPath.Length - 1);
                            Singleton<CGameObjectPool>.GetInstance().PrepareGameObject(iconPath + "1", enResourceType.BattleScene, 1, false);
                            Singleton<CGameObjectPool>.GetInstance().PrepareGameObject(iconPath + "2", enResourceType.BattleScene, 1, false);
                            Singleton<CGameObjectPool>.GetInstance().PrepareGameObject(iconPath + "3", enResourceType.BattleScene, 1, false);
                            Singleton<CGameObjectPool>.GetInstance().PrepareGameObject(iconPath + "4", enResourceType.BattleScene, 1, false);
                        }
                    
                        j++;
                    }
                }
                i++;
            }
            
            var _i___13 = 0;
            while (_i___13 < actorPreload.Count)
            {
                var _loadInfo___14 = actorPreload[_i___13];
                _count___4 += (((_loadInfo___14.parPrefabs.Count + _loadInfo___14.mesPrefabs.Count) + _loadInfo___14.soundBanks.Count) + _loadInfo___14.ageActions.Count) + _loadInfo___14.behaviorXml.Count;
                _i___13++;
            }
            var _keywords___15 = new string[] { "_LIGHT_TEX_ON", "_HURT_EFFECT_ON", "_EFFECT_TEX_ON", "_RIM_COLOR_ON" };
            var allComposites = GameLoader.composite(_keywords___15);
            var _i___17 = 0;
            while (_i___17 < allComposites.Count)
            {
                var _label___18 = string.Empty;
                var _j___19 = 0;
                while (_j___19 < allComposites[_i___17].Count)
                {
                    _label___18 = _label___18 + allComposites[_i___17][_j___19] + ",";
                    _j___19++;
                }
                var _obj0___20 = Singleton<CGameObjectPool>.instance.GetGameObject("HelperResources/Cube", enResourceType.BattleScene);
                var _mat0___21 = new Material(Shader.Find("S_Game_Hero/Hero_Battle (Occlusion)"));
                var _j___22 = 0;
                while (_j___22 < allComposites[_i___17].Count)
                {
                    _mat0___21.EnableKeyword(allComposites[_i___17][_j___22]);
                    _j___22++;
                }
                _obj0___20.renderer.material = _mat0___21;
                var _obj1___23 = Singleton<CGameObjectPool>.instance.GetGameObject("HelperResources/Cube", enResourceType.BattleScene);
                var _mat1___24 = new Material(Shader.Find("S_Game_Hero/Hero_Battle (Shadow) (Occlusion)"));
                var _j___25 = 0;
                while (_j___25 < allComposites[_i___17].Count)
                {
                    _mat1___24.EnableKeyword(allComposites[_i___17][_j___25]);
                    _j___25++;
                }
                _obj1___23.renderer.material = _mat1___24;
                var _obj2___26 = Singleton<CGameObjectPool>.instance.GetGameObject("HelperResources/Cube", enResourceType.BattleScene);
                var _mat2___27 = new Material(Shader.Find("S_Game_Hero/Hero_Battle (Shadow) (Translucent)"));
                var _j___28 = 0;
                while (_j___28 < allComposites[_i___17].Count)
                {
                    _mat2___27.EnableKeyword(allComposites[_i___17][_j___28]);
                    _j___28++;
                }
                _obj2___26.renderer.material = _mat2___27;
                var _obj3___29 = Singleton<CGameObjectPool>.instance.GetGameObject("HelperResources/Cube", enResourceType.BattleScene);
                var _mat3___30 = new Material(Shader.Find("S_Game_Hero/Hero_Battle (Shadow)"));
                var _j___31 = 0;
                while (_j___31 < allComposites[_i___17].Count)
                {
                    _mat3___30.EnableKeyword(allComposites[_i___17][_j___31]);
                    _j___31++;
                }
                _obj3___29.renderer.material = _mat3___30;
                var _obj4___32 = Singleton<CGameObjectPool>.instance.GetGameObject("HelperResources/Cube", enResourceType.BattleScene);
                var _mat4___33 = new Material(Shader.Find("S_Game_Hero/Hero_Battle (Translucent)"));
                var _j___34 = 0;
                while (_j___34 < allComposites[_i___17].Count)
                {
                    _mat4___33.EnableKeyword(allComposites[_i___17][_j___34]);
                    _j___34++;
                }
                _obj4___32.renderer.material = _mat4___33;
                var _obj5___35 = Singleton<CGameObjectPool>.instance.GetGameObject("HelperResources/Cube", enResourceType.BattleScene);
                var _mat5___36 = new Material(Shader.Find("S_Game_Hero/Hero_Battle"));
                var _j___37 = 0;
                while (_j___37 < allComposites[_i___17].Count)
                {
                    _mat5___36.EnableKeyword(allComposites[_i___17][_j___37]);
                    _j___37++;
                }
                _obj5___35.renderer.material = _mat5___36;
                lhc.AddObj("cube0" + _label___18, _obj0___20);
                lhc.AddObj("cube1" + _label___18, _obj1___23);
                lhc.AddObj("cube2" + _label___18, _obj2___26);
                lhc.AddObj("cube3" + _label___18, _obj3___29);
                lhc.AddObj("cube4" + _label___18, _obj4___32);
                lhc.AddObj("cube5" + _label___18, _obj5___35);
                _i___17++;
            }
            var _i___38 = 0;
            while (_i___38 < actorPreload.Count)
            {
                var _loadInfo___40 = actorPreload[_i___38];
                if ((_loadInfo___40.modelPrefab.assetPath == null) || InWrapper.lhc.HasLoaded(_loadInfo___40.modelPrefab.assetPath))
                {
                    _i___38++;
                    continue;
                }
                var _tmpObj___39 = Singleton<CGameObjectPool>.instance.GetGameObject(_loadInfo___40.modelPrefab.assetPath, enResourceType.BattleScene);
                if (_loadInfo___40.modelPrefab.assetPath.ToLower().Contains("prefab_hero"))
                {
                }
                yield return new CHoldForSecond(0f);
                lhc.AddObj(_loadInfo___40.modelPrefab.assetPath, _tmpObj___39);
            
                _i___38++;
            }
            var _i___41 = 0;
            while (_i___41 < actorPreload.Count)
            {
                var _loadInfo___42 = actorPreload[_i___41];
                GameObject _tmpObj___43 = null;
                var _j___44 = 0;
                while (_j___44 < _loadInfo___42.parPrefabs.Count)
                {
                    AssetLoadBase base2 = _loadInfo___42.parPrefabs[_j___44];
                    if (!lhc.HasLoaded(base2.assetPath))
                    {
                        if (GameSettings.DynamicParticleLOD)
                        {
                            var _originalParticleLOD___45 = GameSettings.ParticleLOD;
                            var _targetParticleOLD___46 = _originalParticleLOD___45;
                            if (((Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer() != null) && (_loadInfo___42.theActor.PlayerId == Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer().PlayerId)) && (_targetParticleOLD___46 > 1))
                            {
                                _targetParticleOLD___46 = 1;
                            }
                            var _lod___47 = _targetParticleOLD___46;
                            while (_lod___47 <= 2)
                            {
                                if ((_targetParticleOLD___46 != 0) || (_lod___47 != 1))
                                {
                                    AssetLoadBase base3 = _loadInfo___42.parPrefabs[_j___44];
                                    var _parPathKey___48 = base3.assetPath + "_lod_" + _lod___47;
                                    if (!lhc.HasLoaded(_parPathKey___48))
                                    {
                                        GameSettings.ParticleLOD = _lod___47;
                                        AssetLoadBase base4 = _loadInfo___42.parPrefabs[_j___44];
                                        _tmpObj___43 = MonoSingleton<SceneMgr>.instance.GetPooledGameObjLOD(base4.assetPath, true, SceneObjType.ActionRes, Vector3.zero);
                                        lhc.AddObj(_parPathKey___48, _tmpObj___43);
                                        yield return new CHoldForSecond(0f);
                                    }
                                }

                                _lod___47++;
                            }
                            GameSettings.ParticleLOD = _originalParticleLOD___45;
                        }
                        else
                        {
                            AssetLoadBase base5 = _loadInfo___42.parPrefabs[_j___44];
                            var _parPathKey___49 = base5.assetPath;
                            if (!lhc.HasLoaded(_parPathKey___49))
                            {
                                AssetLoadBase base6 = _loadInfo___42.parPrefabs[_j___44];
                                _tmpObj___43 = MonoSingleton<SceneMgr>.instance.GetPooledGameObjLOD(base6.assetPath, true, SceneObjType.ActionRes, Vector3.zero);
                                lhc.AddObj(_parPathKey___49, _tmpObj___43);
                                yield return new CHoldForSecond(0f);
                            }
                        }
                    }
               
                    _index___5++;
                    UpdateProgress(lhc, _oldProgress___3, duty, _index___5, _count___4);
                    yield return new CHoldForSecond(0f);

                    _j___44++;
                }
                var _j___50 = 0;
                while (_j___50 < _loadInfo___42.mesPrefabs.Count)
                {
                    AssetLoadBase base7 = _loadInfo___42.mesPrefabs[_j___50];
                    if (!lhc.HasLoaded(base7.assetPath))
                    {
                        AssetLoadBase base8 = _loadInfo___42.mesPrefabs[_j___50];
                        _tmpObj___43 = MonoSingleton<SceneMgr>.instance.GetPooledGameObjLOD(base8.assetPath, false, SceneObjType.ActionRes, Vector3.zero);
                        AssetLoadBase base9 = _loadInfo___42.mesPrefabs[_j___50];
                        lhc.AddObj(base9.assetPath, _tmpObj___43);
                        yield return new CHoldForSecond(0f);
                    }
                
                    _index___5++;
                    UpdateProgress(lhc, _oldProgress___3, duty, _index___5, _count___4);
                    yield return new CHoldForSecond(0f);
                    _j___50++;
                }
                var _j___51 = 0;
                while (_j___51 < _loadInfo___42.soundBanks.Count)
                {
                    AssetLoadBase base10 = _loadInfo___42.soundBanks[_j___51];
                    Singleton<CSoundManager>.instance.LoadBank(base10.assetPath, CSoundManager.BankType.Battle);
                    yield return new CHoldForSecond(0f);
                    _index___5++;
                    UpdateProgress(lhc, _oldProgress___3, duty, _index___5, _count___4);
                    yield return new CHoldForSecond(0f);
                    _j___51++;
                }
                var _j___52 = 0;
                while (_j___52 < _loadInfo___42.behaviorXml.Count)
                {
                    AssetLoadBase base11 = _loadInfo___42.behaviorXml[_j___52];
                    var _btPath___53 = base11.assetPath;
                    Workspace.Load(_btPath___53, false);
                    yield return new CHoldForSecond(0f);
                    _index___5++;
                    UpdateProgress(lhc, _oldProgress___3, duty, _index___5, _count___4);
                    yield return new CHoldForSecond(0f);
                    _j___52++;
                }
                _index___5++;
                UpdateProgress(lhc, _oldProgress___3, duty, _index___5, _count___4);
                yield return new CHoldForSecond(0f);

                _i___41++;
            }
            UpdateProgress(lhc, _oldProgress___3, duty, 1, 1);
            yield return new CWaitForNextFrame();
            UpdateProgress(lhc, _oldProgress___3, duty, 1, 1);
            ReleaseMemoryIfNeed();
        }

        private IEnumerator LoadAgeRecursiveAssets(GameLoader.LHCWrapper InWrapper)
        {
            var _lhc___0 = InWrapper.lhc;
            var _duty___1 = InWrapper.duty;
            var _loadHelper___2 = InWrapper.loadHelper;
            var _oldProgress___3 = nProgress;
            var _subDuty___4 = 1;
            var _count___5 = 0;
            var _index___6 = 0;
            var _progress___7 = nProgress;
            var _idx___8 = 0;
            while (_idx___8 < ageRefAssetsList.Count)
            {
                var _restAssets___9 = ageRefAssetsList[_idx___8];
                _count___5 += (_restAssets___9.parPrefabs.Count + _restAssets___9.mesPrefabs.Count) + _restAssets___9.ageActions.Count;
                _idx___8++;
            }
            var _idx___10 = 0;
            while (_idx___10 < ageRefAssetsList.Count)
            {
                var _restAssets___11 = ageRefAssetsList[_idx___10];
                var _markID___12 = _restAssets___11.MarkID;
                var _configID___13 = _restAssets___11.theActor.ConfigId;
                var _j___14 = 0;
                while (_j___14 < _restAssets___11.parPrefabs.Count)
                {
                    GameObject _tmpObj___15 = null;
                    AssetLoadBase base2 = _restAssets___11.parPrefabs[_j___14];
                    var _assetPath___16 = base2.assetPath;
                    if (GameSettings.DynamicParticleLOD)
                    {
                        var _currentParticleLOD___17 = GameSettings.ParticleLOD;
                        var _lod___18 = _currentParticleLOD___17;
                        while (_lod___18 <= 2)
                        {
                            if ((_currentParticleLOD___17 != 0) || (_lod___18 != 1))
                            {
                                var _parPathKey___19 = _assetPath___16 + "_lod_" + _lod___18;
                                if (!_lhc___0.HasLoaded(_parPathKey___19))
                                {
                                    GameSettings.ParticleLOD = _lod___18;
                                    _tmpObj___15 = MonoSingleton<SceneMgr>.instance.GetPooledGameObjLOD(_assetPath___16, true, SceneObjType.ActionRes, Vector3.zero);
                                    _lhc___0.AddObj(_parPathKey___19, _tmpObj___15);
                                    yield return new CHoldForSecond(0f);
                                }
                            }
                       
                            _lod___18++;
                        }
                        GameSettings.ParticleLOD = _currentParticleLOD___17;
                    }
                    else
                    {
                        var _parPathKey___20 = _assetPath___16;
                        if (!_lhc___0.HasLoaded(_parPathKey___20))
                        {
                            _tmpObj___15 = MonoSingleton<SceneMgr>.instance.GetPooledGameObjLOD(_assetPath___16, true, SceneObjType.ActionRes, Vector3.zero);
                            _lhc___0.AddObj(_parPathKey___20, _tmpObj___15);
                            yield return new CHoldForSecond(0f);
                        }
                    }
                
                    _index___6++;
                    UpdateProgress(_lhc___0, _progress___7, _subDuty___4, _index___6, _count___5);
                    yield return new CHoldForSecond(0f);

                    _j___14++;
                }
                var _j___21 = 0;
                while (_j___21 < _restAssets___11.mesPrefabs.Count)
                {
                    GameObject _tmpObj___22 = null;
                    AssetLoadBase base3 = _restAssets___11.mesPrefabs[_j___21];
                    if (!_lhc___0.HasLoaded(base3.assetPath))
                    {
                        AssetLoadBase base4 = _restAssets___11.mesPrefabs[_j___21];
                        _tmpObj___22 = MonoSingleton<SceneMgr>.instance.GetPooledGameObjLOD(base4.assetPath, false, SceneObjType.ActionRes, Vector3.zero);
                        AssetLoadBase base5 = _restAssets___11.mesPrefabs[_j___21];
                        _lhc___0.AddObj(base5.assetPath, _tmpObj___22);
                        yield return new CHoldForSecond(0f);
                    }
                
                    _index___6++;
                    UpdateProgress(_lhc___0, _progress___7, _subDuty___4, _index___6, _count___5);
                    yield return new CHoldForSecond(0f);

                    _j___21++;
                }
                var _textures___23 = new ListView<Texture>();
                var _j___24 = 0;
                while (_j___24 < _restAssets___11.spritePrefabs.Count)
                {
                    AssetLoadBase base6 = _restAssets___11.spritePrefabs[_j___24];
                    var _tempObj___25 = Singleton<CResourceManager>.instance.GetResource(base6.assetPath, typeof(GameObject), enResourceType.UIPrefab, true, false);
                    yield return new CHoldForSecond(0f);

                    if (((_tempObj___25 != null) && (_tempObj___25.m_content != null)) && (_tempObj___25.m_content is GameObject))
                    {
                        var _gameObj___26 = (GameObject)_tempObj___25.m_content;
                        var _sr___27 = _gameObj___26.GetComponent<SpriteRenderer>();
                        if (((_sr___27 != null) && (_sr___27.sprite != null)) && (_sr___27.sprite.texture != null))
                        {
                            _textures___23.Add(_sr___27.sprite.texture);
                        }
                        yield return new CHoldForSecond(0f);
                    }
                
                    _index___6++;
                    UpdateProgress(_lhc___0, _progress___7, _subDuty___4, _index___6, _count___5);
                    yield return new CHoldForSecond(0f);

                    _j___24++;
                }
                _textures___23.Clear();
                var _j___28 = 0;
                while (_j___28 < _restAssets___11.ageActions.Count)
                {
                    AssetLoadBase base7 = _restAssets___11.ageActions[_j___28];
                    var _action___29 = MonoSingleton<ActionManager>.instance.LoadActionResource(base7.assetPath);
                    yield return new CHoldForSecond(0f);

                    _index___6++;
                    UpdateProgress(_lhc___0, _progress___7, _subDuty___4, _index___6, _count___5);
                    _j___28++;
                }
                _idx___10++;
            }
            ReleaseMemoryIfNeed();
            UpdateProgress(_lhc___0, _progress___7, _subDuty___4, 1, 1);
            yield return new CHoldForSecond(0f);
            UpdateProgress(_lhc___0, _progress___7, _subDuty___4, 1, 1);
            UpdateProgress(_lhc___0, _oldProgress___3, _duty___1, 1, 1);
            yield return new CHoldForSecond(0f);
            UpdateProgress(_lhc___0, _oldProgress___3, _duty___1, 1, 1);
        }

        private IEnumerator LoadArtistLevel(GameLoader.LoaderHelperWrapper InWrapper)
        {
            var oldProgress = nProgress;
            var i = 0;
            while (i < levelArtistList.Count)
            {
                var artAssetNameHigh = levelArtistList[i] + "/" + levelArtistList[i];
                var artAssetNameMid = artAssetNameHigh.Replace("_High", "_Mid");
                var artAssetNameLow = artAssetNameHigh.Replace("_High", "_Low");
                var levelNames = new string[] { artAssetNameHigh, artAssetNameMid, artAssetNameLow };
                LevelResAsset levelArtist = null;
                var fullPath = string.Empty;
                var lod = GameSettings.ModelLOD;
                lod = Mathf.Clamp(lod, 0, 2);
                while (lod >= 0)
                {
                    fullPath = "SceneExport/Artist/" + levelNames[lod] + ".asset";
                    levelArtist = (LevelResAsset)Singleton<CResourceManager>.GetInstance().GetResource(fullPath, typeof(LevelResAsset), enResourceType.BattleScene, false, true).m_content;
                    yield return new CHoldForSecond(0f);
                    
                    if (null != levelArtist)
                    {
                        break;
                    }

                    lod--;
                }

                if (null == levelArtist)
                {
                    Debug.LogError("错误，没有找到导出的美术场景SceneExport/Artist/" + levelArtistList[i] + ".asset");
                    i++;
                    continue;
                }
                var holder = new ObjectHolder();
                yield return Singleton<CCoroutineManager>.GetInstance().StartCoroutine(GameLoader.s_serializer.LoadAsync(levelArtist, holder));

                var artRoot = holder.obj as GameObject;
                if (null == artRoot)
                {
                    Debug.LogError("美术场景SceneExport/Artist/" + levelArtistList[i] + ".asset有错误！请检查！");
                    i++;
                    continue;
                }
                var staticRoot = artRoot.transform.Find("StaticMesh");
                if (null != staticRoot)
                {
                    StaticBatchingUtility.Combine(staticRoot.gameObject);
                    yield return new CHoldForSecond(0f);
                }
                Singleton<CResourceManager>.GetInstance().RemoveCachedResource(fullPath);
                yield return new CHoldForSecond(0f);

                var _psArray___12 = artRoot.GetComponentsInChildren<ParticleSystem>();
                yield return new CHoldForSecond(0f);

                var _j___13 = 0;
                while (_j___13 < _psArray___12.Length)
                {
                    if (((_psArray___12[_j___13] != null) && _psArray___12[_j___13].gameObject.activeSelf) && (_psArray___12[_j___13].transform.parent != null))
                    {
                        MonoSingleton<SceneMgr>.GetInstance().AddCulling(_psArray___12[_j___13].transform.gameObject, "ParticleCulling_" + _j___13);
                        yield return new CHoldForSecond(0f);
                    }
                
                    _j___13++;
                }
                var _animators___14 = artRoot.GetComponentsInChildren<Animator>();
                yield return new CHoldForSecond(0f);
                var _j___15 = 0;
                while (_j___15 < _animators___14.Length)
                {
                    _animators___14[_j___15].cullingMode = AnimatorCullingMode.BasedOnRenderers;
                    yield return new CHoldForSecond(0f);
                    _j___15++;
                }
                yield return new CHoldForSecond(0f);
                var _CullingBoxes___16 = artRoot.GetComponentsInChildren<CullingBox>();
                var _j___17 = 0;
                while (_j___17 < _CullingBoxes___16.Length)
                {
                    Object.Destroy(_CullingBoxes___16[_j___17]);
                    _j___17++;
                }
                
                i++;
            }
            Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("Scene");
            var _duty___18 = InWrapper.duty;
            nProgress = oldProgress + _duty___18;
            LoadProgressEvent(nProgress * 0.0001f);
            yield return new CWaitForNextFrame();
        }


        private IEnumerator LoadCommonAssetBundle(GameLoader.LoaderHelperWrapper InWrapper)
        {
            var _duty___0 = InWrapper.duty;
            var _oldProgress___1 = nProgress;
            Singleton<CResourceManager>.GetInstance().LoadAssetBundle("AssetBundle/Hero_CommonRes.assetbundle");
            yield return new CHoldForSecond(0f);

            Singleton<CResourceManager>.GetInstance().LoadAssetBundle("AssetBundle/Skill_CommonEffect1.assetbundle");
            yield return new CHoldForSecond(0f);

            Singleton<CResourceManager>.GetInstance().LoadAssetBundle("AssetBundle/Skill_CommonEffect2.assetbundle");
            yield return new CHoldForSecond(0f);

            Singleton<CResourceManager>.GetInstance().LoadAssetBundle("AssetBundle/Systems_Effects.assetbundle");
            yield return new CHoldForSecond(0f);

            Singleton<CResourceManager>.GetInstance().LoadAssetBundle("AssetBundle/UGUI_Talent.assetbundle");
            Singleton<CResourceManager>.GetInstance().LoadAssetBundle("AssetBundle/UGUI_Map.assetbundle");
            nProgress = _oldProgress___1 + _duty___0;
            LoadProgressEvent(nProgress * 0.0001f);
            yield return new CHoldForSecond(0f);
        }

        private IEnumerator LoadCommonAssets(GameLoader.LoaderHelperWrapper InWrapper)
        {
            var _oldProgress___0 = nProgress;
            var _i___1 = 0;
            while (_i___1 < soundBankList.Count)
            {
                Singleton<CSoundManager>.instance.LoadBank(soundBankList[_i___1], CSoundManager.BankType.Battle);
                yield return new CHoldForSecond(0f); // case 1
                _i___1++;
            }

            MonoSingleton<SceneMgr>.instance.PreloadCommonAssets();
            yield return new CHoldForSecond(0f); // case 2
            
            nProgress = _oldProgress___0 + InWrapper.duty;
            LoadProgressEvent(nProgress * 0.0001f);
            yield return new CWaitForNextFrame();

        }

        private IEnumerator LoadCommonEffect(GameLoader.LoaderHelperWrapper InWrapper)
        {
            var _duty___0 = InWrapper.duty;
            var _oldProgress___1 = nProgress;
            Singleton<CCoroutineManager>.GetInstance().StartCoroutine(MonoSingleton<SceneMgr>.instance.PreloadCommonEffects());
            nProgress = _oldProgress___1 + _duty___0;
            LoadProgressEvent(nProgress * 0.0001f);
            yield return new CWaitForNextFrame();
        }

        private IEnumerator LoadDesignLevel(GameLoader.LoaderHelperWrapper InWrapper)
        {
            var _oldProgress___0 = nProgress;
            var _i___1 = 0;
            while (_i___1 < levelDesignList.Count)
            {
                var _desgineAssetNameHigh___2 = levelDesignList[_i___1] + "/" + levelDesignList[_i___1];
                var _desgineAssetNameMid___3 = _desgineAssetNameHigh___2.Replace("_High", "_Mid");
                var _desgineAssetNameLow___4 = _desgineAssetNameHigh___2.Replace("_High", "_Low");
                var _levelNames___5 = new string[] { _desgineAssetNameHigh___2, _desgineAssetNameMid___3, _desgineAssetNameLow___4 };
                CBinaryObject _binaryObject___6 = null;
                var _fullPath___7 = string.Empty;
                var _lod___8 = GameSettings.ModelLOD;
                _lod___8 = Mathf.Clamp(_lod___8, 0, 2);
                while (_lod___8 >= 0)
                {
                    _fullPath___7 = "SceneExport/Design/" + _levelNames___5[_lod___8] + ".bytes";
                    _binaryObject___6 = Singleton<CResourceManager>.GetInstance().GetResource(_fullPath___7, typeof(TextAsset), enResourceType.BattleScene, false, true).m_content as CBinaryObject;
                    yield return new CHoldForSecond(0f);

                    if (null != _binaryObject___6)
                    {
                        break;
                    }

                    _lod___8--;
                }

                if (null == _binaryObject___6)
                {
                    object[] objArray1 = new object[] { _desgineAssetNameHigh___2 };
                    DebugHelper.Assert(false, "错误，没有找到导出的策划场景 {0}", objArray1);
                    _i___1++;
                    continue;
                }
                var _holder___9 = new ObjectHolder();
                yield return Singleton<CCoroutineManager>.GetInstance().StartCoroutine(GameLoader.s_serializer.LoadAsync(_binaryObject___6.m_data, _holder___9));

                var _designRoot___10 = _holder___9.obj as GameObject;
                if (null != _designRoot___10)
                {
                    var _staticRoot___11 = _designRoot___10.transform.Find("StaticMesh");
                    if (null != _staticRoot___11)
                    {
                        StaticBatchingUtility.Combine(_staticRoot___11.gameObject);
                        yield return new CHoldForSecond(0f);
                    }
                    yield return new CHoldForSecond(0f);
                    var _CullingBoxes___12 = _designRoot___10.GetComponentsInChildren<CullingBox>();
                    var _j___13 = 0;
                    while (_j___13 < _CullingBoxes___12.Length)
                    {
                        Object.Destroy(_CullingBoxes___12[_j___13]);
                        _j___13++;
                    }
                    Singleton<CResourceManager>.GetInstance().RemoveCachedResource(_fullPath___7);
                }
                else
                {
                    object[] inParameters = new object[] { levelDesignList[_i___1] };
                    DebugHelper.Assert(false, "策划场景 SceneExport/Design/{0}.bytes有错误！请检查！", inParameters);
                }

                _i___1++;
            }
            Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("Scene");
            yield return new CHoldForSecond(0f);
            Singleton<SceneManagement>.GetInstance().InitScene();
            yield return new CHoldForSecond(0f);
            nProgress = _oldProgress___0 + InWrapper.duty;
            LoadProgressEvent(nProgress * 0.0001f);
            yield return new CWaitForNextFrame();
        }

        private IEnumerator LoadNoActorAssets(GameLoader.LHCWrapper InWrapper)
        {
            var _lhc___0 = InWrapper.lhc;
            var _duty___1 = InWrapper.duty;
            var _loadHelper___2 = InWrapper.loadHelper;
            var _oldProgress___3 = nProgress;
            var _count___4 = (noActorPreLoad.parPrefabs.Count + noActorPreLoad.mesPrefabs.Count) + noActorPreLoad.spritePrefabs.Count;
            var _index___5 = 0;
            var _j___6 = 0;
            while (_j___6 < noActorPreLoad.parPrefabs.Count)
            {
                GameObject _tmpObj___7 = null;
                AssetLoadBase base2 = noActorPreLoad.parPrefabs[_j___6];
                var _assetPath___8 = base2.assetPath;
                if (!_lhc___0.HasLoaded(_assetPath___8))
                {
                    if (GameSettings.DynamicParticleLOD)
                    {
                        var _currentParticleLOD___9 = GameSettings.ParticleLOD;
                        var _lod___10 = _currentParticleLOD___9;
                        while (_lod___10 <= 2)
                        {
                            AssetLoadBase base4;
                            if ((_currentParticleLOD___9 == 0) && (_lod___10 == 1))
                            {
                                _lod___10++;
                                continue;
                            }
                            var _parPathKey___11 = _assetPath___8 + "_lod_" + _lod___10;
                            if (_lhc___0.HasLoaded(_parPathKey___11))
                            {
                                _lod___10++;
                                continue;
                            }
                            GameSettings.ParticleLOD = _lod___10;
                            _tmpObj___7 = MonoSingleton<SceneMgr>.instance.GetPooledGameObjLOD(_assetPath___8, true, SceneObjType.ActionRes, Vector3.zero);
                            _lhc___0.AddObj(_parPathKey___11, _tmpObj___7);
                            AssetLoadBase base3 = noActorPreLoad.parPrefabs[_j___6];
                            if (base3.nInstantiate <= 1)
                            {
                                yield return new CHoldForSecond(0f);
                                _lod___10++;
                                continue;
                            }

                            var _k___12 = 1;
                            base4 = noActorPreLoad.parPrefabs[_j___6];
                            while (_k___12 < base4.nInstantiate)
                            {
                                var _prepareObj___13 = MonoSingleton<SceneMgr>.instance.GetPooledGameObjLOD(_assetPath___8, true, SceneObjType.ActionRes, Vector3.zero);
                                Singleton<CGameObjectPool>.GetInstance().RecycleGameObjectDelay(_prepareObj___13, 0, null, null, null);
                                _k___12++;
                            }

                            yield return new CHoldForSecond(0f);
                            _lod___10++;
                        }
                        GameSettings.ParticleLOD = _currentParticleLOD___9;
                    }
                    else
                    {
                        var _parPathKey___14 = _assetPath___8;
                        if (!_lhc___0.HasLoaded(_parPathKey___14))
                        {
                            _tmpObj___7 = MonoSingleton<SceneMgr>.instance.GetPooledGameObjLOD(_assetPath___8, true, SceneObjType.ActionRes, Vector3.zero);
                            _lhc___0.AddObj(_parPathKey___14, _tmpObj___7);
                            yield return new CHoldForSecond(0f);
                        }
                    }
                }

                _index___5++;
                UpdateProgress(_lhc___0, _oldProgress___3, _duty___1, _index___5, _count___4);
                yield return new CHoldForSecond(0f);
                _j___6++;
            }

            ReleaseMemoryIfNeed();
            var _j___15 = 0;
            while (_j___15 < noActorPreLoad.mesPrefabs.Count)
            {
                GameObject _tmpObj___16 = null;
                AssetLoadBase base5 = noActorPreLoad.mesPrefabs[_j___15];
                if (!_lhc___0.HasLoaded(base5.assetPath))
                {
                    AssetLoadBase base6 = noActorPreLoad.mesPrefabs[_j___15];
                    _tmpObj___16 = MonoSingleton<SceneMgr>.instance.GetPooledGameObjLOD(base6.assetPath, false, SceneObjType.ActionRes, Vector3.zero);
                    AssetLoadBase base7 = noActorPreLoad.mesPrefabs[_j___15];
                    _lhc___0.AddObj(base7.assetPath, _tmpObj___16);
                    yield return new CHoldForSecond(0f);
                }

                _index___5++;
                UpdateProgress(_lhc___0, _oldProgress___3, _duty___1, _index___5, _count___4);
                yield return new CHoldForSecond(0f);

                _j___15++;
            }

            ReleaseMemoryIfNeed();
            var _textures___17 = new ListView<Texture>();
            var _j___18 = 0;
            while (_j___18 < noActorPreLoad.spritePrefabs.Count)
            {
                AssetLoadBase base8 = noActorPreLoad.spritePrefabs[_j___18];
                var _tempObj___19 = Singleton<CResourceManager>.instance.GetResource(base8.assetPath, typeof(GameObject), enResourceType.UIPrefab, true, false);
                yield return new CHoldForSecond(0f);

                if (((_tempObj___19 != null) && (_tempObj___19.m_content != null)) && (_tempObj___19.m_content is GameObject))
                {
                    var _gameObj___20 = (GameObject)_tempObj___19.m_content;
                    var _sr___21 = _gameObj___20.GetComponent<SpriteRenderer>();
                    if (((_sr___21 != null) && (_sr___21.sprite != null)) && (_sr___21.sprite.texture != null))
                    {
                        _textures___17.Add(_sr___21.sprite.texture);
                    }
                }
                yield return new CHoldForSecond(0f);
                _index___5++;
                UpdateProgress(_lhc___0, _oldProgress___3, _duty___1, _index___5, _count___4);
                yield return new CHoldForSecond(0f);

                _j___18++;
            }
            _textures___17.Clear();
            ReleaseMemoryIfNeed();
            UpdateProgress(_lhc___0, _oldProgress___3, _duty___1, 1, 1);
            yield return new CWaitForNextFrame();
            UpdateProgress(_lhc___0, _oldProgress___3, _duty___1, 1, 1);
        }

        private IEnumerator PreSpawnSoldiers(GameLoader.LoaderHelperWrapper InWrapper)
        {
            var _oldProgress___0 = nProgress;
            var _duty___1 = InWrapper.duty;
            var _spawnCountTotal___2 = 0;
            var _i___3 = 0;
            while (_i___3 < actorPreload.Count)
            {
                var _preloadTab___4 = actorPreload[_i___3];
                var _num___5 = Mathf.Max(Mathf.RoundToInt(_preloadTab___4.spawnCnt), 1);
                if (_preloadTab___4.theActor.ActorType == ActorTypeDef.Actor_Type_Monster)
                {
                    _spawnCountTotal___2 += _num___5;
                }
                _i___3++;
            }
            GameObjMgr.isPreSpawnActors = true;
            var _spawnIndex___6 = 0;
            var _i___7 = 0;
            while (_i___7 < actorPreload.Count)
            {
                var _preloadTab___8 = actorPreload[_i___7];
                var _actorMeta___9 = _preloadTab___8.theActor;
                if (_actorMeta___9.ActorType == ActorTypeDef.Actor_Type_Monster)
                {
                    var _count___10 = Mathf.Max(Mathf.RoundToInt(_preloadTab___8.spawnCnt), 1);
                    string _actorName___11 = null;
                    var _j___12 = 0;
                    while (_j___12 < _count___10)
                    {
                        var _monster___13 = Singleton<GameObjMgr>.GetInstance().SpawnActorEx(null, ref _actorMeta___9, VInt3.zero, VInt3.forward, false, true);
                        yield return new CHoldForSecond(0f);
                        if (_monster___13 != null)
                        {
                            var _monsterActor___14 = _monster___13.handle;
                            _monsterActor___14.InitActor();
                            yield return new CHoldForSecond(0f);
                            _monsterActor___14.PrepareFight();
                            yield return new CHoldForSecond(0f);
                            _monsterActor___14.gameObject.name = _monsterActor___14.TheStaticData.TheResInfo.Name;
                            _monsterActor___14.StartFight();
                            yield return new CHoldForSecond(0f);

                            if (_actorName___11 == null)
                            {
                                _actorName___11 = _monsterActor___14.TheStaticData.TheResInfo.Name;
                            }
                            Singleton<GameObjMgr>.instance.AddToCache(_monster___13);
                        }
                        UpdateProgress(null, _oldProgress___0, _duty___1, ++_spawnIndex___6, _spawnCountTotal___2);
                        yield return new CHoldForSecond(0f);
          
                        _j___12++;
                    }
                }
                _i___7++;
            }
            nProgress = _oldProgress___0 + _duty___1;
            LoadProgressEvent(nProgress * 0.0001f);
            yield return new CHoldForSecond(0f);
            GameObjMgr.isPreSpawnActors = false;
            HudComponent3D.PreallocMapPointer(20, 40);
            yield return new CHoldForSecond(0f);
            SObjPool<SRefParam>.Alloc(0x400);
            yield return new CHoldForSecond(0f);
        }

        private IEnumerator SpawnDynamicActor(GameLoader.LoaderHelperWrapper InWrapper)
        {
            var duty = InWrapper.duty;
            var oldProgress = nProgress;
            var i = 0;
            while (i < actorList.Count)
            {
                var actorMeta = actorList[i];
                var bornPos = new VInt3();
                var bornDir = new VInt3();
                if (actorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
                {
                    DebugHelper.Assert(Singleton<BattleLogic>.instance.mapLogic != null, "what? BattleLogic.instance.mapLogic==null");
                    Singleton<BattleLogic>.GetInstance().mapLogic.GetRevivePosDir(ref actorMeta, true, out bornPos, out bornDir);
                    yield return new CHoldForSecond(0f);
                }

                var actor = Singleton<GameObjMgr>.instance.SpawnActorEx(null, ref actorMeta, bornPos, bornDir, false, true);
                yield return new CHoldForSecond(0f);
                if (actor != null)
                {
                    Singleton<GameObjMgr>.GetInstance().HoldDynamicActor(actor);
                }
                nProgress = oldProgress + ((duty * (i + 1)) / actorList.Count);
                LoadProgressEvent(nProgress * 0.0001f);
                yield return new CHoldForSecond(0f);

                i++;
            }
            nProgress = oldProgress + duty;
            LoadProgressEvent(nProgress * 0.0001f);
            yield return new CHoldForSecond(0f);
        }

        private IEnumerator SpawnStaticActor(GameLoader.LoaderHelperWrapper InWrapper)
        {
            var duty = InWrapper.duty;
            var oldProgress = nProgress;
            var i = 0;
            while (i < staticActors.Count)
            {
                var actorMeta = new ActorMeta();
                actorMeta.ActorType = staticActors[i].ActorType;
                actorMeta.ConfigId = staticActors[i].ConfigID;
                actorMeta.ActorCamp = staticActors[i].CmpType;
                var bornPos = (VInt3)staticActors[i].transform.position;
                var bornDir = (VInt3)staticActors[i].transform.forward;
                var actor = Singleton<GameObjMgr>.instance.SpawnActorEx(staticActors[i].gameObject, ref actorMeta, bornPos, bornDir, false, true);
                yield return new  CHoldForSecond(0f);
                if (actor != null)
                {
                    Singleton<GameObjMgr>.GetInstance().HoldStaticActor(actor);
                }
                nProgress = oldProgress + ((duty * (i + 1)) / staticActors.Count);
                LoadProgressEvent(nProgress * 0.0001f);
                yield return new  CHoldForSecond(0f);
                i++;
            }
            nProgress = oldProgress + duty;
            LoadProgressEvent(nProgress * 0.0001f);
            yield return new CHoldForSecond(0f);
            staticActors.Clear();
        }

		private IEnumerator AnalyseResourcePreload(LoaderHelper loadHelper)
		{
			yield return Singleton<CCoroutineManager>.GetInstance().StartCoroutine(AnalyseActorAssets(loadHelper));
			yield return Singleton<CCoroutineManager>.GetInstance().StartCoroutine(AnalyseNoActorAssets(loadHelper));
			yield return Singleton<CCoroutineManager>.GetInstance().StartCoroutine(AnalyseAgeRecursiveAssets(loadHelper));
		}

		private IEnumerator AnalyseActorAssets(LoaderHelper loadHelper)
		{
			actorPreload = loadHelper.GetActorPreload();
			yield return new CHoldForSecond(0f);
		}
		

		private IEnumerator AnalyseNoActorAssets(LoaderHelper loadHelper)
		{
			if (noActorPreLoad == null)
			{
				noActorPreLoad = new ActorPreloadTab();
			}
			yield return Singleton<CCoroutineManager>.GetInstance().StartCoroutine(loadHelper.GetGlobalPreload(noActorPreLoad));

			int i = 0;
			while (i < actorList.Count)
			{
				var meta = actorList[i];
				if (meta.ActorType == ActorTypeDef.Actor_Type_Hero)
				{
					var path = KillNotifyUT.GetHero_Icon(ref meta, false);
					if (!string.IsNullOrEmpty(path))
					{
						noActorPreLoad.AddSprite(path);
					}
					path = KillNotifyUT.GetHero_Icon(ref meta, true);
					if (!string.IsNullOrEmpty(path))
					{
						noActorPreLoad.AddMesh(path);
					}
				}
				i++;
			}
		}

		private static List<List<string>> composite(string[] input)
		{
			List<List<string>> list = new List<List<string>>();
			if (input.Length > 8)
			{
				throw new Exception("only support less than 8 words");
			}
			byte b = 0;
			while ((float)b < Mathf.Pow(2f, (float)input.Length))
			{
				List<string> list2 = new List<string>();
				for (int i = 0; i < input.Length; i++)
				{
					if ((b >> i & 1) > 0)
					{
						list2.Add(input[i]);
					}
				}
				list.Add(list2);
				b += 1;
			}
			return list;
		}


		private void ReleaseMemoryIfNeed()
		{
			if (DeviceCheckSys.GetAvailMemoryMegaBytes() > 100)
			{
				return;
			}
			Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("ActorInfo");
			Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharIcon");
			Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharBattle");
			Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharShow");
			Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharLoading");
			Resources.UnloadUnusedAssets();
			GC.Collect();
		}

		public void AdvanceStopLoad()
		{
			if (this.isLoadStart)
			{
				Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("ActorInfo");
				Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharIcon");
				Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharBattle");
				Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharShow");
				Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharLoading");
				Singleton<CResourceManager>.GetInstance().UnloadUnusedAssets();
				GC.Collect();
				Singleton<EventRouter>.instance.BroadCastEvent(EventID.ADVANCE_STOP_LOADING);
			}
			this.ResetLoader();
		}
	}
}
