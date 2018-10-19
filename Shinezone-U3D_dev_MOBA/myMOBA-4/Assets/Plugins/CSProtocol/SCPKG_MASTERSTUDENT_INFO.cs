using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class SCPKG_MASTERSTUDENT_INFO : ProtocolObject
	{
		public byte bStudentType;

		public CSDT_FRIEND_INFO stMaster;

		public byte bStudentNum;

		public CSDT_FRIEND_INFO[] astStudentList;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 234u;

		public static readonly int CLASS_ID = 1627;

		public SCPKG_MASTERSTUDENT_INFO()
		{
			this.stMaster = (CSDT_FRIEND_INFO)ProtocolObjectPool.Get(CSDT_FRIEND_INFO.CLASS_ID);
			this.astStudentList = new CSDT_FRIEND_INFO[6];
			for (int i = 0; i < 6; i++)
			{
				this.astStudentList[i] = (CSDT_FRIEND_INFO)ProtocolObjectPool.Get(CSDT_FRIEND_INFO.CLASS_ID);
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
			if (cutVer == 0u || SCPKG_MASTERSTUDENT_INFO.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_MASTERSTUDENT_INFO.CURRVERSION;
			}
			if (SCPKG_MASTERSTUDENT_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bStudentType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stMaster.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bStudentNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (6 < this.bStudentNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astStudentList.Length < (int)this.bStudentNum)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.bStudentNum; i++)
			{
				errorType = this.astStudentList[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || SCPKG_MASTERSTUDENT_INFO.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_MASTERSTUDENT_INFO.CURRVERSION;
			}
			if (SCPKG_MASTERSTUDENT_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bStudentType);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = this.stMaster.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bStudentNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (6 < this.bStudentNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.bStudentNum; i++)
			{
				errorType = this.astStudentList[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return SCPKG_MASTERSTUDENT_INFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bStudentType = 0;
			if (this.stMaster != null)
			{
				this.stMaster.Release();
				this.stMaster = null;
			}
			this.bStudentNum = 0;
			if (this.astStudentList != null)
			{
				for (int i = 0; i < this.astStudentList.Length; i++)
				{
					if (this.astStudentList[i] != null)
					{
						this.astStudentList[i].Release();
						this.astStudentList[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			this.stMaster = (CSDT_FRIEND_INFO)ProtocolObjectPool.Get(CSDT_FRIEND_INFO.CLASS_ID);
			if (this.astStudentList != null)
			{
				for (int i = 0; i < this.astStudentList.Length; i++)
				{
					this.astStudentList[i] = (CSDT_FRIEND_INFO)ProtocolObjectPool.Get(CSDT_FRIEND_INFO.CLASS_ID);
				}
			}
		}
	}
}
