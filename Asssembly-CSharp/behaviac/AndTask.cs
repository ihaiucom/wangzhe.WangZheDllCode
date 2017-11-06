using System;

namespace behaviac
{
	internal class AndTask : Sequence.SequenceTask
	{
		~AndTask()
		{
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

		protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
		{
			for (int i = 0; i < this.m_children.Count; i++)
			{
				BehaviorTask behaviorTask = this.m_children[i];
				EBTStatus eBTStatus = behaviorTask.exec(pAgent);
				if (eBTStatus == EBTStatus.BT_FAILURE)
				{
					return eBTStatus;
				}
			}
			return EBTStatus.BT_SUCCESS;
		}
	}
}
