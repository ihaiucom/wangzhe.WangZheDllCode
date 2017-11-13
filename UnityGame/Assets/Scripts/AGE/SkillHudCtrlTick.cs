using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameSystem;
using System;

namespace AGE
{
	[EventCategory("MMGame/Drama")]
	public class SkillHudCtrlTick : TickEvent
	{
		public bool bSkillSlot;

		public SkillSlotType SlotType;

		public bool bAllSkillSlots;

		public bool bHighlight;

		public bool bHighlightLearnBtn;

		public bool bActivate;

		public bool bShow;

		public bool bShowLearnBtn;

		public bool bYes;

		public bool bNoActivatingOthers;

		public bool bPauseGame;

		public bool bJoystick;

		public bool bHighlightJoystick;

		public bool bPlayerShowAnim;

		public enRestSkillSlotType restSkillBtnType;

		public bool bHideOtherBtn;

		public bool bHighlightOterBtn;

		public bool bRecordUseTime;

		public int recordTimeId;

		public override bool SupportEditMode()
		{
			return true;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			SkillHudCtrlTick skillHudCtrlTick = src as SkillHudCtrlTick;
			this.bSkillSlot = skillHudCtrlTick.bSkillSlot;
			this.SlotType = skillHudCtrlTick.SlotType;
			this.bAllSkillSlots = skillHudCtrlTick.bAllSkillSlots;
			this.bHighlight = skillHudCtrlTick.bHighlight;
			this.bHighlightLearnBtn = skillHudCtrlTick.bHighlightLearnBtn;
			this.bActivate = skillHudCtrlTick.bActivate;
			this.bShow = skillHudCtrlTick.bShow;
			this.bShowLearnBtn = skillHudCtrlTick.bShowLearnBtn;
			this.bYes = skillHudCtrlTick.bYes;
			this.bPauseGame = skillHudCtrlTick.bPauseGame;
			this.bNoActivatingOthers = skillHudCtrlTick.bNoActivatingOthers;
			this.bJoystick = skillHudCtrlTick.bJoystick;
			this.bHighlightJoystick = skillHudCtrlTick.bHighlightJoystick;
			this.restSkillBtnType = skillHudCtrlTick.restSkillBtnType;
			this.bHideOtherBtn = skillHudCtrlTick.bHideOtherBtn;
			this.bHighlightOterBtn = skillHudCtrlTick.bHighlightOterBtn;
			this.bPlayerShowAnim = skillHudCtrlTick.bPlayerShowAnim;
			this.bRecordUseTime = skillHudCtrlTick.bRecordUseTime;
			this.recordTimeId = skillHudCtrlTick.recordTimeId;
		}

		public override BaseEvent Clone()
		{
			SkillHudCtrlTick skillHudCtrlTick = ClassObjPool<SkillHudCtrlTick>.Get();
			skillHudCtrlTick.CopyData(this);
			return skillHudCtrlTick;
		}

		private void ProcessSkillSlot()
		{
			if (!this.bSkillSlot)
			{
				return;
			}
			if (this.bShow)
			{
				Singleton<BattleSkillHudControl>.GetInstance().Show(this.SlotType, this.bYes, this.bAllSkillSlots, this.bPlayerShowAnim);
			}
			if (this.bShowLearnBtn)
			{
				Singleton<BattleSkillHudControl>.GetInstance().ShowLearnBtn(this.SlotType, this.bYes, this.bAllSkillSlots);
			}
			if (this.bActivate)
			{
				Singleton<BattleSkillHudControl>.GetInstance().Activate(this.SlotType, this.bYes, this.bAllSkillSlots);
			}
			if (this.bHighlight)
			{
				Singleton<BattleSkillHudControl>.GetInstance().Highlight(this.SlotType, this.bYes, this.bAllSkillSlots, !this.bNoActivatingOthers, this.bPauseGame, this.bRecordUseTime, this.recordTimeId);
			}
			if (this.bHighlightLearnBtn)
			{
				Singleton<BattleSkillHudControl>.GetInstance().HighlightLearnBtn(this.SlotType, this.bYes, this.bAllSkillSlots, !this.bNoActivatingOthers, this.bPauseGame, this.bRecordUseTime, this.recordTimeId);
			}
			if (this.bHideOtherBtn)
			{
				Singleton<BattleSkillHudControl>.GetInstance().ShowRestkSkillBtn(this.restSkillBtnType, this.bYes);
			}
			if (this.bHighlightOterBtn)
			{
				Singleton<BattleSkillHudControl>.GetInstance().HighlishtRestSkillBtn(this.restSkillBtnType, this.bHighlightOterBtn, !this.bNoActivatingOthers, this.bPauseGame, this.bRecordUseTime, this.recordTimeId);
			}
		}

		private void ProcessJoystick()
		{
			if (this.bJoystick)
			{
				Singleton<BattleSkillHudControl>.GetInstance().HighlightJoystick(this.bHighlightJoystick);
			}
		}

		public override void Process(Action _action, Track _track)
		{
			if (Singleton<CBattleSystem>.GetInstance().IsFormOpen)
			{
				this.ProcessJoystick();
				this.ProcessSkillSlot();
			}
		}
	}
}
