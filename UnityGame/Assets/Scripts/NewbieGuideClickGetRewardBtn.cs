using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

public class NewbieGuideClickGetRewardBtn : NewbieGuideBaseScript
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
		if (form != null)
		{
			CTaskView taskView = Singleton<CTaskSys>.GetInstance().GetTaskView();
			if (taskView != null)
			{
				taskView.On_Tab_Change(0);
				CUIListScript component = form.GetWidget(2).GetComponent<CUIListScript>();
				component.SelectElement(0, true);
				GameObject widget = form.GetWidget(13);
				if (widget != null)
				{
					base.AddHighLightGameObject(widget, true, form, true);
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
