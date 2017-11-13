using System;

namespace Pathfinding.Voxels
{
	public struct LinkedVoxelSpan
	{
		public uint bottom;

		public uint top;

		public int next;

		public int area;

		public LinkedVoxelSpan(uint bottom, uint top, int area)
		{
			this.bottom = bottom;
			this.top = top;
			this.area = area;
			this.next = -1;
		}

		public LinkedVoxelSpan(uint bottom, uint top, int area, int next)
		{
			this.bottom = bottom;
			this.top = top;
			this.area = area;
			this.next = next;
		}
	}
}
