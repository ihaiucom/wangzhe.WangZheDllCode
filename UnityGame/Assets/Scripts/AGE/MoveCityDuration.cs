using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;

namespace AGE
{
	[EventCategory("MMGame/Skill")]
	public class MoveCityDuration : DurationEvent
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int targetId;

		private PoolObjHandle<ActorRoot> actorObj;

		public override bool SupportEditMode()
		{
			return true;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.targetId = 0;
			this.actorObj.Release();
		}

		public override BaseEvent Clone()
		{
			MoveCityDuration moveCityDuration = ClassObjPool<MoveCityDuration>.Get();
			moveCityDuration.CopyData(this);
			return moveCityDuration;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			MoveCityDuration moveCityDuration = src as MoveCityDuration;
			this.targetId = moveCityDuration.targetId;
		}

		private void MoveCity()
		{
			if (!this.actorObj)
			{
				return;
			}
			VInt3 zero = VInt3.zero;
			VInt3 zero2 = VInt3.zero;
			if (Singleton<BattleLogic>.GetInstance().mapLogic.GetRevivePosDir(ref this.actorObj.handle.TheActorMeta, true, out zero2, out zero))
			{
				VInt groundY;
				if (PathfindingUtility.GetGroundY(zero2, out groundY))
				{
					this.actorObj.handle.groundY = groundY;
					zero2.y = groundY.i;
				}
				this.actorObj.handle.forward = zero;
				this.actorObj.handle.location = zero2;
				this.actorObj.handle.ActorControl.AddNoAbilityFlag(ObjAbilityType.ObjAbility_MoveProtect);
				DefaultGameEventParam defaultGameEventParam = new DefaultGameEventParam(this.actorObj, this.actorObj);
				Singleton<GameEventSys>.instance.SendEvent<DefaultGameEventParam>(GameEventDef.Event_ActorMoveCity, ref defaultGameEventParam);
			}
		}

		public override void Enter(Action _action, Track _track)
		{
			base.Enter(_action, _track);
			this.actorObj = _action.GetActorHandle(this.targetId);
			this.MoveCity();
		}

		public override void Leave(Action _action, Track _track)
		{
			if (this.actorObj)
			{
				this.actorObj.handle.ActorControl.RmvNoAbilityFlag(ObjAbilityType.ObjAbility_MoveProtect);
			}
			base.Leave(_action, _track);
		}

		public override void Process(Action _action, Track _track, int _localTime)
		{
			base.Process(_action, _track, _localTime);
		}
	}
}
