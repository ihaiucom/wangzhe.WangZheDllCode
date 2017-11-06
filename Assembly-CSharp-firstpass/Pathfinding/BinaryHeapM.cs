using System;

namespace Pathfinding
{
	public class BinaryHeapM
	{
		private struct Tuple
		{
			public uint F;

			public PathNode node;

			public Tuple(uint F, PathNode node)
			{
				this.F = F;
				this.node = node;
			}
		}

		public const int D = 4;

		private const bool SortGScores = true;

		public int numberOfItems;

		public float growthFactor = 2f;

		private BinaryHeapM.Tuple[] binaryHeap;

		public BinaryHeapM(int numberOfElements)
		{
			this.binaryHeap = new BinaryHeapM.Tuple[numberOfElements];
			this.numberOfItems = 0;
		}

		public void Clear()
		{
			this.numberOfItems = 0;
		}

		internal PathNode GetNode(int i)
		{
			return this.binaryHeap[i].node;
		}

		internal void SetF(int i, uint F)
		{
			this.binaryHeap[i].F = F;
		}

		public void Add(PathNode node)
		{
			if (node == null)
			{
				throw new ArgumentNullException("Sending null node to BinaryHeap");
			}
			if (this.numberOfItems == this.binaryHeap.Length)
			{
				int num = Math.Max(this.binaryHeap.Length + 4, MMGame_Math.RoundToInt((double)((float)this.binaryHeap.Length * this.growthFactor)));
				if (num > 262144)
				{
					throw new Exception("Binary Heap Size really large (2^18). A heap size this large is probably the cause of pathfinding running in an infinite loop. \nRemove this check (in BinaryHeap.cs) if you are sure that it is not caused by a bug");
				}
				BinaryHeapM.Tuple[] array = new BinaryHeapM.Tuple[num];
				for (int i = 0; i < this.binaryHeap.Length; i++)
				{
					array[i] = this.binaryHeap[i];
				}
				this.binaryHeap = array;
			}
			BinaryHeapM.Tuple tuple = new BinaryHeapM.Tuple(node.F, node);
			this.binaryHeap[this.numberOfItems] = tuple;
			int num2 = this.numberOfItems;
			uint f = node.F;
			uint g = node.G;
			while (num2 != 0)
			{
				int num3 = (num2 - 1) / 4;
				if (f >= this.binaryHeap[num3].F && (f != this.binaryHeap[num3].F || g <= this.binaryHeap[num3].node.G))
				{
					break;
				}
				this.binaryHeap[num2] = this.binaryHeap[num3];
				this.binaryHeap[num3] = tuple;
				num2 = num3;
			}
			this.numberOfItems++;
		}

		public PathNode Remove()
		{
			this.numberOfItems--;
			PathNode node = this.binaryHeap[0].node;
			this.binaryHeap[0] = this.binaryHeap[this.numberOfItems];
			int num = 0;
			while (true)
			{
				int num2 = num;
				uint f = this.binaryHeap[num].F;
				int num3 = num2 * 4 + 1;
				if (num3 <= this.numberOfItems && (this.binaryHeap[num3].F < f || (this.binaryHeap[num3].F == f && this.binaryHeap[num3].node.G < this.binaryHeap[num].node.G)))
				{
					f = this.binaryHeap[num3].F;
					num = num3;
				}
				if (num3 + 1 <= this.numberOfItems && (this.binaryHeap[num3 + 1].F < f || (this.binaryHeap[num3 + 1].F == f && this.binaryHeap[num3 + 1].node.G < this.binaryHeap[num].node.G)))
				{
					f = this.binaryHeap[num3 + 1].F;
					num = num3 + 1;
				}
				if (num3 + 2 <= this.numberOfItems && (this.binaryHeap[num3 + 2].F < f || (this.binaryHeap[num3 + 2].F == f && this.binaryHeap[num3 + 2].node.G < this.binaryHeap[num].node.G)))
				{
					f = this.binaryHeap[num3 + 2].F;
					num = num3 + 2;
				}
				if (num3 + 3 <= this.numberOfItems && (this.binaryHeap[num3 + 3].F < f || (this.binaryHeap[num3 + 3].F == f && this.binaryHeap[num3 + 3].node.G < this.binaryHeap[num].node.G)))
				{
					f = this.binaryHeap[num3 + 3].F;
					num = num3 + 3;
				}
				if (num2 == num)
				{
					break;
				}
				BinaryHeapM.Tuple tuple = this.binaryHeap[num2];
				this.binaryHeap[num2] = this.binaryHeap[num];
				this.binaryHeap[num] = tuple;
			}
			return node;
		}

		private void Validate()
		{
			for (int i = 1; i < this.numberOfItems; i++)
			{
				int num = (i - 1) / 4;
				if (this.binaryHeap[num].F > this.binaryHeap[i].F)
				{
					throw new Exception(string.Concat(new object[]
					{
						"Invalid state at ",
						i,
						":",
						num,
						" ( ",
						this.binaryHeap[num].F,
						" > ",
						this.binaryHeap[i].F,
						" ) "
					}));
				}
			}
		}

		public void Rebuild()
		{
			for (int i = 2; i < this.numberOfItems; i++)
			{
				int num = i;
				BinaryHeapM.Tuple tuple = this.binaryHeap[i];
				uint f = tuple.F;
				while (num != 1)
				{
					int num2 = num / 4;
					if (f >= this.binaryHeap[num2].F)
					{
						break;
					}
					this.binaryHeap[num] = this.binaryHeap[num2];
					this.binaryHeap[num2] = tuple;
					num = num2;
				}
			}
		}
	}
}
