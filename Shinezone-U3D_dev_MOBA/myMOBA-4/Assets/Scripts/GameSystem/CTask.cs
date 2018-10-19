using Assets.Scripts.Framework;
using CSProtocol;
using ResData;
using System;

namespace Assets.Scripts.GameSystem
{
	public class CTask
	{
		public enum State
		{
			OnGoing,
			Have_Done,
			Commited = 3,
			NewRefresh,
			None
		}

		public struct PrerequisiteInfo
		{
			public bool m_isReach;

			public int m_value;

			public int m_valueTarget;

			public uint m_conditionType;
		}

		public uint m_baseId;

		public uint m_taskType;

		public byte m_taskSubType;

		public byte m_taskState;

		public uint m_preTaskId;

		public CTask.PrerequisiteInfo[] m_prerequisiteInfo = new CTask.PrerequisiteInfo[3];

		public string m_taskDesc;

		public string m_taskTitle;

		public uint m_rewardId;

		public string m_taskIcon;

		public ResTask m_resTask;

		public ResTaskReward resAward;

		public CTask(uint id, ResTask task)
		{
			DebugHelper.Assert(id > 0u && task != null);
			this.m_resTask = task;
			if (this.m_resTask == null)
			{
				return;
			}
			this.m_baseId = id;
			this.m_taskState = 0;
			this.m_taskType = this.m_resTask.dwTaskType;
			this.m_taskSubType = this.m_resTask.bTaskSubType;
			this.m_preTaskId = this.m_resTask.dwPreTaskID;
			for (int i = 0; i < this.m_resTask.astPrerequisiteArray.Length; i++)
			{
				if (this.m_resTask.astPrerequisiteArray[i].astPrerequisiteParam[0].iParam > 0)
				{
					this.m_prerequisiteInfo[i].m_value = 0;
					this.m_prerequisiteInfo[i].m_valueTarget = this.m_resTask.astPrerequisiteArray[i].astPrerequisiteParam[0].iParam;
					this.m_prerequisiteInfo[i].m_isReach = false;
					this.m_prerequisiteInfo[i].m_conditionType = this.m_resTask.astPrerequisiteArray[i].dwPrerequisiteType;
				}
			}
			this.m_taskTitle = Utility.UTF8Convert(this.m_resTask.szTaskName);
			this.m_taskDesc = Utility.UTF8Convert(this.m_resTask.szTaskDesc);
			this.m_taskIcon = Utility.UTF8Convert(this.m_resTask.szTaskIcon);
			this.m_rewardId = this.m_resTask.dwRewardID;
			if (this.m_rewardId != 0u)
			{
				this.resAward = GameDataMgr.taskRewardDatabin.GetDataByKey(this.m_rewardId);
				if (this.resAward == null)
				{
				}
			}
		}

		public void UpdateState()
		{
			if (this.m_taskState == 3)
			{
				return;
			}
			if (this.IsHaveDone())
			{
				this.SetState(CTask.State.Have_Done);
			}
		}

		private bool IsHaveDone()
		{
			for (int i = 0; i < this.m_prerequisiteInfo.Length; i++)
			{
				if (this.m_prerequisiteInfo[i].m_valueTarget > 0 && !this.m_prerequisiteInfo[i].m_isReach)
				{
					return false;
				}
			}
			return true;
		}

		public void SetState(COM_TASK_STATE state)
		{
			this.m_taskState = (byte)state;
		}

		public void SetState(byte bTaskState)
		{
			this.m_taskState = bTaskState;
		}

		public void SetState(CTask.State state)
		{
			this.m_taskState = (byte)state;
		}

		public bool IsTaskIDAfterThisTask(uint after_taskid)
		{
			if (this.m_resTask.NextTaskID[0] == 0u)
			{
				return false;
			}
			if (this.m_resTask.NextTaskID[0] == after_taskid)
			{
				return true;
			}
			CTask task = Singleton<CTaskSys>.instance.model.GetTask(this.m_resTask.NextTaskID[0]);
			if (task != null)
			{
				return task.IsTaskIDAfterThisTask(after_taskid);
			}
			CTask cTask = TaskUT.Create_Task(this.m_resTask.NextTaskID[0]);
			return cTask != null && cTask.IsTaskIDAfterThisTask(after_taskid);
		}
	}
}
