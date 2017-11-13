using System;

namespace Pathfinding.Voxels
{
	public class VoxelSpan
	{
		public uint bottom;

		public uint top;

		public VoxelSpan next;

		public int area;

		public VoxelSpan(uint b, uint t, int area)
		{
			this.bottom = b;
			this.top = t;
			this.area = area;
		}
	}
}
