using Assets.Scripts.Common;
using ResData;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameLogic
{
	public class BuffProtectRule
	{
		public const int PROTECT_PARAM_INDEX_1 = 4;

		public const int PROTECT_PARAM_INDEX_2 = 5;

		private int protectValue;

		private BuffHolderComponent buffHolder;

		private List<PoolObjHandle<BuffSkill>> PhysicsProtectList = new List<PoolObjHandle<BuffSkill>>();

		private List<PoolObjHandle<BuffSkill>> MagicProtectList = new List<PoolObjHandle<BuffSkill>>();

		private List<PoolObjHandle<BuffSkill>> AllProtectBuffList = new List<PoolObjHandle<BuffSkill>>();

		private List<PoolObjHandle<BuffSkill>> AllIncludeRealHurtProtectBuffList = new List<PoolObjHandle<BuffSkill>>();

		private List<PoolObjHandle<BuffSkill>> NoHurtBuffList = new List<PoolObjHandle<BuffSkill>>();

		private BuffLimiteHurt limiteMaxHpHurt;

		private uint m_uiProtectTotalValue;

		private uint m_uiProtectValueFromHero;

		private uint m_uiBeProtectedTotalValue;

		private uint m_uiBeProtectedValueToHeroPhys;

		private uint m_uiBeProtectedValueToHeroMagic;

		private uint m_uiBeProtectedValueToHeroReal;

		public uint ProtectValueFromHero
		{
			get
			{
				return this.m_uiProtectValueFromHero;
			}
		}

		public uint BePortectedTotalValue
		{
			get
			{
				return this.m_uiBeProtectedTotalValue;
			}
			set
			{
				this.m_uiBeProtectedTotalValue = value;
			}
		}

		public uint BeProtectedValueToHeroPhys
		{
			get
			{
				return this.m_uiBeProtectedValueToHeroPhys;
			}
			set
			{
				this.m_uiBeProtectedValueToHeroPhys = value;
			}
		}

		public uint BeProtectedValueToHeroMagic
		{
			get
			{
				return this.m_uiBeProtectedValueToHeroMagic;
			}
			set
			{
				this.m_uiBeProtectedValueToHeroMagic = value;
			}
		}

		public uint BeProtectedValueToHeroReal
		{
			get
			{
				return this.m_uiBeProtectedValueToHeroReal;
			}
			set
			{
				this.m_uiBeProtectedValueToHeroReal = value;
			}
		}

		public void Init(BuffHolderComponent _buffHolder)
		{
			this.protectValue = 0;
			this.buffHolder = _buffHolder;
			this.limiteMaxHpHurt.bValid = false;
			this.m_uiProtectTotalValue = 0u;
			this.m_uiProtectValueFromHero = 0u;
			this.m_uiBeProtectedTotalValue = 0u;
			this.m_uiBeProtectedValueToHeroPhys = 0u;
			this.m_uiBeProtectedValueToHeroMagic = 0u;
			this.m_uiBeProtectedValueToHeroReal = 0u;
		}

		public void SetLimiteMaxHurt(bool _bOpen, int _value)
		{
			this.limiteMaxHpHurt.bValid = _bOpen;
			this.limiteMaxHpHurt.hurtRate = _value;
		}

		public void SendProtectEvent(int type, int changeValue)
		{
			if (changeValue != 0)
			{
				this.protectValue += changeValue;
				this.buffHolder.actor.ActorControl.OnShieldChange(type, changeValue);
				if (this.protectValue == 0)
				{
					ActorSkillEventParam actorSkillEventParam = new ActorSkillEventParam(this.buffHolder.actorPtr, SkillSlotType.SLOT_SKILL_0);
					Singleton<GameSkillEventSys>.GetInstance().SendEvent<ActorSkillEventParam>(GameSkillEventDef.Event_ProtectDisappear, this.buffHolder.actorPtr, ref actorSkillEventParam, GameSkillEventChannel.Channel_HostCtrlActor);
				}
			}
		}

		private void SendHurtAbsorbEvent(PoolObjHandle<ActorRoot> atker, int changeValue)
		{
			if (changeValue > 0 && this.protectValue != 0)
			{
				DefaultGameEventParam defaultGameEventParam = new DefaultGameEventParam(this.buffHolder.actorPtr, atker);
				Singleton<GameEventSys>.instance.SendEvent<DefaultGameEventParam>(GameEventDef.Event_ActorHurtAbsorb, ref defaultGameEventParam);
			}
		}

		private void SendHurtImmuneEvent(PoolObjHandle<ActorRoot> atker)
		{
			DefaultGameEventParam defaultGameEventParam = new DefaultGameEventParam(this.buffHolder.actorPtr, atker);
			Singleton<GameEventSys>.instance.SendEvent<DefaultGameEventParam>(GameEventDef.Event_ActorImmune, ref defaultGameEventParam);
		}

		public void AddBuff(BuffSkill inBuff)
		{
			if (inBuff.cfgData.bEffectSubType == 1)
			{
				this.PhysicsProtectList.Add(new PoolObjHandle<BuffSkill>(inBuff));
			}
			else if (inBuff.cfgData.bEffectSubType == 2)
			{
				this.MagicProtectList.Add(new PoolObjHandle<BuffSkill>(inBuff));
			}
			else if (inBuff.cfgData.bEffectSubType == 3)
			{
				this.AllProtectBuffList.Add(new PoolObjHandle<BuffSkill>(inBuff));
			}
			else if (inBuff.cfgData.bEffectSubType == 11)
			{
				this.AllIncludeRealHurtProtectBuffList.Add(new PoolObjHandle<BuffSkill>(inBuff));
			}
			else if (inBuff.cfgData.bEffectSubType == 4 || inBuff.cfgData.bEffectSubType == 5 || inBuff.cfgData.bEffectSubType == 6)
			{
				this.NoHurtBuffList.Add(new PoolObjHandle<BuffSkill>(inBuff));
			}
		}

		public void ClearBuff()
		{
			this.ClearProtectBuff(this.PhysicsProtectList);
			this.ClearProtectBuff(this.MagicProtectList);
			this.ClearProtectBuff(this.AllProtectBuffList);
			this.ClearProtectBuff(this.AllIncludeRealHurtProtectBuffList);
			this.NoHurtBuffList.Clear();
		}

		private void RemoveProtectSpawnSkillEffect(ref PoolObjHandle<BuffSkill> inBuff)
		{
			if (inBuff.handle.CustomParams[0] > 0 || inBuff.handle.CustomParams[1] > 0 || inBuff.handle.CustomParams[2] > 0 || inBuff.handle.CustomParams[3] > 0)
			{
				this.SpawnSkillEffect(inBuff.handle.CustomParams[5], inBuff.handle.SlotType, inBuff.handle.skillContext.bExposing);
			}
		}

		public void RemoveBuff(ref PoolObjHandle<BuffSkill> inBuff)
		{
			if (inBuff.handle.cfgData.bEffectSubType == 1)
			{
				this.SendProtectEvent(0, -inBuff.handle.CustomParams[0]);
				this.PhysicsProtectList.Remove(inBuff);
				this.RemoveProtectSpawnSkillEffect(ref inBuff);
			}
			else if (inBuff.handle.cfgData.bEffectSubType == 2)
			{
				this.SendProtectEvent(1, -inBuff.handle.CustomParams[1]);
				this.MagicProtectList.Remove(inBuff);
				this.RemoveProtectSpawnSkillEffect(ref inBuff);
			}
			else if (inBuff.handle.cfgData.bEffectSubType == 3)
			{
				this.SendProtectEvent(2, -inBuff.handle.CustomParams[2]);
				this.AllProtectBuffList.Remove(inBuff);
				this.RemoveProtectSpawnSkillEffect(ref inBuff);
			}
			else if (inBuff.handle.cfgData.bEffectSubType == 11)
			{
				this.SendProtectEvent(3, -inBuff.handle.CustomParams[3]);
				this.AllIncludeRealHurtProtectBuffList.Remove(inBuff);
				this.RemoveProtectSpawnSkillEffect(ref inBuff);
			}
			else if (inBuff.handle.cfgData.bEffectSubType == 4 || inBuff.handle.cfgData.bEffectSubType == 5 || inBuff.handle.cfgData.bEffectSubType == 6)
			{
				this.NoHurtBuffList.Remove(inBuff);
			}
		}

		private void ClearProtectBuff(List<PoolObjHandle<BuffSkill>> _inList)
		{
			if (_inList.get_Count() == 0)
			{
				return;
			}
			PoolObjHandle<BuffSkill>[] array = _inList.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				this.RemoveBuff(ref array[i]);
			}
		}

		private void SpawnSkillEffect(int _skillCombineID, SkillSlotType _slotType, bool inExposing)
		{
			if (this.buffHolder.actorPtr)
			{
				SkillUseParam skillUseParam = default(SkillUseParam);
				skillUseParam.SlotType = _slotType;
				skillUseParam.SetOriginator(this.buffHolder.actorPtr);
				skillUseParam.bExposing = inExposing;
				this.buffHolder.actorPtr.handle.SkillControl.SpawnBuff(this.buffHolder.actorPtr, ref skillUseParam, _skillCombineID, true);
			}
		}

		private int ResistProtectImpl(int _hurtValue, List<PoolObjHandle<BuffSkill>> _inList, int _index)
		{
			if (_inList.get_Count() == 0)
			{
				return _hurtValue;
			}
			PoolObjHandle<BuffSkill>[] array = _inList.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				BuffSkill handle = array[i].handle;
				if (handle.CustomParams[_index] > _hurtValue)
				{
					handle.CustomParams[_index] -= _hurtValue;
					return 0;
				}
				_hurtValue -= handle.CustomParams[_index];
				handle.CustomParams[_index] = 0;
				this.SpawnSkillEffect(handle.CustomParams[4], handle.SlotType, handle.skillContext.bExposing);
				this.buffHolder.RemoveBuff(handle);
				_inList.Remove(array[i]);
			}
			return _hurtValue;
		}

		private bool CheckTargetNoDamage(ref HurtDataInfo _hurt, BuffSkill _buffSkill)
		{
			int skillFuncParam = _buffSkill.GetSkillFuncParam(30, 1, false);
			int skillFuncParam2 = _buffSkill.GetSkillFuncParam(30, 2, false);
			if (skillFuncParam == 0)
			{
				return true;
			}
			if (_hurt.atker)
			{
				int actorType = (int)_hurt.atker.handle.TheActorMeta.ActorType;
				if ((skillFuncParam & 1 << actorType) > 0)
				{
					if (actorType != 1)
					{
						return true;
					}
					if (skillFuncParam2 == 0)
					{
						return true;
					}
					int actorSubType = (int)_hurt.atker.handle.ActorControl.GetActorSubType();
					if (actorSubType == skillFuncParam2)
					{
						return true;
					}
				}
			}
			return false;
		}

		private bool NoDamageImpl(ref HurtDataInfo _hurt)
		{
			for (int i = 0; i < this.NoHurtBuffList.get_Count(); i++)
			{
				BuffSkill buffSkill = this.NoHurtBuffList.get_Item(i);
				if (buffSkill != null)
				{
					if (buffSkill.cfgData.bEffectSubType == 6)
					{
						if (this.CheckTargetNoDamage(ref _hurt, buffSkill))
						{
							return true;
						}
					}
					else if (_hurt.hurtType == HurtTypeDef.PhysHurt)
					{
						if (buffSkill.cfgData.bEffectSubType == 4 && this.CheckTargetNoDamage(ref _hurt, buffSkill))
						{
							return true;
						}
					}
					else if (_hurt.hurtType == HurtTypeDef.MagicHurt && buffSkill.cfgData.bEffectSubType == 5 && this.CheckTargetNoDamage(ref _hurt, buffSkill))
					{
						return true;
					}
				}
			}
			return false;
		}

		private int ResistDeadDamage(ref HurtDataInfo _hurt, int _hurtValue)
		{
			ResDT_SkillFunc skillFunc = null;
			if (this.buffHolder == null || this.buffHolder.actor == null)
			{
				return _hurtValue;
			}
			ActorRoot actor = this.buffHolder.actor;
			for (int i = 0; i < actor.BuffHolderComp.SpawnedBuffList.get_Count(); i++)
			{
				BuffSkill buffSkill = actor.BuffHolderComp.SpawnedBuffList.get_Item(i);
				if (buffSkill != null && buffSkill.FindSkillFunc(54, out skillFunc))
				{
					int skillFuncParam = buffSkill.GetSkillFuncParam(skillFunc, 0, false);
					if (buffSkill.GetSkillFuncParam(skillFunc, 1, false) == 0)
					{
						if (actor.ValueComponent.actorHp <= _hurtValue)
						{
							SkillUseParam skillUseParam = default(SkillUseParam);
							skillUseParam.SetOriginator(_hurt.atker);
							skillUseParam.bExposing = buffSkill.skillContext.bExposing;
							skillUseParam.uiFromId = buffSkill.skillContext.uiFromId;
							skillUseParam.skillUseFrom = buffSkill.skillContext.skillUseFrom;
							actor.SkillControl.SpawnBuff(actor.SelfPtr, ref skillUseParam, skillFuncParam, true);
							this.buffHolder.RemoveBuff(buffSkill);
							DefaultGameEventParam defaultGameEventParam = new DefaultGameEventParam(this.buffHolder.actorPtr, _hurt.atker);
							Singleton<GameEventSys>.instance.SendEvent<DefaultGameEventParam>(GameEventDef.Event_ActorImmuneDeadHurt, ref defaultGameEventParam);
							_hurtValue = 0;
						}
					}
					else
					{
						SkillUseParam skillUseParam2 = default(SkillUseParam);
						skillUseParam2.SetOriginator(_hurt.atker);
						skillUseParam2.bExposing = buffSkill.skillContext.bExposing;
						skillUseParam2.uiFromId = buffSkill.skillContext.uiFromId;
						skillUseParam2.skillUseFrom = buffSkill.skillContext.skillUseFrom;
						actor.SkillControl.SpawnBuff(actor.SelfPtr, ref skillUseParam2, skillFuncParam, true);
						this.buffHolder.RemoveBuff(buffSkill);
						_hurtValue = 0;
					}
				}
				if (_hurt.atkSlot == SkillSlotType.SLOT_SKILL_0 && buffSkill != null && buffSkill.FindSkillFunc(67, out skillFunc) && _hurt.atker && _hurt.atker.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero && _hurt.hurtType != HurtTypeDef.RealHurt)
				{
					int skillFuncParam2 = buffSkill.GetSkillFuncParam(skillFunc, 0, false);
					int skillFuncParam3 = buffSkill.GetSkillFuncParam(skillFunc, 4, false);
					if (skillFuncParam2 == 1)
					{
						_hurtValue = _hurtValue * (10000 - skillFuncParam3) / 10000;
					}
					else if (skillFuncParam2 == 0)
					{
						_hurtValue -= skillFuncParam3;
					}
				}
			}
			return _hurtValue;
		}

		public bool ImmuneDamage(ref HurtDataInfo _hurt)
		{
			if (this.NoDamageImpl(ref _hurt))
			{
				this.SendHurtImmuneEvent(_hurt.atker);
				return true;
			}
			return false;
		}

		public int ResistDamage(ref HurtDataInfo _hurt, int _hurtValue)
		{
			int num = _hurtValue;
			if (_hurtValue <= 0)
			{
				return _hurtValue;
			}
			int num2;
			if (_hurt.hurtType == HurtTypeDef.PhysHurt)
			{
				num2 = _hurtValue;
				_hurtValue = this.ResistProtectImpl(_hurtValue, this.PhysicsProtectList, 0);
				num2 -= _hurtValue;
				this.SendProtectEvent(0, -num2);
				if (_hurtValue > 0)
				{
					num2 = _hurtValue;
					_hurtValue = this.ResistProtectImpl(_hurtValue, this.AllProtectBuffList, 2);
					num2 -= _hurtValue;
					this.SendProtectEvent(2, -num2);
				}
			}
			else if (_hurt.hurtType == HurtTypeDef.MagicHurt)
			{
				num2 = _hurtValue;
				_hurtValue = this.ResistProtectImpl(_hurtValue, this.MagicProtectList, 1);
				num2 -= _hurtValue;
				this.SendProtectEvent(1, -num2);
				if (_hurtValue > 0)
				{
					num2 = _hurtValue;
					_hurtValue = this.ResistProtectImpl(_hurtValue, this.AllProtectBuffList, 2);
					num2 -= _hurtValue;
					this.SendProtectEvent(2, -num2);
				}
			}
			if (_hurtValue > 0)
			{
				num2 = _hurtValue;
				_hurtValue = this.ResistProtectImpl(_hurtValue, this.AllIncludeRealHurtProtectBuffList, 3);
				num2 -= _hurtValue;
				this.SendProtectEvent(3, -num2);
			}
			num2 = num - _hurtValue;
			this.SendHurtAbsorbEvent(_hurt.atker, num2);
			this.StatProtectValue(ref _hurt, num2);
			if (this.limiteMaxHpHurt.bValid)
			{
				int num3 = this.buffHolder.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP].totalValue;
				num3 = num3 * this.limiteMaxHpHurt.hurtRate / 10000;
				if (_hurtValue > num3)
				{
					_hurtValue = num3;
				}
			}
			_hurtValue = this.ResistDeadDamage(ref _hurt, _hurtValue);
			return _hurtValue;
		}

		private void StatProtectValue(ref HurtDataInfo hurt, int iChangeValue)
		{
			if (iChangeValue <= 0)
			{
				return;
			}
			this.m_uiProtectTotalValue += (uint)iChangeValue;
			if (hurt.atker)
			{
				if (hurt.atker.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
				{
					this.m_uiProtectValueFromHero += (uint)iChangeValue;
				}
				if (hurt.atker.handle.BuffHolderComp != null && hurt.atker.handle.BuffHolderComp.protectRule != null)
				{
					hurt.atker.handle.BuffHolderComp.protectRule.BePortectedTotalValue += (uint)iChangeValue;
					if (hurt.target && hurt.target.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
					{
						if (hurt.hurtType == HurtTypeDef.PhysHurt)
						{
							hurt.atker.handle.BuffHolderComp.protectRule.BeProtectedValueToHeroPhys += (uint)iChangeValue;
						}
						else if (hurt.hurtType == HurtTypeDef.MagicHurt)
						{
							hurt.atker.handle.BuffHolderComp.protectRule.BeProtectedValueToHeroMagic += (uint)iChangeValue;
						}
						else if (hurt.hurtType == HurtTypeDef.RealHurt)
						{
							hurt.atker.handle.BuffHolderComp.protectRule.BeProtectedValueToHeroReal += (uint)iChangeValue;
						}
					}
				}
			}
		}

		public uint GetProtectTotalValue()
		{
			return this.m_uiProtectTotalValue;
		}
	}
}
