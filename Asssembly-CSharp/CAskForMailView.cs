using Assets.Scripts.Framework;
using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using UnityEngine;
using UnityEngine.UI;

public class CAskForMailView
{
	private CUIFormScript form;

	public CUIFormScript Form
	{
		set
		{
			this.form = value;
		}
	}

	public CMail Mail
	{
		set
		{
			this.Draw(value);
		}
	}

	public CustomMailType mailType
	{
		set
		{
			if (this.form != null)
			{
				CUIEventScript componetInChild = Utility.GetComponetInChild<CUIEventScript>(this.form.gameObject, "Panel/refuse");
				CUIEventScript componetInChild2 = Utility.GetComponetInChild<CUIEventScript>(this.form.gameObject, "Panel/conform");
				if (componetInChild != null && componetInChild2 != null)
				{
					componetInChild.m_onClickEventID = enUIEventID.Mail_AskForRefuse;
					componetInChild2.m_onClickEventID = enUIEventID.Mail_AskForAccept;
				}
			}
		}
	}

	private void Draw(CMail mail)
	{
		if (this.form != null)
		{
			Text componetInChild = Utility.GetComponetInChild<Text>(this.form.gameObject, "Panel/msgContainer/name");
			Text componetInChild2 = Utility.GetComponetInChild<Text>(this.form.gameObject, "Panel/msgContainer/msg");
			Text componetInChild3 = Utility.GetComponetInChild<Text>(this.form.gameObject, "Panel/msgContainer/from");
			if (componetInChild == null || componetInChild2 == null || componetInChild3 == null)
			{
				return;
			}
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				DebugHelper.Assert(false, "Master Role Info is null");
				return;
			}
			componetInChild.set_text(string.Format(Singleton<CTextManager>.GetInstance().GetText("Mail_Ask_For_Myself"), masterRoleInfo.Name));
			componetInChild2.set_text(mail.mailContent);
			componetInChild3.set_text(string.Format(Singleton<CTextManager>.GetInstance().GetText("Mail_Ask_For_From", new string[]
			{
				mail.from
			}), new object[0]));
			if (mail.accessUseable == null || mail.accessUseable.Count == 0)
			{
				return;
			}
			CUseable cUseable = mail.accessUseable[0];
			switch (cUseable.m_type)
			{
			case COM_ITEM_TYPE.COM_OBJTYPE_HERO:
			{
				ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(cUseable.m_baseID);
				DebugHelper.Assert(dataByKey != null);
				if (dataByKey != null)
				{
					Text component = this.form.transform.Find("Panel/Title/titleText").GetComponent<Text>();
					component.set_text(Singleton<CTextManager>.GetInstance().GetText("Ask_For_Hero_Friend_Title"));
					Text component2 = this.form.transform.Find("Panel/skinBgImage/skinNameText").GetComponent<Text>();
					component2.set_text(StringHelper.UTF8BytesToString(ref dataByKey.szName));
					Image component3 = this.form.transform.Find("Panel/skinBgImage/skinIconImage").GetComponent<Image>();
					component3.SetSprite(CUIUtility.s_Sprite_Dynamic_BustHero_Dir + StringHelper.UTF8BytesToString(ref dataByKey.szImagePath), this.form, false, true, true, true);
					this.form.transform.Find("Panel/Panel_Prop").gameObject.CustomSetActive(false);
					Transform transform = this.form.transform.Find("Panel/skinPricePanel");
					Transform costIcon = transform.Find("costImage");
					CHeroSkinBuyManager.SetPayCostIcon(this.form, costIcon, enPayType.DianQuan);
					Transform costTypeText = transform.Find("costTypeText");
					CHeroSkinBuyManager.SetPayCostTypeText(costTypeText, enPayType.DianQuan);
					uint payValue = 0u;
					IHeroData heroData = CHeroDataFactory.CreateHeroData(cUseable.m_baseID);
					ResHeroPromotion resPromotion = heroData.promotion();
					stPayInfoSet payInfoSetOfGood = CMallSystem.GetPayInfoSetOfGood(dataByKey, resPromotion);
					for (int i = 0; i < payInfoSetOfGood.m_payInfoCount; i++)
					{
						if (payInfoSetOfGood.m_payInfos[i].m_payType == enPayType.Diamond || payInfoSetOfGood.m_payInfos[i].m_payType == enPayType.DianQuan || payInfoSetOfGood.m_payInfos[i].m_payType == enPayType.DiamondAndDianQuan)
						{
							payValue = payInfoSetOfGood.m_payInfos[i].m_payValue;
							break;
						}
					}
					Transform transform2 = transform.Find("costPanel");
					if (transform2)
					{
						Transform currentPrice = transform2.Find("costText");
						CHeroSkinBuyManager.SetPayCurrentPrice(currentPrice, payValue);
					}
				}
				break;
			}
			case COM_ITEM_TYPE.COM_OBJTYPE_HEROSKIN:
			{
				uint heroId = 0u;
				uint skinId = 0u;
				CSkinInfo.ResolveHeroSkin(cUseable.m_baseID, out heroId, out skinId);
				ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(cUseable.m_baseID);
				DebugHelper.Assert(heroSkin != null, "heroSkin is null");
				if (heroSkin != null)
				{
					Text component4 = this.form.transform.Find("Panel/Title/titleText").GetComponent<Text>();
					component4.set_text(Singleton<CTextManager>.GetInstance().GetText("Ask_For_Skin_Friend_Title"));
					Image component5 = this.form.transform.Find("Panel/skinBgImage/skinIconImage").GetComponent<Image>();
					string prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_BustHero_Dir, StringHelper.UTF8BytesToString(ref heroSkin.szSkinPicID));
					component5.SetSprite(prefabPath, this.form, false, true, true, true);
					Text component6 = this.form.transform.Find("Panel/skinBgImage/skinNameText").GetComponent<Text>();
					component6.set_text(StringHelper.UTF8BytesToString(ref heroSkin.szSkinName));
					this.form.transform.Find("Panel/Panel_Prop").gameObject.CustomSetActive(true);
					GameObject gameObject = this.form.transform.Find("Panel/Panel_Prop/List_Prop").gameObject;
					CSkinInfo.GetHeroSkinProp(heroId, skinId, ref CHeroInfoSystem2.s_propArr, ref CHeroInfoSystem2.s_propPctArr, ref CHeroInfoSystem2.s_propImgArr);
					CUICommonSystem.SetListProp(gameObject, ref CHeroInfoSystem2.s_propArr, ref CHeroInfoSystem2.s_propPctArr);
					Transform transform3 = this.form.transform.Find("Panel/skinPricePanel");
					Transform costIcon2 = transform3.Find("costImage");
					CHeroSkinBuyManager.SetPayCostIcon(this.form, costIcon2, enPayType.DianQuan);
					Transform costTypeText2 = transform3.Find("costTypeText");
					CHeroSkinBuyManager.SetPayCostTypeText(costTypeText2, enPayType.DianQuan);
					uint payValue2 = 0u;
					stPayInfoSet skinPayInfoSet = CSkinInfo.GetSkinPayInfoSet(heroId, skinId);
					for (int j = 0; j < skinPayInfoSet.m_payInfoCount; j++)
					{
						if (skinPayInfoSet.m_payInfos[j].m_payType == enPayType.Diamond || skinPayInfoSet.m_payInfos[j].m_payType == enPayType.DianQuan || skinPayInfoSet.m_payInfos[j].m_payType == enPayType.DiamondAndDianQuan)
						{
							payValue2 = skinPayInfoSet.m_payInfos[j].m_payValue;
							break;
						}
					}
					Transform transform4 = transform3.Find("costPanel");
					if (transform4 != null)
					{
						Transform currentPrice2 = transform4.Find("costText");
						CHeroSkinBuyManager.SetPayCurrentPrice(currentPrice2, payValue2);
					}
				}
				break;
			}
			}
		}
	}
}
