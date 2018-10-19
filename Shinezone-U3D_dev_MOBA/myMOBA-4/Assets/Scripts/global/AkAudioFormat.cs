using System;

public class AkAudioFormat : IDisposable
{
	private IntPtr swigCPtr;

	protected bool swigCMemOwn;

	public uint uSampleRate
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkAudioFormat_uSampleRate_get(this.swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkAudioFormat_uSampleRate_set(this.swigCPtr, value);
		}
	}

	public AkChannelConfig channelConfig
	{
		get
		{
			IntPtr intPtr = AkSoundEnginePINVOKE.CSharp_AkAudioFormat_channelConfig_get(this.swigCPtr);
			return (!(intPtr == IntPtr.Zero)) ? new AkChannelConfig(intPtr, false) : null;
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkAudioFormat_channelConfig_set(this.swigCPtr, AkChannelConfig.getCPtr(value));
		}
	}

	public uint uBitsPerSample
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkAudioFormat_uBitsPerSample_get(this.swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkAudioFormat_uBitsPerSample_set(this.swigCPtr, value);
		}
	}

	public uint uBlockAlign
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkAudioFormat_uBlockAlign_get(this.swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkAudioFormat_uBlockAlign_set(this.swigCPtr, value);
		}
	}

	public uint uTypeID
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkAudioFormat_uTypeID_get(this.swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkAudioFormat_uTypeID_set(this.swigCPtr, value);
		}
	}

	public uint uInterleaveID
	{
		get
		{
			return AkSoundEnginePINVOKE.CSharp_AkAudioFormat_uInterleaveID_get(this.swigCPtr);
		}
		set
		{
			AkSoundEnginePINVOKE.CSharp_AkAudioFormat_uInterleaveID_set(this.swigCPtr, value);
		}
	}

	internal AkAudioFormat(IntPtr cPtr, bool cMemoryOwn)
	{
		this.swigCMemOwn = cMemoryOwn;
		this.swigCPtr = cPtr;
	}

	public AkAudioFormat() : this(AkSoundEnginePINVOKE.CSharp_new_AkAudioFormat(), true)
	{
	}

	internal static IntPtr getCPtr(AkAudioFormat obj)
	{
		return (obj != null) ? obj.swigCPtr : IntPtr.Zero;
	}

	~AkAudioFormat()
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
					AkSoundEnginePINVOKE.CSharp_delete_AkAudioFormat(this.swigCPtr);
				}
				this.swigCPtr = IntPtr.Zero;
			}
			GC.SuppressFinalize(this);
		}
	}

	public uint GetNumChannels()
	{
		return AkSoundEnginePINVOKE.CSharp_AkAudioFormat_GetNumChannels(this.swigCPtr);
	}

	public uint GetBitsPerSample()
	{
		return AkSoundEnginePINVOKE.CSharp_AkAudioFormat_GetBitsPerSample(this.swigCPtr);
	}

	public uint GetBlockAlign()
	{
		return AkSoundEnginePINVOKE.CSharp_AkAudioFormat_GetBlockAlign(this.swigCPtr);
	}

	public uint GetTypeID()
	{
		return AkSoundEnginePINVOKE.CSharp_AkAudioFormat_GetTypeID(this.swigCPtr);
	}

	public uint GetInterleaveID()
	{
		return AkSoundEnginePINVOKE.CSharp_AkAudioFormat_GetInterleaveID(this.swigCPtr);
	}

	public void SetAll(uint in_uSampleRate, AkChannelConfig in_channelConfig, uint in_uBitsPerSample, uint in_uBlockAlign, uint in_uTypeID, uint in_uInterleaveID)
	{
		AkSoundEnginePINVOKE.CSharp_AkAudioFormat_SetAll(this.swigCPtr, in_uSampleRate, AkChannelConfig.getCPtr(in_channelConfig), in_uBitsPerSample, in_uBlockAlign, in_uTypeID, in_uInterleaveID);
	}

	public bool IsChannelConfigSupported()
	{
		return AkSoundEnginePINVOKE.CSharp_AkAudioFormat_IsChannelConfigSupported(this.swigCPtr);
	}
}
