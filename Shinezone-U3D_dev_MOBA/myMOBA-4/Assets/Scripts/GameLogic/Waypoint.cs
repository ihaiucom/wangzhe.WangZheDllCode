using System;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public class Waypoint : MonoBehaviour
	{
		public Color color = new Color(1f, 0f, 0f);

		public float radius = 0.25f;

		[HideInInspector]
		public int AccessIndex = -1;

		public void OnDrawGizmos()
		{
			Gizmos.color = this.color;
			Gizmos.DrawSphere(base.transform.position, this.radius);
		}
	}
}
