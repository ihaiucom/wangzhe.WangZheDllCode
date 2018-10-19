using Assets.Scripts.UI;
using ResData;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameSystem
{
	public class CFriendMentorTaskView
	{
		public enum EMentorTaskState
		{
			None,
			Empty,
			Tudi_Task,
			TudiTaskFinish_No_MasterTask,
			MasterTask,
			AllFinish
		}

		private enum enMentorTaskViewWidget
		{
			None = -1,
			TopNode,
			TopNodeHeaderTxt,
			TopNodeDescTxt,
			MiddleNode,
			MiddleNodeHeaderTxt,
			MiddleUnlockNode,
			MiddleDoingNode,
			BottomNode,
			BottomNodeHeaderTxt,
			BottomGetRewardBtn,
			BottomUnFinishInfo,
			BottomRewardContainer
		}

		public struct TaskViewConditionData
		{
			public bool bValid;

			public bool bFinish;

			public string condition;

			public string progress;

			public uint taskId;

			public int tag;

			public enUIEventID m_onClickEventID;

			public bool bShowGoToBtn;

			public void Clear()
			{
				this.bValid = false;
			}
		}

		public List<CFriendMentorTaskView.TaskViewConditionData> taskViewConditionDataList = new List<CFriendMentorTaskView.TaskViewConditionData>();

		public void Open()
		{
			Singleton<EventRouter>.instance.AddEventHandler("TaskUpdated", new Action(this.OnMentorTaskUpdate));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Task_Mentor_GetReward, new CUIEventManager.OnUIEventHandler(this.On_Task_Submit));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Task_Mentor_Close, new CUIEventManager.OnUIEventHandler(this.On_Task_Mentor_Close));
			this.ShowMenu();
		}

		public void ShowMenu()
		{
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().GetForm(CFriendContoller.MentorTaskFormPath);
			if (cUIFormScript == null)
			{
				cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(CFriendContoller.MentorTaskFormPath, false, true);
				if (cUIFormScript == null)
				{
					return;
				}
			}
			CFriendMentorTaskView.EMentorTaskState mentorTaskState = Singleton<CTaskSys>.instance.MentorTaskState;
			GameObject gameObject = cUIFormScript.transform.FindChild("content/title").gameObject;
			GameObject gameObject2 = cUIFormScript.transform.FindChild("content/top").gameObject;
			GameObject gameObject3 = cUIFormScript.transform.FindChild("content/middle").gameObject;
			GameObject gameObject4 = cUIFormScript.transform.FindChild("content/bottom").gameObject;
			GameObject gameObject5 = cUIFormScript.transform.FindChild("content/info").gameObject;
			if (mentorTaskState == CFriendMentorTaskView.EMentorTaskState.Empty)
			{
				gameObject.CustomSetActive(false);
				gameObject2.CustomSetActive(false);
				gameObject3.CustomSetActive(false);
				gameObject4.CustomSetActive(false);
				gameObject5.CustomSetActive(true);
				gameObject5.transform.FindChild("txt").GetComponent<Text>().text = Singleton<CTextManager>.instance.GetText("MTV_Empty");
			}
			else if (mentorTaskState == CFriendMentorTaskView.EMentorTaskState.Tudi_Task)
			{
				gameObject.CustomSetActive(true);
				gameObject2.CustomSetActive(true);
				gameObject3.CustomSetActive(true);
				gameObject4.CustomSetActive(true);
				gameObject5.CustomSetActive(false);
				this.showTask(Singleton<CTaskSys>.instance.apprenticeTaskID);
			}
			else if (mentorTaskState == CFriendMentorTaskView.EMentorTaskState.TudiTaskFinish_No_MasterTask)
			{
				gameObject.CustomSetActive(false);
				gameObject2.CustomSetActive(false);
				gameObject3.CustomSetActive(false);
				gameObject4.CustomSetActive(false);
				gameObject5.CustomSetActive(true);
				gameObject5.transform.FindChild("txt").GetComponent<Text>().text = Singleton<CTextManager>.instance.GetText("MTV_MiddleEmpty");
			}
			else if (mentorTaskState == CFriendMentorTaskView.EMentorTaskState.MasterTask)
			{
				gameObject.CustomSetActive(true);
				gameObject2.CustomSetActive(true);
				gameObject3.CustomSetActive(true);
				gameObject4.CustomSetActive(true);
				gameObject5.CustomSetActive(false);
				this.showTask(Singleton<CTaskSys>.instance.masterTaskID);
			}
			else if (mentorTaskState == CFriendMentorTaskView.EMentorTaskState.AllFinish)
			{
				gameObject.CustomSetActive(false);
				gameObject2.CustomSetActive(false);
				gameObject3.CustomSetActive(false);
				gameObject4.CustomSetActive(false);
				gameObject5.CustomSetActive(true);
				gameObject5.transform.FindChild("txt").GetComponent<Text>().text = Singleton<CTextManager>.instance.GetText("MTV_AllFinish");
			}
			if (!string.IsNullOrEmpty(Singleton<CTaskSys>.instance.errorStr))
			{
				gameObject2.CustomSetActive(true);
				gameObject5.CustomSetActive(false);
				cUIFormScript.GetWidget(2).GetComponent<Text>().text = Singleton<CTaskSys>.instance.errorStr;
			}
		}

		private void On_Task_Mentor_Close(CUIEvent uievent)
		{
			this.Clear();
		}

		public void Clear()
		{
			Singleton<EventRouter>.instance.RemoveEventHandler("TaskUpdated", new Action(this.OnMentorTaskUpdate));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Task_Mentor_GetReward, new CUIEventManager.OnUIEventHandler(this.On_Task_Submit));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Task_Mentor_Close, new CUIEventManager.OnUIEventHandler(this.On_Task_Mentor_Close));
			if (Singleton<CFriendContoller>.instance.view != null)
			{
				Singleton<CFriendContoller>.instance.view.Refresh_Tab();
			}
		}

		public void showTask(uint taskid)
		{
			CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CFriendContoller.MentorTaskFormPath);
			if (form == null)
			{
				return;
			}
			CTask task = Singleton<CTaskSys>.instance.model.GetTask(taskid);
			if (task == null)
			{
				Singleton<CTaskSys>.instance.errorStr = string.Format("{0}, inside showTask can't find task, id:{1},  {2}", Singleton<CTaskSys>.instance.errorStr, taskid, Singleton<CTaskSys>.instance.delStr);
				return;
			}
			Text componetInChild = Utility.GetComponetInChild<Text>(form.gameObject, "content/title/taskName");
			componetInChild.text = task.m_taskTitle;
			form.GetWidget(1).GetComponent<Text>().text = Singleton<CTextManager>.instance.GetText("MTV_TopNodeHeaderTxt");
			form.GetWidget(2).GetComponent<Text>().text = task.m_taskDesc;
			form.GetWidget(4).GetComponent<Text>().text = Singleton<CTextManager>.instance.GetText("MTV_MiddleNodeHeaderTxt");
			List<CFriendMentorTaskView.TaskViewConditionData> list = this.CalcParam(task, null, false);
			int num = Math.Min(3, list.Count);
			for (int i = 0; i < num; i++)
			{
				CFriendMentorTaskView.TaskViewConditionData taskViewConditionData = list[i];
				GameObject widget = form.GetWidget(6);
				GameObject gameObject = widget.transform.FindChild(string.Format("cond_{0}", i)).gameObject;
				if (!(gameObject == null))
				{
					if (taskViewConditionData.bValid)
					{
						gameObject.CustomSetActive(true);
						gameObject.transform.FindChild("desc").GetComponent<Text>().text = taskViewConditionData.condition;
						gameObject.transform.FindChild("progress").GetComponent<Text>().text = taskViewConditionData.progress;
						CUIEventScript component = gameObject.transform.FindChild("btns/goto_btn").GetComponent<CUIEventScript>();
						if (component != null)
						{
							component.m_onClickEventID = taskViewConditionData.m_onClickEventID;
							component.m_onClickEventParams.tag = taskViewConditionData.tag;
							component.m_onClickEventParams.taskId = taskViewConditionData.taskId;
						}
						if (taskViewConditionData.bFinish)
						{
							component.gameObject.CustomSetActive(false);
						}
						else
						{
							component.gameObject.CustomSetActive(taskViewConditionData.bShowGoToBtn);
						}
						gameObject.transform.FindChild("btns/Text").gameObject.CustomSetActive(taskViewConditionData.bFinish);
					}
					else
					{
						gameObject.CustomSetActive(false);
					}
				}
			}
			CTaskView.CTaskUT.ShowTaskAward(form, task, form.GetWidget(11), 4);
			if (task.m_taskState == 1)
			{
				GameObject gameObject2 = form.GetWidget(9).gameObject;
				gameObject2.CustomSetActive(true);
				gameObject2.GetComponent<CUIEventScript>().m_onClickEventParams.tagUInt = task.m_baseId;
				form.GetWidget(10).gameObject.CustomSetActive(false);
			}
			else
			{
				form.GetWidget(9).gameObject.CustomSetActive(false);
				form.GetWidget(10).gameObject.CustomSetActive(false);
			}
		}

		private void OnMentorTaskUpdate()
		{
			this.ShowMenu();
		}

		public void ShowTask(uint taskID)
		{
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

		public List<CFriendMentorTaskView.TaskViewConditionData> CalcParam(CTask task, GameObject goto_obj, bool isMonthWeekCard = false)
		{
			if (task == null)
			{
				return null;
			}
			for (int i = 0; i < this.taskViewConditionDataList.Count; i++)
			{
				this.taskViewConditionDataList[i].Clear();
			}
			this.taskViewConditionDataList.Clear();
			if (isMonthWeekCard)
			{
				goto_obj.CustomSetActive(true);
				CFriendMentorTaskView.TaskViewConditionData item = default(CFriendMentorTaskView.TaskViewConditionData);
				item.bValid = true;
				item.m_onClickEventID = enUIEventID.Mall_Open_Factory_Shop_Tab;
				this.taskViewConditionDataList.Add(item);
				item.bShowGoToBtn = true;
			}
			else
			{
				for (int j = 0; j < task.m_prerequisiteInfo.Length; j++)
				{
					CFriendMentorTaskView.TaskViewConditionData item2 = default(CFriendMentorTaskView.TaskViewConditionData);
					item2.bValid = false;
					item2.bFinish = task.m_prerequisiteInfo[j].m_isReach;
					ResDT_PrerequisiteInTask resDT_PrerequisiteInTask = task.m_resTask.astPrerequisiteArray[j];
					item2.condition = resDT_PrerequisiteInTask.szPrerequisiteDesc;
					RES_PERREQUISITE_TYPE conditionType = (RES_PERREQUISITE_TYPE)task.m_prerequisiteInfo[j].m_conditionType;
					if (task.m_prerequisiteInfo[j].m_valueTarget > 0)
					{
						item2.bValid = true;
						if (conditionType != RES_PERREQUISITE_TYPE.RES_PERREQUISITE_ACNTLVL && conditionType != RES_PERREQUISITE_TYPE.RES_PERREQUISITE_PVPLVL)
						{
							item2.progress = string.Concat(new object[]
							{
								task.m_prerequisiteInfo[j].m_value,
								"/",
								task.m_prerequisiteInfo[j].m_valueTarget,
								" "
							});
						}
						if (!task.m_prerequisiteInfo[j].m_isReach)
						{
							if (conditionType == RES_PERREQUISITE_TYPE.RES_PERREQUISITE_STAGECLEARPVE)
							{
								int iParam = task.m_resTask.astPrerequisiteArray[j].astPrerequisiteParam[3].iParam;
								if ((float)iParam == Mathf.Pow(2f, 0f))
								{
									goto_obj.CustomSetActive(true);
									item2.bShowGoToBtn = true;
									item2.taskId = task.m_baseId;
									item2.tag = task.m_resTask.astPrerequisiteArray[j].astPrerequisiteParam[4].iParam;
									item2.m_onClickEventID = enUIEventID.Task_LinkPve;
								}
								else if ((float)iParam == Mathf.Pow(2f, 7f))
								{
									goto_obj.CustomSetActive(true);
									item2.bShowGoToBtn = true;
									item2.m_onClickEventID = enUIEventID.Burn_OpenForm;
								}
								else if ((float)iParam == Mathf.Pow(2f, 8f))
								{
									goto_obj.CustomSetActive(true);
									item2.bShowGoToBtn = true;
									item2.m_onClickEventID = enUIEventID.Arena_OpenForm;
								}
							}
							else if (conditionType == RES_PERREQUISITE_TYPE.RES_PERREQUISITE_STAGECLEARPVP || conditionType == RES_PERREQUISITE_TYPE.RES_PERREQUISITE_PVPKILLCNT)
							{
								goto_obj.CustomSetActive(true);
								item2.bShowGoToBtn = true;
								item2.m_onClickEventID = enUIEventID.Matching_OpenEntry;
								item2.tag = 0;
							}
							else if (conditionType == RES_PERREQUISITE_TYPE.RES_PERREQUISITE_FRIENDCNT)
							{
								goto_obj.CustomSetActive(false);
								item2.bShowGoToBtn = false;
								item2.m_onClickEventID = enUIEventID.Friend_OpenForm;
							}
							else if (conditionType == RES_PERREQUISITE_TYPE.RES_PERREQUISITE_PVPLVL)
							{
								goto_obj.CustomSetActive(true);
								item2.bShowGoToBtn = true;
								item2.m_onClickEventID = enUIEventID.Matching_OpenEntry;
								item2.tag = 0;
							}
							else if (conditionType == RES_PERREQUISITE_TYPE.RES_PERREQUISITE_GUILDOPT)
							{
								if ((long)task.m_resTask.astPrerequisiteArray[0].astPrerequisiteParam[1].iParam == 2L)
								{
									goto_obj.CustomSetActive(true);
									item2.bShowGoToBtn = true;
									item2.m_onClickEventID = enUIEventID.Matching_OpenEntry;
									item2.tag = 3;
								}
								else
								{
									goto_obj.CustomSetActive(false);
									item2.bShowGoToBtn = false;
								}
							}
							else if (conditionType == RES_PERREQUISITE_TYPE.RES_PERREQUISITE_ARENAOPT)
							{
								goto_obj.CustomSetActive(true);
								item2.bShowGoToBtn = true;
								item2.m_onClickEventID = enUIEventID.Arena_OpenForm;
							}
							else if (conditionType == RES_PERREQUISITE_TYPE.RES_PERREQUISITE_SYMBOLCOMP)
							{
								goto_obj.CustomSetActive(true);
								item2.bShowGoToBtn = true;
								item2.m_onClickEventID = enUIEventID.Symbol_OpenForm;
							}
							else if (conditionType == RES_PERREQUISITE_TYPE.RES_PERREQUISITE_BUYOPT)
							{
								goto_obj.CustomSetActive(true);
								item2.bShowGoToBtn = true;
								item2.m_onClickEventID = enUIEventID.Mall_Open_Factory_Shop_Tab;
							}
							else if (conditionType == RES_PERREQUISITE_TYPE.RES_PERREQUISITE_OPENBOXCNT)
							{
								goto_obj.CustomSetActive(true);
								item2.bShowGoToBtn = true;
								item2.m_onClickEventID = enUIEventID.Lottery_Open_Form;
							}
							else if (conditionType == RES_PERREQUISITE_TYPE.RES_PERREQUISITE_DUOBAO)
							{
								goto_obj.CustomSetActive(true);
								item2.bShowGoToBtn = true;
								if ((long)task.m_resTask.astPrerequisiteArray[0].astPrerequisiteParam[1].iParam == 1L)
								{
									item2.m_onClickEventID = enUIEventID.Mall_GotoDianmondTreasureTab;
								}
								else if ((long)task.m_resTask.astPrerequisiteArray[0].astPrerequisiteParam[1].iParam == 2L)
								{
									item2.m_onClickEventID = enUIEventID.Mall_GotoCouponsTreasureTab;
								}
							}
							else if (conditionType == RES_PERREQUISITE_TYPE.RES_PERREQUISITE_RECALLFRIEND)
							{
								goto_obj.CustomSetActive(true);
								item2.bShowGoToBtn = true;
								item2.m_onClickEventID = enUIEventID.Friend_OpenForm;
							}
							else
							{
								goto_obj.CustomSetActive(false);
								item2.bShowGoToBtn = false;
							}
						}
						else
						{
							goto_obj.CustomSetActive(false);
							item2.bShowGoToBtn = false;
						}
					}
					this.taskViewConditionDataList.Add(item2);
				}
			}
			return this.taskViewConditionDataList;
		}
	}
}
