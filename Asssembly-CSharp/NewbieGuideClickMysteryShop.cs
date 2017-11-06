using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

internal class NewbieGuideClickMysteryShop : NewbieGuideBaseScript
{
	protected override void Initialize()
	{
		CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(Singleton<CShopSystem>.GetInstance().sShopFormPath);
		if (form == null)
		{
			return;
		}
		GameObject gameObject = form.transform.FindChild("pnlShop/Tab/ScrollRect/Content/ListElement_1").gameObject;
		base.AddHighLightGameObject(gameObject, true, form, true);
		base.Initialize();
	}

	protected override bool IsDelegateClickEvent()
	{
		return true;
	}
}
