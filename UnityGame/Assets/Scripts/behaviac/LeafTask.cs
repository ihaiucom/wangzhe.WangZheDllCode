using System;

namespace behaviac
{
	public class LeafTask : BehaviorTask
	{
		protected LeafTask()
		{
		}

		public override void traverse(NodeHandler_t handler, Agent pAgent, object user_data)
		{
			handler(this, pAgent, user_data);
		}

		~LeafTask()
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

		protected override bool isContinueTicking()
		{
			return true;
		}

		public override bool onevent(Agent pAgent, string eventName)
		{
			return base.onevent(pAgent, eventName);
		}

		public override ListView<BehaviorNode> GetRunningNodes()
		{
			ListView<BehaviorNode> listView = new ListView<BehaviorNode>();
			if (this.isContinueTicking() && base.GetStatus() == EBTStatus.BT_RUNNING)
			{
				listView.Add(base.GetNode());
			}
			return listView;
		}
	}
}
