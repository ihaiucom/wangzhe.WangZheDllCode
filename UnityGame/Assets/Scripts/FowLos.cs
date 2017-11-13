using Assets.Scripts.GameSystem;
using CSProtocol;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class FowLos
{
	public enum EBlockRegionType
	{
		EBlockRegionType_Box,
		EBlockRegionType_Sphere,
		EBlockRegionType_Polygon,
		EBlockRegionType_Count
	}

	public enum EViewExploringMode
	{
		EViewExploringMode_ShadowCast,
		EViewExploringMode_RayCast,
		EViewExploringMode_WatchTower,
		EViewExploringMode_DistOnly,
		EViewExploringMode_Count
	}

	public struct SPolylineVertex
	{
		public VInt2 m_point;

		public int m_belongBlockId;

		public List<int> m_belongSegNoList;

		public bool m_bNative;

		public SPolylineVertex(FowLos.SPolylineVertex rhs)
		{
			this.m_point = rhs.m_point;
			this.m_belongBlockId = rhs.m_belongBlockId;
			this.m_belongSegNoList = new List<int>(rhs.m_belongSegNoList);
			this.m_bNative = rhs.m_bNative;
		}

		public SPolylineVertex(VInt2 point)
		{
			this.m_point = point;
			this.m_belongBlockId = -1;
			this.m_belongSegNoList = new List<int>();
			this.m_bNative = true;
		}

		public void ParamlessConstruct()
		{
			this.m_point = VInt2.zero;
			this.m_belongBlockId = -1;
			this.m_belongSegNoList = new List<int>();
			this.m_bNative = true;
		}

		public bool Equals(FowLos.SPolylineVertex rhs)
		{
			return BaseAlgorithm.IsNearlyZero(this.m_point - rhs.m_point, 10) && this.m_belongBlockId == rhs.m_belongBlockId && this.m_belongSegNoList == rhs.m_belongSegNoList && this.m_bNative == rhs.m_bNative;
		}

		public bool NotEqual(FowLos.SPolylineVertex rhs)
		{
			return !this.Equals(rhs);
		}

		public bool IsNear(VInt2 point)
		{
			return BaseAlgorithm.IsNearlyZero(this.m_point - point, 10);
		}
	}

	public struct SPolylineSegment
	{
		public int m_startPtIndex;

		public int m_endPtIndex;

		public int m_belongBlockId;

		public VInt2 m_start;

		public VInt2 m_end;

		public int m_index;

		public SPolylineSegment(FowLos.SPolylineSegment rhs)
		{
			this.m_startPtIndex = rhs.m_startPtIndex;
			this.m_endPtIndex = rhs.m_endPtIndex;
			this.m_belongBlockId = rhs.m_belongBlockId;
			this.m_start = rhs.m_start;
			this.m_end = rhs.m_end;
			this.m_index = rhs.m_index;
		}

		public SPolylineSegment(VInt2 start, VInt2 end)
		{
			this.m_startPtIndex = -1;
			this.m_endPtIndex = -1;
			this.m_belongBlockId = -1;
			this.m_start = start;
			this.m_end = end;
			this.m_index = -1;
		}

		public void ParamlessConstruct()
		{
			this.m_startPtIndex = -1;
			this.m_endPtIndex = -1;
			this.m_belongBlockId = -1;
			this.m_start = VInt2.zero;
			this.m_end = VInt2.zero;
			this.m_index = -1;
		}
	}

	public struct FBoxSphereBounds
	{
		public VInt3 Origin;

		public VInt3 BoxExtent;
	}

	public struct SBlockContext
	{
		public FowLos.FBoxSphereBounds m_bounds;

		public byte m_xMin;

		public byte m_xMax;

		public byte m_yMin;

		public byte m_yMax;

		public int m_areaId;

		public List<int> m_blockerGridIndexList;

		public List<FowLos.SPolylineSegment> m_polylineSegList;

		public List<FowLos.SPolylineVertex> m_polylineVertices;

		public SBlockContext(FowLos.SBlockContext rhs)
		{
			this.m_bounds = rhs.m_bounds;
			this.m_xMin = rhs.m_xMin;
			this.m_xMax = rhs.m_xMax;
			this.m_yMin = rhs.m_yMin;
			this.m_yMax = rhs.m_yMax;
			this.m_areaId = rhs.m_areaId;
			this.m_blockerGridIndexList = new List<int>(rhs.m_blockerGridIndexList);
			this.m_polylineSegList = new List<FowLos.SPolylineSegment>(rhs.m_polylineSegList);
			this.m_polylineVertices = new List<FowLos.SPolylineVertex>(rhs.m_polylineVertices);
		}

		public void ParamlessConstruct()
		{
			this.m_bounds = default(FowLos.FBoxSphereBounds);
			this.m_areaId = -1;
			this.m_blockerGridIndexList = new List<int>();
			this.m_polylineSegList = new List<FowLos.SPolylineSegment>();
			this.m_polylineVertices = new List<FowLos.SPolylineVertex>();
		}

		public bool ContainsPoint(VInt2 point, out int index)
		{
			int count = this.m_polylineVertices.get_Count();
			for (int i = 0; i < count; i++)
			{
				if (this.m_polylineVertices.get_Item(i).IsNear(point))
				{
					index = i;
					return true;
				}
			}
			index = -1;
			return false;
		}
	}

	public struct SGridWall
	{
		public bool m_horizontal;

		public VInt2 m_start;

		public VInt2 m_end;
	}

	public struct SBlockWalls
	{
		public byte m_xMin;

		public byte m_xMax;

		public byte m_yMin;

		public byte m_yMax;

		public byte m_areaId;

		public Dictionary<byte, List<FowLos.SGridWall>> m_wallsHorizontal;

		public Dictionary<byte, List<FowLos.SGridWall>> m_wallsVertical;

		public void ParamlessConstruct()
		{
			this.m_areaId = 0;
			this.m_wallsHorizontal = new Dictionary<byte, List<FowLos.SGridWall>>();
			this.m_wallsVertical = new Dictionary<byte, List<FowLos.SGridWall>>();
		}
	}

	public struct SSegUID
	{
		public int m_blockId;

		public int m_segIndex;

		public SSegUID(int blockId, int segIndex)
		{
			this.m_blockId = blockId;
			this.m_segIndex = segIndex;
		}
	}

	private class ComparerVertex : IComparer<FowLos.SPolylineVertex>
	{
		private VInt2 m_scanlineStart = VInt2.zero;

		public ComparerVertex(VInt2 inSegStart)
		{
			this.m_scanlineStart = inSegStart;
		}

		public int Compare(FowLos.SPolylineVertex A, FowLos.SPolylineVertex B)
		{
			if ((A.m_point - this.m_scanlineStart).sqrMagnitude > (B.m_point - this.m_scanlineStart).sqrMagnitude)
			{
				return 1;
			}
			return -1;
		}
	}

	private class ComparerSortedSlope : IComparer<int>
	{
		public int Compare(int A, int B)
		{
			return A - B;
		}
	}

	private class CRaycastQuadrant
	{
		public VInt2 min;

		public VInt2 max;

		public List<FowLos.SBlockWalls> viewBlockArrayFinal;

		public List<byte> wallsHorizontal;

		public List<byte> wallsVertical;

		public CRaycastQuadrant()
		{
			this.min = VInt2.zero;
			this.max = VInt2.zero;
			this.viewBlockArrayFinal = new List<FowLos.SBlockWalls>();
			this.wallsHorizontal = new List<byte>();
			this.wallsVertical = new List<byte>();
		}

		public void Clear()
		{
			this.viewBlockArrayFinal.Clear();
			this.viewBlockArrayFinal = null;
			this.wallsHorizontal.Clear();
			this.wallsHorizontal = null;
			this.wallsVertical.Clear();
			this.wallsVertical = null;
		}
	}

	public const int Point_Attach_Threshold = 10;

	public const int Point_Attach_Threshold_Bigger = 100;

	private const int VIEWSIGHT_BOUND_DEFLATE_LENGTH = 40;

	public const int INDEX_NONE = -1;

	public const int inThicknessMax = 3;

	private _SetCellVisible m_setCellVisible = default(_SetCellVisible);

	public void ExploreCells(VInt3 location, int inSightRange, GameFowManager pFowMgr, COM_PLAYERCAMP camp, FowLos.EViewExploringMode ViewExploringMode, bool bDrawDebugLines)
	{
		if (pFowMgr == null || pFowMgr.m_pFieldObj == null)
		{
			return;
		}
		if (pFowMgr.m_pFieldObj.ViewBlockArrayImpl == null && Application.isPlaying)
		{
			pFowMgr.m_pFieldObj.CreateBlockWalls();
		}
		VInt3 vInt = location;
		VInt2 zero = VInt2.zero;
		pFowMgr.WorldPosToGrid(vInt, out zero.x, out zero.y);
		pFowMgr.ReviseWorldPosToCenter(vInt, out vInt);
		this.ExploreCellsInternal<_SetCellVisible>(ref this.m_setCellVisible, zero, vInt, inSightRange, pFowMgr, camp, ViewExploringMode, bDrawDebugLines);
	}

	public bool ExploreCellsFast(VInt3 location, int surfSightRange, GameFowManager pFowMgr, FieldObj inFieldObj, COM_PLAYERCAMP camp, bool bStaticExplore, bool bDistOnly)
	{
		if (pFowMgr == null || inFieldObj == null)
		{
			return false;
		}
		VInt2 vInt = VInt2.zero;
		pFowMgr.WorldPosToGrid(location, out vInt.x, out vInt.y);
		FieldObj.SViewBlockAttr sViewBlockAttr = default(FieldObj.SViewBlockAttr);
		if (inFieldObj.QueryAttr(vInt, out sViewBlockAttr) && sViewBlockAttr.BlockType == 2)
		{
			VInt2 zero = VInt2.zero;
			if (inFieldObj.FindNearestGrid(vInt, location, FieldObj.EViewBlockType.Brick, true, 3, null, out zero))
			{
				vInt = zero;
			}
		}
		if (bStaticExplore)
		{
			FowLos.TraverseStaticSurCell(surfSightRange, vInt.x, vInt.y, inFieldObj.NumX, inFieldObj.NumY, camp);
		}
		else
		{
			FowLos.TraverseSurCell(surfSightRange, vInt.x, vInt.y, inFieldObj.NumX, inFieldObj.NumY, camp, bDistOnly);
		}
		return true;
	}

	private bool ValidateViewBlock(FowLos.SBlockWalls inBlockWalls, byte xMin, byte xMax, byte yMin, byte yMax)
	{
		return inBlockWalls.m_xMin <= xMax && inBlockWalls.m_xMax >= xMin && inBlockWalls.m_yMin <= yMax && inBlockWalls.m_yMax >= yMin;
	}

	private bool ValidateViewBlock(FowLos.SBlockWalls inBlockWalls, int xMin, int xMax, int yMin, int yMax)
	{
		return (int)inBlockWalls.m_xMin <= xMax && (int)inBlockWalls.m_xMax >= xMin && (int)inBlockWalls.m_yMin <= yMax && (int)inBlockWalls.m_yMax >= yMin;
	}

	private bool ValidateViewBlock(FowLos.FBoxSphereBounds inBounds, VInt2 segStart, int inViewSight)
	{
		bool result = true;
		int num = inBounds.Origin.x - inBounds.BoxExtent.x;
		int num2 = inBounds.Origin.x + inBounds.BoxExtent.x;
		int num3 = inBounds.Origin.y - inBounds.BoxExtent.y;
		int num4 = inBounds.Origin.y + inBounds.BoxExtent.y;
		if (num > segStart.x + inViewSight || num2 < segStart.x - inViewSight || num3 > segStart.y + inViewSight || num4 < segStart.y - inViewSight)
		{
			result = false;
		}
		return result;
	}

	public static bool SegmentIntersect(VInt2 aa, VInt2 bb, VInt2 cc, VInt2 dd, ref VInt2 intersectPoint)
	{
		Vector2 vector = (Vector2)aa;
		Vector2 vector2 = (Vector2)bb;
		Vector2 vector3 = (Vector2)cc;
		Vector2 vector4 = (Vector2)dd;
		float num = (vector.x - vector3.x) * (vector2.y - vector3.y) - (vector.y - vector3.y) * (vector2.x - vector3.x);
		float num2 = (vector.x - vector4.x) * (vector2.y - vector4.y) - (vector.y - vector4.y) * (vector2.x - vector4.x);
		if (num * num2 >= 0f)
		{
			return false;
		}
		float num3 = (vector3.x - vector.x) * (vector4.y - vector.y) - (vector3.y - vector.y) * (vector4.x - vector.x);
		float num4 = num3 + num - num2;
		if (num3 * num4 >= 0f)
		{
			return false;
		}
		float num5 = num2 - num;
		num5 = ((num5 < 0f) ? Mathf.Min(num5, -0.0001f) : Mathf.Max(num5, 0.0001f));
		float num6 = num3 / num5;
		float num7 = num6 * (vector2.x - vector.x);
		float num8 = num6 * (vector2.y - vector.y);
		Vector2 ob = new Vector2(vector.x + num7, vector.y + num8);
		intersectPoint = (VInt2)ob;
		return true;
	}

	public static bool SegmentIntersect(VInt2 aa, VInt2 bb, VInt2 cc, VInt2 dd)
	{
		Vector2 vector = (Vector2)aa;
		Vector2 vector2 = (Vector2)bb;
		Vector2 vector3 = (Vector2)cc;
		Vector2 vector4 = (Vector2)dd;
		float num = (vector.x - vector3.x) * (vector2.y - vector3.y) - (vector.y - vector3.y) * (vector2.x - vector3.x);
		float num2 = (vector.x - vector4.x) * (vector2.y - vector4.y) - (vector.y - vector4.y) * (vector2.x - vector4.x);
		if (num * num2 >= 0f)
		{
			return false;
		}
		float num3 = (vector3.x - vector.x) * (vector4.y - vector.y) - (vector3.y - vector.y) * (vector4.x - vector.x);
		float num4 = num3 + num - num2;
		return num3 * num4 < 0f;
	}

	public void Init()
	{
		this.m_setCellVisible.Init();
	}

	public void Uninit()
	{
	}

	public void ExploreCellsInternal<TSetCellVisible>(ref TSetCellVisible setCellVisible, VInt2 newSurfPos, VInt3 unitLoc, int surfSightRange, GameFowManager pFowMgr, COM_PLAYERCAMP camp, FowLos.EViewExploringMode ViewExploringMode, bool bDrawDebugLines) where TSetCellVisible : ISetCellVisible
	{
		if (pFowMgr == null || pFowMgr.m_pFieldObj == null)
		{
			return;
		}
		if (ViewExploringMode != FowLos.EViewExploringMode.EViewExploringMode_ShadowCast)
		{
			if (ViewExploringMode == FowLos.EViewExploringMode.EViewExploringMode_DistOnly)
			{
				for (int i = -surfSightRange - 1; i <= surfSightRange + 1; i++)
				{
					for (int j = -surfSightRange - 1; j <= surfSightRange + 1; j++)
					{
						VInt2 b = new VInt2(i, j);
						VInt2 inPos = newSurfPos + b;
						if (pFowMgr.IsInsideSurface(inPos.x, inPos.y) && b.sqrMagnitude < surfSightRange * surfSightRange)
						{
							setCellVisible.SetVisible(inPos, camp, true);
						}
					}
				}
			}
			else if (ViewExploringMode != FowLos.EViewExploringMode.EViewExploringMode_WatchTower && ViewExploringMode == FowLos.EViewExploringMode.EViewExploringMode_RayCast)
			{
				int sightSqr = surfSightRange * surfSightRange;
				int num = newSurfPos.x - surfSightRange;
				int num2 = newSurfPos.x + surfSightRange;
				int num3 = newSurfPos.y - surfSightRange;
				int num4 = newSurfPos.y + surfSightRange;
				num = Mathf.Clamp(num, 0, pFowMgr.m_pFieldObj.NumX - 1);
				num2 = Mathf.Clamp(num2, 0, pFowMgr.m_pFieldObj.NumX - 1);
				num3 = Mathf.Clamp(num3, 0, pFowMgr.m_pFieldObj.NumY - 1);
				num4 = Mathf.Clamp(num4, 0, pFowMgr.m_pFieldObj.NumY - 1);
				byte viewBlockId = pFowMgr.m_pFieldObj.LevelGrid.GetGridCell(newSurfPos).m_viewBlockId;
				FieldObj.SViewBlockAttr sViewBlockAttr = default(FieldObj.SViewBlockAttr);
				pFowMgr.m_pFieldObj.QueryAttr(newSurfPos, out sViewBlockAttr);
				FowLos.CRaycastQuadrant cRaycastQuadrant = new FowLos.CRaycastQuadrant();
				cRaycastQuadrant.min = new VInt2(num, num3);
				cRaycastQuadrant.max = new VInt2(num2, num4);
				if (pFowMgr.m_pFieldObj.ViewBlockArrayImpl != null)
				{
					List<FowLos.SBlockWalls>.Enumerator enumerator = pFowMgr.m_pFieldObj.ViewBlockArrayImpl.GetEnumerator();
					while (enumerator.MoveNext())
					{
						FowLos.SBlockWalls current = enumerator.get_Current();
						int areaId = (int)current.m_areaId;
						DebugHelper.Assert(areaId != 0);
						FieldObj.SViewBlockAttr sViewBlockAttr2;
						if (((int)viewBlockId != areaId || sViewBlockAttr.BlockType != 1) && (pFowMgr.m_pFieldObj.GrassBlockView || !pFowMgr.m_pFieldObj.ViewBlockAttrMap.TryGetValue((byte)areaId, ref sViewBlockAttr2) || sViewBlockAttr2.BlockType != 1) && this.ValidateViewBlock(current, cRaycastQuadrant.min.x, cRaycastQuadrant.max.x, cRaycastQuadrant.min.y, cRaycastQuadrant.max.y))
						{
							cRaycastQuadrant.viewBlockArrayFinal.Add(current);
							Dictionary<byte, List<FowLos.SGridWall>>.Enumerator enumerator2 = current.m_wallsHorizontal.GetEnumerator();
							while (enumerator2.MoveNext())
							{
								List<byte> wallsHorizontal = cRaycastQuadrant.wallsHorizontal;
								KeyValuePair<byte, List<FowLos.SGridWall>> current2 = enumerator2.get_Current();
								BaseAlgorithm.AddUniqueItem<byte>(wallsHorizontal, current2.get_Key());
							}
							enumerator2 = current.m_wallsVertical.GetEnumerator();
							while (enumerator2.MoveNext())
							{
								List<byte> wallsVertical = cRaycastQuadrant.wallsVertical;
								KeyValuePair<byte, List<FowLos.SGridWall>> current3 = enumerator2.get_Current();
								BaseAlgorithm.AddUniqueItem<byte>(wallsVertical, current3.get_Key());
							}
						}
					}
					this.RaycastCheck<TSetCellVisible>(pFowMgr, cRaycastQuadrant, newSurfPos, unitLoc, sightSqr, camp, bDrawDebugLines, viewBlockId, ref setCellVisible);
				}
				cRaycastQuadrant.Clear();
			}
		}
	}

	private void RaycastCheck<TSetCellVisible>(GameFowManager pFowMgr, FowLos.CRaycastQuadrant quadrant, VInt2 newSurfPos, VInt3 unitLoc, int sightSqr, COM_PLAYERCAMP camp, bool bDrawDebugLines, byte inStartViewblockId, ref TSetCellVisible setCellVisible) where TSetCellVisible : ISetCellVisible
	{
		VInt2 aa = new VInt2(unitLoc.x, unitLoc.y);
		int x = quadrant.min.x;
		int x2 = quadrant.max.x;
		int y = quadrant.min.y;
		int y2 = quadrant.max.y;
		VInt2 zero = VInt2.zero;
		int halfSizeX = pFowMgr.m_halfSizeX;
		int halfSizeY = pFowMgr.m_halfSizeY;
		for (int i = x; i <= x2; i++)
		{
			for (int j = y; j <= y2; j++)
			{
				VInt2 vInt = new VInt2(i, j);
				if (i == newSurfPos.x && j == newSurfPos.y)
				{
					setCellVisible.SetVisible(vInt, camp, true);
				}
				else if ((vInt - newSurfPos).sqrMagnitude < sightSqr)
				{
					FieldObj.SViewBlockAttr sViewBlockAttr;
					if (!pFowMgr.m_pFieldObj.GrassBlockView && pFowMgr.m_pFieldObj.QueryAttr(vInt, out sViewBlockAttr) && sViewBlockAttr.BlockType == 1)
					{
						int num = (int)pFowMgr.m_pFieldObj.QueryViewblockId(vInt.x, vInt.y);
						if (num != (int)inStartViewblockId)
						{
							setCellVisible.SetVisible(vInt, camp, false);
							goto IL_6DD;
						}
					}
					VInt3 zero2 = VInt3.zero;
					pFowMgr.m_pFieldObj.LevelGrid.GridToWorldPos(i, j, out zero2);
					VInt2 vInt2 = new VInt2(zero2.x, zero2.y);
					VInt2 bb = vInt2;
					bb.x -= halfSizeX;
					bb.y -= halfSizeY;
					VInt2 bb2 = vInt2;
					bb2.x += halfSizeX;
					bb2.y -= halfSizeY;
					VInt2 bb3 = vInt2;
					bb3.x += halfSizeX;
					bb3.y += halfSizeY;
					VInt2 bb4 = vInt2;
					bb4.x -= halfSizeX;
					bb4.y += halfSizeY;
					FowLos.SGridWall sGridWall = default(FowLos.SGridWall);
					bool flag = false;
					bool flag2 = false;
					bool flag3 = false;
					bool flag4 = i < newSurfPos.x;
					bool flag5 = j < newSurfPos.y;
					int num2 = flag4 ? newSurfPos.x : (newSurfPos.x + 1);
					int num3 = flag5 ? newSurfPos.y : (newSurfPos.y + 1);
					int num4 = 0;
					while (!flag && (!flag2 || !flag3))
					{
						if (!flag2)
						{
							int num5 = num2 + (flag4 ? (-num4) : num4);
							if (flag4 && num5 < i + 1)
							{
								flag2 = true;
							}
							else if (!flag4 && num5 > i)
							{
								flag2 = true;
							}
							if (!flag2 && quadrant.wallsVertical.Contains((byte)num5))
							{
								List<FowLos.SBlockWalls>.Enumerator enumerator = quadrant.viewBlockArrayFinal.GetEnumerator();
								while (enumerator.MoveNext() && !flag)
								{
									FowLos.SBlockWalls current = enumerator.get_Current();
									List<FowLos.SGridWall> list = null;
									if (current.m_wallsVertical.TryGetValue((byte)num5, ref list))
									{
										int count = list.get_Count();
										int num6 = 0;
										while (num6 < count && !flag)
										{
											FowLos.SGridWall sGridWall2 = list.get_Item(num6);
											if (bDrawDebugLines)
											{
												if (FowLos.SegmentIntersect(aa, vInt2, sGridWall2.m_start, sGridWall2.m_end, ref zero))
												{
													flag = true;
												}
												else if (FowLos.SegmentIntersect(aa, bb, sGridWall2.m_start, sGridWall2.m_end, ref zero) || FowLos.SegmentIntersect(aa, bb2, sGridWall2.m_start, sGridWall2.m_end, ref zero) || FowLos.SegmentIntersect(aa, bb3, sGridWall2.m_start, sGridWall2.m_end, ref zero) || FowLos.SegmentIntersect(aa, bb4, sGridWall2.m_start, sGridWall2.m_end, ref zero))
												{
													flag = true;
												}
											}
											else if (FowLos.SegmentIntersect(aa, vInt2, sGridWall2.m_start, sGridWall2.m_end))
											{
												flag = true;
											}
											else if (FowLos.SegmentIntersect(aa, bb, sGridWall2.m_start, sGridWall2.m_end, ref zero) || FowLos.SegmentIntersect(aa, bb2, sGridWall2.m_start, sGridWall2.m_end, ref zero) || FowLos.SegmentIntersect(aa, bb3, sGridWall2.m_start, sGridWall2.m_end, ref zero) || FowLos.SegmentIntersect(aa, bb4, sGridWall2.m_start, sGridWall2.m_end, ref zero))
											{
												flag = true;
											}
											num6++;
										}
									}
								}
							}
						}
						if (!flag3 && !flag)
						{
							int num7 = num3 + (flag5 ? (-num4) : num4);
							if (flag5 && num7 < j + 1)
							{
								flag3 = true;
							}
							else if (!flag5 && num7 > j)
							{
								flag3 = true;
							}
							if (!flag3 && quadrant.wallsHorizontal.Contains((byte)num7))
							{
								List<FowLos.SBlockWalls>.Enumerator enumerator2 = quadrant.viewBlockArrayFinal.GetEnumerator();
								while (enumerator2.MoveNext() && !flag)
								{
									FowLos.SBlockWalls current2 = enumerator2.get_Current();
									List<FowLos.SGridWall> list2 = null;
									if (current2.m_wallsHorizontal.TryGetValue((byte)num7, ref list2))
									{
										int count2 = list2.get_Count();
										int num8 = 0;
										while (num8 < count2 && !flag)
										{
											FowLos.SGridWall sGridWall3 = list2.get_Item(num8);
											if (bDrawDebugLines)
											{
												if (FowLos.SegmentIntersect(aa, vInt2, sGridWall3.m_start, sGridWall3.m_end, ref zero))
												{
													flag = true;
												}
												else if (FowLos.SegmentIntersect(aa, bb, sGridWall3.m_start, sGridWall3.m_end, ref zero) || FowLos.SegmentIntersect(aa, bb2, sGridWall3.m_start, sGridWall3.m_end, ref zero) || FowLos.SegmentIntersect(aa, bb3, sGridWall3.m_start, sGridWall3.m_end, ref zero) || FowLos.SegmentIntersect(aa, bb4, sGridWall3.m_start, sGridWall3.m_end, ref zero))
												{
													flag = true;
												}
											}
											else if (FowLos.SegmentIntersect(aa, vInt2, sGridWall3.m_start, sGridWall3.m_end))
											{
												flag = true;
											}
											else if (FowLos.SegmentIntersect(aa, bb, sGridWall3.m_start, sGridWall3.m_end, ref zero) || FowLos.SegmentIntersect(aa, bb2, sGridWall3.m_start, sGridWall3.m_end, ref zero) || FowLos.SegmentIntersect(aa, bb3, sGridWall3.m_start, sGridWall3.m_end, ref zero) || FowLos.SegmentIntersect(aa, bb4, sGridWall3.m_start, sGridWall3.m_end, ref zero))
											{
												flag = true;
											}
											num8++;
										}
									}
								}
							}
						}
						num4++;
					}
					if (!flag)
					{
						setCellVisible.SetVisible(vInt, camp, true);
					}
				}
				IL_6DD:;
			}
		}
	}

	[DllImport("SGameFowProject", CallingConvention = CallingConvention.Cdecl)]
	private static extern void TraverseSurCell(int sightRange, int surfPosX, int surfPosY, int surfMaxX, int surfMaxY, COM_PLAYERCAMP camp, bool bDistOnly);

	[DllImport("SGameFowProject", CallingConvention = CallingConvention.Cdecl)]
	public static extern void TraverseStaticSurCell(int sightRange, int surfPosX, int surfPosY, int surfMaxX, int surfMaxY, COM_PLAYERCAMP camp);

	[DllImport("SGameFowProject", CallingConvention = CallingConvention.Cdecl)]
	public static extern void PreCopyBitmapFakeSight(int surfPosX, int surfPosY, int surfMaxX, int surfMaxY);

	[DllImport("SGameFowProject", CallingConvention = CallingConvention.Cdecl)]
	public static extern void PreCopyBitmap();
}
