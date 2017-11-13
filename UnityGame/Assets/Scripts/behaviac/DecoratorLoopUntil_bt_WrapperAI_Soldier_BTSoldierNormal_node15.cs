using System;

namespace behaviac
{
	internal class DecoratorLoopUntil_bt_WrapperAI_Soldier_BTSoldierNormal_node15 : DecoratorLoopUntil
	{
		public DecoratorLoopUntil_bt_WrapperAI_Soldier_BTSoldierNormal_node15()
		{
			this.m_bDecorateWhenChildEnds = true;
			this.m_until = true;
		}

		protected override int GetCount(Agent pAgent)
		{
			return -1;
		}
	}
}
