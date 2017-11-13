using Pathfinding;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GraphNodeRasterizer
{
	private struct Edge
	{
		public int x0;

		public int y0;

		public int x1;

		public int y1;
	}

	public List<object>[] cells;

	public VInt2 origin;

	public int cellSize;

	public int numCellsX;

	public int numCellsY;

	public int SizeX;

	public int SizeY;

	private GraphNodeRasterizer.Edge[] edges = new GraphNodeRasterizer.Edge[3];

	private float[] tempVar = new float[4];

	public int DebugNodeID = -1;

	private Color[] debugColors = new Color[]
	{
		new Color(0f, 1f, 0f, 0.5f),
		new Color(1f, 0f, 0f, 0.5f),
		new Color(0f, 0f, 1f, 0.5f),
		new Color(1f, 1f, 0f, 0.5f)
	};

	private void InitEdge(ref VInt2 v0, ref VInt2 v1, int index)
	{
		GraphNodeRasterizer.Edge edge = default(GraphNodeRasterizer.Edge);
		if (v0.y < v1.y)
		{
			edge.x0 = v0.x;
			edge.y0 = v0.y;
			edge.x1 = v1.x;
			edge.y1 = v1.y;
		}
		else
		{
			edge.x0 = v1.x;
			edge.y0 = v1.y;
			edge.x1 = v0.x;
			edge.y1 = v0.y;
		}
		this.edges[index] = edge;
	}

	private int GetLongEdge()
	{
		int num = this.edges[0].y1 - this.edges[0].y0;
		int num2 = this.edges[1].y1 - this.edges[1].y0;
		int num3 = this.edges[2].y1 - this.edges[2].y0;
		if (num > num2)
		{
			return (num > num3) ? 0 : 2;
		}
		return (num2 > num3) ? 1 : 2;
	}

	public void Init(VInt2 pos, int sizeX, int sizeY, int inCellSize)
	{
		this.cellSize = inCellSize;
		this.numCellsX = sizeX / this.cellSize + 1;
		this.numCellsY = sizeY / this.cellSize + 1;
		this.SizeX = this.numCellsX * this.cellSize;
		this.SizeY = this.numCellsY * this.cellSize;
		this.origin = pos;
		this.origin.x = this.origin.x - (this.SizeX - sizeX) / 2;
		this.origin.y = this.origin.y - (this.SizeY - sizeY) / 2;
		this.cells = new List<object>[this.numCellsX * this.numCellsY];
	}

	public void AddTriangle(ref VInt2 v0, ref VInt2 v1, ref VInt2 v2, object data)
	{
		this.InitEdge(ref v0, ref v1, 0);
		this.InitEdge(ref v1, ref v2, 1);
		this.InitEdge(ref v2, ref v0, 2);
		int longEdge = this.GetLongEdge();
		int num = (longEdge + 1) % 3;
		int num2 = (longEdge + 2) % 3;
		this.AddEdge(ref this.edges[longEdge], ref this.edges[num], data);
		this.AddEdge(ref this.edges[longEdge], ref this.edges[num2], data);
	}

	private void AddEdge(ref GraphNodeRasterizer.Edge e0, ref GraphNodeRasterizer.Edge e1, object data)
	{
		if (e0.y0 == e0.y1 || e1.y0 == e1.y1)
		{
			return;
		}
		float num = (float)(e0.y1 - e0.y0) * 0.001f;
		float num2 = (float)(e1.y1 - e1.y0) * 0.001f;
		float num3 = (float)(e0.x1 - e0.x0) * 0.001f;
		float num4 = (float)(e1.x1 - e1.x0) * 0.001f;
		float num5 = (float)(e1.y0 - e0.y0) / num;
		float num6 = (float)this.cellSize / num;
		float num7 = 0f;
		float num8 = (float)this.cellSize / num2;
		int i = e1.y0;
		int num9 = this.cellSize;
		int num10 = (i - this.origin.y) % this.cellSize;
		if (num10 != 0)
		{
			num9 = this.cellSize - num10;
		}
		while (i <= e1.y1)
		{
			this.tempVar[0] = num3 * num5 + (float)e0.x0;
			this.tempVar[1] = num4 * num7 + (float)e1.x0;
			float num11 = 1f;
			if (i + num9 <= e1.y1)
			{
				if (num9 != this.cellSize)
				{
					num11 = (float)num9 / (float)this.cellSize;
				}
			}
			else
			{
				num11 = (float)(e1.y1 - i) / (float)this.cellSize;
			}
			num5 += num6 * num11;
			num7 += num8 * num11;
			this.tempVar[2] = num3 * num5 + (float)e0.x0;
			this.tempVar[3] = num4 * num7 + (float)e1.x0;
			int num12 = Mathf.FloorToInt(Mathf.Min(this.tempVar));
			int num13 = Mathf.CeilToInt(Mathf.Max(this.tempVar));
			int y = (i - this.origin.y) / this.cellSize;
			int x = (num12 - this.origin.x) / this.cellSize;
			int x2 = (num13 - this.origin.x) / this.cellSize;
			this.AddLine(x, x2, y, data);
			i += num9;
			num9 = this.cellSize;
		}
	}

	private void AddLine(int x0, int x1, int y, object data)
	{
		int num = y * this.numCellsX;
		for (int i = x0; i <= x1; i++)
		{
			int num2 = num + i;
			if (num2 < 0 || num2 >= this.cells.Length)
			{
				DebugHelper.Assert(num2 >= 0 && num2 < this.cells.Length, "index of rasterizer cells is out of range !");
			}
			List<object> list = this.cells[num2];
			if (list == null)
			{
				list = new List<object>();
				this.cells[num2] = list;
				list.Add(data);
			}
			else if (!list.Contains(data))
			{
				list.Add(data);
			}
		}
	}

	public List<object> GetLocated(VInt3 pos)
	{
		int num = pos.x - this.origin.x;
		int num2 = pos.z - this.origin.y;
		num /= this.cellSize;
		num2 /= this.cellSize;
		if (num < 0 || num2 < 0 || num >= this.numCellsX || num2 >= this.numCellsY)
		{
			return null;
		}
		return this.cells[num2 * this.numCellsX + num];
	}

	public void GetCellPosClamped(out int x, out int y, VInt3 pos)
	{
		x = (pos.x - this.origin.x) / this.cellSize;
		y = (pos.z - this.origin.y) / this.cellSize;
		x = Mathf.Clamp(x, 0, this.numCellsX - 1);
		y = Mathf.Clamp(y, 0, this.numCellsY - 1);
	}

	public bool IntersectionSegment(int x, int y, VInt3 start, VInt3 end)
	{
		if (x < 0 || y < 0 || x >= this.numCellsX || y >= this.numCellsY)
		{
			return false;
		}
		bool flag = false;
		VInt3 start2;
		start2.x = this.origin.x + x * this.cellSize;
		start2.y = 0;
		start2.z = this.origin.y + y * this.cellSize;
		VInt3 vInt;
		vInt.x = this.origin.x + (x + 1) * this.cellSize;
		vInt.y = 0;
		vInt.z = this.origin.y + y * this.cellSize;
		VInt3 vInt2;
		vInt2.x = this.origin.x + x * this.cellSize;
		vInt2.y = 0;
		vInt2.z = this.origin.y + (y + 1) * this.cellSize;
		VInt3 end2;
		end2.x = this.origin.x + (x + 1) * this.cellSize;
		end2.y = 0;
		end2.z = this.origin.y + (y + 1) * this.cellSize;
		Polygon.SegmentIntersectionPoint(start, end, start2, vInt, out flag);
		if (flag)
		{
			return flag;
		}
		Polygon.SegmentIntersectionPoint(start, end, start2, vInt2, out flag);
		if (flag)
		{
			return flag;
		}
		Polygon.SegmentIntersectionPoint(start, end, vInt, end2, out flag);
		if (flag)
		{
			return flag;
		}
		Polygon.SegmentIntersectionPoint(start, end, vInt2, end2, out flag);
		return flag && flag;
	}

	public List<object> GetObjs(int x, int y)
	{
		int num = y * this.numCellsX + x;
		return this.cells[num];
	}

	private void drawSelected()
	{
		if (this.cells == null || this.DebugNodeID == -1)
		{
			return;
		}
		Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
		float num = (float)this.cellSize * 0.001f;
		Vector3 vector = new Vector3(num, 1f, num);
		for (int i = 0; i < this.numCellsY; i++)
		{
			for (int j = 0; j < this.numCellsX; j++)
			{
				int num2 = i * this.numCellsX + j;
				if (this.cells[num2] != null)
				{
					object obj = this.cells[num2].Find(delegate(object o)
					{
						TriangleMeshNode triangleMeshNode = o as TriangleMeshNode;
						return triangleMeshNode.NodeIndex == this.DebugNodeID;
					});
					if (obj != null)
					{
						float x = (float)(j * this.cellSize + this.origin.x + this.cellSize / 2) * 0.001f;
						float z = (float)(i * this.cellSize + this.origin.y + this.cellSize / 2) * 0.001f;
						Gizmos.color = this.debugColors[i % 2 * 2 + j % 2];
						Gizmos.DrawCube(new Vector3(x, 0f, z), vector);
					}
				}
			}
		}
	}

	private void drawAll()
	{
		if (this.cells == null)
		{
			return;
		}
		Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
		float num = (float)this.cellSize * 0.001f;
		Vector3 vector = new Vector3(num, 1f, num);
		for (int i = 0; i < this.numCellsY; i++)
		{
			for (int j = 0; j < this.numCellsX; j++)
			{
				int num2 = i * this.numCellsX + j;
				if (this.cells[num2] != null)
				{
					float x = (float)(j * this.cellSize + this.origin.x + this.cellSize / 2) * 0.001f;
					float z = (float)(i * this.cellSize + this.origin.y + this.cellSize / 2) * 0.001f;
					Gizmos.color = this.debugColors[i % 2 * 2 + j % 2];
					Gizmos.DrawCube(new Vector3(x, 0f, z), vector);
				}
			}
		}
	}

	public void DrawGizmos()
	{
	}
}
