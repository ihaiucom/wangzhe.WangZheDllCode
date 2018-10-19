using System;
using System.Collections.Generic;

namespace com.tencent.pandora
{
	public static class LuaBinder
	{
		public static List<string> wrapList = new List<string>();

		public static void Bind(IntPtr L, string type = null)
		{
			if (type == null || LuaBinder.wrapList.Contains(type))
			{
				return;
			}
			LuaBinder.wrapList.Add(type);
			type += "Wrap";
			string text = type;
			switch (text)
			{
			case "BehaviourWrap":
				BehaviourWrap.Register(L);
				break;
			case "ComponentWrap":
				ComponentWrap.Register(L);
				break;
			case "com_tencent_pandora_CSharpInterfaceWrap":
				com_tencent_pandora_CSharpInterfaceWrap.Register(L);
				break;
			case "com_tencent_pandora_DelegateFactoryWrap":
				com_tencent_pandora_DelegateFactoryWrap.Register(L);
				break;
			case "com_tencent_pandora_LoggerWrap":
				com_tencent_pandora_LoggerWrap.Register(L);
				break;
			case "com_tencent_pandora_UserDataWrap":
				com_tencent_pandora_UserDataWrap.Register(L);
				break;
			case "EnumWrap":
				EnumWrap.Register(L);
				break;
			case "GameObjectWrap":
				GameObjectWrap.Register(L);
				break;
			case "IEnumeratorWrap":
				IEnumeratorWrap.Register(L);
				break;
			case "ImageWrap":
				ImageWrap.Register(L);
				break;
			case "MonoBehaviourWrap":
				MonoBehaviourWrap.Register(L);
				break;
			case "ObjectWrap":
				ObjectWrap.Register(L);
				break;
			case "RectWrap":
				RectWrap.Register(L);
				break;
			case "stringWrap":
				stringWrap.Register(L);
				break;
			case "System_ObjectWrap":
				System_ObjectWrap.Register(L);
				break;
			case "TextureWrap":
				TextureWrap.Register(L);
				break;
			case "TimeWrap":
				TimeWrap.Register(L);
				break;
			case "TransformWrap":
				TransformWrap.Register(L);
				break;
			case "TypeWrap":
				TypeWrap.Register(L);
				break;
			case "Vector2Wrap":
				Vector2Wrap.Register(L);
				break;
			case "Vector3Wrap":
				Vector3Wrap.Register(L);
				break;
			case "WWWWrap":
				WWWWrap.Register(L);
				break;
			}
		}
	}
}
