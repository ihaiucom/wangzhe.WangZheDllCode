using System;
using UnityEngine;

namespace Pathfinding.Voxels
{
	public class VoxelArea
	{
		public const uint MaxHeight = 65536u;

		public const int MaxHeightInt = 65536;

		public const uint InvalidSpanValue = 4294967295u;

		public const float AvgSpanLayerCountEstimate = 8f;

		public readonly int width;

		public readonly int depth;

		public CompactVoxelSpan[] compactSpans;

		public CompactVoxelCell[] compactCells;

		public int compactSpanCount;

		public ushort[] tmpUShortArr;

		public int[] areaTypes;

		public ushort[] dist;

		public ushort maxDistance;

		public int maxRegions;

		public int[] DirectionX;

		public int[] DirectionZ;

		public Vector3[] VectorDirection;

		private int linkedSpanCount;

		public LinkedVoxelSpan[] linkedSpans;

		private int[] removedStack = new int[128];

		private int removedStackCount;

		public VoxelArea(int width, int depth)
		{
			this.width = width;
			this.depth = depth;
			int num = width * depth;
			this.compactCells = new CompactVoxelCell[num];
			this.linkedSpans = new LinkedVoxelSpan[(int)((float)num * 8f) + 15 & -16];
			this.ResetLinkedVoxelSpans();
			int[] array = new int[4];
			array[0] = -1;
			array[2] = 1;
			this.DirectionX = array;
			this.DirectionZ = new int[]
			{
				default(int),
				width,
				default(int),
				-width
			};
			this.VectorDirection = new Vector3[]
			{
				Vector3.left,
				Vector3.forward,
				Vector3.right,
				Vector3.back
			};
		}

		public void Reset()
		{
			this.ResetLinkedVoxelSpans();
			for (int i = 0; i < this.compactCells.Length; i++)
			{
				this.compactCells[i].count = 0u;
				this.compactCells[i].index = 0u;
			}
		}

		private void ResetLinkedVoxelSpans()
		{
			int num = this.linkedSpans.Length;
			this.linkedSpanCount = this.width * this.depth;
			LinkedVoxelSpan linkedVoxelSpan = new LinkedVoxelSpan(4294967295u, 4294967295u, -1, -1);
			for (int i = 0; i < num; i++)
			{
				this.linkedSpans[i] = linkedVoxelSpan;
				i++;
				this.linkedSpans[i] = linkedVoxelSpan;
				i++;
				this.linkedSpans[i] = linkedVoxelSpan;
				i++;
				this.linkedSpans[i] = linkedVoxelSpan;
				i++;
				this.linkedSpans[i] = linkedVoxelSpan;
				i++;
				this.linkedSpans[i] = linkedVoxelSpan;
				i++;
				this.linkedSpans[i] = linkedVoxelSpan;
				i++;
				this.linkedSpans[i] = linkedVoxelSpan;
				i++;
				this.linkedSpans[i] = linkedVoxelSpan;
				i++;
				this.linkedSpans[i] = linkedVoxelSpan;
				i++;
				this.linkedSpans[i] = linkedVoxelSpan;
				i++;
				this.linkedSpans[i] = linkedVoxelSpan;
				i++;
				this.linkedSpans[i] = linkedVoxelSpan;
				i++;
				this.linkedSpans[i] = linkedVoxelSpan;
				i++;
				this.linkedSpans[i] = linkedVoxelSpan;
				i++;
				this.linkedSpans[i] = linkedVoxelSpan;
			}
			this.removedStackCount = 0;
		}

		public int GetSpanCountAll()
		{
			int num = 0;
			int num2 = this.width * this.depth;
			for (int i = 0; i < num2; i++)
			{
				int num3 = i;
				while (num3 != -1 && this.linkedSpans[num3].bottom != 4294967295u)
				{
					num++;
					num3 = this.linkedSpans[num3].next;
				}
			}
			return num;
		}

		public int GetSpanCount()
		{
			int num = 0;
			int num2 = this.width * this.depth;
			for (int i = 0; i < num2; i++)
			{
				int num3 = i;
				while (num3 != -1 && this.linkedSpans[num3].bottom != 4294967295u)
				{
					if (this.linkedSpans[num3].area != 0)
					{
						num++;
					}
					num3 = this.linkedSpans[num3].next;
				}
			}
			return num;
		}

		public void AddLinkedSpan(int index, uint bottom, uint top, int area, int voxelWalkableClimb)
		{
			if (this.linkedSpans[index].bottom == 4294967295u)
			{
				this.linkedSpans[index] = new LinkedVoxelSpan(bottom, top, area);
				return;
			}
			int num = -1;
			int num2 = index;
			while (index != -1)
			{
				if (this.linkedSpans[index].bottom > top)
				{
					break;
				}
				if (this.linkedSpans[index].top < bottom)
				{
					num = index;
					index = this.linkedSpans[index].next;
				}
				else
				{
					if (this.linkedSpans[index].bottom < bottom)
					{
						bottom = this.linkedSpans[index].bottom;
					}
					if (this.linkedSpans[index].top > top)
					{
						top = this.linkedSpans[index].top;
					}
					if (AstarMath.Abs((int)(top - this.linkedSpans[index].top)) <= voxelWalkableClimb)
					{
						area = AstarMath.Max(area, this.linkedSpans[index].area);
					}
					int next = this.linkedSpans[index].next;
					if (num != -1)
					{
						this.linkedSpans[num].next = next;
						if (this.removedStackCount == this.removedStack.Length)
						{
							int[] array = new int[this.removedStackCount * 4];
							Buffer.BlockCopy(this.removedStack, 0, array, 0, this.removedStackCount * 4);
							this.removedStack = array;
						}
						this.removedStack[this.removedStackCount] = index;
						this.removedStackCount++;
						index = next;
					}
					else
					{
						if (next == -1)
						{
							this.linkedSpans[num2] = new LinkedVoxelSpan(bottom, top, area);
							return;
						}
						this.linkedSpans[num2] = this.linkedSpans[next];
						if (this.removedStackCount == this.removedStack.Length)
						{
							int[] array2 = new int[this.removedStackCount * 4];
							Buffer.BlockCopy(this.removedStack, 0, array2, 0, this.removedStackCount * 4);
							this.removedStack = array2;
						}
						this.removedStack[this.removedStackCount] = next;
						this.removedStackCount++;
						index = this.linkedSpans[num2].next;
					}
				}
			}
			if (this.linkedSpanCount >= this.linkedSpans.Length)
			{
				LinkedVoxelSpan[] array3 = this.linkedSpans;
				int num3 = this.linkedSpanCount;
				int num4 = this.removedStackCount;
				this.linkedSpans = new LinkedVoxelSpan[this.linkedSpans.Length * 2];
				this.ResetLinkedVoxelSpans();
				this.linkedSpanCount = num3;
				this.removedStackCount = num4;
				for (int i = 0; i < this.linkedSpanCount; i++)
				{
					this.linkedSpans[i] = array3[i];
				}
				Debug.Log("Layer estimate too low, doubling size of buffer.\nThis message is harmless.");
			}
			int num5;
			if (this.removedStackCount > 0)
			{
				this.removedStackCount--;
				num5 = this.removedStack[this.removedStackCount];
			}
			else
			{
				num5 = this.linkedSpanCount;
				this.linkedSpanCount++;
			}
			if (num != -1)
			{
				this.linkedSpans[num5] = new LinkedVoxelSpan(bottom, top, area, this.linkedSpans[num].next);
				this.linkedSpans[num].next = num5;
			}
			else
			{
				this.linkedSpans[num5] = this.linkedSpans[num2];
				this.linkedSpans[num2] = new LinkedVoxelSpan(bottom, top, area, num5);
			}
		}
	}
}
