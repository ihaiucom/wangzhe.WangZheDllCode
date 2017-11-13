using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameSystem;
using System;

namespace AGE
{
	[EventCategory("MMGame/Skill")]
	public class ChangeHomeGuardEffect : TickCondition
	{
		[ObjectTemplate(new Type[]
		{

		})]
		public int targetId = -1;

		public bool bNormal;

		public bool bGuildMaxGrade;

		public bool bGuildHighestMatchScore;

		private bool bCheck = true;

		public override BaseEvent Clone()
		{
			ChangeHomeGuardEffect changeHomeGuardEffect = ClassObjPool<ChangeHomeGuardEffect>.Get();
			changeHomeGuardEffect.CopyData(this);
			return changeHomeGuardEffect;
		}

		protected override void CopyData(BaseEvent src)
		{
			base.CopyData(src);
			ChangeHomeGuardEffect changeHomeGuardEffect = src as ChangeHomeGuardEffect;
			this.targetId = changeHomeGuardEffect.targetId;
			this.bNormal = changeHomeGuardEffect.bNormal;
			this.bGuildMaxGrade = changeHomeGuardEffect.bGuildMaxGrade;
			this.bGuildHighestMatchScore = changeHomeGuardEffect.bGuildHighestMatchScore;
			this.bCheck = changeHomeGuardEffect.bCheck;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.targetId = -1;
			this.bNormal = false;
			this.bGuildMaxGrade = false;
			this.bGuildHighestMatchScore = false;
			this.bCheck = true;
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
			if (this.bGuildHighestMatchScore)
			{
				this.bCheck = (CGuildSystem.s_isGuildHighestMatchScore && ActorHelper.IsHostActor(ref actorHandle));
			}
			else if (this.bGuildMaxGrade)
			{
				this.bCheck = (CGuildSystem.s_isGuildMaxGrade && !CGuildSystem.s_isGuildHighestMatchScore && ActorHelper.IsHostActor(ref actorHandle));
			}
			else
			{
				this.bCheck = ((!CGuildSystem.s_isGuildHighestMatchScore && !CGuildSystem.s_isGuildMaxGrade) || !ActorHelper.IsHostActor(ref actorHandle));
			}
			base.Process(_action, _track);
		}

		public override bool Check(Action _action, Track _track)
		{
			return this.bCheck;
		}
	}
}
