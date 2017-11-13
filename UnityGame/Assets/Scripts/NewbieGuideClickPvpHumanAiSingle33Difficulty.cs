using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

public class NewbieGuideClickPvpHumanAiSingle33Difficulty : NewbieGuideBaseScript
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
		CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CMatchingSystem.PATH_MATCHING_ENTRY);
		if (form != null)
		{
			uint num = (uint)base.currentConf.Param[0];
			string name = string.Format("panelGroup3/btnGroup/Button3/btnGrp/Button{0}", num);
			Transform transform = form.transform.FindChild(name);
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
