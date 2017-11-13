using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

public class NewbieGuideClickSymbolBuyClose : NewbieGuideBaseScript
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
			Transform transform = form.transform.Find("Panel_SymbolTranform/Panel_Title/btnClose");
			if (transform != null)
			{
				base.AddHighLightGameObject(transform.gameObject, true, form, true);
				base.Initialize();
			}
		}
	}

	protected override bool IsDelegateClickEvent()
	{
		return true;
	}
}
