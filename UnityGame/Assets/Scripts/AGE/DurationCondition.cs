using System;

namespace AGE
{
	public abstract class DurationCondition : DurationEvent
	{
		public override void Process(Action _action, Track _track, int _localTime)
		{
			bool status = this.Check(_action, _track);
			_action.SetCondition(_track, status);
		}

		public override void Enter(Action _action, Track _track)
		{
			bool status = this.Check(_action, _track);
			_action.SetCondition(_track, status);
		}

		public override void Leave(Action _action, Track _track)
		{
			bool status = this.Check(_action, _track);
			_action.SetCondition(_track, status);
		}

		public virtual bool Check(Action _action, Track _track)
		{
			return true;
		}
	}
}
