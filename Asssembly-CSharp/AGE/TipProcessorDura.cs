using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;

namespace AGE
{
	[EventCategory("MMGame/Drama")]
	public class TipProcessorDura : DurationEvent
	{
		public int GuideTipId;

		public override bool SupportEditMode()
		{
			return true;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			TipProcessorDura tipProcessorDura = src as TipProcessorDura;
			this.GuideTipId = tipProcessorDura.GuideTipId;
		}

		public override BaseEvent Clone()
		{
			TipProcessorDura tipProcessorDura = ClassObjPool<TipProcessorDura>.Get();
			tipProcessorDura.CopyData(this);
			return tipProcessorDura;
		}

		public override void Enter(Action _action, Track _track)
		{
			int guideTipId = this.GuideTipId;
			if (guideTipId <= 0)
			{
				_action.refParams.GetRefParam("GuideTipIdRaw", ref guideTipId);
			}
			if (guideTipId > 0)
			{
				Singleton<TipProcessor>.GetInstance().PlayDrama(guideTipId, null, null);
			}
		}

		public override void Leave(Action _action, Track _track)
		{
			int guideTipId = this.GuideTipId;
			if (guideTipId <= 0)
			{
				_action.refParams.GetRefParam("GuideTipIdRaw", ref guideTipId);
			}
			if (guideTipId > 0)
			{
				Singleton<TipProcessor>.GetInstance().EndDrama(guideTipId);
			}
		}
	}
}
