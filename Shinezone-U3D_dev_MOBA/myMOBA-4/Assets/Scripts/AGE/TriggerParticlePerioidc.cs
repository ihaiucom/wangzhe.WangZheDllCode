using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AGE
{
	[EventCategory("Effect")]
	public class TriggerParticlePerioidc : TriggerParticle
	{
		[AssetReference(AssetRefType.Particle)]
		public string InitialEffectName;

		[AssetReference(AssetRefType.Particle)]
		public string PeriodicEffectName;

		[AssetReference(AssetRefType.Particle)]
		public string FinalEffectName;

		public bool bAutoDestruct = true;

		public int PeriodicInterval = 1000;

		private int intervalTimer;

		private int lastTime;

		private List<GameObject> NonAutoDestructParList = new List<GameObject>();

		public override void OnUse()
		{
			base.OnUse();
			this.InitialEffectName = string.Empty;
			this.PeriodicEffectName = string.Empty;
			this.FinalEffectName = string.Empty;
			this.bAutoDestruct = true;
			this.PeriodicInterval = 1000;
			this.intervalTimer = 0;
			this.lastTime = 0;
			this.NonAutoDestructParList.Clear();
		}

		public override BaseEvent Clone()
		{
			TriggerParticlePerioidc triggerParticlePerioidc = ClassObjPool<TriggerParticlePerioidc>.Get();
			triggerParticlePerioidc.CopyData(this);
			return triggerParticlePerioidc;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			TriggerParticlePerioidc triggerParticlePerioidc = src as TriggerParticlePerioidc;
			this.InitialEffectName = triggerParticlePerioidc.InitialEffectName;
			this.PeriodicEffectName = triggerParticlePerioidc.PeriodicEffectName;
			this.FinalEffectName = triggerParticlePerioidc.FinalEffectName;
			this.bAutoDestruct = triggerParticlePerioidc.bAutoDestruct;
			this.PeriodicInterval = triggerParticlePerioidc.PeriodicInterval;
			this.intervalTimer = triggerParticlePerioidc.intervalTimer;
			this.lastTime = triggerParticlePerioidc.lastTime;
			this.NonAutoDestructParList = triggerParticlePerioidc.NonAutoDestructParList;
		}

		private GameObject InstParObj(string prefabName, Action _action, bool bCheckParLife)
		{
			Vector3 pos = this.bindPosOffset;
			Quaternion rot = this.bindRotOffset;
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
			else if (gameObject != null)
			{
				Transform transform3 = SubObject.FindSubObject(gameObject, this.bindPointName).transform;
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
				Transform transform3 = SubObject.FindSubObject(gameObject2, this.bindPointName).transform;
				if (transform3 != null)
				{
					transform2 = transform3;
				}
				else if (gameObject != null)
				{
					transform2 = gameObject2.transform;
				}
			}
			if (transform != null)
			{
				pos = transform.localToWorldMatrix.MultiplyPoint(this.bindPosOffset);
				rot = transform.rotation * this.bindRotOffset;
			}
			else if (transform2 != null)
			{
				pos = transform2.localToWorldMatrix.MultiplyPoint(this.bindPosOffset);
				rot = transform2.rotation * this.bindRotOffset;
			}
			if (transform2 && transform2.gameObject.layer == LayerMask.NameToLayer("Hide"))
			{
				return null;
			}
			bool flag = false;
			GameObject pooledGameObjLOD = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD(prefabName, true, SceneObjType.ActionRes, pos, rot, out flag);
			if (pooledGameObjLOD == null)
			{
				return null;
			}
			if (transform != null)
			{
				PoolObjHandle<ActorRoot> ptr = (!(transform.gameObject == gameObject)) ? ActorHelper.GetActorRoot(transform.gameObject) : _action.GetActorHandle(this.targetId);
				if (ptr && ptr.handle.ActorMesh)
				{
					pooledGameObjLOD.transform.SetParent(ptr.handle.ActorMesh.transform);
				}
				else
				{
					pooledGameObjLOD.transform.SetParent(transform);
				}
			}
			string layerName = "Particles";
			if (transform && transform.gameObject.layer == LayerMask.NameToLayer("Hide"))
			{
				layerName = "Hide";
			}
			this.particleObject.SetLayer(layerName, false);
			if (!bCheckParLife)
			{
				this.NonAutoDestructParList.Add(pooledGameObjLOD);
			}
			if (flag)
			{
				ParticleHelper.Init(this.particleObject, this.scaling);
			}
			if (this.applyActionSpeedToParticle)
			{
				_action.AddTempObject(Action.PlaySpeedAffectedType.ePSAT_Fx, pooledGameObjLOD);
			}
			return pooledGameObjLOD;
		}

		private void DestroyParObj(Action _action, GameObject inParObj)
		{
			if (inParObj == null)
			{
				return;
			}
			inParObj.transform.SetParent(null);
			ActionManager.DestroyGameObjectFromAction(_action, inParObj);
			if (this.applyActionSpeedToParticle)
			{
				_action.RemoveTempObject(Action.PlaySpeedAffectedType.ePSAT_Fx, inParObj);
			}
		}

		public override void Enter(Action _action, Track _track)
		{
			this.resourceName = this.InitialEffectName;
			base.Enter(_action, _track);
			if (this.PeriodicInterval > 0)
			{
				this.lastTime = 0;
				this.intervalTimer = 0;
			}
		}

		public override void Leave(Action _action, Track _track)
		{
			base.Leave(_action, _track);
			if (this.FinalEffectName.Length > 0)
			{
				this.InstParObj(this.FinalEffectName, _action, true);
			}
			List<GameObject>.Enumerator enumerator = this.NonAutoDestructParList.GetEnumerator();
			while (enumerator.MoveNext())
			{
				this.DestroyParObj(_action, enumerator.Current);
			}
			this.NonAutoDestructParList.Clear();
		}

		public override void Process(Action _action, Track _track, int _localTime)
		{
			if (this.PeriodicInterval <= 0)
			{
				return;
			}
			int num = _localTime - this.lastTime;
			this.lastTime = _localTime;
			this.intervalTimer += num;
			if (this.intervalTimer >= this.PeriodicInterval)
			{
				this.intervalTimer = 0;
				if (this.PeriodicEffectName.Length > 0)
				{
					this.InstParObj(this.PeriodicEffectName, _action, this.bAutoDestruct);
				}
			}
		}
	}
}
