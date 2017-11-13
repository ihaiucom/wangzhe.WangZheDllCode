using Assets.Scripts.Common;
using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	public interface IStarCondition
	{
		string description
		{
			get;
		}

		string rawDescription
		{
			get;
		}

		ResDT_ConditionInfo configInfo
		{
			get;
		}

		StarEvaluationStatus status
		{
			get;
		}

		int type
		{
			get;
		}

		int extraType
		{
			get;
		}

		int[] keys
		{
			get;
		}

		int[] values
		{
			get;
		}

		void Initialize(ResDT_ConditionInfo InConditionInfo);

		void Start();

		void Dispose();

		bool GetActorRef(out PoolObjHandle<ActorRoot> OutSource, out PoolObjHandle<ActorRoot> OutAttacker);

		void OnActorDeath(ref GameDeadEventParam prm);

		void OnCampScoreUpdated(ref SCampScoreUpdateParam prm);
	}
}
