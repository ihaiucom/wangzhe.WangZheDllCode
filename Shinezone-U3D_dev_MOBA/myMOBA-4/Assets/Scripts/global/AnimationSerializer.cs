using System;
using UnityEngine;

[ComponentTypeSerializer(typeof(Animation))]
public class AnimationSerializer : ICustomizedComponentSerializer
{
	private const string DOM_ANIMNAME = "AniName";

	private const string DOM_ATTR_VALUE = "Value";

	public bool IsComponentSame(Component cmp, Component cmpPrefab)
	{
		Animation animation = cmp as Animation;
		Animation animation2 = cmpPrefab as Animation;
		if (animation.GetClipCount() != animation2.GetClipCount())
		{
			return false;
		}
		foreach (AnimationState animationState in animation)
		{
			if (animation.GetClip(animationState.name) != animation2.GetClip(animationState.name))
			{
				return false;
			}
		}
		return true;
	}

	public void ComponentDeserialize(Component o, BinaryNode node)
	{
		Animation animation = o as Animation;
		for (int i = 0; i < node.GetChildNum(); i++)
		{
			BinaryNode child = node.GetChild(i);
			if (!(child.GetName() != "AniName"))
			{
				string nodeAttr = GameSerializer.GetNodeAttr(child, "Value");
				AnimationClip animationClip = (AnimationClip)GameSerializer.GetResource(nodeAttr, typeof(AnimationClip));
				if (null == animationClip)
				{
					Debug.LogError("Cannot find Animation: " + nodeAttr);
					return;
				}
				if (nodeAttr != null && nodeAttr.Length != 0)
				{
					animation.AddClip(animationClip, animationClip.name);
				}
			}
		}
	}
}
