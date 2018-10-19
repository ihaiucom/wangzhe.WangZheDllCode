using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic.GameKernal;
using System;

namespace Assets.Scripts.GameLogic
{
	public class BaseAttackMode : LogicComponent
	{
		protected uint commonAttackEnemyHeroTargetID;

		protected CommonAttackButtonType commonAttackButtonType = CommonAttackButtonType.CommonAttackButton;

		public override void OnUse()
		{
			base.OnUse();
			this.commonAttackEnemyHeroTargetID = 0u;
			this.commonAttackButtonType = CommonAttackButtonType.CommonAttackButton;
		}

		protected bool TargetType(uint selectID, ActorTypeDef ActorType)
		{
			bool result = false;
			if (selectID > 0u)
			{
				PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(selectID);
				result = (actor && actor.handle.TheActorMeta.ActorType == ActorType);
			}
			return result;
		}

		protected bool IsValidTargetID(uint selectID)
		{
			bool flag = false;
			if (selectID <= 0u)
			{
				return flag;
			}
			PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(selectID);
			flag = (actor && !actor.handle.ObjLinker.Invincible && !actor.handle.ActorControl.IsDeadState && !this.actor.IsSelfCamp(actor) && actor.handle.HorizonMarker.IsVisibleFor(this.actor.TheActorMeta.ActorCamp) && actor.handle.AttackOrderReady);
			if (!flag)
			{
				return flag;
			}
			Skill nextSkill = this.actor.ActorControl.GetNextSkill(SkillSlotType.SLOT_SKILL_0);
			if (nextSkill != null)
			{
				long num = (long)nextSkill.GetMaxSearchDistance(0);
				if (!actor || actor.handle.shape == null || actor.handle.ActorAgent == null || nextSkill.cfgData == null)
				{
					return false;
				}
				num += (long)actor.handle.ActorControl.GetDetectedRadius();
				num *= num;
				if ((this.actor.ActorControl.actorLocation - actor.handle.location).sqrMagnitudeLong2D > num)
				{
					return false;
				}
			}
			return flag;
		}

		public virtual uint CommonAttackSearchEnemy(int srchR)
		{
			return 0u;
		}

		public virtual uint SelectSkillTarget(SkillSlot _slot)
		{
			return 0u;
		}

		public virtual VInt3 SelectSkillDirection(SkillSlot _slot)
		{
			return VInt3.one;
		}

		public virtual bool SelectSkillPos(SkillSlot _slot, out VInt3 _position)
		{
			_position = VInt3.zero;
			return false;
		}

		public virtual bool CancelCommonAttackMode()
		{
			return false;
		}

		public virtual void OnDead()
		{
		}

		public void SetEnemyHeroAttackTargetID(uint uiTargetId)
		{
			this.commonAttackEnemyHeroTargetID = uiTargetId;
		}

		public uint GetEnemyHeroAttackTargetID()
		{
			return this.commonAttackEnemyHeroTargetID;
		}

		public void SetCommonButtonType(sbyte type)
		{
			this.commonAttackButtonType = (CommonAttackButtonType)type;
		}

		protected virtual uint NormalModeCommonAttackSearchTarget(int srchR, SelectEnemyType type, ref bool bSearched)
		{
			return 0u;
		}

		protected virtual uint AdvancedModeCommonAttackSearchTarget(int srchR, SelectEnemyType type, ref bool bSearched)
		{
			return 0u;
		}

		protected virtual uint LastHitModeSearchTarget(int srchR, SelectEnemyType type, ref bool bSearched)
		{
			return 0u;
		}

		protected virtual uint AttackOrganModeSearchTarget(int srchR, SelectEnemyType type, ref bool bSearched)
		{
			return 0u;
		}

		protected uint ExecuteSearchTarget(int srchR, ref bool bSearched)
		{
			PoolObjHandle<ActorRoot> poolObjHandle = this.actorPtr;
			MonsterWrapper monsterWrapper = poolObjHandle.handle.ActorControl as MonsterWrapper;
			if (monsterWrapper != null && monsterWrapper.isCalledMonster)
			{
				poolObjHandle = monsterWrapper.hostActor;
			}
			Player ownerPlayer = ActorHelper.GetOwnerPlayer(ref poolObjHandle);
			if (ownerPlayer == null)
			{
				return 0u;
			}
			SelectEnemyType attackTargetMode = ownerPlayer.AttackTargetMode;
			LastHitMode useLastHitMode = ownerPlayer.useLastHitMode;
			AttackOrganMode curAttackOrganMode = ownerPlayer.curAttackOrganMode;
			OperateMode operateMode = ownerPlayer.GetOperateMode();
			uint result;
			if (operateMode == OperateMode.LockMode)
			{
				result = this.NormalModeCommonAttackSearchTarget(srchR, attackTargetMode, ref bSearched);
			}
			else if (this.commonAttackButtonType == CommonAttackButtonType.CommonAttackButton)
			{
				if (useLastHitMode == LastHitMode.None && curAttackOrganMode == AttackOrganMode.None)
				{
					result = this.NormalModeCommonAttackSearchTarget(srchR, attackTargetMode, ref bSearched);
				}
				else
				{
					result = this.AdvancedModeCommonAttackSearchTarget(srchR, attackTargetMode, ref bSearched);
				}
			}
			else if (this.commonAttackButtonType == CommonAttackButtonType.LastHitButton)
			{
				result = this.LastHitModeSearchTarget(srchR, attackTargetMode, ref bSearched);
			}
			else
			{
				result = this.AttackOrganModeSearchTarget(srchR, attackTargetMode, ref bSearched);
			}
			return result;
		}
	}
}
