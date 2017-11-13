using System;
using UnityEngine;

[ObjectTypeSerializer(typeof(CapsuleCollider))]
public class UnityCapsuleColliderrSerializer : ICustomizedComponentSerializer
{
	private const string XML_ATTR_CENTER = "center";

	private const string XML_ATTR_R = "radius";

	private const string XML_ATTR_H = "height";

	private const string XML_ATTR_D = "dir";

	private const string XML_IS_TRIGGER = "trigger";

	public bool IsComponentSame(Component cmp, Component cmpPrefab)
	{
		CapsuleCollider capsuleCollider = cmp as CapsuleCollider;
		CapsuleCollider capsuleCollider2 = cmpPrefab as CapsuleCollider;
		return capsuleCollider.center == capsuleCollider2.center && capsuleCollider.radius == capsuleCollider2.radius && capsuleCollider.height == capsuleCollider2.height && capsuleCollider.direction == capsuleCollider2.direction && capsuleCollider.isTrigger == capsuleCollider2.isTrigger;
	}

	public void ComponentDeserialize(Component cmp, BinaryNode node)
	{
		CapsuleCollider capsuleCollider = cmp as CapsuleCollider;
		capsuleCollider.center = UnityBasetypeSerializer.BytesToVector3(GameSerializer.GetBinaryAttribute(node, "center"));
		capsuleCollider.radius = UnityBasetypeSerializer.BytesToFloat(GameSerializer.GetBinaryAttribute(node, "radius"));
		capsuleCollider.height = UnityBasetypeSerializer.BytesToFloat(GameSerializer.GetBinaryAttribute(node, "height"));
		capsuleCollider.direction = UnityBasetypeSerializer.BytesToInt(GameSerializer.GetBinaryAttribute(node, "dir"));
		capsuleCollider.isTrigger = bool.Parse(GameSerializer.GetAttribute(node, "trigger"));
	}
}
