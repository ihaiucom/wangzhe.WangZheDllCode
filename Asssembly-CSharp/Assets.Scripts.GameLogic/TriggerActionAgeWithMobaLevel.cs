using AGE;
using System;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public class TriggerActionAgeWithMobaLevel : TriggerActionAge
	{
		public TriggerActionAgeWithMobaLevel(TriggerActionWrapper inWrapper, int inTriggerId) : base(inWrapper, inTriggerId)
		{
		}

		protected override ListView<Action> PlayAgeActionShared(AreaEventTrigger.EActTiming inTiming, AreaEventTrigger.STimingAction[] inTimingActs, ActionStopDelegate inCallback, ListView<Action> outDuraActs, GameObject inSrc, GameObject inAtker)
		{
			ListView<Action> listView = new ListView<Action>();
			int playerMobaLevel = Singleton<CBattleGuideManager>.GetInstance().GetPlayerGuideInfo().m_PlayerMobaLevel;
			if (inTimingActs.Length > playerMobaLevel)
			{
				AreaEventTrigger.STimingAction sTimingAction = inTimingActs[playerMobaLevel];
				ActionStopDelegate actionStopDelegate = null;
				if (inTiming == AreaEventTrigger.EActTiming.EnterDura)
				{
					actionStopDelegate = inCallback;
				}
				Action action = TriggerActionAge.PlayAgeActionShared(sTimingAction.ActionName, sTimingAction.HelperName, inSrc, inAtker, sTimingAction.HelperIndex, inCallback);
				if (action != null)
				{
					listView.Add(action);
					if (actionStopDelegate != null)
					{
						outDuraActs.Add(action);
					}
				}
			}
			return listView;
		}
	}
}
