using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic.GameKernal;
using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	public class SkillSelectControl : Singleton<SkillSelectControl>
	{
		private DictionaryView<uint, SkillBaseSelectTarget> registedRule = new DictionaryView<uint, SkillBaseSelectTarget>();

		private PoolObjHandle<ActorRoot> m_SkillTargetObj;

		public bool IsLowerHpMode()
		{
			Player hostPlayer = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer();
			return hostPlayer == null || hostPlayer.AttackTargetMode == SelectEnemyType.SelectLowHp;
		}

		public override void Init()
		{
			ClassEnumerator classEnumerator = new ClassEnumerator(typeof(SkillBaseSelectTargetAttribute), typeof(SkillBaseSelectTarget), typeof(SkillBaseSelectTargetAttribute).get_Assembly(), true, false, false);
			foreach (Type current in classEnumerator.results)
			{
				SkillBaseSelectTarget value = (SkillBaseSelectTarget)Activator.CreateInstance(current);
				Attribute customAttribute = Attribute.GetCustomAttribute(current, typeof(SkillBaseSelectTargetAttribute));
				this.registedRule.Add((uint)(customAttribute as SkillBaseSelectTargetAttribute).TargetRule, value);
			}
		}

		public ActorRoot SelectTarget(SkillTargetRule ruleType, SkillSlot slot)
		{
			if (this.m_SkillTargetObj)
			{
				return this.m_SkillTargetObj;
			}
			SkillBaseSelectTarget skillBaseSelectTarget;
			if (this.registedRule.TryGetValue((uint)ruleType, out skillBaseSelectTarget))
			{
				return skillBaseSelectTarget.SelectTarget(slot);
			}
			return null;
		}

		public VInt3 SelectTargetDir(SkillTargetRule ruleType, SkillSlot slot)
		{
			SkillBaseSelectTarget skillBaseSelectTarget;
			if (this.registedRule.TryGetValue((uint)ruleType, out skillBaseSelectTarget))
			{
				return skillBaseSelectTarget.SelectTargetDir(slot);
			}
			return slot.Actor.handle.forward;
		}

		public VInt3 SelectTargetPos(SkillTargetRule ruleType, SkillSlot slot, out bool bTarget)
		{
			bTarget = false;
			SkillBaseSelectTarget skillBaseSelectTarget;
			if (!this.registedRule.TryGetValue((uint)ruleType, out skillBaseSelectTarget))
			{
				return slot.Actor.handle.location;
			}
			ActorRoot actorRoot = skillBaseSelectTarget.SelectTarget(slot);
			if (actorRoot != null)
			{
				bTarget = true;
				return actorRoot.location;
			}
			return slot.Actor.handle.location;
		}
	}
}
