using System;
using UnityEngine;

[ObjectTypeSerializer(typeof(Vector2))]
public class Vector2Serializer : UnityBasetypeSerializer, ICustomizedObjectSerializer
{
	public bool IsObjectTheSame(object o, object oPrefab)
	{
		return o == oPrefab;
	}

	public void ObjectDeserialize(ref object o, BinaryNode node)
	{
		byte[] binaryAttribute = GameSerializer.GetBinaryAttribute(node, "Value");
		Vector2 vector = default(Vector2);
		UnityBasetypeSerializer.BytesToVector2(ref vector, binaryAttribute);
		o = vector;
	}
}
