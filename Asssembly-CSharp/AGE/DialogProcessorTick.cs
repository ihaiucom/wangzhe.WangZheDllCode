using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;

namespace AGE
{
	[EventCategory("MMGame/Drama")]
	public class DialogProcessorTick : TickEvent
	{
		public int DialogGroupId;

		public override bool SupportEditMode()
		{
			return true;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			DialogProcessorTick dialogProcessorTick = src as DialogProcessorTick;
			this.DialogGroupId = dialogProcessorTick.DialogGroupId;
		}

		public override BaseEvent Clone()
		{
			DialogProcessorTick dialogProcessorTick = ClassObjPool<DialogProcessorTick>.Get();
			dialogProcessorTick.CopyData(this);
			return dialogProcessorTick;
		}

		public override void Process(Action _action, Track _track)
		{
			int dialogGroupId = this.DialogGroupId;
			if (dialogGroupId <= 0)
			{
				_action.refParams.GetRefParam("DialogGroupIdRaw", ref dialogGroupId);
			}
			if (dialogGroupId > 0)
			{
				MonoSingleton<DialogueProcessor>.GetInstance().StartDialogue(dialogGroupId);
			}
		}
	}
}
