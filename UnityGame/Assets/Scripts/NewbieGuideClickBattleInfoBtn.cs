using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

public class NewbieGuideClickBattleInfoBtn : NewbieGuideBaseScript
{
	protected override void Initialize()
	{
	}

	protected override void Update()
	{
		if (base.isInitialize)
		{
			base.Update();
			return;
		}
		CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(FightForm.s_battleUIForm);
		if (form != null)
		{
			GameObject gameObject = form.transform.FindChild("PVPTopRightPanel/PanelBtn/btnViewBattleInfo").gameObject;
			base.AddHighLightGameObject(gameObject, true, form, true);
			base.Initialize();
		}
	}

	protected override bool IsDelegateClickEvent()
	{
		return true;
	}
}
