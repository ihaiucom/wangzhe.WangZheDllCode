using System;

namespace behaviac
{
	public abstract class BehaviorTask
	{
		private static NodeHandler_t abort_handler_ = new NodeHandler_t(BehaviorTask.abort_handler);

		private static NodeHandler_t reset_handler_ = new NodeHandler_t(BehaviorTask.reset_handler);

		private static EBTStatus ms_lastExitStatus_ = EBTStatus.BT_INVALID;

		public EBTStatus m_status;

		protected BehaviorNode m_node;

		protected BranchTask m_parent;

		protected ListView<AttachmentTask> m_attachments;

		private int m_id;

		protected BehaviorTask()
		{
			this.m_status = EBTStatus.BT_INVALID;
			this.m_node = null;
			this.m_attachments = null;
			this.m_parent = null;
		}

		public static void DestroyTask(BehaviorTask task)
		{
		}

		protected void FreeAttachments()
		{
			if (this.m_attachments != null)
			{
				this.m_attachments.Clear();
				this.m_attachments = null;
			}
		}

		public virtual void Clear()
		{
			this.m_status = EBTStatus.BT_INVALID;
			this.m_parent = null;
			this.m_id = -1;
			this.FreeAttachments();
			this.m_node = null;
		}

		public virtual void Init(BehaviorNode node)
		{
			this.m_node = node;
			this.m_id = this.m_node.GetId();
			int attachmentsCount = node.GetAttachmentsCount();
			if (attachmentsCount > 0)
			{
				for (int i = 0; i < attachmentsCount; i++)
				{
					BehaviorNode attachment = node.GetAttachment(i);
					AttachmentTask pAttachment = (AttachmentTask)attachment.CreateAndInitTask();
					this.Attach(pAttachment);
				}
			}
		}

		public virtual void copyto(BehaviorTask target)
		{
			target.m_status = this.m_status;
		}

		public virtual void save(ISerializableNode node)
		{
		}

		public virtual void load(ISerializableNode node)
		{
		}

		public string GetClassNameString()
		{
			if (this.m_node != null)
			{
				return this.m_node.GetClassNameString();
			}
			return "SubBT";
		}

		public int GetId()
		{
			return this.m_id;
		}

		public void SetId(int id)
		{
			this.m_id = id;
		}

		public virtual EBTStatus GetReturnStatus()
		{
			return EBTStatus.BT_INVALID;
		}

		public virtual void SetReturnStatus(EBTStatus status)
		{
		}

		public EBTStatus exec(Agent pAgent)
		{
			bool flag;
			if (this.m_status == EBTStatus.BT_RUNNING)
			{
				flag = true;
			}
			else
			{
				this.m_status = EBTStatus.BT_INVALID;
				flag = this.onenter_action(pAgent);
				bool flag2 = this.isContinueTicking();
				if (flag2)
				{
					BranchTask parentBranch = this.GetParentBranch();
					if (parentBranch != null && parentBranch != this)
					{
						parentBranch.SetCurrentTask(this);
					}
				}
			}
			if (flag)
			{
				EBTStatus returnStatus = this.GetReturnStatus();
				if (returnStatus == EBTStatus.BT_INVALID)
				{
					this.m_status = this.update(pAgent, EBTStatus.BT_RUNNING);
				}
				else
				{
					this.m_status = returnStatus;
				}
				if (this.m_status != EBTStatus.BT_RUNNING)
				{
					bool flag3 = this.isContinueTicking();
					if (flag3)
					{
						BranchTask parentBranch2 = this.GetParentBranch();
						if (parentBranch2 != null && parentBranch2 != this)
						{
							parentBranch2.SetCurrentTask(null);
						}
					}
					this.onexit_action(pAgent, this.m_status);
				}
			}
			else
			{
				this.m_status = EBTStatus.BT_FAILURE;
			}
			EBTStatus status = this.m_status;
			if (this.m_status != EBTStatus.BT_RUNNING && this.NeedRestart())
			{
				this.m_status = EBTStatus.BT_INVALID;
				this.SetReturnStatus(EBTStatus.BT_INVALID);
			}
			return status;
		}

		private static bool abort_handler(BehaviorTask node, Agent pAgent, object user_data)
		{
			if (node.m_status == EBTStatus.BT_RUNNING)
			{
				node.onexit_action(pAgent, EBTStatus.BT_FAILURE);
				node.m_status = EBTStatus.BT_FAILURE;
				node.SetCurrentTask(null);
			}
			return true;
		}

		private static bool reset_handler(BehaviorTask node, Agent pAgent, object user_data)
		{
			node.m_status = EBTStatus.BT_INVALID;
			node.SetCurrentTask(null);
			return true;
		}

		public void abort(Agent pAgent)
		{
			this.traverse(BehaviorTask.abort_handler_, pAgent, null);
		}

		public void reset(Agent pAgent)
		{
			this.traverse(BehaviorTask.reset_handler_, pAgent, null);
		}

		public EBTStatus GetStatus()
		{
			return this.m_status;
		}

		public void SetStatus(EBTStatus s)
		{
			this.m_status = s;
		}

		public BehaviorNode GetNode()
		{
			return this.m_node;
		}

		public BranchTask GetParentBranch()
		{
			for (BehaviorTask parent = this.m_parent; parent != null; parent = parent.m_parent)
			{
				BranchTask branchTask = parent as BranchTask;
				if (branchTask != null && branchTask.isContinueTicking())
				{
					return branchTask;
				}
			}
			return null;
		}

		public void SetParent(BranchTask parent)
		{
			this.m_parent = parent;
		}

		public BranchTask GetParent()
		{
			return this.m_parent;
		}

		public abstract void traverse(NodeHandler_t handler, Agent pAgent, object user_data);

		public virtual bool NeedRestart()
		{
			return false;
		}

		public virtual void SetCurrentTask(BehaviorTask node)
		{
		}

		private bool getBooleanFromStatus(EBTStatus status)
		{
			return status != EBTStatus.BT_FAILURE && status == EBTStatus.BT_SUCCESS;
		}

		public virtual bool CheckPredicates(Agent pAgent)
		{
			if (this.m_attachments == null || this.m_attachments.Count == 0)
			{
				return true;
			}
			bool flag = false;
			for (int i = 0; i < this.m_attachments.Count; i++)
			{
				AttachmentTask attachmentTask = this.m_attachments[i];
				Predicate.PredicateTask predicateTask = attachmentTask as Predicate.PredicateTask;
				if (predicateTask != null)
				{
					EBTStatus eBTStatus = predicateTask.GetStatus();
					if (eBTStatus == EBTStatus.BT_RUNNING || eBTStatus == EBTStatus.BT_INVALID)
					{
						eBTStatus = predicateTask.exec(pAgent);
					}
					bool booleanFromStatus = this.getBooleanFromStatus(eBTStatus);
					if (i == 0)
					{
						flag = booleanFromStatus;
					}
					else
					{
						bool flag2 = predicateTask.IsAnd();
						if (flag2)
						{
							flag = (flag && booleanFromStatus);
						}
						else
						{
							flag = (flag || booleanFromStatus);
						}
					}
				}
			}
			return flag;
		}

		public bool CheckEvents(string eventName, Agent pAgent)
		{
			if (this.m_attachments != null)
			{
				for (int i = 0; i < this.m_attachments.Count; i++)
				{
					AttachmentTask attachmentTask = this.m_attachments[i];
					Event.EventTask eventTask = attachmentTask as Event.EventTask;
					if (eventTask != null && !string.IsNullOrEmpty(eventName))
					{
						string eventName2 = eventTask.GetEventName();
						if (!string.IsNullOrEmpty(eventName2) && eventName2 == eventName)
						{
							EBTStatus eBTStatus = attachmentTask.GetStatus();
							if (eBTStatus == EBTStatus.BT_RUNNING || eBTStatus == EBTStatus.BT_INVALID)
							{
								eBTStatus = attachmentTask.exec(pAgent);
							}
							if (eBTStatus == EBTStatus.BT_SUCCESS)
							{
								if (eventTask.TriggeredOnce())
								{
									return false;
								}
							}
							else if (eBTStatus == EBTStatus.BT_FAILURE)
							{
							}
						}
					}
				}
			}
			return true;
		}

		public virtual bool onevent(Agent pAgent, string eventName)
		{
			return this.m_status != EBTStatus.BT_RUNNING || !this.m_node.HasEvents() || this.CheckEvents(eventName, pAgent);
		}

		~BehaviorTask()
		{
		}

		protected virtual EBTStatus update(Agent pAgent, EBTStatus childStatus)
		{
			return EBTStatus.BT_SUCCESS;
		}

		protected virtual bool onenter(Agent pAgent)
		{
			return true;
		}

		protected virtual void onexit(Agent pAgent, EBTStatus status)
		{
		}

		protected virtual bool isContinueTicking()
		{
			return false;
		}

		private void Attach(AttachmentTask pAttachment)
		{
			if (this.m_attachments == null)
			{
				this.m_attachments = new ListView<AttachmentTask>();
			}
			this.m_attachments.Add(pAttachment);
		}

		private void InstantiatePars(Agent pAgent)
		{
			BehaviorNode node = this.m_node;
			if (node != null && node.m_pars != null)
			{
				for (int i = 0; i < node.m_pars.Count; i++)
				{
					Property property = node.m_pars[i];
					property.Instantiate(pAgent);
				}
			}
		}

		private void UnInstantiatePars(Agent pAgent)
		{
			BehaviorNode node = this.m_node;
			if (node != null && node.m_pars != null)
			{
				for (int i = 0; i < node.m_pars.Count; i++)
				{
					Property property = node.m_pars[i];
					property.UnInstantiate(pAgent);
				}
			}
		}

		public bool onenter_action(Agent pAgent)
		{
			this.InstantiatePars(pAgent);
			bool flag = this.onenter(pAgent);
			if (this.m_node != null && !this.m_node.enteraction_impl(pAgent) && this.m_node.m_enterAction != null)
			{
				ParentType parentType = this.m_node.m_enterAction.GetParentType();
				Agent agent = pAgent;
				if (parentType == ParentType.PT_INSTANCE)
				{
					agent = Agent.GetInstance(this.m_node.m_enterAction.GetInstanceNameString(), agent.GetContextId());
				}
				this.m_node.m_enterAction.run(agent, pAgent);
			}
			if (!flag)
			{
				this.UnInstantiatePars(pAgent);
			}
			return flag;
		}

		public static EBTStatus GetNodeExitStatus()
		{
			return BehaviorTask.ms_lastExitStatus_;
		}

		public void onexit_action(Agent pAgent, EBTStatus status)
		{
			this.onexit(pAgent, status);
			this.SetReturnStatus(EBTStatus.BT_INVALID);
			if (this.m_node != null)
			{
				bool flag = this.m_node.exitaction_impl(pAgent);
				if (flag || this.m_node.m_exitAction != null)
				{
					Agent agent = pAgent;
					if (!flag && this.m_node.m_exitAction != null)
					{
						ParentType parentType = this.m_node.m_exitAction.GetParentType();
						if (parentType == ParentType.PT_INSTANCE)
						{
							agent = Agent.GetInstance(this.m_node.m_exitAction.GetInstanceNameString(), agent.GetContextId());
						}
					}
					if (!flag && this.m_node.m_exitAction != null)
					{
						BehaviorTask.ms_lastExitStatus_ = status;
						this.m_node.m_exitAction.run(agent, pAgent);
						BehaviorTask.ms_lastExitStatus_ = EBTStatus.BT_INVALID;
					}
				}
			}
			this.UnInstantiatePars(pAgent);
		}

		public virtual ListView<BehaviorNode> GetRunningNodes()
		{
			return new ListView<BehaviorNode>();
		}
	}
}
