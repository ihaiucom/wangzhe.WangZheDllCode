using Assets.Scripts.GameLogic;
using Assets.Scripts.UI;
using System;

internal class NewbieGuideIntroFireMatch : NewbieGuideBaseScript
{
	protected override void Initialize()
	{
		CUIEvent cUIEvent = new CUIEvent();
		cUIEvent.m_eventID = enUIEventID.MatchingExt_BeginEnterTrainMent;
		cUIEvent.m_eventParams.commonBool = true;
		uint.TryParse(Singleton<CTextManager>.instance.GetText("MapID_PVP_Fire"), out cUIEvent.m_eventParams.tagUInt);
		string[] imgPath = new string[]
		{
			string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Newbie_Dir, "huokeng1"),
			string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Newbie_Dir, "huokeng2"),
			string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Newbie_Dir, "huokeng3")
		};
		Singleton<CBattleGuideManager>.GetInstance().OpenBannerIntroDialog(0, imgPath, 3, cUIEvent, null, null, true, false);
		this.CompleteHandler();
	}

	protected override bool IsDelegateClickEvent()
	{
		return true;
	}
}
