using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

internal class NewbieGuideClickFullStartAwardConfirm : NewbieGuideBaseScript
{
	protected override void Initialize()
	{
		CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CAdventureSys.ADVENTURE_SELECT_FORM);
		GameObject gameObject = form.transform.FindChild("ChapterRewardPanel/BtnGetReward").gameObject;
		base.AddHighLightGameObject(gameObject, true, form, true);
		base.Initialize();
	}

	protected override bool IsDelegateClickEvent()
	{
		return true;
	}
}
