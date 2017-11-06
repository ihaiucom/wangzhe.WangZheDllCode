using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

internal class NewbieGuideClickAddedSkillForBattle : NewbieGuideBaseScript
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
			int index = (int)base.currentConf.Param[0];
			Transform transform = form.transform.FindChild("PanelAddSkill/ToggleList");
			if (transform != null)
			{
				CUIToggleListScript component = transform.gameObject.GetComponent<CUIToggleListScript>();
				if (component != null)
				{
					component.MoveElementInScrollArea(index, true);
					CUIToggleListElementScript cUIToggleListElementScript = component.GetElemenet(index) as CUIToggleListElementScript;
					if (cUIToggleListElementScript != null)
					{
						GameObject gameObject = cUIToggleListElementScript.transform.gameObject;
						if (gameObject.activeInHierarchy)
						{
							base.AddHighLightGameObject(gameObject, true, form, true);
							base.Initialize();
						}
					}
				}
			}
		}
	}

	protected override bool IsDelegateClickEvent()
	{
		return true;
	}
}
