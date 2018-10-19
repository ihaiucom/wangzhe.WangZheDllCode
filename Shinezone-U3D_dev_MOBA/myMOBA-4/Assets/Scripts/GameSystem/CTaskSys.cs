using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using Assets.Scripts.UI;
using CSProtocol;
using ResData;
using System;
using UnityEngine;

namespace Assets.Scripts.GameSystem
{
	public class CTaskSys : Singleton<CTaskSys>
	{
		public enum SECRETARY_REPORT_INT_TYPE
		{
			SECRETARY_REPORT_INT_TYPE_OPENRECORDKINGTIME = 5,
			SECRETARY_REPORT_INT_TYPE_CREATEKINGTIMEVIDEO,
			SECRETARY_REPORT_INT_TYPE_MAX
		}

		public const string finish_task_form = "UGUI/Form/System/Task/Form_Task_Finish.prefab";

		public const string finish_task_form_new = "UGUI/Form/System/Task/Form_Task_Finish_New.prefab";

		public static readonly string TASK_FORM_PATH = "UGUI/Form/System/Task/Form_MainTask.prefab";

		public static readonly string TASK_LevelRewardFORM_PATH = "UGUI/Form/System/Task/Form_LevelRewardTask.prefab";

		public CTaskView m_taskView = new CTaskView();

		public CTaskModel model;

		private uint _monthCardTaskID;

		private uint _monthCardExpireTime;

		private uint _weekCardTaskID;

		private uint _weekCardExpireTime;

		private uint _lastGetAwardTaskId;

		private uint[] reportInts = new uint[7];

		private int sendReportTimer = -1;

		public string errorStr = string.Empty;

		public string delStr = string.Empty;

		public uint apprenticeTaskID
		{
			get;
			set;
		}

		public uint masterTaskID
		{
			get;
			set;
		}

		public uint mentorTaskID
		{
			get
			{
				if (this.apprenticeTaskID != 0u)
				{
					return this.apprenticeTaskID;
				}
				if (this.masterTaskID != 0u)
				{
					return this.masterTaskID;
				}
				return 0u;
			}
		}

		public CFriendMentorTaskView.EMentorTaskState MentorTaskState
		{
			get
			{
				this.errorStr = string.Empty;
				if (this.apprenticeTaskID != 0u)
				{
					return CFriendMentorTaskView.EMentorTaskState.Tudi_Task;
				}
				if (this.masterTaskID != 0u)
				{
					return CFriendMentorTaskView.EMentorTaskState.MasterTask;
				}
				CTask task = this.model.GetTask(this.model.maxCfgTudiTaskID);
				CTask task2 = this.model.GetTask(this.model.maxCfgMasterTaskID);
				if (this.apprenticeTaskID == 0u && this.masterTaskID == 0u)
				{
					if (this.model.maxServerTudiTaskID == 0u && this.model.maxServerMasterTaskID == 0u)
					{
						return CFriendMentorTaskView.EMentorTaskState.Empty;
					}
					if ((this.model.maxCfgTudiTaskID == this.model.maxServerTudiTaskID && this.model.maxServerMasterTaskID == 0u) || (task != null && task.m_taskState == 3))
					{
						return CFriendMentorTaskView.EMentorTaskState.TudiTaskFinish_No_MasterTask;
					}
					if (this.model.maxCfgMasterTaskID == this.model.maxServerMasterTaskID || (task2 != null && task2.m_taskState == 3))
					{
						return CFriendMentorTaskView.EMentorTaskState.AllFinish;
					}
				}
				this.errorStr = string.Format("apprenticeTaskID:{0}, masterTaskID:{1}, maxCfgTudiTaskID:{2}, maxCfgMasterTaskID:{3}, maxCfgTudiTask is null?:{4}, maxCfgShifuTask is null?:{5},maxServerTudiTaskID:{6}, maxServerMasterTaskID:{7}", new object[]
				{
					this.apprenticeTaskID,
					this.masterTaskID,
					this.model.maxCfgTudiTaskID,
					this.model.maxCfgMasterTaskID,
					task != null,
					task2 != null,
					this.model.maxServerTudiTaskID,
					this.model.maxServerMasterTaskID
				});
				if (task != null)
				{
					this.errorStr += string.Format(" maxCfgTudiTask.m_taskState:{0}", task.m_taskState);
				}
				if (task2 != null)
				{
					this.errorStr += string.Format(" maxCfgShifuTask.m_taskState:{0}", task2.m_taskState);
				}
				return CFriendMentorTaskView.EMentorTaskState.None;
			}
		}

		public uint monthCardTaskID
		{
			get
			{
				if (this._monthCardTaskID == 0u)
				{
					this._monthCardTaskID = GameDataMgr.globalInfoDatabin.GetDataByKey(163u).dwConfValue;
				}
				return this._monthCardTaskID;
			}
		}

		public uint monthCardExpireTime
		{
			get
			{
				return this._monthCardExpireTime;
			}
		}

		public uint weekCardTaskID
		{
			get
			{
				if (this._weekCardTaskID == 0u)
				{
					this._weekCardTaskID = GameDataMgr.globalInfoDatabin.GetDataByKey(164u).dwConfValue;
				}
				return this._weekCardTaskID;
			}
		}

		public uint weekCardExpireTime
		{
			get
			{
				return this._weekCardExpireTime;
			}
		}

		public void InitReport()
		{
			this.ClearReport();
			if (this.sendReportTimer != -1)
			{
				Singleton<CTimerManager>.instance.RemoveTimer(this.sendReportTimer);
			}
			this.sendReportTimer = -1;
			ResGlobalInfo dataByKey = GameDataMgr.globalInfoDatabin.GetDataByKey(136u);
			DebugHelper.Assert(dataByKey != null, "--- 分享统计上报时间间隔 找不到对应的配置项, 请检查...");
			this.sendReportTimer = Singleton<CTimerManager>.instance.AddTimer((int)(dataByKey.dwConfValue * 1000u), 0, new CTimer.OnTimeUpHandler(this.OnReportTimerEnd));
			Singleton<CTimerManager>.instance.ResumeTimer(this.sendReportTimer);
		}

		public bool IsReportAllZero()
		{
			for (int i = 0; i < this.reportInts.Length; i++)
			{
				if (this.reportInts[i] > 0u)
				{
					return false;
				}
			}
			return true;
		}

		private void OnReportTimerEnd(int timersequence)
		{
			if (!Singleton<BattleLogic>.instance.isRuning)
			{
				this.SendReport();
				this.ClearReport();
			}
		}

		public void Increse(int index)
		{
			this.reportInts[index] = this.reportInts[index] + 1u;
		}

		public void ClearReport()
		{
			for (int i = 0; i < this.reportInts.Length; i++)
			{
				this.reportInts[i] = 0u;
			}
		}

		public void SendReport()
		{
			if (this.IsReportAllZero())
			{
				return;
			}
			CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(4307u);
			cSPkg.stPkgData.stShareTLogReq.dwSecretaryNum = 7u;
			for (int i = 0; i < this.reportInts.Length; i++)
			{
				cSPkg.stPkgData.stShareTLogReq.SecretaryDetail[i] = this.reportInts[i];
			}
			Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
		}

		public bool IsMentorTaskRedDot()
		{
			uint mentorTaskID = Singleton<CTaskSys>.instance.mentorTaskID;
			CTask task = Singleton<CTaskSys>.instance.model.GetTask(mentorTaskID);
			return task != null && task.m_taskState == 1;
		}

		public void Clear()
		{
			this.model.Clear();
			this.delStr = string.Empty;
			this.errorStr = string.Empty;
			this.apprenticeTaskID = 0u;
			this.masterTaskID = 0u;
		}

		public override void Init()
		{
			if (this.model == null)
			{
				this.model = new CTaskModel();
			}
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Task_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnOpenTaskForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Task_EngineCloseForm, new CUIEventManager.OnUIEventHandler(this.OnTask_EngineCloseForm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Task_LinkPve, new CUIEventManager.OnUIEventHandler(this.OnLinkPveClick));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Task_TabChanged, new CUIEventManager.OnUIEventHandler(this.OnTaskChanged));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Task_AwardClose, new CUIEventManager.OnUIEventHandler(this.OnTaskAwardClose));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Task_List_ElementEnable, new CUIEventManager.OnUIEventHandler(this.On_Task_List_ElementEnable));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Task_Submit, new CUIEventManager.OnUIEventHandler(this.On_Task_Submit));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Task_Week_Reaward1, new CUIEventManager.OnUIEventHandler(this.OnTask_Week_Reaward1));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Task_Week_Reaward2, new CUIEventManager.OnUIEventHandler(this.OnTask_Week_Reaward2));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Task_Day_Reaward, new CUIEventManager.OnUIEventHandler(this.OnTask_Day_Reaward));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Task_Award_Get_Confirm, new CUIEventManager.OnUIEventHandler(this.OnTaskAwardGetConfirm));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Task_ClickLevelNode, new CUIEventManager.OnUIEventHandler(this.OnTask_ClickLevelNode));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Task_ClickGetLevelReward, new CUIEventManager.OnUIEventHandler(this.OnTask_ClickGetLevelReward));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Task_LevelElemntEnable, new CUIEventManager.OnUIEventHandler(this.OnTask_LevelElemntEnable));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Task_JumpToReward, new CUIEventManager.OnUIEventHandler(this.OnTask_JumpToReward));
			Singleton<EventRouter>.instance.AddEventHandler("TASK_HUOYUEDU_Change", new Action(this.On_TASK_HUOYUEDU_Change));
			this._monthCardTaskID = 0u;
			this._weekCardTaskID = 0u;
			this._monthCardExpireTime = 0u;
			this._weekCardExpireTime = 0u;
			base.Init();
		}

		public override void UnInit()
		{
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Task_OpenForm, new CUIEventManager.OnUIEventHandler(this.OnOpenTaskForm));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Task_LinkPve, new CUIEventManager.OnUIEventHandler(this.OnLinkPveClick));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Task_TabChanged, new CUIEventManager.OnUIEventHandler(this.OnTaskChanged));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Task_AwardClose, new CUIEventManager.OnUIEventHandler(this.OnTaskAwardClose));
			base.UnInit();
		}

		public int GetTotalTaskOfState(enTaskTab type, COM_TASK_STATE inState)
		{
			return this.model.GetTotalTaskOfState(type, inState);
		}

		private void UpdateTaskState()
		{
			this.model.UpdateTaskState();
		}

		public void SetCardExpireTime(RES_PROP_VALFUNC_TYPE funcType, uint expireTime)
		{
			if (funcType == RES_PROP_VALFUNC_TYPE.RES_PROP_VALFUNC_MONTH_CARD)
			{
				this._monthCardExpireTime = expireTime;
			}
			else if (funcType == RES_PROP_VALFUNC_TYPE.RES_PROP_VALFUNC_WEEK_CARD)
			{
				this._weekCardExpireTime = expireTime;
			}
		}

		public void OnInitTask(CSPkg pkg)
		{
			SCPKG_CMD_GAMELOGINRSP stGameLoginRsp = pkg.stPkgData.stGameLoginRsp;
			int num = 0;
			while ((long)num < (long)((ulong)stGameLoginRsp.stLoginTaskInfo.dwCurtaskNum))
			{
				COMDT_ACNT_CURTASK cOMDT_ACNT_CURTASK = stGameLoginRsp.stLoginTaskInfo.astCurtask[num];
				CTask cTask = TaskUT.Create_Task(cOMDT_ACNT_CURTASK.dwBaseID);
				if (cTask != null)
				{
					cTask.SetState(cOMDT_ACNT_CURTASK.bTaskState);
					TaskUT.Add_Task(cTask);
					for (int i = 0; i < (int)cOMDT_ACNT_CURTASK.bPrerequisiteNum; i++)
					{
						int bPosInArray = (int)cOMDT_ACNT_CURTASK.astPrerequisiteInfo[i].bPosInArray;
						bool flag = cOMDT_ACNT_CURTASK.astPrerequisiteInfo[i].bIsReach > 0;
						if (flag)
						{
							cTask.m_prerequisiteInfo[bPosInArray].m_value = cTask.m_prerequisiteInfo[bPosInArray].m_valueTarget;
						}
						else
						{
							cTask.m_prerequisiteInfo[bPosInArray].m_value = (int)cOMDT_ACNT_CURTASK.astPrerequisiteInfo[i].dwCnt;
						}
						cTask.m_prerequisiteInfo[bPosInArray].m_isReach = flag;
					}
					this.ParseMentorTask(cTask);
				}
				num++;
			}
			this.UpdateTaskState();
			this.model.ParseCltCalcCompletedTasks(ref stGameLoginRsp.stLoginTaskInfo.MainTaskIDs);
			Singleton<EventRouter>.instance.BroadCastEvent("TaskUpdated");
			if (this.m_taskView != null)
			{
				this.m_taskView.Refresh();
			}
		}

		private void ParseMentorTask(CTask task)
		{
			if (task == null)
			{
				return;
			}
			if (task.m_resTask.dwTaskType == 0u)
			{
				if (task.m_resTask.bTaskSubType == 3)
				{
					this.masterTaskID = task.m_baseId;
					Singleton<EventRouter>.instance.BroadCastEvent("TASK_Mentor_UPDATED");
				}
				else if (task.m_resTask.bTaskSubType == 4)
				{
					this.apprenticeTaskID = task.m_baseId;
					Singleton<EventRouter>.instance.BroadCastEvent("TASK_Mentor_UPDATED");
				}
			}
		}

		public void OnRefreshTaskView()
		{
			if (this.m_taskView != null)
			{
				this.m_taskView.Refresh();
			}
		}

		public void On_TASKUPD_NTF(ref CSPkg pkg)
		{
			SCPKG_TASKUPD_NTF stTaskUdpNtf = pkg.stPkgData.stTaskUdpNtf;
			int num = 0;
			while ((long)num < (long)((ulong)stTaskUdpNtf.dwUpdTaskCnt))
			{
				SCDT_UPDTASKONE sCDT_UPDTASKONE = stTaskUdpNtf.astUpdTaskDetail[num];
				if (sCDT_UPDTASKONE != null)
				{
					uint key = 0u;
					if (sCDT_UPDTASKONE.bUpdateType == 0)
					{
						key = sCDT_UPDTASKONE.stUpdTaskInfo.stUdpPrerequisite.dwTaskID;
					}
					if (this.model.task_Data.task_map.ContainsKey(key))
					{
						SCDT_UDPPREREQUISITE stUdpPrerequisite = sCDT_UPDTASKONE.stUpdTaskInfo.stUdpPrerequisite;
						if (stUdpPrerequisite != null)
						{
							CTask task = this.model.GetTask(stUdpPrerequisite.dwTaskID);
							if (task != null)
							{
								task.SetState(stUdpPrerequisite.bTaskState);
								for (int i = 0; i < (int)stUdpPrerequisite.bPrerequisiteNum; i++)
								{
									int bPosInArray = (int)stUdpPrerequisite.astPrerequisiteInfo[i].bPosInArray;
									bool flag = stUdpPrerequisite.astPrerequisiteInfo[i].bIsReach > 0;
									if (flag)
									{
										task.m_prerequisiteInfo[bPosInArray].m_value = task.m_prerequisiteInfo[bPosInArray].m_valueTarget;
									}
									else
									{
										task.m_prerequisiteInfo[bPosInArray].m_value = (int)stUdpPrerequisite.astPrerequisiteInfo[i].dwCnt;
									}
									task.m_prerequisiteInfo[bPosInArray].m_isReach = flag;
								}
							}
						}
					}
				}
				num++;
			}
			this.UpdateTaskState();
			Singleton<EventRouter>.instance.BroadCastEvent("TaskUpdated");
			if (this.m_taskView != null)
			{
				this.m_taskView.Refresh();
			}
		}

		public void OnUSUALTASK_RES(ref CSPkg pkg)
		{
			SCDT_USUTASKLIST stUsualTaskList = pkg.stPkgData.stUsualTaskRes.stUpdateDetail.stUsualTaskList;
			for (int i = 0; i < (int)stUsualTaskList.bNewUsualTaskCnt; i++)
			{
				DT_USUTASKINFO dT_USUTASKINFO = stUsualTaskList.astNewUsualTask[i];
				if (dT_USUTASKINFO.bIsNew == 1)
				{
					this.model.Remove(dT_USUTASKINFO.dwTaskID);
					CTask cTask = TaskUT.Create_Task(dT_USUTASKINFO.dwTaskID);
					if (cTask != null)
					{
						cTask.SetState(CTask.State.NewRefresh);
						TaskUT.Add_Task(cTask);
					}
				}
			}
			this.UpdateTaskState();
			Singleton<EventRouter>.instance.BroadCastEvent("TaskUpdated");
			if (this.m_taskView != null)
			{
				this.m_taskView.Refresh();
			}
		}

		public void OnSCID_NEWTASKGET_NTF(ref CSPkg pkg)
		{
			SCPKG_NEWTASKGET_NTF stNewTaskGet = pkg.stPkgData.stNewTaskGet;
			int num = 0;
			while ((long)num < (long)((ulong)stNewTaskGet.dwTaskCnt))
			{
				SCDT_NEWTASKGET sCDT_NEWTASKGET = stNewTaskGet.astNewTask[num];
				CTask cTask = TaskUT.Create_Task(sCDT_NEWTASKGET.dwTaskID);
				DebugHelper.Assert(cTask.m_taskType == sCDT_NEWTASKGET.dwTaskType, "OnSCID_NEWTASKGET_NTF task.m_taskType == info.dwTaskType");
				if (cTask != null)
				{
					this.model.AddTask(cTask);
				}
				this.ParseMentorTask(cTask);
				num++;
			}
			Singleton<EventRouter>.instance.BroadCastEvent("TaskUpdated");
			if (this.m_taskView != null)
			{
				this.m_taskView.Refresh();
			}
		}

		public void OnTASKSUBMIT_RES(ref CSPkg pkg)
		{
			if (pkg.stPkgData.stSubmitTaskRes.bSubmitResult == 0)
			{
				this._lastGetAwardTaskId = pkg.stPkgData.stSubmitTaskRes.dwTaskID;
				this._show_task_award(this._lastGetAwardTaskId);
				CTask task = this.model.GetTask(this._lastGetAwardTaskId);
				if (task != null)
				{
					task.SetState((CTask.State)pkg.stPkgData.stSubmitTaskRes.bTaskState);
					if (task.m_taskSubType == 3)
					{
						this.masterTaskID = 0u;
					}
					if (task.m_taskSubType == 4)
					{
						this.apprenticeTaskID = 0u;
					}
				}
			}
			Singleton<EventRouter>.instance.BroadCastEvent("TaskUpdated");
			if (this.m_taskView != null)
			{
				this.m_taskView.Refresh();
			}
		}

		private void _show_task_award(uint taskid)
		{
			CTask task = this.model.task_Data.GetTask(taskid);
			if (task != null)
			{
				ResTaskReward resAward = task.resAward;
				if (resAward == null)
				{
					return;
				}
				int num = 0;
				for (int i = 0; i < resAward.astRewardInfo.Length; i++)
				{
					ResTaskRewardDetail resTaskRewardDetail = resAward.astRewardInfo[i];
					if (resTaskRewardDetail.iCnt > 0)
					{
						num++;
					}
				}
				CUseable[] array = new CUseable[num];
				for (int j = 0; j < resAward.astRewardInfo.Length; j++)
				{
					ResTaskRewardDetail resTaskRewardDetail2 = resAward.astRewardInfo[j];
					if (resTaskRewardDetail2.iCnt > 0)
					{
						array[j] = CUseableManager.CreateUsableByServerType((RES_REWARDS_TYPE)resTaskRewardDetail2.dwRewardType, resTaskRewardDetail2.iCnt, resTaskRewardDetail2.dwRewardID);
					}
				}
				Singleton<CUIManager>.GetInstance().OpenAwardTip(array, null, false, enUIEventID.Task_Award_Get_Confirm, false, false, "Form_Award");
			}
		}

		public void OnHuoyue_Reward_RES(ref CSPkg pkg)
		{
			SCPKG_GETHUOYUEDUREWARD_RSP stHuoYueDuRewardRsp = pkg.stPkgData.stHuoYueDuRewardRsp;
			this.model.huoyue_data.Get_Reward((RES_HUOYUEDU_TYPE)stHuoYueDuRewardRsp.bRewardType, stHuoYueDuRewardRsp.wHuoYueDuId);
			if (stHuoYueDuRewardRsp.stRewardInfo.bNum > 0)
			{
				ListView<CUseable> useableListFromReward = CUseableManager.GetUseableListFromReward(stHuoYueDuRewardRsp.stRewardInfo);
				Singleton<CUIManager>.GetInstance().OpenAwardTip(LinqS.ToArray<CUseable>(useableListFromReward), null, false, enUIEventID.None, false, false, "Form_Award");
			}
			if (this.m_taskView != null)
			{
				this.m_taskView.Refresh_Huoyue();
			}
		}

		public void OnHuoyue_Info_NTF(ref CSPkg pkg)
		{
			COMDT_HUOYUEDU_DATA stHuoYueDuInfo = pkg.stPkgData.stNtfHuoYueDuInfo.stHuoYueDuInfo;
			this.model.huoyue_data.Set(RES_HUOYUEDU_TYPE.RES_HUOYUEDU_TYPE_WEEK, stHuoYueDuInfo.dwWeekHuoYue, stHuoYueDuInfo.iWeekRewardCnt, stHuoYueDuInfo.WeekReward);
			this.model.huoyue_data.Set(RES_HUOYUEDU_TYPE.RES_HUOYUEDU_TYPE_DAY, stHuoYueDuInfo.dwDayHuoYue, stHuoYueDuInfo.iDayRewardCnt, stHuoYueDuInfo.DayReward);
			if (this.m_taskView != null)
			{
				this.m_taskView.Refresh_Huoyue();
			}
		}

		public void OnHuoyue_Error_NTF(ref CSPkg pkg)
		{
			int iErrorCode = pkg.stPkgData.stHuoYueDuRewardErr.iErrorCode;
			string text = string.Empty;
			switch (iErrorCode)
			{
			case 0:
				text = UT.GetText("CS_HUOYUEDUREWARD_SUCC");
				break;
			case 1:
				text = UT.GetText("CS_HUOYUEDUREWARD_ACNTNULL");
				break;
			case 2:
				text = UT.GetText("CS_HUOYUEDUREWARD_INFONULL");
				break;
			case 3:
				text = UT.GetText("CS_HUOYUEDUREWARD_NOTINTABLE");
				break;
			case 4:
				text = UT.GetText("CS_HUOYUEDUREWARD_NOTENOUGH");
				break;
			case 5:
				text = UT.GetText("CS_HUOYUEDUREWARD_GETED");
				break;
			}
			if (!string.IsNullOrEmpty(text))
			{
				Singleton<CUIManager>.instance.OpenTips(text, false, 1.5f, null, new object[0]);
			}
		}

		public CTaskView GetTaskView()
		{
			return this.m_taskView;
		}

		public void OnOpenTaskForm(CUIEvent uiEvent)
		{
			if (!this.IsOpenTaskSys())
			{
				return;
			}
			this.m_taskView.OpenForm(uiEvent);
			TaskNetUT.Send_Update_Task(0u);
			CMiShuSystem.SendReqCoinGetPathData();
		}

		public bool IsOpenTaskSys()
		{
			if (Singleton<CFunctionUnlockSys>.instance.FucIsUnlock(RES_SPECIALFUNCUNLOCK_TYPE.RES_SPECIALFUNCUNLOCKTYPE_TASK))
			{
				return true;
			}
			ResSpecialFucUnlock dataByKey = GameDataMgr.specialFunUnlockDatabin.GetDataByKey(16u);
			Singleton<CUIManager>.instance.OpenTips(Utility.UTF8Convert(dataByKey.szLockedTip), false, 1.5f, null, new object[0]);
			return false;
		}

		private void OnTaskAwardClose(CUIEvent uiEvent)
		{
			Singleton<CUIManager>.GetInstance().CloseForm("UGUI/Form/Common/Form_Award.prefab");
		}

		private void OnTaskAwardGetConfirm(CUIEvent uiEvent)
		{
			if (this._lastGetAwardTaskId != 0u)
			{
				CTask task = this.model.task_Data.GetTask(this._lastGetAwardTaskId);
				bool flag = this._lastGetAwardTaskId == Singleton<CTaskSys>.instance.monthCardTaskID;
				bool flag2 = this._lastGetAwardTaskId == Singleton<CTaskSys>.instance.weekCardTaskID;
				if (flag)
				{
					this.ShowMonthWeekCardExpireTips(task, true);
					return;
				}
				if (flag2)
				{
					this.ShowMonthWeekCardExpireTips(task, false);
					return;
				}
			}
		}

		public void OnTask_EngineCloseForm(CUIEvent uiEvent)
		{
			if (this.m_taskView != null)
			{
				this.m_taskView.Clear();
			}
			Singleton<CMiShuSystem>.instance.OnCloseTalk(null);
			Singleton<EventRouter>.instance.BroadCastEvent("TaskUpdated");
		}

		public void OnTaskChanged(CUIEvent uiEvent)
		{
			int selectedIndex = uiEvent.m_srcWidget.GetComponent<CUIListScript>().GetSelectedIndex();
			this.m_taskView.On_Tab_Change(selectedIndex);
			switch (selectedIndex)
			{
			case 0:
				CMiShuSystem.SendUIClickToServer(enUIClickReprotID.rp_MainTaskBtn);
				break;
			case 1:
				CMiShuSystem.SendUIClickToServer(enUIClickReprotID.rp_DayTaskBtn);
				break;
			case 2:
				CMiShuSystem.SendUIClickToServer(enUIClickReprotID.rp_GetCoinBtn);
				break;
			case 3:
				CMiShuSystem.SendUIClickToServer(enUIClickReprotID.rp_GetSymbolBtn);
				break;
			case 4:
				CMiShuSystem.SendUIClickToServer(enUIClickReprotID.rp_GetHeroBtn);
				break;
			}
		}

		public void OnLinkPveClick(CUIEvent uiEvent)
		{
			uint taskId = uiEvent.m_eventParams.taskId;
			uiEvent.m_eventID = enUIEventID.Adv_OpenChapterForm;
			ResTask dataByKey = GameDataMgr.taskDatabin.GetDataByKey(taskId);
			int num = 0;
			for (int i = 0; i < dataByKey.astPrerequisiteArray.Length; i++)
			{
				if (dataByKey.astPrerequisiteArray[i].dwPrerequisiteType == 2u)
				{
					num = dataByKey.astPrerequisiteArray[i].astPrerequisiteParam[1].iParam;
					break;
				}
			}
			if (num == 0)
			{
				Singleton<CAdventureSys>.instance.currentChapter = 1;
				Singleton<CAdventureSys>.instance.currentLevelSeq = 1;
				Singleton<CAdventureSys>.instance.currentDifficulty = 1;
				Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uiEvent);
			}
			else
			{
				ResLevelCfgInfo dataByKey2 = GameDataMgr.levelDatabin.GetDataByKey((long)num);
				DebugHelper.Assert(dataByKey2 != null);
				if (dataByKey2 != null)
				{
					Singleton<CAdventureSys>.instance.currentChapter = dataByKey2.iChapterId;
					Singleton<CAdventureSys>.instance.currentLevelSeq = (int)dataByKey2.bLevelNo;
					Singleton<CAdventureSys>.instance.currentDifficulty = uiEvent.m_eventParams.tag;
					Singleton<CUIEventManager>.GetInstance().DispatchUIEvent(uiEvent);
				}
			}
		}

		private void ShowMonthWeekCardExpireTips(CTask task, bool isMonthCard)
		{
			uint num = (!isMonthCard) ? Singleton<CTaskSys>.instance.weekCardExpireTime : Singleton<CTaskSys>.instance.monthCardExpireTime;
			uint currentUTCTime = (uint)CRoleInfo.GetCurrentUTCTime();
			int num2 = Mathf.CeilToInt(((num <= currentUTCTime) ? 0f : (num - currentUTCTime)) / 86400f);
			if (task.m_taskState == 3 && num2 > 0)
			{
				num2--;
			}
			if (isMonthCard)
			{
				ResGlobalInfo resGlobalInfo = null;
				if (GameDataMgr.svr2CltCfgDict.TryGetValue(22u, out resGlobalInfo))
				{
					stUIEventParams param = default(stUIEventParams);
					param.tag2 = (int)GameDataMgr.globalInfoDatabin.GetDataByKey(292u).dwConfValue;
					if (num2 == 0)
					{
						Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("MonthCardExpireTips2"), enUIEventID.Mall_Open_Factory_Shop_Tab, enUIEventID.None, param, "立即续费", "暂不续费", false, string.Empty);
					}
					else if ((long)num2 <= (long)((ulong)resGlobalInfo.dwConfValue))
					{
						Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("MonthCardExpireTips1"), enUIEventID.Mall_Open_Factory_Shop_Tab, enUIEventID.None, param, "立即续费", "暂不续费", false, string.Empty);
					}
				}
			}
			else
			{
				ResGlobalInfo resGlobalInfo2 = null;
				if (GameDataMgr.svr2CltCfgDict.TryGetValue(23u, out resGlobalInfo2))
				{
					stUIEventParams param2 = default(stUIEventParams);
					param2.tag2 = (int)GameDataMgr.globalInfoDatabin.GetDataByKey(293u).dwConfValue;
					if (num2 == 0)
					{
						Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("WeekCardExpireTips2"), enUIEventID.Mall_Open_Factory_Shop_Tab, enUIEventID.None, param2, "立即续费", "暂不续费", false, string.Empty);
					}
					else if ((long)num2 <= (long)((ulong)resGlobalInfo2.dwConfValue))
					{
						Singleton<CUIManager>.GetInstance().OpenMessageBoxWithCancel(Singleton<CTextManager>.GetInstance().GetText("WeekCardExpireTips1"), enUIEventID.Mall_Open_Factory_Shop_Tab, enUIEventID.None, param2, "立即续费", "暂不续费", false, string.Empty);
					}
				}
			}
		}

		public static void Send_Share_Task()
		{
			if (Singleton<CTaskSys>.instance.model.share_task_id > 0u)
			{
				CSPkg cSPkg = NetworkModule.CreateDefaultCSPKG(1505u);
				cSPkg.stPkgData.stClientReportTaskDone.dwTaskID = Singleton<CTaskSys>.instance.model.share_task_id;
				cSPkg.stPkgData.stClientReportTaskDone.bEventType = 0;
				Singleton<NetworkModule>.GetInstance().SendLobbyMsg(ref cSPkg, false);
			}
		}

		private void On_Task_List_ElementEnable(CUIEvent uiEvent)
		{
			if (this.m_taskView != null)
			{
				this.m_taskView.On_List_ElementEnable(uiEvent);
			}
		}

		private void On_Task_Submit(CUIEvent uiEvent)
		{
			uint tagUInt = uiEvent.m_eventParams.tagUInt;
			DebugHelper.Assert(tagUInt > 0u, "---ctask Submit task, taskid should > 0");
			if (tagUInt > 0u)
			{
				TaskNetUT.Send_SubmitTask(tagUInt);
			}
		}

		private void OnTask_Week_Reaward1(CUIEvent uiEvent)
		{
			this._check(uiEvent, false);
		}

		private void OnTask_Week_Reaward2(CUIEvent uiEvent)
		{
			this._check(uiEvent, false);
		}

		private void OnTask_Day_Reaward(CUIEvent uiEvent)
		{
			this._check(uiEvent, true);
		}

		private void _check(CUIEvent uiEvent, bool bDay)
		{
			uint tagUInt = uiEvent.m_eventParams.tagUInt;
			DebugHelper.Assert(tagUInt > 0u, "id must > 0");
			if (tagUInt == 0u)
			{
				return;
			}
			ResHuoYueDuReward rewardCfg = this.model.huoyue_data.GetRewardCfg((ushort)tagUInt);
			RES_HUOYUEDU_TYPE type = (!bDay) ? RES_HUOYUEDU_TYPE.RES_HUOYUEDU_TYPE_WEEK : RES_HUOYUEDU_TYPE.RES_HUOYUEDU_TYPE_DAY;
			if (this.model.huoyue_data.BAlready_Reward(type, rewardCfg.wID))
			{
				Singleton<CUIManager>.instance.OpenTips(UT.GetText("CS_HUOYUEDUREWARD_GETED"), false, 1.5f, null, new object[0]);
				return;
			}
			uint num = (!bDay) ? this.model.huoyue_data.week_curNum : this.model.huoyue_data.day_curNum;
			if (num >= rewardCfg.dwHuoYueDu)
			{
				TaskNetUT.Send_GetHuoyue_Reward(rewardCfg.wID);
			}
			else
			{
				Singleton<CUICommonSystem>.instance.OpenUseableTips(this.model.huoyue_data.GetUsable(rewardCfg.wID), uiEvent.m_pointerEventData.pressPosition.x, uiEvent.m_pointerEventData.pressPosition.y, enUseableTipsPos.enTop);
			}
		}

		private void On_TASK_HUOYUEDU_Change()
		{
			if (this.m_taskView != null)
			{
				this.m_taskView.Refresh_Huoyue();
			}
		}

		private void OnTask_LevelElemntEnable(CUIEvent uievent)
		{
			if (this.m_taskView != null)
			{
				this.m_taskView.On_LevelRewardList_ElementEnable(uievent);
			}
		}

		private void OnTask_JumpToReward(CUIEvent uievent)
		{
			if (this.m_taskView != null)
			{
				if (this.model.curLevelRewardData == null)
				{
					return;
				}
				int levelIndex = this.model.GetLevelIndex(this.model.curLevelRewardData.m_level);
				if (levelIndex == -1)
				{
					return;
				}
				int num = 0;
				LevelRewardData curLevelRewardData = null;
				if (!this.model.CalcNextRewardNode(levelIndex, out num, out curLevelRewardData))
				{
					return;
				}
				this.model.curLevelRewardData = curLevelRewardData;
			}
		}

		private void OnTask_ClickGetLevelReward(CUIEvent uievent)
		{
			DebugHelper.Assert(this.model.curLevelRewardData != null, "OnTask_ClickGetLevelReward model.m_curLevelRewardData == null");
			if (this.model.curLevelRewardData == null)
			{
				return;
			}
			TaskNetUT.Send_Get_Level_Reward_Request(this.model.curLevelRewardData.m_level);
		}

		private void OnTask_ClickLevelNode(CUIEvent uievent)
		{
			int srcWidgetIndexInBelongedList = uievent.m_srcWidgetIndexInBelongedList;
			LevelRewardData levelRewardData_ByIndex = this.model.GetLevelRewardData_ByIndex(srcWidgetIndexInBelongedList);
			if (levelRewardData_ByIndex == null)
			{
				return;
			}
			if (this.model.curLevelRewardData != levelRewardData_ByIndex)
			{
				this.model.curLevelRewardData = levelRewardData_ByIndex;
			}
		}
	}
}
