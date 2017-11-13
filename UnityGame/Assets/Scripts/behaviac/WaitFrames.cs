using System;
using System.Collections.Generic;

namespace behaviac
{
	public class WaitFrames : BehaviorNode
	{
		private class WaitFramesTask : LeafTask
		{
			private int m_start;

			private int m_frames;

			~WaitFramesTask()
			{
			}

			public override void copyto(BehaviorTask target)
			{
				base.copyto(target);
				WaitFrames.WaitFramesTask waitFramesTask = (WaitFrames.WaitFramesTask)target;
				waitFramesTask.m_start = this.m_start;
				waitFramesTask.m_frames = this.m_frames;
			}

			public override void save(ISerializableNode node)
			{
				base.save(node);
				CSerializationID attrId = new CSerializationID("start");
				node.setAttr<int>(attrId, this.m_start);
				CSerializationID attrId2 = new CSerializationID("frames");
				node.setAttr<int>(attrId2, this.m_frames);
			}

			public override void load(ISerializableNode node)
			{
				base.load(node);
			}

			protected override bool onenter(Agent pAgent)
			{
				this.m_start = 0;
				this.m_frames = this.GetFrames(pAgent);
				return this.m_frames > 0;
			}

			protected override void onexit(Agent pAgent, EBTStatus s)
			{
			}

			protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
			{
				this.m_start += Workspace.GetDeltaFrames();
				if (this.m_start >= this.m_frames)
				{
					return EBTStatus.BT_SUCCESS;
				}
				return EBTStatus.BT_RUNNING;
			}

			private int GetFrames(Agent pAgent)
			{
				WaitFrames waitFrames = (WaitFrames)base.GetNode();
				return (waitFrames != null) ? waitFrames.GetFrames(pAgent) : 0;
			}
		}

		private Property m_frames_var;

		private CMethodBase m_frames_method;

		~WaitFrames()
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
					if (current.name == "Frames")
					{
						string propertyName = null;
						int num = current.value.IndexOf('(');
						if (num == -1)
						{
							string text = null;
							this.m_frames_var = Condition.LoadRight(current.value, propertyName, ref text);
						}
						else
						{
							this.m_frames_method = Action.LoadMethod(current.value);
						}
					}
				}
			}
		}

		protected virtual int GetFrames(Agent pAgent)
		{
			if (this.m_frames_var != null)
			{
				return (int)this.m_frames_var.GetValue(pAgent);
			}
			if (this.m_frames_method != null)
			{
				ParentType parentType = this.m_frames_method.GetParentType();
				Agent agent = pAgent;
				if (parentType == ParentType.PT_INSTANCE)
				{
					agent = Agent.GetInstance(this.m_frames_method.GetInstanceNameString(), agent.GetContextId());
				}
				return (int)this.m_frames_method.run(agent, pAgent);
			}
			return 0;
		}

		protected override BehaviorTask createTask()
		{
			return new WaitFrames.WaitFramesTask();
		}
	}
}
