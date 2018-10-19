using Assets.Scripts.Framework;
using ResData;
using System;

namespace Assets.Scripts.GameSystem
{
	internal class CHeroInfoData : IHeroData
	{
		public CHeroInfo m_info;

		private string m_name;

		private string m_imgPath;

		private string m_tilte;

		public uint cfgID
		{
			get
			{
				if (this.m_info != null && this.m_info.cfgInfo != null)
				{
					return this.m_info.cfgInfo.dwCfgID;
				}
				return 0u;
			}
		}

		public uint skinID
		{
			get
			{
				uint result = 0u;
				if (this.m_info.m_skinInfo != null)
				{
					result = this.m_info.m_skinInfo.GetWearSkinId();
				}
				return result;
			}
		}

		public string heroName
		{
			get
			{
				return this.m_name;
			}
		}

		public string heroTitle
		{
			get
			{
				string result = this.m_tilte;
				if (this.m_info.m_skinInfo != null)
				{
					uint wearSkinId = this.m_info.m_skinInfo.GetWearSkinId();
					if (wearSkinId != 0u)
					{
						ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(this.m_info.cfgInfo.dwCfgID, wearSkinId);
						if (heroSkin != null)
						{
							result = StringHelper.UTF8BytesToString(ref heroSkin.szSkinName);
						}
					}
				}
				return result;
			}
		}

		public string imagePath
		{
			get
			{
				string result = this.m_imgPath;
				if (this.m_info.m_skinInfo != null)
				{
					uint wearSkinId = this.m_info.m_skinInfo.GetWearSkinId();
					if (wearSkinId != 0u)
					{
						ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(this.m_info.cfgInfo.dwCfgID, wearSkinId);
						if (heroSkin != null)
						{
							result = StringHelper.UTF8BytesToString(ref heroSkin.szSkinPicID);
						}
					}
				}
				return result;
			}
		}

		public int star
		{
			get
			{
				return this.m_info.mActorValue.actorStar;
			}
		}

		public int level
		{
			get
			{
				return this.m_info.mActorValue.actorLvl;
			}
		}

		public int quality
		{
			get
			{
				return this.m_info.mActorValue.actorQuality;
			}
		}

		public int subQuality
		{
			get
			{
				return this.m_info.mActorValue.actorSubQuality;
			}
		}

		public bool bPlayerOwn
		{
			get
			{
				return !this.IsExperienceHero();
			}
		}

		public int curExp
		{
			get
			{
				return this.m_info.mActorValue.actorExp;
			}
		}

		public int maxExp
		{
			get
			{
				return this.m_info.mActorValue.actorMaxExp;
			}
		}

		public int heroType
		{
			get
			{
				return (int)this.m_info.cfgInfo.bMainJob;
			}
		}

		public bool bIsPlayerUse
		{
			get
			{
				return GameDataMgr.IsHeroAvailable(this.m_info.cfgInfo.dwCfgID);
			}
		}

		public uint proficiency
		{
			get
			{
				return this.m_info.m_Proficiency;
			}
		}

		public byte proficiencyLV
		{
			get
			{
				return this.m_info.m_ProficiencyLV;
			}
		}

		public int combatEft
		{
			get
			{
				return this.m_info.GetCombatEft();
			}
		}

		public uint sortId
		{
			get
			{
				return this.m_info.cfgInfo.dwShowSortId;
			}
		}

		public ResDT_SkillInfo[] skillArr
		{
			get
			{
				return this.m_info.cfgInfo.astSkill;
			}
		}

		public ResHeroCfgInfo heroCfgInfo
		{
			get
			{
				return this.m_info.cfgInfo;
			}
		}

		public CHeroInfoData(CHeroInfo info)
		{
			DebugHelper.Assert(info != null, "Create CHeroInfoData, CHeroInfo = null");
			if (info != null)
			{
				this.m_info = info;
				this.m_name = StringHelper.UTF8BytesToString(ref info.cfgInfo.szName);
				this.m_imgPath = StringHelper.UTF8BytesToString(ref info.cfgInfo.szImagePath);
				this.m_tilte = StringHelper.UTF8BytesToString(ref info.cfgInfo.szHeroTitle);
			}
		}

		public ResHeroPromotion promotion()
		{
			if (this.m_info.shopCfgInfo == null)
			{
				GameDataMgr.heroShopInfoDict.TryGetValue(this.m_info.cfgInfo.dwCfgID, out this.m_info.shopCfgInfo);
				if (this.m_info.shopCfgInfo == null)
				{
					return null;
				}
			}
			for (int i = 0; i < (int)this.m_info.shopCfgInfo.bPromotionCnt; i++)
			{
				uint num = this.m_info.shopCfgInfo.PromotionID[i];
				if (num != 0u)
				{
					if (GameDataMgr.heroPromotionDict.ContainsKey(num))
					{
						ResHeroPromotion resHeroPromotion = new ResHeroPromotion();
						if (GameDataMgr.heroPromotionDict.TryGetValue(num, out resHeroPromotion) && (ulong)resHeroPromotion.dwOnTimeGen <= (ulong)((long)CRoleInfo.GetCurrentUTCTime()) && (ulong)resHeroPromotion.dwOffTimeGen >= (ulong)((long)CRoleInfo.GetCurrentUTCTime()))
						{
							return resHeroPromotion;
						}
					}
				}
			}
			return null;
		}

		public bool IsExperienceHero()
		{
			return this.m_info != null && this.m_info.IsExperienceHero();
		}

		public bool IsValidExperienceHero()
		{
			return this.m_info != null && this.m_info.IsValidExperienceHero();
		}
	}
}
