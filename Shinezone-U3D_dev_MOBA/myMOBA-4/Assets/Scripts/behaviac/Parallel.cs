using System;
using System.Collections.Generic;

namespace behaviac
{
	public class Parallel : BehaviorNode
	{
		private class ParallelTask : CompositeTask
		{
			~ParallelTask()
			{
				this.m_children.Clear();
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

			protected override bool onenter(Agent pAgent)
			{
				return true;
			}

			protected override void onexit(Agent pAgent, EBTStatus s)
			{
			}

			protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
			{
				Parallel parallel = (Parallel)base.GetNode();
				bool flag = false;
				bool flag2 = false;
				bool flag3 = false;
				bool flag4 = true;
				bool flag5 = true;
				bool flag6 = parallel.m_childFinishPolicy == CHILDFINISH_POLICY.CHILDFINISH_LOOP;
				for (int i = 0; i < this.m_children.Count; i++)
				{
					BehaviorTask behaviorTask = this.m_children[i];
					EBTStatus status = behaviorTask.GetStatus();
					if (flag6 || status == EBTStatus.BT_RUNNING || status == EBTStatus.BT_INVALID)
					{
						EBTStatus eBTStatus = behaviorTask.exec(pAgent);
						if (eBTStatus == EBTStatus.BT_FAILURE)
						{
							flag2 = true;
							flag5 = false;
						}
						else if (eBTStatus == EBTStatus.BT_SUCCESS)
						{
							flag = true;
							flag4 = false;
						}
						else if (eBTStatus == EBTStatus.BT_RUNNING)
						{
							flag3 = true;
							flag4 = false;
							flag5 = false;
						}
					}
					else if (status == EBTStatus.BT_SUCCESS)
					{
						flag = true;
						flag4 = false;
					}
					else
					{
						flag2 = true;
						flag5 = false;
					}
				}
				EBTStatus eBTStatus2 = (!flag3) ? EBTStatus.BT_FAILURE : EBTStatus.BT_RUNNING;
				if ((parallel.m_failPolicy == FAILURE_POLICY.FAIL_ON_ALL && flag4) || (parallel.m_failPolicy == FAILURE_POLICY.FAIL_ON_ONE && flag2))
				{
					eBTStatus2 = EBTStatus.BT_FAILURE;
				}
				else if ((parallel.m_succeedPolicy == SUCCESS_POLICY.SUCCEED_ON_ALL && flag5) || (parallel.m_succeedPolicy == SUCCESS_POLICY.SUCCEED_ON_ONE && flag))
				{
					eBTStatus2 = EBTStatus.BT_SUCCESS;
				}
				if (parallel.m_exitPolicy == EXIT_POLICY.EXIT_ABORT_RUNNINGSIBLINGS && (eBTStatus2 == EBTStatus.BT_FAILURE || eBTStatus2 == EBTStatus.BT_SUCCESS))
				{
					for (int j = 0; j < this.m_children.Count; j++)
					{
						BehaviorTask behaviorTask2 = this.m_children[j];
						EBTStatus status2 = behaviorTask2.GetStatus();
						if (status2 == EBTStatus.BT_RUNNING)
						{
							behaviorTask2.abort(pAgent);
						}
					}
				}
				return eBTStatus2;
			}

			protected override bool isContinueTicking()
			{
				return true;
			}
		}

		protected FAILURE_POLICY m_failPolicy;

		protected SUCCESS_POLICY m_succeedPolicy;

		protected EXIT_POLICY m_exitPolicy;

		protected CHILDFINISH_POLICY m_childFinishPolicy;

		public Parallel()
		{
			this.m_failPolicy = FAILURE_POLICY.FAIL_ON_ONE;
			this.m_succeedPolicy = SUCCESS_POLICY.SUCCEED_ON_ALL;
			this.m_exitPolicy = EXIT_POLICY.EXIT_NONE;
			this.m_childFinishPolicy = CHILDFINISH_POLICY.CHILDFINISH_LOOP;
		}

		~Parallel()
		{
		}

		protected override void load(int version, string agentType, List<property_t> properties)
		{
			base.load(version, agentType, properties);
			for (int i = 0; i < properties.Count; i++)
			{
				property_t property_t = properties[i];
				if (property_t.name == "FailurePolicy")
				{
					if (property_t.value == "FAIL_ON_ONE")
					{
						this.m_failPolicy = FAILURE_POLICY.FAIL_ON_ONE;
					}
					else if (property_t.value == "FAIL_ON_ALL")
					{
						this.m_failPolicy = FAILURE_POLICY.FAIL_ON_ALL;
					}
				}
				else if (property_t.name == "SuccessPolicy")
				{
					if (property_t.value == "SUCCEED_ON_ONE")
					{
						this.m_succeedPolicy = SUCCESS_POLICY.SUCCEED_ON_ONE;
					}
					else if (property_t.value == "SUCCEED_ON_ALL")
					{
						this.m_succeedPolicy = SUCCESS_POLICY.SUCCEED_ON_ALL;
					}
				}
				else if (property_t.name == "ExitPolicy")
				{
					if (property_t.value == "EXIT_NONE")
					{
						this.m_exitPolicy = EXIT_POLICY.EXIT_NONE;
					}
					else if (property_t.value == "EXIT_ABORT_RUNNINGSIBLINGS")
					{
						this.m_exitPolicy = EXIT_POLICY.EXIT_ABORT_RUNNINGSIBLINGS;
					}
				}
				else if (property_t.name == "ChildFinishPolicy")
				{
					if (property_t.value == "CHILDFINISH_ONCE")
					{
						this.m_childFinishPolicy = CHILDFINISH_POLICY.CHILDFINISH_ONCE;
					}
					else if (property_t.value == "CHILDFINISH_LOOP")
					{
						this.m_childFinishPolicy = CHILDFINISH_POLICY.CHILDFINISH_LOOP;
					}
				}
			}
		}

		public override bool IsValid(Agent pAgent, BehaviorTask pTask)
		{
			return pTask.GetNode() is Parallel && base.IsValid(pAgent, pTask);
		}

		protected override BehaviorTask createTask()
		{
			return new Parallel.ParallelTask();
		}
	}
}
