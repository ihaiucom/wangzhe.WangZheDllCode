using System;

namespace behaviac
{
	public abstract class DecoratorTask : SingeChildTask
	{
		private bool m_bDecorateWhenChildEnds;

		protected DecoratorTask()
		{
			this.m_bDecorateWhenChildEnds = false;
		}

		~DecoratorTask()
		{
		}

		public override void Init(BehaviorNode node)
		{
			base.Init(node);
			DecoratorNode decoratorNode = node as DecoratorNode;
			this.m_bDecorateWhenChildEnds = decoratorNode.m_bDecorateWhenChildEnds;
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
			EBTStatus eBTStatus = base.update(pAgent, childStatus);
			if (!this.m_bDecorateWhenChildEnds || eBTStatus != EBTStatus.BT_RUNNING)
			{
				EBTStatus result = this.decorate(eBTStatus);
				if (eBTStatus != EBTStatus.BT_RUNNING)
				{
					BehaviorTask root = this.m_root;
					if (root != null)
					{
						root.SetStatus(EBTStatus.BT_INVALID);
					}
					this.SetCurrentTask(null);
				}
				return result;
			}
			return EBTStatus.BT_RUNNING;
		}

		protected abstract EBTStatus decorate(EBTStatus status);

		protected override bool isContinueTicking()
		{
			return true;
		}
	}
}
