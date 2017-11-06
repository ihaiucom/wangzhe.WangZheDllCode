using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class ProgressWidget : ActivityWidget
	{
		public class Cursor
		{
			public GameObject root;

			public Text valTxt;

			public Image arrow;

			public Cursor(GameObject node)
			{
				this.root = node;
				this.valTxt = Utility.GetComponetInChild<Text>(node, "Value");
				this.arrow = Utility.GetComponetInChild<Image>(node, "Arrow");
			}
		}

		private Text _tips;

		private Image _back;

		private Image _fore;

		private ProgressWidget.Cursor _cursorTemplate;

		private ProgressWidget.Cursor[] _cursorArray;

		public ProgressWidget(GameObject node, ActivityView view) : base(node, view)
		{
			this._tips = Utility.GetComponetInChild<Text>(node, "Tips");
			this._back = Utility.GetComponetInChild<Image>(node, "Bar");
			this._fore = Utility.GetComponetInChild<Image>(node, "Bar/Fore");
			this._cursorTemplate = new ProgressWidget.Cursor(Utility.FindChild(node, "Bar/Cursor"));
			ListView<ActivityPhase> phaseList = view.activity.PhaseList;
			if (phaseList.Count > 0)
			{
				this._cursorArray = new ProgressWidget.Cursor[phaseList.Count];
				this._cursorTemplate.root.SetActive(true);
				this._cursorArray[0] = this._cursorTemplate;
				for (int i = 1; i < phaseList.Count; i++)
				{
					ProgressWidget.Cursor cursor = new ProgressWidget.Cursor((GameObject)Object.Instantiate(this._cursorTemplate.root));
					cursor.root.transform.SetParent(this._cursorTemplate.root.transform.parent);
					this._cursorArray[i] = cursor;
				}
			}
			else
			{
				this._cursorTemplate.root.SetActive(false);
			}
			view.activity.OnTimeStateChange += new Activity.ActivityEvent(this.OnStateChange);
			view.activity.OnMaskStateChange += new Activity.ActivityEvent(this.OnStateChange);
			this.Validate();
		}

		public override void Clear()
		{
			base.view.activity.OnTimeStateChange -= new Activity.ActivityEvent(this.OnStateChange);
			base.view.activity.OnMaskStateChange -= new Activity.ActivityEvent(this.OnStateChange);
			if (this._cursorArray != null)
			{
				for (int i = 1; i < this._cursorArray.Length; i++)
				{
					CUICommonSystem.DestoryObj(this._cursorArray[i].root, 0.1f);
				}
				this._cursorArray = null;
			}
		}

		private void OnStateChange(Activity actv)
		{
			this.Validate();
		}

		public override void Validate()
		{
			ListView<ActivityPhase> phaseList = base.view.activity.PhaseList;
			if (phaseList.Count > 0)
			{
				this._tips.set_text(base.view.activity.Tips);
				int target = base.view.activity.Target;
				int num = base.view.activity.Current;
				if (num > target)
				{
					num = target;
				}
				float width = this._back.GetComponent<RectTransform>().rect.width;
				float num2 = (float)num / (float)target;
				RectTransform component = this._fore.GetComponent<RectTransform>();
				component.sizeDelta = new Vector2(num2 * width, component.rect.height);
				for (int i = 0; i < phaseList.Count; i++)
				{
					ActivityPhase activityPhase = phaseList[i];
					float num3 = (float)activityPhase.Target / (float)target;
					ProgressWidget.Cursor cursor = this._cursorArray[i];
					cursor.root.transform.localPosition = new Vector3(num3 * width, 0f, 0f);
					cursor.root.transform.localScale = Vector3.one;
					cursor.root.transform.localRotation = Quaternion.identity;
					bool flag = num >= activityPhase.Target;
					cursor.valTxt.set_text(activityPhase.Target.ToString());
					cursor.valTxt.set_color(flag ? Color.white : Color.gray);
					cursor.arrow.set_color(flag ? Color.white : Color.gray);
				}
			}
		}
	}
}
