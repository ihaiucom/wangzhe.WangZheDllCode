using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.GameSystem;
using System;
using UnityEngine;

namespace AGE
{
	[EventCategory("Effect")]
	public class TriggerParticle : DurationEvent
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int targetId;

		[ObjectTemplate(new Type[]
		{

		})]
		public int objectSpaceId = -1;

		[ObjectTemplate(new Type[]
		{

		})]
		public int VirtualAttachBulletId = -1;

		[AssetReference(AssetRefType.Particle)]
		public string resourceName = string.Empty;

		public bool bUseSkin;

		public bool bUseSkinAdvance;

		[SubObject]
		public string bindPointName = string.Empty;

		public Vector3 bindPosOffset = new Vector3(0f, 0f, 0f);

		public Quaternion bindRotOffset = new Quaternion(0f, 0f, 0f, 1f);

		public Vector3 scaling = new Vector3(1f, 1f, 1f);

		public bool bEnableOptCull = true;

		public bool bBulletPos;

		public bool bBulletDir;

		public bool bBullerPosDir;

		public bool enableLayer;

		public int layer;

		public bool enableTag;

		public string tag = string.Empty;

		public bool applyActionSpeedToParticle = true;

		protected GameObject particleObject;

		public int extend = 10;

		public bool bOnlyFollowPos;

		public int iDelayDisappearTime;

		private Vector3 offsetPosition;

		private Transform followTransform;

		private Transform particleTransform;

		public int particleScaleGrow;

		private ParticleSystem[] particleSystem;

		private float[] particleSystemSize;

		private MeshRenderer[] particleMeshRender;

		private Vector3[] particleMeshRenderScale;

		public override bool SupportEditMode()
		{
			return true;
		}

		public override BaseEvent Clone()
		{
			TriggerParticle triggerParticle = ClassObjPool<TriggerParticle>.Get();
			triggerParticle.CopyData(this);
			return triggerParticle;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			TriggerParticle triggerParticle = src as TriggerParticle;
			this.targetId = triggerParticle.targetId;
			this.objectSpaceId = triggerParticle.objectSpaceId;
			this.VirtualAttachBulletId = triggerParticle.VirtualAttachBulletId;
			this.resourceName = triggerParticle.resourceName;
			this.bindPointName = triggerParticle.bindPointName;
			this.bindPosOffset = triggerParticle.bindPosOffset;
			this.bindRotOffset = triggerParticle.bindRotOffset;
			this.scaling = triggerParticle.scaling;
			this.bEnableOptCull = triggerParticle.bEnableOptCull;
			this.bBulletPos = triggerParticle.bBulletPos;
			this.bBulletDir = triggerParticle.bBulletDir;
			this.bBullerPosDir = triggerParticle.bBullerPosDir;
			this.enableLayer = triggerParticle.enableLayer;
			this.layer = triggerParticle.layer;
			this.enableTag = triggerParticle.enableTag;
			this.tag = triggerParticle.tag;
			this.applyActionSpeedToParticle = triggerParticle.applyActionSpeedToParticle;
			this.particleObject = triggerParticle.particleObject;
			this.extend = triggerParticle.extend;
			this.bOnlyFollowPos = triggerParticle.bOnlyFollowPos;
			this.bUseSkin = triggerParticle.bUseSkin;
			this.bUseSkinAdvance = triggerParticle.bUseSkinAdvance;
			this.iDelayDisappearTime = triggerParticle.iDelayDisappearTime;
			this.particleScaleGrow = triggerParticle.particleScaleGrow;
			this.particleSystem = triggerParticle.particleSystem;
			this.particleSystemSize = triggerParticle.particleSystemSize;
			this.particleMeshRender = triggerParticle.particleMeshRender;
			this.particleMeshRenderScale = triggerParticle.particleMeshRenderScale;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.targetId = 0;
			this.objectSpaceId = -1;
			this.VirtualAttachBulletId = -1;
			this.resourceName = string.Empty;
			this.bindPointName = string.Empty;
			this.bindPosOffset = new Vector3(0f, 0f, 0f);
			this.bindRotOffset = new Quaternion(0f, 0f, 0f, 1f);
			this.scaling = new Vector3(1f, 1f, 1f);
			this.bEnableOptCull = true;
			this.bBulletPos = false;
			this.bBulletDir = false;
			this.bBullerPosDir = false;
			this.enableLayer = false;
			this.layer = 0;
			this.enableTag = false;
			this.tag = string.Empty;
			this.applyActionSpeedToParticle = true;
			this.particleObject = null;
			this.extend = 10;
			this.offsetPosition = Vector3.zero;
			this.followTransform = null;
			this.particleTransform = null;
			this.bOnlyFollowPos = false;
			this.bUseSkin = false;
			this.bUseSkinAdvance = false;
			this.iDelayDisappearTime = 0;
			this.particleScaleGrow = 0;
			this.particleSystem = null;
			this.particleSystemSize = null;
			this.particleMeshRender = null;
			this.particleMeshRenderScale = null;
		}

		public override void Enter(Action _action, Track _track)
		{
			SkillUseContext skillUseContext = null;
			Vector3 vector = this.bindPosOffset;
			Quaternion quaternion = this.bindRotOffset;
			GameObject gameObject = _action.GetGameObject(this.targetId);
			GameObject gameObject2 = _action.GetGameObject(this.objectSpaceId);
			Transform transform = null;
			Transform transform2 = null;
			if (this.bindPointName.Length == 0)
			{
				if (gameObject != null)
				{
					transform = gameObject.transform;
					PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.targetId);
					this.followTransform = transform;
				}
				else if (gameObject2 != null)
				{
					transform2 = gameObject2.transform;
				}
			}
			else if (gameObject != null)
			{
				GameObject gameObject3 = SubObject.FindSubObject(gameObject, this.bindPointName);
				if (gameObject3 != null)
				{
					transform = gameObject3.transform;
				}
				else if (gameObject != null)
				{
					transform = gameObject.transform;
				}
			}
			else if (gameObject2 != null)
			{
				GameObject gameObject3 = SubObject.FindSubObject(gameObject2, this.bindPointName);
				if (gameObject3 != null)
				{
					transform2 = gameObject3.transform;
				}
				else if (gameObject != null)
				{
					transform2 = gameObject2.transform;
				}
			}
			if (this.bEnableOptCull && transform2 && transform2.gameObject.layer == LayerMask.NameToLayer("Hide") && !FogOfWar.enable)
			{
				return;
			}
			if (this.bBulletPos)
			{
				VInt3 zero = VInt3.zero;
				_action.refParams.GetRefParam("_BulletPos", ref zero);
				vector = (Vector3)zero;
				quaternion = Quaternion.identity;
				if (this.bBulletDir)
				{
					VInt3 zero2 = VInt3.zero;
					if (_action.refParams.GetRefParam("_BulletDir", ref zero2))
					{
						quaternion = Quaternion.LookRotation((Vector3)zero2);
					}
				}
			}
			else if (transform != null)
			{
				vector = transform.localToWorldMatrix.MultiplyPoint(this.bindPosOffset);
				quaternion = transform.rotation * this.bindRotOffset;
			}
			else if (transform2 != null)
			{
				if (gameObject2 != null)
				{
					PoolObjHandle<ActorRoot> actorHandle2 = _action.GetActorHandle(this.objectSpaceId);
					if (actorHandle2)
					{
						vector = (Vector3)IntMath.Transform((VInt3)this.bindPosOffset, actorHandle2.handle.forward, actorHandle2.handle.location);
						quaternion = Quaternion.LookRotation((Vector3)actorHandle2.handle.forward) * this.bindRotOffset;
					}
				}
				else
				{
					vector = transform2.localToWorldMatrix.MultiplyPoint(this.bindPosOffset);
					quaternion = transform2.rotation * this.bindRotOffset;
				}
				if (this.bBulletDir)
				{
					VInt3 zero3 = VInt3.zero;
					if (_action.refParams.GetRefParam("_BulletDir", ref zero3))
					{
						quaternion = Quaternion.LookRotation((Vector3)zero3);
						quaternion *= this.bindRotOffset;
					}
				}
				else if (this.bBullerPosDir)
				{
					skillUseContext = _action.refParams.GetRefParamObject<SkillUseContext>("SkillContext");
					if (skillUseContext != null)
					{
						PoolObjHandle<ActorRoot> originator = skillUseContext.Originator;
						if (originator)
						{
							Vector3 a = transform2.position;
							if (gameObject2 != null)
							{
								PoolObjHandle<ActorRoot> actorHandle3 = _action.GetActorHandle(this.objectSpaceId);
								if (actorHandle3)
								{
									a = (Vector3)actorHandle3.handle.location;
								}
							}
							Vector3 forward = a - (Vector3)originator.handle.location;
							quaternion = Quaternion.LookRotation(forward);
							quaternion *= this.bindRotOffset;
						}
					}
				}
			}
			bool flag = false;
			string prefabName;
			if (this.bUseSkin)
			{
				prefabName = SkinResourceHelper.GetResourceName(_action, this.resourceName, this.bUseSkinAdvance);
			}
			else
			{
				prefabName = this.resourceName;
			}
			if (skillUseContext == null)
			{
				skillUseContext = _action.refParams.GetRefParamObject<SkillUseContext>("SkillContext");
			}
			bool flag2 = true;
			int particleLOD = GameSettings.ParticleLOD;
			if (GameSettings.DynamicParticleLOD)
			{
				if (skillUseContext != null && skillUseContext.Originator && skillUseContext.Originator.handle.TheActorMeta.PlayerId == Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer().PlayerId)
				{
					flag2 = false;
				}
				if (!flag2 && particleLOD > 1)
				{
					GameSettings.ParticleLOD = 1;
				}
				MonoSingleton<SceneMgr>.GetInstance().m_dynamicLOD = flag2;
			}
			this.particleObject = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD(prefabName, true, SceneObjType.ActionRes, vector, quaternion, out flag);
			if (this.particleObject != null)
			{
				this.particleTransform = this.particleObject.transform;
			}
			if (GameSettings.DynamicParticleLOD)
			{
				MonoSingleton<SceneMgr>.GetInstance().m_dynamicLOD = false;
			}
			if (this.particleObject == null)
			{
				if (GameSettings.DynamicParticleLOD)
				{
					MonoSingleton<SceneMgr>.GetInstance().m_dynamicLOD = flag2;
				}
				this.particleObject = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD(this.resourceName, true, SceneObjType.ActionRes, vector, quaternion, out flag);
				if (GameSettings.DynamicParticleLOD)
				{
					MonoSingleton<SceneMgr>.GetInstance().m_dynamicLOD = false;
				}
				if (this.particleObject == null)
				{
					if (GameSettings.DynamicParticleLOD)
					{
						GameSettings.ParticleLOD = particleLOD;
					}
					return;
				}
				this.particleTransform = this.particleObject.transform;
			}
			if (GameSettings.DynamicParticleLOD)
			{
				GameSettings.ParticleLOD = particleLOD;
			}
			if (this.particleScaleGrow > 0 && skillUseContext != null && skillUseContext.Originator)
			{
				SkillSlot skillSlot;
				skillUseContext.Originator.handle.SkillControl.TryGetSkillSlot(skillUseContext.SlotType, out skillSlot);
				if (skillSlot != null)
				{
					int num = skillSlot.GetSkillLevel();
					BaseSkill refParamObject = _action.refParams.GetRefParamObject<BaseSkill>("SkillObj");
					if (refParamObject != null)
					{
						BuffSkill buffSkill = (!refParamObject.isBuff) ? null : ((BuffSkill)refParamObject);
						if (buffSkill != null)
						{
							byte b = buffSkill.cfgData.bGrowthType;
							b %= 10;
							if (b > 0 && (SkillSlotType)b != skillUseContext.SlotType + 1)
							{
								SSkillFuncContext sSkillFuncContext = default(SSkillFuncContext);
								sSkillFuncContext.inOriginator = skillUseContext.Originator;
								sSkillFuncContext.inBuffSkill = new PoolObjHandle<BuffSkill>(buffSkill);
								sSkillFuncContext.inTargetObj = skillUseContext.TargetActor;
								sSkillFuncContext.inUseContext = skillUseContext;
								num = sSkillFuncContext.iSkillLevel;
							}
						}
					}
					this.particleSystem = this.particleObject.GetComponentsInChildren<ParticleSystem>(true);
					this.particleSystemSize = new float[this.particleSystem.Length];
					for (int i = 0; i < this.particleSystem.Length; i++)
					{
						this.particleSystemSize[i] = this.particleSystem[i].startSize;
					}
					for (int j = 0; j < this.particleSystem.Length; j++)
					{
						this.particleSystem[j].startSize = this.particleSystemSize[j] * ((float)((num - 1) * this.particleScaleGrow) / 10000f + 1f);
					}
					this.particleMeshRender = this.particleObject.GetComponentsInChildren<MeshRenderer>(true);
					this.particleMeshRenderScale = new Vector3[this.particleMeshRender.Length];
					for (int k = 0; k < this.particleMeshRender.Length; k++)
					{
						this.particleMeshRenderScale[k] = this.particleMeshRender[k].transform.localScale;
					}
					for (int l = 0; l < this.particleMeshRender.Length; l++)
					{
						this.particleMeshRender[l].transform.localScale = this.particleMeshRenderScale[l] * ((float)((num - 1) * this.particleScaleGrow) / 10000f + 1f);
					}
				}
			}
			ParticleHelper.IncParticleActiveNumber();
			if (transform != null)
			{
				if (!this.bOnlyFollowPos)
				{
					PoolObjHandle<ActorRoot> arg_80C_0 = (!(transform.gameObject == gameObject)) ? ActorHelper.GetActorRoot(transform.gameObject) : _action.GetActorHandle(this.targetId);
					this.particleTransform.parent = transform;
				}
				else
				{
					this.offsetPosition = vector - transform.position;
				}
			}
			if (flag)
			{
				if (this.enableLayer || this.enableTag)
				{
					Transform[] componentsInChildren = this.particleObject.GetComponentsInChildren<Transform>();
					for (int m = 0; m < componentsInChildren.Length; m++)
					{
						if (this.enableLayer)
						{
							componentsInChildren[m].gameObject.layer = this.layer;
						}
						if (this.enableTag)
						{
							componentsInChildren[m].gameObject.tag = this.tag;
						}
					}
				}
				ParticleSystem[] componentsInChildren2 = this.particleObject.GetComponentsInChildren<ParticleSystem>();
				if (componentsInChildren2 != null)
				{
					for (int n = 0; n < componentsInChildren2.Length; n++)
					{
						componentsInChildren2[n].startSize *= this.scaling.x;
						componentsInChildren2[n].startLifetime *= this.scaling.y;
						componentsInChildren2[n].startSpeed *= this.scaling.z;
						componentsInChildren2[n].transform.localScale *= this.scaling.x;
					}
					ParticleSystemPoolComponent cachedComponent = Singleton<CGameObjectPool>.GetInstance().GetCachedComponent<ParticleSystemPoolComponent>(this.particleObject, true);
					ParticleSystemPoolComponent.ParticleSystemCache[] array = new ParticleSystemPoolComponent.ParticleSystemCache[componentsInChildren2.Length];
					for (int num2 = 0; num2 < array.Length; num2++)
					{
						array[num2].par = componentsInChildren2[num2];
						array[num2].emmitState = componentsInChildren2[num2].enableEmission;
					}
					cachedComponent.cache = array;
				}
			}
			else
			{
				ParticleSystemPoolComponent cachedComponent2 = Singleton<CGameObjectPool>.GetInstance().GetCachedComponent<ParticleSystemPoolComponent>(this.particleObject, false);
				if (null != cachedComponent2)
				{
					ParticleSystemPoolComponent.ParticleSystemCache[] cache = cachedComponent2.cache;
					if (cache != null)
					{
						for (int num3 = 0; num3 < cache.Length; num3++)
						{
							if (cache[num3].par.enableEmission != cache[num3].emmitState)
							{
								cache[num3].par.enableEmission = cache[num3].emmitState;
							}
						}
					}
				}
			}
			string layerName = "Particles";
			if (transform && transform.gameObject.layer == LayerMask.NameToLayer("Hide"))
			{
				layerName = "Hide";
			}
			if (transform == null && transform2 != null && FogOfWar.enable)
			{
				PoolObjHandle<ActorRoot> actorRoot = ActorHelper.GetActorRoot(transform2.gameObject);
				if (actorRoot)
				{
					if (transform2.gameObject.layer == LayerMask.NameToLayer("Hide"))
					{
						layerName = "Hide";
					}
					PoolObjHandle<ActorRoot> actorHandle4 = _action.GetActorHandle(this.VirtualAttachBulletId);
					Singleton<GameFowManager>.instance.m_collector.AddVirtualParentParticle(this.particleObject, actorHandle4);
				}
			}
			this.particleObject.SetLayer(layerName, false);
			if (this.applyActionSpeedToParticle)
			{
				_action.AddTempObject(Action.PlaySpeedAffectedType.ePSAT_Fx, this.particleObject);
			}
		}

		public override void Process(Action _action, Track _track, int _localTime)
		{
			if (this.bOnlyFollowPos && this.particleTransform != null && this.followTransform != null && this.particleObject != null)
			{
				this.particleTransform.position = this.followTransform.position + this.offsetPosition;
				this.particleTransform.gameObject.SetVisibleSameAs(this.followTransform.gameObject);
			}
			base.Process(_action, _track, _localTime);
		}

		private static void OnDelayRecycleParticleCallback(GameObject recycleObj, float[] objSize, Vector3[] objScale)
		{
			recycleObj.transform.position = new Vector3(10000f, 10000f, 10000f);
			if (objSize != null && objSize.Length > 0)
			{
				ParticleSystem[] componentsInChildren = recycleObj.GetComponentsInChildren<ParticleSystem>(true);
				int num = 0;
				while (num < objSize.Length && num < componentsInChildren.Length)
				{
					componentsInChildren[num].startSize = objSize[num];
					num++;
				}
			}
			if (objScale != null && objScale.Length > 0)
			{
				MeshRenderer[] componentsInChildren2 = recycleObj.GetComponentsInChildren<MeshRenderer>(true);
				int num2 = 0;
				while (num2 < objScale.Length && num2 < componentsInChildren2.Length)
				{
					componentsInChildren2[num2].transform.localScale = objScale[num2];
					num2++;
				}
			}
			if (FogOfWar.enable)
			{
				Singleton<GameFowManager>.instance.m_collector.RemoveVirtualParentParticle(recycleObj);
			}
		}

		public override void Leave(Action _action, Track _track)
		{
			if (this.particleObject != null)
			{
				if (this.iDelayDisappearTime > 0)
				{
					ParticleSystemPoolComponent cachedComponent = Singleton<CGameObjectPool>.GetInstance().GetCachedComponent<ParticleSystemPoolComponent>(this.particleObject, false);
					if (null != cachedComponent)
					{
						ParticleSystemPoolComponent.ParticleSystemCache[] cache = cachedComponent.cache;
						if (cache != null)
						{
							for (int i = 0; i < cache.Length; i++)
							{
								cache[i].par.enableEmission = false;
							}
						}
						MonoSingleton<SceneMgr>.GetInstance().AddToRoot(this.particleObject, SceneObjType.ActionRes);
						Singleton<CGameObjectPool>.GetInstance().RecycleGameObjectDelay(this.particleObject, this.iDelayDisappearTime, new CGameObjectPool.OnDelayRecycleDelegate(TriggerParticle.OnDelayRecycleParticleCallback), this.particleSystemSize, null);
					}
				}
				else
				{
					if (FogOfWar.enable)
					{
						Singleton<GameFowManager>.instance.m_collector.RemoveVirtualParentParticle(this.particleObject);
					}
					if (this.particleSystemSize != null && this.particleSystem != null)
					{
						int num = 0;
						while (num < this.particleSystemSize.Length && num < this.particleSystem.Length)
						{
							this.particleSystem[num].startSize = this.particleSystemSize[num];
							num++;
						}
					}
					if (this.particleMeshRenderScale != null && this.particleMeshRender != null)
					{
						int num2 = 0;
						while (num2 < this.particleMeshRenderScale.Length && num2 < this.particleMeshRender.Length)
						{
							this.particleMeshRender[num2].transform.localScale = this.particleMeshRenderScale[num2];
							num2++;
						}
					}
					this.particleTransform.position = new Vector3(10000f, 10000f, 10000f);
					Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(this.particleObject);
				}
				ParticleHelper.DecParticleActiveNumber();
				if (this.applyActionSpeedToParticle)
				{
					_action.RemoveTempObject(Action.PlaySpeedAffectedType.ePSAT_Fx, this.particleObject);
				}
			}
		}
	}
}
