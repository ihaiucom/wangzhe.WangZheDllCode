using AGE;
using Assets.Scripts.Common;
using System;

namespace Assets.Scripts.GameLogic
{
	public class TriggerActionNewbieForm : TriggerActionBase
	{
		public TriggerActionNewbieForm(TriggerActionWrapper inWrapper, int inTriggerId) : base(inWrapper, inTriggerId)
		{
		}

		public override RefParamOperator TriggerEnter(PoolObjHandle<ActorRoot> src, PoolObjHandle<ActorRoot> atker, ITrigger inTrigger)
		{
			if (this.EnterUniqueId > 0 && this.EnterUniqueId < 34)
			{
				Singleton<CBattleGuideManager>.GetInstance().OpenFormShared((CBattleGuideManager.EBattleGuideFormType)this.EnterUniqueId, 0, true);
			}
			return null;
		}
	}
}
