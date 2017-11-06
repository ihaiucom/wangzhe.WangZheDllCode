using System;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	[AddComponentMenu("MMGameTrigger/AreaTrigger_Dialogue")]
	public class AreaEventTriggerDialogue : AreaEventTrigger
	{
		[FriendlyName("剧情对话组ID")]
		public int DialogueGroupId;

		[FriendlyName("离开时剧情对话组ID")]
		public int DialogueGroupIdLeaving;

		protected override void BuildTriggerWrapper()
		{
			this.PresetActWrapper = new TriggerActionWrapper(EGlobalTriggerAct.TriggerDialogue);
			this.PresetActWrapper.EnterUniqueId = this.DialogueGroupId;
			this.PresetActWrapper.LeaveUniqueId = this.DialogueGroupIdLeaving;
			this.PresetActWrapper.Init(this.ID);
		}
	}
}
