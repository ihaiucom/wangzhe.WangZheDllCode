using Assets.Scripts.UI;
using System;
using UnityEngine;

internal class NewbieGuideClickTaskFinish : NewbieGuideBaseScript
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
		CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Task/Form_Task_Finish.prefab");
		if (form != null)
		{
			Transform transform = form.transform.FindChild("ListElement0/LinkBtn");
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
