using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;

namespace behaviac
{
	public abstract class BehaviorNode
	{
		private string m_className;

		private int m_id;

		protected ListView<BehaviorNode> m_attachments;

		public ListView<Property> m_pars;

		protected BehaviorNode m_parent;

		protected ListView<BehaviorNode> m_children;

		public CMethodBase m_enterAction;

		public CMethodBase m_exitAction;

		protected bool m_bHasEvents;

		public BehaviorTask CreateAndInitTask()
		{
			BehaviorTask behaviorTask = this.createTask();
			behaviorTask.Init(this);
			return behaviorTask;
		}

		public bool HasEvents()
		{
			return this.m_bHasEvents;
		}

		public void SetHasEvents(bool hasEvents)
		{
			this.m_bHasEvents = hasEvents;
		}

		public int GetChildrenCount()
		{
			if (this.m_children != null)
			{
				return this.m_children.Count;
			}
			return 0;
		}

		public BehaviorNode GetChild(int index)
		{
			if (this.m_children != null && index < this.m_children.Count)
			{
				return this.m_children[index];
			}
			return null;
		}

		public int GetAttachmentsCount()
		{
			if (this.m_attachments != null)
			{
				return this.m_attachments.Count;
			}
			return 0;
		}

		public BehaviorNode GetAttachment(int index)
		{
			if (this.m_attachments != null && index < this.m_attachments.Count)
			{
				return this.m_attachments[index];
			}
			return null;
		}

		~BehaviorNode()
		{
			this.Clear();
		}

		public void Clear()
		{
			this.m_enterAction = null;
			this.m_exitAction = null;
			if (this.m_pars != null)
			{
				foreach (Property current in this.m_pars)
				{
					Property.DeleteFromCache(current);
				}
				this.m_pars.Clear();
				this.m_pars = null;
			}
			if (this.m_attachments != null)
			{
				this.m_attachments.Clear();
				this.m_attachments = null;
			}
			if (this.m_children != null)
			{
				this.m_children.Clear();
				this.m_children = null;
			}
		}

		public virtual bool IsValid(Agent pAgent, BehaviorTask pTask)
		{
			return true;
		}

		protected static BehaviorNode Create(string className)
		{
			return Workspace.CreateBehaviorNode(className);
		}

		protected virtual void load(int version, string agentType, List<property_t> properties)
		{
			foreach (property_t current in properties)
			{
				if (current.name == "EnterAction")
				{
					if (!string.IsNullOrEmpty(current.value))
					{
						this.m_enterAction = Action.LoadMethod(current.value);
					}
				}
				else if (current.name == "ExitAction" && !string.IsNullOrEmpty(current.value))
				{
					this.m_exitAction = Action.LoadMethod(current.value);
				}
			}
			string nodeType = this.GetClassNameString().Replace(".", "::");
			Workspace.HandleNodeLoaded(nodeType, properties);
		}

		private void load_par(int version, string agentType, SecurityElement node)
		{
			if (node.Tag != "par")
			{
				return;
			}
			string name = node.Attribute("name");
			string type = node.Attribute("type").Replace("::", ".");
			string value = node.Attribute("value");
			string eventParam = node.Attribute("eventParam");
			this.AddPar(type, name, value, eventParam);
		}

		protected void load_properties_pars_attachments_children(bool bNode, int version, string agentType, SecurityElement node)
		{
			bool flag = this.HasEvents();
			if (node.Children != null)
			{
				List<property_t> list = new List<property_t>();
				foreach (SecurityElement securityElement in node.Children)
				{
					if (!this.load_property_pars(ref list, securityElement, version, agentType) && bNode)
					{
						if (securityElement.Tag == "attachment")
						{
							string text = securityElement.Attribute("class");
							BehaviorNode behaviorNode = BehaviorNode.Create(text);
							if (behaviorNode != null)
							{
								behaviorNode.SetClassNameString(text);
								string value = securityElement.Attribute("id");
								behaviorNode.SetId(Convert.ToInt32(value));
								behaviorNode.load_properties_pars_attachments_children(false, version, agentType, securityElement);
								this.Attach(behaviorNode);
								flag |= (behaviorNode is Event);
							}
						}
						else if (securityElement.Tag == "node")
						{
							BehaviorNode behaviorNode2 = BehaviorNode.load(agentType, securityElement);
							flag |= behaviorNode2.m_bHasEvents;
							this.AddChild(behaviorNode2);
						}
					}
				}
				if (list.Count > 0)
				{
					this.load(version, agentType, list);
				}
			}
			this.m_bHasEvents |= flag;
		}

		private bool load_property_pars(ref List<property_t> properties, SecurityElement c, int version, string agentType)
		{
			if (c.Tag == "property")
			{
				if (c.Attributes.Count == 1)
				{
                    IEnumerator enumerator = c.Attributes.Keys.GetEnumerator();
					{
						if (enumerator.MoveNext())
						{
							string text = (string)enumerator.Current;
							string v = (string)c.Attributes[text];
							property_t item = new property_t(text, v);
							properties.Add(item);
						}
					}
				}
				return true;
			}
			if (c.Tag == "pars")
			{
				if (c.Children != null)
				{
					foreach (SecurityElement securityElement in c.Children)
					{
						if (securityElement.Tag == "par")
						{
							this.load_par(version, agentType, securityElement);
						}
					}
				}
				return true;
			}
			return false;
		}

		protected static BehaviorNode load(string agentType, SecurityElement node)
		{
			int version = int.Parse(node.Attribute("version"));
			string text = node.Attribute("class");
			BehaviorNode behaviorNode = BehaviorNode.Create(text);
			if (behaviorNode != null)
			{
				behaviorNode.SetClassNameString(text);
				string value = node.Attribute("id");
				behaviorNode.SetId(Convert.ToInt32(value));
				behaviorNode.load_properties_pars_attachments_children(true, version, agentType, node);
			}
			return behaviorNode;
		}

		public void Attach(BehaviorNode pAttachment)
		{
			if (this.m_attachments == null)
			{
				this.m_attachments = new ListView<BehaviorNode>();
			}
			this.m_attachments.Add(pAttachment);
		}

		public virtual void AddChild(BehaviorNode pChild)
		{
			pChild.m_parent = this;
			if (this.m_children == null)
			{
				this.m_children = new ListView<BehaviorNode>();
			}
			this.m_children.Add(pChild);
		}

		protected virtual EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			return EBTStatus.BT_FAILURE;
		}

		public void SetClassNameString(string className)
		{
			this.m_className = className;
		}

		public string GetClassNameString()
		{
			return this.m_className;
		}

		public int GetId()
		{
			return this.m_id;
		}

		public void SetId(int id)
		{
			this.m_id = id;
		}

		public void AddPar(string type, string name, string value, string eventParam)
		{
			Property property = Property.Create(type, name, value, false, false);
			if (!string.IsNullOrEmpty(eventParam))
			{
				property.SetRefName(eventParam);
			}
			if (this.m_pars == null)
			{
				this.m_pars = new ListView<Property>();
			}
			this.m_pars.Add(property);
		}

		protected abstract BehaviorTask createTask();

		public virtual bool enteraction_impl(Agent pAgent)
		{
			return false;
		}

		public virtual bool exitaction_impl(Agent pAgent)
		{
			return false;
		}
	}
}
