using Assets.Scripts.GameSystem;
using Assets.Scripts.Sound;
using System;

namespace Assets.Scripts.Framework
{
	[GameState]
	public class LobbyState : BaseState
	{
		private bool s_firstEnterd;

		public override void OnStateEnter()
		{
			Singleton<NewbieWeakGuideControl>.GetInstance().OpenGuideForm();
			MonoSingleton<NewbieGuideManager>.GetInstance().CheckSkipIntoLobby();
			Singleton<CChatController>.GetInstance().bSendChat = true;
			Singleton<CChatController>.GetInstance().SubmitRefreshEvent();
			Singleton<ResourceLoader>.GetInstance().LoadScene("LobbyScene", new ResourceLoader.LoadCompletedDelegate(this.OnLobbySceneCompleted));
			Singleton<CSoundManager>.GetInstance().PostEvent("Login_Stop", null);
			Singleton<CSoundManager>.GetInstance().PostEvent("Main_Play", null);
			Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.LOBBY_STATE_ENTER);
			MonoSingleton<PandroaSys>.GetInstance().PausePandoraSys(false);
		}

		private void OnLobbySceneCompleted()
		{
			Singleton<GameLogic>.GetInstance().OpenLobby();
			CUICommonSystem.OpenFps();
			Singleton<CUIParticleSystem>.GetInstance().Open();
			Singleton<CChatController>.GetInstance().SetChatTimerEnable(true);
			Singleton<CChatController>.GetInstance().SetGuildRecruitTimerEnable(true);
		}

		public override void OnStateLeave()
		{
			Singleton<CChatController>.GetInstance().bSendChat = false;
			Singleton<CChatController>.GetInstance().SetChatTimerEnable(false);
			Singleton<CChatController>.GetInstance().SetGuildRecruitTimerEnable(false);
			Singleton<CChatController>.GetInstance().CancleRefreshEvent();
			Singleton<CChatController>.GetInstance().ClearAllPanel();
			Singleton<CSoundManager>.GetInstance().PostEvent("Main_Stop", null);
			Singleton<CSoundManager>.GetInstance().UnloadBanks(CSoundManager.BankType.Lobby);
			Singleton<ApolloHelper>.GetInstance().HideScrollNotice();
			Singleton<NewbieWeakGuideControl>.GetInstance().CloseGuideForm();
			Singleton<CResourceManager>.GetInstance().UnloadUnusedAssets();
			Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.LOBBY_STATE_LEAVE);
		}
	}
}
