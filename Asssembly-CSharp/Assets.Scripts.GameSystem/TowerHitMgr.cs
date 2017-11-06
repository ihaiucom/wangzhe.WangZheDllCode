using ResData;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameSystem
{
	public class TowerHitMgr
	{
		private DictionaryView<uint, TowerHit> _data = new DictionaryView<uint, TowerHit>();

		public void Init()
		{
		}

		public void Clear()
		{
			DictionaryView<uint, TowerHit>.Enumerator enumerator = this._data.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<uint, TowerHit> current = enumerator.Current;
				TowerHit value = current.get_Value();
				if (value != null)
				{
					value.Clear();
				}
			}
			this._data.Clear();
		}

		public void TryActive(uint id, GameObject target = null)
		{
			if (target == null)
			{
				return;
			}
			TowerHit towerHit = null;
			this._data.TryGetValue(id, out towerHit);
			if (towerHit != null)
			{
				towerHit.TryActive(target);
			}
		}

		public void Register(uint objid, RES_ORGAN_TYPE type)
		{
			if (!this._data.ContainsKey(objid))
			{
				this._data.Add(objid, new TowerHit(type));
			}
			else
			{
				this._data[objid].Clear();
				this._data[objid] = new TowerHit(type);
			}
		}

		public void Remove(uint id)
		{
			TowerHit towerHit = null;
			this._data.TryGetValue(id, out towerHit);
			if (towerHit != null)
			{
				towerHit.Clear();
			}
			this._data.Remove(id);
		}
	}
}
