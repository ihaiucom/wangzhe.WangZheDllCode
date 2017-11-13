using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.UI;
using CSProtocol;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	[MessageHandlerClass]
	public class CSurrenderSystem : Singleton<CSurrenderSystem>
	{
		public static string s_surrenderForm = "UGUI/Form/Battle/Form_Surrender.prefab";

		private bool m_haveRights = true;

		private uint m_lastSurrenderTime;

		private static int SURRENDER_TIME_START;

		private static int SURRENDER_TIME_VALID;

		private static int SURRENDER_TIME_CD;

		private int m_timerSeq = -1;

		private int m_maxCnt;

		private int m_curCnt;

		private byte m_result;

		private Random m_random = new Random();

		private int GetSurrenderStartTime()
		{
			if (CSurrenderSystem.SURRENDER_TIME_START == 0)
			{
				CSurrenderSystem.SURRENDER_TIME_START = (int)GameDataMgr.globalInfoDatabin.GetDataByKey(137u).dwConfValue;
			}
			return CSurrenderSystem.SURRENDER_TIME_START;
		}

		private int GetSurrenderVaildTime()
		{
			if (CSurrenderSystem.SURRENDER_TIME_VALID == 0)
			{
				CSurrenderSystem.SURRENDER_TIME_VALID = (int)GameDataMgr.globalInfoDatabin.GetDataByKey(138u).dwConfValue;
			}
			return CSurrenderSystem.SURRENDER_TIME_VALID;
		}

		private int GetSurrenderCDTime()
		{
			if (CSurrenderSystem.SURRENDER_TIME_CD == 0)
			{
				CSurrenderSystem.SURRENDER_TIME_CD = (int)GameDataMgr.globalInfoDatabin.GetDataByKey(139u).dwConfValue;
			}
			return CSurrenderSystem.SURRENDER_TIME_CD;
		}

		public override void Init()
		{
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_Surrender, new CUIEventManager.OnUIEventHandler(this.OnSurrenderConfirm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_Surrender_Confirm, new CUIEventManager.OnUIEventHandler(this.OnSurrenderConfirm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_Surrender_Against, new CUIEventManager.OnUIEventHandler(this.OnSurrenderAgainst));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_Surrender_CountDown, new CUIEventManager.OnUIEventHandler(this.OnSurrenderCountDown));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_Surrender_TimeUp, new CUIEventManager.OnUIEventHandler(this.OnSurrenderTimeUp));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Battle_Surrender_TimeStart, new CUIEventManager.OnUIEventHandler(this.OnSurrenderTimeStart));
			Singleton<GameEventSys>.instance.AddEventHandler<DefaultGameEventParam>(GameEventDef.Event_FightOver, new RefAction<DefaultGameEventParam>(this.OnFightOver));
		}

		public override void UnInit()
		{
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_Surrender, new CUIEventManager.OnUIEventHandler(this.OnSurrenderConfirm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_Surrender_Confirm, new CUIEventManager.OnUIEventHandler(this.OnSurrenderConfirm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_Surrender_Against, new CUIEventManager.OnUIEventHandler(this.OnSurrenderAgainst));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_Surrender_CountDown, new CUIEventManager.OnUIEventHandler(this.OnSurrenderCountDown));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_Surrender_TimeUp, new CUIEventManager.OnUIEventHandler(this.OnSurrenderTimeUp));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Battle_Surrender_TimeStart, new CUIEventManager.OnUIEventHandler(this.OnSurrenderTimeStart));
			Singleton<GameEventSys>.instance.RmvEventHandler<DefaultGameEventParam>(GameEventDef.Event_FightOver, new RefAction<DefaultGameEventParam>(this.OnFightOver));
		}

		public void Reset()
		{
			this.m_lastSurrenderTime = 0u;
			this.m_haveRights = true;
			if (this.m_timerSeq != -1)
			{
				Singleton<CTimerManager>.instance.RemoveTimer(this.m_timerSeq);
			}
			this.m_timerSeq = -1;
			this.m_maxCnt = 0;
			this.m_curCnt = 0;
			this.m_result = 0;
		}

		private void OnSurrenderConfirm(CUIEvent cuiEvent)
		{
			if (this.IsWarmBattle() && !Singleton<LobbyLogic>.instance.inMultiGame)
			{
				this.m_lastSurrenderTime = (uint)CRoleInfo.GetCurrentUTCTime();
				this.m_maxCnt = this.GetTotalAcnt();
				this.m_curCnt = 1;
				if (this.m_maxCnt == 1)
				{
					COM_PLAYERCAMP playerCamp = Singleton<GamePlayerCenter>.instance.GetHostPlayer().PlayerCamp;
					BattleLogic.ForceKillCrystal((int)playerCamp);
				}
				else
				{
					this.m_haveRights = false;
					this.m_result = this.ConstrcutData(this.m_maxCnt);
					this.m_timerSeq = Singleton<CTimerManager>.instance.AddTimer(this.m_random.Next(500, 3000), 1, new CTimer.OnTimeUpHandler(this.OnTimerWarmBattle));
					this.OpenSurrenderForm(this.m_maxCnt, this.m_curCnt, this.m_result);
				}
			}
			else
			{
				this.SendMsgSurrender(1);
			}
		}

		private bool IsWarmBattle()
		{
			SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
			return curLvelContext != null && curLvelContext.m_isWarmBattle;
		}

		private int GetTotalAcnt()
		{
			SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
			if (curLvelContext == null)
			{
				return 0;
			}
			return curLvelContext.m_pvpPlayerNum / 2;
		}

		private byte ConstrcutData(int maxCnt)
		{
			int num = 0;
			for (int i = 0; i < maxCnt; i++)
			{
				if (i != maxCnt - 1)
				{
					num |= 1 << i;
				}
				else if (this.m_random.Next(0, 2) == 0)
				{
					num |= 1 << i;
				}
			}
			return (byte)num;
		}

		private void OnTimerWarmBattle(int timerSequence)
		{
			Singleton<CTimerManager>.instance.RemoveTimer(this.m_timerSeq);
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(FightForm.s_battleUIForm);
			if (form != null && !form.IsHided())
			{
				this.m_timerSeq = -1;
				if (this.m_curCnt != this.m_maxCnt)
				{
					this.m_curCnt++;
					this.m_timerSeq = Singleton<CTimerManager>.instance.AddTimer(this.m_random.Next(500, 3000), 1, new CTimer.OnTimeUpHandler(this.OnTimerWarmBattle));
					this.OpenSurrenderForm(this.m_maxCnt, this.m_curCnt, this.m_result);
				}
				else
				{
					this.m_haveRights = true;
					COM_PLAYERCAMP playerCamp = Singleton<GamePlayerCenter>.instance.GetHostPlayer().PlayerCamp;
					BattleLogic.ForceKillCrystal((int)playerCamp);
				}
			}
		}

		private void OnSurrenderAgainst(CUIEvent cuiEvent)
		{
			this.SendMsgSurrender(0);
		}

		public bool CanSurrender()
		{
			SLevelContext curLvelContext = Singleton<BattleLogic>.instance.GetCurLvelContext();
			return curLvelContext != null && curLvelContext.IsMultilModeWithWarmBattle();
		}

		public bool InSurrenderCD()
		{
			return (this.m_lastSurrenderTime != 0u || (int)Singleton<FrameSynchr>.instance.LogicFrameTick < this.GetSurrenderStartTime() * 1000) && (this.m_lastSurrenderTime <= 0u || (long)CRoleInfo.GetCurrentUTCTime() < (long)((ulong)this.m_lastSurrenderTime + (ulong)((long)this.GetSurrenderCDTime())));
		}

		public bool InSurrenderCD(out uint time)
		{
			time = 0u;
			if (this.m_lastSurrenderTime == 0u)
			{
				if ((int)Singleton<FrameSynchr>.instance.LogicFrameTick >= this.GetSurrenderStartTime() * 1000)
				{
					return false;
				}
				time = (uint)(this.GetSurrenderStartTime() - (int)((uint)Singleton<FrameSynchr>.instance.LogicFrameTick / 1000u));
				return true;
			}
			else
			{
				if (this.m_lastSurrenderTime <= 0u)
				{
					return true;
				}
				if ((long)CRoleInfo.GetCurrentUTCTime() >= (long)((ulong)this.m_lastSurrenderTime + (ulong)((long)this.GetSurrenderCDTime())))
				{
					return false;
				}
				time = this.m_lastSurrenderTime + (uint)this.GetSurrenderCDTime() - (uint)CRoleInfo.GetCurrentUTCTime();
				return true;
			}
		}

		public void OpenSurrenderForm(int maxNum, int totalNum, byte data)
		{
			bool flag = Singleton<CUIManager>.GetInstance().GetForm(CSurrenderSystem.s_surrenderForm) == null;
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CSurrenderSystem.s_surrenderForm, false, true);
			if (cUIFormScript)
			{
				GameObject gameObject = cUIFormScript.transform.GetChild(0).gameObject;
				GameObject gameObject2 = gameObject.transform.Find("SurrenderElement").gameObject;
				for (int i = 0; i < 5; i++)
				{
					GameObject gameObject3 = gameObject2.transform.GetChild(i).gameObject;
					bool flag2 = i < maxNum;
					gameObject3.SetActive(flag2);
					if (flag2)
					{
						if (i < totalNum)
						{
							bool flag3 = ((int)data & 1 << i) > 0;
							gameObject3.transform.GetChild(0).gameObject.CustomSetActive(flag3);
							gameObject3.transform.GetChild(1).gameObject.CustomSetActive(!flag3);
						}
						else
						{
							gameObject3.transform.GetChild(0).gameObject.CustomSetActive(false);
							gameObject3.transform.GetChild(1).gameObject.CustomSetActive(false);
						}
					}
				}
				gameObject.transform.Find("ButtonGroup/Button_Surrender").gameObject.SetActive(this.m_haveRights);
				gameObject.transform.Find("ButtonGroup/Button_Reject").gameObject.SetActive(this.m_haveRights);
				if (flag)
				{
					CUITimerScript component = gameObject.GetComponent<CUITimerScript>();
					component.SetTotalTime((float)this.GetSurrenderVaildTime());
					component.ReStartTimer();
					if (!this.m_haveRights)
					{
						this.SimplifySurrenderForm();
					}
				}
				if (maxNum == totalNum)
				{
					this.DelayCloseSurrenderForm(5);
				}
			}
		}

		public void CloseSurrenderForm()
		{
			Singleton<CUIManager>.GetInstance().CloseForm(CSurrenderSystem.s_surrenderForm);
		}

		public void DelayCloseSurrenderForm(int delay)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CSurrenderSystem.s_surrenderForm);
			if (form != null)
			{
				CUITimerScript component = form.transform.GetChild(0).GetComponent<CUITimerScript>();
				if (component != null && component.GetCurrentTime() > (float)delay)
				{
					component.SetTotalTime((float)delay);
					component.ReStartTimer();
				}
			}
		}

		private void OnSurrenderCountDown(CUIEvent cuiEvent)
		{
			GameObject srcWidget = cuiEvent.m_srcWidget;
			CUITimerScript cUITimerScript = cuiEvent.m_srcWidgetScript as CUITimerScript;
			if (srcWidget != null && cUITimerScript != null)
			{
				float value = cUITimerScript.GetCurrentTime() / (float)this.GetSurrenderVaildTime();
				Utility.GetComponetInChild<Slider>(srcWidget, "CountDownBar/Bar").set_value(value);
			}
		}

		private void OnSurrenderTimeUp(CUIEvent cuiEvent)
		{
			this.CloseSurrenderForm();
			this.m_haveRights = true;
		}

		private void OnSurrenderTimeStart(CUIEvent cuiEvent)
		{
			GameObject srcWidget = cuiEvent.m_srcWidget;
			CUITimerScript cUITimerScript = cuiEvent.m_srcWidgetScript as CUITimerScript;
			if (srcWidget != null && cUITimerScript != null)
			{
				float value = cUITimerScript.GetCurrentTime() / (float)this.GetSurrenderVaildTime();
				Utility.GetComponetInChild<Slider>(srcWidget, "CountDownBar/Bar").set_value(value);
			}
		}

		public void SendMsgSurrender(byte bSurrender)
		{
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(4900u);
			cSPkg.stPkgData.stSurrenderReq.bSurrender = bSurrender;
			Singleton<NetworkModule>.GetInstance().SendGameMsg(ref cSPkg, 0u);
		}

		public void OnSurrenderNtf(SCPKG_SURRENDER_NTF surrenderNtf)
		{
			this.m_lastSurrenderTime = surrenderNtf.dwSurrenderTime;
			this.OpenSurrenderForm((int)surrenderNtf.bSurrenderValidCnt, (int)surrenderNtf.bSurrenderCnt, surrenderNtf.bSurrenderData);
		}

		public void OnSurrenderRsp(SCPKG_SURRENDER_RSP msg)
		{
			this.m_haveRights = false;
			this.m_lastSurrenderTime = msg.dwSurrenderTime;
			this.SimplifySurrenderForm();
		}

		[MessageHandler(4901)]
		public static void OnSurrenderRsp(CSPkg msg)
		{
			Singleton<CSurrenderSystem>.instance.OnSurrenderRsp(msg.stPkgData.stSurrenderRsp);
		}

		[MessageHandler(4902)]
		public static void OnSurrenderNtf(CSPkg msg)
		{
			SCPKG_SURRENDER_NTF stSurrenderNtf = msg.stPkgData.stSurrenderNtf;
			Singleton<CSurrenderSystem>.instance.OnSurrenderNtf(stSurrenderNtf);
		}

		private void OnFightOver(ref DefaultGameEventParam prm)
		{
			this.CloseSurrenderForm();
		}

		private void SimplifySurrenderForm()
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CSurrenderSystem.s_surrenderForm);
			if (form)
			{
				Transform transform = form.transform.FindChild("PanelSurrender");
				if (transform)
				{
					CUICommonSystem.PlayAnimation(transform, null);
				}
			}
		}
	}
}
