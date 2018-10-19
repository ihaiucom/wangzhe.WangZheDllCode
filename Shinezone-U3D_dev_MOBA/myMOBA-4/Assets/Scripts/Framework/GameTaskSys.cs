using AGE;
using ResData;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Assets.Scripts.Framework
{
	public class GameTaskSys
	{
		public delegate void TaskEventDelegate(GameTask gt);

		private DictionaryView<uint, ListView<ResGameTaskGroup>> _groupData;

		private DictionaryView<uint, GameTask> _taskDict;

		private string _classSpace;

		private DatabinTable<ResGameTask, uint> _taskDB;

		private DatabinTable<ResGameTaskGroup, uint> _taskGroupDB;

		private ActionHelper _actionHelper;

		public event GameTaskSys.TaskEventDelegate OnTaskReady;

		public event GameTaskSys.TaskEventDelegate OnTaskStart;

		public event GameTaskSys.TaskEventDelegate OnTaskGoing;

		public event GameTaskSys.TaskEventDelegate OnTaskClose;

		public bool HasTask
		{
			get
			{
				return this._taskDict.Count > 0;
			}
		}

		public void Initial(string classSpace, DatabinTable<ResGameTask, uint> taskDB, DatabinTable<ResGameTaskGroup, uint> taskGroupDB, ActionHelper actionHelper)
		{
			this._classSpace = classSpace;
			this._taskDB = taskDB;
			this._taskGroupDB = taskGroupDB;
			this._actionHelper = actionHelper;
			this._groupData = new DictionaryView<uint, ListView<ResGameTaskGroup>>();
			this._taskGroupDB.Accept(new Action<ResGameTaskGroup>(this.OnVisit));
			this._taskDict = new DictionaryView<uint, GameTask>();
		}

		private void OnVisit(ResGameTaskGroup InGroup)
		{
			ListView<ResGameTaskGroup> listView;
			if (this._groupData.ContainsKey(InGroup.dwGroupTask))
			{
				listView = this._groupData[InGroup.dwGroupTask];
			}
			else
			{
				listView = new ListView<ResGameTaskGroup>();
				this._groupData.Add(InGroup.dwGroupTask, listView);
			}
			listView.Add(InGroup);
		}

		public void Clear()
		{
			if (this._taskDict == null)
			{
				return;
			}
			DictionaryView<uint, GameTask>.Enumerator enumerator = this._taskDict.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, GameTask> current = enumerator.Current;
				current.Value.Destroy();
			}
		}

		public GameTask AddTask(uint taskId, bool autoStart)
		{
			if (this._taskDict.ContainsKey(taskId))
			{
				return this._taskDict[taskId];
			}
			GameTask gameTask = this.CreateTask(taskId);
			if (gameTask != null)
			{
				this._taskDict.Add(taskId, gameTask);
				if (autoStart)
				{
					gameTask.Start();
				}
			}
			return gameTask;
		}

		public GameTask GetTask(uint taskId, bool addIfNone = false)
		{
			if (this._taskDict.ContainsKey(taskId))
			{
				return this._taskDict[taskId];
			}
			if (addIfNone)
			{
				return this.AddTask(taskId, false);
			}
			return null;
		}

		internal ListView<ResGameTaskGroup> GetGroupData(uint groupTaskId)
		{
			if (this._groupData.ContainsKey(groupTaskId))
			{
				return this._groupData[groupTaskId];
			}
			return null;
		}

		private GameTask CreateTask(uint taskId)
		{
			ResGameTask dataByKey = this._taskDB.GetDataByKey(taskId);
			if (dataByKey != null)
			{
				string text = Utility.UTF8Convert(dataByKey.szType);
				GameTask gameTask;
				if ("Group" == text)
				{
					gameTask = new GameTaskGroup();
				}
				else
				{
					gameTask = (GameTask)Assembly.GetExecutingAssembly().CreateInstance(this._classSpace + ".GameTask" + text);
				}
				gameTask.Initial(dataByKey, this);
				return gameTask;
			}
			return null;
		}

		internal void _OnTaskStart(GameTask gt)
		{
			if (null != this._actionHelper)
			{
				this._actionHelper.PlayAction(gt.StartAction);
			}
			if (this.OnTaskStart != null)
			{
				this.OnTaskStart(gt);
			}
		}

		internal void _OnTaskReady(GameTask gt)
		{
			if (this.OnTaskReady != null)
			{
				this.OnTaskReady(gt);
			}
		}

		internal void _OnTaskGoing(GameTask gt)
		{
			if (this.OnTaskGoing != null)
			{
				this.OnTaskGoing(gt);
			}
		}

		internal void _OnTaskClose(GameTask gt)
		{
			if (null != this._actionHelper)
			{
				this._actionHelper.PlayAction(gt.CloseAction);
			}
			if (this.OnTaskClose != null)
			{
				this.OnTaskClose(gt);
			}
		}
	}
}
