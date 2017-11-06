using System;

namespace behaviac
{
	internal class WaitforSignalTask : SingeChildTask
	{
		private bool m_bTriggered;

		public WaitforSignalTask()
		{
			this.m_bTriggered = false;
		}

		~WaitforSignalTask()
		{
		}

		public override void copyto(BehaviorTask target)
		{
			base.copyto(target);
			WaitforSignalTask waitforSignalTask = (WaitforSignalTask)target;
			waitforSignalTask.m_bTriggered = this.m_bTriggered;
		}

		public override void save(ISerializableNode node)
		{
			base.save(node);
			CSerializationID attrId = new CSerializationID("triggered");
			node.setAttr<bool>(attrId, this.m_bTriggered);
		}

		public override void load(ISerializableNode node)
		{
			base.load(node);
		}

		public override void Init(BehaviorNode node)
		{
			base.Init(node);
		}

		public override bool CheckPredicates(Agent pAgent)
		{
			bool result = false;
			if (this.m_attachments != null)
			{
				result = base.CheckPredicates(pAgent);
			}
			return result;
		}

		protected override bool onenter(Agent pAgent)
		{
			this.m_bTriggered = false;
			return true;
		}

		protected override void onexit(Agent pAgent, EBTStatus s)
		{
		}

		protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
		{
			if (childStatus != EBTStatus.BT_RUNNING)
			{
				return childStatus;
			}
			if (!this.m_bTriggered)
			{
				this.m_bTriggered = this.CheckPredicates(pAgent);
			}
			if (!this.m_bTriggered)
			{
				return EBTStatus.BT_RUNNING;
			}
			if (this.m_root == null)
			{
				return EBTStatus.BT_SUCCESS;
			}
			return base.update(pAgent, childStatus);
		}

		protected override bool isContinueTicking()
		{
			return true;
		}
	}
}
