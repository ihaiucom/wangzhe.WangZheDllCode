using System;
using System.Reflection;

public class CheatCommandMethod : CheatCommandBase
{
	private class MethodCheatCommandName : CheatCommandName
	{
		public MethodCheatCommandName(string InName, string InBaseName) : base(InName)
		{
			base.baseName = InBaseName;
		}
	}

	private MethodInfo Method;

	private CheatCommandName CommandName;

	private CheatCommandEntryMethodAttribute MethodAttr;

	private string Comment;

	private ArgumentDescriptionAttribute[] ArgumentDescs;

	public override CheatCommandName command
	{
		get
		{
			return this.CommandName;
		}
	}

	public override string comment
	{
		get
		{
			return this.Comment;
		}
	}

	public override ArgumentDescriptionAttribute[] argumentsTypes
	{
		get
		{
			return this.ArgumentDescs;
		}
	}

	public override int messageID
	{
		get
		{
			return 0;
		}
	}

	public override bool isSupportInEditor
	{
		get
		{
			return this.MethodAttr.isSupportInEditor;
		}
	}

	public override bool isHiddenInMobile
	{
		get
		{
			return this.MethodAttr.isHiddenInMobile;
		}
	}

	public CheatCommandMethod(MethodInfo InMethod, CheatCommandEntryAttribute InEntryAttr, CheatCommandEntryMethodAttribute InMethodAttr)
	{
		this.Method = InMethod;
		this.CommandName = new CheatCommandMethod.MethodCheatCommandName(string.Format("{0}/{1}", InEntryAttr.group, InMethodAttr.comment), InMethod.Name);
		this.MethodAttr = InMethodAttr;
		string[] array = this.MethodAttr.comment.Split(new char[]
		{
			'/'
		});
		this.Comment = ((array == null || array.Length <= 0) ? this.Method.Name : array[array.Length - 1]);
		this.CacheArgumentDescriptions();
		base.ValidateArgumentsBuffer();
	}

	protected void CacheArgumentDescriptions()
	{
		ParameterInfo[] parameters = this.Method.GetParameters();
		if (parameters != null && parameters.Length > 0)
		{
			this.ArgumentDescs = new ArgumentDescriptionAttribute[parameters.Length];
			for (int i = 0; i < this.ArgumentDescs.Length; i++)
			{
				this.ArgumentDescs[i] = new ArgumentDescriptionAttribute(ArgumentDescriptionAttribute.EDefaultValueTag.Tag, i, parameters[i].ParameterType, parameters[i].Name, (parameters[i].DefaultValue == null) ? string.Empty : parameters[i].DefaultValue.ToString(), new object[0]);
			}
		}
	}

	protected override string Execute(string[] InArguments)
	{
		if (this.argumentsTypes == null || this.argumentsTypes.Length == 0)
		{
			return this.Method.Invoke(null, null) as string;
		}
		object[] array = new object[this.argumentsTypes.Length];
		for (int i = 0; i < array.Length; i++)
		{
			IArgumentDescription description = Singleton<ArgumentDescriptionRepository>.instance.GetDescription(this.argumentsTypes[i].argumentType);
			DebugHelper.Assert(description != null);
			array[i] = description.Convert(InArguments[i], this.argumentsTypes[i].argumentType);
		}
		return this.Method.Invoke(null, array) as string;
	}
}
