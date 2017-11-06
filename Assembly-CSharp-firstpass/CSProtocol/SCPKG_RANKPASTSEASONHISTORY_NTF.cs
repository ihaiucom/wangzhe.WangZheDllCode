using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class SCPKG_RANKPASTSEASONHISTORY_NTF : ProtocolObject
	{
		public byte bNum;

		public COMDT_RANK_PASTSEASON_FIGHT_RECORD[] astRecord;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 241u;

		public static readonly int CLASS_ID = 1387;

		public SCPKG_RANKPASTSEASONHISTORY_NTF()
		{
			this.astRecord = new COMDT_RANK_PASTSEASON_FIGHT_RECORD[10];
			for (int i = 0; i < 10; i++)
			{
				this.astRecord[i] = (COMDT_RANK_PASTSEASON_FIGHT_RECORD)ProtocolObjectPool.Get(COMDT_RANK_PASTSEASON_FIGHT_RECORD.CLASS_ID);
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
			if (cutVer == 0u || SCPKG_RANKPASTSEASONHISTORY_NTF.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_RANKPASTSEASONHISTORY_NTF.CURRVERSION;
			}
			if (SCPKG_RANKPASTSEASONHISTORY_NTF.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (10 < this.bNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astRecord.Length < (int)this.bNum)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.bNum; i++)
			{
				errorType = this.astRecord[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || SCPKG_RANKPASTSEASONHISTORY_NTF.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_RANKPASTSEASONHISTORY_NTF.CURRVERSION;
			}
			if (SCPKG_RANKPASTSEASONHISTORY_NTF.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (10 < this.bNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.bNum; i++)
			{
				errorType = this.astRecord[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return SCPKG_RANKPASTSEASONHISTORY_NTF.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bNum = 0;
			if (this.astRecord != null)
			{
				for (int i = 0; i < this.astRecord.Length; i++)
				{
					if (this.astRecord[i] != null)
					{
						this.astRecord[i].Release();
						this.astRecord[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astRecord != null)
			{
				for (int i = 0; i < this.astRecord.Length; i++)
				{
					this.astRecord[i] = (COMDT_RANK_PASTSEASON_FIGHT_RECORD)ProtocolObjectPool.Get(COMDT_RANK_PASTSEASON_FIGHT_RECORD.CLASS_ID);
				}
			}
		}
	}
}
