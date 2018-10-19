using Assets.Scripts.Framework;
using System;
using System.Collections.Generic;

namespace behaviac
{
	public class SelectorProbability : BehaviorNode
	{
		private class SelectorProbabilityTask : CompositeTask
		{
			private List<int> m_weightingMap = new List<int>();

			private int m_totalSum;

			~SelectorProbabilityTask()
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

			protected override bool onenter(Agent pAgent)
			{
				this.m_weightingMap.Clear();
				this.m_totalSum = 0;
				for (int i = 0; i < this.m_children.Count; i++)
				{
					BehaviorTask behaviorTask = this.m_children[i];
					DecoratorWeight.DecoratorWeightTask decoratorWeightTask = (DecoratorWeight.DecoratorWeightTask)behaviorTask;
					int weight = decoratorWeightTask.GetWeight(pAgent);
					this.m_weightingMap.Add(weight);
					this.m_totalSum += weight;
				}
				return true;
			}

			protected override void onexit(Agent pAgent, EBTStatus s)
			{
				this.m_activeChildIndex = -1;
			}

			protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
			{
				if (childStatus != EBTStatus.BT_RUNNING)
				{
					return childStatus;
				}
				if (this.m_activeChildIndex != -1)
				{
					BehaviorTask behaviorTask = this.m_children[this.m_activeChildIndex];
					return behaviorTask.exec(pAgent);
				}
				int num = (int)FrameRandom.Random((uint)((ushort)this.m_totalSum));
				float num2 = 0f;
				for (int i = 0; i < this.m_children.Count; i++)
				{
					int num3 = this.m_weightingMap[i];
					num2 += (float)num3;
					if (num3 > 0 && num2 >= (float)num)
					{
						BehaviorTask behaviorTask2 = this.m_children[i];
						EBTStatus eBTStatus = behaviorTask2.exec(pAgent);
						if (eBTStatus == EBTStatus.BT_RUNNING)
						{
							this.m_activeChildIndex = i;
						}
						else
						{
							this.m_activeChildIndex = -1;
						}
						return eBTStatus;
					}
				}
				return EBTStatus.BT_FAILURE;
			}
		}

		protected CMethodBase m_method;

		~SelectorProbability()
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

		public override void AddChild(BehaviorNode pBehavior)
		{
			DecoratorWeight decoratorWeight = (DecoratorWeight)pBehavior;
			if (decoratorWeight != null)
			{
				base.AddChild(pBehavior);
			}
		}

		public override bool IsValid(Agent pAgent, BehaviorTask pTask)
		{
			return pTask.GetNode() is SelectorProbability && base.IsValid(pAgent, pTask);
		}

		protected override BehaviorTask createTask()
		{
			return new SelectorProbability.SelectorProbabilityTask();
		}
	}
}
