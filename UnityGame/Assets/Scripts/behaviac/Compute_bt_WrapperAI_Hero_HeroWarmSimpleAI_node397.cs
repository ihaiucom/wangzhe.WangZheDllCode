using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Compute_bt_WrapperAI_Hero_HeroWarmSimpleAI_node397 : Compute
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			int pvPLevelMaxHeroNum = ((ObjAgent)pAgent).GetPvPLevelMaxHeroNum();
			int num = 2;
			int value = pvPLevelMaxHeroNum * num;
			pAgent.SetVariable<int>("p_waitBornFrame", value, 2351850927u);
			return result;
		}
	}
}
