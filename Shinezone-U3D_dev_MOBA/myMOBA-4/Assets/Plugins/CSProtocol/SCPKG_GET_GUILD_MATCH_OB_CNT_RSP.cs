using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class SCPKG_GET_GUILD_MATCH_OB_CNT_RSP : ProtocolObject
	{
		public byte bMatchCnt;

		public COMDT_GUILD_MATCH_OB_CNT[] astMatchOBCnt;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 1607;

		public SCPKG_GET_GUILD_MATCH_OB_CNT_RSP()
		{
			this.astMatchOBCnt = new COMDT_GUILD_MATCH_OB_CNT[10];
			for (int i = 0; i < 10; i++)
			{
				this.astMatchOBCnt[i] = (COMDT_GUILD_MATCH_OB_CNT)ProtocolObjectPool.Get(COMDT_GUILD_MATCH_OB_CNT.CLASS_ID);
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
			if (cutVer == 0u || SCPKG_GET_GUILD_MATCH_OB_CNT_RSP.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_GET_GUILD_MATCH_OB_CNT_RSP.CURRVERSION;
			}
			if (SCPKG_GET_GUILD_MATCH_OB_CNT_RSP.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bMatchCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (10 < this.bMatchCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astMatchOBCnt.Length < (int)this.bMatchCnt)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.bMatchCnt; i++)
			{
				errorType = this.astMatchOBCnt[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || SCPKG_GET_GUILD_MATCH_OB_CNT_RSP.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_GET_GUILD_MATCH_OB_CNT_RSP.CURRVERSION;
			}
			if (SCPKG_GET_GUILD_MATCH_OB_CNT_RSP.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bMatchCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (10 < this.bMatchCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.bMatchCnt; i++)
			{
				errorType = this.astMatchOBCnt[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return SCPKG_GET_GUILD_MATCH_OB_CNT_RSP.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bMatchCnt = 0;
			if (this.astMatchOBCnt != null)
			{
				for (int i = 0; i < this.astMatchOBCnt.Length; i++)
				{
					if (this.astMatchOBCnt[i] != null)
					{
						this.astMatchOBCnt[i].Release();
						this.astMatchOBCnt[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astMatchOBCnt != null)
			{
				for (int i = 0; i < this.astMatchOBCnt.Length; i++)
				{
					this.astMatchOBCnt[i] = (COMDT_GUILD_MATCH_OB_CNT)ProtocolObjectPool.Get(COMDT_GUILD_MATCH_OB_CNT.CLASS_ID);
				}
			}
		}
	}
}
