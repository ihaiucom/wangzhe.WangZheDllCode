using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using ResData;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameLogic
{
	public class BufferLogicEffect
	{
		private BuffHolderComponent buffHolder;

		private SkillSlotHurt[] skillSlotHurt = new SkillSlotHurt[10];

		private List<PoolObjHandle<BuffSkill>> extraHurtList = new List<PoolObjHandle<BuffSkill>>();

		public void Init(BuffHolderComponent _buffHolder)
		{
			this.buffHolder = _buffHolder;
			this.Clear();
		}

		public void Clear()
		{
			for (int i = 0; i < 10; i++)
			{
				this.skillSlotHurt[i].curTotalHurt = 0;
				this.skillSlotHurt[i].nextTotalHurt = 0;
				this.skillSlotHurt[i].skillUseCount = 0u;
				this.skillSlotHurt[i].cdTime = 0;
				this.skillSlotHurt[i].recordTime = 0uL;
			}
		}

		public List<PoolObjHandle<BuffSkill>> GetExtraHurtList()
		{
			return this.extraHurtList;
		}

		public void AddBuff(BuffSkill inBuff)
		{
			ResDT_SkillFunc resDT_SkillFunc = null;
			if (inBuff != null && inBuff.FindSkillFunc(72, out resDT_SkillFunc))
			{
				this.extraHurtList.Add(new PoolObjHandle<BuffSkill>(inBuff));
			}
		}

		public void RemoveBuff(ref PoolObjHandle<BuffSkill> inBuff)
		{
			ResDT_SkillFunc resDT_SkillFunc = null;
			if (inBuff && inBuff.handle.FindSkillFunc(72, out resDT_SkillFunc))
			{
				this.extraHurtList.Remove(inBuff);
			}
		}

		public void ClearBuff()
		{
			this.extraHurtList.Clear();
		}

		public void InitSkillSlotExtraHurt(SkillSlot _slot)
		{
			int slotType = (int)_slot.SlotType;
			uint skillUseCount = _slot.GetSkillUseCount();
			if (this.extraHurtList.Count < 1)
			{
				return;
			}
			if (this.skillSlotHurt[slotType].skillUseCount >= 4294967295u)
			{
				this.skillSlotHurt[slotType].skillUseCount = 0u;
			}
			if (skillUseCount == this.skillSlotHurt[slotType].skillUseCount + 1u && Singleton<FrameSynchr>.GetInstance().LogicFrameTick - this.skillSlotHurt[slotType].recordTime <= (ulong)((long)this.skillSlotHurt[slotType].cdTime))
			{
				this.skillSlotHurt[slotType].curTotalHurt = this.skillSlotHurt[slotType].nextTotalHurt;
				this.skillSlotHurt[slotType].nextTotalHurt = 0;
			}
			else
			{
				this.skillSlotHurt[slotType].curTotalHurt = 0;
				this.skillSlotHurt[slotType].nextTotalHurt = 0;
			}
		}

		private void AddSkillSlotHurt(SkillSlotType _slotType, int _hurtValue)
		{
			SkillSlotHurt[] expr_0C_cp_0 = this.skillSlotHurt;
			expr_0C_cp_0[(int)_slotType].nextTotalHurt = expr_0C_cp_0[(int)_slotType].nextTotalHurt + _hurtValue;
		}

		private int GetSkillSlotExtraHurt(SkillSlotType _slotType, int _hurtRate)
		{
			return this.skillSlotHurt[(int)_slotType].curTotalHurt * _hurtRate / 10000;
		}

		private void SetSkillSlotUseCount(SkillSlotType _slotType, uint _useCount)
		{
			this.skillSlotHurt[(int)_slotType].skillUseCount = _useCount;
		}

		private void SetSkillSlotUseTime(SkillSlotType _slotType, int _cdTime)
		{
			this.skillSlotHurt[(int)_slotType].cdTime = _cdTime;
			this.skillSlotHurt[(int)_slotType].recordTime = Singleton<FrameSynchr>.GetInstance().LogicFrameTick;
		}

		private int DamageExtraEffect(ref HurtDataInfo _hurt, int _hurtVale, int _hurtRate, int _typeMask, int _typeSubMask)
		{
			SkillSlot skillSlot = null;
			if (_hurt.atker.handle.SkillControl.TryGetSkillSlot(_hurt.atkSlot, out skillSlot))
			{
				BufferLogicEffect logicEffect = _hurt.atker.handle.BuffHolderComp.logicEffect;
				int num = _hurtVale + logicEffect.GetSkillSlotExtraHurt(_hurt.atkSlot, _hurtRate);
				if (this.buffHolder.CheckTargetSubType(_typeMask, _typeSubMask, _hurt.target))
				{
					this.AddSkillSlotHurt(_hurt.atkSlot, num);
				}
				return num;
			}
			return _hurtVale;
		}

		public static int OnDamageExtraEffect(ref HurtDataInfo _hurt, int _hurtValue)
		{
			SkillSlot skillSlot = null;
			if (!_hurt.atker || _hurt.atker.handle.ActorControl.IsDeadState)
			{
				return _hurtValue;
			}
			BufferLogicEffect logicEffect = _hurt.atker.handle.BuffHolderComp.logicEffect;
			List<PoolObjHandle<BuffSkill>> list = logicEffect.GetExtraHurtList();
			for (int i = 0; i < list.Count; i++)
			{
				if (!_hurt.target)
				{
					return _hurtValue;
				}
				BuffSkill buffSkill = list[i];
				int skillFuncParam = buffSkill.GetSkillFuncParam(72, 0, false);
				if (_hurt.atkSlot == (SkillSlotType)skillFuncParam && _hurt.atker.handle.SkillControl.TryGetSkillSlot(_hurt.atkSlot, out skillSlot))
				{
					int skillFuncParam2 = buffSkill.GetSkillFuncParam(72, 1, false);
					int skillFuncParam3 = buffSkill.GetSkillFuncParam(72, 2, false);
					int skillFuncParam4 = buffSkill.GetSkillFuncParam(72, 3, false);
					int skillFuncParam5 = buffSkill.GetSkillFuncParam(72, 4, false);
					logicEffect.SetSkillSlotUseTime(_hurt.atkSlot, skillFuncParam5);
					logicEffect.SetSkillSlotUseCount(_hurt.atkSlot, skillSlot.GetSkillUseCount());
					return logicEffect.DamageExtraEffect(ref _hurt, _hurtValue, skillFuncParam2, skillFuncParam3, skillFuncParam4);
				}
			}
			return _hurtValue;
		}
	}
}
