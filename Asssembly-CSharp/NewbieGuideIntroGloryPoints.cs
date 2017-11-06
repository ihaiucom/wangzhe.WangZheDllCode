using Assets.Scripts.GameLogic;
using Assets.Scripts.UI;
using System;

internal class NewbieGuideIntroGloryPoints : NewbieGuideBaseScript
{
	protected override void Initialize()
	{
		string text = Singleton<CTextManager>.GetInstance().GetText("Tutorial_Old_Glory_Title");
		string text2 = Singleton<CTextManager>.GetInstance().GetText("Tutorial_Old_Glory_Button");
		string[] imgPath = new string[]
		{
			string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Newbie_Dir, "Newbie_Honor1"),
			string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Newbie_Dir, "Newbie_Honor2")
		};
		Singleton<CBattleGuideManager>.GetInstance().OpenBannerIntroDialog(0, imgPath, 2, null, text, text2, true, false);
		this.CompleteHandler();
	}

	protected override bool IsDelegateClickEvent()
	{
		return true;
	}
}
