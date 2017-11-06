using System;

public class AkMusicSettings : IDisposable
{
	private IntPtr swigCPtr;

	protected bool swigCMemOwn;

	public float fStreamingLookAheadRatio
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkMusicSettings_fStreamingLookAheadRatio_get(this.swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkMusicSettings_fStreamingLookAheadRatio_set(this.swigCPtr, value);
		}
	}

	internal AkMusicSettings(IntPtr cPtr, bool cMemoryOwn)
	{
		this.swigCMemOwn = cMemoryOwn;
		this.swigCPtr = cPtr;
	}

	public AkMusicSettings() : this(AkSoundEnginePINVOKE.CSharp_new_AkMusicSettings(), true)
	{
	}

	internal static IntPtr getCPtr(AkMusicSettings obj)
	{
		return (obj == null) ? IntPtr.Zero : obj.swigCPtr;
	}

	~AkMusicSettings()
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
					AkSoundEnginePINVOKE.CSharp_delete_AkMusicSettings(this.swigCPtr);
				}
				this.swigCPtr = IntPtr.Zero;
			}
			GC.SuppressFinalize(this);
		}
	}
}
