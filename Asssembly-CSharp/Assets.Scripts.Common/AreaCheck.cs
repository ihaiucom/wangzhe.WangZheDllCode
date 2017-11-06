using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Common
{
	public class AreaCheck
	{
		private class AroundRecord
		{
			public PoolObjHandle<ActorRoot> actor;

			public ulong frame;

			public AroundRecord(PoolObjHandle<ActorRoot> _actor, ulong _frame)
			{
				this.actor = _actor;
				this.frame = _frame;
			}
		}

		public enum ActorAction
		{
			Enter,
			Hover,
			Leave
		}

		public delegate void ActorProcess(PoolObjHandle<ActorRoot> actor, AreaCheck.ActorAction action);

		private ActorFilterDelegate _actorAreaFunc;

		private AreaCheck.ActorProcess _actorProcess;

		private uint _checkFreq = 5u;

		private List<PoolObjHandle<ActorRoot>> _waitCheckRef;

		private DictionaryView<uint, AreaCheck.AroundRecord> _aroundRecords;

		public AreaCheck(ActorFilterDelegate actorAreaFunc, AreaCheck.ActorProcess actorProcess, List<PoolObjHandle<ActorRoot>> checkList)
		{
			this._actorAreaFunc = actorAreaFunc;
			this._actorProcess = actorProcess;
			this._waitCheckRef = checkList;
			this._aroundRecords = new DictionaryView<uint, AreaCheck.AroundRecord>();
		}

		public bool HasActorIn(PoolObjHandle<ActorRoot> actor)
		{
			return this._aroundRecords.ContainsKey(actor.handle.ObjID);
		}

		public int CountActors(ActorFilterDelegate actorFilter)
		{
			int num = 0;
			DictionaryView<uint, AreaCheck.AroundRecord>.Enumerator enumerator = this._aroundRecords.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (actorFilter != null)
				{
					KeyValuePair<uint, AreaCheck.AroundRecord> current = enumerator.Current;
					if (!actorFilter(ref current.get_Value().actor))
					{
						continue;
					}
				}
				num++;
			}
			return num;
		}

		public void UpdateLogic(uint checkPos)
		{
			uint curFrameNum = Singleton<FrameSynchr>.GetInstance().CurFrameNum;
			if (curFrameNum % this._checkFreq == checkPos % this._checkFreq)
			{
				int count = this._waitCheckRef.get_Count();
				for (int i = 0; i < count; i++)
				{
					PoolObjHandle<ActorRoot> actor = this._waitCheckRef.get_Item(i);
					if (this._actorAreaFunc == null || this._actorAreaFunc(ref actor))
					{
						if (this._aroundRecords.ContainsKey(actor.handle.ObjID))
						{
							AreaCheck.AroundRecord aroundRecord = this._aroundRecords[actor.handle.ObjID];
							aroundRecord.frame = (ulong)curFrameNum;
							this._actorProcess(actor, AreaCheck.ActorAction.Hover);
						}
						else
						{
							this._aroundRecords.Add(actor.handle.ObjID, new AreaCheck.AroundRecord(actor, (ulong)curFrameNum));
							this._actorProcess(actor, AreaCheck.ActorAction.Enter);
						}
					}
				}
				if (this._aroundRecords.Count > 0)
				{
					uint key = 0u;
					AreaCheck.AroundRecord aroundRecord2 = null;
					ulong num = (ulong)curFrameNum;
					DictionaryView<uint, AreaCheck.AroundRecord>.Enumerator enumerator = this._aroundRecords.GetEnumerator();
					while (enumerator.MoveNext())
					{
						KeyValuePair<uint, AreaCheck.AroundRecord> current = enumerator.Current;
						AreaCheck.AroundRecord value = current.get_Value();
						if (value.frame < num)
						{
							num = value.frame;
							KeyValuePair<uint, AreaCheck.AroundRecord> current2 = enumerator.Current;
							key = current2.get_Key();
							aroundRecord2 = value;
						}
					}
					if (aroundRecord2 != null)
					{
						this._aroundRecords.Remove(key);
						this._actorProcess(aroundRecord2.actor, AreaCheck.ActorAction.Leave);
					}
				}
			}
		}
	}
}
