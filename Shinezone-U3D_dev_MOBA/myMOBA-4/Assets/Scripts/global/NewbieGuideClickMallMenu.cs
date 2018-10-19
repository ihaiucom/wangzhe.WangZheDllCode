using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

public class NewbieGuideClickMallMenu : NewbieGuideBaseScript
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
		CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(Singleton<CMallSystem>.instance.sMallFormPath);
		if (form == null)
		{
			return;
		}
		CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(form.gameObject, "TopCommon/Panel_Menu/ListMenu");
		if (componetInChild == null)
		{
			return;
		}
		int tabIndex = Singleton<CMallSystem>.instance.GetTabIndex((CMallSystem.Tab)base.currentConf.Param[0]);
		GameObject gameObject = componetInChild.GetElemenet(tabIndex).gameObject;
		base.AddHighLightGameObject(gameObject, true, form, true);
		base.Initialize();
	}

	protected override bool IsDelegateClickEvent()
	{
		return true;
	}
}
