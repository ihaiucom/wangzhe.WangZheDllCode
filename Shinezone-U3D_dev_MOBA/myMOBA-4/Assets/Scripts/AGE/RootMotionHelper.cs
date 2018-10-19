using System;
using UnityEngine;

namespace AGE
{
	public class RootMotionHelper : MonoBehaviour
	{
		public Transform rootTransform;

		private Vector3 posOffset = default(Vector3);

		private void Start()
		{
			this.posOffset = this.rootTransform.localPosition;
		}

		private void LateUpdate()
		{
			this.rootTransform.localPosition = this.posOffset;
		}

		public void ForceStart()
		{
			this.posOffset = this.rootTransform.localPosition;
		}

		public void ForceLateUpdate()
		{
			this.rootTransform.localPosition = this.posOffset;
		}
	}
}
