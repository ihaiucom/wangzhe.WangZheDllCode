using System;

namespace behaviac
{
	internal class DecoratorLoopUntil_bt_WrapperAI_Monster_BTMonsterPassive_node87 : DecoratorLoopUntil
	{
		public DecoratorLoopUntil_bt_WrapperAI_Monster_BTMonsterPassive_node87()
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
