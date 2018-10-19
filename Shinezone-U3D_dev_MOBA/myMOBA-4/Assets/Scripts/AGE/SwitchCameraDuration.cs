using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;
using UnityEngine;

namespace AGE
{
	[EventCategory("MMGame/System")]
	public class SwitchCameraDuration : DurationCondition
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int cameraId = -1;

		[ObjectTemplate(new Type[]
		{

		})]
		public int cameraId2 = -1;

		public int slerpTick = 3000;

		public bool cutBackOnExit;

		private Vector3 startPos = Vector3.zero;

		private Quaternion startRot = Quaternion.identity;

		private Vector3 destPos = Vector3.zero;

		private Quaternion destRot = Quaternion.identity;

		private Camera curCamera;

		private Camera oldCamera;

		private bool switchFinished;

		private bool isMoba_camera;

		public override bool SupportEditMode()
		{
			return true;
		}

		public override BaseEvent Clone()
		{
			SwitchCameraDuration switchCameraDuration = ClassObjPool<SwitchCameraDuration>.Get();
			switchCameraDuration.CopyData(this);
			return switchCameraDuration;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			SwitchCameraDuration switchCameraDuration = src as SwitchCameraDuration;
			this.cameraId = switchCameraDuration.cameraId;
			this.cameraId2 = switchCameraDuration.cameraId2;
			this.slerpTick = switchCameraDuration.slerpTick;
			this.cutBackOnExit = switchCameraDuration.cutBackOnExit;
			this.startPos = switchCameraDuration.startPos;
			this.startRot = switchCameraDuration.startRot;
			this.destPos = switchCameraDuration.destPos;
			this.destRot = switchCameraDuration.destRot;
			this.curCamera = switchCameraDuration.curCamera;
			this.oldCamera = switchCameraDuration.oldCamera;
			this.switchFinished = switchCameraDuration.switchFinished;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.cameraId = -1;
			this.cameraId2 = -1;
			this.slerpTick = 3000;
			this.cutBackOnExit = false;
			this.startPos = Vector3.zero;
			this.startRot = Quaternion.identity;
			this.destPos = Vector3.zero;
			this.destRot = Quaternion.identity;
			this.curCamera = null;
			this.oldCamera = null;
		}

		private GameObject GetDestObj(Action _action)
		{
			GameObject gameObject = _action.GetGameObject(this.cameraId);
			if (this.cameraId2 != -1)
			{
				SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
				if (curLvelContext != null && curLvelContext.m_isCameraFlip)
				{
					gameObject = _action.GetGameObject(this.cameraId2);
				}
			}
			return gameObject;
		}

		public override void Enter(Action _action, Track _track)
		{
			this.switchFinished = false;
			GameObject destObj = this.GetDestObj(_action);
			if (destObj != null)
			{
				if (destObj.transform.parent != null && destObj.transform.parent.parent != null && destObj.transform.parent.parent.GetComponent<Moba_Camera>() != null)
				{
					this.isMoba_camera = true;
				}
				else
				{
					this.isMoba_camera = false;
				}
				this.curCamera = destObj.GetComponent<Camera>();
				DebugHelper.Assert(this.curCamera != null, "switch camera but dest camera not exist");
				if (this.curCamera != null)
				{
					this.curCamera.cullingMask &= ~LayerMask.GetMask(new string[]
					{
						"Hide"
					});
					this.curCamera.cullingMask &= ~LayerMask.GetMask(new string[]
					{
						"UIRaw"
					});
					this.curCamera.cullingMask &= ~LayerMask.GetMask(new string[]
					{
						"UI_Background"
					});
					this.curCamera.cullingMask &= ~LayerMask.GetMask(new string[]
					{
						"UI_Foreground"
					});
					this.curCamera.cullingMask &= ~LayerMask.GetMask(new string[]
					{
						"UI_BottomBG"
					});
					this.curCamera.cullingMask &= ~LayerMask.GetMask(new string[]
					{
						"3DUI"
					});
					this.destPos = this.curCamera.transform.position;
					this.destRot = this.curCamera.transform.rotation;
					this.oldCamera = Camera.main;
					if (this.oldCamera != null)
					{
						this.startPos = this.oldCamera.transform.position;
						this.startRot = this.oldCamera.transform.rotation;
						this.curCamera.transform.position = this.startPos;
						this.curCamera.transform.rotation = this.startRot;
						destObj.SetActive(true);
						SwitchCameraDuration.SwitchCamera(this.oldCamera, this.curCamera);
					}
				}
			}
			base.Enter(_action, _track);
		}

		public override void Leave(Action _action, Track _track)
		{
			if (this.curCamera != null && !this.switchFinished)
			{
				if (this.isMoba_camera)
				{
					this.curCamera.transform.position = this.curCamera.transform.parent.position;
					this.curCamera.transform.rotation = this.curCamera.transform.parent.rotation;
				}
				else
				{
					this.curCamera.transform.position = this.destPos;
					this.curCamera.transform.rotation = this.destRot;
				}
			}
			this.switchFinished = true;
			base.Leave(_action, _track);
		}

		public override void Process(Action _action, Track _track, int _localTime)
		{
			if (this.switchFinished || this.curCamera == null)
			{
				return;
			}
			if (_localTime >= this.slerpTick)
			{
				if (this.isMoba_camera)
				{
					this.curCamera.transform.position = this.curCamera.transform.parent.position;
					this.curCamera.transform.rotation = this.curCamera.transform.parent.rotation;
				}
				else
				{
					this.curCamera.transform.position = this.destPos;
					this.curCamera.transform.rotation = this.destRot;
				}
				this.switchFinished = true;
			}
			else if (this.isMoba_camera)
			{
				this.curCamera.transform.position = Vector3.Lerp(this.startPos, this.curCamera.transform.parent.position, (float)_localTime / (float)this.slerpTick);
				this.curCamera.transform.rotation = Quaternion.Slerp(this.startRot, this.curCamera.transform.parent.rotation, (float)_localTime / (float)this.slerpTick);
			}
			else
			{
				this.curCamera.transform.position = Vector3.Lerp(this.startPos, this.destPos, (float)_localTime / (float)this.slerpTick);
				this.curCamera.transform.rotation = Quaternion.Slerp(this.startRot, this.destRot, (float)_localTime / (float)this.slerpTick);
			}
			base.Process(_action, _track, _localTime);
		}

		public override bool Check(Action _action, Track _track)
		{
			return this.switchFinished;
		}

		private static void SwitchCamera(Camera camera1, Camera camera2)
		{
			if (camera1 != null)
			{
				camera1.enabled = false;
			}
			if (camera2 != null)
			{
				camera2.tag = "MainCamera";
				camera2.enabled = true;
			}
		}
	}
}
