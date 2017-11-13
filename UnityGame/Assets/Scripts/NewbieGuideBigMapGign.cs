using Assets.Scripts.GameLogic;
using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

internal class NewbieGuideBigMapGign : NewbieGuideBaseScript
{
	private const int SKIP_TIME = 10000;

	private CUIEventScript eventScript;

	private GameObject info;

	private GameObject highlighter;

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
		CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(FightForm.s_battleUIForm);
		if (form != null)
		{
			Transform transform = form.GetWidget(71).transform;
			this.info = transform.FindChild("info").gameObject;
			this.highlighter = transform.FindChild("highlighter").gameObject;
			if (this.info && this.highlighter)
			{
				this.info.CustomSetActive(true);
				this.highlighter.CustomSetActive(true);
				this.eventScript = transform.GetComponent<CUIEventScript>();
				CUIEventScript cUIEventScript = this.eventScript;
				cUIEventScript.onClick = (CUIEventScript.OnUIEventHandler)Delegate.Combine(cUIEventScript.onClick, new CUIEventScript.OnUIEventHandler(this.ClickHandler));
				Singleton<CTimerManager>.GetInstance().AddTimer(10000, 1, new CTimer.OnTimeUpHandler(this.OnTimeUp));
			}
		}
		base.isInitialize = true;
		base.isGuiding = true;
		NewbieGuideScriptControl.CloseGuideForm();
	}

	protected override void ClickHandler(CUIEvent uiEvent)
	{
		CUIEventScript cUIEventScript = this.eventScript;
		cUIEventScript.onClick = (CUIEventScript.OnUIEventHandler)Delegate.Remove(cUIEventScript.onClick, new CUIEventScript.OnUIEventHandler(this.ClickHandler));
		this.info.CustomSetActive(false);
		this.highlighter.CustomSetActive(false);
		Singleton<CBattleGuideManager>.GetInstance().OpenFormShared(CBattleGuideManager.EBattleGuideFormType.BigMapGuide, 1500, true);
		this.CompleteHandler();
	}

	private void OnTimeUp(int delt)
	{
		CUIEventScript cUIEventScript = this.eventScript;
		cUIEventScript.onClick = (CUIEventScript.OnUIEventHandler)Delegate.Remove(cUIEventScript.onClick, new CUIEventScript.OnUIEventHandler(this.ClickHandler));
		this.info.CustomSetActive(false);
		this.highlighter.CustomSetActive(false);
		MonoSingleton<NewbieGuideManager>.GetInstance().StopCurrentGuide();
	}

	protected override bool IsDelegateClickEvent()
	{
		return true;
	}

	public override bool IsTimeOutSkip()
	{
		return false;
	}

	protected override bool IsShowGuideMask()
	{
		return false;
	}
}
