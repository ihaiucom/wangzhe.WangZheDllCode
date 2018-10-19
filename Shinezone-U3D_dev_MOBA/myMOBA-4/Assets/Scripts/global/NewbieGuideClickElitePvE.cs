using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

public class NewbieGuideClickElitePvE : NewbieGuideBaseScript
{
	protected override void Initialize()
	{
		CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CAdventureSys.ADVENTURE_SELECT_FORM);
		CUIStepListScript component = form.gameObject.transform.Find("ChapterList").gameObject.GetComponent<CUIStepListScript>();
		component.SelectElementImmediately(0);
		GameObject gameObject = form.transform.Find("TopCommon/Panel_Menu/ListMenu/ScrollRect/Content/ListElement_1").gameObject;
		base.AddHighLightGameObject(gameObject, true, form, true);
		base.Initialize();
	}

	protected override bool IsDelegateClickEvent()
	{
		return true;
	}
}
