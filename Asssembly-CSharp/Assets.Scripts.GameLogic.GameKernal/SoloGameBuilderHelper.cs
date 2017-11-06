using Assets.Scripts.Common;
using Assets.Scripts.GameSystem;
using CSProtocol;
using System;

namespace Assets.Scripts.GameLogic.GameKernal
{
	internal class SoloGameBuilderHelper : BaseBuilderHelper
	{
		public override void BuildGameContext(ProtocolObject svrInfo)
		{
			SCPKG_STARTSINGLEGAMERSP svrGameInfo = (SCPKG_STARTSINGLEGAMERSP)svrInfo;
			Singleton<GameContextEx>.GetInstance().InitSingleGame(svrGameInfo);
		}

		public override void BuildGamePlayer(ProtocolObject svrInfo)
		{
			SCPKG_STARTSINGLEGAMERSP svrGameInfo = (SCPKG_STARTSINGLEGAMERSP)svrInfo;
			this.PlayerBuilder.BuildSoloGamePlayers(svrGameInfo);
		}

		public override void OnGameLoadProgress(float progress)
		{
			CUILoadingSystem.OnSelfLoadProcess(progress);
		}
	}
}
