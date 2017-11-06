using Assets.Scripts.Framework;
using Assets.Scripts.UI;
using ResData;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class QQVipWidget : Singleton<QQVipWidget>
	{
		private GameObject m_BtnQQ;

		public void SetData(GameObject root, CUIFormScript formScript)
		{
			ResRandomRewardStore dataByKey = GameDataMgr.randomRewardDB.GetDataByKey(41001u);
			for (int i = 0; i < 3; i++)
			{
				string name = string.Format("Panel/QQVip/AwardGrid/QQ/ListElement{0}/ItemCell", i);
				GameObject gameObject = root.transform.FindChild(name).gameObject;
				ResDT_RandomRewardInfo resDT_RandomRewardInfo = dataByKey.astRewardDetail[i];
				CUseable cUseable = CUseableManager.CreateUsableByRandowReward((RES_RANDOM_REWARD_TYPE)resDT_RandomRewardInfo.bItemType, (int)resDT_RandomRewardInfo.dwLowCnt, resDT_RandomRewardInfo.dwItemID);
				if (cUseable != null)
				{
					if (gameObject.GetComponent<CUIEventScript>() == null)
					{
						gameObject.AddComponent<CUIEventScript>();
					}
					CUICommonSystem.SetItemCell(formScript, gameObject, cUseable, true, false, false, false);
				}
			}
			ResRandomRewardStore dataByKey2 = GameDataMgr.randomRewardDB.GetDataByKey(41002u);
			for (int j = 0; j < 3; j++)
			{
				string name2 = string.Format("Panel/QQVip/AwardGrid/QQVip/ListElement{0}/ItemCell", j);
				GameObject gameObject2 = root.transform.FindChild(name2).gameObject;
				ResDT_RandomRewardInfo resDT_RandomRewardInfo2 = dataByKey2.astRewardDetail[j];
				CUseable cUseable2 = CUseableManager.CreateUsableByRandowReward((RES_RANDOM_REWARD_TYPE)resDT_RandomRewardInfo2.bItemType, (int)resDT_RandomRewardInfo2.dwLowCnt, resDT_RandomRewardInfo2.dwItemID);
				if (cUseable2 != null)
				{
					if (gameObject2.GetComponent<CUIEventScript>() == null)
					{
						gameObject2.AddComponent<CUIEventScript>();
					}
					CUICommonSystem.SetItemCell(formScript, gameObject2, cUseable2, true, false, false, false);
				}
			}
			this.m_BtnQQ = root.transform.FindChild("Panel/QQVip/AwardGrid/QQ/Button/").gameObject;
			GameObject gameObject3 = root.transform.FindChild("Panel/QQVip/AwardGrid/QQVip/Button/").gameObject;
			Text componentInChildren = gameObject3.GetComponentInChildren<Text>();
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo != null)
			{
				if (masterRoleInfo.HasVip(16))
				{
					if (this.m_BtnQQ.activeInHierarchy)
					{
						this.m_BtnQQ.GetComponentInChildren<Text>().set_text("续费QQ会员");
					}
					componentInChildren.set_text("续费超级会员");
				}
				else if (masterRoleInfo.HasVip(1))
				{
					if (this.m_BtnQQ.activeInHierarchy)
					{
						this.m_BtnQQ.GetComponentInChildren<Text>().set_text("续费QQ会员");
					}
					componentInChildren.set_text("开通超级会员");
				}
				else if (!masterRoleInfo.HasVip(1))
				{
					if (this.m_BtnQQ.activeInHierarchy)
					{
						this.m_BtnQQ.GetComponentInChildren<Text>().set_text("开通QQ会员");
					}
					componentInChildren.set_text("开通超级会员");
				}
			}
		}

		public override void Init()
		{
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BuyPick_QQ, new CUIEventManager.OnUIEventHandler(this.BuyPcikQQ));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.BuyPick_QQVIP, new CUIEventManager.OnUIEventHandler(this.BuyPcikQQVip));
		}

		public override void UnInit()
		{
			this.Clear();
		}

		public void Clear()
		{
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BuyPick_QQ, new CUIEventManager.OnUIEventHandler(this.BuyPcikQQ));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.BuyPick_QQVIP, new CUIEventManager.OnUIEventHandler(this.BuyPcikQQVip));
		}

		private void BuyPcikQQ(CUIEvent uiEvent)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo != null)
			{
				if (masterRoleInfo.HasVip(1))
				{
					Singleton<ApolloHelper>.GetInstance().PayQQVip("LTMCLUB", "续费会员", 1);
				}
				else if (!masterRoleInfo.HasVip(1))
				{
					Singleton<ApolloHelper>.GetInstance().PayQQVip("LTMCLUB", "购买会员", 1);
				}
			}
		}

		private void BuyPcikQQVip(CUIEvent uiEvent)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo != null)
			{
				if (masterRoleInfo.HasVip(16))
				{
					Singleton<ApolloHelper>.GetInstance().PayQQVip("CJCLUBT", "续费超级会员", 1);
				}
				else if (!masterRoleInfo.HasVip(16))
				{
					Singleton<ApolloHelper>.GetInstance().PayQQVip("CJCLUBT", "购买超级会员", 1);
				}
			}
		}
	}
}
