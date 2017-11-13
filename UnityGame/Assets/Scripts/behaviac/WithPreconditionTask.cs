using System;

namespace behaviac
{
	internal class WithPreconditionTask : Sequence.SequenceTask
	{
		protected override void addChild(BehaviorTask pBehavior)
		{
			base.addChild(pBehavior);
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

		protected override bool onenter(Agent pAgent)
		{
			BehaviorTask parent = base.GetParent();
			return true;
		}

		protected override void onexit(Agent pAgent, EBTStatus s)
		{
			BehaviorTask parent = base.GetParent();
		}

		protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
		{
			BehaviorTask parent = base.GetParent();
			return EBTStatus.BT_RUNNING;
		}

		public BehaviorTask PreconditionNode()
		{
			return this.m_children[0];
		}

		public BehaviorTask Action()
		{
			return this.m_children[1];
		}
	}
}
