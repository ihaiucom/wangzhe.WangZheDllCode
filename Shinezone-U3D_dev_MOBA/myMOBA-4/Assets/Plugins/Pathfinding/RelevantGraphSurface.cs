using System;
using UnityEngine;

namespace Pathfinding
{
	[AddComponentMenu("Pathfinding/Navmesh/RelevantGraphSurface")]
	public class RelevantGraphSurface : MonoBehaviour
	{
		private static RelevantGraphSurface root;

		public float maxRange = 1f;

		private RelevantGraphSurface prev;

		private RelevantGraphSurface next;

		private Vector3 position;

		public Vector3 Position
		{
			get
			{
				return this.position;
			}
		}

		public RelevantGraphSurface Next
		{
			get
			{
				return this.next;
			}
		}

		public RelevantGraphSurface Prev
		{
			get
			{
				return this.prev;
			}
		}

		public static RelevantGraphSurface Root
		{
			get
			{
				return RelevantGraphSurface.root;
			}
		}

		public void UpdatePosition()
		{
			this.position = base.transform.position;
		}

		private void OnEnable()
		{
			this.UpdatePosition();
			if (RelevantGraphSurface.root == null)
			{
				RelevantGraphSurface.root = this;
			}
			else
			{
				this.next = RelevantGraphSurface.root;
				RelevantGraphSurface.root.prev = this;
				RelevantGraphSurface.root = this;
			}
		}

		private void OnDisable()
		{
			if (RelevantGraphSurface.root == this)
			{
				RelevantGraphSurface.root = this.next;
				if (RelevantGraphSurface.root != null)
				{
					RelevantGraphSurface.root.prev = null;
				}
			}
			else
			{
				if (this.prev != null)
				{
					this.prev.next = this.next;
				}
				if (this.next != null)
				{
					this.next.prev = this.prev;
				}
			}
			this.prev = null;
			this.next = null;
		}

		public static void UpdateAllPositions()
		{
			RelevantGraphSurface relevantGraphSurface = RelevantGraphSurface.root;
			while (relevantGraphSurface != null)
			{
				relevantGraphSurface.UpdatePosition();
				relevantGraphSurface = relevantGraphSurface.Next;
			}
		}

		public static void FindAllGraphSurfaces()
		{
			RelevantGraphSurface[] array = UnityEngine.Object.FindObjectsOfType(typeof(RelevantGraphSurface)) as RelevantGraphSurface[];
			for (int i = 0; i < array.Length; i++)
			{
				array[i].OnDisable();
				array[i].OnEnable();
			}
		}

		public void OnDrawGizmos()
		{
			Gizmos.color = new Color(0.223529413f, 0.827451f, 0.180392161f, 0.4f);
			Gizmos.DrawLine(base.transform.position - Vector3.up * this.maxRange, base.transform.position + Vector3.up * this.maxRange);
		}

		public void OnDrawGizmosSelected()
		{
			Gizmos.color = new Color(0.223529413f, 0.827451f, 0.180392161f);
			Gizmos.DrawLine(base.transform.position - Vector3.up * this.maxRange, base.transform.position + Vector3.up * this.maxRange);
		}
	}
}
