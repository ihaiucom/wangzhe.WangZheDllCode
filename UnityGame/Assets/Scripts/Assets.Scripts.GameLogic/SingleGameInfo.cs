using Assets.Scripts.GameSystem;
using System;

namespace Assets.Scripts.GameLogic
{
	public class SingleGameInfo : GameInfoBase
	{
		public override void PreBeginPlay()
		{
			this.LoadAllTeamActors();
		}

		public override void PostBeginPlay()
		{
			base.PostBeginPlay();
		}

		public override void StartFight()
		{
			base.StartFight();
		}

		public override void EndGame()
		{
			base.EndGame();
		}

		public override void OnLoadingProgress(float Progress)
		{
			CUILoadingSystem.OnSelfLoadProcess(Progress);
		}
	}
}
