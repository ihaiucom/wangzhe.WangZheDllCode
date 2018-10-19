using CSProtocol;
using System;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	public abstract class FuncRegion : MonoBehaviour, IUpdateLogic
	{
		public COM_PLAYERCAMP CampType;

		[HideInInspector]
		[NonSerialized]
		public bool isStartup;

		public virtual void UpdateLogic(int delta)
		{
		}

		public virtual void Startup()
		{
			this.isStartup = true;
		}

		public virtual void Stop()
		{
			this.isStartup = false;
		}
	}
}
