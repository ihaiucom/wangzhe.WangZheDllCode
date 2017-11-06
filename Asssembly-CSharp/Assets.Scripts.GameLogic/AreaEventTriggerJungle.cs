using System;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	[AddComponentMenu("MMGameTrigger/AreaTrigger_Jungle")]
	public class AreaEventTriggerJungle : AreaEventTrigger
	{
		protected override void BuildTriggerWrapper()
		{
			this.PresetActWrapper = new TriggerActionWrapper(EGlobalTriggerAct.TriggerJungle);
			this.PresetActWrapper.Init(this.ID);
		}

		public override void UpdateLogic(int delta)
		{
			base.UpdateLogic(delta);
		}
	}
}
