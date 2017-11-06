using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class SCPKG_GET_GUILD_MATCH_HISTORY_RSP : ProtocolObject
	{
		public byte bMatchNum;

		public COMDT_GUILD_MATCH_HISTORY_INFO[] astMatchInfo;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 1603;

		public SCPKG_GET_GUILD_MATCH_HISTORY_RSP()
		{
			this.astMatchInfo = new COMDT_GUILD_MATCH_HISTORY_INFO[40];
			for (int i = 0; i < 40; i++)
			{
				this.astMatchInfo[i] = (COMDT_GUILD_MATCH_HISTORY_INFO)ProtocolObjectPool.Get(COMDT_GUILD_MATCH_HISTORY_INFO.CLASS_ID);
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
			if (cutVer == 0u || SCPKG_GET_GUILD_MATCH_HISTORY_RSP.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_GET_GUILD_MATCH_HISTORY_RSP.CURRVERSION;
			}
			if (SCPKG_GET_GUILD_MATCH_HISTORY_RSP.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bMatchNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (40 < this.bMatchNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astMatchInfo.Length < (int)this.bMatchNum)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.bMatchNum; i++)
			{
				errorType = this.astMatchInfo[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || SCPKG_GET_GUILD_MATCH_HISTORY_RSP.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_GET_GUILD_MATCH_HISTORY_RSP.CURRVERSION;
			}
			if (SCPKG_GET_GUILD_MATCH_HISTORY_RSP.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bMatchNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (40 < this.bMatchNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.bMatchNum; i++)
			{
				errorType = this.astMatchInfo[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return SCPKG_GET_GUILD_MATCH_HISTORY_RSP.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bMatchNum = 0;
			if (this.astMatchInfo != null)
			{
				for (int i = 0; i < this.astMatchInfo.Length; i++)
				{
					if (this.astMatchInfo[i] != null)
					{
						this.astMatchInfo[i].Release();
						this.astMatchInfo[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astMatchInfo != null)
			{
				for (int i = 0; i < this.astMatchInfo.Length; i++)
				{
					this.astMatchInfo[i] = (COMDT_GUILD_MATCH_HISTORY_INFO)ProtocolObjectPool.Get(COMDT_GUILD_MATCH_HISTORY_INFO.CLASS_ID);
				}
			}
		}
	}
}
