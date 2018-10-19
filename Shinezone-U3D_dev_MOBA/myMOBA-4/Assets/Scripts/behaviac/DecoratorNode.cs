using System;
using System.Collections.Generic;

namespace behaviac
{
	public abstract class DecoratorNode : BehaviorNode
	{
		public bool m_bDecorateWhenChildEnds;

		public DecoratorNode()
		{
			this.m_bDecorateWhenChildEnds = false;
		}

		~DecoratorNode()
		{
		}

		protected override void load(int version, string agentType, List<property_t> properties)
		{
			base.load(version, agentType, properties);
			foreach (property_t current in properties)
			{
				if (current.name == "DecorateWhenChildEnds" && current.value == "true")
				{
					this.m_bDecorateWhenChildEnds = true;
				}
			}
		}

		public override bool IsValid(Agent pAgent, BehaviorTask pTask)
		{
			return pTask.GetNode() is DecoratorNode && base.IsValid(pAgent, pTask);
		}
	}
}
