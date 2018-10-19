using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

public class NewbieGuideClickEquipBuyBtn : NewbieGuideBaseScript
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
		CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CBattleEquipSystem.s_equipFormPath);
		if (form == null)
		{
			return;
		}
		GameObject gameObject = form.GetWidget(6).transform.FindChild("buyBtn").gameObject;
		DebugHelper.Assert(gameObject != null, "Can't find buybtn~!!");
		base.AddHighLightGameObject(gameObject, true, form, true);
		base.Initialize();
	}

	protected override bool IsDelegateClickEvent()
	{
		return true;
	}
}
