using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.UI;
using ResData;
using System;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	internal class CNewbieAchieveSys : Singleton<CNewbieAchieveSys>
	{
		public enum TrackFlag
		{
			None,
			SINGLE_COMBAT_3V3_ENTER,
			SINGLE_MATCH_3V3_ENTER,
			PVE_1_1_1_Enter,
			SINGLE_MATCH_5V5_ENTER
		}

		public const string ACHIEVE_FORM_PATH = "UGUI/Form/System/Achieve/Form_AchieveShow.prefab";

		private UIParticleInfo m_parInfo;

		public CNewbieAchieveSys.TrackFlag trackFlag;

		public override void Init()
		{
			base.Init();
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Newbie_AchieveFormClose, new CUIEventManager.OnUIEventHandler(this.onAchieveClose));
			Singleton<GameEventSys>.GetInstance().AddEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.onActorDead));
			Singleton<GameEventSys>.GetInstance().AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorLearnTalent, new RefAction<DefaultGameEventParam>(this.onLearnTalent));
			Singleton<GameEventSys>.GetInstance().AddEventHandler<GameDeadEventParam>(GameEventDef.Event_MonsterGroupDead, new RefAction<GameDeadEventParam>(this.onMonsterGroupDead));
		}

		public override void UnInit()
		{
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Newbie_AchieveFormClose, new CUIEventManager.OnUIEventHandler(this.onAchieveClose));
			Singleton<GameEventSys>.GetInstance().RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.onActorDead));
			Singleton<GameEventSys>.GetInstance().RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_ActorLearnTalent, new RefAction<DefaultGameEventParam>(this.onLearnTalent));
			Singleton<GameEventSys>.GetInstance().RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_MonsterGroupDead, new RefAction<GameDeadEventParam>(this.onMonsterGroupDead));
			base.UnInit();
		}

		private void onActorDead(ref GameDeadEventParam param)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			DebugHelper.Assert(masterRoleInfo != null, "Master Roleinfo is NULL!");
			if (masterRoleInfo == null)
			{
				return;
			}
			if (param.atker && ActorHelper.IsHostActor(ref param.atker))
			{
				if (param.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster)
				{
					MonsterWrapper monsterWrapper = param.src.handle.AsMonster();
					DebugHelper.Assert(monsterWrapper != null && monsterWrapper.cfgInfo != null, "Can't find Monster config -- ID: {0}", new object[]
					{
						param.src.handle.TheActorMeta.ConfigId
					});
					if (monsterWrapper.cfgInfo.bMonsterType == 1 && !masterRoleInfo.IsNewbieAchieveSet(0))
					{
						this.ShowAchieve(enNewbieAchieve.COM_ACNT_CLIENT_BITS_TYPE_KILL_SOLDIER);
						masterRoleInfo.SetNewbieAchieve(0, true, false);
					}
				}
				else if (param.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ)
				{
					ResOrganCfgInfo dataCfgInfoByCurLevelDiff = OrganDataHelper.GetDataCfgInfoByCurLevelDiff(param.src.handle.TheActorMeta.ConfigId);
					DebugHelper.Assert(dataCfgInfoByCurLevelDiff != null, "Can't find Organ config -- ID: {0}", new object[]
					{
						param.src.handle.TheActorMeta.ConfigId
					});
					if (dataCfgInfoByCurLevelDiff.bOrganType == 1)
					{
						if (!masterRoleInfo.IsNewbieAchieveSet(1))
						{
							this.ShowAchieve(enNewbieAchieve.COM_ACNT_CLIENT_BITS_TYPE_DESTORY_ARROWTOWER);
							masterRoleInfo.SetNewbieAchieve(1, true, false);
						}
					}
					else if (dataCfgInfoByCurLevelDiff.bOrganType == 2 && !masterRoleInfo.IsNewbieAchieveSet(2))
					{
						this.ShowAchieve(enNewbieAchieve.COM_ACNT_CLIENT_BITS_TYPE_DESTORY_BASETOWER);
						masterRoleInfo.SetNewbieAchieve(2, true, false);
					}
				}
			}
			if (param.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero && param.atker)
			{
				if (ActorHelper.IsHostActor(ref param.atker))
				{
					if (!masterRoleInfo.IsNewbieAchieveSet(8))
					{
						this.ShowAchieve(enNewbieAchieve.COM_ACNT_CLIENT_BITS_TYPE_KILL_HERO);
						masterRoleInfo.SetNewbieAchieve(8, true, false);
					}
				}
				else if (param.atker.handle.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Hero)
				{
					HeroWrapper heroWrapper = param.src.handle.ActorControl as HeroWrapper;
					if (heroWrapper != null)
					{
						PoolObjHandle<ActorRoot> lastHeroAtker = heroWrapper.LastHeroAtker;
						if (lastHeroAtker && ActorHelper.IsHostActor(ref lastHeroAtker) && !masterRoleInfo.IsNewbieAchieveSet(8))
						{
							this.ShowAchieve(enNewbieAchieve.COM_ACNT_CLIENT_BITS_TYPE_KILL_HERO);
							masterRoleInfo.SetNewbieAchieve(8, true, false);
						}
					}
				}
			}
		}

		private void onLearnTalent(ref DefaultGameEventParam param)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			DebugHelper.Assert(masterRoleInfo != null, "Master Roleinfo is NULL!");
			if (masterRoleInfo == null)
			{
				return;
			}
			if (param.src && ActorHelper.IsHostActor(ref param.src) && !masterRoleInfo.IsNewbieAchieveSet(5))
			{
				this.ShowAchieve(enNewbieAchieve.COM_ACNT_CLIENT_BITS_TYPE_LEARN_TALENT);
				masterRoleInfo.SetNewbieAchieve(5, true, false);
			}
		}

		private void onMonsterGroupDead(ref GameDeadEventParam param)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			DebugHelper.Assert(masterRoleInfo != null, "Master Roleinfo is NULL!");
			if (masterRoleInfo == null)
			{
				return;
			}
			if (param.src)
			{
				if (param.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster)
				{
					MonsterWrapper monsterWrapper = param.src.handle.AsMonster();
					DebugHelper.Assert(monsterWrapper != null && monsterWrapper.cfgInfo != null, "Can't find Monster config -- ID: {0}", new object[]
					{
						param.src.handle.TheActorMeta.ConfigId
					});
					if (monsterWrapper.cfgInfo == null || monsterWrapper.cfgInfo.bMonsterType == 1)
					{
						return;
					}
				}
				if (param.atker && ActorHelper.IsHostActor(ref param.atker))
				{
					if (Singleton<BattleLogic>.GetInstance().DragonId != 0 && param.src.handle.TheActorMeta.ConfigId == Singleton<BattleLogic>.GetInstance().DragonId)
					{
						if (!masterRoleInfo.IsNewbieAchieveSet(4))
						{
							Singleton<CNewbieAchieveSys>.GetInstance().ShowAchieve(enNewbieAchieve.COM_ACNT_CLIENT_BITS_TYPE_KILL_LITTLEDRAGON);
							masterRoleInfo.SetNewbieAchieve(4, true, false);
						}
					}
					else if (!masterRoleInfo.IsNewbieAchieveSet(3))
					{
						this.ShowAchieve(enNewbieAchieve.COM_ACNT_CLIENT_BITS_TYPE_DESTORY_MONSTERHOME);
						masterRoleInfo.SetNewbieAchieve(3, true, false);
					}
				}
			}
		}

		private void onAchieveClose(CUIEvent uiEvent)
		{
			Singleton<CUIParticleSystem>.GetInstance().RemoveParticle(this.m_parInfo);
			Singleton<CUIManager>.GetInstance().CloseForm("UGUI/Form/System/Achieve/Form_AchieveShow.prefab");
		}

		public void ShowAchieve(enNewbieAchieve achieveType)
		{
			if (MonoSingleton<Reconnection>.instance.isProcessingRelayRecover || Singleton<WatchController>.GetInstance().IsWatching)
			{
				return;
			}
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/System/Achieve/Form_AchieveShow.prefab", false, true);
			Text component = cUIFormScript.gameObject.transform.Find("Bg/TxtTitle").gameObject.GetComponent<Text>();
			component.text = Singleton<CTextManager>.GetInstance().GetText(string.Format("{0}Title", achieveType.ToString()));
			Text component2 = cUIFormScript.gameObject.transform.Find("Bg/Txt").gameObject.GetComponent<Text>();
			component2.text = Singleton<CTextManager>.GetInstance().GetText(achieveType.ToString());
		}
	}
}
