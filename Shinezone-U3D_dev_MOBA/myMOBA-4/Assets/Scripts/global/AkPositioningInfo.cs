using System;

public class AkPositioningInfo : IDisposable
{
	private IntPtr swigCPtr;

	protected bool swigCMemOwn;

	public float fCenterPct
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_fCenterPct_get(this.swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_fCenterPct_set(this.swigCPtr, value);
		}
	}

	public AkPannerType pannerType
	{
		get
		{
			return (AkPannerType)AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_pannerType_get(this.swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_pannerType_set(this.swigCPtr, (int)value);
		}
	}

	public AkPositionSourceType posSourceType
	{
		get
		{
			return (AkPositionSourceType)AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_posSourceType_get(this.swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_posSourceType_set(this.swigCPtr, (int)value);
		}
	}

	public bool bUpdateEachFrame
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_bUpdateEachFrame_get(this.swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_bUpdateEachFrame_set(this.swigCPtr, value);
		}
	}

	public bool bUseSpatialization
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_bUseSpatialization_get(this.swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_bUseSpatialization_set(this.swigCPtr, value);
		}
	}

	public bool bUseAttenuation
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_bUseAttenuation_get(this.swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_bUseAttenuation_set(this.swigCPtr, value);
		}
	}

	public bool bUseConeAttenuation
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_bUseConeAttenuation_get(this.swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_bUseConeAttenuation_set(this.swigCPtr, value);
		}
	}

	public float fInnerAngle
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_fInnerAngle_get(this.swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_fInnerAngle_set(this.swigCPtr, value);
		}
	}

	public float fOuterAngle
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_fOuterAngle_get(this.swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_fOuterAngle_set(this.swigCPtr, value);
		}
	}

	public float fConeMaxAttenuation
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_fConeMaxAttenuation_get(this.swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_fConeMaxAttenuation_set(this.swigCPtr, value);
		}
	}

	public float LPFCone
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_LPFCone_get(this.swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_LPFCone_set(this.swigCPtr, value);
		}
	}

	public float HPFCone
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_HPFCone_get(this.swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_HPFCone_set(this.swigCPtr, value);
		}
	}

	public float fMaxDistance
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_fMaxDistance_get(this.swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_fMaxDistance_set(this.swigCPtr, value);
		}
	}

	public float fVolDryAtMaxDist
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_fVolDryAtMaxDist_get(this.swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_fVolDryAtMaxDist_set(this.swigCPtr, value);
		}
	}

	public float fVolAuxGameDefAtMaxDist
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_fVolAuxGameDefAtMaxDist_get(this.swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_fVolAuxGameDefAtMaxDist_set(this.swigCPtr, value);
		}
	}

	public float fVolAuxUserDefAtMaxDist
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_fVolAuxUserDefAtMaxDist_get(this.swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_fVolAuxUserDefAtMaxDist_set(this.swigCPtr, value);
		}
	}

	public float LPFValueAtMaxDist
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_LPFValueAtMaxDist_get(this.swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_LPFValueAtMaxDist_set(this.swigCPtr, value);
		}
	}

	public float HPFValueAtMaxDist
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_HPFValueAtMaxDist_get(this.swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkPositioningInfo_HPFValueAtMaxDist_set(this.swigCPtr, value);
		}
	}

	internal AkPositioningInfo(IntPtr cPtr, bool cMemoryOwn)
	{
		this.swigCMemOwn = cMemoryOwn;
		this.swigCPtr = cPtr;
	}

	public AkPositioningInfo() : this(AkSoundEnginePINVOKE.CSharp_new_AkPositioningInfo(), true)
	{
	}

	internal static IntPtr getCPtr(AkPositioningInfo obj)
	{
		return (obj != null) ? obj.swigCPtr : IntPtr.Zero;
	}

	~AkPositioningInfo()
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
					AkSoundEnginePINVOKE.CSharp_delete_AkPositioningInfo(this.swigCPtr);
				}
				this.swigCPtr = IntPtr.Zero;
			}
			GC.SuppressFinalize(this);
		}
	}
}
