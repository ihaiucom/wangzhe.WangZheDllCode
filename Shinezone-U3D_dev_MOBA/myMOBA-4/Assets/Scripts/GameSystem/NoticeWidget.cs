using Assets.Scripts.UI;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class NoticeWidget : ActivityWidget
	{
		private Text _datePeriod;

		private Text _descContent;

		private ScrollRect _scrollRect;

		private GameObject _jumpBtn;

		private Text _jumpBtnLabel;

		public NoticeWidget(GameObject node, ActivityView view) : base(node, view)
		{
			this._datePeriod = Utility.GetComponetInChild<Text>(node, "DatePeriod");
			this._scrollRect = Utility.GetComponetInChild<ScrollRect>(node, "ScrollRect");
			this._descContent = Utility.GetComponetInChild<Text>(node, "ScrollRect/DescContent");
			this._jumpBtn = Utility.FindChild(node, "JumpBtn");
			this._jumpBtnLabel = Utility.GetComponetInChild<Text>(this._jumpBtn, "Text");
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Activity_NoticeJump, new CUIEventManager.OnUIEventHandler(this.OnClickJump));
			this.Validate();
		}

		public override void Clear()
		{
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Activity_NoticeJump, new CUIEventManager.OnUIEventHandler(this.OnClickJump));
		}

		public override void Validate()
		{
			this._datePeriod.text = base.view.activity.PeriodText;
			this._descContent.text = base.view.activity.Content;
			NoticeActivity noticeActivity = base.view.activity as NoticeActivity;
			if (noticeActivity != null)
			{
				if (noticeActivity.timeState == Activity.TimeState.Going)
				{
					string jumpLabel = noticeActivity.JumpLabel;
					if (string.IsNullOrEmpty(jumpLabel))
					{
						this._jumpBtn.CustomSetActive(false);
					}
					else
					{
						this._jumpBtn.CustomSetActive(true);
						this._jumpBtnLabel.text = jumpLabel;
					}
				}
				else
				{
					this._jumpBtn.CustomSetActive(false);
				}
			}
		}

		private void OnClickJump(CUIEvent evt)
		{
			NoticeActivity noticeActivity = base.view.activity as NoticeActivity;
			if (noticeActivity != null)
			{
				base.view.form.Close();
				noticeActivity.Jump();
			}
		}

		public override void OnShow()
		{
			this._scrollRect.verticalNormalizedPosition = 1f;
		}
	}
}
