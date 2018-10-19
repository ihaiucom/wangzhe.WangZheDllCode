using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using ResData;
using System;
using System.Collections.Generic;

namespace AGE
{
	public class HitTriggerDurationContext
	{
		private struct STriggeredBuffContext
		{
			public PoolObjHandle<ActorRoot> actor;

			public int buffId;
		}

		public const uint MAX_TYPE_FILTERS = 4u;

		public const uint MAX_TYPE_ACTORLIST = 4u;

		public int triggerId;

		public int attackerId;

		public int triggerInterval;

		public bool bFilterEnemy;

		public bool bFilterFriend;

		public bool bFilterHero;

		public bool bFileterMonter;

		public bool bFileterOrgan;

		public bool bFilterEye;

		public bool bFilterMyself;

		public bool bFilterDead;

		public bool bFilterDeadControlHero;

		public bool bFilterCurrentTarget;

		public bool bFilterMoveDirection;

		public int Angle;

		public int TriggerActorCount;

		public HitTriggerSelectMode SelectMode;

		public int TriggerActorInterval;

		public int CollideMaxCount;

		public bool bEdgeCheck;

		public bool bExtraBuff;

		[AssetReference(AssetRefType.SkillCombine)]
		public int SelfSkillCombineID_1;

		[AssetReference(AssetRefType.SkillCombine)]
		public int SelfSkillCombineID_2;

		[AssetReference(AssetRefType.SkillCombine)]
		public int SelfSkillCombineID_3;

		[AssetReference(AssetRefType.SkillCombine)]
		public int TargetSkillCombine_1;

		[AssetReference(AssetRefType.SkillCombine)]
		public int TargetSkillCombine_2;

		[AssetReference(AssetRefType.SkillCombine)]
		public int TargetSkillCombine_3;

		public bool TargetSkillLeaveRemove_1;

		public bool TargetSkillLeaveRemove_2;

		public bool TargetSkillLeaveRemove_3;

		public bool bTriggerBullet;

		[AssetReference(AssetRefType.Action)]
		public string BulletActionName;

		public bool bAgeImmeExcute;

		public bool bUseTriggerObj;

		public bool bCheckSight;

		public bool bTriggerMode;

		public bool bTriggerBounceBullet;

		private List<PoolObjHandle<ActorRoot>> triggerHeroList;

		private List<PoolObjHandle<ActorRoot>> triggerMonsterList;

		private List<PoolObjHandle<ActorRoot>> triggerOrganList;

		private List<PoolObjHandle<ActorRoot>> triggerEyeList;

		private List<PoolObjHandle<ActorRoot>> triggerPriority;

		private List<PoolObjHandle<ActorRoot>> collidedActors;

		public bool hit;

		private int residueActorCount;

		private Dictionary<uint, int> collideCountMap;

		private Dictionary<uint, int> collideTimeMap;

		private PoolObjHandle<ActorRoot> attackActor;

		private PoolObjHandle<ActorRoot> triggerActor;

		private int lastTime;

		private int localTime;

		private int deltaTime;

		private int triggerCount;

		private bool bFirstProcess;

		private bool bHitTargetHero;

		private VInt3 HitTargetHeroPos;

		private bool[] type_Filters = new bool[4];

		private List<PoolObjHandle<ActorRoot>>[] type_actorList = new List<PoolObjHandle<ActorRoot>>[4];

		private List<BuffSkill> RemoveSkillList;

		private SkillUseContext skillContext;

		private List<HitTriggerDurationContext.STriggeredBuffContext> TriggeredBuffContextList;

		private PoolObjHandle<ActorRoot> _coordInActor;

		private VCollisionShape _coordShape;

		private SceneManagement.Process _coordHandler;

		public HitTriggerDurationContext()
		{
			this.triggerId = 0;
			this.attackerId = 0;
			this.triggerInterval = 30;
			this.bFilterEnemy = false;
			this.bFilterFriend = true;
			this.bFilterHero = false;
			this.bFileterMonter = false;
			this.bFileterOrgan = false;
			this.bFilterMyself = true;
			this.bFilterDead = true;
			this.bFilterDeadControlHero = true;
			this.bFilterCurrentTarget = false;
			this.bFilterMoveDirection = false;
			this.Angle = -1;
			this.TriggerActorCount = -1;
			this.SelectMode = HitTriggerSelectMode.RandomMode;
			this.TriggerActorInterval = 30;
			this.CollideMaxCount = -1;
			this.bEdgeCheck = false;
			this.bExtraBuff = false;
			this.SelfSkillCombineID_1 = 0;
			this.SelfSkillCombineID_2 = 0;
			this.SelfSkillCombineID_3 = 0;
			this.TargetSkillCombine_1 = 0;
			this.TargetSkillCombine_2 = 0;
			this.TargetSkillCombine_3 = 0;
			this.bTriggerBullet = false;
			this.BulletActionName = null;
			this.bAgeImmeExcute = false;
			this.triggerHeroList = new List<PoolObjHandle<ActorRoot>>();
			this.triggerMonsterList = new List<PoolObjHandle<ActorRoot>>();
			this.triggerOrganList = new List<PoolObjHandle<ActorRoot>>();
			this.triggerEyeList = new List<PoolObjHandle<ActorRoot>>();
			this.triggerPriority = new List<PoolObjHandle<ActorRoot>>();
			this.collidedActors = new List<PoolObjHandle<ActorRoot>>();
			this.hit = false;
			this.residueActorCount = 0;
			this.collideCountMap = new Dictionary<uint, int>();
			this.collideTimeMap = new Dictionary<uint, int>();
			this.attackActor = default(PoolObjHandle<ActorRoot>);
			this.triggerActor = default(PoolObjHandle<ActorRoot>);
			this.lastTime = 0;
			this.localTime = 0;
			this.deltaTime = 0;
			this.triggerCount = 0;
			this.bFirstProcess = true;
			this.bUseTriggerObj = true;
			this.bCheckSight = false;
			this.bHitTargetHero = false;
			this.HitTargetHeroPos = VInt3.zero;
			this._coordInActor = default(PoolObjHandle<ActorRoot>);
			this._coordShape = null;
			this._coordHandler = new SceneManagement.Process(this.FilterCoordActor);
			this.bTriggerMode = false;
			this.bTriggerBounceBullet = false;
			this.TriggeredBuffContextList = new List<HitTriggerDurationContext.STriggeredBuffContext>();
			this.bFilterEye = true;
			this.TargetSkillLeaveRemove_1 = false;
			this.TargetSkillLeaveRemove_2 = false;
			this.TargetSkillLeaveRemove_3 = false;
			this.RemoveSkillList = new List<BuffSkill>();
			this.skillContext = null;
		}

		public void CopyData(ref HitTriggerDurationContext r)
		{
			this.triggerHeroList.Clear();
			this.triggerMonsterList.Clear();
			this.triggerOrganList.Clear();
			this.triggerEyeList.Clear();
			this.triggerPriority.Clear();
			this.collidedActors.Clear();
			this.hit = r.hit;
			this.residueActorCount = r.residueActorCount;
			this.collideCountMap.Clear();
			this.collideTimeMap.Clear();
			this.attackActor = r.attackActor;
			this.triggerActor = r.triggerActor;
			this.lastTime = r.lastTime;
			this.localTime = r.localTime;
			this.deltaTime = r.deltaTime;
			this.triggerCount = r.triggerCount;
			this.bFirstProcess = r.bFirstProcess;
			this.bUseTriggerObj = r.bUseTriggerObj;
			this.bCheckSight = r.bCheckSight;
			this.bTriggerMode = r.bTriggerMode;
			this.bTriggerBounceBullet = r.bTriggerBounceBullet;
			this.TriggeredBuffContextList.Clear();
			this.bFilterEye = r.bFilterEye;
			this.TargetSkillLeaveRemove_1 = r.TargetSkillLeaveRemove_1;
			this.TargetSkillLeaveRemove_2 = r.TargetSkillLeaveRemove_2;
			this.TargetSkillLeaveRemove_3 = r.TargetSkillLeaveRemove_3;
			this.RemoveSkillList.Clear();
		}

		public void Reset(HitTriggerDuration InTriggerDuration)
		{
			this.triggerId = InTriggerDuration.triggerId;
			this.attackerId = InTriggerDuration.attackerId;
			this.triggerInterval = InTriggerDuration.triggerInterval;
			this.bFilterEnemy = InTriggerDuration.bFilterEnemy;
			this.bFilterFriend = InTriggerDuration.bFilterFriend;
			this.bFilterHero = InTriggerDuration.bFilterHero;
			this.bFileterMonter = InTriggerDuration.bFileterMonter;
			this.bFileterOrgan = InTriggerDuration.bFileterOrgan;
			this.bFilterEye = InTriggerDuration.bFilterEye;
			this.bFilterDead = InTriggerDuration.bFilterDead;
			this.bFilterDeadControlHero = InTriggerDuration.bFilterDeadControlHero;
			this.bFilterCurrentTarget = InTriggerDuration.bFilterCurrentTarget;
			this.bFilterMoveDirection = InTriggerDuration.bFilterMoveDirection;
			this.Angle = InTriggerDuration.Angle;
			this.bFilterMyself = InTriggerDuration.bFilterMyself;
			this.TriggerActorCount = InTriggerDuration.TriggerActorCount;
			this.SelectMode = InTriggerDuration.SelectMode;
			this.TriggerActorInterval = InTriggerDuration.TriggerActorInterval;
			this.CollideMaxCount = InTriggerDuration.CollideMaxCount;
			this.bEdgeCheck = InTriggerDuration.bEdgeCheck;
			this.bExtraBuff = InTriggerDuration.bExtraBuff;
			this.SelfSkillCombineID_1 = InTriggerDuration.SelfSkillCombineID_1;
			this.SelfSkillCombineID_2 = InTriggerDuration.SelfSkillCombineID_2;
			this.SelfSkillCombineID_3 = InTriggerDuration.SelfSkillCombineID_3;
			this.TargetSkillCombine_1 = InTriggerDuration.TargetSkillCombine_1;
			this.TargetSkillCombine_2 = InTriggerDuration.TargetSkillCombine_2;
			this.TargetSkillCombine_3 = InTriggerDuration.TargetSkillCombine_3;
			this.bTriggerBullet = InTriggerDuration.bTriggerBullet;
			this.BulletActionName = InTriggerDuration.BulletActionName;
			this.bAgeImmeExcute = InTriggerDuration.bAgeImmeExcute;
			this.bUseTriggerObj = InTriggerDuration.bUseTriggerObj;
			this.bCheckSight = InTriggerDuration.bCheckSight;
			this.bTriggerMode = InTriggerDuration.bTriggerMode;
			this.bTriggerBounceBullet = InTriggerDuration.bTriggerBounceBullet;
			this.TargetSkillLeaveRemove_1 = InTriggerDuration.TargetSkillLeaveRemove_1;
			this.TargetSkillLeaveRemove_2 = InTriggerDuration.TargetSkillLeaveRemove_2;
			this.TargetSkillLeaveRemove_3 = InTriggerDuration.TargetSkillLeaveRemove_3;
			this.RemoveSkillList.Clear();
		}

		public void Reset(BulletTriggerDuration InBulletTrigger)
		{
			this.triggerId = InBulletTrigger.triggerId;
			this.attackerId = InBulletTrigger.attackerId;
			this.triggerInterval = InBulletTrigger.triggerInterval;
			this.bFilterEnemy = InBulletTrigger.bFilterEnemy;
			this.bFilterFriend = InBulletTrigger.bFilterFriend;
			this.bFilterHero = InBulletTrigger.bFilterHero;
			this.bFileterMonter = InBulletTrigger.bFileterMonter;
			this.bFileterOrgan = InBulletTrigger.bFileterOrgan;
			this.bFilterEye = true;
			this.bFilterDead = InBulletTrigger.bFilterDead;
			this.bFilterDeadControlHero = true;
			this.bFilterCurrentTarget = false;
			this.bFilterMoveDirection = false;
			this.bFilterMyself = InBulletTrigger.bFilterMyself;
			this.TriggerActorCount = InBulletTrigger.TriggerActorCount;
			this.SelectMode = InBulletTrigger.SelectMode;
			this.TriggerActorInterval = InBulletTrigger.TriggerActorInterval;
			this.CollideMaxCount = InBulletTrigger.CollideMaxCount;
			this.bEdgeCheck = InBulletTrigger.bEdgeCheck;
			this.bExtraBuff = InBulletTrigger.bExtraBuff;
			this.SelfSkillCombineID_1 = InBulletTrigger.SelfSkillCombineID_1;
			this.SelfSkillCombineID_2 = InBulletTrigger.SelfSkillCombineID_2;
			this.SelfSkillCombineID_3 = InBulletTrigger.SelfSkillCombineID_3;
			this.TargetSkillCombine_1 = InBulletTrigger.TargetSkillCombine_1;
			this.TargetSkillCombine_2 = InBulletTrigger.TargetSkillCombine_2;
			this.TargetSkillCombine_3 = InBulletTrigger.TargetSkillCombine_3;
			this.bTriggerBullet = InBulletTrigger.bTriggerBullet;
			this.BulletActionName = InBulletTrigger.BulletActionName;
			this.bAgeImmeExcute = InBulletTrigger.bAgeImmeExcute;
			this.bTriggerMode = false;
			this.bTriggerBounceBullet = false;
			this.TargetSkillLeaveRemove_1 = false;
			this.TargetSkillLeaveRemove_2 = false;
			this.TargetSkillLeaveRemove_3 = false;
		}

		public void OnUse()
		{
			this.triggerHeroList.Clear();
			this.triggerMonsterList.Clear();
			this.triggerOrganList.Clear();
			this.triggerEyeList.Clear();
			this.triggerPriority.Clear();
			this.collidedActors.Clear();
			this.hit = false;
			this.residueActorCount = 0;
			this.bEdgeCheck = false;
			this.collideCountMap.Clear();
			this.collideTimeMap.Clear();
			this.attackActor.Release();
			this.triggerActor.Release();
			this.lastTime = 0;
			this.localTime = 0;
			this.deltaTime = 0;
			this.triggerCount = 0;
			this.bFirstProcess = true;
			this.bUseTriggerObj = true;
			this.bCheckSight = false;
			this.bHitTargetHero = false;
			this.HitTargetHeroPos = VInt3.zero;
			this.bTriggerMode = false;
			this.bTriggerBounceBullet = false;
			this.TriggeredBuffContextList.Clear();
			this.bFilterEye = true;
			this.RemoveSkillList.Clear();
			this.bFilterMoveDirection = false;
			this.Angle = -1;
			int num = 0;
			while ((long)num < 4L)
			{
				this.type_Filters[num] = true;
				num++;
			}
			int num2 = 0;
			while ((long)num2 < 4L)
			{
				this.type_actorList[num2] = null;
				num2++;
			}
			this.skillContext = null;
		}

		public void Enter(Action _action, Track _track)
		{
			this.hit = false;
			this.triggerCount = 0;
			this.collideCountMap.Clear();
			this.collideTimeMap.Clear();
			this.type_Filters[0] = this.bFilterHero;
			this.type_Filters[1] = this.bFileterMonter;
			this.type_Filters[2] = this.bFileterOrgan;
			this.type_Filters[3] = this.bFilterEye;
			this.type_actorList[0] = this.triggerHeroList;
			this.type_actorList[1] = this.triggerMonsterList;
			this.type_actorList[2] = this.triggerOrganList;
			this.type_actorList[3] = this.triggerEyeList;
			this.triggerActor = _action.GetActorHandle(this.triggerId);
			if (this.bUseTriggerObj)
			{
				if (!this.triggerActor)
				{
					return;
				}
				if (AGE_Helper.GetCollisionShape(this.triggerActor.handle) == null)
				{
					return;
				}
			}
			this.attackActor = _action.GetActorHandle(this.attackerId);
			this.skillContext = _action.refParams.GetRefParamObject<SkillUseContext>("SkillContext");
		}

		public void Leave(Action _action, Track _track)
		{
			if (this.bTriggerMode && this.attackActor)
			{
				int count = this.TriggeredBuffContextList.Count;
				for (int i = 0; i < count; i++)
				{
					HitTriggerDurationContext.STriggeredBuffContext sTriggeredBuffContext = this.TriggeredBuffContextList[i];
					if (sTriggeredBuffContext.actor)
					{
						this.attackActor.handle.SkillControl.RemoveBuff(sTriggeredBuffContext.actor, sTriggeredBuffContext.buffId);
					}
				}
				this.TriggeredBuffContextList.Clear();
			}
			int count2 = this.RemoveSkillList.Count;
			for (int j = 0; j < this.RemoveSkillList.Count; j++)
			{
				BuffSkill buffSkill = this.RemoveSkillList[j];
				if (buffSkill != null && !buffSkill.isFinish)
				{
					if (buffSkill.skillContext.TargetActor)
					{
						buffSkill.skillContext.TargetActor.handle.BuffHolderComp.RemoveBuff(buffSkill);
					}
				}
			}
			this.RemoveSkillList.Clear();
		}

		public void Process(Action _action, Track _track, int _localTime)
		{
			if (!this.attackActor)
			{
				return;
			}
			this.hit = false;
			this.localTime = _localTime;
			if (this.bFirstProcess)
			{
				this.bFirstProcess = false;
				this.HitTrigger(_action);
			}
			else
			{
				this.deltaTime += _localTime - this.lastTime;
				if (this.deltaTime >= this.triggerInterval)
				{
					this.HitTrigger(_action);
					this.deltaTime -= this.triggerInterval;
				}
			}
			this.lastTime = _localTime;
			_action.refParams.SetRefParam("_HitTargetHero", this.bHitTargetHero);
			if (this.bHitTargetHero)
			{
				_action.refParams.SetRefParam("_HitTargetHeroPos", this.HitTargetHeroPos);
			}
		}

		private static bool Intersects(ActorRoot _actor, VCollisionShape _shape, bool bEdge)
		{
			if (_actor == null || _actor.ActorControl.GetNoAbilityFlag(ObjAbilityType.ObjAbiliity_CollisionDetection))
			{
				return false;
			}
			if (bEdge)
			{
				return _actor.shape.EdgeIntersects(_shape);
			}
			return _actor.shape.Intersects(_shape);
		}

		private void FilterCoordActor(ref PoolObjHandle<ActorRoot> actorPtr)
		{
			ActorRoot handle = actorPtr.handle;
			if (handle.shape == null || this.TargetObjTypeFilter(ref this._coordInActor, handle) || !HitTriggerDurationContext.Intersects(handle, this._coordShape, this.bEdgeCheck) || this.TargetCollideTimeFiler(handle) || this.TargetCollideCountFilter(handle) || this.TargetMoveDirectionFilter(ref this._coordInActor, ref actorPtr) || this.TargetCurrentFilter(ref actorPtr))
			{
				return;
			}
			this.collidedActors.Add(actorPtr);
			this.type_actorList[(int)handle.TheActorMeta.ActorType].Add(actorPtr);
		}

		public List<PoolObjHandle<ActorRoot>> GetCollidedActorList(Action _action, PoolObjHandle<ActorRoot> InActor, PoolObjHandle<ActorRoot> triggerActor)
		{
			VCollisionShape vCollisionShape = null;
			if (triggerActor)
			{
				vCollisionShape = triggerActor.handle.shape;
			}
			this.triggerHeroList.Clear();
			this.triggerMonsterList.Clear();
			this.triggerOrganList.Clear();
			this.triggerEyeList.Clear();
			this.triggerPriority.Clear();
			this.collidedActors.Clear();
			if (vCollisionShape == null && this.bUseTriggerObj)
			{
				return null;
			}
			if (this.bUseTriggerObj)
			{
				this._coordInActor = InActor;
				this._coordShape = vCollisionShape;
				SceneManagement instance = Singleton<SceneManagement>.GetInstance();
				SceneManagement.Coordinate coord = default(SceneManagement.Coordinate);
				instance.GetCoord(ref coord, vCollisionShape);
				instance.UpdateDirtyNodes();
				instance.ForeachActors(coord, this._coordHandler);
				this._coordInActor.Release();
				this._coordShape = null;
			}
			else
			{
				List<PoolObjHandle<ActorRoot>> gameActors = Singleton<GameObjMgr>.instance.GameActors;
				int count = gameActors.Count;
				for (int i = 0; i < count; i++)
				{
					PoolObjHandle<ActorRoot> poolObjHandle = gameActors[i];
					if (poolObjHandle)
					{
						ActorRoot handle = poolObjHandle.handle;
						if (!this.TargetObjTypeFilter(ref InActor, handle) && !this.TargetCollideTimeFiler(handle) && !this.TargetCollideCountFilter(handle) && !this.TargetMoveDirectionFilter(ref InActor, ref poolObjHandle) && !this.TargetCurrentFilter(ref poolObjHandle))
						{
							this.collidedActors.Add(poolObjHandle);
							this.type_actorList[(int)handle.TheActorMeta.ActorType].Add(poolObjHandle);
						}
					}
				}
			}
			return this.collidedActors;
		}

		private bool TargetMoveDirectionFilter(ref PoolObjHandle<ActorRoot> sourceActor, ref PoolObjHandle<ActorRoot> targetActor)
		{
			if (!this.bFilterMoveDirection || !sourceActor || !targetActor || this.Angle <= 0)
			{
				return false;
			}
			VInt3 lhs = sourceActor.handle.location - targetActor.handle.location;
			VInt3 forward = targetActor.handle.forward;
			if (lhs.magnitude == 0 || forward.magnitude == 0)
			{
				return false;
			}
			VFactor a = VInt3.AngleInt(lhs, forward);
			VFactor b = VFactor.pi * (long)(this.Angle / 2) / 180L;
			return a > b;
		}

		private bool TargetCurrentFilter(ref PoolObjHandle<ActorRoot> targetActor)
		{
			return this.bFilterCurrentTarget && targetActor && this.skillContext != null && this.skillContext.TargetActor && targetActor == this.skillContext.TargetActor;
		}

		private bool TargetObjTypeFilter(ref PoolObjHandle<ActorRoot> InActor, ActorRoot actor)
		{
			return (actor.ActorControl.IsDeadState && this.bFilterDead && (this.bFilterDeadControlHero || !actor.TheStaticData.TheBaseAttribute.DeadControl)) || (InActor && actor.IsSelfCamp(InActor.handle) && this.bFilterFriend) || (InActor && actor.IsEnemyCamp(InActor.handle) && (this.bFilterEnemy || (!this.bFilterEnemy && actor.ObjLinker.Invincible))) || (actor.TheActorMeta.ActorType >= (ActorTypeDef)this.type_Filters.Length || this.type_Filters[(int)actor.TheActorMeta.ActorType] || (InActor && actor.ObjID == InActor.handle.ObjID && this.bFilterMyself)) || (actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ && !actor.AttackOrderReady) || (this.bCheckSight && !actor.HorizonMarker.IsVisibleFor(InActor.handle.TheActorMeta.ActorCamp)) || (this.bFilterEye && actor.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_EYE);
		}

		private bool TargetCollideTimeFiler(ActorRoot actor)
		{
			int num = 0;
			uint objID = actor.ObjID;
			if (!this.collideTimeMap.TryGetValue(objID, out num))
			{
				return false;
			}
			if (this.localTime - num > this.TriggerActorInterval)
			{
				return false;
			}
			this.collideTimeMap[objID] = num;
			return true;
		}

		private bool TargetCollideCountFilter(ActorRoot actor)
		{
			int num;
			return this.CollideMaxCount > 0 && this.collideCountMap.TryGetValue(actor.ObjID, out num) && num >= this.CollideMaxCount;
		}

		private void CopyTargetList(List<PoolObjHandle<ActorRoot>> _srcList, List<PoolObjHandle<ActorRoot>> _destList, int _count)
		{
			for (int i = 0; i < _count; i++)
			{
				_destList.Add(_srcList[i]);
			}
		}

		private void RandomFindTarget(List<PoolObjHandle<ActorRoot>> _srcList, int _count)
		{
			ushort num = FrameRandom.Random((uint)_srcList.Count);
			for (int i = 0; i < _count; i++)
			{
				this.triggerPriority.Add(_srcList[(int)num]);
				num += 1;
				num %= (ushort)_srcList.Count;
			}
		}

		public static int GetTargetHpRate(PoolObjHandle<ActorRoot> _inActor)
		{
			int num = _inActor.handle.ValueComponent.actorHp * 100;
			int totalValue = _inActor.handle.ValueComponent.mActorValue[RES_FUNCEFT_TYPE.RES_FUNCEFT_MAXHP].totalValue;
			if (totalValue != 0)
			{
				num /= totalValue;
			}
			return num;
		}

		private void LowestHpFindTarget(List<PoolObjHandle<ActorRoot>> _srcList, int _count)
		{
			_srcList.Sort(delegate(PoolObjHandle<ActorRoot> a, PoolObjHandle<ActorRoot> b)
			{
				int targetHpRate = HitTriggerDurationContext.GetTargetHpRate(a);
				int targetHpRate2 = HitTriggerDurationContext.GetTargetHpRate(b);
				if (targetHpRate == targetHpRate2)
				{
					return 0;
				}
				if (targetHpRate < targetHpRate2)
				{
					return -1;
				}
				return 1;
			});
			for (int i = 0; i < _count; i++)
			{
				this.triggerPriority.Add(_srcList[i]);
			}
		}

		private void FindEyeTarget(List<PoolObjHandle<ActorRoot>> _srcList, int _count)
		{
			_srcList.Sort(delegate(PoolObjHandle<ActorRoot> a, PoolObjHandle<ActorRoot> b)
			{
				EyeWrapper eyeWrapper = (EyeWrapper)a.handle.ActorControl;
				EyeWrapper eyeWrapper2 = (EyeWrapper)b.handle.ActorControl;
				int lifeTime = eyeWrapper.LifeTime;
				int lifeTime2 = eyeWrapper2.LifeTime;
				int targetHpRate = HitTriggerDurationContext.GetTargetHpRate(a);
				int targetHpRate2 = HitTriggerDurationContext.GetTargetHpRate(b);
				if (targetHpRate < targetHpRate2)
				{
					return -1;
				}
				if (targetHpRate > targetHpRate2)
				{
					return 1;
				}
				if (lifeTime > lifeTime2)
				{
					return -1;
				}
				if (lifeTime < lifeTime2)
				{
					return 1;
				}
				return 0;
			});
			for (int i = 0; i < _count; i++)
			{
				this.triggerPriority.Add(_srcList[i]);
			}
		}

		private bool PriorityFindTarget(List<PoolObjHandle<ActorRoot>> triggerList)
		{
			if (this.residueActorCount < triggerList.Count)
			{
				if (this.SelectMode == HitTriggerSelectMode.RandomMode)
				{
					this.RandomFindTarget(triggerList, this.residueActorCount);
				}
				else if (this.SelectMode == HitTriggerSelectMode.LowestHp)
				{
					this.LowestHpFindTarget(triggerList, this.residueActorCount);
				}
				else if (this.SelectMode == HitTriggerSelectMode.SelectEyeMode)
				{
					this.FindEyeTarget(triggerList, this.residueActorCount);
				}
				return true;
			}
			this.CopyTargetList(triggerList, this.triggerPriority, triggerList.Count);
			this.residueActorCount -= triggerList.Count;
			return this.residueActorCount == 0;
		}

		private void PriorityTrigger(Action _action)
		{
			this.triggerPriority.Clear();
			this.residueActorCount = this.TriggerActorCount;
			if (!this.PriorityFindTarget(this.triggerHeroList) && !this.PriorityFindTarget(this.triggerMonsterList) && !this.PriorityFindTarget(this.triggerOrganList))
			{
				this.PriorityFindTarget(this.triggerEyeList);
			}
			for (int i = 0; i < this.triggerPriority.Count; i++)
			{
				PoolObjHandle<ActorRoot> poolObjHandle = this.triggerPriority[i];
				this.TriggerAction(_action, ref poolObjHandle);
			}
		}

		private void HitTrigger(Action _action)
		{
			if (!this.attackActor || this.skillContext == null)
			{
				return;
			}
			this.GetCollidedActorList(_action, this.attackActor, this.triggerActor);
			if (this.collidedActors != null && this.collidedActors.Count > 0)
			{
				if (this.bTriggerBounceBullet && !this.skillContext.TargetActor)
				{
					this.skillContext.TargetActor = this.collidedActors[0];
				}
				SkillChooseTargetEventParam skillChooseTargetEventParam = new SkillChooseTargetEventParam(this.attackActor, this.attackActor, this.collidedActors.Count);
				Singleton<GameEventSys>.instance.SendEvent<SkillChooseTargetEventParam>(GameEventDef.Event_HitTrigger, ref skillChooseTargetEventParam);
				if (this.TriggerActorCount > 0 && this.TriggerActorCount < this.collidedActors.Count)
				{
					this.PriorityTrigger(_action);
				}
				else
				{
					for (int i = 0; i < this.collidedActors.Count; i++)
					{
						PoolObjHandle<ActorRoot> poolObjHandle = this.collidedActors[i];
						this.TriggerAction(_action, ref poolObjHandle);
					}
				}
			}
			if (this.bTriggerMode)
			{
				int count = this.TriggeredBuffContextList.Count;
				if (count > 0 && this.attackActor)
				{
					for (int j = count - 1; j >= 0; j--)
					{
						HitTriggerDurationContext.STriggeredBuffContext sTriggeredBuffContext = this.TriggeredBuffContextList[j];
						if (!this.collidedActors.Contains(sTriggeredBuffContext.actor))
						{
							this.attackActor.handle.SkillControl.RemoveBuff(sTriggeredBuffContext.actor, sTriggeredBuffContext.buffId);
							this.TriggeredBuffContextList.RemoveAt(j);
						}
					}
				}
			}
		}

		private void TriggerAction(Action _action, ref PoolObjHandle<ActorRoot> target)
		{
			if (!this.attackActor)
			{
				return;
			}
			uint objID = target.handle.ObjID;
			int num;
			if (this.collideCountMap.TryGetValue(objID, out num))
			{
				num++;
				this.collideCountMap[objID] = num;
			}
			else
			{
				this.collideCountMap.Add(objID, 1);
			}
			int num2 = 0;
			if (this.collideTimeMap.TryGetValue(objID, out num2))
			{
				this.collideTimeMap[objID] = this.localTime;
			}
			else
			{
				this.collideTimeMap.Add(objID, this.localTime);
			}
			if (this.skillContext == null)
			{
				return;
			}
			this.skillContext.EffectCount++;
			this.triggerCount++;
			this.skillContext.EffectCountInSingleTrigger = this.triggerCount;
			if (!this.bTriggerMode)
			{
				this.attackActor.handle.SkillControl.SpawnBuff(this.skillContext.Originator, this.skillContext, this.SelfSkillCombineID_1, false);
				this.attackActor.handle.SkillControl.SpawnBuff(this.skillContext.Originator, this.skillContext, this.SelfSkillCombineID_2, false);
				this.attackActor.handle.SkillControl.SpawnBuff(this.skillContext.Originator, this.skillContext, this.SelfSkillCombineID_3, false);
			}
			else
			{
				if (this.skillContext.Originator && this.SelfSkillCombineID_1 > 0)
				{
					HitTriggerDurationContext.STriggeredBuffContext inPoint = default(HitTriggerDurationContext.STriggeredBuffContext);
					inPoint.actor = this.skillContext.Originator;
					inPoint.buffId = this.SelfSkillCombineID_1;
					if (BaseAlgorithm.AddUniqueItem<HitTriggerDurationContext.STriggeredBuffContext>(this.TriggeredBuffContextList, inPoint))
					{
						this.attackActor.handle.SkillControl.SpawnBuff(this.skillContext.Originator, this.skillContext, this.SelfSkillCombineID_1, false);
					}
				}
				if (this.skillContext.Originator && this.SelfSkillCombineID_2 > 0)
				{
					HitTriggerDurationContext.STriggeredBuffContext inPoint2 = default(HitTriggerDurationContext.STriggeredBuffContext);
					inPoint2.actor = this.skillContext.Originator;
					inPoint2.buffId = this.SelfSkillCombineID_2;
					if (BaseAlgorithm.AddUniqueItem<HitTriggerDurationContext.STriggeredBuffContext>(this.TriggeredBuffContextList, inPoint2))
					{
						this.attackActor.handle.SkillControl.SpawnBuff(this.skillContext.Originator, this.skillContext, this.SelfSkillCombineID_2, false);
					}
				}
				if (this.skillContext.Originator && this.SelfSkillCombineID_3 > 0)
				{
					HitTriggerDurationContext.STriggeredBuffContext inPoint3 = default(HitTriggerDurationContext.STriggeredBuffContext);
					inPoint3.actor = this.skillContext.Originator;
					inPoint3.buffId = this.SelfSkillCombineID_3;
					if (BaseAlgorithm.AddUniqueItem<HitTriggerDurationContext.STriggeredBuffContext>(this.TriggeredBuffContextList, inPoint3))
					{
						this.attackActor.handle.SkillControl.SpawnBuff(this.skillContext.Originator, this.skillContext, this.SelfSkillCombineID_3, false);
					}
				}
			}
			if (target)
			{
				this.hit = true;
				if (target.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero && !this.bHitTargetHero)
				{
					this.bHitTargetHero = true;
					this.HitTargetHeroPos = target.handle.location;
				}
				this.skillContext.EffectDir = this.attackActor.handle.forward;
				bool flag = false;
				BuffSkill buffSkill = null;
				BuffSkill buffSkill2 = null;
				BuffSkill buffSkill3 = null;
				if (!this.bTriggerMode)
				{
					this.attackActor.handle.SkillControl.SpawnBuff(target, this.skillContext, this.TargetSkillCombine_1, ref buffSkill, this.bExtraBuff);
					this.attackActor.handle.SkillControl.SpawnBuff(target, this.skillContext, this.TargetSkillCombine_2, ref buffSkill2, this.bExtraBuff);
					this.attackActor.handle.SkillControl.SpawnBuff(target, this.skillContext, this.TargetSkillCombine_3, ref buffSkill3, this.bExtraBuff);
					if ((buffSkill != null && buffSkill.cfgData.bNotGetHate == 0) || (buffSkill2 != null && buffSkill2.cfgData.bNotGetHate == 0) || (buffSkill3 != null && buffSkill3.cfgData.bNotGetHate == 0))
					{
						flag = true;
					}
				}
				else
				{
					if (this.TargetSkillCombine_1 > 0)
					{
						HitTriggerDurationContext.STriggeredBuffContext inPoint4 = default(HitTriggerDurationContext.STriggeredBuffContext);
						inPoint4.actor = target;
						inPoint4.buffId = this.TargetSkillCombine_1;
						if (BaseAlgorithm.AddUniqueItem<HitTriggerDurationContext.STriggeredBuffContext>(this.TriggeredBuffContextList, inPoint4))
						{
							this.attackActor.handle.SkillControl.SpawnBuff(target, this.skillContext, this.TargetSkillCombine_1, ref buffSkill, false);
						}
						if (buffSkill != null && buffSkill.cfgData.bNotGetHate == 0)
						{
							flag = true;
						}
					}
					if (this.TargetSkillCombine_2 > 0)
					{
						HitTriggerDurationContext.STriggeredBuffContext inPoint5 = default(HitTriggerDurationContext.STriggeredBuffContext);
						inPoint5.actor = target;
						inPoint5.buffId = this.TargetSkillCombine_2;
						if (BaseAlgorithm.AddUniqueItem<HitTriggerDurationContext.STriggeredBuffContext>(this.TriggeredBuffContextList, inPoint5))
						{
							this.attackActor.handle.SkillControl.SpawnBuff(target, this.skillContext, this.TargetSkillCombine_2, ref buffSkill2, false);
						}
						if (buffSkill2 != null && buffSkill2.cfgData.bNotGetHate == 0)
						{
							flag = true;
						}
					}
					if (this.TargetSkillCombine_3 > 0)
					{
						HitTriggerDurationContext.STriggeredBuffContext inPoint6 = default(HitTriggerDurationContext.STriggeredBuffContext);
						inPoint6.actor = target;
						inPoint6.buffId = this.TargetSkillCombine_3;
						if (BaseAlgorithm.AddUniqueItem<HitTriggerDurationContext.STriggeredBuffContext>(this.TriggeredBuffContextList, inPoint6))
						{
							this.attackActor.handle.SkillControl.SpawnBuff(target, this.skillContext, this.TargetSkillCombine_3, ref buffSkill3, false);
						}
						if (buffSkill3 != null && buffSkill3.cfgData.bNotGetHate == 0)
						{
							flag = true;
						}
					}
				}
				if (this.TargetSkillLeaveRemove_1 && buffSkill != null)
				{
					this.RemoveSkillList.Add(buffSkill);
				}
				if (this.TargetSkillLeaveRemove_2 && buffSkill2 != null)
				{
					this.RemoveSkillList.Add(buffSkill2);
				}
				if (this.TargetSkillLeaveRemove_3 && buffSkill3 != null)
				{
					this.RemoveSkillList.Add(buffSkill3);
				}
				if (flag)
				{
					target.handle.ActorControl.BeAttackHit(this.attackActor, this.skillContext.bExposing);
				}
			}
			if (this.bTriggerBullet && this.BulletActionName != null && this.BulletActionName.Length > 0)
			{
				this.skillContext.AppointType = SkillRangeAppointType.Target;
				this.skillContext.TargetActor = target;
				this.attackActor.handle.SkillControl.SpawnBullet(this.skillContext, this.BulletActionName, false, this.bAgeImmeExcute, 0, 0);
			}
		}
	}
}
