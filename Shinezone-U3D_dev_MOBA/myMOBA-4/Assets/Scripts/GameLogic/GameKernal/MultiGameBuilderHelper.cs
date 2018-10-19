using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameSystem;
using CSProtocol;
using System;

namespace Assets.Scripts.GameLogic.GameKernal
{
	internal class MultiGameBuilderHelper : BaseBuilderHelper
	{
		public override void BuildGameContext(ProtocolObject svrInfo)
		{
			SCPKG_MULTGAME_BEGINLOAD svrGameInfo = (SCPKG_MULTGAME_BEGINLOAD)svrInfo;
			Singleton<GameContextEx>.GetInstance().InitMultiGame(svrGameInfo);
		}

		public override void BuildGamePlayer(ProtocolObject svrInfo)
		{
			SCPKG_MULTGAME_BEGINLOAD svrGameInfo = (SCPKG_MULTGAME_BEGINLOAD)svrInfo;
			Singleton<GameContextEx>.GetInstance().InitMultiGame(svrGameInfo);
			this.PlayerBuilder.BuildMultiGamePlayers(svrGameInfo);
		}

		public override void PreLoad()
		{
			base.PreLoad();
			Singleton<FrameSynchr>.GetInstance().SetSynchrConfig(
                Singleton<GameContextEx>.GetInstance().GameContextMobaInfo.KFrapsFreqMs, 
                (uint)Singleton<GameContextEx>.GetInstance().GameContextMobaInfo.KFrapsLater, 
                (uint)Singleton<GameContextEx>.GetInstance().GameContextMobaInfo.PreActFrap, 
                Singleton<GameContextEx>.GetInstance().GameContextMobaInfo.RandomSeed);
		}

		public override void OnGameLoadProgress(float progress)
		{
			if (!Singleton<WatchController>.GetInstance().IsWatching)
			{
				CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1083u);
				cSPkg.stPkgData.stMultGameLoadProcessReq.wProcess = Convert.ToUInt16(progress * 100f);
				Singleton<NetworkModule>.GetInstance().SendGameMsg(ref cSPkg, 0u);
			}
			CUILoadingSystem.OnSelfLoadProcess(progress);
		}
	}
}
