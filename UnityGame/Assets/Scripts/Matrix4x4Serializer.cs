using System;
using UnityEngine;

[ObjectTypeSerializer(typeof(Matrix4x4))]
public class Matrix4x4Serializer : UnityBasetypeSerializer, ICustomizedObjectSerializer
{
	public bool IsObjectTheSame(object o, object oPrefab)
	{
		return o == oPrefab;
	}

	public void ObjectDeserialize(ref object o, BinaryNode node)
	{
		byte[] binaryAttribute = GameSerializer.GetBinaryAttribute(node, "Value");
		Matrix4x4 matrix4x = (Matrix4x4)o;
		UnityBasetypeSerializer.BytesToMatrix4x4(ref matrix4x, binaryAttribute);
		o = matrix4x;
	}
}
