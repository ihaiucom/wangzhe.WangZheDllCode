using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;

namespace AGE
{
	[EventCategory("MMGame/Drama")]
	public class SetInvincibilityTick : TickEvent
	{
		public bool bInvincible;

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
		public int targetId = 2;

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			SetInvincibilityTick setInvincibilityTick = src as SetInvincibilityTick;
			this.bInvincible = setInvincibilityTick.bInvincible;
			this.srcId = setInvincibilityTick.srcId;
			this.atkerId = setInvincibilityTick.atkerId;
			this.targetId = setInvincibilityTick.targetId;
		}

		public override BaseEvent Clone()
		{
			SetInvincibilityTick setInvincibilityTick = ClassObjPool<SetInvincibilityTick>.Get();
			setInvincibilityTick.CopyData(this);
			return setInvincibilityTick;
		}

		public override void Process(Action _action, Track _track)
		{
			PoolObjHandle<ActorRoot> actorHandle = _action.GetActorHandle(this.targetId);
			if (actorHandle)
			{
				actorHandle.handle.ObjLinker.Invincible = this.bInvincible;
			}
		}
	}
}
