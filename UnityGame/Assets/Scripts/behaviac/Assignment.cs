using System;
using System.Collections.Generic;

namespace behaviac
{
	public class Assignment : BehaviorNode
	{
		private class AssignmentTask : LeafTask
		{
			~AssignmentTask()
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

			protected override bool isContinueTicking()
			{
				return false;
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
				EBTStatus result = EBTStatus.BT_SUCCESS;
				Assignment assignment = (Assignment)base.GetNode();
				if (assignment.m_opr_m != null && assignment.m_opl != null)
				{
					ParentType parentType = assignment.m_opr_m.GetParentType();
					Agent agent = pAgent;
					if (parentType == ParentType.PT_INSTANCE)
					{
						agent = Agent.GetInstance(assignment.m_opr_m.GetInstanceNameString(), agent.GetContextId());
					}
					object v = assignment.m_opr_m.run(agent, pAgent);
					ParentType parentType2 = assignment.m_opl.GetParentType();
					Agent agent2 = pAgent;
					if (parentType2 == ParentType.PT_INSTANCE)
					{
						agent2 = Agent.GetInstance(assignment.m_opl.GetInstanceNameString(), agent2.GetContextId());
					}
					assignment.m_opl.SetValue(agent2, v);
				}
				else if (assignment.m_opr != null && assignment.m_opl != null)
				{
					Agent agent3 = pAgent;
					Agent agent4 = pAgent;
					ParentType parentType3 = assignment.m_opl.GetParentType();
					if (parentType3 == ParentType.PT_INSTANCE)
					{
						agent3 = Agent.GetInstance(assignment.m_opl.GetInstanceNameString(), agent3.GetContextId());
					}
					ParentType parentType4 = assignment.m_opr.GetParentType();
					if (parentType4 == ParentType.PT_INSTANCE)
					{
						agent4 = Agent.GetInstance(assignment.m_opr.GetInstanceNameString(), agent4.GetContextId());
						if (agent4 == null)
						{
							agent4 = agent3;
						}
					}
					assignment.m_opl.SetFrom(agent4, assignment.m_opr, agent3);
				}
				else
				{
					result = assignment.update_impl(pAgent, childStatus);
				}
				return result;
			}
		}

		protected Property m_opl;

		protected Property m_opr;

		protected CMethodBase m_opr_m;

		~Assignment()
		{
			this.m_opl = null;
			this.m_opr = null;
			this.m_opr_m = null;
		}

		protected override void load(int version, string agentType, List<property_t> properties)
		{
			base.load(version, agentType, properties);
			string propertyName = null;
			using (List<property_t>.Enumerator enumerator = properties.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					property_t current = enumerator.get_Current();
					if (current.name == "Opl")
					{
						this.m_opl = Condition.LoadLeft(current.value, ref propertyName, null);
					}
					else if (current.name == "Opr")
					{
						int num = current.value.IndexOf('(');
						if (num == -1)
						{
							string text = null;
							this.m_opr = Condition.LoadRight(current.value, propertyName, ref text);
						}
						else
						{
							this.m_opr_m = Action.LoadMethod(current.value);
						}
					}
				}
			}
		}

		public override bool IsValid(Agent pAgent, BehaviorTask pTask)
		{
			return pTask.GetNode() is Assignment && base.IsValid(pAgent, pTask);
		}

		protected override BehaviorTask createTask()
		{
			return new Assignment.AssignmentTask();
		}
	}
}
