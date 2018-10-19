using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class SCPKG_GET_PREPARE_GUILD_LIST_RSP : ProtocolObject
	{
		public byte bPageID;

		public uint dwTotalCnt;

		public byte bGuildNum;

		public COMDT_PREPARE_GUILD_BRIEF_INFO[] astGuildList;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 234u;

		public static readonly int CLASS_ID = 1305;

		public SCPKG_GET_PREPARE_GUILD_LIST_RSP()
		{
			this.astGuildList = new COMDT_PREPARE_GUILD_BRIEF_INFO[10];
			for (int i = 0; i < 10; i++)
			{
				this.astGuildList[i] = (COMDT_PREPARE_GUILD_BRIEF_INFO)ProtocolObjectPool.Get(COMDT_PREPARE_GUILD_BRIEF_INFO.CLASS_ID);
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
			if (cutVer == 0u || SCPKG_GET_PREPARE_GUILD_LIST_RSP.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_GET_PREPARE_GUILD_LIST_RSP.CURRVERSION;
			}
			if (SCPKG_GET_PREPARE_GUILD_LIST_RSP.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt8(this.bPageID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwTotalCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bGuildNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (10 < this.bGuildNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astGuildList.Length < (int)this.bGuildNum)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.bGuildNum; i++)
			{
				errorType = this.astGuildList[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || SCPKG_GET_PREPARE_GUILD_LIST_RSP.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_GET_PREPARE_GUILD_LIST_RSP.CURRVERSION;
			}
			if (SCPKG_GET_PREPARE_GUILD_LIST_RSP.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt8(ref this.bPageID);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwTotalCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bGuildNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (10 < this.bGuildNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.bGuildNum; i++)
			{
				errorType = this.astGuildList[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return SCPKG_GET_PREPARE_GUILD_LIST_RSP.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.bPageID = 0;
			this.dwTotalCnt = 0u;
			this.bGuildNum = 0;
			if (this.astGuildList != null)
			{
				for (int i = 0; i < this.astGuildList.Length; i++)
				{
					if (this.astGuildList[i] != null)
					{
						this.astGuildList[i].Release();
						this.astGuildList[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astGuildList != null)
			{
				for (int i = 0; i < this.astGuildList.Length; i++)
				{
					this.astGuildList[i] = (COMDT_PREPARE_GUILD_BRIEF_INFO)ProtocolObjectPool.Get(COMDT_PREPARE_GUILD_BRIEF_INFO.CLASS_ID);
				}
			}
		}
	}
}
