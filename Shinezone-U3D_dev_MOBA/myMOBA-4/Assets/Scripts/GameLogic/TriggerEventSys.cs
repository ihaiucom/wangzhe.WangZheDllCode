using System;
using System.Runtime.CompilerServices;

namespace Assets.Scripts.GameLogic
{
	public class TriggerEventSys : Singleton<TriggerEventSys>
	{
		public event TriggerEventDelegate OnActorInside;

		public event TriggerEventDelegate OnActorEnter;

		public event TriggerEventDelegate OnActorLeave;

		public void SendEvent(TriggerEventDef eventDef, AreaEventTrigger sourceTrigger, object param)
		{
			switch (eventDef)
			{
			case TriggerEventDef.ActorInside:
				if (this.OnActorInside != null)
				{
					this.OnActorInside(sourceTrigger, param);
				}
				break;
			case TriggerEventDef.ActorEnter:
				if (this.OnActorEnter != null)
				{
					this.OnActorEnter(sourceTrigger, param);
				}
				break;
			case TriggerEventDef.ActorLeave:
				if (this.OnActorLeave != null)
				{
					this.OnActorLeave(sourceTrigger, param);
				}
				break;
			}
		}
	}
}
