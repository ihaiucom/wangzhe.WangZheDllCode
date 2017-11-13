using Assets.Scripts.Framework;
using CSProtocol;
using ResData;
using System;

namespace Assets.Scripts.GameSystem
{
	public class CFriendRelationship
	{
		public class FRConfig
		{
			public int piority = -1;

			public COM_INTIMACY_STATE state;

			public string cfgRelaStr;

			public FRConfig(int piority, COM_INTIMACY_STATE state, string cfgRelaStr)
			{
				this.piority = piority;
				this.state = state;
				this.cfgRelaStr = cfgRelaStr;
			}
		}

		public ListView<CFriendRelationship.FRConfig> frConfig_list = new ListView<CFriendRelationship.FRConfig>();

		private uint _initmacyLimitTime;

		private readonly ListView<CFR> m_cfrList = new ListView<CFR>();

		private ListView<RelationConfig> m_RelationConfig = new ListView<RelationConfig>();

		private COM_INTIMACY_STATE _state;

		public static CFriendRelationship FRData
		{
			get
			{
				return Singleton<CFriendContoller>.instance.model.FRData;
			}
		}

		public uint InitmacyLimitTime
		{
			get
			{
				if (this._initmacyLimitTime == 0u)
				{
					this._initmacyLimitTime = GameDataMgr.GetGlobeValue(RES_GLOBAL_CONF_TYPE.RES_GLOBAL_CONF_TYPE_INTIMACY_LIMITTIME);
				}
				return this._initmacyLimitTime;
			}
		}

		public string IntimRela_CD_CountDown
		{
			get;
			set;
		}

		public string IntimRela_Tips_ReceiveOtherReqRela
		{
			get;
			set;
		}

		public string IntimRela_Tips_Wait4TargetRspReqRela
		{
			get;
			set;
		}

		public string IntimRela_Tips_ReceiveOtherDelRela
		{
			get;
			set;
		}

		public string IntimRela_Tips_Wait4TargetRspDelRela
		{
			get;
			set;
		}

		public string IntimRela_Tips_SelectRelation
		{
			get;
			set;
		}

		public string IntimRela_Tips_MidText
		{
			get;
			set;
		}

		public string IntimRela_Tips_RelaHasDel
		{
			get;
			set;
		}

		public string IntimRela_Tips_OK
		{
			get;
			set;
		}

		public string IntimRela_Tips_Cancle
		{
			get;
			set;
		}

		public string IntimRela_DoFristChoise
		{
			get;
			set;
		}

		public string IntimRela_AleadyFristChoise
		{
			get;
			set;
		}

		public string IntimRela_ReselectRelation
		{
			get;
			set;
		}

		public string IntimRela_ReDelRelation
		{
			get;
			set;
		}

		public string IntimRela_EmptyDataText
		{
			get;
			set;
		}

		public void Clear()
		{
			for (int i = 0; i < this.m_cfrList.Count; i++)
			{
				CFR cFR = this.m_cfrList[i];
				if (cFR != null)
				{
					cFR.Clear();
				}
			}
			this.m_cfrList.Clear();
			this._initmacyLimitTime = 0u;
		}

		public ListView<CFR> GetList()
		{
			return this.m_cfrList;
		}

		public bool HasRedDot()
		{
			for (int i = 0; i < this.m_cfrList.Count; i++)
			{
				CFR cFR = this.m_cfrList[i];
				if (cFR != null && cFR.bRedDot)
				{
					return true;
				}
			}
			return false;
		}

		public void FindSetState(COM_INTIMACY_STATE target1, COM_INTIMACY_STATE target2, COM_INTIMACY_STATE newState)
		{
			for (int i = 0; i < this.m_cfrList.Count; i++)
			{
				CFR cFR = this.m_cfrList[i];
				if (cFR != null && (cFR.state == target1 || cFR.state == target2))
				{
					cFR.state = newState;
				}
			}
		}

		public void FindSetState(COM_INTIMACY_STATE target1, COM_INTIMACY_STATE newState)
		{
			for (int i = 0; i < this.m_cfrList.Count; i++)
			{
				CFR cFR = this.m_cfrList[i];
				if (cFR != null && cFR.state == target1 && !cFR.bReciveOthersRequest)
				{
					cFR.state = newState;
				}
			}
		}

		public CFR FindFrist(COM_INTIMACY_STATE state)
		{
			for (int i = 0; i < this.m_cfrList.Count; i++)
			{
				CFR cFR = this.m_cfrList[i];
				if (cFR != null && cFR.state == state)
				{
					return cFR;
				}
			}
			return null;
		}

		public bool IsRelaUseOut()
		{
			bool flag = this.FindFrist(COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY) != null || this.FindFrist(COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY_DENY) != null;
			bool flag2 = this.FindFrist(COM_INTIMACY_STATE.COM_INTIMACY_STATE_LOVER) != null || this.FindFrist(COM_INTIMACY_STATE.COM_INTIMACY_STATE_LOVER_DENY) != null;
			bool flag3 = this.FindFrist(COM_INTIMACY_STATE.COM_INTIMACY_STATE_SIDEKICK) != null || this.FindFrist(COM_INTIMACY_STATE.COM_INTIMACY_STATE_SIDEKICK_DENY) != null;
			bool flag4 = this.FindFrist(COM_INTIMACY_STATE.COM_INTIMACY_STATE_BESTIE) != null || this.FindFrist(COM_INTIMACY_STATE.COM_INTIMACY_STATE_BESTIE_DENY) != null;
			return flag && flag2 && flag3 && flag4;
		}

		public void ProcessFriendList(COMDT_ACNT_UNIQ uniq, COMDT_INTIMACY_DATA data)
		{
			if (uniq == null || data == null)
			{
				return;
			}
			byte bIntimacyState = data.bIntimacyState;
			CFR cfr = this.GetCfr(uniq.ullUid, uniq.dwLogicWorldId);
			if (cfr != null && cfr.state != COM_INTIMACY_STATE.COM_INTIMACY_STATE_NULL)
			{
				return;
			}
			if (CFR.GetCDDays(data.dwTerminateTime) != -1)
			{
				this.Add(uniq.ullUid, uniq.dwLogicWorldId, (COM_INTIMACY_STATE)bIntimacyState, COM_INTIMACY_RELATION_CHG_TYPE.COM_INTIMACY_RELATION_NULL, data.dwTerminateTime, false);
				return;
			}
			if (data.bIntimacyState == 0 && (uint)data.wIntimacyValue >= Singleton<CFriendContoller>.instance.model.GetIntimacyRequestValue())
			{
				this.Add(uniq.ullUid, uniq.dwLogicWorldId, COM_INTIMACY_STATE.COM_INTIMACY_STATE_VALUE_FULL, COM_INTIMACY_RELATION_CHG_TYPE.COM_INTIMACY_RELATION_NULL, data.dwTerminateTime, false);
				return;
			}
			if (bIntimacyState != 0)
			{
				this.Add(uniq.ullUid, uniq.dwLogicWorldId, (COM_INTIMACY_STATE)bIntimacyState, COM_INTIMACY_RELATION_CHG_TYPE.COM_INTIMACY_RELATION_NULL, data.dwTerminateTime, false);
			}
		}

		public int GetCount()
		{
			return this.m_cfrList.Count;
		}

		public void ProcessOtherRequest(CSDT_VERIFICATION_INFO data, bool bReceiveNtf = false)
		{
			this.Add(data.stFriendInfo.stUin.ullUid, data.stFriendInfo.stUin.dwLogicWorldId, (COM_INTIMACY_STATE)data.bIntimacyState, COM_INTIMACY_RELATION_CHG_TYPE.COM_INTIMACY_RELATION_ADD, 0u, bReceiveNtf);
		}

		public void Add(ulong ulluid, uint worldId, byte state, byte op, uint timeStamp, bool bReceiveNtf = false)
		{
			this.Add(ulluid, worldId, (COM_INTIMACY_STATE)state, (COM_INTIMACY_RELATION_CHG_TYPE)op, timeStamp, bReceiveNtf);
		}

		public void Add(ulong ulluid, uint worldId, COM_INTIMACY_STATE state, COM_INTIMACY_RELATION_CHG_TYPE op, uint timeStamp, bool bReceiveNtf = false)
		{
			CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
			if (masterRoleInfo != null && masterRoleInfo.playerUllUID == ulluid && (long)masterRoleInfo.logicWorldID == (long)((ulong)worldId))
			{
				return;
			}
			CFR cfr = this.GetCfr(ulluid, worldId);
			if (cfr == null)
			{
				this.m_cfrList.Add(new CFR(ulluid, worldId, state, op, timeStamp, bReceiveNtf));
			}
			else
			{
				cfr.SetData(ulluid, worldId, state, op, timeStamp, bReceiveNtf);
			}
			if (IntimacyRelationViewUT.IsRelaState(state))
			{
				COM_INTIMACY_STATE confirmState = IntimacyRelationViewUT.GetConfirmState(state);
				this.FindSetState(confirmState, COM_INTIMACY_STATE.COM_INTIMACY_STATE_VALUE_FULL);
			}
			Singleton<EventRouter>.GetInstance().BroadCastEvent("FRDataChange");
		}

		public void Remove(ulong ulluid, uint worldID)
		{
			int num = -1;
			for (int i = 0; i < this.m_cfrList.Count; i++)
			{
				CFR cFR = this.m_cfrList[i];
				if (cFR != null && cFR.ulluid == ulluid && cFR.worldID == worldID)
				{
					num = i;
				}
			}
			if (num != -1)
			{
				this.m_cfrList.RemoveAt(num);
				Singleton<EventRouter>.GetInstance().BroadCastEvent("FRDataChange");
			}
		}

		public CFR GetCfr(ulong ulluid, uint worldID)
		{
			for (int i = 0; i < this.m_cfrList.Count; i++)
			{
				CFR cFR = this.m_cfrList[i];
				if (cFR != null && cFR.ulluid == ulluid && cFR.worldID == worldID)
				{
					return cFR;
				}
			}
			return null;
		}

		public void ResetChoiseRelaState(ulong ulluid, uint worldID)
		{
			CFR cfr = this.GetCfr(ulluid, worldID);
			if (cfr != null)
			{
				cfr.choiseRelation = -1;
				cfr.bInShowChoiseRelaList = false;
			}
		}

		public void Sort()
		{
		}

		public int GetCandiRelationCount()
		{
			return 2;
		}

		public void LoadConfig()
		{
			this.frConfig_list.Clear();
			this.frConfig_list.Add(new CFriendRelationship.FRConfig(0, COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY, Singleton<CTextManager>.instance.GetText("IntimRela_Type_Gay")));
			this.frConfig_list.Add(new CFriendRelationship.FRConfig(1, COM_INTIMACY_STATE.COM_INTIMACY_STATE_LOVER, Singleton<CTextManager>.instance.GetText("IntimRela_Type_Lover")));
			this.frConfig_list.Add(new CFriendRelationship.FRConfig(2, COM_INTIMACY_STATE.COM_INTIMACY_STATE_SIDEKICK, Singleton<CTextManager>.instance.GetText("IntimRela_Type_Sidekick")));
			this.frConfig_list.Add(new CFriendRelationship.FRConfig(3, COM_INTIMACY_STATE.COM_INTIMACY_STATE_BESTIE, Singleton<CTextManager>.instance.GetText("IntimRela_Type_Besite")));
			this.IntimRela_CD_CountDown = Singleton<CTextManager>.instance.GetText("IntimRela_CD_CountDown");
			this.IntimRela_Tips_ReceiveOtherReqRela = Singleton<CTextManager>.instance.GetText("IntimRela_Tips_ReceiveOtherReqRela");
			this.IntimRela_Tips_Wait4TargetRspReqRela = Singleton<CTextManager>.instance.GetText("IntimRela_Tips_Wait4TargetRspReqRela");
			this.IntimRela_Tips_ReceiveOtherDelRela = Singleton<CTextManager>.instance.GetText("IntimRela_Tips_ReceiveOtherDelRela");
			this.IntimRela_Tips_Wait4TargetRspDelRela = Singleton<CTextManager>.instance.GetText("IntimRela_Tips_Wait4TargetRspDelRela");
			this.IntimRela_Tips_SelectRelation = Singleton<CTextManager>.instance.GetText("IntimRela_Tips_SelectRelation");
			this.IntimRela_Tips_MidText = Singleton<CTextManager>.instance.GetText("IntimRela_Tips_MidText");
			this.IntimRela_Tips_RelaHasDel = Singleton<CTextManager>.instance.GetText("IntimRela_Tips_RelaHasDel");
			this.IntimRela_Tips_OK = Singleton<CTextManager>.instance.GetText("IntimRela_Tips_OK");
			this.IntimRela_Tips_Cancle = Singleton<CTextManager>.instance.GetText("IntimRela_Tips_Cancle");
			this.IntimRela_DoFristChoise = Singleton<CTextManager>.instance.GetText("IntimRela_DoFristChoise");
			this.IntimRela_AleadyFristChoise = Singleton<CTextManager>.instance.GetText("IntimRela_AleadyFristChoise");
			this.IntimRela_ReselectRelation = Singleton<CTextManager>.instance.GetText("IntimRela_ReselectRelation");
			this.IntimRela_ReDelRelation = Singleton<CTextManager>.instance.GetText("IntimRela_ReDelRelation");
			this.IntimRela_EmptyDataText = Singleton<CTextManager>.instance.GetText("IntimRela_EmptyDataText");
			this.InitRelationCfg();
		}

		private void InitRelationCfg()
		{
			CTextManager instance = Singleton<CTextManager>.instance;
			RelationConfig relationConfig = new RelationConfig();
			relationConfig.state = COM_INTIMACY_STATE.COM_INTIMACY_STATE_GAY;
			relationConfig.IntimRela_Tips_AlreadyHas = instance.GetText("IntimRela_Tips_AlreadyHasGay");
			relationConfig.IntimRela_Type = instance.GetText("IntimRela_Type_Gay");
			relationConfig.IntimRela_Tips_SendRequestSuccess = instance.GetText("IntimRela_Tips_SendRequestGaySuccess");
			relationConfig.IntimRela_Tips_SendDelSuccess = instance.GetText("IntimRela_Tips_SendDelGaySuccess");
			relationConfig.IntimRela_Tips_DenyYourRequest = instance.GetText("IntimRela_Tips_DenyYourRequestGay");
			relationConfig.IntimRela_Tips_DenyYourDel = instance.GetText("IntimRela_Tips_DenyYourDelGay");
			relationConfig.IntimRela_TypeColor = instance.GetText("IntimRela_TypeColor_Gay");
			relationConfig.IntimRela_TypeColor_Prefix_Normal = instance.GetText("IntimRela_TypeColor_Gay_Prefix_Normal");
			relationConfig.IntimRela_TypeColor_Prefix_Max = instance.GetText("IntimRela_TypeColor_Gay_Prefix_Max");
			relationConfig.IntimacyShowLoad = instance.GetText("IntimacyShowLoadGay");
			relationConfig.iconLevel_1 = "UGUI/Sprite/Dynamic/Friend/FriendNormal";
			relationConfig.iconLevel_2 = "UGUI/Sprite/Dynamic/Friend/FriendHigh";
			relationConfig.iconLevel_3 = "UGUI/Sprite/Dynamic/Friend/FriendMax";
			this.m_RelationConfig.Add(relationConfig);
			relationConfig = new RelationConfig();
			relationConfig.state = COM_INTIMACY_STATE.COM_INTIMACY_STATE_LOVER;
			relationConfig.IntimRela_Tips_AlreadyHas = instance.GetText("IntimRela_Tips_AlreadyHasLover");
			relationConfig.IntimRela_Type = instance.GetText("IntimRela_Type_Lover");
			relationConfig.IntimRela_Tips_SendRequestSuccess = instance.GetText("IntimRela_Tips_SendRequestLoverSuccess");
			relationConfig.IntimRela_Tips_SendDelSuccess = instance.GetText("IntimRela_Tips_SendDelLoverSuccess");
			relationConfig.IntimRela_Tips_DenyYourRequest = instance.GetText("IntimRela_Tips_DenyYourRequestLover");
			relationConfig.IntimRela_Tips_DenyYourDel = instance.GetText("IntimRela_Tips_DenyYourDelLover");
			relationConfig.IntimRela_TypeColor = instance.GetText("IntimRela_TypeColor_Lover");
			relationConfig.IntimRela_TypeColor_Prefix_Normal = instance.GetText("IntimRela_TypeColor_Lover_Prefix_Normal");
			relationConfig.IntimRela_TypeColor_Prefix_Max = instance.GetText("IntimRela_TypeColor_Lover_Prefix_Max");
			relationConfig.IntimacyShowLoad = instance.GetText("IntimacyShowLoadLover");
			relationConfig.iconLevel_1 = "UGUI/Sprite/Dynamic/Friend/LoverNormal";
			relationConfig.iconLevel_2 = "UGUI/Sprite/Dynamic/Friend/LoverHigh";
			relationConfig.iconLevel_3 = "UGUI/Sprite/Dynamic/Friend/LoverMax";
			this.m_RelationConfig.Add(relationConfig);
			relationConfig = new RelationConfig();
			relationConfig.state = COM_INTIMACY_STATE.COM_INTIMACY_STATE_SIDEKICK;
			relationConfig.IntimRela_Tips_AlreadyHas = instance.GetText("IntimRela_Tips_AlreadyHasSidekick");
			relationConfig.IntimRela_Type = instance.GetText("IntimRela_Type_Sidekick");
			relationConfig.IntimRela_Tips_SendRequestSuccess = instance.GetText("IntimRela_Tips_SendRequestSidekickSuccess");
			relationConfig.IntimRela_Tips_SendDelSuccess = instance.GetText("IntimRela_Tips_SendDelSidekickSuccess");
			relationConfig.IntimRela_Tips_DenyYourRequest = instance.GetText("IntimRela_Tips_DenyYourRequestSidekick");
			relationConfig.IntimRela_Tips_DenyYourDel = instance.GetText("IntimRela_Tips_DenyYourDelSidekick");
			relationConfig.IntimRela_TypeColor = instance.GetText("IntimRela_TypeColor_Sidekick");
			relationConfig.IntimRela_TypeColor_Prefix_Normal = instance.GetText("IntimRela_TypeColor_Sidekick_Prefix_Normal");
			relationConfig.IntimRela_TypeColor_Prefix_Max = instance.GetText("IntimRela_TypeColor_Sidekick_Prefix_Max");
			relationConfig.IntimacyShowLoad = instance.GetText("IntimacyShowLoadSideKick");
			relationConfig.iconLevel_1 = "UGUI/Sprite/Dynamic/Friend/SidekickNormal";
			relationConfig.iconLevel_2 = "UGUI/Sprite/Dynamic/Friend/SidekickHigh";
			relationConfig.iconLevel_3 = "UGUI/Sprite/Dynamic/Friend/SidekickMax";
			this.m_RelationConfig.Add(relationConfig);
			relationConfig = new RelationConfig();
			relationConfig.state = COM_INTIMACY_STATE.COM_INTIMACY_STATE_BESTIE;
			relationConfig.IntimRela_Tips_AlreadyHas = instance.GetText("IntimRela_Tips_AlreadyHasBesite");
			relationConfig.IntimRela_Type = instance.GetText("IntimRela_Type_Besite");
			relationConfig.IntimRela_Tips_SendRequestSuccess = instance.GetText("IntimRela_Tips_SendRequestBesiteSuccess");
			relationConfig.IntimRela_Tips_SendDelSuccess = instance.GetText("IntimRela_Tips_SendDelBesiteSuccess");
			relationConfig.IntimRela_Tips_DenyYourRequest = instance.GetText("IntimRela_Tips_DenyYourRequestBesite");
			relationConfig.IntimRela_Tips_DenyYourDel = instance.GetText("IntimRela_Tips_DenyYourDelBesite");
			relationConfig.IntimRela_TypeColor = instance.GetText("IntimRela_TypeColor_Besite");
			relationConfig.IntimRela_TypeColor_Prefix_Normal = instance.GetText("IntimRela_TypeColor_Besite_Prefix_Normal");
			relationConfig.IntimRela_TypeColor_Prefix_Max = instance.GetText("IntimRela_TypeColor_Besite_Prefix_Max");
			relationConfig.IntimacyShowLoad = instance.GetText("IntimacyShowLoadBesite");
			relationConfig.iconLevel_1 = "UGUI/Sprite/Dynamic/Friend/BesideNormal";
			relationConfig.iconLevel_2 = "UGUI/Sprite/Dynamic/Friend/BesideHigh";
			relationConfig.iconLevel_3 = "UGUI/Sprite/Dynamic/Friend/BesideMax";
			this.m_RelationConfig.Add(relationConfig);
		}

		public RelationConfig GetRelaTextCfg(byte state)
		{
			return this.GetRelaTextCfg((COM_INTIMACY_STATE)state);
		}

		public RelationConfig GetRelaTextCfg(COM_INTIMACY_STATE state)
		{
			if (IntimacyRelationViewUT.IsRelaState(state))
			{
				for (int i = 0; i < this.m_RelationConfig.Count; i++)
				{
					RelationConfig relationConfig = this.m_RelationConfig[i];
					if (relationConfig.state == state)
					{
						return relationConfig;
					}
				}
			}
			return null;
		}

		public CFriendRelationship.FRConfig GetCFGByIndex(int index)
		{
			for (int i = 0; i < this.frConfig_list.Count; i++)
			{
				CFriendRelationship.FRConfig fRConfig = this.frConfig_list[i];
				if (fRConfig != null && fRConfig.piority == index)
				{
					return fRConfig;
				}
			}
			return null;
		}

		public COM_INTIMACY_STATE GetFirstChoiseState()
		{
			return this._state;
		}

		public void SetFirstChoiseState(byte state)
		{
			this.SetFirstChoiseState((COM_INTIMACY_STATE)state);
		}

		public void SetFirstChoiseState(COM_INTIMACY_STATE state)
		{
			this._state = state;
			Singleton<EventRouter>.GetInstance().BroadCastEvent("FRDataChange");
		}
	}
}
