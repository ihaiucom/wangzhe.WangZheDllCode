using Pathfinding.Serialization;
using Pathfinding.Serialization.JsonFx;
using Pathfinding.Util;
using Pathfinding.Voxels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace Pathfinding
{
	[JsonOptIn]
	[Serializable]
	public class RecastGraph : NavGraph, INavmesh, INavmeshHolder, IRaycastableGraph, IUpdatableGraph
	{
		public enum RelevantGraphSurfaceMode
		{
			DoNotRequire,
			OnlyForCompletelyInsideTile,
			RequireForAll
		}

		public class NavmeshTile : INavmesh, INavmeshHolder
		{
			public int[] tris;

			public VInt3[] verts;

			public int x;

			public int z;

			public int w;

			public int d;

			public TriangleMeshNode[] nodes;

			public BBTree bbTree;

			public bool flag;

			public RecastGraph.NavmeshTile Clone(RecastGraph owner)
			{
				RecastGraph.NavmeshTile navmeshTile = new RecastGraph.NavmeshTile();
				navmeshTile.tris = new int[this.tris.Length];
				Array.Copy(this.tris, navmeshTile.tris, this.tris.Length);
				navmeshTile.verts = new VInt3[this.verts.Length];
				Array.Copy(this.verts, navmeshTile.verts, this.verts.Length);
				navmeshTile.x = this.x;
				navmeshTile.z = this.z;
				navmeshTile.w = this.w;
				navmeshTile.d = this.d;
				navmeshTile.flag = this.flag;
				Dictionary<int, TriangleMeshNode> dictionary = new Dictionary<int, TriangleMeshNode>();
				AstarData astarData = owner.astarData;
				uint graphIndex = (uint)astarData.graphs.Length;
				int dataGroupIndex = astarData.DataGroupIndex;
				navmeshTile.nodes = new TriangleMeshNode[this.nodes.Length];
				for (int i = 0; i < this.nodes.Length; i++)
				{
					TriangleMeshNode triangleMeshNode = this.nodes[i];
					TriangleMeshNode triangleMeshNode2 = triangleMeshNode.Clone();
					triangleMeshNode2.GraphIndex = graphIndex;
					triangleMeshNode2.DataGroupIndex = dataGroupIndex;
					navmeshTile.nodes[i] = triangleMeshNode2;
					dictionary.Add(triangleMeshNode2.NodeIndex, navmeshTile.nodes[i]);
				}
				for (int j = 0; j < this.nodes.Length; j++)
				{
					TriangleMeshNode triangleMeshNode3 = this.nodes[j];
					TriangleMeshNode triangleMeshNode4 = navmeshTile.nodes[j];
					if (triangleMeshNode3.connections != null)
					{
						triangleMeshNode4.connections = new GraphNode[triangleMeshNode3.connections.Length];
						for (int k = 0; k < triangleMeshNode3.connections.Length; k++)
						{
							int nodeIndex = triangleMeshNode3.connections[k].NodeIndex;
							TriangleMeshNode triangleMeshNode5;
							dictionary.TryGetValue(nodeIndex, ref triangleMeshNode5);
							triangleMeshNode4.connections[k] = triangleMeshNode5;
						}
					}
				}
				dictionary.Clear();
				return navmeshTile;
			}

			public void PostClone()
			{
				this.bbTree = new BBTree(this);
				for (int i = 0; i < this.nodes.Length; i++)
				{
					this.bbTree.Insert(this.nodes[i]);
				}
			}

			public void GetTileCoordinates(int tileIndex, out int x, out int z)
			{
				x = this.x;
				z = this.z;
			}

			public int GetVertexArrayIndex(int index)
			{
				return index & 4095;
			}

			public VInt3 GetVertex(int index)
			{
				int num = index & 4095;
				return this.verts[num];
			}

			public void GetNodes(GraphNodeDelegateCancelable del)
			{
				if (this.nodes == null)
				{
					return;
				}
				int num = 0;
				while (num < this.nodes.Length && del(this.nodes[num]))
				{
					num++;
				}
			}
		}

		public struct SceneMesh
		{
			public Mesh mesh;

			public Matrix4x4 matrix;

			public Bounds bounds;
		}

		private class CapsuleCache
		{
			public int rows;

			public float height;

			public Vector3[] verts;

			public int[] tris;
		}

		public const int VertexIndexMask = 4095;

		public const int TileIndexMask = 524287;

		public const int TileIndexOffset = 12;

		public const int BorderVertexMask = 1;

		public const int BorderVertexOffset = 31;

		private AstarData dataOverride;

		public bool dynamic = true;

		[JsonMember]
		public float characterRadius = 0.5f;

		[JsonMember]
		public float contourMaxError = 2f;

		[JsonMember]
		public float cellSize = 0.5f;

		[JsonMember]
		public float cellHeight = 0.4f;

		[JsonMember]
		public float walkableHeight = 2f;

		[JsonMember]
		public float walkableClimb = 0.5f;

		[JsonMember]
		public float maxSlope = 30f;

		[JsonMember]
		public float maxEdgeLength = 20f;

		[JsonMember]
		public float minRegionSize = 3f;

		[JsonMember]
		public int editorTileSize = 128;

		[JsonMember]
		public int tileSizeX = 128;

		[JsonMember]
		public int tileSizeZ = 128;

		[JsonMember]
		public bool nearestSearchOnlyXZ;

		[JsonMember]
		public bool useTiles;

		public bool scanEmptyGraph;

		[JsonMember]
		public RecastGraph.RelevantGraphSurfaceMode relevantGraphSurfaceMode;

		[JsonMember]
		public bool rasterizeColliders;

		[JsonMember]
		public bool rasterizeMeshes = true;

		[JsonMember]
		public bool rasterizeTerrain = true;

		[JsonMember]
		public bool rasterizeTrees = true;

		[JsonMember]
		public float colliderRasterizeDetail = 10f;

		[JsonMember]
		public Vector3 forcedBoundsCenter;

		[JsonMember]
		public Vector3 forcedBoundsSize = new Vector3(100f, 40f, 100f);

		[JsonMember]
		public LayerMask mask = -1;

		[JsonMember]
		public List<string> tagMask = new List<string>();

		[JsonMember]
		public bool showMeshOutline = true;

		[JsonMember]
		public bool showNodeConnections;

		[JsonMember]
		public int terrainSampleSize = 3;

		private Voxelize globalVox;

		private BBTree _bbTree;

		private VInt3[] _vertices;

		public int tileXCount;

		public int tileZCount;

		private RecastGraph.NavmeshTile[] tiles;

		private bool batchTileUpdate;

		private List<int> batchUpdatedTiles = new List<int>();

		private Dictionary<VInt2, int> cachedInt2_int_dict = new Dictionary<VInt2, int>();

		private Dictionary<VInt3, int> cachedInt3_int_dict = new Dictionary<VInt3, int>();

		private readonly int[] BoxColliderTris = new int[]
		{
			0,
			1,
			2,
			0,
			2,
			3,
			6,
			5,
			4,
			7,
			6,
			4,
			0,
			5,
			1,
			0,
			4,
			5,
			1,
			6,
			2,
			1,
			5,
			6,
			2,
			7,
			3,
			2,
			6,
			7,
			3,
			4,
			0,
			3,
			7,
			4
		};

		private readonly Vector3[] BoxColliderVerts = new Vector3[]
		{
			new Vector3(-1f, -1f, -1f),
			new Vector3(1f, -1f, -1f),
			new Vector3(1f, -1f, 1f),
			new Vector3(-1f, -1f, 1f),
			new Vector3(-1f, 1f, -1f),
			new Vector3(1f, 1f, -1f),
			new Vector3(1f, 1f, 1f),
			new Vector3(-1f, 1f, 1f)
		};

		private List<RecastGraph.CapsuleCache> capsuleCache = new List<RecastGraph.CapsuleCache>();

		public AstarData astarData
		{
			get
			{
				return this.dataOverride ?? AstarPath.active.astarData;
			}
		}

		public Bounds forcedBounds
		{
			get
			{
				return new Bounds(this.forcedBoundsCenter, this.forcedBoundsSize);
			}
		}

		public BBTree bbTree
		{
			get
			{
				return this._bbTree;
			}
			set
			{
				this._bbTree = value;
			}
		}

		public VInt3[] vertices
		{
			get
			{
				return this._vertices;
			}
			set
			{
				this._vertices = value;
			}
		}

		public override void CreateNodes(int number)
		{
			throw new NotSupportedException();
		}

		public RecastGraph Clone(AstarData owner)
		{
			RecastGraph recastGraph = new RecastGraph();
			recastGraph.dataOverride = owner;
			int graphIndex = owner.graphs.Length;
			TriangleMeshNode.SetNavmeshHolder(owner.DataGroupIndex, graphIndex, recastGraph);
			this.Duplicate(recastGraph);
			return recastGraph;
		}

		protected override void Duplicate(NavGraph graph)
		{
			base.Duplicate(graph);
			RecastGraph recastGraph = (RecastGraph)graph;
			recastGraph.dynamic = this.dynamic;
			recastGraph.characterRadius = this.characterRadius;
			recastGraph.contourMaxError = this.contourMaxError;
			recastGraph.cellSize = this.cellSize;
			recastGraph.cellHeight = this.cellHeight;
			recastGraph.walkableHeight = this.walkableHeight;
			recastGraph.walkableClimb = this.walkableClimb;
			recastGraph.maxSlope = this.maxSlope;
			recastGraph.maxEdgeLength = this.maxEdgeLength;
			recastGraph.minRegionSize = this.minRegionSize;
			recastGraph.editorTileSize = this.editorTileSize;
			recastGraph.tileSizeX = this.tileSizeX;
			recastGraph.tileSizeZ = this.tileSizeZ;
			recastGraph.nearestSearchOnlyXZ = this.nearestSearchOnlyXZ;
			recastGraph.useTiles = this.useTiles;
			recastGraph.scanEmptyGraph = this.scanEmptyGraph;
			recastGraph.relevantGraphSurfaceMode = this.relevantGraphSurfaceMode;
			recastGraph.rasterizeColliders = this.rasterizeColliders;
			recastGraph.rasterizeMeshes = this.rasterizeMeshes;
			recastGraph.rasterizeTerrain = this.rasterizeTerrain;
			recastGraph.rasterizeTrees = this.rasterizeTrees;
			recastGraph.colliderRasterizeDetail = this.colliderRasterizeDetail;
			recastGraph.forcedBoundsCenter = this.forcedBoundsCenter;
			recastGraph.forcedBoundsSize = this.forcedBoundsSize;
			recastGraph.mask = this.mask;
			recastGraph.tagMask = new List<string>(this.tagMask);
			recastGraph.showMeshOutline = this.showMeshOutline;
			recastGraph.showNodeConnections = this.showNodeConnections;
			recastGraph.terrainSampleSize = this.terrainSampleSize;
			if (this._vertices != null)
			{
				recastGraph._vertices = new VInt3[this._vertices.Length];
				for (int i = 0; i < this._vertices.Length; i++)
				{
					recastGraph._vertices[i] = this._vertices[i];
				}
			}
			recastGraph.tileXCount = this.tileXCount;
			recastGraph.tileZCount = this.tileZCount;
			recastGraph.tiles = new RecastGraph.NavmeshTile[this.tiles.Length];
			for (int j = 0; j < this.tiles.Length; j++)
			{
				recastGraph.tiles[j] = this.tiles[j].Clone(recastGraph);
				recastGraph.tiles[j].PostClone();
			}
			recastGraph.batchTileUpdate = this.batchTileUpdate;
			recastGraph.batchUpdatedTiles = new List<int>();
		}

		public VInt3 GetVertex(int index)
		{
			int num = index >> 12 & 524287;
			return this.tiles[num].GetVertex(index);
		}

		public int GetTileIndex(int index)
		{
			return index >> 12 & 524287;
		}

		public int GetVertexArrayIndex(int index)
		{
			return index & 4095;
		}

		public void GetTileCoordinates(int tileIndex, out int x, out int z)
		{
			z = tileIndex / this.tileXCount;
			x = tileIndex - z * this.tileXCount;
		}

		public RecastGraph.NavmeshTile[] GetTiles()
		{
			return this.tiles;
		}

		public Bounds GetTileBounds(IntRect rect)
		{
			return this.GetTileBounds(rect.xmin, rect.ymin, rect.Width, rect.Height);
		}

		public Bounds GetTileBounds(int x, int z, int width = 1, int depth = 1)
		{
			Bounds result = default(Bounds);
			result.SetMinMax(new Vector3((float)(x * this.tileSizeX) * this.cellSize, 0f, (float)(z * this.tileSizeZ) * this.cellSize) + this.forcedBounds.min, new Vector3((float)((x + width) * this.tileSizeX) * this.cellSize, this.forcedBounds.size.y, (float)((z + depth) * this.tileSizeZ) * this.cellSize) + this.forcedBounds.min);
			return result;
		}

		public VInt2 GetTileCoordinates(Vector3 p)
		{
			p -= this.forcedBounds.min;
			p.x /= this.cellSize * (float)this.tileSizeX;
			p.z /= this.cellSize * (float)this.tileSizeZ;
			return new VInt2((int)p.x, (int)p.z);
		}

		public override void OnDestroy()
		{
			base.OnDestroy();
			TriangleMeshNode.SetNavmeshHolder(this.astarData.DataGroupIndex, this.astarData.GetGraphIndex(this), null);
		}

		private static RecastGraph.NavmeshTile NewEmptyTile(int x, int z)
		{
			RecastGraph.NavmeshTile navmeshTile = new RecastGraph.NavmeshTile();
			navmeshTile.x = x;
			navmeshTile.z = z;
			navmeshTile.w = 1;
			navmeshTile.d = 1;
			navmeshTile.verts = new VInt3[0];
			navmeshTile.tris = new int[0];
			navmeshTile.nodes = new TriangleMeshNode[0];
			navmeshTile.bbTree = new BBTree(navmeshTile);
			return navmeshTile;
		}

		public override void GetNodes(GraphNodeDelegateCancelable del)
		{
			if (this.tiles == null)
			{
				return;
			}
			for (int i = 0; i < this.tiles.Length; i++)
			{
				if (this.tiles[i] != null && this.tiles[i].x + this.tiles[i].z * this.tileXCount == i)
				{
					TriangleMeshNode[] nodes = this.tiles[i].nodes;
					if (nodes != null)
					{
						int num = 0;
						while (num < nodes.Length && del(nodes[num]))
						{
							num++;
						}
					}
				}
			}
		}

		public Vector3 ClosestPointOnNode(TriangleMeshNode node, Vector3 pos)
		{
			return Polygon.ClosestPointOnTriangle((Vector3)this.GetVertex(node.v0), (Vector3)this.GetVertex(node.v1), (Vector3)this.GetVertex(node.v2), pos);
		}

		public bool ContainsPoint(TriangleMeshNode node, Vector3 pos)
		{
			return Polygon.IsClockwise((Vector3)this.GetVertex(node.v0), (Vector3)this.GetVertex(node.v1), pos) && Polygon.IsClockwise((Vector3)this.GetVertex(node.v1), (Vector3)this.GetVertex(node.v2), pos) && Polygon.IsClockwise((Vector3)this.GetVertex(node.v2), (Vector3)this.GetVertex(node.v0), pos);
		}

		public void SnapForceBoundsToScene()
		{
			List<ExtraMesh> list = new List<ExtraMesh>();
			RecastGraph.GetSceneMeshes(this.forcedBounds, this.tagMask, this.mask, list);
			if (list.get_Count() == 0)
			{
				return;
			}
			Bounds bounds = default(Bounds);
			Bounds bounds2 = list.get_Item(0).bounds;
			for (int i = 1; i < list.get_Count(); i++)
			{
				bounds2.Encapsulate(list.get_Item(i).bounds);
			}
			this.forcedBoundsCenter = bounds2.center;
			this.forcedBoundsSize = bounds2.size;
		}

		public void GetRecastMeshObjs(Bounds bounds, List<ExtraMesh> buffer)
		{
			List<RecastMeshObj> list = ListPool<RecastMeshObj>.Claim();
			RecastMeshObj.GetAllInBounds(list, bounds);
			DictionaryObjectView<Mesh, Vector3[]> dictionaryObjectView = new DictionaryObjectView<Mesh, Vector3[]>();
			DictionaryObjectView<Mesh, int[]> dictionaryObjectView2 = new DictionaryObjectView<Mesh, int[]>();
			for (int i = 0; i < list.get_Count(); i++)
			{
				MeshFilter meshFilter = list.get_Item(i).GetMeshFilter();
				Renderer renderer = (meshFilter != null) ? meshFilter.GetComponent<Renderer>() : null;
				if (meshFilter != null && renderer != null)
				{
					Mesh sharedMesh = meshFilter.sharedMesh;
					ExtraMesh extraMesh = default(ExtraMesh);
					extraMesh.name = renderer.name;
					extraMesh.matrix = renderer.get_localToWorldMatrix();
					extraMesh.original = meshFilter;
					extraMesh.area = list.get_Item(i).area;
					if (dictionaryObjectView.ContainsKey(sharedMesh))
					{
						extraMesh.vertices = dictionaryObjectView[sharedMesh];
						extraMesh.triangles = dictionaryObjectView2[sharedMesh];
					}
					else
					{
						extraMesh.vertices = sharedMesh.vertices;
						extraMesh.triangles = sharedMesh.triangles;
						dictionaryObjectView[sharedMesh] = extraMesh.vertices;
						dictionaryObjectView2[sharedMesh] = extraMesh.triangles;
					}
					extraMesh.bounds = renderer.bounds;
					buffer.Add(extraMesh);
				}
				else
				{
					Collider collider = list.get_Item(i).GetCollider();
					if (collider == null)
					{
						Debug.LogError("RecastMeshObject (" + list.get_Item(i).gameObject.name + ") didn't have a collider or MeshFilter attached");
					}
					else
					{
						ExtraMesh extraMesh2 = this.RasterizeCollider(collider);
						extraMesh2.area = list.get_Item(i).area;
						if (extraMesh2.vertices != null)
						{
							buffer.Add(extraMesh2);
						}
					}
				}
			}
			this.capsuleCache.Clear();
			ListPool<RecastMeshObj>.Release(list);
		}

		private static void GetSceneMeshes(Bounds bounds, List<string> tagMask, LayerMask layerMask, List<ExtraMesh> meshes)
		{
			if ((tagMask != null && tagMask.get_Count() > 0) || layerMask != 0)
			{
				MeshFilter[] array = Object.FindObjectsOfType(typeof(MeshFilter)) as MeshFilter[];
				ListView<MeshFilter> listView = new ListView<MeshFilter>(array.Length / 3);
				for (int i = 0; i < array.Length; i++)
				{
					MeshFilter meshFilter = array[i];
					Renderer component = meshFilter.GetComponent<Renderer>();
					if (component != null && meshFilter.sharedMesh != null && component.enabled && ((1 << meshFilter.gameObject.layer & layerMask) != 0 || tagMask.Contains(meshFilter.tag)) && meshFilter.GetComponent<RecastMeshObj>() == null)
					{
						listView.Add(meshFilter);
					}
				}
				DictionaryObjectView<Mesh, Vector3[]> dictionaryObjectView = new DictionaryObjectView<Mesh, Vector3[]>();
				DictionaryObjectView<Mesh, int[]> dictionaryObjectView2 = new DictionaryObjectView<Mesh, int[]>();
				bool flag = false;
				for (int j = 0; j < listView.Count; j++)
				{
					MeshFilter meshFilter2 = listView[j];
					Renderer component2 = meshFilter2.GetComponent<Renderer>();
					if (component2.get_isPartOfStaticBatch())
					{
						flag = true;
					}
					else if (component2.bounds.Intersects(bounds))
					{
						Mesh sharedMesh = meshFilter2.sharedMesh;
						ExtraMesh extraMesh = default(ExtraMesh);
						extraMesh.name = component2.name;
						extraMesh.matrix = component2.get_localToWorldMatrix();
						extraMesh.original = meshFilter2;
						if (dictionaryObjectView.ContainsKey(sharedMesh))
						{
							extraMesh.vertices = dictionaryObjectView[sharedMesh];
							extraMesh.triangles = dictionaryObjectView2[sharedMesh];
						}
						else
						{
							extraMesh.vertices = sharedMesh.vertices;
							extraMesh.triangles = sharedMesh.triangles;
							dictionaryObjectView[sharedMesh] = extraMesh.vertices;
							dictionaryObjectView2[sharedMesh] = extraMesh.triangles;
						}
						extraMesh.bounds = component2.bounds;
						meshes.Add(extraMesh);
					}
					if (flag)
					{
						Debug.LogWarning("Some meshes were statically batched. These meshes can not be used for navmesh calculation due to technical constraints.\nDuring runtime scripts cannot access the data of meshes which have been statically batched.\nOne way to solve this problem is to use cached startup (Save & Load tab in the inspector) to only calculate the graph when the game is not playing.");
					}
				}
			}
		}

		public IntRect GetTouchingTiles(Bounds b)
		{
			b.center -= this.forcedBounds.min;
			IntRect intRect = new IntRect(Mathf.FloorToInt(b.min.x / ((float)this.tileSizeX * this.cellSize)), Mathf.FloorToInt(b.min.z / ((float)this.tileSizeZ * this.cellSize)), Mathf.FloorToInt(b.max.x / ((float)this.tileSizeX * this.cellSize)), Mathf.FloorToInt(b.max.z / ((float)this.tileSizeZ * this.cellSize)));
			intRect = IntRect.Intersection(intRect, new IntRect(0, 0, this.tileXCount - 1, this.tileZCount - 1));
			return intRect;
		}

		public IntRect GetTouchingTilesRound(Bounds b)
		{
			b.center -= this.forcedBounds.min;
			IntRect intRect = new IntRect(Mathf.RoundToInt(b.min.x / ((float)this.tileSizeX * this.cellSize)), Mathf.RoundToInt(b.min.z / ((float)this.tileSizeZ * this.cellSize)), Mathf.RoundToInt(b.max.x / ((float)this.tileSizeX * this.cellSize)) - 1, Mathf.RoundToInt(b.max.z / ((float)this.tileSizeZ * this.cellSize)) - 1);
			intRect = IntRect.Intersection(intRect, new IntRect(0, 0, this.tileXCount - 1, this.tileZCount - 1));
			return intRect;
		}

		public GraphUpdateThreading CanUpdateAsync(GraphUpdateObject o)
		{
			if (o.updatePhysics)
			{
				return GraphUpdateThreading.SeparateAndUnityInit;
			}
			return GraphUpdateThreading.SeparateThread;
		}

		public void UpdateAreaInit(GraphUpdateObject o)
		{
			if (!o.updatePhysics)
			{
				return;
			}
			if (!this.dynamic)
			{
				throw new Exception("Recast graph must be marked as dynamic to enable graph updates");
			}
			RelevantGraphSurface.UpdateAllPositions();
			IntRect touchingTiles = this.GetTouchingTiles(o.bounds);
			Bounds tileBounds = this.GetTileBounds(touchingTiles);
			int num = Mathf.CeilToInt(this.characterRadius / this.cellSize);
			int num2 = num + 3;
			tileBounds.Expand(new Vector3((float)num2, 0f, (float)num2) * this.cellSize * 2f);
			List<ExtraMesh> inputExtraMeshes;
			this.CollectMeshes(out inputExtraMeshes, tileBounds);
			Voxelize voxelize = this.globalVox;
			if (voxelize == null)
			{
				voxelize = new Voxelize(this.cellHeight, this.cellSize, this.walkableClimb, this.walkableHeight, this.maxSlope);
				voxelize.maxEdgeLength = this.maxEdgeLength;
				if (this.dynamic)
				{
					this.globalVox = voxelize;
				}
			}
			voxelize.inputExtraMeshes = inputExtraMeshes;
		}

		public void UpdateArea(GraphUpdateObject guo)
		{
			Bounds bounds = guo.bounds;
			bounds.center -= this.forcedBounds.min;
			IntRect a = new IntRect(Mathf.FloorToInt(bounds.min.x / ((float)this.tileSizeX * this.cellSize)), Mathf.FloorToInt(bounds.min.z / ((float)this.tileSizeZ * this.cellSize)), Mathf.FloorToInt(bounds.max.x / ((float)this.tileSizeX * this.cellSize)), Mathf.FloorToInt(bounds.max.z / ((float)this.tileSizeZ * this.cellSize)));
			a = IntRect.Intersection(a, new IntRect(0, 0, this.tileXCount - 1, this.tileZCount - 1));
			if (!guo.updatePhysics)
			{
				for (int i = a.ymin; i <= a.ymax; i++)
				{
					for (int j = a.xmin; j <= a.xmax; j++)
					{
						RecastGraph.NavmeshTile navmeshTile = this.tiles[i * this.tileXCount + j];
						navmeshTile.flag = true;
					}
				}
				for (int k = a.ymin; k <= a.ymax; k++)
				{
					for (int l = a.xmin; l <= a.xmax; l++)
					{
						RecastGraph.NavmeshTile navmeshTile2 = this.tiles[k * this.tileXCount + l];
						if (navmeshTile2.flag)
						{
							navmeshTile2.flag = false;
							NavMeshGraph.UpdateArea(guo, navmeshTile2);
						}
					}
				}
				return;
			}
			if (!this.dynamic)
			{
				throw new Exception("Recast graph must be marked as dynamic to enable graph updates with updatePhysics = true");
			}
			Voxelize voxelize = this.globalVox;
			if (voxelize == null)
			{
				throw new InvalidOperationException("No Voxelizer object. UpdateAreaInit should have been called before this function.");
			}
			for (int m = a.xmin; m <= a.xmax; m++)
			{
				for (int n = a.ymin; n <= a.ymax; n++)
				{
					this.RemoveConnectionsFromTile(this.tiles[m + n * this.tileXCount]);
				}
			}
			for (int num = a.xmin; num <= a.xmax; num++)
			{
				for (int num2 = a.ymin; num2 <= a.ymax; num2++)
				{
					this.BuildTileMesh(voxelize, num, num2);
				}
			}
			uint graphIndex = (uint)this.astarData.GetGraphIndex(this);
			for (int num3 = a.xmin; num3 <= a.xmax; num3++)
			{
				for (int num4 = a.ymin; num4 <= a.ymax; num4++)
				{
					RecastGraph.NavmeshTile navmeshTile3 = this.tiles[num3 + num4 * this.tileXCount];
					GraphNode[] nodes = navmeshTile3.nodes;
					for (int num5 = 0; num5 < nodes.Length; num5++)
					{
						nodes[num5].GraphIndex = graphIndex;
					}
				}
			}
			a = a.Expand(1);
			a = IntRect.Intersection(a, new IntRect(0, 0, this.tileXCount - 1, this.tileZCount - 1));
			for (int num6 = a.xmin; num6 <= a.xmax; num6++)
			{
				for (int num7 = a.ymin; num7 <= a.ymax; num7++)
				{
					if (num6 < this.tileXCount - 1 && a.Contains(num6 + 1, num7))
					{
						this.ConnectTiles(this.tiles[num6 + num7 * this.tileXCount], this.tiles[num6 + 1 + num7 * this.tileXCount]);
					}
					if (num7 < this.tileZCount - 1 && a.Contains(num6, num7 + 1))
					{
						this.ConnectTiles(this.tiles[num6 + num7 * this.tileXCount], this.tiles[num6 + (num7 + 1) * this.tileXCount]);
					}
				}
			}
		}

		public void ConnectTileWithNeighbours(RecastGraph.NavmeshTile tile)
		{
			if (tile.x > 0)
			{
				int num = tile.x - 1;
				for (int i = tile.z; i < tile.z + tile.d; i++)
				{
					this.ConnectTiles(this.tiles[num + i * this.tileXCount], tile);
				}
			}
			if (tile.x + tile.w < this.tileXCount)
			{
				int num2 = tile.x + tile.w;
				for (int j = tile.z; j < tile.z + tile.d; j++)
				{
					this.ConnectTiles(this.tiles[num2 + j * this.tileXCount], tile);
				}
			}
			if (tile.z > 0)
			{
				int num3 = tile.z - 1;
				for (int k = tile.x; k < tile.x + tile.w; k++)
				{
					this.ConnectTiles(this.tiles[k + num3 * this.tileXCount], tile);
				}
			}
			if (tile.z + tile.d < this.tileZCount)
			{
				int num4 = tile.z + tile.d;
				for (int l = tile.x; l < tile.x + tile.w; l++)
				{
					this.ConnectTiles(this.tiles[l + num4 * this.tileXCount], tile);
				}
			}
		}

		public void RemoveConnectionsFromTile(RecastGraph.NavmeshTile tile)
		{
			if (tile.x > 0)
			{
				int num = tile.x - 1;
				for (int i = tile.z; i < tile.z + tile.d; i++)
				{
					this.RemoveConnectionsFromTo(this.tiles[num + i * this.tileXCount], tile);
				}
			}
			if (tile.x + tile.w < this.tileXCount)
			{
				int num2 = tile.x + tile.w;
				for (int j = tile.z; j < tile.z + tile.d; j++)
				{
					this.RemoveConnectionsFromTo(this.tiles[num2 + j * this.tileXCount], tile);
				}
			}
			if (tile.z > 0)
			{
				int num3 = tile.z - 1;
				for (int k = tile.x; k < tile.x + tile.w; k++)
				{
					this.RemoveConnectionsFromTo(this.tiles[k + num3 * this.tileXCount], tile);
				}
			}
			if (tile.z + tile.d < this.tileZCount)
			{
				int num4 = tile.z + tile.d;
				for (int l = tile.x; l < tile.x + tile.w; l++)
				{
					this.RemoveConnectionsFromTo(this.tiles[l + num4 * this.tileXCount], tile);
				}
			}
		}

		public void RemoveConnectionsFromTo(RecastGraph.NavmeshTile a, RecastGraph.NavmeshTile b)
		{
			if (a == null || b == null)
			{
				return;
			}
			if (a == b)
			{
				return;
			}
			int num = b.x + b.z * this.tileXCount;
			for (int i = 0; i < a.nodes.Length; i++)
			{
				TriangleMeshNode triangleMeshNode = a.nodes[i];
				if (triangleMeshNode.connections != null)
				{
					for (int j = 0; j < triangleMeshNode.connections.Length; j++)
					{
						TriangleMeshNode triangleMeshNode2 = triangleMeshNode.connections[j] as TriangleMeshNode;
						if (triangleMeshNode2 != null)
						{
							int num2 = triangleMeshNode2.GetVertexIndex(0);
							num2 = (num2 >> 12 & 524287);
							if (num2 == num)
							{
								triangleMeshNode.RemoveConnection(triangleMeshNode.connections[j]);
								j--;
							}
						}
					}
				}
			}
		}

		public override NNInfo GetNearest(Vector3 position, NNConstraint constraint, GraphNode hint)
		{
			return this.GetNearestForce(position, null);
		}

		public override NNInfo GetNearestForce(Vector3 position, NNConstraint constraint)
		{
			if (this.tiles == null)
			{
				return default(NNInfo);
			}
			Vector3 vector = position - this.forcedBounds.min;
			int num = Mathf.FloorToInt(vector.x / (this.cellSize * (float)this.tileSizeX));
			int num2 = Mathf.FloorToInt(vector.z / (this.cellSize * (float)this.tileSizeZ));
			num = Mathf.Clamp(num, 0, this.tileXCount - 1);
			num2 = Mathf.Clamp(num2, 0, this.tileZCount - 1);
			int num3 = Math.Max(this.tileXCount, this.tileZCount);
			NNInfo nNInfo = default(NNInfo);
			float num4 = float.PositiveInfinity;
			bool flag = this.nearestSearchOnlyXZ || (constraint != null && constraint.distanceXZ);
			for (int i = 0; i < num3; i++)
			{
				if (!flag && num4 < (float)(i - 1) * this.cellSize * (float)Math.Max(this.tileSizeX, this.tileSizeZ))
				{
					break;
				}
				int num5 = Math.Min(i + num2 + 1, this.tileZCount);
				for (int j = Math.Max(-i + num2, 0); j < num5; j++)
				{
					int num6 = Math.Abs(i - Math.Abs(j - num2));
					if (-num6 + num >= 0)
					{
						int num7 = -num6 + num;
						RecastGraph.NavmeshTile navmeshTile = this.tiles[num7 + j * this.tileXCount];
						if (navmeshTile != null)
						{
							if (flag)
							{
								nNInfo = navmeshTile.bbTree.QueryClosestXZ(position, constraint, ref num4, nNInfo);
								if (num4 < float.PositiveInfinity)
								{
									break;
								}
							}
							else
							{
								nNInfo = navmeshTile.bbTree.QueryClosest(position, constraint, ref num4, nNInfo);
							}
						}
					}
					if (num6 != 0 && num6 + num < this.tileXCount)
					{
						int num8 = num6 + num;
						RecastGraph.NavmeshTile navmeshTile2 = this.tiles[num8 + j * this.tileXCount];
						if (navmeshTile2 != null)
						{
							if (flag)
							{
								nNInfo = navmeshTile2.bbTree.QueryClosestXZ(position, constraint, ref num4, nNInfo);
								if (num4 < float.PositiveInfinity)
								{
									break;
								}
							}
							else
							{
								nNInfo = navmeshTile2.bbTree.QueryClosest(position, constraint, ref num4, nNInfo);
							}
						}
					}
				}
			}
			nNInfo.node = nNInfo.constrainedNode;
			nNInfo.constrainedNode = null;
			nNInfo.clampedPosition = nNInfo.constClampedPosition;
			return nNInfo;
		}

		public GraphNode PointOnNavmesh(Vector3 position, NNConstraint constraint)
		{
			if (this.tiles == null)
			{
				return null;
			}
			Vector3 vector = position - this.forcedBounds.min;
			int num = Mathf.FloorToInt(vector.x / (this.cellSize * (float)this.tileSizeX));
			int num2 = Mathf.FloorToInt(vector.z / (this.cellSize * (float)this.tileSizeZ));
			if (num < 0 || num2 < 0 || num >= this.tileXCount || num2 >= this.tileZCount)
			{
				return null;
			}
			RecastGraph.NavmeshTile navmeshTile = this.tiles[num + num2 * this.tileXCount];
			if (navmeshTile != null)
			{
				return navmeshTile.bbTree.QueryInside(position, constraint);
			}
			return null;
		}

		public static string GetRecastPath()
		{
			return Application.dataPath + "/Recast/recast";
		}

		public override void ScanInternal(OnScanStatus statusCallback)
		{
			TriangleMeshNode.SetNavmeshHolder(0, this.astarData.GetGraphIndex(this), this);
			this.ScanTiledNavmesh(statusCallback);
		}

		protected void ScanTiledNavmesh(OnScanStatus statusCallback)
		{
			this.ScanAllTiles(statusCallback);
		}

		protected void ScanAllTiles(OnScanStatus statusCallback)
		{
			int num = (int)(this.forcedBounds.size.x / this.cellSize + 0.5f);
			int num2 = (int)(this.forcedBounds.size.z / this.cellSize + 0.5f);
			if (!this.useTiles)
			{
				this.tileSizeX = num;
				this.tileSizeZ = num2;
			}
			else
			{
				this.tileSizeX = this.editorTileSize;
				this.tileSizeZ = this.editorTileSize;
			}
			int num3 = (num + this.tileSizeX - 1) / this.tileSizeX;
			int num4 = (num2 + this.tileSizeZ - 1) / this.tileSizeZ;
			this.tileXCount = num3;
			this.tileZCount = num4;
			if (this.tileXCount * this.tileZCount > 524288)
			{
				throw new Exception(string.Concat(new object[]
				{
					"Too many tiles (",
					this.tileXCount * this.tileZCount,
					") maximum is ",
					524288,
					"\nTry disabling ASTAR_RECAST_LARGER_TILES under the 'Optimizations' tab in the A* inspector."
				}));
			}
			this.tiles = new RecastGraph.NavmeshTile[this.tileXCount * this.tileZCount];
			if (this.scanEmptyGraph)
			{
				for (int i = 0; i < num4; i++)
				{
					for (int j = 0; j < num3; j++)
					{
						this.tiles[i * this.tileXCount + j] = RecastGraph.NewEmptyTile(j, i);
					}
				}
				return;
			}
			Console.WriteLine("Collecting Meshes");
			List<ExtraMesh> inputExtraMeshes;
			this.CollectMeshes(out inputExtraMeshes, this.forcedBounds);
			Voxelize voxelize = new Voxelize(this.cellHeight, this.cellSize, this.walkableClimb, this.walkableHeight, this.maxSlope);
			voxelize.inputExtraMeshes = inputExtraMeshes;
			voxelize.maxEdgeLength = this.maxEdgeLength;
			int num5 = -1;
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			for (int k = 0; k < num4; k++)
			{
				for (int l = 0; l < num3; l++)
				{
					int num6 = k * this.tileXCount + l;
					Console.WriteLine(string.Concat(new object[]
					{
						"Generating Tile #",
						num6,
						" of ",
						num4 * num3
					}));
					if ((num6 * 10 / this.tiles.Length > num5 || stopwatch.get_ElapsedMilliseconds() > 2000L) && statusCallback != null)
					{
						num5 = num6 * 10 / this.tiles.Length;
						stopwatch.Reset();
						stopwatch.Start();
						statusCallback(new Progress(AstarMath.MapToRange(0.1f, 0.9f, (float)num6 / (float)this.tiles.Length), string.Concat(new object[]
						{
							"Building Tile ",
							num6,
							"/",
							this.tiles.Length
						})));
					}
					this.BuildTileMesh(voxelize, l, k);
				}
			}
			Console.WriteLine("Assigning Graph Indices");
			if (statusCallback != null)
			{
				statusCallback(new Progress(0.9f, "Connecting tiles"));
			}
			uint graphIndex = (uint)this.astarData.GetGraphIndex(this);
			GraphNodeDelegateCancelable del = delegate(GraphNode n)
			{
				n.GraphIndex = graphIndex;
				return true;
			};
			this.GetNodes(del);
			for (int m = 0; m < num4; m++)
			{
				for (int n2 = 0; n2 < num3; n2++)
				{
					Console.WriteLine(string.Concat(new object[]
					{
						"Connecing Tile #",
						m * this.tileXCount + n2,
						" of ",
						num4 * num3
					}));
					if (n2 < num3 - 1)
					{
						this.ConnectTiles(this.tiles[n2 + m * this.tileXCount], this.tiles[n2 + 1 + m * this.tileXCount]);
					}
					if (m < num4 - 1)
					{
						this.ConnectTiles(this.tiles[n2 + m * this.tileXCount], this.tiles[n2 + (m + 1) * this.tileXCount]);
					}
				}
			}
		}

		protected void BuildTileMesh(Voxelize vox, int x, int z)
		{
			float num = (float)this.tileSizeX * this.cellSize;
			float num2 = (float)this.tileSizeZ * this.cellSize;
			int num3 = Mathf.CeilToInt(this.characterRadius / this.cellSize);
			Vector3 min = this.forcedBounds.min;
			Vector3 max = this.forcedBounds.max;
			Bounds forcedBounds = default(Bounds);
			forcedBounds.SetMinMax(new Vector3((float)x * num, 0f, (float)z * num2) + min, new Vector3((float)(x + 1) * num + min.x, max.y, (float)(z + 1) * num2 + min.z));
			vox.borderSize = num3 + 3;
			forcedBounds.Expand(new Vector3((float)vox.borderSize, 0f, (float)vox.borderSize) * this.cellSize * 2f);
			vox.forcedBounds = forcedBounds;
			vox.width = this.tileSizeX + vox.borderSize * 2;
			vox.depth = this.tileSizeZ + vox.borderSize * 2;
			if (!this.useTiles && this.relevantGraphSurfaceMode == RecastGraph.RelevantGraphSurfaceMode.OnlyForCompletelyInsideTile)
			{
				vox.relevantGraphSurfaceMode = RecastGraph.RelevantGraphSurfaceMode.RequireForAll;
			}
			else
			{
				vox.relevantGraphSurfaceMode = this.relevantGraphSurfaceMode;
			}
			vox.minRegionSize = Mathf.RoundToInt(this.minRegionSize / (this.cellSize * this.cellSize));
			vox.Init();
			vox.CollectMeshes();
			vox.VoxelizeInput();
			vox.FilterLedges(vox.voxelWalkableHeight, vox.voxelWalkableClimb, vox.cellSize, vox.cellHeight, vox.forcedBounds.min);
			vox.FilterLowHeightSpans(vox.voxelWalkableHeight, vox.cellSize, vox.cellHeight, vox.forcedBounds.min);
			vox.BuildCompactField();
			vox.BuildVoxelConnections();
			vox.ErodeWalkableArea(num3);
			vox.BuildDistanceField();
			vox.BuildRegions();
			VoxelContourSet cset = new VoxelContourSet();
			vox.BuildContours(this.contourMaxError, 1, cset, 1);
			VoxelMesh mesh;
			vox.BuildPolyMesh(cset, 3, out mesh);
			for (int i = 0; i < mesh.verts.Length; i++)
			{
				mesh.verts[i] = mesh.verts[i] * 1000 * vox.cellScale + (VInt3)vox.voxelOffset;
			}
			RecastGraph.NavmeshTile navmeshTile = this.CreateTile(vox, mesh, x, z);
			this.tiles[navmeshTile.x + navmeshTile.z * this.tileXCount] = navmeshTile;
		}

		private RecastGraph.NavmeshTile CreateTile(Voxelize vox, VoxelMesh mesh, int x, int z)
		{
			if (mesh.tris == null)
			{
				throw new ArgumentNullException("The mesh must be valid. tris is null.");
			}
			if (mesh.verts == null)
			{
				throw new ArgumentNullException("The mesh must be valid. verts is null.");
			}
			RecastGraph.NavmeshTile navmeshTile = new RecastGraph.NavmeshTile();
			navmeshTile.x = x;
			navmeshTile.z = z;
			navmeshTile.w = 1;
			navmeshTile.d = 1;
			navmeshTile.tris = mesh.tris;
			navmeshTile.verts = mesh.verts;
			navmeshTile.bbTree = new BBTree(navmeshTile);
			if (navmeshTile.tris.Length % 3 != 0)
			{
				throw new ArgumentException("Indices array's length must be a multiple of 3 (mesh.tris)");
			}
			if (navmeshTile.verts.Length >= 4095)
			{
				throw new ArgumentException("Too many vertices per tile (more than " + 4095 + ").\nTry enabling ASTAR_RECAST_LARGER_TILES under the 'Optimizations' tab in the A* Inspector");
			}
			Dictionary<VInt3, int> dictionary = this.cachedInt3_int_dict;
			dictionary.Clear();
			int[] array = new int[navmeshTile.verts.Length];
			int num = 0;
			for (int i = 0; i < navmeshTile.verts.Length; i++)
			{
				try
				{
					dictionary.Add(navmeshTile.verts[i], num);
					array[i] = num;
					navmeshTile.verts[num] = navmeshTile.verts[i];
					num++;
				}
				catch
				{
					array[i] = dictionary.get_Item(navmeshTile.verts[i]);
				}
			}
			for (int j = 0; j < navmeshTile.tris.Length; j++)
			{
				navmeshTile.tris[j] = array[navmeshTile.tris[j]];
			}
			VInt3[] array2 = new VInt3[num];
			for (int k = 0; k < num; k++)
			{
				array2[k] = navmeshTile.verts[k];
			}
			navmeshTile.verts = array2;
			TriangleMeshNode[] array3 = new TriangleMeshNode[navmeshTile.tris.Length / 3];
			navmeshTile.nodes = array3;
			int graphIndex = this.astarData.graphs.Length;
			TriangleMeshNode.SetNavmeshHolder(0, graphIndex, navmeshTile);
			int num2 = x + z * this.tileXCount;
			num2 <<= 12;
			for (int l = 0; l < array3.Length; l++)
			{
				TriangleMeshNode triangleMeshNode = new TriangleMeshNode(this.active);
				array3[l] = triangleMeshNode;
				triangleMeshNode.GraphIndex = (uint)graphIndex;
				triangleMeshNode.v0 = (navmeshTile.tris[l * 3] | num2);
				triangleMeshNode.v1 = (navmeshTile.tris[l * 3 + 1] | num2);
				triangleMeshNode.v2 = (navmeshTile.tris[l * 3 + 2] | num2);
				if (!Polygon.IsClockwise(triangleMeshNode.GetVertex(0), triangleMeshNode.GetVertex(1), triangleMeshNode.GetVertex(2)))
				{
					int v = triangleMeshNode.v0;
					triangleMeshNode.v0 = triangleMeshNode.v2;
					triangleMeshNode.v2 = v;
				}
				triangleMeshNode.Walkable = true;
				triangleMeshNode.Penalty = this.initialPenalty;
				triangleMeshNode.UpdatePositionFromVertices();
				navmeshTile.bbTree.Insert(triangleMeshNode);
			}
			this.CreateNodeConnections(navmeshTile.nodes);
			TriangleMeshNode.SetNavmeshHolder(0, graphIndex, null);
			return navmeshTile;
		}

		private void CreateNodeConnections(TriangleMeshNode[] nodes)
		{
			ListLinqView<MeshNode> listLinqView = new ListLinqView<MeshNode>();
			List<uint> list = ListPool<uint>.Claim();
			Dictionary<VInt2, int> dictionary = this.cachedInt2_int_dict;
			dictionary.Clear();
			for (int i = 0; i < nodes.Length; i++)
			{
				TriangleMeshNode triangleMeshNode = nodes[i];
				int vertexCount = triangleMeshNode.GetVertexCount();
				for (int j = 0; j < vertexCount; j++)
				{
					try
					{
						dictionary.Add(new VInt2(triangleMeshNode.GetVertexIndex(j), triangleMeshNode.GetVertexIndex((j + 1) % vertexCount)), i);
					}
					catch (Exception)
					{
					}
				}
			}
			for (int k = 0; k < nodes.Length; k++)
			{
				TriangleMeshNode triangleMeshNode2 = nodes[k];
				listLinqView.Clear();
				list.Clear();
				int vertexCount2 = triangleMeshNode2.GetVertexCount();
				for (int l = 0; l < vertexCount2; l++)
				{
					int vertexIndex = triangleMeshNode2.GetVertexIndex(l);
					int vertexIndex2 = triangleMeshNode2.GetVertexIndex((l + 1) % vertexCount2);
					int num;
					if (dictionary.TryGetValue(new VInt2(vertexIndex2, vertexIndex), ref num))
					{
						TriangleMeshNode triangleMeshNode3 = nodes[num];
						int vertexCount3 = triangleMeshNode3.GetVertexCount();
						for (int m = 0; m < vertexCount3; m++)
						{
							if (triangleMeshNode3.GetVertexIndex(m) == vertexIndex2 && triangleMeshNode3.GetVertexIndex((m + 1) % vertexCount3) == vertexIndex)
							{
								uint costMagnitude = (uint)(triangleMeshNode2.position - triangleMeshNode3.position).costMagnitude;
								listLinqView.Add(triangleMeshNode3);
								list.Add(costMagnitude);
								break;
							}
						}
					}
				}
				triangleMeshNode2.connections = listLinqView.ToArray();
				triangleMeshNode2.connectionCosts = list.ToArray();
			}
			ListPool<uint>.Release(list);
		}

		private void ConnectTiles(RecastGraph.NavmeshTile tile1, RecastGraph.NavmeshTile tile2)
		{
			if (tile1 == null)
			{
				return;
			}
			if (tile2 == null)
			{
				return;
			}
			if (tile1.nodes == null)
			{
				throw new ArgumentException("tile1 does not contain any nodes");
			}
			if (tile2.nodes == null)
			{
				throw new ArgumentException("tile2 does not contain any nodes");
			}
			int num = Mathf.Clamp(tile2.x, tile1.x, tile1.x + tile1.w - 1);
			int num2 = Mathf.Clamp(tile1.x, tile2.x, tile2.x + tile2.w - 1);
			int num3 = Mathf.Clamp(tile2.z, tile1.z, tile1.z + tile1.d - 1);
			int num4 = Mathf.Clamp(tile1.z, tile2.z, tile2.z + tile2.d - 1);
			int num5;
			int i;
			int num6;
			int num7;
			float num8;
			if (num == num2)
			{
				num5 = 2;
				i = 0;
				num6 = num3;
				num7 = num4;
				num8 = (float)this.tileSizeZ * this.cellSize;
			}
			else
			{
				if (num3 != num4)
				{
					throw new ArgumentException("Tiles are not adjacent (neither x or z coordinates match)");
				}
				num5 = 0;
				i = 2;
				num6 = num;
				num7 = num2;
				num8 = (float)this.tileSizeX * this.cellSize;
			}
			if (Math.Abs(num6 - num7) != 1)
			{
				Debug.Log(string.Concat(new object[]
				{
					tile1.x,
					" ",
					tile1.z,
					" ",
					tile1.w,
					" ",
					tile1.d,
					"\n",
					tile2.x,
					" ",
					tile2.z,
					" ",
					tile2.w,
					" ",
					tile2.d,
					"\n",
					num,
					" ",
					num3,
					" ",
					num2,
					" ",
					num4
				}));
				throw new ArgumentException(string.Concat(new object[]
				{
					"Tiles are not adjacent (tile coordinates must differ by exactly 1. Got '",
					num6,
					"' and '",
					num7,
					"')"
				}));
			}
			int num9 = MMGame_Math.RoundToInt((double)(((float)Math.Max(num6, num7) * num8 + this.forcedBounds.min[num5]) * 1000f));
			TriangleMeshNode[] nodes = tile1.nodes;
			TriangleMeshNode[] nodes2 = tile2.nodes;
			for (int j = 0; j < nodes.Length; j++)
			{
				TriangleMeshNode triangleMeshNode = nodes[j];
				int vertexCount = triangleMeshNode.GetVertexCount();
				for (int k = 0; k < vertexCount; k++)
				{
					VInt3 vertex = triangleMeshNode.GetVertex(k);
					VInt3 vertex2 = triangleMeshNode.GetVertex((k + 1) % vertexCount);
					if (Math.Abs(vertex[num5] - num9) < 2 && Math.Abs(vertex2[num5] - num9) < 2)
					{
						int num10 = Math.Min(vertex[i], vertex2[i]);
						int num11 = Math.Max(vertex[i], vertex2[i]);
						if (num10 != num11)
						{
							for (int l = 0; l < nodes2.Length; l++)
							{
								TriangleMeshNode triangleMeshNode2 = nodes2[l];
								int vertexCount2 = triangleMeshNode2.GetVertexCount();
								for (int m = 0; m < vertexCount2; m++)
								{
									VInt3 vertex3 = triangleMeshNode2.GetVertex(m);
									VInt3 vertex4 = triangleMeshNode2.GetVertex((m + 1) % vertexCount);
									if (Math.Abs(vertex3[num5] - num9) < 2 && Math.Abs(vertex4[num5] - num9) < 2)
									{
										int num12 = Math.Min(vertex3[i], vertex4[i]);
										int num13 = Math.Max(vertex3[i], vertex4[i]);
										if (num12 != num13 && num11 > num12 && num10 < num13 && ((vertex == vertex3 && vertex2 == vertex4) || (vertex == vertex4 && vertex2 == vertex3) || Polygon.DistanceSegmentSegment3D((Vector3)vertex, (Vector3)vertex2, (Vector3)vertex3, (Vector3)vertex4) < this.walkableClimb * this.walkableClimb))
										{
											uint costMagnitude = (uint)(triangleMeshNode.position - triangleMeshNode2.position).costMagnitude;
											triangleMeshNode.AddConnection(triangleMeshNode2, costMagnitude);
											triangleMeshNode2.AddConnection(triangleMeshNode, costMagnitude);
										}
									}
								}
							}
						}
					}
				}
			}
		}

		public void StartBatchTileUpdate()
		{
			if (this.batchTileUpdate)
			{
				throw new InvalidOperationException("Calling StartBatchLoad when batching is already enabled");
			}
			this.batchTileUpdate = true;
		}

		public void EndBatchTileUpdate()
		{
			if (!this.batchTileUpdate)
			{
				throw new InvalidOperationException("Calling EndBatchLoad when batching not enabled");
			}
			this.batchTileUpdate = false;
			int num = this.tileXCount;
			int num2 = this.tileZCount;
			for (int i = 0; i < num2; i++)
			{
				for (int j = 0; j < num; j++)
				{
					this.tiles[j + i * this.tileXCount].flag = false;
				}
			}
			for (int k = 0; k < this.batchUpdatedTiles.get_Count(); k++)
			{
				this.tiles[this.batchUpdatedTiles.get_Item(k)].flag = true;
			}
			for (int l = 0; l < num2; l++)
			{
				for (int m = 0; m < num; m++)
				{
					if (m < num - 1 && (this.tiles[m + l * this.tileXCount].flag || this.tiles[m + 1 + l * this.tileXCount].flag) && this.tiles[m + l * this.tileXCount] != this.tiles[m + 1 + l * this.tileXCount])
					{
						this.ConnectTiles(this.tiles[m + l * this.tileXCount], this.tiles[m + 1 + l * this.tileXCount]);
					}
					if (l < num2 - 1 && (this.tiles[m + l * this.tileXCount].flag || this.tiles[m + (l + 1) * this.tileXCount].flag) && this.tiles[m + l * this.tileXCount] != this.tiles[m + (l + 1) * this.tileXCount])
					{
						this.ConnectTiles(this.tiles[m + l * this.tileXCount], this.tiles[m + (l + 1) * this.tileXCount]);
					}
				}
			}
			this.batchUpdatedTiles.Clear();
		}

		public void ReplaceTile(int x, int z, VInt3[] verts, int[] tris, bool worldSpace)
		{
			this.ReplaceTile(x, z, 1, 1, verts, tris, worldSpace);
		}

		public void ReplaceTile(int x, int z, int w, int d, VInt3[] verts, int[] tris, bool worldSpace)
		{
			if (x + w > this.tileXCount || z + d > this.tileZCount || x < 0 || z < 0)
			{
				throw new ArgumentException(string.Concat(new object[]
				{
					"Tile is placed at an out of bounds position or extends out of the graph bounds (",
					x,
					", ",
					z,
					" [",
					w,
					", ",
					d,
					"] ",
					this.tileXCount,
					" ",
					this.tileZCount,
					")"
				}));
			}
			if (w < 1 || d < 1)
			{
				throw new ArgumentException(string.Concat(new object[]
				{
					"width and depth must be greater or equal to 1. Was ",
					w,
					", ",
					d
				}));
			}
			for (int i = z; i < z + d; i++)
			{
				for (int j = x; j < x + w; j++)
				{
					RecastGraph.NavmeshTile navmeshTile = this.tiles[j + i * this.tileXCount];
					if (navmeshTile != null)
					{
						this.RemoveConnectionsFromTile(navmeshTile);
						for (int k = 0; k < navmeshTile.nodes.Length; k++)
						{
							navmeshTile.nodes[k].Destroy();
						}
						for (int l = navmeshTile.z; l < navmeshTile.z + navmeshTile.d; l++)
						{
							for (int m = navmeshTile.x; m < navmeshTile.x + navmeshTile.w; m++)
							{
								RecastGraph.NavmeshTile navmeshTile2 = this.tiles[m + l * this.tileXCount];
								if (navmeshTile2 == null || navmeshTile2 != navmeshTile)
								{
									throw new Exception("This should not happen");
								}
								if (l < z || l >= z + d || m < x || m >= x + w)
								{
									this.tiles[m + l * this.tileXCount] = RecastGraph.NewEmptyTile(m, l);
									if (this.batchTileUpdate)
									{
										this.batchUpdatedTiles.Add(m + l * this.tileXCount);
									}
								}
								else
								{
									this.tiles[m + l * this.tileXCount] = null;
								}
							}
						}
					}
				}
			}
			RecastGraph.NavmeshTile navmeshTile3 = new RecastGraph.NavmeshTile();
			navmeshTile3.x = x;
			navmeshTile3.z = z;
			navmeshTile3.w = w;
			navmeshTile3.d = d;
			navmeshTile3.tris = tris;
			navmeshTile3.verts = verts;
			navmeshTile3.bbTree = new BBTree(navmeshTile3);
			if (navmeshTile3.tris.Length % 3 != 0)
			{
				throw new ArgumentException("Triangle array's length must be a multiple of 3 (tris)");
			}
			if (navmeshTile3.verts.Length > 65535)
			{
				throw new ArgumentException("Too many vertices per tile (more than 65535)");
			}
			if (!worldSpace)
			{
				if (!Mathf.Approximately((float)(x * this.tileSizeX) * this.cellSize * 1000f, (float)MMGame_Math.RoundToInt((double)((float)(x * this.tileSizeX) * this.cellSize * 1000f))))
				{
					Debug.LogWarning("Possible numerical imprecision. Consider adjusting tileSize and/or cellSize");
				}
				if (!Mathf.Approximately((float)(z * this.tileSizeZ) * this.cellSize * 1000f, (float)MMGame_Math.RoundToInt((double)((float)(z * this.tileSizeZ) * this.cellSize * 1000f))))
				{
					Debug.LogWarning("Possible numerical imprecision. Consider adjusting tileSize and/or cellSize");
				}
				VInt3 rhs = (VInt3)(new Vector3((float)(x * this.tileSizeX) * this.cellSize, 0f, (float)(z * this.tileSizeZ) * this.cellSize) + this.forcedBounds.min);
				for (int n = 0; n < verts.Length; n++)
				{
					verts[n] += rhs;
				}
			}
			TriangleMeshNode[] array = new TriangleMeshNode[navmeshTile3.tris.Length / 3];
			navmeshTile3.nodes = array;
			int graphIndex = this.astarData.graphs.Length;
			int dataGroupIndex = this.astarData.DataGroupIndex;
			TriangleMeshNode.SetNavmeshHolder(dataGroupIndex, graphIndex, navmeshTile3);
			int num = x + z * this.tileXCount;
			num <<= 12;
			for (int num2 = 0; num2 < array.Length; num2++)
			{
				TriangleMeshNode triangleMeshNode = new TriangleMeshNode(this.active);
				array[num2] = triangleMeshNode;
				triangleMeshNode.GraphIndex = (uint)graphIndex;
				triangleMeshNode.DataGroupIndex = dataGroupIndex;
				triangleMeshNode.v0 = (navmeshTile3.tris[num2 * 3] | num);
				triangleMeshNode.v1 = (navmeshTile3.tris[num2 * 3 + 1] | num);
				triangleMeshNode.v2 = (navmeshTile3.tris[num2 * 3 + 2] | num);
				if (!Polygon.IsClockwise(triangleMeshNode.GetVertex(0), triangleMeshNode.GetVertex(1), triangleMeshNode.GetVertex(2)))
				{
					int v = triangleMeshNode.v0;
					triangleMeshNode.v0 = triangleMeshNode.v2;
					triangleMeshNode.v2 = v;
				}
				triangleMeshNode.Walkable = true;
				triangleMeshNode.Penalty = this.initialPenalty;
				triangleMeshNode.UpdatePositionFromVertices();
				navmeshTile3.bbTree.Insert(triangleMeshNode);
			}
			this.CreateNodeConnections(navmeshTile3.nodes);
			for (int num3 = z; num3 < z + d; num3++)
			{
				for (int num4 = x; num4 < x + w; num4++)
				{
					this.tiles[num4 + num3 * this.tileXCount] = navmeshTile3;
				}
			}
			if (this.batchTileUpdate)
			{
				this.batchUpdatedTiles.Add(x + z * this.tileXCount);
			}
			else
			{
				this.ConnectTileWithNeighbours(navmeshTile3);
			}
			TriangleMeshNode.SetNavmeshHolder(dataGroupIndex, graphIndex, null);
			graphIndex = this.astarData.GetGraphIndex(this);
			for (int num5 = 0; num5 < array.Length; num5++)
			{
				array[num5].GraphIndex = (uint)graphIndex;
			}
		}

		protected void ScanCRecast()
		{
			Debug.LogError("The C++ version of recast can only be used in osx editor or osx standalone mode, I'm sure it cannot be used in the webplayer, but other platforms are not tested yet\nIf you are in the Unity Editor, try switching Platform to OSX Standalone just when scanning, scanned graphs can be cached to enable them to be used in a webplayer.");
		}

		private void CollectTreeMeshes(Terrain terrain, List<ExtraMesh> extraMeshes)
		{
			TerrainData terrainData = terrain.get_terrainData();
			for (int i = 0; i < terrainData.get_treeInstances().Length; i++)
			{
				TreeInstance treeInstance = terrainData.get_treeInstances()[i];
				TreePrototype treePrototype = terrainData.get_treePrototypes()[treeInstance.get_prototypeIndex()];
				if (treePrototype.get_prefab().GetComponent<Collider>() == null)
				{
					Bounds b = new Bounds(terrain.transform.position + Vector3.Scale(treeInstance.get_position(), terrainData.get_size()), new Vector3(treeInstance.get_widthScale(), treeInstance.get_heightScale(), treeInstance.get_widthScale()));
					Matrix4x4 matrix = Matrix4x4.TRS(terrain.transform.position + Vector3.Scale(treeInstance.get_position(), terrainData.get_size()), Quaternion.identity, new Vector3(treeInstance.get_widthScale(), treeInstance.get_heightScale(), treeInstance.get_widthScale()) * 0.5f);
					ExtraMesh extraMesh = new ExtraMesh(this.BoxColliderVerts, this.BoxColliderTris, b, matrix);
					extraMeshes.Add(extraMesh);
				}
				else
				{
					Vector3 pos = terrain.transform.position + Vector3.Scale(treeInstance.get_position(), terrainData.get_size());
					Vector3 s = new Vector3(treeInstance.get_widthScale(), treeInstance.get_heightScale(), treeInstance.get_widthScale());
					ExtraMesh extraMesh2 = this.RasterizeCollider(treePrototype.get_prefab().GetComponent<Collider>(), Matrix4x4.TRS(pos, Quaternion.identity, s));
					if (extraMesh2.vertices != null)
					{
						extraMesh2.RecalculateBounds();
						extraMeshes.Add(extraMesh2);
					}
				}
			}
		}

		private void CollectTerrainMeshes(Bounds bounds, bool rasterizeTrees, List<ExtraMesh> extraMeshes)
		{
			Terrain[] array = Object.FindObjectsOfType(typeof(Terrain)) as Terrain[];
			if (array.Length > 0)
			{
				for (int i = 0; i < array.Length; i++)
				{
					TerrainData terrainData = array[i].get_terrainData();
					if (!(terrainData == null))
					{
						Vector3 position = array[i].GetPosition();
						Vector3 center = position + terrainData.get_size() * 0.5f;
						Bounds b = new Bounds(center, terrainData.get_size());
						if (b.Intersects(bounds))
						{
							float[,] heights = terrainData.GetHeights(0, 0, terrainData.get_heightmapWidth(), terrainData.get_heightmapHeight());
							this.terrainSampleSize = ((this.terrainSampleSize < 1) ? 1 : this.terrainSampleSize);
							int heightmapWidth = terrainData.get_heightmapWidth();
							int heightmapHeight = terrainData.get_heightmapHeight();
							int num = (terrainData.get_heightmapWidth() + this.terrainSampleSize - 1) / this.terrainSampleSize + 1;
							int num2 = (terrainData.get_heightmapHeight() + this.terrainSampleSize - 1) / this.terrainSampleSize + 1;
							Vector3[] array2 = new Vector3[num * num2];
							Vector3 heightmapScale = terrainData.get_heightmapScale();
							float y = terrainData.get_size().y;
							int num3 = 0;
							for (int j = 0; j < num2; j++)
							{
								int num4 = 0;
								for (int k = 0; k < num; k++)
								{
									int num5 = Math.Min(num4, heightmapWidth - 1);
									int num6 = Math.Min(num3, heightmapHeight - 1);
									array2[j * num + k] = new Vector3((float)num6 * heightmapScale.z, heights[num5, num6] * y, (float)num5 * heightmapScale.x) + position;
									num4 += this.terrainSampleSize;
								}
								num3 += this.terrainSampleSize;
							}
							int[] array3 = new int[(num - 1) * (num2 - 1) * 2 * 3];
							int num7 = 0;
							for (int l = 0; l < num2 - 1; l++)
							{
								for (int m = 0; m < num - 1; m++)
								{
									array3[num7] = l * num + m;
									array3[num7 + 1] = l * num + m + 1;
									array3[num7 + 2] = (l + 1) * num + m + 1;
									num7 += 3;
									array3[num7] = l * num + m;
									array3[num7 + 1] = (l + 1) * num + m + 1;
									array3[num7 + 2] = (l + 1) * num + m;
									num7 += 3;
								}
							}
							extraMeshes.Add(new ExtraMesh(array2, array3, b));
							if (rasterizeTrees)
							{
								this.CollectTreeMeshes(array[i], extraMeshes);
							}
						}
					}
				}
			}
		}

		private void CollectColliderMeshes(Bounds bounds, List<ExtraMesh> extraMeshes)
		{
			Collider[] array = Object.FindObjectsOfType(typeof(Collider)) as Collider[];
			for (int i = 0; i < array.Length; i++)
			{
				Collider collider = array[i];
				if ((1 << collider.gameObject.layer & this.mask) != 0 && collider.get_enabled() && !collider.isTrigger && collider.bounds.Intersects(bounds))
				{
					ExtraMesh extraMesh = this.RasterizeCollider(collider);
					if (extraMesh.vertices != null)
					{
						extraMeshes.Add(extraMesh);
					}
				}
			}
			this.capsuleCache.Clear();
		}

		private bool CollectMeshes(out List<ExtraMesh> extraMeshes, Bounds bounds)
		{
			extraMeshes = new List<ExtraMesh>();
			if (this.rasterizeMeshes)
			{
				RecastGraph.GetSceneMeshes(bounds, this.tagMask, this.mask, extraMeshes);
			}
			this.GetRecastMeshObjs(bounds, extraMeshes);
			if (this.rasterizeTerrain)
			{
				this.CollectTerrainMeshes(bounds, this.rasterizeTrees, extraMeshes);
			}
			if (this.rasterizeColliders)
			{
				this.CollectColliderMeshes(bounds, extraMeshes);
			}
			if (extraMeshes.get_Count() == 0)
			{
				Debug.LogWarning("No MeshFilters where found contained in the layers specified by the 'mask' variables");
				return false;
			}
			return true;
		}

		private ExtraMesh RasterizeCollider(Collider col)
		{
			return this.RasterizeCollider(col, col.transform.localToWorldMatrix);
		}

		private ExtraMesh RasterizeCollider(Collider col, Matrix4x4 localToWorldMatrix)
		{
			if (col is BoxCollider)
			{
				BoxCollider boxCollider = col as BoxCollider;
				Matrix4x4 matrix4x = Matrix4x4.TRS(boxCollider.center, Quaternion.identity, boxCollider.size * 0.5f);
				matrix4x = localToWorldMatrix * matrix4x;
				Bounds bounds = boxCollider.bounds;
				ExtraMesh result = new ExtraMesh(this.BoxColliderVerts, this.BoxColliderTris, bounds, matrix4x);
				return result;
			}
			if (col is SphereCollider || col is CapsuleCollider)
			{
				SphereCollider sphereCollider = col as SphereCollider;
				CapsuleCollider capsuleCollider = col as CapsuleCollider;
				float num = (sphereCollider != null) ? sphereCollider.radius : capsuleCollider.radius;
				float num2 = (sphereCollider != null) ? 0f : (capsuleCollider.height * 0.5f / num - 1f);
				Matrix4x4 matrix4x2 = Matrix4x4.TRS((sphereCollider != null) ? sphereCollider.center : capsuleCollider.center, Quaternion.identity, Vector3.one * num);
				matrix4x2 = localToWorldMatrix * matrix4x2;
				int num3 = Mathf.Max(4, Mathf.RoundToInt(this.colliderRasterizeDetail * Mathf.Sqrt(matrix4x2.MultiplyVector(Vector3.one).magnitude)));
				if (num3 > 100)
				{
					Debug.LogWarning("Very large detail for some collider meshes. Consider decreasing Collider Rasterize Detail (RecastGraph)");
				}
				int num4 = num3;
				RecastGraph.CapsuleCache capsuleCache = null;
				for (int i = 0; i < this.capsuleCache.get_Count(); i++)
				{
					RecastGraph.CapsuleCache capsuleCache2 = this.capsuleCache.get_Item(i);
					if (capsuleCache2.rows == num3 && Mathf.Approximately(capsuleCache2.height, num2))
					{
						capsuleCache = capsuleCache2;
					}
				}
				Vector3[] array;
				if (capsuleCache == null)
				{
					array = new Vector3[num3 * num4 + 2];
					List<int> list = new List<int>();
					array[array.Length - 1] = Vector3.up;
					for (int j = 0; j < num3; j++)
					{
						for (int k = 0; k < num4; k++)
						{
							array[k + j * num4] = new Vector3(Mathf.Cos((float)k * 3.14159274f * 2f / (float)num4) * Mathf.Sin((float)j * 3.14159274f / (float)(num3 - 1)), Mathf.Cos((float)j * 3.14159274f / (float)(num3 - 1)) + ((j < num3 / 2) ? num2 : (-num2)), Mathf.Sin((float)k * 3.14159274f * 2f / (float)num4) * Mathf.Sin((float)j * 3.14159274f / (float)(num3 - 1)));
						}
					}
					array[array.Length - 2] = Vector3.down;
					int l = 0;
					int num5 = num4 - 1;
					while (l < num4)
					{
						list.Add(array.Length - 1);
						list.Add(0 * num4 + num5);
						list.Add(0 * num4 + l);
						num5 = l++;
					}
					for (int m = 1; m < num3; m++)
					{
						int n = 0;
						int num6 = num4 - 1;
						while (n < num4)
						{
							list.Add(m * num4 + n);
							list.Add(m * num4 + num6);
							list.Add((m - 1) * num4 + n);
							list.Add((m - 1) * num4 + num6);
							list.Add((m - 1) * num4 + n);
							list.Add(m * num4 + num6);
							num6 = n++;
						}
					}
					int num7 = 0;
					int num8 = num4 - 1;
					while (num7 < num4)
					{
						list.Add(array.Length - 2);
						list.Add((num3 - 1) * num4 + num8);
						list.Add((num3 - 1) * num4 + num7);
						num8 = num7++;
					}
					capsuleCache = new RecastGraph.CapsuleCache();
					capsuleCache.rows = num3;
					capsuleCache.height = num2;
					capsuleCache.verts = array;
					capsuleCache.tris = list.ToArray();
					this.capsuleCache.Add(capsuleCache);
				}
				array = capsuleCache.verts;
				int[] tris = capsuleCache.tris;
				Bounds bounds2 = col.bounds;
				ExtraMesh result2 = new ExtraMesh(array, tris, bounds2, matrix4x2);
				return result2;
			}
			if (col is MeshCollider)
			{
				MeshCollider meshCollider = col as MeshCollider;
				if (meshCollider.get_sharedMesh() != null)
				{
					ExtraMesh result3 = new ExtraMesh(meshCollider.get_sharedMesh().vertices, meshCollider.get_sharedMesh().triangles, meshCollider.bounds, localToWorldMatrix);
					return result3;
				}
			}
			return default(ExtraMesh);
		}

		public bool Linecast(VInt3 origin, VInt3 end)
		{
			return this.Linecast(origin, end, base.GetNearest(origin, NNConstraint.None).node);
		}

		public bool Linecast(VInt3 origin, VInt3 end, GraphNode hint, out GraphHitInfo hit)
		{
			return NavMeshGraph.Linecast(this, origin, end, hint, out hit, null);
		}

		public bool Linecast(VInt3 origin, VInt3 end, GraphNode hint)
		{
			GraphHitInfo graphHitInfo;
			return NavMeshGraph.Linecast(this, origin, end, hint, out graphHitInfo, null);
		}

		public bool Linecast(VInt3 tmp_origin, VInt3 tmp_end, GraphNode hint, out GraphHitInfo hit, List<GraphNode> trace)
		{
			return NavMeshGraph.Linecast(this, tmp_origin, tmp_end, hint, out hit, trace);
		}

		public override void OnDrawGizmos(bool drawNodes)
		{
			if (!drawNodes)
			{
				return;
			}
			if (this.bbTree != null)
			{
				this.bbTree.OnDrawGizmos();
			}
			Gizmos.DrawWireCube(this.forcedBounds.center, this.forcedBounds.size);
			PathHandler debugData = AstarPath.active.debugPathData;
			GraphNodeDelegateCancelable del = delegate(GraphNode _node)
			{
				TriangleMeshNode triangleMeshNode = _node as TriangleMeshNode;
				if (AstarPath.active.showSearchTree && debugData != null)
				{
					bool flag = NavGraph.InSearchTree(triangleMeshNode, AstarPath.active.debugPath);
					if (flag && this.showNodeConnections)
					{
						PathNode pathNode = debugData.GetPathNode(triangleMeshNode);
						if (pathNode.parent != null)
						{
							Gizmos.color = this.NodeColor(triangleMeshNode, debugData);
							Gizmos.DrawLine((Vector3)triangleMeshNode.position, (Vector3)debugData.GetPathNode(triangleMeshNode).parent.node.position);
						}
					}
					if (this.showMeshOutline)
					{
						Gizmos.color = this.NodeColor(triangleMeshNode, debugData);
						if (!flag)
						{
							Gizmos.color = Gizmos.get_color() * new Color(1f, 1f, 1f, 0.1f);
						}
						Gizmos.DrawLine((Vector3)triangleMeshNode.GetVertex(0), (Vector3)triangleMeshNode.GetVertex(1));
						Gizmos.DrawLine((Vector3)triangleMeshNode.GetVertex(1), (Vector3)triangleMeshNode.GetVertex(2));
						Gizmos.DrawLine((Vector3)triangleMeshNode.GetVertex(2), (Vector3)triangleMeshNode.GetVertex(0));
					}
				}
				else
				{
					if (this.showNodeConnections)
					{
						Gizmos.color = this.NodeColor(triangleMeshNode, null);
						for (int i = 0; i < triangleMeshNode.connections.Length; i++)
						{
							Gizmos.DrawLine((Vector3)triangleMeshNode.position, Vector3.Lerp((Vector3)triangleMeshNode.connections[i].position, (Vector3)triangleMeshNode.position, 0.4f));
						}
					}
					if (this.showMeshOutline)
					{
						Gizmos.color = this.NodeColor(triangleMeshNode, debugData);
						Gizmos.DrawLine((Vector3)triangleMeshNode.GetVertex(0), (Vector3)triangleMeshNode.GetVertex(1));
						Gizmos.DrawLine((Vector3)triangleMeshNode.GetVertex(1), (Vector3)triangleMeshNode.GetVertex(2));
						Gizmos.DrawLine((Vector3)triangleMeshNode.GetVertex(2), (Vector3)triangleMeshNode.GetVertex(0));
					}
				}
				return true;
			};
			this.GetNodes(del);
		}

		public override void SerializeSettings(GraphSerializationContext ctx)
		{
			base.SerializeSettings(ctx);
			ctx.writer.Write(this.characterRadius);
			ctx.writer.Write(this.contourMaxError);
			ctx.writer.Write(this.cellSize);
			ctx.writer.Write(this.cellHeight);
			ctx.writer.Write(this.walkableHeight);
			ctx.writer.Write(this.maxSlope);
			ctx.writer.Write(this.maxEdgeLength);
			ctx.writer.Write(this.editorTileSize);
			ctx.writer.Write(this.tileSizeX);
			ctx.writer.Write(this.nearestSearchOnlyXZ);
			ctx.writer.Write(this.useTiles);
			ctx.writer.Write((int)this.relevantGraphSurfaceMode);
			ctx.writer.Write(this.rasterizeColliders);
			ctx.writer.Write(this.rasterizeMeshes);
			ctx.writer.Write(this.rasterizeTerrain);
			ctx.writer.Write(this.rasterizeTrees);
			ctx.writer.Write(this.colliderRasterizeDetail);
			ctx.SerializeVector3(this.forcedBoundsCenter);
			ctx.SerializeVector3(this.forcedBoundsSize);
			ctx.writer.Write(this.mask);
			ctx.writer.Write(this.tagMask.get_Count());
			for (int i = 0; i < this.tagMask.get_Count(); i++)
			{
				ctx.writer.Write(this.tagMask.get_Item(i));
			}
			ctx.writer.Write(this.showMeshOutline);
			ctx.writer.Write(this.showNodeConnections);
			ctx.writer.Write(this.terrainSampleSize);
		}

		public override void DeserializeSettings(GraphSerializationContext ctx)
		{
			base.DeserializeSettings(ctx);
			this.characterRadius = ctx.reader.ReadSingle();
			this.contourMaxError = ctx.reader.ReadSingle();
			this.cellSize = ctx.reader.ReadSingle();
			this.cellHeight = ctx.reader.ReadSingle();
			this.walkableHeight = ctx.reader.ReadSingle();
			this.maxSlope = ctx.reader.ReadSingle();
			this.maxEdgeLength = ctx.reader.ReadSingle();
			this.editorTileSize = ctx.reader.ReadInt32();
			this.tileSizeX = ctx.reader.ReadInt32();
			this.nearestSearchOnlyXZ = ctx.reader.ReadBoolean();
			this.useTiles = ctx.reader.ReadBoolean();
			this.relevantGraphSurfaceMode = (RecastGraph.RelevantGraphSurfaceMode)ctx.reader.ReadInt32();
			this.rasterizeColliders = ctx.reader.ReadBoolean();
			this.rasterizeMeshes = ctx.reader.ReadBoolean();
			this.rasterizeTerrain = ctx.reader.ReadBoolean();
			this.rasterizeTrees = ctx.reader.ReadBoolean();
			this.colliderRasterizeDetail = ctx.reader.ReadSingle();
			this.forcedBoundsCenter = ctx.DeserializeVector3();
			this.forcedBoundsSize = ctx.DeserializeVector3();
			this.mask = ctx.reader.ReadInt32();
			if (!this.useTiles)
			{
				this.tileSizeX = (int)(this.forcedBoundsSize.x / this.cellSize + 0.5f);
				this.tileSizeZ = (int)(this.forcedBoundsSize.z / this.cellSize + 0.5f);
			}
			else
			{
				this.tileSizeX = this.editorTileSize;
				this.tileSizeZ = this.editorTileSize;
			}
			int num = ctx.reader.ReadInt32();
			this.tagMask = new List<string>(num);
			for (int i = 0; i < num; i++)
			{
				this.tagMask.Add(ctx.reader.ReadString());
			}
			this.showMeshOutline = ctx.reader.ReadBoolean();
			this.showNodeConnections = ctx.reader.ReadBoolean();
			this.terrainSampleSize = ctx.reader.ReadInt32();
		}

		public override void SerializeExtraInfo(GraphSerializationContext ctx)
		{
			BinaryWriter writer = ctx.writer;
			if (this.tiles == null)
			{
				writer.Write(-1);
				return;
			}
			writer.Write(this.tileXCount);
			writer.Write(this.tileZCount);
			for (int i = 0; i < this.tileZCount; i++)
			{
				for (int j = 0; j < this.tileXCount; j++)
				{
					RecastGraph.NavmeshTile navmeshTile = this.tiles[j + i * this.tileXCount];
					if (navmeshTile == null)
					{
						throw new Exception("NULL Tile");
					}
					writer.Write(navmeshTile.x);
					writer.Write(navmeshTile.z);
					if (navmeshTile.x == j && navmeshTile.z == i)
					{
						writer.Write(navmeshTile.w);
						writer.Write(navmeshTile.d);
						writer.Write(navmeshTile.tris.Length);
						for (int k = 0; k < navmeshTile.tris.Length; k++)
						{
							writer.Write(navmeshTile.tris[k]);
						}
						writer.Write(navmeshTile.verts.Length);
						for (int l = 0; l < navmeshTile.verts.Length; l++)
						{
							writer.Write(navmeshTile.verts[l].x);
							writer.Write(navmeshTile.verts[l].y);
							writer.Write(navmeshTile.verts[l].z);
						}
						writer.Write(navmeshTile.nodes.Length);
						for (int m = 0; m < navmeshTile.nodes.Length; m++)
						{
							navmeshTile.nodes[m].SerializeNode(ctx);
						}
					}
				}
			}
		}

		public override void DeserializeExtraInfo(GraphSerializationContext ctx)
		{
			BinaryReader reader = ctx.reader;
			this.tileXCount = reader.ReadInt32();
			if (this.tileXCount < 0)
			{
				return;
			}
			this.tileZCount = reader.ReadInt32();
			this.tiles = new RecastGraph.NavmeshTile[this.tileXCount * this.tileZCount];
			TriangleMeshNode.SetNavmeshHolder(0, ctx.graphIndex, this);
			for (int i = 0; i < this.tileZCount; i++)
			{
				for (int j = 0; j < this.tileXCount; j++)
				{
					int num = j + i * this.tileXCount;
					int num2 = reader.ReadInt32();
					if (num2 < 0)
					{
						throw new Exception("Invalid tile coordinates (x < 0)");
					}
					int num3 = reader.ReadInt32();
					if (num3 < 0)
					{
						throw new Exception("Invalid tile coordinates (z < 0)");
					}
					if (num2 != j || num3 != i)
					{
						this.tiles[num] = this.tiles[num3 * this.tileXCount + num2];
					}
					else
					{
						RecastGraph.NavmeshTile navmeshTile = new RecastGraph.NavmeshTile();
						navmeshTile.x = num2;
						navmeshTile.z = num3;
						navmeshTile.w = reader.ReadInt32();
						navmeshTile.d = reader.ReadInt32();
						navmeshTile.bbTree = new BBTree(navmeshTile);
						this.tiles[num] = navmeshTile;
						int num4 = reader.ReadInt32();
						if (num4 % 3 != 0)
						{
							throw new Exception("Corrupt data. Triangle indices count must be divisable by 3. Got " + num4);
						}
						navmeshTile.tris = new int[num4];
						for (int k = 0; k < navmeshTile.tris.Length; k++)
						{
							navmeshTile.tris[k] = reader.ReadInt32();
						}
						navmeshTile.verts = new VInt3[reader.ReadInt32()];
						for (int l = 0; l < navmeshTile.verts.Length; l++)
						{
							navmeshTile.verts[l] = new VInt3(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
						}
						int num5 = reader.ReadInt32();
						navmeshTile.nodes = new TriangleMeshNode[num5];
						num <<= 12;
						for (int m = 0; m < navmeshTile.nodes.Length; m++)
						{
							TriangleMeshNode triangleMeshNode = new TriangleMeshNode(this.active);
							navmeshTile.nodes[m] = triangleMeshNode;
							triangleMeshNode.GraphIndex = (uint)ctx.graphIndex;
							triangleMeshNode.DeserializeNode(ctx);
							triangleMeshNode.v0 = (navmeshTile.tris[m * 3] | num);
							triangleMeshNode.v1 = (navmeshTile.tris[m * 3 + 1] | num);
							triangleMeshNode.v2 = (navmeshTile.tris[m * 3 + 2] | num);
							triangleMeshNode.UpdatePositionFromVertices();
							navmeshTile.bbTree.Insert(triangleMeshNode);
						}
					}
				}
			}
		}
	}
}
