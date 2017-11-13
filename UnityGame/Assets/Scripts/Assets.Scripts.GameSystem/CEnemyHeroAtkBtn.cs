using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.UI;
using ResData;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class CEnemyHeroAtkBtn
	{
		private enum ENM_ENEMY_HERO_STATE
		{
			ENM_ENEMY_HERO_STATE_SHOW,
			ENM_ENEMY_HERO_STATE_NOTVISIBLE,
			ENM_ENEMY_HERO_STATE_NOTALIVE,
			ENM_ENEMY_HERO_STATE_TOOFAR,
			ENM_ENEMY_HERO_STATE_HOSTDEAD
		}

		private struct BTN_INFO
		{
			public Transform btnTransform;

			public PoolObjHandle<ActorRoot> actorPtr;

			public int heroState;

			public bool bIsUseBlueHpImage;

			public Vector3 btnScreenPos;

			public Vector3 btnUI3DWorldPos;

			public GameObject objUI3DHp;

			public Sprite3D objHp;

			public BTN_INFO(Transform _btnTransform, PoolObjHandle<ActorRoot> _actorPtr, CEnemyHeroAtkBtn.ENM_ENEMY_HERO_STATE _heroState, bool _bIsUseBlueHpImage, GameObject _objUI3DHp, Sprite3D _objHp)
			{
				this.btnTransform = _btnTransform;
				this.actorPtr = _actorPtr;
				this.heroState = 1 << (int)_heroState;
				this.bIsUseBlueHpImage = _bIsUseBlueHpImage;
				this.btnScreenPos = CUIUtility.WorldToScreenPoint(Singleton<CUIManager>.instance.FormCamera, _btnTransform.position);
				this.btnScreenPos.z = (float)MiniMapSysUT.UI3D_Depth;
				this.btnUI3DWorldPos = CEnemyHeroAtkBtn.m_UI3DCamera.ScreenToWorldPoint(this.btnScreenPos);
				this.objUI3DHp = _objUI3DHp;
				this.objUI3DHp.transform.position = this.btnUI3DWorldPos;
				this.objHp = _objHp;
			}
		}

		private const int m_iEnemyPlayerCountMax = 5;

		private const float m_fDrawLinkerMissBtnIndexFrameTimeMax = 500f;

		public const string m_strEnemyHeroAttackEffectHomePath = "Prefab_Skill_Effects/Common_Effects/";

		public const string m_strEnemyHeroAttackPrefabName = "EnemyHeroAttack";

		private static string[] m_arrHeroBtnNames = new string[]
		{
			"EnemyHeroBtn_0",
			"EnemyHeroBtn_1",
			"EnemyHeroBtn_2",
			"EnemyHeroBtn_3",
			"EnemyHeroBtn_4",
			"EnemyHeroBtn_5"
		};

		private int m_iCurEnemyPlayerCount;

		private CEnemyHeroAtkBtn.BTN_INFO[] m_arrBtnInfo = new CEnemyHeroAtkBtn.BTN_INFO[5];

		private GameObject m_objPanelEnemyHeroAtk;

		private PoolObjHandle<ActorRoot> m_hostActor;

		private int m_iEnemyDistanceMax = 14400;

		private bool m_bIsMobaMode;

		private int m_iCurTargetEnemyBtnIndex = -1;

		private int m_iLastTargetEnemyBtnIndex = -1;

		private int m_iLastSkillTargetEnemyBtnIndex = -1;

		private bool m_bLastSkillUsed = true;

		private SkillSlotType m_ShowGuidPrefabSkillSlot = SkillSlotType.SLOT_SKILL_VALID;

		private int m_iCurDrawLinkerBtnIndex = -1;

		private int m_iLastDrawLinkerBtnIndex = -1;

		private float m_fDrawLinkerMissBtnIndexFrameTime = -1f;

		private LineRenderer m_attackLinker;

		private GameObject m_objEnemyHeroDirectionPrefab;

		private GameObject m_objLinker;

		private static Camera m_UI3DCamera = null;

		private GameObject m_ui3dRes;

		private GameObject m_objSelectedImg;

		private GameObject m_objDirection;

		public static void Preload(ref ActorPreloadTab preloadTab)
		{
			preloadTab.AddParticle("Prefab_Skill_Effects/Common_Effects/EnemyHeroAttack.prefab");
		}

		public void Init(GameObject objPanelEnemyHeroAtk)
		{
			SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
			if (curLvelContext != null && curLvelContext.IsMobaMode())
			{
				this.m_bIsMobaMode = true;
				Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_EnemyHeroAtkBtn_Down, new CUIEventManager.OnUIEventHandler(this.OnEnemyHeroAtkBtnDown));
				Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_EnemyHeroAtkBtn_Up, new CUIEventManager.OnUIEventHandler(this.OnEnemyHeroAtkBtnUp));
				Singleton<EventRouter>.GetInstance().AddEventHandler<uint, bool>("ActorVisibleToHostPlayerChnage", new Action<uint, bool>(this.OnActorVisibleToHostPlayerChance));
				Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, int, int>("HeroHpChange", new Action<PoolObjHandle<ActorRoot>, int, int>(this.OnHeroHpChange));
				Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorRevive, new RefAction<DefaultGameEventParam>(this.OnActorRevive));
				Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_FightStart, new RefAction<DefaultGameEventParam>(this.OnFightStart));
				Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_CaptainSwitch, new RefAction<DefaultGameEventParam>(this.OnCaptainSwitched));
				this.m_objPanelEnemyHeroAtk = objPanelEnemyHeroAtk;
				this.m_hostActor = Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain;
				this.m_iEnemyDistanceMax = (int)GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_ENEMYATKBTN_ENEMYDISTANCEMAX);
				int globeValue = (int)GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_HORIZON_RADIUS);
				this.m_iEnemyDistanceMax = ((this.m_iEnemyDistanceMax < globeValue) ? this.m_iEnemyDistanceMax : globeValue);
				this.m_iCurTargetEnemyBtnIndex = -1;
				this.m_iLastTargetEnemyBtnIndex = -1;
				this.m_iLastSkillTargetEnemyBtnIndex = -1;
				this.m_bLastSkillUsed = true;
			}
		}

		public void UnInit()
		{
			if (this.m_bIsMobaMode)
			{
				Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_EnemyHeroAtkBtn_Down, new CUIEventManager.OnUIEventHandler(this.OnEnemyHeroAtkBtnDown));
				Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_EnemyHeroAtkBtn_Up, new CUIEventManager.OnUIEventHandler(this.OnEnemyHeroAtkBtnUp));
				Singleton<EventRouter>.GetInstance().RemoveEventHandler<uint, bool>("ActorVisibleToHostPlayerChnage", new Action<uint, bool>(this.OnActorVisibleToHostPlayerChance));
				Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, int, int>("HeroHpChange", new Action<PoolObjHandle<ActorRoot>, int, int>(this.OnHeroHpChange));
				Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorRevive, new RefAction<DefaultGameEventParam>(this.OnActorRevive));
				Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_FightStart, new RefAction<DefaultGameEventParam>(this.OnFightStart));
				Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_CaptainSwitch, new RefAction<DefaultGameEventParam>(this.OnCaptainSwitched));
				this.m_objPanelEnemyHeroAtk = null;
				if (this.m_hostActor)
				{
					this.m_hostActor.Release();
				}
				this.m_objLinker = null;
				this.m_objSelectedImg = null;
				this.m_attackLinker = null;
				this.m_objDirection = null;
				this.m_arrBtnInfo = null;
				if (this.m_ui3dRes)
				{
					Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(this.m_ui3dRes);
				}
			}
		}

		private void InitBtnInfo()
		{
			List<PoolObjHandle<ActorRoot>> list = ActorHelper.FilterActors(Singleton<GameObjMgr>.instance.HeroActors, new ActorFilterDelegate(Singleton<BattleLogic>.instance.FilterEnemyActor));
			this.m_iCurEnemyPlayerCount = list.get_Count();
			if (this.m_iCurEnemyPlayerCount > 5)
			{
				this.m_iCurEnemyPlayerCount = 5;
			}
			CEnemyHeroAtkBtn.m_UI3DCamera = Singleton<Camera_UI3D>.GetInstance().GetCurrentCamera();
			this.m_ui3dRes = Singleton<CGameObjectPool>.GetInstance().GetGameObject("Prefab_Skill_Effects/Common_Effects/EnemyHeroAttack", enResourceType.BattleScene);
			float num = 1f;
			if (Singleton<CBattleSystem>.instance.FightFormScript != null)
			{
				num = Singleton<CBattleSystem>.instance.FightFormScript.GetScreenScaleValue();
			}
			if (this.m_ui3dRes != null)
			{
				this.m_ui3dRes.transform.SetParent(CEnemyHeroAtkBtn.m_UI3DCamera.transform, true);
				if (this.m_objSelectedImg == null)
				{
					this.m_objSelectedImg = this.m_ui3dRes.transform.FindChild("selected").gameObject;
					if (this.m_objSelectedImg)
					{
						Sprite3D component = this.m_objSelectedImg.GetComponent<Sprite3D>();
						if (component)
						{
							component.width *= num;
							component.height *= num;
						}
						this.m_objDirection = this.m_objSelectedImg.transform.FindChild("direction").gameObject;
						if (this.m_objDirection)
						{
							Vector3 position = this.m_objDirection.transform.position;
							position.y *= num;
							this.m_objDirection.transform.position = position;
							component = this.m_objDirection.GetComponent<Sprite3D>();
							if (component)
							{
								component.width *= num;
								component.height *= num;
							}
						}
					}
				}
				if (this.m_attackLinker == null)
				{
					this.m_objLinker = this.m_ui3dRes.transform.FindChild("linker").gameObject;
					if (this.m_objLinker)
					{
						this.m_attackLinker = this.m_objLinker.GetComponent<LineRenderer>();
						if (this.m_attackLinker != null && CEnemyHeroAtkBtn.m_UI3DCamera)
						{
							this.m_attackLinker.SetVertexCount(2);
							this.m_attackLinker.useWorldSpace = true;
						}
					}
				}
			}
			for (int i = 0; i < this.m_iCurEnemyPlayerCount; i++)
			{
				Transform transform = this.m_objPanelEnemyHeroAtk.transform.FindChild(CEnemyHeroAtkBtn.m_arrHeroBtnNames[i]);
				string heroSkinPic = CSkinInfo.GetHeroSkinPic((uint)list.get_Item(i).handle.TheActorMeta.ConfigId, 0u);
				string prefabPath = CUIUtility.s_Sprite_Dynamic_BustCircle_Dir + heroSkinPic;
				Image component2 = transform.GetComponent<Image>();
				if (component2)
				{
					component2.SetSprite(prefabPath, Singleton<CBattleSystem>.GetInstance().FormScript, true, false, false, false);
				}
				if (this.m_ui3dRes != null)
				{
					GameObject gameObject = this.m_ui3dRes.transform.FindChild("hp_" + i).gameObject;
					if (gameObject)
					{
						Sprite3D component3 = gameObject.GetComponent<Sprite3D>();
						if (component3)
						{
							component3.width *= num;
							component3.height *= num;
						}
						GameObject gameObject2 = gameObject.transform.FindChild("hp").gameObject;
						if (gameObject2)
						{
							Sprite3D component4 = gameObject2.GetComponent<Sprite3D>();
							component4.width *= num;
							component4.height *= num;
							this.m_arrBtnInfo[i] = new CEnemyHeroAtkBtn.BTN_INFO(transform, list.get_Item(i), CEnemyHeroAtkBtn.ENM_ENEMY_HERO_STATE.ENM_ENEMY_HERO_STATE_TOOFAR, true, gameObject, component4);
						}
					}
				}
			}
		}

		private void OnFightStart(ref DefaultGameEventParam prm)
		{
			this.InitBtnInfo();
		}

		private int GetBtnIndexByActor(PoolObjHandle<ActorRoot> actorPtr)
		{
			for (int i = 0; i < this.m_iCurEnemyPlayerCount; i++)
			{
				if (actorPtr == this.m_arrBtnInfo[i].actorPtr)
				{
					return i;
				}
			}
			return -1;
		}

		private void SetBtnStateByBtnInfo(int iBtnIndex)
		{
			if (iBtnIndex >= 0 && iBtnIndex < this.m_iCurEnemyPlayerCount && this.m_arrBtnInfo != null && this.m_arrBtnInfo[iBtnIndex].btnTransform != null)
			{
				if (this.m_arrBtnInfo[iBtnIndex].heroState != 0)
				{
					if (this.m_arrBtnInfo[iBtnIndex].btnTransform.gameObject.activeSelf)
					{
						this.m_arrBtnInfo[iBtnIndex].btnTransform.gameObject.CustomSetActive(false);
						if (this.m_arrBtnInfo[iBtnIndex].objUI3DHp)
						{
							this.m_arrBtnInfo[iBtnIndex].objUI3DHp.gameObject.CustomSetActive(false);
						}
						this.SetEnemyHeroBtnHighlight(iBtnIndex, false);
						if (iBtnIndex == this.m_iCurTargetEnemyBtnIndex)
						{
							this.HandleOnEnemyHeroAtkBtnUp();
							this.m_iCurTargetEnemyBtnIndex = -1;
						}
						else if (this.m_hostActor && this.m_hostActor.handle.ActorAgent != null && this.m_hostActor.handle.ActorAgent.m_wrapper.myBehavior == ObjBehaviMode.Normal_Attack && this.m_hostActor.handle.ActorAgent.m_wrapper.myTarget && this.m_arrBtnInfo[iBtnIndex].actorPtr && this.m_hostActor.handle.ActorAgent.m_wrapper.myTarget.handle.ObjID == this.m_arrBtnInfo[iBtnIndex].actorPtr.handle.ObjID && Singleton<CBattleSystem>.GetInstance().FightForm != null)
						{
							CSkillButtonManager skillButtonManager = Singleton<CBattleSystem>.GetInstance().FightForm.m_skillButtonManager;
							if (skillButtonManager != null)
							{
								skillButtonManager.SendUseCommonAttack(1, 0u);
								skillButtonManager.SendUseCommonAttack(0, 0u);
							}
						}
					}
				}
				else if (!this.m_arrBtnInfo[iBtnIndex].btnTransform.gameObject.activeSelf && Singleton<CBattleSystem>.instance.TheMinimapSys != null && Singleton<CBattleSystem>.instance.TheMinimapSys.CurMapType() != MinimapSys.EMapType.Big && Singleton<CBattleSystem>.instance.TheMinimapSys.CurMapType() != MinimapSys.EMapType.Skill)
				{
					this.SetBtnHpFillAmount(iBtnIndex);
					this.SetEnemyHeroBtnAlpha(iBtnIndex, 0.6f);
					this.m_arrBtnInfo[iBtnIndex].btnTransform.gameObject.CustomSetActive(true);
					if (this.m_arrBtnInfo[iBtnIndex].objUI3DHp)
					{
						this.m_arrBtnInfo[iBtnIndex].objUI3DHp.gameObject.CustomSetActive(true);
					}
					CUICommonSystem.PlayAnimation(this.m_arrBtnInfo[iBtnIndex].btnTransform, enSkillButtonAnimationName.CD_End.ToString());
				}
			}
		}

		private void SetBtnStateByBtnInfo(int iBtnIndex, bool bIsShow)
		{
			if (iBtnIndex >= 0 && iBtnIndex < this.m_iCurEnemyPlayerCount && this.m_arrBtnInfo[iBtnIndex].btnTransform != null)
			{
				this.m_arrBtnInfo[iBtnIndex].btnTransform.gameObject.CustomSetActive(bIsShow);
				if (this.m_arrBtnInfo[iBtnIndex].objUI3DHp)
				{
					this.m_arrBtnInfo[iBtnIndex].objUI3DHp.gameObject.CustomSetActive(bIsShow);
				}
				if (bIsShow)
				{
					this.SetBtnHpFillAmount(iBtnIndex);
					CUICommonSystem.PlayAnimation(this.m_arrBtnInfo[iBtnIndex].btnTransform, enSkillButtonAnimationName.CD_End.ToString());
				}
				else if (this.m_iCurDrawLinkerBtnIndex == iBtnIndex)
				{
					this.HandleOnEnemyHeroAtkBtnUp();
					this.m_iCurTargetEnemyBtnIndex = -1;
					this.m_iLastTargetEnemyBtnIndex = -1;
					if (this.m_attackLinker)
					{
						this.m_attackLinker.gameObject.CustomSetActive(false);
					}
					if (this.m_objSelectedImg)
					{
						this.m_objSelectedImg.gameObject.CustomSetActive(false);
					}
				}
			}
		}

		private void SetBtnHpFillAmount(int iBtnIndex)
		{
			if (iBtnIndex >= 0 && iBtnIndex < this.m_iCurEnemyPlayerCount && this.m_arrBtnInfo[iBtnIndex].btnTransform != null && this.m_arrBtnInfo[iBtnIndex].actorPtr)
			{
				float num = (float)this.m_arrBtnInfo[iBtnIndex].actorPtr.handle.ValueComponent.actorHp / (float)this.m_arrBtnInfo[iBtnIndex].actorPtr.handle.ValueComponent.actorHpTotal;
				Transform btnTransform = this.m_arrBtnInfo[iBtnIndex].btnTransform;
				if (btnTransform)
				{
					Sprite3D objHp = this.m_arrBtnInfo[iBtnIndex].objHp;
					Image component = btnTransform.GetComponent<Image>();
					if (objHp && component)
					{
						if ((double)num < 0.3 && this.m_arrBtnInfo[iBtnIndex].bIsUseBlueHpImage)
						{
							objHp.spriteName = "Battle_HP_Red_Ring";
							component.set_color(CUIUtility.s_Color_EnemyHero_Button_PINK);
							this.m_arrBtnInfo[iBtnIndex].bIsUseBlueHpImage = false;
						}
						else if ((double)num >= 0.3 && !this.m_arrBtnInfo[iBtnIndex].bIsUseBlueHpImage)
						{
							objHp.spriteName = "Battle_HP_Blue_Ring";
							component.set_color(CUIUtility.s_Color_White);
							this.m_arrBtnInfo[iBtnIndex].bIsUseBlueHpImage = true;
						}
						objHp.fillAmount = num;
					}
				}
			}
		}

		private void UpdateHeroAtkBtnWithVisible(int iBtnIndex, bool bVisible)
		{
			if (iBtnIndex >= 0 && iBtnIndex < this.m_iCurEnemyPlayerCount)
			{
				if (bVisible)
				{
					CEnemyHeroAtkBtn.BTN_INFO[] arrBtnInfo = this.m_arrBtnInfo;
					arrBtnInfo[iBtnIndex].heroState = (arrBtnInfo[iBtnIndex].heroState & -3);
				}
				else
				{
					CEnemyHeroAtkBtn.BTN_INFO[] arrBtnInfo2 = this.m_arrBtnInfo;
					arrBtnInfo2[iBtnIndex].heroState = (arrBtnInfo2[iBtnIndex].heroState | 2);
				}
				this.SetBtnStateByBtnInfo(iBtnIndex);
			}
		}

		private void UpdateHeroAtkBtnWithVisible(PoolObjHandle<ActorRoot> hero, bool bVisible)
		{
			int btnIndexByActor = this.GetBtnIndexByActor(hero);
			this.UpdateHeroAtkBtnWithVisible(btnIndexByActor, bVisible);
		}

		private void OnActorVisibleToHostPlayerChance(uint uiObjId, bool bVisible)
		{
			if (!this.CheckShouldUpdate())
			{
				return;
			}
			PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.instance.GetActor(uiObjId);
			if (actor && actor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero && this.m_hostActor && this.m_hostActor.handle.TheActorMeta.ActorCamp != actor.handle.TheActorMeta.ActorCamp)
			{
				this.UpdateHeroAtkBtnWithVisible(actor, bVisible);
			}
		}

		private void UpdateHeroAtkBtnWithHostState(int iBtnIndex, bool bDead)
		{
			if (iBtnIndex >= 0 && iBtnIndex < this.m_iCurEnemyPlayerCount)
			{
				if (bDead)
				{
					CEnemyHeroAtkBtn.BTN_INFO[] arrBtnInfo = this.m_arrBtnInfo;
					arrBtnInfo[iBtnIndex].heroState = (arrBtnInfo[iBtnIndex].heroState | 16);
				}
				else
				{
					CEnemyHeroAtkBtn.BTN_INFO[] arrBtnInfo2 = this.m_arrBtnInfo;
					arrBtnInfo2[iBtnIndex].heroState = (arrBtnInfo2[iBtnIndex].heroState & -17);
				}
				this.SetBtnStateByBtnInfo(iBtnIndex);
			}
		}

		private void OnHeroDead(PoolObjHandle<ActorRoot> hero)
		{
			if (!this.CheckShouldUpdate())
			{
				return;
			}
			if (hero)
			{
				if (ActorHelper.IsHostEnemyActor(ref hero))
				{
					int btnIndexByActor = this.GetBtnIndexByActor(hero);
					if (btnIndexByActor >= 0 && btnIndexByActor < this.m_iCurEnemyPlayerCount)
					{
						CEnemyHeroAtkBtn.BTN_INFO[] arrBtnInfo = this.m_arrBtnInfo;
						int num = btnIndexByActor;
						arrBtnInfo[num].heroState = (arrBtnInfo[num].heroState | 4);
						this.SetBtnStateByBtnInfo(btnIndexByActor);
					}
				}
				else if (hero == this.m_hostActor)
				{
					for (int i = 0; i < this.m_iCurEnemyPlayerCount; i++)
					{
						this.UpdateHeroAtkBtnWithHostState(i, true);
					}
					this.m_iLastTargetEnemyBtnIndex = -1;
					this.m_iCurTargetEnemyBtnIndex = -1;
					this.m_iLastSkillTargetEnemyBtnIndex = -1;
				}
			}
		}

		public void OnActorRevive(ref DefaultGameEventParam prm)
		{
			if (!this.CheckShouldUpdate())
			{
				return;
			}
			if (prm.src && prm.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
			{
				if (prm.src.handle.TheActorMeta.ActorCamp != this.m_hostActor.handle.TheActorMeta.ActorCamp)
				{
					int btnIndexByActor = this.GetBtnIndexByActor(prm.src);
					if (btnIndexByActor >= 0 && btnIndexByActor < this.m_iCurEnemyPlayerCount)
					{
						CEnemyHeroAtkBtn.BTN_INFO[] arrBtnInfo = this.m_arrBtnInfo;
						int num = btnIndexByActor;
						arrBtnInfo[num].heroState = (arrBtnInfo[num].heroState & -5);
						this.UpdateHeroAtkBtnWithDistance(btnIndexByActor);
					}
				}
				else if (prm.src == this.m_hostActor)
				{
					for (int i = 0; i < this.m_iCurEnemyPlayerCount; i++)
					{
						this.UpdateHeroAtkBtnWithHostState(i, false);
						this.UpdateHeroAtkBtnWithDistance(i);
					}
				}
			}
		}

		private void UpdateHeroAtkBtnWithDistance(int iBtnIndex)
		{
			if (this.m_arrBtnInfo == null || !this.m_hostActor)
			{
				return;
			}
			if (iBtnIndex >= 0 && iBtnIndex < this.m_iCurEnemyPlayerCount)
			{
				if ((this.m_arrBtnInfo[iBtnIndex].heroState & 4) != 0 || (this.m_arrBtnInfo[iBtnIndex].heroState & 2) != 0 || !this.m_arrBtnInfo[iBtnIndex].actorPtr)
				{
					return;
				}
				int magnitude2D = (this.m_hostActor.handle.location - this.m_arrBtnInfo[iBtnIndex].actorPtr.handle.location).magnitude2D;
				if (magnitude2D < this.m_iEnemyDistanceMax)
				{
					CEnemyHeroAtkBtn.BTN_INFO[] arrBtnInfo = this.m_arrBtnInfo;
					arrBtnInfo[iBtnIndex].heroState = (arrBtnInfo[iBtnIndex].heroState & -9);
				}
				else
				{
					CEnemyHeroAtkBtn.BTN_INFO[] arrBtnInfo2 = this.m_arrBtnInfo;
					arrBtnInfo2[iBtnIndex].heroState = (arrBtnInfo2[iBtnIndex].heroState | 8);
					if (iBtnIndex == this.m_iLastTargetEnemyBtnIndex)
					{
						this.m_iLastTargetEnemyBtnIndex = -1;
					}
				}
				this.SetBtnStateByBtnInfo(iBtnIndex);
			}
		}

		private void UpdateHeroAtkBtnWithDistance(PoolObjHandle<ActorRoot> hero)
		{
			int btnIndexByActor = this.GetBtnIndexByActor(hero);
			this.UpdateHeroAtkBtnWithDistance(btnIndexByActor);
		}

		private void UpdateHeroAtkBtnDistance()
		{
			if (GameSettings.ShowEnemyHeroHeadBtnMode && this.m_hostActor)
			{
				List<PoolObjHandle<ActorRoot>> heroActors = Singleton<GameObjMgr>.instance.HeroActors;
				int count = heroActors.get_Count();
				for (int i = 0; i < count; i++)
				{
					PoolObjHandle<ActorRoot> hero = heroActors.get_Item(i);
					if (hero.handle.TheActorMeta.ActorCamp != this.m_hostActor.handle.TheActorMeta.ActorCamp)
					{
						this.UpdateHeroAtkBtnWithDistance(hero);
					}
				}
			}
		}

		private void OnHeroHpChange(PoolObjHandle<ActorRoot> hero, int iCurVal, int iMaxVal)
		{
			if (!this.CheckShouldUpdate())
			{
				return;
			}
			if (hero && ActorHelper.IsHostEnemyActor(ref hero))
			{
				int btnIndexByActor = this.GetBtnIndexByActor(hero);
				this.SetBtnHpFillAmount(btnIndexByActor);
			}
			if (iCurVal <= 0)
			{
				this.OnHeroDead(hero);
			}
		}

		private void OnEnemyHeroAtkBtnDown(CUIEvent uiEvent)
		{
			if (uiEvent.m_srcWidget != null)
			{
				int num = int.Parse(uiEvent.m_srcWidget.name.Substring(uiEvent.m_srcWidget.name.IndexOf("_") + 1));
				if (num >= 0 && num < this.m_iCurEnemyPlayerCount)
				{
					if (this.m_arrBtnInfo[num].actorPtr)
					{
						this.m_iCurTargetEnemyBtnIndex = num;
						uint objID = this.m_arrBtnInfo[num].actorPtr.handle.ObjID;
						if (Singleton<CBattleSystem>.GetInstance().FightForm != null)
						{
							CSkillButtonManager skillButtonManager = Singleton<CBattleSystem>.GetInstance().FightForm.m_skillButtonManager;
							if (skillButtonManager != null)
							{
								skillButtonManager.SendUseCommonAttack(1, objID);
							}
						}
					}
					if (this.m_iLastTargetEnemyBtnIndex >= 0 && this.m_iLastTargetEnemyBtnIndex < this.m_iCurEnemyPlayerCount)
					{
						this.SetEnemyHeroBtnHighlight(this.m_iLastTargetEnemyBtnIndex, false);
					}
					this.SetEnemyHeroBtnHighlight(this.m_iCurTargetEnemyBtnIndex, true);
					this.SetEnemyHeroBtnSize(this.m_iCurTargetEnemyBtnIndex, true);
				}
			}
		}

		private void HandleOnEnemyHeroAtkBtnUp()
		{
			if (Singleton<CBattleSystem>.GetInstance().FightForm != null)
			{
				CSkillButtonManager skillButtonManager = Singleton<CBattleSystem>.GetInstance().FightForm.m_skillButtonManager;
				if (skillButtonManager != null)
				{
					skillButtonManager.SendUseCommonAttack(0, 0u);
				}
				this.SetEnemyHeroBtnSize(this.m_iCurTargetEnemyBtnIndex, false);
				this.m_iLastTargetEnemyBtnIndex = this.m_iCurTargetEnemyBtnIndex;
				this.m_iCurTargetEnemyBtnIndex = -1;
			}
		}

		private void OnEnemyHeroAtkBtnUp(CUIEvent uiEvent)
		{
			this.HandleOnEnemyHeroAtkBtnUp();
		}

		private void SetEnemyHeroBtnAlpha(int iBtnIndex, float fAlpha)
		{
			if (iBtnIndex < 0 || iBtnIndex >= this.m_iCurEnemyPlayerCount)
			{
				return;
			}
			if (this.m_arrBtnInfo[iBtnIndex].btnTransform != null)
			{
				Transform btnTransform = this.m_arrBtnInfo[iBtnIndex].btnTransform;
				if (btnTransform)
				{
					Image component = btnTransform.gameObject.GetComponent<Image>();
					if (component)
					{
						Color color = component.get_color();
						color.a = fAlpha;
						component.set_color(color);
					}
				}
			}
		}

		private void SetEnemyHeroBtnSize(int iBtnIndex, bool bIsSmall)
		{
			float num = 1f;
			if (iBtnIndex < 0 || iBtnIndex >= this.m_iCurEnemyPlayerCount)
			{
				return;
			}
			if (bIsSmall)
			{
				num = 0.8f;
			}
			if (this.m_arrBtnInfo[iBtnIndex].btnTransform)
			{
				Vector3 localScale = this.m_arrBtnInfo[iBtnIndex].btnTransform.localScale;
				localScale.x = num;
				localScale.y = num;
				this.m_arrBtnInfo[iBtnIndex].btnTransform.localScale = localScale;
			}
			if (this.m_arrBtnInfo[iBtnIndex].objUI3DHp)
			{
				Vector3 localScale2 = this.m_arrBtnInfo[iBtnIndex].objUI3DHp.transform.localScale;
				localScale2.x = num;
				localScale2.y = num;
				this.m_arrBtnInfo[iBtnIndex].objUI3DHp.transform.localScale = localScale2;
			}
			if (this.m_objSelectedImg != null)
			{
				Vector3 localScale3 = this.m_objSelectedImg.transform.localScale;
				localScale3.x = num;
				localScale3.y = num;
				this.m_objSelectedImg.transform.localScale = localScale3;
			}
		}

		private void SetEnemyHeroBtnHighlight(int iBtnIndex, bool bIsHighlight)
		{
			if (iBtnIndex < 0 || iBtnIndex >= this.m_iCurEnemyPlayerCount)
			{
				return;
			}
			if (bIsHighlight && !this.m_arrBtnInfo[iBtnIndex].btnTransform.gameObject.activeSelf)
			{
				return;
			}
			if (this.m_objSelectedImg != null)
			{
				if (!bIsHighlight)
				{
					if (this.m_objSelectedImg.gameObject.activeSelf)
					{
						this.m_objSelectedImg.CustomSetActive(false);
						if (this.m_objSelectedImg)
						{
							this.m_objSelectedImg.CustomSetActive(false);
						}
						if (this.m_objLinker)
						{
							this.m_objLinker.gameObject.CustomSetActive(false);
						}
					}
					this.SetEnemyHeroBtnAlpha(iBtnIndex, 0.6f);
				}
				else if (bIsHighlight && this.m_arrBtnInfo[iBtnIndex].btnTransform)
				{
					this.m_objSelectedImg.transform.position = this.m_arrBtnInfo[iBtnIndex].btnTransform.position;
					if (!this.m_objSelectedImg.gameObject.activeSelf)
					{
						this.m_objSelectedImg.gameObject.CustomSetActive(true);
						if (this.m_objSelectedImg)
						{
							this.m_objSelectedImg.CustomSetActive(true);
						}
						if (this.m_objLinker)
						{
							this.m_objLinker.gameObject.CustomSetActive(true);
						}
					}
					this.SetEnemyHeroBtnAlpha(iBtnIndex, 1f);
				}
			}
		}

		private SkillSlotType BehaviorToSkillSlotType(ObjBehaviMode behaviMode)
		{
			SkillSlotType result = SkillSlotType.SLOT_SKILL_VALID;
			switch (behaviMode)
			{
			case ObjBehaviMode.Normal_Attack:
				result = SkillSlotType.SLOT_SKILL_0;
				break;
			case ObjBehaviMode.UseSkill_1:
				result = SkillSlotType.SLOT_SKILL_1;
				break;
			case ObjBehaviMode.UseSkill_2:
				result = SkillSlotType.SLOT_SKILL_2;
				break;
			case ObjBehaviMode.UseSkill_3:
				result = SkillSlotType.SLOT_SKILL_3;
				break;
			}
			return result;
		}

		private int GetDrawAttackLinkerBtnIndex()
		{
			int num = this.m_iCurTargetEnemyBtnIndex;
			if (this.m_hostActor && this.m_hostActor.handle.ActorAgent != null && this.m_hostActor.handle.ActorAgent.m_wrapper != null && num < 0)
			{
				ObjBehaviMode myBehavior = this.m_hostActor.handle.ActorAgent.m_wrapper.myBehavior;
				if (myBehavior >= ObjBehaviMode.UseSkill_1 && myBehavior <= ObjBehaviMode.UseSkill_3)
				{
					SkillSlotType type = this.BehaviorToSkillSlotType(myBehavior);
					SkillSlot skillSlot;
					if (this.m_hostActor.handle.SkillControl != null && this.m_hostActor.handle.SkillControl.TryGetSkillSlot(type, out skillSlot))
					{
						Skill skill = (skillSlot.NextSkillObj != null) ? skillSlot.NextSkillObj : skillSlot.SkillObj;
						if (skill != null && skill.cfgData != null && skill.cfgData.bRangeAppointType == 1 && skillSlot.CanUseSkillWithEnemyHeroSelectMode())
						{
							num = this.GetBtnIndexByActor(this.m_hostActor.handle.ActorAgent.m_wrapper.myTarget);
						}
					}
				}
				else if (myBehavior == ObjBehaviMode.Normal_Attack && this.m_hostActor.handle.ActorAgent.m_wrapper.myTarget && this.m_hostActor.handle.ActorAgent.m_wrapper.myTarget.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero && this.m_hostActor.handle.IsEnemyCamp(this.m_hostActor.handle.ActorAgent.m_wrapper.myTarget))
				{
					num = this.GetBtnIndexByActor(this.m_hostActor.handle.ActorAgent.m_wrapper.myTarget);
				}
			}
			return num;
		}

		public void OnSkillBtnUp(SkillSlotType skillSlotType, enSkillJoystickMode mode = enSkillJoystickMode.General)
		{
			uint num = 0u;
			if (this.m_iCurTargetEnemyBtnIndex >= 0 && this.m_iCurTargetEnemyBtnIndex < this.m_iCurEnemyPlayerCount && this.m_arrBtnInfo[this.m_iCurTargetEnemyBtnIndex].actorPtr)
			{
				num = this.m_arrBtnInfo[this.m_iCurTargetEnemyBtnIndex].actorPtr.handle.ObjID;
				this.SetEnemyHeroBtnHighlight(this.m_iCurTargetEnemyBtnIndex, false);
				this.m_iLastSkillTargetEnemyBtnIndex = this.m_iCurTargetEnemyBtnIndex;
				this.m_iCurTargetEnemyBtnIndex = -1;
				this.m_bLastSkillUsed = false;
			}
			Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
			if (hostPlayer != null && hostPlayer.Captain)
			{
				if (num != 0u)
				{
					hostPlayer.Captain.handle.SkillControl.RequestUseSkillSlot(skillSlotType, enSkillJoystickMode.EnemyHeroSelect, num);
					SkillSlot skillSlot;
					if (hostPlayer.Captain.handle.SkillControl.TryGetSkillSlot(skillSlotType, out skillSlot))
					{
						skillSlot.skillIndicator.SetFixedPrefabShow(false);
						skillSlot.skillIndicator.SetGuildPrefabShow(false);
						skillSlot.skillIndicator.SetGuildWarnPrefabShow(false);
						this.m_ShowGuidPrefabSkillSlot = skillSlotType;
					}
				}
				else
				{
					hostPlayer.Captain.handle.SkillControl.RequestUseSkillSlot(skillSlotType, mode, num);
				}
			}
		}

		public void OnSkillBtnDrag(CUIFormScript formScript, SkillSlotType skillSlotType, Vector2 dragScreenPosition, bool bIsInCancelArea)
		{
			bool flag = false;
			if (!this.m_hostActor || this.m_hostActor.handle.ActorAgent == null || this.m_hostActor.handle.ActorAgent.m_wrapper == null)
			{
				return;
			}
			BaseAttackMode currentAttackMode = this.m_hostActor.handle.ActorAgent.m_wrapper.GetCurrentAttackMode();
			uint num = 0u;
			if (currentAttackMode != null)
			{
				num = currentAttackMode.GetEnemyHeroAttackTargetID();
			}
			if (num > 0u)
			{
				flag = true;
			}
			SkillSlot skillSlot = null;
			if (Singleton<GamePlayerCenter>.GetInstance().HostPlayerId != 0u)
			{
				Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
				if (hostPlayer != null && hostPlayer.Captain && hostPlayer.Captain.handle.SkillControl != null)
				{
					hostPlayer.Captain.handle.SkillControl.TryGetSkillSlot(skillSlotType, out skillSlot);
				}
			}
			if (bIsInCancelArea)
			{
				if (!flag)
				{
					this.SetEnemyHeroBtnHighlight(this.m_iCurTargetEnemyBtnIndex, false);
				}
				this.m_iCurTargetEnemyBtnIndex = -1;
				if (skillSlot != null)
				{
					skillSlot.skillIndicator.SetFixedWarnPrefabShow(true);
					skillSlot.skillIndicator.SetGuildPrefabShow(false);
					this.m_ShowGuidPrefabSkillSlot = SkillSlotType.SLOT_SKILL_VALID;
				}
				return;
			}
			bool flag2 = false;
			for (int i = 0; i < this.m_iCurEnemyPlayerCount; i++)
			{
				if (this.m_arrBtnInfo[i].heroState == 0 && Singleton<CBattleSystem>.GetInstance().FightForm != null && Singleton<CBattleSystem>.GetInstance().FightForm.m_skillButtonManager != null && Singleton<CBattleSystem>.GetInstance().FightForm.m_skillButtonManager.IsSkillCursorInTargetArea(formScript, ref dragScreenPosition, this.m_arrBtnInfo[i].btnTransform.gameObject))
				{
					if (i != this.m_iCurTargetEnemyBtnIndex)
					{
						if (this.m_iCurTargetEnemyBtnIndex != -1 && !flag)
						{
							this.SetEnemyHeroBtnHighlight(this.m_iCurTargetEnemyBtnIndex, false);
						}
						if (!flag)
						{
							this.SetEnemyHeroBtnHighlight(i, true);
						}
						this.m_iCurTargetEnemyBtnIndex = i;
						if (skillSlot != null)
						{
							skillSlot.skillIndicator.SetFixedWarnPrefabShow(false);
							skillSlot.skillIndicator.SetGuildPrefabShow(true);
							skillSlot.skillIndicator.SetEffectPrefabShow(false);
							this.m_ShowGuidPrefabSkillSlot = SkillSlotType.SLOT_SKILL_VALID;
						}
					}
					flag2 = true;
					break;
				}
			}
			if (!flag2)
			{
				if (!flag)
				{
					this.SetEnemyHeroBtnHighlight(this.m_iCurTargetEnemyBtnIndex, false);
				}
				if (skillSlot != null)
				{
					skillSlot.skillIndicator.SetFixedWarnPrefabShow(false);
					skillSlot.skillIndicator.SetGuildPrefabShow(false);
					this.m_ShowGuidPrefabSkillSlot = SkillSlotType.SLOT_SKILL_VALID;
				}
				this.m_iCurTargetEnemyBtnIndex = -1;
			}
		}

		public uint GetLockedEnemyHeroObjId()
		{
			uint result = 0u;
			if (this.m_iCurTargetEnemyBtnIndex >= 0 && this.m_iCurTargetEnemyBtnIndex < this.m_iCurEnemyPlayerCount && this.m_arrBtnInfo[this.m_iCurTargetEnemyBtnIndex].actorPtr)
			{
				result = this.m_arrBtnInfo[this.m_iCurTargetEnemyBtnIndex].actorPtr.handle.ObjID;
			}
			return result;
		}

		public void HideEnemyHeroHeadBtn()
		{
			for (int i = 0; i < this.m_iCurEnemyPlayerCount; i++)
			{
				this.SetBtnStateByBtnInfo(i, false);
			}
		}

		public void ShowEnemyHeroHeadBtn()
		{
			if (!GameSettings.ShowEnemyHeroHeadBtnMode)
			{
				return;
			}
			for (int i = 0; i < this.m_iCurEnemyPlayerCount; i++)
			{
				if (this.m_arrBtnInfo[i].actorPtr && this.m_arrBtnInfo[i].actorPtr.handle.Visible && this.m_arrBtnInfo[i].actorPtr.handle.ValueComponent != null && this.m_arrBtnInfo[i].actorPtr.handle.ValueComponent.actorHp > 0)
				{
					this.UpdateHeroAtkBtnWithDistance(i);
				}
			}
		}

		public void LateUpdate()
		{
			try
			{
				if (this.CheckShouldUpdate())
				{
					this.UpdateAndDrawAttackLinker3D();
					this.UpdateHeroAtkBtnDistance();
				}
			}
			catch (Exception ex)
			{
				DebugHelper.Assert(false, "exception in DrawAttackLinker:{0}, \n {1}", new object[]
				{
					ex.get_Message(),
					ex.get_StackTrace()
				});
			}
		}

		private void UpdateAndDrawAttackLinker3D()
		{
			if (!GameSettings.ShowEnemyHeroHeadBtnMode)
			{
				return;
			}
			int num = this.GetDrawAttackLinkerBtnIndex();
			if ((num < 0 || num > this.m_iCurEnemyPlayerCount) && this.m_iLastDrawLinkerBtnIndex >= 0 && this.m_iLastDrawLinkerBtnIndex < this.m_iCurEnemyPlayerCount)
			{
				if (this.m_fDrawLinkerMissBtnIndexFrameTime < 0f)
				{
					this.m_fDrawLinkerMissBtnIndexFrameTime = Singleton<FrameSynchr>.instance.LogicFrameTick;
				}
				else if (Singleton<FrameSynchr>.instance.LogicFrameTick - this.m_fDrawLinkerMissBtnIndexFrameTime >= 500f)
				{
					this.SetEnemyHeroBtnHighlight(this.m_iLastDrawLinkerBtnIndex, false);
					this.m_iLastDrawLinkerBtnIndex = -1;
					this.m_fDrawLinkerMissBtnIndexFrameTime = -1f;
					return;
				}
				num = this.m_iLastDrawLinkerBtnIndex;
			}
			if (num >= 0 && num < this.m_iCurEnemyPlayerCount)
			{
				if (this.m_iLastDrawLinkerBtnIndex != num)
				{
					this.SetEnemyHeroBtnHighlight(this.m_iLastDrawLinkerBtnIndex, false);
					this.m_iLastDrawLinkerBtnIndex = num;
				}
				this.SetEnemyHeroBtnHighlight(num, true);
				this.m_iCurDrawLinkerBtnIndex = num;
				PoolObjHandle<ActorRoot> actorPtr = this.m_arrBtnInfo[num].actorPtr;
				if (actorPtr && !actorPtr.handle.ActorControl.IsDeadState && this.m_hostActor && !this.m_hostActor.handle.ActorControl.IsDeadState)
				{
					if (this.m_objSelectedImg == null || this.m_attackLinker == null)
					{
						return;
					}
					if (this.m_objSelectedImg != null && this.m_attackLinker != null && CEnemyHeroAtkBtn.m_UI3DCamera)
					{
						float num2 = (float)actorPtr.handle.CharInfo.iBulletHeight * 0.001f;
						Vector3 position = actorPtr.handle.myTransform.position;
						position.y += num2;
						Vector3 vector = Camera.main.WorldToScreenPoint(position);
						Vector3 position2 = this.m_arrBtnInfo[num].btnTransform.position;
						Vector3 vector2 = CUIUtility.WorldToScreenPoint(Singleton<CUIManager>.instance.FormCamera, position2);
						vector.z = (vector2.z = (float)MiniMapSysUT.UI3D_Depth);
						Vector3 position3 = CEnemyHeroAtkBtn.m_UI3DCamera.ScreenToWorldPoint(vector);
						Vector3 position4 = CEnemyHeroAtkBtn.m_UI3DCamera.ScreenToWorldPoint(vector2);
						this.m_objSelectedImg.transform.position = position4;
						Vector2 vector3 = vector - vector2;
						float num3 = Mathf.Atan2(vector3.y, vector3.x) * 57.29578f;
						Quaternion rotation = Quaternion.AngleAxis(num3 - 90f, Vector3.forward);
						this.m_objSelectedImg.transform.rotation = rotation;
						this.m_attackLinker.SetPosition(0, this.m_objDirection.transform.position);
						this.m_attackLinker.SetPosition(1, position3);
						if (this.m_objSelectedImg && !this.m_objSelectedImg.gameObject.activeSelf)
						{
							this.SetEnemyHeroBtnHighlight(num, true);
						}
						Singleton<Camera_UI3D>.GetInstance().GetCurrentCanvas().RefreshLayout(null);
					}
				}
				return;
			}
		}

		private bool CheckShouldUpdate()
		{
			return GameSettings.ShowEnemyHeroHeadBtnMode && !Singleton<WatchController>.GetInstance().IsWatching;
		}

		private void OnCaptainSwitched(ref DefaultGameEventParam prm)
		{
			this.m_hostActor = Singleton<GamePlayerCenter>.instance.GetHostPlayer().Captain;
		}
	}
}
