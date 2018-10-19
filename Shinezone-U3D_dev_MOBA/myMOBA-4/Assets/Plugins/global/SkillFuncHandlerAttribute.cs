using System;

public class SkillFuncHandlerAttribute : Attribute, IIdentifierAttribute<int>
{
	private int SkillFuncType;

	private int[] AddFuncTypeList;

	public int ID
	{
		get
		{
			return this.SkillFuncType;
		}
	}

	public int[] AdditionalIdList
	{
		get
		{
			return this.AddFuncTypeList;
		}
	}

	public SkillFuncHandlerAttribute(int inSkillFuncType, params int[] inAddFuncTypeList)
	{
		this.SkillFuncType = inSkillFuncType;
		this.AddFuncTypeList = inAddFuncTypeList;
	}
}
