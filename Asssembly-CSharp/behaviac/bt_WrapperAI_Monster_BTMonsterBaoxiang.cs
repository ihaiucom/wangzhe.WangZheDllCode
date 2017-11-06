using System;

namespace behaviac
{
	public static class bt_WrapperAI_Monster_BTMonsterBaoxiang
	{
		public static bool build_behavior_tree(BehaviorTree bt)
		{
			bt.SetClassNameString("BehaviorTree");
			bt.SetId(-1);
			bt.SetName("WrapperAI/Monster/BTMonsterBaoxiang");
			bt.AddPar("Assets.Scripts.GameLogic.SkillSlotType", "p_curSlotType", "SLOT_SKILL_0", string.Empty);
			bt.AddPar("uint", "p_targetID", "0", string.Empty);
			bt.AddPar("int", "p_srchRange", "0", string.Empty);
			bt.AddPar("UnityEngine.Vector3", "p_AttackMoveDest", "{kEpsilon=0;x=0;y=0;z=0;}", string.Empty);
			bt.AddPar("bool", "p_IsAttackMove_Attack", "false", string.Empty);
			bt.AddPar("bool", "p_AttackIsFinished", "true", string.Empty);
			bt.AddPar("uint", "p_CmdID", "0", string.Empty);
			bt.AddPar("UnityEngine.Vector3", "p_attackPathCurTargetPos", "{kEpsilon=0;x=0;y=0;z=0;}", string.Empty);
			bt.AddPar("int", "p_skillAttackRange", "0", string.Empty);
			bt.AddPar("UnityEngine.Vector3", "p_orignalPos", "{kEpsilon=0;x=0;y=0;z=0;}", string.Empty);
			bt.AddPar("int", "p_follow_range", "9000", string.Empty);
			bt.AddPar("UnityEngine.Vector3", "p_orignalDirection", "{kEpsilon=0;x=0;y=0;z=0;}", string.Empty);
			bt.AddPar("int", "p_pursuitRange", "0", string.Empty);
			bt.AddPar("int", "p_waitFramesToSearch", "10", string.Empty);
			bt.AddPar("uint", "p_selfID", "0", string.Empty);
			DecoratorLoop_bt_WrapperAI_Monster_BTMonsterBaoxiang_node14 decoratorLoop_bt_WrapperAI_Monster_BTMonsterBaoxiang_node = new DecoratorLoop_bt_WrapperAI_Monster_BTMonsterBaoxiang_node14();
			decoratorLoop_bt_WrapperAI_Monster_BTMonsterBaoxiang_node.SetClassNameString("DecoratorLoop");
			decoratorLoop_bt_WrapperAI_Monster_BTMonsterBaoxiang_node.SetId(14);
			bt.AddChild(decoratorLoop_bt_WrapperAI_Monster_BTMonsterBaoxiang_node);
			Sequence sequence = new Sequence();
			sequence.SetClassNameString("Sequence");
			sequence.SetId(22);
			decoratorLoop_bt_WrapperAI_Monster_BTMonsterBaoxiang_node.AddChild(sequence);
			Condition_bt_WrapperAI_Monster_BTMonsterBaoxiang_node3 condition_bt_WrapperAI_Monster_BTMonsterBaoxiang_node = new Condition_bt_WrapperAI_Monster_BTMonsterBaoxiang_node3();
			condition_bt_WrapperAI_Monster_BTMonsterBaoxiang_node.SetClassNameString("Condition");
			condition_bt_WrapperAI_Monster_BTMonsterBaoxiang_node.SetId(3);
			sequence.AddChild(condition_bt_WrapperAI_Monster_BTMonsterBaoxiang_node);
			sequence.SetHasEvents(sequence.HasEvents() | condition_bt_WrapperAI_Monster_BTMonsterBaoxiang_node.HasEvents());
			Action_bt_WrapperAI_Monster_BTMonsterBaoxiang_node4 action_bt_WrapperAI_Monster_BTMonsterBaoxiang_node = new Action_bt_WrapperAI_Monster_BTMonsterBaoxiang_node4();
			action_bt_WrapperAI_Monster_BTMonsterBaoxiang_node.SetClassNameString("Action");
			action_bt_WrapperAI_Monster_BTMonsterBaoxiang_node.SetId(4);
			sequence.AddChild(action_bt_WrapperAI_Monster_BTMonsterBaoxiang_node);
			sequence.SetHasEvents(sequence.HasEvents() | action_bt_WrapperAI_Monster_BTMonsterBaoxiang_node.HasEvents());
			DecoratorLoop_bt_WrapperAI_Monster_BTMonsterBaoxiang_node23 decoratorLoop_bt_WrapperAI_Monster_BTMonsterBaoxiang_node2 = new DecoratorLoop_bt_WrapperAI_Monster_BTMonsterBaoxiang_node23();
			decoratorLoop_bt_WrapperAI_Monster_BTMonsterBaoxiang_node2.SetClassNameString("DecoratorLoop");
			decoratorLoop_bt_WrapperAI_Monster_BTMonsterBaoxiang_node2.SetId(23);
			sequence.AddChild(decoratorLoop_bt_WrapperAI_Monster_BTMonsterBaoxiang_node2);
			Noop noop = new Noop();
			noop.SetClassNameString("Noop");
			noop.SetId(458);
			decoratorLoop_bt_WrapperAI_Monster_BTMonsterBaoxiang_node2.AddChild(noop);
			decoratorLoop_bt_WrapperAI_Monster_BTMonsterBaoxiang_node2.SetHasEvents(decoratorLoop_bt_WrapperAI_Monster_BTMonsterBaoxiang_node2.HasEvents() | noop.HasEvents());
			sequence.SetHasEvents(sequence.HasEvents() | decoratorLoop_bt_WrapperAI_Monster_BTMonsterBaoxiang_node2.HasEvents());
			decoratorLoop_bt_WrapperAI_Monster_BTMonsterBaoxiang_node.SetHasEvents(decoratorLoop_bt_WrapperAI_Monster_BTMonsterBaoxiang_node.HasEvents() | sequence.HasEvents());
			bt.SetHasEvents(bt.HasEvents() | decoratorLoop_bt_WrapperAI_Monster_BTMonsterBaoxiang_node.HasEvents());
			return true;
		}
	}
}
