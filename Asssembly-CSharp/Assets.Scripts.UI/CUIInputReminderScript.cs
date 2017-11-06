using System;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
	public class CUIInputReminderScript : CUIComponent
	{
		public enum enCountType
		{
			CountDown,
			CountUp
		}

		public CUIInputReminderScript.enCountType m_countType;

		public Text m_displayReminderText;

		private InputField m_inputField;

		public override void Initialize(CUIFormScript formScript)
		{
			if (this.m_isInitialized)
			{
				return;
			}
			this.m_inputField = base.gameObject.GetComponent<InputField>();
			this.m_inputField.get_onValueChange().RemoveAllListeners();
			this.m_inputField.get_onValueChange().AddListener(new UnityAction<string>(this.OnTextContentChanged));
			if (this.m_displayReminderText != null)
			{
				if (this.m_countType == CUIInputReminderScript.enCountType.CountDown)
				{
					this.m_displayReminderText.set_text(this.m_inputField.get_characterLimit().ToString());
				}
				else if (this.m_countType == CUIInputReminderScript.enCountType.CountUp)
				{
					this.m_displayReminderText.set_text(0.ToString());
				}
			}
			base.Initialize(formScript);
		}

		protected override void OnDestroy()
		{
			this.m_displayReminderText = null;
			this.m_inputField = null;
			base.OnDestroy();
		}

		private void OnTextContentChanged(string text)
		{
			if (this.m_displayReminderText == null)
			{
				return;
			}
			if (this.m_countType == CUIInputReminderScript.enCountType.CountDown)
			{
				this.m_displayReminderText.set_text((this.m_inputField.get_characterLimit() - text.get_Length()).ToString());
			}
			else if (this.m_countType == CUIInputReminderScript.enCountType.CountUp)
			{
				this.m_displayReminderText.set_text(text.get_Length().ToString());
			}
		}
	}
}
