using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

public class NewbieGuideClickChallenge : NewbieGuideBaseScript
{
	private CUIStepListScript m_stepList;

	protected override void Initialize()
	{
	}

	protected override void Update()
	{
		if (base.isInitialize)
		{
			base.Update();
			if (this.m_stepList != null)
			{
				int count = NewbieGuideBaseScript.ms_highlitGo.Count;
				for (int i = 0; i < count; i++)
				{
					GameObject gameObject = NewbieGuideBaseScript.ms_highlitGo[i];
					GameObject gameObject2 = NewbieGuideBaseScript.ms_originalGo[i];
					RectTransform rectTransform = gameObject.transform as RectTransform;
					rectTransform.localScale = gameObject2.transform.localScale;
					rectTransform.localScale *= 1.2f;
				}
			}
			return;
		}
		CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CAdventureSys.EXLPORE_FORM_PATH);
		if (form != null)
		{
			Transform transform = form.transform.FindChild("ExploreList");
			if (transform != null)
			{
				this.m_stepList = transform.gameObject.GetComponent<CUIStepListScript>();
				int num = (int)base.currentConf.Param[0];
				this.m_stepList.SelectElementImmediately(num);
				CUIListElementScript elemenet = this.m_stepList.GetElemenet(num);
				if (elemenet != null)
				{
					GameObject gameObject3 = elemenet.gameObject;
					if (gameObject3.activeInHierarchy)
					{
						if (num == 1)
						{
							Singleton<CAdventureSys>.instance.currentDifficulty = 1;
							Singleton<CAdventureSys>.instance.currentChapter = 1;
							Singleton<CAdventureSys>.instance.currentLevelSeq = 1;
						}
						base.AddHighLightGameObject(gameObject3, true, form, true);
						base.Initialize();
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
