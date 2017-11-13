using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;

namespace AGE
{
	[EventCategory("MMGame/Skill")]
	public class SetCollisionTick : TickEvent
	{
		public enum ColliderType
		{
			Box,
			Sphere,
			CylinderSector
		}

		[ObjectTemplate(new Type[]
		{

		})]
		public int targetId = -1;

		public SetCollisionTick.ColliderType type;

		public VInt3 Pos = VInt3.zero;

		public VInt3 Size = VInt3.one;

		public int Radius = 1000;

		public VInt3 SizeGrowthValue = VInt3.zero;

		public int RadiusGrowthValue;

		public int SectorRadius = 1000;

		public int Degree = 160;

		public int Rotation;

		public int Height = 500;

		public int SectorRadiusGrow;

		public int DegreeGrow;

		public int RotationGrow;

		public int HeightGrow;

		private PoolObjHandle<ActorRoot> actor;

		public override void OnUse()
		{
			base.OnUse();
			this.targetId = -1;
			this.type = SetCollisionTick.ColliderType.Box;
			this.Pos = VInt3.zero;
			this.Size = VInt3.one;
			this.Radius = 1000;
			this.SectorRadius = 1000;
			this.Degree = 160;
			this.Rotation = 0;
			this.Height = 500;
			this.SizeGrowthValue = VInt3.zero;
			this.RadiusGrowthValue = 0;
			this.SectorRadiusGrow = 0;
			this.DegreeGrow = 0;
			this.RotationGrow = 0;
			this.HeightGrow = 0;
		}

		public override void OnRelease()
		{
			base.OnRelease();
			this.actor.Release();
		}

		public override BaseEvent Clone()
		{
			SetCollisionTick setCollisionTick = ClassObjPool<SetCollisionTick>.Get();
			setCollisionTick.CopyData(this);
			return setCollisionTick;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			SetCollisionTick setCollisionTick = src as SetCollisionTick;
			this.targetId = setCollisionTick.targetId;
			this.type = setCollisionTick.type;
			this.Pos = setCollisionTick.Pos;
			this.Size = setCollisionTick.Size;
			this.Radius = setCollisionTick.Radius;
			this.SectorRadius = setCollisionTick.SectorRadius;
			this.Degree = setCollisionTick.Degree;
			this.Rotation = setCollisionTick.Rotation;
			this.Height = setCollisionTick.Height;
			this.SizeGrowthValue = setCollisionTick.SizeGrowthValue;
			this.RadiusGrowthValue = setCollisionTick.RadiusGrowthValue;
			this.SectorRadiusGrow = setCollisionTick.SectorRadiusGrow;
			this.DegreeGrow = setCollisionTick.DegreeGrow;
			this.RotationGrow = setCollisionTick.RotationGrow;
			this.HeightGrow = setCollisionTick.HeightGrow;
		}

		private static T GetCollisionShape<T>(ActorRoot actorRoot, CollisionShapeType shapeType) where T : VCollisionShape, new()
		{
			if (actorRoot.shape == null || actorRoot.shape.GetShapeType() != shapeType)
			{
				T result = Activator.CreateInstance<T>();
				result.Born(actorRoot);
				return result;
			}
			return actorRoot.shape as T;
		}

		public override void Process(Action _action, Track _track)
		{
			this.actor = _action.GetActorHandle(this.targetId);
			if (!this.actor)
			{
				if (ActionManager.Instance.isPrintLog)
				{
				}
				return;
			}
			ActorRoot handle = this.actor.handle;
			SkillUseContext refParamObject = _action.refParams.GetRefParamObject<SkillUseContext>("SkillContext");
			int num = 1;
			if (refParamObject != null && refParamObject.Originator)
			{
				SkillSlot skillSlot;
				refParamObject.Originator.handle.SkillControl.TryGetSkillSlot(refParamObject.SlotType, out skillSlot);
				if (skillSlot != null)
				{
					num = skillSlot.GetSkillLevel();
				}
				BaseSkill refParamObject2 = _action.refParams.GetRefParamObject<BaseSkill>("SkillObj");
				if (refParamObject2 != null)
				{
					BuffSkill buffSkill = refParamObject2.isBuff ? ((BuffSkill)refParamObject2) : null;
					if (buffSkill != null)
					{
						byte b = buffSkill.cfgData.bGrowthType;
						b %= 10;
						if (b > 0 && (SkillSlotType)b != refParamObject.SlotType + 1)
						{
							SSkillFuncContext sSkillFuncContext = default(SSkillFuncContext);
							sSkillFuncContext.inOriginator = refParamObject.Originator;
							sSkillFuncContext.inBuffSkill = new PoolObjHandle<BuffSkill>(buffSkill);
							sSkillFuncContext.inTargetObj = refParamObject.TargetActor;
							sSkillFuncContext.inUseContext = refParamObject;
							num = sSkillFuncContext.iSkillLevel;
						}
					}
				}
			}
			if (this.type == SetCollisionTick.ColliderType.Box)
			{
				VCollisionBox collisionShape = SetCollisionTick.GetCollisionShape<VCollisionBox>(handle, CollisionShapeType.Box);
				collisionShape.Pos = this.Pos;
				collisionShape.Size = this.Size + this.SizeGrowthValue * (num - 1);
				collisionShape.dirty = true;
				collisionShape.ConditionalUpdateShape();
			}
			else if (this.type == SetCollisionTick.ColliderType.Sphere)
			{
				VCollisionSphere collisionShape2 = SetCollisionTick.GetCollisionShape<VCollisionSphere>(handle, CollisionShapeType.Sphere);
				collisionShape2.Pos = this.Pos;
				collisionShape2.Radius = this.Radius + this.RadiusGrowthValue * (num - 1);
				collisionShape2.dirty = true;
				collisionShape2.ConditionalUpdateShape();
			}
			else if (this.type == SetCollisionTick.ColliderType.CylinderSector)
			{
				VCollisionCylinderSector collisionShape3 = SetCollisionTick.GetCollisionShape<VCollisionCylinderSector>(handle, CollisionShapeType.CylinderSector);
				collisionShape3.Pos = this.Pos;
				collisionShape3.Radius = this.SectorRadius + this.SectorRadiusGrow * (num - 1);
				collisionShape3.Height = this.Height + this.HeightGrow * (num - 1);
				collisionShape3.Degree = this.Degree + this.DegreeGrow * (num - 1);
				collisionShape3.Rotation = this.Rotation + this.RotationGrow * (num - 1);
				collisionShape3.dirty = true;
				collisionShape3.ConditionalUpdateShape();
			}
		}

		public override void ProcessBlend(Action _action, Track _track, TickEvent _prevEvent, float _blendWeight)
		{
		}
	}
}
