using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class ScoreboardPvE
	{
		private GameObject _root;

		private Text _txtTime;

		private int _lastTimeSec;

		public void Init(GameObject obj)
		{
			this._root = obj;
			this._txtTime = Utility.GetComponetInChild<Text>(obj, "TxtTime");
			this._lastTimeSec = 0;
		}

		public void Clear()
		{
			this._root = null;
			this._txtTime = null;
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
			int num = this.CalcCurrentTime();
			if (num != this._lastTimeSec)
			{
				int num2 = num / 60;
				int num3 = num - num2 * 60;
				this._txtTime.text = string.Format("{0:D2}:{1:D2}", num2, num3);
				this._lastTimeSec = num;
			}
		}

		private int CalcCurrentTime()
		{
			int num = (int)Singleton<FrameSynchr>.GetInstance().LogicFrameTick;
			SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
			if (curLvelContext != null && curLvelContext.m_isShowTrainingHelper && Singleton<BattleLogic>.instance.dynamicProperty != null)
			{
				num = (int)Singleton<BattleLogic>.instance.dynamicProperty.m_frameTimer;
			}
			return (int)((float)num * 1f / 1000f);
		}
	}
}
