using System;

namespace Assets.Scripts.GameLogic
{
	public abstract class GameInputMode : BaseState
	{
		protected GameInput inputSys;

		public GameInputMode(GameInput InSys)
		{
			this.inputSys = InSys;
		}

		public abstract void Update();

		public virtual void StopInput()
		{
			this.inputSys.SendStopMove(false);
		}
	}
}
