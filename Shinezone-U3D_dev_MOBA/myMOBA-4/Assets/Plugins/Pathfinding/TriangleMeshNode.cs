using Pathfinding.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
	public class TriangleMeshNode : MeshNode
	{
		public int v0;

		public int v1;

		public int v2;

		private static VInt3[] _staticVerts = new VInt3[3];

		protected static ListView<INavmeshHolder[]> _navmeshHolders = new ListView<INavmeshHolder[]>();

		protected TriangleMeshNode()
		{
		}

		public TriangleMeshNode(AstarPath astar) : base(astar)
		{
		}

		protected override void Duplicate(GraphNode graphNode)
		{
			base.Duplicate(graphNode);
			TriangleMeshNode triangleMeshNode = (TriangleMeshNode)graphNode;
			triangleMeshNode.v0 = this.v0;
			triangleMeshNode.v1 = this.v1;
			triangleMeshNode.v2 = this.v2;
		}

		public TriangleMeshNode Clone()
		{
			TriangleMeshNode triangleMeshNode = new TriangleMeshNode();
			this.Duplicate(triangleMeshNode);
			return triangleMeshNode;
		}

		public void GetPoints(out VInt3 a, out VInt3 b, out VInt3 c)
		{
			INavmeshHolder navmeshHolder = TriangleMeshNode.GetNavmeshHolder(this.DataGroupIndex, base.GraphIndex);
			a = navmeshHolder.GetVertex(this.v0);
			b = navmeshHolder.GetVertex(this.v1);
			c = navmeshHolder.GetVertex(this.v2);
		}

		public void GetPoints(out Vector3 a, out Vector3 b, out Vector3 c)
		{
			INavmeshHolder navmeshHolder = TriangleMeshNode.GetNavmeshHolder(this.DataGroupIndex, base.GraphIndex);
			a = (Vector3)navmeshHolder.GetVertex(this.v0);
			b = (Vector3)navmeshHolder.GetVertex(this.v1);
			c = (Vector3)navmeshHolder.GetVertex(this.v2);
		}

		public bool IsVertex(VInt3 p, out int index)
		{
			INavmeshHolder navmeshHolder = TriangleMeshNode.GetNavmeshHolder(this.DataGroupIndex, base.GraphIndex);
			index = -1;
			if (navmeshHolder.GetVertex(this.v0).IsEqualXZ(ref p))
			{
				index = 0;
			}
			else if (navmeshHolder.GetVertex(this.v1).IsEqualXZ(ref p))
			{
				index = 1;
			}
			else if (navmeshHolder.GetVertex(this.v2).IsEqualXZ(ref p))
			{
				index = 2;
			}
			return index != -1;
		}

		public int EdgeIntersect(VInt3 a, VInt3 b)
		{
			VInt3 vInt;
			VInt3 vInt2;
			VInt3 vInt3;
			this.GetPoints(out vInt, out vInt2, out vInt3);
			if (Polygon.Intersects(vInt, vInt2, a, b))
			{
				return 0;
			}
			if (Polygon.Intersects(vInt2, vInt3, a, b))
			{
				return 1;
			}
			if (Polygon.Intersects(vInt3, vInt, a, b))
			{
				return 2;
			}
			return -1;
		}

		public int EdgeIntersect(VInt3 a, VInt3 b, int startEdge, int count)
		{
			VInt3[] staticVerts = TriangleMeshNode._staticVerts;
			this.GetPoints(out staticVerts[0], out staticVerts[1], out staticVerts[2]);
			for (int i = 0; i < count; i++)
			{
				int num = (startEdge + i) % 3;
				int num2 = (num + 1) % 3;
				if (Polygon.Intersects(staticVerts[num], staticVerts[num2], a, b))
				{
					return num;
				}
			}
			return -1;
		}

		public int GetColinearEdge(VInt3 a, VInt3 b)
		{
			VInt3 vInt;
			VInt3 vInt2;
			VInt3 vInt3;
			this.GetPoints(out vInt, out vInt2, out vInt3);
			if (Polygon.IsColinear(vInt, vInt2, a) && Polygon.IsColinear(vInt, vInt2, b))
			{
				return 0;
			}
			if (Polygon.IsColinear(vInt2, vInt3, a) && Polygon.IsColinear(vInt2, vInt3, b))
			{
				return 1;
			}
			if (Polygon.IsColinear(vInt3, vInt, a) && Polygon.IsColinear(vInt3, vInt, b))
			{
				return 2;
			}
			return -1;
		}

		public int GetColinearEdge(VInt3 a, VInt3 b, int startEdge, int count)
		{
			VInt3[] staticVerts = TriangleMeshNode._staticVerts;
			this.GetPoints(out staticVerts[0], out staticVerts[1], out staticVerts[2]);
			for (int i = 0; i < count; i++)
			{
				int num = (startEdge + i) % 3;
				int num2 = (num + 1) % 3;
				if (Polygon.IsColinear(staticVerts[num], staticVerts[num2], a) && Polygon.IsColinear(staticVerts[num], staticVerts[num2], b))
				{
					return num;
				}
			}
			return -1;
		}

		public TriangleMeshNode GetNeighborByEdge(int edge, out int otherEdge)
		{
			otherEdge = -1;
			if (edge < 0 || edge > 2 || this.connections == null)
			{
				return null;
			}
			int vertexIndex = this.GetVertexIndex(edge % 3);
			int vertexIndex2 = this.GetVertexIndex((edge + 1) % 3);
			TriangleMeshNode result = null;
			for (int i = 0; i < this.connections.Length; i++)
			{
				TriangleMeshNode triangleMeshNode = this.connections[i] as TriangleMeshNode;
				if (triangleMeshNode != null && triangleMeshNode.GraphIndex == base.GraphIndex)
				{
					if (triangleMeshNode.v1 == vertexIndex && triangleMeshNode.v0 == vertexIndex2)
					{
						otherEdge = 0;
					}
					else if (triangleMeshNode.v2 == vertexIndex && triangleMeshNode.v1 == vertexIndex2)
					{
						otherEdge = 1;
					}
					else if (triangleMeshNode.v0 == vertexIndex && triangleMeshNode.v2 == vertexIndex2)
					{
						otherEdge = 2;
					}
					if (otherEdge != -1)
					{
						result = triangleMeshNode;
						break;
					}
				}
			}
			return result;
		}

		public static INavmeshHolder GetNavmeshHolder(int dataGroupIndex, uint graphIndex)
		{
			return TriangleMeshNode._navmeshHolders[dataGroupIndex][(int)graphIndex];
		}

		public static void SetNavmeshHolder(int dataGroupIndex, int graphIndex, INavmeshHolder graph)
		{
			if (dataGroupIndex >= TriangleMeshNode._navmeshHolders.Count)
			{
				for (int i = TriangleMeshNode._navmeshHolders.Count; i <= dataGroupIndex; i++)
				{
					TriangleMeshNode._navmeshHolders.Add(new INavmeshHolder[0]);
				}
			}
			if (TriangleMeshNode._navmeshHolders[dataGroupIndex].Length <= graphIndex)
			{
				INavmeshHolder[] array = new INavmeshHolder[graphIndex + 1];
				INavmeshHolder[] array2 = TriangleMeshNode._navmeshHolders[dataGroupIndex];
				for (int j = 0; j < array2.Length; j++)
				{
					array[j] = array2[j];
				}
				TriangleMeshNode._navmeshHolders[dataGroupIndex] = array;
			}
			TriangleMeshNode._navmeshHolders[dataGroupIndex][graphIndex] = graph;
		}

		public void UpdatePositionFromVertices()
		{
			INavmeshHolder navmeshHolder = TriangleMeshNode.GetNavmeshHolder(this.DataGroupIndex, base.GraphIndex);
			this.position = (navmeshHolder.GetVertex(this.v0) + navmeshHolder.GetVertex(this.v1) + navmeshHolder.GetVertex(this.v2)) * 0.333333f;
		}

		public int GetVertexIndex(int i)
		{
			return (i != 0) ? ((i != 1) ? this.v2 : this.v1) : this.v0;
		}

		public int GetVertexArrayIndex(int i)
		{
			return TriangleMeshNode.GetNavmeshHolder(this.DataGroupIndex, base.GraphIndex).GetVertexArrayIndex((i != 0) ? ((i != 1) ? this.v2 : this.v1) : this.v0);
		}

		public override VInt3 GetVertex(int i)
		{
			VInt3 result = VInt3.zero;
			try
			{
				INavmeshHolder navmeshHolder = TriangleMeshNode.GetNavmeshHolder(this.DataGroupIndex, base.GraphIndex);
				result = navmeshHolder.GetVertex(this.GetVertexIndex(i));
			}
			catch (Exception)
			{
			}
			return result;
		}

		public override int GetVertexCount()
		{
			return 3;
		}

		public override Vector3 ClosestPointOnNode(Vector3 p)
		{
			INavmeshHolder navmeshHolder = TriangleMeshNode.GetNavmeshHolder(this.DataGroupIndex, base.GraphIndex);
			return Polygon.ClosestPointOnTriangle((Vector3)navmeshHolder.GetVertex(this.v0), (Vector3)navmeshHolder.GetVertex(this.v1), (Vector3)navmeshHolder.GetVertex(this.v2), p);
		}

		public override Vector3 ClosestPointOnNodeXZ(Vector3 _p)
		{
			INavmeshHolder navmeshHolder = TriangleMeshNode.GetNavmeshHolder(this.DataGroupIndex, base.GraphIndex);
			VInt3 vertex = navmeshHolder.GetVertex(this.v0);
			VInt3 vertex2 = navmeshHolder.GetVertex(this.v1);
			VInt3 vertex3 = navmeshHolder.GetVertex(this.v2);
			VInt3 point = (VInt3)_p;
			int y = point.y;
			vertex.y = 0;
			vertex2.y = 0;
			vertex3.y = 0;
			point.y = 0;
			if ((long)(vertex2.x - vertex.x) * (long)(point.z - vertex.z) - (long)(point.x - vertex.x) * (long)(vertex2.z - vertex.z) > 0L)
			{
				float num = Mathf.Clamp01(AstarMath.NearestPointFactorXZ(vertex, vertex2, point));
				return new Vector3((float)vertex.x + (float)(vertex2.x - vertex.x) * num, (float)y, (float)vertex.z + (float)(vertex2.z - vertex.z) * num) * 0.001f;
			}
			if ((long)(vertex3.x - vertex2.x) * (long)(point.z - vertex2.z) - (long)(point.x - vertex2.x) * (long)(vertex3.z - vertex2.z) > 0L)
			{
				float num2 = Mathf.Clamp01(AstarMath.NearestPointFactorXZ(vertex2, vertex3, point));
				return new Vector3((float)vertex2.x + (float)(vertex3.x - vertex2.x) * num2, (float)y, (float)vertex2.z + (float)(vertex3.z - vertex2.z) * num2) * 0.001f;
			}
			if ((long)(vertex.x - vertex3.x) * (long)(point.z - vertex3.z) - (long)(point.x - vertex3.x) * (long)(vertex.z - vertex3.z) > 0L)
			{
				float num3 = Mathf.Clamp01(AstarMath.NearestPointFactorXZ(vertex3, vertex, point));
				return new Vector3((float)vertex3.x + (float)(vertex.x - vertex3.x) * num3, (float)y, (float)vertex3.z + (float)(vertex.z - vertex3.z) * num3) * 0.001f;
			}
			return _p;
		}

		private void CalcNearestPoint(out VInt3 cp, ref VInt3 start, ref VInt3 end, ref VInt3 p)
		{
			VInt2 vInt = new VInt2(end.x - start.x, end.z - start.z);
			long sqrMagnitudeLong = vInt.sqrMagnitudeLong;
			VInt2 vInt2 = new VInt2(p.x - start.x, p.z - start.z);
			cp = default(VInt3);
			cp.y = p.y;
			long num = VInt2.DotLong(ref vInt2, ref vInt);
			if (sqrMagnitudeLong != 0L)
			{
				long a = (long)(end.x - start.x) * num;
				long a2 = (long)(end.z - start.z) * num;
				cp.x = (int)IntMath.Divide(a, sqrMagnitudeLong);
				cp.z = (int)IntMath.Divide(a2, sqrMagnitudeLong);
				cp.x += start.x;
				cp.z += start.z;
			}
			else
			{
				int num2 = (int)num;
				cp.x = start.x + (end.x - start.x) * num2;
				cp.z = start.z + (end.z - start.z) * num2;
			}
		}

		public override VInt3 ClosestPointOnNodeXZ(VInt3 p)
		{
			INavmeshHolder navmeshHolder = TriangleMeshNode.GetNavmeshHolder(this.DataGroupIndex, base.GraphIndex);
			VInt3 vertex = navmeshHolder.GetVertex(this.v0);
			VInt3 vertex2 = navmeshHolder.GetVertex(this.v1);
			VInt3 vertex3 = navmeshHolder.GetVertex(this.v2);
			vertex.y = 0;
			vertex2.y = 0;
			vertex3.y = 0;
			VInt3 result;
			if ((long)(vertex2.x - vertex.x) * (long)(p.z - vertex.z) - (long)(p.x - vertex.x) * (long)(vertex2.z - vertex.z) > 0L)
			{
				this.CalcNearestPoint(out result, ref vertex, ref vertex2, ref p);
			}
			else if ((long)(vertex3.x - vertex2.x) * (long)(p.z - vertex2.z) - (long)(p.x - vertex2.x) * (long)(vertex3.z - vertex2.z) > 0L)
			{
				this.CalcNearestPoint(out result, ref vertex2, ref vertex3, ref p);
			}
			else if ((long)(vertex.x - vertex3.x) * (long)(p.z - vertex3.z) - (long)(p.x - vertex3.x) * (long)(vertex.z - vertex3.z) > 0L)
			{
				this.CalcNearestPoint(out result, ref vertex3, ref vertex, ref p);
			}
			else
			{
				result = p;
			}
			return result;
		}

		public override bool ContainsPoint(VInt3 p)
		{
			INavmeshHolder navmeshHolder = TriangleMeshNode.GetNavmeshHolder(this.DataGroupIndex, base.GraphIndex);
			VInt3 vertex = navmeshHolder.GetVertex(this.v0);
			VInt3 vertex2 = navmeshHolder.GetVertex(this.v1);
			VInt3 vertex3 = navmeshHolder.GetVertex(this.v2);
			return (long)(vertex2.x - vertex.x) * (long)(p.z - vertex.z) - (long)(p.x - vertex.x) * (long)(vertex2.z - vertex.z) <= 0L && (long)(vertex3.x - vertex2.x) * (long)(p.z - vertex2.z) - (long)(p.x - vertex2.x) * (long)(vertex3.z - vertex2.z) <= 0L && (long)(vertex.x - vertex3.x) * (long)(p.z - vertex3.z) - (long)(p.x - vertex3.x) * (long)(vertex.z - vertex3.z) <= 0L;
		}

		public override void UpdateRecursiveG(Path path, PathNode pathNode, PathHandler handler)
		{
			base.UpdateG(path, pathNode);
			handler.PushNode(pathNode);
			if (this.connections == null)
			{
				return;
			}
			for (int i = 0; i < this.connections.Length; i++)
			{
				GraphNode graphNode = this.connections[i];
				PathNode pathNode2 = handler.GetPathNode(graphNode);
				if (pathNode2.parent == pathNode && pathNode2.pathID == handler.PathID)
				{
					graphNode.UpdateRecursiveG(path, pathNode2, handler);
				}
			}
		}

		public override void Open(Path path, PathNode pathNode, PathHandler handler)
		{
			if (this.connections == null)
			{
				return;
			}
			bool flag = pathNode.flag2;
			for (int i = this.connections.Length - 1; i >= 0; i--)
			{
				GraphNode graphNode = this.connections[i];
				if (path.CanTraverse(graphNode))
				{
					PathNode pathNode2 = handler.GetPathNode(graphNode);
					if (pathNode2 != pathNode.parent)
					{
						uint num = this.connectionCosts[i];
						if (flag || pathNode2.flag2)
						{
							num = path.GetConnectionSpecialCost(this, graphNode, num);
						}
						if (pathNode2.pathID != handler.PathID)
						{
							pathNode2.node = graphNode;
							pathNode2.parent = pathNode;
							pathNode2.pathID = handler.PathID;
							pathNode2.cost = num;
							pathNode2.H = path.CalculateHScore(graphNode);
							graphNode.UpdateG(path, pathNode2);
							handler.PushNode(pathNode2);
						}
						else if (pathNode.G + num + path.GetTraversalCost(graphNode) < pathNode2.G)
						{
							pathNode2.cost = num;
							pathNode2.parent = pathNode;
							graphNode.UpdateRecursiveG(path, pathNode2, handler);
						}
						else if (pathNode2.G + num + path.GetTraversalCost(this) < pathNode.G && graphNode.ContainsConnection(this))
						{
							pathNode.parent = pathNode2;
							pathNode.cost = num;
							this.UpdateRecursiveG(path, pathNode, handler);
						}
					}
				}
			}
		}

		public int SharedEdge(GraphNode other)
		{
			int result;
			int num;
			this.GetPortal(other, null, null, false, out result, out num);
			return result;
		}

		public override bool GetPortal(GraphNode _other, List<VInt3> left, List<VInt3> right, bool backwards)
		{
			int num;
			int num2;
			return this.GetPortal(_other, left, right, backwards, out num, out num2);
		}

		public bool GetPortal(GraphNode _other, List<VInt3> left, List<VInt3> right, bool backwards, out int aIndex, out int bIndex)
		{
			aIndex = -1;
			bIndex = -1;
			if (_other.GraphIndex != base.GraphIndex)
			{
				return false;
			}
			TriangleMeshNode triangleMeshNode = _other as TriangleMeshNode;
			int num = this.GetVertexIndex(0) >> 12 & 524287;
			int num2 = triangleMeshNode.GetVertexIndex(0) >> 12 & 524287;
			if (num != num2 && TriangleMeshNode.GetNavmeshHolder(this.DataGroupIndex, base.GraphIndex) is RecastGraph)
			{
				INavmeshHolder navmeshHolder = TriangleMeshNode.GetNavmeshHolder(this.DataGroupIndex, base.GraphIndex);
				int num3;
				int num4;
				navmeshHolder.GetTileCoordinates(num, out num3, out num4);
				int num5;
				int num6;
				navmeshHolder.GetTileCoordinates(num2, out num5, out num6);
				int num7;
				if (Math.Abs(num3 - num5) == 1)
				{
					num7 = 0;
				}
				else
				{
					if (Math.Abs(num4 - num6) != 1)
					{
						throw new Exception(string.Concat(new object[]
						{
							"Tiles not adjacent (",
							num3,
							", ",
							num4,
							") (",
							num5,
							", ",
							num6,
							")"
						}));
					}
					num7 = 2;
				}
				int vertexCount = this.GetVertexCount();
				int vertexCount2 = triangleMeshNode.GetVertexCount();
				int num8 = -1;
				int num9 = -1;
				for (int i = 0; i < vertexCount; i++)
				{
					int num10 = this.GetVertex(i)[num7];
					for (int j = 0; j < vertexCount2; j++)
					{
						if (num10 == triangleMeshNode.GetVertex((j + 1) % vertexCount2)[num7] && this.GetVertex((i + 1) % vertexCount)[num7] == triangleMeshNode.GetVertex(j)[num7])
						{
							num8 = i;
							num9 = j;
							i = vertexCount;
							break;
						}
					}
				}
				aIndex = num8;
				bIndex = num9;
				if (num8 != -1)
				{
					VInt3 vertex = this.GetVertex(num8);
					VInt3 vertex2 = this.GetVertex((num8 + 1) % vertexCount);
					int i2 = (num7 != 2) ? 2 : 0;
					int num11 = Math.Min(vertex[i2], vertex2[i2]);
					int num12 = Math.Max(vertex[i2], vertex2[i2]);
					num11 = Math.Max(num11, Math.Min(triangleMeshNode.GetVertex(num9)[i2], triangleMeshNode.GetVertex((num9 + 1) % vertexCount2)[i2]));
					num12 = Math.Min(num12, Math.Max(triangleMeshNode.GetVertex(num9)[i2], triangleMeshNode.GetVertex((num9 + 1) % vertexCount2)[i2]));
					if (vertex[i2] < vertex2[i2])
					{
						vertex[i2] = num11;
						vertex2[i2] = num12;
					}
					else
					{
						vertex[i2] = num12;
						vertex2[i2] = num11;
					}
					if (left != null)
					{
						left.Add(vertex);
						right.Add(vertex2);
					}
					return true;
				}
			}
			else if (!backwards)
			{
				int num13 = -1;
				int num14 = -1;
				int vertexCount3 = this.GetVertexCount();
				int vertexCount4 = triangleMeshNode.GetVertexCount();
				for (int k = 0; k < vertexCount3; k++)
				{
					int vertexIndex = this.GetVertexIndex(k);
					for (int l = 0; l < vertexCount4; l++)
					{
						if (vertexIndex == triangleMeshNode.GetVertexIndex((l + 1) % vertexCount4) && this.GetVertexIndex((k + 1) % vertexCount3) == triangleMeshNode.GetVertexIndex(l))
						{
							num13 = k;
							num14 = l;
							k = vertexCount3;
							break;
						}
					}
				}
				aIndex = num13;
				bIndex = num14;
				if (num13 == -1)
				{
					return false;
				}
				if (left != null)
				{
					left.Add(this.GetVertex(num13));
					right.Add(this.GetVertex((num13 + 1) % vertexCount3));
				}
			}
			return true;
		}

		public override void SerializeNode(GraphSerializationContext ctx)
		{
			base.SerializeNode(ctx);
			ctx.writer.Write(this.v0);
			ctx.writer.Write(this.v1);
			ctx.writer.Write(this.v2);
		}

		public override void DeserializeNode(GraphSerializationContext ctx)
		{
			base.DeserializeNode(ctx);
			this.v0 = ctx.reader.ReadInt32();
			this.v1 = ctx.reader.ReadInt32();
			this.v2 = ctx.reader.ReadInt32();
		}
	}
}
