using Assets.Scripts.UI;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class BannerWidget : ActivityWidget
	{
		public const float SCROLL_TIME_SPAN = 3.5f;

		private CUIStepListScript _stepList;

		private int _curStepIndex;

		private bool _leftToRight;

		private float _lastScrollTime;

		private ListView<UrlAction> _urlaList;

		private Text _timeCD;

		private GameObject _pickObject;

		private int[] _pickIdxList;

		private Vector2 _lastPos;

		private bool _bStopAutoMove;

		public BannerWidget(GameObject node, ActivityView view) : base(node, view)
		{
			this._stepList = Utility.GetComponetInChild<CUIStepListScript>(node, "StepList");
			this._stepList.SetDontUpdate(true);
			this._timeCD = Utility.GetComponetInChild<Text>(node, "TimeCD");
			this._pickObject = Utility.FindChild(node, "StepList/pickObj");
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Activity_BannerClick, new CUIEventManager.OnUIEventHandler(this.OnClick));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Activity_Banner_HoldStart, new CUIEventManager.OnUIEventHandler(this.OnHoldStart_Item));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Activity_Banner_HoldEnd, new CUIEventManager.OnUIEventHandler(this.OnHoldEnd_Item));
			this.Validate();
		}

		public override void Clear()
		{
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Activity_BannerClick, new CUIEventManager.OnUIEventHandler(this.OnClick));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Activity_Banner_HoldStart, new CUIEventManager.OnUIEventHandler(this.OnHoldStart_Item));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Activity_Banner_HoldEnd, new CUIEventManager.OnUIEventHandler(this.OnHoldEnd_Item));
		}

		public override void Update()
		{
			this.updateAutoScroll();
			this.updateOverTime();
		}

		public override void Validate()
		{
			this._urlaList = UrlAction.ParseFromText(base.view.activity.Content, null);
			int count = this._urlaList.Count;
			if (count > 0)
			{
				this._stepList.SetElementAmount(count);
				for (int i = 0; i < count; i++)
				{
					UrlAction urlAction = this._urlaList[i];
					CUIListElementScript elemenet = this._stepList.GetElemenet(i);
					if (null != elemenet)
					{
						CUIHttpImageScript component = elemenet.GetComponent<CUIHttpImageScript>();
						if (null != component)
						{
							component.SetImageUrl(urlAction.target);
						}
					}
				}
				this._pickIdxList = new int[count];
				if (this._pickObject != null)
				{
					CUIContainerScript component2 = this._pickObject.GetComponent<CUIContainerScript>();
					if (component2 != null)
					{
						component2.RecycleAllElement();
						for (int j = 0; j < this._urlaList.Count; j++)
						{
							this._pickIdxList[j] = component2.GetElement();
						}
					}
				}
				this._curStepIndex = 0;
				this._leftToRight = true;
				this._stepList.MoveElementInScrollArea(this._curStepIndex, true);
				this.EnablePickObj(this._curStepIndex);
				this._lastScrollTime = Time.time;
				this.updateOverTime();
			}
		}

		private void EnablePickObj(int idx)
		{
			if (this._pickObject != null)
			{
				CUIContainerScript component = this._pickObject.GetComponent<CUIContainerScript>();
				if (component)
				{
					for (int i = 0; i < this._pickIdxList.Length; i++)
					{
						if (i == idx)
						{
							GameObject element = component.GetElement(this._pickIdxList[i]);
							if (element)
							{
								Transform transform = element.transform.FindChild("Image_Pointer");
								if (transform != null)
								{
									transform.gameObject.CustomSetActive(true);
								}
							}
						}
						else
						{
							GameObject element2 = component.GetElement(this._pickIdxList[i]);
							if (element2)
							{
								Transform transform2 = element2.transform.FindChild("Image_Pointer");
								if (transform2 != null)
								{
									transform2.gameObject.CustomSetActive(false);
								}
							}
						}
					}
				}
			}
		}

		private void OnClick(CUIEvent uiEvent)
		{
			int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
			if (this._urlaList != null && srcWidgetIndexInBelongedList < this._urlaList.Count)
			{
				UrlAction urlAction = this._urlaList[srcWidgetIndexInBelongedList];
				if (urlAction.Execute())
				{
					base.view.form.Close();
				}
			}
		}

		private void OnHoldStart_Item(CUIEvent uiEvent)
		{
			if (uiEvent.m_srcWidgetBelongedListScript != this._stepList)
			{
				return;
			}
			this._lastPos = uiEvent.m_pointerEventData.get_position();
			this._bStopAutoMove = true;
		}

		private void OnHoldEnd_Item(CUIEvent uiEvent)
		{
			if (uiEvent.m_srcWidgetBelongedListScript != this._stepList)
			{
				return;
			}
			int count = this._urlaList.Count;
			if (uiEvent.m_pointerEventData.get_position().x <= this._lastPos.x)
			{
				if (this._curStepIndex + 1 < count)
				{
					this._curStepIndex++;
					this._stepList.MoveElementInScrollArea(this._curStepIndex, false);
					this.EnablePickObj(this._curStepIndex);
				}
			}
			else if (this._curStepIndex > 0)
			{
				this._curStepIndex--;
				this._stepList.MoveElementInScrollArea(this._curStepIndex, false);
				this.EnablePickObj(this._curStepIndex);
			}
			this._bStopAutoMove = false;
			this._lastScrollTime = Time.time;
		}

		private void updateAutoScroll()
		{
			if (this._bStopAutoMove)
			{
				return;
			}
			int count = this._urlaList.Count;
			if (count > 1 && this._curStepIndex < count && Time.time > this._lastScrollTime + (float)this._urlaList[this._curStepIndex].showTime * 0.001f)
			{
				this._lastScrollTime = Time.time;
				if ((this._curStepIndex + 1 == count && this._leftToRight) || (this._curStepIndex == 0 && !this._leftToRight))
				{
					this._leftToRight = !this._leftToRight;
				}
				this._curStepIndex += (this._leftToRight ? 1 : -1);
				this._stepList.MoveElementInScrollArea(this._curStepIndex, false);
				this.EnablePickObj(this._curStepIndex);
			}
		}

		private void updateOverTime()
		{
			UrlAction urlAction = this._urlaList[this._curStepIndex];
			if (urlAction.overTime > 0uL)
			{
				DateTime dateTime = Utility.ULongToDateTime(urlAction.overTime);
				DateTime dateTime2 = Utility.ToUtcTime2Local((long)CRoleInfo.GetCurrentUTCTime());
				TimeSpan timeSpan = dateTime - dateTime2;
				string text = Singleton<CTextManager>.GetInstance().GetText("TIME_SPAN_FORMAT");
				text = text.Replace("{0}", timeSpan.get_Days().ToString());
				text = text.Replace("{1}", timeSpan.get_Hours().ToString());
				text = text.Replace("{2}", timeSpan.get_Minutes().ToString());
				text = text.Replace("{3}", timeSpan.get_Seconds().ToString());
				this._timeCD.gameObject.CustomSetActive(dateTime >= dateTime2);
				this._timeCD.set_text(text);
			}
			else
			{
				this._timeCD.gameObject.CustomSetActive(false);
			}
		}
	}
}
