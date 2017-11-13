using Assets.Scripts.GameLogic;
using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;

internal class NewbieGuideOldPlayerGrow : NewbieGuideBaseScript
{
	protected override void Initialize()
	{
		CTaskView taskView = Singleton<CTaskSys>.GetInstance().GetTaskView();
		if (taskView != null)
		{
			taskView.On_Tab_Change(0);
		}
		string text = Singleton<CTextManager>.GetInstance().GetText("Tutorial_Old_Task_Title");
		string text2 = Singleton<CTextManager>.GetInstance().GetText("Tutorial_Old_Task_Button");
		string[] imgPath = new string[]
		{
			string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Newbie_Dir, "Newbie_Growth1"),
			string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Newbie_Dir, "Newbie_Growth2")
		};
		Singleton<CBattleGuideManager>.GetInstance().OpenBannerIntroDialog(0, imgPath, 2, null, text, text2, true, false);
		this.CompleteHandler();
	}

	protected override bool IsDelegateClickEvent()
	{
		return true;
	}
}
