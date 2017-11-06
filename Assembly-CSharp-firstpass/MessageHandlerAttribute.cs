using System;

public class MessageHandlerAttribute : Attribute, IIdentifierAttribute<uint>
{
	public int MessageID;

	public uint ID
	{
		get
		{
			return (uint)this.MessageID;
		}
	}

	public uint[] AdditionalIdList
	{
		get
		{
			return null;
		}
	}

	public MessageHandlerAttribute(int InMessageID)
	{
		this.MessageID = InMessageID;
	}
}
