using Assets.Scripts.Framework;
using System;
using System.Collections.Generic;

namespace behaviac
{
	public abstract class CompositeStochastic : BehaviorNode
	{
		public class CompositeStochasticTask : CompositeTask
		{
			public static uint RandomMax = 10000u;

			protected List<int> m_set = new List<int>();

			~CompositeStochasticTask()
			{
			}

			public static int GetRandomValue(CMethodBase method, Agent pAgent)
			{
				int result;
				if (method != null)
				{
					ParentType parentType = method.GetParentType();
					Agent agent = pAgent;
					if (parentType == ParentType.PT_INSTANCE)
					{
						agent = Agent.GetInstance(method.GetInstanceNameString(), agent.GetContextId());
					}
					result = (int)method.run(agent, pAgent);
				}
				else
				{
					result = (int)FrameRandom.Random(CompositeStochastic.CompositeStochasticTask.RandomMax);
				}
				return result;
			}

			public override void copyto(BehaviorTask target)
			{
				base.copyto(target);
				CompositeStochastic.CompositeStochasticTask compositeStochasticTask = (CompositeStochastic.CompositeStochasticTask)target;
				compositeStochasticTask.m_set = this.m_set;
			}

			public override void save(ISerializableNode node)
			{
				base.save(node);
				CSerializationID attrId = new CSerializationID("set");
				node.setAttr<List<int>>(attrId, this.m_set);
			}

			public override void load(ISerializableNode node)
			{
				base.load(node);
			}

			protected override bool onenter(Agent pAgent)
			{
				this.random_child(pAgent);
				this.m_activeChildIndex = 0;
				return true;
			}

			protected override void onexit(Agent pAgent, EBTStatus s)
			{
			}

			private void random_child(Agent pAgent)
			{
				CompositeStochastic compositeStochastic = (CompositeStochastic)base.GetNode();
				int count = this.m_children.Count;
				if (this.m_set.Count != count)
				{
					this.m_set.Clear();
					for (int i = 0; i < count; i++)
					{
						this.m_set.Add(i);
					}
				}
				for (int j = 0; j < count; j++)
				{
					int num = (int)((long)(count * CompositeStochastic.CompositeStochasticTask.GetRandomValue((compositeStochastic == null) ? null : compositeStochastic.m_method, pAgent)) / (long)((ulong)CompositeStochastic.CompositeStochasticTask.RandomMax));
					int num2 = (int)((long)(count * CompositeStochastic.CompositeStochasticTask.GetRandomValue((compositeStochastic == null) ? null : compositeStochastic.m_method, pAgent)) / (long)((ulong)CompositeStochastic.CompositeStochasticTask.RandomMax));
					if (num != num2)
					{
						int value = this.m_set[num];
						this.m_set[num] = this.m_set[num2];
						this.m_set[num2] = value;
					}
				}
			}
		}

		protected CMethodBase m_method;

		public CompositeStochastic()
		{
		}

		~CompositeStochastic()
		{
			this.m_method = null;
		}

		protected override void load(int version, string agentType, List<property_t> properties)
		{
			base.load(version, agentType, properties);
			foreach (property_t current in properties)
			{
				if (current.name == "RandomGenerator")
				{
					if (current.value[0] != '\0')
					{
						this.m_method = Action.LoadMethod(current.value);
					}
				}
			}
		}

		public override bool IsValid(Agent pAgent, BehaviorTask pTask)
		{
			return pTask.GetNode() is CompositeStochastic && base.IsValid(pAgent, pTask);
		}
	}
}
