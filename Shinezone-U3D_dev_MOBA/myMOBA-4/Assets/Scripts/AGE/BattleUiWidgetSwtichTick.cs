using Assets.Scripts.Common;
using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using UnityEngine;

namespace AGE
{
	[EventCategory("MMGame/Drama")]
	public class BattleUiWidgetSwtichTick : TickEvent
	{
		public bool bTurnOn;

		public bool bAutoAi;

		public bool bFreeCamera;

		public bool bSettingMenu;

		public bool bBattleInfoView;

		public bool bPauseBtn;

		public bool bResumeBtn;

		public bool bTrainingExit;

		public bool bDetailInfo;

		public override bool SupportEditMode()
		{
			return true;
		}

		private void EnableBattleUiByName(bool bInEnable, string inName)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(FightForm.s_battleUIForm);
			if (form != null)
			{
				Transform transform = form.transform.FindChild(inName);
				GameObject gameObject = (!(transform != null)) ? null : transform.gameObject;
				if (gameObject != null)
				{
					gameObject.CustomSetActive(bInEnable);
				}
			}
		}

		private void EnableTrainingExit(bool bInEnable)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(FightForm.s_battleScene);
			if (form != null)
			{
				Transform transform = form.transform.FindChild("Image/ButtonTrainingExit");
				GameObject gameObject = (!(transform != null)) ? null : transform.gameObject;
				if (gameObject != null)
				{
					gameObject.CustomSetActive(bInEnable);
				}
			}
		}

		private void EnableToggleAuto(bool bInEnable)
		{
			this.EnableBattleUiByName(bInEnable, "PVPTopRightPanel/PanelBtn/ToggleAutoBtn");
		}

		private void EnableBattleInfoView(bool bInEnable)
		{
			this.EnableBattleUiByName(bInEnable, "PVPTopRightPanel/PanelBtn/btnViewBattleInfo");
		}

		private void EnableToggleFreeCam(bool bInEnable)
		{
		}

		private void EnableSettingMenu(bool bInEnable)
		{
			this.EnableBattleUiByName(bInEnable, "PVPTopRightPanel/PanelBtn/MenuBtn");
		}

		private void EnablePauseBtn(bool bInEnable)
		{
			this.EnableBattleUiByName(bInEnable, "PVPTopRightPanel/panelTopRight/PauseBtn");
		}

		private void EnableResumeBtn(bool bInEnable)
		{
			this.EnableBattleUiByName(bInEnable, "PVPTopRightPanel/panelTopRight/ResumeBtn");
		}

		private void EnableDetailInfoBtn(bool bInEnable)
		{
			this.EnableBattleUiByName(bInEnable, "PVPTopRightPanel/ButtonViewSkillInfo");
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			BattleUiWidgetSwtichTick battleUiWidgetSwtichTick = src as BattleUiWidgetSwtichTick;
			this.bAutoAi = battleUiWidgetSwtichTick.bAutoAi;
			this.bFreeCamera = battleUiWidgetSwtichTick.bFreeCamera;
			this.bSettingMenu = battleUiWidgetSwtichTick.bSettingMenu;
			this.bBattleInfoView = battleUiWidgetSwtichTick.bBattleInfoView;
			this.bPauseBtn = battleUiWidgetSwtichTick.bPauseBtn;
			this.bResumeBtn = battleUiWidgetSwtichTick.bResumeBtn;
			this.bTrainingExit = battleUiWidgetSwtichTick.bTrainingExit;
			this.bDetailInfo = battleUiWidgetSwtichTick.bDetailInfo;
			this.bTurnOn = battleUiWidgetSwtichTick.bTurnOn;
		}

		public override BaseEvent Clone()
		{
			BattleUiWidgetSwtichTick battleUiWidgetSwtichTick = ClassObjPool<BattleUiWidgetSwtichTick>.Get();
			battleUiWidgetSwtichTick.CopyData(this);
			return battleUiWidgetSwtichTick;
		}

		public override void Process(Action _action, Track _track)
		{
			base.Process(_action, _track);
			if (this.bAutoAi)
			{
				this.EnableToggleAuto(this.bTurnOn);
			}
			if (this.bFreeCamera)
			{
				this.EnableToggleFreeCam(this.bTurnOn);
			}
			if (this.bSettingMenu)
			{
				this.EnableSettingMenu(this.bTurnOn);
			}
			if (this.bBattleInfoView)
			{
				this.EnableBattleInfoView(this.bTurnOn);
			}
			if (this.bPauseBtn)
			{
				this.EnablePauseBtn(this.bTurnOn);
			}
			if (this.bResumeBtn)
			{
				this.EnableResumeBtn(this.bTurnOn);
			}
			if (this.bTrainingExit)
			{
				this.EnableTrainingExit(this.bTurnOn);
			}
			if (this.bDetailInfo)
			{
				this.EnableDetailInfoBtn(this.bTurnOn);
			}
		}
	}
}
