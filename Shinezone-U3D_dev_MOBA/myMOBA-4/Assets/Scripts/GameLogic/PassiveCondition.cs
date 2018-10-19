using Assets.Scripts.Common;
using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	public class PassiveCondition
	{
		protected PassiveEvent rootEvent;

		protected PoolObjHandle<ActorRoot> sourceActor;

		protected int[] localParams = new int[8];

		private void SetConditionParam(ref ResDT_SkillPassiveCondition _config)
		{
			for (int i = 0; i < 8; i++)
			{
				this.localParams[i] = _config.astConditionParam[i].iParam;
			}
		}

		public virtual void Init(PoolObjHandle<ActorRoot> _source, PassiveEvent _event, ref ResDT_SkillPassiveCondition _config)
		{
			this.rootEvent = _event;
			this.sourceActor = _source;
			this.SetConditionParam(ref _config);
		}

		protected bool CheckTargetSubType(PoolObjHandle<ActorRoot> _actor, int typeMask, int typeSubMask)
		{
			if (typeMask == 0)
			{
				return true;
			}
			if (_actor)
			{
				int actorType = (int)_actor.handle.TheActorMeta.ActorType;
				if ((typeMask & 1 << actorType) > 0)
				{
					if (actorType != 1)
					{
						return true;
					}
					if (typeSubMask == 0)
					{
						return true;
					}
					int actorSubType = (int)_actor.handle.ActorControl.GetActorSubType();
					if (actorSubType == typeSubMask)
					{
						return true;
					}
				}
			}
			return false;
		}

		public virtual void Reset()
		{
		}

		public virtual void UnInit()
		{
		}

		public virtual bool Fit()
		{
			return false;
		}
	}
}
