using Assets.Scripts.UI;
using System;
using UnityEngine;

namespace Assets.Scripts.GameSystem
{
	public class CUINewFlagSystem : Singleton<CUINewFlagSystem>
	{
		private static string NewFlagSetStr = "1";

		public bool IsHaveNewFlagKey(enNewFlagKey newFlagKey)
		{
			return PlayerPrefs.HasKey(newFlagKey.ToString());
		}

		public void SetAllNewFlagKey()
		{
			for (int i = 0; i < 26; i++)
			{
				PlayerPrefs.SetString(((enNewFlagKey)i).ToString(), CUINewFlagSystem.NewFlagSetStr);
			}
		}

		public void AddNewFlag(GameObject obj, enNewFlagKey flagKey, enNewFlagPos newFlagPos = enNewFlagPos.enTopRight, float scale = 1f, float offsetX = 0f, float offsetY = 0f, enNewFlagType newFlagType = enNewFlagType.enNewFlag)
		{
			if (obj == null)
			{
				return;
			}
			Transform newFlag = this.GetNewFlag(obj);
			string key = flagKey.ToString();
			if (newFlag != null)
			{
				if (flagKey > enNewFlagKey.New_None && flagKey < enNewFlagKey.New_Count && !PlayerPrefs.HasKey(key))
				{
					this.DelNewFlag(obj, flagKey, true);
					return;
				}
			}
			else if (flagKey > enNewFlagKey.New_None && flagKey < enNewFlagKey.New_Count && !PlayerPrefs.HasKey(key))
			{
				string text = string.Empty;
				if (newFlagType == enNewFlagType.enNewFlag)
				{
					text = "redDotNew";
				}
				else
				{
					text = "redDotNewSmall";
				}
				GameObject gameObject = UnityEngine.Object.Instantiate(CUIUtility.GetSpritePrefeb("UGUI/Form/Common/" + text, false, false)) as GameObject;
				if (gameObject == null)
				{
					return;
				}
				gameObject.name = text;
				gameObject.transform.SetParent(obj.transform, false);
				gameObject.transform.SetAsLastSibling();
				RectTransform rectTransform = gameObject.transform as RectTransform;
				Vector2 anchorMin = default(Vector2);
				Vector2 anchorMax = default(Vector2);
				Vector2 pivot = default(Vector2);
				switch (newFlagPos)
				{
				case enNewFlagPos.enTopLeft:
					anchorMin.x = 0f;
					anchorMin.y = 1f;
					anchorMax.x = 0f;
					anchorMax.y = 1f;
					pivot.x = 0f;
					pivot.y = 1f;
					break;
				case enNewFlagPos.enTopCenter:
					anchorMin.x = 0.5f;
					anchorMin.y = 1f;
					anchorMax.x = 0.5f;
					anchorMax.y = 1f;
					pivot.x = 0.5f;
					pivot.y = 1f;
					break;
				case enNewFlagPos.enTopRight:
					anchorMin.x = 1f;
					anchorMin.y = 1f;
					anchorMax.x = 1f;
					anchorMax.y = 1f;
					pivot.x = 1f;
					pivot.y = 1f;
					break;
				case enNewFlagPos.enMiddleLeft:
					anchorMin.x = 0f;
					anchorMin.y = 0.5f;
					anchorMax.x = 0f;
					anchorMax.y = 0.5f;
					pivot.x = 0f;
					pivot.y = 0.5f;
					break;
				case enNewFlagPos.enMiddleCenter:
					anchorMin.x = 0.5f;
					anchorMin.y = 0.5f;
					anchorMax.x = 0.5f;
					anchorMax.y = 0.5f;
					pivot.x = 0.5f;
					pivot.y = 0.5f;
					break;
				case enNewFlagPos.enMiddleRight:
					anchorMin.x = 1f;
					anchorMin.y = 0.5f;
					anchorMax.x = 1f;
					anchorMax.y = 0.5f;
					pivot.x = 1f;
					pivot.y = 0.5f;
					break;
				case enNewFlagPos.enBottomLeft:
					anchorMin.x = 0f;
					anchorMin.y = 0f;
					anchorMax.x = 0f;
					anchorMax.y = 0f;
					pivot.x = 0f;
					pivot.y = 0f;
					break;
				case enNewFlagPos.enBottomCenter:
					anchorMin.x = 0.5f;
					anchorMin.y = 0f;
					anchorMax.x = 0.5f;
					anchorMax.y = 0f;
					pivot.x = 0.5f;
					pivot.y = 0f;
					break;
				case enNewFlagPos.enBottomRight:
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
				if (scale != 1f)
				{
					rectTransform.localScale = new Vector3(scale, scale, scale);
				}
				rectTransform.anchoredPosition = new Vector2(offsetX, offsetY);
			}
		}

		public void DelNewFlag(GameObject obj, enNewFlagKey flagKey, bool immediately = true)
		{
			if (obj)
			{
				Transform newFlag = this.GetNewFlag(obj);
				if (newFlag)
				{
					string key = flagKey.ToString();
					if (!PlayerPrefs.HasKey(key))
					{
						PlayerPrefs.SetString(key, CUINewFlagSystem.NewFlagSetStr);
						PlayerPrefs.Save();
						if (immediately)
						{
							newFlag.parent = null;
							newFlag.gameObject.CustomSetActive(false);
						}
					}
				}
			}
		}

		public Transform GetNewFlag(GameObject obj)
		{
			Transform transform = obj.transform.Find("redDotNew");
			if (transform == null)
			{
				transform = obj.transform.Find("redDotNewSmall");
			}
			return transform;
		}

		public void SetNewFlagTop(GameObject obj)
		{
			Transform newFlag = this.GetNewFlag(obj);
			if (newFlag != null)
			{
				newFlag.SetAsLastSibling();
			}
		}

		public void SetNewFlagForLobbyAddedSkill(bool bShow)
		{
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CLobbySystem.LOBBY_FORM_PATH);
			if (form == null)
			{
				return;
			}
			GameObject gameObject = form.transform.Find("LobbyBottom/SysEntry/AddedSkillBtn").gameObject;
			if (gameObject != null)
			{
				if (bShow)
				{
					this.AddNewFlag(gameObject, enNewFlagKey.New_Lobby_AddedSkill_V14, enNewFlagPos.enTopRight, 1f, 0f, 0f, enNewFlagType.enNewFlag);
				}
				else
				{
					this.DelNewFlag(gameObject, enNewFlagKey.New_Lobby_AddedSkill_V14, true);
				}
			}
		}

		public void SetNewFlagForXunYouBuy(bool bShow)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm("UGUI/Form/System/Pay/Form_Partner.prefab");
			if (form != null)
			{
				GameObject widget = form.GetWidget(1);
				if (bShow)
				{
					this.AddNewFlag(widget, enNewFlagKey.New_XunYouJIasu_V14, enNewFlagPos.enTopRight, 1f, 0f, 0f, enNewFlagType.enNewFlag);
				}
				else
				{
					this.DelNewFlag(widget, enNewFlagKey.New_XunYouJIasu_V14, true);
				}
			}
		}

		public void SetNewFlagForSocialFriend(bool bShow)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CFriendContoller.FriendFormPath);
			if (form != null)
			{
				GameObject gameObject = form.transform.Find("btnGroup/btnMyCard").gameObject;
				if (bShow)
				{
					this.AddNewFlag(gameObject, enNewFlagKey.New_SocialFriend_V15, enNewFlagPos.enTopRight, 0.8f, 18f, 8f, enNewFlagType.enNewFlag);
				}
				else
				{
					this.DelNewFlag(gameObject, enNewFlagKey.New_SocialFriend_V15, true);
				}
			}
		}
	}
}
