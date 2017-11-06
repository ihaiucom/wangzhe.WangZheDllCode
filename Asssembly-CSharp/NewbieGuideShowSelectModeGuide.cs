using Assets.Scripts.GameLogic;
using System;

internal class NewbieGuideShowSelectModeGuide : NewbieGuideBaseScript
{
	protected override void Initialize()
	{
		Singleton<CBattleGuideManager>.GetInstance().OpenFormShared(CBattleGuideManager.EBattleGuideFormType.SelectModeGuide, 5000, true);
		this.CompleteHandler();
	}

	protected override bool IsDelegateClickEvent()
	{
		return true;
	}
}
