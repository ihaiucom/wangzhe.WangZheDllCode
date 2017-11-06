using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

internal class NewbieGuideCommonAttackType1 : NewbieGuideBaseScript
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
		CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CSettingsSys.SETTING_FORM);
		if (form != null)
		{
			GameObject gameObject = form.transform.FindChild("OpSetting/ScrollRect/Content/CommonAttackToggle/CommonAttackType1").gameObject;
			if (gameObject.activeInHierarchy)
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

	public override bool IsTimeOutSkip()
	{
		return false;
	}
}
