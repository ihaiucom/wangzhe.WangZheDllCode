using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.Sound;
using ResData;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Assets.Scripts.GameSystem
{
	public abstract class VoiceInteraction
	{
		protected ResVoiceInteraction InteractionCfg;

		protected float LastTriggerTime;

		protected float LastGroupTriggerTime;

		protected int TriggerCount;

		protected bool bBeginTrigger;

		protected static Random Rand = new Random();

		protected PoolObjHandle<ActorRoot> SoundSourceActor;

		public event OnInteractionTriggerDelegate OnBeginTriggered
		{
			[MethodImpl(32)]
			add
			{
				this.OnBeginTriggered = (OnInteractionTriggerDelegate)Delegate.Combine(this.OnBeginTriggered, value);
			}
			[MethodImpl(32)]
			remove
			{
				this.OnBeginTriggered = (OnInteractionTriggerDelegate)Delegate.Remove(this.OnBeginTriggered, value);
			}
		}

		public bool isSpecialTrigger
		{
			get
			{
				return this.InteractionCfg != null && this.InteractionCfg.bSpecialTrigger != 0;
			}
		}

		public bool isSpecialReceiver
		{
			get
			{
				return this.InteractionCfg != null && this.InteractionCfg.bSpecialReceive != 0;
			}
		}

		public int groupID
		{
			get
			{
				return (int)((this.InteractionCfg != null) ? this.InteractionCfg.dwGroupID : 0u);
			}
		}

		public int priority
		{
			get
			{
				return (this.InteractionCfg != null) ? ((int)this.InteractionCfg.bPriorityInGroup) : -1;
			}
		}

		public bool isBeginTrigger
		{
			get
			{
				return this.bBeginTrigger;
			}
			set
			{
				this.bBeginTrigger = value;
			}
		}

		public virtual void Init(ResVoiceInteraction InInteractionCfg)
		{
			this.InteractionCfg = InInteractionCfg;
		}

		public virtual void Unit()
		{
			this.InteractionCfg = null;
			this.OnBeginTriggered = null;
			this.SoundSourceActor = default(PoolObjHandle<ActorRoot>);
			this.bBeginTrigger = false;
			this.TriggerCount = 0;
		}

		protected virtual bool ForwardCheck()
		{
			if (this.InteractionCfg == null)
			{
				return false;
			}
			if (this.InteractionCfg.bTriggerCount != 0 && this.TriggerCount >= (int)this.InteractionCfg.bTriggerCount)
			{
				return false;
			}
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			if (realtimeSinceStartup - this.LastTriggerTime < this.InteractionCfg.dwTriggerInterval || realtimeSinceStartup - this.LastGroupTriggerTime < this.InteractionCfg.dwGroupTriggerInterval)
			{
				return false;
			}
			int num = VoiceInteraction.Rand.Next(0, 10000);
			return (long)num <= (long)((ulong)this.InteractionCfg.dwTriggerProbility);
		}

		protected virtual bool CheckTriggerDistance(ref PoolObjHandle<ActorRoot> InSource, ref PoolObjHandle<ActorRoot> InRelevance)
		{
			Vector3 a = (Vector3)InSource.handle.location;
			Vector3 b = (Vector3)InRelevance.handle.location;
			float num = Vector3.Distance(a, b) * 1000f;
			return num <= this.InteractionCfg.dwTriggerRadius;
		}

		protected virtual bool CheckReceiveDistance(ref PoolObjHandle<ActorRoot> InSource, ref PoolObjHandle<ActorRoot> InRelevance)
		{
			Vector3 a = (Vector3)InSource.handle.location;
			Vector3 b = (Vector3)InRelevance.handle.location;
			float num = Vector3.Distance(a, b) * 1000f;
			return num <= this.InteractionCfg.dwReceiveRadius;
		}

		protected virtual bool TryTrigger(ref PoolObjHandle<ActorRoot> InSource, ref PoolObjHandle<ActorRoot> InRelevance, ref PoolObjHandle<ActorRoot> InSoundSource)
		{
			Player hostPlayer = Singleton<GamePlayerCenter>.instance.GetHostPlayer();
			if (hostPlayer != null && hostPlayer.Captain && InSource)
			{
				bool flag = false;
				if (this.InteractionCfg.bReceiveType == 1 && InSource.handle.IsEnemyCamp(hostPlayer.Captain.handle))
				{
					flag = true;
				}
				else if (this.InteractionCfg.bReceiveType == 100)
				{
					flag = true;
				}
				else if (this.InteractionCfg.bReceiveType == 0 && InSource.handle.IsSelfCamp(hostPlayer.Captain.handle))
				{
					flag = true;
				}
				if (!flag)
				{
					return false;
				}
				if (!this.isSpecialReceiver)
				{
					return this.BeginTrigger(ref InSoundSource);
				}
				int configId = hostPlayer.Captain.handle.TheActorMeta.ConfigId;
				for (int i = 0; i < this.InteractionCfg.SpecialReceiveConditions.Length; i++)
				{
					if (this.InteractionCfg.SpecialReceiveConditions[i] == 0u)
					{
						break;
					}
					if ((ulong)this.InteractionCfg.SpecialReceiveConditions[i] == (ulong)((long)configId))
					{
						return this.BeginTrigger(ref InSoundSource);
					}
				}
			}
			return false;
		}

		protected virtual bool BeginTrigger(ref PoolObjHandle<ActorRoot> InSoundSource)
		{
			this.bBeginTrigger = true;
			this.SoundSourceActor = InSoundSource;
			if (this.OnBeginTriggered != null)
			{
				this.OnBeginTriggered(this);
			}
			return true;
		}

		public void DoTrigger()
		{
			this.LastTriggerTime = Time.realtimeSinceStartup;
			this.TriggerCount++;
			if (this.InteractionCfg != null)
			{
				Singleton<CSoundManager>.instance.PlayBattleSound(this.InteractionCfg.szVoiceEvent, this.SoundSourceActor, null);
			}
		}

		public void FinishTrigger(float InGroupTriggerTime)
		{
			this.LastGroupTriggerTime = InGroupTriggerTime;
			this.bBeginTrigger = false;
			this.SoundSourceActor = default(PoolObjHandle<ActorRoot>);
		}

		public bool IsSpecialTriggerID(int InCheckID)
		{
			if (this.InteractionCfg == null)
			{
				return false;
			}
			for (int i = 0; i < this.InteractionCfg.SpecialTriggerConditions.Length; i++)
			{
				if (this.InteractionCfg.SpecialTriggerConditions[i] == 0u)
				{
					return false;
				}
				if ((ulong)this.InteractionCfg.SpecialTriggerConditions[i] == (ulong)((long)InCheckID))
				{
					return true;
				}
			}
			return false;
		}

		protected bool ValidateTriggerActor(ref PoolObjHandle<ActorRoot> InTestActor)
		{
			return InTestActor && (!this.isSpecialTrigger || this.IsSpecialTriggerID(InTestActor.handle.TheActorMeta.ConfigId));
		}
	}
}
