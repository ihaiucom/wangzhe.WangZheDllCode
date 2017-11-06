using Pathfinding.RVO.Sampled;
using System;

namespace Pathfinding.RVO
{
	public class RVOQuadtree
	{
		private struct Node
		{
			public int child00;

			public int child01;

			public int child10;

			public int child11;

			public byte count;

			public Agent linkedList;

			public void Add(Agent agent)
			{
				agent.next = this.linkedList;
				this.linkedList = agent;
			}

			public void Distribute(RVOQuadtree.Node[] nodes, VRect r)
			{
				VInt2 center = r.center;
				while (this.linkedList != null)
				{
					Agent next = this.linkedList.next;
					if (this.linkedList.position.x > center.x)
					{
						if (this.linkedList.position.z > center.y)
						{
							nodes[this.child11].Add(this.linkedList);
						}
						else
						{
							nodes[this.child10].Add(this.linkedList);
						}
					}
					else if (this.linkedList.position.z > center.y)
					{
						nodes[this.child01].Add(this.linkedList);
					}
					else
					{
						nodes[this.child00].Add(this.linkedList);
					}
					this.linkedList = next;
				}
				this.count = 0;
			}
		}

		private const int LeafSize = 15;

		private long maxRadius;

		private RVOQuadtree.Node[] nodes = new RVOQuadtree.Node[42];

		private int filledNodes = 1;

		private VRect bounds;

		public void Clear()
		{
			this.nodes[0] = default(RVOQuadtree.Node);
			this.filledNodes = 1;
			this.maxRadius = 0L;
		}

		public void SetBounds(VRect r)
		{
			this.bounds = r;
		}

		public int GetNodeIndex()
		{
			if (this.filledNodes == this.nodes.Length)
			{
				RVOQuadtree.Node[] array = new RVOQuadtree.Node[this.nodes.Length * 2];
				for (int i = 0; i < this.nodes.Length; i++)
				{
					array[i] = this.nodes[i];
				}
				this.nodes = array;
			}
			this.nodes[this.filledNodes] = default(RVOQuadtree.Node);
			this.nodes[this.filledNodes].child00 = this.filledNodes;
			this.filledNodes++;
			return this.filledNodes - 1;
		}

		public void Insert(Agent agent)
		{
			int num = 0;
			VRect r = this.bounds;
			VInt2 xz = agent.position.xz;
			agent.next = null;
			this.maxRadius = IntMath.Max((long)agent.radius.i, this.maxRadius);
			int num2 = 0;
			while (true)
			{
				num2++;
				if (this.nodes[num].child00 == num)
				{
					if (this.nodes[num].count < 15 || num2 > 10)
					{
						break;
					}
					RVOQuadtree.Node node = this.nodes[num];
					node.child00 = this.GetNodeIndex();
					node.child01 = this.GetNodeIndex();
					node.child10 = this.GetNodeIndex();
					node.child11 = this.GetNodeIndex();
					this.nodes[num] = node;
					this.nodes[num].Distribute(this.nodes, r);
				}
				if (this.nodes[num].child00 != num)
				{
					VInt2 center = r.center;
					if (xz.x > center.x)
					{
						if (xz.y > center.y)
						{
							num = this.nodes[num].child11;
							r = VRect.MinMaxRect(center.x, center.y, r.xMax, r.yMax);
						}
						else
						{
							num = this.nodes[num].child10;
							r = VRect.MinMaxRect(center.x, r.yMin, r.xMax, center.y);
						}
					}
					else if (xz.y > center.y)
					{
						num = this.nodes[num].child01;
						r = VRect.MinMaxRect(r.xMin, center.y, center.x, r.yMax);
					}
					else
					{
						num = this.nodes[num].child00;
						r = VRect.MinMaxRect(r.xMin, r.yMin, center.x, center.y);
					}
				}
			}
			this.nodes[num].Add(agent);
			RVOQuadtree.Node[] array = this.nodes;
			int num3 = num;
			array[num3].count = array[num3].count + 1;
		}

		public void Query(VInt2 p, long radius, Agent agent)
		{
			this.QueryRec(0, p, radius, agent, this.bounds);
		}

		private long QueryRec(int i, VInt2 p, long radius, Agent agent, VRect r)
		{
			if (this.nodes[i].child00 == i)
			{
				for (Agent agent2 = this.nodes[i].linkedList; agent2 != null; agent2 = agent2.next)
				{
					long num = agent.InsertAgentNeighbour(agent2, radius * radius);
					if (num < radius * radius)
					{
						radius = (long)IntMath.Sqrt(num);
					}
				}
			}
			else
			{
				VInt2 center = r.center;
				if ((long)p.x - radius < (long)center.x)
				{
					if ((long)p.y - radius < (long)center.y)
					{
						radius = this.QueryRec(this.nodes[i].child00, p, radius, agent, VRect.MinMaxRect(r.xMin, r.yMin, center.x, center.y));
					}
					if ((long)p.y + radius > (long)center.y)
					{
						radius = this.QueryRec(this.nodes[i].child01, p, radius, agent, VRect.MinMaxRect(r.xMin, center.y, center.x, r.yMax));
					}
				}
				if ((long)p.x + radius > (long)center.x)
				{
					if ((long)p.y - radius < (long)center.y)
					{
						radius = this.QueryRec(this.nodes[i].child10, p, radius, agent, VRect.MinMaxRect(center.x, r.yMin, r.xMax, center.y));
					}
					if ((long)p.y + radius > (long)center.y)
					{
						radius = this.QueryRec(this.nodes[i].child11, p, radius, agent, VRect.MinMaxRect(center.x, center.y, r.xMax, r.yMax));
					}
				}
			}
			return radius;
		}
	}
}
