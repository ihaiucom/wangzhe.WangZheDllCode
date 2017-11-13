using System;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	[AddComponentMenu("MMGameTrigger/AreaTrigger_Age")]
	public class AreaEventTriggerAge : AreaEventTrigger
	{
		[SerializeField]
		public AreaEventTrigger.STimingAction[] TimingActionsInter = new AreaEventTrigger.STimingAction[0];

		protected override void BuildTriggerWrapper()
		{
			this.PresetActWrapper = new TriggerActionWrapper(EGlobalTriggerAct.TriggerAge);
			this.PresetActWrapper.TimingActionsInter = this.TimingActionsInter;
			this.PresetActWrapper.Init(this.ID);
		}
	}
}
