using Assets.Scripts.GameLogic;
using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

public class NewbieGuide5v5GuideConfirm : NewbieGuideBaseScript
{
	private CUIFormScript Guide5v5ConfirmForm;

	private CUIEventScript ConfirmBtnScript;

	protected override void Initialize()
	{
		this.Guide5v5ConfirmForm = Singleton<CUIManager>.GetInstance().OpenForm(NewbieGuideManager.FORM_5v5GUIDE_CONFIRM, false, true);
		GameObject gameObject = this.Guide5v5ConfirmForm.transform.FindChild("Bg/btnConfirm").gameObject;
		DebugHelper.Assert(gameObject != null, string.Format("{0}can't find cancel and confirm button in {1}", base.logTitle, NewbieGuideManager.FORM_5v5GUIDE_CONFIRM));
		this.ConfirmBtnScript = gameObject.GetComponent<CUIEventScript>();
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
		if (this.ConfirmBtnScript != null)
		{
			CUIEventScript confirmBtnScript = this.ConfirmBtnScript;
			confirmBtnScript.onClick = (CUIEventScript.OnUIEventHandler)Delegate.Remove(confirmBtnScript.onClick, new CUIEventScript.OnUIEventHandler(this.ClickHandler));
		}
		if (uiEvt.m_eventParams.tag == 1)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			this.CompleteHandler();
			LobbyLogic.ReqStartGuideLevel55(false, (uint)masterRoleInfo.acntMobaInfo.iSelectedHeroType);
			masterRoleInfo.SetNewbieAchieve(61, true, true);
		}
	}

	protected override bool IsShowGuideMask()
	{
		return false;
	}
}
