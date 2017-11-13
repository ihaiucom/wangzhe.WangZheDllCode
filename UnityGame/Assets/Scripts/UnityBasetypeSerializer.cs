using System;
using System.Text;
using UnityEngine;

public class UnityBasetypeSerializer
{
	protected const string XML_ATTR_VALUE = "Value";

	private const float FloatPrecision = 1000f;

	private const float FloatFactor = 0.001f;

	private static StringBuilder m_sb;

	private static StringBuilder GetStringBuilder()
	{
		if (UnityBasetypeSerializer.m_sb == null)
		{
			UnityBasetypeSerializer.m_sb = new StringBuilder();
		}
		UnityBasetypeSerializer.m_sb.set_Length(0);
		return UnityBasetypeSerializer.m_sb;
	}

	private static void WriteFloat(byte[] data, int offset, float value)
	{
		int value2 = (int)Mathf.Round(value * 1000f);
		UnityBasetypeSerializer.WriteInt(data, offset, value2);
	}

	private static void WriteInt(byte[] data, int offset, int value)
	{
		byte[] bytes = BitConverter.GetBytes(value);
		if (!BitConverter.IsLittleEndian)
		{
			Array.Reverse(bytes);
		}
		Array.Copy(bytes, 0, data, offset, 4);
	}

	private static float ReadFloat(byte[] data, int offset)
	{
		int num = UnityBasetypeSerializer.ReadInt(data, offset);
		return (float)num * 0.001f;
	}

	private static int ReadInt(byte[] data, int offset)
	{
		if (!BitConverter.IsLittleEndian)
		{
			byte[] array = new byte[4];
			Array.Copy(data, offset, array, 0, 4);
			Array.Reverse(array);
			return BitConverter.ToInt32(array, 0);
		}
		return BitConverter.ToInt32(data, offset);
	}

	public static byte[] FloatToBytes(float value)
	{
		byte[] array = new byte[4];
		UnityBasetypeSerializer.WriteFloat(array, 0, value);
		return array;
	}

	public static byte[] IntToBytes(int value)
	{
		byte[] array = new byte[4];
		UnityBasetypeSerializer.WriteInt(array, 0, value);
		return array;
	}

	public static float BytesToFloat(byte[] data)
	{
		return UnityBasetypeSerializer.ReadFloat(data, 0);
	}

	public static int BytesToInt(byte[] data)
	{
		return UnityBasetypeSerializer.ReadInt(data, 0);
	}

	public static string Vector2ToString(Vector2 vector)
	{
		return UnityBasetypeSerializer.GetStringBuilder().AppendFormat("{0:G9},{1:G9}", vector.x, vector.y).ToString();
	}

	public static byte[] Vector2ToBytes(Vector2 vector)
	{
		byte[] array = new byte[8];
		UnityBasetypeSerializer.WriteFloat(array, 0, vector.x);
		UnityBasetypeSerializer.WriteFloat(array, 4, vector.y);
		return array;
	}

	public static void StringToVector2(ref Vector2 vector, string s)
	{
		string[] array = s.Split(new char[]
		{
			','
		});
		vector.x = Convert.ToSingle(array[0]);
		vector.y = Convert.ToSingle(array[1]);
	}

	public static void BytesToVector2(ref Vector2 vector, byte[] data)
	{
		vector.x = UnityBasetypeSerializer.ReadFloat(data, 0);
		vector.y = UnityBasetypeSerializer.ReadFloat(data, 4);
	}

	public static Vector2 StringToVector2(string s)
	{
		Vector2 result = default(Vector2);
		UnityBasetypeSerializer.StringToVector2(ref result, s);
		return result;
	}

	public static Vector2 BytesToVector2(byte[] data)
	{
		Vector2 result = default(Vector2);
		UnityBasetypeSerializer.BytesToVector2(ref result, data);
		return result;
	}

	public static string Vector3ToString(Vector3 vector)
	{
		return UnityBasetypeSerializer.GetStringBuilder().AppendFormat("{0:G9},{1:G9},{2:G9}", vector.x, vector.y, vector.z).ToString();
	}

	public static byte[] Vector3ToBytes(Vector3 vector)
	{
		byte[] array = new byte[12];
		UnityBasetypeSerializer.WriteFloat(array, 0, vector.x);
		UnityBasetypeSerializer.WriteFloat(array, 4, vector.y);
		UnityBasetypeSerializer.WriteFloat(array, 8, vector.z);
		return array;
	}

	public static void StringToVector3(ref Vector3 vector, string s)
	{
		string[] array = s.Split(new char[]
		{
			','
		});
		vector.x = Convert.ToSingle(array[0]);
		vector.y = Convert.ToSingle(array[1]);
		vector.z = Convert.ToSingle(array[2]);
	}

	public static void BytesToVector3(ref Vector3 vector, byte[] data)
	{
		vector.x = UnityBasetypeSerializer.ReadFloat(data, 0);
		vector.y = UnityBasetypeSerializer.ReadFloat(data, 4);
		vector.z = UnityBasetypeSerializer.ReadFloat(data, 8);
	}

	public static Vector3 StringToVector3(string s)
	{
		Vector3 result = default(Vector3);
		UnityBasetypeSerializer.StringToVector3(ref result, s);
		return result;
	}

	public static Vector3 BytesToVector3(byte[] data)
	{
		Vector3 result = default(Vector3);
		UnityBasetypeSerializer.BytesToVector3(ref result, data);
		return result;
	}

	public static string Vector4ToString(Vector4 vector)
	{
		return UnityBasetypeSerializer.GetStringBuilder().AppendFormat("{0:G9},{1:G9},{2:G9},{3:G9}", new object[]
		{
			vector.x,
			vector.y,
			vector.z,
			vector.w
		}).ToString();
	}

	public static byte[] Vector4ToBytes(Vector4 vector)
	{
		byte[] array = new byte[16];
		UnityBasetypeSerializer.WriteFloat(array, 0, vector.x);
		UnityBasetypeSerializer.WriteFloat(array, 4, vector.y);
		UnityBasetypeSerializer.WriteFloat(array, 8, vector.z);
		UnityBasetypeSerializer.WriteFloat(array, 12, vector.w);
		return array;
	}

	public static void StringToVector4(ref Vector4 vector, string s)
	{
		string[] array = s.Split(new char[]
		{
			','
		});
		vector.x = Convert.ToSingle(array[0]);
		vector.y = Convert.ToSingle(array[1]);
		vector.z = Convert.ToSingle(array[2]);
		vector.w = Convert.ToSingle(array[3]);
	}

	public static void BytesToVector4(ref Vector4 vector, byte[] data)
	{
		vector.x = UnityBasetypeSerializer.ReadFloat(data, 0);
		vector.y = UnityBasetypeSerializer.ReadFloat(data, 4);
		vector.z = UnityBasetypeSerializer.ReadFloat(data, 8);
		vector.w = UnityBasetypeSerializer.ReadFloat(data, 12);
	}

	public static Vector4 StringToVector4(string s)
	{
		Vector4 result = default(Vector4);
		UnityBasetypeSerializer.StringToVector4(ref result, s);
		return result;
	}

	public static Vector4 BytesToVector4(byte[] data)
	{
		Vector4 result = default(Vector4);
		UnityBasetypeSerializer.BytesToVector4(ref result, data);
		return result;
	}

	public static string ColorToString(Color color)
	{
		return UnityBasetypeSerializer.GetStringBuilder().AppendFormat("{0:G9},{1:G9},{2:G9},{3:G9}", new object[]
		{
			color.r,
			color.g,
			color.b,
			color.a
		}).ToString();
	}

	public static byte[] ColorToBytes(Color color)
	{
		byte[] array = new byte[16];
		UnityBasetypeSerializer.WriteFloat(array, 0, color.r);
		UnityBasetypeSerializer.WriteFloat(array, 4, color.g);
		UnityBasetypeSerializer.WriteFloat(array, 8, color.b);
		UnityBasetypeSerializer.WriteFloat(array, 12, color.a);
		return array;
	}

	public static void StringToColor(ref Color color, string s)
	{
		string[] array = s.Split(new char[]
		{
			','
		});
		color.r = Convert.ToSingle(array[0]);
		color.g = Convert.ToSingle(array[1]);
		color.b = Convert.ToSingle(array[2]);
		color.a = Convert.ToSingle(array[3]);
	}

	public static void BytesToColor(ref Color color, byte[] data)
	{
		color.r = UnityBasetypeSerializer.ReadFloat(data, 0);
		color.g = UnityBasetypeSerializer.ReadFloat(data, 4);
		color.b = UnityBasetypeSerializer.ReadFloat(data, 8);
		color.a = UnityBasetypeSerializer.ReadFloat(data, 12);
	}

	public static Color StringToColor(string s)
	{
		Color result = default(Color);
		UnityBasetypeSerializer.StringToColor(ref result, s);
		return result;
	}

	public static Color BytesToColor(byte[] data)
	{
		Color result = default(Color);
		UnityBasetypeSerializer.BytesToColor(ref result, data);
		return result;
	}

	public static string QuaternionToString(Quaternion quaternion)
	{
		return UnityBasetypeSerializer.GetStringBuilder().AppendFormat("{0:G9},{1:G9},{2:G9},{3:G9}", new object[]
		{
			quaternion.x,
			quaternion.y,
			quaternion.z,
			quaternion.w
		}).ToString();
	}

	public static byte[] QuaternionToBytes(Quaternion quaternion)
	{
		byte[] array = new byte[16];
		UnityBasetypeSerializer.WriteFloat(array, 0, quaternion.x);
		UnityBasetypeSerializer.WriteFloat(array, 4, quaternion.y);
		UnityBasetypeSerializer.WriteFloat(array, 8, quaternion.z);
		UnityBasetypeSerializer.WriteFloat(array, 12, quaternion.w);
		return array;
	}

	public static void StringToQuaternion(ref Quaternion quaternion, string s)
	{
		string[] array = s.Split(new char[]
		{
			','
		});
		quaternion.x = Convert.ToSingle(array[0]);
		quaternion.y = Convert.ToSingle(array[1]);
		quaternion.z = Convert.ToSingle(array[2]);
		quaternion.w = Convert.ToSingle(array[3]);
	}

	public static void BytesToQuaternion(ref Quaternion quaternion, byte[] data)
	{
		quaternion.x = UnityBasetypeSerializer.ReadFloat(data, 0);
		quaternion.y = UnityBasetypeSerializer.ReadFloat(data, 4);
		quaternion.z = UnityBasetypeSerializer.ReadFloat(data, 8);
		quaternion.w = UnityBasetypeSerializer.ReadFloat(data, 12);
	}

	public static Quaternion StringToQuaternion(string s)
	{
		Quaternion result = default(Quaternion);
		UnityBasetypeSerializer.StringToQuaternion(ref result, s);
		return result;
	}

	public static Quaternion BytesToQuaternion(byte[] data)
	{
		Quaternion result = default(Quaternion);
		UnityBasetypeSerializer.BytesToQuaternion(ref result, data);
		return result;
	}

	public static string Matrix4x4ToString(Matrix4x4 matrix)
	{
		return UnityBasetypeSerializer.GetStringBuilder().AppendFormat("{0:G9},{1:G9},{2:G9},{3:G9},{4:G9},{5:G9},{6:G9},{7:G9},{8:G9},{9:G9},{10:G9},{11:G9},{12:G9},{13:G9},{14:G9},{15:G9}", new object[]
		{
			matrix.m00,
			matrix.m01,
			matrix.m02,
			matrix.m03,
			matrix.m10,
			matrix.m11,
			matrix.m12,
			matrix.m13,
			matrix.m20,
			matrix.m21,
			matrix.m22,
			matrix.m23,
			matrix.m30,
			matrix.m31,
			matrix.m32,
			matrix.m33
		}).ToString();
	}

	public static byte[] Matrix4x4ToBytes(Matrix4x4 matrix)
	{
		byte[] array = new byte[64];
		UnityBasetypeSerializer.WriteFloat(array, 0, matrix.m00);
		UnityBasetypeSerializer.WriteFloat(array, 4, matrix.m01);
		UnityBasetypeSerializer.WriteFloat(array, 8, matrix.m02);
		UnityBasetypeSerializer.WriteFloat(array, 12, matrix.m03);
		UnityBasetypeSerializer.WriteFloat(array, 16, matrix.m10);
		UnityBasetypeSerializer.WriteFloat(array, 20, matrix.m11);
		UnityBasetypeSerializer.WriteFloat(array, 24, matrix.m12);
		UnityBasetypeSerializer.WriteFloat(array, 28, matrix.m13);
		UnityBasetypeSerializer.WriteFloat(array, 32, matrix.m20);
		UnityBasetypeSerializer.WriteFloat(array, 36, matrix.m21);
		UnityBasetypeSerializer.WriteFloat(array, 40, matrix.m22);
		UnityBasetypeSerializer.WriteFloat(array, 44, matrix.m23);
		UnityBasetypeSerializer.WriteFloat(array, 48, matrix.m30);
		UnityBasetypeSerializer.WriteFloat(array, 52, matrix.m31);
		UnityBasetypeSerializer.WriteFloat(array, 56, matrix.m32);
		UnityBasetypeSerializer.WriteFloat(array, 60, matrix.m33);
		return array;
	}

	public static void StringToMatrix4x4(ref Matrix4x4 matrix, string s)
	{
		string[] array = s.Split(new char[]
		{
			','
		});
		matrix.m00 = Convert.ToSingle(array[0]);
		matrix.m01 = Convert.ToSingle(array[1]);
		matrix.m02 = Convert.ToSingle(array[2]);
		matrix.m03 = Convert.ToSingle(array[3]);
		matrix.m10 = Convert.ToSingle(array[4]);
		matrix.m11 = Convert.ToSingle(array[5]);
		matrix.m12 = Convert.ToSingle(array[6]);
		matrix.m13 = Convert.ToSingle(array[7]);
		matrix.m20 = Convert.ToSingle(array[8]);
		matrix.m21 = Convert.ToSingle(array[9]);
		matrix.m22 = Convert.ToSingle(array[10]);
		matrix.m23 = Convert.ToSingle(array[11]);
		matrix.m30 = Convert.ToSingle(array[12]);
		matrix.m31 = Convert.ToSingle(array[13]);
		matrix.m32 = Convert.ToSingle(array[14]);
		matrix.m33 = Convert.ToSingle(array[15]);
	}

	public static void BytesToMatrix4x4(ref Matrix4x4 matrix, byte[] data)
	{
		matrix.m00 = UnityBasetypeSerializer.ReadFloat(data, 0);
		matrix.m01 = UnityBasetypeSerializer.ReadFloat(data, 4);
		matrix.m02 = UnityBasetypeSerializer.ReadFloat(data, 8);
		matrix.m03 = UnityBasetypeSerializer.ReadFloat(data, 12);
		matrix.m10 = UnityBasetypeSerializer.ReadFloat(data, 16);
		matrix.m11 = UnityBasetypeSerializer.ReadFloat(data, 20);
		matrix.m12 = UnityBasetypeSerializer.ReadFloat(data, 24);
		matrix.m13 = UnityBasetypeSerializer.ReadFloat(data, 28);
		matrix.m20 = UnityBasetypeSerializer.ReadFloat(data, 32);
		matrix.m21 = UnityBasetypeSerializer.ReadFloat(data, 36);
		matrix.m22 = UnityBasetypeSerializer.ReadFloat(data, 40);
		matrix.m23 = UnityBasetypeSerializer.ReadFloat(data, 44);
		matrix.m30 = UnityBasetypeSerializer.ReadFloat(data, 48);
		matrix.m31 = UnityBasetypeSerializer.ReadFloat(data, 52);
		matrix.m32 = UnityBasetypeSerializer.ReadFloat(data, 56);
		matrix.m33 = UnityBasetypeSerializer.ReadFloat(data, 60);
	}

	public static Matrix4x4 StringToMatrix4x4(string s)
	{
		Matrix4x4 result = default(Matrix4x4);
		UnityBasetypeSerializer.StringToMatrix4x4(ref result, s);
		return result;
	}

	public static Matrix4x4 BytesToMatrix4x4(byte[] data)
	{
		Matrix4x4 result = default(Matrix4x4);
		UnityBasetypeSerializer.BytesToMatrix4x4(ref result, data);
		return result;
	}

	public static string RectToString(Rect rect)
	{
		return UnityBasetypeSerializer.GetStringBuilder().AppendFormat("{0:G9},{1:G9},{2:G9},{3:G9}", new object[]
		{
			rect.xMin,
			rect.xMax,
			rect.yMin,
			rect.yMax
		}).ToString();
	}

	public static byte[] RectToBytes(Rect rect)
	{
		byte[] array = new byte[16];
		UnityBasetypeSerializer.WriteFloat(array, 0, rect.xMin);
		UnityBasetypeSerializer.WriteFloat(array, 4, rect.xMax);
		UnityBasetypeSerializer.WriteFloat(array, 8, rect.yMin);
		UnityBasetypeSerializer.WriteFloat(array, 12, rect.yMax);
		return array;
	}

	public static void StringToRect(ref Rect rect, string s)
	{
		string[] array = s.Split(new char[]
		{
			','
		});
		rect.xMin = Convert.ToSingle(array[0]);
		rect.xMax = Convert.ToSingle(array[1]);
		rect.yMin = Convert.ToSingle(array[2]);
		rect.yMax = Convert.ToSingle(array[3]);
	}

	public static void BytesToRect(ref Rect rect, byte[] data)
	{
		rect.xMin = UnityBasetypeSerializer.ReadFloat(data, 0);
		rect.xMax = UnityBasetypeSerializer.ReadFloat(data, 4);
		rect.yMin = UnityBasetypeSerializer.ReadFloat(data, 8);
		rect.yMax = UnityBasetypeSerializer.ReadFloat(data, 12);
	}

	public static Rect StringToRect(string s)
	{
		Rect result = default(Rect);
		UnityBasetypeSerializer.StringToRect(ref result, s);
		return result;
	}

	public static Rect BytesToRect(byte[] data)
	{
		Rect result = default(Rect);
		UnityBasetypeSerializer.BytesToRect(ref result, data);
		return result;
	}

	public static string BoundsToString(Bounds bounds)
	{
		return UnityBasetypeSerializer.GetStringBuilder().AppendFormat("{0:G9},{1:G9},{2:G9},{3:G9},{4:G9},{5:G9}", new object[]
		{
			bounds.center.x,
			bounds.center.y,
			bounds.center.z,
			bounds.extents.x,
			bounds.extents.y,
			bounds.extents.z
		}).ToString();
	}

	public static byte[] BoundsToBytes(Bounds bounds)
	{
		byte[] array = new byte[24];
		UnityBasetypeSerializer.WriteFloat(array, 0, bounds.center.x);
		UnityBasetypeSerializer.WriteFloat(array, 4, bounds.center.y);
		UnityBasetypeSerializer.WriteFloat(array, 8, bounds.center.z);
		UnityBasetypeSerializer.WriteFloat(array, 12, bounds.extents.x);
		UnityBasetypeSerializer.WriteFloat(array, 16, bounds.extents.y);
		UnityBasetypeSerializer.WriteFloat(array, 20, bounds.extents.z);
		return array;
	}

	public static void StringToBounds(ref Bounds bounds, string s)
	{
		string[] array = s.Split(new char[]
		{
			','
		});
		bounds.center = new Vector3(Convert.ToSingle(array[0]), Convert.ToSingle(array[1]), Convert.ToSingle(array[2]));
		bounds.extents = new Vector3(Convert.ToSingle(array[3]), Convert.ToSingle(array[4]), Convert.ToSingle(array[5]));
	}

	public static void BytesToBounds(ref Bounds bounds, byte[] data)
	{
		bounds.center = new Vector3(UnityBasetypeSerializer.ReadFloat(data, 0), UnityBasetypeSerializer.ReadFloat(data, 4), UnityBasetypeSerializer.ReadFloat(data, 8));
		bounds.extents = new Vector3(UnityBasetypeSerializer.ReadFloat(data, 12), UnityBasetypeSerializer.ReadFloat(data, 16), UnityBasetypeSerializer.ReadFloat(data, 20));
	}

	public static Bounds StringToBounds(string s)
	{
		Bounds result = default(Bounds);
		UnityBasetypeSerializer.StringToBounds(ref result, s);
		return result;
	}

	public static Bounds BytesToBounds(byte[] data)
	{
		Bounds result = default(Bounds);
		UnityBasetypeSerializer.BytesToBounds(ref result, data);
		return result;
	}
}
