using Assets.Scripts.GameLogic;
using Assets.Scripts.GameSystem;
using Assets.Scripts.Sound;
using System;

namespace Assets.Scripts.Framework
{
	[GameState]
	public class LoadingState : BaseState
	{
		private string LastLevelBank;

		public override void OnStateEnter()
		{
			Singleton<CResourceManager>.GetInstance().UnloadUnusedAssets();
			Singleton<CUILoadingSystem>.instance.ShowLoading();
			Singleton<CSoundManager>.GetInstance().PostEvent("Login_Stop", null);
			Singleton<CSoundManager>.GetInstance().PostEvent("Play_Hall_Ending", null);
			SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
			string text = (curLvelContext != null) ? curLvelContext.m_musicBankResName : string.Empty;
			if (text != this.LastLevelBank && !string.IsNullOrEmpty(this.LastLevelBank))
			{
				Singleton<CSoundManager>.instance.UnLoadBank(this.LastLevelBank, CSoundManager.BankType.LevelMusic);
			}
			if (!string.IsNullOrEmpty(text))
			{
				this.LastLevelBank = text;
				Singleton<CSoundManager>.instance.LoadBank(text, CSoundManager.BankType.LevelMusic);
			}
			CUICommonSystem.OpenFps();
		}

		public override void OnStateLeave()
		{
			Singleton<CResourceManager>.GetInstance().UnloadUnusedAssets();
			Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharLoading");
		}
	}
}
