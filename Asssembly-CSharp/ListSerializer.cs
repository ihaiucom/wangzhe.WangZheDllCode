using System;
using System.Collections;
using System.Collections.Generic;

[ObjectTypeSerializer(typeof(List))]
public class ListSerializer : ICustomizedObjectSerializer
{
	private const string DOM_NAME_ITEM = "item";

	private const string DOM_ATTR_IS_NULL = "isNull";

	public bool IsObjectTheSame(object o, object oPrefab)
	{
		if (o == null || oPrefab == null)
		{
			return false;
		}
		IList list = (IList)o;
		IList list2 = (IList)oPrefab;
		if (list.get_Count() != list2.get_Count())
		{
			return false;
		}
		for (int i = 0; i < list.get_Count(); i++)
		{
			if (list.get_Item(i) != list2.get_Item(i))
			{
				return false;
			}
		}
		return true;
	}

	public void ObjectDeserialize(ref object o, BinaryNode node)
	{
		IList list = (IList)o;
		int childNum = node.GetChildNum();
		for (int i = 0; i < childNum; i++)
		{
			list.Add(GameSerializer.GetObject(node.GetChild(i)));
		}
	}
}
