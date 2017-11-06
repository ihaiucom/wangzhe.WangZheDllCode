using Assets.Scripts.Common;
using System;

namespace AGE
{
	[EventCategory("Debug")]
	public class BreakPoint : TickEvent
	{
		public bool enabled = true;

		public string info = string.Empty;

		public override BaseEvent Clone()
		{
			BreakPoint breakPoint = ClassObjPool<BreakPoint>.Get();
			breakPoint.CopyData(this);
			return breakPoint;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			BreakPoint breakPoint = src as BreakPoint;
			this.enabled = breakPoint.enabled;
			this.info = breakPoint.info;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.enabled = true;
			this.info = string.Empty;
		}

		public override void Process(Action _action, Track _track)
		{
		}
	}
}
