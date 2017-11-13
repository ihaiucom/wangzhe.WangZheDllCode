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
		private sealed class _Generate_c__AnonStorey97
		{
			internal PropertyInfo[] ps;
		}

		private sealed class _GenConstruct_c__AnonStorey9A
		{
			internal List<ConstructorInfo> list;
		}

		private sealed class _GenOverrideFunc_c__AnonStorey9D
		{
			internal List<MethodInfo> list;
		}

		private sealed class _ProcessExtends_c__AnonStorey9F
		{
			internal List<MethodInfo> list2;
		}

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
			ToLuaExport.binding = 25;
			ToLuaExport.ambig = ObjAmbig.NetObj;
			ToLuaExport.wrapClassName = string.Empty;
			ToLuaExport.libClassName = string.Empty;
			ToLuaExport.extendName = string.Empty;
			ToLuaExport.extendType = null;
			ToLuaExport.eventSet = new HashSet<Type>();
			List<string> list = new List<string>();
			list.Add("AnimationClip.averageDuration");
			list.Add("AnimationClip.averageAngularSpeed");
			list.Add("AnimationClip.averageSpeed");
			list.Add("AnimationClip.apparentSpeed");
			list.Add("AnimationClip.isLooping");
			list.Add("AnimationClip.isAnimatorMotion");
			list.Add("AnimationClip.isHumanMotion");
			list.Add("AnimatorOverrideController.PerformOverrideClipListCleanup");
			list.Add("Caching.SetNoBackupFlag");
			list.Add("Caching.ResetNoBackupFlag");
			list.Add("Light.areaSize");
			list.Add("Security.GetChainOfTrustValue");
			list.Add("Texture2D.alphaIsTransparency");
			list.Add("WWW.movie");
			list.Add("WebCamTexture.MarkNonReadable");
			list.Add("WebCamTexture.isReadable");
			list.Add("Graphic.OnRebuildRequested");
			list.Add("Text.OnRebuildRequested");
			list.Add("UIInput.ProcessEvent");
			list.Add("UIWidget.showHandlesWithMoveTool");
			list.Add("UIWidget.showHandles");
			list.Add("Application.ExternalEval");
			list.Add("Resources.LoadAssetAtPath");
			list.Add("Input.IsJoystickPreconfigured");
			list.Add("String.Chars");
			ToLuaExport.memberFilter = list;
			Dictionary<Type, int> dictionary = new Dictionary<Type, int>();
			dictionary.Add(typeof(bool), 1);
			dictionary.Add(typeof(char), 2);
			dictionary.Add(typeof(byte), 3);
			dictionary.Add(typeof(sbyte), 4);
			dictionary.Add(typeof(ushort), 5);
			dictionary.Add(typeof(short), 6);
			dictionary.Add(typeof(uint), 7);
			dictionary.Add(typeof(int), 8);
			dictionary.Add(typeof(float), 9);
			dictionary.Add(typeof(ulong), 10);
			dictionary.Add(typeof(long), 11);
			dictionary.Add(typeof(double), 12);
			ToLuaExport.typeSize = dictionary;
		}

		public static bool IsMemberFilter(MemberInfo mi)
		{
			return ToLuaExport.memberFilter.Contains(ToLuaExport.type.get_Name() + "." + mi.get_Name());
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
			for (Type baseType = ToLuaExport.type.get_BaseType(); baseType != null; baseType = baseType.get_BaseType())
			{
				MethodInfo[] array = baseType.GetMethods(29);
				for (int i = 0; i < array.Length; i++)
				{
					MetaOp metaOp = ToLuaExport.GetOp(array[i].get_Name());
					if (metaOp != MetaOp.None && (ToLuaExport.op & metaOp) == MetaOp.None)
					{
						list.Add(array[i]);
						ToLuaExport.op |= metaOp;
					}
				}
			}
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
			if (ToLuaExport.type.get_IsEnum())
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
				ToLuaExport.binding |= 2;
			}
			else if (ToLuaExport.baseClassName == null && ToLuaExport.isStaticClass)
			{
				ToLuaExport.binding |= 2;
			}
			if (ToLuaExport.type.get_IsInterface())
			{
				list.AddRange(ToLuaExport.type.GetMethods());
			}
			else
			{
				list.AddRange(ToLuaExport.type.GetMethods(4 | ToLuaExport.binding));
				for (int j = list.get_Count() - 1; j >= 0; j--)
				{
					if (list.get_Item(j).get_Name().Contains("op_") || list.get_Item(j).get_Name().Contains("add_") || list.get_Item(j).get_Name().Contains("remove_"))
					{
						if (!ToLuaExport.IsNeedOp(list.get_Item(j).get_Name()))
						{
							list.RemoveAt(j);
						}
					}
					else if (ToLuaExport.IsObsolete(list.get_Item(j)))
					{
						list.RemoveAt(j);
					}
				}
			}
			_Generate_c__AnonStorey.ps = ToLuaExport.type.GetProperties();
			int i;
			for (i = 0; i < _Generate_c__AnonStorey.ps.Length; i++)
			{
				int num = list.FindIndex((MethodInfo m) => m.get_Name() == "get_" + _Generate_c__AnonStorey.ps[i].get_Name());
				if (num >= 0 && list.get_Item(num).get_Name() != "get_Item")
				{
					list.RemoveAt(num);
				}
				num = list.FindIndex((MethodInfo m) => m.get_Name() == "set_" + _Generate_c__AnonStorey.ps[i].get_Name());
				if (num >= 0 && list.get_Item(num).get_Name() != "set_Item")
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
			using (StreamWriter streamWriter = new StreamWriter(file, false, Encoding.get_UTF8()))
			{
				StringBuilder stringBuilder = new StringBuilder();
				using (HashSet<string>.Enumerator enumerator = ToLuaExport.usingList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string current = enumerator.get_Current();
						stringBuilder.AppendFormat("using {0};\r\n", current);
					}
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
			ToLuaExport.fields = ToLuaExport.type.GetFields(3076 | ToLuaExport.binding);
			ToLuaExport.props = ToLuaExport.type.GetProperties(12292 | ToLuaExport.binding);
			ToLuaExport.propList.AddRange(ToLuaExport.type.GetProperties(12309));
			List<FieldInfo> list = new List<FieldInfo>();
			list.AddRange(ToLuaExport.fields);
			for (int i = list.get_Count() - 1; i >= 0; i--)
			{
				if (ToLuaExport.IsObsolete(list.get_Item(i)))
				{
					list.RemoveAt(i);
				}
			}
			ToLuaExport.fields = list.ToArray();
			List<PropertyInfo> list2 = new List<PropertyInfo>();
			list2.AddRange(ToLuaExport.props);
			for (int j = list2.get_Count() - 1; j >= 0; j--)
			{
				if (list2.get_Item(j).get_Name() == "Item" || ToLuaExport.IsObsolete(list2.get_Item(j)))
				{
					list2.RemoveAt(j);
				}
			}
			ToLuaExport.props = list2.ToArray();
			for (int k = ToLuaExport.propList.get_Count() - 1; k >= 0; k--)
			{
				if (ToLuaExport.propList.get_Item(k).get_Name() == "Item" || ToLuaExport.IsObsolete(ToLuaExport.propList.get_Item(k)))
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
				if (ToLuaExport.fields[l].get_IsLiteral() || ToLuaExport.fields[l].get_IsPrivate() || ToLuaExport.fields[l].get_IsInitOnly())
				{
					ToLuaExport.sb.AppendFormat("\t\t\tnew LuaField(\"{0}\", get_{0}, null),\r\n", ToLuaExport.fields[l].get_Name());
				}
				else
				{
					ToLuaExport.sb.AppendFormat("\t\t\tnew LuaField(\"{0}\", get_{0}, set_{0}),\r\n", ToLuaExport.fields[l].get_Name());
				}
			}
			for (int m = 0; m < ToLuaExport.props.Length; m++)
			{
				if (ToLuaExport.props[m].get_CanRead() && ToLuaExport.props[m].get_CanWrite() && ToLuaExport.props[m].GetSetMethod(true).get_IsPublic())
				{
					ToLuaExport.sb.AppendFormat("\t\t\tnew LuaField(\"{0}\", get_{0}, set_{0}),\r\n", ToLuaExport.props[m].get_Name());
				}
				else if (ToLuaExport.props[m].get_CanRead())
				{
					ToLuaExport.sb.AppendFormat("\t\t\tnew LuaField(\"{0}\", get_{0}, null),\r\n", ToLuaExport.props[m].get_Name());
				}
				else if (ToLuaExport.props[m].get_CanWrite())
				{
					ToLuaExport.sb.AppendFormat("\t\t\tnew LuaField(\"{0}\", null, set_{0}),\r\n", ToLuaExport.props[m].get_Name());
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
				if (!methodInfo.get_IsGenericMethod())
				{
					if (!ToLuaExport.nameCounter.TryGetValue(methodInfo.get_Name(), ref num))
					{
						if (!methodInfo.get_Name().Contains("op_"))
						{
							ToLuaExport.sb.AppendFormat("\t\t\tnew LuaMethod(\"{0}\", {0}),\r\n", methodInfo.get_Name());
						}
						ToLuaExport.nameCounter.set_Item(methodInfo.get_Name(), 1);
					}
					else
					{
						ToLuaExport.nameCounter.set_Item(methodInfo.get_Name(), num + 1);
					}
				}
			}
			ToLuaExport.sb.AppendFormat("\t\t\tnew LuaMethod(\"New\", _Create{0}),\r\n", ToLuaExport.wrapClassName);
			ToLuaExport.sb.AppendLine("\t\t\tnew LuaMethod(\"GetClassType\", GetClassType),");
			int num2 = Array.FindIndex<MethodInfo>(ToLuaExport.methods, (MethodInfo p) => p.get_Name() == "ToString");
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
				if (methodInfo.get_IsGenericMethod())
				{
					Debug.Log("Generic Method " + methodInfo.get_Name() + " cannot be export to lua");
				}
				else
				{
					if (ToLuaExport.nameCounter.get_Item(methodInfo.get_Name()) > 1)
					{
						if (hashSet.Contains(methodInfo.get_Name()))
						{
							goto IL_238;
						}
						MethodInfo methodInfo2 = ToLuaExport.GenOverrideFunc(methodInfo.get_Name());
						if (methodInfo2 == null)
						{
							hashSet.Add(methodInfo.get_Name());
							goto IL_238;
						}
						methodInfo = methodInfo2;
					}
					hashSet.Add(methodInfo.get_Name());
					ToLuaExport.sb.AppendLine("\r\n\t[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]");
					ToLuaExport.sb.AppendFormat("\tstatic int {0}(IntPtr L)\r\n", ToLuaExport.GetFuncName(methodInfo.get_Name()));
					ToLuaExport.sb.AppendLine("\t{");
					if (ToLuaExport.HasAttribute(methodInfo, typeof(OnlyGCAttribute)))
					{
						ToLuaExport.sb.AppendLine("\t\tLuaScriptMgr.__gc(L);");
						ToLuaExport.sb.AppendLine("\t\treturn 0;");
						ToLuaExport.sb.AppendLine("\t}");
					}
					else if (ToLuaExport.HasAttribute(methodInfo, typeof(UseDefinedAttribute)))
					{
						FieldInfo field = ToLuaExport.extendType.GetField(methodInfo.get_Name() + "Defined");
						string text = field.GetValue(null) as string;
						ToLuaExport.sb.AppendLine(text);
						ToLuaExport.sb.AppendLine("\t}");
					}
					else
					{
						ParameterInfo[] parameters = methodInfo.GetParameters();
						int num = methodInfo.get_IsStatic() ? 1 : 2;
						if (!ToLuaExport.HasOptionalParam(parameters))
						{
							int num2 = parameters.Length + num - 1;
							ToLuaExport.sb.AppendFormat("\t\tLuaScriptMgr.CheckArgsCount(L, {0});\r\n", num2);
						}
						else
						{
							ToLuaExport.sb.AppendLine("\t\tint count = LuaDLL.lua_gettop(L);");
						}
						int num3 = (methodInfo.get_ReturnType() == typeof(void)) ? 0 : 1;
						num3 += ToLuaExport.ProcessParams(methodInfo, 2, false, false, false);
						ToLuaExport.sb.AppendFormat("\t\treturn {0};\r\n", num3);
						ToLuaExport.sb.AppendLine("\t}");
					}
				}
				IL_238:;
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
			if (t.get_IsEnum())
			{
				return "Push";
			}
			if (t == typeof(bool) || t.get_IsPrimitive() || t == typeof(string) || t == typeof(LuaTable) || t == typeof(LuaCSFunction) || t == typeof(LuaFunction) || typeof(Object).IsAssignableFrom(t) || t == typeof(Type) || t == typeof(IntPtr) || typeof(Delegate).IsAssignableFrom(t) || t == typeof(LuaStringBuffer) || typeof(TrackedReference).IsAssignableFrom(t) || typeof(IEnumerator).IsAssignableFrom(t))
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
			if (t.get_IsValueType())
			{
				return "PushValue";
			}
			if (t.get_IsArray())
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

		private static void GenConstruct()
		{
			ToLuaExport._GenConstruct_c__AnonStorey9A _GenConstruct_c__AnonStorey9A = new ToLuaExport._GenConstruct_c__AnonStorey9A();
			if (ToLuaExport.isStaticClass || typeof(MonoBehaviour).IsAssignableFrom(ToLuaExport.type))
			{
				ToLuaExport.NoConsturct();
				return;
			}
			ConstructorInfo[] constructors = ToLuaExport.type.GetConstructors(4 | ToLuaExport.binding);
			if (ToLuaExport.extendType != null)
			{
				ConstructorInfo[] constructors2 = ToLuaExport.extendType.GetConstructors(4 | ToLuaExport.binding);
				if (constructors2 != null && constructors2.Length > 0 && ToLuaExport.HasAttribute(constructors2[0], typeof(UseDefinedAttribute)))
				{
					ToLuaExport.sb.AppendLine("\r\n\t[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]");
					ToLuaExport.sb.AppendFormat("\tstatic int _Create{0}(IntPtr L)\r\n", ToLuaExport.wrapClassName);
					ToLuaExport.sb.AppendLine("\t{");
					if (ToLuaExport.HasAttribute(constructors2[0], typeof(UseDefinedAttribute)))
					{
						FieldInfo field = ToLuaExport.extendType.GetField(ToLuaExport.extendName + "Defined");
						string text = field.GetValue(null) as string;
						ToLuaExport.sb.AppendLine(text);
						ToLuaExport.sb.AppendLine("\t}");
						return;
					}
				}
			}
			if (constructors.Length == 0)
			{
				if (!ToLuaExport.type.get_IsValueType())
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
				if (!ToLuaExport.HasDecimal(constructors[k].GetParameters()) && !ToLuaExport.IsObsolete(constructors[k]))
				{
					ConstructorInfo r = constructors[k];
					int num = _GenConstruct_c__AnonStorey9A.list.FindIndex((ConstructorInfo p) => ToLuaExport.CompareMethod(p, r) >= 0);
					if (num >= 0)
					{
						if (ToLuaExport.CompareMethod(_GenConstruct_c__AnonStorey9A.list.get_Item(num), r) == 2)
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
			if (_GenConstruct_c__AnonStorey9A.list.get_Count() == 0)
			{
				if (!ToLuaExport.type.get_IsValueType())
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
			for (i = 0; i < _GenConstruct_c__AnonStorey9A.list.get_Count(); i++)
			{
				int num2 = _GenConstruct_c__AnonStorey9A.list.FindIndex((ConstructorInfo p) => p != _GenConstruct_c__AnonStorey9A.list.get_Item(i) && p.GetParameters().Length == _GenConstruct_c__AnonStorey9A.list.get_Item(i).GetParameters().Length);
				if (num2 >= 0 || (ToLuaExport.HasOptionalParam(_GenConstruct_c__AnonStorey9A.list.get_Item(i).GetParameters()) && _GenConstruct_c__AnonStorey9A.list.get_Item(i).GetParameters().Length > 1))
				{
					list.Add(_GenConstruct_c__AnonStorey9A.list.get_Item(i));
				}
			}
			MethodBase methodBase = _GenConstruct_c__AnonStorey9A.list.get_Item(0);
			bool flag = _GenConstruct_c__AnonStorey9A.list.get_Item(0).GetParameters().Length == 0;
			if (ToLuaExport.HasOptionalParam(methodBase.GetParameters()))
			{
				ParameterInfo[] parameters = methodBase.GetParameters();
				ParameterInfo parameterInfo = parameters[parameters.Length - 1];
				string typeStr = ToLuaExport.GetTypeStr(parameterInfo.get_ParameterType().GetElementType());
				if (parameters.Length > 1)
				{
					string text2 = ToLuaExport.GenParamTypes(parameters, true);
					ToLuaExport.sb.AppendFormat("\t\tif (LuaScriptMgr.CheckTypes(L, 1, {0}) && LuaScriptMgr.CheckParamsType(L, typeof({1}), {2}, {3}))\r\n", new object[]
					{
						text2,
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
				if (_GenConstruct_c__AnonStorey9A.list.get_Count() == 1 || methodBase.GetParameters().Length != _GenConstruct_c__AnonStorey9A.list.get_Item(1).GetParameters().Length)
				{
					ToLuaExport.sb.AppendFormat("\t\tif (count == {0})\r\n", parameters2.Length);
				}
				else
				{
					string text3 = ToLuaExport.GenParamTypes(parameters2, true);
					ToLuaExport.sb.AppendFormat("\t\tif (count == {0} && LuaScriptMgr.CheckTypes(L, 1, {1}))\r\n", parameters2.Length, text3);
				}
			}
			ToLuaExport.sb.AppendLine("\t\t{");
			int num3 = ToLuaExport.ProcessParams(methodBase, 3, true, _GenConstruct_c__AnonStorey9A.list.get_Count() > 1, false);
			ToLuaExport.sb.AppendFormat("\t\t\treturn {0};\r\n", num3);
			ToLuaExport.sb.AppendLine("\t\t}");
			for (int j = 1; j < _GenConstruct_c__AnonStorey9A.list.get_Count(); j++)
			{
				flag = (_GenConstruct_c__AnonStorey9A.list.get_Item(j).GetParameters().Length == 0 || flag);
				methodBase = _GenConstruct_c__AnonStorey9A.list.get_Item(j);
				ParameterInfo[] parameters3 = methodBase.GetParameters();
				if (!ToLuaExport.HasOptionalParam(methodBase.GetParameters()))
				{
					if (list.Contains(_GenConstruct_c__AnonStorey9A.list.get_Item(j)))
					{
						string text4 = ToLuaExport.GenParamTypes(parameters3, true);
						ToLuaExport.sb.AppendFormat("\t\telse if (count == {0} && LuaScriptMgr.CheckTypes(L, 1, {1}))\r\n", parameters3.Length, text4);
					}
					else
					{
						ToLuaExport.sb.AppendFormat("\t\telse if (count == {0})\r\n", parameters3.Length);
					}
				}
				else
				{
					ParameterInfo parameterInfo2 = parameters3[parameters3.Length - 1];
					string typeStr2 = ToLuaExport.GetTypeStr(parameterInfo2.get_ParameterType().GetElementType());
					if (parameters3.Length > 1)
					{
						string text5 = ToLuaExport.GenParamTypes(parameters3, true);
						ToLuaExport.sb.AppendFormat("\t\telse if (LuaScriptMgr.CheckTypes(L, 1, {0}) && LuaScriptMgr.CheckParamsType(L, typeof({1}), {2}, {3}))\r\n", new object[]
						{
							text5,
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
			if (ToLuaExport.type.get_IsValueType() && !flag)
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
			int num = lhs.get_IsStatic() ? 0 : 1;
			int num2 = rhs.get_IsStatic() ? 0 : 1;
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
					return (num3 > num4) ? -1 : 1;
				}
				num3 -= num;
				num4 -= num2;
				if (parameters[num3].get_ParameterType().GetElementType() == typeof(object) && parameters2[num4].get_ParameterType().GetElementType() != typeof(object))
				{
					return 1;
				}
				if (parameters[num3].get_ParameterType().GetElementType() != typeof(object) && parameters2[num4].get_ParameterType().GetElementType() == typeof(object))
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
				if (list.get_Count() > list2.get_Count())
				{
					if (list.get_Item(0).get_ParameterType() == typeof(object))
					{
						return 1;
					}
					list.RemoveAt(0);
				}
				else if (list2.get_Count() > list.get_Count())
				{
					if (list2.get_Item(0).get_ParameterType() == typeof(object))
					{
						return -1;
					}
					list2.RemoveAt(0);
				}
				for (int i = 0; i < list.get_Count(); i++)
				{
					if (list.get_Item(i).get_ParameterType() == typeof(object) && list2.get_Item(i).get_ParameterType() != typeof(object))
					{
						return 1;
					}
					if (list.get_Item(i).get_ParameterType() != typeof(object) && list2.get_Item(i).get_ParameterType() == typeof(object))
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
			string text = (num >= 0) ? str.Remove(num) : str;
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
			if (!md.get_IsStatic() && !beConstruct)
			{
				if (md.get_Name() == "Equals")
				{
					if (!ToLuaExport.type.get_IsValueType())
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
					if (typeof(Object).IsAssignableFrom(ToLuaExport.type))
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
				string typeStr = ToLuaExport.GetTypeStr(parameterInfo.get_ParameterType());
				string text2 = "arg" + j;
				int num2 = (md.get_IsStatic() || beConstruct) ? 1 : 2;
				if (parameterInfo.get_Attributes() == 2)
				{
					Type refBaseType = ToLuaExport.GetRefBaseType(parameterInfo.get_ParameterType().ToString());
					if (refBaseType.get_IsValueType())
					{
						ToLuaExport.sb.AppendFormat("{0}{1} {2};\r\n", text, typeStr, text2);
					}
					else
					{
						ToLuaExport.sb.AppendFormat("{0}{1} {2} = null;\r\n", text, typeStr, text2);
					}
				}
				else if (parameterInfo.get_ParameterType() == typeof(bool))
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
				else if (parameterInfo.get_ParameterType() == typeof(string))
				{
					string text3 = beLuaString ? "GetString" : "GetLuaString";
					ToLuaExport.sb.AppendFormat("{2}string {0} = LuaScriptMgr.{3}(L, {1});\r\n", new object[]
					{
						text2,
						j + num2,
						text,
						text3
					});
				}
				else if (parameterInfo.get_ParameterType().get_IsPrimitive())
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
				else if (parameterInfo.get_ParameterType() == typeof(LuaFunction))
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
				else if (parameterInfo.get_ParameterType().IsSubclassOf(typeof(MulticastDelegate)))
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
					ToLuaExport.GenDelegateBody(parameterInfo.get_ParameterType(), text + "\t", true);
					ToLuaExport.sb.AppendLine(text + "}\r\n");
				}
				else if (parameterInfo.get_ParameterType() == typeof(LuaTable))
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
				else if (parameterInfo.get_ParameterType() == typeof(Vector2) || ToLuaExport.GetRefBaseType(parameterInfo.get_ParameterType().ToString()) == typeof(Vector2))
				{
					ToLuaExport.sb.AppendFormat("{2}Vector2 {0} = LuaScriptMgr.GetVector2(L, {1});\r\n", text2, j + num2, text);
				}
				else if (parameterInfo.get_ParameterType() == typeof(Vector3) || ToLuaExport.GetRefBaseType(parameterInfo.get_ParameterType().ToString()) == typeof(Vector3))
				{
					ToLuaExport.sb.AppendFormat("{2}Vector3 {0} = LuaScriptMgr.GetVector3(L, {1});\r\n", text2, j + num2, text);
				}
				else if (parameterInfo.get_ParameterType() == typeof(Vector4) || ToLuaExport.GetRefBaseType(parameterInfo.get_ParameterType().ToString()) == typeof(Vector4))
				{
					ToLuaExport.sb.AppendFormat("{2}Vector4 {0} = LuaScriptMgr.GetVector4(L, {1});\r\n", text2, j + num2, text);
				}
				else if (parameterInfo.get_ParameterType() == typeof(Quaternion) || ToLuaExport.GetRefBaseType(parameterInfo.get_ParameterType().ToString()) == typeof(Quaternion))
				{
					ToLuaExport.sb.AppendFormat("{2}Quaternion {0} = LuaScriptMgr.GetQuaternion(L, {1});\r\n", text2, j + num2, text);
				}
				else if (parameterInfo.get_ParameterType() == typeof(Color) || ToLuaExport.GetRefBaseType(parameterInfo.get_ParameterType().ToString()) == typeof(Color))
				{
					ToLuaExport.sb.AppendFormat("{2}Color {0} = LuaScriptMgr.GetColor(L, {1});\r\n", text2, j + num2, text);
				}
				else if (parameterInfo.get_ParameterType() == typeof(Ray) || ToLuaExport.GetRefBaseType(parameterInfo.get_ParameterType().ToString()) == typeof(Ray))
				{
					ToLuaExport.sb.AppendFormat("{2}Ray {0} = LuaScriptMgr.GetRay(L, {1});\r\n", text2, j + num2, text);
				}
				else if (parameterInfo.get_ParameterType() == typeof(Bounds) || ToLuaExport.GetRefBaseType(parameterInfo.get_ParameterType().ToString()) == typeof(Bounds))
				{
					ToLuaExport.sb.AppendFormat("{2}Bounds {0} = LuaScriptMgr.GetBounds(L, {1});\r\n", text2, j + num2, text);
				}
				else if (parameterInfo.get_ParameterType() == typeof(object))
				{
					ToLuaExport.sb.AppendFormat("{2}object {0} = LuaScriptMgr.GetVarObject(L, {1});\r\n", text2, j + num2, text);
				}
				else if (parameterInfo.get_ParameterType() == typeof(Type))
				{
					ToLuaExport.sb.AppendFormat("{0}{1} {2} = LuaScriptMgr.GetTypeObject(L, {3});\r\n", new object[]
					{
						text,
						typeStr,
						text2,
						j + num2
					});
				}
				else if (parameterInfo.get_ParameterType() == typeof(LuaStringBuffer))
				{
					ToLuaExport.sb.AppendFormat("{2}LuaStringBuffer {0} = LuaScriptMgr.GetStringBuffer(L, {1});\r\n", text2, j + num2, text);
				}
				else if (parameterInfo.get_ParameterType().get_IsArray())
				{
					Type elementType = parameterInfo.get_ParameterType().GetElementType();
					string typeStr2 = ToLuaExport.GetTypeStr(elementType);
					bool flag = false;
					bool flag2 = false;
					bool flag3 = false;
					string text4;
					if (elementType == typeof(bool))
					{
						text4 = "GetArrayBool";
					}
					else if (elementType.get_IsPrimitive())
					{
						flag = true;
						text4 = "GetArrayNumber";
					}
					else if (elementType == typeof(string))
					{
						flag2 = ToLuaExport.IsParams(parameterInfo);
						text4 = (flag2 ? "GetParamsString" : "GetArrayString");
					}
					else
					{
						flag = true;
						flag2 = ToLuaExport.IsParams(parameterInfo);
						text4 = (flag2 ? "GetParamsObject" : "GetArrayObject");
						if (elementType == typeof(object))
						{
							flag3 = true;
						}
						if (elementType == typeof(Object))
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
				else if (md.get_Name() == "op_Equality")
				{
					if (!ToLuaExport.type.get_IsValueType())
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
				else if (typeof(Object).IsAssignableFrom(parameterInfo.get_ParameterType()))
				{
					ToLuaExport.sb.AppendFormat("{3}{0} {1} = ({0})LuaScriptMgr.GetUnityObject(L, {2}, typeof({0}));\r\n", new object[]
					{
						typeStr,
						text2,
						j + num2,
						text
					});
				}
				else if (typeof(TrackedReference).IsAssignableFrom(parameterInfo.get_ParameterType()))
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
				if (!parameterInfo2.get_ParameterType().get_IsArray())
				{
					if (!parameterInfo2.get_ParameterType().ToString().Contains("&"))
					{
						stringBuilder.Append("arg");
					}
					else
					{
						if (parameterInfo2.get_Attributes() == 2)
						{
							stringBuilder.Append("out arg");
						}
						else
						{
							stringBuilder.Append("ref arg");
						}
						list.Add("arg" + k);
						list2.Add(ToLuaExport.GetRefBaseType(parameterInfo2.get_ParameterType().ToString()));
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
				if (!parameterInfo3.get_ParameterType().get_IsArray())
				{
					if (!parameterInfo3.get_ParameterType().ToString().Contains("&"))
					{
						stringBuilder.Append("arg");
					}
					else
					{
						if (parameterInfo3.get_Attributes() == 2)
						{
							stringBuilder.Append("out arg");
						}
						else
						{
							stringBuilder.Append("ref arg");
						}
						list.Add("arg" + (num - 1));
						list2.Add(ToLuaExport.GetRefBaseType(parameterInfo3.get_ParameterType().ToString()));
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
				for (int l = 0; l < list.get_Count(); l++)
				{
					pushFunction = ToLuaExport.GetPushFunction(list2.get_Item(l));
					ToLuaExport.sb.AppendFormat("{1}LuaScriptMgr.{2}(L, {0});\r\n", list.get_Item(l), text, pushFunction);
				}
				return list.get_Count() + 1;
			}
			string text5 = md.get_IsStatic() ? ToLuaExport.className : "obj";
			MethodInfo methodInfo = md as MethodInfo;
			if (methodInfo.get_ReturnType() == typeof(void))
			{
				if (md.get_Name() == "set_Item")
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
						md.get_Name(),
						stringBuilder.ToString(),
						text
					});
				}
				if (!md.get_IsStatic() && ToLuaExport.type.get_IsValueType())
				{
					ToLuaExport.sb.AppendFormat("{0}LuaScriptMgr.SetValueObject(L, 1, obj);\r\n", text);
				}
			}
			else
			{
				string typeStr3 = ToLuaExport.GetTypeStr(methodInfo.get_ReturnType());
				if (md.get_Name().Contains("op_"))
				{
					ToLuaExport.CallOpFunction(md.get_Name(), tab, typeStr3);
				}
				else if (md.get_Name() == "get_Item")
				{
					ToLuaExport.sb.AppendFormat("{4}{3} o = {0}[{2}];\r\n", new object[]
					{
						text5,
						md.get_Name(),
						stringBuilder.ToString(),
						typeStr3,
						text
					});
				}
				else if (md.get_Name() == "Equals")
				{
					if (ToLuaExport.type.get_IsValueType())
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
						md.get_Name(),
						stringBuilder.ToString(),
						typeStr3,
						text
					});
				}
				string pushFunction2 = ToLuaExport.GetPushFunction(methodInfo.get_ReturnType());
				ToLuaExport.sb.AppendFormat("{0}LuaScriptMgr.{1}(L, o);\r\n", text, pushFunction2);
			}
			for (int m = 0; m < list.get_Count(); m++)
			{
				string pushFunction3 = ToLuaExport.GetPushFunction(list2.get_Item(m));
				ToLuaExport.sb.AppendFormat("{1}LuaScriptMgr.{2}(L, {0});\r\n", list.get_Item(m), text, pushFunction3);
			}
			return list.get_Count();
		}

		private static bool CompareParmsCount(MethodBase l, MethodBase r)
		{
			if (l == r)
			{
				return false;
			}
			int num = l.get_IsStatic() ? 0 : 1;
			int num2 = r.get_IsStatic() ? 0 : 1;
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
			if (!l.get_IsStatic())
			{
				list.Add(ToLuaExport.type);
			}
			if (!r.get_IsStatic())
			{
				list2.Add(ToLuaExport.type);
			}
			for (int i = 0; i < parameters.Length; i++)
			{
				list.Add(parameters[i].get_ParameterType());
			}
			for (int j = 0; j < parameters2.Length; j++)
			{
				list2.Add(parameters2[j].get_ParameterType());
			}
			for (int k = 0; k < list.get_Count(); k++)
			{
				if (!ToLuaExport.typeSize.ContainsKey(list.get_Item(k)) || !ToLuaExport.typeSize.ContainsKey(list2.get_Item(k)))
				{
					if (list.get_Item(k) != list2.get_Item(k))
					{
						return -1;
					}
				}
				else if (list.get_Item(k).get_IsPrimitive() && list2.get_Item(k).get_IsPrimitive() && num == 0)
				{
					num = ((ToLuaExport.typeSize.get_Item(list.get_Item(k)) >= ToLuaExport.typeSize.get_Item(list2.get_Item(k))) ? 1 : 2);
				}
				else if (list.get_Item(k) != list2.get_Item(k))
				{
					return -1;
				}
			}
			if (num == 0 && l.get_IsStatic())
			{
				num = 2;
			}
			return num;
		}

		private static void Push(List<MethodInfo> list, MethodInfo r)
		{
			int num = list.FindIndex((MethodInfo p) => p.get_Name() == r.get_Name() && ToLuaExport.CompareMethod(p, r) >= 0);
			if (num < 0)
			{
				list.Add(r);
				return;
			}
			if (ToLuaExport.CompareMethod(list.get_Item(num), r) == 2)
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
				if (pi[i].get_ParameterType() == typeof(decimal))
				{
					return true;
				}
			}
			return false;
		}

		public static MethodInfo GenOverrideFunc(string name)
		{
			ToLuaExport._GenOverrideFunc_c__AnonStorey9D _GenOverrideFunc_c__AnonStorey9D = new ToLuaExport._GenOverrideFunc_c__AnonStorey9D();
			_GenOverrideFunc_c__AnonStorey9D.list = new List<MethodInfo>();
			for (int k = 0; k < ToLuaExport.methods.Length; k++)
			{
				if (ToLuaExport.methods[k].get_Name() == name && !ToLuaExport.methods[k].get_IsGenericMethod() && !ToLuaExport.HasDecimal(ToLuaExport.methods[k].GetParameters()))
				{
					ToLuaExport.Push(_GenOverrideFunc_c__AnonStorey9D.list, ToLuaExport.methods[k]);
				}
			}
			if (_GenOverrideFunc_c__AnonStorey9D.list.get_Count() == 1)
			{
				return _GenOverrideFunc_c__AnonStorey9D.list.get_Item(0);
			}
			_GenOverrideFunc_c__AnonStorey9D.list.Sort(new Comparison<MethodInfo>(ToLuaExport.Compare));
			ToLuaExport.sb.AppendLine("\r\n\t[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]");
			ToLuaExport.sb.AppendFormat("\tstatic int {0}(IntPtr L)\r\n", ToLuaExport.GetFuncName(name));
			ToLuaExport.sb.AppendLine("\t{");
			ToLuaExport.sb.AppendLine("\t\tint count = LuaDLL.lua_gettop(L);");
			List<MethodInfo> list = new List<MethodInfo>();
			int i;
			for (i = 0; i < _GenOverrideFunc_c__AnonStorey9D.list.get_Count(); i++)
			{
				int num = _GenOverrideFunc_c__AnonStorey9D.list.FindIndex((MethodInfo p) => ToLuaExport.CompareParmsCount(p, _GenOverrideFunc_c__AnonStorey9D.list.get_Item(i)));
				if (num >= 0 || (ToLuaExport.HasOptionalParam(_GenOverrideFunc_c__AnonStorey9D.list.get_Item(i).GetParameters()) && _GenOverrideFunc_c__AnonStorey9D.list.get_Item(i).GetParameters().Length > 1))
				{
					list.Add(_GenOverrideFunc_c__AnonStorey9D.list.get_Item(i));
				}
			}
			ToLuaExport.sb.AppendLine();
			MethodInfo methodInfo = _GenOverrideFunc_c__AnonStorey9D.list.get_Item(0);
			int num2 = (methodInfo.get_ReturnType() == typeof(void)) ? 0 : 1;
			int num3 = methodInfo.get_IsStatic() ? 0 : 1;
			int num4 = num3 + 1;
			int num5 = methodInfo.GetParameters().Length + num3;
			int num6 = _GenOverrideFunc_c__AnonStorey9D.list.get_Item(1).GetParameters().Length + (_GenOverrideFunc_c__AnonStorey9D.list.get_Item(1).get_IsStatic() ? 0 : 1);
			bool flag = true;
			bool beCheckTypes = true;
			if (ToLuaExport.HasOptionalParam(methodInfo.GetParameters()))
			{
				ParameterInfo[] parameters = methodInfo.GetParameters();
				ParameterInfo parameterInfo = parameters[parameters.Length - 1];
				string typeStr = ToLuaExport.GetTypeStr(parameterInfo.get_ParameterType().GetElementType());
				if (parameters.Length > 1)
				{
					string text = ToLuaExport.GenParamTypes(parameters, methodInfo.get_IsStatic());
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
					string text2 = ToLuaExport.GenParamTypes(parameters2, methodInfo.get_IsStatic());
					ToLuaExport.sb.AppendFormat("\t\tif (count == {0} && LuaScriptMgr.CheckTypes(L, 1, {2}))\r\n", parameters2.Length + num3, num4, text2);
				}
				else
				{
					ToLuaExport.sb.AppendFormat("\t\tif (count == {0})\r\n", parameters2.Length + num3);
				}
			}
			ToLuaExport.sb.AppendLine("\t\t{");
			int num7 = ToLuaExport.ProcessParams(methodInfo, 3, false, _GenOverrideFunc_c__AnonStorey9D.list.get_Count() > 1 && flag, beCheckTypes);
			ToLuaExport.sb.AppendFormat("\t\t\treturn {0};\r\n", num2 + num7);
			ToLuaExport.sb.AppendLine("\t\t}");
			for (int j = 1; j < _GenOverrideFunc_c__AnonStorey9D.list.get_Count(); j++)
			{
				flag = true;
				beCheckTypes = true;
				methodInfo = _GenOverrideFunc_c__AnonStorey9D.list.get_Item(j);
				num3 = (methodInfo.get_IsStatic() ? 0 : 1);
				num4 = num3 + 1;
				num2 = ((methodInfo.get_ReturnType() == typeof(void)) ? 0 : 1);
				if (!ToLuaExport.HasOptionalParam(methodInfo.GetParameters()))
				{
					ParameterInfo[] parameters3 = methodInfo.GetParameters();
					if (list.Contains(_GenOverrideFunc_c__AnonStorey9D.list.get_Item(j)))
					{
						string text3 = ToLuaExport.GenParamTypes(parameters3, methodInfo.get_IsStatic());
						ToLuaExport.sb.AppendFormat("\t\telse if (count == {0} && LuaScriptMgr.CheckTypes(L, 1, {2}))\r\n", parameters3.Length + num3, num4, text3);
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
					string typeStr2 = ToLuaExport.GetTypeStr(parameterInfo2.get_ParameterType().GetElementType());
					if (parameters4.Length > 1)
					{
						string text4 = ToLuaExport.GenParamTypes(parameters4, methodInfo.get_IsStatic());
						ToLuaExport.sb.AppendFormat("\t\telse if (LuaScriptMgr.CheckTypes(L, 1, {1}) && LuaScriptMgr.CheckParamsType(L, typeof({2}), {3}, {4}))\r\n", new object[]
						{
							num4,
							text4,
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
				if (types[i].get_IsGenericType())
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
			string fullName = t.get_FullName();
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
			if (t.get_IsArray())
			{
				t = t.GetElementType();
				string typeStr = ToLuaExport.GetTypeStr(t);
				return typeStr + "[]";
			}
			if (t.get_IsGenericType())
			{
				return ToLuaExport.GetGenericName(t);
			}
			return ToLuaExport._C(t.ToString());
		}

		public static string _C(string str)
		{
			if (str.get_Length() > 1 && str.get_Chars(str.get_Length() - 1) == '&')
			{
				str = str.Remove(str.get_Length() - 1);
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
				if (str.get_Length() > 12 && str.Substring(0, 12) == "UnityEngine.")
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
				else if (str.get_Length() > 7 && str.Substring(0, 7) == "System.")
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
			if (t.get_IsArray())
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
				result = string.Format("typeof(LuaTable{1}){0}", sep, t.get_IsArray() ? "[]" : string.Empty);
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
					if (p[i].get_Attributes() != 2)
					{
						list.Add(p[i].get_ParameterType());
					}
					else
					{
						list.Add(null);
					}
				}
			}
			for (int j = 0; j < list.get_Count() - 1; j++)
			{
				stringBuilder.Append(ToLuaExport.GetTypeOf(list.get_Item(j), ", "));
			}
			stringBuilder.Append(ToLuaExport.GetTypeOf(list.get_Item(list.get_Count() - 1), string.Empty));
			return stringBuilder.ToString();
		}

		private static void CheckObjectNull()
		{
			if (ToLuaExport.type.get_IsValueType())
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
				ToLuaExport.sb.AppendFormat("\tstatic int get_{0}(IntPtr L)\r\n", ToLuaExport.fields[i].get_Name());
				ToLuaExport.sb.AppendLine("\t{");
				string pushFunction = ToLuaExport.GetPushFunction(ToLuaExport.fields[i].get_FieldType());
				if (ToLuaExport.fields[i].get_IsStatic())
				{
					ToLuaExport.sb.AppendFormat("\t\tLuaScriptMgr.{2}(L, {0}.{1});\r\n", ToLuaExport.className, ToLuaExport.fields[i].get_Name(), pushFunction);
				}
				else
				{
					ToLuaExport.sb.AppendFormat("\t\tobject o = LuaScriptMgr.GetLuaObject(L, 1);\r\n", new object[0]);
					if (!ToLuaExport.type.get_IsValueType())
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
					ToLuaExport.sb.AppendFormat("\t\t\t\tLuaDLL.luaL_error(L, \"unknown member name {0}\");\r\n", ToLuaExport.fields[i].get_Name());
					ToLuaExport.sb.AppendLine("\t\t\t}");
					ToLuaExport.sb.AppendLine("\t\t\telse");
					ToLuaExport.sb.AppendLine("\t\t\t{");
					ToLuaExport.sb.AppendFormat("\t\t\t\tLuaDLL.luaL_error(L, \"attempt to index {0} on a nil value\");\r\n", ToLuaExport.fields[i].get_Name());
					ToLuaExport.sb.AppendLine("\t\t\t}");
					ToLuaExport.sb.AppendLine("\t\t}");
					ToLuaExport.sb.AppendLine();
					if (ToLuaExport.type.get_IsValueType())
					{
						ToLuaExport.sb.AppendFormat("\t\t{0} obj = ({0})o;\r\n", ToLuaExport.className);
					}
					ToLuaExport.sb.AppendFormat("\t\tLuaScriptMgr.{1}(L, obj.{0});\r\n", ToLuaExport.fields[i].get_Name(), pushFunction);
				}
				ToLuaExport.sb.AppendLine("\t\treturn 1;");
				ToLuaExport.sb.AppendLine("\t}");
			}
			for (int j = 0; j < ToLuaExport.props.Length; j++)
			{
				if (ToLuaExport.props[j].get_CanRead())
				{
					bool flag = true;
					int num = ToLuaExport.propList.IndexOf(ToLuaExport.props[j]);
					if (num >= 0)
					{
						flag = false;
					}
					ToLuaExport.sb.AppendLine("\r\n\t[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]");
					ToLuaExport.sb.AppendFormat("\tstatic int get_{0}(IntPtr L)\r\n", ToLuaExport.props[j].get_Name());
					ToLuaExport.sb.AppendLine("\t{");
					string pushFunction2 = ToLuaExport.GetPushFunction(ToLuaExport.props[j].get_PropertyType());
					if (flag)
					{
						ToLuaExport.sb.AppendFormat("\t\tLuaScriptMgr.{2}(L, {0}.{1});\r\n", ToLuaExport.className, ToLuaExport.props[j].get_Name(), pushFunction2);
					}
					else
					{
						ToLuaExport.sb.AppendFormat("\t\tobject o = LuaScriptMgr.GetLuaObject(L, 1);\r\n", new object[0]);
						if (!ToLuaExport.type.get_IsValueType())
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
						ToLuaExport.sb.AppendFormat("\t\t\t\tLuaDLL.luaL_error(L, \"unknown member name {0}\");\r\n", ToLuaExport.props[j].get_Name());
						ToLuaExport.sb.AppendLine("\t\t\t}");
						ToLuaExport.sb.AppendLine("\t\t\telse");
						ToLuaExport.sb.AppendLine("\t\t\t{");
						ToLuaExport.sb.AppendFormat("\t\t\t\tLuaDLL.luaL_error(L, \"attempt to index {0} on a nil value\");\r\n", ToLuaExport.props[j].get_Name());
						ToLuaExport.sb.AppendLine("\t\t\t}");
						ToLuaExport.sb.AppendLine("\t\t}");
						ToLuaExport.sb.AppendLine();
						if (ToLuaExport.type.get_IsValueType())
						{
							ToLuaExport.sb.AppendFormat("\t\t{0} obj = ({0})o;\r\n", ToLuaExport.className);
						}
						ToLuaExport.sb.AppendFormat("\t\tLuaScriptMgr.{1}(L, obj.{0});\r\n", ToLuaExport.props[j].get_Name(), pushFunction2);
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
				if (!ToLuaExport.fields[i].get_IsLiteral() && !ToLuaExport.fields[i].get_IsInitOnly() && !ToLuaExport.fields[i].get_IsPrivate())
				{
					ToLuaExport.sb.AppendLine("\r\n\t[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]");
					ToLuaExport.sb.AppendFormat("\tstatic int set_{0}(IntPtr L)\r\n", ToLuaExport.fields[i].get_Name());
					ToLuaExport.sb.AppendLine("\t{");
					string o = ToLuaExport.fields[i].get_IsStatic() ? ToLuaExport.className : "obj";
					if (!ToLuaExport.fields[i].get_IsStatic())
					{
						ToLuaExport.sb.AppendFormat("\t\tobject o = LuaScriptMgr.GetLuaObject(L, 1);\r\n", new object[0]);
						if (!ToLuaExport.type.get_IsValueType())
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
						ToLuaExport.sb.AppendFormat("\t\t\t\tLuaDLL.luaL_error(L, \"unknown member name {0}\");\r\n", ToLuaExport.fields[i].get_Name());
						ToLuaExport.sb.AppendLine("\t\t\t}");
						ToLuaExport.sb.AppendLine("\t\t\telse");
						ToLuaExport.sb.AppendLine("\t\t\t{");
						ToLuaExport.sb.AppendFormat("\t\t\t\tLuaDLL.luaL_error(L, \"attempt to index {0} on a nil value\");\r\n", ToLuaExport.fields[i].get_Name());
						ToLuaExport.sb.AppendLine("\t\t\t}");
						ToLuaExport.sb.AppendLine("\t\t}");
						ToLuaExport.sb.AppendLine();
						if (ToLuaExport.type.get_IsValueType())
						{
							ToLuaExport.sb.AppendFormat("\t\t{0} obj = ({0})o;\r\n", ToLuaExport.className);
						}
					}
					ToLuaExport.NewIndexSetValue(ToLuaExport.fields[i].get_FieldType(), o, ToLuaExport.fields[i].get_Name());
					if (!ToLuaExport.fields[i].get_IsStatic() && ToLuaExport.type.get_IsValueType())
					{
						ToLuaExport.sb.AppendLine("\t\tLuaScriptMgr.SetValueObject(L, 1, obj);");
					}
					ToLuaExport.sb.AppendLine("\t\treturn 0;");
					ToLuaExport.sb.AppendLine("\t}");
				}
			}
			for (int j = 0; j < ToLuaExport.props.Length; j++)
			{
				if (ToLuaExport.props[j].get_CanWrite() && ToLuaExport.props[j].GetSetMethod(true).get_IsPublic())
				{
					bool flag = true;
					int num = ToLuaExport.propList.IndexOf(ToLuaExport.props[j]);
					if (num >= 0)
					{
						flag = false;
					}
					ToLuaExport.sb.AppendLine("\r\n\t[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]");
					ToLuaExport.sb.AppendFormat("\tstatic int set_{0}(IntPtr L)\r\n", ToLuaExport.props[j].get_Name());
					ToLuaExport.sb.AppendLine("\t{");
					string o2 = flag ? ToLuaExport.className : "obj";
					if (!flag)
					{
						ToLuaExport.sb.AppendFormat("\t\tobject o = LuaScriptMgr.GetLuaObject(L, 1);\r\n", new object[0]);
						if (!ToLuaExport.type.get_IsValueType())
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
						ToLuaExport.sb.AppendFormat("\t\t\t\tLuaDLL.luaL_error(L, \"unknown member name {0}\");\r\n", ToLuaExport.props[j].get_Name());
						ToLuaExport.sb.AppendLine("\t\t\t}");
						ToLuaExport.sb.AppendLine("\t\t\telse");
						ToLuaExport.sb.AppendLine("\t\t\t{");
						ToLuaExport.sb.AppendFormat("\t\t\t\tLuaDLL.luaL_error(L, \"attempt to index {0} on a nil value\");\r\n", ToLuaExport.props[j].get_Name());
						ToLuaExport.sb.AppendLine("\t\t\t}");
						ToLuaExport.sb.AppendLine("\t\t}");
						ToLuaExport.sb.AppendLine();
						if (ToLuaExport.type.get_IsValueType())
						{
							ToLuaExport.sb.AppendFormat("\t\t{0} obj = ({0})o;\r\n", ToLuaExport.className);
						}
					}
					ToLuaExport.NewIndexSetValue(ToLuaExport.props[j].get_PropertyType(), o2, ToLuaExport.props[j].get_Name());
					if (!flag && ToLuaExport.type.get_IsValueType())
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
				if (method.get_ReturnType() == typeof(void))
				{
					ToLuaExport.sb.AppendFormat("{0}{{\r\n{0}\tfunc.Call();\r\n{0}}};\r\n", head);
				}
				else
				{
					ToLuaExport.sb.AppendFormat("{0}{{\r\n{0}\tobject[] objs = func.Call();\r\n", head);
					ToLuaExport.sb.AppendFormat("{1}\treturn ({0})objs[0];\r\n", ToLuaExport.GetTypeStr(method.get_ReturnType()), head);
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
				string pushFunction = ToLuaExport.GetPushFunction(parameters[j].get_ParameterType());
				ToLuaExport.sb.AppendFormat("{2}\tLuaScriptMgr.{0}(L, param{1});\r\n", pushFunction, j, head);
			}
			ToLuaExport.sb.AppendFormat("{1}\tfunc.PCall(top, {0});\r\n", num, head);
			if (method.get_ReturnType() == typeof(void))
			{
				ToLuaExport.sb.AppendFormat("{0}\tfunc.EndPCall(top);\r\n", head);
			}
			else
			{
				ToLuaExport.sb.AppendFormat("{0}\tobject[] objs = func.PopValues(top);\r\n", head);
				ToLuaExport.sb.AppendFormat("{0}\tfunc.EndPCall(top);\r\n", head);
				ToLuaExport.sb.AppendFormat("{1}\treturn ({0})objs[0];\r\n", ToLuaExport.GetTypeStr(method.get_ReturnType()), head);
			}
			ToLuaExport.sb.AppendFormat("{0}}};\r\n", head);
		}

		private static void NewIndexSetValue(Type t, string o, string name)
		{
			if (t.get_IsArray())
			{
				Type elementType = t.GetElementType();
				string typeStr = ToLuaExport.GetTypeStr(elementType);
				if (elementType == typeof(bool))
				{
					ToLuaExport.sb.AppendFormat("\t\t{0}.{1} = LuaScriptMgr.GetArrayBool(L, 3);\r\n", o, name);
				}
				else if (elementType.get_IsPrimitive())
				{
					ToLuaExport.sb.AppendFormat("\t\t{0}.{1} = LuaScriptMgr.GetArrayNumber<{2}>(L, 3);\r\n", o, name, typeStr);
				}
				else if (elementType == typeof(string))
				{
					ToLuaExport.sb.AppendFormat("\t\t{0}.{1} = LuaScriptMgr.GetArrayString(L, 3);\r\n", o, name);
				}
				else
				{
					if (elementType == typeof(Object))
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
			else if (t.get_IsPrimitive())
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
			else if (typeof(Object).IsAssignableFrom(t))
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
			else if (typeof(object).IsAssignableFrom(t) || t.get_IsEnum())
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
			int num = Array.FindIndex<MethodInfo>(ToLuaExport.methods, (MethodInfo p) => p.get_Name() == "ToString");
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
			ToLuaExport.fields = ToLuaExport.type.GetFields(1048);
			List<FieldInfo> list = new List<FieldInfo>(ToLuaExport.fields);
			for (int i = list.get_Count() - 1; i > 0; i--)
			{
				if (ToLuaExport.IsObsolete(list.get_Item(i)))
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
				ToLuaExport.sb.AppendFormat("\t\tnew LuaMethod(\"{0}\", Get{0}),\r\n", ToLuaExport.fields[j].get_Name());
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
				ToLuaExport.sb.AppendFormat("\tstatic int Get{0}(IntPtr L)\r\n", ToLuaExport.fields[k].get_Name());
				ToLuaExport.sb.AppendLine("\t{");
				ToLuaExport.sb.AppendFormat("\t\tLuaScriptMgr.Push(L, {0}.{1});\r\n", ToLuaExport.className, ToLuaExport.fields[k].get_Name());
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
					Debug.LogError(type.get_FullName() + " not a delegate type");
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
				if (type.get_IsGenericType())
				{
					array[i] = ToLuaExport.GetGenericLibName(types[i]);
				}
				else if (type.get_IsArray())
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
			string name = type.get_Name();
			int num = name.IndexOf('`');
			string text = (num != -1) ? name.Substring(0, num) : name;
			text = ToLuaExport._C(text);
			return text + "_" + string.Join("_", ToLuaExport.GetGenericLibName(genericArguments));
		}

		private static void ProcessExtends(List<MethodInfo> list)
		{
			ToLuaExport.extendName = "ToLua_" + ToLuaExport.libClassName.Replace(".", "_");
			ToLuaExport.extendType = Type.GetType(ToLuaExport.extendName + ", Assembly-CSharp-Editor");
			if (ToLuaExport.extendType != null)
			{
				ToLuaExport._ProcessExtends_c__AnonStorey9F _ProcessExtends_c__AnonStorey9F = new ToLuaExport._ProcessExtends_c__AnonStorey9F();
				_ProcessExtends_c__AnonStorey9F.list2 = new List<MethodInfo>();
				_ProcessExtends_c__AnonStorey9F.list2.AddRange(ToLuaExport.extendType.GetMethods(4 | ToLuaExport.binding | 2));
				int i;
				for (i = _ProcessExtends_c__AnonStorey9F.list2.get_Count() - 1; i >= 0; i--)
				{
					if ((!_ProcessExtends_c__AnonStorey9F.list2.get_Item(i).get_Name().Contains("op_") && !_ProcessExtends_c__AnonStorey9F.list2.get_Item(i).get_Name().Contains("add_") && !_ProcessExtends_c__AnonStorey9F.list2.get_Item(i).get_Name().Contains("remove_")) || ToLuaExport.IsNeedOp(_ProcessExtends_c__AnonStorey9F.list2.get_Item(i).get_Name()))
					{
						list.RemoveAll((MethodInfo md) => md.get_Name() == _ProcessExtends_c__AnonStorey9F.list2.get_Item(i).get_Name());
						if (!ToLuaExport.IsObsolete(_ProcessExtends_c__AnonStorey9F.list2.get_Item(i)))
						{
							list.Add(_ProcessExtends_c__AnonStorey9F.list2.get_Item(i));
						}
					}
				}
			}
		}
	}
}
