using Assets.Scripts.GameLogic;
using System;

public class NewbieGuideShowImageGuide : NewbieGuideBaseScript
{
	protected override void Initialize()
	{
		base.Initialize();
	}

	protected override void Update()
	{
		Singleton<CBattleGuideManager>.GetInstance().OpenBannerDlgByBannerGuideId((uint)base.currentConf.Param[0], null, false);
		this.CompleteHandler();
	}

	protected override bool IsDelegateClickEvent()
	{
		return true;
	}
}
