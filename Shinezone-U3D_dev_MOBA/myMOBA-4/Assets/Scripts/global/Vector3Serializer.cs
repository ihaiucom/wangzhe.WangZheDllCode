using System;
using UnityEngine;

[ObjectTypeSerializer(typeof(Vector3))]
public class Vector3Serializer : UnityBasetypeSerializer, ICustomizedObjectSerializer
{
	public bool IsObjectTheSame(object o, object oPrefab)
	{
		return o == oPrefab;
	}

	public void ObjectDeserialize(ref object o, BinaryNode node)
	{
		byte[] binaryAttribute = GameSerializer.GetBinaryAttribute(node, "Value");
		Vector3 vector = (Vector3)o;
		UnityBasetypeSerializer.BytesToVector3(ref vector, binaryAttribute);
		o = vector;
	}
}
