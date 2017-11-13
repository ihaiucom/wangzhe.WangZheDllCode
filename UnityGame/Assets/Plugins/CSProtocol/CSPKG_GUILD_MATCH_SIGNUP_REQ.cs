using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class CSPKG_GUILD_MATCH_SIGNUP_REQ : ProtocolObject
	{
		public byte bPos1;

		public byte bPos2;

		public byte[] szBeiZhu;

		public byte bHeroNum;

		public COMDT_GUILDMATCH_SIGNUP_HEROINFO[] astHeroInfo;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly uint LENGTH_szBeiZhu = 256u;

		public static readonly int CLASS_ID = 1576;

		public CSPKG_GUILD_MATCH_SIGNUP_REQ()
		{
			this.szBeiZhu = new byte[256];
			this.astHeroInfo = new COMDT_GUILDMATCH_SIGNUP_HEROINFO[3];
			for (int i = 0; i < 3; i++)
			{
				this.astHeroInfo[i] = (COMDT_GUILDMATCH_SIGNUP_HEROINFO)ProtocolObjectPool.Get(COMDT_GUILDMATCH_SIGNUP_HEROINFO.CLASS_ID);
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
			if (cutVer == 0u || CSPKG_GUILD_MATCH_SIGNUP_REQ.CURRVERSION < cutVer)
			{
				cutVer = CSPKG_GUILD_MATCH_SIGNUP_REQ.CURRVERSION;
			}
			if (CSPKG_GUILD_MATCH_SIGNUP_REQ.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bPos1);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bPos2);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int usedSize = destBuf.getUsedSize();
			errorType = destBuf.reserve(4);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int usedSize2 = destBuf.getUsedSize();
			int num = TdrTypeUtil.cstrlen(this.szBeiZhu);
			if (num >= 256)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
			}
			errorType = destBuf.writeCString(this.szBeiZhu, num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(0);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			int src = destBuf.getUsedSize() - usedSize2;
			errorType = destBuf.writeUInt32((uint)src, usedSize);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bHeroNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (3 < this.bHeroNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astHeroInfo.Length < (int)this.bHeroNum)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.bHeroNum; i++)
			{
				errorType = this.astHeroInfo[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || CSPKG_GUILD_MATCH_SIGNUP_REQ.CURRVERSION < cutVer)
			{
				cutVer = CSPKG_GUILD_MATCH_SIGNUP_REQ.CURRVERSION;
			}
			if (CSPKG_GUILD_MATCH_SIGNUP_REQ.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bPos1);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bPos2);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			uint num = 0u;
			errorType = srcBuf.readUInt32(ref num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (num > (uint)srcBuf.getLeftSize())
			{
				return TdrError.ErrorType.TDR_ERR_SHORT_BUF_FOR_READ;
			}
			if (num > (uint)this.szBeiZhu.GetLength(0))
			{
				if ((ulong)num > (ulong)CSPKG_GUILD_MATCH_SIGNUP_REQ.LENGTH_szBeiZhu)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szBeiZhu = new byte[num];
			}
			if (1u > num)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szBeiZhu, (int)num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szBeiZhu[(int)(num - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num2 = TdrTypeUtil.cstrlen(this.szBeiZhu) + 1;
			if ((ulong)num != (ulong)((long)num2))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readUInt8(ref this.bHeroNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (3 < this.bHeroNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.bHeroNum; i++)
			{
				errorType = this.astHeroInfo[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return CSPKG_GUILD_MATCH_SIGNUP_REQ.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bPos1 = 0;
			this.bPos2 = 0;
			this.bHeroNum = 0;
			if (this.astHeroInfo != null)
			{
				for (int i = 0; i < this.astHeroInfo.Length; i++)
				{
					if (this.astHeroInfo[i] != null)
					{
						this.astHeroInfo[i].Release();
						this.astHeroInfo[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astHeroInfo != null)
			{
				for (int i = 0; i < this.astHeroInfo.Length; i++)
				{
					this.astHeroInfo[i] = (COMDT_GUILDMATCH_SIGNUP_HEROINFO)ProtocolObjectPool.Get(COMDT_GUILDMATCH_SIGNUP_HEROINFO.CLASS_ID);
				}
			}
		}
	}
}
