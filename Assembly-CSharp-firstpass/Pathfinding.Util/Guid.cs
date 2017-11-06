using System;
using System.Text;

namespace Pathfinding.Util
{
	public struct Guid
	{
		private const string hex = "0123456789ABCDEF";

		public static readonly Guid zero;

		public static readonly string zeroString;

		private ulong _a;

		private ulong _b;

		private static Random random;

		private static StringBuilder text;

		public Guid(byte[] bytes)
		{
			this._a = ((ulong)bytes[0] | (ulong)bytes[1] << 8 | (ulong)bytes[2] << 16 | (ulong)bytes[3] << 24 | (ulong)bytes[4] << 32 | (ulong)bytes[5] << 40 | (ulong)bytes[6] << 48 | (ulong)bytes[7] << 56);
			this._b = ((ulong)bytes[8] | (ulong)bytes[9] << 8 | (ulong)bytes[10] << 16 | (ulong)bytes[11] << 24 | (ulong)bytes[12] << 32 | (ulong)bytes[13] << 40 | (ulong)bytes[14] << 48 | (ulong)bytes[15] << 56);
		}

		public Guid(string str)
		{
			this._a = 0uL;
			this._b = 0uL;
			if (str.get_Length() < 32)
			{
				throw new FormatException("Invalid Guid format");
			}
			int i = 0;
			int num = 0;
			int num2 = 60;
			while (i < 16)
			{
				if (num >= str.get_Length())
				{
					throw new FormatException("Invalid Guid format. String too short");
				}
				char c = str.get_Chars(num);
				if (c != '-')
				{
					int num3 = "0123456789ABCDEF".IndexOf(char.ToUpperInvariant(c));
					if (num3 == -1)
					{
						throw new FormatException("Invalid Guid format : " + c + " is not a hexadecimal character");
					}
					this._a |= (ulong)((ulong)((long)num3) << num2);
					num2 -= 4;
					i++;
				}
				num++;
			}
			num2 = 60;
			while (i < 32)
			{
				if (num >= str.get_Length())
				{
					throw new FormatException("Invalid Guid format. String too short");
				}
				char c2 = str.get_Chars(num);
				if (c2 != '-')
				{
					int num4 = "0123456789ABCDEF".IndexOf(char.ToUpperInvariant(c2));
					if (num4 == -1)
					{
						throw new FormatException("Invalid Guid format : " + c2 + " is not a hexadecimal character");
					}
					this._b |= (ulong)((ulong)((long)num4) << num2);
					num2 -= 4;
					i++;
				}
				num++;
			}
		}

		static Guid()
		{
			Guid.zero = new Guid(new byte[16]);
			Guid guid = new Guid(new byte[16]);
			Guid.zeroString = guid.ToString();
			Guid.random = new Random();
		}

		public static Guid Parse(string input)
		{
			return new Guid(input);
		}

		public byte[] ToByteArray()
		{
			byte[] array = new byte[16];
			byte[] bytes = BitConverter.GetBytes(this._a);
			byte[] bytes2 = BitConverter.GetBytes(this._b);
			for (int i = 0; i < 8; i++)
			{
				array[i] = bytes[i];
				array[i + 8] = bytes2[i];
			}
			return array;
		}

		public static Guid NewGuid()
		{
			byte[] array = new byte[16];
			Guid.random.NextBytes(array);
			return new Guid(array);
		}

		public override bool Equals(object _rhs)
		{
			if (!(_rhs is Guid))
			{
				return false;
			}
			Guid guid = (Guid)_rhs;
			return this._a == guid._a && this._b == guid._b;
		}

		public override int GetHashCode()
		{
			ulong num = this._a ^ this._b;
			return (int)(num >> 32) ^ (int)num;
		}

		public override string ToString()
		{
			if (Guid.text == null)
			{
				Guid.text = new StringBuilder();
			}
			Guid.text.set_Length(0);
			Guid.text.Append(this._a.ToString("x16")).Append('-').Append(this._b.ToString("x16"));
			return Guid.text.ToString();
		}

		public static bool operator ==(Guid lhs, Guid rhs)
		{
			return lhs._a == rhs._a && lhs._b == rhs._b;
		}

		public static bool operator !=(Guid lhs, Guid rhs)
		{
			return lhs._a != rhs._a || lhs._b != rhs._b;
		}
	}
}
