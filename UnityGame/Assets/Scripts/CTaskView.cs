using Assets.Scripts.Framework;
using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using ResData;
using System;
using UnityEngine;
using UnityEngine.UI;

public class CTaskView
{
	public enum enTaskFormWidget
	{
		None = -1,
		Reserve,
		tablistScript,
		tasklistScript_main,
		tasklistScript_usual,
		tasklistScript_mishu,
		week_huoyue_text,
		week_node1,
		week_node2,
		m_container,
		day_progress_bg,
		day_progress,
		day_huoyue_txt,
		unlock_node,
		getReward_btn,
		task_Node,
		iwantgold_taskList,
		iwantgold_week_progress,
		iwantgold_text,
		tasklistScript_iwantgold
	}

	public enum LevelRewardTaskWidget
	{
		None = -1,
		Reserve,
		Entity_0,
		Entity_1,
		Entity_2
	}

	public class CTaskUT
	{
		public static void ShowTaskAward(CUIFormScript formScript, CTask task, GameObject awardContainer, int awardItemcount = 2)
		{
			if (formScript == null || awardContainer == null || task.m_baseId == 0u)
			{
				return;
			}
			ResTaskReward resAward = task.resAward;
			if (resAward == null)
			{
				return;
			}
			for (int i = 0; i < awardItemcount; i++)
			{
				ResTaskRewardDetail resTaskRewardDetail = resAward.astRewardInfo[i];
				GameObject gameObject = awardContainer.GetComponent<Transform>().FindChild(string.Format("itemCell{0}", i)).gameObject;
				if (resTaskRewardDetail != null && resTaskRewardDetail.iCnt > 0)
				{
					RES_REWARDS_TYPE dwRewardType = (RES_REWARDS_TYPE)resTaskRewardDetail.dwRewardType;
					CUseable itemUseable = CUseableManager.CreateUsableByServerType(dwRewardType, resTaskRewardDetail.iCnt, resTaskRewardDetail.dwRewardID);
					CUICommonSystem.SetItemCell(formScript, gameObject, itemUseable, true, false, false, false);
					gameObject.transform.FindChild("lblIconCount").GetComponent<Text>().set_text(string.Format("x{0}", resTaskRewardDetail.iCnt.ToString()));
					gameObject.gameObject.CustomSetActive(true);
				}
				else
				{
					gameObject.CustomSetActive(false);
				}
			}
		}

		public static void ShowTaskAward(CUIFormScript formScript, ResTask task, GameObject awardContainer)
		{
			if (formScript == null || awardContainer == null || task == null || task.dwTaskID == 0u)
			{
				return;
			}
			ResTaskReward dataByKey = GameDataMgr.taskRewardDatabin.GetDataByKey(task.dwRewardID);
			if (dataByKey == null)
			{
				return;
			}
			for (int i = 0; i < dataByKey.astRewardInfo.Length; i++)
			{
				ResTaskRewardDetail resTaskRewardDetail = dataByKey.astRewardInfo[i];
				Transform transform = awardContainer.transform.FindChild(string.Format("itemCell{0}", i));
				if (!(transform == null))
				{
					GameObject gameObject = transform.gameObject;
					if (!(gameObject == null))
					{
						if (resTaskRewardDetail != null && resTaskRewardDetail.iCnt > 0)
						{
							RES_REWARDS_TYPE dwRewardType = (RES_REWARDS_TYPE)resTaskRewardDetail.dwRewardType;
							CUseable itemUseable = CUseableManager.CreateUsableByServerType(dwRewardType, resTaskRewardDetail.iCnt, resTaskRewardDetail.dwRewardID);
							CUICommonSystem.SetItemCell(formScript, gameObject, itemUseable, true, false, false, false);
							gameObject.transform.FindChild("lblIconCount").GetComponent<Text>().set_text(string.Format("x{0}", resTaskRewardDetail.iCnt.ToString()));
							gameObject.gameObject.CustomSetActive(true);
						}
						else
						{
							gameObject.CustomSetActive(false);
						}
					}
				}
			}
		}
	}

	private CUIFormScript m_CUIForm;

	private int m_tabIndex = -1;

	private CUIListScript tablistScript;

	private CUIListScript tasklistScript_main;

	private CUIListScript tasklistScript_usual;

	private CUIListScript tasklistScript_mishu;

	private CUIListScript tasklistScript_iwantgold;

	private Text week_huoyue_text;

	private GameObject week_node1;

	private GameObject week_node2;

	private CUIContainerScript m_container;

	private Image day_progress_bg;

	private Image day_progress;

	private Text day_huoyue_txt;

	public GameObject m_mainTaskNode;

	public GameObject m_unlockNode;

	public GameObject m_levelRewardNode;

	public CUIListScript m_levelRewardList;

	public GameObject m_normalTaskNode;

	public GameObject m_emptyTaskNode;

	public GameObject m_taskNode0;

	public GameObject m_taskNode1;

	public Text m_unlockInfoTxt;

	public Text m_levelRewardText;

	private GameObject jumpRewardGameObject;

	private float m_orgWeekgoldProgress = -0.1f;

	private bool bCreateDayNode;

	private static int LevelValue = 21;

	public int TabIndex
	{
		get
		{
			return this.m_tabIndex;
		}
		set
		{
			if (this.m_tabIndex == value)
			{
				return;
			}
			this.m_tabIndex = value;
			Singleton<CTaskSys>.instance.Increse(this.m_tabIndex);
			switch (this.m_tabIndex)
			{
			case 0:
				if (this.m_mainTaskNode != null)
				{
					this.m_mainTaskNode.CustomSetActive(true);
				}
				if (this.tasklistScript_usual != null)
				{
					this.tasklistScript_usual.transform.parent.gameObject.CustomSetActive(false);
				}
				if (this.tasklistScript_mishu != null)
				{
					this.tasklistScript_mishu.transform.parent.gameObject.CustomSetActive(false);
				}
				if (this.tasklistScript_iwantgold != null)
				{
					this.tasklistScript_iwantgold.transform.parent.gameObject.CustomSetActive(false);
				}
				break;
			case 1:
				if (this.tasklistScript_usual != null)
				{
					this.tasklistScript_usual.transform.parent.gameObject.CustomSetActive(true);
				}
				if (this.tasklistScript_mishu != null)
				{
					this.tasklistScript_mishu.transform.parent.gameObject.CustomSetActive(false);
				}
				if (this.m_mainTaskNode != null)
				{
					this.m_mainTaskNode.CustomSetActive(false);
				}
				if (this.tasklistScript_iwantgold != null)
				{
					this.tasklistScript_iwantgold.transform.parent.gameObject.CustomSetActive(false);
				}
				this.Refresh_List(this.m_tabIndex);
				break;
			case 2:
				if (this.m_mainTaskNode != null)
				{
					this.m_mainTaskNode.CustomSetActive(false);
				}
				if (this.tasklistScript_usual != null)
				{
					this.tasklistScript_usual.transform.parent.gameObject.CustomSetActive(false);
				}
				if (this.tasklistScript_mishu != null)
				{
					this.tasklistScript_mishu.transform.parent.gameObject.CustomSetActive(false);
				}
				if (this.tasklistScript_iwantgold != null)
				{
					this.tasklistScript_iwantgold.transform.parent.gameObject.CustomSetActive(true);
					Singleton<CMiShuSystem>.instance.InitList(this.m_tabIndex, this.tasklistScript_iwantgold);
					CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.GetInstance().GetMasterRoleInfo();
					if (masterRoleInfo != null)
					{
						Text component = this.m_CUIForm.GetWidget(17).GetComponent<Text>();
						component.set_text(Singleton<CTextManager>.GetInstance().GetText("GoldLimited_1", new string[]
						{
							masterRoleInfo.m_goldWeekCur.ToString(),
							masterRoleInfo.m_goldWeekLimit.ToString()
						}));
						Image component2 = this.m_CUIForm.GetWidget(16).GetComponent<Image>();
						if (this.m_orgWeekgoldProgress < 0f)
						{
							this.m_orgWeekgoldProgress = (component2.transform as RectTransform).sizeDelta.x;
						}
						float orgWeekgoldProgress = this.m_orgWeekgoldProgress;
						float y = (component2.transform as RectTransform).sizeDelta.y;
						uint goldWeekLimit = masterRoleInfo.m_goldWeekLimit;
						float num = masterRoleInfo.m_goldWeekCur / masterRoleInfo.m_goldWeekLimit * orgWeekgoldProgress;
						if (num > orgWeekgoldProgress)
						{
							num = orgWeekgoldProgress;
						}
						(component2.transform as RectTransform).sizeDelta = new Vector2(num, y);
					}
				}
				break;
			default:
				if (this.m_mainTaskNode != null)
				{
					this.m_mainTaskNode.CustomSetActive(false);
				}
				if (this.tasklistScript_usual != null)
				{
					this.tasklistScript_usual.transform.parent.gameObject.CustomSetActive(false);
				}
				if (this.tasklistScript_iwantgold != null)
				{
					this.tasklistScript_iwantgold.transform.parent.gameObject.CustomSetActive(false);
				}
				if (this.tasklistScript_mishu != null)
				{
					this.tasklistScript_mishu.transform.parent.gameObject.CustomSetActive(true);
					Singleton<CMiShuSystem>.instance.InitList(this.m_tabIndex, this.tasklistScript_mishu);
				}
				break;
			}
		}
	}

	public void Clear()
	{
		this.bCreateDayNode = false;
		this.m_tabIndex = -1;
		if (this.m_container != null)
		{
			this.m_container.RecycleAllElement();
		}
		this.m_container = null;
		this.day_progress_bg = (this.day_progress = null);
		this.day_huoyue_txt = null;
		this.tasklistScript_main = null;
		this.tasklistScript_usual = null;
		this.week_huoyue_text = null;
		this.week_node1 = null;
		this.week_node2 = null;
		this.tablistScript = null;
		this.m_CUIForm = null;
		this.m_mainTaskNode = null;
		this.m_unlockNode = null;
		this.m_levelRewardNode = null;
		this.m_levelRewardList = null;
		this.m_mainTaskNode = null;
		this.m_unlockNode = null;
		this.m_levelRewardNode = null;
		this.m_levelRewardList = null;
		this.m_normalTaskNode = null;
		this.m_emptyTaskNode = null;
		this.m_taskNode0 = null;
		this.m_taskNode1 = null;
		this.m_unlockInfoTxt = null;
		this.m_levelRewardText = null;
		this.jumpRewardGameObject = null;
		Singleton<CTaskSys>.instance.model.curLevelRewardData = null;
		CUIFormScript form = Singleton<CUIManager>.GetInstance().GetForm(CTaskSys.TASK_LevelRewardFORM_PATH);
		if (form != null)
		{
			Singleton<CUIManager>.GetInstance().CloseForm(CTaskSys.TASK_LevelRewardFORM_PATH);
		}
	}

	public void OpenForm(CUIEvent uiEvent)
	{
		this.m_CUIForm = Singleton<CUIManager>.GetInstance().OpenForm(CTaskSys.TASK_FORM_PATH, true, true);
		this.tablistScript = this.m_CUIForm.GetWidget(1).GetComponent<CUIListScript>();
		this.tasklistScript_usual = this.m_CUIForm.GetWidget(3).GetComponent<CUIListScript>();
		this.tasklistScript_mishu = this.m_CUIForm.GetWidget(4).GetComponent<CUIListScript>();
		this.tasklistScript_iwantgold = this.m_CUIForm.GetWidget(18).GetComponent<CUIListScript>();
		this.m_mainTaskNode = this.m_CUIForm.transform.Find("node/list_node_main").gameObject;
		this.m_unlockNode = this.m_CUIForm.transform.Find("node/list_node_main/unlock_node").gameObject;
		this.m_levelRewardNode = this.m_CUIForm.transform.Find("node/list_node_main/reward_node").gameObject;
		this.m_levelRewardList = this.m_CUIForm.transform.Find("node/list_node_main/levelList").GetComponent<CUIListScript>();
		this.m_unlockInfoTxt = this.m_CUIForm.transform.Find("node/list_node_main/unlock_node/Text").GetComponent<Text>();
		this.m_levelRewardText = this.m_CUIForm.transform.Find("node/list_node_main/reward_node/Text").GetComponent<Text>();
		this.m_normalTaskNode = this.m_CUIForm.transform.Find("node/list_node_main/task_node/normal").gameObject;
		this.m_emptyTaskNode = this.m_CUIForm.transform.Find("node/list_node_main/task_node/noTask").gameObject;
		this.m_taskNode0 = this.m_CUIForm.transform.Find("node/list_node_main/task_node/normal/task_0").gameObject;
		this.m_taskNode1 = this.m_CUIForm.transform.Find("node/list_node_main/task_node/normal/task_1").gameObject;
		this.jumpRewardGameObject = this.m_CUIForm.transform.Find("node/list_node_main/levelList/goto_btn").gameObject;
		DebugHelper.Assert(this.m_mainTaskNode != null, "ctaskview m_mainTaskNode == null");
		DebugHelper.Assert(this.m_unlockNode != null, "ctaskview m_unlockNode == null");
		DebugHelper.Assert(this.m_levelRewardNode != null, "ctaskview m_levelRewardNode == null");
		DebugHelper.Assert(this.m_levelRewardList != null, "ctaskview m_levelRewardList == null");
		DebugHelper.Assert(this.m_unlockInfoTxt != null, "ctaskview m_unlockInfoTxt == null");
		DebugHelper.Assert(this.m_levelRewardText != null, "ctaskview m_levelRewardText == null");
		DebugHelper.Assert(this.m_normalTaskNode != null, "ctaskview m_normalTaskNode == null");
		DebugHelper.Assert(this.m_emptyTaskNode != null, "ctaskview m_emptyTaskNode == null");
		DebugHelper.Assert(this.m_taskNode0 != null, "ctaskview m_taskNode0 == null");
		DebugHelper.Assert(this.m_taskNode1 != null, "ctaskview m_taskNode1 == null");
		DebugHelper.Assert(this.m_taskNode1 != null, "ctaskview jumpRewardGameObject == null");
		CTaskModel model = Singleton<CTaskSys>.instance.model;
		string[] array = new string[]
		{
			model.Daily_Quest_Career,
			model.Daily_Quest_NeedGrowing,
			model.Daily_Quest_NeedMoney,
			model.Daily_Quest_NeedSeal,
			model.Daily_Quest_NeedHero
		};
		this.tablistScript.SetElementAmount(array.Length);
		for (int i = 0; i < this.tablistScript.m_elementAmount; i++)
		{
			CUIListElementScript elemenet = this.tablistScript.GetElemenet(i);
			Text component = elemenet.gameObject.transform.FindChild("Text").GetComponent<Text>();
			component.set_text(array[i]);
		}
		this.tablistScript.m_alwaysDispatchSelectedChangeEvent = true;
		this.tablistScript.SelectElement(uiEvent.m_eventParams.tag, true);
		this.tablistScript.m_alwaysDispatchSelectedChangeEvent = false;
		this.week_huoyue_text = this.m_CUIForm.GetWidget(5).GetComponent<Text>();
		this.week_node1 = this.m_CUIForm.GetWidget(6);
		this.week_node2 = this.m_CUIForm.GetWidget(7);
		this.m_container = this.m_CUIForm.GetWidget(8).GetComponent<CUIContainerScript>();
		this.day_progress_bg = this.m_CUIForm.GetWidget(9).GetComponent<Image>();
		this.day_progress = this.m_CUIForm.GetWidget(10).GetComponent<Image>();
		this.day_huoyue_txt = this.m_CUIForm.GetWidget(11).GetComponent<Text>();
		this.Refresh_Tab_RedPoint();
		this._init_day_huoyue();
		this.Refresh_Huoyue();
		CTaskModel model2 = Singleton<CTaskSys>.instance.model;
		if (model2.curLevelRewardData == null)
		{
			uint pvpLevel = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().PvpLevel;
			model2.curLevelRewardData = model2.GetLevelRewardData((int)pvpLevel);
		}
	}

	public void On_Tab_Change(int index)
	{
		this.TabIndex = index;
	}

	public void Refresh()
	{
		if (this.m_CUIForm == null)
		{
			return;
		}
		this.Refresh_Tab_RedPoint();
		this.Refresh_List(this.m_tabIndex);
		this.Refresh_LevelTab();
	}

	public void Refresh_Huoyue()
	{
		if (this.m_CUIForm == null)
		{
			return;
		}
		this.Refresh_Week_Huoyue();
		this.refresh_Day_HuoYue();
	}

	public void refresh_Day_HuoYue()
	{
		CTaskModel model = Singleton<CTaskSys>.instance.model;
		this.day_huoyue_txt.set_text(model.huoyue_data.day_curNum.ToString());
		float x = (this.day_progress_bg.transform as RectTransform).sizeDelta.x;
		float y = (this.day_progress.transform as RectTransform).sizeDelta.y;
		uint max_dayhuoyue_num = model.huoyue_data.max_dayhuoyue_num;
		float num = model.huoyue_data.day_curNum / max_dayhuoyue_num * x;
		if (num > x)
		{
			num = x;
		}
		(this.day_progress.transform as RectTransform).sizeDelta = new Vector2(num, y);
		for (int i = 0; i < model.huoyue_data.day_huoyue_list.get_Count(); i++)
		{
			ushort key = model.huoyue_data.day_huoyue_list.get_Item(i);
			ResHuoYueDuReward resHuoYueDuReward = null;
			GameDataMgr.huoyueduDict.TryGetValue(key, out resHuoYueDuReward);
			if (resHuoYueDuReward != null)
			{
				GameObject gameObject = this.m_container.gameObject.transform.FindChild(string.Format("box_{0}", resHuoYueDuReward.wID)).gameObject;
				DebugHelper.Assert(gameObject != null);
				this._show_day_box(gameObject, resHuoYueDuReward.wID);
			}
		}
	}

	private void _init_day_huoyue()
	{
		if (this.bCreateDayNode)
		{
			return;
		}
		CTaskModel model = Singleton<CTaskSys>.instance.model;
		uint max_dayhuoyue_num = model.huoyue_data.max_dayhuoyue_num;
		float x = (this.day_progress_bg.transform as RectTransform).sizeDelta.x;
		int count = model.huoyue_data.day_huoyue_list.get_Count();
		for (int i = 0; i < count; i++)
		{
			ushort key = model.huoyue_data.day_huoyue_list.get_Item(i);
			ResHuoYueDuReward resHuoYueDuReward = null;
			GameDataMgr.huoyueduDict.TryGetValue(key, out resHuoYueDuReward);
			if (resHuoYueDuReward != null)
			{
				float num = resHuoYueDuReward.dwHuoYueDu / max_dayhuoyue_num * x;
				if (num > x)
				{
					num = x;
				}
				int element = this.m_container.GetElement();
				GameObject element2 = this.m_container.GetElement(element);
				if (!(element2 == null))
				{
					(element2.transform as RectTransform).anchoredPosition3D = new Vector3(num, 0f, 0f);
					element2.gameObject.name = string.Format("box_{0}", resHuoYueDuReward.wID);
					element2.transform.Find("icon").GetComponent<CUIEventScript>().m_onDownEventParams.tagUInt = (uint)resHuoYueDuReward.wID;
				}
			}
		}
		this.bCreateDayNode = true;
	}

	public void Refresh_Week_Huoyue()
	{
		CTaskModel model = Singleton<CTaskSys>.instance.model;
		this.Bind_Week_Node(this.week_node1, model.huoyue_data.week_reward1);
		this.Bind_Week_Node(this.week_node2, model.huoyue_data.week_reward2);
		this.week_huoyue_text.set_text(model.huoyue_data.week_curNum.ToString());
	}

	public void Bind_Week_Node(GameObject node, ushort week_id)
	{
		CTaskModel model = Singleton<CTaskSys>.instance.model;
		HuoyueData huoyue_data = model.huoyue_data;
		ResHuoYueDuReward rewardCfg = model.huoyue_data.GetRewardCfg(week_id);
		if (rewardCfg == null)
		{
			return;
		}
		Transform transform = node.transform.Find("node/box/icon");
		transform.GetComponent<CUIEventScript>().m_onDownEventParams.tagUInt = (uint)week_id;
		node.GetComponent<Text>().set_text(rewardCfg.dwHuoYueDu.ToString());
		Image component = transform.GetComponent<Image>();
		ResDT_HuoYueDuReward_PeriodInfo resDT_HuoYueDuReward_PeriodInfo = Singleton<CTaskSys>.instance.model.huoyue_data.IsInTime(rewardCfg);
		if (resDT_HuoYueDuReward_PeriodInfo != null)
		{
			component.SetSprite(CUIUtility.s_Sprite_Dynamic_Icon_Dir + resDT_HuoYueDuReward_PeriodInfo.szIcon, this.m_CUIForm, true, false, false, false);
		}
		else
		{
			component.SetSprite(CUIUtility.s_Sprite_Dynamic_Icon_Dir + rewardCfg.szIcon, this.m_CUIForm, true, false, false, false);
		}
		bool flag = huoyue_data.BAlready_Reward(RES_HUOYUEDU_TYPE.RES_HUOYUEDU_TYPE_WEEK, week_id);
		node.transform.FindChild("node/box/mark").gameObject.CustomSetActive(flag);
		bool flag2 = !flag && huoyue_data.week_curNum >= rewardCfg.dwHuoYueDu;
		node.transform.FindChild("node/box/effect").gameObject.CustomSetActive(flag2);
		node.transform.FindChild("node/box").GetComponent<Animation>().enabled = flag2;
	}

	private void _show_day_box(GameObject box, ushort id)
	{
		Text component = box.transform.FindChild("num").GetComponent<Text>();
		Image component2 = box.transform.FindChild("mark").GetComponent<Image>();
		Image component3 = box.transform.FindChild("icon").GetComponent<Image>();
		GameObject gameObject = box.transform.FindChild("BaoShi").gameObject;
		ResHuoYueDuReward resHuoYueDuReward = null;
		GameDataMgr.huoyueduDict.TryGetValue(id, out resHuoYueDuReward);
		if (resHuoYueDuReward == null)
		{
			return;
		}
		bool flag = Singleton<CTaskSys>.instance.model.huoyue_data.BAlready_Reward(RES_HUOYUEDU_TYPE.RES_HUOYUEDU_TYPE_DAY, id);
		component.set_text(resHuoYueDuReward.dwHuoYueDu.ToString());
		component2.gameObject.CustomSetActive(flag);
		if (component3 != null)
		{
			ResDT_HuoYueDuReward_PeriodInfo resDT_HuoYueDuReward_PeriodInfo = Singleton<CTaskSys>.instance.model.huoyue_data.IsInTime(resHuoYueDuReward);
			if (resDT_HuoYueDuReward_PeriodInfo != null)
			{
				component3.SetSprite(CUIUtility.s_Sprite_Dynamic_Icon_Dir + resDT_HuoYueDuReward_PeriodInfo.szIcon, this.m_CUIForm, true, false, false, false);
			}
			else
			{
				component3.SetSprite(CUIUtility.s_Sprite_Dynamic_Icon_Dir + resHuoYueDuReward.szIcon, this.m_CUIForm, true, false, false, false);
			}
		}
		bool flag2 = !flag && Singleton<CTaskSys>.instance.model.huoyue_data.day_curNum >= resHuoYueDuReward.dwHuoYueDu;
		box.transform.FindChild("effect").gameObject.CustomSetActive(flag2);
		box.GetComponent<Animation>().enabled = flag2;
		gameObject.CustomSetActive(flag2 || flag);
	}

	private CUIListScript getListScript(int type)
	{
		if (type == 0)
		{
			return null;
		}
		if (type == 1)
		{
			return this.tasklistScript_usual;
		}
		return null;
	}

	public void Refresh_List(int tIndex)
	{
		if (tIndex == 0)
		{
			return;
		}
		ListView<CTask> listView = Singleton<CTaskSys>.instance.model.task_Data.GetListView(tIndex);
		CUIListScript listScript = this.getListScript(tIndex);
		if (listScript == null || listView == null)
		{
			return;
		}
		listScript.transform.parent.gameObject.CustomSetActive(true);
		this._refresh_list(listScript, listView);
	}

	public void On_List_ElementEnable(CUIEvent uievent)
	{
		int srcWidgetIndexInBelongedList = uievent.m_srcWidgetIndexInBelongedList;
		CTask cTask = this._get_current_info((RES_TASK_TYPE)this.m_tabIndex, srcWidgetIndexInBelongedList);
		CTaskShower component = uievent.m_srcWidget.GetComponent<CTaskShower>();
		if (component != null && cTask != null)
		{
			component.ShowTask(cTask, this.m_CUIForm);
		}
	}

	private void _refresh_list(CUIListScript listScript, ListView<CTask> data_list)
	{
		if (listScript == null)
		{
			return;
		}
		int count = data_list.Count;
		listScript.SetElementAmount(count);
		for (int i = 0; i < count; i++)
		{
			CUIListElementScript elemenet = listScript.GetElemenet(i);
			if (elemenet != null && listScript.IsElementInScrollArea(i))
			{
				CTaskShower component = elemenet.GetComponent<CTaskShower>();
				CTask cTask = data_list[i];
				if (component != null && cTask != null)
				{
					component.ShowTask(cTask, this.m_CUIForm);
				}
			}
		}
	}

	private CTask _get_current_info(RES_TASK_TYPE taskType, int index)
	{
		ListView<CTask> listView = Singleton<CTaskSys>.instance.model.task_Data.GetListView((int)taskType);
		if (listView != null && index >= 0 && index < listView.Count)
		{
			return listView[index];
		}
		return null;
	}

	public void Refresh_Tab_RedPoint()
	{
		this._calc_red_dot(enTaskTab.TAB_MAIN);
		this._calc_red_dot(enTaskTab.TAB_USUAL);
	}

	private void _calc_red_dot(enTaskTab type)
	{
		int index = (type == enTaskTab.TAB_MAIN) ? 0 : 1;
		int task_Count = Singleton<CTaskSys>.instance.model.task_Data.GetTask_Count(type, CTask.State.Have_Done);
		if (task_Count > 0)
		{
			this.AddRedDot(index, enRedDotPos.enTopRight);
		}
		else
		{
			this.DelRedDot(index);
		}
		if (type == enTaskTab.TAB_MAIN)
		{
			if (Singleton<CTaskSys>.instance.model.IsShowMainTaskTab_RedDotCount())
			{
				this.jumpRewardGameObject.CustomSetActive(true);
				this.AddRedDot(0, enRedDotPos.enTopRight);
			}
			else
			{
				this.jumpRewardGameObject.CustomSetActive(false);
				this.DelRedDot(0);
			}
		}
	}

	public void AddRedDot(int index, enRedDotPos redDotPos)
	{
		CUIListElementScript elemenet = this.tablistScript.GetElemenet(index);
		if (elemenet == null)
		{
			return;
		}
		CUICommonSystem.AddRedDot(elemenet.gameObject, redDotPos, 0, 0, 0);
	}

	public void DelRedDot(int index)
	{
		CUIListElementScript elemenet = this.tablistScript.GetElemenet(index);
		if (elemenet == null)
		{
			return;
		}
		CUICommonSystem.DelRedDot(elemenet.gameObject);
	}

	private void Refresh_LevelTab()
	{
		if (this.m_tabIndex != 0)
		{
			return;
		}
		this._refresh_Level_list(this.m_levelRewardList, Singleton<CTaskSys>.instance.model.m_levelRewardDataMap.Count);
		this.ShowLevelRightSide(Singleton<CTaskSys>.instance.model.curLevelRewardData);
	}

	public void ShowSelectedGameobject(GameObject node, int level, bool bShowSelected = true)
	{
		if (node == null)
		{
			return;
		}
		GameObject gameObject = node.transform.Find("selected").gameObject;
		if (gameObject != null)
		{
			gameObject.CustomSetActive(bShowSelected);
		}
	}

	public void ShowLevelRightSide(LevelRewardData data)
	{
		if (this.m_unlockNode != null)
		{
			this.m_unlockNode.CustomSetActive(data != null);
		}
		if (this.m_levelRewardNode != null)
		{
			this.m_levelRewardNode.CustomSetActive(data != null);
		}
		if (this.m_emptyTaskNode != null)
		{
			this.m_emptyTaskNode.transform.parent.gameObject.CustomSetActive(data != null);
		}
		this.ShowUnLock(data, this.m_unlockNode);
		this.ShowLevelReward(data, this.m_levelRewardNode);
		this.ShowLevelTask(data);
	}

	private void ShowLevelTask(LevelRewardData levelRewardData)
	{
		if (levelRewardData == null)
		{
			return;
		}
		bool flag = levelRewardData.IsConfigTaskAllEmpty();
		bool flag2 = levelRewardData.GetValidTaskCount() > 0;
		bool flag3 = levelRewardData.IsAllLevelTask();
		if (flag || (!flag2 && !levelRewardData.IsHasCltCalcCompeletedTask()) || flag3)
		{
			this.m_emptyTaskNode.CustomSetActive(true);
			this.m_normalTaskNode.CustomSetActive(false);
			Text component = this.m_CUIForm.transform.Find("node/list_node_main/task_node/noTask/Text").GetComponent<Text>();
			if (component != null)
			{
				string text = string.Empty;
				if (flag)
				{
					text = Singleton<CTextManager>.instance.GetText("Task_LevelRewardInfo_None");
				}
				if (!flag2 && !levelRewardData.IsHasCltCalcCompeletedTask())
				{
					text = Singleton<CTextManager>.instance.GetText("Task_LevelRewardInfo_NoGetTask");
				}
				if (flag3)
				{
					if (levelRewardData.m_level >= CTaskView.LevelValue)
					{
						text = Singleton<CTextManager>.instance.GetText("Task_LevelRewardInfo_Up21");
					}
					else
					{
						text = Singleton<CTextManager>.instance.GetText("Task_LevelRewardInfo_AllLevelTask");
					}
				}
				component.set_text(text);
			}
		}
		else
		{
			this.m_emptyTaskNode.CustomSetActive(false);
			this.m_normalTaskNode.CustomSetActive(true);
			Text component2 = this.m_CUIForm.transform.Find("node/list_node_main/task_node/normal/Text").GetComponent<Text>();
			if (component2 != null)
			{
				component2.set_text(Singleton<CTextManager>.instance.GetText("Task_LevelRewardInfo_Normal", new string[]
				{
					levelRewardData.m_level.ToString()
				}));
			}
			int num = 0;
			for (int i = 0; i < levelRewardData.task_ids.Length; i++)
			{
				ResTask resTask = levelRewardData.task_ids[i];
				if (resTask != null)
				{
					CTask task = Singleton<CTaskSys>.instance.model.GetTask(resTask.dwTaskID);
					bool flag4 = task != null && task.m_resTask.bTaskSubType == 0;
					bool flag5 = task != null;
					bool flag6 = false;
					if (!flag5 && resTask != null)
					{
						flag6 = Singleton<CTaskSys>.instance.model.IsInCltCalcCompletedTasks(resTask.dwTaskID);
						flag4 = (resTask.bTaskSubType == 0);
					}
					GameObject gameObject = null;
					if (num == 0)
					{
						gameObject = this.m_taskNode0;
					}
					else if (num == 1)
					{
						gameObject = this.m_taskNode1;
					}
					if (gameObject != null && (flag5 || flag6) && !flag4)
					{
						CTaskShower component3 = gameObject.GetComponent<CTaskShower>();
						if (component3 == null)
						{
							return;
						}
						if (flag5)
						{
							gameObject.CustomSetActive(true);
							component3.ShowTask(task, this.m_CUIForm);
						}
						else if (flag6)
						{
							gameObject.CustomSetActive(true);
							ResTask resTask2 = levelRewardData.task_ids[i];
							if (resTask2 != null)
							{
								component3.ShowTask(resTask2, this.m_CUIForm);
							}
						}
						num++;
					}
				}
			}
			for (int j = num; j < LevelRewardData.TASK_MAX_COUNT; j++)
			{
				GameObject gameObject2 = null;
				if (j == 0)
				{
					gameObject2 = this.m_taskNode0;
				}
				else if (j == 1)
				{
					gameObject2 = this.m_taskNode1;
				}
				if (gameObject2 != null)
				{
					gameObject2.CustomSetActive(false);
				}
			}
		}
	}

	private void ShowUnLock(LevelRewardData data, GameObject node)
	{
		if (data == null || node == null)
		{
			return;
		}
		if (this.m_unlockInfoTxt != null)
		{
			this.m_unlockInfoTxt.set_text(string.Format(Singleton<CTextManager>.instance.GetText("Task_Unlock_Text"), data.m_level));
		}
		int num = 0;
		for (int i = 0; i < LevelRewardData.UNLOCK_MAX_COUNT; i++)
		{
			ResDT_LevelReward_UnlockInfo resDT_LevelReward_UnlockInfo = data.astLockInfo[i];
			if (resDT_LevelReward_UnlockInfo != null && !string.IsNullOrEmpty(resDT_LevelReward_UnlockInfo.szUnlockID))
			{
				Transform transform = node.transform.Find(string.Format("Ent_{0}", num));
				DebugHelper.Assert(transform != null, "CTaskView ShowUnLock item is null, check out, name:Ent_" + num);
				if (transform != null)
				{
					transform.gameObject.CustomSetActive(true);
					Image component = transform.Find("icon").GetComponent<Image>();
					DebugHelper.Assert(component != null, "CTaskView ShowUnLock icon is null, check out...");
					component.SetSprite(resDT_LevelReward_UnlockInfo.szIcon, this.m_CUIForm, true, false, false, false);
					Text component2 = transform.Find("name").GetComponent<Text>();
					DebugHelper.Assert(component2 != null, "CTaskView ShowUnLock txt is null, check out...");
					if (component2 != null)
					{
						component2.set_text(resDT_LevelReward_UnlockInfo.szUnlockID);
					}
					Button component3 = transform.Find("goto_btn").GetComponent<Button>();
					this._SetUnlockButton(component3, (RES_GAME_ENTRANCE_TYPE)resDT_LevelReward_UnlockInfo.bGotoID, (ulong)Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().PvpLevel >= (ulong)((long)data.m_level));
					num++;
				}
			}
		}
		for (int j = num; j < LevelRewardData.UNLOCK_MAX_COUNT; j++)
		{
			Transform transform2 = node.transform.Find(string.Format("Ent_{0}", j));
			if (transform2 != null)
			{
				transform2.gameObject.CustomSetActive(false);
			}
		}
	}

	private void _SetUnlockButton(Button btn, RES_GAME_ENTRANCE_TYPE entryType, bool bEnable)
	{
		RES_SPECIALFUNCUNLOCK_TYPE type = CUICommonSystem.EntryTypeToUnlockType(entryType);
		if (Singleton<CFunctionUnlockSys>.instance.FucIsUnlock(type) && bEnable)
		{
			CUICommonSystem.SetButtonEnableWithShader(btn, true, true);
			btn.GetComponent<CUIEventScript>().m_onClickEventParams.tag = (int)entryType;
		}
		else
		{
			CUICommonSystem.SetButtonEnableWithShader(btn, false, true);
		}
	}

	private void ShowLevelReward(LevelRewardData data, GameObject node)
	{
		if (data == null || node == null)
		{
			return;
		}
		GameObject gameObject = node.transform.Find("goto_btn").gameObject;
		GameObject gameObject2 = node.transform.Find("HasGetReward").gameObject;
		DebugHelper.Assert(gameObject != null, "CTaskView ShowLevelReward goto_btn is null, check out...");
		DebugHelper.Assert(gameObject2 != null, "CTaskView ShowLevelReward has_get is null, check out...");
		if (this.m_levelRewardText != null)
		{
			this.m_levelRewardText.set_text(string.Format(Singleton<CTextManager>.instance.GetText("Task_Award_Text"), data.m_level));
		}
		if (data.GetConfigRewardCount() == 0)
		{
			gameObject.CustomSetActive(false);
			gameObject2.CustomSetActive(false);
		}
		int num = 0;
		for (int i = 0; i < LevelRewardData.REWARD_MAX_COUNT; i++)
		{
			ResDT_LevelReward_Info resDT_LevelReward_Info = data.m_resLevelReward.astRewardInfo[i];
			if (resDT_LevelReward_Info.dwRewardNum != 0u)
			{
				Transform transform = node.transform.Find(string.Format("Ent_{0}", num));
				DebugHelper.Assert(transform != null, "CTaskView ShowLevelReward item is null, check out, name:Ent_" + num);
				if (transform != null)
				{
					transform.gameObject.CustomSetActive(true);
					Image component = transform.Find("icon").GetComponent<Image>();
					DebugHelper.Assert(component != null, "CTaskView ShowLevelReward icon is null, check out...");
					if (!string.IsNullOrEmpty(resDT_LevelReward_Info.szIcon))
					{
						component.SetSprite(resDT_LevelReward_Info.szIcon, this.m_CUIForm, true, false, false, false);
					}
					Text component2 = transform.Find("count").GetComponent<Text>();
					DebugHelper.Assert(component2 != null, "CTaskView ShowLevelReward txt1 is null, check out...");
					if (component2 != null)
					{
						component2.set_text("x " + resDT_LevelReward_Info.dwRewardNum);
					}
					Text component3 = transform.Find("name").GetComponent<Text>();
					DebugHelper.Assert(component3 != null, "CTaskView ShowLevelReward txt2 is null, check out...");
					if (component3 != null)
					{
						component3.set_text(resDT_LevelReward_Info.szDesc);
					}
					num++;
				}
			}
		}
		for (int j = num; j < LevelRewardData.UNLOCK_MAX_COUNT; j++)
		{
			Transform transform2 = node.transform.Find(string.Format("Ent_{0}", j));
			if (transform2 != null)
			{
				transform2.gameObject.CustomSetActive(false);
			}
		}
		if (Singleton<CTaskSys>.instance.model.IsGetLevelReward(data.m_level))
		{
			if (gameObject2 != null)
			{
				gameObject2.CustomSetActive(true);
			}
			if (gameObject != null)
			{
				gameObject.CustomSetActive(false);
			}
		}
		else
		{
			if (gameObject2 != null)
			{
				gameObject2.CustomSetActive(false);
			}
			if ((ulong)Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().PvpLevel >= (ulong)((long)data.m_level))
			{
				if (gameObject != null)
				{
					gameObject.CustomSetActive(true);
				}
			}
			else if (gameObject != null)
			{
				gameObject.CustomSetActive(false);
			}
		}
	}

	public void RefreshLevelList()
	{
		if (this.m_levelRewardList != null && Singleton<CTaskSys>.instance.model.m_levelRewardDataMap != null)
		{
			this._refresh_Level_list(this.m_levelRewardList, Singleton<CTaskSys>.instance.model.m_levelRewardDataMap.Count);
		}
	}

	private void _refresh_Level_list(CUIListScript listScript, int count)
	{
		if (listScript == null)
		{
			return;
		}
		ListView<LevelRewardData> levelRewardDataMap = Singleton<CTaskSys>.instance.model.m_levelRewardDataMap;
		listScript.SetElementAmount(count);
		for (int i = 0; i < count; i++)
		{
			CUIListElementScript elemenet = listScript.GetElemenet(i);
			if (elemenet != null && listScript.IsElementInScrollArea(i))
			{
				this._ShowLevelNode(elemenet.gameObject, levelRewardDataMap[i]);
			}
		}
	}

	public void MoveElementInScrollArea(int index)
	{
		if (this.m_levelRewardList != null)
		{
			this.m_levelRewardList.MoveElementInScrollArea(index, true);
		}
	}

	public void On_LevelRewardList_ElementEnable(CUIEvent uievent)
	{
		ListView<LevelRewardData> levelRewardDataMap = Singleton<CTaskSys>.instance.model.m_levelRewardDataMap;
		int srcWidgetIndexInBelongedList = uievent.m_srcWidgetIndexInBelongedList;
		if (srcWidgetIndexInBelongedList < levelRewardDataMap.Count)
		{
			LevelRewardData levelRewardData = levelRewardDataMap[srcWidgetIndexInBelongedList];
			if (levelRewardData != null)
			{
				this._ShowLevelNode(uievent.m_srcWidget.gameObject, levelRewardData);
			}
		}
	}

	public void OnTask_JumpToReward(CUIEvent uievent)
	{
	}

	private void _ShowLevelNode(GameObject node, LevelRewardData data)
	{
		if (node == null || data == null)
		{
			return;
		}
		int levelRewardData_Index = Singleton<CTaskSys>.instance.model.GetLevelRewardData_Index(data);
		bool flag = levelRewardData_Index == Singleton<CTaskSys>.instance.model.m_levelRewardDataMap.Count - 1;
		GameObject gameObject = null;
		GameObject gameObject2 = null;
		this.GetShowAndHide_LevelNode(levelRewardData_Index, node, out gameObject, out gameObject2);
		gameObject.CustomSetActive(true);
		gameObject2.CustomSetActive(false);
		Text component = gameObject.transform.Find("Text").GetComponent<Text>();
		if (component)
		{
			component.set_text(string.Format("LV.{0}", data.m_level));
		}
		GameObject gameObject3 = gameObject.transform.Find("locked").gameObject;
		GameObject gameObject4 = gameObject.transform.Find("normal").gameObject;
		if ((ulong)Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().PvpLevel >= (ulong)((long)data.m_level))
		{
			if (gameObject3 != null)
			{
				gameObject3.CustomSetActive(false);
			}
			if (gameObject4 != null)
			{
				gameObject4.CustomSetActive(true);
			}
		}
		else
		{
			if (gameObject3 != null)
			{
				gameObject3.CustomSetActive(true);
			}
			if (gameObject4 != null)
			{
				gameObject4.CustomSetActive(false);
			}
		}
		GameObject gameObject5 = gameObject.transform.Find("curLevel").gameObject;
		CRoleInfo masterRoleInfo = Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo();
		bool bActive = masterRoleInfo != null && (ulong)masterRoleInfo.PvpLevel == (ulong)((long)data.m_level);
		if (gameObject5 != null)
		{
			gameObject5.CustomSetActive(bActive);
		}
		GameObject gameObject6 = gameObject.transform.Find("selected").gameObject;
		if (Singleton<CTaskSys>.instance.model.curLevelRewardData != null)
		{
			bool bActive2 = Singleton<CTaskSys>.instance.model.curLevelRewardData.IsEqual(data);
			gameObject6.CustomSetActive(bActive2);
		}
		else
		{
			gameObject6.CustomSetActive(false);
		}
		GameObject gameObject7 = gameObject.transform.Find("line").gameObject;
		if (flag)
		{
			if (gameObject7 != null)
			{
				gameObject7.CustomSetActive(false);
			}
		}
		else if (gameObject7 != null)
		{
			gameObject7.CustomSetActive(true);
		}
		if (Singleton<CTaskSys>.instance.model.IsLevelNode_RedDot(data))
		{
			CUICommonSystem.AddRedDot(gameObject, enRedDotPos.enTopRight, 0, 0, 0);
		}
		else
		{
			CUICommonSystem.DelRedDot(gameObject);
		}
		CUICommonSystem.DelRedDot(gameObject2);
	}

	private void GetShowAndHide_LevelNode(int index, GameObject node, out GameObject showNode, out GameObject hideNode)
	{
		if (index % 2 == 0)
		{
			showNode = node.transform.Find("left_btn").gameObject;
			hideNode = node.transform.Find("right_btn").gameObject;
		}
		else
		{
			showNode = node.transform.Find("right_btn").gameObject;
			hideNode = node.transform.Find("left_btn").gameObject;
		}
	}
}
