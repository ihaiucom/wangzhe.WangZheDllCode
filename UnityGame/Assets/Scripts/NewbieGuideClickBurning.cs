using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

public class NewbieGuideClickBurning : NewbieGuideBaseScript
{
	protected override void Initialize()
	{
		CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CAdventureSys.EXLPORE_FORM_PATH);
		if (form != null)
		{
			GameObject gameObject = form.transform.Find("List/ScrollRect/Content/ListElement_1").gameObject;
			base.AddHighLightGameObject(gameObject, true, form, true);
			base.Initialize();
		}
	}

	protected override bool IsDelegateClickEvent()
	{
		return true;
	}
}
