using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;
using System.Collections.Generic;

namespace AGE
{
	[EventCategory("MMGame/Newbie")]
	public class NewbieSelectActor : TickEvent
	{
		public enum enActorType
		{
			All,
			Hero,
			Soldier
		}

		public NewbieSelectActor.enActorType ActorType;

		public bool bPauseGame;

		public int configId;

		public int index;

		public override bool SupportEditMode()
		{
			return true;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			NewbieSelectActor newbieSelectActor = src as NewbieSelectActor;
			this.ActorType = newbieSelectActor.ActorType;
			this.bPauseGame = newbieSelectActor.bPauseGame;
			this.configId = newbieSelectActor.configId;
			this.index = newbieSelectActor.index;
		}

		public override BaseEvent Clone()
		{
			NewbieSelectActor newbieSelectActor = ClassObjPool<NewbieSelectActor>.Get();
			newbieSelectActor.CopyData(this);
			return newbieSelectActor;
		}

		public override void Process(Action _action, Track _track)
		{
			base.Process(_action, _track);
			PoolObjHandle<ActorRoot> actor;
			switch (this.ActorType)
			{
			case NewbieSelectActor.enActorType.All:
				actor = this.GetActor(Singleton<GameObjMgr>.GetInstance().GameActors, this.configId, this.index);
				break;
			case NewbieSelectActor.enActorType.Hero:
				actor = this.GetActor(Singleton<GameObjMgr>.GetInstance().HeroActors, this.configId, this.index);
				break;
			case NewbieSelectActor.enActorType.Soldier:
				actor = this.GetActor(Singleton<GameObjMgr>.GetInstance().SoldierActors, this.configId, this.index);
				break;
			default:
				actor = default(PoolObjHandle<ActorRoot>);
				break;
			}
			Singleton<BattleSkillHudControl>.GetInstance().AddHighlightForActor(actor, this.bPauseGame);
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
