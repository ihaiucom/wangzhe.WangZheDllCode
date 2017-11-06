using ResData;
using System;

namespace Assets.Scripts.GameSystem
{
	public interface IHeroData
	{
		uint cfgID
		{
			get;
		}

		uint skinID
		{
			get;
		}

		string heroName
		{
			get;
		}

		string heroTitle
		{
			get;
		}

		string imagePath
		{
			get;
		}

		int star
		{
			get;
		}

		int level
		{
			get;
		}

		int quality
		{
			get;
		}

		int subQuality
		{
			get;
		}

		bool bPlayerOwn
		{
			get;
		}

		int curExp
		{
			get;
		}

		int maxExp
		{
			get;
		}

		int heroType
		{
			get;
		}

		bool bIsPlayerUse
		{
			get;
		}

		uint proficiency
		{
			get;
		}

		byte proficiencyLV
		{
			get;
		}

		int combatEft
		{
			get;
		}

		uint sortId
		{
			get;
		}

		ResDT_SkillInfo[] skillArr
		{
			get;
		}

		ResHeroCfgInfo heroCfgInfo
		{
			get;
		}

		ResHeroPromotion promotion();

		bool IsValidExperienceHero();
	}
}
