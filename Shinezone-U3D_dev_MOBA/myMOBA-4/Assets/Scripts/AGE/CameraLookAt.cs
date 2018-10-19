using Assets.Scripts.Common;
using System;
using UnityEngine;

namespace AGE
{
	[EventCategory("Movement")]
	public class CameraLookAt : TickEvent
	{
		public enum EUpDirType
		{
			NoOverrideUp,
			RowAngleByZ
		}

		public Vector3 worldOffset = Vector3.zero;

		public Vector3 localOffset = Vector3.zero;

		public bool overrideUpDir = true;

		public Vector3 upDir = Vector3.up;

		public CameraLookAt.EUpDirType UpDirType;

		public float rowAngleByZ;

		[ObjectTemplate(new Type[]
		{

		})]
		public int cameraId;

		[ObjectTemplate(new Type[]
		{

		})]
		public int targetId = -1;

		public override BaseEvent Clone()
		{
			CameraLookAt cameraLookAt = ClassObjPool<CameraLookAt>.Get();
			cameraLookAt.CopyData(this);
			return cameraLookAt;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			CameraLookAt cameraLookAt = src as CameraLookAt;
			this.worldOffset = cameraLookAt.worldOffset;
			this.localOffset = cameraLookAt.localOffset;
			this.overrideUpDir = cameraLookAt.overrideUpDir;
			this.upDir = cameraLookAt.upDir;
			this.UpDirType = cameraLookAt.UpDirType;
			this.rowAngleByZ = cameraLookAt.rowAngleByZ;
			this.cameraId = cameraLookAt.cameraId;
			this.targetId = cameraLookAt.targetId;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.worldOffset = Vector3.zero;
			this.localOffset = Vector3.zero;
			this.overrideUpDir = true;
			this.upDir = Vector3.up;
			this.UpDirType = CameraLookAt.EUpDirType.NoOverrideUp;
			this.rowAngleByZ = 0f;
			this.cameraId = 0;
			this.targetId = -1;
		}

		public override bool SupportEditMode()
		{
			return true;
		}

		public override void Process(Action _action, Track _track)
		{
			if (_action.GetGameObject(this.cameraId) == null)
			{
				return;
			}
			_action.GetGameObject(this.cameraId).transform.rotation = this.GetLookRotation(_action);
		}

		public override void ProcessBlend(Action _action, Track _track, TickEvent _prevEvent, float _blendWeight)
		{
			if (_action.GetGameObject(this.cameraId) == null || _prevEvent == null)
			{
				return;
			}
			_action.GetGameObject(this.cameraId).transform.rotation = Quaternion.Slerp((_prevEvent as CameraLookAt).GetLookRotation(_action), this.GetLookRotation(_action), _blendWeight);
		}

		private Quaternion GetLookRotation(Action _action)
		{
			GameObject gameObject = _action.GetGameObject(this.cameraId);
			if (gameObject == null)
			{
				return Quaternion.identity;
			}
			GameObject gameObject2 = _action.GetGameObject(this.targetId);
			Vector3 a = new Vector3(0f, 0f, 0f);
			if (gameObject2 == null)
			{
				a = this.localOffset + this.worldOffset;
			}
			else
			{
				a = gameObject2.transform.position + gameObject2.transform.TransformDirection(this.localOffset) + this.worldOffset;
			}
			Vector3 forward = a - gameObject.transform.position;
			if (this.UpDirType == CameraLookAt.EUpDirType.NoOverrideUp)
			{
				return Quaternion.LookRotation(forward, gameObject.transform.up);
			}
			Quaternion rhs = Quaternion.AngleAxis(this.rowAngleByZ, Vector3.forward);
			Quaternion lhs = Quaternion.LookRotation(forward, Vector3.up);
			return lhs * rhs;
		}
	}
}
