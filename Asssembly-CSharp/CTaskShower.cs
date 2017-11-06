using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using ResData;
using System;
using UnityEngine;
using UnityEngine.UI;

public class CTaskShower : MonoBehaviour
{
	public uint taskID;

	public Text taskTitle;

	public Text taskDesc;

	public GameObject taskIcon_back;

	public GameObject taskIcon_front;

	public Text progress;

	public GameObject progressObj;

	public GameObject m_awardContainer;

	public GameObject has_finish;

	public GameObject award_obj;

	public GameObject goto_obj;

	public void ShowTask(CTask task, CUIFormScript fromScript)
	{
		if (task == null || fromScript == null)
		{
			return;
		}
		this.taskID = task.m_baseId;
		if (this.taskTitle != null)
		{
			this.taskTitle.set_text(task.m_taskTitle);
		}
		if (this.taskDesc != null)
		{
			this.taskDesc.set_text(task.m_taskDesc);
		}
		this.taskIcon_back.CustomSetActive(true);
		this.taskIcon_front.CustomSetActive(true);
		if (task.m_resTask.bTaskIconShowType == 0)
		{
			this.taskIcon_front.GetComponent<Image>().enabled = false;
			this.taskIcon_back.GetComponent<Image>().enabled = true;
			this.taskIcon_back.GetComponent<Image>().SetSprite(task.m_resTask.szTaskBgIcon, fromScript, true, false, false, false);
		}
		else if (task.m_resTask.bTaskIconShowType == 1)
		{
			this.taskIcon_back.GetComponent<Image>().enabled = false;
			this.taskIcon_front.GetComponent<Image>().enabled = true;
			this.taskIcon_front.GetComponent<Image>().SetSprite(task.m_resTask.szTaskIcon, fromScript, true, false, false, false);
		}
		else
		{
			this.taskIcon_back.GetComponent<Image>().enabled = true;
			this.taskIcon_front.GetComponent<Image>().enabled = true;
			this.taskIcon_back.GetComponent<Image>().SetSprite(task.m_resTask.szTaskBgIcon, fromScript, true, false, false, false);
			this.taskIcon_front.GetComponent<Image>().SetSprite(task.m_resTask.szTaskIcon, fromScript, true, false, false, false);
		}
		this.award_obj.GetComponent<CUIEventScript>().m_onClickEventParams.tagUInt = this.taskID;
		this.award_obj.CustomSetActive(false);
		bool flag = this.taskID == Singleton<CTaskSys>.instance.monthCardTaskID || this.taskID == Singleton<CTaskSys>.instance.weekCardTaskID;
		if (task.m_taskState == 0 || task.m_taskState == 4)
		{
			CUIEventScript component = this.goto_obj.GetComponent<CUIEventScript>();
			if (flag)
			{
				this.goto_obj.CustomSetActive(true);
				component.m_onClickEventID = enUIEventID.Mall_Open_Factory_Shop_Tab;
			}
			else
			{
				string text = string.Empty;
				for (int i = 0; i < task.m_prerequisiteInfo.Length; i++)
				{
					RES_PERREQUISITE_TYPE conditionType = (RES_PERREQUISITE_TYPE)task.m_prerequisiteInfo[i].m_conditionType;
					if (task.m_prerequisiteInfo[i].m_valueTarget > 0)
					{
						if (conditionType != RES_PERREQUISITE_TYPE.RES_PERREQUISITE_ACNTLVL && conditionType != RES_PERREQUISITE_TYPE.RES_PERREQUISITE_PVPLVL)
						{
							string text2 = text;
							text = string.Concat(new object[]
							{
								text2,
								task.m_prerequisiteInfo[i].m_value,
								"/",
								task.m_prerequisiteInfo[i].m_valueTarget,
								" "
							});
							this.progressObj.CustomSetActive(true);
							this.progress.gameObject.CustomSetActive(true);
						}
						else
						{
							this.progressObj.CustomSetActive(false);
							this.progress.gameObject.CustomSetActive(false);
						}
						if (!task.m_prerequisiteInfo[i].m_isReach)
						{
							if (conditionType == RES_PERREQUISITE_TYPE.RES_PERREQUISITE_STAGECLEARPVE)
							{
								int iParam = task.m_resTask.astPrerequisiteArray[i].astPrerequisiteParam[3].iParam;
								if ((float)iParam == Mathf.Pow(2f, 0f))
								{
									this.goto_obj.CustomSetActive(true);
									component.m_onClickEventParams.taskId = task.m_baseId;
									component.m_onClickEventParams.tag = task.m_resTask.astPrerequisiteArray[i].astPrerequisiteParam[4].iParam;
									component.m_onClickEventID = enUIEventID.Task_LinkPve;
								}
								else if ((float)iParam == Mathf.Pow(2f, 7f))
								{
									this.goto_obj.CustomSetActive(true);
									component.m_onClickEventID = enUIEventID.Burn_OpenForm;
								}
								else if ((float)iParam == Mathf.Pow(2f, 8f))
								{
									this.goto_obj.CustomSetActive(true);
									component.m_onClickEventID = enUIEventID.Arena_OpenForm;
								}
							}
							else if (conditionType == RES_PERREQUISITE_TYPE.RES_PERREQUISITE_STAGECLEARPVP || conditionType == RES_PERREQUISITE_TYPE.RES_PERREQUISITE_PVPKILLCNT)
							{
								this.goto_obj.CustomSetActive(true);
								if (task.m_resTask.astPrerequisiteArray[0].astPrerequisiteParam[3].iParam == 16)
								{
									component.m_onClickEventID = enUIEventID.Matching_OpenLadder;
								}
								else
								{
									component.m_onClickEventID = enUIEventID.Matching_OpenEntry;
									component.m_onClickEventParams = default(stUIEventParams);
								}
							}
							else if (conditionType == RES_PERREQUISITE_TYPE.RES_PERREQUISITE_FRIENDCNT)
							{
								this.goto_obj.CustomSetActive(false);
								component.m_onClickEventID = enUIEventID.Friend_OpenForm;
							}
							else if (conditionType == RES_PERREQUISITE_TYPE.RES_PERREQUISITE_PVPLVL)
							{
								this.goto_obj.CustomSetActive(true);
								component.m_onClickEventID = enUIEventID.Matching_OpenEntry;
								component.m_onClickEventParams = default(stUIEventParams);
							}
							else if (conditionType == RES_PERREQUISITE_TYPE.RES_PERREQUISITE_GUILDOPT)
							{
								if ((long)task.m_resTask.astPrerequisiteArray[0].astPrerequisiteParam[1].iParam == 2L)
								{
									this.goto_obj.CustomSetActive(true);
									component.m_onClickEventID = enUIEventID.Matching_OpenEntry;
									component.m_onClickEventParams = new stUIEventParams
									{
										tag = 3
									};
								}
								else
								{
									this.goto_obj.CustomSetActive(false);
								}
							}
							else if (conditionType == RES_PERREQUISITE_TYPE.RES_PERREQUISITE_ARENAOPT)
							{
								this.goto_obj.CustomSetActive(true);
								component.m_onClickEventID = enUIEventID.Arena_OpenForm;
							}
							else if (conditionType == RES_PERREQUISITE_TYPE.RES_PERREQUISITE_SYMBOLCOMP)
							{
								this.goto_obj.CustomSetActive(true);
								component.m_onClickEventID = enUIEventID.Symbol_OpenForm;
							}
							else if (conditionType == RES_PERREQUISITE_TYPE.RES_PERREQUISITE_BUYOPT)
							{
								this.goto_obj.CustomSetActive(true);
								component.m_onClickEventID = enUIEventID.Mall_Open_Factory_Shop_Tab;
							}
							else if (conditionType == RES_PERREQUISITE_TYPE.RES_PERREQUISITE_OPENBOXCNT)
							{
								this.goto_obj.CustomSetActive(true);
								component.m_onClickEventID = enUIEventID.Lottery_Open_Form;
							}
							else if (conditionType == RES_PERREQUISITE_TYPE.RES_PERREQUISITE_DUOBAO)
							{
								this.goto_obj.CustomSetActive(true);
								if ((long)task.m_resTask.astPrerequisiteArray[0].astPrerequisiteParam[1].iParam == 1L)
								{
									component.m_onClickEventID = enUIEventID.Mall_GotoDianmondTreasureTab;
								}
								else if ((long)task.m_resTask.astPrerequisiteArray[0].astPrerequisiteParam[1].iParam == 2L)
								{
									component.m_onClickEventID = enUIEventID.Mall_GotoCouponsTreasureTab;
								}
							}
							else if (conditionType == RES_PERREQUISITE_TYPE.RES_PERREQUISITE_RECALLFRIEND)
							{
								this.goto_obj.CustomSetActive(true);
								component.m_onClickEventID = enUIEventID.Friend_OpenForm;
							}
							else
							{
								this.goto_obj.CustomSetActive(false);
							}
						}
						else
						{
							this.goto_obj.CustomSetActive(false);
						}
					}
				}
				if (this.progress != null)
				{
					this.progress.set_text(text);
				}
			}
		}
		else if (task.m_taskState == 1)
		{
			this.award_obj.CustomSetActive(true);
			this.goto_obj.CustomSetActive(false);
		}
		if (task.m_taskState == 1 || task.m_taskState == 3)
		{
			this.goto_obj.CustomSetActive(false);
			this.progressObj.CustomSetActive(false);
			this.progress.gameObject.CustomSetActive(false);
		}
		if (flag)
		{
			this.progress.gameObject.CustomSetActive(true);
			this.progressObj.CustomSetActive(true);
			this.progressObj.GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("RemainDays"));
			uint num = (this.taskID == Singleton<CTaskSys>.instance.monthCardTaskID) ? Singleton<CTaskSys>.instance.monthCardExpireTime : Singleton<CTaskSys>.instance.weekCardExpireTime;
			uint currentUTCTime = (uint)CRoleInfo.GetCurrentUTCTime();
			int num2 = Mathf.CeilToInt(((num > currentUTCTime) ? (num - currentUTCTime) : 0f) / 86400f);
			if (task.m_taskState == 3 && num2 > 0)
			{
				num2--;
			}
			this.progress.set_text(num2.ToString());
		}
		else
		{
			this.progressObj.GetComponent<Text>().set_text(Singleton<CTextManager>.GetInstance().GetText("Progress"));
		}
		this.has_finish.CustomSetActive(task.m_taskState == 3);
		CTaskView.CTaskUT.ShowTaskAward(fromScript, task, this.m_awardContainer, 2);
	}

	public void ShowTask(ResTask task, CUIFormScript fromScript)
	{
		if (task == null || fromScript == null)
		{
			return;
		}
		this.taskID = task.dwTaskID;
		if (this.taskTitle != null)
		{
			this.taskTitle.set_text(task.szTaskName);
		}
		if (this.taskDesc != null)
		{
			this.taskDesc.set_text(task.szTaskDesc);
		}
		this.taskIcon_back.CustomSetActive(true);
		this.taskIcon_front.CustomSetActive(true);
		if (task.bTaskIconShowType == 0)
		{
			this.taskIcon_front.GetComponent<Image>().enabled = false;
			this.taskIcon_back.GetComponent<Image>().enabled = true;
			this.taskIcon_back.GetComponent<Image>().SetSprite(task.szTaskBgIcon, fromScript, true, false, false, false);
		}
		else if (task.bTaskIconShowType == 1)
		{
			this.taskIcon_back.GetComponent<Image>().enabled = false;
			this.taskIcon_front.GetComponent<Image>().enabled = true;
			this.taskIcon_front.GetComponent<Image>().SetSprite(task.szTaskIcon, fromScript, true, false, false, false);
		}
		else
		{
			this.taskIcon_back.GetComponent<Image>().enabled = true;
			this.taskIcon_front.GetComponent<Image>().enabled = true;
			this.taskIcon_back.GetComponent<Image>().SetSprite(task.szTaskBgIcon, fromScript, true, false, false, false);
			this.taskIcon_front.GetComponent<Image>().SetSprite(task.szTaskIcon, fromScript, true, false, false, false);
		}
		this.progressObj.CustomSetActive(false);
		this.progress.gameObject.CustomSetActive(false);
		this.award_obj.CustomSetActive(false);
		this.goto_obj.CustomSetActive(false);
		this.has_finish.CustomSetActive(true);
		CTaskView.CTaskUT.ShowTaskAward(fromScript, task, this.m_awardContainer);
	}
}
