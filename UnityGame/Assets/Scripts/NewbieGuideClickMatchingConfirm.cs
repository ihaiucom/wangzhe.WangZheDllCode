using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

public class NewbieGuideClickMatchingConfirm : NewbieGuideBaseScript
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
		CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CMatchingSystem.PATH_MATCHING_CONFIRMBOX);
		if (form != null)
		{
			GameObject gameObject = form.transform.FindChild("Panel/Panel/btnGroup/Button_Confirm").gameObject;
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
