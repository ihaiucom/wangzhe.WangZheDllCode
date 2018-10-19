using System;
using UnityEngine;

[ComponentTypeSerializer(typeof(RectTransform))]
public class RectTransformSerializer : ICustomizedComponentSerializer
{
	public void ComponentDeserialize(Component o, BinaryNode node)
	{
		RectTransform rectTransform = o as RectTransform;
		rectTransform.localScale = new Vector3(float.Parse(GameSerializer.GetAttribute(node, "SX")), float.Parse(GameSerializer.GetAttribute(node, "SY")), float.Parse(GameSerializer.GetAttribute(node, "SZ")));
		rectTransform.localRotation = new Quaternion(float.Parse(GameSerializer.GetAttribute(node, "RX")), float.Parse(GameSerializer.GetAttribute(node, "RY")), float.Parse(GameSerializer.GetAttribute(node, "RZ")), float.Parse(GameSerializer.GetAttribute(node, "RW")));
		rectTransform.anchorMin = new Vector2(float.Parse(GameSerializer.GetAttribute(node, "anchorMinX")), float.Parse(GameSerializer.GetAttribute(node, "anchorMinY")));
		rectTransform.anchorMax = new Vector2(float.Parse(GameSerializer.GetAttribute(node, "anchorMaxX")), float.Parse(GameSerializer.GetAttribute(node, "anchorMaxY")));
		rectTransform.offsetMin = new Vector2(float.Parse(GameSerializer.GetAttribute(node, "offsetMinX")), float.Parse(GameSerializer.GetAttribute(node, "offsetMinY")));
		rectTransform.offsetMax = new Vector2(float.Parse(GameSerializer.GetAttribute(node, "offsetMaxX")), float.Parse(GameSerializer.GetAttribute(node, "offsetMaxY")));
	}

	public bool IsComponentSame(Component cmp, Component cmpPrefab)
	{
		RectTransform rectTransform = cmp as RectTransform;
		RectTransform rectTransform2 = cmpPrefab as RectTransform;
		return rectTransform.localScale == rectTransform2.localScale && rectTransform.localRotation == rectTransform2.localRotation && rectTransform.anchorMin == rectTransform2.anchorMin && rectTransform.anchorMax == rectTransform2.anchorMax && rectTransform.offsetMin == rectTransform2.offsetMin && rectTransform.offsetMax == rectTransform2.offsetMax;
	}
}
