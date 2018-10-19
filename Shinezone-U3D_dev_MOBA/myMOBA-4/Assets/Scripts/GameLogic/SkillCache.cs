using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using System;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public class SkillCache
	{
		private IFrameCommand cacheSkillCommand;

		private SkillUseParam cacheSkillParam;

		private IFrameCommand cacheMoveCommand;

		private bool newAttackCommand;

		private bool moveToAttackTarget;

		private bool commonAttackMode;

		private bool bCacheCommonAttack;

		private bool cacheSkillExpire = true;

		private bool cacheRotateExpire = true;

		private bool cacheMoveExpire = true;

		private bool cacheSkill;

		private bool cacheMove;

		private bool bSpecialCommonAttack;

		public void Clear()
		{
			this.commonAttackMode = false;
			this.cacheSkillExpire = true;
			this.cacheRotateExpire = true;
			this.cacheMoveExpire = true;
			this.cacheSkillCommand = null;
			this.cacheMoveCommand = null;
			this.bCacheCommonAttack = false;
			this.cacheSkill = false;
			this.cacheMove = false;
		}

		public bool IsCacheCommonAttack()
		{
			return this.cacheSkillExpire && this.bCacheCommonAttack;
		}

		public bool IsExistNewAttackCommand()
		{
			return this.newAttackCommand;
		}

		public void SetNewAttackCommand()
		{
			if (this.moveToAttackTarget)
			{
				this.newAttackCommand = true;
			}
			else
			{
				this.newAttackCommand = false;
			}
		}

		public void SetMoveToAttackTarget(bool bFlag)
		{
			this.moveToAttackTarget = bFlag;
			this.newAttackCommand = false;
		}

		public void SetSpecialCommonAttack(bool _bOpen)
		{
			this.bSpecialCommonAttack = _bOpen;
		}

		public bool GetSpecialCommonAttack()
		{
			return this.bSpecialCommonAttack;
		}

		public void SetCommonAttackMode(bool _bOpen)
		{
			this.commonAttackMode = _bOpen;
		}

		public bool GetCommonAttackMode()
		{
			return this.commonAttackMode;
		}

		public bool GetCacheSkill()
		{
			return !this.cacheSkillExpire;
		}

		public bool GetCacheSkillSlotType(out SkillSlotType _slotType)
		{
			if (!this.cacheSkillExpire)
			{
				_slotType = this.cacheSkillParam.SlotType;
				return true;
			}
			_slotType = SkillSlotType.SLOT_SKILL_VALID;
			return false;
		}

		public void SetCacheSkill(bool _bOpen)
		{
			this.cacheSkill = _bOpen;
		}

		public void SetCacheMove(bool _bOpen)
		{
			this.cacheMove = _bOpen;
		}

		public void SetCacheSkillContext(IFrameCommand cmd, SkillUseParam _param)
		{
			if (this.cacheSkill)
			{
				this.cacheSkillCommand = cmd;
				this.cacheSkillParam = _param;
				this.bCacheCommonAttack = false;
				this.cacheSkillExpire = false;
			}
		}

		public void SetCacheNormalAttackContext(IFrameCommand _command)
		{
			if (this.cacheSkill)
			{
				this.cacheSkillCommand = _command;
				this.bCacheCommonAttack = true;
				this.cacheSkillExpire = false;
			}
		}

		public void SetCacheMoveCommand(IFrameCommand _command)
		{
			if (this.cacheMove)
			{
				this.cacheMoveCommand = _command;
				this.cacheMoveExpire = false;
				this.cacheRotateExpire = false;
			}
		}

		public void SetCacheMoveExpire(bool bFlag)
		{
			this.cacheMoveExpire = true;
		}

		public bool GetCacheMoveExpire()
		{
			return this.cacheMoveExpire;
		}

		public void UseSkillCacheLerpMove(PoolObjHandle<ActorRoot> _actorRoot, int _deltaTime, int _moveSpeed)
		{
			if (!_actorRoot)
			{
				return;
			}
			if (this.cacheMoveCommand != null && !this.cacheMoveExpire && this.cacheMoveCommand.cmdType == 131)
			{
				FrameCommand<MoveDirectionCommand> frameCommand = (FrameCommand<MoveDirectionCommand>)this.cacheMoveCommand;
				if (!_actorRoot.handle.ActorControl.GetNoAbilityFlag(ObjAbilityType.ObjAbility_Move))
				{
					VInt3 vInt = VInt3.right.RotateY((int)frameCommand.cmdData.Degree);
					Vector3 position = _actorRoot.handle.myTransform.position;
					VInt3 vInt2 = vInt.NormalizeTo(_moveSpeed * _deltaTime / 1000);
					VInt vInt3 = 0;
					vInt2 = PathfindingUtility.MoveLerp(_actorRoot, (VInt3)position, vInt2, out vInt3);
					if (_actorRoot.handle.MovementComponent.isFlying)
					{
						float y = position.y;
						_actorRoot.handle.myTransform.position += (Vector3)vInt2;
						Vector3 position2 = _actorRoot.handle.myTransform.position;
						position2.y = y;
						_actorRoot.handle.myTransform.position = position2;
					}
					else
					{
						_actorRoot.handle.myTransform.position += (Vector3)vInt2;
					}
				}
			}
		}

		public void UseSkillCacheMove(PoolObjHandle<ActorRoot> _actorRoot, int _deltaTime, int _moveSpeed)
		{
			if (!_actorRoot)
			{
				return;
			}
			if (this.cacheMoveCommand != null && !this.cacheMoveExpire && this.cacheMoveCommand.cmdType == 131)
			{
				FrameCommand<MoveDirectionCommand> frameCommand = (FrameCommand<MoveDirectionCommand>)this.cacheMoveCommand;
				if (!_actorRoot.handle.ActorControl.GetNoAbilityFlag(ObjAbilityType.ObjAbility_Move))
				{
					VInt3 vInt = VInt3.right.RotateY((int)frameCommand.cmdData.Degree).NormalizeTo(_moveSpeed * _deltaTime / 1000);
					VInt groundY = _actorRoot.handle.groundY;
					vInt = PathfindingUtility.Move(_actorRoot.handle, vInt, out groundY, out _actorRoot.handle.hasReachedNavEdge, null);
					if (_actorRoot.handle.MovementComponent.isFlying)
					{
						int y = _actorRoot.handle.location.y;
						_actorRoot.handle.location += vInt;
						VInt3 location = _actorRoot.handle.location;
						location.y = y;
						_actorRoot.handle.location = location;
					}
					else
					{
						_actorRoot.handle.location += vInt;
					}
					_actorRoot.handle.groundY = groundY;
				}
			}
		}

		public void UseSkillCache(PoolObjHandle<ActorRoot> _actorRoot)
		{
			if (!_actorRoot)
			{
				return;
			}
			if (!this.cacheSkillExpire)
			{
				if (!this.bCacheCommonAttack)
				{
					SkillSlot skillSlot = _actorRoot.handle.SkillControl.GetSkillSlot(this.cacheSkillParam.SlotType);
					if (skillSlot != null && skillSlot.IsEnableSkillSlot())
					{
						this.cacheSkillCommand.frameNum = Singleton<FrameSynchr>.GetInstance().CurFrameNum;
						_actorRoot.handle.ActorControl.CmdUseSkill(this.cacheSkillCommand, ref this.cacheSkillParam);
					}
				}
				else
				{
					_actorRoot.handle.ActorControl.CacheNoramlAttack(this.cacheSkillCommand, SkillSlotType.SLOT_SKILL_0);
				}
				this.cacheSkillExpire = true;
			}
			if (this.cacheMoveCommand != null && !this.cacheRotateExpire)
			{
				if (this.cacheMoveCommand.cmdType == 131)
				{
					FrameCommand<MoveDirectionCommand> frameCommand = (FrameCommand<MoveDirectionCommand>)this.cacheMoveCommand;
					if (!_actorRoot.handle.ActorControl.GetNoAbilityFlag(ObjAbilityType.ObjAbility_MoveRotate))
					{
						VInt3 vInt = VInt3.right.RotateY((int)frameCommand.cmdData.Degree);
						_actorRoot.handle.MovementComponent.SetRotate(vInt, true);
						Quaternion rotation = Quaternion.identity;
						rotation = Quaternion.LookRotation((Vector3)vInt);
						_actorRoot.handle.rotation = rotation;
					}
				}
				this.cacheRotateExpire = true;
			}
		}
	}
}
