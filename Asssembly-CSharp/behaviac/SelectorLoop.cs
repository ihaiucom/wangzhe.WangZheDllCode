using System;
using System.Collections.Generic;

namespace behaviac
{
	public class SelectorLoop : BehaviorNode
	{
		public class SelectorLoopTask : CompositeTask
		{
			~SelectorLoopTask()
			{
			}

			public override void Init(BehaviorNode node)
			{
				base.Init(node);
			}

			protected override void addChild(BehaviorTask pBehavior)
			{
				base.addChild(pBehavior);
			}

			public override void copyto(BehaviorTask target)
			{
				base.copyto(target);
				SelectorLoop.SelectorLoopTask selectorLoopTask = (SelectorLoop.SelectorLoopTask)target;
				selectorLoopTask.m_activeChildIndex = this.m_activeChildIndex;
			}

			public override void save(ISerializableNode node)
			{
				base.save(node);
				CSerializationID attrId = new CSerializationID("activeChild");
				node.setAttr<int>(attrId, this.m_activeChildIndex);
			}

			public override void load(ISerializableNode node)
			{
				base.load(node);
			}

			protected override bool onenter(Agent pAgent)
			{
				this.m_activeChildIndex = -1;
				return base.onenter(pAgent);
			}

			protected override void onexit(Agent pAgent, EBTStatus s)
			{
				base.onexit(pAgent, s);
			}

			protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
			{
				int num = -1;
				for (int i = 0; i < this.m_children.Count; i++)
				{
					WithPreconditionTask withPreconditionTask = (WithPreconditionTask)this.m_children[i];
					BehaviorTask behaviorTask = withPreconditionTask.PreconditionNode();
					EBTStatus eBTStatus = behaviorTask.exec(pAgent);
					if (eBTStatus == EBTStatus.BT_SUCCESS)
					{
						num = i;
						break;
					}
				}
				if (num != -1)
				{
					if (this.m_activeChildIndex != -1)
					{
						WithPreconditionTask withPreconditionTask2 = (WithPreconditionTask)this.m_children[this.m_activeChildIndex];
						BehaviorTask behaviorTask2 = withPreconditionTask2.Action();
						WithPreconditionTask withPreconditionTask3 = (WithPreconditionTask)this.m_children[num];
						BehaviorTask behaviorTask3 = withPreconditionTask3.Action();
						if (behaviorTask2 != behaviorTask3)
						{
							behaviorTask2.abort(pAgent);
							withPreconditionTask2.abort(pAgent);
							this.m_activeChildIndex = num;
						}
					}
					for (int j = 0; j < this.m_children.Count; j++)
					{
						WithPreconditionTask withPreconditionTask4 = (WithPreconditionTask)this.m_children[j];
						EBTStatus eBTStatus2 = withPreconditionTask4.exec(pAgent);
						if (j >= num)
						{
							if (j > num)
							{
								BehaviorTask behaviorTask4 = withPreconditionTask4.PreconditionNode();
								EBTStatus eBTStatus3 = behaviorTask4.exec(pAgent);
								if (eBTStatus3 != EBTStatus.BT_SUCCESS)
								{
									goto IL_153;
								}
							}
							BehaviorTask behaviorTask5 = withPreconditionTask4.Action();
							EBTStatus eBTStatus4 = behaviorTask5.exec(pAgent);
							if (eBTStatus4 == EBTStatus.BT_RUNNING)
							{
								this.m_activeChildIndex = num;
							}
							else if (eBTStatus4 == EBTStatus.BT_FAILURE || eBTStatus4 == EBTStatus.BT_INVALID)
							{
								goto IL_153;
							}
							return eBTStatus4;
						}
						IL_153:;
					}
				}
				return EBTStatus.BT_FAILURE;
			}

			protected override bool isContinueTicking()
			{
				return true;
			}
		}

		~SelectorLoop()
		{
		}

		protected override void load(int version, string agentType, List<property_t> properties)
		{
			base.load(version, agentType, properties);
		}

		public override bool IsValid(Agent pAgent, BehaviorTask pTask)
		{
			return pTask.GetNode() is SelectorLoop && base.IsValid(pAgent, pTask);
		}

		protected override BehaviorTask createTask()
		{
			return new SelectorLoop.SelectorLoopTask();
		}
	}
}
