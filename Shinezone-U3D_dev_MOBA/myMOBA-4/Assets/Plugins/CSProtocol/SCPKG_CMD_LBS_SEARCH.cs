using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class SCPKG_CMD_LBS_SEARCH : ProtocolObject
	{
		public uint dwLbsListNum;

		public CSDT_LBS_USER_INFO[] astLbsList;

		public uint dwResult;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 234u;

		public static readonly int CLASS_ID = 1051;

		public SCPKG_CMD_LBS_SEARCH()
		{
			this.astLbsList = new CSDT_LBS_USER_INFO[20];
			for (int i = 0; i < 20; i++)
			{
				this.astLbsList[i] = (CSDT_LBS_USER_INFO)ProtocolObjectPool.Get(CSDT_LBS_USER_INFO.CLASS_ID);
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
			if (cutVer == 0u || SCPKG_CMD_LBS_SEARCH.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_CMD_LBS_SEARCH.CURRVERSION;
			}
			if (SCPKG_CMD_LBS_SEARCH.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwLbsListNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (20u < this.dwLbsListNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if ((long)this.astLbsList.Length < (long)((ulong)this.dwLbsListNum))
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwLbsListNum))
			{
				errorType = this.astLbsList[num].pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				num++;
			}
			errorType = destBuf.writeUInt32(this.dwResult);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
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
			if (cutVer == 0u || SCPKG_CMD_LBS_SEARCH.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_CMD_LBS_SEARCH.CURRVERSION;
			}
			if (SCPKG_CMD_LBS_SEARCH.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwLbsListNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (20u < this.dwLbsListNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwLbsListNum))
			{
				errorType = this.astLbsList[num].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				num++;
			}
			errorType = srcBuf.readUInt32(ref this.dwResult);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return SCPKG_CMD_LBS_SEARCH.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwLbsListNum = 0u;
			if (this.astLbsList != null)
			{
				for (int i = 0; i < this.astLbsList.Length; i++)
				{
					if (this.astLbsList[i] != null)
					{
						this.astLbsList[i].Release();
						this.astLbsList[i] = null;
					}
				}
			}
			this.dwResult = 0u;
		}

		public override void OnUse()
		{
			if (this.astLbsList != null)
			{
				for (int i = 0; i < this.astLbsList.Length; i++)
				{
					this.astLbsList[i] = (CSDT_LBS_USER_INFO)ProtocolObjectPool.Get(CSDT_LBS_USER_INFO.CLASS_ID);
				}
			}
		}
	}
}
