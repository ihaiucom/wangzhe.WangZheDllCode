using System;

namespace behaviac
{
	public class SingeChildTask : BranchTask
	{
		protected BehaviorTask m_root;

		protected SingeChildTask()
		{
			this.m_root = null;
		}

		public override void traverse(NodeHandler_t handler, Agent pAgent, object user_data)
		{
			if (handler(this, pAgent, user_data) && this.m_root != null)
			{
				this.m_root.traverse(handler, pAgent, user_data);
			}
		}

		~SingeChildTask()
		{
			this.m_root = null;
		}

		public override void Init(BehaviorNode node)
		{
			base.Init(node);
			if (node.GetChildrenCount() == 1)
			{
				BehaviorNode child = node.GetChild(0);
				if (child != null)
				{
					BehaviorTask pBehavior = child.CreateAndInitTask();
					this.addChild(pBehavior);
				}
			}
		}

		public override void copyto(BehaviorTask target)
		{
			base.copyto(target);
			SingeChildTask singeChildTask = target as SingeChildTask;
			if (this.m_root != null)
			{
				if (singeChildTask.m_root == null)
				{
					BehaviorNode node = this.m_root.GetNode();
					singeChildTask.m_root = node.CreateAndInitTask();
				}
				this.m_root.copyto(singeChildTask.m_root);
			}
		}

		public override void save(ISerializableNode node)
		{
			base.save(node);
			if (this.m_root != null)
			{
			}
		}

		public override void load(ISerializableNode node)
		{
			base.load(node);
		}

		protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
		{
			if (this.m_currentTask != null)
			{
				return base.update(pAgent, childStatus);
			}
			if (this.m_root != null)
			{
				return this.m_root.exec(pAgent);
			}
			return EBTStatus.BT_INVALID;
		}

		protected override void addChild(BehaviorTask pBehavior)
		{
			pBehavior.SetParent(this);
			this.m_root = pBehavior;
		}

		public override ListView<BehaviorNode> GetRunningNodes()
		{
			ListView<BehaviorNode> listView = new ListView<BehaviorNode>();
			BehaviorTask behaviorTask = base.GetCurrentTask();
			if (behaviorTask == null)
			{
				behaviorTask = this.m_root;
			}
			if (behaviorTask != null)
			{
				listView.AddRange(behaviorTask.GetRunningNodes());
			}
			return listView;
		}
	}
}
