using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace com.tencent.pandora
{
	internal class CodeGeneration
	{
		private Type eventHandlerParent = typeof(LuaEventHandler);

		private Dictionary<Type, Type> eventHandlerCollection = new Dictionary<Type, Type>();

		private Type delegateParent = typeof(LuaDelegate);

		private Dictionary<Type, Type> delegateCollection = new Dictionary<Type, Type>();

		private Type classHelper = typeof(LuaClassHelper);

		private Dictionary<Type, LuaClassType> classCollection = new Dictionary<Type, LuaClassType>();

		private AssemblyName assemblyName;

		private AssemblyBuilder newAssembly;

		private ModuleBuilder newModule;

		private int luaClassNumber = 1;

		private static readonly CodeGeneration instance;

		public static CodeGeneration Instance
		{
			get
			{
				return CodeGeneration.instance;
			}
		}

		private CodeGeneration()
		{
			this.assemblyName = new AssemblyName();
			this.assemblyName.Name = "LuaInterface_generatedcode";
			this.newAssembly = Thread.GetDomain().DefineDynamicAssembly(this.assemblyName, AssemblyBuilderAccess.Run);
			this.newModule = this.newAssembly.DefineDynamicModule("LuaInterface_generatedcode");
		}

		static CodeGeneration()
		{
			CodeGeneration.instance = new CodeGeneration();
		}

		private Type GenerateEvent(Type eventHandlerType)
		{
			string name;
			lock (this)
			{
				name = "LuaGeneratedClass" + this.luaClassNumber;
				this.luaClassNumber++;
			}
			TypeBuilder typeBuilder = this.newModule.DefineType(name, TypeAttributes.Public, this.eventHandlerParent);
			Type[] parameterTypes = new Type[]
			{
				typeof(object),
				eventHandlerType
			};
			Type typeFromHandle = typeof(void);
			MethodBuilder methodBuilder = typeBuilder.DefineMethod("HandleEvent", MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.HideBySig, typeFromHandle, parameterTypes);
			ILGenerator iLGenerator = methodBuilder.GetILGenerator();
			iLGenerator.Emit(OpCodes.Ldarg_0);
			iLGenerator.Emit(OpCodes.Ldarg_1);
			iLGenerator.Emit(OpCodes.Ldarg_2);
			MethodInfo method = this.eventHandlerParent.GetMethod("handleEvent");
			iLGenerator.Emit(OpCodes.Call, method);
			iLGenerator.Emit(OpCodes.Ret);
			return typeBuilder.CreateType();
		}

		private Type GenerateDelegate(Type delegateType)
		{
			string name;
			lock (this)
			{
				name = "LuaGeneratedClass" + this.luaClassNumber;
				this.luaClassNumber++;
			}
			TypeBuilder typeBuilder = this.newModule.DefineType(name, TypeAttributes.Public, this.delegateParent);
			MethodInfo method = delegateType.GetMethod("Invoke");
			ParameterInfo[] parameters = method.GetParameters();
			Type[] array = new Type[parameters.Length];
			Type returnType = method.ReturnType;
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = parameters[i].ParameterType;
				if (!parameters[i].IsIn && parameters[i].IsOut)
				{
					num++;
				}
				if (array[i].IsByRef)
				{
					num2++;
				}
			}
			int[] array2 = new int[num2];
			MethodBuilder methodBuilder = typeBuilder.DefineMethod("CallFunction", method.Attributes, returnType, array);
			ILGenerator iLGenerator = methodBuilder.GetILGenerator();
			iLGenerator.DeclareLocal(typeof(object[]));
			iLGenerator.DeclareLocal(typeof(object[]));
			iLGenerator.DeclareLocal(typeof(int[]));
			if (returnType != typeof(void))
			{
				iLGenerator.DeclareLocal(returnType);
			}
			else
			{
				iLGenerator.DeclareLocal(typeof(object));
			}
			iLGenerator.Emit(OpCodes.Ldc_I4, array.Length);
			iLGenerator.Emit(OpCodes.Newarr, typeof(object));
			iLGenerator.Emit(OpCodes.Stloc_0);
			iLGenerator.Emit(OpCodes.Ldc_I4, array.Length - num);
			iLGenerator.Emit(OpCodes.Newarr, typeof(object));
			iLGenerator.Emit(OpCodes.Stloc_1);
			iLGenerator.Emit(OpCodes.Ldc_I4, num2);
			iLGenerator.Emit(OpCodes.Newarr, typeof(int));
			iLGenerator.Emit(OpCodes.Stloc_2);
			int j = 0;
			int num3 = 0;
			int num4 = 0;
			while (j < array.Length)
			{
				iLGenerator.Emit(OpCodes.Ldloc_0);
				iLGenerator.Emit(OpCodes.Ldc_I4, j);
				iLGenerator.Emit(OpCodes.Ldarg, j + 1);
				if (array[j].IsByRef)
				{
					if (array[j].GetElementType().IsValueType)
					{
						iLGenerator.Emit(OpCodes.Ldobj, array[j].GetElementType());
						iLGenerator.Emit(OpCodes.Box, array[j].GetElementType());
					}
					else
					{
						iLGenerator.Emit(OpCodes.Ldind_Ref);
					}
				}
				else if (array[j].IsValueType)
				{
					iLGenerator.Emit(OpCodes.Box, array[j]);
				}
				iLGenerator.Emit(OpCodes.Stelem_Ref);
				if (array[j].IsByRef)
				{
					iLGenerator.Emit(OpCodes.Ldloc_2);
					iLGenerator.Emit(OpCodes.Ldc_I4, num4);
					iLGenerator.Emit(OpCodes.Ldc_I4, j);
					iLGenerator.Emit(OpCodes.Stelem_I4);
					array2[num4] = j;
					num4++;
				}
				if (parameters[j].IsIn || !parameters[j].IsOut)
				{
					iLGenerator.Emit(OpCodes.Ldloc_1);
					iLGenerator.Emit(OpCodes.Ldc_I4, num3);
					iLGenerator.Emit(OpCodes.Ldarg, j + 1);
					if (array[j].IsByRef)
					{
						if (array[j].GetElementType().IsValueType)
						{
							iLGenerator.Emit(OpCodes.Ldobj, array[j].GetElementType());
							iLGenerator.Emit(OpCodes.Box, array[j].GetElementType());
						}
						else
						{
							iLGenerator.Emit(OpCodes.Ldind_Ref);
						}
					}
					else if (array[j].IsValueType)
					{
						iLGenerator.Emit(OpCodes.Box, array[j]);
					}
					iLGenerator.Emit(OpCodes.Stelem_Ref);
					num3++;
				}
				j++;
			}
			iLGenerator.Emit(OpCodes.Ldarg_0);
			iLGenerator.Emit(OpCodes.Ldloc_0);
			iLGenerator.Emit(OpCodes.Ldloc_1);
			iLGenerator.Emit(OpCodes.Ldloc_2);
			MethodInfo method2 = this.delegateParent.GetMethod("callFunction");
			iLGenerator.Emit(OpCodes.Call, method2);
			if (returnType == typeof(void))
			{
				iLGenerator.Emit(OpCodes.Pop);
				iLGenerator.Emit(OpCodes.Ldnull);
			}
			else if (returnType.IsValueType)
			{
				iLGenerator.Emit(OpCodes.Unbox, returnType);
				iLGenerator.Emit(OpCodes.Ldobj, returnType);
			}
			else
			{
				iLGenerator.Emit(OpCodes.Castclass, returnType);
			}
			iLGenerator.Emit(OpCodes.Stloc_3);
			for (int k = 0; k < array2.Length; k++)
			{
				iLGenerator.Emit(OpCodes.Ldarg, array2[k] + 1);
				iLGenerator.Emit(OpCodes.Ldloc_0);
				iLGenerator.Emit(OpCodes.Ldc_I4, array2[k]);
				iLGenerator.Emit(OpCodes.Ldelem_Ref);
				if (array[array2[k]].GetElementType().IsValueType)
				{
					iLGenerator.Emit(OpCodes.Unbox, array[array2[k]].GetElementType());
					iLGenerator.Emit(OpCodes.Ldobj, array[array2[k]].GetElementType());
					iLGenerator.Emit(OpCodes.Stobj, array[array2[k]].GetElementType());
				}
				else
				{
					iLGenerator.Emit(OpCodes.Castclass, array[array2[k]].GetElementType());
					iLGenerator.Emit(OpCodes.Stind_Ref);
				}
			}
			if (returnType != typeof(void))
			{
				iLGenerator.Emit(OpCodes.Ldloc_3);
			}
			iLGenerator.Emit(OpCodes.Ret);
			return typeBuilder.CreateType();
		}

		public void GenerateClass(Type klass, out Type newType, out Type[][] returnTypes, LuaTable luaTable)
		{
			string name;
			lock (this)
			{
				name = "LuaGeneratedClass" + this.luaClassNumber;
				this.luaClassNumber++;
			}
			TypeBuilder typeBuilder;
			if (klass.IsInterface)
			{
				typeBuilder = this.newModule.DefineType(name, TypeAttributes.Public, typeof(object), new Type[]
				{
					klass,
					typeof(ILuaGeneratedType)
				});
			}
			else
			{
				typeBuilder = this.newModule.DefineType(name, TypeAttributes.Public, klass, new Type[]
				{
					typeof(ILuaGeneratedType)
				});
			}
			FieldBuilder fieldBuilder = typeBuilder.DefineField("__luaInterface_luaTable", typeof(LuaTable), FieldAttributes.Public);
			FieldBuilder fieldBuilder2 = typeBuilder.DefineField("__luaInterface_returnTypes", typeof(Type[][]), FieldAttributes.Public);
			ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[]
			{
				typeof(LuaTable),
				typeof(Type[][])
			});
			ILGenerator iLGenerator = constructorBuilder.GetILGenerator();
			iLGenerator.Emit(OpCodes.Ldarg_0);
			if (klass.IsInterface)
			{
				iLGenerator.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes));
			}
			else
			{
				iLGenerator.Emit(OpCodes.Call, klass.GetConstructor(Type.EmptyTypes));
			}
			iLGenerator.Emit(OpCodes.Ldarg_0);
			iLGenerator.Emit(OpCodes.Ldarg_1);
			iLGenerator.Emit(OpCodes.Stfld, fieldBuilder);
			iLGenerator.Emit(OpCodes.Ldarg_0);
			iLGenerator.Emit(OpCodes.Ldarg_2);
			iLGenerator.Emit(OpCodes.Stfld, fieldBuilder2);
			iLGenerator.Emit(OpCodes.Ret);
			BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
			MethodInfo[] methods = klass.GetMethods(bindingAttr);
			returnTypes = new Type[methods.Length][];
			int num = 0;
			MethodInfo[] array = methods;
			for (int i = 0; i < array.Length; i++)
			{
				MethodInfo methodInfo = array[i];
				if (klass.IsInterface)
				{
					this.GenerateMethod(typeBuilder, methodInfo, MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.VtableLayoutMask, num, fieldBuilder, fieldBuilder2, false, out returnTypes[num]);
					num++;
				}
				else if (!methodInfo.IsPrivate && !methodInfo.IsFinal && methodInfo.IsVirtual && luaTable[methodInfo.Name] != null)
				{
					this.GenerateMethod(typeBuilder, methodInfo, (methodInfo.Attributes | MethodAttributes.VtableLayoutMask) ^ MethodAttributes.VtableLayoutMask, num, fieldBuilder, fieldBuilder2, true, out returnTypes[num]);
					num++;
				}
			}
			MethodBuilder methodBuilder = typeBuilder.DefineMethod("__luaInterface_getLuaTable", MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Virtual | MethodAttributes.HideBySig, typeof(LuaTable), new Type[0]);
			typeBuilder.DefineMethodOverride(methodBuilder, typeof(ILuaGeneratedType).GetMethod("__luaInterface_getLuaTable"));
			iLGenerator = methodBuilder.GetILGenerator();
			iLGenerator.Emit(OpCodes.Ldarg_0);
			iLGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
			iLGenerator.Emit(OpCodes.Ret);
			newType = typeBuilder.CreateType();
		}

		private void GenerateMethod(TypeBuilder myType, MethodInfo method, MethodAttributes attributes, int methodIndex, FieldInfo luaTableField, FieldInfo returnTypesField, bool generateBase, out Type[] returnTypes)
		{
			ParameterInfo[] parameters = method.GetParameters();
			Type[] array = new Type[parameters.Length];
			List<Type> list = new List<Type>();
			int num = 0;
			int num2 = 0;
			Type returnType = method.ReturnType;
			list.Add(returnType);
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = parameters[i].ParameterType;
				if (!parameters[i].IsIn && parameters[i].IsOut)
				{
					num++;
				}
				if (array[i].IsByRef)
				{
					list.Add(array[i].GetElementType());
					num2++;
				}
			}
			int[] array2 = new int[num2];
			returnTypes = list.ToArray();
			if (generateBase)
			{
				string name = "__luaInterface_base_" + method.Name;
				MethodBuilder methodBuilder = myType.DefineMethod(name, MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.VtableLayoutMask, returnType, array);
				ILGenerator iLGenerator = methodBuilder.GetILGenerator();
				iLGenerator.Emit(OpCodes.Ldarg_0);
				for (int j = 0; j < array.Length; j++)
				{
					iLGenerator.Emit(OpCodes.Ldarg, j + 1);
				}
				iLGenerator.Emit(OpCodes.Call, method);
				iLGenerator.Emit(OpCodes.Ret);
			}
			MethodBuilder methodBuilder2 = myType.DefineMethod(method.Name, attributes, returnType, array);
			if (myType.BaseType.Equals(typeof(object)))
			{
				myType.DefineMethodOverride(methodBuilder2, method);
			}
			ILGenerator iLGenerator2 = methodBuilder2.GetILGenerator();
			iLGenerator2.DeclareLocal(typeof(object[]));
			iLGenerator2.DeclareLocal(typeof(object[]));
			iLGenerator2.DeclareLocal(typeof(int[]));
			if (returnType != typeof(void))
			{
				iLGenerator2.DeclareLocal(returnType);
			}
			else
			{
				iLGenerator2.DeclareLocal(typeof(object));
			}
			iLGenerator2.Emit(OpCodes.Ldc_I4, array.Length);
			iLGenerator2.Emit(OpCodes.Newarr, typeof(object));
			iLGenerator2.Emit(OpCodes.Stloc_0);
			iLGenerator2.Emit(OpCodes.Ldc_I4, array.Length - num + 1);
			iLGenerator2.Emit(OpCodes.Newarr, typeof(object));
			iLGenerator2.Emit(OpCodes.Stloc_1);
			iLGenerator2.Emit(OpCodes.Ldc_I4, num2);
			iLGenerator2.Emit(OpCodes.Newarr, typeof(int));
			iLGenerator2.Emit(OpCodes.Stloc_2);
			iLGenerator2.Emit(OpCodes.Ldloc_1);
			iLGenerator2.Emit(OpCodes.Ldc_I4_0);
			iLGenerator2.Emit(OpCodes.Ldarg_0);
			iLGenerator2.Emit(OpCodes.Ldfld, luaTableField);
			iLGenerator2.Emit(OpCodes.Stelem_Ref);
			int k = 0;
			int num3 = 1;
			int num4 = 0;
			while (k < array.Length)
			{
				iLGenerator2.Emit(OpCodes.Ldloc_0);
				iLGenerator2.Emit(OpCodes.Ldc_I4, k);
				iLGenerator2.Emit(OpCodes.Ldarg, k + 1);
				if (array[k].IsByRef)
				{
					if (array[k].GetElementType().IsValueType)
					{
						iLGenerator2.Emit(OpCodes.Ldobj, array[k].GetElementType());
						iLGenerator2.Emit(OpCodes.Box, array[k].GetElementType());
					}
					else
					{
						iLGenerator2.Emit(OpCodes.Ldind_Ref);
					}
				}
				else if (array[k].IsValueType)
				{
					iLGenerator2.Emit(OpCodes.Box, array[k]);
				}
				iLGenerator2.Emit(OpCodes.Stelem_Ref);
				if (array[k].IsByRef)
				{
					iLGenerator2.Emit(OpCodes.Ldloc_2);
					iLGenerator2.Emit(OpCodes.Ldc_I4, num4);
					iLGenerator2.Emit(OpCodes.Ldc_I4, k);
					iLGenerator2.Emit(OpCodes.Stelem_I4);
					array2[num4] = k;
					num4++;
				}
				if (parameters[k].IsIn || !parameters[k].IsOut)
				{
					iLGenerator2.Emit(OpCodes.Ldloc_1);
					iLGenerator2.Emit(OpCodes.Ldc_I4, num3);
					iLGenerator2.Emit(OpCodes.Ldarg, k + 1);
					if (array[k].IsByRef)
					{
						if (array[k].GetElementType().IsValueType)
						{
							iLGenerator2.Emit(OpCodes.Ldobj, array[k].GetElementType());
							iLGenerator2.Emit(OpCodes.Box, array[k].GetElementType());
						}
						else
						{
							iLGenerator2.Emit(OpCodes.Ldind_Ref);
						}
					}
					else if (array[k].IsValueType)
					{
						iLGenerator2.Emit(OpCodes.Box, array[k]);
					}
					iLGenerator2.Emit(OpCodes.Stelem_Ref);
					num3++;
				}
				k++;
			}
			iLGenerator2.Emit(OpCodes.Ldarg_0);
			iLGenerator2.Emit(OpCodes.Ldfld, luaTableField);
			iLGenerator2.Emit(OpCodes.Ldstr, method.Name);
			iLGenerator2.Emit(OpCodes.Call, this.classHelper.GetMethod("getTableFunction"));
			Label label = iLGenerator2.DefineLabel();
			iLGenerator2.Emit(OpCodes.Dup);
			iLGenerator2.Emit(OpCodes.Brtrue_S, label);
			iLGenerator2.Emit(OpCodes.Pop);
			if (!method.IsAbstract)
			{
				iLGenerator2.Emit(OpCodes.Ldarg_0);
				for (int l = 0; l < array.Length; l++)
				{
					iLGenerator2.Emit(OpCodes.Ldarg, l + 1);
				}
				iLGenerator2.Emit(OpCodes.Call, method);
				if (returnType == typeof(void))
				{
					iLGenerator2.Emit(OpCodes.Pop);
				}
				iLGenerator2.Emit(OpCodes.Ret);
				iLGenerator2.Emit(OpCodes.Ldnull);
			}
			else
			{
				iLGenerator2.Emit(OpCodes.Ldnull);
			}
			Label label2 = iLGenerator2.DefineLabel();
			iLGenerator2.Emit(OpCodes.Br_S, label2);
			iLGenerator2.MarkLabel(label);
			iLGenerator2.Emit(OpCodes.Ldloc_0);
			iLGenerator2.Emit(OpCodes.Ldarg_0);
			iLGenerator2.Emit(OpCodes.Ldfld, returnTypesField);
			iLGenerator2.Emit(OpCodes.Ldc_I4, methodIndex);
			iLGenerator2.Emit(OpCodes.Ldelem_Ref);
			iLGenerator2.Emit(OpCodes.Ldloc_1);
			iLGenerator2.Emit(OpCodes.Ldloc_2);
			iLGenerator2.Emit(OpCodes.Call, this.classHelper.GetMethod("callFunction"));
			iLGenerator2.MarkLabel(label2);
			if (returnType == typeof(void))
			{
				iLGenerator2.Emit(OpCodes.Pop);
				iLGenerator2.Emit(OpCodes.Ldnull);
			}
			else if (returnType.IsValueType)
			{
				iLGenerator2.Emit(OpCodes.Unbox, returnType);
				iLGenerator2.Emit(OpCodes.Ldobj, returnType);
			}
			else
			{
				iLGenerator2.Emit(OpCodes.Castclass, returnType);
			}
			iLGenerator2.Emit(OpCodes.Stloc_3);
			for (int m = 0; m < array2.Length; m++)
			{
				iLGenerator2.Emit(OpCodes.Ldarg, array2[m] + 1);
				iLGenerator2.Emit(OpCodes.Ldloc_0);
				iLGenerator2.Emit(OpCodes.Ldc_I4, array2[m]);
				iLGenerator2.Emit(OpCodes.Ldelem_Ref);
				if (array[array2[m]].GetElementType().IsValueType)
				{
					iLGenerator2.Emit(OpCodes.Unbox, array[array2[m]].GetElementType());
					iLGenerator2.Emit(OpCodes.Ldobj, array[array2[m]].GetElementType());
					iLGenerator2.Emit(OpCodes.Stobj, array[array2[m]].GetElementType());
				}
				else
				{
					iLGenerator2.Emit(OpCodes.Castclass, array[array2[m]].GetElementType());
					iLGenerator2.Emit(OpCodes.Stind_Ref);
				}
			}
			if (returnType != typeof(void))
			{
				iLGenerator2.Emit(OpCodes.Ldloc_3);
			}
			iLGenerator2.Emit(OpCodes.Ret);
		}

		public LuaEventHandler GetEvent(Type eventHandlerType, LuaFunction eventHandler)
		{
			Type type;
			if (this.eventHandlerCollection.ContainsKey(eventHandlerType))
			{
				type = this.eventHandlerCollection[eventHandlerType];
			}
			else
			{
				type = this.GenerateEvent(eventHandlerType);
				this.eventHandlerCollection[eventHandlerType] = type;
			}
			LuaEventHandler luaEventHandler = (LuaEventHandler)Activator.CreateInstance(type);
			luaEventHandler.handler = eventHandler;
			return luaEventHandler;
		}

		public Delegate GetDelegate(Type delegateType, LuaFunction luaFunc)
		{
			List<Type> list = new List<Type>();
			Type type;
			if (this.delegateCollection.ContainsKey(delegateType))
			{
				type = this.delegateCollection[delegateType];
			}
			else
			{
				type = this.GenerateDelegate(delegateType);
				this.delegateCollection[delegateType] = type;
			}
			MethodInfo method = delegateType.GetMethod("Invoke");
			list.Add(method.ReturnType);
			ParameterInfo[] parameters = method.GetParameters();
			for (int i = 0; i < parameters.Length; i++)
			{
				ParameterInfo parameterInfo = parameters[i];
				if (parameterInfo.ParameterType.IsByRef)
				{
					list.Add(parameterInfo.ParameterType);
				}
			}
			LuaDelegate luaDelegate = (LuaDelegate)Activator.CreateInstance(type);
			luaDelegate.function = luaFunc;
			luaDelegate.returnTypes = list.ToArray();
			return Delegate.CreateDelegate(delegateType, luaDelegate, "CallFunction");
		}

		public object GetClassInstance(Type klass, LuaTable luaTable)
		{
			LuaClassType value;
			if (this.classCollection.ContainsKey(klass))
			{
				value = this.classCollection[klass];
			}
			else
			{
				value = default(LuaClassType);
				this.GenerateClass(klass, out value.klass, out value.returnTypes, luaTable);
				this.classCollection[klass] = value;
			}
			return Activator.CreateInstance(value.klass, new object[]
			{
				luaTable,
				value.returnTypes
			});
		}
	}
}
