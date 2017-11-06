using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

public class NewbieGuideClickBuyOne : NewbieGuideBaseScript
{
	protected override void Initialize()
	{
		CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(Singleton<CMallSystem>.GetInstance().sMallFormPath);
		if (form == null)
		{
			return;
		}
		CUIListScript component = form.transform.FindChild("TopCommon/Panel_Menu/ListMenu").gameObject.GetComponent<CUIListScript>();
		if (component != null)
		{
			component.MoveElementInScrollArea(0, true);
		}
		GameObject gameObject = form.transform.FindChild("pnlBodyBg/pnlLottery/pnlAction/pnlBuyOneFree/btnPanel/Button").gameObject;
		base.AddHighLightGameObject(gameObject, true, form, true);
		base.Initialize();
	}

	protected override bool IsDelegateClickEvent()
	{
		return true;
	}
}
