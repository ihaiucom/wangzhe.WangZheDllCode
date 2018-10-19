using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace com.tencent.pandora
{
	public static class ToLuaExport
	{
		public static string className;

		public static Type type;

		public static string baseClassName;

		public static bool isStaticClass;

		private static HashSet<string> usingList;

		private static MetaOp op;

		private static StringBuilder sb;

		private static MethodInfo[] methods;

		private static Dictionary<string, int> nameCounter;

		private static FieldInfo[] fields;

		private static PropertyInfo[] props;

		private static List<PropertyInfo> propList;

		private static BindingFlags binding;

		private static ObjAmbig ambig;

		public static string wrapClassName;

		public static string libClassName;

		public static string extendName;

		public static Type extendType;

		public static HashSet<Type> eventSet;

		public static List<string> memberFilter;

		private static Dictionary<Type, int> typeSize;

		static ToLuaExport()
		{
			ToLuaExport.className = string.Empty;
			ToLuaExport.type = null;
			ToLuaExport.baseClassName = null;
			ToLuaExport.isStaticClass = true;
			ToLuaExport.usingList = new HashSet<string>();
			ToLuaExport.op = MetaOp.None;
			ToLuaExport.sb = null;
			ToLuaExport.methods = null;
			ToLuaExport.nameCounter = null;
			ToLuaExport.fields = null;
			ToLuaExport.props = null;
			ToLuaExport.propList = new List<PropertyInfo>();
			ToLuaExport.binding = (BindingFlags.IgnoreCase | BindingFlags.Static | BindingFlags.Public);
			ToLuaExport.ambig = ObjAmbig.NetObj;
			ToLuaExport.wrapClassName = string.Empty;
			ToLuaExport.libClassName = string.Empty;
			ToLuaExport.extendName = string.Empty;
			ToLuaExport.extendType = null;
			ToLuaExport.eventSet = new HashSet<Type>();
			ToLuaExport.memberFilter = new List<string>
			{
				"AnimationClip.averageDuration",
				"AnimationClip.averageAngularSpeed",
				"AnimationClip.averageSpeed",
				"AnimationClip.apparentSpeed",
				"AnimationClip.isLooping",
				"AnimationClip.isAnimatorMotion",
				"AnimationClip.isHumanMotion",
				"AnimatorOverrideController.PerformOverrideClipListCleanup",
				"Caching.SetNoBackupFlag",
				"Caching.ResetNoBackupFlag",
				"Light.areaSize",
				"Security.GetChainOfTrustValue",
				"Texture2D.alphaIsTransparency",
				"WWW.movie",
				"WebCamTexture.MarkNonReadable",
				"WebCamTexture.isReadable",
				"Graphic.OnRebuildRequested",
				"Text.OnRebuildRequested",
				"UIInput.ProcessEvent",
				"UIWidget.showHandlesWithMoveTool",
				"UIWidget.showHandles",
				"Application.ExternalEval",
				"Resources.LoadAssetAtPath",
				"Input.IsJoystickPreconfigured",
				"String.Chars"
			};
			ToLuaExport.typeSize = new Dictionary<Type, int>
			{
				{
					typeof(bool),
					1
				},
				{
					typeof(char),
					2
				},
				{
					typeof(byte),
					3
				},
				{
					typeof(sbyte),
					4
				},
				{
					typeof(ushort),
					5
				},
				{
					typeof(short),
					6
				},
				{
					typeof(uint),
					7
				},
				{
					typeof(int),
					8
				},
				{
					typeof(float),
					9
				},
				{
					typeof(ulong),
					10
				},
				{
					typeof(long),
					11
				},
				{
					typeof(double),
					12
				}
			};
		}

		public static bool IsMemberFilter(MemberInfo mi)
		{
			return ToLuaExport.memberFilter.Contains(ToLuaExport.type.Name + "." + mi.Name);
		}

		public static void Clear()
		{
			ToLuaExport.className = null;
			ToLuaExport.type = null;
			ToLuaExport.isStaticClass = false;
			ToLuaExport.baseClassName = null;
			ToLuaExport.usingList.Clear();
			ToLuaExport.op = MetaOp.None;
			ToLuaExport.sb = new StringBuilder();
			ToLuaExport.methods = null;
			ToLuaExport.fields = null;
			ToLuaExport.props = null;
			ToLuaExport.propList.Clear();
			ToLuaExport.ambig = ObjAmbig.NetObj;
			ToLuaExport.wrapClassName = string.Empty;
			ToLuaExport.libClassName = string.Empty;
		}

		private static MetaOp GetOp(string name)
		{
			if (name == "op_Addition")
			{
				return MetaOp.Add;
			}
			if (name == "op_Subtraction")
			{
				return MetaOp.Sub;
			}
			if (name == "op_Equality")
			{
				return MetaOp.Eq;
			}
			if (name == "op_Multiply")
			{
				return MetaOp.Mul;
			}
			if (name == "op_Division")
			{
				return MetaOp.Div;
			}
			if (name == "op_UnaryNegation")
			{
				return MetaOp.Neg;
			}
			return MetaOp.None;
		}

		private static void GenBaseOpFunction(List<MethodInfo> list)
		{
			for (Type baseType = ToLuaExport.type.BaseType; baseType != null; baseType = baseType.BaseType)
			{
				MethodInfo[] array = baseType.GetMethods(BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
				for (int i = 0; i < array.Length; i++)
				{
					MetaOp metaOp = ToLuaExport.GetOp(array[i].Name);
					if (metaOp != MetaOp.None && (ToLuaExport.op & metaOp) == MetaOp.None)
					{
						list.Add(array[i]);
						ToLuaExport.op |= metaOp;
					}
				}
			}
		}

		
		private sealed class _Generate_c__AnonStorey97
		{
			internal PropertyInfo[] ps;
		}

		public static void Generate(params string[] param)
		{
			ToLuaExport._Generate_c__AnonStorey97 _Generate_c__AnonStorey = new ToLuaExport._Generate_c__AnonStorey97();
			Debug.Log("Begin Generate lua Wrap for class " + ToLuaExport.className + "\r\n");
			ToLuaExport.sb = new StringBuilder();
			ToLuaExport.usingList.Add("System");
			ToLuaExport.GetTypeStr(ToLuaExport.type);
			if (ToLuaExport.wrapClassName == string.Empty)
			{
				ToLuaExport.wrapClassName = ToLuaExport.className;
			}
			if (ToLuaExport.libClassName == string.Empty)
			{
				ToLuaExport.libClassName = ToLuaExport.className;
			}
			if (ToLuaExport.type.IsEnum)
			{
				ToLuaExport.GenEnum();
				ToLuaExport.GenEnumTranslator();
				ToLuaExport.sb.AppendLine("}\r\n");
				ToLuaExport.SaveFile(Application.dataPath + "/Scripts/Pandora/uLua/Source/LuaWrap/" + ToLuaExport.wrapClassName + "Wrap.cs");
				return;
			}
			ToLuaExport.nameCounter = new Dictionary<string, int>();
			List<MethodInfo> list = new List<MethodInfo>();
			if (ToLuaExport.baseClassName != null)
			{
				ToLuaExport.binding |= BindingFlags.DeclaredOnly;
			}
			else if (ToLuaExport.baseClassName == null && ToLuaExport.isStaticClass)
			{
				ToLuaExport.binding |= BindingFlags.DeclaredOnly;
			}
			if (ToLuaExport.type.IsInterface)
			{
				list.AddRange(ToLuaExport.type.GetMethods());
			}
			else
			{
				list.AddRange(ToLuaExport.type.GetMethods(BindingFlags.Instance | ToLuaExport.binding));
				for (int j = list.Count - 1; j >= 0; j--)
				{
					if (list[j].Name.Contains("op_") || list[j].Name.Contains("add_") || list[j].Name.Contains("remove_"))
					{
						if (!ToLuaExport.IsNeedOp(list[j].Name))
						{
							list.RemoveAt(j);
						}
					}
					else if (ToLuaExport.IsObsolete(list[j]))
					{
						list.RemoveAt(j);
					}
				}
			}
			_Generate_c__AnonStorey.ps = ToLuaExport.type.GetProperties();
			int i;
			for (i = 0; i < _Generate_c__AnonStorey.ps.Length; i++)
			{
				int num = list.FindIndex((MethodInfo m) => m.Name == "get_" + _Generate_c__AnonStorey.ps[i].Name);
				if (num >= 0 && list[num].Name != "get_Item")
				{
					list.RemoveAt(num);
				}
				num = list.FindIndex((MethodInfo m) => m.Name == "set_" + _Generate_c__AnonStorey.ps[i].Name);
				if (num >= 0 && list[num].Name != "set_Item")
				{
					list.RemoveAt(num);
				}
			}
			ToLuaExport.ProcessExtends(list);
			ToLuaExport.GenBaseOpFunction(list);
			ToLuaExport.methods = list.ToArray();
			ToLuaExport.sb.AppendFormat("public class {0}Wrap\r\n", ToLuaExport.wrapClassName);
			ToLuaExport.sb.AppendLine("{");
			ToLuaExport.GenRegFunc();
			ToLuaExport.GenConstruct();
			ToLuaExport.GenGetType();
			ToLuaExport.GenIndexFunc();
			ToLuaExport.GenNewIndexFunc();
			ToLuaExport.GenToStringFunc();
			ToLuaExport.GenFunction();
			ToLuaExport.sb.AppendLine("}\r\n");
			string text = Application.dataPath + "/Scripts/Pandora/uLua/Source/LuaWrap/";
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			ToLuaExport.SaveFile(text + ToLuaExport.wrapClassName + "Wrap.cs");
		}

		private static void SaveFile(string file)
		{
			using (StreamWriter streamWriter = new StreamWriter(file, false, Encoding.UTF8))
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (string current in ToLuaExport.usingList)
				{
					stringBuilder.AppendFormat("using {0};\r\n", current);
				}
				stringBuilder.AppendLine("using com.tencent.pandora;");
				if (ToLuaExport.ambig == ObjAmbig.All)
				{
					stringBuilder.AppendLine("using Object = UnityEngine.Object;");
				}
				stringBuilder.AppendLine();
				streamWriter.Write(stringBuilder.ToString());
				streamWriter.Write(ToLuaExport.sb.ToString());
				streamWriter.Flush();
				streamWriter.Close();
			}
		}

		private static void GenLuaFields()
		{
			ToLuaExport.fields = ToLuaExport.type.GetFields(BindingFlags.Instance | BindingFlags.GetField | BindingFlags.SetField | ToLuaExport.binding);
			ToLuaExport.props = ToLuaExport.type.GetProperties(BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty | ToLuaExport.binding);
			ToLuaExport.propList.AddRange(ToLuaExport.type.GetProperties(BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.SetProperty));
			List<FieldInfo> list = new List<FieldInfo>();
			list.AddRange(ToLuaExport.fields);
			for (int i = list.Count - 1; i >= 0; i--)
			{
				if (ToLuaExport.IsObsolete(list[i]))
				{
					list.RemoveAt(i);
				}
			}
			ToLuaExport.fields = list.ToArray();
			List<PropertyInfo> list2 = new List<PropertyInfo>();
			list2.AddRange(ToLuaExport.props);
			for (int j = list2.Count - 1; j >= 0; j--)
			{
				if (list2[j].Name == "Item" || ToLuaExport.IsObsolete(list2[j]))
				{
					list2.RemoveAt(j);
				}
			}
			ToLuaExport.props = list2.ToArray();
			for (int k = ToLuaExport.propList.Count - 1; k >= 0; k--)
			{
				if (ToLuaExport.propList[k].Name == "Item" || ToLuaExport.IsObsolete(ToLuaExport.propList[k]))
				{
					ToLuaExport.propList.RemoveAt(k);
				}
			}
			if (ToLuaExport.fields.Length == 0 && ToLuaExport.props.Length == 0 && ToLuaExport.isStaticClass && ToLuaExport.baseClassName == null)
			{
				return;
			}
			ToLuaExport.sb.AppendLine("\t\tLuaField[] fields = new LuaField[]");
			ToLuaExport.sb.AppendLine("\t\t{");
			for (int l = 0; l < ToLuaExport.fields.Length; l++)
			{
				if (ToLuaExport.fields[l].IsLiteral || ToLuaExport.fields[l].IsPrivate || ToLuaExport.fields[l].IsInitOnly)
				{
					ToLuaExport.sb.AppendFormat("\t\t\tnew LuaField(\"{0}\", get_{0}, null),\r\n", ToLuaExport.fields[l].Name);
				}
				else
				{
					ToLuaExport.sb.AppendFormat("\t\t\tnew LuaField(\"{0}\", get_{0}, set_{0}),\r\n", ToLuaExport.fields[l].Name);
				}
			}
			for (int m = 0; m < ToLuaExport.props.Length; m++)
			{
				if (ToLuaExport.props[m].CanRead && ToLuaExport.props[m].CanWrite && ToLuaExport.props[m].GetSetMethod(true).IsPublic)
				{
					ToLuaExport.sb.AppendFormat("\t\t\tnew LuaField(\"{0}\", get_{0}, set_{0}),\r\n", ToLuaExport.props[m].Name);
				}
				else if (ToLuaExport.props[m].CanRead)
				{
					ToLuaExport.sb.AppendFormat("\t\t\tnew LuaField(\"{0}\", get_{0}, null),\r\n", ToLuaExport.props[m].Name);
				}
				else if (ToLuaExport.props[m].CanWrite)
				{
					ToLuaExport.sb.AppendFormat("\t\t\tnew LuaField(\"{0}\", null, set_{0}),\r\n", ToLuaExport.props[m].Name);
				}
			}
			ToLuaExport.sb.AppendLine("\t\t};\r\n");
		}

		private static void GenLuaMethods()
		{
			ToLuaExport.sb.AppendLine("\t\tLuaMethod[] regs = new LuaMethod[]");
			ToLuaExport.sb.AppendLine("\t\t{");
			for (int i = 0; i < ToLuaExport.methods.Length; i++)
			{
				MethodInfo methodInfo = ToLuaExport.methods[i];
				int num = 1;
				if (!methodInfo.IsGenericMethod)
				{
					if (!ToLuaExport.nameCounter.TryGetValue(methodInfo.Name, out num))
					{
						if (!methodInfo.Name.Contains("op_"))
						{
							ToLuaExport.sb.AppendFormat("\t\t\tnew LuaMethod(\"{0}\", {0}),\r\n", methodInfo.Name);
						}
						ToLuaExport.nameCounter[methodInfo.Name] = 1;
					}
					else
					{
						ToLuaExport.nameCounter[methodInfo.Name] = num + 1;
					}
				}
			}
			ToLuaExport.sb.AppendFormat("\t\t\tnew LuaMethod(\"New\", _Create{0}),\r\n", ToLuaExport.wrapClassName);
			ToLuaExport.sb.AppendLine("\t\t\tnew LuaMethod(\"GetClassType\", GetClassType),");
			int num2 = Array.FindIndex<MethodInfo>(ToLuaExport.methods, (MethodInfo p) => p.Name == "ToString");
			if (num2 >= 0 && !ToLuaExport.isStaticClass)
			{
				ToLuaExport.sb.AppendLine("\t\t\tnew LuaMethod(\"__tostring\", Lua_ToString),");
			}
			ToLuaExport.GenOperatorReg();
			ToLuaExport.sb.AppendLine("\t\t};\r\n");
		}

		private static void GenOperatorReg()
		{
			if ((ToLuaExport.op & MetaOp.Add) != MetaOp.None)
			{
				ToLuaExport.sb.AppendLine("\t\t\tnew LuaMethod(\"__add\", Lua_Add),");
			}
			if ((ToLuaExport.op & MetaOp.Sub) != MetaOp.None)
			{
				ToLuaExport.sb.AppendLine("\t\t\tnew LuaMethod(\"__sub\", Lua_Sub),");
			}
			if ((ToLuaExport.op & MetaOp.Mul) != MetaOp.None)
			{
				ToLuaExport.sb.AppendLine("\t\t\tnew LuaMethod(\"__mul\", Lua_Mul),");
			}
			if ((ToLuaExport.op & MetaOp.Div) != MetaOp.None)
			{
				ToLuaExport.sb.AppendLine("\t\t\tnew LuaMethod(\"__div\", Lua_Div),");
			}
			if ((ToLuaExport.op & MetaOp.Eq) != MetaOp.None)
			{
				ToLuaExport.sb.AppendLine("\t\t\tnew LuaMethod(\"__eq\", Lua_Eq),");
			}
			if ((ToLuaExport.op & MetaOp.Neg) != MetaOp.None)
			{
				ToLuaExport.sb.AppendLine("\t\t\tnew LuaMethod(\"__unm\", Lua_Neg),");
			}
		}

		private static void GenRegFunc()
		{
			ToLuaExport.sb.AppendLine("\tpublic static void Register(IntPtr L)");
			ToLuaExport.sb.AppendLine("\t{");
			ToLuaExport.GenLuaMethods();
			ToLuaExport.GenLuaFields();
			if (ToLuaExport.baseClassName == null)
			{
				if (ToLuaExport.isStaticClass && ToLuaExport.fields.Length == 0 && ToLuaExport.props.Length == 0)
				{
					ToLuaExport.sb.AppendFormat("\t\tLuaScriptMgr.RegisterLib(L, \"{0}\", regs);\r\n", ToLuaExport.libClassName);
				}
				else
				{
					ToLuaExport.sb.AppendFormat("\t\tLuaScriptMgr.RegisterLib(L, \"{0}\", typeof({1}), regs, fields, null);\r\n", ToLuaExport.libClassName, ToLuaExport.className);
				}
			}
			else
			{
				ToLuaExport.sb.AppendFormat("\t\tLuaScriptMgr.RegisterLib(L, \"{0}\", typeof({1}), regs, fields, typeof({2}));\r\n", ToLuaExport.libClassName, ToLuaExport.className, ToLuaExport.baseClassName);
			}
			ToLuaExport.sb.AppendLine("\t}");
		}

		private static bool IsParams(ParameterInfo param)
		{
			return param.GetCustomAttributes(typeof(ParamArrayAttribute), false).Length > 0;
		}

		private static void GenFunction()
		{
			HashSet<string> hashSet = new HashSet<string>();
			for (int i = 0; i < ToLuaExport.methods.Length; i++)
			{
				MethodInfo methodInfo = ToLuaExport.methods[i];
				if (methodInfo.IsGenericMethod)
				{
					Debug.Log("Generic Method " + methodInfo.Name + " cannot be export to lua");
				}
				else
				{
					if (ToLuaExport.nameCounter[methodInfo.Name] > 1)
					{
						if (hashSet.Contains(methodInfo.Name))
						{
							goto IL_241;
						}
						MethodInfo methodInfo2 = ToLuaExport.GenOverrideFunc(methodInfo.Name);
						if (methodInfo2 == null)
						{
							hashSet.Add(methodInfo.Name);
							goto IL_241;
						}
						methodInfo = methodInfo2;
					}
					hashSet.Add(methodInfo.Name);
					ToLuaExport.sb.AppendLine("\r\n\t[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]");
					ToLuaExport.sb.AppendFormat("\tstatic int {0}(IntPtr L)\r\n", ToLuaExport.GetFuncName(methodInfo.Name));
					ToLuaExport.sb.AppendLine("\t{");
					if (ToLuaExport.HasAttribute(methodInfo, typeof(OnlyGCAttribute)))
					{
						ToLuaExport.sb.AppendLine("\t\tLuaScriptMgr.__gc(L);");
						ToLuaExport.sb.AppendLine("\t\treturn 0;");
						ToLuaExport.sb.AppendLine("\t}");
					}
					else if (ToLuaExport.HasAttribute(methodInfo, typeof(UseDefinedAttribute)))
					{
						FieldInfo field = ToLuaExport.extendType.GetField(methodInfo.Name + "Defined");
						string value = field.GetValue(null) as string;
						ToLuaExport.sb.AppendLine(value);
						ToLuaExport.sb.AppendLine("\t}");
					}
					else
					{
						ParameterInfo[] parameters = methodInfo.GetParameters();
						int num = (!methodInfo.IsStatic) ? 2 : 1;
						if (!ToLuaExport.HasOptionalParam(parameters))
						{
							int num2 = parameters.Length + num - 1;
							ToLuaExport.sb.AppendFormat("\t\tLuaScriptMgr.CheckArgsCount(L, {0});\r\n", num2);
						}
						else
						{
							ToLuaExport.sb.AppendLine("\t\tint count = LuaDLL.lua_gettop(L);");
						}
						int num3 = (methodInfo.ReturnType != typeof(void)) ? 1 : 0;
						num3 += ToLuaExport.ProcessParams(methodInfo, 2, false, false, false);
						ToLuaExport.sb.AppendFormat("\t\treturn {0};\r\n", num3);
						ToLuaExport.sb.AppendLine("\t}");
					}
				}
				IL_241:;
			}
		}

		private static void NoConsturct()
		{
			ToLuaExport.sb.AppendLine("\r\n\t[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]");
			ToLuaExport.sb.AppendFormat("\tstatic int _Create{0}(IntPtr L)\r\n", ToLuaExport.wrapClassName);
			ToLuaExport.sb.AppendLine("\t{");
			ToLuaExport.sb.AppendFormat("\t\tLuaDLL.luaL_error(L, \"{0} class does not have a constructor function\");\r\n", ToLuaExport.className);
			ToLuaExport.sb.AppendLine("\t\treturn 0;");
			ToLuaExport.sb.AppendLine("\t}");
		}

		private static string GetPushFunction(Type t)
		{
			if (t.IsEnum)
			{
				return "Push";
			}
			if (t == typeof(bool) || t.IsPrimitive || t == typeof(string) || t == typeof(LuaTable) || t == typeof(LuaCSFunction) || t == typeof(LuaFunction) || typeof(UnityEngine.Object).IsAssignableFrom(t) || t == typeof(Type) || t == typeof(IntPtr) || typeof(Delegate).IsAssignableFrom(t) || t == typeof(LuaStringBuffer) || typeof(TrackedReference).IsAssignableFrom(t) || typeof(IEnumerator).IsAssignableFrom(t))
			{
				return "Push";
			}
			if (t == typeof(Vector3) || t == typeof(Vector2) || t == typeof(Vector4) || t == typeof(Quaternion) || t == typeof(Color) || t == typeof(RaycastHit) || t == typeof(Ray) || t == typeof(Touch) || t == typeof(Bounds))
			{
				return "Push";
			}
			if (t == typeof(object))
			{
				return "PushVarObject";
			}
			if (t.IsValueType)
			{
				return "PushValue";
			}
			if (t.IsArray)
			{
				return "PushArray";
			}
			return "PushObject";
		}

		private static void DefaultConstruct()
		{
			ToLuaExport.sb.AppendLine("\r\n\t[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]");
			ToLuaExport.sb.AppendFormat("\tstatic int _Create{0}(IntPtr L)\r\n", ToLuaExport.wrapClassName);
			ToLuaExport.sb.AppendLine("\t{");
			ToLuaExport.sb.AppendLine("\t\tLuaScriptMgr.CheckArgsCount(L, 0);");
			ToLuaExport.sb.AppendFormat("\t\t{0} obj = new {0}();\r\n", ToLuaExport.className);
			string pushFunction = ToLuaExport.GetPushFunction(ToLuaExport.type);
			ToLuaExport.sb.AppendFormat("\t\tLuaScriptMgr.{0}(L, obj);\r\n", pushFunction);
			ToLuaExport.sb.AppendLine("\t\treturn 1;");
			ToLuaExport.sb.AppendLine("\t}");
		}

		private static string GetCountStr(int count)
		{
			if (count != 0)
			{
				return string.Format("count - {0}", count);
			}
			return "count";
		}

		private static void GenGetType()
		{
			ToLuaExport.sb.AppendFormat("\r\n\tstatic Type classType = typeof({0});\r\n", ToLuaExport.className);
			ToLuaExport.sb.AppendLine("\r\n\t[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]");
			ToLuaExport.sb.AppendFormat("\tstatic int {0}(IntPtr L)\r\n", "GetClassType");
			ToLuaExport.sb.AppendLine("\t{");
			ToLuaExport.sb.AppendLine("\t\tLuaScriptMgr.Push(L, classType);");
			ToLuaExport.sb.AppendLine("\t\treturn 1;");
			ToLuaExport.sb.AppendLine("\t}");
		}

		
		private sealed class _GenConstruct_c__AnonStorey9A
		{
			internal List<ConstructorInfo> list;
		}

		private static void GenConstruct()
		{
			ToLuaExport._GenConstruct_c__AnonStorey9A _GenConstruct_c__AnonStorey9A = new ToLuaExport._GenConstruct_c__AnonStorey9A();
			if (ToLuaExport.isStaticClass || typeof(MonoBehaviour).IsAssignableFrom(ToLuaExport.type))
			{
				ToLuaExport.NoConsturct();
				return;
			}
			ConstructorInfo[] constructors = ToLuaExport.type.GetConstructors(BindingFlags.Instance | ToLuaExport.binding);
			if (ToLuaExport.extendType != null)
			{
				ConstructorInfo[] constructors2 = ToLuaExport.extendType.GetConstructors(BindingFlags.Instance | ToLuaExport.binding);
				if (constructors2 != null && constructors2.Length > 0 && ToLuaExport.HasAttribute(constructors2[0], typeof(UseDefinedAttribute)))
				{
					ToLuaExport.sb.AppendLine("\r\n\t[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]");
					ToLuaExport.sb.AppendFormat("\tstatic int _Create{0}(IntPtr L)\r\n", ToLuaExport.wrapClassName);
					ToLuaExport.sb.AppendLine("\t{");
					if (ToLuaExport.HasAttribute(constructors2[0], typeof(UseDefinedAttribute)))
					{
						FieldInfo field = ToLuaExport.extendType.GetField(ToLuaExport.extendName + "Defined");
						string value = field.GetValue(null) as string;
						ToLuaExport.sb.AppendLine(value);
						ToLuaExport.sb.AppendLine("\t}");
						return;
					}
				}
			}
			if (constructors.Length == 0)
			{
				if (!ToLuaExport.type.IsValueType)
				{
					ToLuaExport.NoConsturct();
				}
				else
				{
					ToLuaExport.DefaultConstruct();
				}
				return;
			}
			_GenConstruct_c__AnonStorey9A.list = new List<ConstructorInfo>();
			for (int k = 0; k < constructors.Length; k++)
			{
				if (!ToLuaExport.HasDecimal(constructors[k].GetParameters()))
				{
					if (!ToLuaExport.IsObsolete(constructors[k]))
					{
						ConstructorInfo r = constructors[k];
						int num = _GenConstruct_c__AnonStorey9A.list.FindIndex((ConstructorInfo p) => ToLuaExport.CompareMethod(p, r) >= 0);
						if (num >= 0)
						{
							if (ToLuaExport.CompareMethod(_GenConstruct_c__AnonStorey9A.list[num], r) == 2)
							{
								_GenConstruct_c__AnonStorey9A.list.RemoveAt(num);
								_GenConstruct_c__AnonStorey9A.list.Add(r);
							}
						}
						else
						{
							_GenConstruct_c__AnonStorey9A.list.Add(r);
						}
					}
				}
			}
			if (_GenConstruct_c__AnonStorey9A.list.Count == 0)
			{
				if (!ToLuaExport.type.IsValueType)
				{
					ToLuaExport.NoConsturct();
				}
				else
				{
					ToLuaExport.DefaultConstruct();
				}
				return;
			}
			_GenConstruct_c__AnonStorey9A.list.Sort(new Comparison<ConstructorInfo>(ToLuaExport.Compare));
			ToLuaExport.sb.AppendLine("\r\n\t[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]");
			ToLuaExport.sb.AppendFormat("\tstatic int _Create{0}(IntPtr L)\r\n", ToLuaExport.wrapClassName);
			ToLuaExport.sb.AppendLine("\t{");
			ToLuaExport.sb.AppendLine("\t\tint count = LuaDLL.lua_gettop(L);");
			ToLuaExport.sb.AppendLine();
			List<ConstructorInfo> list = new List<ConstructorInfo>();
			int i;
			for (i = 0; i < _GenConstruct_c__AnonStorey9A.list.Count; i++)
			{
				int num2 = _GenConstruct_c__AnonStorey9A.list.FindIndex((ConstructorInfo p) => p != _GenConstruct_c__AnonStorey9A.list[i] && p.GetParameters().Length == _GenConstruct_c__AnonStorey9A.list[i].GetParameters().Length);
				if (num2 >= 0 || (ToLuaExport.HasOptionalParam(_GenConstruct_c__AnonStorey9A.list[i].GetParameters()) && _GenConstruct_c__AnonStorey9A.list[i].GetParameters().Length > 1))
				{
					list.Add(_GenConstruct_c__AnonStorey9A.list[i]);
				}
			}
			MethodBase methodBase = _GenConstruct_c__AnonStorey9A.list[0];
			bool flag = _GenConstruct_c__AnonStorey9A.list[0].GetParameters().Length == 0;
			if (ToLuaExport.HasOptionalParam(methodBase.GetParameters()))
			{
				ParameterInfo[] parameters = methodBase.GetParameters();
				ParameterInfo parameterInfo = parameters[parameters.Length - 1];
				string typeStr = ToLuaExport.GetTypeStr(parameterInfo.ParameterType.GetElementType());
				if (parameters.Length > 1)
				{
					string text = ToLuaExport.GenParamTypes(parameters, true);
					ToLuaExport.sb.AppendFormat("\t\tif (LuaScriptMgr.CheckTypes(L, 1, {0}) && LuaScriptMgr.CheckParamsType(L, typeof({1}), {2}, {3}))\r\n", new object[]
					{
						text,
						typeStr,
						parameters.Length,
						ToLuaExport.GetCountStr(parameters.Length - 1)
					});
				}
				else
				{
					ToLuaExport.sb.AppendFormat("\t\tif (LuaScriptMgr.CheckParamsType(L, typeof({0}), {1}, {2}))\r\n", typeStr, parameters.Length, ToLuaExport.GetCountStr(parameters.Length - 1));
				}
			}
			else
			{
				ParameterInfo[] parameters2 = methodBase.GetParameters();
				if (_GenConstruct_c__AnonStorey9A.list.Count == 1 || methodBase.GetParameters().Length != _GenConstruct_c__AnonStorey9A.list[1].GetParameters().Length)
				{
					ToLuaExport.sb.AppendFormat("\t\tif (count == {0})\r\n", parameters2.Length);
				}
				else
				{
					string arg = ToLuaExport.GenParamTypes(parameters2, true);
					ToLuaExport.sb.AppendFormat("\t\tif (count == {0} && LuaScriptMgr.CheckTypes(L, 1, {1}))\r\n", parameters2.Length, arg);
				}
			}
			ToLuaExport.sb.AppendLine("\t\t{");
			int num3 = ToLuaExport.ProcessParams(methodBase, 3, true, _GenConstruct_c__AnonStorey9A.list.Count > 1, false);
			ToLuaExport.sb.AppendFormat("\t\t\treturn {0};\r\n", num3);
			ToLuaExport.sb.AppendLine("\t\t}");
			for (int j = 1; j < _GenConstruct_c__AnonStorey9A.list.Count; j++)
			{
				flag = (_GenConstruct_c__AnonStorey9A.list[j].GetParameters().Length == 0 || flag);
				methodBase = _GenConstruct_c__AnonStorey9A.list[j];
				ParameterInfo[] parameters3 = methodBase.GetParameters();
				if (!ToLuaExport.HasOptionalParam(methodBase.GetParameters()))
				{
					if (list.Contains(_GenConstruct_c__AnonStorey9A.list[j]))
					{
						string arg2 = ToLuaExport.GenParamTypes(parameters3, true);
						ToLuaExport.sb.AppendFormat("\t\telse if (count == {0} && LuaScriptMgr.CheckTypes(L, 1, {1}))\r\n", parameters3.Length, arg2);
					}
					else
					{
						ToLuaExport.sb.AppendFormat("\t\telse if (count == {0})\r\n", parameters3.Length);
					}
				}
				else
				{
					ParameterInfo parameterInfo2 = parameters3[parameters3.Length - 1];
					string typeStr2 = ToLuaExport.GetTypeStr(parameterInfo2.ParameterType.GetElementType());
					if (parameters3.Length > 1)
					{
						string text2 = ToLuaExport.GenParamTypes(parameters3, true);
						ToLuaExport.sb.AppendFormat("\t\telse if (LuaScriptMgr.CheckTypes(L, 1, {0}) && LuaScriptMgr.CheckParamsType(L, typeof({1}), {2}, {3}))\r\n", new object[]
						{
							text2,
							typeStr2,
							parameters3.Length,
							ToLuaExport.GetCountStr(parameters3.Length - 1)
						});
					}
					else
					{
						ToLuaExport.sb.AppendFormat("\t\telse if (LuaScriptMgr.CheckParamsType(L, typeof({0}), {1}, {2}))\r\n", typeStr2, parameters3.Length, ToLuaExport.GetCountStr(parameters3.Length - 1));
					}
				}
				ToLuaExport.sb.AppendLine("\t\t{");
				num3 = ToLuaExport.ProcessParams(methodBase, 3, true, true, false);
				ToLuaExport.sb.AppendFormat("\t\t\treturn {0};\r\n", num3);
				ToLuaExport.sb.AppendLine("\t\t}");
			}
			if (ToLuaExport.type.IsValueType && !flag)
			{
				ToLuaExport.sb.AppendLine("\t\telse if (count == 0)");
				ToLuaExport.sb.AppendLine("\t\t{");
				ToLuaExport.sb.AppendFormat("\t\t\t{0} obj = new {0}();\r\n", ToLuaExport.className);
				string pushFunction = ToLuaExport.GetPushFunction(ToLuaExport.type);
				ToLuaExport.sb.AppendFormat("\t\t\tLuaScriptMgr.{0}(L, obj);\r\n", pushFunction);
				ToLuaExport.sb.AppendLine("\t\t\treturn 1;");
				ToLuaExport.sb.AppendLine("\t\t}");
			}
			ToLuaExport.sb.AppendLine("\t\telse");
			ToLuaExport.sb.AppendLine("\t\t{");
			ToLuaExport.sb.AppendFormat("\t\t\tLuaDLL.luaL_error(L, \"invalid arguments to method: {0}.New\");\r\n", ToLuaExport.className);
			ToLuaExport.sb.AppendLine("\t\t}");
			ToLuaExport.sb.AppendLine();
			ToLuaExport.sb.AppendLine("\t\treturn 0;");
			ToLuaExport.sb.AppendLine("\t}");
		}

		private static int GetOptionalParamPos(ParameterInfo[] infos)
		{
			for (int i = 0; i < infos.Length; i++)
			{
				if (ToLuaExport.IsParams(infos[i]))
				{
					return i;
				}
			}
			return -1;
		}

		private static int Compare(MethodBase lhs, MethodBase rhs)
		{
			int num = (!lhs.IsStatic) ? 1 : 0;
			int num2 = (!rhs.IsStatic) ? 1 : 0;
			ParameterInfo[] parameters = lhs.GetParameters();
			ParameterInfo[] parameters2 = rhs.GetParameters();
			int num3 = ToLuaExport.GetOptionalParamPos(parameters);
			int num4 = ToLuaExport.GetOptionalParamPos(parameters2);
			if (num3 >= 0 && num4 < 0)
			{
				return 1;
			}
			if (num3 < 0 && num4 >= 0)
			{
				return -1;
			}
			if (num3 >= 0 && num4 >= 0)
			{
				num3 += num;
				num4 += num2;
				if (num3 != num4)
				{
					return (num3 <= num4) ? 1 : -1;
				}
				num3 -= num;
				num4 -= num2;
				if (parameters[num3].ParameterType.GetElementType() == typeof(object) && parameters2[num4].ParameterType.GetElementType() != typeof(object))
				{
					return 1;
				}
				if (parameters[num3].ParameterType.GetElementType() != typeof(object) && parameters2[num4].ParameterType.GetElementType() == typeof(object))
				{
					return -1;
				}
			}
			int num5 = num + parameters.Length;
			int num6 = num2 + parameters2.Length;
			if (num5 > num6)
			{
				return 1;
			}
			if (num5 == num6)
			{
				List<ParameterInfo> list = new List<ParameterInfo>(parameters);
				List<ParameterInfo> list2 = new List<ParameterInfo>(parameters2);
				if (list.Count > list2.Count)
				{
					if (list[0].ParameterType == typeof(object))
					{
						return 1;
					}
					list.RemoveAt(0);
				}
				else if (list2.Count > list.Count)
				{
					if (list2[0].ParameterType == typeof(object))
					{
						return -1;
					}
					list2.RemoveAt(0);
				}
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i].ParameterType == typeof(object) && list2[i].ParameterType != typeof(object))
					{
						return 1;
					}
					if (list[i].ParameterType != typeof(object) && list2[i].ParameterType == typeof(object))
					{
						return -1;
					}
				}
				return 0;
			}
			return -1;
		}

		private static bool HasOptionalParam(ParameterInfo[] infos)
		{
			for (int i = 0; i < infos.Length; i++)
			{
				if (ToLuaExport.IsParams(infos[i]))
				{
					return true;
				}
			}
			return false;
		}

		private static Type GetRefBaseType(string str)
		{
			int num = str.IndexOf("&");
			string text = (num < 0) ? str : str.Remove(num);
			Type type = Type.GetType(text);
			if (type == null)
			{
				type = Type.GetType(text + ", UnityEngine");
			}
			if (type == null)
			{
				type = Type.GetType(text + ", Assembly-CSharp-firstpass");
			}
			return type;
		}

		private static int ProcessParams(MethodBase md, int tab, bool beConstruct, bool beLuaString, bool beCheckTypes = false)
		{
			ParameterInfo[] parameters = md.GetParameters();
			int num = parameters.Length;
			string text = string.Empty;
			for (int i = 0; i < tab; i++)
			{
				text += "\t";
			}
			if (!md.IsStatic && !beConstruct)
			{
				if (md.Name == "Equals")
				{
					if (!ToLuaExport.type.IsValueType)
					{
						ToLuaExport.sb.AppendFormat("{0}{1} obj = LuaScriptMgr.GetVarObject(L, 1) as {1};\r\n", text, ToLuaExport.className);
					}
					else
					{
						ToLuaExport.sb.AppendFormat("{0}{1} obj = ({1})LuaScriptMgr.GetVarObject(L, 1);\r\n", text, ToLuaExport.className);
					}
				}
				else if (ToLuaExport.className != "Type" && ToLuaExport.className != "System.Type")
				{
					if (typeof(UnityEngine.Object).IsAssignableFrom(ToLuaExport.type))
					{
						ToLuaExport.sb.AppendFormat("{0}{1} obj = ({1})LuaScriptMgr.GetUnityObjectSelf(L, 1, \"{1}\");\r\n", text, ToLuaExport.className);
					}
					else if (typeof(TrackedReference).IsAssignableFrom(ToLuaExport.type))
					{
						ToLuaExport.sb.AppendFormat("{0}{1} obj = ({1})LuaScriptMgr.GetTrackedObjectSelf(L, 1, \"{1}\");\r\n", text, ToLuaExport.className);
					}
					else
					{
						ToLuaExport.sb.AppendFormat("{0}{1} obj = ({1})LuaScriptMgr.GetNetObjectSelf(L, 1, \"{1}\");\r\n", text, ToLuaExport.className);
					}
				}
				else
				{
					ToLuaExport.sb.AppendFormat("{0}{1} obj = LuaScriptMgr.GetTypeObject(L, 1);\r\n", text, ToLuaExport.className);
				}
			}
			for (int j = 0; j < num; j++)
			{
				ParameterInfo parameterInfo = parameters[j];
				string typeStr = ToLuaExport.GetTypeStr(parameterInfo.ParameterType);
				string text2 = "arg" + j;
				int num2 = (!md.IsStatic && !beConstruct) ? 2 : 1;
				if (parameterInfo.Attributes == ParameterAttributes.Out)
				{
					Type refBaseType = ToLuaExport.GetRefBaseType(parameterInfo.ParameterType.ToString());
					if (refBaseType.IsValueType)
					{
						ToLuaExport.sb.AppendFormat("{0}{1} {2};\r\n", text, typeStr, text2);
					}
					else
					{
						ToLuaExport.sb.AppendFormat("{0}{1} {2} = null;\r\n", text, typeStr, text2);
					}
				}
				else if (parameterInfo.ParameterType == typeof(bool))
				{
					if (beCheckTypes)
					{
						ToLuaExport.sb.AppendFormat("{2}bool {0} = LuaDLL.lua_toboolean(L, {1});\r\n", text2, j + num2, text);
					}
					else
					{
						ToLuaExport.sb.AppendFormat("{2}bool {0} = LuaScriptMgr.GetBoolean(L, {1});\r\n", text2, j + num2, text);
					}
				}
				else if (parameterInfo.ParameterType == typeof(string))
				{
					string text3 = (!beLuaString) ? "GetLuaString" : "GetString";
					ToLuaExport.sb.AppendFormat("{2}string {0} = LuaScriptMgr.{3}(L, {1});\r\n", new object[]
					{
						text2,
						j + num2,
						text,
						text3
					});
				}
				else if (parameterInfo.ParameterType.IsPrimitive)
				{
					if (beCheckTypes)
					{
						ToLuaExport.sb.AppendFormat("{3}{0} {1} = ({0})LuaDLL.lua_tonumber(L, {2});\r\n", new object[]
						{
							typeStr,
							text2,
							j + num2,
							text
						});
					}
					else
					{
						ToLuaExport.sb.AppendFormat("{3}{0} {1} = ({0})LuaScriptMgr.GetNumber(L, {2});\r\n", new object[]
						{
							typeStr,
							text2,
							j + num2,
							text
						});
					}
				}
				else if (parameterInfo.ParameterType == typeof(LuaFunction))
				{
					if (beCheckTypes)
					{
						ToLuaExport.sb.AppendFormat("{2}LuaFunction {0} = LuaScriptMgr.ToLuaFunction(L, {1});\r\n", text2, j + num2, text);
					}
					else
					{
						ToLuaExport.sb.AppendFormat("{2}LuaFunction {0} = LuaScriptMgr.GetLuaFunction(L, {1});\r\n", text2, j + num2, text);
					}
				}
				else if (parameterInfo.ParameterType.IsSubclassOf(typeof(MulticastDelegate)))
				{
					ToLuaExport.sb.AppendFormat("{0}{1} {2} = null;\r\n", text, typeStr, text2);
					ToLuaExport.sb.AppendFormat("{0}LuaTypes funcType{1} = LuaDLL.lua_type(L, {1});\r\n", text, j + num2);
					ToLuaExport.sb.AppendLine();
					ToLuaExport.sb.AppendFormat("{0}if (funcType{1} != LuaTypes.LUA_TFUNCTION)\r\n", text, j + num2);
					ToLuaExport.sb.AppendLine(text + "{");
					if (beCheckTypes)
					{
						ToLuaExport.sb.AppendFormat("{3} {1} = ({0})LuaScriptMgr.GetLuaObject(L, {2});\r\n", new object[]
						{
							typeStr,
							text2,
							j + num2,
							text + "\t"
						});
					}
					else
					{
						ToLuaExport.sb.AppendFormat("{3} {1} = ({0})LuaScriptMgr.GetNetObject(L, {2}, typeof({0}));\r\n", new object[]
						{
							typeStr,
							text2,
							j + num2,
							text + "\t"
						});
					}
					ToLuaExport.sb.AppendFormat("{0}}}\r\n{0}else\r\n{0}{{\r\n", text);
					ToLuaExport.sb.AppendFormat("{0}\tLuaFunction func = LuaScriptMgr.GetLuaFunction(L, {1});\r\n", text, j + num2);
					ToLuaExport.sb.AppendFormat("{0}\t{1} = ", text, text2);
					ToLuaExport.GenDelegateBody(parameterInfo.ParameterType, text + "\t", true);
					ToLuaExport.sb.AppendLine(text + "}\r\n");
				}
				else if (parameterInfo.ParameterType == typeof(LuaTable))
				{
					if (beCheckTypes)
					{
						ToLuaExport.sb.AppendFormat("{2}LuaTable {0} = LuaScriptMgr.ToLuaTable(L, {1});\r\n", text2, j + num2, text);
					}
					else
					{
						ToLuaExport.sb.AppendFormat("{2}LuaTable {0} = LuaScriptMgr.GetLuaTable(L, {1});\r\n", text2, j + num2, text);
					}
				}
				else if (parameterInfo.ParameterType == typeof(Vector2) || ToLuaExport.GetRefBaseType(parameterInfo.ParameterType.ToString()) == typeof(Vector2))
				{
					ToLuaExport.sb.AppendFormat("{2}Vector2 {0} = LuaScriptMgr.GetVector2(L, {1});\r\n", text2, j + num2, text);
				}
				else if (parameterInfo.ParameterType == typeof(Vector3) || ToLuaExport.GetRefBaseType(parameterInfo.ParameterType.ToString()) == typeof(Vector3))
				{
					ToLuaExport.sb.AppendFormat("{2}Vector3 {0} = LuaScriptMgr.GetVector3(L, {1});\r\n", text2, j + num2, text);
				}
				else if (parameterInfo.ParameterType == typeof(Vector4) || ToLuaExport.GetRefBaseType(parameterInfo.ParameterType.ToString()) == typeof(Vector4))
				{
					ToLuaExport.sb.AppendFormat("{2}Vector4 {0} = LuaScriptMgr.GetVector4(L, {1});\r\n", text2, j + num2, text);
				}
				else if (parameterInfo.ParameterType == typeof(Quaternion) || ToLuaExport.GetRefBaseType(parameterInfo.ParameterType.ToString()) == typeof(Quaternion))
				{
					ToLuaExport.sb.AppendFormat("{2}Quaternion {0} = LuaScriptMgr.GetQuaternion(L, {1});\r\n", text2, j + num2, text);
				}
				else if (parameterInfo.ParameterType == typeof(Color) || ToLuaExport.GetRefBaseType(parameterInfo.ParameterType.ToString()) == typeof(Color))
				{
					ToLuaExport.sb.AppendFormat("{2}Color {0} = LuaScriptMgr.GetColor(L, {1});\r\n", text2, j + num2, text);
				}
				else if (parameterInfo.ParameterType == typeof(Ray) || ToLuaExport.GetRefBaseType(parameterInfo.ParameterType.ToString()) == typeof(Ray))
				{
					ToLuaExport.sb.AppendFormat("{2}Ray {0} = LuaScriptMgr.GetRay(L, {1});\r\n", text2, j + num2, text);
				}
				else if (parameterInfo.ParameterType == typeof(Bounds) || ToLuaExport.GetRefBaseType(parameterInfo.ParameterType.ToString()) == typeof(Bounds))
				{
					ToLuaExport.sb.AppendFormat("{2}Bounds {0} = LuaScriptMgr.GetBounds(L, {1});\r\n", text2, j + num2, text);
				}
				else if (parameterInfo.ParameterType == typeof(object))
				{
					ToLuaExport.sb.AppendFormat("{2}object {0} = LuaScriptMgr.GetVarObject(L, {1});\r\n", text2, j + num2, text);
				}
				else if (parameterInfo.ParameterType == typeof(Type))
				{
					ToLuaExport.sb.AppendFormat("{0}{1} {2} = LuaScriptMgr.GetTypeObject(L, {3});\r\n", new object[]
					{
						text,
						typeStr,
						text2,
						j + num2
					});
				}
				else if (parameterInfo.ParameterType == typeof(LuaStringBuffer))
				{
					ToLuaExport.sb.AppendFormat("{2}LuaStringBuffer {0} = LuaScriptMgr.GetStringBuffer(L, {1});\r\n", text2, j + num2, text);
				}
				else if (parameterInfo.ParameterType.IsArray)
				{
					Type elementType = parameterInfo.ParameterType.GetElementType();
					string typeStr2 = ToLuaExport.GetTypeStr(elementType);
					bool flag = false;
					bool flag2 = false;
					bool flag3 = false;
					string text4;
					if (elementType == typeof(bool))
					{
						text4 = "GetArrayBool";
					}
					else if (elementType.IsPrimitive)
					{
						flag = true;
						text4 = "GetArrayNumber";
					}
					else if (elementType == typeof(string))
					{
						flag2 = ToLuaExport.IsParams(parameterInfo);
						text4 = ((!flag2) ? "GetArrayString" : "GetParamsString");
					}
					else
					{
						flag = true;
						flag2 = ToLuaExport.IsParams(parameterInfo);
						text4 = ((!flag2) ? "GetArrayObject" : "GetParamsObject");
						if (elementType == typeof(object))
						{
							flag3 = true;
						}
						if (elementType == typeof(UnityEngine.Object))
						{
							ToLuaExport.ambig |= ObjAmbig.U3dObj;
						}
					}
					if (flag)
					{
						if (flag2)
						{
							if (!flag3)
							{
								ToLuaExport.sb.AppendFormat("{5}{0}[] objs{2} = LuaScriptMgr.{4}<{0}>(L, {1}, {3});\r\n", new object[]
								{
									typeStr2,
									j + num2,
									j,
									ToLuaExport.GetCountStr(j + num2 - 1),
									text4,
									text
								});
							}
							else
							{
								ToLuaExport.sb.AppendFormat("{4}object[] objs{1} = LuaScriptMgr.{3}(L, {0}, {2});\r\n", new object[]
								{
									j + num2,
									j,
									ToLuaExport.GetCountStr(j + num2 - 1),
									text4,
									text
								});
							}
						}
						else
						{
							ToLuaExport.sb.AppendFormat("{4}{0}[] objs{2} = LuaScriptMgr.{3}<{0}>(L, {1});\r\n", new object[]
							{
								typeStr2,
								j + num2,
								j,
								text4,
								text
							});
						}
					}
					else if (flag2)
					{
						ToLuaExport.sb.AppendFormat("{5}{0}[] objs{2} = LuaScriptMgr.{4}(L, {1}, {3});\r\n", new object[]
						{
							typeStr2,
							j + num2,
							j,
							ToLuaExport.GetCountStr(j + num2 - 1),
							text4,
							text
						});
					}
					else
					{
						ToLuaExport.sb.AppendFormat("{5}{0}[] objs{2} = LuaScriptMgr.{4}(L, {1});\r\n", new object[]
						{
							typeStr2,
							j + num2,
							j,
							j + num2 - 1,
							text4,
							text
						});
					}
				}
				else if (md.Name == "op_Equality")
				{
					if (!ToLuaExport.type.IsValueType)
					{
						ToLuaExport.sb.AppendFormat("{3}{0} {1} = LuaScriptMgr.GetLuaObject(L, {2}) as {0};\r\n", new object[]
						{
							typeStr,
							text2,
							j + num2,
							text
						});
					}
					else
					{
						ToLuaExport.sb.AppendFormat("{3}{0} {1} = ({0})LuaScriptMgr.GetVarObject(L, {2});\r\n", new object[]
						{
							typeStr,
							text2,
							j + num2,
							text
						});
					}
				}
				else if (beCheckTypes)
				{
					ToLuaExport.sb.AppendFormat("{3}{0} {1} = ({0})LuaScriptMgr.GetLuaObject(L, {2});\r\n", new object[]
					{
						typeStr,
						text2,
						j + num2,
						text
					});
				}
				else if (typeof(UnityEngine.Object).IsAssignableFrom(parameterInfo.ParameterType))
				{
					ToLuaExport.sb.AppendFormat("{3}{0} {1} = ({0})LuaScriptMgr.GetUnityObject(L, {2}, typeof({0}));\r\n", new object[]
					{
						typeStr,
						text2,
						j + num2,
						text
					});
				}
				else if (typeof(TrackedReference).IsAssignableFrom(parameterInfo.ParameterType))
				{
					ToLuaExport.sb.AppendFormat("{3}{0} {1} = ({0})LuaScriptMgr.GetTrackedObject(L, {2}, typeof({0}));\r\n", new object[]
					{
						typeStr,
						text2,
						j + num2,
						text
					});
				}
				else
				{
					ToLuaExport.sb.AppendFormat("{3}{0} {1} = ({0})LuaScriptMgr.GetNetObject(L, {2}, typeof({0}));\r\n", new object[]
					{
						typeStr,
						text2,
						j + num2,
						text
					});
				}
			}
			StringBuilder stringBuilder = new StringBuilder();
			List<string> list = new List<string>();
			List<Type> list2 = new List<Type>();
			for (int k = 0; k < num - 1; k++)
			{
				ParameterInfo parameterInfo2 = parameters[k];
				if (!parameterInfo2.ParameterType.IsArray)
				{
					if (!parameterInfo2.ParameterType.ToString().Contains("&"))
					{
						stringBuilder.Append("arg");
					}
					else
					{
						if (parameterInfo2.Attributes == ParameterAttributes.Out)
						{
							stringBuilder.Append("out arg");
						}
						else
						{
							stringBuilder.Append("ref arg");
						}
						list.Add("arg" + k);
						list2.Add(ToLuaExport.GetRefBaseType(parameterInfo2.ParameterType.ToString()));
					}
				}
				else
				{
					stringBuilder.Append("objs");
				}
				stringBuilder.Append(k);
				stringBuilder.Append(",");
			}
			if (num > 0)
			{
				ParameterInfo parameterInfo3 = parameters[num - 1];
				if (!parameterInfo3.ParameterType.IsArray)
				{
					if (!parameterInfo3.ParameterType.ToString().Contains("&"))
					{
						stringBuilder.Append("arg");
					}
					else
					{
						if (parameterInfo3.Attributes == ParameterAttributes.Out)
						{
							stringBuilder.Append("out arg");
						}
						else
						{
							stringBuilder.Append("ref arg");
						}
						list.Add("arg" + (num - 1));
						list2.Add(ToLuaExport.GetRefBaseType(parameterInfo3.ParameterType.ToString()));
					}
				}
				else
				{
					stringBuilder.Append("objs");
				}
				stringBuilder.Append(num - 1);
			}
			if (beConstruct)
			{
				ToLuaExport.sb.AppendFormat("{2}{0} obj = new {0}({1});\r\n", ToLuaExport.className, stringBuilder.ToString(), text);
				string pushFunction = ToLuaExport.GetPushFunction(ToLuaExport.type);
				ToLuaExport.sb.AppendFormat("{0}LuaScriptMgr.{1}(L, obj);\r\n", text, pushFunction);
				for (int l = 0; l < list.Count; l++)
				{
					pushFunction = ToLuaExport.GetPushFunction(list2[l]);
					ToLuaExport.sb.AppendFormat("{1}LuaScriptMgr.{2}(L, {0});\r\n", list[l], text, pushFunction);
				}
				return list.Count + 1;
			}
			string text5 = (!md.IsStatic) ? "obj" : ToLuaExport.className;
			MethodInfo methodInfo = md as MethodInfo;
			if (methodInfo.ReturnType == typeof(void))
			{
				if (md.Name == "set_Item")
				{
					if (num == 2)
					{
						ToLuaExport.sb.AppendFormat("{0}{1}[arg0] = arg1;\r\n", text, text5);
					}
					else if (num == 3)
					{
						ToLuaExport.sb.AppendFormat("{0}{1}[arg0, arg1] = arg2;\r\n", text, text5);
					}
				}
				else
				{
					ToLuaExport.sb.AppendFormat("{3}{0}.{1}({2});\r\n", new object[]
					{
						text5,
						md.Name,
						stringBuilder.ToString(),
						text
					});
				}
				if (!md.IsStatic && ToLuaExport.type.IsValueType)
				{
					ToLuaExport.sb.AppendFormat("{0}LuaScriptMgr.SetValueObject(L, 1, obj);\r\n", text);
				}
			}
			else
			{
				string typeStr3 = ToLuaExport.GetTypeStr(methodInfo.ReturnType);
				if (md.Name.Contains("op_"))
				{
					ToLuaExport.CallOpFunction(md.Name, tab, typeStr3);
				}
				else if (md.Name == "get_Item")
				{
					ToLuaExport.sb.AppendFormat("{4}{3} o = {0}[{2}];\r\n", new object[]
					{
						text5,
						md.Name,
						stringBuilder.ToString(),
						typeStr3,
						text
					});
				}
				else if (md.Name == "Equals")
				{
					if (ToLuaExport.type.IsValueType)
					{
						ToLuaExport.sb.AppendFormat("{0}bool o = obj.Equals(arg0);\r\n", text);
					}
					else
					{
						ToLuaExport.sb.AppendFormat("{0}bool o = obj != null ? obj.Equals(arg0) : arg0 == null;\r\n", text);
					}
				}
				else
				{
					ToLuaExport.sb.AppendFormat("{4}{3} o = {0}.{1}({2});\r\n", new object[]
					{
						text5,
						md.Name,
						stringBuilder.ToString(),
						typeStr3,
						text
					});
				}
				string pushFunction2 = ToLuaExport.GetPushFunction(methodInfo.ReturnType);
				ToLuaExport.sb.AppendFormat("{0}LuaScriptMgr.{1}(L, o);\r\n", text, pushFunction2);
			}
			for (int m = 0; m < list.Count; m++)
			{
				string pushFunction3 = ToLuaExport.GetPushFunction(list2[m]);
				ToLuaExport.sb.AppendFormat("{1}LuaScriptMgr.{2}(L, {0});\r\n", list[m], text, pushFunction3);
			}
			return list.Count;
		}

		private static bool CompareParmsCount(MethodBase l, MethodBase r)
		{
			if (l == r)
			{
				return false;
			}
			int num = (!l.IsStatic) ? 1 : 0;
			int num2 = (!r.IsStatic) ? 1 : 0;
			num += l.GetParameters().Length;
			num2 += r.GetParameters().Length;
			return num == num2;
		}

		private static int CompareMethod(MethodBase l, MethodBase r)
		{
			int num = 0;
			if (!ToLuaExport.CompareParmsCount(l, r))
			{
				return -1;
			}
			ParameterInfo[] parameters = l.GetParameters();
			ParameterInfo[] parameters2 = r.GetParameters();
			List<Type> list = new List<Type>();
			List<Type> list2 = new List<Type>();
			if (!l.IsStatic)
			{
				list.Add(ToLuaExport.type);
			}
			if (!r.IsStatic)
			{
				list2.Add(ToLuaExport.type);
			}
			for (int i = 0; i < parameters.Length; i++)
			{
				list.Add(parameters[i].ParameterType);
			}
			for (int j = 0; j < parameters2.Length; j++)
			{
				list2.Add(parameters2[j].ParameterType);
			}
			for (int k = 0; k < list.Count; k++)
			{
				if (!ToLuaExport.typeSize.ContainsKey(list[k]) || !ToLuaExport.typeSize.ContainsKey(list2[k]))
				{
					if (list[k] != list2[k])
					{
						return -1;
					}
				}
				else if (list[k].IsPrimitive && list2[k].IsPrimitive && num == 0)
				{
					num = ((ToLuaExport.typeSize[list[k]] < ToLuaExport.typeSize[list2[k]]) ? 2 : 1);
				}
				else if (list[k] != list2[k])
				{
					return -1;
				}
			}
			if (num == 0 && l.IsStatic)
			{
				num = 2;
			}
			return num;
		}

		private static void Push(List<MethodInfo> list, MethodInfo r)
		{
			int num = list.FindIndex((MethodInfo p) => p.Name == r.Name && ToLuaExport.CompareMethod(p, r) >= 0);
			if (num < 0)
			{
				list.Add(r);
				return;
			}
			if (ToLuaExport.CompareMethod(list[num], r) == 2)
			{
				list.RemoveAt(num);
				list.Add(r);
				return;
			}
		}

		private static bool HasDecimal(ParameterInfo[] pi)
		{
			for (int i = 0; i < pi.Length; i++)
			{
				if (pi[i].ParameterType == typeof(decimal))
				{
					return true;
				}
			}
			return false;
		}

		
		private sealed class _GenOverrideFunc_c__AnonStorey9D
		{
			internal List<MethodInfo> list;
		}

		public static MethodInfo GenOverrideFunc(string name)
		{
			ToLuaExport._GenOverrideFunc_c__AnonStorey9D _GenOverrideFunc_c__AnonStorey9D = new ToLuaExport._GenOverrideFunc_c__AnonStorey9D();
			_GenOverrideFunc_c__AnonStorey9D.list = new List<MethodInfo>();
			for (int k = 0; k < ToLuaExport.methods.Length; k++)
			{
				if (ToLuaExport.methods[k].Name == name && !ToLuaExport.methods[k].IsGenericMethod && !ToLuaExport.HasDecimal(ToLuaExport.methods[k].GetParameters()))
				{
					ToLuaExport.Push(_GenOverrideFunc_c__AnonStorey9D.list, ToLuaExport.methods[k]);
				}
			}
			if (_GenOverrideFunc_c__AnonStorey9D.list.Count == 1)
			{
				return _GenOverrideFunc_c__AnonStorey9D.list[0];
			}
			_GenOverrideFunc_c__AnonStorey9D.list.Sort(new Comparison<MethodInfo>(ToLuaExport.Compare));
			ToLuaExport.sb.AppendLine("\r\n\t[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]");
			ToLuaExport.sb.AppendFormat("\tstatic int {0}(IntPtr L)\r\n", ToLuaExport.GetFuncName(name));
			ToLuaExport.sb.AppendLine("\t{");
			ToLuaExport.sb.AppendLine("\t\tint count = LuaDLL.lua_gettop(L);");
			List<MethodInfo> list = new List<MethodInfo>();
			int i;
			for (i = 0; i < _GenOverrideFunc_c__AnonStorey9D.list.Count; i++)
			{
				int num = _GenOverrideFunc_c__AnonStorey9D.list.FindIndex((MethodInfo p) => ToLuaExport.CompareParmsCount(p, _GenOverrideFunc_c__AnonStorey9D.list[i]));
				if (num >= 0 || (ToLuaExport.HasOptionalParam(_GenOverrideFunc_c__AnonStorey9D.list[i].GetParameters()) && _GenOverrideFunc_c__AnonStorey9D.list[i].GetParameters().Length > 1))
				{
					list.Add(_GenOverrideFunc_c__AnonStorey9D.list[i]);
				}
			}
			ToLuaExport.sb.AppendLine();
			MethodInfo methodInfo = _GenOverrideFunc_c__AnonStorey9D.list[0];
			int num2 = (methodInfo.ReturnType != typeof(void)) ? 1 : 0;
			int num3 = (!methodInfo.IsStatic) ? 1 : 0;
			int num4 = num3 + 1;
			int num5 = methodInfo.GetParameters().Length + num3;
			int num6 = _GenOverrideFunc_c__AnonStorey9D.list[1].GetParameters().Length + ((!_GenOverrideFunc_c__AnonStorey9D.list[1].IsStatic) ? 1 : 0);
			bool flag = true;
			bool beCheckTypes = true;
			if (ToLuaExport.HasOptionalParam(methodInfo.GetParameters()))
			{
				ParameterInfo[] parameters = methodInfo.GetParameters();
				ParameterInfo parameterInfo = parameters[parameters.Length - 1];
				string typeStr = ToLuaExport.GetTypeStr(parameterInfo.ParameterType.GetElementType());
				if (parameters.Length > 1)
				{
					string text = ToLuaExport.GenParamTypes(parameters, methodInfo.IsStatic);
					ToLuaExport.sb.AppendFormat("\t\tif (LuaScriptMgr.CheckTypes(L, 1, {1}) && LuaScriptMgr.CheckParamsType(L, typeof({2}), {3}, {4}))\r\n", new object[]
					{
						num4,
						text,
						typeStr,
						parameters.Length + num3,
						ToLuaExport.GetCountStr(parameters.Length + num3 - 1)
					});
				}
				else
				{
					ToLuaExport.sb.AppendFormat("\t\tif (LuaScriptMgr.CheckParamsType(L, typeof({0}), {1}, {2}))\r\n", typeStr, parameters.Length + num3, ToLuaExport.GetCountStr(parameters.Length + num3 - 1));
				}
			}
			else if (num5 != num6)
			{
				ToLuaExport.sb.AppendFormat("\t\tif (count == {0})\r\n", methodInfo.GetParameters().Length + num3);
				flag = false;
				beCheckTypes = false;
			}
			else
			{
				ParameterInfo[] parameters2 = methodInfo.GetParameters();
				if (parameters2.Length > 0)
				{
					string arg = ToLuaExport.GenParamTypes(parameters2, methodInfo.IsStatic);
					ToLuaExport.sb.AppendFormat("\t\tif (count == {0} && LuaScriptMgr.CheckTypes(L, 1, {2}))\r\n", parameters2.Length + num3, num4, arg);
				}
				else
				{
					ToLuaExport.sb.AppendFormat("\t\tif (count == {0})\r\n", parameters2.Length + num3);
				}
			}
			ToLuaExport.sb.AppendLine("\t\t{");
			int num7 = ToLuaExport.ProcessParams(methodInfo, 3, false, _GenOverrideFunc_c__AnonStorey9D.list.Count > 1 && flag, beCheckTypes);
			ToLuaExport.sb.AppendFormat("\t\t\treturn {0};\r\n", num2 + num7);
			ToLuaExport.sb.AppendLine("\t\t}");
			for (int j = 1; j < _GenOverrideFunc_c__AnonStorey9D.list.Count; j++)
			{
				flag = true;
				beCheckTypes = true;
				methodInfo = _GenOverrideFunc_c__AnonStorey9D.list[j];
				num3 = ((!methodInfo.IsStatic) ? 1 : 0);
				num4 = num3 + 1;
				num2 = ((methodInfo.ReturnType != typeof(void)) ? 1 : 0);
				if (!ToLuaExport.HasOptionalParam(methodInfo.GetParameters()))
				{
					ParameterInfo[] parameters3 = methodInfo.GetParameters();
					if (list.Contains(_GenOverrideFunc_c__AnonStorey9D.list[j]))
					{
						string arg2 = ToLuaExport.GenParamTypes(parameters3, methodInfo.IsStatic);
						ToLuaExport.sb.AppendFormat("\t\telse if (count == {0} && LuaScriptMgr.CheckTypes(L, 1, {2}))\r\n", parameters3.Length + num3, num4, arg2);
					}
					else
					{
						ToLuaExport.sb.AppendFormat("\t\telse if (count == {0})\r\n", parameters3.Length + num3);
						flag = false;
						beCheckTypes = false;
					}
				}
				else
				{
					ParameterInfo[] parameters4 = methodInfo.GetParameters();
					ParameterInfo parameterInfo2 = parameters4[parameters4.Length - 1];
					string typeStr2 = ToLuaExport.GetTypeStr(parameterInfo2.ParameterType.GetElementType());
					if (parameters4.Length > 1)
					{
						string text2 = ToLuaExport.GenParamTypes(parameters4, methodInfo.IsStatic);
						ToLuaExport.sb.AppendFormat("\t\telse if (LuaScriptMgr.CheckTypes(L, 1, {1}) && LuaScriptMgr.CheckParamsType(L, typeof({2}), {3}, {4}))\r\n", new object[]
						{
							num4,
							text2,
							typeStr2,
							parameters4.Length + num3,
							ToLuaExport.GetCountStr(parameters4.Length + num3 - 1)
						});
					}
					else
					{
						ToLuaExport.sb.AppendFormat("\t\telse if (LuaScriptMgr.CheckParamsType(L, typeof({0}), {1}, {2}))\r\n", typeStr2, parameters4.Length + num3, ToLuaExport.GetCountStr(parameters4.Length + num3 - 1));
					}
				}
				ToLuaExport.sb.AppendLine("\t\t{");
				num7 = ToLuaExport.ProcessParams(methodInfo, 3, false, flag, beCheckTypes);
				ToLuaExport.sb.AppendFormat("\t\t\treturn {0};\r\n", num2 + num7);
				ToLuaExport.sb.AppendLine("\t\t}");
			}
			ToLuaExport.sb.AppendLine("\t\telse");
			ToLuaExport.sb.AppendLine("\t\t{");
			ToLuaExport.sb.AppendFormat("\t\t\tLuaDLL.luaL_error(L, \"invalid arguments to method: {0}.{1}\");\r\n", ToLuaExport.className, name);
			ToLuaExport.sb.AppendLine("\t\t}");
			ToLuaExport.sb.AppendLine();
			ToLuaExport.sb.AppendLine("\t\treturn 0;");
			ToLuaExport.sb.AppendLine("\t}");
			return null;
		}

		private static string[] GetGenericName(Type[] types)
		{
			string[] array = new string[types.Length];
			for (int i = 0; i < types.Length; i++)
			{
				if (types[i].IsGenericType)
				{
					array[i] = ToLuaExport.GetGenericName(types[i]);
				}
				else
				{
					array[i] = ToLuaExport.GetTypeStr(types[i]);
				}
			}
			return array;
		}

		private static string GetGenericName(Type t)
		{
			Type[] genericArguments = t.GetGenericArguments();
			string fullName = t.FullName;
			string text = fullName.Substring(0, fullName.IndexOf('`'));
			text = ToLuaExport._C(text);
			if (!fullName.Contains("+"))
			{
				return text + "<" + string.Join(",", ToLuaExport.GetGenericName(genericArguments)) + ">";
			}
			int num = fullName.IndexOf("+");
			int num2 = fullName.IndexOf("[");
			if (num2 > num)
			{
				string text2 = fullName.Substring(num + 1, num2 - num - 1);
				return string.Concat(new string[]
				{
					text,
					"<",
					string.Join(",", ToLuaExport.GetGenericName(genericArguments)),
					">.",
					text2
				});
			}
			return text + "<" + string.Join(",", ToLuaExport.GetGenericName(genericArguments)) + ">";
		}

		public static string GetTypeStr(Type t)
		{
			if (t.IsArray)
			{
				t = t.GetElementType();
				string typeStr = ToLuaExport.GetTypeStr(t);
				return typeStr + "[]";
			}
			if (t.IsGenericType)
			{
				return ToLuaExport.GetGenericName(t);
			}
			return ToLuaExport._C(t.ToString());
		}

		public static string _C(string str)
		{
			if (str.Length > 1 && str[str.Length - 1] == '&')
			{
				str = str.Remove(str.Length - 1);
			}
			if (str == "System.Single" || str == "Single")
			{
				return "float";
			}
			if (str == "System.String" || str == "String")
			{
				return "string";
			}
			if (str == "System.Int32" || str == "Int32")
			{
				return "int";
			}
			if (str == "System.Int64" || str == "Int64")
			{
				return "long";
			}
			if (str == "System.SByte" || str == "SByte")
			{
				return "sbyte";
			}
			if (str == "System.Byte" || str == "Byte")
			{
				return "byte";
			}
			if (str == "System.Int16" || str == "Int16")
			{
				return "short";
			}
			if (str == "System.UInt16" || str == "UInt16")
			{
				return "ushort";
			}
			if (str == "System.Char" || str == "Char")
			{
				return "char";
			}
			if (str == "System.UInt32" || str == "UInt32")
			{
				return "uint";
			}
			if (str == "System.UInt64" || str == "UInt64")
			{
				return "ulong";
			}
			if (str == "System.Decimal" || str == "Decimal")
			{
				return "decimal";
			}
			if (str == "System.Double" || str == "Double")
			{
				return "double";
			}
			if (str == "System.Boolean" || str == "Boolean")
			{
				return "bool";
			}
			if (str == "System.Object")
			{
				return "object";
			}
			if (str.Contains("."))
			{
				int num = str.LastIndexOf('.');
				string text = str.Substring(0, num);
				if (str.Length > 12 && str.Substring(0, 12) == "UnityEngine.")
				{
					if (text == "UnityEngine")
					{
						ToLuaExport.usingList.Add("UnityEngine");
					}
					else if (text == "UnityEngine.UI")
					{
						ToLuaExport.usingList.Add("UnityEngine.UI");
					}
					else if (text == "UnityEngine.EventSystems")
					{
						ToLuaExport.usingList.Add("UnityEngine.EventSystems");
					}
					if (str == "UnityEngine.Object")
					{
						ToLuaExport.ambig |= ObjAmbig.U3dObj;
					}
				}
				else if (str.Length > 7 && str.Substring(0, 7) == "System.")
				{
					if (text == "System.Collections")
					{
						ToLuaExport.usingList.Add(text);
					}
					else if (text == "System.Collections.Generic")
					{
						ToLuaExport.usingList.Add(text);
					}
					else if (text == "System")
					{
						ToLuaExport.usingList.Add(text);
					}
					if (str == "System.Object")
					{
						str = "object";
					}
				}
				if (ToLuaExport.usingList.Contains(text))
				{
					str = str.Substring(num + 1);
				}
			}
			if (str.Contains("+"))
			{
				return str.Replace('+', '.');
			}
			if (str == ToLuaExport.extendName)
			{
				return ToLuaExport.GetTypeStr(ToLuaExport.type);
			}
			return str;
		}

		private static bool IsLuaTableType(Type t)
		{
			if (t.IsArray)
			{
				t = t.GetElementType();
			}
			return t == typeof(Vector3) || t == typeof(Vector2) || t == typeof(Vector4) || t == typeof(Quaternion) || t == typeof(Color) || t == typeof(Ray) || t == typeof(Bounds);
		}

		private static string GetTypeOf(Type t, string sep)
		{
			string result;
			if (t == null)
			{
				result = string.Format("null{0}", sep);
			}
			else if (ToLuaExport.IsLuaTableType(t))
			{
				result = string.Format("typeof(LuaTable{1}){0}", sep, (!t.IsArray) ? string.Empty : "[]");
			}
			else
			{
				result = string.Format("typeof({0}){1}", ToLuaExport.GetTypeStr(t), sep);
			}
			return result;
		}

		private static string GenParamTypes(ParameterInfo[] p, bool isStatic)
		{
			StringBuilder stringBuilder = new StringBuilder();
			List<Type> list = new List<Type>();
			if (!isStatic)
			{
				list.Add(ToLuaExport.type);
			}
			for (int i = 0; i < p.Length; i++)
			{
				if (!ToLuaExport.IsParams(p[i]))
				{
					if (p[i].Attributes != ParameterAttributes.Out)
					{
						list.Add(p[i].ParameterType);
					}
					else
					{
						list.Add(null);
					}
				}
			}
			for (int j = 0; j < list.Count - 1; j++)
			{
				stringBuilder.Append(ToLuaExport.GetTypeOf(list[j], ", "));
			}
			stringBuilder.Append(ToLuaExport.GetTypeOf(list[list.Count - 1], string.Empty));
			return stringBuilder.ToString();
		}

		private static void CheckObjectNull()
		{
			if (ToLuaExport.type.IsValueType)
			{
				ToLuaExport.sb.AppendLine("\t\tif (o == null)");
			}
			else
			{
				ToLuaExport.sb.AppendLine("\t\tif (obj == null)");
			}
		}

		private static void GenIndexFunc()
		{
			for (int i = 0; i < ToLuaExport.fields.Length; i++)
			{
				ToLuaExport.sb.AppendLine("\r\n\t[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]");
				ToLuaExport.sb.AppendFormat("\tstatic int get_{0}(IntPtr L)\r\n", ToLuaExport.fields[i].Name);
				ToLuaExport.sb.AppendLine("\t{");
				string pushFunction = ToLuaExport.GetPushFunction(ToLuaExport.fields[i].FieldType);
				if (ToLuaExport.fields[i].IsStatic)
				{
					ToLuaExport.sb.AppendFormat("\t\tLuaScriptMgr.{2}(L, {0}.{1});\r\n", ToLuaExport.className, ToLuaExport.fields[i].Name, pushFunction);
				}
				else
				{
					ToLuaExport.sb.AppendFormat("\t\tobject o = LuaScriptMgr.GetLuaObject(L, 1);\r\n", new object[0]);
					if (!ToLuaExport.type.IsValueType)
					{
						ToLuaExport.sb.AppendFormat("\t\t{0} obj = ({0})o;\r\n", ToLuaExport.className);
					}
					ToLuaExport.sb.AppendLine();
					ToLuaExport.CheckObjectNull();
					ToLuaExport.sb.AppendLine("\t\t{");
					ToLuaExport.sb.AppendLine("\t\t\tLuaTypes types = LuaDLL.lua_type(L, 1);");
					ToLuaExport.sb.AppendLine();
					ToLuaExport.sb.AppendLine("\t\t\tif (types == LuaTypes.LUA_TTABLE)");
					ToLuaExport.sb.AppendLine("\t\t\t{");
					ToLuaExport.sb.AppendFormat("\t\t\t\tLuaDLL.luaL_error(L, \"unknown member name {0}\");\r\n", ToLuaExport.fields[i].Name);
					ToLuaExport.sb.AppendLine("\t\t\t}");
					ToLuaExport.sb.AppendLine("\t\t\telse");
					ToLuaExport.sb.AppendLine("\t\t\t{");
					ToLuaExport.sb.AppendFormat("\t\t\t\tLuaDLL.luaL_error(L, \"attempt to index {0} on a nil value\");\r\n", ToLuaExport.fields[i].Name);
					ToLuaExport.sb.AppendLine("\t\t\t}");
					ToLuaExport.sb.AppendLine("\t\t}");
					ToLuaExport.sb.AppendLine();
					if (ToLuaExport.type.IsValueType)
					{
						ToLuaExport.sb.AppendFormat("\t\t{0} obj = ({0})o;\r\n", ToLuaExport.className);
					}
					ToLuaExport.sb.AppendFormat("\t\tLuaScriptMgr.{1}(L, obj.{0});\r\n", ToLuaExport.fields[i].Name, pushFunction);
				}
				ToLuaExport.sb.AppendLine("\t\treturn 1;");
				ToLuaExport.sb.AppendLine("\t}");
			}
			for (int j = 0; j < ToLuaExport.props.Length; j++)
			{
				if (ToLuaExport.props[j].CanRead)
				{
					bool flag = true;
					int num = ToLuaExport.propList.IndexOf(ToLuaExport.props[j]);
					if (num >= 0)
					{
						flag = false;
					}
					ToLuaExport.sb.AppendLine("\r\n\t[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]");
					ToLuaExport.sb.AppendFormat("\tstatic int get_{0}(IntPtr L)\r\n", ToLuaExport.props[j].Name);
					ToLuaExport.sb.AppendLine("\t{");
					string pushFunction2 = ToLuaExport.GetPushFunction(ToLuaExport.props[j].PropertyType);
					if (flag)
					{
						ToLuaExport.sb.AppendFormat("\t\tLuaScriptMgr.{2}(L, {0}.{1});\r\n", ToLuaExport.className, ToLuaExport.props[j].Name, pushFunction2);
					}
					else
					{
						ToLuaExport.sb.AppendFormat("\t\tobject o = LuaScriptMgr.GetLuaObject(L, 1);\r\n", new object[0]);
						if (!ToLuaExport.type.IsValueType)
						{
							ToLuaExport.sb.AppendFormat("\t\t{0} obj = ({0})o;\r\n", ToLuaExport.className);
						}
						ToLuaExport.sb.AppendLine();
						ToLuaExport.CheckObjectNull();
						ToLuaExport.sb.AppendLine("\t\t{");
						ToLuaExport.sb.AppendLine("\t\t\tLuaTypes types = LuaDLL.lua_type(L, 1);");
						ToLuaExport.sb.AppendLine();
						ToLuaExport.sb.AppendLine("\t\t\tif (types == LuaTypes.LUA_TTABLE)");
						ToLuaExport.sb.AppendLine("\t\t\t{");
						ToLuaExport.sb.AppendFormat("\t\t\t\tLuaDLL.luaL_error(L, \"unknown member name {0}\");\r\n", ToLuaExport.props[j].Name);
						ToLuaExport.sb.AppendLine("\t\t\t}");
						ToLuaExport.sb.AppendLine("\t\t\telse");
						ToLuaExport.sb.AppendLine("\t\t\t{");
						ToLuaExport.sb.AppendFormat("\t\t\t\tLuaDLL.luaL_error(L, \"attempt to index {0} on a nil value\");\r\n", ToLuaExport.props[j].Name);
						ToLuaExport.sb.AppendLine("\t\t\t}");
						ToLuaExport.sb.AppendLine("\t\t}");
						ToLuaExport.sb.AppendLine();
						if (ToLuaExport.type.IsValueType)
						{
							ToLuaExport.sb.AppendFormat("\t\t{0} obj = ({0})o;\r\n", ToLuaExport.className);
						}
						ToLuaExport.sb.AppendFormat("\t\tLuaScriptMgr.{1}(L, obj.{0});\r\n", ToLuaExport.props[j].Name, pushFunction2);
					}
					ToLuaExport.sb.AppendLine("\t\treturn 1;");
					ToLuaExport.sb.AppendLine("\t}");
				}
			}
		}

		private static void GenNewIndexFunc()
		{
			for (int i = 0; i < ToLuaExport.fields.Length; i++)
			{
				if (!ToLuaExport.fields[i].IsLiteral && !ToLuaExport.fields[i].IsInitOnly && !ToLuaExport.fields[i].IsPrivate)
				{
					ToLuaExport.sb.AppendLine("\r\n\t[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]");
					ToLuaExport.sb.AppendFormat("\tstatic int set_{0}(IntPtr L)\r\n", ToLuaExport.fields[i].Name);
					ToLuaExport.sb.AppendLine("\t{");
					string o = (!ToLuaExport.fields[i].IsStatic) ? "obj" : ToLuaExport.className;
					if (!ToLuaExport.fields[i].IsStatic)
					{
						ToLuaExport.sb.AppendFormat("\t\tobject o = LuaScriptMgr.GetLuaObject(L, 1);\r\n", new object[0]);
						if (!ToLuaExport.type.IsValueType)
						{
							ToLuaExport.sb.AppendFormat("\t\t{0} obj = ({0})o;\r\n", ToLuaExport.className);
						}
						ToLuaExport.sb.AppendLine();
						ToLuaExport.CheckObjectNull();
						ToLuaExport.sb.AppendLine("\t\t{");
						ToLuaExport.sb.AppendLine("\t\t\tLuaTypes types = LuaDLL.lua_type(L, 1);");
						ToLuaExport.sb.AppendLine();
						ToLuaExport.sb.AppendLine("\t\t\tif (types == LuaTypes.LUA_TTABLE)");
						ToLuaExport.sb.AppendLine("\t\t\t{");
						ToLuaExport.sb.AppendFormat("\t\t\t\tLuaDLL.luaL_error(L, \"unknown member name {0}\");\r\n", ToLuaExport.fields[i].Name);
						ToLuaExport.sb.AppendLine("\t\t\t}");
						ToLuaExport.sb.AppendLine("\t\t\telse");
						ToLuaExport.sb.AppendLine("\t\t\t{");
						ToLuaExport.sb.AppendFormat("\t\t\t\tLuaDLL.luaL_error(L, \"attempt to index {0} on a nil value\");\r\n", ToLuaExport.fields[i].Name);
						ToLuaExport.sb.AppendLine("\t\t\t}");
						ToLuaExport.sb.AppendLine("\t\t}");
						ToLuaExport.sb.AppendLine();
						if (ToLuaExport.type.IsValueType)
						{
							ToLuaExport.sb.AppendFormat("\t\t{0} obj = ({0})o;\r\n", ToLuaExport.className);
						}
					}
					ToLuaExport.NewIndexSetValue(ToLuaExport.fields[i].FieldType, o, ToLuaExport.fields[i].Name);
					if (!ToLuaExport.fields[i].IsStatic && ToLuaExport.type.IsValueType)
					{
						ToLuaExport.sb.AppendLine("\t\tLuaScriptMgr.SetValueObject(L, 1, obj);");
					}
					ToLuaExport.sb.AppendLine("\t\treturn 0;");
					ToLuaExport.sb.AppendLine("\t}");
				}
			}
			for (int j = 0; j < ToLuaExport.props.Length; j++)
			{
				if (ToLuaExport.props[j].CanWrite && ToLuaExport.props[j].GetSetMethod(true).IsPublic)
				{
					bool flag = true;
					int num = ToLuaExport.propList.IndexOf(ToLuaExport.props[j]);
					if (num >= 0)
					{
						flag = false;
					}
					ToLuaExport.sb.AppendLine("\r\n\t[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]");
					ToLuaExport.sb.AppendFormat("\tstatic int set_{0}(IntPtr L)\r\n", ToLuaExport.props[j].Name);
					ToLuaExport.sb.AppendLine("\t{");
					string o2 = (!flag) ? "obj" : ToLuaExport.className;
					if (!flag)
					{
						ToLuaExport.sb.AppendFormat("\t\tobject o = LuaScriptMgr.GetLuaObject(L, 1);\r\n", new object[0]);
						if (!ToLuaExport.type.IsValueType)
						{
							ToLuaExport.sb.AppendFormat("\t\t{0} obj = ({0})o;\r\n", ToLuaExport.className);
						}
						ToLuaExport.sb.AppendLine();
						ToLuaExport.CheckObjectNull();
						ToLuaExport.sb.AppendLine("\t\t{");
						ToLuaExport.sb.AppendLine("\t\t\tLuaTypes types = LuaDLL.lua_type(L, 1);");
						ToLuaExport.sb.AppendLine();
						ToLuaExport.sb.AppendLine("\t\t\tif (types == LuaTypes.LUA_TTABLE)");
						ToLuaExport.sb.AppendLine("\t\t\t{");
						ToLuaExport.sb.AppendFormat("\t\t\t\tLuaDLL.luaL_error(L, \"unknown member name {0}\");\r\n", ToLuaExport.props[j].Name);
						ToLuaExport.sb.AppendLine("\t\t\t}");
						ToLuaExport.sb.AppendLine("\t\t\telse");
						ToLuaExport.sb.AppendLine("\t\t\t{");
						ToLuaExport.sb.AppendFormat("\t\t\t\tLuaDLL.luaL_error(L, \"attempt to index {0} on a nil value\");\r\n", ToLuaExport.props[j].Name);
						ToLuaExport.sb.AppendLine("\t\t\t}");
						ToLuaExport.sb.AppendLine("\t\t}");
						ToLuaExport.sb.AppendLine();
						if (ToLuaExport.type.IsValueType)
						{
							ToLuaExport.sb.AppendFormat("\t\t{0} obj = ({0})o;\r\n", ToLuaExport.className);
						}
					}
					ToLuaExport.NewIndexSetValue(ToLuaExport.props[j].PropertyType, o2, ToLuaExport.props[j].Name);
					if (!flag && ToLuaExport.type.IsValueType)
					{
						ToLuaExport.sb.AppendLine("\t\tLuaScriptMgr.SetValueObject(L, 1, obj);");
					}
					ToLuaExport.sb.AppendLine("\t\treturn 0;");
					ToLuaExport.sb.AppendLine("\t}");
				}
			}
		}

		private static void GenDelegateBody(Type t, string head, bool haveState)
		{
			ToLuaExport.eventSet.Add(t);
			MethodInfo method = t.GetMethod("Invoke");
			ParameterInfo[] parameters = method.GetParameters();
			int num = parameters.Length;
			if (num == 0)
			{
				ToLuaExport.sb.AppendLine("() =>");
				if (method.ReturnType == typeof(void))
				{
					ToLuaExport.sb.AppendFormat("{0}{{\r\n{0}\tfunc.Call();\r\n{0}}};\r\n", head);
				}
				else
				{
					ToLuaExport.sb.AppendFormat("{0}{{\r\n{0}\tobject[] objs = func.Call();\r\n", head);
					ToLuaExport.sb.AppendFormat("{1}\treturn ({0})objs[0];\r\n", ToLuaExport.GetTypeStr(method.ReturnType), head);
					ToLuaExport.sb.AppendFormat("{0}}};\r\n", head);
				}
				return;
			}
			ToLuaExport.sb.AppendFormat("(param0", new object[0]);
			for (int i = 1; i < num; i++)
			{
				ToLuaExport.sb.AppendFormat(", param{0}", i);
			}
			ToLuaExport.sb.AppendFormat(") =>\r\n{0}{{\r\n{0}", head);
			ToLuaExport.sb.AppendLine("\tint top = func.BeginPCall();");
			if (!haveState)
			{
				ToLuaExport.sb.AppendFormat("{0}\tIntPtr L = func.GetLuaState();\r\n", head);
			}
			for (int j = 0; j < num; j++)
			{
				string pushFunction = ToLuaExport.GetPushFunction(parameters[j].ParameterType);
				ToLuaExport.sb.AppendFormat("{2}\tLuaScriptMgr.{0}(L, param{1});\r\n", pushFunction, j, head);
			}
			ToLuaExport.sb.AppendFormat("{1}\tfunc.PCall(top, {0});\r\n", num, head);
			if (method.ReturnType == typeof(void))
			{
				ToLuaExport.sb.AppendFormat("{0}\tfunc.EndPCall(top);\r\n", head);
			}
			else
			{
				ToLuaExport.sb.AppendFormat("{0}\tobject[] objs = func.PopValues(top);\r\n", head);
				ToLuaExport.sb.AppendFormat("{0}\tfunc.EndPCall(top);\r\n", head);
				ToLuaExport.sb.AppendFormat("{1}\treturn ({0})objs[0];\r\n", ToLuaExport.GetTypeStr(method.ReturnType), head);
			}
			ToLuaExport.sb.AppendFormat("{0}}};\r\n", head);
		}

		private static void NewIndexSetValue(Type t, string o, string name)
		{
			if (t.IsArray)
			{
				Type elementType = t.GetElementType();
				string typeStr = ToLuaExport.GetTypeStr(elementType);
				if (elementType == typeof(bool))
				{
					ToLuaExport.sb.AppendFormat("\t\t{0}.{1} = LuaScriptMgr.GetArrayBool(L, 3);\r\n", o, name);
				}
				else if (elementType.IsPrimitive)
				{
					ToLuaExport.sb.AppendFormat("\t\t{0}.{1} = LuaScriptMgr.GetArrayNumber<{2}>(L, 3);\r\n", o, name, typeStr);
				}
				else if (elementType == typeof(string))
				{
					ToLuaExport.sb.AppendFormat("\t\t{0}.{1} = LuaScriptMgr.GetArrayString(L, 3);\r\n", o, name);
				}
				else
				{
					if (elementType == typeof(UnityEngine.Object))
					{
						ToLuaExport.ambig |= ObjAmbig.U3dObj;
					}
					ToLuaExport.sb.AppendFormat("\t\t{0}.{1} = LuaScriptMgr.GetArrayObject<{2}>(L, 3);\r\n", o, name, typeStr);
				}
				return;
			}
			if (t == typeof(bool))
			{
				ToLuaExport.sb.AppendFormat("\t\t{0}.{1} = LuaScriptMgr.GetBoolean(L, 3);\r\n", o, name);
			}
			else if (t == typeof(string))
			{
				ToLuaExport.sb.AppendFormat("\t\t{0}.{1} = LuaScriptMgr.GetString(L, 3);\r\n", o, name);
			}
			else if (t.IsPrimitive)
			{
				ToLuaExport.sb.AppendFormat("\t\t{0}.{1} = ({2})LuaScriptMgr.GetNumber(L, 3);\r\n", o, name, ToLuaExport._C(t.ToString()));
			}
			else if (t == typeof(LuaFunction))
			{
				ToLuaExport.sb.AppendFormat("\t\t{0}.{1} = LuaScriptMgr.GetLuaFunction(L, 3);\r\n", o, name);
			}
			else if (t == typeof(LuaTable))
			{
				ToLuaExport.sb.AppendFormat("\t\t{0}.{1} = LuaScriptMgr.GetLuaTable(L, 3);\r\n", o, name);
			}
			else if (t == typeof(object))
			{
				ToLuaExport.sb.AppendFormat("\t\t{0}.{1} = LuaScriptMgr.GetVarObject(L, 3);\r\n", o, name);
			}
			else if (t == typeof(Vector3))
			{
				ToLuaExport.sb.AppendFormat("\t\t{0}.{1} = LuaScriptMgr.GetVector3(L, 3);\r\n", o, name);
			}
			else if (t == typeof(Quaternion))
			{
				ToLuaExport.sb.AppendFormat("\t\t{0}.{1} = LuaScriptMgr.GetQuaternion(L, 3);\r\n", o, name);
			}
			else if (t == typeof(Vector2))
			{
				ToLuaExport.sb.AppendFormat("\t\t{0}.{1} = LuaScriptMgr.GetVector2(L, 3);\r\n", o, name);
			}
			else if (t == typeof(Vector4))
			{
				ToLuaExport.sb.AppendFormat("\t\t{0}.{1} = LuaScriptMgr.GetVector4(L, 3);\r\n", o, name);
			}
			else if (t == typeof(Color))
			{
				ToLuaExport.sb.AppendFormat("\t\t{0}.{1} = LuaScriptMgr.GetColor(L, 3);\r\n", o, name);
			}
			else if (t == typeof(Ray))
			{
				ToLuaExport.sb.AppendFormat("\t\t{0}.{1} = LuaScriptMgr.GetRay(L, 3);\r\n", o, name);
			}
			else if (t == typeof(Bounds))
			{
				ToLuaExport.sb.AppendFormat("\t\t{0}.{1} = LuaScriptMgr.GetBounds(L, 3);\r\n", o, name);
			}
			else if (t == typeof(LuaStringBuffer))
			{
				ToLuaExport.sb.AppendFormat("\t\t{0}.{1} = LuaScriptMgr.GetStringBuffer(L, 3);\r\n", o, name);
			}
			else if (typeof(TrackedReference).IsAssignableFrom(t))
			{
				ToLuaExport.sb.AppendFormat("\t\t{0}.{1} = ({2})LuaScriptMgr.GetTrackedObject(L, 3, typeof(2));\r\n", o, name, ToLuaExport.GetTypeStr(t));
			}
			else if (typeof(UnityEngine.Object).IsAssignableFrom(t))
			{
				ToLuaExport.sb.AppendFormat("\t\t{0}.{1} = ({2})LuaScriptMgr.GetUnityObject(L, 3, typeof({2}));\r\n", o, name, ToLuaExport.GetTypeStr(t));
			}
			else if (typeof(Delegate).IsAssignableFrom(t))
			{
				ToLuaExport.sb.AppendLine("\t\tLuaTypes funcType = LuaDLL.lua_type(L, 3);\r\n");
				ToLuaExport.sb.AppendLine("\t\tif (funcType != LuaTypes.LUA_TFUNCTION)");
				ToLuaExport.sb.AppendLine("\t\t{");
				ToLuaExport.sb.AppendFormat("\t\t\t{0}.{1} = ({2})LuaScriptMgr.GetNetObject(L, 3, typeof({2}));\r\n", o, name, ToLuaExport.GetTypeStr(t));
				ToLuaExport.sb.AppendLine("\t\t}\r\n\t\telse");
				ToLuaExport.sb.AppendLine("\t\t{");
				ToLuaExport.sb.AppendLine("\t\t\tLuaFunction func = LuaScriptMgr.ToLuaFunction(L, 3);");
				ToLuaExport.sb.AppendFormat("\t\t\t{0}.{1} = ", o, name);
				ToLuaExport.GenDelegateBody(t, "\t\t\t", true);
				ToLuaExport.sb.AppendLine("\t\t}");
			}
			else if (typeof(object).IsAssignableFrom(t) || t.IsEnum)
			{
				ToLuaExport.sb.AppendFormat("\t\t{0}.{1} = ({2})LuaScriptMgr.GetNetObject(L, 3, typeof({2}));\r\n", o, name, ToLuaExport.GetTypeStr(t));
			}
			else if (t == typeof(Type))
			{
				ToLuaExport.sb.AppendFormat("\t\t{0}.{1} = LuaScriptMgr.GetTypeObject(L, 3);\r\n", o, name);
			}
			else
			{
				Debug.LogError("not defined type " + t.ToString());
			}
		}

		private static void GenToStringFunc()
		{
			int num = Array.FindIndex<MethodInfo>(ToLuaExport.methods, (MethodInfo p) => p.Name == "ToString");
			if (num < 0 || ToLuaExport.isStaticClass)
			{
				return;
			}
			ToLuaExport.sb.AppendLine("\r\n\t[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]");
			ToLuaExport.sb.AppendLine("\tstatic int Lua_ToString(IntPtr L)");
			ToLuaExport.sb.AppendLine("\t{");
			ToLuaExport.sb.AppendLine("\t\tobject obj = LuaScriptMgr.GetLuaObject(L, 1);\r\n");
			ToLuaExport.sb.AppendLine("\t\tif (obj != null)");
			ToLuaExport.sb.AppendLine("\t\t{");
			ToLuaExport.sb.AppendLine("\t\t\tLuaScriptMgr.Push(L, obj.ToString());");
			ToLuaExport.sb.AppendLine("\t\t}");
			ToLuaExport.sb.AppendLine("\t\telse");
			ToLuaExport.sb.AppendLine("\t\t{");
			ToLuaExport.sb.AppendFormat("\t\t\tLuaScriptMgr.Push(L, \"Table: {0}\");\r\n", ToLuaExport.libClassName);
			ToLuaExport.sb.AppendLine("\t\t}");
			ToLuaExport.sb.AppendLine();
			ToLuaExport.sb.AppendLine("\t\treturn 1;");
			ToLuaExport.sb.AppendLine("\t}");
		}

		private static bool IsNeedOp(string name)
		{
			if (name == "op_Addition")
			{
				ToLuaExport.op |= MetaOp.Add;
			}
			else if (name == "op_Subtraction")
			{
				ToLuaExport.op |= MetaOp.Sub;
			}
			else if (name == "op_Equality")
			{
				ToLuaExport.op |= MetaOp.Eq;
			}
			else if (name == "op_Multiply")
			{
				ToLuaExport.op |= MetaOp.Mul;
			}
			else if (name == "op_Division")
			{
				ToLuaExport.op |= MetaOp.Div;
			}
			else
			{
				if (!(name == "op_UnaryNegation"))
				{
					return false;
				}
				ToLuaExport.op |= MetaOp.Neg;
			}
			return true;
		}

		private static void CallOpFunction(string name, int count, string ret)
		{
			string text = string.Empty;
			for (int i = 0; i < count; i++)
			{
				text += "\t";
			}
			if (name == "op_Addition")
			{
				ToLuaExport.sb.AppendFormat("{0}{1} o = arg0 + arg1;\r\n", text, ret);
			}
			else if (name == "op_Subtraction")
			{
				ToLuaExport.sb.AppendFormat("{0}{1} o = arg0 - arg1;\r\n", text, ret);
			}
			else if (name == "op_Equality")
			{
				ToLuaExport.sb.AppendFormat("{0}bool o = arg0 == arg1;\r\n", text);
			}
			else if (name == "op_Multiply")
			{
				ToLuaExport.sb.AppendFormat("{0}{1} o = arg0 * arg1;\r\n", text, ret);
			}
			else if (name == "op_Division")
			{
				ToLuaExport.sb.AppendFormat("{0}{1} o = arg0 / arg1;\r\n", text, ret);
			}
			else if (name == "op_UnaryNegation")
			{
				ToLuaExport.sb.AppendFormat("{0}{1} o = -arg0;\r\n", text, ret);
			}
		}

		private static string GetFuncName(string name)
		{
			if (name == "op_Addition")
			{
				return "Lua_Add";
			}
			if (name == "op_Subtraction")
			{
				return "Lua_Sub";
			}
			if (name == "op_Equality")
			{
				return "Lua_Eq";
			}
			if (name == "op_Multiply")
			{
				return "Lua_Mul";
			}
			if (name == "op_Division")
			{
				return "Lua_Div";
			}
			if (name == "op_UnaryNegation")
			{
				return "Lua_Neg";
			}
			return name;
		}

		public static bool IsObsolete(MemberInfo mb)
		{
			object[] customAttributes = mb.GetCustomAttributes(true);
			for (int i = 0; i < customAttributes.Length; i++)
			{
				Type type = customAttributes[i].GetType();
				if (type == typeof(ObsoleteAttribute) || type == typeof(NoToLuaAttribute))
				{
					return true;
				}
			}
			return ToLuaExport.IsMemberFilter(mb);
		}

		public static bool HasAttribute(MemberInfo mb, Type atrtype)
		{
			object[] customAttributes = mb.GetCustomAttributes(true);
			for (int i = 0; i < customAttributes.Length; i++)
			{
				Type type = customAttributes[i].GetType();
				if (type == atrtype)
				{
					return true;
				}
			}
			return false;
		}

		private static void GenEnum()
		{
			ToLuaExport.fields = ToLuaExport.type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.GetField);
			List<FieldInfo> list = new List<FieldInfo>(ToLuaExport.fields);
			for (int i = list.Count - 1; i > 0; i--)
			{
				if (ToLuaExport.IsObsolete(list[i]))
				{
					list.RemoveAt(i);
				}
			}
			ToLuaExport.fields = list.ToArray();
			ToLuaExport.sb.AppendFormat("public class {0}Wrap\r\n", ToLuaExport.wrapClassName);
			ToLuaExport.sb.AppendLine("{");
			ToLuaExport.sb.AppendLine("\tstatic LuaMethod[] enums = new LuaMethod[]");
			ToLuaExport.sb.AppendLine("\t{");
			for (int j = 0; j < ToLuaExport.fields.Length; j++)
			{
				ToLuaExport.sb.AppendFormat("\t\tnew LuaMethod(\"{0}\", Get{0}),\r\n", ToLuaExport.fields[j].Name);
			}
			ToLuaExport.sb.AppendFormat("\t\tnew LuaMethod(\"IntToEnum\", IntToEnum),\r\n", new object[0]);
			ToLuaExport.sb.AppendLine("\t};");
			ToLuaExport.sb.AppendLine("\r\n\tpublic static void Register(IntPtr L)");
			ToLuaExport.sb.AppendLine("\t{");
			ToLuaExport.sb.AppendFormat("\t\tLuaScriptMgr.RegisterLib(L, \"{0}\", typeof({0}), enums);\r\n", ToLuaExport.libClassName);
			ToLuaExport.sb.AppendLine("\t}");
			for (int k = 0; k < ToLuaExport.fields.Length; k++)
			{
				ToLuaExport.sb.AppendLine("\r\n\t[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]");
				ToLuaExport.sb.AppendFormat("\tstatic int Get{0}(IntPtr L)\r\n", ToLuaExport.fields[k].Name);
				ToLuaExport.sb.AppendLine("\t{");
				ToLuaExport.sb.AppendFormat("\t\tLuaScriptMgr.Push(L, {0}.{1});\r\n", ToLuaExport.className, ToLuaExport.fields[k].Name);
				ToLuaExport.sb.AppendLine("\t\treturn 1;");
				ToLuaExport.sb.AppendLine("\t}");
			}
		}

		private static void GenEnumTranslator()
		{
			ToLuaExport.sb.AppendLine("\r\n\t[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]");
			ToLuaExport.sb.AppendLine("\tstatic int IntToEnum(IntPtr L)");
			ToLuaExport.sb.AppendLine("\t{");
			ToLuaExport.sb.AppendLine("\t\tint arg0 = (int)LuaDLL.lua_tonumber(L, 1);");
			ToLuaExport.sb.AppendFormat("\t\t{0} o = ({0})arg0;\r\n", ToLuaExport.className);
			ToLuaExport.sb.AppendLine("\t\tLuaScriptMgr.Push(L, o);");
			ToLuaExport.sb.AppendLine("\t\treturn 1;");
			ToLuaExport.sb.AppendLine("\t}");
		}

		public static void GenDelegates(DelegateType[] list)
		{
			ToLuaExport.usingList.Add("System");
			ToLuaExport.usingList.Add("System.Collections.Generic");
			for (int i = 0; i < list.Length; i++)
			{
				Type type = list[i].type;
				if (!typeof(Delegate).IsAssignableFrom(type))
				{
					Debug.LogError(type.FullName + " not a delegate type");
					return;
				}
			}
			ToLuaExport.sb.AppendLine("namespace com.tencent.pandora");
			ToLuaExport.sb.AppendLine("{");
			ToLuaExport.sb.AppendLine("\tpublic static class DelegateFactory");
			ToLuaExport.sb.AppendLine("\t{");
			ToLuaExport.sb.AppendLine("\t\tdelegate Delegate DelegateValue(LuaFunction func);");
			ToLuaExport.sb.AppendLine("\t\tstatic Dictionary<Type, DelegateValue> dict = new Dictionary<Type, DelegateValue>();");
			ToLuaExport.sb.AppendLine();
			ToLuaExport.sb.AppendLine("\t\t[NoToLuaAttribute]");
			ToLuaExport.sb.AppendLine("\t\tpublic static void Register(IntPtr L)");
			ToLuaExport.sb.AppendLine("\t\t{");
			for (int j = 0; j < list.Length; j++)
			{
				string strType = list[j].strType;
				string name = list[j].name;
				ToLuaExport.sb.AppendFormat("\t\t\tdict.Add(typeof({0}), new DelegateValue({1}));\r\n", strType, name);
			}
			ToLuaExport.sb.AppendLine("\t\t}\r\n");
			ToLuaExport.sb.AppendLine("\t\t[NoToLuaAttribute]");
			ToLuaExport.sb.AppendLine("\t\tpublic static Delegate CreateDelegate(Type t, LuaFunction func)");
			ToLuaExport.sb.AppendLine("\t\t{");
			ToLuaExport.sb.AppendLine("\t\t\tDelegateValue create = null;\r\n");
			ToLuaExport.sb.AppendLine("\t\t\tif (!dict.TryGetValue(t, out create))");
			ToLuaExport.sb.AppendLine("\t\t\t{");
			ToLuaExport.sb.AppendLine("\t\t\t\tUnityEngine.Debug.LogError(\"Delegate \" + t.FullName + \" not register\");");
			ToLuaExport.sb.AppendLine("\t\t\t\treturn null;");
			ToLuaExport.sb.AppendLine("\t\t\t}");
			ToLuaExport.sb.AppendLine("\t\t\treturn create(func);");
			ToLuaExport.sb.AppendLine("\t\t}\r\n");
			for (int k = 0; k < list.Length; k++)
			{
				Type t = list[k].type;
				string strType2 = list[k].strType;
				string name2 = list[k].name;
				ToLuaExport.sb.AppendFormat("\t\tpublic static Delegate {0}(LuaFunction func)\r\n", name2);
				ToLuaExport.sb.AppendLine("\t\t{");
				ToLuaExport.sb.AppendFormat("\t\t\t{0} d = ", strType2);
				ToLuaExport.GenDelegateBody(t, "\t\t\t", false);
				ToLuaExport.sb.AppendLine("\t\t\treturn d;");
				ToLuaExport.sb.AppendLine("\t\t}\r\n");
			}
			ToLuaExport.sb.AppendLine("\t\tpublic static void Clear()");
			ToLuaExport.sb.AppendLine("\t\t{");
			ToLuaExport.sb.AppendLine("\t\t\tdict.Clear();");
			ToLuaExport.sb.AppendLine("\t\t}\r\n");
			ToLuaExport.sb.AppendLine("\t}");
			ToLuaExport.sb.AppendLine("}");
			ToLuaExport.SaveFile(Application.dataPath + "/Scripts/Pandora/uLua/Source/Base/DelegateFactory.cs");
			ToLuaExport.Clear();
		}

		private static string[] GetGenericLibName(Type[] types)
		{
			string[] array = new string[types.Length];
			for (int i = 0; i < types.Length; i++)
			{
				Type type = types[i];
				if (type.IsGenericType)
				{
					array[i] = ToLuaExport.GetGenericLibName(types[i]);
				}
				else if (type.IsArray)
				{
					type = type.GetElementType();
					array[i] = ToLuaExport._C(type.ToString()) + "s";
				}
				else
				{
					array[i] = ToLuaExport._C(type.ToString());
				}
			}
			return array;
		}

		public static string GetGenericLibName(Type type)
		{
			Type[] genericArguments = type.GetGenericArguments();
			string name = type.Name;
			int num = name.IndexOf('`');
			string text = (num == -1) ? name : name.Substring(0, num);
			text = ToLuaExport._C(text);
			return text + "_" + string.Join("_", ToLuaExport.GetGenericLibName(genericArguments));
		}

		
		private sealed class _ProcessExtends_c__AnonStorey9F
		{
			internal List<MethodInfo> list2;
		}

		private static void ProcessExtends(List<MethodInfo> list)
		{
			ToLuaExport.extendName = "ToLua_" + ToLuaExport.libClassName.Replace(".", "_");
			ToLuaExport.extendType = Type.GetType(ToLuaExport.extendName + ", Assembly-CSharp-Editor");
			if (ToLuaExport.extendType != null)
			{
				ToLuaExport._ProcessExtends_c__AnonStorey9F _ProcessExtends_c__AnonStorey9F = new ToLuaExport._ProcessExtends_c__AnonStorey9F();
				_ProcessExtends_c__AnonStorey9F.list2 = new List<MethodInfo>();
				_ProcessExtends_c__AnonStorey9F.list2.AddRange(ToLuaExport.extendType.GetMethods(BindingFlags.Instance | ToLuaExport.binding | BindingFlags.DeclaredOnly));
				int i;
				for (i = _ProcessExtends_c__AnonStorey9F.list2.Count - 1; i >= 0; i--)
				{
					if ((!_ProcessExtends_c__AnonStorey9F.list2[i].Name.Contains("op_") && !_ProcessExtends_c__AnonStorey9F.list2[i].Name.Contains("add_") && !_ProcessExtends_c__AnonStorey9F.list2[i].Name.Contains("remove_")) || ToLuaExport.IsNeedOp(_ProcessExtends_c__AnonStorey9F.list2[i].Name))
					{
						list.RemoveAll((MethodInfo md) => md.Name == _ProcessExtends_c__AnonStorey9F.list2[i].Name);
						if (!ToLuaExport.IsObsolete(_ProcessExtends_c__AnonStorey9F.list2[i]))
						{
							list.Add(_ProcessExtends_c__AnonStorey9F.list2[i]);
						}
					}
				}
			}
		}
	}
}
