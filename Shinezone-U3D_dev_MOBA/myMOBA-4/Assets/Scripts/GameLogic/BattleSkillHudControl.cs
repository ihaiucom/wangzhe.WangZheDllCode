using Assets.Scripts.Common;
using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameLogic
{
	public class BattleSkillHudControl : Singleton<BattleSkillHudControl>
	{
		private bool[] m_buttonHighLighted = new bool[10];

		private bool[] m_buttonHidden = new bool[10];

		private bool[] m_learnBtnHighLighted = new bool[4];

		public bool[] m_learnBtnHidden = new bool[4];

		private bool[] m_RestSkillBtnHidden = new bool[4];

		private bool[] m_restSkilBtnHighLighted = new bool[4];

		private RecordTime[] m_buttonUseTimeRecord = new RecordTime[10];

		private RecordTime[] m_learnBtnUseTimeRecord = new RecordTime[4];

		private RecordTime[] m_restSkilBtnUseTimeRecord = new RecordTime[4];

		private string m_joystickHighlightName = "Joystick/Axis/Cursor/Highlight";

		public override void Init()
		{
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnSkillButtonUp, new CUIEventManager.OnUIEventHandler(this.OnBattleRequestUseSkill));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnSkillButtonDown, new CUIEventManager.OnUIEventHandler(this.OnBattleSkillBtnDown));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_LearnSkillBtn_Click, new CUIEventManager.OnUIEventHandler(this.OnBattleLearnSkillBtnClick));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnAtkSelectHeroUp, new CUIEventManager.OnUIEventHandler(this.OnAtkSelectHero));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_OnAtkSelectSoldierUp, new CUIEventManager.OnUIEventHandler(this.OnAtkSelectSoldier));
			Singleton<EventRouter>.GetInstance().AddEventHandler<int, bool>("HeroSkillLearnButtonStateChange", new Action<int, bool>(this.onSkillLearnBtnStateChange));
			Singleton<EventRouter>.GetInstance().AddEventHandler("ResetSkillButtonManager", new Action(this.onResetSkillButtonManager));
			Singleton<EventRouter>.GetInstance().AddEventHandler("CommonAttack_Type_Changed", new Action(this.onCommonAttackTypeChange));
		}

		public override void UnInit()
		{
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnSkillButtonUp, new CUIEventManager.OnUIEventHandler(this.OnBattleRequestUseSkill));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnSkillButtonDown, new CUIEventManager.OnUIEventHandler(this.OnBattleSkillBtnDown));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_LearnSkillBtn_Click, new CUIEventManager.OnUIEventHandler(this.OnBattleLearnSkillBtnClick));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnAtkSelectHeroUp, new CUIEventManager.OnUIEventHandler(this.OnAtkSelectHero));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_OnAtkSelectSoldierUp, new CUIEventManager.OnUIEventHandler(this.OnAtkSelectSoldier));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<int, bool>("HeroSkillLearnButtonStateChange", new Action<int, bool>(this.onSkillLearnBtnStateChange));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler("ResetSkillButtonManager", new Action(this.onResetSkillButtonManager));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler("CommonAttack_Type_Changed", new Action(this.onCommonAttackTypeChange));
		}

		private void onResetSkillButtonManager()
		{
			for (int i = 0; i < 10; i++)
			{
				if (this.m_buttonHidden[i])
				{
					Singleton<CBattleSystem>.GetInstance().FightForm.GetButton((SkillSlotType)i).m_button.CustomSetActive(false);
				}
			}
		}

		private void onSkillLearnBtnStateChange(int inSlotType, bool bShow)
		{
			if (this.m_learnBtnHidden[inSlotType] && bShow)
			{
				GameObject learnSkillButton = Singleton<CBattleSystem>.GetInstance().FightForm.GetButton((SkillSlotType)inSlotType).GetLearnSkillButton();
				if (learnSkillButton != null)
				{
					learnSkillButton.CustomSetActive(false);
				}
			}
		}

		private void onCommonAttackTypeChange()
		{
			for (int i = 0; i < 4; i++)
			{
				if (this.m_RestSkillBtnHidden[i])
				{
					GameObject gameObject = this.QueryRestSkillBtn((enRestSkillSlotType)i);
					if (gameObject != null)
					{
						gameObject.CustomSetActive(false);
					}
				}
			}
		}

		public List<GameObject> QuerySkillButtons(SkillSlotType inSlotType, bool bAll)
		{
			List<GameObject> list = new List<GameObject>();
			FightForm fightForm = Singleton<CBattleSystem>.GetInstance().FightForm;
			if (fightForm == null)
			{
				return list;
			}
			if (bAll)
			{
				for (int i = 0; i < 10; i++)
				{
					SkillButton button = fightForm.GetButton((SkillSlotType)i);
					if (button != null)
					{
						list.Add(button.m_button);
					}
				}
			}
			else if (inSlotType < SkillSlotType.SLOT_SKILL_COUNT)
			{
				SkillButton button2 = fightForm.GetButton(inSlotType);
				if (button2 != null)
				{
					list.Add(button2.m_button);
				}
			}
			return list;
		}

		public GameObject QueryRestSkillBtn(enRestSkillSlotType inSlotType)
		{
			CUIFormScript cUIFormScript = (Singleton<CBattleSystem>.GetInstance().FightForm != null) ? Singleton<CBattleSystem>.GetInstance().FightForm.GetSkillBtnFormScript() : null;
			if (cUIFormScript == null)
			{
				return null;
			}
			GameObject result = null;
			switch (inSlotType)
			{
			case enRestSkillSlotType.BTN_SKILL_SELHERO:
				result = cUIFormScript.GetWidget(24);
				break;
			case enRestSkillSlotType.BTN_SKILL_SELSOLDIER:
				result = cUIFormScript.GetWidget(9);
				break;
			case enRestSkillSlotType.BTN_SKILL_LASTHITBTN:
				result = cUIFormScript.GetWidget(25);
				break;
			case enRestSkillSlotType.BTN_SKILL_AttackOrganBtn:
				result = cUIFormScript.GetWidget(33);
				break;
			}
			return result;
		}

		public void Show(SkillSlotType inSlotType, bool bShow, bool bAll, bool bPlayShowAnim = false)
		{
			List<GameObject> list = this.QuerySkillButtons(inSlotType, bAll);
			for (int i = 0; i < list.Count; i++)
			{
				GameObject gameObject = list[i];
				if (gameObject != null)
				{
					gameObject.CustomSetActive(bShow);
					if (bShow && bPlayShowAnim && inSlotType != SkillSlotType.SLOT_SKILL_0)
					{
						Transform transform = gameObject.transform.FindChild("Present");
						if (transform)
						{
							CUICommonSystem.PlayAnimation(transform, enSkillButtonAnimationName.normal.ToString());
						}
					}
				}
			}
			if (!bShow && Singleton<CBattleSystem>.GetInstance().FightForm != null && Singleton<CBattleSystem>.GetInstance().FightForm.m_skillButtonManager != null)
			{
				if (bAll)
				{
					for (int j = 0; j < 10; j++)
					{
						Singleton<CBattleSystem>.GetInstance().FightForm.m_skillButtonManager.SkillButtonUp(Singleton<CBattleSystem>.GetInstance().FightFormScript, (SkillSlotType)j, false, default(Vector2));
					}
				}
				else
				{
					Singleton<CBattleSystem>.GetInstance().FightForm.m_skillButtonManager.SkillButtonUp(Singleton<CBattleSystem>.GetInstance().FightFormScript, inSlotType, false, default(Vector2));
				}
			}
			if (inSlotType < SkillSlotType.SLOT_SKILL_COUNT)
			{
				this.m_buttonHidden[(int)inSlotType] = !bShow;
			}
		}

		public void ShowLearnBtn(SkillSlotType inSlotType, bool bShow, bool bAll)
		{
			if (!bAll && (inSlotType < SkillSlotType.SLOT_SKILL_1 || inSlotType > SkillSlotType.SLOT_SKILL_3))
			{
				return;
			}
			List<GameObject> list = this.QuerySkillButtons(inSlotType, bAll);
			for (int i = 0; i < list.Count; i++)
			{
				GameObject gameObject = list[i];
				if (gameObject != null)
				{
					Transform transform = gameObject.transform.FindChild("LearnBtn");
					if (!(transform == null))
					{
						GameObject gameObject2 = transform.gameObject;
						gameObject2.CustomSetActive(bShow);
						this.m_learnBtnHidden[(int)inSlotType] = !bShow;
					}
				}
			}
		}

		public void ShowRestkSkillBtn(enRestSkillSlotType inRestSkillType, bool bShow)
		{
			GameObject gameObject = this.QueryRestSkillBtn(inRestSkillType);
			if (gameObject != null)
			{
				if (bShow)
				{
					if (this.m_RestSkillBtnHidden[(int)inRestSkillType])
					{
						gameObject.CustomSetActive(true);
						this.m_RestSkillBtnHidden[(int)inRestSkillType] = false;
					}
				}
				else
				{
					gameObject.CustomSetActive(false);
					this.m_RestSkillBtnHidden[(int)inRestSkillType] = true;
				}
			}
		}

		public void Activate(SkillSlotType inSlotType, bool bActivate, bool bAll)
		{
			NewbieGuideManager.CloseGuideForm();
			List<GameObject> list = this.QuerySkillButtons(inSlotType, bAll);
			for (int i = 0; i < list.Count; i++)
			{
				GameObject gameObject = list[i];
				if (gameObject != null)
				{
					Button component = gameObject.GetComponent<Button>();
					if (component)
					{
						component.enabled = bActivate;
					}
					CUIEventScript component2 = gameObject.GetComponent<CUIEventScript>();
					if (component2)
					{
						component2.enabled = bActivate;
					}
				}
			}
			if (!bActivate && Singleton<CBattleSystem>.GetInstance().FightForm != null && Singleton<CBattleSystem>.GetInstance().FightForm.m_skillButtonManager != null)
			{
				if (bAll)
				{
					for (int j = 0; j < 10; j++)
					{
						Singleton<CBattleSystem>.GetInstance().FightForm.m_skillButtonManager.SkillButtonUp(Singleton<CBattleSystem>.GetInstance().FightFormScript, (SkillSlotType)j, false, default(Vector2));
					}
				}
				else
				{
					Singleton<CBattleSystem>.GetInstance().FightForm.m_skillButtonManager.SkillButtonUp(Singleton<CBattleSystem>.GetInstance().FightFormScript, inSlotType, false, default(Vector2));
				}
			}
		}

		public void ActivateLearnBtn(SkillSlotType inSlotType, bool bActivate, bool bAll)
		{
			NewbieGuideManager.CloseGuideForm();
			if (!bAll && (inSlotType < SkillSlotType.SLOT_SKILL_1 || inSlotType > SkillSlotType.SLOT_SKILL_3))
			{
				return;
			}
			List<GameObject> list = this.QuerySkillButtons(inSlotType, bAll);
			for (int i = 0; i < list.Count; i++)
			{
				GameObject gameObject = list[i];
				if (gameObject != null)
				{
					Transform transform = gameObject.transform.FindChild("LearnBtn");
					if (!(transform == null))
					{
						GameObject gameObject2 = transform.gameObject;
						Button component = gameObject2.GetComponent<Button>();
						if (component)
						{
							component.enabled = bActivate;
						}
						CUIEventScript component2 = gameObject2.GetComponent<CUIEventScript>();
						if (component2)
						{
							component2.enabled = bActivate;
						}
					}
				}
			}
		}

		public void ActivateOtherBtn(enRestSkillSlotType inSlotType, bool bActivate, bool bAll)
		{
			NewbieGuideManager.CloseGuideForm();
			if (!bAll && (inSlotType < enRestSkillSlotType.BTN_SKILL_SELHERO || inSlotType > enRestSkillSlotType.BTN_SKILL_COUNT))
			{
				return;
			}
			if (bAll)
			{
				for (int i = 0; i < 4; i++)
				{
					GameObject gameObject = this.QueryRestSkillBtn((enRestSkillSlotType)i);
					if (gameObject != null)
					{
						Button component = gameObject.GetComponent<Button>();
						if (component)
						{
							component.enabled = bActivate;
						}
						CUIEventScript component2 = gameObject.GetComponent<CUIEventScript>();
						if (component2)
						{
							component2.enabled = bActivate;
						}
					}
				}
			}
			else
			{
				GameObject gameObject2 = this.QueryRestSkillBtn(inSlotType);
				if (gameObject2 != null)
				{
					Button component3 = gameObject2.GetComponent<Button>();
					if (component3)
					{
						component3.enabled = bActivate;
					}
					CUIEventScript component4 = gameObject2.GetComponent<CUIEventScript>();
					if (component4)
					{
						component4.enabled = bActivate;
					}
				}
			}
		}

		public void Highlight(SkillSlotType inSlotType, bool bHighlight, bool bAll, bool bDoActivating, bool bPauseGame, bool bRecordUseTime = false, int id = 0)
		{
			if (bHighlight)
			{
				this.ClearHighlight();
			}
			List<GameObject> list = this.QuerySkillButtons(inSlotType, bAll);
			bool flag = list.Count == 1;
			if (flag && bDoActivating)
			{
				if (bHighlight)
				{
					this.Activate(SkillSlotType.SLOT_SKILL_COUNT, false, true);
					this.Activate(inSlotType, true, false);
				}
				else
				{
					this.Activate(SkillSlotType.SLOT_SKILL_COUNT, true, true);
				}
			}
			for (int i = 0; i < list.Count; i++)
			{
				GameObject gameObject = list[i];
				if (gameObject != null)
				{
					Singleton<CBattleSystem>.GetInstance().FightForm.SetButtonHighLight(gameObject, bHighlight);
				}
			}
			if (inSlotType < SkillSlotType.SLOT_SKILL_COUNT)
			{
				this.m_buttonHighLighted[(int)inSlotType] = bHighlight;
				if (bHighlight)
				{
					this.m_buttonUseTimeRecord[(int)inSlotType].Id = id;
					this.m_buttonUseTimeRecord[(int)inSlotType].useTime = CRoleInfo.GetCurrentUTCTime();
				}
				else
				{
					RecordTime recordTime = this.m_buttonUseTimeRecord[(int)inSlotType];
					Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().ReqSetInBattleNewbieBit((uint)recordTime.Id, false, CRoleInfo.GetCurrentUTCTime() - recordTime.useTime);
					recordTime.Id = 0;
					recordTime.useTime = 0;
				}
			}
			this.UpdateGamePausing(bPauseGame);
		}

		public void HighlightLearnBtn(SkillSlotType inSlotType, bool bHighlight, bool bAll, bool bDoActivating, bool bPauseGame, bool bRecordUseTime = false, int id = 0)
		{
			if (bHighlight)
			{
				this.ClearHighlight();
			}
			if (!bAll && (inSlotType < SkillSlotType.SLOT_SKILL_1 || inSlotType > SkillSlotType.SLOT_SKILL_3))
			{
				return;
			}
			List<GameObject> list = this.QuerySkillButtons(inSlotType, bAll);
			if (list.Count == 0)
			{
				return;
			}
			bool flag = false;
			if (list.Count == 1)
			{
				flag = true;
			}
			if (flag && bDoActivating)
			{
				if (bHighlight)
				{
					this.Activate(SkillSlotType.SLOT_SKILL_COUNT, false, true);
					this.ActivateLearnBtn(SkillSlotType.SLOT_SKILL_COUNT, false, true);
					this.ActivateLearnBtn(inSlotType, true, false);
				}
				else
				{
					this.Activate(SkillSlotType.SLOT_SKILL_COUNT, true, true);
					this.ActivateLearnBtn(SkillSlotType.SLOT_SKILL_COUNT, true, true);
				}
			}
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i] != null)
				{
					Transform transform = list[i].transform.FindChild("LearnBtn");
					if (transform != null)
					{
						Singleton<CBattleSystem>.GetInstance().FightForm.SetLearnBtnHighLight(transform.gameObject, bHighlight);
					}
				}
			}
			if (inSlotType < SkillSlotType.SLOT_SKILL_COUNT)
			{
				this.m_learnBtnHighLighted[(int)inSlotType] = bHighlight;
				if (bHighlight)
				{
					this.m_learnBtnUseTimeRecord[(int)inSlotType].Id = id;
					this.m_learnBtnUseTimeRecord[(int)inSlotType].useTime = CRoleInfo.GetCurrentUTCTime();
				}
				else
				{
					RecordTime recordTime = this.m_learnBtnUseTimeRecord[(int)inSlotType];
					Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().ReqSetInBattleNewbieBit((uint)recordTime.Id, false, CRoleInfo.GetCurrentUTCTime() - recordTime.useTime);
					recordTime.Id = 0;
					recordTime.useTime = 0;
				}
			}
			this.UpdateGamePausing(bPauseGame);
		}

		public void HighlishtRestSkillBtn(enRestSkillSlotType inRestSkillType, bool bHighlight, bool bDoActivating, bool bPause, bool bRecordUseTime = false, int id = 0)
		{
			if (bHighlight)
			{
				this.ClearHighlight();
			}
			GameObject gameObject = this.QueryRestSkillBtn(inRestSkillType);
			if (gameObject != null)
			{
				if (bDoActivating)
				{
					if (bHighlight)
					{
						this.Activate(SkillSlotType.SLOT_SKILL_COUNT, false, true);
						this.ActivateOtherBtn(enRestSkillSlotType.BTN_SKILL_COUNT, false, true);
						this.ActivateOtherBtn(inRestSkillType, true, false);
						this.ActivateUI(false);
					}
					else
					{
						this.Activate(SkillSlotType.SLOT_SKILL_COUNT, true, true);
						this.ActivateOtherBtn(enRestSkillSlotType.BTN_SKILL_COUNT, true, true);
						this.ActivateUI(true);
					}
				}
				Singleton<CBattleSystem>.GetInstance().FightForm.SetButtonHighLight(gameObject, bHighlight);
				if (inRestSkillType < enRestSkillSlotType.BTN_SKILL_COUNT)
				{
					this.m_restSkilBtnHighLighted[(int)inRestSkillType] = bHighlight;
					if (bHighlight)
					{
						this.m_restSkilBtnUseTimeRecord[(int)inRestSkillType].Id = id;
						this.m_restSkilBtnUseTimeRecord[(int)inRestSkillType].useTime = CRoleInfo.GetCurrentUTCTime();
					}
					else
					{
						RecordTime recordTime = this.m_restSkilBtnUseTimeRecord[(int)inRestSkillType];
						Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().ReqSetInBattleNewbieBit((uint)recordTime.Id, false, CRoleInfo.GetCurrentUTCTime() - recordTime.useTime);
						recordTime.Id = 0;
						recordTime.useTime = 0;
					}
				}
				this.UpdateGamePausing(bPause);
			}
		}

		public void HighlightJoystick(bool bHighlight)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(FightForm.s_battleJoystick);
			if (form)
			{
				form.transform.FindChild(this.m_joystickHighlightName).gameObject.CustomSetActive(bHighlight);
			}
		}

		private void ClearHighlight()
		{
			for (int i = 0; i < 10; i++)
			{
				if (this.m_buttonHighLighted[i])
				{
					this.Highlight((SkillSlotType)i, false, false, true, false, false, 0);
				}
			}
			for (int j = 0; j < 4; j++)
			{
				if (this.m_learnBtnHighLighted[j])
				{
					this.HighlightLearnBtn((SkillSlotType)j, false, false, true, false, false, 0);
				}
			}
			for (int k = 0; k < 4; k++)
			{
				if (this.m_restSkilBtnHighLighted[k])
				{
					this.HighlishtRestSkillBtn((enRestSkillSlotType)k, false, false, true, false, 0);
				}
			}
		}

		public void OnBattleRequestUseSkill(CUIEvent uiEvent)
		{
			if (this.m_buttonHighLighted[(int)uiEvent.m_eventParams.m_skillSlotType])
			{
				this.Highlight(uiEvent.m_eventParams.m_skillSlotType, false, false, true, false, false, 0);
			}
		}

		private void OnBattleSkillBtnDown(CUIEvent uiEvent)
		{
			if (uiEvent.m_eventParams.m_skillSlotType == SkillSlotType.SLOT_SKILL_0 && this.m_buttonHighLighted[0])
			{
				this.Highlight(uiEvent.m_eventParams.m_skillSlotType, false, false, true, false, false, 0);
			}
		}

		public void OnBattleLearnSkillBtnClick(CUIEvent uiEvent)
		{
			string name = uiEvent.m_srcWidget.transform.parent.name;
			int num = int.Parse(name.Substring(name.Length - 1));
			if (this.m_learnBtnHighLighted[num])
			{
				this.HighlightLearnBtn((SkillSlotType)num, false, false, true, false, false, 0);
			}
		}

		public void OnAtkSelectHero(CUIEvent uiEvent)
		{
			if (this.m_restSkilBtnHighLighted[0])
			{
				this.HighlishtRestSkillBtn(enRestSkillSlotType.BTN_SKILL_SELHERO, false, true, false, false, 0);
			}
		}

		public void OnAtkSelectSoldier(CUIEvent uiEvent)
		{
			if (this.m_restSkilBtnHighLighted[1])
			{
				this.HighlishtRestSkillBtn(enRestSkillSlotType.BTN_SKILL_SELSOLDIER, false, true, false, false, 0);
			}
		}

		public void UpdateGamePausing(bool bPauseGame)
		{
			if (bPauseGame)
			{
				Singleton<CBattleGuideManager>.GetInstance().PauseGame(this, false);
				Singleton<CUIParticleSystem>.GetInstance().ClearAll(true);
			}
			else
			{
				Singleton<CBattleGuideManager>.GetInstance().ResumeGame(this);
			}
		}

		public void AddHighlightForActor(PoolObjHandle<ActorRoot> actor, bool bPauseGame)
		{
			if (!actor)
			{
				return;
			}
			ActorRoot handle = actor.handle;
			if (!handle.InCamera)
			{
				return;
			}
			GameObject gameObject = Singleton<CResourceManager>.GetInstance().GetResource("UGUI/Form/System/Dialog/WeakGuideHighlighter.prefab", typeof(GameObject), enResourceType.UIPrefab, false, false).m_content as GameObject;
			if (gameObject != null)
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject) as GameObject;
				if (gameObject2 != null)
				{
					if (NewbieGuideScriptControl.FormGuideMask == null)
					{
						NewbieGuideScriptControl.OpenGuideForm();
					}
					CUIFormScript formGuideMask = NewbieGuideScriptControl.FormGuideMask;
					Transform transform = formGuideMask.transform;
					Vector3 v = CUIUtility.WorldToScreenPoint(Camera.main, (Vector3)handle.location);
					Vector3 vector = CUIUtility.ScreenToWorldPoint(formGuideMask.GetCamera(), v, transform.position.z);
					Transform transform2 = gameObject2.transform;
					transform2.SetSiblingIndex(1);
					transform2.SetParent(NewbieGuideScriptControl.FormGuideMask.transform);
					formGuideMask.InitializeWidgetPosition(gameObject2, vector);
					transform2.position = vector;
					transform2.localScale = Vector3.one;
					CUIEventScript cUIEventScript = gameObject2.AddComponent<CUIEventScript>();
					CUIEventScript expr_117 = cUIEventScript;
					expr_117.onClick = (CUIEventScript.OnUIEventHandler)Delegate.Combine(expr_117.onClick, new CUIEventScript.OnUIEventHandler(this.HighliterForActorClickHandler));
					Singleton<CBattleGuideManager>.GetInstance().PauseGame(gameObject2, false);
				}
			}
		}

		public void HighliterForActorClickHandler(CUIEvent uiEvt)
		{
			GameObject srcWidget = uiEvt.m_srcWidget;
			Singleton<CBattleGuideManager>.GetInstance().ResumeGame(srcWidget);
			CUIFormScript formGuideMask = NewbieGuideScriptControl.FormGuideMask;
			Vector2 screenPos = CUIUtility.WorldToScreenPoint(formGuideMask.GetCamera(), srcWidget.transform.position);
			Singleton<LockModeScreenSelector>.GetInstance().OnClickBattleScene(screenPos);
			CUICommonSystem.DestoryObj(srcWidget, 0.1f);
			NewbieGuideScriptControl.CloseGuideForm();
		}

		public void ActivateUI(bool bActive)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(FightForm.s_battleUIForm);
			if (form != null)
			{
				GameObject widget = form.GetWidget(69);
				this.ActivateBtn(widget, bActive);
			}
		}

		private void ActivateBtn(GameObject btn, bool bActive)
		{
			if (btn != null)
			{
				Button component = btn.GetComponent<Button>();
				if (component)
				{
					component.enabled = bActive;
				}
				CUIEventScript component2 = btn.GetComponent<CUIEventScript>();
				if (component2)
				{
					component2.enabled = bActive;
				}
			}
		}
	}
}
