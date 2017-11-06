using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

namespace com.tencent.pandora
{
	public class MetaFunctions
	{
		internal static string luaIndexFunction = "\n        local function index(obj,name)        \n        local meta=getmetatable(obj)\n        local cached=meta.cache[name]        \n        if cached then\n           return cached\n        else\n           local value,isFunc = get_object_member(obj,name)\n           if value==nil and type(isFunc)=='string' then error(isFunc,2) end\n           if isFunc then\n            meta.cache[name]=value\n           end\n           return value\n         end\n    end\n    return index";

		private ObjectTranslator translator;

		private Hashtable memberCache = new Hashtable();

		internal LuaCSFunction gcFunction;

		internal LuaCSFunction indexFunction;

		internal LuaCSFunction newindexFunction;

		internal LuaCSFunction baseIndexFunction;

		internal LuaCSFunction classIndexFunction;

		internal LuaCSFunction classNewindexFunction;

		internal LuaCSFunction execDelegateFunction;

		internal LuaCSFunction callConstructorFunction;

		internal LuaCSFunction toStringFunction;

		public MetaFunctions(ObjectTranslator translator)
		{
			this.translator = translator;
			this.gcFunction = new LuaCSFunction(MetaFunctions.collectObject);
			this.toStringFunction = new LuaCSFunction(MetaFunctions.toString);
			this.indexFunction = new LuaCSFunction(MetaFunctions.getMethod);
			this.newindexFunction = new LuaCSFunction(MetaFunctions.setFieldOrProperty);
			this.baseIndexFunction = new LuaCSFunction(MetaFunctions.getBaseMethod);
			this.callConstructorFunction = new LuaCSFunction(MetaFunctions.callConstructor);
			this.classIndexFunction = new LuaCSFunction(MetaFunctions.getClassMethod);
			this.classNewindexFunction = new LuaCSFunction(MetaFunctions.setClassFieldOrProperty);
			this.execDelegateFunction = new LuaCSFunction(MetaFunctions.runFunctionDelegate);
		}

		[MonoPInvokeCallback(typeof(LuaCSFunction))]
		public static int runFunctionDelegate(IntPtr luaState)
		{
			ObjectTranslator objectTranslator = ObjectTranslator.FromState(luaState);
			LuaCSFunction luaCSFunction = (LuaCSFunction)objectTranslator.getRawNetObject(luaState, 1);
			LuaDLL.lua_remove(luaState, 1);
			return luaCSFunction(luaState);
		}

		[MonoPInvokeCallback(typeof(LuaCSFunction))]
		public static int collectObject(IntPtr luaState)
		{
			int num = LuaDLL.luanet_rawnetobj(luaState, 1);
			if (num != -1)
			{
				ObjectTranslator objectTranslator = ObjectTranslator.FromState(luaState);
				objectTranslator.collectObject(num);
			}
			return 0;
		}

		[MonoPInvokeCallback(typeof(LuaCSFunction))]
		public static int toString(IntPtr luaState)
		{
			ObjectTranslator objectTranslator = ObjectTranslator.FromState(luaState);
			object rawNetObject = objectTranslator.getRawNetObject(luaState, 1);
			if (rawNetObject != null)
			{
				objectTranslator.push(luaState, rawNetObject.ToString() + ": " + rawNetObject.GetHashCode());
			}
			else
			{
				LuaDLL.lua_pushnil(luaState);
			}
			return 1;
		}

		public static void dumpStack(ObjectTranslator translator, IntPtr luaState)
		{
			int num = LuaDLL.lua_gettop(luaState);
			for (int i = 1; i <= num; i++)
			{
				LuaTypes luaTypes = LuaDLL.lua_type(luaState, i);
				string arg_2E_0 = (luaTypes == LuaTypes.LUA_TTABLE) ? "table" : LuaDLL.lua_typename(luaState, luaTypes);
				string text = LuaDLL.lua_tostring(luaState, i);
				if (luaTypes == LuaTypes.LUA_TUSERDATA)
				{
					object rawNetObject = translator.getRawNetObject(luaState, i);
					text = rawNetObject.ToString();
				}
			}
		}

		[MonoPInvokeCallback(typeof(LuaCSFunction))]
		public static int getMethod(IntPtr luaState)
		{
			ObjectTranslator objectTranslator = ObjectTranslator.FromState(luaState);
			object rawNetObject = objectTranslator.getRawNetObject(luaState, 1);
			if (rawNetObject == null)
			{
				objectTranslator.throwError(luaState, "trying to index an invalid object reference");
				LuaDLL.lua_pushnil(luaState);
				return 1;
			}
			object obj = objectTranslator.getObject(luaState, 2);
			string text = obj as string;
			Type type = rawNetObject.GetType();
			try
			{
				if (text != null && objectTranslator.metaFunctions.isMemberPresent(type, text))
				{
					int member = objectTranslator.metaFunctions.getMember(luaState, type, rawNetObject, text, 5);
					int result = member;
					return result;
				}
			}
			catch
			{
			}
			bool flag = true;
			if (type.get_IsArray() && obj is double)
			{
				int num = (int)((double)obj);
				Array array = rawNetObject as Array;
				if (num >= array.get_Length())
				{
					return objectTranslator.pushError(luaState, string.Concat(new object[]
					{
						"array index out of bounds: ",
						num,
						" ",
						array.get_Length()
					}));
				}
				object value = array.GetValue(num);
				objectTranslator.push(luaState, value);
				flag = false;
			}
			else
			{
				MethodInfo[] methods = type.GetMethods();
				MethodInfo[] array2 = methods;
				for (int i = 0; i < array2.Length; i++)
				{
					MethodInfo methodInfo = array2[i];
					if (methodInfo.get_Name() == "get_Item" && methodInfo.GetParameters().Length == 1)
					{
						MethodInfo methodInfo2 = methodInfo;
						ParameterInfo[] array3 = (methodInfo2 != null) ? methodInfo2.GetParameters() : null;
						if (array3 == null || array3.Length != 1)
						{
							return objectTranslator.pushError(luaState, "method not found (or no indexer): " + obj);
						}
						obj = objectTranslator.getAsType(luaState, 2, array3[0].get_ParameterType());
						try
						{
							object o = methodInfo2.Invoke(rawNetObject, new object[]
							{
								obj
							});
							objectTranslator.push(luaState, o);
							flag = false;
						}
						catch (TargetInvocationException ex)
						{
							int result;
							int num2;
							if (ex.get_InnerException() is KeyNotFoundException)
							{
								num2 = objectTranslator.pushError(luaState, "key '" + obj + "' not found ");
								result = num2;
								return result;
							}
							num2 = objectTranslator.pushError(luaState, string.Concat(new object[]
							{
								"exception indexing '",
								obj,
								"' ",
								ex.get_Message()
							}));
							result = num2;
							return result;
						}
					}
				}
			}
			if (flag)
			{
				return objectTranslator.pushError(luaState, "cannot find " + obj);
			}
			LuaDLL.lua_pushboolean(luaState, false);
			return 2;
		}

		[MonoPInvokeCallback(typeof(LuaCSFunction))]
		public static int getBaseMethod(IntPtr luaState)
		{
			ObjectTranslator objectTranslator = ObjectTranslator.FromState(luaState);
			object rawNetObject = objectTranslator.getRawNetObject(luaState, 1);
			if (rawNetObject == null)
			{
				objectTranslator.throwError(luaState, "trying to index an invalid object reference");
				LuaDLL.lua_pushnil(luaState);
				LuaDLL.lua_pushboolean(luaState, false);
				return 2;
			}
			string text = LuaDLL.lua_tostring(luaState, 2);
			if (text == null)
			{
				LuaDLL.lua_pushnil(luaState);
				LuaDLL.lua_pushboolean(luaState, false);
				return 2;
			}
			objectTranslator.metaFunctions.getMember(luaState, rawNetObject.GetType(), rawNetObject, "__luaInterface_base_" + text, 5);
			LuaDLL.lua_settop(luaState, -2);
			if (LuaDLL.lua_type(luaState, -1) == LuaTypes.LUA_TNIL)
			{
				LuaDLL.lua_settop(luaState, -2);
				return objectTranslator.metaFunctions.getMember(luaState, rawNetObject.GetType(), rawNetObject, text, 5);
			}
			LuaDLL.lua_pushboolean(luaState, false);
			return 2;
		}

		private bool isMemberPresent(IReflect objType, string methodName)
		{
			object obj = this.checkMemberCache(this.memberCache, objType, methodName);
			if (obj != null)
			{
				return true;
			}
			MemberInfo[] member = objType.GetMember(methodName, 29);
			return member.Length > 0;
		}

		private int getMember(IntPtr luaState, IReflect objType, object obj, string methodName, BindingFlags bindingType)
		{
			bool flag = false;
			MemberInfo memberInfo = null;
			object obj2 = this.checkMemberCache(this.memberCache, objType, methodName);
			if (obj2 is LuaCSFunction)
			{
				this.translator.pushFunction(luaState, (LuaCSFunction)obj2);
				this.translator.push(luaState, true);
				return 2;
			}
			if (obj2 != null)
			{
				memberInfo = (MemberInfo)obj2;
			}
			else
			{
				MemberInfo[] member = objType.GetMember(methodName, bindingType | 16 | 1);
				if (member.Length > 0)
				{
					memberInfo = member[0];
				}
				else
				{
					member = objType.GetMember(methodName, bindingType | 8 | 16 | 1);
					if (member.Length > 0)
					{
						memberInfo = member[0];
						flag = true;
					}
				}
			}
			if (memberInfo != null)
			{
				if (memberInfo.get_MemberType() == 4)
				{
					FieldInfo fieldInfo = (FieldInfo)memberInfo;
					if (obj2 == null)
					{
						this.setMemberCache(this.memberCache, objType, methodName, memberInfo);
					}
					try
					{
						this.translator.push(luaState, fieldInfo.GetValue(obj));
					}
					catch
					{
						LuaDLL.lua_pushnil(luaState);
					}
				}
				else if (memberInfo.get_MemberType() == 16)
				{
					PropertyInfo propertyInfo = (PropertyInfo)memberInfo;
					if (obj2 == null)
					{
						this.setMemberCache(this.memberCache, objType, methodName, memberInfo);
					}
					try
					{
						object o = propertyInfo.GetGetMethod().Invoke(obj, null);
						this.translator.push(luaState, o);
					}
					catch (ArgumentException)
					{
						if (objType is Type && (Type)objType != typeof(object))
						{
							return this.getMember(luaState, ((Type)objType).get_BaseType(), obj, methodName, bindingType);
						}
						LuaDLL.lua_pushnil(luaState);
					}
					catch (TargetInvocationException e)
					{
						this.ThrowError(luaState, e);
						LuaDLL.lua_pushnil(luaState);
					}
				}
				else if (memberInfo.get_MemberType() == 2)
				{
					EventInfo eventInfo = (EventInfo)memberInfo;
					if (obj2 == null)
					{
						this.setMemberCache(this.memberCache, objType, methodName, memberInfo);
					}
					this.translator.push(luaState, new RegisterEventHandler(this.translator.pendingEvents, obj, eventInfo));
				}
				else if (!flag)
				{
					if (memberInfo.get_MemberType() != 128)
					{
						LuaCSFunction luaCSFunction = new LuaCSFunction(new LuaMethodWrapper(this.translator, objType, methodName, bindingType).call);
						if (obj2 == null)
						{
							this.setMemberCache(this.memberCache, objType, methodName, luaCSFunction);
						}
						this.translator.pushFunction(luaState, luaCSFunction);
						this.translator.push(luaState, true);
						return 2;
					}
					if (obj2 == null)
					{
						this.setMemberCache(this.memberCache, objType, methodName, memberInfo);
					}
					string name = memberInfo.get_Name();
					Type declaringType = memberInfo.get_DeclaringType();
					string className = declaringType.get_FullName() + "+" + name;
					Type t = this.translator.FindType(className);
					this.translator.pushType(luaState, t);
				}
				else
				{
					this.translator.throwError(luaState, "can't pass instance to static method " + methodName);
					LuaDLL.lua_pushnil(luaState);
				}
			}
			else
			{
				this.translator.throwError(luaState, "unknown member name " + methodName);
				LuaDLL.lua_pushnil(luaState);
			}
			this.translator.push(luaState, false);
			return 2;
		}

		private object checkMemberCache(Hashtable memberCache, IReflect objType, string memberName)
		{
			Hashtable hashtable = (Hashtable)memberCache.get_Item(objType);
			if (hashtable != null)
			{
				return hashtable.get_Item(memberName);
			}
			return null;
		}

		private void setMemberCache(Hashtable memberCache, IReflect objType, string memberName, object member)
		{
			Hashtable hashtable = (Hashtable)memberCache.get_Item(objType);
			if (hashtable == null)
			{
				hashtable = new Hashtable();
				memberCache.set_Item(objType, hashtable);
			}
			hashtable.set_Item(memberName, member);
		}

		[MonoPInvokeCallback(typeof(LuaCSFunction))]
		public static int setFieldOrProperty(IntPtr luaState)
		{
			ObjectTranslator objectTranslator = ObjectTranslator.FromState(luaState);
			object rawNetObject = objectTranslator.getRawNetObject(luaState, 1);
			if (rawNetObject == null)
			{
				objectTranslator.throwError(luaState, "trying to index and invalid object reference");
				return 0;
			}
			Type type = rawNetObject.GetType();
			string message;
			bool flag = objectTranslator.metaFunctions.trySetMember(luaState, type, rawNetObject, 5, out message);
			if (flag)
			{
				return 0;
			}
			try
			{
				if (type.get_IsArray() && LuaDLL.lua_isnumber(luaState, 2))
				{
					int num = (int)LuaDLL.lua_tonumber(luaState, 2);
					Array array = (Array)rawNetObject;
					object asType = objectTranslator.getAsType(luaState, 3, array.GetType().GetElementType());
					array.SetValue(asType, num);
				}
				else
				{
					MethodInfo method = type.GetMethod("set_Item");
					if (method != null)
					{
						ParameterInfo[] parameters = method.GetParameters();
						Type parameterType = parameters[1].get_ParameterType();
						object asType2 = objectTranslator.getAsType(luaState, 3, parameterType);
						Type parameterType2 = parameters[0].get_ParameterType();
						object asType3 = objectTranslator.getAsType(luaState, 2, parameterType2);
						method.Invoke(rawNetObject, new object[]
						{
							asType3,
							asType2
						});
					}
					else
					{
						objectTranslator.throwError(luaState, message);
					}
				}
			}
			catch (SEHException)
			{
				throw;
			}
			catch (Exception e)
			{
				objectTranslator.metaFunctions.ThrowError(luaState, e);
			}
			return 0;
		}

		private bool trySetMember(IntPtr luaState, IReflect targetType, object target, BindingFlags bindingType, out string detailMessage)
		{
			detailMessage = null;
			if (LuaDLL.lua_type(luaState, 2) != LuaTypes.LUA_TSTRING)
			{
				detailMessage = "property names must be strings";
				return false;
			}
			string text = LuaDLL.lua_tostring(luaState, 2);
			if (text == null || text.get_Length() < 1 || (!char.IsLetter(text.get_Chars(0)) && text.get_Chars(0) != '_'))
			{
				detailMessage = "invalid property name";
				return false;
			}
			MemberInfo memberInfo = (MemberInfo)this.checkMemberCache(this.memberCache, targetType, text);
			if (memberInfo == null)
			{
				MemberInfo[] member = targetType.GetMember(text, bindingType | 16 | 1);
				if (member.Length <= 0)
				{
					detailMessage = "field or property '" + text + "' does not exist";
					return false;
				}
				memberInfo = member[0];
				this.setMemberCache(this.memberCache, targetType, text, memberInfo);
			}
			if (memberInfo.get_MemberType() == 4)
			{
				FieldInfo fieldInfo = (FieldInfo)memberInfo;
				object asType = this.translator.getAsType(luaState, 3, fieldInfo.get_FieldType());
				try
				{
					fieldInfo.SetValue(target, asType);
				}
				catch (Exception e)
				{
					this.ThrowError(luaState, e);
				}
				return true;
			}
			if (memberInfo.get_MemberType() == 16)
			{
				PropertyInfo propertyInfo = (PropertyInfo)memberInfo;
				object asType2 = this.translator.getAsType(luaState, 3, propertyInfo.get_PropertyType());
				try
				{
					propertyInfo.SetValue(target, asType2, null);
				}
				catch (Exception e2)
				{
					this.ThrowError(luaState, e2);
				}
				return true;
			}
			detailMessage = "'" + text + "' is not a .net field or property";
			return false;
		}

		private int setMember(IntPtr luaState, IReflect targetType, object target, BindingFlags bindingType)
		{
			string message;
			if (!this.trySetMember(luaState, targetType, target, bindingType, out message))
			{
				this.translator.throwError(luaState, message);
			}
			return 0;
		}

		private void ThrowError(IntPtr luaState, Exception e)
		{
			TargetInvocationException ex = e as TargetInvocationException;
			if (ex != null)
			{
				e = ex.get_InnerException();
			}
			this.translator.throwError(luaState, e.get_Message());
		}

		[MonoPInvokeCallback(typeof(LuaCSFunction))]
		public static int getClassMethod(IntPtr luaState)
		{
			ObjectTranslator objectTranslator = ObjectTranslator.FromState(luaState);
			object rawNetObject = objectTranslator.getRawNetObject(luaState, 1);
			if (rawNetObject == null || !(rawNetObject is IReflect))
			{
				objectTranslator.throwError(luaState, "trying to index an invalid type reference");
				LuaDLL.lua_pushnil(luaState);
				return 1;
			}
			IReflect reflect = (IReflect)rawNetObject;
			if (LuaDLL.lua_isnumber(luaState, 2))
			{
				int num = (int)LuaDLL.lua_tonumber(luaState, 2);
				objectTranslator.push(luaState, Array.CreateInstance(reflect.get_UnderlyingSystemType(), num));
				return 1;
			}
			string text = LuaDLL.lua_tostring(luaState, 2);
			if (text == null)
			{
				LuaDLL.lua_pushnil(luaState);
				return 1;
			}
			return objectTranslator.metaFunctions.getMember(luaState, reflect, null, text, 73);
		}

		[MonoPInvokeCallback(typeof(LuaCSFunction))]
		public static int setClassFieldOrProperty(IntPtr luaState)
		{
			ObjectTranslator objectTranslator = ObjectTranslator.FromState(luaState);
			object rawNetObject = objectTranslator.getRawNetObject(luaState, 1);
			if (rawNetObject == null || !(rawNetObject is IReflect))
			{
				objectTranslator.throwError(luaState, "trying to index an invalid type reference");
				return 0;
			}
			IReflect targetType = (IReflect)rawNetObject;
			return objectTranslator.metaFunctions.setMember(luaState, targetType, null, 73);
		}

		[MonoPInvokeCallback(typeof(LuaCSFunction))]
		public static int callConstructor(IntPtr luaState)
		{
			ObjectTranslator objectTranslator = ObjectTranslator.FromState(luaState);
			MethodCache methodCache = default(MethodCache);
			object rawNetObject = objectTranslator.getRawNetObject(luaState, 1);
			if (rawNetObject == null || !(rawNetObject is IReflect))
			{
				LuaDLL.luaL_error(luaState, "trying to call constructor on an invalid type reference");
				LuaDLL.lua_pushnil(luaState);
				return 1;
			}
			IReflect reflect = (IReflect)rawNetObject;
			LuaDLL.lua_remove(luaState, 1);
			ConstructorInfo[] constructors = reflect.get_UnderlyingSystemType().GetConstructors();
			ConstructorInfo[] array = constructors;
			for (int i = 0; i < array.Length; i++)
			{
				ConstructorInfo constructorInfo = array[i];
				bool flag = objectTranslator.metaFunctions.matchParameters(luaState, constructorInfo, ref methodCache);
				if (flag)
				{
					try
					{
						objectTranslator.push(luaState, constructorInfo.Invoke(methodCache.args));
					}
					catch (TargetInvocationException e)
					{
						objectTranslator.metaFunctions.ThrowError(luaState, e);
						LuaDLL.lua_pushnil(luaState);
					}
					catch
					{
						LuaDLL.lua_pushnil(luaState);
					}
					return 1;
				}
			}
			string text = (constructors.Length == 0) ? "unknown" : constructors[0].get_Name();
			LuaDLL.luaL_error(luaState, string.Format("{0} does not contain constructor({1}) argument match", reflect.get_UnderlyingSystemType(), text));
			LuaDLL.lua_pushnil(luaState);
			return 1;
		}

		private static bool IsInteger(double x)
		{
			return Math.Ceiling(x) == x;
		}

		internal Array TableToArray(object luaParamValue, Type paramArrayType)
		{
			Array array;
			if (luaParamValue is LuaTable)
			{
				LuaTable luaTable = (LuaTable)luaParamValue;
				IDictionaryEnumerator enumerator = luaTable.GetEnumerator();
				enumerator.Reset();
				array = Array.CreateInstance(paramArrayType, luaTable.Values.get_Count());
				int num = 0;
				while (enumerator.MoveNext())
				{
					object obj = enumerator.get_Value();
					if (paramArrayType == typeof(object) && obj != null && obj.GetType() == typeof(double) && MetaFunctions.IsInteger((double)obj))
					{
						obj = Convert.ToInt32((double)obj);
					}
					array.SetValue(Convert.ChangeType(obj, paramArrayType), num);
					num++;
				}
			}
			else
			{
				array = Array.CreateInstance(paramArrayType, 1);
				array.SetValue(luaParamValue, 0);
			}
			return array;
		}

		internal bool matchParameters(IntPtr luaState, MethodBase method, ref MethodCache methodCache)
		{
			bool flag = true;
			ParameterInfo[] parameters = method.GetParameters();
			int num = 1;
			int num2 = LuaDLL.lua_gettop(luaState);
			ArrayList arrayList = new ArrayList();
			List<int> list = new List<int>();
			List<MethodArgs> list2 = new List<MethodArgs>();
			ParameterInfo[] array = parameters;
			for (int i = 0; i < array.Length; i++)
			{
				ParameterInfo parameterInfo = array[i];
				ExtractValue extractValue;
				if (!parameterInfo.get_IsIn() && parameterInfo.get_IsOut())
				{
					list.Add(arrayList.Add(null));
				}
				else if (num > num2)
				{
					if (!parameterInfo.get_IsOptional())
					{
						flag = false;
						break;
					}
					arrayList.Add(parameterInfo.get_DefaultValue());
				}
				else if (this._IsTypeCorrect(luaState, num, parameterInfo, out extractValue))
				{
					int num3 = arrayList.Add(extractValue(luaState, num));
					list2.Add(new MethodArgs
					{
						index = num3,
						extractValue = extractValue
					});
					if (parameterInfo.get_ParameterType().get_IsByRef())
					{
						list.Add(num3);
					}
					num++;
				}
				else if (this._IsParamsArray(luaState, num, parameterInfo, out extractValue))
				{
					object luaParamValue = extractValue(luaState, num);
					Type elementType = parameterInfo.get_ParameterType().GetElementType();
					Array array2 = this.TableToArray(luaParamValue, elementType);
					int index = arrayList.Add(array2);
					list2.Add(new MethodArgs
					{
						index = index,
						extractValue = extractValue,
						isParamsArray = true,
						paramsArrayType = elementType
					});
					num++;
				}
				else
				{
					if (!parameterInfo.get_IsOptional())
					{
						flag = false;
						break;
					}
					arrayList.Add(parameterInfo.get_DefaultValue());
				}
			}
			if (num != num2 + 1)
			{
				flag = false;
			}
			if (flag)
			{
				methodCache.args = arrayList.ToArray();
				methodCache.cachedMethod = method;
				methodCache.outList = list.ToArray();
				methodCache.argTypes = list2.ToArray();
			}
			return flag;
		}

		private bool _IsTypeCorrect(IntPtr luaState, int currentLuaParam, ParameterInfo currentNetParam, out ExtractValue extractValue)
		{
			bool result;
			try
			{
				ExtractValue extractValue2;
				extractValue = (extractValue2 = this.translator.typeChecker.checkType(luaState, currentLuaParam, currentNetParam.get_ParameterType()));
				result = (extractValue2 != null);
			}
			catch
			{
				extractValue = null;
				result = false;
			}
			return result;
		}

		private bool _IsParamsArray(IntPtr luaState, int currentLuaParam, ParameterInfo currentNetParam, out ExtractValue extractValue)
		{
			extractValue = null;
			if (currentNetParam.GetCustomAttributes(typeof(ParamArrayAttribute), false).Length > 0)
			{
				LuaTypes luaTypes;
				try
				{
					luaTypes = LuaDLL.lua_type(luaState, currentLuaParam);
				}
				catch (Exception var_2_2A)
				{
					extractValue = null;
					bool flag = false;
					bool result = flag;
					return result;
				}
				if (luaTypes == LuaTypes.LUA_TTABLE)
				{
					try
					{
						extractValue = this.translator.typeChecker.getExtractor(typeof(LuaTable));
					}
					catch (Exception var_3_67)
					{
					}
					return extractValue != null;
				}
				Type elementType = currentNetParam.get_ParameterType().GetElementType();
				try
				{
					extractValue = this.translator.typeChecker.checkType(luaState, currentLuaParam, elementType);
				}
				catch (Exception var_5_A2)
				{
				}
				return extractValue != null;
			}
			return false;
		}
	}
}
