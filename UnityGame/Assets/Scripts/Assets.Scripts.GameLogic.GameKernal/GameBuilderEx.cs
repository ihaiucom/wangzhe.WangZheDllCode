using Assets.Scripts.Common;
using CSProtocol;
using System;

namespace Assets.Scripts.GameLogic.GameKernal
{
	public class GameBuilderEx : Singleton<GameBuilderEx>
	{
		private readonly BaseBuilderHelper _soloGameBuilder = new SoloGameBuilderHelper();

		private readonly BaseBuilderHelper _multiGameBuilder = new MultiGameBuilderHelper();

		private BaseBuilderHelper _curGameBuilder;

		public float LastLoadingTime
		{
			get
			{
				return this._curGameBuilder.LastLoadingTime;
			}
		}

		public void BuildGame(ProtocolObject svrInfo)
		{
			this._curGameBuilder = null;
			if (svrInfo is SCPKG_STARTSINGLEGAMERSP)
			{
				this._curGameBuilder = this._soloGameBuilder;
			}
			else if (svrInfo is SCPKG_MULTGAME_BEGINLOAD)
			{
				this._curGameBuilder = this._multiGameBuilder;
			}
			if (this._curGameBuilder != null)
			{
				this._curGameBuilder.BuildGameContext(svrInfo);
				this._curGameBuilder.BuildGamePlayer(svrInfo);
				this._curGameBuilder.PreLoad();
				this._curGameBuilder.Load();
			}
		}

		public void EndGame()
		{
			if (this._curGameBuilder != null)
			{
				this._curGameBuilder.EndGame();
			}
		}
	}
}
