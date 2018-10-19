using System;

namespace Assets.Scripts.GameSystem
{
	[MessageHandlerClass]
	public class CMentorSystem : Singleton<CMentorSystem>
	{
		public override void Init()
		{
			this.InitUIEventListener();
		}

		public override void UnInit()
		{
			this.UinitUIEventListener();
		}

		private void InitUIEventListener()
		{
		}

		private void UinitUIEventListener()
		{
		}
	}
}
