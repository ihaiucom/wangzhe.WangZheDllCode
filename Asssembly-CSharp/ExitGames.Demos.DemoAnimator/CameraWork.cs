using System;
using UnityEngine;

namespace ExitGames.Demos.DemoAnimator
{
	public class CameraWork : MonoBehaviour
	{
		[Tooltip("The distance in the local x-z plane to the target")]
		public float distance = 7f;

		[Tooltip("The height we want the camera to be above the target")]
		public float height = 3f;

		[Tooltip("The Smooth time lag for the height of the camera.")]
		public float heightSmoothLag = 0.3f;

		[Tooltip("Allow the camera to be offseted vertically from the target, for example giving more view of the sceneray and less ground.")]
		public Vector3 centerOffset = Vector3.zero;

		[Tooltip("Set this as false if a component of a prefab being instanciated by Photon Network, and manually call OnStartFollowing() when and if needed.")]
		public bool followOnStart;

		private Transform cameraTransform;

		private bool isFollowing;

		private float heightVelocity;

		private float targetHeight = 100000f;

		private void Start()
		{
			if (this.followOnStart)
			{
				this.OnStartFollowing();
			}
		}

		private void LateUpdate()
		{
			if (this.cameraTransform == null && this.isFollowing)
			{
				this.OnStartFollowing();
			}
			if (this.isFollowing)
			{
				this.Apply();
			}
		}

		public void OnStartFollowing()
		{
			this.cameraTransform = Camera.main.transform;
			this.isFollowing = true;
			this.Cut();
		}

		private void Apply()
		{
			Vector3 vector = base.transform.position + this.centerOffset;
			float y = base.transform.eulerAngles.y;
			float y2 = this.cameraTransform.eulerAngles.y;
			float num = y;
			y2 = num;
			this.targetHeight = vector.y + this.height;
			float num2 = this.cameraTransform.position.y;
			num2 = Mathf.SmoothDamp(num2, this.targetHeight, ref this.heightVelocity, this.heightSmoothLag);
			Quaternion rotation = Quaternion.Euler(0f, y2, 0f);
			this.cameraTransform.position = vector;
			this.cameraTransform.position += rotation * Vector3.back * this.distance;
			this.cameraTransform.position = new Vector3(this.cameraTransform.position.x, num2, this.cameraTransform.position.z);
			this.SetUpRotation(vector);
		}

		private void Cut()
		{
			float num = this.heightSmoothLag;
			this.heightSmoothLag = 0.001f;
			this.Apply();
			this.heightSmoothLag = num;
		}

		private void SetUpRotation(Vector3 centerPos)
		{
			Vector3 position = this.cameraTransform.position;
			Vector3 vector = centerPos - position;
			Quaternion lhs = Quaternion.LookRotation(new Vector3(vector.x, 0f, vector.z));
			Vector3 forward = Vector3.forward * this.distance + Vector3.down * this.height;
			this.cameraTransform.rotation = lhs * Quaternion.LookRotation(forward);
		}
	}
}
