using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using ResData;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameLogic
{
	public class BuffHolderComponent : LogicComponent
	{
		public List<BuffSkill> SpawnedBuffList = new List<BuffSkill>();

		public BuffClearRule clearRule;

		public BuffOverlayRule overlayRule;

		public BuffProtectRule protectRule;

		public BuffChangeSkillRule changeSkillRule;

		public BufferMarkRule markRule;

		public BufferLogicEffect logicEffect;

		public bool bRemoveList = true;

		private List<BuffSkill> delBuffList = new List<BuffSkill>(3);

		public override void OnUse()
		{
			base.OnUse();
			this.SpawnedBuffList.Clear();
			this.overlayRule = null;
			this.clearRule = null;
			this.protectRule = null;
			this.changeSkillRule = null;
			this.markRule = null;
			this.logicEffect = null;
			this.bRemoveList = true;
			this.delBuffList.Clear();
		}

		public override void Init()
		{
			this.overlayRule = new BuffOverlayRule();
			this.clearRule = new BuffClearRule();
			this.protectRule = new BuffProtectRule();
			this.changeSkillRule = new BuffChangeSkillRule();
			this.markRule = new BufferMarkRule();
			this.logicEffect = new BufferLogicEffect();
			this.overlayRule.Init(this);
			this.clearRule.Init(this);
			this.protectRule.Init(this);
			this.changeSkillRule.Init(this);
			this.markRule.Init(this);
			this.logicEffect.Init(this);
			base.Init();
		}

		public override void Deactive()
		{
			this.ClearBuff();
			base.Deactive();
		}

		public override void Reactive()
		{
			base.Reactive();
			this.overlayRule.Init(this);
			this.clearRule.Init(this);
			this.protectRule.Init(this);
			this.changeSkillRule.Init(this);
			this.markRule.Init(this);
			this.logicEffect.Init(this);
		}

		public override void UpdateLogic(int nDelta)
		{
			if (this.markRule != null)
			{
				this.markRule.UpdateLogic(nDelta);
			}
		}

		public void AddBuff(BuffSkill inBuff)
		{
			this.SpawnedBuffList.Add(inBuff);
			this.protectRule.AddBuff(inBuff);
			this.logicEffect.AddBuff(inBuff);
			BuffChangeEventParam buffChangeEventParam = new BuffChangeEventParam(true, this.actorPtr, inBuff);
			Singleton<GameSkillEventSys>.GetInstance().SendEvent<BuffChangeEventParam>(GameSkillEventDef.AllEvent_BuffChange, this.actorPtr, ref buffChangeEventParam, GameSkillEventChannel.Channel_HostCtrlActor);
			if (inBuff.cfgData != null && inBuff.cfgData.bIsAssistEffect == 1 && inBuff.skillContext.Originator && this.actor.ValueComponent.actorHp > 0)
			{
				if (this.actor.TheActorMeta.ActorCamp == inBuff.skillContext.Originator.handle.TheActorMeta.ActorCamp)
				{
					this.actor.ActorControl.AddHelpSelfActor(inBuff.skillContext.Originator);
				}
				else
				{
					this.actor.ActorControl.AddHurtSelfActor(inBuff.skillContext.Originator);
				}
			}
		}

		public void ActionRemoveBuff(BuffSkill inBuff)
		{
			if (this.SpawnedBuffList.Remove(inBuff))
			{
				PoolObjHandle<BuffSkill> poolObjHandle = new PoolObjHandle<BuffSkill>(inBuff);
				this.protectRule.RemoveBuff(ref poolObjHandle);
				this.logicEffect.RemoveBuff(ref poolObjHandle);
				BuffChangeEventParam buffChangeEventParam = new BuffChangeEventParam(false, this.actorPtr, inBuff);
				Singleton<GameSkillEventSys>.GetInstance().SendEvent<BuffChangeEventParam>(GameSkillEventDef.AllEvent_BuffChange, this.actorPtr, ref buffChangeEventParam, GameSkillEventChannel.Channel_AllActor);
				inBuff.Release();
			}
		}

		public void RemoveBuff(BuffSkill inBuff)
		{
			if (this.SpawnedBuffList.Count == 0)
			{
				return;
			}
			this.delBuffList = this.SpawnedBuffList;
			for (int i = 0; i < this.delBuffList.Count; i++)
			{
				BuffSkill buffSkill = this.delBuffList[i];
				if (buffSkill == inBuff)
				{
					buffSkill.Stop();
					if (inBuff.cfgData.bEffectType == 2 && inBuff.cfgData.bShowType != 2 && this.actorPtr)
					{
						LimitMoveEventParam limitMoveEventParam = new LimitMoveEventParam(0, inBuff.SkillID, this.actorPtr);
						Singleton<GameSkillEventSys>.GetInstance().SendEvent<LimitMoveEventParam>(GameSkillEventDef.AllEvent_CancelLimitMove, this.actorPtr, ref limitMoveEventParam, GameSkillEventChannel.Channel_AllActor);
					}
				}
			}
		}

		public void RemoveBuff(int inSkillCombineId)
		{
			if (this.SpawnedBuffList.Count == 0)
			{
				return;
			}
			this.delBuffList = this.SpawnedBuffList;
			for (int i = 0; i < this.delBuffList.Count; i++)
			{
				BuffSkill buffSkill = this.delBuffList[i];
				if (buffSkill != null && buffSkill.SkillID == inSkillCombineId)
				{
					buffSkill.Stop();
					if (buffSkill.cfgData.bEffectType == 2 && buffSkill.cfgData.bShowType != 2 && this.actorPtr)
					{
						LimitMoveEventParam limitMoveEventParam = new LimitMoveEventParam(0, buffSkill.SkillID, this.actorPtr);
						Singleton<GameSkillEventSys>.GetInstance().SendEvent<LimitMoveEventParam>(GameSkillEventDef.AllEvent_CancelLimitMove, this.actorPtr, ref limitMoveEventParam, GameSkillEventChannel.Channel_AllActor);
					}
				}
			}
		}

		public void RemoveSkillEffectGroup(int inGroupID)
		{
			if (this.SpawnedBuffList.Count == 0)
			{
				return;
			}
			this.delBuffList = this.SpawnedBuffList;
			for (int i = 0; i < this.delBuffList.Count; i++)
			{
				BuffSkill buffSkill = this.delBuffList[i];
				if (buffSkill != null && buffSkill.cfgData != null && buffSkill.cfgData.iCroupID == inGroupID)
				{
					buffSkill.Stop();
					if (buffSkill.cfgData.bEffectType == 2 && buffSkill.cfgData.bShowType != 2 && this.actorPtr)
					{
						LimitMoveEventParam limitMoveEventParam = new LimitMoveEventParam(0, buffSkill.SkillID, this.actorPtr);
						Singleton<GameSkillEventSys>.GetInstance().SendEvent<LimitMoveEventParam>(GameSkillEventDef.AllEvent_CancelLimitMove, this.actorPtr, ref limitMoveEventParam, GameSkillEventChannel.Channel_AllActor);
					}
				}
			}
		}

		public void ClearEffectTypeBuff(int _typeMask)
		{
			if (this.SpawnedBuffList.Count == 0)
			{
				return;
			}
			this.delBuffList = this.SpawnedBuffList;
			for (int i = 0; i < this.delBuffList.Count; i++)
			{
				BuffSkill buffSkill = this.delBuffList[i];
				if ((_typeMask & 1 << (int)buffSkill.cfgData.bEffectType) > 0)
				{
					buffSkill.Stop();
				}
			}
			if (this.markRule != null)
			{
				this.markRule.ClearBufferMark(_typeMask);
			}
		}

		public void ClearBuff()
		{
			this.bRemoveList = false;
			for (int i = 0; i < this.SpawnedBuffList.Count; i++)
			{
				BuffSkill buffSkill = this.SpawnedBuffList[i];
				if (buffSkill != null)
				{
					buffSkill.Stop();
				}
			}
			if (this.protectRule != null)
			{
				this.protectRule.ClearBuff();
			}
			if (this.logicEffect != null)
			{
				this.logicEffect.ClearBuff();
			}
			for (int j = 0; j < this.SpawnedBuffList.Count; j++)
			{
				BuffSkill buffSkill = this.SpawnedBuffList[j];
				if (buffSkill != null)
				{
					buffSkill.Release();
				}
			}
			this.SpawnedBuffList.Clear();
			this.delBuffList.Clear();
			this.bRemoveList = true;
		}

		public BuffSkill FindBuff(int inSkillCombineId)
		{
			if (this.SpawnedBuffList != null)
			{
				for (int i = 0; i < this.SpawnedBuffList.Count; i++)
				{
					BuffSkill buffSkill = this.SpawnedBuffList[i];
					if (buffSkill != null && buffSkill.SkillID == inSkillCombineId)
					{
						return buffSkill;
					}
				}
			}
			return null;
		}

		public int FindBuffCount(int inSkillCombineId)
		{
			int num = 0;
			for (int i = 0; i < this.SpawnedBuffList.Count; i++)
			{
				BuffSkill buffSkill = this.SpawnedBuffList[i];
				if (buffSkill != null && buffSkill.SkillID == inSkillCombineId)
				{
					num++;
				}
			}
			return num;
		}

		public int GetSoulExpAddRate(PoolObjHandle<ActorRoot> _target)
		{
			int num = 0;
			ResDT_SkillFunc skillFunc = null;
			if (!_target)
			{
				return num;
			}
			for (int i = 0; i < this.SpawnedBuffList.Count; i++)
			{
				BuffSkill buffSkill = this.SpawnedBuffList[i];
				if (buffSkill != null && buffSkill.FindSkillFunc(49, out skillFunc))
				{
					int skillFuncParam = buffSkill.GetSkillFuncParam(skillFunc, 0, false);
					int skillFuncParam2 = buffSkill.GetSkillFuncParam(skillFunc, 1, false);
					int skillFuncParam3 = buffSkill.GetSkillFuncParam(skillFunc, 2, false);
					int skillFuncParam4 = buffSkill.GetSkillFuncParam(skillFunc, 3, false);
					if (this.CheckTargetSubType(skillFuncParam, skillFuncParam2, _target))
					{
						bool flag = true;
						if (skillFuncParam4 > 0)
						{
							flag = this.CheckTargetFromEnemy(this.actorPtr, _target);
						}
						if (flag)
						{
							num += skillFuncParam3;
						}
					}
				}
			}
			return num;
		}

		public int GetCoinAddRate(PoolObjHandle<ActorRoot> _target, bool bIsKiller = true)
		{
			int num = 0;
			ResDT_SkillFunc skillFunc = null;
			if (!_target)
			{
				return num;
			}
			for (int i = 0; i < this.SpawnedBuffList.Count; i++)
			{
				BuffSkill buffSkill = this.SpawnedBuffList[i];
				if (buffSkill != null && buffSkill.FindSkillFunc(71, out skillFunc))
				{
					int skillFuncParam = buffSkill.GetSkillFuncParam(skillFunc, 0, false);
					int skillFuncParam2 = buffSkill.GetSkillFuncParam(skillFunc, 1, false);
					int skillFuncParam3 = buffSkill.GetSkillFuncParam(skillFunc, 2, true);
					int skillFuncParam4 = buffSkill.GetSkillFuncParam(skillFunc, 3, false);
					if (buffSkill.GetSkillFuncParam(skillFunc, 4, false) != 0 || bIsKiller)
					{
						if (this.CheckTargetSubType(skillFuncParam, skillFuncParam2, _target))
						{
							bool flag = true;
							if (skillFuncParam4 > 0)
							{
								flag = this.CheckTargetFromEnemy(this.actorPtr, _target);
							}
							if (flag)
							{
								num += skillFuncParam3;
							}
						}
					}
				}
			}
			return num;
		}

		private int OnConditionExtraHurt(BuffSkill _buffSkill, PoolObjHandle<ActorRoot> _attack)
		{
			int result = 0;
			ResDT_SkillFunc skillFunc = null;
			if (_buffSkill != null && _buffSkill.FindSkillFunc(44, out skillFunc))
			{
				int skillFuncParam = _buffSkill.GetSkillFuncParam(skillFunc, 0, false);
				int skillFuncParam2 = _buffSkill.GetSkillFuncParam(skillFunc, 1, false);
				int skillFuncParam3 = _buffSkill.GetSkillFuncParam(skillFunc, 2, false);
				int skillFuncParam4 = _buffSkill.GetSkillFuncParam(skillFunc, 3, false);
				bool flag = skillFuncParam == 1;
				int num = (!flag) ? this.actor.ValueComponent.actorHp : _attack.handle.ValueComponent.actorHp;
				int num2 = (!flag) ? this.actor.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP].totalValue : _attack.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP].totalValue;
				int num3 = num * 10000 / num2;
				if (skillFuncParam3 == 1)
				{
					if (num3 <= skillFuncParam2)
					{
						result = skillFuncParam4;
					}
				}
				else if (skillFuncParam3 == 4 && num3 >= skillFuncParam2)
				{
					result = skillFuncParam4;
				}
			}
			return result;
		}

		private bool CheckTargetFromEnemy(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> target)
		{
			bool result = false;
			if (!src || !target)
			{
				return result;
			}
			if (src.handle.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Hero)
			{
				return result;
			}
			if (target.handle.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Monster)
			{
				return src.handle.TheActorMeta.ActorCamp != target.handle.TheActorMeta.ActorCamp;
			}
			MonsterWrapper monsterWrapper = target.handle.AsMonster();
			if (monsterWrapper != null)
			{
				RES_MONSTER_TYPE bMonsterType = (RES_MONSTER_TYPE)monsterWrapper.cfgInfo.bMonsterType;
				if (bMonsterType == RES_MONSTER_TYPE.RES_MONSTER_TYPE_SOLDIERLINE)
				{
					if (src.handle.TheActorMeta.ActorCamp != target.handle.TheActorMeta.ActorCamp)
					{
						result = true;
					}
				}
				else if (bMonsterType == RES_MONSTER_TYPE.RES_MONSTER_TYPE_JUNGLE)
				{
					byte actorSubSoliderType = monsterWrapper.GetActorSubSoliderType();
					if (actorSubSoliderType != 8 && actorSubSoliderType != 9 && actorSubSoliderType != 7 && actorSubSoliderType != 14)
					{
						long num = 0L;
						long num2 = 0L;
						VInt3 bornPos = target.handle.BornPos;
						List<PoolObjHandle<ActorRoot>> organActors = Singleton<GameObjMgr>.instance.OrganActors;
						int num3 = 0;
						for (int i = 0; i < organActors.Count; i++)
						{
							PoolObjHandle<ActorRoot> poolObjHandle = organActors[i];
							if (poolObjHandle.handle.TheStaticData.TheOrganOnlyInfo.OrganType == 2)
							{
								VInt3 location = poolObjHandle.handle.location;
								if (poolObjHandle.handle.TheActorMeta.ActorCamp == src.handle.TheActorMeta.ActorCamp)
								{
									num = (bornPos - location).sqrMagnitudeLong2D;
								}
								else
								{
									num2 = (bornPos - location).sqrMagnitudeLong2D;
								}
								num3++;
								if (num3 >= 2)
								{
									break;
								}
							}
						}
						if (num > num2)
						{
							result = true;
						}
					}
				}
			}
			return result;
		}

		public bool CheckTargetSubType(int typeMask, int typeSubMask, PoolObjHandle<ActorRoot> target)
		{
			if (typeMask == 0)
			{
				return true;
			}
			if (target)
			{
				int actorType = (int)target.handle.TheActorMeta.ActorType;
				if ((typeMask & 1 << actorType) > 0)
				{
					if (actorType != 1)
					{
						return true;
					}
					if (typeSubMask == 0)
					{
						return true;
					}
					int actorSubType = (int)target.handle.ActorControl.GetActorSubType();
					if (actorSubType == typeSubMask)
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool CheckTriggerCondtion(int conditionalType, int iParam, PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> target)
		{
			return conditionalType == 0 || conditionalType != 1 || this.CheckHpConditional(src, target, iParam);
		}

		public bool CheckHpConditional(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> target, int iParam)
		{
			if (!src || !target)
			{
				return false;
			}
			ulong propertyHpRate = TargetProperty.GetPropertyHpRate(src, RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP);
			ulong propertyHpRate2 = TargetProperty.GetPropertyHpRate(target, RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP);
			return SmartCompare.Compare<ulong>(propertyHpRate, propertyHpRate2, iParam);
		}

		private int OnTargetExtraHurt(BuffSkill _buffSkill, PoolObjHandle<ActorRoot> _attack)
		{
			int result = 0;
			ResDT_SkillFunc skillFunc = null;
			if (_buffSkill != null && _buffSkill.FindSkillFunc(48, out skillFunc))
			{
				int skillFuncParam = _buffSkill.GetSkillFuncParam(skillFunc, 0, false);
				int skillFuncParam2 = _buffSkill.GetSkillFuncParam(skillFunc, 1, false);
				int skillFuncParam3 = _buffSkill.GetSkillFuncParam(skillFunc, 2, false);
				if (this.CheckTargetSubType(skillFuncParam, skillFuncParam2, this.actorPtr))
				{
					result = skillFuncParam3;
				}
			}
			return result;
		}

		private int OnControlExtraHurt(BuffSkill _buffSkill, PoolObjHandle<ActorRoot> _attack)
		{
			int result = 0;
			ResDT_SkillFunc skillFunc = null;
			if (_buffSkill != null && _buffSkill.FindSkillFunc(51, out skillFunc) && this.actor != null)
			{
				for (int i = 0; i < this.actor.BuffHolderComp.SpawnedBuffList.Count; i++)
				{
					BuffSkill buffSkill = this.actor.BuffHolderComp.SpawnedBuffList[i];
					if (buffSkill != null && buffSkill.cfgData.bEffectType == 2)
					{
						result = _buffSkill.GetSkillFuncParam(skillFunc, 0, false);
						break;
					}
				}
			}
			return result;
		}

		public int GetExtraHurtOutputRate(PoolObjHandle<ActorRoot> _attack)
		{
			int num = 0;
			if (!_attack)
			{
				return 0;
			}
			for (int i = 0; i < _attack.handle.BuffHolderComp.SpawnedBuffList.Count; i++)
			{
				BuffSkill buffSkill = _attack.handle.BuffHolderComp.SpawnedBuffList[i];
				num += this.OnConditionExtraHurt(buffSkill, _attack);
				num += this.OnTargetExtraHurt(buffSkill, _attack);
				num += this.OnControlExtraHurt(buffSkill, _attack);
			}
			return num;
		}

		private bool OnChangeExtraEffectSkillSlot(PoolObjHandle<ActorRoot> _attack, SkillSlotType _slotType, out SkillSlotType _outSlotType)
		{
			ResDT_SkillFunc skillFunc = null;
			_outSlotType = _slotType;
			if (!_attack || _attack.handle.BuffHolderComp == null)
			{
				return false;
			}
			for (int i = 0; i < _attack.handle.BuffHolderComp.SpawnedBuffList.Count; i++)
			{
				BuffSkill buffSkill = _attack.handle.BuffHolderComp.SpawnedBuffList[i];
				if (buffSkill != null && buffSkill.FindSkillFunc(78, out skillFunc))
				{
					int skillFuncParam = buffSkill.GetSkillFuncParam(skillFunc, 0, false);
					int skillFuncParam2 = buffSkill.GetSkillFuncParam(skillFunc, 1, false);
					if (_slotType == (SkillSlotType)skillFuncParam)
					{
						_outSlotType = (SkillSlotType)skillFuncParam2;
						return true;
					}
				}
			}
			return false;
		}

		public bool CheckAttackTypeForDamageTriggerEffect(int typeMask, int monsterTypeMask, int soldierTypeMask, PoolObjHandle<ActorRoot> attacker)
		{
			if (typeMask == 0)
			{
				return true;
			}
			if (attacker)
			{
				int actorType = (int)attacker.handle.TheActorMeta.ActorType;
				if ((typeMask & 1 << actorType) > 0)
				{
					if (actorType != 1)
					{
						return true;
					}
					if (monsterTypeMask == 0)
					{
						return true;
					}
					int actorSubType = (int)attacker.handle.ActorControl.GetActorSubType();
					if (actorSubType == monsterTypeMask)
					{
						if (soldierTypeMask == 0)
						{
							return true;
						}
						int actorSubSoliderType = (int)attacker.handle.ActorControl.GetActorSubSoliderType();
						if (actorSubSoliderType == soldierTypeMask)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		private void OnDamageTriggerEffect(PoolObjHandle<ActorRoot> _attacker)
		{
			ResDT_SkillFunc skillFunc = null;
			if (!this.actorPtr || !_attacker || _attacker.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ)
			{
				return;
			}
			for (int i = 0; i < this.actorPtr.handle.BuffHolderComp.SpawnedBuffList.Count; i++)
			{
				BuffSkill buffSkill = this.actorPtr.handle.BuffHolderComp.SpawnedBuffList[i];
				if (buffSkill != null && buffSkill.FindSkillFunc(84, out skillFunc))
				{
					int skillFuncParam = buffSkill.GetSkillFuncParam(skillFunc, 3, false);
					int skillFuncParam2 = buffSkill.GetSkillFuncParam(skillFunc, 4, false);
					int skillFuncParam3 = buffSkill.GetSkillFuncParam(skillFunc, 5, false);
					if (this.CheckAttackTypeForDamageTriggerEffect(skillFuncParam, skillFuncParam2, skillFuncParam3, _attacker))
					{
						int skillFuncParam4 = buffSkill.GetSkillFuncParam(skillFunc, 0, false);
						int skillFuncParam5 = buffSkill.GetSkillFuncParam(skillFunc, 1, false);
						int skillFuncParam6 = buffSkill.GetSkillFuncParam(skillFunc, 2, false);
						if (skillFuncParam4 > 0)
						{
							SkillUseParam skillUseParam = default(SkillUseParam);
							PoolObjHandle<ActorRoot> inTargetActor = _attacker;
							skillUseParam.Init();
							skillUseParam.SetOriginator(this.actorPtr);
							skillUseParam.bExposing = buffSkill.skillContext.bExposing;
							skillUseParam.uiFromId = buffSkill.skillContext.uiFromId;
							skillUseParam.skillUseFrom = buffSkill.skillContext.skillUseFrom;
							if (skillFuncParam5 > 0 && _attacker.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Monster)
							{
								MonsterWrapper monsterWrapper = _attacker.handle.ActorControl as MonsterWrapper;
								if (monsterWrapper != null && monsterWrapper.isCalledMonster)
								{
									inTargetActor = monsterWrapper.hostActor;
								}
							}
							this.actorPtr.handle.SkillControl.SpawnBuff(inTargetActor, ref skillUseParam, skillFuncParam4, true);
						}
						if (skillFuncParam6 > 0)
						{
							int skillFuncParam7 = buffSkill.GetSkillFuncParam(skillFunc, 6, false);
							PoolObjHandle<ActorRoot> originator = this.actorPtr;
							PoolObjHandle<ActorRoot> inTargetActor2 = _attacker;
							if (skillFuncParam7 == 1)
							{
								originator = _attacker;
								inTargetActor2 = this.actorPtr;
							}
							SkillUseParam skillUseParam2 = default(SkillUseParam);
							skillUseParam2.Init();
							skillUseParam2.SetOriginator(originator);
							skillUseParam2.bExposing = buffSkill.skillContext.bExposing;
							skillUseParam2.uiFromId = buffSkill.skillContext.uiFromId;
							skillUseParam2.skillUseFrom = buffSkill.skillContext.skillUseFrom;
							originator.handle.SkillControl.SpawnBuff(inTargetActor2, ref skillUseParam2, skillFuncParam6, true);
						}
						if (buffSkill.GetSkillFuncParam(skillFunc, 7, false) == 0)
						{
							this.actorPtr.handle.BuffHolderComp.RemoveBuff(buffSkill);
						}
					}
				}
			}
		}

		private void OnDamageExtraEffect(PoolObjHandle<ActorRoot> _attack, SkillSlotType _slotType, SkillSlotType _extraEffectSlotType)
		{
			ResDT_SkillFunc skillFunc = null;
			if (!_attack)
			{
				return;
			}
			for (int i = 0; i < _attack.handle.BuffHolderComp.SpawnedBuffList.Count; i++)
			{
				BuffSkill buffSkill = _attack.handle.BuffHolderComp.SpawnedBuffList[i];
				if (buffSkill != null && buffSkill.FindSkillFunc(33, out skillFunc))
				{
					bool flag = false;
					bool flag2 = true;
					int skillFuncParam = buffSkill.GetSkillFuncParam(skillFunc, 0, false);
					int skillFuncParam2 = buffSkill.GetSkillFuncParam(skillFunc, 1, false);
					int skillFuncParam3 = buffSkill.GetSkillFuncParam(skillFunc, 2, false);
					int skillFuncParam4 = buffSkill.GetSkillFuncParam(skillFunc, 3, false);
					int skillFuncParam5 = buffSkill.GetSkillFuncParam(skillFunc, 4, false);
					int skillFuncParam6 = buffSkill.GetSkillFuncParam(skillFunc, 5, false);
					int skillFuncParam7 = buffSkill.GetSkillFuncParam(skillFunc, 6, false);
					int skillFuncParam8 = buffSkill.GetSkillFuncParam(skillFunc, 7, false);
					if (skillFuncParam3 == 0 && this.CheckTargetSubType(skillFuncParam4, skillFuncParam5, this.actorPtr))
					{
						if (skillFuncParam7 == 0 || this.CheckTriggerCondtion(skillFuncParam7, skillFuncParam8, _attack, this.actorPtr))
						{
							if (skillFuncParam2 == 0)
							{
								flag = true;
							}
							else if ((skillFuncParam2 & 1 << (int)_extraEffectSlotType) > 0)
							{
								flag = true;
							}
							if (skillFuncParam6 > 0)
							{
								flag2 = (Singleton<FrameSynchr>.GetInstance().LogicFrameTick - buffSkill.controlTime >= (ulong)((long)skillFuncParam6));
							}
							if (flag && flag2)
							{
								if (skillFuncParam6 != -2 || !buffSkill.IsNextDestroy())
								{
									SkillUseParam skillUseParam = default(SkillUseParam);
									skillUseParam.Init();
									skillUseParam.SetOriginator(_attack);
									skillUseParam.bExposing = buffSkill.skillContext.bExposing;
									skillUseParam.uiFromId = buffSkill.skillContext.uiFromId;
									skillUseParam.skillUseFrom = buffSkill.skillContext.skillUseFrom;
									if (buffSkill.skillContext != null)
									{
										if (skillUseParam.skillUseFrom != SKILL_USE_FROM_TYPE.SKILL_USE_FROM_TYPE_EQUIP)
										{
											skillUseParam.SlotType = _slotType;
										}
										else
										{
											skillUseParam.SlotType = buffSkill.skillContext.SlotType;
										}
									}
									else
									{
										skillUseParam.SlotType = _slotType;
									}
									_attack.handle.SkillControl.SpawnBuff(this.actorPtr, ref skillUseParam, skillFuncParam, true);
									buffSkill.controlTime = Singleton<FrameSynchr>.GetInstance().LogicFrameTick;
									if (skillFuncParam6 == -1 || skillFuncParam6 == -2)
									{
										_attack.handle.BuffHolderComp.RemoveBuff(buffSkill);
									}
								}
							}
						}
					}
				}
			}
		}

		public bool BuffImmuneDamage(ref HurtDataInfo _hurt)
		{
			return this.protectRule.ImmuneDamage(ref _hurt);
		}

		public int OnDamage(ref HurtDataInfo _hurt, int _hurtValue)
		{
			if (!_hurt.bLastHurt)
			{
				this.clearRule.CheckBuffClear(RES_SKILLFUNC_CLEAR_RULE.RES_SKILLFUNC_CLEAR_DAMAGE);
			}
			if (!_hurt.bExtraBuff)
			{
				SkillSlotType atkSlot = _hurt.atkSlot;
				SkillSlotType atkSlot2;
				bool flag = this.OnChangeExtraEffectSkillSlot(_hurt.atker, _hurt.atkSlot, out atkSlot2);
				if (flag)
				{
					_hurt.atkSlot = atkSlot2;
				}
				this.OnDamageTriggerEffect(_hurt.atker);
				this.OnDamageExtraEffect(_hurt.atker, _hurt.atkSlot, _hurt.ExtraEffectSlotType);
				if (flag)
				{
					_hurt.atkSlot = atkSlot;
				}
			}
			int num = _hurtValue * _hurt.iEffectFadeRate / 10000;
			num = num * _hurt.iOverlayFadeRate / 10000;
			num = this.protectRule.ResistDamage(ref _hurt, num);
			num = BufferLogicEffect.OnDamageExtraEffect(ref _hurt, num);
			num = this.DealDamageContionType(ref _hurt, num);
			this.OnDamageExtraHurtFunc(ref _hurt, _hurt.atkSlot);
			return num;
		}

		private int DealDamageContionType(ref HurtDataInfo _hurt, int _hurtValue)
		{
			if (!_hurt.atker)
			{
				return _hurtValue;
			}
			if (_hurt.iConditionType == 1)
			{
				int nAddHp = _hurtValue * _hurt.iConditionParam / 10000;
				_hurt.atker.handle.ActorControl.ReviveHp(nAddHp);
			}
			else if (_hurt.iConditionType == 2 && _hurt.target && _hurt.atker)
			{
				int magnitude2D = (_hurt.atker.handle.location - _hurt.target.handle.location).magnitude2D;
				int num = (int)((long)magnitude2D * (long)_hurtValue * (long)_hurt.iConditionParam / 10000000L);
				_hurtValue += num;
			}
			return _hurtValue;
		}

		public bool IsExistSkillFuncType(int inSkillFuncType)
		{
			ResDT_SkillFunc resDT_SkillFunc = null;
			for (int i = 0; i < this.SpawnedBuffList.Count; i++)
			{
				BuffSkill buffSkill = this.SpawnedBuffList[i];
				if (buffSkill != null && buffSkill.FindSkillFunc(inSkillFuncType, out resDT_SkillFunc))
				{
					return true;
				}
			}
			return false;
		}

		public void OnAssistEffect(ref PoolObjHandle<ActorRoot> deadActor)
		{
			ResDT_SkillFunc skillFunc = null;
			for (int i = 0; i < this.SpawnedBuffList.Count; i++)
			{
				BuffSkill buffSkill = this.SpawnedBuffList[i];
				if (buffSkill != null && buffSkill.FindSkillFunc(33, out skillFunc))
				{
					int skillFuncParam = buffSkill.GetSkillFuncParam(skillFunc, 0, false);
					int skillFuncParam2 = buffSkill.GetSkillFuncParam(skillFunc, 1, false);
					int skillFuncParam3 = buffSkill.GetSkillFuncParam(skillFunc, 2, false);
					int skillFuncParam4 = buffSkill.GetSkillFuncParam(skillFunc, 3, false);
					int skillFuncParam5 = buffSkill.GetSkillFuncParam(skillFunc, 4, false);
					if (skillFuncParam3 == 2 && this.CheckTargetSubType(skillFuncParam4, skillFuncParam5, deadActor))
					{
						SkillUseParam skillUseParam = default(SkillUseParam);
						skillUseParam.Init();
						skillUseParam.SetOriginator(this.actorPtr);
						skillUseParam.bExposing = buffSkill.skillContext.bExposing;
						skillUseParam.uiFromId = buffSkill.skillContext.uiFromId;
						skillUseParam.skillUseFrom = buffSkill.skillContext.skillUseFrom;
						this.actor.SkillControl.SpawnBuff(this.actorPtr, ref skillUseParam, skillFuncParam, true);
					}
				}
			}
		}

		private void OnDeadExtraEffect(PoolObjHandle<ActorRoot> _attack)
		{
			if (!_attack)
			{
				return;
			}
			ResDT_SkillFunc skillFunc = null;
			for (int i = 0; i < _attack.handle.BuffHolderComp.SpawnedBuffList.Count; i++)
			{
				BuffSkill buffSkill = _attack.handle.BuffHolderComp.SpawnedBuffList[i];
				if (buffSkill != null && buffSkill.FindSkillFunc(33, out skillFunc))
				{
					int skillFuncParam = buffSkill.GetSkillFuncParam(skillFunc, 0, false);
					int skillFuncParam2 = buffSkill.GetSkillFuncParam(skillFunc, 1, false);
					int skillFuncParam3 = buffSkill.GetSkillFuncParam(skillFunc, 2, false);
					int skillFuncParam4 = buffSkill.GetSkillFuncParam(skillFunc, 3, false);
					int skillFuncParam5 = buffSkill.GetSkillFuncParam(skillFunc, 4, false);
					if (skillFuncParam3 == 1 && this.CheckTargetSubType(skillFuncParam4, skillFuncParam5, this.actorPtr))
					{
						SkillUseParam skillUseParam = default(SkillUseParam);
						skillUseParam.Init();
						skillUseParam.SetOriginator(_attack);
						skillUseParam.bExposing = buffSkill.skillContext.bExposing;
						skillUseParam.uiFromId = buffSkill.skillContext.uiFromId;
						skillUseParam.skillUseFrom = buffSkill.skillContext.skillUseFrom;
						_attack.handle.SkillControl.SpawnBuff(_attack, ref skillUseParam, skillFuncParam, true);
					}
				}
			}
		}

		public void OnDead(PoolObjHandle<ActorRoot> _attack)
		{
			ResDT_SkillFunc skillFunc = null;
			if (this.clearRule != null)
			{
				this.clearRule.CheckBuffNoClear(RES_SKILLFUNC_CLEAR_RULE.RES_SKILLFUNC_CLEAR_DEAD);
			}
			if (this.logicEffect != null)
			{
				this.logicEffect.Clear();
			}
			if (this.actorPtr.handle.ActorControl.IsKilledByHero())
			{
				_attack = this.actorPtr.handle.ActorControl.LastHeroAtker;
			}
			int num = -1;
			int num2 = -1;
			for (int i = 0; i < this.SpawnedBuffList.Count; i++)
			{
				BuffSkill buffSkill = this.SpawnedBuffList[i];
				if (buffSkill != null && buffSkill.FindSkillFunc(32, out skillFunc))
				{
					int skillFuncParam = buffSkill.GetSkillFuncParam(skillFunc, 7, false);
					if (skillFuncParam > num2)
					{
						num2 = skillFuncParam;
						num = i;
					}
				}
			}
			for (int j = 0; j < this.SpawnedBuffList.Count; j++)
			{
				BuffSkill buffSkill = this.SpawnedBuffList[j];
				if (num == j && buffSkill != null && buffSkill.FindSkillFunc(32, out skillFunc))
				{
					int skillFuncParam2 = buffSkill.GetSkillFuncParam(skillFunc, 0, false);
					int skillFuncParam3 = buffSkill.GetSkillFuncParam(skillFunc, 1, false);
					int skillFuncParam4 = buffSkill.GetSkillFuncParam(skillFunc, 2, false);
					bool autoReset = (skillFuncParam4 & 1) == 1;
					bool bBaseRevive = buffSkill.GetSkillFuncParam(skillFunc, 3, false) == 0;
					bool bCDReset = buffSkill.GetSkillFuncParam(skillFunc, 4, false) == 1;
					int skillFuncParam5 = buffSkill.GetSkillFuncParam(skillFunc, 5, false);
					int skillFuncParam6 = buffSkill.GetSkillFuncParam(skillFunc, 6, false);
					bool bIsPassiveSkill = (skillFuncParam4 & 2) > 0;
					uint uiBuffObjId = 0u;
					if (buffSkill.skillContext != null && buffSkill.skillContext.Originator)
					{
						uiBuffObjId = buffSkill.skillContext.Originator.handle.ObjID;
					}
					this.actor.ActorControl.SetReviveContext(skillFuncParam2, skillFuncParam3, autoReset, bBaseRevive, bCDReset, skillFuncParam5, skillFuncParam6, bIsPassiveSkill, uiBuffObjId);
					this.RemoveBuff(buffSkill);
				}
				if (buffSkill != null && buffSkill.cfgData != null && buffSkill.cfgData.bIsInheritByKiller == 1)
				{
					this.RemoveBuff(buffSkill);
					if (_attack && _attack.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero && !_attack.handle.ActorControl.IsDeadState)
					{
						SkillUseParam skillUseParam = default(SkillUseParam);
						skillUseParam.SetOriginator(_attack);
						skillUseParam.bExposing = buffSkill.skillContext.bExposing;
						skillUseParam.uiFromId = buffSkill.skillContext.uiFromId;
						skillUseParam.skillUseFrom = buffSkill.skillContext.skillUseFrom;
						_attack.handle.SkillControl.SpawnBuff(_attack, ref skillUseParam, buffSkill.SkillID, true);
					}
				}
			}
			this.OnDeadExtraEffect(_attack);
			this.markRule.Clear();
		}

		public void OnDamageExtraValueEffect(ref HurtDataInfo _hurt, PoolObjHandle<ActorRoot> _attack, SkillSlotType _slotType)
		{
			ResDT_SkillFunc skillFunc = null;
			if (!_attack)
			{
				return;
			}
			for (int i = 0; i < _attack.handle.BuffHolderComp.SpawnedBuffList.Count; i++)
			{
				BuffSkill buffSkill = _attack.handle.BuffHolderComp.SpawnedBuffList[i];
				if (_hurt.hurtType == HurtTypeDef.Therapic)
				{
					if (buffSkill != null && buffSkill.FindSkillFunc(64, out skillFunc))
					{
						_hurt.iAddTotalHurtValueRate = _attack.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_PROPERTY_RECOVERYGAINEFFECT].addRatio;
						_hurt.iAddTotalHurtValue = _attack.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_PROPERTY_RECOVERYGAINEFFECT].addValue;
					}
				}
				else
				{
					if (_slotType == SkillSlotType.SLOT_SKILL_0 && _attack.handle.SkillControl != null && _attack.handle.SkillControl.bIsLastAtkUseSkill && buffSkill != null && buffSkill.FindSkillFunc(61, out skillFunc))
					{
						int skillFuncParam = buffSkill.GetSkillFuncParam(skillFunc, 0, false);
						int num = buffSkill.GetSkillFuncParam(skillFunc, 1, false);
						int skillFuncParam2 = buffSkill.GetSkillFuncParam(skillFunc, 2, false);
						int skillFuncParam3 = buffSkill.GetSkillFuncParam(skillFunc, 3, false);
						if (skillFuncParam == 1)
						{
							num = num * _hurt.hurtValue / 10000;
							_hurt.hurtValue += num;
							_hurt.adValue += skillFuncParam2;
							_hurt.apValue += skillFuncParam3;
						}
						else
						{
							_hurt.hurtValue += num;
							_hurt.attackInfo.iActorATT = _hurt.attackInfo.iActorATT + skillFuncParam2;
							_hurt.attackInfo.iActorINT = _hurt.attackInfo.iActorINT + skillFuncParam3;
						}
					}
					if (_hurt.target && _hurt.target.handle.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Organ && buffSkill != null && buffSkill.FindSkillFunc(68, out skillFunc))
					{
						int skillFuncParam4 = buffSkill.GetSkillFuncParam(skillFunc, 0, false);
						int num2 = buffSkill.GetSkillFuncParam(skillFunc, 4, false);
						int skillFuncParam5 = buffSkill.GetSkillFuncParam(skillFunc, 5, false);
						if (_hurt.target.handle.ValueComponent != null)
						{
							if (skillFuncParam4 == 1)
							{
								num2 = _hurt.target.handle.ValueComponent.actorHpTotal * num2 / 10000;
							}
							if (_hurt.target.handle.ValueComponent.actorHp <= num2 && _hurt.target.handle.ActorControl != null && Singleton<FrameSynchr>.instance.LogicFrameTick - _hurt.target.handle.ActorControl.lastExtraHurtByLowHpBuffTime >= (ulong)((long)skillFuncParam5))
							{
								_hurt.target.handle.ActorControl.lastExtraHurtByLowHpBuffTime = Singleton<FrameSynchr>.instance.LogicFrameTick;
								int num3 = buffSkill.GetSkillFuncParam(skillFunc, 1, false);
								int num4 = buffSkill.GetSkillFuncParam(skillFunc, 2, false);
								int num5 = buffSkill.GetSkillFuncParam(skillFunc, 3, false);
								if (skillFuncParam4 == 1)
								{
									num3 = num3 * _hurt.hurtValue / 10000;
									num4 = num4 * _hurt.adValue / 10000;
									num5 = num5 * _hurt.apValue / 10000;
								}
								_hurt.hurtValue += num3;
								_hurt.adValue += num4;
								_hurt.apValue += num5;
							}
						}
					}
				}
			}
		}

		public void OnDamageExtraHurtFunc(ref HurtDataInfo _hurt, SkillSlotType _slotType)
		{
			ResDT_SkillFunc skillFunc = null;
			for (int i = 0; i < this.SpawnedBuffList.Count; i++)
			{
				BuffSkill buffSkill = this.SpawnedBuffList[i];
				if (buffSkill != null && buffSkill.FindSkillFunc(80, out skillFunc))
				{
					int skillFuncParam = buffSkill.GetSkillFuncParam(skillFunc, 0, false);
					if (skillFuncParam == 0 || (skillFuncParam & 1 << (int)_slotType) > 0)
					{
						int skillFuncParam2 = buffSkill.GetSkillFuncParam(skillFunc, 2, false);
						if (skillFuncParam2 == 0 || skillFuncParam2 < (int)FrameRandom.Random(10000u))
						{
							int skillFuncParam3 = buffSkill.GetSkillFuncParam(skillFunc, 1, true);
							_hurt.iReduceDamage += skillFuncParam3;
							int skillFuncParam4 = buffSkill.GetSkillFuncParam(skillFunc, 3, false);
							if (skillFuncParam4 != 0)
							{
								SkillUseParam skillUseParam = default(SkillUseParam);
								skillUseParam.SetOriginator(_hurt.target);
								skillUseParam.uiFromId = buffSkill.skillContext.uiFromId;
								skillUseParam.skillUseFrom = SKILL_USE_FROM_TYPE.SKILL_USE_FROM_TYPE_PASSIVESKILL;
								_hurt.target.handle.SkillControl.SpawnBuff(_hurt.target, ref skillUseParam, skillFuncParam4, false);
							}
						}
					}
				}
			}
		}

		public int OnHurtBounceDamage(ref HurtDataInfo hurt, int hp)
		{
			if (hp <= 0)
			{
				return hp;
			}
			if (!hurt.atker || hurt.bBounceHurt)
			{
				return hp;
			}
			ResDT_SkillFunc skillFunc = null;
			int num = hp;
			for (int i = 0; i < this.SpawnedBuffList.Count; i++)
			{
				BuffSkill buffSkill = this.SpawnedBuffList[i];
				if (buffSkill != null && buffSkill.FindSkillFunc(83, out skillFunc))
				{
					int skillFuncParam = buffSkill.GetSkillFuncParam(skillFunc, 2, false);
					if ((skillFuncParam & 1 << (int)hurt.atker.handle.TheActorMeta.ActorType) <= 0)
					{
						int skillFuncParam2 = buffSkill.GetSkillFuncParam(skillFunc, 4, false);
						if (skillFuncParam2 <= 0 || (skillFuncParam2 & 1 << (int)hurt.hurtType) > 0)
						{
							int skillFuncParam3 = buffSkill.GetSkillFuncParam(skillFunc, 0, false);
							int skillFuncParam4 = buffSkill.GetSkillFuncParam(skillFunc, 1, false);
							int skillFuncParam5 = buffSkill.GetSkillFuncParam(skillFunc, 3, false);
							int num2 = num * skillFuncParam3 / 10000;
							if (skillFuncParam5 == 1)
							{
								num -= num2;
							}
							HurtDataInfo hurtDataInfo = default(HurtDataInfo);
							HurtAttackerInfo attackInfo = default(HurtAttackerInfo);
							attackInfo.Init(hurt.target, hurt.atker);
							hurtDataInfo.atker = hurt.target;
							hurtDataInfo.target = hurt.atker;
							hurtDataInfo.attackInfo = attackInfo;
							hurtDataInfo.atkSlot = SkillSlotType.SLOT_SKILL_VALID;
							hurtDataInfo.ExtraEffectSlotType = SkillSlotType.SLOT_SKILL_VALID;
							hurtDataInfo.hurtType = (HurtTypeDef)skillFuncParam4;
							hurtDataInfo.extraHurtType = ExtraHurtTypeDef.ExtraHurt_Value;
							hurtDataInfo.hurtValue = num2;
							hurtDataInfo.adValue = 0;
							hurtDataInfo.apValue = 0;
							hurtDataInfo.hpValue = 0;
							hurtDataInfo.loseHpValue = 0;
							hurtDataInfo.iConditionType = 0;
							hurtDataInfo.iConditionParam = 0;
							hurtDataInfo.hurtCount = 0;
							hurtDataInfo.firstHemoFadeRate = 0;
							hurtDataInfo.followUpHemoFadeRate = 0;
							hurtDataInfo.iEffectCountInSingleTrigger = 0;
							hurtDataInfo.bExtraBuff = false;
							hurtDataInfo.gatherTime = 0;
							hurtDataInfo.bBounceHurt = true;
							hurtDataInfo.bLastHurt = false;
							hurtDataInfo.iAddTotalHurtValueRate = 0;
							hurtDataInfo.iAddTotalHurtValue = 0;
							hurtDataInfo.iEffectFadeRate = 10000;
							hurtDataInfo.iOverlayFadeRate = 10000;
							SkillUseContext skillUseContext = buffSkill.GetSkillUseContext();
							if (skillUseContext != null)
							{
								hurtDataInfo.SkillUseFrom = skillUseContext.skillUseFrom;
								hurtDataInfo.uiFromId = skillUseContext.uiFromId;
							}
							int num3 = hurt.atker.handle.ActorControl.TakeBouncesDamage(ref hurtDataInfo);
						}
					}
				}
			}
			return num;
		}
	}
}
