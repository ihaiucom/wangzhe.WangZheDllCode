using System;
using UnityEngine;

namespace Pathfinding
{
	public class BBTree
	{
		private struct BBTreeBox
		{
			public Rect rect;

			public MeshNode node;

			public int left;

			public int right;

			public bool IsLeaf
			{
				get
				{
					return this.node != null;
				}
			}

			public BBTreeBox(BBTree tree, MeshNode node)
			{
				this.node = node;
				Vector3 vector = (Vector3)node.GetVertex(0);
				Vector2 vector2 = new Vector2(vector.x, vector.z);
				Vector2 vector3 = vector2;
				for (int i = 1; i < node.GetVertexCount(); i++)
				{
					Vector3 vector4 = (Vector3)node.GetVertex(i);
					vector2.x = Math.Min(vector2.x, vector4.x);
					vector2.y = Math.Min(vector2.y, vector4.z);
					vector3.x = Math.Max(vector3.x, vector4.x);
					vector3.y = Math.Max(vector3.y, vector4.z);
				}
				this.rect = Rect.MinMaxRect(vector2.x, vector2.y, vector3.x, vector3.y);
				this.left = (this.right = -1);
			}

			public bool Contains(Vector3 p)
			{
				return this.rect.Contains(new Vector2(p.x, p.z));
			}
		}

		private BBTree.BBTreeBox[] arr = new BBTree.BBTreeBox[6];

		private int count;

		public INavmeshHolder graph;

		public Rect Size
		{
			get
			{
				return (this.count != 0) ? this.arr[0].rect : new Rect(0f, 0f, 0f, 0f);
			}
		}

		public BBTree(INavmeshHolder graph)
		{
			this.graph = graph;
		}

		public void Clear()
		{
			this.count = 0;
		}

		private void EnsureCapacity(int c)
		{
			if (this.arr.Length < c)
			{
				BBTree.BBTreeBox[] array = new BBTree.BBTreeBox[Math.Max(c, (int)((float)this.arr.Length * 1.5f))];
				for (int i = 0; i < this.count; i++)
				{
					array[i] = this.arr[i];
				}
				this.arr = array;
			}
		}

		private int GetBox(MeshNode node)
		{
			if (this.count >= this.arr.Length)
			{
				this.EnsureCapacity(this.count + 1);
			}
			this.arr[this.count] = new BBTree.BBTreeBox(this, node);
			this.count++;
			return this.count - 1;
		}

		public void Insert(MeshNode node)
		{
			int box = this.GetBox(node);
			if (box == 0)
			{
				return;
			}
			BBTree.BBTreeBox bBTreeBox = this.arr[box];
			int num = 0;
			BBTree.BBTreeBox bBTreeBox2;
			while (true)
			{
				bBTreeBox2 = this.arr[num];
				bBTreeBox2.rect = BBTree.ExpandToContain(bBTreeBox2.rect, bBTreeBox.rect);
				if (bBTreeBox2.node != null)
				{
					break;
				}
				this.arr[num] = bBTreeBox2;
				float num2 = BBTree.ExpansionRequired(this.arr[bBTreeBox2.left].rect, bBTreeBox.rect);
				float num3 = BBTree.ExpansionRequired(this.arr[bBTreeBox2.right].rect, bBTreeBox.rect);
				if (num2 < num3)
				{
					num = bBTreeBox2.left;
				}
				else if (num3 < num2)
				{
					num = bBTreeBox2.right;
				}
				else
				{
					num = ((BBTree.RectArea(this.arr[bBTreeBox2.left].rect) < BBTree.RectArea(this.arr[bBTreeBox2.right].rect)) ? bBTreeBox2.left : bBTreeBox2.right);
				}
			}
			bBTreeBox2.left = box;
			int box2 = this.GetBox(bBTreeBox2.node);
			bBTreeBox2.right = box2;
			bBTreeBox2.node = null;
			this.arr[num] = bBTreeBox2;
		}

		public NNInfo Query(Vector3 p, NNConstraint constraint)
		{
			if (this.count == 0)
			{
				return new NNInfo(null);
			}
			NNInfo result = default(NNInfo);
			this.SearchBox(0, p, constraint, ref result);
			result.UpdateInfo();
			return result;
		}

		public NNInfo QueryCircle(Vector3 p, float radius, NNConstraint constraint)
		{
			if (this.count == 0)
			{
				return new NNInfo(null);
			}
			NNInfo result = new NNInfo(null);
			this.SearchBoxCircle(0, p, radius, constraint, ref result);
			result.UpdateInfo();
			return result;
		}

		public NNInfo QueryClosest(Vector3 p, NNConstraint constraint, out float distance)
		{
			distance = float.PositiveInfinity;
			return this.QueryClosest(p, constraint, ref distance, new NNInfo(null));
		}

		public NNInfo QueryClosestXZ(Vector3 p, NNConstraint constraint, ref float distance, NNInfo previous)
		{
			if (this.count == 0)
			{
				return previous;
			}
			this.SearchBoxClosestXZ(0, p, ref distance, constraint, ref previous);
			return previous;
		}

		private void SearchBoxClosestXZ(int boxi, Vector3 p, ref float closestDist, NNConstraint constraint, ref NNInfo nnInfo)
		{
			BBTree.BBTreeBox bBTreeBox = this.arr[boxi];
			if (bBTreeBox.node != null)
			{
				Vector3 constClampedPosition = bBTreeBox.node.ClosestPointOnNodeXZ(p);
				float num = (constClampedPosition.x - p.x) * (constClampedPosition.x - p.x) + (constClampedPosition.z - p.z) * (constClampedPosition.z - p.z);
				if (constraint == null || constraint.Suitable(bBTreeBox.node))
				{
					if (nnInfo.constrainedNode == null)
					{
						nnInfo.constrainedNode = bBTreeBox.node;
						nnInfo.constClampedPosition = constClampedPosition;
						closestDist = (float)Math.Sqrt((double)num);
					}
					else if (num < closestDist * closestDist)
					{
						nnInfo.constrainedNode = bBTreeBox.node;
						nnInfo.constClampedPosition = constClampedPosition;
						closestDist = (float)Math.Sqrt((double)num);
					}
				}
			}
			else
			{
				if (BBTree.RectIntersectsCircle(this.arr[bBTreeBox.left].rect, p, closestDist))
				{
					this.SearchBoxClosestXZ(bBTreeBox.left, p, ref closestDist, constraint, ref nnInfo);
				}
				if (BBTree.RectIntersectsCircle(this.arr[bBTreeBox.right].rect, p, closestDist))
				{
					this.SearchBoxClosestXZ(bBTreeBox.right, p, ref closestDist, constraint, ref nnInfo);
				}
			}
		}

		public NNInfo QueryClosest(Vector3 p, NNConstraint constraint, ref float distance, NNInfo previous)
		{
			if (this.count == 0)
			{
				return previous;
			}
			this.SearchBoxClosest(0, p, ref distance, constraint, ref previous);
			return previous;
		}

		private void SearchBoxClosest(int boxi, Vector3 p, ref float closestDist, NNConstraint constraint, ref NNInfo nnInfo)
		{
			BBTree.BBTreeBox bBTreeBox = this.arr[boxi];
			if (bBTreeBox.node != null)
			{
				if (BBTree.NodeIntersectsCircle(bBTreeBox.node, p, closestDist))
				{
					Vector3 vector = bBTreeBox.node.ClosestPointOnNode(p);
					float sqrMagnitude = (vector - p).sqrMagnitude;
					if (constraint == null || constraint.Suitable(bBTreeBox.node))
					{
						if (nnInfo.constrainedNode == null)
						{
							nnInfo.constrainedNode = bBTreeBox.node;
							nnInfo.constClampedPosition = vector;
							closestDist = (float)Math.Sqrt((double)sqrMagnitude);
						}
						else if (sqrMagnitude < closestDist * closestDist)
						{
							nnInfo.constrainedNode = bBTreeBox.node;
							nnInfo.constClampedPosition = vector;
							closestDist = (float)Math.Sqrt((double)sqrMagnitude);
						}
					}
				}
			}
			else
			{
				if (BBTree.RectIntersectsCircle(this.arr[bBTreeBox.left].rect, p, closestDist))
				{
					this.SearchBoxClosest(bBTreeBox.left, p, ref closestDist, constraint, ref nnInfo);
				}
				if (BBTree.RectIntersectsCircle(this.arr[bBTreeBox.right].rect, p, closestDist))
				{
					this.SearchBoxClosest(bBTreeBox.right, p, ref closestDist, constraint, ref nnInfo);
				}
			}
		}

		public MeshNode QueryInside(Vector3 p, NNConstraint constraint)
		{
			if (this.count == 0)
			{
				return null;
			}
			return this.SearchBoxInside(0, p, constraint);
		}

		private MeshNode SearchBoxInside(int boxi, Vector3 p, NNConstraint constraint)
		{
			BBTree.BBTreeBox bBTreeBox = this.arr[boxi];
			if (bBTreeBox.node != null)
			{
				if (bBTreeBox.node.ContainsPoint((VInt3)p) && (constraint == null || constraint.Suitable(bBTreeBox.node)))
				{
					return bBTreeBox.node;
				}
			}
			else
			{
				if (this.arr[bBTreeBox.left].rect.Contains(new Vector2(p.x, p.z)))
				{
					MeshNode meshNode = this.SearchBoxInside(bBTreeBox.left, p, constraint);
					if (meshNode != null)
					{
						return meshNode;
					}
				}
				if (this.arr[bBTreeBox.right].rect.Contains(new Vector2(p.x, p.z)))
				{
					MeshNode meshNode2 = this.SearchBoxInside(bBTreeBox.right, p, constraint);
					if (meshNode2 != null)
					{
						return meshNode2;
					}
				}
			}
			return null;
		}

		private void SearchBoxCircle(int boxi, Vector3 p, float radius, NNConstraint constraint, ref NNInfo nnInfo)
		{
			BBTree.BBTreeBox bBTreeBox = this.arr[boxi];
			if (bBTreeBox.node != null)
			{
				if (BBTree.NodeIntersectsCircle(bBTreeBox.node, p, radius))
				{
					Vector3 vector = bBTreeBox.node.ClosestPointOnNode(p);
					float sqrMagnitude = (vector - p).sqrMagnitude;
					if (nnInfo.node == null)
					{
						nnInfo.node = bBTreeBox.node;
						nnInfo.clampedPosition = vector;
					}
					else if (sqrMagnitude < (nnInfo.clampedPosition - p).sqrMagnitude)
					{
						nnInfo.node = bBTreeBox.node;
						nnInfo.clampedPosition = vector;
					}
					if (constraint == null || constraint.Suitable(bBTreeBox.node))
					{
						if (nnInfo.constrainedNode == null)
						{
							nnInfo.constrainedNode = bBTreeBox.node;
							nnInfo.constClampedPosition = vector;
						}
						else if (sqrMagnitude < (nnInfo.constClampedPosition - p).sqrMagnitude)
						{
							nnInfo.constrainedNode = bBTreeBox.node;
							nnInfo.constClampedPosition = vector;
						}
					}
				}
				return;
			}
			if (BBTree.RectIntersectsCircle(this.arr[bBTreeBox.left].rect, p, radius))
			{
				this.SearchBoxCircle(bBTreeBox.left, p, radius, constraint, ref nnInfo);
			}
			if (BBTree.RectIntersectsCircle(this.arr[bBTreeBox.right].rect, p, radius))
			{
				this.SearchBoxCircle(bBTreeBox.right, p, radius, constraint, ref nnInfo);
			}
		}

		private void SearchBox(int boxi, Vector3 p, NNConstraint constraint, ref NNInfo nnInfo)
		{
			BBTree.BBTreeBox bBTreeBox = this.arr[boxi];
			if (bBTreeBox.node != null)
			{
				if (bBTreeBox.node.ContainsPoint((VInt3)p))
				{
					if (nnInfo.node == null)
					{
						nnInfo.node = bBTreeBox.node;
					}
					else if (Mathf.Abs(((Vector3)bBTreeBox.node.position).y - p.y) < Mathf.Abs(((Vector3)nnInfo.node.position).y - p.y))
					{
						nnInfo.node = bBTreeBox.node;
					}
					if (constraint.Suitable(bBTreeBox.node))
					{
						if (nnInfo.constrainedNode == null)
						{
							nnInfo.constrainedNode = bBTreeBox.node;
						}
						else if (Mathf.Abs((float)bBTreeBox.node.position.y - p.y) < Mathf.Abs((float)nnInfo.constrainedNode.position.y - p.y))
						{
							nnInfo.constrainedNode = bBTreeBox.node;
						}
					}
				}
				return;
			}
			if (BBTree.RectContains(this.arr[bBTreeBox.left].rect, p))
			{
				this.SearchBox(bBTreeBox.left, p, constraint, ref nnInfo);
			}
			if (BBTree.RectContains(this.arr[bBTreeBox.right].rect, p))
			{
				this.SearchBox(bBTreeBox.right, p, constraint, ref nnInfo);
			}
		}

		public void OnDrawGizmos()
		{
			Gizmos.color = new Color(1f, 1f, 1f, 0.5f);
			if (this.count == 0)
			{
				return;
			}
		}

		private void OnDrawGizmos(int boxi, int depth)
		{
			BBTree.BBTreeBox bBTreeBox = this.arr[boxi];
			Vector3 a = new Vector3(bBTreeBox.rect.xMin, 0f, bBTreeBox.rect.yMin);
			Vector3 vector = new Vector3(bBTreeBox.rect.xMax, 0f, bBTreeBox.rect.yMax);
			Vector3 vector2 = (a + vector) * 0.5f;
			Vector3 vector3 = (vector - vector2) * 2f;
			vector2.y += (float)depth * 0.2f;
			Gizmos.color = AstarMath.IntToColor(depth, 0.05f);
			Gizmos.DrawCube(vector2, vector3);
			if (bBTreeBox.node == null)
			{
				this.OnDrawGizmos(bBTreeBox.left, depth + 1);
				this.OnDrawGizmos(bBTreeBox.right, depth + 1);
			}
		}

		private static bool NodeIntersectsCircle(MeshNode node, Vector3 p, float radius)
		{
			return float.IsPositiveInfinity(radius) || (p - node.ClosestPointOnNode(p)).sqrMagnitude < radius * radius;
		}

		private static bool RectIntersectsCircle(Rect r, Vector3 p, float radius)
		{
			if (float.IsPositiveInfinity(radius))
			{
				return true;
			}
			Vector3 vector = p;
			p.x = Math.Max(p.x, r.xMin);
			p.x = Math.Min(p.x, r.xMax);
			p.z = Math.Max(p.z, r.yMin);
			p.z = Math.Min(p.z, r.yMax);
			return (p.x - vector.x) * (p.x - vector.x) + (p.z - vector.z) * (p.z - vector.z) < radius * radius;
		}

		private static bool RectContains(Rect r, Vector3 p)
		{
			return p.x >= r.xMin && p.x <= r.xMax && p.z >= r.yMin && p.z <= r.yMax;
		}

		private static float ExpansionRequired(Rect r, Rect r2)
		{
			float num = Math.Min(r.xMin, r2.xMin);
			float num2 = Math.Max(r.xMax, r2.xMax);
			float num3 = Math.Min(r.yMin, r2.yMin);
			float num4 = Math.Max(r.yMax, r2.yMax);
			return (num2 - num) * (num4 - num3) - BBTree.RectArea(r);
		}

		private static Rect ExpandToContain(Rect r, Rect r2)
		{
			float left = Math.Min(r.xMin, r2.xMin);
			float right = Math.Max(r.xMax, r2.xMax);
			float top = Math.Min(r.yMin, r2.yMin);
			float bottom = Math.Max(r.yMax, r2.yMax);
			return Rect.MinMaxRect(left, top, right, bottom);
		}

		private static float RectArea(Rect r)
		{
			return r.width * r.height;
		}
	}
}
