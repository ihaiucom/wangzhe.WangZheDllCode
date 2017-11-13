using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_TALENTARRAY : ProtocolObject
	{
		public byte bWakeState;

		public COMDT_TALENTINFO[] astTalentInfo;

		public COMDT_HEROWAKE_STEP stWakeStep;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 35u;

		public static readonly uint VERSION_stWakeStep = 35u;

		public static readonly int CLASS_ID = 108;

		public COMDT_TALENTARRAY()
		{
			this.astTalentInfo = new COMDT_TALENTINFO[20];
			for (int i = 0; i < 20; i++)
			{
				this.astTalentInfo[i] = (COMDT_TALENTINFO)ProtocolObjectPool.Get(COMDT_TALENTINFO.CLASS_ID);
			}
			this.stWakeStep = (COMDT_HEROWAKE_STEP)ProtocolObjectPool.Get(COMDT_HEROWAKE_STEP.CLASS_ID);
		}

		public override TdrError.ErrorType construct()
		{
			return TdrError.ErrorType.TDR_NO_ERROR;
		}

		public TdrError.ErrorType pack(ref byte[] buffer, int size, ref int usedSize, uint cutVer)
		{
			if (buffer == null || buffer.GetLength(0) == 0 || size > buffer.GetLength(0))
			{
				return TdrError.ErrorType.TDR_ERR_INVALID_BUFFER_PARAMETER;
			}
			TdrWriteBuf tdrWriteBuf = ClassObjPool<TdrWriteBuf>.Get();
			tdrWriteBuf.set(ref buffer, size);
			TdrError.ErrorType errorType = this.pack(ref tdrWriteBuf, cutVer);
			if (errorType == TdrError.ErrorType.TDR_NO_ERROR)
			{
				buffer = tdrWriteBuf.getBeginPtr();
				usedSize = tdrWriteBuf.getUsedSize();
			}
			tdrWriteBuf.Release();
			return errorType;
		}

		public override TdrError.ErrorType pack(ref TdrWriteBuf destBuf, uint cutVer)
		{
			if (cutVer == 0u || COMDT_TALENTARRAY.CURRVERSION < cutVer)
			{
				cutVer = COMDT_TALENTARRAY.CURRVERSION;
			}
			if (COMDT_TALENTARRAY.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bWakeState);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int i = 0; i < 20; i++)
			{
				errorType = this.astTalentInfo[i].pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_TALENTARRAY.VERSION_stWakeStep <= cutVer)
			{
				errorType = this.stWakeStep.pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public TdrError.ErrorType unpack(ref byte[] buffer, int size, ref int usedSize, uint cutVer)
		{
			if (buffer == null || buffer.GetLength(0) == 0 || size > buffer.GetLength(0))
			{
				return TdrError.ErrorType.TDR_ERR_INVALID_BUFFER_PARAMETER;
			}
			TdrReadBuf tdrReadBuf = ClassObjPool<TdrReadBuf>.Get();
			tdrReadBuf.set(ref buffer, size);
			TdrError.ErrorType result = this.unpack(ref tdrReadBuf, cutVer);
			usedSize = tdrReadBuf.getUsedSize();
			tdrReadBuf.Release();
			return result;
		}

		public override TdrError.ErrorType unpack(ref TdrReadBuf srcBuf, uint cutVer)
		{
			if (cutVer == 0u || COMDT_TALENTARRAY.CURRVERSION < cutVer)
			{
				cutVer = COMDT_TALENTARRAY.CURRVERSION;
			}
			if (COMDT_TALENTARRAY.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bWakeState);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			for (int i = 0; i < 20; i++)
			{
				errorType = this.astTalentInfo[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			if (COMDT_TALENTARRAY.VERSION_stWakeStep <= cutVer)
			{
				errorType = this.stWakeStep.unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			else
			{
				errorType = this.stWakeStep.construct();
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_TALENTARRAY.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bWakeState = 0;
			if (this.astTalentInfo != null)
			{
				for (int i = 0; i < this.astTalentInfo.Length; i++)
				{
					if (this.astTalentInfo[i] != null)
					{
						this.astTalentInfo[i].Release();
						this.astTalentInfo[i] = null;
					}
				}
			}
			if (this.stWakeStep != null)
			{
				this.stWakeStep.Release();
				this.stWakeStep = null;
			}
		}

		public override void OnUse()
		{
			if (this.astTalentInfo != null)
			{
				for (int i = 0; i < this.astTalentInfo.Length; i++)
				{
					this.astTalentInfo[i] = (COMDT_TALENTINFO)ProtocolObjectPool.Get(COMDT_TALENTINFO.CLASS_ID);
				}
			}
			this.stWakeStep = (COMDT_HEROWAKE_STEP)ProtocolObjectPool.Get(COMDT_HEROWAKE_STEP.CLASS_ID);
		}
	}
}
