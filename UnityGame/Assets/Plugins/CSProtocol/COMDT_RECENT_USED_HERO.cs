using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class COMDT_RECENT_USED_HERO : ProtocolObject
	{
		public uint dwCtrlMask;

		public uint dwHeroNum;

		public COMDT_RECENT_USED_HERO_INFO[] astHeroInfo;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 624;

		public COMDT_RECENT_USED_HERO()
		{
			this.astHeroInfo = new COMDT_RECENT_USED_HERO_INFO[3];
			for (int i = 0; i < 3; i++)
			{
				this.astHeroInfo[i] = (COMDT_RECENT_USED_HERO_INFO)ProtocolObjectPool.Get(COMDT_RECENT_USED_HERO_INFO.CLASS_ID);
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
			if (cutVer == 0u || COMDT_RECENT_USED_HERO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_RECENT_USED_HERO.CURRVERSION;
			}
			if (COMDT_RECENT_USED_HERO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwCtrlMask);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt32(this.dwHeroNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (3u < this.dwHeroNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if ((long)this.astHeroInfo.Length < (long)((ulong)this.dwHeroNum))
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwHeroNum))
			{
				errorType = this.astHeroInfo[num].pack(ref destBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				num++;
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
			if (cutVer == 0u || COMDT_RECENT_USED_HERO.CURRVERSION < cutVer)
			{
				cutVer = COMDT_RECENT_USED_HERO.CURRVERSION;
			}
			if (COMDT_RECENT_USED_HERO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwCtrlMask);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt32(ref this.dwHeroNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (3u < this.dwHeroNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwHeroNum))
			{
				errorType = this.astHeroInfo[num].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
				num++;
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return COMDT_RECENT_USED_HERO.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwCtrlMask = 0u;
			this.dwHeroNum = 0u;
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
					this.astHeroInfo[i] = (COMDT_RECENT_USED_HERO_INFO)ProtocolObjectPool.Get(COMDT_RECENT_USED_HERO_INFO.CLASS_ID);
				}
			}
		}
	}
}
