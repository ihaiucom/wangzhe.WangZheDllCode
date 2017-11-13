using Assets.Scripts.Framework;
using CSProtocol;
using ResData;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameSystem
{
	public class CTaskModel
	{
		public uint share_task_id;

		public CombineData task_Data = new CombineData();

		public HuoyueData huoyue_data = new HuoyueData();

		public ListView<LevelRewardData> m_levelRewardDataMap = new ListView<LevelRewardData>();

		private ulong m_levelRewardFlag;

		private LevelRewardData m_curLevelRewardData;

		public string Daily_Quest_Career = string.Empty;

		public string Daily_Quest_NeedGrowing = string.Empty;

		public string Daily_Quest_NeedMoney = string.Empty;

		public string Daily_Quest_NeedSeal = string.Empty;

		public string Daily_Quest_NeedHero = string.Empty;

		public Dictionary<int, List<uint>> m_cltCalcCompletedTasks = new Dictionary<int, List<uint>>();

		private uint[] maxTaskIds;

		public LevelRewardData curLevelRewardData
		{
			get
			{
				return this.m_curLevelRewardData;
			}
			set
			{
				this.m_curLevelRewardData = value;
				CTaskView taskView = Singleton<CTaskSys>.instance.m_taskView;
				if (taskView != null)
				{
					taskView.RefreshLevelList();
				}
				if (taskView != null)
				{
					taskView.ShowLevelRightSide(this.curLevelRewardData);
				}
				if (taskView != null && this.m_curLevelRewardData != null)
				{
					taskView.MoveElementInScrollArea(Singleton<CTaskSys>.instance.model.GetLevelIndex(this.m_curLevelRewardData.m_level));
				}
			}
		}

		public uint maxCfgTudiTaskID
		{
			get;
			private set;
		}

		public uint maxCfgMasterTaskID
		{
			get;
			private set;
		}

		public uint maxServerTudiTaskID
		{
			get
			{
				return this.maxTaskIds[4];
			}
		}

		public uint maxServerMasterTaskID
		{
			get
			{
				return this.maxTaskIds[3];
			}
		}

		public int GetLevelIndex(int level)
		{
			for (int i = 0; i < this.m_levelRewardDataMap.Count; i++)
			{
				LevelRewardData levelRewardData = this.m_levelRewardDataMap[i];
				if (levelRewardData.m_level == level)
				{
					return i;
				}
			}
			return -1;
		}

		public bool IsInCltCalcCompletedTasks(uint taskid, int task_type)
		{
			if (taskid == 0u)
			{
				return false;
			}
			List<uint> list = null;
			this.m_cltCalcCompletedTasks.TryGetValue(task_type, ref list);
			return list != null && list.Contains(taskid);
		}

		public bool IsInCltCalcCompletedTasks(uint taskid)
		{
			Dictionary<int, List<uint>>.Enumerator enumerator = this.m_cltCalcCompletedTasks.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<int, List<uint>> current = enumerator.get_Current();
				List<uint> value = current.get_Value();
				if (value != null && value.Contains(taskid))
				{
					return true;
				}
			}
			return false;
		}

		public void ParseCltCalcCompletedTasks(ref uint[] taskids)
		{
			this.maxCfgTudiTaskID = 0u;
			this.maxCfgMasterTaskID = 0u;
			this.maxTaskIds = new uint[taskids.Length];
			for (int i = 0; i < taskids.Length; i++)
			{
				this.maxTaskIds[i] = taskids[i];
			}
			Dictionary<int, List<uint>>.Enumerator enumerator = this.m_cltCalcCompletedTasks.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<int, List<uint>> current = enumerator.get_Current();
				List<uint> value = current.get_Value();
				if (value != null)
				{
					value.Clear();
				}
			}
			this.m_cltCalcCompletedTasks.Clear();
			Dictionary<byte, uint> dictionary = new Dictionary<byte, uint>();
			for (int j = 0; j < taskids.Length; j++)
			{
				uint num = taskids[j];
				if (num != 0u)
				{
					ResTask dataByKey = GameDataMgr.taskDatabin.GetDataByKey(num);
					DebugHelper.Assert(dataByKey != null, "ParseCltCalcCompletedTasks, taskDatabin.GetDataByKey({0}) is null", new object[]
					{
						num
					});
					if (dataByKey != null)
					{
						if (dictionary.ContainsKey(dataByKey.bTaskSubType))
						{
							if (num < dictionary.get_Item(dataByKey.bTaskSubType))
							{
								dictionary.set_Item(dataByKey.bTaskSubType, num);
							}
						}
						else
						{
							dictionary.Add(dataByKey.bTaskSubType, num);
						}
					}
				}
			}
			Dictionary<long, object>.Enumerator enumerator2 = GameDataMgr.taskDatabin.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				KeyValuePair<long, object> current2 = enumerator2.get_Current();
				ResTask resTask = (ResTask)current2.get_Value();
				if (resTask != null && resTask.dwTaskType == 0u)
				{
					if (dictionary.ContainsKey(resTask.bTaskSubType) && resTask.dwTaskID < dictionary.get_Item(resTask.bTaskSubType))
					{
						this.InsertCltCalcCompletedTasks(resTask.dwTaskID, (int)resTask.bTaskSubType);
					}
					if (resTask.bTaskSubType == 3 && resTask.dwTaskID > this.maxCfgMasterTaskID)
					{
						this.maxCfgMasterTaskID = resTask.dwTaskID;
					}
					if (resTask.bTaskSubType == 4 && resTask.dwTaskID > this.maxCfgTudiTaskID)
					{
						this.maxCfgTudiTaskID = resTask.dwTaskID;
					}
				}
			}
			Dictionary<byte, uint>.Enumerator enumerator3 = dictionary.GetEnumerator();
			while (enumerator3.MoveNext())
			{
				KeyValuePair<byte, uint> current3 = enumerator3.get_Current();
				if (this.GetTask(current3.get_Value()) == null)
				{
					DatabinTable<ResTask, uint> taskDatabin = GameDataMgr.taskDatabin;
					KeyValuePair<byte, uint> current4 = enumerator3.get_Current();
					ResTask dataByKey2 = taskDatabin.GetDataByKey(current4.get_Value());
					this.InsertCltCalcCompletedTasks(dataByKey2.dwTaskID, (int)dataByKey2.bTaskSubType);
				}
			}
		}

		private void InsertCltCalcCompletedTasks(uint taskid, int task_sub_type)
		{
			List<uint> list = null;
			if (this.m_cltCalcCompletedTasks.ContainsKey(task_sub_type))
			{
				this.m_cltCalcCompletedTasks.TryGetValue(task_sub_type, ref list);
				if (list == null)
				{
					list = new List<uint>();
					this.m_cltCalcCompletedTasks.set_Item(task_sub_type, list);
				}
			}
			else
			{
				list = new List<uint>();
				this.m_cltCalcCompletedTasks.Add(task_sub_type, list);
			}
			if (!list.Contains(taskid))
			{
				list.Add(taskid);
			}
		}

		public void Clear()
		{
			this.task_Data.Clear();
			this.huoyue_data.Clear();
		}

		public int GetMainTask_RedDotCount()
		{
			int num = 0;
			for (int i = 0; i < this.m_levelRewardDataMap.Count; i++)
			{
				LevelRewardData levelRewardData = this.m_levelRewardDataMap[i];
				if (levelRewardData != null)
				{
					bool flag = false;
					if (!Singleton<CTaskSys>.instance.model.IsGetLevelReward(levelRewardData.m_level) && (ulong)Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().PvpLevel >= (ulong)((long)levelRewardData.m_level))
					{
						flag = true;
					}
					if (flag || levelRewardData.GetHaveDoneTaskCount() > 0)
					{
						num++;
					}
				}
			}
			return num;
		}

		public bool IsShowMainTaskTab_RedDotCount()
		{
			for (int i = 0; i < this.m_levelRewardDataMap.Count; i++)
			{
				LevelRewardData levelRewardData = this.m_levelRewardDataMap[i];
				if (levelRewardData != null && this.IsLevelNode_RedDot(levelRewardData))
				{
					return true;
				}
			}
			return false;
		}

		public bool IsLevelNode_RedDot(LevelRewardData levelRewardData)
		{
			if (levelRewardData != null)
			{
				bool flag = false;
				if (!Singleton<CTaskSys>.instance.model.IsGetLevelReward(levelRewardData.m_level) && (ulong)Singleton<CRoleInfoManager>.instance.GetMasterRoleInfo().PvpLevel >= (ulong)((long)levelRewardData.m_level))
				{
					flag = true;
				}
				if (flag || levelRewardData.GetHaveDoneTaskCount() > 0)
				{
					return true;
				}
			}
			return false;
		}

		public bool CalcNextRewardNode(int startIndex, out int nextlistIndex, out LevelRewardData nextData)
		{
			nextlistIndex = 0;
			nextData = null;
			ListView<LevelRewardData> levelRewardDataMap = Singleton<CTaskSys>.instance.model.m_levelRewardDataMap;
			for (int i = startIndex + 1; i < levelRewardDataMap.Count; i++)
			{
				LevelRewardData levelRewardData = levelRewardDataMap[i];
				if (levelRewardData != null && this.IsLevelNode_RedDot(levelRewardData))
				{
					nextlistIndex = i;
					nextData = levelRewardData;
					return true;
				}
			}
			for (int j = 0; j < startIndex; j++)
			{
				LevelRewardData levelRewardData2 = levelRewardDataMap[j];
				if (levelRewardData2 != null && this.IsLevelNode_RedDot(levelRewardData2))
				{
					nextlistIndex = j;
					nextData = levelRewardData2;
					return true;
				}
			}
			return false;
		}

		public int GetLevelRewardData_Index(LevelRewardData data)
		{
			if (data == null)
			{
				return -1;
			}
			for (int i = 0; i < this.m_levelRewardDataMap.Count; i++)
			{
				LevelRewardData levelRewardData = this.m_levelRewardDataMap[i];
				if (levelRewardData == data)
				{
					return i;
				}
			}
			return -1;
		}

		public LevelRewardData GetLevelRewardData_ByIndex(int index)
		{
			DebugHelper.Assert(index < this.m_levelRewardDataMap.Count, "CTaskModel GetLevelRewardData_ByIndex, index > count, check out...");
			if (index < this.m_levelRewardDataMap.Count)
			{
				return this.m_levelRewardDataMap[index];
			}
			return null;
		}

		public LevelRewardData GetLevelRewardData(int level)
		{
			for (int i = 0; i < this.m_levelRewardDataMap.Count; i++)
			{
				LevelRewardData levelRewardData = this.m_levelRewardDataMap[i];
				if (levelRewardData != null && levelRewardData.m_level == level)
				{
					return levelRewardData;
				}
			}
			return null;
		}

		public void SyncServerLevelRewardFlagData(ulong num)
		{
			this.m_levelRewardFlag = num;
			for (int i = 0; i < this.m_levelRewardDataMap.Count; i++)
			{
				LevelRewardData levelRewardData = this.m_levelRewardDataMap[i];
				if (levelRewardData != null)
				{
					levelRewardData.m_bHasGetReward = this.IsGetLevelReward(this.m_levelRewardFlag, levelRewardData.m_level);
					if (levelRewardData.m_bHasGetReward)
					{
					}
				}
			}
		}

		public bool IsGetLevelReward(int level)
		{
			return this.IsGetLevelReward(this.m_levelRewardFlag, level);
		}

		private bool IsGetLevelReward(ulong flagdata, int level)
		{
			ulong num = 1uL << level - 1;
			return (flagdata & num) != 0uL;
		}

		public void ParseLevelRewardData()
		{
			Dictionary<long, object>.Enumerator enumerator = GameDataMgr.resPvpLevelRewardDatabin.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<long, object> current = enumerator.get_Current();
				ResPvpLevelReward resPvpLevelReward = (ResPvpLevelReward)current.get_Value();
				if (resPvpLevelReward != null && this.GetLevelRewardData(resPvpLevelReward.iLevel) == null)
				{
					LevelRewardData levelRewardData = new LevelRewardData();
					levelRewardData.m_level = resPvpLevelReward.iLevel;
					levelRewardData.m_resLevelReward = resPvpLevelReward;
					DebugHelper.Assert(resPvpLevelReward.astLockInfo.Length <= 2, "ParseLevelRewardData 等级奖励配置表配 解锁数量不该超过2个, 翔哥 check out...");
					for (int i = 0; i < resPvpLevelReward.astLockInfo.Length; i++)
					{
						ResDT_LevelReward_UnlockInfo resDT_LevelReward_UnlockInfo = resPvpLevelReward.astLockInfo[i];
						if (resDT_LevelReward_UnlockInfo != null)
						{
							levelRewardData.astLockInfo[i] = resDT_LevelReward_UnlockInfo;
						}
					}
					this.m_levelRewardDataMap.Add(levelRewardData);
				}
			}
			Dictionary<long, object>.Enumerator enumerator2 = GameDataMgr.taskDatabin.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				KeyValuePair<long, object> current2 = enumerator2.get_Current();
				ResTask resTask = (ResTask)current2.get_Value();
				if (resTask != null && resTask.dwOpenType == 2u)
				{
					int iParam = resTask.astOpenTaskParam[0].iParam;
					LevelRewardData levelRewardData2 = this.GetLevelRewardData(iParam);
					if (levelRewardData2 != null && levelRewardData2.GetResTaskIDIndex(resTask.dwTaskID) == -1)
					{
						int fristNullResTaskIndex = levelRewardData2.GetFristNullResTaskIndex();
						if (fristNullResTaskIndex != -1)
						{
							levelRewardData2.task_ids[fristNullResTaskIndex] = resTask;
						}
					}
				}
			}
		}

		public void Load_Task_Tab_String()
		{
			this.Daily_Quest_Career = UT.GetText("Daily_Quest_Career");
			this.Daily_Quest_NeedGrowing = UT.GetText("Daily_Quest_NeedGrowing");
			this.Daily_Quest_NeedMoney = UT.GetText("Daily_Quest_NeedMoney");
			this.Daily_Quest_NeedSeal = UT.GetText("Daily_Quest_NeedSeal");
			this.Daily_Quest_NeedHero = UT.GetText("Daily_Quest_NeedHero");
		}

		public CTask GetTask(uint TaskId)
		{
			return this.task_Data.GetTask(TaskId);
		}

		public bool AnyTaskOfState(COM_TASK_STATE state, RES_TASK_TYPE taskType, out CTask outTask)
		{
			outTask = null;
			ListView<CTask> listView = this.task_Data.GetListView((int)taskType);
			if (listView == null)
			{
				return false;
			}
			for (int i = 0; i < listView.Count; i++)
			{
				CTask cTask = listView[i];
				if (cTask != null && cTask.m_taskState == (byte)state)
				{
					outTask = cTask;
					return true;
				}
			}
			return false;
		}

		public ListView<CTask> GetTasks(enTaskTab type)
		{
			return this.task_Data.GetListView((int)type);
		}

		public int GetTotalTaskOfState(enTaskTab type, COM_TASK_STATE state)
		{
			ListView<CTask> listView = this.task_Data.GetListView((int)type);
			if (listView == null)
			{
				return 0;
			}
			int num = 0;
			for (int i = 0; i < listView.Count; i++)
			{
				if ((COM_TASK_STATE)listView[i].m_taskState == state)
				{
					num++;
				}
			}
			return num;
		}

		public void UpdateTaskState()
		{
			if (this.task_Data != null)
			{
				DictionaryView<uint, CTask>.Enumerator enumerator = this.task_Data.task_map.GetEnumerator();
				while (enumerator.MoveNext())
				{
					KeyValuePair<uint, CTask> current = enumerator.Current;
					CTask value = current.get_Value();
					if (value != null)
					{
						value.UpdateState();
					}
				}
			}
		}

		public bool IsAnyTaskInState(enTaskTab type, CTask.State state)
		{
			ListView<CTask> listView = this.task_Data.GetListView((int)type);
			DebugHelper.Assert(listView != null);
			if (listView == null)
			{
				return false;
			}
			for (int i = 0; i < listView.Count; i++)
			{
				CTask cTask = listView[i];
				if (cTask != null && cTask.m_taskState == (byte)state)
				{
					return true;
				}
			}
			return false;
		}

		public void AddTask(CTask task)
		{
			if (task != null)
			{
				this.task_Data.Add(task.m_baseId, task);
			}
		}

		public void Remove(uint id)
		{
			this.task_Data.Remove(id);
		}

		public int GetTasks_Count(enTaskTab type, CTask.State state)
		{
			ListView<CTask> listView = this.task_Data.GetListView((int)type);
			DebugHelper.Assert(listView != null);
			if (listView == null)
			{
				return 0;
			}
			int num = 0;
			for (int i = 0; i < listView.Count; i++)
			{
				CTask cTask = listView[i];
				if (cTask != null && cTask.m_taskState == (byte)state)
				{
					num++;
				}
			}
			return num;
		}

		public CTask GetMaxIndex_TaskID_InState(enTaskTab type, CTask.State state)
		{
			return this.task_Data.GetMaxIndex_TaskID_InState(type, state);
		}

		public void Load_Share_task()
		{
			Dictionary<long, object>.Enumerator enumerator = GameDataMgr.taskDatabin.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<long, object> current = enumerator.get_Current();
				ResTask resTask = (ResTask)current.get_Value();
				bool flag = resTask.astPrerequisiteArray[0].dwPrerequisiteType == 19u;
				if (flag)
				{
					this.share_task_id = resTask.dwTaskID;
					break;
				}
			}
		}
	}
}
