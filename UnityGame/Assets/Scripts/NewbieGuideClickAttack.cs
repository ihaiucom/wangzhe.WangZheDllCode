using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

public class NewbieGuideClickAttack : NewbieGuideBaseScript
{
	private GameObject buttonObj;

	private CSignalButton signalButton;

	private CUIFormScript battleForm;

	protected override void Initialize()
	{
		this.battleForm = Singleton<CUIManager>.GetInstance().GetForm(FightForm.s_battleUIForm);
		if (this.battleForm == null)
		{
			return;
		}
		this.buttonObj = this.battleForm.GetWidget(13);
		if (this.buttonObj != null)
		{
			CUIEventScript component = this.buttonObj.GetComponent<CUIEventScript>();
			this.signalButton = Singleton<CBattleSystem>.GetInstance().FightForm.GetSignalPanel().GetSingleButton(component.m_onClickEventParams.tag);
		}
	}

	protected override void Update()
	{
		if (base.isInitialize)
		{
			base.Update();
			return;
		}
		if (this.signalButton == null || this.signalButton.IsInCooldown())
		{
			return;
		}
		if (this.buttonObj != null)
		{
			base.AddHighLightGameObject(this.buttonObj, true, this.battleForm, true);
			base.Initialize();
		}
	}

	protected override bool IsDelegateClickEvent()
	{
		return true;
	}
}
