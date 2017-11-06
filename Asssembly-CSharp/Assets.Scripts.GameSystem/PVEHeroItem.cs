using Assets.Scripts.Framework;
using ResData;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	internal class PVEHeroItem : PVEExpItemBase
	{
		private CHeroInfo heroInfo;

		private uint m_HeroId;

		public PVEHeroItem(GameObject heroItem, uint heroId)
		{
			this.m_Root = heroItem;
			this.m_HeroId = heroId;
			if (!Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GetHeroInfoDic().TryGetValue(heroId, out this.heroInfo))
			{
				DebugHelper.Assert(false);
				return;
			}
			this.m_Name = StringHelper.UTF8BytesToString(ref this.heroInfo.cfgInfo.szName);
			this.m_NameText = this.m_Root.transform.Find("Name").GetComponent<Text>();
			this.m_LevelTxt = this.m_Root.transform.Find("Lv").GetComponent<Text>();
			this.m_ExpTxt = this.m_Root.transform.Find("Exp_Bar/Bar_Value").GetComponent<Text>();
			this.m_ExpBar1 = this.m_Root.transform.Find("Exp_Bar/Bar_Img").GetComponent<Image>();
		}

		public override void addExp(uint addVal)
		{
			CRoleInfo.GetHeroPreLevleAndExp(this.m_HeroId, addVal, out this.m_level, out this.m_exp);
			base.addExp(addVal);
		}

		protected override uint calcMaxExp()
		{
			ResHeroLvlUpInfo dataByKey = GameDataMgr.heroLvlUpDatabin.GetDataByKey((uint)this.m_level);
			return dataByKey.dwExp;
		}

		protected override void SetUI()
		{
			base.SetUI();
			for (int i = 0; i < 5; i++)
			{
				GameObject gameObject = this.m_Root.transform.Find(string.Format("starPanel/imageStar{0}", i)).gameObject;
				if (this.heroInfo.mActorValue.actorStar > i)
				{
					gameObject.CustomSetActive(true);
				}
				else
				{
					gameObject.CustomSetActive(false);
				}
			}
		}
	}
}
