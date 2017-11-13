using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic.GameKernal;
using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	public class LockTargetAttackMode : BaseAttackMode
	{
		private uint lockTargetID;

		public override void OnUse()
		{
			base.OnUse();
			this.lockTargetID = 0u;
		}

		public uint GetLockTargetID()
		{
			return this.lockTargetID;
		}

		public void SetLockTargetID(uint _targetID, bool bIsFromAttackEnemyHero = false)
		{
			if (!bIsFromAttackEnemyHero && this.commonAttackEnemyHeroTargetID != 0u)
			{
				this.commonAttackEnemyHeroTargetID = 0u;
			}
			if (this.lockTargetID != _targetID)
			{
				this.lockTargetID = _targetID;
				LockTargetEventParam lockTargetEventParam = new LockTargetEventParam(this.lockTargetID);
				Singleton<GameSkillEventSys>.GetInstance().SendEvent<LockTargetEventParam>(GameSkillEventDef.Event_LockTarget, base.GetActor(), ref lockTargetEventParam, GameSkillEventChannel.Channel_HostCtrlActor);
			}
		}

		public void ClearTargetID()
		{
			LockTargetEventParam lockTargetEventParam = new LockTargetEventParam(this.lockTargetID);
			Singleton<GameSkillEventSys>.GetInstance().SendEvent<LockTargetEventParam>(GameSkillEventDef.Event_ClearLockTarget, base.GetActor(), ref lockTargetEventParam, GameSkillEventChannel.Channel_HostCtrlActor);
			this.lockTargetID = 0u;
		}

		public bool IsValidSkillTargetID(uint _targetID, uint _targetMask)
		{
			if (_targetID <= 0u)
			{
				return false;
			}
			PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(_targetID);
			return actor && ((ulong)_targetMask & 1uL << (int)(actor.handle.TheActorMeta.ActorType & (ActorTypeDef)31)) <= 0uL;
		}

		public bool IsValidLockTargetID(uint _targetID)
		{
			bool flag = false;
			if (_targetID <= 0u)
			{
				return flag;
			}
			PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(_targetID);
			flag = (actor && !actor.handle.ObjLinker.Invincible && !actor.handle.ActorControl.IsDeadState && !this.actor.IsSelfCamp(actor) && actor.handle.HorizonMarker.IsVisibleFor(this.actor.TheActorMeta.ActorCamp) && actor.handle.AttackOrderReady);
			if (!flag)
			{
				return flag;
			}
			long num = 12000L;
			num *= num;
			return (this.actor.ActorControl.actorLocation - actor.handle.location).sqrMagnitudeLong2D <= num && flag;
		}

		public override uint CommonAttackSearchEnemy(int srchR)
		{
			if (this.commonAttackEnemyHeroTargetID > 0u)
			{
				PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(this.commonAttackEnemyHeroTargetID);
				if (!actor || actor.handle.ActorControl.IsDeadState)
				{
					this.ClearTargetID();
					base.SetEnemyHeroAttackTargetID(0u);
					return 0u;
				}
				this.SetLockTargetID(this.commonAttackEnemyHeroTargetID, true);
				return this.commonAttackEnemyHeroTargetID;
			}
			else
			{
				bool flag = false;
				uint num = base.ExecuteSearchTarget(srchR, ref flag);
				if (!flag)
				{
					return num;
				}
				if (!base.IsValidTargetID(num))
				{
					SkillCache skillUseCache = this.actor.SkillControl.SkillUseCache;
					if (skillUseCache != null && !skillUseCache.GetSpecialCommonAttack())
					{
						this.CancelCommonAttackMode();
						num = 0u;
					}
				}
				if (num == 0u)
				{
					this.ClearTargetID();
				}
				else
				{
					this.SetLockTargetID(num, false);
				}
				return num;
			}
		}

		protected override uint NormalModeCommonAttackSearchTarget(int srchR, SelectEnemyType type, ref bool bSearched)
		{
			uint num = this.lockTargetID;
			if (base.IsValidTargetID(num))
			{
				bSearched = false;
				return num;
			}
			if (type == SelectEnemyType.SelectLowHp)
			{
				num = Singleton<AttackModeTargetSearcher>.GetInstance().SearchLowestHpTarget(ref this.actorPtr, srchR, 0u, true, SearchTargetPriority.CommonAttack);
			}
			else
			{
				num = Singleton<AttackModeTargetSearcher>.GetInstance().SearchNearestTarget(ref this.actorPtr, srchR, 0u, true, SearchTargetPriority.CommonAttack);
			}
			bSearched = true;
			return num;
		}

		protected override uint AdvancedModeCommonAttackSearchTarget(int srchR, SelectEnemyType type, ref bool bSearched)
		{
			uint num = this.lockTargetID;
			if (base.IsValidTargetID(num) && base.TargetType(num, ActorTypeDef.Actor_Type_Hero))
			{
				bSearched = false;
				return num;
			}
			bSearched = true;
			int srchR2;
			if (type != SelectEnemyType.SelectLowHp)
			{
				srchR2 = this.actor.ActorControl.SearchRange;
				return Singleton<AttackModeTargetSearcher>.GetInstance().SearchNearestTarget(ref this.actorPtr, srchR2, 0u, true, SearchTargetPriority.CommonAttack);
			}
			srchR2 = this.actor.ActorControl.AttackRange;
			num = Singleton<AttackModeTargetSearcher>.GetInstance().SearchLowestHpTarget(ref this.actorPtr, srchR2, 0u, false, SearchTargetPriority.CommonAttack);
			if (num > 0u && base.TargetType(num, ActorTypeDef.Actor_Type_Hero))
			{
				return num;
			}
			srchR2 = this.actor.ActorControl.SearchRange;
			num = Singleton<AttackModeTargetSearcher>.GetInstance().SearchLowestHpTarget(ref this.actorPtr, srchR2, 0u, false, SearchTargetPriority.CommonAttack);
			if (num > 0u && base.TargetType(num, ActorTypeDef.Actor_Type_Hero))
			{
				return num;
			}
			srchR2 = this.actor.ActorControl.SearchRange;
			return Singleton<AttackModeTargetSearcher>.GetInstance().SearchNearestTarget(ref this.actorPtr, srchR2, 0u, true, SearchTargetPriority.CommonAttack);
		}

		protected override uint LastHitModeSearchTarget(int srchR, SelectEnemyType type, ref bool bSearched)
		{
			uint num = this.lockTargetID;
			if (base.IsValidTargetID(num) && !base.TargetType(num, ActorTypeDef.Actor_Type_Hero))
			{
				bSearched = false;
				return num;
			}
			bSearched = true;
			int srchR2;
			if (type != SelectEnemyType.SelectLowHp)
			{
				srchR2 = this.actor.ActorControl.SearchRange;
				return Singleton<AttackModeTargetSearcher>.GetInstance().SearchNearestTarget(ref this.actorPtr, srchR2, 0u, true, SearchTargetPriority.LastHit);
			}
			srchR2 = this.actor.ActorControl.AttackRange;
			num = Singleton<AttackModeTargetSearcher>.GetInstance().SearchLowestHpTarget(ref this.actorPtr, srchR2, 0u, false, SearchTargetPriority.LastHit);
			if (num > 0u && !base.TargetType(num, ActorTypeDef.Actor_Type_Hero))
			{
				return num;
			}
			srchR2 = this.actor.ActorControl.SearchRange;
			num = Singleton<AttackModeTargetSearcher>.GetInstance().SearchLowestHpTarget(ref this.actorPtr, srchR2, 0u, false, SearchTargetPriority.LastHit);
			if (num > 0u && !base.TargetType(num, ActorTypeDef.Actor_Type_Hero))
			{
				return num;
			}
			srchR2 = this.actor.ActorControl.SearchRange;
			return Singleton<AttackModeTargetSearcher>.GetInstance().SearchNearestTarget(ref this.actorPtr, srchR2, 0u, true, SearchTargetPriority.LastHit);
		}

		public override uint SelectSkillTarget(SkillSlot _slot)
		{
			uint num = this.lockTargetID;
			SelectEnemyType selectEnemyType = SelectEnemyType.SelectLowHp;
			Player ownerPlayer = ActorHelper.GetOwnerPlayer(ref this.actorPtr);
			SkillSelectControl instance = Singleton<SkillSelectControl>.GetInstance();
			Skill skill = (_slot.NextSkillObj != null) ? _slot.NextSkillObj : _slot.SkillObj;
			uint dwSkillTargetFilter = skill.cfgData.dwSkillTargetFilter;
			if (skill.cfgData.bSkillTargetRule == 2)
			{
				num = this.actor.ObjID;
			}
			else if (skill.cfgData.bSkillTargetRule == 5)
			{
				ActorRoot actorRoot = instance.SelectTarget(SkillTargetRule.NextSkillTarget, _slot);
				if (actorRoot != null)
				{
					num = actorRoot.ObjID;
					if (this.IsValidLockTargetID(num))
					{
						Singleton<NetLockAttackTarget>.GetInstance().SendLockAttackTarget(num);
					}
				}
			}
			else
			{
				ActorRoot useSkillTargetLockAttackMode = _slot.skillIndicator.GetUseSkillTargetLockAttackMode();
				if (useSkillTargetLockAttackMode != null)
				{
					if (this.IsValidLockTargetID(useSkillTargetLockAttackMode.ObjID))
					{
						num = useSkillTargetLockAttackMode.ObjID;
						Singleton<NetLockAttackTarget>.GetInstance().SendLockAttackTarget(num);
					}
				}
				else if (!this.IsValidLockTargetID(this.lockTargetID))
				{
					if (ownerPlayer != null)
					{
						selectEnemyType = ownerPlayer.AttackTargetMode;
					}
					int srchR;
					if (skill.AppointType == SkillRangeAppointType.Target)
					{
						srchR = skill.GetMaxSearchDistance(_slot.GetSkillLevel());
					}
					else
					{
						srchR = skill.cfgData.iMaxAttackDistance;
					}
					if (selectEnemyType == SelectEnemyType.SelectLowHp)
					{
						num = Singleton<AttackModeTargetSearcher>.GetInstance().SearchLowestHpTarget(ref this.actorPtr, srchR, dwSkillTargetFilter, true, SearchTargetPriority.CommonAttack);
					}
					else
					{
						num = Singleton<AttackModeTargetSearcher>.GetInstance().SearchNearestTarget(ref this.actorPtr, srchR, dwSkillTargetFilter, true, SearchTargetPriority.CommonAttack);
					}
					if (this.IsValidLockTargetID(num))
					{
						Singleton<NetLockAttackTarget>.GetInstance().SendLockAttackTarget(num);
					}
				}
				else if (!this.IsValidSkillTargetID(num, dwSkillTargetFilter))
				{
					num = 0u;
				}
			}
			return num;
		}

		public override VInt3 SelectSkillDirection(SkillSlot _slot)
		{
			VInt3 one = VInt3.one;
			Skill arg_22_0 = (_slot.NextSkillObj != null) ? _slot.NextSkillObj : _slot.SkillObj;
			return (VInt3)_slot.skillIndicator.GetUseSkillDirection();
		}

		public override bool SelectSkillPos(SkillSlot _slot, out VInt3 _position)
		{
			bool result = false;
			Skill arg_1E_0 = (_slot.NextSkillObj != null) ? _slot.NextSkillObj : _slot.SkillObj;
			if (_slot.skillIndicator.IsAllowUseSkill())
			{
				_position = (VInt3)_slot.skillIndicator.GetUseSkillPosition();
				return true;
			}
			_position = VInt3.zero;
			return result;
		}

		public override void UpdateLogic(int nDelta)
		{
			if (this.lockTargetID != 0u && !this.IsValidLockTargetID(this.lockTargetID))
			{
				this.ClearTargetID();
			}
			if (this.actorPtr && this.actorPtr.handle.ActorAgent != null && this.actorPtr.handle.ActorAgent.m_wrapper != null)
			{
				ObjBehaviMode myBehavior = this.actorPtr.handle.ActorAgent.m_wrapper.myBehavior;
				if (myBehavior != ObjBehaviMode.Normal_Attack && (myBehavior <= ObjBehaviMode.UseSkill_0 || myBehavior >= ObjBehaviMode.UseSkill_7) && this.commonAttackEnemyHeroTargetID != 0u)
				{
					this.commonAttackEnemyHeroTargetID = 0u;
				}
			}
		}

		public override bool CancelCommonAttackMode()
		{
			if (this.actor.SkillControl.SkillUseCache != null)
			{
				this.actor.SkillControl.SkillUseCache.SetCommonAttackMode(false);
			}
			return true;
		}

		public override void OnDead()
		{
			this.ClearTargetID();
		}
	}
}
