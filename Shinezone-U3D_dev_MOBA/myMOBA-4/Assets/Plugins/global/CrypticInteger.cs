using System;
using UnityEngine;

public struct CrypticInteger
{
	private int _Decrypt;

	private int _Cryptic;

	public CrypticInteger(int nValue)
	{
		this._Decrypt = UnityEngine.Random.Range(32767, 2147483647);
		this._Cryptic = (nValue ^ this._Decrypt);
	}

	public CrypticInteger(uint nValue)
	{
		this._Decrypt = UnityEngine.Random.Range(32767, 2147483647);
		this._Cryptic = (int)(nValue ^ (uint)this._Decrypt);
	}

	public static implicit operator CrypticInteger(int nValue)
	{
		return new CrypticInteger(nValue);
	}

	public static implicit operator CrypticInteger(uint nValue)
	{
		return new CrypticInteger(nValue);
	}

	public static implicit operator int(CrypticInteger inData)
	{
		return inData._Cryptic ^ inData._Decrypt;
	}

	public static implicit operator uint(CrypticInteger inData)
	{
		return (uint)(inData._Cryptic ^ inData._Decrypt);
	}

	public static implicit operator long(CrypticInteger inData)
	{
		return (long)(inData._Cryptic ^ inData._Decrypt);
	}

	public static implicit operator float(CrypticInteger inData)
	{
		return (float)(inData._Cryptic ^ inData._Decrypt);
	}

	public static explicit operator ushort(CrypticInteger inData)
	{
		return (ushort)(inData._Cryptic ^ inData._Decrypt);
	}
}
