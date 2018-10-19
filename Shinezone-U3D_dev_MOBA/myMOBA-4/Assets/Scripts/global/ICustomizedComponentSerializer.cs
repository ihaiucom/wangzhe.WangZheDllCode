using System;
using UnityEngine;

public interface ICustomizedComponentSerializer
{
	bool IsComponentSame(Component cmp, Component cmpPrefab);

	void ComponentDeserialize(Component cmp, BinaryNode node);
}
