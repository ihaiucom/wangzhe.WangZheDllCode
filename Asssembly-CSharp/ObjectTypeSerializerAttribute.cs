using System;

public class ObjectTypeSerializerAttribute : Attribute
{
	public Type type
	{
		get;
		private set;
	}

	public ObjectTypeSerializerAttribute(Type serializeType)
	{
		this.type = serializeType;
	}
}
