using CSProtocol;
using System;

public class FrameCommandClassAttribute : Attribute, IIdentifierAttribute<FRAMECMD_ID_DEF>
{

	public FRAMECMD_ID_DEF CmdID;

	public FRAMECMD_ID_DEF ID
	{
		get
		{
			return this.CmdID;
		}
	}

	public FRAMECMD_ID_DEF[] AdditionalIdList
	{
		get
		{
			return null;
		}
	}

	public FrameCommandClassAttribute(FRAMECMD_ID_DEF InCmdID)
	{
		this.CmdID = InCmdID;
	}
}
