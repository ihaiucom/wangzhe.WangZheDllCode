using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class CSDT_SVRHERO_LIST : ProtocolObject
	{
		public uint dwHeroNum;

		public CSDT_HERO_DETAIL[] astHeroInfoList;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 208u;

		public static readonly int CLASS_ID = 863;

		public CSDT_SVRHERO_LIST()
		{
			this.astHeroInfoList = new CSDT_HERO_DETAIL[200];
			for (int i = 0; i < 200; i++)
			{
				this.astHeroInfoList[i] = (CSDT_HERO_DETAIL)ProtocolObjectPool.Get(CSDT_HERO_DETAIL.CLASS_ID);
			}
		}

		public override TdrError.ErrorType construct()
		{
			TdrError.ErrorType errorType = TdrError.ErrorType.TDR_NO_ERROR;
			this.dwHeroNum = 0u;
			for (int i = 0; i < 200; i++)
			{
				errorType = this.astHeroInfoList[i].construct();
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
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
			if (cutVer == 0u || CSDT_SVRHERO_LIST.CURRVERSION < cutVer)
			{
				cutVer = CSDT_SVRHERO_LIST.CURRVERSION;
			}
			if (CSDT_SVRHERO_LIST.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwHeroNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (200u < this.dwHeroNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if ((long)this.astHeroInfoList.Length < (long)((ulong)this.dwHeroNum))
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwHeroNum))
			{
				errorType = this.astHeroInfoList[num].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || CSDT_SVRHERO_LIST.CURRVERSION < cutVer)
			{
				cutVer = CSDT_SVRHERO_LIST.CURRVERSION;
			}
			if (CSDT_SVRHERO_LIST.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwHeroNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (200u < this.dwHeroNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwHeroNum))
			{
				errorType = this.astHeroInfoList[num].unpack(ref srcBuf, cutVer);
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
			return CSDT_SVRHERO_LIST.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwHeroNum = 0u;
			if (this.astHeroInfoList != null)
			{
				for (int i = 0; i < this.astHeroInfoList.Length; i++)
				{
					if (this.astHeroInfoList[i] != null)
					{
						this.astHeroInfoList[i].Release();
						this.astHeroInfoList[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astHeroInfoList != null)
			{
				for (int i = 0; i < this.astHeroInfoList.Length; i++)
				{
					this.astHeroInfoList[i] = (CSDT_HERO_DETAIL)ProtocolObjectPool.Get(CSDT_HERO_DETAIL.CLASS_ID);
				}
			}
		}
	}
}
