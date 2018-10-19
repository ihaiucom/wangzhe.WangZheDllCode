using System;

public class AkRamp : IDisposable
{
	private IntPtr swigCPtr;

	protected bool swigCMemOwn;

	public float fPrev
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkRamp_fPrev_get(this.swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkRamp_fPrev_set(this.swigCPtr, value);
		}
	}

	public float fNext
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkRamp_fNext_get(this.swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkRamp_fNext_set(this.swigCPtr, value);
		}
	}

	internal AkRamp(IntPtr cPtr, bool cMemoryOwn)
	{
		this.swigCMemOwn = cMemoryOwn;
		this.swigCPtr = cPtr;
	}

	public AkRamp() : this(AkSoundEnginePINVOKE.CSharp_new_AkRamp__SWIG_0(), true)
	{
	}

	public AkRamp(float in_fPrev, float in_fNext) : this(AkSoundEnginePINVOKE.CSharp_new_AkRamp__SWIG_1(in_fPrev, in_fNext), true)
	{
	}

	internal static IntPtr getCPtr(AkRamp obj)
	{
		return (obj != null) ? obj.swigCPtr : IntPtr.Zero;
	}

	~AkRamp()
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
					AkSoundEnginePINVOKE.CSharp_delete_AkRamp(this.swigCPtr);
				}
				this.swigCPtr = IntPtr.Zero;
			}
			GC.SuppressFinalize(this);
		}
	}
}
