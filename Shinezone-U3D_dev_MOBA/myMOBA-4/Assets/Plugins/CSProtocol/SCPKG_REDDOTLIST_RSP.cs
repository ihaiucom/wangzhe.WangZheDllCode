using Assets.Scripts.Common;
using System;
using tsf4g_tdr_csharp;

namespace CSProtocol
{
	public class SCPKG_REDDOTLIST_RSP : ProtocolObject
	{
		public uint dwGetTime;

		public ushort wRedDotCnt;

		public COMDT_REDDOT_INFO[] astRedDotList;

		public static readonly uint BASEVERSION = 1u;

		public static readonly uint CURRVERSION = 1u;

		public static readonly int CLASS_ID = 1169;

		public SCPKG_REDDOTLIST_RSP()
		{
			this.astRedDotList = new COMDT_REDDOT_INFO[100];
			for (int i = 0; i < 100; i++)
			{
				this.astRedDotList[i] = (COMDT_REDDOT_INFO)ProtocolObjectPool.Get(COMDT_REDDOT_INFO.CLASS_ID);
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
			if (cutVer == 0u || SCPKG_REDDOTLIST_RSP.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_REDDOTLIST_RSP.CURRVERSION;
			}
			if (SCPKG_REDDOTLIST_RSP.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = destBuf.writeUInt32(this.dwGetTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = destBuf.writeUInt16(this.wRedDotCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (100 < this.wRedDotCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			if (this.astRedDotList.Length < (int)this.wRedDotCnt)
			{
				return TdrError.ErrorType.TDR_ERR_VAR_ARRAY_CONFLICT;
			}
			for (int i = 0; i < (int)this.wRedDotCnt; i++)
			{
				errorType = this.astRedDotList[i].pack(ref destBuf, cutVer);
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
			if (cutVer == 0u || SCPKG_REDDOTLIST_RSP.CURRVERSION < cutVer)
			{
				cutVer = SCPKG_REDDOTLIST_RSP.CURRVERSION;
			}
			if (SCPKG_REDDOTLIST_RSP.BASEVERSION > cutVer)
			{
				return TdrError.ErrorType.TDR_ERR_CUTVER_TOO_SMALL;
			}
			TdrError.ErrorType errorType = srcBuf.readUInt32(ref this.dwGetTime);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			errorType = srcBuf.readUInt16(ref this.wRedDotCnt);
			if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
			{
				return errorType;
			}
			if (100 < this.wRedDotCnt)
			{
				return TdrError.ErrorType.TDR_ERR_REFER_SURPASS_COUNT;
			}
			for (int i = 0; i < (int)this.wRedDotCnt; i++)
			{
				errorType = this.astRedDotList[i].unpack(ref srcBuf, cutVer);
				if (errorType != TdrError.ErrorType.TDR_NO_ERROR)
				{
					return errorType;
				}
			}
			return errorType;
		}

		public override int GetClassID()
		{
			return SCPKG_REDDOTLIST_RSP.CLASS_ID;
		}

		public override void OnRelease()
		{
			this.dwGetTime = 0u;
			this.wRedDotCnt = 0;
			if (this.astRedDotList != null)
			{
				for (int i = 0; i < this.astRedDotList.Length; i++)
				{
					if (this.astRedDotList[i] != null)
					{
						this.astRedDotList[i].Release();
						this.astRedDotList[i] = null;
					}
				}
			}
		}

		public override void OnUse()
		{
			if (this.astRedDotList != null)
			{
				for (int i = 0; i < this.astRedDotList.Length; i++)
				{
					this.astRedDotList[i] = (COMDT_REDDOT_INFO)ProtocolObjectPool.Get(COMDT_REDDOT_INFO.CLASS_ID);
				}
			}
		}
	}
}
