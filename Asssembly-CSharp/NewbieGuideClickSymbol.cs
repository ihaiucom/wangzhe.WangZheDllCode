using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

public class NewbieGuideClickSymbol : NewbieGuideBaseScript
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
		CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CSymbolSystem.s_symbolFormPath);
		if (form != null)
		{
			Transform transform = form.transform.Find("SymbolEquip/Panel_SymbolEquip/Panel_SymbolBag/Panel_BagList/List");
			if (transform != null)
			{
				CUIListScript component = transform.GetComponent<CUIListScript>();
				if (component != null)
				{
					int index = (int)base.currentConf.Param[1];
					if (base.currentConf.Param[0] > 0)
					{
						index = Singleton<CSymbolSystem>.GetInstance().GetSymbolListIndex(NewbieGuideCheckTriggerConditionUtil.AvailableSymbolId);
					}
					CUIListElementScript elemenet = component.GetElemenet(index);
					GameObject gameObject = (elemenet != null) ? elemenet.gameObject : null;
					if (gameObject != null && gameObject.activeInHierarchy)
					{
						base.AddHighLightGameObject(gameObject, true, form, true);
						base.Initialize();
					}
				}
			}
		}
	}

	protected override bool IsDelegateClickEvent()
	{
		return true;
	}
}
