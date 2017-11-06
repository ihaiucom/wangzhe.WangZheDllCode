using AGE;
using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using ResData;
using System;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public class BuffSkill : BaseSkill
	{
		public const int MaxCustomParam = 6;

		public SkillSlotType SlotType;

		private int overlayCount;

		public int[] CustomParams = new int[6];

		public ulong controlTime;

		public ResSkillCombineCfgInfo cfgData;

		public bool bExtraBuff;

		private ResBattleParam battleParam;

		private int buffLevel = 1;

		public ulong ulStartTime;

		public int iBuffWorkTimes;

		public bool bFirstEffect;

		private bool nextDestroy;

		public override bool isBuff
		{
			get
			{
				return true;
			}
		}

		public string SkillFuncCombineName
		{
			get
			{
				return Utility.UTF8Convert(this.cfgData.szSkillCombineName);
			}
		}

		public string SkillFuncCombineDesc
		{
			get
			{
				return Utility.UTF8Convert(this.cfgData.szSkillCombineDesc);
			}
		}

		public void SetBuffLevel(int _level)
		{
			this.buffLevel = _level;
		}

		public int GetBuffLevel()
		{
			return this.buffLevel;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.buffLevel = 1;
			this.overlayCount = 0;
			this.controlTime = 0uL;
			Array.Clear(this.CustomParams, 0, this.CustomParams.Length);
			this.bExtraBuff = false;
			this.cfgData = null;
			this.battleParam = null;
			this.ulStartTime = 0uL;
			this.iBuffWorkTimes = 0;
			this.bFirstEffect = false;
			this.nextDestroy = false;
			this.SlotType = SkillSlotType.SLOT_SKILL_0;
		}

		public override void OnRelease()
		{
			this.buffLevel = 1;
			this.overlayCount = 0;
			Array.Clear(this.CustomParams, 0, this.CustomParams.Length);
			this.bExtraBuff = false;
			this.cfgData = null;
			this.battleParam = null;
			this.nextDestroy = false;
			base.OnRelease();
		}

		public bool IsNextDestroy()
		{
			return this.nextDestroy;
		}

		public int GetOverlayCount()
		{
			return this.overlayCount;
		}

		public void SetOverlayCount(int _count)
		{
			this.overlayCount = _count;
		}

		public override void Stop()
		{
			this.nextDestroy = true;
			base.Stop();
		}

		public void Init(int id)
		{
			this.SkillID = id;
			this.cfgData = GameDataMgr.skillCombineDatabin.GetDataByKey((long)id);
			if (this.cfgData != null)
			{
				this.ActionName = StringHelper.UTF8BytesToString(ref this.cfgData.szPrefab);
				this.bAgeImmeExcute = (this.cfgData.bAgeImmeExcute == 1);
			}
			for (int i = 0; i < 6; i++)
			{
				this.CustomParams[i] = 0;
			}
			this.controlTime = 0uL;
			this.battleParam = GameDataMgr.battleParam.GetAnyData();
			this.ulStartTime = Singleton<FrameSynchr>.GetInstance().LogicFrameTick;
		}

		public override void OnActionStoped(ref PoolObjHandle<Action> action)
		{
			PoolObjHandle<ActorRoot> ptr = default(PoolObjHandle<ActorRoot>);
			action.handle.refParams.GetRefParam("TargetActor", ref ptr);
			if (ptr && ptr.handle.BuffHolderComp != null && ptr.handle.BuffHolderComp.bRemoveList)
			{
				ptr.handle.BuffHolderComp.ActionRemoveBuff(this);
			}
			base.OnActionStoped(ref action);
		}

		private bool CheckUseRule(SkillUseContext context)
		{
			return (!context.TargetActor.handle.ActorControl.GetNoAbilityFlag(ObjAbilityType.ObjAbility_ImmuneNegative) || (this.cfgData.bEffectType != 1 && this.cfgData.bEffectType != 2)) && (!context.TargetActor.handle.ActorControl.GetNoAbilityFlag(ObjAbilityType.ObjAbility_ImmuneControl) || this.cfgData.bEffectType != 2);
		}

		private void DealTenacity(PoolObjHandle<ActorRoot> target)
		{
			int num = 0;
			ValueDataInfo valueDataInfo = target.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_PROPERTY_TENACITY];
			int totalValue = valueDataInfo.totalValue;
			int num2 = totalValue + target.handle.ValueComponent.mActorValue.actorLvl * (int)this.battleParam.dwM_Tenacity + (int)this.battleParam.dwN_Tenacity;
			if (num2 != 0)
			{
				num = totalValue * 10000 / num2;
			}
			num += target.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_CTRLREDUCE].totalValue;
			int num3 = this.curAction.handle.length;
			if (num != 0)
			{
				num3 = num3 * (10000 - num) / 10000;
				this.curAction.handle.ResetLength(num3, false);
			}
		}

		private bool UseImpl(PoolObjHandle<ActorRoot> user)
		{
			if (this.skillContext == null || !this.skillContext.TargetActor || this.cfgData == null)
			{
				return false;
			}
			BuffHolderComponent buffHolderComp = this.skillContext.TargetActor.handle.BuffHolderComp;
			if (buffHolderComp == null)
			{
				return false;
			}
			if (!this.CheckUseRule(this.skillContext))
			{
				return false;
			}
			if (!buffHolderComp.overlayRule.CheckOverlay(this))
			{
				return false;
			}
			bool flag = false;
			bool flag2 = false;
			VInt3 value = VInt3.forward;
			switch (this.skillContext.AppointType)
			{
			case SkillRangeAppointType.Pos:
				flag = true;
				break;
			case SkillRangeAppointType.Directional:
				flag2 = true;
				value = this.skillContext.UseVector;
				if (this.skillContext.TargetID != 0u)
				{
					PoolObjHandle<ActorRoot> actor = Singleton<GameObjMgr>.GetInstance().GetActor(this.skillContext.TargetID);
					if (actor)
					{
						VInt3 vInt = actor.handle.location - user.handle.location;
						vInt.y = 0;
						vInt.Normalize();
						value = vInt;
					}
				}
				break;
			case SkillRangeAppointType.Track:
				flag = true;
				flag2 = true;
				value = this.skillContext.EndVector - this.skillContext.UseVector;
				if (value.sqrMagnitudeLong < 1L)
				{
					value = VInt3.forward;
				}
				break;
			}
			GameObject gameObject = this.skillContext.Originator ? this.skillContext.Originator.handle.gameObject : null;
			GameObject gameObject2 = this.skillContext.TargetActor ? this.skillContext.TargetActor.handle.gameObject : null;
			this.curAction = new PoolObjHandle<Action>(ActionManager.Instance.PlayAction(this.ActionName, true, false, new GameObject[]
			{
				gameObject,
				gameObject2
			}));
			if (!this.curAction)
			{
				return false;
			}
			this.curAction.handle.onActionStop += new ActionStopDelegate(this.OnActionStoped);
			this.curAction.handle.refParams.AddRefParam("SkillObj", this);
			this.curAction.handle.refParams.AddRefParam("SkillContext", this.skillContext);
			this.curAction.handle.refParams.AddRefParam("TargetActor", this.skillContext.TargetActor);
			this.curAction.handle.refParams.SetRefParam("_BulletPos", this.skillContext.EffectPos);
			this.curAction.handle.refParams.SetRefParam("_BulletDir", this.skillContext.EffectDir);
			if (flag)
			{
				this.curAction.handle.refParams.SetRefParam("_TargetPos", this.skillContext.UseVector);
			}
			if (flag2)
			{
				this.curAction.handle.refParams.SetRefParam("_TargetDir", value);
			}
			if (this.cfgData != null)
			{
				int num = this.cfgData.iDuration;
				if (this.cfgData.iDurationGrow > 0)
				{
					SkillSlotType skillSlotType = this.skillContext.SlotType;
					int num2 = (int)(this.cfgData.bGrowthType % 10);
					int num3 = (int)(this.cfgData.bGrowthType / 10);
					if ((skillSlotType >= SkillSlotType.SLOT_SKILL_1 && skillSlotType <= SkillSlotType.SLOT_SKILL_3) || (skillSlotType == SkillSlotType.SLOT_SKILL_0 && num2 > 0))
					{
						int num4 = 1;
						if (this.skillContext.Originator && this.skillContext.Originator.handle.SkillControl != null && this.skillContext.Originator.handle.ValueComponent != null)
						{
							SkillSlot skillSlot = null;
							if (num2 == 1)
							{
								num4 = this.skillContext.Originator.handle.ValueComponent.actorSoulLevel;
							}
							else
							{
								if (num2 - 1 >= 1 && num2 - 1 <= 3)
								{
									skillSlotType = (SkillSlotType)(num2 - 1);
								}
								this.skillContext.Originator.handle.SkillControl.TryGetSkillSlot(skillSlotType, out skillSlot);
								if (skillSlot != null)
								{
									num4 = skillSlot.GetSkillLevel();
								}
							}
						}
						num4 = ((num4 < 1) ? 1 : num4);
						num3 = ((num3 < 1) ? 1 : num3);
						num += (num4 - 1) / num3 * this.cfgData.iDurationGrow;
					}
				}
				if (this.skillContext.Originator && this.skillContext.Originator.handle != null && this.skillContext.Originator.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero && this.skillContext.Originator.handle.TheStaticData.TheHeroOnlyInfo.AttackDistanceType == 2 && this.cfgData.iLongRangeReduction > 0)
				{
					num = num * this.cfgData.iLongRangeReduction / 10000;
				}
				this.curAction.handle.ResetLength(num, false);
				if (this.cfgData.bEffectType == 2)
				{
					this.DealTenacity(this.skillContext.TargetActor);
				}
			}
			bool flag3 = true;
			if (this.cfgData.bShowType != 0 || this.cfgData.bFloatTextID > 0)
			{
				if (!this.skillContext.TargetActor || this.skillContext.TargetActor.handle == null || this.skillContext.TargetActor.handle.BuffHolderComp == null || this.skillContext.TargetActor.handle.BuffHolderComp.SpawnedBuffList == null)
				{
					return false;
				}
				for (int i = 0; i < this.skillContext.TargetActor.handle.BuffHolderComp.SpawnedBuffList.get_Count(); i++)
				{
					BuffSkill buffSkill = this.skillContext.TargetActor.handle.BuffHolderComp.SpawnedBuffList.get_Item(i);
					if (buffSkill != null && buffSkill.cfgData != null && buffSkill.cfgData.iCfgID == this.cfgData.iCfgID)
					{
						flag3 = false;
						break;
					}
				}
				if (flag3)
				{
					SpawnBuffEventParam spawnBuffEventParam = new SpawnBuffEventParam((uint)this.cfgData.bShowType, (uint)this.cfgData.bFloatTextID, this.skillContext.TargetActor);
					Singleton<GameSkillEventSys>.GetInstance().SendEvent<SpawnBuffEventParam>(GameSkillEventDef.Event_SpawnBuff, this.skillContext.TargetActor, ref spawnBuffEventParam, GameSkillEventChannel.Channel_HostCtrlActor);
				}
			}
			this.skillContext.TargetActor.handle.BuffHolderComp.AddBuff(this);
			if (this.cfgData.bEffectType == 2 && this.cfgData.bShowType != 2)
			{
				LimitMoveEventParam limitMoveEventParam = new LimitMoveEventParam(base.CurAction.handle.length, this.SkillID, this.skillContext.TargetActor);
				Singleton<GameSkillEventSys>.GetInstance().SendEvent<LimitMoveEventParam>(GameSkillEventDef.AllEvent_LimitMove, this.skillContext.TargetActor, ref limitMoveEventParam, GameSkillEventChannel.Channel_AllActor);
			}
			if (this.bAgeImmeExcute)
			{
				this.curAction.handle.UpdateLogic((int)Singleton<FrameSynchr>.GetInstance().FrameDelta);
			}
			return true;
		}

		public override bool Use(PoolObjHandle<ActorRoot> user, ref SkillUseParam param)
		{
			this.skillContext.Copy(ref param);
			return this.UseImpl(user);
		}

		public override bool Use(PoolObjHandle<ActorRoot> user)
		{
			return this.UseImpl(user);
		}

		public bool FindSkillFunc(int inSkillFuncType, out ResDT_SkillFunc outSkillFunc)
		{
			outSkillFunc = null;
			if (this.cfgData == null)
			{
				return false;
			}
			for (int i = 0; i < 4; i++)
			{
				if ((int)this.cfgData.astSkillFuncInfo[i].bSkillFuncType == inSkillFuncType)
				{
					outSkillFunc = this.cfgData.astSkillFuncInfo[i];
					return true;
				}
			}
			return false;
		}

		public ResDT_SkillFunc FindSkillFunc(int inSkillFuncType)
		{
			if (this.cfgData == null)
			{
				return null;
			}
			for (int i = 0; i < 4; i++)
			{
				if ((int)this.cfgData.astSkillFuncInfo[i].bSkillFuncType == inSkillFuncType)
				{
					return this.cfgData.astSkillFuncInfo[i];
				}
			}
			return null;
		}

		public int GetSkillFuncParam(ResDT_SkillFunc skillFunc, int _index, bool _bGrow)
		{
			if (skillFunc == null)
			{
				DebugHelper.Assert(false, "FindSkillFunc error: skillFunc = null");
				return 0;
			}
			if (_index < 0 || _index + 1 > 8)
			{
				DebugHelper.Assert(false, "GetSkillFuncParam: index = {0}", new object[]
				{
					_index
				});
			}
			if (_bGrow)
			{
				int num = skillFunc.astSkillFuncParam[_index].iParam;
				int iParam = skillFunc.astSkillFuncGroup[_index].iParam;
				int num2 = iParam * (this.buffLevel - 1);
				num += num2;
				return num * this.overlayCount;
			}
			return skillFunc.astSkillFuncParam[_index].iParam;
		}

		public int GetSkillFuncParam(int inSkillFuncType, int _index, bool _bGrow)
		{
			ResDT_SkillFunc resDT_SkillFunc = this.FindSkillFunc(inSkillFuncType);
			if (resDT_SkillFunc == null)
			{
				DebugHelper.Assert(false, "FindSkillFunc error: inSkillFuncType = {0}", new object[]
				{
					inSkillFuncType
				});
				return 0;
			}
			if (_index < 0 || _index + 1 > 8)
			{
				DebugHelper.Assert(false, "GetSkillFuncParam: index = {0}", new object[]
				{
					_index
				});
			}
			if (_bGrow)
			{
				int num = resDT_SkillFunc.astSkillFuncParam[_index].iParam;
				int iParam = resDT_SkillFunc.astSkillFuncGroup[_index].iParam;
				int num2 = iParam * (this.buffLevel - 1);
				num += num2;
				return num * this.overlayCount;
			}
			return resDT_SkillFunc.astSkillFuncParam[_index].iParam;
		}
	}
}
