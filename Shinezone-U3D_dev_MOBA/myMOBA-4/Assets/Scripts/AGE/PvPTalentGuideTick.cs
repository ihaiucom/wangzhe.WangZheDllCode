using Assets.Scripts.Common;
using System;

namespace AGE
{
	[EventCategory("MMGame/Newbie")]
	internal class PvPTalentGuideTick : TickEvent
	{
		public int GuideType;

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			PvPTalentGuideTick pvPTalentGuideTick = src as PvPTalentGuideTick;
			this.GuideType = pvPTalentGuideTick.GuideType;
		}

		public override BaseEvent Clone()
		{
			PvPTalentGuideTick pvPTalentGuideTick = ClassObjPool<PvPTalentGuideTick>.Get();
			pvPTalentGuideTick.CopyData(this);
			return pvPTalentGuideTick;
		}

		public override void Process(Action _action, Track _track)
		{
			if (this.GuideType == 1)
			{
				MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.newbieTalent, new uint[0]);
			}
			else if (this.GuideType == 2)
			{
				MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.newbiePvPTalent, new uint[0]);
			}
			else if (this.GuideType == 3)
			{
				MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.newbieTalentSecondTime, new uint[0]);
			}
			else
			{
				DebugHelper.Assert(false, "Invalid Talent GuideType -- {0}", new object[]
				{
					this.GuideType
				});
			}
		}
	}
}
