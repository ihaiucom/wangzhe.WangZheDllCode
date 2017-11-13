using System;

public class NewbieGuideClickAnyWhereScreen : NewbieGuideBaseScript
{
	protected override void Initialize()
	{
		base.AddHighLightAnyWhere();
		base.Initialize();
	}

	protected override bool IsDelegateClickEvent()
	{
		return true;
	}
}
