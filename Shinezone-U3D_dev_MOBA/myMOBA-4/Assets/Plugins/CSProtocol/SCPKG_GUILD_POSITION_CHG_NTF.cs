using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class SCPKG_GUILD_POSITION_CHG_NTF : ProtocolObject
	{
		public byte bCount;

		public COMDT_MEMBER_POSITION[] astChgInfo;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 1347;

		public SCPKG_GUILD_POSITION_CHG_NTF()
		{
			this.astChgInfo = new COMDT_MEMBER_POSITION[8];
			for (int i = 0; i < 8; i++)
			{
				this.astChgInfo[i] = (COMDT_MEMBER_POSITION)ProtocolObjectPool.Get(COMDT_MEMBER_POSITION.CLASS_ID);
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
			if (cutVer == 0u || SCPKG_GUILD_POSITION_CHG_NTF.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_GUILD_POSITION_CHG_NTF.CURRVERSION;
			}
			if (SCPKG_GUILD_POSITION_CHG_NTF.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bCount);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (8 < this.bCount)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astChgInfo.Length < (int)this.bCount)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.bCount; i++)
			{
				errorType = this.astChgInfo[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || SCPKG_GUILD_POSITION_CHG_NTF.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_GUILD_POSITION_CHG_NTF.CURRVERSION;
			}
			if (SCPKG_GUILD_POSITION_CHG_NTF.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bCount);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (8 < this.bCount)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.bCount; i++)
			{
				errorType = this.astChgInfo[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return SCPKG_GUILD_POSITION_CHG_NTF.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bCount = 0;
			if (this.astChgInfo != null)
			{
				for (int i = 0; i < this.astChgInfo.Length; i++)
				{
					if (this.astChgInfo[i] != null)
					{
						this.astChgInfo[i].Release();
						this.astChgInfo[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astChgInfo != null)
			{
				for (int i = 0; i < this.astChgInfo.Length; i++)
				{
					this.astChgInfo[i] = (COMDT_MEMBER_POSITION)ProtocolObjectPool.Get(COMDT_MEMBER_POSITION.CLASS_ID);
				}
			}
		}
	}
}
