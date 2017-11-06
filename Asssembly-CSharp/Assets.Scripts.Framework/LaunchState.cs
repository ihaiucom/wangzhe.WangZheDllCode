using Assets.Scripts.GameSystem;
using System;

namespace Assets.Scripts.Framework
{
	[GameState]
	public class LaunchState : BaseState
	{
		private bool m_isSplashPlayComplete;

		private bool m_isBaseSystemPrepareComplete;

		private bool m_jumpState;

		public override void OnStateEnter()
		{
			Singleton<ResourceLoader>.GetInstance().LoadScene("SplashScene", new ResourceLoader.LoadCompletedDelegate(this.OnSplashLoadCompleted));
			this.m_isSplashPlayComplete = false;
			this.m_isBaseSystemPrepareComplete = false;
		}

		private void OnSplashLoadCompleted()
		{
			Singleton<CTimerManager>.GetInstance().AddTimer(1000, 1, new CTimer.OnTimeUpHandler(this.OnTimiPlayComplete));
			Singleton<CTimerManager>.GetInstance().AddTimer(3000, 1, new CTimer.OnTimeUpHandler(this.OnSplashPlayComplete));
			Singleton<CCheatSystem>.GetInstance().OpenCheatTriggerForm(new CCheatSystem.OnDisable(this.OnCheatSystemDisable));
		}

		private void OnTimiPlayComplete(int timerSequence)
		{
			MonoSingleton<GameFramework>.GetInstance().StartPrepareBaseSystem(new GameFramework.DelegateOnBaseSystemPrepareComplete(this.OnPrepareBaseSystemComplete));
		}

		private void OnSplashPlayComplete(int timerSequence)
		{
			this.m_isSplashPlayComplete = true;
			this.CheckContionToNextState();
		}

		private void OnPrepareBaseSystemComplete()
		{
			this.m_isBaseSystemPrepareComplete = true;
			this.CheckContionToNextState();
		}

		private void OnCheatSystemDisable()
		{
			this.CheckContionToNextState();
		}

		private void CheckContionToNextState()
		{
			if (this.m_isSplashPlayComplete && this.m_isBaseSystemPrepareComplete && !Singleton<CCheatSystem>.GetInstance().m_enabled && !this.m_jumpState)
			{
				this.m_jumpState = true;
				Singleton<GameStateCtrl>.GetInstance().GotoState("VersionUpdateState");
			}
		}
	}
}
