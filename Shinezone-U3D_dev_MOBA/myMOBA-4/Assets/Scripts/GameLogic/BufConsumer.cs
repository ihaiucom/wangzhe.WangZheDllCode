using Assets.Scripts.Common;
using System;

namespace Assets.Scripts.GameLogic
{
	public struct BufConsumer
	{
		public int BuffID;

		public PoolObjHandle<ActorRoot> TargetActor;

		public PoolObjHandle<ActorRoot> SrcActor;

		public BuffFense buffSkill;

		public BufConsumer(int InBuffID, PoolObjHandle<ActorRoot> InTargetActor, PoolObjHandle<ActorRoot> inSrcActor)
		{
			this.BuffID = InBuffID;
			this.TargetActor = InTargetActor;
			this.SrcActor = inSrcActor;
			this.buffSkill = null;
		}

		public bool Use()
		{
			BuffSkill buffSkill = ClassObjPool<BuffSkill>.Get();
			buffSkill.Init(this.BuffID);
			if (buffSkill.cfgData == null)
			{
				buffSkill.Release();
				return false;
			}
			SkillUseParam skillUseParam = default(SkillUseParam);
			skillUseParam.Init(SkillSlotType.SLOT_SKILL_VALID, this.TargetActor.handle.ObjID);
			skillUseParam.SetOriginator(this.SrcActor);
			skillUseParam.skillUseFrom = SKILL_USE_FROM_TYPE.SKILL_USE_FROM_TYPE_AREATRIGGER;
			skillUseParam.uiFromId = (uint)this.BuffID;
			if (!buffSkill.Use(this.SrcActor, ref skillUseParam))
			{
				buffSkill.Release();
				return false;
			}
			this.buffSkill = new BuffFense(buffSkill);
			return true;
		}
	}
}
