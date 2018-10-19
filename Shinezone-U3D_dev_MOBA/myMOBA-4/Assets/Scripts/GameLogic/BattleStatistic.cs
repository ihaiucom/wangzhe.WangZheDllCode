using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic.GameKernal;
using CSProtocol;
using ResData;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public class BattleStatistic : Singleton<BattleStatistic>
	{
		private DictionaryView<uint, CampInfo> campStat = new DictionaryView<uint, CampInfo>();

		private DictionaryView<uint, Dictionary<int, DestroyStat>> destroyStats = new DictionaryView<uint, Dictionary<int, DestroyStat>>();

		public DictionaryView<uint, DictionaryView<uint, NONHERO_STATISTIC_INFO>> m_NonHeroInfo = new DictionaryView<uint, DictionaryView<uint, NONHERO_STATISTIC_INFO>>();

		private int m_iBattleResult;

		private AchievementRecorder m_optAchievementRecorder = new AchievementRecorder();

		public CBattleDeadStat m_battleDeadStat = new CBattleDeadStat();

		public CPlayerKDAStat m_playerKDAStat = new CPlayerKDAStat();

		public CPlayerSoulLevelStat m_playerSoulLevelStat = new CPlayerSoulLevelStat();

		public CShenFuStat m_shenFuStat = new CShenFuStat();

		public CPlayerBehaviorStat m_playerBehaviorStat = new CPlayerBehaviorStat();

		public CHeroSkillStat m_heroSkillStat = new CHeroSkillStat();

		public CBattleBuffStat m_battleBuffStat = new CBattleBuffStat();

		private uint m_winMvpId;

		private uint m_loseMvpId;

		private bool _useServerMvpScore;

		private Dictionary<uint, int> _mvpScore = new Dictionary<uint, int>();

		private uint m_lastBestPlayer;

		public COMDT_SETTLE_HERO_RESULT_DETAIL heroSettleInfo;

		public COMDT_RANK_SETTLE_INFO rankInfo;

		public COMDT_ACNT_INFO acntInfo;

		public COMDT_REWARD_MULTIPLE_DETAIL multiDetail;

		public GET_SOUL_EXP_STATISTIC_INFO m_stSoulStatisticInfo = new GET_SOUL_EXP_STATISTIC_INFO();

		public CPlayerLocationStat m_locStat = new CPlayerLocationStat();

		public COMDT_PVPSPECITEM_OUTPUT SpecialItemInfo;

		public COMDT_REWARD_DETAIL Rewards;

		public VDStat m_vdStat = new VDStat();

		public bool bSelfCampHaveWinningFlag;

		public ulong u64EmenyIsVisibleToHostHero;

		public int iBattleResult
		{
			get
			{
				return this.m_iBattleResult;
			}
			set
			{
				this.m_iBattleResult = value;
			}
		}

		public void StartStatistic()
		{
			this.campStat.Clear();
			this.destroyStats.Clear();
			CPlayerBehaviorStat.Clear();
			this.destroyStats.Add(0u, new Dictionary<int, DestroyStat>());
			this.destroyStats.Add(1u, new Dictionary<int, DestroyStat>());
			this.destroyStats.Add(2u, new Dictionary<int, DestroyStat>());
			this.m_playerKDAStat.StartKDARecord();
			this.m_battleDeadStat.StartRecord();
			this.m_playerSoulLevelStat.StartRecord();
			this.m_shenFuStat.StartRecord();
			this.m_battleBuffStat.StartRecord();
			this.m_optAchievementRecorder.StartRecord();
			this.m_locStat.StartRecord();
			this.m_vdStat.StartRecord();
			this.m_NonHeroInfo.Clear();
			for (uint num = 0u; num < 3u; num += 1u)
			{
				this.campStat.Add(num, new CampInfo((COM_PLAYERCAMP)num));
			}
			this.initEvent();
			this.initNotifyDestroyStat();
			this.m_winMvpId = 0u;
			this.m_loseMvpId = 0u;
			this._useServerMvpScore = false;
			this._mvpScore.Clear();
			this.m_lastBestPlayer = 0u;
			this.u64EmenyIsVisibleToHostHero = 0uL;
			this.m_heroSkillStat.StartRecord();
		}

		private void initNotifyDestroyStat()
		{
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			if (curLvelContext.IsMobaMode())
			{
				return;
			}
			if (curLvelContext != null && curLvelContext.m_starDetail != null)
			{
				for (int i = 0; i < curLvelContext.m_starDetail.Length; i++)
				{
					int iParam = curLvelContext.m_starDetail[i].iParam;
					if (iParam != 0)
					{
						ResEvaluateStarInfo dataByKey = GameDataMgr.evaluateCondInfoDatabin.GetDataByKey((uint)iParam);
						this.CondfoToDestroyStat(dataByKey.astConditions);
					}
				}
			}
			else
			{
				DebugHelper.Assert(false, string.Format("LevelConfig is null -- levelID: {0}", curLvelContext.m_mapID));
			}
		}

		private void CondfoToDestroyStat(ResDT_ConditionInfo[] astCond)
		{
			for (int i = 0; i < astCond.Length; i++)
			{
				if (astCond[i].dwType == 1u && astCond[i].KeyDetail[1] != 0)
				{
					Dictionary<int, DestroyStat> dictionary;
					if (!this.destroyStats.TryGetValue((uint)astCond[i].KeyDetail[0], out dictionary))
					{
						dictionary = new Dictionary<int, DestroyStat>();
						this.destroyStats.Add((uint)astCond[i].KeyDetail[0], dictionary);
					}
					DestroyStat value;
					if (!dictionary.TryGetValue(astCond[i].KeyDetail[1], out value))
					{
						value = default(DestroyStat);
						dictionary.Add(astCond[i].KeyDetail[1], value);
					}
				}
			}
		}

		private void initEvent()
		{
			this.unInitEvent();
			Singleton<GameEventSys>.instance.AddEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.onActorDead));
			Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_BeginFightOver, new RefAction<DefaultGameEventParam>(this.onFightOver));
			Singleton<GameEventSys>.instance.AddEventHandler<PoolObjHandle<ActorRoot>>(GameEventDef.Event_ActorInit, new RefAction<PoolObjHandle<ActorRoot>>(this.OnActorInit));
			Singleton<GameEventSys>.instance.AddEventHandler<HurtEventResultInfo>(GameEventDef.Event_ActorDamage, new RefAction<HurtEventResultInfo>(this.OnActorDamage));
			Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, int, int, int>("HeroSoulExpChange", new Action<PoolObjHandle<ActorRoot>, int, int, int>(this.onSoulExpChange));
			Singleton<EventRouter>.GetInstance().AddEventHandler<PoolObjHandle<ActorRoot>, int, bool, PoolObjHandle<ActorRoot>>("HeroGoldCoinInBattleChange", new Action<PoolObjHandle<ActorRoot>, int, bool, PoolObjHandle<ActorRoot>>(this.OnActorBattleCoinChanged));
			Singleton<GameEventSys>.instance.AddEventHandler<AddSoulExpEventParam>(GameEventDef.Event_AddExpValue, new RefAction<AddSoulExpEventParam>(this.OnAddExpValue));
		}

		public void unInitEvent()
		{
			Singleton<GameEventSys>.instance.RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.onActorDead));
			Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_BeginFightOver, new RefAction<DefaultGameEventParam>(this.onFightOver));
			Singleton<GameEventSys>.instance.RmvEventHandler<PoolObjHandle<ActorRoot>>(GameEventDef.Event_ActorInit, new RefAction<PoolObjHandle<ActorRoot>>(this.OnActorInit));
			Singleton<GameEventSys>.instance.RmvEventHandler<HurtEventResultInfo>(GameEventDef.Event_ActorDamage, new RefAction<HurtEventResultInfo>(this.OnActorDamage));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, int, int, int>("HeroSoulExpChange", new Action<PoolObjHandle<ActorRoot>, int, int, int>(this.onSoulExpChange));
			Singleton<EventRouter>.GetInstance().RemoveEventHandler<PoolObjHandle<ActorRoot>, int, bool, PoolObjHandle<ActorRoot>>("HeroGoldCoinInBattleChange", new Action<PoolObjHandle<ActorRoot>, int, bool, PoolObjHandle<ActorRoot>>(this.OnActorBattleCoinChanged));
			Singleton<GameEventSys>.instance.RmvEventHandler<AddSoulExpEventParam>(GameEventDef.Event_AddExpValue, new RefAction<AddSoulExpEventParam>(this.OnAddExpValue));
		}

		public void PostEndGame()
		{
			this.unInitEvent();
			this.m_battleBuffStat.RemoveTimerEvent();
			this.m_locStat.Clear();
			this.m_vdStat.Clear();
		}

		public void UpdateLogic(int DeltaTime)
		{
			if (this.m_locStat != null)
			{
				this.m_locStat.UpdateLogic(DeltaTime);
			}
			uint num;
			if (Singleton<BattleLogic>.instance.isFighting && Singleton<FrameSynchr>.instance.CurFrameNum % 450u == 0u && (num = Singleton<FrameSynchr>.instance.CurFrameNum / 450u) < 64u)
			{
				List<PoolObjHandle<ActorRoot>> list = ActorHelper.FilterActors(Singleton<GameObjMgr>.instance.HeroActors, new ActorFilterDelegate(Singleton<BattleLogic>.instance.FilterEnemyActor));
				int count = list.Count;
				bool flag = true;
				for (int i = 0; i < count; i++)
				{
					flag &= list[i].handle.Visible;
				}
				this.u64EmenyIsVisibleToHostHero |= (ulong)((ulong)((!flag) ? 0L : 1L) << (int)num);
			}
			if (this.m_playerKDAStat != null)
			{
				this.m_playerKDAStat.UpdateLogic();
			}
		}

		public DictionaryView<uint, CampInfo> GetCampStat()
		{
			return this.campStat;
		}

		public void GetCampsByScoreRank(RES_STAR_CONDITION_DATA_SUB_TYPE inDataSubType, out List<COM_PLAYERCAMP> result, out List<int> resultScore)
		{
			result = new List<COM_PLAYERCAMP>();
			resultScore = new List<int>();
			Dictionary<uint, int> dictionary = new Dictionary<uint, int>();
			DictionaryView<uint, CampInfo>.Enumerator enumerator = this.campStat.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, CampInfo> current = enumerator.Current;
				CampInfo value = current.Value;
				if (value != null)
				{
					KeyValuePair<uint, CampInfo> current2 = enumerator.Current;
					uint key = current2.Key;
					int score = value.GetScore(inDataSubType);
					if (score >= 0)
					{
						dictionary.Add(key, score);
					}
				}
			}
			Dictionary<uint, int>.Enumerator enumerator2 = dictionary.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				KeyValuePair<uint, int> current3 = enumerator2.Current;
				COM_PLAYERCAMP key2 = (COM_PLAYERCAMP)current3.Key;
				KeyValuePair<uint, int> current4 = enumerator2.Current;
				int value2 = current4.Value;
				bool flag = false;
				int count = result.Count;
				for (int i = 0; i < count; i++)
				{
					if (resultScore[i] < value2)
					{
						result.Insert(i, key2);
						resultScore.Insert(i, value2);
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					result.Add(key2);
					resultScore.Add(value2);
				}
			}
			DebugHelper.Assert(resultScore.Count == result.Count);
		}

		public uint GetScoreRank(COM_PLAYERCAMP inCampType, RES_STAR_CONDITION_DATA_SUB_TYPE inDataSubType)
		{
			List<COM_PLAYERCAMP> list = null;
			List<int> list2 = null;
			this.GetCampsByScoreRank(inDataSubType, out list, out list2);
			HashSet<int> hashSet = new HashSet<int>();
			if (list != null && list.Count > 0)
			{
				int num = list.IndexOf(inCampType);
				if (num >= 0)
				{
					for (int i = 0; i <= num; i++)
					{
						hashSet.Add(list2[i]);
					}
				}
			}
			return (uint)hashSet.Count;
		}

		public CampInfo GetCampInfoByCamp(COM_PLAYERCAMP campType)
		{
			CampInfo result;
			if (this.campStat.TryGetValue((uint)campType, out result))
			{
				return result;
			}
			return null;
		}

		public DictionaryView<uint, Dictionary<int, DestroyStat>> GetDestroyStat()
		{
			return this.destroyStats;
		}

		private void onActorDead(ref GameDeadEventParam prm)
		{
			if (prm.bImmediateRevive)
			{
				return;
			}
			this.doDestroyStat(prm.src, prm.orignalAtker);
			if (prm.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
			{
				HeroWrapper heroWrapper = prm.src.handle.ActorControl as HeroWrapper;
				CampInfo campInfo = null;
				PoolObjHandle<ActorRoot> poolObjHandle = new PoolObjHandle<ActorRoot>(null);
				if (prm.orignalAtker && prm.orignalAtker.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
				{
					campInfo = this.GetCampInfoByCamp(prm.orignalAtker.handle.TheActorMeta.ActorCamp);
					poolObjHandle = prm.orignalAtker;
				}
				else if (heroWrapper.IsKilledByHero() && heroWrapper.LastHeroAtker)
				{
					campInfo = this.GetCampInfoByCamp(heroWrapper.LastHeroAtker.handle.TheActorMeta.ActorCamp);
					poolObjHandle = heroWrapper.LastHeroAtker;
				}
				if (campInfo != null && poolObjHandle)
				{
					campInfo.IncCampScore(prm.src, poolObjHandle);
					uint dwConfValue = GameDataMgr.globalInfoDatabin.GetDataByKey(109u).dwConfValue;
					campInfo.IncHeadPoints((int)dwConfValue, prm.src, poolObjHandle);
				}
			}
			else if (prm.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster)
			{
				MonsterWrapper monsterWrapper = prm.src.handle.ActorControl as MonsterWrapper;
				if (monsterWrapper.IsKilledByHero())
				{
					CampInfo campInfoByCamp = this.GetCampInfoByCamp(monsterWrapper.LastHeroAtker.handle.TheActorMeta.ActorCamp);
					DebugHelper.Assert(campInfoByCamp != null);
					if (campInfoByCamp != null)
					{
						campInfoByCamp.IncHeadPoints(monsterWrapper.cfgInfo.iHeadPoints, prm.src, prm.orignalAtker);
					}
				}
				else if (prm.orignalAtker && prm.orignalAtker.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
				{
					CampInfo campInfoByCamp2 = this.GetCampInfoByCamp(prm.orignalAtker.handle.TheActorMeta.ActorCamp);
					DebugHelper.Assert(campInfoByCamp2 != null);
					if (campInfoByCamp2 != null)
					{
						campInfoByCamp2.IncHeadPoints(monsterWrapper.cfgInfo.iHeadPoints, prm.src, prm.orignalAtker);
					}
				}
			}
			DictionaryView<uint, NONHERO_STATISTIC_INFO> dictionaryView;
			NONHERO_STATISTIC_INFO nONHERO_STATISTIC_INFO;
			if (prm.src && prm.src.handle.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Hero && this.m_NonHeroInfo.TryGetValue((uint)prm.src.handle.TheActorMeta.ActorType, out dictionaryView) && dictionaryView.TryGetValue((uint)prm.src.handle.TheActorMeta.ActorCamp, out nONHERO_STATISTIC_INFO))
			{
				nONHERO_STATISTIC_INFO.uiTotalDeadNum += 1u;
			}
			if (prm.atker && prm.src && prm.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ)
			{
				OrganWrapper organWrapper = prm.src.handle.ActorControl as OrganWrapper;
				if (organWrapper != null && organWrapper.cfgInfo.bOrganType == 1)
				{
					CampInfo campInfoByCamp3 = this.GetCampInfoByCamp(prm.atker.handle.TheActorMeta.ActorCamp);
					if (campInfoByCamp3 != null)
					{
						campInfoByCamp3.destoryTowers++;
						Singleton<EventRouter>.instance.BroadCastEvent(EventID.BATTLE_TOWER_DESTROY_CHANGED);
					}
				}
			}
		}

		public void onFightOver(ref DefaultGameEventParam prm)
		{
			this.iBattleResult = Singleton<BattleLogic>.instance.JudgeBattleResult(prm.src, prm.atker);
		}

		private void doDestroyStat(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker)
		{
			Dictionary<int, DestroyStat> dictionary;
			if (!this.destroyStats.TryGetValue((uint)src.handle.TheActorMeta.ActorType, out dictionary))
			{
				dictionary = new Dictionary<int, DestroyStat>();
				this.destroyStats.Add((uint)src.handle.TheActorMeta.ActorType, dictionary);
			}
			DestroyStat value;
			if (!dictionary.TryGetValue(src.handle.TheActorMeta.ConfigId, out value))
			{
				value = default(DestroyStat);
				dictionary.Add(src.handle.TheActorMeta.ConfigId, value);
			}
			COM_PLAYERCAMP playerCamp = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer().PlayerCamp;
			COM_PLAYERCAMP actorCamp = src.handle.TheActorMeta.ActorCamp;
			int num = (playerCamp == actorCamp) ? 0 : 1;
			if (num == 1)
			{
				if (actorCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_MID)
				{
					if (atker && atker.handle.TheActorMeta.ActorCamp == playerCamp)
					{
						value.CampEnemyNum++;
					}
				}
				else
				{
					value.CampEnemyNum++;
				}
			}
			else if (actorCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_MID)
			{
				if (atker && atker.handle.TheActorMeta.ActorCamp != playerCamp)
				{
					value.CampSelfNum++;
				}
			}
			else
			{
				value.CampSelfNum++;
			}
			this.destroyStats[(uint)src.handle.TheActorMeta.ActorType][src.handle.TheActorMeta.ConfigId] = value;
		}

		public int GetCampScore(COM_PLAYERCAMP camp)
		{
			int num = 0;
			DictionaryView<uint, PlayerKDA>.Enumerator enumerator = this.m_playerKDAStat.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
				if (camp == current.Value.PlayerCamp)
				{
					int arg_42_0 = num;
					KeyValuePair<uint, PlayerKDA> current2 = enumerator.Current;
					num = arg_42_0 + current2.Value.numKill;
				}
			}
			return num;
		}

		private void OnActorInit(ref PoolObjHandle<ActorRoot> inActor)
		{
			if (inActor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster)
			{
				DictionaryView<uint, NONHERO_STATISTIC_INFO> dictionaryView;
				if (!this.m_NonHeroInfo.TryGetValue((uint)inActor.handle.TheActorMeta.ActorType, out dictionaryView))
				{
					dictionaryView = new DictionaryView<uint, NONHERO_STATISTIC_INFO>();
					this.m_NonHeroInfo.Add((uint)inActor.handle.TheActorMeta.ActorType, dictionaryView);
				}
				NONHERO_STATISTIC_INFO nONHERO_STATISTIC_INFO;
				if (!dictionaryView.TryGetValue((uint)inActor.handle.TheActorMeta.ActorCamp, out nONHERO_STATISTIC_INFO))
				{
					nONHERO_STATISTIC_INFO = new NONHERO_STATISTIC_INFO();
					dictionaryView.Add((uint)inActor.handle.TheActorMeta.ActorCamp, nONHERO_STATISTIC_INFO);
				}
				nONHERO_STATISTIC_INFO.ActorType = inActor.handle.TheActorMeta.ActorType;
				nONHERO_STATISTIC_INFO.ActorCamp = inActor.handle.TheActorMeta.ActorCamp;
				nONHERO_STATISTIC_INFO.uiTotalSpawnNum += 1u;
				this.m_NonHeroInfo[(uint)inActor.handle.TheActorMeta.ActorType][(uint)inActor.handle.TheActorMeta.ActorCamp] = nONHERO_STATISTIC_INFO;
			}
		}

		private void OnActorDamageAtker(ref HurtEventResultInfo prm)
		{
			if (prm.atker && prm.atker.handle.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Hero && prm.hurtInfo.hurtType != HurtTypeDef.Therapic)
			{
				DictionaryView<uint, NONHERO_STATISTIC_INFO> dictionaryView;
				if (!this.m_NonHeroInfo.TryGetValue((uint)prm.atker.handle.TheActorMeta.ActorType, out dictionaryView))
				{
					dictionaryView = new DictionaryView<uint, NONHERO_STATISTIC_INFO>();
					this.m_NonHeroInfo.Add((uint)prm.atker.handle.TheActorMeta.ActorType, dictionaryView);
				}
				NONHERO_STATISTIC_INFO nONHERO_STATISTIC_INFO;
				if (!dictionaryView.TryGetValue((uint)prm.atker.handle.TheActorMeta.ActorCamp, out nONHERO_STATISTIC_INFO))
				{
					nONHERO_STATISTIC_INFO = new NONHERO_STATISTIC_INFO();
					dictionaryView.Add((uint)prm.atker.handle.TheActorMeta.ActorCamp, nONHERO_STATISTIC_INFO);
				}
				nONHERO_STATISTIC_INFO.uiTotalAttackNum += 1u;
				nONHERO_STATISTIC_INFO.uiTotalHurtCount += (uint)prm.hurtTotal;
				nONHERO_STATISTIC_INFO.uiHurtMax = (uint)((nONHERO_STATISTIC_INFO.uiHurtMax <= (uint)prm.hurtTotal) ? prm.hurtTotal : ((int)nONHERO_STATISTIC_INFO.uiHurtMax));
				nONHERO_STATISTIC_INFO.uiHurtMin = (uint)((nONHERO_STATISTIC_INFO.uiHurtMin >= (uint)prm.hurtTotal) ? prm.hurtTotal : ((int)nONHERO_STATISTIC_INFO.uiHurtMin));
				DebugHelper.Assert(prm.atker.handle.SkillControl != null, "empty skill control");
				if (prm.atker.handle.SkillControl != null && prm.atker.handle.SkillControl.stSkillStat != null && prm.atker.handle.SkillControl.stSkillStat.SkillStatistictInfo != null)
				{
					int atkSlot = (int)prm.hurtInfo.atkSlot;
					int num = prm.atker.handle.SkillControl.stSkillStat.SkillStatistictInfo.Length;
					bool flag = atkSlot >= 0 && atkSlot < num;
					if (flag)
					{
						SKILLSTATISTICTINFO sKILLSTATISTICTINFO = prm.atker.handle.SkillControl.stSkillStat.SkillStatistictInfo[(int)prm.hurtInfo.atkSlot];
						nONHERO_STATISTIC_INFO.uiAttackDistanceMax = (uint)sKILLSTATISTICTINFO.iAttackDistanceMax;
						if (prm.atker.handle.SkillControl.CurUseSkillSlot != null && prm.atker.handle.SkillControl.CurUseSkillSlot.SkillObj != null && prm.atker.handle.SkillControl.CurUseSkillSlot.SkillObj.cfgData != null)
						{
							uint num2 = prm.atker.handle.SkillControl.CurUseSkillSlot.SkillObj.cfgData.iMaxAttackDistance;
							nONHERO_STATISTIC_INFO.uiAttackDistanceMin = ((nONHERO_STATISTIC_INFO.uiAttackDistanceMin >= num2) ? num2 : nONHERO_STATISTIC_INFO.uiAttackDistanceMin);
						}
					}
				}
				if (nONHERO_STATISTIC_INFO.uiFirstBeAttackTime == 0u)
				{
					nONHERO_STATISTIC_INFO.uiFirstBeAttackTime = (uint)Singleton<FrameSynchr>.instance.LogicFrameTick;
				}
				this.m_NonHeroInfo[(uint)prm.atker.handle.TheActorMeta.ActorType][(uint)prm.atker.handle.TheActorMeta.ActorCamp] = nONHERO_STATISTIC_INFO;
			}
		}

		private void OnActorDamage(ref HurtEventResultInfo prm)
		{
			DebugHelper.Assert(this.m_NonHeroInfo != null, "invalid m_NonHeroInfo");
			this.OnActorDamageAtker(ref prm);
			if (prm.src && prm.src.handle.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Hero && prm.hurtInfo.hurtType != HurtTypeDef.Therapic)
			{
				DictionaryView<uint, NONHERO_STATISTIC_INFO> dictionaryView;
				if (!this.m_NonHeroInfo.TryGetValue((uint)prm.src.handle.TheActorMeta.ActorType, out dictionaryView))
				{
					dictionaryView = new DictionaryView<uint, NONHERO_STATISTIC_INFO>();
					this.m_NonHeroInfo.Add((uint)prm.src.handle.TheActorMeta.ActorType, dictionaryView);
				}
				NONHERO_STATISTIC_INFO nONHERO_STATISTIC_INFO;
				if (!dictionaryView.TryGetValue((uint)prm.src.handle.TheActorMeta.ActorCamp, out nONHERO_STATISTIC_INFO))
				{
					nONHERO_STATISTIC_INFO = new NONHERO_STATISTIC_INFO();
					dictionaryView.Add((uint)prm.src.handle.TheActorMeta.ActorCamp, nONHERO_STATISTIC_INFO);
				}
				nONHERO_STATISTIC_INFO.uiTotalBeAttackedNum += 1u;
				nONHERO_STATISTIC_INFO.uiTotalBeHurtCount += (uint)prm.hurtTotal;
				nONHERO_STATISTIC_INFO.uiBeHurtMax = (uint)((nONHERO_STATISTIC_INFO.uiBeHurtMax <= (uint)prm.hurtTotal) ? prm.hurtTotal : ((int)nONHERO_STATISTIC_INFO.uiBeHurtMax));
				nONHERO_STATISTIC_INFO.uiBeHurtMin = (uint)((nONHERO_STATISTIC_INFO.uiBeHurtMin >= (uint)prm.hurtTotal) ? prm.hurtTotal : ((int)nONHERO_STATISTIC_INFO.uiBeHurtMin));
				int num = (prm.src.handle.ValueComponent == null) ? 0 : prm.src.handle.ValueComponent.actorHp;
				int num2 = num - prm.hurtTotal;
				if (num2 < 0)
				{
					num2 = 0;
				}
				nONHERO_STATISTIC_INFO.uiHpMax = (uint)((nONHERO_STATISTIC_INFO.uiHpMax <= (uint)num) ? num : ((int)nONHERO_STATISTIC_INFO.uiHpMax));
				nONHERO_STATISTIC_INFO.uiHpMin = (uint)((nONHERO_STATISTIC_INFO.uiHpMin >= (uint)num2) ? num2 : ((int)nONHERO_STATISTIC_INFO.uiHpMin));
				this.m_NonHeroInfo[(uint)prm.src.handle.TheActorMeta.ActorType][(uint)prm.src.handle.TheActorMeta.ActorCamp] = nONHERO_STATISTIC_INFO;
			}
		}

		private void onSoulExpChange(PoolObjHandle<ActorRoot> act, int changeValue, int curVal, int maxVal)
		{
			CampInfo campInfo = this.campStat[(uint)act.handle.TheActorMeta.ActorCamp];
			campInfo.soulExpTotal += changeValue;
		}

		private void OnActorBattleCoinChanged(PoolObjHandle<ActorRoot> actor, int changeValue, bool isIncome, PoolObjHandle<ActorRoot> target)
		{
			if (isIncome)
			{
				CampInfo campInfo = this.campStat[(uint)actor.handle.TheActorMeta.ActorCamp];
				campInfo.coinTotal += changeValue;
			}
		}

		private void OnAddExpValue(ref AddSoulExpEventParam prm)
		{
			this.m_stSoulStatisticInfo.iAddExpTotal += prm.iAddExpValue;
			if (prm.src)
			{
				if (prm.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster && prm.src.handle.TheActorMeta.ActorCamp != COM_PLAYERCAMP.COM_PLAYERCAMP_MID)
				{
					this.m_stSoulStatisticInfo.iKillSoldierExpMax = Math.Max(this.m_stSoulStatisticInfo.iKillSoldierExpMax, prm.iAddExpValue);
				}
				else if (prm.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster && prm.src.handle.TheActorMeta.ActorCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_MID)
				{
					this.m_stSoulStatisticInfo.iKillMonsterExpMax = Math.Max(this.m_stSoulStatisticInfo.iKillMonsterExpMax, prm.iAddExpValue);
				}
				else if (prm.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
				{
					this.m_stSoulStatisticInfo.iKillHeroExpMax = Math.Max(this.m_stSoulStatisticInfo.iKillHeroExpMax, prm.iAddExpValue);
				}
				else if (prm.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ)
				{
					this.m_stSoulStatisticInfo.iKillOrganExpMax = Math.Max(this.m_stSoulStatisticInfo.iKillOrganExpMax, prm.iAddExpValue);
				}
			}
			if (prm.atker && prm.atker.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
			{
				ValueProperty valueComponent = prm.atker.handle.ValueComponent;
				if (valueComponent != null && valueComponent.ObjValueStatistic != null)
				{
					ulong logicFrameTick = Singleton<FrameSynchr>.instance.LogicFrameTick;
					uint num = (uint)(logicFrameTick - valueComponent.ObjValueStatistic.ulLastAddSoulExpTime);
					valueComponent.ObjValueStatistic.uiAddSoulExpIntervalMax = ((valueComponent.ObjValueStatistic.uiAddSoulExpIntervalMax <= num) ? num : valueComponent.ObjValueStatistic.uiAddSoulExpIntervalMax);
				}
				List<PoolObjHandle<ActorRoot>> heroActors = Singleton<GameObjMgr>.instance.HeroActors;
				for (int i = 0; i < heroActors.Count; i++)
				{
					if (heroActors[i].handle.TheActorMeta.ActorCamp == prm.atker.handle.TheActorMeta.ActorCamp && heroActors[i].handle.ValueComponent != null && heroActors[i].handle.ValueComponent.ObjValueStatistic != null)
					{
						heroActors[i].handle.ValueComponent.ObjValueStatistic.uiTeamSoulExpTotal += (uint)prm.iAddExpValue;
					}
				}
			}
		}

		public void RecordMvp(COMDT_GAME_INFO gameInfo)
		{
			this.m_winMvpId = gameInfo.dwWinMvpObjID;
			this.m_loseMvpId = gameInfo.dwLoseMvpObjID;
			this._mvpScore.Clear();
			COMDT_MVP_SCORE_DETAIL stMvpScoreDetail = gameInfo.stMvpScoreDetail;
			for (int i = 0; i < (int)stMvpScoreDetail.bAcntNum; i++)
			{
				this._mvpScore.Add(stMvpScoreDetail.astMvpScoreDetail[i].dwObjID, stMvpScoreDetail.astMvpScoreDetail[i].iMvpScoreTTH);
			}
			this._useServerMvpScore = true;
		}

		public uint GetWinMvp()
		{
			return this.m_winMvpId;
		}

		public uint GetLoseMvp()
		{
			return this.m_loseMvpId;
		}

		public uint GetLastBestPlayer()
		{
			return this.m_lastBestPlayer;
		}

		public bool GetServerMvpScore(uint playerId, out float score)
		{
			bool result = false;
			score = 0f;
			if (this._useServerMvpScore && this._mvpScore.ContainsKey(playerId))
			{
				result = true;
				int num = 0;
				this._mvpScore.TryGetValue(playerId, out num);
				score = (float)num / 100f;
			}
			return result;
		}

		public int GetServerRawMvpScore(uint playerId)
		{
			int result = 0;
			if (this._useServerMvpScore && this._mvpScore.ContainsKey(playerId))
			{
				this._mvpScore.TryGetValue(playerId, out result);
			}
			return result;
		}

		public uint GetMvpPlayer(COM_PLAYERCAMP camp, bool bWin)
		{
			if (this.m_winMvpId != 0u || this.m_loseMvpId != 0u)
			{
				if (bWin)
				{
					return this.m_winMvpId;
				}
				return this.m_loseMvpId;
			}
			else
			{
				SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
				int num = 0;
				if (curLvelContext != null && curLvelContext.IsMobaModeWithOutGuide())
				{
					num = curLvelContext.m_pvpPlayerNum;
				}
				if (num <= 2)
				{
					return 0u;
				}
				uint result = 0u;
				float num2 = 0f;
				int num3 = 0;
				DictionaryView<uint, PlayerKDA>.Enumerator enumerator = Singleton<BattleStatistic>.instance.m_playerKDAStat.GetEnumerator();
				float num4 = GameDataMgr.globalInfoDatabin.GetDataByKey(177u).dwConfValue / 10000f;
				while (enumerator.MoveNext())
				{
					KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
					PlayerKDA value = current.Value;
					if (!value.bHangup && !value.bRunaway && !value.bDisconnect)
					{
						float mvpValue = value.MvpValue;
						float kDAValue = value.KDAValue;
						if (value.PlayerCamp == camp)
						{
							if (mvpValue >= num2)
							{
								if (mvpValue == num2)
								{
									KeyValuePair<uint, PlayerKDA> current2 = enumerator.Current;
									if (current2.Value.numKill < num3)
									{
										continue;
									}
								}
								if (bWin || mvpValue >= num4)
								{
									if (bWin || kDAValue >= 3f)
									{
										KeyValuePair<uint, PlayerKDA> current3 = enumerator.Current;
										result = current3.Value.PlayerId;
										num2 = mvpValue;
										KeyValuePair<uint, PlayerKDA> current4 = enumerator.Current;
										num3 = current4.Value.numKill;
									}
								}
							}
						}
					}
				}
				return result;
			}
		}

		public uint GetBestPlayer()
		{
			DictionaryView<uint, PlayerKDA>.Enumerator enumerator = Singleton<BattleStatistic>.instance.m_playerKDAStat.GetEnumerator();
			float num = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_HONOR_KILL_FACTOR) / 10000f;
			float num2 = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_HONOR_ASSIST_FACTOR) / 10000f;
			float num3 = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_HONOR_DEAD_FACTOR) / 10000f;
			uint num4 = this.m_lastBestPlayer;
			float num5 = 0f;
			int num6 = 0;
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
				PlayerKDA value = current.Value;
				float num7 = (float)value.numKill * num + (float)value.numAssist * num2 - (float)value.numDead * num3;
				if (Mathf.Approximately(num7, num5))
				{
					KeyValuePair<uint, PlayerKDA> current2 = enumerator.Current;
					if (current2.Value.numKill >= num6)
					{
						if (value.PlayerId != this.m_lastBestPlayer)
						{
							num4 = value.PlayerId;
						}
						num5 = num7;
						KeyValuePair<uint, PlayerKDA> current3 = enumerator.Current;
						num6 = current3.Value.numKill;
					}
				}
				else if (num7 >= num5)
				{
					num4 = value.PlayerId;
					num5 = num7;
					KeyValuePair<uint, PlayerKDA> current4 = enumerator.Current;
					num6 = current4.Value.numKill;
				}
			}
			if (Mathf.Approximately(num5, 0f))
			{
				this.m_lastBestPlayer = 0u;
				return 0u;
			}
			this.m_lastBestPlayer = num4;
			return num4;
		}

		public string GetPlayerName(ulong playerUid, uint playerLogicWorldId)
		{
			if (this.m_playerKDAStat != null)
			{
				DictionaryView<uint, PlayerKDA>.Enumerator enumerator = this.m_playerKDAStat.GetEnumerator();
				while (enumerator.MoveNext())
				{
					KeyValuePair<uint, PlayerKDA> current = enumerator.Current;
					if (playerUid == current.Value.PlayerUid)
					{
						long arg_4D_0 = (long)((ulong)playerLogicWorldId);
						KeyValuePair<uint, PlayerKDA> current2 = enumerator.Current;
						if (arg_4D_0 == (long)current2.Value.WorldId)
						{
							KeyValuePair<uint, PlayerKDA> current3 = enumerator.Current;
							return current3.Value.PlayerName;
						}
					}
				}
			}
			return string.Empty;
		}
	}
}
