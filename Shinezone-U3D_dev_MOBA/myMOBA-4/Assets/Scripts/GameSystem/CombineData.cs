using ResData;
using System;

namespace Assets.Scripts.GameSystem
{
	public class CombineData
	{
		public DictionaryView<uint, CTask> task_map = new DictionaryView<uint, CTask>();

		public ListView<CTask>[] type_taskList_array = new ListView<CTask>[3];

		public CombineData()
		{
			this.type_taskList_array[(int)((UIntPtr)0)] = new ListView<CTask>();
			this.type_taskList_array[(int)((UIntPtr)1)] = new ListView<CTask>();
			this.type_taskList_array[(int)((UIntPtr)2)] = new ListView<CTask>();
		}

		public void Clear()
		{
			this.task_map.Clear();
			for (int i = 0; i < this.type_taskList_array.Length; i++)
			{
				ListView<CTask> listView = this.type_taskList_array[i];
				if (listView != null)
				{
					listView.Clear();
				}
			}
		}

		public CTask GetTask(uint uid)
		{
			CTask result;
			this.task_map.TryGetValue(uid, out result);
			return result;
		}

		public void Add(uint uid, CTask task)
		{
			this._insert(uid, task);
		}

		public void Remove(uint uid)
		{
			if (uid == 0u)
			{
				return;
			}
			this._remove(uid);
		}

		public void Sort(RES_TASK_TYPE type)
		{
			ListView<CTask> listView = this.GetListView((int)type);
			if (listView != null)
			{
				listView.Sort(new Comparison<CTask>(this._sort_main));
			}
		}

		public int GetTask_Count(enTaskTab type, CTask.State state)
		{
			int num = 0;
			ListView<CTask> listView = this.GetListView((int)type);
			if (listView != null)
			{
				for (int i = 0; i < listView.Count; i++)
				{
					CTask cTask = listView[i];
					if (cTask != null && cTask.m_taskState == (byte)state)
					{
						num++;
					}
				}
			}
			return num;
		}

		public CTask GetMaxIndex_TaskID_InState(enTaskTab type, CTask.State state)
		{
			CTask cTask = null;
			uint num = 0u;
			ListView<CTask> listView = this.GetListView((int)type);
			if (listView != null)
			{
				for (int i = 0; i < listView.Count; i++)
				{
					CTask cTask2 = listView[i];
					if (cTask2 != null && cTask2.m_taskState == (byte)state && cTask2.m_resTask.dwMiShuIndex > num)
					{
						cTask = cTask2;
						num = cTask.m_resTask.dwMiShuIndex;
					}
				}
			}
			return cTask;
		}

		public ListView<CTask> GetListView(int type)
		{
			if (type >= 0 && type < 3)
			{
				return this.type_taskList_array[(int)((UIntPtr)type)];
			}
			return null;
		}

		public ListView<CTask> GetListView(uint type)
		{
			if (type >= 0u && type < 3u)
			{
				return this.type_taskList_array[(int)((UIntPtr)type)];
			}
			return null;
		}

		private int _sort_main(CTask l, CTask r)
		{
			if (l == r)
			{
				return 0;
			}
			if (l == null || r == null)
			{
				return 0;
			}
			if (r.m_taskState == 1)
			{
				if (l.m_taskState == 1)
				{
					return (int)(r.m_baseId - l.m_baseId);
				}
				return 1;
			}
			else if (r.m_taskState == 0)
			{
				if (l.m_taskState == 1)
				{
					return -1;
				}
				if (l.m_taskState == 0)
				{
					return (int)(r.m_baseId - l.m_baseId);
				}
				return 1;
			}
			else
			{
				if (r.m_taskState != 3)
				{
					return -1;
				}
				if (l.m_taskState != 3)
				{
					return -1;
				}
				return (int)(r.m_baseId - l.m_baseId);
			}
		}

		private void _insert(uint uid, CTask task)
		{
			if (uid == 0u || task == null)
			{
				return;
			}
			if (!this.task_map.ContainsKey(uid))
			{
				this.task_map.Add(uid, task);
				ListView<CTask> listView = this.GetListView(task.m_taskType);
				if (listView != null)
				{
					listView.Add(task);
				}
			}
		}

		private void _remove(uint uid)
		{
			CTask cTask;
			this.task_map.TryGetValue(uid, out cTask);
			if (cTask != null)
			{
				this.task_map.Remove(uid);
				ListView<CTask> listView = this.GetListView(cTask.m_taskType);
				if (listView != null)
				{
					listView.Remove(cTask);
				}
			}
		}

		private int _getIndex(CTask task)
		{
			if (task == null)
			{
				return -1;
			}
			ListView<CTask> listView = this.GetListView(task.m_taskType);
			if (listView == null)
			{
				return -1;
			}
			return listView.IndexOf(task);
		}
	}
}
