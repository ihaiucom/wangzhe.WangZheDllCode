using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;

namespace AGE
{
	[EventCategory("MMGame/Skill")]
	public class GetLinkedActorTick : TickEvent
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int tempId = -1;

		public int srcId = -1;

		public bool bIsHostHeroGetCallActor;

		public bool bIsHostHeroGetMonster;

		public bool bIsCallActorGetHostHero;

		public bool bIsMonsterGetHostHero;

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			GetLinkedActorTick getLinkedActorTick = src as GetLinkedActorTick;
			if (getLinkedActorTick != null)
			{
				this.tempId = getLinkedActorTick.tempId;
				this.srcId = getLinkedActorTick.srcId;
				this.bIsHostHeroGetCallActor = getLinkedActorTick.bIsHostHeroGetCallActor;
				this.bIsHostHeroGetMonster = getLinkedActorTick.bIsHostHeroGetMonster;
				this.bIsCallActorGetHostHero = getLinkedActorTick.bIsCallActorGetHostHero;
				this.bIsMonsterGetHostHero = getLinkedActorTick.bIsMonsterGetHostHero;
			}
		}

		public override BaseEvent Clone()
		{
			GetLinkedActorTick getLinkedActorTick = ClassObjPool<GetLinkedActorTick>.Get();
			getLinkedActorTick.CopyData(this);
			return getLinkedActorTick;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.tempId = -1;
			this.srcId = -1;
			this.bIsHostHeroGetCallActor = false;
			this.bIsHostHeroGetMonster = false;
			this.bIsCallActorGetHostHero = false;
			this.bIsMonsterGetHostHero = false;
		}

		private void HostHeroGetCallActor(ref PoolObjHandle<ActorRoot> srcActor, ref PoolObjHandle<ActorRoot> tempActor)
		{
			if (!srcActor || srcActor.handle.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Hero)
			{
				return;
			}
			HeroWrapper heroWrapper = srcActor.handle.ActorControl as HeroWrapper;
			if (heroWrapper != null)
			{
				tempActor = heroWrapper.GetCallActor();
			}
		}

		private void HostHeroGetMonster(ref PoolObjHandle<ActorRoot> srcActor, ref PoolObjHandle<ActorRoot> tempActor)
		{
			if (!srcActor || srcActor.handle.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Hero)
			{
				return;
			}
			HeroWrapper heroWrapper = srcActor.handle.ActorControl as HeroWrapper;
			if (heroWrapper != null && heroWrapper.hasCalledMonster)
			{
				tempActor = heroWrapper.CallMonster;
			}
		}

		private void CallActorGetHostHero(ref PoolObjHandle<ActorRoot> srcActor, ref PoolObjHandle<ActorRoot> tempActor)
		{
			if (!srcActor || srcActor.handle.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Call)
			{
				return;
			}
			CallActorWrapper callActorWrapper = srcActor.handle.ActorControl as CallActorWrapper;
			if (callActorWrapper != null)
			{
				tempActor = callActorWrapper.GetHostActor();
			}
		}

		private void MonsterGetHostHero(ref PoolObjHandle<ActorRoot> srcActor, ref PoolObjHandle<ActorRoot> tempActor)
		{
			if (!srcActor || srcActor.handle.TheActorMeta.ActorType != ActorTypeDef.Actor_Type_Monster)
			{
				return;
			}
			MonsterWrapper monsterWrapper = srcActor.handle.ActorControl as MonsterWrapper;
			if (monsterWrapper != null && monsterWrapper.isCalledMonster)
			{
				tempActor = monsterWrapper.GetOrignalActor();
			}
		}

		public override void Process(Action _action, Track _track)
		{
			PoolObjHandle<ActorRoot> ptr = new PoolObjHandle<ActorRoot>(null);
			PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.srcId);
			if (!actorHandle)
			{
				if (ActionManager.Instance.isPrintLog)
				{
				}
				return;
			}
			if (this.bIsHostHeroGetCallActor)
			{
				this.HostHeroGetCallActor(ref actorHandle, ref ptr);
			}
			else if (this.bIsHostHeroGetMonster)
			{
				this.HostHeroGetMonster(ref actorHandle, ref ptr);
			}
			else if (this.bIsCallActorGetHostHero)
			{
				this.CallActorGetHostHero(ref actorHandle, ref ptr);
			}
			else if (this.bIsMonsterGetHostHero)
			{
				this.MonsterGetHostHero(ref actorHandle, ref ptr);
			}
			if (ptr && this.tempId >= 0)
			{
				_action.ExpandGameObject(this.tempId);
				_action.SetGameObject(this.tempId, ptr.handle.gameObject);
			}
		}
	}
}
