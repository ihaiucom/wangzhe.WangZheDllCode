using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameKernal;
using CSProtocol;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class ScoreBoard
	{
		private GameObject _root;

		private GameObject _rootTime;

		private Text _leftScore;

		private Text _rightScore;

		private Text _txtTime;

		private int _lastTimeSec;

		private SLevelContext levelContext;

		private bool m_battleKDAChanged;

		private bool m_battleKDAChangedByActorDead;

		private PoolObjHandle<ActorRoot> m_kingActor;

		private int m_lastMostKill;

		private int _duration;

		private int _startTime;

		public void Init(GameObject obj, GameObject objTime)
		{
			this.levelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			if (this.levelContext == null)
			{
				return;
			}
			this._root = obj;
			this._rootTime = objTime;
			this._leftScore = Utility.GetComponetInChild<Text>(obj, "LeftScore");
			this._rightScore = Utility.GetComponetInChild<Text>(obj, "RightScore");
			this._txtTime = Utility.GetComponetInChild<Text>(objTime, "TxtTime");
			this._lastTimeSec = 0;
			Utility.GetComponetInChild<Text>(this._root, "Kill").text = "0";
			Utility.GetComponetInChild<Text>(this._root, "Death").text = "0";
			Utility.GetComponetInChild<Text>(this._root, "Assist").text = "0";
		}

		public void Clear()
		{
			int num = this._duration / 60;
			int num2 = this._duration - num * 60;
			if (num2 < 0)
			{
				num2 = 0;
			}
			string duration = string.Format("{0:D2}:{1:D2}", num, num2);
			DateTime dateTime = Utility.ToUtcTime2Local((long)this._startTime);
			string startTime = string.Format("{0:D4}/{1:D2}/{2:D2} {3:D2}:{4:D2}", new object[]
			{
				dateTime.Year,
				dateTime.Month,
				dateTime.Day,
				dateTime.Hour,
				dateTime.Minute
			});
			Singleton<SettlementSystem>.instance.SetLastMatchDuration(duration, startTime, (uint)this._startTime);
			this._duration = 0;
			this._startTime = 0;
			DictionaryView<uint, CampInfo> campStat = Singleton<BattleLogic>.GetInstance().battleStat.GetCampStat();
			if (campStat.ContainsKey(1u))
			{
				campStat[1u].onCampScoreChanged -= new CampInfo.CampInfoValueChanged(this.validateScore);
			}
			if (campStat.ContainsKey(2u))
			{
				campStat[2u].onCampScoreChanged -= new CampInfo.CampInfoValueChanged(this.validateScore);
			}
			Singleton<EventRouter>.instance.RemoveEventHandler(EventID.BATTLE_KDA_CHANGED, new Action(this.OnBattleKDAChanged));
			Singleton<EventRouter>.instance.RemoveEventHandler(EventID.BATTLE_KDA_CHANGED_BY_ACTOR_DEAD, new Action(this.OnBattleKDAChangedByActorDead));
			this._root = null;
			this._leftScore = null;
			this._rightScore = null;
			this._txtTime = null;
			this.levelContext = null;
			this.m_kingActor.Release();
			this.m_lastMostKill = 0;
		}

		public void RegiseterEvent()
		{
			DictionaryView<uint, CampInfo> campStat = Singleton<BattleLogic>.GetInstance().battleStat.GetCampStat();
			if (campStat.ContainsKey(1u))
			{
				campStat[1u].onCampScoreChanged += new CampInfo.CampInfoValueChanged(this.validateScore);
			}
			if (campStat.ContainsKey(2u))
			{
				campStat[2u].onCampScoreChanged += new CampInfo.CampInfoValueChanged(this.validateScore);
			}
			Singleton<EventRouter>.instance.AddEventHandler(EventID.BATTLE_KDA_CHANGED, new Action(this.OnBattleKDAChanged));
			Singleton<EventRouter>.instance.AddEventHandler(EventID.BATTLE_KDA_CHANGED_BY_ACTOR_DEAD, new Action(this.OnBattleKDAChangedByActorDead));
		}

		public void LateUpdate()
		{
			if (this.m_battleKDAChanged)
			{
				this.UpdateSelfKDADisplay();
				this.UpdateKingOfKiller();
				this.m_battleKDAChanged = false;
			}
			if (this.m_battleKDAChangedByActorDead)
			{
				this.UpdateMVPHonor();
				this.m_battleKDAChangedByActorDead = false;
			}
		}

		public void Show()
		{
			this._root.CustomSetActive(true);
			this._rootTime.CustomSetActive(true);
			DictionaryView<uint, CampInfo> campStat = Singleton<BattleLogic>.GetInstance().battleStat.GetCampStat();
			if (campStat.ContainsKey(1u))
			{
				this.validateScore(COM_PLAYERCAMP.COM_PLAYERCAMP_1, campStat[1u].campScore, campStat[1u].HeadPoints);
			}
			if (campStat.ContainsKey(2u))
			{
				this.validateScore(COM_PLAYERCAMP.COM_PLAYERCAMP_2, campStat[2u].campScore, campStat[2u].HeadPoints);
			}
		}

		public void Hide()
		{
			this._root.CustomSetActive(false);
			this._rootTime.CustomSetActive(false);
		}

		public bool IsShown()
		{
			return this._root.activeSelf;
		}

		public int GetStartTime()
		{
			return this._startTime;
		}

		public int GetDuration()
		{
			return this._duration;
		}

		public void Update()
		{
			int num = Singleton<BattleLogic>.GetInstance().CalcCurrentTime();
			this._duration = num;
			if (this._startTime == 0)
			{
				this._startTime = CRoleInfo.GetCurrentUTCTime();
			}
			if (num != this._lastTimeSec)
			{
				if (Singleton<WinLoseByStarSys>.instance.CurLevelTimeDuration <= 0u)
				{
					int num2 = num / 60;
					int num3 = num - num2 * 60;
					this._txtTime.text = string.Format("{0:D2}:{1:D2}", num2, num3);
				}
				else
				{
					uint num4 = Singleton<WinLoseByStarSys>.instance.CurLevelTimeDuration / 1000u;
					uint num5 = 0u;
					if (num4 > (uint)num)
					{
						num5 = num4 - (uint)num;
					}
					uint num6 = num5 / 60u;
					uint num7 = num5 - num6 * 60u;
					this._txtTime.text = string.Format("{0:D2}:{1:D2}", num6, num7);
				}
				this._lastTimeSec = num;
			}
		}

		private void validateScore(COM_PLAYERCAMP campType, int inCampScore, int inHeadPts)
		{
			if (inHeadPts < 0)
			{
				return;
			}
			if (campType == Singleton<GamePlayerCenter>.instance.GetHostPlayer().PlayerCamp)
			{
				if (this.levelContext.m_headPtsUpperLimit > 0)
				{
					this._leftScore.text = string.Format(Singleton<CTextManager>.GetInstance().GetText("ScoreBoard_FireHole_1"), inHeadPts, this.levelContext.m_headPtsUpperLimit);
				}
				else
				{
					this._leftScore.text = string.Format(Singleton<CTextManager>.GetInstance().GetText("ScoreBoard_Normal_1"), inHeadPts);
				}
			}
			else if (campType != COM_PLAYERCAMP.COM_PLAYERCAMP_MID && campType != COM_PLAYERCAMP.COM_PLAYERCAMP_COUNT)
			{
				if (this.levelContext.m_headPtsUpperLimit > 0)
				{
					this._rightScore.text = string.Format(Singleton<CTextManager>.GetInstance().GetText("ScoreBoard_FireHole_2"), inHeadPts, this.levelContext.m_headPtsUpperLimit);
				}
				else
				{
					this._rightScore.text = string.Format(Singleton<CTextManager>.GetInstance().GetText("ScoreBoard_Normal_2"), inHeadPts);
				}
			}
		}

		private void UpdateSelfKDADisplay()
		{
			CPlayerKDAStat playerKDAStat = Singleton<BattleLogic>.GetInstance().battleStat.m_playerKDAStat;
			if (playerKDAStat == null)
			{
				return;
			}
			PlayerKDA hostKDA = playerKDAStat.GetHostKDA();
			if (hostKDA == null || this._root == null)
			{
				return;
			}
			Utility.GetComponetInChild<Text>(this._root, "Kill").text = SimpleNumericString.GetNumeric(hostKDA.numKill);
			Utility.GetComponetInChild<Text>(this._root, "Death").text = SimpleNumericString.GetNumeric(hostKDA.numDead);
			Utility.GetComponetInChild<Text>(this._root, "Assist").text = SimpleNumericString.GetNumeric(hostKDA.numAssist);
		}

		private void UpdateKingOfKiller()
		{
			SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
			if (curLvelContext == null || curLvelContext.m_headPtsUpperLimit <= 0)
			{
				return;
			}
			CPlayerKDAStat playerKDAStat = Singleton<BattleLogic>.GetInstance().battleStat.m_playerKDAStat;
			if (playerKDAStat == null)
			{
				return;
			}
			uint playerId = 0u;
			int num = 0;
			DictionaryView<uint, PlayerKDA>.Enumerator enumerator = playerKDAStat.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
				if (current.Value != null)
				{
					KeyValuePair<uint, PlayerKDA> current2 = enumerator.Current;
					if (current2.Value.numKill > num)
					{
						KeyValuePair<uint, PlayerKDA> current3 = enumerator.Current;
						num = current3.Value.numKill;
						KeyValuePair<uint, PlayerKDA> current4 = enumerator.Current;
						playerId = current4.Value.PlayerId;
					}
				}
			}
			if (num > this.m_lastMostKill)
			{
				Player player = Singleton<GamePlayerCenter>.instance.GetPlayer(playerId);
				if (player != null && player.Captain != this.m_kingActor)
				{
					if (this.m_kingActor && this.m_kingActor.handle.EffectControl != null)
					{
						this.m_kingActor.handle.EffectControl.EndKingOfKillerEffect();
					}
					this.m_kingActor = player.Captain;
					if (this.m_kingActor && this.m_kingActor.handle.EffectControl != null)
					{
						this.m_kingActor.handle.EffectControl.StartKingOfKillerEffect();
					}
				}
				this.m_lastMostKill = num;
			}
		}

		private void UpdateMVPHonor()
		{
			SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
			if (curLvelContext == null || !curLvelContext.m_isShowHonor)
			{
				return;
			}
			if (Singleton<BattleLogic>.GetInstance().battleStat.m_playerKDAStat == null)
			{
				return;
			}
			uint lastBestPlayer = Singleton<BattleStatistic>.GetInstance().GetLastBestPlayer();
			uint bestPlayer = Singleton<BattleStatistic>.GetInstance().GetBestPlayer();
			if (bestPlayer != lastBestPlayer)
			{
				Player player = Singleton<GamePlayerCenter>.instance.GetPlayer(lastBestPlayer);
				if (player != null)
				{
					ActorRoot actorRoot = player.Captain;
					actorRoot.HudControl.EndHonorAni();
				}
			}
			Player player2 = Singleton<GamePlayerCenter>.instance.GetPlayer(bestPlayer);
			if (player2 != null)
			{
				ActorRoot actorRoot2 = player2.Captain;
				actorRoot2.HudControl.PlayHonorAni(player2.HonorId, player2.HonorLevel);
			}
		}

		private void OnBattleKDAChanged()
		{
			this.m_battleKDAChanged = true;
		}

		private void OnBattleKDAChangedByActorDead()
		{
			this.m_battleKDAChangedByActorDead = true;
		}
	}
}
