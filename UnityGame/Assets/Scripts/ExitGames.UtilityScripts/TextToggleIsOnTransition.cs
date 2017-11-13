using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ExitGames.UtilityScripts
{
	[RequireComponent(typeof(Text))]
	public class TextToggleIsOnTransition : MonoBehaviour, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler
	{
		public Toggle toggle;

		private Text _text;

		public Color NormalOnColor = Color.white;

		public Color NormalOffColor = Color.black;

		public Color HoverOnColor = Color.black;

		public Color HoverOffColor = Color.black;

		private bool isHover;

		public void OnEnable()
		{
			this._text = base.GetComponent<Text>();
			this.toggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnValueChanged));
		}

		public void OnDisable()
		{
			this.toggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.OnValueChanged));
		}

		public void OnValueChanged(bool isOn)
		{
			this._text.set_color((!isOn) ? ((!this.isHover) ? this.NormalOffColor : this.NormalOnColor) : ((!this.isHover) ? this.HoverOffColor : this.HoverOnColor));
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			this.isHover = true;
			this._text.set_color((!this.toggle.get_isOn()) ? this.HoverOffColor : this.HoverOnColor);
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			this.isHover = false;
			this._text.set_color((!this.toggle.get_isOn()) ? this.NormalOffColor : this.NormalOnColor);
		}
	}
}
