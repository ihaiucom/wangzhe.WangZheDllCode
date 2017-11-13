using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class SCPKG_COINGETPATH_RSP : ProtocolObject
	{
		public uint dwPathNum;

		public CSDT_PATH_COIN[] astPathCoin;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 1244;

		public SCPKG_COINGETPATH_RSP()
		{
			this.astPathCoin = new CSDT_PATH_COIN[3];
			for (int i = 0; i < 3; i++)
			{
				this.astPathCoin[i] = (CSDT_PATH_COIN)ProtocolObjectPool.Get(CSDT_PATH_COIN.CLASS_ID);
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
			if (cutVer == 0u || SCPKG_COINGETPATH_RSP.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_COINGETPATH_RSP.CURRVERSION;
			}
			if (SCPKG_COINGETPATH_RSP.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwPathNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (3u < this.dwPathNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if ((long)this.astPathCoin.Length < (long)((ulong)this.dwPathNum))
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwPathNum))
			{
				errorType = this.astPathCoin[num].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || SCPKG_COINGETPATH_RSP.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_COINGETPATH_RSP.CURRVERSION;
			}
			if (SCPKG_COINGETPATH_RSP.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwPathNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (3u < this.dwPathNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwPathNum))
			{
				errorType = this.astPathCoin[num].unpack(ref srcBuf, cutVer);
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
			return SCPKG_COINGETPATH_RSP.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwPathNum = 0u;
			if (this.astPathCoin != null)
			{
				for (int i = 0; i < this.astPathCoin.Length; i++)
				{
					if (this.astPathCoin[i] != null)
					{
						this.astPathCoin[i].Release();
						this.astPathCoin[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astPathCoin != null)
			{
				for (int i = 0; i < this.astPathCoin.Length; i++)
				{
					this.astPathCoin[i] = (CSDT_PATH_COIN)ProtocolObjectPool.Get(CSDT_PATH_COIN.CLASS_ID);
				}
			}
		}
	}
}
