using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

public class NewbieGuideClickSymbolBuy : NewbieGuideBaseScript
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
		CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CSymbolMakeController.s_symbolTransformPath);
		if (form != null)
		{
			GameObject gameObject = form.transform.Find("Panel_SymbolTranform/Panel_Content/btnMake").gameObject;
			if (gameObject != null)
			{
				base.AddHighLightGameObject(gameObject, true, form, true);
				base.Initialize();
			}
		}
	}

	protected override bool IsDelegateClickEvent()
	{
		return true;
	}
}
