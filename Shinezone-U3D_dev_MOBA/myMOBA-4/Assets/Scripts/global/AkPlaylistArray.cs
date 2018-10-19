using System;

public class AkPlaylistArray : IDisposable
{
	private IntPtr swigCPtr;

	protected bool swigCMemOwn;

	internal AkPlaylistArray(IntPtr cPtr, bool cMemoryOwn)
	{
		this.swigCMemOwn = cMemoryOwn;
		this.swigCPtr = cPtr;
	}

	public AkPlaylistArray() : this(AkSoundEnginePINVOKE.CSharp_new_AkPlaylistArray(), true)
	{
	}

	internal static IntPtr getCPtr(AkPlaylistArray obj)
	{
		return (obj != null) ? obj.swigCPtr : IntPtr.Zero;
	}

	~AkPlaylistArray()
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
					AkSoundEnginePINVOKE.CSharp_delete_AkPlaylistArray(this.swigCPtr);
				}
				this.swigCPtr = IntPtr.Zero;
			}
			GC.SuppressFinalize(this);
		}
	}

	public Iterator Begin()
	{
		return new Iterator(AkSoundEnginePINVOKE.CSharp_AkPlaylistArray_Begin(this.swigCPtr), true);
	}

	public Iterator End()
	{
		return new Iterator(AkSoundEnginePINVOKE.CSharp_AkPlaylistArray_End(this.swigCPtr), true);
	}

	public Iterator FindEx(PlaylistItem in_Item)
	{
		return new Iterator(AkSoundEnginePINVOKE.CSharp_AkPlaylistArray_FindEx(this.swigCPtr, PlaylistItem.getCPtr(in_Item)), true);
	}

	public Iterator Erase(Iterator in_rIter)
	{
		return new Iterator(AkSoundEnginePINVOKE.CSharp_AkPlaylistArray_Erase__SWIG_0(this.swigCPtr, Iterator.getCPtr(in_rIter)), true);
	}

	public void Erase(uint in_uIndex)
	{
		AkSoundEnginePINVOKE.CSharp_AkPlaylistArray_Erase__SWIG_1(this.swigCPtr, in_uIndex);
	}

	public Iterator EraseSwap(Iterator in_rIter)
	{
		return new Iterator(AkSoundEnginePINVOKE.CSharp_AkPlaylistArray_EraseSwap(this.swigCPtr, Iterator.getCPtr(in_rIter)), true);
	}

	public AKRESULT Reserve(uint in_ulReserve)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_AkPlaylistArray_Reserve(this.swigCPtr, in_ulReserve);
	}

	public uint Reserved()
	{
		return AkSoundEnginePINVOKE.CSharp_AkPlaylistArray_Reserved(this.swigCPtr);
	}

	public void Term()
	{
		AkSoundEnginePINVOKE.CSharp_AkPlaylistArray_Term(this.swigCPtr);
	}

	public uint Length()
	{
		return AkSoundEnginePINVOKE.CSharp_AkPlaylistArray_Length(this.swigCPtr);
	}

	public bool IsEmpty()
	{
		return AkSoundEnginePINVOKE.CSharp_AkPlaylistArray_IsEmpty(this.swigCPtr);
	}

	public PlaylistItem Exists(PlaylistItem in_Item)
	{
		IntPtr intPtr = AkSoundEnginePINVOKE.CSharp_AkPlaylistArray_Exists(this.swigCPtr, PlaylistItem.getCPtr(in_Item));
		return (!(intPtr == IntPtr.Zero)) ? new PlaylistItem(intPtr, false) : null;
	}

	public PlaylistItem AddLast()
	{
		IntPtr intPtr = AkSoundEnginePINVOKE.CSharp_AkPlaylistArray_AddLast__SWIG_0(this.swigCPtr);
		return (!(intPtr == IntPtr.Zero)) ? new PlaylistItem(intPtr, false) : null;
	}

	public PlaylistItem AddLast(PlaylistItem in_rItem)
	{
		IntPtr intPtr = AkSoundEnginePINVOKE.CSharp_AkPlaylistArray_AddLast__SWIG_1(this.swigCPtr, PlaylistItem.getCPtr(in_rItem));
		return (!(intPtr == IntPtr.Zero)) ? new PlaylistItem(intPtr, false) : null;
	}

	public PlaylistItem Last()
	{
		return new PlaylistItem(AkSoundEnginePINVOKE.CSharp_AkPlaylistArray_Last(this.swigCPtr), false);
	}

	public void RemoveLast()
	{
		AkSoundEnginePINVOKE.CSharp_AkPlaylistArray_RemoveLast(this.swigCPtr);
	}

	public AKRESULT Remove(PlaylistItem in_rItem)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_AkPlaylistArray_Remove(this.swigCPtr, PlaylistItem.getCPtr(in_rItem));
	}

	public AKRESULT RemoveSwap(PlaylistItem in_rItem)
	{
		return (AKRESULT)AkSoundEnginePINVOKE.CSharp_AkPlaylistArray_RemoveSwap(this.swigCPtr, PlaylistItem.getCPtr(in_rItem));
	}

	public void RemoveAll()
	{
		AkSoundEnginePINVOKE.CSharp_AkPlaylistArray_RemoveAll(this.swigCPtr);
	}

	public PlaylistItem ItemAtIndex(uint uiIndex)
	{
		return new PlaylistItem(AkSoundEnginePINVOKE.CSharp_AkPlaylistArray_ItemAtIndex(this.swigCPtr, uiIndex), false);
	}

	public PlaylistItem Insert(uint in_uIndex)
	{
		IntPtr intPtr = AkSoundEnginePINVOKE.CSharp_AkPlaylistArray_Insert(this.swigCPtr, in_uIndex);
		return (!(intPtr == IntPtr.Zero)) ? new PlaylistItem(intPtr, false) : null;
	}

	public bool GrowArray(uint in_uGrowBy)
	{
		return AkSoundEnginePINVOKE.CSharp_AkPlaylistArray_GrowArray__SWIG_0(this.swigCPtr, in_uGrowBy);
	}

	public bool GrowArray()
	{
		return AkSoundEnginePINVOKE.CSharp_AkPlaylistArray_GrowArray__SWIG_1(this.swigCPtr);
	}

	public bool Resize(uint in_uiSize)
	{
		return AkSoundEnginePINVOKE.CSharp_AkPlaylistArray_Resize(this.swigCPtr, in_uiSize);
	}

	public void Transfer(AkPlaylistArray in_rSource)
	{
		AkSoundEnginePINVOKE.CSharp_AkPlaylistArray_Transfer(this.swigCPtr, AkPlaylistArray.getCPtr(in_rSource));
	}
}
