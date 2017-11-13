using Assets.Scripts.Common;
using System;

namespace Assets.Scripts.GameLogic
{
	public delegate bool ActorFilterDelegate(ref PoolObjHandle<ActorRoot> actor);
}
