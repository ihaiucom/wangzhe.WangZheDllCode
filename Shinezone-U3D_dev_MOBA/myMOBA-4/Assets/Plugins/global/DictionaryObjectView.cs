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
				KeyValuePair<object, object> current = this.Iter.Current;
				TKey arg_7E_0;
				if (current.Key != null)
				{
					KeyValuePair<object, object> current2 = this.Iter.Current;
					arg_7E_0 = (TKey)((object)current2.Key);
				}
				else
				{
					arg_7E_0 = default(TKey);
				}
				KeyValuePair<object, object> current3 = this.Iter.Current;
				TValue arg_7E_1;
				if (current3.Value != null)
				{
					KeyValuePair<object, object> current4 = this.Iter.Current;
					arg_7E_1 = (TValue)((object)current4.Value);
				}
				else
				{
					arg_7E_1 = default(TValue);
				}
				return new KeyValuePair<TKey, TValue>(arg_7E_0, arg_7E_1);
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
			return this.Context.Count;
		}
	}

	public TValue this[TKey key]
	{
		get
		{
			object obj = this.Context[key];
			return (obj == null) ? default(TValue) : ((TValue)((object)obj));
		}
		set
		{
			this.Context[key] = value;
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
		bool result = this.Context.TryGetValue(key, out obj);
		value = ((obj == null) ? default(TValue) : ((TValue)((object)obj)));
		return result;
	}

	public DictionaryObjectView<TKey, TValue>.Enumerator GetEnumerator()
	{
		return new DictionaryObjectView<TKey, TValue>.Enumerator(this.Context);
	}
}
