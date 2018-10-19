using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class CSDT_RECONN_CAMPPICKINFO : ProtocolObject
	{
		public uint dwPlayerNum;

		public CSDT_RECONN_PLAYERPICKINFO[] astPlayerInfo;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 241u;

		public static readonly int CLASS_ID = 1273;

		public CSDT_RECONN_CAMPPICKINFO()
		{
			this.astPlayerInfo = new CSDT_RECONN_PLAYERPICKINFO[5];
			for (int i = 0; i < 5; i++)
			{
				this.astPlayerInfo[i] = (CSDT_RECONN_PLAYERPICKINFO)ProtocolObjectPool.Get(CSDT_RECONN_PLAYERPICKINFO.CLASS_ID);
			}
		}

		public override TdrError.ErrorType construct()
		{
			TdrError.ErrorType errorType = TdrError.ErrorType.TDR_NO_ERROR;
			this.dwPlayerNum = 0u;
			for (int i = 0; i < 5; i++)
			{
				errorType = this.astPlayerInfo[i].construct();
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
			if (cutVer == 0u || CSDT_RECONN_CAMPPICKINFO.CURRVERSION < cutVer)
			{
				cutVer = CSDT_RECONN_CAMPPICKINFO.CURRVERSION;
			}
			if (CSDT_RECONN_CAMPPICKINFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwPlayerNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (5u < this.dwPlayerNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if ((long)this.astPlayerInfo.Length < (long)((ulong)this.dwPlayerNum))
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwPlayerNum))
			{
				errorType = this.astPlayerInfo[num].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || CSDT_RECONN_CAMPPICKINFO.CURRVERSION < cutVer)
			{
				cutVer = CSDT_RECONN_CAMPPICKINFO.CURRVERSION;
			}
			if (CSDT_RECONN_CAMPPICKINFO.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwPlayerNum);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (5u < this.dwPlayerNum)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			int num = 0;
			while ((long)num < (long)((ulong)this.dwPlayerNum))
			{
				errorType = this.astPlayerInfo[num].unpack(ref srcBuf, cutVer);
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
			return CSDT_RECONN_CAMPPICKINFO.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwPlayerNum = 0u;
			if (this.astPlayerInfo != null)
			{
				for (int i = 0; i < this.astPlayerInfo.Length; i++)
				{
					if (this.astPlayerInfo[i] != null)
					{
						this.astPlayerInfo[i].Release();
						this.astPlayerInfo[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astPlayerInfo != null)
			{
				for (int i = 0; i < this.astPlayerInfo.Length; i++)
				{
					this.astPlayerInfo[i] = (CSDT_RECONN_PLAYERPICKINFO)ProtocolObjectPool.Get(CSDT_RECONN_PLAYERPICKINFO.CLASS_ID);
				}
			}
		}
	}
}
