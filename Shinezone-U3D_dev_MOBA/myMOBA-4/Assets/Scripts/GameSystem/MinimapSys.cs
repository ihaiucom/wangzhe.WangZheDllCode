using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameSystem
{
	public class MinimapSys
	{
		public enum ElementType
		{
			None,
			Tower,
			Base,
			Hero,
			Dragon_5_big,
			Dragon_5_small,
			Dragon_3,
			Eye,
			Solider
		}

		public enum EMapType
		{
			None,
			Mini,
			Big,
			Skill
		}

		public class ElementInMap
		{
			public VInt3 pos;

			public GameObject smallElement;

			public GameObject bigElement;

			public ElementInMap(VInt3 pos, GameObject smallElement, GameObject bigElement)
			{
				this.pos = pos;
				this.smallElement = smallElement;
				this.bigElement = bigElement;
			}
		}

		public struct BornRecord
		{
			public VInt3 pos;

			public uint cfgId;
		}

		public static float DEPTH = 30f;

		public static string self_Eye = "UGUI/Sprite/Battle/Img_Map_BlueEye";

		public static string enemy_Eye = "UGUI/Sprite/Battle/Img_Map_RedEye";

		public static string miniMapPrefabPath = "UI3D/Battle/MiniMap/Mini_Map.prefab";

		private List<MinimapSys.ElementInMap> m_mapElements = new List<MinimapSys.ElementInMap>();

		private List<MinimapSys.BornRecord> m_bornRecords = new List<MinimapSys.BornRecord>();

		private MinimapSys.EMapType curMapType;

		private CUIFormScript ownerForm;

		public DragonIcon m_dragonIcon;

		public MiniMapCameraFrame_3DUI MMiniMapCameraFrame_3Dui;

		public BackCityCom_3DUI MMiniMapBackCityCom_3Dui;

		public MiniMapTrack_3DUI MMiniMapTrack_3Dui;

		public MinimapSkillIndicator_3DUI MMinimapSkillIndicator_3Dui;

		public UI3DEventMgr UI3DEventMgr = new UI3DEventMgr();

		public MiniMapEffectModule miniMapEffectModule = new MiniMapEffectModule();

		public MiniMapSignalPanel miniMapSignalPanel = new MiniMapSignalPanel();

		private COM_PLAYERCAMP m_playerCamp = COM_PLAYERCAMP.COM_PLAYERCAMP_COUNT;

		private int m_elementIndex;

		private SkillSlotType m_CurSelectedSlotType = SkillSlotType.SLOT_SKILL_COUNT;

		private Vector2 m_mmStandardScreenPos;

		private Vector2 m_bmStandardScreenPos;

		private Vector2 m_mmFinalScreenPos;

		private Vector2 m_bmFinalScreenPos;

		private GameObject extraBtnObj_bigMap;

		private GameObject extraBtnObj_skillMap;

		private Vector3 cachePos = Vector3.zero;

		private Sprite3D mmSpt3D;

		private Sprite3D bmSpt3D;

		private ListView<RectTransform> m_AllTransformWithBigMapMask = new ListView<RectTransform>();

		private ListView<RectTransform> m_AllTransformWithBigMapShow = new ListView<RectTransform>();

		public GameObject mmRoot
		{
			get;
			private set;
		}

		public GameObject mmUGUIRoot
		{
			get;
			private set;
		}

		public GameObject mmpcAlies
		{
			get;
			private set;
		}

		public GameObject mmpcHeroBgFrame_Friend
		{
			get;
			private set;
		}

		public GameObject mmpcHeroBgFrame_Enemy
		{
			get;
			private set;
		}

		public GameObject mmpcRedBuff
		{
			get;
			private set;
		}

		public GameObject mmpcBlueBuff
		{
			get;
			private set;
		}

		public GameObject mmpcJungle
		{
			get;
			private set;
		}

		public GameObject mmpcEnemy
		{
			get;
			private set;
		}

		public GameObject mmpcOrgan
		{
			get;
			private set;
		}

		public GameObject mmpcSignal
		{
			get;
			private set;
		}

		public GameObject mmBGMap_3dui
		{
			get;
			private set;
		}

		public GameObject mmpcDragon_ugui
		{
			get;
			private set;
		}

		public GameObject mmpcDragon_3dui
		{
			get;
			private set;
		}

		public GameObject mmpcEffect
		{
			get;
			private set;
		}

		public GameObject mmpcEye
		{
			get;
			private set;
		}

		public GameObject mmpcTrack
		{
			get;
			private set;
		}

		public GameObject mmHeroIconNode
		{
			get;
			private set;
		}

		public GameObject mmHeroIconNode_Self
		{
			get;
			private set;
		}

		public GameObject mmHeroIconNode_Friend
		{
			get;
			private set;
		}

		public GameObject mmHeroIconNode_Enemy
		{
			get;
			private set;
		}

		public GameObject bmRoot
		{
			get;
			private set;
		}

		public GameObject bmUGUIRoot
		{
			get;
			private set;
		}

		public GameObject bmpcAlies
		{
			get;
			private set;
		}

		public GameObject bmpcHeroBgFrame_Friend
		{
			get;
			private set;
		}

		public GameObject bmpcHeroBgFrame_Enemy
		{
			get;
			private set;
		}

		public GameObject bmpcRedBuff
		{
			get;
			private set;
		}

		public GameObject bmpcBlueBuff
		{
			get;
			private set;
		}

		public GameObject bmpcJungle
		{
			get;
			private set;
		}

		public GameObject bmpcEnemy
		{
			get;
			private set;
		}

		public GameObject bmpcOrgan
		{
			get;
			private set;
		}

		public GameObject bmpcSignal
		{
			get;
			private set;
		}

		public GameObject bmBGMap_3dui
		{
			get;
			private set;
		}

		public GameObject bmpcDragon_ugui
		{
			get;
			private set;
		}

		public GameObject bmpcDragon_3dui
		{
			get;
			private set;
		}

		public GameObject bmpcEffect
		{
			get;
			private set;
		}

		public GameObject bmpcEye
		{
			get;
			private set;
		}

		public GameObject bmpcTrack
		{
			get;
			private set;
		}

		public GameObject bmHeroIconNode
		{
			get;
			private set;
		}

		public GameObject bmHeroIconNode_Self
		{
			get;
			private set;
		}

		public GameObject bmHeroIconNode_Friend
		{
			get;
			private set;
		}

		public GameObject bmHeroIconNode_Enemy
		{
			get;
			private set;
		}

		public Vector2 mmFinalScreenSize
		{
			get;
			private set;
		}

		public Vector2 bmFinalScreenSize
		{
			get;
			private set;
		}

		public Vector2 GetMMStandardScreenPos()
		{
			return this.m_mmStandardScreenPos;
		}

		public Vector2 GetBMStandardScreenPos()
		{
			return this.m_bmStandardScreenPos;
		}

		public Vector2 GetMMFianlScreenPos()
		{
			return this.m_mmFinalScreenPos;
		}

		public Vector2 GetBMFianlScreenPos()
		{
			return this.m_bmFinalScreenPos;
		}

		public static void Preload(ref ActorPreloadTab preloadTab)
		{
			MiniMapSysUT.Preload(ref preloadTab);
		}

		private void GetCacheObj()
		{
			this.mmpcAlies = Utility.FindChild(this.mmRoot, "Container_MiniMapPointer_Alies");
			this.mmpcHeroBgFrame_Friend = Utility.FindChild(this.mmRoot, "Container_MiniMapPointer_Hero/friend");
			this.mmpcHeroBgFrame_Enemy = Utility.FindChild(this.mmRoot, "Container_MiniMapPointer_Hero/enemy");
			this.mmpcRedBuff = Utility.FindChild(this.mmRoot, "Container_MiniMapPointer_RedBuff");
			this.mmpcBlueBuff = Utility.FindChild(this.mmRoot, "Container_MiniMapPointer_BlueBuff");
			this.mmpcJungle = Utility.FindChild(this.mmRoot, "Container_MiniMapPointer_Jungle");
			this.mmpcEnemy = Utility.FindChild(this.mmRoot, "Container_MiniMapPointer_Enemy");
			this.mmpcOrgan = Utility.FindChild(this.mmRoot, "Container_MiniMapPointer_Organ");
			this.mmpcSignal = Utility.FindChild(this.mmRoot, "Container_MiniMapPointer_Signal");
			this.mmpcDragon_ugui = Utility.FindChild(this.ownerForm.gameObject, "MapPanel/Mini/Container_MiniMapPointer_Dragon");
			this.mmpcDragon_ugui.CustomSetActive(true);
			this.mmpcDragon_3dui = Utility.FindChild(this.mmRoot, "Container_MiniMapPointer_Dragon");
			this.mmpcEffect = Utility.FindChild(this.mmRoot, "BigMapEffectRoot");
			this.mmpcEye = Utility.FindChild(this.mmRoot, "Container_MiniMapPointer_Eye");
			this.mmpcTrack = Utility.FindChild(this.mmRoot, "Container_MiniMapPointer_Track");
			this.mmHeroIconNode = Utility.FindChild(this.mmRoot, "HeroIconNode");
			this.mmHeroIconNode_Self = Utility.FindChild(this.mmRoot, "HeroIconNode/self");
			this.mmHeroIconNode_Friend = Utility.FindChild(this.mmRoot, "HeroIconNode/friend");
			this.mmHeroIconNode_Enemy = Utility.FindChild(this.mmRoot, "HeroIconNode/enemy");
			this.bmpcAlies = Utility.FindChild(this.bmRoot, "Container_BigMapPointer_Alies");
			this.bmpcHeroBgFrame_Friend = Utility.FindChild(this.bmRoot, "Container_BigMapPointer_Hero/friend");
			this.bmpcHeroBgFrame_Enemy = Utility.FindChild(this.bmRoot, "Container_BigMapPointer_Hero/enemy");
			this.bmpcRedBuff = Utility.FindChild(this.bmRoot, "Container_BigMapPointer_RedBuff");
			this.bmpcBlueBuff = Utility.FindChild(this.bmRoot, "Container_BigMapPointer_BlueBuff");
			this.bmpcJungle = Utility.FindChild(this.bmRoot, "Container_BigMapPointer_Jungle");
			this.bmpcEnemy = Utility.FindChild(this.bmRoot, "Container_BigMapPointer_Enemy");
			this.bmpcOrgan = Utility.FindChild(this.bmRoot, "Container_BigMapPointer_Organ");
			this.bmpcSignal = Utility.FindChild(this.bmRoot, "Container_BigMapPointer_Signal");
			this.bmpcDragon_ugui = Utility.FindChild(this.ownerForm.gameObject, "MapPanel/Big/Container_BigMapPointer_Dragon");
			this.bmpcDragon_ugui.CustomSetActive(true);
			this.bmpcDragon_3dui = Utility.FindChild(this.bmRoot, "Container_BigMapPointer_Dragon");
			this.mmpcEffect = Utility.FindChild(this.bmRoot, "BigMapEffectRoot");
			this.bmpcEye = Utility.FindChild(this.bmRoot, "Container_BigMapPointer_Eye");
			this.bmpcTrack = Utility.FindChild(this.bmRoot, "Container_BigMapPointer_Track");
			this.bmHeroIconNode = Utility.FindChild(this.bmRoot, "HeroIconNode");
			this.bmHeroIconNode_Self = Utility.FindChild(this.bmRoot, "HeroIconNode/self");
			this.bmHeroIconNode_Friend = Utility.FindChild(this.bmRoot, "HeroIconNode/friend");
			this.bmHeroIconNode_Enemy = Utility.FindChild(this.bmRoot, "HeroIconNode/enemy");
		}

		public void Init(CUIFormScript formObj, SLevelContext levelContext)
		{
			if (formObj == null || levelContext == null)
			{
				return;
			}
			this.m_playerCamp = Singleton<GamePlayerCenter>.instance.hostPlayerCamp;
			this.ownerForm = formObj;
			this.extraBtnObj_bigMap = Utility.FindChild(formObj.gameObject, "MapPanel/Big/Button_CloseBigMap");
			this.extraBtnObj_bigMap.CustomSetActive(false);
			this.extraBtnObj_skillMap = Utility.FindChild(formObj.gameObject, "MapPanel/Big/Button_CloseSkillMap");
			this.extraBtnObj_skillMap.CustomSetActive(false);
			this.mmUGUIRoot = Utility.FindChild(formObj.gameObject, "MapPanel/Mini");
			this.bmUGUIRoot = Utility.FindChild(formObj.gameObject, "MapPanel/Big");
			if (!levelContext.IsMobaMode())
			{
				this.mmUGUIRoot.CustomSetActive(false);
				this.bmUGUIRoot.CustomSetActive(false);
				return;
			}
			CUIEventScript component = this.bmUGUIRoot.GetComponent<CUIEventScript>();
			if (component != null)
			{
				component.c_holdTimeValue = 0.2f;
			}
			GameObject gameObject = Singleton<CGameObjectPool>.GetInstance().GetGameObject(MinimapSys.miniMapPrefabPath, enResourceType.BattleScene);
			DebugHelper.Assert(gameObject != null, "---minimap3DUI is null...");
			gameObject.name = "Mini_Map";
			Camera currentCamera = Singleton<Camera_UI3D>.GetInstance().GetCurrentCamera();
			if (currentCamera == null)
			{
				return;
			}
			gameObject.transform.parent = currentCamera.transform;
			gameObject.transform.localPosition = new Vector3(0f, 0f, MinimapSys.DEPTH);
			this.mmRoot = Utility.FindChild(gameObject.gameObject, "Mini");
			this.bmRoot = Utility.FindChild(gameObject.gameObject, "Big");
			if (this.mmRoot == null || this.bmRoot == null)
			{
				return;
			}
			this.mmRoot.CustomSetActive(true);
			this.bmRoot.CustomSetActive(false);
			string prefabFullPath = CUIUtility.s_Sprite_Dynamic_Map_Dir + levelContext.m_miniMapPath;
			string prefabFullPath2 = CUIUtility.s_Sprite_Dynamic_Map_Dir + levelContext.m_bigMapPath;
			this.mmBGMap_3dui = Singleton<CGameObjectPool>.GetInstance().GetGameObject(prefabFullPath, enResourceType.UI3DImage);
			this.bmBGMap_3dui = Singleton<CGameObjectPool>.GetInstance().GetGameObject(prefabFullPath2, enResourceType.UI3DImage);
			this.mmBGMap_3dui.transform.SetParent(this.mmRoot.transform, true);
			this.mmBGMap_3dui.transform.localScale = new Vector3(1f, 1f, 1f);
			this.mmBGMap_3dui.transform.localRotation = Quaternion.identity;
			this.bmBGMap_3dui.transform.SetParent(this.bmRoot.transform, true);
			this.bmBGMap_3dui.transform.localScale = new Vector3(1f, 1f, 1f);
			this.bmBGMap_3dui.transform.localRotation = Quaternion.identity;
			this.mmBGMap_3dui.transform.SetAsFirstSibling();
			this.bmBGMap_3dui.transform.SetAsFirstSibling();
			Singleton<Camera_UI3D>.GetInstance().GetCurrentCanvas().RefreshLayout(null);
			MiniMapSysUT.NativeSizeLize(gameObject);
			if (!levelContext.IsMobaMode())
			{
				this.mmRoot.SetActive(false);
				this.bmRoot.SetActive(false);
				return;
			}
			this.regEvent();
			this.GetCacheObj();
			this.mmSpt3D = this.mmBGMap_3dui.GetComponent<Sprite3D>();
			this.bmSpt3D = this.bmBGMap_3dui.GetComponent<Sprite3D>();
			if (levelContext.IsMobaMode())
			{
				this.Switch(MinimapSys.EMapType.Mini);
				if (this.mmSpt3D != null)
				{
					this.initWorldUITransformFactor(new Vector2((float)this.mmSpt3D.textureWidth, (float)this.mmSpt3D.textureHeight), levelContext, true, out Singleton<CBattleSystem>.instance.world_UI_Factor_Small, out Singleton<CBattleSystem>.instance.UI_world_Factor_Small, this.mmSpt3D);
				}
				if (this.bmSpt3D != null)
				{
					this.initWorldUITransformFactor(new Vector2((float)this.bmSpt3D.textureWidth, (float)this.bmSpt3D.textureHeight), levelContext, false, out Singleton<CBattleSystem>.instance.world_UI_Factor_Big, out Singleton<CBattleSystem>.instance.UI_world_Factor_Big, this.bmSpt3D);
				}
				if (this.mmSpt3D != null)
				{
					this.m_mmFinalScreenPos.x = (float)this.mmSpt3D.textureWidth * 0.5f * Sprite3D.Ratio();
					this.m_mmFinalScreenPos.y = (float)Screen.height - (float)this.mmSpt3D.textureHeight * 0.5f * Sprite3D.Ratio();
					this.mmSpt3D.transform.position = this.Get3DUIObj_WorldPos(this.m_mmFinalScreenPos.x, this.m_mmFinalScreenPos.y);
					this.mmFinalScreenSize = new Vector2((float)this.mmSpt3D.textureWidth * Sprite3D.Ratio(), (float)this.mmSpt3D.textureHeight * Sprite3D.Ratio());
				}
				if (this.bmSpt3D != null)
				{
					this.m_bmFinalScreenPos.x = (float)this.bmSpt3D.textureWidth * 0.5f * Sprite3D.Ratio();
					this.m_bmFinalScreenPos.y = (float)Screen.height - (float)this.bmSpt3D.textureHeight * 0.5f * Sprite3D.Ratio();
					this.bmSpt3D.transform.position = this.Get3DUIObj_WorldPos(this.m_bmFinalScreenPos.x, this.m_bmFinalScreenPos.y);
					this.bmFinalScreenSize = new Vector2((float)this.bmSpt3D.textureWidth * Sprite3D.Ratio(), (float)this.bmSpt3D.textureHeight * Sprite3D.Ratio());
				}
				if (this.mmSpt3D != null)
				{
					this.m_mmStandardScreenPos.x = (float)this.mmSpt3D.textureWidth * 0.5f;
					this.m_mmStandardScreenPos.y = (float)this.mmSpt3D.textureHeight * 0.5f;
				}
				if (this.bmSpt3D != null)
				{
					this.m_bmStandardScreenPos.x = (float)this.bmSpt3D.textureWidth * 0.5f;
					this.m_bmStandardScreenPos.y = (float)this.bmSpt3D.textureHeight * 0.5f;
				}
				RectTransform rectTransform = this.mmUGUIRoot.transform as RectTransform;
				rectTransform.sizeDelta = new Vector2((float)this.mmSpt3D.textureWidth, (float)this.mmSpt3D.textureHeight);
				rectTransform.anchoredPosition = new Vector2((float)this.mmSpt3D.textureWidth * 0.5f, (float)(-(float)this.mmSpt3D.textureHeight) * 0.5f);
				RectTransform rectTransform2 = this.bmUGUIRoot.transform as RectTransform;
				rectTransform2.sizeDelta = new Vector2((float)this.bmSpt3D.textureWidth, (float)this.bmSpt3D.textureHeight);
				rectTransform2.anchoredPosition = new Vector2((float)this.bmSpt3D.textureWidth * 0.5f, (float)(-(float)this.bmSpt3D.textureHeight) * 0.5f);
				MiniMapSysUT.RefreshMapPointerBig(this.mmUGUIRoot);
				MiniMapSysUT.RefreshMapPointerBig(this.bmUGUIRoot);
				if (levelContext.m_pvpPlayerNum == 6)
				{
					GameObject gameObject2 = Utility.FindChild(this.ownerForm.gameObject, "MapPanel/DragonInfo");
					if (gameObject2 != null)
					{
						RectTransform rectTransform3 = gameObject2.gameObject.transform as RectTransform;
						if (rectTransform3 != null)
						{
							rectTransform3.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, rectTransform3.anchoredPosition.y);
						}
					}
				}
			}
			else
			{
				this.Switch(MinimapSys.EMapType.None);
			}
			this.curMapType = MinimapSys.EMapType.Mini;
			bool flag = false;
			bool b5V = false;
			if (levelContext.m_pveLevelType == RES_LEVEL_TYPE.RES_LEVEL_TYPE_GUIDE)
			{
				int mapID = levelContext.m_mapID;
				if (mapID == 2)
				{
					flag = true;
					b5V = false;
				}
				if (mapID == 3 || mapID == 6 || mapID == 7)
				{
					flag = true;
					b5V = true;
				}
			}
			else if (levelContext.m_pvpPlayerNum == 6 || levelContext.m_pvpPlayerNum == 10)
			{
				flag = true;
				b5V = (levelContext.m_pvpPlayerNum == 10);
			}
			if (flag && this.mmpcDragon_ugui)
			{
				this.m_dragonIcon = new DragonIcon();
				this.m_dragonIcon.Init(this.mmpcDragon_ugui, this.bmpcDragon_ugui, this.mmpcDragon_3dui, this.bmpcDragon_3dui, b5V);
				this.mmpcDragon_ugui.CustomSetActive(true);
				this.bmpcDragon_ugui.CustomSetActive(true);
			}
			else
			{
				this.mmpcDragon_ugui.CustomSetActive(false);
				this.bmpcDragon_ugui.CustomSetActive(false);
			}
			GameObject gameObject3 = this.mmUGUIRoot.transform.Find("CameraFrame").gameObject;
			if (gameObject3 == null)
			{
				return;
			}
			this.MMiniMapCameraFrame_3Dui = new MiniMapCameraFrame_3DUI(gameObject3, (float)this.mmSpt3D.textureWidth, (float)this.mmSpt3D.textureHeight);
			this.MMiniMapCameraFrame_3Dui.SetFrameSize((CameraHeightType)GameSettings.CameraHeight);
			this.MMiniMapBackCityCom_3Dui = new BackCityCom_3DUI();
			this.MMiniMapTrack_3Dui = new MiniMapTrack_3DUI();
			this.MMinimapSkillIndicator_3Dui = new MinimapSkillIndicator_3DUI();
			GameObject miniTrackNode = Utility.FindChild(this.mmUGUIRoot, "Track");
			GameObject bigTrackNode = Utility.FindChild(this.bmUGUIRoot, "Track");
			this.MMinimapSkillIndicator_3Dui.Init(miniTrackNode, bigTrackNode);
			this.miniMapSignalPanel.Init(formObj);
			this.ChangeBigMapSide(true);
			this.InitAllTransformWithBigMapMask();
		}

		private Vector3 Get3DUIObj_WorldPos(float finalScreenX, float finalScreenY)
		{
			Camera currentCamera = Singleton<Camera_UI3D>.GetInstance().GetCurrentCamera();
			if (currentCamera == null)
			{
				return Vector3.zero;
			}
			Vector3 position = new Vector3(finalScreenX, finalScreenY, MinimapSys.DEPTH);
			return currentCamera.ScreenToWorldPoint(position);
		}

		public void Clear()
		{
			this.cachePos = Vector3.zero;
			this.extraBtnObj_bigMap = null;
			this.extraBtnObj_skillMap = null;
			this.unRegEvent();
			if (this.MMiniMapTrack_3Dui != null)
			{
				this.MMiniMapTrack_3Dui.Clear();
				this.MMiniMapTrack_3Dui = null;
			}
			if (this.m_dragonIcon != null)
			{
				this.m_dragonIcon.Clear();
				this.m_dragonIcon = null;
			}
			if (this.MMiniMapCameraFrame_3Dui != null)
			{
				this.MMiniMapCameraFrame_3Dui.Clear();
				this.MMiniMapCameraFrame_3Dui = null;
			}
			if (this.MMinimapSkillIndicator_3Dui != null)
			{
				this.MMinimapSkillIndicator_3Dui.Clear();
				this.MMinimapSkillIndicator_3Dui = null;
			}
			if (this.miniMapEffectModule != null)
			{
				this.miniMapEffectModule.Clear();
				this.miniMapEffectModule = null;
			}
			if (this.miniMapSignalPanel != null)
			{
				this.miniMapSignalPanel.Clear();
				this.miniMapSignalPanel = null;
			}
			this.mmRoot = null;
			this.bmRoot = null;
			this.mmSpt3D = null;
			this.mmBGMap_3dui = null;
			this.mmpcAlies = null;
			this.mmpcHeroBgFrame_Friend = null;
			this.mmpcHeroBgFrame_Enemy = null;
			this.mmpcRedBuff = null;
			this.mmpcBlueBuff = null;
			this.mmpcJungle = null;
			this.mmpcEnemy = null;
			this.mmpcOrgan = null;
			this.mmpcSignal = null;
			this.mmpcDragon_ugui = null;
			this.mmpcDragon_3dui = null;
			this.mmpcEffect = null;
			this.mmpcEye = null;
			this.mmpcTrack = null;
			this.mmHeroIconNode = null;
			this.mmHeroIconNode_Self = null;
			this.mmHeroIconNode_Friend = null;
			this.mmHeroIconNode_Enemy = null;
			this.bmSpt3D = null;
			this.bmBGMap_3dui = null;
			this.bmpcAlies = null;
			this.bmpcHeroBgFrame_Friend = null;
			this.bmpcHeroBgFrame_Enemy = null;
			this.bmpcRedBuff = null;
			this.bmpcBlueBuff = null;
			this.bmpcJungle = null;
			this.bmpcEnemy = null;
			this.bmpcOrgan = null;
			this.bmpcSignal = null;
			this.bmpcDragon_ugui = null;
			this.bmpcDragon_3dui = null;
			this.mmpcEffect = null;
			this.bmpcEye = null;
			this.bmpcTrack = null;
			this.bmHeroIconNode = null;
			this.bmHeroIconNode_Self = null;
			this.bmHeroIconNode_Friend = null;
			this.bmHeroIconNode_Enemy = null;
			this.ownerForm = null;
			this.m_playerCamp = COM_PLAYERCAMP.COM_PLAYERCAMP_COUNT;
			this.m_elementIndex = 0;
			for (int i = 0; i < this.m_mapElements.Count; i++)
			{
				if (this.m_mapElements[i].smallElement != null)
				{
					MiniMapSysUT.RecycleMapGameObject(this.m_mapElements[i].smallElement);
					this.m_mapElements[i].smallElement = null;
				}
				if (this.m_mapElements[i].bigElement != null)
				{
					MiniMapSysUT.RecycleMapGameObject(this.m_mapElements[i].bigElement);
					this.m_mapElements[i].bigElement = null;
				}
			}
			this.m_mapElements.Clear();
			this.m_bornRecords.Clear();
		}

		public MinimapSys.EMapType CurMapType()
		{
			return this.curMapType;
		}

		public void Switch(MinimapSys.EMapType type)
		{
			this.curMapType = type;
			if (this.ownerForm == null)
			{
				return;
			}
			if (this.curMapType == MinimapSys.EMapType.Mini)
			{
				CUICommonSystem.SetObjActive(this.mmRoot, true);
				CUICommonSystem.SetObjActive(this.mmUGUIRoot, true);
				this.extraBtnObj_bigMap.CustomSetActive(false);
				this.extraBtnObj_skillMap.CustomSetActive(false);
				this.SetMiniMapElementVisible(true);
				CUICommonSystem.SetObjActive(this.bmRoot, false);
				CUICommonSystem.SetObjActive(this.bmUGUIRoot, false);
				CUIEventScript component = this.bmUGUIRoot.GetComponent<CUIEventScript>();
				if (component != null)
				{
					component.ClearInputStatus();
				}
				if (Singleton<CBattleSystem>.instance.FightForm != null)
				{
					Singleton<CBattleSystem>.instance.FightForm.SetBigMapBtnVisible(true);
				}
				this.SetObjectCoveredBySkillMapActive(true, true);
				this.HideTeleIndicator();
				this.miniMapSignalPanel.Hide();
				this.EnableShowFow(MinimapSys.EMapType.Mini, true);
				this.EnableShowFow(MinimapSys.EMapType.Big, true);
			}
			else if (this.curMapType == MinimapSys.EMapType.Big)
			{
				bool flag = GameSettings.MiniMapPosMode == MiniMapPosType.MiniMapLeftMode;
				CUICommonSystem.SetObjActive(this.mmRoot, !flag);
				CUICommonSystem.SetObjActive(this.mmUGUIRoot, !flag);
				CUICommonSystem.SetObjActive(this.bmRoot, true);
				CUICommonSystem.SetObjActive(this.bmUGUIRoot, true);
				this.SetMiniMapElementVisible(false);
				this.extraBtnObj_bigMap.CustomSetActive(flag);
				this.extraBtnObj_skillMap.CustomSetActive(!flag);
				if (Singleton<CBattleSystem>.instance.FightForm != null)
				{
					Singleton<CBattleSystem>.instance.FightForm.SetBigMapBtnVisible(false);
				}
				this.ChangeBigMapSide(flag);
				this.ChangeMapPointerDepth(this.curMapType);
				if (!flag)
				{
					this.SetObjectCoveredBySkillMapActive(false, true);
				}
				this.HideTeleIndicator();
				this.EnableShowFow(MinimapSys.EMapType.Mini, false);
				this.EnableShowFow(MinimapSys.EMapType.Big, true);
			}
			else if (this.curMapType == MinimapSys.EMapType.Skill)
			{
				CUICommonSystem.SetObjActive(this.mmRoot, true);
				CUICommonSystem.SetObjActive(this.mmUGUIRoot, true);
				CUICommonSystem.SetObjActive(this.bmRoot, true);
				CUICommonSystem.SetObjActive(this.bmUGUIRoot, true);
				this.SetMiniMapElementVisible(false);
				this.extraBtnObj_bigMap.CustomSetActive(false);
				this.extraBtnObj_skillMap.CustomSetActive(true);
				if (Singleton<CBattleSystem>.instance.FightForm != null)
				{
					Singleton<CBattleSystem>.instance.FightForm.SetBigMapBtnVisible(false);
				}
				this.ChangeBigMapSide(false);
				this.ChangeMapPointerDepth(this.curMapType);
				this.SetObjectCoveredBySkillMapActive(false, false);
				this.ShowTeleIndicator();
				this.EnableShowFow(MinimapSys.EMapType.Mini, false);
				this.EnableShowFow(MinimapSys.EMapType.Big, true);
			}
			else
			{
				this.mmRoot.CustomSetActive(false);
				CUICommonSystem.SetObjActive(this.mmUGUIRoot, false);
				this.bmRoot.CustomSetActive(false);
				CUICommonSystem.SetObjActive(this.bmUGUIRoot, false);
				this.SetObjectCoveredBySkillMapActive(true, true);
				this.HideTeleIndicator();
				this.EnableShowFow(MinimapSys.EMapType.Mini, true);
				this.EnableShowFow(MinimapSys.EMapType.Big, true);
			}
			this.m_CurSelectedSlotType = SkillSlotType.SLOT_SKILL_COUNT;
			if (this.MMinimapSkillIndicator_3Dui != null)
			{
				this.MMinimapSkillIndicator_3Dui.ForceUpdate();
			}
			if (this.curMapType == MinimapSys.EMapType.Mini)
			{
				this.ShowTransformWithBigMapMask();
			}
			else
			{
				this.HideTransformWithBigMapMask();
			}
		}

		private void SetMiniMapElementVisible(bool bShow)
		{
			this.mmpcAlies.CustomSetActive(bShow);
			this.mmpcHeroBgFrame_Friend.CustomSetActive(bShow);
			this.mmpcHeroBgFrame_Enemy.CustomSetActive(bShow);
			this.mmpcRedBuff.CustomSetActive(bShow);
			this.mmpcBlueBuff.CustomSetActive(bShow);
			this.mmpcJungle.CustomSetActive(bShow);
			this.mmpcEnemy.CustomSetActive(bShow);
			this.mmpcOrgan.CustomSetActive(bShow);
			this.mmpcSignal.CustomSetActive(bShow);
			this.mmpcDragon_3dui.CustomSetActive(bShow);
			this.mmpcEye.CustomSetActive(bShow);
			this.mmHeroIconNode.CustomSetActive(bShow);
		}

		private void ShowTeleIndicator()
		{
			this.ShowTeleIndicator(this.UI3DEventMgr.TowerList);
			this.ShowTeleIndicator(this.UI3DEventMgr.EyesList);
		}

		private void ShowTeleIndicator(ListView<UI3DEventCom> list)
		{
			for (int i = 0; i < list.Count; i++)
			{
				UI3DEventCom uI3DEventCom = list[i];
				if (uI3DEventCom != null && uI3DEventCom.bHostSameCamp && !uI3DEventCom.isDead)
				{
					if (uI3DEventCom.UIParticleInfo == null && !uI3DEventCom.bCreateParticleByPosition)
					{
						uI3DEventCom.bCreateParticleByPosition = true;
					}
					if (uI3DEventCom.UIParticleInfo != null && uI3DEventCom.UIParticleInfo.parObj != null)
					{
						uI3DEventCom.UIParticleInfo.parObj.CustomSetActive(true);
					}
				}
			}
		}

		private void HideTeleIndicator()
		{
			this.HideTeleIndicator(this.UI3DEventMgr.TowerList);
			this.HideTeleIndicator(this.UI3DEventMgr.EyesList);
		}

		private void HideTeleIndicator(ListView<UI3DEventCom> list)
		{
			for (int i = 0; i < list.Count; i++)
			{
				UI3DEventCom uI3DEventCom = list[i];
				if (uI3DEventCom != null && uI3DEventCom.bHostSameCamp && !uI3DEventCom.isDead && uI3DEventCom.UIParticleInfo != null && uI3DEventCom.UIParticleInfo.parObj != null)
				{
					uI3DEventCom.UIParticleInfo.parObj.CustomSetActive(false);
				}
			}
		}

		public void Switch(MinimapSys.EMapType type, SkillSlotType slotType)
		{
			this.Switch(type);
			this.m_CurSelectedSlotType = slotType;
		}

		public void ChangeMapPointerDepth(MinimapSys.EMapType mapType)
		{
			if (mapType == MinimapSys.EMapType.Skill)
			{
				this.bmpcOrgan.transform.localPosition = new Vector3(0f, 0f, -20f);
				this.bmpcEye.transform.localPosition = new Vector3(0f, 0f, -21f);
				this.bmpcAlies.transform.localPosition = new Vector3(0f, 0f, -22f);
				this.bmpcEnemy.transform.localPosition = new Vector3(0f, 0f, -22f);
			}
			else
			{
				this.bmpcOrgan.transform.localPosition = Vector3.zero;
				this.bmpcEye.transform.localPosition = Vector3.zero;
				this.bmpcAlies.transform.localPosition = Vector3.zero;
				this.bmpcEnemy.transform.localPosition = Vector3.zero;
			}
		}

		public void ChangeBigMapSide(bool bLeft)
		{
			if (Singleton<WatchController>.instance.IsWatching)
			{
				return;
			}
			if (this.bmBGMap_3dui != null && this.bmRoot != null && this.bmUGUIRoot != null)
			{
				Sprite3D component = this.bmBGMap_3dui.GetComponent<Sprite3D>();
				if (component == null)
				{
				}
				this.m_bmFinalScreenPos.x = ((!bLeft) ? ((float)Screen.width - (float)component.textureWidth * 0.5f * Sprite3D.Ratio()) : ((float)component.textureWidth * 0.5f * Sprite3D.Ratio()));
				this.m_bmFinalScreenPos.y = (float)Screen.height - (float)component.textureHeight * 0.5f * Sprite3D.Ratio();
				if (bLeft)
				{
					if (this.cachePos == Vector3.zero)
					{
						this.cachePos = this.Get3DUIObj_WorldPos(this.m_bmFinalScreenPos.x, this.m_bmFinalScreenPos.y);
					}
					component.transform.position = this.cachePos;
				}
				if (!bLeft)
				{
					component.transform.position = this.Get3DUIObj_WorldPos(this.m_bmFinalScreenPos.x, this.m_bmFinalScreenPos.y);
				}
				List<PoolObjHandle<ActorRoot>> gameActors = Singleton<GameObjMgr>.GetInstance().GameActors;
				for (int i = 0; i < gameActors.Count; i++)
				{
					PoolObjHandle<ActorRoot> ptr = gameActors[i];
					if (ptr && ptr.handle != null && ptr.handle.HudControl != null)
					{
						ptr.handle.HudControl.AddForceUpdateFlag();
					}
				}
				if (this.ownerForm != null)
				{
					RectTransform rectTransform = this.bmUGUIRoot.transform as RectTransform;
					rectTransform.sizeDelta = new Vector2((float)component.textureWidth, (float)component.textureHeight);
					float x = (!bLeft) ? (this.ownerForm.ChangeScreenValueToForm((float)Screen.width) - (float)component.textureWidth * 0.5f) : ((float)component.textureWidth * 0.5f);
					rectTransform.anchoredPosition = new Vector2(x, (float)(-(float)component.textureHeight) * 0.5f);
				}
				if (this.m_dragonIcon != null)
				{
					this.m_dragonIcon.RefreshDragNode(bLeft, false);
				}
				MiniMapSysUT.RefreshMapPointerBig(this.bmUGUIRoot);
			}
		}

		private void SetObjectCoveredBySkillMapActive(bool active, bool bSendDragonSignal)
		{
			if (Singleton<WatchController>.instance.IsWatching || Singleton<CBattleSystem>.instance.FightForm == null)
			{
				return;
			}
			this.SetSkillMapDragonEventActive(bSendDragonSignal);
			if (Singleton<CBattleSystem>.instance.FightForm.m_enemyHeroAtkBtn != null)
			{
				if (!active)
				{
					Singleton<CBattleSystem>.instance.FightForm.m_enemyHeroAtkBtn.HideEnemyHeroHeadBtn();
				}
				else
				{
					Singleton<CBattleSystem>.instance.FightForm.m_enemyHeroAtkBtn.ShowEnemyHeroHeadBtn();
				}
			}
			this.SetSkillBtnSelectActive(!active);
		}

		public void SetSkillBtnSelectActive(bool bIsActive)
		{
			if (Singleton<WatchController>.instance.IsWatching)
			{
				return;
			}
			if (Singleton<CBattleSystem>.instance.FightForm != null)
			{
				CSkillButtonManager skillButtonManager = Singleton<CBattleSystem>.instance.FightForm.m_skillButtonManager;
				if (skillButtonManager != null)
				{
					skillButtonManager.SetSkillButtuonSelectActive(SkillSlotType.SLOT_SKILL_5, bIsActive && this.curMapType == MinimapSys.EMapType.Skill);
				}
			}
		}

		public void SetSkillBtnActive(bool active)
		{
			if (Singleton<WatchController>.instance.IsWatching)
			{
				return;
			}
			if (Singleton<CBattleSystem>.instance.FightForm != null)
			{
				CSkillButtonManager skillButtonManager = Singleton<CBattleSystem>.instance.FightForm.m_skillButtonManager;
				if (skillButtonManager != null)
				{
					skillButtonManager.SetSkillButtuonActive(SkillSlotType.SLOT_SKILL_2, active);
					skillButtonManager.SetSkillButtuonActive(SkillSlotType.SLOT_SKILL_3, active);
					if (active)
					{
						Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
						if (hostPlayer != null && hostPlayer.Captain && hostPlayer.Captain.handle.EquipComponent != null && hostPlayer.Captain.handle.EquipComponent.GetEquipActiveSkillShowFlag(SkillSlotType.SLOT_SKILL_9))
						{
							skillButtonManager.SetSkillButtuonActive(SkillSlotType.SLOT_SKILL_9, true);
						}
					}
					else
					{
						skillButtonManager.SetSkillButtuonActive(SkillSlotType.SLOT_SKILL_9, false);
					}
				}
			}
		}

		public void SetSkillMapDragonEventActive(bool bIsActive)
		{
			if (Singleton<WatchController>.instance.IsWatching)
			{
				return;
			}
			if (Singleton<CBattleSystem>.instance.FightForm != null)
			{
				GameObject bigMapDragonContainer = Singleton<CBattleSystem>.instance.FightForm.GetBigMapDragonContainer();
				if (bigMapDragonContainer && bigMapDragonContainer.transform)
				{
					int childCount = bigMapDragonContainer.transform.childCount;
					for (int i = 0; i < childCount; i++)
					{
						Transform child = bigMapDragonContainer.transform.GetChild(i);
						if (child)
						{
							CUIEventScript component = child.GetComponent<CUIEventScript>();
							if (component)
							{
								component.enabled = bIsActive;
							}
						}
					}
				}
			}
		}

		public void ClearMapSkillStatus()
		{
			if (this.curMapType == MinimapSys.EMapType.Skill)
			{
				this.Switch(MinimapSys.EMapType.Mini);
			}
		}

		private void initWorldUITransformFactor(Vector2 mapImgSize, SLevelContext levelContext, bool bMiniMap, out Vector2 world_UI_Factor, out Vector2 UI_world_Factor, Sprite3D sprite)
		{
			int num = (!bMiniMap) ? levelContext.m_bigMapWidth : levelContext.m_mapWidth;
			int num2 = (!bMiniMap) ? levelContext.m_bigMapHeight : levelContext.m_mapHeight;
			float num3 = mapImgSize.x / (float)num;
			float num4 = mapImgSize.y / (float)num2;
			world_UI_Factor = new Vector2(num3, num4);
			float num5 = (float)num / mapImgSize.x;
			float num6 = (float)num2 / mapImgSize.y;
			UI_world_Factor = new Vector2(num5, num6);
			if (levelContext.m_isCameraFlip)
			{
				world_UI_Factor = new Vector2(-num3, -num4);
				UI_world_Factor = new Vector2(-num5, -num6);
			}
			if (null != sprite)
			{
				float x = (!bMiniMap) ? levelContext.m_bigMapFowScale : levelContext.m_mapFowScale;
				float y = (!levelContext.m_isCameraFlip) ? 1f : 0f;
				sprite.atlas.material.SetVector("_FowParams", new Vector4(x, y, 1f, 1f));
			}
		}

		public void EnableShowFow(MinimapSys.EMapType type, bool bShowFow)
		{
			bool flag = type == MinimapSys.EMapType.Mini;
			Sprite3D sprite3D = (!flag) ? this.bmSpt3D : this.mmSpt3D;
			if (sprite3D == null)
			{
				return;
			}
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			float x = (!flag) ? curLvelContext.m_bigMapFowScale : curLvelContext.m_mapFowScale;
			if (!bShowFow)
			{
				x = 0f;
			}
			if (sprite3D.atlas != null && sprite3D.atlas.material != null)
			{
				sprite3D.atlas.material.SetVector("_FowParams", new Vector4(x, (!curLvelContext.m_isCameraFlip) ? 1f : 0f, 1f, 1f));
			}
		}

		private void regEvent()
		{
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BigMap_Open_BigMap, new CUIEventManager.OnUIEventHandler(this.OnBigMap_Open_BigMap));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_CloseBigMap, new CUIEventManager.OnUIEventHandler(this.OnCloseBigMap));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BigMap_Click_5_Dalong, new CUIEventManager.OnUIEventHandler(this.OnBigMap_Click_5_Dalong));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BigMap_Click_5_Xiaolong, new CUIEventManager.OnUIEventHandler(this.OnBigMap_Click_5_Xiaolong));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BigMap_Click_3_long, new CUIEventManager.OnUIEventHandler(this.OnBigMap_Click_3_long));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BigMap_Click_Organ, new CUIEventManager.OnUIEventHandler(this.OnBigMap_Click_Organ));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BigMap_Click_Hero, new CUIEventManager.OnUIEventHandler(this.OnBigMap_Click_Hero));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BigMap_Click_Map, new CUIEventManager.OnUIEventHandler(this.OnBigMap_Click_Map));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BigMap_Click_Eye, new CUIEventManager.OnUIEventHandler(this.OnBigMap_Click_Eye));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BigMap_Click_Solider, new CUIEventManager.OnUIEventHandler(this.OnBigMap_Click_Solider));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BigMap_Hold_EmptyArea, new CUIEventManager.OnUIEventHandler(this.OnBigMap_Hold_EmptyArea));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BigMap_HoldEnd, new CUIEventManager.OnUIEventHandler(this.OnBigMap_HoldEnd));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BigMap_SignalPanel_Drag, new CUIEventManager.OnUIEventHandler(this.OnBigMap_SignalPanel_Drag));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_Down_MiniMap, new CUIEventManager.OnUIEventHandler(this.OnMiniMap_Click_Down));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_Click_MiniMap_Up, new CUIEventManager.OnUIEventHandler(this.OnMiniMap_Click_Up));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_Drag_SignalPanel, new CUIEventManager.OnUIEventHandler(this.OnDragMiniMap));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_Drag_SignalPanel_End, new CUIEventManager.OnUIEventHandler(this.OnDragMiniMapEnd));
			Singleton<GameEventSys>.GetInstance().AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorBeAttack, new RefAction<DefaultGameEventParam>(this.OnBuildingAttacked));
			Singleton<GameEventSys>.GetInstance().AddEventHandler<GameDeadEventParam>(GameEventDef.Event_MonsterGroupDead, new RefAction<GameDeadEventParam>(this.OnMonsterGroupDead));
			Singleton<GameEventSys>.GetInstance().AddEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.OnActorDead));
			Singleton<GameEventSys>.GetInstance().AddEventHandler<EyePerishWarnParam>(GameEventDef.Event_EyePerish, new RefAction<EyePerishWarnParam>(this.OnEvent_EyePerish));
		}

		private void OnEvent_EyePerish(ref EyePerishWarnParam prm)
		{
			if (!prm.SrcEye.handle.IsHostCamp())
			{
				return;
			}
			if (this.CurMapType() == MinimapSys.EMapType.Big || this.CurMapType() == MinimapSys.EMapType.Skill)
			{
				Vector3 position = prm.SrcEye.handle.gameObject.transform.position;
				Vector2 sreenLoc = MiniMapSysUT.CalcScreenPosInMapByWorldPos(ref position, false);
				UIParticleInfo uIParticleInfo = Singleton<CUIParticleSystem>.instance.AddParticle(MiniMapEffectModule.EyePerishEft, (float)(prm.LeftTime / 1000 + 2), sreenLoc, null);
				if (uIParticleInfo != null && uIParticleInfo.parObj != null)
				{
					ParticleScaler component = uIParticleInfo.parObj.GetComponent<ParticleScaler>();
					if (component != null)
					{
						component.particleScale = 4f;
						component.CheckAndApplyScale();
					}
				}
			}
			else if (this.CurMapType() == MinimapSys.EMapType.Mini)
			{
				Vector3 position2 = prm.SrcEye.handle.gameObject.transform.position;
				Vector2 sreenLoc2 = MiniMapSysUT.CalcScreenPosInMapByWorldPos(ref position2, true);
				UIParticleInfo uIParticleInfo2 = Singleton<CUIParticleSystem>.instance.AddParticle(MiniMapEffectModule.EyePerishEft, (float)(prm.LeftTime / 1000 + 2), sreenLoc2, null);
				if (uIParticleInfo2 != null && uIParticleInfo2.parObj != null)
				{
					ParticleScaler component2 = uIParticleInfo2.parObj.GetComponent<ParticleScaler>();
					if (component2 != null)
					{
						component2.particleScale = 2f;
						component2.CheckAndApplyScale();
					}
				}
			}
		}

		private void OnMonsterGroupDead(ref GameDeadEventParam evtParam)
		{
			if (FogOfWar.enable && !Singleton<WatchController>.instance.IsWatching)
			{
				SpawnGroup x = evtParam.spawnPoint as SpawnGroup;
				if (evtParam.src && evtParam.orignalAtker && x != null)
				{
					bool flag = evtParam.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster && evtParam.src.handle.ActorControl.GetActorSubType() == 2;
					byte actorSubSoliderType = evtParam.src.handle.ActorControl.GetActorSubSoliderType();
					bool flag2 = actorSubSoliderType == 7 || actorSubSoliderType == 8 || actorSubSoliderType == 9;
					if (flag && !flag2)
					{
						Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
						if (hostPlayer.PlayerCamp != evtParam.orignalAtker.handle.TheActorMeta.ActorCamp)
						{
							GameObject mapPointerSmall = evtParam.src.handle.HudControl.MapPointerSmall;
							GameObject mapPointerBig = evtParam.src.handle.HudControl.MapPointerBig;
							if (mapPointerSmall != null && mapPointerBig != null)
							{
								UI3DEventCom uI3DEventCom = null;
								GameObject mapGameObject = MiniMapSysUT.GetMapGameObject(evtParam.src, true, out uI3DEventCom);
								GameObject mapGameObject2 = MiniMapSysUT.GetMapGameObject(evtParam.src, false, out uI3DEventCom);
								if (mapGameObject != null)
								{
									mapGameObject.transform.localPosition = mapPointerSmall.transform.localPosition;
									mapGameObject.transform.localRotation = mapPointerSmall.transform.localRotation;
									mapGameObject.transform.localScale = mapPointerSmall.transform.localScale;
									mapGameObject.CustomSetActive(true);
								}
								if (mapGameObject2 != null)
								{
									mapGameObject2.transform.localPosition = mapPointerBig.transform.localPosition;
									mapGameObject2.transform.localRotation = mapPointerBig.transform.localRotation;
									mapGameObject2.transform.localScale = mapPointerBig.transform.localScale;
									mapGameObject2.CustomSetActive(true);
								}
								this.m_mapElements.Add(new MinimapSys.ElementInMap(evtParam.src.handle.BornPos, mapGameObject, mapGameObject2));
							}
						}
					}
				}
			}
		}

		private void OnBuildingAttacked(ref DefaultGameEventParam evtParam)
		{
			if (!evtParam.src)
			{
				return;
			}
			ActorRoot handle = evtParam.src.handle;
			int organType = handle.TheStaticData.TheOrganOnlyInfo.OrganType;
			if (organType == 1 || organType == 4 || organType == 2)
			{
				float single = handle.ValueComponent.GetHpRate().single;
				HudComponent3D hudControl = handle.HudControl;
				if (hudControl == null)
				{
					return;
				}
				if (single < 1f)
				{
					TowerHitMgr towerHitMgr = Singleton<CBattleSystem>.GetInstance().TowerHitMgr;
					GameObject mapPointerSmall = hudControl.MapPointerSmall;
					if (mapPointerSmall != null && towerHitMgr != null)
					{
						towerHitMgr.TryActive(handle.ObjID, mapPointerSmall);
					}
				}
				Sprite3D bigTower_Spt3D = hudControl.GetBigTower_Spt3D();
				Sprite3D smallTower_Spt3D = hudControl.GetSmallTower_Spt3D();
				if (bigTower_Spt3D != null && smallTower_Spt3D != null && handle.ValueComponent != null)
				{
					Sprite3D arg_ED_0 = smallTower_Spt3D;
					float fillAmount = single;
					bigTower_Spt3D.fillAmount = fillAmount;
					arg_ED_0.fillAmount = fillAmount;
				}
			}
		}

		private void OnActorDead(ref GameDeadEventParam prm)
		{
			if (!prm.src)
			{
				return;
			}
			if (ActorHelper.IsHostCtrlActor(ref prm.src) && this.curMapType == MinimapSys.EMapType.Skill)
			{
				this.Switch(MinimapSys.EMapType.Mini);
			}
		}

		private void unRegEvent()
		{
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BigMap_Open_BigMap, new CUIEventManager.OnUIEventHandler(this.OnBigMap_Open_BigMap));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_CloseBigMap, new CUIEventManager.OnUIEventHandler(this.OnCloseBigMap));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BigMap_Click_5_Dalong, new CUIEventManager.OnUIEventHandler(this.OnBigMap_Click_5_Dalong));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BigMap_Click_5_Xiaolong, new CUIEventManager.OnUIEventHandler(this.OnBigMap_Click_5_Xiaolong));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BigMap_Click_3_long, new CUIEventManager.OnUIEventHandler(this.OnBigMap_Click_3_long));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BigMap_Click_Organ, new CUIEventManager.OnUIEventHandler(this.OnBigMap_Click_Organ));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BigMap_Click_Hero, new CUIEventManager.OnUIEventHandler(this.OnBigMap_Click_Hero));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BigMap_Click_Map, new CUIEventManager.OnUIEventHandler(this.OnBigMap_Click_Map));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BigMap_Click_Eye, new CUIEventManager.OnUIEventHandler(this.OnBigMap_Click_Eye));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BigMap_Click_Solider, new CUIEventManager.OnUIEventHandler(this.OnBigMap_Click_Solider));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BigMap_SignalPanel_Drag, new CUIEventManager.OnUIEventHandler(this.OnBigMap_SignalPanel_Drag));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_Down_MiniMap, new CUIEventManager.OnUIEventHandler(this.OnMiniMap_Click_Down));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_Click_MiniMap_Up, new CUIEventManager.OnUIEventHandler(this.OnMiniMap_Click_Up));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_Drag_SignalPanel, new CUIEventManager.OnUIEventHandler(this.OnDragMiniMap));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_Drag_SignalPanel_End, new CUIEventManager.OnUIEventHandler(this.OnDragMiniMapEnd));
			Singleton<GameEventSys>.GetInstance().RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.OnActorDead));
			Singleton<GameEventSys>.GetInstance().RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorBeAttack, new RefAction<DefaultGameEventParam>(this.OnBuildingAttacked));
			Singleton<GameEventSys>.GetInstance().RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_MonsterGroupDead, new RefAction<GameDeadEventParam>(this.OnMonsterGroupDead));
			Singleton<GameEventSys>.GetInstance().RmvEventHandler<EyePerishWarnParam>(GameEventDef.Event_EyePerish, new RefAction<EyePerishWarnParam>(this.OnEvent_EyePerish));
		}

		private void OnBigMap_Open_BigMap(CUIEvent uievent)
		{
			CPlayerBehaviorStat.Plus(CPlayerBehaviorStat.BehaviorType.Battle_OpenBigMap);
			this.Switch(MinimapSys.EMapType.Big);
			this.RefreshMapPointers();
		}

		public void RefreshMapPointers()
		{
			List<PoolObjHandle<ActorRoot>> heroActors = Singleton<GameObjMgr>.GetInstance().HeroActors;
			for (int i = 0; i < heroActors.Count; i++)
			{
				PoolObjHandle<ActorRoot> ptr = heroActors[i];
				if (ptr && ptr.handle.HudControl != null)
				{
					ptr.handle.HudControl.RefreshMapPointerBig();
				}
			}
		}

		private void OnCloseBigMap(CUIEvent uiEvent)
		{
			if (this.curMapType == MinimapSys.EMapType.Big || this.curMapType == MinimapSys.EMapType.Skill)
			{
				this.Switch(MinimapSys.EMapType.Mini);
			}
		}

		private void OnMiniMap_Click_Down(CUIEvent uievent)
		{
			SignalPanel theSignalPanel = Singleton<CBattleSystem>.GetInstance().TheSignalPanel;
			if (theSignalPanel == null)
			{
				this.Move_CameraToClickPosition(uievent);
			}
			else if (!theSignalPanel.IsUseSingalButton())
			{
				this.Move_CameraToClickPosition(uievent);
			}
		}

		public void Move_CameraToClickPosition(CUIEvent uiEvent)
		{
			if (MonoSingleton<CameraSystem>.GetInstance().enableLockedCamera)
			{
				MonoSingleton<CameraSystem>.instance.ToggleFreeDragCamera(true);
				if (Singleton<CBattleSystem>.GetInstance().WatchForm != null)
				{
					Singleton<CBattleSystem>.GetInstance().WatchForm.FreeFocus();
				}
			}
			CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
			if (srcFormScript != null && uiEvent.m_srcWidget != null && uiEvent.m_pointerEventData != null)
			{
				Vector2 position = uiEvent.m_pointerEventData.position;
				MinimapSys theMinimapSys = Singleton<CBattleSystem>.GetInstance().TheMinimapSys;
				if (theMinimapSys == null)
				{
					return;
				}
				Vector2 mMFianlScreenPos = theMinimapSys.GetMMFianlScreenPos();
				float num = position.x - mMFianlScreenPos.x;
				float num2 = position.y - mMFianlScreenPos.y;
				num = uiEvent.m_srcFormScript.ChangeScreenValueToForm(num);
				num2 = uiEvent.m_srcFormScript.ChangeScreenValueToForm(num2);
				float x = num * Singleton<CBattleSystem>.instance.UI_world_Factor_Small.x;
				float z = num2 * Singleton<CBattleSystem>.instance.UI_world_Factor_Small.y;
				if (MonoSingleton<CameraSystem>.instance.MobaCamera != null)
				{
					MonoSingleton<CameraSystem>.instance.MobaCamera.SetAbsoluteLockLocation(new Vector3(x, 1f, z));
					if (this.MMiniMapCameraFrame_3Dui != null)
					{
						if (!this.MMiniMapCameraFrame_3Dui.IsCameraFrameShow)
						{
							this.MMiniMapCameraFrame_3Dui.Show();
							this.MMiniMapCameraFrame_3Dui.ShowNormal();
						}
						this.MMiniMapCameraFrame_3Dui.SetPos(num, num2);
					}
				}
			}
		}

		private void OnMiniMap_Click_Up(CUIEvent uievent)
		{
			if (this.MMiniMapCameraFrame_3Dui != null)
			{
				this.MMiniMapCameraFrame_3Dui.Hide();
			}
			Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
			if (!Singleton<WatchController>.GetInstance().IsWatching && hostPlayer != null && hostPlayer.Captain && hostPlayer.Captain.handle.ActorControl != null && (!hostPlayer.Captain.handle.ActorControl.IsDeadState || hostPlayer.Captain.handle.TheStaticData.TheBaseAttribute.DeadControl))
			{
				MonoSingleton<CameraSystem>.instance.ToggleFreeDragCamera(false);
			}
		}

		public void OnActorDamage(ref HurtEventResultInfo hri)
		{
			if (this.MMiniMapCameraFrame_3Dui == null || !this.MMiniMapCameraFrame_3Dui.IsCameraFrameShow)
			{
				return;
			}
			if (hri.hurtInfo.hurtType != HurtTypeDef.Therapic)
			{
				Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
				if (hri.src && hostPlayer != null && hostPlayer.Captain && hostPlayer.Captain == hri.src)
				{
					this.MMiniMapCameraFrame_3Dui.ShowRed();
				}
			}
		}

		public void Update()
		{
			if (this.MMiniMapCameraFrame_3Dui != null)
			{
				this.MMiniMapCameraFrame_3Dui.Update();
			}
			if (this.MMinimapSkillIndicator_3Dui != null)
			{
				this.MMinimapSkillIndicator_3Dui.Update();
			}
			if (this.miniMapEffectModule != null)
			{
				this.miniMapEffectModule.Update(0f);
			}
			if (FogOfWar.enable && !Singleton<WatchController>.instance.IsWatching)
			{
				if (this.m_elementIndex >= 0 && this.m_elementIndex < this.m_mapElements.Count)
				{
					MinimapSys.ElementInMap elementInMap = this.m_mapElements[this.m_elementIndex];
					VInt3 worldLoc = new VInt3(elementInMap.pos.x, elementInMap.pos.z, 0);
					if (Singleton<GameFowManager>.instance.IsVisible(worldLoc, this.m_playerCamp))
					{
						if (elementInMap.smallElement != null)
						{
							MiniMapSysUT.RecycleMapGameObject(elementInMap.smallElement);
							elementInMap.smallElement = null;
						}
						if (elementInMap.bigElement != null)
						{
							MiniMapSysUT.RecycleMapGameObject(elementInMap.bigElement);
							elementInMap.bigElement = null;
						}
						this.m_mapElements.RemoveAt(this.m_elementIndex);
					}
					this.m_elementIndex++;
				}
				else
				{
					this.m_elementIndex = 0;
				}
			}
		}

		private void OnDragMiniMap(CUIEvent uiEvent)
		{
			CUIFormScript srcFormScript = uiEvent.m_srcFormScript;
			if (uiEvent == null || srcFormScript == null || uiEvent.m_pointerEventData == null || uiEvent.m_srcWidget == null)
			{
				return;
			}
			if (MonoSingleton<CameraSystem>.GetInstance().enableLockedCamera)
			{
				MonoSingleton<CameraSystem>.GetInstance().ToggleFreeDragCamera(true);
				if (Singleton<CBattleSystem>.GetInstance().WatchForm != null)
				{
					Singleton<CBattleSystem>.GetInstance().WatchForm.FreeFocus();
				}
			}
			Vector2 position = uiEvent.m_pointerEventData.position;
			MinimapSys theMinimapSys = Singleton<CBattleSystem>.GetInstance().TheMinimapSys;
			if (theMinimapSys == null)
			{
				return;
			}
			Vector2 mMFianlScreenPos = theMinimapSys.GetMMFianlScreenPos();
			float num = position.x - mMFianlScreenPos.x;
			float num2 = position.y - mMFianlScreenPos.y;
			num = uiEvent.m_srcFormScript.ChangeScreenValueToForm(num);
			num2 = uiEvent.m_srcFormScript.ChangeScreenValueToForm(num2);
			float x = num * Singleton<CBattleSystem>.instance.UI_world_Factor_Small.x;
			float z = num2 * Singleton<CBattleSystem>.instance.UI_world_Factor_Small.y;
			if (MonoSingleton<CameraSystem>.instance.MobaCamera != null)
			{
				MonoSingleton<CameraSystem>.instance.MobaCamera.SetAbsoluteLockLocation(new Vector3(x, 1f, z));
			}
			if (this.MMiniMapCameraFrame_3Dui != null)
			{
				if (!this.MMiniMapCameraFrame_3Dui.IsCameraFrameShow)
				{
					this.MMiniMapCameraFrame_3Dui.Show();
					this.MMiniMapCameraFrame_3Dui.ShowNormal();
				}
				this.MMiniMapCameraFrame_3Dui.SetPos(num, num2);
			}
		}

		private void OnDragMiniMapEnd(CUIEvent uievent)
		{
			Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
			if (!Singleton<WatchController>.GetInstance().IsWatching && hostPlayer != null && hostPlayer.Captain && hostPlayer.Captain.handle.ActorControl != null && !hostPlayer.Captain.handle.ActorControl.IsDeadState)
			{
				MonoSingleton<CameraSystem>.instance.ToggleFreeDragCamera(false);
				if (this.MMiniMapCameraFrame_3Dui != null)
				{
					this.MMiniMapCameraFrame_3Dui.Hide();
				}
			}
		}

		private void OnBigMap_Click_5_Dalong(CUIEvent uievent)
		{
			this.send_signal(uievent, this.bmRoot, MinimapSys.ElementType.Dragon_5_big, 0);
		}

		private void OnBigMap_Click_5_Xiaolong(CUIEvent uievent)
		{
			this.send_signal(uievent, this.bmRoot, MinimapSys.ElementType.Dragon_5_small, 0);
		}

		private void OnBigMap_Click_3_long(CUIEvent uievent)
		{
			this.send_signal(uievent, this.bmRoot, MinimapSys.ElementType.Dragon_3, 0);
		}

		private void OnBigMap_Click_Organ(CUIEvent uievent)
		{
			this.send_signal(uievent, this.bmRoot, MinimapSys.ElementType.None, 0);
		}

		private void OnBigMap_Click_Hero(CUIEvent uievent)
		{
			this.send_signal(uievent, this.bmRoot, MinimapSys.ElementType.None, 0);
		}

		private void OnBigMap_Click_Map(CUIEvent uievent)
		{
			if (this.curMapType == MinimapSys.EMapType.Skill)
			{
				if (!this.UI3DEventMgr.HandleSkillClickEvent(uievent.m_pointerEventData))
				{
					this.send_signal(uievent, this.bmRoot, MinimapSys.ElementType.None, 1);
				}
			}
			else if (!this.UI3DEventMgr.HandleClickEvent(uievent.m_pointerEventData))
			{
				this.send_signal(uievent, this.bmRoot, MinimapSys.ElementType.None, 1);
			}
		}

		private void OnBigMap_Click_Eye(CUIEvent uievent)
		{
			this.send_signal(uievent, this.bmRoot, MinimapSys.ElementType.None, 0);
		}

		private void OnBigMap_Click_Solider(CUIEvent uievent)
		{
			this.send_signal(uievent, this.bmRoot, MinimapSys.ElementType.None, 0);
		}

		private void OnBigMap_Hold_EmptyArea(CUIEvent uievent)
		{
			if (this.miniMapSignalPanel != null)
			{
				this.miniMapSignalPanel.Show(uievent);
			}
		}

		private void OnBigMap_HoldEnd(CUIEvent uievent)
		{
			if (this.miniMapSignalPanel != null)
			{
				this.miniMapSignalPanel.OnHoldUp(uievent);
				this.miniMapSignalPanel.Hide();
			}
		}

		private void OnBigMap_SignalPanel_Drag(CUIEvent uievent)
		{
			if (this.miniMapSignalPanel != null && this.miniMapSignalPanel.IsSignalPanelModel())
			{
				this.miniMapSignalPanel.OnDrag(uievent);
			}
		}

		private void send_signal(CUIEvent uiEvent, GameObject img, MinimapSys.ElementType elementType = MinimapSys.ElementType.None, int signal_id = 0)
		{
			if (uiEvent == null || img == null)
			{
				return;
			}
			byte b = (byte)uiEvent.m_eventParams.tag2;
			uint tagUInt = uiEvent.m_eventParams.tagUInt;
			if (signal_id == 0)
			{
				signal_id = uiEvent.m_eventParams.tag3;
			}
			MinimapSys.EMapType eMapType = this.curMapType;
			SkillSlotType curSelectedSlotType = this.m_CurSelectedSlotType;
			if (eMapType != MinimapSys.EMapType.Skill)
			{
				this.Switch(MinimapSys.EMapType.Mini);
			}
			SignalPanel signalPanel = (Singleton<CBattleSystem>.GetInstance().FightForm == null) ? null : Singleton<CBattleSystem>.instance.FightForm.GetSignalPanel();
			if (signalPanel != null)
			{
				if (eMapType == MinimapSys.EMapType.Skill)
				{
					Singleton<CBattleSystem>.GetInstance().FightForm.m_skillButtonManager.SelectedMapTarget(curSelectedSlotType, tagUInt);
					if (tagUInt != 0u)
					{
						Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
						if (hostPlayer != null && hostPlayer.Captain)
						{
							SkillSlot skillSlot = hostPlayer.Captain.handle.SkillControl.GetSkillSlot(curSelectedSlotType);
							if (skillSlot != null)
							{
								PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(tagUInt);
								if (actor && skillSlot.IsValidSkillTarget(ref actor))
								{
									this.m_CurSelectedSlotType = SkillSlotType.SLOT_SKILL_COUNT;
									this.Switch(MinimapSys.EMapType.Mini);
								}
							}
						}
					}
					return;
				}
				if (b == 3 || b == 1 || b == 2)
				{
					signalPanel.SendCommand_SignalMiniMap_Target((byte)signal_id, b, tagUInt);
					return;
				}
				if (elementType == MinimapSys.ElementType.Dragon_3 || elementType == MinimapSys.ElementType.Dragon_5_big || elementType == MinimapSys.ElementType.Dragon_5_small)
				{
					MinimapSys.Send_Position_Signal(uiEvent, img, 2, elementType, true);
					return;
				}
				MinimapSys.Send_Position_Signal(uiEvent, img, (byte)signal_id, MinimapSys.ElementType.None, true);
			}
		}

		public static void Send_Position_Signal(CUIEvent uiEvent, GameObject img, byte signal_id, MinimapSys.ElementType type, bool bBigMap = true)
		{
			if (Singleton<CBattleSystem>.GetInstance().FightForm == null)
			{
				return;
			}
			SignalPanel signalPanel = Singleton<CBattleSystem>.instance.FightForm.GetSignalPanel();
			if (signalPanel == null)
			{
				return;
			}
			MinimapSys theMinimapSys = Singleton<CBattleSystem>.GetInstance().TheMinimapSys;
			if (theMinimapSys == null)
			{
				return;
			}
			Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
			if (hostPlayer == null || !hostPlayer.Captain)
			{
				return;
			}
			ActorRoot handle = hostPlayer.Captain.handle;
			Vector2 vector = (!bBigMap) ? theMinimapSys.GetMMFianlScreenPos() : theMinimapSys.GetBMFianlScreenPos();
			float num = uiEvent.m_pointerEventData.position.x - vector.x;
			float num2 = uiEvent.m_pointerEventData.position.y - vector.y;
			num = uiEvent.m_srcFormScript.ChangeScreenValueToForm(num);
			num2 = uiEvent.m_srcFormScript.ChangeScreenValueToForm(num2);
			VInt3 zero = VInt3.zero;
			zero.x = (int)(num * Singleton<CBattleSystem>.GetInstance().UI_world_Factor_Big.x);
			zero.y = (int)((Vector3)handle.location).y;
			zero.z = (int)(num2 * Singleton<CBattleSystem>.GetInstance().UI_world_Factor_Big.y);
			signalPanel.SendCommand_SignalMiniMap_Position(signal_id, zero, type);
		}

		public static void Send_Position_Signal(float x, float y, CUIFormScript srcFormScript, byte signal_id, MinimapSys.ElementType type, bool bBigMap = true)
		{
			if (srcFormScript == null || Singleton<CBattleSystem>.GetInstance().FightForm == null)
			{
				return;
			}
			SignalPanel signalPanel = Singleton<CBattleSystem>.instance.FightForm.GetSignalPanel();
			if (signalPanel == null)
			{
				return;
			}
			MinimapSys theMinimapSys = Singleton<CBattleSystem>.GetInstance().TheMinimapSys;
			if (theMinimapSys == null)
			{
				return;
			}
			Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
			if (hostPlayer == null || !hostPlayer.Captain)
			{
				return;
			}
			ActorRoot handle = hostPlayer.Captain.handle;
			Vector2 vector = (!bBigMap) ? theMinimapSys.GetMMFianlScreenPos() : theMinimapSys.GetBMFianlScreenPos();
			float num = x - vector.x;
			float num2 = y - vector.y;
			num = srcFormScript.ChangeScreenValueToForm(num);
			num2 = srcFormScript.ChangeScreenValueToForm(num2);
			VInt3 zero = VInt3.zero;
			zero.x = (int)(num * Singleton<CBattleSystem>.GetInstance().UI_world_Factor_Big.x);
			zero.y = (int)((Vector3)handle.location).y;
			zero.z = (int)(num2 * Singleton<CBattleSystem>.GetInstance().UI_world_Factor_Big.y);
			signalPanel.SendCommand_SignalMiniMap_Position(signal_id, zero, type);
			int num3 = 201;
			int num4 = 203;
			if ((int)signal_id >= num3 && (int)signal_id <= num4)
			{
				CPlayerBehaviorStat.BehaviorType type2 = (int)signal_id - num3 + CPlayerBehaviorStat.BehaviorType.Battle_SignalPanel1;
				CPlayerBehaviorStat.Plus(type2);
			}
		}

		public static void ShowBack2City(PoolObjHandle<ActorRoot> actorHandle)
		{
			MinimapSys theMinimapSys = Singleton<CBattleSystem>.GetInstance().TheMinimapSys;
			if (theMinimapSys != null && theMinimapSys.MMiniMapBackCityCom_3Dui != null)
			{
				theMinimapSys.MMiniMapBackCityCom_3Dui.ShowBack2City_Imp(actorHandle);
			}
		}

		private void InitAllTransformWithBigMapMask()
		{
			FightForm fightForm = Singleton<CBattleSystem>.instance.FightForm;
			if (fightForm == null)
			{
				return;
			}
			this.m_AllTransformWithBigMapMask.Clear();
			this.m_AllTransformWithBigMapMask.Add(fightForm._formScript.gameObject.transform.Find("Panel_Equip") as RectTransform);
			this.m_AllTransformWithBigMapMask.Add(fightForm._formScript.gameObject.transform.Find("PanelHeroInfo") as RectTransform);
			this.m_AllTransformWithBigMapMask.Add(fightForm._formScript.gameObject.transform.Find("PVPTopRightPanel/PanelBtn") as RectTransform);
			this.m_AllTransformWithBigMapMask.Add(fightForm._formScript.gameObject.transform.Find("PVPTopRightPanel/FPSAndLag") as RectTransform);
			this.m_AllTransformWithBigMapMask.Add(fightForm._formScript.gameObject.transform.Find("PVPTopRightPanel/ButtonViewSkillInfo") as RectTransform);
			this.m_AllTransformWithBigMapMask.Add(fightForm._formScript.gameObject.transform.Find("PVPTopRightPanel/WifiIcon") as RectTransform);
			this.m_AllTransformWithBigMapMask.Add(fightForm._formScript.gameObject.transform.Find("PVPTopRightPanel/ScoreBoard") as RectTransform);
			this.m_AllTransformWithBigMapMask.Add(fightForm._formScript.gameObject.transform.Find("PVPTopRightPanel/ScoreBoardTime") as RectTransform);
			this.m_AllTransformWithBigMapMask.Add(fightForm._formScript.gameObject.transform.Find("PVPTopRightPanel/panelTopRight/ResumeBtn") as RectTransform);
			this.m_AllTransformWithBigMapMask.Add(fightForm._formScript.gameObject.transform.Find("PVPTopRightPanel/panelTopRight/PauseBtn") as RectTransform);
			this.m_AllTransformWithBigMapMask.Add(fightForm._formScript.gameObject.transform.Find("PVPTopRightPanel/panelTopRight/ReplayKit") as RectTransform);
			this.m_AllTransformWithBigMapMask.Add(fightForm._formScript.gameObject.transform.Find("PVPTopRightPanel/panelTopRight/SignalPanel/Button_Signal_2") as RectTransform);
			this.m_AllTransformWithBigMapMask.Add(fightForm._formScript.gameObject.transform.Find("PVPTopRightPanel/panelTopRight/SignalPanel/Button_Signal_3") as RectTransform);
			this.m_AllTransformWithBigMapMask.Add(fightForm._formScript.gameObject.transform.Find("PVPTopRightPanel/panelTopRight/SignalPanel/Button_Signal_4") as RectTransform);
			this.m_AllTransformWithBigMapMask.Add(fightForm._formScript.gameObject.transform.Find("PVPTopRightPanel/panelTopRight/SignalPanel/Button_Chat") as RectTransform);
			this.m_AllTransformWithBigMapMask.Add(fightForm._formScript.gameObject.transform.Find("Panel_Prop/ButtonOpen") as RectTransform);
			this.m_AllTransformWithBigMapMask.Add(fightForm._formScript.gameObject.transform.Find("Panel_Prop/Panel_BaseProp") as RectTransform);
			this.AddSkillButtonToBigMapMask(SkillSlotType.SLOT_SKILL_2);
			this.AddSkillButtonToBigMapMask(SkillSlotType.SLOT_SKILL_3);
			this.AddSkillButtonToBigMapMask(SkillSlotType.SLOT_SKILL_9);
			CUIFormScript skillBtnFormScript = fightForm.GetSkillBtnFormScript();
			if (skillBtnFormScript != null)
			{
				this.m_AllTransformWithBigMapMask.Add(skillBtnFormScript.GetWidget(33).transform as RectTransform);
			}
		}

		private void AddSkillButtonToBigMapMask(SkillSlotType skillSlotType)
		{
			FightForm fightForm = Singleton<CBattleSystem>.instance.FightForm;
			if (fightForm == null)
			{
				return;
			}
			SkillButton button = fightForm.GetButton(skillSlotType);
			if (button != null)
			{
				if (button.m_button != null)
				{
					this.m_AllTransformWithBigMapMask.Add(button.m_button.transform as RectTransform);
				}
				if (button.m_beanText != null)
				{
					this.m_AllTransformWithBigMapMask.Add(button.m_beanText.transform as RectTransform);
				}
				if (button.m_cdText != null)
				{
					this.m_AllTransformWithBigMapMask.Add(button.m_cdText.transform as RectTransform);
				}
			}
		}

		private void HideTransformWithBigMapMask()
		{
			FightForm fightForm = Singleton<CBattleSystem>.instance.FightForm;
			if (fightForm == null)
			{
				return;
			}
			RectTransform component = this.bmUGUIRoot.GetComponent<RectTransform>();
			Vector3 vector = CUIUtility.WorldToScreenPoint(Singleton<CUIManager>.instance.FormCamera, component.transform.position);
			Rect targetRect = default(Rect);
			targetRect.x = vector.x;
			targetRect.y = vector.y;
			targetRect.width = fightForm._formScript.ChangeFormValueToScreen(component.rect.width);
			targetRect.height = fightForm._formScript.ChangeFormValueToScreen(component.rect.height);
			targetRect.x -= targetRect.width / 2f;
			targetRect.y -= targetRect.height / 2f;
			Vector3 vector2 = Vector3.zero;
			this.m_AllTransformWithBigMapShow.Clear();
			for (int i = 0; i < this.m_AllTransformWithBigMapMask.Count; i++)
			{
				RectTransform rectTransform = this.m_AllTransformWithBigMapMask[i];
				if (!(rectTransform == null) && !(component == null))
				{
					vector2 = CUIUtility.WorldToScreenPoint(Singleton<CUIManager>.instance.FormCamera, rectTransform.transform.position);
					if (rectTransform.gameObject.activeInHierarchy && this.IsPointInRect(vector2.x, vector2.y, targetRect))
					{
						rectTransform.gameObject.CustomSetActive(false);
						this.m_AllTransformWithBigMapShow.Add(rectTransform);
					}
				}
			}
		}

		private bool IsPointInRect(float x, float y, Rect targetRect)
		{
			return x >= targetRect.x && x <= targetRect.x + targetRect.width && y >= targetRect.y && y <= targetRect.y + targetRect.height;
		}

		private void ShowTransformWithBigMapMask()
		{
			if (Singleton<CBattleSystem>.instance.FightForm == null)
			{
				return;
			}
			for (int i = 0; i < this.m_AllTransformWithBigMapShow.Count; i++)
			{
				RectTransform rectTransform = this.m_AllTransformWithBigMapShow[i];
				rectTransform.gameObject.CustomSetActive(true);
			}
		}
	}
}
