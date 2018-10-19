using System;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	[AddComponentMenu("MMGameTrigger/AreaTrigger_BuffSkill")]
	public class AreaTrigger : AreaEventTrigger
	{
		[FriendlyName("进入时挂的BUFF")]
		public int BuffID;

		[FriendlyName("轮询探测时挂的BUFF")]
		public int UpdateBuffID;

		[FriendlyName("离开时挂的BUFF")]
		public int LeaveBuffID;

		[FriendlyName("离开时拔除")]
		public bool bStopWhenLeaving = true;

		[FriendlyName("作用于进入者")]
		public bool bToActor = true;

		[FriendlyName("作用于肇事者")]
		public bool bToAtker;

		protected override void BuildTriggerWrapper()
		{
			this.PresetActWrapper = new TriggerActionWrapper(EGlobalTriggerAct.TriggerBuff);
			this.PresetActWrapper.EnterUniqueId = this.BuffID;
			this.PresetActWrapper.UpdateUniqueId = this.UpdateBuffID;
			this.PresetActWrapper.LeaveUniqueId = this.LeaveBuffID;
			this.PresetActWrapper.bStopWhenLeaving = this.bStopWhenLeaving;
			this.PresetActWrapper.bSrc = this.bToActor;
			this.PresetActWrapper.bAtker = this.bToAtker;
			this.PresetActWrapper.Init(this.ID);
		}
	}
}
