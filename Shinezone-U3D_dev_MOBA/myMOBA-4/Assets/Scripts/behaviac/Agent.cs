using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using UnityEngine;

namespace behaviac
{
	public class Agent : MonoBehaviour
	{
		public class State_t
		{
			protected Variables m_vars = new Variables();

			protected BehaviorTreeTask m_bt;

			public Variables Vars
			{
				get
				{
					return this.m_vars;
				}
			}

			public BehaviorTreeTask BT
			{
				get
				{
					return this.m_bt;
				}
				set
				{
					this.m_bt = value;
				}
			}

			public State_t()
			{
			}

			public State_t(Agent.State_t c)
			{
				c.m_vars.CopyTo(null, this.m_vars);
				if (c.m_bt != null)
				{
					BehaviorNode node = c.m_bt.GetNode();
					this.m_bt = (BehaviorTreeTask)node.CreateAndInitTask();
					c.m_bt.CopyTo(this.m_bt);
				}
			}

			~State_t()
			{
				this.Clear();
			}

			public void Clear()
			{
				this.m_vars.Clear();
				this.m_bt = null;
			}

			public bool SaveToFile(string fileName)
			{
				return false;
			}

			public bool LoadFromFile(string fileName)
			{
				return false;
			}
		}

		private class BehaviorTreeStackItem_t
		{
			public BehaviorTreeTask bt;

			public TriggerMode triggerMode;

			public bool triggerByEvent;

			public BehaviorTreeStackItem_t(BehaviorTreeTask bt_, TriggerMode tm, bool bByEvent)
			{
				this.bt = bt_;
				this.triggerMode = tm;
				this.triggerByEvent = bByEvent;
			}
		}

		public struct AgentName_t
		{
			public string instantceName_;

			public string className_;

			public string displayName_;

			public string desc_;

			public string ClassName
			{
				get
				{
					return this.className_;
				}
			}

			public AgentName_t(string instanceName, string className, string displayName, string desc)
			{
				this.instantceName_ = instanceName;
				this.className_ = className;
				if (!string.IsNullOrEmpty(displayName))
				{
					this.displayName_ = displayName;
				}
				else
				{
					this.displayName_ = this.instantceName_.Replace(".", "::");
				}
				if (!string.IsNullOrEmpty(desc))
				{
					this.desc_ = desc;
				}
				else
				{
					this.desc_ = this.displayName_;
				}
			}
		}

		public class CTagObjectDescriptor
		{
			public ListView<CMemberBase> ms_members = new ListView<CMemberBase>();

			public ListView<CMethodBase> ms_methods = new ListView<CMethodBase>();

			public Type type;

			public string displayName;

			public string desc;

			public Agent.CTagObjectDescriptor m_parent;

			public void Load(Agent parent, ISerializableNode node)
			{
				foreach (CMemberBase current in this.ms_members)
				{
					current.Load(parent, node);
				}
				if (this.m_parent != null)
				{
					this.m_parent.Load(parent, node);
				}
			}

			public void Save(Agent parent, ISerializableNode node)
			{
				if (this.m_parent != null)
				{
					this.m_parent.Save(parent, node);
				}
				foreach (CMemberBase current in this.ms_members)
				{
					current.Save(parent, node);
				}
			}

			public CMemberBase GetMember(string memberName)
			{
				if (this.ms_members != null)
				{
					for (int i = 0; i < this.ms_members.Count; i++)
					{
						CMemberBase cMemberBase = this.ms_members[i];
						if (cMemberBase.GetName() == memberName)
						{
							return cMemberBase;
						}
					}
				}
				if (this.m_parent != null)
				{
					return this.m_parent.GetMember(memberName);
				}
				return null;
			}
		}

		[TypeConverter]
		public class StructConverter : TypeConverter
		{
			public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
			{
				return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
			}

			public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
			{
				if (value is string)
				{
				}
				return base.ConvertFrom(context, culture, value);
			}

			public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
			{
				if (destinationType == typeof(string))
				{
				}
				return base.ConvertTo(context, culture, value, destinationType);
			}
		}

		private ListView<BehaviorTreeTask> m_behaviorTreeTasks;

		private ListView<Agent.BehaviorTreeStackItem_t> m_btStack;

		private BehaviorTreeTask m_currentBT;

		private int m_id = -1;

		private bool m_bActive = true;

		private bool m_referencetree;

		public int m_priority;

		public int m_contextId;

		private static uint ms_idMask;

		private uint m_idFlag;

		public Variables m_variables;

		private static int ms_agent_index;

		private static Dictionary<string, int> ms_agent_type_index;

		private static Dictionary<string, Agent.AgentName_t> ms_names;

		private Agent.CTagObjectDescriptor m_objectDescriptor;

		private static DictionaryView<CStringID, Agent.CTagObjectDescriptor> ms_metas;

		private DictionaryView<CStringID, CNamedEvent> m_eventInfos;

		private ListView<BehaviorTreeTask> BehaviorTreeTasks
		{
			get
			{
				if (this.m_behaviorTreeTasks == null)
				{
					this.m_behaviorTreeTasks = new ListView<BehaviorTreeTask>();
				}
				return this.m_behaviorTreeTasks;
			}
		}

		private ListView<Agent.BehaviorTreeStackItem_t> BTStack
		{
			get
			{
				if (this.m_btStack == null)
				{
					this.m_btStack = new ListView<Agent.BehaviorTreeStackItem_t>();
				}
				return this.m_btStack;
			}
		}

		public Variables Variables
		{
			get
			{
				if (this.m_variables == null)
				{
					this.m_variables = new Variables();
				}
				return this.m_variables;
			}
		}

		public static Dictionary<string, AgentName_t> Names
		{
			get
			{
				if (Agent.ms_names == null)
				{
					Agent.ms_names = new Dictionary<string, AgentName_t>();
				}
				return Agent.ms_names;
			}
		}

		public static DictionaryView<CStringID, Agent.CTagObjectDescriptor> Metas
		{
			get
			{
				if (Agent.ms_metas == null)
				{
					Agent.ms_metas = new DictionaryView<CStringID, Agent.CTagObjectDescriptor>();
				}
				return Agent.ms_metas;
			}
		}

		private DictionaryView<CStringID, CNamedEvent> EventInfos
		{
			get
			{
				if (this.m_eventInfos == null)
				{
					this.m_eventInfos = new DictionaryView<CStringID, CNamedEvent>();
				}
				return this.m_eventInfos;
			}
		}

		protected void Init()
		{
			Agent.Init_(this.m_contextId, this, this.m_priority, base.name);
		}

		protected virtual void OnDestroy()
		{
			this.UnSubsribeToNetwork();
			if (this.m_contextId >= 0)
			{
				Context context = Context.GetContext(this.m_contextId);
				World world = context.GetWorld(false);
				if (!object.ReferenceEquals(world, null) && !object.ReferenceEquals(world, this))
				{
					world.RemoveAgent(this);
				}
			}
			if (this.m_behaviorTreeTasks != null)
			{
				for (int i = 0; i < this.m_behaviorTreeTasks.Count; i++)
				{
					BehaviorTreeTask behaviorTreeTask = this.m_behaviorTreeTasks[i];
					Workspace.DestroyBehaviorTreeTask(behaviorTreeTask, this);
				}
				this.m_behaviorTreeTasks.Clear();
				this.m_behaviorTreeTasks = null;
			}
			if (this.m_eventInfos != null)
			{
				this.m_eventInfos.Clear();
				this.m_eventInfos = null;
			}
		}

		public int GetId()
		{
			return this.m_id;
		}

		public int GetPriority()
		{
			return this.m_priority;
		}

		public string GetClassTypeName()
		{
			return base.GetType().FullName;
		}

		public bool IsMasked()
		{
			return (this.m_idFlag & Agent.IdMask()) != 0u;
		}

		public void SetIdFlag(uint idMask)
		{
			this.m_idFlag = idMask;
		}

		public static bool IsDerived(Agent pAgent, string agentType)
		{
			bool result = false;
			for (Type type = pAgent.GetType(); type != null; type = type.BaseType)
			{
				if (type.FullName == agentType)
				{
					result = true;
					break;
				}
			}
			return result;
		}

		public static void SetIdMask(uint idMask)
		{
			Agent.ms_idMask = idMask;
		}

		public static uint IdMask()
		{
			return Agent.ms_idMask;
		}

		public string GetName()
		{
			return base.name;
		}

		public void SetName(string instanceName)
		{
			if (string.IsNullOrEmpty(instanceName))
			{
				string fullName = base.GetType().FullName;
				int num = fullName.LastIndexOf(':');
				string arg;
				if (num != -1)
				{
					arg = fullName.Substring(num + 1);
				}
				else
				{
					arg = fullName;
				}
				if (Agent.ms_agent_type_index == null)
				{
					Agent.ms_agent_type_index = new Dictionary<string, int>();
				}
				int num2;
				if (!Agent.ms_agent_type_index.ContainsKey(fullName))
				{
					num2 = 0;
					Agent.ms_agent_type_index[fullName] = 1;
				}
				else
				{
					Dictionary<string, int> dictionary;
					Dictionary<string, int> expr_78 = dictionary = Agent.ms_agent_type_index;
					string key;
					string expr_7C = key = fullName;
					int num3 = dictionary[key];
					int num4;
					expr_78[expr_7C] = (num4 = num3) + 1;
					num2 = num4;
				}
				base.name += string.Format("{0}_{1}_{2}", arg, num2, this.m_id);
			}
			else
			{
				base.name = instanceName;
			}
		}

		public int GetContextId()
		{
			return this.m_contextId;
		}

		public bool IsActive()
		{
			return this.m_bActive;
		}

		private void CustomSetActive(bool bActive)
		{
			this.m_bActive = bActive;
		}

		public void SetVariableRegistry(CMemberBase pMember, string variableName, object value, string staticClassName, uint variableId)
		{
			bool flag = !string.IsNullOrEmpty(variableName);
			if (flag)
			{
				if (!string.IsNullOrEmpty(staticClassName))
				{
					int contextId = this.GetContextId();
					Context context = Context.GetContext(contextId);
					context.SetStaticVariable<object>(pMember, variableName, value, staticClassName, variableId);
				}
				else
				{
					this.Variables.Set(this, pMember, variableName, value, variableId);
				}
			}
		}

		public static bool IsAgentClassName(CStringID agentClassId)
		{
			return Agent.Metas.ContainsKey(agentClassId);
		}

		public static bool IsAgentClassName(string agentClassName)
		{
			CStringID agentClassId = new CStringID(agentClassName);
			return Agent.IsAgentClassName(agentClassId);
		}

		public static bool RegisterName<TAGENT>(string agentInstanceName, string displayName, string desc) where TAGENT : Agent
		{
			string text = agentInstanceName;
			if (string.IsNullOrEmpty(text))
			{
				text = typeof(TAGENT).FullName;
			}
			if (!Agent.Names.ContainsKey(text))
			{
				string fullName = typeof(TAGENT).FullName;
				Agent.Names[text] = new Agent.AgentName_t(text, fullName, displayName, desc);
				return true;
			}
			return false;
		}

		public static bool RegisterName<TAGENT>(string agentInstanceName) where TAGENT : Agent
		{
			return Agent.RegisterName<TAGENT>(agentInstanceName, null, null);
		}

		public static bool RegisterName<TAGENT>() where TAGENT : Agent
		{
			return Agent.RegisterName<TAGENT>(null, null, null);
		}

		public static bool RegisterStaticClass(Type type, string displayName, string desc)
		{
			string fullName = type.FullName;
			if (!Agent.Names.ContainsKey(fullName))
			{
				Agent.Names[fullName] = new Agent.AgentName_t(fullName, fullName, displayName, desc);
				Utils.AddStaticClass(type);
				return true;
			}
			return false;
		}

		public static void UnRegisterName<TAGENT>(string agentInstanceName) where TAGENT : Agent
		{
			string text = agentInstanceName;
			if (string.IsNullOrEmpty(text))
			{
				text = typeof(TAGENT).FullName;
			}
			if (Agent.Names.ContainsKey(text))
			{
				Agent.Names.Remove(text);
			}
		}

		public static void UnRegisterName<TAGENT>() where TAGENT : Agent
		{
			Agent.UnRegisterName<TAGENT>(null);
		}

		public static bool IsNameRegistered(string agentInstanceName)
		{
			return Agent.Names.ContainsKey(agentInstanceName);
		}

		public static string GetRegisteredClassName(string agentInstanceName)
		{
			if (Agent.Names.ContainsKey(agentInstanceName))
			{
				return Agent.Names[agentInstanceName].ClassName;
			}
			return null;
		}

		public static bool BindInstance(Agent pAgentInstance, string agentInstanceName, int contextId)
		{
			Context context = Context.GetContext(contextId);
			return context.BindInstance(pAgentInstance, agentInstanceName);
		}

		public static bool BindInstance(Agent pAgentInstance, string agentInstanceName)
		{
			return Agent.BindInstance(pAgentInstance, agentInstanceName, 0);
		}

		public static bool BindInstance(Agent pAgentInstance)
		{
			return Agent.BindInstance(pAgentInstance, null, 0);
		}

		public static bool UnbindInstance(string agentInstanceName, int contextId)
		{
			Context context = Context.GetContext(contextId);
			return context.UnbindInstance(agentInstanceName);
		}

		public static bool UnbindInstance(string agentInstanceName)
		{
			return Agent.UnbindInstance(agentInstanceName, 0);
		}

		public static bool UnbindInstance<T>()
		{
			string fullName = typeof(T).FullName;
			return Agent.UnbindInstance(fullName);
		}

		public static Agent GetInstance(string agentInstanceName, int contextId)
		{
			Context context = Context.GetContext(contextId);
			return context.GetInstance(agentInstanceName);
		}

		public static Agent GetInstance(string agentInstanceName)
		{
			return Agent.GetInstance(agentInstanceName, 0);
		}

		public static TAGENT GetInstance<TAGENT>(string agentInstanceName, int contextId) where TAGENT : Agent, new()
		{
			string text = agentInstanceName;
			if (string.IsNullOrEmpty(text))
			{
				text = typeof(TAGENT).FullName;
			}
			Agent instance = Agent.GetInstance(text, contextId);
			return (TAGENT)((object)instance);
		}

		public static TAGENT GetInstance<TAGENT>(string agentInstanceName) where TAGENT : Agent, new()
		{
			return Agent.GetInstance<TAGENT>(agentInstanceName, 0);
		}

		public static TAGENT GetInstance<TAGENT>() where TAGENT : Agent, new()
		{
			return Agent.GetInstance<TAGENT>(null, 0);
		}

		public static Agent.CTagObjectDescriptor GetDescriptorByName(string className)
		{
			CStringID key = new CStringID(className);
			if (Agent.Metas.ContainsKey(key))
			{
				return Agent.Metas[key];
			}
			Agent.CTagObjectDescriptor cTagObjectDescriptor = new Agent.CTagObjectDescriptor();
			Agent.Metas.Add(key, cTagObjectDescriptor);
			return cTagObjectDescriptor;
		}

		public Agent.CTagObjectDescriptor GetDescriptor()
		{
			if (this.m_objectDescriptor == null)
			{
				this.m_objectDescriptor = Agent.GetDescriptorByName(base.GetType().FullName);
			}
			return this.m_objectDescriptor;
		}

		public static bool IsTypeRegisterd(string typeName)
		{
			CStringID key = new CStringID(typeName);
			return Agent.ms_metas.ContainsKey(key);
		}

		public CMemberBase FindMember(string propertyName)
		{
			uint propertyId = Utils.MakeVariableId(propertyName);
			return this.FindMember(propertyId);
		}

		public CMemberBase FindMember(uint propertyId)
		{
			Agent.CTagObjectDescriptor descriptor = this.GetDescriptor();
			for (int i = 0; i < descriptor.ms_members.Count; i++)
			{
				CMemberBase cMemberBase = descriptor.ms_members[i];
				if (cMemberBase.GetId().GetId() == propertyId)
				{
					return cMemberBase;
				}
			}
			return null;
		}

		private static int ParsePropertyNames(string fullPropertnName, ref string agentClassName)
		{
			int num = fullPropertnName.LastIndexOf(':');
			if (num != -1)
			{
				num++;
				int result = num - 2;
				agentClassName = fullPropertnName.Substring(num);
				return result;
			}
			return -1;
		}

		public static CMemberBase FindMemberBase(string propertyName)
		{
			string str = null;
			int num = Agent.ParsePropertyNames(propertyName, ref str);
			if (num != -1)
			{
				string str2 = propertyName.Substring(0, num).Replace("::", ".");
				CStringID agentClassId = new CStringID(str2);
				CStringID propertyId = new CStringID(str);
				return Agent.FindMemberBase(agentClassId, propertyId);
			}
			return null;
		}

		public static CMethodBase FindMethodBase(string propertyName)
		{
			int num = propertyName.LastIndexOf(':');
			if (num != -1)
			{
				string str = propertyName.Substring(0, num - 1);
				CStringID agentClassId = new CStringID(str);
				CStringID propertyId = new CStringID(propertyName.Substring(num + 1));
				return Agent.FindMethodBase(agentClassId, propertyId);
			}
			return null;
		}

		public static CMemberBase FindMemberBase(CStringID agentClassId, CStringID propertyId)
		{
			if (Agent.Metas.ContainsKey(agentClassId))
			{
				Agent.CTagObjectDescriptor cTagObjectDescriptor = Agent.Metas[agentClassId];
				for (int i = 0; i < cTagObjectDescriptor.ms_members.Count; i++)
				{
					CMemberBase cMemberBase = cTagObjectDescriptor.ms_members[i];
					if (cMemberBase.GetId() == propertyId)
					{
						return cMemberBase;
					}
				}
				if (cTagObjectDescriptor.type.BaseType != null)
				{
					CStringID agentClassId2 = new CStringID(cTagObjectDescriptor.type.BaseType.FullName);
					return Agent.FindMemberBase(agentClassId2, propertyId);
				}
			}
			return null;
		}

		public static CMethodBase FindMethodBase(CStringID agentClassId, CStringID propertyId)
		{
			if (Agent.Metas.ContainsKey(agentClassId))
			{
				Agent.CTagObjectDescriptor cTagObjectDescriptor = Agent.Metas[agentClassId];
				for (int i = 0; i < cTagObjectDescriptor.ms_methods.Count; i++)
				{
					CMethodBase cMethodBase = cTagObjectDescriptor.ms_methods[i];
					if (cMethodBase.GetId() == propertyId)
					{
						return cMethodBase;
					}
				}
				if (cTagObjectDescriptor.type != null && cTagObjectDescriptor.type.BaseType != null)
				{
					CStringID agentClassId2 = new CStringID(cTagObjectDescriptor.type.BaseType.FullName);
					return Agent.FindMethodBase(agentClassId2, propertyId);
				}
			}
			return null;
		}

		public static Property CreateProperty(string typeName, string propertyName, string defaultValue)
		{
			CMemberBase cMemberBase = Agent.FindMemberBase(propertyName);
			if (cMemberBase != null)
			{
				return cMemberBase.CreateProperty(defaultValue, false);
			}
			return null;
		}

		public static CMethodBase CreateMethod(CStringID agentClassId, CStringID methodClassId)
		{
			CMethodBase cMethodBase = Agent.FindMethodBase(agentClassId, methodClassId);
			if (cMethodBase != null)
			{
				return cMethodBase.clone();
			}
			return null;
		}

		public object GetVariable(string variableName)
		{
			uint varId = Utils.MakeVariableId(variableName);
			return this.Variables.Get(this, varId);
		}

		public object GetVariable(uint variableId)
		{
			return this.Variables.Get(this, variableId);
		}

		public void SetVariable<VariableType>(string variableName, VariableType value)
		{
			uint variableId = Utils.MakeVariableId(variableName);
			this.SetVariable<VariableType>(variableName, value, variableId);
		}

		public void SetVariable<VariableType>(string variableName, VariableType value, uint variableId)
		{
			if (variableId == 0u)
			{
				variableId = Utils.MakeVariableId(variableName);
			}
			this.Variables.Set(this, null, variableName, value, variableId);
		}

		public void SetVariableFromString(string variableName, string valueStr)
		{
			this.Variables.SetFromString(this, variableName, valueStr);
		}

		public void Instantiate<VariableType>(VariableType value, Property property_)
		{
			this.Variables.Instantiate(property_, value);
		}

		public void UnInstantiate(string variableName)
		{
			this.Variables.UnInstantiate(variableName);
		}

		public void UnLoad(string variableName)
		{
			this.Variables.UnLoad(variableName);
		}

		protected static void Init_(int contextId, Agent pAgent, int priority, string agentInstanceName)
		{
			pAgent.m_contextId = contextId;
			pAgent.m_id = Agent.ms_agent_index++;
			pAgent.m_priority = priority;
			pAgent.SetName(agentInstanceName);
			pAgent.InitVariableRegistry();
			Context context = Context.GetContext(contextId);
			World world = context.GetWorld(true);
			if (!object.ReferenceEquals(world, null) && !object.ReferenceEquals(world, pAgent))
			{
				world.AddAgent(pAgent);
			}
			pAgent.SubsribeToNetwork();
		}

		public void btresetcurrrent()
		{
			if (this.m_currentBT != null)
			{
				this.m_currentBT.reset(this);
			}
		}

		public void btsetcurrent(string relativePath)
		{
			this._btsetcurrent(relativePath, TriggerMode.TM_Transfer, false);
		}

		public void btreferencetree(string relativePath)
		{
			this._btsetcurrent(relativePath, TriggerMode.TM_Return, false);
			this.m_referencetree = true;
		}

		public void bteventtree(string relativePath, TriggerMode triggerMode)
		{
			this._btsetcurrent(relativePath, triggerMode, true);
		}

		private void _btsetcurrent(string relativePath, TriggerMode triggerMode, bool bByEvent)
		{
			if (!string.IsNullOrEmpty(relativePath))
			{
				if (!Workspace.Load(relativePath))
				{
					string str = base.GetType().FullName;
					str += "::";
					str += base.name;
				}
				else
				{
					Workspace.RecordBTAgentMapping(relativePath, this);
					if (this.m_currentBT != null)
					{
						if (triggerMode == TriggerMode.TM_Return)
						{
							Agent.BehaviorTreeStackItem_t item = new Agent.BehaviorTreeStackItem_t(this.m_currentBT, triggerMode, bByEvent);
							this.BTStack.Add(item);
						}
						else if (triggerMode == TriggerMode.TM_Transfer)
						{
							this.m_currentBT.abort(this);
							this.m_currentBT.reset(this);
						}
					}
					BehaviorTreeTask behaviorTreeTask = null;
					for (int i = 0; i < this.BehaviorTreeTasks.Count; i++)
					{
						BehaviorTreeTask behaviorTreeTask2 = this.BehaviorTreeTasks[i];
						if (behaviorTreeTask2.GetName() == relativePath)
						{
							behaviorTreeTask = behaviorTreeTask2;
							break;
						}
					}
					bool flag = false;
					if (behaviorTreeTask != null && this.BTStack.Count > 0)
					{
						for (int j = 0; j < this.BTStack.Count; j++)
						{
							Agent.BehaviorTreeStackItem_t behaviorTreeStackItem_t = this.BTStack[j];
							if (behaviorTreeStackItem_t.bt.GetName() == relativePath)
							{
								flag = true;
								break;
							}
						}
					}
					if (behaviorTreeTask == null || flag)
					{
						behaviorTreeTask = Workspace.CreateBehaviorTreeTask(relativePath);
						this.BehaviorTreeTasks.Add(behaviorTreeTask);
					}
					this.m_currentBT = behaviorTreeTask;
				}
			}
		}

		private EBTStatus btexec_()
		{
			if (this.m_currentBT == null)
			{
				return EBTStatus.BT_INVALID;
			}
			EBTStatus eBTStatus = this.m_currentBT.exec(this);
			if (this == null)
			{
				return EBTStatus.BT_FAILURE;
			}
			while (eBTStatus != EBTStatus.BT_RUNNING)
			{
				this.m_currentBT.reset(this);
				if (this.BTStack.Count <= 0)
				{
					break;
				}
				Agent.BehaviorTreeStackItem_t behaviorTreeStackItem_t = this.BTStack[this.BTStack.Count - 1];
				this.m_currentBT = behaviorTreeStackItem_t.bt;
				this.BTStack.RemoveAt(this.BTStack.Count - 1);
				if (behaviorTreeStackItem_t.triggerMode != TriggerMode.TM_Return)
				{
					eBTStatus = this.m_currentBT.exec(this);
					break;
				}
				string name = this.m_currentBT.GetName();
				LogManager.Log(this, name, EActionResult.EAR_none, LogMode.ELM_return);
				if (!behaviorTreeStackItem_t.triggerByEvent)
				{
					this.m_currentBT.resume(this, eBTStatus);
					eBTStatus = this.m_currentBT.exec(this);
				}
			}
			return eBTStatus;
		}

		public virtual EBTStatus btexec()
		{
			if (this.m_bActive)
			{
				this.UpdateVariableRegistry();
				EBTStatus eBTStatus = this.btexec_();
				while (this.m_referencetree && eBTStatus == EBTStatus.BT_RUNNING)
				{
					this.m_referencetree = false;
					eBTStatus = this.btexec_();
				}
				if (this.IsMasked())
				{
					this.LogVariables(false);
				}
				return eBTStatus;
			}
			return EBTStatus.BT_INVALID;
		}

		public void btonevent(string btEvent)
		{
			if (this.m_currentBT != null)
			{
				this.m_currentBT.onevent(this, btEvent);
			}
		}

		public BehaviorTreeTask btgetcurrent()
		{
			return this.m_currentBT;
		}

		public bool btload(string relativePath, bool bForce)
		{
			bool flag = Workspace.Load(relativePath, bForce);
			if (flag)
			{
				Workspace.RecordBTAgentMapping(relativePath, this);
			}
			return flag;
		}

		public bool btload(string relativePath)
		{
			return this.btload(relativePath, false);
		}

		public void btunload(string relativePath)
		{
			if (this.m_currentBT != null && this.m_currentBT.GetName() == relativePath)
			{
				BehaviorNode node = this.m_currentBT.GetNode();
				BehaviorTree bt = node as BehaviorTree;
				this.btunload_pars(bt);
				this.m_currentBT = null;
			}
			for (int i = 0; i < this.BTStack.Count; i++)
			{
				Agent.BehaviorTreeStackItem_t behaviorTreeStackItem_t = this.BTStack[i];
				if (behaviorTreeStackItem_t.bt.GetName() == relativePath)
				{
					this.BTStack.Remove(behaviorTreeStackItem_t);
					break;
				}
			}
			for (int j = 0; j < this.BehaviorTreeTasks.Count; j++)
			{
				BehaviorTreeTask behaviorTreeTask = this.BehaviorTreeTasks[j];
				if (behaviorTreeTask.GetName() == relativePath)
				{
					Workspace.DestroyBehaviorTreeTask(behaviorTreeTask, this);
					this.BehaviorTreeTasks.Remove(behaviorTreeTask);
					break;
				}
			}
			Workspace.UnLoad(relativePath);
		}

		public virtual void bthotreloaded(BehaviorTree bt)
		{
			this.btunload_pars(bt);
		}

		public void btunloadall()
		{
			ListView<BehaviorTree> listView = new ListView<BehaviorTree>();
			foreach (BehaviorTreeTask current in this.BehaviorTreeTasks)
			{
				BehaviorNode node = current.GetNode();
				BehaviorTree behaviorTree = (BehaviorTree)node;
				bool flag = false;
				foreach (BehaviorTree current2 in listView)
				{
					if (current2 == behaviorTree)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					listView.Add(behaviorTree);
				}
				Workspace.DestroyBehaviorTreeTask(current, this);
			}
			foreach (BehaviorTree current3 in listView)
			{
				this.btunload_pars(current3);
				Workspace.UnLoad(current3.GetName());
			}
			this.BehaviorTreeTasks.Clear();
			this.m_currentBT = null;
			this.BTStack.Clear();
			this.Variables.Unload();
		}

		public void btreloadall()
		{
			this.m_currentBT = null;
			this.BTStack.Clear();
			if (this.m_behaviorTreeTasks != null)
			{
				List<string> list = new List<string>();
				foreach (BehaviorTreeTask current in this.m_behaviorTreeTasks)
				{
					string name = current.GetName();
					if (list.IndexOf(name) == -1)
					{
						list.Add(name);
					}
				}
				for (int i = 0; i < list.Count; i++)
				{
					string relativePath = list[i];
					Workspace.Load(relativePath, true);
				}
				this.BehaviorTreeTasks.Clear();
			}
			this.Variables.Unload();
		}

		public bool btsave(Agent.State_t state)
		{
			this.m_variables.CopyTo(null, state.Vars);
			if (this.m_currentBT != null)
			{
				Workspace.DestroyBehaviorTreeTask(state.BT, this);
				BehaviorNode node = this.m_currentBT.GetNode();
				state.BT = (BehaviorTreeTask)node.CreateAndInitTask();
				this.m_currentBT.CopyTo(state.BT);
				return true;
			}
			return false;
		}

		public bool btload(Agent.State_t state)
		{
			state.Vars.CopyTo(this, this.m_variables);
			if (state.BT != null)
			{
				if (this.m_currentBT != null)
				{
					for (int i = 0; i < this.m_behaviorTreeTasks.Count; i++)
					{
						BehaviorTreeTask behaviorTreeTask = this.m_behaviorTreeTasks[i];
						if (behaviorTreeTask == this.m_currentBT)
						{
							Workspace.DestroyBehaviorTreeTask(behaviorTreeTask, this);
							this.m_behaviorTreeTasks.Remove(behaviorTreeTask);
							break;
						}
					}
				}
				BehaviorNode node = state.BT.GetNode();
				this.m_currentBT = (BehaviorTreeTask)node.CreateAndInitTask();
				state.BT.CopyTo(this.m_currentBT);
				return true;
			}
			return false;
		}

		private void btunload_pars(BehaviorTree bt)
		{
			if (bt.m_pars != null)
			{
				for (int i = 0; i < bt.m_pars.Count; i++)
				{
					Property property = bt.m_pars[i];
					property.UnLoad(this);
				}
			}
		}

		private static CNamedEvent findNamedEventTemplate(ListView<CMethodBase> methods, string eventName, int context_id)
		{
			Context context = Context.GetContext(context_id);
			return context.FindNamedEventTemplate(methods, eventName);
		}

		private CNamedEvent findEvent(string eventName)
		{
			Agent.CTagObjectDescriptor descriptor = this.GetDescriptor();
			int contextId = this.GetContextId();
			CNamedEvent cNamedEvent = Agent.findNamedEventTemplate(descriptor.ms_methods, eventName, contextId);
			if (cNamedEvent != null)
			{
				CNamedEvent cNamedEvent2 = (CNamedEvent)cNamedEvent.clone();
				CStringID key = new CStringID(eventName);
				this.EventInfos[key] = cNamedEvent2;
				return cNamedEvent2;
			}
			return null;
		}

		public void FireEvent(string eventName)
		{
			CNamedEvent cNamedEvent = this.findEvent(eventName);
			if (cNamedEvent == null)
			{
				int contextId = this.GetContextId();
				Agent.CTagObjectDescriptor descriptor = this.GetDescriptor();
				cNamedEvent = Agent.findNamedEventTemplate(descriptor.ms_methods, eventName, contextId);
			}
			if (cNamedEvent != null)
			{
				cNamedEvent.SetFired(this, true);
			}
		}

		public void FireEvent<ParamType>(string eventName, ParamType param)
		{
			CNamedEvent cNamedEvent = this.findEvent(eventName);
			if (cNamedEvent == null)
			{
				int contextId = this.GetContextId();
				Agent.CTagObjectDescriptor descriptor = this.GetDescriptor();
				cNamedEvent = Agent.findNamedEventTemplate(descriptor.ms_methods, eventName, contextId);
			}
			if (cNamedEvent != null)
			{
				cNamedEvent.SetParam<ParamType>(this, param);
				cNamedEvent.SetFired(this, true);
			}
		}

		public void FireEvent<ParamType1, ParamType2>(string eventName, ParamType1 param1, ParamType2 param2)
		{
			CNamedEvent cNamedEvent = this.findEvent(eventName);
			if (cNamedEvent == null)
			{
				int contextId = this.GetContextId();
				Agent.CTagObjectDescriptor descriptor = this.GetDescriptor();
				cNamedEvent = Agent.findNamedEventTemplate(descriptor.ms_methods, eventName, contextId);
			}
			if (cNamedEvent != null)
			{
				cNamedEvent.SetParam<ParamType1, ParamType2>(this, param1, param2);
				cNamedEvent.SetFired(this, true);
			}
		}

		public void FireEvent<ParamType1, ParamType2, ParamType3>(string eventName, ParamType1 param1, ParamType2 param2, ParamType3 param3)
		{
			CNamedEvent cNamedEvent = this.findEvent(eventName);
			if (cNamedEvent == null)
			{
				int contextId = this.GetContextId();
				Agent.CTagObjectDescriptor descriptor = this.GetDescriptor();
				cNamedEvent = Agent.findNamedEventTemplate(descriptor.ms_methods, eventName, contextId);
			}
			if (cNamedEvent != null)
			{
				cNamedEvent.SetParam<ParamType1, ParamType2, ParamType3>(this, param1, param2, param3);
				cNamedEvent.SetFired(this, true);
			}
		}

		public bool IsFired(string eventName)
		{
			return false;
		}

		public void ResetEvent(string eventName)
		{
			CStringID key = new CStringID(eventName);
			if (this.EventInfos.ContainsKey(key))
			{
				CNamedEvent cNamedEvent = this.EventInfos[key];
				cNamedEvent.SetFired(this, false);
				return;
			}
			int contextId = this.GetContextId();
			Agent.CTagObjectDescriptor descriptor = this.GetDescriptor();
			CNamedEvent cNamedEvent2 = Agent.findNamedEventTemplate(descriptor.ms_methods, eventName, contextId);
			if (cNamedEvent2 != null)
			{
				cNamedEvent2.SetFired(this, false);
			}
		}

		public void LogVariables(bool bForce)
		{
		}

		private void ResetChangedVariables()
		{
			this.Variables.Reset();
		}

		private void InitVariableRegistry()
		{
			this.ResetChangedVariables();
		}

		private void UpdateVariableRegistry()
		{
			this.ReplicateProperties();
		}

		private void Save(CPropertyNode node)
		{
		}

		private void SubsribeToNetwork()
		{
		}

		private void UnSubsribeToNetwork()
		{
		}

		private void ReplicateProperties()
		{
		}

		public static Type GetTypeFromName(string typeName)
		{
			CStringID key = new CStringID(typeName);
			if (Agent.Metas.ContainsKey(key))
			{
				Agent.CTagObjectDescriptor cTagObjectDescriptor = Agent.Metas[key];
				return cTagObjectDescriptor.type;
			}
			return null;
		}
	}
}
