using Pathfinding.Util;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding.Voxels
{
	public class Voxelize
	{
		public const uint NotConnected = 63u;

		public const int MaxLayers = 65535;

		public const int MaxRegions = 500;

		public const int UnwalkableArea = 0;

		public const ushort BorderReg = 32768;

		public const int RC_BORDER_VERTEX = 65536;

		public const int RC_AREA_BORDER = 131072;

		public const int VERTEX_BUCKET_COUNT = 4096;

		public const int RC_CONTOUR_TESS_WALL_EDGES = 1;

		public const int RC_CONTOUR_TESS_AREA_EDGES = 2;

		public const int ContourRegMask = 65535;

		private static List<int[]> intArrCache = new List<int[]>();

		private static readonly int[] emptyArr = new int[0];

		public List<ExtraMesh> inputExtraMeshes;

		protected Vector3[] inputVertices;

		protected int[] inputTriangles;

		public readonly int voxelWalkableClimb;

		public readonly uint voxelWalkableHeight;

		public readonly float cellSize = 0.2f;

		public readonly float cellHeight = 0.1f;

		public int minRegionSize = 100;

		public int borderSize;

		public float maxEdgeLength = 20f;

		public float maxSlope = 30f;

		public RecastGraph.RelevantGraphSurfaceMode relevantGraphSurfaceMode;

		public Bounds forcedBounds;

		public VoxelArea voxelArea;

		public VoxelContourSet countourSet;

		public int width;

		public int depth;

		public Vector3 voxelOffset;

		public string debugString = string.Empty;

		public readonly Vector3 cellScale;

		public readonly Vector3 cellScaleDivision;

		public Voxelize(float ch, float cs, float wc, float wh, float ms)
		{
			this.cellSize = cs;
			this.cellHeight = ch;
			this.maxSlope = ms;
			this.cellScale = new Vector3(this.cellSize, this.cellHeight, this.cellSize);
			this.cellScaleDivision = new Vector3(1f / this.cellSize, 1f / this.cellHeight, 1f / this.cellSize);
			this.voxelWalkableHeight = (uint)(wh / this.cellHeight);
			this.voxelWalkableClimb = Mathf.RoundToInt(wc / this.cellHeight);
		}

		public void BuildContours(float maxError, int maxEdgeLength, VoxelContourSet cset, int buildFlags)
		{
			int num = this.voxelArea.width;
			int num2 = this.voxelArea.depth;
			int num3 = num * num2;
			int num4 = Mathf.Max(8, 8);
			List<VoxelContour> list = new List<VoxelContour>(num4);
			ushort[] array = this.voxelArea.tmpUShortArr;
			if (array.Length < this.voxelArea.compactSpanCount)
			{
				array = (this.voxelArea.tmpUShortArr = new ushort[this.voxelArea.compactSpanCount]);
			}
			for (int i = 0; i < num3; i += this.voxelArea.width)
			{
				for (int j = 0; j < this.voxelArea.width; j++)
				{
					CompactVoxelCell compactVoxelCell = this.voxelArea.compactCells[j + i];
					int k = (int)compactVoxelCell.index;
					int num5 = (int)(compactVoxelCell.index + compactVoxelCell.count);
					while (k < num5)
					{
						ushort num6 = 0;
						CompactVoxelSpan compactVoxelSpan = this.voxelArea.compactSpans[k];
						if (compactVoxelSpan.reg == 0 || (compactVoxelSpan.reg & 32768) == 32768)
						{
							array[k] = 0;
						}
						else
						{
							for (int l = 0; l < 4; l++)
							{
								int num7 = 0;
								if ((long)compactVoxelSpan.GetConnection(l) != 63L)
								{
									int num8 = j + this.voxelArea.DirectionX[l];
									int num9 = i + this.voxelArea.DirectionZ[l];
									int num10 = (int)(this.voxelArea.compactCells[num8 + num9].index + (uint)compactVoxelSpan.GetConnection(l));
									num7 = this.voxelArea.compactSpans[num10].reg;
								}
								if (num7 == compactVoxelSpan.reg)
								{
									num6 |= (ushort)(1 << l);
								}
							}
							array[k] = (num6 ^ 15);
						}
						k++;
					}
				}
			}
			List<int> list2 = ListPool<int>.Claim(256);
			List<int> list3 = ListPool<int>.Claim(64);
			for (int m = 0; m < num3; m += this.voxelArea.width)
			{
				for (int n = 0; n < this.voxelArea.width; n++)
				{
					CompactVoxelCell compactVoxelCell2 = this.voxelArea.compactCells[n + m];
					int num11 = (int)compactVoxelCell2.index;
					int num12 = (int)(compactVoxelCell2.index + compactVoxelCell2.count);
					while (num11 < num12)
					{
						if (array[num11] == 0 || array[num11] == 15)
						{
							array[num11] = 0;
						}
						else
						{
							int reg = this.voxelArea.compactSpans[num11].reg;
							if (reg != 0 && (reg & 32768) != 32768)
							{
								int area = this.voxelArea.areaTypes[num11];
								list2.Clear();
								list3.Clear();
								this.WalkContour(n, m, num11, array, list2);
								this.SimplifyContour(list2, list3, maxError, maxEdgeLength, buildFlags);
								this.RemoveDegenerateSegments(list3);
								VoxelContour voxelContour = default(VoxelContour);
								voxelContour.verts = Voxelize.ClaimIntArr(list3.get_Count(), false);
								for (int num13 = 0; num13 < list3.get_Count(); num13++)
								{
									voxelContour.verts[num13] = list3.get_Item(num13);
								}
								voxelContour.nverts = list3.get_Count() / 4;
								voxelContour.reg = reg;
								voxelContour.area = area;
								list.Add(voxelContour);
							}
						}
						num11++;
					}
				}
			}
			ListPool<int>.Release(list2);
			ListPool<int>.Release(list3);
			for (int num14 = 0; num14 < list.get_Count(); num14++)
			{
				VoxelContour voxelContour2 = list.get_Item(num14);
				if (this.CalcAreaOfPolygon2D(voxelContour2.verts, voxelContour2.nverts) < 0)
				{
					int num15 = -1;
					for (int num16 = 0; num16 < list.get_Count(); num16++)
					{
						if (num14 != num16 && list.get_Item(num16).nverts > 0 && list.get_Item(num16).reg == voxelContour2.reg && this.CalcAreaOfPolygon2D(list.get_Item(num16).verts, list.get_Item(num16).nverts) > 0)
						{
							num15 = num16;
							break;
						}
					}
					if (num15 == -1)
					{
						Debug.LogError("rcBuildContours: Could not find merge target for bad contour " + num14 + ".");
					}
					else
					{
						Debug.LogWarning("Fixing contour");
						VoxelContour voxelContour3 = list.get_Item(num15);
						int num17 = 0;
						int num18 = 0;
						this.GetClosestIndices(voxelContour3.verts, voxelContour3.nverts, voxelContour2.verts, voxelContour2.nverts, ref num17, ref num18);
						if (num17 == -1 || num18 == -1)
						{
							Debug.LogWarning(string.Concat(new object[]
							{
								"rcBuildContours: Failed to find merge points for ",
								num14,
								" and ",
								num15,
								"."
							}));
						}
						else if (!Voxelize.MergeContours(ref voxelContour3, ref voxelContour2, num17, num18))
						{
							Debug.LogWarning(string.Concat(new object[]
							{
								"rcBuildContours: Failed to merge contours ",
								num14,
								" and ",
								num15,
								"."
							}));
						}
						else
						{
							list.set_Item(num15, voxelContour3);
							list.set_Item(num14, voxelContour2);
						}
					}
				}
			}
			cset.conts = list;
		}

		private void GetClosestIndices(int[] vertsa, int nvertsa, int[] vertsb, int nvertsb, ref int ia, ref int ib)
		{
			int num = 268435455;
			ia = -1;
			ib = -1;
			for (int i = 0; i < nvertsa; i++)
			{
				int num2 = (i + 1) % nvertsa;
				int num3 = (i + nvertsa - 1) % nvertsa;
				int num4 = i * 4;
				int b = num2 * 4;
				int a = num3 * 4;
				for (int j = 0; j < nvertsb; j++)
				{
					int num5 = j * 4;
					if (Voxelize.Ileft(a, num4, num5, vertsa, vertsa, vertsb) && Voxelize.Ileft(num4, b, num5, vertsa, vertsa, vertsb))
					{
						int num6 = vertsb[num5] - vertsa[num4];
						int num7 = vertsb[num5 + 2] / this.voxelArea.width - vertsa[num4 + 2] / this.voxelArea.width;
						int num8 = num6 * num6 + num7 * num7;
						if (num8 < num)
						{
							ia = i;
							ib = j;
							num = num8;
						}
					}
				}
			}
		}

		private static void ReleaseIntArr(int[] arr)
		{
			if (arr != null)
			{
				Voxelize.intArrCache.Add(arr);
			}
		}

		private static int[] ClaimIntArr(int minCapacity, bool zero)
		{
			for (int i = 0; i < Voxelize.intArrCache.get_Count(); i++)
			{
				if (Voxelize.intArrCache.get_Item(i).Length >= minCapacity)
				{
					int[] array = Voxelize.intArrCache.get_Item(i);
					Voxelize.intArrCache.RemoveAt(i);
					if (zero)
					{
						Memory.MemSet<int>(array, 0, 4);
					}
					return array;
				}
			}
			return new int[minCapacity];
		}

		private static void ReleaseContours(VoxelContourSet cset)
		{
			for (int i = 0; i < cset.conts.get_Count(); i++)
			{
				VoxelContour voxelContour = cset.conts.get_Item(i);
				Voxelize.ReleaseIntArr(voxelContour.verts);
				Voxelize.ReleaseIntArr(voxelContour.rverts);
			}
			cset.conts = null;
		}

		public static bool MergeContours(ref VoxelContour ca, ref VoxelContour cb, int ia, int ib)
		{
			int num = ca.nverts + cb.nverts + 2;
			int[] array = Voxelize.ClaimIntArr(num * 4, false);
			int num2 = 0;
			for (int i = 0; i <= ca.nverts; i++)
			{
				int num3 = num2 * 4;
				int num4 = (ia + i) % ca.nverts * 4;
				array[num3] = ca.verts[num4];
				array[num3 + 1] = ca.verts[num4 + 1];
				array[num3 + 2] = ca.verts[num4 + 2];
				array[num3 + 3] = ca.verts[num4 + 3];
				num2++;
			}
			for (int j = 0; j <= cb.nverts; j++)
			{
				int num5 = num2 * 4;
				int num6 = (ib + j) % cb.nverts * 4;
				array[num5] = cb.verts[num6];
				array[num5 + 1] = cb.verts[num6 + 1];
				array[num5 + 2] = cb.verts[num6 + 2];
				array[num5 + 3] = cb.verts[num6 + 3];
				num2++;
			}
			Voxelize.ReleaseIntArr(ca.verts);
			Voxelize.ReleaseIntArr(cb.verts);
			ca.verts = array;
			ca.nverts = num2;
			cb.verts = Voxelize.emptyArr;
			cb.nverts = 0;
			return true;
		}

		public void SimplifyContour(List<int> verts, List<int> simplified, float maxError, int maxEdgeLenght, int buildFlags)
		{
			bool flag = false;
			for (int i = 0; i < verts.get_Count(); i += 4)
			{
				if ((verts.get_Item(i + 3) & 65535) != 0)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				int j = 0;
				int num = verts.get_Count() / 4;
				while (j < num)
				{
					int num2 = (j + 1) % num;
					bool flag2 = (verts.get_Item(j * 4 + 3) & 65535) != (verts.get_Item(num2 * 4 + 3) & 65535);
					bool flag3 = (verts.get_Item(j * 4 + 3) & 131072) != (verts.get_Item(num2 * 4 + 3) & 131072);
					if (flag2 || flag3)
					{
						simplified.Add(verts.get_Item(j * 4));
						simplified.Add(verts.get_Item(j * 4 + 1));
						simplified.Add(verts.get_Item(j * 4 + 2));
						simplified.Add(j);
					}
					j++;
				}
			}
			if (simplified.get_Count() == 0)
			{
				int num3 = verts.get_Item(0);
				int num4 = verts.get_Item(1);
				int num5 = verts.get_Item(2);
				int num6 = 0;
				int num7 = verts.get_Item(0);
				int num8 = verts.get_Item(1);
				int num9 = verts.get_Item(2);
				int num10 = 0;
				for (int k = 0; k < verts.get_Count(); k += 4)
				{
					int num11 = verts.get_Item(k);
					int num12 = verts.get_Item(k + 1);
					int num13 = verts.get_Item(k + 2);
					if (num11 < num3 || (num11 == num3 && num13 < num5))
					{
						num3 = num11;
						num4 = num12;
						num5 = num13;
						num6 = k / 4;
					}
					if (num11 > num7 || (num11 == num7 && num13 > num9))
					{
						num7 = num11;
						num8 = num12;
						num9 = num13;
						num10 = k / 4;
					}
				}
				simplified.Add(num3);
				simplified.Add(num4);
				simplified.Add(num5);
				simplified.Add(num6);
				simplified.Add(num7);
				simplified.Add(num8);
				simplified.Add(num9);
				simplified.Add(num10);
			}
			int num14 = verts.get_Count() / 4;
			maxError *= maxError;
			int l = 0;
			while (l < simplified.get_Count() / 4)
			{
				int num15 = (l + 1) % (simplified.get_Count() / 4);
				int num16 = simplified.get_Item(l * 4);
				int num17 = simplified.get_Item(l * 4 + 2);
				int num18 = simplified.get_Item(l * 4 + 3);
				int num19 = simplified.get_Item(num15 * 4);
				int num20 = simplified.get_Item(num15 * 4 + 2);
				int num21 = simplified.get_Item(num15 * 4 + 3);
				float num22 = 0f;
				int num23 = -1;
				int num24;
				int num25;
				int num26;
				if (num19 > num16 || (num19 == num16 && num20 > num17))
				{
					num24 = 1;
					num25 = (num18 + num24) % num14;
					num26 = num21;
				}
				else
				{
					num24 = num14 - 1;
					num25 = (num21 + num24) % num14;
					num26 = num18;
				}
				if ((verts.get_Item(num25 * 4 + 3) & 65535) == 0 || (verts.get_Item(num25 * 4 + 3) & 131072) == 131072)
				{
					while (num25 != num26)
					{
						float num27 = AstarMath.DistancePointSegment(verts.get_Item(num25 * 4), verts.get_Item(num25 * 4 + 2) / this.voxelArea.width, num16, num17 / this.voxelArea.width, num19, num20 / this.voxelArea.width);
						if (num27 > num22)
						{
							num22 = num27;
							num23 = num25;
						}
						num25 = (num25 + num24) % num14;
					}
				}
				if (num23 != -1 && num22 > maxError)
				{
					simplified.Add(0);
					simplified.Add(0);
					simplified.Add(0);
					simplified.Add(0);
					int num28 = simplified.get_Count() / 4;
					for (int m = num28 - 1; m > l; m--)
					{
						simplified.set_Item(m * 4, simplified.get_Item((m - 1) * 4));
						simplified.set_Item(m * 4 + 1, simplified.get_Item((m - 1) * 4 + 1));
						simplified.set_Item(m * 4 + 2, simplified.get_Item((m - 1) * 4 + 2));
						simplified.set_Item(m * 4 + 3, simplified.get_Item((m - 1) * 4 + 3));
					}
					simplified.set_Item((l + 1) * 4, verts.get_Item(num23 * 4));
					simplified.set_Item((l + 1) * 4 + 1, verts.get_Item(num23 * 4 + 1));
					simplified.set_Item((l + 1) * 4 + 2, verts.get_Item(num23 * 4 + 2));
					simplified.set_Item((l + 1) * 4 + 3, num23);
				}
				else
				{
					l++;
				}
			}
			float num29 = this.maxEdgeLength / this.cellSize;
			if (num29 > 0f && (buildFlags & 3) != 0)
			{
				int n = 0;
				while (n < simplified.get_Count() / 4)
				{
					if (simplified.get_Count() / 4 > 200)
					{
						break;
					}
					int num30 = (n + 1) % (simplified.get_Count() / 4);
					int num31 = simplified.get_Item(n * 4);
					int num32 = simplified.get_Item(n * 4 + 2);
					int num33 = simplified.get_Item(n * 4 + 3);
					int num34 = simplified.get_Item(num30 * 4);
					int num35 = simplified.get_Item(num30 * 4 + 2);
					int num36 = simplified.get_Item(num30 * 4 + 3);
					int num37 = -1;
					int num38 = (num33 + 1) % num14;
					bool flag4 = false;
					if ((buildFlags & 1) == 1 && (verts.get_Item(num38 * 4 + 3) & 65535) == 0)
					{
						flag4 = true;
					}
					if ((buildFlags & 2) == 1 && (verts.get_Item(num38 * 4 + 3) & 131072) == 1)
					{
						flag4 = true;
					}
					if (flag4)
					{
						int num39 = num34 - num31;
						int num40 = num35 / this.voxelArea.width - num32 / this.voxelArea.width;
						if ((float)(num39 * num39 + num40 * num40) > num29 * num29)
						{
							if (num34 > num31 || (num34 == num31 && num35 > num32))
							{
								int num41 = (num36 < num33) ? (num36 + num14 - num33) : (num36 - num33);
								num37 = (num33 + num41 / 2) % num14;
							}
							else
							{
								int num42 = (num36 < num33) ? (num36 + num14 - num33) : (num36 - num33);
								num37 = (num33 + (num42 + 1) / 2) % num14;
							}
						}
					}
					if (num37 != -1)
					{
						simplified.AddRange(new int[4]);
						int num43 = simplified.get_Count() / 4;
						for (int num44 = num43 - 1; num44 > n; num44--)
						{
							simplified.set_Item(num44 * 4, simplified.get_Item((num44 - 1) * 4));
							simplified.set_Item(num44 * 4 + 1, simplified.get_Item((num44 - 1) * 4 + 1));
							simplified.set_Item(num44 * 4 + 2, simplified.get_Item((num44 - 1) * 4 + 2));
							simplified.set_Item(num44 * 4 + 3, simplified.get_Item((num44 - 1) * 4 + 3));
						}
						simplified.set_Item((n + 1) * 4, verts.get_Item(num37 * 4));
						simplified.set_Item((n + 1) * 4 + 1, verts.get_Item(num37 * 4 + 1));
						simplified.set_Item((n + 1) * 4 + 2, verts.get_Item(num37 * 4 + 2));
						simplified.set_Item((n + 1) * 4 + 3, num37);
					}
					else
					{
						n++;
					}
				}
			}
			for (int num45 = 0; num45 < simplified.get_Count() / 4; num45++)
			{
				int num46 = (simplified.get_Item(num45 * 4 + 3) + 1) % num14;
				int num47 = simplified.get_Item(num45 * 4 + 3);
				simplified.set_Item(num45 * 4 + 3, (verts.get_Item(num46 * 4 + 3) & 65535) | (verts.get_Item(num47 * 4 + 3) & 65536));
			}
		}

		public void WalkContour(int x, int z, int i, ushort[] flags, List<int> verts)
		{
			int num = 0;
			while ((flags[i] & (ushort)(1 << num)) == 0)
			{
				num++;
			}
			int num2 = num;
			int num3 = i;
			int num4 = this.voxelArea.areaTypes[i];
			int num5 = 0;
			while (num5++ < 40000)
			{
				if ((flags[i] & (ushort)(1 << num)) != 0)
				{
					bool flag = false;
					bool flag2 = false;
					int num6 = x;
					int cornerHeight = this.GetCornerHeight(x, z, i, num, ref flag);
					int num7 = z;
					switch (num)
					{
					case 0:
						num7 += this.voxelArea.width;
						break;
					case 1:
						num6++;
						num7 += this.voxelArea.width;
						break;
					case 2:
						num6++;
						break;
					}
					int num8 = 0;
					CompactVoxelSpan compactVoxelSpan = this.voxelArea.compactSpans[i];
					if ((long)compactVoxelSpan.GetConnection(num) != 63L)
					{
						int num9 = x + this.voxelArea.DirectionX[num];
						int num10 = z + this.voxelArea.DirectionZ[num];
						int num11 = (int)(this.voxelArea.compactCells[num9 + num10].index + (uint)compactVoxelSpan.GetConnection(num));
						num8 = this.voxelArea.compactSpans[num11].reg;
						if (num4 != this.voxelArea.areaTypes[num11])
						{
							flag2 = true;
						}
					}
					if (flag)
					{
						num8 |= 65536;
					}
					if (flag2)
					{
						num8 |= 131072;
					}
					verts.Add(num6);
					verts.Add(cornerHeight);
					verts.Add(num7);
					verts.Add(num8);
					flags[i] = (ushort)((int)flags[i] & ~(1 << num));
					num = (num + 1 & 3);
				}
				else
				{
					int num12 = -1;
					int num13 = x + this.voxelArea.DirectionX[num];
					int num14 = z + this.voxelArea.DirectionZ[num];
					CompactVoxelSpan compactVoxelSpan2 = this.voxelArea.compactSpans[i];
					if ((long)compactVoxelSpan2.GetConnection(num) != 63L)
					{
						CompactVoxelCell compactVoxelCell = this.voxelArea.compactCells[num13 + num14];
						num12 = (int)(compactVoxelCell.index + (uint)compactVoxelSpan2.GetConnection(num));
					}
					if (num12 == -1)
					{
						Debug.LogError("This should not happen");
						return;
					}
					x = num13;
					z = num14;
					i = num12;
					num = (num + 3 & 3);
				}
				if (num3 == i && num2 == num)
				{
					break;
				}
			}
		}

		public int GetCornerHeight(int x, int z, int i, int dir, ref bool isBorderVertex)
		{
			CompactVoxelSpan compactVoxelSpan = this.voxelArea.compactSpans[i];
			int num = (int)compactVoxelSpan.y;
			int num2 = dir + 1 & 3;
			uint[] array = new uint[4];
			array[0] = (uint)(this.voxelArea.compactSpans[i].reg | this.voxelArea.areaTypes[i] << 16);
			if ((long)compactVoxelSpan.GetConnection(dir) != 63L)
			{
				int num3 = x + this.voxelArea.DirectionX[dir];
				int num4 = z + this.voxelArea.DirectionZ[dir];
				int num5 = (int)(this.voxelArea.compactCells[num3 + num4].index + (uint)compactVoxelSpan.GetConnection(dir));
				CompactVoxelSpan compactVoxelSpan2 = this.voxelArea.compactSpans[num5];
				num = AstarMath.Max(num, (int)compactVoxelSpan2.y);
				array[1] = (uint)(compactVoxelSpan2.reg | this.voxelArea.areaTypes[num5] << 16);
				if ((long)compactVoxelSpan2.GetConnection(num2) != 63L)
				{
					int num6 = num3 + this.voxelArea.DirectionX[num2];
					int num7 = num4 + this.voxelArea.DirectionZ[num2];
					int num8 = (int)(this.voxelArea.compactCells[num6 + num7].index + (uint)compactVoxelSpan2.GetConnection(num2));
					CompactVoxelSpan compactVoxelSpan3 = this.voxelArea.compactSpans[num8];
					num = AstarMath.Max(num, (int)compactVoxelSpan3.y);
					array[2] = (uint)(compactVoxelSpan3.reg | this.voxelArea.areaTypes[num8] << 16);
				}
			}
			if ((long)compactVoxelSpan.GetConnection(num2) != 63L)
			{
				int num9 = x + this.voxelArea.DirectionX[num2];
				int num10 = z + this.voxelArea.DirectionZ[num2];
				int num11 = (int)(this.voxelArea.compactCells[num9 + num10].index + (uint)compactVoxelSpan.GetConnection(num2));
				CompactVoxelSpan compactVoxelSpan4 = this.voxelArea.compactSpans[num11];
				num = AstarMath.Max(num, (int)compactVoxelSpan4.y);
				array[3] = (uint)(compactVoxelSpan4.reg | this.voxelArea.areaTypes[num11] << 16);
				if ((long)compactVoxelSpan4.GetConnection(dir) != 63L)
				{
					int num12 = num9 + this.voxelArea.DirectionX[dir];
					int num13 = num10 + this.voxelArea.DirectionZ[dir];
					int num14 = (int)(this.voxelArea.compactCells[num12 + num13].index + (uint)compactVoxelSpan4.GetConnection(dir));
					CompactVoxelSpan compactVoxelSpan5 = this.voxelArea.compactSpans[num14];
					num = AstarMath.Max(num, (int)compactVoxelSpan5.y);
					array[2] = (uint)(compactVoxelSpan5.reg | this.voxelArea.areaTypes[num14] << 16);
				}
			}
			for (int j = 0; j < 4; j++)
			{
				int num15 = j;
				int num16 = j + 1 & 3;
				int num17 = j + 2 & 3;
				int num18 = j + 3 & 3;
				bool flag = (array[num15] & array[num16] & 32768u) != 0u && array[num15] == array[num16];
				bool flag2 = ((array[num17] | array[num18]) & 32768u) == 0u;
				bool flag3 = array[num17] >> 16 == array[num18] >> 16;
				bool flag4 = array[num15] != 0u && array[num16] != 0u && array[num17] != 0u && array[num18] != 0u;
				if (flag && flag2 && flag3 && flag4)
				{
					isBorderVertex = true;
					break;
				}
			}
			return num;
		}

		public void RemoveDegenerateSegments(List<int> simplified)
		{
			for (int i = 0; i < simplified.get_Count() / 4; i++)
			{
				int num = i + 1;
				if (num >= simplified.get_Count() / 4)
				{
					num = 0;
				}
				if (simplified.get_Item(i * 4) == simplified.get_Item(num * 4) && simplified.get_Item(i * 4 + 2) == simplified.get_Item(num * 4 + 2))
				{
					simplified.RemoveRange(i, 4);
				}
			}
		}

		public int CalcAreaOfPolygon2D(int[] verts, int nverts)
		{
			int num = 0;
			int i = 0;
			int num2 = nverts - 1;
			while (i < nverts)
			{
				int num3 = i * 4;
				int num4 = num2 * 4;
				num += verts[num3] * (verts[num4 + 2] / this.voxelArea.width) - verts[num4] * (verts[num3 + 2] / this.voxelArea.width);
				num2 = i++;
			}
			return (num + 1) / 2;
		}

		public static bool Ileft(int a, int b, int c, int[] va, int[] vb, int[] vc)
		{
			return (vb[b] - va[a]) * (vc[c + 2] - va[a + 2]) - (vc[c] - va[a]) * (vb[b + 2] - va[a + 2]) <= 0;
		}

		public static bool Diagonal(int i, int j, int n, int[] verts, int[] indices)
		{
			return Voxelize.InCone(i, j, n, verts, indices) && Voxelize.Diagonalie(i, j, n, verts, indices);
		}

		public static bool InCone(int i, int j, int n, int[] verts, int[] indices)
		{
			int num = (indices[i] & 268435455) * 4;
			int num2 = (indices[j] & 268435455) * 4;
			int c = (indices[Voxelize.Next(i, n)] & 268435455) * 4;
			int num3 = (indices[Voxelize.Prev(i, n)] & 268435455) * 4;
			if (Voxelize.LeftOn(num3, num, c, verts))
			{
				return Voxelize.Left(num, num2, num3, verts) && Voxelize.Left(num2, num, c, verts);
			}
			return !Voxelize.LeftOn(num, num2, c, verts) || !Voxelize.LeftOn(num2, num, num3, verts);
		}

		public static bool Left(int a, int b, int c, int[] verts)
		{
			return Voxelize.Area2(a, b, c, verts) < 0;
		}

		public static bool LeftOn(int a, int b, int c, int[] verts)
		{
			return Voxelize.Area2(a, b, c, verts) <= 0;
		}

		public static bool Collinear(int a, int b, int c, int[] verts)
		{
			return Voxelize.Area2(a, b, c, verts) == 0;
		}

		public static int Area2(int a, int b, int c, int[] verts)
		{
			return (verts[b] - verts[a]) * (verts[c + 2] - verts[a + 2]) - (verts[c] - verts[a]) * (verts[b + 2] - verts[a + 2]);
		}

		private static bool Diagonalie(int i, int j, int n, int[] verts, int[] indices)
		{
			int a = (indices[i] & 268435455) * 4;
			int num = (indices[j] & 268435455) * 4;
			for (int k = 0; k < n; k++)
			{
				int num2 = Voxelize.Next(k, n);
				if (k != i && num2 != i && k != j && num2 != j)
				{
					int num3 = (indices[k] & 268435455) * 4;
					int num4 = (indices[num2] & 268435455) * 4;
					if (!Voxelize.Vequal(a, num3, verts) && !Voxelize.Vequal(num, num3, verts) && !Voxelize.Vequal(a, num4, verts) && !Voxelize.Vequal(num, num4, verts) && Voxelize.Intersect(a, num, num3, num4, verts))
					{
						return false;
					}
				}
			}
			return true;
		}

		public static bool Xorb(bool x, bool y)
		{
			return !x ^ !y;
		}

		public static bool IntersectProp(int a, int b, int c, int d, int[] verts)
		{
			return !Voxelize.Collinear(a, b, c, verts) && !Voxelize.Collinear(a, b, d, verts) && !Voxelize.Collinear(c, d, a, verts) && !Voxelize.Collinear(c, d, b, verts) && Voxelize.Xorb(Voxelize.Left(a, b, c, verts), Voxelize.Left(a, b, d, verts)) && Voxelize.Xorb(Voxelize.Left(c, d, a, verts), Voxelize.Left(c, d, b, verts));
		}

		private static bool Between(int a, int b, int c, int[] verts)
		{
			if (!Voxelize.Collinear(a, b, c, verts))
			{
				return false;
			}
			if (verts[a] != verts[b])
			{
				return (verts[a] <= verts[c] && verts[c] <= verts[b]) || (verts[a] >= verts[c] && verts[c] >= verts[b]);
			}
			return (verts[a + 2] <= verts[c + 2] && verts[c + 2] <= verts[b + 2]) || (verts[a + 2] >= verts[c + 2] && verts[c + 2] >= verts[b + 2]);
		}

		private static bool Intersect(int a, int b, int c, int d, int[] verts)
		{
			return Voxelize.IntersectProp(a, b, c, d, verts) || Voxelize.Between(a, b, c, verts) || Voxelize.Between(a, b, d, verts) || Voxelize.Between(c, d, a, verts) || Voxelize.Between(c, d, b, verts);
		}

		private static bool Vequal(int a, int b, int[] verts)
		{
			return verts[a] == verts[b] && verts[a + 2] == verts[b + 2];
		}

		public static int Prev(int i, int n)
		{
			return (i - 1 >= 0) ? (i - 1) : (n - 1);
		}

		public static int Next(int i, int n)
		{
			return (i + 1 < n) ? (i + 1) : 0;
		}

		public void BuildPolyMesh(VoxelContourSet cset, int nvp, out VoxelMesh mesh)
		{
			nvp = 3;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			for (int i = 0; i < cset.conts.get_Count(); i++)
			{
				if (cset.conts.get_Item(i).nverts >= 3)
				{
					num += cset.conts.get_Item(i).nverts;
					num2 += cset.conts.get_Item(i).nverts - 2;
					num3 = AstarMath.Max(num3, cset.conts.get_Item(i).nverts);
				}
			}
			if (num >= 65534)
			{
				Debug.LogWarning("To many vertices for unity to render - Unity might screw up rendering, but hopefully the navmesh will work ok");
			}
			VInt3[] array = new VInt3[num];
			int[] array2 = new int[num2 * nvp];
			Memory.MemSet<int>(array2, 255, 4);
			int[] array3 = new int[num3];
			int[] array4 = new int[num3 * 3];
			int num4 = 0;
			int num5 = 0;
			for (int j = 0; j < cset.conts.get_Count(); j++)
			{
				VoxelContour voxelContour = cset.conts.get_Item(j);
				if (voxelContour.nverts >= 3)
				{
					for (int k = 0; k < voxelContour.nverts; k++)
					{
						array3[k] = k;
						voxelContour.verts[k * 4 + 2] /= this.voxelArea.width;
					}
					int num6 = this.Triangulate(voxelContour.nverts, voxelContour.verts, ref array3, ref array4);
					int num7 = num4;
					for (int l = 0; l < num6 * 3; l++)
					{
						array2[num5] = array4[l] + num7;
						num5++;
					}
					for (int m = 0; m < voxelContour.nverts; m++)
					{
						array[num4] = new VInt3(voxelContour.verts[m * 4], voxelContour.verts[m * 4 + 1], voxelContour.verts[m * 4 + 2]);
						num4++;
					}
				}
			}
			mesh = default(VoxelMesh);
			VInt3[] array5 = new VInt3[num4];
			for (int n = 0; n < num4; n++)
			{
				array5[n] = array[n];
			}
			int[] array6 = new int[num5];
			Buffer.BlockCopy(array2, 0, array6, 0, num5 * 4);
			mesh.verts = array5;
			mesh.tris = array6;
		}

		public int Triangulate(int n, int[] verts, ref int[] indices, ref int[] tris)
		{
			int num = 0;
			int[] array = tris;
			int num2 = 0;
			int num3 = n;
			for (int i = 0; i < n; i++)
			{
				int num4 = Voxelize.Next(i, n);
				int j = Voxelize.Next(num4, n);
				if (Voxelize.Diagonal(i, j, n, verts, indices))
				{
					indices[num4] |= 1073741824;
				}
			}
			while (n > 3)
			{
				int num5 = -1;
				int num6 = -1;
				for (int k = 0; k < n; k++)
				{
					int num7 = Voxelize.Next(k, n);
					if ((indices[num7] & 1073741824) != 0)
					{
						int num8 = (indices[k] & 268435455) * 4;
						int num9 = (indices[Voxelize.Next(num7, n)] & 268435455) * 4;
						int num10 = verts[num9] - verts[num8];
						int num11 = verts[num9 + 2] - verts[num8 + 2];
						int num12 = num10 * num10 + num11 * num11;
						if (num5 < 0 || num12 < num5)
						{
							num5 = num12;
							num6 = k;
						}
					}
				}
				if (num6 == -1)
				{
					Debug.LogError("This should not happen");
					for (int l = 0; l < num3; l++)
					{
						this.DrawLine(Voxelize.Prev(l, num3), l, indices, verts, Color.red);
					}
					return -num;
				}
				int num13 = num6;
				int num14 = Voxelize.Next(num13, n);
				int num15 = Voxelize.Next(num14, n);
				array[num2] = (indices[num13] & 268435455);
				num2++;
				array[num2] = (indices[num14] & 268435455);
				num2++;
				array[num2] = (indices[num15] & 268435455);
				num2++;
				num++;
				n--;
				for (int m = num14; m < n; m++)
				{
					indices[m] = indices[m + 1];
				}
				if (num14 >= n)
				{
					num14 = 0;
				}
				num13 = Voxelize.Prev(num14, n);
				if (Voxelize.Diagonal(Voxelize.Prev(num13, n), num14, n, verts, indices))
				{
					indices[num13] |= 1073741824;
				}
				else
				{
					indices[num13] &= 268435455;
				}
				if (Voxelize.Diagonal(num13, Voxelize.Next(num14, n), n, verts, indices))
				{
					indices[num14] |= 1073741824;
				}
				else
				{
					indices[num14] &= 268435455;
				}
			}
			array[num2] = (indices[0] & 268435455);
			num2++;
			array[num2] = (indices[1] & 268435455);
			num2++;
			array[num2] = (indices[2] & 268435455);
			num2++;
			return num + 1;
		}

		public Vector3 CompactSpanToVector(int x, int z, int i)
		{
			return this.voxelOffset + new Vector3((float)x * this.cellSize, (float)this.voxelArea.compactSpans[i].y * this.cellHeight, (float)z * this.cellSize);
		}

		public void VectorToIndex(Vector3 p, out int x, out int z)
		{
			p -= this.voxelOffset;
			x = Mathf.RoundToInt(p.x / this.cellSize);
			z = Mathf.RoundToInt(p.z / this.cellSize);
		}

		public void OnGUI()
		{
			GUI.Label(new Rect(5f, 5f, 200f, (float)Screen.height), this.debugString);
		}

		public void CollectMeshes()
		{
			Voxelize.CollectMeshes(this.inputExtraMeshes, this.forcedBounds, out this.inputVertices, out this.inputTriangles);
		}

		public static void CollectMeshes(List<ExtraMesh> extraMeshes, Bounds bounds, out Vector3[] verts, out int[] tris)
		{
			verts = null;
			tris = null;
		}

		public void Init()
		{
			if (this.voxelArea == null || this.voxelArea.width != this.width || this.voxelArea.depth != this.depth)
			{
				this.voxelArea = new VoxelArea(this.width, this.depth);
			}
			else
			{
				this.voxelArea.Reset();
			}
		}

		public void VoxelizeInput()
		{
			Vector3 min = this.forcedBounds.min;
			this.voxelOffset = min;
			float num = 1f / this.cellSize;
			float num2 = 1f / this.cellHeight;
			float num3 = Mathf.Cos(Mathf.Atan(Mathf.Tan(this.maxSlope * 0.0174532924f) * (num2 * this.cellSize)));
			float[] array = new float[9];
			float[] array2 = new float[21];
			float[] array3 = new float[21];
			float[] array4 = new float[21];
			float[] array5 = new float[21];
			if (this.inputExtraMeshes == null)
			{
				throw new NullReferenceException("inputExtraMeshes not set");
			}
			int num4 = 0;
			for (int i = 0; i < this.inputExtraMeshes.get_Count(); i++)
			{
				if (this.inputExtraMeshes.get_Item(i).bounds.Intersects(this.forcedBounds))
				{
					num4 = Math.Max(this.inputExtraMeshes.get_Item(i).vertices.Length, num4);
				}
			}
			Vector3[] array6 = new Vector3[num4];
			Matrix4x4 lhs = Matrix4x4.Scale(new Vector3(num, num2, num)) * Matrix4x4.TRS(-min, Quaternion.identity, Vector3.one);
			for (int j = 0; j < this.inputExtraMeshes.get_Count(); j++)
			{
				ExtraMesh extraMesh = this.inputExtraMeshes.get_Item(j);
				bool flag = MMGame_Math.isMirror(extraMesh.matrix);
				if (flag)
				{
					Debug.Log(string.Format("GameObject {0} is mirrored!", extraMesh.name));
				}
				if (extraMesh.bounds.Intersects(this.forcedBounds))
				{
					Matrix4x4 rhs = extraMesh.matrix;
					rhs = lhs * rhs;
					Vector3[] vertices = extraMesh.vertices;
					int[] triangles = extraMesh.triangles;
					int num5 = triangles.Length;
					for (int k = 0; k < vertices.Length; k++)
					{
						array6[k] = rhs.MultiplyPoint3x4(vertices[k]);
					}
					int area = extraMesh.area;
					for (int l = 0; l < num5; l += 3)
					{
						Vector3 vector;
						Vector3 vector2;
						Vector3 vector3;
						if (flag)
						{
							vector = array6[triangles[l]];
							vector2 = array6[triangles[l + 2]];
							vector3 = array6[triangles[l + 1]];
						}
						else
						{
							vector = array6[triangles[l]];
							vector2 = array6[triangles[l + 1]];
							vector3 = array6[triangles[l + 2]];
						}
						int num6 = (int)Utility.Min(vector.x, vector2.x, vector3.x);
						int num7 = (int)Utility.Min(vector.z, vector2.z, vector3.z);
						int num8 = (int)Math.Ceiling((double)Utility.Max(vector.x, vector2.x, vector3.x));
						int num9 = (int)Math.Ceiling((double)Utility.Max(vector.z, vector2.z, vector3.z));
						num6 = Mathf.Clamp(num6, 0, this.voxelArea.width - 1);
						num8 = Mathf.Clamp(num8, 0, this.voxelArea.width - 1);
						num7 = Mathf.Clamp(num7, 0, this.voxelArea.depth - 1);
						num9 = Mathf.Clamp(num9, 0, this.voxelArea.depth - 1);
						if (num6 < this.voxelArea.width && num7 < this.voxelArea.depth && num8 > 0 && num9 > 0)
						{
							float num10 = Vector3.Dot(Vector3.Cross(vector2 - vector, vector3 - vector).normalized, Vector3.up);
							int area2;
							if (num10 < num3)
							{
								area2 = 0;
							}
							else
							{
								area2 = 1 + area;
							}
							Utility.CopyVector(array, 0, vector);
							Utility.CopyVector(array, 3, vector2);
							Utility.CopyVector(array, 6, vector3);
							for (int m = num6; m <= num8; m++)
							{
								int num11 = Utility.ClipPolygon(array, 3, array2, 1f, -(float)m + 0.5f, 0);
								if (num11 >= 3)
								{
									num11 = Utility.ClipPolygon(array2, num11, array3, -1f, (float)m + 0.5f, 0);
									if (num11 >= 3)
									{
										float num12 = array3[2];
										float num13 = array3[2];
										for (int n = 1; n < num11; n++)
										{
											float num14 = array3[n * 3 + 2];
											num12 = Math.Min(num12, num14);
											num13 = Math.Max(num13, num14);
										}
										int num15 = AstarMath.Clamp(MMGame_Math.RoundToInt((double)num12), 0, this.voxelArea.depth - 1);
										int num16 = AstarMath.Clamp(MMGame_Math.RoundToInt((double)num13), 0, this.voxelArea.depth - 1);
										for (int num17 = num15; num17 <= num16; num17++)
										{
											int num18 = Utility.ClipPolygon(array3, num11, array4, 1f, -(float)num17 + 0.5f, 2);
											if (num18 >= 3)
											{
												num18 = Utility.ClipPolygonY(array4, num18, array5, -1f, (float)num17 + 0.5f, 2);
												if (num18 >= 3)
												{
													float num19 = array5[1];
													float num20 = array5[1];
													for (int num21 = 1; num21 < num18; num21++)
													{
														float num22 = array5[num21 * 3 + 1];
														num19 = Math.Min(num19, num22);
														num20 = Math.Max(num20, num22);
													}
													int num23 = (int)Math.Ceiling((double)num20);
													if (num23 >= 0)
													{
														int num24 = (int)(num19 + 1f);
														this.voxelArea.AddLinkedSpan(num17 * this.voxelArea.width + m, (uint)((num24 >= 0) ? num24 : 0), (uint)num23, area2, this.voxelWalkableClimb);
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		public void BuildCompactField()
		{
			int spanCount = this.voxelArea.GetSpanCount();
			this.voxelArea.compactSpanCount = spanCount;
			if (this.voxelArea.compactSpans == null || this.voxelArea.compactSpans.Length < spanCount)
			{
				this.voxelArea.compactSpans = new CompactVoxelSpan[spanCount];
				this.voxelArea.areaTypes = new int[spanCount];
			}
			uint num = 0u;
			int num2 = this.voxelArea.width;
			int num3 = this.voxelArea.depth;
			int num4 = num2 * num3;
			if (this.voxelWalkableHeight >= 65535u)
			{
				Debug.LogWarning("Too high walkable height to guarantee correctness. Increase voxel height or lower walkable height.");
			}
			LinkedVoxelSpan[] linkedSpans = this.voxelArea.linkedSpans;
			int i = 0;
			int num5 = 0;
			while (i < num4)
			{
				for (int j = 0; j < num2; j++)
				{
					int num6 = j + i;
					if (linkedSpans[num6].bottom == 4294967295u)
					{
						this.voxelArea.compactCells[j + i] = new CompactVoxelCell(0u, 0u);
					}
					else
					{
						uint i2 = num;
						uint num7 = 0u;
						while (num6 != -1)
						{
							if (linkedSpans[num6].area != 0)
							{
								int top = (int)linkedSpans[num6].top;
								int next = linkedSpans[num6].next;
								int num8 = (int)((next != -1) ? linkedSpans[next].bottom : 65536u);
								this.voxelArea.compactSpans[(int)((uint)((UIntPtr)num))] = new CompactVoxelSpan((ushort)((top > 65535) ? 65535 : top), (uint)((num8 - top > 65535) ? 65535 : (num8 - top)));
								this.voxelArea.areaTypes[(int)((uint)((UIntPtr)num))] = linkedSpans[num6].area;
								num += 1u;
								num7 += 1u;
							}
							num6 = linkedSpans[num6].next;
						}
						this.voxelArea.compactCells[j + i] = new CompactVoxelCell(i2, num7);
					}
				}
				i += num2;
				num5++;
			}
		}

		public void BuildVoxelConnections()
		{
			int num = this.voxelArea.width * this.voxelArea.depth;
			CompactVoxelSpan[] compactSpans = this.voxelArea.compactSpans;
			CompactVoxelCell[] compactCells = this.voxelArea.compactCells;
			int i = 0;
			int num2 = 0;
			while (i < num)
			{
				for (int j = 0; j < this.voxelArea.width; j++)
				{
					CompactVoxelCell compactVoxelCell = compactCells[j + i];
					int k = (int)compactVoxelCell.index;
					int num3 = (int)(compactVoxelCell.index + compactVoxelCell.count);
					while (k < num3)
					{
						CompactVoxelSpan compactVoxelSpan = compactSpans[k];
						compactSpans[k].con = 4294967295u;
						for (int l = 0; l < 4; l++)
						{
							int num4 = j + this.voxelArea.DirectionX[l];
							int num5 = i + this.voxelArea.DirectionZ[l];
							if (num4 >= 0 && num5 >= 0 && num5 < num && num4 < this.voxelArea.width)
							{
								CompactVoxelCell compactVoxelCell2 = compactCells[num4 + num5];
								int m = (int)compactVoxelCell2.index;
								int num6 = (int)(compactVoxelCell2.index + compactVoxelCell2.count);
								while (m < num6)
								{
									CompactVoxelSpan compactVoxelSpan2 = compactSpans[m];
									int num7 = (int)Math.Max(compactVoxelSpan.y, compactVoxelSpan2.y);
									int num8 = AstarMath.Min((int)((uint)compactVoxelSpan.y + compactVoxelSpan.h), (int)((uint)compactVoxelSpan2.y + compactVoxelSpan2.h));
									if ((long)(num8 - num7) >= (long)((ulong)this.voxelWalkableHeight) && Math.Abs((int)(compactVoxelSpan2.y - compactVoxelSpan.y)) <= this.voxelWalkableClimb)
									{
										uint num9 = (uint)(m - (int)compactVoxelCell2.index);
										if (num9 <= 65535u)
										{
											compactSpans[k].SetConnection(l, num9);
											break;
										}
										Debug.LogError("Too many layers");
									}
									m++;
								}
							}
						}
						k++;
					}
				}
				i += this.voxelArea.width;
				num2++;
			}
		}

		public void DrawLine(int a, int b, int[] indices, int[] verts, Color col)
		{
			int num = (indices[a] & 268435455) * 4;
			int num2 = (indices[b] & 268435455) * 4;
			Debug.DrawLine(this.ConvertPosCorrZ(verts[num], verts[num + 1], verts[num + 2]), this.ConvertPosCorrZ(verts[num2], verts[num2 + 1], verts[num2 + 2]), col);
		}

		public Vector3 ConvertPos(int x, int y, int z)
		{
			return Vector3.Scale(new Vector3((float)x + 0.5f, (float)y, (float)z / (float)this.voxelArea.width + 0.5f), this.cellScale) + this.voxelOffset;
		}

		public Vector3 ConvertPosCorrZ(int x, int y, int z)
		{
			return Vector3.Scale(new Vector3((float)x, (float)y, (float)z), this.cellScale) + this.voxelOffset;
		}

		public Vector3 ConvertPosWithoutOffset(int x, int y, int z)
		{
			return Vector3.Scale(new Vector3((float)x, (float)y, (float)z / (float)this.voxelArea.width), this.cellScale) + this.voxelOffset;
		}

		public Vector3 ConvertPosition(int x, int z, int i)
		{
			CompactVoxelSpan compactVoxelSpan = this.voxelArea.compactSpans[i];
			return new Vector3((float)x * this.cellSize, (float)compactVoxelSpan.y * this.cellHeight, (float)z / (float)this.voxelArea.width * this.cellSize) + this.voxelOffset;
		}

		public void ErodeWalkableArea(int radius)
		{
			ushort[] array = this.voxelArea.tmpUShortArr;
			if (array == null || array.Length < this.voxelArea.compactSpanCount)
			{
				array = (this.voxelArea.tmpUShortArr = new ushort[this.voxelArea.compactSpanCount]);
			}
			Memory.MemSet<ushort>(array, 65535, 2);
			this.CalculateDistanceField(array);
			for (int i = 0; i < array.Length; i++)
			{
				if ((int)array[i] < radius * 2)
				{
					this.voxelArea.areaTypes[i] = 0;
				}
			}
		}

		public void BuildDistanceField()
		{
			ushort[] array = this.voxelArea.tmpUShortArr;
			if (array == null || array.Length < this.voxelArea.compactSpanCount)
			{
				array = (this.voxelArea.tmpUShortArr = new ushort[this.voxelArea.compactSpanCount]);
			}
			Memory.MemSet<ushort>(array, 65535, 2);
			this.voxelArea.maxDistance = this.CalculateDistanceField(array);
			ushort[] array2 = this.voxelArea.dist;
			if (array2 == null || array2.Length < this.voxelArea.compactSpanCount)
			{
				array2 = new ushort[this.voxelArea.compactSpanCount];
			}
			array2 = this.BoxBlur(array, array2);
			this.voxelArea.dist = array2;
		}

		[Obsolete("This function is not complete and should not be used")]
		public void ErodeVoxels(int radius)
		{
			if (radius > 255)
			{
				Debug.LogError("Max Erode Radius is 255");
				radius = 255;
			}
			int num = this.voxelArea.width * this.voxelArea.depth;
			int[] array = new int[this.voxelArea.compactSpanCount];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = 255;
			}
			for (int j = 0; j < num; j += this.voxelArea.width)
			{
				for (int k = 0; k < this.voxelArea.width; k++)
				{
					CompactVoxelCell compactVoxelCell = this.voxelArea.compactCells[k + j];
					int l = (int)compactVoxelCell.index;
					int num2 = (int)(compactVoxelCell.index + compactVoxelCell.count);
					while (l < num2)
					{
						if (this.voxelArea.areaTypes[l] != 0)
						{
							CompactVoxelSpan compactVoxelSpan = this.voxelArea.compactSpans[l];
							int num3 = 0;
							for (int m = 0; m < 4; m++)
							{
								if ((long)compactVoxelSpan.GetConnection(m) != 63L)
								{
									num3++;
								}
							}
							if (num3 != 4)
							{
								array[l] = 0;
							}
						}
						l++;
					}
				}
			}
		}

		public void FilterLowHeightSpans(uint voxelWalkableHeight, float cs, float ch, Vector3 min)
		{
			int num = this.voxelArea.width * this.voxelArea.depth;
			LinkedVoxelSpan[] linkedSpans = this.voxelArea.linkedSpans;
			int i = 0;
			int num2 = 0;
			while (i < num)
			{
				for (int j = 0; j < this.voxelArea.width; j++)
				{
					int num3 = i + j;
					while (num3 != -1 && linkedSpans[num3].bottom != 4294967295u)
					{
						uint top = linkedSpans[num3].top;
						uint num4 = (linkedSpans[num3].next != -1) ? linkedSpans[linkedSpans[num3].next].bottom : 65536u;
						if (num4 - top < voxelWalkableHeight)
						{
							linkedSpans[num3].area = 0;
						}
						num3 = linkedSpans[num3].next;
					}
				}
				i += this.voxelArea.width;
				num2++;
			}
		}

		public void FilterLedges(uint voxelWalkableHeight, int voxelWalkableClimb, float cs, float ch, Vector3 min)
		{
			int num = this.voxelArea.width * this.voxelArea.depth;
			LinkedVoxelSpan[] linkedSpans = this.voxelArea.linkedSpans;
			int[] directionX = this.voxelArea.DirectionX;
			int[] directionZ = this.voxelArea.DirectionZ;
			int num2 = this.voxelArea.width;
			int i = 0;
			int num3 = 0;
			while (i < num)
			{
				for (int j = 0; j < num2; j++)
				{
					if (linkedSpans[j + i].bottom != 4294967295u)
					{
						for (int num4 = j + i; num4 != -1; num4 = linkedSpans[num4].next)
						{
							if (linkedSpans[num4].area != 0)
							{
								int top = (int)linkedSpans[num4].top;
								int num5 = (int)((linkedSpans[num4].next != -1) ? linkedSpans[linkedSpans[num4].next].bottom : 65536u);
								int num6 = 65536;
								int num7 = (int)linkedSpans[num4].top;
								int num8 = num7;
								for (int k = 0; k < 4; k++)
								{
									int num9 = j + directionX[k];
									int num10 = i + directionZ[k];
									if (num9 < 0 || num10 < 0 || num10 >= num || num9 >= num2)
									{
										linkedSpans[num4].area = 0;
										break;
									}
									int num11 = num9 + num10;
									int num12 = -voxelWalkableClimb;
									int num13 = (int)((linkedSpans[num11].bottom != 4294967295u) ? linkedSpans[num11].bottom : 65536u);
									if ((long)(Math.Min(num5, num13) - Math.Max(top, num12)) > (long)((ulong)voxelWalkableHeight))
									{
										num6 = Math.Min(num6, num12 - top);
									}
									if (linkedSpans[num11].bottom != 4294967295u)
									{
										for (int num14 = num11; num14 != -1; num14 = linkedSpans[num14].next)
										{
											num12 = (int)linkedSpans[num14].top;
											num13 = (int)((linkedSpans[num14].next != -1) ? linkedSpans[linkedSpans[num14].next].bottom : 65536u);
											if ((long)(Math.Min(num5, num13) - Math.Max(top, num12)) > (long)((ulong)voxelWalkableHeight))
											{
												num6 = AstarMath.Min(num6, num12 - top);
												if (Math.Abs(num12 - top) <= voxelWalkableClimb)
												{
													if (num12 < num7)
													{
														num7 = num12;
													}
													if (num12 > num8)
													{
														num8 = num12;
													}
												}
											}
										}
									}
								}
								if (num6 < -voxelWalkableClimb || num8 - num7 > voxelWalkableClimb)
								{
									linkedSpans[num4].area = 0;
								}
							}
						}
					}
				}
				i += num2;
				num3++;
			}
		}

		public ushort[] ExpandRegions(int maxIterations, uint level, ushort[] srcReg, ushort[] srcDist, ushort[] dstReg, ushort[] dstDist, List<int> stack)
		{
			int num = this.voxelArea.width;
			int num2 = this.voxelArea.depth;
			int num3 = num * num2;
			stack.Clear();
			int i = 0;
			int num4 = 0;
			while (i < num3)
			{
				for (int j = 0; j < this.voxelArea.width; j++)
				{
					CompactVoxelCell compactVoxelCell = this.voxelArea.compactCells[i + j];
					int k = (int)compactVoxelCell.index;
					int num5 = (int)(compactVoxelCell.index + compactVoxelCell.count);
					while (k < num5)
					{
						if ((uint)this.voxelArea.dist[k] >= level && srcReg[k] == 0 && this.voxelArea.areaTypes[k] != 0)
						{
							stack.Add(j);
							stack.Add(i);
							stack.Add(k);
						}
						k++;
					}
				}
				i += num;
				num4++;
			}
			int num6 = 0;
			int count = stack.get_Count();
			if (count > 0)
			{
				while (true)
				{
					int num7 = 0;
					Buffer.BlockCopy(srcReg, 0, dstReg, 0, srcReg.Length * 2);
					Buffer.BlockCopy(srcDist, 0, dstDist, 0, dstDist.Length * 2);
					for (int l = 0; l < count; l += 3)
					{
						if (l >= count)
						{
							break;
						}
						int num8 = stack.get_Item(l);
						int num9 = stack.get_Item(l + 1);
						int num10 = stack.get_Item(l + 2);
						if (num10 < 0)
						{
							num7++;
						}
						else
						{
							ushort num11 = srcReg[num10];
							ushort num12 = 65535;
							CompactVoxelSpan compactVoxelSpan = this.voxelArea.compactSpans[num10];
							int num13 = this.voxelArea.areaTypes[num10];
							for (int m = 0; m < 4; m++)
							{
								if ((long)compactVoxelSpan.GetConnection(m) != 63L)
								{
									int num14 = num8 + this.voxelArea.DirectionX[m];
									int num15 = num9 + this.voxelArea.DirectionZ[m];
									int num16 = (int)(this.voxelArea.compactCells[num14 + num15].index + (uint)compactVoxelSpan.GetConnection(m));
									if (num13 == this.voxelArea.areaTypes[num16] && srcReg[num16] > 0 && (srcReg[num16] & 32768) == 0 && srcDist[num16] + 2 < num12)
									{
										num11 = srcReg[num16];
										num12 = srcDist[num16] + 2;
									}
								}
							}
							if (num11 != 0)
							{
								stack.set_Item(l + 2, -1);
								dstReg[num10] = num11;
								dstDist[num10] = num12;
							}
							else
							{
								num7++;
							}
						}
					}
					ushort[] array = srcReg;
					srcReg = dstReg;
					dstReg = array;
					array = srcDist;
					srcDist = dstDist;
					dstDist = array;
					if (num7 * 3 >= count)
					{
						break;
					}
					if (level > 0u)
					{
						num6++;
						if (num6 >= maxIterations)
						{
							break;
						}
					}
				}
			}
			return srcReg;
		}

		public bool FloodRegion(int x, int z, int i, uint level, ushort r, ushort[] srcReg, ushort[] srcDist, List<int> stack)
		{
			int num = this.voxelArea.areaTypes[i];
			stack.Clear();
			stack.Add(x);
			stack.Add(z);
			stack.Add(i);
			srcReg[i] = r;
			srcDist[i] = 0;
			int num2 = (int)((level >= 2u) ? (level - 2u) : 0u);
			int num3 = 0;
			while (stack.get_Count() > 0)
			{
				int num4 = stack.get_Item(stack.get_Count() - 1);
				stack.RemoveAt(stack.get_Count() - 1);
				int num5 = stack.get_Item(stack.get_Count() - 1);
				stack.RemoveAt(stack.get_Count() - 1);
				int num6 = stack.get_Item(stack.get_Count() - 1);
				stack.RemoveAt(stack.get_Count() - 1);
				CompactVoxelSpan compactVoxelSpan = this.voxelArea.compactSpans[num4];
				ushort num7 = 0;
				for (int j = 0; j < 4; j++)
				{
					if ((long)compactVoxelSpan.GetConnection(j) != 63L)
					{
						int num8 = num6 + this.voxelArea.DirectionX[j];
						int num9 = num5 + this.voxelArea.DirectionZ[j];
						int num10 = (int)(this.voxelArea.compactCells[num8 + num9].index + (uint)compactVoxelSpan.GetConnection(j));
						if (this.voxelArea.areaTypes[num10] == num)
						{
							ushort num11 = srcReg[num10];
							if ((num11 & 32768) != 32768)
							{
								if (num11 != 0 && num11 != r)
								{
									num7 = num11;
								}
								CompactVoxelSpan compactVoxelSpan2 = this.voxelArea.compactSpans[num10];
								int num12 = j + 1 & 3;
								if ((long)compactVoxelSpan2.GetConnection(num12) != 63L)
								{
									int num13 = num8 + this.voxelArea.DirectionX[num12];
									int num14 = num9 + this.voxelArea.DirectionZ[num12];
									int num15 = (int)(this.voxelArea.compactCells[num13 + num14].index + (uint)compactVoxelSpan2.GetConnection(num12));
									if (this.voxelArea.areaTypes[num15] == num)
									{
										num11 = srcReg[num15];
										if (num11 != 0 && num11 != r)
										{
											num7 = num11;
										}
									}
								}
							}
						}
					}
				}
				if (num7 != 0)
				{
					srcReg[num4] = 0;
				}
				else
				{
					num3++;
					for (int k = 0; k < 4; k++)
					{
						if ((long)compactVoxelSpan.GetConnection(k) != 63L)
						{
							int num16 = num6 + this.voxelArea.DirectionX[k];
							int num17 = num5 + this.voxelArea.DirectionZ[k];
							int num18 = (int)(this.voxelArea.compactCells[num16 + num17].index + (uint)compactVoxelSpan.GetConnection(k));
							if (this.voxelArea.areaTypes[num18] == num && (int)this.voxelArea.dist[num18] >= num2 && srcReg[num18] == 0)
							{
								srcReg[num18] = r;
								srcDist[num18] = 0;
								stack.Add(num16);
								stack.Add(num17);
								stack.Add(num18);
							}
						}
					}
				}
			}
			return num3 > 0;
		}

		public void MarkRectWithRegion(int minx, int maxx, int minz, int maxz, ushort region, ushort[] srcReg)
		{
			int num = maxz * this.voxelArea.width;
			for (int i = minz * this.voxelArea.width; i < num; i += this.voxelArea.width)
			{
				for (int j = minx; j < maxx; j++)
				{
					CompactVoxelCell compactVoxelCell = this.voxelArea.compactCells[i + j];
					int k = (int)compactVoxelCell.index;
					int num2 = (int)(compactVoxelCell.index + compactVoxelCell.count);
					while (k < num2)
					{
						if (this.voxelArea.areaTypes[k] != 0)
						{
							srcReg[k] = region;
						}
						k++;
					}
				}
			}
		}

		public ushort CalculateDistanceField(ushort[] src)
		{
			int num = this.voxelArea.width * this.voxelArea.depth;
			for (int i = 0; i < num; i += this.voxelArea.width)
			{
				for (int j = 0; j < this.voxelArea.width; j++)
				{
					CompactVoxelCell compactVoxelCell = this.voxelArea.compactCells[j + i];
					int k = (int)compactVoxelCell.index;
					int num2 = (int)(compactVoxelCell.index + compactVoxelCell.count);
					while (k < num2)
					{
						CompactVoxelSpan compactVoxelSpan = this.voxelArea.compactSpans[k];
						int num3 = 0;
						for (int l = 0; l < 4; l++)
						{
							if ((long)compactVoxelSpan.GetConnection(l) == 63L)
							{
								break;
							}
							num3++;
						}
						if (num3 != 4)
						{
							src[k] = 0;
						}
						k++;
					}
				}
			}
			for (int m = 0; m < num; m += this.voxelArea.width)
			{
				for (int n = 0; n < this.voxelArea.width; n++)
				{
					CompactVoxelCell compactVoxelCell2 = this.voxelArea.compactCells[n + m];
					int num4 = (int)compactVoxelCell2.index;
					int num5 = (int)(compactVoxelCell2.index + compactVoxelCell2.count);
					while (num4 < num5)
					{
						CompactVoxelSpan compactVoxelSpan2 = this.voxelArea.compactSpans[num4];
						if ((long)compactVoxelSpan2.GetConnection(0) != 63L)
						{
							int num6 = n + this.voxelArea.DirectionX[0];
							int num7 = m + this.voxelArea.DirectionZ[0];
							int num8 = (int)((ulong)this.voxelArea.compactCells[num6 + num7].index + (ulong)((long)compactVoxelSpan2.GetConnection(0)));
							if (src[num8] + 2 < src[num4])
							{
								src[num4] = src[num8] + 2;
							}
							CompactVoxelSpan compactVoxelSpan3 = this.voxelArea.compactSpans[num8];
							if ((long)compactVoxelSpan3.GetConnection(3) != 63L)
							{
								int num9 = num6 + this.voxelArea.DirectionX[3];
								int num10 = num7 + this.voxelArea.DirectionZ[3];
								int num11 = (int)((ulong)this.voxelArea.compactCells[num9 + num10].index + (ulong)((long)compactVoxelSpan3.GetConnection(3)));
								if (src[num11] + 3 < src[num4])
								{
									src[num4] = src[num11] + 3;
								}
							}
						}
						if ((long)compactVoxelSpan2.GetConnection(3) != 63L)
						{
							int num12 = n + this.voxelArea.DirectionX[3];
							int num13 = m + this.voxelArea.DirectionZ[3];
							int num14 = (int)((ulong)this.voxelArea.compactCells[num12 + num13].index + (ulong)((long)compactVoxelSpan2.GetConnection(3)));
							if (src[num14] + 2 < src[num4])
							{
								src[num4] = src[num14] + 2;
							}
							CompactVoxelSpan compactVoxelSpan4 = this.voxelArea.compactSpans[num14];
							if ((long)compactVoxelSpan4.GetConnection(2) != 63L)
							{
								int num15 = num12 + this.voxelArea.DirectionX[2];
								int num16 = num13 + this.voxelArea.DirectionZ[2];
								int num17 = (int)((ulong)this.voxelArea.compactCells[num15 + num16].index + (ulong)((long)compactVoxelSpan4.GetConnection(2)));
								if (src[num17] + 3 < src[num4])
								{
									src[num4] = src[num17] + 3;
								}
							}
						}
						num4++;
					}
				}
			}
			for (int num18 = num - this.voxelArea.width; num18 >= 0; num18 -= this.voxelArea.width)
			{
				for (int num19 = this.voxelArea.width - 1; num19 >= 0; num19--)
				{
					CompactVoxelCell compactVoxelCell3 = this.voxelArea.compactCells[num19 + num18];
					int num20 = (int)compactVoxelCell3.index;
					int num21 = (int)(compactVoxelCell3.index + compactVoxelCell3.count);
					while (num20 < num21)
					{
						CompactVoxelSpan compactVoxelSpan5 = this.voxelArea.compactSpans[num20];
						if ((long)compactVoxelSpan5.GetConnection(2) != 63L)
						{
							int num22 = num19 + this.voxelArea.DirectionX[2];
							int num23 = num18 + this.voxelArea.DirectionZ[2];
							int num24 = (int)((ulong)this.voxelArea.compactCells[num22 + num23].index + (ulong)((long)compactVoxelSpan5.GetConnection(2)));
							if (src[num24] + 2 < src[num20])
							{
								src[num20] = src[num24] + 2;
							}
							CompactVoxelSpan compactVoxelSpan6 = this.voxelArea.compactSpans[num24];
							if ((long)compactVoxelSpan6.GetConnection(1) != 63L)
							{
								int num25 = num22 + this.voxelArea.DirectionX[1];
								int num26 = num23 + this.voxelArea.DirectionZ[1];
								int num27 = (int)((ulong)this.voxelArea.compactCells[num25 + num26].index + (ulong)((long)compactVoxelSpan6.GetConnection(1)));
								if (src[num27] + 3 < src[num20])
								{
									src[num20] = src[num27] + 3;
								}
							}
						}
						if ((long)compactVoxelSpan5.GetConnection(1) != 63L)
						{
							int num28 = num19 + this.voxelArea.DirectionX[1];
							int num29 = num18 + this.voxelArea.DirectionZ[1];
							int num30 = (int)((ulong)this.voxelArea.compactCells[num28 + num29].index + (ulong)((long)compactVoxelSpan5.GetConnection(1)));
							if (src[num30] + 2 < src[num20])
							{
								src[num20] = src[num30] + 2;
							}
							CompactVoxelSpan compactVoxelSpan7 = this.voxelArea.compactSpans[num30];
							if ((long)compactVoxelSpan7.GetConnection(0) != 63L)
							{
								int num31 = num28 + this.voxelArea.DirectionX[0];
								int num32 = num29 + this.voxelArea.DirectionZ[0];
								int num33 = (int)((ulong)this.voxelArea.compactCells[num31 + num32].index + (ulong)((long)compactVoxelSpan7.GetConnection(0)));
								if (src[num33] + 3 < src[num20])
								{
									src[num20] = src[num33] + 3;
								}
							}
						}
						num20++;
					}
				}
			}
			ushort num34 = 0;
			for (int num35 = 0; num35 < this.voxelArea.compactSpanCount; num35++)
			{
				num34 = Math.Max(src[num35], num34);
			}
			return num34;
		}

		public ushort[] BoxBlur(ushort[] src, ushort[] dst)
		{
			ushort num = 20;
			int num2 = this.voxelArea.width * this.voxelArea.depth;
			for (int i = num2 - this.voxelArea.width; i >= 0; i -= this.voxelArea.width)
			{
				for (int j = this.voxelArea.width - 1; j >= 0; j--)
				{
					CompactVoxelCell compactVoxelCell = this.voxelArea.compactCells[j + i];
					int k = (int)compactVoxelCell.index;
					int num3 = (int)(compactVoxelCell.index + compactVoxelCell.count);
					while (k < num3)
					{
						CompactVoxelSpan compactVoxelSpan = this.voxelArea.compactSpans[k];
						ushort num4 = src[k];
						if (num4 < num)
						{
							dst[k] = num4;
						}
						else
						{
							int num5 = (int)num4;
							for (int l = 0; l < 4; l++)
							{
								if ((long)compactVoxelSpan.GetConnection(l) != 63L)
								{
									int num6 = j + this.voxelArea.DirectionX[l];
									int num7 = i + this.voxelArea.DirectionZ[l];
									int num8 = (int)((ulong)this.voxelArea.compactCells[num6 + num7].index + (ulong)((long)compactVoxelSpan.GetConnection(l)));
									num5 += (int)src[num8];
									CompactVoxelSpan compactVoxelSpan2 = this.voxelArea.compactSpans[num8];
									int num9 = l + 1 & 3;
									if ((long)compactVoxelSpan2.GetConnection(num9) != 63L)
									{
										int num10 = num6 + this.voxelArea.DirectionX[num9];
										int num11 = num7 + this.voxelArea.DirectionZ[num9];
										int num12 = (int)((ulong)this.voxelArea.compactCells[num10 + num11].index + (ulong)((long)compactVoxelSpan2.GetConnection(num9)));
										num5 += (int)src[num12];
									}
									else
									{
										num5 += (int)num4;
									}
								}
								else
								{
									num5 += (int)(num4 * 2);
								}
							}
							dst[k] = (ushort)((float)(num5 + 5) / 9f);
						}
						k++;
					}
				}
			}
			return dst;
		}

		private void FloodOnes(List<VInt3> st1, ushort[] regs, uint level, ushort reg)
		{
			for (int i = 0; i < st1.get_Count(); i++)
			{
				int x = st1.get_Item(i).x;
				int y = st1.get_Item(i).y;
				int z = st1.get_Item(i).z;
				regs[y] = reg;
				CompactVoxelSpan compactVoxelSpan = this.voxelArea.compactSpans[y];
				int num = this.voxelArea.areaTypes[y];
				for (int j = 0; j < 4; j++)
				{
					if ((long)compactVoxelSpan.GetConnection(j) != 63L)
					{
						int num2 = x + this.voxelArea.DirectionX[j];
						int num3 = z + this.voxelArea.DirectionZ[j];
						int num4 = (int)(this.voxelArea.compactCells[num2 + num3].index + (uint)compactVoxelSpan.GetConnection(j));
						if (num == this.voxelArea.areaTypes[num4] && regs[num4] == 1)
						{
							regs[num4] = reg;
							st1.Add(new VInt3(num2, num4, num3));
						}
					}
				}
			}
		}

		public void BuildRegions()
		{
			int num = this.voxelArea.width;
			int num2 = this.voxelArea.depth;
			int num3 = num * num2;
			int num4 = 8;
			int compactSpanCount = this.voxelArea.compactSpanCount;
			List<int> list = ListPool<int>.Claim(1024);
			ushort[] array = new ushort[compactSpanCount];
			ushort[] array2 = new ushort[compactSpanCount];
			ushort[] array3 = new ushort[compactSpanCount];
			ushort[] array4 = new ushort[compactSpanCount];
			ushort num5 = 2;
			this.MarkRectWithRegion(0, this.borderSize, 0, num2, num5 | 32768, array);
			num5 += 1;
			this.MarkRectWithRegion(num - this.borderSize, num, 0, num2, num5 | 32768, array);
			num5 += 1;
			this.MarkRectWithRegion(0, num, 0, this.borderSize, num5 | 32768, array);
			num5 += 1;
			this.MarkRectWithRegion(0, num, num2 - this.borderSize, num2, num5 | 32768, array);
			num5 += 1;
			uint num6 = (uint)(this.voxelArea.maxDistance + 1) & 4294967294u;
			int num7 = 0;
			while (num6 > 0u)
			{
				num6 = ((num6 >= 2u) ? (num6 - 2u) : 0u);
				if (this.ExpandRegions(num4, num6, array, array2, array3, array4, list) != array)
				{
					ushort[] array5 = array;
					array = array3;
					array3 = array5;
					array5 = array2;
					array2 = array4;
					array4 = array5;
				}
				int i = 0;
				int num8 = 0;
				while (i < num3)
				{
					for (int j = 0; j < this.voxelArea.width; j++)
					{
						CompactVoxelCell compactVoxelCell = this.voxelArea.compactCells[i + j];
						int k = (int)compactVoxelCell.index;
						int num9 = (int)(compactVoxelCell.index + compactVoxelCell.count);
						while (k < num9)
						{
							if ((uint)this.voxelArea.dist[k] >= num6 && array[k] == 0 && this.voxelArea.areaTypes[k] != 0 && this.FloodRegion(j, i, k, num6, num5, array, array2, list))
							{
								num5 += 1;
							}
							k++;
						}
					}
					i += num;
					num8++;
				}
				num7++;
			}
			if (this.ExpandRegions(num4 * 8, 0u, array, array2, array3, array4, list) != array)
			{
				array = array3;
			}
			this.voxelArea.maxRegions = (int)num5;
			this.FilterSmallRegions(array, this.minRegionSize, this.voxelArea.maxRegions);
			for (int l = 0; l < this.voxelArea.compactSpanCount; l++)
			{
				this.voxelArea.compactSpans[l].reg = (int)array[l];
			}
			ListPool<int>.Release(list);
		}

		private static int union_find_find(int[] arr, int x)
		{
			if (arr[x] < 0)
			{
				return x;
			}
			return arr[x] = Voxelize.union_find_find(arr, arr[x]);
		}

		private static void union_find_union(int[] arr, int a, int b)
		{
			a = Voxelize.union_find_find(arr, a);
			b = Voxelize.union_find_find(arr, b);
			if (a == b)
			{
				return;
			}
			if (arr[a] > arr[b])
			{
				int num = a;
				a = b;
				b = num;
			}
			arr[a] += arr[b];
			arr[b] = a;
		}

		public void FilterSmallRegions(ushort[] reg, int minRegionSize, int maxRegions)
		{
			RelevantGraphSurface relevantGraphSurface = RelevantGraphSurface.Root;
			bool flag = relevantGraphSurface != null && this.relevantGraphSurfaceMode != RecastGraph.RelevantGraphSurfaceMode.DoNotRequire;
			if (!flag && minRegionSize <= 0)
			{
				return;
			}
			int[] array = new int[maxRegions];
			ushort[] array2 = this.voxelArea.tmpUShortArr;
			if (array2 == null || array2.Length < maxRegions)
			{
				array2 = (this.voxelArea.tmpUShortArr = new ushort[maxRegions]);
			}
			Memory.MemSet<int>(array, -1, 4);
			Memory.MemSet<ushort>(array2, 0, maxRegions, 2);
			int num = array.Length;
			int num2 = this.voxelArea.width * this.voxelArea.depth;
			int num3 = 2 | ((this.relevantGraphSurfaceMode == RecastGraph.RelevantGraphSurfaceMode.OnlyForCompletelyInsideTile) ? 1 : 0);
			if (flag)
			{
				while (relevantGraphSurface != null)
				{
					int num4;
					int num5;
					this.VectorToIndex(relevantGraphSurface.Position, out num4, out num5);
					if (num4 < 0 || num5 < 0 || num4 >= this.voxelArea.width || num5 >= this.voxelArea.depth)
					{
						relevantGraphSurface = relevantGraphSurface.Next;
					}
					else
					{
						int num6 = (int)((relevantGraphSurface.Position.y - this.voxelOffset.y) / this.cellHeight);
						int num7 = (int)(relevantGraphSurface.maxRange / this.cellHeight);
						CompactVoxelCell compactVoxelCell = this.voxelArea.compactCells[num4 + num5 * this.voxelArea.width];
						int num8 = (int)compactVoxelCell.index;
						while ((long)num8 < (long)((ulong)(compactVoxelCell.index + compactVoxelCell.count)))
						{
							CompactVoxelSpan compactVoxelSpan = this.voxelArea.compactSpans[num8];
							if (Math.Abs((int)compactVoxelSpan.y - num6) <= num7 && reg[num8] != 0)
							{
								ushort[] array3 = array2;
								int num9 = Voxelize.union_find_find(array, (int)reg[num8] & -32769);
								ushort[] expr_1CB_cp_0 = array3;
								int expr_1CB_cp_1 = num9;
								expr_1CB_cp_0[expr_1CB_cp_1] |= 2;
							}
							num8++;
						}
						relevantGraphSurface = relevantGraphSurface.Next;
					}
				}
			}
			int i = 0;
			int num10 = 0;
			while (i < num2)
			{
				for (int j = 0; j < this.voxelArea.width; j++)
				{
					CompactVoxelCell compactVoxelCell2 = this.voxelArea.compactCells[j + i];
					int num11 = (int)compactVoxelCell2.index;
					while ((long)num11 < (long)((ulong)(compactVoxelCell2.index + compactVoxelCell2.count)))
					{
						CompactVoxelSpan compactVoxelSpan2 = this.voxelArea.compactSpans[num11];
						int num12 = (int)reg[num11];
						if ((num12 & -32769) != 0)
						{
							if (num12 >= num)
							{
								ushort[] array4 = array2;
								int num13 = Voxelize.union_find_find(array, num12 & -32769);
								ushort[] expr_290_cp_0 = array4;
								int expr_290_cp_1 = num13;
								expr_290_cp_0[expr_290_cp_1] |= 1;
							}
							else
							{
								int num14 = Voxelize.union_find_find(array, num12);
								array[num14]--;
								for (int k = 0; k < 4; k++)
								{
									if ((long)compactVoxelSpan2.GetConnection(k) != 63L)
									{
										int num15 = j + this.voxelArea.DirectionX[k];
										int num16 = i + this.voxelArea.DirectionZ[k];
										int num17 = (int)(this.voxelArea.compactCells[num15 + num16].index + (uint)compactVoxelSpan2.GetConnection(k));
										int num18 = (int)reg[num17];
										if (num12 != num18 && (num18 & -32769) != 0)
										{
											if ((num18 & 32768) != 0)
											{
												ushort[] array5 = array2;
												int num19 = num14;
												ushort[] expr_351_cp_0 = array5;
												int expr_351_cp_1 = num19;
												expr_351_cp_0[expr_351_cp_1] |= 1;
											}
											else
											{
												Voxelize.union_find_union(array, num14, num18);
											}
										}
									}
								}
							}
						}
						num11++;
					}
				}
				i += this.voxelArea.width;
				num10++;
			}
			for (int l = 0; l < array.Length; l++)
			{
				ushort[] array6 = array2;
				int num20 = Voxelize.union_find_find(array, l);
				ushort[] expr_3E7_cp_0 = array6;
				int expr_3E7_cp_1 = num20;
				expr_3E7_cp_0[expr_3E7_cp_1] |= array2[l];
			}
			for (int m = 0; m < array.Length; m++)
			{
				int num21 = Voxelize.union_find_find(array, m);
				if ((array2[num21] & 1) != 0)
				{
					array[num21] = -minRegionSize - 2;
				}
				if (flag && ((int)array2[num21] & num3) == 0)
				{
					array[num21] = -1;
				}
			}
			for (int n = 0; n < this.voxelArea.compactSpanCount; n++)
			{
				int num22 = (int)reg[n];
				if (num22 < num && array[Voxelize.union_find_find(array, num22)] >= -minRegionSize - 1)
				{
					reg[n] = 0;
				}
			}
		}
	}
}
