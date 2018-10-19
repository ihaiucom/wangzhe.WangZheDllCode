using Assets.Scripts.Common;
using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

namespace AGE
{
	[EventCategory("MMGame/Newbie")]
	internal class ToggleTalentPanelTick : TickEvent
	{
		public bool bShow;

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			ToggleTalentPanelTick toggleTalentPanelTick = src as ToggleTalentPanelTick;
			this.bShow = toggleTalentPanelTick.bShow;
		}

		public override BaseEvent Clone()
		{
			ToggleTalentPanelTick toggleTalentPanelTick = ClassObjPool<ToggleTalentPanelTick>.Get();
			toggleTalentPanelTick.CopyData(this);
			return toggleTalentPanelTick;
		}

		public override void Process(Action _action, Track _track)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(FightForm.s_battleUIForm);
			if (form != null)
			{
				GameObject obj = Utility.FindChild(form.gameObject, "PanelTalent");
				obj.CustomSetActive(this.bShow);
			}
		}
	}
}
