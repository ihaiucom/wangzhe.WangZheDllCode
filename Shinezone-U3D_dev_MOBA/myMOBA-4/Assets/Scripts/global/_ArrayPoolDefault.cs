using System;

public class _ArrayPoolDefault : IDisposable
{
	private IntPtr swigCPtr;

	protected bool swigCMemOwn;

	internal _ArrayPoolDefault(IntPtr cPtr, bool cMemoryOwn)
	{
		this.swigCMemOwn = cMemoryOwn;
		this.swigCPtr = cPtr;
	}

	public _ArrayPoolDefault() : this(AkSoundEnginePINVOKE.CSharp_new__ArrayPoolDefault(), true)
	{
	}

	internal static IntPtr getCPtr(_ArrayPoolDefault obj)
	{
		return (obj != null) ? obj.swigCPtr : IntPtr.Zero;
	}

	~_ArrayPoolDefault()
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
					AkSoundEnginePINVOKE.CSharp_delete__ArrayPoolDefault(this.swigCPtr);
				}
				this.swigCPtr = IntPtr.Zero;
			}
			GC.SuppressFinalize(this);
		}
	}

	public static int Get()
	{
		return AkSoundEnginePINVOKE.CSharp__ArrayPoolDefault_Get();
	}
}
