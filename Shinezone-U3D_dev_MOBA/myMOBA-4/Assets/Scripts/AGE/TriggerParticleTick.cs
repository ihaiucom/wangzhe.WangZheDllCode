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
	public class TriggerParticleTick : TickEvent
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

		public float lifeTime = 2f;

		[SubObject]
		public string bindPointName = string.Empty;

		public Vector3 bindPosOffset = new Vector3(0f, 0f, 0f);

		public Quaternion bindRotOffset = new Quaternion(0f, 0f, 0f, 1f);

		public Vector3 scaling = new Vector3(1f, 1f, 1f);

		public bool bEnableOptCull = true;

		public bool bBulletPos;

		public bool bBulletDir;

		public bool bBullerPosDir;

		public bool bUseIndicatorDir;

		public bool enableLayer;

		public int layer;

		public bool enableTag;

		public string tag = string.Empty;

		public bool enableMaxLimit;

		public int MaxLimit = 10;

		public int LimitType = -1;

		public bool applyActionSpeedToParticle = true;

		public bool bUseSkin;

		public bool bUseSkinAdvance;

		private GameObject particleObject;

		public int extend = 10;

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
			TriggerParticleTick triggerParticleTick = ClassObjPool<TriggerParticleTick>.Get();
			triggerParticleTick.CopyData(this);
			return triggerParticleTick;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			TriggerParticleTick triggerParticleTick = src as TriggerParticleTick;
			this.targetId = triggerParticleTick.targetId;
			this.objectSpaceId = triggerParticleTick.objectSpaceId;
			this.VirtualAttachBulletId = triggerParticleTick.VirtualAttachBulletId;
			this.resourceName = triggerParticleTick.resourceName;
			this.lifeTime = triggerParticleTick.lifeTime;
			this.bindPointName = triggerParticleTick.bindPointName;
			this.bindPosOffset = triggerParticleTick.bindPosOffset;
			this.bindRotOffset = triggerParticleTick.bindRotOffset;
			this.scaling = triggerParticleTick.scaling;
			this.bEnableOptCull = triggerParticleTick.bEnableOptCull;
			this.bBulletPos = triggerParticleTick.bBulletPos;
			this.bBulletDir = triggerParticleTick.bBulletDir;
			this.bBullerPosDir = triggerParticleTick.bBullerPosDir;
			this.bUseIndicatorDir = triggerParticleTick.bUseIndicatorDir;
			this.enableLayer = triggerParticleTick.enableLayer;
			this.layer = triggerParticleTick.layer;
			this.enableTag = triggerParticleTick.enableTag;
			this.tag = triggerParticleTick.tag;
			this.enableMaxLimit = triggerParticleTick.enableMaxLimit;
			this.MaxLimit = triggerParticleTick.MaxLimit;
			this.LimitType = triggerParticleTick.LimitType;
			this.applyActionSpeedToParticle = triggerParticleTick.applyActionSpeedToParticle;
			this.particleObject = triggerParticleTick.particleObject;
			this.extend = triggerParticleTick.extend;
			this.bUseSkin = triggerParticleTick.bUseSkin;
			this.bUseSkinAdvance = triggerParticleTick.bUseSkinAdvance;
			this.particleScaleGrow = triggerParticleTick.particleScaleGrow;
			this.particleSystem = triggerParticleTick.particleSystem;
			this.particleSystemSize = triggerParticleTick.particleSystemSize;
			this.particleMeshRender = triggerParticleTick.particleMeshRender;
			this.particleMeshRenderScale = triggerParticleTick.particleMeshRenderScale;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.targetId = 0;
			this.objectSpaceId = -1;
			this.VirtualAttachBulletId = -1;
			this.lifeTime = 5f;
			this.resourceName = string.Empty;
			this.bindPointName = string.Empty;
			this.bindPosOffset = new Vector3(0f, 0f, 0f);
			this.bindRotOffset = new Quaternion(0f, 0f, 0f, 1f);
			this.scaling = new Vector3(1f, 1f, 1f);
			this.bEnableOptCull = true;
			this.bBulletPos = false;
			this.bBulletDir = false;
			this.bBullerPosDir = false;
			this.bUseIndicatorDir = false;
			this.enableLayer = false;
			this.layer = 0;
			this.enableTag = false;
			this.tag = string.Empty;
			this.enableMaxLimit = false;
			this.MaxLimit = 10;
			this.LimitType = -1;
			this.applyActionSpeedToParticle = true;
			this.particleObject = null;
			this.extend = 10;
			this.bUseSkinAdvance = false;
			this.particleScaleGrow = 0;
			this.particleSystem = null;
			this.particleSystemSize = null;
			this.particleMeshRender = null;
			this.particleMeshRenderScale = null;
		}

		public override void Process(Action _action, Track _track)
		{
			if (MonoSingleton<Reconnection>.GetInstance().isProcessingRelayRecover)
			{
				return;
			}
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
				}
				else if (gameObject2 != null)
				{
					transform2 = gameObject2.transform;
				}
			}
			else
			{
				Transform transform3 = null;
				if (gameObject != null)
				{
					GameObject gameObject3 = SubObject.FindSubObject(gameObject, this.bindPointName);
					if (gameObject3 != null)
					{
						transform3 = gameObject3.transform;
					}
					if (transform3 != null)
					{
						transform = transform3;
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
						transform3 = gameObject3.transform;
					}
					if (transform3 != null)
					{
						transform2 = transform3;
					}
					else if (gameObject != null)
					{
						transform2 = gameObject2.transform;
					}
				}
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
					PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.objectSpaceId);
					if (actorHandle)
					{
						vector = (Vector3)IntMath.Transform((VInt3)this.bindPosOffset, actorHandle.handle.forward, actorHandle.handle.location);
						quaternion = Quaternion.LookRotation((Vector3)actorHandle.handle.forward) * this.bindRotOffset;
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
								PoolObjHandle<ActorRoot> actorHandle2 = _action.GetActorHandle(this.objectSpaceId);
								if (actorHandle2)
								{
									a = (Vector3)actorHandle2.handle.location;
								}
							}
							Vector3 forward = a - (Vector3)originator.handle.location;
							quaternion = Quaternion.LookRotation(forward);
							quaternion *= this.bindRotOffset;
						}
					}
				}
			}
			if (this.bUseIndicatorDir)
			{
				skillUseContext = _action.refParams.GetRefParamObject<SkillUseContext>("SkillContext");
				if (skillUseContext != null)
				{
					PoolObjHandle<ActorRoot> originator2 = skillUseContext.Originator;
					VInt3 ob;
					if (originator2 && skillUseContext.CalcAttackerDir(out ob, originator2))
					{
						quaternion = Quaternion.LookRotation((Vector3)ob);
						quaternion *= this.bindRotOffset;
					}
				}
			}
			if (this.bEnableOptCull && transform2 && transform2.gameObject.layer == LayerMask.NameToLayer("Hide") && !FogOfWar.enable)
			{
				return;
			}
			if (this.bEnableOptCull && MonoSingleton<GlobalConfig>.instance.bEnableParticleCullOptimize && !MonoSingleton<CameraSystem>.instance.CheckVisiblity(new Bounds(vector, new Vector3((float)this.extend, (float)this.extend, (float)this.extend))))
			{
				return;
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
			}
			if (GameSettings.DynamicParticleLOD)
			{
				GameSettings.ParticleLOD = particleLOD;
			}
			if (!this.particleObject)
			{
				return;
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
				PoolObjHandle<ActorRoot> ptr = (!(transform.gameObject == gameObject)) ? ActorHelper.GetActorRoot(transform.gameObject) : _action.GetActorHandle(this.targetId);
				if (ptr && ptr.handle.ActorMesh)
				{
					this.particleObject.transform.parent = ptr.handle.ActorMesh.transform;
				}
				else
				{
					this.particleObject.transform.parent = transform.parent;
					if (ptr && FogOfWar.enable && (Singleton<WatchController>.instance.IsWatching || !Singleton<WatchController>.instance.CoversCamp(ptr.handle.TheActorMeta.ActorCamp)))
					{
						if (ptr.handle.HorizonMarker != null)
						{
							ptr.handle.HorizonMarker.AddSubParObj(this.particleObject);
						}
						else
						{
							BulletWrapper bulletWrapper = ptr.handle.ActorControl as BulletWrapper;
							if (bulletWrapper != null)
							{
								bulletWrapper.AddSubParObj(this.particleObject);
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
					PoolObjHandle<ActorRoot> actorHandle3 = _action.GetActorHandle(this.VirtualAttachBulletId);
					Singleton<GameFowManager>.instance.m_collector.AddVirtualParentParticle(this.particleObject, actorHandle3);
				}
			}
			this.particleObject.SetLayer(layerName, false);
			if (flag)
			{
				ParticleHelper.Init(this.particleObject, this.scaling);
			}
			Singleton<CGameObjectPool>.GetInstance().RecycleGameObjectDelay(this.particleObject, Mathf.Max(_action.length, (int)(this.lifeTime * 1000f)), new CGameObjectPool.OnDelayRecycleDelegate(TriggerParticleTick.OnRecycleTickObj), this.particleSystemSize, this.particleMeshRenderScale);
			if (this.applyActionSpeedToParticle && this.particleObject != null)
			{
				_action.AddTempObject(Action.PlaySpeedAffectedType.ePSAT_Fx, this.particleObject);
			}
		}

		public static void OnRecycleTickObj(GameObject obj, float[] objSize, Vector3[] objScale)
		{
			if (objSize != null && objSize.Length > 0)
			{
				ParticleSystem[] componentsInChildren = obj.GetComponentsInChildren<ParticleSystem>(true);
				int num = 0;
				while (num < objSize.Length && num < componentsInChildren.Length)
				{
					componentsInChildren[num].startSize = objSize[num];
					num++;
				}
			}
			if (objScale != null && objScale.Length > 0)
			{
				MeshRenderer[] componentsInChildren2 = obj.GetComponentsInChildren<MeshRenderer>(true);
				int num2 = 0;
				while (num2 < objScale.Length && num2 < componentsInChildren2.Length)
				{
					componentsInChildren2[num2].transform.localScale = objScale[num2];
					num2++;
				}
			}
			ParticleHelper.DecParticleActiveNumber();
			if (FogOfWar.enable)
			{
				Singleton<GameFowManager>.instance.m_collector.RemoveVirtualParentParticle(obj);
			}
		}
	}
}
