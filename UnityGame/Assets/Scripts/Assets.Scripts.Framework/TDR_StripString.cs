using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Assets.Scripts.Framework
{
	public static class TDR_StripString
	{
		public static int bytes;

		public static int savedBytes;

		public static double time;

		public static void exec(Dictionary<long, object>.ValueCollection items, Type InKeyType, Type InValueType)
		{
			object[] array = new object[items.get_Count()];
			items.CopyTo(array, 0);
			TDR_StripString.exec(array, InKeyType, InValueType);
		}

		public static void exec(object[] items, Type InKeyType, Type InValueType)
		{
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			FieldInfo[] fields = InValueType.GetFields(52);
			for (int i = 0; i < fields.Length; i++)
			{
				FieldInfo fieldInfo = fields[i];
				if (fieldInfo.get_FieldType().get_IsArray())
				{
					Type elementType = fieldInfo.get_FieldType().GetElementType();
					if (elementType == typeof(byte))
					{
						for (int j = 0; j < items.Length; j++)
						{
							object obj = items[j];
							if (obj != null)
							{
								byte[] array = (byte[])fieldInfo.GetValue(obj);
								byte[] array2 = TDR_StripString.strip(array);
								if (array2 != array)
								{
									fieldInfo.SetValue(obj, array2);
								}
							}
						}
					}
				}
			}
			float realtimeSinceStartup2 = Time.realtimeSinceStartup;
			TDR_StripString.time += (double)(realtimeSinceStartup2 - realtimeSinceStartup);
		}

		private static byte[] strip(byte[] src)
		{
			if (src == null || src.Length == 0)
			{
				return src;
			}
			int num = -1;
			for (int i = 0; i < src.Length; i++)
			{
				if (src[i] == 0)
				{
					num = i;
					break;
				}
			}
			if (num == -1)
			{
				return src;
			}
			byte[] array = new byte[num + 1];
			Array.Copy(src, array, num);
			array[num] = 0;
			TDR_StripString.bytes += num + 1;
			TDR_StripString.savedBytes += src.Length - num - 1;
			return array;
		}
	}
}
