using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	[MessageHandlerClass]
	public class PauseControl
	{
		public const byte NoLimitTimes = 255;

		private GameObject _root;

		private GameObject _pauseNode;

		private GameObject _resumeNode;

		private Text _passedTimeTxt;

		private Text _pauseCampTxt;

		private GameObject _remainNode;

		private Text _remainTimesTxt;

		private Button _resumeButton;

		private Text _resumeTimerTxt;

		private int _pauseTimer;

		private int _passedSecond;

		private int _waitResumeSecond;

		public byte MaxAllowTimes
		{
			get;
			private set;
		}

		public byte CurPauseTimes
		{
			get;
			private set;
		}

		public bool EnablePause
		{
			get
			{
				return this.MaxAllowTimes > 0;
			}
		}

		public bool CanPause
		{
			get
			{
				return this.CurPauseTimes < this.MaxAllowTimes || this.MaxAllowTimes == 255;
			}
		}

		public PauseControl(CUIFormScript rootForm)
		{
			GameObject gameObject = Utility.FindChild(rootForm.gameObject, "PauseResume");
			gameObject.CustomSetActive(false);
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			this.MaxAllowTimes = (curLvelContext.IsGameTypePvpRoom() ? (Singleton<WatchController>.GetInstance().IsLiveCast ? 255 : curLvelContext.m_pauseTimes) : 0);
			if (this.MaxAllowTimes == 0)
			{
				this._root = null;
				return;
			}
			this.CurPauseTimes = ((Singleton<LobbyLogic>.GetInstance().reconnGameInfo != null) ? Singleton<LobbyLogic>.GetInstance().reconnGameInfo.bPauseNum : 0);
			this._root = gameObject;
			if (this._root)
			{
				this._pauseNode = Utility.FindChild(this._root, "PauseNode");
				this._resumeNode = Utility.FindChild(this._root, "ResumeNode");
				this._passedTimeTxt = Utility.GetComponetInChild<Text>(this._pauseNode, "PassedTime");
				this._pauseCampTxt = Utility.GetComponetInChild<Text>(this._pauseNode, "PauseCamp");
				this._resumeButton = Utility.GetComponetInChild<Button>(this._pauseNode, "ResumeButton");
				this._resumeTimerTxt = Utility.GetComponetInChild<Text>(this._pauseNode, "ResumeButton/Text");
				this._remainNode = Utility.FindChild(this._pauseNode, "RemainTimes");
				this._remainTimesTxt = Utility.GetComponetInChild<Text>(this._remainNode, "Times");
				this._root.CustomSetActive(false);
			}
			this._pauseTimer = 0;
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_ResumeMultiGame, new CUIEventManager.OnUIEventHandler(this.OnResumeMultiGame));
		}

		public void Clear()
		{
			Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref this._pauseTimer);
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_ResumeMultiGame, new CUIEventManager.OnUIEventHandler(this.OnResumeMultiGame));
			this._root = null;
		}

		public void RequestPause(bool isPause)
		{
			if (isPause)
			{
				if (this.CanPause)
				{
					CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(5243u);
					cSPkg.stPkgData.stPauseReq.bType = 1;
					Singleton<NetworkModule>.GetInstance().SendGameMsg(ref cSPkg, 0u);
				}
			}
			else
			{
				CSPkg cSPkg2 = NetworkModule.CreateDefaultCSPKG(5243u);
				cSPkg2.stPkgData.stPauseReq.bType = 2;
				Singleton<NetworkModule>.GetInstance().SendGameMsg(ref cSPkg2, 0u);
			}
		}

		private void ResponsePause(SCPKG_PAUSE_RSP pkg)
		{
			if (pkg.bResult != 0)
			{
				return;
			}
			if (pkg.bType == 2)
			{
				Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref this._pauseTimer);
				if (this._root)
				{
					this._pauseNode.CustomSetActive(false);
					this._resumeNode.CustomSetActive(true);
				}
			}
			else
			{
				this.CurPauseTimes += 1;
			}
		}

		private void OnResumeMultiGame(CUIEvent uiEvt)
		{
			this.RequestPause(false);
		}

		[MessageHandler(5244)]
		public static void OnPauseResponse(CSPkg msg)
		{
			if (Singleton<CBattleSystem>.instance.pauseControl != null)
			{
				Singleton<CBattleSystem>.instance.pauseControl.ResponsePause(msg.stPkgData.stPauseRsp);
			}
		}

		public void ExecPauseCommand(byte pauseCommand, byte pauseByCamp, byte[] campPauseTimes)
		{
			bool flag = pauseCommand == 1;
			if (Singleton<WatchController>.GetInstance().IsWatching)
			{
				Singleton<WatchController>.GetInstance().IsRunning = !flag;
			}
			else
			{
				Singleton<FrameSynchr>.GetInstance().SetSynchrRunning(!flag);
				Singleton<FrameSynchr>.GetInstance().ResetStartTime();
			}
			if (this._root)
			{
				this._root.CustomSetActive(flag);
				if (flag)
				{
					Singleton<CBattleSystem>.GetInstance().ClosePopupForms();
					this._pauseNode.CustomSetActive(true);
					this._resumeNode.CustomSetActive(false);
					COM_PLAYERCAMP cOM_PLAYERCAMP = Singleton<WatchController>.GetInstance().IsWatching ? COM_PLAYERCAMP.COM_PLAYERCAMP_MID : Singleton<GamePlayerCenter>.GetInstance().hostPlayerCamp;
					this._pauseCampTxt.set_text(Singleton<CTextManager>.GetInstance().GetText(string.Format("PauseTips_{0}Look{1}", (int)cOM_PLAYERCAMP, (int)pauseByCamp)));
					this._passedSecond = 0;
					if ((COM_PLAYERCAMP)pauseByCamp == cOM_PLAYERCAMP)
					{
						this._waitResumeSecond = 0;
					}
					else
					{
						ResGlobalInfo dataByKey = GameDataMgr.globalInfoDatabin.GetDataByKey(242u);
						this._waitResumeSecond = (int)((dataByKey != null) ? dataByKey.dwConfValue : 60u);
					}
					this.ValidateTimerState();
					if (cOM_PLAYERCAMP != COM_PLAYERCAMP.COM_PLAYERCAMP_MID)
					{
						this._remainNode.CustomSetActive(true);
						SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
						int num = curLvelContext.m_pvpPlayerNum / 2 * (int)this.MaxAllowTimes;
						this._remainTimesTxt.set_text((num - (int)campPauseTimes[(int)cOM_PLAYERCAMP]).ToString());
					}
					else
					{
						this._remainNode.CustomSetActive(false);
					}
					if (this._pauseTimer == 0)
					{
						this._pauseTimer = Singleton<CTimerManager>.GetInstance().AddTimer(1000, -1, new CTimer.OnTimeUpHandler(this.OnPauseTimer), false);
					}
				}
				else
				{
					Singleton<CTimerManager>.GetInstance().RemoveTimerSafely(ref this._pauseTimer);
				}
			}
		}

		private void OnPauseTimer(int timeSeq)
		{
			this._passedSecond++;
			if (this._waitResumeSecond > 0)
			{
				this._waitResumeSecond--;
			}
			this.ValidateTimerState();
		}

		private void ValidateTimerState()
		{
			this._passedTimeTxt.set_text(string.Format(Singleton<CTextManager>.GetInstance().GetText("PauseTimeFormate"), this._passedSecond / 60, this._passedSecond % 60));
			bool flag = this._waitResumeSecond >= 0;
			if (flag)
			{
				string text = Singleton<CTextManager>.GetInstance().GetText("ResumeGame");
				if (this._waitResumeSecond > 0)
				{
					this._resumeTimerTxt.set_text(string.Concat(new object[]
					{
						text,
						"(",
						this._waitResumeSecond,
						")"
					}));
				}
				else
				{
					this._resumeTimerTxt.set_text(text);
				}
				bool flag2 = this._waitResumeSecond == 0;
				CUICommonSystem.SetButtonEnable(this._resumeButton, flag2, flag2, true);
				this._resumeButton.gameObject.CustomSetActive(true);
			}
			else
			{
				this._resumeButton.gameObject.CustomSetActive(false);
			}
		}
	}
}
