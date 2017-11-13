using System;

public class AkDeviceSettings : IDisposable
{
	private IntPtr swigCPtr;

	protected bool swigCMemOwn;

	public IntPtr pIOMemory
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_pIOMemory_get(this.swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_pIOMemory_set(this.swigCPtr, value);
		}
	}

	public uint uIOMemorySize
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_uIOMemorySize_get(this.swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_uIOMemorySize_set(this.swigCPtr, value);
		}
	}

	public uint uIOMemoryAlignment
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_uIOMemoryAlignment_get(this.swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_uIOMemoryAlignment_set(this.swigCPtr, value);
		}
	}

	public int ePoolAttributes
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_ePoolAttributes_get(this.swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_ePoolAttributes_set(this.swigCPtr, value);
		}
	}

	public uint uGranularity
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_uGranularity_get(this.swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_uGranularity_set(this.swigCPtr, value);
		}
	}

	public uint uSchedulerTypeFlags
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_uSchedulerTypeFlags_get(this.swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_uSchedulerTypeFlags_set(this.swigCPtr, value);
		}
	}

	public AkThreadProperties threadProperties
	{
		get
		{
			IntPtr intPtr = AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_threadProperties_get(this.swigCPtr);
			return (intPtr == IntPtr.Zero) ? null : new AkThreadProperties(intPtr, false);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_threadProperties_set(this.swigCPtr, AkThreadProperties.getCPtr(value));
		}
	}

	public float fTargetAutoStmBufferLength
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_fTargetAutoStmBufferLength_get(this.swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_fTargetAutoStmBufferLength_set(this.swigCPtr, value);
		}
	}

	public uint uMaxConcurrentIO
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_uMaxConcurrentIO_get(this.swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_uMaxConcurrentIO_set(this.swigCPtr, value);
		}
	}

	public float fMaxCacheRatio
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_fMaxCacheRatio_get(this.swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_fMaxCacheRatio_set(this.swigCPtr, value);
		}
	}

	public uint uMaxCachePinnedBytes
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_uMaxCachePinnedBytes_get(this.swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkDeviceSettings_uMaxCachePinnedBytes_set(this.swigCPtr, value);
		}
	}

	internal AkDeviceSettings(IntPtr cPtr, bool cMemoryOwn)
	{
		this.swigCMemOwn = cMemoryOwn;
		this.swigCPtr = cPtr;
	}

	public AkDeviceSettings() : this(AkSoundEnginePINVOKE.CSharp_new_AkDeviceSettings(), true)
	{
	}

	internal static IntPtr getCPtr(AkDeviceSettings obj)
	{
		return (obj == null) ? IntPtr.Zero : obj.swigCPtr;
	}

	~AkDeviceSettings()
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
					AkSoundEnginePINVOKE.CSharp_delete_AkDeviceSettings(this.swigCPtr);
				}
				this.swigCPtr = IntPtr.Zero;
			}
			GC.SuppressFinalize(this);
		}
	}
}
