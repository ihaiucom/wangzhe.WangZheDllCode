using System;

public class AkAuxSendValue : IDisposable
{
	private IntPtr swigCPtr;

	protected bool swigCMemOwn;

	public uint auxBusID
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkAuxSendValue_auxBusID_get(this.swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkAuxSendValue_auxBusID_set(this.swigCPtr, value);
		}
	}

	public float fControlValue
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkAuxSendValue_fControlValue_get(this.swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkAuxSendValue_fControlValue_set(this.swigCPtr, value);
		}
	}

	internal AkAuxSendValue(IntPtr cPtr, bool cMemoryOwn)
	{
		this.swigCMemOwn = cMemoryOwn;
		this.swigCPtr = cPtr;
	}

	internal static IntPtr getCPtr(AkAuxSendValue obj)
	{
		return (obj != null) ? obj.swigCPtr : IntPtr.Zero;
	}

	~AkAuxSendValue()
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
					AkSoundEnginePINVOKE.CSharp_delete_AkAuxSendValue(this.swigCPtr);
				}
				this.swigCPtr = IntPtr.Zero;
			}
			GC.SuppressFinalize(this);
		}
	}
}
