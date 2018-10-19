using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;
using System.Collections.Generic;

namespace AGE
{
	[EventCategory("MMGame/Skill")]
	public class HitTriggerTick : TickEvent
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int targetId = -1;

		[ObjectTemplate(new Type[]
		{

		})]
		public int triggerId;

		public bool bulletHit;

		public int victimId = -1;

		public bool lastHit;

		public bool bSkillCombineChoose;

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

		public bool bCheckSight;

		public bool bActOnCallMonster;

		public bool bFindTargetByRotateBodyBullet;

		private VCollisionShape shape;

		private static List<PoolObjHandle<ActorRoot>> targetActors = new List<PoolObjHandle<ActorRoot>>();

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			HitTriggerTick hitTriggerTick = src as HitTriggerTick;
			this.targetId = hitTriggerTick.targetId;
			this.triggerId = hitTriggerTick.triggerId;
			this.victimId = hitTriggerTick.victimId;
			this.lastHit = hitTriggerTick.lastHit;
			this.SelfSkillCombineID_1 = hitTriggerTick.SelfSkillCombineID_1;
			this.SelfSkillCombineID_2 = hitTriggerTick.SelfSkillCombineID_2;
			this.SelfSkillCombineID_3 = hitTriggerTick.SelfSkillCombineID_3;
			this.TargetSkillCombine_1 = hitTriggerTick.TargetSkillCombine_1;
			this.TargetSkillCombine_2 = hitTriggerTick.TargetSkillCombine_2;
			this.TargetSkillCombine_3 = hitTriggerTick.TargetSkillCombine_3;
			this.bCheckSight = hitTriggerTick.bCheckSight;
			this.bSkillCombineChoose = hitTriggerTick.bSkillCombineChoose;
			this.shape = hitTriggerTick.shape;
			this.bActOnCallMonster = hitTriggerTick.bActOnCallMonster;
			this.bFindTargetByRotateBodyBullet = hitTriggerTick.bFindTargetByRotateBodyBullet;
		}

		public override BaseEvent Clone()
		{
			HitTriggerTick hitTriggerTick = ClassObjPool<HitTriggerTick>.Get();
			hitTriggerTick.CopyData(this);
			return hitTriggerTick;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.bCheckSight = false;
			this.shape = null;
			this.bActOnCallMonster = false;
			this.bFindTargetByRotateBodyBullet = false;
		}

		public override void Process(Action _action, Track _track)
		{
			PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.targetId);
			if (!actorHandle)
			{
				return;
			}
			SkillComponent skillControl = actorHandle.handle.SkillControl;
			if (skillControl == null)
			{
				return;
			}
			BaseSkill refParamObject = _action.refParams.GetRefParamObject<BaseSkill>("SkillObj");
			if (refParamObject != null)
			{
				List<PoolObjHandle<ActorRoot>> list = this.FilterTargetByTriggerRegion(_action, actorHandle, refParamObject);
				if (this.bActOnCallMonster)
				{
					HeroWrapper heroWrapper = actorHandle.handle.ActorControl as HeroWrapper;
					if (list != null && heroWrapper != null && heroWrapper.hasCalledMonster)
					{
						list.Clear();
						list.Add(heroWrapper.CallMonster);
					}
				}
				if (list != null && list.Count > 0)
				{
					SkillChooseTargetEventParam skillChooseTargetEventParam = new SkillChooseTargetEventParam(actorHandle, actorHandle, list.Count);
					Singleton<GameEventSys>.instance.SendEvent<SkillChooseTargetEventParam>(GameEventDef.Event_HitTrigger, ref skillChooseTargetEventParam);
				}
				SkillUseContext refParamObject2 = _action.refParams.GetRefParamObject<SkillUseContext>("SkillContext");
				if (refParamObject2 != null)
				{
					int num = 0;
					refParamObject2.EffectCountInSingleTrigger = 1;
					if (this.bSkillCombineChoose && _action.refParams.GetRefParam("SpecifiedSkillCombineIndex", ref num))
					{
						switch (num)
						{
						case 1:
							skillControl.SpawnBuff(actorHandle, refParamObject2, this.SelfSkillCombineID_1, false);
							break;
						case 2:
							skillControl.SpawnBuff(actorHandle, refParamObject2, this.SelfSkillCombineID_2, false);
							break;
						case 3:
							skillControl.SpawnBuff(actorHandle, refParamObject2, this.SelfSkillCombineID_3, false);
							break;
						}
					}
					else
					{
						skillControl.SpawnBuff(actorHandle, refParamObject2, this.SelfSkillCombineID_1, false);
						skillControl.SpawnBuff(actorHandle, refParamObject2, this.SelfSkillCombineID_2, false);
						skillControl.SpawnBuff(actorHandle, refParamObject2, this.SelfSkillCombineID_3, false);
					}
					if (list != null && list.Count > 0)
					{
						for (int i = 0; i < list.Count; i++)
						{
							refParamObject2.EffectDir = actorHandle.handle.forward;
							bool flag = skillControl.SpawnBuff(list[i], refParamObject2, this.TargetSkillCombine_1, false);
							flag |= skillControl.SpawnBuff(list[i], refParamObject2, this.TargetSkillCombine_2, false);
							flag |= skillControl.SpawnBuff(list[i], refParamObject2, this.TargetSkillCombine_3, false);
							if (flag)
							{
								list[i].handle.ActorControl.BeAttackHit(actorHandle, refParamObject2.bExposing);
							}
						}
						list.Clear();
					}
				}
			}
			this.shape = null;
		}

		private List<PoolObjHandle<ActorRoot>> FilterTargetByTriggerRegion(Action _action, PoolObjHandle<ActorRoot> _attackActor, BaseSkill _skill)
		{
			if (!_attackActor || _skill == null)
			{
				return null;
			}
			HitTriggerTick.targetActors.Clear();
			if (this.bFindTargetByRotateBodyBullet)
			{
				PoolObjHandle<ActorRoot> poolObjHandle = default(PoolObjHandle<ActorRoot>);
				_action.refParams.GetRefParam("FindEnemyActor", ref poolObjHandle);
				if (poolObjHandle)
				{
					HitTriggerTick.targetActors.Add(poolObjHandle);
				}
				return HitTriggerTick.targetActors;
			}
			PoolObjHandle<ActorRoot> targetActor = _skill.GetTargetActor();
			if (targetActor && !targetActor.handle.ActorControl.IsDeadState)
			{
				HitTriggerTick.targetActors.Add(targetActor);
			}
			PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.triggerId);
			if (!actorHandle)
			{
				return HitTriggerTick.targetActors;
			}
			this.shape = AGE_Helper.GetCollisionShape(actorHandle);
			if (this.shape != null)
			{
				Singleton<TargetSearcher>.instance.BeginCollidedActorList(_attackActor, this.shape, false, true, null, this.bCheckSight);
				List<PoolObjHandle<ActorRoot>> collidedActors = Singleton<TargetSearcher>.instance.GetCollidedActors();
				if (collidedActors != null && collidedActors.Count > 0)
				{
					List<PoolObjHandle<ActorRoot>>.Enumerator enumerator = collidedActors.GetEnumerator();
					while (enumerator.MoveNext())
					{
						if (HitTriggerTick.targetActors.IndexOf(enumerator.Current) == -1)
						{
							HitTriggerTick.targetActors.Add(enumerator.Current);
						}
					}
				}
				Singleton<TargetSearcher>.instance.EndCollidedActorList();
			}
			return HitTriggerTick.targetActors;
		}
	}
}
