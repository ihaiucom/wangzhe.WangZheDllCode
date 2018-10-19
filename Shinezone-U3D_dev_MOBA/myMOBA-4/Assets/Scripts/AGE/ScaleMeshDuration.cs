using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;
using UnityEngine;

namespace AGE
{
	[EventCategory("MMGame/Skill")]
	public class ScaleMeshDuration : DurationEvent
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int targetId;

		public int scaleRate = 10000;

		private Vector3 originalScale;

		private int originalRadius;

		private VInt3 originalSize;

		private PoolObjHandle<ActorRoot> actorObj;

		public override bool SupportEditMode()
		{
			return true;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.targetId = 0;
			this.originalScale = Vector3.one;
			this.originalRadius = 1;
			this.originalSize = VInt3.one;
			this.scaleRate = 10000;
			this.actorObj.Release();
		}

		public override BaseEvent Clone()
		{
			ScaleMeshDuration scaleMeshDuration = ClassObjPool<ScaleMeshDuration>.Get();
			scaleMeshDuration.CopyData(this);
			return scaleMeshDuration;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			ScaleMeshDuration scaleMeshDuration = src as ScaleMeshDuration;
			this.targetId = scaleMeshDuration.targetId;
			this.scaleRate = scaleMeshDuration.scaleRate;
		}

		private void SetMeshScale(int _scaleRate)
		{
			if (this.actorObj && this.actorObj.handle.ActorMesh != null)
			{
				float num = (float)_scaleRate / 10000f;
				Vector3 localScale = new Vector3(num, num, num);
				this.originalScale = this.actorObj.handle.ActorMesh.transform.localScale;
				this.actorObj.handle.ActorMesh.transform.localScale = localScale;
			}
		}

		private void RecorveMeshScale()
		{
			if (this.actorObj && this.actorObj.handle.ActorMesh != null)
			{
				this.actorObj.handle.ActorMesh.transform.localScale = this.originalScale;
			}
		}

		private void SetCollisionScale(int _scaleRate)
		{
			if (this.actorObj)
			{
				VFactor vFactor = new VFactor((long)this.scaleRate, 10000L);
				VCollisionShape shape = this.actorObj.handle.shape;
				if (shape != null)
				{
					int roundInt = vFactor.roundInt;
					if (shape is VCollisionSphere)
					{
						VCollisionSphere vCollisionSphere = shape as VCollisionSphere;
						if (vCollisionSphere != null)
						{
							this.originalRadius = vCollisionSphere.Radius;
							vCollisionSphere.Radius *= roundInt;
						}
					}
					else if (shape is VCollisionBox)
					{
						VCollisionBox vCollisionBox = shape as VCollisionBox;
						if (vCollisionBox != null)
						{
							this.originalSize = vCollisionBox.Size;
							VInt3 size;
							size.x = vCollisionBox.Size.x * roundInt;
							size.y = vCollisionBox.Size.y * roundInt;
							size.z = vCollisionBox.Size.z * roundInt;
							vCollisionBox.Size = size;
						}
					}
				}
			}
		}

		private void RecorveCollisionScale()
		{
			if (this.actorObj)
			{
				VCollisionShape shape = this.actorObj.handle.shape;
				if (shape != null)
				{
					if (shape is VCollisionSphere)
					{
						VCollisionSphere vCollisionSphere = shape as VCollisionSphere;
						if (vCollisionSphere != null)
						{
							vCollisionSphere.Radius = this.originalRadius;
						}
					}
					else if (shape is VCollisionBox)
					{
						VCollisionBox vCollisionBox = shape as VCollisionBox;
						if (vCollisionBox != null)
						{
							vCollisionBox.Size = this.originalSize;
						}
					}
				}
			}
		}

		public override void Enter(Action _action, Track _track)
		{
			base.Enter(_action, _track);
			this.actorObj = _action.GetActorHandle(this.targetId);
			this.SetMeshScale(this.scaleRate);
			this.SetCollisionScale(this.scaleRate);
		}

		public override void Leave(Action _action, Track _track)
		{
			this.RecorveMeshScale();
			this.RecorveCollisionScale();
			base.Leave(_action, _track);
		}

		public override void Process(Action _action, Track _track, int _localTime)
		{
			base.Process(_action, _track, _localTime);
		}
	}
}
