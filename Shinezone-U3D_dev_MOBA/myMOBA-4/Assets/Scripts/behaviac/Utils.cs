using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace behaviac
{
	public static class Utils
	{
		private static Dictionary<string, bool> ms_staticClasses;

		private static Dictionary<string, string> ms_type_mapping = new Dictionary<string, string>
		{
			{
				"Boolean",
				"bool"
			},
			{
				"System.Boolean",
				"bool"
			},
			{
				"Int32",
				"int"
			},
			{
				"System.Int32",
				"int"
			},
			{
				"UInt32",
				"uint"
			},
			{
				"System.UInt32",
				"uint"
			},
			{
				"Int16",
				"short"
			},
			{
				"System.Int16",
				"short"
			},
			{
				"UInt16",
				"ushort"
			},
			{
				"System.UInt16",
				"ushort"
			},
			{
				"Int8",
				"sbyte"
			},
			{
				"System.Int8",
				"sbyte"
			},
			{
				"SByte",
				"sbyte"
			},
			{
				"System.SByte",
				"sbyte"
			},
			{
				"UInt8",
				"ubyte"
			},
			{
				"System.UInt8",
				"ubyte"
			},
			{
				"Byte",
				"ubyte"
			},
			{
				"System.Byte",
				"ubyte"
			},
			{
				"Char",
				"char"
			},
			{
				"Int64",
				"long"
			},
			{
				"System.Int64",
				"long"
			},
			{
				"UInt64",
				"ulong"
			},
			{
				"System.UInt64",
				"ulong"
			},
			{
				"Single",
				"float"
			},
			{
				"System.Single",
				"float"
			},
			{
				"Double",
				"double"
			},
			{
				"System.Double",
				"double"
			},
			{
				"String",
				"string"
			},
			{
				"System.String",
				"string"
			},
			{
				"Void",
				"void"
			}
		};

		private static Dictionary<string, bool> StaticClasses
		{
			get
			{
				if (Utils.ms_staticClasses == null)
				{
					Utils.ms_staticClasses = new Dictionary<string, bool>();
				}
				return Utils.ms_staticClasses;
			}
		}

		public static bool IsNull(object aObj)
		{
			return aObj == null || aObj.Equals(null);
		}

		public static bool IsStaticType(Type type)
		{
			return type != null && type.IsAbstract && type.IsSealed;
		}

		public static void AddStaticClass(Type type)
		{
			if (Utils.IsStaticType(type))
			{
				Utils.StaticClasses[type.FullName] = true;
			}
		}

		public static bool IsStaticClass(string className)
		{
			return Utils.StaticClasses.ContainsKey(className);
		}

		public static uint MakeVariableId(string idstring)
		{
			return CRC32.CalcCRC(idstring);
		}

		public static bool IsParVar(string variableName)
		{
			int num = variableName.LastIndexOf(':');
			return num == -1 || variableName[num - 1] != ':';
		}

		public static string GetNameWithoutClassName(string variableName)
		{
			if (string.IsNullOrEmpty(variableName))
			{
				return null;
			}
			int num = variableName.LastIndexOf(':');
			if (num > 0 && variableName[num - 1] == ':')
			{
				return variableName.Substring(num + 1);
			}
			return variableName;
		}

		public static int GetClassTypeNumberId<T>()
		{
			return 0;
		}

		public static void ConvertFromInteger<T>(int v, ref T ret)
		{
		}

		public static uint ConvertToInteger<T>(T v)
		{
			return 0u;
		}

		public static Type GetType(string typeName)
		{
			Type type = Utility.GetType(typeName);
			if (type != null)
			{
				return type;
			}
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (int i = 0; i < assemblies.Length; i++)
			{
				Assembly assembly = assemblies[i];
				type = assembly.GetType(typeName);
				if (type != null)
				{
					return type;
				}
			}
			if (typeName.StartsWith("System.Collections.Generic.List"))
			{
				int num = typeName.IndexOf("[[");
				if (num > -1)
				{
					int num2 = typeName.IndexOf(",");
					if (num2 < 0)
					{
						num2 = typeName.IndexOf("]]");
					}
					if (num2 > num)
					{
						string typeName2 = typeName.Substring(num + 2, num2 - num - 2);
						type = Utils.GetType(typeName2);
						if (type != null)
						{
							return typeof(List<>).MakeGenericType(new Type[]
							{
								type
							});
						}
					}
				}
			}
			return null;
		}

		private static object GetDefaultValue(Type t)
		{
			if (t.IsValueType)
			{
				return Activator.CreateInstance(t);
			}
			return null;
		}

		public static object GetValueFromString(Type type, string value)
		{
			if (value != null)
			{
				if (type == typeof(string) && !string.IsNullOrEmpty(value) && value.Length > 1 && value[0] == '"' && value[value.Length - 1] == '"')
				{
					value = value.Substring(1, value.Length - 2);
				}
				try
				{
					TypeConverter converter = TypeDescriptor.GetConverter(type);
					object result = converter.ConvertFromString(value);
					return result;
				}
				catch
				{
					if (type == typeof(bool))
					{
						bool flag;
						if (bool.TryParse(value, out flag))
						{
							object result = flag;
							return result;
						}
					}
					else if (type == typeof(int))
					{
						int num;
						if (int.TryParse(value, out num))
						{
							object result = num;
							return result;
						}
					}
					else if (type == typeof(uint))
					{
						uint num2;
						if (uint.TryParse(value, out num2))
						{
							object result = num2;
							return result;
						}
					}
					else if (type == typeof(short))
					{
						short num3;
						if (short.TryParse(value, out num3))
						{
							object result = num3;
							return result;
						}
					}
					else if (type == typeof(ushort))
					{
						ushort num4;
						if (ushort.TryParse(value, out num4))
						{
							object result = num4;
							return result;
						}
					}
					else if (type == typeof(char))
					{
						char c;
						if (char.TryParse(value, out c))
						{
							object result = c;
							return result;
						}
					}
					else if (type == typeof(sbyte))
					{
						sbyte b;
						if (sbyte.TryParse(value, out b))
						{
							object result = b;
							return result;
						}
					}
					else if (type == typeof(byte))
					{
						byte b2;
						if (byte.TryParse(value, out b2))
						{
							object result = b2;
							return result;
						}
					}
					else if (type == typeof(long))
					{
						long num5;
						if (long.TryParse(value, out num5))
						{
							object result = num5;
							return result;
						}
					}
					else if (type == typeof(ulong))
					{
						ulong num6;
						if (ulong.TryParse(value, out num6))
						{
							object result = num6;
							return result;
						}
					}
					else if (type == typeof(float))
					{
						float num7;
						if (float.TryParse(value, out num7))
						{
							object result = num7;
							return result;
						}
					}
					else if (type == typeof(double))
					{
						double num8;
						if (double.TryParse(value, out num8))
						{
							object result = num8;
							return result;
						}
					}
					else if (type == typeof(string))
					{
						object result = value;
						return result;
					}
				}
			}
			return Utils.GetDefaultValue(type);
		}

		public static Type GetTypeFromName(string typeName)
		{
			if (string.IsNullOrEmpty(typeName))
			{
				return null;
			}
			switch (typeName)
			{
			case "bool":
				return typeof(bool);
			case "int":
				return typeof(int);
			case "uint":
				return typeof(uint);
			case "short":
				return typeof(short);
			case "ushort":
				return typeof(ushort);
			case "char":
				return typeof(char);
			case "sbyte":
				return typeof(sbyte);
			case "ubyte":
			case "byte":
				return typeof(byte);
			case "long":
				return typeof(long);
			case "ulong":
				return typeof(ulong);
			case "float":
				return typeof(float);
			case "double":
				return typeof(double);
			case "string":
				return typeof(string);
			}
			return Utils.GetType(typeName);
		}

		public static string GetNativeTypeName(string typeName)
		{
			if (string.IsNullOrEmpty(typeName))
			{
				return string.Empty;
			}
			foreach (KeyValuePair<string, string> current in Utils.ms_type_mapping)
			{
				if (current.Key == typeName)
				{
					string result = current.Value;
					return result;
				}
				string a = current.Key + "&";
				if (a == typeName)
				{
					string result = current.Value + "&";
					return result;
				}
			}
			string[] array = typeName.Split(new char[]
			{
				'.'
			}, StringSplitOptions.RemoveEmptyEntries);
			return array[array.Length - 1];
		}

		public static string GetNativeTypeName(Type type)
		{
			if (Utils.IsArrayType(type))
			{
				Type type2 = type.GetGenericArguments()[0];
				return string.Format("vector<{0}>", Utils.GetNativeTypeName(type2));
			}
			return Utils.GetNativeTypeName(type.Name);
		}

		public static bool IsStringType(Type type)
		{
			return type == typeof(string);
		}

		public static bool IsEnumType(Type type)
		{
			return type != null && type.IsEnum;
		}

		public static bool IsArrayType(Type type)
		{
			return type != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
		}

		public static bool IsCustomClassType(Type type)
		{
			return type != null && !type.IsByRef && (type.IsClass || type.IsValueType) && type != typeof(void) && !type.IsEnum && !type.IsPrimitive && !Utils.IsStringType(type) && !Utils.IsArrayType(type);
		}
	}
}
