using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;

namespace AGE
{
	[EventCategory("MMGame/Drama")]
	public class SendGameEventTick : TickEvent
	{
		public GameEventDef eventType;

		[ObjectTemplate(new Type[]
		{

		})]
		public int eventSrcId;

		[ObjectTemplate(new Type[]
		{

		})]
		public int eventAtkerId = -1;

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			SendGameEventTick sendGameEventTick = src as SendGameEventTick;
			this.eventType = sendGameEventTick.eventType;
			this.eventSrcId = sendGameEventTick.eventSrcId;
			this.eventAtkerId = sendGameEventTick.eventAtkerId;
		}

		public override BaseEvent Clone()
		{
			SendGameEventTick sendGameEventTick = ClassObjPool<SendGameEventTick>.Get();
			sendGameEventTick.CopyData(this);
			return sendGameEventTick;
		}

		public override void Process(Action _action, Track _track)
		{
			PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.eventSrcId);
			PoolObjHandle<ActorRoot> poolObjHandle = _action.GetActorHandle(this.eventAtkerId);
			if (!poolObjHandle && actorHandle)
			{
				poolObjHandle = actorHandle.handle.ActorControl.myLastAtker;
			}
			DefaultGameEventParam defaultGameEventParam = new DefaultGameEventParam(actorHandle, poolObjHandle);
			if (this.eventType == GameEventDef.Event_GameEnd)
			{
				Singleton<GameEventSys>.instance.PostEvent<DefaultGameEventParam>(this.eventType, ref defaultGameEventParam);
			}
			else
			{
				Singleton<GameEventSys>.instance.SendEvent<DefaultGameEventParam>(this.eventType, ref defaultGameEventParam);
			}
		}
	}
}
