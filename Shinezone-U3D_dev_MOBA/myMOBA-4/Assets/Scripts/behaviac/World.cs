using System;
using System.Collections.Generic;

namespace behaviac
{
	public class World : Agent
	{
		public struct HeapItem_t : IComparable<World.HeapItem_t>
		{
			public int priority;

			public DictionaryView<int, Agent> agents;

			public int CompareTo(World.HeapItem_t other)
			{
				if (this.priority < other.priority)
				{
					return -1;
				}
				if (this.priority > other.priority)
				{
					return 1;
				}
				return 0;
			}
		}

		private List<World.HeapItem_t> m_agents;

		private bool m_bTickAgents;

		public List<World.HeapItem_t> Agents
		{
			get
			{
				if (this.m_agents == null)
				{
					this.m_agents = new List<World.HeapItem_t>();
				}
				return this.m_agents;
			}
			set
			{
				this.m_agents = value;
			}
		}

		protected World()
		{
		}

		protected new void OnDestroy()
		{
			Context context = Context.GetContext(this.m_contextId);
			World world = context.GetWorld(false);
			if (world == this)
			{
				context.SetWorld(null);
			}
			base.OnDestroy();
		}

		protected new void Init()
		{
			Context context = Context.GetContext(this.m_contextId);
			context.SetWorld(this);
			base.Init();
			this.m_bTickAgents = true;
		}

		public static World GetInstance(int contextId)
		{
			Context context = Context.GetContext(contextId);
			return context.GetWorld(true);
		}

		public void AddAgent(Agent pAgent)
		{
			int id = pAgent.GetId();
			int priority = pAgent.GetPriority();
			int num = -1;
			for (int i = 0; i < this.Agents.Count; i++)
			{
				if (this.Agents[i].priority == priority)
				{
					num = i;
					break;
				}
			}
			if (num == -1)
			{
				World.HeapItem_t item = default(World.HeapItem_t);
				item.agents = new DictionaryView<int, Agent>();
				item.priority = priority;
				item.agents[id] = pAgent;
				this.Agents.Add(item);
			}
			else
			{
				this.Agents[num].agents[id] = pAgent;
			}
		}

		public void RemoveAgent(Agent pAgent)
		{
			int id = pAgent.GetId();
			int priority = pAgent.GetPriority();
			int num = -1;
			for (int i = 0; i < this.Agents.Count; i++)
			{
				if (this.Agents[i].priority == priority)
				{
					num = i;
					break;
				}
			}
			if (num != -1 && this.Agents[num].agents.ContainsKey(id))
			{
				this.Agents[num].agents.Remove(id);
			}
		}

		public void RemoveAllAgents()
		{
			this.Agents.Clear();
		}

		public override EBTStatus btexec()
		{
			Workspace.LogFrames();
			Workspace.HandleRequests();
			if (Workspace.GetAutoHotReload())
			{
				Workspace.HotReload();
			}
			base.btexec();
			if (this.m_bTickAgents)
			{
				this.btexec_agents();
			}
			return EBTStatus.BT_RUNNING;
		}

		public void ToggleTickAgents(bool bTickAgents)
		{
			this.m_bTickAgents = bTickAgents;
		}

		public bool IsTickAgents()
		{
			return this.m_bTickAgents;
		}

		private void btexec_agents()
		{
			this.Agents.Sort();
			for (int i = 0; i < this.Agents.Count; i++)
			{
				foreach (KeyValuePair<int, Agent> current in this.Agents[i].agents)
				{
					if (current.Value.IsActive())
					{
						current.Value.btexec();
						if (!this.m_bTickAgents)
						{
							break;
						}
					}
				}
			}
			if (Agent.IdMask() != 0u)
			{
				int contextId = base.GetContextId();
				Context context = Context.GetContext(contextId);
				context.LogStaticVariables(null);
			}
		}

		public void LogCurrentStates()
		{
			string text = string.Format("LogCurrentStates {0} {1}", base.GetClassTypeName(), this.Agents.Count);
			foreach (World.HeapItem_t current in this.Agents)
			{
				foreach (KeyValuePair<int, Agent> current2 in current.agents)
				{
					if (current2.Value.IsMasked())
					{
						current2.Value.LogVariables(true);
					}
				}
			}
			if (base.IsMasked())
			{
				base.LogVariables(true);
			}
		}
	}
}
