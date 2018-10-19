using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
	public class CUIUtility : Singleton<CUIUtility>
	{
		public delegate void TextComboClickCallback(CUIEvent evt);

		public const int c_hideLayer = 31;

		public const int c_uiLayer = 5;

		public const int c_defaultLayer = 0;

		public const int c_UIBottomBg = 18;

		public const int c_formBaseWidth = 960;

		public const int c_formBaseHeight = 640;

		public const string s_Form_Battle_Dir = "UGUI/Form/Battle/";

		public const string s_Form_System_Dir = "UGUI/Form/System/";

		public const string s_Form_Common_Dir = "UGUI/Form/Common/";

		public const string s_Sprite_Battle_Dir = "UGUI/Sprite/Battle/";

		public const string s_Sprite_System_Dir = "UGUI/Sprite/System/";

		public const string s_Sprite_Common_Dir = "UGUI/Sprite/Common/";

		public const string s_Sprite_Dynamic_Dir = "UGUI/Sprite/Dynamic/";

		public const string s_Sprite_Dynamic_Quality_Dir = "UGUI/Sprite/Dynamic/Quality/";

		public static string s_Form_Activity_Dir = "UGUI/Form/System/OpActivity/";

		public static string s_Sprite_Activity_Dir = "UGUI/Sprite/System/OpActivity/";

		public static string s_Sprite_HeroInfo_Dir = "UGUI/Sprite/Dynamic/HeroInfo/";

		public static string s_IDIP_Form_Dir = "UGUI/Form/System/IDIPNotice/";

		public static string s_Animation3D_Dir = "UGUI/Animation/";

		public static string s_Particle_Dir = "UGUI/Particle/";

		public static string s_heroSceneBgPath = "UIScene_HeroInfo";

		public static string s_heroSelectBgPath = "UIScene_HeroSelect";

		public static string s_recommendHeroInfoBgPath = "UIScene_Recommend_HeroInfo";

		public static string s_lotterySceneBgPath = "UIScene_Lottery";

		public static string s_battleResultBgPath = "UIScene_BattleResult";

		public static string s_Sprite_Dynamic_Icon_Dir = "UGUI/Sprite/Dynamic/Icon/";

		public static string s_Sprite_Dynamic_BustHero_Dir = "UGUI/Sprite/Dynamic/BustHero/";

		public static string s_Sprite_Dynamic_BustCircle_Dir = "UGUI/Sprite/Dynamic/BustCircle/";

		public static string s_Sprite_Dynamic_BustCircleSmall_Dir = "UGUI/Sprite/Dynamic/BustCircleSmall/";

		public static string s_Sprite_Dynamic_BustHeroLarge_Dir = "UGUI/Sprite/Dynamic/BustHeroLarge/";

		public static string s_Sprite_Dynamic_ActivityPve_Dir = "UGUI/Sprite/Dynamic/ActivityPve/";

		public static string s_Sprite_Dynamic_FucUnlock_Dir = "UGUI/Sprite/Dynamic/FunctionUnlock/";

		public static string s_Sprite_Dynamic_Dialog_Dir = "UGUI/Sprite/Dynamic/Dialog/";

		public static string s_Sprite_Dynamic_Dialog_Dir_Head = CUIUtility.s_Sprite_Dynamic_Dialog_Dir + "Heads/";

		public static string s_Sprite_Dynamic_Dialog_Dir_Portrait = CUIUtility.s_Sprite_Dynamic_Dialog_Dir + "Portraits/";

		public static string s_Sprite_Dynamic_Map_Dir = "UGUI/Sprite/Dynamic/Map/";

		public static string s_Sprite_Dynamic_Talent_Dir = "UGUI/Sprite/Dynamic/Skill/";

		public static string s_Sprite_Dynamic_Adventure_Dir = "UGUI/Sprite/Dynamic/Adventure/";

		public static string s_Sprite_Dynamic_Task_Dir = "UGUI/Sprite/Dynamic/Task/";

		public static string s_Sprite_Dynamic_Skill_Dir = "UGUI/Sprite/Dynamic/Skill/";

		public static string s_Sprite_Dynamic_PvPTitle_Dir = "UGUI/Sprite/Dynamic/PvPTitle/";

		public static string s_Sprite_Dynamic_GuildHead_Dir = "UGUI/Sprite/Dynamic/GuildHead/";

		public static string s_Sprite_Dynamic_Guild_Dir = "UGUI/Sprite/Dynamic/Guild/";

		public static string s_Sprite_Dynamic_Profession_Dir = "UGUI/Sprite/Dynamic/Profession/";

		public static string s_Sprite_Dynamic_Pvp_Settle_Dir = "UGUI/Sprite/System/PvpIcon/";

		public static string s_Sprite_Dynamic_Pvp_Settle_Large_Dir = "UGUI/Sprite/System/PvpIconLarge/";

		public static string s_Sprite_Dynamic_Achieve_Dir = "UGUI/Sprite/Dynamic/Achieve/";

		public static string s_Sprite_Dynamic_Purchase_Dir = "UGUI/Sprite/Dynamic/Purchase/";

		public static string s_Sprite_Dynamic_BustPlayer_Dir = "UGUI/Sprite/Dynamic/BustPlayer/";

		public static string s_Sprite_Dynamic_AddedSkill_Dir = "UGUI/Sprite/Dynamic/AddedSkill/";

		public static string s_Sprite_Dynamic_Proficiency_Dir = "UGUI/Sprite/Dynamic/HeroProficiency/";

		public static string s_Sprite_Dynamic_PvpEntry_Dir = "UGUI/Sprite/Dynamic/PvpEntry/";

		public static string s_Sprite_Dynamic_SkinQuality_Dir = "UGUI/Sprite/Dynamic/SkinQuality/";

		public static string s_Sprite_Dynamic_ExperienceCard_Dir = "UGUI/Sprite/Dynamic/ExperienceCard/";

		public static string s_Sprite_Dynamic_PvpAchievementShare_Dir = "UGUI/Sprite/Dynamic/PvpShare/";

		public static string s_Sprite_Dynamic_UnionBattleBaner_Dir = "UGUI/Sprite/Dynamic/UnionBattleBaner/";

		public static string s_Sprite_Dynamic_Nobe_Dir = "UGUI/Sprite/Dynamic/Nobe/";

		public static string s_Sprite_Dynamic_Newbie_Dir = "UGUI/Sprite/Dynamic/Newbie/";

		public static string s_Sprite_Dynamic_SkinFeature_Dir = "UGUI/Sprite/Dynamic/SkinFeature/";

		public static string s_Sprite_Dynamic_Signal_Dir = "UGUI/Sprite/Dynamic/Signal/";

		public static string s_Sprite_Dynamic_Mall_Dir = "UGUI/Sprite/Dynamic/Mall/";

		public static string s_Sprite_System_Equip_Dir = "UGUI/Sprite/System/Equip/";

		public static string s_Sprite_System_BattleEquip_Dir = "UGUI/Sprite/System/BattleEquip/";

		public static string s_Sprite_System_Honor_Dir = "UGUI/Sprite/System/Honor/";

		public static string s_Sprite_System_HeroSelect_Dir = "UGUI/Sprite/System/HeroSelect/";

		public static string s_Sprite_System_Qualifying_Dir = "UGUI/Sprite/System/Qualifying/";

		public static string s_Sprite_System_Burn_Dir = "UGUI/Sprite/System/BurnExpedition/";

		public static string s_Sprite_System_Mall_Dir = "UGUI/Sprite/System/Mall/";

		public static string s_Sprite_System_Ladder_Dir = "UGUI/Sprite/System/Ladder/";

		public static string s_Sprite_System_ShareUI_Dir = "UGUI/Sprite/System/ShareUI/";

		public static string s_Sprite_System_Lobby_Dir = "UGUI/Sprite/System/LobbyDynamic/";

		public static string s_Sprite_System_Wifi_Dir = "UGUI/Sprite/System/Wifi/";

		public static string s_battleSignalPrefabDir = "UGUI/Sprite/Battle/Signal/";

		public static Color s_Color_White = new Color(1f, 1f, 1f);

		public static Color s_Color_White_HalfAlpha = new Color(1f, 1f, 1f, 0.490196079f);

		public static Color s_Color_White_FullAlpha = new Color(1f, 1f, 1f, 0f);

		public static Color s_Color_Grey = new Color(0.3137255f, 0.3137255f, 0.3137255f);

		public static Color s_Color_GrayShader = new Color(0f, 1f, 1f);

		public static Color s_Color_Full = new Color(1f, 1f, 1f, 1f);

		public static Color s_Color_DisableGray = new Color(0.392156869f, 0.392156869f, 0.392156869f, 1f);

		public static Color s_Text_Color_White = new Color(1f, 1f, 1f);

		public static Color s_Text_Color_Disable = new Color(0.6039216f, 0.6f, 0.6f);

		public static Color s_Text_Color_Vip_Chat_Self = new Color(1f, 0.894117653f, 0f);

		public static Color s_Text_Color_Vip_Chat_Other = new Color(0.7764706f, 0.6509804f, 0.3137255f);

		public static Color Intimacy_Full = new Color(1f, 0.09411765f, 0f);

		public static Color Intimacy_High = new Color(1f, 0.09411765f, 1f);

		public static Color Intimacy_Mid = new Color(1f, 0.521568656f, 0.13333334f);

		public static Color Intimacy_Low = new Color(1f, 0.8745098f, 0.180392161f);

		public static Color Intimacy_Freeze = new Color(0.807843149f, 0.8117647f, 0.882352948f);

		public static Color s_Text_Color_Self = new Color(0.9490196f, 0.7882353f, 0.3019608f);

		public static Color s_Text_Color_Camp_1 = new Color(0.403921574f, 0.6039216f, 0.968627453f);

		public static Color s_Text_Color_Camp_2 = new Color(0.858823538f, 0.180392161f, 0.282352954f);

		public static Color s_Text_Color_CommonGray = new Color(0.7019608f, 0.7058824f, 0.7137255f);

		public static Color s_Text_Color_MyHeroName = new Color(0.8784314f, 0.7294118f, 0.13333334f);

		public static Color s_Text_OutLineColor_MyHeroName = new Color(0.117647059f, 0.08627451f, 0.0235294122f);

		public static Color s_Text_Color_ListElement_Normal = new Color(0.494117647f, 0.533333361f, 0.635294139f);

		public static Color s_Text_Color_ListElement_Select = new Color(1f, 1f, 1f);

		public static Color s_Text_Color_Hero_Name_Active = new Color(0.9254902f, 0.8509804f, 0.6431373f);

		public static Color s_Text_Color_Hero_Name_DeActive = new Color(0.4f, 0.3647059f, 0.270588249f);

		public static Color s_Text_Color_Camp_Allies = new Color(0.3529412f, 0.549019635f, 0.8352941f);

		public static Color s_Text_Color_Camp_Enemy = new Color(0.6862745f, 0.160784319f, 0.235294119f);

		public static Color s_Color_Button_Disable = new Color(0.384313732f, 0.384313732f, 0.384313732f, 0.9019608f);

		public static Color s_Color_BraveScore_BaojiKedu_On = new Color(0f, 1f, 0.607843161f, 1f);

		public static Color s_Color_BraveScore_BaojiKedu_Off = new Color(0f, 0.7921569f, 1f, 1f);

		public static Color s_Color_EnemyHero_Button_PINK = new Color(1f, 0.5647059f, 0.5647059f);

		public static Color[] s_Text_Color_Hero_Advance = new Color[]
		{
			new Color(1f, 1f, 1f),
			new Color(0.3882353f, 0.9019608f, 0.239215687f),
			new Color(0.117647059f, 0.6431373f, 0.9137255f),
			new Color(0.7647059f, 0.3372549f, 0.8235294f),
			new Color(0.9490196f, 0.466666669f, 0.07058824f)
		};

		public static Color s_Text_Color_Can_Afford = Color.white;

		public static Color s_Text_Color_Can_Not_Afford = new Color(0.6784314f, 0.227450982f, 0.203921571f);

		private static readonly Regex s_regexEmoji = new Regex("\\ud83c[\\udf00-\\udfff]|\\ud83d[\\udc00-\\udeff]|\\ud83d[\\ude80-\\udeff]");

		public static string s_ui_defaultShaderName = "Sprites/Default";

		public static string s_ui_graySpritePath = CUIUtility.s_Sprite_Dynamic_BustHero_Dir + "gray";

		public static string[] s_materianlParsKey = new string[]
		{
			"_StencilComp",
			"_Stencil",
			"_StencilOp",
			"_StencilWriteMask",
			"_StencilReadMask",
			"_ColorMask"
		};

		public static Dictionary<CUIListScript, string[]> s_textComboBoxContent = new Dictionary<CUIListScript, string[]>();

		public static Dictionary<GameObject, CUIListScript> s_textComboBoxBtnListMap = new Dictionary<GameObject, CUIListScript>();

		public static Dictionary<CUIListScript, CUIUtility.TextComboClickCallback> s_textComboBoxClickCallbackMap = new Dictionary<CUIListScript, CUIUtility.TextComboClickCallback>();

		public static Color[] s_Text_Skill_HurtType_Color = new Color[]
		{
			new Color(0.8156863f, 0.423529416f, 0.423529416f),
			new Color(0.5882353f, 0.596078455f, 0.9607843f),
			new Color(0.9372549f, 0.75686276f, 0.211764708f),
			new Color(0.5882353f, 0.9607843f, 0.8352941f)
		};

		public static Color[] s_Text_SkillName_And_HurtValue_Color = new Color[]
		{
			new Color(0.831372559f, 0.623529434f, 0.623529434f),
			new Color(0.6039216f, 0.607843161f, 0.7882353f),
			new Color(0.796078444f, 0.7372549f, 0.5686275f),
			new Color(0.545098066f, 0.698039234f, 0.670588255f)
		};

		public static Color[] s_Text_Total_Damage_Text_Color = new Color[]
		{
			new Color(0.933333337f, 0.8862745f, 0.6627451f),
			new Color(0.5058824f, 0.5647059f, 0.8862745f)
		};

		public static Color[] s_Text_Total_Damage_Value_Color = new Color[]
		{
			new Color(0.933333337f, 0.8784314f, 0.7882353f),
			new Color(0.7882353f, 0.858823538f, 0.933333337f)
		};

		public static Color[] s_Text_Total_Damage_Text_Outline_Color = new Color[]
		{
			new Color(0.227450982f, 0.109803922f, 0.07058824f),
			new Color(0.07058824f, 0.109803922f, 0.227450982f)
		};

		public override void Init()
		{
			base.Init();
			CUIEventManager instance = Singleton<CUIEventManager>.GetInstance();
			instance.AddUIEventListener(enUIEventID.TextCombo_Choose, new CUIEventManager.OnUIEventHandler(CUIUtility.TextComboDealListChosen));
			instance.AddUIEventListener(enUIEventID.TextCombo_OnEnable, new CUIEventManager.OnUIEventHandler(this.OnTextComboEnable));
			instance.AddUIEventListener(enUIEventID.TextCombo_TriggerClick, new CUIEventManager.OnUIEventHandler(this.OnTextComboTriggerBtnClick));
			instance.AddUIEventListener(enUIEventID.TextCombo_CloseCombo, new CUIEventManager.OnUIEventHandler(this.OnTexeComboCloseCombos));
		}

		public override void UnInit()
		{
			base.UnInit();
			CUIEventManager instance = Singleton<CUIEventManager>.GetInstance();
			instance.RemoveUIEventListener(enUIEventID.TextCombo_Choose, new CUIEventManager.OnUIEventHandler(CUIUtility.TextComboDealListChosen));
			instance.RemoveUIEventListener(enUIEventID.TextCombo_OnEnable, new CUIEventManager.OnUIEventHandler(this.OnTextComboEnable));
			instance.RemoveUIEventListener(enUIEventID.TextCombo_TriggerClick, new CUIEventManager.OnUIEventHandler(this.OnTextComboTriggerBtnClick));
			instance.RemoveUIEventListener(enUIEventID.TextCombo_CloseCombo, new CUIEventManager.OnUIEventHandler(this.OnTexeComboCloseCombos));
		}

		public static Vector2 GetFixedTextSize(Text text, string content, float fixedWidth)
		{
			return Vector2.zero;
		}

		public static T GetComponentInChildren<T>(GameObject go) where T : Component
		{
			if (go == null)
			{
				return (T)((object)null);
			}
			T t = go.GetComponent<T>();
			if (t != null)
			{
				return t;
			}
			Transform transform = go.transform;
			int childCount = transform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				t = CUIUtility.GetComponentInChildren<T>(transform.GetChild(i).gameObject);
				if (t != null)
				{
					return t;
				}
			}
			return (T)((object)null);
		}

		public static void GetComponentsInChildren<T>(GameObject go, T[] components, ref int count) where T : Component
		{
			T component = go.GetComponent<T>();
			if (component != null)
			{
				components[count] = component;
				count++;
			}
			for (int i = 0; i < go.transform.childCount; i++)
			{
				CUIUtility.GetComponentsInChildren<T>(go.transform.GetChild(i).gameObject, components, ref count);
			}
		}

		public static string StringReplace(string scrStr, params string[] values)
		{
			return string.Format(scrStr, values);
		}

		public static Vector3 ScreenToWorldPoint(Camera camera, Vector2 screenPoint, float z)
		{
			return (!(camera == null)) ? camera.ViewportToWorldPoint(new Vector3(screenPoint.x / (float)Screen.width, screenPoint.y / (float)Screen.height, z)) : new Vector3(screenPoint.x, screenPoint.y, z);
		}

		public static Vector2 WorldToScreenPoint(Camera camera, Vector3 worldPoint)
		{
            return (!(camera == null)) ? (Vector2)camera.WorldToScreenPoint(worldPoint) : new Vector2(worldPoint.x, worldPoint.y);
		}

		public static void SetGameObjectLayer(GameObject gameObject, int layer)
		{
			gameObject.layer = layer;
			for (int i = 0; i < gameObject.transform.childCount; i++)
			{
				CUIUtility.SetGameObjectLayer(gameObject.transform.GetChild(i).gameObject, layer);
			}
		}

		public static float ValueInRange(float value, float min, float max)
		{
			if (value < min)
			{
				return min;
			}
			if (value > max)
			{
				return max;
			}
			return value;
		}

		public static void ResetUIScale(GameObject target)
		{
			Vector3 localScale = target.transform.localScale;
			Transform parent = target.transform.parent;
			target.transform.SetParent(null);
			target.transform.localScale = localScale;
			target.transform.SetParent(parent);
		}

		public static string RemoveEmoji(string str)
		{
			return CUIUtility.s_regexEmoji.Replace(str, string.Empty);
		}

		public static GameObject GetSpritePrefeb(string prefebPath, bool needCached = false, bool unloadBelongedAssetBundleAfterLoaded = false)
		{
			GameObject gameObject = Singleton<CResourceManager>.GetInstance().GetResource(prefebPath, typeof(GameObject), enResourceType.UISprite, needCached, unloadBelongedAssetBundleAfterLoaded).m_content as GameObject;
			if (gameObject == null)
			{
				gameObject = (Singleton<CResourceManager>.GetInstance().GetResource(CUIUtility.s_Sprite_Dynamic_Icon_Dir + "0", typeof(GameObject), enResourceType.UISprite, true, true).m_content as GameObject);
			}
			return gameObject;
		}

		public static float[] GetMaterailMaskPars(Material tarMat)
		{
			float[] array = new float[CUIUtility.s_materianlParsKey.Length];
			for (int i = 0; i < CUIUtility.s_materianlParsKey.Length; i++)
			{
				array[i] = tarMat.GetFloat(CUIUtility.s_materianlParsKey[i]);
			}
			return array;
		}

		public static void SetMaterailMaskPars(float[] pars, Material tarMat)
		{
			for (int i = 0; i < CUIUtility.s_materianlParsKey.Length; i++)
			{
				tarMat.SetFloat(CUIUtility.s_materianlParsKey[i], pars[i]);
			}
		}

		public static void SetImageSprite(Image image, GameObject prefab, bool isShowSpecMatrial = false)
		{
			if (image == null)
			{
				return;
			}
			if (prefab == null)
			{
				image.sprite = null;
				return;
			}
			SpriteRenderer component = prefab.GetComponent<SpriteRenderer>();
			if (component != null)
			{
				image.sprite = component.sprite;
				if (isShowSpecMatrial && component.sharedMaterial != null && component.sharedMaterial.shader != null && !component.sharedMaterial.shader.name.Equals(CUIUtility.s_ui_defaultShaderName))
				{
					float[] materailMaskPars = CUIUtility.GetMaterailMaskPars(image.material);
					image.material = new Material(component.sharedMaterial);
					image.material.shaderKeywords = component.sharedMaterial.shaderKeywords;
					CUIUtility.SetMaterailMaskPars(materailMaskPars, image.material);
				}
				else if (isShowSpecMatrial)
				{
					image.material = null;
				}
			}
			if (image is Image2)
			{
				SGameSpriteSettings component2 = prefab.GetComponent<SGameSpriteSettings>();
				Image2 image2 = image as Image2;
				image2.alphaTexLayout = ((!(component2 != null)) ? ImageAlphaTexLayout.None : component2.layout);
			}
		}

		public static void SetImageSprite(Image image, string prefabPath, CUIFormScript formScript, bool loadSync = true, bool needCached = false, bool unloadBelongedAssetBundleAfterLoaded = false, bool isShowSpecMatrial = false)
		{
			if (image == null)
			{
				return;
			}
			if (loadSync)
			{
				CUIUtility.SetImageSprite(image, CUIUtility.GetSpritePrefeb(prefabPath, needCached, unloadBelongedAssetBundleAfterLoaded), isShowSpecMatrial);
			}
			else
			{
				image.color = new Color(image.color.r, image.color.g, image.color.b, 0f);
				formScript.AddASyncLoadedImage(image, prefabPath, needCached, unloadBelongedAssetBundleAfterLoaded, isShowSpecMatrial);
			}
		}

		public static void SetGenderImageSprite(Image image, byte bGender)
		{
			if (bGender == 1)
			{
				CUIUtility.SetImageSprite(image, string.Format("{0}icon/Ico_boy.prefab", "UGUI/Sprite/Dynamic/"), null, true, false, false, false);
			}
			else if (bGender == 2)
			{
				CUIUtility.SetImageSprite(image, string.Format("{0}icon/Ico_girl.prefab", "UGUI/Sprite/Dynamic/"), null, true, false, false, false);
			}
		}

		public static void SetImageSprite(Image image, Image targetImage)
		{
			if (image == null)
			{
				return;
			}
			if (targetImage == null)
			{
				image.sprite = null;
				return;
			}
			image.sprite = targetImage.sprite;
			if (image is Image2)
			{
				Image2 image2 = image as Image2;
				image2.alphaTexLayout = ImageAlphaTexLayout.None;
				if (targetImage is Image2)
				{
					Image2 image3 = targetImage as Image2;
					image2.alphaTexLayout = image3.alphaTexLayout;
				}
			}
		}

		public static void SetImageGrey(Graphic graphic, bool isSetGrey)
		{
			CUIUtility.SetImageGrey(graphic, isSetGrey, Color.white);
		}

		private static void SetImageGrey(Graphic graphic, bool isSetGrey, Color defaultColor)
		{
			if (graphic != null)
			{
				graphic.color = ((!isSetGrey) ? defaultColor : CUIUtility.s_Color_Grey);
			}
		}

		public static void SetImageGrayMatrial(Image image)
		{
			GameObject spritePrefeb = CUIUtility.GetSpritePrefeb(CUIUtility.s_ui_graySpritePath, false, false);
			SpriteRenderer component = spritePrefeb.GetComponent<SpriteRenderer>();
			if (component != null)
			{
				float[] materailMaskPars = CUIUtility.GetMaterailMaskPars(image.material);
				image.material = new Material(component.sharedMaterial);
				image.material.shaderKeywords = component.sharedMaterial.shaderKeywords;
				CUIUtility.SetMaterailMaskPars(materailMaskPars, image.material);
			}
		}

		public static CUIFormScript GetFormScript(Transform transform)
		{
			if (transform == null)
			{
				return null;
			}
			CUIFormScript component = transform.gameObject.GetComponent<CUIFormScript>();
			if (component != null)
			{
				return component;
			}
			return CUIUtility.GetFormScript(transform.parent);
		}

		public static void RegisterTextComboboxContent(GameObject triggerBtn, CUIListScript list, string[] content, CUIUtility.TextComboClickCallback clickcallback = null)
		{
			CUIUtility.s_textComboBoxContent[list] = content;
			list.SetElementAmount(content.Length);
			if (content.Length > 0)
			{
				list.SelectElement(0, true);
			}
			CUIUtility.s_textComboBoxBtnListMap[triggerBtn] = list;
			if (clickcallback != null)
			{
				CUIUtility.s_textComboBoxClickCallbackMap[list] = clickcallback;
			}
		}

		public static void ClearTextComboboxContent()
		{
			CUIUtility.s_textComboBoxContent.Clear();
			CUIUtility.s_textComboBoxBtnListMap.Clear();
			CUIUtility.s_textComboBoxClickCallbackMap.Clear();
		}

		public void OnTextComboTriggerBtnClick(CUIEvent evt)
		{
			if (evt == null || evt.m_srcWidget == null)
			{
				return;
			}
			CUIListScript cUIListScript = null;
			if (CUIUtility.s_textComboBoxBtnListMap.TryGetValue(evt.m_srcWidget, out cUIListScript))
			{
				bool flag = !cUIListScript.gameObject.activeSelf;
				if (flag)
				{
					foreach (CUIListScript current in CUIUtility.s_textComboBoxContent.Keys)
					{
						current.gameObject.CustomSetActive(false);
					}
					if (cUIListScript.m_comboClickPanel != null)
					{
						cUIListScript.m_comboClickPanel.CustomSetActive(true);
					}
				}
				cUIListScript.gameObject.CustomSetActive(flag);
			}
		}

		public void OnTexeComboCloseCombos(CUIEvent evt)
		{
			foreach (CUIListScript current in CUIUtility.s_textComboBoxContent.Keys)
			{
				current.gameObject.CustomSetActive(false);
			}
			evt.m_srcWidget.CustomSetActive(false);
		}

		public void OnTextComboEnable(CUIEvent evt)
		{
			if (evt == null || evt.m_srcWidgetBelongedListScript == null)
			{
				return;
			}
			if (evt.m_srcWidget == null)
			{
				return;
			}
			CUIListScript srcWidgetBelongedListScript = evt.m_srcWidgetBelongedListScript;
			string[] array = null;
			if (CUIUtility.s_textComboBoxContent.TryGetValue(srcWidgetBelongedListScript, out array))
			{
				Transform transform = evt.m_srcWidget.transform.FindChild("Text");
				if (transform == null)
				{
					return;
				}
				Text component = transform.gameObject.GetComponent<Text>();
				if (component != null)
				{
					int srcWidgetIndexInBelongedList = evt.m_srcWidgetIndexInBelongedList;
					if (srcWidgetIndexInBelongedList >= 0 && srcWidgetIndexInBelongedList < array.Length)
					{
						component.text = array[evt.m_srcWidgetIndexInBelongedList];
					}
				}
			}
		}

		public static void SetToggleEnable(Toggle tgl, bool enable, Color textOrgColor)
		{
			tgl.enabled = enable;
			CUIToggleEventScript component = tgl.GetComponent<CUIToggleEventScript>();
			if (component)
			{
				component.enabled = enable;
			}
			Text componentInChildren = tgl.GetComponentInChildren<Text>();
			if (componentInChildren)
			{
				componentInChildren.color = ((!enable) ? CUIUtility.s_Text_Color_Disable : textOrgColor);
			}
			Image componetInChild = Utility.GetComponetInChild<Image>(tgl.gameObject, "Checkmark");
			if (componetInChild == null)
			{
				componetInChild = Utility.GetComponetInChild<Image>(tgl.gameObject, "CheckMark");
			}
			if (componetInChild != null)
			{
				componetInChild.color = ((!enable) ? CUIUtility.s_Color_GrayShader : CUIUtility.s_Color_White);
			}
		}

		public static void TextComboDealListChosen(CUIEvent evt)
		{
			if (evt == null || evt.m_srcWidget == null)
			{
				return;
			}
			CUIListScript component = evt.m_srcWidget.GetComponent<CUIListScript>();
			if (component == null)
			{
				return;
			}
			string[] array = null;
			if (CUIUtility.s_textComboBoxContent.TryGetValue(component, out array))
			{
				int selectedIndex = component.GetSelectedIndex();
				if (selectedIndex >= 0 && selectedIndex < array.Length)
				{
					Transform transform = component.gameObject.transform.parent.FindChild("Button_Down");
					if (transform != null)
					{
						Transform transform2 = transform.FindChild("Text");
						if (transform2 != null)
						{
							Text component2 = transform2.gameObject.GetComponent<Text>();
							component2.text = array[selectedIndex];
						}
					}
				}
				component.gameObject.CustomSetActive(false);
			}
			CUIUtility.TextComboClickCallback textComboClickCallback = null;
			if (CUIUtility.s_textComboBoxClickCallbackMap.TryGetValue(component, out textComboClickCallback) && textComboClickCallback != null)
			{
				textComboClickCallback(evt);
			}
		}
	}
}
