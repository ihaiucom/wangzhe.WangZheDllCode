using System;
using System.Collections.Generic;

namespace behaviac
{
	public class Compute : BehaviorNode
	{
		private class ComputeTask : LeafTask
		{
			~ComputeTask()
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
				Compute compute = (Compute)base.GetNode();
				bool flag = false;
				object value = null;
				if (compute.m_opl != null)
				{
					if (compute.m_opr1_m != null)
					{
						flag = true;
						ParentType parentType = compute.m_opr1_m.GetParentType();
						Agent agent = pAgent;
						if (parentType == ParentType.PT_INSTANCE)
						{
							agent = Agent.GetInstance(compute.m_opr1_m.GetInstanceNameString(), agent.GetContextId());
						}
						value = compute.m_opr1_m.run(agent, pAgent);
					}
					else if (compute.m_opr1 != null)
					{
						flag = true;
						Agent agent2 = pAgent;
						Agent agent3 = pAgent;
						ParentType parentType2 = compute.m_opl.GetParentType();
						if (parentType2 == ParentType.PT_INSTANCE)
						{
							agent2 = Agent.GetInstance(compute.m_opl.GetInstanceNameString(), agent2.GetContextId());
						}
						ParentType parentType3 = compute.m_opr1.GetParentType();
						if (parentType3 == ParentType.PT_INSTANCE)
						{
							agent3 = Agent.GetInstance(compute.m_opr1.GetInstanceNameString(), agent3.GetContextId());
							if (agent3 == null)
							{
								agent3 = agent2;
							}
						}
						compute.m_opl.SetFrom(agent3, compute.m_opr1, agent2);
						value = compute.m_opl.GetValue(agent2);
					}
					if (compute.m_opr2_m != null)
					{
						flag = true;
						ParentType parentType4 = compute.m_opr2_m.GetParentType();
						Agent agent4 = pAgent;
						if (parentType4 == ParentType.PT_INSTANCE)
						{
							agent4 = Agent.GetInstance(compute.m_opr2_m.GetInstanceNameString(), agent4.GetContextId());
						}
						object value2 = compute.m_opr2_m.run(agent4, pAgent);
						ParentType parentType5 = compute.m_opl.GetParentType();
						Agent agent5 = pAgent;
						if (parentType5 == ParentType.PT_INSTANCE)
						{
							agent5 = Agent.GetInstance(compute.m_opl.GetInstanceNameString(), agent5.GetContextId());
						}
						object v = Details.ComputeValue(value, value2, compute.m_operator);
						compute.m_opl.SetValue(agent5, v);
					}
					else if (compute.m_opr2 != null)
					{
						flag = true;
						Agent agent6 = pAgent;
						Agent agent7 = pAgent;
						ParentType parentType6 = compute.m_opl.GetParentType();
						if (parentType6 == ParentType.PT_INSTANCE)
						{
							agent6 = Agent.GetInstance(compute.m_opl.GetInstanceNameString(), agent6.GetContextId());
						}
						ParentType parentType7 = compute.m_opr2.GetParentType();
						if (parentType7 == ParentType.PT_INSTANCE)
						{
							agent7 = Agent.GetInstance(compute.m_opr2.GetInstanceNameString(), agent7.GetContextId());
							if (agent7 == null)
							{
								agent7 = agent6;
							}
						}
						object value3 = compute.m_opr2.GetValue(agent7);
						object v2 = Details.ComputeValue(value, value3, compute.m_operator);
						compute.m_opl.SetValue(agent6, v2);
					}
				}
				if (!flag)
				{
					result = compute.update_impl(pAgent, childStatus);
				}
				return result;
			}
		}

		protected Property m_opl;

		protected Property m_opr1;

		protected CMethodBase m_opr1_m;

		protected Property m_opr2;

		protected CMethodBase m_opr2_m;

		protected EComputeOperator m_operator;

		~Compute()
		{
			this.m_opl = null;
			this.m_opr1 = null;
			this.m_opr1_m = null;
			this.m_opr2 = null;
			this.m_opr2_m = null;
		}

		protected override void load(int version, string agentType, List<property_t> properties)
		{
			base.load(version, agentType, properties);
			string propertyName = null;
			foreach (property_t current in properties)
			{
				if (current.name == "Opl")
				{
					this.m_opl = Condition.LoadLeft(current.value, ref propertyName, null);
				}
				else if (current.name == "Operator")
				{
					if (current.value == "Add")
					{
						this.m_operator = EComputeOperator.E_ADD;
					}
					else if (current.value == "Sub")
					{
						this.m_operator = EComputeOperator.E_SUB;
					}
					else if (current.value == "Mul")
					{
						this.m_operator = EComputeOperator.E_MUL;
					}
					else if (current.value == "Div")
					{
						this.m_operator = EComputeOperator.E_DIV;
					}
				}
				else if (current.name == "Opr1")
				{
					int num = current.value.IndexOf('(');
					if (num == -1)
					{
						string text = null;
						this.m_opr1 = Condition.LoadRight(current.value, propertyName, ref text);
					}
					else
					{
						this.m_opr1_m = Action.LoadMethod(current.value);
					}
				}
				else if (current.name == "Opr2")
				{
					int num2 = current.value.IndexOf('(');
					if (num2 == -1)
					{
						string text2 = null;
						this.m_opr2 = Condition.LoadRight(current.value, propertyName, ref text2);
					}
					else
					{
						this.m_opr2_m = Action.LoadMethod(current.value);
					}
				}
			}
		}

		public override bool IsValid(Agent pAgent, BehaviorTask pTask)
		{
			return pTask.GetNode() is Compute && base.IsValid(pAgent, pTask);
		}

		protected override BehaviorTask createTask()
		{
			return new Compute.ComputeTask();
		}
	}
}
