using Assets.Scripts.Common;
using Assets.Scripts.GameSystem;
using System;

namespace AGE
{
	[EventCategory("MMGame/Newbie")]
	public class NewbieTlogTick : TickEvent
	{
		public int iStepId;

		public bool bIsLastStep;

		public override bool SupportEditMode()
		{
			return true;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			NewbieTlogTick newbieTlogTick = src as NewbieTlogTick;
			this.iStepId = newbieTlogTick.iStepId;
			this.bIsLastStep = newbieTlogTick.bIsLastStep;
		}

		public override BaseEvent Clone()
		{
			NewbieTlogTick newbieTlogTick = ClassObjPool<NewbieTlogTick>.Get();
			newbieTlogTick.CopyData(this);
			return newbieTlogTick;
		}

		public override void Process(Action _action, Track _track)
		{
			base.Process(_action, _track);
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo != null)
			{
				masterRoleInfo.ReqSetInBattleNewbieBit((uint)this.iStepId, this.bIsLastStep, 0);
			}
		}
	}
}
