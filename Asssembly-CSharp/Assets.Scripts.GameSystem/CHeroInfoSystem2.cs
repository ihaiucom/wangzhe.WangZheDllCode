using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.DataCenter;
using Assets.Scripts.Sound;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	[MessageHandlerClass]
	public class CHeroInfoSystem2 : Singleton<CHeroInfoSystem2>
	{
		private const int c_maxSelectedHeroIDsBeforeGC = 3;

		public static string s_heroInfoFormPath = "UGUI/Form/System/HeroInfo/Form_HeroInfo2.prefab";

		public static string s_heroStoryFormPath = "UGUI/Form/System/HeroInfo/Form_Hero_Story.prefab";

		public static string s_heroPropertyFormPath = "UGUI/Form/System/HeroInfo/Form_Hero_Property.prefab";

		public static string s_heroLeftPanel = "Panel_Left";

		public static readonly string valForm1 = "<color=#60bd67>{0}</color>({1}+<color=#60bd67>{2}</color>)";

		public static readonly string valForm2 = "<color=#60bd67>{0}</color>";

		public static readonly string valForm3 = "<color=#60bd67>{0}</color>|{1}";

		public static readonly string valForm4 = "{0}|{1}";

		public static int s_maxBasePropVal = 10;

		private CUIFormScript m_heroInfoForm;

		protected string m_heroModelPath = string.Empty;

		protected string m_heroImgPath = string.Empty;

		public uint m_selectHeroID;

		public IHeroData m_selectHeroData;

		private enHeroFormOpenSrc m_OpenSrc;

		private bool m_curShow2DImage;

		public uint m_currentDisplayedHeroID;

		private List<uint> m_selectedHeroIDs = new List<uint>();

		public static int[] s_propArr = new int[37];

		public static int[] s_propPctArr = new int[37];

		public static string[] s_propImgArr = new string[37];

		public static int[] s_propValAddArr = new int[37];

		public static int[] s_propPctAddArr = new int[37];

		public override void Init()
		{
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroInfo_ViewStory, new CUIEventManager.OnUIEventHandler(this.OnViewHeroStory));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroInfo_ViewProperty, new CUIEventManager.OnUIEventHandler(this.OnViewHeroProperty));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroInfo_OpenCustomEquipPanel, new CUIEventManager.OnUIEventHandler(this.OnOpenCustomEquipPanel));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroInfo_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_OpenForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroInfo_FormClose, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_FormClose));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroInfo_Appear, new CUIEventManager.OnUIEventHandler(this.OnHeroInfoFormApper));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroInfo_TurnLeft, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_TurnLeft));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroInfo_TurnRight, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_TurnRight));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroInfo_GotoRank_God, new CUIEventManager.OnUIEventHandler(this.OnHeroInfoGotoRankGod));
			Singleton<EventRouter>.instance.AddEventHandler<string>("HeroUnlockPvP", new Action<string>(this.OnHeroUnlockPvP));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroInfo_MenuSelect_Dummy, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_MenuSelect_Dummy));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroInfo_Material_Direct_Buy, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_MaterialDirectBuy));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroInfo_Material_Direct_Buy_Confirm, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_MaterialDirectBuyConfirm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroInfo_SkillTipOpen, new CUIEventManager.OnUIEventHandler(this.OnHeroSkillTipOpen));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroInfo_SkillTipClose, new CUIEventManager.OnUIEventHandler(this.OnHeroSkillTipClose));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSkin_ItemSelect, new CUIEventManager.OnUIEventHandler(this.OnHeroSkin_ItemSelect));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSkin_ItemEnable, new CUIEventManager.OnUIEventHandler(this.OnHeroSkin_ItemEnable));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroSkin_Wear, new CUIEventManager.OnUIEventHandler(this.OnHeroSkin_Wear));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroInfo_Show2DImage, new CUIEventManager.OnUIEventHandler(this.OnShow2DImage));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroInfo_Show3DImage, new CUIEventManager.OnUIEventHandler(this.OnShow3DImage));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.HeroInfo_OpenSkinUrl, new CUIEventManager.OnUIEventHandler(this.OnOpenSkinUrl));
			Singleton<EventRouter>.instance.AddEventHandler<uint>("HeroAdd", new Action<uint>(this.OnNtyAddHero));
			Singleton<EventRouter>.instance.AddEventHandler<uint, uint, uint>("HeroSkinAdd", new Action<uint, uint, uint>(this.OnAddHeroSkin));
			Singleton<EventRouter>.instance.AddEventHandler(EventID.GLOBAL_REFRESH_TIME, new Action(CHeroInfoSystem2.ResetDirectBuyLimit));
		}

		public override void UnInit()
		{
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.HeroInfo_ViewStory, new CUIEventManager.OnUIEventHandler(this.OnViewHeroStory));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.HeroInfo_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_OpenForm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.HeroInfo_FormClose, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_FormClose));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.HeroInfo_TurnLeft, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_TurnLeft));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.HeroInfo_TurnRight, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_TurnRight));
			Singleton<EventRouter>.instance.RemoveEventHandler<string>("HeroUnlockPvP", new Action<string>(this.OnHeroUnlockPvP));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.HeroInfo_MenuSelect_Dummy, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_MenuSelect_Dummy));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.HeroInfo_Material_Direct_Buy, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_MaterialDirectBuy));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.HeroInfo_Material_Direct_Buy_Confirm, new CUIEventManager.OnUIEventHandler(this.OnHeroInfo_MaterialDirectBuyConfirm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.HeroInfo_Show2DImage, new CUIEventManager.OnUIEventHandler(this.OnShow2DImage));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.HeroInfo_Show3DImage, new CUIEventManager.OnUIEventHandler(this.OnShow3DImage));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.HeroInfo_OpenSkinUrl, new CUIEventManager.OnUIEventHandler(this.OnOpenSkinUrl));
			Singleton<EventRouter>.instance.RemoveEventHandler<uint>("HeroAdd", new Action<uint>(this.OnNtyAddHero));
			Singleton<EventRouter>.instance.RemoveEventHandler<uint, uint, uint>("HeroSkinAdd", new Action<uint, uint, uint>(this.OnAddHeroSkin));
			Singleton<EventRouter>.instance.RemoveEventHandler(EventID.GLOBAL_REFRESH_TIME, new Action(CHeroInfoSystem2.ResetDirectBuyLimit));
		}

		public void OnHeroInfo_OpenForm(CUIEvent uiEvent)
		{
			OutlineFilter.EnableSurfaceShaderOutline(true);
			this.m_selectHeroID = uiEvent.m_eventParams.openHeroFormPar.heroId;
			this.m_OpenSrc = uiEvent.m_eventParams.openHeroFormPar.openSrc;
			this.m_selectHeroData = CHeroDataFactory.CreateHeroData(this.m_selectHeroID);
			this.m_heroInfoForm = Singleton<CUIManager>.GetInstance().OpenForm(CHeroInfoSystem2.s_heroInfoFormPath, true, true);
			if (this.m_heroInfoForm != null)
			{
				if (GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_HEROINFO_IS_SHOW_2D_ID) == 0u)
				{
					this.m_heroInfoForm.GetWidget(18).CustomSetActive(false);
				}
				Singleton<CLobbySystem>.GetInstance().SetTopBarPriority(enFormPriority.Priority1);
				Singleton<CUIManager>.GetInstance().LoadUIScenePrefab(CUIUtility.s_heroSceneBgPath, this.m_heroInfoForm);
				this.SetCurModelShowType(false);
				this.RefreshHeroInfoForm();
				if (this.m_OpenSrc == enHeroFormOpenSrc.SkinBuyClick)
				{
					this.SelectSkinElement(this.m_selectHeroID, uiEvent.m_eventParams.openHeroFormPar.skinId);
				}
				else
				{
					uint heroWearSkinId = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GetHeroWearSkinId(this.m_selectHeroID);
					this.SelectSkinElement(this.m_selectHeroID, heroWearSkinId);
				}
				this.UpdateSwitchButton(this.m_heroInfoForm);
				DynamicShadow.EnableDynamicShow(this.m_heroInfoForm.gameObject, true);
				this.SetVictoryTipsEvt();
				if (CSysDynamicBlock.bLobbyEntryBlocked)
				{
					CUICommonSystem.SetObjActive(this.m_heroInfoForm.transform.FindChild("Panel_Left/Button_Goto_Rank"), false);
					CUICommonSystem.SetObjActive(this.m_heroInfoForm.GetWidget(18), false);
				}
				GameObject widget = this.m_heroInfoForm.GetWidget(5);
				if (widget != null)
				{
					widget.CustomSetActive(false);
				}
			}
		}

		public void OnHeroInfo_FormClose(CUIEvent uiEvent)
		{
			this.m_heroInfoForm = null;
			OutlineFilter.EnableSurfaceShaderOutline(false);
			DynamicShadow.DisableAllDynamicShadows();
			Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharShow");
			Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharSkillIcon");
			Singleton<CResourceManager>.GetInstance().UnloadUnusedAssets();
			this.m_selectedHeroIDs.Clear();
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.HeroInfo_SkillTipClose);
			Singleton<CSoundManager>.GetInstance().PostEvent("Stop_Show", null);
		}

		private void OnHeroInfoFormApper(CUIEvent uiEvent)
		{
			if (this.m_heroInfoForm != null)
			{
				GameObject widget = this.m_heroInfoForm.GetWidget(2);
				if (null == widget)
				{
					return;
				}
				CUIListScript component = widget.transform.Find("List_Skin").GetComponent<CUIListScript>();
				int selectedIndex = component.GetSelectedIndex();
				if (this.m_curShow2DImage)
				{
					this.Refresh2DImage(this.m_selectHeroID, (int)CSkinInfo.GetSkinIdByIndex(this.m_selectHeroID, selectedIndex));
				}
				else
				{
					this.Refresh3DModel(uiEvent.m_srcFormScript.gameObject, this.m_selectHeroID, (int)CSkinInfo.GetSkinIdByIndex(this.m_selectHeroID, selectedIndex), false);
				}
			}
		}

		public void OnHeroInfo_TurnLeft(CUIEvent uiEvent)
		{
			switch (this.m_OpenSrc)
			{
			case enHeroFormOpenSrc.HeroListClick:
			{
				int heroIndexByConfigId = Singleton<CHeroOverviewSystem>.GetInstance().GetHeroIndexByConfigId(this.m_selectHeroID);
				if (heroIndexByConfigId > 0)
				{
					int index = heroIndexByConfigId - 1;
					this.m_selectHeroData = Singleton<CHeroOverviewSystem>.GetInstance().GetHeroDataBuyIndex(index);
					if (this.m_selectHeroData != null)
					{
						uint heroWearSkinId = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GetHeroWearSkinId(this.m_selectHeroData.cfgID);
						this.SwitchHeroInfo(this.m_selectHeroData.cfgID, heroWearSkinId, true, true);
						Singleton<CSoundManager>.GetInstance().PostEvent("UI_hero_page", null);
					}
				}
				break;
			}
			case enHeroFormOpenSrc.HeroBuyClick:
			{
				int heroIndexByConfigId2 = Singleton<CMallSystem>.GetInstance().m_heroMallCtrl.GetHeroIndexByConfigId(this.m_selectHeroID);
				if (heroIndexByConfigId2 > 0)
				{
					int index2 = heroIndexByConfigId2 - 1;
					this.m_selectHeroData = Singleton<CMallSystem>.GetInstance().m_heroMallCtrl.GetHeroDataByIndex(index2);
					if (this.m_selectHeroData != null)
					{
						uint heroWearSkinId2 = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GetHeroWearSkinId(this.m_selectHeroData.cfgID);
						this.SwitchHeroInfo(this.m_selectHeroData.cfgID, heroWearSkinId2, true, true);
						Singleton<CSoundManager>.GetInstance().PostEvent("UI_hero_page", null);
					}
				}
				break;
			}
			case enHeroFormOpenSrc.SkinBuyClick:
			{
				GameObject widget = this.m_heroInfoForm.GetWidget(2);
				CUIListScript component = widget.transform.Find("List_Skin").GetComponent<CUIListScript>();
				if (component == null)
				{
					return;
				}
				int selectedIndex = component.GetSelectedIndex();
				uint skinIdByIndex = CSkinInfo.GetSkinIdByIndex(this.m_selectHeroID, selectedIndex);
				uint skinCfgId = CSkinInfo.GetSkinCfgId(this.m_selectHeroID, skinIdByIndex);
				int skinIndexByConfigId = Singleton<CMallSystem>.GetInstance().m_skinMallCtrl.GetSkinIndexByConfigId(skinCfgId);
				if (skinIndexByConfigId > 0)
				{
					int index3 = skinIndexByConfigId - 1;
					ResHeroSkin skinDataByIndex = Singleton<CMallSystem>.GetInstance().m_skinMallCtrl.GetSkinDataByIndex(index3);
					if (skinDataByIndex != null)
					{
						uint num = 0u;
						uint skinId = 0u;
						CSkinInfo.ResolveHeroSkin(skinDataByIndex.dwID, out num, out skinId);
						if (num == 0u)
						{
							return;
						}
						this.m_selectHeroData = CHeroDataFactory.CreateHeroData(num);
						this.SwitchHeroInfo(this.m_selectHeroData.cfgID, skinId, false, false);
						Singleton<CSoundManager>.GetInstance().PostEvent("UI_hero_page", null);
					}
				}
				break;
			}
			}
			this.SetVictoryTipsEvt();
		}

		private void SwitchHeroInfo(uint heroID, uint skinId, bool playAni = true, bool updateSwitchButton = true)
		{
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.HeroInfo_SkillTipClose);
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CHeroInfoSystem2.s_heroInfoFormPath);
			if (form == null)
			{
				return;
			}
			this.m_selectHeroID = heroID;
			if (updateSwitchButton)
			{
				this.UpdateSwitchButton(form);
			}
			this.RefreshHeroInfoForm();
			this.SelectSkinElement(heroID, skinId);
		}

		public void OnHeroInfo_TurnRight(CUIEvent uiEvent)
		{
			switch (this.m_OpenSrc)
			{
			case enHeroFormOpenSrc.HeroListClick:
			{
				int heroIndexByConfigId = Singleton<CHeroOverviewSystem>.GetInstance().GetHeroIndexByConfigId(this.m_selectHeroID);
				if (heroIndexByConfigId < Singleton<CHeroOverviewSystem>.GetInstance().GetHeroListCount() - 1)
				{
					int index = heroIndexByConfigId + 1;
					this.m_selectHeroData = Singleton<CHeroOverviewSystem>.GetInstance().GetHeroDataBuyIndex(index);
					if (this.m_selectHeroData != null)
					{
						uint heroWearSkinId = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GetHeroWearSkinId(this.m_selectHeroData.cfgID);
						this.SwitchHeroInfo(this.m_selectHeroData.cfgID, heroWearSkinId, true, true);
						Singleton<CSoundManager>.GetInstance().PostEvent("UI_hero_page", null);
					}
				}
				break;
			}
			case enHeroFormOpenSrc.HeroBuyClick:
			{
				int heroIndexByConfigId2 = Singleton<CMallSystem>.GetInstance().m_heroMallCtrl.GetHeroIndexByConfigId(this.m_selectHeroID);
				if (heroIndexByConfigId2 < Singleton<CMallSystem>.GetInstance().m_heroMallCtrl.GetHeroListCount() - 1)
				{
					int index2 = heroIndexByConfigId2 + 1;
					this.m_selectHeroData = Singleton<CMallSystem>.GetInstance().m_heroMallCtrl.GetHeroDataByIndex(index2);
					if (this.m_selectHeroData != null)
					{
						uint heroWearSkinId2 = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GetHeroWearSkinId(this.m_selectHeroData.cfgID);
						this.SwitchHeroInfo(this.m_selectHeroData.cfgID, heroWearSkinId2, true, true);
						Singleton<CSoundManager>.GetInstance().PostEvent("UI_hero_page", null);
					}
				}
				break;
			}
			case enHeroFormOpenSrc.SkinBuyClick:
			{
				GameObject widget = this.m_heroInfoForm.GetWidget(2);
				CUIListScript component = widget.transform.Find("List_Skin").GetComponent<CUIListScript>();
				if (component == null)
				{
					return;
				}
				int selectedIndex = component.GetSelectedIndex();
				uint skinIdByIndex = CSkinInfo.GetSkinIdByIndex(this.m_selectHeroID, selectedIndex);
				uint skinCfgId = CSkinInfo.GetSkinCfgId(this.m_selectHeroID, skinIdByIndex);
				int skinIndexByConfigId = Singleton<CMallSystem>.GetInstance().m_skinMallCtrl.GetSkinIndexByConfigId(skinCfgId);
				if (skinIndexByConfigId < Singleton<CMallSystem>.GetInstance().m_skinMallCtrl.GetSkinListCount() - 1)
				{
					int index3 = skinIndexByConfigId + 1;
					ResHeroSkin skinDataByIndex = Singleton<CMallSystem>.GetInstance().m_skinMallCtrl.GetSkinDataByIndex(index3);
					if (skinDataByIndex != null)
					{
						uint num = 0u;
						uint skinId = 0u;
						CSkinInfo.ResolveHeroSkin(skinDataByIndex.dwID, out num, out skinId);
						if (num == 0u)
						{
							return;
						}
						this.m_selectHeroData = CHeroDataFactory.CreateHeroData(num);
						this.SwitchHeroInfo(this.m_selectHeroData.cfgID, skinId, false, false);
						Singleton<CSoundManager>.GetInstance().PostEvent("UI_hero_page", null);
					}
				}
				break;
			}
			}
			this.SetVictoryTipsEvt();
		}

		private void OnHeroInfoGotoRankGod(CUIEvent uiEvent)
		{
			Singleton<RankingSystem>.instance.OpenRankingForm(RankingSystem.RankingType.God, this.m_selectHeroData.cfgID);
		}

		private void OnHeroUnlockPvP(string heroName)
		{
			Singleton<CUIManager>.GetInstance().OpenTips(string.Format("PVP_Hero_Unlock_Tip", true, heroName), false, 1.5f, null, new object[0]);
		}

		private void OnHeroInfo_MenuSelect_Dummy(CUIEvent uiEvent)
		{
			DebugHelper.Assert(true);
		}

		public static void ResetDirectBuyLimit()
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return;
			}
			uint dwConfValue = GameDataMgr.globalInfoDatabin.GetDataByKey(86u).dwConfValue;
			masterRoleInfo.MaterialDirectBuyLimit = (byte)dwConfValue;
			Singleton<EventRouter>.instance.BroadCastEvent("MasterAttributesChanged");
		}

		private void OnHeroInfo_MaterialDirectBuy(CUIEvent uiEvent)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return;
			}
			if (masterRoleInfo.MaterialDirectBuyLimit <= 0)
			{
				Singleton<CUIManager>.GetInstance().OpenTips("Hero_Material_Direct_Buy_Limit_Tip", true, 1.5f, null, new object[0]);
				return;
			}
			string strContent = string.Format(Singleton<CTextManager>.GetInstance().GetText("Hero_Material_Direct_Buy_Tip"), new object[]
			{
				(long)((ulong)uiEvent.m_eventParams.iconUseable.m_dianQuanDirectBuy * (ulong)((long)uiEvent.m_eventParams.iconUseable.m_stackCount)),
				uiEvent.m_eventParams.iconUseable.m_stackCount,
				uiEvent.m_eventParams.iconUseable.m_name,
				masterRoleInfo.MaterialDirectBuyLimit
			});
			Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(strContent, enUIEventID.HeroInfo_Material_Direct_Buy_Confirm, enUIEventID.None, uiEvent.m_eventParams, false);
		}

		private void OnHeroInfo_MaterialDirectBuyConfirm(CUIEvent uiEvent)
		{
			int num = (int)(uiEvent.m_eventParams.iconUseable.m_dianQuanDirectBuy * (uint)uiEvent.m_eventParams.iconUseable.m_stackCount);
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo == null)
			{
				return;
			}
			if ((long)num > (long)masterRoleInfo.DianQuan)
			{
				CUICommonSystem.OpenDianQuanNotEnoughTip();
			}
			else
			{
				CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(4301u);
				cSPkg.stPkgData.stDirectBuyItemReq.bItemType = (byte)uiEvent.m_eventParams.iconUseable.m_type;
				cSPkg.stPkgData.stDirectBuyItemReq.dwItemID = uiEvent.m_eventParams.iconUseable.m_baseID;
				cSPkg.stPkgData.stDirectBuyItemReq.dwItemCnt = (uint)uiEvent.m_eventParams.iconUseable.m_stackCount;
				Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
			}
		}

		private void ColloctMem(uint heroID)
		{
			if (this.m_currentDisplayedHeroID != heroID)
			{
				this.m_currentDisplayedHeroID = heroID;
				if (this.m_selectedHeroIDs.get_Count() >= 3)
				{
					Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharShow");
					Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharSkillIcon");
					Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("ActorInfo");
					Singleton<CResourceManager>.GetInstance().UnloadAssetBundlesByTag("CharLoading");
					Singleton<CResourceManager>.GetInstance().UnloadUnusedAssets();
					this.m_selectedHeroIDs.Clear();
				}
				if (this.m_currentDisplayedHeroID > 0u && !this.m_selectedHeroIDs.Contains(this.m_currentDisplayedHeroID))
				{
					this.m_selectedHeroIDs.Add(this.m_currentDisplayedHeroID);
				}
			}
		}

		public void Refresh3DModel(GameObject root, uint heroID, int skinId, bool bInitAnima = false)
		{
			this.ColloctMem(heroID);
			GameObject gameObject = root.transform.Find(CHeroInfoSystem2.s_heroLeftPanel).gameObject;
			GameObject gameObject2 = gameObject.transform.Find("3DImage").gameObject;
			DebugHelper.Assert(gameObject2 != null);
			if (gameObject2 == null)
			{
				return;
			}
			CUI3DImageScript component = gameObject2.GetComponent<CUI3DImageScript>();
			ObjNameData heroPrefabPath = CUICommonSystem.GetHeroPrefabPath(heroID, skinId, true);
			if (!string.IsNullOrEmpty(this.m_heroModelPath))
			{
				component.RemoveGameObject(this.m_heroModelPath);
			}
			this.m_heroModelPath = heroPrefabPath.ObjectName;
			GameObject gameObject3 = component.AddGameObject(this.m_heroModelPath, false, false);
			CHeroAnimaSystem instance = Singleton<CHeroAnimaSystem>.GetInstance();
			instance.Set3DModel(gameObject3);
			if (gameObject3 == null)
			{
				return;
			}
			gameObject3.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
			if (heroPrefabPath.ActorInfo != null)
			{
				gameObject3.transform.localScale = new Vector3(heroPrefabPath.ActorInfo.LobbyScale, heroPrefabPath.ActorInfo.LobbyScale, heroPrefabPath.ActorInfo.LobbyScale);
			}
			if (bInitAnima)
			{
				instance.InitAnimatList();
				instance.InitAnimatSoundList(this.m_selectHeroID, (uint)skinId);
			}
			this.SetCurModelShowType(false);
		}

		public void Refresh2DImage(uint heroID, int skinId)
		{
			this.ColloctMem(heroID);
			GameObject widget = this.m_heroInfoForm.GetWidget(14);
			Image component = widget.GetComponent<Image>();
			string text = string.Format("30{0}{1:D1}", this.m_selectHeroID, skinId);
			CBinaryObject cBinaryObject = Singleton<CResourceManager>.GetInstance().GetResource("UGUI/Sprite/Dynamic/BustHeroLarge/" + text + ".bytes", typeof(TextAsset), enResourceType.UISprite, false, false).m_content as CBinaryObject;
			if (cBinaryObject == null)
			{
				this.Refresh3DModel(this.m_heroInfoForm.gameObject, heroID, skinId, true);
				Singleton<CHeroAnimaSystem>.GetInstance().OnModePlayAnima("Come");
				return;
			}
			byte[] data = cBinaryObject.m_data;
			Texture2D texture2D = new Texture2D(0, 0, TextureFormat.RGB24, false);
			texture2D.LoadImage(data);
			Sprite sprite = Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), new Vector2(0.5f, 0.5f));
			component.set_sprite(sprite);
			this.m_heroImgPath = text;
			this.SetCurModelShowType(true);
			Singleton<CSoundManager>.GetInstance().PostEvent("Stop_Show", null);
		}

		private void SetCurModelShowType(bool curShow2DImage)
		{
			if (this.m_heroInfoForm == null)
			{
				return;
			}
			this.m_curShow2DImage = curShow2DImage;
			GameObject widget = this.m_heroInfoForm.GetWidget(14);
			GameObject widget2 = this.m_heroInfoForm.GetWidget(15);
			widget.CustomSetActive(this.m_curShow2DImage);
			this.m_heroInfoForm.GetWidget(16).CustomSetActive(!this.m_curShow2DImage);
			this.m_heroInfoForm.GetWidget(17).CustomSetActive(this.m_curShow2DImage);
		}

		private void UpdateSwitchButton(CUIFormScript form)
		{
			if (this.m_heroInfoForm != null)
			{
				GameObject widget = this.m_heroInfoForm.GetWidget(10);
				GameObject widget2 = this.m_heroInfoForm.GetWidget(11);
				switch (this.m_OpenSrc)
				{
				case enHeroFormOpenSrc.HeroListClick:
				{
					int heroIndexByConfigId = Singleton<CHeroOverviewSystem>.GetInstance().GetHeroIndexByConfigId(this.m_selectHeroID);
					widget.CustomSetActive(heroIndexByConfigId > 0);
					widget2.CustomSetActive(heroIndexByConfigId < Singleton<CHeroOverviewSystem>.GetInstance().GetHeroListCount() - 1);
					break;
				}
				case enHeroFormOpenSrc.HeroBuyClick:
				{
					int heroIndexByConfigId2 = Singleton<CMallSystem>.GetInstance().m_heroMallCtrl.GetHeroIndexByConfigId(this.m_selectHeroID);
					widget.CustomSetActive(heroIndexByConfigId2 > 0);
					widget2.CustomSetActive(heroIndexByConfigId2 < Singleton<CMallSystem>.GetInstance().m_heroMallCtrl.GetHeroListCount() - 1);
					break;
				}
				case enHeroFormOpenSrc.SkinBuyClick:
				{
					GameObject widget3 = this.m_heroInfoForm.GetWidget(2);
					CUIListScript component = widget3.transform.Find("List_Skin").GetComponent<CUIListScript>();
					if (component == null)
					{
						return;
					}
					int selectedIndex = component.GetSelectedIndex();
					uint skinIdByIndex = CSkinInfo.GetSkinIdByIndex(this.m_selectHeroID, selectedIndex);
					uint skinCfgId = CSkinInfo.GetSkinCfgId(this.m_selectHeroID, skinIdByIndex);
					int skinIndexByConfigId = Singleton<CMallSystem>.GetInstance().m_skinMallCtrl.GetSkinIndexByConfigId(skinCfgId);
					widget.CustomSetActive(skinIndexByConfigId > 0);
					widget2.CustomSetActive(skinIndexByConfigId < Singleton<CMallSystem>.GetInstance().m_skinMallCtrl.GetSkinListCount() - 1);
					break;
				}
				default:
					widget.CustomSetActive(false);
					widget2.CustomSetActive(false);
					break;
				}
			}
		}

		private void OnOpenCustomEquipPanel(CUIEvent uiEvent)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			DebugHelper.Assert(masterRoleInfo != null, "OnOpenCustomEquipPanel role is null");
			if (masterRoleInfo != null)
			{
				masterRoleInfo.m_rcmdEquipInfo.m_customRecommendEquipsLastChangedHeroID = this.m_selectHeroID;
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.CustomEquip_OpenForm);
			}
		}

		protected void OnViewHeroStory(CUIEvent uiEvent)
		{
			ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(this.m_selectHeroID);
			if (dataByKey != null)
			{
				CUICommonSystem.OpenUrl(dataByKey.szStoryUrl, true, 0);
			}
		}

		private void OnViewHeroProperty(CUIEvent uiEvent)
		{
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CHeroInfoSystem2.s_heroPropertyFormPath, false, true);
			if (cUIFormScript == null)
			{
				return;
			}
			Transform transform = cUIFormScript.transform.Find("Panel/Panel_Proterty").transform;
			GameObject gameObject = transform.Find("Panel_BaseProp").gameObject;
			GameObject gameObject2 = transform.Find("Panel_AtkProp").gameObject;
			GameObject gameObject3 = transform.Find("Panel_DefProp").gameObject;
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			ValueDataInfo[] array = null;
			int level = 1;
			CHeroInfo cHeroInfo;
			if (masterRoleInfo != null && masterRoleInfo.GetHeroInfo(this.m_selectHeroID, out cHeroInfo, false))
			{
				DebugHelper.Assert(cHeroInfo != null && cHeroInfo.mActorValue != null);
				if (cHeroInfo != null && cHeroInfo.mActorValue != null)
				{
					array = cHeroInfo.mActorValue.GetActorValue();
					level = cHeroInfo.mActorValue.SoulLevel;
				}
			}
			else
			{
				ActorMeta actorMeta = default(ActorMeta);
				ActorMeta actorMeta2 = actorMeta;
				actorMeta2.ConfigId = (int)this.m_selectHeroID;
				actorMeta = actorMeta2;
				PropertyHelper propertyHelper = new PropertyHelper();
				propertyHelper.InitValueDataArr(ref actorMeta, true);
				array = propertyHelper.GetActorValue();
				level = propertyHelper.SoulLevel;
			}
			if (array != null)
			{
				CHeroInfoSystem2.RefreshBasePropPanel(gameObject, ref array, level, this.m_selectHeroID);
				CHeroInfoSystem2.RefreshAtkPropPanel(gameObject2, ref array, level, this.m_selectHeroID);
				CHeroInfoSystem2.RefreshDefPropPanel(gameObject3, ref array, level, this.m_selectHeroID);
			}
		}

		private void OnShow2DImage(CUIEvent uiEvent)
		{
			GameObject widget = this.m_heroInfoForm.GetWidget(2);
			CUIListScript component = widget.transform.Find("List_Skin").GetComponent<CUIListScript>();
			int selectedIndex = component.GetSelectedIndex();
			string text = string.Format("30{0}{1:D1}", this.m_selectHeroID, selectedIndex);
			CBinaryObject x = Singleton<CResourceManager>.GetInstance().GetResource("UGUI/Sprite/Dynamic/BustHeroLarge/" + text + ".bytes", typeof(TextAsset), enResourceType.UISprite, false, false).m_content as CBinaryObject;
			if (x == null)
			{
				Singleton<CUIManager>.GetInstance().OpenTips("HeroInfo_2DImageNotComplete", true, 1.5f, null, new object[0]);
			}
			else
			{
				this.Refresh2DImage(this.m_selectHeroID, (int)CSkinInfo.GetSkinIdByIndex(this.m_selectHeroID, selectedIndex));
			}
		}

		private void OnShow3DImage(CUIEvent uiEvent)
		{
			GameObject widget = this.m_heroInfoForm.GetWidget(2);
			CUIListScript component = widget.transform.Find("List_Skin").GetComponent<CUIListScript>();
			int selectedIndex = component.GetSelectedIndex();
			this.Refresh3DModel(uiEvent.m_srcFormScript.gameObject, this.m_selectHeroID, (int)CSkinInfo.GetSkinIdByIndex(this.m_selectHeroID, selectedIndex), true);
			Singleton<CHeroAnimaSystem>.GetInstance().OnModePlayAnima("Come");
		}

		private void OnOpenSkinUrl(CUIEvent uiEvent)
		{
			GameObject widget = this.m_heroInfoForm.GetWidget(2);
			CUIListScript component = widget.transform.Find("List_Skin").GetComponent<CUIListScript>();
			int selectedIndex = component.GetSelectedIndex();
			int skinIdByIndex = (int)CSkinInfo.GetSkinIdByIndex(this.m_selectHeroID, selectedIndex);
			ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(this.m_selectHeroID, (uint)skinIdByIndex);
			if (heroSkin != null && !string.IsNullOrEmpty(heroSkin.szUrl))
			{
				CUICommonSystem.OpenUrl(heroSkin.szUrl, true, 0);
			}
		}

		public void RefreshHeroInfoForm()
		{
			if (this.m_heroInfoForm != null)
			{
				this.RefreshBuyHeroPanel();
				this.RefreshModelBaseInfo();
				this.RefreshExperiencePanel();
				this.RefreshRightPanel();
			}
		}

		private void RefreshExperiencePanel()
		{
			if (null == this.m_heroInfoForm)
			{
				return;
			}
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			GameObject widget = this.m_heroInfoForm.GetWidget(6);
			GameObject widget2 = this.m_heroInfoForm.GetWidget(8);
			widget.CustomSetActive(false);
			widget2.CustomSetActive(false);
			if (masterRoleInfo.IsValidExperienceHero(this.m_selectHeroID))
			{
				CUICommonSystem.RefreshExperienceHeroLeftTime(widget, widget2, this.m_selectHeroID);
			}
		}

		private void RefreshModelBaseInfo()
		{
			if (this.m_heroInfoForm != null)
			{
				GameObject widget = this.m_heroInfoForm.GetWidget(4);
				Text component = widget.transform.Find("heroNameText").GetComponent<Text>();
				Text component2 = widget.transform.Find("heroTitleText").GetComponent<Text>();
				GameObject gameObject = widget.transform.Find("jobImage").gameObject;
				CUICommonSystem.SetHeroJob(this.m_heroInfoForm, gameObject, (enHeroJobType)this.m_selectHeroData.heroType);
				component.set_text(this.m_selectHeroData.heroName);
				if (Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo() == null)
				{
					return;
				}
			}
		}

		private void RefreshRightPanel()
		{
			if (this.m_heroInfoForm != null)
			{
				this.RefreshBaseInfoPanel();
				this.RefreshSkillPanel();
				this.RefreshSkinPanel();
			}
		}

		private void RefreshBaseInfoPanel()
		{
			if (this.m_heroInfoForm != null)
			{
				GameObject widget = this.m_heroInfoForm.GetWidget(0);
				if (null == widget)
				{
					return;
				}
				ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(this.m_selectHeroID);
				Transform transform = widget.transform;
				Text component = transform.Find("jobTitleTxt").GetComponent<Text>();
				component.set_text(CHeroInfo.GetHeroJob(this.m_selectHeroID));
				Text component2 = transform.Find("jobFeatureTxt").GetComponent<Text>();
				component2.set_text(CHeroInfo.GetJobFeature(this.m_selectHeroID));
				bool flag = this.m_selectHeroData.bPlayerOwn && (this.m_selectHeroData.proficiencyLV > 1 || this.m_selectHeroData.proficiency > 0u);
				GameObject gameObject = transform.Find("heroProficiencyImg").gameObject;
				Text component3 = transform.Find("proficiencyLevel").GetComponent<Text>();
				Text component4 = transform.Find("proficiencyValTxt").GetComponent<Text>();
				gameObject.CustomSetActive(flag);
				component3.gameObject.CustomSetActive(flag);
				if (flag)
				{
					ResHeroProficiency heroProficiency = CHeroInfo.GetHeroProficiency(this.m_selectHeroData.heroType, (int)this.m_selectHeroData.proficiencyLV);
					if (heroProficiency != null)
					{
						component3.set_text(heroProficiency.szTitle);
						int maxProficiency = CHeroInfo.GetMaxProficiency();
						if ((int)this.m_selectHeroData.proficiencyLV >= maxProficiency)
						{
							component4.set_text(this.m_selectHeroData.proficiency.ToString());
						}
						else
						{
							component4.set_text(string.Format("{0}/{1}", this.m_selectHeroData.proficiency, heroProficiency.dwTopPoint));
						}
						CUICommonSystem.SetHeroProficiencyIconImage(this.m_heroInfoForm, gameObject, (int)this.m_selectHeroData.proficiencyLV);
					}
				}
				else
				{
					component4.set_text("暂无");
				}
				GameObject gameObject2 = transform.Find("viabilityBar").gameObject;
				Text component5 = transform.Find("viabilityText").GetComponent<Text>();
				component5.set_text(Singleton<CTextManager>.GetInstance().GetText("Hero_Viability"));
				CUICommonSystem.SetProgressBarData(gameObject2, dataByKey.iViability, CHeroInfoSystem2.s_maxBasePropVal, false);
				Text component6 = transform.Find("phyDamageText").GetComponent<Text>();
				component6.set_text(Singleton<CTextManager>.GetInstance().GetText("Hero_AtkDamage"));
				GameObject gameObject3 = transform.Find("phyDamageBar").gameObject;
				CUICommonSystem.SetProgressBarData(gameObject3, dataByKey.iPhyDamage, CHeroInfoSystem2.s_maxBasePropVal, false);
				Text component7 = transform.Find("spellDamageText").GetComponent<Text>();
				component7.set_text(Singleton<CTextManager>.GetInstance().GetText("Hero_SkillFunc"));
				GameObject gameObject4 = transform.Find("spellDamageBar").gameObject;
				CUICommonSystem.SetProgressBarData(gameObject4, dataByKey.iSpellDamage, CHeroInfoSystem2.s_maxBasePropVal, false);
				Text component8 = transform.Find("startedDiffText").GetComponent<Text>();
				component8.set_text(Singleton<CTextManager>.GetInstance().GetText("Hero_StartedDifficulty"));
				GameObject gameObject5 = transform.Find("startedDiffBar").gameObject;
				CUICommonSystem.SetProgressBarData(gameObject5, dataByKey.iStartedDifficulty, CHeroInfoSystem2.s_maxBasePropVal, false);
			}
		}

		private void OnHeroSkillTipOpen(CUIEvent uiEvent)
		{
			if (null == this.m_heroInfoForm)
			{
				return;
			}
			GameObject widget = this.m_heroInfoForm.GetWidget(5);
			GameObject skillPropertyInfoPanel = Utility.FindChild(widget, "SkillPropertyInfo");
			CUICommonSystem.RefreshSkillLevelUpProperty(skillPropertyInfoPanel, ref uiEvent.m_eventParams.skillPropertyDesc, uiEvent.m_eventParams.skillSlotId);
			Text component = widget.transform.Find("skillNameText").GetComponent<Text>();
			component.set_text(uiEvent.m_eventParams.skillTipParam.skillName);
			Text component2 = widget.transform.Find("SkillDescribeText").GetComponent<Text>();
			component2.set_text(uiEvent.m_eventParams.skillTipParam.strTipText);
			Text component3 = widget.transform.Find("SkillCDText").GetComponent<Text>();
			component3.set_text(uiEvent.m_eventParams.skillTipParam.skillCoolDown);
			Text component4 = component3.transform.Find("SkillEnergyCostText").GetComponent<Text>();
			component4.set_text(uiEvent.m_eventParams.skillTipParam.skillEnergyCost);
			ushort[] skillEffect = uiEvent.m_eventParams.skillTipParam.skillEffect;
			for (int i = 1; i <= 2; i++)
			{
				GameObject gameObject = component.transform.Find(string.Format("EffectNode{0}", i)).gameObject;
				if (i <= skillEffect.Length && skillEffect[i - 1] != 0)
				{
					gameObject.CustomSetActive(true);
					gameObject.GetComponent<Image>().SetSprite(CSkillData.GetEffectSlotBg((SkillEffectType)skillEffect[i - 1]), uiEvent.m_srcFormScript, true, false, false, false);
					gameObject.transform.Find("Text").GetComponent<Text>().set_text(CSkillData.GetEffectDesc((SkillEffectType)skillEffect[i - 1]));
				}
				else
				{
					gameObject.CustomSetActive(false);
				}
			}
			widget.CustomSetActive(true);
		}

		private void OnHeroSkillTipClose(CUIEvent uiEvent)
		{
			if (null == this.m_heroInfoForm)
			{
				return;
			}
			GameObject widget = this.m_heroInfoForm.GetWidget(5);
			widget.CustomSetActive(false);
		}

		private void RefreshSkillPanel()
		{
			if (this.m_heroInfoForm != null)
			{
				GameObject widget = this.m_heroInfoForm.GetWidget(1);
				if (null == widget)
				{
					return;
				}
				GameObject gameObject = widget.transform.Find("List").gameObject;
				CUIListScript component = gameObject.GetComponent<CUIListScript>();
				component.SetElementAmount(4);
				for (int i = 0; i < 4; i++)
				{
					CUIListElementScript elemenet = component.GetElemenet(i);
					GameObject gameObject2 = elemenet.gameObject.transform.Find("Item_Skill").gameObject;
					int iSkillID = this.m_selectHeroData.heroCfgInfo.astSkill[i].iSkillID;
					ResSkillCfgInfo skillCfgInfo = CSkillData.GetSkillCfgInfo(iSkillID);
					if (skillCfgInfo == null)
					{
						DebugHelper.Assert(false, "CHeroBaseInfoPanel.RefreshBaseDataPanel(): skillInfo is null, skillId={0}", new object[]
						{
							iSkillID
						});
						break;
					}
					GameObject gameObject3 = gameObject2.transform.Find("skill_Icon").gameObject;
					string prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Skill_Dir, skillCfgInfo.szIconPath);
					CUIUtility.SetImageSprite(gameObject3.GetComponent<Image>(), prefabPath, elemenet.m_belongedFormScript, true, false, false, false);
					CUIEventScript component2 = gameObject2.GetComponent<CUIEventScript>();
					stUIEventParams eventParams = default(stUIEventParams);
					eventParams.skillPropertyDesc = CUICommonSystem.ParseSkillLevelUpProperty(ref skillCfgInfo.astSkillPropertyDescInfo, this.m_selectHeroData.cfgID);
					eventParams.skillSlotId = i;
					eventParams.skillTipParam.skillName = skillCfgInfo.szSkillName;
					eventParams.skillTipParam.strTipText = CUICommonSystem.GetSkillDescLobby(skillCfgInfo.szLobbySkillDesc, this.m_selectHeroData.cfgID);
					eventParams.skillTipParam.skillCoolDown = ((i == 0) ? Singleton<CTextManager>.instance.GetText("Skill_Common_Effect_Type_5") : Singleton<CTextManager>.instance.GetText("Skill_Cool_Down_Tips", new string[]
					{
						CUICommonSystem.ConvertMillisecondToSecondWithOneDecimal(skillCfgInfo.iCoolDown)
					}));
					eventParams.skillTipParam.skillEnergyCost = ((i == 0 || skillCfgInfo.bEnergyCostType == 6) ? string.Empty : Singleton<CTextManager>.instance.GetText(EnergyCommon.GetEnergyShowText((uint)skillCfgInfo.bEnergyCostType, EnergyShowType.CostValue), new string[]
					{
						skillCfgInfo.iEnergyCost.ToString()
					}));
					eventParams.skillTipParam.skillEffect = skillCfgInfo.SkillEffectType;
					component2.SetUIEvent(enUIEventType.Down, enUIEventID.HeroInfo_SkillTipOpen, eventParams);
					component2.SetUIEvent(enUIEventType.HoldEnd, enUIEventID.HeroInfo_SkillTipClose, eventParams);
					component2.SetUIEvent(enUIEventType.Click, enUIEventID.HeroInfo_SkillTipClose, eventParams);
					component2.SetUIEvent(enUIEventType.DragEnd, enUIEventID.HeroInfo_SkillTipClose, eventParams);
				}
				component.SelectElement(0, true);
			}
		}

		private void RefreshSkinFeaturePanel(uint heroId, uint skinId)
		{
			if (this.m_heroInfoForm != null)
			{
				GameObject widget = this.m_heroInfoForm.GetWidget(12);
				GameObject widget2 = this.m_heroInfoForm.GetWidget(0);
				GameObject widget3 = this.m_heroInfoForm.GetWidget(1);
				if (null == widget || null == widget2 || null == widget3)
				{
					return;
				}
				Transform transform = widget.transform.Find("List");
				if (transform != null)
				{
					CUIListScript component = transform.GetComponent<CUIListScript>();
					int num = 0;
					ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(heroId, skinId);
					bool skinFeatureCnt = CSkinInfo.GetSkinFeatureCnt(heroId, skinId, out num);
					widget.gameObject.CustomSetActive(skinFeatureCnt);
					widget2.gameObject.CustomSetActive(!skinFeatureCnt);
					widget3.gameObject.CustomSetActive(!skinFeatureCnt);
					if (skinFeatureCnt)
					{
						component.SetElementAmount(num);
						if (heroSkin != null)
						{
							for (int i = 0; i < num; i++)
							{
								CUIListElementScript elemenet = component.GetElemenet(i);
								Transform transform2 = elemenet.transform.Find("featureImage");
								string prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_SkinFeature_Dir, heroSkin.astFeature[i].szIconPath);
								transform2.GetComponent<Image>().SetSprite(prefabPath, this.m_heroInfoForm, true, false, false, false);
								Transform transform3 = elemenet.transform.Find("featureDescText");
								transform3.GetComponent<Text>().set_text(heroSkin.astFeature[i].szDesc);
							}
						}
					}
					else
					{
						component.SetElementAmount(0);
					}
					GameObject gameObject = Utility.FindChild(widget, "heroItemCell");
					if (!string.IsNullOrEmpty(heroSkin.szUrl) && !CSysDynamicBlock.bLobbyEntryBlocked)
					{
						gameObject.CustomSetActive(true);
						CUICommonSystem.SetHeroItemImage(this.m_heroInfoForm, gameObject, CSkinInfo.GetHeroSkinPic(heroId, skinId), enHeroHeadType.enIcon, false, false);
					}
					else
					{
						gameObject.CustomSetActive(false);
					}
				}
			}
		}

		private void SelectSkinElement(uint heroId, uint skinId)
		{
			if (this.m_heroInfoForm != null)
			{
				GameObject widget = this.m_heroInfoForm.GetWidget(2);
				if (null == widget)
				{
					return;
				}
				int indexBySkinId = CSkinInfo.GetIndexBySkinId(heroId, skinId);
				CUIListScript component = widget.transform.Find("List_Skin").GetComponent<CUIListScript>();
				component.SelectElement(-1, false);
				component.SelectElement(indexBySkinId, true);
			}
		}

		private void OnHeroSkin_ItemEnable(CUIEvent uiEvent)
		{
			if (this.m_heroInfoForm != null)
			{
				GameObject widget = this.m_heroInfoForm.GetWidget(2);
				if (null == widget)
				{
					return;
				}
				int heroSkinCnt = CSkinInfo.GetHeroSkinCnt(this.m_selectHeroID);
				int srcWidgetIndexInBelongedList = uiEvent.m_srcWidgetIndexInBelongedList;
				if (srcWidgetIndexInBelongedList < 0 || srcWidgetIndexInBelongedList >= heroSkinCnt)
				{
					return;
				}
				ListView<ResHeroSkin> availableSkinByHeroId = CSkinInfo.GetAvailableSkinByHeroId(this.m_selectHeroID);
				CUIListScript component = widget.transform.Find("List_Skin").GetComponent<CUIListScript>();
				int selectedIndex = component.GetSelectedIndex();
				this.SetSkinListItem(uiEvent.m_srcWidget, availableSkinByHeroId[srcWidgetIndexInBelongedList], srcWidgetIndexInBelongedList == selectedIndex);
			}
		}

		private void SetVictoryTipsEvt()
		{
			if (this.m_heroInfoForm == null)
			{
				return;
			}
			GameObject widget = this.m_heroInfoForm.GetWidget(13);
			if (CBattleGuideManager.EnableHeroVictoryTips() && !CSysDynamicBlock.bLobbyEntryBlocked)
			{
				widget.CustomSetActive(true);
				Transform transform = widget.transform;
				CUIEventScript component = transform.FindChild("Btn").GetComponent<CUIEventScript>();
				ulong num = 0uL;
				CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
				if (masterRoleInfo != null)
				{
					num = masterRoleInfo.playerUllUID;
				}
				string platformArea = CUICommonSystem.GetPlatformArea();
				component.m_onClickEventParams.tagStr = Singleton<CTextManager>.GetInstance().GetText("WinTrick_Url_Hero", new string[]
				{
					this.m_selectHeroID.ToString(),
					this.m_selectHeroID.ToString(),
					MonoSingleton<TdirMgr>.GetInstance().SelectedTdir.logicWorldID.ToString(),
					num.ToString(),
					platformArea.ToString()
				});
				ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(this.m_selectHeroID);
				string text;
				if (dataByKey != null)
				{
					text = dataByKey.szName;
				}
				else
				{
					text = Singleton<CTextManager>.GetInstance().GetText("WinTrick_Tips_DefaultHeroName");
				}
				if (masterRoleInfo != null)
				{
					if (!masterRoleInfo.IsClientBitsSet(3))
					{
						CUICommonSystem.SetObjActive(transform.FindChild("Panel_Guide"), true);
						CUICommonSystem.SetTextContent(transform.FindChild("Panel_Guide/Text"), Singleton<CTextManager>.GetInstance().GetText("WinTrick_Tips_text", new string[]
						{
							text
						}));
						masterRoleInfo.SetClientBits(3, true, true);
					}
					else
					{
						CUICommonSystem.SetObjActive(transform.FindChild("Panel_Guide"), false);
					}
				}
			}
			else
			{
				widget.CustomSetActive(false);
			}
		}

		private void RefreshSkinPanel()
		{
			if (this.m_heroInfoForm != null)
			{
				GameObject widget = this.m_heroInfoForm.GetWidget(2);
				if (null == widget)
				{
					return;
				}
				CUIListScript component = widget.transform.Find("List_Skin").GetComponent<CUIListScript>();
				ListView<ResHeroSkin> availableSkinByHeroId = CSkinInfo.GetAvailableSkinByHeroId(this.m_selectHeroID);
				uint heroWearSkinId = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo().GetHeroWearSkinId(this.m_selectHeroID);
				int heroSkinCnt = CSkinInfo.GetHeroSkinCnt(this.m_selectHeroID);
				component.SetElementAmount(heroSkinCnt);
			}
		}

		protected void SetSkinListItem(GameObject listItem, ResHeroSkin skinInfo, bool bSelect)
		{
			if (listItem == null || skinInfo == null)
			{
				return;
			}
			Transform transform = listItem.transform.Find("skinItem");
			Text component = transform.Find("skinInfoPanel/skinNamePanel/skinNameText").GetComponent<Text>();
			component.set_text(StringHelper.UTF8BytesToString(ref skinInfo.szSkinName));
			Text component2 = transform.Find("skinInfoPanel/skinNamePanel/heroNameText").GetComponent<Text>();
			component2.set_text(skinInfo.szHeroName);
			Text component3 = transform.Find("skinStateText").GetComponent<Text>();
			component3.set_text(string.Empty);
			Transform transform2 = transform.Find("skinLabelImage");
			CUICommonSystem.SetHeroSkinLabelPic(this.m_heroInfoForm, transform2.gameObject, skinInfo.dwHeroID, skinInfo.dwSkinID);
			Image component4 = transform.Find("skinIconImage").GetComponent<Image>();
			string prefabPath = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_BustHero_Dir, StringHelper.UTF8BytesToString(ref skinInfo.szSkinPicID));
			component4.SetSprite(prefabPath, this.m_heroInfoForm, true, false, false, true);
			GameObject gameObject = transform.Find("buyButton").gameObject;
			GameObject gameObject2 = transform.Find("wearButton").gameObject;
			gameObject.CustomSetActive(false);
			gameObject2.CustomSetActive(false);
			Text component5 = gameObject.transform.Find("Text").GetComponent<Text>();
			component5.set_text(Singleton<CTextManager>.GetInstance().GetText("HeroInfo_BuyAndWear"));
			GameObject gameObject3 = transform.Find("skinInfoPanel/pricePanel").gameObject;
			gameObject3.CustomSetActive(false);
			GameObject gameObject4 = transform.Find("txtSkinLeftTime").gameObject;
			GameObject gameObject5 = transform.Find("txtSkinLeftTime/Timer").gameObject;
			CUICommonSystem.RefreshExperienceSkinLeftTime(gameObject4, gameObject5, skinInfo.dwHeroID, skinInfo.dwSkinID, Singleton<CTextManager>.GetInstance().GetText("ExpCard_ExpTime"), true);
			CTextManager instance = Singleton<CTextManager>.GetInstance();
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			switch (masterRoleInfo.GetHeroSkinState(this.m_selectHeroID, skinInfo.dwSkinID))
			{
			case HeroSkinState.NormalHero_NormalSkin_Wear:
			case HeroSkinState.LimitHero_NormalSkin_Wear:
				this.SetSkinStateText(component3, instance.GetText("Hero_SkinState_Wear"));
				break;
			case HeroSkinState.NormalHero_LimitSkin_Wear:
				if (CSkinInfo.IsCanBuy(skinInfo.dwHeroID, skinInfo.dwSkinID))
				{
					this.SetBuyBtn(gameObject, skinInfo);
					this.SetSkinStateText(component3, instance.GetText("Avatar_hero_buybutton"));
				}
				else
				{
					this.SetSkinStateText(component3, CHeroInfoSystem2.GetSkinCannotBuyStr(skinInfo));
				}
				break;
			case HeroSkinState.NormalHero_NormalSkin_Own:
			case HeroSkinState.LimitHero_NormalSkin_Own:
				if (bSelect)
				{
					this.SetWearBtn(gameObject2, skinInfo);
				}
				this.SetSkinStateText(component3, instance.GetText("Hero_SkinState_Own"));
				break;
			case HeroSkinState.NormalHero_LimitSkin_Own:
				if (bSelect)
				{
					this.SetWearBtn(gameObject2, skinInfo);
				}
				if (CSkinInfo.IsCanBuy(skinInfo.dwHeroID, skinInfo.dwSkinID))
				{
					this.SetPricePanel(gameObject3, skinInfo);
				}
				else
				{
					this.SetSkinStateText(component3, CHeroInfoSystem2.GetSkinCannotBuyStr(skinInfo));
				}
				break;
			case HeroSkinState.NormalHero_Skin_NotOwn:
				if (CSkinInfo.IsCanBuy(skinInfo.dwHeroID, skinInfo.dwSkinID))
				{
					if (bSelect)
					{
						this.SetBuyBtn(gameObject, skinInfo);
					}
					this.SetPricePanel(gameObject3, skinInfo);
				}
				else
				{
					this.SetSkinStateText(component3, CHeroInfoSystem2.GetSkinCannotBuyStr(skinInfo));
				}
				break;
			case HeroSkinState.LimitHero_LimitSkin_Wear:
				this.SetSkinStateText(component3, instance.GetText("ExpCard_In_Experience"));
				break;
			case HeroSkinState.LimitHero_LimitSkin_Own:
				if (bSelect)
				{
					this.SetWearBtn(gameObject2, skinInfo);
				}
				if (CSkinInfo.IsCanBuy(skinInfo.dwHeroID, skinInfo.dwSkinID))
				{
					this.SetSkinStateText(component3, instance.GetText("Hero_SkinState_UnUsable"));
					this.SetPricePanel(gameObject3, skinInfo);
				}
				break;
			case HeroSkinState.LimitHero_Skin_NotOwn:
				if (CSkinInfo.IsCanBuy(skinInfo.dwHeroID, skinInfo.dwSkinID))
				{
					this.SetSkinStateText(component3, instance.GetText("Hero_SkinState_UnUsable"));
					this.SetPricePanel(gameObject3, skinInfo);
				}
				else
				{
					this.SetSkinStateText(component3, CHeroInfoSystem2.GetSkinCannotBuyStr(skinInfo));
				}
				break;
			case HeroSkinState.NoHero_Skin_Wear:
				this.SetSkinStateText(component3, instance.GetText("Hero_SkinState_UnUsable"));
				break;
			case HeroSkinState.NoHero_NormalSkin_Own:
				this.SetSkinStateText(component3, instance.GetText("Hero_SkinState_Own"));
				break;
			case HeroSkinState.NoHero_LimitSkin_Own:
			case HeroSkinState.NoHero_Skin_NotOwn:
				if (CSkinInfo.IsCanBuy(skinInfo.dwHeroID, skinInfo.dwSkinID))
				{
					this.SetSkinStateText(component3, instance.GetText("Hero_SkinState_UnUsable"));
					this.SetPricePanel(gameObject3, skinInfo);
				}
				else
				{
					this.SetSkinStateText(component3, CHeroInfoSystem2.GetSkinCannotBuyStr(skinInfo));
				}
				break;
			}
		}

		private void SetWearBtn(GameObject wearBtnObj, ResHeroSkin skinInfo)
		{
			wearBtnObj.CustomSetActive(true);
			CUIEventScript component = wearBtnObj.GetComponent<CUIEventScript>();
			stUIEventParams eventParams = default(stUIEventParams);
			eventParams.heroSkinParam.heroId = skinInfo.dwHeroID;
			eventParams.heroSkinParam.skinId = skinInfo.dwSkinID;
			component.SetUIEvent(enUIEventType.Click, enUIEventID.HeroSkin_Wear, eventParams);
		}

		private void SetBuyBtn(GameObject buyBtnObj, ResHeroSkin skinInfo)
		{
			buyBtnObj.CustomSetActive(true);
			CUIEventScript component = buyBtnObj.GetComponent<CUIEventScript>();
			stUIEventParams eventParams = default(stUIEventParams);
			eventParams.heroSkinParam.heroId = skinInfo.dwHeroID;
			eventParams.heroSkinParam.skinId = skinInfo.dwSkinID;
			eventParams.heroSkinParam.isCanCharge = true;
			component.SetUIEvent(enUIEventType.Click, enUIEventID.HeroSkin_OpenBuySkinForm, eventParams);
		}

		private void SetPricePanel(GameObject pricePanelObj, ResHeroSkin skinInfo)
		{
			pricePanelObj.CustomSetActive(true);
			stPayInfoSet skinPayInfoSet = CSkinInfo.GetSkinPayInfoSet(skinInfo.dwHeroID, skinInfo.dwSkinID);
			this.RefreshHeroSkinPrice(this.m_heroInfoForm, pricePanelObj.transform, skinInfo.dwSkinID, ref skinPayInfoSet);
		}

		private void SetSkinStateText(Text skinStateText, string content)
		{
			skinStateText.set_text(content);
		}

		public static string GetSkinCannotBuyStr(ResHeroSkin skinInfo)
		{
			ResHeroSkinShop resHeroSkinShop = null;
			GameDataMgr.skinShopInfoDict.TryGetValue(skinInfo.dwID, out resHeroSkinShop);
			if (resHeroSkinShop != null)
			{
				return string.IsNullOrEmpty(resHeroSkinShop.szGetPath) ? Singleton<CTextManager>.GetInstance().GetText("Hero_SkinState_CannotBuy") : resHeroSkinShop.szGetPath;
			}
			return null;
		}

		public void OnHeroSkin_ItemSelect(CUIEvent uiEvent)
		{
			if (null == this.m_heroInfoForm)
			{
				return;
			}
			GameObject widget = this.m_heroInfoForm.GetWidget(2);
			CUIListScript component = widget.transform.Find("List_Skin").GetComponent<CUIListScript>();
			CUIListElementScript lastSelectedElement = component.GetLastSelectedElement();
			CUIListElementScript selectedElement = component.GetSelectedElement();
			int lastSelectedIndex = component.GetLastSelectedIndex();
			ResHeroSkin heroSkin = CSkinInfo.GetHeroSkin(this.m_selectHeroID, CSkinInfo.GetSkinIdByIndex(this.m_selectHeroID, lastSelectedIndex));
			if (lastSelectedElement != null)
			{
				this.SetSkinListItem(lastSelectedElement.gameObject, heroSkin, false);
			}
			int selectedIndex = component.GetSelectedIndex();
			uint skinIdByIndex = CSkinInfo.GetSkinIdByIndex(this.m_selectHeroID, selectedIndex);
			heroSkin = CSkinInfo.GetHeroSkin(this.m_selectHeroID, skinIdByIndex);
			if (selectedElement != null)
			{
				this.SetSkinListItem(selectedElement.gameObject, heroSkin, true);
			}
			this.RefreshSkinFeaturePanel(this.m_selectHeroID, skinIdByIndex);
			if (this.m_curShow2DImage)
			{
				this.Refresh2DImage(this.m_selectHeroID, (int)skinIdByIndex);
			}
			else
			{
				this.Refresh3DModel(uiEvent.m_srcFormScript.gameObject, this.m_selectHeroID, (int)skinIdByIndex, true);
				Singleton<CHeroAnimaSystem>.GetInstance().OnModePlayAnima("Come");
			}
			this.UpdateSwitchButton(this.m_heroInfoForm);
			GameObject gameObject = this.m_heroInfoForm.transform.Find(CHeroInfoSystem2.s_heroLeftPanel).gameObject;
			Text component2 = gameObject.transform.Find("heroInfoPanel/heroTitleText").GetComponent<Text>();
			component2.set_text(CHeroInfo.GetHeroTitle(this.m_selectHeroID, skinIdByIndex));
			GameObject widget2 = this.m_heroInfoForm.GetWidget(19);
		}

		public void OnHeroSkin_Wear(CUIEvent uiEvent)
		{
			uint heroId = uiEvent.m_eventParams.heroSkinParam.heroId;
			uint skinId = uiEvent.m_eventParams.heroSkinParam.skinId;
			CHeroInfoSystem2.ReqWearHeroSkin(heroId, skinId, false);
		}

		public static void ReqWearHeroSkin(uint heroId, uint skinId, bool isSendGameSvr = false)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1821u);
			cSPkg.stPkgData.stWearHeroSkinReq.dwHeroID = heroId;
			cSPkg.stPkgData.stWearHeroSkinReq.dwSkinID = skinId;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, true);
		}

		private void RefreshHeroSkinPrice(CUIFormScript formScript, Transform pricePanelTransform, uint skinId, ref stPayInfoSet payInfoSet)
		{
			if (pricePanelTransform == null)
			{
				return;
			}
			CMallSystem.SetSkinBuyPricePanel(formScript, pricePanelTransform, ref payInfoSet.m_payInfos[0]);
		}

		private void SetWearExperienceSkinButton(CRoleInfo role, ResHeroSkin skinInfo, GameObject experienceBtnObj)
		{
			if (role.IsValidExperienceSkin(skinInfo.dwHeroID, skinInfo.dwSkinID))
			{
				experienceBtnObj.CustomSetActive(true);
				CUIEventScript component = experienceBtnObj.GetComponent<CUIEventScript>();
				stUIEventParams eventParams = default(stUIEventParams);
				eventParams.heroSkinParam.heroId = skinInfo.dwHeroID;
				eventParams.heroSkinParam.skinId = skinInfo.dwSkinID;
				component.SetUIEvent(enUIEventType.Click, enUIEventID.HeroSkin_Wear, eventParams);
			}
		}

		public void OnNtyAddHero(uint id)
		{
			if (this.m_selectHeroID != 0u)
			{
				this.m_selectHeroData = CHeroDataFactory.CreateHeroData(this.m_selectHeroID);
			}
			this.RefreshHeroInfoForm();
		}

		private void OnAddHeroSkin(uint heroId, uint skinId, uint addReason)
		{
			if (addReason == 1u && !Singleton<CHeroSelectBaseSystem>.instance.m_isInHeroSelectState)
			{
				CUICommonSystem.ShowNewHeroOrSkin(heroId, skinId, enUIEventID.None, true, COM_REWARDS_TYPE.COM_REWARDS_TYPE_SKIN, false, null, enFormPriority.Priority1, 0u, 0);
			}
			if (this.m_heroInfoForm == null)
			{
				return;
			}
			if (heroId == this.m_selectHeroID)
			{
				this.RefreshHeroInfoForm();
			}
		}

		public void OnHeroSkinWearSuc(uint heroId, uint skinId)
		{
			if (this.m_heroInfoForm != null && heroId == this.m_selectHeroID)
			{
				this.RefreshSkinPanel();
			}
		}

		public void OnHeroSkinBuySuc(uint heroId)
		{
			if (this.m_heroInfoForm != null && heroId == this.m_selectHeroID)
			{
				this.RefreshSkinPanel();
			}
		}

		private static string GetFormStr(float baseValue, float growValue)
		{
			if (growValue > 0f)
			{
				return string.Format(CHeroInfoSystem2.valForm1, baseValue + growValue, baseValue, growValue);
			}
			return baseValue.ToString();
		}

		private static string GetFormPercentStr(int percent, bool isExtra)
		{
			if (isExtra)
			{
				return string.Format(CHeroInfoSystem2.valForm2, CUICommonSystem.GetValuePercent(percent));
			}
			return CUICommonSystem.GetValuePercent(percent);
		}

		public static void RefreshBasePropPanel(GameObject root, ref ValueDataInfo[] info, int level, uint heroId)
		{
			Transform transform = root.transform;
			Text component = transform.Find("TextL1").GetComponent<Text>();
			Text component2 = transform.Find("TextR1").GetComponent<Text>();
			Text component3 = transform.Find("TextL2").GetComponent<Text>();
			Text component4 = transform.Find("TextR2").GetComponent<Text>();
			Text component5 = transform.Find("TextL3").GetComponent<Text>();
			Text component6 = transform.Find("TextR3").GetComponent<Text>();
			Text component7 = transform.Find("TextL4").GetComponent<Text>();
			Text component8 = transform.Find("TextR4").GetComponent<Text>();
			Text component9 = transform.Find("TextL5").GetComponent<Text>();
			Text component10 = transform.Find("TextR5").GetComponent<Text>();
			Text component11 = transform.Find("TextL6").GetComponent<Text>();
			Text component12 = transform.Find("TextR6").GetComponent<Text>();
			Text component13 = transform.Find("TextL7").GetComponent<Text>();
			Text component14 = transform.Find("TextR7").GetComponent<Text>();
			Text component15 = transform.Find("TextL8").GetComponent<Text>();
			Text component16 = transform.Find("TextR8").GetComponent<Text>();
			component.set_text(Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_MaxHp"));
			component2.set_text(CHeroInfoSystem2.GetFormStr((float)info[5].basePropertyValue, (float)info[5].extraPropertyValue));
			component3.set_text(Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_PhyAtkPt"));
			component4.set_text(CHeroInfoSystem2.GetFormStr((float)info[1].basePropertyValue, (float)info[1].extraPropertyValue));
			component5.set_text(Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_MgcAtkPt"));
			component6.set_text(CHeroInfoSystem2.GetFormStr((float)info[2].basePropertyValue, (float)info[2].extraPropertyValue));
			ResBattleParam anyData = GameDataMgr.battleParam.GetAnyData();
			int totalValue = info[3].totalValue;
			int percent = totalValue * 10000 / (totalValue + level * (int)anyData.dwM_PhysicsDefend + (int)anyData.dwN_PhysicsDefend);
			component7.set_text(Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_PhyDefPt"));
			component8.set_text(string.Format("{0}|{1}", CHeroInfoSystem2.GetFormStr((float)info[3].basePropertyValue, (float)info[3].extraPropertyValue), CHeroInfoSystem2.GetFormPercentStr(percent, info[3].extraPropertyValue > 0)));
			totalValue = info[4].totalValue;
			percent = totalValue * 10000 / (totalValue + level * (int)anyData.dwM_MagicDefend + (int)anyData.dwN_MagicDefend);
			component9.set_text(Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_MgcDefPt"));
			component10.set_text(string.Format("{0}|{1}", CHeroInfoSystem2.GetFormStr((float)info[4].basePropertyValue, (float)info[4].extraPropertyValue), CHeroInfoSystem2.GetFormPercentStr(percent, info[4].extraPropertyValue > 0)));
			ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(heroId);
			uint energyType = (dataByKey == null) ? 0u : dataByKey.dwEnergyType;
			component15.set_text(Singleton<CTextManager>.GetInstance().GetText(EnergyCommon.GetEnergyShowText(energyType, EnergyShowType.MaxValue)));
			component16.set_text(CHeroInfoSystem2.GetFormStr((float)info[32].basePropertyValue, (float)info[32].extraPropertyValue));
		}

		public static void RefreshAtkPropPanel(GameObject root, ref ValueDataInfo[] info, int level, uint heroId)
		{
			Transform transform = root.transform;
			Text component = transform.Find("TextL1").GetComponent<Text>();
			Text component2 = transform.Find("TextR1").GetComponent<Text>();
			Text component3 = transform.Find("TextL2").GetComponent<Text>();
			Text component4 = transform.Find("TextR2").GetComponent<Text>();
			Text component5 = transform.Find("TextL3").GetComponent<Text>();
			Text component6 = transform.Find("TextR3").GetComponent<Text>();
			Text component7 = transform.Find("TextL4").GetComponent<Text>();
			Text component8 = transform.Find("TextR4").GetComponent<Text>();
			Text component9 = transform.Find("TextL5").GetComponent<Text>();
			Text component10 = transform.Find("TextR5").GetComponent<Text>();
			Text component11 = transform.Find("TextL6").GetComponent<Text>();
			Text component12 = transform.Find("TextR6").GetComponent<Text>();
			Text component13 = transform.Find("TextL7").GetComponent<Text>();
			Text component14 = transform.Find("TextR7").GetComponent<Text>();
			Text component15 = transform.Find("TextL8").GetComponent<Text>();
			Text component16 = transform.Find("TextR8").GetComponent<Text>();
			Text component17 = transform.Find("TextL9").GetComponent<Text>();
			Text component18 = transform.Find("TextR9").GetComponent<Text>();
			Text component19 = transform.Find("TextL10").GetComponent<Text>();
			Text component20 = transform.Find("TextR10").GetComponent<Text>();
			ResBattleParam anyData = GameDataMgr.battleParam.GetAnyData();
			component.set_text(Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_MoveSpd"));
			MonoSingleton<GlobalConfig>.instance.bOnExternalSpeedPicker = true;
			component2.set_text(CHeroInfoSystem2.GetFormStr((float)(info[15].basePropertyValue / 10), (float)(info[15].extraPropertyValue / 10)));
			MonoSingleton<GlobalConfig>.instance.bOnExternalSpeedPicker = false;
			component3.set_text(Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_PhyArmorHurt"));
			component4.set_text(string.Format("{0}|{1}", CHeroInfoSystem2.GetFormStr((float)info[7].baseValue, (float)info[7].extraPropertyValue), CHeroInfoSystem2.GetFormPercentStr(info[34].totalValue, info[34].extraPropertyValue > 0)));
			component5.set_text(Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_MgcArmorHurt"));
			component6.set_text(string.Format("{0}|{1}", CHeroInfoSystem2.GetFormStr((float)info[8].baseValue, (float)info[8].extraPropertyValue), CHeroInfoSystem2.GetFormPercentStr(info[35].totalValue, info[35].extraPropertyValue > 0)));
			int totalValue = info[28].totalValue;
			int percent = 10000 * totalValue / (totalValue + level * (int)anyData.dwM_AttackSpeed + (int)anyData.dwN_AttackSpeed) + info[18].totalValue;
			component7.set_text(Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_AtkSpdLvl"));
			component8.set_text(CHeroInfoSystem2.GetFormPercentStr(percent, info[18].extraPropertyValue > 0));
			totalValue = info[24].totalValue;
			percent = 10000 * totalValue / (totalValue + level * (int)anyData.dwM_Critical + (int)anyData.dwN_Critical) + info[6].totalValue;
			component9.set_text(Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_CritLvl"));
			component10.set_text(CHeroInfoSystem2.GetFormPercentStr(percent, info[6].extraPropertyValue > 0));
			totalValue = info[12].totalValue;
			percent = totalValue + 10000;
			component11.set_text(Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_CritEft"));
			component12.set_text(CHeroInfoSystem2.GetFormPercentStr(percent, info[12].extraPropertyValue > 0));
			totalValue = info[26].totalValue;
			percent = 10000 * totalValue / (totalValue + level * (int)anyData.dwM_PhysicsHemophagia + (int)anyData.dwN_PhysicsHemophagia) + info[9].totalValue;
			component13.set_text(Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_PhyVampLvl"));
			component14.set_text(CHeroInfoSystem2.GetFormPercentStr(percent, info[9].extraPropertyValue > 0));
			totalValue = info[27].totalValue;
			percent = 10000 * totalValue / (totalValue + level * (int)anyData.dwM_MagicHemophagia + (int)anyData.dwN_MagicHemophagia) + info[10].totalValue;
			component15.set_text(Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_MgcVampLvl"));
			component16.set_text(CHeroInfoSystem2.GetFormPercentStr(percent, info[10].extraPropertyValue > 0));
			component17.set_text(Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_CdReduce"));
			percent = info[20].totalValue;
			component18.set_text(CHeroInfoSystem2.GetFormPercentStr(percent, info[20].extraPropertyValue > 0));
			ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(heroId);
			if (dataByKey != null)
			{
				component19.set_text(Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_BaseAtkRange"));
				component20.set_text(Utility.UTF8Convert(dataByKey.szAttackRangeDesc));
			}
			else
			{
				component19.set_text(string.Empty);
				component20.set_text(string.Empty);
			}
		}

		public static void RefreshDefPropPanel(GameObject root, ref ValueDataInfo[] info, int level, uint heroId)
		{
			Transform transform = root.transform;
			Text component = transform.Find("TextL1").GetComponent<Text>();
			Text component2 = transform.Find("TextR1").GetComponent<Text>();
			Text component3 = transform.Find("TextL2").GetComponent<Text>();
			Text component4 = transform.Find("TextR2").GetComponent<Text>();
			Text component5 = transform.Find("TextL3").GetComponent<Text>();
			Text component6 = transform.Find("TextR3").GetComponent<Text>();
			ResBattleParam anyData = GameDataMgr.battleParam.GetAnyData();
			component.set_text(Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_CtrlReduceLvl"));
			int totalValue = info[29].totalValue;
			int percent = 10000 * totalValue / (totalValue + level * (int)anyData.dwM_Tenacity + (int)anyData.dwN_Tenacity) + info[17].totalValue;
			component2.set_text(CHeroInfoSystem2.GetFormPercentStr(percent, info[17].extraPropertyValue > 0));
			component3.set_text(Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_HpRecover"));
			totalValue = info[16].totalValue;
			string text = string.Format(Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_HpRecover_Desc"), totalValue);
			component4.set_text(CHeroInfoSystem2.GetFormStr((float)info[16].basePropertyValue, (float)info[16].extraPropertyValue));
			ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(heroId);
			uint energyType = (dataByKey == null) ? 0u : dataByKey.dwEnergyType;
			component5.set_text(Singleton<CTextManager>.GetInstance().GetText(EnergyCommon.GetEnergyShowText(energyType, EnergyShowType.RecoverValue)));
			totalValue = info[33].totalValue;
			string text2 = string.Format(Singleton<CTextManager>.GetInstance().GetText("Hero_Prop_EpRecover_Desc"), totalValue);
			component6.set_text(CHeroInfoSystem2.GetFormStr((float)info[33].basePropertyValue, (float)info[33].extraPropertyValue));
		}

		private void RefreshBuyHeroPanel()
		{
			if (null == this.m_heroInfoForm)
			{
				return;
			}
			GameObject widget = this.m_heroInfoForm.GetWidget(3);
			bool bPlayerOwn = this.m_selectHeroData.bPlayerOwn;
			widget.CustomSetActive(!bPlayerOwn);
			if (bPlayerOwn)
			{
				return;
			}
			ResHeroCfgInfo dataByKey = GameDataMgr.heroDatabin.GetDataByKey(this.m_selectHeroID);
			DebugHelper.Assert(dataByKey != null, "Failed Find Hero Cfg {0}", new object[]
			{
				this.m_selectHeroID
			});
			if (dataByKey == null)
			{
				return;
			}
			ResHeroShop resHeroShop = null;
			GameDataMgr.heroShopInfoDict.TryGetValue(dataByKey.dwCfgID, out resHeroShop);
			if (CSysDynamicBlock.bLobbyEntryBlocked && resHeroShop != null)
			{
				resHeroShop.bIsBuyCoin = 1;
				resHeroShop.bIsBuyCoupons = 1;
			}
			GameObject gameObject = widget.transform.Find("buyBtn").gameObject;
			gameObject.CustomSetActive(true);
			Text component = widget.transform.Find("getWayText").GetComponent<Text>();
			component.gameObject.CustomSetActive(false);
			IHeroData heroData = CHeroDataFactory.CreateHeroData(this.m_selectHeroID);
			ResHeroPromotion resPromotion = heroData.promotion();
			stPayInfoSet stPayInfoSet = default(stPayInfoSet);
			stPayInfoSet = CMallSystem.GetPayInfoSetOfGood(dataByKey, resPromotion);
			CHeroSkinBuyManager.SetHeroBuyPricePanel(this.m_heroInfoForm, widget, ref stPayInfoSet, this.m_selectHeroID, enUIEventID.None);
			if (stPayInfoSet.m_payInfoCount <= 0)
			{
				gameObject.CustomSetActive(false);
				component.gameObject.CustomSetActive(true);
				if (resHeroShop != null)
				{
					component.set_text(StringHelper.UTF8BytesToString(ref resHeroShop.szObtWay));
				}
				else
				{
					component.set_text(null);
				}
			}
			if (gameObject.activeSelf)
			{
				CUIEventScript component2 = gameObject.GetComponent<CUIEventScript>();
				component2.SetUIEvent(enUIEventType.Click, enUIEventID.HeroInfo_OpenBuyHeroForm, new stUIEventParams
				{
					heroId = this.m_selectHeroID
				});
			}
		}

		[MessageHandler(4302)]
		public static void ReceiveMaterialDirectBuy(CSPkg msg)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			if (masterRoleInfo != null)
			{
				uint dwConfValue = GameDataMgr.globalInfoDatabin.GetDataByKey(86u).dwConfValue;
				masterRoleInfo.MaterialDirectBuyLimit = (byte)(dwConfValue - msg.stPkgData.stDirectBuyItemRsp.dwDirectBuyItemCnt);
			}
		}

		[MessageHandler(1822)]
		public static void OnWearHeroSkinRsp(CSPkg msg)
		{
			Singleton<CUIManager>.GetInstance().CloseSendMsgAlert();
			SCPKG_WEARHEROSKIN_RSP stWearHeroSkinRsp = msg.stPkgData.stWearHeroSkinRsp;
			if (stWearHeroSkinRsp.iResult == 0)
			{
				CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
				masterRoleInfo.OnWearHeroSkin(stWearHeroSkinRsp.dwHeroID, stWearHeroSkinRsp.dwSkinID);
				Singleton<CHeroInfoSystem2>.GetInstance().OnHeroSkinWearSuc(stWearHeroSkinRsp.dwHeroID, stWearHeroSkinRsp.dwSkinID);
				Singleton<CHeroSelectBaseSystem>.GetInstance().OnHeroSkinWearSuc(stWearHeroSkinRsp.dwHeroID, stWearHeroSkinRsp.dwSkinID);
			}
			else
			{
				CS_WEARHEROSKIN_ERRCODE iResult = (CS_WEARHEROSKIN_ERRCODE)stWearHeroSkinRsp.iResult;
				CTextManager instance = Singleton<CTextManager>.GetInstance();
				CS_WEARHEROSKIN_ERRCODE cS_WEARHEROSKIN_ERRCODE = iResult;
				if (cS_WEARHEROSKIN_ERRCODE != CS_WEARHEROSKIN_ERRCODE.CS_WEARHEROSKIN_NOOWNEDHERO)
				{
					if (cS_WEARHEROSKIN_ERRCODE == CS_WEARHEROSKIN_ERRCODE.CS_WEARHEROSKIN_NOOWNEDSKIN)
					{
						Singleton<CUIManager>.GetInstance().OpenTips(instance.GetText("Hero_SkinWear_Skin_Not_Exist"), false, 1.5f, null, new object[0]);
					}
				}
				else
				{
					Singleton<CUIManager>.GetInstance().OpenTips(instance.GetText("Hero_SkinWear_Hero_Not_Exist"), false, 1.5f, null, new object[0]);
				}
			}
		}
	}
}
