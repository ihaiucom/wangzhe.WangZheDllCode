using System;

namespace Pathfinding.Util
{
	public class LockFreeStack
	{
		public Path head;

		public void Push(Path p)
		{
			p.next = this.head;
			this.head = p;
		}

		public Path PopAll()
		{
			Path result = this.head;
			this.head = null;
			return result;
		}
	}
}
