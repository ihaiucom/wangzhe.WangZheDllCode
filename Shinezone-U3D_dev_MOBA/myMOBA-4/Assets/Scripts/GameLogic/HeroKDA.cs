using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic.DataCenter;
using CSProtocol;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameLogic
{
	public class HeroKDA : KDAStat
	{
		public int HeroId;

		public uint SkinId;

		public int SoulLevel = 1;

		public COMDT_SETTLE_TALENT_INFO[] TalentArr = new COMDT_SETTLE_TALENT_INFO[5];

		public stEquipInfo[] Equips = new stEquipInfo[6];

		public PoolObjHandle<ActorRoot> actorHero;

		public uint commonSkillID;

		public int CampPos;

		public uint scanEyeCnt;

		public void Initialize(PoolObjHandle<ActorRoot> actorRoot, int campPos)
		{
			this.CampPos = campPos;
			this.actorHero = actorRoot;
			this.HeroId = this.actorHero.handle.TheActorMeta.ConfigId;
			ActorServerData actorServerData = default(ActorServerData);
			IGameActorDataProvider actorDataProvider = Singleton<ActorDataCenter>.instance.GetActorDataProvider(GameActorDataProviderType.ServerDataProvider);
			if (actorDataProvider.GetActorServerData(ref actorRoot.handle.TheActorMeta, ref actorServerData))
			{
				this.SkinId = actorServerData.SkinId;
			}
			this.commonSkillID = 0u;
			actorDataProvider.GetActorServerCommonSkillData(ref this.actorHero.handle.TheActorMeta, out this.commonSkillID);
			for (int i = 0; i < 5; i++)
			{
				this.TalentArr[i] = new COMDT_SETTLE_TALENT_INFO();
			}
			this.m_numKill = 0;
			this.m_numAssist = 0;
			this.m_numDead = 0;
			this.m_stJudgeStat.Reset();
			this._totalCoin = 0;
			this.m_iCoinFromKillMonster = 0;
			this.m_iCoinFromKillDragon = 0;
			this.m_iCoinFromKillHero = 0;
			this.m_iCoinFromKillSolider = 0;
			this.m_iCoinFromKillTower = 0;
			this.m_iCoinFromSystem = 0;
			this.m_iCoinFromSystemNoAI = 0;
			this.scanEyeCnt = 0u;
			this.m_JudgeRecords = new ListValueView<JudgeStatc>(30);
		}

		public void unInit()
		{
			this.HeroId = 0;
			this.SoulLevel = 1;
			for (int i = 0; i < 7; i++)
			{
				Dictionary<uint, uint> dictionary = this.m_arrCoinInfos[i];
				if (dictionary != null)
				{
					dictionary.Clear();
				}
			}
			this.m_JudgeRecords = null;
		}

		private void SetCoinRecorderInfo(KDAStat.GET_COIN_CHANNEL_TYPE type, int iValue, uint uiCurIndex)
		{
			if (this.m_arrCoinInfos != null)
			{
				if (this.m_arrCoinInfos[(int)type] == null)
				{
					this.m_arrCoinInfos[(int)type] = new Dictionary<uint, uint>();
				}
				if (this.m_arrCoinInfos[(int)type] != null)
				{
					if (uiCurIndex != this.uiLastRecordCoinIndex)
					{
						this.m_arrCoinInfos[(int)type].Add(uiCurIndex, (uint)iValue);
					}
					else
					{
						this.m_arrCoinInfos[(int)type].Remove(uiCurIndex);
						this.m_arrCoinInfos[(int)type].Add(uiCurIndex, (uint)iValue);
					}
				}
			}
		}

		public void OnActorBattleCoinChanged(PoolObjHandle<ActorRoot> actor, int changeValue, bool isIncome, PoolObjHandle<ActorRoot> target)
		{
			if (isIncome && actor == this.actorHero)
			{
				uint num = 0u;
				uint num2 = (uint)(Singleton<FrameSynchr>.instance.LogicFrameTick / 1000uL);
				if (num2 >= 60u)
				{
					num = (num2 - 60u) / 30u + 1u;
				}
				this._totalCoin += changeValue;
				if (!this.actorHero.handle.ActorAgent.IsAutoAI() && changeValue > 0)
				{
					this.m_stJudgeStat.GainCoin = (ushort)(this.m_stJudgeStat.GainCoin + (ushort)changeValue);
				}
				this.SetCoinRecorderInfo(KDAStat.GET_COIN_CHANNEL_TYPE.GET_COIN_CHANNEL_TYPE_ALL, this._totalCoin, num);
				if (target)
				{
					ActorTypeDef actorType = target.handle.TheActorMeta.ActorType;
					if (actorType == ActorTypeDef.Actor_Type_Hero)
					{
						this.m_iCoinFromKillHero += changeValue;
						this.SetCoinRecorderInfo(KDAStat.GET_COIN_CHANNEL_TYPE.GET_COIN_CHANNEL_TYPE_KILLHERO, this.m_iCoinFromKillHero, num);
					}
					else if (actorType == ActorTypeDef.Actor_Type_Organ)
					{
						this.m_iCoinFromKillTower += changeValue;
						this.SetCoinRecorderInfo(KDAStat.GET_COIN_CHANNEL_TYPE.GET_COIN_CHANNEL_TYPE_KILLTOWER, this.m_iCoinFromKillTower, num);
					}
					else if (actorType == ActorTypeDef.Actor_Type_Monster)
					{
						if (target.handle.ActorControl.GetActorSubType() == 1)
						{
							this.m_iCoinFromKillSolider += changeValue;
							this.SetCoinRecorderInfo(KDAStat.GET_COIN_CHANNEL_TYPE.GET_COIN_CHANNEL_TYPE_KILLSOLIDER, this.m_iCoinFromKillSolider, num);
						}
						else
						{
							MonsterWrapper monsterWrapper = target.handle.AsMonster();
							if (monsterWrapper != null && monsterWrapper.cfgInfo != null)
							{
								if (monsterWrapper.cfgInfo.bSoldierType == 7 || monsterWrapper.cfgInfo.bSoldierType == 9 || monsterWrapper.cfgInfo.bSoldierType == 8)
								{
									this.m_iCoinFromKillDragon += changeValue;
									this.SetCoinRecorderInfo(KDAStat.GET_COIN_CHANNEL_TYPE.GET_COIN_CHANNEL_TYPE_KILLDRAGON, this.m_iCoinFromKillDragon, num);
								}
								else
								{
									this.m_iCoinFromKillMonster += changeValue;
									this.SetCoinRecorderInfo(KDAStat.GET_COIN_CHANNEL_TYPE.GET_COIN_CHANNEL_TYPE_KILLMONSTER, this.m_iCoinFromKillMonster, num);
								}
							}
						}
					}
				}
				else
				{
					this.m_iCoinFromSystem += changeValue;
					if (!this.actorHero.handle.ActorAgent.IsAutoAI())
					{
						this.m_iCoinFromSystemNoAI += changeValue;
					}
				}
				this.uiLastRecordCoinIndex = num;
			}
		}

		public void OnActorDead(ref GameDeadEventParam prm)
		{
			if (prm.src == this.actorHero)
			{
				base.recordDead(prm.src, prm.orignalAtker);
			}
			else if (prm.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
			{
				HeroWrapper heroWrapper = prm.src.handle.ActorControl as HeroWrapper;
				PoolObjHandle<ActorRoot> poolObjHandle = default(PoolObjHandle<ActorRoot>);
				bool flag = false;
				if (prm.orignalAtker && prm.orignalAtker.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
				{
					flag = true;
					poolObjHandle = prm.orignalAtker;
				}
				else if (heroWrapper.IsKilledByHero())
				{
					flag = true;
					poolObjHandle = heroWrapper.LastHeroAtker;
				}
				if (flag)
				{
					if (poolObjHandle == this.actorHero)
					{
						this.m_numKill++;
						if (!this.actorHero.handle.ActorAgent.IsAutoAI())
						{
							this.m_stJudgeStat.KillNum = (ushort)(this.m_stJudgeStat.KillNum + (ushort)1);
						}
						if (this.actorHero.handle.ValueComponent.actorHp * 100 / this.actorHero.handle.ValueComponent.actorHpTotal <= 10)
						{
							this.m_iKillHeroUnderTenPercent += 1u;
						}
					}
					else
					{
						base.recordAssist(prm.src, prm.orignalAtker, this.actorHero, poolObjHandle);
					}
				}
			}
			else if (prm.orignalAtker && prm.orignalAtker == this.actorHero)
			{
				if (prm.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster)
				{
					if (prm.src.handle.ActorControl.GetActorSubType() == 2)
					{
						this.m_numKillMonster++;
						MonsterWrapper monsterWrapper = prm.src.handle.AsMonster();
						if (monsterWrapper != null && monsterWrapper.cfgInfo != null)
						{
							if (monsterWrapper.cfgInfo.bSoldierType == 7)
							{
								this.m_numKillDragon++;
								Singleton<EventRouter>.instance.BroadCastEvent(EventID.BATTLE_DRAGON_KILL_CHANGED);
							}
							else if (monsterWrapper.cfgInfo.bSoldierType == 9)
							{
								this.m_numKillDragon++;
								Singleton<EventRouter>.instance.BroadCastEvent(EventID.BATTLE_DRAGON_KILL_CHANGED);
							}
							else if (monsterWrapper.cfgInfo.bSoldierType == 8)
							{
								this.m_numKillBaron++;
								Singleton<EventRouter>.instance.BroadCastEvent(EventID.BATTLE_DRAGON_KILL_CHANGED);
							}
							else if (monsterWrapper.cfgInfo.bSoldierType == 10)
							{
								this.m_numKillBlueBa++;
							}
							else if (monsterWrapper.cfgInfo.bSoldierType == 11)
							{
								this.m_numKillRedBa++;
							}
						}
					}
					else if (prm.src.handle.ActorControl.GetActorSubType() == 1)
					{
						this.m_numKillSoldier++;
					}
					if (prm.src.handle.TheActorMeta.ConfigId != prm.src.handle.TheActorMeta.EnCId)
					{
						this.m_numKillFakeMonster++;
					}
				}
				else if (prm.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ)
				{
					if (prm.src.handle.TheStaticData.TheOrganOnlyInfo.OrganType == 1 || prm.src.handle.TheStaticData.TheOrganOnlyInfo.OrganType == 4)
					{
						this.m_numKillOrgan++;
					}
					else if (prm.src.handle.TheStaticData.TheOrganOnlyInfo.OrganType == 2)
					{
						this.m_numDestroyBase++;
					}
				}
				else if (prm.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_EYE)
				{
					EyeWrapper eyeWrapper = prm.src.handle.ActorControl as EyeWrapper;
					if (eyeWrapper != null && !eyeWrapper.bLifeTimeOver)
					{
						this.scanEyeCnt += 1u;
					}
				}
			}
		}

		public void StatisticSkillInfo(HurtEventResultInfo prm)
		{
			if (prm.hurtInfo.atkSlot >= SkillSlotType.SLOT_SKILL_COUNT)
			{
				return;
			}
			if (!this.actorHero || this.actorHero.handle.SkillControl == null || this.actorHero.handle.SkillControl.stSkillStat == null || this.actorHero.handle.SkillControl.stSkillStat.SkillStatistictInfo == null)
			{
				return;
			}
			SKILLSTATISTICTINFO sKILLSTATISTICTINFO = this.actorHero.handle.SkillControl.stSkillStat.SkillStatistictInfo[(int)prm.hurtInfo.atkSlot];
			if (prm.hurtInfo.hurtType != HurtTypeDef.Therapic)
			{
				sKILLSTATISTICTINFO.iHurtTotal += prm.hurtTotal;
				if (prm.src && prm.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
				{
					sKILLSTATISTICTINFO.iHurtToHeroTotal += prm.hurtTotal;
					if (!sKILLSTATISTICTINFO.bIsCurUseSkillHitHero)
					{
						sKILLSTATISTICTINFO.bIsCurUseSkillHitHero = true;
						sKILLSTATISTICTINFO.iUseSkillHitHeroTimes++;
					}
					sKILLSTATISTICTINFO.iHitHeroCount++;
				}
				sKILLSTATISTICTINFO.iHurtMax = Math.Max(sKILLSTATISTICTINFO.iHurtMax, prm.hurtTotal);
				if (sKILLSTATISTICTINFO.iHurtMin == -1)
				{
					sKILLSTATISTICTINFO.iHurtMin = prm.hurtTotal;
				}
				else
				{
					sKILLSTATISTICTINFO.iHurtMin = Math.Min(sKILLSTATISTICTINFO.iHurtMin, prm.hurtTotal);
				}
				if (sKILLSTATISTICTINFO.iTmpHitAllHurtCountIndex++ < sKILLSTATISTICTINFO.iHitCount)
				{
					sKILLSTATISTICTINFO.iTmpHitAllHurtTotal += prm.hurtTotal;
				}
				if (sKILLSTATISTICTINFO.iTmpHitAllHurtCountIndex == sKILLSTATISTICTINFO.iHitCount)
				{
					sKILLSTATISTICTINFO.iHitAllHurtTotalMax = Math.Max(sKILLSTATISTICTINFO.iHitAllHurtTotalMax, sKILLSTATISTICTINFO.iTmpHitAllHurtTotal);
					if (sKILLSTATISTICTINFO.iHitAllHurtTotalMin == -1)
					{
						sKILLSTATISTICTINFO.iHitAllHurtTotalMin = sKILLSTATISTICTINFO.iTmpHitAllHurtTotal;
					}
					else
					{
						sKILLSTATISTICTINFO.iHitAllHurtTotalMin = Math.Min(sKILLSTATISTICTINFO.iHitAllHurtTotalMin, sKILLSTATISTICTINFO.iTmpHitAllHurtTotal);
					}
				}
				sKILLSTATISTICTINFO.iadValue = Math.Max(sKILLSTATISTICTINFO.iadValue, prm.hurtInfo.adValue);
				sKILLSTATISTICTINFO.iapValue = Math.Max(sKILLSTATISTICTINFO.iapValue, prm.hurtInfo.apValue);
				sKILLSTATISTICTINFO.ihemoFadeRate = Math.Max(sKILLSTATISTICTINFO.ihemoFadeRate, prm.hurtInfo.firstHemoFadeRate);
				sKILLSTATISTICTINFO.ihpValue = Math.Max(sKILLSTATISTICTINFO.ihpValue, prm.hurtInfo.hpValue);
				sKILLSTATISTICTINFO.ihurtCount = Math.Max(sKILLSTATISTICTINFO.ihurtCount, prm.hurtInfo.hurtCount);
				sKILLSTATISTICTINFO.ihurtValue = Math.Max(sKILLSTATISTICTINFO.ihurtValue, prm.hurtInfo.hurtValue);
				sKILLSTATISTICTINFO.iloseHpValue = Math.Max(sKILLSTATISTICTINFO.iloseHpValue, prm.hurtInfo.loseHpValue);
			}
		}

		public void StatisticActorInfo(HurtEventResultInfo prm)
		{
			ActorValueStatistic objValueStatistic = this.actorHero.handle.ValueComponent.ObjValueStatistic;
			if (objValueStatistic != null)
			{
				objValueStatistic.iActorLvl = Math.Max(objValueStatistic.iActorLvl, prm.hurtInfo.attackInfo.iActorLvl);
				objValueStatistic.iActorATT = Math.Max(objValueStatistic.iActorATT, prm.hurtInfo.attackInfo.iActorATT);
				objValueStatistic.iActorINT = Math.Max(objValueStatistic.iActorINT, prm.hurtInfo.attackInfo.iActorINT);
				objValueStatistic.iActorMaxHp = Math.Max(objValueStatistic.iActorMaxHp, prm.hurtInfo.attackInfo.iActorMaxHp);
				objValueStatistic.iActorMinHp = Math.Max(objValueStatistic.iActorMinHp, prm.src.handle.ValueComponent.actorHp);
				objValueStatistic.iDEFStrike = Math.Max(objValueStatistic.iDEFStrike, prm.hurtInfo.attackInfo.iDEFStrike);
				objValueStatistic.iRESStrike = Math.Max(objValueStatistic.iRESStrike, prm.hurtInfo.attackInfo.iRESStrike);
				objValueStatistic.iFinalHurt = Math.Max(objValueStatistic.iFinalHurt, prm.hurtInfo.attackInfo.iFinalHurt);
				objValueStatistic.iCritStrikeRate = Math.Max(objValueStatistic.iCritStrikeRate, prm.hurtInfo.attackInfo.iCritStrikeRate);
				objValueStatistic.iCritStrikeValue = Math.Max(objValueStatistic.iCritStrikeValue, prm.hurtInfo.attackInfo.iCritStrikeValue);
				objValueStatistic.iReduceCritStrikeRate = Math.Max(objValueStatistic.iReduceCritStrikeRate, prm.hurtInfo.attackInfo.iReduceCritStrikeRate);
				objValueStatistic.iReduceCritStrikeValue = Math.Max(objValueStatistic.iReduceCritStrikeValue, prm.hurtInfo.attackInfo.iReduceCritStrikeValue);
				objValueStatistic.iCritStrikeEff = Math.Max(objValueStatistic.iCritStrikeEff, prm.hurtInfo.attackInfo.iCritStrikeEff);
				objValueStatistic.iPhysicsHemophagiaRate = Math.Max(objValueStatistic.iPhysicsHemophagiaRate, prm.hurtInfo.attackInfo.iPhysicsHemophagiaRate);
				objValueStatistic.iMagicHemophagiaRate = Math.Max(objValueStatistic.iMagicHemophagiaRate, prm.hurtInfo.attackInfo.iMagicHemophagiaRate);
				objValueStatistic.iPhysicsHemophagia = Math.Max(objValueStatistic.iPhysicsHemophagia, prm.hurtInfo.attackInfo.iPhysicsHemophagia);
				objValueStatistic.iMagicHemophagia = Math.Max(objValueStatistic.iMagicHemophagia, prm.hurtInfo.attackInfo.iMagicHemophagia);
				objValueStatistic.iHurtOutputRate = Math.Max(objValueStatistic.iHurtOutputRate, prm.hurtInfo.attackInfo.iHurtOutputRate);
			}
		}

		public void OnActorDamage(ref HurtEventResultInfo prm)
		{
			if (prm.src == this.actorHero)
			{
				if (prm.hurtInfo.hurtType != HurtTypeDef.Therapic)
				{
					this.m_uiBeAttackedNum += 1u;
					this.m_hurtTakenByEnemy += prm.hurtTotal;
					if (prm.atker && prm.atker.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
					{
						this.m_hurtTakenByHero += prm.hurtTotal;
						if (!this.actorHero.handle.ActorAgent.IsAutoAI() && prm.hurtTotal > 0)
						{
							this.m_stJudgeStat.SufferHero = this.m_stJudgeStat.SufferHero + (uint)prm.hurtTotal;
						}
					}
					this.m_BeHurtMax = ((this.m_BeHurtMax >= prm.hurtTotal) ? this.m_BeHurtMax : prm.hurtTotal);
					if (this.m_BeHurtMin == -1)
					{
						this.m_BeHurtMin = prm.hurtTotal;
					}
					else
					{
						this.m_BeHurtMin = ((this.m_BeHurtMin <= prm.hurtTotal) ? this.m_BeHurtMin : prm.hurtTotal);
					}
				}
				else
				{
					if (prm.atker && prm.atker.handle.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Organ)
					{
						this.m_uiHealNum += 1u;
						this.m_iBeHeal += prm.hurtTotal;
						this.m_BeHealMax = ((this.m_BeHealMax >= prm.hurtTotal) ? this.m_BeHealMax : prm.hurtTotal);
						if (this.m_BeHealMin == -1)
						{
							this.m_BeHealMin = prm.hurtTotal;
						}
						else
						{
							this.m_BeHealMin = ((this.m_BeHealMin <= prm.hurtTotal) ? this.m_BeHealMin : prm.hurtTotal);
						}
					}
					if (prm.atker && prm.atker == this.actorHero)
					{
						this.m_iSelfHealNum++;
						this.m_iSelfHealCount += prm.hurtTotal;
						this.m_iSelfHealMax = ((this.m_iSelfHealMax >= prm.hurtTotal) ? this.m_iSelfHealMax : prm.hurtTotal);
						if (this.m_iSelfHealMin == -1)
						{
							this.m_iSelfHealMin = prm.hurtTotal;
						}
						else
						{
							this.m_iSelfHealMin = ((this.m_iSelfHealMin <= prm.hurtTotal) ? this.m_iSelfHealMin : prm.hurtTotal);
						}
					}
				}
			}
			if (prm.atker && prm.atker == this.actorHero)
			{
				if (prm.hurtInfo.hurtType != HurtTypeDef.Therapic)
				{
					int num = -prm.hpChanged;
					if (prm.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
					{
						this.m_hurtToHero += num;
						this.m_uiHurtToHeroNum += 1u;
						if (!this.actorHero.handle.ActorAgent.IsAutoAI() && num > 0)
						{
							this.m_stJudgeStat.HurtToHero = this.m_stJudgeStat.HurtToHero + (uint)num;
						}
						if (prm.hurtInfo.hurtType == HurtTypeDef.PhysHurt)
						{
							this.m_iPhysHurtToHero += num;
						}
						else if (prm.hurtInfo.hurtType == HurtTypeDef.MagicHurt)
						{
							this.m_iMagicHurtToHero += num;
						}
						else if (prm.hurtInfo.hurtType == HurtTypeDef.RealHurt)
						{
							this.m_iRealHurtToHero += num;
						}
					}
					else if (prm.src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ)
					{
						this.m_iHurtToOrgan += num;
					}
					this.m_hurtToEnemy += num;
					this.m_uiHurtToEnemyNum += 1u;
					if (!this.actorHero.handle.ActorAgent.IsAutoAI() && num > 0)
					{
						this.m_stJudgeStat.HurtToAll = this.m_stJudgeStat.HurtToAll + (uint)num;
					}
					if (prm.atker.handle.SkillControl.CurUseSkill != null && prm.atker.handle.SkillControl.CurUseSkill.SlotType == SkillSlotType.SLOT_SKILL_0)
					{
						this.m_Skill0HurtToEnemy += num;
					}
					if (prm.hurtInfo.atkSlot == SkillSlotType.SLOT_SKILL_COUNT)
					{
						this.m_iEquipeHurtValue += (uint)num;
					}
				}
				else
				{
					this.m_heal += prm.hurtTotal;
				}
				this.StatisticSkillInfo(prm);
				this.StatisticActorInfo(prm);
			}
			else if (prm.atker && prm.atker.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster)
			{
				MonsterWrapper monsterWrapper = prm.atker.handle.ActorControl as MonsterWrapper;
				if (monsterWrapper != null && monsterWrapper.hostActor && monsterWrapper.hostActor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero && monsterWrapper.spawnSkillSlot < SkillSlotType.SLOT_SKILL_COUNT && monsterWrapper.hostActor.handle.SkillControl != null && monsterWrapper.hostActor.handle.SkillControl.stSkillStat != null && monsterWrapper.hostActor.handle.SkillControl.stSkillStat.SkillStatistictInfo != null)
				{
					monsterWrapper.hostActor.handle.SkillControl.stSkillStat.SkillStatistictInfo[(int)monsterWrapper.spawnSkillSlot].iHurtTotal += prm.hurtTotal;
				}
			}
		}

		public void OnActorDoubleKill(ref DefaultGameEventParam prm)
		{
			if (prm.logicAtker && prm.logicAtker == this.actorHero)
			{
				this.m_DoubleKillNum++;
			}
		}

		public void OnActorTripleKill(ref DefaultGameEventParam prm)
		{
			if (prm.logicAtker && prm.logicAtker == this.actorHero)
			{
				this.m_TripleKillNum++;
			}
		}

		public void OnActorQuataryKill(ref DefaultGameEventParam prm)
		{
			if (prm.logicAtker && prm.logicAtker == this.actorHero)
			{
				this.m_QuataryKillNum++;
			}
		}

		public void OnActorPentaKill(ref DefaultGameEventParam prm)
		{
			if (prm.logicAtker && prm.logicAtker == this.actorHero)
			{
				this.m_PentaKillNum++;
			}
		}

		public void OnActorOdyssey(ref DefaultGameEventParam prm)
		{
			if (prm.logicAtker && prm.logicAtker == this.actorHero)
			{
				this.m_LegendaryNum++;
				if (this.m_LegendaryNum > 1)
				{
					this.m_LegendaryNum = 1;
				}
			}
		}

		public void OnActorBeChosen(ref SkillChooseTargetEventParam prm)
		{
			if (prm.src && prm.src == this.actorHero)
			{
				if (prm.atker && prm.src.handle.IsEnemyCamp(prm.atker.handle))
				{
					this.m_uiBeChosenAsAttackTargetNum += 1u;
				}
				else
				{
					this.m_uiBeChosenAsHealTargetNum += 1u;
				}
			}
		}

		public void OnHitTrigger(ref SkillChooseTargetEventParam prm)
		{
			if (prm.atker && prm.atker == this.actorHero && prm.atker.handle.SkillControl != null && prm.atker.handle.SkillControl.stSkillStat != null && prm.atker.handle.SkillControl.stSkillStat.SkillStatistictInfo != null && prm.atker.handle.SkillControl.CurUseSkillSlot != null)
			{
				SKILLSTATISTICTINFO sKILLSTATISTICTINFO = prm.atker.handle.SkillControl.stSkillStat.SkillStatistictInfo[(int)prm.atker.handle.SkillControl.CurUseSkillSlot.SlotType];
				sKILLSTATISTICTINFO.iHitCount = prm.iTargetCount;
				sKILLSTATISTICTINFO.iTmpHitAllHurtCountIndex = 0;
				sKILLSTATISTICTINFO.iTmpHitAllHurtTotal = 0;
				sKILLSTATISTICTINFO.iHitCountMax = Math.Max(sKILLSTATISTICTINFO.iHitCountMax, prm.iTargetCount);
				if (sKILLSTATISTICTINFO.iHitCountMin == -1)
				{
					sKILLSTATISTICTINFO.iHitCountMin = prm.iTargetCount;
				}
				else
				{
					sKILLSTATISTICTINFO.iHitCountMin = Math.Min(sKILLSTATISTICTINFO.iHitCountMin, prm.iTargetCount);
				}
			}
		}

		protected override int GetTotalShieldProtectedValue()
		{
			if (this.actorHero && this.actorHero.handle != null && this.actorHero.handle.BuffHolderComp != null && this.actorHero.handle.BuffHolderComp.protectRule != null)
			{
				return (int)this.actorHero.handle.BuffHolderComp.protectRule.GetProtectTotalValue();
			}
			return 0;
		}

		protected override uint GetShieldProtectedValueFormHero()
		{
			if (this.actorHero && this.actorHero.handle != null && this.actorHero.handle.BuffHolderComp != null && this.actorHero.handle.BuffHolderComp.protectRule != null)
			{
				return this.actorHero.handle.BuffHolderComp.protectRule.ProtectValueFromHero;
			}
			return 0u;
		}

		protected override uint GetBeShieldProtectedValueToHeroPhys()
		{
			if (this.actorHero && this.actorHero.handle != null && this.actorHero.handle.BuffHolderComp != null && this.actorHero.handle.BuffHolderComp.protectRule != null)
			{
				return this.actorHero.handle.BuffHolderComp.protectRule.BeProtectedValueToHeroPhys;
			}
			return 0u;
		}

		protected override uint GetBeShieldProtectedValueToHeroMagic()
		{
			if (this.actorHero && this.actorHero.handle != null && this.actorHero.handle.BuffHolderComp != null && this.actorHero.handle.BuffHolderComp.protectRule != null)
			{
				return this.actorHero.handle.BuffHolderComp.protectRule.BeProtectedValueToHeroMagic;
			}
			return 0u;
		}

		protected override uint GetBeShieldProtectedValueToHeroReal()
		{
			if (this.actorHero && this.actorHero.handle != null && this.actorHero.handle.BuffHolderComp != null && this.actorHero.handle.BuffHolderComp.protectRule != null)
			{
				return this.actorHero.handle.BuffHolderComp.protectRule.BeProtectedValueToHeroReal;
			}
			return 0u;
		}

		public void OnActorHemophagia(ref HemophagiaEventResultInfo prm)
		{
			if (prm.src == this.actorHero)
			{
				this.m_uiHealNum += 1u;
				this.m_iBeHeal += prm.hpChanged;
				this.m_BeHealMax = ((this.m_BeHealMax >= prm.hpChanged) ? this.m_BeHealMax : prm.hpChanged);
				if (this.m_BeHealMin == -1)
				{
					this.m_BeHealMin = prm.hpChanged;
				}
				else
				{
					this.m_BeHealMin = ((this.m_BeHealMin <= prm.hpChanged) ? this.m_BeHealMin : prm.hpChanged);
				}
				this.m_iSelfHealNum++;
				this.m_iSelfHealCount += prm.hpChanged;
				this.m_iSelfHealMax = ((this.m_iSelfHealMax >= prm.hpChanged) ? this.m_iSelfHealMax : prm.hpChanged);
				if (this.m_iSelfHealMin == -1)
				{
					this.m_iSelfHealMin = prm.hpChanged;
				}
				else
				{
					this.m_iSelfHealMin = ((this.m_iSelfHealMin <= prm.hpChanged) ? this.m_iSelfHealMin : prm.hpChanged);
				}
			}
		}

		public void RecordJudgeStatc()
		{
			ushort gainCoin = this.m_stJudgeStat.GainCoin;
			this.m_stJudgeStat.GainCoin = (ushort)((int)gainCoin - this.m_iCoinFromSystemNoAI);
			this.m_JudgeRecords.Add(this.m_stJudgeStat);
			this.m_stJudgeStat.GainCoin = gainCoin;
		}
	}
}
