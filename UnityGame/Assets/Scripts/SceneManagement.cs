using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SceneManagement : Singleton<SceneManagement>
{
	public struct Coordinate
	{
		public int X;

		public int Y;

		public int NumX;

		public int NumY;

		public bool Equals(ref SceneManagement.Coordinate r)
		{
			return this.X == r.X && this.Y == r.Y && this.NumX == r.NumX && this.NumY == r.NumY;
		}
	}

	public class Node : PooledClassObject
	{
		public SceneManagement.Coordinate coord = new SceneManagement.Coordinate
		{
			X = -1,
			Y = -1
		};

		public PoolObjHandle<ActorRoot> owner;

		public int curInvokeIdx = -1;

		public bool dirty
		{
			get;
			private set;
		}

		public bool attached
		{
			get;
			private set;
		}

		public Node()
		{
			this.bChkReset = false;
		}

		public void Attach()
		{
			if (!this.owner || this.owner.handle.shape == null || this.owner.handle.TheActorMeta.ActorType == ActorTypeDef.Invalid || this.attached)
			{
				return;
			}
			SceneManagement instance = Singleton<SceneManagement>.GetInstance();
			instance.GetCoord(ref this.coord, this.owner.handle.shape);
			instance.AddToTile(this, ref this.coord);
			this.dirty = false;
			this.attached = true;
		}

		public void Detach()
		{
			if (!this.owner || !this.attached)
			{
				return;
			}
			SceneManagement instance = Singleton<SceneManagement>.GetInstance();
			instance.RemoveFromTile(this, ref this.coord);
			instance.nodes.Remove(this);
			this.attached = false;
			this.dirty = false;
		}

		public void makeDirty()
		{
			if (this.dirty || !this.attached)
			{
				return;
			}
			SceneManagement instance = Singleton<SceneManagement>.GetInstance();
			instance.nodes.Add(this);
			this.dirty = true;
		}

		public void Update()
		{
			if (this.dirty && this.owner && this.owner.handle.shape != null)
			{
				SceneManagement instance = Singleton<SceneManagement>.instance;
				SceneManagement.Coordinate coordinate = default(SceneManagement.Coordinate);
				instance.GetCoord(ref coordinate, this.owner.handle.shape);
				if (!this.coord.Equals(ref coordinate))
				{
					instance.RemoveFromTile(this, ref this.coord);
					this.coord = coordinate;
					instance.AddToTile(this, ref this.coord);
				}
				this.dirty = false;
			}
		}

		public override void OnRelease()
		{
			this.owner.Release();
			this.coord.X = (this.coord.Y = -1);
			this.curInvokeIdx = -1;
		}
	}

	public class Tile
	{
		public List<SceneManagement.Node> nodes = new List<SceneManagement.Node>(32);
	}

	public delegate void Process(ref PoolObjHandle<ActorRoot> actor);

	public delegate bool Process_Bool(ref PoolObjHandle<ActorRoot> actor);

	public const int TileSize = 10000;

	public VInt2 TileOrigin;

	public int TileCountX = 1;

	public int TileCountY = 1;

	public int TileIndexMax_X;

	public int TileIndexMax_Y;

	public SceneManagement.Tile[] tiles;

	public List<SceneManagement.Node> nodes = new List<SceneManagement.Node>();

	private int invokeIndex;

	public override void UnInit()
	{
		base.UnInit();
		this.Clear();
	}

	public void InitScene()
	{
		AstarPath active = AstarPath.active;
		if (active == null || active.astarData == null || active.astarData.recastGraph == null)
		{
			return;
		}
		Bounds forcedBounds = active.astarData.recastGraph.forcedBounds;
		VInt3 vInt = (VInt3)forcedBounds.size;
		this.TileCountX = vInt.x / 10000 + ((vInt.x % 10000 > 0) ? 1 : 0);
		this.TileCountY = vInt.z / 10000 + ((vInt.z % 10000 > 0) ? 1 : 0);
		this.TileIndexMax_X = this.TileCountX - 1;
		this.TileIndexMax_Y = this.TileCountY - 1;
		this.TileOrigin = ((VInt3)forcedBounds.min).xz;
		this.tiles = new SceneManagement.Tile[this.TileCountX * this.TileCountY];
		for (int i = 0; i < this.tiles.Length; i++)
		{
			this.tiles[i] = new SceneManagement.Tile();
		}
		this.nodes.Clear();
	}

	public void Clear()
	{
		if (this.tiles != null)
		{
			for (int i = 0; i < this.tiles.Length; i++)
			{
				this.tiles[i].nodes.Clear();
			}
		}
		this.nodes.Clear();
		this.invokeIndex = 0;
	}

	private void RemoveFromTile(SceneManagement.Node node, ref SceneManagement.Coordinate coord)
	{
		if (coord.X == -1 || coord.Y == -1)
		{
			return;
		}
		int num = coord.Y * this.TileCountX + coord.X;
		for (int i = 0; i < coord.NumY; i++)
		{
			for (int j = 0; j < coord.NumX; j++)
			{
				this.tiles[num + j].nodes.Remove(node);
			}
			num += this.TileCountX;
		}
	}

	private void AddToTile(SceneManagement.Node node, ref SceneManagement.Coordinate coord)
	{
		int num = coord.Y * this.TileCountX + coord.X;
		for (int i = 0; i < coord.NumY; i++)
		{
			for (int j = 0; j < coord.NumX; j++)
			{
				this.tiles[num + j].nodes.Add(node);
			}
			num += this.TileCountX;
		}
	}

	public void GetCoord(ref SceneManagement.Coordinate coord, VCollisionShape shape)
	{
		VInt2 origin;
		VInt2 size;
		shape.GetAabb2D(out origin, out size);
		this.GetCoord(ref coord, origin, size);
	}

	public void GetCoord_Center(ref SceneManagement.Coordinate coord, VInt2 origin, int radius)
	{
		origin.x -= radius + this.TileOrigin.x;
		origin.y -= radius + this.TileOrigin.y;
		int num = radius << 1;
		coord.X = Mathf.Clamp(origin.x / 10000, 0, this.TileIndexMax_X);
		coord.Y = Mathf.Clamp(origin.y / 10000, 0, this.TileIndexMax_Y);
		coord.NumX = Mathf.Min((origin.x - coord.X * 10000 + num) / 10000 + 1, this.TileCountX - coord.X);
		coord.NumY = Mathf.Min((origin.y - coord.Y * 10000 + num) / 10000 + 1, this.TileCountY - coord.Y);
	}

	public void GetCoord(ref SceneManagement.Coordinate coord, VInt2 origin, VInt2 size)
	{
		origin -= this.TileOrigin;
		coord.X = Mathf.Clamp(origin.x / 10000, 0, this.TileIndexMax_X);
		coord.Y = Mathf.Clamp(origin.y / 10000, 0, this.TileIndexMax_Y);
		coord.NumX = Mathf.Min((origin.x - coord.X * 10000 + size.x) / 10000 + 1, this.TileCountX - coord.X);
		coord.NumY = Mathf.Min((origin.y - coord.Y * 10000 + size.y) / 10000 + 1, this.TileCountY - coord.Y);
	}

	public void UpdateDirtyNodes()
	{
		int count = this.nodes.get_Count();
		if (count > 0)
		{
			for (int i = 0; i < count; i++)
			{
				this.nodes.get_Item(i).Update();
			}
			this.nodes.Clear();
		}
	}

	public void ForeachActors(SceneManagement.Coordinate coord, SceneManagement.Process proc)
	{
		if (coord.X == -1 || coord.Y == -1)
		{
			return;
		}
		this.invokeIndex++;
		int num = coord.Y * this.TileCountX + coord.X;
		for (int i = 0; i < coord.NumY; i++)
		{
			for (int j = 0; j < coord.NumX; j++)
			{
				List<SceneManagement.Node> list = this.tiles[num + j].nodes;
				int count = list.get_Count();
				for (int k = 0; k < count; k++)
				{
					SceneManagement.Node node = list.get_Item(k);
					if (node.owner && node.curInvokeIdx != this.invokeIndex)
					{
						node.curInvokeIdx = this.invokeIndex;
						proc(ref node.owner);
					}
				}
			}
			num += this.TileCountX;
		}
	}

	public void ForeachActorsBreak(SceneManagement.Coordinate coord, SceneManagement.Process_Bool proc)
	{
		if (coord.X == -1 || coord.Y == -1)
		{
			return;
		}
		this.invokeIndex++;
		int num = coord.Y * this.TileCountX + coord.X;
		for (int i = 0; i < coord.NumY; i++)
		{
			for (int j = 0; j < coord.NumX; j++)
			{
				List<SceneManagement.Node> list = this.tiles[num + j].nodes;
				int count = list.get_Count();
				for (int k = 0; k < count; k++)
				{
					SceneManagement.Node node = list.get_Item(k);
					if (node.owner && node.curInvokeIdx != this.invokeIndex)
					{
						node.curInvokeIdx = this.invokeIndex;
						if (!proc(ref node.owner))
						{
							return;
						}
					}
				}
			}
			num += this.TileCountX;
		}
	}
}
