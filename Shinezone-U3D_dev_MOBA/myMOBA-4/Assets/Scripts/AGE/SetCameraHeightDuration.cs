using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameSystem;
using System;
using UnityEngine;

namespace AGE
{
	[EventCategory("MMGame/Skill")]
	public class SetCameraHeightDuration : DurationCondition
	{
		public int slerpTick = 500;

		public bool cutBackOnExit = true;

		private bool setFinished;

		public float heightRate = 1f;

		private float from = 1f;

		private float to = 1f;

		private bool bInvalid;

		public override bool SupportEditMode()
		{
			return true;
		}

		public override BaseEvent Clone()
		{
			SetCameraHeightDuration setCameraHeightDuration = ClassObjPool<SetCameraHeightDuration>.Get();
			setCameraHeightDuration.CopyData(this);
			return setCameraHeightDuration;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			SetCameraHeightDuration setCameraHeightDuration = src as SetCameraHeightDuration;
			this.slerpTick = setCameraHeightDuration.slerpTick;
			this.cutBackOnExit = setCameraHeightDuration.cutBackOnExit;
			this.setFinished = setCameraHeightDuration.setFinished;
			this.heightRate = setCameraHeightDuration.heightRate;
			this.bInvalid = setCameraHeightDuration.bInvalid;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.slerpTick = 500;
			this.cutBackOnExit = true;
			this.setFinished = false;
			this.heightRate = 1f;
			this.from = 1f;
			this.to = 1f;
			this.bInvalid = false;
		}

		public override void Enter(Action _action, Track _track)
		{
			SkillUseContext refParamObject = _action.refParams.GetRefParamObject<SkillUseContext>("SkillContext");
			PoolObjHandle<ActorRoot> originator = refParamObject.Originator;
			if (!ActorHelper.IsHostCtrlActor(ref originator) || Singleton<WatchController>.instance.IsWatching)
			{
				this.bInvalid = true;
				return;
			}
			this.heightRate = ((this.heightRate >= 1f) ? this.heightRate : 1f);
			if (MonoSingleton<CameraSystem>.instance.ZoomRateFromAge != this.heightRate)
			{
				this.setFinished = false;
				this.from = ((this.heightRate <= 1f) ? MonoSingleton<CameraSystem>.instance.ZoomRateFromAge : 1f);
				this.to = ((this.heightRate <= 1f) ? 1f : this.heightRate);
			}
			else
			{
				this.setFinished = true;
			}
			base.Enter(_action, _track);
		}

		public override void Leave(Action _action, Track _track)
		{
			if (this.bInvalid)
			{
				return;
			}
			if (this.cutBackOnExit)
			{
				MonoSingleton<CameraSystem>.instance.ZoomRateFromAge = 1f;
			}
			else
			{
				MonoSingleton<CameraSystem>.instance.ZoomRateFromAge = this.heightRate;
			}
			this.setFinished = true;
			base.Leave(_action, _track);
		}

		public override void Process(Action _action, Track _track, int _localTime)
		{
			if (this.bInvalid || this.setFinished)
			{
				return;
			}
			if (_localTime >= this.slerpTick)
			{
				MonoSingleton<CameraSystem>.instance.ZoomRateFromAge = this.heightRate;
				this.setFinished = true;
			}
			else
			{
				float zoomRateFromAge = Mathf.Lerp(this.from, this.to, (float)_localTime / (float)this.slerpTick);
				MonoSingleton<CameraSystem>.instance.ZoomRateFromAge = zoomRateFromAge;
			}
			base.Process(_action, _track, _localTime);
		}

		public override bool Check(Action _action, Track _track)
		{
			return this.setFinished;
		}
	}
}
