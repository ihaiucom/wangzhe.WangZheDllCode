using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;

public class NewbieGuideHighLightHeroListArea : NewbieGuideBaseScript
{
	private int _freq;

	protected override void Initialize()
	{
	}

	protected override void Update()
	{
		if (base.isInitialize)
		{
			base.Update();
			return;
		}
		CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(Singleton<CMallSystem>.GetInstance().sMallFormPath);
		if (form != null)
		{
			CUIListScript componetInChild = Utility.GetComponetInChild<CUIListScript>(form.gameObject, "pnlBodyBg/pnlBuyHero/List");
			if (componetInChild != null)
			{
				CUIListElementScript elemenet = componetInChild.GetElemenet(0);
				if (elemenet != null)
				{
					base.AddHighLightAreaClickAnyWhere(elemenet.gameObject, form);
					base.Initialize();
				}
			}
		}
	}

	protected override bool IsDelegateClickEvent()
	{
		return true;
	}
}
