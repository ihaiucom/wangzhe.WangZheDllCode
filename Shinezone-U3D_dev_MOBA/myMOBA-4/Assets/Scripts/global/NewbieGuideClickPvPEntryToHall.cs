using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

public class NewbieGuideClickPvPEntryToHall : NewbieGuideBaseScript
{
	protected override void Initialize()
	{
		CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CMatchingSystem.PATH_MATCHING_ENTRY);
		if (form != null)
		{
			Transform transform = form.transform.FindChild("panelGroup1/Button_Close");
			if (transform != null)
			{
				base.AddHighLightGameObject(transform.gameObject, true, form, true);
			}
			base.Initialize();
		}
	}

	protected override bool IsDelegateClickEvent()
	{
		return true;
	}
}
