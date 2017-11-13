using Assets.Scripts.Common;
using System;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public abstract class ActorComponent : BaseComponent, IActorComponent, IPooledMonoBehaviour
	{
		[HideInInspector]
		public ActorRoot actor;

		public PoolObjHandle<ActorRoot> actorPtr;

		public VInt3 actorLocation
		{
			get
			{
				return this.actor.location;
			}
		}

		public virtual void OnCreate()
		{
		}

		public virtual void OnGet()
		{
			this.actor = null;
			this.actorPtr.Release();
		}

		public virtual void OnRecycle()
		{
			this.actor = null;
			this.actorPtr.Release();
		}

		protected override void Start()
		{
		}

		public virtual void Born(ActorRoot owner)
		{
			this.actor = owner;
			this.actorPtr = new PoolObjHandle<ActorRoot>(this.actor);
		}

		public virtual void Init()
		{
		}

		public virtual void Uninit()
		{
		}

		public virtual void Prepare()
		{
		}

		public virtual void Fight()
		{
		}

		public virtual void FightOver()
		{
		}

		public virtual void UpdateLogic(int delta)
		{
		}

		public PoolObjHandle<ActorRoot> GetActor()
		{
			return this.actorPtr;
		}
	}
}
