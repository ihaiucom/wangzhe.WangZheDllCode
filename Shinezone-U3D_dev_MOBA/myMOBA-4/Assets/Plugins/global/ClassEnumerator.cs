using System;
using System.Reflection;
using UnityEngine;

public class ClassEnumerator
{
	protected ListView<Type> Results = new ListView<Type>();

	private Type AttributeType;

	private Type InterfaceType;

	public ListView<Type> results
	{
		get
		{
			return this.Results;
		}
	}

	public ClassEnumerator(Type InAttributeType, Type InInterfaceType, Assembly InAssembly, bool bIgnoreAbstract = true, bool bInheritAttribute = false, bool bShouldCrossAssembly = false)
	{
		this.AttributeType = InAttributeType;
		this.InterfaceType = InInterfaceType;
		try
		{
			if (bShouldCrossAssembly)
			{
				Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
				if (assemblies != null)
				{
					for (int i = 0; i < assemblies.Length; i++)
					{
						Assembly inAssembly = assemblies[i];
						this.CheckInAssembly(inAssembly, bIgnoreAbstract, bInheritAttribute);
					}
				}
			}
			else
			{
				this.CheckInAssembly(InAssembly, bIgnoreAbstract, bInheritAttribute);
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("Error in enumerate classes :" + ex.Message);
		}
	}

	protected void CheckInAssembly(Assembly InAssembly, bool bInIgnoreAbstract, bool bInInheritAttribute)
	{
		Type[] types = InAssembly.GetTypes();
		if (types != null)
		{
			for (int i = 0; i < types.Length; i++)
			{
				Type type = types[i];
				if ((this.InterfaceType == null || this.InterfaceType.IsAssignableFrom(type)) && (!bInIgnoreAbstract || (bInIgnoreAbstract && !type.IsAbstract)) && type.GetCustomAttributes(this.AttributeType, bInInheritAttribute).Length > 0)
				{
					this.Results.Add(type);
				}
			}
		}
	}
}
