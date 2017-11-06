using ResData;
using System;

namespace Assets.Scripts.GameLogic
{
	public class SkillDetectionControl : Singleton<SkillDetectionControl>
	{
		private DictionaryView<uint, SkillBaseDetection> _registedRule = new DictionaryView<uint, SkillBaseDetection>();

		public override void Init()
		{
			ClassEnumerator classEnumerator = new ClassEnumerator(typeof(SkillBaseDetectionAttribute), typeof(SkillBaseDetection), typeof(SkillBaseDetectionAttribute).get_Assembly(), true, false, false);
			foreach (Type current in classEnumerator.results)
			{
				SkillBaseDetection value = (SkillBaseDetection)Activator.CreateInstance(current);
				Attribute customAttribute = Attribute.GetCustomAttribute(current, typeof(SkillBaseDetectionAttribute));
				this._registedRule.Add((uint)(customAttribute as SkillBaseDetectionAttribute).UseRule, value);
			}
		}

		public bool Detection(SkillUseRule ruleType, SkillSlot slot)
		{
			SkillBaseDetection skillBaseDetection;
			return !this._registedRule.TryGetValue((uint)ruleType, out skillBaseDetection) || skillBaseDetection.Detection(slot);
		}
	}
}
