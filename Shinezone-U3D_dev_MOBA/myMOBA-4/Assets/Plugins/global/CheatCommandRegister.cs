using System;
using System.Collections;
using System.Reflection;

public class CheatCommandRegister : Singleton<CheatCommandRegister>
{
	protected ListView<ICheatCommand> CommandRepositories = new ListView<ICheatCommand>();

	public void Register(Assembly InAssembly)
	{
		this.RegisterCommonCommands(InAssembly);
		this.RegisterMethodCommands(InAssembly);
	}

	protected void RegisterCommonCommands(Assembly InAssembly)
	{
		Type[] types = InAssembly.GetTypes();
		int num = 0;
		while (types != null && num < types.Length)
		{
			Type type = types[num];
			object[] customAttributes = type.GetCustomAttributes(typeof(CheatCommandAttribute), true);
			if (customAttributes != null)
			{
				for (int i = 0; i < customAttributes.Length; i++)
				{
					CheatCommandAttribute cheatCommandAttribute = customAttributes[i] as CheatCommandAttribute;
					if (cheatCommandAttribute != null)
					{
						this.OnFoundClass(cheatCommandAttribute.ID, type);
					}
				}
			}
			num++;
		}
	}

	protected void OnFoundClass(string InID, Type InType)
	{
		CheatCommandAttribute cheatCommandAttribute = InType.GetCustomAttributes(typeof(CheatCommandAttribute), false)[0] as CheatCommandAttribute;
		DebugHelper.Assert(cheatCommandAttribute != null);
		ICheatCommand cheatCommand = Activator.CreateInstance(InType) as ICheatCommand;
		DebugHelper.Assert(cheatCommand != null);
		this.CommandRepositories.Add(cheatCommand);
		Singleton<CheatCommandsRepository>.instance.RegisterCommand(cheatCommand);
	}

	protected void RegisterMethodCommands(Assembly InAssembly)
	{
		ClassEnumerator classEnumerator = new ClassEnumerator(typeof(CheatCommandEntryAttribute), null, InAssembly, true, false, false);
		ListView<Type>.Enumerator enumerator = classEnumerator.results.GetEnumerator();
		while (enumerator.MoveNext())
		{
			Type current = enumerator.Current;
			this.RegisterMethods(current);
		}
	}

	protected void RegisterMethods(Type InType)
	{
		CheatCommandEntryAttribute cheatCommandEntryAttribute = (CheatCommandEntryAttribute)InType.GetCustomAttributes(typeof(CheatCommandEntryAttribute), false)[0];
		DebugHelper.Assert(cheatCommandEntryAttribute != null);
		MethodInfo[] methods = InType.GetMethods();
		if (methods != null)
		{
			IEnumerator enumerator = methods.GetEnumerator();
			while (enumerator.MoveNext())
			{
				MethodInfo methodInfo = (MethodInfo)enumerator.Current;
				if (methodInfo.IsStatic)
				{
					object[] customAttributes = methodInfo.GetCustomAttributes(typeof(CheatCommandEntryMethodAttribute), false);
					if (customAttributes != null && customAttributes.Length > 0 && this.ValidateMethodArguments(methodInfo))
					{
						CheatCommandEntryMethodAttribute inMethodAttr = (CheatCommandEntryMethodAttribute)customAttributes[0];
						this.RegisterMethod(InType, cheatCommandEntryAttribute, methodInfo, inMethodAttr);
					}
				}
			}
		}
	}

	protected bool ValidateMethodArguments(MethodInfo InMethod)
	{
		Type returnType = InMethod.ReturnType;
		if (returnType != typeof(string))
		{
			DebugHelper.Assert(false, "Method Command must return a string.");
			return false;
		}
		ParameterInfo[] parameters = InMethod.GetParameters();
		if (parameters != null && parameters.Length > 0)
		{
			for (int i = 0; i < parameters.Length; i++)
			{
				ParameterInfo parameterInfo = parameters[i];
				if (parameterInfo.IsOut)
				{
					DebugHelper.Assert(false, string.Format("method command argument can't be out parameter. Method:{0}, Parameter:{1} {2}", InMethod.Name, parameterInfo.ParameterType.Name, parameterInfo.Name));
					return false;
				}
				if (parameterInfo.ParameterType.IsByRef)
				{
					DebugHelper.Assert(false, string.Format("method command argument can't be ref parameter. Method:{0}, Parameter:{1} {2}", InMethod.Name, parameterInfo.ParameterType.Name, parameterInfo.Name));
					return false;
				}
				IArgumentDescription description = Singleton<ArgumentDescriptionRepository>.instance.GetDescription(parameterInfo.ParameterType);
				DebugHelper.Assert(description != null);
				if (!description.AcceptAsMethodParameter(parameterInfo.ParameterType))
				{
					DebugHelper.Assert(false, string.Format("unsupported argument type for method command. Method:{0}, {1}, {2}", InMethod.Name, parameterInfo.ParameterType.Name, parameterInfo.Name));
					return false;
				}
			}
		}
		return true;
	}

	protected void RegisterMethod(Type InEntryType, CheatCommandEntryAttribute InEntryAttr, MethodInfo InMethod, CheatCommandEntryMethodAttribute InMethodAttr)
	{
		CheatCommandMethod inCommand = new CheatCommandMethod(InMethod, InEntryAttr, InMethodAttr);
		Singleton<CheatCommandsRepository>.instance.RegisterCommand(inCommand);
	}
}
