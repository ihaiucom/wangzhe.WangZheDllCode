using Assets.Scripts.Framework;
using Assets.Scripts.GameSystem;
using ResData;
using System;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public class SkillControlIndicator
	{
		private enum EEffectPrefabSelect
		{
			Normal,
			Block,
			Grass
		}

		public const float PrefabHeight = 0.3f;

		private const string PrefabBlockPostfix = "_y";

		private const string PrefabGrassPostfix = "_G";

		public Vector3 highLitColor = new Vector3(1f, 1f, 1f);

		private int hlcId;

		private SkillSlot skillSlot;

		private Vector3 useOffsetPosition = Vector3.zero;

		private Vector3 useSkillPosition = Vector3.zero;

		private Vector3 useSkillDirection = Vector3.zero;

		private Vector3 rootRosition = Vector3.zero;

		private Vector3 deltaPosition = Vector3.zero;

		private Vector3 deltaDirection = Vector3.zero;

		private Vector3 movePosition = Vector3.zero;

		private float rotateAngle;

		private float deltaAngle;

		private Vector3 rotateDirection = Vector3.zero;

		private int effectHideFrameNum;

		private GameObject guidePrefab;

		private bool guideSelectActive;

		private GameObject guideWarnPrefab;

		private bool effectSelectActive;

		private GameObject effectWarnPrefab;

		private GameObject fixedPrefab;

		private GameObject fixedWarnPrefab;

		private float moveSpeed = 0.03f;

		private float rotateSpeed = 0.5f;

		private bool bMoveFlag;

		private bool bRotateFlag;

		private bool bControlMove;

		private int pressTime;

		private bool bUseAdvanceSelect;

		private ActorRoot targetActor;

		private bool bSelectEffectPrefab;

		private bool bSkillBtnDrag;

		private SkillControlIndicator.EEffectPrefabSelect m_prefabSelecter;

		private GameObject effectPrefabNormal;

		private GameObject effectPrefabBlock;

		private GameObject effectPrefabGrass;

		public GameObject effectPrefab
		{
			get
			{
				if (!this.bSelectEffectPrefab)
				{
					return this.effectPrefabNormal;
				}
				GameObject result = this.effectPrefabNormal;
				if (this.PrefabSelecter == SkillControlIndicator.EEffectPrefabSelect.Block && this.effectPrefabBlock != null)
				{
					result = this.effectPrefabBlock;
				}
				else if (this.PrefabSelecter == SkillControlIndicator.EEffectPrefabSelect.Grass && this.effectPrefabGrass != null)
				{
					result = this.effectPrefabGrass;
				}
				return result;
			}
		}

		private SkillControlIndicator.EEffectPrefabSelect PrefabSelecter
		{
			get
			{
				return this.m_prefabSelecter;
			}
			set
			{
				if (this.m_prefabSelecter != value)
				{
					GameObject effectPrefab = this.effectPrefab;
					bool flag = this.IsPrefabVisible(this.effectPrefab);
					this.m_prefabSelecter = value;
					if (flag)
					{
						this.HidePrefab(effectPrefab);
						this.ShowPrefab(this.effectPrefab);
						this.effectPrefab.transform.position = effectPrefab.transform.position;
					}
				}
			}
		}

		public SkillControlIndicator(SkillSlot _skillSlot)
		{
			this.pressTime = 0;
			this.effectHideFrameNum = 0;
			this.bControlMove = false;
			this.skillSlot = _skillSlot;
			this.bUseAdvanceSelect = true;
			this.targetActor = null;
		}

		public void SetIndicatorSpeed(float _moveSpeed, float _rotateSpeed)
		{
			this.moveSpeed = _moveSpeed;
			this.rotateSpeed = _rotateSpeed;
		}

		public bool IsAllowUseSkill()
		{
			Skill skill = (this.skillSlot.NextSkillObj != null) ? this.skillSlot.NextSkillObj : this.skillSlot.SkillObj;
			return skill.cfgData.bRangeAppointType != 2 || this.bControlMove || this.pressTime > 1000;
		}

		private void PlayCommonAttackTargetEffect(ActorRoot actorRoot)
		{
			if (actorRoot != null)
			{
				Singleton<SkillIndicateSystem>.GetInstance().PlayCommonAttackTargetEffect(actorRoot);
				if (actorRoot.MatHurtEffect != null)
				{
					this.hlcId = actorRoot.MatHurtEffect.PlayHighLitEffect(this.highLitColor);
				}
			}
		}

		private void StopCommonAttackTargetEffect(ActorRoot actorRoot)
		{
			Singleton<SkillIndicateSystem>.GetInstance().StopCommonAttackTargetEffect();
			if (actorRoot != null && actorRoot.MatHurtEffect)
			{
				actorRoot.MatHurtEffect.StopHighLitEffect(this.hlcId);
			}
		}

		public ActorRoot GetUseSkillTargetLockAttackMode()
		{
			if (this.bUseAdvanceSelect)
			{
				return this.targetActor;
			}
			return null;
		}

		public ActorRoot GetUseSkillTargetDefaultAttackMode()
		{
			if (!this.bUseAdvanceSelect)
			{
				SkillSelectControl instance = Singleton<SkillSelectControl>.GetInstance();
				Skill skill = (this.skillSlot.NextSkillObj != null) ? this.skillSlot.NextSkillObj : this.skillSlot.SkillObj;
				if (skill.cfgData.bWheelType != 1 && skill.cfgData.bWheelType != 6)
				{
					this.targetActor = instance.SelectTarget((SkillTargetRule)skill.cfgData.bSkillTargetRule, this.skillSlot);
				}
			}
			return this.targetActor;
		}

		public void SetUseSkillTarget()
		{
			ActorRoot actorRoot = null;
			Skill skill = (this.skillSlot.NextSkillObj != null) ? this.skillSlot.NextSkillObj : this.skillSlot.SkillObj;
			if (skill == null || skill.cfgData == null || skill.cfgData.bWheelType == 1 || skill.cfgData.bWheelType == 6)
			{
				return;
			}
			uint dwSkillTargetFilter = skill.cfgData.dwSkillTargetFilter;
			if (skill.cfgData.bRangeAppointType == 1)
			{
				if (!this.bUseAdvanceSelect && this.guidePrefab != null && this.guideSelectActive)
				{
					SkillSelectControl instance = Singleton<SkillSelectControl>.GetInstance();
					actorRoot = instance.SelectTarget((SkillTargetRule)skill.cfgData.bSkillTargetRule, this.skillSlot);
				}
				else if (this.bUseAdvanceSelect && this.effectPrefab != null && this.effectSelectActive)
				{
					int srchR;
					if (this.skillSlot.SlotType != SkillSlotType.SLOT_SKILL_0)
					{
						srchR = skill.cfgData.iMaxAttackDistance;
					}
					else
					{
						srchR = skill.GetMaxSearchDistance(this.skillSlot.GetSkillLevel());
					}
					uint num = 2u;
					num |= dwSkillTargetFilter;
					actorRoot = Singleton<SectorTargetSearcher>.GetInstance().GetEnemyTarget(this.skillSlot.Actor, srchR, this.useSkillDirection, 50f, num);
					if (actorRoot == null)
					{
						uint num2 = 1u;
						num2 |= dwSkillTargetFilter;
						actorRoot = Singleton<SectorTargetSearcher>.GetInstance().GetEnemyTarget(this.skillSlot.Actor, srchR, this.useSkillDirection, 50f, num2);
					}
				}
				if (actorRoot != this.targetActor)
				{
					this.StopCommonAttackTargetEffect(this.targetActor);
					if (this.skillSlot.Actor && actorRoot != null && actorRoot.ObjID != this.skillSlot.Actor.handle.ObjID)
					{
						this.PlayCommonAttackTargetEffect(actorRoot);
					}
					this.targetActor = actorRoot;
				}
			}
		}

		public void SetUseSkillTarget(ActorRoot actorRoot)
		{
			if (actorRoot != this.targetActor)
			{
				if (this.targetActor != null)
				{
					this.StopCommonAttackTargetEffect(this.targetActor);
				}
				if (actorRoot != null)
				{
					this.PlayCommonAttackTargetEffect(actorRoot);
				}
				this.targetActor = actorRoot;
			}
		}

		public Vector3 GetUseSkillPosition()
		{
			return this.useSkillPosition;
		}

		public Vector3 GetUseSkillDirection()
		{
			return this.useSkillDirection;
		}

		public void InitControlIndicator()
		{
			if (!ActorHelper.IsHostActor(ref this.skillSlot.Actor))
			{
				return;
			}
			float num = 0f;
			float num2 = 0f;
			GameSettings.GetLunPanSensitivity(out num, out num2);
			this.SetIndicatorSpeed(num, num2);
		}

		private void SetPrefabTag(GameObject _prefab)
		{
			if (_prefab != null)
			{
				_prefab.tag = "SCI";
			}
		}

		private void ShowPrefab(GameObject _prefab)
		{
			if (_prefab != null)
			{
				_prefab.SetLayer("Actor", "Particles", false);
			}
		}

		private void HidePrefab(GameObject _prefab)
		{
			if (_prefab != null)
			{
				_prefab.SetLayer("Hide", false);
			}
		}

		private bool IsPrefabVisible(GameObject _prefab)
		{
			if (_prefab != null)
			{
				int num = LayerMask.NameToLayer("Hide");
				if (_prefab.layer != num)
				{
					return true;
				}
			}
			return false;
		}

		private void ShowPrefabEffect()
		{
			this.ShowPrefab(this.effectPrefab);
		}

		private void HidePrefabEffect()
		{
			this.PrefabSelecter = SkillControlIndicator.EEffectPrefabSelect.Normal;
			this.HidePrefab(this.effectPrefab);
		}

		public void UpdatePrefabScaler(Skill _skillObj)
		{
			if (_skillObj != null)
			{
				int distance = _skillObj.cfgData.iFixedDistance;
				int distance2 = _skillObj.cfgData.iGuideDistance;
				this.SetPrefabScaler(this.guidePrefab, distance2);
				this.SetPrefabScaler(this.guideWarnPrefab, distance2);
				if (_skillObj.cfgData.bRangeAppointType == 3 || _skillObj.cfgData.bRangeAppointType == 1)
				{
					this.SetPrefabScaler(this.effectPrefabNormal, distance2);
					this.SetPrefabScaler(this.effectPrefabBlock, distance2);
					this.SetPrefabScaler(this.effectPrefabGrass, distance2);
					this.SetPrefabScaler(this.effectWarnPrefab, distance2);
				}
				this.SetPrefabScaler(this.fixedPrefab, distance);
				this.SetPrefabScaler(this.fixedWarnPrefab, distance);
			}
		}

		public void CreateIndicatePrefab(Skill _skillObj)
		{
			if (!this.skillSlot.Actor || !ActorHelper.IsHostActor(ref this.skillSlot.Actor))
			{
				return;
			}
			if (_skillObj == null || _skillObj.cfgData == null)
			{
				return;
			}
			this.effectHideFrameNum = 0;
			ActorRoot handle = this.skillSlot.Actor.handle;
			Quaternion rotation = handle.myTransform.rotation;
			Vector3 position = handle.myTransform.position;
			position.y += 0.3f;
			GameObject pooledGameObjLOD = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD(_skillObj.GuidePrefabName, true, SceneObjType.ActionRes, position, rotation);
			if (pooledGameObjLOD != null)
			{
				this.guidePrefab = pooledGameObjLOD;
				this.guidePrefab.transform.SetParent(handle.myTransform);
				this.HidePrefab(this.guidePrefab);
				this.SetPrefabTag(this.guidePrefab);
				this.guideSelectActive = false;
			}
			pooledGameObjLOD = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD(_skillObj.GuideWarnPrefabName, true, SceneObjType.ActionRes, position, rotation);
			if (pooledGameObjLOD != null)
			{
				this.guideWarnPrefab = pooledGameObjLOD;
				this.guideWarnPrefab.transform.SetParent(handle.myTransform);
				this.HidePrefab(this.guideWarnPrefab);
				this.SetPrefabTag(this.guideWarnPrefab);
			}
			pooledGameObjLOD = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD(_skillObj.EffectPrefabName, true, SceneObjType.ActionRes, position, rotation);
			if (pooledGameObjLOD != null)
			{
				this.effectPrefabNormal = pooledGameObjLOD;
				this.HidePrefab(this.effectPrefabNormal);
				this.SetPrefabTag(this.effectPrefabNormal);
				this.effectSelectActive = false;
				MonoSingleton<SceneMgr>.GetInstance().AddToRoot(this.effectPrefabNormal, SceneObjType.ActionRes);
			}
			this.bSelectEffectPrefab = (_skillObj.cfgData.bSelectEffectPrefab > 0);
			if (this.bSelectEffectPrefab)
			{
				pooledGameObjLOD = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD(_skillObj.EffectPrefabName + "_y", true, SceneObjType.ActionRes, position, rotation);
				if (pooledGameObjLOD != null)
				{
					this.effectPrefabBlock = pooledGameObjLOD;
					this.HidePrefab(this.effectPrefabBlock);
					this.SetPrefabTag(this.effectPrefabBlock);
					MonoSingleton<SceneMgr>.GetInstance().AddToRoot(this.effectPrefabBlock, SceneObjType.ActionRes);
				}
				pooledGameObjLOD = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD(_skillObj.EffectPrefabName + "_G", true, SceneObjType.ActionRes, position, rotation);
				if (pooledGameObjLOD != null)
				{
					this.effectPrefabGrass = pooledGameObjLOD;
					this.HidePrefab(this.effectPrefabGrass);
					this.SetPrefabTag(this.effectPrefabGrass);
					MonoSingleton<SceneMgr>.GetInstance().AddToRoot(this.effectPrefabGrass, SceneObjType.ActionRes);
				}
			}
			pooledGameObjLOD = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD(_skillObj.EffectWarnPrefabName, true, SceneObjType.ActionRes, position, rotation);
			if (pooledGameObjLOD != null)
			{
				this.effectWarnPrefab = pooledGameObjLOD;
				this.HidePrefab(this.effectWarnPrefab);
				this.SetPrefabTag(this.effectWarnPrefab);
				MonoSingleton<SceneMgr>.GetInstance().AddToRoot(this.effectWarnPrefab, SceneObjType.ActionRes);
			}
			pooledGameObjLOD = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD(_skillObj.FixedPrefabName, true, SceneObjType.ActionRes, position, rotation);
			if (pooledGameObjLOD != null)
			{
				this.fixedPrefab = pooledGameObjLOD;
				this.HidePrefab(this.fixedPrefab);
				this.SetPrefabTag(this.fixedPrefab);
				this.fixedPrefab.transform.SetParent(handle.myTransform);
			}
			pooledGameObjLOD = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD(_skillObj.FixedWarnPrefabName, true, SceneObjType.ActionRes, position, rotation);
			if (pooledGameObjLOD != null)
			{
				this.fixedWarnPrefab = pooledGameObjLOD;
				this.HidePrefab(this.fixedWarnPrefab);
				this.SetPrefabTag(this.fixedWarnPrefab);
				this.fixedWarnPrefab.transform.SetParent(handle.myTransform);
			}
			int distance = _skillObj.cfgData.iGuideDistance;
			this.SetPrefabScaler(this.guidePrefab, distance);
			this.SetPrefabScaler(this.guideWarnPrefab, distance);
			if (_skillObj.cfgData.bRangeAppointType == 3 || _skillObj.cfgData.bRangeAppointType == 1)
			{
				this.SetPrefabScaler(this.effectPrefabNormal, distance);
				this.SetPrefabScaler(this.effectPrefabBlock, distance);
				this.SetPrefabScaler(this.effectPrefabGrass, distance);
				this.SetPrefabScaler(this.effectWarnPrefab, distance);
			}
			int distance2 = _skillObj.cfgData.iFixedDistance;
			this.SetPrefabScaler(this.fixedPrefab, distance2);
			this.SetPrefabScaler(this.fixedWarnPrefab, distance2);
		}

		public void SetIndicatorToCallMonster()
		{
			if (this.skillSlot != null && this.skillSlot.Actor)
			{
				HeroWrapper heroWrapper = this.skillSlot.Actor.handle.ActorControl as HeroWrapper;
				if (heroWrapper != null && heroWrapper.hasCalledMonster)
				{
					if (this.guidePrefab != null)
					{
						this.guidePrefab.transform.SetParent(heroWrapper.CallMonster.handle.myTransform);
						this.guidePrefab.transform.localPosition = Vector3.zero;
					}
					if (this.guideWarnPrefab != null)
					{
						this.guideWarnPrefab.transform.SetParent(heroWrapper.CallMonster.handle.myTransform);
						this.guideWarnPrefab.transform.localPosition = Vector3.zero;
					}
					if (this.fixedPrefab != null)
					{
						this.fixedPrefab.transform.SetParent(heroWrapper.CallMonster.handle.myTransform);
						this.fixedPrefab.transform.localPosition = Vector3.zero;
					}
					if (this.fixedWarnPrefab != null)
					{
						this.fixedWarnPrefab.transform.SetParent(heroWrapper.CallMonster.handle.myTransform);
						this.fixedWarnPrefab.transform.localPosition = Vector3.zero;
					}
				}
			}
		}

		public void UnInitIndicatePrefab(bool bDestroy)
		{
			if (!this.skillSlot.Actor || !ActorHelper.IsHostActor(ref this.skillSlot.Actor))
			{
				return;
			}
			if (this.guidePrefab != null)
			{
				this.HidePrefab(this.guidePrefab);
				this.guideSelectActive = false;
				if (bDestroy)
				{
					Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(this.guidePrefab);
					this.guidePrefab = null;
				}
			}
			if (this.effectPrefabNormal != null)
			{
				this.HidePrefab(this.effectPrefabNormal);
				this.effectSelectActive = false;
				if (bDestroy)
				{
					Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(this.effectPrefabNormal);
					this.effectPrefabNormal = null;
				}
			}
			if (this.effectPrefabBlock != null)
			{
				this.HidePrefab(this.effectPrefabBlock);
				if (bDestroy)
				{
					Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(this.effectPrefabBlock);
					this.effectPrefabBlock = null;
				}
			}
			if (this.effectPrefabGrass != null)
			{
				this.HidePrefab(this.effectPrefabGrass);
				if (bDestroy)
				{
					Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(this.effectPrefabGrass);
					this.effectPrefabGrass = null;
				}
			}
			if (this.guideWarnPrefab != null)
			{
				this.HidePrefab(this.guideWarnPrefab);
				if (bDestroy)
				{
					Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(this.guideWarnPrefab);
					this.guideWarnPrefab = null;
				}
			}
			if (this.effectWarnPrefab != null)
			{
				this.HidePrefab(this.effectWarnPrefab);
				if (bDestroy)
				{
					Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(this.effectWarnPrefab);
					this.effectWarnPrefab = null;
				}
			}
			if (this.fixedPrefab != null)
			{
				this.HidePrefab(this.fixedPrefab);
				if (bDestroy)
				{
					Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(this.fixedPrefab);
					this.fixedPrefab = null;
				}
			}
			if (this.fixedWarnPrefab != null)
			{
				this.HidePrefab(this.fixedWarnPrefab);
				if (bDestroy)
				{
					Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(this.fixedWarnPrefab);
					this.fixedWarnPrefab = null;
				}
			}
		}

		public void SetPrefabScaler(GameObject _obj, int _distance)
		{
			if (null == _obj)
			{
				return;
			}
			if (_obj != null)
			{
				ParticleScaler[] componentsInChildren = _obj.GetComponentsInChildren<ParticleScaler>(true);
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].particleScale = (float)_distance / 10000f;
					componentsInChildren[i].CheckAndApplyScale();
				}
			}
		}

		public void SetEffectPrefabShow(bool bShow)
		{
			if (this.effectPrefab != null && !Singleton<GameInput>.GetInstance().IsSmartUse())
			{
				if (bShow)
				{
					this.ShowPrefabEffect();
				}
				else
				{
					this.HidePrefabEffect();
				}
				this.effectSelectActive = bShow;
			}
		}

		public void SetEffectWarnPrefabShow(bool bShow)
		{
			if (this.effectWarnPrefab != null && !Singleton<GameInput>.GetInstance().IsSmartUse())
			{
				if (bShow)
				{
					this.ShowPrefab(this.effectWarnPrefab);
				}
				else
				{
					this.HidePrefab(this.effectWarnPrefab);
				}
			}
		}

		public void SetGuildPrefabShow(bool bShow)
		{
			if (bShow)
			{
				this.effectHideFrameNum = 0;
				this.ForceSetGuildPrefabShow(bShow);
			}
			else
			{
				this.effectHideFrameNum = Time.frameCount;
			}
		}

		private void ForceSetGuildPrefabShow(bool bShow)
		{
			if (this.guidePrefab != null)
			{
				if (bShow)
				{
					this.ShowPrefab(this.guidePrefab);
				}
				else
				{
					this.HidePrefab(this.guidePrefab);
				}
				this.guideSelectActive = bShow;
			}
			if (this.effectPrefab != null && !Singleton<GameInput>.GetInstance().IsSmartUse())
			{
				if (bShow)
				{
					this.ShowPrefabEffect();
				}
				else
				{
					this.HidePrefabEffect();
				}
				this.effectSelectActive = bShow;
			}
		}

		public void SetGuildWarnPrefabShow(bool bShow)
		{
			if (this.guideWarnPrefab != null)
			{
				if (bShow)
				{
					this.ShowPrefab(this.guideWarnPrefab);
				}
				else
				{
					this.HidePrefab(this.guideWarnPrefab);
				}
			}
			if (this.effectWarnPrefab != null && !Singleton<GameInput>.GetInstance().IsSmartUse())
			{
				if (bShow)
				{
					this.ShowPrefab(this.effectWarnPrefab);
				}
				else
				{
					this.HidePrefab(this.effectWarnPrefab);
				}
			}
			if (this.fixedWarnPrefab != null && !Singleton<GameInput>.GetInstance().IsSmartUse())
			{
				if (bShow)
				{
					this.ShowPrefab(this.fixedWarnPrefab);
				}
				else
				{
					this.HidePrefab(this.fixedWarnPrefab);
				}
			}
		}

		public void SetFixedPrefabShow(bool bShow)
		{
			if (this.fixedPrefab != null)
			{
				if (bShow)
				{
					this.ShowPrefab(this.fixedPrefab);
				}
				else
				{
					this.HidePrefab(this.fixedPrefab);
				}
			}
		}

		public void SetFixedWarnPrefabShow(bool bShow)
		{
			if (this.fixedWarnPrefab != null)
			{
				if (bShow)
				{
					this.ShowPrefab(this.fixedWarnPrefab);
				}
				else
				{
					this.HidePrefab(this.fixedWarnPrefab);
				}
			}
		}

		public void SetUseAdvanceMode(bool b)
		{
			this.bUseAdvanceSelect = b;
		}

		public bool GetUseAdvanceMode()
		{
			return this.bUseAdvanceSelect;
		}

		public void LateUpdate(int nDelta)
		{
			if (this.skillSlot == null || this.skillSlot.SkillObj == null || this.skillSlot.SkillObj.cfgData == null)
			{
				return;
			}
			if (this.effectHideFrameNum > 0 && Time.frameCount > this.effectHideFrameNum)
			{
				this.ForceSetGuildPrefabShow(false);
				this.effectHideFrameNum = 0;
			}
			this.pressTime += nDelta;
			if (this.effectPrefab != null)
			{
				if (this.bMoveFlag)
				{
					Vector3 b = this.deltaDirection * this.moveSpeed * (float)nDelta;
					this.deltaPosition += b;
					if (this.deltaPosition.sqrMagnitude >= this.movePosition.sqrMagnitude)
					{
						this.bMoveFlag = false;
						this.useSkillPosition = this.skillSlot.Actor.handle.myTransform.position + this.useOffsetPosition;
						this.effectPrefab.transform.position = this.useSkillPosition;
					}
					else
					{
						this.useSkillPosition = this.effectPrefab.transform.position + b;
						this.useSkillPosition += this.skillSlot.Actor.handle.myTransform.position - this.rootRosition;
						this.effectPrefab.transform.position = this.useSkillPosition;
						this.rootRosition = this.skillSlot.Actor.handle.myTransform.position;
					}
				}
				else
				{
					this.useSkillPosition += this.skillSlot.Actor.handle.myTransform.position - this.rootRosition;
					this.effectPrefab.transform.position = this.useSkillPosition;
					this.rootRosition = this.skillSlot.Actor.handle.myTransform.position;
				}
				if (this.bRotateFlag)
				{
					float num = this.rotateSpeed * (float)nDelta;
					this.deltaAngle += num;
					if (this.deltaAngle >= this.rotateAngle)
					{
						this.bRotateFlag = false;
						this.useSkillDirection = this.rotateDirection;
						this.effectPrefab.transform.forward = this.useSkillDirection;
					}
					else
					{
						Vector3 point = this.effectPrefab.transform.forward;
						if (Vector3.Cross(this.useSkillDirection, this.rotateDirection).y < 0f)
						{
							point = Quaternion.Euler(0f, -num, 0f) * point;
						}
						else
						{
							point = Quaternion.Euler(0f, num, 0f) * point;
						}
						this.useSkillDirection = point;
						this.effectPrefab.transform.forward = this.useSkillDirection;
					}
				}
				VInt ob = 0;
				if (PathfindingUtility.GetGroundY((VInt3)this.effectPrefab.transform.position, out ob))
				{
					Vector3 position = this.effectPrefab.transform.position;
					position.y = (float)ob + 0.3f;
					this.effectPrefab.transform.position = position;
				}
			}
			if (this.effectWarnPrefab != null && this.effectPrefab != null)
			{
				this.effectWarnPrefab.transform.position = this.effectPrefab.transform.position;
				this.effectWarnPrefab.transform.forward = this.effectPrefab.transform.forward;
			}
			this.SetUseSkillTarget();
			if (this.bSelectEffectPrefab && this.IsPrefabVisible(this.effectPrefab) && FogOfWar.enable)
			{
				VInt3 vInt = (VInt3)this.effectPrefab.transform.position;
				if (!PathfindingUtility.IsValidTarget(this.skillSlot.Actor.handle, vInt))
				{
					this.PrefabSelecter = SkillControlIndicator.EEffectPrefabSelect.Block;
				}
				else if (Singleton<GameFowManager>.instance.QueryAttr(vInt) == FieldObj.EViewBlockType.Grass)
				{
					this.PrefabSelecter = SkillControlIndicator.EEffectPrefabSelect.Grass;
				}
				else if (SkillControlIndicator.CheckGrassAttaching(ref vInt))
				{
					this.PrefabSelecter = SkillControlIndicator.EEffectPrefabSelect.Grass;
				}
				else
				{
					this.PrefabSelecter = SkillControlIndicator.EEffectPrefabSelect.Normal;
				}
			}
		}

		public static bool CheckGrassAttaching(ref VInt3 newPos)
		{
			FieldObj pFieldObj = Singleton<GameFowManager>.instance.m_pFieldObj;
			VInt3 vInt = new VInt3(newPos.x, newPos.z, 0);
			VInt2 zero = VInt2.zero;
			pFieldObj.LevelGrid.WorldPosToGrid(vInt, out zero.x, out zero.y);
			FieldObj.SViewBlockAttr sViewBlockAttr = default(FieldObj.SViewBlockAttr);
			pFieldObj.QueryAttr(zero, out sViewBlockAttr);
			if (sViewBlockAttr.BlockType == 0 && pFieldObj.FindNearestGrid(zero, vInt, FieldObj.EViewBlockType.Grass, false, 1, null, out zero))
			{
				pFieldObj.LevelGrid.GridToWorldPos(zero.x, zero.y, out vInt);
				int num = MonoSingleton<GlobalConfig>.instance.GrassEyeAbsorbDist;
				num += pFieldObj.PaneX / 2;
				if (vInt.x + num > newPos.x && vInt.x - num < newPos.x && vInt.y + num > newPos.z && vInt.y - num < newPos.z)
				{
					newPos = new VInt3(vInt.x, newPos.y, vInt.y);
					return true;
				}
			}
			return false;
		}

		public void SelectSkillTarget(Vector2 axis, bool isSkillCursorInCancelArea, bool isControlMove = true)
		{
			SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
			Skill skill = (this.skillSlot.NextSkillObj != null) ? this.skillSlot.NextSkillObj : this.skillSlot.SkillObj;
			if (curLvelContext != null && curLvelContext.m_isCameraFlip)
			{
				axis = -axis;
			}
			Vector3 zero = Vector3.zero;
			Vector3 zero2 = Vector3.zero;
			if (this.effectPrefab != null && skill != null)
			{
				if (skill.cfgData.bRangeAppointType == 1)
				{
					zero2.x = skill.cfgData.iGuideDistance * axis.x;
					zero2.z = skill.cfgData.iGuideDistance * axis.y;
					if ((double)zero2.magnitude <= (double)skill.cfgData.iGuideDistance * 0.5)
					{
						this.bUseAdvanceSelect = false;
					}
					else
					{
						this.bUseAdvanceSelect = true;
					}
					zero.x = axis.x;
					zero.z = axis.y;
					this.useOffsetPosition = Vector3.zero;
					this.bRotateFlag = true;
					this.rotateAngle = Vector3.Angle(this.useSkillDirection, zero);
					this.deltaAngle = 0f;
					this.rotateDirection = zero;
					this.rootRosition = this.skillSlot.Actor.handle.myTransform.position;
					this.useSkillPosition = this.skillSlot.Actor.handle.myTransform.position;
				}
				else if (skill.cfgData.bRangeAppointType == 2)
				{
					zero2.x = skill.cfgData.iGuideDistance / 1000f * axis.x;
					zero2.z = skill.cfgData.iGuideDistance / 1000f * axis.y;
					this.useOffsetPosition = zero2;
					this.movePosition = this.skillSlot.Actor.handle.myTransform.position + zero2 - this.effectPrefab.transform.position;
					this.movePosition.y = 0f;
					this.deltaDirection = this.movePosition;
					this.deltaDirection.Normalize();
					this.deltaPosition = Vector3.zero;
					this.bMoveFlag = true;
					this.rootRosition = this.skillSlot.Actor.handle.myTransform.position;
					if (isControlMove)
					{
						this.bControlMove = true;
					}
					if (skill.cfgData.bIndicatorType == 1)
					{
						zero.x = axis.x;
						zero.z = axis.y;
						this.bRotateFlag = true;
						this.rotateAngle = Vector3.Angle(this.useSkillDirection, zero);
						this.deltaAngle = 0f;
						this.rotateDirection = zero;
					}
				}
				else if (skill.cfgData.bRangeAppointType == 3)
				{
					if (axis == Vector2.zero && !isSkillCursorInCancelArea)
					{
						return;
					}
					zero.x = axis.x;
					zero.z = axis.y;
					this.useOffsetPosition = Vector3.zero;
					this.bRotateFlag = true;
					this.rotateAngle = Vector3.Angle(this.useSkillDirection, zero);
					this.deltaAngle = 0f;
					this.rotateDirection = zero;
					this.rootRosition = this.skillSlot.Actor.handle.myTransform.position;
					this.useSkillPosition = this.skillSlot.Actor.handle.myTransform.position;
				}
			}
			if (isSkillCursorInCancelArea)
			{
				if (this.effectWarnPrefab != null && this.effectPrefab != null)
				{
					this.effectWarnPrefab.transform.position = this.effectPrefab.transform.position;
					this.effectWarnPrefab.transform.forward = this.effectPrefab.transform.forward;
				}
				this.SetGuildWarnPrefabShow(true);
				this.SetGuildPrefabShow(false);
				this.SetFixedPrefabShow(false);
			}
			else
			{
				this.SetGuildWarnPrefabShow(false);
				this.SetFixedPrefabShow(true);
				if (!this.bUseAdvanceSelect)
				{
					this.SetGuildPrefabShow(false);
					this.SetEffectPrefabShow(false);
				}
				else
				{
					this.SetGuildPrefabShow(true);
					this.SetEffectPrefabShow(true);
				}
			}
		}

		public void SetSkillUsePosition(ActorRoot target)
		{
			Vector3 vector = Vector3.zero;
			Vector3 forward = Vector3.zero;
			Skill skill = (this.skillSlot.NextSkillObj != null) ? this.skillSlot.NextSkillObj : this.skillSlot.SkillObj;
			if (skill == null)
			{
				return;
			}
			if (skill.cfgData.bRangeAppointType == 2)
			{
				this.bControlMove = true;
				this.useSkillPosition = target.myTransform.position;
				vector = target.myTransform.position - this.skillSlot.Actor.handle.myTransform.position;
				this.useOffsetPosition = vector;
				forward = vector;
				forward.y = 0f;
				forward.Normalize();
				this.useSkillDirection = forward;
				if (this.effectPrefab != null)
				{
					this.effectPrefab.transform.forward = forward;
					this.effectPrefab.transform.position = target.myTransform.position;
					this.effectPrefab.transform.Translate(0f, 0.3f, 0f);
				}
			}
			else if (skill.cfgData.bRangeAppointType == 3)
			{
				this.useSkillPosition = this.skillSlot.Actor.handle.myTransform.position;
				vector = target.myTransform.position - this.skillSlot.Actor.handle.myTransform.position;
				forward = vector;
				forward.y = 0f;
				forward.Normalize();
				this.useSkillDirection = forward;
				if (this.effectPrefab != null)
				{
					this.effectPrefab.transform.forward = forward;
					this.effectPrefab.transform.position = this.skillSlot.Actor.handle.myTransform.position;
					this.effectPrefab.transform.Translate(0f, 0.3f, 0f);
				}
			}
		}

		public void SetSkillUseDefaultPosition()
		{
			this.pressTime = 0;
			this.bControlMove = false;
			this.useOffsetPosition = Vector3.zero;
			this.useSkillPosition = this.skillSlot.Actor.handle.myTransform.position;
			this.useSkillDirection = this.skillSlot.Actor.handle.myTransform.forward;
			if (this.effectPrefab != null)
			{
				this.effectPrefab.transform.position = this.skillSlot.Actor.handle.myTransform.position;
				this.effectPrefab.transform.Translate(0f, 0.3f, 0f);
				this.effectPrefab.transform.forward = this.skillSlot.Actor.handle.myTransform.forward;
			}
		}

		public void SetSkillBtnDrag(bool bDrag)
		{
			this.bSkillBtnDrag = bDrag;
		}

		public bool GetSkillBtnDrag()
		{
			return this.bSkillBtnDrag;
		}
	}
}
