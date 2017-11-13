using Assets.Scripts.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class CHyperLink
	{
		public const char StartChar = '[';

		public const char EndChar = ']';

		public const char CommandDelimiter = '|';

		public const char ParamDelimiter = ',';

		public static bool Bind(GameObject target, string hyperlink)
		{
			if (string.IsNullOrEmpty(hyperlink))
			{
				return false;
			}
			string[] array = hyperlink.Split(new char[]
			{
				'|'
			});
			if (array.Length != 2)
			{
				return false;
			}
			int num;
			if (!int.TryParse(array[0], ref num))
			{
				return false;
			}
			string[] array2 = array[1].Split(new char[]
			{
				','
			});
			CUIEventScript component = target.GetComponent<CUIEventScript>();
			stUIEventParams eventParams = default(stUIEventParams);
			switch (num)
			{
			case 1:
			{
				if (array2.Length != 4)
				{
					return false;
				}
				ulong commonUInt64Param = ulong.Parse(array2[0]);
				int tag = int.Parse(array2[1]);
				ulong commonUInt64Param2 = ulong.Parse(array2[2]);
				int tag2 = int.Parse(array2[3]);
				eventParams.commonUInt64Param1 = commonUInt64Param;
				eventParams.tag = tag;
				eventParams.commonUInt64Param2 = commonUInt64Param2;
				eventParams.tag2 = tag2;
				component.SetUIEvent(enUIEventType.Click, enUIEventID.Guild_Hyperlink_Search_Guild, eventParams);
				Utility.GetComponetInChild<Text>(target, "Text").set_text(Singleton<CTextManager>.instance.GetText("Common_Check"));
				return true;
			}
			case 2:
			{
				if (array2.Length != 2)
				{
					return false;
				}
				ulong commonUInt64Param3 = ulong.Parse(array2[0]);
				int tag3 = int.Parse(array2[1]);
				eventParams.commonUInt64Param1 = commonUInt64Param3;
				eventParams.tag = tag3;
				component.SetUIEvent(enUIEventType.Click, enUIEventID.Guild_Hyperlink_Search_PrepareGuild, eventParams);
				Utility.GetComponetInChild<Text>(target, "Text").set_text(Singleton<CTextManager>.instance.GetText("Common_Check"));
				return true;
			}
			case 3:
			{
				int tag4 = 0;
				if (int.TryParse(array2[0], ref tag4))
				{
					eventParams.tag = tag4;
					if (array2.Length >= 3)
					{
						eventParams.tagList = new List<uint>();
						for (int i = 0; i < array2.Length - 2; i++)
						{
							int num2 = 0;
							if (int.TryParse(array2[i + 2], ref num2))
							{
								eventParams.tagList.Add((uint)num2);
							}
						}
					}
					component.SetUIEvent(enUIEventType.Click, enUIEventID.Mail_JumpForm, eventParams);
					if (array2.Length >= 2)
					{
						Utility.GetComponetInChild<Text>(target, "Text").set_text(array2[1]);
					}
					else
					{
						Utility.GetComponetInChild<Text>(target, "Text").set_text(Singleton<CTextManager>.instance.GetText("Common_Check"));
					}
					return true;
				}
				return false;
			}
			case 4:
				eventParams.tagStr = array2[0];
				component.SetUIEvent(enUIEventType.Click, enUIEventID.Mail_JumpUrl, eventParams);
				if (array2.Length == 2)
				{
					Utility.GetComponetInChild<Text>(target, "Text").set_text(array2[1]);
				}
				else
				{
					Utility.GetComponetInChild<Text>(target, "Text").set_text(Singleton<CTextManager>.instance.GetText("Common_Check"));
				}
				return true;
			default:
				return false;
			}
		}

		public static bool IsStandCommond(string hyperlink)
		{
			if (string.IsNullOrEmpty(hyperlink))
			{
				return false;
			}
			string[] array = hyperlink.Split(new char[]
			{
				'|'
			});
			if (array.Length != 2)
			{
				return false;
			}
			int num;
			if (!int.TryParse(array[0], ref num))
			{
				return false;
			}
			string[] array2 = array[1].Split(new char[]
			{
				','
			});
			switch (num)
			{
			case 1:
				return array2.Length == 4;
			case 2:
				return array2.Length == 1;
			case 3:
			{
				int num2 = 0;
				return int.TryParse(array2[0], ref num2);
			}
			case 4:
				return true;
			default:
				return false;
			}
		}
	}
}
