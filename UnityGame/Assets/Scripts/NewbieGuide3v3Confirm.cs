using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

public class NewbieGuide3v3Confirm : NewbieGuideBaseScript
{
	private CUIFormScript Guide3v3ConfirmForm;

	private CUIEventScript CanCelBtnScript;

	private CUIEventScript ConfirmBtnScript;

	protected override void Initialize()
	{
		this.Guide3v3ConfirmForm = Singleton<CUIManager>.GetInstance().OpenForm(NewbieGuideManager.FORM_3v3GUIDE_CONFIRM, false, true);
		GameObject gameObject = this.Guide3v3ConfirmForm.transform.FindChild("Bg/btnCancel").gameObject;
		GameObject gameObject2 = this.Guide3v3ConfirmForm.transform.FindChild("Bg/btnConfirm").gameObject;
		DebugHelper.Assert(gameObject != null && gameObject2 != null, string.Format("{0}can't find cancel and confirm button in {1}", base.logTitle, NewbieGuideManager.FORM_3v3GUIDE_CONFIRM));
		this.CanCelBtnScript = gameObject.GetComponent<CUIEventScript>();
		if (this.CanCelBtnScript != null)
		{
			this.CanCelBtnScript.m_onClickEventParams.tag = 0;
			CUIEventScript canCelBtnScript = this.CanCelBtnScript;
			canCelBtnScript.onClick = (CUIEventScript.OnUIEventHandler)Delegate.Combine(canCelBtnScript.onClick, new CUIEventScript.OnUIEventHandler(this.ClickHandler));
		}
		this.ConfirmBtnScript = gameObject2.GetComponent<CUIEventScript>();
		if (this.ConfirmBtnScript != null)
		{
			this.ConfirmBtnScript.m_onClickEventParams.tag = 1;
			CUIEventScript confirmBtnScript = this.ConfirmBtnScript;
			confirmBtnScript.onClick = (CUIEventScript.OnUIEventHandler)Delegate.Combine(confirmBtnScript.onClick, new CUIEventScript.OnUIEventHandler(this.ClickHandler));
		}
		base.Initialize();
	}

	protected override void ClickHandler(CUIEvent uiEvt)
	{
		if (this.CanCelBtnScript != null)
		{
			CUIEventScript canCelBtnScript = this.CanCelBtnScript;
			canCelBtnScript.onClick = (CUIEventScript.OnUIEventHandler)Delegate.Remove(canCelBtnScript.onClick, new CUIEventScript.OnUIEventHandler(this.ClickHandler));
		}
		if (this.ConfirmBtnScript != null)
		{
			CUIEventScript confirmBtnScript = this.ConfirmBtnScript;
			confirmBtnScript.onClick = (CUIEventScript.OnUIEventHandler)Delegate.Remove(confirmBtnScript.onClick, new CUIEventScript.OnUIEventHandler(this.ClickHandler));
		}
		if (uiEvt.m_eventParams.tag == 1)
		{
			this.openTrainEntry();
			this.CompleteHandler();
		}
		else
		{
			this.CompleteAllHandler();
		}
	}

	private void openTrainEntry()
	{
		CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().GetForm(CMatchingSystem.PATH_MATCHING_ENTRY);
		if (cUIFormScript == null)
		{
			cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CMatchingSystem.PATH_MATCHING_ENTRY, false, true);
		}
		cUIFormScript.GetWidget(3).CustomSetActive(false);
		cUIFormScript.GetWidget(4).CustomSetActive(false);
		cUIFormScript.GetWidget(2).CustomSetActive(false);
		cUIFormScript.GetWidget(5).CustomSetActive(false);
		cUIFormScript.GetWidget(6).CustomSetActive(false);
		cUIFormScript.GetWidget(7).CustomSetActive(false);
		cUIFormScript.GetWidget(9).CustomSetActive(true);
		cUIFormScript.GetWidget(10).CustomSetActive(false);
		Singleton<CMatchingSystem>.instance.ShowBonusImage(cUIFormScript);
	}
}
