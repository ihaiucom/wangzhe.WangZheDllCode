using System;

public class WwiseObjectID : WwiseObjectIDext
{
	private IntPtr swigCPtr;

	internal WwiseObjectID(IntPtr cPtr, bool cMemoryOwn) : base(AkSoundEnginePINVOKE.CSharp_WwiseObjectID_SWIGUpcast(cPtr), cMemoryOwn)
	{
		this.swigCPtr = cPtr;
	}

	public WwiseObjectID() : this(AkSoundEnginePINVOKE.CSharp_new_WwiseObjectID__SWIG_0(), true)
	{
	}

	public WwiseObjectID(uint in_ID) : this(AkSoundEnginePINVOKE.CSharp_new_WwiseObjectID__SWIG_1(in_ID), true)
	{
	}

	public WwiseObjectID(uint in_ID, bool in_bIsBus) : this(AkSoundEnginePINVOKE.CSharp_new_WwiseObjectID__SWIG_2(in_ID, in_bIsBus), true)
	{
	}

	public WwiseObjectID(uint in_ID, AkNodeType in_eNodeType) : this(AkSoundEnginePINVOKE.CSharp_new_WwiseObjectID__SWIG_3(in_ID, (int)in_eNodeType), true)
	{
	}

	internal static IntPtr getCPtr(WwiseObjectID obj)
	{
		return (obj == null) ? IntPtr.Zero : obj.swigCPtr;
	}

	~WwiseObjectID()
	{
		this.Dispose();
	}

	public override void Dispose()
	{
		lock (this)
		{
			if (this.swigCPtr != IntPtr.Zero)
			{
				if (this.swigCMemOwn)
				{
					this.swigCMemOwn = false;
					AkSoundEnginePINVOKE.CSharp_delete_WwiseObjectID(this.swigCPtr);
				}
				this.swigCPtr = IntPtr.Zero;
			}
			GC.SuppressFinalize(this);
			base.Dispose();
		}
	}
}
