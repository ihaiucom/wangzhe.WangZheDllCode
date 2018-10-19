using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic.GameKernal;
using CSProtocol;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public class CPlayerKDAStat
	{
		private const uint RECORD_PERIOD = 30000u;

		private const uint RECORD_START = 60000u;

		private DictionaryView<uint, PlayerKDA> m_PlayerKDA = new DictionaryView<uint, PlayerKDA>();

		public CCampKDAStat m_CampKdaStat;

		private uint m_LastRecordTick;

		private ListValueView<CampJudgeRecord> m_CampJudgeRecords = new ListValueView<CampJudgeRecord>(60);

		private CampJudgeRecord _campR1Cache = default(CampJudgeRecord);

		private CampJudgeRecord _campR2Cache = default(CampJudgeRecord);

		public DictionaryView<uint, PlayerKDA>.Enumerator GetEnumerator()
		{
			return this.m_PlayerKDA.GetEnumerator();
		}

		public PlayerKDA GetHostKDA()
		{
			PlayerKDA result;
			this.m_PlayerKDA.TryGetValue(Singleton<GamePlayerCenter>.instance.HostPlayerId, out result);
			return result;
		}

		public PlayerKDA GetPlayerKDA(uint playerId)
		{
			PlayerKDA result;
			this.m_PlayerKDA.TryGetValue(playerId, out result);
			return result;
		}

		public HeroKDA GetHeroKDA(uint objID)
		{
			DictionaryView<uint, PlayerKDA>.Enumerator enumerator = this.m_PlayerKDA.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
				ListView<HeroKDA>.Enumerator enumerator2 = current.Value.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					if (enumerator2.Current.actorHero.handle.ObjID == objID)
					{
						return enumerator2.Current;
					}
				}
			}
			return null;
		}

		public float GetTeamKDA(COM_PLAYERCAMP camp)
		{
			float num = 0f;
			DictionaryView<uint, PlayerKDA>.Enumerator enumerator = this.m_PlayerKDA.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
				if (current.Value.PlayerCamp == camp)
				{
					float arg_46_0 = num;
					KeyValuePair<uint, PlayerKDA> current2 = enumerator.Current;
					num = arg_46_0 + current2.Value.KDAValue;
				}
			}
			return num;
		}

		public int GetTeamKillNum(COM_PLAYERCAMP camp)
		{
			int num = 0;
			DictionaryView<uint, PlayerKDA>.Enumerator enumerator = this.m_PlayerKDA.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
				if (current.Value.PlayerCamp == camp)
				{
					int arg_42_0 = num;
					KeyValuePair<uint, PlayerKDA> current2 = enumerator.Current;
					num = arg_42_0 + current2.Value.numKill;
				}
			}
			return num;
		}

		public int GetTeamDeadNum(COM_PLAYERCAMP camp)
		{
			int num = 0;
			DictionaryView<uint, PlayerKDA>.Enumerator enumerator = this.m_PlayerKDA.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
				if (current.Value.PlayerCamp == camp)
				{
					int arg_42_0 = num;
					KeyValuePair<uint, PlayerKDA> current2 = enumerator.Current;
					num = arg_42_0 + current2.Value.numDead;
				}
			}
			return num;
		}

		public int GetTeamAssistNum(COM_PLAYERCAMP camp)
		{
			int num = 0;
			DictionaryView<uint, PlayerKDA>.Enumerator enumerator = this.m_PlayerKDA.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
				if (current.Value.PlayerCamp == camp)
				{
					int arg_42_0 = num;
					KeyValuePair<uint, PlayerKDA> current2 = enumerator.Current;
					num = arg_42_0 + current2.Value.numAssist;
				}
			}
			return num;
		}

		public int GetTeamHurtToHero(COM_PLAYERCAMP camp)
		{
			int num = 0;
			DictionaryView<uint, PlayerKDA>.Enumerator enumerator = this.m_PlayerKDA.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
				if (current.Value.PlayerCamp == camp)
				{
					int arg_42_0 = num;
					KeyValuePair<uint, PlayerKDA> current2 = enumerator.Current;
					num = arg_42_0 + current2.Value.TotalHurtToHero;
				}
			}
			return num;
		}

		public int GetTeamBeHurtByHero(COM_PLAYERCAMP camp)
		{
			int num = 0;
			DictionaryView<uint, PlayerKDA>.Enumerator enumerator = this.m_PlayerKDA.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
				if (current.Value.PlayerCamp == camp)
				{
					int arg_42_0 = num;
					KeyValuePair<uint, PlayerKDA> current2 = enumerator.Current;
					num = arg_42_0 + current2.Value.TotalBeHurtByHero;
				}
			}
			return num;
		}

		public int GetTeamCoin(COM_PLAYERCAMP camp)
		{
			int num = 0;
			DictionaryView<uint, PlayerKDA>.Enumerator enumerator = this.m_PlayerKDA.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
				if (current.Value.PlayerCamp == camp)
				{
					int arg_42_0 = num;
					KeyValuePair<uint, PlayerKDA> current2 = enumerator.Current;
					num = arg_42_0 + current2.Value.TotalCoin;
				}
			}
			return num;
		}

		public void StartKDARecord()
		{
			this.reset();
			this.initialize();
		}

		public void GenerateStatData()
		{
			int num = (int)GameDataMgr.globalInfoDatabin.GetDataByKey(188u).dwConfValue;
			int num2 = 0;
			int num3 = 0;
			int num4 = (int)GameDataMgr.globalInfoDatabin.GetDataByKey(189u).dwConfValue;
			int num5 = 0;
			int num6 = (int)GameDataMgr.globalInfoDatabin.GetDataByKey(190u).dwConfValue;
			int num7 = 0;
			DictionaryView<uint, PlayerKDA>.Enumerator enumerator = this.m_PlayerKDA.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
				ListView<HeroKDA>.Enumerator enumerator2 = current.Value.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					if (num < enumerator2.Current.numKill)
					{
						num = enumerator2.Current.numKill;
					}
					if (num2 < enumerator2.Current.hurtToEnemy)
					{
						num2 = enumerator2.Current.hurtToEnemy;
					}
					if (num3 < enumerator2.Current.hurtTakenByEnemy)
					{
						num3 = enumerator2.Current.hurtTakenByEnemy;
					}
					if (num4 < enumerator2.Current.numAssist)
					{
						num4 = enumerator2.Current.numAssist;
					}
					if (num5 < enumerator2.Current.TotalCoin)
					{
						num5 = enumerator2.Current.TotalCoin;
					}
					if (num6 < enumerator2.Current.numKillOrgan)
					{
						num6 = enumerator2.Current.numKillOrgan;
					}
					if (num7 < enumerator2.Current.hurtToHero)
					{
						num7 = enumerator2.Current.hurtToHero;
					}
				}
			}
			enumerator = this.m_PlayerKDA.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, PlayerKDA> current2 = enumerator.Current;
				ListView<HeroKDA>.Enumerator enumerator3 = current2.Value.GetEnumerator();
				while (enumerator3.MoveNext())
				{
					if (num == enumerator3.Current.numKill)
					{
						enumerator3.Current.bKillMost = true;
					}
					if (num2 == enumerator3.Current.hurtToEnemy)
					{
						enumerator3.Current.bHurtMost = true;
					}
					if (num3 == enumerator3.Current.hurtTakenByEnemy)
					{
						enumerator3.Current.bHurtTakenMost = true;
					}
					if (num4 == enumerator3.Current.numAssist)
					{
						enumerator3.Current.bAsssistMost = true;
					}
					if (num5 == enumerator3.Current.TotalCoin)
					{
						enumerator3.Current.bGetCoinMost = true;
					}
					if (num6 == enumerator3.Current.numKillOrgan && enumerator3.Current.numKillOrgan > 0)
					{
						enumerator3.Current.bKillOrganMost = true;
					}
					if (num7 == enumerator3.Current.hurtToHero)
					{
						enumerator3.Current.bHurtToHeroMost = true;
					}
				}
			}
			if (this.m_CampKdaStat == null)
			{
				this.m_CampKdaStat = new CCampKDAStat();
				if (this.m_CampKdaStat != null)
				{
					this.m_CampKdaStat.Initialize(this.m_PlayerKDA);
				}
			}
		}

		public void reset()
		{
			DictionaryView<uint, PlayerKDA>.Enumerator enumerator = this.m_PlayerKDA.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
				current.Value.clear();
			}
			this.m_PlayerKDA.Clear();
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, sbyte, uint>("HeroLearnTalent", new Action<PoolObjHandle<ActorRoot>, sbyte, uint>(this.OnHeroLearnTalent));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, int>("HeroSoulLevelChange", new Action<PoolObjHandle<ActorRoot>, int>(this.OnHeroSoulLvlChange));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<uint, stEquipInfo[], bool, int>("HeroEquipInBattleChange", new Action<uint, stEquipInfo[], bool, int>(this.OnHeroBattleEquipChange));
			Singleton<GameEventSys>.instance.RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.OnActorDead));
			Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_GameEnd, new RefAction<DefaultGameEventParam>(this.OnGameEnd));
			Singleton<GameEventSys>.instance.RmvEventHandler<HurtEventResultInfo>(GameEventDef.Event_ActorDamage, new RefAction<HurtEventResultInfo>(this.OnActorDamage));
			Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_DoubleKill, new RefAction<DefaultGameEventParam>(this.OnActorDoubleKill));
			Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_TripleKill, new RefAction<DefaultGameEventParam>(this.OnActorTripleKill));
			Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_QuataryKill, new RefAction<DefaultGameEventParam>(this.OnActorQuataryKill));
			Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_PentaKill, new RefAction<DefaultGameEventParam>(this.OnActorPentaKill));
			Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_Odyssey, new RefAction<DefaultGameEventParam>(this.OnActorOdyssey));
			Singleton<GameEventSys>.instance.RmvEventHandler<SkillChooseTargetEventParam>(GameEventDef.Event_ActorBeChosenAsTarget, new RefAction<SkillChooseTargetEventParam>(this.OnActorBeChosen));
			Singleton<GameEventSys>.instance.RmvEventHandler<SkillChooseTargetEventParam>(GameEventDef.Event_HitTrigger, new RefAction<SkillChooseTargetEventParam>(this.OnHitTrigger));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, int, bool, PoolObjHandle<ActorRoot>>("HeroGoldCoinInBattleChange", new Action<PoolObjHandle<ActorRoot>, int, bool, PoolObjHandle<ActorRoot>>(this.OnActorBattleCoinChanged));
			Singleton<GameEventSys>.instance.RmvEventHandler<HemophagiaEventResultInfo>(GameEventDef.Event_Hemophagia, new RefAction<HemophagiaEventResultInfo>(this.OnActorHemophagia));
		}

		private void initialize()
		{
			List<Player> allPlayers = Singleton<GamePlayerCenter>.instance.GetAllPlayers();
			List<Player>.Enumerator enumerator = allPlayers.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Player current = enumerator.Current;
				if (current != null)
				{
					PlayerKDA playerKDA = new PlayerKDA();
					playerKDA.initialize(current);
					this.m_PlayerKDA.Add(current.PlayerId, playerKDA);
				}
			}
			this.m_LastRecordTick = 0u;
			Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, sbyte, uint>("HeroLearnTalent", new Action<PoolObjHandle<ActorRoot>, sbyte, uint>(this.OnHeroLearnTalent));
			Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, int>("HeroSoulLevelChange", new Action<PoolObjHandle<ActorRoot>, int>(this.OnHeroSoulLvlChange));
			Singleton<EventRouter>.GetInstance().AddEventHandler<uint, stEquipInfo[], bool, int>("HeroEquipInBattleChange", new Action<uint, stEquipInfo[], bool, int>(this.OnHeroBattleEquipChange));
			Singleton<GameEventSys>.instance.AddEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.OnActorDead));
			Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_GameEnd, new RefAction<DefaultGameEventParam>(this.OnGameEnd));
			Singleton<GameEventSys>.instance.AddEventHandler<HurtEventResultInfo>(GameEventDef.Event_ActorDamage, new RefAction<HurtEventResultInfo>(this.OnActorDamage));
			Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_DoubleKill, new RefAction<DefaultGameEventParam>(this.OnActorDoubleKill));
			Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_TripleKill, new RefAction<DefaultGameEventParam>(this.OnActorTripleKill));
			Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_QuataryKill, new RefAction<DefaultGameEventParam>(this.OnActorQuataryKill));
			Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_PentaKill, new RefAction<DefaultGameEventParam>(this.OnActorPentaKill));
			Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_Odyssey, new RefAction<DefaultGameEventParam>(this.OnActorOdyssey));
			Singleton<GameEventSys>.instance.AddEventHandler<SkillChooseTargetEventParam>(GameEventDef.Event_ActorBeChosenAsTarget, new RefAction<SkillChooseTargetEventParam>(this.OnActorBeChosen));
			Singleton<GameEventSys>.instance.AddEventHandler<SkillChooseTargetEventParam>(GameEventDef.Event_HitTrigger, new RefAction<SkillChooseTargetEventParam>(this.OnHitTrigger));
			Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, int, bool, PoolObjHandle<ActorRoot>>("HeroGoldCoinInBattleChange", new Action<PoolObjHandle<ActorRoot>, int, bool, PoolObjHandle<ActorRoot>>(this.OnActorBattleCoinChanged));
			Singleton<GameEventSys>.instance.AddEventHandler<HemophagiaEventResultInfo>(GameEventDef.Event_Hemophagia, new RefAction<HemophagiaEventResultInfo>(this.OnActorHemophagia));
		}

		public void OnHeroLearnTalent(PoolObjHandle<ActorRoot> hero, sbyte talentLevel, uint talentID)
		{
			DictionaryView<uint, PlayerKDA>.Enumerator enumerator = this.m_PlayerKDA.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
				ListView<HeroKDA>.Enumerator enumerator2 = current.Value.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					if (hero == enumerator2.Current.actorHero)
					{
						enumerator2.Current.TalentArr[(int)talentLevel].dwTalentID = talentID;
						enumerator2.Current.TalentArr[(int)talentLevel].dwLearnLevel = (uint)hero.handle.ValueComponent.actorSoulLevel;
						return;
					}
				}
			}
		}

		public void OnHeroBattleEquipChange(uint actorId, stEquipInfo[] equips, bool bIsAdd, int iEquipSlotIndex)
		{
			DictionaryView<uint, PlayerKDA>.Enumerator enumerator = this.m_PlayerKDA.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
				ListView<HeroKDA>.Enumerator enumerator2 = current.Value.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					if (enumerator2.Current != null && enumerator2.Current.actorHero.handle.ObjID == actorId)
					{
						equips.CopyTo(enumerator2.Current.Equips, 0);
						Singleton<EventRouter>.instance.BroadCastEvent(EventID.BATTLE_KDA_CHANGED);
						Singleton<EventRouter>.instance.BroadCastEvent(EventID.BATTLE_HERO_PROPERTY_CHANGED);
						return;
					}
				}
			}
		}

		public void OnHeroSoulLvlChange(PoolObjHandle<ActorRoot> hero, int level)
		{
			DictionaryView<uint, PlayerKDA>.Enumerator enumerator = this.m_PlayerKDA.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
				ListView<HeroKDA>.Enumerator enumerator2 = current.Value.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					if (hero == enumerator2.Current.actorHero)
					{
						enumerator2.Current.SoulLevel = Math.Max(level, 1);
						Singleton<EventRouter>.instance.BroadCastEvent(EventID.BATTLE_KDA_CHANGED);
						Singleton<EventRouter>.instance.BroadCastEvent(EventID.BATTLE_HERO_PROPERTY_CHANGED);
						return;
					}
				}
			}
		}

		public void DumpDebugInfo()
		{
			DictionaryView<uint, PlayerKDA>.Enumerator enumerator = this.m_PlayerKDA.GetEnumerator();
			while (enumerator.MoveNext())
			{
				string arg_2A_0 = "PlayerKDA Id {0}";
				KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
				Debug.Log(string.Format(arg_2A_0, current.Key));
			}
		}

		public void OnActorBattleCoinChanged(PoolObjHandle<ActorRoot> actor, int changeValue, bool isIncome, PoolObjHandle<ActorRoot> target)
		{
			DictionaryView<uint, PlayerKDA>.Enumerator enumerator = this.m_PlayerKDA.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
				ListView<HeroKDA>.Enumerator enumerator2 = current.Value.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					if (actor == enumerator2.Current.actorHero)
					{
						enumerator2.Current.OnActorBattleCoinChanged(actor, changeValue, isIncome, target);
						Singleton<EventRouter>.instance.BroadCastEvent(EventID.BATTLE_KDA_CHANGED);
						return;
					}
				}
			}
		}

		public void OnActorDead(ref GameDeadEventParam prm)
		{
			if (prm.bImmediateRevive)
			{
				return;
			}
			DictionaryView<uint, PlayerKDA>.Enumerator enumerator = this.m_PlayerKDA.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
				ListView<HeroKDA>.Enumerator enumerator2 = current.Value.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					enumerator2.Current.OnActorDead(ref prm);
				}
				KeyValuePair<uint, PlayerKDA> current2 = enumerator.Current;
				if (current2.Value.IsHost)
				{
					KeyValuePair<uint, PlayerKDA> current3 = enumerator.Current;
					if (current3.Value.m_hostHeroDamage != null)
					{
						KeyValuePair<uint, PlayerKDA> current4 = enumerator.Current;
						ListView<CHostHeroDamage>.Enumerator enumerator3 = current4.Value.m_hostHeroDamage.GetEnumerator();
						while (enumerator3.MoveNext())
						{
							if (enumerator3.Current != null)
							{
								enumerator3.Current.OnActorDead(ref prm);
							}
						}
					}
				}
			}
			if (prm.src && prm.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
			{
				Singleton<EventRouter>.instance.BroadCastEvent(EventID.BATTLE_KDA_CHANGED);
				Singleton<EventRouter>.instance.BroadCastEvent(EventID.BATTLE_KDA_CHANGED_BY_ACTOR_DEAD);
			}
		}

		public void OnActorDamage(ref HurtEventResultInfo prm)
		{
			DictionaryView<uint, PlayerKDA>.Enumerator enumerator = this.m_PlayerKDA.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
				ListView<HeroKDA>.Enumerator enumerator2 = current.Value.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					enumerator2.Current.OnActorDamage(ref prm);
				}
				KeyValuePair<uint, PlayerKDA> current2 = enumerator.Current;
				if (current2.Value.IsHost)
				{
					KeyValuePair<uint, PlayerKDA> current3 = enumerator.Current;
					if (current3.Value.m_hostHeroDamage != null)
					{
						KeyValuePair<uint, PlayerKDA> current4 = enumerator.Current;
						ListView<CHostHeroDamage>.Enumerator enumerator3 = current4.Value.m_hostHeroDamage.GetEnumerator();
						while (enumerator3.MoveNext())
						{
							if (enumerator3.Current != null)
							{
								enumerator3.Current.OnActorDamage(ref prm);
							}
						}
					}
				}
			}
			Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.BATTLE_TEAMFIGHT_DAMAGE_UPDATE);
		}

		public void OnActorDoubleKill(ref DefaultGameEventParam prm)
		{
			DictionaryView<uint, PlayerKDA>.Enumerator enumerator = this.m_PlayerKDA.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
				ListView<HeroKDA>.Enumerator enumerator2 = current.Value.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					if (prm.logicAtker && prm.logicAtker == enumerator2.Current.actorHero)
					{
						enumerator2.Current.OnActorDoubleKill(ref prm);
						return;
					}
				}
			}
		}

		public void OnActorTripleKill(ref DefaultGameEventParam prm)
		{
			DictionaryView<uint, PlayerKDA>.Enumerator enumerator = this.m_PlayerKDA.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
				ListView<HeroKDA>.Enumerator enumerator2 = current.Value.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					if (prm.logicAtker && prm.logicAtker == enumerator2.Current.actorHero)
					{
						enumerator2.Current.OnActorTripleKill(ref prm);
						return;
					}
				}
			}
		}

		public void OnActorQuataryKill(ref DefaultGameEventParam prm)
		{
			DictionaryView<uint, PlayerKDA>.Enumerator enumerator = this.m_PlayerKDA.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
				ListView<HeroKDA>.Enumerator enumerator2 = current.Value.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					if (prm.logicAtker && prm.logicAtker == enumerator2.Current.actorHero)
					{
						enumerator2.Current.OnActorQuataryKill(ref prm);
						return;
					}
				}
			}
		}

		public void OnActorPentaKill(ref DefaultGameEventParam prm)
		{
			DictionaryView<uint, PlayerKDA>.Enumerator enumerator = this.m_PlayerKDA.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
				ListView<HeroKDA>.Enumerator enumerator2 = current.Value.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					if (prm.logicAtker && prm.logicAtker == enumerator2.Current.actorHero)
					{
						enumerator2.Current.OnActorPentaKill(ref prm);
						return;
					}
				}
			}
		}

		public void OnActorOdyssey(ref DefaultGameEventParam prm)
		{
			DictionaryView<uint, PlayerKDA>.Enumerator enumerator = this.m_PlayerKDA.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
				ListView<HeroKDA>.Enumerator enumerator2 = current.Value.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					if (prm.logicAtker && prm.logicAtker == enumerator2.Current.actorHero)
					{
						enumerator2.Current.OnActorOdyssey(ref prm);
						return;
					}
				}
			}
		}

		public void OnActorBeChosen(ref SkillChooseTargetEventParam prm)
		{
			DictionaryView<uint, PlayerKDA>.Enumerator enumerator = this.m_PlayerKDA.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
				ListView<HeroKDA>.Enumerator enumerator2 = current.Value.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					enumerator2.Current.OnActorBeChosen(ref prm);
				}
			}
		}

		public void OnHitTrigger(ref SkillChooseTargetEventParam prm)
		{
			DictionaryView<uint, PlayerKDA>.Enumerator enumerator = this.m_PlayerKDA.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
				ListView<HeroKDA>.Enumerator enumerator2 = current.Value.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					enumerator2.Current.OnHitTrigger(ref prm);
				}
			}
		}

		private void OnGameEnd(ref DefaultGameEventParam prm)
		{
			Singleton<GameEventSys>.instance.RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.OnActorDead));
			Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_GameEnd, new RefAction<DefaultGameEventParam>(this.OnGameEnd));
			Singleton<GameEventSys>.instance.RmvEventHandler<HurtEventResultInfo>(GameEventDef.Event_ActorDamage, new RefAction<HurtEventResultInfo>(this.OnActorDamage));
			Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_DoubleKill, new RefAction<DefaultGameEventParam>(this.OnActorDoubleKill));
			Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_TripleKill, new RefAction<DefaultGameEventParam>(this.OnActorTripleKill));
			Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_QuataryKill, new RefAction<DefaultGameEventParam>(this.OnActorQuataryKill));
			Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_PentaKill, new RefAction<DefaultGameEventParam>(this.OnActorPentaKill));
			Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_Odyssey, new RefAction<DefaultGameEventParam>(this.OnActorOdyssey));
			Singleton<GameEventSys>.instance.RmvEventHandler<SkillChooseTargetEventParam>(GameEventDef.Event_ActorBeChosenAsTarget, new RefAction<SkillChooseTargetEventParam>(this.OnActorBeChosen));
			Singleton<GameEventSys>.instance.RmvEventHandler<SkillChooseTargetEventParam>(GameEventDef.Event_HitTrigger, new RefAction<SkillChooseTargetEventParam>(this.OnHitTrigger));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, int, bool, PoolObjHandle<ActorRoot>>("HeroGoldCoinInBattleChange", new Action<PoolObjHandle<ActorRoot>, int, bool, PoolObjHandle<ActorRoot>>(this.OnActorBattleCoinChanged));
			Singleton<GameEventSys>.instance.RmvEventHandler<HemophagiaEventResultInfo>(GameEventDef.Event_Hemophagia, new RefAction<HemophagiaEventResultInfo>(this.OnActorHemophagia));
		}

		private void OnActorHemophagia(ref HemophagiaEventResultInfo hri)
		{
			DictionaryView<uint, PlayerKDA>.Enumerator enumerator = this.m_PlayerKDA.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
				ListView<HeroKDA>.Enumerator enumerator2 = current.Value.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					enumerator2.Current.OnActorHemophagia(ref hri);
				}
			}
		}

		public void UpdateLogic()
		{
			uint num = (uint)Singleton<FrameSynchr>.instance.LogicFrameTick;
			if (num > this.m_LastRecordTick + 30000u && num > 60000u)
			{
				this.m_LastRecordTick = num;
				this._campR1Cache.Reset();
				this._campR2Cache.Reset();
				DictionaryView<uint, PlayerKDA>.Enumerator enumerator = this.m_PlayerKDA.GetEnumerator();
				while (enumerator.MoveNext())
				{
					KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
					ListView<HeroKDA>.Enumerator enumerator2 = current.Value.GetEnumerator();
					while (enumerator2.MoveNext())
					{
						HeroKDA current2 = enumerator2.Current;
						current2.RecordJudgeStatc();
						if (current2.actorHero)
						{
							if (current2.actorHero.handle.TheActorMeta.ActorCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
							{
								CPlayerKDAStat.CampRecordAux(ref this._campR1Cache, current2);
							}
							else if (current2.actorHero.handle.TheActorMeta.ActorCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_2)
							{
								CPlayerKDAStat.CampRecordAux(ref this._campR2Cache, current2);
							}
						}
						else
						{
							DebugHelper.Assert(false, "heroKda.actorHero is released! _handleSeq = {0} , _handleObj = {1} , _handleObj.usingSeq = {2} , isFighting = {3}", new object[]
							{
								current2.actorHero._handleSeq,
								current2.actorHero._handleObj,
								(current2.actorHero._handleObj == null) ? 0u : current2.actorHero._handleObj.usingSeq,
								Singleton<BattleLogic>.instance.isFighting.ToString()
							});
						}
					}
				}
				this.m_CampJudgeRecords.Add(this._campR1Cache);
				this.m_CampJudgeRecords.Add(this._campR2Cache);
			}
		}

		public bool GetCampJudgeRecord(COM_PLAYERCAMP camp, int timeIndex, ref CampJudgeRecord cjr)
		{
			int num = timeIndex * 2 + (camp - COM_PLAYERCAMP.COM_PLAYERCAMP_1);
			if (num < this.m_CampJudgeRecords.Count)
			{
				cjr = this.m_CampJudgeRecords[num];
				return true;
			}
			return false;
		}

		private static void CampRecordAux(ref CampJudgeRecord cjr, HeroKDA heroKda)
		{
			cjr.killNum += (ushort)heroKda.numKill;
			cjr.deadNum += (ushort)heroKda.numDead;
			cjr.gainCoin += (uint)(heroKda.TotalCoin - heroKda.CoinFromSystem);
			cjr.hurtToHero += (uint)heroKda.hurtToHero;
			cjr.sufferHero += (uint)heroKda.hurtTakenByHero;
		}
	}
}
