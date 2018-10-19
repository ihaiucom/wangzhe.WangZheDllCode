using Assets.Scripts.Common;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AGE
{
	public class Track : PooledClassObject
	{
		private Type eventType;

		private bool isDurationEvent;

		private bool isCondition;

		public List<BaseEvent> trackEvents = new List<BaseEvent>();

		public Action action;

		public bool started;

		public bool enabled;

		public uint startCount;

		private bool supportEditMode;

		private List<DurationEvent> activeEvents = new List<DurationEvent>();

		public Dictionary<int, bool> waitForConditions;

		public int curTime;

		private int preExcuteTime;

		public int trackIndex = -1;

		public Color color = Color.red;

		public string trackName = string.Empty;

		public bool execOnActionCompleted;

		public bool execOnForceStopped;

		public int Length
		{
			get
			{
				return this.action.length;
			}
		}

		public bool Loop
		{
			get
			{
				return this.action.loop;
			}
		}

		public bool IsDurationEvent
		{
			get
			{
				return this.isDurationEvent;
			}
		}

		public Type EventType
		{
			get
			{
				return this.eventType;
			}
		}

		public Track()
		{
			this.bChkReset = false;
		}

		public Track(Action _action, Type _eventType)
		{
			this.CopyData(_action, _eventType);
		}

		public void CopyData(Action _action, Type _eventType)
		{
			this.action = _action;
			this.eventType = _eventType;
			if (this.eventType.IsSubclassOf(typeof(DurationEvent)))
			{
				this.isDurationEvent = true;
			}
			if (this.eventType.IsSubclassOf(typeof(TickCondition)) || this.eventType.IsSubclassOf(typeof(DurationCondition)))
			{
				this.isCondition = true;
			}
			BaseEvent baseEvent = (BaseEvent)Activator.CreateInstance(this.eventType);
			this.supportEditMode = baseEvent.SupportEditMode();
			this.curTime = 0;
			this.preExcuteTime = 0;
		}

		public void CopyData(Track src)
		{
			this.action = src.action;
			this.eventType = src.eventType;
			this.isDurationEvent = src.isDurationEvent;
			this.isCondition = src.isCondition;
			this.supportEditMode = src.supportEditMode;
			this.curTime = 0;
			this.preExcuteTime = 0;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.waitForConditions = null;
			this.curTime = 0;
			this.preExcuteTime = 0;
			this.trackIndex = -1;
			this.trackName = string.Empty;
			this.color = Color.red;
			this.execOnActionCompleted = false;
			this.execOnForceStopped = false;
			this.eventType = null;
			this.isDurationEvent = false;
			this.isCondition = false;
			this.trackEvents.Clear();
			this.action = null;
			this.started = false;
			this.enabled = false;
			this.startCount = 0u;
			this.supportEditMode = false;
			this.activeEvents.Clear();
		}

		public override void OnRelease()
		{
			int count = this.trackEvents.Count;
			for (int i = 0; i < count; i++)
			{
				this.trackEvents[i].Release();
			}
			this.trackEvents.Clear();
			this.activeEvents.Clear();
			this.waitForConditions = null;
			this.action = null;
		}

		public Track Clone()
		{
			Track track = ClassObjPool<Track>.Get();
			track.CopyData(this);
			int count = this.trackEvents.Count;
			for (int i = 0; i < count; i++)
			{
				BaseEvent baseEvent = this.trackEvents[i];
				BaseEvent baseEvent2 = baseEvent.Clone();
				baseEvent2.track = track;
				track.trackEvents.Add(baseEvent2);
			}
			track.waitForConditions = this.waitForConditions;
			track.enabled = this.enabled;
			track.color = this.color;
			track.trackName = this.trackName;
			track.execOnActionCompleted = this.execOnActionCompleted;
			track.execOnForceStopped = this.execOnForceStopped;
			return track;
		}

		public BaseEvent AddEvent(int _time, int _length)
		{
			BaseEvent baseEvent = (BaseEvent)Activator.CreateInstance(this.eventType);
			baseEvent.time = _time;
			if (this.isDurationEvent)
			{
				(baseEvent as DurationEvent).length = _length;
			}
			int num = 0;
			if (this.LocateInsertPos(_time, out num))
			{
				if (num > this.trackEvents.Count)
				{
					num = this.trackEvents.Count;
				}
				this.trackEvents.Insert(num, baseEvent);
			}
			else
			{
				this.trackEvents.Add(baseEvent);
			}
			baseEvent.track = this;
			BaseEvent baseEvent2 = baseEvent.Clone();
			baseEvent2.Release();
			return baseEvent;
		}

		public bool LocateEvent(int _curTime, out int _result)
		{
			_result = 0;
			int length = this.Length;
			int count = this.trackEvents.Count;
			if (count == 0)
			{
				return false;
			}
			if (_curTime < 0)
			{
				_curTime = 0;
			}
			else if (_curTime > length)
			{
				_curTime = length;
			}
			int num = 0;
			int num2 = this.trackEvents.Count - 1;
			while (num != num2)
			{
				int num3 = (num + num2) / 2 + 1;
				if (_curTime < this.trackEvents[num3].time)
				{
					num2 = num3 - 1;
				}
				else
				{
					num = num3;
				}
			}
			int time = this.trackEvents[0].time;
			int time2 = this.trackEvents[count - 1].time;
			int num4;
			if (num == 0 && _curTime < time)
			{
				num4 = -1;
			}
			else
			{
				num4 = num;
			}
			if (num4 < 0)
			{
				_result = -1 + _curTime / time;
			}
			else if (num4 == count - 1)
			{
				_result = count - 1 + (_curTime - time2) / (length - time2);
			}
			else
			{
				int time3 = this.trackEvents[num4].time;
				int time4 = this.trackEvents[num4 + 1].time;
				_result = num4 + (_curTime - time3) / (time4 - time3);
			}
			return true;
		}

		private bool LocateInsertPos(int _curTime, out int _result)
		{
			_result = 0;
			int length = this.Length;
			int count = this.trackEvents.Count;
			if (count == 0)
			{
				return false;
			}
			if (_curTime < 0)
			{
				_curTime = 0;
			}
			else if (_curTime > length)
			{
				_curTime = length;
			}
			int num = 0;
			int num2 = this.trackEvents.Count - 1;
			while (num != num2)
			{
				int num3 = (num + num2) / 2 + 1;
				if (_curTime < this.trackEvents[num3].time)
				{
					num2 = num3 - 1;
				}
				else
				{
					num = num3;
				}
			}
			int time = this.trackEvents[0].time;
			int num4;
			if (num == 0 && _curTime < time)
			{
				num4 = -1;
			}
			else
			{
				num4 = num;
			}
			if (num4 < 0)
			{
				_result = 0;
			}
			else if (num4 == count - 1)
			{
				_result = count;
			}
			else
			{
				_result = num4;
			}
			return true;
		}

		public void Process(int _curTime)
		{
			this.preExcuteTime = this.curTime;
			this.curTime = _curTime;
			int num = 0;
			if (!this.LocateEvent(_curTime, out num) || num < 0)
			{
				return;
			}
			int length = this.Length;
			if (_curTime >= length)
			{
				num = this.trackEvents.Count - 1;
			}
			int num2 = num - 1;
			if (num2 < 0)
			{
				num2 = 0;
			}
			int num3 = num + 1;
			if (num3 >= this.trackEvents.Count)
			{
				num3 = num;
			}
			if (this.isDurationEvent)
			{
				for (int i = num2; i < this.trackEvents.Count; i++)
				{
					DurationEvent durationEvent = this.trackEvents[i] as DurationEvent;
					if (this.CheckSkip(_curTime, durationEvent.Start) && durationEvent.CheckConditions(this.action))
					{
						if (this.activeEvents.Count == 0)
						{
							durationEvent.Enter(this.action, this);
						}
						else
						{
							DurationEvent durationEvent2 = this.activeEvents[0];
							int blendTime = durationEvent2.End - durationEvent.Start;
							durationEvent.EnterBlend(this.action, this, durationEvent2, blendTime);
						}
						this.activeEvents.Add(durationEvent);
					}
					if (this.CheckSkip(_curTime, durationEvent.End) && this.activeEvents.Contains(durationEvent))
					{
						if (this.activeEvents.Count > 1)
						{
							DurationEvent durationEvent3 = this.activeEvents[1];
							int blendTime2 = durationEvent.End - durationEvent3.Start;
							durationEvent.LeaveBlend(this.action, this, durationEvent3, blendTime2);
						}
						else
						{
							durationEvent.Leave(this.action, this);
						}
						this.activeEvents.Remove(durationEvent);
					}
				}
			}
			else
			{
				for (int j = num2; j < this.trackEvents.Count; j++)
				{
					TickEvent tickEvent = this.trackEvents[j] as TickEvent;
					if (this.CheckSkip(_curTime, tickEvent.time) && tickEvent.CheckConditions(this.action))
					{
						tickEvent.Process(this.action, this);
					}
				}
				if (num != num3)
				{
					TickEvent tickEvent2 = this.trackEvents[num] as TickEvent;
					TickEvent tickEvent3 = this.trackEvents[num3] as TickEvent;
					float blendWeight = (float)(_curTime - tickEvent2.time) / (float)(tickEvent3.time - tickEvent2.time);
					tickEvent3.ProcessBlend(this.action, this, tickEvent2, blendWeight);
				}
				else
				{
					TickEvent tickEvent4 = this.trackEvents[num] as TickEvent;
					int localTime = _curTime - tickEvent4.time;
					tickEvent4.PostProcess(this.action, this, localTime);
				}
			}
			if (this.activeEvents.Count == 1)
			{
				DurationEvent durationEvent4 = this.activeEvents[0];
				int localTime2;
				if (_curTime >= durationEvent4.Start)
				{
					localTime2 = _curTime - durationEvent4.Start;
				}
				else
				{
					localTime2 = _curTime + length - durationEvent4.Start;
				}
				durationEvent4.Process(this.action, this, localTime2);
			}
			else if (this.activeEvents.Count == 2)
			{
				DurationEvent durationEvent5 = this.activeEvents[0];
				DurationEvent durationEvent6 = this.activeEvents[1];
				if (durationEvent5.Start < durationEvent6.Start && durationEvent5.End < length)
				{
					int localTime3 = _curTime - durationEvent6.Start;
					int prevLocalTime = _curTime - durationEvent5.Start;
					float blendWeight2 = (float)(_curTime - durationEvent6.Start) / (float)(durationEvent5.End - durationEvent6.Start);
					durationEvent6.ProcessBlend(this.action, this, localTime3, durationEvent5, prevLocalTime, blendWeight2);
				}
				else if (durationEvent5.Start < durationEvent6.Start && durationEvent5.End >= length)
				{
					if (_curTime >= durationEvent6.Start)
					{
						int localTime4 = _curTime - durationEvent6.Start;
						int prevLocalTime2 = _curTime - durationEvent5.Start;
						float blendWeight3 = (float)(_curTime - durationEvent6.Start) / (float)(durationEvent5.End - durationEvent6.Start);
						durationEvent6.ProcessBlend(this.action, this, localTime4, durationEvent5, prevLocalTime2, blendWeight3);
					}
					else
					{
						int localTime5 = _curTime + length - durationEvent6.Start;
						int prevLocalTime3 = _curTime + length - durationEvent5.Start;
						float blendWeight4 = (float)(_curTime + length - durationEvent6.Start) / (float)(durationEvent5.End - durationEvent6.Start);
						durationEvent6.ProcessBlend(this.action, this, localTime5, durationEvent5, prevLocalTime3, blendWeight4);
					}
				}
				else
				{
					int localTime6 = _curTime - durationEvent6.Start;
					int prevLocalTime4 = _curTime + length - durationEvent5.Start;
					float blendWeight5 = (float)(_curTime - durationEvent6.Start) / (float)(durationEvent5.End - length - durationEvent6.Start);
					durationEvent6.ProcessBlend(this.action, this, localTime6, durationEvent5, prevLocalTime4, blendWeight5);
				}
			}
		}

		protected bool CheckSkip(int _curTime, int _checkTime)
		{
			return _checkTime < _curTime && _checkTime >= this.preExcuteTime;
		}

		public BaseEvent GetOffsetEvent(BaseEvent _curEvent, int _offset)
		{
			int num = this.trackEvents.LastIndexOf(_curEvent);
			if (this.Loop)
			{
				int num2 = (num + _offset) % this.trackEvents.Count;
				if (num2 < 0)
				{
					num2 += this.trackEvents.Count;
				}
				return this.trackEvents[num2];
			}
			int num3 = num + _offset;
			if (num3 < 0 || num3 >= this.trackEvents.Count)
			{
				return null;
			}
			return this.trackEvents[num3];
		}

		public BaseEvent GetEvent(int index)
		{
			if (index >= 0 && index < this.trackEvents.Count)
			{
				return this.trackEvents[index];
			}
			return null;
		}

		public int GetIndexOfEvent(BaseEvent _curEvent)
		{
			return this.trackEvents.LastIndexOf(_curEvent);
		}

		public int GetEventsCount()
		{
			return this.trackEvents.Count;
		}

		public void DoLoop()
		{
		}

		public void Start(Action _action)
		{
			if (!this.enabled)
			{
				return;
			}
			if (!this.isCondition)
			{
				_action.SetCondition(this, true);
			}
			this.curTime = 0;
			this.preExcuteTime = 0;
			this.started = true;
			this.startCount += 1u;
		}

		public void Stop(Action _action)
		{
			if (!this.started)
			{
				return;
			}
			for (int i = 0; i < this.activeEvents.Count; i++)
			{
				this.activeEvents[i].Leave(this.action, this);
			}
			this.activeEvents.Clear();
			if (!this.isCondition)
			{
				_action.SetCondition(this, false);
			}
			this.started = false;
		}

		public bool SupportEditMode()
		{
			return this.supportEditMode;
		}

		public bool CheckConditions(Action _action)
		{
			if (this.waitForConditions != null)
			{
				Dictionary<int, bool>.Enumerator enumerator = this.waitForConditions.GetEnumerator();
				while (enumerator.MoveNext())
				{
					KeyValuePair<int, bool> current = enumerator.Current;
					int key = current.Key;
					if (key >= 0 && key < _action.GetConditionCount())
					{
						bool arg_5B_0 = _action.GetCondition(_action.GetTrack(key));
						KeyValuePair<int, bool> current2 = enumerator.Current;
						if (arg_5B_0 != current2.Value)
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		public int GetEventEndTime()
		{
			if (this.trackEvents.Count == 0)
			{
				return 0;
			}
			if (this.isDurationEvent)
			{
				return (this.trackEvents[this.trackEvents.Count - 1] as DurationEvent).End + 33;
			}
			return (this.trackEvents[this.trackEvents.Count - 1] as TickEvent).time + 33;
		}

		public Dictionary<string, bool> GetAssociatedResources()
		{
			Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
			foreach (BaseEvent current in this.trackEvents)
			{
				Dictionary<string, bool> associatedResources = current.GetAssociatedResources();
				if (associatedResources != null)
				{
					foreach (string current2 in associatedResources.Keys)
					{
						if (dictionary.ContainsKey(current2))
						{
							Dictionary<string, bool> dictionary2;
							Dictionary<string, bool> expr_55 = dictionary2 = dictionary;
							string key;
							string expr_5A = key = current2;
							bool flag = dictionary2[key];
							expr_55[expr_5A] = (flag | associatedResources[current2]);
						}
						else
						{
							dictionary.Add(current2, associatedResources[current2]);
						}
					}
				}
			}
			return dictionary;
		}

		public void GetAssociatedResources(Dictionary<object, AssetRefType> results, int markID)
		{
			for (int i = 0; i < this.trackEvents.Count; i++)
			{
				BaseEvent baseEvent = this.trackEvents[i];
				if (baseEvent != null)
				{
					baseEvent.GetAssociatedResources(results, markID);
				}
			}
		}
	}
}
