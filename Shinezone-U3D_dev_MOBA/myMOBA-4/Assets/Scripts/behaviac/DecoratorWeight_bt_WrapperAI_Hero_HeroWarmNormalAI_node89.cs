using System;

namespace behaviac
{
	internal class DecoratorWeight_bt_WrapperAI_Hero_HeroWarmNormalAI_node89 : DecoratorWeight
	{
		public DecoratorWeight_bt_WrapperAI_Hero_HeroWarmNormalAI_node89()
		{
			this.m_bDecorateWhenChildEnds = false;
		}

		protected override int GetWeight(Agent pAgent)
		{
			return 1000;
		}
	}
}
