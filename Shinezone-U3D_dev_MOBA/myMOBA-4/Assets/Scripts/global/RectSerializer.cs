using System;
using UnityEngine;

[ObjectTypeSerializer(typeof(Rect))]
public class RectSerializer : UnityBasetypeSerializer, ICustomizedObjectSerializer
{
	public bool IsObjectTheSame(object o, object oPrefab)
	{
		return o == oPrefab;
	}

	public void ObjectDeserialize(ref object o, BinaryNode node)
	{
		byte[] binaryAttribute = GameSerializer.GetBinaryAttribute(node, "Value");
		Rect rect = (Rect)o;
		UnityBasetypeSerializer.BytesToRect(ref rect, binaryAttribute);
		o = rect;
	}
}
