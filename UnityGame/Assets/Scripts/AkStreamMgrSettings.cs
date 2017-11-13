using System;

public class AkStreamMgrSettings : IDisposable
{
	private IntPtr swigCPtr;

	protected bool swigCMemOwn;

	public uint uMemorySize
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkStreamMgrSettings_uMemorySize_get(this.swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkStreamMgrSettings_uMemorySize_set(this.swigCPtr, value);
		}
	}

	internal AkStreamMgrSettings(IntPtr cPtr, bool cMemoryOwn)
	{
		this.swigCMemOwn = cMemoryOwn;
		this.swigCPtr = cPtr;
	}

	public AkStreamMgrSettings() : this(AkSoundEnginePINVOKE.CSharp_new_AkStreamMgrSettings(), true)
	{
	}

	internal static IntPtr getCPtr(AkStreamMgrSettings obj)
	{
		return (obj == null) ? IntPtr.Zero : obj.swigCPtr;
	}

	~AkStreamMgrSettings()
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
					AkSoundEnginePINVOKE.CSharp_delete_AkStreamMgrSettings(this.swigCPtr);
				}
				this.swigCPtr = IntPtr.Zero;
			}
			GC.SuppressFinalize(this);
		}
	}
}
