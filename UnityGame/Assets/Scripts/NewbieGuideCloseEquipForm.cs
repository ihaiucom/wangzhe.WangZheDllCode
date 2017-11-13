using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

public class NewbieGuideCloseEquipForm : NewbieGuideBaseScript
{
	private float m_timer;

	protected override void Initialize()
	{
		base.AddHighLightAnyWhere();
		this.m_timer = (float)base.currentConf.Param[0];
		base.Initialize();
	}

	protected override void Update()
	{
		base.Update();
		this.m_timer -= Time.deltaTime;
		if (this.m_timer <= 0f)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CBattleEquipSystem.s_equipFormPath);
			if (form != null)
			{
				Singleton<CUIManager>.GetInstance().CloseForm(form);
				this.CompleteHandler();
			}
		}
	}
}
