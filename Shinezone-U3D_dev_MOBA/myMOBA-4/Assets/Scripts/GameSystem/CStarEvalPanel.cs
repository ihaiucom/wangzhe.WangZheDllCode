using Assets.Scripts.GameLogic;
using Assets.Scripts.UI;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class CStarEvalPanel
	{
		private bool bPanelOpen;

		private Text[] conditionTexts = new Text[3];

		private GameObject m_Obj;

		private GameObject taskPanel;

		private GameObject arrowIcon;

		private Image PanelIcon;

		public void Init(GameObject obj)
		{
			this.m_Obj = obj;
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			if (curLvelContext.IsMobaMode() || curLvelContext.IsGameTypeGuide() || curLvelContext.IsGameTypeBurning() || curLvelContext.IsGameTypeArena())
			{
				this.Hide();
				return;
			}
			this.Show();
			this.bPanelOpen = false;
			this.taskPanel = this.m_Obj.transform.Find("TaskPanel").gameObject;
			this.arrowIcon = this.m_Obj.transform.Find("Image").gameObject;
			this.taskPanel.CustomSetActive(false);
			this.PanelIcon = this.m_Obj.transform.Find("icon").GetComponent<Image>();
			this.initEvent();
			UT.If_Null_Error<GameObject>(this.m_Obj);
			UT.If_Null_Error<GameObject>(this.taskPanel);
			UT.If_Null_Error<Image>(this.PanelIcon);
		}

		public void Clear()
		{
			this.m_Obj = null;
			this.taskPanel = null;
			this.arrowIcon = null;
			this.PanelIcon = null;
			this.conditionTexts = new Text[3];
			this.DeinitEvent();
		}

		public void Show()
		{
			if (this.m_Obj != null)
			{
				this.m_Obj.gameObject.CustomSetActive(true);
			}
		}

		public void Hide()
		{
			if (this.m_Obj != null)
			{
				this.m_Obj.gameObject.CustomSetActive(false);
			}
		}

		private void initEvent()
		{
			Singleton<StarSystem>.GetInstance().OnEvaluationChanged += new OnEvaluationChangedDelegate(this.OnEvaluationChange);
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Adv_OpenTaskPanel, new CUIEventManager.OnUIEventHandler(this.openTaskPanel));
			Singleton<EventRouter>.instance.AddEventHandler(EventID.StarSystemInitialized, new Action(this.reset));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_TaskPanel_SlideEnd, new CUIEventManager.OnUIEventHandler(this.onTaskPanelSlideEnd));
		}

		private void DeinitEvent()
		{
			Singleton<StarSystem>.GetInstance().OnEvaluationChanged -= new OnEvaluationChangedDelegate(this.OnEvaluationChange);
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Adv_OpenTaskPanel, new CUIEventManager.OnUIEventHandler(this.openTaskPanel));
			Singleton<EventRouter>.instance.RemoveEventHandler(EventID.StarSystemInitialized, new Action(this.reset));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_TaskPanel_SlideEnd, new CUIEventManager.OnUIEventHandler(this.onTaskPanelSlideEnd));
		}

		public void reset()
		{
			Transform transform = this.m_Obj.transform.Find("TaskPanel").transform;
			ListView<IStarEvaluation>.Enumerator enumerator = Singleton<StarSystem>.GetInstance().GetEnumerator();
			while (enumerator.MoveNext())
			{
				this.conditionTexts[enumerator.Current.index] = transform.Find(string.Format("Condition{0}", enumerator.Current.index + 1)).GetComponent<Text>();
				this.conditionTexts[enumerator.Current.index].text = enumerator.Current.description;
				if (enumerator.Current.isSuccess)
				{
					this.conditionTexts[enumerator.Current.index].color = Color.green;
				}
				else
				{
					this.conditionTexts[enumerator.Current.index].color = Color.white;
				}
			}
			UT.If_Null_Error<Text[]>(this.conditionTexts);
		}

		public void OnEvaluationChange(IStarEvaluation InStarEvaluation, IStarCondition InStarCondition)
		{
			if (this.conditionTexts[InStarEvaluation.index] != null)
			{
				this.conditionTexts[InStarEvaluation.index].text = InStarEvaluation.description;
			}
			Transform transform = this.m_Obj.transform.Find("TaskPanel").transform;
			if (transform)
			{
				if (InStarEvaluation.isSuccess)
				{
					transform.Find(string.Format("Condition{0}", InStarEvaluation.index + 1)).GetComponent<Text>().color = Color.green;
				}
				else
				{
					transform.Find(string.Format("Condition{0}", InStarEvaluation.index + 1)).GetComponent<Text>().color = Color.white;
				}
			}
		}

		public void openTaskPanel(CUIEvent uiEvent)
		{
			if (!this.bPanelOpen)
			{
				this.taskPanel.CustomSetActive(true);
				this.arrowIcon.CustomSetActive(true);
			}
			stUIEventParams eventParams = default(stUIEventParams);
			eventParams.tag = ((!this.bPanelOpen) ? 6 : 5);
			this.taskPanel.GetComponent<CUIAnimatorScript>().SetUIEvent(enAnimatorEventType.AnimatorEnd, enUIEventID.Battle_TaskPanel_SlideEnd, eventParams);
			this.taskPanel.GetComponent<Animator>().Play((!this.bPanelOpen) ? "Form_Battle_EvalPlanel_in" : "Form_Battle_EvalPlanel_out");
			this.arrowIcon.GetComponent<Animator>().Play((!this.bPanelOpen) ? "Form_Battle_EvalPlanel_in2" : "Form_Battle_EvalPlanel_out2");
			this.bPanelOpen = !this.bPanelOpen;
		}

		private void onTaskPanelSlideEnd(CUIEvent uiEvt)
		{
			if (uiEvt.m_eventParams.tag == 5)
			{
				this.taskPanel.CustomSetActive(false);
				this.arrowIcon.CustomSetActive(false);
			}
		}
	}
}
