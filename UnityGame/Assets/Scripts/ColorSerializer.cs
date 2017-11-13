using System;
using UnityEngine;

[ObjectTypeSerializer(typeof(Color))]
public class ColorSerializer : UnityBasetypeSerializer, ICustomizedObjectSerializer
{
	public bool IsObjectTheSame(object o, object oPrefab)
	{
		return o == oPrefab;
	}

	public void ObjectDeserialize(ref object o, BinaryNode node)
	{
		byte[] binaryAttribute = GameSerializer.GetBinaryAttribute(node, "Value");
		Color color = (Color)o;
		UnityBasetypeSerializer.BytesToColor(ref color, binaryAttribute);
		o = color;
	}
}
