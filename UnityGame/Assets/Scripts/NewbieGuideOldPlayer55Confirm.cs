using Assets.Scripts.UI;
using System;

public class NewbieGuideOldPlayer55Confirm : NewbieGuideBaseScript
{
	protected override void Initialize()
	{
		base.Initialize();
		Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("TrainLevel_Tips0"), enUIEventID.Matching_Guide_5v5, enUIEventID.None, false);
		this.CompleteAllHandler();
	}
}
