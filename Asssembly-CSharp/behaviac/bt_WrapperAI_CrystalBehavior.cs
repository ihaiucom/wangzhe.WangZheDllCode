using System;

namespace behaviac
{
	public static class bt_WrapperAI_CrystalBehavior
	{
		public static bool build_behavior_tree(BehaviorTree bt)
		{
			bt.SetClassNameString("BehaviorTree");
			bt.SetId(-1);
			bt.SetName("WrapperAI/CrystalBehavior");
			bt.AddPar("Assets.Scripts.GameLogic.SkillSlotType", "p_curSlotType", "SLOT_SKILL_0", string.Empty);
			bt.AddPar("uint", "p_enemyID", "0", string.Empty);
			bt.AddPar("int", "p_srchRange", "0", string.Empty);
			bt.AddPar("UnityEngine.Vector3", "p_AttackMoveDest", "{kEpsilon=0;x=0;y=0;z=0;}", string.Empty);
			bt.AddPar("bool", "p_IsAttackMove_Attack", "false", string.Empty);
			bt.AddPar("bool", "p_AttackIsFinished", "true", string.Empty);
			bt.AddPar("uint", "p_CmdID", "0", string.Empty);
			DecoratorLoop_bt_WrapperAI_CrystalBehavior_node14 decoratorLoop_bt_WrapperAI_CrystalBehavior_node = new DecoratorLoop_bt_WrapperAI_CrystalBehavior_node14();
			decoratorLoop_bt_WrapperAI_CrystalBehavior_node.SetClassNameString("DecoratorLoop");
			decoratorLoop_bt_WrapperAI_CrystalBehavior_node.SetId(14);
			bt.AddChild(decoratorLoop_bt_WrapperAI_CrystalBehavior_node);
			Sequence sequence = new Sequence();
			sequence.SetClassNameString("Sequence");
			sequence.SetId(0);
			decoratorLoop_bt_WrapperAI_CrystalBehavior_node.AddChild(sequence);
			Condition_bt_WrapperAI_CrystalBehavior_node3 condition_bt_WrapperAI_CrystalBehavior_node = new Condition_bt_WrapperAI_CrystalBehavior_node3();
			condition_bt_WrapperAI_CrystalBehavior_node.SetClassNameString("Condition");
			condition_bt_WrapperAI_CrystalBehavior_node.SetId(3);
			sequence.AddChild(condition_bt_WrapperAI_CrystalBehavior_node);
			sequence.SetHasEvents(sequence.HasEvents() | condition_bt_WrapperAI_CrystalBehavior_node.HasEvents());
			Sequence sequence2 = new Sequence();
			sequence2.SetClassNameString("Sequence");
			sequence2.SetId(59);
			sequence.AddChild(sequence2);
			Action_bt_WrapperAI_CrystalBehavior_node4 action_bt_WrapperAI_CrystalBehavior_node = new Action_bt_WrapperAI_CrystalBehavior_node4();
			action_bt_WrapperAI_CrystalBehavior_node.SetClassNameString("Action");
			action_bt_WrapperAI_CrystalBehavior_node.SetId(4);
			sequence2.AddChild(action_bt_WrapperAI_CrystalBehavior_node);
			sequence2.SetHasEvents(sequence2.HasEvents() | action_bt_WrapperAI_CrystalBehavior_node.HasEvents());
			Action_bt_WrapperAI_CrystalBehavior_node5 action_bt_WrapperAI_CrystalBehavior_node2 = new Action_bt_WrapperAI_CrystalBehavior_node5();
			action_bt_WrapperAI_CrystalBehavior_node2.SetClassNameString("Action");
			action_bt_WrapperAI_CrystalBehavior_node2.SetId(5);
			sequence2.AddChild(action_bt_WrapperAI_CrystalBehavior_node2);
			sequence2.SetHasEvents(sequence2.HasEvents() | action_bt_WrapperAI_CrystalBehavior_node2.HasEvents());
			DecoratorLoop_bt_WrapperAI_CrystalBehavior_node65 decoratorLoop_bt_WrapperAI_CrystalBehavior_node2 = new DecoratorLoop_bt_WrapperAI_CrystalBehavior_node65();
			decoratorLoop_bt_WrapperAI_CrystalBehavior_node2.SetClassNameString("DecoratorLoop");
			decoratorLoop_bt_WrapperAI_CrystalBehavior_node2.SetId(65);
			sequence2.AddChild(decoratorLoop_bt_WrapperAI_CrystalBehavior_node2);
			Noop noop = new Noop();
			noop.SetClassNameString("Noop");
			noop.SetId(66);
			decoratorLoop_bt_WrapperAI_CrystalBehavior_node2.AddChild(noop);
			decoratorLoop_bt_WrapperAI_CrystalBehavior_node2.SetHasEvents(decoratorLoop_bt_WrapperAI_CrystalBehavior_node2.HasEvents() | noop.HasEvents());
			sequence2.SetHasEvents(sequence2.HasEvents() | decoratorLoop_bt_WrapperAI_CrystalBehavior_node2.HasEvents());
			sequence.SetHasEvents(sequence.HasEvents() | sequence2.HasEvents());
			decoratorLoop_bt_WrapperAI_CrystalBehavior_node.SetHasEvents(decoratorLoop_bt_WrapperAI_CrystalBehavior_node.HasEvents() | sequence.HasEvents());
			bt.SetHasEvents(bt.HasEvents() | decoratorLoop_bt_WrapperAI_CrystalBehavior_node.HasEvents());
			return true;
		}
	}
}
