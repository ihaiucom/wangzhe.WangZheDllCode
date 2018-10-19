using System;

public class CheatCommandName
{
	public string baseName
	{
		get;
		protected set;
	}

	public string groupName
	{
		get;
		protected set;
	}

	public string rawName
	{
		get;
		protected set;
	}

	public string[] groupHierarchies
	{
		get;
		protected set;
	}

	public CheatCommandName(string InName)
	{
		DebugHelper.Assert(!string.IsNullOrEmpty(InName));
		this.rawName = InName;
		string[] array = InName.Split(new char[]
		{
			'/'
		});
		if (array != null && array.Length > 1)
		{
			this.baseName = array[array.Length - 1];
			this.groupName = array[array.Length - 2];
			this.groupHierarchies = LinqS.Take(array, array.Length - 1);
		}
		else
		{
			this.baseName = InName;
			this.groupName = "通用";
			this.groupHierarchies = new string[]
			{
				this.groupName
			};
		}
	}
}
