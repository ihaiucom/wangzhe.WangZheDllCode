using System;
using UnityEngine;

[ObjectTypeSerializer(typeof(Vector4))]
public class Vector4Serializer : UnityBasetypeSerializer, ICustomizedObjectSerializer
{
	public bool IsObjectTheSame(object o, object oPrefab)
	{
		return o == oPrefab;
	}

	public void ObjectDeserialize(ref object o, BinaryNode node)
	{
		byte[] binaryAttribute = GameSerializer.GetBinaryAttribute(node, "Value");
		Vector4 vector = (Vector4)o;
		UnityBasetypeSerializer.BytesToVector4(ref vector, binaryAttribute);
		o = vector;
	}
}
