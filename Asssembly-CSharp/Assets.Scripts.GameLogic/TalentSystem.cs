using Assets.Scripts.Common;
using System;

namespace Assets.Scripts.GameLogic
{
	public class TalentSystem
	{
		private int talentCount;

		private PoolObjHandle<ActorRoot> actor = default(PoolObjHandle<ActorRoot>);

		private PassiveSkill[] TalentObjArray = new PassiveSkill[10];

		public void InitTalent(int _talentID, SKILL_USE_FROM_TYPE skillUseFrom = SKILL_USE_FROM_TYPE.SKILL_USE_FROM_TYPE_SKILL, uint uiFromId = 0u)
		{
			if (this.talentCount < 10)
			{
				PassiveSkill passiveSkill = new PassiveSkill(_talentID, this.actor);
				if (passiveSkill.skillContext != null)
				{
					passiveSkill.skillContext.uiFromId = uiFromId;
					passiveSkill.skillContext.skillUseFrom = skillUseFrom;
				}
				for (int i = 0; i < 10; i++)
				{
					if (this.TalentObjArray[i] == null)
					{
						this.TalentObjArray[i] = passiveSkill;
						this.talentCount++;
						return;
					}
				}
			}
		}

		public void InitTalent(int _talentID, int _cdTime, SKILL_USE_FROM_TYPE skillUseFrom = SKILL_USE_FROM_TYPE.SKILL_USE_FROM_TYPE_SKILL, uint uiFromId = 0u)
		{
			if (this.talentCount < 10)
			{
				PassiveSkill passiveSkill = new PassiveSkill(_talentID, this.actor);
				if (passiveSkill.skillContext != null)
				{
					passiveSkill.skillContext.uiFromId = uiFromId;
					passiveSkill.skillContext.skillUseFrom = skillUseFrom;
				}
				for (int i = 0; i < 10; i++)
				{
					if (this.TalentObjArray[i] == null)
					{
						this.TalentObjArray[i] = passiveSkill;
						this.talentCount++;
						passiveSkill.InitCDTime(_cdTime);
						return;
					}
				}
			}
		}

		public void RemoveTalent(int _talentID)
		{
			for (int i = 0; i < 10; i++)
			{
				PassiveSkill passiveSkill = this.TalentObjArray[i];
				if (passiveSkill != null && passiveSkill.SkillID == _talentID)
				{
					if (passiveSkill.passiveEvent != null)
					{
						passiveSkill.passiveEvent.UnInit();
					}
					this.TalentObjArray[i] = null;
					this.talentCount--;
				}
			}
		}

		public int GetTalentCDTime(int _talentID)
		{
			for (int i = 0; i < 10; i++)
			{
				PassiveSkill passiveSkill = this.TalentObjArray[i];
				if (passiveSkill != null && passiveSkill.SkillID == _talentID)
				{
					return passiveSkill.GetCDTime();
				}
			}
			return 0;
		}

		public PassiveSkill[] QueryTalents()
		{
			return this.TalentObjArray;
		}

		public void UpdateLogic(int nDelta)
		{
			for (int i = 0; i < 10; i++)
			{
				PassiveSkill passiveSkill = this.TalentObjArray[i];
				if (passiveSkill != null)
				{
					passiveSkill.UpdateLogic(nDelta);
				}
			}
		}

		public void Init(PoolObjHandle<ActorRoot> _actor)
		{
			this.actor = _actor;
			this.talentCount = 0;
			for (int i = 0; i < 10; i++)
			{
				this.TalentObjArray[i] = null;
			}
		}

		public void UnInit()
		{
			for (int i = 0; i < 10; i++)
			{
				PassiveSkill passiveSkill = this.TalentObjArray[i];
				if (passiveSkill != null && passiveSkill.passiveEvent != null)
				{
					passiveSkill.passiveEvent.UnInit();
				}
			}
		}

		public void Reset()
		{
			this.actor.Validate();
			for (int i = 0; i < 10; i++)
			{
				PassiveSkill passiveSkill = this.TalentObjArray[i];
				if (passiveSkill != null)
				{
					passiveSkill.Reset();
				}
			}
		}

		public void ChangePassiveParam(int _id, int _index, int _value)
		{
			for (int i = 0; i < 10; i++)
			{
				PassiveSkill passiveSkill = this.TalentObjArray[i];
				if (passiveSkill != null && passiveSkill.SkillID == _id)
				{
					passiveSkill.ChangeEventParam(_index, _value);
				}
			}
		}
	}
}
