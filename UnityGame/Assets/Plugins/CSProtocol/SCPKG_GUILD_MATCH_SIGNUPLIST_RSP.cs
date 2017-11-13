using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class SCPKG_GUILD_MATCH_SIGNUPLIST_RSP : ProtocolObject
	{
		public byte bSingUpNum;

		public CSDT_GUILDMATCH_SIGNUPINFO[] astSingUpList;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 1575;

		public SCPKG_GUILD_MATCH_SIGNUPLIST_RSP()
		{
			this.astSingUpList = new CSDT_GUILDMATCH_SIGNUPINFO[150];
			for (int i = 0; i < 150; i++)
			{
				this.astSingUpList[i] = (CSDT_GUILDMATCH_SIGNUPINFO)ProtocolObjectPool.Get(CSDT_GUILDMATCH_SIGNUPINFO.CLASS_ID);
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
			if (cutVer == 0u || SCPKG_GUILD_MATCH_SIGNUPLIST_RSP.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_GUILD_MATCH_SIGNUPLIST_RSP.CURRVERSION;
			}
			if (SCPKG_GUILD_MATCH_SIGNUPLIST_RSP.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bSingUpNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (150 < this.bSingUpNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astSingUpList.Length < (int)this.bSingUpNum)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.bSingUpNum; i++)
			{
				errorType = this.astSingUpList[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || SCPKG_GUILD_MATCH_SIGNUPLIST_RSP.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_GUILD_MATCH_SIGNUPLIST_RSP.CURRVERSION;
			}
			if (SCPKG_GUILD_MATCH_SIGNUPLIST_RSP.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bSingUpNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (150 < this.bSingUpNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.bSingUpNum; i++)
			{
				errorType = this.astSingUpList[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return SCPKG_GUILD_MATCH_SIGNUPLIST_RSP.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bSingUpNum = 0;
			if (this.astSingUpList != null)
			{
				for (int i = 0; i < this.astSingUpList.Length; i++)
				{
					if (this.astSingUpList[i] != null)
					{
						this.astSingUpList[i].Release();
						this.astSingUpList[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astSingUpList != null)
			{
				for (int i = 0; i < this.astSingUpList.Length; i++)
				{
					this.astSingUpList[i] = (CSDT_GUILDMATCH_SIGNUPINFO)ProtocolObjectPool.Get(CSDT_GUILDMATCH_SIGNUPINFO.CLASS_ID);
				}
			}
		}
	}
}
