using System;

public class Iterator : IDisposable
{
	private IntPtr swigCPtr;

	protected bool swigCMemOwn;

	public PlaylistItem pItem
	{
		get
		{
			IntPtr intPtr = AkSoundEnginePINVOKE.CSharp_Iterator_pItem_get(this.swigCPtr);
			return (intPtr == IntPtr.Zero) ? null : new PlaylistItem(intPtr, false);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_Iterator_pItem_set(this.swigCPtr, PlaylistItem.getCPtr(value));
		}
	}

	internal Iterator(IntPtr cPtr, bool cMemoryOwn)
	{
		this.swigCMemOwn = cMemoryOwn;
		this.swigCPtr = cPtr;
	}

	public Iterator() : this(AkSoundEnginePINVOKE.CSharp_new_Iterator(), true)
	{
	}

	internal static IntPtr getCPtr(Iterator obj)
	{
		return (obj == null) ? IntPtr.Zero : obj.swigCPtr;
	}

	~Iterator()
	{
		this.Dispose();
	}

	public virtual void Dispose()
	{
		lock (this)
		{
			if (this.swigCPtr != IntPtr.Zero)
			{
				if (this.swigCMemOwn)
				{
					this.swigCMemOwn = false;
					AkSoundEnginePINVOKE.CSharp_delete_Iterator(this.swigCPtr);
				}
				this.swigCPtr = IntPtr.Zero;
			}
			GC.SuppressFinalize(this);
		}
	}

	public Iterator NextIter()
	{
		return new Iterator(AkSoundEnginePINVOKE.CSharp_Iterator_NextIter(this.swigCPtr), false);
	}

	public Iterator PrevIter()
	{
		return new Iterator(AkSoundEnginePINVOKE.CSharp_Iterator_PrevIter(this.swigCPtr), false);
	}

	public PlaylistItem GetItem()
	{
		return new PlaylistItem(AkSoundEnginePINVOKE.CSharp_Iterator_GetItem(this.swigCPtr), false);
	}

	public bool IsEqualTo(Iterator in_rOp)
	{
		return AkSoundEnginePINVOKE.CSharp_Iterator_IsEqualTo(this.swigCPtr, Iterator.getCPtr(in_rOp));
	}

	public bool IsDifferentFrom(Iterator in_rOp)
	{
		return AkSoundEnginePINVOKE.CSharp_Iterator_IsDifferentFrom(this.swigCPtr, Iterator.getCPtr(in_rOp));
	}
}
