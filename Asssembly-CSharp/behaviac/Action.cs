using System;
using System.Collections.Generic;

namespace behaviac
{
	public class Action : BehaviorNode
	{
		private class ActionTask : LeafTask
		{
			private static int ms_lastNodeId = -2;

			~ActionTask()
			{
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

			private static void SetNodeId(int nodeId)
			{
				Action.ActionTask.ms_lastNodeId = nodeId;
			}

			private static void ClearNodeId()
			{
				Action.ActionTask.ms_lastNodeId = -2;
			}

			public static int GetNodeId()
			{
				return Action.ActionTask.ms_lastNodeId;
			}

			protected override EBTStatus update(Agent pAgent, EBTStatus childStatus)
			{
				Action action = (Action)base.GetNode();
				if (!this.CheckPredicates(pAgent))
				{
					return action.m_resultPreconditionFail;
				}
				EBTStatus result;
				if (action.m_method != null)
				{
					ParentType parentType = action.m_method.GetParentType();
					Agent agent = pAgent;
					if (parentType == ParentType.PT_INSTANCE)
					{
						agent = Agent.GetInstance(action.m_method.GetInstanceNameString(), agent.GetContextId());
					}
					int id = base.GetId();
					Action.ActionTask.SetNodeId(id);
					object obj = action.m_method.run(agent, pAgent);
					if (action.m_resultOption != EBTStatus.BT_INVALID)
					{
						result = action.m_resultOption;
					}
					else if (action.m_resultFunctor != null)
					{
						ParentType parentType2 = action.m_resultFunctor.GetParentType();
						Agent parent = pAgent;
						if (parentType2 == ParentType.PT_INSTANCE)
						{
							parent = Agent.GetInstance(action.m_resultFunctor.GetInstanceNameString(), agent.GetContextId());
						}
						result = (EBTStatus)((int)action.m_resultFunctor.run(parent, pAgent, obj));
					}
					else
					{
						result = (EBTStatus)((int)obj);
					}
					Action.ActionTask.ClearNodeId();
				}
				else
				{
					result = action.update_impl(pAgent, childStatus);
				}
				return result;
			}
		}

		protected CMethodBase m_method;

		private EBTStatus m_resultOption;

		private CMethodBase m_resultFunctor;

		private EBTStatus m_resultPreconditionFail;

		public Action()
		{
			this.m_resultOption = EBTStatus.BT_INVALID;
			this.m_resultPreconditionFail = EBTStatus.BT_FAILURE;
		}

		~Action()
		{
			this.m_method = null;
			this.m_resultFunctor = null;
		}

		private static int ParseMethodNames(string fullName, ref string agentIntanceName, ref string agentClassName, ref string methodName)
		{
			int num = fullName.IndexOf('.');
			agentIntanceName = fullName.Substring(0, num);
			int num2 = num + 1;
			int num3 = fullName.IndexOf('(', num2);
			int num4 = fullName.LastIndexOf(':', num3);
			num4++;
			int num5 = num3 - num4;
			methodName = fullName.Substring(num4, num5);
			int num6 = num4 - 2 - num2;
			agentClassName = fullName.Substring(num2, num6).Replace("::", ".");
			return num3;
		}

		private static List<string> ParseForParams(string tsrc)
		{
			int length = tsrc.get_Length();
			int num = 0;
			int i = 0;
			int num2 = 0;
			List<string> list = new List<string>();
			while (i < length)
			{
				if (tsrc.get_Chars(i) == '"')
				{
					num2++;
					if ((num2 & 1) == 0)
					{
						num2 -= 2;
					}
				}
				else if (num2 == 0 && tsrc.get_Chars(i) == ',')
				{
					int num3 = i - num;
					string text = tsrc.Substring(num, num3);
					list.Add(text);
					num = i + 1;
				}
				i++;
			}
			int num4 = i - num;
			if (num4 > 0)
			{
				string text2 = tsrc.Substring(num, num4);
				list.Add(text2);
			}
			return list;
		}

		public static CMethodBase LoadMethod(string value_)
		{
			string agentInstanceName = null;
			string str = null;
			string str2 = null;
			int num = Action.ParseMethodNames(value_, ref agentInstanceName, ref str, ref str2);
			CStringID agentClassId = new CStringID(str);
			CStringID methodClassId = new CStringID(str2);
			CMethodBase cMethodBase = Agent.CreateMethod(agentClassId, methodClassId);
			if (cMethodBase != null)
			{
				if (Agent.IsNameRegistered(agentInstanceName))
				{
					cMethodBase.SetInstanceNameString(agentInstanceName, ParentType.PT_INSTANCE);
				}
				string text = value_.Substring(num);
				int length = text.get_Length();
				string tsrc = text.Substring(1, length - 2);
				List<string> list = Action.ParseForParams(tsrc);
				if (list != null)
				{
					cMethodBase.Load(null, list);
				}
			}
			return cMethodBase;
		}

		protected override void load(int version, string agentType, List<property_t> properties)
		{
			base.load(version, agentType, properties);
			using (List<property_t>.Enumerator enumerator = properties.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					property_t current = enumerator.get_Current();
					if (current.name == "Method")
					{
						if (!string.IsNullOrEmpty(current.value))
						{
							this.m_method = Action.LoadMethod(current.value);
						}
					}
					else if (current.name == "ResultOption")
					{
						if (current.value == "BT_INVALID")
						{
							this.m_resultOption = EBTStatus.BT_INVALID;
						}
						else if (current.value == "BT_FAILURE")
						{
							this.m_resultOption = EBTStatus.BT_FAILURE;
						}
						else if (current.value == "BT_RUNNING")
						{
							this.m_resultOption = EBTStatus.BT_RUNNING;
						}
						else
						{
							this.m_resultOption = EBTStatus.BT_SUCCESS;
						}
					}
					else if (current.name == "ResultFunctor")
					{
						if (current.value != null && current.value.get_Chars(0) != '\0')
						{
							this.m_resultFunctor = Action.LoadMethod(current.value);
						}
					}
					else if (current.name == "PreconditionFailResult")
					{
						if (current.value == "BT_FAILURE")
						{
							this.m_resultPreconditionFail = EBTStatus.BT_FAILURE;
						}
						else if (current.value == "BT_BT_SUCCESS")
						{
							this.m_resultPreconditionFail = EBTStatus.BT_SUCCESS;
						}
					}
				}
			}
		}

		public override bool IsValid(Agent pAgent, BehaviorTask pTask)
		{
			return pTask.GetNode() is Action && base.IsValid(pAgent, pTask);
		}

		protected override BehaviorTask createTask()
		{
			return new Action.ActionTask();
		}
	}
}
