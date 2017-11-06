using Assets.Scripts.Framework;
using Assets.Scripts.UI;
using ResData;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class CUIRedDotSystem : Singleton<CUIRedDotSystem>
	{
		public static string s_redDotName = "redDot";

		public static string s_redVersionKey = "RedVer_";

		public static bool IsShowRedDotByVersion(enRedID redID)
		{
			bool result = false;
			bool flag = false;
			uint redIDVersionByServerData = CUIRedDotSystem.GetRedIDVersionByServerData(redID, out flag);
			if (!flag)
			{
				return result;
			}
			string key = CUIRedDotSystem.s_redVersionKey + (int)redID;
			if (!PlayerPrefs.HasKey(key))
			{
				CUIRedDotSystem.SetRedDotViewByVersion(redID);
				return result;
			}
			string @string = PlayerPrefs.GetString(key);
			if (@string != null)
			{
				string[] array = @string.Split(new char[]
				{
					'_'
				});
				if (array.Length <= 1)
				{
					CUIRedDotSystem.SetRedDotViewByVersion(redID);
					return result;
				}
				uint num = 0u;
				int num2 = 0;
				uint.TryParse(array[0], ref num);
				int.TryParse(array[1], ref num2);
				if (num2 == 0 || num != redIDVersionByServerData)
				{
					result = true;
				}
			}
			return result;
		}

		public static bool IsShowRedDotByLogic(enRedID redID)
		{
			bool result = false;
			if (redID != enRedID.Mall_SymbolTab)
			{
				if (redID == enRedID.Mall_MysteryTab)
				{
					result = Singleton<MySteryShop>.GetInstance().IsShopAvailable();
				}
			}
			else
			{
				result = Singleton<CMallSystem>.GetInstance().HasFreeDrawCnt(redID);
			}
			return result;
		}

		public static void SetRedDotViewByVersion(enRedID redID)
		{
			bool flag = false;
			uint redIDVersionByServerData = CUIRedDotSystem.GetRedIDVersionByServerData(redID, out flag);
			if (!flag)
			{
				return;
			}
			string key = CUIRedDotSystem.s_redVersionKey + (int)redID;
			string value = redIDVersionByServerData + "_1";
			PlayerPrefs.SetString(key, value);
			PlayerPrefs.Save();
		}

		public static uint GetRedIDVersionByServerData(enRedID redID, out bool isHaveData)
		{
			uint result = 0u;
			isHaveData = false;
			ResRedDotInfo resRedDotInfo = new ResRedDotInfo();
			if (GameDataMgr.redDotInfoDict.TryGetValue((uint)redID, out resRedDotInfo))
			{
				result = resRedDotInfo.dwVerNum;
				isHaveData = true;
			}
			return result;
		}

		public static void AddRedDot(GameObject target, enRedDotPos dotPos = enRedDotPos.enTopRight, int alertNum = 0, int offsetX = 0, int offsetY = 0)
		{
			if (target == null || target.transform == null)
			{
				return;
			}
			CUIRedDotSystem.DelRedDot(target);
			GameObject gameObject;
			if (alertNum == 0)
			{
				gameObject = (Object.Instantiate(CUIUtility.GetSpritePrefeb("UGUI/Form/Common/redDot", false, false)) as GameObject);
			}
			else
			{
				gameObject = (Object.Instantiate(CUIUtility.GetSpritePrefeb("UGUI/Form/Common/redDotBig", false, false)) as GameObject);
			}
			Transform transform = gameObject.transform;
			transform.gameObject.name = CUIRedDotSystem.s_redDotName;
			CUIMiniEventScript component = transform.GetComponent<CUIMiniEventScript>();
			component.m_onDownEventParams.tag = 0;
			if (alertNum != 0 && transform.Find("Text") != null)
			{
				Text component2 = transform.Find("Text").GetComponent<Text>();
				component2.set_text(alertNum.ToString());
			}
			transform.SetParent(target.transform, false);
			transform.SetAsLastSibling();
			RectTransform rectTransform = transform as RectTransform;
			Vector2 anchorMin = default(Vector2);
			Vector2 anchorMax = default(Vector2);
			Vector2 pivot = default(Vector2);
			switch (dotPos)
			{
			case enRedDotPos.enTopLeft:
				anchorMin.x = 0f;
				anchorMin.y = 1f;
				anchorMax.x = 0f;
				anchorMax.y = 1f;
				pivot.x = 0f;
				pivot.y = 1f;
				break;
			case enRedDotPos.enTopCenter:
				anchorMin.x = 0.5f;
				anchorMin.y = 1f;
				anchorMax.x = 0.5f;
				anchorMax.y = 1f;
				pivot.x = 0.5f;
				pivot.y = 1f;
				break;
			case enRedDotPos.enTopRight:
				anchorMin.x = 1f;
				anchorMin.y = 1f;
				anchorMax.x = 1f;
				anchorMax.y = 1f;
				pivot.x = 1f;
				pivot.y = 1f;
				break;
			case enRedDotPos.enMiddleLeft:
				anchorMin.x = 0f;
				anchorMin.y = 0.5f;
				anchorMax.x = 0f;
				anchorMax.y = 0.5f;
				pivot.x = 0f;
				pivot.y = 0.5f;
				break;
			case enRedDotPos.enMiddleCenter:
				anchorMin.x = 0.5f;
				anchorMin.y = 0.5f;
				anchorMax.x = 0.5f;
				anchorMax.y = 0.5f;
				pivot.x = 0.5f;
				pivot.y = 0.5f;
				break;
			case enRedDotPos.enMiddleRight:
				anchorMin.x = 1f;
				anchorMin.y = 0.5f;
				anchorMax.x = 1f;
				anchorMax.y = 0.5f;
				pivot.x = 1f;
				pivot.y = 0.5f;
				break;
			case enRedDotPos.enBottomLeft:
				anchorMin.x = 0f;
				anchorMin.y = 0f;
				anchorMax.x = 0f;
				anchorMax.y = 0f;
				pivot.x = 0f;
				pivot.y = 0f;
				break;
			case enRedDotPos.enBottomCenter:
				anchorMin.x = 0.5f;
				anchorMin.y = 0f;
				anchorMax.x = 0.5f;
				anchorMax.y = 0f;
				pivot.x = 0.5f;
				pivot.y = 0f;
				break;
			case enRedDotPos.enBottomRight:
				anchorMin.x = 1f;
				anchorMin.y = 0f;
				anchorMax.x = 1f;
				anchorMax.y = 0f;
				pivot.x = 1f;
				pivot.y = 0f;
				break;
			}
			rectTransform.pivot = pivot;
			rectTransform.anchorMin = anchorMin;
			rectTransform.anchorMax = anchorMax;
			rectTransform.anchoredPosition = new Vector2((float)offsetX, (float)offsetY);
			Singleton<CUINewFlagSystem>.instance.SetNewFlagTop(target);
		}

		public static void DelRedDot(GameObject target)
		{
			if (target == null || target.transform == null)
			{
				return;
			}
			CUIMiniEventScript[] componentsInChildren = target.transform.GetComponentsInChildren<CUIMiniEventScript>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (componentsInChildren[i].m_onDownEventID == enUIEventID.Common_RedDotParsEvent && componentsInChildren[i].m_onDownEventParams.tag == 0)
				{
					componentsInChildren[i].m_onDownEventParams.tag = 1;
					componentsInChildren[i].gameObject.CustomSetActive(false);
					CUICommonSystem.DestoryObj(componentsInChildren[i].gameObject, 0.1f);
				}
			}
		}

		public static bool IsHaveRedDot(GameObject target)
		{
			if (target == null || target.transform == null)
			{
				return false;
			}
			bool result = false;
			CUIMiniEventScript[] componentsInChildren = target.transform.GetComponentsInChildren<CUIMiniEventScript>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (componentsInChildren[i].m_onDownEventID == enUIEventID.Common_RedDotParsEvent && componentsInChildren[i].m_onDownEventParams.tag == 0)
				{
					result = true;
					break;
				}
			}
			return result;
		}
	}
}
