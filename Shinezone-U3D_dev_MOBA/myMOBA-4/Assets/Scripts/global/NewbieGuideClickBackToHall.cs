using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

public class NewbieGuideClickBackToHall : NewbieGuideBaseScript
{
	private void SetActiveHighlit()
	{
		for (int i = 0; i < NewbieGuideBaseScript.ms_originalGo.Count; i++)
		{
			GameObject gameObject = NewbieGuideBaseScript.ms_originalGo[i];
			GameObject obj = NewbieGuideBaseScript.ms_highlitGo[i];
			obj.CustomSetActive(gameObject.activeInHierarchy);
		}
	}

	protected override void Initialize()
	{
	}

	protected override void Update()
	{
		if (base.isInitialize)
		{
			base.Update();
			this.SetActiveHighlit();
			return;
		}
		CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(PVESettleSys.PATH_ITEM);
		if (form != null)
		{
			Transform transform = form.transform.FindChild("Root/Panel_Interactable/Button_ReturnLobby");
			if (transform != null)
			{
				GameObject gameObject = transform.gameObject;
				if (gameObject.activeInHierarchy)
				{
					base.AddHighLightGameObject(gameObject, true, form, true);
					this.SetActiveHighlit();
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
