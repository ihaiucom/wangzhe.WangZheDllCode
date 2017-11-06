using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

public class NewbieGuideClickOneLevel : NewbieGuideBaseScript
{
	protected override void Initialize()
	{
		CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CAdventureSys.ADVENTURE_SELECT_FORM);
		if (form == null)
		{
			return;
		}
		int num = (int)base.currentConf.Param[0];
		string name = string.Format("LevelList/ScrollRect/Content/ListElement_{0}", num);
		GameObject gameObject = form.transform.FindChild(name).gameObject;
		base.AddHighLightGameObject(gameObject, true, form, true);
		base.Initialize();
	}

	protected override bool IsDelegateClickEvent()
	{
		return true;
	}
}
