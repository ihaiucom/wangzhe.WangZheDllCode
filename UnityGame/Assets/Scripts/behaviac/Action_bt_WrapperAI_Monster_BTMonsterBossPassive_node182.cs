using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class Action_bt_WrapperAI_Monster_BTMonsterBossPassive_node182 : Action
	{
		protected override EBTStatus update_impl(Agent pAgent, EBTStatus childStatus)
		{
			((ObjAgent)pAgent).ResetMustUseNextSkillSlotType();
			return EBTStatus.BT_SUCCESS;
		}
	}
}
