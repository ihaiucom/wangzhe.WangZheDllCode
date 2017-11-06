using System;
using System.Reflection;

namespace AGE
{
	public class RefData
	{
		public FieldInfo fieldInfo;

		public object dataObject;

		public int trackIndex = -1;

		public int eventIdx = -1;

		public RefData(FieldInfo field, object obj)
		{
			this.fieldInfo = field;
			this.dataObject = obj;
			if (this.dataObject is Track)
			{
				Track track = this.dataObject as Track;
				this.trackIndex = track.trackIndex;
				this.eventIdx = -1;
			}
			else
			{
				BaseEvent baseEvent = this.dataObject as BaseEvent;
				this.trackIndex = baseEvent.track.trackIndex;
				this.eventIdx = baseEvent.track.GetIndexOfEvent(baseEvent);
			}
		}
	}
}
