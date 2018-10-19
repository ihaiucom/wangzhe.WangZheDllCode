using System;

public interface ICustomizedObjectSerializer
{
	bool IsObjectTheSame(object o, object oPrefab);

	void ObjectDeserialize(ref object o, BinaryNode node);
}
