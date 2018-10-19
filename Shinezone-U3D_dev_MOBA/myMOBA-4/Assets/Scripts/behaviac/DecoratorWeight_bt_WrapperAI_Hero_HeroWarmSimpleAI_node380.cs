using System;

namespace behaviac
{
	internal class DecoratorWeight_bt_WrapperAI_Hero_HeroWarmSimpleAI_node380 : DecoratorWeight
	{
		public DecoratorWeight_bt_WrapperAI_Hero_HeroWarmSimpleAI_node380()
		{
			this.m_bDecorateWhenChildEnds = false;
		}

		protected override int GetWeight(Agent pAgent)
		{
			return (int)pAgent.GetVariable(4066292203u);
		}
	}
}
