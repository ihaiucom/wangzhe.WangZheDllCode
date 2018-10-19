using System;

namespace behaviac
{
	internal class ConditionBaseTask : LeafTask
	{
		~ConditionBaseTask()
		{
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
			return EBTStatus.BT_SUCCESS;
		}

		protected override bool isContinueTicking()
		{
			return false;
		}
	}
}
