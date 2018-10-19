using System;

namespace behaviac
{
	public class BehaviorTreeTask : SingeChildTask
	{
		public void SetRootTask(BehaviorTask pRoot)
		{
			this.addChild(pRoot);
		}

		public void CopyTo(BehaviorTreeTask target)
		{
			this.copyto(target);
		}

		public void Save(ISerializableNode node)
		{
		}

		public void Load(ISerializableNode node)
		{
			this.load(node);
		}

		public string GetName()
		{
			BehaviorTree behaviorTree = this.m_node as BehaviorTree;
			return behaviorTree.GetName();
		}

		~BehaviorTreeTask()
		{
		}

		public override void Clear()
		{
			base.Clear();
			this.m_currentTask = null;
			this.m_returnStatus = EBTStatus.BT_INVALID;
			this.m_root = null;
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

		public EBTStatus resume(Agent pAgent, EBTStatus status)
		{
			BranchTask branchTask = null;
			BranchTask branchTask2;
			for (BehaviorTask currentTask = this.m_currentTask; currentTask != null; currentTask = branchTask2.GetCurrentTask())
			{
				branchTask2 = (currentTask as BranchTask);
				if (branchTask2 == null)
				{
					branchTask = currentTask.GetParent();
					break;
				}
				branchTask = branchTask2;
			}
			if (branchTask != null)
			{
				branchTask.onexit_action(pAgent, status);
				branchTask.SetReturnStatus(status);
			}
			return status;
		}

		protected override bool onenter(Agent pAgent)
		{
			return true;
		}

		protected override void onexit(Agent pAgent, EBTStatus s)
		{
		}

		protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus eBTStatus = base.update(pAgent, childStatus);
			if (eBTStatus != EBTStatus.BT_RUNNING)
			{
				this.SetCurrentTask(null);
			}
			return eBTStatus;
		}

		public override bool NeedRestart()
		{
			BehaviorTask root = this.m_root;
			return root != null && root.NeedRestart();
		}

		public override bool onevent(Agent pAgent, string eventName)
		{
			if (this.m_node.HasEvents())
			{
				bool flag = this.m_root.onevent(pAgent, eventName);
				if (flag)
				{
					return this.m_status == EBTStatus.BT_RUNNING && this.m_node.HasEvents() && !base.CheckEvents(eventName, pAgent) && false;
				}
			}
			return true;
		}

		protected override bool isContinueTicking()
		{
			return true;
		}
	}
}
