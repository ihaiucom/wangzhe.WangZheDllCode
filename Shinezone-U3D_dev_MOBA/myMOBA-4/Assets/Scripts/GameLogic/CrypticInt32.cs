using Assets.Scripts.Framework;
using System;

namespace Assets.Scripts.GameLogic
{
	public struct CrypticInt32
	{
		private int _Decrypt;

		private int _Cryptic;

		public CrypticInt32(int nValue)
		{
			this._Decrypt = FrameRandom.GetSeed();
			this._Cryptic = (nValue ^ this._Decrypt);
		}

		public int ToInt()
		{
			return this;
		}

		public uint ToUint()
		{
			return (uint)this;
		}

		public override string ToString()
		{
			return this.ToInt().ToString();
		}

		public string ToUintString()
		{
			return this.ToUint().ToString();
		}

		public static implicit operator CrypticInt32(int nValue)
		{
			return new CrypticInt32(nValue);
		}

		public static implicit operator int(CrypticInt32 inData)
		{
			return inData._Cryptic ^ inData._Decrypt;
		}

		public static explicit operator uint(CrypticInt32 inData)
		{
			return (uint)(inData._Cryptic ^ inData._Decrypt);
		}

		public static explicit operator ushort(CrypticInt32 inData)
		{
			return (ushort)(inData._Cryptic ^ inData._Decrypt);
		}
	}
}
