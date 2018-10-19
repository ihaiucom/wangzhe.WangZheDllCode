using Assets.Scripts.Common;
using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	public class SkillUseContext
	{
		public PoolObjHandle<ActorRoot> TargetActor;

		public PoolObjHandle<ActorRoot> Originator;

		public object Instigator;

		public bool bSpecialUse;

		public int GatherTime = 1;

		public int EffectCount;

		public int EffectCountInSingleTrigger;

		public int MarkCount;

		public bool bExposing;

		public SKILL_USE_FROM_TYPE skillUseFrom;

		public uint uiFromId;

		public SkillSlotType SlotType
		{
			get;
			set;
		}

		public SkillRangeAppointType AppointType
		{
			get;
			set;
		}

		public uint TargetID
		{
			get;
			private set;
		}

		public VInt3 UseVector
		{
			get;
			private set;
		}

		public VInt3 EndVector
		{
			get;
			private set;
		}

		public VInt3 EffectPos
		{
			get;
			set;
		}

		public VInt3 EffectDir
		{
			get;
			set;
		}

		public VInt3 BulletPos
		{
			get;
			set;
		}

		public SkillUseContext()
		{
			this.SlotType = SkillSlotType.SLOT_SKILL_VALID;
		}

		public SkillUseContext(SkillSlotType InSlot)
		{
			this.SlotType = InSlot;
			this.bSpecialUse = false;
			this.AppointType = SkillRangeAppointType.Target;
		}

		public SkillUseContext(SkillSlotType InSlot, uint ObjID)
		{
			this.SlotType = InSlot;
			this.TargetID = ObjID;
			this.TargetActor = Singleton<GameObjMgr>.GetInstance().GetActor(this.TargetID);
			this.UseVector = ((!this.TargetActor) ? VInt3.zero : this.TargetActor.handle.location);
			this.AppointType = SkillRangeAppointType.Target;
			this.bSpecialUse = false;
		}

		public SkillUseContext(SkillSlotType InSlot, VInt3 InVec)
		{
			this.SlotType = InSlot;
			this.UseVector = InVec;
			this.AppointType = SkillRangeAppointType.Pos;
			this.bSpecialUse = false;
		}

		public SkillUseContext(SkillSlotType InSlot, VInt3 InVec, bool bSpecial)
		{
			this.SlotType = InSlot;
			this.UseVector = InVec;
			this.AppointType = SkillRangeAppointType.Directional;
			this.bSpecialUse = bSpecial;
		}

		public SkillUseContext(SkillSlotType InSlot, VInt3 InBegin, VInt3 InEnd)
		{
			this.SlotType = InSlot;
			this.UseVector = InBegin;
			this.EndVector = InEnd;
			this.bSpecialUse = false;
		}

		public SkillUseContext(SkillSlotType InSlot, PoolObjHandle<ActorRoot> InActorRoot)
		{
			this.SlotType = InSlot;
			this.TargetActor = InActorRoot;
			this.bSpecialUse = false;
		}

		public void Reset()
		{
			this.SlotType = SkillSlotType.SLOT_SKILL_VALID;
			this.AppointType = SkillRangeAppointType.Auto;
			this.TargetID = 0u;
			this.UseVector = VInt3.zero;
			this.EndVector = VInt3.zero;
			this.EffectPos = VInt3.zero;
			this.EffectDir = VInt3.zero;
			this.BulletPos = VInt3.zero;
			this.bSpecialUse = false;
			this.GatherTime = 1;
			this.EffectCount = 0;
			this.EffectCountInSingleTrigger = 0;
			this.MarkCount = 0;
			this.TargetActor.Release();
			this.Originator.Release();
			this.Instigator = null;
			this.bExposing = false;
			this.skillUseFrom = SKILL_USE_FROM_TYPE.SKILL_USE_FROM_TYPE_SKILL;
			this.uiFromId = 0u;
		}

		public void Copy(ref SkillUseParam param)
		{
			this.Reset();
			this.SlotType = param.SlotType;
			this.AppointType = param.AppointType;
			this.TargetID = param.TargetID;
			this.UseVector = param.UseVector;
			this.bSpecialUse = param.bSpecialUse;
			this.TargetActor = param.TargetActor;
			this.Originator = param.Originator;
			this.Instigator = param.Instigator;
			this.bExposing = param.bExposing;
			this.skillUseFrom = param.skillUseFrom;
			this.uiFromId = param.uiFromId;
			this.MarkCount = param.MarkCount;
		}

		public void Copy(SkillUseContext rhs)
		{
			this.SlotType = rhs.SlotType;
			this.AppointType = rhs.AppointType;
			this.TargetID = rhs.TargetID;
			this.UseVector = rhs.UseVector;
			this.EndVector = rhs.EndVector;
			this.EffectPos = rhs.EffectPos;
			this.EffectDir = rhs.EffectDir;
			this.BulletPos = rhs.BulletPos;
			this.TargetActor = rhs.TargetActor;
			this.Originator = rhs.Originator;
			this.Instigator = rhs.Instigator;
			this.bSpecialUse = rhs.bSpecialUse;
			this.GatherTime = rhs.GatherTime;
			this.EffectCount = rhs.EffectCount;
			this.EffectCountInSingleTrigger = rhs.EffectCountInSingleTrigger;
			this.MarkCount = rhs.MarkCount;
			this.bExposing = rhs.bExposing;
			this.skillUseFrom = rhs.skillUseFrom;
			this.uiFromId = rhs.uiFromId;
		}

		private bool IsEquals(SkillUseContext rhs)
		{
			return this.SlotType == rhs.SlotType && this.AppointType == rhs.AppointType && this.TargetID == rhs.TargetID && this.UseVector == rhs.UseVector && this.EndVector == rhs.EndVector && this.EffectPos == rhs.EffectPos && this.BulletPos == rhs.BulletPos && this.EffectDir == rhs.EffectDir && this.TargetActor == rhs.TargetActor && this.Originator == rhs.Originator && this.Instigator == rhs.Instigator && this.bSpecialUse == rhs.bSpecialUse && this.GatherTime == rhs.GatherTime && this.EffectCount == rhs.EffectCount && this.EffectCountInSingleTrigger == rhs.EffectCountInSingleTrigger && this.MarkCount == rhs.MarkCount && this.bExposing == rhs.bExposing && this.skillUseFrom == rhs.skillUseFrom && this.uiFromId == rhs.uiFromId;
		}

		public override bool Equals(object obj)
		{
			return obj != null && base.GetType() == obj.GetType() && this.IsEquals((SkillUseContext)obj);
		}

		// bsh: override GetHashCode to avoid warning
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public void SetOriginator(PoolObjHandle<ActorRoot> inOriginator)
		{
			this.Originator = inOriginator;
		}

		public bool CalcAttackerDir(out VInt3 dir, PoolObjHandle<ActorRoot> attacker)
		{
			if (!attacker)
			{
				dir = VInt3.forward;
				return false;
			}
			dir = attacker.handle.forward;
			switch (this.AppointType)
			{
			case SkillRangeAppointType.Auto:
			case SkillRangeAppointType.Target:
				if (!this.TargetActor)
				{
					return false;
				}
				dir = this.TargetActor.handle.location;
				dir -= attacker.handle.location;
				dir.y = 0;
				break;
			case SkillRangeAppointType.Pos:
				dir = this.UseVector;
				dir -= attacker.handle.location;
				dir.y = 0;
				break;
			case SkillRangeAppointType.Directional:
				dir = this.UseVector;
				dir.y = 0;
				break;
			case SkillRangeAppointType.Track:
				dir = (this.EndVector + this.UseVector).DivBy2() - attacker.handle.location;
				dir.y = 0;
				break;
			}
			return true;
		}
	}
}
