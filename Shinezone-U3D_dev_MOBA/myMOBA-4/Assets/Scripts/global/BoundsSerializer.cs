using System;
using UnityEngine;

[ObjectTypeSerializer(typeof(Bounds))]
public class BoundsSerializer : UnityBasetypeSerializer, ICustomizedObjectSerializer
{
	public bool IsObjectTheSame(object o, object oPrefab)
	{
		return o == oPrefab;
	}

	public void ObjectDeserialize(ref object o, BinaryNode node)
	{
		byte[] binaryAttribute = GameSerializer.GetBinaryAttribute(node, "Value");
		Bounds bounds = (Bounds)o;
		UnityBasetypeSerializer.BytesToBounds(ref bounds, binaryAttribute);
		o = bounds;
	}
}
