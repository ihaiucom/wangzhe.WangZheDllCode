using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	public class DefaultAttackMode : BaseAttackMode
	{
		private uint commonAttackTargetID;

		private uint showInfoTargetID;

		public override void OnUse()
		{
			base.OnUse();
			this.commonAttackTargetID = 0u;
			this.showInfoTargetID = 0u;
		}

		public override uint CommonAttackSearchEnemy(int srchR)
		{
			if (this.commonAttackEnemyHeroTargetID > 0u)
			{
				PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(this.commonAttackEnemyHeroTargetID);
				if (!actor || actor.handle.ActorControl.IsDeadState)
				{
					this.ClearCommonAttackTarget();
					base.SetEnemyHeroAttackTargetID(0u);
					return 0u;
				}
				this.SetCommonAttackTarget(this.commonAttackEnemyHeroTargetID, true);
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
					this.ClearCommonAttackTarget();
				}
				else
				{
					this.SetCommonAttackTarget(num, false);
				}
				return num;
			}
		}

		protected override uint NormalModeCommonAttackSearchTarget(int srchR, SelectEnemyType type, ref bool bSearched)
		{
			uint num = this.commonAttackTargetID;
			if (base.IsValidTargetID(num))
			{
				this.SetCommonAttackTarget(num, false);
				bSearched = false;
				return num;
			}
			if (type == SelectEnemyType.SelectLowHp)
			{
				num = Singleton<CommonAttackSearcher>.GetInstance().CommonAttackSearchLowestHpTarget(this.actor.ActorControl, srchR);
			}
			else
			{
				num = Singleton<CommonAttackSearcher>.GetInstance().CommonAttackSearchNearestTarget(this.actor.ActorControl, srchR);
			}
			bSearched = true;
			return num;
		}

		protected override uint AdvancedModeCommonAttackSearchTarget(int srchR, SelectEnemyType type, ref bool bSearched)
		{
			uint num = this.commonAttackTargetID;
			if (base.IsValidTargetID(num) && base.TargetType(num, ActorTypeDef.Actor_Type_Hero))
			{
				this.SetCommonAttackTarget(num, false);
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
			uint num = this.commonAttackTargetID;
			if (base.IsValidTargetID(num) && base.TargetType(num, ActorTypeDef.Actor_Type_Monster))
			{
				this.SetCommonAttackTarget(num, false);
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
			if (num > 0u && base.TargetType(num, ActorTypeDef.Actor_Type_Monster))
			{
				return num;
			}
			srchR2 = this.actor.ActorControl.SearchRange;
			num = Singleton<AttackModeTargetSearcher>.GetInstance().SearchLowestHpTarget(ref this.actorPtr, srchR2, 0u, false, SearchTargetPriority.LastHit);
			if (num > 0u && base.TargetType(num, ActorTypeDef.Actor_Type_Monster))
			{
				return num;
			}
			srchR2 = this.actor.ActorControl.SearchRange;
			return Singleton<AttackModeTargetSearcher>.GetInstance().SearchNearestTarget(ref this.actorPtr, srchR2, 0u, true, SearchTargetPriority.LastHit);
		}

		protected override uint AttackOrganModeSearchTarget(int srchR, SelectEnemyType type, ref bool bSearched)
		{
			uint num = this.commonAttackTargetID;
			if (base.IsValidTargetID(num) && base.TargetType(num, ActorTypeDef.Actor_Type_Organ))
			{
				this.SetCommonAttackTarget(num, false);
				bSearched = false;
				return num;
			}
			bSearched = true;
			int srchR2;
			if (type != SelectEnemyType.SelectLowHp)
			{
				srchR2 = this.actor.ActorControl.SearchRange;
				return Singleton<AttackModeTargetSearcher>.GetInstance().SearchNearestTarget(ref this.actorPtr, srchR2, 0u, true, SearchTargetPriority.AttackOrgan);
			}
			srchR2 = this.actor.ActorControl.AttackRange;
			num = Singleton<AttackModeTargetSearcher>.GetInstance().SearchLowestHpTarget(ref this.actorPtr, srchR2, 0u, false, SearchTargetPriority.AttackOrgan);
			if (num > 0u && base.TargetType(num, ActorTypeDef.Actor_Type_Organ))
			{
				return num;
			}
			srchR2 = this.actor.ActorControl.SearchRange;
			num = Singleton<AttackModeTargetSearcher>.GetInstance().SearchLowestHpTarget(ref this.actorPtr, srchR2, 0u, false, SearchTargetPriority.AttackOrgan);
			if (num > 0u && base.TargetType(num, ActorTypeDef.Actor_Type_Organ))
			{
				return num;
			}
			srchR2 = this.actor.ActorControl.SearchRange;
			return Singleton<AttackModeTargetSearcher>.GetInstance().SearchNearestTarget(ref this.actorPtr, srchR2, 0u, true, SearchTargetPriority.AttackOrgan);
		}

		public override uint SelectSkillTarget(SkillSlot _slot)
		{
			SkillSelectControl instance = Singleton<SkillSelectControl>.GetInstance();
			Skill skill = (_slot.NextSkillObj == null) ? _slot.SkillObj : _slot.NextSkillObj;
			ActorRoot actorRoot;
			if (Singleton<GameInput>.GetInstance().IsSmartUse() || skill.cfgData.bSkillTargetRule == 2)
			{
				actorRoot = instance.SelectTarget((SkillTargetRule)skill.cfgData.bSkillTargetRule, _slot);
			}
			else
			{
				actorRoot = _slot.skillIndicator.GetUseSkillTargetDefaultAttackMode();
			}
			if (actorRoot != null && base.IsValidTargetID(actorRoot.ObjID))
			{
				this.SetShowTargetInfo(actorRoot.ObjID);
			}
			return (actorRoot == null) ? 0u : actorRoot.ObjID;
		}

		public override VInt3 SelectSkillDirection(SkillSlot _slot)
		{
			VInt3 result = VInt3.one;
			SkillSelectControl instance = Singleton<SkillSelectControl>.GetInstance();
			Skill skill = (_slot.NextSkillObj == null) ? _slot.SkillObj : _slot.NextSkillObj;
			if (Singleton<GameInput>.GetInstance().IsSmartUse())
			{
				result = instance.SelectTargetDir((SkillTargetRule)skill.cfgData.bSkillTargetRule, _slot);
			}
			else
			{
				result = (VInt3)_slot.skillIndicator.GetUseSkillDirection();
			}
			return result;
		}

		public override bool SelectSkillPos(SkillSlot _slot, out VInt3 _position)
		{
			bool result = false;
			SkillSelectControl instance = Singleton<SkillSelectControl>.GetInstance();
			Skill skill = (_slot.NextSkillObj == null) ? _slot.SkillObj : _slot.NextSkillObj;
			if (Singleton<GameInput>.GetInstance().IsSmartUse())
			{
				_position = instance.SelectTargetPos((SkillTargetRule)skill.cfgData.bSkillTargetRule, _slot, out result);
				return result;
			}
			if (_slot.skillIndicator.IsAllowUseSkill())
			{
				_position = (VInt3)_slot.skillIndicator.GetUseSkillPosition();
				return true;
			}
			_position = VInt3.zero;
			return false;
		}

		public void ClearCommonAttackTarget()
		{
			if (this.commonAttackTargetID != 0u)
			{
				this.commonAttackTargetID = 0u;
			}
		}

		public void SetCommonAttackTarget(uint _targetID, bool bIsFromAttackEnemyHero = false)
		{
			if (!bIsFromAttackEnemyHero && this.commonAttackEnemyHeroTargetID != 0u)
			{
				this.commonAttackEnemyHeroTargetID = 0u;
			}
			if (this.commonAttackTargetID != _targetID)
			{
				this.commonAttackTargetID = _targetID;
				this.SetShowTargetInfo(_targetID);
			}
		}

		public void ClearShowTargetInfo()
		{
			if (this.showInfoTargetID != 0u)
			{
				if (ActorHelper.IsHostActor(ref this.actorPtr))
				{
					SelectTargetEventParam selectTargetEventParam = new SelectTargetEventParam(this.showInfoTargetID);
					Singleton<GameSkillEventSys>.GetInstance().SendEvent<SelectTargetEventParam>(GameSkillEventDef.Event_ClearTarget, base.GetActor(), ref selectTargetEventParam, GameSkillEventChannel.Channel_HostCtrlActor);
				}
				this.showInfoTargetID = 0u;
			}
		}

		public void SetShowTargetInfo(uint _TargetID)
		{
			this.showInfoTargetID = _TargetID;
			if (ActorHelper.IsHostActor(ref this.actorPtr))
			{
				SelectTargetEventParam selectTargetEventParam = new SelectTargetEventParam(this.showInfoTargetID);
				Singleton<GameSkillEventSys>.GetInstance().SendEvent<SelectTargetEventParam>(GameSkillEventDef.Event_SelectTarget, base.GetActor(), ref selectTargetEventParam, GameSkillEventChannel.Channel_HostCtrlActor);
			}
		}

		public override bool CancelCommonAttackMode()
		{
			if (this.actor.SkillControl.SkillUseCache != null)
			{
				this.actor.SkillControl.SkillUseCache.SetCommonAttackMode(false);
				this.ClearCommonAttackTarget();
			}
			return true;
		}

		public override void UpdateLogic(int nDelta)
		{
			if (this.showInfoTargetID != 0u && !base.IsValidTargetID(this.showInfoTargetID))
			{
				this.ClearShowTargetInfo();
			}
			if (this.actorPtr)
			{
				ObjWrapper actorControl = this.actor.ActorControl;
				if (actorControl != null)
				{
					ObjBehaviMode myBehavior = actorControl.myBehavior;
					if (myBehavior != ObjBehaviMode.Normal_Attack && (myBehavior <= ObjBehaviMode.UseSkill_0 || myBehavior >= ObjBehaviMode.UseSkill_7))
					{
						this.commonAttackEnemyHeroTargetID = 0u;
					}
				}
			}
		}

		public override void OnDead()
		{
			this.ClearCommonAttackTarget();
		}
	}
}
