using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace behaviac
{
	public static class StringUtils
	{
		private static int SkipPairedBrackets(string src, int indexBracketBegin)
		{
			if (!string.IsNullOrEmpty(src) && src.get_Chars(indexBracketBegin) == '{')
			{
				int num = 0;
				for (int i = indexBracketBegin; i < src.get_Length(); i++)
				{
					if (src.get_Chars(i) == '{')
					{
						num++;
					}
					else if (src.get_Chars(i) == '}' && --num == 0)
					{
						return i;
					}
				}
			}
			return -1;
		}

		private static object FromStringStruct(Type type, string src)
		{
			object obj = Activator.CreateInstance(type);
			DictionaryView<string, FieldInfo> dictionaryView = new DictionaryView<string, FieldInfo>();
			FieldInfo[] fields = type.GetFields(62);
			for (int i = 0; i < fields.Length; i++)
			{
				FieldInfo fieldInfo = fields[i];
				if (!fieldInfo.get_IsLiteral())
				{
					dictionaryView.Add(fieldInfo.get_Name(), fieldInfo);
				}
			}
			if (string.IsNullOrEmpty(src))
			{
				return obj;
			}
			int num = StringUtils.SkipPairedBrackets(src, 0);
			int num2 = 1;
			int num3 = src.IndexOf(';', num2);
			while (num3 != -1)
			{
				if (num3 > num2)
				{
					int num4 = src.IndexOf('=', num2);
					int num5 = num4 - num2;
					string key = src.Substring(num2, num5);
					char c = src.get_Chars(num4 + 1);
					string valStr;
					if (c != '{')
					{
						num5 = num3 - num4 - 1;
						valStr = src.Substring(num4 + 1, num5);
					}
					else
					{
						int num6 = 0;
						num6 += num4 + 1;
						int num7 = StringUtils.SkipPairedBrackets(src, num6);
						num5 = num7 - num6 + 1;
						valStr = src.Substring(num4 + 1, num5);
						num3 = num4 + 1 + num5;
					}
					if (dictionaryView.ContainsKey(key))
					{
						FieldInfo fieldInfo2 = dictionaryView[key];
						object obj2 = StringUtils.FromString(fieldInfo2.get_FieldType(), valStr, false);
						fieldInfo2.SetValue(obj, obj2);
					}
				}
				num2 = num3 + 1;
				num3 = src.IndexOf(';', num2);
				if (num3 > num)
				{
					break;
				}
			}
			return obj;
		}

		private static object FromStringVector(Type type, string src)
		{
			Type type2 = typeof(List).MakeGenericType(new Type[]
			{
				type
			});
			IList list = (IList)Activator.CreateInstance(type2);
			if (string.IsNullOrEmpty(src))
			{
				return list;
			}
			int num = src.IndexOf(':');
			string text = src.Substring(0, num);
			int num2 = int.Parse(text);
			int num3 = num + 1;
			int num4 = num3;
			if (num3 < src.get_Length() && src.get_Chars(num3) == '{')
			{
				num4 = StringUtils.SkipPairedBrackets(src, num3);
			}
			for (num4 = src.IndexOf('|', num4); num4 != -1; num4 = src.IndexOf('|', num4))
			{
				int num5 = num4 - num3;
				string valStr = src.Substring(num3, num5);
				object obj = StringUtils.FromString(type, valStr, false);
				list.Add(obj);
				num3 = num4 + 1;
				if (num3 < src.get_Length() && src.get_Chars(num3) == '{')
				{
					num4 = StringUtils.SkipPairedBrackets(src, num3);
				}
				else
				{
					num4 = num3;
				}
			}
			if (num3 < src.get_Length())
			{
				int num6 = src.get_Length() - num3;
				string valStr2 = src.Substring(num3, num6);
				object obj2 = StringUtils.FromString(type, valStr2, false);
				list.Add(obj2);
			}
			return list;
		}

		public static object FromString(Type type, string valStr, bool bStrIsArrayType)
		{
			if (!string.IsNullOrEmpty(valStr) && valStr == "null")
			{
				return null;
			}
			if (type.get_IsByRef())
			{
				type = type.GetElementType();
			}
			bool flag = Utils.IsArrayType(type);
			object result;
			if (bStrIsArrayType || flag)
			{
				if (flag)
				{
					Type type2 = type.GetGenericArguments()[0];
					result = StringUtils.FromStringVector(type2, valStr);
				}
				else
				{
					result = StringUtils.FromStringVector(type, valStr);
				}
			}
			else if (type == typeof(Property))
			{
				result = Condition.LoadProperty(valStr);
			}
			else if (Utils.IsCustomClassType(type))
			{
				result = StringUtils.FromStringStruct(type, valStr);
			}
			else
			{
				result = Utils.GetValueFromString(type, valStr);
			}
			return result;
		}

		public static string ToString(object value)
		{
			string text = string.Empty;
			if (value != null)
			{
				Type type = value.GetType();
				bool flag = Utils.IsArrayType(type);
				if (flag)
				{
					IList list = value as IList;
					text = string.Format("{0}:", list.get_Count());
					if (list.get_Count() > 0)
					{
						for (int i = 0; i < list.get_Count() - 1; i++)
						{
							object value2 = list.get_Item(i);
							string text2 = StringUtils.ToString(value2);
							text += string.Format("{0}|", text2);
						}
						object value3 = list.get_Item(list.get_Count() - 1);
						string text3 = StringUtils.ToString(value3);
						text += string.Format("{0}", text3);
					}
				}
				else if (Utils.IsCustomClassType(type))
				{
					bool flag2 = type == typeof(Agent) || type.IsSubclassOf(typeof(Agent));
					if (flag2)
					{
						text = string.Format("{0:x08}", value.GetHashCode());
					}
					else
					{
						text = "{";
						FieldInfo[] fields = type.GetFields(62);
						for (int j = 0; j < fields.Length; j++)
						{
							FieldInfo fieldInfo = fields[j];
							object value4 = fieldInfo.GetValue(value);
							string text4 = StringUtils.ToString(value4);
							text += string.Format("{0}={1};", fieldInfo.get_Name(), text4);
						}
						text += "}";
					}
				}
				else
				{
					text = value.ToString();
				}
			}
			else
			{
				text = "null";
			}
			return text;
		}

		public static string FindExtension(string path)
		{
			return Path.GetExtension(path);
		}

		public static int FirstToken(string params_, char sep, ref string token)
		{
			int num = params_.IndexOf(sep);
			if (num != -1)
			{
				token = params_.Substring(0, num);
				return num;
			}
			return -1;
		}

		public static bool ParseForStruct(Type type, string str, ref string strT, DictionaryView<string, Property> props)
		{
			int num = 0;
			for (int i = 0; i < str.get_Length(); i++)
			{
				char c = str.get_Chars(i);
				if (c == ';' || c == '{' || c == '}')
				{
					int j = num;
					while (j <= i)
					{
						strT += str.get_Chars(j++);
					}
					num = i + 1;
				}
				else if (c == ' ')
				{
					string text = string.Empty;
					int num2 = num;
					while (str.get_Chars(num2) != '=')
					{
						text += str.get_Chars(num2++);
					}
					num2++;
					string text2 = string.Empty;
					while (str.get_Chars(num2) != ' ')
					{
						text2 += str.get_Chars(num2++);
					}
					bool bStatic = false;
					if (text2 == "static")
					{
						num2++;
						while (str.get_Chars(num2) != ' ')
						{
							text2 += str.get_Chars(num2++);
						}
						bStatic = true;
					}
					string text3 = string.Empty;
					i++;
					while (str.get_Chars(i) != ';')
					{
						text3 += str.get_Chars(i++);
					}
					props[text] = Property.Create(text2, text3, null, bStatic, false);
					num = i + 1;
				}
			}
			return true;
		}
	}
}
