using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;

namespace AGE
{
	[EventCategory("MMGame/Newbie")]
	internal class ShowGuideUIFormTick : TickEvent
	{
		public CBattleGuideManager.EBattleGuideFormType FormType;

		public int delayShowInteractable;

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			ShowGuideUIFormTick showGuideUIFormTick = src as ShowGuideUIFormTick;
			this.FormType = showGuideUIFormTick.FormType;
			this.delayShowInteractable = showGuideUIFormTick.delayShowInteractable;
		}

		public override BaseEvent Clone()
		{
			ShowGuideUIFormTick showGuideUIFormTick = ClassObjPool<ShowGuideUIFormTick>.Get();
			showGuideUIFormTick.CopyData(this);
			return showGuideUIFormTick;
		}

		public override void Process(Action _action, Track _track)
		{
			base.Process(_action, _track);
			if (this.FormType > CBattleGuideManager.EBattleGuideFormType.Invalid)
			{
				Singleton<CBattleGuideManager>.GetInstance().OpenFormShared(this.FormType, this.delayShowInteractable, true);
			}
		}
	}
}
