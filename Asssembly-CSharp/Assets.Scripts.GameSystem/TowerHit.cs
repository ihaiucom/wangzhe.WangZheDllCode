using Assets.Scripts.Framework;
using Assets.Scripts.Sound;
using ResData;
using System;
using UnityEngine;

namespace Assets.Scripts.GameSystem
{
	public class TowerHit
	{
		public byte organ_type;

		public uint cd_time;

		public string voice;

		public string effect;

		public uint effect_last_time;

		private bool bValid;

		private int cd_timer = -1;

		public TowerHit(RES_ORGAN_TYPE type)
		{
			this.bValid = false;
			TowerHitConf dataByKey = GameDataMgr.towerHitDatabin.GetDataByKey((uint)((byte)type));
			DebugHelper.Assert(dataByKey != null, "TowerHit towerHitDatabin.GetDataByKey is null, type:" + type);
			if (dataByKey == null)
			{
				return;
			}
			this.organ_type = (byte)type;
			this.cd_time = dataByKey.dwCdTime;
			this.voice = dataByKey.szVoice;
			this.effect = dataByKey.szEffect;
			this.effect_last_time = dataByKey.dwLastTime;
			if (this.cd_time > 0u)
			{
				this.cd_timer = Singleton<CTimerManager>.instance.AddTimer((int)this.cd_time, -1, new CTimer.OnTimeUpHandler(this.On_CD_Timer_Finish));
				Singleton<CTimerManager>.instance.PauseTimer(this.cd_timer);
			}
			this.bValid = true;
		}

		public void Clear()
		{
			if (this.cd_timer != -1)
			{
				Singleton<CTimerManager>.instance.RemoveTimer(this.cd_timer);
			}
			this.cd_timer = -1;
		}

		public void TryActive(GameObject target)
		{
			if (target == null || !this.bValid)
			{
				return;
			}
			if (!string.IsNullOrEmpty(this.effect))
			{
				MinimapSys theMinimapSys = Singleton<CBattleSystem>.GetInstance().TheMinimapSys;
				if (theMinimapSys != null && theMinimapSys.CurMapType() == MinimapSys.EMapType.Mini)
				{
					TowerHit.PlayEffect(this.effect, 2f, target);
				}
			}
			if (!string.IsNullOrEmpty(this.voice))
			{
				Singleton<CSoundManager>.GetInstance().PlayBattleSound2D(this.voice);
			}
			Singleton<CTimerManager>.instance.ResumeTimer(this.cd_timer);
			this.bValid = false;
		}

		private void On_CD_Timer_Finish(int index)
		{
			this.bValid = true;
			Singleton<CTimerManager>.instance.PauseTimer(this.cd_timer);
		}

		public static void PlayEffect(string effect_name, float playTime, GameObject target)
		{
			if (target == null)
			{
				return;
			}
			Camera currentCamera = Singleton<Camera_UI3D>.GetInstance().GetCurrentCamera();
			if (currentCamera == null)
			{
				return;
			}
			Vector2 sreenLoc = currentCamera.WorldToScreenPoint(target.transform.position);
			Singleton<CUIParticleSystem>.instance.AddParticle(effect_name, playTime, sreenLoc, default(Quaternion?));
		}
	}
}
