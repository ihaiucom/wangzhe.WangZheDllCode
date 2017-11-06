using Assets.Scripts.UI;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class CheckInWidget : ActivityWidget
	{
		private Text _awardTitle;

		private Text _awardDesc;

		private Text _progLabel;

		private Text _timeRemain;

		private GameObject _getBtn;

		private Text _getBtnText;

		private uint _remainSeconds;

		public CheckInWidget(GameObject node, ActivityView view) : base(node, view)
		{
			this._awardTitle = Utility.GetComponetInChild<Text>(node, "Content/AwardTitle");
			this._awardDesc = Utility.GetComponetInChild<Text>(node, "Content/AwardDesc");
			this._timeRemain = Utility.GetComponetInChild<Text>(node, "Content/TimeRemain");
			this._progLabel = Utility.GetComponetInChild<Text>(node, "Content/Progress");
			this._getBtn = Utility.FindChild(node, "GetAward");
			this._getBtnText = Utility.GetComponetInChild<Text>(this._getBtn, "Text");
			this.Validate();
			view.activity.OnMaskStateChange += new Activity.ActivityEvent(this.OnStateChanged);
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Wealfare_CheckInGet, new CUIEventManager.OnUIEventHandler(this.OnClickGet));
		}

		public override void Clear()
		{
			base.view.activity.OnMaskStateChange -= new Activity.ActivityEvent(this.OnStateChanged);
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Wealfare_CheckInGet, new CUIEventManager.OnUIEventHandler(this.OnClickGet));
		}

		public override void Validate()
		{
			ActivityPhase curPhase = base.view.activity.CurPhase;
			if (curPhase == null)
			{
				return;
			}
			if (curPhase.RewardDesc.get_Length() > 0)
			{
				this._awardDesc.set_text(curPhase.RewardDesc);
			}
			else
			{
				CUseable useable = curPhase.GetUseable(0);
				if (useable != null)
				{
					this._awardDesc.set_text(useable.m_name + ":" + curPhase.GetDropCount(0));
				}
			}
			if (curPhase.ReadyForGet)
			{
				this._getBtn.GetComponent<CUIEventScript>().enabled = true;
				this._getBtn.GetComponent<Button>().set_interactable(true);
				this._getBtnText.set_color(Color.white);
				this._awardTitle.set_text(Singleton<CTextManager>.GetInstance().GetText("awardToday"));
				this._getBtnText.set_text(Singleton<CTextManager>.GetInstance().GetText("get"));
				this._remainSeconds = 0u;
				this._timeRemain.set_text(Singleton<CTextManager>.GetInstance().GetText("timeCountDown").Replace("{0}", Utility.SecondsToTimeText(0u)));
			}
			else
			{
				this._getBtn.GetComponent<CUIEventScript>().enabled = false;
				this._getBtn.GetComponent<Button>().set_interactable(false);
				this._getBtnText.set_color(Color.gray);
				if (base.view.activity.Completed)
				{
					this._awardTitle.set_text(Singleton<CTextManager>.GetInstance().GetText("awardToday"));
					this._getBtnText.set_text(Singleton<CTextManager>.GetInstance().GetText("finished"));
					this._remainSeconds = 0u;
					this._timeRemain.set_text(Singleton<CTextManager>.GetInstance().GetText("congraduFinish"));
				}
				else
				{
					this._awardTitle.set_text(Singleton<CTextManager>.GetInstance().GetText("awardTomorrow"));
					this._getBtnText.set_text(Singleton<CTextManager>.GetInstance().GetText("notInTime"));
					DateTime dateTime = Utility.ToUtcTime2Local((long)CRoleInfo.GetCurrentUTCTime());
					DateTime dateTime2 = dateTime.AddDays(1.0);
					dateTime2 = new DateTime(dateTime2.get_Year(), dateTime2.get_Month(), dateTime2.get_Day(), 0, 0, 0);
					this._remainSeconds = (uint)(dateTime2 - dateTime).get_TotalSeconds();
					this._timeRemain.set_text(Singleton<CTextManager>.GetInstance().GetText("timeCountDown").Replace("{0}", Utility.SecondsToTimeText(this._remainSeconds)));
				}
			}
			Text progLabel = this._progLabel;
			string text = Singleton<CTextManager>.GetInstance().GetText("CheckInProgress");
			string text2 = "{0}";
			int current = base.view.activity.Current;
			progLabel.set_text(text.Replace(text2, current.ToString()).Replace("{1}", base.view.activity.Target.ToString()));
		}

		public override void Update()
		{
			if (this._remainSeconds > 0u)
			{
				this._remainSeconds -= 1u;
				this._timeRemain.set_text(Singleton<CTextManager>.GetInstance().GetText("timeCountDown").Replace("{0}", Utility.SecondsToTimeText(this._remainSeconds)));
			}
		}

		private void OnClickGet(CUIEvent uiEvent)
		{
			ActivityPhase curPhase = base.view.activity.CurPhase;
			if (curPhase != null)
			{
				curPhase.DrawReward();
			}
		}

		private void OnStateChanged(Activity actv)
		{
			this.Validate();
		}
	}
}
