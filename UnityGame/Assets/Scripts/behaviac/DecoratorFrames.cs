using System;
using System.Collections.Generic;

namespace behaviac
{
	public class DecoratorFrames : DecoratorNode
	{
		private class DecoratorFramesTask : DecoratorTask
		{
			private int m_start;

			private int m_frames;

			~DecoratorFramesTask()
			{
			}

			public override void copyto(BehaviorTask target)
			{
				base.copyto(target);
				DecoratorFrames.DecoratorFramesTask decoratorFramesTask = (DecoratorFrames.DecoratorFramesTask)target;
				decoratorFramesTask.m_start = this.m_start;
				decoratorFramesTask.m_frames = this.m_frames;
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
				base.onenter(pAgent);
				this.m_start = 0;
				this.m_frames = this.GetFrames(pAgent);
				return this.m_frames > 0;
			}

			protected override EBTStatus decorate(EBTStatus status)
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
				DecoratorFrames decoratorFrames = (DecoratorFrames)base.GetNode();
				return (decoratorFrames != null) ? decoratorFrames.GetFrames(pAgent) : 0;
			}
		}

		private Property m_frames_var;

		~DecoratorFrames()
		{
			this.m_frames_var = null;
		}

		protected override void load(int version, string agentType, List<property_t> properties)
		{
			base.load(version, agentType, properties);
			using (List<property_t>.Enumerator enumerator = properties.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					property_t current = enumerator.get_Current();
					if (current.name == "Time")
					{
						string text = null;
						string propertyName = null;
						this.m_frames_var = Condition.LoadRight(current.value, propertyName, ref text);
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
			return 0;
		}

		protected override BehaviorTask createTask()
		{
			return new DecoratorFrames.DecoratorFramesTask();
		}
	}
}
