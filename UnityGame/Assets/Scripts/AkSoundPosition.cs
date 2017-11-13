using System;

public class AkSoundPosition : IDisposable
{
	private IntPtr swigCPtr;

	protected bool swigCMemOwn;

	public AkVector Position
	{
		get
		{
			IntPtr intPtr = AkSoundEnginePINVOKE.CSharp_AkSoundPosition_Position_get(this.swigCPtr);
			return (intPtr == IntPtr.Zero) ? null : new AkVector(intPtr, false);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkSoundPosition_Position_set(this.swigCPtr, AkVector.getCPtr(value));
		}
	}

	public AkVector Orientation
	{
		get
		{
			IntPtr intPtr = AkSoundEnginePINVOKE.CSharp_AkSoundPosition_Orientation_get(this.swigCPtr);
			return (intPtr == IntPtr.Zero) ? null : new AkVector(intPtr, false);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkSoundPosition_Orientation_set(this.swigCPtr, AkVector.getCPtr(value));
		}
	}

	internal AkSoundPosition(IntPtr cPtr, bool cMemoryOwn)
	{
		this.swigCMemOwn = cMemoryOwn;
		this.swigCPtr = cPtr;
	}

	public AkSoundPosition() : this(AkSoundEnginePINVOKE.CSharp_new_AkSoundPosition(), true)
	{
	}

	internal static IntPtr getCPtr(AkSoundPosition obj)
	{
		return (obj == null) ? IntPtr.Zero : obj.swigCPtr;
	}

	~AkSoundPosition()
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
					AkSoundEnginePINVOKE.CSharp_delete_AkSoundPosition(this.swigCPtr);
				}
				this.swigCPtr = IntPtr.Zero;
			}
			GC.SuppressFinalize(this);
		}
	}
}
