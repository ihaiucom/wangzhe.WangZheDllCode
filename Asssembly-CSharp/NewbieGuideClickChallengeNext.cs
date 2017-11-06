using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

public class NewbieGuideClickChallengeNext : NewbieGuideBaseScript
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
		CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CAdventureSys.ADVENTURE_SELECT_FORM);
		if (form != null)
		{
			CUIListScript component = form.transform.FindChild("LevelList").GetComponent<CUIListScript>();
			component.SelectElement(0, true);
			CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
			uIEvent.m_eventParams.tag = 1;
			uIEvent.m_eventID = enUIEventID.Adv_SelectLevel;
			uIEvent.m_srcFormScript = form;
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uIEvent);
			Transform transform = form.transform.FindChild("ButtonEnter");
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
