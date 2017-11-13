using System;

namespace behaviac
{
	internal class Parallel_bt_WrapperAI_Organ_CrystalWithAttack_node14 : Parallel
	{
		public Parallel_bt_WrapperAI_Organ_CrystalWithAttack_node14()
		{
			this.m_failPolicy = FAILURE_POLICY.FAIL_ON_ALL;
			this.m_succeedPolicy = SUCCESS_POLICY.SUCCEED_ON_ALL;
			this.m_exitPolicy = EXIT_POLICY.EXIT_NONE;
			this.m_childFinishPolicy = CHILDFINISH_POLICY.CHILDFINISH_LOOP;
		}
	}
}
