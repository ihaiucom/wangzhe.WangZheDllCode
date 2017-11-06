using Assets.Scripts.Common;
using ResData;
using System;
using System.Runtime.CompilerServices;

namespace Assets.Scripts.GameLogic
{
	public abstract class StarCondition : IStarCondition
	{
		protected string Description = string.Empty;

		public event OnStarConditionChangedDelegate OnStarConditionChanged
		{
			[MethodImpl(32)]
			add
			{
				this.OnStarConditionChanged = (OnStarConditionChangedDelegate)Delegate.Combine(this.OnStarConditionChanged, value);
			}
			[MethodImpl(32)]
			remove
			{
				this.OnStarConditionChanged = (OnStarConditionChangedDelegate)Delegate.Remove(this.OnStarConditionChanged, value);
			}
		}

		public ResDT_ConditionInfo ConditionInfo
		{
			get;
			protected set;
		}

		public virtual StarEvaluationStatus status
		{
			get
			{
				return StarEvaluationStatus.InProgressing;
			}
		}

		public virtual string description
		{
			get
			{
				return this.Description;
			}
		}

		public string rawDescription
		{
			get
			{
				return this.Description;
			}
		}

		public ResDT_ConditionInfo configInfo
		{
			get
			{
				return this.ConditionInfo;
			}
		}

		public virtual int operation
		{
			get
			{
				return this.ConditionInfo.ComparetorDetail[0];
			}
		}

		public virtual int defaultValue
		{
			get
			{
				return this.ConditionInfo.ValueDetail[0];
			}
		}

		public virtual int type
		{
			get
			{
				return (int)this.ConditionInfo.dwType;
			}
		}

		public virtual int extraType
		{
			get
			{
				return this.ConditionInfo.KeyDetail[0];
			}
		}

		public abstract int[] values
		{
			get;
		}

		public virtual int[] keys
		{
			get
			{
				DebugHelper.Assert(this.ConditionInfo != null);
				return new int[]
				{
					this.ConditionInfo.KeyDetail[0],
					this.ConditionInfo.KeyDetail[1],
					this.ConditionInfo.KeyDetail[2],
					this.ConditionInfo.KeyDetail[3]
				};
			}
		}

		public virtual void Initialize(ResDT_ConditionInfo InConditionInfo)
		{
			this.ConditionInfo = InConditionInfo;
		}

		public virtual void Start()
		{
		}

		public virtual void Dispose()
		{
		}

		protected virtual void TriggerChangedEvent()
		{
			if (this.OnStarConditionChanged != null)
			{
				this.OnStarConditionChanged(this);
			}
		}

		public static void UnPackUInt64ToUInt32(ulong InNumeric, out uint OutHigh, out uint OutLow)
		{
			OutHigh = (uint)(InNumeric >> 32);
			OutLow = (uint)(InNumeric | (ulong)-1);
		}

		public static ulong PackUInt32ToUInt64(uint InHigh, uint InLow)
		{
			ulong num = 0uL;
			num |= (ulong)InHigh << 32;
			return num | (ulong)InLow;
		}

		public virtual bool GetActorRef(out PoolObjHandle<ActorRoot> OutSource, out PoolObjHandle<ActorRoot> OutAttacker)
		{
			OutSource = new PoolObjHandle<ActorRoot>(null);
			OutAttacker = new PoolObjHandle<ActorRoot>(null);
			return false;
		}

		public virtual void OnActorDeath(ref GameDeadEventParam prm)
		{
		}

		public virtual void OnCampScoreUpdated(ref SCampScoreUpdateParam prm)
		{
		}
	}
}
