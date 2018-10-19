using System;
using System.Collections.Generic;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class CheatCommandAttribute : AutoRegisterAttribute, IIdentifierAttribute<string>
{
	private class ADAComparer : IComparer<ArgumentDescriptionAttribute>
	{
		public int Compare(ArgumentDescriptionAttribute x, ArgumentDescriptionAttribute y)
		{
			if (x.index < y.index)
			{
				return -1;
			}
			if (x.index == y.index)
			{
				return 0;
			}
			return 1;
		}
	}

	private bool bHasInitialized;

	public CheatCommandName command
	{
		get;
		protected set;
	}

	public string comment
	{
		get;
		protected set;
	}

	public ArgumentDescriptionAttribute[] argumentsTypes
	{
		get;
		protected set;
	}

	public int messageID
	{
		get;
		protected set;
	}

	public string ID
	{
		get
		{
			return this.command.baseName;
		}
	}

	public string[] AdditionalIdList
	{
		get
		{
			return null;
		}
	}

	public CheatCommandAttribute(string InExpression, string InComment, int InMessageID = 0)
	{
		this.command = new CheatCommandName(InExpression);
		this.comment = InComment;
		this.messageID = InMessageID;
	}

	internal void IndependentInitialize(object[] InReferencesArguments)
	{
		if (!this.bHasInitialized)
		{
			this.bHasInitialized = true;
			this.argumentsTypes = new ArgumentDescriptionAttribute[InReferencesArguments.Length];
			for (int i = 0; i < this.argumentsTypes.Length; i++)
			{
				this.argumentsTypes[i] = (InReferencesArguments[i] as ArgumentDescriptionAttribute);
				DebugHelper.Assert(this.argumentsTypes[i] != null);
			}
			Array.Sort<ArgumentDescriptionAttribute>(this.argumentsTypes, new CheatCommandAttribute.ADAComparer());
			for (int j = 0; j < this.argumentsTypes.Length; j++)
			{
				this.argumentsTypes[j].ValidateDependencies(j - 1);
			}
		}
	}
}
