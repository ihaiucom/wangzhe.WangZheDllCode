using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using System;
using UnityEngine;

namespace AGE
{
	[EventCategory("MMGame/Drama")]
	public class TimeScalerDuration : DurationEvent
	{
		public float TimeScale = 1f;

		public override bool SupportEditMode()
		{
			return true;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.TimeScale = 1f;
		}

		public override BaseEvent Clone()
		{
			TimeScalerDuration timeScalerDuration = ClassObjPool<TimeScalerDuration>.Get();
			timeScalerDuration.CopyData(this);
			return timeScalerDuration;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			TimeScalerDuration timeScalerDuration = src as TimeScalerDuration;
			this.TimeScale = timeScalerDuration.TimeScale;
		}

		public override void Enter(Action _action, Track _track)
		{
			base.Enter(_action, _track);
			DebugHelper.Assert(!Singleton<FrameSynchr>.instance.bActive, "frame synchr active forbid set timeScale");
			Time.timeScale = this.TimeScale;
		}

		public override void Leave(Action _action, Track _track)
		{
			Time.timeScale = 1f;
			DebugHelper.Assert(!Singleton<FrameSynchr>.instance.bActive, "frame synchr active forbid set timeScale");
			base.Leave(_action, _track);
		}
	}
}
