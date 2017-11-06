using Assets.Scripts.Common;
using System;

namespace AGE
{
	[EventCategory("MMGame/Newbie")]
	public class TriggerNewbieGuideByNameTick : TickEvent
	{
		public enum eNewbieGuideName
		{
			MapSignGuide,
			JungleBuyEquipGuide,
			SelectCommonAttackType,
			BattleInfoGuide
		}

		public TriggerNewbieGuideByNameTick.eNewbieGuideName newbieGuideType;

		public int parm0;

		public override bool SupportEditMode()
		{
			return true;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			TriggerNewbieGuideByNameTick triggerNewbieGuideByNameTick = src as TriggerNewbieGuideByNameTick;
			this.newbieGuideType = triggerNewbieGuideByNameTick.newbieGuideType;
			this.parm0 = triggerNewbieGuideByNameTick.parm0;
		}

		public override BaseEvent Clone()
		{
			TriggerNewbieGuideByNameTick triggerNewbieGuideByNameTick = ClassObjPool<TriggerNewbieGuideByNameTick>.Get();
			triggerNewbieGuideByNameTick.CopyData(this);
			return triggerNewbieGuideByNameTick;
		}

		public override void Process(Action _action, Track _track)
		{
			base.Process(_action, _track);
			switch (this.newbieGuideType)
			{
			case TriggerNewbieGuideByNameTick.eNewbieGuideName.MapSignGuide:
				MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.onMiniMapSignGuide, new uint[0]);
				return;
			case TriggerNewbieGuideByNameTick.eNewbieGuideName.JungleBuyEquipGuide:
				MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.onJungleEquipGuide, new uint[0]);
				return;
			case TriggerNewbieGuideByNameTick.eNewbieGuideName.SelectCommonAttackType:
				MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.onSetLockOrFreeTargetType, new uint[]
				{
					(uint)this.parm0
				});
				return;
			case TriggerNewbieGuideByNameTick.eNewbieGuideName.BattleInfoGuide:
				MonoSingleton<NewbieGuideManager>.GetInstance().CheckTriggerTime(NewbieGuideTriggerTimeType.onClickBattleInfoBtn, new uint[0]);
				return;
			default:
				return;
			}
		}
	}
}
