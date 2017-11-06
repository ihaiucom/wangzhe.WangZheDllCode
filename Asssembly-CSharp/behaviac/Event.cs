using System;
using System.Collections.Generic;

namespace behaviac
{
	public class Event : ConditionBase
	{
		public class EventTask : AttachmentTask
		{
			~EventTask()
			{
			}

			public bool TriggeredOnce()
			{
				Event @event = base.GetNode() as Event;
				return @event.m_bTriggeredOnce;
			}

			public TriggerMode GetTriggerMode()
			{
				Event @event = base.GetNode() as Event;
				return @event.m_triggerMode;
			}

			public string GetEventName()
			{
				Event @event = base.GetNode() as Event;
				return @event.m_event.Name;
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
				EBTStatus result = EBTStatus.BT_SUCCESS;
				Event @event = base.GetNode() as Event;
				if (!string.IsNullOrEmpty(@event.m_referencedBehaviorPath) && pAgent != null)
				{
					TriggerMode triggerMode = this.GetTriggerMode();
					pAgent.bteventtree(@event.m_referencedBehaviorPath, triggerMode);
					pAgent.btexec();
				}
				return result;
			}

			public override bool NeedRestart()
			{
				return true;
			}
		}

		protected CMethodBase m_event;

		protected string m_referencedBehaviorPath;

		protected TriggerMode m_triggerMode;

		protected bool m_bTriggeredOnce;

		public Event()
		{
			this.m_bTriggeredOnce = false;
			this.m_triggerMode = TriggerMode.TM_Transfer;
		}

		~Event()
		{
			this.m_event = null;
		}

		protected override void load(int version, string agentType, List<property_t> properties)
		{
			base.load(version, agentType, properties);
			using (List<property_t>.Enumerator enumerator = properties.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					property_t current = enumerator.get_Current();
					if (current.name == "EventName")
					{
						this.m_event = Action.LoadMethod(current.value);
					}
					else if (current.name == "ReferenceFilename")
					{
						this.m_referencedBehaviorPath = current.value;
					}
					else if (current.name == "TriggeredOnce")
					{
						if (current.value == "true")
						{
							this.m_bTriggeredOnce = true;
						}
					}
					else if (current.name == "TriggerMode")
					{
						if (current.value == "Transfer")
						{
							this.m_triggerMode = TriggerMode.TM_Transfer;
						}
						else if (current.value == "Return")
						{
							this.m_triggerMode = TriggerMode.TM_Return;
						}
					}
				}
			}
		}

		public override bool IsValid(Agent pAgent, BehaviorTask pTask)
		{
			return pTask.GetNode() is Event && base.IsValid(pAgent, pTask);
		}

		protected override BehaviorTask createTask()
		{
			return new Event.EventTask();
		}
	}
}
