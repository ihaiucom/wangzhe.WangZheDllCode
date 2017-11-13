using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_TRANS_CONTEXT_OF_APPROVE_JOIN_GUILD : ProtocolObject
	{
		public COMDT_GUILD_MEMBER_BRIEF_INFO stApplyPlayer;

		public ulong ullGuildID;

		public int iGuildLogicWorldID;

		public byte[] szGuildName;

		public int iRollbackFlag;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 234u;

		public static readonly uint LENGTH_szGuildName = 32u;

		public static readonly int CLASS_ID = 401;

		public COMDT_TRANS_CONTEXT_OF_APPROVE_JOIN_GUILD()
		{
			this.stApplyPlayer = (COMDT_GUILD_MEMBER_BRIEF_INFO)ProtocolObjectPool.Get(COMDT_GUILD_MEMBER_BRIEF_INFO.CLASS_ID);
			this.szGuildName = new byte[32];
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
			if (cutVer == 0u || COMDT_TRANS_CONTEXT_OF_APPROVE_JOIN_GUILD.CURRVERSION < cutVer)
			{
				cutVer = COMDT_TRANS_CONTEXT_OF_APPROVE_JOIN_GUILD.CURRVERSION;
			}
			if (COMDT_TRANS_CONTEXT_OF_APPROVE_JOIN_GUILD.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stApplyPlayer.pack(ref destBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt64(this.ullGuildID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iGuildLogicWorldID);
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
			int num = TdrTypeUtil.cstrlen(this.szGuildName);
			if (num >= 32)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
			}
			errorType = destBuf.writeCString(this.szGuildName, num);
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
			errorType = destBuf.writeInt32(this.iRollbackFlag);
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
			if (cutVer == 0u || COMDT_TRANS_CONTEXT_OF_APPROVE_JOIN_GUILD.CURRVERSION < cutVer)
			{
				cutVer = COMDT_TRANS_CONTEXT_OF_APPROVE_JOIN_GUILD.CURRVERSION;
			}
			if (COMDT_TRANS_CONTEXT_OF_APPROVE_JOIN_GUILD.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = this.stApplyPlayer.unpack(ref srcBuf, cutVer);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt64(ref this.ullGuildID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iGuildLogicWorldID);
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
			if (num > (uint)this.szGuildName.GetLength(0))
			{
				if ((ulong)num > (ulong)COMDT_TRANS_CONTEXT_OF_APPROVE_JOIN_GUILD.LENGTH_szGuildName)
				{
					return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_BIG;
				}
				this.szGuildName = new byte[num];
			}
			if (1u > num)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_TOO_SMALL;
			}
			errorType = srcBuf.readCString(ref this.szGuildName, (int)num);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (this.szGuildName[(int)(num - 1u)] != 0)
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			int num2 = TdrTypeUtil.cstrlen(this.szGuildName) + 1;
			if ((ulong)num != (ulong)((long)num2))
			{
				return TdrError.ErrorType.TDR_ERR_STR_LEN_CONFLICT;
			}
			errorType = srcBuf.readInt32(ref this.iRollbackFlag);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_TRANS_CONTEXT_OF_APPROVE_JOIN_GUILD.CLASS_ID;
		}

		public override void OnRelease()
		{
			if (this.stApplyPlayer != null)
			{
				this.stApplyPlayer.Release();
				this.stApplyPlayer = null;
			}
			this.ullGuildID = 0uL;
			this.iGuildLogicWorldID = 0;
			this.iRollbackFlag = 0;
		}

		public override void OnUse()
		{
			this.stApplyPlayer = (COMDT_GUILD_MEMBER_BRIEF_INFO)ProtocolObjectPool.Get(COMDT_GUILD_MEMBER_BRIEF_INFO.CLASS_ID);
		}
	}
}
