using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;

namespace AGE
{
	[EventCategory("MMGame/Drama")]
	public class TipProcessorTick : TickEvent
	{
		public bool bPlayTip = true;

		public int GuideTipId;

		public override bool SupportEditMode()
		{
			return true;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			TipProcessorTick tipProcessorTick = src as TipProcessorTick;
			this.bPlayTip = tipProcessorTick.bPlayTip;
			this.GuideTipId = tipProcessorTick.GuideTipId;
		}

		public override BaseEvent Clone()
		{
			TipProcessorTick tipProcessorTick = ClassObjPool<TipProcessorTick>.Get();
			tipProcessorTick.CopyData(this);
			return tipProcessorTick;
		}

		public override void Process(Action _action, Track _track)
		{
			int guideTipId = this.GuideTipId;
			if (guideTipId <= 0)
			{
				_action.refParams.GetRefParam("GuideTipIdRaw", ref guideTipId);
			}
			if (guideTipId > 0)
			{
				if (this.bPlayTip)
				{
					Singleton<TipProcessor>.GetInstance().PlayDrama(guideTipId, null, null);
				}
				else
				{
					Singleton<TipProcessor>.GetInstance().EndDrama(guideTipId);
				}
			}
		}
	}
}
