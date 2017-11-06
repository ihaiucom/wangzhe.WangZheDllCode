using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class SCPKG_GET_GUILD_RECOMMEND_LIST_RSP : ProtocolObject
	{
		public uint dwTotalCnt;

		public byte bPageId;

		public byte bCount;

		public COMDT_GUILD_RECOMMEND_INFO[] astRecommendInfo;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 1339;

		public SCPKG_GET_GUILD_RECOMMEND_LIST_RSP()
		{
			this.astRecommendInfo = new COMDT_GUILD_RECOMMEND_INFO[25];
			for (int i = 0; i < 25; i++)
			{
				this.astRecommendInfo[i] = (COMDT_GUILD_RECOMMEND_INFO)ProtocolObjectPool.Get(COMDT_GUILD_RECOMMEND_INFO.CLASS_ID);
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
			if (cutVer == 0u || SCPKG_GET_GUILD_RECOMMEND_LIST_RSP.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_GET_GUILD_RECOMMEND_LIST_RSP.CURRVERSION;
			}
			if (SCPKG_GET_GUILD_RECOMMEND_LIST_RSP.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwTotalCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bPageId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bCount);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (25 < this.bCount)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astRecommendInfo.Length < (int)this.bCount)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.bCount; i++)
			{
				errorType = this.astRecommendInfo[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || SCPKG_GET_GUILD_RECOMMEND_LIST_RSP.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_GET_GUILD_RECOMMEND_LIST_RSP.CURRVERSION;
			}
			if (SCPKG_GET_GUILD_RECOMMEND_LIST_RSP.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwTotalCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bPageId);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bCount);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (25 < this.bCount)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.bCount; i++)
			{
				errorType = this.astRecommendInfo[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return SCPKG_GET_GUILD_RECOMMEND_LIST_RSP.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwTotalCnt = 0u;
			this.bPageId = 0;
			this.bCount = 0;
			if (this.astRecommendInfo != null)
			{
				for (int i = 0; i < this.astRecommendInfo.Length; i++)
				{
					if (this.astRecommendInfo[i] != null)
					{
						this.astRecommendInfo[i].Release();
						this.astRecommendInfo[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astRecommendInfo != null)
			{
				for (int i = 0; i < this.astRecommendInfo.Length; i++)
				{
					this.astRecommendInfo[i] = (COMDT_GUILD_RECOMMEND_INFO)ProtocolObjectPool.Get(COMDT_GUILD_RECOMMEND_INFO.CLASS_ID);
				}
			}
		}
	}
}
