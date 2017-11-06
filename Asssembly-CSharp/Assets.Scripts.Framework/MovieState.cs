using System;

namespace Assets.Scripts.Framework
{
	[GameState]
	public class MovieState : BaseState
	{
		public override void OnStateEnter()
		{
			this.GoNextState();
		}

		private void GoNextState()
		{
			Singleton<GameStateCtrl>.GetInstance().GotoState("LoginState");
		}
	}
}
