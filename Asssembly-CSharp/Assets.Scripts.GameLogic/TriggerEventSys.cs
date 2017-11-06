using System;
using System.Runtime.CompilerServices;

namespace Assets.Scripts.GameLogic
{
	public class TriggerEventSys : Singleton<TriggerEventSys>
	{
		public event TriggerEventDelegate OnActorInside
		{
			[MethodImpl(32)]
			add
			{
				this.OnActorInside = (TriggerEventDelegate)Delegate.Combine(this.OnActorInside, value);
			}
			[MethodImpl(32)]
			remove
			{
				this.OnActorInside = (TriggerEventDelegate)Delegate.Remove(this.OnActorInside, value);
			}
		}

		public event TriggerEventDelegate OnActorEnter
		{
			[MethodImpl(32)]
			add
			{
				this.OnActorEnter = (TriggerEventDelegate)Delegate.Combine(this.OnActorEnter, value);
			}
			[MethodImpl(32)]
			remove
			{
				this.OnActorEnter = (TriggerEventDelegate)Delegate.Remove(this.OnActorEnter, value);
			}
		}

		public event TriggerEventDelegate OnActorLeave
		{
			[MethodImpl(32)]
			add
			{
				this.OnActorLeave = (TriggerEventDelegate)Delegate.Combine(this.OnActorLeave, value);
			}
			[MethodImpl(32)]
			remove
			{
				this.OnActorLeave = (TriggerEventDelegate)Delegate.Remove(this.OnActorLeave, value);
			}
		}

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
