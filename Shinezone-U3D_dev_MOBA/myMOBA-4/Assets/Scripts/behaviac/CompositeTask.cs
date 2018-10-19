using System;

namespace behaviac
{
	public class CompositeTask : BranchTask
	{
		protected const int InvalidChildIndex = -1;

		protected ListView<BehaviorTask> m_children = new ListView<BehaviorTask>();

		protected int m_activeChildIndex = -1;

		protected CompositeTask()
		{
			this.m_activeChildIndex = -1;
		}

		public override void traverse(NodeHandler_t handler, Agent pAgent, object user_data)
		{
			if (handler(this, pAgent, user_data))
			{
				for (int i = 0; i < this.m_children.Count; i++)
				{
					BehaviorTask behaviorTask = this.m_children[i];
					behaviorTask.traverse(handler, pAgent, user_data);
				}
			}
		}

		~CompositeTask()
		{
			this.m_children.Clear();
		}

		public override void Init(BehaviorNode node)
		{
			base.Init(node);
			int childrenCount = node.GetChildrenCount();
			for (int i = 0; i < childrenCount; i++)
			{
				BehaviorNode child = node.GetChild(i);
				BehaviorTask pBehavior = child.CreateAndInitTask();
				this.addChild(pBehavior);
			}
		}

		public override void copyto(BehaviorTask target)
		{
			base.copyto(target);
			CompositeTask compositeTask = target as CompositeTask;
			compositeTask.m_activeChildIndex = this.m_activeChildIndex;
			int count = this.m_children.Count;
			for (int i = 0; i < count; i++)
			{
				BehaviorTask behaviorTask = this.m_children[i];
				BehaviorTask target2 = compositeTask.m_children[i];
				behaviorTask.copyto(target2);
			}
		}

		public override void save(ISerializableNode node)
		{
			base.save(node);
		}

		public override void load(ISerializableNode node)
		{
			base.load(node);
		}

		protected override void addChild(BehaviorTask pBehavior)
		{
			pBehavior.SetParent(this);
			this.m_children.Add(pBehavior);
		}

		public override ListView<BehaviorNode> GetRunningNodes()
		{
			ListView<BehaviorNode> listView = new ListView<BehaviorNode>();
			foreach (BehaviorTask current in this.m_children)
			{
				listView.AddRange(current.GetRunningNodes());
			}
			return listView;
		}
	}
}
