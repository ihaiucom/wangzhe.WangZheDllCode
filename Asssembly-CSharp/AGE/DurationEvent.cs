using System;

namespace AGE
{
	public abstract class DurationEvent : BaseEvent
	{
		public int length;

		public virtual bool bScaleLength
		{
			get
			{
				return true;
			}
		}

		public int Start
		{
			get
			{
				return this.time;
			}
			set
			{
				int end = this.End;
				this.time = value;
				if (value < end)
				{
					this.length = end - this.time;
				}
				else
				{
					this.length = 0;
				}
			}
		}

		public int End
		{
			get
			{
				return this.time + this.length;
			}
			set
			{
				if (value > this.time)
				{
					this.length = value - this.time;
				}
				else
				{
					this.length = 0;
					this.time = value;
				}
			}
		}

		public float lengthSec
		{
			get
			{
				return ActionUtility.MsToSec(this.length);
			}
		}

		public override void OnUse()
		{
			base.OnUse();
			this.length = 0;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			DurationEvent durationEvent = src as DurationEvent;
			this.length = durationEvent.length;
		}

		public virtual void Process(Action _action, Track _track, int _localTime)
		{
		}

		public virtual void ProcessBlend(Action _action, Track _track, int _localTime, DurationEvent _prevEvent, int _prevLocalTime, float _blendWeight)
		{
			if (_prevEvent != null)
			{
				_prevEvent.Process(_action, _track, _prevLocalTime);
			}
			this.Process(_action, _track, _localTime);
		}

		public virtual void Enter(Action _action, Track _track)
		{
		}

		public virtual void EnterBlend(Action _action, Track _track, BaseEvent _prevEvent, int _blendTime)
		{
			this.Enter(_action, _track);
		}

		public virtual void Leave(Action _action, Track _track)
		{
		}

		public virtual void LeaveBlend(Action _action, Track _track, BaseEvent _nextEvent, int _blendTime)
		{
			this.Leave(_action, _track);
		}
	}
}
