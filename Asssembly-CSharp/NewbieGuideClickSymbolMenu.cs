using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

public class NewbieGuideClickSymbolMenu : NewbieGuideBaseScript
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
			Transform transform = form.transform.Find("TopCommon/Panel_Menu/ListMenu");
			if (transform != null)
			{
				CUIListScript component = transform.GetComponent<CUIListScript>();
				CUIListElementScript elemenet = component.GetElemenet((int)base.currentConf.Param[0]);
				if (elemenet != null)
				{
					base.AddHighLightGameObject(elemenet.gameObject, true, form, true);
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
