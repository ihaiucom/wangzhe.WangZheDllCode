using Assets.Scripts.GameLogic;
using System;

namespace behaviac
{
	internal class WaitFrames_bt_WrapperAI_Organ_CrystalWithAttack_node52 : WaitFrames
	{
		protected override int GetFrames(Agent pAgent)
		{
			int frame = (int)pAgent.GetVariable(1311805792u);
			return ((ObjAgent)pAgent).GetWaitFrame(frame);
		}
	}
}
