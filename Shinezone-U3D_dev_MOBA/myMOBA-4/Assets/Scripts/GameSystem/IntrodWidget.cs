using Assets.Scripts.UI;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class IntrodWidget : ActivityWidget
	{
		private Text _datePeriod;

		private Text _descTitle;

		private Text _descContent;

		private GameObject _moreBtn;

		private Text _moreBtnLabel;

		private bool _isDetail;

		public IntrodWidget(GameObject node, ActivityView view) : base(node, view)
		{
			this._datePeriod = Utility.GetComponetInChild<Text>(node, "DatePeriod");
			this._descTitle = Utility.GetComponetInChild<Text>(node, "DescTitle");
			this._descContent = Utility.GetComponetInChild<Text>(node, "DescContent");
			this._moreBtn = Utility.FindChild(node, "DetailBtn");
			this._moreBtnLabel = Utility.GetComponetInChild<Text>(this._moreBtn, "Text");
			this._isDetail = false;
			this._moreBtn.SetActive(view.activity.Content.Trim().Length > 0);
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Activity_ClickMore, new CUIEventManager.OnUIEventHandler(this.OnClickMore));
			this.Validate();
		}

		public override void Clear()
		{
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Activity_ClickMore, new CUIEventManager.OnUIEventHandler(this.OnClickMore));
		}

		public override void Validate()
		{
			this._datePeriod.text = base.view.activity.PeriodText;
			if (this._isDetail)
			{
				this._descTitle.text = Singleton<CTextManager>.GetInstance().GetText("activityDetailTitle");
				this._descContent.text = base.view.activity.Content;
				this._moreBtnLabel.text = Singleton<CTextManager>.GetInstance().GetText("return");
			}
			else
			{
				this._descTitle.text = Singleton<CTextManager>.GetInstance().GetText("activityBriefTitle");
				this._descContent.text = base.view.activity.Brief;
				this._moreBtnLabel.text = Singleton<CTextManager>.GetInstance().GetText("moreDetail");
			}
		}

		public override void OnShow()
		{
			if (base.view.WidgetCount == 1)
			{
				this._isDetail = true;
				this._moreBtn.SetActive(false);
				this._descTitle.text = Singleton<CTextManager>.GetInstance().GetText("activityDetailTitle");
				this._descContent.text = base.view.activity.Content;
			}
		}

		private void OnClickMore(CUIEvent evt)
		{
			this._isDetail = !this._isDetail;
			if (this._isDetail)
			{
				((CampaignFormView)base.view).ExcludeShow(this);
			}
			else
			{
				((CampaignFormView)base.view).RestoreShow();
			}
			this.Validate();
		}
	}
}
