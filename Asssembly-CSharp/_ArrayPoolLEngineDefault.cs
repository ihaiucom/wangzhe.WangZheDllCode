using System;

public class _ArrayPoolLEngineDefault : IDisposable
{
	private IntPtr swigCPtr;

	protected bool swigCMemOwn;

	internal _ArrayPoolLEngineDefault(IntPtr cPtr, bool cMemoryOwn)
	{
		this.swigCMemOwn = cMemoryOwn;
		this.swigCPtr = cPtr;
	}

	public _ArrayPoolLEngineDefault() : this(AkSoundEnginePINVOKE.CSharp_new__ArrayPoolLEngineDefault(), true)
	{
	}

	internal static IntPtr getCPtr(_ArrayPoolLEngineDefault obj)
	{
		return (obj == null) ? IntPtr.Zero : obj.swigCPtr;
	}

	~_ArrayPoolLEngineDefault()
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
					AkSoundEnginePINVOKE.CSharp_delete__ArrayPoolLEngineDefault(this.swigCPtr);
				}
				this.swigCPtr = IntPtr.Zero;
			}
			GC.SuppressFinalize(this);
		}
	}

	public static int Get()
	{
		return AkSoundEnginePINVOKE.CSharp__ArrayPoolLEngineDefault_Get();
	}
}
