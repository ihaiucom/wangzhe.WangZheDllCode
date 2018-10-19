using Mono.Xml;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading;

namespace behaviac
{
	public static class Workspace
	{
		[Flags]
		public enum EFileFormat
		{
			EFF_xml = 1,
			EFF_cs = 4,
			EFF_default = 5
		}

		private class BreakpointInfo_t
		{
			public ushort hit_config;

			public EActionResult action_result;

			public BreakpointInfo_t()
			{
				this.hit_config = 0;
				this.action_result = EActionResult.EAR_all;
			}
		}

		private class BTItem_t
		{
			public ListView<BehaviorTreeTask> bts = new ListView<BehaviorTreeTask>();

			public ListView<Agent> agents = new ListView<Agent>();
		}

		private class TypeInfo_t
		{
			public Type type;

			public bool bIsInherited;
		}

		private delegate void BehaviorNodeLoadedHandler_t(string nodeType, List<property_t> properties);

		public delegate void DRespondToBreakHandler(string msg, string title);

		private static Workspace.EFileFormat fileFormat_ = Workspace.EFileFormat.EFF_xml;

		private static string ms_workspaceExportPath;

		private static int ms_deltaFrames;

		private static string m_workspaceFileAbs = string.Empty;

		private static uint m_frame;

		private static DictionaryView<string, BehaviorTree> ms_behaviortrees;

		private static DictionaryView<string, MethodInfo> ms_btCreators;

		private static DictionaryView<string, Workspace.BTItem_t> ms_allBehaviorTreeTasks = new DictionaryView<string, Workspace.BTItem_t>();

		private static DictionaryView<string, Type> ms_behaviorNodeTypes;

		private static List<Workspace.TypeInfo_t> ms_agentTypes = new List<Workspace.TypeInfo_t>();

        private static event Workspace.BehaviorNodeLoadedHandler_t OnNodeLoaded;

        public static event Workspace.DRespondToBreakHandler RespondToBreakHandler;

		public static Workspace.EFileFormat FileFormat
		{
			get
			{
				return Workspace.fileFormat_;
			}
			set
			{
				Workspace.fileFormat_ = value;
			}
		}

		public static string WorkspaceExportPath
		{
			get
			{
				return Workspace.ms_workspaceExportPath;
			}
		}

		private static DictionaryView<string, BehaviorTree> BehaviorTrees
		{
			get
			{
				if (Workspace.ms_behaviortrees == null)
				{
					Workspace.ms_behaviortrees = new DictionaryView<string, BehaviorTree>();
				}
				return Workspace.ms_behaviortrees;
			}
		}

		private static DictionaryView<string, MethodInfo> BTCreators
		{
			get
			{
				if (Workspace.ms_btCreators == null)
				{
					Workspace.ms_btCreators = new DictionaryView<string, MethodInfo>();
				}
				return Workspace.ms_btCreators;
			}
		}

		public static void HandleNodeLoaded(string nodeType, List<property_t> properties)
		{
			if (Workspace.OnNodeLoaded != null)
			{
				Workspace.OnNodeLoaded(nodeType, properties);
			}
		}

		public static void RespondToBreak(string msg, string title)
		{
			if (Workspace.RespondToBreakHandler != null)
			{
				Workspace.RespondToBreakHandler(msg, title);
				return;
			}
			Workspace.WaitforContinue();
			Thread.Sleep(500);
		}

		public static bool SetWorkspaceSettings(string workspaceExportPath, Workspace.EFileFormat format)
		{
			bool flag = string.IsNullOrEmpty(Workspace.ms_workspaceExportPath);
			Workspace.ms_workspaceExportPath = workspaceExportPath;
			if (!Workspace.ms_workspaceExportPath.EndsWith("/"))
			{
				Workspace.ms_workspaceExportPath += '/';
			}
			Workspace.fileFormat_ = format;
			if (string.IsNullOrEmpty(Workspace.ms_workspaceExportPath))
			{
				return false;
			}
			Workspace.LoadWorkspaceAbsolutePath();
			Workspace.ms_deltaFrames = 1;
			if (flag)
			{
				Details.RegisterCompareValue();
				Details.RegisterComputeValue();
				Workspace.RegisterBehaviorNode();
				Workspace.RegisterMetas();
			}
			return true;
		}

		public static bool SetWorkspaceSettings(string workspaceExportPath)
		{
			return Workspace.SetWorkspaceSettings(workspaceExportPath, Workspace.EFileFormat.EFF_xml);
		}

		public static void SetDeltaFrames(int deltaFrames)
		{
			Workspace.ms_deltaFrames = deltaFrames;
		}

		public static int GetDeltaFrames()
		{
			return Workspace.ms_deltaFrames;
		}

		private static bool LoadWorkspaceSetting(string file, string ext, ref string workspaceFile)
		{
			try
			{
				byte[] array = Workspace.ReadFileToBuffer(file, ext);
				if (array != null)
				{
					string @string = Encoding.UTF8.GetString(array);
					SecurityParser securityParser = new SecurityParser();
					securityParser.LoadXml(@string);
					SecurityElement securityElement = securityParser.ToXml();
					if (securityElement.Tag == "workspace")
					{
						workspaceFile = securityElement.Attribute("path");
						return true;
					}
				}
			}
			catch (Exception ex)
			{
				string text = string.Format("Load Workspace {0} Error : {1}", file, ex.Message);
			}
			return false;
		}

		public static string GetWorkspaceAbsolutePath()
		{
			return Workspace.m_workspaceFileAbs;
		}

		public static void LoadWorkspaceAbsolutePath()
		{
		}

		public static void SetAutoHotReload(bool enable)
		{
		}

		public static bool GetAutoHotReload()
		{
			return false;
		}

		public static void HotReload()
		{
		}

		public static void LogFrames()
		{
			LogManager.Log("[frame]{0}\n", new object[]
			{
				Workspace.m_frame++
			});
		}

		public static void WaitforContinue()
		{
		}

		public static bool HandleRequests()
		{
			return false;
		}

		public static void RecordBTAgentMapping(string relativePath, Agent agent)
		{
			if (Workspace.ms_allBehaviorTreeTasks == null)
			{
				Workspace.ms_allBehaviorTreeTasks = new DictionaryView<string, Workspace.BTItem_t>();
			}
			if (!Workspace.ms_allBehaviorTreeTasks.ContainsKey(relativePath))
			{
				Workspace.ms_allBehaviorTreeTasks[relativePath] = new Workspace.BTItem_t();
			}
			Workspace.BTItem_t bTItem_t = Workspace.ms_allBehaviorTreeTasks[relativePath];
			if (bTItem_t.agents.IndexOf(agent) == -1)
			{
				bTItem_t.agents.Add(agent);
			}
		}

		public static void UnLoad(string relativePath)
		{
			if (Workspace.BehaviorTrees.ContainsKey(relativePath))
			{
				Workspace.BehaviorTrees.Remove(relativePath);
			}
		}

		public static void UnLoadAll()
		{
			Workspace.ms_allBehaviorTreeTasks.Clear();
			Workspace.BehaviorTrees.Clear();
			Workspace.BTCreators.Clear();
		}

		private static byte[] ReadFileToBuffer(string file, string ext)
		{
			return FileManager.Instance.FileOpen(file, ext);
		}

		private static void PopFileFromBuffer(string file, string ext, byte[] pBuffer)
		{
			FileManager.Instance.FileClose(file, ext, pBuffer);
		}

		public static bool Load(string relativePath, bool bForce)
		{
			BehaviorTree behaviorTree = null;
			if (Workspace.BehaviorTrees.ContainsKey(relativePath))
			{
				if (!bForce)
				{
					return true;
				}
				behaviorTree = Workspace.BehaviorTrees[relativePath];
			}
			string text = Workspace.WorkspaceExportPath;
			text += relativePath;
			string ext = string.Empty;
			Workspace.EFileFormat eFileFormat = Workspace.FileFormat;
			switch (eFileFormat)
			{
			case Workspace.EFileFormat.EFF_xml:
				ext = ".xml";
				break;
			case Workspace.EFileFormat.EFF_default:
				ext = ".xml";
				if (FileManager.Instance.FileExist(text, ext))
				{
					eFileFormat = Workspace.EFileFormat.EFF_xml;
				}
				else
				{
					ext = ".bson";
					if (FileManager.Instance.FileExist(text, ext))
					{
						throw new NotImplementedException("bson support has been removed!!!");
					}
					eFileFormat = Workspace.EFileFormat.EFF_cs;
				}
				break;
			}
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			if (behaviorTree == null)
			{
				flag3 = true;
				behaviorTree = new BehaviorTree();
				Workspace.BehaviorTrees[relativePath] = behaviorTree;
			}
			if (eFileFormat == Workspace.EFileFormat.EFF_xml)
			{
				byte[] array = Workspace.ReadFileToBuffer(text, ext);
				if (array != null)
				{
					if (!flag3)
					{
						flag2 = true;
						behaviorTree.Clear();
					}
					if (eFileFormat == Workspace.EFileFormat.EFF_xml)
					{
						flag = behaviorTree.load_xml(array);
					}
					Workspace.PopFileFromBuffer(text, ext, array);
				}
			}
			else if (eFileFormat == Workspace.EFileFormat.EFF_cs)
			{
				if (!flag3)
				{
					flag2 = true;
					behaviorTree.Clear();
				}
				try
				{
					MethodInfo methodInfo = null;
					if (Workspace.BTCreators.ContainsKey(relativePath))
					{
						methodInfo = Workspace.BTCreators[relativePath];
					}
					else
					{
						string typeName = "behaviac.bt_" + relativePath.Replace("/", "_");
						Type type = Utils.GetType(typeName);
						if (type != null)
						{
							methodInfo = type.GetMethod("build_behavior_tree", BindingFlags.Static | BindingFlags.Public);
							if (methodInfo != null)
							{
								Workspace.BTCreators[relativePath] = methodInfo;
							}
						}
					}
					if (methodInfo != null)
					{
						object[] parameters = new object[]
						{
							behaviorTree
						};
						flag = (bool)methodInfo.Invoke(null, parameters);
					}
				}
				catch (Exception ex)
				{
					string text2 = string.Format("The behavior {0} failed to be loaded : {1}", relativePath, ex.Message);
				}
			}
			if (flag)
			{
				if (!flag3)
				{
				}
			}
			else if (flag3)
			{
				bool flag4 = Workspace.BehaviorTrees.Remove(relativePath);
			}
			else if (flag2)
			{
				Workspace.BehaviorTrees.Remove(relativePath);
			}
			return flag;
		}

		public static bool Load(string relativePath)
		{
			return Workspace.Load(relativePath, false);
		}

		public static bool IsValidPath(string relativePath)
		{
			return (relativePath[0] != '.' || (relativePath[1] != '/' && relativePath[1] != '\\')) && relativePath[0] != '/' && relativePath[0] != '\\';
		}

		public static BehaviorTreeTask CreateBehaviorTreeTask(string relativePath)
		{
			BehaviorTree behaviorTree = null;
			if (Workspace.BehaviorTrees.ContainsKey(relativePath))
			{
				behaviorTree = Workspace.BehaviorTrees[relativePath];
			}
			else
			{
				bool flag = Workspace.Load(relativePath);
				if (flag)
				{
					behaviorTree = Workspace.BehaviorTrees[relativePath];
				}
			}
			if (behaviorTree != null)
			{
				BehaviorTask behaviorTask = behaviorTree.CreateAndInitTask();
				BehaviorTreeTask behaviorTreeTask = behaviorTask as BehaviorTreeTask;
				if (!Workspace.ms_allBehaviorTreeTasks.ContainsKey(relativePath))
				{
					Workspace.ms_allBehaviorTreeTasks[relativePath] = new Workspace.BTItem_t();
				}
				Workspace.BTItem_t bTItem_t = Workspace.ms_allBehaviorTreeTasks[relativePath];
				if (!bTItem_t.bts.Contains(behaviorTreeTask))
				{
					bTItem_t.bts.Add(behaviorTreeTask);
				}
				return behaviorTreeTask;
			}
			return null;
		}

		public static void DestroyBehaviorTreeTask(BehaviorTreeTask behaviorTreeTask, Agent agent)
		{
			if (behaviorTreeTask != null)
			{
				if (Workspace.ms_allBehaviorTreeTasks.ContainsKey(behaviorTreeTask.GetName()))
				{
					Workspace.BTItem_t bTItem_t = Workspace.ms_allBehaviorTreeTasks[behaviorTreeTask.GetName()];
					bTItem_t.bts.Remove(behaviorTreeTask);
					if (agent != null)
					{
						bTItem_t.agents.Remove(agent);
					}
				}
				BehaviorTask.DestroyTask(behaviorTreeTask);
			}
		}

		public static DictionaryView<string, BehaviorTree> GetBehaviorTrees()
		{
			return Workspace.ms_behaviortrees;
		}

		public static void RegisterBehaviorNode()
		{
			Assembly callingAssembly = Assembly.GetCallingAssembly();
			Workspace.RegisterBehaviorNode(callingAssembly);
		}

		public static void RegisterBehaviorNode(Assembly a)
		{
			if (Workspace.ms_behaviorNodeTypes == null)
			{
				Workspace.ms_behaviorNodeTypes = new DictionaryView<string, Type>();
			}
			Type[] types = a.GetTypes();
			Type[] array = types;
			for (int i = 0; i < array.Length; i++)
			{
				Type type = array[i];
				if (type.IsSubclassOf(typeof(BehaviorNode)) && !type.IsAbstract)
				{
					Workspace.ms_behaviorNodeTypes[type.Name] = type;
				}
			}
		}

		public static BehaviorNode CreateBehaviorNode(string className)
		{
			if (Workspace.ms_behaviorNodeTypes.ContainsKey(className))
			{
				Type type = Workspace.ms_behaviorNodeTypes[className];
				object obj = Activator.CreateInstance(type);
				return obj as BehaviorNode;
			}
			return null;
		}

		public static bool ExportMetas(string filePath, bool onlyExportPublicMembers)
		{
			return false;
		}

		public static bool ExportMetas(string filePath)
		{
			return Workspace.ExportMetas(filePath, false);
		}

		private static bool IsRegisterd(Type type)
		{
			int num = Workspace.ms_agentTypes.FindIndex((Workspace.TypeInfo_t t) => t.type == type);
			return num != -1;
		}

		private static void RegisterMetas()
		{
			Assembly callingAssembly = Assembly.GetCallingAssembly();
			Workspace.RegisterMetas(callingAssembly);
		}

		private static void RegisterMetas(Assembly a)
		{
			ListView<Type> listView = new ListView<Type>();
			Type[] types = a.GetTypes();
			Type[] array = types;
			for (int i = 0; i < array.Length; i++)
			{
				Type type3 = array[i];
				if ((type3.IsSubclassOf(typeof(Agent)) || Utils.IsStaticType(type3)) && !Workspace.IsRegisterd(type3))
				{
					Attribute[] array2 = (Attribute[])type3.GetCustomAttributes(typeof(TypeMetaInfoAttribute), false);
					if (array2.Length > 0)
					{
						Workspace.TypeInfo_t typeInfo_t = new Workspace.TypeInfo_t();
						typeInfo_t.type = type3;
						Workspace.ms_agentTypes.Add(typeInfo_t);
						if (type3.BaseType != null && (type3.BaseType == typeof(Agent) || type3.BaseType.IsSubclassOf(typeof(Agent))))
						{
							listView.Add(type3.BaseType);
						}
						if (Utils.IsStaticType(type3))
						{
							TypeMetaInfoAttribute typeMetaInfoAttribute = array2[0] as TypeMetaInfoAttribute;
							Agent.RegisterStaticClass(type3, typeMetaInfoAttribute.DisplayName, typeMetaInfoAttribute.Description);
						}
					}
				}
			}
			foreach (Type type in listView)
			{
				if (!Workspace.IsRegisterd(type) && Workspace.ms_agentTypes.Find((Workspace.TypeInfo_t t) => t.type == type) == null)
				{
					Workspace.TypeInfo_t typeInfo_t2 = new Workspace.TypeInfo_t();
					typeInfo_t2.type = type;
					typeInfo_t2.bIsInherited = true;
					Workspace.ms_agentTypes.Add(typeInfo_t2);
				}
			}
			Workspace.ms_agentTypes.Sort(delegate(Workspace.TypeInfo_t x, Workspace.TypeInfo_t y)
			{
				if (x.bIsInherited && !y.bIsInherited)
				{
					return -1;
				}
				if (!x.bIsInherited && y.bIsInherited)
				{
					return 1;
				}
				if (x.type.IsSubclassOf(y.type))
				{
					return 1;
				}
				if (y.type.IsSubclassOf(x.type))
				{
					return -1;
				}
				return x.type.FullName.CompareTo(y.type.FullName);
			});
			foreach (Workspace.TypeInfo_t current in Workspace.ms_agentTypes)
			{
				Type type2 = current.type;
				Workspace.RegisterType(type2, true);
			}
		}

		private static void RegisterType(Type type, bool bIsAgentType)
		{
			Attribute[] array = (Attribute[])type.GetCustomAttributes(typeof(TypeMetaInfoAttribute), false);
			if (!bIsAgentType || array.Length > 0)
			{
				TypeMetaInfoAttribute typeMetaInfoAttribute = (array.Length <= 0) ? null : ((TypeMetaInfoAttribute)array[0]);
				Agent.CTagObjectDescriptor descriptorByName = Agent.GetDescriptorByName(type.FullName);
				if (type.BaseType == typeof(Agent) || type.BaseType.IsSubclassOf(typeof(Agent)))
				{
					Agent.CTagObjectDescriptor descriptorByName2 = Agent.GetDescriptorByName(type.BaseType.FullName);
					descriptorByName.m_parent = descriptorByName2;
				}
				descriptorByName.type = type;
				descriptorByName.displayName = ((typeMetaInfoAttribute != null && !string.IsNullOrEmpty(typeMetaInfoAttribute.DisplayName)) ? typeMetaInfoAttribute.DisplayName : type.FullName);
				descriptorByName.desc = ((typeMetaInfoAttribute != null && !string.IsNullOrEmpty(typeMetaInfoAttribute.Description)) ? typeMetaInfoAttribute.Description : descriptorByName.displayName);
				if (Utils.IsEnumType(type))
				{
					return;
				}
				BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
				FieldInfo[] fields = type.GetFields(bindingAttr);
				FieldInfo[] array2 = fields;
				for (int i = 0; i < array2.Length; i++)
				{
					FieldInfo fieldInfo = array2[i];
					bool flag = false;
					MemberMetaInfoAttribute a = null;
					if (bIsAgentType)
					{
						Attribute[] array3 = (Attribute[])fieldInfo.GetCustomAttributes(typeof(MemberMetaInfoAttribute), false);
						if (array3.Length > 0)
						{
							a = (MemberMetaInfoAttribute)array3[0];
							flag = true;
						}
					}
					else
					{
						flag = true;
					}
					if (flag)
					{
						CMemberBase cMemberBase = new CMemberBase(fieldInfo, a);
						for (int j = 0; j < descriptorByName.ms_members.Count; j++)
						{
							if (cMemberBase.GetId() == descriptorByName.ms_members[j].GetId())
							{
								CMemberBase cMemberBase2 = descriptorByName.ms_members[j];
								break;
							}
						}
						descriptorByName.ms_members.Add(cMemberBase);
						if ((Utils.IsCustomClassType(fieldInfo.FieldType) || Utils.IsEnumType(fieldInfo.FieldType)) && !Agent.IsTypeRegisterd(fieldInfo.FieldType.FullName))
						{
							Workspace.RegisterType(fieldInfo.FieldType, false);
						}
					}
				}
				if (bIsAgentType)
				{
					MethodInfo[] methods = type.GetMethods(bindingAttr);
					MethodInfo[] array4 = methods;
					for (int k = 0; k < array4.Length; k++)
					{
						MethodInfo methodInfo = array4[k];
						Attribute[] array5 = (Attribute[])methodInfo.GetCustomAttributes(typeof(MethodMetaInfoAttribute), false);
						if (array5.Length > 0)
						{
							MethodMetaInfoAttribute a2 = (MethodMetaInfoAttribute)array5[0];
							CMethodBase item = new CMethodBase(methodInfo, a2, null);
							descriptorByName.ms_methods.Add(item);
							ParameterInfo[] parameters = methodInfo.GetParameters();
							ParameterInfo[] array6 = parameters;
							for (int l = 0; l < array6.Length; l++)
							{
								ParameterInfo parameterInfo = array6[l];
								if ((Utils.IsCustomClassType(parameterInfo.ParameterType) || Utils.IsEnumType(parameterInfo.ParameterType)) && !Agent.IsTypeRegisterd(parameterInfo.ParameterType.FullName))
								{
									Workspace.RegisterType(parameterInfo.ParameterType, false);
								}
							}
							if ((Utils.IsCustomClassType(methodInfo.ReturnType) || Utils.IsEnumType(methodInfo.ReturnType)) && !Agent.IsTypeRegisterd(methodInfo.ReturnType.FullName))
							{
								Workspace.RegisterType(methodInfo.ReturnType, false);
							}
						}
					}
					Type[] nestedTypes = type.GetNestedTypes(bindingAttr);
					Type[] array7 = nestedTypes;
					for (int m = 0; m < array7.Length; m++)
					{
						Type type2 = array7[m];
						Attribute[] array8 = (Attribute[])type2.GetCustomAttributes(typeof(EventMetaInfoAttribute), false);
						if (array8.Length > 0)
						{
							EventMetaInfoAttribute a3 = (EventMetaInfoAttribute)array8[0];
							MethodInfo method = type2.GetMethod("Invoke");
							CNamedEvent item2 = new CNamedEvent(method, a3, type2.Name);
							descriptorByName.ms_methods.Add(item2);
						}
					}
				}
			}
		}
	}
}
