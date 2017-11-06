using System;

public class WwiseObjectIDext : IDisposable
{
	private IntPtr swigCPtr;

	protected bool swigCMemOwn;

	public uint id
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_WwiseObjectIDext_id_get(this.swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_WwiseObjectIDext_id_set(this.swigCPtr, value);
		}
	}

	public bool bIsBus
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_WwiseObjectIDext_bIsBus_get(this.swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_WwiseObjectIDext_bIsBus_set(this.swigCPtr, value);
		}
	}

	internal WwiseObjectIDext(IntPtr cPtr, bool cMemoryOwn)
	{
		this.swigCMemOwn = cMemoryOwn;
		this.swigCPtr = cPtr;
	}

	public WwiseObjectIDext() : this(AkSoundEnginePINVOKE.CSharp_new_WwiseObjectIDext(), true)
	{
	}

	internal static IntPtr getCPtr(WwiseObjectIDext obj)
	{
		return (obj == null) ? IntPtr.Zero : obj.swigCPtr;
	}

	~WwiseObjectIDext()
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
					AkSoundEnginePINVOKE.CSharp_delete_WwiseObjectIDext(this.swigCPtr);
				}
				this.swigCPtr = IntPtr.Zero;
			}
			GC.SuppressFinalize(this);
		}
	}

	public bool IsEqualTo(WwiseObjectIDext in_rOther)
	{
		return AkSoundEnginePINVOKE.CSharp_WwiseObjectIDext_IsEqualTo(this.swigCPtr, WwiseObjectIDext.getCPtr(in_rOther));
	}

	public AkNodeType GetNodeType()
	{
		return (AkNodeType)AkSoundEnginePINVOKE.CSharp_WwiseObjectIDext_GetNodeType(this.swigCPtr);
	}
}
