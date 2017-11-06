using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;
using UnityEngine;

namespace Assets.Scripts.GameSystem
{
	public class MiniMapEffectModule
	{
		public class TimerElement
		{
			private int m_timer = -1;

			private GameObject obj;

			public void SetData(GameObject obj, int time)
			{
				this.m_timer = Singleton<CTimerManager>.instance.AddTimer(time, 1, new CTimer.OnTimeUpHandler(this.OnTimerEnd));
				this.obj = obj;
			}

			public void Clear()
			{
				if (this.m_timer != -1)
				{
					Singleton<CTimerManager>.instance.RemoveTimer(this.m_timer);
				}
				this.m_timer = -1;
				this.obj = null;
			}

			private void OnTimerEnd(int timerSequence)
			{
				Singleton<CGameObjectPool>.instance.RecycleGameObject(this.obj);
				this.Clear();
			}
		}

		public class Element
		{
			public UIParticleInfo uiParticleInfo;

			public PoolObjHandle<ActorRoot> m_signalRelatedActor;

			public Element(string eftName, float time, PoolObjHandle<ActorRoot> relatedActor)
			{
				if (!relatedActor)
				{
					return;
				}
				bool bMiniMap = Singleton<CBattleSystem>.instance.TheMinimapSys.CurMapType() == MinimapSys.EMapType.Mini;
				this.m_signalRelatedActor = relatedActor;
				Vector3 vector = (Vector3)this.m_signalRelatedActor.handle.location;
				Vector2 sreenLoc = MiniMapSysUT.CalcScreenPosInMapByWorldPos(ref vector, bMiniMap);
				this.uiParticleInfo = Singleton<CUIParticleSystem>.instance.AddParticle(eftName, time, sreenLoc, default(Quaternion?));
			}

			public void Clear()
			{
				Singleton<CUIParticleSystem>.instance.RemoveParticle(this.uiParticleInfo);
				this.uiParticleInfo = null;
			}

			public void SetVisible(bool bShow)
			{
				if (this.uiParticleInfo != null)
				{
					this.uiParticleInfo.parObj.CustomSetActive(bShow);
				}
			}

			public void Update(float deltaTime)
			{
				if (!this.m_signalRelatedActor)
				{
					return;
				}
				Vector3 vector = (Vector3)this.m_signalRelatedActor.handle.location;
				bool bMiniMap = Singleton<CBattleSystem>.instance.TheMinimapSys.CurMapType() == MinimapSys.EMapType.Mini;
				Vector2 vector2 = MiniMapSysUT.CalcScreenPosInMapByWorldPos(ref vector, bMiniMap);
				if (this.uiParticleInfo != null && this.uiParticleInfo.parObj != null)
				{
					Singleton<CUIParticleSystem>.GetInstance().SetParticleScreenPosition(this.uiParticleInfo, ref vector2);
				}
			}
		}

		public static string teleportUIEft_src = "Prefab_Skill_Effects/tongyong_effects/Sence_Effeft/Chuansong_cx_ui";

		public static string teleportUIEft_dst = "Prefab_Skill_Effects/tongyong_effects/Sence_Effeft/Chuansong_cx_ui_sf";

		public static string teleportUIEft_src_enemy = "Prefab_Skill_Effects/tongyong_effects/Sence_Effeft/Chuansong_cx_ui_red";

		public static string teleportUIEft_dst_enemy = "Prefab_Skill_Effects/tongyong_effects/Sence_Effeft/Chuansong_cx_ui_sf_red";

		public static string EyePerishEft = "Prefab_Skill_Effects/tongyong_effects/Indicator/yan_ui_01";

		public static string teleportEft = "Prefab_Skill_Effects/tongyong_effects/Sence_Effeft/Chuansong_jiantou_ui";

		private ListView<MiniMapEffectModule.Element> followEffectList = new ListView<MiniMapEffectModule.Element>();

		public GameObject PlaySceneEffect(string eft, int msTime, Vector3 worldpos)
		{
			GameObject pooledGameObjLOD = MonoSingleton<SceneMgr>.GetInstance().GetPooledGameObjLOD(eft, true, SceneObjType.Temp, worldpos);
			pooledGameObjLOD.CustomSetActive(true);
			MiniMapEffectModule.TimerElement timerElement = new MiniMapEffectModule.TimerElement();
			timerElement.SetData(pooledGameObjLOD, msTime);
			return pooledGameObjLOD;
		}

		public void PlayScreenPosEffect(string eft, float time, Vector2 screenPos)
		{
			if (string.IsNullOrEmpty(eft))
			{
				return;
			}
			Singleton<CUIParticleSystem>.instance.AddParticle(eft, time, screenPos, default(Quaternion?));
		}

		public void PlayFollowActorEffect(string eft, float time, PoolObjHandle<ActorRoot> relatedActor)
		{
			if (string.IsNullOrEmpty(eft) || !relatedActor)
			{
				return;
			}
			MiniMapEffectModule.Element followEffect = this.GetFollowEffect(eft, time, relatedActor);
			if (!this.followEffectList.Contains(followEffect))
			{
				this.followEffectList.Add(followEffect);
			}
		}

		public void StopFollowActorEffect(PoolObjHandle<ActorRoot> relatedActor)
		{
			this.StopFollowActorEffect(relatedActor.handle.TheActorMeta.PlayerId);
		}

		public void StopFollowActorEffect(uint PlayerId)
		{
			for (int i = 0; i < this.followEffectList.Count; i++)
			{
				MiniMapEffectModule.Element element = this.followEffectList[i];
				if (element.m_signalRelatedActor && element.m_signalRelatedActor.handle.TheActorMeta.PlayerId == PlayerId)
				{
					element.Clear();
				}
			}
		}

		public void Update(float deltaTime = 0f)
		{
			for (int i = 0; i < this.followEffectList.Count; i++)
			{
				MiniMapEffectModule.Element element = this.followEffectList[i];
				element.Update(deltaTime);
			}
		}

		public void Clear()
		{
			for (int i = 0; i < this.followEffectList.Count; i++)
			{
				MiniMapEffectModule.Element element = this.followEffectList[i];
				element.Clear();
			}
			this.followEffectList.Clear();
		}

		private MiniMapEffectModule.Element GetFollowEffect(string eft, float time, PoolObjHandle<ActorRoot> relatedActor)
		{
			return new MiniMapEffectModule.Element(eft, time, relatedActor);
		}
	}
}
