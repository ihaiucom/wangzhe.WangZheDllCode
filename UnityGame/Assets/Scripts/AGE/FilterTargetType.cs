using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameKernal;
using System;

namespace AGE
{
	[EventCategory("MMGame/Skill")]
	public class FilterTargetType : TickCondition
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int targetId = -1;

		public bool bFilterHero;

		public bool bFilterMonter;

		public bool bFilterBoss;

		public bool bFilterOrgan;

		public bool bFilterSameCamp;

		public bool bFilterDiffCamp;

		public bool bFilterEye;

		public bool bOnlySelf;

		public bool bOnlyHostHero;

		public bool bImmediateRevive;

		private bool bCheckFilter = true;

		public override BaseEvent Clone()
		{
			FilterTargetType filterTargetType = ClassObjPool<FilterTargetType>.Get();
			filterTargetType.CopyData(this);
			return filterTargetType;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			FilterTargetType filterTargetType = src as FilterTargetType;
			this.targetId = filterTargetType.targetId;
			this.bFilterHero = filterTargetType.bFilterHero;
			this.bFilterMonter = filterTargetType.bFilterMonter;
			this.bFilterBoss = filterTargetType.bFilterBoss;
			this.bFilterOrgan = filterTargetType.bFilterOrgan;
			this.bFilterSameCamp = filterTargetType.bFilterSameCamp;
			this.bFilterDiffCamp = filterTargetType.bFilterDiffCamp;
			this.bFilterEye = filterTargetType.bFilterEye;
			this.bCheckFilter = filterTargetType.bCheckFilter;
			this.bOnlySelf = filterTargetType.bOnlySelf;
			this.bImmediateRevive = filterTargetType.bImmediateRevive;
			this.bOnlyHostHero = filterTargetType.bOnlyHostHero;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.targetId = -1;
			this.bFilterHero = false;
			this.bFilterMonter = false;
			this.bFilterBoss = false;
			this.bFilterOrgan = false;
			this.bFilterSameCamp = false;
			this.bFilterDiffCamp = false;
			this.bCheckFilter = true;
			this.bOnlySelf = false;
			this.bImmediateRevive = false;
			this.bFilterEye = false;
			this.bOnlyHostHero = false;
		}

		private void HostPlayerHeroOnly(PoolObjHandle<ActorRoot> actorObj)
		{
			Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
			if (hostPlayer == null || !hostPlayer.Captain)
			{
				return;
			}
			this.bCheckFilter = (hostPlayer.Captain == actorObj);
		}

		private void FilterActorCamp(PoolObjHandle<ActorRoot> actorObj)
		{
			Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
			if (hostPlayer == null || !hostPlayer.Captain)
			{
				return;
			}
			if (this.bFilterSameCamp && !hostPlayer.Captain.handle.IsEnemyCamp(actorObj.handle))
			{
				this.bCheckFilter = false;
				return;
			}
			if (this.bFilterDiffCamp && hostPlayer.Captain.handle.IsEnemyCamp(actorObj.handle))
			{
				this.bCheckFilter = false;
				return;
			}
		}

		private void FilterActorType(PoolObjHandle<ActorRoot> actorObj)
		{
			if (this.bFilterHero && actorObj.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Hero)
			{
				this.bCheckFilter = false;
				return;
			}
			if (this.bFilterMonter)
			{
				MonsterWrapper monsterWrapper = actorObj.handle.AsMonster();
				if (monsterWrapper != null && monsterWrapper.cfgInfo != null && monsterWrapper.cfgInfo.bMonsterGrade != 3)
				{
					this.bCheckFilter = false;
					return;
				}
			}
			if (this.bFilterBoss)
			{
				MonsterWrapper monsterWrapper2 = actorObj.handle.AsMonster();
				if (monsterWrapper2 != null && monsterWrapper2.cfgInfo != null && monsterWrapper2.cfgInfo.bMonsterGrade == 3)
				{
					this.bCheckFilter = false;
					return;
				}
			}
			if (this.bFilterOrgan && actorObj.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ)
			{
				this.bCheckFilter = false;
				return;
			}
			if (this.bFilterEye && actorObj.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_EYE)
			{
				this.bCheckFilter = false;
				return;
			}
		}

		public override void Process(Action _action, Track _track)
		{
			PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.targetId);
			if (!actorHandle)
			{
				if (ActionManager.Instance.isPrintLog)
				{
				}
				return;
			}
			if (this.bImmediateRevive)
			{
				this.bCheckFilter = actorHandle.handle.ActorControl.IsEnableReviveContext();
			}
			else if (this.bOnlyHostHero)
			{
				this.HostPlayerHeroOnly(actorHandle);
			}
			else if (!this.bOnlySelf)
			{
				this.FilterActorType(actorHandle);
				this.FilterActorCamp(actorHandle);
			}
			else
			{
				SkillUseContext refParamObject = _action.refParams.GetRefParamObject<SkillUseContext>("SkillContext");
				if (refParamObject == null || !refParamObject.Originator || refParamObject.Originator.handle.ActorControl == null)
				{
					DebugHelper.Assert(false, "Failed find orignal actor of this skill. action:{0}", new object[]
					{
						_action.name
					});
					return;
				}
				this.bCheckFilter = (refParamObject.Originator == actorHandle);
			}
			base.Process(_action, _track);
		}

		public override bool Check(Action _action, Track _track)
		{
			return this.bCheckFilter;
		}
	}
}
