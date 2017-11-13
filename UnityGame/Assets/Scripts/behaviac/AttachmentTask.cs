using System;

namespace behaviac
{
	public class AttachmentTask : BehaviorTask
	{
		protected AttachmentTask()
		{
		}

		~AttachmentTask()
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

		public override void traverse(NodeHandler_t handler, Agent pAgent, object user_data)
		{
			handler(this, pAgent, user_data);
		}
	}
}
