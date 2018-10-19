using Assets.Scripts.UI;
using System;
using UnityEngine;

internal class NewbieGuideClickMsgBox : NewbieGuideBaseScript
{
	protected override void Initialize()
	{
		CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/Common/Form_MessageBox.prefab");
		if (form != null)
		{
			bool flag = base.currentConf.Param[0] == 0;
			string name = string.Format("Panel/Panel/btnGroup/Button_{0}", (!flag) ? "Cancel" : "Confirm");
			GameObject gameObject = form.transform.FindChild(name).gameObject;
			base.AddHighLightGameObject(gameObject, true, form, true);
			NewbieGuideScriptControl.FormGuideMask.SetPriority(enFormPriority.Priority9);
			base.Initialize();
		}
	}

	protected override void Update()
	{
		base.Update();
	}

	protected override bool IsDelegateClickEvent()
	{
		return true;
	}
}
