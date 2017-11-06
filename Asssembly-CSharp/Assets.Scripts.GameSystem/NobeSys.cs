using Apollo;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	[MessageHandlerClass]
	public class NobeSys : MonoSingleton<NobeSys>
	{
		public class DelayNobeInfo
		{
			public int Type;

			public int nShow;

			public SCPKG_GAME_VIP_NTF m_vipInfoBack;

			public DelayNobeInfo()
			{
				this.nShow = -1;
				this.m_vipInfoBack = new SCPKG_GAME_VIP_NTF();
			}
		}

		public struct PrivilegeInfo
		{
			public bool isVisible;

			public string name;
		}

		public enum Tab
		{
			Base_Info,
			Grow_Info
		}

		public enum enPlayerFormWidget
		{
			Tab,
			Base_Info_Tab,
			Grow_Info_Tab,
			Top_Title_Text,
			GrowInfo_Reawrd_List,
			GrowInfo_Rule_Desc,
			GrowInfo_Head_Left,
			GrowInfo_Head_Right,
			GrowInfo_Head_Progress,
			GrowInfo_Head_Progress_Tips,
			BaseInfo_Head_Left,
			BaseInfo_Head_Right,
			BaseInfo_Head_Progress,
			BaseInfo_Head_Progress_Tips,
			BaseInfo_Head_Left_LevelText,
			BaseInfo_Head_Right_LevelText,
			GrowInfo_Head_Left_LevelText,
			GrowInfo_Head_Right_LevelText,
			GrowInfo_Level_Desc,
			BaseInfo_Level_Desc,
			BaseInfo_Reawrd_List,
			BaseInfo_Noble_LevelText,
			BaseInfo_Image_Level_Tips,
			GrowInfo_Image_Level_Tips,
			BaseInfo_Noble_LevelNUm1,
			BaseInfo_Noble_LevelNUm2,
			BaseInfo_OnLeftObject,
			BaseInfo_OnRightObject,
			GrowInfo_OnLeftObject,
			GrowInfo_OnRightObject
		}

		protected enum NobeLevelUpFormWidget
		{
			None = -1,
			CurNobeLv,
			LastNobeLv,
			CurNobeLv2,
			TeQuanPanel,
			Gift
		}

		private enum NobelLoseFormWidget
		{
			TextContrl,
			TeQuanPanel,
			TeQuanPanelText
		}

		private const int m_nTotalLevel = 8;

		private const int maxPrivilegeNum = 4;

		public int m_Level = 2;

		public int m_Score = 100;

		public int m_headidx = 2;

		private int m_CurPage = 1;

		private int m_CurPageLevel;

		public SCPKG_GAME_VIP_NTF m_vipInfo = new SCPKG_GAME_VIP_NTF();

		private Material m_particleMaterial;

		public bool m_IsShowDelayNobeTips;

		public NobeSys.DelayNobeInfo m_DelayNobeInfo = new NobeSys.DelayNobeInfo();

		private NobeSys.DelayNobeInfo m_LoseNobeInfo = new NobeSys.DelayNobeInfo();

		private NobeSys.Tab m_CurTab;

		public string sPlayerInfoFormPath = CUIUtility.s_IDIP_Form_Dir + "Form_Nobe.prefab";

		public string sNobeChangeHeadIconForm = CUIUtility.s_IDIP_Form_Dir + "Form_Nobe_HeadChangeIcon.prefab";

		private bool m_IsFormOpen;

		private CUIFormScript m_Form;

		private GameObject m_BaseInfoOnLeftObj;

		private GameObject m_BaseInfoOnRightObj;

		private GameObject m_GrowInfoOnLeftObj;

		private GameObject m_GrowInfoOnRightObj;

		private bool m_bInit;

		private bool m_bInitDiamondForm;

		private Vector2[] m_NumPosBackDiamondForm = new Vector2[6];

		private Vector2[] m_NumPosBack = new Vector2[6];

		private CUIFormScript m_ChangeHeadIconForm;

		private ListView<ResNobeInfo> m_ChangeHeadIconResList = new ListView<ResNobeInfo>();

		private string m_iconPath = CUIUtility.s_Sprite_System_ShareUI_Dir + "nobe_iconXiao.prefab";

		private static uint _lastNobeLv;

		private uint[] tempIndex = new uint[4];

		private string[] tempStr = new string[4];

		private string[] tempName = new string[4];

		public NobeSys.Tab CurTab
		{
			get
			{
				return this.m_CurTab;
			}
			set
			{
				this.m_CurTab = value;
			}
		}

		private Material GetParticleMaterial(Material SrcMatetial)
		{
			if (this.m_particleMaterial == null)
			{
				this.m_particleMaterial = new Material(SrcMatetial);
				this.m_particleMaterial.shader = Shader.Find("S_Game_Particle/Additive");
			}
			return this.m_particleMaterial;
		}

		[MessageHandler(4600)]
		public static void OnGetNobeVipInfo(CSPkg msg)
		{
			MonoSingleton<NobeSys>.GetInstance().m_vipInfo = msg.stPkgData.stGameVipNtf;
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
			NobeSys._lastNobeLv = masterRoleInfo.GetNobeInfo().stGameVipClient.dwCurLevel;
			int dwCurLevel = (int)msg.stPkgData.stGameVipNtf.stGameVipClient.dwCurLevel;
			masterRoleInfo.SetNobeInfo(msg.stPkgData.stGameVipNtf);
			if (Singleton<BattleLogic>.instance.isRuning)
			{
				MonoSingleton<NobeSys>.GetInstance().m_DelayNobeInfo.Type = 0;
				MonoSingleton<NobeSys>.GetInstance().m_DelayNobeInfo.nShow = 0;
				MonoSingleton<NobeSys>.GetInstance().m_DelayNobeInfo.m_vipInfoBack = msg.stPkgData.stGameVipNtf;
			}
			else
			{
				if (MonoSingleton<NobeSys>.GetInstance().m_IsShowDelayNobeTips && msg.stPkgData.stGameVipNtf.dwNtfType > 0u)
				{
					MonoSingleton<NobeSys>.GetInstance().m_DelayNobeInfo.Type = 1;
					MonoSingleton<NobeSys>.GetInstance().m_DelayNobeInfo.nShow = 0;
					MonoSingleton<NobeSys>.GetInstance().m_DelayNobeInfo.m_vipInfoBack = msg.stPkgData.stGameVipNtf;
				}
				else
				{
					MonoSingleton<NobeSys>.GetInstance().ShowLoginTips(MonoSingleton<NobeSys>.GetInstance().m_vipInfo);
					MonoSingleton<NobeSys>.GetInstance().UpdateNobeLevelChange();
				}
				MonoSingleton<NobeSys>.GetInstance().ShowNobeTipsInDiamond();
			}
			Singleton<EventRouter>.GetInstance().BroadCastEvent(EventID.NOBE_STATE_CHANGE);
		}

		public void ShowDelayNobeTipsInfo()
		{
			if (MonoSingleton<NobeSys>.GetInstance().m_DelayNobeInfo.nShow == 0)
			{
				MonoSingleton<NobeSys>.GetInstance().m_DelayNobeInfo.nShow = 1;
				MonoSingleton<NobeSys>.GetInstance().ShowLoginTips(MonoSingleton<NobeSys>.GetInstance().m_DelayNobeInfo.m_vipInfoBack);
				MonoSingleton<NobeSys>.GetInstance().m_DelayNobeInfo.m_vipInfoBack.dwNtfType = 0u;
			}
		}

		public void ShowDelayNobeLoseTipsInfo()
		{
			if (this.m_LoseNobeInfo.nShow == 0)
			{
				this.m_LoseNobeInfo.nShow = 1;
				this.ShowNobeLoseTips(MonoSingleton<NobeSys>.GetInstance().m_LoseNobeInfo.m_vipInfoBack);
				this.m_LoseNobeInfo.m_vipInfoBack.dwNtfType = 0u;
			}
		}

		private void ShowLoginTips(SCPKG_GAME_VIP_NTF vipInfo)
		{
			if (vipInfo == null)
			{
				return;
			}
			switch (vipInfo.dwNtfType)
			{
			case 1u:
				this.ShowNobeLevelUpForm(vipInfo);
				break;
			case 2u:
				this.m_LoseNobeInfo.m_vipInfoBack = vipInfo;
				this.m_LoseNobeInfo.nShow = 0;
				this.m_LoseNobeInfo.Type = 2;
				break;
			case 3u:
			{
				string strContent = string.Format(Singleton<CTextManager>.GetInstance().GetText("VIPREGAIN"), vipInfo.stGameVipClient.dwCurLevel);
				Singleton<CUIManager>.GetInstance().OpenMessageBox(strContent, false);
				break;
			}
			case 4u:
				this.ShowNobeLevelUpForm(vipInfo);
				break;
			case 5u:
				this.m_LoseNobeInfo.m_vipInfoBack = vipInfo;
				this.m_LoseNobeInfo.nShow = 0;
				this.m_LoseNobeInfo.Type = 5;
				break;
			}
		}

		protected override void Init()
		{
			base.Init();
			CUIEventManager instance = Singleton<CUIEventManager>.GetInstance();
			instance.AddUIEventListener(enUIEventID.NOBE_OPEN_Form, new CUIEventManager.OnUIEventHandler(this.OpenForm));
			instance.AddUIEventListener(enUIEventID.NOBE_CLOSE_Form, new CUIEventManager.OnUIEventHandler(this.CloseForm));
			instance.AddUIEventListener(enUIEventID.NOBE_TAB_Change, new CUIEventManager.OnUIEventHandler(this.OnTabChange));
			instance.AddUIEventListener(enUIEventID.NOBE_PAY, new CUIEventManager.OnUIEventHandler(this.OnNobePay));
			instance.AddUIEventListener(enUIEventID.NOBE_GOTO_STROE, new CUIEventManager.OnUIEventHandler(this.OnNobeGotoStore));
			instance.AddUIEventListener(enUIEventID.NOBE_LEFT, new CUIEventManager.OnUIEventHandler(this.OnLeft));
			instance.AddUIEventListener(enUIEventID.NOBE_RIGHT, new CUIEventManager.OnUIEventHandler(this.OnRight));
			instance.AddUIEventListener(enUIEventID.NOBE_LEFT_Nobe_Level, new CUIEventManager.OnUIEventHandler(this.OnLeftLevel));
			instance.AddUIEventListener(enUIEventID.NOBE_RIGHT_Nobe_Level, new CUIEventManager.OnUIEventHandler(this.OnRightLevel));
			instance.AddUIEventListener(enUIEventID.NOBE_CHANGEHEAD_ICON_OPEN_FORM, new CUIEventManager.OnUIEventHandler(this.OpenChangeHeadIconForm));
			instance.AddUIEventListener(enUIEventID.NOBE_CHANGEHEAD_ICON_CLOSE_FORM, new CUIEventManager.OnUIEventHandler(this.CloseChangeHeadIconForm));
			instance.AddUIEventListener(enUIEventID.Mall_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnMallOpenForm));
			instance.AddUIEventListener(enUIEventID.Mall_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnMallCloseForm));
			Singleton<EventRouter>.instance.AddEventHandler(EventID.Mall_Get_Product_OK, new Action(this.OnMall_Get_Product_OK));
			instance.AddUIEventListener(enUIEventID.Mall_Get_AWARD_CLOSE_FORM, new CUIEventManager.OnUIEventHandler(this.OnMall_Get_AWARD_CLOSE_FORM));
			instance.AddUIEventListener(enUIEventID.Mall_Roulette_Close_Award_Form, new CUIEventManager.OnUIEventHandler(this.OnMall_Get_AWARD_CLOSE_FORM));
			this.m_IsFormOpen = false;
			this.m_CurTab = NobeSys.Tab.Base_Info;
			this.m_Form = null;
			this.m_ChangeHeadIconResList = this.GetAllHeadIcon();
		}

		public void OpenForm(CUIEvent uiEvent)
		{
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Pay_ClosePayDianQuanForm);
			Singleton<CUIManager>.GetInstance().CloseForm(this.sPlayerInfoFormPath);
			this.m_IsFormOpen = true;
			this.m_Form = Singleton<CUIManager>.GetInstance().OpenForm(this.sPlayerInfoFormPath, false, true);
			this.CurTab = NobeSys.Tab.Base_Info;
			this.m_BaseInfoOnLeftObj = this.m_Form.GetWidget(26);
			this.m_BaseInfoOnRightObj = this.m_Form.GetWidget(27);
			this.m_GrowInfoOnLeftObj = this.m_Form.GetWidget(28);
			this.m_GrowInfoOnRightObj = this.m_Form.GetWidget(29);
			this.InitTab();
		}

		private void CloseForm(CUIEvent uiEvent)
		{
			if (!this.m_IsFormOpen)
			{
				return;
			}
			this.m_IsFormOpen = false;
			Singleton<CUIManager>.GetInstance().CloseForm(this.m_Form);
			this.m_Form = null;
		}

		private void OnMallOpenForm(CUIEvent uiEvent)
		{
			MonoSingleton<NobeSys>.GetInstance().m_IsShowDelayNobeTips = true;
		}

		private void OnMallCloseForm(CUIEvent uiEvent)
		{
			MonoSingleton<NobeSys>.GetInstance().m_IsShowDelayNobeTips = false;
		}

		private void OnMall_Get_Product_OK()
		{
			if (MonoSingleton<NobeSys>.GetInstance().m_IsShowDelayNobeTips && MonoSingleton<NobeSys>.GetInstance().m_DelayNobeInfo.nShow == 0)
			{
				MonoSingleton<NobeSys>.GetInstance().m_DelayNobeInfo.nShow = 1;
				MonoSingleton<NobeSys>.GetInstance().ShowLoginTips(MonoSingleton<NobeSys>.GetInstance().m_DelayNobeInfo.m_vipInfoBack);
				MonoSingleton<NobeSys>.GetInstance().m_DelayNobeInfo.m_vipInfoBack.dwNtfType = 0u;
			}
		}

		private void OnMall_Get_AWARD_CLOSE_FORM(CUIEvent uiEvent)
		{
			this.OnMall_Get_Product_OK();
		}

		private void OnNobePay(CUIEvent uiEvent)
		{
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Pay_OpenBuyDianQuanPanel);
		}

		private void OnNobeGotoStore(CUIEvent uiEvent)
		{
			Singleton<CUIManager>.instance.CloseForm(CPaySystem.s_buyDianQuanFormPath);
			Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(enUIEventID.Mall_OpenForm);
		}

		private void UpdateBaseInfolevel(int curPage)
		{
			if (curPage == 0)
			{
				this.m_BaseInfoOnLeftObj.CustomSetActive(false);
			}
			else
			{
				this.m_BaseInfoOnLeftObj.CustomSetActive(true);
			}
			if (curPage == 7)
			{
				this.m_BaseInfoOnRightObj.CustomSetActive(false);
			}
			else
			{
				this.m_BaseInfoOnRightObj.CustomSetActive(true);
			}
		}

		private void OnLeft(CUIEvent uiEvent)
		{
			if (!this.m_IsFormOpen || this.m_Form == null)
			{
				return;
			}
			int num = this.m_CurPage - 1;
			this.UpdateBaseInfolevel(num);
			if (num < 0)
			{
				return;
			}
			this.m_CurPage = num;
			CUIListScript component = this.m_Form.GetWidget(20).GetComponent<CUIListScript>();
			component.MoveElementInScrollArea(this.m_CurPage, false);
		}

		private void OnRight(CUIEvent uiEvent)
		{
			if (!this.m_IsFormOpen || this.m_Form == null)
			{
				return;
			}
			int num = this.m_CurPage + 1;
			this.UpdateBaseInfolevel(num);
			if (num >= 8)
			{
				return;
			}
			this.m_CurPage = num;
			CUIListScript component = this.m_Form.GetWidget(20).GetComponent<CUIListScript>();
			component.MoveElementInScrollArea(this.m_CurPage, false);
		}

		private void UpdateGrowInfoLevel(int curPage)
		{
			if (curPage == 0)
			{
				this.m_GrowInfoOnLeftObj.CustomSetActive(false);
			}
			else
			{
				this.m_GrowInfoOnLeftObj.CustomSetActive(true);
			}
			if (curPage == 1)
			{
				this.m_GrowInfoOnRightObj.CustomSetActive(false);
			}
			else
			{
				this.m_GrowInfoOnRightObj.CustomSetActive(true);
			}
		}

		private void OnLeftLevel(CUIEvent uiEvent)
		{
			if (!this.m_IsFormOpen || this.m_Form == null)
			{
				return;
			}
			int num = this.m_CurPageLevel - 1;
			this.UpdateGrowInfoLevel(num);
			if (num < 0)
			{
				return;
			}
			this.m_CurPageLevel = num;
			CUIListScript component = this.m_Form.GetWidget(4).GetComponent<CUIListScript>();
			component.MoveElementInScrollArea(this.m_CurPageLevel, false);
		}

		private void OnRightLevel(CUIEvent uiEvent)
		{
			if (!this.m_IsFormOpen || this.m_Form == null)
			{
				return;
			}
			int num = this.m_CurPageLevel + 1;
			this.UpdateGrowInfoLevel(num);
			if (num >= 2)
			{
				return;
			}
			this.m_GrowInfoOnLeftObj.CustomSetActive(true);
			this.m_CurPageLevel = num;
			CUIListScript component = this.m_Form.GetWidget(4).GetComponent<CUIListScript>();
			component.MoveElementInScrollArea(this.m_CurPageLevel, false);
		}

		private void InitTab()
		{
			if (this.m_Form == null || !this.m_IsFormOpen)
			{
				return;
			}
			GameObject widget = this.m_Form.GetWidget(1);
			if (widget != null && widget.activeSelf)
			{
				widget.CustomSetActive(false);
			}
			GameObject widget2 = this.m_Form.GetWidget(2);
			if (widget2 != null && widget2.activeSelf)
			{
				widget2.CustomSetActive(false);
			}
			NobeSys.Tab[] array = (NobeSys.Tab[])Enum.GetValues(typeof(NobeSys.Tab));
			string[] array2 = new string[array.Length];
			byte b = 0;
			while ((int)b < array.Length)
			{
				NobeSys.Tab tab = array[(int)b];
				if (tab != NobeSys.Tab.Base_Info)
				{
					if (tab == NobeSys.Tab.Grow_Info)
					{
						array2[(int)b] = "成长体系";
					}
				}
				else
				{
					array2[(int)b] = "贵族特权";
				}
				b += 1;
			}
			GameObject widget3 = this.m_Form.GetWidget(0);
			CUIListScript component = widget3.GetComponent<CUIListScript>();
			if (component != null)
			{
				component.SetElementAmount(array2.Length);
				for (int i = 0; i < component.m_elementAmount; i++)
				{
					CUIListElementScript elemenet = component.GetElemenet(i);
					Text component2 = elemenet.gameObject.transform.Find("Text").GetComponent<Text>();
					component2.set_text(array2[i]);
				}
				component.m_alwaysDispatchSelectedChangeEvent = true;
				component.SelectElement((int)this.CurTab, true);
			}
		}

		private void OnTabChange(CUIEvent uiEvent)
		{
			if (this.m_Form == null || !this.m_IsFormOpen)
			{
				return;
			}
			if (this.m_vipInfo == null)
			{
				return;
			}
			if (!this.m_bInit)
			{
				GameObject widget = this.m_Form.GetWidget(22);
				for (int i = 0; i < 5; i++)
				{
					Transform transform = widget.transform.Find("n" + i);
					this.m_NumPosBack[i] = transform.GetComponent<RectTransform>().anchoredPosition;
				}
				Transform transform2 = widget.transform.Find("tipsRight");
				this.m_NumPosBack[5] = transform2.GetComponent<RectTransform>().anchoredPosition;
				this.m_bInit = true;
			}
			CUIListScript component = uiEvent.m_srcWidget.GetComponent<CUIListScript>();
			int selectedIndex = component.GetSelectedIndex();
			this.CurTab = (NobeSys.Tab)selectedIndex;
			if (this.m_Form != null)
			{
				GameObject widget2 = this.m_Form.GetWidget(1);
				GameObject widget3 = this.m_Form.GetWidget(2);
				Text component2 = this.m_Form.GetWidget(3).GetComponent<Text>();
				NobeSys.Tab curTab = this.m_CurTab;
				if (curTab != NobeSys.Tab.Base_Info)
				{
					if (curTab == NobeSys.Tab.Grow_Info && widget2 != null && widget3 != null && !widget3.activeSelf)
					{
						widget2.CustomSetActive(false);
						widget3.CustomSetActive(true);
						component2.set_text(component.GetElemenet(selectedIndex).gameObject.transform.Find("Text").GetComponent<Text>().get_text());
						this.UpdateGrowInfo();
					}
				}
				else if (widget2 != null && !widget2.activeSelf)
				{
					widget2.CustomSetActive(true);
					if (widget3)
					{
						widget3.CustomSetActive(false);
					}
					component2.set_text(component.GetElemenet(selectedIndex).gameObject.transform.Find("Text").GetComponent<Text>().get_text());
					this.UpdateBaseInfo();
				}
			}
		}

		public void ShowNobeTipsInDiamond()
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CPaySystem.s_buyDianQuanFormPath);
			if (form == null)
			{
				return;
			}
			if (this.m_vipInfo == null)
			{
				return;
			}
			if (form == null)
			{
				return;
			}
			Transform transform = form.gameObject.transform.Find("NobTips");
			if (transform == null)
			{
				return;
			}
			int dwCurLevel = (int)this.m_vipInfo.stGameVipClient.dwCurLevel;
			int dwScore = (int)this.m_vipInfo.stGameVipClient.dwScore;
			int num = GameDataMgr.resVipDianQuan.Count();
			Transform transform2 = transform.Find("txtTips");
			if (transform2 == null)
			{
				return;
			}
			if (!this.m_bInitDiamondForm)
			{
				for (int i = 0; i < 5; i++)
				{
					Transform transform3 = transform2.transform.Find("n" + i);
					this.m_NumPosBackDiamondForm[i] = transform3.GetComponent<RectTransform>().anchoredPosition;
				}
				Transform transform4 = transform2.transform.Find("tipsRight");
				this.m_NumPosBackDiamondForm[5] = transform4.GetComponent<RectTransform>().anchoredPosition;
				this.m_bInitDiamondForm = true;
			}
			transform2.gameObject.CustomSetActive(false);
			GameObject gameObject = null;
			Transform transform5 = transform.transform.Find("level");
			if (transform5)
			{
				gameObject = transform5.gameObject;
			}
			Transform transform6 = transform.transform.Find("txtWins");
			string text = string.Empty;
			if (dwCurLevel >= num)
			{
				text = "贵族等级已满";
				if (transform6)
				{
					transform6.gameObject.CustomSetActive(true);
					transform6.GetComponent<Text>().set_text(text);
				}
				Transform transform7 = transform.Find("tipsLeft/vipLevel");
				if (transform7)
				{
					transform7.gameObject.CustomSetActive(true);
					string prefabPath = string.Format("{0}{1}.prefab", CUIUtility.s_Sprite_System_ShareUI_Dir, num.ToString());
					CUIUtility.SetImageSprite(transform7.gameObject.GetComponent<Image>(), prefabPath, null, true, false, false, false);
				}
				if (gameObject)
				{
					gameObject.GetComponent<Image>().set_fillAmount(1f);
				}
			}
			else
			{
				int num2 = dwCurLevel + 1;
				ResVIPCoupons dataByKey = GameDataMgr.resVipDianQuan.GetDataByKey((long)num2);
				if (dataByKey != null)
				{
					int num3 = (int)(dataByKey.dwConsumeCoupons - (uint)dwScore);
					if (num3 > 0)
					{
						text = string.Format("再消费{0}点券，升级到贵族{1}", num3, num2);
						if (transform6)
						{
							transform6.gameObject.CustomSetActive(false);
						}
						this.UpdateImageLevelTips(transform2.gameObject, num2, num3, true);
					}
					else
					{
						int dwMaxLevel = (int)this.m_vipInfo.dwMaxLevel;
						ResVIPCoupons dataByKey2 = GameDataMgr.resVipDianQuan.GetDataByKey((long)dwMaxLevel);
						text = "只要消费任意金额的点券，即可恢复到之前的最高等级";
						if (transform6)
						{
							transform6.gameObject.CustomSetActive(true);
							transform6.GetComponent<Text>().set_text(text);
						}
					}
					if (gameObject)
					{
						gameObject.GetComponent<Image>().set_fillAmount(1f * (float)dwScore / dataByKey.dwConsumeCoupons);
					}
				}
				Transform transform8 = transform.Find("tipsLeft/vipLevel");
				if (transform8)
				{
					transform8.gameObject.CustomSetActive(true);
					string prefabPath2 = string.Format("{0}{1}.prefab", CUIUtility.s_Sprite_System_ShareUI_Dir, dwCurLevel.ToString());
					CUIUtility.SetImageSprite(transform8.gameObject.GetComponent<Image>(), prefabPath2, null, true, false, false, false);
				}
			}
			if (CSysDynamicBlock.bVipBlock)
			{
				transform.gameObject.CustomSetActive(false);
			}
		}

		private void UpdateNobeLevelChange()
		{
			if (this.m_Form != null)
			{
				GameObject widget = this.m_Form.GetWidget(1);
				GameObject widget2 = this.m_Form.GetWidget(2);
				Text component = this.m_Form.GetWidget(3).GetComponent<Text>();
				NobeSys.Tab curTab = this.m_CurTab;
				if (curTab != NobeSys.Tab.Base_Info)
				{
					if (curTab == NobeSys.Tab.Grow_Info && widget != null && widget2 != null && widget2.activeSelf)
					{
						this.UpdateGrowInfo();
					}
				}
				else if (widget != null && widget.activeSelf)
				{
					this.UpdateBaseInfo();
				}
			}
		}

		private void UpdateBaseInfo()
		{
			if (this.m_vipInfo == null)
			{
				return;
			}
			Image component = this.m_Form.GetWidget(14).GetComponent<Image>();
			GameObject widget = this.m_Form.GetWidget(1);
			Image component2 = this.m_Form.GetWidget(10).GetComponent<Image>();
			int dwCurLevel = (int)this.m_vipInfo.stGameVipClient.dwCurLevel;
			int dwScore = (int)this.m_vipInfo.stGameVipClient.dwScore;
			int num = GameDataMgr.resVipDianQuan.Count();
			GameObject gameObject = null;
			if (widget)
			{
				Transform transform = widget.transform.Find("pnlHead/mvp/Img_Bg1/tips");
				if (transform)
				{
					gameObject = transform.gameObject;
					gameObject.CustomSetActive(false);
				}
			}
			GameObject gameObject2 = null;
			if (widget)
			{
				Transform transform2 = widget.transform.Find("pnlHead/mvp/LevelLeft/level");
				if (transform2)
				{
					gameObject2 = transform2.gameObject;
				}
			}
			string text = string.Empty;
			GameObject widget2 = this.m_Form.GetWidget(22);
			widget2.CustomSetActive(false);
			if (dwCurLevel >= num)
			{
				text = "贵族等级已满";
				string prefabPath = string.Format("{0}{1}.prefab", CUIUtility.s_Sprite_System_ShareUI_Dir, dwCurLevel.ToString());
				CUIUtility.SetImageSprite(component, prefabPath, null, true, false, false, false);
				if (gameObject)
				{
					gameObject.CustomSetActive(true);
					gameObject.GetComponent<Text>().set_text(text);
				}
				if (gameObject2)
				{
					gameObject2.GetComponent<Image>().set_fillAmount(1f);
				}
			}
			else
			{
				int num2 = dwCurLevel + 1;
				ResVIPCoupons dataByKey = GameDataMgr.resVipDianQuan.GetDataByKey((long)num2);
				if (dataByKey != null)
				{
					int num3 = (int)(dataByKey.dwConsumeCoupons - (uint)dwScore);
					if (num3 > 0)
					{
						this.UpdateImageLevelTips(widget2, num2, num3, false);
						string prefabPath2 = string.Format("{0}{1}.prefab", CUIUtility.s_Sprite_System_ShareUI_Dir, dwCurLevel.ToString());
						CUIUtility.SetImageSprite(component, prefabPath2, null, true, false, false, false);
						if (gameObject2)
						{
							gameObject2.GetComponent<Image>().set_fillAmount(1f * (float)dwScore / dataByKey.dwConsumeCoupons);
						}
						component2.gameObject.CustomSetActive(true);
					}
					else
					{
						widget2.CustomSetActive(false);
						int dwMaxLevel = (int)this.m_vipInfo.dwMaxLevel;
						ResVIPCoupons dataByKey2 = GameDataMgr.resVipDianQuan.GetDataByKey((long)dwMaxLevel);
						text = "只要消费任意金额的点券，即可恢复到之前的最高等级";
						if (gameObject2)
						{
							gameObject2.GetComponent<Image>().set_fillAmount(1f * (float)dwScore / dataByKey.dwConsumeCoupons);
						}
						string prefabPath3 = string.Format("{0}{1}.prefab", CUIUtility.s_Sprite_System_ShareUI_Dir, dwCurLevel.ToString());
						CUIUtility.SetImageSprite(component, prefabPath3, null, true, false, false, false);
						component2.gameObject.CustomSetActive(true);
						if (gameObject)
						{
							gameObject.CustomSetActive(true);
							gameObject.GetComponent<Text>().set_text(text);
						}
					}
				}
			}
			DictionaryView<uint, ListView<ResNobeInfo>> dictionaryView = new DictionaryView<uint, ListView<ResNobeInfo>>();
			for (int i = 0; i < GameDataMgr.resNobeInfoDatabin.count; i++)
			{
				ResNobeInfo dataByIndex = GameDataMgr.resNobeInfoDatabin.GetDataByIndex(i);
				if (dataByIndex != null && dataByIndex.bResType == 1)
				{
					if (!dictionaryView.ContainsKey(dataByIndex.dwNobeLevel))
					{
						ListView<ResNobeInfo> listView = new ListView<ResNobeInfo>();
						listView.Add(dataByIndex);
						dictionaryView.Add(dataByIndex.dwNobeLevel, listView);
					}
					else
					{
						ListView<ResNobeInfo> listView2 = dictionaryView[dataByIndex.dwNobeLevel];
						listView2.Add(dataByIndex);
					}
				}
			}
			DictionaryView<uint, ListView<ResNobeInfo>> dictionaryView2 = new DictionaryView<uint, ListView<ResNobeInfo>>();
			for (uint num4 = 1u; num4 <= 8u; num4 += 1u)
			{
				ListView<ResNobeInfo> listView3 = new ListView<ResNobeInfo>();
				for (uint num5 = num4; num5 >= 1u; num5 -= 1u)
				{
					if (dictionaryView.ContainsKey(num5))
					{
						for (int j = 0; j < dictionaryView[num5].Count; j++)
						{
							ResNobeInfo resNobeInfo = dictionaryView[num5][j];
							bool flag = false;
							for (int k = 0; k < listView3.Count; k++)
							{
								ResNobeInfo resNobeInfo2 = listView3[k];
								string text2 = Utility.UTF8Convert(resNobeInfo2.szResIcon);
								string text3 = Utility.UTF8Convert(resNobeInfo.szResIcon);
								if (text2 == text3)
								{
									flag = true;
									break;
								}
							}
							if (!flag)
							{
								listView3.Add(resNobeInfo);
								listView3.Sort(new Comparison<ResNobeInfo>(NobeSys.ComparebyShowIdx));
							}
						}
					}
				}
				dictionaryView2.Add(num4, listView3);
			}
			CUIListScript component3 = this.m_Form.GetWidget(20).GetComponent<CUIListScript>();
			if (component3 == null)
			{
				return;
			}
			component3.SetElementAmount(8);
			for (uint num6 = 0u; num6 < 8u; num6 += 1u)
			{
				CUIListElementScript elemenet = component3.GetElemenet((int)num6);
				if (!(elemenet == null) && dictionaryView2.ContainsKey(num6 + 1u))
				{
					ListView<ResNobeInfo> listView4 = dictionaryView2[num6 + 1u];
					for (int l = 0; l < 4; l++)
					{
						GameObject gameObject3 = null;
						Transform transform3 = elemenet.transform.Find("Item/NobeObj/itemCell" + l);
						if (transform3 != null)
						{
							gameObject3 = transform3.gameObject;
						}
						if (l < listView4.Count && gameObject3 != null)
						{
							ResNobeInfo resNobeInfo3 = listView4[l];
							if (gameObject3)
							{
								gameObject3.CustomSetActive(true);
								Image component4 = gameObject3.transform.Find("imgIcon").GetComponent<Image>();
								Text component5 = gameObject3.transform.Find("Text").GetComponent<Text>();
								string prefabPath4 = string.Format("{0}{1}.prefab", CUIUtility.s_Sprite_System_ShareUI_Dir, Utility.UTF8Convert(resNobeInfo3.szResIcon));
								CUIUtility.SetImageSprite(component4, prefabPath4, null, true, false, false, false);
								component5.set_text(Utility.UTF8Convert(resNobeInfo3.szResName));
							}
						}
						else if (gameObject3 != null)
						{
							gameObject3.CustomSetActive(false);
						}
					}
					Transform itemCell = elemenet.transform.Find("Item/NobeObj");
					this.UpdateNobeLevelText(itemCell, (int)(num6 + 1u));
					Transform itemCell2 = elemenet.transform.Find("Item/GiftObj");
					this.SetGifIcon(itemCell2, (int)(num6 + 1u));
					this.UpdateNobeLevelText(itemCell2, (int)(num6 + 1u));
					Transform itemCell3 = elemenet.transform.Find("Item/WeekObj");
					this.SetWeekGifIcon(itemCell3, (int)(num6 + 1u));
					this.UpdateNobeLevelText(itemCell3, (int)(num6 + 1u));
				}
			}
			if (dwCurLevel == 0)
			{
				component3.MoveElementInScrollArea(dwCurLevel, false);
				this.m_CurPage = dwCurLevel;
			}
			else
			{
				component3.MoveElementInScrollArea(dwCurLevel - 1, false);
				this.m_CurPage = dwCurLevel - 1;
			}
			this.UpdateBaseInfolevel(this.m_CurPage);
			if (component3)
			{
				Transform transform4 = component3.transform.Find("ScrollRect");
				if (transform4)
				{
					ScrollRect component6 = transform4.GetComponent<ScrollRect>();
					if (component6)
					{
						component6.set_horizontal(false);
						component6.set_vertical(false);
					}
				}
			}
		}

		private void UpdateNobeLevelText(Transform itemCell, int level)
		{
			if (itemCell == null)
			{
				return;
			}
			Transform transform = itemCell.Find("imgMyPointBg/tipsRight/vipLevel");
			if (transform != null)
			{
				Image component = transform.GetComponent<Image>();
				if (component)
				{
					string prefabPath = string.Format("{0}{1}.prefab", CUIUtility.s_Sprite_System_ShareUI_Dir, level.ToString());
					CUIUtility.SetImageSprite(component, prefabPath, null, true, false, false, false);
				}
			}
		}

		private void SetGifIcon(Transform itemCell, int level)
		{
			if (itemCell == null)
			{
				return;
			}
			int num = 0;
			for (int i = 1; i <= GameDataMgr.resNobeInfoDatabin.count; i++)
			{
				ResNobeInfo dataByIndex = GameDataMgr.resNobeInfoDatabin.GetDataByIndex(i - 1);
				if (dataByIndex != null && dataByIndex.bResType == 3 && (ulong)dataByIndex.dwNobeLevel == (ulong)((long)level))
				{
					Transform transform = itemCell.transform.Find("Gift" + num);
					if (transform)
					{
						transform.gameObject.CustomSetActive(true);
						Transform transform2 = transform.FindChild(string.Format("Icon", new object[0]));
						Transform transform3 = transform.FindChild(string.Format("Text", new object[0]));
						Transform transform4 = transform.FindChild(string.Format("ExperienceCard", new object[0]));
						transform4.gameObject.CustomSetActive(dataByIndex.bShowIdx == 1);
						Transform transform5 = transform.FindChild(string.Format("TextNum", new object[0]));
						transform5.gameObject.GetComponent<Text>().set_text((dataByIndex.dwJiaoBiaoNum > 0u) ? dataByIndex.dwJiaoBiaoNum.ToString(CultureInfo.get_InvariantCulture()) : null);
						string prefabPath = string.Format("{0}{1}.prefab", CUIUtility.s_Sprite_Dynamic_Icon_Dir, Utility.UTF8Convert(dataByIndex.szResIcon));
						ResVIPCoupons dataByKey = GameDataMgr.resVipDianQuan.GetDataByKey((long)level);
						if (dataByKey != null)
						{
							int dwGiftID = (int)dataByKey.dwGiftID;
							ResRandomRewardStore dataByKey2 = GameDataMgr.randomRewardDB.GetDataByKey((long)dwGiftID);
							if (dataByKey2 != null && num < dataByKey2.astRewardDetail.Length)
							{
								CUseable cUseable = CUseableManager.CreateUsableByRandowReward((RES_RANDOM_REWARD_TYPE)dataByKey2.astRewardDetail[num].bItemType, (int)dataByKey2.astRewardDetail[num].dwLowCnt, dataByKey2.astRewardDetail[num].dwItemID);
								if (cUseable != null)
								{
									CUIEventScript cUIEventScript = transform.GetComponent<CUIEventScript>();
									if (cUIEventScript == null)
									{
										cUIEventScript = transform.gameObject.AddComponent<CUIEventScript>();
									}
									stUIEventParams eventParams = new stUIEventParams
									{
										iconUseable = cUseable
									};
									cUIEventScript.SetUIEvent(enUIEventType.Down, enUIEventID.Tips_ItemInfoOpen, eventParams);
									cUIEventScript.SetUIEvent(enUIEventType.HoldEnd, enUIEventID.Tips_ItemInfoClose, eventParams);
									cUIEventScript.SetUIEvent(enUIEventType.Click, enUIEventID.Tips_ItemInfoClose, eventParams);
									cUIEventScript.SetUIEvent(enUIEventType.DragEnd, enUIEventID.Tips_ItemInfoClose, eventParams);
								}
							}
						}
						if (transform2 != null && transform3 != null)
						{
							transform2.gameObject.GetComponent<Image>().SetSprite(prefabPath, this.m_Form, true, false, false, false);
							transform3.gameObject.GetComponent<Text>().set_text(Utility.UTF8Convert(dataByIndex.szResName));
							num++;
						}
					}
				}
			}
			int num2 = 2;
			for (int j = num; j <= num2; j++)
			{
				Transform transform6 = itemCell.transform.Find("Gift" + j);
				if (transform6 != null)
				{
					transform6.gameObject.CustomSetActive(false);
				}
			}
		}

		private void SetWeekGifIcon(Transform itemCell, int level)
		{
			if (itemCell == null)
			{
				return;
			}
			int num = 0;
			int num2 = 3;
			ResVIPCoupons dataByKey = GameDataMgr.resVipDianQuan.GetDataByKey((long)level);
			if (dataByKey != null)
			{
				int dwWeekGiftID = (int)dataByKey.dwWeekGiftID;
				ResRandomRewardStore dataByKey2 = GameDataMgr.randomRewardDB.GetDataByKey((long)dwWeekGiftID);
				if (dataByKey2 != null)
				{
					for (int i = 0; i < dataByKey2.astRewardDetail.Length; i++)
					{
						if (dataByKey2.astRewardDetail[i].bItemType == 0 || dataByKey2.astRewardDetail[i].bItemType >= 18)
						{
							break;
						}
						CUseable cUseable = CUseableManager.CreateUsableByRandowReward((RES_RANDOM_REWARD_TYPE)dataByKey2.astRewardDetail[i].bItemType, (int)dataByKey2.astRewardDetail[i].dwLowCnt, dataByKey2.astRewardDetail[i].dwItemID);
						if (cUseable != null)
						{
							Transform transform = itemCell.Find("Week" + num);
							if (!(transform == null))
							{
								if (transform)
								{
									Image component = transform.transform.Find("Icon").GetComponent<Image>();
									CUIUtility.SetImageSprite(component, cUseable.GetIconPath(), null, true, false, false, false);
									Transform transform2 = transform.transform.Find("Text");
									if (transform2 != null)
									{
										Text component2 = transform2.gameObject.GetComponent<Text>();
										if (component2 != null)
										{
											component2.set_text(cUseable.m_name);
										}
									}
									Transform transform3 = transform.transform.Find("TextNum");
									if (transform3 != null)
									{
										Text component3 = transform3.GetComponent<Text>();
										if (cUseable.m_stackCount < 10000)
										{
											component3.set_text(cUseable.m_stackCount.ToString());
										}
										else
										{
											component3.set_text(cUseable.m_stackCount / 10000 + "万");
										}
									}
									Transform transform4 = itemCell.transform.Find("ExperienceCard");
									if (transform4 != null)
									{
										if (cUseable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP && CItem.IsHeroExperienceCard(cUseable.m_baseID))
										{
											transform4.gameObject.CustomSetActive(true);
											transform4.GetComponent<Image>().SetSprite(CUIUtility.GetSpritePrefeb(CExperienceCardSystem.HeroExperienceCardMarkPath, false, false), false);
										}
										else if (cUseable.m_type == COM_ITEM_TYPE.COM_OBJTYPE_ITEMPROP && CItem.IsSkinExperienceCard(cUseable.m_baseID))
										{
											transform4.gameObject.CustomSetActive(true);
											transform4.GetComponent<Image>().SetSprite(CUIUtility.GetSpritePrefeb(CExperienceCardSystem.SkinExperienceCardMarkPath, false, false), false);
										}
										else
										{
											transform4.gameObject.CustomSetActive(false);
										}
									}
									num++;
								}
								CUIEventScript cUIEventScript = transform.GetComponent<CUIEventScript>();
								if (cUIEventScript == null)
								{
									cUIEventScript = transform.gameObject.AddComponent<CUIEventScript>();
								}
								stUIEventParams eventParams = new stUIEventParams
								{
									iconUseable = cUseable
								};
								cUIEventScript.SetUIEvent(enUIEventType.Down, enUIEventID.Tips_ItemInfoOpen, eventParams);
								cUIEventScript.SetUIEvent(enUIEventType.HoldEnd, enUIEventID.Tips_ItemInfoClose, eventParams);
								cUIEventScript.SetUIEvent(enUIEventType.Click, enUIEventID.Tips_ItemInfoClose, eventParams);
								cUIEventScript.SetUIEvent(enUIEventType.DragEnd, enUIEventID.Tips_ItemInfoClose, eventParams);
							}
						}
					}
				}
			}
			for (int j = num; j <= num2; j++)
			{
				Transform transform5 = itemCell.transform.Find("Week" + j);
				if (transform5 != null)
				{
					transform5.gameObject.CustomSetActive(false);
				}
			}
		}

		private static int ComparebyShowIdx(ResNobeInfo info1, ResNobeInfo info2)
		{
			if (info1.bShowIdx > info2.bShowIdx)
			{
				return 1;
			}
			if (info1.bShowIdx == info2.bShowIdx)
			{
				return 0;
			}
			return -1;
		}

		private void UpdateGrowInfo()
		{
			if (this.m_vipInfo == null)
			{
				return;
			}
			Image component = this.m_Form.GetWidget(16).GetComponent<Image>();
			GameObject widget = this.m_Form.GetWidget(2);
			Image component2 = this.m_Form.GetWidget(6).GetComponent<Image>();
			int dwCurLevel = (int)this.m_vipInfo.stGameVipClient.dwCurLevel;
			int dwScore = (int)this.m_vipInfo.stGameVipClient.dwScore;
			int num = GameDataMgr.resVipDianQuan.Count();
			GameObject gameObject = null;
			if (widget)
			{
				Transform transform = widget.transform.Find("pnlHead/mvp/Img_Bg1/tips");
				if (transform)
				{
					gameObject = transform.gameObject;
					gameObject.CustomSetActive(false);
				}
			}
			GameObject gameObject2 = null;
			if (widget)
			{
				Transform transform2 = widget.transform.Find("pnlHead/mvp/LevelLeft/level");
				if (transform2)
				{
					gameObject2 = transform2.gameObject;
				}
			}
			string text = string.Empty;
			GameObject widget2 = this.m_Form.GetWidget(23);
			widget2.CustomSetActive(false);
			if (dwCurLevel >= num)
			{
				text = "贵族等级已满";
				string prefabPath = string.Format("{0}{1}.prefab", CUIUtility.s_Sprite_System_ShareUI_Dir, dwCurLevel.ToString());
				CUIUtility.SetImageSprite(component, prefabPath, null, true, false, false, false);
				if (gameObject)
				{
					gameObject.CustomSetActive(true);
					gameObject.GetComponent<Text>().set_text(text);
				}
				if (gameObject2)
				{
					gameObject2.GetComponent<Image>().set_fillAmount(1f);
				}
			}
			else
			{
				int num2 = dwCurLevel + 1;
				ResVIPCoupons dataByKey = GameDataMgr.resVipDianQuan.GetDataByKey((long)num2);
				if (dataByKey != null)
				{
					int num3 = (int)(dataByKey.dwConsumeCoupons - (uint)dwScore);
					if (num3 > 0)
					{
						this.UpdateImageLevelTips(widget2, num2, num3, false);
						string prefabPath2 = string.Format("{0}{1}.prefab", CUIUtility.s_Sprite_System_ShareUI_Dir, dwCurLevel.ToString());
						CUIUtility.SetImageSprite(component, prefabPath2, null, true, false, false, false);
						if (gameObject2)
						{
							gameObject2.GetComponent<Image>().set_fillAmount(1f * (float)dwScore / dataByKey.dwConsumeCoupons);
						}
						component2.gameObject.CustomSetActive(true);
					}
					else
					{
						widget2.CustomSetActive(false);
						int dwMaxLevel = (int)this.m_vipInfo.dwMaxLevel;
						ResVIPCoupons dataByKey2 = GameDataMgr.resVipDianQuan.GetDataByKey((long)dwMaxLevel);
						text = "只要消费任意金额的点券，即可恢复到之前的最高等级";
						if (gameObject2)
						{
							gameObject2.GetComponent<Image>().set_fillAmount(1f * (float)dwScore / dataByKey.dwConsumeCoupons);
						}
						string prefabPath3 = string.Format("{0}{1}.prefab", CUIUtility.s_Sprite_System_ShareUI_Dir, dwCurLevel.ToString());
						CUIUtility.SetImageSprite(component, prefabPath3, null, true, false, false, false);
						component2.gameObject.CustomSetActive(true);
						if (gameObject)
						{
							gameObject.CustomSetActive(true);
							gameObject.GetComponent<Text>().set_text(text);
						}
					}
				}
			}
			CUIListScript component3 = this.m_Form.GetWidget(4).GetComponent<CUIListScript>();
			if (component3 == null)
			{
				return;
			}
			int num4 = num / 5;
			float num5 = (float)num * 1f / 5f;
			if (num5 - (float)num4 > 0f)
			{
				num4++;
			}
			component3.SetElementAmount(num4);
			DictionaryView<int, ListView<ResVIPCoupons>> dictionaryView = new DictionaryView<int, ListView<ResVIPCoupons>>();
			for (int i = 1; i <= GameDataMgr.resVipDianQuan.count; i++)
			{
				ResVIPCoupons dataByKey3 = GameDataMgr.resVipDianQuan.GetDataByKey((long)i);
				int key = (i - 1) / 5;
				if (!dictionaryView.ContainsKey(key))
				{
					dictionaryView.Add(key, new ListView<ResVIPCoupons>
					{
						dataByKey3
					});
				}
				else
				{
					ListView<ResVIPCoupons> listView = dictionaryView[key];
					listView.Add(dataByKey3);
				}
			}
			for (int j = 0; j < num4; j++)
			{
				CUIListElementScript elemenet = component3.GetElemenet(j);
				if (dictionaryView.ContainsKey(j))
				{
					ListView<ResVIPCoupons> listView2 = dictionaryView[j];
					for (int k = 0; k < 5; k++)
					{
						GameObject gameObject3 = null;
						Transform transform3 = elemenet.transform.Find("itemCell" + k);
						if (transform3 != null)
						{
							gameObject3 = transform3.gameObject;
						}
						if (k < listView2.Count && gameObject3 != null)
						{
							ResVIPCoupons resVIPCoupons = listView2[k];
							gameObject3.CustomSetActive(true);
							Text component4 = gameObject3.transform.Find("Text").GetComponent<Text>();
							component4.set_text(string.Format("<size=24><color=#f3CA4Dff>{0}</color></size><size=18><color=#7e88a2ff>积分</color></size>", resVIPCoupons.dwConsumeCoupons.ToString()));
							Image component5 = gameObject3.transform.Find("imgIcon/Img_Bg1").GetComponent<Image>();
							string prefabPath4 = string.Format("{0}{1}.prefab", CUIUtility.s_Sprite_System_ShareUI_Dir, resVIPCoupons.dwVIPLevel);
							CUIUtility.SetImageSprite(component5, prefabPath4, null, true, false, false, false);
						}
						else if (gameObject3 != null)
						{
							gameObject3.CustomSetActive(false);
						}
					}
				}
			}
			this.UpdateGrowInfoLevel(0);
			if (component3)
			{
				this.m_CurPageLevel = 0;
				component3.MoveElementInScrollArea(this.m_CurPageLevel, true);
			}
			if (component3)
			{
				Transform transform4 = component3.transform.Find("ScrollRect");
				if (transform4)
				{
					ScrollRect component6 = transform4.GetComponent<ScrollRect>();
					if (component6)
					{
						component6.set_horizontal(false);
						component6.set_vertical(false);
					}
				}
			}
		}

		private void UpdateImageLevelTips(GameObject parent, int level, int resPoint, bool bdiamondForm = false)
		{
			parent.CustomSetActive(true);
			Transform[] array = new Transform[5];
			for (int i = 0; i < 5; i++)
			{
				array[i] = parent.transform.Find("n" + i);
				array[i].gameObject.CustomSetActive(false);
				if (bdiamondForm)
				{
					array[i].GetComponent<RectTransform>().anchoredPosition = this.m_NumPosBackDiamondForm[i];
				}
				else
				{
					array[i].GetComponent<RectTransform>().anchoredPosition = this.m_NumPosBack[i];
				}
			}
			int length = resPoint.ToString().get_Length();
			int num = 5 - length;
			if (num < 0)
			{
				return;
			}
			string[] array2 = new string[5];
			string text = resPoint.ToString();
			for (int j = 0; j < num; j++)
			{
				array2[j] = "-1";
			}
			for (int k = num; k < 5; k++)
			{
				array2[k] = text.Substring(k - num, 1);
			}
			float num2 = 0f;
			for (int l = 0; l < 5; l++)
			{
				if (array2[l] == "-1")
				{
					num2 += Mathf.Abs(array[l].GetComponent<RectTransform>().anchoredPosition.x - array[l + 1].GetComponent<RectTransform>().anchoredPosition.x);
					array[l].gameObject.CustomSetActive(false);
				}
				else
				{
					array[l].gameObject.CustomSetActive(true);
					string prefabPath = string.Format("{0}{1}.prefab", CUIUtility.s_Sprite_System_ShareUI_Dir, array2[l].ToString());
					CUIUtility.SetImageSprite(array[l].gameObject.GetComponent<Image>(), prefabPath, null, true, false, false, false);
					array[l].GetComponent<RectTransform>().anchoredPosition = new Vector2(array[l].GetComponent<RectTransform>().anchoredPosition.x - num2, array[l].GetComponent<RectTransform>().anchoredPosition.y);
				}
			}
			Transform transform = parent.transform.Find("tipsRight");
			if (bdiamondForm)
			{
				transform.GetComponent<RectTransform>().anchoredPosition = this.m_NumPosBackDiamondForm[5];
			}
			else
			{
				transform.GetComponent<RectTransform>().anchoredPosition = this.m_NumPosBack[5];
			}
			Transform transform2 = parent.transform.Find("tipsRight/vipLevel");
			string prefabPath2 = string.Format("{0}{1}.prefab", CUIUtility.s_Sprite_System_ShareUI_Dir, level.ToString());
			CUIUtility.SetImageSprite(transform2.gameObject.GetComponent<Image>(), prefabPath2, null, true, false, false, false);
			transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(transform.GetComponent<RectTransform>().anchoredPosition.x - num2, transform.GetComponent<RectTransform>().anchoredPosition.y);
		}

		private void OpenChangeHeadIconForm(CUIEvent uiEvent)
		{
			this.m_ChangeHeadIconForm = Singleton<CUIManager>.GetInstance().OpenForm(this.sNobeChangeHeadIconForm, false, true);
			int dwHeadIconId = (int)this.m_vipInfo.stGameVipClient.dwHeadIconId;
			this.UpdateCHangeHeadFormData(dwHeadIconId);
		}

		private void CloseChangeHeadIconForm(CUIEvent uiEvent)
		{
			Singleton<CUIManager>.GetInstance().CloseForm(this.m_ChangeHeadIconForm);
			this.m_ChangeHeadIconForm = null;
		}

		private void UpdateCHangeHeadFormData(int nHeadIDX)
		{
			if (this.m_ChangeHeadIconForm == null)
			{
				return;
			}
			int dwCurLevel = (int)this.m_vipInfo.stGameVipClient.dwCurLevel;
			ListView<ResNobeInfo> changeHeadIconResList = this.m_ChangeHeadIconResList;
			int count = changeHeadIconResList.Count;
			CUIListScript component = this.m_ChangeHeadIconForm.GetWidget(0).GetComponent<CUIListScript>();
			component.SetElementAmount(count);
			for (int i = 0; i < count; i++)
			{
				CUIListElementScript elemenet = component.GetElemenet(i);
				ResNobeInfo resNobeInfo = changeHeadIconResList[i];
				string text = Utility.UTF8Convert(resNobeInfo.szResIcon);
				string text2 = Utility.UTF8Convert(resNobeInfo.szResName);
				GameObject gameObject = elemenet.transform.Find("itemCell").gameObject;
				Image component2 = gameObject.transform.Find("imgIcon").GetComponent<Image>();
				string prefabPath = string.Format("{0}{1}.prefab", CUIUtility.s_Sprite_System_ShareUI_Dir, text);
				CUIUtility.SetImageSprite(component2, prefabPath, null, true, false, false, false);
				Text component3 = gameObject.transform.Find("Text").GetComponent<Text>();
				component3.set_text(text2);
				if ((ulong)resNobeInfo.dwNobeLevel <= (ulong)((long)dwCurLevel))
				{
					GameObject gameObject2 = gameObject.transform.Find("Btn/Btn").gameObject;
					GameObject gameObject3 = gameObject.transform.Find("Btn/Text").gameObject;
					if ((long)nHeadIDX == (long)((ulong)resNobeInfo.dwID))
					{
						gameObject3.CustomSetActive(true);
						string text3 = "已使用";
						gameObject3.GetComponent<Text>().set_text(text3);
						gameObject2.CustomSetActive(false);
					}
					else
					{
						gameObject3.CustomSetActive(false);
						gameObject2.CustomSetActive(true);
					}
				}
				else
				{
					GameObject gameObject4 = gameObject.transform.Find("Btn/Btn").gameObject;
					gameObject4.CustomSetActive(false);
					GameObject gameObject5 = gameObject.transform.Find("Btn/Text").gameObject;
					gameObject5.CustomSetActive(true);
					string text4 = string.Empty;
					if (i == 0)
					{
						text4 = "贵族1级解锁";
					}
					else if (i == 1)
					{
						text4 = "贵族4级解锁";
					}
					else if (i == 2)
					{
						text4 = "贵族7级解锁";
					}
					gameObject5.GetComponent<Text>().set_text(text4);
				}
			}
		}

		private ListView<ResNobeInfo> GetAllHeadIcon()
		{
			ListView<ResNobeInfo> listView = new ListView<ResNobeInfo>();
			for (int i = 0; i < GameDataMgr.resNobeInfoDatabin.count; i++)
			{
				ResNobeInfo dataByIndex = GameDataMgr.resNobeInfoDatabin.GetDataByIndex(i);
				if (dataByIndex != null && dataByIndex.bResType == 2)
				{
					listView.Add(dataByIndex);
				}
			}
			return listView;
		}

		public string GetNobleNameColorString(string playerName, int playerLevel)
		{
			playerLevel = 1;
			if (playerLevel > 0)
			{
				return string.Format("<color=#ff0000ff>{0}</color>", playerName);
			}
			return playerName;
		}

		public void SetNobeIcon(Image image, int level, bool useDefaultBackImage = false, bool isSelf = true, ulong privacyBits = 0uL)
		{
			if (image == null)
			{
				return;
			}
			GameObject gameObject;
			if (image.gameObject.name == "NobeIcon")
			{
				gameObject = image.gameObject;
			}
			else
			{
				gameObject = image.transform.FindChild("NobeIcon").gameObject;
			}
			if (gameObject == null)
			{
				return;
			}
			if (CSysDynamicBlock.bSocialBlocked)
			{
				gameObject.CustomSetActive(false);
				return;
			}
			if (CRoleInfo.IsPrivacyOpen(privacyBits, COM_USER_PRIVACY_MASK.COM_USER_PRIVACY_MASK_VIP_LEVEL))
			{
				gameObject.CustomSetActive(false);
				return;
			}
			Image component = gameObject.GetComponent<Image>();
			if (level <= 0)
			{
				component.gameObject.CustomSetActive(false);
			}
			else
			{
				component.gameObject.CustomSetActive(true);
				if (!useDefaultBackImage)
				{
					CUIUtility.SetImageSprite(component, this.m_iconPath, null, true, false, false, false);
				}
				Transform transform = component.transform.FindChild("text");
				if (transform != null)
				{
					transform.GetComponent<Text>().set_text(level.ToString());
				}
			}
		}

		public void SetHeadIconBk(Image image, int headIdx)
		{
			if (image == null)
			{
				return;
			}
			if (headIdx > 0)
			{
				ResHeadImage resHeadImage = Singleton<HeadIconSys>.instance.GetResHeadImage(headIdx);
				if (resHeadImage == null)
				{
					image.gameObject.CustomSetActive(false);
				}
				else
				{
					string text = string.Format("{0}{1}", CUIUtility.s_Sprite_Dynamic_Nobe_Dir, resHeadImage.szHeadIcon);
					if (text != string.Empty)
					{
						image.gameObject.CustomSetActive(true);
						CUIUtility.SetImageSprite(image, text, null, true, false, false, false);
					}
					while (image.transform.childCount > 0)
					{
						Transform child = image.transform.GetChild(0);
						child.parent = null;
						CUIParticleScript component = child.GetComponent<CUIParticleScript>();
						if (component != null && component.m_belongedFormScript != null)
						{
							component.m_belongedFormScript.RemoveUIComponent(component);
						}
						CUICommonSystem.DestoryObj(child.gameObject, 0.1f);
					}
				}
			}
			else
			{
				image.gameObject.CustomSetActive(false);
			}
		}

		public void SetHeadIconBkEffect(Image image, int headIdx, CUIFormScript form, float scale, bool useMask = false)
		{
			if (headIdx > 0)
			{
				ResHeadImage resHeadImage = Singleton<HeadIconSys>.instance.GetResHeadImage(headIdx);
				if (resHeadImage == null)
				{
					return;
				}
				string szEffect = resHeadImage.szEffect;
				if (!string.IsNullOrEmpty(szEffect) && form != null)
				{
					string prefabPath = string.Format("{0}{1}.prefeb", CUIUtility.s_Particle_Dir, szEffect);
					GameObject uIPrefab = CUICommonSystem.GetUIPrefab(prefabPath);
					if (uIPrefab == null)
					{
						return;
					}
					Transform transform = uIPrefab.transform;
					RectTransform component = uIPrefab.GetComponent<RectTransform>();
					if (transform != null)
					{
						transform.SetParent(image.transform);
						transform.localPosition = Vector3.zero;
						transform.localRotation = Quaternion.identity;
						transform.localScale = Vector3.one;
					}
					ParticleScaler component2 = uIPrefab.GetComponent<ParticleScaler>();
					if (component2 != null)
					{
						component2.particleScale = scale;
						component2.CheckAndApplyScale();
					}
					form.InitializeComponent(image.gameObject);
					CUIParticleScript component3 = uIPrefab.GetComponent<CUIParticleScript>();
					if (component3 != null)
					{
						component3.SetSortingOrder(form.GetSortingOrder());
					}
					if (!useMask)
					{
						ParticleSystem[] componentsInChildren = transform.GetComponentsInChildren<ParticleSystem>(true);
						ParticleSystem[] array = componentsInChildren;
						for (int i = 0; i < array.Length; i++)
						{
							ParticleSystem particleSystem = array[i];
							particleSystem.renderer.sharedMaterial = this.GetParticleMaterial(particleSystem.renderer.material);
						}
					}
				}
			}
		}

		public int GetSelfNobeLevel()
		{
			if (this.m_vipInfo != null)
			{
				return (int)this.m_vipInfo.stGameVipClient.dwCurLevel;
			}
			return 0;
		}

		public int GetSelfNobeScore()
		{
			if (this.m_vipInfo != null)
			{
				return (int)this.m_vipInfo.stGameVipClient.dwScore;
			}
			return 0;
		}

		public int GetSelfNobeHeadIdx()
		{
			if (this.m_vipInfo != null)
			{
				return (int)this.m_vipInfo.stGameVipClient.dwHeadIconId;
			}
			return 0;
		}

		public void SetMyQQVipHead(Image image)
		{
			if (image == null)
			{
				return;
			}
			GameObject gameObject = image.gameObject;
			if (gameObject == null)
			{
				return;
			}
			if (ApolloConfig.platform != ApolloPlatform.QQ && ApolloConfig.platform != ApolloPlatform.WTLogin)
			{
				gameObject.CustomSetActive(false);
			}
			if (Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().HasVip(16))
			{
				gameObject.CustomSetActive(true);
				CUIUtility.SetImageSprite(image, string.Format("{0}QQ_Svip_icon.prefab", "UGUI/Sprite/Common/"), null, true, false, false, false);
			}
			else if (Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().HasVip(1))
			{
				gameObject.CustomSetActive(true);
				CUIUtility.SetImageSprite(image, string.Format("{0}QQ_vip_icon.prefab", "UGUI/Sprite/Common/"), null, true, false, false, false);
			}
			else
			{
				gameObject.CustomSetActive(false);
			}
		}

		public void SetOtherQQVipHead(Image image, int bit)
		{
			if (image == null)
			{
				return;
			}
			GameObject gameObject = image.gameObject;
			if (gameObject == null)
			{
				return;
			}
			if (ApolloConfig.platform != ApolloPlatform.QQ && ApolloConfig.platform != ApolloPlatform.WTLogin)
			{
				gameObject.CustomSetActive(false);
				return;
			}
			if ((bit & 16) != 0)
			{
				gameObject.CustomSetActive(true);
				CUIUtility.SetImageSprite(image, string.Format("{0}QQ_Svip_icon.prefab", "UGUI/Sprite/Common/"), null, true, false, false, false);
			}
			else if ((bit & 1) != 0)
			{
				gameObject.CustomSetActive(true);
				CUIUtility.SetImageSprite(image, string.Format("{0}QQ_vip_icon.prefab", "UGUI/Sprite/Common/"), null, true, false, false, false);
			}
			else
			{
				gameObject.CustomSetActive(false);
			}
		}

		public void SetGameCenterVisible(GameObject obj, COM_PRIVILEGE_TYPE privilegeType, ApolloPlatform platform, bool platformForceShow = false, bool changeName = false, string privilegeName = "", string nonePrivilegeName = "")
		{
			if (obj == null)
			{
				return;
			}
			NobeSys.PrivilegeInfo gameCenterVisible = this.GetGameCenterVisible(privilegeType, platform, platformForceShow, changeName, privilegeName, nonePrivilegeName);
			obj.CustomSetActive(gameCenterVisible.isVisible);
			if (gameCenterVisible.isVisible && changeName)
			{
				Transform transform = obj.transform.Find("PlayerName");
				if (transform != null)
				{
					Text component = transform.GetComponent<Text>();
					if (component != null)
					{
						component.set_text(gameCenterVisible.name);
					}
				}
			}
		}

		public NobeSys.PrivilegeInfo GetGameCenterVisible(COM_PRIVILEGE_TYPE privilegeType, ApolloPlatform platform, bool platformForceShow = false, bool changeName = false, string privilegeName = "", string nonePrivilegeName = "")
		{
			NobeSys.PrivilegeInfo result = default(NobeSys.PrivilegeInfo);
			bool flag = false;
			switch (platform)
			{
			case ApolloPlatform.Wechat:
				if (Singleton<ApolloHelper>.GetInstance().CurPlatform != ApolloPlatform.Wechat)
				{
					result.isVisible = false;
					goto IL_109;
				}
				if (platformForceShow)
				{
					result.isVisible = true;
				}
				else if (privilegeType == COM_PRIVILEGE_TYPE.COM_PRIVILEGE_TYPE_WXGAMECENTER_LOGIN)
				{
					result.isVisible = true;
					flag = true;
				}
				else
				{
					result.isVisible = false;
				}
				goto IL_109;
			case ApolloPlatform.QQ:
				if (Singleton<ApolloHelper>.GetInstance().CurPlatform != ApolloPlatform.QQ)
				{
					result.isVisible = false;
					goto IL_109;
				}
				if (platformForceShow)
				{
					result.isVisible = true;
				}
				else if (privilegeType == COM_PRIVILEGE_TYPE.COM_PRIVILEGE_TYPE_QQGAMECENTER_LOGIN)
				{
					result.isVisible = true;
					flag = true;
				}
				else
				{
					result.isVisible = false;
				}
				goto IL_109;
			case ApolloPlatform.Guest:
				if (Singleton<ApolloHelper>.GetInstance().CurPlatform != ApolloPlatform.Guest)
				{
					result.isVisible = false;
					goto IL_109;
				}
				result.isVisible = true;
				flag = true;
				goto IL_109;
			}
			result.isVisible = false;
			IL_109:
			if (MonoSingleton<BannerImageSys>.GetInstance().IsWaifaBlockChannel())
			{
				result.isVisible = false;
			}
			if (result.isVisible && changeName)
			{
				if (flag)
				{
					result.name = privilegeName;
				}
				else
				{
					result.name = nonePrivilegeName;
				}
			}
			return result;
		}

		protected void ShowNobeLevelUpForm(SCPKG_GAME_VIP_NTF vipInfo)
		{
			uint dwCurLevel = vipInfo.stGameVipClient.dwCurLevel;
			string formPath = string.Format("{0}{1}", "UGUI/Form/System/", "IDIPNotice/Form_NobeLevelUp.prefab");
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(formPath, false, true);
			if (cUIFormScript != null)
			{
				cUIFormScript.m_formWidgets[0].GetComponent<Text>().set_text(dwCurLevel.ToString(CultureInfo.get_InvariantCulture()));
				cUIFormScript.m_formWidgets[1].GetComponent<Text>().set_text(NobeSys._lastNobeLv.ToString(CultureInfo.get_InvariantCulture()));
				cUIFormScript.m_formWidgets[2].GetComponent<Text>().set_text(dwCurLevel.ToString(CultureInfo.get_InvariantCulture()));
				this.SetNobePrivilege(dwCurLevel, cUIFormScript, 3);
				this.SetNobeGift(dwCurLevel, cUIFormScript);
			}
		}

		private void ShowNobeLoseTips(SCPKG_GAME_VIP_NTF vipInfo)
		{
			if (vipInfo == null)
			{
				return;
			}
			int dwCurLevel = (int)vipInfo.stGameVipClient.dwCurLevel;
			int dwMaxLevel = (int)vipInfo.dwMaxLevel;
			CS_GAMEVIP_NTF_TYPE dwNtfType = (CS_GAMEVIP_NTF_TYPE)vipInfo.dwNtfType;
			string text = string.Empty;
			if (dwNtfType == CS_GAMEVIP_NTF_TYPE.CS_GAMEVIP_NTF_LOSE)
			{
				text = "<size=28>尊贵的召唤师：</size>                               非常遗憾，由于您上个月未消费点券，您将暂时失去贵族身份，不过不用担心哦，只要您本月消费<color=#e7b135>任意金额</color>的点券，就能立即恢复到<color=#e7b135>贵族" + dwMaxLevel + "级，1点券</color>也可以哦！";
			}
			else
			{
				if (dwNtfType != CS_GAMEVIP_NTF_TYPE.CS_GAMEVIP_NTF_DEGRADE)
				{
					return;
				}
				text = string.Concat(new object[]
				{
					"<size=28>尊贵的召唤师：</size>                               非常遗憾，由于您上个月未消费点券，您的贵族等级下降为<color=#e7b135>贵族",
					dwCurLevel,
					"级</color>，不过不用担心哦，只要您本月消费<color=#e7b135>任意金额</color>的点券，就能立即恢复到<color=#e7b135>贵族",
					dwMaxLevel,
					"级，1点券</color>也可以哦！"
				});
			}
			string formPath = string.Format("{0}{1}", "UGUI/Form/System/", "IDIPNotice/Form_NobeTip.prefab");
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(formPath, false, true);
			if (cUIFormScript != null)
			{
				Text component = cUIFormScript.GetWidget(0).GetComponent<Text>();
				if (component)
				{
					component.set_text(text);
				}
				this.SetNobePrivilege((uint)dwMaxLevel, cUIFormScript, 1);
				Text component2 = cUIFormScript.GetWidget(2).GetComponent<Text>();
				if (component2)
				{
					component2.set_text(string.Format("贵族{0}特权", dwMaxLevel));
				}
			}
		}

		private void SetNobeGift(uint curLv, CUIFormScript formScript)
		{
			int num = 1;
			for (int i = 1; i <= GameDataMgr.resNobeInfoDatabin.count; i++)
			{
				ResNobeInfo dataByIndex = GameDataMgr.resNobeInfoDatabin.GetDataByIndex(i - 1);
				if (dataByIndex != null && dataByIndex.bResType == 3 && dataByIndex.dwNobeLevel == curLv)
				{
					Transform transform = formScript.m_formWidgets[4].transform.FindChild(string.Format("Gift{0}", num));
					if (transform != null)
					{
						transform.gameObject.CustomSetActive(true);
						Transform transform2 = transform.FindChild(string.Format("Icon", new object[0]));
						Transform transform3 = transform.FindChild(string.Format("Text", new object[0]));
						Transform transform4 = transform.FindChild(string.Format("ExperienceCard", new object[0]));
						Transform transform5 = transform.FindChild(string.Format("TextNum", new object[0]));
						transform4.gameObject.CustomSetActive(dataByIndex.bShowIdx == 1);
						transform5.gameObject.GetComponent<Text>().set_text((dataByIndex.dwJiaoBiaoNum > 0u) ? dataByIndex.dwJiaoBiaoNum.ToString(CultureInfo.get_InvariantCulture()) : null);
						string prefabPath = string.Format("{0}{1}.prefab", CUIUtility.s_Sprite_Dynamic_Icon_Dir, Utility.UTF8Convert(dataByIndex.szResIcon));
						if (transform2 != null && transform3 != null)
						{
							transform2.gameObject.GetComponent<Image>().SetSprite(prefabPath, formScript, true, false, false, false);
							transform3.gameObject.GetComponent<Text>().set_text(Utility.UTF8Convert(dataByIndex.szResName));
							num++;
						}
					}
				}
			}
			for (int j = num; j <= 3; j++)
			{
				Transform transform6 = formScript.m_formWidgets[4].transform.FindChild(string.Format("Gift{0}", j));
				if (transform6 != null)
				{
					transform6.gameObject.CustomSetActive(false);
				}
			}
		}

		private void SetNobePrivilege(uint curLv, CUIFormScript formScript, int widgetID)
		{
			int num = 1;
			int num2 = 0;
			int num3 = 100;
			for (int i = 0; i < 4; i++)
			{
				this.tempIndex[i] = 998u;
				this.tempStr[i] = null;
				this.tempName[i] = null;
			}
			for (int j = 1; j <= GameDataMgr.resNobeInfoDatabin.count; j++)
			{
				ResNobeInfo dataByIndex = GameDataMgr.resNobeInfoDatabin.GetDataByIndex(j - 1);
				if (dataByIndex != null && dataByIndex.bResType == 1)
				{
					if (dataByIndex.dwNobeLevel > curLv)
					{
						break;
					}
					if ((j == 4 || j == 2 || j == 5) && j > num2)
					{
						num2 = j;
						num3 = Math.Min(num, num3);
						string text = string.Format("{0}{1}.prefab", CUIUtility.s_Sprite_System_ShareUI_Dir, Utility.UTF8Convert(dataByIndex.szResIcon));
						this.tempIndex[num3 - 1] = (uint)dataByIndex.bShowIdx;
						this.tempStr[num3 - 1] = text;
						this.tempName[num3 - 1] = dataByIndex.szResName;
						if (num <= num3)
						{
							num++;
						}
					}
					else if (j != 4 && j != 2 && j != 5)
					{
						string text2 = string.Format("{0}{1}.prefab", CUIUtility.s_Sprite_System_ShareUI_Dir, Utility.UTF8Convert(dataByIndex.szResIcon));
						this.tempIndex[num - 1] = (uint)dataByIndex.bShowIdx;
						this.tempStr[num - 1] = text2;
						this.tempName[num - 1] = dataByIndex.szResName;
						num++;
					}
				}
			}
			for (int k = 3; k >= 1; k--)
			{
				for (int l = k - 1; l >= 0; l--)
				{
					if (this.tempIndex[k] < this.tempIndex[l])
					{
						string text3 = this.tempStr[l];
						string text4 = this.tempName[l];
						uint num4 = this.tempIndex[l];
						this.tempStr[l] = this.tempStr[k];
						this.tempName[l] = this.tempName[k];
						this.tempIndex[l] = this.tempIndex[k];
						this.tempStr[k] = text3;
						this.tempName[k] = text4;
						this.tempIndex[k] = num4;
					}
				}
			}
			for (int m = 0; m < num - 1; m++)
			{
				Transform transform = formScript.m_formWidgets[widgetID].transform.FindChild(string.Format("TeQuan{0}", m + 1));
				if (transform != null)
				{
					transform.gameObject.CustomSetActive(true);
					Transform transform2 = transform.FindChild(string.Format("Icon", new object[0]));
					Transform transform3 = transform.FindChild(string.Format("Text", new object[0]));
					transform2.gameObject.GetComponent<Image>().SetSprite(this.tempStr[m], formScript, true, false, false, false);
					transform3.gameObject.GetComponent<Text>().set_text(Utility.UTF8Convert(this.tempName[m]));
				}
			}
			for (int n = num; n <= 4; n++)
			{
				Transform transform4 = formScript.m_formWidgets[widgetID].transform.FindChild(string.Format("TeQuan{0}", n));
				if (transform4 != null)
				{
					transform4.gameObject.CustomSetActive(false);
				}
			}
		}
	}
}
