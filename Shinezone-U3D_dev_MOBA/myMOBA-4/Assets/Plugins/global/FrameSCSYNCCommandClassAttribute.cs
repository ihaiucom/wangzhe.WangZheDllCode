using CSProtocol;
using System;

public class FrameSCSYNCCommandClassAttribute : Attribute, IIdentifierAttribute<SC_FRAME_CMD_ID_DEF>
{

	public SC_FRAME_CMD_ID_DEF CmdID;

	public SC_FRAME_CMD_ID_DEF ID
	{
		get
		{
			return this.CmdID;
		}
	}

	public SC_FRAME_CMD_ID_DEF[] AdditionalIdList
	{
		get
		{
			return null;
		}
	}

	public FrameSCSYNCCommandClassAttribute(SC_FRAME_CMD_ID_DEF InCmdID)
	{
		this.CmdID = InCmdID;
	}
}
