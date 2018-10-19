using Assets.Scripts.UI;
using System;
using UnityEngine;

public class NewbieGuideClickConfirmReward : NewbieGuideBaseScript
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
		CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(string.Format("{0}{1}", "UGUI/Form/Common/", "Form_Award"));
		if (form != null)
		{
			GameObject gameObject = form.transform.FindChild("btnGroup/Button_Back").gameObject;
			base.AddHighLightGameObject(gameObject, true, form, true);
			base.Initialize();
		}
	}

	protected override bool IsDelegateClickEvent()
	{
		return true;
	}
}
