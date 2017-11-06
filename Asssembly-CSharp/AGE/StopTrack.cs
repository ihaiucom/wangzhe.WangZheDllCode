using Assets.Scripts.Common;
using System;

namespace AGE
{
	[EventCategory("Utility")]
	public class StopTrack : TickEvent
	{
		public int trackId = -1;

		public override BaseEvent Clone()
		{
			StopTrack stopTrack = ClassObjPool<StopTrack>.Get();
			stopTrack.CopyData(this);
			return stopTrack;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			StopTrack stopTrack = src as StopTrack;
			this.trackId = stopTrack.trackId;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.trackId = -1;
		}

		public override void Process(Action _action, Track _track)
		{
			Track track = _action.GetTrack(this.trackId);
			if (track != null)
			{
				track.Stop(_action);
			}
		}
	}
}
