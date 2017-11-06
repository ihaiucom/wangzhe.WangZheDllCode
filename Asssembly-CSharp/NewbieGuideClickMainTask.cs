using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

public class NewbieGuideClickMainTask : NewbieGuideBaseScript
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
		CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CTaskSys.TASK_FORM_PATH);
		if (form == null)
		{
			return;
		}
		CUIListScript component = form.transform.FindChild("TopCommon/Panel_Menu/List").GetComponent<CUIListScript>();
		GameObject gameObject = null;
		int index = (int)base.currentConf.Param[0];
		if (component != null)
		{
			component.MoveElementInScrollArea(index, true);
			gameObject = component.GetElemenet(index).gameObject;
		}
		if (gameObject != null)
		{
			base.AddHighLightGameObject(gameObject, true, form, true);
			base.Initialize();
		}
	}

	protected override bool IsDelegateClickEvent()
	{
		return true;
	}
}
