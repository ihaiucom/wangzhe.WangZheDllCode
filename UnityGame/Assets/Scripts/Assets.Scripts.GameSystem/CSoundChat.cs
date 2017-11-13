using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.Sound;
using Assets.Scripts.UI;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class CSoundChat
	{
		private GameObject tipObj;

		private Text tipText;

		private int m_timer;

		private bool bTimerInCD;

		private Image m_cooldownImage;

		private ulong m_startCooldownTimestamp;

		public void Init(GameObject cooldownImage, GameObject tipObj)
		{
			this.tipObj = tipObj;
			this.tipObj.CustomSetActive(false);
			this.tipText = this.tipObj.transform.Find("Text").GetComponent<Text>();
			this.m_cooldownImage = cooldownImage.GetComponent<Image>();
			this.m_timer = Singleton<CTimerManager>.instance.AddTimer(MonoSingleton<VoiceSys>.GetInstance().TotalVoiceTime * 1000, -1, new CTimer.OnTimeUpHandler(this.OnTimerEnd));
			Singleton<CTimerManager>.instance.PauseTimer(this.m_timer);
			Singleton<CTimerManager>.instance.ResetTimer(this.m_timer);
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_Voice_Btn, new CUIEventManager.OnUIEventHandler(this.OnBattle_Voice_Btn));
		}

		public void Clear()
		{
			this.tipObj = null;
			this.tipText = null;
			Singleton<CTimerManager>.instance.RemoveTimer(this.m_timer);
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_Voice_Btn, new CUIEventManager.OnUIEventHandler(this.OnBattle_Voice_Btn));
		}

		public void OnBattle_Voice_Btn(CUIEvent uiEvent)
		{
			if (Singleton<CBattleGuideManager>.instance.bPauseGame)
			{
				return;
			}
			if (CFakePvPHelper.bInFakeSelect)
			{
				if (!MonoSingleton<VoiceSys>.GetInstance().IsUseVoiceSysSetting)
				{
					this.tipText.set_text("语音聊天未开启，请在设置界面中打开");
					if (this.tipObj == null)
					{
						return;
					}
					if (this.bTimerInCD)
					{
						return;
					}
					Singleton<CSoundManager>.instance.PlayBattleSound2D("UI_common_tishi");
					this.StartTimer();
					this.tipObj.CustomSetActive(true);
					this.StartCooldown(2000u);
					return;
				}
			}
			else if (!MonoSingleton<VoiceSys>.GetInstance().GlobalVoiceSetting)
			{
				this.tipText.set_text("暂时无法连接语音服务器，请稍后尝试");
				if (this.tipObj == null)
				{
					return;
				}
				if (this.bTimerInCD)
				{
					return;
				}
				Singleton<CSoundManager>.instance.PlayBattleSound2D("UI_common_tishi");
				this.StartTimer();
				this.tipObj.CustomSetActive(true);
				this.StartCooldown(2000u);
				return;
			}
			else if (!MonoSingleton<VoiceSys>.GetInstance().IsUseVoiceSysSetting)
			{
				this.tipText.set_text("语音聊天未开启，请在设置界面中打开");
				if (this.tipObj == null)
				{
					return;
				}
				if (this.bTimerInCD)
				{
					return;
				}
				Singleton<CSoundManager>.instance.PlayBattleSound2D("UI_common_tishi");
				this.StartTimer();
				this.tipObj.CustomSetActive(true);
				this.StartCooldown(2000u);
				return;
			}
			else if (!MonoSingleton<VoiceSys>.GetInstance().IsInVoiceRoom())
			{
				this.tipText.set_text("语音服务器未连接");
				if (this.tipObj == null)
				{
					return;
				}
				if (this.bTimerInCD)
				{
					return;
				}
				Singleton<CSoundManager>.instance.PlayBattleSound2D("UI_common_tishi");
				this.StartTimer();
				this.tipObj.CustomSetActive(true);
				this.StartCooldown(2000u);
				return;
			}
			if (this.tipObj == null)
			{
				return;
			}
			if (this.bTimerInCD)
			{
				return;
			}
			Singleton<CSoundManager>.instance.PlayBattleSound2D("UI_common_tishi");
			this.StartTimer();
			this.tipObj.CustomSetActive(false);
			MonoSingleton<VoiceSys>.GetInstance().OpenSoundInBattle();
			this.StartCooldown((uint)(MonoSingleton<VoiceSys>.GetInstance().TotalVoiceTime * 1000));
		}

		private void OnTimerEnd(int timersequence)
		{
			if (this.tipObj == null)
			{
				return;
			}
			this.bTimerInCD = false;
			Singleton<CTimerManager>.instance.PauseTimer(this.m_timer);
			Singleton<CTimerManager>.instance.ResetTimer(this.m_timer);
			this.tipObj.CustomSetActive(false);
			MonoSingleton<VoiceSys>.GetInstance().CloseSoundInBattle();
			this.EndCooldown();
		}

		private void StartTimer()
		{
			this.bTimerInCD = true;
			Singleton<CTimerManager>.instance.ResumeTimer(this.m_timer);
		}

		private void StartCooldown(uint maxCooldownTime)
		{
			if (this.m_cooldownImage != null)
			{
				if (maxCooldownTime > 0u)
				{
					this.m_cooldownImage.enabled = true;
					this.m_cooldownImage.set_type(3);
					this.m_cooldownImage.set_fillMethod(4);
					this.m_cooldownImage.set_fillOrigin(2);
					this.m_startCooldownTimestamp = Singleton<FrameSynchr>.GetInstance().LogicFrameTick;
					this.m_cooldownImage.CustomFillAmount(1f);
				}
				else
				{
					this.m_startCooldownTimestamp = 0uL;
					this.m_cooldownImage.enabled = false;
				}
			}
		}

		private void EndCooldown()
		{
			this.m_startCooldownTimestamp = 0uL;
			if (this.m_cooldownImage)
			{
				this.m_cooldownImage.enabled = false;
			}
		}

		public void Update()
		{
			this.UpdateCooldown();
		}

		private void UpdateCooldown()
		{
			if (this.m_startCooldownTimestamp == 0uL)
			{
				return;
			}
			uint num = (uint)(Singleton<FrameSynchr>.GetInstance().LogicFrameTick - this.m_startCooldownTimestamp);
			if ((ulong)num >= (ulong)((long)(MonoSingleton<VoiceSys>.GetInstance().TotalVoiceTime * 1000)))
			{
				this.m_startCooldownTimestamp = 0uL;
				if (this.m_cooldownImage != null)
				{
					this.m_cooldownImage.enabled = false;
				}
			}
			else if (this.m_cooldownImage != null)
			{
				float value = (float)((long)(MonoSingleton<VoiceSys>.GetInstance().TotalVoiceTime * 1000) - (long)((ulong)num)) * 1f / (float)(MonoSingleton<VoiceSys>.GetInstance().TotalVoiceTime * 1000);
				this.m_cooldownImage.CustomFillAmount(value);
			}
		}
	}
}
