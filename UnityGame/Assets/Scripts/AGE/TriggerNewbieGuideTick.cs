using Assets.Scripts.Common;
using System;

namespace AGE
{
	[EventCategory("MMGame/Newbie")]
	public class TriggerNewbieGuideTick : TickEvent
	{
		public bool bWeakGuide;

		public bool bCurrentGuideOver;

		public int NewbieTriggerType;

		public override bool SupportEditMode()
		{
			return true;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			TriggerNewbieGuideTick triggerNewbieGuideTick = src as TriggerNewbieGuideTick;
			this.bWeakGuide = triggerNewbieGuideTick.bWeakGuide;
			this.bCurrentGuideOver = triggerNewbieGuideTick.bCurrentGuideOver;
			this.NewbieTriggerType = triggerNewbieGuideTick.NewbieTriggerType;
		}

		public override BaseEvent Clone()
		{
			TriggerNewbieGuideTick triggerNewbieGuideTick = ClassObjPool<TriggerNewbieGuideTick>.Get();
			triggerNewbieGuideTick.CopyData(this);
			return triggerNewbieGuideTick;
		}

		public override void Process(Action _action, Track _track)
		{
			base.Process(_action, _track);
			if (this.NewbieTriggerType <= 0)
			{
				return;
			}
			if (this.bCurrentGuideOver)
			{
				if (this.bWeakGuide)
				{
					MonoSingleton<NewbieGuideManager>.GetInstance().ForceCompleteWeakGuide();
				}
				else
				{
					MonoSingleton<NewbieGuideManager>.GetInstance().ForceCompleteNewbieGuide();
				}
			}
			else if (this.bWeakGuide)
			{
				MonoSingleton<NewbieGuideManager>.GetInstance().CheckWeakGuideTrigger((NewbieGuideWeakGuideType)this.NewbieTriggerType, new uint[0]);
			}
			else
			{
				MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime((NewbieGuideTriggerTimeType)this.NewbieTriggerType, new uint[0]);
			}
		}
	}
}
