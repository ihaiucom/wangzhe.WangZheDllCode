using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

internal class NewbieGuideClickBattleHeroAddedSkillSwitch : NewbieGuideBaseScript
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
		CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CHeroSelectNormalSystem.s_heroSelectFormPath);
		if (form != null)
		{
			Transform transform = form.transform.FindChild("Other/SkillList/AddedSkillItem");
			if (transform != null)
			{
				GameObject gameObject = transform.gameObject;
				if (gameObject.activeInHierarchy)
				{
					base.AddHighLightGameObject(gameObject, true, form, true);
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
