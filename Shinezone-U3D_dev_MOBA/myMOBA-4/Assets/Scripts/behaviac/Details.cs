using System;

namespace behaviac
{
	public static class Details
	{
		private interface ICompareValue
		{
			bool Greater(object lhs, object rhs);

			bool GreaterEqual(object lhs, object rhs);

			bool Less(object lhs, object rhs);

			bool LessEqual(object lhs, object rhs);
		}

		private class CompareValueInt : Details.ICompareValue
		{
			public bool Greater(object lhs, object rhs)
			{
				return (int)lhs > (int)rhs;
			}

			public bool GreaterEqual(object lhs, object rhs)
			{
				return (int)lhs >= (int)rhs;
			}

			public bool Less(object lhs, object rhs)
			{
				return (int)lhs < (int)rhs;
			}

			public bool LessEqual(object lhs, object rhs)
			{
				return (int)lhs <= (int)rhs;
			}
		}

		private class CompareValueLong : Details.ICompareValue
		{
			public bool Greater(object lhs, object rhs)
			{
				return (long)lhs > (long)rhs;
			}

			public bool GreaterEqual(object lhs, object rhs)
			{
				return (long)lhs >= (long)rhs;
			}

			public bool Less(object lhs, object rhs)
			{
				return (long)lhs < (long)rhs;
			}

			public bool LessEqual(object lhs, object rhs)
			{
				return (long)lhs <= (long)rhs;
			}
		}

		private class CompareValueShort : Details.ICompareValue
		{
			public bool Greater(object lhs, object rhs)
			{
				return (short)lhs > (short)rhs;
			}

			public bool GreaterEqual(object lhs, object rhs)
			{
				return (short)lhs >= (short)rhs;
			}

			public bool Less(object lhs, object rhs)
			{
				return (short)lhs < (short)rhs;
			}

			public bool LessEqual(object lhs, object rhs)
			{
				return (short)lhs <= (short)rhs;
			}
		}

		private class CompareValueByte : Details.ICompareValue
		{
			public bool Greater(object lhs, object rhs)
			{
				return (int)((sbyte)lhs) > (int)((sbyte)rhs);
			}

			public bool GreaterEqual(object lhs, object rhs)
			{
				return (int)((sbyte)lhs) >= (int)((sbyte)rhs);
			}

			public bool Less(object lhs, object rhs)
			{
				return (int)((sbyte)lhs) < (int)((sbyte)rhs);
			}

			public bool LessEqual(object lhs, object rhs)
			{
				return (int)((sbyte)lhs) <= (int)((sbyte)rhs);
			}
		}

		private class CompareValueFloat : Details.ICompareValue
		{
			public bool Greater(object lhs, object rhs)
			{
				return (float)lhs > (float)rhs;
			}

			public bool GreaterEqual(object lhs, object rhs)
			{
				return (float)lhs >= (float)rhs;
			}

			public bool Less(object lhs, object rhs)
			{
				return (float)lhs < (float)rhs;
			}

			public bool LessEqual(object lhs, object rhs)
			{
				return (float)lhs <= (float)rhs;
			}
		}

		private class CompareValueUInt : Details.ICompareValue
		{
			public bool Greater(object lhs, object rhs)
			{
				return (uint)lhs > (uint)rhs;
			}

			public bool GreaterEqual(object lhs, object rhs)
			{
				return (uint)lhs >= (uint)rhs;
			}

			public bool Less(object lhs, object rhs)
			{
				return (uint)lhs < (uint)rhs;
			}

			public bool LessEqual(object lhs, object rhs)
			{
				return (uint)lhs <= (uint)rhs;
			}
		}

		private class CompareValueULong : Details.ICompareValue
		{
			public bool Greater(object lhs, object rhs)
			{
				return (ulong)lhs > (ulong)rhs;
			}

			public bool GreaterEqual(object lhs, object rhs)
			{
				return (ulong)lhs >= (ulong)rhs;
			}

			public bool Less(object lhs, object rhs)
			{
				return (ulong)lhs < (ulong)rhs;
			}

			public bool LessEqual(object lhs, object rhs)
			{
				return (ulong)lhs <= (ulong)rhs;
			}
		}

		private class CompareValueUShort : Details.ICompareValue
		{
			public bool Greater(object lhs, object rhs)
			{
				return (ushort)lhs > (ushort)rhs;
			}

			public bool GreaterEqual(object lhs, object rhs)
			{
				return (ushort)lhs >= (ushort)rhs;
			}

			public bool Less(object lhs, object rhs)
			{
				return (ushort)lhs < (ushort)rhs;
			}

			public bool LessEqual(object lhs, object rhs)
			{
				return (ushort)lhs <= (ushort)rhs;
			}
		}

		private class CompareValueUByte : Details.ICompareValue
		{
			public bool Greater(object lhs, object rhs)
			{
				return (byte)lhs > (byte)rhs;
			}

			public bool GreaterEqual(object lhs, object rhs)
			{
				return (byte)lhs >= (byte)rhs;
			}

			public bool Less(object lhs, object rhs)
			{
				return (byte)lhs < (byte)rhs;
			}

			public bool LessEqual(object lhs, object rhs)
			{
				return (byte)lhs <= (byte)rhs;
			}
		}

		private class CompareValueDouble : Details.ICompareValue
		{
			public bool Greater(object lhs, object rhs)
			{
				return (double)lhs > (double)rhs;
			}

			public bool GreaterEqual(object lhs, object rhs)
			{
				return (double)lhs >= (double)rhs;
			}

			public bool Less(object lhs, object rhs)
			{
				return (double)lhs < (double)rhs;
			}

			public bool LessEqual(object lhs, object rhs)
			{
				return (double)lhs <= (double)rhs;
			}
		}

		private interface IComputeValue
		{
			object Add(object opr1, object opr2);

			object Sub(object opr1, object opr2);

			object Mul(object opr1, object opr2);

			object Div(object opr1, object opr2);
		}

		private class ComputeValueInt : Details.IComputeValue
		{
			public object Add(object lhs, object rhs)
			{
				int num = (int)lhs + (int)rhs;
				return num;
			}

			public object Sub(object lhs, object rhs)
			{
				int num = (int)lhs - (int)rhs;
				return num;
			}

			public object Mul(object lhs, object rhs)
			{
				int num = (int)lhs * (int)rhs;
				return num;
			}

			public object Div(object lhs, object rhs)
			{
				int num = (int)lhs / (int)rhs;
				return num;
			}
		}

		private class ComputeValueLong : Details.IComputeValue
		{
			public object Add(object lhs, object rhs)
			{
				long num = (long)lhs + (long)rhs;
				return num;
			}

			public object Sub(object lhs, object rhs)
			{
				long num = (long)lhs - (long)rhs;
				return num;
			}

			public object Mul(object lhs, object rhs)
			{
				long num = (long)lhs * (long)rhs;
				return num;
			}

			public object Div(object lhs, object rhs)
			{
				long num = (long)lhs / (long)rhs;
				return num;
			}
		}

		private class ComputeValueShort : Details.IComputeValue
		{
			public object Add(object lhs, object rhs)
			{
				short num = (short)((short)lhs + (short)rhs);
				return num;
			}

			public object Sub(object lhs, object rhs)
			{
				short num = (short)((short)lhs - (short)rhs);
				return num;
			}

			public object Mul(object lhs, object rhs)
			{
				short num = (short)((short)lhs * (short)rhs);
				return num;
			}

			public object Div(object lhs, object rhs)
			{
				short num = (short)((short)lhs / (short)rhs);
				return num;
			}
		}

		private class ComputeValueByte : Details.IComputeValue
		{
			public object Add(object lhs, object rhs)
			{
				sbyte b = (sbyte)((int)((sbyte)lhs) + (int)((sbyte)rhs));
				return b;
			}

			public object Sub(object lhs, object rhs)
			{
				sbyte b = (sbyte)((int)((sbyte)lhs) - (int)((sbyte)rhs));
				return b;
			}

			public object Mul(object lhs, object rhs)
			{
				sbyte b = (sbyte)((int)((sbyte)lhs) * (int)((sbyte)rhs));
				return b;
			}

			public object Div(object lhs, object rhs)
			{
				sbyte b = (sbyte)((int)((sbyte)lhs) / (int)((sbyte)rhs));
				return b;
			}
		}

		private class ComputeValueFloat : Details.IComputeValue
		{
			public object Add(object lhs, object rhs)
			{
				float num = (float)lhs + (float)rhs;
				return num;
			}

			public object Sub(object lhs, object rhs)
			{
				float num = (float)lhs - (float)rhs;
				return num;
			}

			public object Mul(object lhs, object rhs)
			{
				float num = (float)lhs * (float)rhs;
				return num;
			}

			public object Div(object lhs, object rhs)
			{
				float num = (float)lhs / (float)rhs;
				return num;
			}
		}

		private class ComputeValueUInt : Details.IComputeValue
		{
			public object Add(object lhs, object rhs)
			{
				uint num = (uint)lhs + (uint)rhs;
				return num;
			}

			public object Sub(object lhs, object rhs)
			{
				uint num = (uint)lhs - (uint)rhs;
				return num;
			}

			public object Mul(object lhs, object rhs)
			{
				uint num = (uint)lhs * (uint)rhs;
				return num;
			}

			public object Div(object lhs, object rhs)
			{
				uint num = (uint)lhs / (uint)rhs;
				return num;
			}
		}

		private class ComputeValueULong : Details.IComputeValue
		{
			public object Add(object lhs, object rhs)
			{
				ulong num = (ulong)lhs + (ulong)rhs;
				return num;
			}

			public object Sub(object lhs, object rhs)
			{
				ulong num = (ulong)lhs - (ulong)rhs;
				return num;
			}

			public object Mul(object lhs, object rhs)
			{
				ulong num = (ulong)lhs * (ulong)rhs;
				return num;
			}

			public object Div(object lhs, object rhs)
			{
				ulong num = (ulong)lhs / (ulong)rhs;
				return num;
			}
		}

		private class ComputeValueUShort : Details.IComputeValue
		{
			public object Add(object lhs, object rhs)
			{
                ushort num = (ushort)((ushort)lhs + (ushort)rhs);
				return num;
			}

			public object Sub(object lhs, object rhs)
			{
                ushort num = (ushort)((ushort)lhs - (ushort)rhs);
				return num;
			}

			public object Mul(object lhs, object rhs)
			{
                ushort num = (ushort)((ushort)lhs * (ushort)rhs);
				return num;
			}

			public object Div(object lhs, object rhs)
			{
                ushort num = (ushort)((ushort)lhs / (ushort)rhs);
				return num;
			}
		}

		private class ComputeValueUByte : Details.IComputeValue
		{
			public object Add(object lhs, object rhs)
			{
                byte b = (byte)((byte)lhs + (byte)rhs);
				return b;
			}

			public object Sub(object lhs, object rhs)
			{
                byte b = (byte)((byte)lhs - (byte)rhs);
				return b;
			}

			public object Mul(object lhs, object rhs)
			{
                byte b = (byte)((byte)lhs * (byte)rhs);
				return b;
			}

			public object Div(object lhs, object rhs)
			{
                byte b = (byte)((byte)lhs / (byte)rhs);
				return b;
			}
		}

		private class ComputeValueDouble : Details.IComputeValue
		{
			public object Add(object lhs, object rhs)
			{
				double num = (double)lhs + (double)rhs;
				return num;
			}

			public object Sub(object lhs, object rhs)
			{
				double num = (double)lhs - (double)rhs;
				return num;
			}

			public object Mul(object lhs, object rhs)
			{
				double num = (double)lhs * (double)rhs;
				return num;
			}

			public object Div(object lhs, object rhs)
			{
				double num = (double)lhs / (double)rhs;
				return num;
			}
		}

		private static DictionaryView<Type, Details.ICompareValue> ms_comparers;

		private static DictionaryView<Type, Details.IComputeValue> ms_computers;

		public static void RegisterCompareValue()
		{
			Details.ms_comparers = new DictionaryView<Type, Details.ICompareValue>();
			Details.ms_comparers[typeof(int)] = new Details.CompareValueInt();
			Details.ms_comparers[typeof(long)] = new Details.CompareValueLong();
			Details.ms_comparers[typeof(short)] = new Details.CompareValueShort();
			Details.ms_comparers[typeof(sbyte)] = new Details.CompareValueByte();
			Details.ms_comparers[typeof(float)] = new Details.CompareValueFloat();
			Details.ms_comparers[typeof(uint)] = new Details.CompareValueUInt();
			Details.ms_comparers[typeof(ulong)] = new Details.CompareValueULong();
			Details.ms_comparers[typeof(ushort)] = new Details.CompareValueUShort();
			Details.ms_comparers[typeof(byte)] = new Details.CompareValueUByte();
			Details.ms_comparers[typeof(double)] = new Details.CompareValueDouble();
		}

		public static bool Equal(object lhs, object rhs)
		{
			return object.Equals(lhs, rhs);
		}

		public static bool Greater(object lhs, object rhs)
		{
			Type type = lhs.GetType();
			if (Details.ms_comparers.ContainsKey(type))
			{
				Details.ICompareValue compareValue = Details.ms_comparers[type];
				return compareValue.Greater(lhs, rhs);
			}
			return false;
		}

		public static bool GreaterEqual(object lhs, object rhs)
		{
			Type type = lhs.GetType();
			if (Details.ms_comparers.ContainsKey(type))
			{
				Details.ICompareValue compareValue = Details.ms_comparers[type];
				return compareValue.GreaterEqual(lhs, rhs);
			}
			return false;
		}

		public static bool Less(object lhs, object rhs)
		{
			Type type = lhs.GetType();
			if (Details.ms_comparers.ContainsKey(type))
			{
				Details.ICompareValue compareValue = Details.ms_comparers[type];
				return compareValue.Less(lhs, rhs);
			}
			return false;
		}

		public static bool LessEqual(object lhs, object rhs)
		{
			Type type = lhs.GetType();
			if (Details.ms_comparers.ContainsKey(type))
			{
				Details.ICompareValue compareValue = Details.ms_comparers[type];
				return compareValue.LessEqual(lhs, rhs);
			}
			return false;
		}

		public static void RegisterComputeValue()
		{
			Details.ms_computers = new DictionaryView<Type, Details.IComputeValue>();
			Details.ms_computers[typeof(int)] = new Details.ComputeValueInt();
			Details.ms_computers[typeof(long)] = new Details.ComputeValueLong();
			Details.ms_computers[typeof(short)] = new Details.ComputeValueShort();
			Details.ms_computers[typeof(sbyte)] = new Details.ComputeValueByte();
			Details.ms_computers[typeof(float)] = new Details.ComputeValueFloat();
			Details.ms_computers[typeof(uint)] = new Details.ComputeValueUInt();
			Details.ms_computers[typeof(ulong)] = new Details.ComputeValueULong();
			Details.ms_computers[typeof(ushort)] = new Details.ComputeValueUShort();
			Details.ms_computers[typeof(byte)] = new Details.ComputeValueUByte();
			Details.ms_computers[typeof(double)] = new Details.ComputeValueDouble();
		}

		public static object ComputeValue(object value1, object value2, EComputeOperator opr)
		{
			Type type = value1.GetType();
			if (Details.ms_computers.ContainsKey(type))
			{
				Details.IComputeValue computeValue = Details.ms_computers[type];
				switch (opr)
				{
				case EComputeOperator.E_ADD:
					return computeValue.Add(value1, value2);
				case EComputeOperator.E_SUB:
					return computeValue.Sub(value1, value2);
				case EComputeOperator.E_MUL:
					return computeValue.Mul(value1, value2);
				case EComputeOperator.E_DIV:
					return computeValue.Div(value1, value2);
				}
			}
			return null;
		}
	}
}
