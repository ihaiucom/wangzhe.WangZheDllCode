using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public class WaypointsHolder : FuncRegion
	{
		public Color color = new Color(0f, 1f, 1f, 0.9f);

		public bool colorizeWaypoints = true;

		public float arrowHeadLength = 1f;

		public int m_index;

		[HideInInspector]
		public Waypoint[] MyWaypoints;

		public Waypoint[] wayPoints
		{
			get
			{
				return this.MyWaypoints;
			}
		}

		public Waypoint startPoint
		{
			get
			{
				return (this.wayPoints == null || this.wayPoints.Length <= 0) ? null : this.wayPoints[0];
			}
		}

		public Waypoint endPoint
		{
			get
			{
				return (this.wayPoints == null || this.wayPoints.Length <= 0) ? null : this.wayPoints[this.wayPoints.Length - 1];
			}
		}

		public Waypoint GetNextPoint(Waypoint InPoint)
		{
			if (InPoint == null)
			{
				return null;
			}
			return (InPoint.AccessIndex >= this.wayPoints.Length - 1) ? null : this.wayPoints[InPoint.AccessIndex + 1];
		}

		public Waypoint GetPrePoint(Waypoint InPoint)
		{
			if (InPoint == null)
			{
				return null;
			}
			return (InPoint.AccessIndex <= 0) ? null : this.wayPoints[InPoint.AccessIndex - 1];
		}

		private Waypoint[] FindChildrenPoints()
		{
			Waypoint[] componentsInChildren = base.GetComponentsInChildren<Waypoint>();
			if (componentsInChildren.Length > 0)
			{
				Waypoint[] array = new Waypoint[componentsInChildren.Length];
				IEnumerator enumerator = componentsInChildren.GetEnumerator();
				int num = 0;
				while (enumerator.MoveNext())
				{
					array[num] = (enumerator.Current as Waypoint);
					array[num].AccessIndex = num;
					num++;
				}
				return array;
			}
			return null;
		}

		private void Start()
		{
			if (this.MyWaypoints == null || this.MyWaypoints.Length == 0)
			{
				this.MyWaypoints = this.FindChildrenPoints();
			}
		}

		private void OnDrawGizmos()
		{
			Waypoint[] array = this.FindChildrenPoints();
			if (array != null && array.Length > 0)
			{
				Gizmos.color = this.color;
				for (int i = 0; i < array.Length - 1; i++)
				{
					Vector3 vector = array[i].gameObject.transform.position;
					Vector3 vector2 = array[i + 1].gameObject.transform.position;
					Vector3 normalized = (vector2 - vector).normalized;
					float d = Vector3.Distance(vector2, vector) - array[i + 1].radius - array[i].radius;
					vector += normalized * array[i].radius;
					vector2 = vector + normalized * d;
					DrawArrowHelper.Draw(vector, vector2, this.arrowHeadLength, 20f);
					if (this.colorizeWaypoints)
					{
						array[i + 1].color = this.color;
					}
				}
				Gizmos.DrawIcon(new Vector3(array[0].transform.position.x, array[0].transform.position.y + array[0].radius * 3.5f, array[0].transform.position.z), "StartPoint", true);
				Gizmos.DrawIcon(new Vector3(array[array.Length - 1].transform.position.x, array[array.Length - 1].transform.position.y + array[array.Length - 1].radius * 3.5f, array[array.Length - 1].transform.position.z), "EndPoint", true);
			}
		}
	}
}
