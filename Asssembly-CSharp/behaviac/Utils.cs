using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace behaviac
{
	public static class Utils
	{
		private static Dictionary<string, bool> ms_staticClasses;

		private static Dictionary<string, string> ms_type_mapping;

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

		static Utils()
		{
			// Note: this type is marked as 'beforefieldinit'.
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("Boolean", "bool");
			dictionary.Add("System.Boolean", "bool");
			dictionary.Add("Int32", "int");
			dictionary.Add("System.Int32", "int");
			dictionary.Add("UInt32", "uint");
			dictionary.Add("System.UInt32", "uint");
			dictionary.Add("Int16", "short");
			dictionary.Add("System.Int16", "short");
			dictionary.Add("UInt16", "ushort");
			dictionary.Add("System.UInt16", "ushort");
			dictionary.Add("Int8", "sbyte");
			dictionary.Add("System.Int8", "sbyte");
			dictionary.Add("SByte", "sbyte");
			dictionary.Add("System.SByte", "sbyte");
			dictionary.Add("UInt8", "ubyte");
			dictionary.Add("System.UInt8", "ubyte");
			dictionary.Add("Byte", "ubyte");
			dictionary.Add("System.Byte", "ubyte");
			dictionary.Add("Char", "char");
			dictionary.Add("Int64", "long");
			dictionary.Add("System.Int64", "long");
			dictionary.Add("UInt64", "ulong");
			dictionary.Add("System.UInt64", "ulong");
			dictionary.Add("Single", "float");
			dictionary.Add("System.Single", "float");
			dictionary.Add("Double", "double");
			dictionary.Add("System.Double", "double");
			dictionary.Add("String", "string");
			dictionary.Add("System.String", "string");
			dictionary.Add("Void", "void");
			Utils.ms_type_mapping = dictionary;
		}

		public static bool IsNull(object aObj)
		{
			return aObj == null || aObj.Equals(null);
		}

		public static bool IsStaticType(Type type)
		{
			return type != null && type.get_IsAbstract() && type.get_IsSealed();
		}

		public static void AddStaticClass(Type type)
		{
			if (Utils.IsStaticType(type))
			{
				Utils.StaticClasses.set_Item(type.get_FullName(), true);
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
			return num == -1 || variableName.get_Chars(num - 1) != ':';
		}

		public static string GetNameWithoutClassName(string variableName)
		{
			if (string.IsNullOrEmpty(variableName))
			{
				return null;
			}
			int num = variableName.LastIndexOf(':');
			if (num > 0 && variableName.get_Chars(num - 1) == ':')
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
			Assembly[] assemblies = AppDomain.get_CurrentDomain().GetAssemblies();
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
							return typeof(List).MakeGenericType(new Type[]
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
			if (t.get_IsValueType())
			{
				return Activator.CreateInstance(t);
			}
			return null;
		}

		public static object GetValueFromString(Type type, string value)
		{
			if (value != null)
			{
				if (type == typeof(string) && !string.IsNullOrEmpty(value) && value.get_Length() > 1 && value.get_Chars(0) == '"' && value.get_Chars(value.get_Length() - 1) == '"')
				{
					value = value.Substring(1, value.get_Length() - 2);
				}
				try
				{
					TypeConverter converter = TypeDescriptor.GetConverter(type);
					object obj = converter.ConvertFromString(value);
					object result = obj;
					return result;
				}
				catch
				{
					if (type == typeof(bool))
					{
						bool flag;
						if (bool.TryParse(value, ref flag))
						{
							object obj2 = flag;
							object result = obj2;
							return result;
						}
					}
					else if (type == typeof(int))
					{
						int num;
						if (int.TryParse(value, ref num))
						{
							object obj3 = num;
							object result = obj3;
							return result;
						}
					}
					else if (type == typeof(uint))
					{
						uint num2;
						if (uint.TryParse(value, ref num2))
						{
							object obj4 = num2;
							object result = obj4;
							return result;
						}
					}
					else if (type == typeof(short))
					{
						short num3;
						if (short.TryParse(value, ref num3))
						{
							object obj5 = num3;
							object result = obj5;
							return result;
						}
					}
					else if (type == typeof(ushort))
					{
						ushort num4;
						if (ushort.TryParse(value, ref num4))
						{
							object obj6 = num4;
							object result = obj6;
							return result;
						}
					}
					else if (type == typeof(char))
					{
						char c;
						if (char.TryParse(value, ref c))
						{
							object obj7 = c;
							object result = obj7;
							return result;
						}
					}
					else if (type == typeof(sbyte))
					{
						sbyte b;
						if (sbyte.TryParse(value, ref b))
						{
							object obj8 = b;
							object result = obj8;
							return result;
						}
					}
					else if (type == typeof(byte))
					{
						byte b2;
						if (byte.TryParse(value, ref b2))
						{
							object obj9 = b2;
							object result = obj9;
							return result;
						}
					}
					else if (type == typeof(long))
					{
						long num5;
						if (long.TryParse(value, ref num5))
						{
							object obj10 = num5;
							object result = obj10;
							return result;
						}
					}
					else if (type == typeof(ulong))
					{
						ulong num6;
						if (ulong.TryParse(value, ref num6))
						{
							object obj11 = num6;
							object result = obj11;
							return result;
						}
					}
					else if (type == typeof(float))
					{
						float num7;
						if (float.TryParse(value, ref num7))
						{
							object obj12 = num7;
							object result = obj12;
							return result;
						}
					}
					else if (type == typeof(double))
					{
						double num8;
						if (double.TryParse(value, ref num8))
						{
							object obj13 = num8;
							object result = obj13;
							return result;
						}
					}
					else if (type == typeof(string))
					{
						object obj14 = value;
						object result = obj14;
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
			if (typeName != null)
			{
				if (Utils.<>f__switch$map3 == null)
				{
					Dictionary<string, int> dictionary = new Dictionary<string, int>(14);
					dictionary.Add("bool", 0);
					dictionary.Add("int", 1);
					dictionary.Add("uint", 2);
					dictionary.Add("short", 3);
					dictionary.Add("ushort", 4);
					dictionary.Add("char", 5);
					dictionary.Add("sbyte", 6);
					dictionary.Add("ubyte", 7);
					dictionary.Add("byte", 7);
					dictionary.Add("long", 8);
					dictionary.Add("ulong", 9);
					dictionary.Add("float", 10);
					dictionary.Add("double", 11);
					dictionary.Add("string", 12);
					Utils.<>f__switch$map3 = dictionary;
				}
				int num;
				if (Utils.<>f__switch$map3.TryGetValue(typeName, ref num))
				{
					switch (num)
					{
					case 0:
						return typeof(bool);
					case 1:
						return typeof(int);
					case 2:
						return typeof(uint);
					case 3:
						return typeof(short);
					case 4:
						return typeof(ushort);
					case 5:
						return typeof(char);
					case 6:
						return typeof(sbyte);
					case 7:
						return typeof(byte);
					case 8:
						return typeof(long);
					case 9:
						return typeof(ulong);
					case 10:
						return typeof(float);
					case 11:
						return typeof(double);
					case 12:
						return typeof(string);
					}
				}
			}
			return Utils.GetType(typeName);
		}

		public static string GetNativeTypeName(string typeName)
		{
			if (string.IsNullOrEmpty(typeName))
			{
				return string.Empty;
			}
			using (Dictionary<string, string>.Enumerator enumerator = Utils.ms_type_mapping.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, string> current = enumerator.get_Current();
					if (current.get_Key() == typeName)
					{
						string value = current.get_Value();
						string result = value;
						return result;
					}
					string text = current.get_Key() + "&";
					if (text == typeName)
					{
						string text2 = current.get_Value() + "&";
						string result = text2;
						return result;
					}
				}
			}
			string[] array = typeName.Split(new char[]
			{
				'.'
			}, 1);
			return array[array.Length - 1];
		}

		public static string GetNativeTypeName(Type type)
		{
			if (Utils.IsArrayType(type))
			{
				Type type2 = type.GetGenericArguments()[0];
				return string.Format("vector<{0}>", Utils.GetNativeTypeName(type2));
			}
			return Utils.GetNativeTypeName(type.get_Name());
		}

		public static bool IsStringType(Type type)
		{
			return type == typeof(string);
		}

		public static bool IsEnumType(Type type)
		{
			return type != null && type.get_IsEnum();
		}

		public static bool IsArrayType(Type type)
		{
			return type != null && type.get_IsGenericType() && type.GetGenericTypeDefinition() == typeof(List);
		}

		public static bool IsCustomClassType(Type type)
		{
			return type != null && !type.get_IsByRef() && (type.get_IsClass() || type.get_IsValueType()) && type != typeof(void) && !type.get_IsEnum() && !type.get_IsPrimitive() && !Utils.IsStringType(type) && !Utils.IsArrayType(type);
		}
	}
}
