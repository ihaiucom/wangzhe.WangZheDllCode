using Assets.Scripts.GameLogic;
using System;

public class NewbieGuideOpenForm : NewbieGuideBaseScript
{
	protected override void Initialize()
	{
		base.Initialize();
		Singleton<CBattleGuideManager>.GetInstance().OpenFormShared((CBattleGuideManager.EBattleGuideFormType)base.currentConf.Param[0], 0, false);
		this.CompleteHandler();
	}

	protected override bool IsDelegateClickEvent()
	{
		return true;
	}
}
