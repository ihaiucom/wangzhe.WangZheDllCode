using Assets.Scripts.Common;
using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	[PassiveCondition(PassiveConditionType.RemoveBuffPassiveCondition)]
	public class PassiveRemoveBuffCondition : PassiveCondition
	{
		private bool bTrigger;

		public override void Init(PoolObjHandle<ActorRoot> _source, PassiveEvent _event, ref ResDT_SkillPassiveCondition _config)
		{
			this.bTrigger = true;
			base.Init(_source, _event, ref _config);
			if (_source && _source.handle.BuffHolderComp != null && _source.handle.BuffHolderComp.SpawnedBuffList != null)
			{
				for (int i = 0; i < _source.handle.BuffHolderComp.SpawnedBuffList.get_Count(); i++)
				{
					BuffSkill buffSkill = _source.handle.BuffHolderComp.SpawnedBuffList.get_Item(i);
					if (buffSkill != null && buffSkill.cfgData != null && buffSkill.cfgData.iCfgID == this.localParams[0])
					{
						this.bTrigger = false;
						break;
					}
				}
			}
			Singleton<GameSkillEventSys>.GetInstance().AddEventHandler<BuffChangeEventParam>(GameSkillEventDef.AllEvent_BuffChange, new GameSkillEvent<BuffChangeEventParam>(this.OnPlayerBuffChange));
		}

		public override void UnInit()
		{
			Singleton<GameSkillEventSys>.GetInstance().RmvEventHandler<BuffChangeEventParam>(GameSkillEventDef.AllEvent_BuffChange, new GameSkillEvent<BuffChangeEventParam>(this.OnPlayerBuffChange));
		}

		public override void Reset()
		{
			this.bTrigger = false;
		}

		private void OnPlayerBuffChange(ref BuffChangeEventParam prm)
		{
			if (prm.target != this.sourceActor || !prm.stBuffSkill)
			{
				return;
			}
			if (prm.stBuffSkill.handle.cfgData != null && prm.stBuffSkill.handle.cfgData.iCfgID == this.localParams[0])
			{
				if (prm.bIsAdd)
				{
					this.bTrigger = false;
				}
				else
				{
					this.bTrigger = true;
				}
			}
		}

		public override bool Fit()
		{
			return this.bTrigger;
		}
	}
}
