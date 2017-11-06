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
			string text2 = text;
			if (text2 != null)
			{
				if (LuaBinder.<>f__switch$map4 == null)
				{
					Dictionary<string, int> dictionary = new Dictionary<string, int>(22);
					dictionary.Add("BehaviourWrap", 0);
					dictionary.Add("ComponentWrap", 1);
					dictionary.Add("com_tencent_pandora_CSharpInterfaceWrap", 2);
					dictionary.Add("com_tencent_pandora_DelegateFactoryWrap", 3);
					dictionary.Add("com_tencent_pandora_LoggerWrap", 4);
					dictionary.Add("com_tencent_pandora_UserDataWrap", 5);
					dictionary.Add("EnumWrap", 6);
					dictionary.Add("GameObjectWrap", 7);
					dictionary.Add("IEnumeratorWrap", 8);
					dictionary.Add("ImageWrap", 9);
					dictionary.Add("MonoBehaviourWrap", 10);
					dictionary.Add("ObjectWrap", 11);
					dictionary.Add("RectWrap", 12);
					dictionary.Add("stringWrap", 13);
					dictionary.Add("System_ObjectWrap", 14);
					dictionary.Add("TextureWrap", 15);
					dictionary.Add("TimeWrap", 16);
					dictionary.Add("TransformWrap", 17);
					dictionary.Add("TypeWrap", 18);
					dictionary.Add("Vector2Wrap", 19);
					dictionary.Add("Vector3Wrap", 20);
					dictionary.Add("WWWWrap", 21);
					LuaBinder.<>f__switch$map4 = dictionary;
				}
				int num;
				if (LuaBinder.<>f__switch$map4.TryGetValue(text2, ref num))
				{
					switch (num)
					{
					case 0:
						BehaviourWrap.Register(L);
						break;
					case 1:
						ComponentWrap.Register(L);
						break;
					case 2:
						com_tencent_pandora_CSharpInterfaceWrap.Register(L);
						break;
					case 3:
						com_tencent_pandora_DelegateFactoryWrap.Register(L);
						break;
					case 4:
						com_tencent_pandora_LoggerWrap.Register(L);
						break;
					case 5:
						com_tencent_pandora_UserDataWrap.Register(L);
						break;
					case 6:
						EnumWrap.Register(L);
						break;
					case 7:
						GameObjectWrap.Register(L);
						break;
					case 8:
						IEnumeratorWrap.Register(L);
						break;
					case 9:
						ImageWrap.Register(L);
						break;
					case 10:
						MonoBehaviourWrap.Register(L);
						break;
					case 11:
						ObjectWrap.Register(L);
						break;
					case 12:
						RectWrap.Register(L);
						break;
					case 13:
						stringWrap.Register(L);
						break;
					case 14:
						System_ObjectWrap.Register(L);
						break;
					case 15:
						TextureWrap.Register(L);
						break;
					case 16:
						TimeWrap.Register(L);
						break;
					case 17:
						TransformWrap.Register(L);
						break;
					case 18:
						TypeWrap.Register(L);
						break;
					case 19:
						Vector2Wrap.Register(L);
						break;
					case 20:
						Vector3Wrap.Register(L);
						break;
					case 21:
						WWWWrap.Register(L);
						break;
					}
				}
			}
		}
	}
}
