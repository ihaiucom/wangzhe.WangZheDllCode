using System;

public class AkOutputSettings : IDisposable
{
	private IntPtr swigCPtr;

	protected bool swigCMemOwn;

	public AkPanningRule ePanningRule
	{
		get
		{
			return (AkPanningRule)AkSoundEnginePINVOKE.CSharp_AkOutputSettings_ePanningRule_get(this.swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkOutputSettings_ePanningRule_set(this.swigCPtr, (int)value);
		}
	}

	public AkChannelConfig channelConfig
	{
		get
		{
			IntPtr intPtr = AkSoundEnginePINVOKE.CSharp_AkOutputSettings_channelConfig_get(this.swigCPtr);
			return (intPtr == IntPtr.Zero) ? null : new AkChannelConfig(intPtr, false);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkOutputSettings_channelConfig_set(this.swigCPtr, AkChannelConfig.getCPtr(value));
		}
	}

	internal AkOutputSettings(IntPtr cPtr, bool cMemoryOwn)
	{
		this.swigCMemOwn = cMemoryOwn;
		this.swigCPtr = cPtr;
	}

	public AkOutputSettings() : this(AkSoundEnginePINVOKE.CSharp_new_AkOutputSettings(), true)
	{
	}

	internal static IntPtr getCPtr(AkOutputSettings obj)
	{
		return (obj == null) ? IntPtr.Zero : obj.swigCPtr;
	}

	~AkOutputSettings()
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
					AkSoundEnginePINVOKE.CSharp_delete_AkOutputSettings(this.swigCPtr);
				}
				this.swigCPtr = IntPtr.Zero;
			}
			GC.SuppressFinalize(this);
		}
	}
}
