using System;
using UnityEngine;

public class NewbieGuideWaitSomeTime : NewbieGuideBaseScript
{
	private int m_timer;

	protected override void Initialize()
	{
		base.AddHighlightWaiting();
		this.m_timer = (int)base.currentConf.Param[0];
		base.Initialize();
	}

	protected override bool IsDelegateClickEvent()
	{
		return true;
	}

	protected override void Update()
	{
		base.Update();
		float num = Time.deltaTime;
		num *= 1000f;
		this.m_timer -= (int)num;
		if (this.m_timer <= 0)
		{
			this.CompleteHandler();
		}
	}
}
