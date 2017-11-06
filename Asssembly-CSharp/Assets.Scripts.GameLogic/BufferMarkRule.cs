using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using ResData;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.GameLogic
{
	public class BufferMarkRule
	{
		private BuffHolderComponent buffHolder;

		private DictionaryView<ulong, BufferMark> buffMarkSet = new DictionaryView<ulong, BufferMark>();

		public void Init(BuffHolderComponent _buffHolder)
		{
			this.buffHolder = _buffHolder;
			this.Clear();
		}

		public void Clear()
		{
			DictionaryView<ulong, BufferMark>.Enumerator enumerator = this.buffMarkSet.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<ulong, BufferMark> current = enumerator.Current;
				BufferMark value = current.get_Value();
				value.SetCurLayer(0);
			}
			this.buffMarkSet.Clear();
		}

		public bool CheckDependMark(uint _objID, int _markID)
		{
			ResSkillMarkCfgInfo dataByKey = GameDataMgr.skillMarkDatabin.GetDataByKey((long)_markID);
			if (dataByKey == null)
			{
				return false;
			}
			if (dataByKey.iDependCfgID == 0)
			{
				return true;
			}
			int iDependCfgID = dataByKey.iDependCfgID;
			ulong key = (ulong)_objID | (ulong)((ulong)((long)iDependCfgID) << 32);
			BufferMark bufferMark;
			return this.buffMarkSet.TryGetValue(key, out bufferMark) && bufferMark != null && bufferMark.GetCurLayer() > 0;
		}

		public void AddBufferMark(PoolObjHandle<ActorRoot> _originator, int _markID, uint _markType, SkillUseContext inUseContext)
		{
			if (!_originator)
			{
				return;
			}
			uint objID = _originator.handle.ObjID;
			if (!this.CheckDependMark(objID, _markID))
			{
				return;
			}
			ulong key = (ulong)objID | (ulong)((ulong)((long)_markID) << 32);
			BufferMark bufferMark;
			if (this.buffMarkSet.TryGetValue(key, out bufferMark))
			{
				bufferMark.AutoTrigger(_originator, inUseContext);
			}
			else
			{
				bufferMark = new BufferMark(_markID);
				if (bufferMark.cfgData != null)
				{
					bufferMark.Init(this.buffHolder, _originator, _markType);
					this.buffMarkSet.Add(key, bufferMark);
				}
			}
		}

		public void ClearBufferMark(PoolObjHandle<ActorRoot> _originator, int _markID)
		{
			if (!_originator)
			{
				return;
			}
			uint objID = _originator.handle.ObjID;
			ulong key = (ulong)objID | (ulong)((ulong)((long)_markID) << 32);
			BufferMark bufferMark;
			if (this.buffMarkSet.TryGetValue(key, out bufferMark))
			{
				bufferMark.DecLayer(0);
			}
		}

		public void RemoveBufferMark(PoolObjHandle<ActorRoot> _originator, int _markID)
		{
			if (!_originator)
			{
				return;
			}
			uint objID = _originator.handle.ObjID;
			ulong key = (ulong)objID | (ulong)((ulong)((long)_markID) << 32);
			BufferMark bufferMark;
			if (this.buffMarkSet.TryGetValue(key, out bufferMark))
			{
				bufferMark.DecLayer(1);
			}
		}

		public void ClearBufferMark(int _typeMask)
		{
			DictionaryView<ulong, BufferMark>.Enumerator enumerator = this.buffMarkSet.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<ulong, BufferMark> current = enumerator.Current;
				BufferMark value = current.get_Value();
				int markType = (int)value.GetMarkType();
				if ((_typeMask & 1 << markType) > 0)
				{
					value.SetCurLayer(0);
				}
			}
		}

		public void TriggerBufferMark(PoolObjHandle<ActorRoot> _originator, int _markID, SkillUseContext inUseContext)
		{
			if (!_originator)
			{
				return;
			}
			uint objID = _originator.handle.ObjID;
			ulong key = (ulong)objID | (ulong)((ulong)((long)_markID) << 32);
			BufferMark bufferMark;
			if (this.buffMarkSet.TryGetValue(key, out bufferMark))
			{
				bufferMark.UpperTrigger(_originator, inUseContext);
			}
		}

		public void UpdateLogic(int nDelta)
		{
			DictionaryView<ulong, BufferMark>.Enumerator enumerator = this.buffMarkSet.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<ulong, BufferMark> current = enumerator.Current;
				current.get_Value().UpdateLogic(nDelta);
			}
		}
	}
}
