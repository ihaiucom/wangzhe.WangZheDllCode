using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameKernal;
using System;
using UnityEngine;

namespace AGE
{
	public class CameraShakeDuration : DurationEvent
	{
		public bool useMainCamera;

		[ObjectTemplate(new Type[]
		{

		})]
		public int targetId = -1;

		public Vector3 shakeRange = Vector3.zero;

		private Vector3 originPos = Vector3.zero;

		private Vector3 shock = Vector3.zero;

		private float recovery = 0.1f;

		private bool enableFixedCam;

		private GameObject targetObject;

		private bool enterShaking;

		public bool filter_target;

		public bool filter_self;

		public bool filter_enemy;

		public bool filter_allies;

		public bool useAccumOffset;

		private Vector3 lastOffset = Vector3.zero;

		public static int shakeDistance = 15000;

		public override bool SupportEditMode()
		{
			return false;
		}

		public override BaseEvent Clone()
		{
			CameraShakeDuration cameraShakeDuration = ClassObjPool<CameraShakeDuration>.Get();
			cameraShakeDuration.CopyData(this);
			return cameraShakeDuration;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			CameraShakeDuration cameraShakeDuration = src as CameraShakeDuration;
			this.useMainCamera = cameraShakeDuration.useMainCamera;
			this.targetId = cameraShakeDuration.targetId;
			this.shakeRange = cameraShakeDuration.shakeRange;
			this.originPos = cameraShakeDuration.originPos;
			this.shock = cameraShakeDuration.shock;
			this.recovery = cameraShakeDuration.recovery;
			this.enableFixedCam = cameraShakeDuration.enableFixedCam;
			this.targetObject = cameraShakeDuration.targetObject;
			this.enterShaking = cameraShakeDuration.enterShaking;
			this.filter_target = cameraShakeDuration.filter_target;
			this.filter_self = cameraShakeDuration.filter_self;
			this.filter_enemy = cameraShakeDuration.filter_enemy;
			this.filter_allies = cameraShakeDuration.filter_allies;
			this.useAccumOffset = cameraShakeDuration.useAccumOffset;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.useMainCamera = false;
			this.targetId = -1;
			this.shakeRange = Vector3.zero;
			this.originPos = Vector3.zero;
			this.shock = Vector3.zero;
			this.recovery = 0.1f;
			this.enableFixedCam = false;
			this.targetObject = null;
			this.enterShaking = false;
			this.filter_target = false;
			this.filter_self = false;
			this.filter_enemy = false;
			this.filter_allies = false;
			this.useAccumOffset = false;
			this.lastOffset = Vector3.zero;
		}

		public bool CheckShakeDistance(ActorRoot captain, ActorRoot user)
		{
			if (captain == null || user == null)
			{
				return false;
			}
			if (captain == user)
			{
				return true;
			}
			VInt3 vInt = captain.location - user.location;
			long num = (long)CameraShakeDuration.shakeDistance;
			num *= num;
			return vInt.sqrMagnitudeLong2D <= num;
		}

		public bool ShouldShake(Action _action)
		{
			BaseSkill refParamObject = _action.refParams.GetRefParamObject<BaseSkill>("SkillObj");
			SkillUseContext refParamObject2 = _action.refParams.GetRefParamObject<SkillUseContext>("SkillContext");
			if (refParamObject == null || refParamObject2 == null)
			{
				return true;
			}
			PoolObjHandle<ActorRoot> originator = refParamObject2.Originator;
			if (ActorHelper.IsHostCtrlActor(ref originator) && this.filter_self)
			{
				return true;
			}
			Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
			if (this.filter_target && hostPlayer != null && refParamObject2.TargetActor == hostPlayer.Captain)
			{
				return true;
			}
			if (hostPlayer != null)
			{
				Player player = Singleton<GamePlayerCenter>.instance.GetPlayer(originator.handle.TheActorMeta.PlayerId);
				if (player != null)
				{
					if (this.filter_enemy && player.PlayerCamp != hostPlayer.PlayerCamp)
					{
						return this.CheckShakeDistance(hostPlayer.Captain.handle, originator.handle);
					}
					if (this.filter_allies && player.PlayerCamp == hostPlayer.PlayerCamp)
					{
						return this.CheckShakeDistance(hostPlayer.Captain.handle, originator.handle);
					}
				}
				else if (this.filter_enemy)
				{
					return this.CheckShakeDistance(hostPlayer.Captain.handle, originator.handle);
				}
			}
			return false;
		}

		public override void Enter(Action _action, Track _track)
		{
			if (Singleton<BattleLogic>.GetInstance().IsModifyingCamera)
			{
				return;
			}
			if (!this.ShouldShake(_action))
			{
				return;
			}
			if (this.useMainCamera && Camera.main)
			{
				this.targetObject = Camera.main.gameObject;
			}
			else
			{
				this.targetObject = _action.GetGameObject(this.targetId);
			}
			if (this.targetObject == null || this.targetObject.transform == null)
			{
				if (ActionManager.Instance.isPrintLog)
				{
				}
				return;
			}
			Singleton<BattleLogic>.GetInstance().IsModifyingCamera = true;
			this.enterShaking = true;
			this.originPos = this.targetObject.transform.localPosition;
			this.shock = this.shakeRange;
		}

		public override void Process(Action _action, Track _track, int _localTime)
		{
			if (!this.enterShaking)
			{
				return;
			}
			if (this.useMainCamera && Camera.main)
			{
				this.targetObject = Camera.main.gameObject;
			}
			else
			{
				this.targetObject = _action.GetGameObject(this.targetId);
			}
			if (this.targetObject == null || this.targetObject.transform == null)
			{
				if (ActionManager.Instance.isPrintLog)
				{
				}
				return;
			}
			Vector3 a = new Vector3(Random.Range(-this.shock.x, this.shock.x), Random.Range(-this.shock.y, this.shock.y), Random.Range(-this.shock.z, this.shock.z));
			if (this.useAccumOffset)
			{
				this.targetObject.transform.localPosition += a - this.lastOffset;
				this.lastOffset = a;
			}
			else
			{
				this.targetObject.transform.localPosition = a + this.originPos;
			}
			this.shock *= 1f - this.recovery;
		}

		public override void Leave(Action _action, Track _track)
		{
			if (!this.enterShaking)
			{
				return;
			}
			if (this.useMainCamera && Camera.main)
			{
				this.targetObject = Camera.main.gameObject;
			}
			else
			{
				this.targetObject = _action.GetGameObject(this.targetId);
			}
			if (this.targetObject == null || this.targetObject.transform == null)
			{
				this.enterShaking = false;
				if (ActionManager.Instance.isPrintLog)
				{
				}
				return;
			}
			this.shock = Vector3.zero;
			if (this.useAccumOffset)
			{
				this.targetObject.transform.localPosition -= this.lastOffset;
			}
			else
			{
				this.targetObject.transform.localPosition = this.originPos;
			}
			Singleton<BattleLogic>.GetInstance().IsModifyingCamera = false;
			this.enterShaking = false;
		}
	}
}
