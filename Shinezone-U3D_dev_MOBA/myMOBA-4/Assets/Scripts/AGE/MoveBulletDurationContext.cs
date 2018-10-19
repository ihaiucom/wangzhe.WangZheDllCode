using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AGE
{
	internal class MoveBulletDurationContext
	{
		public int length;

		public int targetId;

		public int destId;

		public ActorMoveType MoveType;

		public VInt3 targetPosition;

		public VInt3 offsetDir;

		public int velocity;

		public int acceleration;

		public int distance;

		public int gravity;

		public bool bMoveRotate;

		public bool bAdjustSpeed;

		public bool bBulletUseDir;

		public bool bUseIndicatorDir;

		public bool bReachDestStop;

		public bool bResetMoveDistance;

		public bool bMoveOnXAxis;

		public int distanceZ0;

		public int distanceZ1;

		public int distanceX;

		public int rotateBodyDegreeSpeed;

		public int rotateBodyRadius;

		public int rotateBodyHeight;

		public int rotateBodyFindEnemyLatency;

		public int rotateBodyFindEnemyRadius;

		public int rotateBodyFindEnemyCd;

		public int rotateBodyBulletCount;

		public bool bFindTargetByRotateBodyBullet;

		private SkillUseContext skillContext;

		private VInt3 destPosition;

		private VInt3 xDestPosition;

		private VInt3 zCurPosition;

		public int lastTime;

		private int hitHeight;

		public PoolObjHandle<ActorRoot> tarActor;

		public PoolObjHandle<ActorRoot> moveActor;

		public PoolObjHandle<ActorRoot> originateActor;

		private AccelerateMotionControler gravityControler;

		private xAxisAccelerateMotionControler xControler;

		public bool stopCondtion;

		public bool stopLerpCondtion;

		private VInt3 moveDirection;

		private VInt3 lerpDirection;

		private int lastVelocity;

		private int lastLerpVelocity;

		private VInt3 zDirection;

		private VInt3 xDirection;

		private int rotateBodyCurDirDegreeAngle;

		private int lerpRotateBodyCurDirDegreeAngle;

		public bool shouldUseAcceleration
		{
			get
			{
				return !this.bAdjustSpeed && this.acceleration != 0;
			}
		}

		public void Reset(MoveBulletDuration InBulletDuration)
		{
			this.length = InBulletDuration.length;
			this.targetId = InBulletDuration.targetId;
			this.destId = InBulletDuration.destId;
			this.MoveType = InBulletDuration.MoveType;
			this.targetPosition = InBulletDuration.targetPosition;
			this.offsetDir = InBulletDuration.offsetDir;
			this.velocity = InBulletDuration.velocity;
			this.acceleration = InBulletDuration.acceleration;
			this.distance = InBulletDuration.distance;
			this.gravity = InBulletDuration.gravity;
			this.bMoveRotate = InBulletDuration.bMoveRotate;
			this.bAdjustSpeed = InBulletDuration.bAdjustSpeed;
			this.bBulletUseDir = InBulletDuration.bBulletUseDir;
			this.bUseIndicatorDir = InBulletDuration.bUseIndicatorDir;
			this.bReachDestStop = InBulletDuration.bReachDestStop;
			this.bResetMoveDistance = InBulletDuration.bResetMoveDistance;
			this.lastVelocity = (this.lastLerpVelocity = 0);
			this.stopLerpCondtion = false;
			this.bMoveOnXAxis = InBulletDuration.bMoveOnXAxis;
			this.distanceZ0 = InBulletDuration.distanceZ0;
			this.distanceZ1 = InBulletDuration.distanceZ1;
			this.distanceX = InBulletDuration.distanceX;
			this.rotateBodyDegreeSpeed = InBulletDuration.rotateBodyDegreeSpeed;
			this.rotateBodyRadius = InBulletDuration.rotateBodyRadius;
			this.rotateBodyHeight = InBulletDuration.rotateBodyHeight;
			this.rotateBodyFindEnemyLatency = InBulletDuration.rotateBodyFindEnemyLatency;
			this.rotateBodyFindEnemyRadius = InBulletDuration.rotateBodyFindEnemyRadius;
			this.rotateBodyFindEnemyCd = InBulletDuration.rotateBodyFindEnemyCd;
			this.rotateBodyBulletCount = InBulletDuration.rotateBodyBulletCount;
			this.bFindTargetByRotateBodyBullet = InBulletDuration.bFindTargetByRotateBodyBullet;
		}

		public void Reset(BulletTriggerDuration InBulletTrigger)
		{
			this.length = InBulletTrigger.length;
			this.targetId = InBulletTrigger.targetId;
			this.destId = InBulletTrigger.destId;
			this.MoveType = InBulletTrigger.MoveType;
			this.targetPosition = InBulletTrigger.targetPosition;
			this.offsetDir = InBulletTrigger.offsetDir;
			this.velocity = InBulletTrigger.velocity;
			this.acceleration = InBulletTrigger.acceleration;
			this.distance = InBulletTrigger.distance;
			this.gravity = InBulletTrigger.gravity;
			this.bMoveRotate = InBulletTrigger.bMoveRotate;
			this.bAdjustSpeed = InBulletTrigger.bAdjustSpeed;
			this.bBulletUseDir = InBulletTrigger.bBulletUseDir;
			this.bUseIndicatorDir = InBulletTrigger.bUseIndicatorDir;
			this.bReachDestStop = InBulletTrigger.bReachDestStop;
			this.lastVelocity = (this.lastLerpVelocity = 0);
			this.stopLerpCondtion = false;
			this.bResetMoveDistance = false;
			this.bMoveOnXAxis = InBulletTrigger.bMoveOnXAxis;
			this.distanceZ0 = InBulletTrigger.distanceZ0;
			this.distanceZ1 = InBulletTrigger.distanceZ1;
			this.distanceX = InBulletTrigger.distanceX;
		}

		public void CopyData(ref MoveBulletDurationContext r)
		{
			this.length = r.length;
			this.targetId = r.targetId;
			this.destId = r.destId;
			this.MoveType = r.MoveType;
			this.targetPosition = r.targetPosition;
			this.offsetDir = r.offsetDir;
			this.velocity = r.velocity;
			this.acceleration = r.acceleration;
			this.distance = r.distance;
			this.gravity = r.gravity;
			this.bMoveRotate = r.bMoveRotate;
			this.bAdjustSpeed = r.bAdjustSpeed;
			this.bBulletUseDir = r.bBulletUseDir;
			this.bUseIndicatorDir = r.bUseIndicatorDir;
			this.bReachDestStop = r.bReachDestStop;
			this.bResetMoveDistance = r.bResetMoveDistance;
			this.skillContext = r.skillContext;
			this.destPosition = r.destPosition;
			this.lastTime = r.lastTime;
			this.hitHeight = r.hitHeight;
			this.tarActor = r.tarActor;
			this.originateActor = r.originateActor;
			this.moveActor = r.moveActor;
			this.gravityControler = r.gravityControler;
			this.stopCondtion = r.stopCondtion;
			this.moveDirection = r.moveDirection;
			this.lerpDirection = r.lerpDirection;
			this.lastVelocity = r.lastVelocity;
			this.lastLerpVelocity = r.lastLerpVelocity;
			this.zCurPosition = r.zCurPosition;
			this.xDirection = r.xDirection;
			this.bMoveOnXAxis = r.bMoveOnXAxis;
			this.distanceZ0 = r.distanceZ0;
			this.distanceZ1 = r.distanceZ1;
			this.distanceX = r.distanceX;
			this.zDirection = r.zDirection;
			this.xDestPosition = r.xDestPosition;
			this.rotateBodyDegreeSpeed = r.rotateBodyDegreeSpeed;
			this.rotateBodyRadius = r.rotateBodyRadius;
			this.rotateBodyHeight = r.rotateBodyHeight;
			this.rotateBodyFindEnemyLatency = r.rotateBodyFindEnemyLatency;
			this.rotateBodyFindEnemyRadius = r.rotateBodyFindEnemyRadius;
			this.rotateBodyFindEnemyCd = r.rotateBodyFindEnemyCd;
			this.rotateBodyBulletCount = r.rotateBodyBulletCount;
			this.bFindTargetByRotateBodyBullet = r.bFindTargetByRotateBodyBullet;
			this.rotateBodyCurDirDegreeAngle = r.rotateBodyCurDirDegreeAngle;
			this.lerpRotateBodyCurDirDegreeAngle = r.lerpRotateBodyCurDirDegreeAngle;
		}

		public void Enter(Action _action)
		{
			this.skillContext = _action.refParams.GetRefParamObject<SkillUseContext>("SkillContext");
			this.lastTime = 0;
			this.lastVelocity = (this.lastLerpVelocity = this.velocity);
			this.stopCondtion = false;
			this.moveActor = _action.GetActorHandle(this.targetId);
			if (!this.moveActor)
			{
				return;
			}
			this.gravityControler = new AccelerateMotionControler();
			this.xControler = new xAxisAccelerateMotionControler();
			this.moveActor.handle.ObjLinker.AddCustomMoveLerp(new CustomMoveLerpFunc(this.ActionMoveLerp));
			if (this.MoveType == ActorMoveType.Target)
			{
				if (this.bFindTargetByRotateBodyBullet)
				{
					_action.refParams.GetRefParam("FindEnemyActor", ref this.tarActor);
				}
				else
				{
					this.tarActor = _action.GetActorHandle(this.destId);
				}
				if (!this.tarActor)
				{
					return;
				}
				this.destPosition = this.tarActor.handle.location;
				CActorInfo charInfo = this.tarActor.handle.CharInfo;
				if (charInfo != null)
				{
					this.hitHeight = charInfo.iBulletHeight;
					VInt3 a = this.moveActor.handle.location - this.destPosition;
					a.y = 0;
					a = a.NormalizeTo(1000);
					this.destPosition += IntMath.Divide(a, (long)charInfo.iCollisionSize.x, 1000L);
				}
				this.destPosition.y = this.destPosition.y + this.hitHeight;
			}
			else if (this.MoveType == ActorMoveType.Directional)
			{
				VInt3 vInt = VInt3.one;
				if (this.skillContext == null)
				{
					return;
				}
				PoolObjHandle<ActorRoot> originator = this.skillContext.Originator;
				if (!originator)
				{
					return;
				}
				if (this.bBulletUseDir)
				{
					_action.refParams.GetRefParam("_BulletUseDir", ref vInt);
				}
				else if (this.bUseIndicatorDir)
				{
					SkillUseContext refParamObject = _action.refParams.GetRefParamObject<SkillUseContext>("SkillContext");
					VInt3 vInt2;
					if (refParamObject != null && refParamObject.CalcAttackerDir(out vInt2, originator))
					{
						vInt = vInt2;
					}
					else
					{
						vInt = originator.handle.forward;
					}
				}
				else
				{
					vInt = originator.handle.forward;
				}
				this.moveActor.handle.forward = vInt;
				this.moveActor.handle.rotation = Quaternion.LookRotation((Vector3)vInt);
				vInt = vInt.RotateY(this.offsetDir.y);
				if (this.bResetMoveDistance)
				{
					int num = 0;
					_action.refParams.GetRefParam("_BulletRealFlyingTime", ref num);
					int num2 = num * this.velocity / 1000;
					this.distance = ((num2 <= 0) ? this.distance : num2);
				}
				this.destPosition = this.moveActor.handle.location + vInt.NormalizeTo(this.distance);
				this.destPosition.y = this.moveActor.handle.location.y;
			}
			else if (this.MoveType == ActorMoveType.Position)
			{
				if (this.bReachDestStop)
				{
					this.destPosition = this.targetPosition;
				}
				else
				{
					VInt3 lhs = this.targetPosition - this.moveActor.handle.location;
					lhs.y = 0;
					lhs = lhs.NormalizeTo(1000);
					this.destPosition = this.moveActor.handle.location + lhs * (this.length * this.velocity / 1000);
					VInt vInt3;
					if (PathfindingUtility.GetGroundY(this.destPosition, out vInt3))
					{
						this.destPosition.y = vInt3.i;
					}
				}
			}
			else if (this.MoveType == ActorMoveType.RotateBody)
			{
				this.originateActor = this.skillContext.Originator;
				if (!this.originateActor)
				{
					DebugHelper.Assert(false, "产生子弹的originateActor不能为空!!!");
					return;
				}
				this.rotateBodyBulletCount = Mathf.Clamp(this.rotateBodyBulletCount, 1, 360);
				this.rotateBodyCurDirDegreeAngle = 360 / this.rotateBodyBulletCount * this.skillContext.BulletPos.x;
				VInt3 vInt4 = this.moveActor.handle.forward.RotateY(-this.rotateBodyCurDirDegreeAngle);
				this.moveActor.handle.forward = vInt4;
				this.moveActor.handle.rotation = Quaternion.LookRotation((Vector3)vInt4);
			}
			if (this.bAdjustSpeed)
			{
				VInt3 vInt5 = this.destPosition - this.moveActor.handle.location;
				int num3 = this.length - 100;
				num3 = ((num3 > 0) ? num3 : this.length);
				this.velocity = (int)IntMath.Divide((long)vInt5.magnitude2D * 1000L, (long)num3);
			}
			if (this.gravity < 0)
			{
				if (this.velocity == 0)
				{
					this.stopCondtion = true;
					return;
				}
				VInt3 vInt6 = this.destPosition - this.moveActor.handle.location;
				int num4;
				if (!this.shouldUseAcceleration)
				{
					num4 = (int)IntMath.Divide((long)vInt6.magnitude2D * 1000L, (long)this.velocity);
				}
				else
				{
					long num5 = (long)this.velocity;
					long num6 = (long)this.acceleration;
					long num7 = (long)vInt6.magnitude2D;
					long a2 = num5 * num5 + 2L * num6 * num7;
					num4 = (int)IntMath.Divide(((long)IntMath.Sqrt(a2) - num5) * 1000L, num6);
					this.lastVelocity = (this.lastLerpVelocity = this.velocity);
				}
				if (num4 == 0)
				{
					this.stopCondtion = true;
					return;
				}
				VInt vInt7;
				if (PathfindingUtility.GetGroundY(this.destPosition, out vInt7))
				{
					this.gravityControler.InitMotionControler(num4, vInt7.i - this.moveActor.handle.location.y, this.gravity);
				}
				else
				{
					this.gravityControler.InitMotionControler(num4, 0, this.gravity);
				}
			}
			if (this.bMoveOnXAxis)
			{
				this.zDirection = this.destPosition - this.moveActor.handle.location;
				this.xDirection = VInt3.Cross(VInt3.up, this.zDirection);
				this.zCurPosition = this.moveActor.handle.location;
				if (this.bReachDestStop)
				{
					int[] array = new int[3];
					VInt3[] array2 = new VInt3[]
					{
						default(VInt3),
						default(VInt3),
						this.destPosition - this.moveActor.handle.location
					};
					array2[0] = array2[2];
					array2[1] = array2[2];
					array2[0].NormalizeTo(this.distanceZ0);
					array2[1].NormalizeTo(this.distanceZ1);
					for (int i = 0; i < array.Length; i++)
					{
						if (!this.shouldUseAcceleration)
						{
							array[i] = (int)IntMath.Divide((long)array2[i].magnitude2D * 1000L, (long)this.velocity);
						}
						else
						{
							long num8 = (long)this.velocity;
							long num9 = (long)this.acceleration;
							long num10 = (long)array2[i].magnitude2D;
							long a3 = num8 * num8 + 2L * num9 * num10;
							array[i] = (int)IntMath.Divide(((long)IntMath.Sqrt(a3) - num8) * 1000L, num9);
						}
					}
					if (array[2] > array[1] && array[1] > array[0] && array[0] > 0)
					{
						this.xControler.InitMotionControler(array[0], array[1], array[2], this.distanceZ0, this.distanceZ1, this.distanceX);
						int desPostion = this.xControler.getDesPostion();
						this.xDestPosition = this.xDirection;
						this.xDestPosition.NormalizeTo(Math.Abs(desPostion));
						if (desPostion < 0)
						{
							this.xDestPosition = -this.xDestPosition;
						}
					}
					else
					{
						this.bMoveOnXAxis = false;
					}
				}
			}
		}

		public void Leave(Action _action, Track _track)
		{
			if (this.moveActor)
			{
				this.moveActor.handle.ObjLinker.RmvCustomMoveLerp(new CustomMoveLerpFunc(this.ActionMoveLerp));
				this.moveActor.handle.myTransform.position = (Vector3)this.moveActor.handle.location;
				if (this.moveActor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Bullet)
				{
					BulletWrapper bulletWrapper = this.moveActor.handle.ActorControl as BulletWrapper;
					if (bulletWrapper != null && bulletWrapper.GetMoveCollisiong())
					{
						bulletWrapper.SetMoveDelta(0);
					}
				}
			}
			this.skillContext = null;
			this.tarActor.Release();
			this.moveActor.Release();
			this.originateActor.Release();
			this.gravityControler = null;
		}

		private void RotateMoveBullet(VInt3 _dir)
		{
			if (this.MoveType == ActorMoveType.Target || this.MoveType == ActorMoveType.Directional)
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
		}

		private void ActionMoveLerp(ActorRoot actor, uint nDelta, bool bReset)
		{
			if (actor == null || this.stopCondtion || this.stopLerpCondtion)
			{
				return;
			}
			Vector3 vector = Vector3.one;
			if (this.MoveType == ActorMoveType.RotateBody)
			{
				int num = this.rotateBodyDegreeSpeed * (int)nDelta / 1000;
				this.lerpRotateBodyCurDirDegreeAngle += num;
				this.lerpRotateBodyCurDirDegreeAngle %= 360;
				long nom = (long)((float)this.lerpRotateBodyCurDirDegreeAngle * 0.0174532924f * 1000f);
				VFactor f;
				VFactor f2;
				IntMath.sincos(out f, out f2, nom, 1000L);
				vector.x = this.originateActor.handle.myTransform.position.x + (float)(this.rotateBodyRadius * f2);
				vector.y = (float)this.rotateBodyHeight;
				vector.z = this.originateActor.handle.myTransform.position.z + (float)(this.rotateBodyRadius * f);
			}
			else
			{
				int newMagn;
				if (!this.shouldUseAcceleration)
				{
					newMagn = this.velocity * (int)nDelta / 1000;
				}
				else
				{
					long num2 = (long)this.lastLerpVelocity * (long)((ulong)nDelta) + (long)this.acceleration * (long)((ulong)nDelta) * (long)((ulong)nDelta) / 2L / 1000L;
					num2 /= 1000L;
					newMagn = (int)num2;
					this.lastLerpVelocity += (int)((long)this.acceleration * (long)((ulong)nDelta)) / 1000;
				}
				vector = actor.myTransform.position;
				if (this.gravity < 0)
				{
					this.lerpDirection.y = 0;
					vector += (Vector3)this.lerpDirection.NormalizeTo(newMagn);
					vector.y += (float)this.gravityControler.GetMotionLerpDistance((int)nDelta) / 1000f;
					VInt ob;
					if (PathfindingUtility.GetGroundY(this.destPosition, out ob) && vector.y < (float)ob)
					{
						vector.y = (float)ob;
					}
				}
				else
				{
					vector += (Vector3)this.lerpDirection.NormalizeTo(newMagn);
				}
				if (this.bMoveOnXAxis)
				{
					int motionLerpDistance = this.xControler.GetMotionLerpDistance((int)nDelta);
					VInt3 vInt = this.xDirection;
					vInt.NormalizeTo(Math.Abs(motionLerpDistance));
					if (motionLerpDistance < 0)
					{
						vInt = -vInt;
					}
					vector += (Vector3)vInt;
				}
			}
			actor.myTransform.position = vector;
		}

		public void ProcessInner(Action _action, Track _track, int delta)
		{
			if (this.MoveType == ActorMoveType.RotateBody)
			{
				int num = this.rotateBodyDegreeSpeed * delta / 1000;
				VInt3 vInt = this.moveActor.handle.forward.RotateY(-num);
				this.moveActor.handle.forward = vInt;
				this.moveActor.handle.rotation = Quaternion.LookRotation((Vector3)vInt);
				this.rotateBodyCurDirDegreeAngle += num;
				this.rotateBodyCurDirDegreeAngle %= 360;
				long nom = (long)((float)this.rotateBodyCurDirDegreeAngle * 0.0174532924f * 1000f);
				VFactor f;
				VFactor f2;
				IntMath.sincos(out f, out f2, nom, 1000L);
				this.destPosition.x = this.originateActor.handle.location.x + this.rotateBodyRadius * f2;
				this.destPosition.y = this.rotateBodyHeight;
				this.destPosition.z = this.originateActor.handle.location.z + this.rotateBodyRadius * f;
				this.moveDirection = this.destPosition - this.moveActor.handle.location;
				this.lerpDirection = this.moveDirection;
				this.moveActor.handle.location = this.destPosition;
			}
			else
			{
				VInt3 location = this.moveActor.handle.location;
				if (this.MoveType == ActorMoveType.Target && this.tarActor)
				{
					this.destPosition = this.tarActor.handle.location;
					if (this.tarActor && this.tarActor.handle.CharInfo != null)
					{
						CActorInfo charInfo = this.tarActor.handle.CharInfo;
						this.hitHeight = charInfo.iBulletHeight;
						VInt3 a = this.moveActor.handle.location - this.destPosition;
						a.y = 0;
						a = a.NormalizeTo(1000);
						this.destPosition += IntMath.Divide(a, (long)charInfo.iCollisionSize.x, 1000L);
					}
					this.destPosition.y = this.destPosition.y + this.hitHeight;
				}
				if (this.bMoveOnXAxis)
				{
					this.moveDirection = this.destPosition - this.zCurPosition;
				}
				else
				{
					this.moveDirection = this.destPosition - location;
				}
				this.lerpDirection = this.moveDirection;
				if (this.bMoveRotate && !this.bMoveOnXAxis)
				{
					this.RotateMoveBullet(this.moveDirection);
				}
				int num2;
				if (!this.shouldUseAcceleration)
				{
					num2 = this.velocity * delta / 1000;
				}
				else
				{
					long num3 = (long)this.lastVelocity * (long)delta + (long)this.acceleration * (long)delta * (long)delta / 2L / 1000L;
					num3 /= 1000L;
					num2 = (int)num3;
					this.lastVelocity += this.acceleration * delta / 1000;
				}
				if ((long)num2 * (long)num2 >= this.moveDirection.sqrMagnitudeLong2D && this.bReachDestStop)
				{
					int magnitude2D = (this.destPosition - this.moveActor.handle.location).magnitude2D;
					if (this.bMoveOnXAxis)
					{
						this.destPosition += this.xDestPosition;
					}
					this.moveActor.handle.location = this.destPosition;
					this.stopCondtion = true;
					if (this.moveActor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Bullet)
					{
						BulletWrapper bulletWrapper = this.moveActor.handle.ActorControl as BulletWrapper;
						if (bulletWrapper != null && bulletWrapper.GetMoveCollisiong())
						{
							bulletWrapper.SetMoveDelta(magnitude2D);
						}
					}
				}
				else
				{
					VInt3 vInt2;
					if (this.gravity < 0)
					{
						this.moveDirection.y = 0;
						vInt2 = location + this.moveDirection.NormalizeTo(num2);
						vInt2.y += this.gravityControler.GetMotionDeltaDistance(delta);
						VInt vInt3;
						if (PathfindingUtility.GetGroundY(this.destPosition, out vInt3) && vInt2.y < vInt3.i)
						{
							vInt2.y = vInt3.i;
						}
					}
					else
					{
						vInt2 = location + this.moveDirection.NormalizeTo(num2);
					}
					if (this.bMoveOnXAxis)
					{
						this.zCurPosition += this.moveDirection.NormalizeTo(num2);
						int motionDeltaDistance = this.xControler.GetMotionDeltaDistance(delta);
						VInt3 vInt4 = this.xDirection;
						vInt4.NormalizeTo(Math.Abs(motionDeltaDistance));
						if (motionDeltaDistance < 0)
						{
							vInt4 = -vInt4;
						}
						vInt2 += vInt4;
						if (this.bMoveRotate)
						{
							VInt3 dir = vInt2 - location;
							this.RotateMoveBullet(dir);
						}
					}
					if (this.moveActor.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Bullet)
					{
						BulletWrapper bulletWrapper2 = this.moveActor.handle.ActorControl as BulletWrapper;
						if (bulletWrapper2 != null && bulletWrapper2.GetMoveCollisiong())
						{
							bulletWrapper2.SetMoveDelta(num2);
						}
					}
					this.moveActor.handle.location = vInt2;
				}
			}
			SkillUseContext refParamObject = _action.refParams.GetRefParamObject<SkillUseContext>("SkillContext");
			if (refParamObject != null)
			{
				refParamObject.EffectPos = this.moveActor.handle.location;
				refParamObject.EffectDir = this.moveDirection;
			}
		}

		public int ProcessSubdivide(Action _action, Track _track, int _localTime, int _count)
		{
			if (!this.moveActor || this.stopCondtion || _count <= 0)
			{
				return 0;
			}
			int num = _localTime - this.lastTime;
			this.lastTime = _localTime;
			int num2 = num / _count;
			int num3 = num - num2;
			this.lastTime -= num3;
			this.ProcessInner(_action, _track, num2);
			return num3;
		}

		public void Process(Action _action, Track _track, int _localTime)
		{
			if (!this.moveActor || this.stopCondtion)
			{
				return;
			}
			int delta = _localTime - this.lastTime;
			this.lastTime = _localTime;
			this.ProcessInner(_action, _track, delta);
			this.FindEnemy(_action, _localTime);
		}

		private void FindEnemy(Action _action, int _localTime)
		{
			if (this.MoveType != ActorMoveType.RotateBody || _localTime < this.rotateBodyFindEnemyLatency)
			{
				return;
			}
			SkillComponent skillControl = this.originateActor.handle.SkillControl;
			if (Singleton<FrameSynchr>.GetInstance().LogicFrameTick - skillControl.RotateBodyBulletFindEnemyLogicFrameTick < (ulong)((long)this.rotateBodyFindEnemyCd))
			{
				return;
			}
			List<PoolObjHandle<ActorRoot>> heroActors = Singleton<GameObjMgr>.instance.HeroActors;
			for (int i = 0; i < heroActors.Count; i++)
			{
				if (this.IsValidEnemy(heroActors[i]))
				{
					_action.refParams.AddRefParam("FindEnemyActor", heroActors[i]);
					skillControl.RotateBodyBulletFindEnemyLogicFrameTick = Singleton<FrameSynchr>.GetInstance().LogicFrameTick;
					this.stopCondtion = true;
					return;
				}
			}
			List<PoolObjHandle<ActorRoot>> monsterActors = Singleton<GameObjMgr>.instance.MonsterActors;
			for (int j = 0; j < monsterActors.Count; j++)
			{
				if (this.IsValidEnemy(monsterActors[j]))
				{
					_action.refParams.AddRefParam("FindEnemyActor", monsterActors[j]);
					skillControl.RotateBodyBulletFindEnemyLogicFrameTick = Singleton<FrameSynchr>.GetInstance().LogicFrameTick;
					this.stopCondtion = true;
					return;
				}
			}
		}

		private bool IsValidEnemy(PoolObjHandle<ActorRoot> enemyActor)
		{
			return enemyActor && enemyActor.handle.TheActorMeta.ActorCamp != this.originateActor.handle.TheActorMeta.ActorCamp && (this.moveActor.handle.location - enemyActor.handle.location).sqrMagnitudeLong2D <= (long)(this.rotateBodyFindEnemyRadius * this.rotateBodyFindEnemyRadius);
		}
	}
}
