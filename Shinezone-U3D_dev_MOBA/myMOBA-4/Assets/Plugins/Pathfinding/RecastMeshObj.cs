using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
	[AddComponentMenu("Pathfinding/Navmesh/RecastMeshObj")]
	public class RecastMeshObj : MonoBehaviour
	{
		protected static RecastBBTree tree = new RecastBBTree();

		protected static List<RecastMeshObj> dynamicMeshObjs = new List<RecastMeshObj>();

		[HideInInspector]
		public Bounds bounds;

		public bool dynamic;

		public int area;

		private bool _dynamic;

		private bool registered;

		public static void GetAllInBounds(List<RecastMeshObj> buffer, Bounds bounds)
		{
			if (!Application.isPlaying)
			{
				RecastMeshObj[] array = UnityEngine.Object.FindObjectsOfType(typeof(RecastMeshObj)) as RecastMeshObj[];
				for (int i = 0; i < array.Length; i++)
				{
					array[i].RecalculateBounds();
					if (array[i].GetBounds().Intersects(bounds))
					{
						buffer.Add(array[i]);
					}
				}
				return;
			}
			if (Time.timeSinceLevelLoad == 0f)
			{
				RecastMeshObj[] array2 = UnityEngine.Object.FindObjectsOfType(typeof(RecastMeshObj)) as RecastMeshObj[];
				for (int j = 0; j < array2.Length; j++)
				{
					array2[j].Register();
				}
			}
			for (int k = 0; k < RecastMeshObj.dynamicMeshObjs.Count; k++)
			{
				if (RecastMeshObj.dynamicMeshObjs[k].GetBounds().Intersects(bounds))
				{
					buffer.Add(RecastMeshObj.dynamicMeshObjs[k]);
				}
			}
			Rect rect = Rect.MinMaxRect(bounds.min.x, bounds.min.z, bounds.max.x, bounds.max.z);
			RecastMeshObj.tree.QueryInBounds(rect, buffer);
		}

		private void OnEnable()
		{
			this.Register();
		}

		private void Register()
		{
			if (this.registered)
			{
				return;
			}
			this.registered = true;
			this.area = Mathf.Clamp(this.area, -1, 33554432);
			Renderer component = base.GetComponent<Renderer>();
			Collider component2 = base.GetComponent<Collider>();
			if (component == null && component2 == null)
			{
				throw new Exception("A renderer or a collider should be attached to the GameObject");
			}
			MeshFilter component3 = base.GetComponent<MeshFilter>();
			if (component != null && component3 == null)
			{
				throw new Exception("A renderer was attached but no mesh filter");
			}
			if (component != null)
			{
				this.bounds = component.bounds;
			}
			else
			{
				this.bounds = component2.bounds;
			}
			this._dynamic = this.dynamic;
			if (this._dynamic)
			{
				RecastMeshObj.dynamicMeshObjs.Add(this);
			}
			else
			{
				RecastMeshObj.tree.Insert(this);
			}
		}

		private void RecalculateBounds()
		{
			Renderer component = base.GetComponent<Renderer>();
			Collider collider = this.GetCollider();
			if (component == null && collider == null)
			{
				throw new Exception("A renderer or a collider should be attached to the GameObject");
			}
			MeshFilter component2 = base.GetComponent<MeshFilter>();
			if (component != null && component2 == null)
			{
				throw new Exception("A renderer was attached but no mesh filter");
			}
			if (component != null)
			{
				this.bounds = component.bounds;
			}
			else
			{
				this.bounds = collider.bounds;
			}
		}

		public Bounds GetBounds()
		{
			if (this._dynamic)
			{
				this.RecalculateBounds();
			}
			return this.bounds;
		}

		public MeshFilter GetMeshFilter()
		{
			return base.GetComponent<MeshFilter>();
		}

		public Collider GetCollider()
		{
			return base.GetComponent<Collider>();
		}

		private void OnDisable()
		{
			this.registered = false;
			if (this._dynamic)
			{
				RecastMeshObj.dynamicMeshObjs.Remove(this);
			}
			else if (!RecastMeshObj.tree.Remove(this))
			{
				throw new Exception("Could not remove RecastMeshObj from tree even though it should exist in it. Has the object moved without being marked as dynamic?");
			}
			this._dynamic = this.dynamic;
		}
	}
}
