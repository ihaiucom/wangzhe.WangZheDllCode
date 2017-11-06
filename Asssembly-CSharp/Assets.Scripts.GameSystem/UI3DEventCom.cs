using Assets.Scripts.UI;
using System;
using UnityEngine;

namespace Assets.Scripts.GameSystem
{
	public class UI3DEventCom
	{
		public Rect m_screenSize;

		public stUIEventParams m_eventParams;

		public enUIEventID m_eventID;

		public UIParticleInfo UIParticleInfo;

		public bool bCreateParticleByPosition;

		public bool bHostSameCamp;

		public bool isDead;

		public void CreateParticleByInsidePos()
		{
			if (this.UIParticleInfo == null && this.bCreateParticleByPosition)
			{
				this.bCreateParticleByPosition = false;
				this.UIParticleInfo = Singleton<CUIParticleSystem>.instance.AddParticle(MiniMapEffectModule.teleportEft, -1f, this.m_screenSize.center, default(Quaternion?));
				this.UIParticleInfo.parObj.CustomSetActive(true);
			}
		}

		public void Clear()
		{
			if (this.UIParticleInfo != null)
			{
				Singleton<CUIParticleSystem>.instance.RemoveParticle(this.UIParticleInfo);
			}
			this.UIParticleInfo = null;
			this.bCreateParticleByPosition = false;
		}
	}
}
