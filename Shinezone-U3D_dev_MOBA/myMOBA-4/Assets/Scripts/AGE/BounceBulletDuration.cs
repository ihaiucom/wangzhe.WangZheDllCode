using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AGE
{
	[EventCategory("MMGame/Skill")]
	public class BounceBulletDuration : DurationCondition
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int targetId = -1;

		[ObjectTemplate(new Type[]
		{

		})]
		public int attackId = -1;

		[ObjectTemplate(new Type[]
		{

		})]
		public int destId = -1;

		public int velocity = 15000;

		public bool bMoveRotate = true;

		public int maxTargetCount = 1;

		public int maxEffectCount = 3;

		public int searchRadius = 50000;

		public int gravity;

		[AssetReference(AssetRefType.SkillCombine)]
		public int TargetSkillCombine_1;

		[AssetReference(AssetRefType.SkillCombine)]
		public int TargetSkillCombine_2;

		[AssetReference(AssetRefType.SkillCombine)]
		public int TargetSkillCombine_3;

		public bool bHeroWithPrioritySelection = true;

		private PoolObjHandle<ActorRoot> tarActor;

		private PoolObjHandle<ActorRoot> attackActor;

		private PoolObjHandle<ActorRoot> moveActor;

		private AccelerateMotionControler gravityControler;

		private Dictionary<uint, int> effectCountMap = new Dictionary<uint, int>();

		private VInt3 destPosition = VInt3.zero;

		private bool stopCondtion;

		private int lastTime;

		private int curEffectCount;

		public override bool SupportEditMode()
		{
			return true;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			BounceBulletDuration bounceBulletDuration = src as BounceBulletDuration;
			this.targetId = bounceBulletDuration.targetId;
			this.attackId = bounceBulletDuration.attackId;
			this.destId = bounceBulletDuration.destId;
			this.effectCountMap.Clear();
			this.velocity = bounceBulletDuration.velocity;
			this.bMoveRotate = bounceBulletDuration.bMoveRotate;
			this.tarActor = bounceBulletDuration.tarActor;
			this.moveActor = bounceBulletDuration.moveActor;
			this.attackActor = bounceBulletDuration.attackActor;
			this.destPosition = bounceBulletDuration.destPosition;
			this.stopCondtion = bounceBulletDuration.stopCondtion;
			this.lastTime = bounceBulletDuration.lastTime;
			this.maxTargetCount = bounceBulletDuration.maxTargetCount;
			this.maxEffectCount = bounceBulletDuration.maxEffectCount;
			this.searchRadius = bounceBulletDuration.searchRadius;
			this.gravity = bounceBulletDuration.gravity;
			this.TargetSkillCombine_1 = bounceBulletDuration.TargetSkillCombine_1;
			this.TargetSkillCombine_2 = bounceBulletDuration.TargetSkillCombine_2;
			this.TargetSkillCombine_3 = bounceBulletDuration.TargetSkillCombine_3;
			this.bHeroWithPrioritySelection = bounceBulletDuration.bHeroWithPrioritySelection;
		}

		public override BaseEvent Clone()
		{
			BounceBulletDuration bounceBulletDuration = ClassObjPool<BounceBulletDuration>.Get();
			bounceBulletDuration.CopyData(this);
			return bounceBulletDuration;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.tarActor.Release();
			this.moveActor.Release();
			this.attackActor.Release();
			this.gravityControler = null;
			this.effectCountMap.Clear();
			this.destPosition = VInt3.zero;
			this.stopCondtion = false;
			this.lastTime = 0;
			this.curEffectCount = 0;
			this.bHeroWithPrioritySelection = true;
		}

		private void Init(Action _action)
		{
			this.moveActor = _action.GetActorHandle(this.targetId);
			if (!this.moveActor)
			{
				this.stopCondtion = true;
				return;
			}
			this.tarActor = _action.GetActorHandle(this.destId);
			this.attackActor = _action.GetActorHandle(this.attackId);
			if (!this.tarActor || !this.attackActor)
			{
				this.stopCondtion = true;
				return;
			}
			this.gravityControler = new AccelerateMotionControler();
			this.moveActor.handle.ObjLinker.AddCustomMoveLerp(new CustomMoveLerpFunc(this.ActionMoveLerp));
		}

		private void SpawnBuff(Action _action, int triggerCount)
		{
			if (this.tarActor && this.attackActor)
			{
				SkillUseContext refParamObject = _action.refParams.GetRefParamObject<SkillUseContext>("SkillContext");
				if (refParamObject == null)
				{
					return;
				}
				refParamObject.EffectCountInSingleTrigger = triggerCount;
				bool flag = this.attackActor.handle.SkillControl.SpawnBuff(this.tarActor, refParamObject, this.TargetSkillCombine_1, false);
				flag |= this.attackActor.handle.SkillControl.SpawnBuff(this.tarActor, refParamObject, this.TargetSkillCombine_2, false);
				flag |= this.attackActor.handle.SkillControl.SpawnBuff(this.tarActor, refParamObject, this.TargetSkillCombine_3, false);
				if (flag)
				{
					this.tarActor.handle.ActorControl.BeAttackHit(this.attackActor, refParamObject.bExposing);
				}
			}
		}

		private PoolObjHandle<ActorRoot> SearchTarget()
		{
			int num = 0;
			PoolObjHandle<ActorRoot> poolObjHandle = default(PoolObjHandle<ActorRoot>);
			PoolObjHandle<ActorRoot> poolObjHandle2 = default(PoolObjHandle<ActorRoot>);
			int num2 = 4;
			ulong num3 = (ulong)((long)this.searchRadius * (long)this.searchRadius);
			ulong num4 = num3;
			ulong num5 = num3;
			int num6 = this.maxTargetCount;
			int num7 = this.maxTargetCount;
			List<PoolObjHandle<ActorRoot>> gameActors = Singleton<GameObjMgr>.GetInstance().GameActors;
			for (int i = 0; i < gameActors.Count; i++)
			{
				PoolObjHandle<ActorRoot> poolObjHandle3 = gameActors[i];
				ActorRoot handle = poolObjHandle3.handle;
				if (this.attackActor.handle.CanAttack(handle) && (num2 & 1 << (int)handle.TheActorMeta.ActorType) <= 0 && handle.ObjID != this.tarActor.handle.ObjID && handle.HorizonMarker.IsVisibleFor(this.attackActor.handle.TheActorMeta.ActorCamp))
				{
					if (!this.effectCountMap.TryGetValue(handle.ObjID, out num) || num < this.maxTargetCount)
					{
						ulong sqrMagnitudeLong2D = (ulong)(handle.location - this.tarActor.handle.location).sqrMagnitudeLong2D;
						if (sqrMagnitudeLong2D < num3)
						{
							if (!this.bHeroWithPrioritySelection || handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
							{
								if (num < num6)
								{
									poolObjHandle = poolObjHandle3;
									num4 = sqrMagnitudeLong2D;
									num6 = num;
								}
								else if (num == num6 && sqrMagnitudeLong2D < num4)
								{
									poolObjHandle = poolObjHandle3;
									num4 = sqrMagnitudeLong2D;
								}
							}
							else if (num < num7)
							{
								poolObjHandle2 = poolObjHandle3;
								num5 = sqrMagnitudeLong2D;
								num7 = num;
							}
							else if (num == num7 && sqrMagnitudeLong2D < num5)
							{
								poolObjHandle2 = poolObjHandle3;
								num5 = sqrMagnitudeLong2D;
							}
						}
					}
				}
			}
			return (!poolObjHandle) ? poolObjHandle2 : poolObjHandle;
		}

		private void InitTarget()
		{
			VInt3 a = VInt3.one;
			int num = 0;
			this.destPosition = this.tarActor.handle.location;
			CActorInfo charInfo = this.tarActor.handle.CharInfo;
			if (charInfo != null)
			{
				num = charInfo.iBulletHeight;
				a = this.moveActor.handle.location - this.destPosition;
				a.y = 0;
				a = a.NormalizeTo(1000);
				this.destPosition += IntMath.Divide(a, (long)charInfo.iCollisionSize.x, 1000L);
			}
			this.destPosition.y = this.destPosition.y + num;
		}

		private bool InitGravity()
		{
			int num = (int)IntMath.Divide((long)(this.destPosition - this.moveActor.handle.location).magnitude2D * 1000L, (long)this.velocity);
			if (num == 0)
			{
				return false;
			}
			VInt vInt = 0;
			if (PathfindingUtility.GetGroundY(this.destPosition, out vInt))
			{
				this.gravityControler.InitMotionControler(num, vInt.i - this.moveActor.handle.location.y, this.gravity);
			}
			else
			{
				this.gravityControler.InitMotionControler(num, 0, this.gravity);
			}
			return true;
		}

		private void ActionMoveLerp(ActorRoot actor, uint nDelta, bool bReset)
		{
			if (actor == null || this.stopCondtion)
			{
				return;
			}
			VInt3 location = actor.location;
			VInt3 vInt = this.destPosition - location;
			int newMagn = this.velocity * (int)nDelta / 1000;
			Vector3 vector = actor.myTransform.position;
			if (this.gravity < 0)
			{
				vInt.y = 0;
				vector += (Vector3)vInt.NormalizeTo(newMagn);
				vector.y += (float)this.gravityControler.GetMotionLerpDistance((int)nDelta) / 1000f;
				VInt ob;
				if (PathfindingUtility.GetGroundY((VInt3)vector, out ob) && vector.y < (float)ob)
				{
					vector.y = (float)ob;
				}
			}
			else
			{
				vector += (Vector3)vInt.NormalizeTo(newMagn);
			}
			actor.myTransform.position = vector;
		}

		private void MoveToTarget(Action _action, int _localTime)
		{
			int num = _localTime - this.lastTime;
			this.lastTime = _localTime;
			VInt3 location = this.moveActor.handle.location;
			this.InitTarget();
			VInt3 dir = this.destPosition - location;
			if (this.bMoveRotate)
			{
				this.RotateMoveBullet(dir);
			}
			int num2 = this.velocity * num / 1000;
			if ((long)num2 * (long)num2 >= dir.sqrMagnitudeLong2D)
			{
				this.moveActor.handle.location = this.destPosition;
				if (this.curEffectCount < this.maxEffectCount)
				{
					this.curEffectCount++;
					int num3 = 0;
					if (this.effectCountMap.TryGetValue(this.tarActor.handle.ObjID, out num3))
					{
						num3 = (this.effectCountMap[this.tarActor.handle.ObjID] = num3 + 1);
					}
					else
					{
						this.effectCountMap.Add(this.tarActor.handle.ObjID, ++num3);
					}
					this.SpawnBuff(_action, this.curEffectCount);
					if (this.curEffectCount < this.maxEffectCount)
					{
						this.tarActor = this.SearchTarget();
						if (!this.tarActor)
						{
							this.stopCondtion = true;
						}
					}
					else
					{
						this.stopCondtion = true;
					}
				}
			}
			else
			{
				VInt3 vInt;
				if (this.gravity < 0 && this.InitGravity())
				{
					dir.y = 0;
					vInt = location + dir.NormalizeTo(num2);
					vInt.y += this.gravityControler.GetMotionDeltaDistance(num);
					VInt vInt2;
					if (PathfindingUtility.GetGroundY(vInt, out vInt2) && vInt.y < vInt2.i)
					{
						vInt.y = vInt2.i;
					}
				}
				else
				{
					vInt = location + dir.NormalizeTo(num2);
				}
				this.moveActor.handle.location = vInt;
			}
		}

		private void RotateMoveBullet(VInt3 _dir)
		{
			if (_dir == VInt3.zero)
			{
				return;
			}
			this.moveActor.handle.forward = _dir.NormalizeTo(1000);
			Quaternion rotation = Quaternion.identity;
			rotation = Quaternion.LookRotation((Vector3)_dir);
			this.moveActor.handle.rotation = rotation;
		}

		public override void Enter(Action _action, Track _track)
		{
			this.stopCondtion = false;
			this.Init(_action);
			base.Enter(_action, _track);
		}

		public override void Leave(Action _action, Track _track)
		{
			base.Leave(_action, _track);
			if (this.moveActor)
			{
				this.moveActor.handle.ObjLinker.RmvCustomMoveLerp(new CustomMoveLerpFunc(this.ActionMoveLerp));
			}
			this.tarActor.Release();
			this.moveActor.Release();
		}

		public override void Process(Action _action, Track _track, int _localTime)
		{
			if (!this.moveActor || !this.tarActor || this.stopCondtion)
			{
				return;
			}
			this.MoveToTarget(_action, _localTime);
			base.Process(_action, _track, _localTime);
		}

		public override bool Check(Action _action, Track _track)
		{
			return this.stopCondtion;
		}
	}
}
