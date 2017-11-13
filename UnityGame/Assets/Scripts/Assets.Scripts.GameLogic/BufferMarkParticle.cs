using Assets.Scripts.Common;
using ResData;
using System;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	internal struct BufferMarkParticle
	{
		private string layerEffectName1;

		private string layerEffectName2;

		private string layerEffectName3;

		private string layerEffectName4;

		private string layerEffectName5;

		private GameObject particleObj;

		public void Init(ResSkillMarkCfgInfo _cfg)
		{
			if (_cfg == null)
			{
				return;
			}
			this.layerEffectName1 = StringHelper.UTF8BytesToString(ref _cfg.szLayerEffectName1);
			this.layerEffectName2 = StringHelper.UTF8BytesToString(ref _cfg.szLayerEffectName2);
			this.layerEffectName3 = StringHelper.UTF8BytesToString(ref _cfg.szLayerEffectName3);
			this.layerEffectName4 = StringHelper.UTF8BytesToString(ref _cfg.szLayerEffectName4);
			this.layerEffectName5 = StringHelper.UTF8BytesToString(ref _cfg.szLayerEffectName5);
		}

		public void PlayParticle(ref PoolObjHandle<ActorRoot> _origin, ref PoolObjHandle<ActorRoot> _target, int _layer)
		{
			this.RemoveParticle();
			if (_layer == 1 && !string.IsNullOrEmpty(this.layerEffectName1))
			{
				this.CreateParticle(this.layerEffectName1, ref _origin, ref _target);
			}
			else if (_layer == 2 && !string.IsNullOrEmpty(this.layerEffectName2))
			{
				this.CreateParticle(this.layerEffectName2, ref _origin, ref _target);
			}
			else if (_layer == 3 && !string.IsNullOrEmpty(this.layerEffectName3))
			{
				this.CreateParticle(this.layerEffectName3, ref _origin, ref _target);
			}
			else if (_layer == 4 && !string.IsNullOrEmpty(this.layerEffectName4))
			{
				this.CreateParticle(this.layerEffectName4, ref _origin, ref _target);
			}
			else if (_layer == 5 && !string.IsNullOrEmpty(this.layerEffectName5))
			{
				this.CreateParticle(this.layerEffectName5, ref _origin, ref _target);
			}
		}

		private void CreateParticle(string _resName, ref PoolObjHandle<ActorRoot> _origin, ref PoolObjHandle<ActorRoot> _target)
		{
			if (!_target)
			{
				return;
			}
			string resourceName = SkinResourceHelper.GetResourceName(ref _origin, _resName, true);
			this.particleObj = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD(resourceName, true, SceneObjType.ActionRes, _target.handle.myTransform.position);
			if (this.particleObj == null)
			{
				this.particleObj = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD(_resName, true, SceneObjType.ActionRes, _target.handle.myTransform.position);
			}
			if (this.particleObj != null)
			{
				string layerName = "Particles";
				if (_target.handle.gameObject.layer == LayerMask.NameToLayer("Hide"))
				{
					layerName = "Hide";
				}
				this.particleObj.SetLayer(layerName, false);
				this.particleObj.transform.SetParent(_target.handle.myTransform);
				ParticleSystem component = this.particleObj.GetComponent<ParticleSystem>();
				if (component != null)
				{
					component.Play(true);
				}
			}
		}

		private void RemoveParticle()
		{
			if (this.particleObj != null)
			{
				this.particleObj.transform.position = new Vector3(10000f, 10000f, 10000f);
				Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(this.particleObj);
				this.particleObj = null;
			}
		}
	}
}
