using System;

namespace Assets.Scripts.GameLogic
{
	public interface IActorComponent
	{
		void UpdateLogic(int delta);

		void Born(ActorRoot owner);
	}
}
