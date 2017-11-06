using Assets.Scripts.UI;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class MultiGainWgt : ActivityWidget
	{
		public class MultiGainListItem
		{
			public const int REWARD_ITEM_COUNT = 4;

			public GameObject root;

			public GameObject gotoBtn;

			public Text gotoBtnTxt;

			public Text title;

			public Text tips;

			public Text remainTimes;

			public MultiGainPhase actvPhase;

			public MultiGainListItem(GameObject node, MultiGainPhase ap)
			{
				this.root = node;
				this.actvPhase = ap;
				this.gotoBtn = Utility.FindChild(node, "Goto");
				this.gotoBtnTxt = Utility.GetComponetInChild<Text>(this.gotoBtn, "Text");
				this.title = Utility.GetComponetInChild<Text>(node, "Title");
				this.tips = Utility.GetComponetInChild<Text>(node, "Tips");
				this.remainTimes = Utility.GetComponetInChild<Text>(node, "RemainTimes");
				this.actvPhase.OnMaskStateChange += new ActivityPhase.ActivityPhaseEvent(this.OnStateChange);
				this.actvPhase.OnTimeStateChange += new ActivityPhase.ActivityPhaseEvent(this.OnStateChange);
			}

			public void Clear()
			{
				this.actvPhase.OnMaskStateChange -= new ActivityPhase.ActivityPhaseEvent(this.OnStateChange);
				this.actvPhase.OnTimeStateChange -= new ActivityPhase.ActivityPhaseEvent(this.OnStateChange);
			}

			public void Validate()
			{
				this.title.set_text(this.actvPhase.Desc);
				this.tips.set_text(this.actvPhase.Tips);
				this.remainTimes.set_text((this.actvPhase.LimitTimes > 0) ? string.Format("{0:D}/{1:D}", this.actvPhase.RemainTimes, this.actvPhase.LimitTimes) : Singleton<CTextManager>.GetInstance().GetText("noLimit"));
				if (this.actvPhase.timeState == ActivityPhase.TimeState.Started)
				{
					bool readyForGo = this.actvPhase.ReadyForGo;
					this.gotoBtn.GetComponent<CUIEventScript>().enabled = readyForGo;
					this.gotoBtn.GetComponent<Button>().set_interactable(readyForGo);
					this.gotoBtnTxt.set_text(Singleton<CTextManager>.GetInstance().GetText(readyForGo ? "gotoFinish" : "finished"));
					this.gotoBtnTxt.set_color(readyForGo ? Color.white : Color.gray);
				}
				else
				{
					this.gotoBtn.GetComponent<CUIEventScript>().enabled = false;
					this.gotoBtn.GetComponent<Button>().set_interactable(false);
					this.gotoBtnTxt.set_text(Singleton<CTextManager>.GetInstance().GetText((this.actvPhase.timeState == ActivityPhase.TimeState.Closed) ? "outOfTime" : "notInTime"));
					this.gotoBtnTxt.set_color(Color.gray);
				}
			}

			private void OnStateChange(ActivityPhase ap)
			{
				this.Validate();
			}
		}

		private const float SPACING_Y = 5f;

		private ListView<MultiGainWgt.MultiGainListItem> _elementList;

		private GameObject _elementTmpl;

		public MultiGainWgt(GameObject node, ActivityView view) : base(node, view)
		{
			this._elementTmpl = Utility.FindChild(node, "Template");
			float height = this._elementTmpl.GetComponent<RectTransform>().rect.height;
			ListView<ActivityPhase> phaseList = view.activity.PhaseList;
			this._elementList = new ListView<MultiGainWgt.MultiGainListItem>();
			for (int i = 0; i < phaseList.Count; i++)
			{
				GameObject gameObject;
				if (i > 0)
				{
					gameObject = (GameObject)Object.Instantiate(this._elementTmpl);
					gameObject.transform.SetParent(this._elementTmpl.transform.parent);
					gameObject.transform.localPosition = this._elementList[i - 1].root.transform.localPosition + new Vector3(0f, -(height + 5f), 0f);
					gameObject.transform.localScale = Vector3.one;
					gameObject.transform.localRotation = Quaternion.identity;
				}
				else
				{
					this._elementTmpl.SetActive(true);
					gameObject = this._elementTmpl;
				}
				MultiGainWgt.MultiGainListItem item = new MultiGainWgt.MultiGainListItem(gameObject, (MultiGainPhase)phaseList[i]);
				this._elementList.Add(item);
			}
			if (this._elementList.Count == 0)
			{
				this._elementTmpl.SetActive(false);
			}
			node.GetComponent<RectTransform>().sizeDelta = new Vector2(node.GetComponent<RectTransform>().sizeDelta.x, (this._elementList.Count > 0) ? (height * (float)this._elementList.Count + (float)(this._elementList.Count - 1) * 5f) : 0f);
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Activity_ClickGoto, new CUIEventManager.OnUIEventHandler(this.OnClickGoto));
			view.activity.OnTimeStateChange += new Activity.ActivityEvent(this.OnStateChange);
			this.Validate();
		}

		private void OnStateChange(Activity acty)
		{
			this.Validate();
		}

		public override void Clear()
		{
			base.view.activity.OnTimeStateChange -= new Activity.ActivityEvent(this.OnStateChange);
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Activity_ClickGoto, new CUIEventManager.OnUIEventHandler(this.OnClickGoto));
			for (int i = 0; i < this._elementList.Count; i++)
			{
				this._elementList[i].Clear();
				if (i > 0)
				{
					CUICommonSystem.DestoryObj(this._elementList[i].root, 0.1f);
				}
			}
			this._elementList = null;
			this._elementTmpl = null;
		}

		private void OnClickGoto(CUIEvent uiEvent)
		{
			for (int i = 0; i < this._elementList.Count; i++)
			{
				MultiGainWgt.MultiGainListItem multiGainListItem = this._elementList[i];
				if (multiGainListItem.gotoBtn == uiEvent.m_srcWidget)
				{
					base.view.form.Close();
					multiGainListItem.actvPhase.AchieveJump();
					break;
				}
			}
		}

		public override void Validate()
		{
			for (int i = 0; i < this._elementList.Count; i++)
			{
				MultiGainWgt.MultiGainListItem multiGainListItem = this._elementList[i];
				multiGainListItem.Validate();
			}
		}
	}
}
