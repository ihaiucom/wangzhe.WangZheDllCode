using Assets.Scripts.GameLogic;
using Assets.Scripts.GameSystem;
using Assets.Scripts.Sound;
using Assets.Scripts.UI;
using System;
using UnityEngine;

namespace Assets.Scripts.Framework
{
	[GameState]
	public class BattleState : BaseState
	{
		private BlendWeights m_originalBlendWeight;

		public override void OnStateEnter()
		{
			this.m_originalBlendWeight = QualitySettings.blendWeights;
			if (GameSettings.RenderQuality == SGameRenderQuality.Low)
			{
				QualitySettings.blendWeights = BlendWeights.OneBone;
			}
			else
			{
				QualitySettings.blendWeights = BlendWeights.TwoBones;
			}
			SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
			string eventName = (curLvelContext != null && !string.IsNullOrEmpty(curLvelContext.m_musicStartEvent)) ? curLvelContext.m_musicStartEvent : "PVP01_Play";
			Singleton<CSoundManager>.GetInstance().PostEvent(eventName, null);
			string text = (curLvelContext != null) ? curLvelContext.m_ambientSoundEvent : string.Empty;
			if (!string.IsNullOrEmpty(text))
			{
				Singleton<CSoundManager>.instance.PostEvent(text, null);
			}
			CUICommonSystem.OpenFps();
			Singleton<CUIParticleSystem>.GetInstance().Open();
			CResourceManager.isBattleState = true;
			switch (Singleton<CNewbieAchieveSys>.GetInstance().trackFlag)
			{
			case CNewbieAchieveSys.TrackFlag.SINGLE_COMBAT_3V3_ENTER:
				MonoSingleton<NewbieGuideManager>.GetInstance().SetNewbieBit(10, true, false);
				break;
			case CNewbieAchieveSys.TrackFlag.SINGLE_MATCH_3V3_ENTER:
				MonoSingleton<NewbieGuideManager>.GetInstance().SetNewbieBit(11, true, false);
				break;
			case CNewbieAchieveSys.TrackFlag.PVE_1_1_1_Enter:
				MonoSingleton<NewbieGuideManager>.GetInstance().SetNewbieBit(13, true, false);
				break;
			}
			if (curLvelContext.IsMobaModeWithOutGuide())
			{
				Singleton<CPlayerPvpHistoryController>.instance.StartBattle();
			}
			MonoSingleton<PandroaSys>.GetInstance().PausePandoraSys(true);
		}

		public override void OnStateLeave()
		{
			Singleton<CRecordUseSDK>.instance.OnBadGameEnd();
			MonoSingleton<PandroaSys>.GetInstance().PausePandoraSys(false);
			QualitySettings.blendWeights = this.m_originalBlendWeight;
			CResourceManager.isBattleState = false;
			SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
			string eventName = (curLvelContext != null && !string.IsNullOrEmpty(curLvelContext.m_musicEndEvent)) ? curLvelContext.m_musicEndEvent : "PVP01_Stop";
			Singleton<CSoundManager>.GetInstance().PostEvent(eventName, null);
			string[] exceptFormNames = new string[]
			{
				CSettleSystem.PATH_PVP_SETTLE_PVP,
				SettlementSystem.SettlementFormName,
				PVESettleSys.PATH_LOSE
			};
			Singleton<CUIManager>.GetInstance().CloseAllForm(exceptFormNames, true, true);
			MonoSingleton<ShareSys>.instance.m_bShowTimeline = false;
			VCollisionShape.ClearCache();
			Singleton<CGameObjectPool>.GetInstance().ClearPooledObjects();
			Singleton<CResourceManager>.GetInstance().RemoveCachedResources(new enResourceType[]
			{
				enResourceType.BattleScene,
				enResourceType.UI3DImage,
				enResourceType.UIForm,
				enResourceType.UIPrefab,
				enResourceType.UISprite
			});
			Singleton<CResourceManager>.GetInstance().UnloadUnusedAssets();
			Singleton<CSoundManager>.GetInstance().UnloadBanks(CSoundManager.BankType.Battle);
			Singleton<GameDataMgr>.GetInstance().UnloadReducedDatabin();
		}
	}
}
