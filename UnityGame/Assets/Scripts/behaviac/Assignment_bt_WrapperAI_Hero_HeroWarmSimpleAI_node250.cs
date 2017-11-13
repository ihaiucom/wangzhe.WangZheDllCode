using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Assignment_bt_WrapperAI_Hero_HeroWarmSimpleAI_node250 : Assignment
	{
		private int opr_p0;

		public Assignment_bt_WrapperAI_Hero_HeroWarmSimpleAI_node250()
		{
			this.opr_p0 = 30000;
		}

		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			EBTStatus result = EBTStatus.BT_SUCCESS;
			uint nearestEnemyIgnoreVisible = ((ObjAgent)pAgent).GetNearestEnemyIgnoreVisible(this.opr_p0);
			pAgent.SetVariable<uint>("p_targetID", nearestEnemyIgnoreVisible, 1128863647u);
			return result;
		}
	}
}
