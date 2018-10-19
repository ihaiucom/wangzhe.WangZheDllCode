using AGE;
using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic.DataCenter;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.GameSystem;
using Assets.Scripts.Sound;
using behaviac;
using CSProtocol;
using ResData;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	[TypeMetaInfo("ObjWrapperAgent", "具有ObjWrapper能力的代理")]
	public class ObjAgent : BTBaseAgent, IPooledMonoBehaviour, IActorComponent
	{
		public const long SKILL_ALLOWABLE_ERROR_VALUE = 11000L;

		private const int DENGER_COOL_TICKS = 30;

		private const int Sound_Interval = 18000;

		private const int Hp_Rate = 10000;

		public const string path_HeadExclamationMark = "Prefab_Skill_Effects/tongyong_effects/UI_fx/Gantanhao_UI_01";

		private const int FrameMod_HeroAutoAI = 3;

		private const int FrameMod_MonsterBoss = 4;

		private const int FrameMod_Organ = 5;

		private const int FrameMod_MonsterNormal = 6;

		public ObjWrapper m_wrapper;

		public bool m_isActionPlaying;

		private PoolObjHandle<AGE.Action> m_currentAction = default(PoolObjHandle<AGE.Action>);

		public uint m_frame;

		private int m_closeToTargetFrame;

		private int m_dengerCoolTick = 30;

		private int m_sound_Interval = 18000;

		public static void Preload(ref ActorPreloadTab result)
		{
			result.AddParticle("Prefab_Skill_Effects/tongyong_effects/UI_fx/Gantanhao_UI_01");
		}

		public virtual void OnCreate()
		{
		}

		public virtual void OnGet()
		{
			this.m_wrapper = null;
			this.m_isActionPlaying = false;
			this.m_currentAction.Release();
			this.m_closeToTargetFrame = 0;
			this.m_dengerCoolTick = 30;
		}

		public virtual void OnRecycle()
		{
			this.m_wrapper = null;
			this.m_currentAction.Release();
		}

		public void Reset()
		{
			this.StopCurAgeAction();
			this.m_currentAction.Release();
			this.m_closeToTargetFrame = 0;
			this.m_dengerCoolTick = 30;
			this.m_frame = 0u;
		}

		public virtual void Born(ActorRoot actor)
		{
			this.m_wrapper = actor.ActorControl;
			Player ownerPlayer = ActorHelper.GetOwnerPlayer(ref this.m_wrapper.actorPtr);
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			if (curLvelContext != null && curLvelContext.IsMobaMode() && curLvelContext.m_warmHeroAiDiffInfo != null && ownerPlayer != null && ownerPlayer.Computer)
			{
				int iAILevel = curLvelContext.m_warmHeroAiDiffInfo.iAILevel;
				ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey((uint)actor.TheActorMeta.ConfigId);
				if (iAILevel == 1)
				{
					this.m_AgentFileName = StringHelper.UTF8BytesToString(ref dataByKey.szAI_Entry);
				}
				else if (iAILevel == 2)
				{
					this.m_AgentFileName = StringHelper.UTF8BytesToString(ref dataByKey.szAI_Simple);
				}
				else if (iAILevel == 3)
				{
					this.m_AgentFileName = StringHelper.UTF8BytesToString(ref dataByKey.szAI_Normal);
				}
				else if (iAILevel == 4)
				{
					this.m_AgentFileName = StringHelper.UTF8BytesToString(ref dataByKey.szAI_Hard);
				}
				else if (iAILevel == 5)
				{
					if (this.m_wrapper.actor.TheActorMeta.ActorCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
					{
						this.m_AgentFileName = StringHelper.UTF8BytesToString(ref dataByKey.szAI_Warm);
					}
					else
					{
						this.m_AgentFileName = StringHelper.UTF8BytesToString(ref dataByKey.szAI_WarmSimple);
					}
					if (curLvelContext.IsGameTypeLadder())
					{
						this.m_AgentFileName = StringHelper.UTF8BytesToString(ref dataByKey.szAI_Warm);
					}
				}
				else if (iAILevel == 6)
				{
					if (this.m_wrapper.actor.TheActorMeta.ActorCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
					{
						this.m_AgentFileName = StringHelper.UTF8BytesToString(ref dataByKey.szAI_Warm);
					}
					else
					{
						this.m_AgentFileName = StringHelper.UTF8BytesToString(ref dataByKey.szAI_WarmSimple);
					}
					if (curLvelContext.IsGameTypeLadder())
					{
						this.m_AgentFileName = StringHelper.UTF8BytesToString(ref dataByKey.szAI_Warm);
					}
				}
				else
				{
					this.m_AgentFileName = StringHelper.UTF8BytesToString(ref dataByKey.szAI_Normal);
				}
				base.SetCurAgentActive();
			}
			else if (actor.CharInfo && !string.IsNullOrEmpty(actor.CharInfo.BtResourcePath) && actor.ActorControl != null)
			{
				this.m_AgentFileName = actor.CharInfo.BtResourcePath;
				base.SetCurAgentActive();
			}
		}

		[MethodMetaInfo("小兵是否需要切换当前目标(不包含炮兵)，需要就返回目标ID，不需要返回传入的之前ID", "对方小兵>对方箭塔>对方英雄，")]
		public uint GetNewTargetByPriority(uint objID, int srchR)
		{
			PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(objID);
			if (!actor)
			{
				return objID;
			}
			if (actor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster)
			{
				return objID;
			}
			ActorRoot nearestEnemyWithTwoPriorityWithoutJungleMonster = Singleton<TargetSearcher>.GetInstance().GetNearestEnemyWithTwoPriorityWithoutJungleMonster(this.m_wrapper.actor, srchR, TargetPriority.TargetPriority_Monster, TargetPriority.TargetPriority_Organ);
			if (nearestEnemyWithTwoPriorityWithoutJungleMonster != null && nearestEnemyWithTwoPriorityWithoutJungleMonster.ObjID != objID)
			{
				return nearestEnemyWithTwoPriorityWithoutJungleMonster.ObjID;
			}
			return objID;
		}

		[MethodMetaInfo("炮兵是否需要切换当前目标，需要就返回目标ID，不需要返回传入的之前ID", "对方箭塔>对方小兵>对方英雄，")]
		public uint GetNewTargetByPriorityForSiege(uint objID, int srchR)
		{
			PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(objID);
			if (!actor)
			{
				return objID;
			}
			if (actor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ)
			{
				return objID;
			}
			ActorRoot nearestEnemyWithTwoPriorityWithoutJungleMonster = Singleton<TargetSearcher>.GetInstance().GetNearestEnemyWithTwoPriorityWithoutJungleMonster(this.m_wrapper.actor, srchR, TargetPriority.TargetPriority_Organ, TargetPriority.TargetPriority_Monster);
			if (nearestEnemyWithTwoPriorityWithoutJungleMonster != null && nearestEnemyWithTwoPriorityWithoutJungleMonster.ObjID != objID)
			{
				return nearestEnemyWithTwoPriorityWithoutJungleMonster.ObjID;
			}
			return objID;
		}

		[MethodMetaInfo("设置怪物耐力是否需要下降", "野怪才有这个功能")]
		public void SetMonsterEnduranceDown(bool state)
		{
			MonsterWrapper monsterWrapper = this.m_wrapper as MonsterWrapper;
			if (monsterWrapper != null)
			{
				monsterWrapper.IsEnduranceDown = state;
			}
		}

		[MethodMetaInfo("获取怪物耐力", "野怪才有这个功能")]
		public int GetMonsterEndurance()
		{
			MonsterWrapper monsterWrapper = this.m_wrapper as MonsterWrapper;
			if (monsterWrapper != null)
			{
				return monsterWrapper.Endurance;
			}
			return 0;
		}

		[MethodMetaInfo("重置怪物耐力", "野怪才有这个功能")]
		public void ResetEndurance()
		{
			MonsterWrapper monsterWrapper = this.m_wrapper as MonsterWrapper;
			if (monsterWrapper != null && monsterWrapper.cfgInfo != null)
			{
				monsterWrapper.Endurance = monsterWrapper.cfgInfo.iPursuitE;
			}
		}

		[MethodMetaInfo("获取自己当前的位置", "")]
		public Vector3 GetMyCurPos()
		{
			return this.m_wrapper.actor.location.vec3;
		}

		[MethodMetaInfo("获取自己当前的朝向", "")]
		public Vector3 GetMyForward()
		{
			return this.m_wrapper.actor.forward.vec3;
		}

		[MethodMetaInfo("获取自己的ID", "")]
		public uint GetMyObjID()
		{
			return this.m_wrapper.actor.ObjID;
		}

		[MethodMetaInfo("获取自己的阵营", "")]
		public COM_PLAYERCAMP GetMyCamp()
		{
			return this.m_wrapper.actor.TheActorMeta.ActorCamp;
		}

		[MethodMetaInfo("获取敌方的阵营", "对中立阵营无效")]
		public COM_PLAYERCAMP GetEnemyCamp()
		{
			if (this.m_wrapper.actor.TheActorMeta.ActorCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
			{
				return COM_PLAYERCAMP.COM_PLAYERCAMP_2;
			}
			if (this.m_wrapper.actor.TheActorMeta.ActorCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_2)
			{
				return COM_PLAYERCAMP.COM_PLAYERCAMP_1;
			}
			return COM_PLAYERCAMP.COM_PLAYERCAMP_1;
		}

		[MethodMetaInfo("获取出生点Index", "")]
		public int GetCampIndex()
		{
			int result = -1;
			if (this.m_wrapper.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
			{
				IGameActorDataProvider actorDataProvider = Singleton<ActorDataCenter>.instance.GetActorDataProvider(GameActorDataProviderType.ServerDataProvider);
				result = actorDataProvider.Fast_GetActorServerDataBornIndex(ref this.m_wrapper.actor.TheActorMeta);
			}
			return result;
		}

		[MethodMetaInfo("获取当前生命值比率", "10000表示满血")]
		public int GetHPPercent()
		{
			return this.m_wrapper.actor.ValueComponent.actorHp * 10000 / this.m_wrapper.actor.ValueComponent.actorHpTotal;
		}

		[MethodMetaInfo("获取当前魂值等级", "获取当前魂值等级")]
		public int GetCurLevel()
		{
			return this.m_wrapper.actor.ValueComponent.actorSoulLevel;
		}

		[MethodMetaInfo("是否是自动AI", "是否是自动AI")]
		public bool IsAutoAI()
		{
			bool flag = false;
			Player ownerPlayer = ActorHelper.GetOwnerPlayer(ref this.m_wrapper.actorPtr);
			if (ownerPlayer != null)
			{
				flag = ownerPlayer.Computer;
			}
			return this.m_wrapper.m_isAutoAI || flag;
		}

		[MethodMetaInfo("是否是掉线玩家", "是否是掉线玩家")]
		public EBTStatus IsOffline()
		{
			if (this.m_wrapper.m_offline)
			{
				return EBTStatus.BT_SUCCESS;
			}
			return EBTStatus.BT_FAILURE;
		}

		[MethodMetaInfo("是否跟随其他玩家", "掉线后，有玩家选择跟随玩家时，这里返回成功")]
		public EBTStatus IsFollowOtherPlayer()
		{
			if (this.m_wrapper.m_followOther)
			{
				return EBTStatus.BT_SUCCESS;
			}
			return EBTStatus.BT_FAILURE;
		}

		[MethodMetaInfo("设置是否走纯AI", "设置AI模式")]
		public void SetAutoAI(bool isAuto)
		{
			this.m_wrapper.m_isAutoAI = isAuto;
		}

		[MethodMetaInfo("是否被人控制(是否是队长)", "是否被人控制,被人控制的就是队长")]
		public bool IsControlByMan()
		{
			Player ownerPlayer = ActorHelper.GetOwnerPlayer(ref this.m_wrapper.actorPtr);
			return ownerPlayer == null || !ownerPlayer.Captain || this.m_wrapper.m_isControledByMan;
		}

		[MethodMetaInfo("是否被当前持有者操控", "是否被当前持有者操控")]
		public EBTStatus IsControlByHostPlayer()
		{
			if (ActorHelper.IsHostCtrlActor(ref this.m_wrapper.actorPtr))
			{
				return EBTStatus.BT_SUCCESS;
			}
			return EBTStatus.BT_FAILURE;
		}

		[MethodMetaInfo("是否低智商AI", "是否低智商AI,条件是个人人机对战，个人的等级在5级以下的敌方电脑AI")]
		public EBTStatus IsLowAI()
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo.PvpLevel > 5u)
			{
				return EBTStatus.BT_FAILURE;
			}
			List<Player> allPlayers = Singleton<GamePlayerCenter>.GetInstance().GetAllPlayers();
			int num = 0;
			for (int i = 0; i < allPlayers.Count; i++)
			{
				if (!allPlayers[i].Computer)
				{
					num++;
					if (num > 1)
					{
						return EBTStatus.BT_FAILURE;
					}
				}
			}
			Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
			if (!Singleton<GamePlayerCenter>.GetInstance().IsAtSameCamp(hostPlayer.PlayerId, this.m_wrapper.actor.TheActorMeta.PlayerId))
			{
				return EBTStatus.BT_SUCCESS;
			}
			return EBTStatus.BT_FAILURE;
		}

		[MethodMetaInfo("当前的行为", "用于决定AI走哪个分支")]
		public ObjBehaviMode GetCurBehavior()
		{
			return this.m_wrapper.myBehavior;
		}

		[MethodMetaInfo("设置当前的行为", "")]
		public void SetCurBehavior(ObjBehaviMode behaviMode)
		{
			if (!this.m_wrapper.IsDeadState)
			{
				this.m_wrapper.SetObjBehaviMode(behaviMode);
			}
		}

		[MethodMetaInfo("切换到下一个行为", "切换到下一个行为,如果有的话")]
		public void SwitchToNextBehavior()
		{
			if (this.m_wrapper.nextBehavior != ObjBehaviMode.State_Null && !this.m_wrapper.IsDeadState)
			{
				this.m_wrapper.SetObjBehaviMode(this.m_wrapper.nextBehavior);
				this.m_wrapper.nextBehavior = ObjBehaviMode.State_Null;
			}
			else if (this.m_wrapper.m_isAutoAI && this.m_wrapper.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
			{
				this.m_wrapper.SetObjBehaviMode(ObjBehaviMode.State_Idle);
			}
		}

		[MethodMetaInfo("获取AI配置表指定Type的配置值", "")]
		public int GetAIParam(RES_AI_PARAM_TYPE type)
		{
			ResAiParamConf dataByKey = GameDataMgr.aiParamConfDataBin.GetDataByKey((uint)((ushort)type));
			if (dataByKey == null)
			{
				return 0;
			}
			return dataByKey.iParam;
		}

		[MethodMetaInfo("获取搜索的范围", "")]
		public int GetSearchRange()
		{
			return this.m_wrapper.SearchRange;
		}

		[MethodMetaInfo("获取视野范围", "")]
		public int GetSightArea()
		{
			return this.m_wrapper.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_SightArea].totalValue;
		}

		[MethodMetaInfo("获取追击范围", "")]
		public int GetPursuitRange()
		{
			MonsterWrapper monsterWrapper = this.m_wrapper.actor.AsMonster();
			if (monsterWrapper != null && monsterWrapper.cfgInfo != null)
			{
				return monsterWrapper.cfgInfo.iPursuitR;
			}
			return 0;
		}

		[MethodMetaInfo("自己是否已经离开了追踪访问", "考虑了新的扇形区域配置")]
		public bool IsOutOfPursuitRange(Vector3 aimPos)
		{
			VInt3 rhs = new VInt3(aimPos);
			MonsterWrapper monsterWrapper = this.m_wrapper.actor.AsMonster();
			if (monsterWrapper != null)
			{
				PursuitInfo pursuit = monsterWrapper.Pursuit;
				if (pursuit != null && pursuit.IsVaild())
				{
					VInt3 vInt = this.m_wrapper.actorLocation - pursuit.PursuitOrigin;
					long sqrMagnitudeLong2D = vInt.sqrMagnitudeLong2D;
					vInt.Normalize();
					int num = pursuit.PursuitAngle / 2;
					if (sqrMagnitudeLong2D >= (long)pursuit.PursuitRadius * (long)pursuit.PursuitRadius || (num < 180 && VInt3.DotXZLong(ref vInt, ref pursuit.PursuitDir) <= (long)pursuit.CosHalfAngle))
					{
						return true;
					}
				}
				else if (monsterWrapper.cfgInfo != null)
				{
					int iPursuitR = monsterWrapper.cfgInfo.iPursuitR;
					return (this.m_wrapper.actorLocation - rhs).sqrMagnitudeLong2D >= (long)(iPursuitR * iPursuitR);
				}
			}
			return false;
		}

		[MethodMetaInfo("获取攻击的范围", "")]
		public int GetAttackRange()
		{
			return this.m_wrapper.AttackRange;
		}

		[MethodMetaInfo("移动命令未完成", "")]
		public bool HasMoveCMD()
		{
			return this.m_wrapper.curMoveCommand != null;
		}

		[MethodMetaInfo("普攻立即释放", "")]
		public bool IsImmediateAttack()
		{
			return this.m_wrapper.actor.SkillControl != null && this.m_wrapper.actor.SkillControl.bImmediateAttack;
		}

		[MethodMetaInfo("清除移动指令", "")]
		public void ClearMoveCMD()
		{
			this.m_wrapper.ClearMoveCommandWithOutNotice();
		}

		[MethodMetaInfo("后续普攻是否打断当前技能", "")]
		public bool IsContinueAbortUseSkill()
		{
			return false;
		}

		[MethodMetaInfo("技能使用完成是否继续普攻", "")]
		public bool IsContinueCommonAttack()
		{
			bool flag = false;
			SkillCache skillUseCache = this.m_wrapper.actor.SkillControl.SkillUseCache;
			OperateMode playerOperateMode = ActorHelper.GetPlayerOperateMode(ref this.m_wrapper.actorPtr);
			if (skillUseCache != null)
			{
				flag = skillUseCache.GetCommonAttackMode();
			}
			if (playerOperateMode == OperateMode.DefaultMode)
			{
				return flag;
			}
			if (!flag)
			{
				Skill curUseSkill = this.m_wrapper.actor.SkillControl.CurUseSkill;
				if (curUseSkill != null && curUseSkill.cfgData != null && ((curUseSkill.AppointType == SkillRangeAppointType.Target && curUseSkill.cfgData.bSkillTargetRule != 2) || curUseSkill.AppointType == SkillRangeAppointType.Pos))
				{
					LockTargetAttackMode lockTargetAttackModeControl = this.m_wrapper.actor.LockTargetAttackModeControl;
					if (lockTargetAttackModeControl != null)
					{
						uint lockTargetID = lockTargetAttackModeControl.GetLockTargetID();
						if (lockTargetAttackModeControl.IsValidLockTargetID(lockTargetID) && skillUseCache != null)
						{
							skillUseCache.SetCommonAttackMode(true);
							return true;
						}
					}
				}
				return false;
			}
			return true;
		}

		[MethodMetaInfo("普攻命令未完成", "")]
		public bool HasNormalAttackCMD()
		{
			SkillCache skillUseCache = this.m_wrapper.actor.SkillControl.SkillUseCache;
			return skillUseCache != null && skillUseCache.GetCommonAttackMode();
		}

		[MethodMetaInfo("自己攻击目标的ID", "")]
		public uint GetMyTargetID()
		{
			uint result = 0u;
			if (this.m_wrapper.myTarget)
			{
				result = this.m_wrapper.myTarget.handle.ObjID;
			}
			return result;
		}

		[MethodMetaInfo("是否被敌方英雄攻击", "")]
		public bool IsAttackedByEnemyHero()
		{
			return this.m_wrapper.m_isAttackedByEnemyHero;
		}

		[MethodMetaInfo("设置是否被敌方英雄攻击", "")]
		public void SetIsAttackByEnemyHero(bool yesOrNot)
		{
			this.m_wrapper.m_isAttackedByEnemyHero = yesOrNot;
		}

		[MethodMetaInfo("获取攻击自己的敌方英雄", "")]
		public uint GetHeroWhoAttackSelf()
		{
			this.m_wrapper.m_isAttackedByEnemyHero = false;
			return this.m_wrapper.LastHeroAtker.handle.ObjID;
		}

		[MethodMetaInfo("是否被敌方攻击", "")]
		public bool IsAttackByEnemy()
		{
			return this.m_wrapper.m_isAttacked;
		}

		[MethodMetaInfo("设置是否被敌方攻击", "")]
		public void SetIsAttackByEnemy(bool yesOrNot)
		{
			this.m_wrapper.m_isAttacked = yesOrNot;
		}

		[MethodMetaInfo("是否可以移动", "是否可以移动")]
		public EBTStatus CanMove()
		{
			if (this.m_wrapper.GetNoAbilityFlag(ObjAbilityType.ObjAbility_Move))
			{
				return EBTStatus.BT_FAILURE;
			}
			return EBTStatus.BT_SUCCESS;
		}

		[MethodMetaInfo("是否在战斗中", "是否在战斗中")]
		public EBTStatus IsInBattle()
		{
			if (this.m_wrapper.IsInBattle)
			{
				return EBTStatus.BT_SUCCESS;
			}
			return EBTStatus.BT_FAILURE;
		}

		[MethodMetaInfo("设置自己进入危险状态", "设置自己进入危险状态")]
		public void SetInDanger(int frame)
		{
			this.m_dengerCoolTick = frame;
		}

		[MethodMetaInfo("是否进入极危险区域", "是否越塔等")]
		public EBTStatus IsOverTower()
		{
			if (this.IsInDanger())
			{
				return EBTStatus.BT_SUCCESS;
			}
			if (Singleton<TargetSearcher>.GetInstance().HasCantAttackEnemyBuilding(this.m_wrapper.actor, 8000))
			{
				this.SetInDanger();
				return EBTStatus.BT_SUCCESS;
			}
			return EBTStatus.BT_FAILURE;
		}

		[MethodMetaInfo("是否在敌人塔下", "是否在敌人塔下")]
		public uint IsUnderEnemyBuilding(int srchR)
		{
			return Singleton<TargetSearcher>.GetInstance().GetEnemyBuilding(this.m_wrapper.actor, srchR);
		}

		[MethodMetaInfo("建筑周围是否有我方小兵", "建筑周围是否有我方小兵")]
		public EBTStatus HasSelfSoldierCountRoundEnemyBuilding(uint buildId)
		{
			EBTStatus result = EBTStatus.BT_FAILURE;
			if (Singleton<TargetSearcher>.GetInstance().HasSelfSoldierCountRoundEnemyBuilding(this.m_wrapper.actor, buildId))
			{
				result = EBTStatus.BT_SUCCESS;
			}
			return result;
		}

		[MethodMetaInfo("是否在攻击建筑时被其他英雄和小兵攻击", "是否在攻击建筑时被其他英雄和小兵攻击")]
		public EBTStatus IsAttackingBuildingAndHeroOrSoldierAttackMe()
		{
			if (!this.m_wrapper.myLastAtker || !this.m_wrapper.myTarget)
			{
				return EBTStatus.BT_FAILURE;
			}
			if (this.m_wrapper.myTarget.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ && this.m_wrapper.myLastAtker.handle.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Organ)
			{
				return EBTStatus.BT_SUCCESS;
			}
			return EBTStatus.BT_FAILURE;
		}

		[MethodMetaInfo("获取失控的类型", "获取失控的类型")]
		public OutOfControlType GetOutOfControlType()
		{
			return this.m_wrapper.m_outOfControl.m_outOfControlType;
		}

		[MethodMetaInfo("是否需要播出生动画", "是否需要播出生动画")]
		public EBTStatus IsNeedToPlayBornAge()
		{
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			if (curLvelContext != null && curLvelContext.IsMobaModeWithOutGuide() && curLvelContext.m_heroAiType == RES_LEVEL_HEROAITYPE.RES_LEVEL_HEROAITYPE_FREEDOM)
			{
				return EBTStatus.BT_SUCCESS;
			}
			return EBTStatus.BT_FAILURE;
		}

		[MethodMetaInfo("显示头部感叹号", "显示头部感叹号")]
		public void ShowHeadExclamationMark()
		{
			this.m_wrapper.actor.HudControl.ShowHeadExclamationMark("Prefab_Skill_Effects/tongyong_effects/UI_fx/Gantanhao_UI_01", 2f);
		}

		[MethodMetaInfo("随机发送信号", "随机发送信号")]
		public void ShowRandomSignal()
		{
			Player ownerPlayer = ActorHelper.GetOwnerPlayer(ref this.m_wrapper.actorPtr);
			uint playerId = ownerPlayer.PlayerId;
			uint configId = (uint)ownerPlayer.Captain.handle.TheActorMeta.ConfigId;
			List<PoolObjHandle<ActorRoot>> towerActors = Singleton<GameObjMgr>.GetInstance().TowerActors;
			int count = towerActors.Count;
			int index = (int)FrameRandom.Random((uint)count);
			int num = (int)(1 + FrameRandom.Random(4u));
			ActorRoot handle = towerActors[index].handle;
			if (handle.IsSelfCamp(this.m_wrapper.actor))
			{
				if (num == 2)
				{
					num = 1;
				}
			}
			else if (num == 3)
			{
				num = 1;
			}
			int num2 = handle.location.x + (int)FrameRandom.Random(10000u) - 5000;
			int y = handle.location.y;
			int num3 = handle.location.z + (int)FrameRandom.Random(6000u) - 3000;
			SignalPanel signalPanel = (Singleton<CBattleSystem>.GetInstance().FightForm == null) ? null : Singleton<CBattleSystem>.GetInstance().FightForm.GetSignalPanel();
			if (signalPanel != null)
			{
				signalPanel.ExecCommand(playerId, configId, num, num2 / 1000, y / 1000, num3 / 1000, 0, 0, 0u);
			}
		}

		[MethodMetaInfo("发送攻击信号", "发送攻击信号")]
		public void ShowAtkSignal()
		{
			SignalPanel signalPanel = (Singleton<CBattleSystem>.GetInstance().FightForm == null) ? null : Singleton<CBattleSystem>.GetInstance().FightForm.GetSignalPanel();
			if (signalPanel != null)
			{
				Player ownerPlayer = ActorHelper.GetOwnerPlayer(ref this.m_wrapper.actorPtr);
				uint playerId = ownerPlayer.PlayerId;
				uint configId = (uint)ownerPlayer.Captain.handle.TheActorMeta.ConfigId;
				signalPanel.ExecCommand(playerId, configId, 2, 0, 0, 0, 0, 0, 0u);
			}
		}

		[MethodMetaInfo("发送集合信号", "发送集合信号")]
		public void ShowTogetherSignal()
		{
			SignalPanel signalPanel = (Singleton<CBattleSystem>.GetInstance().FightForm == null) ? null : Singleton<CBattleSystem>.GetInstance().FightForm.GetSignalPanel();
			if (signalPanel != null)
			{
				Player ownerPlayer = ActorHelper.GetOwnerPlayer(ref this.m_wrapper.actorPtr);
				uint playerId = ownerPlayer.PlayerId;
				uint configId = (uint)ownerPlayer.Captain.handle.TheActorMeta.ConfigId;
				signalPanel.ExecCommand(playerId, configId, 4, 0, 0, 0, 0, 0, 0u);
			}
		}

		[MethodMetaInfo("获取最近的回血点位置", "")]
		public Vector3 GetRestoredHpPos()
		{
			Vector3 result = Vector3.zero;
			VInt3 zero = VInt3.zero;
			VInt3 forward = VInt3.forward;
			if (Singleton<BattleLogic>.GetInstance().mapLogic.GetRevivePosDir(ref this.m_wrapper.actor.TheActorMeta, true, out zero, out forward))
			{
				result = (Vector3)zero;
			}
			return result;
		}

		[MethodMetaInfo("获取最近神符的位置", "获取最近神符的位置,如果返回的位置的Y小于-1000则表示符文无效")]
		public Vector3 GetNearestShenfuInRange(int range)
		{
			foreach (KeyValuePair<int, ShenFuObjects> current in Singleton<ShenFuSystem>.GetInstance()._shenFuTriggerPool)
			{
				if (current.Value.ShenFu.activeSelf)
				{
					Vector3 position = current.Value.ShenFu.transform.position;
					VInt3 rhs = new VInt3(position);
					long num = (long)range;
					if ((this.m_wrapper.actorLocation - rhs).sqrMagnitudeLong2D <= num * num)
					{
						return position;
					}
				}
			}
			Vector3 result = new Vector3(0f, -10000f, 0f);
			return result;
		}

		[MethodMetaInfo("获取队长", "获取队长")]
		public uint GetCaptain()
		{
			Player ownerPlayer = ActorHelper.GetOwnerPlayer(ref this.m_wrapper.actorPtr);
			return ownerPlayer.Captain.handle.ObjID;
		}

		[MethodMetaInfo("获取当前持有者玩的队长", "注意,这只能用在只有一个玩家,其他都是电脑的情况下")]
		public uint GetHostPlayrCaptain()
		{
			Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
			return hostPlayer.Captain.handle.ObjID;
		}

		[MethodMetaInfo("获取领导者", "由后台下发")]
		public uint GetLeader()
		{
			PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(this.m_wrapper.m_leaderID);
			if (actor)
			{
				if (actor.handle.ActorControl.IsDeadState)
				{
					ActorRoot nearestSelfCampHero = Singleton<TargetSearcher>.instance.GetNearestSelfCampHero(this.m_wrapper.actor, 16000);
					if (nearestSelfCampHero != null)
					{
						return nearestSelfCampHero.ObjID;
					}
				}
				return this.m_wrapper.m_leaderID;
			}
			return 0u;
		}

		[MethodMetaInfo("获取主人", "召唤的怪物才有这个功能")]
		public uint GetMaster()
		{
			MonsterWrapper monsterWrapper = this.m_wrapper as MonsterWrapper;
			if (monsterWrapper != null && monsterWrapper.hostActor)
			{
				return monsterWrapper.hostActor.handle.ObjID;
			}
			return 0u;
		}

		[MethodMetaInfo("判断角色是否活的", "是否活的")]
		public EBTStatus IsAlive(uint objID)
		{
			PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(objID);
			if (actor)
			{
				return (!actor.handle.ActorControl.IsDeadState) ? EBTStatus.BT_SUCCESS : EBTStatus.BT_FAILURE;
			}
			return EBTStatus.BT_FAILURE;
		}

		[MethodMetaInfo("获取恐惧自己的敌人", "获取恐惧自己的敌人")]
		public uint GetTerrorMeActor()
		{
			if (this.m_wrapper.m_terrorMeActor)
			{
				return this.m_wrapper.m_terrorMeActor.handle.ObjID;
			}
			return 0u;
		}

		[MethodMetaInfo("获取队友攻击目标", "获取队友攻击目标")]
		public uint GetTeamMemberTarget()
		{
			Player ownerPlayer = ActorHelper.GetOwnerPlayer(ref this.m_wrapper.actorPtr);
			ReadonlyContext<PoolObjHandle<ActorRoot>>.Enumerator enumerator = ownerPlayer.GetAllHeroes().GetEnumerator();
			while (enumerator.MoveNext())
			{
				PoolObjHandle<ActorRoot> current = enumerator.Current;
				ActorRoot handle = current.handle;
				if (handle != null && handle.ActorControl != null && handle.ActorControl.myTarget && handle.ActorControl.myTarget.handle.ActorControl != null && !handle.ActorControl.myTarget.handle.ActorControl.IsDeadState && !this.m_wrapper.actor.IsSelfCamp(handle.ActorControl.myTarget.handle))
				{
					return handle.ActorControl.myTarget.handle.ObjID;
				}
			}
			return 0u;
		}

		[MethodMetaInfo("获取指定角色的攻击目标", "获取指定角色的攻击目标")]
		public uint GetGivenActorTarget(uint objID)
		{
			PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(objID);
			if (actor && actor.handle.ActorControl != null && actor.handle.ActorControl.myTarget && actor.handle.ActorControl.myTarget.handle.ActorControl != null && !actor.handle.ActorControl.myTarget.handle.ActorControl.IsDeadState)
			{
				return actor.handle.ActorControl.myTarget.handle.ObjID;
			}
			return 0u;
		}

		[MethodMetaInfo("是否有队友的血量小于指定的比率值", "是否有队友的血量小于指定的比率值")]
		public bool HasMemberHpLessThan(int hpRate)
		{
			Player ownerPlayer = ActorHelper.GetOwnerPlayer(ref this.m_wrapper.actorPtr);
			ReadonlyContext<PoolObjHandle<ActorRoot>>.Enumerator enumerator = ownerPlayer.GetAllHeroes().GetEnumerator();
			while (enumerator.MoveNext())
			{
				PoolObjHandle<ActorRoot> current = enumerator.Current;
				ActorRoot handle = current.handle;
				if (hpRate > handle.ValueComponent.actorHp * 10000 / handle.ValueComponent.actorHpTotal)
				{
					return true;
				}
			}
			return false;
		}

		[MethodMetaInfo("获取最近的队友", "获取最近的队友")]
		public uint GetNearestMember()
		{
			ActorRoot actorRoot = null;
			Player ownerPlayer = ActorHelper.GetOwnerPlayer(ref this.m_wrapper.actorPtr);
			ReadonlyContext<PoolObjHandle<ActorRoot>>.Enumerator enumerator = ownerPlayer.GetAllHeroes().GetEnumerator();
			ulong num = 18446744073709551615uL;
			while (enumerator.MoveNext())
			{
				PoolObjHandle<ActorRoot> current = enumerator.Current;
				ActorRoot handle = current.handle;
				if (this.m_wrapper.actor.ObjID != handle.ObjID)
				{
					ulong sqrMagnitudeLong2D = (ulong)(handle.location - this.m_wrapper.actor.location).sqrMagnitudeLong2D;
					if (sqrMagnitudeLong2D < num)
					{
						actorRoot = handle;
						num = sqrMagnitudeLong2D;
					}
				}
			}
			if (actorRoot == null)
			{
				return 0u;
			}
			return actorRoot.ObjID;
		}

		[MethodMetaInfo("获取最近的非队长队友", "获取最近的非队长队友")]
		public uint GetNearestMemberNotCaptain()
		{
			ActorRoot actorRoot = null;
			Player ownerPlayer = ActorHelper.GetOwnerPlayer(ref this.m_wrapper.actorPtr);
			ReadonlyContext<PoolObjHandle<ActorRoot>>.Enumerator enumerator = ownerPlayer.GetAllHeroes().GetEnumerator();
			ulong num = 18446744073709551615uL;
			while (enumerator.MoveNext())
			{
				PoolObjHandle<ActorRoot> current = enumerator.Current;
				ActorRoot handle = current.handle;
				if (this.m_wrapper.actor.ObjID != handle.ObjID && ownerPlayer.Captain.handle.ObjID != handle.ObjID)
				{
					ulong sqrMagnitudeLong2D = (ulong)(handle.location - this.m_wrapper.actor.location).sqrMagnitudeLong2D;
					if (sqrMagnitudeLong2D < num)
					{
						actorRoot = handle;
						num = sqrMagnitudeLong2D;
					}
				}
			}
			if (actorRoot == null)
			{
				return 0u;
			}
			return actorRoot.ObjID;
		}

		[MethodMetaInfo("地图AI模式", "地图AI模式")]
		public RES_LEVEL_HEROAITYPE GetMapAIMode()
		{
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			return curLvelContext.m_heroAiType;
		}

		[MethodMetaInfo("对象是否处于攻击模式", "处在被人打和打人的模式")]
		public bool IsActorInBattle(uint objID)
		{
			PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(objID);
			if (!actor)
			{
				return false;
			}
			ObjAgent actorAgent = actor.handle.ActorAgent;
			return actorAgent.GetCurBehavior() != ObjBehaviMode.State_Idle && actorAgent.GetCurBehavior() != ObjBehaviMode.State_Dead && actorAgent.GetCurBehavior() != ObjBehaviMode.State_Null;
		}

		[MethodMetaInfo("对象是否是野区怪物", "不是兵线上的就是野区的小怪物")]
		public bool IsJungleMonster(uint objID)
		{
			PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(objID);
			if (!actor)
			{
				return false;
			}
			MonsterWrapper monsterWrapper = actor.handle.AsMonster();
			return monsterWrapper != null && monsterWrapper.cfgInfo != null && monsterWrapper.cfgInfo.bMonsterType == 2;
		}

		[MethodMetaInfo("获取角色的类型", "英雄，怪物，还是建筑")]
		public ActorTypeDef GetActorType(uint objID)
		{
			PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(objID);
			if (!actor)
			{
				return ActorTypeDef.Invalid;
			}
			return actor.handle.TheActorMeta.ActorType;
		}

		[MethodMetaInfo("获取角色的位置", "获取角色的位置")]
		public Vector3 GetActorPosition(uint objID)
		{
			PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(objID);
			if (!actor)
			{
				return Vector3.zero;
			}
			return actor.handle.location.vec3;
		}

		[MethodMetaInfo("获取指定角色当前生命值比率", "10000表示满血")]
		public int GetActorHPPercent(uint objID)
		{
			PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(objID);
			if (actor)
			{
				return actor.handle.ValueComponent.actorHp * 10000 / actor.handle.ValueComponent.actorHpTotal;
			}
			return 0;
		}

		[MethodMetaInfo("是否是基地", "是否是基地")]
		public EBTStatus IsBase(uint objID)
		{
			PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(objID);
			if (!actor)
			{
				return EBTStatus.BT_FAILURE;
			}
			if (actor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ && actor.handle.TheStaticData.TheOrganOnlyInfo.OrganType == 2)
			{
				return EBTStatus.BT_SUCCESS;
			}
			return EBTStatus.BT_FAILURE;
		}

		[MethodMetaInfo("处于塔下且有小兵时是否危险", "处于塔下且有小兵时是否危险")]
		public EBTStatus IsDangerUnderEnemyBuilding(uint targetHeroId, int srchR)
		{
			ulong num = (ulong)((long)srchR * (long)srchR);
			List<PoolObjHandle<ActorRoot>> heroActors = Singleton<GameObjMgr>.GetInstance().HeroActors;
			int count = heroActors.Count;
			int num2 = 0;
			int num3 = 0;
			ActorRoot actorRoot = null;
			for (int i = 0; i < count; i++)
			{
				ActorRoot handle = heroActors[i].handle;
				if (handle.ObjID == targetHeroId)
				{
					actorRoot = handle;
				}
				ulong sqrMagnitudeLong2D = (ulong)(handle.location - this.m_wrapper.actor.location).sqrMagnitudeLong2D;
				if (sqrMagnitudeLong2D < num)
				{
					if (this.m_wrapper.actor.IsSelfCamp(handle))
					{
						num2++;
					}
					else
					{
						num3++;
					}
				}
			}
			if (actorRoot != null && actorRoot.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
			{
				int num4 = actorRoot.ValueComponent.actorHp * 10000 / actorRoot.ValueComponent.actorHpTotal;
				if (num4 >= 3000 && num4 < 5000 && num2 > num3)
				{
					return EBTStatus.BT_FAILURE;
				}
				if (num4 < 3000 && num2 >= num3)
				{
					return EBTStatus.BT_FAILURE;
				}
				if (num4 >= 5000 && num2 > num3 + 1)
				{
					return EBTStatus.BT_FAILURE;
				}
				return EBTStatus.BT_SUCCESS;
			}
			else
			{
				if (num2 > num3)
				{
					return EBTStatus.BT_FAILURE;
				}
				return EBTStatus.BT_SUCCESS;
			}
		}

		[MethodMetaInfo("是否在一定范围内己方强于敌人", "是否在一定范围内己方强于敌人 strengthRate代表了对方强弱的比率,如0.8,表示是否强于对方血量的0.8")]
		public EBTStatus IsAroundTeamThanStrongThanEnemise(int srchR, int strengthRate)
		{
			ulong num = (ulong)((long)srchR * (long)srchR);
			List<PoolObjHandle<ActorRoot>> heroActors = Singleton<GameObjMgr>.GetInstance().HeroActors;
			int count = heroActors.Count;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			for (int i = 0; i < count; i++)
			{
				ActorRoot handle = heroActors[i].handle;
				ulong sqrMagnitudeLong2D = (ulong)(handle.location - this.m_wrapper.actor.location).sqrMagnitudeLong2D;
				if (sqrMagnitudeLong2D < num)
				{
					if (this.m_wrapper.actor.IsSelfCamp(handle))
					{
						num2 += handle.ValueComponent.actorHp;
						num3 += handle.ValueComponent.actorHpTotal;
					}
					else
					{
						num4 += handle.ValueComponent.actorHp;
						num5 += handle.ValueComponent.actorHpTotal;
					}
				}
			}
			if (num4 == 0)
			{
				return EBTStatus.BT_SUCCESS;
			}
			ulong num6 = (ulong)((long)num2 * (long)num5 * 10000L);
			ulong num7 = (ulong)((long)num4 * (long)num3 * (long)strengthRate);
			if (num6 > num7)
			{
				return EBTStatus.BT_SUCCESS;
			}
			return EBTStatus.BT_FAILURE;
		}

		[MethodMetaInfo("是否有指定的阵营的人在范围内", "是否有指定的阵营的人在范围内")]
		public EBTStatus IsGivenCampActorsInRange(COM_PLAYERCAMP camp, int range)
		{
			ulong num = (ulong)((long)range * (long)range);
			List<PoolObjHandle<ActorRoot>> gameActors = Singleton<GameObjMgr>.GetInstance().GameActors;
			int count = gameActors.Count;
			for (int i = 0; i < count; i++)
			{
				ActorRoot handle = gameActors[i].handle;
				if (handle.TheActorMeta.ActorCamp == camp)
				{
					ulong sqrMagnitudeLong2D = (ulong)(handle.location - this.m_wrapper.actor.location).sqrMagnitudeLong2D;
					if (sqrMagnitudeLong2D < num)
					{
						return EBTStatus.BT_SUCCESS;
					}
				}
			}
			return EBTStatus.BT_FAILURE;
		}

		[MethodMetaInfo("是否有敌人在范围内", "是否有敌人在范围内")]
		public EBTStatus HasEnemyInRange(int range)
		{
			ulong num = (ulong)((long)range * (long)range);
			List<PoolObjHandle<ActorRoot>> gameActors = Singleton<GameObjMgr>.GetInstance().GameActors;
			int count = gameActors.Count;
			for (int i = 0; i < count; i++)
			{
				ActorRoot handle = gameActors[i].handle;
				if (!this.m_wrapper.actor.IsSelfCamp(handle))
				{
					MonsterWrapper monsterWrapper = handle.AsMonster();
					if (monsterWrapper != null)
					{
						ResMonsterCfgInfo cfgInfo = monsterWrapper.cfgInfo;
						if (cfgInfo != null && cfgInfo.bMonsterType == 2)
						{
							ObjAgent actorAgent = handle.ActorAgent;
							if (actorAgent.GetCurBehavior() == ObjBehaviMode.State_Idle || actorAgent.GetCurBehavior() == ObjBehaviMode.State_Dead || actorAgent.GetCurBehavior() == ObjBehaviMode.State_Null)
							{
								goto IL_E0;
							}
						}
					}
					ulong sqrMagnitudeLong2D = (ulong)(handle.location - this.m_wrapper.actor.location).sqrMagnitudeLong2D;
					if (sqrMagnitudeLong2D < num)
					{
						return EBTStatus.BT_SUCCESS;
					}
				}
				IL_E0:;
			}
			return EBTStatus.BT_FAILURE;
		}

		[MethodMetaInfo("获取范围内的敌人数量", "获取范围内的敌人数量")]
		public int GetEnemyCountInRange(int srchR)
		{
			return Singleton<TargetSearcher>.GetInstance().GetEnemyCountInRange(this.m_wrapper.actor, srchR);
		}

		[MethodMetaInfo("获取范围内的敌人英雄数量", "获取范围内的敌人英雄数量")]
		public int GetEnemyHeroCountInRange(int srchR)
		{
			return Singleton<TargetSearcher>.GetInstance().GetEnemyHeroCountInRange(this.m_wrapper.actor, srchR);
		}

		[MethodMetaInfo("获取范围内的友军数量,包含自己", "获取范围内的友军数量,包含自己")]
		public int GetOurCampActorsCount(int srchR)
		{
			int num = 1;
			List<ActorRoot> ourCampActors = Singleton<TargetSearcher>.GetInstance().GetOurCampActors(this.m_wrapper.actor, srchR);
			if (ourCampActors != null)
			{
				return num + ourCampActors.Count;
			}
			return num;
		}

		[MethodMetaInfo("小龙是否活着", "就是小龙是否被刷怪点刷出来了")]
		public EBTStatus IsDragonAlive()
		{
			if (Singleton<BattleLogic>.GetInstance().m_dragonSpawn != null && Singleton<BattleLogic>.GetInstance().m_dragonSpawn.GetSpawnedList().Count > 0)
			{
				PoolObjHandle<ActorRoot> ptr = Singleton<BattleLogic>.GetInstance().m_dragonSpawn.GetSpawnedList()[0];
				if (ptr)
				{
					ActorRoot handle = ptr.handle;
					if (handle.ActorControl.myBehavior != ObjBehaviMode.State_Dead && handle.ActorControl.myBehavior != ObjBehaviMode.State_GameOver)
					{
						return EBTStatus.BT_SUCCESS;
					}
				}
			}
			return EBTStatus.BT_FAILURE;
		}

		[MethodMetaInfo("获取小龙的ID", "获取小龙的ID")]
		public uint GetDragonId()
		{
			if (Singleton<BattleLogic>.GetInstance().m_dragonSpawn != null && Singleton<BattleLogic>.GetInstance().m_dragonSpawn.GetSpawnedList().Count > 0)
			{
				PoolObjHandle<ActorRoot> ptr = Singleton<BattleLogic>.GetInstance().m_dragonSpawn.GetSpawnedList()[0];
				if (ptr)
				{
					ActorRoot handle = ptr.handle;
					return handle.ObjID;
				}
			}
			return 0u;
		}

		[MethodMetaInfo("获取男爵死亡次数", "获取男爵死亡次数")]
		public int GetBaronDeadTimes()
		{
			if (Singleton<BattleStatistic>.instance.m_battleDeadStat != null)
			{
				return Singleton<BattleStatistic>.instance.m_battleDeadStat.GetBaronDeadCount();
			}
			return 0;
		}

		[MethodMetaInfo("是否有塔在范围内，且塔下没有自己的小兵", "是否有塔在范围内，且塔下没有自己的小兵")]
		public EBTStatus HasEnemyBuildingAndEnemyBuildingWillAttackSelf(int srchR)
		{
			if (Singleton<TargetSearcher>.GetInstance().HasEnemyBuildingAndEnemyBuildingWillAttackSelf(this.m_wrapper.actor, srchR))
			{
				return EBTStatus.BT_SUCCESS;
			}
			return EBTStatus.BT_FAILURE;
		}

		[MethodMetaInfo("建筑获取自身机关类型", "建筑获取自身机关类型")]
		public int GetMyOrganType()
		{
			return this.m_wrapper.actor.TheStaticData.TheOrganOnlyInfo.OrganType;
		}

		[MethodMetaInfo("播放一段ageAction", "")]
		public EBTStatus PlayAgeAction(string actionName)
		{
			if (this.m_isActionPlaying)
			{
				if (this.m_currentAction)
				{
					return EBTStatus.BT_RUNNING;
				}
				this.m_isActionPlaying = false;
				return EBTStatus.BT_SUCCESS;
			}
			else
			{
				if (actionName == null || actionName.Length <= 0)
				{
					return EBTStatus.BT_FAILURE;
				}
				this.m_currentAction = new PoolObjHandle<AGE.Action>(ActionManager.Instance.PlayAction(actionName, true, false, new GameObject[]
				{
					base.gameObject
				}));
				SkillUseContext skillUseContext = new SkillUseContext();
				skillUseContext.Originator = this.m_wrapper.actorPtr;
				this.m_currentAction.handle.refParams.AddRefParam("SkillContext", skillUseContext);
				if (!this.m_currentAction)
				{
					return EBTStatus.BT_FAILURE;
				}
				this.m_isActionPlaying = true;
				return EBTStatus.BT_RUNNING;
			}
		}

		[MethodMetaInfo("播放一段出生ageAction", "")]
		public EBTStatus PlayBornAgeAction()
		{
			if (this.m_wrapper.actor.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Hero)
			{
				return EBTStatus.BT_INVALID;
			}
			this.m_wrapper.actor.Visible = true;
			this.m_wrapper.actor.EffectControl.ApplyIntimacyEffect();
			ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey((uint)this.m_wrapper.actor.TheActorMeta.ConfigId);
			string actionName = StringHelper.UTF8BytesToString(ref dataByKey.szBorn_Age);
			return this.PlayAgeAction(actionName);
		}

		[MethodMetaInfo("播放一段复活ageAction", "")]
		public EBTStatus PlayReviveAgeAction()
		{
			if (this.m_wrapper.actor.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Hero)
			{
				return EBTStatus.BT_INVALID;
			}
			ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey((uint)this.m_wrapper.actor.TheActorMeta.ConfigId);
			string actionName = StringHelper.UTF8BytesToString(ref dataByKey.szRevive_Age);
			return this.PlayAgeAction(actionName);
		}

		[MethodMetaInfo("播放一段死亡剧情ageAction", "")]
		public EBTStatus PlayDeadAgeAction()
		{
			if (this.m_isActionPlaying)
			{
				if (this.m_currentAction)
				{
					return EBTStatus.BT_RUNNING;
				}
				this.m_isActionPlaying = false;
				return EBTStatus.BT_SUCCESS;
			}
			else
			{
				string deadAgePath = this.m_wrapper.actor.CharInfo.deadAgePath;
				if (deadAgePath == null || deadAgePath.Length <= 0)
				{
					return EBTStatus.BT_FAILURE;
				}
				this.m_currentAction = new PoolObjHandle<AGE.Action>(ActionManager.Instance.PlayAction(deadAgePath, true, false, new GameObject[]
				{
					base.gameObject
				}));
				if (!this.m_currentAction)
				{
					return EBTStatus.BT_FAILURE;
				}
				this.m_isActionPlaying = true;
				return EBTStatus.BT_RUNNING;
			}
		}

		[MethodMetaInfo("播放死亡后移动到指定点ageAction", "专为")]
		public EBTStatus PlayDeadMoveToPositionAgeAction()
		{
			if (this.m_isActionPlaying)
			{
				if (this.m_currentAction)
				{
					return EBTStatus.BT_RUNNING;
				}
				this.m_isActionPlaying = false;
				return EBTStatus.BT_SUCCESS;
			}
			else
			{
				string deadAgePath = this.m_wrapper.actor.CharInfo.deadAgePath;
				if (deadAgePath == null || deadAgePath.Length <= 0)
				{
					return EBTStatus.BT_FAILURE;
				}
				this.m_currentAction = new PoolObjHandle<AGE.Action>(ActionManager.Instance.PlayAction(deadAgePath, true, false, new GameObject[]
				{
					base.gameObject,
					this.m_wrapper.m_deadPointGo
				}));
				if (!this.m_currentAction)
				{
					return EBTStatus.BT_FAILURE;
				}
				this.m_isActionPlaying = true;
				return EBTStatus.BT_RUNNING;
			}
		}

		[MethodMetaInfo("播放一段AgeHelper的action", "")]
		public EBTStatus PlayHelperAgeAction(string actionName)
		{
			ActionHelper component = base.gameObject.GetComponent<ActionHelper>();
			if (component == null)
			{
				return EBTStatus.BT_FAILURE;
			}
			if (this.m_isActionPlaying)
			{
				if (this.m_currentAction)
				{
					return EBTStatus.BT_RUNNING;
				}
				this.m_isActionPlaying = false;
				return EBTStatus.BT_SUCCESS;
			}
			else
			{
				if (actionName == null || actionName.Length <= 0)
				{
					return EBTStatus.BT_FAILURE;
				}
				this.m_currentAction = new PoolObjHandle<AGE.Action>(component.PlayAction(actionName));
				if (!this.m_currentAction)
				{
					return EBTStatus.BT_FAILURE;
				}
				this.m_isActionPlaying = true;
				return EBTStatus.BT_RUNNING;
			}
		}

		[MethodMetaInfo("停止当前播放的AgeAction", "停止当前播放的AgeAction")]
		public void StopCurAgeAction()
		{
			if (this.m_isActionPlaying)
			{
				if (this.m_currentAction)
				{
					ActionManager.Instance.StopAction(this.m_currentAction);
				}
				this.m_isActionPlaying = false;
			}
		}

		[MethodMetaInfo("通知友军自己被攻击", "通知友军自己被攻击,range是半径")]
		public void NotifySelfCampSelfBeAttacked(int range)
		{
			this.m_wrapper.NotifySelfCampSelfBeAttacked(range);
		}

		[MethodMetaInfo("通知友军自己要主动攻击谁", "通知友军自己要主动攻击,range是半径")]
		public void NotifySelfCampSelfWillAttack(int range)
		{
			this.m_wrapper.NotifySelfCampSelfWillAttack(range);
		}

		[MethodMetaInfo("播放Animation", "")]
		public void PlayAnimation(string animationName, float blendTime, int layer, bool loop)
		{
			this.m_wrapper.PlayAnimation(animationName, blendTime, layer, loop);
		}

		[MethodMetaInfo("是否在播放动画", "")]
		public EBTStatus IsPlayingAnimation()
		{
			if (this.m_wrapper.IsPlayingAnimation())
			{
				return EBTStatus.BT_SUCCESS;
			}
			return EBTStatus.BT_FAILURE;
		}

		[MethodMetaInfo("走砍中的移动", "走砍中的移动")]
		public virtual void RealMoveInMoveAttack(uint objID, int range)
		{
			PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(objID);
			if (!actor)
			{
				return;
			}
			bool flag = false;
			if (actor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
			{
				int num = this.m_wrapper.actor.ValueComponent.actorHp * 10000 / this.m_wrapper.actor.ValueComponent.actorHpTotal;
				int num2 = actor.handle.ValueComponent.actorHp * 10000 / this.m_wrapper.actor.ValueComponent.actorHpTotal;
				if (num > 3000 && num2 < 1000)
				{
					flag = true;
				}
			}
			if (flag)
			{
				this.m_wrapper.RealMovePosition(actor.handle.location, 0u);
			}
			else
			{
				VInt3 vInt = this.m_wrapper.actor.location - actor.handle.location;
				long num3 = (long)(range - vInt.magnitude2D);
				vInt.y = 0;
				vInt = vInt.NormalizeTo(1000);
				VInt3 vInt2 = (!(vInt == VInt3.zero)) ? vInt : VInt3.forward;
				if (actor.handle.ActorControl.GetActorSubType() != 2 && Math.Abs(num3) >= 1000L)
				{
					VInt3 dest = this.m_wrapper.actor.location + vInt2 * (float)num3 / 1000f;
					this.m_wrapper.RealMovePosition(dest, 0u);
				}
				else
				{
					int num4 = (int)(FrameRandom.Random(2u) * 10000);
					VInt3 lhs = VInt3.Cross(VInt3.up, vInt2);
					int rhs = (num4 - 5000) / 5000;
					lhs *= rhs;
					if (actor.handle.ActorControl.GetActorSubType() == 2)
					{
						lhs -= vInt2;
						lhs.NormalizeTo(1000);
					}
					int rhs2 = (int)(2000 + FrameRandom.Random(500u));
					VInt3 dest2 = this.m_wrapper.actor.location + lhs * rhs2 / 1000f;
					this.m_wrapper.RealMovePosition(dest2, 0u);
				}
			}
		}

		[MethodMetaInfo("移动到目标点", "移动到目标点")]
		public virtual void RealMovePosition(Vector3 dest)
		{
			VInt3 dest2 = new VInt3(dest);
			this.m_wrapper.RealMovePosition(dest2, 0u);
		}

		[MethodMetaInfo("设定自己的朝向", "设定自己的朝向")]
		public virtual void LookAtDirection(Vector3 dest)
		{
			if (dest == Vector3.zero)
			{
				return;
			}
			VInt3 inDirection = new VInt3(dest);
			this.m_wrapper.actor.MovementComponent.SetRotate(inDirection, true);
		}

		[MethodMetaInfo("移动到Actor目标点", "移动到Actor目标点")]
		public virtual void RealMoveToActor(uint objID)
		{
			PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(objID);
			if (!actor)
			{
				return;
			}
			this.m_wrapper.RealMovePosition(actor.handle.location, 0u);
		}

		[MethodMetaInfo("移动到Actor目标点左边", "移动到Actor目标点左边")]
		public virtual void RealMoveToActorLeft(uint objID, int unit)
		{
			PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(objID);
			if (!actor)
			{
				return;
			}
			ActorRoot handle = actor.handle;
			VInt3 dest = handle.location + new VInt3(handle.forward.z * -1 * unit, handle.forward.y * unit, handle.forward.x * unit);
			this.m_wrapper.RealMovePosition(dest, 0u);
		}

		[MethodMetaInfo("移动到Actor目标点右边", "移动到Actor目标点右边")]
		public virtual void RealMoveToActorRight(uint objID, int unit)
		{
			PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(objID);
			if (!actor)
			{
				return;
			}
			ActorRoot handle = actor.handle;
			VInt3 dest = handle.location + new VInt3(handle.forward.z * unit, handle.forward.y * unit, handle.forward.x * unit * -1);
			this.m_wrapper.RealMovePosition(dest, 0u);
		}

		[MethodMetaInfo("是否能移动到Actor目标点左边", "是否能移动到Actor目标点左边")]
		public bool IsCanMoveToActorLeft(uint objID, int unit)
		{
			PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(objID);
			if (!actor)
			{
				return false;
			}
			ActorRoot handle = actor.handle;
			VInt3 target = handle.location + new VInt3(handle.forward.z * -1 * unit, handle.forward.y * unit, handle.forward.x * unit);
			return PathfindingUtility.IsValidTarget(handle, target);
		}

		[MethodMetaInfo("是否能移动到Actor目标点右边", "是否能移动到Actor目标点右边")]
		public bool IsCanMoveToActorRight(uint objID, int unit)
		{
			PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(objID);
			if (!actor)
			{
				return false;
			}
			ActorRoot handle = actor.handle;
			VInt3 target = handle.location + new VInt3(handle.forward.z * unit, handle.forward.y * unit, handle.forward.x * unit * -1);
			return PathfindingUtility.IsValidTarget(handle, target);
		}

		[MethodMetaInfo("移动到目标点带ID", "移动到目标点带ID")]
		public virtual void RealMovePositionWithID(Vector3 dest, uint id)
		{
			VInt3 dest2 = new VInt3(dest);
			this.m_wrapper.RealMovePosition(dest2, id);
		}

		[MethodMetaInfo("朝某个方向移动", "朝某个方向移动")]
		public virtual void RealMoveDirection(Vector3 dest)
		{
			VInt3 direction = new VInt3(dest);
			this.m_wrapper.RealMoveDirection(direction, 0u);
		}

		[MethodMetaInfo("朝某个方向移动带ID", "朝某个方向移动带ID")]
		public virtual void RealMoveDirectionWithID(Vector3 dest, uint id)
		{
			VInt3 direction = new VInt3(dest);
			this.m_wrapper.RealMoveDirection(direction, id);
		}

		[MethodMetaInfo("远离指定的点", "远离指定的点")]
		public virtual void LeavePoint(Vector3 dest)
		{
			VInt3 rhs = new VInt3(dest);
			VInt3 direction = this.m_wrapper.actor.location - rhs;
			this.m_wrapper.RealMoveDirection(direction, 0u);
		}

		[MethodMetaInfo("远离指定的Actor", "远离指定的Actor")]
		public virtual void LeaveActor(uint objID)
		{
			PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(objID);
			if (!actor)
			{
				return;
			}
			ActorRoot handle = actor.handle;
			VInt3 location = handle.location;
			VInt3 direction = this.m_wrapper.actor.location - location;
			this.m_wrapper.RealMoveDirection(direction, 0u);
		}

		[MethodMetaInfo("获取远离指定的Actor的随机点", "获取远离指定的Actor的随机点")]
		public virtual Vector3 GetRandomFarPoint(uint objID)
		{
			PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(objID);
			if (!actor)
			{
				return Vector3.zero;
			}
			ActorRoot handle = actor.handle;
			VInt3 location = handle.location;
			VInt3 lhs = (this.m_wrapper.actor.location - location).NormalizeTo(1000);
			for (int i = 0; i < 20; i++)
			{
				VInt3 target = this.m_wrapper.actor.location + lhs * (int)(FrameRandom.Random(5u) + 6);
				target.x += (int)FrameRandom.Random(15000u) * ((FrameRandom.Random(3000u) % 2 != 0) ? -1 : 1);
				target.z += (int)FrameRandom.Random(15000u) * ((FrameRandom.Random(3000u) % 2 != 0) ? -1 : 1);
				if (PathfindingUtility.IsValidTarget(handle, target))
				{
					return target.vec3;
				}
			}
			return this.m_wrapper.actor.location.vec3;
		}

		[MethodMetaInfo("获取当前命令的目的地", "获取当前命令的目的地")]
		public Vector3 GetCmdDest()
		{
            if (this.m_wrapper.curMoveCommand.cmdType == (byte)FRAMECMD_ID_DEF.FRAME_CMD_PLAYERMOVE)
			{
				return ((FrameCommand<MoveToPosCommand>)this.m_wrapper.curMoveCommand).cmdData.destPosition.vec3;
			}
			if (this.m_wrapper.curMoveCommand.cmdType == (byte)FRAMECMD_ID_DEF.FRAME_CMD_ATTACKPOSITION)
			{
				return ((FrameCommand<AttackPositionCommand>)this.m_wrapper.curMoveCommand).cmdData.WorldPos.vec3;
			}
			return Vector3.zero;
		}

		[MethodMetaInfo("获取移动命令的ID", "获取移动命令的ID")]
		public uint GetMoveCmdId()
		{
			if (this.m_wrapper.curMoveCommand == null)
			{
				return 0u;
			}
			return this.m_wrapper.curMoveCommand.cmdId;
		}

		[MethodMetaInfo("移动命令是否完成", "移动是否完成,这个是指大的移动命令是否完成")]
		public virtual bool IsMoveCMDCompleted()
		{
			if (this.m_wrapper.actor.MovementComponent.isFinished)
			{
				if (this.m_wrapper.curMoveCommand == null)
				{
					return true;
				}
				if (this.m_wrapper.curMoveCommand.cmdId > 0u && this.m_wrapper.curMoveCommand.cmdId == this.m_wrapper.actor.MovementComponent.uCommandId)
				{
					return true;
				}
			}
			return false;
		}

		[MethodMetaInfo("当前移动是否完成", "")]
		public virtual bool IsCurMoveCompleted()
		{
			return this.m_wrapper.actor.MovementComponent.isFinished || !this.m_wrapper.actor.MovementComponent.isMoving;
		}

		[MethodMetaInfo("停止移动并清空移动命令", "停止移动,清除移动命令,停止移动组件")]
		public void StopMove()
		{
			this.m_wrapper.CmdStopMove();
		}

		[MethodMetaInfo("终止当前的移动", "仅仅停止移动组件,不走了")]
		public void TerminateMove()
		{
			this.m_wrapper.TerminateMove();
		}

		[MethodMetaInfo("执行当前命令移动", "执行命令的移动")]
		public void ExMoveCmd()
		{
			this.m_wrapper.BTExMoveCmd();
		}

		[MethodMetaInfo("向目标移动", "前提是已设定目标")]
		public void MoveToTarget()
		{
			this.m_wrapper.MoveToTarget();
		}

		[MethodMetaInfo("自己同指定位置目标的距离是否大于指定值", "")]
		public bool IsDistanceToPosMoreThanRange(Vector3 aimPos, int range)
		{
			VInt3 rhs = new VInt3(aimPos);
			long num = (long)range;
			return (this.m_wrapper.actorLocation - rhs).sqrMagnitudeLong2D > num * num;
		}

		[MethodMetaInfo("自己同指定位置目标的距离是否小于指定值", "")]
		public bool IsDistanceToPosLessThanRange(Vector3 aimPos, int range)
		{
			VInt3 rhs = new VInt3(aimPos);
			long num = (long)range;
			return (this.m_wrapper.actorLocation - rhs).sqrMagnitudeLong2D < num * num;
		}

		[MethodMetaInfo("自己同指定Actor目标的距离是否大于指定值", "")]
		public bool IsDistanceToActorMoreThanRange(uint objID, int range)
		{
			PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(objID);
			if (!actor)
			{
				return false;
			}
			ActorRoot handle = actor.handle;
			long num = (long)range;
			if (handle.CharInfo != null)
			{
				num += (long)handle.CharInfo.iCollisionSize.x;
			}
			return (this.m_wrapper.actorLocation - handle.location).sqrMagnitudeLong2D > num * num;
		}

		[MethodMetaInfo("自己同指定Actor目标的距离是否小于指定值", "")]
		public bool IsDistanceToActorLessThanRange(uint objID, int range)
		{
			PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(objID);
			if (!actor)
			{
				return false;
			}
			ActorRoot handle = actor.handle;
			long num = (long)range;
			if (handle.CharInfo != null)
			{
				num += (long)handle.CharInfo.iCollisionSize.x;
			}
			return (this.m_wrapper.actorLocation - handle.location).sqrMagnitudeLong2D < num * num;
		}

		[MethodMetaInfo("获取路径点中的当前点的位置", "前提是已设定好路径")]
		public Vector3 GetRouteCurWaypointPos()
		{
			return this.m_wrapper.GetRouteCurWaypointPos().vec3;
		}

		[MethodMetaInfo("获取路径点中的当前点的位置,用于沿路径点返回", "前提是已设定好路径")]
		public Vector3 GetRouteCurWaypointPosPre()
		{
			return this.m_wrapper.GetRouteCurWaypointPosPre().vec3;
		}

		[MethodMetaInfo("当前路径点是不是起始点", "当前路径点是不是起始点")]
		public EBTStatus IsCurWayPointStartPoint()
		{
			if (this.m_wrapper.m_isStartPoint)
			{
				return EBTStatus.BT_SUCCESS;
			}
			return EBTStatus.BT_FAILURE;
		}

		[MethodMetaInfo("路径点中的当前点是否有效", "前提是已设定好路径")]
		public bool IsCurWaypointValid()
		{
			return this.m_wrapper.IsCurWaypointValid();
		}

		[MethodMetaInfo("当前路径点是不是最后一个路径点", "前提是已设定好路径")]
		public bool IsCurWaypointEndPoint()
		{
			return this.m_wrapper.m_isCurWaypointEndPoint;
		}

		[MethodMetaInfo("选择一条兵线做路径", "随机选择一条兵线")]
		public bool SelectRoute()
		{
			if (this.m_wrapper == null)
			{
				DebugHelper.Assert(false, "m_wrapper为空");
				return false;
			}
			if (Singleton<BattleLogic>.GetInstance().mapLogic == null)
			{
				DebugHelper.Assert(false, "BattleLogic.GetInstance().mapLogic为空, GameState:{0}", new object[]
				{
					Singleton<GameStateCtrl>.instance.currentStateName
				});
				return false;
			}
			ListView<WaypointsHolder> waypointsList = Singleton<BattleLogic>.GetInstance().mapLogic.GetWaypointsList(this.m_wrapper.actor.TheActorMeta.ActorCamp);
			if (waypointsList == null || waypointsList.Count == 0)
			{
				return false;
			}
			int num = (int)FrameRandom.Random(10000u);
			num %= waypointsList.Count;
			if (waypointsList[num] == null)
			{
				DebugHelper.Assert(false, "routeList[index]为空");
				return false;
			}
			this.m_wrapper.m_curWaypointsHolder = waypointsList[num];
			this.m_wrapper.m_curWaypointTarget = this.m_wrapper.m_curWaypointsHolder.startPoint;
			this.m_wrapper.m_curWaypointTargetPosition = new VInt3(this.m_wrapper.m_curWaypointTarget.transform.position);
			return true;
		}

		[MethodMetaInfo("根据在队伍中的位置选择一条兵线", "随机选择一条兵线")]
		public bool SelectRouteBySelfIndex()
		{
			ListView<WaypointsHolder> waypointsList = Singleton<BattleLogic>.GetInstance().mapLogic.GetWaypointsList(this.m_wrapper.actor.TheActorMeta.ActorCamp);
			if (waypointsList == null || waypointsList.Count == 0)
			{
				return false;
			}
			Player ownerPlayer = ActorHelper.GetOwnerPlayer(ref this.m_wrapper.actorPtr);
			if (ownerPlayer == null || this.m_wrapper == null)
			{
				return false;
			}
			int heroTeamPosIndex = ownerPlayer.GetHeroTeamPosIndex((uint)this.m_wrapper.actor.TheActorMeta.ConfigId);
			if (heroTeamPosIndex < 0)
			{
				return false;
			}
			for (int i = 0; i < waypointsList.Count; i++)
			{
				if (waypointsList[i] == null)
				{
					return false;
				}
				if (waypointsList[i].m_index == heroTeamPosIndex)
				{
					this.m_wrapper.m_curWaypointsHolder = waypointsList[i];
					this.m_wrapper.m_curWaypointTarget = this.m_wrapper.m_curWaypointsHolder.startPoint;
					this.m_wrapper.m_curWaypointTargetPosition = new VInt3(this.m_wrapper.m_curWaypointTarget.transform.position);
					return true;
				}
			}
			if (waypointsList.Count > heroTeamPosIndex)
			{
				this.m_wrapper.m_curWaypointsHolder = waypointsList[heroTeamPosIndex];
				this.m_wrapper.m_curWaypointTarget = this.m_wrapper.m_curWaypointsHolder.startPoint;
				this.m_wrapper.m_curWaypointTargetPosition = new VInt3(this.m_wrapper.m_curWaypointTarget.transform.position);
				return true;
			}
			if (waypointsList.Count > 0)
			{
				this.m_wrapper.m_curWaypointsHolder = waypointsList[0];
				this.m_wrapper.m_curWaypointTarget = this.m_wrapper.m_curWaypointsHolder.startPoint;
				this.m_wrapper.m_curWaypointTargetPosition = new VInt3(this.m_wrapper.m_curWaypointTarget.transform.position);
				return true;
			}
			return false;
		}

		[MethodMetaInfo("根据在阵营中的位置选择一条兵线", "根据在阵营中的位置选择一条兵线")]
		public EBTStatus SelectRouteBySelfCampIndex()
		{
			IGameActorDataProvider actorDataProvider = Singleton<ActorDataCenter>.instance.GetActorDataProvider(GameActorDataProviderType.ServerDataProvider);
			int num = actorDataProvider.Fast_GetActorServerDataBornIndex(ref this.m_wrapper.actor.TheActorMeta);
			ListView<WaypointsHolder> waypointsList = Singleton<BattleLogic>.GetInstance().mapLogic.GetWaypointsList(this.m_wrapper.actor.TheActorMeta.ActorCamp);
			if (waypointsList == null || waypointsList.Count == 0)
			{
				return EBTStatus.BT_INVALID;
			}
			if (num < 0)
			{
				return EBTStatus.BT_INVALID;
			}
			for (int i = 0; i < waypointsList.Count; i++)
			{
				if (!(waypointsList[i] == null))
				{
					if (waypointsList[i].m_index == num)
					{
						this.m_wrapper.m_curWaypointsHolder = waypointsList[i];
						this.m_wrapper.m_curWaypointTarget = this.m_wrapper.m_curWaypointsHolder.startPoint;
						this.m_wrapper.m_curWaypointTargetPosition = new VInt3(this.m_wrapper.m_curWaypointTarget.transform.position);
						return EBTStatus.BT_SUCCESS;
					}
				}
			}
			num %= waypointsList.Count;
			this.m_wrapper.m_curWaypointsHolder = waypointsList[num];
			this.m_wrapper.m_curWaypointTarget = this.m_wrapper.m_curWaypointsHolder.startPoint;
			this.m_wrapper.m_curWaypointTargetPosition = new VInt3(this.m_wrapper.m_curWaypointTarget.transform.position);
			return EBTStatus.BT_SUCCESS;
		}

		[MethodMetaInfo("选择离自己最近的一条兵线", "选择离自己最近的一条兵线")]
		public EBTStatus SelectNearestRoute()
		{
			if (Singleton<BattleLogic>.GetInstance() == null || Singleton<BattleLogic>.GetInstance().mapLogic == null)
			{
				return EBTStatus.BT_FAILURE;
			}
			ListView<WaypointsHolder> waypointsList = Singleton<BattleLogic>.GetInstance().mapLogic.GetWaypointsList(this.m_wrapper.actor.TheActorMeta.ActorCamp);
			if (waypointsList == null || waypointsList.Count == 0)
			{
				return EBTStatus.BT_FAILURE;
			}
			long num = 9223372036854775807L;
			WaypointsHolder waypointsHolder = null;
			for (int i = 0; i < waypointsList.Count; i++)
			{
				VInt3 rhs = new VInt3(waypointsList[i].startPoint.transform.position);
				long sqrMagnitudeLong2D = (this.m_wrapper.actorLocation - rhs).sqrMagnitudeLong2D;
				if (sqrMagnitudeLong2D < num)
				{
					waypointsHolder = waypointsList[i];
					num = sqrMagnitudeLong2D;
				}
			}
			if (waypointsHolder == null)
			{
				return EBTStatus.BT_FAILURE;
			}
			this.m_wrapper.m_curWaypointsHolder = waypointsHolder;
			this.m_wrapper.m_curWaypointTarget = this.m_wrapper.m_curWaypointsHolder.startPoint;
			this.m_wrapper.m_curWaypointTargetPosition = new VInt3(this.m_wrapper.m_curWaypointTarget.transform.position);
			return EBTStatus.BT_SUCCESS;
		}

		[MethodMetaInfo("是否已有路径", "是否有路径")]
		public bool HasRoute()
		{
			return !(this.m_wrapper.m_curWaypointsHolder == null);
		}

		[MethodMetaInfo("重置寻路路径", "设置寻路路点从第一个开始")]
		public EBTStatus ResetRouteStartPoint()
		{
			if (this.m_wrapper.m_curWaypointsHolder == null || this.m_wrapper.m_curWaypointsHolder.startPoint == null || this.m_wrapper.m_curWaypointTarget.transform == null)
			{
				return EBTStatus.BT_SUCCESS;
			}
			this.m_wrapper.m_curWaypointTarget = this.m_wrapper.m_curWaypointsHolder.startPoint;
			this.m_wrapper.m_curWaypointTargetPosition = new VInt3(this.m_wrapper.m_curWaypointTarget.transform.position);
			return EBTStatus.BT_SUCCESS;
		}

		[MethodMetaInfo("获取当前路径最后的方向", "获取当前路径最后的方向")]
		public Vector3 GetCurRouteLastForward()
		{
			if (this.m_wrapper.m_curWaypointsHolder == null || this.m_wrapper.m_curWaypointsHolder.wayPoints == null || this.m_wrapper.m_curWaypointsHolder.wayPoints.Length <= 1)
			{
				return Vector3.zero;
			}
			Waypoint endPoint = this.m_wrapper.m_curWaypointsHolder.endPoint;
			Waypoint waypoint = this.m_wrapper.m_curWaypointsHolder.wayPoints[this.m_wrapper.m_curWaypointsHolder.wayPoints.Length - 2];
			return endPoint.transform.position - waypoint.transform.position;
		}

		[MethodMetaInfo("获取多边形边上的一点", "index表示第几条边")]
		public Vector3 GetPolygonEdgePoint(int index)
		{
			VInt2 randomPoint = this.m_wrapper.m_rangePolygon.GetRandomPoint(index);
			VInt3 vInt = new VInt3(randomPoint.x, this.m_wrapper.actor.location.y, randomPoint.y);
			return vInt.vec3;
		}

		[MethodMetaInfo("获取指定点周围的随机点", "获取指定点周围的随机点")]
		public Vector3 GetRandomPointAroundGivenPoint(Vector3 aimPos, int range)
		{
			int num = (int)FrameRandom.Random((uint)(range * 2)) - range;
			int num2 = (int)FrameRandom.Random((uint)(range * 2)) - range;
			VInt3 target = new VInt3((int)(aimPos.x * 1000f + (float)num), (int)(aimPos.y * 1000f), (int)(aimPos.z * 1000f + (float)num2));
			if (PathfindingUtility.IsValidTarget(this.m_wrapper.actor, target))
			{
				return target.vec3;
			}
			return aimPos;
		}

		[MethodMetaInfo("获取指定点朝向随机点", "获取指定点朝向随机点")]
		public Vector3 GetRandomPointByGivenPoint(Vector3 aimPos, int range, int distance)
		{
			Vector3 vector = aimPos - this.m_wrapper.actor.location.vec3;
			Vector3 aimPos2 = this.m_wrapper.actor.location.vec3 + vector.normalized * ((float)distance * 0.001f);
			return this.GetRandomPointAroundGivenPoint(aimPos2, range);
		}

		[MethodMetaInfo("获取指定点朝向随机点,指定最小随机", "获取指定点朝向随机点,指定最小随机")]
		public Vector3 GetRandomPointByGivenPointAndMinRange(Vector3 aimPos, int maxRange, int minRange, int distance)
		{
			VInt3 lhs = new VInt3(aimPos);
			VInt3 vInt = lhs - this.m_wrapper.actor.location;
			VInt3 vInt2 = this.m_wrapper.actor.location + vInt.NormalizeTo(1000) * (distance / 1000);
			int num = maxRange - minRange;
			int num2 = (int)FrameRandom.Random((uint)(num * 2)) - num;
			int num3 = (int)FrameRandom.Random((uint)(num * 2)) - num;
			if (num2 > 0)
			{
				num2 += minRange;
			}
			else
			{
				num2 -= minRange;
			}
			if (num3 > 0)
			{
				num3 += minRange;
			}
			else
			{
				num3 -= minRange;
			}
			VInt3 target = new VInt3(vInt2.x + num2, vInt2.y, vInt2.z + num3);
			if (PathfindingUtility.IsValidTarget(this.m_wrapper.actor, target))
			{
				return target.vec3;
			}
			return vInt2.vec3;
		}

		[MethodMetaInfo("使用技能", "")]
		public EBTStatus RealUseSkill(SkillSlotType InSlot)
		{
			bool flag = this.m_wrapper.RealUseSkill(InSlot);
			if (flag)
			{
				return EBTStatus.BT_SUCCESS;
			}
			return EBTStatus.BT_FAILURE;
		}

		[MethodMetaInfo("中断当前正在施放的技能", "")]
		public bool AbortCurUseSkill()
		{
			return true;
		}

		[MethodMetaInfo("获取技能目标规则", "获取技能目标规则")]
		public SkillTargetRule GetSkillTargetRule(SkillSlotType InSlot)
		{
			Skill skill = this.m_wrapper.GetSkill(InSlot);
			if (skill != null && skill.cfgData != null)
			{
				return (SkillTargetRule)skill.cfgData.bSkillTargetRule;
			}
			return SkillTargetRule.LowerHpEnermy;
		}

		[MethodMetaInfo("获取召唤师技能类型", "获取召唤师技能类型")]
		public RES_SUMMONERSKILL_TYPE GetSummonerSkillType(SkillSlotType InSlot)
		{
			Skill skill = this.m_wrapper.GetSkill(InSlot);
			if (skill != null && skill.cfgData != null)
			{
				return (RES_SUMMONERSKILL_TYPE)skill.cfgData.bSkillType;
			}
			return RES_SUMMONERSKILL_TYPE.RES_SUMMONERSKILL_HITMONSTER;
		}

		[MethodMetaInfo("用其他技能中断当前正在施放的技能", "")]
		public EBTStatus AbortCurUseSkillByType(SkillAbortType abortType)
		{
			SkillSlot curUseSkillSlot = this.m_wrapper.actor.SkillControl.CurUseSkillSlot;
			if (curUseSkillSlot == null)
			{
				return EBTStatus.BT_SUCCESS;
			}
			if (curUseSkillSlot.ImmediateAbort(abortType))
			{
				return EBTStatus.BT_SUCCESS;
			}
			return EBTStatus.BT_FAILURE;
		}

		[MethodMetaInfo("检查技能是否能对该目标类型释放", "检查技能是否能对该目标类型释放")]
		public EBTStatus CheckSkillFilter(SkillSlotType InSlot, uint objID)
		{
			Skill skill = this.m_wrapper.GetSkill(InSlot);
			uint dwSkillTargetFilter = skill.cfgData.dwSkillTargetFilter;
			PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(objID);
			if (((ulong)dwSkillTargetFilter & (ulong)(1L << (int)(actor.handle.TheActorMeta.ActorType & (ActorTypeDef)31))) > 0uL)
			{
				return EBTStatus.BT_FAILURE;
			}
			return EBTStatus.BT_SUCCESS;
		}

		[MethodMetaInfo("是否能使用技能", "是否能使用指定的技能")]
		public EBTStatus CanUseSkill(SkillSlotType InSlot)
		{
			bool flag = this.m_wrapper.CanUseSkill(InSlot);
			if (flag)
			{
				return EBTStatus.BT_SUCCESS;
			}
			return EBTStatus.BT_FAILURE;
		}

		[MethodMetaInfo("技能使用是否完成", "技能使用是否完成")]
		public bool IsUseSkillCompleted()
		{
			Skill curUseSkill = this.m_wrapper.actor.SkillControl.CurUseSkill;
			return curUseSkill == null || curUseSkill.isFinish;
		}

		[MethodMetaInfo("是否打断普攻", "是否打断普攻")]
		public bool IsAbortNormalAttack()
		{
			SkillSlot curUseSkillSlot = this.m_wrapper.actor.SkillControl.CurUseSkillSlot;
			if (curUseSkillSlot == null)
			{
				return true;
			}
			SkillCache skillUseCache = this.m_wrapper.actor.SkillControl.SkillUseCache;
			if (skillUseCache != null)
			{
				if (skillUseCache.GetCommonAttackMode())
				{
					bool flag = curUseSkillSlot.IsAbort(SkillAbortType.TYPE_SKILL_0) && curUseSkillSlot.IsCDReady;
					if (flag)
					{
						curUseSkillSlot.Abort(SkillAbortType.TYPE_SKILL_0);
					}
					return flag;
				}
				if (this.m_wrapper.curMoveCommand != null)
				{
					return curUseSkillSlot.Abort(SkillAbortType.TYPE_MOVE);
				}
			}
			return false;
		}

		[MethodMetaInfo("当前技能是否能被打断", "前技能是否能被打断")]
		public bool IsUseSkillCompletedOrCanAbort()
		{
			Skill curUseSkill = this.m_wrapper.actor.SkillControl.CurUseSkill;
			if (curUseSkill == null)
			{
				return true;
			}
			if (curUseSkill.skillAbort.AbortWithAI())
			{
				this.m_wrapper.actor.SkillControl.ForceAbortCurUseSkill();
				return true;
			}
			return false;
		}

		[MethodMetaInfo("是否打断当前技能", "是否打断当前技能")]
		public bool IsAbortUseSkill()
		{
			SkillSlot curUseSkillSlot = this.m_wrapper.actor.SkillControl.CurUseSkillSlot;
			if (curUseSkillSlot == null)
			{
				return true;
			}
			SkillCache skillUseCache = this.m_wrapper.actor.SkillControl.SkillUseCache;
			SkillSlotType type;
			return (skillUseCache != null && skillUseCache.GetCacheSkillSlotType(out type) && curUseSkillSlot.IsAbort((SkillAbortType)type)) || (this.m_wrapper.curMoveCommand != null && curUseSkillSlot.IsAbort(SkillAbortType.TYPE_MOVE));
		}

		[MethodMetaInfo("使用普攻缓存", "使用普攻缓存")]
		public bool UseCommonAttackCache()
		{
			bool flag = false;
			SkillCache skillUseCache = this.m_wrapper.actor.SkillControl.SkillUseCache;
			if (skillUseCache != null)
			{
				flag = skillUseCache.IsCacheCommonAttack();
				if (flag)
				{
					skillUseCache.UseSkillCache(this.m_wrapper.actorPtr);
				}
			}
			return flag;
		}

		[MethodMetaInfo("是否高级模式普攻", "是否高级模式普攻")]
		public bool IsUseAdvanceCommonAttack()
		{
			return this.m_wrapper.IsUseAdvanceCommonAttack();
		}

		[MethodMetaInfo("高级模式下关闭普攻空放", "高级模式下关闭普攻空放")]
		public bool DisableSpecialCommonAttack()
		{
			if (this.m_wrapper.actor.SkillControl.SkillUseCache != null)
			{
				this.m_wrapper.actor.SkillControl.SkillUseCache.SetSpecialCommonAttack(false);
			}
			return true;
		}

		[MethodMetaInfo("高级模式下是否执行普攻空放", "高级模式下是否执行普攻空放")]
		public bool IsUseSpecialCommonAttack()
		{
			bool flag = this.m_wrapper.IsUseAdvanceCommonAttack();
			return (this.m_wrapper.actor.SkillControl != null && this.m_wrapper.actor.SkillControl.bImmediateAttack) || !flag || this.m_wrapper.actor.SkillControl.SkillUseCache == null || this.m_wrapper.actor.SkillControl.SkillUseCache.GetSpecialCommonAttack();
		}

		[MethodMetaInfo("高级模式下取消普攻模式", "高级模式下取消普攻模式")]
		public bool CancelCommonAttackMode()
		{
			return this.m_wrapper.CancelCommonAttackMode();
		}

		[MethodMetaInfo("获取当前技能Type", "获取当前分支对应的技能Type")]
		public SkillSlotType GetCurSkillSlotType()
		{
			return this.m_wrapper.curSkillUseInfo.SlotType;
		}

		[MethodMetaInfo("获取下次必放的技能", "获取下次必放的技能")]
		public SkillSlotType GetMustUseNextSkillSlotType()
		{
			return this.m_wrapper.m_nextMustUseSkill;
		}

		[MethodMetaInfo("重置下次必放的技能", "重置下次必放的技能")]
		public void ResetMustUseNextSkillSlotType()
		{
			this.m_wrapper.m_nextMustUseSkill = SkillSlotType.SLOT_SKILL_VALID;
		}

		[MethodMetaInfo("获取指定技能的搜索范围", "获取指定技能的搜索范围")]
		public int GetSkillSearchRange(SkillSlotType InSlot)
		{
			Skill skill = this.m_wrapper.GetSkill(InSlot);
			SkillSlot skillSlot = this.m_wrapper.GetSkillSlot(InSlot);
			if (skill != null && skill.cfgData != null)
			{
				return skill.GetMaxSearchDistance(skillSlot.GetSkillLevel());
			}
			return 0;
		}

		[MethodMetaInfo("获取指定技能的攻击范围", "获取指定技能的攻击范围")]
		public int GetSkillAttackRange(SkillSlotType InSlot)
		{
			Skill skill = this.m_wrapper.GetSkill(InSlot);
			if (skill != null && skill.cfgData != null)
			{
				return skill.cfgData.iMaxAttackDistance;
			}
			return 0;
		}

		[MethodMetaInfo("检测技能释放位置", "检测技能释放位置")]
		public bool CheckUseSkillPosition()
		{
			if (this.m_wrapper == null)
			{
				return false;
			}
			SkillUseParam curSkillUseInfo = this.m_wrapper.curSkillUseInfo;
			Skill skill = this.m_wrapper.GetSkill(curSkillUseInfo.SlotType);
			if (skill != null)
			{
				switch (curSkillUseInfo.AppointType)
				{
				case SkillRangeAppointType.Target:
				{
					if (!curSkillUseInfo.TargetActor || curSkillUseInfo.TargetActor.handle.ActorControl.IsDeadState)
					{
						return false;
					}
					long num = (long)(skill.cfgData.iMaxAttackDistance * skill.cfgData.iMaxAttackDistance);
					long num2 = (this.m_wrapper.actorLocation - curSkillUseInfo.TargetActor.handle.location).sqrMagnitudeLong2D - num;
					return (double)num2 <= (double)num * 0.09;
				}
				case SkillRangeAppointType.Pos:
				{
					long num = (long)(skill.cfgData.iMaxAttackDistance * skill.cfgData.iMaxAttackDistance);
					long num2 = (this.m_wrapper.actorLocation - curSkillUseInfo.UseVector).sqrMagnitudeLong2D - num;
					return (double)num2 <= (double)num * 0.09;
				}
				case SkillRangeAppointType.Directional:
					return true;
				}
			}
			return false;
		}

		[MethodMetaInfo("是否需要朝技能目标点移动", "是否需要朝技能目标点移动")]
		public virtual bool IsSkillMoveToTarget()
		{
			if (this.m_wrapper == null)
			{
				return false;
			}
			SkillUseParam curSkillUseInfo = this.m_wrapper.curSkillUseInfo;
			Skill nextSkill = this.m_wrapper.GetNextSkill(curSkillUseInfo.SlotType);
			if (nextSkill != null)
			{
				switch (curSkillUseInfo.AppointType)
				{
				case SkillRangeAppointType.Target:
				{
					if (!curSkillUseInfo.TargetActor || curSkillUseInfo.TargetActor.handle.ActorControl.IsDeadState)
					{
						return false;
					}
					long num = nextSkill.cfgData.iMaxAttackDistance;
					num += (long)curSkillUseInfo.TargetActor.handle.ActorControl.GetDetectedRadius();
					num *= num;
					return (this.m_wrapper.actorLocation - curSkillUseInfo.TargetActor.handle.location).sqrMagnitudeLong2D > num;
				}
				case SkillRangeAppointType.Pos:
				{
					long num2 = nextSkill.cfgData.iMaxAttackDistance * 11000L / 10000L;
					long num = num2 * num2;
					long num3 = (this.m_wrapper.actorLocation - curSkillUseInfo.UseVector).sqrMagnitudeLong2D - num;
					return num3 > 0L;
				}
				case SkillRangeAppointType.Directional:
					return false;
				}
			}
			return false;
		}

		[MethodMetaInfo("朝技能目标点移动", "朝技能目标点移动,前提是当前技能已经设置")]
		public virtual EBTStatus MoveToSkillTarget()
		{
			if (this.m_closeToTargetFrame > 0)
			{
				this.m_closeToTargetFrame--;
				return EBTStatus.BT_RUNNING;
			}
			this.m_closeToTargetFrame = 1;
			if (this.m_wrapper == null)
			{
				this.m_closeToTargetFrame = 0;
				return EBTStatus.BT_FAILURE;
			}
			SkillUseParam curSkillUseInfo = this.m_wrapper.curSkillUseInfo;
			Skill nextSkill = this.m_wrapper.GetNextSkill(curSkillUseInfo.SlotType);
			if (nextSkill != null)
			{
				switch (curSkillUseInfo.AppointType)
				{
				case SkillRangeAppointType.Target:
				{
					if (!curSkillUseInfo.TargetActor || curSkillUseInfo.TargetActor.handle.ActorControl.IsDeadState)
					{
						this.m_closeToTargetFrame = 0;
						return EBTStatus.BT_FAILURE;
					}
					long num = nextSkill.cfgData.iMaxAttackDistance;
					num += (long)curSkillUseInfo.TargetActor.handle.ActorControl.GetDetectedRadius();
					num *= num;
					if ((this.m_wrapper.actorLocation - curSkillUseInfo.TargetActor.handle.location).sqrMagnitudeLong2D > num)
					{
						this.m_wrapper.RealMovePosition(curSkillUseInfo.TargetActor.handle.location, 0u);
						return EBTStatus.BT_RUNNING;
					}
					this.m_closeToTargetFrame = 0;
					return EBTStatus.BT_SUCCESS;
				}
				case SkillRangeAppointType.Pos:
				{
					long num = (long)(nextSkill.cfgData.iMaxAttackDistance * nextSkill.cfgData.iMaxAttackDistance);
					if ((this.m_wrapper.actorLocation - curSkillUseInfo.UseVector).sqrMagnitudeLong2D > num)
					{
						this.m_wrapper.RealMovePosition(curSkillUseInfo.UseVector, 0u);
						return EBTStatus.BT_RUNNING;
					}
					this.m_closeToTargetFrame = 0;
					return EBTStatus.BT_SUCCESS;
				}
				case SkillRangeAppointType.Directional:
					this.m_closeToTargetFrame = 0;
					return EBTStatus.BT_SUCCESS;
				}
			}
			this.m_closeToTargetFrame = 0;
			return EBTStatus.BT_FAILURE;
		}

		[MethodMetaInfo("是否需要朝普攻目标移动", "攻击距离不够的情况下需要移动")]
		public virtual bool IsAttackMoveToTarget()
		{
			if (this.m_wrapper == null)
			{
				return false;
			}
			SkillUseParam curSkillUseInfo = this.m_wrapper.curSkillUseInfo;
			Skill nextSkill = this.m_wrapper.GetNextSkill(curSkillUseInfo.SlotType);
			if (nextSkill != null)
			{
				SkillRangeAppointType appointType = curSkillUseInfo.AppointType;
				if (appointType == SkillRangeAppointType.Target)
				{
					if (!curSkillUseInfo.TargetActor || curSkillUseInfo.TargetActor.handle.ActorAgent == null || curSkillUseInfo.TargetActor.handle.ActorControl.IsDeadState)
					{
						return false;
					}
					DebugHelper.Assert(nextSkill.cfgData != null, "skillObj.cfgData != null");
					DebugHelper.Assert(curSkillUseInfo.TargetActor.handle.shape != null, "skillContext.TargetActor.handle.shape!=null");
					if (nextSkill.cfgData == null || curSkillUseInfo.TargetActor.handle.shape == null)
					{
						return false;
					}
					long num = nextSkill.cfgData.iMaxAttackDistance;
					num += (long)curSkillUseInfo.TargetActor.handle.ActorControl.GetDetectedRadius();
					num *= num;
					return (this.m_wrapper.actorLocation - curSkillUseInfo.TargetActor.handle.location).sqrMagnitudeLong2D > num;
				}
			}
			return false;
		}

		[MethodMetaInfo("高级模式下普通攻击寻敌", "高级模式下普通攻击寻敌")]
		public virtual EBTStatus MoveToCommonAttackTargetWithRange(int range)
		{
			if (this.m_wrapper == null)
			{
				return EBTStatus.BT_FAILURE;
			}
			SkillUseParam curSkillUseInfo = this.m_wrapper.curSkillUseInfo;
			Skill nextSkill = this.m_wrapper.GetNextSkill(curSkillUseInfo.SlotType);
			if (nextSkill != null)
			{
				SkillRangeAppointType appointType = curSkillUseInfo.AppointType;
				if (appointType == SkillRangeAppointType.Target)
				{
					if (this.m_wrapper.actor.SkillControl.SkillUseCache.IsExistNewAttackCommand())
					{
						this.m_wrapper.actor.SkillControl.SkillUseCache.SetMoveToAttackTarget(false);
						return EBTStatus.BT_FAILURE;
					}
					if (!curSkillUseInfo.TargetActor || curSkillUseInfo.TargetActor.handle.ActorControl.IsDeadState || !curSkillUseInfo.TargetActor.handle.HorizonMarker.IsVisibleFor(this.m_wrapper.actor.TheActorMeta.ActorCamp))
					{
						this.m_wrapper.TerminateMove();
						this.m_wrapper.ClearTarget();
						this.m_wrapper.actor.SkillControl.SkillUseCache.SetMoveToAttackTarget(false);
						return EBTStatus.BT_FAILURE;
					}
					Player ownerPlayer = ActorHelper.GetOwnerPlayer(ref this.m_wrapper.actorPtr);
					HeroWrapper heroWrapper = this.m_wrapper as HeroWrapper;
					if (ownerPlayer != null && heroWrapper != null)
					{
						if (heroWrapper.CurOpMode == OperateMode.LockMode || ownerPlayer.useLastHitMode == LastHitMode.LastHit || ownerPlayer.curAttackOrganMode == AttackOrganMode.AttackOrgan)
						{
							range = heroWrapper.ChaseRange;
						}
						else
						{
							range = heroWrapper.SearchRange;
						}
					}
					range += curSkillUseInfo.TargetActor.handle.ActorControl.GetDetectedRadius();
					long num = (long)range * (long)range;
					BaseAttackMode currentAttackMode = this.m_wrapper.GetCurrentAttackMode();
					if ((currentAttackMode == null || currentAttackMode.GetEnemyHeroAttackTargetID() <= 0u) && (this.m_wrapper.actorLocation - curSkillUseInfo.TargetActor.handle.location).sqrMagnitudeLong2D > num)
					{
						this.m_wrapper.TerminateMove();
						this.m_wrapper.ClearTarget();
						this.m_wrapper.actor.SkillControl.SkillUseCache.SetMoveToAttackTarget(false);
						return EBTStatus.BT_FAILURE;
					}
					long num2 = nextSkill.cfgData.iMaxAttackDistance;
					num2 += (long)curSkillUseInfo.TargetActor.handle.ActorControl.GetDetectedRadius();
					num2 *= num2;
					if ((this.m_wrapper.actorLocation - curSkillUseInfo.TargetActor.handle.location).sqrMagnitudeLong2D > num2)
					{
						this.m_wrapper.RealMovePosition(curSkillUseInfo.TargetActor.handle.location, 0u);
						this.m_wrapper.actor.SkillControl.SkillUseCache.SetMoveToAttackTarget(true);
						return EBTStatus.BT_RUNNING;
					}
					this.m_wrapper.actor.SkillControl.SkillUseCache.SetMoveToAttackTarget(false);
					return EBTStatus.BT_SUCCESS;
				}
			}
			this.m_wrapper.ClearTarget();
			this.m_wrapper.actor.SkillControl.SkillUseCache.SetMoveToAttackTarget(false);
			return EBTStatus.BT_FAILURE;
		}

		[MethodMetaInfo("朝技能目标点移动,大于指定的范围则丢失目标", "朝技能目标点移动,大于指定的范围则丢失目标,前提是当前技能已经设置")]
		public virtual EBTStatus MoveToSkillTargetWithRange(int range)
		{
			if (this.m_closeToTargetFrame > 0)
			{
				this.m_closeToTargetFrame--;
				return EBTStatus.BT_RUNNING;
			}
			this.m_closeToTargetFrame = 1;
			if (this.m_wrapper == null)
			{
				this.m_closeToTargetFrame = 0;
				return EBTStatus.BT_FAILURE;
			}
			SkillUseParam curSkillUseInfo = this.m_wrapper.curSkillUseInfo;
			Skill nextSkill = this.m_wrapper.GetNextSkill(curSkillUseInfo.SlotType);
			if (nextSkill != null)
			{
				SkillRangeAppointType appointType = curSkillUseInfo.AppointType;
				if (appointType != SkillRangeAppointType.Target)
				{
					if (appointType == SkillRangeAppointType.Pos)
					{
						long num = (long)(nextSkill.cfgData.iMaxAttackDistance * nextSkill.cfgData.iMaxAttackDistance);
						if ((this.m_wrapper.actorLocation - curSkillUseInfo.UseVector).sqrMagnitudeLong2D > num)
						{
							this.m_wrapper.RealMovePosition(curSkillUseInfo.UseVector, 0u);
							return EBTStatus.BT_RUNNING;
						}
						this.m_closeToTargetFrame = 0;
						return EBTStatus.BT_SUCCESS;
					}
				}
				else
				{
					if (!curSkillUseInfo.TargetActor || curSkillUseInfo.TargetActor.handle.ActorControl.IsDeadState || !curSkillUseInfo.TargetActor.handle.HorizonMarker.IsVisibleFor(this.m_wrapper.actor.TheActorMeta.ActorCamp))
					{
						this.m_closeToTargetFrame = 0;
						this.m_wrapper.TerminateMove();
						this.m_wrapper.ClearTarget();
						return EBTStatus.BT_FAILURE;
					}
					long num2 = (long)range * (long)range;
					if ((this.m_wrapper.actorLocation - curSkillUseInfo.TargetActor.handle.location).sqrMagnitudeLong2D > num2)
					{
						this.m_closeToTargetFrame = 0;
						this.m_wrapper.TerminateMove();
						this.m_wrapper.ClearTarget();
						return EBTStatus.BT_FAILURE;
					}
					long num = nextSkill.cfgData.iMaxAttackDistance;
					num += (long)curSkillUseInfo.TargetActor.handle.ActorControl.GetDetectedRadius();
					num *= num;
					if ((this.m_wrapper.actorLocation - curSkillUseInfo.TargetActor.handle.location).sqrMagnitudeLong2D > num)
					{
						this.m_wrapper.RealMovePosition(curSkillUseInfo.TargetActor.handle.location, 0u);
						return EBTStatus.BT_RUNNING;
					}
					this.m_closeToTargetFrame = 0;
					return EBTStatus.BT_SUCCESS;
				}
			}
			this.m_closeToTargetFrame = 0;
			this.m_wrapper.ClearTarget();
			return EBTStatus.BT_FAILURE;
		}

		[MethodMetaInfo("对自己使用回血技能", "对自己使用技能,会自动判断该技能是否准备好")]
		public void UseHpRecoverSkillToSelf()
		{
			this.m_wrapper.UseHpRecoverSkillToSelf();
		}

		[MethodMetaInfo("使用回城", "使用回城")]
		public void UseGoHomeSkill()
		{
			this.m_wrapper.UseGoHomeSkill();
		}

		[MethodMetaInfo("死亡后是否可控制", "死亡控制")]
		public bool bActorDeadControl()
		{
			return this.m_wrapper.GetNoAbilityFlag(ObjAbilityType.ObjAbility_DeadControl);
		}

		[MethodMetaInfo("角色死亡状态", "死亡状态")]
		public ObjDeadMode GetActorDeadState()
		{
			return this.m_wrapper.deadMode;
		}

		[MethodMetaInfo("高级模式下普攻搜索敌人", "高级模式下普攻搜索敌人")]
		public uint AdvanceCommonAttackSearchEnemy(int srchR)
		{
			uint result = 0u;
			BaseAttackMode currentAttackMode = this.m_wrapper.GetCurrentAttackMode();
			if (currentAttackMode != null)
			{
				result = currentAttackMode.CommonAttackSearchEnemy(srchR);
			}
			return result;
		}

		[MethodMetaInfo("普攻选择范围内的敌人", "普攻选择范围内的敌人")]
		public uint NormalAttackSearchEnemy(int srchR)
		{
			Player ownerPlayer = ActorHelper.GetOwnerPlayer(ref this.m_wrapper.actorPtr);
			if (ownerPlayer != null && this.m_wrapper.myTarget && ownerPlayer.bCommonAttackLockMode)
			{
				if (this.m_wrapper.IsTargetObjInSearchDistance())
				{
					return this.m_wrapper.myTarget.handle.ObjID;
				}
				this.m_wrapper.ClearTarget();
			}
			SelectEnemyType selectEnemyType;
			if (ownerPlayer == null)
			{
				selectEnemyType = SelectEnemyType.SelectLowHp;
			}
			else
			{
				selectEnemyType = ownerPlayer.AttackTargetMode;
			}
			if (selectEnemyType == SelectEnemyType.SelectLowHp)
			{
				return Singleton<CommonAttackSearcher>.GetInstance().CommonAttackSearchLowestHpTarget(this.m_wrapper, srchR);
			}
			return Singleton<CommonAttackSearcher>.GetInstance().CommonAttackSearchNearestTarget(this.m_wrapper, srchR);
		}

		[MethodMetaInfo("选择范围内的敌人", "选择范围内的敌人")]
		public uint GetNearestEnemy(int srchR)
		{
			ActorRoot nearestEnemy = Singleton<TargetSearcher>.GetInstance().GetNearestEnemy(this.m_wrapper.actor, srchR, 0u, true);
			if (nearestEnemy != null)
			{
				return nearestEnemy.ObjID;
			}
			return 0u;
		}

		[MethodMetaInfo("选择最近的敌人，带筛选", "选择最近的敌人，带筛选")]
		public uint GetNearestEnemyWithFilter(int srchR, uint filter)
		{
			ActorRoot nearestEnemy = Singleton<TargetSearcher>.GetInstance().GetNearestEnemy(this.m_wrapper.actor, srchR, filter, true);
			if (nearestEnemy != null)
			{
				return nearestEnemy.ObjID;
			}
			return 0u;
		}

		[MethodMetaInfo("选择范围内最近的敌人", "TargetPriority表示敌人的类型")]
		public uint GetNearestEnemyWithTargetPriority(int srchR, TargetPriority priotity)
		{
			ActorRoot nearestEnemy = Singleton<TargetSearcher>.GetInstance().GetNearestEnemy(this.m_wrapper.actor, srchR, priotity, 0u, true);
			if (nearestEnemy != null)
			{
				return nearestEnemy.ObjID;
			}
			return 0u;
		}

		[MethodMetaInfo("选择范围内最近的敌人，带优先级，排除野怪", "TargetPriority表示敌人的类型")]
		public uint GetNearestEnemyWithPriorityWithoutJungleMonster(int srchR, TargetPriority priotity)
		{
			ActorRoot nearestEnemyWithPriorityWithoutJungleMonster = Singleton<TargetSearcher>.GetInstance().GetNearestEnemyWithPriorityWithoutJungleMonster(this.m_wrapper.actor, srchR, priotity);
			if (nearestEnemyWithPriorityWithoutJungleMonster != null)
			{
				return nearestEnemyWithPriorityWithoutJungleMonster.ObjID;
			}
			return 0u;
		}

		[MethodMetaInfo("选择范围内的敌人,优先小兵,然后英雄", "选择范围内的敌人,优先小兵,然后英雄")]
		public uint GetNearestEnemyDogfaceFirst(int srchR)
		{
			ActorRoot nearestEnemyDogfaceFirst = Singleton<TargetSearcher>.GetInstance().GetNearestEnemyDogfaceFirst(this.m_wrapper.actor, srchR);
			if (nearestEnemyDogfaceFirst != null)
			{
				return nearestEnemyDogfaceFirst.ObjID;
			}
			return 0u;
		}

		[MethodMetaInfo("选择范围内的敌人,优先小兵,超级兵》近战兵》远程兵,然后英雄", "选择范围内的敌人,优先小兵,超级兵》近战兵》远程兵,然后英雄")]
		public uint GetNearestEnemyDogfaceFirstAndDogfaceHasPriority(int srchR)
		{
			ActorRoot nearestEnemyDogfaceFirstAndDogfaceHasPriority = Singleton<TargetSearcher>.GetInstance().GetNearestEnemyDogfaceFirstAndDogfaceHasPriority(this.m_wrapper.actor, srchR);
			if (nearestEnemyDogfaceFirstAndDogfaceHasPriority != null)
			{
				return nearestEnemyDogfaceFirstAndDogfaceHasPriority.ObjID;
			}
			return 0u;
		}

		[MethodMetaInfo("选择范围内的敌人,不包括没有处于战斗状态的野怪", "选择范围内的敌人,不包括没有处于战斗状态的野怪")]
		public uint GetNearestEnemyWithoutNotInBattleJungleMonster(int srchR)
		{
			ActorRoot nearestEnemyWithoutNotInBattleJungleMonster = Singleton<TargetSearcher>.GetInstance().GetNearestEnemyWithoutNotInBattleJungleMonster(this.m_wrapper.actorPtr, srchR);
			if (nearestEnemyWithoutNotInBattleJungleMonster != null)
			{
				return nearestEnemyWithoutNotInBattleJungleMonster.ObjID;
			}
			return 0u;
		}

		[MethodMetaInfo("选择范围内的敌人,带优先级，不包括没有处于战斗状态的野怪", "选择范围内的敌人,带优先级,不包括没有处于战斗状态的野怪")]
		public uint GetNearestEnemyWithPriorityWithoutNotInBattleJungleMonster(int srchR, TargetPriority priotity)
		{
			ActorRoot nearestEnemyWithPriorityWithoutNotInBattleJungleMonster = Singleton<TargetSearcher>.GetInstance().GetNearestEnemyWithPriorityWithoutNotInBattleJungleMonster(this.m_wrapper.actorPtr, srchR, priotity);
			if (nearestEnemyWithPriorityWithoutNotInBattleJungleMonster != null)
			{
				return nearestEnemyWithPriorityWithoutNotInBattleJungleMonster.ObjID;
			}
			return 0u;
		}

		[MethodMetaInfo("选择范围内的敌人,带优先级，不包含指定Actor且不包括没有处于战斗状态的野怪,,", "选择范围内的敌人,带优先级,不包含指定Actor且不包括没有处于战斗状态的野怪")]
		public uint GetNearestEnemyWithPriorityWithoutNotInBattleJungleMonsterWithoutActor(int srchR, TargetPriority priotity, uint withOutActor)
		{
			ActorRoot nearestEnemyWithPriorityWithoutNotInBattleJungleMonsterWithoutActor = Singleton<TargetSearcher>.GetInstance().GetNearestEnemyWithPriorityWithoutNotInBattleJungleMonsterWithoutActor(this.m_wrapper.actorPtr, srchR, priotity, withOutActor);
			if (nearestEnemyWithPriorityWithoutNotInBattleJungleMonsterWithoutActor != null)
			{
				return nearestEnemyWithPriorityWithoutNotInBattleJungleMonsterWithoutActor.ObjID;
			}
			return 0u;
		}

		[MethodMetaInfo("选择范围内的敌人,不包含指定Actor且不包括没有处于战斗状态的野怪", "选择范围内的敌人,不包括没有处于战斗状态的野怪且不包含指定Actor")]
		public uint GetNearestEnemyWithoutNotInBattleJungleMonsterWithoutActor(int srchR, uint withOutActor)
		{
			ActorRoot nearestEnemyWithoutNotInBattleJungleMonsterWithoutActor = Singleton<TargetSearcher>.GetInstance().GetNearestEnemyWithoutNotInBattleJungleMonsterWithoutActor(this.m_wrapper.actorPtr, srchR, withOutActor);
			if (nearestEnemyWithoutNotInBattleJungleMonsterWithoutActor != null)
			{
				return nearestEnemyWithoutNotInBattleJungleMonsterWithoutActor.ObjID;
			}
			return 0u;
		}

		[MethodMetaInfo("选择范围内的敌人,不包含指定Actor且不包括野怪", "选择范围内的敌人,不包括野怪且不包含指定Actor")]
		public uint GetNearestEnemyWithoutJungleMonsterWithoutActor(int srchR, uint withOutActor)
		{
			ActorRoot nearestEnemyWithoutJungleMonsterWithoutActor = Singleton<TargetSearcher>.GetInstance().GetNearestEnemyWithoutJungleMonsterWithoutActor(this.m_wrapper.actorPtr, srchR, withOutActor);
			if (nearestEnemyWithoutJungleMonsterWithoutActor != null)
			{
				return nearestEnemyWithoutJungleMonsterWithoutActor.ObjID;
			}
			return 0u;
		}

		[MethodMetaInfo("选择范围内的敌人,不包含指定Actor且不包括野怪和伪装状态傀儡", "选择范围内的敌人,不包括野怪且不包含指定Actor和伪装状态傀儡")]
		public uint GetNearestEnemyWithoutJungleMonsterAndCallActorWithoutActor(int srchR, uint withOutActor)
		{
			ActorRoot nearestEnemyWithoutJungleMonsterAndCallActorWithoutActor = Singleton<TargetSearcher>.GetInstance().GetNearestEnemyWithoutJungleMonsterAndCallActorWithoutActor(this.m_wrapper.actorPtr, srchR, withOutActor);
			if (nearestEnemyWithoutJungleMonsterAndCallActorWithoutActor != null)
			{
				return nearestEnemyWithoutJungleMonsterAndCallActorWithoutActor.ObjID;
			}
			return 0u;
		}

		[MethodMetaInfo("选择范围内的敌人,忽略视野", "选择范围内的敌人,忽略视野，包括野怪")]
		public uint GetNearestEnemyIgnoreVisible(int srchR)
		{
			ActorRoot nearestEnemyIgnoreVisible = Singleton<TargetSearcher>.GetInstance().GetNearestEnemyIgnoreVisible(this.m_wrapper.actorPtr, srchR, 0u);
			if (nearestEnemyIgnoreVisible != null)
			{
				return nearestEnemyIgnoreVisible.ObjID;
			}
			return 0u;
		}

		[MethodMetaInfo("获取低血量的队友", "HPRate是比例,10000表示1; InSlot技能槽位,用于过滤")]
		public uint GetLowHpTeamMember(int srchR, int HPRate, SkillSlotType InSlot)
		{
			uint filter = 0u;
			Skill skill = this.m_wrapper.GetSkill(InSlot);
			if (skill != null && skill.cfgData != null)
			{
				filter = skill.cfgData.dwSkillTargetFilter;
			}
			ActorRoot lowHpTeamMember = Singleton<TargetSearcher>.GetInstance().GetLowHpTeamMember(this.m_wrapper.actorPtr, srchR, HPRate, filter);
			if (lowHpTeamMember != null)
			{
				return lowHpTeamMember.ObjID;
			}
			return 0u;
		}

		[MethodMetaInfo("清空选择的目标", "即没有目标")]
		public void ClearTarget()
		{
			this.m_wrapper.ClearTarget();
		}

		[MethodMetaInfo("设定目标", "")]
		public void SelectTarget(uint objID)
		{
			PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(objID);
			this.m_wrapper.SelectTarget(actor);
		}

		[MethodMetaInfo("设定技能", "")]
		public EBTStatus SetSkill(SkillSlotType InSlot)
		{
			bool flag = this.m_wrapper.SetSkill(InSlot, false);
			if (flag)
			{
				return EBTStatus.BT_SUCCESS;
			}
			return EBTStatus.BT_FAILURE;
		}

		[MethodMetaInfo("设定技能特殊释放", "")]
		public void SetSkillSpecial(SkillSlotType InSlot)
		{
			this.m_wrapper.SetSkill(InSlot, true);
		}

		[MethodMetaInfo("指定技能是否准备好", "")]
		public bool IsSkillCDReady(SkillSlotType InSlot)
		{
			SkillSlot skillSlot = null;
			return this.m_wrapper.actor.SkillControl.TryGetSkillSlot(InSlot, out skillSlot) && (skillSlot.SkillObj != null && skillSlot.SkillObj.cfgData != null && skillSlot.IsCDReady);
		}

		[MethodMetaInfo("是否找到普攻目标", "是否找到普攻目标")]
		public bool IsSearchCommonAttackTarget(uint objID)
		{
			if (this.m_wrapper.actor.SkillControl != null && this.m_wrapper.actor.SkillControl.bImmediateAttack)
			{
				return false;
			}
			if (objID <= 0u)
			{
				this.m_wrapper.ClearTarget();
				return false;
			}
			PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(objID);
			if (!actor)
			{
				this.m_wrapper.ClearTarget();
				return false;
			}
			return true;
		}

		[MethodMetaInfo("判断目标是否可被攻击", "判断目标是否可被攻击,可被攻击的前提是活的,不是无敌的,不是一个阵营的")]
		public bool IsTargetCanBeAttacked(uint objID)
		{
			PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(objID);
			if (!actor)
			{
				this.m_wrapper.ClearTarget();
				return false;
			}
			bool flag = this.m_wrapper.CanAttack(actor) && actor.handle.HorizonMarker.IsVisibleFor(this.m_wrapper.actor.TheActorMeta.ActorCamp);
			if (!flag)
			{
				this.m_wrapper.ClearTarget();
			}
			return flag;
		}

		[MethodMetaInfo("判断目标是否可见", "判断目标是否可见")]
		public EBTStatus IsVisible(uint objID)
		{
			PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(objID);
			if (!actor)
			{
				return EBTStatus.BT_FAILURE;
			}
			if (actor.handle.HorizonMarker.IsVisibleFor(this.m_wrapper.actor.TheActorMeta.ActorCamp))
			{
				return EBTStatus.BT_SUCCESS;
			}
			return EBTStatus.BT_FAILURE;
		}

		[MethodMetaInfo("判断目标是否可被攻击,忽略是否可见", "判断目标是否可被攻击,可被攻击的前提是活的,不是无敌的,不是一个阵营的")]
		public bool IsTargetCanBeAttackedIgnoreVisible(uint objID)
		{
			PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(objID);
			if (!actor)
			{
				this.m_wrapper.ClearTarget();
				this.m_wrapper.CancelCommonAttackMode();
				return false;
			}
			bool flag = this.m_wrapper.CanAttack(actor);
			if (!flag)
			{
				this.m_wrapper.ClearTarget();
				this.m_wrapper.CancelCommonAttackMode();
			}
			return flag;
		}

		[MethodMetaInfo("是否需要帮助他人攻击", "是否需要帮助他人攻击")]
		public bool IsNeedToHelpOther()
		{
			return this.m_wrapper.IsNeedToHelpOther && this.m_wrapper.m_needToHelpTarget && this.m_wrapper.m_needToHelpTarget.handle.ObjID != 0u;
		}

		[MethodMetaInfo("获取需要帮助的角色ID", "获取需要帮助的角色ID")]
		public uint GetNeedHelpTarget()
		{
			if (this.m_wrapper.m_needToHelpTarget)
			{
				return this.m_wrapper.m_needToHelpTarget.handle.ObjID;
			}
			return 0u;
		}

		[MethodMetaInfo("清除需要帮助他人的标记", "清除需要帮助他人的标记")]
		public void ClearHelpOther()
		{
			this.m_wrapper.ClearNeedToHelpOther();
		}

		[MethodMetaInfo("帮助他人攻击", "切换由他人传过的攻击目标")]
		public void HelpToAttack()
		{
			this.m_wrapper.HelpToAttack();
		}

		[MethodMetaInfo("是否需要切换目标", "切换由他人传过的攻击目标")]
		public bool IsNeedToSwitchTarget()
		{
			return this.m_wrapper.IsNeedSwitchTarget;
		}

		[MethodMetaInfo("切换目标", "切换由他人传过的攻击目标")]
		public void SwitchTarget()
		{
			this.m_wrapper.SwitchTarget();
		}

		[MethodMetaInfo("是否忽略搜索范围", "是否忽略搜索范围")]
		public bool IsIgnoreSrchRange()
		{
			return this.m_wrapper.m_isAttackEnemyIgnoreSrchrange;
		}

		[MethodMetaInfo("设置是否忽略搜索范围", "设置是否忽略搜索范围")]
		public void SetIgnoreSrchrange(bool ignore)
		{
			this.m_wrapper.m_isAttackEnemyIgnoreSrchrange = ignore;
		}

		[MethodMetaInfo("切换目标为攻击自己的敌人", "切换目标为攻击自己的敌人")]
		public void ChangeTargetToAtker()
		{
			this.m_wrapper.myTarget = this.m_wrapper.myLastAtker;
		}

		[MethodMetaInfo("切换目标为嘲讽自己的敌人", "切换目标为嘲讽自己的敌人")]
		public void SetTauntMeActorAsMyTarget()
		{
			this.m_wrapper.myTarget = this.m_wrapper.m_tauntMeActor;
		}

		[MethodMetaInfo("通知系统进入战斗", "通知进入战斗")]
		public void NotifyEventSysEnterCombat()
		{
			this.m_wrapper.SetSelfInBattle();
		}

		[MethodMetaInfo("通知系统脱离战斗", "通知脱离战斗")]
		public void NotifyEventSysExitCombat()
		{
			this.m_wrapper.SetSelfExitBattle();
		}

		[MethodMetaInfo("播放英雄动作声音", "播放英雄动作声音")]
		public void PlayHeroActSound(EActType actType)
		{
			if (this.m_sound_Interval < 18000)
			{
				return;
			}
			string soundName = null;
			for (int i = 0; i < this.m_wrapper.actor.CharInfo.ActSounds.Length; i++)
			{
				if (this.m_wrapper.actor.CharInfo.ActSounds[i].SoundActType == actType)
				{
					soundName = this.m_wrapper.actor.CharInfo.ActSounds[i].ActSoundName;
					break;
				}
			}
			uint num = Singleton<CSoundManager>.GetInstance().PlayHeroActSound(soundName);
			if (num != 0u)
			{
				this.m_sound_Interval = 0;
			}
		}

		[MethodMetaInfo("播放剧情对话", "播放剧情对话")]
		public void PlayDialogue(int groupId)
		{
			if (groupId > 0)
			{
				MonoSingleton<DialogueProcessor>.GetInstance().StartDialogue(groupId);
			}
		}

		[MethodMetaInfo("是不是网络对战", "是不是网络对战")]
		public EBTStatus IsPlayOnNetwork()
		{
			if (Singleton<FrameSynchr>.GetInstance().bActive)
			{
				return EBTStatus.BT_SUCCESS;
			}
			return EBTStatus.BT_FAILURE;
		}

		[MethodMetaInfo("获取该关卡最大英雄个数", "是不是网络对战")]
		public int GetPvPLevelMaxHeroNum()
		{
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			if (curLvelContext != null && curLvelContext.IsMobaModeWithOutGuide())
			{
				return curLvelContext.m_pvpPlayerNum;
			}
			return 0;
		}

		[MethodMetaInfo("是不是排位赛", "是不是排位赛")]
		public EBTStatus IsLadder()
		{
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			if (curLvelContext.IsGameTypeLadder())
			{
				return EBTStatus.BT_SUCCESS;
			}
			return EBTStatus.BT_FAILURE;
		}

		[MethodMetaInfo("是不是温暖局", "是不是温暖局")]
		public EBTStatus IsWarmBattle()
		{
			SLevelContext curLvelContext = Singleton<BattleLogic>.GetInstance().GetCurLvelContext();
			if (curLvelContext.m_isWarmBattle)
			{
				return EBTStatus.BT_SUCCESS;
			}
			return EBTStatus.BT_FAILURE;
		}

		[MethodMetaInfo("当前局运行时间", "当前局运行时间毫秒")]
		public ulong BattleTime()
		{
			return Singleton<FrameSynchr>.instance.LogicFrameTick;
		}

		[MethodMetaInfo("获取等待帧数", "不是网络对战要乘2")]
		public int GetWaitFrame(int frame)
		{
			int num = frame;
			if (!Singleton<FrameSynchr>.GetInstance().bActive)
			{
				num *= 2;
			}
			if (this.m_wrapper.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
			{
				if (this.m_wrapper.myBehavior == ObjBehaviMode.State_AutoAI)
				{
					num = (num - 1) / 3 + 1;
				}
			}
			else
			{
				MonsterWrapper monsterWrapper = this.m_wrapper.actor.AsMonster();
				if (monsterWrapper != null && monsterWrapper.cfgInfo != null && monsterWrapper.cfgInfo.bIsBoss > 0)
				{
					num = (num - 1) / 4 + 1;
				}
				else if (this.m_wrapper.actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ)
				{
					num = (num - 1) / 5 + 1;
				}
				else
				{
					num = (num - 1) / 6 + 1;
				}
			}
			return num;
		}

		[MethodMetaInfo("通知团队攻击敌人", "敌人可以任何非己方团队成员")]
		public void NotifyTeamAttackEnemy(uint enemyObjId, int range, bool ignoreSrchRange)
		{
			this.m_wrapper.CommanderNotifyToAttack(enemyObjId, range, ignoreSrchRange);
		}

		[MethodMetaInfo("通知团队选择一条路径", "id表示以id为中心")]
		public void NotifyTeamSelectRoute(int index, int range, uint centerObjId)
		{
			ListView<WaypointsHolder> waypointsList = Singleton<BattleLogic>.GetInstance().mapLogic.GetWaypointsList(this.m_wrapper.actor.TheActorMeta.ActorCamp);
			if (waypointsList == null || waypointsList.Count == 0)
			{
				return;
			}
			if (index < 0)
			{
				return;
			}
			List<PoolObjHandle<ActorRoot>> heroActors = Singleton<GameObjMgr>.GetInstance().HeroActors;
			for (int i = 0; i < waypointsList.Count; i++)
			{
				if (!(waypointsList[i] == null))
				{
					if (waypointsList[i].m_index == index)
					{
						long num = (long)range;
						num *= num;
						PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.instance.GetActor(centerObjId);
						for (int j = 0; j < heroActors.Count; j++)
						{
							PoolObjHandle<ActorRoot> poolObjHandle = heroActors[j];
							if (poolObjHandle.handle.TheActorMeta.ActorCamp == this.m_wrapper.actor.TheActorMeta.ActorCamp)
							{
								long sqrMagnitudeLong2D = (poolObjHandle.handle.location - actor.handle.location).sqrMagnitudeLong2D;
								if (sqrMagnitudeLong2D < num)
								{
									poolObjHandle.handle.ActorControl.m_curWaypointsHolder = waypointsList[i];
									poolObjHandle.handle.ActorControl.m_curWaypointTarget = poolObjHandle.handle.ActorControl.m_curWaypointsHolder.startPoint;
									poolObjHandle.handle.ActorControl.m_curWaypointTargetPosition = new VInt3(poolObjHandle.handle.ActorControl.m_curWaypointTarget.transform.position);
								}
							}
						}
						break;
					}
				}
			}
		}

		[MethodMetaInfo("通知团队选择一条随机路径", "id表示以id为中心")]
		public void NotifyTeamSelectRandomRoute(int range, uint centerObjId)
		{
			List<PoolObjHandle<ActorRoot>> heroActors = Singleton<GameObjMgr>.GetInstance().HeroActors;
			long num = (long)range;
			num *= num;
			PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.instance.GetActor(centerObjId);
			for (int i = 0; i < heroActors.Count; i++)
			{
				PoolObjHandle<ActorRoot> poolObjHandle = heroActors[i];
				if (poolObjHandle.handle.TheActorMeta.ActorCamp == this.m_wrapper.actor.TheActorMeta.ActorCamp)
				{
					long sqrMagnitudeLong2D = (poolObjHandle.handle.location - actor.handle.location).sqrMagnitudeLong2D;
					if (sqrMagnitudeLong2D < num)
					{
						poolObjHandle.handle.ActorAgent.SelectRoute();
					}
				}
			}
		}

		[MethodMetaInfo("获取小龙(新的接口)", "如果死亡或是不存在为0")]
		public uint GetSmallDragon()
		{
			int count = Singleton<GameObjMgr>.instance.GameActors.Count;
			for (int i = 0; i < count; i++)
			{
				PoolObjHandle<ActorRoot> ptr = Singleton<GameObjMgr>.instance.GameActors[i];
				if (ptr && !ptr.handle.ActorControl.IsDeadState)
				{
					MonsterWrapper monsterWrapper = ptr.handle.ActorControl as MonsterWrapper;
					if (monsterWrapper != null && monsterWrapper.cfgInfo.bSoldierType == 9)
					{
						return ptr.handle.ObjID;
					}
				}
			}
			return 0u;
		}

		[MethodMetaInfo("获取大龙", "如果死亡或是不存在为0")]
		public uint GetBigDragon()
		{
			int count = Singleton<GameObjMgr>.instance.GameActors.Count;
			for (int i = 0; i < count; i++)
			{
				PoolObjHandle<ActorRoot> ptr = Singleton<GameObjMgr>.instance.GameActors[i];
				if (ptr && !ptr.handle.ActorControl.IsDeadState)
				{
					MonsterWrapper monsterWrapper = ptr.handle.ActorControl as MonsterWrapper;
					if (monsterWrapper != null && monsterWrapper.cfgInfo.bSoldierType == 8)
					{
						return ptr.handle.ObjID;
					}
				}
			}
			return 0u;
		}

		[MethodMetaInfo("获取指挥官要攻击的英雄", "range表示以该敌人为中心的半径")]
		public uint GetCommandAttackHero(int range)
		{
			PoolObjHandle<ActorRoot> commanderHeroTarget = Singleton<TargetSearcher>.instance.GetCommanderHeroTarget(this.m_wrapper.actor.TheActorMeta.ActorCamp, range);
			if (commanderHeroTarget)
			{
				return commanderHeroTarget.handle.ObjID;
			}
			return 0u;
		}

		[MethodMetaInfo("获取己方活着的英雄数量", "获取己方活着的英雄数量")]
		public int GetSelfCampAliveHerpNum()
		{
			return Singleton<TargetSearcher>.instance.GetAliveHeroNum(this.m_wrapper.actor.TheActorMeta.ActorCamp);
		}

		[MethodMetaInfo("获取敌方活着的英雄数量", "获取敌方活着的英雄数量")]
		public int GetEnemyCampAliveHerpNum()
		{
			COM_PLAYERCAMP camp = COM_PLAYERCAMP.COM_PLAYERCAMP_MID;
			if (this.m_wrapper.actor.TheActorMeta.ActorCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_1)
			{
				camp = COM_PLAYERCAMP.COM_PLAYERCAMP_2;
			}
			else if (this.m_wrapper.actor.TheActorMeta.ActorCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_2)
			{
				camp = COM_PLAYERCAMP.COM_PLAYERCAMP_1;
			}
			return Singleton<TargetSearcher>.instance.GetAliveHeroNum(camp);
		}

		[MethodMetaInfo("获取指定角色周围的英雄数量", "获取指定角色周围的英雄数量")]
		public int GetHeroNumInRange(uint objID, int range, COM_PLAYERCAMP camp)
		{
			PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(objID);
			if (actor)
			{
				return Singleton<TargetSearcher>.instance.GetHeroNumInRange(actor.handle, range, camp);
			}
			return 0;
		}

		[MethodMetaInfo("获取指挥官要攻击的塔", "range表示以该塔为中心的半径")]
		public uint GetCommandAttackOrgan(int friendrange, int enemyRange)
		{
			PoolObjHandle<ActorRoot> commanderTowerTarget = Singleton<TargetSearcher>.instance.GetCommanderTowerTarget(this.m_wrapper.actor.TheActorMeta.ActorCamp, friendrange, enemyRange);
			if (commanderTowerTarget)
			{
				return commanderTowerTarget.handle.ObjID;
			}
			return 0u;
		}

		[MethodMetaInfo("根据塔id获取路径Index", "根据塔id获取路径Index")]
		public int GerPathIndexBuTowerId(uint TowerID)
		{
			List<PoolObjHandle<ActorRoot>> towerActors = Singleton<GameObjMgr>.GetInstance().TowerActors;
			for (int i = 0; i < towerActors.Count; i++)
			{
				PoolObjHandle<ActorRoot> poolObjHandle = towerActors[i];
				if (poolObjHandle.handle.ObjID == TowerID)
				{
					return poolObjHandle.handle.ObjLinker.PathIndex;
				}
			}
			return 0;
		}

		public override void UpdateLogic(int delta)
		{
			if (this.m_isPaused)
			{
				return;
			}
			int num = 1;
			if (!Singleton<FrameSynchr>.GetInstance().bActive)
			{
				num = 2;
			}
			this.m_frame = Singleton<FrameSynchr>.instance.CurFrameNum;
			if (this.m_wrapper != null)
			{
				ActorTypeDef actorType = this.m_wrapper.actor.TheActorMeta.ActorType;
				if (actorType == ActorTypeDef.Actor_Type_Hero || actorType == ActorTypeDef.Actor_Type_Call)
				{
					if (this.m_wrapper.myBehavior == ObjBehaviMode.State_AutoAI)
					{
						if ((this.m_frame + this.m_wrapper.actor.ObjID) % (uint)(3 * num) == 0u)
						{
							base.UpdateLogic(delta);
						}
					}
					else
					{
						base.UpdateLogic(delta);
					}
				}
				else
				{
					MonsterWrapper monsterWrapper = this.m_wrapper.actor.AsMonster();
					if (monsterWrapper != null && monsterWrapper.cfgInfo != null && monsterWrapper.cfgInfo.bIsBoss > 0)
					{
						if ((this.m_frame + this.m_wrapper.actor.ObjID) % (uint)(4 * num) == 0u)
						{
							base.UpdateLogic(delta);
						}
					}
					else if (actorType == ActorTypeDef.Actor_Type_Organ)
					{
						if ((this.m_frame + this.m_wrapper.actor.ObjID) % (uint)(5 * num) == 0u)
						{
							base.UpdateLogic(delta);
						}
					}
					else
					{
						int num2 = (int)((this.m_frame + this.m_wrapper.actor.ObjID) % (uint)(6 * num));
						if (num2 == 0 || (monsterWrapper != null && monsterWrapper.isCalledMonster))
						{
							base.UpdateLogic(delta);
						}
					}
				}
			}
			if (this.m_dengerCoolTick > 0)
			{
				this.m_dengerCoolTick--;
			}
			if (this.m_sound_Interval < 18000)
			{
				this.m_sound_Interval += delta;
			}
		}

		public bool IsInDanger()
		{
			return this.m_dengerCoolTick > 0;
		}

		public void SetInDanger()
		{
			if (Singleton<FrameSynchr>.instance.bActive)
			{
				this.m_dengerCoolTick = 30;
			}
			else
			{
				this.m_dengerCoolTick = 60;
			}
		}
	}
}
