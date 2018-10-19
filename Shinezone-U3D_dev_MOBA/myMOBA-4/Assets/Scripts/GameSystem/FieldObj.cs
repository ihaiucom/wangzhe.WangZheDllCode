using Assets.Scripts.GameLogic;
using CSProtocol;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameSystem
{
	public class FieldObj : MonoBehaviour
	{
		[Serializable]
		public struct FOGridInfo
		{
			[SerializeField]
			public int CellNumX;

			[SerializeField]
			public int CellNumY;

			[SerializeField]
			public int CellSizeX;

			[SerializeField]
			public int CellSizeY;

			[SerializeField]
			public VInt2 GridPos;

			public void Clear()
			{
				this.CellNumX = (this.CellNumY = 0);
				this.CellSizeX = (this.CellSizeY = 0);
				this.GridPos = VInt2.zero;
			}
		}

		[Serializable]
		public struct FOGridCell
		{
			[SerializeField]
			public byte CellX;

			[SerializeField]
			public byte CellY;

			[SerializeField]
			public byte m_viewBlockId;

			public bool QueryAttr(FieldObj inFieldObj, out FieldObj.SViewBlockAttr outAttr)
			{
				return inFieldObj.ViewBlockAttrMap.TryGetValue(this.m_viewBlockId, out outAttr);
			}
		}

		public enum EViewBlockType : byte
		{
			None,
			Grass,
			Brick
		}

		public enum EViewLightType : byte
		{
			None,
			PermanentCamp1,
			PermanentCamp2,
			PermanentForAll
		}

		[Serializable]
		public struct SViewBlockAttr
		{
			public byte BlockType;

			public byte LightType;

			public SViewBlockAttr(byte inBlockType, byte inLightType)
			{
				this.BlockType = inBlockType;
				this.LightType = inLightType;
			}
		}

		[Serializable]
		public struct SViewBlockAttrIndexed
		{
			[SerializeField]
			public byte ViewBlockId;

			[SerializeField]
			public FieldObj.SViewBlockAttr Attr;
		}

		[Serializable]
		public struct FOLevelGrid
		{
			[SerializeField]
			public FieldObj.FOGridInfo GridInfo;

			[SerializeField]
			public FieldObj.FOGridCell[] GridCells;

			public void Clear()
			{
				this.GridCells = null;
				this.GridInfo.Clear();
			}

			public void SetViewBlockId(int index, byte inViewBlockId)
			{
				this.GridCells[index].m_viewBlockId = inViewBlockId;
			}

			public FieldObj.FOGridCell GetGridCell(VInt2 inCell)
			{
				return this.GetGridCell(inCell.x, inCell.y);
			}

			public FieldObj.FOGridCell GetGridCell(int X, int Y)
			{
				return this.GridCells[this.GetGridCellIndex(X, Y)];
			}

			public int GetGridCellIndex(int X, int Y)
			{
				return Y * this.GridInfo.CellNumX + X;
			}

			public int GetGridCellX(int index)
			{
				return index % this.GridInfo.CellNumX;
			}

			public int GetGridCellY(int index)
			{
				return index / this.GridInfo.CellNumX;
			}

			public void WorldPosToGrid(VInt3 inWorldPos, out int outCellX, out int outCellY)
			{
				outCellX = Mathf.Clamp((inWorldPos.x - this.GridInfo.GridPos.x) / this.GridInfo.CellSizeX, 0, this.GridInfo.CellNumX - 1);
				outCellY = Mathf.Clamp((inWorldPos.y - this.GridInfo.GridPos.y) / this.GridInfo.CellSizeY, 0, this.GridInfo.CellNumY - 1);
			}

			public bool WorldPosToGridMayFail(VInt3 inWorldPos, out int outCellX, out int outCellY)
			{
				outCellX = (inWorldPos.x - this.GridInfo.GridPos.x) / this.GridInfo.CellSizeX;
				outCellY = (inWorldPos.y - this.GridInfo.GridPos.y) / this.GridInfo.CellSizeY;
				return outCellX >= 0 && outCellX < this.GridInfo.CellNumX && outCellY >= 0 && outCellY < this.GridInfo.CellNumY;
			}

			public void GridToWorldPos(int inCellX, int inCellY, out VInt3 outWorldPos)
			{
				inCellX = Mathf.Clamp(inCellX, 0, this.GridInfo.CellNumX - 1);
				inCellY = Mathf.Clamp(inCellY, 0, this.GridInfo.CellNumY - 1);
				outWorldPos.x = this.GridInfo.GridPos.x + (int)((float)this.GridInfo.CellSizeX * ((float)inCellX + 0.5f));
				outWorldPos.y = this.GridInfo.GridPos.y + (int)((float)this.GridInfo.CellSizeY * ((float)inCellY + 0.5f));
				outWorldPos.z = 0;
			}

			public void ReviseWorldPosToCenter(VInt3 inWorldPos, out VInt3 outWorldPos)
			{
				outWorldPos = VInt3.zero;
				int inCellY = 0;
				int inCellX;
				this.WorldPosToGrid(inWorldPos, out inCellX, out inCellY);
				this.GridToWorldPos(inCellX, inCellY, out outWorldPos);
			}
		}

		private const int INT_MIN = -2147483648;

		private const int INT_MAX = 2147483647;

		[SerializeField]
		public FieldObj.FOLevelGrid LevelGrid;

		[NonSerialized]
		public int CellScale = 1;

		[SerializeField]
		public FieldObj.SViewBlockAttrIndexed[] SerializedBlockAttrList = new FieldObj.SViewBlockAttrIndexed[0];

		[SerializeField]
		public byte[] fowOfflineData = new byte[0];

		[NonSerialized]
		public FOWSurfCell[] m_fowCells;

		public bool GrassBlockView = true;

		public Dictionary<byte, FieldObj.SViewBlockAttr> ViewBlockAttrMap = new Dictionary<byte, FieldObj.SViewBlockAttr>();

		public bool bSynced
		{
			get
			{
				return this.LevelGrid.GridCells != null && this.LevelGrid.GridCells.Length > 0;
			}
		}

		public List<FowLos.SBlockWalls> ViewBlockArrayImpl
		{
			get;
			private set;
		}

		public int NumX
		{
			get
			{
				return this.LevelGrid.GridInfo.CellNumX;
			}
		}

		public int NumY
		{
			get
			{
				return this.LevelGrid.GridInfo.CellNumY;
			}
		}

		public int PaneX
		{
			get
			{
				return this.LevelGrid.GridInfo.CellSizeX;
			}
		}

		public int PaneY
		{
			get
			{
				return this.LevelGrid.GridInfo.CellSizeY;
			}
		}

		public int FieldX
		{
			get;
			private set;
		}

		public int FieldY
		{
			get;
			private set;
		}

		public int xMin
		{
			get;
			private set;
		}

		public int xMax
		{
			get;
			private set;
		}

		public int yMin
		{
			get;
			private set;
		}

		public int yMax
		{
			get;
			private set;
		}

		public void InitField()
		{
			if (this.bSynced)
			{
				DebugHelper.Assert(this.NumX > 0);
				DebugHelper.Assert(this.NumY > 0);
				DebugHelper.Assert(this.PaneX > 0);
				DebugHelper.Assert(this.PaneY > 0);
			}
			this.FieldX = this.NumX * this.PaneX;
			this.FieldY = this.NumY * this.PaneY;
			this.xMin = this.LevelGrid.GridInfo.GridPos.x;
			this.xMax = this.LevelGrid.GridInfo.GridPos.x + this.FieldX;
			this.yMin = this.LevelGrid.GridInfo.GridPos.y;
			this.yMax = this.LevelGrid.GridInfo.GridPos.y + this.FieldY;
			this.DeserializeViewBlockAttrMap();
		}

		public void UninitField()
		{
			if (this.ViewBlockArrayImpl != null)
			{
				this.ViewBlockArrayImpl.Clear();
				this.ViewBlockArrayImpl = null;
			}
			this.m_fowCells = null;
			this.ViewBlockAttrMap.Clear();
		}

		public bool AddViewBlockAttr(byte inViewBlockId, byte inBlockType, byte inLightType)
		{
			if (!this.ViewBlockAttrMap.ContainsKey(inViewBlockId) && inViewBlockId > 0)
			{
				FieldObj.SViewBlockAttr value = default(FieldObj.SViewBlockAttr);
				value.BlockType = inBlockType;
				value.LightType = inLightType;
				this.ViewBlockAttrMap.Add(inViewBlockId, value);
				return true;
			}
			return false;
		}

		public void ClearViewBlockAttrMap()
		{
			this.ViewBlockAttrMap.Clear();
			this.SerializedBlockAttrList = new FieldObj.SViewBlockAttrIndexed[0];
		}

		public void DeserializeViewBlockAttrMap()
		{
			this.ViewBlockAttrMap.Clear();
			if (this.SerializedBlockAttrList != null && this.SerializedBlockAttrList.Length > 0)
			{
				FieldObj.SViewBlockAttrIndexed[] serializedBlockAttrList = this.SerializedBlockAttrList;
				for (int i = 0; i < serializedBlockAttrList.Length; i++)
				{
					FieldObj.SViewBlockAttrIndexed sViewBlockAttrIndexed = serializedBlockAttrList[i];
					this.AddViewBlockAttr(sViewBlockAttrIndexed.ViewBlockId, sViewBlockAttrIndexed.Attr.BlockType, sViewBlockAttrIndexed.Attr.LightType);
				}
				if (Application.isPlaying)
				{
					this.SerializedBlockAttrList = null;
				}
			}
		}

		public void SerializeViewBlockAttrMap()
		{
			if (this.ViewBlockAttrMap.Count > 0)
			{
				this.SerializedBlockAttrList = new FieldObj.SViewBlockAttrIndexed[this.ViewBlockAttrMap.Count];
				Dictionary<byte, FieldObj.SViewBlockAttr>.Enumerator enumerator = this.ViewBlockAttrMap.GetEnumerator();
				int num = 0;
				while (enumerator.MoveNext())
				{
					FieldObj.SViewBlockAttrIndexed sViewBlockAttrIndexed = default(FieldObj.SViewBlockAttrIndexed);
					KeyValuePair<byte, FieldObj.SViewBlockAttr> current = enumerator.Current;
					sViewBlockAttrIndexed.ViewBlockId = current.Key;
					KeyValuePair<byte, FieldObj.SViewBlockAttr> current2 = enumerator.Current;
					sViewBlockAttrIndexed.Attr = current2.Value;
					this.SerializedBlockAttrList[num++] = sViewBlockAttrIndexed;
				}
			}
			else
			{
				this.SerializedBlockAttrList = new FieldObj.SViewBlockAttrIndexed[0];
			}
		}

		public void SyncField(VInt2 inPos, int inFieldX, int inFieldY, int inCellSize)
		{
			int num = inFieldX / inCellSize;
			int num2 = inFieldY / inCellSize;
			DebugHelper.Assert(num <= 255);
			DebugHelper.Assert(num2 <= 255);
			this.LevelGrid.GridInfo.CellNumX = num;
			this.LevelGrid.GridInfo.CellNumY = num2;
			this.LevelGrid.GridInfo.CellSizeX = inFieldX / num;
			this.LevelGrid.GridInfo.CellSizeY = inFieldY / num2;
			this.LevelGrid.GridInfo.GridPos = inPos;
			this.LevelGrid.GridCells = new FieldObj.FOGridCell[this.LevelGrid.GridInfo.CellNumX * this.LevelGrid.GridInfo.CellNumY];
			int num3 = this.NumX * this.NumY;
			for (int i = 0; i < num3; i++)
			{
				this.LevelGrid.GridCells[i].CellX = (byte)(i % this.LevelGrid.GridInfo.CellNumX);
				this.LevelGrid.GridCells[i].CellY = (byte)(i / this.LevelGrid.GridInfo.CellNumX);
			}
			this.FieldX = this.NumX * this.PaneX;
			this.FieldY = this.NumY * this.PaneY;
			this.xMin = this.LevelGrid.GridInfo.GridPos.x;
			this.xMax = this.LevelGrid.GridInfo.GridPos.x + this.FieldX;
			this.yMin = this.LevelGrid.GridInfo.GridPos.y;
			this.yMax = this.LevelGrid.GridInfo.GridPos.y + this.FieldY;
		}

		public bool QueryAttr(int x, int y, out FieldObj.SViewBlockAttr outAttr)
		{
			return this.LevelGrid.GetGridCell(x, y).QueryAttr(this, out outAttr);
		}

		public bool QueryAttr(VInt2 inCell, out FieldObj.SViewBlockAttr outAttr)
		{
			return this.LevelGrid.GetGridCell(inCell).QueryAttr(this, out outAttr);
		}

		public byte QueryViewblockId(int x, int y)
		{
			return this.LevelGrid.GetGridCell(x, y).m_viewBlockId;
		}

		public bool IsAreaPermanentLit(int x, int y, COM_PLAYERCAMP inCamp)
		{
			VInt2 inCell = new VInt2(x, y);
			FieldObj.SViewBlockAttr sViewBlockAttr = default(FieldObj.SViewBlockAttr);
			if (this.QueryAttr(inCell, out sViewBlockAttr))
			{
				if (sViewBlockAttr.LightType == 3)
				{
					return true;
				}
				if (sViewBlockAttr.LightType == 1 && inCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
				{
					return true;
				}
				if (sViewBlockAttr.LightType == 2 && inCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_2)
				{
					return true;
				}
			}
			return false;
		}

		public byte QueryViewblockId(VInt2 inCell)
		{
			return this.LevelGrid.GetGridCell(inCell.x, inCell.y).m_viewBlockId;
		}

		public void Clear()
		{
			if (this.ViewBlockArrayImpl != null)
			{
				this.ViewBlockArrayImpl.Clear();
				this.ViewBlockArrayImpl = null;
			}
			int num = 0;
			this.FieldY = num;
			this.FieldX = num;
			num = 0;
			this.yMax = num;
			num = num;
			this.yMin = num;
			num = num;
			this.xMax = num;
			this.xMin = num;
			this.LevelGrid.Clear();
		}

		public void DeserializeMapIfNotYet()
		{
			if (this.SerializedBlockAttrList != null && this.SerializedBlockAttrList.Length > 0 && this.ViewBlockAttrMap.Count == 0)
			{
				this.DeserializeViewBlockAttrMap();
			}
		}

		private void CollectNeighbourGrids(List<VInt2> inNeighbourGrids, List<VInt2> inSearchedGrids, VInt2 inCenterCell, int inThickness)
		{
			int x = inCenterCell.x;
			int y = inCenterCell.y;
			int num = Mathf.Max(x - inThickness, 0);
			int num2 = Mathf.Min(x + inThickness, this.NumX - 1);
			int num3 = Mathf.Max(y - inThickness, 0);
			int num4 = Mathf.Min(y + inThickness, this.NumY - 1);
			for (int i = num; i <= num2; i++)
			{
				for (int j = num3; j <= num4; j++)
				{
					VInt2 vInt = new VInt2(i, j);
					if (!inSearchedGrids.Contains(vInt) && vInt != inCenterCell)
					{
						inNeighbourGrids.Add(vInt);
					}
				}
			}
		}

		public bool FindNearestNotBrickFromWorldLocNonFow(ref VInt3 newPos, ActorRoot ar)
		{
			VInt3 vInt = new VInt3(newPos.x, newPos.z, 0);
			VInt2 vInt2 = VInt2.zero;
			this.LevelGrid.WorldPosToGrid(vInt, out vInt2.x, out vInt2.y);
			bool flag = false;
			FieldObj.SViewBlockAttr sViewBlockAttr = default(FieldObj.SViewBlockAttr);
			if (this.QueryAttr(vInt2, out sViewBlockAttr) && sViewBlockAttr.BlockType == 2)
			{
				flag = true;
			}
			else if (!PathfindingUtility.IsValidTarget(ar, newPos))
			{
				flag = true;
			}
			if (!flag)
			{
				return true;
			}
			VInt2 zero = VInt2.zero;
			if (this.FindNearestGrid(vInt2, vInt, FieldObj.EViewBlockType.Brick, true, 4, ar, out zero))
			{
				vInt2 = zero;
				VInt3 zero2 = VInt3.zero;
				this.LevelGrid.GridToWorldPos(vInt2.x, vInt2.y, out zero2);
				newPos = new VInt3(zero2.x, newPos.y, zero2.y);
				return true;
			}
			return false;
		}

		public bool FindNearestGrid(VInt2 inCenterCell, VInt3 inCenterPosWorld, FieldObj.EViewBlockType inBlockType, bool bNonType, int inThicknessMax, ActorRoot ar, out VInt2 result)
		{
			result = VInt2.zero;
			FieldObj.SViewBlockAttr sViewBlockAttr = default(FieldObj.SViewBlockAttr);
			bool flag = false;
			int num = 1;
			List<VInt2> list = new List<VInt2>();
			List<VInt2> list2 = new List<VInt2>();
			while (!flag && num <= inThicknessMax)
			{
				list2.Clear();
				this.CollectNeighbourGrids(list2, list, inCenterCell, num);
				num++;
				list.AddRange(list2);
				if (list2.Count > 0)
				{
					for (int i = list2.Count - 1; i >= 0; i--)
					{
						this.QueryAttr(list2[i], out sViewBlockAttr);
						if ((bNonType && sViewBlockAttr.BlockType == (byte)inBlockType) || (!bNonType && sViewBlockAttr.BlockType != (byte)inBlockType))
						{
							list2.RemoveAt(i);
						}
					}
				}
				if (list2.Count > 0)
				{
					long num2 = 2147483647L;
					for (int j = 0; j < list2.Count; j++)
					{
						VInt2 vInt = list2[j];
						VInt3 zero = VInt3.zero;
						this.LevelGrid.GridToWorldPos(vInt.x, vInt.y, out zero);
						long sqrMagnitudeLong = (inCenterPosWorld - zero).sqrMagnitudeLong;
						if (sqrMagnitudeLong < num2)
						{
							if (ar == null)
							{
								flag = true;
								num2 = sqrMagnitudeLong;
								result = list2[j];
							}
							else
							{
								VInt3 target = new VInt3(zero.x, 0, zero.y);
								if (PathfindingUtility.IsValidTarget(ar, target))
								{
									flag = true;
									num2 = sqrMagnitudeLong;
									result = list2[j];
								}
							}
						}
					}
				}
			}
			return flag;
		}

		public int GetSurfaceCellX()
		{
			return this.PaneX;
		}

		public int GetSurfaceCellY()
		{
			return this.PaneY;
		}

		public void GridToUnrealX(int gridUnits, out int unrealUnits)
		{
			unrealUnits = gridUnits * this.PaneX;
		}

		public void GridToUnrealY(int gridUnits, out int unrealUnits)
		{
			unrealUnits = gridUnits * this.PaneY;
		}

		public void UnrealToGridX(int unrealUnits, out int gridUnits)
		{
			DebugHelper.Assert(this.PaneX > 0);
			gridUnits = unrealUnits / this.PaneX;
		}

		public void UnrealToGridY(int unrealUnits, out int gridUnits)
		{
			DebugHelper.Assert(this.PaneY > 0);
			gridUnits = unrealUnits / this.PaneY;
		}

		public bool IsCellViewBlocking(int surfCellX, int surfCellY, int unitAreadId)
		{
			int num = surfCellY * this.LevelGrid.GridInfo.CellNumX + surfCellX;
			FieldObj.FOGridCell fOGridCell = this.LevelGrid.GridCells[num];
			return fOGridCell.m_viewBlockId != 0 && (unitAreadId == 0 || unitAreadId != (int)fOGridCell.m_viewBlockId) && this.IsAreaViewBlocking(fOGridCell.m_viewBlockId);
		}

		private bool IsAreaViewBlocking(byte areaId)
		{
			return areaId != 0 && this.ViewBlockAttrMap[areaId].BlockType > 0;
		}

		public void CreateBlockWalls()
		{
			this.ViewBlockArrayImpl = this.CreateBlockWallsInternal();
		}

		private List<FowLos.SBlockWalls> CreateBlockWallsInternal()
		{
			List<FowLos.SBlockWalls> list = new List<FowLos.SBlockWalls>();
			Dictionary<byte, FowLos.SBlockContext> dictionary = this.CreateBlockMap();
			Dictionary<byte, FowLos.SBlockContext>.Enumerator enumerator = dictionary.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<byte, FowLos.SBlockContext> current = enumerator.Current;
				FowLos.SBlockContext value = current.Value;
				KeyValuePair<byte, FowLos.SBlockContext> current2 = enumerator.Current;
				byte key = current2.Key;
				FowLos.SBlockWalls item = default(FowLos.SBlockWalls);
				item.ParamlessConstruct();
				item.m_areaId = key;
				item.m_xMin = value.m_xMin;
				item.m_xMax = value.m_xMax;
				item.m_yMin = value.m_yMin;
				item.m_yMax = value.m_yMax;
				KeyValuePair<byte, FowLos.SBlockContext> current3 = enumerator.Current;
				List<FowLos.SPolylineSegment> polylineSegList = current3.Value.m_polylineSegList;
				int count = polylineSegList.Count;
				for (int i = 0; i < count; i++)
				{
					FowLos.SGridWall item2 = default(FowLos.SGridWall);
					item2.m_start = polylineSegList[i].m_start;
					item2.m_end = polylineSegList[i].m_end;
					DebugHelper.Assert(item2.m_start != item2.m_end);
					if (item2.m_start.x == item2.m_end.x)
					{
						item2.m_horizontal = false;
						byte b = (byte)((item2.m_start.x + this.PaneX / 2 - this.LevelGrid.GridInfo.GridPos.x) / this.PaneX);
						DebugHelper.Assert(b >= 0);
						DebugHelper.Assert((int)b <= this.NumX);
						if (item2.m_start.y > item2.m_end.y)
						{
							item2.m_start.y = item2.m_start.y + 10;
							item2.m_end.y = item2.m_end.y - 10;
						}
						else
						{
							DebugHelper.Assert(item2.m_start.y < item2.m_end.y);
							item2.m_start.y = item2.m_start.y - 10;
							item2.m_end.y = item2.m_end.y + 10;
						}
						List<FowLos.SGridWall> list2 = null;
						if (item.m_wallsVertical.TryGetValue(b, out list2))
						{
							item.m_wallsVertical[b].Add(item2);
						}
						else
						{
							list2 = new List<FowLos.SGridWall>();
							list2.Add(item2);
							item.m_wallsVertical.Add(b, list2);
						}
					}
					else
					{
						DebugHelper.Assert(item2.m_start.y == item2.m_end.y);
						item2.m_horizontal = true;
						byte b2 = (byte)((item2.m_start.y + this.PaneY / 2 - this.LevelGrid.GridInfo.GridPos.y) / this.PaneY);
						DebugHelper.Assert(b2 >= 0);
						DebugHelper.Assert((int)b2 <= this.NumY);
						if (item2.m_start.x > item2.m_end.x)
						{
							item2.m_start.x = item2.m_start.x + 10;
							item2.m_end.x = item2.m_end.x - 10;
						}
						else
						{
							DebugHelper.Assert(item2.m_start.x < item2.m_end.x);
							item2.m_start.x = item2.m_start.x - 10;
							item2.m_end.x = item2.m_end.x + 10;
						}
						List<FowLos.SGridWall> list3 = null;
						if (item.m_wallsHorizontal.TryGetValue(b2, out list3))
						{
							item.m_wallsHorizontal[b2].Add(item2);
						}
						else
						{
							list3 = new List<FowLos.SGridWall>();
							list3.Add(item2);
							item.m_wallsHorizontal.Add(b2, list3);
						}
					}
				}
				list.Add(item);
			}
			return list;
		}

		private Dictionary<byte, FowLos.SBlockContext> CreateBlockMap()
		{
			Dictionary<byte, FowLos.SBlockContext> dictionary = new Dictionary<byte, FowLos.SBlockContext>();
			FieldObj.FOGridInfo gridInfo = this.LevelGrid.GridInfo;
			int cellNumX = gridInfo.CellNumX;
			int cellNumY = gridInfo.CellNumY;
			int num = cellNumX * cellNumY;
			Dictionary<byte, List<int>> dictionary2 = new Dictionary<byte, List<int>>();
			for (int i = 0; i < num; i++)
			{
				FieldObj.FOGridCell fOGridCell = this.LevelGrid.GridCells[i];
				byte viewBlockId = fOGridCell.m_viewBlockId;
				if (this.IsAreaViewBlocking(viewBlockId))
				{
					List<int> list = null;
					if (dictionary2.TryGetValue(viewBlockId, out list))
					{
						list.Add(i);
					}
					else
					{
						list = new List<int>();
						list.Add(i);
						dictionary2.Add(viewBlockId, list);
					}
				}
			}
			Dictionary<byte, List<int>>.Enumerator enumerator = dictionary2.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<byte, List<int>> current = enumerator.Current;
				List<int> value = current.Value;
				KeyValuePair<byte, List<int>> current2 = enumerator.Current;
				byte key = current2.Key;
				List<int> list2 = new List<int>();
				int num2 = 2147483647;
				int num3 = 2147483647;
				int num4 = -2147483647;
				int num5 = -2147483647;
				int count = value.Count;
				for (int j = 0; j < count; j++)
				{
					int num6 = value[j];
					FieldObj.FOGridCell fOGridCell2 = this.LevelGrid.GridCells[num6];
					int cellX = (int)fOGridCell2.CellX;
					int cellY = (int)fOGridCell2.CellY;
					if (num2 > cellX)
					{
						num2 = cellX;
					}
					if (num4 < cellX)
					{
						num4 = cellX;
					}
					if (num3 > cellY)
					{
						num3 = cellY;
					}
					if (num5 < cellY)
					{
						num5 = cellY;
					}
					List<int> list3 = new List<int>();
					bool flag = false;
					if (cellX > 0)
					{
						list3.Add(num6 - 1);
					}
					else
					{
						list2.Add(num6);
						flag = true;
					}
					if (!flag)
					{
						if (cellX < cellNumX - 1)
						{
							list3.Add(num6 + 1);
						}
						else
						{
							list2.Add(num6);
							flag = true;
						}
						if (!flag)
						{
							if (cellY > 0)
							{
								list3.Add(num6 - cellNumX);
							}
							else
							{
								list2.Add(num6);
								flag = true;
							}
							if (!flag)
							{
								if (cellY < cellNumY - 1)
								{
									list3.Add(num6 + cellNumX);
								}
								else
								{
									list2.Add(num6);
									flag = true;
								}
								if (!flag)
								{
									int count2 = list3.Count;
									for (int k = 0; k < count2; k++)
									{
										int item = list3[k];
										if (!value.Contains(item))
										{
											list2.Add(num6);
											break;
										}
									}
								}
							}
						}
					}
				}
				FowLos.SBlockContext value2 = default(FowLos.SBlockContext);
				value2.ParamlessConstruct();
				value2.m_areaId = (int)key;
				int num7 = gridInfo.GridPos.x + gridInfo.CellSizeX * (num4 + 1);
				int num8 = gridInfo.GridPos.y + gridInfo.CellSizeY * (num5 + 1);
				int num9 = gridInfo.GridPos.x + gridInfo.CellSizeX * num2;
				int num10 = gridInfo.GridPos.y + gridInfo.CellSizeY * num3;
				value2.m_bounds.Origin.x = (num7 + num9) / 2;
				value2.m_bounds.Origin.y = (num8 + num10) / 2;
				value2.m_bounds.Origin.z = 0;
				value2.m_bounds.BoxExtent.x = (num7 - num9) / 2;
				value2.m_bounds.BoxExtent.y = (num8 - num10) / 2;
				value2.m_bounds.BoxExtent.z = 1000;
				DebugHelper.Assert(num2 <= 255);
				DebugHelper.Assert(num4 <= 255);
				DebugHelper.Assert(num3 <= 255);
				DebugHelper.Assert(num5 <= 255);
				value2.m_xMin = (byte)num2;
				value2.m_xMax = (byte)num4;
				value2.m_yMin = (byte)num3;
				value2.m_yMax = (byte)num5;
				int count3 = list2.Count;
				List<FowLos.SPolylineSegment> list4 = new List<FowLos.SPolylineSegment>();
				for (int l = num3; l <= num5; l++)
				{
					Dictionary<int, int> dictionary3 = new Dictionary<int, int>();
					for (int m = 0; m < count3; m++)
					{
						int num11 = list2[m];
						FieldObj.FOGridCell fOGridCell3 = this.LevelGrid.GridCells[num11];
						int cellX2 = (int)fOGridCell3.CellX;
						int cellY2 = (int)fOGridCell3.CellY;
						if (cellY2 == l)
						{
							dictionary3.Add(cellX2, num11);
						}
					}
					List<FowLos.SPolylineSegment> list5 = new List<FowLos.SPolylineSegment>();
					List<FowLos.SPolylineSegment> list6 = new List<FowLos.SPolylineSegment>();
					int num12 = -1;
					int num13 = -1;
					Dictionary<int, int>.Enumerator enumerator2 = dictionary3.GetEnumerator();
					while (enumerator2.MoveNext())
					{
						KeyValuePair<int, int> current3 = enumerator2.Current;
						int value3 = current3.Value;
						FieldObj.FOGridCell fOGridCell4 = this.LevelGrid.GridCells[value3];
						int cellX3 = (int)fOGridCell4.CellX;
						int cellY3 = (int)fOGridCell4.CellY;
						VInt2 start = new VInt2(gridInfo.GridPos.x + gridInfo.CellSizeX * cellX3, gridInfo.GridPos.y + gridInfo.CellSizeY * cellY3);
						VInt2 end = new VInt2(gridInfo.GridPos.x + gridInfo.CellSizeX * (cellX3 + 1), gridInfo.GridPos.y + gridInfo.CellSizeY * (cellY3 + 1));
						VInt2 start2 = new VInt2(start.x, end.y);
						VInt2 end2 = new VInt2(end.x, start.y);
						if (cellY3 == 0 || key != this.LevelGrid.GridCells[value3 - cellNumX].m_viewBlockId)
						{
							if (num12 != -1 && cellX3 == num12 + 1)
							{
								if (list6.Count > 0)
								{
									FowLos.SPolylineSegment value4 = new FowLos.SPolylineSegment(list6[list6.Count - 1]);
									value4.m_end = end2;
									list6[list6.Count - 1] = value4;
								}
								else
								{
									list6.Add(new FowLos.SPolylineSegment(start, end2));
								}
							}
							else
							{
								list6.Add(new FowLos.SPolylineSegment(start, end2));
							}
							num12 = cellX3;
						}
						if (cellY3 == cellNumY - 1 || key != this.LevelGrid.GridCells[value3 + cellNumX].m_viewBlockId)
						{
							if (num13 != -1 && cellX3 == num13 + 1)
							{
								if (list5.Count > 0)
								{
									FowLos.SPolylineSegment value5 = new FowLos.SPolylineSegment(list5[list5.Count - 1]);
									value5.m_end = end;
									list5[list5.Count - 1] = value5;
								}
								else
								{
									list5.Add(new FowLos.SPolylineSegment(start2, end));
								}
							}
							else
							{
								list5.Add(new FowLos.SPolylineSegment(start2, end));
							}
							num13 = cellX3;
						}
					}
					FieldObj.MergeBlockSegList(ref value2, list4, (int)key);
					FieldObj.MergeBlockSegList(ref value2, list6, (int)key);
					list4 = list5;
				}
				FieldObj.MergeBlockSegList(ref value2, list4, (int)key);
				list4.Clear();
				DebugHelper.Assert(value2.m_polylineSegList.Count >= 2);
				for (int n = num2; n <= num4; n++)
				{
					Dictionary<int, int> dictionary4 = new Dictionary<int, int>();
					for (int num14 = 0; num14 < count3; num14++)
					{
						int num15 = list2[num14];
						FieldObj.FOGridCell fOGridCell5 = this.LevelGrid.GridCells[num15];
						int cellX4 = (int)fOGridCell5.CellX;
						int cellY4 = (int)fOGridCell5.CellY;
						if (cellX4 == n)
						{
							dictionary4.Add(cellY4, num15);
						}
					}
					List<FowLos.SPolylineSegment> list7 = new List<FowLos.SPolylineSegment>();
					List<FowLos.SPolylineSegment> list8 = new List<FowLos.SPolylineSegment>();
					int num16 = -1;
					int num17 = -1;
					Dictionary<int, int>.Enumerator enumerator3 = dictionary4.GetEnumerator();
					while (enumerator3.MoveNext())
					{
						KeyValuePair<int, int> current4 = enumerator3.Current;
						int value6 = current4.Value;
						FieldObj.FOGridCell fOGridCell6 = this.LevelGrid.GridCells[value6];
						int cellX5 = (int)fOGridCell6.CellX;
						int cellY5 = (int)fOGridCell6.CellY;
						VInt2 start3 = new VInt2(gridInfo.GridPos.x + gridInfo.CellSizeX * cellX5, gridInfo.GridPos.y + gridInfo.CellSizeY * cellY5);
						VInt2 end3 = new VInt2(gridInfo.GridPos.x + gridInfo.CellSizeX * (cellX5 + 1), gridInfo.GridPos.y + gridInfo.CellSizeY * (cellY5 + 1));
						VInt2 end4 = new VInt2(start3.x, end3.y);
						VInt2 start4 = new VInt2(end3.x, start3.y);
						if (cellX5 == 0 || key != this.LevelGrid.GridCells[value6 - 1].m_viewBlockId)
						{
							if (num16 != -1 && cellY5 == num16 + 1)
							{
								if (list8.Count > 0)
								{
									FowLos.SPolylineSegment value7 = new FowLos.SPolylineSegment(list8[list8.Count - 1]);
									value7.m_end = end4;
									list8[list8.Count - 1] = value7;
								}
								else
								{
									list8.Add(new FowLos.SPolylineSegment(start3, end4));
								}
							}
							else
							{
								list8.Add(new FowLos.SPolylineSegment(start3, end4));
							}
							num16 = cellY5;
						}
						if (cellX5 == cellNumX - 1 || key != this.LevelGrid.GridCells[value6 + 1].m_viewBlockId)
						{
							if (num17 != -1 && cellY5 == num17 + 1)
							{
								if (list7.Count > 0)
								{
									FowLos.SPolylineSegment value8 = new FowLos.SPolylineSegment(list7[list7.Count - 1]);
									value8.m_end = end3;
									list7[list7.Count - 1] = value8;
								}
								else
								{
									list7.Add(new FowLos.SPolylineSegment(start4, end3));
								}
							}
							else
							{
								list7.Add(new FowLos.SPolylineSegment(start4, end3));
							}
							num17 = cellY5;
						}
					}
					FieldObj.MergeBlockSegList(ref value2, list4, (int)key);
					FieldObj.MergeBlockSegList(ref value2, list8, (int)key);
					list4 = list7;
				}
				FieldObj.MergeBlockSegList(ref value2, list4, (int)key);
				list4.Clear();
				DebugHelper.Assert(value2.m_polylineSegList.Count >= 4);
				int count4 = value2.m_polylineVertices.Count;
				for (int num18 = 0; num18 < count4; num18++)
				{
					FowLos.SPolylineVertex sPolylineVertex = value2.m_polylineVertices[num18];
					DebugHelper.Assert(sPolylineVertex.m_bNative);
					DebugHelper.Assert(sPolylineVertex.m_belongBlockId != -1);
					DebugHelper.Assert(sPolylineVertex.m_belongSegNoList.Count <= 4);
					DebugHelper.Assert(sPolylineVertex.m_belongSegNoList.Count >= 2);
				}
				int count5 = value2.m_polylineSegList.Count;
				for (int num19 = 0; num19 < count5; num19++)
				{
					FowLos.SPolylineSegment sPolylineSegment = value2.m_polylineSegList[num19];
					DebugHelper.Assert(sPolylineSegment.m_index == num19);
					DebugHelper.Assert(sPolylineSegment.m_belongBlockId != -1);
					DebugHelper.Assert(sPolylineSegment.m_startPtIndex != -1);
					DebugHelper.Assert(sPolylineSegment.m_endPtIndex != -1);
				}
				value2.m_blockerGridIndexList = value;
				if (dictionary.ContainsKey(key))
				{
					dictionary[key] = value2;
				}
				else
				{
					dictionary.Add(key, value2);
				}
			}
			return dictionary;
		}

		private static void MergeBlockSegList(ref FowLos.SBlockContext blockContext, List<FowLos.SPolylineSegment> rawPolylineSegList, int inAreaId)
		{
			int count = rawPolylineSegList.Count;
			for (int i = 0; i < count; i++)
			{
				int count2 = blockContext.m_polylineSegList.Count;
				FowLos.SPolylineSegment item = default(FowLos.SPolylineSegment);
				item.ParamlessConstruct();
				item.m_belongBlockId = inAreaId;
				item.m_index = count2;
				VInt2 start = rawPolylineSegList[i].m_start;
				VInt2 end = rawPolylineSegList[i].m_end;
				int num = -1;
				if (blockContext.ContainsPoint(start, out num))
				{
					item.m_startPtIndex = num;
					item.m_start = blockContext.m_polylineVertices[num].m_point;
					DebugHelper.Assert(blockContext.m_polylineVertices[num].m_belongSegNoList.Count > 0);
					blockContext.m_polylineVertices[num].m_belongSegNoList.Add(count2);
					DebugHelper.Assert(blockContext.m_polylineVertices[num].m_belongSegNoList.Count <= 4);
				}
				else
				{
					int count3 = blockContext.m_polylineVertices.Count;
					FowLos.SPolylineVertex item2 = default(FowLos.SPolylineVertex);
					item2.ParamlessConstruct();
					item2.m_point = start;
					item2.m_belongBlockId = inAreaId;
					item2.m_belongSegNoList.Add(count2);
					item2.m_bNative = true;
					item.m_startPtIndex = count3;
					item.m_start = item2.m_point;
					blockContext.m_polylineVertices.Add(item2);
				}
				if (blockContext.ContainsPoint(end, out num))
				{
					item.m_endPtIndex = num;
					item.m_end = blockContext.m_polylineVertices[num].m_point;
					DebugHelper.Assert(blockContext.m_polylineVertices[num].m_belongSegNoList.Count > 0);
					blockContext.m_polylineVertices[num].m_belongSegNoList.Add(count2);
					DebugHelper.Assert(blockContext.m_polylineVertices[num].m_belongSegNoList.Count <= 4);
				}
				else
				{
					int count4 = blockContext.m_polylineVertices.Count;
					FowLos.SPolylineVertex item3 = default(FowLos.SPolylineVertex);
					item3.ParamlessConstruct();
					item3.m_point = end;
					item3.m_belongBlockId = inAreaId;
					item3.m_belongSegNoList.Add(count2);
					item3.m_bNative = true;
					item.m_endPtIndex = count4;
					item.m_end = item3.m_point;
					blockContext.m_polylineVertices.Add(item3);
				}
				blockContext.m_polylineSegList.Add(item);
			}
		}
	}
}
