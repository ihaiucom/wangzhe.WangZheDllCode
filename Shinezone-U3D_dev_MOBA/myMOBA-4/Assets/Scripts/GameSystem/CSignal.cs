using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.Sound;
using Assets.Scripts.UI;
using ResData;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameSystem
{
	public class CSignal
	{
		public uint m_playerID;

		public int m_signalID;

		public ResSignalInfo m_signalInfo;

		public uint m_heroID;

		public Vector3 m_worldPosition;

		public Vector2 m_localPosition;

		public Quaternion m_localRotation;

		private bool bUseRotate;

		public bool m_bUseSignal;

		private GameObject m_effectInScene;

		private UIParticleInfo m_signalInUIEffect;

		private PoolObjHandle<ActorRoot> m_signalRelatedActor;

		public bool bSmall = true;

		public bool bUseCfgSound;

		private float m_duringTime;

		private float m_maxDuringTime;

		private MinimapSys.ElementType m_type;

		private GameObject m_signalInUi;

		public CSignal(uint playerID, int signalID, int worldPositionX, int worldPositionY, int worldPositionZ, bool bUseSignal, bool bSmall, bool bUseCfgSound)
		{
			this.m_playerID = playerID;
			this.m_signalID = signalID;
			this.bSmall = bSmall;
			this.bUseCfgSound = bUseCfgSound;
			this.m_worldPosition = new Vector3((float)worldPositionX, (float)worldPositionY, (float)worldPositionZ);
			this.m_bUseSignal = bUseSignal;
			this.m_effectInScene = null;
			this.m_signalInUIEffect = null;
			this.m_signalInUi = null;
		}

		public CSignal(PoolObjHandle<ActorRoot> followActor, int signalID, bool bUseSignal, bool bSmall, bool bUseCfgSound)
		{
			this.m_playerID = 0u;
			this.m_signalID = signalID;
			this.m_signalRelatedActor = followActor;
			this.bSmall = bSmall;
			this.bUseCfgSound = bUseCfgSound;
			this.m_worldPosition = Vector3.zero;
			this.m_bUseSignal = bUseSignal;
			this.m_effectInScene = null;
			this.m_signalInUIEffect = null;
			this.m_signalInUi = null;
		}

		public CSignal(int signalID, Vector2 localPosition, bool bUseSignal, bool bSmall, bool bUseCfgSound, bool useRotation, Quaternion localRotation)
		{
			this.m_playerID = 0u;
			this.m_signalID = signalID;
			this.bSmall = bSmall;
			this.bUseCfgSound = bUseCfgSound;
			this.m_worldPosition = Vector3.zero;
			this.m_bUseSignal = bUseSignal;
			this.m_localPosition = localPosition;
			this.bUseRotate = useRotation;
			if (!this.bUseRotate)
			{
				this.m_localRotation = Quaternion.identity;
			}
			else
			{
				this.m_localRotation = localRotation;
			}
			this.m_effectInScene = null;
			this.m_signalInUIEffect = null;
			this.m_signalInUi = null;
		}

		public static void Preload(ref ActorPreloadTab preloadTab)
		{
			GameDataMgr.signalDatabin.Reload();
			Dictionary<long, object>.Enumerator enumerator = GameDataMgr.signalDatabin.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<long, object> current = enumerator.Current;
				ResSignalInfo resSignalInfo = (ResSignalInfo)current.Value;
				if (resSignalInfo != null && !string.IsNullOrEmpty(resSignalInfo.szRealEffect))
				{
					preloadTab.AddSprite(resSignalInfo.szRealEffect);
				}
				if (resSignalInfo != null && !string.IsNullOrEmpty(resSignalInfo.szUIIcon))
				{
					preloadTab.AddSprite(resSignalInfo.szUIIcon);
				}
			}
		}

		public void Initialize(CUIFormScript formScript, ResSignalInfo signalInfo)
		{
			if (this.m_playerID > 0u)
			{
				Player player = Singleton<GamePlayerCenter>.GetInstance().GetPlayer(this.m_playerID);
				if (player != null)
				{
					this.m_signalRelatedActor = player.Captain;
				}
			}
			this.m_signalInfo = signalInfo;
			if (this.m_signalInfo == null || formScript == null)
			{
				this.m_duringTime = 0f;
				this.m_maxDuringTime = 0f;
				return;
			}
			this.m_duringTime = 0f;
			this.m_maxDuringTime = (float)this.m_signalInfo.bTime;
			if (this.m_signalInfo.bSignalType == 0 && !string.IsNullOrEmpty(this.m_signalInfo.szSceneEffect))
			{
				this.m_effectInScene = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD(this.m_signalInfo.szSceneEffect, true, SceneObjType.Temp, this.m_worldPosition);
				this.m_effectInScene.CustomSetActive(true);
			}
			if (this.m_bUseSignal && !string.IsNullOrEmpty(this.m_signalInfo.szUIIcon) && this.m_signalInfo.bSignalType != 2)
			{
				if (this.m_signalInUi == null)
				{
					this.m_signalInUi = MiniMapSysUT.GetSignalGameObject(true);
				}
				if (this.m_signalInUi != null)
				{
					Vector3 vector = this.m_worldPosition;
					if (this.m_signalInfo.bSignalType == 1 && this.m_signalRelatedActor)
					{
						vector = (Vector3)this.m_signalRelatedActor.handle.location;
					}
					float x;
					float y;
					this.m_signalInUi.transform.position = MiniMapSysUT.Set3DUIWorldPos_ByScreenPoint(ref vector, true, out x, out y);
					if (!string.IsNullOrEmpty(this.m_signalInfo.szRealEffect) && this.m_signalInUi != null && Singleton<CBattleSystem>.instance.TheMinimapSys.CurMapType() == MinimapSys.EMapType.Mini)
					{
						Vector2 vector2 = new Vector2(x, y);
						if (Singleton<CBattleSystem>.instance.FightForm.GetSignalPanel().CheckSignalPositionValid(vector2))
						{
							this.m_signalInUIEffect = Singleton<CUIParticleSystem>.instance.AddParticle(this.m_signalInfo.szRealEffect, (float)this.m_signalInfo.bTime, vector2, null);
						}
					}
				}
			}
			if (this.m_bUseSignal && this.m_signalInfo.bSignalType == 2)
			{
				if (this.m_signalInUi == null)
				{
					this.m_signalInUi = MiniMapSysUT.GetSignalGameObject(true);
				}
				if (this.m_signalInUi != null && !string.IsNullOrEmpty(this.m_signalInfo.szRealEffect) && Singleton<CBattleSystem>.instance.TheMinimapSys.CurMapType() == MinimapSys.EMapType.Mini && Singleton<CBattleSystem>.instance.FightForm.GetSignalPanel().CheckSignalPositionValid(this.m_localPosition))
				{
					this.m_signalInUIEffect = Singleton<CUIParticleSystem>.instance.AddParticle(this.m_signalInfo.szRealEffect, (float)this.m_signalInfo.bTime, this.m_localPosition, new Quaternion?(this.m_localRotation));
				}
			}
			if (this.bUseCfgSound)
			{
				string text = StringHelper.UTF8BytesToString(ref this.m_signalInfo.szSound);
				if (!string.IsNullOrEmpty(text))
				{
					Singleton<CSoundManager>.GetInstance().PlayBattleSound2D(text);
				}
			}
		}

		public void Update(CUIFormScript formScript, float deltaTime)
		{
			if (this.m_duringTime < this.m_maxDuringTime)
			{
				this.m_duringTime += deltaTime;
				if (this.bSignalFollowActor())
				{
					Vector3 vector = (Vector3)this.m_signalRelatedActor.handle.location;
					float x;
					float y;
					this.m_signalInUi.transform.position = MiniMapSysUT.Set3DUIWorldPos_ByScreenPoint(ref vector, true, out x, out y);
					if (this.m_signalInUIEffect != null && this.m_signalInUIEffect.parObj != null)
					{
						Vector2 vector2 = new Vector2(x, y);
						Singleton<CUIParticleSystem>.GetInstance().SetParticleScreenPosition(this.m_signalInUIEffect, ref vector2);
					}
				}
			}
		}

		private bool bSignalFollowActor()
		{
			return this.m_signalInfo != null && this.m_signalInfo.bSignalType == 1 && this.m_signalRelatedActor && this.m_signalInUi != null;
		}

		public bool IsNeedDisposed()
		{
			return this.m_duringTime >= this.m_maxDuringTime;
		}

		public void Dispose()
		{
			if (this.m_effectInScene != null)
			{
				Singleton<CGameObjectPool>.GetInstance().RecycleGameObject(this.m_effectInScene);
				this.m_effectInScene = null;
			}
			MiniMapSysUT.RecycleMapGameObject(this.m_signalInUi);
			this.m_signalInUi = null;
			this.m_effectInScene = null;
			this.m_signalInUIEffect = null;
			this.m_bUseSignal = false;
		}
	}
}
