using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using ResData;
using System;
using UnityEngine;
using UnityEngine.UI;

public class CSignalTipShower : MonoBehaviour
{
	public static string res_dir = "UGUI/Sprite/Dynamic/Signal/";

	public static string S_Bg_Green = CUIUtility.s_battleSignalPrefabDir + "Signal_Tips_Green.prefab";

	public static string S_Bg_Blue = CUIUtility.s_battleSignalPrefabDir + "Signal_Tips_Blue.prefab";

	public static string S_kn_Tower_Icon = "UGUI/Sprite/Dynamic/Signal/kn_Tower";

	public static string S_Base_blue_Icon = "UGUI/Sprite/Dynamic/Signal/Base_blue";

	public static string S_Dragon_big_Icon = "UGUI/Sprite/Dynamic/Signal/Dragon_big";

	public static string S_Dragon_small_Icon = "UGUI/Sprite/Dynamic/Signal/Dragon_small";

	public static string S_kn_dragon_Icon = "UGUI/Sprite/Dynamic/Signal/kn_dragon";

	public static string S_kn_dragon_Icon_3v3 = "UGUI/Sprite/Dynamic/Signal/kn_dragon_3v3";

	public GameObject leftIcon;

	public GameObject rightIcon;

	public GameObject bg_icon;

	public GameObject signal_node;

	public GameObject signal_icon;

	public Text signal_txt;

	public GameObject inbattlemsg_node;

	public Text inbattlemsg_txt;

	public static void Preload(ref ActorPreloadTab preloadTab)
	{
		preloadTab.AddSprite(CSignalTipShower.S_kn_Tower_Icon);
		preloadTab.AddSprite(CSignalTipShower.S_Base_blue_Icon);
		preloadTab.AddSprite(CSignalTipShower.S_Dragon_big_Icon);
		preloadTab.AddSprite(CSignalTipShower.S_Dragon_small_Icon);
		preloadTab.AddSprite(CSignalTipShower.S_kn_dragon_Icon);
		preloadTab.AddSprite(CSignalTipShower.S_Bg_Green);
		preloadTab.AddSprite(CSignalTipShower.S_Bg_Blue);
	}

	public void Set(CSignalTipsElement data, CUIFormScript formScript)
	{
		if (data == null || formScript == null)
		{
			return;
		}
		if (data.type == CSignalTipsElement.EType.Signal)
		{
			this.Show(data as CSignalTips, formScript);
		}
		else if (data.type == CSignalTipsElement.EType.InBattleMsg)
		{
			this.Show(data as CSignalTips_InBatMsg, formScript);
		}
	}

	private void Show(CSignalTips data, CUIFormScript formScript)
	{
		if (data == null)
		{
			return;
		}
		this.signal_node.CustomSetActive(true);
		this.inbattlemsg_node.CustomSetActive(false);
		ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(data.m_heroID);
		if (dataByKey == null)
		{
			return;
		}
		this.SetHeroHeadIcon(this.leftIcon.gameObject.GetComponent<Image>(), formScript, dataByKey);
		string text = data.m_isHostPlayer ? CSignalTipShower.S_Bg_Green : CSignalTipShower.S_Bg_Blue;
		if (this.bg_icon == null)
		{
			return;
		}
		Image component = this.bg_icon.GetComponent<Image>();
		if (component != null && !string.IsNullOrEmpty(text))
		{
			component.SetSprite(text, formScript, true, false, false, false);
		}
		ResSignalInfo dataByKey2 = GameDataMgr.signalDatabin.GetDataByKey((long)data.m_signalID);
		if (dataByKey2 == null)
		{
			return;
		}
		this.signal_icon.CustomSetActive(true);
		if (this.signal_icon == null)
		{
			return;
		}
		Image component2 = this.signal_icon.GetComponent<Image>();
		if (component2 != null)
		{
			component2.SetSprite(dataByKey2.szUIIcon, formScript, true, false, false, false);
		}
		if (this.signal_txt != null)
		{
			this.signal_txt.set_text(dataByKey2.szText);
		}
		if (data.m_elementType >= 1)
		{
			Image component3 = this.rightIcon.GetComponent<Image>();
			if (component3 == null)
			{
				return;
			}
			switch (data.m_elementType)
			{
			case 1:
				component3.SetSprite(CSignalTipShower.S_kn_Tower_Icon, formScript, true, false, false, false);
				break;
			case 2:
				component3.SetSprite(CSignalTipShower.S_Base_blue_Icon, formScript, true, false, false, false);
				break;
			case 3:
			{
				ResHeroCfgInfo dataByKey3 = GameDataMgr.heroDatabin.GetDataByKey(data.m_targetHeroID);
				if (dataByKey3 == null)
				{
					return;
				}
				this.SetHeroHeadIcon(component3, formScript, dataByKey3);
				break;
			}
			case 4:
			{
				SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
				if (curLvelContext != null && curLvelContext.IsFireHolePlayMode())
				{
					component3.SetSprite(CSignalTipShower.S_kn_dragon_Icon_3v3, formScript, true, false, false, false);
				}
				else
				{
					component3.SetSprite(KillNotify.soldier_bigdragon_icon, formScript, true, false, false, false);
				}
				break;
			}
			case 5:
			{
				SLevelContext curLvelContext2 = Singleton<BattleLogic>.instance.GetCurLvelContext();
				if (curLvelContext2 != null && curLvelContext2.IsFireHolePlayMode())
				{
					component3.SetSprite(CSignalTipShower.S_kn_dragon_Icon_3v3, formScript, true, false, false, false);
				}
				else
				{
					component3.SetSprite(CSignalTipShower.S_kn_dragon_Icon, formScript, true, false, false, false);
				}
				break;
			}
			case 6:
				component3.SetSprite(CSignalTipShower.S_kn_dragon_Icon_3v3, formScript, true, false, false, false);
				break;
			default:
				return;
			}
			this.rightIcon.CustomSetActive(true);
		}
		else
		{
			this.rightIcon.CustomSetActive(false);
		}
	}

	private void Show(CSignalTips_InBatMsg data, CUIFormScript formScript)
	{
		if (data == null || formScript == null)
		{
			return;
		}
		if (this.signal_node != null)
		{
			this.signal_node.CustomSetActive(false);
		}
		if (this.inbattlemsg_node != null)
		{
			this.inbattlemsg_node.CustomSetActive(true);
		}
		ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(data.heroID);
		if (dataByKey == null)
		{
			return;
		}
		if (this.leftIcon == null)
		{
			return;
		}
		this.SetHeroHeadIcon(this.leftIcon.gameObject.GetComponent<Image>(), formScript, dataByKey);
		Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
		Player playerByUid = Singleton<GamePlayerCenter>.GetInstance().GetPlayerByUid(data.playerID);
		if (hostPlayer == null || playerByUid == null)
		{
			return;
		}
		string text = (playerByUid == hostPlayer) ? CSignalTipShower.S_Bg_Green : CSignalTipShower.S_Bg_Blue;
		if (this.bg_icon == null)
		{
			return;
		}
		Image component = this.bg_icon.GetComponent<Image>();
		if (component != null && !string.IsNullOrEmpty(text))
		{
			component.SetSprite(text, formScript, true, false, false, false);
		}
		if (this.inbattlemsg_txt != null)
		{
			this.inbattlemsg_txt.set_text(data.content);
			this.inbattlemsg_txt.gameObject.CustomSetActive(true);
		}
		if (this.rightIcon != null)
		{
			this.rightIcon.CustomSetActive(false);
		}
		if (this.signal_icon != null)
		{
			this.signal_icon.CustomSetActive(false);
		}
	}

	private void SetHeroHeadIcon(Image img, CUIFormScript formScript, ResHeroCfgInfo heroCfgInfo)
	{
		if (img == null || formScript == null || heroCfgInfo == null)
		{
			return;
		}
		string prefabPath = CUIUtility.s_Sprite_Dynamic_BustCircle_Dir + heroCfgInfo.szImagePath;
		img.SetSprite(prefabPath, formScript, true, false, false, false);
	}
}
