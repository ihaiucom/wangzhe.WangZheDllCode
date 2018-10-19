using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

internal class NewbieGuideShowHighlightPrefabeText : NewbieGuideBaseScript
{
	private const int SKIP_TIME = 15000;

	private CUIEventScript eventScript;

	private GameObject info;

	private GameObject highlighter;

	private int timeSeq;

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
			Transform transform = form.GetWidget(54).transform;
			this.info = transform.FindChild("info").gameObject;
			this.highlighter = transform.FindChild("highlighter").gameObject;
			if (this.info && this.highlighter)
			{
				this.info.CustomSetActive(true);
				this.highlighter.CustomSetActive(true);
				this.eventScript = transform.GetComponent<CUIEventScript>();
				CUIEventScript expr_B2 = this.eventScript;
				expr_B2.onClick = (CUIEventScript.OnUIEventHandler)Delegate.Combine(expr_B2.onClick, new CUIEventScript.OnUIEventHandler(this.ClickHandler));
				Singleton<CTimerManager>.GetInstance().AddTimer(15000, 1, new CTimer.OnTimeUpHandler(this.OnTimeUp));
			}
		}
		base.isInitialize = true;
		base.isGuiding = true;
		NewbieGuideScriptControl.CloseGuideForm();
	}

	protected override void ClickHandler(CUIEvent uiEvent)
	{
		CUIEventScript expr_06 = this.eventScript;
		expr_06.onClick = (CUIEventScript.OnUIEventHandler)Delegate.Remove(expr_06.onClick, new CUIEventScript.OnUIEventHandler(this.ClickHandler));
		this.info.CustomSetActive(false);
		this.highlighter.CustomSetActive(false);
		this.CompleteHandler();
	}

	private void OnTimeUp(int delt)
	{
		CUIEventScript expr_06 = this.eventScript;
		expr_06.onClick = (CUIEventScript.OnUIEventHandler)Delegate.Remove(expr_06.onClick, new CUIEventScript.OnUIEventHandler(this.ClickHandler));
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
}
