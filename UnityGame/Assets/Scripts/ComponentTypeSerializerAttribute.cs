using System;

public class ComponentTypeSerializerAttribute : Attribute
{
	public Type type
	{
		get;
		private set;
	}

	public ComponentTypeSerializerAttribute(Type serializeType)
	{
		this.type = serializeType;
	}
}
