using Assets.Scripts.UI;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class ReplayControl
	{
		private GameObject _root;

		private GameObject _playBtn;

		private GameObject _pauseBtn;

		private GameObject _speedDownBtn;

		private GameObject _speedUpBtn;

		private Text _speedTxt;

		private Slider _progress;

		private Text _currentTimeTxt;

		private Text _totalTimeTxt;

		public GameObject Root
		{
			get
			{
				return this._root;
			}
		}

		public ReplayControl(GameObject root)
		{
			this._root = root;
			this._playBtn = Utility.FindChild(root, "PlayBtn");
			this._pauseBtn = Utility.FindChild(root, "PauseBtn");
			this._speedDownBtn = Utility.FindChild(root, "SpeedDownBtn");
			this._speedUpBtn = Utility.FindChild(root, "SpeedUpBtn");
			this._speedTxt = Utility.GetComponetInChild<Text>(root, "SpeedText");
			this._progress = Utility.GetComponetInChild<Slider>(root, "Progress");
			this._currentTimeTxt = Utility.GetComponetInChild<Text>(root, "CurrentTime");
			this._totalTimeTxt = Utility.GetComponetInChild<Text>(root, "TotalTime");
			if (this._speedTxt)
			{
				this._speedTxt.text = Singleton<WatchController>.GetInstance().SpeedRate.ToString() + "X";
			}
			this.ValidatePlayBtnState();
			this.ValidateSpeedBtnState();
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Watch_ClickPlay, new CUIEventManager.OnUIEventHandler(this.OnClickPlay));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Watch_ClickSpeedUp, new CUIEventManager.OnUIEventHandler(this.OnClickSpeedUp));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Watch_ClickSpeedDown, new CUIEventManager.OnUIEventHandler(this.OnClickSpeedDown));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Watch_JudgePause, new CUIEventManager.OnUIEventHandler(this.ConfirmJudgePause));
		}

		public void Clear()
		{
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Watch_ClickPlay, new CUIEventManager.OnUIEventHandler(this.OnClickPlay));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Watch_ClickSpeedUp, new CUIEventManager.OnUIEventHandler(this.OnClickSpeedUp));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Watch_ClickSpeedDown, new CUIEventManager.OnUIEventHandler(this.OnClickSpeedDown));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Watch_JudgePause, new CUIEventManager.OnUIEventHandler(this.ConfirmJudgePause));
		}

		public void LateUpdate()
		{
			uint num = Singleton<WatchController>.GetInstance().CurFrameNo * Singleton<WatchController>.GetInstance().FrameDelta;
			uint num2 = Singleton<WatchController>.GetInstance().EndFrameNo * Singleton<WatchController>.GetInstance().FrameDelta;
			if (this._progress)
			{
				this._progress.value = ((num2 <= 0u) ? 0f : (num / num2));
			}
			if (this._currentTimeTxt)
			{
				this._currentTimeTxt.text = string.Format("{0:D2}:{1:D2}", num / 60000u, num / 1000u % 60u);
			}
			if (this._totalTimeTxt)
			{
				this._totalTimeTxt.text = string.Format("{0:D2}:{1:D2}", num2 / 60000u, num2 / 1000u % 60u);
			}
			if (this._speedTxt)
			{
				this._speedTxt.text = Singleton<WatchController>.GetInstance().SpeedRate.ToString() + "X";
			}
			if (Singleton<WatchController>.GetInstance().IsLiveCast)
			{
				this.ValidatePlayBtnState();
			}
		}

		private void OnClickPlay(CUIEvent evt)
		{
			PauseControl pauseControl = Singleton<CBattleSystem>.GetInstance().pauseControl;
			if (pauseControl != null && pauseControl.EnablePause)
			{
				if (Singleton<WatchController>.GetInstance().IsRunning)
				{
					Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("confirmJudgePause"), enUIEventID.Watch_JudgePause, enUIEventID.None, false);
				}
				else
				{
					pauseControl.RequestPause(false);
				}
			}
			else
			{
				Singleton<WatchController>.GetInstance().IsRunning = !Singleton<WatchController>.GetInstance().IsRunning;
			}
			this.ValidatePlayBtnState();
		}

		private void ConfirmJudgePause(CUIEvent evt)
		{
			PauseControl pauseControl = Singleton<CBattleSystem>.GetInstance().pauseControl;
			if (pauseControl != null && pauseControl.EnablePause && Singleton<WatchController>.GetInstance().IsRunning)
			{
				pauseControl.RequestPause(true);
			}
		}

		private void OnClickSpeedUp(CUIEvent evt)
		{
			WatchController expr_05 = Singleton<WatchController>.GetInstance();
			expr_05.SpeedRate += 1;
			this.ValidateSpeedBtnState();
		}

		private void OnClickSpeedDown(CUIEvent evt)
		{
			WatchController expr_05 = Singleton<WatchController>.GetInstance();
			expr_05.SpeedRate -= 1;
			this.ValidateSpeedBtnState();
		}

		private void ValidatePlayBtnState()
		{
			this._playBtn.CustomSetActive(!Singleton<WatchController>.GetInstance().IsRunning);
			this._pauseBtn.CustomSetActive(Singleton<WatchController>.GetInstance().IsRunning);
		}

		private void ValidateSpeedBtnState()
		{
			if (this._speedUpBtn)
			{
				bool flag = !Singleton<WatchController>.GetInstance().IsLiveCast && Singleton<WatchController>.GetInstance().SpeedRate < Singleton<WatchController>.GetInstance().SpeedRateMax;
				this._speedUpBtn.GetComponent<Button>().interactable = flag;
				this._speedUpBtn.GetComponent<CUIEventScript>().enabled = flag;
			}
			if (this._speedDownBtn)
			{
				bool flag2 = !Singleton<WatchController>.GetInstance().IsLiveCast && Singleton<WatchController>.GetInstance().SpeedRate > Singleton<WatchController>.GetInstance().SpeedRateMin;
				this._speedDownBtn.GetComponent<Button>().interactable = flag2;
				this._speedDownBtn.GetComponent<CUIEventScript>().enabled = flag2;
			}
		}
	}
}
