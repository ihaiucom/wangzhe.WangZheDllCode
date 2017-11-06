using System;

public class CheatCommandEntryMethodAttribute : AutoRegisterAttribute
{
	public string comment
	{
		get;
		protected set;
	}

	public bool isSupportInEditor
	{
		get;
		protected set;
	}

	public bool isHiddenInMobile
	{
		get;
		protected set;
	}

	public CheatCommandEntryMethodAttribute(string InComment, bool bInSupportInEditor, bool bHiddenInMobile = false)
	{
		this.comment = InComment;
		this.isSupportInEditor = bInSupportInEditor;
		this.isHiddenInMobile = bHiddenInMobile;
	}
}
