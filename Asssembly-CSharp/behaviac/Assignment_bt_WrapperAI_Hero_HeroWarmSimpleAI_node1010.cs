using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Hero_HeroWarmSimpleAI_node1010 : Assignment
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			uint heroWhoAttackSelf = ((ObjAgent)pAgent).GetHeroWhoAttackSelf();
			pAgent.SetVariable<uint>("p_targetID", heroWhoAttackSelf, 1128863647u);
			return result;
		}
	}
}
