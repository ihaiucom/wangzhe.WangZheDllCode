using Assets.Scripts.GameLogic;
using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

internal class NewbieGuideClickCaptainButton : NewbieGuideBaseScript
{
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
		CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(FightForm.s_battleUIForm);
		if (form != null)
		{
			string name = string.Format("HeroHeadHud/HeroHead{0}", base.currentConf.Param[0]);
			Transform transform = form.transform.FindChild(name);
			if (transform != null)
			{
				GameObject gameObject = transform.gameObject;
				if (gameObject.activeInHierarchy)
				{
					base.AddHighLightGameObject(gameObject, true, form, true);
					if (NewbieGuideBaseScript.ms_originalGo.Count > 0 && NewbieGuideBaseScript.ms_highlitGo.Count > 0)
					{
						PlayerHead component = NewbieGuideBaseScript.ms_originalGo[0].GetComponent<PlayerHead>();
						PlayerHead component2 = NewbieGuideBaseScript.ms_highlitGo[0].GetComponent<PlayerHead>();
						if (component != null && component2 != null)
						{
							component2.SetPrivates(component.state, component.MyHero, component.HudOwner);
						}
					}
					base.Initialize();
				}
				else
				{
					this.CompleteHandler();
				}
			}
		}
	}

	protected override bool IsDelegateClickEvent()
	{
		return true;
	}
}
