using System;
using System.Collections.Generic;

namespace behaviac
{
	public class ReferencedBehavior : BehaviorNode
	{
		private class ReferencedBehaviorTask : SingeChildTask
		{
			~ReferencedBehaviorTask()
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

			protected override bool onenter(Agent pAgent)
			{
				return true;
			}

			protected override void onexit(Agent pAgent, EBTStatus s)
			{
			}

			protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
			{
				ReferencedBehavior referencedBehavior = base.GetNode() as ReferencedBehavior;
				if (referencedBehavior != null)
				{
					string name = pAgent.btgetcurrent().GetName();
					string btMsg = string.Format("{0}[{1}] {2}", name, referencedBehavior.GetId(), referencedBehavior.m_referencedBehaviorPath);
					LogManager.Log(pAgent, btMsg, EActionResult.EAR_none, LogMode.ELM_jump);
					pAgent.btreferencetree(referencedBehavior.m_referencedBehaviorPath);
				}
				return EBTStatus.BT_RUNNING;
			}
		}

		protected string m_referencedBehaviorPath;

		~ReferencedBehavior()
		{
		}

		protected override void load(int version, string agentType, List<property_t> properties)
		{
			base.load(version, agentType, properties);
			using (List<property_t>.Enumerator enumerator = properties.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					property_t current = enumerator.get_Current();
					if (current.name == "ReferenceFilename")
					{
						this.m_referencedBehaviorPath = current.value;
						bool flag = Workspace.Load(this.m_referencedBehaviorPath);
					}
				}
			}
		}

		public override bool IsValid(Agent pAgent, BehaviorTask pTask)
		{
			return pTask.GetNode() is ReferencedBehavior && base.IsValid(pAgent, pTask);
		}

		protected override BehaviorTask createTask()
		{
			return new ReferencedBehavior.ReferencedBehaviorTask();
		}
	}
}
