using System;
using System.Threading;

namespace Pathfinding
{
	public class ThreadControlQueue
	{
		public class QueueTerminationException : Exception
		{
		}

		private Path head;

		private Path tail;

		private object lockObj = new object();

		private int numReceivers;

		private bool blocked;

		private int blockedReceivers;

		private bool starving;

		private bool terminate;

		private ManualResetEvent block = new ManualResetEvent(true);

		public bool IsEmpty
		{
			get
			{
				return this.head == null;
			}
		}

		public bool IsTerminating
		{
			get
			{
				return this.terminate;
			}
		}

		public bool AllReceiversBlocked
		{
			get
			{
				return this.blocked && this.blockedReceivers == this.numReceivers;
			}
		}

		public ThreadControlQueue(int numReceivers)
		{
			this.numReceivers = numReceivers;
		}

		public void Block()
		{
			this.blocked = true;
			this.block.Reset();
		}

		public void Unblock()
		{
			this.blocked = false;
			this.block.Set();
		}

		public void Lock()
		{
			Monitor.Enter(this.lockObj);
		}

		public void Unlock()
		{
			Monitor.Exit(this.lockObj);
		}

		public void PushFront(Path p)
		{
			if (this.terminate)
			{
				return;
			}
			if (this.tail == null)
			{
				this.head = p;
				this.tail = p;
				if (this.starving && !this.blocked)
				{
					this.starving = false;
					this.block.Set();
				}
				else
				{
					this.starving = false;
				}
			}
			else
			{
				p.next = this.head;
				this.head = p;
			}
		}

		public void Push(Path p)
		{
			if (this.terminate)
			{
				return;
			}
			if (this.tail == null)
			{
				this.head = p;
				this.tail = p;
				if (this.starving && !this.blocked)
				{
					this.starving = false;
					this.block.Set();
				}
				else
				{
					this.starving = false;
				}
			}
			else
			{
				this.tail.next = p;
				this.tail = p;
			}
		}

		private void Starving()
		{
			this.starving = true;
			this.block.Reset();
		}

		public void TerminateReceivers()
		{
			this.terminate = true;
			this.block.Set();
		}

		public Path Pop()
		{
			Path result;
			lock (this.lockObj)
			{
				if (this.terminate)
				{
					this.blockedReceivers++;
					throw new ThreadControlQueue.QueueTerminationException();
				}
				if (this.head == null)
				{
					this.Starving();
				}
				while (this.blocked || this.starving)
				{
					this.blockedReceivers++;
					if (this.terminate)
					{
						throw new ThreadControlQueue.QueueTerminationException();
					}
					if (this.blockedReceivers != this.numReceivers)
					{
						if (this.blockedReceivers > this.numReceivers)
						{
							throw new InvalidOperationException(string.Concat(new object[]
							{
								"More receivers are blocked than specified in constructor (",
								this.blockedReceivers,
								" > ",
								this.numReceivers,
								")"
							}));
						}
					}
					Monitor.Exit(this.lockObj);
					this.block.WaitOne();
					Monitor.Enter(this.lockObj);
					this.blockedReceivers--;
					if (this.head == null)
					{
						this.Starving();
					}
				}
				Path path = this.head;
				DebugHelper.Assert(this.head != null);
				if (this.head.next == null)
				{
					this.tail = null;
				}
				this.head = this.head.next;
				result = path;
			}
			return result;
		}

		public void ReceiverTerminated()
		{
			Monitor.Enter(this.lockObj);
			this.blockedReceivers++;
			Monitor.Exit(this.lockObj);
		}

		public Path PopNoBlock(bool blockedBefore)
		{
			Path result;
			lock (this.lockObj)
			{
				if (this.terminate)
				{
					this.blockedReceivers++;
					throw new ThreadControlQueue.QueueTerminationException();
				}
				if (this.head == null)
				{
					this.Starving();
				}
				if (this.blocked || this.starving)
				{
					if (!blockedBefore)
					{
						this.blockedReceivers++;
						if (this.terminate)
						{
							throw new ThreadControlQueue.QueueTerminationException();
						}
						if (this.blockedReceivers != this.numReceivers)
						{
							if (this.blockedReceivers > this.numReceivers)
							{
								throw new InvalidOperationException(string.Concat(new object[]
								{
									"More receivers are blocked than specified in constructor (",
									this.blockedReceivers,
									" > ",
									this.numReceivers,
									")"
								}));
							}
						}
					}
					result = null;
				}
				else
				{
					if (blockedBefore)
					{
						this.blockedReceivers--;
					}
					Path path = this.head;
					DebugHelper.Assert(this.head != null);
					if (this.head.next == null)
					{
						this.tail = null;
					}
					this.head = this.head.next;
					result = path;
				}
			}
			return result;
		}
	}
}
