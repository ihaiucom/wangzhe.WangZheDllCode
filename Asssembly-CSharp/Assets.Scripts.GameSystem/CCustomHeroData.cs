using Assets.Scripts.Framework;
using ResData;
using System;

namespace Assets.Scripts.GameSystem
{
	internal class CCustomHeroData : IHeroData
	{
		private ResHeroCfgInfo m_cfgInfo;

		private ResHeroShop _heroShopInfo;

		private string m_name;

		private string m_imgPath;

		private string m_tilte;

		public int m_star;

		public int m_level;

		public int m_quaility;

		public int m_subQualility;

		public uint cfgID
		{
			get
			{
				return this.m_cfgInfo.dwCfgID;
			}
		}

		public uint skinID
		{
			get
			{
				return 0u;
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
				return this.m_tilte;
			}
		}

		public string imagePath
		{
			get
			{
				return this.m_imgPath;
			}
		}

		public int star
		{
			get
			{
				return this.m_star;
			}
		}

		public int level
		{
			get
			{
				return this.m_level;
			}
		}

		public int quality
		{
			get
			{
				return this.m_quaility;
			}
		}

		public int subQuality
		{
			get
			{
				return this.m_subQualility;
			}
		}

		public bool bPlayerOwn
		{
			get
			{
				return false;
			}
		}

		public int curExp
		{
			get
			{
				return 0;
			}
		}

		public int maxExp
		{
			get
			{
				return 0;
			}
		}

		public int heroType
		{
			get
			{
				return (int)this.m_cfgInfo.bMainJob;
			}
		}

		public bool bIsPlayerUse
		{
			get
			{
				return GameDataMgr.IsHeroAvailable(this.m_cfgInfo.dwCfgID);
			}
		}

		public uint proficiency
		{
			get
			{
				return 0u;
			}
		}

		public byte proficiencyLV
		{
			get
			{
				return 0;
			}
		}

		public int combatEft
		{
			get
			{
				return CHeroInfo.GetInitCombatByHeroId(this.m_cfgInfo.dwCfgID);
			}
		}

		public uint sortId
		{
			get
			{
				return this.m_cfgInfo.dwShowSortId;
			}
		}

		public ResDT_SkillInfo[] skillArr
		{
			get
			{
				return this.m_cfgInfo.astSkill;
			}
		}

		public int heroMaxHP
		{
			get
			{
				return this.m_cfgInfo.iBaseHP;
			}
		}

		public ResHeroCfgInfo heroCfgInfo
		{
			get
			{
				return this.m_cfgInfo;
			}
		}

		public CCustomHeroData(uint id)
		{
			this.m_cfgInfo = GameDataMgr.heroDatabin.GetDataByKey(id);
			GameDataMgr.heroShopInfoDict.TryGetValue(id, out this._heroShopInfo);
			if (this.m_cfgInfo == null)
			{
				DebugHelper.Assert(false, "ResHeroCfgInfo can not find id = {0}", new object[]
				{
					id
				});
				return;
			}
			this.m_name = StringHelper.UTF8BytesToString(ref this.m_cfgInfo.szName);
			this.m_imgPath = StringHelper.UTF8BytesToString(ref this.m_cfgInfo.szImagePath);
			this.m_tilte = StringHelper.UTF8BytesToString(ref this.m_cfgInfo.szHeroTitle);
		}

		public ResHeroPromotion promotion()
		{
			if (this._heroShopInfo == null)
			{
				return null;
			}
			for (int i = 0; i < (int)this._heroShopInfo.bPromotionCnt; i++)
			{
				uint num = this._heroShopInfo.PromotionID[i];
				if (num != 0u && GameDataMgr.heroPromotionDict.ContainsKey(num))
				{
					ResHeroPromotion resHeroPromotion = new ResHeroPromotion();
					if (GameDataMgr.heroPromotionDict.TryGetValue(num, out resHeroPromotion) && (ulong)resHeroPromotion.dwOnTimeGen <= (ulong)((long)CRoleInfo.GetCurrentUTCTime()) && (ulong)resHeroPromotion.dwOffTimeGen >= (ulong)((long)CRoleInfo.GetCurrentUTCTime()))
					{
						return resHeroPromotion;
					}
				}
			}
			return null;
		}

		public bool IsValidExperienceHero()
		{
			return false;
		}
	}
}
