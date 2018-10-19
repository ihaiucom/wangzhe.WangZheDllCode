using Pathfinding.Serialization;
using Pathfinding.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Pathfinding
{
	[Serializable]
	public class AstarData
	{
		[NonSerialized]
		public NavMeshGraph navmesh;

		[NonSerialized]
		public RecastGraph recastGraph;

		public Type[] graphTypes;

		[NonSerialized]
		public NavGraph[] graphs = new NavGraph[0];

		[NonSerialized]
		public UserConnection[] userConnections = new UserConnection[0];

		public bool hasBeenReverted;

		[SerializeField]
		private byte[] data;

		public uint dataChecksum;

		public byte[] data_backup;

		public byte[] data_cachedStartup;

		public byte[] revertData;

		[SerializeField]
		public bool cacheStartup;

		public int DataGroupIndex;

		public GraphNodeRasterizer rasterizer;

		public AstarPath active
		{
			get
			{
				return AstarPath.active;
			}
		}

		public void RasterizeGraphNodes()
		{
			this.RasterizeGraphNodesInternal();
		}

		public GraphNodeRasterizer InitRasterizer(int inCellSize, bool bUseForceBoundsSize = false)
		{
			if (this.graphs == null || this.graphs.Length == 0)
			{
				return null;
			}
			GraphNodeRasterizer r = new GraphNodeRasterizer();
			VInt2 min = new VInt2(2147483647, 2147483647);
			VInt2 max = new VInt2(-2147483648, -2147483648);
			if (bUseForceBoundsSize)
			{
				for (int i = 0; i < this.graphs.Length; i++)
				{
					RecastGraph recastGraph = this.graphs[i] as RecastGraph;
					if (recastGraph == null)
					{
						return null;
					}
					VInt2 vInt = new VInt2((int)(recastGraph.forcedBounds.min.x * 1000f), (int)(recastGraph.forcedBounds.min.z * 1000f));
					VInt2 vInt2 = new VInt2((int)(recastGraph.forcedBounds.max.x * 1000f), (int)(recastGraph.forcedBounds.max.z * 1000f));
					min.Min(ref vInt);
					max.Max(ref vInt2);
				}
				r.Init(min, max.x - min.x, max.y - min.y, inCellSize);
			}
			else
			{
				for (int j = 0; j < this.graphs.Length; j++)
				{
					RecastGraph recastGraph2 = this.graphs[j] as RecastGraph;
					if (recastGraph2 == null)
					{
						return null;
					}
					recastGraph2.GetNodes(delegate(GraphNode node)
					{
						TriangleMeshNode triangleMeshNode = node as TriangleMeshNode;
						if (triangleMeshNode != null)
						{
							VInt2 xz = triangleMeshNode.GetVertex(0).xz;
							VInt2 xz2 = triangleMeshNode.GetVertex(1).xz;
							VInt2 xz3 = triangleMeshNode.GetVertex(2).xz;
							min.Min(ref xz);
							min.Min(ref xz2);
							min.Min(ref xz3);
							max.Max(ref xz);
							max.Max(ref xz2);
							max.Max(ref xz3);
						}
						return true;
					});
				}
				r.Init(min, max.x - min.x, max.y - min.y, inCellSize);
			}
			for (int k = 0; k < this.graphs.Length; k++)
			{
				RecastGraph recastGraph3 = this.graphs[k] as RecastGraph;
				if (recastGraph3 != null)
				{
					recastGraph3.GetNodes(delegate(GraphNode node)
					{
						TriangleMeshNode triangleMeshNode = node as TriangleMeshNode;
						if (triangleMeshNode != null)
						{
							VInt2 xz = triangleMeshNode.GetVertex(0).xz;
							VInt2 xz2 = triangleMeshNode.GetVertex(1).xz;
							VInt2 xz3 = triangleMeshNode.GetVertex(2).xz;
							r.AddTriangle(ref xz, ref xz2, ref xz3, triangleMeshNode);
						}
						return true;
					});
				}
			}
			return r;
		}

        /** All graphs which implements the UpdateableGraph interface
 * \code foreach (IUpdatableGraph graph in AstarPath.data.GetUpdateableGraphs ()) {
 *  //Do something with the graph
 * } \endcode
 * \see AstarPath.RegisterSafeNodeUpdate
 * \see Pathfinding.IUpdatableGraph */
        public IEnumerable GetUpdateableGraphs()
        {
            if (graphs == null) yield break;
            for (int i = 0; i < graphs.Length; i++)
            {
                if (graphs[i] is IUpdatableGraph)
                {
                    yield return graphs[i];
                }
            }
        }

		private void RasterizeGraphNodesInternal()
		{
			this.rasterizer = this.InitRasterizer(4000, false);
		}

		public TriangleMeshNode GetLocatedByRasterizer(VInt3 position)
		{
			TriangleMeshNode result = null;
			if (this.rasterizer != null)
			{
				List<object> located = this.rasterizer.GetLocated(position);
				if (located != null)
				{
					for (int i = 0; i < located.Count; i++)
					{
						TriangleMeshNode triangleMeshNode = located[i] as TriangleMeshNode;
						if (triangleMeshNode == null)
						{
							break;
						}
						VInt3 a;
						VInt3 b;
						VInt3 c;
						triangleMeshNode.GetPoints(out a, out b, out c);
						if (Polygon.ContainsPoint(a, b, c, position))
						{
							result = triangleMeshNode;
							break;
						}
					}
				}
			}
			return result;
		}

		public TriangleMeshNode GetNearestByRasterizer(VInt3 position, out VInt3 clampedPosition)
		{
			clampedPosition = VInt3.zero;
			if (this.rasterizer == null)
			{
				return null;
			}
			TriangleMeshNode triangleMeshNode = this.GetLocatedByRasterizer(position);
			if (triangleMeshNode != null)
			{
				clampedPosition = position;
				return triangleMeshNode;
			}
			triangleMeshNode = this.FindNearestByRasterizer(position, -1);
			if (triangleMeshNode == null)
			{
				return null;
			}
			clampedPosition = triangleMeshNode.ClosestPointOnNodeXZ(position);
			return triangleMeshNode;
		}

		private bool getNearest(VInt3 position, List<object> objs, ref long minDist, ref TriangleMeshNode nearestNode)
		{
			if (objs == null || objs.Count == 0)
			{
				return false;
			}
			bool result = false;
			for (int i = 0; i < objs.Count; i++)
			{
				TriangleMeshNode triangleMeshNode = objs[i] as TriangleMeshNode;
				if (triangleMeshNode == null)
				{
					return false;
				}
				long num = position.XZSqrMagnitude(triangleMeshNode.position);
				if (num < minDist)
				{
					minDist = num;
					nearestNode = triangleMeshNode;
					result = true;
				}
			}
			return result;
		}

		public TriangleMeshNode FindNearestByRasterizer(VInt3 position, int maxRange = -1)
		{
			if (this.rasterizer == null)
			{
				return null;
			}
			int num;
			int num2;
			this.rasterizer.GetCellPosClamped(out num, out num2, position);
			long num3 = 9223372036854775807L;
			TriangleMeshNode triangleMeshNode = null;
			List<object> objs = this.rasterizer.GetObjs(num, num2);
			if (this.getNearest(position, objs, ref num3, ref triangleMeshNode))
			{
				return triangleMeshNode;
			}
			int i = 1;
			int num4 = Mathf.Max(this.rasterizer.numCellsX, this.rasterizer.numCellsY);
			if (maxRange == -1)
			{
				maxRange = num4;
			}
			else
			{
				maxRange = Mathf.Clamp(maxRange, 1, num4);
			}
			while (i < maxRange)
			{
				int num5 = Mathf.Max(num - i, 0);
				int num6 = Mathf.Min(num + i, this.rasterizer.numCellsX - 1);
				int num7 = Mathf.Max(num2 - i, 0);
				int num8 = Mathf.Min(num2 + i, this.rasterizer.numCellsY - 1);
				if (num - i == num5)
				{
					for (int j = num7; j <= num8; j++)
					{
						this.getNearest(position, this.rasterizer.GetObjs(num5, j), ref num3, ref triangleMeshNode);
					}
				}
				if (num + i == num6)
				{
					for (int k = num7; k <= num8; k++)
					{
						this.getNearest(position, this.rasterizer.GetObjs(num6, k), ref num3, ref triangleMeshNode);
					}
				}
				if (num2 - i == num7)
				{
					for (int l = num5 + 1; l < num6; l++)
					{
						this.getNearest(position, this.rasterizer.GetObjs(l, num7), ref num3, ref triangleMeshNode);
					}
				}
				if (num2 + i == num8)
				{
					for (int m = num5 + 1; m < num6; m++)
					{
						this.getNearest(position, this.rasterizer.GetObjs(m, num8), ref num3, ref triangleMeshNode);
					}
				}
				if (triangleMeshNode != null)
				{
					break;
				}
				i++;
			}
			return triangleMeshNode;
		}

		public bool CheckSegmentIntersects(VInt3 start, VInt3 end, int gridX, int gridY, out VInt3 outPoint, out TriangleMeshNode nearestNode)
		{
			List<object> objs = this.rasterizer.GetObjs(gridX, gridY);
			outPoint = end;
			nearestNode = null;
			if (objs == null || objs.Count == 0)
			{
				return false;
			}
			VInt3[] array = new VInt3[3];
			bool result = false;
			long num = 9223372036854775807L;
			for (int i = 0; i < objs.Count; i++)
			{
				TriangleMeshNode triangleMeshNode = objs[i] as TriangleMeshNode;
				triangleMeshNode.GetPoints(out array[0], out array[1], out array[2]);
				for (int j = 0; j < 3; j++)
				{
					int num2 = j;
					int num3 = (j + 1) % 3;
					bool flag = false;
					VInt3 vInt = Polygon.SegmentIntersectionPoint(array[num2], array[num3], start, end, out flag);
					if (flag)
					{
						long num4 = start.XZSqrMagnitude(ref vInt);
						if (num4 < num)
						{
							nearestNode = triangleMeshNode;
							num = num4;
							outPoint = vInt;
							result = true;
						}
					}
				}
			}
			return result;
		}

		private TriangleMeshNode checkObjIntersects(ref int edge, VInt3 start, VInt3 end, int gridX, int gridY)
		{
			List<object> objs = this.rasterizer.GetObjs(gridX, gridY);
			if (objs == null || objs.Count == 0)
			{
				return null;
			}
			VInt3[] array = new VInt3[3];
			TriangleMeshNode triangleMeshNode = null;
			int num = -1;
			long num2 = 9223372036854775807L;
			for (int i = 0; i < objs.Count; i++)
			{
				TriangleMeshNode triangleMeshNode2 = objs[i] as TriangleMeshNode;
				triangleMeshNode2.GetPoints(out array[0], out array[1], out array[2]);
				for (int j = 0; j < 3; j++)
				{
					int num3 = j;
					int num4 = (j + 1) % 3;
					if (Polygon.Intersects(array[num3], array[num4], start, end))
					{
						bool flag;
						VInt3 vInt = Polygon.IntersectionPoint(ref array[num3], ref array[num4], ref start, ref end, out flag);
						DebugHelper.Assert(flag);
						long num5 = start.XZSqrMagnitude(ref vInt);
						if (num5 < num2)
						{
							num2 = num5;
							triangleMeshNode = triangleMeshNode2;
							num = j;
						}
					}
				}
			}
			if (num != -1 && triangleMeshNode != null)
			{
				edge = num;
				return triangleMeshNode;
			}
			return null;
		}

		public TriangleMeshNode IntersectByRasterizer(VInt3 start, VInt3 end, out int edge)
		{
			edge = -1;
			if (this.rasterizer == null)
			{
				return null;
			}
			int num = end.x - start.x;
			int num2 = end.z - start.z;
			int num3 = start.x - this.rasterizer.origin.x;
			int num4 = start.z - this.rasterizer.origin.y;
			int num5 = Mathf.Abs(num);
			int num6 = num3 % this.rasterizer.cellSize;
			int num7 = (num <= 0) ? (-num6 - 1) : (this.rasterizer.cellSize - num6);
			int num8 = Mathf.Abs(num7);
			int num9 = this.rasterizer.numCellsX * this.rasterizer.cellSize;
			int num10 = this.rasterizer.numCellsY * this.rasterizer.cellSize;
			int num11 = num3;
			int num12 = num4;
			while (num5 >= 0 && num11 >= 0 && num11 < num9)
			{
				int gridX = num11 / this.rasterizer.cellSize;
				int num13 = Mathf.Abs((num == 0) ? num2 : IntMath.Divide(num2 * num7, num));
				int num14 = num12 % this.rasterizer.cellSize;
				int num15 = (num2 < 0) ? (-num14 - 1) : (this.rasterizer.cellSize - num14);
				int num16 = Mathf.Abs(num15);
				int num17 = num12;
				while (num13 >= 0 && num12 >= 0 && num12 < num10)
				{
					int gridY = num12 / this.rasterizer.cellSize;
					TriangleMeshNode triangleMeshNode = this.checkObjIntersects(ref edge, start, end, gridX, gridY);
					if (triangleMeshNode != null)
					{
						return triangleMeshNode;
					}
					num12 += num15;
					num13 -= num16;
					num15 = ((num2 < 0) ? (-this.rasterizer.cellSize) : this.rasterizer.cellSize);
					num16 = this.rasterizer.cellSize;
				}
				num11 += num7;
				num5 -= num8;
				num7 = ((num < 0) ? (-this.rasterizer.cellSize) : this.rasterizer.cellSize);
				num8 = this.rasterizer.cellSize;
				if (num != 0)
				{
					num12 = (num17 * num + num2 * num7) / num;
				}
			}
			return null;
		}

		public byte[] GetData()
		{
			return this.data;
		}

		public void SetData(byte[] data, uint checksum)
		{
			this.data = data;
			this.dataChecksum = checksum;
		}

		public void Awake()
		{
			this.userConnections = new UserConnection[0];
			this.graphs = new NavGraph[0];
			if (this.cacheStartup && this.data_cachedStartup != null)
			{
				this.LoadFromCache();
			}
			else
			{
				this.DeserializeGraphs();
			}
		}

		public void UpdateShortcuts()
		{
			this.navmesh = (NavMeshGraph)this.FindGraphOfType(typeof(NavMeshGraph));
			this.recastGraph = (RecastGraph)this.FindGraphOfType(typeof(RecastGraph));
		}

		public void LoadFromCache()
		{
			AstarPath.active.BlockUntilPathQueueBlocked();
			if (this.data_cachedStartup != null && this.data_cachedStartup.Length > 0)
			{
				this.DeserializeGraphs(this.data_cachedStartup);
				GraphModifier.TriggerEvent(GraphModifier.EventType.PostCacheLoad);
			}
			else
			{
				Debug.LogError("Can't load from cache since the cache is empty");
			}
		}

		public void SaveCacheData(SerializeSettings settings)
		{
			this.data_cachedStartup = this.SerializeGraphs(settings);
			this.cacheStartup = true;
		}

		public byte[] SerializeGraphs()
		{
			return this.SerializeGraphs(SerializeSettings.Settings);
		}

		public byte[] SerializeGraphs(SerializeSettings settings)
		{
			uint num;
			return this.SerializeGraphs(settings, out num);
		}

		public byte[] SerializeGraphs(SerializeSettings settings, out uint checksum)
		{
			AstarPath.active.BlockUntilPathQueueBlocked();
			AstarSerializer astarSerializer = new AstarSerializer(this, settings);
			astarSerializer.OpenSerialize();
			this.SerializeGraphsPart(astarSerializer);
			byte[] result = astarSerializer.CloseSerialize();
			checksum = astarSerializer.GetChecksum();
			return result;
		}

		public void SerializeGraphsPart(AstarSerializer sr)
		{
			sr.SerializeGraphs(this.graphs);
			sr.SerializeUserConnections(this.userConnections);
			sr.SerializeNodes();
			sr.SerializeExtraInfo();
		}

		public byte[] SerializeGraphsExtra(SerializeSettings settings)
		{
			AstarPath.active.BlockUntilPathQueueBlocked();
			AstarSerializer astarSerializer = new AstarSerializer(this, settings);
			astarSerializer.OpenSerialize();
			astarSerializer.graphs = this.graphs;
			byte[] result = astarSerializer.SerializeExtraInfoBytes();
			astarSerializer.CloseSerialize();
			return result;
		}

		public void DeserializeGraphs()
		{
			if (this.data != null)
			{
				this.DeserializeGraphs(this.data);
			}
		}

		private void ClearGraphs()
		{
			if (this.graphs == null)
			{
				return;
			}
			for (int i = 0; i < this.graphs.Length; i++)
			{
				if (this.graphs[i] != null)
				{
					this.graphs[i].OnDestroy();
				}
			}
			this.graphs = null;
			this.UpdateShortcuts();
		}

		public void OnDestroy()
		{
			this.ClearGraphs();
		}

		public void DeserializeGraphs(byte[] bytes)
		{
			AstarPath.active.BlockUntilPathQueueBlocked();
			try
			{
				if (bytes == null)
				{
					throw new ArgumentNullException("Bytes should not be null when passed to DeserializeGraphs");
				}
				AstarSerializer astarSerializer = new AstarSerializer(this);
				if (astarSerializer.OpenDeserialize(bytes))
				{
					this.DeserializeGraphsPart(astarSerializer);
					astarSerializer.CloseDeserialize();
					this.UpdateShortcuts();
				}
				else
				{
					Debug.Log("Invalid data file (cannot read zip).\nThe data is either corrupt or it was saved using a 3.0.x or earlier version of the system");
				}
				this.active.VerifyIntegrity();
			}
			catch (Exception arg)
			{
				Debug.LogWarning("Caught exception while deserializing data.\n" + arg);
				this.data_backup = bytes;
			}
		}

		public void DeserializeGraphsAdditive(byte[] bytes)
		{
			AstarPath.active.BlockUntilPathQueueBlocked();
			try
			{
				if (bytes == null)
				{
					throw new ArgumentNullException("Bytes should not be null when passed to DeserializeGraphs");
				}
				AstarSerializer astarSerializer = new AstarSerializer(this);
				if (astarSerializer.OpenDeserialize(bytes))
				{
					this.DeserializeGraphsPartAdditive(astarSerializer);
					astarSerializer.CloseDeserialize();
				}
				else
				{
					Debug.Log("Invalid data file (cannot read zip).");
				}
				this.active.VerifyIntegrity();
			}
			catch (Exception arg)
			{
				Debug.LogWarning("Caught exception while deserializing data.\n" + arg);
			}
		}

		public void DeserializeGraphsPart(AstarSerializer sr)
		{
			this.ClearGraphs();
			this.graphs = sr.DeserializeGraphs();
			if (this.graphs != null)
			{
				for (int j = 0; j < this.graphs.Length; j++)
				{
					if (this.graphs[j] != null)
					{
						this.graphs[j].graphIndex = (uint)j;
					}
				}
			}
			this.userConnections = sr.DeserializeUserConnections();
			sr.DeserializeExtraInfo();
			int i;
			for (i = 0; i < this.graphs.Length; i++)
			{
				if (this.graphs[i] != null)
				{
					this.graphs[i].GetNodes(delegate(GraphNode node)
					{
						node.GraphIndex = (uint)i;
						return true;
					});
				}
			}
			sr.PostDeserialization();
		}

		public void DeserializeGraphsPartAdditive(AstarSerializer sr)
		{
			if (this.graphs == null)
			{
				this.graphs = new NavGraph[0];
			}
			if (this.userConnections == null)
			{
				this.userConnections = new UserConnection[0];
			}
			List<NavGraph> list = new List<NavGraph>(this.graphs);
			list.AddRange(sr.DeserializeGraphs());
			this.graphs = list.ToArray();
			if (this.graphs != null)
			{
				for (int l = 0; l < this.graphs.Length; l++)
				{
					if (this.graphs[l] != null)
					{
						this.graphs[l].graphIndex = (uint)l;
					}
				}
			}
			List<UserConnection> list2 = new List<UserConnection>(this.userConnections);
			list2.AddRange(sr.DeserializeUserConnections());
			this.userConnections = list2.ToArray();
			sr.DeserializeNodes();
			DebugHelper.Assert(this.graphs != null, "不能为空");
			int i;
			for (i = 0; i < this.graphs.Length; i++)
			{
				if (this.graphs[i] != null)
				{
					this.graphs[i].GetNodes(delegate(GraphNode node)
					{
						node.GraphIndex = (uint)i;
						return true;
					});
				}
			}
			sr.DeserializeExtraInfo();
			sr.PostDeserialization();
			for (int j = 0; j < this.graphs.Length; j++)
			{
				for (int k = j + 1; k < this.graphs.Length; k++)
				{
					if (this.graphs[j] != null && this.graphs[k] != null && this.graphs[j].guid == this.graphs[k].guid)
					{
						Debug.LogWarning("Guid Conflict when importing graphs additively. Imported graph will get a new Guid.\nThis message is (relatively) harmless.");
						this.graphs[j].guid = Pathfinding.Util.Guid.NewGuid();
						break;
					}
				}
			}
		}

		public void FindGraphTypes()
		{
			Assembly assembly = Assembly.GetAssembly(typeof(AstarPath));
			Type[] types = assembly.GetTypes();
			List<Type> list = new List<Type>();
			Type[] array = types;
			for (int i = 0; i < array.Length; i++)
			{
				Type type = array[i];
				for (Type baseType = type.BaseType; baseType != null; baseType = baseType.BaseType)
				{
					if (object.Equals(baseType, typeof(NavGraph)))
					{
						list.Add(type);
						break;
					}
				}
			}
			this.graphTypes = list.ToArray();
		}

		public Type GetGraphType(string type)
		{
			for (int i = 0; i < this.graphTypes.Length; i++)
			{
				if (this.graphTypes[i].Name == type)
				{
					return this.graphTypes[i];
				}
			}
			return null;
		}

		public NavGraph CreateGraph(string type)
		{
			Debug.Log("Creating Graph of type '" + type + "'");
			for (int i = 0; i < this.graphTypes.Length; i++)
			{
				if (this.graphTypes[i].Name == type)
				{
					return this.CreateGraph(this.graphTypes[i]);
				}
			}
			Debug.LogError("Graph type (" + type + ") wasn't found");
			return null;
		}

		public NavGraph CreateGraph(Type type)
		{
			NavGraph navGraph = Activator.CreateInstance(type) as NavGraph;
			navGraph.active = this.active;
			return navGraph;
		}

		public NavGraph AddGraph(string type)
		{
			NavGraph navGraph = null;
			for (int i = 0; i < this.graphTypes.Length; i++)
			{
				if (this.graphTypes[i].Name == type)
				{
					navGraph = this.CreateGraph(this.graphTypes[i]);
				}
			}
			if (navGraph == null)
			{
				Debug.LogError("No NavGraph of type '" + type + "' could be found");
				return null;
			}
			this.AddGraph(navGraph);
			return navGraph;
		}

		public NavGraph AddGraph(Type type)
		{
			NavGraph navGraph = null;
			for (int i = 0; i < this.graphTypes.Length; i++)
			{
				if (object.Equals(this.graphTypes[i], type))
				{
					navGraph = this.CreateGraph(this.graphTypes[i]);
				}
			}
			if (navGraph == null)
			{
				Debug.LogError(string.Concat(new object[]
				{
					"No NavGraph of type '",
					type,
					"' could be found, ",
					this.graphTypes.Length,
					" graph types are avaliable"
				}));
				return null;
			}
			this.AddGraph(navGraph);
			return navGraph;
		}

		public void AddGraph(NavGraph graph)
		{
			AstarPath.active.BlockUntilPathQueueBlocked();
			for (int i = 0; i < this.graphs.Length; i++)
			{
				if (this.graphs[i] == null)
				{
					this.graphs[i] = graph;
					graph.active = this.active;
					graph.Awake();
					graph.graphIndex = (uint)i;
					this.UpdateShortcuts();
					return;
				}
			}
			if (this.graphs != null && (long)this.graphs.Length >= 255L)
			{
				throw new Exception("Graph Count Limit Reached. You cannot have more than " + 255u + " graphs. Some compiler directives can change this limit, e.g ASTAR_MORE_AREAS, look under the 'Optimizations' tab in the A* Inspector");
			}
			this.graphs = new List<NavGraph>(this.graphs)
			{
				graph
			}.ToArray();
			this.UpdateShortcuts();
			graph.active = this.active;
			graph.Awake();
			graph.graphIndex = (uint)(this.graphs.Length - 1);
		}

		public bool RemoveGraph(NavGraph graph)
		{
			graph.SafeOnDestroy();
			int i;
			for (i = 0; i < this.graphs.Length; i++)
			{
				if (this.graphs[i] == graph)
				{
					break;
				}
			}
			if (i == this.graphs.Length)
			{
				return false;
			}
			this.graphs[i] = null;
			this.UpdateShortcuts();
			return true;
		}

		public static NavGraph GetGraph(GraphNode node)
		{
			if (node == null)
			{
				return null;
			}
			AstarPath active = AstarPath.active;
			if (active == null)
			{
				return null;
			}
			AstarData astarData = active.astarData;
			if (astarData == null)
			{
				return null;
			}
			if (astarData.graphs == null)
			{
				return null;
			}
			uint graphIndex = node.GraphIndex;
			if ((ulong)graphIndex >= (ulong)((long)astarData.graphs.Length))
			{
				return null;
			}
			return astarData.graphs[(int)graphIndex];
		}

		public GraphNode GetNode(int graphIndex, int nodeIndex)
		{
			return this.GetNode(graphIndex, nodeIndex, this.graphs);
		}

		public GraphNode GetNode(int graphIndex, int nodeIndex, NavGraph[] graphs)
		{
			throw new NotImplementedException();
		}

		public NavGraph FindGraphOfType(Type type)
		{
			if (this.graphs != null)
			{
				for (int i = 0; i < this.graphs.Length; i++)
				{
					if (this.graphs[i] != null && object.Equals(this.graphs[i].GetType(), type))
					{
						return this.graphs[i];
					}
				}
			}
			return null;
		}
		

		public int GetGraphIndex(NavGraph graph)
		{
			if (graph == null)
			{
				throw new ArgumentNullException("graph");
			}
			if (this.graphs != null)
			{
				for (int i = 0; i < this.graphs.Length; i++)
				{
					if (graph == this.graphs[i])
					{
						return i;
					}
				}
			}
			Debug.LogError("Graph doesn't exist");
			return -1;
		}

		public int GuidToIndex(Pathfinding.Util.Guid guid)
		{
			if (this.graphs == null)
			{
				return -1;
			}
			for (int i = 0; i < this.graphs.Length; i++)
			{
				if (this.graphs[i] != null)
				{
					if (this.graphs[i].guid == guid)
					{
						return i;
					}
				}
			}
			return -1;
		}

		public NavGraph GuidToGraph(Pathfinding.Util.Guid guid)
		{
			if (this.graphs == null)
			{
				return null;
			}
			for (int i = 0; i < this.graphs.Length; i++)
			{
				if (this.graphs[i] != null)
				{
					if (this.graphs[i].guid == guid)
					{
						return this.graphs[i];
					}
				}
			}
			return null;
		}
	}
}
