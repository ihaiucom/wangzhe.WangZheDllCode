using Assets.Scripts.Framework;
using Assets.Scripts.Sound;
using Assets.Scripts.UI;
using ResData;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	internal class PVEPlayerItem : PVEExpItemBase
	{
		public PVEPlayerItem(GameObject playerItem)
		{
			this.m_Root = playerItem;
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			this.m_Name = masterRoleInfo.Name;
			this.m_NameText = this.m_Root.transform.Find("Name").GetComponent<Text>();
			this.m_LevelTxt = this.m_Root.transform.Find("Lv").GetComponent<Text>();
			this.m_ExpTxt = this.m_Root.transform.Find("Exp_Bar/Bar_Value").GetComponent<Text>();
			this.m_ExpBar1 = this.m_Root.transform.Find("Exp_Bar/Bar_Img").GetComponent<Image>();
			GameObject gameObject = this.m_Root.transform.Find("Player_Pic").gameObject;
			if (gameObject != null && !string.IsNullOrEmpty(masterRoleInfo.HeadUrl))
			{
				CUIHttpImageScript component = gameObject.GetComponent<CUIHttpImageScript>();
				component.SetImageUrl(masterRoleInfo.HeadUrl);
			}
		}

		public override void addExp(uint addVal)
		{
			CRoleInfo.GetPlayerPreLevleAndExp(addVal, out this.m_level, out this.m_exp);
			base.addExp(addVal);
			Singleton<CSoundManager>.GetInstance().PostEvent("UI_count_jingyan", null);
		}

		protected override void TweenEnd(float val)
		{
			if (this.m_maxExp == this.m_exp)
			{
				if ((long)base.Level >= (long)((ulong)GameDataMgr.acntExpDatabin.GetDataByKey(Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().Level).dwLimitHeroLevel))
				{
					return;
				}
				CUIEvent cUIEvent = new CUIEvent();
				cUIEvent.m_eventID = enUIEventID.Settle_OpenLvlUp;
				cUIEvent.m_eventParams.tag = base.Level;
				cUIEvent.m_eventParams.tag2 = base.Level + 1;
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(cUIEvent);
			}
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Settle_EscapeAnim);
			base.TweenEnd(val);
		}

		protected override uint calcMaxExp()
		{
			ResAcntExpInfo dataByKey = GameDataMgr.acntExpDatabin.GetDataByKey((uint)this.m_level);
			return dataByKey.dwNeedExp;
		}
	}
}
