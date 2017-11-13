using System;
using System.ComponentModel;

public abstract class CheatCommandBase : ICheatCommand
{
	public static readonly string Done = "done";

	private string[] Arguments;

	public virtual bool isSupportInEditor
	{
		get
		{
			return true;
		}
	}

	public virtual bool isHiddenInMobile
	{
		get
		{
			return false;
		}
	}

	public abstract CheatCommandName command
	{
		get;
	}

	public abstract string comment
	{
		get;
	}

	public abstract ArgumentDescriptionAttribute[] argumentsTypes
	{
		get;
	}

	public abstract int messageID
	{
		get;
	}

	public virtual string fullyHelper
	{
		get
		{
			return string.Format("{0} 描述: {1}", this.description, this.comment);
		}
	}

	public virtual string[] arguments
	{
		get
		{
			return this.Arguments;
		}
	}

	public virtual string description
	{
		get
		{
			string text = string.Empty;
			if (this.argumentsTypes != null && this.argumentsTypes.Length > 0)
			{
				for (int i = 0; i < this.argumentsTypes.Length; i++)
				{
					Type argumentType = this.argumentsTypes[i].argumentType;
					text += string.Format(" <{0}{2}|{1}>", this.argumentsTypes[i].name, argumentType.get_Name(), this.argumentsTypes[i].isOptional ? "(Optional)" : string.Empty);
				}
			}
			return string.Format("{0}{1}", this.command.baseName, text);
		}
	}

	public CheatCommandBase()
	{
	}

	protected void ValidateArgumentsBuffer()
	{
		if (this.argumentsTypes != null && this.argumentsTypes.Length > 0)
		{
			this.Arguments = new string[this.argumentsTypes.Length];
			for (int i = 0; i < this.Arguments.Length; i++)
			{
				this.Arguments[i] = this.argumentsTypes[i].defaultValue;
			}
		}
	}

	private string GetArgumentNameAt(int Index)
	{
		DebugHelper.Assert(Index >= 0 && Index < this.argumentsTypes.Length);
		return this.argumentsTypes[Index].name;
	}

	protected virtual bool CheckDependencies(ArgumentDescriptionAttribute InArugmentDescription, DependencyDescription[] InDependencies, string[] InArguments, out string OutMessage)
	{
		OutMessage = string.Empty;
		if (InArguments == null)
		{
			OutMessage = "缺少所有必要参数";
			return false;
		}
		for (int i = 0; i < InDependencies.Length; i++)
		{
			DependencyDescription dependencyDescription = InDependencies[i];
			DebugHelper.Assert(dependencyDescription.dependsIndex >= 0 && dependencyDescription.dependsIndex < this.argumentsTypes.Length, "maybe internal error, can't find depend argument description.");
			if (dependencyDescription.dependsIndex < 0 || dependencyDescription.dependsIndex >= this.argumentsTypes.Length)
			{
				OutMessage = "maybe internal error, can't find depend argument description.";
				return false;
			}
			DebugHelper.Assert(dependencyDescription.dependsIndex < InArguments.Length);
			string text = InArguments[dependencyDescription.dependsIndex];
			Type argumentType = this.argumentsTypes[dependencyDescription.dependsIndex].argumentType;
			IArgumentDescription description = Singleton<ArgumentDescriptionRepository>.instance.GetDescription(argumentType);
			DebugHelper.Assert(description != null);
			text = description.GetValue(argumentType, text);
			if (dependencyDescription.ShouldBackOff(text))
			{
				OutMessage = string.Format("您必须提供参数<{2}>, 因为参数<{0}>=\"{1}\"", this.argumentsTypes[dependencyDescription.dependsIndex].name, text, InArugmentDescription.name);
				return false;
			}
		}
		return true;
	}

	public virtual bool CheckArguments(string[] InArguments, out string OutMessage)
	{
		if (this.argumentsTypes != null && this.argumentsTypes.Length > 0)
		{
			for (int i = 0; i < this.argumentsTypes.Length; i++)
			{
				ArgumentDescriptionAttribute argumentDescriptionAttribute = this.argumentsTypes[i];
				if (InArguments == null || i >= InArguments.Length)
				{
					if (!argumentDescriptionAttribute.isOptional)
					{
						OutMessage = string.Format("无法执行命令，因为缺少参数<{0}>, 类型为:<{1}>", argumentDescriptionAttribute.name, argumentDescriptionAttribute.argumentType.get_Name());
						return false;
					}
					DependencyDescription[] depends = argumentDescriptionAttribute.depends;
					if (depends != null && !this.CheckDependencies(argumentDescriptionAttribute, depends, InArguments, out OutMessage))
					{
						return false;
					}
				}
				else if (!argumentDescriptionAttribute.isOptional)
				{
					string text;
					if (!CheatCommandBase.TypeCastCheck(InArguments[i], this.argumentsTypes[i], out text))
					{
						OutMessage = string.Format("无法执行命令，因为参数[{2}]=\"{0}\"无法转换到{1}, 错误信息:{3}", new object[]
						{
							InArguments[i],
							this.argumentsTypes[i].argumentType.get_Name(),
							this.GetArgumentNameAt(i),
							text
						});
						return false;
					}
				}
				else
				{
					DependencyDescription[] depends2 = argumentDescriptionAttribute.depends;
					string text2;
					if (depends2 != null && !this.CheckDependencies(argumentDescriptionAttribute, depends2, InArguments, out OutMessage) && !CheatCommandBase.TypeCastCheck(InArguments[i], this.argumentsTypes[i], out text2))
					{
						OutMessage = string.Format("无法执行命令，因为参数[{2}]=\"{0}\"无法转换到{1}, 错误信息:{3}", new object[]
						{
							InArguments[i],
							this.argumentsTypes[i].argumentType.get_Name(),
							this.GetArgumentNameAt(i),
							text2
						});
						return false;
					}
				}
			}
		}
		OutMessage = string.Empty;
		return true;
	}

	public virtual string StartProcess(string[] InArguments)
	{
		string result;
		if (!this.CheckArguments(InArguments, out result))
		{
			return result;
		}
		return this.Execute(InArguments);
	}

	protected abstract string Execute(string[] InArguments);

	public static T SmartConvert<T>(string InArgument)
	{
		TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
		if (converter != null)
		{
			DebugHelper.Assert(converter.CanConvertFrom(typeof(string)));
			return (T)((object)converter.ConvertFrom(InArgument));
		}
		return default(T);
	}

	public static bool TypeCastCheck(string InArgument, ArgumentDescriptionAttribute InArgDescription, out string OutErrorMessage)
	{
		DebugHelper.Assert(InArgDescription != null);
		return CheatCommandBase.TypeCastCheck(InArgument, InArgDescription.argumentType, out OutErrorMessage);
	}

	public static bool TypeCastCheck(string InArgument, Type InType, out string OutErrorMessage)
	{
		IArgumentDescription description = Singleton<ArgumentDescriptionRepository>.instance.GetDescription(InType);
		DebugHelper.Assert(description != null);
		return description.CheckConvert(InArgument, InType, out OutErrorMessage);
	}

	public static int StringToEnum(string InTest, Type InEnumType)
	{
		return ArgumentDescriptionEnum.StringToEnum(InEnumType, InTest);
	}
}
