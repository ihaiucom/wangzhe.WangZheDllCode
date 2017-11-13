using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

public class NewbieGuideClickVoice : NewbieGuideBaseScript
{
	private float timeToWait = 2f;

	private float timer;

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
		this.timer += Time.deltaTime;
		if (this.timer < this.timeToWait)
		{
			return;
		}
		CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(FightForm.s_battleUIForm);
		if (form == null)
		{
			return;
		}
		GameObject widget = form.GetWidget(54);
		base.AddHighLightGameObject(widget, true, form, true);
		base.Initialize();
	}

	protected override bool IsDelegateClickEvent()
	{
		return true;
	}
}
