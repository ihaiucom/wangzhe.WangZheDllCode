using System;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts.Framework
{
	public class MultiValueHashDictionary<TKey, TValue> : DictionaryView<TKey, HashSet<TValue>>
	{
		public void Add(TKey key, TValue value)
		{
			HashSet<TValue> hashSet = null;
			if (!base.TryGetValue(key, out hashSet))
			{
				hashSet = new HashSet<TValue>();
				base.Add(key, hashSet);
			}
			hashSet.Add(value);
		}

		public HashSet<TValue> GetValues(TKey key, bool returnEmptySet = true)
		{
			HashSet<TValue> result = null;
			if (!base.TryGetValue(key, out result) && returnEmptySet)
			{
				result = new HashSet<TValue>();
			}
			return result;
		}

		public TValue[] GetAllValueArray()
		{
			ListLinqView<TValue> listLinqView = new ListLinqView<TValue>();
			DictionaryView<TKey, HashSet<TValue>>.Enumerator enumerator = base.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<TKey, HashSet<TValue>> current = enumerator.Current;
				HashSet<TValue> value = current.Value;
				if (value != null)
				{
					IEnumerator enumerator2 = value.GetEnumerator();
					while (enumerator2.MoveNext())
					{
						TValue item = (TValue)((object)enumerator2.Current);
						listLinqView.Add(item);
					}
				}
			}
			return listLinqView.ToArray();
		}
	}
}
