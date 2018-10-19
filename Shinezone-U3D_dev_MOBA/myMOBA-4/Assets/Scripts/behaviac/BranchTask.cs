using System;

namespace behaviac
{
	public abstract class BranchTask : BehaviorTask
	{
		protected BehaviorTask m_currentTask;

		protected EBTStatus m_returnStatus;

		protected BranchTask()
		{
			this.m_returnStatus = EBTStatus.BT_INVALID;
		}

		public override void SetCurrentTask(BehaviorTask node)
		{
			this.m_currentTask = node;
		}

		public BehaviorTask GetCurrentTask()
		{
			return this.m_currentTask;
		}

		public override EBTStatus GetReturnStatus()
		{
			return this.m_returnStatus;
		}

		public override void SetReturnStatus(EBTStatus status)
		{
			this.m_returnStatus = status;
		}

		~BranchTask()
		{
		}

		public override void Init(BehaviorNode node)
		{
			base.Init(node);
		}

		public override void copyto(BehaviorTask target)
		{
			base.copyto(target);
		}

		public override void save(ISerializableNode node)
		{
			base.save(node);
		}

		public override void load(ISerializableNode node)
		{
			base.load(node);
		}

		protected EBTStatus tickCurrentNode(Agent pAgent)
		{
			EBTStatus status = this.m_currentTask.GetStatus();
			if (status == EBTStatus.BT_INVALID || status == EBTStatus.BT_RUNNING)
			{
				BehaviorTask currentTask = this.m_currentTask;
				EBTStatus eBTStatus = this.m_currentTask.exec(pAgent);
				if (eBTStatus != EBTStatus.BT_RUNNING)
				{
					BranchTask parent = currentTask.GetParent();
					this.SetCurrentTask(null);
					while (parent != null && parent != this)
					{
						eBTStatus = parent.update(pAgent, eBTStatus);
						if (eBTStatus == EBTStatus.BT_RUNNING)
						{
							return EBTStatus.BT_RUNNING;
						}
						parent.onexit_action(pAgent, eBTStatus);
						parent.m_status = eBTStatus;
						parent = parent.GetParent();
					}
				}
				return eBTStatus;
			}
			return status;
		}

		public override bool onevent(Agent pAgent, string eventName)
		{
			bool result = true;
			if (this.m_status == EBTStatus.BT_RUNNING && this.m_node.HasEvents())
			{
				result = this.oneventCurrentNode(pAgent, eventName);
			}
			return result;
		}

		protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_INVALID;
			if (this.m_currentTask != null)
			{
				EBTStatus status = this.m_currentTask.GetStatus();
				if (status != EBTStatus.BT_RUNNING)
				{
					this.SetCurrentTask(null);
				}
			}
			if (this.m_currentTask != null)
			{
				result = this.tickCurrentNode(pAgent);
			}
			return result;
		}

		protected override bool isContinueTicking()
		{
			return false;
		}

		private bool oneventCurrentNode(Agent pAgent, string eventName)
		{
			EBTStatus status = this.m_currentTask.GetStatus();
			bool flag = this.m_currentTask.onevent(pAgent, eventName);
			if (flag)
			{
				BranchTask parent = this.m_currentTask.GetParent();
				while (parent != null && parent != this)
				{
					flag = parent.onevent(pAgent, eventName);
					if (!flag)
					{
						return false;
					}
					parent = parent.GetParent();
				}
			}
			return flag;
		}

		protected abstract void addChild(BehaviorTask pBehavior);
	}
}
