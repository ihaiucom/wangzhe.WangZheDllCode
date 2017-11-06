using System;
using UnityEngine;
using UnityEngine.UI;

namespace ExitGames.Demos.DemoAnimator
{
	public class PlayerUI : MonoBehaviour
	{
		[Tooltip("Pixel offset from the player target")]
		public Vector3 ScreenOffset = new Vector3(0f, 30f, 0f);

		[Tooltip("UI Text to display Player's Name")]
		public Text PlayerNameText;

		[Tooltip("UI Slider to display Player's Health")]
		public Slider PlayerHealthSlider;

		private PlayerManager _target;

		private float _characterControllerHeight;

		private Transform _targetTransform;

		private Renderer _targetRenderer;

		private Vector3 _targetPosition;

		private void Awake()
		{
			base.GetComponent<Transform>().SetParent(GameObject.Find("Canvas").GetComponent<Transform>());
		}

		private void Update()
		{
			if (this._target == null)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			if (this.PlayerHealthSlider != null)
			{
				this.PlayerHealthSlider.set_value(this._target.Health);
			}
		}

		private void LateUpdate()
		{
			if (this._targetRenderer != null)
			{
				base.gameObject.SetActive(this._targetRenderer.isVisible);
			}
			if (this._targetTransform != null)
			{
				this._targetPosition = this._targetTransform.position;
				this._targetPosition.y = this._targetPosition.y + this._characterControllerHeight;
				base.transform.position = Camera.main.WorldToScreenPoint(this._targetPosition) + this.ScreenOffset;
			}
		}

		public void SetTarget(PlayerManager target)
		{
			if (target == null)
			{
				Debug.LogError("<Color=Red><b>Missing</b></Color> PlayMakerManager target for PlayerUI.SetTarget.", this);
				return;
			}
			this._target = target;
			this._targetTransform = this._target.GetComponent<Transform>();
			this._targetRenderer = this._target.GetComponent<Renderer>();
			CharacterController component = this._target.GetComponent<CharacterController>();
			if (component != null)
			{
				this._characterControllerHeight = component.get_height();
			}
			if (this.PlayerNameText != null)
			{
				this.PlayerNameText.set_text(this._target.photonView.owner.NickName);
			}
		}
	}
}
