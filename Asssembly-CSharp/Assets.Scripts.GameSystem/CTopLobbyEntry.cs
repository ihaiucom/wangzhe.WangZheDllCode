using Assets.Scripts.UI;
using System;

namespace Assets.Scripts.GameSystem
{
	public class CTopLobbyEntry : Singleton<CTopLobbyEntry>
	{
		public override void Init()
		{
			base.Init();
			Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.Lobby_OpenTopLobbyForm, new CUIEventManager.OnUIEventHandler(this.OnOpenTopLobbyForm));
			Singleton<CUIEventManager>.instance.AddUIEventListener(enUIEventID.Lobby_CloseTopLobbyForm, new CUIEventManager.OnUIEventHandler(this.OnCloseTopLobbyForm));
		}

		public override void UnInit()
		{
			base.UnInit();
			Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.Lobby_OpenTopLobbyForm, new CUIEventManager.OnUIEventHandler(this.OnOpenTopLobbyForm));
			Singleton<CUIEventManager>.instance.RemoveUIEventListener(enUIEventID.Lobby_CloseTopLobbyForm, new CUIEventManager.OnUIEventHandler(this.OnCloseTopLobbyForm));
		}

		private void OnOpenTopLobbyForm(CUIEvent cuiEvent)
		{
			this.OpenForm();
		}

		private void OnCloseTopLobbyForm(CUIEvent cuiEvent)
		{
			this.CloseForm();
		}

		public void OpenForm()
		{
			Singleton<CLobbySystem>.instance.SetTopBarPriority(enFormPriority.Priority4);
		}

		public void CloseForm()
		{
			Singleton<CLobbySystem>.instance.SetTopBarPriority(enFormPriority.Priority1);
		}
	}
}
