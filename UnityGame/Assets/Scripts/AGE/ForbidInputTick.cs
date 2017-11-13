using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

namespace AGE
{
	[EventCategory("MMGame/Drama")]
	public class ForbidInputTick : TickEvent
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int srcId;

		public bool bForbid = true;

		public override bool SupportEditMode()
		{
			return true;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.srcId = 0;
			this.bForbid = true;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			ForbidInputTick forbidInputTick = src as ForbidInputTick;
			this.srcId = forbidInputTick.srcId;
			this.bForbid = forbidInputTick.bForbid;
		}

		public override BaseEvent Clone()
		{
			ForbidInputTick forbidInputTick = ClassObjPool<ForbidInputTick>.Get();
			forbidInputTick.CopyData(this);
			return forbidInputTick;
		}

		public override void Process(Action _action, Track _track)
		{
			FightForm fightForm = Singleton<CBattleSystem>.GetInstance().FightForm;
			if (fightForm != null)
			{
				GameObject moveJoystick = fightForm.GetMoveJoystick();
				if (moveJoystick == null)
				{
					return;
				}
				bool bActive = !this.bForbid;
				CUIJoystickScript component = moveJoystick.GetComponent<CUIJoystickScript>();
				if (component)
				{
					component.ResetAxis();
				}
				moveJoystick.CustomSetActive(bActive);
				component = moveJoystick.GetComponent<CUIJoystickScript>();
				if (component)
				{
					component.ResetAxis();
				}
				if (this.bForbid && Singleton<CBattleSystem>.GetInstance().FightForm != null && Singleton<CBattleSystem>.GetInstance().FightForm.m_skillButtonManager != null)
				{
					for (int i = 0; i < 5; i++)
					{
						Singleton<CBattleSystem>.GetInstance().FightForm.m_skillButtonManager.SkillButtonUp(Singleton<CBattleSystem>.GetInstance().FightFormScript, (SkillSlotType)i, false, default(Vector2));
					}
				}
			}
		}
	}
}
