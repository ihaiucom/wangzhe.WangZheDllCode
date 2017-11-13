using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

internal class NewbieGuideClickBattleHero : NewbieGuideBaseScript
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
			bool flag = base.currentConf.Param[1] > 0;
			bool flag2 = base.currentConf.Param[2] > 0;
			if (flag2)
			{
				uint heroID = (uint)base.currentConf.Param[0];
				index = Singleton<CHeroSelectBaseSystem>.instance.GetCanUseHeroIndex(heroID);
			}
			string name = "PanelLeft/ListHostHeroInfo";
			if (flag)
			{
				name = "PanelLeft/ListHostHeroInfoFull";
			}
			Transform transform = form.transform.FindChild(name);
			if (transform != null)
			{
				CUIListScript component = transform.gameObject.GetComponent<CUIListScript>();
				if (component != null)
				{
					CUIListElementScript elemenet = component.GetElemenet(index);
					if (elemenet != null)
					{
						Transform transform2 = elemenet.transform.Find("heroItemCell");
						if (transform2 != null)
						{
							GameObject gameObject = transform2.gameObject;
							if (gameObject.activeInHierarchy)
							{
								CUIEventScript component2 = gameObject.GetComponent<CUIEventScript>();
								if (component2 != null && !component2.enabled)
								{
									this.CompleteHandler();
								}
								else
								{
									base.AddHighLightGameObject(gameObject, true, form, true);
									base.Initialize();
								}
							}
						}
					}
					else
					{
						this.CompleteHandler();
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
