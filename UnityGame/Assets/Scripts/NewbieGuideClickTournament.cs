using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

internal class NewbieGuideClickTournament : NewbieGuideBaseScript
{
	protected override void Initialize()
	{
		CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CAdventureSys.EXLPORE_FORM_PATH);
		if (form == null)
		{
			return;
		}
		GameObject gameObject = form.transform.FindChild("List/ScrollRect/Content/ListElement_3").gameObject;
		base.AddHighLightGameObject(gameObject, true, form, true);
		base.Initialize();
	}

	protected override bool IsDelegateClickEvent()
	{
		return true;
	}
}
