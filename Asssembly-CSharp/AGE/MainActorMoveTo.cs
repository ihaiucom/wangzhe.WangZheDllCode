using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameKernal;
using System;
using UnityEngine;

namespace AGE
{
	[EventCategory("MMGame/Drama")]
	public class MainActorMoveTo : TickEvent
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int srcId;

		[ObjectTemplate(new Type[]
		{

		})]
		public int atkerId = 1;

		[ObjectTemplate(new Type[]
		{

		})]
		public int destId = 2;

		public Vector3 TargetPos = new Vector3(0f, 0f, 0f);

		public override bool SupportEditMode()
		{
			return true;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			MainActorMoveTo mainActorMoveTo = src as MainActorMoveTo;
			this.srcId = mainActorMoveTo.srcId;
			this.atkerId = mainActorMoveTo.atkerId;
			this.destId = mainActorMoveTo.destId;
			this.TargetPos = mainActorMoveTo.TargetPos;
		}

		public override BaseEvent Clone()
		{
			MainActorMoveTo mainActorMoveTo = ClassObjPool<MainActorMoveTo>.Get();
			mainActorMoveTo.CopyData(this);
			return mainActorMoveTo;
		}

		public override void Process(Action _action, Track _track)
		{
			Vector3 ob = this.TargetPos;
			GameObject gameObject = _action.GetGameObject(this.destId);
			if (gameObject != null)
			{
				ob = gameObject.transform.position;
			}
			Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
			PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.srcId);
			if (!actorHandle || (hostPlayer.Captain && actorHandle == hostPlayer.Captain))
			{
				ObjWrapper actorControl = hostPlayer.Captain.handle.ActorControl;
				if (actorControl != null)
				{
					FrameCommandFactory.CreateFrameCommand<StopMoveCommand>().Send();
					FrameCommand<MoveToPosCommand> frameCommand = FrameCommandFactory.CreateFrameCommand<MoveToPosCommand>();
					frameCommand.cmdData.destPosition = (VInt3)ob;
					frameCommand.Send();
				}
			}
			else if (actorHandle)
			{
				actorHandle.handle.ActorControl.RealMovePosition((VInt3)ob, 0u);
			}
		}
	}
}
