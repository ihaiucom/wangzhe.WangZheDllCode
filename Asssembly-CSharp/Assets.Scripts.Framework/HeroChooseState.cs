using Assets.Scripts.GameLogic;
using System;

namespace Assets.Scripts.Framework
{
	[GameState]
	public class HeroChooseState : BaseState
	{
		public override void OnStateEnter()
		{
			Singleton<HeroChooseLogic>.GetInstance().OpenInitChooseHeroForm();
			Singleton<ResourceLoader>.GetInstance().LoadScene("ChooseHero", new ResourceLoader.LoadCompletedDelegate(this.OnHeroChooseSceneCompleted));
		}

		private void OnHeroChooseSceneCompleted()
		{
		}

		public override void OnStateLeave()
		{
			Singleton<HeroChooseLogic>.GetInstance().CloseInitChooseHeroForm();
			Singleton<CResourceManager>.GetInstance().UnloadUnusedAssets();
		}
	}
}
