using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

internal class NewbieGuideClickPvpSettleContinue : NewbieGuideBaseScript
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
		CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CSettleSystem.PATH_PVP_SETTLE_PVP);
		if (form != null)
		{
			Transform transform = form.transform.FindChild("PanelA/Btn_Continue");
			if (transform != null)
			{
				GameObject gameObject = transform.gameObject;
				if (gameObject.activeInHierarchy)
				{
					base.AddHighLightGameObject(gameObject, true, form, true);
					base.Initialize();
				}
			}
		}
	}

	protected override bool IsDelegateClickEvent()
	{
		return true;
	}
}
