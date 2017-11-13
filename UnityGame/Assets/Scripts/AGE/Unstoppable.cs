using Assets.Scripts.Common;
using System;

namespace AGE
{
	[EventCategory("ActionControl")]
	public class Unstoppable : DurationEvent
	{
		public override BaseEvent Clone()
		{
			Unstoppable unstoppable = ClassObjPool<Unstoppable>.Get();
			unstoppable.CopyData(this);
			return unstoppable;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
		}

		public override void OnUse()
		{
			base.OnUse();
		}

		public override void Enter(Action _action, Track _track)
		{
			_action.unstoppable = true;
		}

		public override void Leave(Action _action, Track _track)
		{
			_action.unstoppable = false;
		}
	}
}
