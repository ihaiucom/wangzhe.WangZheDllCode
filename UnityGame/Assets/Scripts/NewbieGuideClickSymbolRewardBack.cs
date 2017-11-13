using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

public class NewbieGuideClickSymbolRewardBack : NewbieGuideBaseScript
{
	private const float DelayTimer = 2f;

	private float m_timer;

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
		this.m_timer += Time.deltaTime;
		if (this.m_timer < 2f)
		{
			return;
		}
		CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CUICommonSystem.s_newSymbolFormPath);
		if (form != null)
		{
			Transform transform = form.transform.Find("Btn_Continue");
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
