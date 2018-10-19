using Pathfinding;
using System;
using UnityEngine;

[ComponentTypeSerializer(typeof(AstarPath))]
public class AstarPathSerializer : ICustomizedComponentSerializer
{
	private readonly string XML_ATTR_CACHE_AT_START_UP = "cache_at_start_up";

	private readonly string XML_ATTR_LOG_LEVLE = "log";

	public bool IsComponentSame(Component cmp, Component cmpPrefab)
	{
		return false;
	}

	public void ComponentDeserialize(Component o, BinaryNode node)
	{
		AstarPath astarPath = (AstarPath)o;
		astarPath.astarData = new AstarData();
		astarPath.astarData.cacheStartup = bool.Parse(GameSerializer.GetNodeAttr(node, this.XML_ATTR_CACHE_AT_START_UP));
		((AstarPath)o).logPathResults = PathLog.OnlyErrors;
		astarPath.astarData.data_cachedStartup = node.GetValue();
		astarPath.astarData.cacheStartup = true;
	}
}
