using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
	public class RecastBBTree
	{
		public RecastBBTreeBox root;

		public void QueryInBounds(Rect bounds, List<RecastMeshObj> buffer)
		{
			RecastBBTreeBox recastBBTreeBox = this.root;
			if (recastBBTreeBox == null)
			{
				return;
			}
			this.QueryBoxInBounds(recastBBTreeBox, bounds, buffer);
		}

		public void QueryBoxInBounds(RecastBBTreeBox box, Rect bounds, List<RecastMeshObj> boxes)
		{
			if (box.mesh != null)
			{
				if (this.RectIntersectsRect(box.rect, bounds))
				{
					boxes.Add(box.mesh);
				}
			}
			else
			{
				if (this.RectIntersectsRect(box.c1.rect, bounds))
				{
					this.QueryBoxInBounds(box.c1, bounds, boxes);
				}
				if (this.RectIntersectsRect(box.c2.rect, bounds))
				{
					this.QueryBoxInBounds(box.c2, bounds, boxes);
				}
			}
		}

		public bool Remove(RecastMeshObj mesh)
		{
			if (mesh == null)
			{
				throw new ArgumentNullException("mesh");
			}
			if (this.root == null)
			{
				return false;
			}
			bool result = false;
			Bounds bounds = mesh.GetBounds();
			Rect bounds2 = Rect.MinMaxRect(bounds.min.x, bounds.min.z, bounds.max.x, bounds.max.z);
			this.root = this.RemoveBox(this.root, mesh, bounds2, ref result);
			return result;
		}

		private RecastBBTreeBox RemoveBox(RecastBBTreeBox c, RecastMeshObj mesh, Rect bounds, ref bool found)
		{
			if (!this.RectIntersectsRect(c.rect, bounds))
			{
				return c;
			}
			if (c.mesh == mesh)
			{
				found = true;
				return null;
			}
			if (c.mesh == null && !found)
			{
				c.c1 = this.RemoveBox(c.c1, mesh, bounds, ref found);
				if (c.c1 == null)
				{
					return c.c2;
				}
				if (!found)
				{
					c.c2 = this.RemoveBox(c.c2, mesh, bounds, ref found);
					if (c.c2 == null)
					{
						return c.c1;
					}
				}
				if (found)
				{
					c.rect = this.ExpandToContain(c.c1.rect, c.c2.rect);
				}
			}
			return c;
		}

		public void Insert(RecastMeshObj mesh)
		{
			RecastBBTreeBox recastBBTreeBox = new RecastBBTreeBox(this, mesh);
			if (this.root == null)
			{
				this.root = recastBBTreeBox;
				return;
			}
			RecastBBTreeBox recastBBTreeBox2 = this.root;
			while (true)
			{
				recastBBTreeBox2.rect = this.ExpandToContain(recastBBTreeBox2.rect, recastBBTreeBox.rect);
				if (recastBBTreeBox2.mesh != null)
				{
					break;
				}
				float num = this.ExpansionRequired(recastBBTreeBox2.c1.rect, recastBBTreeBox.rect);
				float num2 = this.ExpansionRequired(recastBBTreeBox2.c2.rect, recastBBTreeBox.rect);
				if (num < num2)
				{
					recastBBTreeBox2 = recastBBTreeBox2.c1;
				}
				else if (num2 < num)
				{
					recastBBTreeBox2 = recastBBTreeBox2.c2;
				}
				else
				{
					recastBBTreeBox2 = ((this.RectArea(recastBBTreeBox2.c1.rect) < this.RectArea(recastBBTreeBox2.c2.rect)) ? recastBBTreeBox2.c1 : recastBBTreeBox2.c2);
				}
			}
			recastBBTreeBox2.c1 = recastBBTreeBox;
			RecastBBTreeBox c = new RecastBBTreeBox(this, recastBBTreeBox2.mesh);
			recastBBTreeBox2.c2 = c;
			recastBBTreeBox2.mesh = null;
		}

		public void OnDrawGizmos()
		{
		}

		public void OnDrawGizmos(RecastBBTreeBox box)
		{
			if (box == null)
			{
				return;
			}
			Vector3 a = new Vector3(box.rect.xMin, 0f, box.rect.yMin);
			Vector3 vector = new Vector3(box.rect.xMax, 0f, box.rect.yMax);
			Vector3 vector2 = (a + vector) * 0.5f;
			Vector3 vector3 = (vector - vector2) * 2f;
			Gizmos.DrawCube(vector2, vector3);
			this.OnDrawGizmos(box.c1);
			this.OnDrawGizmos(box.c2);
		}

		public void TestIntersections(Vector3 p, float radius)
		{
			RecastBBTreeBox box = this.root;
			this.TestIntersections(box, p, radius);
		}

		public void TestIntersections(RecastBBTreeBox box, Vector3 p, float radius)
		{
			if (box == null)
			{
				return;
			}
			this.RectIntersectsCircle(box.rect, p, radius);
			this.TestIntersections(box.c1, p, radius);
			this.TestIntersections(box.c2, p, radius);
		}

		public bool RectIntersectsRect(Rect r, Rect r2)
		{
			return r.xMax > r2.xMin && r.yMax > r2.yMin && r2.xMax > r.xMin && r2.yMax > r.yMin;
		}

		public bool RectIntersectsCircle(Rect r, Vector3 p, float radius)
		{
			return float.IsPositiveInfinity(radius) || this.RectContains(r, p) || this.XIntersectsCircle(r.xMin, r.xMax, r.yMin, p, radius) || this.XIntersectsCircle(r.xMin, r.xMax, r.yMax, p, radius) || this.ZIntersectsCircle(r.yMin, r.yMax, r.xMin, p, radius) || this.ZIntersectsCircle(r.yMin, r.yMax, r.xMax, p, radius);
		}

		public bool RectContains(Rect r, Vector3 p)
		{
			return p.x >= r.xMin && p.x <= r.xMax && p.z >= r.yMin && p.z <= r.yMax;
		}

		public bool ZIntersectsCircle(float z1, float z2, float xpos, Vector3 circle, float radius)
		{
			double num = (double)(Math.Abs(xpos - circle.x) / radius);
			if (num > 1.0 || num < -1.0)
			{
				return false;
			}
			float num2 = (float)Math.Sqrt(1.0 - num * num) * radius;
			float num3 = circle.z - num2;
			num2 += circle.z;
			float num4 = Math.Min(num2, num3);
			float num5 = Math.Max(num2, num3);
			num4 = Mathf.Max(z1, num4);
			num5 = Mathf.Min(z2, num5);
			return num5 > num4;
		}

		public bool XIntersectsCircle(float x1, float x2, float zpos, Vector3 circle, float radius)
		{
			double num = (double)(Math.Abs(zpos - circle.z) / radius);
			if (num > 1.0 || num < -1.0)
			{
				return false;
			}
			float num2 = (float)Math.Sqrt(1.0 - num * num) * radius;
			float num3 = circle.x - num2;
			num2 += circle.x;
			float num4 = Math.Min(num2, num3);
			float num5 = Math.Max(num2, num3);
			num4 = Mathf.Max(x1, num4);
			num5 = Mathf.Min(x2, num5);
			return num5 > num4;
		}

		public float ExpansionRequired(Rect r, Rect r2)
		{
			float num = Mathf.Min(r.xMin, r2.xMin);
			float num2 = Mathf.Max(r.xMax, r2.xMax);
			float num3 = Mathf.Min(r.yMin, r2.yMin);
			float num4 = Mathf.Max(r.yMax, r2.yMax);
			return (num2 - num) * (num4 - num3) - this.RectArea(r);
		}

		public Rect ExpandToContain(Rect r, Rect r2)
		{
			float left = Mathf.Min(r.xMin, r2.xMin);
			float right = Mathf.Max(r.xMax, r2.xMax);
			float top = Mathf.Min(r.yMin, r2.yMin);
			float bottom = Mathf.Max(r.yMax, r2.yMax);
			return Rect.MinMaxRect(left, top, right, bottom);
		}

		public float RectArea(Rect r)
		{
			return r.width * r.height;
		}

		public void ToString()
		{
			RecastBBTreeBox recastBBTreeBox = this.root;
			Stack<RecastBBTreeBox> stack = new Stack<RecastBBTreeBox>();
			stack.Push(recastBBTreeBox);
			recastBBTreeBox.WriteChildren(0);
		}
	}
}
