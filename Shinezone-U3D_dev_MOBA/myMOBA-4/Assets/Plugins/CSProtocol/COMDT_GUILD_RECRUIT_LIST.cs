using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_GUILD_RECRUIT_LIST : ProtocolObject
	{
		public int iNum;

		public COMDT_GUILD_RECRUIT_INFO[] astInfo;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 355;

		public COMDT_GUILD_RECRUIT_LIST()
		{
			this.astInfo = new COMDT_GUILD_RECRUIT_INFO[100];
			for (int i = 0; i < 100; i++)
			{
				this.astInfo[i] = (COMDT_GUILD_RECRUIT_INFO)ProtocolObjectPool.Get(COMDT_GUILD_RECRUIT_INFO.CLASS_ID);
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
			if (cutVer == 0u || COMDT_GUILD_RECRUIT_LIST.CURRVERSION < cutVer)
			{
				cutVer = COMDT_GUILD_RECRUIT_LIST.CURRVERSION;
			}
			if (COMDT_GUILD_RECRUIT_LIST.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeInt32(this.iNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (0 > this.iNum)
			{
				return TdrError.ErrorType.TDR_ERR_MINUS_REFER_VALUE;
			}
			if (100 < this.iNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astInfo.Length < this.iNum)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < this.iNum; i++)
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
			if (cutVer == 0u || COMDT_GUILD_RECRUIT_LIST.CURRVERSION < cutVer)
			{
				cutVer = COMDT_GUILD_RECRUIT_LIST.CURRVERSION;
			}
			if (COMDT_GUILD_RECRUIT_LIST.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readInt32(ref this.iNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (0 > this.iNum)
			{
				return TdrError.ErrorType.TDR_ERR_MINUS_REFER_VALUE;
			}
			if (100 < this.iNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < this.iNum; i++)
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
			return COMDT_GUILD_RECRUIT_LIST.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.iNum = 0;
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
					this.astInfo[i] = (COMDT_GUILD_RECRUIT_INFO)ProtocolObjectPool.Get(COMDT_GUILD_RECRUIT_INFO.CLASS_ID);
				}
			}
		}
	}
}
