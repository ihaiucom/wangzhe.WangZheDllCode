using System;
using UnityEngine;

[ObjectTypeSerializer(typeof(BoxCollider))]
public class UnityBoxColliderSerializer : ICustomizedComponentSerializer
{
	private const string XML_ATTR_CENTER = "center";

	private const string XML_ATTR_SIZE = "size";

	private const string XML_IS_TRIGGER = "trigger";

	public bool IsComponentSame(Component cmp, Component cmpPrefab)
	{
		BoxCollider boxCollider = cmp as BoxCollider;
		BoxCollider boxCollider2 = cmpPrefab as BoxCollider;
		return boxCollider.center == boxCollider2.center && boxCollider.size == boxCollider2.size && boxCollider.isTrigger == boxCollider2.isTrigger;
	}

	public void ComponentDeserialize(Component cmp, BinaryNode node)
	{
		BoxCollider boxCollider = cmp as BoxCollider;
		boxCollider.center = UnityBasetypeSerializer.BytesToVector3(GameSerializer.GetBinaryAttribute(node, "center"));
		boxCollider.size = UnityBasetypeSerializer.BytesToVector3(GameSerializer.GetBinaryAttribute(node, "size"));
		boxCollider.isTrigger = bool.Parse(GameSerializer.GetAttribute(node, "trigger"));
	}
}
