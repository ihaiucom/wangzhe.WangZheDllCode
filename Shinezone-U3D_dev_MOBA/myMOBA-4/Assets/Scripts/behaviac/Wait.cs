using Assets.Scripts.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace behaviac
{
	public class Wait : BehaviorNode
	{
		private class WaitTask : LeafTask
		{
			private float m_start;

			private float m_time;

			public WaitTask()
			{
				this.m_start = 0f;
				this.m_time = 0f;
			}

			public override void copyto(BehaviorTask target)
			{
				base.copyto(target);
				Wait.WaitTask waitTask = (Wait.WaitTask)target;
				waitTask.m_start = this.m_start;
				waitTask.m_time = this.m_time;
			}

			public override void save(ISerializableNode node)
			{
				base.save(node);
				CSerializationID attrId = new CSerializationID("start");
				node.setAttr<float>(attrId, this.m_start);
				CSerializationID attrId2 = new CSerializationID("time");
				node.setAttr<float>(attrId2, this.m_time);
			}

			public override void load(ISerializableNode node)
			{
				base.load(node);
			}

			private bool GetIgnoreTimeScale()
			{
				Wait wait = base.GetNode() as Wait;
				return wait != null && wait.m_ignoreTimeScale;
			}

			private float GetTime(Agent pAgent)
			{
				Wait wait = base.GetNode() as Wait;
				return (wait == null) ? 0f : wait.GetTime(pAgent);
			}

			protected override bool onenter(Agent pAgent)
			{
				if (this.GetIgnoreTimeScale())
				{
					this.m_start = Time.realtimeSinceStartup * 1000f;
				}
				else
				{
					this.m_start = Singleton<FrameSynchr>.GetInstance().LogicFrameTick;
				}
				this.m_time = this.GetTime(pAgent);
				return this.m_time > 0f;
			}

			protected override void onexit(Agent pAgent, EBTStatus s)
			{
			}

			protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
			{
				if (this.GetIgnoreTimeScale())
				{
					if (Time.realtimeSinceStartup * 1000f - this.m_start >= this.m_time)
					{
						return EBTStatus.BT_SUCCESS;
					}
				}
				else if (Singleton<FrameSynchr>.GetInstance().LogicFrameTick - this.m_start >= this.m_time)
				{
					return EBTStatus.BT_SUCCESS;
				}
				return EBTStatus.BT_RUNNING;
			}
		}

		protected bool m_ignoreTimeScale;

		protected Property m_time_var;

		public Wait()
		{
			this.m_ignoreTimeScale = false;
			this.m_time_var = null;
		}

		~Wait()
		{
			this.m_time_var = null;
		}

		protected override void load(int version, string agentType, List<property_t> properties)
		{
			base.load(version, agentType, properties);
			foreach (property_t current in properties)
			{
				if (current.name == "IgnoreTimeScale")
				{
					this.m_ignoreTimeScale = (current.value == "true");
				}
				else if (current.name == "Time")
				{
					string text = null;
					string propertyName = null;
					this.m_time_var = Condition.LoadRight(current.value, propertyName, ref text);
				}
			}
		}

		protected virtual float GetTime(Agent pAgent)
		{
			if (this.m_time_var != null)
			{
				object value = this.m_time_var.GetValue(pAgent);
				return Convert.ToSingle(value);
			}
			return 0f;
		}

		protected override BehaviorTask createTask()
		{
			return new Wait.WaitTask();
		}
	}
}
