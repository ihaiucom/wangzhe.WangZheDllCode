using ResData;
using System;

namespace Assets.Scripts.GameSystem
{
	public class LevelRewardData
	{
		public const int TASK_REWARD_MAX_COUNT = 2;

		public static int TASK_MAX_COUNT = 2;

		public static int UNLOCK_MAX_COUNT = 2;

		public static int REWARD_MAX_COUNT = 2;

		public int m_level;

		public ResPvpLevelReward m_resLevelReward;

		public bool m_bHasGetReward;

		public ResDT_LevelReward_UnlockInfo[] astLockInfo = new ResDT_LevelReward_UnlockInfo[LevelRewardData.UNLOCK_MAX_COUNT];

		public ResTask[] task_ids = new ResTask[LevelRewardData.TASK_MAX_COUNT];

		public bool IsEqual(LevelRewardData data)
		{
			if (data == null)
			{
				return false;
			}
			if (this.m_resLevelReward != null && data.m_resLevelReward != null)
			{
				return this.m_resLevelReward.iLevel == data.m_resLevelReward.iLevel && this.m_level == data.m_level;
			}
			return this.m_resLevelReward == null && data.m_resLevelReward == null && this.m_level == data.m_level;
		}

		public void Clear()
		{
			this.m_resLevelReward = null;
			for (int i = 0; i < this.astLockInfo.Length; i++)
			{
				this.astLockInfo[i] = null;
			}
			for (int j = 0; j < this.task_ids.Length; j++)
			{
				this.task_ids[j] = null;
			}
		}

		public int GetConfigRewardCount()
		{
			if (this.m_resLevelReward == null)
			{
				return 0;
			}
			int num = 0;
			for (int i = 0; i < this.m_resLevelReward.astRewardInfo.Length; i++)
			{
				ResDT_LevelReward_Info resDT_LevelReward_Info = this.m_resLevelReward.astRewardInfo[i];
				if (resDT_LevelReward_Info.dwRewardNum != 0u)
				{
					num++;
				}
			}
			return num;
		}

		public int GetHaveDoneTaskCount()
		{
			int num = 0;
			for (int i = 0; i < this.task_ids.Length; i++)
			{
				ResTask resTask = this.task_ids[i];
				if (resTask != null)
				{
					CTask task = Singleton<CTaskSys>.instance.model.GetTask(resTask.dwTaskID);
					if (task != null && task.m_taskSubType != 0 && task.m_taskState == 1)
					{
						num++;
					}
				}
			}
			return num;
		}

		public bool IsAllLevelTask()
		{
			for (int i = 0; i < this.task_ids.Length; i++)
			{
				ResTask resTask = this.task_ids[i];
				if (resTask != null && resTask != null && resTask.bTaskSubType != 0)
				{
					return false;
				}
			}
			return true;
		}

		public bool IsHasCltCalcCompeletedTask()
		{
			for (int i = 0; i < this.task_ids.Length; i++)
			{
				ResTask resTask = this.task_ids[i];
				if (resTask != null && Singleton<CTaskSys>.instance.model.IsInCltCalcCompletedTasks(resTask.dwTaskID))
				{
					return true;
				}
			}
			return false;
		}

		public int GetValidTaskCount()
		{
			int num = 0;
			for (int i = 0; i < this.task_ids.Length; i++)
			{
				ResTask resTask = this.task_ids[i];
				if (resTask != null)
				{
					CTask task = Singleton<CTaskSys>.instance.model.GetTask(resTask.dwTaskID);
					if (task != null)
					{
						num++;
					}
				}
			}
			return num;
		}

		public bool IsConfigTaskAllEmpty()
		{
			for (int i = 0; i < this.task_ids.Length; i++)
			{
				if (this.task_ids[i] != null)
				{
					return false;
				}
			}
			return true;
		}

		public int GetResTaskIDIndex(uint id)
		{
			for (int i = 0; i < this.task_ids.Length; i++)
			{
				ResTask resTask = this.task_ids[i];
				if (resTask != null && resTask.dwTaskID == id)
				{
					return i;
				}
			}
			return -1;
		}

		public int GetFristNullResTaskIndex()
		{
			for (int i = 0; i < this.task_ids.Length; i++)
			{
				if (this.task_ids[i] == null)
				{
					return i;
				}
			}
			return -1;
		}
	}
}
