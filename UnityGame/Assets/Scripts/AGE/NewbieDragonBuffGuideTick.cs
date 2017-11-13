using Assets.Scripts.Common;
using System;

namespace AGE
{
	[EventCategory("MMGame/Newbie")]
	internal class NewbieDragonBuffGuideTick : TickEvent
	{
		public override BaseEvent Clone()
		{
			NewbieDragonBuffGuideTick newbieDragonBuffGuideTick = ClassObjPool<NewbieDragonBuffGuideTick>.Get();
			newbieDragonBuffGuideTick.CopyData(this);
			return newbieDragonBuffGuideTick;
		}

		public override void Process(Action _action, Track _track)
		{
			MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.newbiePvPDragonBuff, new uint[0]);
		}
	}
}
