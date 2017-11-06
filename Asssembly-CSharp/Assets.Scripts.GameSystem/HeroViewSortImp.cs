using Assets.Scripts.Framework;
using ResData;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Assets.Scripts.GameSystem
{
	public class HeroViewSortImp : Singleton<HeroViewSortImp>, IMallSort<CMallSortHelper.HeroViewSortType>, IComparer<IHeroData>
	{
		private CMallSortHelper.HeroViewSortType m_sortType;

		private bool m_desc;

		private CultureInfo m_culture;

		public override void Init()
		{
			base.Init();
			this.m_sortType = CMallSortHelper.HeroViewSortType.Default;
			this.m_desc = false;
			this.m_culture = new CultureInfo("zh-CN");
		}

		public override void UnInit()
		{
			base.UnInit();
			this.m_sortType = CMallSortHelper.HeroViewSortType.Default;
			this.m_desc = false;
			this.m_culture = null;
		}

		public string GetSortTypeName(CMallSortHelper.HeroViewSortType sortType = CMallSortHelper.HeroViewSortType.Default)
		{
			if (sortType < CMallSortHelper.HeroViewSortType.Default || sortType > (CMallSortHelper.HeroViewSortType)CMallSortHelper.heroViewSortTypeNameKeys.Length)
			{
				return null;
			}
			return Singleton<CTextManager>.GetInstance().GetText(CMallSortHelper.heroViewSortTypeNameKeys[(int)sortType]);
		}

		public CMallSortHelper.HeroViewSortType GetCurSortType()
		{
			return this.m_sortType;
		}

		public bool IsDesc()
		{
			return this.m_desc;
		}

		public void SetSortType(CMallSortHelper.HeroViewSortType sortType = CMallSortHelper.HeroViewSortType.Default)
		{
			this.m_sortType = sortType;
			if (this.m_sortType == CMallSortHelper.HeroViewSortType.Default)
			{
				this.m_desc = false;
			}
		}

		public void SetDesc(bool bDesc)
		{
			this.m_desc = bDesc;
		}

		private int CompareDefault(IHeroData l, IHeroData r)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				DebugHelper.Assert(false, "CompareDefault role is null");
				return 0;
			}
			if (l.bPlayerOwn && !r.bPlayerOwn)
			{
				return -1;
			}
			if (r.bPlayerOwn && !l.bPlayerOwn)
			{
				return 1;
			}
			if (!r.bPlayerOwn && !l.bPlayerOwn)
			{
				if (masterRoleInfo.IsFreeHero(l.cfgID) && !masterRoleInfo.IsFreeHero(r.cfgID))
				{
					return -1;
				}
				if (masterRoleInfo.IsFreeHero(r.cfgID) && !masterRoleInfo.IsFreeHero(l.cfgID))
				{
					return 1;
				}
				if (!masterRoleInfo.IsFreeHero(r.cfgID) && !masterRoleInfo.IsFreeHero(l.cfgID))
				{
					if (masterRoleInfo.IsValidExperienceHero(l.cfgID) && !masterRoleInfo.IsValidExperienceHero(r.cfgID))
					{
						return -1;
					}
					if (masterRoleInfo.IsValidExperienceHero(r.cfgID) && !masterRoleInfo.IsValidExperienceHero(l.cfgID))
					{
						return 1;
					}
				}
			}
			return l.sortId.CompareTo(r.sortId);
		}

		private int CompareReleaseTime(IHeroData l, IHeroData r)
		{
			ResHeroShop resHeroShop = null;
			ResHeroShop resHeroShop2 = null;
			GameDataMgr.heroShopInfoDict.TryGetValue(l.cfgID, out resHeroShop);
			GameDataMgr.heroShopInfoDict.TryGetValue(r.cfgID, out resHeroShop2);
			if (resHeroShop == null)
			{
				return 1;
			}
			if (resHeroShop2 == null)
			{
				return -1;
			}
			return resHeroShop.dwReleaseId.CompareTo(resHeroShop2.dwReleaseId);
		}

		private int CompareName(IHeroData l, IHeroData r)
		{
			return string.Compare(l.heroCfgInfo.szName, r.heroCfgInfo.szName, this.m_culture, 0);
		}

		private int CompareProficiency(IHeroData l, IHeroData r)
		{
			if (l.proficiencyLV != r.proficiencyLV)
			{
				return r.proficiencyLV.CompareTo(l.proficiencyLV);
			}
			if (l.proficiency != r.proficiency)
			{
				return r.proficiency.CompareTo(l.proficiency);
			}
			if (l.bPlayerOwn && !r.bPlayerOwn)
			{
				return -1;
			}
			if (r.bPlayerOwn && !l.bPlayerOwn)
			{
				return 1;
			}
			return this.CompareName(l, r);
		}

		public int Compare(IHeroData l, IHeroData r)
		{
			if (l == null || l.heroCfgInfo == null)
			{
				return 1;
			}
			if (r == null || r.heroCfgInfo == null)
			{
				return -1;
			}
			switch (this.m_sortType)
			{
			case CMallSortHelper.HeroViewSortType.Name:
				return this.CompareName(l, r);
			case CMallSortHelper.HeroViewSortType.Proficiency:
				return this.CompareProficiency(l, r);
			case CMallSortHelper.HeroViewSortType.ReleaseTime:
				return this.CompareReleaseTime(l, r);
			default:
				return this.CompareDefault(l, r);
			}
		}
	}
}
