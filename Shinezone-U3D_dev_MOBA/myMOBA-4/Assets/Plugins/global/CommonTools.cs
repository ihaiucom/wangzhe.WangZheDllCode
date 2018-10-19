using CSProtocol;
using System;

public static class CommonTools
{
	public static void FromVector3(VInt3 InVector3, ref FRAMEDT_VECTOR3 v3)
	{
		v3.X = InVector3.x;
		v3.Y = InVector3.y;
		v3.Z = InVector3.z;
	}

	public static void CSDTFromVector3(VInt3 InVector3, ref CSDT_VECTOR3 v3)
	{
		v3.X = InVector3.x;
		v3.Y = InVector3.y;
		v3.Z = InVector3.z;
	}

	public static void CSDTFromVector2(VInt2 InVector2, ref CSDT_VECTOR2 v2)
	{
		v2.X = InVector2.x;
		v2.Z = InVector2.y;
	}

	public static VInt3 ToVector3(FRAMEDT_VECTOR3 InFrameVector3)
	{
		return new VInt3(InFrameVector3.X, InFrameVector3.Y, InFrameVector3.Z);
	}

	public static VInt3 ToVector3(CSDT_VECTOR3 InFrameVector3)
	{
		return new VInt3(InFrameVector3.X, InFrameVector3.Y, InFrameVector3.Z);
	}

	public static VInt2 ToVector2(CSDT_VECTOR2 InFrameVector2)
	{
		return new VInt2(InFrameVector2.X, InFrameVector2.Z);
	}
}
