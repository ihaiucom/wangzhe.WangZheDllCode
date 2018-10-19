using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class SCPKG_SET_GUILD_MATCH_READY_NTF : ProtocolObject
	{
		public byte bCnt;

		public COMDT_GUILD_MATCH_MEMBER_ISREADY_CHG[] astInfo;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 1597;

		public SCPKG_SET_GUILD_MATCH_READY_NTF()
		{
			this.astInfo = new COMDT_GUILD_MATCH_MEMBER_ISREADY_CHG[5];
			for (int i = 0; i < 5; i++)
			{
				this.astInfo[i] = (COMDT_GUILD_MATCH_MEMBER_ISREADY_CHG)ProtocolObjectPool.Get(COMDT_GUILD_MATCH_MEMBER_ISREADY_CHG.CLASS_ID);
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
			if (cutVer == 0u || SCPKG_SET_GUILD_MATCH_READY_NTF.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_SET_GUILD_MATCH_READY_NTF.CURRVERSION;
			}
			if (SCPKG_SET_GUILD_MATCH_READY_NTF.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (5 < this.bCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astInfo.Length < (int)this.bCnt)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.bCnt; i++)
			{
				errorType = this.astInfo[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || SCPKG_SET_GUILD_MATCH_READY_NTF.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_SET_GUILD_MATCH_READY_NTF.CURRVERSION;
			}
			if (SCPKG_SET_GUILD_MATCH_READY_NTF.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (5 < this.bCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.bCnt; i++)
			{
				errorType = this.astInfo[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return SCPKG_SET_GUILD_MATCH_READY_NTF.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bCnt = 0;
			if (this.astInfo != null)
			{
				for (int i = 0; i < this.astInfo.Length; i++)
				{
					if (this.astInfo[i] != null)
					{
						this.astInfo[i].Release();
						this.astInfo[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astInfo != null)
			{
				for (int i = 0; i < this.astInfo.Length; i++)
				{
					this.astInfo[i] = (COMDT_GUILD_MATCH_MEMBER_ISREADY_CHG)ProtocolObjectPool.Get(COMDT_GUILD_MATCH_MEMBER_ISREADY_CHG.CLASS_ID);
				}
			}
		}
	}
}
