using System;
using UnityEngine;

namespace Apollo
{
	public abstract class ApolloObject
	{
		private static ulong s_objectId = 1uL;

		private float lastTime;

		public ulong ObjectId
		{
			get;
			private set;
		}

		public bool AcceptMonoBehaviour
		{
			get;
			private set;
		}

		public bool Reflectible
		{
			get;
			private set;
		}

		protected float UpdateTimeLeft
		{
			get;
			set;
		}

		internal bool Removable
		{
			get;
			set;
		}

		protected ApolloObject()
		{
			this.Reflectible = true;
			this.AcceptMonoBehaviour = false;
			this.Removable = false;
			this.UpdateTimeLeft = -1f;
			this.init();
		}

		protected ApolloObject(bool reflectible, bool acceptMonoBehaviour)
		{
			this.UpdateTimeLeft = -1f;
			this.Reflectible = reflectible;
			this.AcceptMonoBehaviour = acceptMonoBehaviour;
			this.init();
		}

		private void init()
		{
			if (this.Reflectible)
			{
				ulong expr_11 = ApolloObject.s_objectId;
				ApolloObject.s_objectId = expr_11 + 1uL;
				this.ObjectId = expr_11;
				if (ApolloObject.s_objectId == 0uL)
				{
					ApolloObject.s_objectId = 1uL;
				}
				ApolloObjectManager.Instance.AddObject(this);
			}
			if (this.AcceptMonoBehaviour)
			{
				ApolloObjectManager.Instance.AddAcceptUpdatedObject(this);
			}
		}

		~ApolloObject()
		{
			this.Destroy();
		}

		public void Destroy()
		{
			this.Removable = true;
		}

		public virtual void Update()
		{
			float time = Time.time;
			float num = time - this.lastTime;
			this.lastTime = time;
			this.OnUpdate(num);
			if (this.UpdateTimeLeft > 0f)
			{
				this.UpdateTimeLeft -= num;
				if (this.UpdateTimeLeft <= 0f)
				{
					this.OnTimeOut();
				}
			}
		}

		protected virtual void OnUpdate(float deltaTime)
		{
		}

		protected virtual void OnTimeOut()
		{
		}

		public virtual void OnApplicationQuit()
		{
		}

		public virtual void OnApplicationPause(bool pauseStatus)
		{
		}

		public virtual void OnDisable()
		{
		}
	}
}
