using Assets.Scripts.GameSystem;
using System;

namespace Assets.Scripts.GameLogic
{
	public static class EnergyCommon
	{
		private static string[] energySpriteName = new string[]
		{
			"Battle_blueHp",
			"None",
			"Battle_whiteHp",
			"Battle_redHp",
			"Battle_redHp",
			"Battle_purpleHp",
			"None"
		};

		private static string[][] s_energyShowText = new string[][]
		{
			new string[]
			{
				"Hero_Prop_MaxEp",
				"Hero_Prop_MaxEp",
				"Hero_Prop_MaxEnergyEp",
				"Hero_Prop_MaxAngerEp",
				"Hero_Prop_MaxMadnessEp",
				"Hero_Prop_MaxEp",
				"Hero_Prop_MaxEp"
			},
			new string[]
			{
				"Skill_Energy_Cost_Tips",
				"Skill_Energy_Cost_Tips",
				"Skill_EnergyEp_Cost_Tips",
				"Skill_Anger_Cost_Tips",
				"Skill_Madness_Cost_Tips",
				"Skill_Energy_Cost_Tips",
				"Skill_Energy_Cost_Tips"
			},
			new string[]
			{
				"Hero_Prop_EpRecover",
				"Hero_Prop_EpRecover",
				"Hero_Prop_Energy_EpRecover",
				"Hero_Prop_Anger_EpRecover",
				"Hero_Prop_Madness_EpRecover",
				"Hero_Prop_EpRecover",
				"Hero_Prop_EpRecover"
			}
		};

		private static enOtherFloatTextContent[] energyShortageFloatText = new enOtherFloatTextContent[]
		{
			enOtherFloatTextContent.MagicShortage,
			enOtherFloatTextContent.MagicShortage,
			enOtherFloatTextContent.EnergyShortage,
			enOtherFloatTextContent.FuryShortage,
			enOtherFloatTextContent.MadnessShortage,
			enOtherFloatTextContent.MagicShortage,
			enOtherFloatTextContent.BloodShortage
		};

		public static string GetSpriteName(int index)
		{
			if (index < 0 || index >= EnergyCommon.energySpriteName.Length)
			{
				index = 0;
			}
			return EnergyCommon.energySpriteName[index];
		}

		public static enOtherFloatTextContent GetShortageText(int index)
		{
			if (index < 0 || index >= EnergyCommon.energyShortageFloatText.Length)
			{
				index = 0;
			}
			return EnergyCommon.energyShortageFloatText[index];
		}

		public static string GetEnergyShowText(uint energyType, EnergyShowType showType = EnergyShowType.CostValue)
		{
			if (energyType < 0u && (ulong)energyType >= (ulong)((long)EnergyCommon.s_energyShowText[(int)showType].Length))
			{
				energyType = 0u;
			}
			return EnergyCommon.s_energyShowText[(int)showType][(int)((UIntPtr)energyType)];
		}
	}
}
