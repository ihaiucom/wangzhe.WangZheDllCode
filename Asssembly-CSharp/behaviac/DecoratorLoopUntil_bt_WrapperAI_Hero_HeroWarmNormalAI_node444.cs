using System;

namespace behaviac
{
	internal class DecoratorLoopUntil_bt_WrapperAI_Hero_HeroWarmNormalAI_node444 : DecoratorLoopUntil
	{
		public DecoratorLoopUntil_bt_WrapperAI_Hero_HeroWarmNormalAI_node444()
		{
			this.m_bDecorateWhenChildEnds = true;
			this.m_until = true;
		}

		protected override int GetCount(Agent pAgent)
		{
			return 7;
		}
	}
}
