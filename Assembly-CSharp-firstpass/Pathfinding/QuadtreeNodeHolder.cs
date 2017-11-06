using System;

namespace Pathfinding
{
	public class QuadtreeNodeHolder
	{
		public QuadtreeNode node;

		public QuadtreeNodeHolder c0;

		public QuadtreeNodeHolder c1;

		public QuadtreeNodeHolder c2;

		public QuadtreeNodeHolder c3;

		public void GetNodes(GraphNodeDelegateCancelable del)
		{
			if (this.node != null)
			{
				del(this.node);
				return;
			}
			this.c0.GetNodes(del);
			this.c1.GetNodes(del);
			this.c2.GetNodes(del);
			this.c3.GetNodes(del);
		}
	}
}
