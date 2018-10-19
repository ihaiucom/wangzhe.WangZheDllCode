using System;

namespace Assets.Scripts.GameLogic
{
	public class SkillAbort
	{
		public const int MAX_ABORTTYPE_COUNT = 14;

		private bool[] abortRuleArray = new bool[14];

		public void InitAbort(bool _bAbort)
		{
			for (int i = 0; i < 14; i++)
			{
				this.abortRuleArray[i] = _bAbort;
			}
			this.abortRuleArray[13] = false;
		}

		public void SetAbort(SkillAbortType _type)
		{
			if (_type < SkillAbortType.TYPE_SKILL_0 || _type > SkillAbortType.TYPE_DAMAGE)
			{
				return;
			}
			this.abortRuleArray[(int)_type] = true;
		}

		public bool Abort(SkillAbortType _type)
		{
			return _type >= SkillAbortType.TYPE_SKILL_0 && _type <= SkillAbortType.TYPE_DAMAGE && this.abortRuleArray[(int)_type];
		}

		public bool AbortWithAI()
		{
			for (int i = 0; i < 14; i++)
			{
				if (!this.abortRuleArray[i])
				{
					return false;
				}
			}
			return true;
		}
	}
}
