using Assets.Scripts.Framework;
using ResData;
using System;

namespace Assets.Scripts.GameSystem
{
	internal class CHeroCfgData : IHeroData
	{
		private ResHeroCfgInfo m_cfgInfo;

		private ResHeroShop _heroShopInfo;

		private string m_name;

		private string m_imgPath;

		private string m_tilte;

		public uint cfgID
		{
			get
			{
				if (this.m_cfgInfo != null)
				{
					return this.m_cfgInfo.dwCfgID;
				}
				return 0u;
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
				return this.m_cfgInfo.iInitialStar;
			}
		}

		public int level
		{
			get
			{
				return 0;
			}
		}

		public int quality
		{
			get
			{
				return 1;
			}
		}

		public int subQuality
		{
			get
			{
				return 0;
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

		public ResHeroCfgInfo heroCfgInfo
		{
			get
			{
				return this.m_cfgInfo;
			}
		}

		public CHeroCfgData(uint id)
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

		public bool IsValidExperienceHero()
		{
			return false;
		}
	}
}
