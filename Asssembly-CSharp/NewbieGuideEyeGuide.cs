using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

internal class NewbieGuideEyeGuide : NewbieGuideBaseScript
{
	private Transform guidePanel;

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
		CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(FightForm.s_skillBtnFormPath);
		if (form != null)
		{
			this.guidePanel = form.transform.FindChild("AtkSkill/SkillBtn7/Panel_Guide");
			if (this.guidePanel != null)
			{
				this.guidePanel.gameObject.CustomSetActive(true);
				CUICommonSystem.SetTextContent(this.guidePanel.FindChild("Text").gameObject, Singleton<CTextManager>.GetInstance().GetText("NewbieGuide_EyeGuide_Txt"));
				Singleton<CTimerManager>.GetInstance().AddTimer(10000, 1, new CTimer.OnTimeUpHandler(this.hideGuidePanel));
				base.Initialize();
			}
		}
	}

	private void hideGuidePanel(int index)
	{
		Singleton<CTimerManager>.GetInstance().RemoveTimer(new CTimer.OnTimeUpHandler(this.hideGuidePanel));
		if (this.guidePanel != null)
		{
			this.guidePanel.gameObject.CustomSetActive(false);
			this.CompleteHandler();
		}
	}

	protected override bool IsDelegateClickEvent()
	{
		return true;
	}

	protected override bool IsShowGuideMask()
	{
		return false;
	}
}
