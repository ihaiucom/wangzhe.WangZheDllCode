using Assets.Scripts.Framework;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class CFunctionUnlockSys : Singleton<CFunctionUnlockSys>
	{
		public static readonly string FUC_UNLOCK_FORM_PATH = "UGUI/Form/System/FunctionUnlock/Form_FucUnlockTip.prefab";

		private ulong m_tipUnlockMask;

		private byte ChapterUnlockMask
		{
			get
			{
				return (byte)(this.m_tipUnlockMask >> 56);
			}
		}

		public override void Init()
		{
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.FucUnlock_TimerUp, new CUIEventManager.OnUIEventHandler(this.OnUnlockTipsTimeUp));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.FucUnlock_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnCloseUnlockTip));
			Singleton<EventRouter>.instance.AddEventHandler("FucUnlockConditionChanged", new Action(this.UnlockConditionChanged));
			base.Init();
		}

		public override void UnInit()
		{
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.FucUnlock_TimerUp, new CUIEventManager.OnUIEventHandler(this.OnUnlockTipsTimeUp));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.FucUnlock_CloseForm, new CUIEventManager.OnUIEventHandler(this.OnCloseUnlockTip));
			Singleton<EventRouter>.instance.RemoveEventHandler("FucUnlockConditionChanged", new Action(this.UnlockConditionChanged));
			base.UnInit();
		}

		public bool FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE type)
		{
			ResSpecialFucUnlock dataByKey = GameDataMgr.specialFunUnlockDatabin.GetDataByKey((uint)type);
			if (dataByKey == null)
			{
				return true;
			}
			if (dataByKey.bIsAnd == 1)
			{
				for (int i = 0; i < dataByKey.UnlockArray.Length; i++)
				{
					uint num = dataByKey.UnlockArray[i];
					if (num > 0u && !this.CheckUnlock(num))
					{
						return false;
					}
				}
				return true;
			}
			bool flag = false;
			for (int j = 0; j < dataByKey.UnlockArray.Length; j++)
			{
				uint num2 = dataByKey.UnlockArray[j];
				if (num2 > 0u)
				{
					flag = true;
				}
				if (num2 > 0u && this.CheckUnlock(num2))
				{
					return true;
				}
			}
			return !flag;
		}

		public uint[] GetConditionParam(RES_SPECIALFUNCUNLOCK_TYPE type, RES_UNLOCKCONDITION_TYPE conditionType)
		{
			uint[] result = new uint[0];
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			ResSpecialFucUnlock dataByKey = GameDataMgr.specialFunUnlockDatabin.GetDataByKey((uint)type);
			if (dataByKey != null && masterRoleInfo != null)
			{
				for (int i = 0; i < dataByKey.UnlockArray.Length; i++)
				{
					uint num = dataByKey.UnlockArray[i];
					if (num > 0u)
					{
						ResUnlockCondition dataByKey2 = GameDataMgr.unlockConditionDatabin.GetDataByKey(num);
						if (dataByKey2 != null && (RES_UNLOCKCONDITION_TYPE)dataByKey2.wUnlockType == conditionType)
						{
							return dataByKey2.UnlockParam;
						}
					}
				}
			}
			return result;
		}

		public bool IsTypeHasCondition(RES_SPECIALFUNCUNLOCK_TYPE type)
		{
			ResSpecialFucUnlock dataByKey = GameDataMgr.specialFunUnlockDatabin.GetDataByKey((uint)type);
			if (dataByKey == null)
			{
				return false;
			}
			for (int i = 0; i < dataByKey.UnlockArray.Length; i++)
			{
				uint num = dataByKey.UnlockArray[i];
				if (num > 0u)
				{
					return true;
				}
			}
			return false;
		}

		public bool CheckUnlock(uint id)
		{
			bool result = false;
			ResUnlockCondition dataByKey = GameDataMgr.unlockConditionDatabin.GetDataByKey(id);
			DebugHelper.Assert(dataByKey != null, "ResUnlockCondition[{0}] can not be find.", new object[]
			{
				id
			});
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			DebugHelper.Assert(masterRoleInfo != null, "roleinfo can't be null in CheckUnlock");
			if (masterRoleInfo != null && dataByKey != null)
			{
				switch (dataByKey.wUnlockType)
				{
				case 1:
					result = (masterRoleInfo.PvpLevel >= dataByKey.UnlockParam[0]);
					break;
				case 2:
				{
					int num = (int)dataByKey.UnlockParam[0];
					int num2 = (int)dataByKey.UnlockParam[1];
					int num3 = 0;
					ResLevelCfgInfo dataByKey2 = GameDataMgr.levelDatabin.GetDataByKey((long)num);
					DebugHelper.Assert(dataByKey2 != null, "can't find level = {0}", new object[]
					{
						num
					});
					if (dataByKey2 != null && dataByKey2 != null && masterRoleInfo.pveLevelDetail[num3] != null && masterRoleInfo.pveLevelDetail[num3].ChapterDetailList[dataByKey2.iChapterId - 1] != null)
					{
						PVE_CHAPTER_COMPLETE_INFO pVE_CHAPTER_COMPLETE_INFO = masterRoleInfo.pveLevelDetail[num3].ChapterDetailList[dataByKey2.iChapterId - 1];
						for (int i = 0; i < pVE_CHAPTER_COMPLETE_INFO.LevelDetailList.Length; i++)
						{
							PVE_LEVEL_COMPLETE_INFO pVE_LEVEL_COMPLETE_INFO = pVE_CHAPTER_COMPLETE_INFO.LevelDetailList[i];
							if (pVE_LEVEL_COMPLETE_INFO != null && pVE_LEVEL_COMPLETE_INFO.iLevelID == num)
							{
								bool arg_17A_0 = pVE_LEVEL_COMPLETE_INFO.levelStatus == 1 && num2 <= CAdventureSys.GetStarNum(pVE_LEVEL_COMPLETE_INFO.bStarBits);
								break;
							}
						}
					}
					result = true;
					break;
				}
				case 4:
				{
					uint num4 = 0u;
					if (masterRoleInfo.pvpDetail != null)
					{
						num4 = masterRoleInfo.pvpDetail.stOneVsOneInfo.dwTotalNum + masterRoleInfo.pvpDetail.stTwoVsTwoInfo.dwTotalNum + masterRoleInfo.pvpDetail.stThreeVsThreeInfo.dwTotalNum;
					}
					result = (dataByKey.UnlockParam[0] <= num4 && dataByKey.UnlockParam[1] <= masterRoleInfo.PvpLevel);
					break;
				}
				}
			}
			return result;
		}

		public void UnlockConditionChanged()
		{
			if (!(Singleton<GameStateCtrl>.instance.GetCurrentState() is LobbyState))
			{
				return;
			}
			int num = 1;
			int num2 = 29;
			for (int i = num; i < num2; i++)
			{
				ResSpecialFucUnlock dataByKey = GameDataMgr.specialFunUnlockDatabin.GetDataByKey((uint)i);
				RES_SPECIALFUNCUNLOCK_TYPE rES_SPECIALFUNCUNLOCK_TYPE = (RES_SPECIALFUNCUNLOCK_TYPE)i;
				if (dataByKey != null && !this.TipsHasShow(rES_SPECIALFUNCUNLOCK_TYPE) && dataByKey.bIsShowUnlockTip == 1 && this.FucIsUnlock(rES_SPECIALFUNCUNLOCK_TYPE))
				{
					if (this.IsBottomBtn(rES_SPECIALFUNCUNLOCK_TYPE))
					{
						Singleton<CLobbySystem>.instance.Play_UnLock_Animation(rES_SPECIALFUNCUNLOCK_TYPE);
						this.SetUnlockTipsMask(rES_SPECIALFUNCUNLOCK_TYPE);
						this.ReqUnlockTipsMask();
						return;
					}
					this.SetUnlockTipsMask(rES_SPECIALFUNCUNLOCK_TYPE);
					this.ReqUnlockTipsMask();
				}
			}
		}

		private bool IsBottomBtn(RES_SPECIALFUNCUNLOCK_TYPE type)
		{
			return type == RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_HERO || type == RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_SYMBOL || type == RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_BAG || type == RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_TASK || type == RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_UNION || type == RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_FRIEND || type == RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_ADDEDSKILL || type == RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_ACHIEVEMENT;
		}

		public void OpenUnlockTip(string tips, string icon)
		{
			CUIFormScript cUIFormScript = Singleton<CUIManager>.instance.OpenForm(CFunctionUnlockSys.FUC_UNLOCK_FORM_PATH, false, false);
			if (cUIFormScript == null)
			{
				return;
			}
			Text component = cUIFormScript.transform.FindChild("TipContentTxt").GetComponent<Text>();
			Image component2 = cUIFormScript.transform.FindChild("TipIconImg").GetComponent<Image>();
			cUIFormScript.GetComponent<CUIEventScript>().enabled = false;
			component.text = tips;
			component2.SetSprite(CUIUtility.s_Sprite_Dynamic_FucUnlock_Dir + icon, cUIFormScript, true, false, false, false);
			component2.SetNativeSize();
			component.gameObject.CustomSetActive(true);
		}

		public void OpenUnlockTip(RES_SPECIALFUNCUNLOCK_TYPE type)
		{
			ResSpecialFucUnlock dataByKey = GameDataMgr.specialFunUnlockDatabin.GetDataByKey((uint)type);
			DebugHelper.Assert(dataByKey != null);
			if (dataByKey == null)
			{
				return;
			}
			string tips = Utility.UTF8Convert(dataByKey.szUnlockTip);
			string icon = Utility.UTF8Convert(dataByKey.szUnlockTipIcon);
			this.OpenUnlockTip(tips, icon);
			CUIFormScript form = Singleton<CUIManager>.instance.GetForm(CFunctionUnlockSys.FUC_UNLOCK_FORM_PATH);
			form.GetComponent<CUITimerScript>().m_eventParams[1].tag = (int)type;
		}

		private void OnUnlockTipsTimeUp(CUIEvent cuiEvent)
		{
			this.OnCloseUnlockTip(null);
		}

		private void OnCloseUnlockTip(CUIEvent cuiEvent)
		{
			Singleton<CUIManager>.instance.CloseForm(CFunctionUnlockSys.FUC_UNLOCK_FORM_PATH);
			this.UnlockConditionChanged();
		}

		public void OnSetUnlockTipsMask(SCPKG_CMD_GAMELOGINRSP rsp)
		{
			this.m_tipUnlockMask = rsp.ullFuncUnlockFlag;
		}

		public bool TipsHasShow(RES_SPECIALFUNCUNLOCK_TYPE type)
		{
			ulong num = 1uL << (int)type;
			return (this.m_tipUnlockMask & num) > 0uL;
		}

		private void SetUnlockTipsMask(RES_SPECIALFUNCUNLOCK_TYPE type)
		{
			ulong num = 1uL << (int)type;
			this.m_tipUnlockMask |= num;
		}

		public void ReqUnlockTipsMask()
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1402u);
			cSPkg.stPkgData.stFuncUnlockReq.ullUnlockFlag = this.m_tipUnlockMask;
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		public bool ChapterIsUnlock(uint ChapterId)
		{
			ResChapterInfo dataByKey = GameDataMgr.chapterInfoDatabin.GetDataByKey(ChapterId);
			DebugHelper.Assert(dataByKey != null, "ChapterIsUnlock : ChapterId[{0}] can not be find.", new object[]
			{
				ChapterId
			});
			if (dataByKey != null)
			{
				uint dwUnlockLevel = dataByKey.dwUnlockLevel;
				return dwUnlockLevel <= 0u || this.CheckUnlock(dwUnlockLevel);
			}
			return false;
		}
	}
}
