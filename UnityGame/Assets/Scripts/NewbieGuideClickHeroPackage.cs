using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

public class NewbieGuideClickHeroPackage : NewbieGuideBaseScript
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
		CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(Singleton<CMallSystem>.GetInstance().sMallFormPath);
		if (form != null)
		{
			Transform transform = form.transform.FindChild("TopCommon/Panel_Menu/ListMenu/ScrollRect/Content/ListElement_0");
			if (transform != null)
			{
				GameObject gameObject = transform.gameObject;
				if (gameObject.activeInHierarchy)
				{
					base.AddHighLightGameObject(gameObject, true, form, true);
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
