using Assets.Scripts.Common;
using System;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public abstract class BasePet
	{
		private bool bActive;

		private string curAnimName;

		protected int deltaTime;

		protected Vector3 moveDir;

		protected float moveSpeed;

		protected PetState curState;

		protected Vector3 offset;

		protected float offsetDistance;

		protected GameObject meshObj;

		protected Animation animSet;

		protected GameObject parentObj;

		protected Transform petTrans;

		protected Transform parentTrans;

		protected PoolObjHandle<ActorRoot> actorPtr;

		private bool IsEquals(BasePet rhs)
		{
			return this.bActive == rhs.bActive && this.curAnimName == rhs.curAnimName && this.deltaTime == rhs.deltaTime && this.moveDir == rhs.moveDir && this.moveSpeed == rhs.moveSpeed && this.curState == rhs.curState && this.offsetDistance == rhs.offsetDistance && this.offset == rhs.offset && this.meshObj == rhs.meshObj && this.animSet == rhs.animSet && this.parentObj == rhs.parentObj && this.actorPtr == rhs.actorPtr;
		}

		public override bool Equals(object obj)
		{
			return obj != null && base.GetType() == obj.GetType() && this.IsEquals((BasePet)obj);
		}

		// bsh: override GetHashCode to avoid warning
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public virtual void OnUse()
		{
			this.bActive = false;
			this.curAnimName = null;
			this.deltaTime = 0;
			this.moveDir = Vector3.zero;
			this.moveSpeed = 0f;
			this.curState = PetState.Idle;
			this.offset = Vector3.zero;
			this.offsetDistance = 0f;
			this.meshObj = null;
			this.animSet = null;
			this.parentObj = null;
			this.actorPtr.Release();
		}

		public virtual void Init(ref PoolObjHandle<ActorRoot> _actor)
		{
			this.actorPtr = _actor;
			if (this.actorPtr)
			{
				this.parentObj = this.actorPtr.handle.gameObject;
			}
		}

		public virtual void Create(string _prefabName, Vector3 _offset)
		{
			if (!this.actorPtr || this.parentObj == null || this.parentObj.transform == null)
			{
				return;
			}
			this.offset = _offset;
			this.offsetDistance = Vector3.Distance(new Vector3(0f, 0f, 0f), this.offset);
			this.deltaTime = 0;
			Vector3 pos = this.parentObj.transform.localToWorldMatrix.MultiplyPoint(this.offset);
			this.meshObj = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD(_prefabName, false, SceneObjType.ActionRes, pos, this.parentObj.transform.rotation);
			if (this.meshObj != null)
			{
				this.animSet = this.meshObj.GetComponent<Animation>();
				if (this.animSet != null)
				{
					this.animSet.Play("Idle");
				}
				this.petTrans = this.meshObj.transform;
				this.parentTrans = this.parentObj.transform;
				this.bActive = true;
				this.curState = PetState.Idle;
				return;
			}
			this.bActive = false;
		}

		public virtual void Destory()
		{
			if (this.bActive && this.meshObj != null)
			{
				Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(this.meshObj);
				this.meshObj = null;
				this.bActive = false;
			}
		}

		protected void PlayAnimation(string animName, float blendTime)
		{
			if (this.curAnimName != animName)
			{
				AnimationState animationState = this.animSet.CrossFadeQueued(animName, blendTime, QueueMode.PlayNow);
				this.curAnimName = animName;
			}
		}

		protected Quaternion ObjRotationLerp(Vector3 _moveDir, int nDelta)
		{
			return Quaternion.RotateTowards(this.meshObj.transform.rotation, Quaternion.LookRotation(_moveDir), (float)nDelta);
		}

		protected bool CheckUpdate()
		{
			return this.bActive && !(this.meshObj == null);
		}

		public virtual void LateUpdate(int nDelta)
		{
		}
	}
}
