using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class SCPKG_GUILD_MATCH_GETINVITE_RSP : ProtocolObject
	{
		public byte bInviteNum;

		public CSDT_GUILDMATCH_INVITEINFO[] astInviteList;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 1582;

		public SCPKG_GUILD_MATCH_GETINVITE_RSP()
		{
			this.astInviteList = new CSDT_GUILDMATCH_INVITEINFO[20];
			for (int i = 0; i < 20; i++)
			{
				this.astInviteList[i] = (CSDT_GUILDMATCH_INVITEINFO)ProtocolObjectPool.Get(CSDT_GUILDMATCH_INVITEINFO.CLASS_ID);
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
			if (cutVer == 0u || SCPKG_GUILD_MATCH_GETINVITE_RSP.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_GUILD_MATCH_GETINVITE_RSP.CURRVERSION;
			}
			if (SCPKG_GUILD_MATCH_GETINVITE_RSP.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bInviteNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (20 < this.bInviteNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astInviteList.Length < (int)this.bInviteNum)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.bInviteNum; i++)
			{
				errorType = this.astInviteList[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || SCPKG_GUILD_MATCH_GETINVITE_RSP.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_GUILD_MATCH_GETINVITE_RSP.CURRVERSION;
			}
			if (SCPKG_GUILD_MATCH_GETINVITE_RSP.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bInviteNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (20 < this.bInviteNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.bInviteNum; i++)
			{
				errorType = this.astInviteList[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return SCPKG_GUILD_MATCH_GETINVITE_RSP.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bInviteNum = 0;
			if (this.astInviteList != null)
			{
				for (int i = 0; i < this.astInviteList.Length; i++)
				{
					if (this.astInviteList[i] != null)
					{
						this.astInviteList[i].Release();
						this.astInviteList[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astInviteList != null)
			{
				for (int i = 0; i < this.astInviteList.Length; i++)
				{
					this.astInviteList[i] = (CSDT_GUILDMATCH_INVITEINFO)ProtocolObjectPool.Get(CSDT_GUILDMATCH_INVITEINFO.CLASS_ID);
				}
			}
		}
	}
}
