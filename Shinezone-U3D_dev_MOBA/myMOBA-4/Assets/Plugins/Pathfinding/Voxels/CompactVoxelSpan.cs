using System;

namespace Pathfinding.Voxels
{
	public struct CompactVoxelSpan
	{
		public ushort y;

		public uint con;

		public uint h;

		public int reg;

		public CompactVoxelSpan(ushort bottom, uint height)
		{
			this.con = 24u;
			this.y = bottom;
			this.h = height;
			this.reg = 0;
		}

		public void SetConnection(int dir, uint value)
		{
			int num = dir * 6;
			this.con = (uint)(((ulong)this.con & (ulong)(~(63L << (num & 31)))) | (ulong)((ulong)(value & 63u) << num));
		}

		public int GetConnection(int dir)
		{
			return (int)this.con >> dir * 6 & 63;
		}
	}
}
