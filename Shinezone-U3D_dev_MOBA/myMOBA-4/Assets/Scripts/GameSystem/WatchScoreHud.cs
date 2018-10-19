using Assets.Scripts.GameLogic;
using CSProtocol;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class WatchScoreHud
	{
		private GameObject _root;

		private int _lastTime;

		private Text _timeText;

		private Text _campScoreText_1;

		private Text _campScoreText_2;

		private Text _campMoneyText_1;

		private Text _campMoneyText_2;

		private Text _campTowerText_1;

		private Text _campTowerText_2;

		private Text _campDragonText_1;

		private Text _campDragonText_2;

		public WatchScoreHud(GameObject root)
		{
			this._root = root;
			this._timeText = Utility.GetComponetInChild<Text>(root, "Time");
			this._campScoreText_1 = Utility.GetComponetInChild<Text>(root, "Camp_1/Score");
			this._campScoreText_2 = Utility.GetComponetInChild<Text>(root, "Camp_2/Score");
			this._campMoneyText_1 = Utility.GetComponetInChild<Text>(root, "Camp_1/Money");
			this._campMoneyText_2 = Utility.GetComponetInChild<Text>(root, "Camp_2/Money");
			this._campTowerText_1 = Utility.GetComponetInChild<Text>(root, "Camp_1/Tower");
			this._campTowerText_2 = Utility.GetComponetInChild<Text>(root, "Camp_2/Tower");
			this._campDragonText_1 = Utility.GetComponetInChild<Text>(root, "Camp_1/Dragon");
			this._campDragonText_2 = Utility.GetComponetInChild<Text>(root, "Camp_2/Dragon");
			this._lastTime = 0;
			this._timeText.text = string.Format("{0:D2}:{1:D2}", 0, 0);
			this._campTowerText_1.text = "0";
			this._campTowerText_2.text = "0";
			this._campDragonText_1.text = "0";
			this._campDragonText_2.text = "0";
			this._campScoreText_1.text = "0";
			this._campScoreText_2.text = "0";
			Singleton<EventRouter>.instance.AddEventHandler(EventID.BATTLE_TOWER_DESTROY_CHANGED, new Action(this.OnCampTowerChange));
			Singleton<EventRouter>.instance.AddEventHandler(EventID.BATTLE_DRAGON_KILL_CHANGED, new Action(this.OnCampDragonChange));
			Singleton<GameEventSys>.instance.AddEventHandler<SCampScoreUpdateParam>(GameEventDef.Event_CampScoreUpdated, new RefAction<SCampScoreUpdateParam>(this.OnBattleScoreChange));
		}

		public void Clear()
		{
			Singleton<GameEventSys>.instance.RmvEventHandler<SCampScoreUpdateParam>(GameEventDef.Event_CampScoreUpdated, new RefAction<SCampScoreUpdateParam>(this.OnBattleScoreChange));
			Singleton<EventRouter>.instance.RemoveEventHandler(EventID.BATTLE_TOWER_DESTROY_CHANGED, new Action(this.OnCampTowerChange));
			Singleton<EventRouter>.instance.RemoveEventHandler(EventID.BATTLE_DRAGON_KILL_CHANGED, new Action(this.OnCampDragonChange));
		}

		public void LateUpdate()
		{
			int num = Singleton<BattleLogic>.GetInstance().CalcCurrentTime();
			if (num != this._lastTime)
			{
				this._lastTime = num;
				this._timeText.text = string.Format("{0:D2}:{1:D2}", num / 60, num % 60);
			}
		}

		private void OnBattleScoreChange(ref SCampScoreUpdateParam param)
		{
			if (!this._root)
			{
				this.Clear();
				return;
			}
			if (param.HeadPoints < 0)
			{
				return;
			}
			SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
			if (param.CampType == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
			{
				if (curLvelContext.m_headPtsUpperLimit > 0)
				{
					this._campScoreText_1.text = string.Format(Singleton<CTextManager>.GetInstance().GetText("ScoreBoard_FireHole_1"), param.HeadPoints, curLvelContext.m_headPtsUpperLimit);
				}
				else
				{
					this._campScoreText_1.text = string.Format(Singleton<CTextManager>.GetInstance().GetText("ScoreBoard_Normal_1"), param.HeadPoints);
				}
			}
			else if (param.CampType == COM_PLAYERCAMP.COM_PLAYERCAMP_2)
			{
				if (curLvelContext.m_headPtsUpperLimit > 0)
				{
					this._campScoreText_2.text = string.Format(Singleton<CTextManager>.GetInstance().GetText("ScoreBoard_FireHole_2"), param.HeadPoints, curLvelContext.m_headPtsUpperLimit);
				}
				else
				{
					this._campScoreText_2.text = string.Format(Singleton<CTextManager>.GetInstance().GetText("ScoreBoard_Normal_2"), param.HeadPoints);
				}
			}
		}

		public void ValidateMoney(int campMoney_1, int campMoney_2)
		{
			this._campMoneyText_1.text = string.Format("{0:N1}k", (float)campMoney_1 * 0.001f);
			this._campMoneyText_2.text = string.Format("{0:N1}k", (float)campMoney_2 * 0.001f);
		}

		private void OnCampTowerChange()
		{
			if (!this._root)
			{
				this.Clear();
				return;
			}
			this._campTowerText_1.text = Singleton<BattleLogic>.GetInstance().battleStat.GetCampInfoByCamp(COM_PLAYERCAMP.COM_PLAYERCAMP_1).destoryTowers.ToString();
			this._campTowerText_2.text = Singleton<BattleLogic>.GetInstance().battleStat.GetCampInfoByCamp(COM_PLAYERCAMP.COM_PLAYERCAMP_2).destoryTowers.ToString();
		}

		private void OnCampDragonChange()
		{
			if (!this._root)
			{
				this.Clear();
				return;
			}
			int num = 0;
			int num2 = 0;
			CPlayerKDAStat playerKDAStat = Singleton<BattleLogic>.GetInstance().battleStat.m_playerKDAStat;
			DictionaryView<uint, PlayerKDA>.Enumerator enumerator = playerKDAStat.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
				PlayerKDA value = current.Value;
				if (value.PlayerCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
				{
					ListView<HeroKDA>.Enumerator enumerator2 = value.GetEnumerator();
					while (enumerator2.MoveNext())
					{
						num += enumerator2.Current.numKillDragon + enumerator2.Current.numKillBaron;
					}
				}
				else if (value.PlayerCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_2)
				{
					ListView<HeroKDA>.Enumerator enumerator3 = value.GetEnumerator();
					while (enumerator3.MoveNext())
					{
						num2 += enumerator3.Current.numKillDragon + enumerator3.Current.numKillBaron;
					}
				}
			}
			this._campDragonText_1.text = num.ToString();
			this._campDragonText_2.text = num2.ToString();
		}
	}
}
