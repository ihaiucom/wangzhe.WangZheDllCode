using Assets.Scripts.GameLogic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class SoldierWave
	{
		private GameObject _root;

		private Text _waveText;

		private Text _countdownText;

		private Text _countdownTitle;

		private int _totalWave;

		private int _currentWave;

		private int _countdown;

		public void Init(GameObject obj)
		{
			this._root = obj;
			this._waveText = Utility.GetComponetInChild<Text>(obj, "WavesContent");
			this._countdownText = Utility.GetComponetInChild<Text>(obj, "CounterContent");
			this._countdownTitle = Utility.GetComponetInChild<Text>(obj, "CounterTitle");
			SoldierRegion soldirRegion = Singleton<BattleLogic>.GetInstance().mapLogic.GetSoldirRegion();
			if (soldirRegion)
			{
				this._totalWave = soldirRegion.GetTotalCount();
			}
			this._waveText.set_text(string.Format("{0}/{1}", this._currentWave, this._totalWave));
			Singleton<GameEventSys>.instance.AddEventHandler<SoldierWaveParam>(GameEventDef.Event_SoldierWaveNext, new RefAction<SoldierWaveParam>(this.OnNextWave));
			Singleton<GameEventSys>.instance.AddEventHandler<SoldierWaveParam>(GameEventDef.Event_SoldierWaveNextRepeat, new RefAction<SoldierWaveParam>(this.OnNextRepeat));
		}

		public void Clear()
		{
			Singleton<GameEventSys>.instance.RmvEventHandler<SoldierWaveParam>(GameEventDef.Event_SoldierWaveNextRepeat, new RefAction<SoldierWaveParam>(this.OnNextRepeat));
			Singleton<GameEventSys>.instance.RmvEventHandler<SoldierWaveParam>(GameEventDef.Event_SoldierWaveNext, new RefAction<SoldierWaveParam>(this.OnNextWave));
			this._totalWave = 0;
			this._currentWave = 0;
			this._countdown = 0;
		}

		public void Show()
		{
			this._root.CustomSetActive(true);
		}

		public void Hide()
		{
			this._root.CustomSetActive(false);
		}

		public void Update()
		{
			MapWrapper mapLogic = Singleton<BattleLogic>.GetInstance().mapLogic;
			if (!mapLogic.DoesSoldierOverNum())
			{
				SoldierRegion soldirRegion = mapLogic.GetSoldirRegion();
				DebugHelper.Assert(soldirRegion != null, "region 不能为空");
				if (soldirRegion != null && soldirRegion.isStartup)
				{
					int num = (int)(Time.deltaTime * 1000f);
					this._countdown -= num;
					if (this._countdown <= 0)
					{
						this._countdown = 0;
					}
				}
			}
			if (this._countdownText != null && this._countdownText.gameObject.activeSelf)
			{
				int num2 = 0;
				int num3 = 0;
				this.CalcMinSec(this._countdown, out num3, out num2);
				this._countdownText.set_text(string.Format("{0:D2} : {1:D2}", num2, num3));
			}
		}

		private void OnNextRepeat(ref SoldierWaveParam inParam)
		{
			int currentWave = this._currentWave;
			SoldierRegion soldirRegion = Singleton<BattleLogic>.GetInstance().mapLogic.GetSoldirRegion();
			if (soldirRegion)
			{
				this._currentWave = soldirRegion.GetRepeatCountTill(inParam.WaveIndex);
			}
			this._waveText.set_text(string.Format("{0}/{1}", this._currentWave, this._totalWave));
			if (currentWave != this._currentWave)
			{
				if (inParam.NextDuration >= 0 && this._countdownText.gameObject.activeSelf)
				{
					this._countdown = inParam.NextDuration;
				}
				else
				{
					this._countdownText.gameObject.CustomSetActive(false);
					this._countdownTitle.gameObject.CustomSetActive(false);
				}
			}
		}

		private void OnNextWave(ref SoldierWaveParam inParam)
		{
			if (inParam.NextDuration >= 0 && this._countdownText.gameObject.activeSelf)
			{
				this._countdown = inParam.NextDuration;
			}
			else
			{
				this._countdownText.gameObject.CustomSetActive(false);
				this._countdownTitle.gameObject.CustomSetActive(false);
			}
		}

		private void CalcMinSec(int inMs, out int outSec, out int outMin)
		{
			outSec = inMs / 1000;
			outMin = outSec / 60;
			outSec %= 60;
		}
	}
}
