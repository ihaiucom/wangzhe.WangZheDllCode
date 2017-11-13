using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	public interface IStarEvaluation
	{
		string description
		{
			get;
		}

		string rawDescription
		{
			get;
		}

		ResEvaluateStarInfo configInfo
		{
			get;
		}

		int index
		{
			get;
		}

		StarEvaluationStatus status
		{
			get;
		}

		bool isSuccess
		{
			get;
		}

		bool isFailure
		{
			get;
		}

		bool isInProgressing
		{
			get;
		}

		void Initialize(ResEvaluateStarInfo InStarInfo);

		void Start();

		void Dispose();

		ListView<IStarCondition>.Enumerator GetEnumerator();

		IStarCondition GetConditionAt(int Index);

		void OnActorDeath(ref GameDeadEventParam prm);

		void OnCampScoreUpdated(ref SCampScoreUpdateParam prm);
	}
}
