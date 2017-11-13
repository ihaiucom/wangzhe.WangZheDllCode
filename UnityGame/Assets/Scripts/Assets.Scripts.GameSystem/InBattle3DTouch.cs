using Assets.Scripts.Framework;
using Assets.Scripts.UI;
using System;
using UnityEngine;

namespace Assets.Scripts.GameSystem
{
	public class InBattle3DTouch
	{
		public void Init()
		{
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_3DTouch_FullScreen, new CUIEventManager.OnUIEventHandler(this.OnBattle_3DTouch_FullScreen));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_3DTouch_FullScreen_Scene, new CUIEventManager.OnUIEventHandler(this.OnBattle_3DTouch_FullScreen_Scene));
		}

		public void Clear()
		{
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_3DTouch_FullScreen, new CUIEventManager.OnUIEventHandler(this.OnBattle_3DTouch_FullScreen));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_3DTouch_FullScreen_Scene, new CUIEventManager.OnUIEventHandler(this.OnBattle_3DTouch_FullScreen_Scene));
		}

		private void OnBattle_3DTouch_FullScreen_Scene(CUIEvent uievent)
		{
			if (!GameSettings.Unity3DTouchEnable)
			{
				return;
			}
			if (uievent.m_pointerEventData == null)
			{
				return;
			}
			if (uievent.m_pointerEventData.get_position().x <= (float)Screen.width * 0.5f && uievent.m_pointerEventData.get_position().x >= 0f)
			{
				this.Process_MiniMap();
			}
			else
			{
				this.Process_InBattleShortCutMsg();
			}
		}

		private void OnBattle_3DTouch_FullScreen(CUIEvent uievent)
		{
			if (!GameSettings.Unity3DTouchEnable)
			{
				return;
			}
			this.Porcess_CloseOtherForm();
		}

		public bool Porcess_CloseOtherForm()
		{
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CBattleEquipSystem.s_equipFormPath);
			if (form != null && (!form.IsHided() || form.GetComponent<Canvas>().enabled))
			{
				Singleton<CUIManager>.instance.CloseForm(form);
				return true;
			}
			CUIFormScript form2 = Singleton<CUIManager>.instance.GetForm(CSettingsSys.SETTING_FORM);
			if (form2 != null && !form2.IsHided())
			{
				Singleton<CUIManager>.instance.CloseForm(form2);
				return true;
			}
			CUIFormScript form3 = Singleton<CUIManager>.instance.GetForm(BattleStatView.s_battleStateViewUIForm);
			if (form3 != null && (!form3.IsHided() || form3.GetComponent<Canvas>().enabled) && Singleton<CBattleSystem>.instance.BattleStatView != null)
			{
				Singleton<CBattleSystem>.instance.BattleStatView.Hide();
				return true;
			}
			return false;
		}

		public void Process_MiniMap()
		{
			MinimapSys theMinimapSys = Singleton<CBattleSystem>.GetInstance().TheMinimapSys;
			if (theMinimapSys == null)
			{
				return;
			}
			if (theMinimapSys.CurMapType() == MinimapSys.EMapType.Mini)
			{
				Singleton<CUIEventManager>.instance.DispatchUIEvent(enUIEventID.BigMap_Open_BigMap);
			}
		}

		public void Process_InBattleShortCutMsg()
		{
			if (Singleton<InBattleMsgMgr>.instance.m_shortcutChat != null)
			{
				Singleton<InBattleMsgMgr>.instance.m_shortcutChat.Send_Config_Chat(0);
			}
		}
	}
}
