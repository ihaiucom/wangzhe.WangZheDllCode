using System;
using System.Collections;
using System.Collections.Generic;

public class ListView<T> : ListViewBase, IEnumerable, IEnumerable<T>
{
	private struct ComparerConverter : IComparer<object>
	{
		private IComparer<T> ComparerRef;

		public ComparerConverter(IComparer<T> comparer)
		{
			this.ComparerRef = comparer;
		}

		public int Compare(object x, object y)
		{
			return this.ComparerRef.Compare((T)((object)x), (T)((object)y));
		}
	}

	private struct ComparisonConverter : IComparer<object>
	{
		private Comparison<T> ComparerRef;

		public ComparisonConverter(Comparison<T> comparer)
		{
			this.ComparerRef = comparer;
		}

		public int Compare(object x, object y)
		{
			return this.ComparerRef.Invoke((T)((object)x), (T)((object)y));
		}
	}

	public struct Enumerator : IDisposable, IEnumerator, IEnumerator<T>
	{
		private List<object> Reference;

		private List<object>.Enumerator Iter;

		object IEnumerator.Current
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public T Current
		{
			get
			{
				return (T)((object)this.Iter.get_Current());
			}
		}

		public Enumerator(List<object> InReference)
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

	public T this[int index]
	{
		get
		{
			return (T)((object)this.Context.get_Item(index));
		}
		set
		{
			this.Context.set_Item(index, value);
		}
	}

	public ListView()
	{
		this.Context = new List<object>();
	}

	public ListView(IEnumerable<T> collection)
	{
		this.Context = new List<object>();
		IEnumerator<T> enumerator = collection.GetEnumerator();
		while (enumerator.MoveNext())
		{
			this.Context.Add(enumerator.get_Current());
		}
	}

	public ListView(int capacity)
	{
		this.Context = new List<object>(capacity);
	}

	IEnumerator<T> IEnumerable<T>.GetEnumerator()
	{
		return this.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		throw new NotImplementedException();
	}

	public void Add(T item)
	{
		this.Context.Add(item);
	}

	public void AddRange(IEnumerable<T> collection)
	{
		if (collection != null)
		{
			IEnumerator<T> enumerator = collection.GetEnumerator();
			while (enumerator.MoveNext())
			{
				this.Context.Add(enumerator.get_Current());
			}
		}
	}

	public int BinarySearch(T item, IComparer<T> comparer)
	{
		return this.Context.BinarySearch(item, new ListView<T>.ComparerConverter(comparer));
	}

	public bool Contains(T item)
	{
		return this.Context.Contains(item);
	}

	public int IndexOf(T item)
	{
		return this.Context.IndexOf(item);
	}

	public void Insert(int index, T item)
	{
		this.Context.Insert(index, item);
	}

	public int LastIndexOf(T item)
	{
		return this.Context.LastIndexOf(item);
	}

	public bool Remove(T item)
	{
		return this.Context.Remove(item);
	}

	public void Sort(IComparer<T> comparer)
	{
		this.Context.Sort(new ListView<T>.ComparerConverter(comparer));
	}

	public void Sort(Comparison<T> comparison)
	{
		this.Context.Sort(new ListView<T>.ComparisonConverter(comparison));
	}

	public ListView<T>.Enumerator GetEnumerator()
	{
		return new ListView<T>.Enumerator(this.Context);
	}
}
