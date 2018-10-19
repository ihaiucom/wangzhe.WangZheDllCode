using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

namespace AGE
{
	[EventCategory("MMGame/Drama")]
	public class BattleUiSwitcherTick : TickEvent
	{
		public bool bOpenOrClose;

		public bool bIncludeBattleUi = true;

		public bool bIncludeBattleHero;

		public bool bIncludeFpsForm;

		public override bool SupportEditMode()
		{
			return true;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			BattleUiSwitcherTick battleUiSwitcherTick = src as BattleUiSwitcherTick;
			this.bOpenOrClose = battleUiSwitcherTick.bOpenOrClose;
			this.bIncludeBattleUi = battleUiSwitcherTick.bIncludeBattleUi;
			this.bIncludeBattleHero = battleUiSwitcherTick.bIncludeBattleHero;
			this.bIncludeFpsForm = battleUiSwitcherTick.bIncludeFpsForm;
		}

		public override BaseEvent Clone()
		{
			BattleUiSwitcherTick battleUiSwitcherTick = ClassObjPool<BattleUiSwitcherTick>.Get();
			battleUiSwitcherTick.CopyData(this);
			return battleUiSwitcherTick;
		}

		public override void Process(Action _action, Track _track)
		{
			if (this.bIncludeBattleUi)
			{
				CUIFormScript fightFormScript = Singleton<CBattleSystem>.GetInstance().FightFormScript;
				if (fightFormScript)
				{
					if (this.bOpenOrClose)
					{
						fightFormScript.Appear(enFormHideFlag.HideByCustom, true);
						if (GameSettings.EnableOutline)
						{
							Transform y = Camera.main.transform.Find(Camera.main.name + " particles");
							if (null != y)
							{
							}
						}
					}
					else
					{
						fightFormScript.Hide(enFormHideFlag.HideByCustom, true);
						if (GameSettings.EnableOutline)
						{
							Transform y2 = Camera.main.transform.Find(Camera.main.name + " particles");
							if (null != y2)
							{
							}
						}
					}
				}
			}
			if (this.bIncludeBattleHero)
			{
				CUIFormScript formScript = Singleton<CBattleHeroInfoPanel>.GetInstance().m_FormScript;
				if (formScript)
				{
					if (this.bOpenOrClose)
					{
						formScript.Appear(enFormHideFlag.HideByCustom, true);
					}
					else
					{
						formScript.Hide(enFormHideFlag.HideByCustom, true);
					}
				}
			}
			if (this.bIncludeFpsForm)
			{
				CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CUICommonSystem.FPS_FORM_PATH);
				if (form)
				{
					if (this.bOpenOrClose)
					{
						form.Appear(enFormHideFlag.HideByCustom, true);
					}
					else
					{
						form.Hide(enFormHideFlag.HideByCustom, true);
					}
				}
			}
		}
	}
}
