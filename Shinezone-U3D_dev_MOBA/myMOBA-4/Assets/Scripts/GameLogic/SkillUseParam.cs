using Assets.Scripts.Common;
using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	public struct SkillUseParam
	{
		public SkillSlotType SlotType;

		public SkillRangeAppointType AppointType;

		public uint TargetID;

		public VInt3 UseVector;

		public bool bSpecialUse;

		public PoolObjHandle<ActorRoot> TargetActor;

		public PoolObjHandle<ActorRoot> Originator;

		public object Instigator;

		public bool bExposing;

		public int MarkCount;

		public SKILL_USE_FROM_TYPE skillUseFrom;

		public uint uiFromId;

		public void Reset()
		{
			this.SlotType = SkillSlotType.SLOT_SKILL_0;
			this.AppointType = SkillRangeAppointType.Auto;
			this.TargetID = 0u;
			this.UseVector = VInt3.zero;
			this.bSpecialUse = false;
			this.bExposing = false;
			this.TargetActor.Release();
			this.Originator.Release();
			this.Instigator = null;
			this.skillUseFrom = SKILL_USE_FROM_TYPE.SKILL_USE_FROM_TYPE_SKILL;
			this.uiFromId = 0u;
			this.MarkCount = 0;
		}

		private bool IsEquals(SkillUseParam rhs)
		{
			return this.SlotType == rhs.SlotType && this.AppointType == rhs.AppointType && this.TargetID == rhs.TargetID && this.UseVector == rhs.UseVector && this.TargetActor == rhs.TargetActor && this.Originator == rhs.Originator && this.Instigator == rhs.Instigator && this.bSpecialUse == rhs.bSpecialUse && this.bExposing == rhs.bExposing && this.skillUseFrom == rhs.skillUseFrom && this.uiFromId == rhs.uiFromId && this.MarkCount == rhs.MarkCount;
		}

		public override bool Equals(object obj)
		{
			return obj != null && base.GetType() == obj.GetType() && this.IsEquals((SkillUseParam)obj);
		}

		// bsh: override GetHashCode to avoid warning
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public void Init()
		{
			this.SlotType = SkillSlotType.SLOT_SKILL_VALID;
			this.bSpecialUse = false;
		}

		public void Init(SkillSlotType InSlot)
		{
			this.SlotType = InSlot;
			this.bSpecialUse = false;
			this.AppointType = SkillRangeAppointType.Target;
		}

		public void Init(SkillSlotType InSlot, uint ObjID)
		{
			this.SlotType = InSlot;
			this.TargetID = ObjID;
			this.TargetActor = Singleton<GameObjMgr>.GetInstance().GetActor(this.TargetID);
			this.UseVector = ((!this.TargetActor) ? VInt3.zero : this.TargetActor.handle.location);
			this.AppointType = SkillRangeAppointType.Target;
			this.bSpecialUse = false;
		}

		public void Init(SkillSlotType InSlot, VInt3 InVec)
		{
			this.SlotType = InSlot;
			this.UseVector = InVec;
			this.AppointType = SkillRangeAppointType.Pos;
			this.bSpecialUse = false;
		}

		public void Init(SkillSlotType InSlot, VInt3 InVec, bool bSpecial, uint iTagertId)
		{
			this.SlotType = InSlot;
			this.UseVector = InVec;
			this.AppointType = SkillRangeAppointType.Directional;
			this.bSpecialUse = bSpecial;
			this.TargetID = iTagertId;
		}

		public void Init(SkillSlotType InSlot, PoolObjHandle<ActorRoot> InActorRoot)
		{
			this.SlotType = InSlot;
			this.TargetActor = InActorRoot;
			this.bSpecialUse = false;
		}

		public void SetOriginator(PoolObjHandle<ActorRoot> inOriginator)
		{
			this.Originator = inOriginator;
		}
	}
}
