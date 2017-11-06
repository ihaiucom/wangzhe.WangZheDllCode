using System;

[ObjectTypeSerializer(typeof(byte[]))]
public class ByteArraySerializer : BaseArrayTypeSerializer, ICustomInstantiate, ICustomizedObjectSerializer
{
	public object Instantiate(BinaryNode node)
	{
		int num = int.Parse(GameSerializer.GetNodeAttr(node, "Size"));
		return new byte[num];
	}

	public bool IsObjectTheSame(object o, object oPrefab)
	{
		byte[] array = (byte[])o;
		byte[] array2 = (byte[])oPrefab;
		if (array == array2)
		{
			return true;
		}
		if (array == null || array2 == null)
		{
			return false;
		}
		if (array.Length != array2.Length)
		{
			return false;
		}
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != array2[i])
			{
				return false;
			}
		}
		return true;
	}

	public void ObjectDeserialize(ref object o, BinaryNode node)
	{
		BinaryNode child = node.GetChild(0);
		o = child.GetValue();
	}
}
