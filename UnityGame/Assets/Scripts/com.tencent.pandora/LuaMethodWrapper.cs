using System;
using System.Collections.Generic;
using System.Reflection;

namespace com.tencent.pandora
{
	internal class LuaMethodWrapper
	{
		private ObjectTranslator _Translator;

		private MethodBase _Method;

		private MethodCache _LastCalledMethod = default(MethodCache);

		private string _MethodName;

		private MemberInfo[] _Members;

		public IReflect _TargetType;

		private ExtractValue _ExtractTarget;

		private object _Target;

		private BindingFlags _BindingType;

		public LuaMethodWrapper(ObjectTranslator translator, object target, IReflect targetType, MethodBase method)
		{
			this._Translator = translator;
			this._Target = target;
			this._TargetType = targetType;
			if (targetType != null)
			{
				this._ExtractTarget = translator.typeChecker.getExtractor(targetType);
			}
			this._Method = method;
			this._MethodName = method.get_Name();
			if (method.get_IsStatic())
			{
				this._BindingType = 8;
			}
			else
			{
				this._BindingType = 4;
			}
		}

		public LuaMethodWrapper(ObjectTranslator translator, IReflect targetType, string methodName, BindingFlags bindingType)
		{
			this._Translator = translator;
			this._MethodName = methodName;
			this._TargetType = targetType;
			if (targetType != null)
			{
				this._ExtractTarget = translator.typeChecker.getExtractor(targetType);
			}
			this._BindingType = bindingType;
			this._Members = targetType.get_UnderlyingSystemType().GetMember(methodName, 8, bindingType | 16 | 1);
		}

		private int SetPendingException(Exception e)
		{
			return this._Translator.interpreter.SetPendingException(e);
		}

		private static bool IsInteger(double x)
		{
			return Math.Ceiling(x) == x;
		}

		private void ClearCachedArgs()
		{
			if (this._LastCalledMethod.args == null)
			{
				return;
			}
			for (int i = 0; i < this._LastCalledMethod.args.Length; i++)
			{
				this._LastCalledMethod.args[i] = null;
			}
		}

		public int call(IntPtr luaState)
		{
			MethodBase method = this._Method;
			object obj = this._Target;
			bool flag = true;
			int num = 0;
			if (!LuaDLL.lua_checkstack(luaState, 5))
			{
				throw new LuaException("Lua stack overflow");
			}
			bool flag2 = (this._BindingType & 8) == 8;
			this.SetPendingException(null);
			if (method == null)
			{
				if (flag2)
				{
					obj = null;
				}
				else
				{
					obj = this._ExtractTarget(luaState, 1);
				}
				if (this._LastCalledMethod.cachedMethod != null)
				{
					int num2 = flag2 ? 0 : 1;
					int num3 = LuaDLL.lua_gettop(luaState) - num2;
					MethodBase cachedMethod = this._LastCalledMethod.cachedMethod;
					if (num3 == this._LastCalledMethod.argTypes.Length)
					{
						if (!LuaDLL.lua_checkstack(luaState, this._LastCalledMethod.outList.Length + 6))
						{
							throw new LuaException("Lua stack overflow");
						}
						object[] args = this._LastCalledMethod.args;
						try
						{
							for (int i = 0; i < this._LastCalledMethod.argTypes.Length; i++)
							{
								MethodArgs methodArgs = this._LastCalledMethod.argTypes[i];
								object obj2 = methodArgs.extractValue(luaState, i + 1 + num2);
								if (this._LastCalledMethod.argTypes[i].isParamsArray)
								{
									args[methodArgs.index] = this._Translator.tableToArray(obj2, methodArgs.paramsArrayType);
								}
								else
								{
									args[methodArgs.index] = obj2;
								}
								if (args[methodArgs.index] == null && !LuaDLL.lua_isnil(luaState, i + 1 + num2))
								{
									throw new LuaException("argument number " + (i + 1) + " is invalid");
								}
							}
							if ((this._BindingType & 8) == 8)
							{
								this._Translator.push(luaState, cachedMethod.Invoke(null, args));
							}
							else if (this._LastCalledMethod.cachedMethod.get_IsConstructor())
							{
								this._Translator.push(luaState, ((ConstructorInfo)cachedMethod).Invoke(args));
							}
							else
							{
								this._Translator.push(luaState, cachedMethod.Invoke(obj, args));
							}
							flag = false;
						}
						catch (TargetInvocationException ex)
						{
							int num4 = this.SetPendingException(ex.GetBaseException());
							int result = num4;
							return result;
						}
						catch (Exception pendingException)
						{
							if (this._Members.Length == 1)
							{
								int num5 = this.SetPendingException(pendingException);
								int result = num5;
								return result;
							}
						}
					}
				}
				if (flag)
				{
					if (!flag2)
					{
						if (obj == null)
						{
							this._Translator.throwError(luaState, string.Format("instance method '{0}' requires a non null target object", this._MethodName));
							LuaDLL.lua_pushnil(luaState);
							return 1;
						}
						LuaDLL.lua_remove(luaState, 1);
					}
					bool flag3 = false;
					string text = null;
					MemberInfo[] members = this._Members;
					for (int j = 0; j < members.Length; j++)
					{
						MemberInfo memberInfo = members[j];
						text = memberInfo.get_ReflectedType().get_Name() + "." + memberInfo.get_Name();
						MethodBase method2 = (MethodInfo)memberInfo;
						bool flag4 = this._Translator.matchParameters(luaState, method2, ref this._LastCalledMethod);
						if (flag4)
						{
							flag3 = true;
							break;
						}
					}
					if (!flag3)
					{
						string message = (text == null) ? "invalid arguments to method call" : ("invalid arguments to method: " + text);
						LuaDLL.luaL_error(luaState, message);
						LuaDLL.lua_pushnil(luaState);
						this.ClearCachedArgs();
						return 1;
					}
				}
			}
			else if (method.get_ContainsGenericParameters())
			{
				this._Translator.matchParameters(luaState, method, ref this._LastCalledMethod);
				if (method.get_IsGenericMethodDefinition())
				{
					List<Type> list = new List<Type>();
					object[] args2 = this._LastCalledMethod.args;
					for (int k = 0; k < args2.Length; k++)
					{
						object obj3 = args2[k];
						list.Add(obj3.GetType());
					}
					MethodInfo methodInfo = (method as MethodInfo).MakeGenericMethod(list.ToArray());
					this._Translator.push(luaState, methodInfo.Invoke(obj, this._LastCalledMethod.args));
					flag = false;
				}
				else if (method.get_ContainsGenericParameters())
				{
					LuaDLL.luaL_error(luaState, "unable to invoke method on generic class as the current method is an open generic method");
					LuaDLL.lua_pushnil(luaState);
					this.ClearCachedArgs();
					return 1;
				}
			}
			else
			{
				if (!method.get_IsStatic() && !method.get_IsConstructor() && obj == null)
				{
					obj = this._ExtractTarget(luaState, 1);
					LuaDLL.lua_remove(luaState, 1);
				}
				if (!this._Translator.matchParameters(luaState, method, ref this._LastCalledMethod))
				{
					LuaDLL.luaL_error(luaState, "invalid arguments to method call");
					LuaDLL.lua_pushnil(luaState);
					this.ClearCachedArgs();
					return 1;
				}
			}
			if (flag)
			{
				if (!LuaDLL.lua_checkstack(luaState, this._LastCalledMethod.outList.Length + 6))
				{
					this.ClearCachedArgs();
					throw new LuaException("Lua stack overflow");
				}
				try
				{
					if (flag2)
					{
						this._Translator.push(luaState, this._LastCalledMethod.cachedMethod.Invoke(null, this._LastCalledMethod.args));
					}
					else if (this._LastCalledMethod.cachedMethod.get_IsConstructor())
					{
						this._Translator.push(luaState, ((ConstructorInfo)this._LastCalledMethod.cachedMethod).Invoke(this._LastCalledMethod.args));
					}
					else
					{
						this._Translator.push(luaState, this._LastCalledMethod.cachedMethod.Invoke(obj, this._LastCalledMethod.args));
					}
				}
				catch (TargetInvocationException ex2)
				{
					this.ClearCachedArgs();
					int num6 = this.SetPendingException(ex2.GetBaseException());
					int result = num6;
					return result;
				}
				catch (Exception pendingException2)
				{
					this.ClearCachedArgs();
					int num7 = this.SetPendingException(pendingException2);
					int result = num7;
					return result;
				}
			}
			for (int l = 0; l < this._LastCalledMethod.outList.Length; l++)
			{
				num++;
				this._Translator.push(luaState, this._LastCalledMethod.args[this._LastCalledMethod.outList[l]]);
			}
			if (!this._LastCalledMethod.IsReturnVoid && num > 0)
			{
				num++;
			}
			this.ClearCachedArgs();
			return (num < 1) ? 1 : num;
		}
	}
}
