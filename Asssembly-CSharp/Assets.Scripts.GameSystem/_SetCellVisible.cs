using CSProtocol;
using System;

namespace Assets.Scripts.GameSystem
{
	public struct _SetCellVisible : ISetCellVisible
	{
		private GameFowManager pFowMgr;

		private byte maxAlpha;

		public void Init()
		{
			this.pFowMgr = Singleton<GameFowManager>.instance;
			this.maxAlpha = 255;
		}

		public void SetVisible(VInt2 inPos, COM_PLAYERCAMP camp, bool bVisible)
		{
			if (this.pFowMgr == null)
			{
				return;
			}
			GameFowManager.SetVisible(true, inPos.x, inPos.y, camp);
		}

		public void SetBaseDataVisible(VInt2 inPos, COM_PLAYERCAMP camp, bool bVisible)
		{
			if (this.pFowMgr == null)
			{
				return;
			}
			GameFowManager.SetBaseDataVisible(true, inPos.x, inPos.y, camp);
		}
	}
}
