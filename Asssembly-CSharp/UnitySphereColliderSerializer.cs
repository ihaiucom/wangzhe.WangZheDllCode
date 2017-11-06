using System;
using UnityEngine;

[ObjectTypeSerializer(typeof(SphereCollider))]
public class UnitySphereColliderSerializer : ICustomizedComponentSerializer
{
	private const string XML_ATTR_CENTER = "center";

	private const string XML_ATTR_R = "radius";

	private const string XML_IS_TRIGGER = "trigger";

	public bool IsComponentSame(Component cmp, Component cmpPrefab)
	{
		SphereCollider sphereCollider = cmp as SphereCollider;
		SphereCollider sphereCollider2 = cmpPrefab as SphereCollider;
		return sphereCollider.center == sphereCollider2.center && sphereCollider.radius == sphereCollider2.radius && sphereCollider.isTrigger == sphereCollider2.isTrigger;
	}

	public void ComponentDeserialize(Component cmp, BinaryNode node)
	{
		SphereCollider sphereCollider = cmp as SphereCollider;
		sphereCollider.center = UnityBasetypeSerializer.BytesToVector3(GameSerializer.GetBinaryAttribute(node, "center"));
		sphereCollider.radius = UnityBasetypeSerializer.BytesToFloat(GameSerializer.GetBinaryAttribute(node, "radius"));
		sphereCollider.isTrigger = bool.Parse(GameSerializer.GetAttribute(node, "trigger"));
	}
}
