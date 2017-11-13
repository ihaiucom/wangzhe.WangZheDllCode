using System;
using UnityEngine;

[ObjectTypeSerializer(typeof(Quaternion))]
public class QuaternionSerializer : UnityBasetypeSerializer, ICustomizedObjectSerializer
{
	public bool IsObjectTheSame(object o, object oPrefab)
	{
		return o == oPrefab;
	}

	public void ObjectDeserialize(ref object o, BinaryNode node)
	{
		byte[] binaryAttribute = GameSerializer.GetBinaryAttribute(node, "Value");
		Quaternion quaternion = (Quaternion)o;
		UnityBasetypeSerializer.BytesToQuaternion(ref quaternion, binaryAttribute);
		o = quaternion;
	}
}
