using System;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	[AddComponentMenu("MMGameTrigger/AreaTrigger_BlockSwitcher")]
	public class AreaEventTriggerBlockSwitcher : AreaEventTrigger
	{
		public GameObject[] DynamicBlockList = new GameObject[0];

		[FriendlyName("开启")]
		public bool bEnable;

		[FriendlyName("离开时拔除")]
		public bool bStopWhenLeaving;

		protected override void BuildTriggerWrapper()
		{
			this.PresetActWrapper = new TriggerActionWrapper(EGlobalTriggerAct.TriggerDynamicBlock);
			this.PresetActWrapper.bEnable = this.bEnable;
			this.PresetActWrapper.bStopWhenLeaving = this.bStopWhenLeaving;
			this.PresetActWrapper.RefObjList = this.DynamicBlockList;
			this.PresetActWrapper.Init(this.ID);
		}
	}
}
