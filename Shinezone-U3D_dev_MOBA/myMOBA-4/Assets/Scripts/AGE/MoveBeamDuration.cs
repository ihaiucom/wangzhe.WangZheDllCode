using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;
using UnityEngine;

namespace AGE
{
	[EventCategory("MMGame/Skill")]
	public class MoveBeamDuration : DurationCondition
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int sourceId;

		[ObjectTemplate(new Type[]
		{

		})]
		public int targetId;

		[AssetReference(AssetRefType.Particle)]
		public string resourceName = string.Empty;

		public VInt3 bindPosOffset = new VInt3(0, 0, 0);

		public VInt3 bindDestOffet = new VInt3(0, 0, 0);

		public int textureScale = 5;

		public float beamWidth = 1f;

		[SubObject]
		public string bindPointName = string.Empty;

		private PoolObjHandle<ActorRoot> srcActor;

		private PoolObjHandle<ActorRoot> targetActor;

		private bool bInit;

		private bool bDone;

		private GameObject beamObject;

		private LineRenderer lineRenderer;

		public override BaseEvent Clone()
		{
			MoveBeamDuration moveBeamDuration = ClassObjPool<MoveBeamDuration>.Get();
			moveBeamDuration.CopyData(this);
			return moveBeamDuration;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			MoveBeamDuration moveBeamDuration = src as MoveBeamDuration;
			this.sourceId = moveBeamDuration.sourceId;
			this.targetId = moveBeamDuration.targetId;
			this.bindPosOffset = moveBeamDuration.bindPosOffset;
			this.bindDestOffet = moveBeamDuration.bindDestOffet;
			this.resourceName = moveBeamDuration.resourceName;
			this.beamWidth = moveBeamDuration.beamWidth;
			this.textureScale = moveBeamDuration.textureScale;
			this.bindPointName = moveBeamDuration.bindPointName;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.srcActor.Release();
			this.targetActor.Release();
			this.bInit = false;
			this.bDone = false;
			this.beamObject = null;
			this.lineRenderer = null;
			this.bindPointName = string.Empty;
		}

		private Vector3 GetSrcPosition()
		{
			Vector3 result = Vector3.zero;
			if (this.bindPointName.Length == 0)
			{
				result = (Vector3)IntMath.Transform(this.bindPosOffset, this.srcActor.handle.forward, this.srcActor.handle.location);
			}
			else
			{
				GameObject gameObject = SubObject.FindSubObject(this.srcActor.handle.gameObject, this.bindPointName);
				if (gameObject != null)
				{
					result = gameObject.transform.position;
				}
				else
				{
					result = (Vector3)IntMath.Transform(this.bindPosOffset, this.srcActor.handle.forward, this.srcActor.handle.location);
				}
			}
			return result;
		}

		private bool Init(Action _action)
		{
			this.srcActor = _action.GetActorHandle(this.sourceId);
			this.targetActor = _action.GetActorHandle(this.targetId);
			if (!this.srcActor || !this.targetActor)
			{
				return false;
			}
			Vector3 srcPosition = this.GetSrcPosition();
			Quaternion rot = Quaternion.LookRotation((Vector3)this.targetActor.handle.forward);
			bool flag = false;
			this.beamObject = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD(this.resourceName, true, SceneObjType.ActionRes, srcPosition, rot, out flag);
			if (this.beamObject == null)
			{
				return false;
			}
			this.lineRenderer = this.beamObject.gameObject.GetComponentInChildren<LineRenderer>();
			if (this.lineRenderer == null)
			{
				return false;
			}
			this.lineRenderer.SetWidth(this.beamWidth, this.beamWidth);
			return true;
		}

		private void UnInit()
		{
			this.HideBeam();
			if (this.beamObject != null)
			{
				this.beamObject.transform.position = new Vector3(10000f, 10000f, 10000f);
				Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(this.beamObject);
			}
		}

		public override void Enter(Action _action, Track _track)
		{
			this.bInit = this.Init(_action);
			base.Enter(_action, _track);
		}

		private void HideBeam()
		{
			if (this.lineRenderer != null)
			{
				this.lineRenderer.SetVertexCount(0);
			}
		}

		private void RenderBeam()
		{
			Vector3 srcPosition = this.GetSrcPosition();
			Vector3 position = (Vector3)IntMath.Transform(this.bindDestOffet, this.targetActor.handle.forward, (VInt3)this.targetActor.handle.myTransform.position);
			this.lineRenderer.SetVertexCount(2);
			this.lineRenderer.SetPosition(0, srcPosition);
			this.lineRenderer.SetPosition(1, position);
		}

		private void SetBeamLength(int _length)
		{
			this.SetTextureScale(_length * this.textureScale / 1000, 1);
		}

		private void SetTextureScale(int _tileX, int _tileY)
		{
			Renderer component = this.beamObject.GetComponent<Renderer>();
			if (component != null && component.material != null)
			{
				component.material.SetTextureScale("_MainTex", new Vector2((float)_tileX, (float)_tileY));
			}
		}

		public override void Process(Action _action, Track _track, int _localTime)
		{
			base.Process(_action, _track, _localTime);
			if (this.bInit || !this.bDone)
			{
				if (!this.srcActor || !this.targetActor || this.srcActor.handle.ActorControl.IsDeadState || this.targetActor.handle.ActorControl.IsDeadState || (!this.targetActor.handle.Visible && !this.srcActor.handle.Visible))
				{
					this.bDone = true;
					this.HideBeam();
					return;
				}
				VInt3 location = this.srcActor.handle.location;
				VInt3 location2 = this.targetActor.handle.location;
				int magnitude2D = (location2 - location).magnitude2D;
				if (magnitude2D > 0)
				{
					this.SetBeamLength(magnitude2D);
					this.RenderBeam();
				}
			}
		}

		public override void Leave(Action _action, Track _track)
		{
			this.UnInit();
			base.Leave(_action, _track);
		}
	}
}
