using System;

namespace AGE
{
	public abstract class TickEvent : BaseEvent
	{
		public virtual void Process(Action _action, Track _track)
		{
		}

		public virtual void ProcessBlend(Action _action, Track _track, TickEvent _prevEvent, float _blendWeight)
		{
		}

		public virtual void PostProcess(Action _action, Track _track, int _localTime)
		{
		}
	}
}
