using System;

public class EnvelopePoint : IDisposable
{
	private IntPtr swigCPtr;

	protected bool swigCMemOwn;

	public uint uPosition
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_EnvelopePoint_uPosition_get(this.swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_EnvelopePoint_uPosition_set(this.swigCPtr, value);
		}
	}

	public ushort uAttenuation
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_EnvelopePoint_uAttenuation_get(this.swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_EnvelopePoint_uAttenuation_set(this.swigCPtr, value);
		}
	}

	internal EnvelopePoint(IntPtr cPtr, bool cMemoryOwn)
	{
		this.swigCMemOwn = cMemoryOwn;
		this.swigCPtr = cPtr;
	}

	public EnvelopePoint() : this(AkSoundEnginePINVOKE.CSharp_new_EnvelopePoint(), true)
	{
	}

	internal static IntPtr getCPtr(EnvelopePoint obj)
	{
		return (obj == null) ? IntPtr.Zero : obj.swigCPtr;
	}

	~EnvelopePoint()
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
					AkSoundEnginePINVOKE.CSharp_delete_EnvelopePoint(this.swigCPtr);
				}
				this.swigCPtr = IntPtr.Zero;
			}
			GC.SuppressFinalize(this);
		}
	}
}
