using System;
using System.Collections;
using UnityEngine;

namespace Pathfinding
{
	public class QuadtreeGraph : NavGraph
	{
		public int editorWidthLog2 = 6;

		public int editorHeightLog2 = 6;

		public LayerMask layerMask = -1;

		public float nodeSize = 1f;

		public int minDepth = 3;

		private QuadtreeNodeHolder root;

		public Vector3 center;

		private BitArray map;

		public int Width
		{
			get;
			protected set;
		}

		public int Height
		{
			get;
			protected set;
		}

		public override void GetNodes(GraphNodeDelegateCancelable del)
		{
			if (this.root == null)
			{
				return;
			}
			this.root.GetNodes(del);
		}

		public bool CheckCollision(int x, int y)
		{
			Vector3 vector = this.LocalToWorldPosition(x, y, 1);
			return !Physics.CheckSphere(vector, this.nodeSize * 1.4142f, this.layerMask);
		}

		public int CheckNode(int xs, int ys, int width)
		{
			Debug.Log(string.Concat(new object[]
			{
				"Checking Node ",
				xs,
				" ",
				ys,
				" width: ",
				width
			}));
			bool flag = this.map.get_Item(xs + ys * this.Width);
			for (int i = xs; i < xs + width; i++)
			{
				for (int j = ys; j < ys + width; j++)
				{
					if (this.map.get_Item(i + j * this.Width) != flag)
					{
						return -1;
					}
				}
			}
			return flag ? 1 : 0;
		}

		public override void ScanInternal(OnScanStatus statusCallback)
		{
			this.Width = 1 << this.editorWidthLog2;
			this.Height = 1 << this.editorHeightLog2;
			this.map = new BitArray(this.Width * this.Height);
			for (int i = 0; i < this.Width; i++)
			{
				for (int j = 0; j < this.Height; j++)
				{
					this.map.Set(i + j * this.Width, this.CheckCollision(i, j));
				}
			}
			QuadtreeNodeHolder holder = new QuadtreeNodeHolder();
			this.CreateNodeRec(holder, 0, 0, 0);
			this.root = holder;
			this.RecalculateConnectionsRec(this.root, 0, 0, 0);
		}

		public void RecalculateConnectionsRec(QuadtreeNodeHolder holder, int depth, int x, int y)
		{
			if (holder.node != null)
			{
				this.RecalculateConnections(holder, depth, x, y);
			}
			else
			{
				int num = 1 << Math.Min(this.editorHeightLog2, this.editorWidthLog2) - depth;
				this.RecalculateConnectionsRec(holder.c0, depth + 1, x, y);
				this.RecalculateConnectionsRec(holder.c1, depth + 1, x + num / 2, y);
				this.RecalculateConnectionsRec(holder.c2, depth + 1, x + num / 2, y + num / 2);
				this.RecalculateConnectionsRec(holder.c3, depth + 1, x, y + num / 2);
			}
		}

		public Vector3 LocalToWorldPosition(int x, int y, int width)
		{
			return new Vector3(((float)x + (float)width * 0.5f) * this.nodeSize, 0f, ((float)y + (float)width * 0.5f) * this.nodeSize);
		}

		public void CreateNodeRec(QuadtreeNodeHolder holder, int depth, int x, int y)
		{
			int num = 1 << Math.Min(this.editorHeightLog2, this.editorWidthLog2) - depth;
			int num2;
			if (depth < this.minDepth)
			{
				num2 = -1;
			}
			else
			{
				num2 = this.CheckNode(x, y, num);
			}
			if (num2 == 1 || num2 == 0 || num == 1)
			{
				QuadtreeNode quadtreeNode = new QuadtreeNode(this.active);
				quadtreeNode.SetPosition((VInt3)this.LocalToWorldPosition(x, y, num));
				quadtreeNode.Walkable = (num2 == 1);
				holder.node = quadtreeNode;
			}
			else
			{
				holder.c0 = new QuadtreeNodeHolder();
				holder.c1 = new QuadtreeNodeHolder();
				holder.c2 = new QuadtreeNodeHolder();
				holder.c3 = new QuadtreeNodeHolder();
				this.CreateNodeRec(holder.c0, depth + 1, x, y);
				this.CreateNodeRec(holder.c1, depth + 1, x + num / 2, y);
				this.CreateNodeRec(holder.c2, depth + 1, x + num / 2, y + num / 2);
				this.CreateNodeRec(holder.c3, depth + 1, x, y + num / 2);
			}
		}

		public void RecalculateConnections(QuadtreeNodeHolder holder, int depth, int x, int y)
		{
			if (this.root == null)
			{
				throw new InvalidOperationException("Graph contains no nodes");
			}
			if (holder.node == null)
			{
				throw new ArgumentException("No leaf node specified. Holder has no node.");
			}
			int num = 1 << Math.Min(this.editorHeightLog2, this.editorWidthLog2) - depth;
			ListLinqView<QuadtreeNode> listLinqView = new ListLinqView<QuadtreeNode>();
			ListView<QuadtreeNode> arr = listLinqView;
			QuadtreeNodeHolder holder2 = this.root;
			int depth2 = 0;
			int x2 = 0;
			int y2 = 0;
			IntRect intRect = new IntRect(x, y, x + num, y + num);
			this.AddNeighboursRec(arr, holder2, depth2, x2, y2, intRect.Expand(0), holder.node);
			holder.node.connections = listLinqView.ToArray();
			holder.node.connectionCosts = new uint[listLinqView.Count];
			for (int i = 0; i < listLinqView.Count; i++)
			{
				uint costMagnitude = (uint)(listLinqView[i].position - holder.node.position).costMagnitude;
				holder.node.connectionCosts[i] = costMagnitude;
			}
		}

		public void AddNeighboursRec(ListView<QuadtreeNode> arr, QuadtreeNodeHolder holder, int depth, int x, int y, IntRect bounds, QuadtreeNode dontInclude)
		{
			int num = 1 << Math.Min(this.editorHeightLog2, this.editorWidthLog2) - depth;
			IntRect a = new IntRect(x, y, x + num, y + num);
			if (!IntRect.Intersects(a, bounds))
			{
				return;
			}
			if (holder.node != null)
			{
				if (holder.node != dontInclude)
				{
					arr.Add(holder.node);
				}
			}
			else
			{
				this.AddNeighboursRec(arr, holder.c0, depth + 1, x, y, bounds, dontInclude);
				this.AddNeighboursRec(arr, holder.c1, depth + 1, x + num / 2, y, bounds, dontInclude);
				this.AddNeighboursRec(arr, holder.c2, depth + 1, x + num / 2, y + num / 2, bounds, dontInclude);
				this.AddNeighboursRec(arr, holder.c3, depth + 1, x, y + num / 2, bounds, dontInclude);
			}
		}

		public QuadtreeNode QueryPoint(int qx, int qy)
		{
			if (this.root == null)
			{
				return null;
			}
			QuadtreeNodeHolder quadtreeNodeHolder = this.root;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			while (quadtreeNodeHolder.node == null)
			{
				int num4 = 1 << Math.Min(this.editorHeightLog2, this.editorWidthLog2) - num;
				if (qx >= num2 + num4 / 2)
				{
					num2 += num4 / 2;
					if (qy >= num3 + num4 / 2)
					{
						num3 += num4 / 2;
						quadtreeNodeHolder = quadtreeNodeHolder.c2;
					}
					else
					{
						quadtreeNodeHolder = quadtreeNodeHolder.c1;
					}
				}
				else if (qy >= num3 + num4 / 2)
				{
					num3 += num4 / 2;
					quadtreeNodeHolder = quadtreeNodeHolder.c3;
				}
				else
				{
					quadtreeNodeHolder = quadtreeNodeHolder.c0;
				}
				num++;
			}
			return quadtreeNodeHolder.node;
		}

		public override void OnDrawGizmos(bool drawNodes)
		{
			base.OnDrawGizmos(drawNodes);
			if (!drawNodes)
			{
				return;
			}
			if (this.root != null)
			{
				this.DrawRec(this.root, 0, 0, 0, Vector3.zero);
			}
		}

		public void DrawRec(QuadtreeNodeHolder h, int depth, int x, int y, Vector3 parentPos)
		{
			int num = 1 << Math.Min(this.editorHeightLog2, this.editorWidthLog2) - depth;
			Vector3 vector = this.LocalToWorldPosition(x, y, num);
			Debug.DrawLine(vector, parentPos, Color.red);
			if (h.node != null)
			{
				Debug.DrawRay(vector, Vector3.down, h.node.Walkable ? Color.green : Color.yellow);
			}
			else
			{
				this.DrawRec(h.c0, depth + 1, x, y, vector);
				this.DrawRec(h.c1, depth + 1, x + num / 2, y, vector);
				this.DrawRec(h.c2, depth + 1, x + num / 2, y + num / 2, vector);
				this.DrawRec(h.c3, depth + 1, x, y + num / 2, vector);
			}
		}
	}
}
