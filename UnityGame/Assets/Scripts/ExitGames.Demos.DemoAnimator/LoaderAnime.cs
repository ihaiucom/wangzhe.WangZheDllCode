using System;
using UnityEngine;

namespace ExitGames.Demos.DemoAnimator
{
	public class LoaderAnime : MonoBehaviour
	{
		[Tooltip("Angular Speed in degrees per seconds")]
		public float speed = 180f;

		[Tooltip("Radius os the loader")]
		public float radius = 1f;

		public GameObject particles;

		private Vector3 _offset;

		private Transform _transform;

		private Transform _particleTransform;

		private bool _isAnimating;

		private void Awake()
		{
			this._particleTransform = this.particles.GetComponent<Transform>();
			this._transform = base.GetComponent<Transform>();
		}

		private void Update()
		{
			if (this._isAnimating)
			{
				this._transform.Rotate(0f, 0f, this.speed * Time.deltaTime);
				this._particleTransform.localPosition = Vector3.MoveTowards(this._particleTransform.localPosition, this._offset, 0.5f * Time.deltaTime);
			}
		}

		public void StartLoaderAnimation()
		{
			this._isAnimating = true;
			this._offset = new Vector3(this.radius, 0f, 0f);
			this.particles.SetActive(true);
		}

		public void StopLoaderAnimation()
		{
			this.particles.SetActive(false);
		}
	}
}
