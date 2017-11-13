using AGE;
using Assets.Scripts.Common;
using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public class TriggerActionShowToggleAuto : TriggerActionBase
	{
		public TriggerActionShowToggleAuto(TriggerActionWrapper inWrapper, int inTriggerId) : base(inWrapper, inTriggerId)
		{
		}

		private void EnableToggleAuto(bool bInEnable)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(FightForm.s_battleUIForm);
			if (form)
			{
				GameObject gameObject = form.transform.FindChild("PVPTopRightPanel/PanelBtn/ToggleAutoBtn").gameObject;
				gameObject.CustomSetActive(bInEnable);
			}
		}

		public override RefParamOperator TriggerEnter(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker, ITrigger inTrigger)
		{
			this.EnableToggleAuto(this.bEnable);
			return null;
		}

		public override void TriggerLeave(PoolObjHandle<ActorRoot> src, ITrigger inTrigger)
		{
			if (this.bStopWhenLeaving)
			{
				this.EnableToggleAuto(!this.bEnable);
			}
		}
	}
}
