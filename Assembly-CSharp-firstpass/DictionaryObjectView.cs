using System;
using System.Collections;
using System.Collections.Generic;

public class DictionaryObjectView<TKey, TValue> : IEnumerable, IEnumerable<KeyValuePair<TKey, TValue>>
{
	public struct Enumerator : IDisposable, IEnumerator, IEnumerator<KeyValuePair<TKey, TValue>>
	{
		private Dictionary<object, object> Reference;

		private Dictionary<object, object>.Enumerator Iter;

		object IEnumerator.Current
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public KeyValuePair<TKey, TValue> Current
		{
			get
			{
				KeyValuePair<object, object> current = this.Iter.get_Current();
				TKey tKey;
				if (current.get_Key() != null)
				{
					KeyValuePair<object, object> current2 = this.Iter.get_Current();
					tKey = (TKey)((object)current2.get_Key());
				}
				else
				{
					tKey = default(TKey);
				}
				KeyValuePair<object, object> current3 = this.Iter.get_Current();
				TValue tValue;
				if (current3.get_Value() != null)
				{
					KeyValuePair<object, object> current4 = this.Iter.get_Current();
					tValue = (TValue)((object)current4.get_Value());
				}
				else
				{
					tValue = default(TValue);
				}
				return new KeyValuePair<TKey, TValue>(tKey, tValue);
			}
		}

		public Enumerator(Dictionary<object, object> InReference)
		{
			this.Reference = InReference;
			this.Iter = this.Reference.GetEnumerator();
		}

		public void Reset()
		{
			this.Iter = this.Reference.GetEnumerator();
		}

		public void Dispose()
		{
			this.Iter.Dispose();
			this.Reference = null;
		}

		public bool MoveNext()
		{
			return this.Iter.MoveNext();
		}
	}

	protected Dictionary<object, object> Context;

	public int Count
	{
		get
		{
			return this.Context.get_Count();
		}
	}

	public TValue this[TKey key]
	{
		get
		{
			object obj = this.Context.get_Item(key);
			return (obj != null) ? ((TValue)((object)obj)) : default(TValue);
		}
		set
		{
			this.Context.set_Item(key, value);
		}
	}

	public DictionaryObjectView()
	{
		this.Context = new Dictionary<object, object>();
	}

	public DictionaryObjectView(int capacity)
	{
		this.Context = new Dictionary<object, object>(capacity);
	}

	IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
	{
		return this.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		throw new NotImplementedException();
	}

	public void Add(TKey key, TValue value)
	{
		this.Context.Add(key, value);
	}

	public void Clear()
	{
		this.Context.Clear();
	}

	public bool ContainsKey(TKey key)
	{
		return this.Context.ContainsKey(key);
	}

	public bool Remove(TKey key)
	{
		return this.Context.Remove(key);
	}

	public bool TryGetValue(TKey key, out TValue value)
	{
		object obj = null;
		bool result = this.Context.TryGetValue(key, ref obj);
		value = ((obj != null) ? ((TValue)((object)obj)) : default(TValue));
		return result;
	}

	public DictionaryObjectView<TKey, TValue>.Enumerator GetEnumerator()
	{
		return new DictionaryObjectView<TKey, TValue>.Enumerator(this.Context);
	}
}
