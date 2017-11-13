using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
	public class CUIToggleEventScript : CUIComponent
	{
		[HideInInspector]
		public enUIEventID m_onValueChangedEventID;

		private Toggle m_toggle;

		public override void Initialize(CUIFormScript formScript)
		{
			if (this.m_isInitialized)
			{
				return;
			}
			this.m_toggle = base.gameObject.GetComponent<Toggle>();
			this.m_toggle.onValueChanged.RemoveAllListeners();
			this.m_toggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnToggleValueChanged));
			Transform transform = base.gameObject.transform.FindChild("Label");
			if (transform != null)
			{
				if (this.m_toggle.get_isOn())
				{
					transform.GetComponent<Text>().set_color(CUIUtility.s_Color_White);
				}
				else
				{
					transform.GetComponent<Text>().set_color(CUIUtility.s_Text_Color_ListElement_Normal);
				}
			}
			base.Initialize(formScript);
		}

		protected override void OnDestroy()
		{
			this.m_toggle = null;
			base.OnDestroy();
		}

		private void OnToggleValueChanged(bool isOn)
		{
			if (this.m_onValueChangedEventID == enUIEventID.None)
			{
				return;
			}
			CUIEvent uIEvent = Singleton<CUIEventManager>.GetInstance().GetUIEvent();
			uIEvent.m_srcFormScript = this.m_belongedFormScript;
			uIEvent.m_srcWidget = base.gameObject;
			uIEvent.m_srcWidgetScript = this;
			uIEvent.m_srcWidgetBelongedListScript = this.m_belongedListScript;
			uIEvent.m_srcWidgetIndexInBelongedList = this.m_indexInlist;
			uIEvent.m_pointerEventData = null;
			uIEvent.m_eventID = this.m_onValueChangedEventID;
			uIEvent.m_eventParams.togleIsOn = isOn;
			Transform transform = base.gameObject.transform.FindChild("Label");
			if (transform != null)
			{
				if (isOn)
				{
					transform.GetComponent<Text>().set_color(CUIUtility.s_Color_White);
				}
				else
				{
					transform.GetComponent<Text>().set_color(CUIUtility.s_Text_Color_ListElement_Normal);
				}
			}
			base.DispatchUIEvent(uIEvent);
		}
	}
}
