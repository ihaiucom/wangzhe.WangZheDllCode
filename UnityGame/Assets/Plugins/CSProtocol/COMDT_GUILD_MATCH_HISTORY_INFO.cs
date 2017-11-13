using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_GUILD_MATCH_HISTORY_INFO : ProtocolObject
	{
		public uint dwMatchTime;

		public int iScore;

		public byte bMemNum;

		public COMDT_GUILD_MATCH_HISTORY_MEMBER_INFO[] astMemInfo;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 620;

		public COMDT_GUILD_MATCH_HISTORY_INFO()
		{
			this.astMemInfo = new COMDT_GUILD_MATCH_HISTORY_MEMBER_INFO[5];
			for (int i = 0; i < 5; i++)
			{
				this.astMemInfo[i] = (COMDT_GUILD_MATCH_HISTORY_MEMBER_INFO)ProtocolObjectPool.Get(COMDT_GUILD_MATCH_HISTORY_MEMBER_INFO.CLASS_ID);
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
			if (cutVer == 0u || COMDT_GUILD_MATCH_HISTORY_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_GUILD_MATCH_HISTORY_INFO.CURRVERSION;
			}
			if (COMDT_GUILD_MATCH_HISTORY_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwMatchTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeInt32(this.iScore);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt8(this.bMemNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (5 < this.bMemNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astMemInfo.Length < (int)this.bMemNum)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.bMemNum; i++)
			{
				errorType = this.astMemInfo[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || COMDT_GUILD_MATCH_HISTORY_INFO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_GUILD_MATCH_HISTORY_INFO.CURRVERSION;
			}
			if (COMDT_GUILD_MATCH_HISTORY_INFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwMatchTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readInt32(ref this.iScore);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt8(ref this.bMemNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (5 < this.bMemNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.bMemNum; i++)
			{
				errorType = this.astMemInfo[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_GUILD_MATCH_HISTORY_INFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwMatchTime = 0u;
			this.iScore = 0;
			this.bMemNum = 0;
			if (this.astMemInfo != null)
			{
				for (int i = 0; i < this.astMemInfo.Length; i++)
				{
					if (this.astMemInfo[i] != null)
					{
						this.astMemInfo[i].Release();
						this.astMemInfo[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astMemInfo != null)
			{
				for (int i = 0; i < this.astMemInfo.Length; i++)
				{
					this.astMemInfo[i] = (COMDT_GUILD_MATCH_HISTORY_MEMBER_INFO)ProtocolObjectPool.Get(COMDT_GUILD_MATCH_HISTORY_MEMBER_INFO.CLASS_ID);
				}
			}
		}
	}
}
