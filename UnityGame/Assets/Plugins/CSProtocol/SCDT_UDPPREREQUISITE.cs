using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class SCDT_UDPPREREQUISITE : ProtocolObject
	{
		public uint dwTaskID;

		public byte bTaskState;

		public byte bPrerequisiteNum;

		public COMDT_PREREQUISITE_DETAIL[] astPrerequisiteInfo;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 1102;

		public SCDT_UDPPREREQUISITE()
		{
			this.astPrerequisiteInfo = new COMDT_PREREQUISITE_DETAIL[3];
			for (int i = 0; i < 3; i++)
			{
				this.astPrerequisiteInfo[i] = (COMDT_PREREQUISITE_DETAIL)ProtocolObjectPool.Get(COMDT_PREREQUISITE_DETAIL.CLASS_ID);
			}
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
			if (cutVer == 0u || SCDT_UDPPREREQUISITE.CURRVERSION < cutVer)
			{
				cutVer = SCDT_UDPPREREQUISITE.CURRVERSION;
			}
			if (SCDT_UDPPREREQUISITE.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwTaskID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bTaskState);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bPrerequisiteNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (3 < this.bPrerequisiteNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astPrerequisiteInfo.Length < (int)this.bPrerequisiteNum)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.bPrerequisiteNum; i++)
			{
				errorType = this.astPrerequisiteInfo[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || SCDT_UDPPREREQUISITE.CURRVERSION < cutVer)
			{
				cutVer = SCDT_UDPPREREQUISITE.CURRVERSION;
			}
			if (SCDT_UDPPREREQUISITE.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwTaskID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bTaskState);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bPrerequisiteNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (3 < this.bPrerequisiteNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.bPrerequisiteNum; i++)
			{
				errorType = this.astPrerequisiteInfo[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return SCDT_UDPPREREQUISITE.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwTaskID = 0u;
			this.bTaskState = 0;
			this.bPrerequisiteNum = 0;
			if (this.astPrerequisiteInfo != null)
			{
				for (int i = 0; i < this.astPrerequisiteInfo.Length; i++)
				{
					if (this.astPrerequisiteInfo[i] != null)
					{
						this.astPrerequisiteInfo[i].Release();
						this.astPrerequisiteInfo[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astPrerequisiteInfo != null)
			{
				for (int i = 0; i < this.astPrerequisiteInfo.Length; i++)
				{
					this.astPrerequisiteInfo[i] = (COMDT_PREREQUISITE_DETAIL)ProtocolObjectPool.Get(COMDT_PREREQUISITE_DETAIL.CLASS_ID);
				}
			}
		}
	}
}
