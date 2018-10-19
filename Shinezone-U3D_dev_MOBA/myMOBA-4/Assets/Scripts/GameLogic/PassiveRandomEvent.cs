using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	[PassiveEvent(PassiveEventType.RandomPassiveEvent)]
	public class PassiveRandomEvent : PassiveEvent
	{
		private int randomRate;

		public override void Init(PoolObjHandle<ActorRoot> _actor, PassiveSkill _skill)
		{
			base.Init(_actor, _skill);
			this.randomRate = this.localParams[0];
		}

		public override void UpdateLogic(int _delta)
		{
			base.UpdateLogic(_delta);
			if (base.Fit())
			{
				int num = (int)FrameRandom.Random(10000u);
				if (num < this.randomRate)
				{
					base.Trigger();
				}
				base.Reset();
			}
			else
			{
				base.Reset();
			}
		}
	}
}
