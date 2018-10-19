using System;

namespace Pathfinding
{
	public class PathNode
	{
		private const uint CostMask = 268435455u;

		private const int Flag1Offset = 28;

		private const uint Flag1Mask = 268435456u;

		private const int Flag2Offset = 29;

		private const uint Flag2Mask = 536870912u;

		public GraphNode node;

		public PathNode parent;

		public ushort pathID;

		private uint flags;

		private uint g;

		private uint h;

		public uint cost
		{
			get
			{
				return this.flags & 268435455u;
			}
			set
			{
				this.flags = ((this.flags & 4026531840u) | value);
			}
		}

		public bool flag1
		{
			get
			{
				return (this.flags & 268435456u) != 0u;
			}
			set
			{
				this.flags = ((this.flags & 4026531839u) | ((!value) ? 0u : 268435456u));
			}
		}

		public bool flag2
		{
			get
			{
				return (this.flags & 536870912u) != 0u;
			}
			set
			{
				this.flags = ((this.flags & 3758096383u) | ((!value) ? 0u : 536870912u));
			}
		}

		public uint G
		{
			get
			{
				return this.g;
			}
			set
			{
				this.g = value;
			}
		}

		public uint H
		{
			get
			{
				return this.h;
			}
			set
			{
				this.h = value;
			}
		}

		public uint F
		{
			get
			{
				return this.g + this.h;
			}
		}
	}
}
