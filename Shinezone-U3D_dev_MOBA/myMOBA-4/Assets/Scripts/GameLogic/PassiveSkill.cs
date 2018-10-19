using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	public class PassiveSkill : BaseSkill
	{
		public SkillSlotType SlotType;

		private PoolObjHandle<ActorRoot> sourceActor;

		public ResSkillPassiveCfgInfo cfgData;

		public PassiveEvent passiveEvent;

		public bool bShowAsElite
		{
			get
			{
				return this.cfgData.bShowAsElite != 0;
			}
		}

		public string PassiveSkillName
		{
			get
			{
				return Utility.UTF8Convert(this.cfgData.szPassiveName);
			}
		}

		public string PassiveSkillDesc
		{
			get
			{
				return Utility.UTF8Convert(this.cfgData.szPassiveDesc);
			}
		}

		public bool bExposing
		{
			get
			{
				return this.cfgData.bPassiveExposing > 0;
			}
		}

		public PassiveSkill(int id, PoolObjHandle<ActorRoot> root)
		{
			this.sourceActor = root;
			this.SkillID = id;
			this.cfgData = GameDataMgr.skillPassiveDatabin.GetDataByKey((long)id);
			if (this.cfgData == null)
			{
				return;
			}
			this.Init();
		}

		public void Init()
		{
			this.SlotType = SkillSlotType.SLOT_SKILL_VALID;
			this.ActionName = StringHelper.UTF8BytesToString(ref this.cfgData.szActionName);
			this.bAgeImmeExcute = (this.cfgData.bAgeImmeExcute == 1);
			if (this.sourceActor)
			{
				this.skillContext.uiFromId = this.sourceActor.handle.ObjID;
				this.skillContext.skillUseFrom = SKILL_USE_FROM_TYPE.SKILL_USE_FROM_TYPE_PASSIVESKILL;
			}
			this.passiveEvent = Singleton<PassiveCreater<PassiveEvent, PassiveEventAttribute>>.GetInstance().Create((int)this.cfgData.dwPassiveEventType);
			if (this.passiveEvent == null)
			{
				return;
			}
			for (int i = 0; i < 2; i++)
			{
				int dwConditionType = (int)this.cfgData.astPassiveConditon[i].dwConditionType;
				PassiveCondition passiveCondition = Singleton<PassiveCreater<PassiveCondition, PassiveConditionAttribute>>.GetInstance().Create(dwConditionType);
				if (passiveCondition != null)
				{
					this.passiveEvent.AddCondition(passiveCondition);
				}
			}
			this.passiveEvent.Init(this.sourceActor, this);
		}

		public void InitCDTime(int _cdTime)
		{
			if (this.passiveEvent != null)
			{
				this.passiveEvent.InitCDTime(_cdTime);
			}
		}

		public int GetCDTime()
		{
			if (this.passiveEvent != null)
			{
				return this.passiveEvent.GetCDTime();
			}
			return 0;
		}

		public void UnInit()
		{
			if (this.passiveEvent != null)
			{
				this.passiveEvent.UnInit();
			}
		}

		public void Reset()
		{
			this.sourceActor.Validate();
			if (this.passiveEvent != null)
			{
				this.passiveEvent.UnInit();
				this.passiveEvent.Init(this.sourceActor, this);
			}
		}

		public override bool Use(PoolObjHandle<ActorRoot> user, ref SkillUseParam param)
		{
			param.Instigator = this;
			DebugHelper.Assert(param.Originator);
			param.bExposing = this.bExposing;
			return base.Use(user, ref param);
		}

		public void UpdateLogic(int nDelta)
		{
			if (this.passiveEvent != null)
			{
				this.passiveEvent.UpdateLogic(nDelta);
			}
		}

		public void ChangeEventParam(int index, int value)
		{
			if (this.passiveEvent != null)
			{
				this.passiveEvent.ChangeEventParam(index, value);
			}
		}
	}
}
