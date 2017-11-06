using System;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	[AddComponentMenu("MMGameTrigger/AreaTrigger_GuideTip")]
	public class AreaEventTriggerGuideTip : AreaEventTrigger
	{
		[FriendlyName("新手提示ID")]
		public int GuideTipId;

		[FriendlyName("离开时新手提示ID")]
		public int GuideTipIdLeaving;

		protected override void BuildTriggerWrapper()
		{
			this.PresetActWrapper = new TriggerActionWrapper(EGlobalTriggerAct.TriggerGuideTip);
			this.PresetActWrapper.EnterUniqueId = this.GuideTipId;
			this.PresetActWrapper.LeaveUniqueId = this.GuideTipIdLeaving;
			this.PresetActWrapper.Init(this.ID);
		}
	}
}
