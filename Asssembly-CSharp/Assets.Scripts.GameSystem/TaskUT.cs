using Assets.Scripts.Framework;
using CSProtocol;
using ResData;
using System;

namespace Assets.Scripts.GameSystem
{
	public class TaskUT
	{
		public static CTask Create_Task(uint taskid)
		{
			if (taskid == 0u)
			{
				return null;
			}
			ResTask dataByKey = GameDataMgr.taskDatabin.GetDataByKey(taskid);
			if (dataByKey == null)
			{
				return null;
			}
			return new CTask(taskid, dataByKey);
		}

		public static void Add_Task(CTask task)
		{
			if (task == null)
			{
				return;
			}
			Singleton<CTaskSys>.instance.model.AddTask(task);
		}

		public static CUseable GetUseableFromReward(int reward_type, uint equip_id, int count)
		{
			if (reward_type != 4)
			{
				return null;
			}
			return CUseableManager.CreateUseable(COM_ITEM_TYPE.COM_OBJTYPE_ITEMEQUIP, 0uL, equip_id, count, 0);
		}
	}
}
