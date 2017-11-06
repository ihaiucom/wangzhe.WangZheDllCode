using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;
using System.Collections.Generic;

namespace AGE
{
	[EventCategory("MMGame/Newbie")]
	public class NewbiekillActor : TickEvent
	{
		public enum enActorType
		{
			All,
			Hero,
			Soldier
		}

		public NewbiekillActor.enActorType ActorType;

		public bool bPauseGame;

		public int configId;

		public int index;

		public bool hideActor;

		public override bool SupportEditMode()
		{
			return true;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			NewbiekillActor newbiekillActor = src as NewbiekillActor;
			this.ActorType = newbiekillActor.ActorType;
			this.bPauseGame = newbiekillActor.bPauseGame;
			this.configId = newbiekillActor.configId;
			this.index = newbiekillActor.index;
			this.hideActor = newbiekillActor.hideActor;
		}

		public override BaseEvent Clone()
		{
			NewbiekillActor newbiekillActor = ClassObjPool<NewbiekillActor>.Get();
			newbiekillActor.CopyData(this);
			return newbiekillActor;
		}

		public override void Process(Action _action, Track _track)
		{
			base.Process(_action, _track);
			PoolObjHandle<ActorRoot> actor;
			switch (this.ActorType)
			{
			case NewbiekillActor.enActorType.All:
				actor = this.GetActor(Singleton<GameObjMgr>.GetInstance().GameActors, this.configId, this.index);
				break;
			case NewbiekillActor.enActorType.Hero:
				actor = this.GetActor(Singleton<GameObjMgr>.GetInstance().HeroActors, this.configId, this.index);
				break;
			case NewbiekillActor.enActorType.Soldier:
				actor = this.GetActor(Singleton<GameObjMgr>.GetInstance().SoldierActors, this.configId, this.index);
				break;
			default:
				actor = default(PoolObjHandle<ActorRoot>);
				break;
			}
			this.SkillActor(actor);
		}

		private void SkillActor(PoolObjHandle<ActorRoot> actor)
		{
			if (this.hideActor)
			{
				actor.handle.ActorMesh.SetLayer("Hide", false);
			}
			actor.handle.ValueComponent.actorHp = 0;
		}

		private PoolObjHandle<ActorRoot> GetActor(List<PoolObjHandle<ActorRoot>> actorList, int configId, int index)
		{
			PoolObjHandle<ActorRoot> result = default(PoolObjHandle<ActorRoot>);
			if (actorList == null)
			{
				return result;
			}
			int num = 0;
			int count = actorList.get_Count();
			int i = 0;
			while (i < count)
			{
				PoolObjHandle<ActorRoot> poolObjHandle = actorList.get_Item(i);
				ActorRoot handle = poolObjHandle.handle;
				if (handle.TheActorMeta.ConfigId == configId && num == index)
				{
					result = poolObjHandle;
					break;
				}
				i++;
				num++;
			}
			return result;
		}
	}
}
