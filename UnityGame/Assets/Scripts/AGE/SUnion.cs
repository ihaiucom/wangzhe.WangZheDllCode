using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace AGE
{
	[StructLayout(LayoutKind.Explicit)]
	public struct SUnion
	{
		[FieldOffset(0)]
		public bool _bool;

		[FieldOffset(0)]
		public int _int;

		[FieldOffset(0)]
		public uint _uint;

		[FieldOffset(0)]
		public float _float;

		[FieldOffset(0)]
		public VInt3 _vint3;

		[FieldOffset(0)]
		public Vector3 _vec3;

		[FieldOffset(0)]
		public Quaternion _quat;
	}
}
