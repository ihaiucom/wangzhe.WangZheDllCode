using Assets.Scripts.Common;
using System;

namespace Assets.Scripts.GameLogic
{
	public interface ITreasureChestStrategy
	{
		event OnDropTreasureChestDelegate OnDropTreasure;

		int maxCount
		{
			get;
		}

		int droppedCount
		{
			get;
		}

		bool isSupportDrop
		{
			get;
		}

		void Initialize(int InMaxCount);

		void Stop();

		void NotifyDropEvent(PoolObjHandle<ActorRoot> actor);

		void NotifyMatchEnd();
	}
}
