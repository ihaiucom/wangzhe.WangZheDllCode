using System;
using System.Collections;
using System.Collections.Generic;

public struct ReadonlyContext<T>
{
	public struct Enumerator : IDisposable, IEnumerator, IEnumerator<T>
	{
		private List<T> Reference;

		private List<T>.Enumerator IterReference;

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
				return this.IterReference.get_Current();
			}
		}

		public Enumerator(List<T> InRefernce)
		{
			this.Reference = InRefernce;
			this.IterReference = InRefernce.GetEnumerator();
		}

		public void Reset()
		{
			this.IterReference = this.Reference.GetEnumerator();
		}

		public void Dispose()
		{
			this.IterReference.Dispose();
			this.Reference = null;
		}

		public bool MoveNext()
		{
			return this.IterReference.MoveNext();
		}
	}

	private List<T> Reference;

	public bool isValidReference
	{
		get
		{
			return this.Reference != null;
		}
	}

	public T this[int index]
	{
		get
		{
			return this.Reference.get_Item(index);
		}
	}

	public int Count
	{
		get
		{
			return this.Reference.get_Count();
		}
	}

	public ReadonlyContext(List<T> InReference)
	{
		this.Reference = InReference;
	}

	public ReadonlyContext<T>.Enumerator GetEnumerator()
	{
		return new ReadonlyContext<T>.Enumerator(this.Reference);
	}
}
